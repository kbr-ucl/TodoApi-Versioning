using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace TodoApplication.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/values")]
    public class ValuesV1Controller : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] {"v1-value1", "v1-value2"};
        }
    }

    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/values")]
    public class ValuesV2Controller : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] {"v2-value1", "v2-value2"};
        }
    }
}