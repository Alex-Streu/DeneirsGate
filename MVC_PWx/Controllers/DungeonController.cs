using MVC_PWx.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    [Authorize, HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
    public class DungeonController : DeneirsController
    {
        // GET: Dungeon
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return RedirectToAction("Edit", new { id = Guid.NewGuid() });
        }

        public ActionResult Edit(Guid id)
        {
            try
            {
                ViewBag.TileImages = Directory.EnumerateFiles(Server.MapPath("~/Content/img/dungeon tiles/"))
                    .Select(x => "/Content/img/dungeon tiles/" + Path.GetFileName(x)).ToList();
            }
            catch (Exception ex) { }

            return View();
        }
    }
}