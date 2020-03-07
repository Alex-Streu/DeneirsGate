using DeneirsGate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace MVC_PWx.Controllers
{
    public class UsersController : DeneirsController
    {
        [HttpPost]
        public async Task<JsonResult> SearchUsers(string search)
        {
            var user = new UserViewModel();
            try
            {
                var _user = await UserManager.FindByNameAsync(search);
                if (_user != null)
                {
                    user = new UserViewModel
                    {
                        Picture = _user.Picture,
                        UserId = _user.UserId,
                        Username = _user.UserName
                    };
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Got user successfully!", data = user });
        }

        [HttpPost]
        public JsonResult GetFriends()
        {
            var friends = new List<UserViewModel>();
            try
            {
                friends = UserSvc.GetFriends(AppUser.UserId);
                foreach (var friend in friends)
                {
                    friend.IsOnline = Membership.GetUser(friend.Username).IsOnline;
                }

                friends = friends.OrderBy(x => x.IsOnline).ThenBy(x => x.Username).ToList();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Got friends successfully!", data = friends });
        }

        public ActionResult RegisterPlayer(string id)
        {
            var model = new PlayerRegistryViewModel();
            try
            {
                model = AuthSvc.GetPlayerRegistry(id);
            }
            catch (Exception ex) { }

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
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Character Registration Successful!" });
        }
    }
}