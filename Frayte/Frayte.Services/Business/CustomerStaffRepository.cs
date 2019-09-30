using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;
using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Utility;

namespace Frayte.Services.Business
{
    public class CustomerStaffRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<FrayteCustomerStaff> GetCustomerStaff(FrayteCustomerStaffTrack track)
        {
            List<FrayteCustomerStaff> staff = new List<FrayteCustomerStaff>();

            FrayteOperationZone OperationZone = UtilityRepository.GetOperationZone();

            int SkipRows = 0;
            SkipRows = (track.CurrentPage - 1) * track.TakeRows;

            if (track.RoleId == (int)FrayteUserRole.Admin || track.RoleId == (int)FrayteUserRole.Staff)
            {
                staff = (from u in dbContext.Users
                         join uaa in dbContext.UserAdditionals on u.UserId equals uaa.UserId
                         join ur in dbContext.UserRoles on u.UserId equals ur.UserId
                         join ua in dbContext.UserAddresses on ur.UserId equals ua.UserId
                         join c in dbContext.Countries on ua.CountryId equals c.CountryId
                         where u.IsActive == true &&
                               u.OperationZoneId == OperationZone.OperationZoneId &&
                               ur.RoleId == (int)FrayteUserRole.CustomerStaff
                         select new FrayteCustomerStaff
                         {
                             UserId = u.UserId,
                             ContactName = u.ContactName,
                             Email = u.Email,
                             PhoneNo = u.PhoneNumber,
                             TotalRows = 0,
                             Country = new FrayteCountryCode()
                             {
                                 CountryId = c.CountryId,
                                 Name = c.CountryName,
                                 Code = c.CountryCode,
                                 Code2 = c.CountryCode2,
                                 CountryPhoneCode = c.CountryPhoneCode,
                             }
                         }).ToList();

                int total = staff.Count();

                staff = staff.OrderBy(p => p.UserId).Skip(SkipRows).Take(track.TakeRows).ToList();

                staff.ForEach(p => p.TotalRows = total);
            }
            else if (track.RoleId == (int)FrayteUserRole.Customer)
            {
                staff = (from u in dbContext.Users
                         join ur in dbContext.UserRoles on u.UserId equals ur.UserId
                         join ua in dbContext.UserAddresses on ur.UserId equals ua.UserId
                         join c in dbContext.Countries on ua.CountryId equals c.CountryId
                         join cs in dbContext.CustomerStaffs on u.UserId equals cs.CustomerStaffId
                         where u.IsActive == true &&
                               u.OperationZoneId == OperationZone.OperationZoneId &&
                               ur.RoleId == (int)FrayteUserRole.CustomerStaff &&
                               cs.UserId == track.CreatedBy &&
                               cs.IsActive == true
                         select new FrayteCustomerStaff
                         {
                             UserId = u.UserId,
                             ContactName = u.ContactName,
                             Email = u.Email,
                             PhoneNo = u.PhoneNumber,
                             TotalRows = 0,
                             Country = new FrayteCountryCode()
                             {
                                 CountryId = c.CountryId,
                                 Name = c.CountryName,
                                 Code = c.CountryCode,
                                 Code2 = c.CountryCode2,
                                 CountryPhoneCode = c.CountryPhoneCode,
                             }
                         }).ToList();

                int total = staff.Count();

                staff = staff.OrderBy(p => p.UserId).Skip(SkipRows).Take(track.TakeRows).ToList();

                staff.ForEach(p => p.TotalRows = total);
            }
            return staff;
        }

