using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models.Express;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using RazorEngine.Templating;
using System.IO;
using System.Web;
using Frayte.Services.Models.Tradelane;
using System.Data.Entity.Validation;

namespace Frayte.Services.Business
{
    public class ExpressRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        #region ExpressSerices

        public ExpressAddressModel getHubAddress(int countryId, string postCode, string state)
        {
            ExpressAddressModel hubAddress;

            hubAddress = (from r in dbContext.Hubs
                          join hc in dbContext.HubCarrierServiceCountries on r.HubId equals hc.HubId
                          join c in dbContext.Countries on r.CountryId equals c.CountryId
                          where hc.CountryId == countryId
                          select new ExpressAddressModel
                          {
                              FirstName = r.ContactFirstName,
                              LastName = r.ContactLastName,
                              HubId = r.HubId,
                              HubCode = r.Code,
                              Address = r.Address,
                              Address2 = r.Address2,
                              Area = r.Area,
                              City = r.City,
                              Country = new FrayteCountryCode
                              {
                                  Code = c.CountryCode,
                                  Code2 = c.CountryCode2,
                                  CountryId = c.CountryId,
                                  CountryPhoneCode = c.CountryPhoneCode,
                                  Name = c.CountryName
                              },
                              Email = r.Email,
                              CompanyName = r.Name,
                              Phone = r.TelephoneNo,
                              PostCode = r.PostCode,
                              State = r.State
                          }).FirstOrDefault();

            if (hubAddress == null)
            {
                hubAddress = (from r in dbContext.Hubs
                              join hc in dbContext.HubCarrierServiceCountryStates on r.HubId equals hc.HubId
                              join c in dbContext.Countries on r.CountryId equals c.CountryId
                              where hc.CountryId == countryId && hc.State == state
                              select new ExpressAddressModel
                              {
                                  FirstName = r.ContactFirstName,
                                  LastName = r.ContactLastName,
                                  HubId = r.HubId,
                                  HubCode = r.Code,
                                  Address = r.Address,
                                  Address2 = r.Address2,
                                  Area = r.Area,
                                  City = r.City,
                                  Country = new FrayteCountryCode
                                  {
                                      Code = c.CountryCode,
                                      Code2 = c.CountryCode2,
                                      CountryId = c.CountryId,
                                      CountryPhoneCode = c.CountryPhoneCode,
                                      Name = c.CountryName
                                  },
                                  Email = r.Email,
                                  CompanyName = r.Name,
                                  Phone = r.TelephoneNo,
                                  PostCode = r.PostCode,
                                  State = r.State
                              }).FirstOrDefault();
            }
            if (hubAddress == null)
            {
                hubAddress = (from r in dbContext.Hubs
                              join hc in dbContext.HubCarrierServiceCountryPostCodes on r.HubId equals hc.HubId
                              join c in dbContext.Countries on r.CountryId equals c.CountryId
                              where hc.CountryId == countryId && hc.PostCode.Contains(postCode)
                              select new ExpressAddressModel
                              {
                                  FirstName = r.ContactFirstName,
                                  LastName = r.ContactLastName,
                                  HubId = r.HubId,
                                  HubCode = r.Code,
                                  Address = r.Address,
                                  Address2 = r.Address2,
                                  Area = r.Area,
                                  City = r.City,
                                  Country = new FrayteCountryCode
                                  {
                                      Code = c.CountryCode,
                                      Code2 = c.CountryCode2,
                                      CountryId = c.CountryId,
                                      CountryPhoneCode = c.CountryPhoneCode,
                                      Name = c.CountryName
                                  },
                                  Email = r.Email,
                                  CompanyName = r.Name,
                                  Phone = r.TelephoneNo,
                                  PostCode = r.PostCode,
                                  State = r.State
                              }).FirstOrDefault();
            }
            if (hubAddress == null)
            {
                hubAddress = new ExpressAddressModel();
            }
            return hubAddress;
        }

        public ExpressAddressModel GetHubAgentByCarrierServiceId(int hubCarrierServiceId)
        {
            ExpressAddressModel address = (from r in dbContext.HubUsers
                                           join U in dbContext.UserAddresses on r.UserId equals U.UserId
                                           join hc in dbContext.HubCarriers on r.HubId equals hc.HubId
                                           join hcs in dbContext.HubCarrierServices on hc.HubCarrierId equals hcs.HubCarrierId
                                           join c in dbContext.Countries on U.CountryId equals c.CountryId
                                           where hcs.HubCarrierServiceId == hubCarrierServiceId
                                           select new ExpressAddressModel
                                           {
                                               //{ 
                                               //    CompanyName = U.CompanyName,
                                               //    FirstName = U.ContactFirstName,
                                               //    LastName = U.ContactLastName,
                                               //    Phone = U.PhoneNo,
                                               //    Email = U.Email,
                                               //    Address = U.Address1,
                                               //    Address2 = U.Address2,
                                               //    Area = U.Area,
                                               //    City = U.City,
                                               //    State = U.State,
                                               //    PostCode = U.Zip,
                                               //    Country = new FrayteCountryCode() { CountryId = U.CountryId } 
                                           }).FirstOrDefault();

            return address;
        }

        public TradelBookingAdress GetHubAgentByHubId(int hubId)
        {
            TradelBookingAdress address = (from r in dbContext.HubUsers
                                           join u in dbContext.Users on r.UserId equals u.UserId
                                           join U in dbContext.UserAddresses on r.UserId equals U.UserId
                                           join h in dbContext.Hubs on r.HubId equals h.HubId
                                           join c in dbContext.Countries on U.CountryId equals c.CountryId
                                           where h.HubId == hubId
                                           select new TradelBookingAdress
                                           {
                                               CompanyName = u.CompanyName,
                                               FirstName = u.ContactName,
                                               LastName = "",
                                               Phone = u.TelephoneNo,
                                               Email = u.Email,
                                               Address = U.Address,
                                               Address2 = U.Address2,
                                               Area = U.Suburb,
                                               City = U.City,
                                               State = U.State,
                                               PostCode = U.Zip,
                                               Country = new FrayteCountryCode() { CountryId = U.CountryId }
                                           }).FirstOrDefault();
            return address;
        }

        public TradelBookingAdress GetNotifyPartyByHubId(int hubId)
        {
            TradelBookingAdress address = (from h in dbContext.Hubs
                                           join c in dbContext.Countries on h.CountryId equals c.CountryId
                                           where h.HubId == hubId
                                           select new TradelBookingAdress
                                           {
                                               CompanyName = h.Name,
                                               FirstName = h.ContactFirstName,
                                               LastName = h.ContactLastName,
                                               Phone = h.TelephoneNo,
                                               Email = h.Email,
                                               Address = h.Address,
                                               Address2 = h.Address2,
                                               Area = h.Area,
                                               City = h.City,
                                               State = h.State,
                                               PostCode = h.PostCode,
                                               Country = new FrayteCountryCode() { CountryId = c.CountryId }
                                           }).FirstOrDefault();
            return address;
        }

        public string AWBlabelPath(string aWBNumber)
        {
            var AWBPath = "";
            AWBPath = AppSettings.WebApiPath + "AwbImage/" + aWBNumber.Replace(" ", "") + ".jpg";
            return AWBPath;
        }

        public List<HubService> GetExpressServices(ExpressServiceRequestModel serviceObj)
        {
            List<HubService> services = new List<HubService>();
            HubService service = new HubService();
            int hubId = 0;
            if (serviceObj != null)
            {
                if (serviceObj.ToCountryId > 0)
                {
                    var hubCarrierServiceCountry = dbContext.HubCarrierServiceCountries.Where(p => p.CountryId == serviceObj.ToCountryId).ToList();
                    if (hubCarrierServiceCountry.Count > 0)
                    {
                        foreach (var hub in hubCarrierServiceCountry)
                        {
                            hubId = hub.HubId;
                            service = (from h in dbContext.Hubs
                                       join hc in dbContext.HubCarriers on h.HubId equals hc.HubId
                                       join hcs in dbContext.HubCarrierServices on hc.HubCarrierId equals hcs.HubCarrierId
                                       join hcsc in dbContext.HubCarrierServiceCountries on hcs.HubCarrierServiceId equals hcsc.HubCarrierServiceId
                                       join ccs in dbContext.CustomerHubCarrierServices on hcs.HubCarrierServiceId equals ccs.HubCarrierServiceId
                                       where
                                            h.HubId == hubId &&
                                            hcs.IsActive == true &&
                                            ccs.CustomerId == serviceObj.CustomerId &&
                                            hcsc.CountryId == serviceObj.ToCountryId &&
                                            hcs.HubCarrierServiceId == hub.HubCarrierServiceId
                                       select new HubService
                                       {
                                           HubCarrierId = hc.HubCarrierId,
                                           HubCarrierServiceId = hcs.HubCarrierServiceId,
                                           HubCarrier = hc.Carrier,
                                           NetworkCode = hcs.NetworkCode,
                                           HubCarrierDisplay = hc.CarrierService,
                                           CourierAccountNo = hcs.AccountNumber,
                                           RateType = hcs.ServiceType,
                                           RateTypeDisplay = hcs.ServiceTypeDisplay,
                                           TransitTime = hcs.TransitTime,
                                           ActualWeight = serviceObj.TotalWeight,
                                           CarrierLogo = hcs.Logo,
                                           WeightRoundLogic = hcs.WeightRoundLogic,
                                           DefaultCurrency = h.DefaultCurrency
                                       }).FirstOrDefault();

                            services.Add(service);
                        }
                    }
                    else
                    {
                        var hubCarrierServiceCountryState = dbContext.HubCarrierServiceCountryStates.Where(p => p.CountryId == serviceObj.ToCountryId && p.State == serviceObj.ToState).FirstOrDefault();
                        if (hubCarrierServiceCountryState != null)
                        {
                            services = (from h in dbContext.Hubs
                                        join hc in dbContext.HubCarriers on h.HubId equals hc.HubId
                                        join hcs in dbContext.HubCarrierServices on hc.HubCarrierId equals hcs.HubCarrierId
                                        join hccs in dbContext.HubCarrierServiceCountryStates on hcs.HubCarrierServiceId equals hccs.HubCarrierServiceId
                                        join ccs in dbContext.CustomerHubCarrierServices on hcs.HubCarrierServiceId equals ccs.HubCarrierServiceId
                                        where hcs.IsActive == true && hccs.CountryId == serviceObj.ToCountryId && hccs.State == serviceObj.ToState &&
                                        ccs.CustomerId == serviceObj.CustomerId && h.HubId == hubCarrierServiceCountryState.HubId
                                        select new HubService
                                        {
                                            HubCarrierId = hc.HubCarrierId,
                                            HubCarrierServiceId = hcs.HubCarrierServiceId,
                                            HubCarrier = hc.Carrier,
                                            HubCarrierDisplay = hc.CarrierService,
                                            NetworkCode = hcs.NetworkCode,
                                            CourierAccountNo = hcs.AccountNumber,
                                            RateType = hcs.ServiceType,
                                            RateTypeDisplay = hcs.ServiceTypeDisplay,
                                            TransitTime = hcs.TransitTime,
                                            ActualWeight = serviceObj.TotalWeight,
                                            CarrierLogo = hcs.Logo,
                                            WeightRoundLogic = hcs.WeightRoundLogic,
                                            DefaultCurrency = h.DefaultCurrency
                                        }).ToList();
                        }
                        else
                        {

                            var hubCarrierServiceCountryPostCode = dbContext.HubCarrierServiceCountryPostCodes.Where(p => p.CountryId == serviceObj.ToCountryId && p.PostCode == serviceObj.ToPostCode).FirstOrDefault();

                            if (hubCarrierServiceCountryPostCode != null)
                            {
                                services = (from h in dbContext.Hubs
                                            join hc in dbContext.HubCarriers on h.HubId equals hc.HubId
                                            join hcs in dbContext.HubCarrierServices on hc.HubCarrierId equals hcs.HubCarrierId
                                            join ccs in dbContext.CustomerHubCarrierServices on hcs.HubCarrierServiceId equals ccs.HubCarrierServiceId
                                            join hcsp in dbContext.HubCarrierServiceCountryPostCodes on hcs.HubCarrierServiceId equals hcsp.HubCarrierServiceId
                                            where hcs.IsActive == true && hcsp.CountryId == serviceObj.ToCountryId && hcsp.PostCode == serviceObj.ToPostCode
                                            && ccs.CustomerId == serviceObj.CustomerId && h.HubId == hubCarrierServiceCountryPostCode.HubId
                                            select new HubService
                                            {
                                                HubCarrierId = hc.HubCarrierId,
                                                HubCarrierServiceId = hcs.HubCarrierServiceId,
                                                HubCarrier = hc.Carrier,
                                                HubCarrierDisplay = hc.CarrierService,
                                                NetworkCode = hcs.NetworkCode,
                                                CourierAccountNo = hcs.AccountNumber,
                                                RateType = hcs.ServiceType,
                                                RateTypeDisplay = hcs.ServiceTypeDisplay,
                                                TransitTime = hcs.TransitTime,
                                                ActualWeight = serviceObj.TotalWeight,
                                                CarrierLogo = hcs.Logo,
                                                WeightRoundLogic = hcs.WeightRoundLogic,
                                                DefaultCurrency = h.DefaultCurrency
                                            }).ToList();
                            }
                        }
                    }
                    if (services.Count > 0)
                    {

                        var hub = dbContext.Hubs.Find(hubId);

                        foreach (var item in services)
                        {
                            if (hub != null && hub.CountryId == 228 && serviceObj.ToCountryId == 228)
                            {
                                item.LogisticServiceType = "UK Domestic";
                            }

                            // to do : implement weight round logic 
                            if (item.WeightRoundLogic == "Five")
                            {
                            }
                            else if (item.WeightRoundLogic == "One")
                            {
                            }
                            else
                            {

                            }
                            item.BillingWeight = Math.Round(item.ActualWeight, 2);
                        }
                    }
                }
            }

            if (serviceObj.ToCountryId != 193 && serviceObj.ToCountryId != 105)
            {
                var carrierserviceeam = (from h in dbContext.Hubs
                                         join hc in dbContext.HubCarriers on h.HubId equals hc.HubId
                                         join hcs in dbContext.HubCarrierServices on hc.HubCarrierId equals hcs.HubCarrierId
                                         join ccs in dbContext.CustomerHubCarrierServices on hcs.HubCarrierServiceId equals ccs.HubCarrierServiceId
                                         where ccs.CustomerId == serviceObj.CustomerId && hc.Carrier == "EAM Express"
                                         select new HubService
                                         {
                                             HubCarrierId = hc.HubCarrierId,
                                             HubCarrierServiceId = hcs.HubCarrierServiceId,
                                             HubCarrier = hc.Carrier,
                                             HubCarrierDisplay = hc.CarrierService,
                                             NetworkCode = hcs.NetworkCode,
                                             CourierAccountNo = hcs.AccountNumber,
                                             RateType = hcs.ServiceType,
                                             RateTypeDisplay = hcs.ServiceTypeDisplay,
                                             TransitTime = hcs.TransitTime,
                                             ActualWeight = serviceObj.TotalWeight,
                                             CarrierLogo = hcs.Logo,
                                             WeightRoundLogic = hcs.WeightRoundLogic,
                                             DefaultCurrency = h.DefaultCurrency
                                         }).ToList();

                var Finalservice = services.Concat(carrierserviceeam).ToList();
                return Finalservice;
            }
            return services;
        }

