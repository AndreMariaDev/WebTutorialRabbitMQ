using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Remotion.Linq.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebTutorialRabbitMQ.MQ
{
    public class QueueRabbitMQ
    {
        private static IConfiguration _configuration;
        private RabbitMQConfigurations rabbitMQConfigurations;
        public QueueRabbitMQ()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json");
            _configuration = builder.Build();

            this.rabbitMQConfigurations = new RabbitMQConfigurations();
            new ConfigureFromConfigurationOptions<RabbitMQConfigurations>(
                _configuration.GetSection("RabbitMQConfigurations"))
                    .Configure(rabbitMQConfigurations);

        }

        public ConnectionFactory Factory()
        {
            return new ConnectionFactory()
            {
                HostName = rabbitMQConfigurations.HostName,
                Port = rabbitMQConfigurations.Port,
                UserName = rabbitMQConfigurations.UserName,
                Password = rabbitMQConfigurations.Password
            };
        }

        public void SendQueue<T>(T entity, string queueName)
        {
            try
            {
                using (var connection = this.Factory().CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entity));

                    channel.BasicPublish(exchange: "",
                                         routingKey: queueName,
                                         basicProperties: null,
                                         body: body);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SendExchange<T>(T entity, string queueName) 
        {
            var factory = this.Factory();
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "direct_itfc",
                                        type: "direct");

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entity));
                channel.BasicPublish(exchange: "direct_itfc",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);
            }
        }

        public T ReceiveQueue<T>(string queueName)
        {
            var factory = this.Factory();
            factory.DispatchConsumersAsync = true;
            var message = "";
            try
            {
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        using (var signal = new ManualResetEvent(false))
                        {
                            var consumer = new AsyncEventingBasicConsumer(channel);
                            consumer.Received += async (sender, args) => {
                                message = Encoding.UTF8.GetString(args.Body);
                                signal.Set();
                            };

                            channel.BasicConsume(queueName, false, consumer);
                            bool timeout = !signal.WaitOne(TimeSpan.FromSeconds(10));
                            channel.BasicCancel(consumer.ConsumerTag);
                            if (timeout)
                            {
                                throw new Exception("timeout");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return !String.IsNullOrEmpty(message)? JsonConvert.DeserializeObject<T>(message): default(T);
        }

        public T Receive<T>(string queueName) 
        {
            var factory = this.Factory();
            factory.DispatchConsumersAsync = true;
            var message = "";
            try
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    using (var signal = new ManualResetEvent(false))
                    {
                        channel.ExchangeDeclare(exchange: "direct_itfc", type: "direct");
                        var queueNameNew = channel.QueueDeclare().QueueName;

                        channel.QueueBind(queue: queueNameNew, exchange: "direct_itfc", routingKey: queueName);
                        var consumer = new AsyncEventingBasicConsumer(channel);
                        consumer.Received += async (sender, args) =>
                        {
                            message = Encoding.UTF8.GetString(args.Body);
                            signal.Set();
                        };
                        var result = channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                        bool timeout = !signal.WaitOne(TimeSpan.FromSeconds(10));
                        if (timeout)
                        {
                            message = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return !String.IsNullOrEmpty(message) ? JsonConvert.DeserializeObject<T>(message) : default(T);
        }

        private static async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            var message = Encoding.UTF8.GetString(@event.Body);

            Console.WriteLine($"Begin processing {message}");

            await Task.Delay(250);

            Console.WriteLine($"End processing {message}");
        }
    }

    public class MessageReceiver : DefaultBasicConsumer 
    {
        private readonly IModel _channel;
        public MessageReceiver(IModel channel)
        {
            _channel = channel;
        }
        public String Message { get; set; }
        public override void HandleBasicDeliver(
            string consumerTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties properties,
            byte[] body)
        {
            //Console.WriteLine($"Consuming Message");
            //Console.WriteLine(string.Concat("Message received from the exchange ", exchange));
            //Console.WriteLine(string.Concat("Consumer tag: ", consumerTag));
            //Console.WriteLine(string.Concat("Delivery tag: ", deliveryTag));
            //Console.WriteLine(string.Concat("Routing tag: ", routingKey));
            //Console.WriteLine(string.Concat("Message: ", Encoding.UTF8.GetString(body)));
            this.Message = Encoding.UTF8.GetString(body);
            _channel.BasicAck(deliveryTag, false);
        }
    }
}
