using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    public class ErrorsController : Controller
    {
        public ActionResult Error404()
        {
            ViewBag.Header = "Whoopsie!";
            ViewBag.Message = "The page you're looking for doesn't exist. No worries! Just use the button below to return to your dashboard.";
            ViewBag.Html = $"<a class='btn btn-lg btn-default' href='{Url.Action("/", "Home")}'>Return</a>";
            return View("Error");
        }

        public ActionResult Error500()
        {
            ViewBag.Header = "Our Bad!";
            ViewBag.Message = "Looks like something went wrong with your request. No worries! We have been notified of the issue and will look right into it! You can use the button below to return to your dashboard.";
            ViewBag.Html = $"<a class='btn btn-lg btn-default' href='{Url.Action("/", "Home")}'>Return</a>";
            return View("Error");
        }
    }
}