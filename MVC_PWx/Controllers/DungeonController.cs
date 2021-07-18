using DeneirsGate.Services;
using DeneirsGateSite.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace DeneirsGateSite.Controllers
{
    [Authorize, HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
    public class DungeonController : DeneirsController
    {
        EventService eventSvc;
        MonsterService monsterSvc;
        MagicItemService magicItemSvc;
        DungeonService dungeonSvc;

        public DungeonController(EventService eventService, MonsterService monsterService, MagicItemService magicItemService, DungeonService dungeonService)
        {
            eventSvc = eventService;
            monsterSvc = monsterService;
            magicItemSvc = magicItemService;
            dungeonSvc = dungeonService;
        }

        #region Dungeons
        public ActionResult Index()
        {
            var model = new List<DungeonListViewModel>();
            try
            {
                model = dungeonSvc.GetDungeons(AppUser.UserId, AppUser.ActiveCampaign.Value);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

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
                model = dungeonSvc.GetDungeon(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
                foreach (var tile in model.Tiles)
                {
                    if (tile.Encounter != null)
                    {
                        tile.Encounter = eventSvc.GetEncounter(tile.Encounter.EncounterKey);
                    }
                }

                ViewBag.IsNew = isNew;
                ViewBag.TrapNatures = new SelectList(dungeonSvc.GetTrapNatures(), "NatureKey", "Name");
                ViewBag.TrapTypes = new SelectList(dungeonSvc.GetTrapTypes(), "TypeKey", "Name");

                ViewBag.TileImages1 = Directory.EnumerateFiles(Server.MapPath("~/Content/img/dungeon tiles/1/"))
                    .Select(x => "/Content/img/dungeon tiles/1/" + Path.GetFileName(x)).ToList();
                ViewBag.TileImages2 = Directory.EnumerateFiles(Server.MapPath("~/Content/img/dungeon tiles/2/"))
                    .Select(x => "/Content/img/dungeon tiles/2/" + Path.GetFileName(x)).ToList();
                ViewBag.TileImages3 = Directory.EnumerateFiles(Server.MapPath("~/Content/img/dungeon tiles/3/"))
                    .Select(x => "/Content/img/dungeon tiles/3/" + Path.GetFileName(x)).ToList();
                ViewBag.TileImages4 = Directory.EnumerateFiles(Server.MapPath("~/Content/img/dungeon tiles/4/"))
                    .Select(x => "/Content/img/dungeon tiles/4/" + Path.GetFileName(x)).ToList();
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

            return View(model);
        }

        public ActionResult Print(Guid id)
        {
            var model = new DungeonViewModel();
            try
            {
                model = dungeonSvc.GetDungeon(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
                foreach (var tile in model.Tiles)
                {
                    if (tile.Encounter != null)
                    {
                        tile.Encounter = eventSvc.GetEncounter(tile.Encounter.EncounterKey);
                        monsterSvc.GetEncounterMonsters(AppUser.UserId, tile.Encounter);
                        magicItemSvc.GetEncounterItems(AppUser.UserId, tile.Encounter);
                    }
                }
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

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
                    eventSvc.DeleteDungeonEncounters(model.DungeonKey, model.Tiles.Where(x => x.Encounter != null).Select(x => x.Encounter.EncounterKey).ToList());
                    foreach (var tile in model.Tiles)
                    {
                        if (tile.Encounter == null) { continue; }
                        eventSvc.UpdateEncounter(tile.Encounter);
                    }

                    //Update Dungeon
                    dungeonSvc.UpdateDungeon(AppUser.UserId, AppUser.ActiveCampaign.Value, model);
                }
                catch (Exception ex)
                {
                    return HandleExceptionJsonErrorResponse(ex);
                }

                return GetJson(true, "Updated successfully!");
            }
            return HandleValidationJsonErrorResponse();
        }

        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            try
            {
                var encounters = dungeonSvc.DeleteDungeon(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
                foreach (var encounter in encounters)
                {
                    eventSvc.DeleteEncounter(encounter);
                }
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Deleted successfully!");
        }
        #endregion


        #region Traps
        public ActionResult Traps()
        {
            var model = new List<TrapViewModel>();
            try
            {
                model = dungeonSvc.GetTraps(AppUser.UserId, AppUser.ActiveCampaign.Value);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

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
                model = dungeonSvc.GetTrapEdit(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

                ViewBag.IsNew = isNew;
                ViewBag.Natures = new SelectList(dungeonSvc.GetTrapNatures(), "NatureKey", "Name", model.NatureKey);
                ViewBag.Types = new SelectList(dungeonSvc.GetTrapTypes(), "TypeKey", "Name", model.TypeKey);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateTrap(TrapPostModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    dungeonSvc.UpdateTrap(AppUser.UserId, AppUser.ActiveCampaign.Value, model);
                }
                catch (Exception ex)
                {
                    return HandleExceptionJsonErrorResponse(ex);
                }

                return GetJson(true, "Updated successfully!");
            }
            return HandleValidationJsonErrorResponse();
        }

        [HttpPost]
        public JsonResult DeleteTrap(Guid id)
        {
            try
            {
                dungeonSvc.DeleteTrap(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Deleted successfully!");
        }

        [HttpPost]
        public JsonResult SuggestTrap(TrapSuggestPostModel model)
        {
            var trap = new TrapViewModel();
            try
            {
                var partyLevel = eventSvc.GetPartyLevel(AppUser.UserId, AppUser.ActiveCampaign.Value);
                trap = dungeonSvc.SuggestTrap(AppUser.UserId, AppUser.ActiveCampaign.Value, partyLevel, model.Name, model.NatureKey, model.TypeKey);

                if (trap == null)
                {
                    return GetJson(false, "No traps were found!");
                }
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Retrieved successfully", trap);
        }

        [HttpPost]
        public JsonResult GenerateTrapStats(TrapStatsPostModel model)
        {
            var stats = new TrapStatsModel();
            try
            {
                var partyLevel = eventSvc.GetPartyLevel(AppUser.UserId, AppUser.ActiveCampaign.Value);
                stats = dungeonSvc.GenerateTrapStats(model.TypeKey, partyLevel);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Retrieved successfully", stats);
        }
        #endregion
    }
}