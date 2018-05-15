using dotnetcore_demo.Model;
using Microsoft.AspNetCore.Mvc;

namespace dotnetcore_demo.Controllers
{
    public class SampleController : Controller
    {
        private ISample _scoped;
        private ISample _transient;
        private ISample _singleton;

        public SampleController(ISampleScoped scoped, ISampleTransient transient, ISampleSingleton singleton)
        {
            _scoped = scoped;
            _transient = transient;
            _singleton = singleton;
        }

        public IActionResult Index()
        {
            // return Content("$[ISample]\r\n"
            // + $"Id: {_sample.Id}\r\n"
            // + $"HashCode: {_sample.GetHashCode()}\r\n"
            // + $"Type: {_sample.GetType()}");
            
            ViewBag.TransientId = _transient.Id;
            ViewBag.TransientHashCode = _transient.GetHashCode();

            ViewBag.ScopedId = _scoped.Id;
            ViewBag.ScopedHashCode = _scoped.GetHashCode();

            ViewBag.SingletonId = _singleton.Id;
            ViewBag.SingletonHashCode = _singleton.GetHashCode();

            return View();
        }
    }
}