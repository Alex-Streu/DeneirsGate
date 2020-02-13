using MVC_PWx.Helpers;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    [HasAccess(Priviledge = AppLogic.Priviledge.Player)]
    public class CharacterController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}