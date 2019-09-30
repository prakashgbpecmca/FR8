using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;

namespace Frayte.Services.Business
{
    public class LoginRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        //User login Table is removed now // Prakash will delete it later
        //public LoginDetail Login(FrayteLogin userCrednetial)
        //{
        //    var userResult = (from UL in dbContext.UserLogins
        //                      join UR in dbContext.UserRoles on UL.UserId equals UR.UserId
        //                      where UL.UserName == userCrednetial.UserName && UL.Password == userCrednetial.Password
        //                      select new
        //                      {
        //                          LastLogIn = UL.LastLoginDate,
        //                          RoleId = UR.RoleId,
        //                          UserId = UL.UserId
        //                      }).FirstOrDefault();

        //    if (userResult != null)
        //    {
        //        if (userResult.RoleId > 0 && userResult.RoleId != (int)FrayteUserRole.Customer)
        //        {
        //            var OperationZone = UtilityRepository.GetOperationZone();
        //            var result = (from ul in dbContext.UserLogins
        //                          join u in dbContext.Users on ul.UserId equals u.UserId
        //                          join ur in dbContext.UserRoles on ul.UserId equals ur.UserId
        //                          where
        //                               ul.UserName == userCrednetial.UserName &&
        //                               ul.Password == userCrednetial.Password &&
        //                               u.IsActive == true
        //                          select new LoginDetail()
        //                          {
        //                              EmployeeId = ul.UserId,
        //                              EmployeeName = u.ContactName,
        //                              EmployeeMail = u.Email,
        //                              EmployeeRoleId = ur.RoleId,
        //                              SessionId = "123456789",
        //                              IsLastLogin = userResult.LastLogIn.HasValue ? true : false,
        //                              PhotoUrl = ul.ProfileImage,
        //                              ValidDays = 0,
        //                              CustomerCurrency = "",
        //                              OperationZoneId = OperationZone.OperationZoneId,
        //                              OperationZoneName = OperationZone.OperationZoneName
        //                          }).FirstOrDefault();

        //            return result;
        //        }
        //        else
        //        {
        //            var MarginCost = dbContext.CustomerMarginCosts.Where(p => p.CustomerId == userResult.UserId).ToList();
        //            if (MarginCost != null && MarginCost.Count > 0)
        //            {
        //                var OperationZone = UtilityRepository.GetOperationZone();
        //                var result = (from ul in dbContext.UserLogins
        //                              join u in dbContext.Users on ul.UserId equals u.UserId
        //                              join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
        //                              join ur in dbContext.UserRoles on ul.UserId equals ur.UserId
        //                              where
        //                                   ul.UserName == userCrednetial.UserName &&
        //                                   ul.Password == userCrednetial.Password &&
        //                                   u.IsActive == true
        //                              select new LoginDetail()
        //                              {
        //                                  EmployeeId = ul.UserId,
        //                                  EmployeeName = u.ContactName,
        //                                  EmployeeMail = u.Email,
        //                                  EmployeeRoleId = ur.RoleId,
        //                                  SessionId = "123456789",
        //                                  IsLastLogin = userResult.LastLogIn.HasValue ? true : false,
        //                                  ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
        //                                  CustomerCurrency = ua.CreditLimitCurrencyCode,
        //                                  PhotoUrl = ul.ProfileImage,
        //                                  OperationZoneId = OperationZone.OperationZoneId,
        //                                  OperationZoneName = OperationZone.OperationZoneName,
        //                                  UserOperationZoneId = u.OperationZoneId
        //                              }).FirstOrDefault();

        //                if (OperationZone.OperationZoneId == result.UserOperationZoneId)
        //                {
        //                    return result;
        //                }
        //                else
        //                {
        //                    return null;
        //                }
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public RecoveryEmail SendRecoveryEmail(RecoveryEmail recoveryEmail)
        {
            return recoveryEmail;
        }

        //User login Table is removed now // Prakash will delete it later
        //public FrayteLogin GetLoginByEmail(RecoveryEmail recoveryEmail)
        //{
        //    var result = (from ul in dbContext.UserLogins
        //                  join u in dbContext.Users on ul.UserId equals u.UserId
        //                  join ur in dbContext.UserRoles on ul.UserId equals ur.UserId
        //                  where u.Email == recoveryEmail.Email
        //                  select new FrayteLogin()
        //                  {
        //                      Name = u.ContactName,
        //                      UserName = ul.UserName,
        //                      Password = ul.Password
        //                  }).FirstOrDefault();

        //    return result;
        //}

