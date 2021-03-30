using DeneirsGate.Services;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    public class SuggestionController : DeneirsController
    {
        public ActionResult Index()
        {
            var model = new List<SuggestionViewModel>();
            try
            {
                model = SuggestionSvc.GetSuggestions(AppUser.UserId);

                ViewBag.Types = new SelectList(SuggestionSvc.GetSuggestionTypeList(), "Key", "Value");
            }
            catch (Exception ex) { }

            return View(model);
        }

        public PartialViewResult _Pending()
        {
            var model = new List<SuggestionViewModel>();
            try
            {
                model = SuggestionSvc.GetPendingSuggestions(AppUser.UserId);
            }
            catch (Exception ex) { }

            return PartialView(model);
        }

        [HttpPost]
        public JsonResult Update(SuggestionPostModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SuggestionSvc.UpdateSuggestion(AppUser.UserId, model, User.IsInRole("Admin"));
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }

                return Json(new { success = true, message = "Submitted successfully!" });
            }
            return Json(new { success = false, message = GetValidationError() });
        }

        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SuggestionSvc.DeleteSuggestion(AppUser.UserId, id);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }

                return Json(new { success = true, message = "Deleted successfully!" });
            }
            return Json(new { success = false, message = GetValidationError() });
        }

        [HttpPost]
        public JsonResult GenerateSuggestion(SuggestionService.SuggestionType type)
        {
            if (ModelState.IsValid)
            {
                var suggestion = "";
                try
                {
                    suggestion = SuggestionSvc.GenerateSuggestion(type);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }

                if  (suggestion == null) { return Json(new { success = false, message = "No suggestions available." }); }

                return Json(new { success = true, message = "Generated successfully!", data = suggestion });
            }
            return Json(new { success = false, message = GetValidationError() });
        }
    }
}