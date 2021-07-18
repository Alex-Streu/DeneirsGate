using DeneirsGate.Services;
using DeneirsGateSite.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DeneirsGateSite.Controllers
{
    [Authorize, HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
    public class RelationshipTreeController : DeneirsController
    {
        RelationshipTreeService relationshipTreeSvc;

        public RelationshipTreeController(RelationshipTreeService relationshipTreeService)
        {
            relationshipTreeSvc = relationshipTreeService;
        }

        public ActionResult Index()
        {
            var model = new List<RelationshipTreeSearchModel>();

            try
            {
                model = relationshipTreeSvc.GetSearchTrees(AppUser.UserId, AppUser.ActiveCampaign.Value);

                ViewBag.SearchBy = new SelectList(relationshipTreeSvc.GetSearchDropdown(), "Key", "Value");
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

            var model = new RelationshipTreeViewModel();
            try
            {
                model = relationshipTreeSvc.GetRelationshipTree(id, AppUser.ActiveCampaign.Value);

                ViewBag.IsNew = isNew;
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateTree(RelationshipTreePostModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    relationshipTreeSvc.UpdateRelationshipTree(AppUser.UserId, model);
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
                relationshipTreeSvc.DeleteRelationshipTree(AppUser.UserId, id);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Deleted successfully!");
        }
    }
}