using System;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DeneirsGate.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using DeneirsGateSite.Helpers;
using Unity;

namespace DeneirsGateSite.Controllers
{
    [Authorize]
    public class AccountController : DeneirsController
    {
        private AuthService authSvc;

        public AccountController()
        {

        }

        [InjectionConstructor]
        public AccountController(AuthService authService)
        {
            authSvc = authService;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        [AnonymousOnly]
        public ActionResult Login(string returnUrl)
        {
            var user = UserManager.FindByName("TestRegistration");
            if (user != null) { UserManager.Delete(user); }

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Role = RoleManager.Roles.Where(x => x.Name != "Admin").OrderBy(x => x.Priviledge).ToList();
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous, AnonymousOnly]
        public async Task<JsonResult> AttemptLogin(LoginPostModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Require the user to have a confirmed email before they can log on.
                    var user = await UserManager.FindByNameOrEmailAsync(model.Username);
                    if (user != null)
                    {
                        if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                        {
                            //string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account - Resend");
                            
                            return GetJson(false, null, null, new ErrorPostModel
                            {
                                ErrorCode = 1
                            });
                        }
                    }

                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, change to shouldLockout: true
                    var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: false);
                    switch (result)
                    {
                        case SignInStatus.Success:
                            user.LastLoginDate = DateTime.UtcNow;
                            SetActiveCampaign(user.ActiveCampaign, false);
                            await UserManager.UpdateAsync(user);
                            AppUser = user;
                            return GetJson(true, "Login successful!<br/>Opening the gate...", model.ReturnUrl ?? Url.Action("/", "Campaign"));
                        //case SignInStatus.LockedOut:
                        //    return View("Lockout");
                        //case SignInStatus.RequiresVerification:
                        //    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            break;
                    }
                }

                return HandleValidationJsonErrorResponse();
            }
            catch (Exception ex)
            {
                GetJson(new ErrorPostModel
                {
                    Header = "Oops! Looks like something went wrong!",
                    Message = "We apologize for the inconvenience. Our team has been notified and will work on resolving this issue as soon as possible.",
                    ReturnUrl = Url.Action("Login")
                }, ex);
            }

