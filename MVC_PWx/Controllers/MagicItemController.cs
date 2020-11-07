using DeneirsGate.Services;
using MVC_PWx.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    [Authorize, HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
    public class MagicItemController : DeneirsController
    {
        public ActionResult Index()
        {
            var model = new List<MagicItemViewModel>();
            try
            {
                model = MagicItemSvc.GetMagicItems(AppUser.UserId, AppUser.ActiveCampaign.Value, !User.IsInRole("Admin"));
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
            var model = new MagicItemEditModel();
            try
            {
                model = MagicItemSvc.GetEditMagicItem(AppUser.UserId, id, AppUser.ActiveCampaign.Value);

                ViewBag.IsNew = isNew;
                ViewBag.Rarities = new SelectList(MagicItemSvc.GetRarities(), "RarityKey", "Name");
                ViewBag.Types = new SelectList(MagicItemSvc.GetTypes(), "TypeKey", "Name");
            }
            catch (Exception ex) { }

            return View(model);
        }

        [HttpPost]
        public JsonResult Update(MagicItemPostModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    MagicItemSvc.Update(AppUser.UserId, model);
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
                MagicItemSvc.Delete(AppUser.UserId, id, User.IsInRole("Admin"));
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Deleted successfully!" });
        }
    }
}