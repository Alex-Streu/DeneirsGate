using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : DeneirsController
    {
        public ActionResult Users()
        {
            var users = UserManager.Users.OrderBy(x => x.UserName).ToList();
            return View(users);
        }

        public ActionResult EditUser(string id)
        {
            var user = UserManager.FindById(id) ?? new ApplicationUser();
            ViewBag.Roles = new SelectList(RoleManager.Roles.ToList(), "Name", "Name", user.Roles.FirstOrDefault());

            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> EditUser(ApplicationUserPostModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(RoleManager.Roles.ToList(), "Name", "Name");
                var _user = await UserManager.FindByIdAsync(model.Id);
                return View(_user);
            }

            var user = await UserManager.FindByIdAsync(model.Id);
            user.LockoutEnabled = model.LockoutEnabled;
            user.LockoutEndDateUtc = model.LockoutEndDateUtc;
            user.PhoneNumber = model.PhoneNumber;
            user.PhoneNumberConfirmed = model.PhoneNumberConfirmed;
            user.Picture = model.Picture;
            user.TwoFactorEnabled = model.TwoFactorEnabled;
            user.UserName = model.UserName;
            var userRoles = user.Roles?.Select(x => x.RoleId).ToList();
            if (userRoles != null)
            {
                foreach (var role in userRoles)
                {
                    var _role = RoleManager.FindById(role);
                    await UserManager.RemoveFromRoleAsync(model.Id, _role.Name);
                }
            }
            await UserManager.AddToRoleAsync(model.Id, model.Role);

            var result = await UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Users");
            }

            ViewBag.Roles = new SelectList(RoleManager.Roles.ToList(), "Name", "Name");
            ModelState.AddModelError("", "Failed to update user.");
            return View(model);
        }

        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                await UserManager.DeleteAsync(user);
            }

            return RedirectToAction("Users");
        }

        public ActionResult Roles()
        {
            var roles = RoleManager.Roles.OrderBy(x => x.Priviledge).ToList();
            return View(roles);
        }

        public ActionResult EditRole(string id)
        {
            var role = RoleManager.FindById(id) ?? new ApplicationRole();

            return View(role);
        }

        [HttpPost]
        public async Task<ActionResult> EditRole(ApplicationRole model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await RoleManager.CreateAsync(model);
            if (result.Succeeded)
            {
                return RedirectToAction("Roles");
            }

            ModelState.AddModelError("", "Failed to add role.");
            return View(model);
        }

        public async Task<ActionResult> DeleteRole(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                await RoleManager.DeleteAsync(role);
            }

            return RedirectToAction("Roles");
        }

        public ActionResult Monsters()
        {
            var users = UserManager.Users.OrderBy(x => x.UserName).ToList();
            return View(users);
        }
    }
}