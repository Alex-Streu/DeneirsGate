using DeneirsGate.Services;
using DeneirsGateSite.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DeneirsGateSite.Controllers
{
    [Authorize, HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
    public class MonsterController : DeneirsController
    {
        MonsterService monsterSvc;
        PresetService presetSvc;

        public MonsterController(MonsterService monsterService, PresetService presetService)
        {
            monsterSvc = monsterService;
            presetSvc = presetService;
        }

        public ActionResult Index()
        {
            var model = new List<MonsterViewModel>();
            try
            {
                model = monsterSvc.GetMonsters(AppUser.UserId, AppUser.ActiveCampaign.Value, !User.IsInRole("Admin"));
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
            var model = new MonsterEditModel();
            try
            {
                model = monsterSvc.GetEditMonster(AppUser.UserId, id, AppUser.ActiveCampaign.Value, User.IsInRole("Admin"));

                ViewBag.IsNew = isNew;
                ViewBag.Sizes = new SelectList(monsterSvc.GetSizes(), "SizeKey", "Name");
                ViewBag.Types = new SelectList(monsterSvc.GetTypes(), "TypeKey", "Name");
                ViewBag.ChallengeRatings = new SelectList(monsterSvc.GetChallengeRatings(), "RatingKey", "Challenge");
                ViewBag.Environments = new MultiSelectList(presetSvc.GetEnvironments(), "EnvironmentKey", "Name", model.Environments);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult Update(MonsterPostModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                        monsterSvc.UpdateMonster(AppUser.UserId, model, User.IsInRole("Admin"));
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
                monsterSvc.Delete(AppUser.UserId, id, User.IsInRole("Admin"));
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Deleted successfully!");
        }
    }
}