using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Frayte.Services.Business
{
    public class FrayteUserRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        #region -- User Detail --

        public LoginDetail LoginUserDetail(int userId)
        {
            try
            {
                var userResult = (from UL in dbContext.Users
                                  join UR in dbContext.UserRoles on UL.UserId equals UR.UserId
                                  where UL.UserId == userId
                                  select new
                                  {
                                      LastLogIn = UL.LastLoginDate,
                                      RoleId = UR.RoleId,
                                      UserId = UL.UserId
                                  }).FirstOrDefault();


                if (userResult != null)
                {
                    if (userResult.RoleId == (int)FrayteUserRole.Customer)
                    {
                        var specialCustomer = dbContext.UserCustomers.Where(p => p.UserId == userId).FirstOrDefault();
                        if (specialCustomer != null)
                        {
                            var OperationZone = UtilityRepository.GetOperationZone();
                            var result = (from u in dbContext.Users
                                          join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                          join cd in dbContext.CustomerCompanyDetails on u.UserId equals cd.UserId
                                          join ur in dbContext.UserRoles on u.UserId equals ur.UserId
                                          where
                                             u.UserId == userId &&
                                             u.IsActive == true
                                          select new LoginDetail()
                                          {
                                              EmployeeId = u.UserId,
                                              UserType = ua.UserType,
                                              EmployeeCustomerId = u.UserId,
                                              EmployeeCustomerRoleId = ur.RoleId,
                                              EmployeeCompanyLogo = cd.LogoFileName,
                                              EmployeeCompanyName = u.CompanyName,
                                              UserName = u.UserName,
                                              EmployeeName = u.ContactName,
                                              EmployeeMail = u.UserEmail,
                                              EmployeeRoleId = ur.RoleId,
                                              SessionId = "123456789",
                                              IsRateShow = ua.IsAllowRate.HasValue ? ua.IsAllowRate.Value : false,
                                              IsLastLogin = userResult.LastLogIn.HasValue ? true : false,
                                              ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                              CustomerCurrency = ua.CreditLimitCurrencyCode,
                                              PhotoUrl = u.ProfileImage,
                                              OperationZoneId = OperationZone.OperationZoneId,
                                              OperationZoneName = OperationZone.OperationZoneName,
                                              UserOperationZoneId = u.OperationZoneId,
                                              EmployeeCustomerDetail = new EmployeeCustomerLogInDetail
                                              {
                                                  EmployeeCustomerId = u.UserId,
                                                  EmployeeCustomerRoleId = ur.RoleId,
                                                  EmployeeCustomerCompany = u.CompanyName,
                                                  EmployeeCustomerCompanyLogo = cd.LogoFileName,
                                                  EmployeeCustomerTrackingEmail = cd.TrackingEmail,
                                                  CustomerService = cd.CustomerService
                                              }
                                          }).FirstOrDefault();

                            if (OperationZone.OperationZoneId == result.UserOperationZoneId)
                            {
                                return result;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            var OperationZone = UtilityRepository.GetOperationZone();
                            var result = (from u in dbContext.Users
                                          join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                          join ur in dbContext.UserRoles on u.UserId equals ur.UserId
                                          where
                                             u.UserId == userId &&
                                             u.IsActive == true
                                          select new LoginDetail()
                                          {
                                              EmployeeId = u.UserId,
                                              UserName = u.UserName,
                                              UserType = ua.UserType,
                                              EmployeeName = u.ContactName,
                                              EmployeeMail = u.UserEmail,
                                              EmployeeRoleId = ur.RoleId,
                                              SessionId = "123456789",
                                              IsRateShow = ua.IsAllowRate.HasValue ? ua.IsAllowRate.Value : false,
                                              IsLastLogin = userResult.LastLogIn.HasValue ? true : false,
                                              ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                              CustomerCurrency = ua.CreditLimitCurrencyCode,
                                              PhotoUrl = u.ProfileImage,
                                              OperationZoneId = OperationZone.OperationZoneId,
                                              OperationZoneName = OperationZone.OperationZoneName,
                                              UserOperationZoneId = u.OperationZoneId,
                                              EmployeeCustomerDetail = new EmployeeCustomerLogInDetail
                                              {
                                                  EmployeeCustomerId = 0,
                                                  EmployeeCustomerRoleId = 0,
                                                  EmployeeCustomerCompany = "Frayte Logistics LTD",
                                                  EmployeeCustomerCompanyLogo = "FrayteLogo1.png",
                                                  EmployeeCustomerTrackingEmail = OperationZone.OperationZoneId == 1 ? "tracking@frayte.com" : "tracking@frayte.co.uk",
                                                  CustomerService = "FRAYTE Customer Service Team"
                                              }
                                          }).FirstOrDefault();

                            if (OperationZone.OperationZoneId == result.UserOperationZoneId)
                            {
                                return result;
                            }
                            else
                            {
                                return null;
                            }
                        }

                    }
                    else if (userResult.RoleId == (int)FrayteUserRole.UserCustomer)
                    {
                        var OperationZone = UtilityRepository.GetOperationZone();
                        var result = (from u in dbContext.Users
                                      join uc in dbContext.UserCustomers on u.UserId equals uc.CustomerId
                                      join ucu in dbContext.Users on uc.UserId equals ucu.UserId
                                      join ccd in dbContext.CustomerCompanyDetails on ucu.UserId equals ccd.UserId
                                      join ucur in dbContext.UserRoles on uc.UserId equals ucur.UserId
                                      join ur in dbContext.UserRoles on u.UserId equals ur.UserId
                                      where
                                        u.UserId == userId &&
                                        u.IsActive == true
                                      select new LoginDetail()
                                      {
                                          EmployeeId = u.UserId,
                                          UserName = u.UserName,
                                          UserType = "",
                                          EmployeeName = u.ContactName,
                                          EmployeeMail = u.UserEmail,
                                          EmployeeRoleId = ur.RoleId,
                                          EmployeeCustomerId = ccd.UserId,
                                          EmployeeCustomerRoleId = ucur.RoleId,
                                          SessionId = "123456789",
                                          IsRateShow = false,
                                          IsLastLogin = userResult.LastLogIn.HasValue ? true : false,
                                          PhotoUrl = u.ProfileImage,
                                          ValidDays = 0,
                                          CustomerCurrency = "",
                                          OperationZoneId = OperationZone.OperationZoneId,
                                          OperationZoneName = OperationZone.OperationZoneName,
                                          EmployeeCustomerDetail = new EmployeeCustomerLogInDetail
                                          {
                                              EmployeeCustomerId = ccd.UserId,
                                              EmployeeCustomerRoleId = ucur.RoleId,
                                              EmployeeCustomerCompany = ucu.CompanyName,
                                              EmployeeCustomerCompanyLogo = ccd.LogoFileName,
                                              EmployeeCustomerTrackingEmail = ccd.TrackingEmail,
                                              CustomerService = ccd.CustomerService
                                          }
                                      }).FirstOrDefault();
                        return result;
                    }
                    else
                    {
                        var OperationZone = UtilityRepository.GetOperationZone();
                        var result = (from u in dbContext.Users
                                      join ur in dbContext.UserRoles on u.UserId equals ur.UserId
                                      where
                                           u.UserId == userId &&
                                           u.IsActive == true
                                      select new LoginDetail()
                                      {
                                          EmployeeId = u.UserId,
                                          UserName = u.UserName,
                                          EmployeeName = u.ContactName,
                                          UserType = "",
                                          EmployeeMail = u.Email,
                                          EmployeeRoleId = ur.RoleId,
                                          SessionId = "123456789",
                                          IsRateShow = true,
                                          IsLastLogin = userResult.LastLogIn.HasValue ? true : false,
                                          PhotoUrl = u.ProfileImage,
                                          ValidDays = 0,
                                          CustomerCurrency = "",
                                          OperationZoneId = OperationZone.OperationZoneId,
                                          OperationZoneName = OperationZone.OperationZoneName,
                                          EmployeeCustomerDetail = new EmployeeCustomerLogInDetail
                                          {
                                              EmployeeCustomerId = 0,
                                              EmployeeCustomerRoleId = 0,
                                              EmployeeCustomerCompany = "Frayte Logistics LTD",
                                              EmployeeCustomerCompanyLogo = "FrayteLogo1.png",
                                              EmployeeCustomerTrackingEmail = OperationZone.OperationZoneId == 1 ? "tracking@frayte.com" : "tracking@frayte.co.uk",
                                              CustomerService = "FRAYTE Customer Service Team"
                                          }
                                      }).FirstOrDefault();

                        return result;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return null;
        }

        public MobileInternalUserConfiguration GetMobileConfiguration(int userId)
        {
            MobileInternalUserConfiguration mobileUserConfiguration = new MobileInternalUserConfiguration();

            mobileUserConfiguration.UserId = userId;

            var userAddional = dbContext.UserAdditionals.Where(p => p.UserId == userId).FirstOrDefault();
            if (userAddional != null)
            {
                var mobileUserDetail = dbContext.MobileUserConfigurations.Where(p => p.UserId == userId).ToList();
                var collection = (from r in dbContext.MasterTrackings
                                  join td in dbContext.MasterTrackingDetails on r.MasterTrackingId equals td.MasterTrackingId
                                  select new
                                  {
                                      MasterTrackingId = r.MasterTrackingId,
                                      ModuleType = r.ModuleType,
                                      EventName = r.EventKey,
                                      EventKey = r.EventKey,
                                      EventKeyDisplay = r.EventKeyDisplay,
                                      IsDefault = r.IsDefault,
                                      IsOnlyForTracking = r.IsOnlyForTracking,
                                      IsOnlyMAWBTracking = r.IsOnlyMAWBTracking,

                                      MasterTrackingDetailId = td.MasterTrackingDetailId,
                                      SubEventKey = td.EventKey,
                                      SubEventDisplay = td.EventDisplay,
                                      IsExternal = td.IsExternal,
                                      SubIsDefault = td.IsDefault,
                                      SubEventMessage = td.EventMessage
                                  }
                                  ).ToList();

                if (userAddional.IsExpressSolutions.HasValue && userAddional.IsExpressSolutions.Value)
                {
                    mobileUserConfiguration.ExpressConfiguration = collection.Where(p => p.ModuleType == FrayteShipmentServiceType.Express).GroupBy(p => p.MasterTrackingId)
                                                                      .Select(group => new MasterTrackingModel
                                                                      {
                                                                          MasterTrackingId = group.FirstOrDefault().MasterTrackingId,
                                                                          EventKey = group.FirstOrDefault().EventKey,
                                                                          EventDisplay = group.FirstOrDefault().EventKeyDisplay,
                                                                          ModuleType = group.FirstOrDefault().ModuleType,
                                                                          TrackingDetail = group.Select(p => new MasterTrackingDetailModel
                                                                          {
                                                                              EventDisplay = p.SubEventDisplay,
                                                                              EventKey = p.EventKey,
                                                                              IsDefault = p.SubIsDefault,
                                                                              IsExternal = p.IsExternal,
                                                                              MasterTrackingDetailId = p.MasterTrackingDetailId,
                                                                              Message = p.SubEventMessage,
                                                                          }).ToList()
                                                                      }).ToList();


                    if (mobileUserConfiguration.ExpressConfiguration.Count > 0)
                    {
                        foreach (var item in mobileUserConfiguration.ExpressConfiguration)
                        {
                            foreach (var td in item.TrackingDetail)
                            {
                                foreach (var mo in mobileUserDetail)
                                {
                                    if (mo.MasterTrackingDetailId == td.MasterTrackingDetailId)
                                    {
                                        td.MobileUserConfigurationId = mo.MobileUserConfigurationId;
                                        td.Message = mo.EventMessage;
                                        td.IsActive = mo.IsActive;
                                        td.IsExternal = mo.IsExternal;
                                    }
                                }
                            }
                        }
                    }

                }
                if (userAddional.IsTradelaneBooking.HasValue && userAddional.IsTradelaneBooking.Value)
                {
                    mobileUserConfiguration.TradelaneConfiguration = collection.Where(p => p.ModuleType == FrayteShipmentServiceType.TradeLaneBooking).GroupBy(p => p.MasterTrackingId)
                                                                   .Select(group => new MasterTrackingModel
                                                                   {
                                                                       MasterTrackingId = group.FirstOrDefault().MasterTrackingId,
                                                                       EventKey = group.FirstOrDefault().EventKey,
                                                                       EventDisplay = group.FirstOrDefault().EventKeyDisplay,
                                                                       ModuleType = group.FirstOrDefault().ModuleType,
                                                                       TrackingDetail = group.Select(p => new MasterTrackingDetailModel
                                                                       {
                                                                           EventDisplay = p.SubEventDisplay,
                                                                           EventKey = p.EventKey,
                                                                           IsDefault = p.SubIsDefault,
                                                                           IsExternal = p.IsExternal,
                                                                           MasterTrackingDetailId = p.MasterTrackingDetailId,
                                                                           Message = p.SubEventMessage,
                                                                       }).ToList()
                                                                   }).ToList();


                    if (mobileUserConfiguration.TradelaneConfiguration.Count > 0)
                    {
                        foreach (var item in mobileUserConfiguration.TradelaneConfiguration)
                        {
                            foreach (var td in item.TrackingDetail)
                            {
                                foreach (var mo in mobileUserDetail)
                                {
                                    if (mo.MasterTrackingDetailId == td.MasterTrackingDetailId)
                                    {
                                        td.MobileUserConfigurationId = mo.MobileUserConfigurationId;
                                        td.Message = mo.EventMessage;
                                        td.IsActive = mo.IsActive;
                                        td.IsExternal = mo.IsExternal;
                                    }
                                }
                            }
                        }
                    }
                }
                if (userAddional.IsBreakBulkBooking.HasValue && userAddional.IsBreakBulkBooking.Value)
                {
                    mobileUserConfiguration.BreakBulkConfiguration = collection.Where(p => p.ModuleType == FrayteShipmentServiceType.BreakBulk).GroupBy(p => p.MasterTrackingId)
                                                                  .Select(group => new MasterTrackingModel
                                                                  {
                                                                      MasterTrackingId = group.FirstOrDefault().MasterTrackingId,
                                                                      EventKey = group.FirstOrDefault().EventKey,
                                                                      EventDisplay = group.FirstOrDefault().EventKeyDisplay,
                                                                      ModuleType = group.FirstOrDefault().ModuleType,
                                                                      TrackingDetail = group.Select(p => new MasterTrackingDetailModel
                                                                      {
                                                                          EventDisplay = p.SubEventDisplay,
                                                                          EventKey = p.EventKey,
                                                                          IsDefault = p.SubIsDefault,
                                                                          IsExternal = p.IsExternal,
                                                                          MasterTrackingDetailId = p.MasterTrackingDetailId,
                                                                          Message = p.SubEventMessage,
                                                                      }).ToList()
                                                                  }).ToList();


                    if (mobileUserConfiguration.BreakBulkConfiguration.Count > 0)
                    {
                        foreach (var item in mobileUserConfiguration.BreakBulkConfiguration)
                        {
                            foreach (var td in item.TrackingDetail)
                            {
                                foreach (var mo in mobileUserDetail)
                                {
                                    if (mo.MasterTrackingDetailId == td.MasterTrackingDetailId)
                                    {
                                        td.MobileUserConfigurationId = mo.MobileUserConfigurationId;
                                        td.Message = mo.EventMessage;
                                        td.IsActive = mo.IsActive;
                                        td.IsExternal = mo.IsExternal;
                                    }
                                }
                            }
                        }
                    }
                }
                if (userAddional.IsDirectBooking.HasValue && userAddional.IsDirectBooking.Value)
                {
                    mobileUserConfiguration.DirectBookingConfiguration = collection.Where(p => p.ModuleType == FrayteShipmentServiceType.DirectBooking).GroupBy(p => p.MasterTrackingId)
                                                                .Select(group => new MasterTrackingModel
                                                                {
                                                                    MasterTrackingId = group.FirstOrDefault().MasterTrackingId,
                                                                    EventKey = group.FirstOrDefault().EventKey,
                                                                    EventDisplay = group.FirstOrDefault().EventKeyDisplay,
                                                                    ModuleType = group.FirstOrDefault().ModuleType,
                                                                    TrackingDetail = group.Select(p => new MasterTrackingDetailModel
                                                                    {
                                                                        EventDisplay = p.SubEventDisplay,
                                                                        EventKey = p.EventKey,
                                                                        IsDefault = p.SubIsDefault,
                                                                        IsExternal = p.IsExternal,
                                                                        MasterTrackingDetailId = p.MasterTrackingDetailId,
                                                                        Message = p.SubEventMessage,
                                                                    }).ToList()
                                                                }).ToList();


                    if (mobileUserConfiguration.DirectBookingConfiguration.Count > 0)
                    {
                        foreach (var item in mobileUserConfiguration.DirectBookingConfiguration)
                        {
                            foreach (var td in item.TrackingDetail)
                            {
                                foreach (var mo in mobileUserDetail)
                                {
                                    if (mo.MasterTrackingDetailId == td.MasterTrackingDetailId)
                                    {
                                        td.MobileUserConfigurationId = mo.MobileUserConfigurationId;
                                        td.Message = mo.EventMessage;
                                        td.IsActive = mo.IsActive;
                                        td.IsExternal = mo.IsExternal;
                                    }
                                }
                            }
                        }
                    }
                } 
            } 
            return mobileUserConfiguration;
        }

        public FrayteResult SaveMobileconfiguration(MobileInternalUserConfiguration mobileUserConfiguration)
        {
            FrayteResult result = new FrayteResult();
            if (mobileUserConfiguration != null)
            {
                if (mobileUserConfiguration.ExpressConfiguration != null && mobileUserConfiguration.ExpressConfiguration.Count > 0)
                {
                    MobileUserConfiguration config1;

                    foreach (var config in mobileUserConfiguration.ExpressConfiguration)
                    {
                        if (config.TrackingDetail != null && config.TrackingDetail.Count > 0)
                        {
                            foreach (var item in config.TrackingDetail)
                            {

                                config1 = dbContext.MobileUserConfigurations.Find(item.MobileUserConfigurationId);

                                if (config1 != null)
                                {
                                    config1.EventMessage = item.Message;
                                    config1.IsDefalut = item.IsDefault;
                                    config1.IsActive = item.IsActive;
                                    config1.IsExternal = item.IsExternal;
                                    dbContext.SaveChanges();
                                }
                                else
                                {
                                    if (item.IsActive)
                                    {
                                        config1 = new MobileUserConfiguration();

                                        config1.MasterTrackingDetailId = item.MasterTrackingDetailId;
                                        config1.UserId = mobileUserConfiguration.UserId;
                                        config1.IsActive = config1.IsActive;
                                        config1.EventMessage = item.Message;
                                        config1.IsDefalut = item.IsDefault;
                                        config1.IsActive = item.IsActive;
                                        dbContext.MobileUserConfigurations.Add(config1);
                                        dbContext.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }

                result.Status = true;

            }

            return result;
        }

        public List<FrayteInternalUser> GetInternalUserList(TrackFryateUser trackUser)
        {
            List<FrayteInternalUser> lstFrayteUsers = new List<FrayteInternalUser>();

            var OperationZone = UtilityRepository.GetOperationZone();
            int SkipRows = 0;
            SkipRows = (trackUser.CurrentPage - 1) * trackUser.TakeRows;

            var lstUsers = dbContext.spGet_TrackUserDetail(trackUser.RoleId, SkipRows, trackUser.TakeRows, OperationZone.OperationZoneId).ToList();

            if (lstUsers != null && lstUsers.Count > 0)
            {
                foreach (var user in lstUsers)
                {
                    FrayteInternalUser frayteUser = new FrayteInternalUser();

                    frayteUser.UserId = user.UserId;
                    frayteUser.CargoWiseId = user.CargoWiseId;
                    frayteUser.CargoWiseBardCode = user.CargoWiseBardCode;
                    frayteUser.CompanyName = user.CompanyName;
                    frayteUser.ContactName = user.ContactName;
                    frayteUser.Email = user.Email;
                    frayteUser.TelephoneNo = user.TelephoneNo;
                    frayteUser.MobileNo = user.MobileNo;
                    frayteUser.FaxNumber = user.FaxNumber;
                    frayteUser.Timezone = new TimeZoneModal();
                    frayteUser.Timezone.TimezoneId = user.TimezoneId.HasValue ? user.TimezoneId.Value : 0;
                    frayteUser.WorkingStartTime = UtilityRepository.TimeZoneTime(user.WorkingStartTime, frayteUser.Timezone.Name);
                    frayteUser.WorkingEndTime = UtilityRepository.TimeZoneTime(user.WorkingEndTime, frayteUser.Timezone.Name);
                    frayteUser.ShortName = user.ShortName;
                    frayteUser.Position = user.Position;
                    frayteUser.Skype = user.Skype;
                    frayteUser.RoleId = user.RoleId;
                    frayteUser.TotalRows = user.TotalRows.HasValue ? user.TotalRows.Value : 0;
                    frayteUser.RoleName = UtilityRepository.GetRoleName(user.RoleId);

                    frayteUser.ManagerUser = new FrayteCustomerAssociatedUser();
                    if (user.ManagerUserId.HasValue)
                    {
                        frayteUser.ManagerUser.UserId = user.ManagerUserId.Value;
                        frayteUser.ManagerUser.AssociateType = FrayteAssociateType.Manager;
                        frayteUser.ManagerUser.ContactName = user.ManagerName;
                        frayteUser.ManagerUser.Email = user.ManagerEmail;
                        frayteUser.ManagerUser.TelephoneNo = user.ManagerTelephoneNo;
                        frayteUser.ManagerUser.WorkingHours = UtilityRepository.GetWorkingHours(user.ManagerWorkingStartTime, user.ManagerWorkingEndTime);
                    }

                    if (user.ManagerWorkingStartTime.HasValue && user.ManagerWorkingEndTime.HasValue)
                    {
                        frayteUser.ManagerUser.WorkingHours = user.ManagerWorkingStartTime.Value.ToString() + " - " + user.ManagerWorkingEndTime.Value.ToString();
                    }

                    frayteUser.IsFuelSurCharge = user.IsFuelSurCharge == null ? false : user.IsFuelSurCharge.Value;
                    frayteUser.IsCurrency = user.IsCurrency == null ? false : user.IsCurrency.Value;
                    frayteUser.UserAddress = new FrayteAddress();
                    {
                        frayteUser.UserAddress.UserAddressId = user.UserAddressId.HasValue ? user.UserAddressId.Value : 0;
                        frayteUser.UserAddress.UserId = user.UserId;
                        frayteUser.UserAddress.AddressTypeId = user.AddressTypeId.HasValue ? user.AddressTypeId.Value : 0;
                        frayteUser.UserAddress.Address = user.Address;
                        frayteUser.UserAddress.Address2 = user.Address2;
                        frayteUser.UserAddress.Address3 = user.Address3;
                        frayteUser.UserAddress.Suburb = user.Suburb;
                        frayteUser.UserAddress.City = user.City;
                        frayteUser.UserAddress.State = user.State;
                        frayteUser.UserAddress.Zip = user.Zip;
                        frayteUser.UserAddress.Country = new FrayteCountryCode();
                        {
                            var country = dbContext.Countries.Where(p => p.CountryId == user.CountryId).FirstOrDefault();
                            if (country != null)
                            {
                                frayteUser.UserAddress.Country = new FrayteCountryCode();
                                frayteUser.UserAddress.Country.CountryId = country.CountryId;
                                frayteUser.UserAddress.Country.Code = country.CountryCode;
                                frayteUser.UserAddress.Country.Code2 = country.CountryCode2;
                                frayteUser.UserAddress.Country.Name = country.CountryName;
                            }
                        }
                        frayteUser.UserAddress.EasyPostAddressId = user.EasyPostAddressId;
                    }

                    lstFrayteUsers.Add(frayteUser);
                }
            }

            return lstFrayteUsers;
        }

        public List<FrayteSystemRole> GetSystemRoles(int userId)
        {
            List<FrayteSystemRole> roles = new List<FrayteSystemRole>();
            var list = dbContext.Roles.Where(p => p.RoleId != (int)FrayteUserRole.Customer &&
            p.RoleId != (int)FrayteUserRole.MasterAdmin &&
            p.RoleId != (int)FrayteUserRole.Receiver &&
            p.RoleId != (int)FrayteUserRole.Shipper).ToList();
            if (list != null && list.Count > 0)
            {
                foreach (var data in list)
                {
                    FrayteSystemRole role = new FrayteSystemRole();
                    role.RoleId = data.RoleId;
                    role.Name = data.RoleName;
                    role.DisplayName = data.RoleDisplayName;
                    if (data.RoleId == (int)FrayteUserRole.Staff)
                    {
                        role.IsDefault = true;
                    }
                    else
                    {
                        role.IsDefault = false;
                    }

                    roles.Add(role);
                }
            }

            return roles;
        }

        public FrayteInternalUser GetInternalUserDetail(int userId)
        {
            FrayteInternalUser internalUser = new FrayteInternalUser();
            WorkingWeekDay workingDays = new WorkingWeekDay();
            //Step 1: Get internal user's basic information
            var internalU = dbContext.Users.Where(p => p.UserId == userId).FirstOrDefault();

            if (internalU != null)
            {
                internalUser = UtilityRepository.InternalUserMapping(internalU);

                if (internalUser.WorkingWeekDay.WorkingWeekDayId > 0)
                {
                    workingDays = dbContext.WorkingWeekDays.Find(internalUser.WorkingWeekDay.WorkingWeekDayId);
                }

                if (workingDays != null)
                {
                    internalUser.WorkingWeekDay = workingDays;
                }

                var userRole = dbContext.UserRoles.Where(p => p.UserId == userId).FirstOrDefault();
                if (userRole != null)
                {
                    internalUser.RoleId = userRole.RoleId;
                }

                var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == internalU.TimezoneId).FirstOrDefault();
                if (timeZone != null)
                {
                    internalUser.Timezone = new TimeZoneModal();
                    internalUser.Timezone.TimezoneId = timeZone.TimezoneId;
                    internalUser.Timezone.Name = timeZone.Name;
                    internalUser.Timezone.Offset = timeZone.Offset;
                    internalUser.Timezone.OffsetShort = timeZone.OffsetShort;

                    //internalUser.WST = UtilityRepository.GetTimeZoneTime(internalUser.WorkingStartTime.Value.TimeOfDay, timeZone.Name);
                }

                //Step 2: Get internal user's other information
                var internalUserOtherDetails = dbContext.UserAdditionals.Where(p => p.UserId == userId).FirstOrDefault();
                if (internalUserOtherDetails != null)
                {

                    if (internalUserOtherDetails.IsDirectBooking.HasValue)
                    {
                        internalUser.IsDirectBooking = internalUserOtherDetails.IsDirectBooking.Value;
                    }
                    else
                    {
                        internalUser.IsDirectBooking = false;
                    }
                    if (internalUserOtherDetails.IsECommerce.HasValue)
                    {
                        internalUser.IsECommerce = internalUserOtherDetails.IsECommerce.Value;
                    }
                    else
                    {
                        internalUser.IsECommerce = false;
                    }
                    if (internalUserOtherDetails.IsTradelaneBooking.HasValue)
                    {
                        internalUser.IsTradelaneBooking = internalUserOtherDetails.IsTradelaneBooking.Value;
                    }
                    else
                    {
                        internalUser.IsTradelaneBooking = false;
                    }

                    if (internalUserOtherDetails.IsBreakBulkBooking.HasValue)
                    {
                        internalUser.IsBreakBulk = internalUserOtherDetails.IsBreakBulkBooking.Value;
                    }
                    else
                    {
                        internalUser.IsBreakBulk = false;
                    }
                    if (internalUserOtherDetails.IsExpressSolutions.HasValue)
                    {
                        internalUser.IsExpressBooking = internalUserOtherDetails.IsExpressSolutions.Value;
                    }
                    else
                    {
                        internalUser.IsExpressBooking = false;
                    }
                    if (internalUserOtherDetails.IsWarehouseTransport.HasValue)
                    {
                        internalUser.IsWarehouseTransport = internalUserOtherDetails.IsWarehouseTransport.Value;
                    }
                    else
                    {
                        internalUser.IsWarehouseTransport = false;
                    }

                    if (internalUserOtherDetails.IsFuelSurCharge.HasValue)
                    {
                        internalUser.IsFuelSurCharge = internalUserOtherDetails.IsFuelSurCharge.Value;
                    }

                    if (internalUserOtherDetails.IsCurrency.HasValue)
                    {
                        internalUser.IsCurrency = internalUserOtherDetails.IsCurrency.Value;
                    }
                    //Get associated Frayte User's detail
                    GetAssociateUsersDetail(internalUser, internalUserOtherDetails);
                }

                //Step 3: Get user address
                var address = dbContext.UserAddresses.Where(p => p.UserId == userId).FirstOrDefault();
                if (address != null)
                {
                    internalUser.UserAddress = new FrayteAddress();
                    internalUser.UserAddress.Address = address.Address;
                    internalUser.UserAddress.Address2 = address.Address2;
                    internalUser.UserAddress.Address3 = address.Address3;
                    internalUser.UserAddress.AddressTypeId = address.AddressTypeId;
                    internalUser.UserAddress.City = address.City;
                    internalUser.UserAddress.EasyPostAddressId = address.EasyPostAddressId;
                    internalUser.UserAddress.State = address.State;
                    internalUser.UserAddress.Suburb = address.Suburb;
                    internalUser.UserAddress.UserAddressId = address.UserAddressId;
                    internalUser.UserAddress.UserId = address.UserId;
                    internalUser.UserAddress.Zip = address.Zip;
                    internalUser.UserAddress.Country = new FrayteCountryCode();
                    {
                        var country = dbContext.Countries.Where(p => p.CountryId == address.CountryId).FirstOrDefault();
                        if (country != null)
                        {
                            internalUser.UserAddress.Country.CountryId = country.CountryId;
                            internalUser.UserAddress.Country.Code = country.CountryCode;
                            internalUser.UserAddress.Country.Code2 = country.CountryCode2;
                            internalUser.UserAddress.Country.Name = country.CountryName;
                            internalUser.UserAddress.Country.CountryPhoneCode = country.CountryPhoneCode;
                        }
                    }
                }
            }

            return internalUser;
        }

        public int GetUserRole(int id)
        {
            var data = dbContext.UserRoles.Where(p => p.UserId == id).FirstOrDefault();
            if (data != null)
            {
                return data.RoleId;
            }
            else
            {
                return 0;
            }
        }

        public List<FrayteCustomerAssociatedUser> GetAssociatedUsers(string searchName)
        {
            List<FrayteCustomerAssociatedUser> lstAssociatedUser = new List<FrayteCustomerAssociatedUser>();

            var result = (from u in dbContext.Users
                          join r in dbContext.UserRoles on u.UserId equals r.UserId
                          join ua in dbContext.UserAddresses on u.UserId equals ua.UserId
                          join c in dbContext.Countries on ua.CountryId equals c.CountryId
                          where u.ContactName.Contains(searchName)
                          && u.IsActive == true
                          && (r.RoleId == (int)FrayteUserRole.Staff || r.RoleId == (int)FrayteUserRole.HSCodeOperatorManager)
                          select new
                          {
                              UserId = u.UserId,
                              ContactName = u.ContactName,
                              Email = u.Email,
                              TelephoneNo = u.TelephoneNo,
                              WorkingStartTime = u.WorkingStartTime,
                              WorkingEndTime = u.WorkingEndTime,
                              TimezoneId = u.TimezoneId,
                              PhoneCode = c.CountryPhoneCode
                          }).ToList();

            if (result != null)
            {
                foreach (var user in result)
                {
                    FrayteCustomerAssociatedUser associatedUser = new FrayteCustomerAssociatedUser();
                    associatedUser.UserId = user.UserId;
                    associatedUser.AssociateType = "Manager";
                    associatedUser.ContactName = user.ContactName;
                    associatedUser.Email = user.Email;
                    associatedUser.TelephoneNo = user.PhoneCode == "" ? user.TelephoneNo : "(+" + user.PhoneCode + ") " + user.TelephoneNo;
                    associatedUser.WorkingHours = UtilityRepository.GetWorkingHoursWithTimeZone(user.WorkingStartTime, user.WorkingEndTime, user.TimezoneId);
                    lstAssociatedUser.Add(associatedUser);
                }
            }

            return lstAssociatedUser;
        }

        public void SaveUser(FrayteInternalUser frayteUser)
        {
            FrayteUserRepository userRepository = new FrayteUserRepository();

            //Step 1: Save User Details
            userRepository.SaveUserDetail(frayteUser);

            //Step 2: Save useradditional details
            SaveUserAdditional(frayteUser);

            //Step 3: Save useraddress details
            SaveUserAddress(frayteUser.UserAddress);

            //Step 3: Save user role
            userRepository.SaveUserRole(frayteUser.UserId, frayteUser.RoleId);
        }

        public List<User> getUserByEmail(string email)
        {
            try
            {
                var users = dbContext.Users.Where(p => p.UserEmail == email).ToList();
                return users;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region -- Public Methods --

        public List<FrayteUser> GetUserTypeList(int userType, int userAddressType)
        {
            List<FrayteUser> lstFrayteUsers = new List<FrayteUser>();

            var operationzone = UtilityRepository.GetOperationZone();
            var lstAgents = (from c in dbContext.Users
                             join ur in dbContext.UserRoles on c.UserId equals ur.UserId
                             where ur.RoleId == userType &&
                                   c.OperationZoneId == operationzone.OperationZoneId &&
                                   c.IsActive == true
                             select c).ToList();

            if (lstAgents != null)
            {
                foreach (User user in lstAgents)
                {
                    FrayteUser frayteUser = UtilityRepository.UserMapping(user);
                    frayteUser.TelephoneNo = user.TelephoneNo;
                    // Get Customer Account No.
                    var data = dbContext.UserAdditionals.Where(p => p.UserId == user.UserId).FirstOrDefault();
                    if (data != null)
                    {
                        frayteUser.CustomerAccountNo = data.AccountNo;
                        frayteUser.CustomerRateCardType = data.CustomerRateCardType;
                        frayteUser.UserType = data.UserType;
                    }
                    //Get agent address information                    
                    frayteUser.UserAddress = new FrayteAddress();
                    var userAddress = (from ua in dbContext.UserAddresses
                                       join c in dbContext.Countries on ua.CountryId equals c.CountryId
                                       where ua.UserId == user.UserId && ua.AddressTypeId == userAddressType
                                       select new
                                       {
                                           ua.UserAddressId,
                                           ua.UserId,
                                           ua.AddressTypeId,
                                           ua.Address,
                                           ua.Address2,
                                           ua.Address3,
                                           ua.Suburb,
                                           ua.City,
                                           ua.State,
                                           ua.Zip,
                                           ua.CountryId,
                                           c.CountryCode,
                                           c.CountryName
                                       }).FirstOrDefault();


                    if (userAddress != null)
                    {
                        frayteUser.UserAddress.UserAddressId = userAddress.UserAddressId;
                        frayteUser.UserAddress.UserId = userAddress.UserId;
                        frayteUser.UserAddress.AddressTypeId = userAddress.AddressTypeId;
                        frayteUser.UserAddress.Address = userAddress.Address;
                        frayteUser.UserAddress.Address2 = userAddress.Address2;
                        frayteUser.UserAddress.Address3 = userAddress.Address3;
                        frayteUser.UserAddress.Suburb = userAddress.Suburb;
                        frayteUser.UserAddress.City = userAddress.City;
                        frayteUser.UserAddress.State = userAddress.State;
                        frayteUser.UserAddress.Zip = userAddress.Zip;
                        frayteUser.UserAddress.Country = new FrayteCountryCode();
                        frayteUser.UserAddress.Country.CountryId = userAddress.CountryId;
                        frayteUser.UserAddress.Country.Code = userAddress.CountryCode;
                        frayteUser.UserAddress.Country.Name = userAddress.CountryName;
                    }

                    lstFrayteUsers.Add(frayteUser);
                }
            }

            return lstFrayteUsers;
        }

        public List<FrayteUser> GetCustomerTypeList(int userType, int userAddressType, int UserId)
        {
            List<FrayteUser> lstFrayteUsers = new List<FrayteUser>();

            List<User> lstAgents = new List<User>();

            var operationzone = UtilityRepository.GetOperationZone();

            var role = (from r in dbContext.Roles
                        join ur in dbContext.UserRoles on r.RoleId equals ur.RoleId
                        where ur.UserId == UserId
                        select new
                        {
                            r.RoleName
                        }).FirstOrDefault();

            if (role != null && (role.RoleName == "Admin" || role.RoleName == "Staff"))
            {
                lstAgents = (from c in dbContext.Users
                             join ur in dbContext.UserRoles on c.UserId equals ur.UserId
                             where ur.RoleId == userType &&
                                   c.OperationZoneId == operationzone.OperationZoneId &&
                                   c.IsActive == true
                             select c).ToList();
            }
            else if (role != null && (role.RoleName == "Customer"))
            {
                lstAgents = (from c in dbContext.Users
                             join ur in dbContext.UserRoles on c.UserId equals ur.UserId
                             where ur.RoleId == (int)FrayteUserRole.UserCustomer &&
                                 c.OperationZoneId == operationzone.OperationZoneId &&
                                 c.CreatedBy == UserId &&
                                 c.IsActive == true
                             select c).ToList();
            }

            if (lstAgents != null)
            {
                foreach (User user in lstAgents)
                {
                    FrayteUser frayteUser = UtilityRepository.UserMapping(user);
                    frayteUser.TelephoneNo = user.TelephoneNo;
                    // Get Customer Account No.
                    var data = dbContext.UserAdditionals.Where(p => p.UserId == user.UserId).FirstOrDefault();
                    if (data != null)
                    {
                        frayteUser.CustomerAccountNo = data.AccountNo;
                        frayteUser.CustomerRateCardType = data.CustomerRateCardType;
                    }
                    //Get agent address information                    
                    frayteUser.UserAddress = new FrayteAddress();
                    var userAddress = (from ua in dbContext.UserAddresses
                                       join c in dbContext.Countries on ua.CountryId equals c.CountryId
                                       where ua.UserId == user.UserId && ua.AddressTypeId == userAddressType
                                       select new
                                       {
                                           ua.UserAddressId,
                                           ua.UserId,
                                           ua.AddressTypeId,
                                           ua.Address,
                                           ua.Address2,
                                           ua.Address3,
                                           ua.Suburb,
                                           ua.City,
                                           ua.State,
                                           ua.Zip,
                                           ua.CountryId,
                                           c.CountryCode,
                                           c.CountryName
                                       }).FirstOrDefault();


                    if (userAddress != null)
                    {
                        frayteUser.UserAddress.UserAddressId = userAddress.UserAddressId;
                        frayteUser.UserAddress.UserId = userAddress.UserId;
                        frayteUser.UserAddress.AddressTypeId = userAddress.AddressTypeId;
                        frayteUser.UserAddress.Address = userAddress.Address;
                        frayteUser.UserAddress.Address2 = userAddress.Address2;
                        frayteUser.UserAddress.Address3 = userAddress.Address3;
                        frayteUser.UserAddress.Suburb = userAddress.Suburb;
                        frayteUser.UserAddress.City = userAddress.City;
                        frayteUser.UserAddress.State = userAddress.State;
                        frayteUser.UserAddress.Zip = userAddress.Zip;
                        frayteUser.UserAddress.Country = new FrayteCountryCode();
                        frayteUser.UserAddress.Country.CountryId = userAddress.CountryId;
                        frayteUser.UserAddress.Country.Code = userAddress.CountryCode;
                        frayteUser.UserAddress.Country.Name = userAddress.CountryName;
                    }

                    lstFrayteUsers.Add(frayteUser);
                }
            }

            return lstFrayteUsers;
        }

        public void SaveUserDetail(FrayteUser frayteUser)
        {
            User user;
            if (frayteUser.UserId == 0)
            {
                user = new User();
                user.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
                user.CargoWiseId = frayteUser.CargoWiseId;
                user.CargoWiseBardCode = frayteUser.CargoWiseBardCode;
                user.WorkingWeekDayId = frayteUser.WorkingWeekDay.WorkingWeekDayId;
                user.CompanyName = frayteUser.CompanyName;
                user.ClientId = frayteUser.ClientId;
                user.IsClient = frayteUser.IsClient;
                user.CountryOfOperation = frayteUser.CountryOfOperation;
                user.ContactName = frayteUser.ContactName;
                user.Email = frayteUser.Email;
                user.TelephoneNo = frayteUser.TelephoneNo;
                user.MobileNo = frayteUser.MobileNo;
                user.TimezoneId = frayteUser.Timezone != null ? frayteUser.Timezone.TimezoneId : 0;
                user.WorkingStartTime = frayteUser.WorkingStartTime.TimeOfDay;
                user.WorkingEndTime = frayteUser.WorkingEndTime.TimeOfDay;
                user.VATGST = frayteUser.VATGST;
                user.ShortName = frayteUser.ShortName;
                user.Position = frayteUser.Position;
                user.Skype = frayteUser.Skype;
                user.IsActive = true;
                user.CreatedOn = DateTime.UtcNow;
                user.CreatedBy = frayteUser.CreatedBy;
                dbContext.Users.Add(user);
            }
            else
            {
                user = dbContext.Users.Where(p => p.UserId == frayteUser.UserId).FirstOrDefault();

                if (user != null)
                {
                    user.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
                    user.CargoWiseId = frayteUser.CargoWiseId;
                    user.CargoWiseBardCode = frayteUser.CargoWiseBardCode;
                    user.WorkingWeekDayId = frayteUser.WorkingWeekDay.WorkingWeekDayId;
                    user.CompanyName = frayteUser.CompanyName;
                    user.ClientId = frayteUser.ClientId;
                    user.IsClient = frayteUser.IsClient;
                    user.CountryOfOperation = frayteUser.CountryOfOperation;
                    user.ContactName = frayteUser.ContactName;
                    user.Email = frayteUser.Email;
                    user.TelephoneNo = frayteUser.TelephoneNo;
                    user.MobileNo = frayteUser.MobileNo;
                    user.FaxNumber = frayteUser.FaxNumber;
                    user.TimezoneId = frayteUser.Timezone != null ? frayteUser.Timezone.TimezoneId : 0;
                    user.WorkingStartTime = UtilityRepository.GetTimeZoneUTCTime(frayteUser.WorkingStartTime.ToString(), user.TimezoneId);
                    user.WorkingEndTime = UtilityRepository.GetTimeZoneUTCTime(frayteUser.WorkingEndTime.ToString(), user.TimezoneId);
                    user.VATGST = frayteUser.VATGST;
                    user.ShortName = frayteUser.ShortName;
                    user.Position = frayteUser.Position;
                    user.Skype = frayteUser.Skype;
                    user.UpdatedOn = DateTime.UtcNow;
                    user.UpdatedBy = 1;
                }
            }

            if (user != null)
            {
                dbContext.SaveChanges();
            }
            frayteUser.UserId = user.UserId;
            frayteUser.UserAddress.UserId = user.UserId;
        }

        public void SaveUserRole(int userId, int roleId)
        {
            UserRole userRole = dbContext.UserRoles.Where(p => p.UserId == userId && p.RoleId == roleId).FirstOrDefault();
            if (userRole == null)
            {
                userRole = new UserRole();
                userRole.UserId = userId;
                userRole.RoleId = roleId;
                dbContext.UserRoles.Add(userRole);
                dbContext.SaveChanges();
            }
        }

        public void SaveUserAddress(FrayteAddress frayteUserAddress)
        {
            UserAddress userAddress;
            if (frayteUserAddress.UserAddressId == 0)
            {
                userAddress = new UserAddress();
                userAddress.UserId = frayteUserAddress.UserId;
                userAddress.AddressTypeId = frayteUserAddress.AddressTypeId;
                userAddress.Address = frayteUserAddress.Address;
                userAddress.Address2 = frayteUserAddress.Address2;
                userAddress.Address3 = frayteUserAddress.Address3;
                userAddress.City = frayteUserAddress.City;
                userAddress.Suburb = frayteUserAddress.Suburb;
                userAddress.State = frayteUserAddress.State;
                userAddress.Zip = frayteUserAddress.Zip;
                userAddress.CountryId = frayteUserAddress.Country.CountryId;
                dbContext.UserAddresses.Add(userAddress);
            }
            else
            {
                userAddress = dbContext.UserAddresses.Where(p => p.UserAddressId == frayteUserAddress.UserAddressId).FirstOrDefault();
                if (userAddress != null)
                {
                    userAddress.UserId = frayteUserAddress.UserId;
                    userAddress.AddressTypeId = frayteUserAddress.AddressTypeId;
                    userAddress.Address = frayteUserAddress.Address;
                    userAddress.Address2 = frayteUserAddress.Address2;
                    userAddress.Address3 = frayteUserAddress.Address3;
                    userAddress.City = frayteUserAddress.City;
                    userAddress.Suburb = frayteUserAddress.Suburb;
                    userAddress.State = frayteUserAddress.State;
                    userAddress.Zip = frayteUserAddress.Zip;
                    userAddress.CountryId = frayteUserAddress.Country.CountryId;
                }
            }

            if (userAddress != null)
            {
                dbContext.SaveChanges();
            }

            frayteUserAddress.UserAddressId = userAddress.UserAddressId;
        }

        public FrayteResult DeleteUser(int userId, int userRoleId, int addressType)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                if (addressType > 0)
                {
                    UserAddress userAddress = dbContext.UserAddresses.Where(p => p.UserId == userId && p.AddressTypeId == addressType).FirstOrDefault();
                    if (userAddress != null)
                    {
                        //Step 1: Delete User Address                                        
                        dbContext.UserAddresses.Remove(userAddress);
                        dbContext.SaveChanges();
                    }
                }

                //Remove user role
                UserRole userRole = dbContext.UserRoles.Where(p => p.UserId == userId && p.RoleId == userRoleId).FirstOrDefault();
                if (userRole != null)
                {
                    dbContext.UserRoles.Remove(userRole);
                    dbContext.SaveChanges();
                }

                //Step 2: Delete User
                var user = new User { UserId = userId };
                dbContext.Users.Attach(user);
                dbContext.Users.Remove(user);
                dbContext.SaveChanges();

                result.Status = true;

            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Errors = new List<string>();
                result.Errors.Add(ex.Message);
            }

            return result;
        }

        public FrayteResult MarkForDelete(int userId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                User userResult = dbContext.Users.Where(p => p.UserId == userId).FirstOrDefault();
                if (userResult != null)
                {
                    //User marked IsActive = false means user will no longer is available in the system.
                    userResult.IsActive = false;
                    dbContext.SaveChanges();

                    result.Status = true;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Errors = new List<string>();
                result.Errors.Add(ex.Message);
            }

            return result;
        }

        public void SendEmail_NewUser(FrayteLoginUserLogin userLogin)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            NewUserModel mailDetail = new NewUserModel();
            var user = dbContext.Users.Find(userLogin.UserId);
            var OperationName = UtilityRepository.GetOperationZone();
            string Site = string.Empty;
            if (OperationName.OperationZoneId == 1)
            {
                Site = AppSettings.TrackingUrl;
            }
            else if (OperationName.OperationZoneId == 2)
            {
                Site = AppSettings.TrackingUrl;
            }
            mailDetail.Name = user.ContactName;
            mailDetail.UserName = userLogin.UserName;
            mailDetail.Password = userLogin.Password;
            mailDetail.ImageHeader = "FrayteLogo";
            string OperationUserEmail = "";
            int RoleId = 0;
            if (userLogin.OperationStaffId > 0)
            {
                var result = (from Usr in dbContext.Users
                              join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                              where Usr.UserId == userLogin.OperationStaffId
                              select new
                              {
                                  Usr.Email,
                                  Rl.RoleId
                              }).FirstOrDefault();

                if (result != null)
                {
                    RoleId = result.RoleId;
                    OperationUserEmail = result.Email;
                }
            }

            mailDetail.RoleId = userLogin.RoleId;
            if (OperationName.OperationZoneId == 1)
            {
                mailDetail.SalesEmail = OperationUserEmail != "" ? OperationUserEmail : "sales@frayte.com";
                mailDetail.DeptName = RoleId != 1 ? "Operation Staff" : "Admin";
                mailDetail.PhoneNumber = "(+852) 2148 4880";
                mailDetail.SiteAddress = AppSettings.TrackingUrl;
                mailDetail.SiteLink = "Frayte HK";
                mailDetail.RecoveryLink = string.Format(AppSettings.HKUrl + "newPassword/{0}", user.UserId);
            }
            if (OperationName.OperationZoneId == 2)
            {
                mailDetail.SalesEmail = OperationUserEmail != "" ? OperationUserEmail : "sales@frayte.co.uk";
                mailDetail.DeptName = RoleId != 1 ? "Operation Staff" : "Admin";
                mailDetail.PhoneNumber = "(+44) 01792 277295";
                mailDetail.SiteAddress = AppSettings.TrackingUrl;
                mailDetail.SiteLink = "Frayte UK";
                mailDetail.RecoveryLink = string.Format(AppSettings.UKUrl + "newPassword/{0}", user.UserId);
            }

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/NewUser.cshtml");
            var templateService = new TemplateService();

            var EmailBody = templateService.Parse(template, mailDetail, null, null);
            var EmailSubject = "Login Credentials for Frayte Management System";

            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("new User " + userLogin.UserName + " " + userLogin.UserEmail)));
            FrayteEmail.SendMail(userLogin.UserEmail, "", EmailSubject, EmailBody, "", logoImage);
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception(EmailBody)));
        }

        public void SendEmail_NewUserCustomer(FrayteLoginUserLogin userLogin, string ContactName, int CreatedBy)
        {
            try
            {
                NewUserModel mailDetail = new NewUserModel();
                var user = dbContext.CustomerCompanyDetails.Where(p => p.UserId == CreatedBy).FirstOrDefault();
                if (user != null)
                {
                    mailDetail.Name = ContactName;
                    mailDetail.UserName = userLogin.UserName;
                    mailDetail.Password = userLogin.Password;
                    mailDetail.ImageHeader = "FrayteLogo";
                    mailDetail.RoleId = userLogin.RoleId;
                    mailDetail.SalesEmail = user.SalesStaffEmail;
                    mailDetail.DeptName = user.OperationStaff;
                    mailDetail.PhoneNumber = user.OperationStaffPhone;
                    mailDetail.SiteAddress = user.SiteAddress;
                    mailDetail.SiteLink = user.SiteLink;
                    mailDetail.RecoveryLink = string.Format(user.RecoveryLink + "#/newPassword/{0}", userLogin.UserId);

                    string logoImage = AppSettings.EmailServicePath + "/EmailTeamplate/" + CreatedBy + "/Images/" + user.LogoFileName;

                    string template = template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + CreatedBy + "/NewUser.cshtml");
                    var templateService = new TemplateService();

                    var EmailBody = templateService.Parse(template, mailDetail, null, null);
                    var EmailSubject = "Login Credentials for " + user.KindRegards + " Management System";

                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("new user Customer " + userLogin.UserName + " " + userLogin.UserEmail)));

                    FrayteEmail.SendMail(userLogin.UserEmail, "", EmailSubject, EmailBody, "", logoImage, CreatedBy);

                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception(EmailBody)));

                }
            }
            catch (Exception ex)
            {

            }
        }

        public void SendWelcomeEmail_NewAgent(FrayteLoginUserLogin userLogin)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            NewUserModel mailDetail = new NewUserModel();
            var user = dbContext.Users.Find(userLogin.UserId);
            var OperationName = UtilityRepository.GetOperationZone();
            string Site = string.Empty;
            if (OperationName.OperationZoneId == 1)
            {
                Site = AppSettings.TrackingUrl;
            }
            else if (OperationName.OperationZoneId == 2)
            {
                Site = AppSettings.TrackingUrl;
            }
            mailDetail.Name = user.ContactName;
            mailDetail.UserName = userLogin.UserName;
            mailDetail.Password = userLogin.Password;
            mailDetail.ImageHeader = "FrayteLogo";
            string OperationUserEmail = "";
            int RoleId = 0;
            if (userLogin.OperationStaffId > 0)
            {
                var result = (from Usr in dbContext.Users
                              join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                              where Usr.UserId == userLogin.OperationStaffId
                              select new
                              {
                                  Usr.Email,
                                  Rl.RoleId
                              }).FirstOrDefault();

                if (result != null)
                {
                    RoleId = result.RoleId;
                    OperationUserEmail = result.Email;
                }
            }

            mailDetail.RoleId = userLogin.RoleId;
            if (OperationName.OperationZoneId == 1)
            {
                mailDetail.SalesEmail = OperationUserEmail != "" ? OperationUserEmail : "sales@frayte.com";
                mailDetail.DeptName = RoleId != 1 ? "Operation Staff" : "Admin";
                mailDetail.PhoneNumber = "(+852) 2148 4880";
                mailDetail.SiteAddress = AppSettings.TrackingUrl;
                mailDetail.SiteLink = "Frayte HK";
                mailDetail.RecoveryLink = string.Format(AppSettings.HKUrl + "newPassword/{0}", user.UserId);
            }
            if (OperationName.OperationZoneId == 2)
            {
                mailDetail.SalesEmail = OperationUserEmail != "" ? OperationUserEmail : "sales@frayte.co.uk";
                mailDetail.DeptName = RoleId != 1 ? "Operation Staff" : "Admin";
                mailDetail.PhoneNumber = "(+44) 01792 277295";
                mailDetail.SiteAddress = AppSettings.TrackingUrl;
                mailDetail.SiteLink = "Frayte UK";
                mailDetail.RecoveryLink = string.Format(AppSettings.UKUrl + "newPassword/{0}", user.UserId);
            }

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/NewAgentWelcome.cshtml");
            var templateService = new TemplateService();

            var EmailBody = templateService.Parse(template, mailDetail, null, null);
            //   var EmailBody = Engine.Razor.(template, mailDetail, null, null);
            var EmailSubject = "Welcome to the FRAYTE Tradelane Management System";
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("new Agent " + userLogin.UserName + " " + userLogin.UserEmail)));
            FrayteEmail.SendMail(userLogin.UserEmail, "", EmailSubject, EmailBody, "", logoImage);
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception(EmailBody)));
        }

        #endregion -- Public Methods --

        #region -- Private Methods --

        public void SaveUserAdditional(FrayteInternalUser frayteInternalUser)
        {
            UserAdditional customerDetail = dbContext.UserAdditionals.Where(p => p.UserId == frayteInternalUser.UserId).FirstOrDefault();

            if (customerDetail == null)
            {
                customerDetail = new UserAdditional();
                customerDetail.UserId = frayteInternalUser.UserId;
                if (frayteInternalUser.ManagerUser != null && frayteInternalUser.ManagerUser.UserId > 0)
                    customerDetail.ManagerUserId = frayteInternalUser.ManagerUser.UserId;
                customerDetail.IsFuelSurCharge = frayteInternalUser.IsFuelSurCharge;
                customerDetail.IsCurrency = frayteInternalUser.IsCurrency;
                customerDetail.IsApiAllow = false;
                customerDetail.CustomerType = null;
                customerDetail.IsDirectBooking = frayteInternalUser.IsDirectBooking;
                customerDetail.IsECommerce = frayteInternalUser.IsECommerce;
                customerDetail.IsTradelaneBooking = frayteInternalUser.IsTradelaneBooking;
                customerDetail.IsBreakBulkBooking = frayteInternalUser.IsBreakBulk;
                customerDetail.IsExpressSolutions = frayteInternalUser.IsExpressBooking;
                customerDetail.IsWarehouseTransport = frayteInternalUser.IsWarehouseTransport;

                dbContext.UserAdditionals.Add(customerDetail);
            }
            else
            {
                if (frayteInternalUser.ManagerUser != null && frayteInternalUser.ManagerUser.UserId > 0)
                    customerDetail.ManagerUserId = frayteInternalUser.ManagerUser.UserId;
                customerDetail.IsFuelSurCharge = frayteInternalUser.IsFuelSurCharge;
                customerDetail.IsCurrency = frayteInternalUser.IsCurrency;
                customerDetail.IsApiAllow = false;
                customerDetail.CustomerType = null;
                customerDetail.IsDirectBooking = frayteInternalUser.IsDirectBooking;
                customerDetail.IsECommerce = frayteInternalUser.IsECommerce;
                customerDetail.IsTradelaneBooking = frayteInternalUser.IsTradelaneBooking;
                customerDetail.IsBreakBulkBooking = frayteInternalUser.IsBreakBulk;
                customerDetail.IsExpressSolutions = frayteInternalUser.IsExpressBooking;
                customerDetail.IsWarehouseTransport = frayteInternalUser.IsWarehouseTransport;
            }

            if (customerDetail != null)
            {
                dbContext.SaveChanges();
            }
        }

        private void GetAssociateUsersDetail(FrayteInternalUser internalUserDetail, UserAdditional userAdditionalInfo)
        {
            List<int> associateFrayteUserIds = new List<int>();

            if (userAdditionalInfo.ManagerUserId.HasValue)
            {
                associateFrayteUserIds.Add(userAdditionalInfo.ManagerUserId.Value);
            }

            if (associateFrayteUserIds.Count > 0)
            {
                var result = dbContext.Users.Where(p => associateFrayteUserIds.Contains(p.UserId)).ToList();
                if (result != null)
                {
                    if (userAdditionalInfo.ManagerUserId.HasValue)
                    {
                        User findUser = result.Where(p => p.UserId == userAdditionalInfo.ManagerUserId.Value).FirstOrDefault();
                        if (findUser != null)
                        {
                            FrayteCustomerAssociatedUser associateUser = new FrayteCustomerAssociatedUser();
                            associateUser.UserId = findUser.UserId;
                            associateUser.AssociateType = FrayteAssociateType.Manager;
                            associateUser.ContactName = findUser.ContactName;
                            associateUser.Email = findUser.Email;
                            associateUser.TelephoneNo = findUser.TelephoneNo;
                            associateUser.WorkingHours = UtilityRepository.GetWorkingHours(findUser.WorkingStartTime, findUser.WorkingEndTime);
                            internalUserDetail.ManagerUser = associateUser;
                        }
                    }
                }
            }
        }

        public void SaveAssociateCustomer(FrayteInternalUser internalUserDetail)
        {
            if (internalUserDetail != null)
            {
                var createdinfo = (from u in dbContext.Users
                                   join ur in dbContext.UserRoles on u.UserId equals ur.UserId
                                   where u.UserId == internalUserDetail.LoginUserId
                                   select new
                                   {
                                       ur.RoleId
                                   }).FirstOrDefault();

                CustomerStaff cs;
                if (createdinfo != null && createdinfo.RoleId == (int)FrayteUserRole.Customer)
                {
                    foreach (var cc in internalUserDetail.AssociateCustomer)
                    {
                        if (cc.CustomerStaffDetailId == 0)
                        {
                            cs = new CustomerStaff();
                            cs.CustomerStaffId = internalUserDetail.UserId;
                            cs.UserId = internalUserDetail.CreatedBy;
                            cs.IsActive = true;
                            dbContext.CustomerStaffs.Add(cs);
                            dbContext.SaveChanges();
                        }
                    }
                }
                else
                {
                    if (internalUserDetail.AssociateCustomer.Count > 0)
                    {
                        foreach (var cc in internalUserDetail.AssociateCustomer)
                        {
                            if (cc.CustomerStaffDetailId == 0)
                            {
                                cs = new CustomerStaff();
                                cs.CustomerStaffId = internalUserDetail.UserId;
                                cs.UserId = cc.CustomerId;
                                cs.IsActive = true;
                                dbContext.CustomerStaffs.Add(cs);
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                var detail = dbContext.CustomerStaffs.Find(cc.CustomerStaffDetailId);
                                if (detail != null)
                                {
                                    detail.CustomerStaffId = internalUserDetail.UserId;
                                    detail.UserId = cc.CustomerId;
                                    detail.IsActive = true;
                                    dbContext.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion -- Private Methods --

        public FrayteResult CheckUserEmail(string email)
        {
            FrayteResult result = new FrayteResult();
            var userResult = dbContext.Users.Where(p => p.Email == email).FirstOrDefault();

            if (userResult != null)
            {
                if (userResult.IsActive == true)
                {
                    result.Status = true;
                }
                else
                {
                    result.Status = false;
                }
            }

            return result;
        }
    }
}