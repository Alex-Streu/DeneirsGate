using DeneirsGate.Services;
using DeneirsGateSite.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace DeneirsGateSite.Controllers
{
    [Authorize]
    public class CharacterController : DeneirsController
    {
        CharacterService characterSvc;
        UserService userSvc;
        SuggestionService suggestionSvc;
        PresetService presetSvc;

        public CharacterController(CharacterService characterService, UserService userService, SuggestionService suggestionService, PresetService presetService)
        {
            characterSvc = characterService;
            userSvc = userService;
            suggestionSvc = suggestionService;
            presetSvc = presetService;
        }

        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult Index()
        {
            var model = new List<CharacterShortViewModel>();
            try
            {
                model = characterSvc.GetAllCharacters(AppUser.UserId, AppUser.ActiveCampaign.Value);

                ViewBag.CampaignKey = AppUser.ActiveCampaign.Value;
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

            return View(model);
        }

        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult CreateCharacter(bool isPlayer = false)
        {
            return RedirectToAction("EditCharacter", new { id = Guid.NewGuid(), isPlayer, isNew = true });
        }

        [HasAccess(Priviledge = AppLogic.Priviledge.Player)]
        public ActionResult EditCharacter(Guid id, bool isPlayer = false, bool isNew = false)
        {
            var player = new PlayerViewModel();
            try
            {
                player = characterSvc.GetPlayer(AppUser.UserId, AppUser.ActiveCampaign, id);

                ViewBag.IsPlayer = isPlayer || player.IsPlayer;
                ViewBag.IsNew = isNew;
                ViewBag.IsDM = !User.IsInRole("Player");
                ViewBag.AllFriends = userSvc.GetFriends(AppUser.UserId, OnlineUsers);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

            return View(player);
        }

        [HttpPost, HasAccess(Priviledge = AppLogic.Priviledge.Player)]
        public JsonResult UpdatePlayer(PlayerPostModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    characterSvc.UpdateCharacter(AppUser.UserId, AppUser.ActiveCampaign, model, true, model.UserKey);
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
        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public JsonResult UpdateCharacter(CharacterPostModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    characterSvc.UpdateCharacter(AppUser.UserId, AppUser.ActiveCampaign.Value, model, false);
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
        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public JsonResult DeleteCharacter(Guid id)
        {
            try
            {
                characterSvc.DeleteCharacter(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

                var path = AppLogic.GetCharacterContentDir(AppUser.ActiveCampaign.Value, id);
                var fullPath = Server.MapPath(path);
                var dirInfo = new DirectoryInfo(fullPath);
                dirInfo.Delete(true);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Deleted successfully!");
        }

        [HttpPost]
        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public JsonResult SuggestCharacter()
        {
            var character = new CharacterViewModel();
            try
            {
                character = suggestionSvc.GenerateCharacter();

                var rand = new Random();
                var alignments = presetSvc.GetAlignments();
                var toSkip = rand.Next(0, alignments.Count);
                character.Alignment = alignments.OrderBy(x => Guid.NewGuid()).Skip(toSkip).Take(1).FirstOrDefault().Key;
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Deleted successfully!", character);
        }
    }
}