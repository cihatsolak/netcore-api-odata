using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;

namespace OData.API.Controllers
{
    public class HelpersController : ODataController
    {
        [HttpGet]
        [ODataRoute("GetKdv")]
        public IActionResult GetKdv()
        {
            return Ok(18);
        }
    }
}
