using DeneirsGate.Services;
using Sentry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    public class UsersController : DeneirsController
    {
        public ActionResult Friends(string to = "friends")
        {
            var friends = new AllFriendsViewModel();
            try
            {
                friends = UserSvc.GetAllFriends(AppUser.UserId, OnlineUsers);

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
                else { friends = UserSvc.GetFriends(AppUser.UserId, OnlineUsers); }
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
                else { friends = UserSvc.GetRequests(AppUser.UserId); }
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
                else { friends = UserSvc.GetPending(AppUser.UserId); }
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
                else { friends = UserSvc.GetBlocked(AppUser.UserId); }
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(friends);
        }

        public ActionResult ReadNotification(Guid id, string returnUrl)
        {
            try
            {
                UserSvc.ReadNotification(id);
            }
            catch (Exception ex)
            {
                SentrySdk.WithScope(scope =>
                {
                    scope.User = new User
                    {
                        Username = AppUser.UserName
                    };
                    SentrySdk.CaptureException(ex);
                });
            }

            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        public async Task<JsonResult> SearchUsers(SearchUserPostModel model)
        {
            var user = new SearchUserViewModel();
            var status = FriendStatus.None;
            try
            {
                var _user = await UserManager.FindByNameAsync(model.Search);
                if (_user != null) { status = UserSvc.CheckFriendStatus(AppUser.UserId, _user.UserId); }
                if (_user == null || status == FriendStatus.Blocked || _user.UserName == User.Identity.Name)
                {
                    throw new Exception($"No user with username '{model.Search}' was found!");
                }
                
                user = new SearchUserViewModel
                {
                    Picture = _user.Picture,
                    UserId = _user.UserId,
                    Username = _user.UserName,
                    Status = UserSvc.CheckFriendStatus(AppUser.UserId, _user.UserId)
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
                var requestUser = await UserManager.FindByNameAsync(model.RequestUserName);
                if (requestUser == null) { throw new Exception("User not found!"); }

                model.RequestUserId = new Guid(requestUser.Id);
                UserSvc.SendFriendRequest(AppUser.UserId, model);
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
                UserSvc.UpdateFriendRequest(AppUser.UserId, model.SenderId, model.Status);
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
                model = AuthSvc.GetPlayerRegistry(id);
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
                AuthSvc.UpdatePlayerRegistry(AppUser.UserId, model);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Character registration successful!");
        }
    }
}