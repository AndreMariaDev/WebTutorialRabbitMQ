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
    public class BenjaminController : ControllerBase
    {
        [HttpGet]
        public ActionResult<LotesEletronicos> Get()
        {
            var rabbitMQ = new MQ.QueueRabbitMQ();
            return rabbitMQ.Receive<LotesEletronicos>("Benjamin_Start"); 
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<LotesEletronicos> Get(int id)
        {
            var rabbitMQ = new MQ.QueueRabbitMQ();
            return rabbitMQ.ReceiveQueue<LotesEletronicos>("Benjamin_Start");
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] LotesEletronicos value)
        {
            var rabbitMQ = new MQ.QueueRabbitMQ();
            rabbitMQ.SendQueue<LotesEletronicos>(value, "Benjamin_Start");
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
