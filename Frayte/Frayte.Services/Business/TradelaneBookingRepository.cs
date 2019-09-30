using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;
using Frayte.Services.Models.Tradelane;
using Frayte.Services.DataAccess;
using System.Text.RegularExpressions;
using System.Data;
using Frayte.Services.Utility;
using RazorEngine.Templating;
using System.IO;
using System.Web;
using Frayte.Services.Models.BreakBulk;
using Frayte.Services.Models.Express;

namespace Frayte.Services.Business
{
    public class TradelaneBookingRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        #region  Shipment Initials 

        public List<TradelaneCustomer> GetCustomers(int userId, string moduleType)
        {
            var operationzone = UtilityRepository.GetOperationZone();

            List<TradelaneCustomer> customers;

            if (new TradelaneBookingRepository().GetUserRole(userId) == (int)FrayteUserRole.Shipper)
            {
                customers = (from r in dbContext.Users
                             join sc in dbContext.TradelaneShipperCustomers on r.UserId equals sc.CustomerId
                             join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                             join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                             where
                                ur.RoleId == (int)FrayteUserRole.Customer &&
                                ua.IsTradelaneBooking == true &&
                                r.IsActive == true &&
                                r.OperationZoneId == operationzone.OperationZoneId
                             select new TradelaneCustomer
                             {
                                 CustomerId = r.UserId,
                                 CustomerName = r.ContactName,
                                 CompanyName = r.CompanyName,
                                 AccountNumber = ua.AccountNo,
                                 IsShipperPayTaxAndDuty = ua.IsShipperTaxAndDuty.HasValue ? ua.IsShipperTaxAndDuty.Value : false,
                                 EmailId = r.UserEmail,
                                 ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                 CustomerCurrency = ua.CreditLimitCurrencyCode,
                                 OperationZoneId = r.OperationZoneId
                             }).Distinct().OrderBy(p => p.CompanyName).ToList();

                for (int i = 0; i < customers.Count; i++)
                {
                    customers[i].OrderNumber = i + 1;
                }

                var customer = new TradelaneCustomer();
                customer.CustomerId = 0;
                customer.AccountNumber = "xxx-xxx-xxx";
                customer.CompanyName = "Add new customer";
                customer.EmailId = "";
                customer.OperationZoneId = operationzone.OperationZoneId;
                customer.ValidDays = 0;
                customer.CustomerCurrency = "";
                customer.OrderNumber = customers.Count + 1;

                customers.Add(customer);

            }
            else
            {
                customers = (from r in dbContext.Users
                             join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                             join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                             where
                                ur.RoleId == (int)FrayteUserRole.Customer &&
                                ua.IsTradelaneBooking == true &&
                                r.IsActive == true &&
                                r.OperationZoneId == operationzone.OperationZoneId
                             select new TradelaneCustomer
                             {
                                 CustomerId = r.UserId,
                                 CustomerName = r.ContactName,
                                 CompanyName = r.CompanyName,
                                 AccountNumber = ua.AccountNo,
                                 IsShipperPayTaxAndDuty = ua.IsShipperTaxAndDuty.HasValue ? ua.IsShipperTaxAndDuty.Value : false,
                                 EmailId = r.UserEmail,
                                 ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                 CustomerCurrency = ua.CreditLimitCurrencyCode,
                                 OperationZoneId = r.OperationZoneId
                             }).Distinct().OrderBy(p => p.CompanyName).ToList();

                for (int i = 0; i < customers.Count; i++)
                {
                    customers[i].OrderNumber = i + 1;
                }

            }

            return customers;
        }

        public string GetFileDocumentType(int shipmentId, string filename)
        {
            var db = dbContext.TradelaneShipmentDocuments.Where(p => p.TradelaneShipmentId == shipmentId && p.DocumentName == filename).FirstOrDefault();
            if (db != null)
            {
                return db.DocumentType;
            }
            else
            {
                return "";
            }
        }

        public TradelaneCustomer GetCustomerDetail(int userId)
        {
            var userRole = dbContext.UserRoles.Where(p => p.UserId == userId).FirstOrDefault();

            if (userRole.RoleId == (int)FrayteUserRole.Customer)
            {
                var operationzone = UtilityRepository.GetOperationZone();
                var customer = (from r in dbContext.Users
                                join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                                join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                                where r.UserId == userId
                                select new TradelaneCustomer
                                {
                                    CustomerId = r.UserId,
                                    CustomerName = r.ContactName,
                                    CompanyName = r.CompanyName,
                                    AccountNumber = ua.AccountNo,
                                    IsShipperPayTaxAndDuty = ua.IsShipperTaxAndDuty.HasValue ? ua.IsShipperTaxAndDuty.Value : false,
                                    EmailId = r.Email,
                                    ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                    CustomerCurrency = ua.CreditLimitCurrencyCode,
                                    OperationZoneId = r.OperationZoneId
                                }).FirstOrDefault();
                return customer;
            }
            return null;
        }

        public TradelaneCustomerDefaultAddress UserDefaultAddresses(int userId)
        {
            TradelaneCustomerDefaultAddress address = new TradelaneCustomerDefaultAddress();

            address.ShipFrom = (from U in dbContext.AddressBooks
                                join Add in dbContext.UserAdditionals on U.CustomerId equals Add.UserId
                                where U.CustomerId == userId && U.IsActive == true && U.FromAddress == true && U.IsDefault == true
                                select new TradelBookingAdress
                                {
                                    CustomerId = userId,
                                    CompanyName = U.CompanyName,
                                    FirstName = U.ContactFirstName,
                                    LastName = U.ContactLastName,
                                    Phone = U.PhoneNo,
                                    Email = U.Email,
                                    Address = U.Address1,
                                    Address2 = U.Address2,
                                    Area = U.Area,
                                    City = U.City,
                                    State = U.State,
                                    PostCode = U.Zip,
                                    Country = new FrayteCountryCode() { CountryId = U.CountryId },
                                    IsDefault = true
                                }
                                ).FirstOrDefault();

            if (address.ShipFrom != null)
            {
                address.ShipFrom.Phone = Regex.Replace(address.ShipFrom.Phone, "(\\(.*\\))", "").Trim();
                //Step : Get country information
                var country = dbContext.Countries.Where(p => p.CountryId == address.ShipFrom.Country.CountryId).FirstOrDefault();
                if (country != null)
                {
                    address.ShipFrom.Country.CountryId = country.CountryId;
                    address.ShipFrom.Country.Code = country.CountryCode;
                    address.ShipFrom.Country.Code2 = country.CountryCode2;
                    address.ShipFrom.Country.Name = country.CountryName;
                }
            }

            address.ShipTo = (from U in dbContext.AddressBooks
                              join Add in dbContext.UserAdditionals on U.CustomerId equals Add.UserId
                              where U.CustomerId == userId && U.IsActive == true && U.ToAddress == true && U.IsDefault == true
                              select new TradelBookingAdress
                              {
                                  CustomerId = userId,
                                  CompanyName = U.CompanyName,
                                  FirstName = U.ContactFirstName,
                                  LastName = U.ContactLastName,
                                  Phone = U.PhoneNo,
                                  Email = U.Email,
                                  Address = U.Address1,
                                  Address2 = U.Address2,
                                  Area = U.Area,
                                  City = U.City,
                                  State = U.State,
                                  PostCode = U.Zip,
                                  Country = new FrayteCountryCode() { CountryId = U.CountryId },
                                  IsDefault = true
                              }
                              ).FirstOrDefault();

            if (address.ShipTo != null)
            {
                address.ShipTo.Phone = Regex.Replace(address.ShipTo.Phone, "(\\(.*\\))", "").Trim();
                //Step : Get country information
                var country = dbContext.Countries.Where(p => p.CountryId == address.ShipTo.Country.CountryId).FirstOrDefault();
                if (country != null)
                {
                    address.ShipTo.Country.CountryId = country.CountryId;
                    address.ShipTo.Country.Code = country.CountryCode;
                    address.ShipTo.Country.Code2 = country.CountryCode2;
                    address.ShipTo.Country.Name = country.CountryName;
                }
            }

            if (address.ShipFrom == null)
            {
                address.ShipFrom = (from U in dbContext.Users
                                    join UA in dbContext.UserAddresses on U.UserId equals UA.UserId
                                    join Add in dbContext.UserAdditionals on UA.UserId equals Add.UserId
                                    where U.UserId == userId
                                    select new TradelBookingAdress()
                                    {
                                        CustomerId = userId,
                                        CompanyName = U.CompanyName,
                                        FirstName = U.ContactName,
                                        Phone = U.TelephoneNo,
                                        Email = U.Email,
                                        Address = UA.Address,
                                        Address2 = UA.Address2,
                                        Area = UA.Suburb,
                                        City = UA.City,
                                        State = UA.State,
                                        PostCode = UA.Zip,
                                        Country = new FrayteCountryCode() { CountryId = UA.CountryId },
                                        IsDefault = false
                                    }).FirstOrDefault();

                if (address.ShipFrom != null)
                {
                    address.ShipFrom.Phone = Regex.Replace(address.ShipFrom.Phone, "(\\(.*\\))", "").Trim();
                    //Step : Get country information
                    var country = dbContext.Countries.Where(p => p.CountryId == address.ShipFrom.Country.CountryId).FirstOrDefault();
                    if (country != null)
                    {
                        address.ShipFrom.Country.CountryId = country.CountryId;
                        address.ShipFrom.Country.Code = country.CountryCode;
                        address.ShipFrom.Country.Code2 = country.CountryCode2;
                        address.ShipFrom.Country.Name = country.CountryName;
                    }
                }
            }
            return address;
        }

