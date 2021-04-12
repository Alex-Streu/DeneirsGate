using DeneirsGate.Services;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    [AllowAnonymous]
    public class ErrorsController : Controller
    {
        public ActionResult Error404()
        {
            ViewBag.Header = "Whoopsie!";
            ViewBag.Message = "The page you're looking for doesn't exist. No worries!";
            ViewBag.ReturnUrl = Url.Action("/", "Campaign");
            return View("Error");
        }

        public ActionResult Error500(string error = null)
        {
            ViewBag.Header = "Our Bad!";
            ViewBag.Message = "Looks like something went wrong with your request. No worries! We have been notified of the issue and will look right into it!";
            ViewBag.ReturnUrl = Url.Action("/", "Campaign");
            ViewBag.Error = error;

            return View("Error");
        }

        public ActionResult CustomError(ErrorPostModel model)
        {
            ViewBag.Header = model.Header;
            ViewBag.Message = model.Message;
            ViewBag.Html = model.Html;
            ViewBag.ReturnUrl = model.ReturnUrl;
            ViewBag.Error = model.Error;

            return View("Error");
        }

        public ActionResult CustomInfo(ErrorPostModel model)
        {
            ViewBag.Header = model.Header;
            ViewBag.Message = model.Message;
            ViewBag.Html = model.Html;
            ViewBag.ReturnUrl = model.ReturnUrl;

            return View("Info");
        }
    }
}