            return GetJson(false, "Oops! Something went wrong. Please try again later!");
        }

        [HttpPost, AllowAnonymous]
        public async Task<JsonResult> ResendConfirmation(ResendConfirmationPostModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await UserManager.FindByNameOrEmailAsync(model.Username);
                    if (user != null)
                    {
                        string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account - Resend");

                        return GetJson(true, "Confirmation has been resent!");
                    }
                }

                return HandleValidationJsonErrorResponse();
            }
            catch (Exception ex)
            {
                GetJson(new ErrorPostModel
                {
                    Header = "Oops! Looks like something went wrong!",
                    Message = "We apologize for the inconvenience. Our team has been notified and will work on resolving this issue as soon as possible.",
                    ReturnUrl = Url.Action("Login")
                }, ex);
            }

            return GetJson(false, "Oops! Something went wrong. Please try again later!");
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [AnonymousOnly]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }
        
        [HttpPost]
        [AllowAnonymous]
        [AnonymousOnly]
        public async Task<JsonResult> Register(RegisterViewModel model)
        {
            string response;
            try
            {
                var allowRegistration = Boolean.Parse(ConfigurationManager.AppSettings["allowRegistration"] ?? "false");
                if (!allowRegistration)
                {
                    throw new Exception("We're sorry. Registration has been temporarily disabled.");
                }

                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser { UserName = model.Username, Picture = model.Picture, Email = model.Email, CreatedDate = DateTime.UtcNow };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        user = await UserManager.FindByNameAsync(model.Username);
                        user.UserId = new Guid(user.Id);
                        await UserManager.UpdateAsync(user);
                        await UserManager.AddToRoleAsync(user.Id, model.Role);

                        //  Comment the following line to prevent log in until the user is confirmed.
                        //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                        string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account");

                        return GetJson(true, "Check your email for a confirmation link, the enjoy your time in the gates!", "Welcome adventurer!");
                    }
                    else
                    {
                        throw new Exception(result.Errors.FirstOrDefault());
                    }
                }
                else
                {
                    throw new Exception(GetValidationError());
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            return Json(new { success = false, message = response ?? "We're sorry. There seems to be an obstacle in our path. Please try again later." });
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous, AnonymousOnly]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                ViewBag.Header = "Welcome, adventurer!";
                ViewBag.Message = "Your messenger owl is confirmed and you are ready to enter the gates!";
                ViewBag.Html = $"<a class='btn btn-lg btn-default mt' href='{Url.Action("Login")}'>Log In</a>";
            }
            else
            {
                ViewBag.Header = "You've made a wrong turn!";
                ViewBag.Message = "We were unable to confirm your email.";
            }
            return View("Info");
        }
        
        [HttpPost, AllowAnonymous]
        public async Task<JsonResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (model.Email.IsNullOrEmpty() && AppUser != null) { model.Email = AppUser.Email; }
            else if (!ModelState.IsValid)
            {
                return HandleValidationJsonErrorResponse();
            }

            try
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return GetJson(true, "Please check your email to reset your password.");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { code }, protocol: Request.Url.Scheme);

                authSvc.SetPasswordResetCode(user.UserId, code);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", AppLogic.GetEmailBody("Did you request a password change?", "If this was you, I get it. Happens all the time.<br/><br/>You can reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>.<br/><br/>This link will only be valid for 1 hour."));
                return GetJson(true, "Please check your email to reset your password.");
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            var model = new ResetPasswordViewModel();
            try
            {
                var userId = authSvc.GetPasswordResetUser(code);
                var user = UserManager.FindById(userId.ToString());
                model = new ResetPasswordViewModel
                {
                    Code = code,
                    Email = user.Email
                };
            }
            catch (Exception ex)
            {
                HandleExceptionRedirectError(ex);
            }

            return View(model);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            ViewBag.Reset = false;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = UserManager.FindByEmail(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    ViewBag.Reset = true;
                    LogError($"Invalid user using reset password: {model.Email}");
                    return View(model);
                }

                var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                if (result.Succeeded)
                {
                    authSvc.ClearPasswordReset(user.UserId);
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    Session.Abandon();
                    ViewBag.Reset = true;
                    return View(model);
                }

                AddErrors(result);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

            return View(model);
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Abandon();
            RemoveOnlineUser();
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            return RedirectToAction("Login");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [HasAccess(Priviledge = AppLogic.Priviledge.Player)]
        public ActionResult EditProfile()
        {
            var profile = new ProfileViewModel(AppUser.UserName, AppUser.Picture, AppUser.Email);
            return View(profile);
        }

        [HasAccess(Priviledge = AppLogic.Priviledge.Player)]
        public JsonResult UpdateProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = UserManager.FindById(AppUser.UserId.ToString());
                    user.UserName = model.UserName;
                    user.Picture = model.Picture;
                    user.Email = model.Email;

                    var response = UserManager.Update(user);
                    if (!response.Succeeded)
                    {
                        return GetJson(false, response.Errors.FirstOrDefault());
                    }

                    AppUser = user;
                }
                catch (Exception ex)
                {
                    return HandleExceptionJsonErrorResponse(ex);
                }

                return GetJson(true, "Updated successfully!");
            }
            return HandleValidationJsonErrorResponse();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (SignInManager != null)
                {
                    SignInManager.Dispose();
                    SignInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = userID, code }, protocol: Request.Url.Scheme);
            var body = $"You'll need to confirm your account before you get to log in.<br/><br/>You can confirm your account by clicking <a href='{callbackUrl}'>here</a>!";
            await UserManager.SendEmailAsync(userID, subject, AppLogic.GetEmailBody("Congrats on your new account!", body));

            return callbackUrl;
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}