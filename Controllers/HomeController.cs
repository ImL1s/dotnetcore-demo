using dotnetcore_demo.Model;
using Microsoft.AspNetCore.Mvc;

namespace dotnetcore_demo.Controllers
{
    // [MiddlewareFilter(typeof(FirstMiddleware))]
    public class HomeController : Controller
    {
        // [MiddlewareFilter(typeof(SecondMiddleware))]
        public IActionResult Index()
        {
            var model = new HomeModel()
            {
                Name = "Sam"
            };
            return View(model);
            // return Content("a");

            // IActionResult 回傳的方式可以有很多種
            // 透過繼承 Controller 後，就可以使用 Controller 的方法:

            // [View]
            // View() 找到該 Controller & Action 對應的 *.cshtml， 並且把 UserModel 傳給 View 使用

            // [HTTP Status Code]
            // StatusCode({code}) 回應包含 HTTP Status。常用的回應有 Ok、BadRequest、NotFound 等
            // return StatusCode(200); | 回傳指定的code
            // BadRequest("Server are to busy"); | 回傳400

            // [Redirect]
            // return Redirect({urlString});
            // RedirectToAction({actionName});
            // RedirectToRoute({routeName});

            // [Formatted Response]
            // return Json({DTO instance});
        }
    }
}