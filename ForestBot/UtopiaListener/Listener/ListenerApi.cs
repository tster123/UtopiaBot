using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UtopiaListener.Listener
{
    [Route("listener/[controller]")]
    [ApiController]
    public class ListenerApi : ControllerBase
    {
        // GET: api/<Listener>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<Listener>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<Listener>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<Listener>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Listener>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
