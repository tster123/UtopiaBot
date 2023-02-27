using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Fluent;
using NLog.Web;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UtopiaListener.Listener
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntelController : ControllerBase
    {
        private static Logger log = LogManager.GetCurrentClassLogger();


        // GET: api/<IntelController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            log.Info("Get");
            return new string[] { "value1", "value2" };
        }

        // GET api/<IntelController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<IntelController>
        [HttpPost]
        [Consumes("application/json")]
        public void Post([FromBody] string value)
        {
            log.Info("POST: " + value);
        }

        // PUT api/<IntelController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<IntelController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
