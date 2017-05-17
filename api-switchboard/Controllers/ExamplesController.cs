using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace api_switchboard.Controllers
{
    [Route("api/[controller]")]
    public class ExamplesController : Controller
    {
        // POST api/examples
        [HttpPost]
        public void Post([FromBody]string value)
        {
            // your connector code here
        }

        // PUT api/examples/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
            // your connector code here
        }
    }
}
