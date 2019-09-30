using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;
using Frayte.Services.Models.Express;
using Frayte.Services.Models.Tradelane;
using Frayte.Services.Utility;
using System.Web;

namespace Frayte.Services.Business
{
    public class ExpressManifestRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public int TradelaneEntryFromManifest(int shipmentId, int userId, int customerId, int hubId)
        {
            try
            {
                var tradelane = dbContext.TradelaneShipments.Find(shipmentId);
                if (tradelane == null)
                {
                    var shipmentDetail = TradelaneHubInitials(customerId, hubId);

                    // Ship From
                    TradelaneShipmentAddress dbAddressFrom = new TradelaneShipmentAddress();
                    saveAddress(dbAddressFrom, shipmentDetail.ShipFrom);

                    // Ship To
                    TradelaneShipmentAddress dbAddressTo = new TradelaneShipmentAddress();
                    saveAddress(dbAddressTo, shipmentDetail.ShipTo);

                    // Notify Party
                    TradelBookingAdress asddress = new ExpressRepository().GetHubAgentByHubId(hubId);
                    TradelaneShipmentAddress notifyParty = new TradelaneShipmentAddress();
                    saveAddress(notifyParty, asddress);

                    // Shipment Detail 
                    TradelaneShipment shipment = new TradelaneShipment();

                    shipment.FromAddressId = dbAddressFrom.TradelaneShipmentAddressId;
                    shipment.ToAddressId = dbAddressTo.TradelaneShipmentAddressId;
                    shipment.CustomerId = customerId;
                    shipment.NotifyPartyAddressId = notifyParty.TradelaneShipmentAddressId;
                    shipment.IsNotifyPartySameAsReceiver = false;
                    shipment.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
                    shipment.PackageCalculatonType = FraytePakageCalculationType.kgtoCms;

                    shipment.PaymentPartyTaxAndDuty = "Shipper";
                    shipment.ShipmentStatusId = (int)FrayteTradelaneShipmentStatus.Draft;
                    shipment.FrayteNumber = "TL" + CommonConversion.GetNewFrayteNumber();

                    shipment.CreatedBy = userId;
                    shipment.CreatedOnUtc = DateTime.UtcNow;

                    dbContext.TradelaneShipments.Add(shipment);
                    dbContext.SaveChanges();
                    return shipment.TradelaneShipmentId;
                }
                else
                {
                    return tradelane.TradelaneShipmentId;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public void UpdateExpressShipmentStatus(List<IntegratedTradelanePackage> packages)
        {
            if (packages != null && packages.Count > 0)
            {
                foreach (var item in packages)
                {
                    var shipments = dbContext.Expresses.Where(p => p.BagId == item.BagId).ToList();
                    if (shipments.Count > 0)
                    {
                        foreach (var ship in shipments)
                        {
                            ship.ShipmentStatusId = (int)FrayteExpressShipmentStatus.InTransit;
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
        }

        public TradelaneBooking MapIntegratedShipmentToTradelaneShipment(ExpressTradelaneIntegration integratedShipment, TradelaneBooking mappedShipment)
        {
            var shipment = dbContext.TradelaneShipments.Find(integratedShipment.Shipment.TradelaneShipmentId);

            if (integratedShipment != null && shipment != null)
            {
                mappedShipment.TradelaneShipmentId = integratedShipment.Shipment.TradelaneShipmentId;
                mappedShipment.BatteryDeclarationType = integratedShipment.Shipment.BatteryDeclarationType;
                mappedShipment.CreatedBy = integratedShipment.Shipment.CreatedBy;
                mappedShipment.CustomerId = integratedShipment.Shipment.CustomerId;
                mappedShipment.MAWB = integratedShipment.Shipment.MAWB;
                mappedShipment.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
                mappedShipment.PakageCalculatonType = FraytePakageCalculationType.kgtoCms;
                mappedShipment.PayTaxAndDuties = "Shipper";
                mappedShipment.ShipmentHandlerMethod = integratedShipment.Shipment.ShipmentHandlerMethod;
                mappedShipment.ShipmentStatusId = (int)FrayteTradelaneShipmentStatus.ShipmentBooked;
                mappedShipment.CreatedBy = integratedShipment.Shipment.CreatedBy;
                mappedShipment.HAWBNumber = 1;
                mappedShipment.IsNotifyPartySameAsReceiver = false;
                mappedShipment.MAWB = integratedShipment.Shipment.MAWB;
                mappedShipment.Packages = new List<TradelanePackage>();
                mappedShipment.AirlinePreference = new TradelaneAirline();
                mappedShipment.AirlinePreference.AirlineId = integratedShipment.MAWBList[0].AirlineId;

                //
                var fromHubAddress = new ExpressRepository().getHubAddress(integratedShipment.Shipment.ShipFrom.Country.CountryId, integratedShipment.Shipment.ShipFrom.PostCode, integratedShipment.Shipment.ShipFrom.State);
                if (fromHubAddress != null)
                {
                    mappedShipment.DepartureAirport = new TradelaneAirport();
                    mappedShipment.DepartureAirport.AirportCode = fromHubAddress.HubCode;
                }

                var toHubAddress = new ExpressRepository().getHubAddress(integratedShipment.Shipment.ShipTo.Country.CountryId, integratedShipment.Shipment.ShipFrom.PostCode, integratedShipment.Shipment.ShipFrom.State);

                if (toHubAddress != null)
                {
                    mappedShipment.DestinationAirport = new TradelaneAirport();
                    mappedShipment.DestinationAirport.AirportCode = toHubAddress.HubCode;
                }

                if (integratedShipment.Shipment.Packages != null && integratedShipment.Shipment.Packages.Count > 0)
                {
                    TradelanePackage pkg;
                    foreach (var item in integratedShipment.Shipment.Packages)
                    {
                        pkg = new TradelanePackage();
                        pkg.CartonNumber = item.CartonNumber;
                        pkg.CartonValue = 1;
                        pkg.HAWB = integratedShipment.Shipment.MAWB;
                        pkg.Height = item.Height;
                        pkg.Length = item.Length;
                        pkg.Width = item.Width;
                        pkg.Weight = item.Weight;
                        pkg.HAWB = integratedShipment.Shipment.MAWB;
                        mappedShipment.Packages.Add(pkg);
                    }
                }

                mappedShipment.ShipFrom = new TradelBookingAdress();

                var shipFrom = dbContext.TradelaneShipmentAddresses.Find(shipment.FromAddressId);

                if (shipFrom != null)
                {
                    mappedShipment.ShipFrom.TradelaneShipmentAddressId = shipFrom.TradelaneShipmentAddressId;
                    mappedShipment.ShipFrom.Address = shipFrom.Address1;
                    mappedShipment.ShipFrom.Address2 = shipFrom.Address2;
                    mappedShipment.ShipFrom.Area = shipFrom.Area;
                    mappedShipment.ShipFrom.City = shipFrom.City;
                    mappedShipment.ShipFrom.CompanyName = shipFrom.CompanyName;
                    mappedShipment.ShipFrom.Country = new FrayteCountryCode()
                    {
                        CountryId = shipFrom.CountryId
                    };
                    mappedShipment.ShipFrom.Email = shipFrom.Email;
                    mappedShipment.ShipFrom.FirstName = shipFrom.ContactFirstName;
                    mappedShipment.ShipFrom.LastName = shipFrom.ContactLastName;
                    mappedShipment.ShipFrom.Phone = shipFrom.PhoneNo;
                    mappedShipment.ShipFrom.PostCode = shipFrom.Zip;
                    mappedShipment.ShipFrom.State = shipFrom.State;
                }
                mappedShipment.ShipTo = new TradelBookingAdress();

                var shipTo = dbContext.TradelaneShipmentAddresses.Find(shipment.ToAddressId);
                if (shipTo != null)
                {
                    mappedShipment.ShipTo.TradelaneShipmentAddressId = shipTo.TradelaneShipmentAddressId;
                    mappedShipment.ShipTo.Address = shipTo.Address1;
                    mappedShipment.ShipTo.Address2 = shipTo.Address2;
                    mappedShipment.ShipTo.Area = shipTo.Area;
                    mappedShipment.ShipTo.City = shipTo.City;
                    mappedShipment.ShipTo.CompanyName = shipTo.CompanyName;
                    mappedShipment.ShipTo.Country = new FrayteCountryCode() { CountryId = shipTo.CountryId };
                    mappedShipment.ShipTo.Email = shipTo.Email;
                    mappedShipment.ShipTo.FirstName = shipTo.ContactFirstName;
                    mappedShipment.ShipTo.LastName = shipTo.ContactLastName;
                    mappedShipment.ShipTo.Phone = shipTo.PhoneNo;
                    mappedShipment.ShipTo.PostCode = shipTo.Zip;
                    mappedShipment.ShipTo.State = shipTo.State;
                }

                mappedShipment.NotifyParty = new TradelBookingAdress();

                var notifyParty = dbContext.TradelaneShipmentAddresses.Find(shipment.NotifyPartyAddressId);
                if (notifyParty != null)
                {
                    mappedShipment.NotifyParty.TradelaneShipmentAddressId = notifyParty.TradelaneShipmentAddressId;
                    mappedShipment.NotifyParty.Address = notifyParty.Address1;
                    mappedShipment.NotifyParty.Address2 = notifyParty.Address2;
                    mappedShipment.NotifyParty.Area = notifyParty.Area;
                    mappedShipment.NotifyParty.City = notifyParty.City;
                    mappedShipment.NotifyParty.CompanyName = notifyParty.CompanyName;
                    mappedShipment.NotifyParty.Country = new FrayteCountryCode() { CountryId = shipTo.CountryId };
                    mappedShipment.NotifyParty.Email = notifyParty.Email;
                    mappedShipment.NotifyParty.FirstName = notifyParty.ContactFirstName;
                    mappedShipment.NotifyParty.LastName = notifyParty.ContactLastName;
                    mappedShipment.NotifyParty.Phone = notifyParty.PhoneNo;
                    mappedShipment.NotifyParty.PostCode = notifyParty.Zip;
                    mappedShipment.NotifyParty.State = notifyParty.State;
                }
            }
            else if (integratedShipment != null && shipment == null)
            {
                mappedShipment.TradelaneShipmentId = integratedShipment.Shipment.TradelaneShipmentId;
                mappedShipment.BatteryDeclarationType = integratedShipment.Shipment.BatteryDeclarationType;
                mappedShipment.CreatedBy = integratedShipment.Shipment.CreatedBy;
                mappedShipment.CustomerId = integratedShipment.Shipment.CustomerId;
                mappedShipment.MAWB = integratedShipment.Shipment.MAWB;
                mappedShipment.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
                mappedShipment.PakageCalculatonType = FraytePakageCalculationType.kgtoCms;
                mappedShipment.PayTaxAndDuties = "Shipper";
                mappedShipment.ShipmentHandlerMethod = integratedShipment.Shipment.ShipmentHandlerMethod;
                mappedShipment.ShipmentStatusId = (int)FrayteTradelaneShipmentStatus.ShipmentBooked;
                mappedShipment.CreatedBy = integratedShipment.Shipment.CreatedBy;
                mappedShipment.HAWBNumber = 1;
                mappedShipment.IsNotifyPartySameAsReceiver = false;
                mappedShipment.MAWB = integratedShipment.Shipment.MAWB;
                mappedShipment.Packages = new List<TradelanePackage>();
                mappedShipment.AirlinePreference = new TradelaneAirline();
                mappedShipment.AirlinePreference.AirlineId = integratedShipment.MAWBList[0].AirlineId;

                //
                var fromHubAddress = new ExpressRepository().getHubAddress(integratedShipment.Shipment.ShipFrom.Country.CountryId, integratedShipment.Shipment.ShipFrom.PostCode, integratedShipment.Shipment.ShipFrom.State);
                if (fromHubAddress != null)
                {
                    mappedShipment.DepartureAirport = new TradelaneAirport();
                    mappedShipment.DepartureAirport.AirportCode = fromHubAddress.HubCode;
                }

                var toHubAddress = new ExpressRepository().getHubAddress(integratedShipment.Shipment.ShipTo.Country.CountryId, integratedShipment.Shipment.ShipTo.PostCode, integratedShipment.Shipment.ShipTo.State);

                if (toHubAddress != null)
                {
                    mappedShipment.DestinationAirport = new TradelaneAirport();
                    mappedShipment.DestinationAirport.AirportCode = toHubAddress.HubCode;
                }

                if (integratedShipment.Shipment.Packages != null && integratedShipment.Shipment.Packages.Count > 0)
                {
                    TradelanePackage pkg;
                    foreach (var item in integratedShipment.Shipment.Packages)
                    {
                        pkg = new TradelanePackage();
                        pkg.CartonNumber = item.CartonNumber;
                        pkg.CartonValue = 1;
                        pkg.HAWB = integratedShipment.Shipment.MAWB;
                        pkg.Height = item.Height;
                        pkg.Length = item.Length;
                        pkg.Width = item.Width;
                        pkg.Weight = item.Weight;
                        pkg.HAWB = integratedShipment.Shipment.MAWB;
                        mappedShipment.Packages.Add(pkg);
                    }
                }

                mappedShipment.ShipFrom = new TradelBookingAdress();

                //var shipFrom = dbContext.TradelaneShipmentAddresses.Find(shipment.FromAddressId);

                if (integratedShipment.Shipment.ShipFrom != null)
                {
                    mappedShipment.ShipFrom.TradelaneShipmentAddressId = integratedShipment.Shipment.ShipFrom.TradelaneShipmentAddressId;
                    mappedShipment.ShipFrom.Address = integratedShipment.Shipment.ShipFrom.Address;
                    mappedShipment.ShipFrom.Address2 = integratedShipment.Shipment.ShipFrom.Address2;
                    mappedShipment.ShipFrom.Area = integratedShipment.Shipment.ShipFrom.Area;
                    mappedShipment.ShipFrom.City = integratedShipment.Shipment.ShipFrom.City;
                    mappedShipment.ShipFrom.CompanyName = integratedShipment.Shipment.ShipFrom.CompanyName;
                    mappedShipment.ShipFrom.Country = new FrayteCountryCode()
                    {
                        CountryId = integratedShipment.Shipment.ShipFrom.Country.CountryId
                    };
                    mappedShipment.ShipFrom.Email = integratedShipment.Shipment.ShipFrom.Email;
                    mappedShipment.ShipFrom.FirstName = integratedShipment.Shipment.ShipFrom.FirstName;
                    mappedShipment.ShipFrom.LastName = integratedShipment.Shipment.ShipFrom.LastName;
                    mappedShipment.ShipFrom.Phone = integratedShipment.Shipment.ShipFrom.Phone;
                    mappedShipment.ShipFrom.PostCode = integratedShipment.Shipment.ShipFrom.PostCode;
                    mappedShipment.ShipFrom.State = integratedShipment.Shipment.ShipFrom.State;
                }
                mappedShipment.ShipTo = new TradelBookingAdress();

                //var shipTo = dbContext.TradelaneShipmentAddresses.Find(shipment.ToAddressId);
                if (integratedShipment.Shipment.ShipTo != null)
                {
                    mappedShipment.ShipTo.TradelaneShipmentAddressId = integratedShipment.Shipment.ShipTo.TradelaneShipmentAddressId;
                    mappedShipment.ShipTo.Address = integratedShipment.Shipment.ShipTo.Address;
                    mappedShipment.ShipTo.Address2 = integratedShipment.Shipment.ShipTo.Address2;
                    mappedShipment.ShipTo.Area = integratedShipment.Shipment.ShipTo.Area;
                    mappedShipment.ShipTo.City = integratedShipment.Shipment.ShipTo.City;
                    mappedShipment.ShipTo.CompanyName = integratedShipment.Shipment.ShipTo.CompanyName;
                    mappedShipment.ShipTo.Country = new FrayteCountryCode() { CountryId = integratedShipment.Shipment.ShipTo.Country.CountryId };
                    mappedShipment.ShipTo.Email = integratedShipment.Shipment.ShipTo.Email;
                    mappedShipment.ShipTo.FirstName = integratedShipment.Shipment.ShipTo.FirstName;
                    mappedShipment.ShipTo.LastName = integratedShipment.Shipment.ShipTo.LastName;
                    mappedShipment.ShipTo.Phone = integratedShipment.Shipment.ShipTo.Phone;
                    mappedShipment.ShipTo.PostCode = integratedShipment.Shipment.ShipTo.PostCode;
                    mappedShipment.ShipTo.State = integratedShipment.Shipment.ShipTo.State;
                }

                mappedShipment.NotifyParty = new TradelBookingAdress();

                //var notifyParty = dbContext.TradelaneShipmentAddresses.Find(shipment.NotifyPartyAddressId);
                TradelBookingAdress asddress = new ExpressRepository().GetHubAgentByHubId(integratedShipment.Shipment.Hub.HubId);
                //saveAddress(notifyParty, asddress);
                if (asddress != null)
                {
                    mappedShipment.NotifyParty.TradelaneShipmentAddressId = asddress.TradelaneShipmentAddressId;
                    mappedShipment.NotifyParty.Address = asddress.Address;
                    mappedShipment.NotifyParty.Address2 = asddress.Address2;
                    mappedShipment.NotifyParty.Area = asddress.Area;
                    mappedShipment.NotifyParty.City = asddress.City;
                    mappedShipment.NotifyParty.CompanyName = asddress.CompanyName;
                    mappedShipment.NotifyParty.Country = new FrayteCountryCode() { CountryId = integratedShipment.Shipment.ShipTo.Country.CountryId };
                    mappedShipment.NotifyParty.Email = asddress.Email;
                    mappedShipment.NotifyParty.FirstName = asddress.FirstName;
                    mappedShipment.NotifyParty.LastName = asddress.LastName;
                    mappedShipment.NotifyParty.Phone = asddress.Phone;
                    mappedShipment.NotifyParty.PostCode = asddress.PostCode;
                    mappedShipment.NotifyParty.State = asddress.State;
                }
            }
            return mappedShipment;
        }

        public void UpdateExpressAWbStatus(int tradelaneShipmentId)
        {
            var collection = (from r in dbContext.ExpressManifests
                              join eb in dbContext.ExpressBags on r.ExpressManifestId equals eb.ManifestId
                              join e in dbContext.Expresses on eb.BagId equals e.BagId
                              where r.TradelaneShipmentId == tradelaneShipmentId
                              select e
                              ).ToList();

            if (collection.Count > 0)
            {
                foreach (var item in collection)
                {
                    var ship = dbContext.Expresses.Find(item.ExpressId);
                    if (ship != null)
                    {
                        ship.ShipmentStatusId = (int)FrayteExpressShipmentStatus.HubReceived;
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        public IntegratedTradelaneShipment TradelaneHubInitials(int customerId, int hubId)
        {
            IntegratedTradelaneShipment shipment = new IntegratedTradelaneShipment();
            try
            {
                shipment.ShipFrom = (from U in dbContext.Users
                                     join Add in dbContext.UserAddresses on U.UserId equals Add.UserId
                                     join c in dbContext.Countries on Add.CountryId equals c.CountryId
                                     where U.UserId == customerId
                                     select new TradelBookingAdress
                                     {
                                         CustomerId = customerId,
                                         CompanyName = U.CompanyName,
                                         FirstName = U.ContactName,
                                         LastName = "",
                                         Phone = U.TelephoneNo,
                                         Email = U.UserEmail,
                                         Address = Add.Address,
                                         Address2 = Add.Address2,
                                         Area = Add.Suburb,
                                         City = Add.City,
                                         State = Add.State,
                                         PostCode = Add.Zip,
                                         Country = new FrayteCountryCode()
                                         {
                                             CountryId = c.CountryId,
                                             Code = c.CountryCode,
                                             Code2 = c.CountryCode2,
                                             CountryPhoneCode = c.CountryPhoneCode,
                                             Name = c.CountryName
                                         },
                                         IsDefault = true
                                     }
                              ).FirstOrDefault();

                shipment.ShipTo = (from r in dbContext.Hubs
                                   join c in dbContext.Countries on r.CountryId equals c.CountryId
                                   where r.HubId == hubId
                                   select new TradelBookingAdress
                                   {
                                       CompanyName = "",
                                       FirstName = r.Name,
                                       LastName = "",
                                       Phone = r.TelephoneNo,
                                       Email = r.Email,
                                       Address = r.Address,
                                       Address2 = r.Address2,
                                       Area = r.Area,
                                       City = r.City,
                                       State = r.State,
                                       PostCode = r.PostCode,
                                       Country = new FrayteCountryCode()
                                       {
                                           CountryId = c.CountryId,
                                           Code = c.CountryCode,
                                           Code2 = c.CountryCode2,
                                           CountryPhoneCode = c.CountryPhoneCode,
                                           Name = c.CountryName
                                       },
                                       IsDefault = true
                                   }
                                    ).FirstOrDefault();


                shipment.BatteryDeclarationType = "None";
                shipment.PakageCalculatonType = FraytePakageCalculationType.kgtoCms;
                shipment.Hub = new HubDetailModel();
                shipment.PayTaxAndDuties = "Shipment";
            }
            catch (Exception ex)
            {
            }
            return shipment;
        }
        private void saveShipmentDetail(TradelaneShipment shipment)
        {
            throw new NotImplementedException();
        }

        #region Address

        private void saveAddress(TradelaneShipmentAddress dbAddress, TradelBookingAdress address)
        {
            if (address != null)
            {
                dbAddress.Address1 = address.Address;
                dbAddress.Address2 = address.Address2;
                dbAddress.City = address.City;
                dbAddress.State = address.State;
                dbAddress.Zip = address.PostCode;
                dbAddress.PhoneNo = address.Phone;
                dbAddress.Area = address.State;
                dbAddress.Email = address.Email;
                dbAddress.CountryId = address.Country.CountryId;
                dbAddress.CompanyName = address.CompanyName;
                dbAddress.ContactFirstName = address.FirstName;
                dbAddress.ContactLastName = address.LastName;

                dbContext.TradelaneShipmentAddresses.Add(dbAddress);
                dbContext.SaveChanges();
            }

        }

        #endregion

        #region Vikshit Code
        public MainfestDetailModel GetNonManifestedShipments(int OperationZoneId, int UserId, int CreatedBy, string moduleType, string subModuleType)
        {
            MainfestDetailModel MDM = new MainfestDetailModel();
            MDM.Shipments = new List<FrayteUserDirectShipment>();
            MDM.Customers = new List<FrayteCustomer>();
            var userDetail = (from r in dbContext.Users
                              join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                              where r.UserId == CreatedBy
                              select new
                              {
                                  RoleId = ur.RoleId
                              }).FirstOrDefault();

            var list = dbContext.spGet_GetNonManifestedShipments(OperationZoneId, UserId, CreatedBy, userDetail.RoleId, moduleType, subModuleType).ToList();

            if (list != null && list.Count > 0)
            {
                FrayteUserDirectShipment frayte;
                foreach (var detail in list)
                {
                    frayte = new FrayteUserDirectShipment();
                    frayte.ShipmentId = detail.DirectShipmentId;
                    frayte.ShipmentCode = "";
                    frayte.Customer = dbContext.Users.Where(a => a.UserId == detail.CustomerId).FirstOrDefault().CompanyName != null ? dbContext.Users.Where(a => a.UserId == detail.CustomerId).FirstOrDefault().CompanyName : "";
                    frayte.ShippedFromCompany = detail.FromCompany;
                    frayte.ShippedToCompany = detail.ToCompany;
                    frayte.ShippingBy = detail.LogisticCompany;
                    //frayte.MAWB = 
                    frayte.DisplayName = detail.LogisticCompanyDisplay;
                    frayte.RateType = detail.RateType;
                    frayte.RateTypeDisplay = detail.RateTypeDisplay;
                    frayte.ShippingDate = detail.CreatedOn;
                    frayte.Reference1 = detail.Reference1;
                    frayte.FrayteNumber = detail.FrayteNumber;
                    frayte.Status = detail.StatusName;
                    frayte.TrackingNo = detail.TrackingNo;
                    frayte.TotalPieces = detail.TotalCarton != null ? detail.TotalCarton.Value : 0;
                    frayte.TotalWeight = detail.ChargeableWeight != null ? detail.ChargeableWeight.Value : 0;
                    MDM.Shipments.Add(frayte);
                }

                var CustomerList = list.GroupBy(a => a.CustomerId).ToList();
                foreach (var Cus in CustomerList)
                {
                    //FrayteCustomer FC = new FrayteCustomer();
                    var Cust = (from r in dbContext.Users
                                join ur in dbContext.UserAdditionals on r.UserId equals ur.UserId
                                where r.UserId == Cus.Key
                                select new FrayteCustomer
                                {
                                    ContactName = r.ContactName,
                                    AccountNumber = ur.AccountNo,
                                    CompanyName = r.CompanyName,
                                    UserId = r.UserId

                                }
                            ).FirstOrDefault();
                    MDM.Customers.Add(Cust);
                }

            }
            return MDM;
        }


        public List<ManifestReport> DownLoadManifest(int ManifestId, string moduleType, int UserId, int RoleId)
        {
            List<ManifestReport> _manifest = new List<ManifestReport>();
            try
            {
                var detail = (from uu in dbContext.Users
                              join tz in dbContext.Timezones on uu.TimezoneId equals tz.TimezoneId
                              where uu.UserId == UserId
                              select new
                              {
                                  uu.ContactName,
                                  tz.OffsetShort
                              }).FirstOrDefault();

                //To Do : DownLoad Manifest -> call to dev-express report
                if (moduleType == FrayteShipmentServiceType.DirectBooking || moduleType == FrayteShipmentServiceType.eCommerce)
                {
                    if (detail != null)
                    {
                        var data = dbContext.spGet_DirectBookingManifestDetail(ManifestId, moduleType, RoleId).ToList();

                        if (data != null && data.Count > 0)
                        {
                            ManifestReport report;
                            foreach (var Obj in data)
                            {
                                report = new ManifestReport();
                                report.ManifestFileName = Obj.ManifestName;
                                report.ManifestName = Obj.LogisticCompanyDisplay + " " + Obj.RateTypeDisplay + " - Manifest - " + Obj.ManifestName;
                                report.PrintedBy = detail.ContactName;
                                report.CustomerName = Obj.ContactName;
                                report.CompanyName = Obj.CompanyName;
                                report.ManifestDate = Obj.CreatedOn.ToString("dd-MMM-yyyy HH:mm") + " " + "(" + detail.OffsetShort + ")";
                                report.TimeZone = "(" + detail.OffsetShort + ")";
                                report.AccountNo = Obj.AccountNo;
                                report.Collection = new List<ManifestCollection>()
                                {
                                    new ManifestCollection()
                                    {
                                        Customer = Obj.ContactName,
                                        FromCompany = Obj.FromAddress,
                                        ToCompany = Obj.ToAddress,
                                        DisplayName = Obj.LogisticCompanyDisplay + " " + Obj.RateTypeDisplay,
                                        LogisticCompany = Obj.LogisticCompanyDisplay,
                                        RateTypeDisplay =  Obj.RateTypeDisplay,
                                        LogisticType = Obj.LogisticType,
                                        ShippingDate = Obj.DirectShipmentCreate,
                                        Status = Obj.DisplayStatusName,
                                        Reference = Obj.Reference1,
                                        FrayteNumber = Obj.TrackingNo,
                                        PlateNo = Obj.PlateNo,
                                        TotalPcs = (int)Obj.TotalPcs,
                                        TotalWeight = (float)Obj.TotalWeight
                                    }
                                };
                                _manifest.Add(report);
                            }
                        }
                    }
                }
                else if (moduleType == "sd")
                {
                    var data = dbContext.spGet_ManifestDetail(ManifestId, moduleType).FirstOrDefault();
                    var data1 = dbContext.spGet_DirectBookingManifestDetail(ManifestId, moduleType, RoleId).ToList();
                    _manifest = (from ds in dbContext.eCommerceShipments
                                 join dsd in dbContext.eCommerceShipmentDetails on ds.eCommerceShipmentId equals dsd.eCommerceShipmentId
                                 join us in dbContext.Users on ds.CustomerId equals us.UserId
                                 join dsa in dbContext.eCommerceShipmentAddresses on ds.FromAddressId equals dsa.eCommerceShipmentAddressId
                                 join dsad in dbContext.eCommerceShipmentAddresses on ds.ToAddressId equals dsad.eCommerceShipmentAddressId
                                 join cc in dbContext.Countries on dsa.CountryId equals cc.CountryId
                                 join cc1 in dbContext.Countries on dsad.CountryId equals cc1.CountryId
                                 join cl in dbContext.CountryLogistics on dsad.CountryId equals cl.CountryId
                                 join ss in dbContext.ShipmentStatus on ds.ShipmentStatusId equals ss.ShipmentStatusId
                                 join mf in dbContext.Manifests on ds.ManifestId equals mf.ManifestId
                                 where
                                       mf.ManifestId == ManifestId
                                 select new ManifestReport
                                 {
                                     ManifestName = mf.ManifestName,
                                     ManifestDate = mf.CreatedOn.ToString(),
                                     CustomerName = us.ContactName,
                                     PrintedBy = detail.ContactName + " " + "(" + detail.OffsetShort + ")",
                                     TimeZone = "(" + detail.OffsetShort + ")",
                                     TotalShipments = data.Total_Count.HasValue ? data.Total_Count.Value : 0,
                                     TotalWeights = (float)data.Weight,
                                     Collection = new List<ManifestCollection>()
                                     {
                                            new ManifestCollection()
                                            {
                                                  Customer = us.ContactName,
                                                  FromCompany = dsa.Address1 + " " + dsa.Area + " " + dsa.City + " " + dsa.State + " " + cc.CountryName,
                                                  ToCompany = dsad.Address1 + " " + dsad.Area + " " + dsad.City + " " + dsad.State + " " + cc1.CountryName,
                                                  DisplayName = cl.LogisicServiceDisplay,
                                                  ShippingDate = ds.CreatedOn,
                                                  Status = ss.DisplayStatusName,
                                                  Reference = ds.Reference1,
                                                  FrayteNumber = ds.FrayteNumber,
                                                  TotalPcs = dsd.CartoonValue,
                                                  TotalWeight = (float) (dsd.CartoonValue * dsd.Weight)
                                            }
                                     }
                                 }).ToList();
                }
                return _manifest;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        public ExpressViewManifest GetManifestDetail(int manifestId)
        {
            var list = dbContext.spGet_GetExpressManifestedShipments(manifestId).ToList();
            ExpressViewManifest EVM = new ExpressViewManifest();

            EVM.ManifestedList = new List<ExpressManifestDetail>();
            if (list != null && list.Count > 0)
            {

                var Bags = list.GroupBy(a => a.BagBarCode).ToList();
                foreach (var b in Bags)
                {
                    ExpressManifestDetail frayte = new ExpressManifestDetail();
                    EVM.ManifestName = Bags.FirstOrDefault().FirstOrDefault().BarCode;
                    frayte.BagId = b.FirstOrDefault().BagId.Value;
                    frayte.BagNumber = b.FirstOrDefault().BagBarCode.Replace("BGL-", "");
                    frayte.Carrier = b.FirstOrDefault().Carrier;
                    frayte.Customer = b.FirstOrDefault().ContactName;
                    frayte.TotalShipments = b.FirstOrDefault().TotalNoOfShipments.Value;
                    frayte.TotalWeight = b.FirstOrDefault().TotalWeight;
                    EVM.ManifestedList.Add(frayte);
                }
            }
            return EVM;
        }

        public ExpressViewManifest GetBagsDetail(int HubId, int CustomerId)
        {
            var list = dbContext.spGet_GetExpressBags(HubId, CustomerId).ToList();
            ExpressViewManifest EVM = new ExpressViewManifest();
            EVM.ManifestedList = new List<ExpressManifestDetail>();
            try
            {

                if (list != null && list.Count > 0)
                {
                    var Bags = list.GroupBy(a => a.BagBarCode).ToList();
                    foreach (var b in Bags)
                    {
                        ExpressManifestDetail frayte = new ExpressManifestDetail();
                        var CusId = b.FirstOrDefault().CustomerId;
                        var res = dbContext.Users.Where(a => a.UserId == CusId).FirstOrDefault();
                        if (res != null)
                        {
                            var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == res.TimezoneId).FirstOrDefault();
                            var TZ = new TimeZoneModal();
                            if (timeZone != null)
                            {

                                TZ.TimezoneId = timeZone.TimezoneId;
                                TZ.Name = timeZone.Name;
                                TZ.Offset = timeZone.Offset;
                                TZ.OffsetShort = timeZone.OffsetShort;
                            }
                            var TimeZone = TimeZoneInfo.FindSystemTimeZoneById(TZ.Name);

                            var tm = UtilityRepository.UtcDateToOtherTimezone(b.FirstOrDefault().CreatedOnUtc.Value.Date, b.FirstOrDefault().CreatedOnUtc.Value.TimeOfDay, TimeZone);
                            frayte.CreatedOn = tm.Item1;
                            frayte.CreatedOnTime = tm.Item2.Substring(0, 2) + ":" + tm.Item2.Substring(2, 2);
                        }
                        EVM.ManifestName = b.Key;
                        frayte.BagId = b.FirstOrDefault().BagId.HasValue ? b.FirstOrDefault().BagId.Value : 0;
                        frayte.BagNumber = b.FirstOrDefault().BagBarCode.Replace("BGL-", "");
                        frayte.CreatedOn = b.FirstOrDefault().CreatedOnUtc;
                        frayte.Carrier = b.FirstOrDefault().Carrier;
                        frayte.Customer = b.FirstOrDefault().ContactName;
                        frayte.TotalShipments = b.FirstOrDefault().TotalNoOfShipments.Value;
                        frayte.TotalWeight = b.FirstOrDefault().TotalWeight.HasValue ? b.FirstOrDefault().TotalWeight.Value : 0.0M;
                        EVM.ManifestedList.Add(frayte);
                    }
                }
            }
            catch (Exception ex)
            {
                EVM.ManifestedList = new List<ExpressManifestDetail>();
            }

            return EVM;
        }

        public TradelaneFile GetBagLabel(int BagId)
        {
            TradelaneFile FN = new TradelaneFile();
            var BagDetail = dbContext.ExpressBags.Where(a => a.BagId == BagId).FirstOrDefault();
            if (BagDetail != null)
            {
                var GetFile = System.IO.Directory.GetFiles(AppSettings.UploadFolderPath + "/ExpressBag/" + BagDetail.BagId);
                foreach (var fl in GetFile)
                {
                    var flName = fl.Split('\\');
                    if (fl.Contains("EXS-BGL"))
                    {
                        FN.TradelaneShipmentId = BagDetail.BagId;
                        FN.FileName = flName[flName.Length - 1];
                        FN.FilePath = AppSettings.WebApiPath + "/UploadFiles/ExpressBag/" + BagDetail.BagId + '/' + FN.FileName;
                    }
                }
            }
            return FN;
        }

        public List<HubDetailModel> GetHubs()
        {
            var list = dbContext.Hubs.ToList();
            List<HubDetailModel> HubList = new List<HubDetailModel>();


            if (list != null && list.Count > 0)
            {
                foreach (var b in list)
                {
                    HubDetailModel hub = new HubDetailModel();
                    hub.Code = b.Code;
                    hub.HubId = b.HubId;
                    hub.Name = b.Name;
                    hub.DefaultCurrency = b.DefaultCurrency;
                    HubList.Add(hub);
                }
            }
            return HubList;
        }

        public List<ExpressManifestModel> GetManifests(ExpressTrackManifest trackManifest)
        {
            var userDetail = (from r in dbContext.Users
                              join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                              where r.UserId == trackManifest.CreatedBy
                              select new
                              {
                                  RoleId = ur.RoleId
                              }).FirstOrDefault();
            DateTime? fromdate;
            DateTime? todate;
            if (trackManifest.FromDate.HasValue)
            {
                fromdate = trackManifest.FromDate.Value;
            }
            else
            {
                fromdate = trackManifest.FromDate;
            }

            if (trackManifest.ToDate.HasValue)
            {
                todate = trackManifest.ToDate.Value;
            }
            else
            {
                todate = trackManifest.ToDate;
            }
            if (userDetail != null && (userDetail.RoleId == 1 || userDetail.RoleId == 20))
            {

                trackManifest.CreatedBy = 0;
            }
            if (trackManifest.UserId > 0)
            {
                trackManifest.CreatedBy = 0;
            }

            List<ExpressManifestModel> manifests = new List<ExpressManifestModel>();
            //if (userDetail != null)
            //{
            int SkipRows = 0;
            SkipRows = (trackManifest.CurrentPage - 1) * trackManifest.TakeRows;
            var list = dbContext.spGet_ExpressTrackManifest(trackManifest.FromDate, trackManifest.ToDate, SkipRows, trackManifest.TakeRows, trackManifest.UserId, trackManifest.CreatedBy, trackManifest.ManifestName).ToList();

            ExpressManifestModel manifestData;
            if (list != null && list.Count > 0)
            {
                foreach (var data in list)
                {
                    manifestData = new ExpressManifestModel();
                    var res = dbContext.Users.Where(a => a.UserId == data.CustomerId).FirstOrDefault();
                    var TradelaneShipment = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == data.TradelaneShipmentId).FirstOrDefault();
                    if (res != null)
                    {
                        var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == res.TimezoneId).FirstOrDefault();
                        var TZ = new TimeZoneModal();
                        if (timeZone != null)
                        {

                            TZ.TimezoneId = timeZone.TimezoneId;
                            TZ.Name = timeZone.Name;
                            TZ.Offset = timeZone.Offset;
                            TZ.OffsetShort = timeZone.OffsetShort;
                        }
                        var TimeZone = TimeZoneInfo.FindSystemTimeZoneById(TZ.Name);

                        var tm = UtilityRepository.UtcDateToOtherTimezone(data.CreatedOn.Value.Date, data.CreatedOn.Value.TimeOfDay, TimeZone);
                        manifestData.CreatedOn = tm.Item1;
                        manifestData.CreatedOnTime = tm.Item2.Substring(0, 2) + ":" + tm.Item2.Substring(2, 2);
                    }

                    manifestData.CustomerId = data.CustomerId.Value;
                    manifestData.TradelaneShipmentId = data.TradelaneShipmentId ?? 0;
                    manifestData.TradelaneshipmentStatusId = TradelaneShipment.ShipmentStatusId;
                    manifestData.CreatedOn = data.CreatedOn ?? null;
                    manifestData.MAWB = data.MAWB;
                    manifestData.NoOfShipments = data.TotalNoOfShipments.HasValue ? data.TotalNoOfShipments.Value : 0;
                    manifestData.NoOfBags = data.TotalNoOfBags.HasValue ? data.TotalNoOfBags.Value : 0;
                    manifestData.TotalWeight = (float)data.TotalWeight;
                    manifestData.ManifestId = data.ExpressManifestId != null ? data.ExpressManifestId.Value : 0;
                    manifestData.ManifestName = data.BarCode.Replace("MNESX-", "");
                    manifestData.TotalRows = data.TotalRows.Value;
                    manifestData.Customer = dbContext.Users.Where(a => a.UserId == data.CustomerId).FirstOrDefault().CompanyName != null ? dbContext.Users.Where(a => a.UserId == data.CustomerId).FirstOrDefault().CompanyName : "";
                    manifests.Add(manifestData);
                }
            }
            return manifests;
        }

        public void SaveExportManifest(int TradelaneShipmentId, IntegratedTradelaneShipment Shipment)
        {
            try
            {
                ExpressManifest EM = new ExpressManifest();
                EM.TradelaneShipmentId = TradelaneShipmentId;
                EM.CustomerId = Shipment.CustomerId;
                EM.BarCode = "MNESX-" + Shipment.Hub.Code + "-" + new Random().Next(10000000, 99999999);
                EM.CreadtdBy = Shipment.CreatedBy;
                EM.CreatedOn = DateTime.UtcNow;
                EM.UpdatedBy = Shipment.CreatedBy;
                EM.UpdatedOn = DateTime.UtcNow;
                EM.HubId = Shipment.Hub.HubId;
                dbContext.ExpressManifests.Add(EM);
                dbContext.SaveChanges();

                if (EM.ExpressManifestId > 0 && Shipment.Packages.Count > 0)
                {
                    foreach (var Bag in Shipment.Packages)
                    {
                        var BagInfo = dbContext.ExpressBags.Where(a => a.BagId == Bag.BagId).FirstOrDefault();
                        if (BagInfo != null)
                        {
                            BagInfo.ManifestId = EM.ExpressManifestId;
                            dbContext.Entry(BagInfo).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                }

                if (EM != null && EM.BarCode != null)
                {
                    var result = dbContext.TrackingNumberRoutes.Where(a => a.Number == EM.BarCode).FirstOrDefault();
                    if (result == null)
                    {
                        TrackingNumberRoute TNR = new TrackingNumberRoute();
                        TNR.Number = EM.BarCode;
                        TNR.ShipmentId = TradelaneShipmentId;
                        TNR.ModuleType = FrayteShipmentServiceType.TradeLaneBooking;
                        TNR.IsExpressManifestNumber = true;
                        dbContext.TrackingNumberRoutes.Add(TNR);
                        dbContext.SaveChanges();
                    }
                }

                //save driver manifest
                SaveDriverManifest(EM.ExpressManifestId);
            }
            catch (Exception ex)
            {

            }
        }

        public void SaveDriverManifest(int ExpressManifestId)
        {
            try
            {
                var Result = dbContext.ExpressBags.Where(a => a.ManifestId == ExpressManifestId).ToList();
                if (Result != null && Result.Count > 0)
                {
                    var Rs = Result.GroupBy(a => a.Courier).ToList();
                    foreach (var Res in Rs)
                    {
                        string hubcode = string.Empty;
                        var HubCarrierid = Res.FirstOrDefault().HubCarrierId;
                        var hubcar = dbContext.HubCarriers.Where(a => a.HubCarrierId == HubCarrierid).FirstOrDefault();
                        if (hubcar != null)
                        {
                            hubcode = dbContext.Hubs.Where(a => a.HubId == hubcar.HubId).FirstOrDefault().Code;
                        }
                        ExpressDriverManifest EDM = new ExpressDriverManifest();
                        EDM.DriverManifestBarCode = "MNDR-" + hubcode + "-" + new Random().Next(10000000, 99999999);
                        EDM.ExpressManifestId = ExpressManifestId;
                        dbContext.ExpressDriverManifests.Add(EDM);
                        dbContext.SaveChanges();

                        foreach (var r in Res)
                        {
                            var bagres = dbContext.ExpressBags.Where(a => a.BagId == r.BagId).FirstOrDefault();
                            if (bagres != null)
                            {
                                bagres.DriverManifestId = EDM.ExpressDriverManifestId;
                                dbContext.Entry(bagres).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public List<ExpressGetShipmentModel> GetShipments(int BagId)
        {
            List<ExpressGetShipmentModel> Shipments = new List<ExpressGetShipmentModel>();
            //join EXD in dbContext.ExpressDetails on EX.ExpressId equals EXD.ExpressId
            Shipments = (from EX in dbContext.Expresses
                         join Usr in dbContext.Users on EX.CustomerId equals Usr.UserId
                         join HCS in dbContext.HubCarrierServices on EX.HubCarrierServiceId equals HCS.HubCarrierServiceId
                         join HC in dbContext.HubCarriers on HCS.HubCarrierId equals HC.HubCarrierId
                         join Hu in dbContext.Hubs on HC.HubId equals Hu.HubId
                         join EXFA in dbContext.ExpressAddresses on EX.FromAddressId equals EXFA.ExpressAddressId
                         join EXTA in dbContext.ExpressAddresses on EX.ToAddressId equals EXTA.ExpressAddressId
                         where EX.BagId == BagId
                         select new ExpressGetShipmentModel()
                         {
                             ExpressShipmentId = EX.ExpressId,
                             AWBNumber = EX.AWBBarcode.Substring(0, 3) + " " + EX.AWBBarcode.Substring(3, 3) + " " + EX.AWBBarcode.Substring(6, 3) + " " + EX.AWBBarcode.Substring(9, 3),
                             TrackingNumber = EX.TrackingNumber,
                             CreatedOn = EX.CreatedOnUtc,
                             HubCode = Hu.Code,
                             Shipper = EXFA.CompanyName,
                             Receiver = EXTA.ContactFirstName + " " + EXTA.ContactLastName,
                             Customer = Usr.ContactName,
                             CustomerId = Usr.UserId,
                             Carrier = HC.Carrier,
                             TotalCarton = dbContext.ExpressDetails.Where(a => a.ExpressId == EX.ExpressId).Sum(a => a.CartonQty),
                             TotalWeight = dbContext.ExpressDetails.Where(a => a.ExpressId == EX.ExpressId).Sum(ab => ab.Weight * ab.CartonQty)
                         }).ToList();

            if (Shipments.Count > 0)
            {
                foreach (var r in Shipments)
                {
                    var res = dbContext.Users.Where(a => a.UserId == r.CustomerId).FirstOrDefault();
                    if (res != null)
                    {
                        var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == res.TimezoneId).FirstOrDefault();
                        var TZ = new TimeZoneModal();
                        if (timeZone != null)
                        {

                            TZ.TimezoneId = timeZone.TimezoneId;
                            TZ.Name = timeZone.Name;
                            TZ.Offset = timeZone.Offset;
                            TZ.OffsetShort = timeZone.OffsetShort;
                        }
                        var TimeZone = TimeZoneInfo.FindSystemTimeZoneById(TZ.Name);

                        var tm = UtilityRepository.UtcDateToOtherTimezone(r.CreatedOn.Date, r.CreatedOn.TimeOfDay, TimeZone);
                        r.CreatedOn = tm.Item1;
                        r.CreatedOnTime = tm.Item2.Substring(0, 2) + ":" + tm.Item2.Substring(2, 2);
                    }
                }
            }

            return Shipments;
        }

        public ExpressViewManifest GetManifestShipments(int ManifestId)
        {
            ExpressViewManifest Shipments = new ExpressViewManifest();
            Shipments.ManifestedShipmentList = new List<ExpressGetShipmentModel>();
            Shipments.ManifestName = dbContext.ExpressManifests.Where(a => a.ExpressManifestId == ManifestId).FirstOrDefault().BarCode;
            Shipments.ManifestedShipmentList = (from Bg in dbContext.ExpressBags
                                                join EX in dbContext.Expresses on Bg.BagId equals EX.BagId
                                                join Usr in dbContext.Users on EX.CustomerId equals Usr.UserId
                                                join HCS in dbContext.HubCarrierServices on EX.HubCarrierServiceId equals HCS.HubCarrierServiceId
                                                join HC in dbContext.HubCarriers on HCS.HubCarrierId equals HC.HubCarrierId
                                                join Hu in dbContext.Hubs on HC.HubId equals Hu.HubId
                                                join EXFA in dbContext.ExpressAddresses on EX.FromAddressId equals EXFA.ExpressAddressId
                                                join EXTA in dbContext.ExpressAddresses on EX.ToAddressId equals EXTA.ExpressAddressId
                                                where Bg.ManifestId == ManifestId
                                                select new ExpressGetShipmentModel()
                                                {
                                                    ExpressShipmentId = EX.ExpressId,
                                                    AWBNumber = EX.AWBBarcode.Substring(0, 3) + " " + EX.AWBBarcode.Substring(3, 3) + " " + EX.AWBBarcode.Substring(6, 3) + " " + EX.AWBBarcode.Substring(9, 3),
                                                    TrackingNumber = EX.TrackingNumber,
                                                    BagBarcodeNo = Bg.BagBarCode.Replace("BGL-", ""),
                                                    CreatedOn = EX.CreatedOnUtc,
                                                    HubCode = Hu.Code,
                                                    BagId = Bg.BagId,
                                                    Shipper = EXFA.CompanyName,
                                                    Receiver = EXTA.ContactFirstName + " " + EXTA.ContactLastName,
                                                    Customer = Usr.ContactName,
                                                    CustomerId = Usr.UserId,
                                                    Carrier = HC.Carrier,
                                                    TotalCarton = dbContext.ExpressDetails.Where(a => a.ExpressId == EX.ExpressId).Sum(a => a.CartonQty),
                                                    TotalWeight = dbContext.ExpressDetails.Where(a => a.ExpressId == EX.ExpressId).Sum(ab => ab.Weight * ab.CartonQty)
                                                }).ToList();

            if (Shipments.ManifestedShipmentList.Count > 0)
            {
                foreach (var r in Shipments.ManifestedShipmentList)
                {
                    var res = dbContext.Users.Where(a => a.UserId == r.CustomerId).FirstOrDefault();
                    if (res != null)
                    {
                        var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == res.TimezoneId).FirstOrDefault();
                        var TZ = new TimeZoneModal();
                        if (timeZone != null)
                        {

                            TZ.TimezoneId = timeZone.TimezoneId;
                            TZ.Name = timeZone.Name;
                            TZ.Offset = timeZone.Offset;
                            TZ.OffsetShort = timeZone.OffsetShort;
                        }
                        var TimeZone = TimeZoneInfo.FindSystemTimeZoneById(TZ.Name);

                        var tm = UtilityRepository.UtcDateToOtherTimezone(r.CreatedOn.Date, r.CreatedOn.TimeOfDay, TimeZone);
                        r.CreatedOn = tm.Item1;
                        r.CreatedOnTime = tm.Item2.Substring(0, 2) + ":" + tm.Item2.Substring(2, 2);
                    }
                }
            }

            return Shipments;
        }

        public void SendExportManifest(ExpressDownloadPDFModel TradelaneShipment, string FilePath)
        {
            FrayteResult result = new FrayteResult();
            ExpressEmailModel emailObj = new ExpressEmailModel();
            emailObj.ManifestDetail = GetExportManifestData(TradelaneShipment.TradelaneShipmentId);
            emailObj.ManifestDetail.CustomerId = TradelaneShipment.CustomerId;
            result = new ExpressEmailRepository().SendExportManifest(emailObj, FilePath, TradelaneShipment.UserId);
        }

        public ExpressManifestModel GetExportManifestData(int TradelaneShipmentId)
        {
            var ExpressManifestModel = (from r in dbContext.ExpressManifests
                                        join TL in dbContext.TradelaneShipments on r.TradelaneShipmentId equals TL.TradelaneShipmentId
                                        join AL in dbContext.Airlines on TL.AirlineId equals AL.AirlineId
                                        join b in dbContext.ExpressBags on r.ExpressManifestId equals b.ManifestId
                                        where r.TradelaneShipmentId == TradelaneShipmentId
                                        select new ExpressManifestModel()
                                        {
                                            ManifestName = r.BarCode,
                                            MAWB = AL.AirlineCode + " " + TL.MAWB.Substring(0, 4) + " " + TL.MAWB.Substring(4, 4),
                                            NoOfBags = dbContext.ExpressBags.Where(a => a.ManifestId == r.ExpressManifestId).Count(),
                                            CreatedOn = r.CreatedOn,
                                            CustomerId = r.CustomerId.HasValue ? r.CustomerId.Value : 0
                                        }).FirstOrDefault();

            return ExpressManifestModel;

        }

        public void SendDriverManifest(ExpressDownloadPDFModel TradelaneShipment, string FilePath)
        {
            FrayteResult result = new FrayteResult();
            ExpressEmailModel emailObj = new ExpressEmailModel();
            TradelaneFile TL = new TradelaneFile();
            TL.FilePath = FilePath;
            emailObj.DriverManifestDetail = GetDriverManifestData(TradelaneShipment, TL);

            result = new ExpressEmailRepository().SendDriverManifest(emailObj, FilePath, TradelaneShipment);
        }

        public ExpressReportDriverManifest GetDriverManifestData(ExpressDownloadPDFModel TradelaneShipment, TradelaneFile File)
        {
            var ExpressManifestModel = new ExpressReportRepository().GetDriverManifestReportObj(TradelaneShipment.TradelaneShipmentId, TradelaneShipment.CustomerId);
            return ExpressManifestModel.FirstOrDefault();
        }

        public string GetTimeZoneName(int BagId)
        {
            string timezone = (from es in dbContext.Expresses
                               join ea in dbContext.ExpressAddresses on es.FromAddressId equals ea.ExpressAddressId
                               join cc in dbContext.Countries on ea.CountryId equals cc.CountryId
                               join tz in dbContext.Timezones on cc.TimeZoneId equals tz.TimezoneId
                               where es.BagId == BagId
                               select tz.Name).FirstOrDefault();

            return timezone;
        }

        #endregion
    }
}