        public FrayteInternalUser CustomerStaffDetail(int UserId)
        {
            FrayteInternalUser customer = new FrayteInternalUser();
            WorkingWeekDay workingDays = new WorkingWeekDay();

            var detail = dbContext.Users.Where(p => p.UserId == UserId).FirstOrDefault();
            if (detail != null)
            {
                customer = UtilityRepository.InternalUserMapping(detail);

                if (customer.WorkingWeekDay.WorkingWeekDayId > 0)
                {
                    workingDays = dbContext.WorkingWeekDays.Find(customer.WorkingWeekDay.WorkingWeekDayId);
                }

                if (workingDays != null)
                {
                    customer.WorkingWeekDay = workingDays;
                }

                var userRole = dbContext.UserRoles.Where(p => p.UserId == UserId).FirstOrDefault();
                if (userRole != null)
                {
                    customer.RoleId = userRole.RoleId;
                }

                var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == detail.TimezoneId).FirstOrDefault();
                if (timeZone != null)
                {
                    customer.Timezone = new TimeZoneModal();
                    customer.Timezone.TimezoneId = timeZone.TimezoneId;
                    customer.Timezone.Name = timeZone.Name;
                    customer.Timezone.Offset = timeZone.Offset;
                    customer.Timezone.OffsetShort = timeZone.OffsetShort;
                }

                //Step 2: Get internal user's other information
                var internalUserOtherDetails = dbContext.UserAdditionals.Where(p => p.UserId == UserId).FirstOrDefault();
                if (internalUserOtherDetails != null)
                {
                    if (internalUserOtherDetails.IsFuelSurCharge.HasValue)
                    {
                        customer.IsFuelSurCharge = internalUserOtherDetails.IsFuelSurCharge.Value;
                    }
                    if (internalUserOtherDetails.IsCurrency.HasValue)
                    {
                        customer.IsCurrency = internalUserOtherDetails.IsCurrency.Value;
                    }

                    customer.ManagerUser = new FrayteCustomerAssociatedUser();
                    customer.ManagerUser = null;

                    //Get associated customer's detail
                    GetAssociateCustomeDetail(customer);
                }

                //Step 3: Get user address
                var address = dbContext.UserAddresses.Where(p => p.UserId == UserId).FirstOrDefault();
                if (address != null)
                {
                    customer.UserAddress = new FrayteAddress();
                    customer.UserAddress.Address = address.Address;
                    customer.UserAddress.Address2 = address.Address2;
                    customer.UserAddress.Address3 = address.Address3;
                    customer.UserAddress.AddressTypeId = address.AddressTypeId;
                    customer.UserAddress.City = address.City;
                    customer.UserAddress.EasyPostAddressId = address.EasyPostAddressId;
                    customer.UserAddress.State = address.State;
                    customer.UserAddress.Suburb = address.Suburb;
                    customer.UserAddress.UserAddressId = address.UserAddressId;
                    customer.UserAddress.UserId = address.UserId;
                    customer.UserAddress.Zip = address.Zip;
                    customer.UserAddress.Country = new FrayteCountryCode();
                    {
                        var country = dbContext.Countries.Where(p => p.CountryId == address.CountryId).FirstOrDefault();
                        if (country != null)
                        {
                            customer.UserAddress.Country.CountryId = country.CountryId;
                            customer.UserAddress.Country.Code = country.CountryCode;
                            customer.UserAddress.Country.Code2 = country.CountryCode2;
                            customer.UserAddress.Country.Name = country.CountryName;
                            customer.UserAddress.Country.CountryPhoneCode = country.CountryPhoneCode;
                        }
                    }
                }
            }

