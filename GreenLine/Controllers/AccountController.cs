using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Drawing;
using GreenLine.Classes;
using GreenLine.Models;
using GreenLine.Security;
using WebMatrix.WebData;
using Microsoft.Web.WebPages.OAuth;
using System.Transactions;
using DotNetOpenAuth.AspNet;
using GreenLineDataService.Helper.Interface;
using GreenLineDataService.Helper;

namespace GreenLine.Controllers
{
    [Authorize]
    //[InitializeSimpleMembership]
    public class AccountController : Controller
    {
        //
        readonly GeneralFunctions GF = new GeneralFunctions();
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated == true)
            {
                if (string.IsNullOrEmpty(returnUrl) || returnUrl == "/" || returnUrl.ToLower().Contains("account/login"))
                {
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToLocal(returnUrl);
            }
            else
            {
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
        }

        private string GetClientIpAddress(string clientProvidedIp = null)
        {
            string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.UserHostAddress;
            }
            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.ServerVariables["REMOTE_ADDR"];
            }

            if (!string.IsNullOrEmpty(ip))
            {
                ip = ip.Split(',')[0].Trim();
            }

            // If the IP resolves to localhost (IPv4 or IPv6) and we have a valid client-side public IP, use it instead.
            if ((ip == "::1" || ip == "127.0.0.1") && !string.IsNullOrWhiteSpace(clientProvidedIp))
            {
                return clientProvidedIp;
            }

