using dotnetcore_demo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace dotnetcore_demo.Controllers
{
    public class ModelBindingController : Controller
    {
        public IActionResult Index(int id)
        {
            return Content($"id: {id}");
        }

        public IActionResult FirstSample(
            [FromHeader] string header,
            [FromForm] string form,
            [FromRoute] string id,
            [FromQuery] string query)
        {
            return Content($"header: {header}, form: {form}, id: {id}, query: {query}");
        }

        public IActionResult DISample([FromServices] ILogger<HomeController> logger)
        {
            return Content($"logger is null? {logger == null}");
        }

        public IActionResult BodySample([FromBody] UserModel model)
        {
            if(model.Id < 1)
            {
                ModelState.AddModelError("Id", "Id not exist");
            }
            if (ModelState.IsValid)
            {
                return Ok(model);
            }
            return BadRequest(ModelState);
        }
    }


}