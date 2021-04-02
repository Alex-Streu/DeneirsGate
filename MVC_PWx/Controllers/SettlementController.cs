using DeneirsGate.Services;
using MVC_PWx.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    [Authorize, HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
    public class SettlementController : DeneirsController
    {
        public ActionResult Index()
        {
            var model = new List<SettlementViewModel>();
            try
            {
                model = SettlementSvc.GetSettlements(AppUser.UserId, AppUser.ActiveCampaign.Value);
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
            var model = new SettlementViewModel();
            try
            {
                model = SettlementSvc.GetSettlement(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

                ViewBag.IsNew = isNew;
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

            return View(model);
        }

        public ActionResult Print(Guid id, bool isNew = false)
        {
            var model = new SettlementViewModel();
            try
            {
                model = SettlementSvc.GetSettlement(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

                ViewBag.IsNew = isNew;
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult Update(SettlementViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SettlementSvc.UpdateSettlement(AppUser.UserId, AppUser.ActiveCampaign.Value, model);
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
                SettlementSvc.DeleteSettlement(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Deleted successfully!");
        }
    }
}