            return ip ?? string.Empty;
        }

        //
        // POST: /Account/SendOTP (AJAX)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SendOTP(OtpLoginModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.MobileNo))
                {
                    return Json(new { success = false, message = "Please enter your mobile number." });
                }

                // Sanitize input & capture browser info
                string mobileNo = model.MobileNo.Trim();
                string serverIp = GetClientIpAddress(model.LoginIP);
                string userAgent = Request.UserAgent ?? "";

                // Call the stored procedure to generate and store OTP
                System.Data.SqlClient.SqlParameter[] parameters = new System.Data.SqlClient.SqlParameter[]
                {
                    new System.Data.SqlClient.SqlParameter("@MobileNo", mobileNo),
                    new System.Data.SqlClient.SqlParameter("@IPAddress", string.IsNullOrEmpty(serverIp) ? (object)DBNull.Value : serverIp),
                    new System.Data.SqlClient.SqlParameter("@UserAgent", string.IsNullOrEmpty(userAgent) ? (object)DBNull.Value : userAgent),
                    new System.Data.SqlClient.SqlParameter("@ForceNewOTP", model.ForceNewOTP)
                };

                DataSet ds = GF.GetDataSetFromSP_New("USP_InsertOTP", parameters);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string status = ds.Tables[0].Rows[0]["Status"].ToString();
                    string message = ds.Tables[0].Rows[0]["Message"].ToString();

                    if (status == "0")
                    {
                        // For UAT/testing: return OTP in response for toast display
                        // TODO: In production, remove otpCode from response and send via SMS API (MSG91/Twilio)
                        string otpCode = ds.Tables[0].Rows[0]["OTPCode"].ToString();
                        return Json(new { success = true, message = message, otpCode = otpCode, alreadyVerified = false });
                    }
                    else
                    {
                        return Json(new { success = false, message = message });
                    }
                }

                return Json(new { success = false, message = "Failed to process request. Please try again." });
            }
            catch (Exception ex)
            {
                // Log exception
                return Json(new { success = false, message = "An error occurred. Please try again later." });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult DirectLoginAfterDailyOTP(OtpLoginModel model, string returnUrl, int? LgType, string Mlocation)
        {
            string mobileNo = "";
            string userName = "";
            string serverIp = "";
            string userAgent = "";
            string sessionId = "";

            try
            {
                if (string.IsNullOrWhiteSpace(model.MobileNo))
                {
                    return Json(new { success = false, message = "Mobile number is required." });
                }

                mobileNo = model.MobileNo.Trim();
                serverIp = GetClientIpAddress(model.LoginIP);
                userAgent = Request.UserAgent ?? "";
                sessionId = Session.SessionID ?? "";

                // Step 1: Verify that today's OTP is actually verified in the database
                System.Data.SqlClient.SqlParameter[] checkParams = new System.Data.SqlClient.SqlParameter[]
                {
                    new System.Data.SqlClient.SqlParameter("@MobileNo", mobileNo)
                };

                DataSet checkResult = GF.GetDataSetFromSP_New("USP_CheckDailyOTPStatus", checkParams);

                if (checkResult == null || checkResult.Tables.Count == 0 || checkResult.Tables[0].Rows.Count == 0)
                {
                    return Json(new { success = false, message = "Verification check failed. Please try again." });
                }

                string status = checkResult.Tables[0].Rows[0]["Status"].ToString();
                userName = checkResult.Tables[0].Rows[0]["UserName"].ToString();
                bool isVerifiedToday = Convert.ToBoolean(checkResult.Tables[0].Rows[0]["IsVerifiedToday"]);

                if (status != "0" || !isVerifiedToday)
                {
                    // OTP not verified today — cannot do direct login, force OTP flow
                    return Json(new { success = false, message = "OTP verification required. Please request and verify an OTP." });
                }

                // Step 2: OTP is verified for today — fetch user details and create auth session
                IMasterService MS = new MasterService();
                DataSet userDetails = MS.CheckValidUserforLogin(userName, "", "", "");

                if (userDetails == null || userDetails.Tables.Count < 2 || userDetails.Tables[1].Rows.Count == 0)
                {
                    LogLoginHistory(userName, mobileNo, false, "User account not found (daily re-login)", serverIp, userAgent, model, sessionId);
                    return Json(new { success = false, message = "User account not found. Please contact administrator." });
                }

                // Step 3: Build LoginModel and authenticate
                LoginModel loginModel = new LoginModel
                {
                    UserName = userName,
                    Password = "",
                    RememberMe = false,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    LoginCity = model.LoginCity,
                    LoginCountry = model.LoginCountry,
                    LoginCountryCode = model.LoginCountryCode,
                    LoginIP = serverIp,
                    CaptchaInput = ""
                };

                if (CustomPrincipal.Login(loginModel, userDetails.Tables[1]))
                {
                    // Override cookie to expire at end of today
                    var existingCookie = Response.Cookies[FormsAuthentication.FormsCookieName];
                    if (existingCookie != null)
                    {
                        existingCookie.Expires = DateTime.Today.AddDays(1);
                    }

                    // Log successful login (daily re-login)
                    LogLoginHistory(userName, mobileNo, true, null, serverIp, userAgent, model, sessionId);

                    // Determine redirect URL
                    string redirectUrl = GetRedirectUrl(returnUrl, LgType, Mlocation);

                    TempData["ForceHardReload"] = true;
                    return Json(new { success = true, message = "Welcome back! Logged in successfully.", redirectUrl = redirectUrl });
                }

                LogLoginHistory(userName, mobileNo, false, "Authentication failed (daily re-login)", serverIp, userAgent, model, sessionId);
                return Json(new { success = false, message = "Authentication failed. Please contact administrator." });
            }
            catch (Exception ex)
            {
                LogLoginHistory(string.IsNullOrEmpty(userName) ? mobileNo : userName, mobileNo, false, "Exception (daily re-login): " + ex.Message, serverIp, userAgent, model, sessionId);
                return Json(new { success = false, message = "An error occurred. Please try again." });
            }
        }

        //
        // POST: /Account/VerifyOTP (AJAX)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyOTP(OtpLoginModel model, string returnUrl, int? LgType, string Mlocation)
        {
            string mobileNo = "";
            string userName = "";
            string serverIp = "";
            string userAgent = "";
            string sessionId = "";

            try
            {
                if (string.IsNullOrWhiteSpace(model.MobileNo) || string.IsNullOrWhiteSpace(model.OTPCode))
                {
                    return Json(new { success = false, message = "Mobile number and OTP are required." });
                }

                mobileNo = model.MobileNo.Trim();
                string otpCode = model.OTPCode.Trim();
                serverIp = GetClientIpAddress(model.LoginIP);
                userAgent = Request.UserAgent ?? "";
                sessionId = Session.SessionID ?? "";

                // Step 1: Verify OTP via stored procedure
                System.Data.SqlClient.SqlParameter[] verifyParams = new System.Data.SqlClient.SqlParameter[]
                {
                    new System.Data.SqlClient.SqlParameter("@MobileNo", mobileNo),
                    new System.Data.SqlClient.SqlParameter("@OTPCode", otpCode),
                    new System.Data.SqlClient.SqlParameter("@IPAddress", string.IsNullOrEmpty(serverIp) ? (object)DBNull.Value : serverIp),
                    new System.Data.SqlClient.SqlParameter("@UserAgent", string.IsNullOrEmpty(userAgent) ? (object)DBNull.Value : userAgent)
                };

                DataSet verifyResult = GF.GetDataSetFromSP_New("USP_VerifyOTP", verifyParams);

                if (verifyResult == null || verifyResult.Tables.Count == 0 || verifyResult.Tables[0].Rows.Count == 0)
                {
                    return Json(new { success = false, message = "Verification failed. Please try again." });
                }

                string status = verifyResult.Tables[0].Rows[0]["Status"].ToString();
                string message = verifyResult.Tables[0].Rows[0]["Message"].ToString();
                userName = verifyResult.Tables[0].Rows[0]["UserName"].ToString();

                if (status != "0")
                {
                    // OTP verification failed (expired, invalidated, wrong code, etc.)
                    LogLoginHistory(string.IsNullOrEmpty(userName) ? mobileNo : userName, mobileNo, false, message, serverIp, userAgent, model, sessionId);
                    return Json(new { success = false, message = message });
                }

                // Step 2: OTP verified — now fetch user details and create auth session
                IMasterService MS = new MasterService();
                DataSet userDetails = MS.CheckValidUserforLogin(userName, "", "", "");

                if (userDetails == null || userDetails.Tables.Count < 2 || userDetails.Tables[1].Rows.Count == 0)
                {
                    LogLoginHistory(userName, mobileNo, false, "User account not found", serverIp, userAgent, model, sessionId);
                    return Json(new { success = false, message = "User account not found. Please contact administrator." });
                }

                // Step 3: Build a LoginModel for the existing CustomPrincipal.Login() method
                LoginModel loginModel = new LoginModel
                {
                    UserName = userName,
                    Password = "", // Not used in OTP flow
                    RememberMe = false,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    LoginCity = model.LoginCity,
                    LoginCountry = model.LoginCountry,
                    LoginCountryCode = model.LoginCountryCode,
                    LoginIP = serverIp,
                    CaptchaInput = "" // Not used in OTP flow
                };

                // Step 4: Authenticate using existing infrastructure
                if (CustomPrincipal.Login(loginModel, userDetails.Tables[1]))
                {
                    // Step 5: Override cookie to expire at the specific end of today (Today's date wise)
                    // This ensures that the user MUST request a new OTP on the next calendar day.
                    var existingCookie = Response.Cookies[FormsAuthentication.FormsCookieName];
                    if (existingCookie != null)
                    {
                        existingCookie.Expires = DateTime.Today.AddDays(1);
                    }

                    // Log successful login
                    LogLoginHistory(userName, mobileNo, true, null, serverIp, userAgent, model, sessionId);

                    // Step 6: Determine redirect URL
                    string redirectUrl = GetRedirectUrl(returnUrl, LgType, Mlocation);

                    TempData["ForceHardReload"] = true;
                    return Json(new { success = true, message = "Login successful!", redirectUrl = redirectUrl });
                }

                LogLoginHistory(userName, mobileNo, false, "Authentication failed", serverIp, userAgent, model, sessionId);
                return Json(new { success = false, message = "Authentication failed. Please contact administrator." });
            }
            catch (Exception ex)
            {
                LogLoginHistory(string.IsNullOrEmpty(userName) ? mobileNo : userName, mobileNo, false, "Exception: " + ex.Message, serverIp, userAgent, model, sessionId);
                return Json(new { success = false, message = "An error occurred during verification. Please try again." });
            }
        }

        /// <summary>
        /// Determines the post-login redirect URL based on user type and request parameters.
        /// Shared by VerifyOTP and DirectLoginAfterDailyOTP to avoid code duplication.
        /// </summary>
        private string GetRedirectUrl(string returnUrl, int? LgType, string Mlocation)
        {
            try
            {
                var empType = ((CustomIdentity)User.Identity).EmpType;

                if (!string.IsNullOrEmpty(empType) && empType == "10")
                {
                    return Url.Action("Index", "CustomerPortal");
                }
                else if (LgType == 1)
                {
                    return Url.Action("LoadingSheet", "MobileApplication", new { Type = "MF", IsMobileUser = "1", BaseLocation = Mlocation });
                }
                else if (LgType == 2)
                {
                    return Url.Action("Challan", "MobileApplication", new { TYP = "1", IsMobileUser = "1", BaseLocation = Mlocation });
                }
                else if (LgType == 3)
                {
                    return Url.Action("ArrivalUpdate", "MobileApplication", new { id = "1", IsMobileUser = "1", BaseLocation = Mlocation });
                }
                else if (LgType == 4)
                {
                    return Url.Action("ArrivalUpdate", "MobileApplication", new { id = "2", IsMobileUser = "1", BaseLocation = Mlocation });
                }
                else if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return returnUrl;
                }
            }
            catch { /* Fallback to Home */ }

            return Url.Action("Index", "Home");
        }

        /// <summary>
        /// Logs every login attempt (success or failure) to TBL_Login_History.
        /// This method is fire-and-forget — it never throws exceptions to avoid breaking the login flow.
        /// </summary>
        private void LogLoginHistory(string userName, string mobileNo, bool isSuccess, string failReason,
            string ipAddress, string userAgent, OtpLoginModel model, string sessionId)
        {
            try
            {
                string deviceType = GetDeviceType(userAgent);

                System.Data.SqlClient.SqlParameter[] logParams = new System.Data.SqlClient.SqlParameter[]
                {
                    new System.Data.SqlClient.SqlParameter("@UserName", (object)userName ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@MobileNo", (object)mobileNo ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@LoginMethod", "OTP"),
                    new System.Data.SqlClient.SqlParameter("@DeviceType", (object)deviceType ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@IsSuccess", isSuccess),
                    new System.Data.SqlClient.SqlParameter("@FailReason", (object)failReason ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@IPAddress", (object)ipAddress ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@UserAgent", (object)userAgent ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@LoginCity", (object)(model != null ? model.LoginCity : null) ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@LoginCountry", (object)(model != null ? model.LoginCountry : null) ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@Latitude", (object)(model != null ? model.Latitude : null) ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@Longitude", (object)(model != null ? model.Longitude : null) ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@SessionId", (object)sessionId ?? DBNull.Value)
                };
                GF.GetDataSetFromSP_New("USP_InsertLoginHistory", logParams);
            }
            catch { /* Logging should never break the login flow */ }
        }

        /// <summary>
        /// Analyzes the User Agent to return the specific device or OS name.
        /// </summary>
        private string GetDeviceType(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "Unknown";

            string ua = userAgent.ToLower();

            if (ua.Contains("windows phone"))
                return "Windows Phone";
            if (ua.Contains("iphone"))
                return "iPhone";
            if (ua.Contains("ipad"))
                return "iPad";
            if (ua.Contains("android"))
                return "Android Device";
            if (ua.Contains("macintosh") || ua.Contains("mac os x"))
                return "Mac Desktop";
            if (ua.Contains("windows nt"))
                return "Windows Desktop";
            if (ua.Contains("linux"))
                return "Linux Desktop";

            return "Web Browser";
        }

        //
        // POST: /Account/LogOff

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            CustomPrincipal.Logout();
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();

            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);

            return RedirectToAction("Login", "Account");
        }

        // GET version for simple redirects or JS trigger
        [AllowAnonymous]
        public ActionResult Logout(string returnUrl)
        {
            CustomPrincipal.Logout();
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();

            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);

            return RedirectToAction("Login", "Account", new { returnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult KeepAlive()
        {
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }
        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out string provider, out string providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult DeleteAllFile()
        {
            GeneralFunctions GF = new GeneralFunctions();
            GF.DeleteAllFile();
            return null;
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        public string GetData(string Method, IDictionary<string, string> parameters)
        {
            GeneralFunctions GF = new GeneralFunctions();
            System.Data.DataSet DS = GF.GetdatasetFromParams("USP_" + Method, parameters);
            if (DS.Tables.Count == 1)
            {
                return JsonConvert.SerializeObject(DS.Tables[0]);
            }
            else
            {
                return JsonConvert.SerializeObject(DS);
            }
        }

        public string GetDataWithoutAlias(string Method, IDictionary<string, string> parameters)
        {
            GeneralFunctions GF = new GeneralFunctions();
            System.Data.DataSet DS = GF.GetdatasetFromParams(Method, parameters);
            if (DS.Tables.Count == 1)
            {
                return JsonConvert.SerializeObject(DS.Tables[0]);
            }
            else
            {
                return JsonConvert.SerializeObject(DS);
            }
        }

        public string GetDataWithoutAliasAsync(string SQRY)
        {
            GeneralFunctions GF = new GeneralFunctions();
            System.Data.DataTable DS = GF.GetDateTableFromQuery_Async(SQRY);
            if (DS.Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(DS);
            }
            else
            {
                return JsonConvert.SerializeObject(new DataTable());
            }
        }

        public string GetPRQData(string Method)
        {
            DataSet DS = GF.GetDataSetFromSP(Method);
            if (DS.Tables.Count == 1)
            {
                return JsonConvert.SerializeObject(DS.Tables[0]);
            }
            else
            {
                return JsonConvert.SerializeObject(DS);
            }
        }
        public string SetData(string Method, IDictionary<string, string> parameters)
        {
            GeneralFunctions GF = new GeneralFunctions();
            System.Data.DataSet DS = GF.GetdatasetFromParams("USP_" + Method, parameters);
            if (DS.Tables.Count == 1)
            {
                return JsonConvert.SerializeObject(DS.Tables[0]);
            }
            else
            {
                return JsonConvert.SerializeObject(DS);
            }
        }


    }
}
