using DeneirsGate.Services;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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
                CampaignSvc.DeleteUserCampaigns(user.UserId);
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
            return View();
        }

        [HttpPost]
        public JsonResult UploadMonsterList()
        {
            try
            {
                if (Request != null)
                {
                    HttpPostedFileBase monstersfile = Request.Files["Monsters"];
                    if ((monstersfile != null) && (monstersfile.ContentLength > 0) && !string.IsNullOrEmpty(monstersfile.FileName))
                    {
                        byte[] fileBytes = new byte[monstersfile.ContentLength];
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (var package = new ExcelPackage(monstersfile.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfRow = workSheet.Dimension.End.Row;
                            for (var i = 1; i <= noOfRow; i++)
                            {
                                MonsterSvc.UploadMonster(AppUser.UserId, workSheet.Cells[i, 1].Value.ToString(), null, workSheet.Cells[i, 2].Value.ToString(), workSheet.Cells[i, 3].Value.ToString(), workSheet.Cells[i, 4].Value.ToString(), workSheet.Cells[i, 7].Value.ToString(), workSheet.Cells[i, 19].Value.ToString());
                            }
                        }
                    }

                    HttpPostedFileBase environmentsFile = Request.Files["Environments"];
                    if ((environmentsFile != null) && (environmentsFile.ContentLength > 0) && !string.IsNullOrEmpty(environmentsFile.FileName))
                    {
                        byte[] fileBytes = new byte[environmentsFile.ContentLength];
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (var package = new ExcelPackage(environmentsFile.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;
                            for (var i = 1; i <= noOfRow; i++)
                            {
                                var monsters = workSheet.Cells[i, 1].Value.ToString().Split(',');
                                var environment = workSheet.Cells[i, 2].Value.ToString();
                                foreach (var monster in monsters)
                                {
                                    MonsterSvc.UploadMonsterEnvironments(monster.Trim(), environment);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Uploaded successfully!");
        }

        public ActionResult MagicItems()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UploadMagicItemList()
        {
            try
            {
                if (Request != null)
                {
                    HttpPostedFileBase magicItemsFile = Request.Files["MagicItems"];
                    if ((magicItemsFile != null) && (magicItemsFile.ContentLength > 0) && !string.IsNullOrEmpty(magicItemsFile.FileName))
                    {
                        byte[] fileBytes = new byte[magicItemsFile.ContentLength];
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (var package = new ExcelPackage(magicItemsFile.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfRow = workSheet.Dimension.End.Row;
                            for (var i = 2; i <= noOfRow; i++)
                            {
                                MagicItemSvc.UploadMagicItem(AppUser.UserId, workSheet.Cells[i, 1].Value?.ToString(), workSheet.Cells[i, 5].Value?.ToString(), workSheet.Cells[i, 2].Value?.ToString(), workSheet.Cells[i, 3].Value?.ToString(), workSheet.Cells[i, 4].Value?.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Uploaded successfully!");
        }

        public ActionResult Suggestions()
        {
            var model = new List<SuggestionViewModel>();
            try
            {
                model = SuggestionSvc.GetPendingSuggestions(AppUser.UserId, true);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ReviewSuggestion(SuggestionReviewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SuggestionSvc.ReviewSuggestion(model);
                }
                catch (Exception ex)
                {
                    return HandleExceptionJsonErrorResponse(ex);
                }

                return GetJson(true, "Reviewed successfully!");
            }
            return HandleValidationJsonErrorResponse();
        }
    }
}