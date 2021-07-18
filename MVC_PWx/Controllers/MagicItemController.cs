using DeneirsGate.Services;
using DeneirsGateSite.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DeneirsGateSite.Controllers
{
    [Authorize, HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
    public class MagicItemController : DeneirsController
    {
        MagicItemService magicItemSvc;

        public MagicItemController(MagicItemService magicItemService)
        {
            magicItemSvc = magicItemService;
        }

        public ActionResult Index()
        {
            var model = new List<MagicItemViewModel>();
            try
            {
                model = magicItemSvc.GetMagicItems(AppUser.UserId, AppUser.ActiveCampaign.Value, !User.IsInRole("Admin"));
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
            var model = new MagicItemEditModel();
            try
            {
                model = magicItemSvc.GetEditMagicItem(AppUser.UserId, id, AppUser.ActiveCampaign.Value, User.IsInRole("Admin"));

                ViewBag.IsNew = isNew;
                ViewBag.Rarities = new SelectList(magicItemSvc.GetRarities(), "RarityKey", "Name");
                ViewBag.Types = new SelectList(magicItemSvc.GetTypes(), "TypeKey", "Name");
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult Update(MagicItemPostModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    magicItemSvc.Update(AppUser.UserId, model, User.IsInRole("Admin"));
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
                magicItemSvc.Delete(AppUser.UserId, id, User.IsInRole("Admin"));
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Deleted successfully!");
        }
    }
}