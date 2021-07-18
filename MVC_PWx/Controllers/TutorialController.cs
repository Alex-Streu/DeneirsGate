using DeneirsGate.Services;
using System;
using System.Web.Mvc;

namespace DeneirsGateSite.Controllers
{
    public class TutorialController : DeneirsController
    {
        TutorialService tutorialSvc;

        public TutorialController(TutorialService tutorialService)
        {
            tutorialSvc = tutorialService;
        }

        [HttpPost]
        public JsonResult GetUserTutorial(UserTutorialQueryModel model)
        {
            var userTutorial = new UserTutorialViewModel();
            try
            {
                userTutorial = tutorialSvc.GetUserTutorial(AppUser.UserId, model.Route, model.Name);
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
                tutorialSvc.UpdateUserTutorial(AppUser.UserId, model.TutorialKey, model.IsComplete, model.LastStep);
            }
            catch (Exception ex)
            {
                HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Updated successfully!");
        }
    }
}