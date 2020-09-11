﻿using DeneirsGate.Services;
using MVC_PWx.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
    public class RelationshipTreeController : DeneirsController
    {
        public ActionResult Index()
        {
            var model = new List<RelationshipTreeSearchModel>();

            try
            {
                model = RelationshipTreeSvc.GetSearchTrees(AppUser.UserId, AppUser.ActiveCampaign.Value);

                ViewBag.SearchBy = new SelectList(RelationshipTreeSvc.GetSearchDropdown(), "Key", "Value");
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

            var model = new RelationshipTreeViewModel();
            try
            {
                model = RelationshipTreeSvc.GetRelationshipTree(id, AppUser.ActiveCampaign.Value);

                ViewBag.IsNew = isNew;
            }
            catch (Exception ex) { }

            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateTree(RelationshipTreePostModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    RelationshipTreeSvc.UpdateRelationshipTree(AppUser.UserId, model);
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
                RelationshipTreeSvc.DeleteRelationshipTree(AppUser.UserId, id);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Deleted successfully!" });
        }
    }
}