        public string GetShipmentAlllabelPath(int expressId)
        {
            string Attachments = string.Empty;
            throw new NotImplementedException();
        }

        #endregion

        #region Save Express Shipment

        public ExpressShipmentModel SaveShipment(ExpressShipmentModel shipment)
        {
            shipment.Error = new FratyteError();
            try
            {
                // Step 1 : Save Shipment From Address
                shipment.ShipFrom.CustomerId = shipment.CustomerId;
                shipment.ShipFrom.AddressType = FrayteFromToAddressType.FromAddress;
                SaveExpressAddress(shipment.ShipmentStatusId, shipment.ShipFrom, FrayteFromToAddressType.FromAddress);

                // Step 2 : Save Shipment To Address
                shipment.ShipTo.CustomerId = shipment.CustomerId;
                shipment.ShipTo.AddressType = FrayteFromToAddressType.ToAddress;
                SaveExpressAddress(shipment.ShipmentStatusId, shipment.ShipTo, FrayteFromToAddressType.ToAddress);

                // Step 3 : Save Shipment Detail
                SaveShipmentDetail(shipment);

                // Step 4 : Save Package Detail
                SavePackageDetail(shipment);

                // Step 5 : Save Custom Detail 
                SaveCustomDetail(shipment);

                shipment.Error.Status = true;               

            }
            catch (DbEntityValidationException dbEx)
            {
                string validationErrorMsg = "";
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        validationErrorMsg = validationErrorMsg + string.Format("Property: {0} Error: {1}",
                        validationError.PropertyName,
                        validationError.ErrorMessage);
                    }
                }
                throw (new FrayteApiException("EntityError", dbEx));
            }
            return shipment;
        }

        private void SaveCustomDetail(ExpressShipmentModel shipment)
        {
            ExpressCustomDetail dbCustom;
            try
            {
                if (shipment.CustomInformation != null)
                {
                    if (shipment.CustomInformation.ShipmentCustomDetailId == 0)
                    {
                        dbCustom = new ExpressCustomDetail();
                        dbCustom.ShipmentId = shipment.ExpressId;
                        dbCustom.CustomsCertify = shipment.CustomInformation.CustomsCertify;
                        dbCustom.CustomsSigner = shipment.CustomInformation.CustomsSigner;
                        dbCustom.ContentsType = shipment.CustomInformation.ContentsType;
                        dbCustom.ContentsExplanation = shipment.CustomInformation.ContentsExplanation;
                        dbCustom.RestrictionType = shipment.CustomInformation.RestrictionType;
                        dbCustom.RestrictionComments = shipment.CustomInformation.RestrictionComments;
                        dbCustom.NonDeliveryOption = shipment.CustomInformation.NonDeliveryOption;
                        dbContext.ExpressCustomDetails.Add(dbCustom);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        dbCustom = dbContext.ExpressCustomDetails.Find(shipment.CustomInformation.ShipmentCustomDetailId);
                        if (dbCustom != null)
                        {
                            dbCustom.ShipmentId = shipment.ExpressId;
                            dbCustom.CustomsCertify = shipment.CustomInformation.CustomsCertify;
                            dbCustom.CustomsSigner = shipment.CustomInformation.CustomsSigner;
                            dbCustom.ContentsType = shipment.CustomInformation.ContentsType;
                            dbCustom.ContentsExplanation = shipment.CustomInformation.ContentsExplanation;
                            dbCustom.RestrictionType = shipment.CustomInformation.RestrictionType;
                            dbCustom.RestrictionComments = shipment.CustomInformation.RestrictionComments;
                            dbCustom.NonDeliveryOption = shipment.CustomInformation.NonDeliveryOption;
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                throw (new FrayteApiException("ShipmentCustomInfoError", Ex));
            }
        }

        public ExpressAddressModel GetHubAddress(int countryId, string postcode)
        {
            throw new NotImplementedException();
        }

        public List<string> GetCustomerAWBs(int customerId, string AWB)
        {
            AWB = AWB.Replace(" ", "");
            var OperationZone = UtilityRepository.GetOperationZone().OperationZoneId;
            if (customerId > 0)
            {
                var awbs = dbContext.Expresses.Where(p => p.OperationZoneId == OperationZone &&
                p.CustomerId == customerId && p.ShipmentStatusId == 37 &&
                p.AWBBarcode.Contains(AWB)).Select(p => p.AWBBarcode).ToList();
                if (awbs != null)
                {
                    for (int i = 0; i < awbs.Count; i++)
                    {
                        awbs[i] = awbs[i].Substring(0, 3) + " " + awbs[i].Substring(3, 3) + " " + awbs[i].Substring(6, 3) + " " + awbs[i].Substring(9, 3);
                    }
                }
                return awbs;
            }
            else
            {
                var awbs = dbContext.Expresses.Where(p => p.OperationZoneId == OperationZone && p.ShipmentStatusId == 37 && p.AWBBarcode.Contains(AWB)).Select(p => p.AWBBarcode).ToList();
                if (awbs != null)
                {
                    for (int i = 0; i < awbs.Count; i++)
                    {
                        awbs[i] = awbs[i].Substring(0, 3) + " " + awbs[i].Substring(3, 3) + " " + awbs[i].Substring(6, 3) + " " + awbs[i].Substring(9, 3);
                    }
                }
                return awbs;
            }
        }

        public FrayteResult SendLabelEmail(ExpressLabelEmail labelObj)
        {
            FrayteResult result = new FrayteResult();
            var emailObj = new ExpressEmailRepository().ExpressEmailObj(labelObj.ShipmentId);
            emailObj.To = labelObj.Email;
            result = new ExpressEmailRepository().SendLabelEmail(emailObj);
            return result;
        }

        private void SaveExpressAddress(int ShipmentStatusId, ExpressAddressModel address, string addressType)
        {
            ExpressAddress shipmentAddress;
            try
            {
                if (addressType == FrayteFromToAddressType.FromAddress)
                {
                    if (address.ExpressAddressId == 0)
                    {
                        shipmentAddress = new ExpressAddress();
                        shipmentAddress.CountryId = address.Country.CountryId;
                        shipmentAddress.CompanyName = address.CompanyName;
                        shipmentAddress.ContactFirstName = address.FirstName;
                        shipmentAddress.ContactLastName = address.LastName;
                        shipmentAddress.City = address.City;
                        shipmentAddress.State = address.State;
                        shipmentAddress.Zip = address.PostCode;
                        shipmentAddress.Address1 = address.Address;
                        shipmentAddress.Address2 = address.Address2;
                        shipmentAddress.Email = address.Email;
                        shipmentAddress.PhoneNo = address.Phone;
                        shipmentAddress.Area = address.Area;
                        dbContext.ExpressAddresses.Add(shipmentAddress);
                        dbContext.SaveChanges();

                        address.ExpressAddressId = shipmentAddress.ExpressAddressId;
                    }
                    else
                    {
                        shipmentAddress = dbContext.ExpressAddresses.Find(address.ExpressAddressId);
                        shipmentAddress.CountryId = address.Country.CountryId;
                        shipmentAddress.CompanyName = address.CompanyName;
                        shipmentAddress.ContactFirstName = address.FirstName;
                        shipmentAddress.ContactLastName = address.LastName;
                        shipmentAddress.City = address.City;
                        shipmentAddress.State = address.State;
                        shipmentAddress.Zip = address.PostCode;
                        shipmentAddress.Address1 = address.Address;
                        shipmentAddress.Address2 = address.Address2;
                        shipmentAddress.Email = address.Email;
                        shipmentAddress.PhoneNo = address.Phone;
                        shipmentAddress.Area = address.Area;
                        dbContext.SaveChanges();
                    }

                    if (ShipmentStatusId != (int)FrayteExpressShipmentStatus.Draft)
                    {
                        // Check if  shipfrom address is different then save it to adress Book
                        var addressBooks = dbContext.AddressBooks.Where(p => p.Address1 == address.Address &&
                                                                             p.Address2 == address.Address2 &&
                                                                             p.CustomerId == address.CustomerId &&
                                                                             p.City == address.City &&
                                                                             p.State == address.State &&
                                                                             p.PhoneNo == address.Phone &&
                                                                             p.Area == address.Area &&
                                                                             p.CompanyName == address.CompanyName &&
                                                                             p.ContactFirstName == address.FirstName &&
                                                                             p.ContactLastName == address.LastName &&
                                                                             p.CountryId == address.Country.CountryId &&
                                                                             p.CustomerId == address.CustomerId &&
                                                                             p.Email == address.Email &&
                                                                             p.Zip == address.PostCode &&
                                                                             p.IsActive == true &&
                                                                             p.FromAddress == true).FirstOrDefault();

                        if (addressBooks == null)
                        {
                            AddressBook addressBook = new AddressBook();
                            addressBook.Address1 = address.Address;
                            addressBook.Address2 = address.Address2;
                            addressBook.Area = address.Area;
                            addressBook.City = address.City;
                            addressBook.CompanyName = address.CompanyName;
                            addressBook.ContactFirstName = address.FirstName;
                            addressBook.ContactLastName = address.LastName;
                            addressBook.CountryId = address.Country.CountryId;
                            addressBook.CustomerId = address.CustomerId;
                            addressBook.FromAddress = true;
                            addressBook.Email = address.Email;
                            addressBook.Zip = address.PostCode;
                            addressBook.ToAddress = false;
                            addressBook.IsActive = true;
                            addressBook.IsDefault = address.IsDefault;
                            addressBook.PhoneNo = address.Phone;
                            dbContext.AddressBooks.Add(addressBook);
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            if (address.IsDefault)
                            {
                                addressBooks.IsDefault = address.IsDefault;
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
                else if (addressType == FrayteFromToAddressType.ToAddress)
                {
                    if (address.ExpressAddressId == 0)
                    {
                        shipmentAddress = new ExpressAddress();
                        shipmentAddress.CountryId = address.Country.CountryId;
                        shipmentAddress.CompanyName = address.CompanyName;
                        shipmentAddress.ContactFirstName = address.FirstName;
                        shipmentAddress.ContactLastName = address.LastName;
                        shipmentAddress.City = address.City;
                        shipmentAddress.State = address.State;
                        shipmentAddress.Zip = address.PostCode;
                        shipmentAddress.Address1 = address.Address;
                        shipmentAddress.Address2 = address.Address2;
                        shipmentAddress.Email = address.Email;
                        shipmentAddress.PhoneNo = address.Phone;
                        shipmentAddress.Area = address.Area;
                        dbContext.ExpressAddresses.Add(shipmentAddress);
                        dbContext.SaveChanges();

                        address.ExpressAddressId = shipmentAddress.ExpressAddressId;
                    }
                    else
                    {
                        shipmentAddress = dbContext.ExpressAddresses.Find(address.ExpressAddressId);
                        shipmentAddress.CountryId = address.Country.CountryId;
                        shipmentAddress.CompanyName = address.CompanyName;
                        shipmentAddress.ContactFirstName = address.FirstName;
                        shipmentAddress.ContactLastName = address.LastName;
                        shipmentAddress.City = address.City;
                        shipmentAddress.State = address.State;
                        shipmentAddress.Zip = address.PostCode;
                        shipmentAddress.Address1 = address.Address;
                        shipmentAddress.Address2 = address.Address2;
                        shipmentAddress.Email = address.Email;
                        shipmentAddress.PhoneNo = address.Phone;
                        shipmentAddress.Area = address.Area;
                        dbContext.SaveChanges();

                        address.ExpressAddressId = shipmentAddress.ExpressAddressId;
                    }

                    if (ShipmentStatusId != (int)FrayteExpressShipmentStatus.Draft)
                    {
                        // Check if  shipfrom address is different then save it to adress Book
                        var addressBooks = dbContext.AddressBooks.Where(p => p.Address1 == address.Address &&
                                                                             p.Address2 == address.Address2 &&
                                                                             p.CustomerId == address.CustomerId &&
                                                                             p.City == address.City &&
                                                                             p.State == address.State &&
                                                                             p.PhoneNo == address.Phone &&
                                                                             p.Area == address.Area &&
                                                                             p.CompanyName == address.CompanyName &&
                                                                             p.ContactFirstName == address.FirstName &&
                                                                             p.ContactLastName == address.LastName &&
                                                                             p.CountryId == address.Country.CountryId &&
                                                                             p.CustomerId == address.CustomerId &&
                                                                             p.Email == address.Email &&
                                                                             p.Zip == address.PostCode &&
                                                                             p.IsActive == true &&
                                                                             p.ToAddress == true).FirstOrDefault();

                        if (addressBooks == null)
                        {
                            AddressBook addressBook = new AddressBook();
                            addressBook.Address1 = address.Address;
                            addressBook.Address2 = address.Address2;
                            addressBook.Area = address.Area;
                            addressBook.City = address.City;
                            addressBook.CompanyName = address.CompanyName;
                            addressBook.ContactFirstName = address.FirstName;
                            addressBook.ContactLastName = address.LastName;
                            addressBook.CountryId = address.Country.CountryId;
                            addressBook.CustomerId = address.CustomerId;
                            addressBook.FromAddress = false;
                            addressBook.Email = address.Email;
                            addressBook.Zip = address.PostCode;
                            addressBook.ToAddress = true;
                            addressBook.IsActive = true;
                            addressBook.IsDefault = address.IsDefault;
                            addressBook.PhoneNo = address.Phone;
                            dbContext.AddressBooks.Add(addressBook);
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            if (address.IsDefault)
                            {
                                addressBooks.IsDefault = address.IsDefault;
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw (new FrayteApiException("AddressIssue", ex));
            }
        }

        public FrayteResult PrintLabel(int id, string type)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                if (type == "Shipment")
                {
                    var shipmentDetail = dbContext.ExpressDetails.Where(p => p.ExpressId == id).ToList();
                    if (shipmentDetail != null && shipmentDetail.Count > 0)
                    {
                        foreach (var item in shipmentDetail)
                        {
                            var labels = dbContext.ExpressDetailPackageLabels.Where(p => p.ExpressShipmentDetailId == item.ExpressDetailId);
                            if (labels != null)
                            {
                                foreach (var item1 in labels)
                                {
                                    item1.IsDownloaded = true;
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                        result.Status = true;
                    }
                }
                else
                {
                    var label = dbContext.ExpressDetailPackageLabels.Find(id);
                    if (label != null)
                    {
                        label.IsDownloaded = true;
                        dbContext.SaveChanges();
                        result.Status = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

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
                                ua.IsExpressSolutions == true &&
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
            else if (new TradelaneBookingRepository().GetUserRole(userId) == (int)FrayteUserRole.CustomerStaff)
            {
                customers = (from cs in dbContext.CustomerStaffs
                             join r in dbContext.Users on cs.UserId equals r.UserId
                             join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                             where
                                cs.CustomerStaffId == userId &&
                                 ua.IsExpressSolutions == true &&
                                cs.IsActive == true &&
                                r.IsActive == true &&
                                r.OperationZoneId == operationzone.OperationZoneId
                             select new TradelaneCustomer
                             {
                                 CustomerId = r.UserId,
                                 CustomerName = r.ContactName,
                                 CompanyName = r.CompanyName,
                                 AccountNumber = ua.AccountNo,
                                 EmailId = r.UserEmail,
                                 ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                 CustomerCurrency = ua.CreditLimitCurrencyCode,
                                 OperationZoneId = r.OperationZoneId,
                             }).Distinct().ToList();

                for (int i = 0; i < customers.Count; i++)
                {
                    customers[i].OrderNumber = i + 1;
                }
            }
            else
            {
                customers = (from r in dbContext.Users
                             join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                             join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                             where
                                ur.RoleId == (int)FrayteUserRole.Customer &&
                                ua.IsExpressSolutions == true &&
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

        public ExpressEmailModel Fill_EXS_E1Model(ExpressEmailModel emailModel, ExpressShipmentModel shipment)
        {
            try
            {
                emailModel.ShipmentDetail = shipment;
                var shipmentDetail = dbContext.Expresses.Find(shipment.ExpressId);
                var userDetail = (from r in dbContext.Users
                                  join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                                  where r.UserId == shipment.CreatedBy
                                  select new
                                  {
                                      Email = r.Email,
                                      TimeZone = r.TimezoneId,
                                      UserEmail = r.UserEmail,
                                      RoleId = ur.RoleId
                                  }).FirstOrDefault();

                //Get Customer Name and Customer User Detail
                var customerDetail = (from u in dbContext.Users
                                      where u.UserId == (userDetail.RoleId == (int)FrayteUserRole.UserCustomer ? shipment.CreatedBy : shipment.CustomerId)
                                      select new
                                      {
                                          CustomerName = u.ContactName,
                                          CustomerEmail = u.UserEmail,
                                          CompanyName = u.CompanyName
                                      }).FirstOrDefault();

                var staffdetail = (from r in dbContext.Users
                                   join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                                   join uau in dbContext.Users on ua.OperationUserId equals uau.UserId
                                   join uauad in dbContext.UserAddresses on uau.UserId equals uauad.UserId
                                   join c in dbContext.Countries on uauad.CountryId equals c.CountryId
                                   where ua.UserId == shipment.CustomerId
                                   select new
                                   {
                                       CustomerEmail = r.UserEmail,
                                       StaffEmail = uau.Email,
                                       StaffName = uau.ContactName,
                                       StaffPosition = uau.Position,
                                       StaffPhone = uau.TelephoneNo,
                                       CountryPhoneCode = c.CountryPhoneCode
                                   }).FirstOrDefault();

                //Get SMTP Setting
                var detail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == emailModel.ShipmentDetail.CustomerId).FirstOrDefault();

                emailModel.To = staffdetail.CustomerEmail;

                if (userDetail.RoleId != 3)
                {
                    emailModel.To += ";" + userDetail.Email;
                }

                if (shipment.ShipFrom.Email != null)
                {
                    emailModel.To += ";" + shipment.ShipFrom.Email;
                }

                if (shipment.ShipTo.Email != null)
                {
                    emailModel.To += ";" + shipment.ShipTo.Email;
                }

                if (detail != null)
                {
                    emailModel.StaffUserEmail = detail.OperationStaffEmail;
                    emailModel.UserPosition = detail.UserPosition;
                    emailModel.StaffUserName = detail.OperationStaff;
                    emailModel.CompanyName = detail.CompanyName;
                    emailModel.UserPhone = detail.OperationStaffPhone;
                    emailModel.SiteAddress = detail.SiteAddress;
                    emailModel.TrackingWebsite = detail.TrackingUrl + "/#/tracking/" + shipment.AWBNumber.Replace(" ", "");
                    emailModel.TrackingURL = detail.TrackingUrl + "/#/tracking/" + shipmentDetail.TrackingNumber.Replace("Order_", "");
                }
                else
                {
                    emailModel.StaffUserEmail = staffdetail.StaffEmail;
                    emailModel.UserPosition = staffdetail.StaffPosition;
                    emailModel.StaffUserName = staffdetail.StaffName;
                    emailModel.CompanyName = "FRAYTE GLOBAL";
                    emailModel.UserPhone = "( + " + staffdetail.CountryPhoneCode + " ) " + staffdetail.StaffPhone;
                    emailModel.SiteAddress = UtilityRepository.GetOperationZone().OperationZoneId == 1 ? "www.FRAYTE.com" : "www.FRAYTE.co.uk";
                    emailModel.TrackingWebsite = UtilityRepository.GetOperationZone().OperationZoneId == 1 ? "https://frayte.com/tracking-detail/" + shipment.AWBNumber.Replace(" ", "") : "https://frayte.co.uk/tracking-detail/" + shipment.AWBNumber.Replace(" ", "");
                    emailModel.TrackingURL = AppSettings.TrackingUrl + "/tracking-detail/" + shipmentDetail.TrackingNumber.Replace("Order_", "");
                }
                emailModel.ShipmentDetail.TrackingNumber = shipmentDetail.TrackingNumber;
                emailModel.CustomerName = customerDetail.CustomerName;
                emailModel.TotalCarton = shipment.Packages.Sum(p => p.CartonValue);
                emailModel.TotalWeight = Math.Round(shipment.Packages.Sum(p => p.Weight * p.CartonValue), 2);

                return emailModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void SendEmail_EXS_E1(ExpressEmailModel emailModel)
        {
            if (emailModel.ShipmentDetail.Service.HubCarrier == "SkyPostal")
            {
                emailModel.ShipmentDetail.Service.HubCarrier = "UPS Ground";
            }

            var operationzone = UtilityRepository.GetOperationZone();
            emailModel.ImageHeader = "FrayteLogo";
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Express/EXS_E1.cshtml");
            emailModel.CreatedOn = emailModel.ShipmentDetail.CreatedOnUtc != null ? emailModel.ShipmentDetail.CreatedOnUtc.ToString("dd-MMM-yyyy hh:mm") : "";
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailModel, null, null);
            string EmailSubject = string.Empty;

            EmailSubject = emailModel.CompanyName + " - EXS Booking Summary - " + emailModel.ShipmentDetail.AWBNumber;

            var To = emailModel.To;
            var Status = "";

            #region Attach Labels 

            string Attachment = PackageLabelPath_EXS_E1(emailModel.ShipmentDetail.ExpressId);

            #endregion

            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Customer Email " + emailModel.ShipmentDetail.FrayteNumber));
            //Send mail to Customer
            Send_FrayteEmail(To, AppSettings.TOCC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment, Status, emailModel.ShipmentDetail.CustomerId);

            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(EmailBody));
        }

        private string PackageLabelPath_EXS_E1(int expressId)
        {
            string attachments = string.Empty;
            var shipment = dbContext.Expresses.Find(expressId);
            if (shipment != null)
            {
                FrayteLogicalPhysicalPath path = new ExpressRepository().GetShipmentLogisticlabelPath(expressId);
                string pdfFileName = string.Empty;
                string physycalpath = string.Empty;

                if (path != null)
                {
                    if (path.PhysicalPath.Contains("~"))
                    {
                        // For developement
                        pdfFileName = AppSettings.WebApiPath + "PackageLabel/Express/" + expressId + "/";
                        physycalpath = HttpContext.Current.Server.MapPath(path.PhysicalPath + "PackageLabel/Express/" + expressId + "/");
                    }
                    else
                    {
                        pdfFileName = path.LogicalPath + "PackageLabel/Express/" + expressId + "/";
                        physycalpath = HttpContext.Current.Server.MapPath("~/PackageLabel/Express/" + expressId + "/");
                    }
                }
                else
                {
                    pdfFileName = AppSettings.WebApiPath + "PackageLabel/Express/" + expressId + "/";
                    physycalpath = HttpContext.Current.Server.MapPath("~/PackageLabel/Express/" + expressId + "/");
                }

                if (AppSettings.LabelSave == "")
                {
                    var result = (from DS in dbContext.Expresses
                                  where DS.ExpressId == expressId
                                  select new
                                  {
                                      LogisticLabel = DS.LogisticLabel
                                  }).FirstOrDefault();

                    if (result != null)
                    {
                        if (!string.IsNullOrEmpty(result.LogisticLabel))
                        {
                            if (result.LogisticLabel.Contains(".html"))
                            {
                                string[] strAttachmentPath = result.LogisticLabel.Split(new string[] { ";" }, StringSplitOptions.None);
                                for (int i = 0; i < strAttachmentPath.Length; i++)
                                {
                                    if (strAttachmentPath[i].Trim() != "")
                                    {
                                        attachments += physycalpath + strAttachmentPath[i].Trim() + ";";
                                    }
                                }
                            }
                            else
                            {
                                attachments += AppSettings.WebApiPath + "/PackageLabel/Express/" + expressId + "/" + result.LogisticLabel;
                            }
                        }
                    }
                }
                else
                {
                    var result = (from DS in dbContext.Expresses
                                  where DS.ExpressId == expressId
                                  select new
                                  {
                                      LogisticLabel = DS.LogisticLabel
                                  }).FirstOrDefault();

                    if (result != null)
                    {
                        if (!string.IsNullOrEmpty(result.LogisticLabel))
                        {
                            if (result.LogisticLabel.Contains(".html"))
                            {
                                string[] strAttachmentPath = result.LogisticLabel.Split(new string[] { ";" }, StringSplitOptions.None);
                                for (int i = 0; i < strAttachmentPath.Length; i++)
                                {
                                    if (strAttachmentPath[i].Trim() != "")
                                    {
                                        attachments += physycalpath + strAttachmentPath[i].Trim() + ";";
                                    }
                                }
                            }
                            else
                            {
                                attachments += physycalpath + result.LogisticLabel;
                            }
                        }
                    }
                }
            }
            return attachments;
        }

        public FrayteLogicalPhysicalPath GetShipmentLogisticlabelPath(int expressId)
        {
            var detail = (from r in dbContext.Expresses
                          join u in dbContext.Users on r.CustomerId equals u.UserId
                          join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                          where r.ExpressId == expressId
                          select new
                          {
                              CustomerId = r.CustomerId,
                              UserType = ua.UserType
                          }
                             ).FirstOrDefault();

            if (detail != null && !string.IsNullOrEmpty(detail.UserType))
            {
                if (detail.UserType == FrayteCustomerTypeEnum.SPECIAL)
                {
                    FrayteLogicalPhysicalPath documnetPath = new FrayteLogicalPhysicalPath();
                    var customercompanyDetail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == detail.CustomerId).FirstOrDefault();
                    documnetPath.LogicalPath = customercompanyDetail.DocumentSiteLogical;
                    documnetPath.PhysicalPath = customercompanyDetail.DocumentSitePhysical;
                    return documnetPath;
                }
            }
            return null;
        }

        public void Send_FrayteEmail(string toEmail, string ccEmail, string DisplayName, string EmailSubject, string EmailBody, string AttachmentFilePath, string Status, int customerId)
        {
            var detail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == customerId).FirstOrDefault();

            string logoImage = detail == null ? AppSettings.EmailServicePath + "/Images/FrayteLogo.png" : AppSettings.EmailServicePath + "/EmailTeamplate/" + customerId + "/Images/" + detail.LogoFileName;
            string trackImage = AppSettings.EmailServicePath + "/Images/TrackShipment.png";
            string amdentImage = AppSettings.EmailServicePath + "/Images/Amend.png";
            string confirmImage = AppSettings.EmailServicePath + "/Images/Confirm.png";
            string rejectImage = AppSettings.EmailServicePath + "/Images/Reject.png";

            List<string> ImagePath = new List<string>();
            if (Status == "Confirmation")
            {
                ImagePath.Add(logoImage);
                ImagePath.Add(trackImage);
            }
            else if (Status == "Cancel")
            {
                ImagePath.Add(logoImage);
                ImagePath.Add(rejectImage);
            }
            else if (Status == "Amend")
            {
                ImagePath.Add(logoImage);
                ImagePath.Add(amdentImage);
            }
            else if (Status == "Tracking")
            {
                ImagePath.Add(logoImage);
                ImagePath.Add(trackImage);
            }
            //For Special Customer
            else
            {
                ImagePath.Add(logoImage);
            }

            FrayteEmail.SendFrayteEmail(toEmail, ccEmail, DisplayName, EmailSubject, EmailBody, AttachmentFilePath, ImagePath, Status, customerId);
        }

        public ExpressShipmentModel ShipmentDetail(int expressId)
        {
            ExpressShipmentModel shipment = new ExpressShipmentModel();
            shipment.ExpressId = expressId;
            return shipment;
        }

        public void SaveMainTrackingDetail(ExpressShipmentModel shipment, IntegrtaionResult result, string request, string response)
        {
            if (shipment.Service.HubCarrier == FrayteCourierCompany.DHL)
            {
                for (int i = 0; i < result.PieceTrackingDetails.Count; i++)
                {
                    if (result.PieceTrackingDetails[i].PieceTrackingNumber.Contains("AirwayBillNumber_"))
                    {
                        var dbshipment = dbContext.Expresses.Find(shipment.ExpressId);
                        if (dbshipment != null)
                        {
                            dbshipment.TrackingNumber = result.PieceTrackingDetails[i].PieceTrackingNumber.Replace("AirwayBillNumber_", "");
                            dbshipment.IntegrationRequest = !string.IsNullOrEmpty(request) ? request : "";
                            dbshipment.IntegrationResponse = !string.IsNullOrEmpty(result.IntegrationReponse) ? result.IntegrationReponse : "";
                            dbContext.SaveChanges();

                            //Save Tracking No in TrackingNumberRoute Table for tracking
                            TrackingNumberRoute tnr = new TrackingNumberRoute();
                            tnr.Number = result.TrackingNumber.Replace("Order_", "");
                            tnr.ShipmentId = shipment.ExpressId;
                            tnr.ModuleType = "ExpressBooking";
                            tnr.IsTrackingNumber = true;
                            dbContext.TrackingNumberRoutes.Add(tnr);
                            dbContext.SaveChanges();
                        }
                        break;
                    }
                }
            }
            if (shipment.Service.HubCarrier == FrayteCourierCompany.DPD)
            {
                for (int i = 0; i < result.PieceTrackingDetails.Count; i++)
                {
                    if (result.PieceTrackingDetails[i].PieceTrackingNumber.Length > 0)
                    {
                        var dbshipment = dbContext.Expresses.Find(shipment.ExpressId);
                        if (dbshipment != null)
                        {
                            dbshipment.TrackingNumber = result.PieceTrackingDetails[i].PieceTrackingNumber.Replace("AirwayBillNumber_", "");
                            dbContext.SaveChanges();

                            //Save Tracking No in TrackingNumberRoute Table for tracking
                            TrackingNumberRoute tnr = new TrackingNumberRoute();
                            tnr.Number = result.TrackingNumber;
                            tnr.ShipmentId = shipment.ExpressId;
                            tnr.ModuleType = "ExpressBooking";
                            tnr.IsTrackingNumber = true;
                            dbContext.TrackingNumberRoutes.Add(tnr);
                            dbContext.SaveChanges();
                        }
                        break;
                    }
                }
            }
            if (shipment.Service.HubCarrier == FrayteCourierCompany.DPDCH)
            {
                for (int i = 0; i < result.PieceTrackingDetails.Count; i++)
                {
                    if (result.PieceTrackingDetails[i].PieceTrackingNumber.Length > 0)
                    {
                        var dbshipment = dbContext.Expresses.Find(shipment.ExpressId);
                        if (dbshipment != null)
                        {
                            dbshipment.TrackingNumber = result.PieceTrackingDetails[i].PieceTrackingNumber.Replace("AirwayBillNumber_", "");
                            dbshipment.IntegrationRequest = !string.IsNullOrEmpty(request) ? request : "";
                            dbshipment.IntegrationResponse = !string.IsNullOrEmpty(response) ? response : "";
                            dbContext.SaveChanges();

                            //Save Tracking No in TrackingNumberRoute Table for tracking
                            TrackingNumberRoute tnr = new TrackingNumberRoute();
                            tnr.Number = result.TrackingNumber;
                            tnr.ShipmentId = shipment.ExpressId;
                            tnr.ModuleType = "ExpressBooking";
                            tnr.IsTrackingNumber = true;
                            dbContext.TrackingNumberRoutes.Add(tnr);
                            dbContext.SaveChanges();
                        }
                        break;
                    }
                }
            }
            if (shipment.Service.HubCarrier == FrayteCourierCompany.BRING)
            {
                for (int i = 0; i < result.PieceTrackingDetails.Count; i++)
                {
                    if (result.PieceTrackingDetails[i].PieceTrackingNumber.Length > 0)
                    {
                        var dbshipment = dbContext.Expresses.Find(shipment.ExpressId);
                        if (dbshipment != null)
                        {
                            dbshipment.TrackingNumber = result.PieceTrackingDetails[i].PieceTrackingNumber.Replace("AirwayBillNumber_", "");
                            dbshipment.IntegrationRequest = !string.IsNullOrEmpty(request) ? request : "";
                            dbshipment.IntegrationResponse = !string.IsNullOrEmpty(response) ? response : "";
                            dbContext.SaveChanges();

                            //Save Tracking No in TrackingNumberRoute Table for tracking
                            TrackingNumberRoute tnr = new TrackingNumberRoute();
                            tnr.Number = result.TrackingNumber;
                            tnr.ShipmentId = shipment.ExpressId;
                            tnr.ModuleType = "ExpressBooking";
                            tnr.IsTrackingNumber = true;
                            dbContext.TrackingNumberRoutes.Add(tnr);
                            dbContext.SaveChanges();
                        }
                        break;
                    }
                }
            }
            if (shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.SKYPOSTAL)
            {
                var dbshipment = dbContext.Expresses.Find(shipment.ExpressId);
                if (dbshipment != null)
                {
                    dbshipment.TrackingNumber = result.TrackingNumber;
                    dbshipment.IntegrationRequest = !string.IsNullOrEmpty(request) ? request : "";
                    dbshipment.IntegrationResponse = !string.IsNullOrEmpty(response) ? response : "";
                    dbContext.SaveChanges();

                    //Save Tracking No in TrackingNumberRoute Table for tracking
                    TrackingNumberRoute tnr = new TrackingNumberRoute();
                    tnr.Number = result.TrackingNumber;
                    tnr.ShipmentId = shipment.ExpressId;
                    tnr.ModuleType = "ExpressBooking";
                    tnr.IsTrackingNumber = true;
                    dbContext.TrackingNumberRoutes.Add(tnr);
                    dbContext.SaveChanges();
                }
            }
            if (shipment.Service.HubCarrier == FrayteCourierCompany.DomesticA || shipment.Service.HubCarrier == FrayteCourierCompany.EAMExpress)
            {
                var dbshipment = dbContext.Expresses.Find(shipment.ExpressId);
                if (dbshipment != null)
                {
                    dbshipment.TrackingNumber = result.TrackingNumber;
                    dbshipment.IntegrationRequest = !string.IsNullOrEmpty(request) ? request : "";
                    dbshipment.IntegrationResponse = !string.IsNullOrEmpty(response) ? response : "";
                    dbContext.SaveChanges();

                    //Save Tracking No in TrackingNumberRoute Table for tracking
                    TrackingNumberRoute tnr = new TrackingNumberRoute();
                    tnr.Number = result.TrackingNumber;
                    tnr.ShipmentId = shipment.ExpressId;
                    tnr.ModuleType = "ExpressBooking";
                    tnr.IsTrackingNumber = true;
                    dbContext.TrackingNumberRoutes.Add(tnr);
                    dbContext.SaveChanges();
                }
            }
            if (shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.CANADAPOST)
            {
                var dbshipment = dbContext.Expresses.Find(shipment.ExpressId);
                if (dbshipment != null)
                {
                    dbshipment.TrackingNumber = result.TrackingNumber;
                    dbshipment.IntegrationRequest = !string.IsNullOrEmpty(request) ? request : "";
                    dbshipment.IntegrationResponse = !string.IsNullOrEmpty(response) ? response : "";
                    dbContext.SaveChanges();

                    //Save Tracking No in TrackingNumberRoute Table for tracking
                    TrackingNumberRoute tnr = new TrackingNumberRoute();
                    tnr.Number = result.TrackingNumber;
                    tnr.ShipmentId = shipment.ExpressId;
                    tnr.ModuleType = "ExpressBooking";
                    tnr.IsTrackingNumber = true;
                    dbContext.TrackingNumberRoutes.Add(tnr);
                    dbContext.SaveChanges();
                }
            }
            if (shipment.Service.HubCarrier == FrayteCourierCompany.Yodel)
            {
                var dbshipment = dbContext.Expresses.Find(shipment.ExpressId);
                if (dbshipment != null)
                {
                    dbshipment.TrackingNumber = result.TrackingNumber;
                    dbshipment.IntegrationRequest = !string.IsNullOrEmpty(request) ? request : "";
                    dbshipment.IntegrationResponse = !string.IsNullOrEmpty(response) ? response : "";
                    dbContext.SaveChanges();

                    //Save Tracking No in TrackingNumberRoute Table for tracking
                    TrackingNumberRoute tnr = new TrackingNumberRoute();
                    tnr.Number = result.TrackingNumber;
                    tnr.ShipmentId = shipment.ExpressId;
                    tnr.ModuleType = "ExpressBooking";
                    tnr.IsTrackingNumber = true;
                    dbContext.TrackingNumberRoutes.Add(tnr);
                    dbContext.SaveChanges();
                }
            }

            if (shipment.Service.HubCarrier == FrayteCourierCompany.Hermes)
            {
                var dbshipment = dbContext.Expresses.Find(shipment.ExpressId);
                if (dbshipment != null)
                {
                    dbshipment.TrackingNumber = result.TrackingNumber;
                    dbshipment.IntegrationRequest = !string.IsNullOrEmpty(request) ? request : "";
                    dbshipment.IntegrationResponse = !string.IsNullOrEmpty(response) ? response : "";
                    dbContext.SaveChanges();

                    //Save Tracking No in TrackingNumberRoute Table for tracking
                    TrackingNumberRoute tnr = new TrackingNumberRoute();
                    tnr.Number = result.TrackingNumber;
                    tnr.ShipmentId = shipment.ExpressId;
                    tnr.ModuleType = "ExpressBooking";
                    tnr.IsTrackingNumber = true;
                    dbContext.TrackingNumberRoutes.Add(tnr);
                    dbContext.SaveChanges();
                }
            }

            if (shipment.Service.HubCarrier == FrayteCourierCompany.USPS)
            {
                var dbshipment = dbContext.Expresses.Find(shipment.ExpressId);
                if (dbshipment != null)
                {
                    dbshipment.TrackingNumber = result.TrackingNumber;
                    dbshipment.IntegrationRequest = !string.IsNullOrEmpty(request) ? request : "";
                    dbshipment.IntegrationResponse = !string.IsNullOrEmpty(response) ? response : "";
                    dbContext.SaveChanges();

                    //Save Tracking No in TrackingNumberRoute Table for tracking
                    TrackingNumberRoute tnr = new TrackingNumberRoute();
                    tnr.Number = result.TrackingNumber;
                    tnr.ShipmentId = shipment.ExpressId;
                    tnr.ModuleType = "ExpressBooking";
                    tnr.IsTrackingNumber = true;
                    dbContext.TrackingNumberRoutes.Add(tnr);
                    dbContext.SaveChanges();
                }
            }

            var dbShipment = dbContext.Expresses.Find(shipment.ExpressId);
            if (dbShipment != null)
            {
                dbShipment.ShipmentStatusId = (int)FrayteExpressShipmentStatus.Current;
                dbShipment.HubCarrierServiceId = shipment.Service.HubCarrierServiceId;
                dbShipment.FrayteNumber = CommonConversion.GetNewFrayteNumber();
                shipment.FrayteNumber = dbShipment.FrayteNumber;
                dbContext.SaveChanges();
            }
        }

        public void SaveLogisticLabelImage(int expressId, string data1)
        {
            var shipment = dbContext.Expresses.Find(expressId);
            if (shipment != null)
            {
                shipment.LogisticLabelImage = data1;
                dbContext.SaveChanges();
            }
        }

        public string GetTrackingNo(int directShipmentId)
        {
            return dbContext.Expresses.Find(directShipmentId).TrackingNumber;
        }

        public List<ExpressPackageModel> GetPackageDetails(int directShipmentId)
        {
            return dbContext.ExpressDetails.Where(p => p.ExpressId == directShipmentId).Select(p => new ExpressPackageModel
            {
                ExpressDetailId = p.ExpressDetailId,
                ExpressId = p.ExpressId,
                CartonValue = p.CartonQty,
                Height = p.Height,
                Content = p.PiecesContent,
                Length = p.Length,
                Value = p.Value,
                Weight = p.Weight,
                Width = p.Width
            }).ToList();
        }

        public bool SaveTrackingDetail(ExpressShipmentModel shipment, IntegrtaionResult result)
        {
            var count = 1;
            foreach (var Obj in result.PieceTrackingDetails)
            {
                if (!Obj.PieceTrackingNumber.Contains("AirwayBillNumber_"))
                {
                    Frayte.Services.Models.Package package = new Frayte.Services.Models.Package();
                    package.DirectShipmentDetailId = Obj.DirectShipmentDetailId;

                    package.TrackingNo = Obj.PieceTrackingNumber;
                    //package.LabelName = Obj.LabelName;
                    SavePackageDetail(package, "", Obj.DirectShipmentDetailId, shipment.Service.HubCarrier, count);
                    count++;
                }
                else
                {
                    Frayte.Services.Models.Package package = new Frayte.Services.Models.Package();
                    package.DirectShipmentDetailId = Obj.DirectShipmentDetailId;
                    package.TrackingNo = Obj.PieceTrackingNumber;
                    package.LabelName = Obj.LabelName;
                    SavePackageDetail(package, "", Obj.DirectShipmentDetailId, shipment.Service.HubCarrier, count);
                    count++;
                }
            }
            return true;
        }

        public Express GetShipmentImage(int directShipmentId)
        {
            return dbContext.Expresses.Find(directShipmentId);
        }

        public void SavePackageDetail(CourierPieceDetail pieceDetail, string CourierCompany)
        {
            Frayte.Services.Models.Package package = new Frayte.Services.Models.Package();
            package.LabelName = pieceDetail.PieceTrackingNumber;
            SavePackageDetail(package, pieceDetail.LabelName, pieceDetail.DirectShipmentDetailId, CourierCompany, 0);
        }

        public void SavePackageDetail(Package package, string ImageName, int expressShipmentDetailId, string CourierCompany, int count)
        {
            if (CourierCompany == FrayteLogisticServiceType.Hermes)
            {
                if (count == 0)
                {
                    var detail = dbContext.ExpressDetailPackageLabels.Where(p => p.TrackingNumber == package.LabelName &&
                                                                             p.PackageLabelName == null).FirstOrDefault();
                    if (detail != null)
                    {
                        if (detail.PackageLabelName == "" || detail.PackageLabelName == null)
                        {
                            detail.PackageLabelName = ImageName;
                            detail.IsDownloaded = false;
                            dbContext.SaveChanges();
                        }
                    }
                }
                else if (count > 0)
                {
                    ExpressDetailPackageLabel sph = new ExpressDetailPackageLabel();
                    sph.ExpressShipmentDetailId = expressShipmentDetailId;
                    sph.TrackingNumber = package.LabelName;
                    sph.PackageLabelName = ImageName;
                    sph.IsDownloaded = false;
                    dbContext.ExpressDetailPackageLabels.Add(sph);
                    dbContext.SaveChanges();
                }
            }
            else
            {
                var detail = dbContext.ExpressDetailPackageLabels.Where(p => p.TrackingNumber == package.LabelName &&
                                                                         p.ExpressShipmentDetailId == expressShipmentDetailId).FirstOrDefault();
                if (detail != null)
                {
                    detail.PackageLabelName = ImageName;
                    detail.IsDownloaded = false;
                    dbContext.SaveChanges();
                }
                else
                {
                    ExpressDetailPackageLabel sph = new ExpressDetailPackageLabel();
                    sph.ExpressShipmentDetailId = expressShipmentDetailId;
                    sph.TrackingNumber = package.LabelName;
                    sph.PackageLabelName = ImageName;
                    sph.IsDownloaded = false;
                    dbContext.ExpressDetailPackageLabels.Add(sph);
                    dbContext.SaveChanges();
                }
            }
        }

        public bool SaveExpressPiecesTrackingNo(string ExpressPiecesTrackingNo, int ExpressId)
        {
            var Result = dbContext.TrackingNumberRoutes.Where(a => a.Number == ExpressPiecesTrackingNo && a.ModuleType == FrayteShipmentServiceType.ExpressBooking && a.IsPiecesTrackingNo == true).FirstOrDefault();
            if (Result == null)
            {
                TrackingNumberRoute TNR = new TrackingNumberRoute();
                TNR.Number = ExpressPiecesTrackingNo;
                TNR.ShipmentId = ExpressId;
                TNR.ModuleType = FrayteShipmentServiceType.ExpressBooking;
                TNR.IsPiecesTrackingNo = true;
                dbContext.TrackingNumberRoutes.Add(TNR);
                dbContext.SaveChanges();
                return true;
            }
            return true;
        }

        public void SaveLogisticLabel(int directShipmentId, string logisticLabel)
        {
            var shipment = dbContext.Expresses.Find(directShipmentId);
            if (shipment != null)
            {
                shipment.LogisticLabel = logisticLabel;
                dbContext.SaveChanges();
            }
        }

        public List<ExpressDetailPackageLabel> GetPackageTracking(int expressDetailId)
        {
            return dbContext.ExpressDetailPackageLabels.Where(p => p.ExpressShipmentDetailId == expressDetailId).ToList();
        }

        public List<HubService> ExpressSerice(ExpressServiceObj sericeObj)
        {
            List<HubService> services = new List<HubService>();

            HubService service = new HubService();

            services.Add(service);

            throw new NotImplementedException();
        }

        public void MappingCourierPieceDetail(IntegrtaionResult result, ExpressShipmentModel shipment)
        {
            if (shipment.ExpressId > 0)
            {
                int k = 0, i = 0;
                List<int> _shiId = new List<int>();
                foreach (var Obj in shipment.Packages)
                {
                    _shiId = GetExpressShipmetDetailUIds(shipment.ExpressId);
                    for (int j = 1; j <= Obj.CartonValue; j++)
                    {
                        result.PieceTrackingDetails[k].DirectShipmentDetailId = _shiId[i];
                        k++;
                    }
                    i++;
                }
            }
        }

        public List<int> GetExpressShipmetDetailUIds(int expressId)
        {
            return dbContext.ExpressDetails.Where(p => p.ExpressId == expressId).Select(p => p.ExpressDetailId).ToList();
        }

        private void SavePackageDetail(ExpressShipmentModel shipment)
        {
            ExpressDetail expressPackage;

            try
            {
                if (shipment.Packages != null && shipment.Packages.Count > 0)
                {
                    if (shipment.ExpressId == 0)
                    {
                        foreach (var pkg in shipment.Packages)
                        {
                            expressPackage = new ExpressDetail();
                            if (pkg.CartonValue > 0)
                            {
                                expressPackage.ExpressId = shipment.ExpressId;
                                expressPackage.CartonQty = pkg.CartonValue;
                                expressPackage.Height = pkg.Height;
                                expressPackage.Weight = pkg.Weight;
                                expressPackage.Width = pkg.Width;
                                expressPackage.PiecesContent = pkg.Content;
                                expressPackage.Length = pkg.Length;
                                expressPackage.Value = pkg.Value;
                                expressPackage.ProductCatalogId = pkg.ProductCatalogId;
                                dbContext.ExpressDetails.Add(expressPackage);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        var expressDetails = dbContext.ExpressDetails.Where(x => x.ExpressId == shipment.ExpressId).ToList();
                        if (expressDetails != null && expressDetails.Count > 0)
                        {
                            if (shipment.Packages.Count == expressDetails.Count)
                            {
                                for (int i = 0; i < expressDetails.Count; i++)
                                {
                                    var ed = dbContext.ExpressDetails.Find(expressDetails[i].ExpressDetailId);
                                    if (ed != null)
                                    {
                                        ed.CartonQty = shipment.Packages[i].CartonValue;
                                        ed.Height = shipment.Packages[i].Height;
                                        ed.Weight = shipment.Packages[i].Weight;
                                        ed.Width = shipment.Packages[i].Width;
                                        ed.PiecesContent = shipment.Packages[i].Content;
                                        ed.Length = shipment.Packages[i].Length;
                                        ed.Value = shipment.Packages[i].Value;
                                        ed.ProductCatalogId = shipment.Packages[i].ProductCatalogId;
                                        dbContext.Entry(ed).State = System.Data.Entity.EntityState.Modified;
                                        dbContext.SaveChanges();
                                    }
                                }
                            }
                            else if (shipment.Packages.Count < expressDetails.Count)
                            {
                                int j = 0;
                                for (int i = 0; i < shipment.Packages.Count; i++)
                                {
                                    var ed = dbContext.ExpressDetails.Find(expressDetails[i].ExpressDetailId);
                                    if (ed != null)
                                    {
                                        ed.CartonQty = shipment.Packages[i].CartonValue;
                                        ed.Height = shipment.Packages[i].Height;
                                        ed.Weight = shipment.Packages[i].Weight;
                                        ed.Width = shipment.Packages[i].Width;
                                        ed.PiecesContent = shipment.Packages[i].Content;
                                        ed.Length = shipment.Packages[i].Length;
                                        ed.Value = shipment.Packages[i].Value;
                                        ed.ProductCatalogId = shipment.Packages[i].ProductCatalogId;
                                        dbContext.Entry(ed).State = System.Data.Entity.EntityState.Modified;
                                        dbContext.SaveChanges();
                                        j++;
                                    }
                                }
                                if (j > 0)
                                {
                                    var ed = dbContext.ExpressDetails.Find(expressDetails[j].ExpressDetailId);
                                    if (ed != null)
                                    {
                                        dbContext.ExpressDetails.Remove(ed);
                                        dbContext.SaveChanges();
                                    }
                                }
                            }
                            else if (shipment.Packages.Count > expressDetails.Count)
                            {
                                int k = 0;
                                for (int i = 0; i < expressDetails.Count; i++)
                                {
                                    var ed = dbContext.ExpressDetails.Find(expressDetails[i].ExpressDetailId);
                                    if (ed != null)
                                    {
                                        ed.CartonQty = shipment.Packages[i].CartonValue;
                                        ed.Height = shipment.Packages[i].Height;
                                        ed.Weight = shipment.Packages[i].Weight;
                                        ed.Width = shipment.Packages[i].Width;
                                        ed.PiecesContent = shipment.Packages[i].Content;
                                        ed.Length = shipment.Packages[i].Length;
                                        ed.Value = shipment.Packages[i].Value;
                                        ed.ProductCatalogId = shipment.Packages[i].ProductCatalogId;
                                        dbContext.Entry(ed).State = System.Data.Entity.EntityState.Modified;
                                        dbContext.SaveChanges();
                                        k++;
                                    }
                                }
                                if (k > 0)
                                {
                                    var ed = dbContext.ExpressDetails.Find(expressDetails[k].ExpressDetailId);
                                    if (ed != null)
                                    {
                                        dbContext.ExpressDetails.Remove(ed);
                                        dbContext.SaveChanges();
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var pkg in shipment.Packages)
                            {
                                expressPackage = new ExpressDetail();
                                if (pkg.CartonValue > 0)
                                {
                                    expressPackage.ExpressId = shipment.ExpressId;
                                    expressPackage.CartonQty = pkg.CartonValue;
                                    expressPackage.Height = pkg.Height;
                                    expressPackage.Weight = pkg.Weight;
                                    expressPackage.Width = pkg.Width;
                                    expressPackage.PiecesContent = pkg.Content;
                                    expressPackage.Length = pkg.Length;
                                    expressPackage.Value = pkg.Value;
                                    expressPackage.ProductCatalogId = pkg.ProductCatalogId;
                                    dbContext.ExpressDetails.Add(expressPackage);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                throw (new FrayteApiException("ShipmentPackageError", Ex));
            }
        }

        public HubService GetShipmentService(int expressId)
        {
            var service = (from r in dbContext.Expresses

                           join hcs in dbContext.HubCarrierServices on r.HubCarrierServiceId equals hcs.HubCarrierServiceId
                           join hc in dbContext.HubCarriers on hcs.HubCarrierId equals hc.HubCarrierId
                           where r.ExpressId == expressId
                           select new HubService
                           {
                               ActualWeight = r.ActualWeight.Value,
                               HubCarrierId = hcs.HubCarrierId,
                               CarrierLogo = hcs.Logo,
                               HubCarrier = hc.Carrier,
                               HubCarrierDisplay = hc.Carrier,
                               HubCarrierServiceId = hcs.HubCarrierServiceId,
                               RateType = hcs.ServiceType,
                               RateTypeDisplay = hcs.ServiceType,
                               TransitTime = hcs.TransitTime,
                               WeightRoundLogic = hcs.WeightRoundLogic,
                               BillingWeight = r.ActualWeight.Value,
                           }).FirstOrDefault();

            return service;
        }

        private void SaveShipmentDetail(ExpressShipmentModel shipment)
        {
            Express dbShipment;
            try
            {
                if (shipment.ExpressId > 0)
                {
                    dbShipment = dbContext.Expresses.Find(shipment.ExpressId);
                    dbShipment.ShipmentStatusId = (int)FrayteExpressShipmentStatus.Draft;
                    dbShipment.CustomerId = shipment.CustomerId;
                    dbShipment.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
                    dbShipment.FromAddressId = shipment.ShipFrom.ExpressAddressId;
                    dbShipment.ToAddressId = shipment.ShipTo.ExpressAddressId;
                    if (shipment.DeclaredCurrency != null)
                    {
                        dbShipment.DeclaredCurrencyCode = shipment.DeclaredCurrency.CurrencyCode;
                    }

                    dbShipment.ShipmentReference = shipment.ShipmentReference;
                    dbShipment.CreatedBy = shipment.CreatedBy;
                    dbShipment.CreatedOnUtc = DateTime.UtcNow;
                    dbShipment.DeclaredValue = shipment.DeclaredValue;
                    dbShipment.PaymentPartyTaxAndDuty = shipment.PayTaxAndDuties;
                    dbShipment.PacakgeCalculationType = shipment.PakageCalculatonType;
                    dbShipment.ActualWeight = shipment.ActualWeight;
                    dbShipment.AdditionalInfo = shipment.AdditionalInfo;
                    dbShipment.ParcelType = shipment.ParcelType == null ? "" : shipment.ParcelType.ParcelType;
                    dbShipment.IsActive = true;
                    dbContext.SaveChanges();
                }
                else
                {
                    dbShipment = dbContext.Expresses.Where(p => p.AWBBarcode == shipment.AWBNumber.Replace(" ", "")).FirstOrDefault();
                    if (dbShipment != null)
                    {
                        shipment.ExpressId = dbShipment.ExpressId;
                        dbShipment.ShipmentStatusId = (int)FrayteExpressShipmentStatus.Draft;
                        dbShipment.CustomerId = shipment.CustomerId;
                        dbShipment.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
                        dbShipment.FromAddressId = shipment.ShipFrom.ExpressAddressId;
                        dbShipment.ToAddressId = shipment.ShipTo.ExpressAddressId;
                        if (shipment.DeclaredCurrency != null)
                        {
                            dbShipment.DeclaredCurrencyCode = shipment.DeclaredCurrency.CurrencyCode;
                        }
                        dbShipment.ShipmentReference = shipment.ShipmentReference;
                        dbShipment.CreatedBy = shipment.CreatedBy;
                        dbShipment.CreatedOnUtc = DateTime.UtcNow;
                        dbShipment.DeclaredValue = shipment.DeclaredValue;
                        dbShipment.PaymentPartyTaxAndDuty = shipment.PayTaxAndDuties;
                        dbShipment.PacakgeCalculationType = shipment.PakageCalculatonType;
                        dbShipment.ActualWeight = shipment.ActualWeight;
                        dbShipment.AdditionalInfo = shipment.AdditionalInfo;
                        dbShipment.ParcelType = shipment.ParcelType == null ? "" : shipment.ParcelType.ParcelType;
                        dbShipment.IsActive = true;
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw (new FrayteApiException("ShipmentDetailError", ex));
            }
        }

        public List<ExpressState> GetCountryState(int CountryId)
        {
            List<ExpressState> ESList = new List<ExpressState>();

            ESList = (from ES in dbContext.HubCarrierServiceCountryStates
                      where ES.CountryId == CountryId
                      select new ExpressState
                      {
                          HubCarrierServiceCountryStateId = ES.HubCarrierServiceCountryStateId,
                          HubCarrierServiceId = ES.HubCarrierServiceId,
                          HubId = ES.HubId,
                          CountryId = ES.CountryId,
                          State = ES.State,
                          StateDisplay = ES.StateDisplay
                      }).OrderBy(a => a.State).ToList();

            return ESList;
        }

        public List<ExpressState> GetFromCountryState(int CountryId)
        {
            List<ExpressState> ESList = new List<ExpressState>();

            ESList = (from ES in dbContext.CountryStates
                      where ES.CountryId == CountryId
                      select new ExpressState
                      {
                          CountryId = ES.CountryId,
                          State = ES.StateName,
                          StateDisplay = ES.StateCode
                      }).OrderBy(a => a.State).ToList();

            return ESList;
        }

        #endregion

        #region  GetScanned ShipmentDetail

        public ExpressShipmentModel ScannedShipmentDetail(int shipmentId, string callingType)
        {
            ExpressShipmentModel expressModel = new ExpressShipmentModel();
            expressModel.ExpressId = shipmentId;

            //Step 1: Get Shipment Detail
            GetShipmnetDetail(expressModel);

            //Step 2: Get Ship From , Ship To and   Notify Party Address detail
            GetDirectShipmentAddressDetail(expressModel);

            // Step 2.1:  Get hub Address
            expressModel.Packages = new List<ExpressPackageModel>(); //  for package grid
            expressModel.DetailPackages = new List<ExpressPackageDetailModel>(); //  for package download 

            //Step 3: Get PackageDetails 
            GetShipmentPackagesDetail(expressModel, callingType);

            //Step: 4 Get Custom detail
            GetShipmentCustomDetail(expressModel, callingType);

            if (callingType == FrayteCallingType.ShipmentClone)
            {
                expressModel.ExpressId = 0;
                expressModel.ShipFrom.ExpressAddressId = 0;
                expressModel.ShipTo.ExpressAddressId = 0;
                if (expressModel.CustomInformation != null)
                {
                    expressModel.CustomInformation.ShipmentCustomDetailId = 0;
                }

                if (expressModel.Packages.Count > 0)
                {
                    foreach (var item in expressModel.Packages)
                    {
                        item.ExpressDetailId = 0;
                        item.ExpressId = 0;
                    }
                }
            }
            return expressModel;
        }

        private void GetShipmentCustomDetail(ExpressShipmentModel expressModel, string callingType)
        {
            var dbCustom = dbContext.ExpressCustomDetails.Where(p => p.ShipmentId == expressModel.ExpressId).FirstOrDefault();
            expressModel.CustomInformation = new ExpressCustomInformationModel();
            if (dbCustom != null)
            {
                expressModel.CustomInformation.ShipmentCustomDetailId = dbCustom.ExpressCustomDetailId;
                expressModel.CustomInformation.ShipmentId = dbCustom.ShipmentId;
                expressModel.CustomInformation.ContentsType = dbCustom.ContentsType;
                expressModel.CustomInformation.CustomsCertify = dbCustom.CustomsCertify;
                expressModel.CustomInformation.CustomsSigner = dbCustom.CustomsSigner;
                expressModel.CustomInformation.ContentsExplanation = dbCustom.ContentsExplanation;
                expressModel.CustomInformation.RestrictionType = dbCustom.RestrictionType;
                expressModel.CustomInformation.RestrictionComments = dbCustom.RestrictionComments;
                expressModel.CustomInformation.NonDeliveryOption = dbCustom.NonDeliveryOption;
            }
        }

        private void GetShipmentPackagesDetail(ExpressShipmentModel expressModel, string callingType)
        {
            var details = dbContext.ExpressDetails.Where(p => p.ExpressId == expressModel.ExpressId).ToList();

            ExpressPackageModel package;
            if (details != null && details.Count > 0)
            {
                ExpressPackageDetailModel detailPackage;
                foreach (var item in details)
                {
                    package = new ExpressPackageModel();
                    package.CartonValue = item.CartonQty;
                    package.Content = item.PiecesContent;
                    package.ExpressDetailId = item.ExpressDetailId;
                    package.ExpressId = expressModel.ExpressId;
                    package.Height = item.Height;
                    package.Width = item.Width;
                    package.Weight = item.Weight;
                    package.Length = item.Length;
                    package.Value = item.Value;
                    expressModel.Packages.Add(package);

                    var packagelabels = dbContext.ExpressDetailPackageLabels.Where(p => p.ExpressShipmentDetailId == item.ExpressDetailId).ToList();

                    foreach (var detail in packagelabels)
                    {
                        detailPackage = new ExpressPackageDetailModel();
                        detailPackage.ExpressDetailId = detail.ExpressShipmentDetailId;
                        detailPackage.ExpressDetailPackageLabelId = detail.ExpressDetailPackageLabelId;
                        detailPackage.CartonValue = 1;
                        detailPackage.Height = item.Height;
                        detailPackage.Width = item.Width;
                        detailPackage.Weight = item.Weight;
                        detailPackage.Length = item.Length;
                        detailPackage.Value = item.Value;
                        detailPackage.Content = item.PiecesContent;
                        if (!string.IsNullOrEmpty(detail.PackageLabelName))
                        {
                            if (detail.PackageLabelName.Contains(".png"))
                            {
                                detailPackage.LabelName = detail.PackageLabelName.Replace(".png", ".pdf");
                                detailPackage.LabelPath = AppSettings.WebApiPath + "/PackageLabel/Express/" + expressModel.ExpressId + "/" + detail.PackageLabelName.Replace(".png", ".pdf");
                            }
                            else
                            {
                                detailPackage.LabelName = detail.PackageLabelName.Replace(".jpg", ".pdf");
                                detailPackage.LabelPath = AppSettings.WebApiPath + "/PackageLabel/Express/" + expressModel.ExpressId + "/" + detail.PackageLabelName.Replace(".jpg", ".pdf");
                            }
                        }
                        detailPackage.TrackingNumber = detail.TrackingNumber;
                        detailPackage.IsDownloaded = detail.IsDownloaded;
                        expressModel.DetailPackages.Add(detailPackage);
                    }
                }
            }
            else
            {
                package = new ExpressPackageModel();
                expressModel.Packages.Add(package);
            }
        }

        private void GetDirectShipmentAddressDetail(ExpressShipmentModel expressModel)
        {
            ExpressAddress shipFrom = dbContext.ExpressAddresses.Find(expressModel.ShipFrom.ExpressAddressId);
            if (shipFrom != null)
            {
                expressModel.ShipFrom.Address = shipFrom.Address1;
                expressModel.ShipFrom.Address2 = shipFrom.Address2;
                expressModel.ShipFrom.Area = shipFrom.Area;
                expressModel.ShipFrom.City = shipFrom.City;
                expressModel.ShipFrom.CompanyName = shipFrom.CompanyName;
                expressModel.ShipFrom.FirstName = shipFrom.ContactFirstName;
                expressModel.ShipFrom.LastName = shipFrom.ContactLastName;
                expressModel.ShipFrom.Phone = shipFrom.PhoneNo;
                expressModel.ShipFrom.PostCode = shipFrom.Zip;
                expressModel.ShipFrom.State = shipFrom.State;
                expressModel.ShipFrom.Email = shipFrom.Email;
                expressModel.ShipFrom.Country = new FrayteCountryCode();
                expressModel.ShipFrom.Country.CountryId = shipFrom.CountryId;
                if (expressModel.ShipFrom.Country.CountryId > 0)
                {
                    Country shipFromCountry = dbContext.Countries.Find(expressModel.ShipFrom.Country.CountryId);
                    if (shipFromCountry != null)
                    {
                        expressModel.ShipFrom.Country.Code = shipFromCountry.CountryCode;
                        expressModel.ShipFrom.Country.Code2 = shipFromCountry.CountryCode2;
                        expressModel.ShipFrom.Country.Name = shipFromCountry.CountryName;
                        expressModel.ShipFrom.Country.CountryPhoneCode = shipFromCountry.CountryPhoneCode;
                    }
                }
            }

            // Ship To 
            ExpressAddress shipTo = dbContext.ExpressAddresses.Find(expressModel.ShipTo.ExpressAddressId);
            if (shipTo != null)
            {
                expressModel.ShipTo.Address = shipTo.Address1;
                expressModel.ShipTo.Address2 = shipTo.Address2;
                expressModel.ShipTo.Area = shipTo.Area;
                expressModel.ShipTo.City = shipTo.City;
                expressModel.ShipTo.CompanyName = shipTo.CompanyName;
                expressModel.ShipTo.FirstName = shipTo.ContactFirstName;
                expressModel.ShipTo.LastName = shipTo.ContactLastName;
                expressModel.ShipTo.Phone = shipTo.PhoneNo;
                expressModel.ShipTo.PostCode = shipTo.Zip;
                expressModel.ShipTo.State = shipTo.State;
                expressModel.ShipTo.Email = shipTo.Email;
                expressModel.ShipTo.Country = new FrayteCountryCode();
                expressModel.ShipTo.Country.CountryId = shipTo.CountryId;
                if (expressModel.ShipTo.Country.CountryId > 0)
                {
                    Country shipToCountry = dbContext.Countries.Find(expressModel.ShipTo.Country.CountryId);
                    if (shipToCountry != null)
                    {
                        expressModel.ShipTo.Country.Code = shipToCountry.CountryCode;
                        expressModel.ShipTo.Country.Code2 = shipToCountry.CountryCode2;
                        expressModel.ShipTo.Country.Name = shipToCountry.CountryName;
                        expressModel.ShipTo.Country.CountryPhoneCode = shipToCountry.CountryPhoneCode;
                    }
                }
            }
        }

        private void GetShipmnetDetail(ExpressShipmentModel expressModel)
        {
            var dbShipment = dbContext.Expresses.Find(expressModel.ExpressId);
            if (dbShipment != null)
            {
                expressModel.ShipFrom = new ExpressAddressModel();
                expressModel.ShipFrom.ExpressAddressId = dbShipment.FromAddressId.HasValue ? dbShipment.FromAddressId.Value : 0;

                expressModel.ShipTo = new ExpressAddressModel();
                expressModel.ShipTo.ExpressAddressId = dbShipment.ToAddressId.HasValue ? dbShipment.ToAddressId.Value : 0;

                expressModel.TrackingNo = dbShipment.TrackingNumber;

                if (dbShipment.HubCarrierServiceId.HasValue && dbShipment.HubCarrierServiceId.Value > 0)
                {
                    var service = (from r in dbContext.HubCarrierServices
                                   join c in dbContext.HubCarriers on r.HubCarrierId equals c.HubCarrierId
                                   where r.HubCarrierServiceId == dbShipment.HubCarrierServiceId.Value
                                   select new
                                   {
                                       Carrier = c.Carrier,
                                       Service = r.ServiceTypeDisplay
                                   }).FirstOrDefault();

                    if (service != null)
                    {
                        if (service.Carrier == "SkyPostal")
                        {
                            expressModel.Carrier = "UPS Ground";
                            expressModel.CarrierService = "UPS Ground";
                        }
                        else
                        {
                            expressModel.Carrier = service.Carrier;
                            expressModel.CarrierService = service.Service;
                        }
                    }
                }

                // Label path
                expressModel.LabelName = dbShipment.LogisticLabel;
                expressModel.LabelPath = AppSettings.WebApiPath + "/PackageLabel/Express/" + expressModel.ExpressId + "/" + dbShipment.LogisticLabel;

                var user1 = dbContext.Users.Find(dbShipment.CreatedBy);
                if (user1 != null)
                {
                    expressModel.ScannedBy = user1.ContactName;
                    var userInfo1 = (from r in dbContext.Users
                                     join tz in dbContext.Timezones on r.TimezoneId equals tz.TimezoneId
                                     where r.UserId == dbShipment.CreatedBy
                                     select tz).FirstOrDefault();

                    expressModel.CreatedByName = user1.ContactName;

                    var UserTimeZoneInfo1 = TimeZoneInfo.FindSystemTimeZoneById(userInfo1.Name);
                    expressModel.CreatedOnUtc = UtilityRepository.UtcDateToOtherTimezone(dbShipment.CreatedOnUtc, dbShipment.CreatedOnUtc.TimeOfDay, UserTimeZoneInfo1).Item1;
                    expressModel.CreatedOnTime = UtilityRepository.GetFormattedTimeFromString(UtilityRepository.UtcDateToOtherTimezone(dbShipment.CreatedOnUtc, dbShipment.CreatedOnUtc.TimeOfDay, UserTimeZoneInfo1).Item2);

                    // Append the created by time zone in time  
                    expressModel.CreatedOnTime += " " + userInfo1.OffsetShort;
                    expressModel.CreatedBy = dbShipment.CreatedBy;
                }
                expressModel.TrackingNumber = dbShipment.TrackingNumber;
                expressModel.PayTaxAndDuties = dbShipment.PaymentPartyTaxAndDuty;
                expressModel.PakageCalculatonType = dbShipment.PacakgeCalculationType;
                expressModel.ActualWeight = dbShipment.ActualWeight.HasValue ? dbShipment.ActualWeight.Value : 0.00M;
                expressModel.ShipmentReference = dbShipment.ShipmentReference;
                expressModel.CustomerId = dbShipment.CustomerId;
                expressModel.AWBNumber = dbShipment.AWBBarcode.Substring(0, 3) + " " + dbShipment.AWBBarcode.Substring(3, 3) + " " + dbShipment.AWBBarcode.Substring(6, 3) + " " + dbShipment.AWBBarcode.Substring(9, 3);
                expressModel.ShipmentStatusId = dbShipment.ShipmentStatusId;
                expressModel.FrayteNumber = dbShipment.FrayteNumber;

                var status = dbContext.ShipmentStatus.Find(dbShipment.ShipmentStatusId);
                if (status != null)
                {
                    expressModel.ShipmentStatusDisplay = status.DisplayStatusName;
                }
                var user = dbContext.Users.Find(dbShipment.AWBScannedBy);
                if (user != null)
                {
                    expressModel.ScannedBy = user.ContactName;
                    var userInfo = (from r in dbContext.Users
                                    join tz in dbContext.Timezones on r.TimezoneId equals tz.TimezoneId
                                    where r.UserId == dbShipment.AWBScannedBy
                                    select tz).FirstOrDefault();

                    var UserTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userInfo.Name);
                    expressModel.ScannedOnDate = UtilityRepository.UtcDateToOtherTimezone(dbShipment.AWBScannedOnUtc, dbShipment.AWBScannedOnUtc.TimeOfDay, UserTimeZoneInfo).Item1;
                    expressModel.ScannedOnTime = UtilityRepository.GetFormattedTimeFromString(UtilityRepository.UtcDateToOtherTimezone(dbShipment.AWBScannedOnUtc, dbShipment.AWBScannedOnUtc.TimeOfDay, UserTimeZoneInfo).Item2);

                    // Append the created by time zone in time  
                    expressModel.ScannedOnTime += " " + userInfo.OffsetShort;
                    expressModel.ScannedById = dbShipment.AWBScannedBy;
                    expressModel.AdditionalInfo = dbShipment.AdditionalInfo;
                }
                expressModel.DeclaredCurrency = new CurrencyType();
                expressModel.DeclaredCurrency = dbContext.CurrencyTypes.Where(p => p.CurrencyCode == dbShipment.DeclaredCurrencyCode).FirstOrDefault();

                expressModel.ParcelType = new FrayteParcelType();
                expressModel.ParcelType.ParcelType = dbShipment.ParcelType;
                expressModel.ParcelType.ParcelDescription = dbShipment.ParcelType;
            }
        }

        #endregion

        #region AvinashCode

        public List<object> getExpressTimeZone()
        {
            List<object> xyz = new List<object>();
            var user = dbContext.UserAdditionals.Where(x => x.IsExpressSolutions == true).ToList();
            var date = DateTime.UtcNow.AddMinutes(30);
            foreach (var abc in user)
            {
                var express = dbContext.Expresses.Where(x => x.ShipmentStatusId == 37 && x.CustomerId == abc.UserId).ToList();
                if (express != null)
                {
                    foreach (var my in express)
                    {
                        if (my.CreatedOnUtc.Date == date.Date && my.CreatedOnUtc.TimeOfDay > date.TimeOfDay)
                        {
                            xyz.Add(my);
                        }
                    }
                }
            }

            return xyz;
        }

        public List<ProductCatalog> GetProductCatalog(int CustomerId, int HubId)
        {
            List<ProductCatalog> my = dbContext.ProductCatalogs.Where(x => x.CustomerId == CustomerId && x.HubId == HubId).ToList();
            return my;
        }

        public void UpdateExpress(int ExpressId, int HubcarrierServiceId, string TrackingNo)
        {
            var hub = dbContext.Expresses.Find(ExpressId);

            if (hub != null)
            {
                hub.HubCarrierServiceId = HubcarrierServiceId;
                hub.ShipmentStatusId = 38;
                hub.TrackingNumber = TrackingNo.Replace(" ", "");
                dbContext.Entry(hub).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void UpdateExpressLogisticlabel(int ExpressId, string labelName)
        {
            var hub = dbContext.Expresses.Find(ExpressId);

            if (hub != null)
            {
                hub.LogisticLabel = labelName;
                hub.LogisticLabelImage = labelName.Replace(" (All)", "");
                dbContext.Entry(hub).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void SaveExpressDetailPackagelabel(int ExpressId)
        {
            var hub = (from ex in dbContext.Expresses
                       join ed in dbContext.ExpressDetails on ex.ExpressId equals ed.ExpressId
                       where ed.ExpressId == ExpressId
                       select new
                       {
                           ExpresDetailId = ed.ExpressDetailId,
                           TrackingNumber = ex.TrackingNumber,
                           LabelName = ex.LogisticLabel
                       }).ToList();

            if (hub != null && hub.Count > 0)
            {
                foreach (var dd in hub)
                {
                    ExpressDetailPackageLabel exp = new ExpressDetailPackageLabel();
                    exp.ExpressShipmentDetailId = dd.ExpresDetailId;
                    exp.TrackingNumber = dd.TrackingNumber;
                    exp.PackageLabelName = dd.LabelName;
                    exp.IsDownloaded = false;
                    dbContext.ExpressDetailPackageLabels.Add(exp);
                    dbContext.SaveChanges();
                }
            }
        }

        public List<ManifestShipmentModel> GetExpressTrackAndTraceDetail(ExpressTrackandTrace trackdetail)
        {
            List<ManifestShipmentModel> _detail = new List<ManifestShipmentModel>();
            FrayteOperationZone OperationZone = UtilityRepository.GetOperationZone();
            DateTime? fromdate;
            DateTime? todate;

            if (trackdetail.FromDate.HasValue)
            {
                fromdate = trackdetail.FromDate.Value.Date;
            }
            else
            {
                fromdate = trackdetail.FromDate;
            }

            if (trackdetail.ToDate.HasValue)
            {
                todate = trackdetail.ToDate.Value.Date;
            }
            else
            {
                todate = trackdetail.ToDate;
            }

            var result = dbContext.spGet_ExpressManifests(trackdetail.Mawb, fromdate, todate, trackdetail.AwbNo, trackdetail.TrackingNo, trackdetail.ShipmentStatusId,
                                                          trackdetail.CustomerId, trackdetail.UserId, OperationZone.OperationZoneId, trackdetail.RoleId).ToList();

            if (result != null && result.Count > 0)
            {
                foreach (var Obj in result)
                {
                    ManifestShipmentModel track = new ManifestShipmentModel();
                    track.StatusType = Obj.StatusType;
                    track.Courier = Obj.Courier;
                    track.CustomerCompany = Obj.CustomerCompany;
                    track.CustomerAccountNo = Obj.CustomerAccountNo;
                    track.CustomerName = Obj.CustomerName;
                    track.CustomerCountry = Obj.CustomerCountry;
                    track.CustomerPostCode = Obj.CustomerPostCode;
                    track.CustomerEmail = Obj.CustomerEmail;
                    track.CustomerPhoneNo = Obj.CustomerPhoneNo;
                    track.CustomerAddress1 = Obj.CustomerAddress1;
                    track.CustomerAddress2 = Obj.CustomerAddress2;
                    track.CustomerAddress3 = Obj.CustomerAddress3;
                    track.State = Obj.State;
                    track.City = Obj.City;
                    track.Area = Obj.Suburb;
                    track.ShipFromName = Obj.ShipFromName;
                    track.ShipFromCompany = Obj.ShipFromCompany;
                    track.ShipFromAddress = Obj.ShipFromAddress;
                    track.ShipFromAddress2 = Obj.ShipFromAddress2;
                    track.ShipFromCity = Obj.ShipFromCity;
                    track.ShipFromState = Obj.ShipFromState;
                    track.ShipFromPostCode = Obj.ShipFromPostCode;
                    track.ShipFromCountry = Obj.ShipFromCountry;
                    track.ShipFromPhone = Obj.ShipFromPhone;
                    track.ShipFromEmail = Obj.ShipFromEmail;
                    track.ShipToName = Obj.ShipToName;
                    track.ShipToCompany = Obj.ShipToCompany;
                    track.ShipToAddress = Obj.ShipToAddress;
                    track.ShipToAddress2 = Obj.ShipToAddress2;
                    track.ShipToCity = Obj.ShipToCity;
                    track.ShipToState = Obj.ShipToState;
                    track.ShipToPostCode = Obj.ShipToPostCode;
                    track.ShipToCountry = Obj.ShipToCountry;
                    track.ShipToPhone = Obj.ShipToPhone;
                    track.ShipToEmail = Obj.ShipToEmail;
                    track.ContentsType = Obj.ContentsType;
                    track.RestrictionType = Obj.RestrictionType;
                    track.ContentsExplanation = Obj.ContentsExplanation;
                    track.NonDeliveryOption = Obj.NonDeliveryOption;
                    track.CustomsSigner = Obj.CustomsSigner;
                    track.TrackingNo = Obj.TrackingNo;
                    track.FrayteNumber = Obj.FrayteNumber;
                    track.ParcelType = Obj.ParcelType;
                    track.ShipmentReference = Obj.ShipmentReference;
                    track.ShipmentType = Obj.ShipmentType;
                    track.PackageCalculationType = Obj.PackageCalculationType;
                    track.TotalCartons = int.Parse(Obj.TotalCartons.ToString());
                    track.ShipmentWeight = Obj.ShipmentWeight;
                    track.PaymentPartyTaxAndDuties = Obj.PaymentPartyTaxAndDuties;
                    track.PaymentPartyTaxAndDutiesAcceptedBy = Obj.PaymentPartyTaxAndDutiesAcceptedBy;
                    track.ManifestNumber = Obj.ManifestNumber;
                    track.ShipmentContent = Obj.ShipmentContent;
                    track.CreatedOn = Obj.CreatedOn;
                    track.CreatedBy = Obj.CreatedBy.ToString();
                    track.DeclaredValueCurrency = Obj.DeclaredValueCurrency;
                    track.DeliveryDate = Obj.DeliveryDate.HasValue ? Obj.DeliveryDate.Value.ToString("dd-MMM-yyyy") : "";
                    track.DeliveryTime = Obj.DeliveryTime.HasValue ? Obj.DeliveryTime.Value.ToString() : "";
                    track.SignedBy = Obj.SignedBy;
                    _detail.Add(track);
                }
                return _detail;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
