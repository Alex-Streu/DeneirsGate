using MVC_PWx.Helpers;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    [Authorize]
    public class HomeController : DeneirsController
    {
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}