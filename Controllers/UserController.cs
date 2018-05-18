using dotnetcore_demo.Model;
using Microsoft.AspNetCore.Mvc;

namespace dotnetcore_demo.Controllers
{
    // 當 controller 加上 route attribute 時,底下的 action 也需要加上 attribute, 反之不用
    // 如果在 Startup 有使用 mvc路由並且本 controller 符合該路由條件時,就必須加上 route attribute 覆蓋 Startup 的 mvc 路由
    [Route("[controller]")]
    public class UserController : Controller
    {
        [Route("[action]/{name}")]
        public IActionResult test(string name)
        {
            var model = new UserModel()
            {
                Name = name
            };
            return Content(model.Name);
        }
        [Route("")]
        public IActionResult Profile()
        {
            return View();
        }


        [Route("change-password")]
        public IActionResult ChangePassword()
        {
            return View();
        }


        [Route("[action]")]
        public IActionResult Other()
        {
            return View();
        }
    }
}