using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Frayte.WebApi.Models;
using Frayte.WebApi.Providers;
using Frayte.WebApi.Results;
using Frayte.Security.IdentityManager;
using Frayte.Security.IdentityModels;
using Frayte.Services.Models;
using Frayte.Services.Business;
using Frayte.Services.Utility;
using System.IO;
using RazorEngine;
using System.Net;
using Frayte.Services.DataAccess;

namespace Frayte.WebApi.Controllers
{
    public class AccountController : ApiController
    {
        #region -- Member Vars --

        private FrayteIdentityUserManager _userManager;
        private FrayteIdentityRoleManager _roleManager;
        private FrayteIdentitySignInManager _signInManager;

        #endregion

        #region -- Properties --

        public FrayteIdentitySignInManager SignInManager
        {
            get
            {
                return _signInManager ?? Request.GetOwinContext().Get<FrayteIdentitySignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public FrayteIdentityUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<FrayteIdentityUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public FrayteIdentityRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? Request.GetOwinContext().Get<FrayteIdentityRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return Request.GetOwinContext().Authentication;
            }
        }

        #endregion

        #region -- Constructors and Destructors --

        public AccountController()
        {
        }

        #endregion

        #region -- Register User --

        [Authorize(Roles = "Admin, Staff, Customer")]
        [HttpPost]
        public async Task<IHttpActionResult> RegisterUser(FrayteInternalUser model)
        {
            FrayteResult frayteResult = new FrayteResult();
            try
            {
                //Step 1: Save User Details
                FrayteIdentityUser user;
                if (model.UserId == 0)
                {
                    user = new FrayteIdentityUser();
                    user.UserName = model.Email;
                    user.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
                    user.ContactName = model.ContactName;
                    user.CompanyName = model.CompanyName;
                    user.TimezoneId = model.Timezone.TimezoneId;
                    user.FaxNumber = model.FaxNumber;
                    user.MobileNo = model.MobileNo;
                    user.PhoneNumber = model.TelephoneNo;
                    user.Position = model.Position;
                    user.ProfileImage = "staff.png";
                    user.TelephoneNo = model.TelephoneNo;
                    user.VATGST = model.VATGST;
                    user.WorkingStartTime = UtilityRepository.ConvertToUniversalTime(model.startTime, model.Timezone).TimeOfDay;
                    user.WorkingEndTime = UtilityRepository.ConvertToUniversalTime(model.EndTime, model.Timezone).TimeOfDay;
                    user.Skype = model.Skype;
                    user.WorkingWeekDayId = model.WorkingWeekDay != null ? model.WorkingWeekDay.WorkingWeekDayId : 0;
                    user.ShortName = model.ShortName;
                    user.Email = model.Email;
                    user.CreatedOn = DateTime.UtcNow;
                    user.CreatedBy = model.CreatedBy;
                    user.IsActive = true;

                    Random rnd = new Random();
                    string paasword = rnd.Next(10000000, 99999999).ToString();
                    var identityResult = await UserManager.CreateAsync(user, paasword);

                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("User Created")));

                    if (identityResult.Succeeded)
                    {
                        //Step 2: Save user role
                        var role = await RoleManager.FindByIdAsync(model.RoleId);
                        Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("Role Find")));
                        var result = UserManager.AddToRole(user.Id, role.Name);
                        Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("Role Added")));
                        if (result.Succeeded)
                        {
                            model.UserId = user.Id;

                            if (model.RoleId == (int)FrayteUserRole.CustomerStaff)
                            {
                                //Step 3: Save Associate Customer Detail
                                new FrayteUserRepository().SaveAssociateCustomer(model);
                            }

                            //Step 4: Save useradditional details
                            new FrayteUserRepository().SaveUserAdditional(model);
                            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("User Additional Added")));
                            model.UserAddress.UserId = user.Id;
                            //Step 5: Save useraddress details
                            new FrayteUserRepository().SaveUserAddress(model.UserAddress);
                            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("User Addresss Added")));
                            //Step 6 : Send credentials mail to user
                            if (model.RoleId != 2)
                            {
                                new FrayteUserRepository().SendEmail_NewUser(new FrayteLoginUserLogin { UserName = user.UserName, UserEmail = user.UserName, UserId = user.Id, Password = paasword, OperationStaffId = user.CreatedBy, RoleId = role.Id });
                            }
                            else if (model.RoleId == 2)
                            {
                                new FrayteUserRepository().SendWelcomeEmail_NewAgent(new FrayteLoginUserLogin { UserName = user.UserName, UserEmail = user.UserName, UserId = user.Id, Password = paasword, OperationStaffId = user.CreatedBy, RoleId = role.Id });
                            }
                            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("User mail Added")));
                            frayteResult.Status = true;
                        }
                    }
                }
                else
                {
                    user = await UserManager.FindByIdAsync(model.UserId);
                    if (user != null)
                    {
                        user.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
                        user.ContactName = model.ContactName;
                        user.CompanyName = model.CompanyName;
                        user.TimezoneId = model.Timezone.TimezoneId;
                        user.FaxNumber = model.FaxNumber;
                        user.MobileNo = model.MobileNo;
                        user.PhoneNumber = model.TelephoneNo;
                        user.Position = model.Position;
                        user.TelephoneNo = model.TelephoneNo;
                        user.VATGST = model.VATGST;
                        user.WorkingStartTime = UtilityRepository.ConvertToUniversalTime(model.startTime, model.Timezone).TimeOfDay;
                        user.WorkingEndTime = UtilityRepository.ConvertToUniversalTime(model.EndTime, model.Timezone).TimeOfDay;
                        user.Skype = model.Skype;
                        user.WorkingWeekDayId = model.WorkingWeekDay != null ? model.WorkingWeekDay.WorkingWeekDayId : 0;
                        user.ShortName = model.ShortName;
                        user.UpdatedOn = DateTime.UtcNow;
                        user.UpdatedBy = model.UpdatedBy;
                        user.IsActive = true;
                        var UpdateResult = await UserManager.UpdateAsync(user);

                        if (UpdateResult.Succeeded)
                        {
                            var role = await RoleManager.FindByIdAsync(model.RoleId);
                            if (!(await UserManager.IsInRoleAsync(user.Id, role.Name)))
                            {
                                var roles = await UserManager.GetRolesAsync(user.Id);
                                if (roles != null)
                                {
                                    foreach (var userRole in roles)
                                    {
                                        await UserManager.RemoveFromRoleAsync(user.Id, userRole);
                                    }
                                }
                                await UserManager.AddToRoleAsync(user.Id, role.Name);
                                frayteResult.Status = true;
                            }

                            if (model.RoleId == (int)FrayteUserRole.CustomerStaff)
                            {
                                //Step 3: Save Associate Customer Detail
                                new FrayteUserRepository().SaveAssociateCustomer(model);
                            }

                            //Step 4: Save useradditional details
                            new FrayteUserRepository().SaveUserAdditional(model);

                            model.UserAddress.UserId = user.Id;
                            //Step 5: Save useraddress details
                            new FrayteUserRepository().SaveUserAddress(model.UserAddress);
                            frayteResult.Status = true;
                        }
                        else
                        {
                            frayteResult.Status = false;
                        }
                    }
                }
                return Ok(frayteResult);
            }
            catch (Exception ex)
            {
                frayteResult.Status = false;
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest();
            }
        }

        #endregion

        #region -- Register Customer --

        [Authorize(Roles = "Admin, Staff, Customer")]
        [HttpPost]
        public async Task<IHttpActionResult> RegisterCustomer(FrayteCustomer model)
        {
            List<string> _change = new List<string>();
            // For Customer ammednment mail to Staff or Admin
            List<string> _amendment = new List<string>();
            bool isNewCustomer = model.UserId == 0;
            FrayteCustomer newCustomer = new FrayteCustomer();
            if (model.UserId > 0)
            {
                var customerDetail = new CustomerRepository().GetCustomerDetail(model.UserId);
                newCustomer.UserId = model.UserId;
                newCustomer.ContactName = customerDetail.ContactName;
                newCustomer.CompanyName = customerDetail.CompanyName;
                newCustomer.OperationUser = new FrayteCustomerAssociatedUser();
                if (customerDetail.OperationUser != null && customerDetail.OperationUser.UserId > 0)
                {
                    newCustomer.OperationUser.UserId = customerDetail.OperationUser.UserId;
                }
                FrayteCustomer newcustomer = new FrayteCustomer();
                _change = new CustomerRepository().UpdatedCustomerDetail(_amendment, newcustomer, customerDetail, model);
            }

            FrayteIdentityUser user;
            IdentityResult identityResult = new IdentityResult();
            if (model.UserId == 0)
            {
                //Step 1: Save User Detail 
                user = new FrayteIdentityUser();
                user.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
                user.CargoWiseId = model.CargoWiseId;
                user.CargoWiseBardCode = model.CargoWiseBardCode;
                user.WorkingWeekDayId = model.WorkingWeekDay.WorkingWeekDayId;
                user.CompanyName = model.CompanyName;
                user.ClientId = model.ClientId;
                user.IsClient = model.IsClient;
                user.CountryOfOperation = model.CountryOfOperation;
                user.ContactName = model.ContactName;

                user.UserEmail = model.UserEmail;
                // Customer can login in using it's unique Account No 
                Begin:
                Random rnd = new Random();
                var RanNo = rnd.Next(100, 999).ToString();
                var UsrDtl = new CustomerRepository().GetCustomerAccNo(RanNo);
                if (UsrDtl == "")
                {
                    user.UserName = RanNo + rnd.Next(100000, 999999).ToString();
                }
                else
                {
                    goto Begin;
                }

                //user.UserName = model.Email;
                model.AccountNumber = user.UserName;
                if (UtilityRepository.GetOperationZone().OperationZoneId == 1)
                {
                    if (model.CreatedByRoleId == (int)FrayteUserRole.Customer)
                    {
                        user.Email = user.UserName + "@bookshipment.com";
                    }
                    else
                    {
                        user.Email = user.UserName + "@frayte.com";
                    }
                }
                else
                {
                    if (model.CreatedByRoleId == (int)FrayteUserRole.Customer)
                    {
                        user.Email = user.UserName + "@bookshipment.com";
                    }
                    else
                    {
                        user.Email = user.UserName + "@frayte.co.uk";
                    }
                }

                user.TelephoneNo = model.TelephoneNo;
                user.MobileNo = model.MobileNo;
                user.TimezoneId = model.Timezone != null ? model.Timezone.TimezoneId : 0;
                user.WorkingStartTime = UtilityRepository.ConvertToUniversalTime(model.startTime, model.Timezone).TimeOfDay;
                user.WorkingEndTime = UtilityRepository.ConvertToUniversalTime(model.EndTime, model.Timezone).TimeOfDay;
                user.VATGST = model.VATGST;
                user.ShortName = model.ShortName;
                user.Position = model.Position;
                user.Skype = model.Skype;
                user.IsActive = true;
                user.CreatedOn = DateTime.UtcNow;
                user.CreatedBy = model.CreatedBy;

                string paasword = rnd.Next(10000000, 99999999).ToString();

                identityResult = await UserManager.CreateAsync(user, paasword);

                if (identityResult.Succeeded)
                {
                    model.UserId = user.Id;
                    //Step 2: Save user role
                    if (model.CreatedByRoleId == (int)FrayteUserRole.Customer)
                    {
                        var role = await RoleManager.FindByIdAsync((int)FrayteUserRole.UserCustomer);
                        var result = UserManager.AddToRole(user.Id, role.Name);

                        //Save user customer to UserCustomer table
                        new CustomerRepository().SaveUserCustomerDetail(model.CreatedBy, user.Id);
                        new FrayteUserRepository().SendEmail_NewUserCustomer(new FrayteLoginUserLogin { UserName = user.UserName, UserEmail = user.UserEmail, UserId = user.Id, Password = paasword, OperationStaffId = model.OperationUser.UserId, RoleId = role.Id }, model.ContactName, model.CreatedBy);
                    }
                    else
                    {
                        var role = await RoleManager.FindByIdAsync((int)FrayteUserRole.Customer);
                        var result = UserManager.AddToRole(user.Id, role.Name);

                        new FrayteUserRepository().SendEmail_NewUser(new FrayteLoginUserLogin { UserName = user.UserName, UserEmail = user.UserEmail, UserId = user.Id, Password = paasword, OperationStaffId = model.OperationUser.UserId, RoleId = role.Id });
                        if (model.CreatedByRoleId == (int)FrayteUserRole.Staff)
                        {
                            //new ShipmentEmailRepository().SendEmailToAdmin(model);
                        }
                    }

                    //Save Document to userdocument folder

                    if (model != null && model.UserId > 0)
                    {

                        if ((System.IO.File.Exists(AppSettings.UploadFolderPath + "/UserDocuments/TempUserDocument/" + model.UserDocumentDisplay)))
                        {
                            System.IO.Directory.CreateDirectory(AppSettings.UploadFolderPath + "/UserDocuments/" + model.UserId);
                            File.Copy(AppSettings.UploadFolderPath + "/UserDocuments/TempUserDocument/" + model.UserDocumentDisplay, AppSettings.UploadFolderPath + "/UserDocuments/" + model.UserId + "/" + model.UserDocument);
                        }
                        else
                        {

                        }
                    }
                }
            }
            else
            {
                user = await UserManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    user.UserEmail = model.UserEmail;

                    user.UserName = model.AccountNumber;
                    user.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
                    user.CargoWiseId = model.CargoWiseId;
                    user.CargoWiseBardCode = model.CargoWiseBardCode;
                    user.WorkingWeekDayId = model.WorkingWeekDay.WorkingWeekDayId;
                    user.CompanyName = model.CompanyName;
                    user.ClientId = model.ClientId;
                    user.IsClient = model.IsClient;
                    user.CountryOfOperation = model.CountryOfOperation;
                    user.ContactName = model.ContactName;
                    user.TelephoneNo = model.TelephoneNo;
                    user.MobileNo = model.MobileNo;
                    user.FaxNumber = model.FaxNumber;
                    user.TimezoneId = model.Timezone != null ? model.Timezone.TimezoneId : 0;
                    user.WorkingStartTime = UtilityRepository.ConvertToUniversalTime(model.startTime, model.Timezone).TimeOfDay;
                    user.WorkingEndTime = UtilityRepository.ConvertToUniversalTime(model.EndTime, model.Timezone).TimeOfDay;
                    user.VATGST = model.VATGST;
                    user.ShortName = model.ShortName;
                    user.Position = model.Position;
                    user.Skype = model.Skype;
                    user.UpdatedOn = DateTime.UtcNow;
                    user.UpdatedBy = model.CreatedBy;
                    identityResult = await UserManager.UpdateAsync(user);
                    if (identityResult.Succeeded)
                    {
                        var role = await RoleManager.FindByIdAsync(model.RoleId);

                        if (!(await UserManager.IsInRoleAsync(user.Id, role.Name)))
                        {
                            var roles = await UserManager.GetRolesAsync(user.Id);
                            if (roles != null)
                            {
                                foreach (var userRole in roles)
                                {
                                    await UserManager.RemoveFromRoleAsync(user.Id, userRole);
                                }
                            }
                            await UserManager.AddToRoleAsync(user.Id, role.Name);
                        }

                        if (newCustomer.UserId > 0)
                        {
                            if (_change.Count > 0)
                            {
                                //To Do : Send Ammend mail to Operation Staff or Customer
                                if (model.CreatedByRoleId == (int)FrayteUserRole.Customer)
                                {
                                    new ShipmentEmailRepository().SendUserCustomerAmmendMail(newCustomer, _change, model.CreatedByRoleId, model.CreatedBy);
                                }
                                else
                                {
                                    new ShipmentEmailRepository().SendAmmendMailToStaff(newCustomer, _change, model.CreatedByRoleId, (int)model.UpdatedBy);
                                }
                            }
                        }
                        if (model != null && model.UserId > 0)
                        {
                            if (System.IO.File.Exists(AppSettings.UploadFolderPath + "/UserDocuments/TempUserDocument/" + model.UserDocumentDisplay))
                            {
                                if (System.IO.Directory.Exists(AppSettings.UploadFolderPath + "/UserDocuments/" + model.UserId) && System.IO.File.Exists(AppSettings.UploadFolderPath + "/UserDocuments/" + model.UserId + "/" + model.UserDocument))
                                {

                                }
                                else
                                {
                                    if (System.IO.Directory.Exists(AppSettings.UploadFolderPath + "/UserDocuments/" + model.UserId))
                                    {
                                        string[] filePaths = Directory.GetFiles(AppSettings.UploadFolderPath + "/UserDocuments/" + model.UserId);
                                        foreach (string filePath in filePaths)
                                        {
                                            File.Delete(filePath);
                                        }
                                        File.Copy(AppSettings.UploadFolderPath + "/UserDocuments/TempUserDocument/" + model.UserDocumentDisplay, AppSettings.UploadFolderPath + "/UserDocuments/" + model.UserId + "/" + model.UserDocument);
                                        File.Delete(AppSettings.UploadFolderPath + "/UserDocuments/TempUserDocument/" + model.UserDocumentDisplay);
                                    }
                                    else
                                    {
                                        System.IO.Directory.CreateDirectory(AppSettings.UploadFolderPath + "/UserDocuments/" + model.UserId);
                                        File.Copy(AppSettings.UploadFolderPath + "/UserDocuments/TempUserDocument/" + model.UserDocumentDisplay, AppSettings.UploadFolderPath + "/UserDocuments/" + model.UserId + "/" + model.UserDocument);
                                        File.Delete(AppSettings.UploadFolderPath + "/UserDocuments/TempUserDocument/" + model.UserDocumentDisplay);
                                    }
                                }
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }

            if (identityResult.Succeeded)
            {
                //Step 3: Save user customer detail
                new CustomerRepository().SaveCustomerAdditional(model);

                //Step 4: Save Customer Address information
                model.UserAddress.AddressTypeId = (int)FrayteAddressType.MainAddress;
                model.UserAddress.UserId = user.Id;
                new FrayteUserRepository().SaveUserAddress(model.UserAddress);

                if (model.CreatedByRoleId == (int)FrayteUserRole.Staff || model.CreatedByRoleId == (int)FrayteUserRole.Admin)
                {
                    //Step 5: Save Customer other address information
                    if (model.OtherAddresses != null && model.OtherAddresses.Count > 0)
                    {
                        foreach (FrayteAddress address in model.OtherAddresses)
                        {
                            address.AddressTypeId = (int)FrayteAddressType.OtherAddress;
                            address.UserId = model.UserId;
                            new FrayteUserRepository().SaveUserAddress(address);
                        }
                    }

                    //Step 7: Save/Edit Customer POD Setting
                    new CustomerRepository().SaveCustomerSetting(model.CustomerPODSetting, model.UserId, model.Timezone);
                }
                return Ok();
            }
            return BadRequest();
        }

        #endregion

        #region -- User Login --

        #region -- Login User --

        [Authorize]
        [HttpPost]
        public async Task<IHttpActionResult> LoginUserDetail(FrayteLogin userCrednetial)
        {
            LoginDetail loginDetail = new LoginDetail();
            try
            {
                // get user detail using identity library
                var user = await UserManager.FindByNameAsync(userCrednetial.UserName);
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("user")));
                if (user != null)
                {
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("user found")));
                    var roleId = new FrayteUserRepository().GetUserRole(user.Id);
                    if (roleId == (int)FrayteUserRole.Admin || roleId == (int)FrayteUserRole.MasterAdmin || roleId == (int)FrayteUserRole.Staff || user.OperationZoneId == UtilityRepository.GetOperationZone().OperationZoneId)
                    {
                        loginDetail = new FrayteUserRepository().LoginUserDetail(user.Id);
                        if (loginDetail != null)
                        {
                            if (string.IsNullOrEmpty(loginDetail.PhotoUrl))
                                loginDetail.PhotoUrl = AppSettings.ProfileImagePath + "avtar.jpg";
                            else
                                loginDetail.PhotoUrl = AppSettings.WebApiPath + "UploadFiles/ProfilePhoto/" + loginDetail.PhotoUrl;

                            if (loginDetail.IsLastLogin)
                            {
                                //   loginDetail.frayteTabs = new LoginRepository().GetUserTabs(loginDetail.EmployeeId, loginDetail.EmployeeRoleId);
                            }
                        }
                    }
                }
                else
                {
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("user not found")));
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            return Ok(loginDetail);
        }

        #endregion

        #region -- Change Password --

        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> ChangeFirstPassword(FrayteChangeFirstPassword changePasswordDetail)
        {
            try
            {
                var user = await UserManager.FindByIdAsync(changePasswordDetail.UserId);
                var currentUser = await UserManager.FindAsync(user.UserName, changePasswordDetail.CurrentPassword);
                if (user != null && currentUser != null && user.OperationZoneId == UtilityRepository.GetOperationZone().OperationZoneId)
                {
                    if ((await UserManager.RemovePasswordAsync(user.Id)).Succeeded)
                    {
                        if ((await UserManager.AddPasswordAsync(user.Id, changePasswordDetail.NewPassword)).Succeeded)
                        {
                            user.LastLoginDate = DateTime.UtcNow;
                            user.LastPasswordChangeDate = DateTime.UtcNow;
                            if ((await UserManager.UpdateAsync(user)).Succeeded)
                            {
                                //LoginDetail loginDetail = new LoginRepository().ChangeFirstPassword(changePasswordDetail);
                                LoginDetail loginDetail = new FrayteUserRepository().LoginUserDetail(user.Id);
                                if (loginDetail != null)
                                {
                                    if (string.IsNullOrEmpty(loginDetail.PhotoUrl))
                                    {
                                        loginDetail.PhotoUrl = AppSettings.ProfileImagePath + "avtar.jpg";
                                    }
                                    else
                                    {
                                        //Get full path of the resource
                                        loginDetail.PhotoUrl = AppSettings.WebApiPath + "UploadFiles/ProfilePhoto/" + loginDetail.PhotoUrl;
                                    }
                                    if (loginDetail.IsLastLogin)
                                    {
                                        //  loginDetail.frayteTabs = new LoginRepository().GetUserTabs(loginDetail.EmployeeId, loginDetail.EmployeeRoleId , "");
                                    }
                                    return Ok(loginDetail);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

            }

            return BadRequest();
        }

        [Authorize]
        [HttpPost]
        public async Task<IHttpActionResult> ChangePassword(FrayteChangePassword changePasswordDetail)
        {
            try
            {
                var user = await UserManager.FindAsync(changePasswordDetail.UserName, changePasswordDetail.CurrentPassword);
                if (user != null)
                {
                    if ((await UserManager.RemovePasswordAsync(user.Id)).Succeeded)
                    {
                        if ((await UserManager.AddPasswordAsync(user.Id, changePasswordDetail.NewPassword)).Succeeded)
                        {
                            user.LastLoginDate = DateTime.UtcNow;
                            user.LastPasswordChangeDate = DateTime.UtcNow;
                            if ((await UserManager.UpdateAsync(user)).Succeeded)
                            {
                                return Ok();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return BadRequest();
        }

        #endregion

        #region -- Forget Password --

        /// <summary>
        /// RecoverPassword will set a new paasword for user if token is valid for user
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> RecoverPassword(RecoverPassword recoverPassword)
        {
            try
            {
                if (recoverPassword.Token == null || recoverPassword.UserId == 0)
                {
                    return BadRequest("Invalid token.");
                }
                var myuser = await UserManager.FindByIdAsync(recoverPassword.UserId);

                var userRole = new FrayteUserRepository().GetUserRole(myuser.Id);

                if (myuser != null)
                {
                    if (userRole != (int)FrayteUserRole.Staff && userRole != (int)FrayteUserRole.Admin)
                    {
                        if (myuser.OperationZoneId == UtilityRepository.GetOperationZone().OperationZoneId)
                        {
                            if (await UserManager.UserTokenProvider.ValidateAsync("ResetPassword", WebUtility.UrlDecode(recoverPassword.Token), UserManager, myuser))
                            {
                                var result = await UserManager.ResetPasswordAsync(recoverPassword.UserId, WebUtility.UrlDecode(recoverPassword.Token), recoverPassword.NewPassword);
                                if (result.Succeeded)
                                {
                                    return Ok(userRole);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (await UserManager.UserTokenProvider.ValidateAsync("ResetPassword", WebUtility.UrlDecode(recoverPassword.Token), UserManager, myuser))
                        {
                            var result = await UserManager.ResetPasswordAsync(recoverPassword.UserId, WebUtility.UrlDecode(recoverPassword.Token), recoverPassword.NewPassword);
                            if (result.Succeeded)
                            {
                                return Ok(userRole);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return BadRequest("Invalid token.");
        }

        /// <summary>
        /// ForgetPassword will send mail to user with a secure token 
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> ForgetPassword(RecoveryEmail model)
        {
            var users = new FrayteUserRepository().getUserByEmail(model.Email);
            if (users.Count > 1)
            {
                return BadRequest("Enter_Account_Number");
            }
            else if (users.Count == 1)
            {
                int RoleId = new FrayteUserRepository().GetUserRole(users[0].UserId);
                if (RoleId != (int)FrayteUserRole.Staff && RoleId != (int)FrayteUserRole.Admin)
                {
                    if (UtilityRepository.GetOperationZone().OperationZoneId == users[0].OperationZoneId)
                    {
                        FrayteResult result = new ShipmentEmailRepository().sendForgetPasswordEmail(users[0].UserId, WebUtility.UrlEncode(await UserManager.GeneratePasswordResetTokenAsync(users[0].UserId)));
                        return Ok(new
                        {
                            Status = result.Status,
                            Email = users[0].UserEmail
                        });
                    }
                    else
                    {
                        return BadRequest("Invalid_OperationZone");
                    }
                }
                else
                {
                    FrayteResult result = new ShipmentEmailRepository().sendForgetPasswordEmail(users[0].UserId, WebUtility.UrlEncode(await UserManager.GeneratePasswordResetTokenAsync(users[0].UserId)));
                    return Ok(new
                    {
                        Status = result.Status,
                        Email = users[0].Email
                    });

                }
            }
            else
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    int RoleId = new FrayteUserRepository().GetUserRole(user.Id);
                    if (RoleId != (int)FrayteUserRole.Staff && RoleId != (int)FrayteUserRole.Admin)
                    {
                        if (UtilityRepository.GetOperationZone().OperationZoneId == user.OperationZoneId)
                        {
                            FrayteResult result = new ShipmentEmailRepository().sendForgetPasswordEmail(user.Id, WebUtility.UrlEncode(await UserManager.GeneratePasswordResetTokenAsync(user.Id)));
                            return Ok(new
                            {
                                Status = result.Status,
                                Email = user.UserEmail
                            });
                        }
                        else
                        {
                            return BadRequest("InvalidOperationZone" + "-" + UtilityRepository.GetOperationZone());
                        }
                    }
                    else
                    {
                        FrayteResult result = new ShipmentEmailRepository().sendForgetPasswordEmail(user.Id, WebUtility.UrlEncode(await UserManager.GeneratePasswordResetTokenAsync(user.Id)));
                        return Ok(new
                        {
                            Status = result.Status,
                            Email = user.Email
                        });
                    }
                }
                return BadRequest("User_Not_Found");
            }
        }

        #endregion

        #endregion

        #region -- Profile Photo --

        [Authorize]
        [HttpPost]
        public async Task<HttpResponseMessage> UploadProfilePhoto()
        {
            HttpResponseMessage result = null;
            try
            {
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    HttpFileCollection files = httpRequest.Files;
                    var userId = httpRequest.Form["UserId"];
                    string PhotoUrl = "";
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFile file = files[i];

                        string fname = "";
                        string fileExtension = Path.GetExtension(file.FileName);
                        Random random = new Random();
                        string profileImage = userId + "_" + random.Next(999, 999999999) + fileExtension;
                        fname = HttpContext.Current.Server.MapPath("~/UploadFiles/ProfilePhoto/") + profileImage;

                        file.SaveAs(fname);
                        PhotoUrl = AppSettings.WebApiPath + "UploadFiles/ProfilePhoto/" + profileImage;

                        //Update image in database.
                        FrayteIdentityUser user = await UserManager.FindByIdAsync(int.Parse(userId));
                        user.ProfileImage = profileImage;

                        if ((await UserManager.UpdateAsync(user)).Succeeded)
                        {
                            result = Request.CreateResponse(HttpStatusCode.Created, PhotoUrl);
                        }
                        else
                        {
                            result = Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            return result;
        }

        #endregion

        #region -- Is Email Registered --

        [Authorize]
        [HttpGet]
        public async Task<IHttpActionResult> IsEmailExist(string email, string userType)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(userType))
                {
                    FrayteIdentityUser user = await UserManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        result.Status = false;
                    }
                    else
                    {
                        if (userType == "Customer")
                        {
                            if (await UserManager.IsInRoleAsync(user.Id, "Customer"))
                            {
                                result.Status = false;
                            }
                            else
                            {
                                result.Status = true;
                            }
                        }
                        else if (userType == "CustomerStaff")
                        {
                            if (await UserManager.IsInRoleAsync(user.Id, "CustomerStaff"))
                            {
                                result.Status = false;
                            }
                            else
                            {
                                result.Status = true;
                            }
                        }
                        else
                        {
                            result.Status = true;
                        }

                    }
                }
                else
                {
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                result.Status = false;
            }
            return Ok(result);
        }

        #endregion
    }

    ///
    /// //  Don't remove commentd code below for now. Prakash will remove it later when the login functionality will be stable
    ///

    //[Authorize]
    //[RoutePrefix("api/Account")]
    //public class AccountController : ApiController
    //{
    //    private const string LocalLoginProvider = "Local";
    //    private FrayteIdentityUserManager _userManager;

    //    public AccountController()
    //    {
    //    }

    //    public AccountController(FrayteIdentityUserManager userManager,
    //        ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
    //    {
    //        UserManager = userManager;
    //        AccessTokenFormat = accessTokenFormat;
    //    }

    //    public FrayteIdentityUserManager UserManager
    //    {
    //        get
    //        {
    //            return _userManager ?? Request.GetOwinContext().GetUserManager<FrayteIdentityUserManager>();
    //        }
    //        private set
    //        {
    //            _userManager = value;
    //        }
    //    }

    //    public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

    //    // GET api/Account/UserInfo
    //    [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
    //    [Route("UserInfo")]
    //    public UserInfoViewModel GetUserInfo()
    //    {
    //        ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

    //        return new UserInfoViewModel
    //        {
    //            Email = User.Identity.GetUserName(),
    //            HasRegistered = externalLogin == null,
    //            LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
    //        };
    //    }

    //    // POST api/Account/Logout
    //    [Route("Logout")]
    //    public IHttpActionResult Logout()
    //    {
    //        Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
    //        return Ok();
    //    }

    //    // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
    //    [Route("ManageInfo")]
    //    public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
    //    {
    //        IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

    //        if (user == null)
    //        {
    //            return null;
    //        }

    //        List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

    //        foreach (IdentityUserLogin linkedAccount in user.Logins)
    //        {
    //            logins.Add(new UserLoginInfoViewModel
    //            {
    //                LoginProvider = linkedAccount.LoginProvider,
    //                ProviderKey = linkedAccount.ProviderKey
    //            });
    //        }

    //        if (user.PasswordHash != null)
    //        {
    //            logins.Add(new UserLoginInfoViewModel
    //            {
    //                LoginProvider = LocalLoginProvider,
    //                ProviderKey = user.UserName,
    //            });
    //        }

    //        return new ManageInfoViewModel
    //        {
    //            LocalLoginProvider = LocalLoginProvider,
    //            Email = user.UserName,
    //            Logins = logins,
    //            ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
    //        };
    //    }

    //    // POST api/Account/ChangePassword
    //    [Route("ChangePassword")]
    //    public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return BadRequest(ModelState);
    //        }

    //        IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
    //            model.NewPassword);

    //        if (!result.Succeeded)
    //        {
    //            return GetErrorResult(result);
    //        }

    //        return Ok();
    //    }

    //    // POST api/Account/SetPassword
    //    [Route("SetPassword")]
    //    public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return BadRequest(ModelState);
    //        }

    //        IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

    //        if (!result.Succeeded)
    //        {
    //            return GetErrorResult(result);
    //        }

    //        return Ok();
    //    }

    //    // POST api/Account/AddExternalLogin
    //    [Route("AddExternalLogin")]
    //    public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return BadRequest(ModelState);
    //        }

    //        Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

    //        AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

    //        if (ticket == null || ticket.Identity == null || (ticket.Properties != null
    //            && ticket.Properties.ExpiresUtc.HasValue
    //            && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
    //        {
    //            return BadRequest("External login failure.");
    //        }

    //        ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

    //        if (externalData == null)
    //        {
    //            return BadRequest("The external login is already associated with an account.");
    //        }

    //        IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
    //            new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

    //        if (!result.Succeeded)
    //        {
    //            return GetErrorResult(result);
    //        }

    //        return Ok();
    //    }

    //    // POST api/Account/RemoveLogin
    //    [Route("RemoveLogin")]
    //    public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return BadRequest(ModelState);
    //        }

    //        IdentityResult result;

    //        if (model.LoginProvider == LocalLoginProvider)
    //        {
    //            result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
    //        }
    //        else
    //        {
    //            result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
    //                new UserLoginInfo(model.LoginProvider, model.ProviderKey));
    //        }

    //        if (!result.Succeeded)
    //        {
    //            return GetErrorResult(result);
    //        }

    //        return Ok();
    //    }

    //    // GET api/Account/ExternalLogin
    //    [OverrideAuthentication]
    //    [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
    //    [AllowAnonymous]
    //    [Route("ExternalLogin", Name = "ExternalLogin")]
    //    public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
    //    {
    //        if (error != null)
    //        {
    //            return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
    //        }

    //        if (!User.Identity.IsAuthenticated)
    //        {
    //            return new ChallengeResult(provider, this);
    //        }

    //        ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

    //        if (externalLogin == null)
    //        {
    //            return InternalServerError();
    //        }

    //        if (externalLogin.LoginProvider != provider)
    //        {
    //            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
    //            return new ChallengeResult(provider, this);
    //        }

    //        ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
    //            externalLogin.ProviderKey));

    //        bool hasRegistered = user != null;

    //        if (hasRegistered)
    //        {
    //            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

    //             ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
    //                OAuthDefaults.AuthenticationType);
    //            ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
    //                CookieAuthenticationDefaults.AuthenticationType);

    //            AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
    //            Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
    //        }
    //        else
    //        {
    //            IEnumerable<Claim> claims = externalLogin.GetClaims();
    //            ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
    //            Authentication.SignIn(identity);
    //        }

    //        return Ok();
    //    }

    //    // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
    //    [AllowAnonymous]
    //    [Route("ExternalLogins")]
    //    public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
    //    {
    //        IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
    //        List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

    //        string state;

    //        if (generateState)
    //        {
    //            const int strengthInBits = 256;
    //            state = RandomOAuthStateGenerator.Generate(strengthInBits);
    //        }
    //        else
    //        {
    //            state = null;
    //        }

    //        foreach (AuthenticationDescription description in descriptions)
    //        {
    //            ExternalLoginViewModel login = new ExternalLoginViewModel
    //            {
    //                Name = description.Caption,
    //                Url = Url.Route("ExternalLogin", new
    //                {
    //                    provider = description.AuthenticationType,
    //                    response_type = "token",
    //                    client_id = Startup.PublicClientId,
    //                    redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
    //                    state = state
    //                }),
    //                State = state
    //            };
    //            logins.Add(login);
    //        }

    //        return logins;
    //    }

    //    // POST api/Account/Register
    //    [AllowAnonymous]
    //    [Route("Register")]
    //    public async Task<IHttpActionResult> Register(RegisterBindingModel model)
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return BadRequest(ModelState);
    //        }

    //        var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

    //        IdentityResult result = await UserManager.CreateAsync(user, model.Password);

    //        if (!result.Succeeded)
    //        {
    //            return GetErrorResult(result);
    //        }

    //        return Ok();
    //    }

    //    // POST api/Account/RegisterExternal
    //    [OverrideAuthentication]
    //    [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
    //    [Route("RegisterExternal")]
    //    public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return BadRequest(ModelState);
    //        }

    //        var info = await Authentication.GetExternalLoginInfoAsync();
    //        if (info == null)
    //        {
    //            return InternalServerError();
    //        }

    //        var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

    //        IdentityResult result = await UserManager.CreateAsync(user);
    //        if (!result.Succeeded)
    //        {
    //            return GetErrorResult(result);
    //        }

    //        result = await UserManager.AddLoginAsync(user.Id, info.Login);
    //        if (!result.Succeeded)
    //        {
    //            return GetErrorResult(result); 
    //        }
    //        return Ok();
    //    }

    //    protected override void Dispose(bool disposing)
    //    {
    //        if (disposing && _userManager != null)
    //        {
    //            _userManager.Dispose();
    //            _userManager = null;
    //        }

    //        base.Dispose(disposing);
    //    }

    //    #region Helpers

    //    private IAuthenticationManager Authentication
    //    {
    //        get { return Request.GetOwinContext().Authentication; }
    //    }

    //    private IHttpActionResult GetErrorResult(IdentityResult result)
    //    {
    //        if (result == null)
    //        {
    //            return InternalServerError();
    //        }

    //        if (!result.Succeeded)
    //        {
    //            if (result.Errors != null)
    //            {
    //                foreach (string error in result.Errors)
    //                {
    //                    ModelState.AddModelError("", error);
    //                }
    //            }

    //            if (ModelState.IsValid)
    //            {
    //                // No ModelState errors are available to send, so just return an empty BadRequest.
    //                return BadRequest();
    //            }

    //            return BadRequest(ModelState);
    //        }

    //        return null;
    //    }

    //    private class ExternalLoginData
    //    {
    //        public string LoginProvider { get; set; }
    //        public string ProviderKey { get; set; }
    //        public string UserName { get; set; }

    //        public IList<Claim> GetClaims()
    //        {
    //            IList<Claim> claims = new List<Claim>();
    //            claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

    //            if (UserName != null)
    //            {
    //                claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
    //            }

    //            return claims;
    //        }

    //        public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
    //        {
    //            if (identity == null)
    //            {
    //                return null;
    //            }

    //            Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

    //            if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
    //                || String.IsNullOrEmpty(providerKeyClaim.Value))
    //            {
    //                return null;
    //            }

    //            if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
    //            {
    //                return null;
    //            }

    //            return new ExternalLoginData
    //            {
    //                LoginProvider = providerKeyClaim.Issuer,
    //                ProviderKey = providerKeyClaim.Value,
    //                UserName = identity.FindFirstValue(ClaimTypes.Name)
    //            };
    //        }
    //    }

    //    private static class RandomOAuthStateGenerator
    //    {
    //        private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

    //        public static string Generate(int strengthInBits)
    //        {
    //            const int bitsPerByte = 8;

    //            if (strengthInBits % bitsPerByte != 0)
    //            {
    //                throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
    //            }

    //            int strengthInBytes = strengthInBits / bitsPerByte;

    //            byte[] data = new byte[strengthInBytes];
    //            _random.GetBytes(data);
    //            return HttpServerUtility.UrlTokenEncode(data);
    //        }
    //    }

    //    #endregion
    //}
}
