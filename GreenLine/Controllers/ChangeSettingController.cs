using GreenLine.Classes;
using GreenLine.Models;
using GreenLine.Security;
using GreenLineDataService.Helper.Interface;
using Microsoft.Web.WebPages.OAuth;
using System;
using System.Data;
using System.IO;
using System.Web.Mvc;
using WebMatrix.WebData;
using GreenLineDataService.Helper;
using GreenLineDataService.Models;

namespace GreenLine.Controllers
{
    [Authorize]
    public class ChangeSettingController : BaseController
    {
        //
        // GET: /ChangeSetting/

      //  readonly MasterService MS = new MasterService();
        readonly GeneralFunctions GF = new GeneralFunctions();

        public readonly IMasterService MS;

        public ChangeSettingController()
        {
            MS = new MasterService();
        }
        public ActionResult Index()
        {
            ChangeSettingsViewModel CSV = new ChangeSettingsViewModel
            {
                changeSetting = new ChangeSetting
                {
                    LocCode = BaseLocationCode,
                    FinYear = BaseFinYear,
                    CompanyCode = BaseCompanyCode
                },
                ListCompany = MS.GetCompanyMappedToEmployee(User.Identity.Name),
                ListLocation = MS.GetWorkingLocationsNewPortal(BaseLocationCode, MainLocCode, BaseUserName.ToUpper()),
                ListYears = MS.GetFinacialYearDetails(),
                submitType = 1
            };
            //if (BaseUserName.ToUpper() == "ADMIN")
            //{
            //    CSV.ListYears.Add(new vw_Get_Finacial_Years { FinYear = "2015-2016", YearVal = "2015-2016" });
            //    CSV.ListYears.Add(new vw_Get_Finacial_Years { FinYear = "2014-2015", YearVal = "2014-2015" });
            //}            
            return View(CSV);
        }

        public ActionResult popUpIndex()
        {
            ChangeSettingsViewModel CSV = new ChangeSettingsViewModel
            {
                ListCompany = MS.GetCompanyMappedToEmployee(User.Identity.Name),
                ListLocation = MS.GetWorkingLocationsNewPortal(BaseLocationCode, MainLocCode, BaseUserName.ToUpper()),
                ListYears = MS.GetFinacialYearDetails(),
                changeSetting = new ChangeSetting
                {
                    LocCode = BaseLocationCode,
                    FinYear = BaseFinYear,
                    CompanyCode = BaseCompanyCode
                },
                submitType = 2
            };
            //if (BaseUserName.ToUpper() == "ADMIN")
            //{
            //    CSV.ListYears.Add(new vw_Get_Finacial_Years { FinYear = "2015-2016", YearVal = "2015-2016" });
            //    CSV.ListYears.Add(new vw_Get_Finacial_Years { FinYear = "2014-2015", YearVal = "2014-2015" });
            //}
            return PartialView("_ChangeSetting", CSV);
        }

        //[HttpPost]
        //public ActionResult SaveSettings(ChangeSettingsViewModel CSV)
        //{
        //    CustomPrincipal.setChangeLocation(BaseUserName, CSV.changeSetting.LocCode, CSV.changeSetting.FinYear, CSV.changeSetting.CompanyCode);
        //    return RedirectToAction("Index", "Home");
        //}

        public ActionResult SaveChangeSettings(ChangeSettingsViewModel CSV)
        {
            try
            {
                CustomPrincipal.SetChangeLocation(BaseUserName, CSV.changeSetting.LocCode, CSV.changeSetting.FinYear, CSV.changeSetting.CompanyCode);
                string UserMenufolderPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/UserMenu/");
                GF.DeleteUserMenuTextFiles(BaseUserName);

                string App_DatafolderPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/");
                string curFile1 = App_DatafolderPath + "UserMenuRights_" + User.Identity.Name + ".xml";
                string curFile2 = App_DatafolderPath + "UserMenuRights_" + User.Identity.Name + ".xml";
                FileInfo file = new FileInfo(curFile1);
                if (file.Exists)//check file exsit or not  
                {
                    file.Delete();
                }
                file = new FileInfo(curFile2);
                if (file.Exists)//check file exsit or not  
                {
                    file.Delete();
                }

                if (CSV.submitType == 1)
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (CSV.submitType == 2)
                {
                    string sameFinYear = "Y", status = "success";
                    if (BaseFinYear != CSV.changeSetting.FinYear)
                    {
                        sameFinYear = "N";
                    }
                    status = status + "-" + sameFinYear;
                    return Json(status);
                }
                else
                {
                    return Json("fail");
                }
            }
            catch (Exception)
            {
                return Json("fail");
                throw;
            }
        }

        public ActionResult ChangePassword()
        {
            ViewBag.UserId = User.Identity.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    string psSult = "WebX";
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {

                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                        if (changePasswordSucceeded)
                        {
                            string commandText1 = "Update CYGNUS_Master_Users set LastPwd = UserPwd, UserPwd='" + GF.Encrypt(model.NewPassword, psSult) + "' , PwdLastChangeOn = GETDATE() Where UserId ='" + User.Identity.Name + "'";
                            DataTable dataTable1 = GF.GetDateTableFromQuery(commandText1);

                            return RedirectToAction("ChangePasswordSuccess", "ChangeSetting", new { Message = ManageMessageId.ChangePasswordSuccess });
                        }
                        else
                        {
                            TempData["Message"] = "The current password is incorrect or the new password is invalid.";
                            ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                        }
                    }
                    catch (Exception)
                    {
                        TempData["Message"] = "The current password is incorrect or the new password is invalid.";
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                        //changePasswordSucceeded = false;
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
                        return RedirectToAction("ChangePasswordSuccess", "ChangeSetting", new { Message = ManageMessageId.SetPasswordSuccess });
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

        public ActionResult ChangePasswordSuccess(string Message)
        {
            ViewBag.Message = Message;
            return View();
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }
    }
}
