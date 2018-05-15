using Microsoft.AspNetCore.Mvc;

namespace dotnetcore_demo.Controllers
{
    // [MiddlewareFilter(typeof(FirstMiddleware))]
    public class HomeController : Controller
    {
        // [MiddlewareFilter(typeof(SecondMiddleware))]
        public IActionResult Index()
        {
            return Content("a");
        }
    }
}