        //User login Table is removed now // Prakash will delete it later
        //public FrayteResult ChangePassword(FrayteChangePassword changePasswordDetail)
        //{
        //    try
        //    {
        //        var userLogin = dbContext.UserLogins.Where(p => p.UserName == changePasswordDetail.UserName && p.Password == changePasswordDetail.CurrentPassword).FirstOrDefault();

        //        if (userLogin != null)
        //        {
        //            userLogin.Password = changePasswordDetail.NewPassword;
        //            dbContext.SaveChanges();
        //            return new FrayteResult() { Status = true };
        //        }
        //        else
        //        {
        //            return new FrayteResult() { Status = false };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}

        //User login Table is removed now // Prakash will delete it later
        //public LoginDetail ChangeFirstPassword(FrayteChangeFirstPassword changePasswordDetail)
        //{
        //    LoginDetail loginDetail;
        //    var userLogin = dbContext.UserLogins.Where(p => p.UserId == changePasswordDetail.UserId && p.Password == changePasswordDetail.CurrentPassword).FirstOrDefault();
        //    FrayteLogin userCrednetial = new FrayteLogin();


        //    var result = new FrayteResult();
        //    if (userLogin != null)
        //    {
        //        userLogin.Password = changePasswordDetail.NewPassword;
        //        userLogin.LastLoginDate = DateTime.UtcNow;
        //        userLogin.LastPasswordChangeDate = DateTime.UtcNow;
        //        dbContext.SaveChanges();
        //        result.Status = true;


        //    }
        //    else
        //    {
        //        result.Status = false;
        //    }
        //    if (result.Status)
        //    {
        //        userCrednetial.UserName = userLogin.UserName;
        //        userCrednetial.Password = userLogin.Password;
        //        loginDetail = Login(userCrednetial);
        //        return loginDetail;
        //    }
        //    return null;
        //}

