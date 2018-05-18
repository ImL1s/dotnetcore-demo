using System;
using System.Linq;
using System.Text.RegularExpressions;
using dotnetcore_demo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnetcore_demo.Controllers
{
    // 當 controller 加上 route attribute 時,底下的 action 也需要加上 attribute, 反之不用
    // 如果在 Startup 有使用 mvc路由並且本 controller 符合該路由條件時,就必須加上 route attribute 覆蓋 Startup 的 mvc 路由
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly MyContext _context;

        public UserController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("{q}")]
        public ResultModel Get(string q)
        {
            var result = new ResultModel();
            result.Data = _context.Users.Where(x =>
                string.IsNullOrEmpty(q) ||
                Regex.IsMatch(x.Name, q, RegexOptions.IgnoreCase)
            );
            result.IsSuccess = true;
            return result;
        }

        [HttpGet("{id}")]
        public ResultModel Get(int id)
        {
            var result = new ResultModel();
            result.Data = _context.Users.SingleOrDefault(x => x.Id == id);
            result.IsSuccess = true;
            return result;
        }

        [HttpPost]
        public ResultModel Post([FromBody]UserModel user)
        {
            var result = new ResultModel();
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);

                try
                {
                    _context.SaveChanges();
                    result.Data = user.Id;
                    result.IsSuccess = true;
                }
                catch (DbUpdateException)
                {
                    result.IsSuccess = false;
                    result.Message = "無法指定ID, 該值為伺服器自動產生";
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ModelState.ToString();
            }
            return result;

        }

        [HttpPut("{id}")]
        public ResultModel Put([FromBody] UserModel user)
        {
            var result = new ResultModel();
            var originalUser = _context.Users.SingleOrDefault(x => x.Id == user.Id);
            if (originalUser != null)
            {
                _context.Entry(originalUser).CurrentValues.SetValues(user);
                _context.SaveChanges();
                result.IsSuccess = true;
            }
            return result;
        }

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