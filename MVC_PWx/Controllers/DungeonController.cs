using DeneirsGate.Services;
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
        #region Dungeons
        public ActionResult Index()
        {
            var model = new List<DungeonListViewModel>();
            try
            {
                model = DungeonSvc.GetDungeons(AppUser.UserId, AppUser.ActiveCampaign.Value);
            }
            catch (Exception ex) { }

            return View(model);
        }

        public ActionResult Create()
        {
            return RedirectToAction("Edit", new { id = Guid.NewGuid(), isNew = true });
        }

        public ActionResult Edit(Guid id, bool isNew = false)
        {
            var model = new DungeonViewModel();
            try
            {
                model = DungeonSvc.GetDungeon(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
                foreach (var tile in model.Tiles)
                {
                    if (tile.Encounter != null)
                    {
                        tile.Encounter = EventSvc.GetEncounter(tile.Encounter.EncounterKey);
                    }
                }

                ViewBag.IsNew = isNew;
                ViewBag.TrapNatures = new SelectList(DungeonSvc.GetTrapNatures(), "NatureKey", "Name");
                ViewBag.TrapTypes = new SelectList(DungeonSvc.GetTrapTypes(), "TypeKey", "Name");

                ViewBag.TileImages1 = Directory.EnumerateFiles(Server.MapPath("~/Content/img/dungeon tiles/1/"))
                    .Select(x => "/Content/img/dungeon tiles/1/" + Path.GetFileName(x)).ToList();
                ViewBag.TileImages2 = Directory.EnumerateFiles(Server.MapPath("~/Content/img/dungeon tiles/2/"))
                    .Select(x => "/Content/img/dungeon tiles/2/" + Path.GetFileName(x)).ToList();
                ViewBag.TileImages3 = Directory.EnumerateFiles(Server.MapPath("~/Content/img/dungeon tiles/3/"))
                    .Select(x => "/Content/img/dungeon tiles/3/" + Path.GetFileName(x)).ToList();
                ViewBag.TileImages4 = Directory.EnumerateFiles(Server.MapPath("~/Content/img/dungeon tiles/4/"))
                    .Select(x => "/Content/img/dungeon tiles/4/" + Path.GetFileName(x)).ToList();
            }
            catch (Exception ex) { }

            return View(model);
        }

        public ActionResult Print(Guid id)
        {
            var model = new DungeonViewModel();
            try
            {
                model = DungeonSvc.GetDungeon(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
                foreach (var tile in model.Tiles)
                {
                    if (tile.Encounter != null)
                    {
                        tile.Encounter = EventSvc.GetEncounter(tile.Encounter.EncounterKey);
                        MonsterSvc.GetEncounterMonsters(AppUser.UserId, tile.Encounter);
                        MagicItemSvc.GetEncounterItems(AppUser.UserId, tile.Encounter);
                    }
                }
            }
            catch (Exception ex) { }

            return View(model);
        }

        [HttpPost]
        public JsonResult Update(DungeonPostModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Update Encounters
                    EventSvc.DeleteDungeonEncounters(model.DungeonKey, model.Tiles.Where(x => x.Encounter != null).Select(x => x.Encounter.EncounterKey).ToList());
                    foreach (var tile in model.Tiles)
                    {
                        if (tile.Encounter == null) { continue; }
                        EventSvc.UpdateEncounter(tile.Encounter);
                    }

                    //Update Dungeon
                    DungeonSvc.UpdateDungeon(AppUser.UserId, AppUser.ActiveCampaign.Value, model);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }

                return Json(new { success = true, message = "Updated successfully!" });
            }
            return Json(new { success = false, message = GetValidationError() });
        }

        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            try
            {
                var encounters = DungeonSvc.DeleteDungeon(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
                foreach (var encounter in encounters)
                {
                    EventSvc.DeleteEncounter(encounter);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Deleted successfully!" });
        }
        #endregion


        #region Traps
        public ActionResult Traps()
        {
            var model = new List<TrapViewModel>();
            try
            {
                model = DungeonSvc.GetTraps(AppUser.UserId, AppUser.ActiveCampaign.Value);
            }
            catch (Exception ex) { }

            return View(model);
        }

        public ActionResult CreateTrap()
        {
            return RedirectToAction("EditTrap", new { id = Guid.NewGuid(), isNew = true });
        }

        public ActionResult EditTrap(Guid id, bool isNew = false)
        {
            var model = new TrapEditModel();
            try
            {
                model = DungeonSvc.GetTrapEdit(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

                ViewBag.IsNew = isNew;
                ViewBag.Natures = new SelectList(DungeonSvc.GetTrapNatures(), "NatureKey", "Name", model.NatureKey);
                ViewBag.Types = new SelectList(DungeonSvc.GetTrapTypes(), "TypeKey", "Name", model.TypeKey);
            }
            catch (Exception ex) { }

            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateTrap(TrapPostModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DungeonSvc.UpdateTrap(AppUser.UserId, AppUser.ActiveCampaign.Value, model);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }

                return Json(new { success = true, message = "Updated successfully!" });
            }
            return Json(new { success = false, message = GetValidationError() });
        }

        [HttpPost]
        public JsonResult DeleteTrap(Guid id)
        {
            try
            {
                DungeonSvc.DeleteTrap(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Deleted successfully!" });
        }

        [HttpPost]
        public JsonResult SuggestTrap(TrapSuggestPostModel model)
        {
            var trap = new TrapViewModel();
            try
            {
                var partyLevel = EventSvc.GetPartyLevel(AppUser.UserId, AppUser.ActiveCampaign.Value);
                trap = DungeonSvc.SuggestTrap(AppUser.UserId, AppUser.ActiveCampaign.Value, partyLevel, model.Name, model.NatureKey, model.TypeKey);

                if (trap == null)
                {
                    return Json(new { success = false, message = "No traps were found!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Retrieved successfully!", data = trap });
        }

        [HttpPost]
        public JsonResult GenerateTrapStats(TrapStatsPostModel model)
        {
            var stats = new TrapStatsModel();
            try
            {
                var partyLevel = EventSvc.GetPartyLevel(AppUser.UserId, AppUser.ActiveCampaign.Value);
                stats = DungeonSvc.GenerateTrapStats(model.TypeKey, partyLevel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Retrieved successfully!", data = stats });
        }
        #endregion
    }
}