        public FrayteUserTabStatus GetUserTabs(int userId, int roleId, string moduleType)
        {
            FrayteUserTabStatus userTab = new FrayteUserTabStatus();
            userTab.tabs = new List<FrayteTab>();

            var userDetail = dbContext.UserAdditionals.Where(p => p.UserId == userId).FirstOrDefault();
            var OperationZone = UtilityRepository.GetOperationZone();
            var OperationZoneDetail = dbContext.OperationZones.Find(OperationZone.OperationZoneId);
            var customerAdditional = dbContext.UserAdditionals.Where(p => p.UserId == userId).FirstOrDefault();

            var tabs = (from rm in dbContext.RoleModules
                        join ml in dbContext.ModuleLevels on rm.ModuleLevelId equals ml.ModuleLevelId
                        join mld in dbContext.ModuleLevelDetails on rm.ModuleLevelDetailId equals mld.ModuleLevelDetailId into tempMLDt
                        from tempMLD in tempMLDt.DefaultIfEmpty()
                        where rm.RoleId == roleId && rm.IsActive == true && ml.OperationZoneId == OperationZone.OperationZoneId && ml.ModuleType == moduleType
                        select new
                        {
                            ModuleLevelId = rm.ModuleLevelId,
                            ModuleLevelDetailId = rm.ModuleLevelDetailId,
                            heading = ml.ModuleName,
                            tabKey = ml.MultilingualKey,
                            ModuleOrderNumber = ml.OrderNumber,
                            SubModuleOrderNumber = tempMLD == null ? 0 : tempMLD.OrderNumber,
                            subHeading = tempMLD.SubModuleName == null ? "" : tempMLD.SubModuleName,
                            subTabKey = tempMLD.MultilingualKey == null ? "" : tempMLD.MultilingualKey,
                            active = false,
                            route = rm.Route,
                            IsParent = tempMLD == null ? false : tempMLD.IsParent,
                            IsDefaultRoute = rm.IsDefaultRoute
                        }).ToList();



            if (roleId == (int)FrayteUserRole.Customer && customerAdditional != null)
            {
                if (customerAdditional.IsApiAllow == false)
                {
                    var reemovetab = tabs.Where(p => p.route == "loginView.userTabs.profile-setting.api-detail").FirstOrDefault();
                    if (reemovetab != null)
                    {
                        tabs.Remove(reemovetab);
                    }
                }
                if(customerAdditional.UserType == "SPECIAL")
                {
                    var reemovetab = tabs.Where(p => p.route == "loginView.userTabs.profile-setting.api-detail").FirstOrDefault();
                    if(reemovetab != null)
                    {
                        tabs.Remove(reemovetab);
                    }
                    var reemovetab1 = tabs.Where(p => p.route == "loginView.userTabs.profile-setting.service-code").FirstOrDefault();
                    if (reemovetab1 != null)
                    {
                        tabs.Remove(reemovetab1);
                    } 
                }
                if (customerAdditional.IsDirectBooking.HasValue && customerAdditional.IsDirectBooking.Value && moduleType == FrayteShipmentServiceType.DirectBooking)
                {

                    userTab.tabs = tabs.GroupBy(x => x.ModuleLevelId)
                                  .Select(group => new FrayteTab
                                  {
                                      ModuleLevelId = group.Key,
                                      heading = group.FirstOrDefault().heading,
                                      TabOrder = group.FirstOrDefault().ModuleOrderNumber.HasValue ? group.FirstOrDefault().ModuleOrderNumber.Value : 100,
                                      route = group.FirstOrDefault().ModuleLevelDetailId == 0 ? group.FirstOrDefault().route : (group.Where(p => p.IsDefaultRoute == true).FirstOrDefault() == null ? group.FirstOrDefault().route : group.Where(p => p.IsDefaultRoute == true).FirstOrDefault().route),
                                      tabKey = group.FirstOrDefault().tabKey,
                                      active = false,
                                      childTabs = group.FirstOrDefault().ModuleLevelDetailId == 0 ? null : group.Where(p => p.IsParent == false)
                                      .Select(subGroup => new ChildTab
                                      {
                                          heading = subGroup.subHeading,
                                          TabOrder = subGroup.SubModuleOrderNumber.HasValue ? subGroup.SubModuleOrderNumber.Value : 100,
                                          route = subGroup.route,
                                          tabKey = subGroup.subTabKey,
                                          IsDefaultRoute = subGroup.IsDefaultRoute
                                      }).ToList()
                                  }).ToList();

                }
                else
                {
                    userTab.tabs = tabs.GroupBy(x => x.ModuleLevelId)
                            .Select(group => new FrayteTab
                            {
                                ModuleLevelId = group.Key,
                                heading = group.FirstOrDefault().heading,
                                TabOrder = group.FirstOrDefault().ModuleOrderNumber.HasValue ? group.FirstOrDefault().ModuleOrderNumber.Value : 100,
                                route = group.FirstOrDefault().ModuleLevelDetailId == 0 ? group.FirstOrDefault().route : (group.Where(p => p.IsDefaultRoute == true).FirstOrDefault() == null ? group.FirstOrDefault().route : group.Where(p => p.IsDefaultRoute == true).FirstOrDefault().route),
                                tabKey = group.FirstOrDefault().tabKey,
                                active = false,
                                childTabs = group.FirstOrDefault().ModuleLevelDetailId == 0 ? null : group.Where(p => p.IsParent == false)
                                .Select(subGroup => new ChildTab
                                {
                                    heading = subGroup.subHeading,
                                    TabOrder = subGroup.SubModuleOrderNumber.HasValue ? subGroup.SubModuleOrderNumber.Value : 100,
                                    route = subGroup.route,
                                    tabKey = subGroup.subTabKey,
                                    IsDefaultRoute = subGroup.IsDefaultRoute
                                }).ToList()
                            })
                            .ToList();
                }

            }
            else
            {
                userTab.tabs = tabs.GroupBy(x => x.ModuleLevelId)
                            .Select(group => new FrayteTab
                            {
                                ModuleLevelId = group.Key,
                                heading = group.FirstOrDefault().heading,
                                TabOrder = group.FirstOrDefault().ModuleOrderNumber.HasValue ? group.FirstOrDefault().ModuleOrderNumber.Value : 100,
                                route = group.FirstOrDefault().ModuleLevelDetailId == 0 ? group.FirstOrDefault().route : (group.Where(p => p.IsDefaultRoute == true).FirstOrDefault() == null ? group.FirstOrDefault().route : group.Where(p => p.IsDefaultRoute == true).FirstOrDefault().route),
                                tabKey = group.FirstOrDefault().tabKey,
                                active = false,
                                childTabs = group.FirstOrDefault().ModuleLevelDetailId == 0 ? null : group.Where(p => p.IsParent == false)
                                .Select(subGroup => new ChildTab
                                {
                                    heading = subGroup.subHeading,
                                    TabOrder = subGroup.SubModuleOrderNumber.HasValue ? subGroup.SubModuleOrderNumber.Value : 100,
                                    route = subGroup.route,
                                    tabKey = subGroup.subTabKey,
                                    IsDefaultRoute = subGroup.IsDefaultRoute
                                }).ToList()
                            })
                            .ToList();
            }

            return userTab;
        }
    }
}
