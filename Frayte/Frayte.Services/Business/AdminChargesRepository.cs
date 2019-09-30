using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;

namespace Frayte.Services.Business
{
    public class AdminChargesRepository
    {
        FrayteEntities dbcontext = new FrayteEntities();
        public List<AdminChargesTypes> GetAdminCharges(int userId)
        {

            List<AdminChargesTypes> adminCharges = new List<AdminChargesTypes>();
            try
            {
                AdminChargesTypes adminCharge;
                var collection = (from r in dbcontext.AdminCharges
                                  where r.CustomerId == 0 && r.IsActive == true
                                  select r
                                   ).ToList();
                foreach (var item in collection)
                {
                    adminCharge = new AdminChargesTypes();
                    adminCharge.AdminChargeId = item.AdminChargesId;
                    adminCharge.CreatedBy = item.CreatedBy;
                    adminCharge.ChargeType = item.ChargeType;
                    adminCharge.Amount = item.Value;
                    adminCharge.Key = item.ShortName;
                    adminCharge.Value = item.Name;
                    adminCharge.CurrencyCode = item.CurrencyCode;
                    adminCharge.CreatedOn = item.CreatedOnUtc;
                    adminCharges.Add(adminCharge);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return adminCharges;
        }

        public FrayteResult SaveCustomerCharge(FrayteCustomerSpecificAdminCharges charge)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                AdminCharge adminCharge;
                foreach (var item in charge.Charges)
                {
                    if (item.AdminChargeId == 0)
                    {
                        adminCharge = new AdminCharge();
                        adminCharge.Value = item.Amount;
                        adminCharge.CustomerId = charge.CustomerId;
                        adminCharge.CreatedOnUtc = DateTime.UtcNow;
                        adminCharge.IsActive = true;
                        adminCharge.Name = item.Value;
                        adminCharge.ShortName = item.Key;
                        adminCharge.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
                        adminCharge.ChargeType = item.ChargeType;
                        adminCharge.CreatedBy = item.CreatedBy;
                        adminCharge.CurrencyCode = "GBP";// item.CurrencyCode;
                        dbcontext.AdminCharges.Add(adminCharge);
                        dbcontext.SaveChanges();
                    }
                    else
                    {
                        adminCharge = dbcontext.AdminCharges.Find(item.AdminChargeId);
                        adminCharge.Value = item.Amount;
                        adminCharge.UpdatedOnUtc = DateTime.UtcNow;
                        adminCharge.UpdatedBy = item.CreatedBy;
                        dbcontext.SaveChanges();
                    }
                }
                result.Status = true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                result.Status = false;
            }
            return result;
        }

        public FrayteResult RemoveCustomerAdminCharges(int customerId, int userId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var collection = dbcontext.AdminCharges.Where(p => p.CustomerId == customerId).ToList();
                foreach (var item in collection)
                {
                    item.UpdatedBy = userId;
                    item.UpdatedOnUtc = DateTime.UtcNow;
                    item.IsActive = false;
                    dbcontext.SaveChanges();
                }

                result.Status = true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                result.Status = false;
            }
            return result;
        }

