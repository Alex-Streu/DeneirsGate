using DeneirsGate.Services;
using DeneirsGateSite.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DeneirsGateSite.Controllers
{
    [Authorize, HasAccess(Roles = "Player")]
    public class PlayerController : DeneirsController
    {
        CharacterService characterSvc;
        PlayerService playerSvc;

        public PlayerController(CharacterService characterService, PlayerService playerService)
        {
            characterSvc = characterService;
            playerSvc = playerService;
        }

        public ActionResult Index()
        {
            var characters = new List<PlayerShortViewModel>();
            try
            {
                characters = characterSvc.GetPlayerShorts(AppUser.UserId);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

            return View(characters);
        }

        public JsonResult SendInvite(PlayerInvitePostModel model)
        {
            var requestKey = Guid.Empty;
            try
            {
                if (model.CharacterShortKey == null) { model.CharacterShortKey = model.CharacterKey; }
                requestKey = playerSvc.SendInvite(AppUser.UserId, model);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Invite sent!", requestKey);
        }

        public JsonResult RespondToInvite(PlayerInviteResponseModel model)
        {
            try
            {
                var userKey = playerSvc.RespondToInvite(model);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Invite sent!");
        }
    }
}