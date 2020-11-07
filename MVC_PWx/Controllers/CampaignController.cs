using DeneirsGate.Services;
using MVC_PWx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    [Authorize]
    [HasAccess(Priviledge = AppLogic.Priviledge.Player)]
    public class CampaignController : DeneirsController
    {
        #region Campaigns

        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult Index()
        {
            var model = new CampaignDashboardViewModel();
            try
            {
                var arcs = CampaignSvc.GetArcs(AppUser.UserId, AppUser.ActiveCampaign.Value);
                var activeArc = arcs.FirstOrDefault(x => x.IsActive);
                var characters = new List<Guid>();
                if (activeArc != null)
                {
                    characters = CampaignSvc.GetArcCharacters(AppUser.UserId, AppUser.ActiveCampaign.Value, activeArc.ArcKey);
                }

                model = CampaignSvc.GetCampaignDashboard(AppUser.UserId, AppUser.ActiveCampaign.Value);
                model.NPCs = CharacterSvc.GetArcCharacters(AppUser.UserId, AppUser.ActiveCampaign.Value, characters);
                model.Players = CharacterSvc.GetPlayerShorts(AppUser.UserId, AppUser.ActiveCampaign.Value);

                ViewBag.Arcs = new SelectList(arcs, "ArcKey", "Name");

            }
            catch (Exception ex) { }

            return View(model);
        }

        [HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult ChangeCampaign()
        {
            var campaigns = new List<CampaignViewModel>();
            try
            {
                campaigns = CampaignSvc.GetCampaigns(AppUser.UserId, true);
            }
            catch (Exception ex) { }

            return View(campaigns);
        }

        [HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult ActivateCampaign(Guid? id)
        {
            if (id != null)
            {
                SetActiveCampaign(id);
            }

            return RedirectToAction("/");
        }

        [HttpPost]
        [HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public JsonResult AddArcCharacter(ArcCharacterPostModel model)
        {
            try
            {
                CampaignSvc.AddArcCharacter(model.ArcKey, model.CharacterKey, model.Add);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Character added successfully!" });
        }

        #endregion

        #region Arcs

        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult Arcs()
        {
            var model = new List<ArcViewModel>();
            try
            {
                model = CampaignSvc.GetArcs(AppUser.UserId, AppUser.ActiveCampaign.Value);
            }
            catch (Exception ex) { }

            return View(model);
        }

        public ActionResult CreateArc()
        {
            return RedirectToAction("EditArc", new { id = Guid.NewGuid(), isNew = true });
        }

        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult EditArc(Guid id, bool isNew = false)
        {
            var model = new ArcViewModel();
            try
            {
                model = CampaignSvc.GetArc(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
                foreach (var quest in model.Quests)
                {
                    foreach (var questEvent in quest.Events)
                    {
                        if (questEvent.Encounter != null)
                        {
                            questEvent.Encounter = EventSvc.GetEncounter(questEvent.Encounter.EncounterKey);
                        }
                    }
                }

                ViewBag.IsNew = isNew;
            }
            catch (Exception ex) { }

            return View(model);
        }

        public ActionResult Print(Guid id)
        {
            var model = new ArcViewModel();
            try
            {
                model = CampaignSvc.GetArc(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
                foreach (var quest in model.Quests)
                {
                    foreach (var questEvent in quest.Events)
                    {
                        if (questEvent.Encounter != null)
                        {
                            questEvent.Encounter = EventSvc.GetEncounter(questEvent.Encounter.EncounterKey);
                            MonsterSvc.GetEncounterMonsters(AppUser.UserId, questEvent.Encounter);
                            MagicItemSvc.GetEncounterItems(AppUser.UserId, questEvent.Encounter);
                        }
                    }
                }
            }
            catch (Exception ex) { }

            return View(model);
        }

        [HttpPost]
        public JsonResult SetArcActive(ArcActivePostModel model)
        {
            try
            {
                CampaignSvc.UpdateArcActive(AppUser.UserId, AppUser.ActiveCampaign.Value, model);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Updated successfully!" });
        }

        [HttpPost]
        public JsonResult UpdateArc(ArcPostModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Update encounters
                    var encounterKeys = new List<Guid>();
                    foreach (var quest in model.Quests)
                    {
                        if (quest.Events == null) { continue; }
                        encounterKeys.AddRange(quest.Events.Where(x => x.Encounter != null).Select(x => x.Encounter.EncounterKey).ToList());
                        foreach (var qEvent in quest.Events)
                        {
                            if (qEvent.Encounter == null) { continue; }
                            EventSvc.UpdateEncounter(qEvent.Encounter);
                        }
                    }
                    EventSvc.DeleteQuestEventEncounters(model.ArcKey, encounterKeys);

                    //Update arc
                    CampaignSvc.UpdateArc(AppUser.UserId, AppUser.ActiveCampaign.Value, model);
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
        public JsonResult DeleteArc(Guid id)
        {
            try
            {
                var encounters = CampaignSvc.DeleteArc(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
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
    }
}