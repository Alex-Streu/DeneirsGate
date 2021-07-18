using DeneirsGate.Services;
using DeneirsGateSite.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DeneirsGateSite.Controllers
{
    [Authorize, HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
    public class SettlementController : DeneirsController
    {
        SettlementService settlementSvc;

        public SettlementController(SettlementService settlementService)
        {
            settlementSvc = settlementService;
        }

        public ActionResult Index()
        {
            var model = new List<SettlementViewModel>();
            try
            {
                model = settlementSvc.GetSettlements(AppUser.UserId, AppUser.ActiveCampaign.Value);
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
                model = settlementSvc.GetSettlement(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

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
                model = settlementSvc.GetSettlement(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

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
                    settlementSvc.UpdateSettlement(AppUser.UserId, AppUser.ActiveCampaign.Value, model);
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
                settlementSvc.DeleteSettlement(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Deleted successfully!");
        }
    }
}