        public FrayteCustomerSpecificAdminCharges GetCustomerAdminCharges(int customerId)
        {
            try
            {
                FrayteCustomerSpecificAdminCharges charge = new FrayteCustomerSpecificAdminCharges();
                charge.Charges = new List<AdminChargesTypes>();
                var data = (from r in dbcontext.AdminCharges
                            join u in dbcontext.Users on r.CustomerId equals u.UserId
                            join ua in dbcontext.UserAdditionals on r.CustomerId equals ua.UserId
                            where r.CustomerId == customerId && r.IsActive == true && r.OperationZoneId == UtilityRepository.GetOperationZone().OperationZoneId
                            select new
                            {
                                CustomerName = u.CompanyName,
                                CustomerId = r.CustomerId,
                                AdminChargeId = r.AdminChargesId,
                                CreatedBy = r.CreatedBy,
                                ChargeType = r.ChargeType,
                                Key = r.ShortName,
                                Value = r.Name,
                                Amount = r.Value,
                                CreatedOn = r.CreatedOnUtc
                            }).ToList();
                charge = data.GroupBy(x => x.CustomerId)
                                                .Select(group => new FrayteCustomerSpecificAdminCharges
                                                {
                                                    CustomerId = group.FirstOrDefault().CustomerId,
                                                    CustomerName = group.FirstOrDefault().CustomerName,
                                                    Charges = group.Select(subGroup => new AdminChargesTypes
                                                    {
                                                        AdminChargeId = subGroup.AdminChargeId,
                                                        Amount = subGroup.Amount,
                                                        ChargeType = subGroup.ChargeType,
                                                        CreatedBy = subGroup.CreatedBy,
                                                        CreatedOn = subGroup.CreatedOn,
                                                        Key = subGroup.Key,
                                                        Value = subGroup.Value

                                                    }).ToList()
                                                }).FirstOrDefault();
                return charge;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        public List<FrayteCustomerSpecificAdminCharges> GetCustomerSpecificAdminCharges()
        {
            List<FrayteCustomerSpecificAdminCharges> list = new List<FrayteCustomerSpecificAdminCharges>();
            try
            {
                var OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;

                var charges = (from r in dbcontext.AdminCharges
                               join u in dbcontext.Users on r.CustomerId equals u.UserId
                               join ua in dbcontext.UserAdditionals on r.CustomerId equals ua.UserId
                               where r.IsActive == true && r.OperationZoneId == OperationZoneId
                               select new
                               {
                                   CustomerName = u.CompanyName,
                                   CustomerId = r.CustomerId,
                                   AdminChargeId = r.AdminChargesId,
                                   CreatedBy = r.CreatedBy,
                                   ChargeType = r.ChargeType,
                                   Key = r.ShortName,
                                   Value = r.Name,
                                   Amount = r.Value,
                                   CurrencyCode = r.CurrencyCode,
                                   CreatedOn = r.CreatedOnUtc
                               }
           ).ToList();
                list = charges.GroupBy(x => x.CustomerId)
                                            .Select(group => new FrayteCustomerSpecificAdminCharges
                                            {
                                                CustomerId = group.FirstOrDefault().CustomerId,
                                                CustomerName = group.FirstOrDefault().CustomerName,
                                                Charges = group.Select(subGroup => new AdminChargesTypes
                                                {
                                                    AdminChargeId = subGroup.AdminChargeId,
                                                    Amount = subGroup.Amount,
                                                    ChargeType = subGroup.ChargeType,
                                                    CreatedBy = subGroup.CreatedBy,
                                                    CreatedOn = subGroup.CreatedOn,
                                                    Key = subGroup.Key,
                                                    CurrencyCode = subGroup.CurrencyCode,
                                                    Value = subGroup.Value
                                                }).ToList()
                                            }).ToList();
 
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return list;
        }
        public List<DirectBookingCustomer> GetCustomersWithoutCharges(int userId, string moduleType, string mode)
        {

            var customerIds = dbcontext.AdminCharges.Where(p => p.CustomerId > 0 && p.IsActive == true).Select(p => p.CustomerId).Distinct().ToList();

            // To Do : customer should come according to moduleType
            var operationzone = UtilityRepository.GetOperationZone();
            List<DirectBookingCustomer> customers = new List<DirectBookingCustomer>();
            if (mode == "Add")
            {
                customers = (from r in dbcontext.Users
                             join ua in dbcontext.UserAdditionals on r.UserId equals ua.UserId
                             join ur in dbcontext.UserRoles on r.UserId equals ur.UserId
                             join CM in dbcontext.CustomerMarginCosts on r.UserId equals CM.CustomerId
                             where
                                ur.RoleId == (int)FrayteUserRole.Customer &&
                                r.IsActive == true &&
                                r.OperationZoneId == operationzone.OperationZoneId
                                && !customerIds.Contains(r.UserId)
                             select new DirectBookingCustomer
                             {
                                 CustomerId = r.UserId,
                                 CustomerName = r.ContactName,
                                 CompanyName = r.CompanyName,
                                 AccountNumber = ua.AccountNo,
                                 EmailId = r.Email,
                                 ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                 CustomerCurrency = ua.CreditLimitCurrencyCode,
                                 OperationZoneId = r.OperationZoneId
                             }).Distinct().ToList();


                //      customers.Where(p => !customerIds.Contains(p.CustomerId)).ToList();

            }
            else
            {
                customers = (from r in dbcontext.Users
                             join ua in dbcontext.UserAdditionals on r.UserId equals ua.UserId
                             join ur in dbcontext.UserRoles on r.UserId equals ur.UserId
                             join CM in dbcontext.CustomerMarginCosts on r.UserId equals CM.CustomerId
                             where
                                ur.RoleId == (int)FrayteUserRole.Customer &&
                                r.IsActive == true &&
                                r.OperationZoneId == operationzone.OperationZoneId
                             select new DirectBookingCustomer
                             {
                                 CustomerId = r.UserId,
                                 CustomerName = r.ContactName,
                                 CompanyName = r.CompanyName,
                                 AccountNumber = ua.AccountNo,
                                 EmailId = r.Email,
                                 ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                 CustomerCurrency = ua.CreditLimitCurrencyCode,
                                 OperationZoneId = r.OperationZoneId
                             }).Distinct().ToList();
            }

            return customers;
        }

        public FrayteResult DeleteAdminCharge(int adminChargeId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var data = dbcontext.AdminCharges.Find(adminChargeId);
                data.IsActive = false;
                dbcontext.SaveChanges();
                result.Status = true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                result.Status = false;
            }
            return result;
        }

        public List<AdminChargesTypes> CreateCharges(List<AdminChargesTypes> charges)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                AdminCharge adminCharge;
                foreach (var item in charges)
                {
                    if (item.AdminChargeId == 0)
                    {
                        adminCharge = new AdminCharge();
                        adminCharge.ChargeType = item.ChargeType;
                        adminCharge.CreatedBy = item.CreatedBy;
                        adminCharge.CreatedOnUtc = DateTime.UtcNow;
                        adminCharge.CustomerId = 0;
                        adminCharge.Name = item.Value;
                        adminCharge.ShortName = item.Key;
                        adminCharge.Value = item.Amount;
                        adminCharge.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
                        adminCharge.IsActive = true;
                        adminCharge.CurrencyCode = "GBP";// item.CurrencyCode;
                                                         //dbcontext.AdminCharges.Attach(adminCharge);
                        dbcontext.AdminCharges.Add(adminCharge);
                        dbcontext.SaveChanges();
                        item.AdminChargeId = adminCharge.AdminChargesId;

                        var collection = GetCustomerSpecificAdminCharges();

                        if (collection.Count > 0)
                        {
                            foreach (var ite in collection)
                            {
                                AdminCharge customerAdminCharge = new AdminCharge();
                                customerAdminCharge.ChargeType = item.ChargeType;
                                customerAdminCharge.CreatedBy = item.CreatedBy;
                                customerAdminCharge.CreatedOnUtc = DateTime.UtcNow;
                                customerAdminCharge.CustomerId = ite.CustomerId;
                                customerAdminCharge.Name = item.Value;
                                customerAdminCharge.ShortName = item.Key;
                                customerAdminCharge.Value = item.Amount;
                                customerAdminCharge.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
                                customerAdminCharge.IsActive = true;
                                customerAdminCharge.CurrencyCode = "GBP";// item.CurrencyCode;
                                                                         //dbcontext.AdminCharges.Attach(adminCharge);
                                dbcontext.AdminCharges.Add(customerAdminCharge);
                                dbcontext.SaveChanges();
                            }
                        }

                    }
                    else
                    {
                        adminCharge = dbcontext.AdminCharges.Find(item.AdminChargeId);
                        if (adminCharge != null)
                        {
                            adminCharge.UpdatedBy = item.CreatedBy;
                            adminCharge.UpdatedOnUtc = DateTime.UtcNow;
                            adminCharge.Value = item.Amount;

                            dbcontext.SaveChanges();
                        }
                    }

                }
                result.Status = true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                result.Status = false;
            }
            if (result.Status)
                return charges;
            else
                return null;
        }

