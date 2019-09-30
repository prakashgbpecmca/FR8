using Frayte.Services.Business;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;
using System.Web;
using Frayte.WebApi.Utility;
using Frayte.Services.Utility;
using Elmah;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class LoginController : ApiController
    {

        //User login Table is removed now // Prakash will delete it later
        //[HttpPost]
        //public LoginDetail LoginUser(FrayteLogin userCrednetial)
        //{
        //    LoginDetail loginDetail = new LoginDetail();
        //    try
        //    {
        //        loginDetail = new LoginRepository().Login(userCrednetial);

        //        if (loginDetail != null)
        //        {
        //            if (string.IsNullOrEmpty(loginDetail.PhotoUrl))
        //            {
        //                loginDetail.PhotoUrl = AppSettings.ProfileImagePath + "avtar.jpg";
        //            }
        //            else
        //            {
        //                //Get full path of the resource
        //                loginDetail.PhotoUrl = AppSettings.WebApiPath + "UploadFiles/ProfilePhoto/" + loginDetail.PhotoUrl;
        //            }
        //            if (loginDetail.IsLastLogin)
        //            {
        //                loginDetail.frayteTabs = new LoginRepository().GetUserTabs(loginDetail.EmployeeId, loginDetail.EmployeeRoleId);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //    }
        //    return loginDetail;
        //}

        //User login Table is removed now // Prakash will delete it later
        //[HttpPost]
        //public RecoveryEmail RecoveryEmail(RecoveryEmail recoveryEmail)
        //{
        //    RecoveryEmail email = new RecoveryEmail();
        //    email.Email = "";
        //    var OperationZone = UtilityRepository.GetOperationZone();


        //    FrayteLogin frayteLogin = new LoginRepository().GetLoginByEmail(recoveryEmail);

        //    if (frayteLogin != null)
        //    {
        //        try
        //        {
        //            RecoverEmailModel model = new RecoverEmailModel();
        //            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
        //            model.Name = frayteLogin.Name;
        //            model.UserName = frayteLogin.UserName;
        //            model.Password = frayteLogin.Password;
        //            model.ImageHeader = "FrayteLogo";
        //            if (OperationZone.OperationZoneId == 1)
        //            {
        //                model.SalesEmail = "sales@frayte.com";
        //                model.PhoneNumber = "(+852) 2148 4880";
        //                model.SiteAddress = "www.FRAYTE.com";
        //                model.SiteLink = "Frayte HK";
        //                model.RecoveryLink = "http://app.frayte.com/#/forgotPassword";
        //            }
        //            if (OperationZone.OperationZoneId == 2)
        //            {
        //                model.SalesEmail = "sales@frayte.co.uk";
        //                model.PhoneNumber = "(+44) 01792 277295";
        //                model.SiteAddress = "www.FRAYTE.co.uk";
        //                model.SiteLink = "Frayte UK";
        //                model.RecoveryLink = "http://app.frayte.co.uk/#/forgotPassword";
        //            }

        //            string template1 = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/EmailRecovery.cshtml");
        //            var EmailBody = Engine.Razor.RunCompile(template1, "EmailRecovery1", null, model);
        //            var EmailSubject = "Request for Login Credentials Recovery for Frayte Management System";

        //            FrayteEmail.SendMail(recoveryEmail.Email, "", EmailSubject, EmailBody, "", logoImage);
        //            return recoveryEmail;
        //        }
        //        catch (Exception ex)
        //        {
        //            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //            return email;
        //        }
        //    }
        //    return email;
        //}

        //[HttpPost]
        //public IHttpActionResult ChangePassword(FrayteChangePassword changePasswordDetail)
        //{
        //    FrayteResult result = new LoginRepository().ChangePassword(changePasswordDetail);

        //    if (result != null && result.Status)
        //    {
        //        return Ok();
        //    } 
        //    else
        //    {
        //        return BadRequest();
        //    }
        //}

        [HttpGet]
        public IHttpActionResult GetUserTabs(int userId, int roleId, string moduleType)
        {
            var tabs = new LoginRepository().GetUserTabs(userId, roleId, moduleType);

            if (tabs != null)
            {
                return Ok(tabs);
            }
            else
            {
                return NotFound();
            }
        }

        //User login Table is removed now // Prakash will delete it later
        //[HttpPost]
        //public LoginDetail ChangeFirstPassword(FrayteChangeFirstPassword changePasswordDetail)
        //{
        //    LoginDetail loginDetail = new LoginRepository().ChangeFirstPassword(changePasswordDetail);

        //    if (loginDetail != null)
        //    {
        //        if (string.IsNullOrEmpty(loginDetail.PhotoUrl))
        //        {
        //            loginDetail.PhotoUrl = AppSettings.ProfileImagePath + "avtar.jpg";
        //        }
        //        else
        //        {
        //            //Get full path of the resource
        //            loginDetail.PhotoUrl = AppSettings.WebApiPath + "UploadFiles/ProfilePhoto/" + loginDetail.PhotoUrl;
        //        }
        //        if (loginDetail.IsLastLogin)
        //        {
        //            loginDetail.frayteTabs = new LoginRepository().GetUserTabs(loginDetail.EmployeeId, loginDetail.EmployeeRoleId);
        //        }
        //        return loginDetail;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
    }
}
