using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebTutorialRabbitMQ.Model;

namespace WebTutorialRabbitMQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        // GET api/values
        [HttpGet]
        public ActionResult<Logs> Get()
        {
            var rabbitMQ = new MQ.QueueRabbitMQ();
            return rabbitMQ.Receive<Logs>("Logs_ITFC");
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<Logs> Get(int id)
        {
            var rabbitMQ = new MQ.QueueRabbitMQ();
            return rabbitMQ.ReceiveQueue<Logs>("Logs_ITFC");
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] Logs value)
        {
            var rabbitMQ = new MQ.QueueRabbitMQ();
            rabbitMQ.SendQueue<Logs>(value, "Logs_ITFC");
            rabbitMQ.SendExchange<Logs>(value, "Logs_ITFC");
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