        public bool ShipmentMAWBAsHAWB(ShipmentMAWBAsHAWBModel obj)
        {
            try
            {
                var dbShipment = dbContext.TradelaneShipments.Find(obj.TradelaneShipmentId);
                if (dbShipment != null)
                {
                    dbShipment.MAWB = obj.MAWB;
                    dbShipment.HAWBNumber = 1;
                    dbContext.SaveChanges();

                    var dbShipmentDetails = dbContext.TradelaneShipmentDetails.Where(p => p.TradelaneShipmentId == obj.TradelaneShipmentId).ToList();
                    foreach (var item in dbShipmentDetails)
                    {
                        item.HAWB = obj.MAWB;
                        item.FromAddressId = dbShipment.FromAddressId;
                        item.ToAddressId = dbShipment.ToAddressId;
                        item.IsNotifyPartySameAsReceiver = true;
                        dbContext.SaveChanges();
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public TradelBookingAdress GetCustomerAddress(int customerId)
        {
            var result = (from U in dbContext.Users
                          join UA in dbContext.UserAddresses on U.UserId equals UA.UserId
                          join Add in dbContext.UserAdditionals on UA.UserId equals Add.UserId
                          where U.UserId == customerId
                          select new TradelBookingAdress
                          {
                              CompanyName = U.CompanyName,
                              FirstName = U.ContactName,
                              Phone = U.TelephoneNo,
                              Email = U.Email,
                              Address = UA.Address,
                              Address2 = UA.Address2,
                              Area = UA.Suburb,
                              City = UA.City,
                              State = UA.State,
                              PostCode = UA.Zip,
                              Country = new FrayteCountryCode() { CountryId = UA.CountryId }
                          }).FirstOrDefault();

            if (result != null)
            {
                result.Phone = Regex.Replace(result.Phone, "(\\(.*\\))", "").Trim();
                //Step : Get country information
                var country = dbContext.Countries.Where(p => p.CountryId == result.Country.CountryId).FirstOrDefault();
                if (country != null)
                {
                    result.Country.CountryId = country.CountryId;
                    result.Country.Code = country.CountryCode;
                    result.Country.Code2 = country.CountryCode2;
                    result.Country.Name = country.CountryName;
                }
            }

            return result;
        }

        #endregion

        #region Place Booking

        public TradelaneBooking SaveShipment(TradelaneBooking shipment, string callingType)
        {
            shipment.Error = new FratyteError();
            try
            {
                // Step 1: Save Shipment To Address 
                shipment.ShipFrom.CustomerId = shipment.CustomerId;
                shipment.ShipFrom.AddressType = FrayteFromToAddressType.FromAddress;
                SaveTradelaneShipmentAddress(shipment.ShipmentStatusId, shipment.ShipFrom, FrayteFromToAddressType.FromAddress);

                // Step 2: Save Shipment To Address 
                shipment.ShipTo.CustomerId = shipment.CustomerId;
                shipment.ShipTo.AddressType = FrayteFromToAddressType.ToAddress;
                SaveTradelaneShipmentAddress(shipment.ShipmentStatusId, shipment.ShipTo, FrayteFromToAddressType.ToAddress);

                if (shipment.NotifyParty != null)
                {
                    // Step 3: Save Notify Address
                    shipment.NotifyParty.CustomerId = shipment.CustomerId;
                    if (!shipment.IsNotifyPartySameAsReceiver)
                    {
                        shipment.NotifyParty.AddressType = FrayteFromToAddressType.NotifyPartyAddress;
                        SaveTradelaneShipmentAddress(shipment.ShipmentStatusId, shipment.NotifyParty, FrayteFromToAddressType.NotifyPartyAddress);
                    }
                }

                // Step 4 : Save Shipment Detail
                saveShipmentDetail(shipment, callingType);

                // save mawb in route table
                if (shipment != null && shipment.MAWB.Length > 7 && shipment.AirlinePreference.AirlineId > 0)
                {
                    var mawb = dbContext.Airlines.Where(a => a.AirlineId == shipment.AirlinePreference.AirlineId).FirstOrDefault().AirlineCode + " " + shipment.MAWB.Substring(0, 4) + " " + shipment.MAWB.Substring(4, 4);
                    var result = dbContext.TrackingNumberRoutes.Where(a => a.Number == mawb).FirstOrDefault();
                    if (result == null)
                    {
                        TrackingNumberRoute TNR = new TrackingNumberRoute();
                        TNR.Number = dbContext.Airlines.Where(a => a.AirlineId == shipment.AirlinePreference.AirlineId).FirstOrDefault().AirlineCode + " " + shipment.MAWB.Substring(0, 4) + " " + shipment.MAWB.Substring(4, 4);
                        TNR.ShipmentId = shipment.TradelaneShipmentId;
                        TNR.ModuleType = FrayteShipmentServiceType.TradeLaneBooking;
                        TNR.IsMAWB = true;
                        dbContext.TrackingNumberRoutes.Add(TNR);
                        dbContext.SaveChanges();
                    }
                }

                //save tradelane frayte no in tracking no route table
                if (shipment != null && shipment.FrayteNumber != null && shipment.FrayteNumber.Length > 7)
                {
                    var result = dbContext.TrackingNumberRoutes.Where(a => a.Number == shipment.FrayteNumber).FirstOrDefault();
                    if (result == null)
                    {
                        TrackingNumberRoute TNR = new TrackingNumberRoute();
                        TNR.Number = shipment.FrayteNumber;
                        TNR.ShipmentId = shipment.TradelaneShipmentId;
                        TNR.ModuleType = FrayteShipmentServiceType.TradeLaneBooking;
                        TNR.IsTradelaneRefNumber = true;
                        dbContext.TrackingNumberRoutes.Add(TNR);
                        dbContext.SaveChanges();
                    }
                }

                //Step 4.1 : Save packages Details
                if (callingType == "Express")
                {
                    savePackagesDetail(shipment);
                }
                else
                {
                    if (shipment.ShipmentStatusId == 28)
                    {
                        // Step 5: Save Package Detail -> Remove the packages for which HAWB is not allocated for shipment booked
                        saveShipmentPackages(shipment);

                        // Step 6: Update HAWB number
                        saveHAWBNumber(shipment);
                    }
                }

                //Step 7: Set Shipper Customer
                if (new TradelaneBookingRepository().GetUserRole(shipment.CreatedBy) == (int)FrayteUserRole.Shipper)
                {
                    linkCustomerToShipper(shipment);
                }

                // Step 5: Save Shipment Document
                shipment.Error.Status = false;
            }
            catch (Exception ex)
            {
                shipment.Error.Status = true;
            }

            return shipment;
        }

        private void savePackagesDetail(TradelaneBooking shipment)
        {
            if (shipment.Packages != null && shipment.Packages.Count > 0)
            {
                TradelaneShipmentDetail pkg;
                foreach (var item in shipment.Packages)
                {
                    pkg = dbContext.TradelaneShipmentDetails.Find(item.TradelaneShipmentDetailId);
                    if (pkg != null)
                    {
                        pkg.CartonNumber = item.CartonNumber;
                        pkg.CartonValue = item.CartonValue;
                        pkg.HAWB = item.HAWB;
                        pkg.Height = item.Height;
                        pkg.Length = item.Length;
                        pkg.Width = item.Width;
                        pkg.Weight = item.Weight;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        pkg = new TradelaneShipmentDetail();
                        pkg.CartonNumber = item.CartonNumber;
                        pkg.TradelaneShipmentId = shipment.TradelaneShipmentId;
                        pkg.CartonValue = item.CartonValue;
                        pkg.HAWB = item.HAWB;
                        pkg.Height = item.Height;
                        pkg.Length = item.Length;
                        pkg.Width = item.Width;
                        pkg.Weight = item.Weight;
                        dbContext.TradelaneShipmentDetails.Add(pkg);
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        public void SetShipmentStatus(int shipmentId, string actionType)
        {
            var shipment = dbContext.TradelaneShipments.Find(shipmentId);
            int statusId = 0;
            if (actionType == ShipmentCustomerAction.Confirm)
            {
                statusId = (int)FrayteTradelaneShipmentStatus.ShipmentBooked;
            }
            else if (actionType == ShipmentCustomerAction.Reject)
            {
                statusId = (int)FrayteTradelaneShipmentStatus.Rejected;
            }
            if (shipment != null)
            {
                shipment.ShipmentStatusId = statusId;
                dbContext.SaveChanges();
            }
        }

        public int CustomerAction(FrayteCustomerActionShippment confirmationDetail)
        {
            var shipment = dbContext.TradelaneShipments.Where(p => p.CustomerConfirmationCode.ToString() == confirmationDetail.ConfirmationCode).FirstOrDefault();
            if (shipment != null)
            {
                return shipment.TradelaneShipmentId;
            }
            return 0;
        }

        public TradelanePackageWeightObj TrackTradelanePackages(int tradelaneShipmentId)
        {
            TradelanePackageWeightObj obj = new TradelanePackageWeightObj();

            var shipment = dbContext.TradelaneShipments.Find(tradelaneShipmentId);
            if (shipment != null)
            {
                var collection = dbContext.TradelaneShipmentDetails.Where(p => p.TradelaneShipmentId == tradelaneShipmentId).ToList();

                decimal totalWeight = 0.00M;
                decimal totalCBM = 0.00M;

                foreach (var item in collection)
                {
                    totalCBM += shipment.PackageCalculatonType == FraytePakageCalculationType.kgtoCms ? (item.Length / 100) * (item.Width / 100) * (item.Height / 100) : (item.Length / 39.37M) * (item.Width / 39.37M) * (item.Height / 39.37M);
                    totalWeight += item.Weight * item.CartonValue;
                }

                obj.TotalCBM = Math.Round(totalCBM, 2);
                obj.TotalChargeableWeight = Math.Round(totalWeight, 2);
                obj.TotalEstimatedWeight = Math.Round(totalWeight, 2);
                obj.TotalGrossWeight = Math.Round(totalWeight, 2); ;
                obj.TotalWeight = Math.Round(totalWeight, 2);

                return obj;
            }
            throw new NotImplementedException();
        }

        public FrayteResult UpdateHAWBNumber(int shipmentId, int hawbNumber)
        {
            FrayteResult result = new FrayteResult();
            var shipment = dbContext.TradelaneShipments.Find(shipmentId);
            if (shipment != null)
            {
                shipment.HAWBNumber = hawbNumber;
                dbContext.SaveChanges();
                result.Status = true;
            }
            else
            {
                result.Status = false;
            }
            return result;
        }

        private void linkCustomerToShipper(TradelaneBooking shipment)
        {
            if (dbContext.TradelaneShipperCustomers.Where(p => p.ShipperId == shipment.CreatedBy && p.CustomerId == shipment.CustomerId).FirstOrDefault() == null)
            {
                TradelaneShipperCustomer shipperCustomer = new TradelaneShipperCustomer();
                shipperCustomer.CustomerId = shipment.CustomerId;
                shipperCustomer.ShipperId = shipment.CreatedBy;
                dbContext.TradelaneShipperCustomers.Add(shipperCustomer);
                dbContext.SaveChanges();
            }
        }

        private void saveHAWBNumber(TradelaneBooking shipment)
        {
            int hawbCount = dbContext.TradelaneShipmentDetails.Where(p => p.TradelaneShipmentId == shipment.TradelaneShipmentId)
                .GroupBy(p => p.HAWB).Count();
            if (hawbCount > 0)
            {
                var tradelaneShipment = dbContext.TradelaneShipments.Find(shipment.TradelaneShipmentId);
                if (tradelaneShipment != null)
                {
                    tradelaneShipment.HAWBNumber = hawbCount;
                    dbContext.SaveChanges();
                }
            }
        }

        public FrayteResult ChangeMAWBAddress(TradelBookingAdress tradelaneAddress)
        {
            FrayteResult result = new FrayteResult();
            var shipmentAddress = dbContext.TradelaneShipmentAddresses.Find(tradelaneAddress.TradelaneShipmentAddressId);

            if (shipmentAddress != null)
            {
                shipmentAddress.CountryId = tradelaneAddress.Country.CountryId;
                shipmentAddress.CompanyName = tradelaneAddress.CompanyName;
                shipmentAddress.ContactFirstName = tradelaneAddress.FirstName;
                shipmentAddress.ContactLastName = tradelaneAddress.LastName;
                shipmentAddress.City = tradelaneAddress.City;
                shipmentAddress.State = tradelaneAddress.State;
                shipmentAddress.Zip = tradelaneAddress.PostCode;
                shipmentAddress.Address1 = tradelaneAddress.Address;
                shipmentAddress.Address2 = tradelaneAddress.Address2;
                shipmentAddress.Email = tradelaneAddress.Email;
                shipmentAddress.PhoneNo = tradelaneAddress.Phone;
                shipmentAddress.Area = tradelaneAddress.Area;

                dbContext.SaveChanges();
                result.Status = true;
            }
            return result;
        }

        public FrayteResult RemoveTradelaneDocument(int tradelaneShipmentDocumentId)
        {
            FrayteResult reult = new FrayteResult();
            var shipmentDoc = dbContext.TradelaneShipmentDocuments.Find(tradelaneShipmentDocumentId);
            if (shipmentDoc != null)
            {
                string filePath = HttpContext.Current.Server.MapPath(string.Format(FrayteTradelaneDocumentPath.ShipmentDocument, shipmentDoc.TradelaneShipmentId));
                if (File.Exists(filePath + shipmentDoc.DocumentName))
                {
                    try
                    {
                        File.Delete(filePath + shipmentDoc.DocumentName);
                        dbContext.TradelaneShipmentDocuments.Remove(shipmentDoc);
                        dbContext.SaveChanges();
                        reult.Status = true;
                    }
                    catch (Exception ex)
                    {
                        reult.Status = false;
                    }
                }
            }
            return reult;
        }

        private void saveShipmentDocument(TradelaneBooking shipment)
        {
            throw new NotImplementedException();
        }

        private void saveShipmentPackages(TradelaneBooking shipment)
        {
            var packages = dbContext.TradelaneShipmentDetails.Where(p => p.TradelaneShipmentId == shipment.TradelaneShipmentId).ToList();

            if (packages.Count > 0)
            {
                foreach (var item in packages)
                {
                    if (string.IsNullOrEmpty(item.HAWB))
                    {
                        dbContext.TradelaneShipmentDetails.Remove(item);
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        public int GetUserRole(int createdBy)
        {
            return dbContext.UserRoles.Where(p => p.UserId == createdBy).FirstOrDefault().RoleId;
        }

        public List<HAWBTradelanePackage> GetGroupedHAWBPieces(int tradelaneShipmentId)
        {
            try
            {
                List<HAWBTradelanePackage> list;

                var collection = (from r in dbContext.TradelaneShipmentDetails
                                  join s in dbContext.TradelaneShipments on r.TradelaneShipmentId equals s.TradelaneShipmentId
                                  join fc in dbContext.TradelaneShipmentAddresses on s.FromAddressId equals fc.TradelaneShipmentAddressId
                                  join c in dbContext.Countries on fc.CountryId equals c.CountryId
                                  join tc in dbContext.TradelaneShipmentAddresses on s.ToAddressId equals tc.TradelaneShipmentAddressId
                                  join cc in dbContext.Countries on tc.CountryId equals cc.CountryId
                                  join nf in dbContext.TradelaneShipmentAddresses on s.NotifyPartyAddressId equals nf.TradelaneShipmentAddressId into leftjoin
                                  from le in leftjoin.DefaultIfEmpty()
                                  join nfc in dbContext.Countries on le.CountryId equals nfc.CountryId into leftnjoin
                                  from nfe in leftnjoin.DefaultIfEmpty()
                                  select new HAWBTradelanePackage
                                  {
                                      HAWB = r.HAWB,
                                      HAWBNumber = s.HAWBNumber.HasValue ? s.HAWBNumber.Value : 0,
                                      TradelaneShipmentId = r.TradelaneShipmentId,
                                      SONumber = r.SONumber,
                                      Packages = new List<TradelanePackage>()
                                      {
                                         new TradelanePackage()
                                         {
                                             CartonNumber = r.CartonNumber,
                                             CartonValue = r.CartonValue,
                                             Length = r.Length,
                                             Width = r.Width,
                                             Height = r.Height,
                                             Weight = r.Weight,
                                             HAWB = r.HAWB,
                                             IsScanned = r.IsScaned,
                                             PackageCalculatonType = s.PackageCalculatonType
                                         }
                                      }
                                  }).Where(p => p.TradelaneShipmentId == tradelaneShipmentId && !string.IsNullOrEmpty(p.HAWB)).ToList();

                if (collection.Count > 0)
                {
                    list = collection.GroupBy(x => x.HAWB)
                                     .Select(group => new HAWBTradelanePackage
                                     {
                                         TradelaneShipmentId = group.FirstOrDefault().TradelaneShipmentId,
                                         HAWB = group.Key,
                                         SONumber = group.FirstOrDefault().SONumber,
                                         HAWBNumber = group.FirstOrDefault().HAWBNumber.HasValue ? group.FirstOrDefault().HAWBNumber.Value : 0,
                                         TotalVolume = group.Select(p => p.Packages.FirstOrDefault().PackageCalculatonType == FraytePakageCalculationType.kgtoCms ? Math.Round((p.Packages.FirstOrDefault().Length / 100) * (p.Packages.FirstOrDefault().Width / 100) * (p.Packages.FirstOrDefault().Height / 100), 2) : Math.Round((p.Packages.FirstOrDefault().Length / 39.37M) * (p.Packages.FirstOrDefault().Width / 39.37M) * (p.Packages.FirstOrDefault().Height / 39.37M), 2)).Sum(),
                                         TotalCartons = group.Select(p => p.Packages.FirstOrDefault().CartonValue).Sum(),
                                         EstimatedWeight = group.Select(p => p.Packages.FirstOrDefault().Weight * p.Packages.FirstOrDefault().CartonValue).Sum(),
                                         TotalWeight = group.Select(p => p.Packages.FirstOrDefault().Weight * p.Packages.FirstOrDefault().CartonValue).Sum(),
                                         PackagesCount = group.Count(),
                                         Packages = group
                                                   .Select(subGroup => new TradelanePackage
                                                   {
                                                       CartonNumber = subGroup.Packages.FirstOrDefault().CartonNumber,
                                                       CartonValue = subGroup.Packages.FirstOrDefault().CartonValue,
                                                       HAWB = subGroup.HAWB,
                                                       Height = subGroup.Packages.FirstOrDefault().Height,
                                                       Length = subGroup.Packages.FirstOrDefault().Length,
                                                       TradelaneShipmentDetailId = subGroup.Packages.FirstOrDefault().TradelaneShipmentDetailId,
                                                       TradelaneShipmentId = subGroup.TradelaneShipmentId,
                                                       Weight = Math.Round(subGroup.Packages.FirstOrDefault().Weight, 2),
                                                       Width = subGroup.Packages.FirstOrDefault().Width,
                                                       IsScanned = subGroup.Packages.FirstOrDefault().IsScanned != null ? subGroup.Packages.FirstOrDefault().IsScanned.Value : false
                                                   }).ToList()
                                     }).ToList();

                    return list;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public TradelaneMappedUnMappedShipment IsAllHawbAssigned(int tradelaneShipmentId)
        {
            TradelaneMappedUnMappedShipment result = new TradelaneMappedUnMappedShipment();

            var total = dbContext.TradelaneShipmentDetails.Where(p => p.TradelaneShipmentId == tradelaneShipmentId).ToList();
            var allocated = dbContext.TradelaneShipmentDetails.Where(p => p.TradelaneShipmentId == tradelaneShipmentId && !string.IsNullOrEmpty(p.HAWB)).ToList();
            var unallocated = dbContext.TradelaneShipmentDetails.Where(p => p.TradelaneShipmentId == tradelaneShipmentId && string.IsNullOrEmpty(p.HAWB)).ToList();
            var shipment = dbContext.TradelaneShipments.Find(tradelaneShipmentId);
            if (shipment != null && shipment.HAWBNumber.HasValue)
            {
                result.HAWBNumber = shipment.HAWBNumber.Value;
            }
            else
            {
                result.HAWBNumber = 0;
            }
            result.TotalShipments = total.Count;
            result.AllocatedShipments = allocated.Count;
            result.UnAllocatedShipments = unallocated.Count;
            return result;
        }

        public FrayteResult AssigneedHAWB(List<TrackTradelanePackage> packages)
        {
            FrayteResult result = new FrayteResult();

            if (packages != null && packages.Count > 0)
            {
                TradelaneShipmentDetail package;

                foreach (var item in packages)
                {
                    try
                    {
                        package = dbContext.TradelaneShipmentDetails.Find(item.TradelaneShipmentDetailId);
                        if (packages != null)
                        {
                            package.HAWB = item.HAWB;
                            dbContext.SaveChanges();
                            if (package.TradelaneShipmentDetailId > 0 && package.HAWB.Length > 0)
                            {
                                SaveTradelaneHawb(package.HAWB, package.TradelaneShipmentId);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                result.Status = true;
            }
            return result;
        }

        public bool SaveHAWBAddress(FrayteHAWBAddress hawbaddress)
        {
            try
            {
                TradelaneShipmentAddress FromAddress = new TradelaneShipmentAddress();
                if (hawbaddress.ShipFrom != null)
                {
                    FromAddress.ContactFirstName = hawbaddress.ShipFrom.FirstName;
                    FromAddress.ContactLastName = hawbaddress.ShipFrom.LastName;
                    FromAddress.CompanyName = hawbaddress.ShipFrom.CompanyName;
                    FromAddress.Email = hawbaddress.ShipFrom.Email;
                    FromAddress.PhoneNo = hawbaddress.ShipFrom.Phone;
                    FromAddress.Address1 = hawbaddress.ShipFrom.Address;
                    FromAddress.Address2 = hawbaddress.ShipFrom.Address2;
                    FromAddress.Area = hawbaddress.ShipFrom.Area;
                    FromAddress.City = hawbaddress.ShipFrom.City;
                    FromAddress.State = hawbaddress.ShipFrom.State;
                    FromAddress.Zip = hawbaddress.ShipFrom.PostCode;
                    FromAddress.CountryId = hawbaddress.ShipFrom.Country.CountryId;
                    FromAddress.TradelaneShipmentDetailId = hawbaddress.ShipFrom.TradelaneShipmentDetailId;
                    dbContext.TradelaneShipmentAddresses.Add(FromAddress);
                    dbContext.SaveChanges();

                    //Update From Address in TradelaneShipmentDetail table
                    int FromAddressid = FromAddress.TradelaneShipmentAddressId;

                    var UpdateFromAddress = dbContext.TradelaneShipmentDetails.Find(hawbaddress.ShipFrom.TradelaneShipmentDetailId);
                    if (UpdateFromAddress != null)
                    {
                        UpdateFromAddress.FromAddressId = FromAddressid;
                        dbContext.Entry(UpdateFromAddress).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }

                TradelaneShipmentAddress ToAddress = new TradelaneShipmentAddress();
                if (hawbaddress.ShipTo != null)
                {
                    ToAddress.ContactFirstName = hawbaddress.ShipTo.FirstName;
                    ToAddress.ContactLastName = hawbaddress.ShipTo.LastName;
                    ToAddress.CompanyName = hawbaddress.ShipTo.CompanyName;
                    ToAddress.Email = hawbaddress.ShipTo.Email;
                    ToAddress.PhoneNo = hawbaddress.ShipTo.Phone;
                    ToAddress.Address1 = hawbaddress.ShipTo.Address;
                    ToAddress.Address2 = hawbaddress.ShipTo.Address2;
                    ToAddress.Area = hawbaddress.ShipTo.Area;
                    ToAddress.City = hawbaddress.ShipTo.City;
                    ToAddress.State = hawbaddress.ShipTo.State;
                    ToAddress.Zip = hawbaddress.ShipTo.PostCode;
                    ToAddress.CountryId = hawbaddress.ShipTo.Country.CountryId;
                    ToAddress.TradelaneShipmentDetailId = hawbaddress.ShipTo.TradelaneShipmentDetailId;
                    dbContext.TradelaneShipmentAddresses.Add(ToAddress);
                    dbContext.SaveChanges();

                    //Update To Address in TradelaneShipmentDetail table
                    int ToAddressid = ToAddress.TradelaneShipmentAddressId;

                    var UpdateToAddress = dbContext.TradelaneShipmentDetails.Find(hawbaddress.ShipTo.TradelaneShipmentDetailId);
                    if (UpdateToAddress != null)
                    {
                        UpdateToAddress.IsNotifyPartySameAsReceiver = hawbaddress.IsNotifyPartySameAsReceiver;
                        UpdateToAddress.ToAddressId = ToAddressid;
                        dbContext.Entry(UpdateToAddress).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }

                TradelaneShipmentAddress NotifyPartyAddress = new TradelaneShipmentAddress();
                if (hawbaddress.NotifyParty != null)
                {
                    if (hawbaddress.IsNotifyPartySameAsReceiver == false)
                    {
                        NotifyPartyAddress.ContactFirstName = hawbaddress.NotifyParty.FirstName;
                        NotifyPartyAddress.ContactLastName = hawbaddress.NotifyParty.LastName;
                        NotifyPartyAddress.CompanyName = hawbaddress.NotifyParty.CompanyName;
                        NotifyPartyAddress.Email = hawbaddress.NotifyParty.Email;
                        NotifyPartyAddress.PhoneNo = hawbaddress.NotifyParty.Phone;
                        NotifyPartyAddress.Address1 = hawbaddress.NotifyParty.Address;
                        NotifyPartyAddress.Address2 = hawbaddress.NotifyParty.Address2;
                        NotifyPartyAddress.Area = hawbaddress.NotifyParty.Area;
                        NotifyPartyAddress.City = hawbaddress.NotifyParty.City;
                        NotifyPartyAddress.State = hawbaddress.NotifyParty.State;
                        NotifyPartyAddress.Zip = hawbaddress.NotifyParty.PostCode;
                        NotifyPartyAddress.CountryId = hawbaddress.NotifyParty.Country.CountryId;
                        NotifyPartyAddress.TradelaneShipmentDetailId = hawbaddress.NotifyParty.TradelaneShipmentDetailId;
                        dbContext.TradelaneShipmentAddresses.Add(NotifyPartyAddress);
                        dbContext.SaveChanges();

                        //Update NotifyParty Address  in TradelaneShipmentDetail table
                        int NotifyAddressid = NotifyPartyAddress.TradelaneShipmentAddressId;

                        var UpdateNotifyPartyAddress = dbContext.TradelaneShipmentDetails.Find(hawbaddress.NotifyParty.TradelaneShipmentDetailId);
                        if (UpdateNotifyPartyAddress != null)
                        {
                            UpdateNotifyPartyAddress.NotifyPartyAddressId = NotifyAddressid;
                            dbContext.Entry(UpdateNotifyPartyAddress).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public FrayteHAWBAddress GetHAWBAddress(string HAWB)
        {
            var detail = (from r in dbContext.TradelaneShipmentDetails
                          join fc in dbContext.TradelaneShipmentAddresses on r.FromAddressId equals fc.TradelaneShipmentAddressId
                          join c in dbContext.Countries on fc.CountryId equals c.CountryId
                          join tc in dbContext.TradelaneShipmentAddresses on r.ToAddressId equals tc.TradelaneShipmentAddressId
                          join cc in dbContext.Countries on tc.CountryId equals cc.CountryId
                          join nf in dbContext.TradelaneShipmentAddresses on r.NotifyPartyAddressId equals nf.TradelaneShipmentAddressId into leftjoin
                          from le in leftjoin.DefaultIfEmpty()
                          join nfc in dbContext.Countries on le.CountryId equals nfc.CountryId into leftnjoin
                          from nfe in leftnjoin.DefaultIfEmpty()
                          where r.HAWB == HAWB
                          select new FrayteHAWBAddress
                          {
                              ShipFrom = new TradelBookingAdress()
                              {
                                  FirstName = fc.ContactFirstName,
                                  LastName = fc.ContactLastName,
                                  CompanyName = fc.CompanyName,
                                  Email = fc.Email,
                                  Phone = fc.PhoneNo,
                                  Address = fc.Address1,
                                  Address2 = fc.Address2,
                                  Area = fc.Area,
                                  City = fc.City,
                                  State = fc.State,
                                  PostCode = fc.Zip,
                                  Country = new FrayteCountryCode()
                                  {
                                      CountryId = c.CountryId,
                                      Name = c.CountryName,
                                      Code = c.CountryCode,
                                      Code2 = c.CountryCode2,
                                      CountryPhoneCode = c.CountryPhoneCode
                                  }
                              },
                              ShipTo = new TradelBookingAdress()
                              {
                                  FirstName = tc.ContactFirstName,
                                  LastName = tc.ContactLastName,
                                  CompanyName = tc.CompanyName,
                                  Email = tc.Email,
                                  Phone = tc.PhoneNo,
                                  Address = tc.Address1,
                                  Address2 = tc.Address2,
                                  Area = tc.Area,
                                  City = tc.City,
                                  State = tc.State,
                                  PostCode = tc.Zip,
                                  Country = new FrayteCountryCode()
                                  {
                                      CountryId = cc.CountryId,
                                      Name = cc.CountryName,
                                      Code = cc.CountryCode,
                                      Code2 = cc.CountryCode2,
                                      CountryPhoneCode = cc.CountryPhoneCode
                                  }
                              },
                              NotifyParty = new TradelBookingAdress()
                              {
                                  FirstName = le != null ? le.ContactFirstName : "",
                                  LastName = le != null ? le.ContactLastName : "",
                                  CompanyName = le != null ? le.CompanyName : "",
                                  Email = le != null ? le.Email : "",
                                  Phone = le != null ? le.PhoneNo : "",
                                  Address = le != null ? le.Address1 : "",
                                  Address2 = le != null ? le.Address2 : "",
                                  Area = le != null ? le.Area : "",
                                  City = le != null ? le.City : "",
                                  State = le != null ? le.State : "",
                                  PostCode = le != null ? le.Zip : "",
                                  Country = new FrayteCountryCode()
                                  {
                                      CountryId = nfe != null ? nfe.CountryId : 0,
                                      Name = nfe != null ? nfe.CountryName : "",
                                      Code = nfe != null ? nfe.CountryCode : "",
                                      Code2 = nfe != null ? nfe.CountryCode2 : "",
                                      CountryPhoneCode = nfe != null ? nfe.CountryPhoneCode : ""
                                  }
                              },
                              IsNotifyPartySameAsReceiver = r.IsNotifyPartySameAsReceiver
                          }).FirstOrDefault();

            return detail;
        }

        public List<TrackTradelanePackage> TrackTradelanePackages(TrackPackage track)
        {
            List<TrackTradelanePackage> list = new List<TrackTradelanePackage>();
            int TotalRows = 0;
            int SkipRows = 0;
            SkipRows = (track.CurrentPage - 1) * track.TakeRows;

            var collection = dbContext.TrackPackages(track.ShipmentId, track.Type, track.HAWB, SkipRows, track.TakeRows).ToList();

            TrackTradelanePackage pack;

            if (collection != null && collection.Count > 0)
            {
                foreach (var item in collection)
                {
                    pack = new TrackTradelanePackage();
                    pack.TotalRows = item.TotalRows.HasValue ? item.TotalRows.Value : 0;
                    pack.CartonNumber = item.CartonNumber;
                    pack.CartonValue = item.CartonValue;
                    pack.HAWB = item.HAWB;
                    pack.Height = item.Height;
                    pack.Length = item.Length;
                    pack.TradelaneShipmentDetailId = item.TradelaneShipmentDetailId;
                    pack.TradelaneShipmentId = item.TradelaneShipmentId;
                    pack.Width = item.Width;
                    pack.Weight = item.Weight;
                    list.Add(pack);
                }
            }
            return list;
        }

        public void SaveSalesOrderNumber(List<HAWBTradelanePackage> hAWBpackages)
        {
            foreach (var item in hAWBpackages)
            {
                var collection = dbContext.TradelaneShipmentDetails.Where(p => p.HAWB == item.HAWB).ToList();
                foreach (var item1 in collection)
                {
                    item.SONumber = item.SONumber;
                    dbContext.SaveChanges();
                }
            }
        }

        public string GetFileNameFromString(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                string fileNameWithoutExtension = string.Empty;
                var arr = filename.Split('.');
                if (arr != null && arr.Length > 0)
                {
                    if (arr.Length == 2)
                    {
                        fileNameWithoutExtension = arr[0];
                    }
                    else if (arr.Length > 2)
                    {
                        for (int i = 0; i < arr.Length - 1; i++)
                        {
                            fileNameWithoutExtension += arr[i];
                        }
                    }

                    return fileNameWithoutExtension;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public string GetFileExtensionFromString(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                string fileExtension = string.Empty;
                var arr = filename.Split('.');
                if (arr != null && arr.Length > 0)
                {
                    if (arr.Length == 2)
                    {
                        fileExtension = arr[1];
                    }
                    else if (arr.Length > 2)
                    {
                        fileExtension += arr[arr.Length - 1];
                    }
                    return fileExtension;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public FrayteResult UpdateMAWBDetail(TradelaneMAWBDetail mawbDetail)
        {
            FrayteResult result = new FrayteResult();

            if (mawbDetail.List != null && mawbDetail.List.Count > 0)
            {
                var allocation = dbContext.TradelaneShipmentAllocations.Find(mawbDetail.List[0].MawbAllocationId);

                if (allocation != null)
                {
                    if (string.IsNullOrEmpty(allocation.LegNum) || allocation.LegNum == "Leg1")
                    {
                        var shipment = dbContext.TradelaneShipments.Find(mawbDetail.TradelaneShipmentId);
                        if (shipment != null)
                        {
                            shipment.IsAgentMAWBAllocated = true;
                            dbContext.SaveChanges();
                            result.Status = true;
                        }
                    }
                    else
                    {
                        result.Status = true;
                    }
                }
            }
            return result;
        }

        public List<TradelanePackage> GetPiecesDetail(System.Data.DataTable exceldata)
        {
            List<TradelanePackage> _shipmentdetail = new List<TradelanePackage>();
            TradelanePackage frayteshipment;

            foreach (DataRow shipmentdetail in exceldata.Rows)
            {
                frayteshipment = new TradelanePackage();
                frayteshipment.TradelaneShipmentDetailId = 0;

                // format the value  // Parse the values
                int CartonQTY = Convert.ToInt32(Math.Floor(CommonConversion.ConvertToDecimal(shipmentdetail["CartonQTY"].ToString().Trim() != "" || shipmentdetail["CartonQTY"].ToString().Trim() != null ? shipmentdetail["CartonQTY"].ToString().Trim() : "")));
                string CartonNumber = shipmentdetail["CartonNumber"].ToString().Trim() != "" || shipmentdetail["CartonNumber"].ToString().Trim() != null ? shipmentdetail["CartonNumber"].ToString().Trim() : "";
                decimal Length = CommonConversion.ConvertToDecimal(shipmentdetail["Length"].ToString().Trim() != "" || shipmentdetail["Length"].ToString().Trim() != null ? shipmentdetail["Length"].ToString().Trim() : "");
                decimal Width = CommonConversion.ConvertToDecimal(shipmentdetail["Width"].ToString().Trim() != "" || shipmentdetail["Width"].ToString().Trim() != null ? shipmentdetail["Width"].ToString().Trim() : "");
                decimal Height = CommonConversion.ConvertToDecimal(shipmentdetail["Height"].ToString().Trim() != "" || shipmentdetail["Height"].ToString().Trim() != null ? shipmentdetail["Height"].ToString().Trim() : "");
                decimal Weight = CommonConversion.ConvertToDecimal(shipmentdetail["Weight"].ToString().Trim() != "" || shipmentdetail["Weight"].ToString().Trim() != null ? shipmentdetail["Weight"].ToString().Trim() : "");

                // if the record is vaild then add it 
                if (CartonQTY > 0 && CartonQTY < 999 &&
                    !string.IsNullOrEmpty(CartonNumber) && CartonNumber.Length <= 15 &&
                    Length > 0 && Length < 999 && Width > 0 && Width < 999 && Height > 0 && Height < 999 &&
                    Weight > 0 && Weight < 999
                    )
                {

                    frayteshipment.CartonNumber = CartonNumber;
                    frayteshipment.CartonValue = CartonQTY;
                    frayteshipment.Weight = Weight;
                    frayteshipment.Length = Length;
                    frayteshipment.Width = Width;
                    frayteshipment.Height = Height;

                    TradelanePackage frayteshipmentD;
                    if (_shipmentdetail.Count > 0)
                    {
                        frayteshipmentD = _shipmentdetail.Where(p => p.CartonNumber == CartonNumber).FirstOrDefault();

                        if (frayteshipmentD == null)
                        {
                            _shipmentdetail.Add(frayteshipment);
                        }
                    }
                    else
                    {
                        _shipmentdetail.Add(frayteshipment);
                    }
                }
            }
            return _shipmentdetail;
        }

        public bool IsMAWBUploaded(int tradelaneShipmentId, int agentId, string legNum)
        {
            var shipmentDetail = dbContext.TradelaneShipments.Find(tradelaneShipmentId);
            if (shipmentDetail.IsMawbCorrection.HasValue && shipmentDetail.IsMawbCorrection.Value)
            {
                return false;
            }
            else
            {
                if (string.IsNullOrEmpty(legNum))
                {
                    if (shipmentDetail.IsAgentMAWBAllocated.HasValue)
                    {
                        return shipmentDetail.IsAgentMAWBAllocated.Value;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (legNum == "Leg1")
                    {
                        if (shipmentDetail.IsAgentMAWBAllocated.HasValue)
                        {
                            return shipmentDetail.IsAgentMAWBAllocated.Value;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        var allocation = dbContext.TradelaneShipmentAllocations.Where(p => p.CreatedBy == agentId && p.TradelaneShipmentId == tradelaneShipmentId && p.LegNum == legNum).FirstOrDefault();
                        if (allocation != null)
                        {
                            if (!string.IsNullOrEmpty(allocation.MAWB))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        public bool ValidateAgent(int agentId, int tradelaneShipmentId)
        {
            var shipmentDetail = dbContext.TradelaneShipments.Where(p => p.MAWBAgentId == agentId && p.TradelaneShipmentId == tradelaneShipmentId).FirstOrDefault();

            if (shipmentDetail != null)
            {
                if (shipmentDetail.ShipmentStatusId == (int)FrayteTradelaneShipmentStatus.Delivered)
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ValidateAgent(int agentId, int tradelaneShipmentId, string legNum)
        {
            var shipmentAllocationDetail = dbContext.TradelaneShipmentAllocations.Where(p => p.AgentId == agentId && p.TradelaneShipmentId == tradelaneShipmentId && p.LegNum == legNum).FirstOrDefault();
            if (shipmentAllocationDetail != null)
            {
                var detail = dbContext.TradelaneShipments.Find(tradelaneShipmentId);
                if (detail.ShipmentStatusId == (int)FrayteTradelaneShipmentStatus.Delivered)
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public int ValidateMAWBInitial(TradelaneMAWBUlpoadInitial initialDetail)
        {
            bool flag = false;

            int id = 0;

            var shipment = (from r in dbContext.TradelaneShipments
                            where r.FrayteNumber == initialDetail.FrayteNumber
                            select r).FirstOrDefault();

            if (shipment != null)
            {
                id = shipment.TradelaneShipmentId;

                var user = (from r in dbContext.Users
                            join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                            where r.UserId == initialDetail.AgentId && ur.RoleId == (int)FrayteUserRole.Agent
                            select r).FirstOrDefault();

                if (user != null)
                {
                    var shipmentMethod = dbContext.ShipmentHandlerMethods.Find(initialDetail.ShipmentMethodHandlerId);

                    if (shipmentMethod != null)
                    {
                        if (!string.IsNullOrEmpty(initialDetail.Leg))
                        {
                            if (initialDetail.Leg == "Leg1" || initialDetail.Leg == "Leg2")
                            {
                                flag = true;
                            }
                            else
                            {
                                flag = false;
                            }
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                }
            }

            if (flag)
            {
                return id;
            }
            else
            {
                return 0;
            }
        }

        public TradelaneMAWBUlpoadInitial GetDetailsFormString(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                TradelaneMAWBUlpoadInitial detail = new TradelaneMAWBUlpoadInitial();
                string[] arr = key.Split('-');
                if (arr != null && arr.Length >= 3 && arr.Length <= 4)
                {
                    if (arr.Length == 3)
                    {
                        detail.FrayteNumber = arr[0];
                        detail.AgentId = CommonConversion.ConvertToInt(arr[1]);
                        detail.ShipmentMethodHandlerId = CommonConversion.ConvertToInt(arr[2]);
                    }
                    else
                    {
                        detail.FrayteNumber = arr[0];
                        detail.Leg = arr[1];
                        detail.AgentId = CommonConversion.ConvertToInt(arr[2]);
                        detail.ShipmentMethodHandlerId = CommonConversion.ConvertToInt(arr[3]);
                    }

                    return detail;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private void SaveTradelaneShipmentAddress(int ShipmentStatusId, TradelBookingAdress tradelaneAddress, string AddressType)
        {
            TradelaneShipmentAddress shipmentAddress;

            if (AddressType == FrayteFromToAddressType.FromAddress)
            {
                if (tradelaneAddress.TradelaneShipmentAddressId == 0)
                {
                    shipmentAddress = new TradelaneShipmentAddress();

                    shipmentAddress.CountryId = tradelaneAddress.Country.CountryId;
                    shipmentAddress.CompanyName = tradelaneAddress.CompanyName;
                    shipmentAddress.ContactFirstName = tradelaneAddress.FirstName;
                    shipmentAddress.ContactLastName = tradelaneAddress.LastName;
                    shipmentAddress.City = tradelaneAddress.City;
                    shipmentAddress.State = tradelaneAddress.State;
                    shipmentAddress.Zip = tradelaneAddress.PostCode;
                    shipmentAddress.Address1 = tradelaneAddress.Address;
                    shipmentAddress.Address2 = tradelaneAddress.Address2;
                    shipmentAddress.Email = tradelaneAddress.Email;
                    shipmentAddress.PhoneNo = tradelaneAddress.Phone;
                    shipmentAddress.Area = tradelaneAddress.Area;
                    dbContext.TradelaneShipmentAddresses.Add(shipmentAddress);
                    dbContext.SaveChanges();

                    tradelaneAddress.TradelaneShipmentAddressId = shipmentAddress.TradelaneShipmentAddressId;
                }
                else
                {
                    shipmentAddress = dbContext.TradelaneShipmentAddresses.Find(tradelaneAddress.TradelaneShipmentAddressId);
                    shipmentAddress.CountryId = tradelaneAddress.Country.CountryId;
                    shipmentAddress.CompanyName = tradelaneAddress.CompanyName;
                    shipmentAddress.ContactFirstName = tradelaneAddress.FirstName;
                    shipmentAddress.ContactLastName = tradelaneAddress.LastName;
                    shipmentAddress.City = tradelaneAddress.City;
                    shipmentAddress.State = tradelaneAddress.State;
                    shipmentAddress.Zip = tradelaneAddress.PostCode;
                    shipmentAddress.Address1 = tradelaneAddress.Address;
                    shipmentAddress.Address2 = tradelaneAddress.Address2;
                    shipmentAddress.Email = tradelaneAddress.Email;
                    shipmentAddress.PhoneNo = tradelaneAddress.Phone;
                    shipmentAddress.Area = tradelaneAddress.Area;
                    dbContext.SaveChanges();
                }

                if (ShipmentStatusId != (int)FrayteTradelaneShipmentStatus.Draft)
                {
                    // Check if  shipfrom address is different then save it to adress Book
                    var addressBooks = dbContext.AddressBooks.Where(p => p.Address1 == tradelaneAddress.Address &&
                                                                           p.Address2 == tradelaneAddress.Address2 &&
                                                                           p.CustomerId == tradelaneAddress.CustomerId &&
                                                                           p.City == tradelaneAddress.City &&
                                                                           p.State == tradelaneAddress.State &&
                                                                           p.PhoneNo == tradelaneAddress.Phone &&
                                                                           p.Area == tradelaneAddress.Area &&
                                                                           p.CompanyName == tradelaneAddress.CompanyName &&
                                                                           p.ContactFirstName == tradelaneAddress.FirstName &&
                                                                           p.ContactLastName == tradelaneAddress.LastName &&
                                                                           p.CountryId == tradelaneAddress.Country.CountryId &&
                                                                           p.CustomerId == tradelaneAddress.CustomerId &&
                                                                           p.Email == tradelaneAddress.Email &&
                                                                           p.Zip == tradelaneAddress.PostCode &&
                                                                           p.IsActive == true &&
                                                                           p.FromAddress == true).FirstOrDefault();

                    if (addressBooks == null)
                    {
                        AddressBook address = new AddressBook();

                        address.Address1 = tradelaneAddress.Address;
                        address.Address2 = tradelaneAddress.Address2;
                        address.Area = tradelaneAddress.Area;
                        address.City = tradelaneAddress.City;
                        address.CompanyName = tradelaneAddress.CompanyName;
                        address.ContactFirstName = tradelaneAddress.FirstName;
                        address.ContactLastName = tradelaneAddress.LastName;
                        address.CountryId = tradelaneAddress.Country.CountryId;
                        address.CustomerId = tradelaneAddress.CustomerId;
                        address.FromAddress = true;
                        address.Email = tradelaneAddress.Email;
                        address.Zip = tradelaneAddress.PostCode;
                        address.ToAddress = false;
                        address.IsActive = true;
                        address.IsDefault = tradelaneAddress.IsDefault;
                        address.PhoneNo = tradelaneAddress.Phone;
                        dbContext.AddressBooks.Add(address);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        if (tradelaneAddress.IsDefault)
                        {
                            addressBooks.IsDefault = tradelaneAddress.IsDefault;
                            dbContext.SaveChanges();
                        }
                    }
                }
            }

            else if (AddressType == FrayteFromToAddressType.ToAddress)
            {
                if (tradelaneAddress.TradelaneShipmentAddressId == 0)
                {
                    shipmentAddress = new TradelaneShipmentAddress();

                    shipmentAddress.CountryId = tradelaneAddress.Country.CountryId;
                    shipmentAddress.CompanyName = tradelaneAddress.CompanyName;
                    shipmentAddress.ContactFirstName = tradelaneAddress.FirstName;
                    shipmentAddress.ContactLastName = tradelaneAddress.LastName;
                    shipmentAddress.City = tradelaneAddress.City;
                    shipmentAddress.State = tradelaneAddress.State;
                    shipmentAddress.Zip = tradelaneAddress.PostCode;
                    shipmentAddress.Address1 = tradelaneAddress.Address;
                    shipmentAddress.Address2 = tradelaneAddress.Address2;
                    shipmentAddress.Email = tradelaneAddress.Email;
                    shipmentAddress.PhoneNo = tradelaneAddress.Phone;
                    shipmentAddress.Area = tradelaneAddress.Area;
                    dbContext.TradelaneShipmentAddresses.Add(shipmentAddress);
                    dbContext.SaveChanges();

                    tradelaneAddress.TradelaneShipmentAddressId = shipmentAddress.TradelaneShipmentAddressId;
                }
                else
                {
                    shipmentAddress = dbContext.TradelaneShipmentAddresses.Find(tradelaneAddress.TradelaneShipmentAddressId);

                    shipmentAddress.CountryId = tradelaneAddress.Country.CountryId;
                    shipmentAddress.CompanyName = tradelaneAddress.CompanyName;
                    shipmentAddress.ContactFirstName = tradelaneAddress.FirstName;
                    shipmentAddress.ContactLastName = tradelaneAddress.LastName;
                    shipmentAddress.City = tradelaneAddress.City;
                    shipmentAddress.State = tradelaneAddress.State;
                    shipmentAddress.Zip = tradelaneAddress.PostCode;
                    shipmentAddress.Address1 = tradelaneAddress.Address;
                    shipmentAddress.Address2 = tradelaneAddress.Address2;
                    shipmentAddress.Email = tradelaneAddress.Email;
                    shipmentAddress.PhoneNo = tradelaneAddress.Phone;
                    shipmentAddress.Area = tradelaneAddress.Area;

                    dbContext.SaveChanges();

                    tradelaneAddress.TradelaneShipmentAddressId = shipmentAddress.TradelaneShipmentAddressId;
                }

                if (ShipmentStatusId != (int)FrayteTradelaneShipmentStatus.Draft)
                {
                    // Check if  shipfrom address is different then save it to adress Book
                    var addressBooks = dbContext.AddressBooks.Where(p => p.Address1 == tradelaneAddress.Address &&
                                                                           p.Address2 == tradelaneAddress.Address2 &&
                                                                           p.CustomerId == tradelaneAddress.CustomerId &&
                                                                           p.City == tradelaneAddress.City &&
                                                                           p.State == tradelaneAddress.State &&
                                                                           p.PhoneNo == tradelaneAddress.Phone &&
                                                                           p.Area == tradelaneAddress.Area &&
                                                                           p.CompanyName == tradelaneAddress.CompanyName &&
                                                                           p.ContactFirstName == tradelaneAddress.FirstName &&
                                                                           p.ContactLastName == tradelaneAddress.LastName &&
                                                                           p.CountryId == tradelaneAddress.Country.CountryId &&
                                                                           p.CustomerId == tradelaneAddress.CustomerId &&
                                                                           p.Email == tradelaneAddress.Email &&
                                                                           p.Zip == tradelaneAddress.PostCode &&
                                                                           p.IsActive == true &&
                                                                           p.ToAddress == true).FirstOrDefault();

                    if (addressBooks == null)
                    {
                        AddressBook address = new AddressBook();

                        address.Address1 = tradelaneAddress.Address;
                        address.Address2 = tradelaneAddress.Address2;
                        address.Area = tradelaneAddress.Area;
                        address.City = tradelaneAddress.City;
                        address.CompanyName = tradelaneAddress.CompanyName;
                        address.ContactFirstName = tradelaneAddress.FirstName;
                        address.ContactLastName = tradelaneAddress.LastName;
                        address.CountryId = tradelaneAddress.Country.CountryId;
                        address.CustomerId = tradelaneAddress.CustomerId;
                        address.FromAddress = false;
                        address.Email = tradelaneAddress.Email;
                        address.Zip = tradelaneAddress.PostCode;
                        address.ToAddress = true;
                        address.IsActive = true;
                        address.IsDefault = tradelaneAddress.IsDefault;
                        address.PhoneNo = tradelaneAddress.Phone;
                        dbContext.AddressBooks.Add(address);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        if (tradelaneAddress.IsDefault)
                        {
                            addressBooks.IsDefault = tradelaneAddress.IsDefault;
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            else if (AddressType == FrayteFromToAddressType.NotifyPartyAddress)
            {
                if (tradelaneAddress.TradelaneShipmentAddressId == 0)
                {
                    shipmentAddress = new TradelaneShipmentAddress();
                    if (tradelaneAddress.Country != null && tradelaneAddress.Country.CountryId > 0)
                    {
                        shipmentAddress.CountryId = tradelaneAddress.Country.CountryId;
                        shipmentAddress.CompanyName = tradelaneAddress.CompanyName;
                        shipmentAddress.ContactFirstName = tradelaneAddress.FirstName;
                        shipmentAddress.ContactLastName = tradelaneAddress.LastName;
                        shipmentAddress.City = tradelaneAddress.City;
                        shipmentAddress.State = tradelaneAddress.State;
                        shipmentAddress.Zip = tradelaneAddress.PostCode;
                        shipmentAddress.Address1 = tradelaneAddress.Address;
                        shipmentAddress.Address2 = tradelaneAddress.Address2;
                        shipmentAddress.Email = tradelaneAddress.Email;
                        shipmentAddress.PhoneNo = tradelaneAddress.Phone;
                        shipmentAddress.Area = tradelaneAddress.Area;
                        dbContext.TradelaneShipmentAddresses.Add(shipmentAddress);
                        dbContext.SaveChanges();
                        tradelaneAddress.TradelaneShipmentAddressId = shipmentAddress.TradelaneShipmentAddressId;
                    }
                }
                else
                {
                    shipmentAddress = dbContext.TradelaneShipmentAddresses.Find(tradelaneAddress.TradelaneShipmentAddressId);
                    if (tradelaneAddress.Country != null && tradelaneAddress.Country.CountryId > 0)
                    {
                        shipmentAddress.CountryId = tradelaneAddress.Country.CountryId;
                        shipmentAddress.CompanyName = tradelaneAddress.CompanyName;
                        shipmentAddress.ContactFirstName = tradelaneAddress.FirstName;
                        shipmentAddress.ContactLastName = tradelaneAddress.LastName;
                        shipmentAddress.City = tradelaneAddress.City;
                        shipmentAddress.State = tradelaneAddress.State;
                        shipmentAddress.Zip = tradelaneAddress.PostCode;
                        shipmentAddress.Address1 = tradelaneAddress.Address;
                        shipmentAddress.Address2 = tradelaneAddress.Address2;
                        shipmentAddress.Email = tradelaneAddress.Email;
                        shipmentAddress.PhoneNo = tradelaneAddress.Phone;
                        shipmentAddress.Area = tradelaneAddress.Area;
                        dbContext.SaveChanges();
                        tradelaneAddress.TradelaneShipmentAddressId = shipmentAddress.TradelaneShipmentAddressId;
                    }
                }
            }
        }

        public void SavePackagedetailShipment(List<TradelanePackage> packages, int shipmentId)
        {
            if (packages.Count > 0)
            {
                TradelaneShipmentDetail packageDetail;
                foreach (var item in packages)
                {
                    if (shipmentId > 0)
                    {
                        try
                        {
                            packageDetail = new TradelaneShipmentDetail();
                            packageDetail.TradelaneShipmentId = shipmentId;
                            packageDetail.Length = item.Length;
                            packageDetail.Height = item.Height;
                            packageDetail.Width = item.Width;
                            packageDetail.Weight = item.Weight;
                            packageDetail.CartonNumber = item.CartonNumber;
                            packageDetail.CartonValue = item.CartonValue;
                            dbContext.TradelaneShipmentDetails.Add(packageDetail);
                            dbContext.SaveChanges();
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
        }

        public void SaveShipmentTracking(TradelaneBooking shipment)
        {
            try
            {
                var shipmentDetail = dbContext.TradelaneShipmentDetails.Where(p => p.TradelaneShipmentId == shipment.TradelaneShipmentId).ToList();
                TradelaneShipmentTracking shipmentTracking = new TradelaneShipmentTracking();
                shipmentTracking.TradlaneShipmentId = shipment.TradelaneShipmentId;
                shipmentTracking.TrackingCode = "BKD";
                shipmentTracking.TrackingDescription = "Shipment Booked";
                shipmentTracking.CreatedOnUtc = DateTime.UtcNow;
                shipmentTracking.CreatedBy = shipment.CreatedBy;
                shipmentTracking.Weight = decimal.Round(shipmentDetail.Sum(p => p.Weight), 2);
                shipmentTracking.Pieces = shipmentDetail.Sum(p => p.CartonValue);
                shipmentTracking.AirportCode = shipment.DepartureAirport.AirportCode;
                dbContext.TradelaneShipmentTrackings.Add(shipmentTracking);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public void SaveShipmentFlightDetail(TradelaneBooking shipment)
        {
            try
            {
                var dbShipment = dbContext.TradelaneShipments.Find(shipment.TradelaneShipmentId);
                var shipmentDetail = dbContext.TradelaneShipmentDetails.Where(p => p.TradelaneShipmentId == shipment.TradelaneShipmentId).ToList();
                TradelaneFlightDetail flightDetail = new TradelaneFlightDetail();
                flightDetail.TradelaneShipmentId = shipment.TradelaneShipmentId;
                flightDetail.DepartureAirportCode = shipment.DepartureAirport.AirportCode;
                flightDetail.ArrivalAirportCode = shipment.DestinationAirport.AirportCode;
                flightDetail.BookingStatus = "Shipment Booked";
                flightDetail.TotalWeight = shipmentDetail.Sum(p => p.Weight);
                flightDetail.Pieces = shipmentDetail.Sum(p => p.CartonValue);
                flightDetail.TotalVolume = dbShipment.PackageCalculatonType == FraytePakageCalculationType.kgtoCms ? Math.Round((shipmentDetail.Sum(p => p.Length * p.Weight * p.Height)) / (100 * 100 * 100), 2) : Math.Round((shipmentDetail.Sum(p => p.Length * p.Weight * p.Height)) / (39.37M * 39.37M * 39.37M), 2);
                dbContext.TradelaneFlightDetails.Add(flightDetail);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }

        private void saveShipmentDetail(TradelaneBooking shipment, string ShipmentType)
        {
            TradelaneShipment dbShipment;
            if (shipment.TradelaneShipmentId == 0)
            {
                dbShipment = new TradelaneShipment();

                dbShipment.CustomerId = shipment.CustomerId;
                if (ShipmentType == "Express")
                {
                    dbShipment.ShipmentStatusId = (int)FrayteTradelaneShipmentStatus.ShipmentBooked;
                }
                else
                {
                    dbShipment.ShipmentStatusId = (int)FrayteTradelaneShipmentStatus.Draft;
                }

                if (shipment.AirlinePreference != null)
                {
                    dbShipment.AirlineId = shipment.AirlinePreference.AirlineId;
                }
                if (shipment.DeclaredCurrency != null)
                {
                    dbShipment.DeclaredCurrencyCode = shipment.DeclaredCurrency.CurrencyCode;
                }
                if (shipment.DepartureAirport != null)
                {
                    dbShipment.DepartureAirportCode = shipment.DepartureAirport.AirportCode;
                }
                if (shipment.DestinationAirport != null)
                {
                    dbShipment.DestinationAirportCode = shipment.DestinationAirport.AirportCode;
                }
                if (shipment.ShipmentHandlerMethod != null)
                {
                    dbShipment.ShipmentHandlerMethodId = shipment.ShipmentHandlerMethod.ShipmentHandlerMethodId;
                }
                dbShipment.CertificateOfOrigin = shipment.CertificateOfOrigin;
                dbShipment.DangerousGoods = shipment.DangerousGoods;

                dbShipment.DeclaredValue = shipment.DeclaredValue;
                dbShipment.ExportLicenceNo = shipment.ExportLicenceNo;

                dbShipment.LogisticType = "AirExport";
                dbShipment.TotalEstimatedWeight = shipment.TotalEstimatedWeight;
                dbShipment.TaxAndDutiesAccountNo = shipment.TaxAndDutiesAccountNo;
                dbShipment.TaxAndDutiesAcceptedBy = shipment.TaxAndDutiesAcceptedBy;
                dbShipment.PaymentPartyTaxAndDuty = shipment.PayTaxAndDuties;
                dbShipment.ShipmentDescription = shipment.ShipmentDescription;
                dbShipment.ShipmentReference = shipment.ShipmentReference;
                dbShipment.PackageCalculatonType = shipment.PakageCalculatonType;
                dbShipment.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
                dbShipment.CreatedOnUtc = DateTime.UtcNow;
                dbShipment.CreatedBy = shipment.CreatedBy;
                dbShipment.FromAddressId = shipment.ShipFrom.TradelaneShipmentAddressId;
                dbShipment.ToAddressId = shipment.ShipTo.TradelaneShipmentAddressId;
                if (shipment.NotifyParty != null)
                {
                    dbShipment.NotifyPartyAddressId = shipment.NotifyParty.TradelaneShipmentAddressId;
                    dbShipment.IsNotifyPartySameAsReceiver = shipment.IsNotifyPartySameAsReceiver;
                }

                dbShipment.FrayteNumber = "TL" + CommonConversion.GetNewFrayteNumber();
                dbShipment.BatteryDeclarationType = shipment.BatteryDeclarationType;

                dbShipment.CustomerConfirmationCode = Guid.NewGuid();

                dbShipment.AdditionalInfo = shipment.AdditionalInfo;
                dbShipment.IncotermId = shipment.Incoterm == null ? 0 : shipment.Incoterm.IncotermID;
                dbContext.TradelaneShipments.Add(dbShipment);
                dbContext.SaveChanges();

                shipment.TradelaneShipmentId = dbShipment.TradelaneShipmentId;
                shipment.FrayteNumber = dbShipment.FrayteNumber;
                if (shipment.FrayteNumber.Length > 0 && shipment.TradelaneShipmentId > 0)
                {
                    SaveTradelaneRef(shipment.FrayteNumber, shipment.TradelaneShipmentId);
                }
            }
            else
            {
                dbShipment = dbContext.TradelaneShipments.Find(shipment.TradelaneShipmentId);
                if (dbShipment != null)
                {
                    dbShipment.CustomerId = shipment.CustomerId;
                    dbShipment.ShipmentStatusId = shipment.ShipmentStatusId;
                    if (shipment.AirlinePreference != null)
                    {
                        dbShipment.AirlineId = shipment.AirlinePreference.AirlineId;
                    }
                    if (shipment.DeclaredCurrency != null)
                    {
                        dbShipment.DeclaredCurrencyCode = shipment.DeclaredCurrency.CurrencyCode;
                    }
                    if (shipment.DepartureAirport != null)
                    {
                        dbShipment.DepartureAirportCode = shipment.DepartureAirport.AirportCode;
                    }
                    if (shipment.DestinationAirport != null)
                    {
                        dbShipment.DestinationAirportCode = shipment.DestinationAirport.AirportCode;
                    }
                    if (shipment.ShipmentHandlerMethod != null)
                    {
                        dbShipment.ShipmentHandlerMethodId = shipment.ShipmentHandlerMethod.ShipmentHandlerMethodId;
                    }
                    dbShipment.CertificateOfOrigin = shipment.CertificateOfOrigin;
                    dbShipment.DangerousGoods = shipment.DangerousGoods;

                    dbShipment.DeclaredValue = shipment.DeclaredValue;
                    dbShipment.ExportLicenceNo = shipment.ExportLicenceNo;

                    dbShipment.LogisticType = "AirExport";
                    dbShipment.TotalEstimatedWeight = shipment.TotalEstimatedWeight;
                    dbShipment.TaxAndDutiesAccountNo = shipment.TaxAndDutiesAccountNo;
                    dbShipment.TaxAndDutiesAcceptedBy = shipment.TaxAndDutiesAcceptedBy;
                    dbShipment.PaymentPartyTaxAndDuty = shipment.PayTaxAndDuties;

                    dbShipment.ShipmentDescription = shipment.ShipmentDescription;
                    dbShipment.ShipmentReference = shipment.ShipmentReference;
                    dbShipment.PackageCalculatonType = shipment.PakageCalculatonType;
                    dbShipment.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
                    dbShipment.UpdatedOnUtc = DateTime.UtcNow;
                    dbShipment.UpdatedBy = shipment.CreatedBy;
                    // dbShipment.MAWB = shipment.MAWB;
                    dbShipment.FromAddressId = shipment.ShipFrom.TradelaneShipmentAddressId;
                    dbShipment.ToAddressId = shipment.ShipTo.TradelaneShipmentAddressId;
                    dbShipment.NotifyPartyAddressId = shipment.NotifyParty.TradelaneShipmentAddressId;
                    dbShipment.IsNotifyPartySameAsReceiver = shipment.IsNotifyPartySameAsReceiver;
                    dbShipment.BatteryDeclarationType = shipment.BatteryDeclarationType;
                    dbShipment.AdditionalInfo = shipment.AdditionalInfo;
                    dbShipment.IncotermId = shipment.Incoterm == null ? 0 : shipment.Incoterm.IncotermID;
                    dbContext.SaveChanges();
                    if (dbShipment.FrayteNumber.Length > 0 && dbShipment.TradelaneShipmentId > 0)
                    {
                        SaveTradelaneRef(dbShipment.FrayteNumber, dbShipment.TradelaneShipmentId);
                    }
                }
            }
        }

        #endregion

        #region Shipment Detail

        public TradelaneBooking GetTradelaneBookingDetails(int shipmentId, string CallingType)
        {
            TradelaneBooking dbDetail = new TradelaneBooking();
            dbDetail.TradelaneShipmentId = shipmentId;

            //Step 1: Get Shipment Detail
            GetDirectShipmnetDetail(dbDetail);

            //Step 2: Get Ship From, Ship To and  Notify Party Address detail
            GetDirectShipmentAddressDetail(dbDetail);

            if (string.IsNullOrEmpty(CallingType))
            {
                //Step 3: Get PackageDetails
                GetDirectShipmentPackagesDetail(dbDetail, CallingType);
            }

            if (CallingType == FrayteCallingType.ShipmentClone)
            {
                dbDetail.TradelaneShipmentId = 0;
                dbDetail.ShipFrom.TradelaneShipmentAddressId = 0;
                dbDetail.ShipTo.TradelaneShipmentAddressId = 0;
                dbDetail.NotifyParty.TradelaneShipmentAddressId = 0;
            }

            return dbDetail;
        }

        //Shipment Detail
        private void GetDirectShipmnetDetail(TradelaneBooking dbDetail)
        {
            TradelaneShipment shipment = dbContext.TradelaneShipments.Find(dbDetail.TradelaneShipmentId);

            dbDetail.ShipFrom = new TradelBookingAdress();
            dbDetail.ShipFrom.TradelaneShipmentAddressId = shipment.FromAddressId;

            dbDetail.ShipTo = new TradelBookingAdress();
            dbDetail.ShipTo.TradelaneShipmentAddressId = shipment.ToAddressId;

            dbDetail.NotifyParty = new TradelBookingAdress();
            dbDetail.NotifyParty.TradelaneShipmentAddressId = shipment.NotifyPartyAddressId.HasValue ? shipment.NotifyPartyAddressId.Value : 0;
            dbDetail.IsNotifyPartySameAsReceiver = shipment.IsNotifyPartySameAsReceiver;

            //Airline preference Detail
            dbDetail.AirlinePreference = new TradelaneAirline();
            dbDetail.AirlinePreference.AirlineId = shipment.AirlineId.HasValue ? shipment.AirlineId.Value : 0;
            if (dbDetail.AirlinePreference.AirlineId > 0)
            {
                Airline airLine = dbContext.Airlines.Find(dbDetail.AirlinePreference.AirlineId);
                if (airLine != null)
                {
                    dbDetail.AirlinePreference.AirLineName = airLine.AirLineName;
                    dbDetail.AirlinePreference.AilineCode = airLine.AirlineCode;
                    dbDetail.AirlinePreference.CarrierCode2 = airLine.CarrierCode2;
                    dbDetail.AirlinePreference.CarrierCode3 = airLine.CarrierCode3;
                }
            }

            dbDetail.BatteryDeclarationType = shipment.BatteryDeclarationType;
            dbDetail.CertificateOfOrigin = shipment.CertificateOfOrigin;
            dbDetail.CreatedBy = shipment.CreatedBy;

            var userInfo = (from r in dbContext.Users
                            join tz in dbContext.Timezones on r.TimezoneId equals tz.TimezoneId
                            where r.UserId == shipment.CreatedBy
                            select tz
                        ).FirstOrDefault();

            var UserTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userInfo.Name);
            dbDetail.CreatedOnUtc = UtilityRepository.UtcDateToOtherTimezone(shipment.CreatedOnUtc, shipment.CreatedOnUtc.TimeOfDay, UserTimeZoneInfo).Item1;
            dbDetail.CreatedOnTime = UtilityRepository.GetFormattedTimeFromString(UtilityRepository.UtcDateToOtherTimezone(shipment.CreatedOnUtc, shipment.CreatedOnUtc.TimeOfDay, UserTimeZoneInfo).Item2);

            // Append the created by time zone in time 
            dbDetail.CreatedOnTime += " " + userInfo.OffsetShort;
            dbDetail.CustomerId = shipment.CustomerId;
            dbDetail.DangerousGoods = shipment.DangerousGoods.HasValue ? shipment.DangerousGoods.Value : false;

            //Shipment Currency Detail
            dbDetail.DeclaredCurrency = new CurrencyType();
            dbDetail.DeclaredCurrency.CurrencyCode = shipment.DeclaredCurrencyCode;

            if (!string.IsNullOrEmpty(dbDetail.DeclaredCurrency.CurrencyCode))
            {
                CurrencyType currency = dbContext.CurrencyTypes.Where(p => p.CurrencyCode == dbDetail.DeclaredCurrency.CurrencyCode).FirstOrDefault();
                if (currency != null)
                {
                    dbDetail.DeclaredCurrency.CurrencyDescription = currency.CurrencyDescription;
                }
            }

            dbDetail.DeclaredValue = shipment.DeclaredValue;

            //Departure Detail
            dbDetail.DepartureAirport = new TradelaneAirport();
            dbDetail.DepartureAirport.AirportCode = shipment.DepartureAirportCode;
            if (!string.IsNullOrEmpty(dbDetail.DepartureAirport.AirportCode))
            {
                Airport departureAirport = dbContext.Airports.Where(p => p.AirportCode == dbDetail.DepartureAirport.AirportCode).FirstOrDefault();
                dbDetail.DepartureAirport.AirportCodeId = departureAirport.AirportCodeId;
                dbDetail.DepartureAirport.AirportName = departureAirport.AirportName;
                dbDetail.DepartureAirport.CountryId = departureAirport.CountryId;
            }

            //Destination Airport Detail
            dbDetail.DestinationAirport = new TradelaneAirport();
            dbDetail.DestinationAirport.AirportCode = shipment.DestinationAirportCode;
            ; if (!string.IsNullOrEmpty(dbDetail.DestinationAirport.AirportCode))
            {
                Airport destinationAirport = dbContext.Airports.Where(p => p.AirportCode == dbDetail.DestinationAirport.AirportCode).FirstOrDefault();
                dbDetail.DestinationAirport.AirportCodeId = destinationAirport.AirportCodeId;
                dbDetail.DestinationAirport.AirportName = destinationAirport.AirportName;
                dbDetail.DestinationAirport.CountryId = destinationAirport.CountryId;
            }

            dbDetail.ExportLicenceNo = shipment.ExportLicenceNo;
            dbDetail.FrayteNumber = shipment.FrayteNumber;
            dbDetail.LogisticType = shipment.LogisticType;
            dbDetail.ManifestName = shipment.ManifestName;
            dbDetail.MAWB = shipment.MAWB;
            dbDetail.MAWBAgentId = shipment.MAWBAgentId.HasValue ? shipment.MAWBAgentId.Value : 0;
            dbDetail.OperationZoneId = shipment.OperationZoneId;
            dbDetail.PakageCalculatonType = shipment.PackageCalculatonType;
            dbDetail.PayTaxAndDuties = shipment.PaymentPartyTaxAndDuty;
            dbDetail.ShipmentDescription = shipment.ShipmentDescription;

            //Shipment Handler Method Detail
            dbDetail.ShipmentHandlerMethod = new TradelaneShipmentHandlerMethod();
            dbDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId = shipment.ShipmentHandlerMethodId.HasValue ? shipment.ShipmentHandlerMethodId.Value : 0;
            if (dbDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId > 0)
            {
                ShipmentHandlerMethod shipmentHandlerMethod = dbContext.ShipmentHandlerMethods.Find(dbDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId);
                if (shipmentHandlerMethod != null)
                {
                    dbDetail.ShipmentHandlerMethod.ShipmentHandlerMethodDisplay = shipmentHandlerMethod.ShipmentHandlerMethodDisplay;
                    dbDetail.ShipmentHandlerMethod.ShipmentHandlerMethodName = shipmentHandlerMethod.ShipmentHandlerMethodName;
                    dbDetail.ShipmentHandlerMethod.ShipmentHandlerMethodType = shipmentHandlerMethod.ShipmentHandlerMethodType;
                    dbDetail.ShipmentHandlerMethod.ShipmentHandlerMethodCode = shipmentHandlerMethod.ShipmentHandlerMethodCode;
                    dbDetail.ShipmentHandlerMethod.DisplayName = shipmentHandlerMethod.DisplayName;
                }
            }

            dbDetail.ShipmentReference = shipment.ShipmentReference;
            dbDetail.ShipmentStatusId = shipment.ShipmentStatusId;
            dbDetail.TaxAndDutiesAcceptedBy = shipment.TaxAndDutiesAcceptedBy;
            dbDetail.TaxAndDutiesAccountNo = shipment.TaxAndDutiesAccountNo;

            dbDetail.TotalEstimatedWeight = shipment.TotalEstimatedWeight.HasValue ? shipment.TotalEstimatedWeight.Value : 0;
            dbDetail.UpdatedBy = shipment.UpdatedBy.HasValue ? shipment.UpdatedBy.Value : 0;
            dbDetail.UpdatedOnUtc = shipment.UpdatedOnUtc;
            dbDetail.AdditionalInfo = shipment.AdditionalInfo;
            dbDetail.Incoterm = new FrayteIncoterm();
            dbDetail.Incoterm.IncotermID = shipment.IncotermId.HasValue ? shipment.IncotermId.Value : 0;
        }

        //Shipment Address
        private void GetDirectShipmentAddressDetail(TradelaneBooking dbDetail)
        {
            // Ship From 
            TradelaneShipmentAddress shipFrom = dbContext.TradelaneShipmentAddresses.Find(dbDetail.ShipFrom.TradelaneShipmentAddressId);
            if (shipFrom != null)
            {
                dbDetail.ShipFrom.Address = shipFrom.Address1;
                dbDetail.ShipFrom.Address2 = shipFrom.Address2;
                dbDetail.ShipFrom.Area = shipFrom.Area;
                dbDetail.ShipFrom.City = shipFrom.City;
                dbDetail.ShipFrom.CompanyName = shipFrom.CompanyName;
                dbDetail.ShipFrom.FirstName = shipFrom.ContactFirstName;
                dbDetail.ShipFrom.LastName = shipFrom.ContactLastName;
                dbDetail.ShipFrom.Phone = shipFrom.PhoneNo;
                dbDetail.ShipFrom.PostCode = shipFrom.Zip;
                dbDetail.ShipFrom.State = shipFrom.State;
                dbDetail.ShipFrom.Email = shipFrom.Email;
                dbDetail.ShipFrom.Country = new FrayteCountryCode();
                dbDetail.ShipFrom.Country.CountryId = shipFrom.CountryId;
                if (dbDetail.ShipFrom.Country.CountryId > 0)
                {
                    Country shipFromCountry = dbContext.Countries.Find(dbDetail.ShipFrom.Country.CountryId);
                    if (shipFromCountry != null)
                    {
                        dbDetail.ShipFrom.Country.Code = shipFromCountry.CountryCode;
                        dbDetail.ShipFrom.Country.Code2 = shipFromCountry.CountryCode2;
                        dbDetail.ShipFrom.Country.Name = shipFromCountry.CountryName;
                        dbDetail.ShipFrom.Country.CountryPhoneCode = shipFromCountry.CountryPhoneCode;
                    }
                }

            }

            // Ship To 
            TradelaneShipmentAddress shipTo = dbContext.TradelaneShipmentAddresses.Find(dbDetail.ShipTo.TradelaneShipmentAddressId);
            if (shipTo != null)
            {
                dbDetail.ShipTo.Address = shipTo.Address1;
                dbDetail.ShipTo.Address2 = shipTo.Address2;
                dbDetail.ShipTo.Area = shipTo.Area;
                dbDetail.ShipTo.City = shipTo.City;
                dbDetail.ShipTo.CompanyName = shipTo.CompanyName;
                dbDetail.ShipTo.FirstName = shipTo.ContactFirstName;
                dbDetail.ShipTo.LastName = shipTo.ContactLastName;
                dbDetail.ShipTo.Phone = shipTo.PhoneNo;
                dbDetail.ShipTo.PostCode = shipTo.Zip;
                dbDetail.ShipTo.State = shipTo.State;
                dbDetail.ShipTo.Email = shipTo.Email;
                dbDetail.ShipTo.Country = new FrayteCountryCode();
                dbDetail.ShipTo.Country.CountryId = shipTo.CountryId;
                if (dbDetail.ShipTo.Country.CountryId > 0)
                {
                    Country shipToCountry = dbContext.Countries.Find(dbDetail.ShipTo.Country.CountryId);
                    if (shipToCountry != null)
                    {
                        dbDetail.ShipTo.Country.Code = shipToCountry.CountryCode;
                        dbDetail.ShipTo.Country.Code2 = shipToCountry.CountryCode2;
                        dbDetail.ShipTo.Country.Name = shipToCountry.CountryName;
                        dbDetail.ShipTo.Country.CountryPhoneCode = shipToCountry.CountryPhoneCode;
                    }
                }
            }
            if (!dbDetail.IsNotifyPartySameAsReceiver)
            {
                // Notify Party 
                TradelaneShipmentAddress notifyParty = dbContext.TradelaneShipmentAddresses.Find(dbDetail.NotifyParty.TradelaneShipmentAddressId);
                if (notifyParty != null)
                {
                    dbDetail.NotifyParty.Address = notifyParty.Address1;
                    dbDetail.NotifyParty.Address2 = notifyParty.Address2;
                    dbDetail.NotifyParty.Area = notifyParty.Area;
                    dbDetail.NotifyParty.City = notifyParty.City;
                    dbDetail.NotifyParty.CompanyName = notifyParty.CompanyName;
                    dbDetail.NotifyParty.FirstName = notifyParty.ContactFirstName;
                    dbDetail.NotifyParty.LastName = notifyParty.ContactLastName;
                    dbDetail.NotifyParty.Phone = notifyParty.PhoneNo;
                    dbDetail.NotifyParty.PostCode = notifyParty.Zip;
                    dbDetail.NotifyParty.State = notifyParty.State;
                    dbDetail.NotifyParty.Email = notifyParty.Email;
                    dbDetail.NotifyParty.Country = new FrayteCountryCode();
                    dbDetail.NotifyParty.Country.CountryId = notifyParty.CountryId;
                    if (dbDetail.NotifyParty.Country.CountryId > 0)
                    {
                        Country notifyPartyCountry = dbContext.Countries.Find(dbDetail.NotifyParty.Country.CountryId);
                        if (notifyPartyCountry != null)
                        {
                            dbDetail.NotifyParty.Country.Code = notifyPartyCountry.CountryCode;
                            dbDetail.NotifyParty.Country.Code2 = notifyPartyCountry.CountryCode2;
                            dbDetail.NotifyParty.Country.Name = notifyPartyCountry.CountryName;
                            dbDetail.NotifyParty.Country.CountryPhoneCode = notifyPartyCountry.CountryPhoneCode;
                        }
                    }
                }
            }
        }

        //package Details
        private void GetDirectShipmentPackagesDetail(TradelaneBooking dbDetail, string CallingType)
        {
            dbDetail.HAWBPackages = GetGroupedHAWBPieces(dbDetail.TradelaneShipmentId);
        }

        #endregion

        internal FrayteSalesRepresentiveEmail GetAssociateStaffDetail(int customerId, int createdBy)
        {

            FrayteSalesRepresentiveEmail detail = new FrayteSalesRepresentiveEmail();

            var RoleId = dbContext.UserRoles.Where(p => p.UserId == createdBy).FirstOrDefault().RoleId;
            if (RoleId == (int)FrayteUserRole.Admin || RoleId == (int)FrayteUserRole.Customer)
            {
                detail = (from US1 in dbContext.Users
                          join US2 in dbContext.UserAdditionals on US1.UserId equals US2.UserId
                          join uadd in dbContext.UserAddresses on US1.UserId equals uadd.UserId
                          join c in dbContext.Countries on uadd.CountryId equals c.CountryId
                          join US3 in dbContext.Users on US2.OperationUserId equals US3.UserId
                          where US1.UserId == customerId
                          select new FrayteSalesRepresentiveEmail
                          {
                              OperationStaffName = US3.ContactName,
                              OperationStaffEmail = US3.Email,
                              UserPhone = !string.IsNullOrEmpty(c.CountryPhoneCode) ? "(" + c.CountryPhoneCode + ") " + US1.TelephoneNo : US1.TelephoneNo,
                              DeptName = "Operation Staff"
                          }).FirstOrDefault();
            }
            else if (RoleId == 6)
            {
                detail = (from US1 in dbContext.Users
                          join US2 in dbContext.UserAdditionals on US1.UserId equals US2.UserId
                          join uad in dbContext.UserAddresses on US1.UserId equals uad.UserId
                          join c in dbContext.Countries on uad.CountryId equals c.CountryId
                          join US3 in dbContext.UserRoles on US2.UserId equals US3.UserId
                          where US1.UserId == createdBy &&
                                US3.RoleId == RoleId
                          select new FrayteSalesRepresentiveEmail
                          {
                              OperationStaffName = US1.ContactName,
                              UserPhone = !string.IsNullOrEmpty(c.CountryPhoneCode) ? "(" + c.CountryPhoneCode + ") " + US1.TelephoneNo : US1.TelephoneNo,
                              OperationStaffEmail = US1.Email,
                              DeptName = "Operation Staff"
                          }).FirstOrDefault();
            }

            return detail;
        }

        #region Shipment Document

        public List<TradelaneMAWBShipmentDocument> GetMAWBDocuments(int shipmentId, string mAWB)
        {

            List<TradelaneMAWBShipmentDocument> list = dbContext.TradelaneShipmentDocuments.Where(p => p.TradelaneShipmentId == shipmentId && p.DocumentType == mAWB)
                .OrderByDescending(p => p.RevisionNumber)
                .Select(p => new TradelaneMAWBShipmentDocument
                {
                    FileName = p.DocumentName,
                    RevisionNumber = p.RevisionNumber.HasValue ? p.RevisionNumber.Value : 0
                })
                .ToList();

            if (list.Count > 0)
            {
                return list;
            }
            else
            {
                return null;
            }
        }

        public FrayteResult SaveShipmentDocument(int shipmentId, string batteryForm, string fileName, int userId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                TradelaneShipmentDocument shipmentDoc;
                if (batteryForm == FrayteTradelaneShipmentDocumentEnum.MAWB)
                {
                    var list = dbContext.TradelaneShipmentDocuments.Where(p => p.TradelaneShipmentId == shipmentId && p.DocumentType == batteryForm).OrderByDescending(p => p.RevisionNumber).ToList();

                    int num = 0;
                    if (list.Count > 0)
                    {
                        num = (list[0].RevisionNumber.HasValue ? list[0].RevisionNumber.Value : 0) + 1;
                    }
                    else
                    {
                        num += 1;
                    }

                    shipmentDoc = new TradelaneShipmentDocument();
                    shipmentDoc.DocumentName = fileName;
                    shipmentDoc.DocumentType = batteryForm;
                    shipmentDoc.DocumentNameDisplay = batteryForm;
                    shipmentDoc.RevisionNumber = num;
                    shipmentDoc.UploadedOnUtc = DateTime.UtcNow;
                    shipmentDoc.UploadedBy = userId;
                    shipmentDoc.TradelaneShipmentId = shipmentId;
                    dbContext.TradelaneShipmentDocuments.Add(shipmentDoc);
                    dbContext.SaveChanges();
                    result.Status = true;

                }
                else
                {
                    shipmentDoc = new TradelaneShipmentDocument();
                    shipmentDoc.DocumentName = fileName;
                    shipmentDoc.DocumentType = batteryForm;
                    shipmentDoc.DocumentNameDisplay = batteryForm;
                    shipmentDoc.UploadedOnUtc = DateTime.UtcNow;
                    shipmentDoc.UploadedBy = userId;
                    shipmentDoc.TradelaneShipmentId = shipmentId;
                    dbContext.TradelaneShipmentDocuments.Add(shipmentDoc);
                    dbContext.SaveChanges();
                    result.Status = true;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
            }
            return result;
        }

        public List<TradelanePreAlertDocument> GetShipmentDocuments(int shipmentId)
        {
            var shipmentDetail = dbContext.TradelaneShipments.Find(shipmentId);

            var data = (from r in dbContext.TradelaneShipmentDocuments
                        select r).Where(p => p.TradelaneShipmentId == shipmentId).GroupBy(x => x.DocumentType)
                                  .Select(group => new TradelanePreAlertDocument
                                  {
                                      DocumentType = group.FirstOrDefault().DocumentType,
                                      DocumentTypeDisplay = group.FirstOrDefault().DocumentNameDisplay,
                                      Documents = group.Select(subgroup => new TradelanePreAlertDoc
                                      {
                                          Document = subgroup.DocumentName,
                                          FileName = subgroup.DocumentName,
                                          FilePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + shipmentId + "/" + subgroup.DocumentName,
                                          TradelaneShipmentDocumentId = subgroup.TradelaneShipmentDocumentId,
                                          IsSelected = false,
                                          RevNumber = subgroup.RevisionNumber.HasValue ? subgroup.RevisionNumber.Value : 0
                                      }).OrderByDescending(p => p.RevNumber).ToList()
                                  }).ToList();

            return data;

        }

        public List<TradelanePreAlertDocument> GetShipmentOtherDocuments(int shipmentId)
        {
            var data = (from r in dbContext.TradelaneShipmentDocuments
                        select r).Where(p => p.TradelaneShipmentId == shipmentId && p.DocumentType == FrayteShipmentDocumentType.OtherDocument).GroupBy(x => x.DocumentType)
                                 .Select(group => new TradelanePreAlertDocument
                                 {
                                     DocumentType = group.FirstOrDefault().DocumentType,
                                     DocumentTypeDisplay = group.FirstOrDefault().DocumentNameDisplay,
                                     Documents = group.Select(subgroup => new TradelanePreAlertDoc
                                     {
                                         Document = subgroup.DocumentName,
                                         FileName = subgroup.DocumentName,
                                         FilePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + shipmentId + "/" + subgroup.DocumentName,
                                         TradelaneShipmentDocumentId = subgroup.TradelaneShipmentDocumentId,
                                         IsSelected = false
                                     }).ToList()
                                 }).OrderBy(p => p.DocumentType).ToList();

            return data;
        }

        public List<TradelanePreAlertDocument> TradelaneShipmenDocuments(int userId, int shipmentId)
        {
            var userDetail = dbContext.UserRoles.Where(p => p.UserId == userId).FirstOrDefault();

            List<TradelanePreAlertDocument> list = new List<TradelanePreAlertDocument>();

            if (userDetail != null)
            {
                var shipmentDetail = dbContext.TradelaneShipments.Find(shipmentId);
                var documents = dbContext.TradelaneShipmentDocuments.Where(p => p.TradelaneShipmentId == shipmentId).ToList();

                // MAWB

                TradelanePreAlertDocument mawbDocument = new TradelanePreAlertDocument();

                mawbDocument.DocumentType = FrayteTradelaneShipmentDocumentEnum.MAWB;
                mawbDocument.DocumentTypeDisplay = FrayteTradelaneShipmentDocumentEnum.MAWBDisplay;
                mawbDocument.OrderNumber = 1;
                mawbDocument.Documents = new List<TradelanePreAlertDoc>();
                if (documents.Count > 0)
                {
                    if (userDetail.RoleId == (int)FrayteUserRole.Customer)
                    {
                        mawbDocument.Documents = documents.Where(p => p.DocumentType == FrayteTradelaneShipmentDocumentEnum.MAWB).Select(p => new TradelanePreAlertDoc
                        {
                            TradelaneShipmentDocumentId = p.TradelaneShipmentDocumentId,
                            OrderNumber = 0,
                            Document = p.DocumentName,
                            FileName = p.DocumentName,
                            FilePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + shipmentId + "/" + p.DocumentName,
                            IsSelected = false,
                            RevNumber = p.RevisionNumber.HasValue ? p.RevisionNumber.Value : 0
                        }).OrderByDescending(p => p.RevNumber).Take(1).ToList();
                    }
                    else
                    {
                        mawbDocument.Documents = documents.Where(p => p.DocumentType == FrayteTradelaneShipmentDocumentEnum.MAWB).Select(p => new TradelanePreAlertDoc
                        {
                            TradelaneShipmentDocumentId = p.TradelaneShipmentDocumentId,
                            OrderNumber = 0,
                            Document = p.DocumentName,
                            FileName = p.DocumentName,
                            FilePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + shipmentId + "/" + p.DocumentName,
                            IsSelected = false,
                            RevNumber = p.RevisionNumber.HasValue ? p.RevisionNumber.Value : 0
                        }).OrderByDescending(p => p.RevNumber).ToList();
                    }
                }
                list.Add(mawbDocument);

                // HAWB  
                TradelanePreAlertDocument hawbDocument = new TradelanePreAlertDocument();
                hawbDocument.DocumentType = FrayteTradelaneShipmentDocumentEnum.HAWB;
                hawbDocument.DocumentTypeDisplay = FrayteTradelaneShipmentDocumentEnum.HAWBDisplay;
                hawbDocument.OrderNumber = 2;
                hawbDocument.Documents = new List<TradelanePreAlertDoc>();

                if (shipmentDetail.ShipmentStatusId != (int)FrayteShipmentStatus.Delivered)
                {
                    hawbDocument.Documents = GetGroupedHAWBPieces(shipmentId).Select(p => new TradelanePreAlertDoc
                    {
                        Document = p.HAWB,
                        FileName = p.HAWB,
                        FilePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + shipmentId + "/" + p.HAWB,
                        IsSelected = false,
                        OrderNumber = 0,
                        RevNumber = 0,
                        TradelaneShipmentDocumentId = 0

                    }).ToList();
                }
                else
                {
                    hawbDocument.Documents = documents.Where(p => p.DocumentType == FrayteTradelaneShipmentDocumentEnum.HAWB).Select(p => new TradelanePreAlertDoc
                    {
                        TradelaneShipmentDocumentId = p.TradelaneShipmentDocumentId,
                        OrderNumber = 0,
                        Document = p.DocumentName,
                        FileName = p.DocumentName,
                        FilePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + shipmentId + "/" + p.DocumentName,
                        IsSelected = false,
                        RevNumber = p.RevisionNumber.HasValue ? p.RevisionNumber.Value : 0
                    }).OrderByDescending(p => p.RevNumber).ToList();
                }

                list.Add(hawbDocument);

                // Manifest

                TradelanePreAlertDocument manifestDocument = new TradelanePreAlertDocument();
                manifestDocument.DocumentType = FrayteTradelaneShipmentDocumentEnum.Manifest;
                manifestDocument.DocumentTypeDisplay = FrayteTradelaneShipmentDocumentEnum.ManifestDisplay;
                manifestDocument.OrderNumber = 3;
                manifestDocument.Documents = new List<TradelanePreAlertDoc>();
                if (documents.Count > 0)
                {
                    manifestDocument.Documents = documents.Where(p => p.DocumentType == FrayteTradelaneShipmentDocumentEnum.Manifest).Select(p => new TradelanePreAlertDoc
                    {
                        TradelaneShipmentDocumentId = p.TradelaneShipmentDocumentId,
                        OrderNumber = 0,
                        Document = p.DocumentName,
                        FileName = p.DocumentName,
                        FilePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + shipmentId + "/" + p.DocumentName,
                        IsSelected = false,
                        RevNumber = p.RevisionNumber.HasValue ? p.RevisionNumber.Value : 0
                    }).OrderByDescending(p => p.RevNumber).ToList();
                }
                list.Add(manifestDocument);

                // ShipmentDetail

                TradelanePreAlertDocument shipmentDocument = new TradelanePreAlertDocument();
                shipmentDocument.DocumentType = FrayteTradelaneShipmentDocumentEnum.ShipmentDetail;
                shipmentDocument.DocumentTypeDisplay = FrayteTradelaneShipmentDocumentEnum.ShipmentDetailDisplay;
                shipmentDocument.OrderNumber = 4;

                shipmentDocument.Documents = new List<TradelanePreAlertDoc>();

                if (documents.Count > 0)
                {
                    shipmentDocument.Documents = documents.Where(p => p.DocumentType == FrayteTradelaneShipmentDocumentEnum.ShipmentDetail).Select(p => new TradelanePreAlertDoc
                    {
                        TradelaneShipmentDocumentId = p.TradelaneShipmentDocumentId,
                        OrderNumber = 0,
                        Document = p.DocumentName,
                        FileName = p.DocumentName,
                        FilePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + shipmentId + "/" + p.DocumentName,
                        IsSelected = false,
                        RevNumber = p.RevisionNumber.HasValue ? p.RevisionNumber.Value : 0
                    }).OrderByDescending(p => p.RevNumber).ToList();
                }

                list.Add(shipmentDocument);

                // Battery Declaration
                if (shipmentDetail.BatteryDeclarationType != "None")
                {
                    TradelanePreAlertDocument battreyDocument = new TradelanePreAlertDocument();
                    battreyDocument.DocumentType = FrayteTradelaneShipmentDocumentEnum.BatteryForm;
                    battreyDocument.DocumentTypeDisplay = FrayteTradelaneShipmentDocumentEnum.BatteryFormDisplay;
                    battreyDocument.OrderNumber = 6;
                    battreyDocument.Documents = new List<TradelanePreAlertDoc>();
                    if (documents.Count > 0)
                    {
                        battreyDocument.Documents = documents.Where(p => p.DocumentType == FrayteTradelaneShipmentDocumentEnum.BatteryForm).Select(p => new TradelanePreAlertDoc
                        {
                            TradelaneShipmentDocumentId = p.TradelaneShipmentDocumentId,
                            OrderNumber = 0,
                            Document = p.DocumentName,
                            FileName = p.DocumentName,
                            FilePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + shipmentId + "/" + p.DocumentName,
                            IsSelected = false,
                            RevNumber = p.RevisionNumber.HasValue ? p.RevisionNumber.Value : 0
                        }).OrderByDescending(p => p.RevNumber).ToList();
                    }
                    list.Add(battreyDocument);
                }

                //Coload form

                TradelanePreAlertDocument coloadDocument = new TradelanePreAlertDocument();
                coloadDocument.DocumentType = FrayteTradelaneShipmentDocumentEnum.CoLoadForm;
                coloadDocument.DocumentTypeDisplay = FrayteTradelaneShipmentDocumentEnum.CoLoadFormDisplay;
                coloadDocument.OrderNumber = 7;
                coloadDocument.Documents = new List<TradelanePreAlertDoc>();
                if (documents.Count > 0)
                {
                    coloadDocument.Documents = documents.Where(p => p.DocumentType == FrayteTradelaneShipmentDocumentEnum.CoLoadForm).Select(p => new TradelanePreAlertDoc
                    {
                        TradelaneShipmentDocumentId = p.TradelaneShipmentDocumentId,
                        OrderNumber = 0,
                        Document = p.DocumentName,
                        FileName = p.DocumentName,
                        FilePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + shipmentId + "/" + p.DocumentName,
                        IsSelected = false,
                        RevNumber = p.RevisionNumber.HasValue ? p.RevisionNumber.Value : 0
                    }).OrderByDescending(p => p.RevNumber).ToList();
                }

                list.Add(coloadDocument);


                // Export Manifest 


                // Driver Manifest 
                TradelanePreAlertDocument driverManifestDocument = new TradelanePreAlertDocument();
                driverManifestDocument.DocumentType = FrayteTradelaneShipmentDocumentEnum.DriverManifest;
                driverManifestDocument.DocumentTypeDisplay = FrayteTradelaneShipmentDocumentEnum.DriverManifestDisplay;
                driverManifestDocument.OrderNumber = 8;
                driverManifestDocument.Documents = new List<TradelanePreAlertDoc>();
                if (documents.Count > 0)
                {
                    driverManifestDocument.Documents = documents.Where(p => p.DocumentType == FrayteTradelaneShipmentDocumentEnum.DriverManifest).Select(p => new TradelanePreAlertDoc
                    {
                        TradelaneShipmentDocumentId = p.TradelaneShipmentDocumentId,
                        OrderNumber = 0,
                        Document = p.DocumentName,
                        FileName = p.DocumentName,
                        FilePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + shipmentId + "/" + p.DocumentName,
                        IsSelected = false,
                        RevNumber = p.RevisionNumber.HasValue ? p.RevisionNumber.Value : 0
                    }).OrderByDescending(p => p.RevNumber).ToList();
                }

                list.Add(driverManifestDocument);

                // Export Manifest 
                TradelanePreAlertDocument exportManifestDocument = new TradelanePreAlertDocument();
                exportManifestDocument.DocumentType = FrayteTradelaneShipmentDocumentEnum.ExportManifest;
                exportManifestDocument.DocumentTypeDisplay = FrayteTradelaneShipmentDocumentEnum.ExportManifestDisplay;
                exportManifestDocument.OrderNumber = 8;
                exportManifestDocument.Documents = new List<TradelanePreAlertDoc>();
                if (documents.Count > 0)
                {
                    exportManifestDocument.Documents = documents.Where(p => p.DocumentType == FrayteTradelaneShipmentDocumentEnum.BagLabel).Select(p => new TradelanePreAlertDoc
                    {
                        TradelaneShipmentDocumentId = p.TradelaneShipmentDocumentId,
                        OrderNumber = 0,
                        Document = p.DocumentName,
                        FileName = p.DocumentName,
                        FilePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + shipmentId + "/" + p.DocumentName,
                        IsSelected = false,
                        RevNumber = p.RevisionNumber.HasValue ? p.RevisionNumber.Value : 0
                    }).OrderByDescending(p => p.RevNumber).ToList();
                }

                list.Add(exportManifestDocument);

                //Other Document 
                TradelanePreAlertDocument otherDocument = new TradelanePreAlertDocument();
                otherDocument.DocumentType = FrayteTradelaneShipmentDocumentEnum.OtherDocument;
                otherDocument.DocumentTypeDisplay = FrayteTradelaneShipmentDocumentEnum.OtherDocumentDisplay;
                otherDocument.OrderNumber = 8;
                otherDocument.Documents = new List<TradelanePreAlertDoc>();
                if (documents.Count > 0)
                {
                    otherDocument.Documents = documents.Where(p => p.DocumentType == FrayteTradelaneShipmentDocumentEnum.OtherDocument).Select(p => new TradelanePreAlertDoc
                    {
                        TradelaneShipmentDocumentId = p.TradelaneShipmentDocumentId,
                        OrderNumber = 0,
                        Document = p.DocumentName,
                        FileName = p.DocumentName,
                        FilePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + shipmentId + "/" + p.DocumentName,
                        IsSelected = false,
                        RevNumber = p.RevisionNumber.HasValue ? p.RevisionNumber.Value : 0
                    }).OrderByDescending(p => p.RevNumber).ToList();
                }

                list.Add(otherDocument);
            }

            return list;
        }

        #endregion

        #region MAWB Email

        public void SendMAWBEmail()
        {

            new MawbAllocationRepository().GetMawbUnallocatedShipments();
        }

        public DirectBookingShipmentDetail GetDirectBookingShipment(string FrayteRefNo)
        {
            DirectBookingShipmentDetail DSD = new DirectBookingShipmentDetail();
            var shipment = dbContext.DirectShipments.Where(a => a.FrayteNumber == FrayteRefNo).FirstOrDefault();

            if (shipment != null && shipment.LogisticServiceType == "DHL")
            {
                var result = (from ds in dbContext.DirectShipments
                              join dss in dbContext.DirectShipmentDetails on ds.DirectShipmentId equals dss.DirectShipmentId
                              join dTD in dbContext.PackageTrackingDetails on dss.DirectShipmentDetailId equals dTD.DirectShipmentDetailId
                              where shipment.DirectShipmentId == ds.DirectShipmentId
                              select new DirectBookingShipmentDetail
                              {
                                  TrackingNo = dTD.TrackingNo,
                                  LogisticCompany = ds.LogisticServiceType

                              }).FirstOrDefault();
                return result;
            }
            else if (shipment != null)
            {
                var result = (from ds in dbContext.DirectShipments
                              where shipment.DirectShipmentId == ds.DirectShipmentId
                              select new DirectBookingShipmentDetail
                              {
                                  TrackingNo = ds.TrackingDetail,
                                  LogisticCompany = ds.LogisticServiceType

                              }).FirstOrDefault();
                return result;
            }
            else
            {
                return DSD;
            }


        }

        #endregion

        public void SaveIsAgentMawbAllocationDocument(int TradelaneShipmentId)
        {
            var Result = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).FirstOrDefault();
            if (Result != null)
            {
                Result.IsAgentMAWBAllocated = true;
                dbContext.Entry(Result).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public TradelaneApiScanModel CartonScan(string CartonNumber)
        {
            TradelaneApiScanModel TAS = new TradelaneApiScanModel();
            var CartonData = dbContext.TradelaneShipmentDetails.Where(a => a.CartonNumber == CartonNumber).FirstOrDefault();
            if (CartonData != null && (CartonData.IsScaned == false || CartonData.IsScaned == null))
            {
                CartonData.IsScaned = true;
                dbContext.SaveChanges();
            }
            else if (CartonData != null && CartonData.IsScaned == true)
            {
                TAS.ErrorObject = new List<ApiErrorObj>();
                ApiErrorObj AEO = new ApiErrorObj();
                TAS.Status = false;
                AEO.ErrorCode = "Carton Already Scanned";
                AEO.ErrorMessage = "This carton is already scanned";
                TAS.ErrorObject.Add(AEO);
                return TAS;
            }
            ////Checking Mawb No Exist or not
            //var IsMawbExist = (from TSD in dbContext.TradelaneShipmentDetails
            //                   join TS in dbContext.TradelaneShipments on TSD.TradelaneShipmentId equals TS.TradelaneShipmentId
            //                   where TSD.CartonNumber == CartonNumber
            //                   select new
            //                   {
            //                       TS.MAWB
            //                   }).FirstOrDefault();

            ////if Mawb No Exist Only then continue the further process
            //if (IsMawbExist != null && IsMawbExist.MAWB != null && IsMawbExist.MAWB != "")
            //{
            var CartonScanObj = (from TSD in dbContext.TradelaneShipmentDetails
                                 join TS in dbContext.TradelaneShipments on TSD.TradelaneShipmentId equals TS.TradelaneShipmentId
                                 let TSId = TS.TradelaneShipmentId
                                 where TSD.CartonNumber == CartonNumber
                                 select new TradelaneApiScanModel
                                 {
                                     Mawb = TS.MAWB,
                                     HawbNumber = TSD.HAWB,
                                     FrayteNumber = TS.FrayteNumber,
                                     TotalCarton = dbContext.TradelaneShipmentDetails.Where(a => a.HAWB == TSD.HAWB).ToList().Count(),
                                     ScanedCarton = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TSId && a.HAWB == TSD.HAWB && a.IsScaned == true).ToList().Count(),
                                     Length = TSD.Length,
                                     Width = TSD.Width,
                                     Weight = TSD.Weight,
                                     Height = TSD.Height,
                                     CartonValue = TSD.CartonValue.ToString()
                                 }).FirstOrDefault();
            // }
            if (CartonScanObj != null && CartonScanObj.Mawb != null)
            {
                CartonScanObj.CartonNumber = CartonNumber;
                return CartonScanObj;
            }
            else
            {
                TAS.ErrorObject = new List<ApiErrorObj>();
                ApiErrorObj AEO = new ApiErrorObj();
                TAS.Status = false;
                AEO.ErrorCode = "MAWB Not Exist";
                AEO.ErrorMessage = "Mawb not exist for this Carton number";
                TAS.ErrorObject.Add(AEO);
                return TAS;
            }
        }

        public TradelaneDetail GetTradelaneDetail(TradelanePrintLabel PrintObj)
        {
            var Result = (from TSD in dbContext.TradelaneShipmentDetails
                          join TS in dbContext.TradelaneShipments on TSD.TradelaneShipmentId equals TS.TradelaneShipmentId
                          where TSD.HAWB == PrintObj.Hawb
                          select new TradelaneDetail
                          {
                              TradelaneShipmentId = TS.TradelaneShipmentId,
                              TradelaneShipmentDetailId = TSD.TradelaneShipmentDetailId
                          }).FirstOrDefault();
            return Result;
        }

        public FrayteResult HawbUpdate(TradelaneBooking TB, TradelanePrintLabel PrintObj)
        {
            FrayteResult FR = new FrayteResult();
            FR.Status = true;

            var Hawb = dbContext.TradelaneShipmentDetails.Where(a => a.HAWB == PrintObj.Hawb).ToList();
            var IsHawbScanned = false;
            foreach (var hawbdata in Hawb)
            {
                if (hawbdata.IsScaned != true)
                {
                    IsHawbScanned = true;
                    break;
                }
            }

            var Result = dbContext.TradelaneShipmentHawbs.Where(a => a.HawbNumber == PrintObj.Hawb && a.IsScanned == null && a.ScannedBy == null).FirstOrDefault();
            if (Result != null)
            {
                Result.HawbNumber = PrintObj.Hawb;
                Result.PrintedBy = TB.CustomerId;
                Result.TradelaneShipmentId = TB.TradelaneShipmentId;
                Result.IsPrinted = IsHawbScanned;
                dbContext.Entry(Result).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            else
            {
                TradelaneShipmentHawb TSH = new TradelaneShipmentHawb();
                TSH.HawbNumber = PrintObj.Hawb;
                TSH.PrintedOn = DateTime.UtcNow;
                TSH.PrintedBy = TB.CustomerId;
                TSH.TradelaneShipmentId = TB.TradelaneShipmentId;
                TSH.IsPrinted = IsHawbScanned;
                dbContext.TradelaneShipmentHawbs.Add(TSH);
                dbContext.SaveChanges();
            }
            //}
            return FR;
        }

        public TradelaneApiErrorModel ShipmentReceived(string Mawb)
        {
            TradelaneApiErrorModel TAE = new TradelaneApiErrorModel();
            TAE.Status = false;
            var IsScanned = false;
            var Result = dbContext.TradelaneShipments.Where(a => a.MAWB == Mawb).FirstOrDefault();

            if (Result != null)
            {
                TAE.Status = true;
                var ShipmentDetail = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == Result.TradelaneShipmentId).ToList();
                if (ShipmentDetail.Count > 0)
                {

                    foreach (var SD in ShipmentDetail)
                    {
                        if (SD.IsScaned == false || SD.IsScaned == null)
                        {
                            IsScanned = true;
                            break;
                        }
                    }
                    //Mail send code
                    if (IsScanned == true)
                    {
                        try
                        {
                            new TradelaneEmailRepository().ShipmentReceivedMail(Result.TradelaneShipmentId);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                var Tracking = dbContext.TradelaneShipmentTrackings.Where(a => a.TradlaneShipmentId == Result.TradelaneShipmentId && a.TrackingCode == "RCB").FirstOrDefault();
                if (Tracking != null)
                {
                    TradelaneShipmentTracking TST = new TradelaneShipmentTracking();
                    Tracking.TradlaneShipmentId = Result.TradelaneShipmentId;
                    Tracking.TrackingCode = "RCB";
                    Tracking.Pieces = ShipmentDetail.Count;
                    Tracking.Weight = ShipmentDetail.Sum(a => a.Weight);
                    Tracking.TrackingDescription = "Shipment Received at Hub";
                    Tracking.CreatedOnUtc = DateTime.UtcNow;
                    Tracking.CreatedBy = Result.CustomerId;
                    dbContext.Entry(Tracking).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
                else
                {
                    TradelaneShipmentTracking TST = new TradelaneShipmentTracking();
                    TST.TradlaneShipmentId = Result.TradelaneShipmentId;
                    TST.TrackingCode = "RCB";
                    TST.TrackingDescription = "Shipment Received at Hub";
                    TST.Pieces = ShipmentDetail.Count;
                    TST.Weight = ShipmentDetail.Sum(a => a.Weight);
                    TST.CreatedOnUtc = DateTime.UtcNow;
                    TST.CreatedBy = Result.CustomerId;
                    dbContext.TradelaneShipmentTrackings.Add(TST);
                    dbContext.SaveChanges();
                }
                TAE.ErrorObject = new List<ApiErrorObj>();
                ApiErrorObj AEO = new ApiErrorObj();
                TAE.Status = true;
                AEO.ErrorCode = "Tracking Updated";
                AEO.ErrorMessage = "Shipment Received at Hub.";
                TAE.ErrorObject.Add(AEO);
            }
            else
            {
                TAE.ErrorObject = new List<ApiErrorObj>();
                ApiErrorObj AEO = new ApiErrorObj();
                TAE.Status = false;
                AEO.ErrorCode = "MAWB Not Exist";
                AEO.ErrorMessage = "Given mawb does not exist.";
                TAE.ErrorObject.Add(AEO);
            }
            return TAE;
        }

        public TradelaneScannedStatusModel TradelaneScannedStatus(string Mawb)
        {
            TradelaneScannedStatusModel TAE = new TradelaneScannedStatusModel();
            TAE.HawbList = new List<TradelaneHawbStatusModel>();
            var TradelaneDetail = dbContext.TradelaneShipments.Where(a => a.MAWB == Mawb).FirstOrDefault();
            var TradelaneShipmentList = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId).Where(b => b.HAWB != null).GroupBy(a => a.HAWB).ToList();
            if (TradelaneShipmentList != null && TradelaneShipmentList.Count > 0)
                foreach (var TSH in TradelaneShipmentList)
                {
                    var THSM = new TradelaneHawbStatusModel();
                    THSM.CartonList = new List<CartonType>();
                    THSM.Hawb = TSH.Key;
                    foreach (var T in TSH)
                    {
                        CartonType CT = new CartonType();
                        CT.CartonNumber = T.CartonNumber;
                        CT.IsSCanned = T.IsScaned == null || T.IsScaned == false ? false : true;
                        THSM.CartonList.Add(CT);
                    }
                    TAE.HawbList.Add(THSM);
                }
            TAE.ScannedCount = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId && a.IsScaned == true && a.HAWB != null).Count();
            TAE.MissingScanCount = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId && (a.IsScaned == false || a.IsScaned == null) && a.HAWB != null).Count();
            return TAE;
        }

        public TradelaneApiErrorModel TradelaneUpdateStatus(TradelaneUpdateStatusModel UpdateStatus)
        {
            TradelaneApiErrorModel TAEM = new TradelaneApiErrorModel();
            if (!string.IsNullOrEmpty(UpdateStatus.Mawb) && !string.IsNullOrEmpty(UpdateStatus.StatusCode) && !string.IsNullOrEmpty(UpdateStatus.Airport))
            {
                var MAWB = UpdateStatus.Mawb.Trim().Replace(" ", "");
                if (MAWB.Length == 11)
                {
                    char[] charArray = MAWB.ToCharArray();
                    Array.Reverse(charArray);
                    MAWB = new string(charArray);
                    MAWB = (string)MAWB.Substring(0, 8);
                    char[] charArray1 = MAWB.ToCharArray();
                    Array.Reverse(charArray1);
                    MAWB = new string(charArray1);
                    var TradelaneDetail = dbContext.TradelaneShipments.Where(a => a.MAWB == MAWB).FirstOrDefault();
                    if (TradelaneDetail != null)
                    {
                        var UpdateDetail = dbContext.TradelaneShipmentTrackings.Where(a => a.TrackingCode == UpdateStatus.StatusCode && a.AirportCode == UpdateStatus.Airport).FirstOrDefault();
                        if (UpdateDetail != null)
                        {
                            TAEM.ErrorObject = new List<ApiErrorObj>();
                            ApiErrorObj AE = new ApiErrorObj();
                            AE.ErrorCode = "Status exist";
                            AE.ErrorMessage = "Status already for this airport";
                            TAEM.ErrorObject.Add(AE);
                            TAEM.Status = false;
                        }
                        else
                        {
                            TradelaneShipmentTracking TST = new TradelaneShipmentTracking();
                            TST.AirportCode = UpdateStatus.Airport;
                            TST.TradlaneShipmentId = TradelaneDetail.TradelaneShipmentId;
                            TST.TrackingCode = UpdateStatus.StatusCode;
                            TST.TrackingDescription = UpdateStatus.Description;
                            TST.CreatedOnUtc = DateTime.UtcNow;
                            dbContext.TradelaneShipmentTrackings.Add(TST);
                            dbContext.SaveChanges();
                            TAEM.Status = true;
                        }
                    }
                    else
                    {
                        TAEM.ErrorObject = new List<ApiErrorObj>();
                        ApiErrorObj AE = new ApiErrorObj();
                        AE.ErrorCode = "MAWB not exist";
                        AE.ErrorMessage = "Given MAWB does not exist";
                        TAEM.ErrorObject.Add(AE);
                        TAEM.Status = false;
                    }
                }
                else
                {
                    TAEM.ErrorObject = new List<ApiErrorObj>();
                    ApiErrorObj AE = new ApiErrorObj();
                    AE.ErrorCode = "Given Mawb is worng";
                    AE.ErrorMessage = "Given Mawb is worng, please check it again.";
                    TAEM.ErrorObject.Add(AE);
                    TAEM.Status = false;
                }
            }
            else
            {
                TAEM.ErrorObject = new List<ApiErrorObj>();
                ApiErrorObj AE = new ApiErrorObj();
                AE.ErrorCode = "Information missing";
                AE.ErrorMessage = "Some information is missing, please check it again.";
                TAEM.Status = false;
            }
            return TAEM;
        }

        public string GetLastScannedCarton(int TradelaneShipmentDetailId)
        {
            var Name = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneShipmentDetailId && a.IsScaned == true).ToList();
            string Name1 = "";
            if (Name.Count > 0)
            {
                Name.Reverse();
                Name1 = Name.FirstOrDefault().CartonNumber;
            }

            return Name1;
        }

        public TradelaneUpdateHawbModel TradelaneHawbTrackingUpdate(string HAWB, string Email)
        {
            TradelaneUpdateHawbModel TAEM = new TradelaneUpdateHawbModel();
            try
            {
                if (!string.IsNullOrEmpty(HAWB) && !string.IsNullOrEmpty(Email))
                {
                    var TestAgent = (from Usr in dbContext.Users
                                     join UsrAdd in dbContext.UserAddresses on Usr.UserId equals UsrAdd.UserId
                                     join UsrRL in dbContext.UserRoles on Usr.UserId equals UsrRL.UserId
                                     where Usr.Email == Email && (UsrRL.RoleId == 2 || UsrRL.RoleId == 7)
                                     select new
                                     {
                                         AgentType = "Agent",
                                         RoleId = UsrRL.RoleId,
                                         UsrAdd.CountryId,
                                         Usr.UserId
                                     }).FirstOrDefault();
                    if (TestAgent != null && TestAgent.RoleId == 7)
                    {
                        var TradelaneDetail = dbContext.TradelaneShipmentDetails.Where(n => n.HAWB == HAWB).FirstOrDefault();
                        if (TradelaneDetail != null)
                        {
                            var Tradelane = dbContext.TradelaneShipments.Where(n => n.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId).FirstOrDefault();
                            var TotalHawb = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId).GroupBy(a => a.HAWB).Count();
                            var HawbCount = dbContext.TradelaneShipmentHawbs.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId && a.IsScanned == true && (a.ScannedBy != null && a.ScannedBy == TestAgent.UserId)).ToList().Count;
                            var HawbDetail = dbContext.TradelaneShipmentHawbs.Where(a => a.HawbNumber == TradelaneDetail.HAWB && a.IsScanned == true && (a.ScannedBy != null && a.ScannedBy == TestAgent.UserId)).FirstOrDefault();
                            if (HawbDetail == null)
                            {
                                TradelaneShipmentHawb TSH = new TradelaneShipmentHawb();
                                TSH.HawbNumber = TradelaneDetail.HAWB;
                                TSH.TradelaneShipmentId = TradelaneDetail.TradelaneShipmentId;
                                TSH.IsScanned = true;
                                TSH.ScannedBy = TestAgent.UserId;
                                dbContext.TradelaneShipmentHawbs.Add(TSH);
                                dbContext.SaveChanges();
                                TAEM.Status = true;
                                TAEM.TotalHawb = TotalHawb;
                                TAEM.PendingHawb = TotalHawb - dbContext.TradelaneShipmentHawbs.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId && a.IsScanned == true && (a.ScannedBy != null && a.ScannedBy == TestAgent.UserId)).ToList().Count; ;
                                TAEM.TotalHawbCarton = dbContext.TradelaneShipmentDetails.Where(a => a.HAWB == TradelaneDetail.HAWB).Count();
                            }
                            else
                            {
                                TAEM.Status = false;
                                TAEM.ErrorObject = new List<ApiErrorObj>();
                                ApiErrorObj AE = new ApiErrorObj();
                                TAEM.TotalHawb = TotalHawb;
                                TAEM.PendingHawb = TotalHawb - dbContext.TradelaneShipmentHawbs.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId && a.IsScanned == true && (a.ScannedBy != null && a.ScannedBy == TestAgent.UserId)).ToList().Count;
                                TAEM.TotalHawbCarton = dbContext.TradelaneShipmentDetails.Where(a => a.HAWB == TradelaneDetail.HAWB).Count();
                                AE.ErrorCode = "Scanned HAWB";
                                AE.ErrorMessage = "This HAWB has already been scanned.";
                                TAEM.ErrorObject.Add(AE);
                                return TAEM;
                            }

                            if (TotalHawb == dbContext.TradelaneShipmentHawbs.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId && a.IsScanned == true && (a.ScannedBy != null && a.ScannedBy == TestAgent.UserId)).ToList().Count)
                            {
                                TradelaneShipmentTracking TST = new TradelaneShipmentTracking();
                                TST.TrackingCode = TradelaneStatus.WarehouseDeparted;
                                TST.TrackingDescription = "Departed from warehouse";
                                TST.CreatedOnUtc = DateTime.UtcNow;
                                TST.CreatedBy = TestAgent.UserId;
                                TST.Pieces = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId).Count();
                                TST.Weight = Convert.ToInt32(dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId).Sum(a => a.Weight));
                                TST.AirportCode = Tradelane.DepartureAirportCode;
                                TST.TradlaneShipmentId = TradelaneDetail.TradelaneShipmentId;
                                dbContext.TradelaneShipmentTrackings.Add(TST);
                                dbContext.SaveChanges();
                                TAEM.Status = true;
                                TAEM.PendingHawb = 0;
                                TAEM.TotalHawb = TotalHawb;
                                TAEM.TotalHawbCarton = dbContext.TradelaneShipmentDetails.Where(a => a.HAWB == TradelaneDetail.HAWB).Count();
                                return TAEM;
                            }
                        }
                        else
                        {
                            TAEM.ErrorObject = new List<ApiErrorObj>();
                            ApiErrorObj AE = new ApiErrorObj();
                            AE.ErrorCode = "Scanned HAWB";
                            AE.ErrorMessage = "HAWB does not exist";
                            TAEM.ErrorObject.Add(AE);
                            return TAEM;
                        }
                    }
                    else if (TestAgent != null && TestAgent.RoleId == 2)
                    {
                        var TradelaneDetail = (from TSD in dbContext.TradelaneShipmentDetails
                                               join TS in dbContext.TradelaneShipments on TSD.TradelaneShipmentId equals TS.TradelaneShipmentId
                                               join TSFA in dbContext.TradelaneShipmentAddresses on TS.FromAddressId equals TSFA.TradelaneShipmentAddressId
                                               join TSTA in dbContext.TradelaneShipmentAddresses on TS.ToAddressId equals TSTA.TradelaneShipmentAddressId
                                               where TSD.HAWB == HAWB && (TSFA.CountryId == TestAgent.CountryId || TSTA.CountryId == TestAgent.CountryId)
                                               select new
                                               {
                                                   TSD.HAWB,
                                                   TSD.TradelaneShipmentId,
                                                   TS.DepartureAirportCode,
                                                   TS.DestinationAirportCode,
                                                   FromId = TSFA.CountryId,
                                                   ToId = TSTA.CountryId
                                               }).FirstOrDefault();
                        if (TradelaneDetail != null && TradelaneDetail.FromId == TestAgent.CountryId)
                        {
                            var TotalHawb = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId).GroupBy(a => a.HAWB).Count();
                            var HawbCount = dbContext.TradelaneShipmentHawbs.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId && a.IsScanned == true && (a.ScannedBy != null && a.ScannedBy == TestAgent.UserId)).ToList().Count;
                            var HawbDetail = dbContext.TradelaneShipmentHawbs.Where(a => a.HawbNumber == TradelaneDetail.HAWB && a.IsScanned == true && (a.ScannedBy != null && a.ScannedBy == TestAgent.UserId)).FirstOrDefault();
                            if (HawbDetail == null)
                            {
                                TradelaneShipmentHawb TSH = new TradelaneShipmentHawb();
                                TSH.HawbNumber = TradelaneDetail.HAWB;
                                TSH.TradelaneShipmentId = TradelaneDetail.TradelaneShipmentId;
                                TSH.IsScanned = true;
                                TSH.ScannedBy = TestAgent.UserId;
                                dbContext.TradelaneShipmentHawbs.Add(TSH);
                                dbContext.SaveChanges();
                                TAEM.Status = true;
                                TAEM.TotalHawb = TotalHawb;
                                TAEM.PendingHawb = TotalHawb - dbContext.TradelaneShipmentHawbs.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId && a.IsScanned == true && (a.ScannedBy != null && a.ScannedBy == TestAgent.UserId)).ToList().Count;
                                TAEM.TotalHawbCarton = dbContext.TradelaneShipmentDetails.Where(a => a.HAWB == TradelaneDetail.HAWB).Count();
                            }
                            else
                            {
                                TAEM.Status = false;
                                TAEM.ErrorObject = new List<ApiErrorObj>();
                                ApiErrorObj AE = new ApiErrorObj();
                                TAEM.PendingHawb = TotalHawb - dbContext.TradelaneShipmentHawbs.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId && a.IsScanned == true && (a.ScannedBy != null && a.ScannedBy == TestAgent.UserId)).ToList().Count;
                                TAEM.TotalHawbCarton = dbContext.TradelaneShipmentDetails.Where(a => a.HAWB == TradelaneDetail.HAWB).Count();
                                TAEM.TotalHawb = TotalHawb;
                                AE.ErrorCode = "Scanned HAWB";
                                AE.ErrorMessage = "This HAWB has already been scanned.";
                                TAEM.ErrorObject.Add(AE);
                                return TAEM;
                            }
                            if (TotalHawb == dbContext.TradelaneShipmentHawbs.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId && a.IsScanned == true && (a.ScannedBy != null && a.ScannedBy == TestAgent.UserId)).ToList().Count)
                            {
                                TradelaneShipmentTracking TST = new TradelaneShipmentTracking();
                                TST.TrackingCode = TradelaneStatus.AirportArrived;
                                TST.TrackingDescription = "Arrived at Origin Airport";
                                TST.CreatedOnUtc = DateTime.UtcNow;
                                TST.CreatedBy = TestAgent.UserId;
                                TST.Pieces = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId).Count();
                                TST.Weight = Convert.ToInt32(dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId).Sum(a => a.Weight));
                                TST.AirportCode = TradelaneDetail.DepartureAirportCode;
                                TST.TradlaneShipmentId = TradelaneDetail.TradelaneShipmentId;
                                dbContext.TradelaneShipmentTrackings.Add(TST);
                                dbContext.SaveChanges();
                                TAEM.Status = true;
                                TAEM.PendingHawb = 0;
                                TAEM.TotalHawb = TotalHawb;
                                TAEM.TotalHawbCarton = dbContext.TradelaneShipmentDetails.Where(a => a.HAWB == TradelaneDetail.HAWB).Count();
                                return TAEM;
                            }
                        }
                        else if (TradelaneDetail != null && TradelaneDetail.ToId == TestAgent.CountryId)
                        {
                            var TotalHawb = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId).GroupBy(a => a.HAWB).Count();
                            var HawbCount = dbContext.TradelaneShipmentHawbs.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId && a.IsScanned == true && (a.ScannedBy != null && a.ScannedBy == TestAgent.UserId)).ToList().Count;

                            var HawbDetail = dbContext.TradelaneShipmentHawbs.Where(a => a.HawbNumber == TradelaneDetail.HAWB && a.IsScanned == true && (a.ScannedBy != null && a.ScannedBy == TestAgent.UserId)).FirstOrDefault();
                            if (HawbDetail == null)
                            {
                                TradelaneShipmentHawb TSH = new TradelaneShipmentHawb();
                                TSH.HawbNumber = TradelaneDetail.HAWB;
                                TSH.TradelaneShipmentId = TradelaneDetail.TradelaneShipmentId;
                                TSH.IsScanned = true;
                                TSH.ScannedBy = TestAgent.UserId;
                                dbContext.TradelaneShipmentHawbs.Add(TSH);
                                dbContext.SaveChanges();
                                TAEM.TotalHawb = TotalHawb;
                                TAEM.Status = true;
                                TAEM.PendingHawb = TotalHawb - dbContext.TradelaneShipmentHawbs.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId && a.IsScanned == true && (a.ScannedBy != null && a.ScannedBy == TestAgent.UserId)).ToList().Count;
                                TAEM.TotalHawbCarton = dbContext.TradelaneShipmentDetails.Where(a => a.HAWB == TradelaneDetail.HAWB).Count();
                            }
                            else
                            {
                                TAEM.Status = false;
                                TAEM.ErrorObject = new List<ApiErrorObj>();
                                ApiErrorObj AE = new ApiErrorObj();
                                TAEM.PendingHawb = TotalHawb - dbContext.TradelaneShipmentHawbs.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId && a.IsScanned == true && (a.ScannedBy != null && a.ScannedBy == TestAgent.UserId)).ToList().Count;
                                TAEM.TotalHawbCarton = dbContext.TradelaneShipmentDetails.Where(a => a.HAWB == TradelaneDetail.HAWB).Count();
                                AE.ErrorCode = "Scanned HAWB";
                                AE.ErrorMessage = "This HAWB has already been scanned.";
                                TAEM.TotalHawb = TotalHawb;
                                TAEM.ErrorObject.Add(AE);
                                return TAEM;
                            }
                            if (TotalHawb == dbContext.TradelaneShipmentHawbs.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId && a.IsScanned == true && (a.ScannedBy != null && a.ScannedBy == TestAgent.UserId)).ToList().Count)
                            {
                                TradelaneShipmentTracking TST = new TradelaneShipmentTracking();
                                TST.TrackingCode = TradelaneStatus.AirportDeparture;
                                TST.TrackingDescription = "Arrived at Destination Airport";
                                TST.CreatedOnUtc = DateTime.UtcNow;
                                TST.CreatedBy = TestAgent.UserId;
                                TST.Pieces = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId).Count();
                                TST.Weight = Convert.ToInt32(dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneDetail.TradelaneShipmentId).Sum(a => a.Weight));
                                TST.AirportCode = TradelaneDetail.DestinationAirportCode;
                                TST.TradlaneShipmentId = TradelaneDetail.TradelaneShipmentId;
                                dbContext.TradelaneShipmentTrackings.Add(TST);
                                dbContext.SaveChanges();
                                TAEM.Status = true;
                                TAEM.PendingHawb = 0;
                                TAEM.TotalHawbCarton = dbContext.TradelaneShipmentDetails.Where(a => a.HAWB == TradelaneDetail.HAWB).Count();
                                TAEM.TotalHawb = TotalHawb;
                                return TAEM;
                            }
                        }
                        else
                        {
                            TAEM.ErrorObject = new List<ApiErrorObj>();
                            ApiErrorObj AE = new ApiErrorObj();
                            AE.ErrorCode = "Scanned HAWB";
                            AE.ErrorMessage = "HAWB does not exist";
                            TAEM.ErrorObject.Add(AE);
                            return TAEM;
                        }
                    }
                    else
                    {
                        TAEM.ErrorObject = new List<ApiErrorObj>();
                        ApiErrorObj AE = new ApiErrorObj();
                        AE.ErrorCode = "Information missing";
                        AE.ErrorMessage = "Your email is not valid, please check it again.";
                        TAEM.ErrorObject.Add(AE);
                        TAEM.Status = false;
                    }
                }
                else
                {
                    TAEM.ErrorObject = new List<ApiErrorObj>();
                    ApiErrorObj AE = new ApiErrorObj();
                    AE.ErrorCode = "Information missing";
                    AE.ErrorMessage = "Your email or hawb is missing, please check it again.";
                    TAEM.ErrorObject.Add(AE);
                    TAEM.Status = false;
                }
            }
            catch (Exception ex)
            {

            }
            return TAEM;
        }

        public bool SaveTradelaneRef(string TradelaneRefNo, int TradelaneShipmentId)
        {
            var Result = dbContext.TrackingNumberRoutes.Where(a => a.Number == TradelaneRefNo && a.ModuleType == FrayteShipmentServiceType.TradeLaneBooking && a.IsTradelaneRefNumber == true).FirstOrDefault();
            if (Result == null)
            {
                TrackingNumberRoute TNR = new TrackingNumberRoute();
                TNR.Number = TradelaneRefNo;
                TNR.ShipmentId = TradelaneShipmentId;
                TNR.ModuleType = FrayteShipmentServiceType.TradeLaneBooking;
                TNR.IsTradelaneRefNumber = true;
                dbContext.TrackingNumberRoutes.Add(TNR);
                dbContext.SaveChanges();
                return true;
            }
            return true;
        }

        public bool SaveTradelaneMawb(string Mawb, int TradelaneShipmentId)
        {
            var Result = dbContext.TrackingNumberRoutes.Where(a => a.Number == Mawb && a.ModuleType == FrayteShipmentServiceType.TradeLaneBooking && a.IsMAWB == true).FirstOrDefault();
            if (Result == null)
            {
                TrackingNumberRoute TNR = new TrackingNumberRoute();
                TNR.Number = Mawb;
                TNR.ShipmentId = TradelaneShipmentId;
                TNR.ModuleType = FrayteShipmentServiceType.TradeLaneBooking;
                TNR.IsMAWB = true;
                dbContext.TrackingNumberRoutes.Add(TNR);
                dbContext.SaveChanges();
                return true;
            }
            return true;
        }

        public bool SaveTradelaneHawb(string Hawb, int TradelaneShipmentId)
        {
            var Result = dbContext.TrackingNumberRoutes.Where(a => a.Number == Hawb && a.ModuleType == FrayteShipmentServiceType.TradeLaneBooking && a.IsHAWB == true).FirstOrDefault();
            if (Result == null)
            {
                TrackingNumberRoute TNR = new TrackingNumberRoute();
                TNR.Number = Hawb;
                TNR.ShipmentId = TradelaneShipmentId;
                TNR.ModuleType = FrayteShipmentServiceType.TradeLaneBooking;
                TNR.IsHAWB = true;
                dbContext.TrackingNumberRoutes.Add(TNR);
                dbContext.SaveChanges();
                return true;
            }
            return true;
        }

        public bool SaveAirPort(int CountryId, string AirportName, string AirportCode)
        {
            try
            {
                Airport aa = new Airport();
                aa.AirportCode = AirportCode;
                aa.AirportName = AirportName;
                aa.CountryId = CountryId;
                dbContext.Airports.Add(aa);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<TradelanePackage> SendAssignedHAWB(int TradelaneshipmentId)
        {
            List<TradelanePackage> list = new List<TradelanePackage>();

            try
            {
                var collection = (from r in dbContext.TradelaneShipmentDetails
                                  join s in dbContext.TradelaneShipments on r.TradelaneShipmentId equals s.TradelaneShipmentId
                                  where s.TradelaneShipmentId == TradelaneshipmentId
                                  select new TradelanePackage
                                  {
                                      HAWB = r.HAWB,
                                      TradelaneShipmentId = r.TradelaneShipmentId,
                                      TradelaneShipmentDetailId = r.TradelaneShipmentDetailId,
                                      CartonNumber = r.CartonNumber,
                                      CartonValue = r.CartonValue,
                                      Length = r.Length,
                                      Width = r.Weight,
                                      Height = r.Height,
                                      Weight = r.Weight,
                                      IsScanned = r.IsScaned,
                                      PackageCalculatonType = s.PackageCalculatonType
                                  }).ToList();

                if (collection.Count > 0)
                {
                    return collection;
                }
            }
            catch (Exception ex)
            {
                return list;
            }
            return list;
        }

        public bool SavePcsHAWB(List<TradelanePackage> Pcs)
        {
            try
            {
                if (Pcs.Count > 0)
                {
                    foreach (TradelanePackage pp in Pcs)
                    {
                        TradelaneShipmentDetail tsd = new TradelaneShipmentDetail();
                        tsd.TradelaneShipmentId = pp.TradelaneShipmentId;
                        tsd.CartonNumber = pp.CartonNumber;
                        tsd.CartonValue = pp.CartonValue;
                        tsd.Length = pp.Length;
                        tsd.Width = pp.Width;
                        tsd.Height = pp.Height;
                        tsd.Weight = pp.Weight;
                        tsd.HAWB = pp.HAWB;
                        dbContext.TradelaneShipmentDetails.Add(tsd);
                        dbContext.SaveChanges();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ValidateHAWBAddress HAWBAddressExist(string HAWBNo)
        {
            ValidateHAWBAddress address = new ValidateHAWBAddress();

            address = (from tsd in dbContext.TradelaneShipmentDetails
                       where tsd.HAWB == HAWBNo
                       select new ValidateHAWBAddress
                       {
                           ShipToAddressId = tsd.ToAddressId ?? 0,
                           ShipFromAddressId = tsd.FromAddressId ?? 0,
                           NotifyPartyAddressId = tsd.NotifyPartyAddressId ?? 0,
                           TradeLaneShipmentDetailId = tsd.TradelaneShipmentDetailId,
                           IsNotifyPartySameAsReceiver = tsd.IsNotifyPartySameAsReceiver
                       }).FirstOrDefault();

            if (address != null && address.ShipFromAddressId != 0 && address.ShipToAddressId != 0)
            {
                return address;
            }
            else
            {
                return address = null;
            }
        }

        public bool UpdateHAWBAddress(ValidateHAWBAddress address)
        {
            var detail = dbContext.TradelaneShipmentDetails.Find(address.TradeLaneShipmentDetailId);
            if (detail != null)
            {
                detail.ToAddressId = address.ShipToAddressId;
                detail.FromAddressId = address.ShipFromAddressId;
                detail.NotifyPartyAddressId = address.NotifyPartyAddressId;
                detail.IsNotifyPartySameAsReceiver = address.IsNotifyPartySameAsReceiver;
                dbContext.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        #region AvinashCode

        public HawbLabel CreateHawbLabels(string HAWB, int index, int TradelaneShipmentDetailId)
        {
            HawbLabel result = new HawbLabel();

            result = (from tl in dbContext.TradelaneShipments
                      join ts in dbContext.TradelaneShipmentDetails on tl.TradelaneShipmentId equals ts.TradelaneShipmentId
                      join al in dbContext.Airlines on tl.AirlineId equals al.AirlineId
                      where ts.HAWB == HAWB && ts.TradelaneShipmentDetailId == TradelaneShipmentDetailId
                      select new HawbLabel
                      {
                          TradelaneShipmentId = tl.TradelaneShipmentId,
                          MAWBBarcode = tl.AirlineId + " " + tl.MAWB + " 0 00" + index,
                          MAWBBarcodeDisplay = al.AirlineCode + " " + tl.MAWB.Substring(0, 4) + " " + tl.MAWB.Substring(4, 8) + " 0 00" + index,
                          MAWB = al.AirlineCode + " " + tl.MAWB,
                          DestinationAirportCode = tl.DestinationAirportCode,
                          DepartureAirportCode = tl.DepartureAirportCode,
                          HAWBBarCodeDisplay = ts.HAWB,
                          HAWBBarCode = ts.HAWB,
                          HAWB = ts.HAWB,
                          TotalCarton = dbContext.TradelaneShipmentDetails.Where(a => a.HAWB == ts.HAWB).Sum(a => a.CartonValue),
                          HAWBPCS = dbContext.TradelaneShipmentDetails.Where(x => x.TradelaneShipmentId == ts.TradelaneShipmentId).GroupBy(a => a.HAWB).Count()
                      }).FirstOrDefault();

            if (result != null)
            {
                return result;
            }
            return result;
        }

        public List<FrayteHAWBDetail> GetHAWBData(int TradelaneShipmentId)
        {
            List<FrayteHAWBDetail> _listdetail = new List<FrayteHAWBDetail>();

            FrayteHAWBDetail obj;

            var data = dbContext.TradelaneShipmentDetails.Where(x => x.TradelaneShipmentId == TradelaneShipmentId).ToList();

            if (data.Count > 0)
            {
                foreach (var hawb in data)
                {
                    obj = new FrayteHAWBDetail();
                    obj.TradelaneShipmentDetailId = hawb.TradelaneShipmentDetailId;
                    obj.CartonNumber = hawb.CartonNumber;
                    obj.CartonValue = hawb.CartonValue;
                    obj.HAWB = hawb.HAWB;
                    obj.Height = hawb.Height;
                    obj.Length = hawb.Length;
                    obj.Width = hawb.Width;
                    obj.Weight = hawb.Weight;
                    _listdetail.Add(obj);
                }
            }
            return _listdetail;
        }

        public List<ExpressReportDriverManifest> TradeLaneDriverManifest(int TradelaneShipmentId, int userId)
        {
            List<ExpressReportDriverManifest> list = new List<ExpressReportDriverManifest>();
            try
            {
                var user = dbContext.Users.Find(userId);
                string refNO = CommonConversion.GetNewFrayteNumber();
                var customerDetail = (from r in dbContext.Users
                                      join s in dbContext.TradelaneShipments on r.UserId equals s.CustomerId
                                      where s.TradelaneShipmentId == TradelaneShipmentId
                                      select new
                                      {
                                          CustomerName = r.CompanyName,
                                          MAWB = s.MAWB

                                      }).FirstOrDefault();

                var Result = dbContext.ExpressManifests.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).FirstOrDefault();
                var ShipmentDetail = new TradelaneBookingRepository().GetTradelaneBookingDetails(TradelaneShipmentId, "");

                ExpressReportDriverManifest reportObj = new ExpressReportDriverManifest();
                reportObj.Ref = refNO;
                reportObj.Barcode = "MN" + "-" + "TLN-" + ShipmentDetail.DepartureAirport.AirportCode + "-" + customerDetail.MAWB;
                reportObj.DriverManifestName = "Destination Manifest" + "-" + "MN" + "-" + "TLN" + "-" + ShipmentDetail.DepartureAirport.AirportCode + "-" + customerDetail.MAWB + " (" + ShipmentDetail.ShipmentHandlerMethod.DisplayName + ")";

                reportObj.PuickUpAddress = ShipmentDetail.ShipFrom.CompanyName + Environment.NewLine + ShipmentDetail.ShipFrom.Address + Environment.NewLine + ShipmentDetail.ShipFrom.Address2 + Environment.NewLine + ShipmentDetail.ShipFrom.City + "-" + ShipmentDetail.ShipFrom.PostCode + Environment.NewLine + ShipmentDetail.ShipFrom.Country.Name;

                reportObj.PrintedBy = user.ContactName;

                var userInfo = (from r in dbContext.Users
                                join tz in dbContext.Timezones on r.TimezoneId equals tz.TimezoneId
                                where r.UserId == userId
                                select tz
                          ).FirstOrDefault();


                var UserTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userInfo.Name);
                DateTime date = UtilityRepository.UtcDateToOtherTimezone(DateTime.UtcNow, DateTime.UtcNow.TimeOfDay, UserTimeZoneInfo).Item1;
                string time = UtilityRepository.GetFormattedTimeFromString(UtilityRepository.UtcDateToOtherTimezone(DateTime.UtcNow, DateTime.UtcNow.TimeOfDay, UserTimeZoneInfo).Item2);

                reportObj.PrintedDateTime = date.ToString("dd-MMM-yy") + " " + time + " " + userInfo.OffsetShort;

                reportObj.Code = ShipmentDetail.DepartureAirport.AirportCode;
                reportObj.MAWB = ShipmentDetail.AirlinePreference.AilineCode + " " + ShipmentDetail.MAWB.Substring(0, 4) + " " + ShipmentDetail.MAWB.Substring(4, 4);

                reportObj.MAWBChargeableWeight = ShipmentDetail.HAWBPackages.Sum(p => p.TotalWeight * p.TotalCartons);

                reportObj.MAWBGrossWeight = ShipmentDetail.HAWBPackages.Sum(p => p.TotalWeight * p.TotalCartons);

                if (ShipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId == 5)
                {
                    var shipmentAllocation = dbContext.TradelaneShipmentAllocations.Where(p => p.TradelaneShipmentId == TradelaneShipmentId && p.LegNum == "Leg2").OrderByDescending(p => p.TradelaneShipmentAllocationId).FirstOrDefault();

                    reportObj.FlightNumber = shipmentAllocation.FlightNumber;

                    var Airline = dbContext.Airlines.Find(shipmentAllocation.AirlineId);
                    if (Airline != null)
                    {
                        reportObj.Airline = Airline.AirLineName + " - " + Airline.CarrierCode3;
                    }
                    var timezone = dbContext.Timezones.Find(shipmentAllocation.TimezoneId);
                    if (timezone != null)
                    {
                        var TimeZoneInfor = TimeZoneInfo.FindSystemTimeZoneById(userInfo.Name);

                        DateTime date1 = UtilityRepository.UtcDateToOtherTimezone(DateTime.UtcNow, DateTime.UtcNow.TimeOfDay, TimeZoneInfor).Item1;
                        string time1 = UtilityRepository.GetFormattedTimeFromString(UtilityRepository.UtcDateToOtherTimezone(DateTime.UtcNow, DateTime.UtcNow.TimeOfDay, TimeZoneInfor).Item2);

                        reportObj.ETA = date1.ToString("dd-MMM-yy") + " " + time1;
                        reportObj.ETATimeZone = "(" + timezone.OffsetShort + ")";
                    }
                }
                else
                {
                    var shipmentAllocation = dbContext.TradelaneShipmentAllocations.Where(p => p.TradelaneShipmentId == TradelaneShipmentId).OrderByDescending(p => p.TradelaneShipmentAllocationId).FirstOrDefault();
                    reportObj.FlightNumber = shipmentAllocation.FlightNumber;
                    var Airline = dbContext.Airlines.Find(shipmentAllocation.AirlineId);
                    if (Airline != null)
                    {
                        reportObj.Airline = Airline.AirLineName + " - " + Airline.CarrierCode3;
                    }
                    var timezone = dbContext.Timezones.Find(shipmentAllocation.TimezoneId);
                    if (timezone != null)
                    {
                        var TimeZoneInfor = TimeZoneInfo.FindSystemTimeZoneById(userInfo.Name);

                        DateTime date1 = UtilityRepository.UtcDateToOtherTimezone(DateTime.UtcNow, DateTime.UtcNow.TimeOfDay, TimeZoneInfor).Item1;
                        string time1 = UtilityRepository.GetFormattedTimeFromString(UtilityRepository.UtcDateToOtherTimezone(DateTime.UtcNow, DateTime.UtcNow.TimeOfDay, TimeZoneInfor).Item2);

                        reportObj.ETA = date1.ToString("dd-MMM-yy") + " " + time1;
                        reportObj.ETATimeZone = "(" + timezone.OffsetShort + ")";
                    }
                }

                reportObj.CarrierBags = new List<ExpressReportDriverManifestBagDetail>();

                ExpressReportDriverManifestBagDetail m = new ExpressReportDriverManifestBagDetail();

                var detail = dbContext.TradelaneShipmentDetails.Where(x => x.TradelaneShipmentId == TradelaneShipmentId).OrderByDescending(p => p.TradelaneShipmentId == TradelaneShipmentId).FirstOrDefault();

                if (detail != null)
                {
                    m.Carrier = customerDetail.CustomerName;
                    m.CutOffTime = "";
                    m.NoOfBags = ShipmentDetail.HAWBPackages.FirstOrDefault().Packages.Sum(x => x.CartonValue);
                    m.TotalPieces = detail.CartonValue;
                    m.TotalWeight = detail.Weight;

                    reportObj.CarrierBags.Add(m);
                }


                reportObj.CarrierManifests = new List<ExpressReportDriverCarrierManifest>();
                ExpressReportDriverCarrierManifest cb = new ExpressReportDriverCarrierManifest();

                cb.CarrierBagDetails = new List<ExpressReportCarrierBagDetail>();
                ExpressReportCarrierBagDetail cs = new ExpressReportCarrierBagDetail();

                cs.BagId = 0;
                cs.BagNumber = "";
                cs.Carrier = customerDetail.CustomerName;
                cs.ExporterName = customerDetail.CustomerName;
                cs.TermAndCondition = "";
                cs.TotalPieces = detail.CartonValue;
                cs.TotalWeight = detail.Weight;
                cb.CarrierBagDetails.Add(cs);

                reportObj.CarrierManifests.Add(cb);

                list.Add(reportObj);

                return list;
            }
            catch (Exception ex)
            {
                return list;
            }
        }

        #endregion
    }
}