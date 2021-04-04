using DeneirsGate.Services;
using System;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    public class TutorialController : DeneirsController
    {
        [HttpPost]
        public JsonResult GetUserTutorial(UserTutorialQueryModel model)
        {
            var userTutorial = new UserTutorialViewModel();
            try
            {
                userTutorial = TutorialSvc.GetUserTutorial(AppUser.UserId, model.Route, model.Name);
            }
            catch (Exception ex)
            {
                HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, null, userTutorial);
        }

        [HttpPost]
        public JsonResult UpdateUserTutorial(UserTutorialPostModel model)
        {
            if (!ModelState.IsValid) { HandleValidationJsonErrorResponse(); }
            try
            {
                TutorialSvc.UpdateUserTutorial(AppUser.UserId, model.TutorialKey, model.IsComplete, model.LastStep);
            }
            catch (Exception ex)
            {
                HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Updated successfully!");
        }
    }
}