            return customer;
        }

        public void GetAssociateCustomeDetail(FrayteInternalUser customer)
        {
            customer.AssociateCustomer = new List<FrayteAssociateCustomer>();
            FrayteAssociateCustomer user;

            var associatedetail = (from cs in dbContext.CustomerStaffs
                                   join u in dbContext.Users on cs.UserId equals u.UserId
                                   join ua in dbContext.UserAddresses on u.UserId equals ua.UserId
                                   join c in dbContext.Countries on ua.CountryId equals c.CountryId
                                   where cs.CustomerStaffId == customer.UserId &&
                                         cs.IsActive == true &&
                                         u.IsActive == true
                                   select new
                                   {
                                       CustomerStaffDetailId = cs.CustomerStaffDetailId,
                                       UserId = u.UserId,
                                       ContactName = u.ContactName,
                                       Email = u.UserEmail,
                                       PhoneCode = (c == null || c.CountryPhoneCode == "") ? "" : c.CountryPhoneCode,
                                       TelephoneNo = u.TelephoneNo,
                                       WorkingStartTime = u.WorkingStartTime,
                                       WorkingEndTime = u.WorkingEndTime
                                   }).ToList();

            if (associatedetail != null && associatedetail.Count > 0)
            {
                foreach (var dd in associatedetail)
                {
                    user = new FrayteAssociateCustomer();
                    user.CustomerId = dd.UserId;
                    user.CustomerStaffDetailId = dd.CustomerStaffDetailId;
                    user.CustomerName = dd.ContactName;
                    user.Email = dd.Email;
                    user.ContactNo = dd.PhoneCode == "" ? dd.TelephoneNo : "(+" + dd.PhoneCode + ") " + dd.TelephoneNo;
                    user.WorkingHours = UtilityRepository.GetWorkingHours(dd.WorkingStartTime, dd.WorkingEndTime);
                    customer.AssociateCustomer.Add(user);
                }
            }
        }

        public FrayteResult RemoveAssociateCustomer(int CustomerStaffDetailId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var detail = dbContext.CustomerStaffs.Where(p => p.CustomerStaffDetailId == CustomerStaffDetailId).FirstOrDefault();
                if (detail != null)
                {
                    detail.IsActive = false;
                    dbContext.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    result.Status = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Status = false;
                return result;
            }
        }

        public FrayteResult RemoveCustomerStaff(int UserId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var detail = dbContext.Users.Where(p => p.UserId == UserId).FirstOrDefault();
                if (detail != null)
                {
                    detail.IsActive = false;
                    dbContext.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    result.Status = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Status = false;
                return result;
            }
        }

        public List<FrayteCustomerAssociatedUser> GetCustomerDetail(string Name, int RoleId)
        {
            List<FrayteCustomerAssociatedUser> customer = new List<FrayteCustomerAssociatedUser>();

            // To Do : customer should come according to moduleType
            var operationzone = UtilityRepository.GetOperationZone();

            if (RoleId == (int)FrayteUserRole.Admin || RoleId == (int)FrayteUserRole.Staff)
            {
                var findUser = (from u in dbContext.Users.Where(p => p.ContactName.Contains(Name))
                                join ur in dbContext.UserRoles on u.UserId equals ur.UserId
                                where u.IsActive == true &&
                                      u.OperationZoneId == operationzone.OperationZoneId &&
                                      ur.RoleId == (int)FrayteUserRole.Customer
                                select u).ToList();

                if (findUser != null && findUser.Count > 0)
                {
                    FrayteCustomerAssociatedUser associateUser;
                    foreach (var uu in findUser)
                    {
                        var phonecode = (from ua in dbContext.UserAddresses
                                         join c in dbContext.Countries on ua.CountryId equals c.CountryId
                                         where ua.UserId == uu.UserId
                                         select new
                                         {
                                             PhoneCode = c.CountryPhoneCode
                                         }).FirstOrDefault();

                        associateUser = new FrayteCustomerAssociatedUser();
                        associateUser.UserId = uu.UserId;
                        associateUser.ContactName = uu.ContactName;
                        associateUser.Email = uu.UserEmail;
                        associateUser.TelephoneNo = (phonecode == null || phonecode.PhoneCode == "") ? uu.TelephoneNo : "(+" + phonecode.PhoneCode + ") " + uu.TelephoneNo;
                        associateUser.WorkingHours = UtilityRepository.GetWorkingHours(uu.WorkingStartTime, uu.WorkingEndTime);
                        customer.Add(associateUser);
                    }
                }
            }

            return customer;
        }
    }
}
