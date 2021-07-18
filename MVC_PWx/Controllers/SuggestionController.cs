using DeneirsGate.Services;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DeneirsGateSite.Controllers
{
    [Authorize]
    public class SuggestionController : DeneirsController
    {
        SuggestionService suggestionSvc;

        public SuggestionController(SuggestionService suggestionService)
        {
            suggestionSvc = suggestionService;
        }

        public ActionResult Index()
        {
            var model = new List<SuggestionViewModel>();
            try
            {
                model = suggestionSvc.GetSuggestions(AppUser.UserId);

                ViewBag.Types = new SelectList(suggestionSvc.GetSuggestionTypeList(), "Key", "Value");
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

            return View(model);
        }

        public PartialViewResult _Pending()
        {
            var model = new List<SuggestionViewModel>();
            try
            {
                model = suggestionSvc.GetPendingSuggestions(AppUser.UserId);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        [HttpPost]
        public JsonResult Update(SuggestionPostModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    suggestionSvc.UpdateSuggestion(AppUser.UserId, model, User.IsInRole("Admin"));
                }
                catch (Exception ex)
                {
                    return HandleExceptionJsonErrorResponse(ex);
                }

                return GetJson(true, "Submitted successfully!");
            }
            return HandleValidationJsonErrorResponse();
        }

        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            try
            {
                suggestionSvc.DeleteSuggestion(AppUser.UserId, id);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Deleted successfully!");
        }

        [HttpPost]
        public JsonResult GenerateSuggestion(SuggestionService.SuggestionType type)
        {
            var suggestion = "";
            try
            {
                suggestion = suggestionSvc.GenerateSuggestion(type);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            if (suggestion == null) { return GetJson(false, "No suggestions available."); }

            return GetJson(true, "Generated successfully!", suggestion);
        }
    }
}