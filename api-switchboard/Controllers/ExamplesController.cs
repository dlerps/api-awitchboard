using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace api_switchboard.Controllers
{
    /// <summary>
    /// This is just an example controller which can be deleted. For your own use you
    /// might wanna create new controllers for different incoming calls.
    /// </summary>
    [Route("api/[controller]")]
    public class ExamplesController : Controller
    {
        // POST api/examples
        [HttpPost]
        public void Post([FromBody]string value)
        {
            Dictionary<string, object> valuePairs = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
            // your connector code here
            // IApiConnector connector = new YourImplementionConnector();
            // await connector.Connect(valuePairs);
        }

        // PUT api/examples/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
            // your connector code here
        }
    }
}