        public FrayteCustomerSpecificAdminCharges GetDefaultCustomerAdminCharges(int customerId)
        {
            try
            {
                var OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
                FrayteCustomerSpecificAdminCharges charge = new FrayteCustomerSpecificAdminCharges();
                charge.Charges = new List<AdminChargesTypes>();
                var data = (from r in dbcontext.AdminCharges
                            join u in dbcontext.Users on r.CustomerId equals u.UserId
                            join ua in dbcontext.UserAdditionals on r.CustomerId equals ua.UserId
                            where r.CustomerId == customerId && r.IsActive == true && r.OperationZoneId == OperationZoneId
                            select new
                            {
                                CustomerName = u.CompanyName,
                                CustomerId = r.CustomerId,
                                AdminChargeId = r.AdminChargesId,
                                CreatedBy = r.CreatedBy,
                                ChargeType = r.ChargeType,
                                Key = r.ShortName,
                                Value = r.Name,
                                Amount = r.Value,
                                Currency = r.CurrencyCode,
                                CreatedOn = r.CreatedOnUtc
                            }).ToList();
                if (data == null || (data != null && data.Count == 0))
                {
                    data = (from r in dbcontext.AdminCharges
                            where r.CustomerId == 0 && r.IsActive == true && r.OperationZoneId == OperationZoneId
                            select new
                            {
                                CustomerName = "",
                                CustomerId = r.CustomerId,
                                AdminChargeId = r.AdminChargesId,
                                CreatedBy = r.CreatedBy,
                                ChargeType = r.ChargeType,
                                Key = r.ShortName,
                                Value = r.Name,
                                Amount = r.Value,
                                Currency = r.CurrencyCode,
                                CreatedOn = r.CreatedOnUtc
                            }).ToList();
                }

                charge = data.GroupBy(x => x.CustomerId)
                                                .Select(group => new FrayteCustomerSpecificAdminCharges
                                                {
                                                    CustomerId = group.FirstOrDefault().CustomerId,
                                                    CustomerName = group.FirstOrDefault().CustomerName,
                                                    Charges = group.Select(subGroup => new AdminChargesTypes
                                                    {
                                                        AdminChargeId = subGroup.AdminChargeId,
                                                        Amount = subGroup.Amount,
                                                        ChargeType = subGroup.ChargeType,
                                                        CreatedBy = subGroup.CreatedBy,
                                                        CreatedOn = subGroup.CreatedOn,
                                                        Key = subGroup.Key,
                                                        CurrencyCode = subGroup.Currency,
                                                        Value = subGroup.Value

                                                    }).ToList()
                                                }).FirstOrDefault();
                return charge;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }



    }
}
