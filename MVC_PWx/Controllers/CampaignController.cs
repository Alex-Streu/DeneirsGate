﻿using DeneirsGate.Services;
using MVC_PWx.Helpers;
using Sentry;
using System;
using System.Collections.Generic;
using System.IO;
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
                model = CampaignSvc.GetCampaignDashboard(AppUser.UserId, AppUser.ActiveCampaign.Value);

                //Handle a no longer existing campaign
                if (model == null)
                {
                    SetActiveCampaign(null);
                    return RedirectToAction("ChangeCampaign");
                }

                var arcs = CampaignSvc.GetArcs(AppUser.UserId, AppUser.ActiveCampaign.Value);
                var activeArc = arcs.FirstOrDefault(x => x.IsActive);
                var characters = new List<Guid>();
                if (activeArc != null)
                {
                    characters = CampaignSvc.GetArcCharacters(AppUser.UserId, AppUser.ActiveCampaign.Value, activeArc.ArcKey);
                }

                model.NPCs = CharacterSvc.GetArcCharacters(AppUser.UserId, AppUser.ActiveCampaign.Value, characters);
                model.Players = CharacterSvc.GetPlayerShorts(AppUser.UserId, AppUser.ActiveCampaign.Value);

                //Generate Character Dropdown List
                var characterList = model.NPCs.Where(x => x.IsSelected).Select(x => new { CharacterKey = x.CharacterKey, Name = x.FirstName + " " + x.LastName }).ToList();
                characterList.AddRange(model.Players.Select(x => new { CharacterKey = x.CharacterKey, Name = x.FirstName + " " + x.LastName }).ToList());

                //Generate Quest Events List
                var eventsList = new Dictionary<string, string>();
                foreach (var quest in model.Arc.Quests)
                {
                    foreach (var qEvent in quest.Events)
                    {
                        eventsList.Add($"{quest.QuestKey}_{qEvent.EventKey}", qEvent.Name);
                    }
                }

                ViewBag.Arcs = new SelectList(arcs, "ArcKey", "Name");
                ViewBag.ActivityLogTypes = new SelectList(CampaignSvc.GetLogTypes(), "Key", "Value");
                ViewBag.ArcCharacters = new SelectList(characterList.OrderBy(x => x.Name), "CharacterKey", "Name");
                ViewBag.ArcQuests = new SelectList(model.Arc.Quests, "QuestKey", "Name");
                ViewBag.ArcEvents = new SelectList(eventsList, "Key", "Value");
            }
            catch (Exception ex) 
            {
                return HandleExceptionRedirectError(ex);
            }

            return View(model);
        }

        public PartialViewResult _ActivityLog(Guid? arcKey)
        {
            var model = new List<ActivityLogViewModel>();
            try
            {
                if (arcKey.HasValue)
                {
                    model = CampaignSvc.GetActivityLog(arcKey.Value);
                }
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        [HttpPost]
        public JsonResult UpdateTodoItem(TodoItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CampaignSvc.UpdateTodoItem(model);
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
        public JsonResult DeleteTodoItem(TodoItemDeleteModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CampaignSvc.DeleteTodoItem(model);
                }
                catch (Exception ex)
                {
                    return HandleExceptionJsonErrorResponse(ex);
                }

                return GetJson(true, "Deleted successfully!");
            }
            return HandleValidationJsonErrorResponse();
        }

        [HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult ChangeCampaign()
        {
            //If ActiveCampaign exists but isn't in session yet, set it and redirect back to dashboard
            if (Session["ActiveCampaign"] == null && AppUser.ActiveCampaign != null)
            {
                Session["ActiveCampaign"] = AppUser.ActiveCampaign.Value;
                return RedirectToAction("/");
            }

            var campaigns = new List<CampaignViewModel>();
            try
            {
                campaigns = CampaignSvc.GetCampaigns(AppUser.UserId, true);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

            return View(campaigns);
        }

        [HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult Create()
        {
            return RedirectToAction("Edit", new { id = Guid.NewGuid(), isNew = true });
        }

        [HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult Edit(Guid id, bool isNew = false)
        {
            var model = new CampaignViewModel();
            try
            {
                model = CampaignSvc.GetCampaign(AppUser.UserId, id);

                ViewBag.IsNew = isNew;
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult Update(CampaignPostModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CampaignSvc.UpdateCampaign(AppUser.UserId, model);
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
        [HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public JsonResult Delete(Guid id)
        {
            try
            {
                CampaignSvc.DeleteCampaign(AppUser.UserId, id);

                var path = AppLogic.GetCampaignContentDir(id);
                var fullPath = Server.MapPath(path);
                if (Directory.Exists(fullPath))
                {
                    var dirInfo = new DirectoryInfo(fullPath);
                    dirInfo.Delete(true);
                }

                if (AppUser.ActiveCampaign != null && AppUser.ActiveCampaign.Value == id)
                {
                    SetActiveCampaign(null);
                }
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Deleted successfully!");
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
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Character added successfully!");
        }

        [HttpPost]
        [HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public JsonResult UpdateActivityLog(ActivityLogPostModel model)
        {
            try
            {
                CampaignSvc.UpdateActivityLog(AppUser.UserId, AppUser.ActiveCampaign.Value, model.ArcKey, model.LogKey, model.LogDescription, model.Type, model.ContentKey);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Log added successfully!");
        }

        [HttpPost]
        [HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public JsonResult UpdateQuestStatus(QuestStatusPostModel model)
        {
            try
            {
                CampaignSvc.UpdateQuestStatus(AppUser.UserId, model.ArcKey, model.QuestKey, model.Status);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Quest status updated successfully!");
        }

        [HttpPost]
        [HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public JsonResult DeleteActivityLog(ActivityLogDeleteModel model)
        {
            try
            {
                CampaignSvc.DeleteActivityLog(AppUser.UserId, AppUser.ActiveCampaign.Value, model.ArcKey, model.LogKey);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Deleted successfully!");
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
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

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
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

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
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

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
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Updated successfully!");
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
                    return HandleExceptionJsonErrorResponse(ex);
                }

                return GetJson(true, "Updated successfully!");
            }
            return HandleValidationJsonErrorResponse();
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

                var path = AppLogic.GetArcMapContentDir(AppUser.ActiveCampaign.Value, id);
                var fullPath = Server.MapPath(path);
                var dirInfo = new DirectoryInfo(fullPath);
                dirInfo.Delete(true);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Deleted successfully!");
        }

        #endregion
    }
}