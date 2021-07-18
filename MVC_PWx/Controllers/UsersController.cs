using DeneirsGate.Services;
using Sentry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DeneirsGateSite.Controllers
{
    [Authorize]
    public class UsersController : DeneirsController
    {
        ApplicationUserManager userManager;
        UserService userSvc;
        AuthService authSvc;

        public UsersController(ApplicationUserManager userManager, UserService userService, AuthService authService)
        {
            this.userManager = userManager;
            userSvc = userService;
            authSvc = authService;
        }

        public ActionResult Friends(string to = "friends")
        {
            var friends = new AllFriendsViewModel();
            try
            {
                friends = userSvc.GetAllFriends(AppUser.UserId, OnlineUsers);

                var toList = new List<string> { "friends", "pending", "requests", "blocked" };
                if (!toList.Contains(to)) { to = "friends"; }
                ViewBag.To = to;
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

            return View(friends);
        }
        
        public ActionResult _Friends(Guid id, List<FriendViewModel> list = null)
        {
            var friends = new List<FriendViewModel>();
            try
            {
                if (list != null) { friends = list; }
                else { friends = userSvc.GetFriends(AppUser.UserId, OnlineUsers); }
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(friends);
        }
        
        public ActionResult _Requests(Guid id, List<FriendViewModel> list = null)
        {
            var friends = new List<FriendViewModel>();
            try
            {
                if (list != null) { friends = list; }
                else { friends = userSvc.GetRequests(AppUser.UserId); }
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(friends);
        }
        
        public ActionResult _Pending(Guid id, List<FriendViewModel> list = null)
        {
            var friends = new List<FriendViewModel>();
            try
            {
                if (list != null) { friends = list; }
                else { friends = userSvc.GetPending(AppUser.UserId); }
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(friends);
        }
        
        public ActionResult _Blocked(Guid id, List<FriendViewModel> list = null)
        {
            var friends = new List<FriendViewModel>();
            try
            {
                if (list != null) { friends = list; }
                else { friends = userSvc.GetBlocked(AppUser.UserId); }
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(friends);
        }

        [HttpPost]
        public JsonResult ReadNotification(Guid id)
        {
            try
            {
                userSvc.ReadNotification(id);
            }
            catch (Exception ex)
            {
                HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true);
        }

        [HttpPost]
        public JsonResult DeleteNotification(Guid id)
        {
            try
            {
                userSvc.DeleteNotification(id);
            }
            catch (Exception ex)
            {
                HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true);
        }

        [HttpPost]
        public async Task<JsonResult> SearchUsers(SearchUserPostModel model)
        {
            var user = new SearchUserViewModel();
            var status = FriendStatus.None;
            try
            {
                var _user = await userManager.FindByNameAsync(model.Search);
                if (_user != null) { status = userSvc.CheckFriendStatus(AppUser.UserId, _user.UserId); }
                if (_user == null || status == FriendStatus.Blocked || _user.UserName == User.Identity.Name)
                {
                    throw new Exception($"No user with username '{model.Search}' was found!");
                }
                
                user = new SearchUserViewModel
                {
                    Picture = _user.Picture,
                    UserId = _user.UserId,
                    Username = _user.UserName,
                    Status = userSvc.CheckFriendStatus(AppUser.UserId, _user.UserId)
                };

                switch (user.Status)
                {
                    case FriendStatus.None:
                        user.CanAdd = true;
                        user.StatusMessage = "Make a friend!";
                        break;
                    case FriendStatus.Accepted:
                        user.CanAdd = false;
                        user.StatusMessage = "Already friends!";
                        break;
                    case FriendStatus.Pending:
                        user.CanAdd = false;
                        user.StatusMessage = "Request pending...";
                        break;
                }
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Got user successfully!", user);
        }

        [HttpPost]
        public async Task<JsonResult> SendFriendRequest(FriendRequestPostModel model)
        {
            try
            {
                var requestUser = await userManager.FindByNameAsync(model.RequestUserName);
                if (requestUser == null) { throw new Exception("User not found!"); }

                model.RequestUserId = new Guid(requestUser.Id);
                userSvc.SendFriendRequest(AppUser.UserId, model);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Request sent!");
        }

        [HttpPost]
        public JsonResult UpdateFriendRequest(UpdateFriendRequestPostModel model)
        {
            try
            {
                userSvc.UpdateFriendRequest(AppUser.UserId, model.SenderId, model.Status);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Updated successfully!");
        }

        public ActionResult RegisterPlayer(string id)
        {
            var model = new PlayerRegistryViewModel();
            try
            {
                model = authSvc.GetPlayerRegistry(id);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult RegisterPlayer(PlayerRegistryPostModel model)
        {
            try
            {
                authSvc.UpdatePlayerRegistry(AppUser.UserId, model);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Character registration successful!");
        }
    }
}