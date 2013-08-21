using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetSyncBlock()
        {
            Thread.Sleep(new Random().Next(1000, 10000));

            return Json(new {date = DateTime.Now.ToString()}, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAsyncBlock()
        {
            await Task.Factory.StartNew(() => Thread.Sleep(new Random().Next(1000, 10000)));

            return Json(new { date = DateTime.Now.ToString() }, JsonRequestBehavior.AllowGet);
        }
    }
}   