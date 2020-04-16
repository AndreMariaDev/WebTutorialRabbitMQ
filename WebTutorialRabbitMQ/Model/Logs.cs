using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTutorialRabbitMQ.Model
{
    public class Logs
    {
        public String Name { get; set; }
        public String Message { get; set; }
        public DateTime Data { get; set; }
        public String Application { get; set; }
    }
}
