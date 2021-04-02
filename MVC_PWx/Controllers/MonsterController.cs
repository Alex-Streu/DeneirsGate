using DeneirsGate.Services;
using MVC_PWx.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    [Authorize, HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
    public class MonsterController : DeneirsController
    {
        public ActionResult Index()
        {
            var model = new List<MonsterViewModel>();
            try
            {
                model = MonsterSvc.GetMonsters(AppUser.UserId, AppUser.ActiveCampaign.Value, !User.IsInRole("Admin"));
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
                model = MonsterSvc.GetEditMonster(AppUser.UserId, id, AppUser.ActiveCampaign.Value, User.IsInRole("Admin"));

                ViewBag.IsNew = isNew;
                ViewBag.Sizes = new SelectList(MonsterSvc.GetSizes(), "SizeKey", "Name");
                ViewBag.Types = new SelectList(MonsterSvc.GetTypes(), "TypeKey", "Name");
                ViewBag.ChallengeRatings = new SelectList(MonsterSvc.GetChallengeRatings(), "RatingKey", "Challenge");
                ViewBag.Environments = new MultiSelectList(PresetSvc.GetEnvironments(), "EnvironmentKey", "Name", model.Environments);
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
                        MonsterSvc.UpdateMonster(AppUser.UserId, model, User.IsInRole("Admin"));
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
                MonsterSvc.Delete(AppUser.UserId, id, User.IsInRole("Admin"));
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Deleted successfully!");
        }
    }
}