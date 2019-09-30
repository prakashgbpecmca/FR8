using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.Express;
using Frayte.Services.Models.Aftership;
using Frayte.Services.Utility;
using AftershipAPI;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Net;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;

namespace Frayte.Services.Business
{
    public class DirectShipmentRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public DirectBookingCollection GetCustomerDetail(int customerId)
        {
            var result = (from U in dbContext.Users
                          join UA in dbContext.UserAddresses on U.UserId equals UA.UserId
                          join Add in dbContext.UserAdditionals on UA.UserId equals Add.UserId
                          where U.UserId == customerId
                          select new DirectBookingCollection()
                          {
                              CustomerId = customerId,
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
                              CurrencyCode = Add.CreditLimitCurrencyCode,
                              Country = new FrayteCountryCode() { CountryId = UA.CountryId },
                              IsShipperTaxAndDuty = Add.IsShipperTaxAndDuty.HasValue ? Add.IsShipperTaxAndDuty.Value : false,
                              IsRateShow = Add.IsAllowRate.Value == true ? true : false
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

        public void UpdateShipmentDetail(int delivery_time, object delivery_date, string signed_by, string frayteRef, string moduleType)
        {
            if (!string.IsNullOrEmpty(moduleType) && moduleType == FrayteShipmentServiceType.Express && delivery_time != 0 && delivery_date != null && !string.IsNullOrEmpty(frayteRef))
            {
                var shipment = dbContext.Expresses.Where(e => e.FrayteNumber == frayteRef).FirstOrDefault();
                if (shipment != null)
                {
                    shipment.DeliveryDate = Convert.ToDateTime(delivery_date);
                    shipment.DeliveryTime = TimeSpan.Parse(Convert.ToDateTime(delivery_date).ToString());
                    shipment.SignedBy = signed_by;
                    dbContext.SaveChanges();
                }
            }
        }

        public void UpdateShipmentStatus(string moduleType, string tag, string frayteRef)
        {
            if (!string.IsNullOrEmpty(moduleType) && moduleType == FrayteShipmentServiceType.Express && !string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(frayteRef))
            {
                var shipment = dbContext.Expresses.Where(p => p.FrayteNumber == frayteRef).FirstOrDefault();
                if (shipment != null)
                {
                    int StatusId = 0;
                    if (tag == FrayteAftershipStatusTagString.Delivered)
                    {
                        StatusId = 44;
                    }

                    shipment.ShipmentStatusId = StatusId;
                    dbContext.SaveChanges();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(frayteRef))
                {
                    var shipment = dbContext.DirectShipments.Where(p => p.FrayteNumber == frayteRef).FirstOrDefault();
                    if (shipment != null)
                    {
                        int StatusId = 0;
                        if (tag == FrayteAftershipStatusTagString.Delivered)
                        {
                            StatusId = 26;
                        }
                        else if (tag == FrayteAftershipStatusTagString.OutForDelivery)
                        {
                            StatusId = 16;
                        }
                        else if (tag == FrayteAftershipStatusTagString.InTransit)
                        {
                            StatusId = 15;
                        }
                        else if (tag == FrayteAftershipStatusTagString.InfoReceived)
                        {
                            StatusId = 13;
                        }
                        else if (tag == FrayteAftershipStatusTagString.Exception)
                        {
                            StatusId = 24;
                        }
                        else if (tag == FrayteAftershipStatusTagString.AttemptFail)
                        {
                            StatusId = 25;
                        }
                        shipment.ShipmentStatusId = StatusId;
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        public FrayteUserDefaultAddresses UserDefaultAddress(int customerId)
        {
            FrayteUserDefaultAddresses address = new FrayteUserDefaultAddresses();
            address.ShipFrom = (from U in dbContext.AddressBooks
                                join uca in dbContext.UserCountryAddresses on U.AddressBookId equals uca.FromAddressId
                                join Add in dbContext.UserAdditionals on U.CustomerId equals Add.UserId
                                where U.CustomerId == customerId
                                && uca.CustomerId == customerId
                                && U.IsActive == true && U.FromAddress == true && U.IsDefault == true
                                select new DirectBookingCollection
                                {
                                    CustomerId = customerId,
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
                                    CurrencyCode = Add.CreditLimitCurrencyCode,
                                    Country = new FrayteCountryCode() { CountryId = U.CountryId },
                                    IsShipperTaxAndDuty = Add.IsShipperTaxAndDuty.HasValue ? Add.IsShipperTaxAndDuty.Value : false,
                                    IsRateShow = Add.IsAllowRate.Value == true ? true : false,
                                    IsDefaultAddress = true
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

            address.ShipTo = (from U in dbContext.AddressBooks
                              join Add in dbContext.UserAdditionals on U.CustomerId equals Add.UserId
                              where U.CustomerId == customerId && U.IsActive == true && U.ToAddress == true && U.IsDefault == true
                              select new DirectBookingCollection
                              {
                                  CustomerId = customerId,
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
                                  CurrencyCode = Add.CreditLimitCurrencyCode,
                                  Country = new FrayteCountryCode() { CountryId = U.CountryId },
                                  IsShipperTaxAndDuty = Add.IsShipperTaxAndDuty.HasValue ? Add.IsShipperTaxAndDuty.Value : false,
                                  IsRateShow = Add.IsAllowRate.Value == true ? true : false,
                                  IsDefaultAddress = true
                              }).FirstOrDefault();

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
                                    where U.UserId == customerId
                                    select new DirectBookingCollection()
                                    {
                                        CustomerId = customerId,
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
                                        CurrencyCode = Add.CreditLimitCurrencyCode,
                                        Country = new FrayteCountryCode() { CountryId = UA.CountryId },
                                        IsShipperTaxAndDuty = Add.IsShipperTaxAndDuty.HasValue ? Add.IsShipperTaxAndDuty.Value : false,
                                        IsRateShow = Add.IsAllowRate.Value == true ? true : false,
                                        IsDefaultAddress = false
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

        public FrayteUserDefaultAddresses getUserDefaultAddress(int userId, int countryId, string addressType)
        {
            FrayteUserDefaultAddresses defaultAddress = new FrayteUserDefaultAddresses();

            if (addressType == "ShipFrom")
            {
                defaultAddress.ShipFrom = (from r in dbContext.UserCountryAddresses
                                           join ab in dbContext.AddressBooks on r.FromAddressId equals ab.AddressBookId
                                           join c in dbContext.Countries on ab.CountryId equals c.CountryId
                                           where r.CountryId == countryId && r.CustomerId == userId
                                           select new DirectBookingCollection
                                           {
                                               FirstName = ab.ContactFirstName,
                                               LastName = ab.ContactLastName,
                                               CompanyName = ab.CompanyName,
                                               Address = ab.Address1,
                                               Address2 = ab.Address2,
                                               Area = ab.Area,
                                               AddressType = "FromAddress",
                                               Email = ab.Email,
                                               City = ab.City,
                                               CustomerId = ab.CustomerId,
                                               State = ab.State,
                                               PostCode = ab.Zip,
                                               Phone = ab.PhoneNo,
                                               Country = new FrayteCountryCode
                                               {
                                                   Name = c.CountryName,
                                                   Code = c.CountryCode,
                                                   Code2 = c.CountryCode2,
                                                   CountryId = c.CountryId,
                                                   CountryPhoneCode = c.CountryPhoneCode,
                                               }
                                           }).FirstOrDefault();
            }

            if (addressType == "ShipTo")
            {
                defaultAddress.ShipTo = (from r in dbContext.UserCountryAddresses
                                         join ab in dbContext.AddressBooks on r.ToAddressId equals ab.AddressBookId
                                         join c in dbContext.Countries on ab.CountryId equals c.CountryId
                                         where r.CountryId == countryId && r.CustomerId == userId
                                         select new DirectBookingCollection
                                         {
                                             FirstName = ab.ContactFirstName,
                                             LastName = ab.ContactLastName,
                                             CompanyName = ab.CompanyName,
                                             Address = ab.Address1,
                                             Address2 = ab.Address2,
                                             Area = ab.Area,
                                             AddressType = "FromAddress",
                                             Email = ab.Email,
                                             City = ab.City,
                                             CustomerId = ab.CustomerId,
                                             State = ab.State,
                                             PostCode = ab.Zip,
                                             Phone = ab.PhoneNo,
                                             Country = new FrayteCountryCode
                                             {
                                                 Name = c.CountryName,
                                                 Code = c.CountryCode,
                                                 Code2 = c.CountryCode2,
                                                 CountryId = c.CountryId,
                                                 CountryPhoneCode = c.CountryPhoneCode,
                                             }
                                         }).FirstOrDefault();

            }
            return defaultAddress;
        }

        public FrayteResult SetCustomerDefaultAddress(int addressBookId, int countryId, int userId, string addressType)
        {
            FrayteResult result = new FrayteResult();
            if (addressType == "FromAddress")
            {
                UserCountryAddress address;
                address = dbContext.UserCountryAddresses.Where(p => p.CountryId == countryId && p.CustomerId == userId && p.FromAddressId != null).FirstOrDefault();

                if (address != null)
                {
                    address.FromAddressId = addressBookId;
                    dbContext.SaveChanges();
                }
                else
                {
                    address = new UserCountryAddress();
                    address.CountryId = countryId;
                    address.CustomerId = userId;
                    address.FromAddressId = addressBookId;
                    dbContext.UserCountryAddresses.Add(address);
                    dbContext.SaveChanges();
                }

                result.Status = true;
                //var addressBook = dbContext.AddressBooks.Where(p => p.IsActive == true && p.CustomerId == userId && p.IsDefault == true && p.FromAddress == true).FirstOrDefault();
                //if (addressBook != null)
                //{
                //    addressBook.IsDefault = false;
                //    dbContext.SaveChanges();
                //}

                //var address = dbContext.AddressBooks.Find(addressBookId);
                //if (address != null)
                //{
                //    address.IsDefault = true;
                //    result.Status = true;
                //    dbContext.SaveChanges();
                //}
            }
            if (addressType == "ToAddress")
            {
                UserCountryAddress address;
                address = dbContext.UserCountryAddresses.Where(p => p.CountryId == countryId && p.CustomerId == userId && p.ToAddressId != null).FirstOrDefault();

                if (address != null)
                {
                    address.ToAddressId = addressBookId;
                    dbContext.SaveChanges();
                }
                else
                {
                    address = new UserCountryAddress();
                    address.CountryId = countryId;
                    address.CustomerId = userId;
                    address.ToAddressId = addressBookId;
                    dbContext.UserCountryAddresses.Add(address);
                    dbContext.SaveChanges();
                }

                var addressBook = dbContext.AddressBooks.Where(p => p.IsActive == true && p.CustomerId == userId && p.IsDefault == true && p.FromAddress == true).FirstOrDefault();
                if (addressBook != null)
                {
                    addressBook.IsDefault = false;
                    dbContext.SaveChanges();
                }
                result.Status = true;

                //var addressBook = dbContext.AddressBooks.Where(p => p.IsActive == true && p.CustomerId == userId && p.IsDefault == true && p.ToAddress == true).FirstOrDefault();
                //if (addressBook != null)
                //{
                //    addressBook.IsDefault = false;
                //    dbContext.SaveChanges();
                //}
                //var address = dbContext.AddressBooks.Find(addressBookId);
                //if (address != null)
                //{
                //    address.IsDefault = true;
                //    result.Status = true;
                //    dbContext.SaveChanges();
                //}
            }
            return result;
        }

        public FrayteResult SetCustomerDefaultAddress(int addressBookId, int countryId, int userId, bool value, string addressType)
        {
            FrayteResult result = new FrayteResult();
            if (addressType == "FromAddress")
            {
                UserCountryAddress address;
                address = dbContext.UserCountryAddresses.Where(p => p.CountryId == countryId && p.CustomerId == userId && p.FromAddressId != null).FirstOrDefault();
                if (value)
                {
                    if (address != null)
                    {
                        address.FromAddressId = addressBookId;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        address = new UserCountryAddress();
                        address.CountryId = countryId;
                        address.CustomerId = userId;
                        address.FromAddressId = addressBookId;
                        dbContext.UserCountryAddresses.Add(address);
                        dbContext.SaveChanges();
                    }
                }
                else
                {
                    if (address != null)
                    {
                        dbContext.UserCountryAddresses.Remove(address);
                        dbContext.SaveChanges();
                    }
                }
                result.Status = true;
            }
            if (addressType == "ToAddress")
            {
                UserCountryAddress address;
                address = dbContext.UserCountryAddresses.Where(p => p.CountryId == countryId && p.CustomerId == userId && p.ToAddressId != null).FirstOrDefault();
                if (value)
                {
                    if (address != null)
                    {
                        address.ToAddressId = addressBookId;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        address = new UserCountryAddress();
                        address.CountryId = countryId;
                        address.CustomerId = userId;
                        address.ToAddressId = addressBookId;
                        dbContext.UserCountryAddresses.Add(address);
                        dbContext.SaveChanges();
                    }
                }
                else
                {
                    if (address != null)
                    {
                        dbContext.UserCountryAddresses.Remove(address);
                        dbContext.SaveChanges();
                    }
                }

                result.Status = true;

            }

            return result;
        }

        public List<DirectBookingService> GetServices(DirectBookingFindService serviceRequest)
        {
            //Step 0.1: Set Proper Postcode For GetSerives
            serviceRequest.FromPostCode = UtilityRepository.PostCodeVerification(serviceRequest.FromPostCode, serviceRequest.FromCountry.Code2);
            serviceRequest.ToPostCode = UtilityRepository.PostCodeVerification(serviceRequest.ToPostCode, serviceRequest.ToCountry.Code2);

            //Step 1: Get/Set Operation Zone Detail
            FrayteOperationZone OperationZone = UtilityRepository.GetOperationZone();
            serviceRequest.OperationZoneId = OperationZone.OperationZoneId;

            //Step 2: Check wheter both the companies are in Europe?
            bool isEuropeCountry = CheckEuropeCountries(serviceRequest);

            //Step 3: Find out the Logistic Type for the shipment
            string logisticType = UtilityRepository.GetLogisticType(OperationZone.OperationZoneName, serviceRequest.FromCountry.Code, serviceRequest.ToCountry.Code, isEuropeCountry);

            //Step 4: Get Available Service for the Shipment
            List<DirectBookingService> directBookingServices = new List<DirectBookingService>();

            string PackageType = string.Empty;
            string DocType = string.Empty;
            if (logisticType == FrayteLogisticType.UKShipment)
            {
                DocType = null;
                PackageType = serviceRequest.PackageType;
            }
            else if (logisticType == FrayteLogisticType.Import || logisticType == FrayteLogisticType.Export || logisticType == FrayteLogisticType.ThirdParty)
            {
                DocType = serviceRequest.DocType;
                PackageType = null;
            }
            else
            {
                DocType = null;
                PackageType = null;
            }

            //Get OperationZone Wise Exchange Rate
            var exchangeRate = (from OZ in dbContext.OperationZones
                                join OZER in dbContext.OperationZoneExchangeRates on OZ.OperationZoneId equals OZER.OperationZoneId
                                where
                                    OZ.OperationZoneId == OperationZone.OperationZoneId &&
                                    OZER.ExchangeType == FrayteOperationZoneExchangeType.Sell &&
                                    OZER.CurrencyCode == OZ.Currency
                                select new FrayteServiceExchangeRate
                                {
                                    Currency = OZ.Currency,
                                    ExchangeRate = OZER.ExchangeRate
                                }).FirstOrDefault();

            //Get Exchange Rate
            var customercurrency = (from US in dbContext.Users
                                    join UA in dbContext.UserAdditionals on US.UserId equals UA.UserId
                                    join OZER in dbContext.OperationZoneExchangeRates on UA.CreditLimitCurrencyCode equals OZER.CurrencyCode
                                    where
                                        US.UserId == serviceRequest.CustomerId &&
                                        OZER.OperationZoneId == OperationZone.OperationZoneId &&
                                        OZER.ExchangeType == FrayteOperationZoneExchangeType.Sell
                                    select new FrayteCustomerCurrency
                                    {
                                        CreditLimitCurrencyCode = UA.CreditLimitCurrencyCode,
                                        ExchangeRate = OZER.ExchangeRate
                                    }).FirstOrDefault();

            //Date into MM/dd/yyyy format
            string datevalue = serviceRequest.Date.Value.Month.ToString() + "/" + serviceRequest.Date.Value.Day.ToString() + "/" + serviceRequest.Date.Value.Year.ToString();
            DateTime date = Frayte.Services.CommonConversion.ConvertToDateTime(datevalue);

            DataTable packagelist = new DataTable();
            packagelist.Columns.Add("Carton", typeof(int));
            packagelist.Columns.Add("Length", typeof(decimal));
            packagelist.Columns.Add("Width", typeof(decimal));
            packagelist.Columns.Add("Height", typeof(decimal));
            packagelist.Columns.Add("Weight", typeof(decimal));

            foreach (var item in serviceRequest.Packages)
            {
                DataRow row = packagelist.NewRow();
                row["Carton"] = item.CartoonValue;
                row["Length"] = item.Length;
                row["Width"] = item.Width;
                row["Height"] = item.Height;
                row["Weight"] = item.Weight;
                packagelist.Rows.Add(row);
            }
            packagelist.TableName = "PackageList";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["IdentityConnection"].ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.spGet_DirectBookingServices", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@LogisticType", logisticType));
            cmd.Parameters.Add(new SqlParameter("@Weight", serviceRequest.Weight));
            cmd.Parameters.Add(new SqlParameter("@OperationZoneId", OperationZone.OperationZoneId));
            cmd.Parameters.Add(new SqlParameter("@FromCountryId", serviceRequest.FromCountry.CountryId));
            cmd.Parameters.Add(new SqlParameter("@ToCountryId", serviceRequest.ToCountry.CountryId));
            if (serviceRequest.FromPostCode == null)
            {
                cmd.Parameters.Add(new SqlParameter("@FromPostcode", DBNull.Value));
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter("@FromPostcode", serviceRequest.FromPostCode));
            }

            if (serviceRequest.ToPostCode == null)
            {
                cmd.Parameters.Add(new SqlParameter("@ToPostcode", DBNull.Value));
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter("@ToPostcode", serviceRequest.ToPostCode));
            }
            if (PackageType == null)
            {
                cmd.Parameters.Add(new SqlParameter("@PackageType", DBNull.Value));
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter("@PackageType", PackageType));
            }
            if (DocType == null)
            {
                cmd.Parameters.Add(new SqlParameter("@DocType", DBNull.Value));
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter("@DocType", DocType));
            }

            cmd.Parameters.Add(new SqlParameter("@UserId", serviceRequest.CustomerId));
            cmd.Parameters.Add(new SqlParameter("@Date", date));
            cmd.Parameters.Add(new SqlParameter("@AddressType", serviceRequest.AddressType));
            cmd.Parameters.Add(new SqlParameter("@PackageCalculationType", serviceRequest.PackageCalculationType));
            cmd.Parameters.Add(new SqlParameter("@PackageItem", packagelist));

            DataTable dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);
            con.Close();

            if (dt.Rows.Count > 0)
            {
                if (serviceRequest.CallingFrom == FrayteShipmentServiceType.DirectBooking)
                {
                    directBookingServices = GetDirectBookingServices(dt, serviceRequest, exchangeRate, customercurrency);
                }
                else if (serviceRequest.CallingFrom == FrayteShipmentServiceType.Quotation)
                {
                    directBookingServices = GetQuotationServices(dt, serviceRequest, exchangeRate, customercurrency);
                }
            }

            return directBookingServices;
        }

        public List<DirectBookingService> GetQuotationServices(DataTable services, DirectBookingFindService serviceRequest, FrayteServiceExchangeRate exchangeRate, FrayteCustomerCurrency customercurrency)
        {
            List<DirectBookingService> directBookingServices = new List<DirectBookingService>();

            foreach (DataRow service in services.Rows)
            {
                int LogisticServiceId = int.Parse(service["LogisticServiceId"].ToString());
                var options = (from ls in dbContext.LogisticServices
                               join los in dbContext.LogisticOptionalServices on ls.LogisticServiceId equals los.LogisticServiceId
                               where ls.LogisticServiceId == LogisticServiceId
                               select los).ToList();

                DirectBookingService bookingService = new DirectBookingService();
                bookingService.CustomerId = serviceRequest.CustomerId;
                bookingService.CourierId = 0;
                bookingService.LogisticServiceId = int.Parse(service["LogisticServiceId"].ToString());
                bookingService.OptionalServices = new List<DirectBookingOptionalServices>();
                {
                    if (options.Count > 0)
                    {
                        foreach (LogisticOptionalService losp in options)
                        {
                            DirectBookingOptionalServices seropt = new DirectBookingOptionalServices
                            {
                                LogisticOptionalServiceId = losp.LogisticOptionalServiceId,
                                LogisticCompany = service["LogisticCompanyDisplay"].ToString(),
                                ServiceCode = losp.ServiceCode,
                                ServiceDescription = losp.Description,
                                IsEnable = false
                            };
                            bookingService.OptionalServices.Add(seropt);
                        }
                    }
                };
                if (decimal.Parse(service["LogisticRate"].ToString()) >= 0.0m)
                {
                    if (exchangeRate.Currency == service["LogisticCurrency"].ToString())
                    {
                        if (customercurrency != null)
                        {
                            bookingService.AdditionalSurcharge = (float)Math.Round(((service["AddOnRate"].ToString() == null || service["AddOnRate"].ToString() == "") ? 0.0m : decimal.Parse(service["AddOnRate"].ToString())) * customercurrency.ExchangeRate, 2);
                            bookingService.BaseRate = Math.Round((decimal.Parse(service["BaseRate"].ToString()) * customercurrency.ExchangeRate), 2);
                            decimal TotalBasePrice = (bookingService.BaseRate + Convert.ToDecimal(bookingService.AdditionalSurcharge));
                            bookingService.Rate = UtilityRepository.TotalCustomerPrice(TotalBasePrice, decimal.Parse(service["MarginPercent"].ToString()));
                        }
                        else
                        {
                            bookingService.AdditionalSurcharge = (float)Math.Round(((service["AddOnRate"].ToString() == null || service["AddOnRate"].ToString() == "") ? 0.0m : decimal.Parse(service["AddOnRate"].ToString())), 2);
                            bookingService.BaseRate = Math.Round(decimal.Parse(service["BaseRate"].ToString()), 2);
                            decimal TotalBasePrice = (bookingService.BaseRate + Convert.ToDecimal(bookingService.AdditionalSurcharge));
                            bookingService.Rate = UtilityRepository.TotalCustomerPrice(TotalBasePrice, decimal.Parse(service["MarginPercent"].ToString()));
                        }
                    }
                    else if (exchangeRate != null && customercurrency != null)
                    {
                        bookingService.AdditionalSurcharge = (float)Math.Round((((service["AddOnRate"].ToString() == null || service["AddOnRate"].ToString() == "") ? 0.0m : decimal.Parse(service["AddOnRate"].ToString())) / exchangeRate.ExchangeRate) * customercurrency.ExchangeRate, 2);
                        bookingService.BaseRate = Math.Round(((decimal.Parse(service["BaseRate"].ToString()) / exchangeRate.ExchangeRate) * customercurrency.ExchangeRate), 2);
                        decimal TotalBasePrice = (bookingService.BaseRate + Convert.ToDecimal(bookingService.AdditionalSurcharge));
                        bookingService.Rate = UtilityRepository.TotalCustomerPrice(TotalBasePrice, decimal.Parse(service["MarginPercent"].ToString()));
                    }
                    else if (exchangeRate == null && customercurrency != null)
                    {
                        bookingService.AdditionalSurcharge = (float)Math.Round(((service["AddOnRate"].ToString() == null || service["AddOnRate"].ToString() == "") ? 0.0m : decimal.Parse(service["AddOnRate"].ToString())) * customercurrency.ExchangeRate, 2);
                        bookingService.BaseRate = Math.Round(decimal.Parse(service["BaseRate"].ToString()) * customercurrency.ExchangeRate, 2);
                        decimal TotalBasePrice = (bookingService.BaseRate + Convert.ToDecimal(bookingService.AdditionalSurcharge));
                        bookingService.Rate = UtilityRepository.TotalCustomerPrice(TotalBasePrice, decimal.Parse(service["MarginPercent"].ToString()));
                    }
                    else if (exchangeRate != null && customercurrency == null)
                    {
                        bookingService.AdditionalSurcharge = (float)Math.Round(((service["AddOnRate"].ToString() == null || service["AddOnRate"].ToString() == "") ? 0.0m : decimal.Parse(service["AddOnRate"].ToString())) / exchangeRate.ExchangeRate, 2);
                        bookingService.BaseRate = Math.Round(decimal.Parse(service["BaseRate"].ToString()) / exchangeRate.ExchangeRate, 2);
                        decimal TotalBasePrice = (bookingService.BaseRate + Convert.ToDecimal(bookingService.AdditionalSurcharge));
                        bookingService.Rate = UtilityRepository.TotalCustomerPrice(TotalBasePrice, decimal.Parse(service["MarginPercent"].ToString()));
                    }
                }
                bookingService.LogisticShipmentCode = service["LogisticServiceShipmentCode"].ToString();
                bookingService.MarginPercent = decimal.Parse(service["MarginPercent"].ToString());
                bookingService.CourierAccountId = int.Parse(service["LogisticServiceCourierAccountId"].ToString());
                bookingService.CourierAccountNo = service["CourierAccountNo"].ToString();
                bookingService.CourierDescription = service["CourierDescription"].ToString();
                bookingService.CourierAccountCountryCode = service["CourierAccountCountryCode"].ToString();
                bookingService.CourierName = service["LogisticCompany"].ToString();
                if (service["LogisticType"].ToString() == FrayteLogisticType.UKShipment)
                {
                    bookingService.DisplayName = service["LogisticCompanyDisplay"].ToString();
                }
                else
                {
                    bookingService.DisplayName = service["LogisticCompanyDisplay"].ToString();
                }
                bookingService.RateType = service["RateType"].ToString();
                bookingService.RateTypeDisplay = service["RateTypeDisplay"].ToString();
                bookingService.WeightType = service["WeightType"].ToString();
                bookingService.ZoneRateCardId = int.Parse(service["LogisticServiceBaseRateCardId"].ToString());
                bookingService.IntegrationAccountId = service["IntegrationAccountId"].ToString();
                bookingService.PakageType = service["PackageType"].ToString();
                bookingService.ParcelServiceType = service["ParcelType"].ToString();
                bookingService.LogisticType = service["LogisticType"].ToString();
                bookingService.UnitOfMeasurement = service["UOM"].ToString();
                bookingService.FuelSurcharge = (float)Math.Round(service["FuelSurcharge"].ToString() == null ? 0.0m : decimal.Parse(service["FuelSurcharge"].ToString()), 2);
                if (bookingService.WeightType == FrayteShipmentType.WeightType)
                {
                    var baseprice = (bookingService.BaseRate * serviceRequest.Weight);
                    var CustomerPrice = Math.Round((baseprice * decimal.Parse(service["MarginPercent"].ToString()) / 100), 2);
                    bookingService.TotalEstimatedCharge = (float)UtilityRepository.GrandTotal((baseprice + CustomerPrice), Convert.ToDecimal(bookingService.FuelSurcharge), bool.Parse(service["IsFuelSurchargeCalculate"].ToString()));
                }
                else
                {
                    bookingService.TotalEstimatedCharge = (float)UtilityRepository.GrandTotal(bookingService.Rate, Convert.ToDecimal(bookingService.FuelSurcharge), bool.Parse(service["IsFuelSurchargeCalculate"].ToString()));
                }
                bookingService.CurrencyCode = service["LogisticCurrency"].ToString();
                if (customercurrency != null)
                {
                    bookingService.CustomerCurrency = customercurrency.CreditLimitCurrencyCode;
                }

                bookingService.LogisticServiceType = service["LogisticTypeDisplay"].ToString() == null ? "" : service["LogisticTypeDisplay"].ToString();
                bookingService.FuelMonth = service["FuelMonth"].ToString() + (DateTime.Parse(service["FuelDate"].ToString()).ToString("dd/MM/yyyy") == "01/01/0001" ? "" : "-" + DateTime.Parse(service["FuelDate"].ToString()).Year.ToString().Substring(2, 2));
                bookingService.FuelDate = DateTime.Parse(service["FuelDate"].ToString());
                bookingService.LogisticShipmentId = int.Parse(service["LogisticServiceShipmentTypeId"].ToString());
                bookingService.LogisticDescription = service["LogisticDescription"].ToString();
                bookingService.Weight = decimal.Parse(service["UserWeight"].ToString());
                bookingService.TransitTime = (int.Parse(service["TransitTime"].ToString()) == 0 ? "N/A" : (int.Parse(service["TransitTime"].ToString()) + "-" + (int.Parse(service["TransitTime"].ToString()) + 1)).ToString() + " Days");
                bookingService.IsFuelSurchargeCalculate = bool.Parse(service["IsFuelSurchargeCalculate"].ToString());
                bookingService.NetworkCode = service["NetworkCode"].ToString();
                bookingService.CarrierLogo = "";
                directBookingServices.Add(bookingService);
            }
            return directBookingServices;
        }

        public List<DirectBookingService> GetDirectBookingServices(DataTable services, DirectBookingFindService serviceRequest, FrayteServiceExchangeRate exchangeRate, FrayteCustomerCurrency customercurrency)
        {
            List<DirectBookingService> directBookingServices = new List<DirectBookingService>();

            foreach (DataRow service in services.Rows)
            {
                int LogisticServiceId = int.Parse(service["LogisticServiceId"].ToString());
                var options = (from ls in dbContext.LogisticServices
                               join los in dbContext.LogisticOptionalServices on ls.LogisticServiceId equals los.LogisticServiceId
                               where ls.LogisticServiceId == LogisticServiceId
                               select los).ToList();

                DirectBookingService bookingService = new DirectBookingService();
                bookingService.CustomerId = serviceRequest.CustomerId;
                bookingService.CourierId = 0;
                bookingService.LogisticServiceId = int.Parse(service["LogisticServiceId"].ToString());
                bookingService.OptionalServices = new List<DirectBookingOptionalServices>();
                {
                    if (options.Count > 0)
                    {
                        foreach (LogisticOptionalService losp in options)
                        {
                            DirectBookingOptionalServices seropt = new DirectBookingOptionalServices
                            {
                                LogisticOptionalServiceId = losp.LogisticOptionalServiceId,
                                LogisticCompany = service["LogisticCompanyDisplay"].ToString(),
                                ServiceCode = losp.ServiceCode,
                                ServiceDescription = losp.Description,
                                IsEnable = false
                            };
                            bookingService.OptionalServices.Add(seropt);
                        }
                    }
                };
                if (decimal.Parse(service["LogisticRate"].ToString()) >= 0.0m)
                {
                    if (exchangeRate.Currency == service["LogisticCurrency"].ToString())
                    {
                        if (customercurrency != null)
                        {
                            bookingService.AdditionalSurcharge = (float)Math.Round(((service["AddOnRate"].ToString() == null || service["AddOnRate"].ToString() == "") ? 0.0m : decimal.Parse(service["AddOnRate"].ToString())) * customercurrency.ExchangeRate, 2);
                            bookingService.BaseRate = Math.Round((decimal.Parse(service["BaseRate"].ToString()) * customercurrency.ExchangeRate), 2);
                            decimal TotalBasePrice = (bookingService.BaseRate + Convert.ToDecimal(bookingService.AdditionalSurcharge));
                            bookingService.Rate = UtilityRepository.TotalCustomerPrice(TotalBasePrice, decimal.Parse(service["MarginPercent"].ToString()));
                        }
                        else
                        {
                            bookingService.AdditionalSurcharge = (float)Math.Round(((service["AddOnRate"].ToString() == null || service["AddOnRate"].ToString() == "") ? 0.0m : decimal.Parse(service["AddOnRate"].ToString())), 2);
                            bookingService.BaseRate = Math.Round(decimal.Parse(service["BaseRate"].ToString()), 2);
                            decimal TotalBasePrice = (bookingService.BaseRate + Convert.ToDecimal(bookingService.AdditionalSurcharge));
                            bookingService.Rate = UtilityRepository.TotalCustomerPrice(TotalBasePrice, decimal.Parse(service["MarginPercent"].ToString()));
                        }
                    }
                    else if (exchangeRate != null && customercurrency != null)
                    {
                        bookingService.AdditionalSurcharge = (float)Math.Round((((service["AddOnRate"].ToString() == null || service["AddOnRate"].ToString() == "") ? 0.0m : decimal.Parse(service["AddOnRate"].ToString())) / exchangeRate.ExchangeRate) * customercurrency.ExchangeRate, 2);
                        bookingService.BaseRate = Math.Round(((decimal.Parse(service["BaseRate"].ToString()) / exchangeRate.ExchangeRate) * customercurrency.ExchangeRate), 2);
                        decimal TotalBasePrice = (bookingService.BaseRate + Convert.ToDecimal(bookingService.AdditionalSurcharge));
                        bookingService.Rate = UtilityRepository.TotalCustomerPrice(TotalBasePrice, decimal.Parse(service["MarginPercent"].ToString()));
                    }
                    else if (exchangeRate == null && customercurrency != null)
                    {
                        bookingService.AdditionalSurcharge = (float)Math.Round(((service["AddOnRate"].ToString() == null || service["AddOnRate"].ToString() == "") ? 0.0m : decimal.Parse(service["AddOnRate"].ToString())) * customercurrency.ExchangeRate, 2);
                        bookingService.BaseRate = Math.Round(decimal.Parse(service["BaseRate"].ToString()) * customercurrency.ExchangeRate, 2);
                        decimal TotalBasePrice = (bookingService.BaseRate + Convert.ToDecimal(bookingService.AdditionalSurcharge));
                        bookingService.Rate = UtilityRepository.TotalCustomerPrice(TotalBasePrice, decimal.Parse(service["MarginPercent"].ToString()));
                    }
                    else if (exchangeRate != null && customercurrency == null)
                    {
                        bookingService.AdditionalSurcharge = (float)Math.Round(((service["AddOnRate"].ToString() == null || service["AddOnRate"].ToString() == "") ? 0.0m : decimal.Parse(service["AddOnRate"].ToString())) / exchangeRate.ExchangeRate, 2);
                        bookingService.BaseRate = Math.Round(decimal.Parse(service["BaseRate"].ToString()) / exchangeRate.ExchangeRate, 2);
                        decimal TotalBasePrice = (bookingService.BaseRate + Convert.ToDecimal(bookingService.AdditionalSurcharge));
                        bookingService.Rate = UtilityRepository.TotalCustomerPrice(TotalBasePrice, decimal.Parse(service["MarginPercent"].ToString()));
                    }
                }
                bookingService.LogisticShipmentCode = service["LogisticServiceShipmentCode"].ToString();
                bookingService.MarginPercent = decimal.Parse(service["MarginPercent"].ToString());
                bookingService.CourierAccountId = int.Parse(service["LogisticServiceCourierAccountId"].ToString());
                bookingService.CourierAccountNo = service["CourierAccountNo"].ToString();
                bookingService.CourierDescription = service["CourierDescription"].ToString();
                bookingService.CourierAccountCountryCode = service["CourierAccountCountryCode"].ToString();
                bookingService.CourierName = service["LogisticCompany"].ToString();
                if (service["LogisticType"].ToString() == FrayteLogisticType.UKShipment)
                {
                    bookingService.DisplayName = service["LogisticCompanyDisplay"].ToString();
                }
                else
                {
                    bookingService.DisplayName = service["LogisticCompanyDisplay"].ToString();
                }
                bookingService.RateType = service["RateType"].ToString();
                bookingService.RateTypeDisplay = service["RateTypeDisplay"].ToString();
                bookingService.WeightType = service["WeightType"].ToString();
                bookingService.ZoneRateCardId = int.Parse(service["LogisticServiceBaseRateCardId"].ToString());
                bookingService.IntegrationAccountId = service["IntegrationAccountId"].ToString();
                bookingService.PakageType = service["PackageType"].ToString();
                bookingService.ParcelServiceType = service["ParcelType"].ToString();
                bookingService.LogisticType = service["LogisticType"].ToString();
                bookingService.UnitOfMeasurement = service["UOM"].ToString();
                bookingService.FuelSurcharge = (float)Math.Round(service["FuelSurcharge"].ToString() == null ? 0.0m : decimal.Parse(service["FuelSurcharge"].ToString()), 2);
                if (bookingService.WeightType == FrayteShipmentType.WeightType)
                {
                    var baseprice = (bookingService.BaseRate * serviceRequest.Weight);
                    var CustomerPrice = Math.Round((baseprice * decimal.Parse(service["MarginPercent"].ToString()) / 100), 2);
                    bookingService.TotalEstimatedCharge = (float)UtilityRepository.GrandTotal((baseprice + CustomerPrice), Convert.ToDecimal(bookingService.FuelSurcharge), bool.Parse(service["IsFuelSurchargeCalculate"].ToString()));
                }
                else
                {
                    bookingService.TotalEstimatedCharge = (float)UtilityRepository.GrandTotal(bookingService.Rate, Convert.ToDecimal(bookingService.FuelSurcharge), bool.Parse(service["IsFuelSurchargeCalculate"].ToString()));
                }
                bookingService.CurrencyCode = service["LogisticCurrency"].ToString();
                if (customercurrency != null)
                {
                    bookingService.CustomerCurrency = customercurrency.CreditLimitCurrencyCode;
                }

                bookingService.LogisticServiceType = service["LogisticTypeDisplay"].ToString() == null ? "" : service["LogisticTypeDisplay"].ToString();
                bookingService.FuelMonth = service["FuelMonth"].ToString() + (DateTime.Parse(service["FuelDate"].ToString()).ToString("dd/MM/yyyy") == "01/01/0001" ? "" : "-" + DateTime.Parse(service["FuelDate"].ToString()).Year.ToString().Substring(2, 2));
                bookingService.FuelDate = DateTime.Parse(service["FuelDate"].ToString());
                bookingService.LogisticShipmentId = int.Parse(service["LogisticServiceShipmentTypeId"].ToString());
                bookingService.LogisticDescription = service["LogisticDescription"].ToString();
                bookingService.Weight = decimal.Parse(service["UserWeight"].ToString());
                bookingService.TransitTime = (int.Parse(service["TransitTime"].ToString()) == 0 ? "N/A" : (int.Parse(service["TransitTime"].ToString()) + "-" + (int.Parse(service["TransitTime"].ToString()) + 1)).ToString() + " Days");
                bookingService.IsFuelSurchargeCalculate = bool.Parse(service["IsFuelSurchargeCalculate"].ToString());
                bookingService.NetworkCode = service["NetworkCode"].ToString();
                bookingService.CarrierLogo = "";
                directBookingServices.Add(bookingService);
            }
            return directBookingServices;
        }

        public bool MarkDraftShipmentAsPublic(int DirectShipmentDraftId, bool IsPublic)
        {
            if (DirectShipmentDraftId > 0)
            {
                var detail = dbContext.DirectShipmentDrafts.Find(DirectShipmentDraftId);
                if (detail != null)
                {
                    detail.IsPublic = IsPublic;
                    dbContext.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
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

        public List<int> GetAllDirectShipments()
        {
            try
            {
                var OperationZone = UtilityRepository.GetOperationZone();
                return dbContext.DirectShipments.Where(p => p.OpearionZoneId == OperationZone.OperationZoneId).Select(p => p.DirectShipmentId).ToList();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        public List<DirectShipmentDetail> GetPackageDetails(int directShipmentId)
        {
            var data = dbContext.DirectShipmentDetails.Where(p => p.DirectShipmentId == directShipmentId).ToList();
            if (data != null && data.Count > 0)
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        public DirectShipment GetShipmentImage(int directShipmentId)
        {
            var data = dbContext.DirectShipments.FirstOrDefault(p => p.DirectShipmentId == directShipmentId);
            if (data != null)
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        public List<PackageTrackingDetail> GetPackageTracking(int directShipmentDetailId)
        {
            try
            {
                var PackageTracking = dbContext.PackageTrackingDetails.Where(p => p.DirectShipmentDetailId == directShipmentDetailId).ToList();
                if (PackageTracking != null)
                {
                    return PackageTracking;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        private bool CheckEuropeCountries(DirectBookingFindService serviceRequest)
        {
            bool isFromCountryInEurope = false;
            bool isToCountryInEurope = false;

            //Check fromCountryCode
            if (serviceRequest.FromCountry.Code == "GBR")
            {
                isFromCountryInEurope = true;
            }
            {
                var countryFound = (from zc in dbContext.LogisticServiceZoneCountries
                                    join c in dbContext.Countries on zc.CountryId equals c.CountryId
                                    join z in dbContext.LogisticServiceZones on zc.LogisticServiceZoneId equals z.LogisticServiceZoneId
                                    join ls in dbContext.LogisticServices on z.LogisticServiceId equals ls.LogisticServiceId
                                    where
                                        ls.OperationZoneId == serviceRequest.OperationZoneId && c.CountryId == serviceRequest.FromCountry.CountryId
                                        && (ls.LogisticType == FrayteLogisticType.EUImport || ls.LogisticType == FrayteLogisticType.EUExport)
                                    select zc).ToList();
                if (countryFound != null && countryFound.Count > 0)
                {
                    isFromCountryInEurope = true;
                }
            }

            if (serviceRequest.ToCountry.Code == "GBR")
            {
                isToCountryInEurope = true;
            }
            {
                var countryFound = (from zc in dbContext.LogisticServiceZoneCountries
                                    join c in dbContext.Countries on zc.CountryId equals c.CountryId
                                    join z in dbContext.LogisticServiceZones on zc.LogisticServiceZoneId equals z.LogisticServiceZoneId
                                    join ls in dbContext.LogisticServices on z.LogisticServiceId equals ls.LogisticServiceId
                                    where
                                        ls.OperationZoneId == serviceRequest.OperationZoneId && c.CountryId == serviceRequest.ToCountry.CountryId
                                        && (ls.LogisticType == FrayteLogisticType.EUImport || ls.LogisticType == FrayteLogisticType.EUExport)
                                    select zc).ToList();

                if (countryFound != null && countryFound.Count > 0)
                {
                    isToCountryInEurope = true;
                }
            }

            return isFromCountryInEurope && isToCountryInEurope;
        }

        #region -- GetDirectBookingDetail --

        public DirectBookingShipmentDetail GetDirectBookingDetail(int directShipmentId, string CallingType)
        {
            DirectBookingShipmentDetail dbDetail = new DirectBookingShipmentDetail();
            dbDetail.DirectShipmentId = directShipmentId;

            //Step 1: Get Shipment Detail
            GetDirectShipmnetDetail(dbDetail);

            //Step 2: Get Shipment Packages Detail
            GetDirectShipmentPackagesDetail(dbDetail, CallingType);

            //Step 3: Get Ship From and Ship To detail
            GetDirectShipmentCollectionDetail(dbDetail);

            //Step 4: Get Custom Info Detail
            GetDirectShipmentCustomDetail(dbDetail);

            if (CallingType == FrayteCallingType.ShipmentClone)
            {
                dbDetail.DirectShipmentId = 0;
                dbDetail.ShipFrom.DirectShipmentAddressId = 0;
                dbDetail.ShipTo.DirectShipmentAddressId = 0;
            }
            else if (CallingType == FrayteCallingType.ShipmentReturn)
            {
                dbDetail.DirectShipmentId = 0;
                dbDetail.ShipFrom.DirectShipmentAddressId = 0;
                dbDetail.ShipTo.DirectShipmentAddressId = 0;
            }

            return dbDetail;
        }

        public FrayteDirectShipment GetShipmentDetail(int DirectShipmentid)
        {
            var result = (from DS in dbContext.DirectShipments
                          where DS.DirectShipmentId == DirectShipmentid
                          select new FrayteDirectShipment
                          {
                              DirectShipmentId = DS.DirectShipmentId,
                              FromAddressId = DS.FromAddressId,
                              ToAddressId = DS.ToAddressId,
                              ShipmentStatusId = DS.ShipmentStatusId
                          }).FirstOrDefault();

            return result;
        }

        public DirectBookingShipmentDraftDetail GetDirectShipmentDraftDetail(int DirectShipmentDraftId, string CallingType)
        {
            DirectBookingShipmentDraftDetail dbDetail = new DirectBookingShipmentDraftDetail();
            if (CallingType == FrayteCallingType.ShipmentClone)
            {
                dbDetail.DirectShipmentDraftId = DirectShipmentDraftId;

                //Step 1: Get Shipment Detail Draft
                GetDirectShipmnetDraftDetail(dbDetail, CallingType);

                //Step 2: Get Shipment Packages Draft Detail
                GetDirectShipmentPackagesDraftDetail(dbDetail, CallingType);

                //Step 3: Get Ship From and Ship To Detail Draft
                GetDirectShipmentCollectionDraftDetail(dbDetail, CallingType);

                //Step 4: Get Custom Info Detail Draft
                GetDirectShipmentCustomDraftDetail(dbDetail, CallingType);

                dbDetail.DirectShipmentDraftId = 0;
                dbDetail.ShipFrom.AddressBookId = 0;
                dbDetail.ShipTo.AddressBookId = 0;
            }
            else if (CallingType == FrayteCallingType.ShipmentReturn)
            {
                dbDetail.DirectShipmentDraftId = DirectShipmentDraftId;

                //Step 5: Get Shipment Detail Draft
                GetDirectShipmnetDraftDetail(dbDetail, CallingType);

                //Step 6: Get Shipment Packages Draft Detail
                GetDirectShipmentPackagesDraftDetail(dbDetail, CallingType);

                //Step 7: Get Ship From and Ship To Detail Draft
                GetDirectShipmentCollectionDraftDetail(dbDetail, CallingType);

                //Step 8: Get Custom Info Detail Draft
                GetDirectShipmentCustomDraftDetail(dbDetail, CallingType);

                dbDetail.DirectShipmentDraftId = 0;
                dbDetail.ShipFrom.AddressBookId = 0;
                dbDetail.ShipTo.AddressBookId = 0;
            }
            else if (CallingType == FrayteCallingType.ShipmentDraft)
            {
                dbDetail.DirectShipmentDraftId = DirectShipmentDraftId;

                //Step 5: Get Shipment Detail Draft
                GetDirectShipmnetDraftDetail(dbDetail, CallingType);

                //Step 6: Get Shipment Packages Draft Detail
                GetDirectShipmentPackagesDraftDetail(dbDetail, CallingType);

                //Step 7: Get Ship From and Ship To Detail Draft
                GetDirectShipmentCollectionDraftDetail(dbDetail, CallingType);

                //Step 8: Get Custom Info Detail Draft
                GetDirectShipmentCustomDraftDetail(dbDetail, CallingType);
            }
            else if (CallingType == FrayteCallingType.ShipmentQuotation)
            {
                dbDetail.DirectShipmentDraftId = DirectShipmentDraftId;

                //Step 9: Get Quotation Ship From and Ship To Detail and Set In Model Properties
                GetQuotationShipFromShipToDetail(dbDetail);

                //Step 10: Get Quotation Shipment Detail and Set In Model Properties
                GetQuotationShipmentDetail(dbDetail);

                //Step 11: Get Quotation Shipment Package Detail and Set In Model Properties
                GetQuotationShipmentPackageDetail(dbDetail);

                //Step 12: Get Quotation Seleted Service and Set In Model Properties
                GetQuotationServices(dbDetail);

                dbDetail.DirectShipmentDraftId = 0;
            }
            return dbDetail;
        }

        private void GetDirectShipmentCustomDetail(DirectBookingShipmentDetail dbDetail)
        {
            ShipmentCustomDetail customDetail = dbContext.ShipmentCustomDetails.Where(p => p.ShipmentId == dbDetail.DirectShipmentId && p.ShipmentServiceType == FrayteShipmentServiceType.DirectBooking).FirstOrDefault();
            if (customDetail != null)
            {
                dbDetail.CustomInfo = new CustomInformation();
                dbDetail.CustomInfo.ShipmentId = dbDetail.DirectShipmentId;
                dbDetail.CustomInfo.ShipmentCustomDetailId = customDetail.ShipmentCustomDetailId;
                //EasyPost Details
                dbDetail.CustomInfo.ContentsType = customDetail.ContentsType;
                dbDetail.CustomInfo.ContentsExplanation = customDetail.ContentsExplanation;
                dbDetail.CustomInfo.RestrictionType = customDetail.RestrictionType;
                dbDetail.CustomInfo.RestrictionComments = customDetail.RestrictionComments;
                dbDetail.CustomInfo.CustomsCertify = customDetail.CustomsCertify;
                dbDetail.CustomInfo.CustomsSigner = customDetail.CustomsSigner;
                dbDetail.CustomInfo.NonDeliveryOption = customDetail.NonDeliveryOption;
                dbDetail.CustomInfo.EelPfc = customDetail.EelPfc;

                //Parcle Hub Details
                dbDetail.CustomInfo.CatagoryOfItem = customDetail.CatagoryOfItem;
                dbDetail.CustomInfo.CatagoryOfItemExplanation = customDetail.CatagoryOfItemExplanation;
                dbDetail.CustomInfo.TermOfTrade = dbDetail.PayTaxAndDuties;
            }
        }

        private void GetDirectShipmentCollectionDetail(DirectBookingShipmentDetail dbDetail)
        {
            //Ship From
            DirectShipmentAddress shipFrom = dbContext.DirectShipmentAddresses.Find(dbDetail.ShipFrom.DirectShipmentAddressId);
            if (shipFrom != null)
            {
                dbDetail.ShipFrom.Address = shipFrom.Address1;
                dbDetail.ShipFrom.Address2 = shipFrom.Address2;
                dbDetail.ShipFrom.Area = shipFrom.Area;
                dbDetail.ShipFrom.City = shipFrom.City;
                dbDetail.ShipFrom.CompanyName = shipFrom.CompanyName;

                dbDetail.ShipFrom.Country = new FrayteCountryCode();
                var country = dbContext.Countries.Where(p => p.CountryId == shipFrom.CountryId).FirstOrDefault();
                if (country != null)
                {
                    dbDetail.ShipFrom.Country.CountryId = country.CountryId;
                    dbDetail.ShipFrom.Country.Code = country.CountryCode;
                    dbDetail.ShipFrom.Country.Code2 = country.CountryCode2;
                    dbDetail.ShipFrom.Country.Name = country.CountryName;
                    dbDetail.ShipFrom.Phone = "( +" + country.CountryPhoneCode + " ) " + shipFrom.PhoneNo;
                }
                else
                {
                    dbDetail.ShipFrom.Phone = shipFrom.PhoneNo;
                }

                dbDetail.ShipFrom.Email = shipFrom.Email;
                dbDetail.ShipFrom.FirstName = shipFrom.ContactFirstName;
                dbDetail.ShipFrom.LastName = shipFrom.ContactLastName;
                dbDetail.ShipFrom.PostCode = shipFrom.Zip;
                dbDetail.ShipFrom.State = shipFrom.State;

            }

            //Ship To
            DirectShipmentAddress shipTo = dbContext.DirectShipmentAddresses.Find(dbDetail.ShipTo.DirectShipmentAddressId);
            if (shipTo != null)
            {
                dbDetail.ShipTo.Address = shipTo.Address1;
                dbDetail.ShipTo.Address2 = shipTo.Address2;
                dbDetail.ShipTo.Area = shipTo.Area;
                dbDetail.ShipTo.City = shipTo.City;
                dbDetail.ShipTo.CompanyName = shipTo.CompanyName;

                dbDetail.ShipTo.Country = new FrayteCountryCode();
                var country = dbContext.Countries.Where(p => p.CountryId == shipTo.CountryId).FirstOrDefault();
                if (country != null)
                {
                    dbDetail.ShipTo.Country.CountryId = country.CountryId;
                    dbDetail.ShipTo.Country.Code = country.CountryCode;
                    dbDetail.ShipTo.Country.Code2 = country.CountryCode2;
                    dbDetail.ShipTo.Country.Name = country.CountryName;

                    dbDetail.ShipTo.Phone = "( +" + country.CountryPhoneCode + " ) " + shipTo.PhoneNo;
                }
                else
                {
                    dbDetail.ShipTo.Phone = shipTo.PhoneNo;
                }
                dbDetail.ShipTo.Email = shipTo.Email;
                dbDetail.ShipTo.FirstName = shipTo.ContactFirstName;
                dbDetail.ShipTo.LastName = shipTo.ContactLastName;
                dbDetail.ShipTo.PostCode = shipTo.Zip;
                dbDetail.ShipTo.State = shipTo.State;
            }
        }

        public int SaveShipment(DirectBookingShipmentDraftDetail dbDetail, IntegrtaionResult result)
        {
            try
            {
                int directShipmentId = SaveShipment(dbDetail, result.TrackingNumber, dbDetail.CustomerId, result.PickupRef, result.ShipmentImage);
                return directShipmentId;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public int UpdateSession(DirectBookingShipmentDraftDetail dbDetail)
        {
            var b = new DirectBookingUploadShipmentRepository().UpdateSession(dbDetail.SessionId, 1);
            return b;
        }

        public int UpdateExpressSession(ExpressShipmentModel expressBookingDetail)
        {
            var b = new DirectBookingUploadShipmentRepository().UpdateSession(Convert.ToInt32(expressBookingDetail.Service.HubCarrierServiceId), 1);
            return b;
        }

        public int SaveShipment(DirectBookingShipmentDraftDetail dbDetail, string orderId, int CustomerId, string PickupRef, string ShipmentImange)
        {
            int DirectShipmentId = 0;
            if (orderId != null || orderId != "")
            {

                //Step 1.0 Save Draft Data Into Direct Shipment Table
                dbContext.spGet_SaveDraftAsDirectShipment(dbDetail.DirectShipmentDraftId, orderId, "DirectBooking", 0, DateTime.UtcNow, DirectBookingShippingStatus.Current, CustomerId, null, null, null, dbDetail.FrayteNumber, PickupRef, ShipmentImange);

                //Step 1.1 Save Tracking No in DirectShipment Table
                DirectShipment directShipment = dbContext.DirectShipments.Where(p => p.TrackingDetail == orderId).FirstOrDefault();
                DirectShipmentId = directShipment.DirectShipmentId;

                //Save tracking no to trackingroute table
                if (!string.IsNullOrEmpty(orderId) && DirectShipmentId > 0)
                {
                    SaveDirectBookingTrackingNo(orderId, DirectShipmentId);
                }
                //Save FrayteRef no to trackingroute table
                if (!string.IsNullOrEmpty(directShipment.FrayteNumber) && DirectShipmentId > 0)
                {
                    SaveDirectBookingRef(directShipment.FrayteNumber, DirectShipmentId);
                }
                //Step 1.2 Save Direct Shipment Address from Addressbook
                DirectShipmentAddress address;
                if (dbDetail.ShipFrom != null)
                {
                    address = new DirectShipmentAddress();
                    address.ContactFirstName = dbDetail.ShipFrom.FirstName;
                    address.ContactLastName = dbDetail.ShipFrom.LastName;
                    address.CompanyName = dbDetail.ShipFrom.CompanyName;
                    address.Email = dbDetail.ShipFrom.Email;
                    address.PhoneNo = dbDetail.ShipFrom.Phone;
                    address.Address1 = dbDetail.ShipFrom.Address;
                    address.Area = dbDetail.ShipFrom.Area;
                    address.Address2 = dbDetail.ShipFrom.Address2;
                    address.City = dbDetail.ShipFrom.City;
                    address.State = dbDetail.ShipFrom.State;
                    address.Zip = dbDetail.ShipFrom.PostCode;
                    address.CountryId = dbDetail.ShipFrom.Country.CountryId;
                    address.IsActive = true;
                    address.TableType = FrayteTableType.DirectBooking;
                    dbContext.DirectShipmentAddresses.Add(address);
                    if (address != null)
                    {
                        dbContext.SaveChanges();
                    }
                    //Update Direct Shipment From Address Id
                    if (directShipment != null)
                    {
                        directShipment.FromAddressId = address.DirectShipmentAddressId;
                        dbContext.SaveChanges();
                    }
                }
                if (dbDetail.ShipTo != null)
                {
                    address = new DirectShipmentAddress();
                    address.ContactFirstName = dbDetail.ShipTo.FirstName;
                    address.ContactLastName = dbDetail.ShipTo.LastName;
                    address.CompanyName = dbDetail.ShipTo.CompanyName;
                    address.Email = dbDetail.ShipTo.Email;
                    address.PhoneNo = dbDetail.ShipTo.Phone;
                    address.Address1 = dbDetail.ShipTo.Address;
                    address.Area = dbDetail.ShipTo.Area;
                    address.Address2 = dbDetail.ShipTo.Address2;
                    address.City = dbDetail.ShipTo.City;
                    address.State = dbDetail.ShipTo.State;
                    address.Zip = dbDetail.ShipTo.PostCode;
                    address.CountryId = dbDetail.ShipTo.Country.CountryId;
                    address.IsActive = true;
                    address.TableType = FrayteTableType.DirectBooking;
                    dbContext.DirectShipmentAddresses.Add(address);

                    if (address != null)
                    {
                        dbContext.SaveChanges();
                    }
                    //updateSession Table
                    UpdateSession(dbDetail);

                    //Update Direct Shipment To Address Id
                    if (directShipment != null)
                    {
                        directShipment.ToAddressId = address.DirectShipmentAddressId;
                        dbContext.SaveChanges();
                    }
                }

                //Step 1.21 Set default address in address book
                if (dbDetail.ShipFrom.IsDefaultAddess)
                {
                    UserCountryAddress countryAddress;
                    var shipfromAdd = dbContext.AddressBooks.Find(dbDetail.ShipFrom.AddressBookId);
                    if (shipfromAdd != null)
                    {

                        countryAddress = dbContext.UserCountryAddresses.Where(p => p.CustomerId == dbDetail.CustomerId && p.CountryId == dbDetail.ShipFrom.Country.CountryId && p.FromAddressId != null).FirstOrDefault();
                        if (countryAddress != null)
                        {
                            countryAddress.FromAddressId = shipfromAdd.AddressBookId;
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            countryAddress.CountryId = dbDetail.ShipFrom.Country.CountryId;
                            countryAddress.CustomerId = dbDetail.CustomerId;
                            countryAddress.FromAddressId = shipfromAdd.AddressBookId;
                            dbContext.UserCountryAddresses.Add(countryAddress);
                            dbContext.SaveChanges();
                        }
                        if (!shipfromAdd.IsDefault)
                        {
                            shipfromAdd.IsDefault = dbDetail.ShipFrom.IsDefaultAddess;
                            dbContext.SaveChanges();
                        }
                    }
                }

                if (dbDetail.ShipTo.IsDefaultAddess)
                {
                    var shiptoAdd = dbContext.AddressBooks.Find(dbDetail.ShipTo.AddressBookId);
                    if (shiptoAdd != null)
                    {
                        UserCountryAddress countryAddress;
                        countryAddress = dbContext.UserCountryAddresses.Where(p => p.CustomerId == dbDetail.CustomerId && p.CountryId == dbDetail.ShipTo.Country.CountryId && p.ToAddressId != null).FirstOrDefault();
                        if (countryAddress != null)
                        {
                            countryAddress.FromAddressId = shiptoAdd.AddressBookId;
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            countryAddress.CountryId = dbDetail.ShipTo.Country.CountryId;
                            countryAddress.CustomerId = dbDetail.CustomerId;
                            countryAddress.ToAddressId = shiptoAdd.AddressBookId;
                            dbContext.UserCountryAddresses.Add(countryAddress);
                            dbContext.SaveChanges();
                        }
                        if (!shiptoAdd.IsDefault)
                        {
                            shiptoAdd.IsDefault = dbDetail.ShipTo.IsDefaultAddess;
                            dbContext.SaveChanges();
                        }
                    }
                }

                //Step 1.3 Save NDS optional service detail
                DirectShipmentOptionalService ser;
                if (dbDetail.CustomerRateCard.OptionalServices != null && dbDetail.CustomerRateCard.OptionalServices.Count > 0)
                {
                    foreach (var detail in dbDetail.CustomerRateCard.OptionalServices)
                    {
                        if (detail.IsEnable)
                        {
                            ser = new DirectShipmentOptionalService();
                            ser.LogisticOptionalServiceId = detail.LogisticOptionalServiceId;
                            ser.OptionalServiceCode = detail.ServiceCode;
                            ser.DirectShipmentId = DirectShipmentId;
                            dbContext.DirectShipmentOptionalServices.Add(ser);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            return DirectShipmentId;
        }

        public List<int> GetDirectShipmentDetailID(int DirectShipmentID)
        {
            List<int> _id = new List<int>();
            _id = dbContext.DirectShipmentDetails.Where(p => p.DirectShipmentId == DirectShipmentID).Select(p => p.DirectShipmentDetailId).ToList();
            return _id;
        }

        public List<int> GetExpressDirectShipmentDetailID(int ExpressShipmentID)
        {
            List<int> _id = new List<int>();
            _id = dbContext.ExpressDetails.Where(p => p.ExpressId == ExpressShipmentID).Select(p => p.ExpressDetailId).ToList();
            return _id;
        }

        private void GetDirectShipmentPackagesDetail(DirectBookingShipmentDetail dbDetail, string CallingType)
        {
            Package dbPackage;
            dbDetail.Packages = new List<Package>();
            if (CallingType == FrayteCallingType.ShipmentClone)
            {
                var result = (from DS in dbContext.DirectShipments
                              join DSD in dbContext.DirectShipmentDetails on DS.DirectShipmentId equals DSD.DirectShipmentId
                              where DS.DirectShipmentId == dbDetail.DirectShipmentId
                              select new
                              {
                                  DirectShipmetDetailId = 0,
                                  PiecesContent = DSD.PiecesContent,
                                  CartoonValue = DSD.CartoonValue,
                                  Height = DSD.Height,
                                  Length = DSD.Length,
                                  Width = DSD.Width,
                                  Weight = DSD.Weight,
                                  DeclaredValue = DSD.DeclaredValue
                              }).ToList();

                if (result != null && result.Count > 0)
                {
                    foreach (var Obj in result)
                    {
                        dbPackage = new Package();
                        dbPackage.Content = Obj.PiecesContent;
                        dbPackage.DirectShipmentDetailId = Obj.DirectShipmetDetailId;
                        dbPackage.CartoonValue = Obj.CartoonValue;
                        dbPackage.Height = Obj.Height;
                        dbPackage.Length = Obj.Length;
                        if (Obj.DeclaredValue.HasValue)
                        {
                            dbPackage.Value = Obj.DeclaredValue.Value;
                        }
                        dbPackage.Weight = Obj.Weight;
                        dbPackage.Width = Obj.Width;
                        dbDetail.Packages.Add(dbPackage);
                    }
                }
                else
                {
                    dbPackage = new Package();
                    dbPackage.Content = "";
                    dbPackage.DirectShipmentDetailId = 0;
                    dbPackage.PackageTrackingDetailId = 0;
                    dbPackage.CartoonValue = 0;
                    dbPackage.Height = 0;
                    dbPackage.Length = 0;
                    dbPackage.Value = 0;
                    dbPackage.Weight = 0;
                    dbPackage.Width = 0;
                    dbPackage.IsPrinted = false;
                    dbPackage.TrackingNo = "";
                    dbPackage.LabelName = "";
                    dbDetail.Packages.Add(dbPackage);
                }
            }
            else if (CallingType == FrayteCallingType.ShipmentReturn)
            {
                var result = (from DS in dbContext.DirectShipments
                              join DSD in dbContext.DirectShipmentDetails on DS.DirectShipmentId equals DSD.DirectShipmentId
                              where DS.DirectShipmentId == dbDetail.DirectShipmentId
                              select new
                              {
                                  DirectShipmetDetailId = 0,
                                  PiecesContent = DSD.PiecesContent,
                                  CartoonValue = DSD.CartoonValue,
                                  Height = DSD.Height,
                                  Length = DSD.Length,
                                  Width = DSD.Width,
                                  Weight = DSD.Weight,
                                  DeclaredValue = DSD.DeclaredValue
                              }).ToList();

                if (result != null && result.Count > 0)
                {
                    foreach (var Obj in result)
                    {
                        dbPackage = new Package();
                        dbPackage.Content = Obj.PiecesContent;
                        dbPackage.DirectShipmentDetailId = Obj.DirectShipmetDetailId;
                        dbPackage.CartoonValue = Obj.CartoonValue;
                        dbPackage.Height = Obj.Height;
                        dbPackage.Length = Obj.Length;
                        if (Obj.DeclaredValue.HasValue)
                        {
                            dbPackage.Value = Obj.DeclaredValue.Value;
                        }
                        dbPackage.Weight = Obj.Weight;
                        dbPackage.Width = Obj.Width;
                        dbDetail.Packages.Add(dbPackage);
                    }
                }
                else
                {
                    dbPackage = new Package();
                    dbPackage.Content = "";
                    dbPackage.DirectShipmentDetailId = 0;
                    dbPackage.PackageTrackingDetailId = 0;
                    dbPackage.CartoonValue = 0;
                    dbPackage.Height = 0;
                    dbPackage.Length = 0;
                    dbPackage.Value = 0;
                    dbPackage.Weight = 0;
                    dbPackage.Width = 0;
                    dbPackage.IsPrinted = false;
                    dbPackage.TrackingNo = "";
                    dbPackage.LabelName = "";
                    dbDetail.Packages.Add(dbPackage);
                }
            }
            else if (CallingType == FrayteCallingType.ShipmentDraft)
            {
                dbDetail.CustomerRateCard = null;

                var result = (from DS in dbContext.DirectShipments
                              join DSD in dbContext.DirectShipmentDetails on DS.DirectShipmentId equals DSD.DirectShipmentId
                              where DS.DirectShipmentId == dbDetail.DirectShipmentId
                              select new
                              {
                                  DirectShipmetDetailId = DSD.DirectShipmentDetailId,
                                  PiecesContent = DSD.PiecesContent,
                                  CartoonValue = DSD.CartoonValue,
                                  Height = DSD.Height,
                                  Length = DSD.Length,
                                  Width = DSD.Width,
                                  Weight = DSD.Weight,
                                  DeclaredValue = DSD.DeclaredValue
                              }).ToList();

                if (result != null && result.Count > 0)
                {
                    foreach (var Obj in result)
                    {
                        dbPackage = new Package();
                        dbPackage.Content = Obj.PiecesContent;
                        dbPackage.DirectShipmentDetailId = Obj.DirectShipmetDetailId;
                        dbPackage.CartoonValue = Obj.CartoonValue;
                        dbPackage.Height = Obj.Height;
                        dbPackage.Length = Obj.Length;
                        if (Obj.DeclaredValue.HasValue)
                        {
                            dbPackage.Value = Obj.DeclaredValue.Value;
                        }
                        dbPackage.Weight = Obj.Weight;
                        dbPackage.Width = Obj.Width;
                        dbDetail.Packages.Add(dbPackage);
                    }
                }
                else
                {
                    dbPackage = new Package();
                    dbPackage.Content = "";
                    dbPackage.DirectShipmentDetailId = 0;
                    dbPackage.PackageTrackingDetailId = 0;
                    dbPackage.CartoonValue = 0;
                    dbPackage.Height = 0;
                    dbPackage.Length = 0;
                    dbPackage.Value = 0;
                    dbPackage.Weight = 0;
                    dbPackage.Width = 0;
                    dbPackage.IsPrinted = false;
                    dbPackage.TrackingNo = "";
                    dbPackage.LabelName = "";
                    dbDetail.Packages.Add(dbPackage);
                }
            }
            else if (CallingType == "" || CallingType == null)
            {
                FrayteLogicalPhysicalPath path = new DirectShipmentRepository().GetShipmentLogisticlabelPath(dbDetail.DirectShipmentId);
                string pdfFileName = string.Empty;
                string physycalpath = string.Empty;

                if (path != null)
                {
                    if (path.PhysicalPath.Contains("~"))
                    {
                        // For developement
                        pdfFileName = AppSettings.WebApiPath + "PackageLabel/" + dbDetail.DirectShipmentId + "/";
                        physycalpath = HttpContext.Current.Server.MapPath(path.PhysicalPath + "PackageLabel/" + dbDetail.DirectShipmentId + "/");
                    }
                    else
                    {
                        pdfFileName = path.LogicalPath + "PackageLabel/" + dbDetail.DirectShipmentId + "/";
                        physycalpath = path.PhysicalPath + "/PackageLabel/" + dbDetail.DirectShipmentId + "/";
                    }
                }
                else
                {
                    pdfFileName = AppSettings.WebApiPath + "PackageLabel/" + dbDetail.DirectShipmentId + "/";
                    physycalpath = HttpContext.Current.Server.MapPath("~/PackageLabel/" + dbDetail.DirectShipmentId + "/");
                }

                var result = (from DS in dbContext.DirectShipments
                              join DSD in dbContext.DirectShipmentDetails on DS.DirectShipmentId equals DSD.DirectShipmentId
                              join PTD in dbContext.PackageTrackingDetails on DSD.DirectShipmentDetailId equals PTD.DirectShipmentDetailId
                              join CC in dbContext.LogisticServiceCourierAccounts on DS.CourierAccountId equals CC.LogisticServiceCourierAccountId
                              join LS in dbContext.LogisticServices on CC.LogisticServiceId equals LS.LogisticServiceId
                              where DS.DirectShipmentId == dbDetail.DirectShipmentId
                              select new
                              {
                                  PackageTrackingDetailId = PTD.PackageTrackingDetailId,
                                  DirectShipmentDetailId = PTD.DirectShipmentDetailId,
                                  IsPrinted = PTD.IsPrinted,
                                  PackageImage = PTD.PackageImage.Replace(".jpg", ".pdf"),
                                  PiecesContent = DSD.PiecesContent,
                                  CartoonValue = DSD.CartoonValue,
                                  Height = DSD.Height,
                                  Length = DSD.Length,
                                  Width = DSD.Width,
                                  Weight = DSD.Weight,
                                  DeclaredValue = DSD.DeclaredValue,
                                  CourierName = LS.LogisticCompany,
                                  UKDHLTrackingNo = DS.TrackingDetail,
                                  EasyPostTrackingNo = PTD.TrackingNo
                              }).ToList();

                if (result != null && result.Count > 0)
                {
                    foreach (var Obj in result)
                    {
                        if (Obj.CourierName == FrayteCourierCompany.UKMail || Obj.CourierName == FrayteCourierCompany.Yodel || Obj.CourierName == FrayteCourierCompany.Hermes || Obj.CourierName == FrayteCourierCompany.TNT)
                        {
                            dbPackage = new Package();
                            dbPackage.Content = Obj.PiecesContent;
                            dbPackage.DirectShipmentDetailId = Obj.DirectShipmentDetailId;
                            dbPackage.PackageTrackingDetailId = Obj.PackageTrackingDetailId;
                            dbPackage.CartoonValue = Obj.CartoonValue / Obj.CartoonValue;
                            dbPackage.Height = Obj.Height;
                            dbPackage.Length = Obj.Length;
                            if (Obj.DeclaredValue.HasValue)
                            {
                                dbPackage.Value = Obj.DeclaredValue.Value;
                            }
                            dbPackage.Weight = Obj.Weight;
                            dbPackage.Width = Obj.Width;
                            dbPackage.IsPrinted = Obj.IsPrinted;
                            dbPackage.TrackingNo = Obj.UKDHLTrackingNo;
                            dbPackage.LabelName = pdfFileName + Obj.PackageImage;
                            dbPackage.Label = Obj.PackageImage;
                            dbDetail.Packages.Add(dbPackage);
                        }
                        else
                        {
                            dbPackage = new Package();
                            dbPackage.Content = Obj.PiecesContent;
                            dbPackage.DirectShipmentDetailId = Obj.DirectShipmentDetailId;
                            dbPackage.PackageTrackingDetailId = Obj.PackageTrackingDetailId;
                            dbPackage.CartoonValue = Obj.CartoonValue / Obj.CartoonValue;
                            dbPackage.Height = Obj.Height;
                            dbPackage.Length = Obj.Length;
                            if (Obj.DeclaredValue.HasValue)
                            {
                                dbPackage.Value = Obj.DeclaredValue.Value;
                            }
                            dbPackage.Weight = Obj.Weight;
                            dbPackage.Width = Obj.Width;
                            dbPackage.IsPrinted = Obj.IsPrinted;
                            dbPackage.TrackingNo = Obj.EasyPostTrackingNo;
                            dbPackage.LabelName = pdfFileName + Obj.PackageImage;
                            dbPackage.Label = Obj.PackageImage;
                            dbDetail.Packages.Add(dbPackage);
                        }
                    }
                }
                else
                {
                    dbPackage = new Package();
                    dbPackage.Content = "";
                    dbPackage.DirectShipmentDetailId = 0;
                    dbPackage.PackageTrackingDetailId = 0;
                    dbPackage.CartoonValue = 0;
                    dbPackage.Height = 0;
                    dbPackage.Length = 0;
                    dbPackage.Value = 0;
                    dbPackage.Weight = 0;
                    dbPackage.Width = 0;
                    dbPackage.IsPrinted = false;
                    dbPackage.TrackingNo = "";
                    dbPackage.LabelName = "";
                    dbDetail.Packages.Add(dbPackage);
                }
            }
        }

        private void GetDirectShipmnetDetail(DirectBookingShipmentDetail dbDetail)
        {
            DirectShipment result = dbContext.DirectShipments.Find(dbDetail.DirectShipmentId);
            if (result != null)
            {
                var info = (from ua in dbContext.UserAdditionals
                            where ua.UserId == result.CustomerId
                            select ua).FirstOrDefault();

                var createdByInfo = (from r in dbContext.Users
                                     join ua in dbContext.UserAddresses on r.UserId equals ua.UserId
                                     where r.UserId == result.CreatedBy
                                     select new
                                     {
                                         CountryId = ua.CountryId
                                     }).FirstOrDefault();

                var ShipFromDetail = dbContext.DirectShipmentAddresses.Where(a => a.DirectShipmentAddressId == result.FromAddressId).FirstOrDefault();
                var TimeZone = new DirectShipmentRepository().TimeZoneDetail(createdByInfo.CountryId);
                var TimeZoneInformation = TimeZoneInfo.FindSystemTimeZoneById(TimeZone.Name);

                if (TimeZone != null)
                {
                    dbDetail.TimeZone = new Models.TimeZoneModal();
                    dbDetail.TimeZone.Name = TimeZone.Name;
                    dbDetail.TimeZone.Offset = TimeZone.Offset;
                    dbDetail.TimeZone.OffsetShort = TimeZone.OffsetShort;
                    dbDetail.TimeZone.TimezoneId = TimeZone.TimezoneId;
                }

                if (info.UserType == FrayteCustomerTypeEnum.SPECIAL)
                {
                    var createinfo = (from ua in dbContext.UserAdditionals
                                      where ua.UserId == result.CreatedBy
                                      select ua).FirstOrDefault();

                    dbDetail.IsRateShow = (createinfo.IsAllowRate.HasValue ? createinfo.IsAllowRate.Value : false);
                }
                else
                {
                    dbDetail.IsRateShow = (info.IsAllowRate.HasValue ? info.IsAllowRate.Value : false);
                }
                dbDetail.OpearionZoneId = result.OpearionZoneId;
                dbDetail.CustomerId = result.CustomerId;
                dbDetail.ShipmentStatusId = result.ShipmentStatusId;
                dbDetail.LogisticCompany = result.LogisticServiceType;
                dbDetail.Currency = new CurrencyType();

                var dbCurrency = dbContext.CurrencyTypes.Find(result.CurrencyCode);
                if (dbCurrency != null)
                {
                    dbDetail.Currency.CurrencyCode = dbCurrency.CurrencyCode;
                    dbDetail.Currency.CurrencyDescription = dbCurrency.CurrencyDescription;
                }

                if (result.CourierAccountId > 0)
                {
                    dbDetail.CustomerRateCard = new DirectBookingService();
                    dbDetail.CustomerRateCard.Weight = result.ChargeableWeight.HasValue ? result.ChargeableWeight.Value : 0.00m;
                    var shipmentTypeDetail = dbContext.LogisticServiceShipmentTypes.Find(result.ShipmentTypeId);
                    if (shipmentTypeDetail != null)
                    {
                        dbDetail.CustomerRateCard.WeightType = shipmentTypeDetail.LogisticDescriptionDisplayType;
                    }

                    var courierResult = (from ca in dbContext.LogisticServiceCourierAccounts
                                         join ls in dbContext.LogisticServices on ca.LogisticServiceId equals ls.LogisticServiceId
                                         join zrc in dbContext.LogisticServiceBaseRateCards on ca.LogisticServiceCourierAccountId equals zrc.LogisticServiceCourierAccountId into leftRate
                                         from lr in leftRate.DefaultIfEmpty()
                                         join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                         join lw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lw.LogisticServiceShipmentTypeId
                                         where ca.LogisticServiceCourierAccountId == result.CourierAccountId
                                         select new
                                         {
                                             LogisticServiceId = ls.LogisticServiceId,
                                             LogisticCompany = ls.LogisticCompany,
                                             LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                             LogisticType = ls.LogisticType,
                                             LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                             RateType = ls.RateType,
                                             RateTypeDisplay = ls.RateTypeDisplay,
                                             AccountNo = ca.AccountNo,
                                             AccountCountryCode = ca.AccountCountryCode,
                                             IntegrationAccountId = ca.IntegrationAccountId,
                                             LogisticRate = (lr == null ? 0.00m : lr.LogisticRate),
                                             LogisticCurrency = (lr == null ? "" : lr.LogisticCurrency),
                                             Description = ca.Description,
                                             UOM = lw.UOM
                                         }).FirstOrDefault();

                    if (courierResult != null)
                    {
                        dbDetail.CustomerRateCard.LogisticServiceId = courierResult.LogisticServiceId;
                        dbDetail.CustomerRateCard.CourierName = courierResult.LogisticCompany;
                        if (result.LogisticType == FrayteLogisticType.UKShipment && !string.IsNullOrEmpty(result.LogisticServiceType))
                        {
                            dbDetail.CustomerRateCard.DisplayName = courierResult.LogisticCompanyDisplay;
                        }
                        else
                        {
                            dbDetail.CustomerRateCard.DisplayName = courierResult.LogisticCompanyDisplay;
                        }
                        dbDetail.CustomerRateCard.LogisticType = courierResult.LogisticType;
                        dbDetail.CustomerRateCard.LogisticServiceType = courierResult.LogisticTypeDisplay;
                        dbDetail.CustomerRateCard.CourierAccountCountryCode = courierResult.AccountCountryCode;
                        dbDetail.CustomerRateCard.CurrencyCode = courierResult.LogisticCurrency;
                        dbDetail.CustomerRateCard.Rate = courierResult.LogisticRate;
                        dbDetail.CustomerRateCard.CourierAccountNo = courierResult.AccountNo;
                        dbDetail.CustomerRateCard.CourierDescription = courierResult.Description;
                        dbDetail.CustomerRateCard.UnitOfMeasurement = courierResult.UOM;
                        dbDetail.CustomerRateCard.RateType = courierResult.RateType;
                        dbDetail.CustomerRateCard.RateTypeDisplay = courierResult.RateTypeDisplay;
                        var Currency = dbContext.UserAdditionals.Find(result.CustomerId);
                        if (Currency != null)
                        {
                            dbDetail.CustomerRateCard.CustomerCurrency = Currency.CreditLimitCurrencyCode;
                        }
                    }
                }

                dbDetail.CustomInfo = new CustomInformation();
                dbDetail.CustomInfo.ShipmentId = result.DirectShipmentId;

                dbDetail.ParcelType = new FrayteParcelType();
                dbDetail.ParcelType.ParcelType = result.ParcelType;
                dbDetail.ParcelType.ParcelDescription = result.ParcelType;

                dbDetail.PaymentPartyAccountNumber = result.PaymentPartyTaxAndDutiesAccountNo;
                dbDetail.PayTaxAndDuties = result.PaymentPartyTaxAndDuties;
                dbDetail.ReferenceDetail = new ReferenceDetail();
                if (result.CollectionDate != null && result.CollectionTime != null)
                {
                    dbDetail.ReferenceDetail.CollectionDate = UtilityRepository.UtcDateToOtherTimezone(Convert.ToDateTime(result.CollectionDate), result.CollectionTime.Value, TimeZoneInformation).Item1;
                    dbDetail.ReferenceDetail.CollectionTime = UtilityRepository.UtcDateToOtherTimezone(Convert.ToDateTime(result.CollectionDate), result.CollectionTime.Value, TimeZoneInformation).Item2 + " " + dbDetail.TimeZone.OffsetShort;
                }
                else
                {
                    //dbDetail.ReferenceDetail.CollectionDate = DateTime.UtcNow;
                    //dbDetail.ReferenceDetail.CollectionTime = "12:00:00";
                }
                dbDetail.ReferenceDetail.ContentDescription = result.ContentDescription;
                dbDetail.ReferenceDetail.Reference1 = result.Reference1;
                dbDetail.ReferenceDetail.Reference2 = result.Reference2;
                dbDetail.ReferenceDetail.SpecialInstruction = result.SpecialInstruction;

                dbDetail.ShipFrom = new DirectBookingCollection();
                dbDetail.ShipFrom.DirectShipmentAddressId = result.FromAddressId;

                dbDetail.ShipTo = new DirectBookingCollection();
                dbDetail.ShipTo.DirectShipmentAddressId = result.ToAddressId;
                dbDetail.CreatedOn = result.CreatedOn;

                var data = (from r in dbContext.Users
                            join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                            where r.UserId == result.CreatedBy
                            select new
                            {
                                CreatedByUserId = r.UserId,
                                CompanyName = r.CompanyName,
                                ContactName = r.ContactName,
                                CreatedByRoleId = ur.RoleId
                            }
                           ).FirstOrDefault();

                if (data != null)
                {
                    dbDetail.CreatedBy = ((data.CompanyName == "" || data.CompanyName == null) ? data.ContactName : data.ContactName + "-" + data.CompanyName);
                    dbDetail.CreatedByRoleId = data.CreatedByRoleId;
                    dbDetail.CreatedByUserId = data.CreatedByUserId;
                }

                dbDetail.BaseRate = decimal.Parse((result.BaseRate == null ? 0.0m : result.BaseRate.Value).ToString("N", new System.Globalization.CultureInfo("en-US")));
                dbDetail.MarginCost = decimal.Parse((result.Margin == null ? 0.0m : result.Margin.Value).ToString("N", new System.Globalization.CultureInfo("en-US")));
                if (result.FuelMonthYear.HasValue)
                {
                    dbDetail.FuelMonth = result.FuelMonthYear.Value;
                }
                if (result.FuelSurchargePercent.HasValue)
                {
                    dbDetail.FuelPercent = (float)result.FuelSurchargePercent.Value;
                }
                dbDetail.AdditionalSurcharge = float.Parse((Math.Round(result.AdditionalSurcharge == null ? 0.0m : result.AdditionalSurcharge.Value, 2)).ToString("N", new System.Globalization.CultureInfo("en-US")));
                dbDetail.FuelSurCharge = float.Parse((Math.Round(result.FuelSurCharge == null ? 0.0m : result.FuelSurCharge.Value, 2)).ToString("N", new System.Globalization.CultureInfo("en-US")));
                dbDetail.EstimatedCost = float.Parse((Math.Round(dbDetail.BaseRate + dbDetail.MarginCost, 2)).ToString("N", new System.Globalization.CultureInfo("en-US")));
                dbDetail.EstimatedTotalCost = float.Parse((Math.Round(dbDetail.EstimatedCost + dbDetail.FuelSurCharge + dbDetail.AdditionalSurcharge, 2)).ToString("N", new System.Globalization.CultureInfo("en-US")));
                dbDetail.EstimatedWeight = result.ChargeableWeight.HasValue ? result.ChargeableWeight.Value : 0.00M;

                if (dbDetail.CustomerRateCard != null)
                {
                    dbDetail.CustomerRateCard.FuelSurcharge = float.Parse((Math.Round(result.FuelSurCharge == null ? 0.0m : result.FuelSurCharge.Value, 2)).ToString("N", new System.Globalization.CultureInfo("en-US")));
                }
                if (dbDetail.CustomerRateCard != null)
                {
                    dbDetail.CustomerRateCard.Rate = decimal.Parse((dbDetail.BaseRate + dbDetail.MarginCost).ToString("N", new System.Globalization.CultureInfo("en-US")));
                }
                if (dbDetail.CustomerRateCard != null)
                {
                    dbDetail.CustomerRateCard.AdditionalSurcharge = float.Parse((dbDetail.AdditionalSurcharge).ToString("N", new System.Globalization.CultureInfo("en-US")));
                }
                if (dbDetail.CustomerRateCard != null)
                {
                    dbDetail.CustomerRateCard.TotalEstimatedCharge = float.Parse((Math.Round(dbDetail.EstimatedCost + dbDetail.FuelSurCharge + dbDetail.AdditionalSurcharge, 2)).ToString("N", new System.Globalization.CultureInfo("en-US")));
                }
                if (!string.IsNullOrEmpty(result.PackageCaculatonType))
                {
                    dbDetail.PakageCalculatonType = result.PackageCaculatonType;
                }

                dbDetail.DeliveryDate = result.DeliveryDate;
                dbDetail.DeliveryTime = result.DeliveryTime;
                dbDetail.SignedBy = result.SignedBy;
                dbDetail.TaxAndDutiesAcceptedBy = result.TaxAndDutiesAcceptedBy;
                dbDetail.FrayteNumber = result.FrayteNumber;
                dbDetail.ShipmentStatus = dbContext.ShipmentStatus.Where(p => p.ShipmentStatusId == result.ShipmentStatusId).Select(p => p.DisplayStatusName).FirstOrDefault();
                if (result.TrackingDetail.Contains("Order_"))
                {
                    dbDetail.TrackingNo = result.TrackingDetail.Replace("Order_", "");
                }
                else
                {
                    dbDetail.TrackingNo = result.TrackingDetail;
                }
            }
        }

        public void GetDirectShipmnetDraftDetail(DirectBookingShipmentDraftDetail dbDetail, string CallingType)
        {
            if (CallingType == FrayteCallingType.ShipmentClone || CallingType == FrayteCallingType.ShipmentReturn)
            {
                DirectShipment result = dbContext.DirectShipments.Find(dbDetail.DirectShipmentDraftId);
                var TimeZone = new TimeZoneModal();
                if (result != null)
                {
                    var fromCounry = dbContext.DirectShipmentAddresses.FirstOrDefault(k => k.DirectShipmentAddressId == result.FromAddressId);
                    TimeZone = TimeZoneDetail(fromCounry.CountryId);
                }

                var TimeZoneInformation = TimeZoneInfo.FindSystemTimeZoneById(TimeZone.Name);

                if (result != null)
                {
                    dbDetail.OpearionZoneId = result.OpearionZoneId;
                    dbDetail.CustomerId = result.CustomerId;
                    dbDetail.ShipmentStatusId = result.ShipmentStatusId;
                    dbDetail.SessionId = result.SessionId != null ? result.SessionId.Value : 0;
                    dbDetail.Currency = new CurrencyType();

                    var dbCurrency = dbContext.CurrencyTypes.Find(result.CurrencyCode);
                    if (dbCurrency != null)
                    {
                        dbDetail.Currency.CurrencyCode = dbCurrency.CurrencyCode;
                        dbDetail.Currency.CurrencyDescription = dbCurrency.CurrencyDescription;
                    }

                    if (result.CourierAccountId > 0)
                    {
                        dbDetail.CustomerRateCard = new DirectBookingService();
                        dbDetail.CustomerRateCard.Weight = result.ChargeableWeight.HasValue ? result.ChargeableWeight.Value : 0.00m;
                        var shipmentTypeDetail = dbContext.LogisticServiceShipmentTypes.Find(result.ShipmentTypeId);
                        if (shipmentTypeDetail != null)
                        {
                            dbDetail.CustomerRateCard.WeightType = shipmentTypeDetail.LogisticDescriptionDisplayType;
                        }

                        var courierResult = (from ca in dbContext.LogisticServiceCourierAccounts
                                             join ls in dbContext.LogisticServices on ca.LogisticServiceId equals ls.LogisticServiceId
                                             join zrc in dbContext.LogisticServiceBaseRateCards on ca.LogisticServiceCourierAccountId equals zrc.LogisticServiceCourierAccountId into leftRate
                                             from lr in leftRate.DefaultIfEmpty()
                                             join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                             join lw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lw.LogisticServiceShipmentTypeId
                                             where ca.LogisticServiceCourierAccountId == result.CourierAccountId
                                             select new
                                             {
                                                 LogisticServiceId = ls.LogisticServiceId,
                                                 LogisticCompany = ls.LogisticCompany,
                                                 LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                 LogisticType = ls.LogisticType,
                                                 LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                 RateType = ls.RateType,
                                                 RateTypeDisplay = ls.RateTypeDisplay,
                                                 AccountNo = ca.AccountNo,
                                                 AccountCountryCode = ca.AccountCountryCode,
                                                 IntegrationAccountId = ca.IntegrationAccountId,
                                                 LogisticRate = (lr == null ? 0.00m : lr.LogisticRate),
                                                 LogisticCurrency = (lr == null ? "" : lr.LogisticCurrency),
                                                 Description = ca.Description,
                                                 UOM = lw.UOM
                                             }).FirstOrDefault();

                        if (courierResult != null)
                        {
                            dbDetail.CustomerRateCard.CourierId = 0;
                            dbDetail.CustomerRateCard.CourierName = courierResult.LogisticCompany;
                            if (result.LogisticType == FrayteLogisticType.UKShipment || result.LogisticType == FrayteLogisticType.EUImport || result.LogisticType == FrayteLogisticType.EUImport)
                            {
                                dbDetail.CustomerRateCard.DisplayName = courierResult.LogisticCompanyDisplay;
                            }
                            else
                            {
                                dbDetail.CustomerRateCard.DisplayName = courierResult.LogisticCompanyDisplay;
                            }
                            dbDetail.CustomerRateCard.LogisticType = courierResult.LogisticType;
                            dbDetail.CustomerRateCard.LogisticServiceType = courierResult.LogisticTypeDisplay;
                            dbDetail.CustomerRateCard.CourierAccountCountryCode = courierResult.AccountCountryCode;
                            dbDetail.CustomerRateCard.CurrencyCode = courierResult.LogisticCurrency;
                            dbDetail.CustomerRateCard.Rate = courierResult.LogisticRate;
                            dbDetail.CustomerRateCard.CourierAccountNo = courierResult.AccountNo;
                            dbDetail.CustomerRateCard.CourierDescription = courierResult.Description;
                            dbDetail.CustomerRateCard.UnitOfMeasurement = courierResult.UOM;
                            dbDetail.CustomerRateCard.RateType = courierResult.RateType;
                            dbDetail.CustomerRateCard.RateTypeDisplay = courierResult.RateTypeDisplay;
                            var Currency = dbContext.UserAdditionals.Find(result.CustomerId);
                            if (Currency != null)
                            {
                                dbDetail.CustomerRateCard.CustomerCurrency = Currency.CreditLimitCurrencyCode;
                            }
                        }
                    }

                    dbDetail.CustomInfo = new CustomInformation();
                    dbDetail.CustomInfo.ShipmentId = result.DirectShipmentId;

                    dbDetail.ParcelType = new FrayteParcelType();
                    dbDetail.ParcelType.ParcelType = result.ParcelType;
                    dbDetail.ParcelType.ParcelDescription = result.ParcelType;

                    dbDetail.PaymentPartyAccountNumber = result.PaymentPartyTaxAndDutiesAccountNo;
                    dbDetail.PayTaxAndDuties = result.PaymentPartyTaxAndDuties;
                    dbDetail.ReferenceDetail = new ReferenceDetail();
                    if (result.CollectionTime.HasValue)
                    {
                        dbDetail.ReferenceDetail.CollectionDate = DateTime.UtcNow;
                        dbDetail.ReferenceDetail.CollectionTime = UtilityRepository.Get24HourFormatedTime(UtilityRepository.UtcDateToOtherTimezone(Convert.ToDateTime(result.CollectionDate), result.CollectionTime.Value, TimeZoneInformation).Item2);
                    }
                    else
                    {
                        dbDetail.ReferenceDetail.CollectionDate = DateTime.UtcNow;
                        dbDetail.ReferenceDetail.CollectionTime = "12:00:00";
                    }
                    dbDetail.ReferenceDetail.ContentDescription = result.ContentDescription;
                    dbDetail.ReferenceDetail.Reference1 = result.Reference1;
                    dbDetail.ReferenceDetail.Reference2 = result.Reference2;
                    dbDetail.ReferenceDetail.SpecialInstruction = result.SpecialInstruction;

                    dbDetail.ShipFrom = new DirectBookingDraftCollection();
                    dbDetail.ShipFrom.AddressBookId = result.FromAddressId;

                    dbDetail.ShipTo = new DirectBookingDraftCollection();
                    dbDetail.ShipTo.AddressBookId = result.ToAddressId;

                    dbDetail.BaseRate = result.BaseRate == null ? 0.0m : result.BaseRate.Value;
                    dbDetail.MarginCost = result.Margin == null ? 0.0m : result.Margin.Value;
                    if (result.FuelMonthYear.HasValue)
                    {
                        dbDetail.FuelMonth = result.FuelMonthYear.Value;
                    }
                    if (result.FuelSurchargePercent.HasValue)
                    {
                        dbDetail.FuelPercent = (float)result.FuelSurchargePercent.Value;
                    }
                    dbDetail.AdditionalSurcharge = (float)Math.Round(result.AdditionalSurcharge == null ? 0.0m : result.AdditionalSurcharge.Value, 2);
                    dbDetail.FuelSurCharge = (float)Math.Round(result.FuelSurCharge == null ? 0.0m : result.FuelSurCharge.Value, 2);
                    dbDetail.EstimatedCost = (float)Math.Round(dbDetail.BaseRate + dbDetail.MarginCost, 2);
                    dbDetail.EstimatedTotalCost = (float)Math.Round(dbDetail.EstimatedCost + dbDetail.FuelSurCharge + dbDetail.AdditionalSurcharge, 2);
                    if (dbDetail.CustomerRateCard != null)
                    {
                        dbDetail.CustomerRateCard.FuelSurcharge = (float)Math.Round(result.FuelSurCharge == null ? 0.0m : result.FuelSurCharge.Value, 2);
                    }
                    if (dbDetail.CustomerRateCard != null)
                    {
                        dbDetail.CustomerRateCard.Rate = dbDetail.BaseRate + dbDetail.MarginCost;
                    }
                    if (dbDetail.CustomerRateCard != null)
                    {
                        dbDetail.CustomerRateCard.AdditionalSurcharge = dbDetail.AdditionalSurcharge;
                    }
                    if (dbDetail.CustomerRateCard != null)
                    {
                        dbDetail.CustomerRateCard.TotalEstimatedCharge = (float)Math.Round(dbDetail.EstimatedCost + dbDetail.FuelSurCharge + dbDetail.AdditionalSurcharge, 2);
                    }
                    if (!string.IsNullOrEmpty(result.PackageCaculatonType))
                    {
                        dbDetail.PakageCalculatonType = result.PackageCaculatonType;
                    }
                    dbDetail.CreatedBy = result.CreatedBy;
                    dbDetail.TaxAndDutiesAcceptedBy = result.TaxAndDutiesAcceptedBy;
                    dbDetail.AddressType = result.AddressType;
                }
            }
            else
            {
                DirectShipmentDraft result = dbContext.DirectShipmentDrafts.Find(dbDetail.DirectShipmentDraftId);
                var TimeZone = new TimeZoneModal();
                if (result != null)
                {
                    var fromCounry = dbContext.AddressBooks.FirstOrDefault(k => k.AddressBookId == result.FromAddressId);
                    TimeZone = TimeZoneDetail(fromCounry.CountryId);
                }
                var TimeZoneInformation = TimeZoneInfo.FindSystemTimeZoneById(TimeZone.Name);
                if (result != null)
                {
                    dbDetail.OpearionZoneId = result.OpearionZoneId.Value;
                    dbDetail.CustomerId = result.CustomerId;
                    dbDetail.ShipmentStatusId = result.ShipmentStatusId.Value;
                    dbDetail.SessionId = result.SessionId != null ? result.SessionId.Value : 0;
                    dbDetail.Currency = new CurrencyType();

                    var dbCurrency = dbContext.CurrencyTypes.Find(result.CurrencyCode);
                    if (dbCurrency != null)
                    {
                        dbDetail.Currency.CurrencyCode = dbCurrency.CurrencyCode;
                        dbDetail.Currency.CurrencyDescription = dbCurrency.CurrencyDescription;
                    }

                    if (result.CourierAccountId > 0)
                    {
                        dbDetail.CustomerRateCard = new DirectBookingService();
                        dbDetail.CustomerRateCard.Weight = result.ChargeableWeight.HasValue ? result.ChargeableWeight.Value : 0.00m;
                        var shipmentTypeDetail = dbContext.LogisticServiceShipmentTypes.Find(result.ShipmentTypeId);
                        if (shipmentTypeDetail != null)
                        {
                            dbDetail.CustomerRateCard.WeightType = shipmentTypeDetail.LogisticDescriptionDisplayType;
                        }

                        var courierResult = (from ca in dbContext.LogisticServiceCourierAccounts
                                             join ls in dbContext.LogisticServices on ca.LogisticServiceId equals ls.LogisticServiceId
                                             join zrc in dbContext.LogisticServiceBaseRateCards on ca.LogisticServiceCourierAccountId equals zrc.LogisticServiceCourierAccountId into leftRate
                                             from lr in leftRate.DefaultIfEmpty()
                                             join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                             join lw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lw.LogisticServiceShipmentTypeId
                                             where ca.LogisticServiceCourierAccountId == result.CourierAccountId
                                             select new
                                             {
                                                 LogisticServiceId = ls.LogisticServiceId,
                                                 LogisticCompany = ls.LogisticCompany,
                                                 LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                 LogisticType = ls.LogisticType,
                                                 LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                 RateType = ls.RateType,
                                                 RateTypeDisplay = ls.RateTypeDisplay,
                                                 AccountNo = ca.AccountNo,
                                                 AccountCountryCode = ca.AccountCountryCode,
                                                 IntegrationAccountId = ca.IntegrationAccountId,
                                                 LogisticRate = (lr == null ? 0.00m : lr.LogisticRate),
                                                 LogisticCurrency = (lr == null ? "" : lr.LogisticCurrency),
                                                 Description = ca.Description,
                                                 UOM = lw.UOM
                                             }).FirstOrDefault();

                        if (courierResult != null)
                        {
                            dbDetail.CustomerRateCard.CourierId = 0;
                            dbDetail.CustomerRateCard.CourierName = courierResult.LogisticCompany;
                            if (result.LogisticType == FrayteLogisticType.UKShipment || result.LogisticType == FrayteLogisticType.EUImport || result.LogisticType == FrayteLogisticType.EUImport)
                            {
                                dbDetail.CustomerRateCard.DisplayName = courierResult.LogisticCompanyDisplay;
                            }
                            else
                            {
                                dbDetail.CustomerRateCard.DisplayName = courierResult.LogisticCompanyDisplay;
                            }
                            dbDetail.CustomerRateCard.LogisticType = courierResult.LogisticType;
                            dbDetail.CustomerRateCard.LogisticServiceType = courierResult.LogisticTypeDisplay;
                            dbDetail.CustomerRateCard.CourierAccountCountryCode = courierResult.AccountCountryCode;
                            dbDetail.CustomerRateCard.CurrencyCode = courierResult.LogisticCurrency;
                            dbDetail.CustomerRateCard.Rate = courierResult.LogisticRate;
                            dbDetail.CustomerRateCard.CourierAccountNo = courierResult.AccountNo;
                            dbDetail.CustomerRateCard.CourierDescription = courierResult.Description;
                            dbDetail.CustomerRateCard.UnitOfMeasurement = courierResult.UOM;
                            dbDetail.CustomerRateCard.RateType = courierResult.RateType;
                            dbDetail.CustomerRateCard.RateTypeDisplay = courierResult.RateTypeDisplay;
                            var Currency = dbContext.UserAdditionals.Find(result.CustomerId);
                            if (Currency != null)
                            {
                                dbDetail.CustomerRateCard.CustomerCurrency = Currency.CreditLimitCurrencyCode;
                            }
                        }
                    }

                    dbDetail.CustomInfo = new CustomInformation();
                    dbDetail.CustomInfo.ShipmentId = result.DirectShipmentDraftId;

                    dbDetail.ParcelType = new FrayteParcelType();
                    dbDetail.ParcelType.ParcelType = result.ParcelType;
                    dbDetail.ParcelType.ParcelDescription = result.ParcelType;

                    dbDetail.PaymentPartyAccountNumber = result.PaymentPartyTaxAndDutiesAccountNo;
                    dbDetail.PayTaxAndDuties = result.PaymentPartyTaxAndDuties;
                    dbDetail.ReferenceDetail = new ReferenceDetail();
                    if (result.CollectionTime.HasValue)
                    {
                        dbDetail.ReferenceDetail.CollectionDate = DateTime.UtcNow;
                        dbDetail.ReferenceDetail.CollectionTime = UtilityRepository.UtcDateToOtherTimezone(Convert.ToDateTime(result.CollectionDate), result.CollectionTime.Value, TimeZoneInformation).Item2;
                    }
                    else
                    {
                        dbDetail.ReferenceDetail.CollectionDate = DateTime.UtcNow;
                        dbDetail.ReferenceDetail.CollectionTime = "12:00:00";
                    }
                    dbDetail.ReferenceDetail.ContentDescription = result.ContentDescription;
                    dbDetail.ReferenceDetail.Reference1 = result.Reference1;
                    dbDetail.ReferenceDetail.SpecialInstruction = result.SpecialInstruction;

                    dbDetail.ShipFrom = new DirectBookingDraftCollection();
                    dbDetail.ShipFrom.AddressBookId = result.FromAddressId.Value;

                    dbDetail.ShipTo = new DirectBookingDraftCollection();
                    dbDetail.ShipTo.AddressBookId = result.ToAddressId.Value;

                    dbDetail.BaseRate = result.BaseRate == null ? 0.0m : result.BaseRate.Value;
                    dbDetail.MarginCost = result.Margin == null ? 0.0m : result.Margin.Value;
                    if (result.FuelMonthYear.HasValue)
                    {
                        dbDetail.FuelMonth = result.FuelMonthYear.Value;
                    }
                    if (result.FuelSurchargePercent.HasValue)
                    {
                        dbDetail.FuelPercent = (float)result.FuelSurchargePercent.Value;
                    }
                    dbDetail.AdditionalSurcharge = (float)Math.Round(result.AdditionalSurcharge == null ? 0.0m : result.AdditionalSurcharge.Value, 2);
                    dbDetail.FuelSurCharge = (float)Math.Round(result.FuelSurCharge == null ? 0.0m : result.FuelSurCharge.Value, 2);
                    dbDetail.EstimatedCost = (float)Math.Round(dbDetail.BaseRate + dbDetail.MarginCost, 2);
                    dbDetail.EstimatedTotalCost = (float)Math.Round(dbDetail.EstimatedCost + dbDetail.FuelSurCharge + dbDetail.AdditionalSurcharge, 2);
                    if (dbDetail.CustomerRateCard != null)
                    {
                        dbDetail.CustomerRateCard.FuelSurcharge = (float)Math.Round(result.FuelSurCharge == null ? 0.0m : result.FuelSurCharge.Value, 2);
                    }
                    if (dbDetail.CustomerRateCard != null)
                    {
                        dbDetail.CustomerRateCard.Rate = dbDetail.BaseRate + dbDetail.MarginCost;
                    }
                    if (dbDetail.CustomerRateCard != null)
                    {
                        dbDetail.CustomerRateCard.AdditionalSurcharge = dbDetail.AdditionalSurcharge;
                    }
                    if (dbDetail.CustomerRateCard != null)
                    {
                        dbDetail.CustomerRateCard.TotalEstimatedCharge = (float)Math.Round(dbDetail.EstimatedCost + dbDetail.FuelSurCharge + dbDetail.AdditionalSurcharge, 2);
                    }
                    if (!string.IsNullOrEmpty(result.PackageCaculatonType))
                    {
                        dbDetail.PakageCalculatonType = result.PackageCaculatonType;
                    }
                    if (result.CreatedBy.HasValue)
                    {
                        dbDetail.CreatedBy = result.CreatedBy.Value;
                    }
                    dbDetail.TaxAndDutiesAcceptedBy = result.TaxAndDutiesAcceptedBy;
                    dbDetail.AddressType = result.AddressType;
                    dbDetail.IsPublic = false;
                }
            }
        }

        public void SaveLogisticLabel(int directShipmentId, string logisticLabel)
        {
            try
            {
                var shipment = dbContext.DirectShipments.Find(directShipmentId);
                shipment.LogisticLabel = logisticLabel;
                dbContext.Entry(shipment).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        private void GetQuotationShipmentDetail(DirectBookingShipmentDraftDetail dbDetail)
        {
            QuotationShipment result = dbContext.QuotationShipments.Find(dbDetail.DirectShipmentDraftId);

            if (result != null)
            {
                dbDetail.ShipmentStatusId = 14;
                dbDetail.ShippingMethodId = result.ShipmentTypeId.HasValue ? result.ShipmentTypeId.Value : 0;
                dbDetail.Currency = new CurrencyType();
                dbDetail.Currency.CurrencyCode = result.CurrencyCode;
                dbDetail.BaseRate = result.BaseRate.HasValue ? result.BaseRate.Value : 0.0m;
                dbDetail.OpearionZoneId = result.OpearionZoneId;
                dbDetail.MarginCost = result.Margin.HasValue ? result.Margin.Value : 0.0m;
                dbDetail.AdditionalSurcharge = (float)(result.AdditionalSurcharge.HasValue ? result.AdditionalSurcharge.Value : 0.0m);
                if (result.FuelMonthYear.HasValue)
                {
                    dbDetail.FuelMonth = result.FuelMonthYear.Value;
                }
                dbDetail.FuelPercent = (float)(result.FuelSurchargePercent.HasValue ? result.FuelSurchargePercent.Value : 0.0m);
                dbDetail.FuelSurCharge = (float)(result.FuelSurCharge.HasValue ? result.FuelSurCharge.Value : 0.0m);
                dbDetail.ReferenceDetail = new ReferenceDetail();
                dbDetail.ReferenceDetail.CollectionDate = DateTime.UtcNow;
                dbDetail.ReferenceDetail.CollectionTime = "1630";
                dbDetail.ReferenceDetail.ContentDescription = result.CourierDescription;
                dbDetail.ReferenceDetail.SpecialInstruction = "";
                dbDetail.CustomerId = result.CustomerId;
                dbDetail.OpearionZoneId = result.OpearionZoneId;
                dbDetail.ParcelType = new FrayteParcelType();
                dbDetail.ParcelType.ParcelType = result.ParcelType;
                dbDetail.ParcelType.ParcelDescription = result.ParcelServiceType;
                dbDetail.CreatedBy = result.CreatedBy;
                dbDetail.CustomInfo = new CustomInformation();
                dbDetail.CustomInfo.ContentsType = "";
                dbDetail.CustomInfo.ContentsExplanation = "";
                dbDetail.CustomInfo.RestrictionType = "";
                dbDetail.CustomInfo.RestrictionComments = "";
                dbDetail.CustomInfo.CustomsCertify = true;
                dbDetail.CustomInfo.CustomsSigner = "";
                dbDetail.CustomInfo.NonDeliveryOption = "";
                dbDetail.CustomInfo.EelPfc = "";
                dbDetail.CustomInfo.CatagoryOfItem = "";
                dbDetail.CustomInfo.CatagoryOfItemExplanation = "";
                dbDetail.CustomInfo.CommodityCode = "";
                dbDetail.CustomInfo.TermOfTrade = "";
                dbDetail.CustomInfo.ModuleType = "";
            }
        }

        public void GetDirectShipmentPackagesDraftDetail(DirectBookingShipmentDraftDetail dbDetail, string CallingType)
        {
            PackageDraft dbPackage;
            dbDetail.Packages = new List<PackageDraft>();
            if (CallingType == FrayteCallingType.ShipmentClone)
            {
                var result = (from DS in dbContext.DirectShipments
                              join DSD in dbContext.DirectShipmentDetails on DS.DirectShipmentId equals DSD.DirectShipmentId
                              where DS.DirectShipmentId == dbDetail.DirectShipmentDraftId
                              select new
                              {
                                  DirectShipmetDetailId = 0,
                                  PiecesContent = DSD.PiecesContent,
                                  CartoonValue = DSD.CartoonValue,
                                  Height = DSD.Height,
                                  Length = DSD.Length,
                                  Width = DSD.Width,
                                  Weight = DSD.Weight,
                                  DeclaredValue = DSD.DeclaredValue
                              }).ToList();

                if (result != null && result.Count > 0)
                {
                    foreach (var Obj in result)
                    {
                        dbPackage = new PackageDraft();
                        dbPackage.Content = Obj.PiecesContent;
                        dbPackage.DirectShipmentDetailDraftId = Obj.DirectShipmetDetailId;
                        dbPackage.CartoonValue = Obj.CartoonValue;
                        dbPackage.Height = Obj.Height;
                        dbPackage.Length = Obj.Length;
                        if (Obj.DeclaredValue.HasValue)
                        {
                            dbPackage.Value = Obj.DeclaredValue.Value;
                        }
                        dbPackage.Weight = Obj.Weight;
                        dbPackage.Width = Obj.Width;
                        dbDetail.Packages.Add(dbPackage);
                    }
                }
                else
                {
                    dbPackage = new PackageDraft();
                    dbPackage.Content = "";
                    dbPackage.DirectShipmentDetailDraftId = 0;
                    dbPackage.PackageTrackingDetailId = 0;
                    dbPackage.CartoonValue = 0;
                    dbPackage.Height = 0;
                    dbPackage.Length = 0;
                    dbPackage.Value = 0;
                    dbPackage.Weight = 0;
                    dbPackage.Width = 0;
                    dbPackage.IsPrinted = false;
                    dbPackage.TrackingNo = "";
                    dbPackage.LabelName = "";
                    dbDetail.Packages.Add(dbPackage);
                }
            }
            else if (CallingType == FrayteCallingType.ShipmentReturn)
            {
                var result = (from DS in dbContext.DirectShipments
                              join DSD in dbContext.DirectShipmentDetails on DS.DirectShipmentId equals DSD.DirectShipmentId
                              where DS.DirectShipmentId == dbDetail.DirectShipmentDraftId
                              select new
                              {
                                  DirectShipmetDetailId = 0,
                                  PiecesContent = DSD.PiecesContent,
                                  CartoonValue = DSD.CartoonValue,
                                  Height = DSD.Height,
                                  Length = DSD.Length,
                                  Width = DSD.Width,
                                  Weight = DSD.Weight,
                                  DeclaredValue = DSD.DeclaredValue
                              }).ToList();

                if (result != null && result.Count > 0)
                {
                    foreach (var Obj in result)
                    {
                        dbPackage = new PackageDraft();
                        dbPackage.Content = Obj.PiecesContent;
                        dbPackage.DirectShipmentDetailDraftId = Obj.DirectShipmetDetailId;
                        dbPackage.CartoonValue = Obj.CartoonValue;
                        dbPackage.Height = Obj.Height;
                        dbPackage.Length = Obj.Length;
                        if (Obj.DeclaredValue.HasValue)
                        {
                            dbPackage.Value = Obj.DeclaredValue.Value;
                        }
                        dbPackage.Weight = Obj.Weight;
                        dbPackage.Width = Obj.Width;
                        dbDetail.Packages.Add(dbPackage);
                    }
                }
                else
                {
                    dbPackage = new PackageDraft();
                    dbPackage.Content = "";
                    dbPackage.DirectShipmentDetailDraftId = 0;
                    dbPackage.PackageTrackingDetailId = 0;
                    dbPackage.CartoonValue = 0;
                    dbPackage.Height = 0;
                    dbPackage.Length = 0;
                    dbPackage.Value = 0;
                    dbPackage.Weight = 0;
                    dbPackage.Width = 0;
                    dbPackage.IsPrinted = false;
                    dbPackage.TrackingNo = "";
                    dbPackage.LabelName = "";
                    dbDetail.Packages.Add(dbPackage);
                }
            }
            else if (CallingType == FrayteCallingType.ShipmentDraft)
            {
                dbDetail.CustomerRateCard = null;

                var result = (from DS in dbContext.DirectShipmentDrafts
                              join DSD in dbContext.DirectShipmentDetailDrafts on DS.DirectShipmentDraftId equals DSD.DirectShipmentDraftId
                              where DS.DirectShipmentDraftId == dbDetail.DirectShipmentDraftId
                              select new
                              {
                                  DirectShipmetDetailDraftId = DSD.DirectShipmentDetailDraftId,
                                  PiecesContent = DSD.PiecesContent,
                                  CartoonValue = DSD.CartoonValue,
                                  Height = DSD.Height,
                                  Length = DSD.Length,
                                  Width = DSD.Width,
                                  Weight = DSD.Weight,
                                  DeclaredValue = DSD.DeclaredValue
                              }).ToList();

                if (result != null && result.Count > 0)
                {
                    foreach (var Obj in result)
                    {
                        dbPackage = new PackageDraft();
                        dbPackage.Content = Obj.PiecesContent;
                        dbPackage.DirectShipmentDetailDraftId = Obj.DirectShipmetDetailDraftId;
                        dbPackage.CartoonValue = Obj.CartoonValue.Value;
                        if (Obj.Height.HasValue)
                        {
                            dbPackage.Height = Obj.Height.Value;
                        }
                        else
                        {
                            dbPackage.Height = 0.0m;
                        }
                        if (Obj.Length.HasValue)
                        {
                            dbPackage.Length = Obj.Length.Value;
                        }
                        else
                        {
                            dbPackage.Length = 0.0m;
                        }
                        if (Obj.DeclaredValue.HasValue)
                        {
                            dbPackage.Value = Obj.DeclaredValue.Value;
                        }
                        if (Obj.Weight.HasValue)
                        {
                            dbPackage.Weight = Obj.Weight.Value;
                        }
                        else
                        {
                            dbPackage.Weight = 0.0m;
                        }
                        if (Obj.Width.HasValue)
                        {
                            dbPackage.Width = Obj.Width.Value;
                        }
                        else
                        {
                            dbPackage.Width = 0.0m;
                        }
                        dbDetail.Packages.Add(dbPackage);
                    }
                }
                else
                {
                    dbPackage = new PackageDraft();
                    dbPackage.Content = "";
                    dbPackage.DirectShipmentDetailDraftId = 0;
                    dbPackage.PackageTrackingDetailId = 0;
                    dbPackage.CartoonValue = 0;
                    dbPackage.Height = 0;
                    dbPackage.Length = 0;
                    dbPackage.Value = 0;
                    dbPackage.Weight = 0;
                    dbPackage.Width = 0;
                    dbPackage.IsPrinted = false;
                    dbPackage.TrackingNo = "";
                    dbPackage.LabelName = "";
                    dbDetail.Packages.Add(dbPackage);
                }
            }
            else if (CallingType == FrayteCallingType.FrayteApiDraft)
            {
                var result = (from DS in dbContext.DirectShipmentDrafts
                              join DSD in dbContext.DirectShipmentDetailDrafts on DS.DirectShipmentDraftId equals DSD.DirectShipmentDraftId
                              where DS.DirectShipmentDraftId == dbDetail.DirectShipmentDraftId
                              select new
                              {
                                  DirectShipmetDetailDraftId = DSD.DirectShipmentDetailDraftId,
                                  PiecesContent = DSD.PiecesContent,
                                  CartoonValue = DSD.CartoonValue,
                                  Height = DSD.Height,
                                  Length = DSD.Length,
                                  Width = DSD.Width,
                                  Weight = DSD.Weight,
                                  DeclaredValue = DSD.DeclaredValue
                              }).ToList();

                if (result != null && result.Count > 0)
                {
                    foreach (var Obj in result)
                    {
                        dbPackage = new PackageDraft();
                        dbPackage.Content = Obj.PiecesContent;
                        dbPackage.DirectShipmentDetailDraftId = Obj.DirectShipmetDetailDraftId;
                        dbPackage.CartoonValue = Obj.CartoonValue.Value;
                        if (Obj.Height.HasValue)
                        {
                            dbPackage.Height = Obj.Height.Value;
                        }
                        else
                        {
                            dbPackage.Height = 0.0m;
                        }
                        if (Obj.Length.HasValue)
                        {
                            dbPackage.Length = Obj.Length.Value;
                        }
                        else
                        {
                            dbPackage.Length = 0.0m;
                        }
                        if (Obj.DeclaredValue.HasValue)
                        {
                            dbPackage.Value = Obj.DeclaredValue.Value;
                        }
                        if (Obj.Weight.HasValue)
                        {
                            dbPackage.Weight = Obj.Weight.Value;
                        }
                        else
                        {
                            dbPackage.Weight = 0.0m;
                        }
                        if (Obj.Width.HasValue)
                        {
                            dbPackage.Width = Obj.Width.Value;
                        }
                        else
                        {
                            dbPackage.Width = 0.0m;
                        }
                        dbDetail.Packages.Add(dbPackage);
                    }
                }
                else
                {
                    dbPackage = new PackageDraft();
                    dbPackage.Content = "";
                    dbPackage.DirectShipmentDetailDraftId = 0;
                    dbPackage.PackageTrackingDetailId = 0;
                    dbPackage.CartoonValue = 0;
                    dbPackage.Height = 0;
                    dbPackage.Length = 0;
                    dbPackage.Value = 0;
                    dbPackage.Weight = 0;
                    dbPackage.Width = 0;
                    dbPackage.IsPrinted = false;
                    dbPackage.TrackingNo = "";
                    dbPackage.LabelName = "";
                    dbDetail.Packages.Add(dbPackage);
                }
            }
        }

        private void GetQuotationShipmentPackageDetail(DirectBookingShipmentDraftDetail dbDetail)
        {
            List<QuotationShipmentDetail> result = dbContext.QuotationShipmentDetails.Where(p => p.QuotationShipmentId == dbDetail.DirectShipmentDraftId).ToList();
            if (result != null && result.Count > 0)
            {
                PackageDraft dbPackage;
                dbDetail.Packages = new List<PackageDraft>();
                foreach (QuotationShipmentDetail Obj in result)
                {
                    dbPackage = new PackageDraft();
                    dbPackage.DirectShipmentDetailDraftId = 0;
                    dbPackage.Height = Obj.Height;
                    dbPackage.Length = Obj.Length;
                    dbPackage.Width = Obj.Width;
                    dbPackage.Weight = Obj.Weight;
                    dbPackage.Content = Obj.PiecesContent;
                    dbPackage.CartoonValue = Obj.CartonValue;
                    dbPackage.Value = Obj.DeclaredValue;
                    dbDetail.Packages.Add(dbPackage);
                }
            }
        }

        private void GetQuotationServices(DirectBookingShipmentDraftDetail dbDetail)
        {
            var services = dbContext.QuotationShipments.Find(dbDetail.DirectShipmentDraftId);
            if (services != null)
            {
                dbDetail.CustomerRateCard = new DirectBookingService();
                dbDetail.CustomerRateCard.LogisticServiceId = 2;

                var currency = (from UA in dbContext.UserAdditionals
                                where UA.UserId == services.CustomerId
                                select new { UA.CreditLimitCurrencyCode }).FirstOrDefault();

                if (services.LogisticType == FrayteLogisticType.UKShipment)
                {
                    var courier = (from lsca in dbContext.LogisticServiceCourierAccounts
                                   join ls in dbContext.LogisticServices on lsca.LogisticServiceId equals ls.LogisticServiceId
                                   where ls.LogisticType == services.LogisticType &&
                                         ls.LogisticCompany == services.LogisticCompany &&
                                         lsca.AccountNo == services.CourierAccountNo
                                   select new { lsca.LogisticServiceCourierAccountId }).FirstOrDefault();

                    if (courier != null)
                    {
                        dbDetail.CustomerRateCard.CourierId = courier.LogisticServiceCourierAccountId;
                    }
                    else
                    {
                        dbDetail.CustomerRateCard.CourierId = 0;
                    }
                }
                else
                {
                    var courier = (from lsca in dbContext.LogisticServiceCourierAccounts
                                   join ls in dbContext.LogisticServices on lsca.LogisticServiceId equals ls.LogisticServiceId
                                   where ls.LogisticType == services.LogisticType &&
                                         ls.LogisticCompany == services.LogisticCompany &&
                                         ls.RateType == services.RateType &&
                                         lsca.AccountNo == services.CourierAccountNo
                                   select new { lsca.LogisticServiceCourierAccountId }).FirstOrDefault();

                    if (courier != null)
                    {
                        dbDetail.CustomerRateCard.CourierId = courier.LogisticServiceCourierAccountId;
                    }
                    else
                    {
                        dbDetail.CustomerRateCard.CourierId = 0;
                    }
                }

                var optser = (from qs in dbContext.QuotationShipments
                              join qos in dbContext.QuotationShipmentOptionalServices on qs.QuotationShipmentId equals qos.QuotationShipmentId
                              join los in dbContext.LogisticOptionalServices on qos.LogisticOptionalServiceId equals los.LogisticOptionalServiceId
                              join ls in dbContext.LogisticServices on los.LogisticServiceId equals ls.LogisticServiceId
                              where qs.QuotationShipmentId == services.QuotationShipmentId
                              select new DirectBookingOptionalServices
                              {
                                  LogisticOptionalServiceId = los.LogisticOptionalServiceId,
                                  LogisticCompany = ls.LogisticCompany,
                                  ServiceCode = qos.OptionalServiceCode,
                                  ServiceDescription = los.Description,
                                  IsEnable = true
                              }).ToList();

                dbDetail.CustomerRateCard.OptionalServices = new List<DirectBookingOptionalServices>();
                {
                    if (optser != null && optser.Count > 0)
                    {
                        DirectBookingOptionalServices optservices;
                        foreach (var ss in optser)
                        {
                            if (ss.IsEnable)
                            {
                                optservices = new DirectBookingOptionalServices();
                                optservices.LogisticOptionalServiceId = ss.LogisticOptionalServiceId;
                                optservices.LogisticCompany = ss.LogisticCompany;
                                optservices.ServiceDescription = ss.ServiceDescription;
                                optservices.ServiceCode = ss.ServiceCode;
                                optservices.IsEnable = true;
                                dbDetail.CustomerRateCard.OptionalServices.Add(optservices);
                            }
                        }
                    }
                    //else
                    //{
                    //    var dd = (from lsca in dbContext.LogisticServiceCourierAccounts
                    //              join ls in dbContext.LogisticServices on lsca.LogisticServiceId equals ls.LogisticServiceId
                    //              join los in dbContext.LogisticOptionalServices on ls.LogisticServiceId equals los.LogisticOptionalServiceId
                    //              where ls.LogisticType == services.LogisticType &&
                    //                    ls.LogisticCompany == services.LogisticCompany &&
                    //                    ls.RateType == services.RateType &&
                    //                    lsca.AccountNo == services.CourierAccountNo
                    //              select new DirectBookingOptionalServices
                    //              {
                    //                  LogisticOptionalServiceId = los.LogisticOptionalServiceId,
                    //                  LogisticCompany = ls.LogisticCompany,
                    //                  ServiceCode = los.ServiceCode,
                    //                  ServiceDescription = los.Description,
                    //                  IsEnable = true
                    //              }).ToList();

                    //    if()
                    //}
                }

                dbDetail.CustomerRateCard.AdditionalSurcharge = (float)(services.AdditionalSurcharge.HasValue ? services.AdditionalSurcharge.Value : 0.0m);
                dbDetail.CustomerRateCard.Rate = Math.Round((services.BaseRate.HasValue ? services.BaseRate.Value : 0.0m) + (services.Margin.HasValue ? services.Margin.Value : 0.0m), 2);
                dbDetail.CustomerRateCard.Margin = services.Margin.HasValue ? services.Margin.Value : 0.0m;
                dbDetail.CustomerRateCard.IntegrationAccountId = services.IntegrationAccountId;
                dbDetail.CustomerRateCard.CourierAccountNo = services.CourierAccountNo;
                dbDetail.CustomerRateCard.CourierAccountCountryCode = services.LogisticCompany;
                dbDetail.CustomerRateCard.CourierName = services.LogisticCompany;
                dbDetail.CustomerRateCard.DisplayName = services.LogisticCompanyDisplay;
                dbDetail.CustomerRateCard.CourierDescription = services.CourierDescription;
                dbDetail.CustomerRateCard.PakageType = services.ParcelType;
                dbDetail.CustomerRateCard.ParcelServiceType = services.ParcelServiceType;
                dbDetail.CustomerRateCard.LogisticType = services.LogisticType;
                dbDetail.CustomerRateCard.WeightType = services.ShipmentType;
                dbDetail.CustomerRateCard.FuelSurcharge = (float)(services.FuelSurCharge.HasValue ? services.FuelSurCharge.Value : 0.0m);
                dbDetail.CustomerRateCard.FuelSurchargePercent = (float)(services.FuelSurchargePercent.HasValue ? services.FuelSurchargePercent.Value : 0.0m);
                dbDetail.CustomerRateCard.TotalEstimatedCharge = (float)Math.Round((float)(dbDetail.CustomerRateCard.Rate) + dbDetail.CustomerRateCard.AdditionalSurcharge + dbDetail.CustomerRateCard.FuelSurcharge, 2);
                dbDetail.CustomerRateCard.CurrencyCode = services.CurrencyCode;
                if (currency != null)
                {
                    dbDetail.CustomerRateCard.CustomerCurrency = currency.CreditLimitCurrencyCode;
                }
                dbDetail.CustomerRateCard.FuelMonth = UtilityRepository.MonthName(services.FuelMonthYear.Value) + "-" + services.FuelMonthYear.Value.Year.ToString().Substring(2, 2);
                dbDetail.CustomerRateCard.FuelDate = services.FuelMonthYear.Value;
                dbDetail.CustomerRateCard.RateType = services.RateType;
                dbDetail.CustomerRateCard.RateTypeDisplay = services.RateTypeDisplay;
                dbDetail.CustomerRateCard.PackageCalculationType = services.PackageCaculatonType;
                dbDetail.CustomerRateCard.TransitTime = services.TransitTime;
                dbDetail.CustomerRateCard.Weight = services.ChargeableWeight.HasValue ? services.ChargeableWeight.Value : 0.00m;
            }
        }

        public void GetDirectShipmentCollectionDraftDetail(DirectBookingShipmentDraftDetail dbDetail, string CallingType)
        {
            if (CallingType == FrayteCallingType.ShipmentClone || CallingType == FrayteCallingType.ShipmentReturn)
            {
                //Ship From
                DirectShipmentAddress shipFrom = dbContext.DirectShipmentAddresses.Find(dbDetail.ShipFrom.AddressBookId);
                if (shipFrom != null)
                {
                    dbDetail.ShipFrom.Address = shipFrom.Address1;
                    dbDetail.ShipFrom.Address2 = shipFrom.Address2;
                    dbDetail.ShipFrom.Area = shipFrom.Area;
                    dbDetail.ShipFrom.City = shipFrom.City;
                    dbDetail.ShipFrom.CompanyName = shipFrom.CompanyName;

                    dbDetail.ShipFrom.Country = new FrayteCountryCode();
                    var country = dbContext.Countries.Where(p => p.CountryId == shipFrom.CountryId).FirstOrDefault();
                    if (country != null)
                    {
                        dbDetail.ShipFrom.Country.CountryId = country.CountryId;
                        dbDetail.ShipFrom.Country.Code = country.CountryCode;
                        dbDetail.ShipFrom.Country.Code2 = country.CountryCode2;
                        dbDetail.ShipFrom.Country.Name = country.CountryName;
                    }

                    dbDetail.ShipFrom.Email = shipFrom.Email;
                    dbDetail.ShipFrom.FirstName = shipFrom.ContactFirstName;
                    dbDetail.ShipFrom.LastName = shipFrom.ContactLastName;
                    dbDetail.ShipFrom.Phone = shipFrom.PhoneNo;
                    dbDetail.ShipFrom.PostCode = shipFrom.Zip;
                    dbDetail.ShipFrom.State = shipFrom.State;
                }

                //Ship To
                DirectShipmentAddress shipTo = dbContext.DirectShipmentAddresses.Find(dbDetail.ShipTo.AddressBookId);
                if (shipTo != null)
                {
                    dbDetail.ShipTo.Address = shipTo.Address1;
                    dbDetail.ShipTo.Address2 = shipTo.Address2;
                    dbDetail.ShipTo.Area = shipTo.Area;
                    dbDetail.ShipTo.City = shipTo.City;
                    dbDetail.ShipTo.CompanyName = shipTo.CompanyName;

                    dbDetail.ShipTo.Country = new FrayteCountryCode();
                    var country = dbContext.Countries.Where(p => p.CountryId == shipTo.CountryId).FirstOrDefault();
                    if (country != null)
                    {
                        dbDetail.ShipTo.Country.CountryId = country.CountryId;
                        dbDetail.ShipTo.Country.Code = country.CountryCode;
                        dbDetail.ShipTo.Country.Code2 = country.CountryCode2;
                        dbDetail.ShipTo.Country.Name = country.CountryName;
                    }

                    dbDetail.ShipTo.Email = shipTo.Email;
                    dbDetail.ShipTo.FirstName = shipTo.ContactFirstName;
                    dbDetail.ShipTo.LastName = shipTo.ContactLastName;
                    dbDetail.ShipTo.Phone = shipTo.PhoneNo;
                    dbDetail.ShipTo.PostCode = shipTo.Zip;
                    dbDetail.ShipTo.State = shipTo.State;
                }
            }
            else
            {
                //Ship From
                if (dbDetail.ShipFrom != null && dbDetail.ShipFrom.AddressBookId > 0)
                {
                    AddressBook shipFrom = dbContext.AddressBooks.Find(dbDetail.ShipFrom.AddressBookId);
                    if (shipFrom != null)
                    {
                        dbDetail.ShipFrom.CustomerId = shipFrom.CustomerId;
                        var CustomerName = dbContext.Users.Where(p => p.UserId == shipFrom.CustomerId).FirstOrDefault();
                        if (CustomerName != null)
                        {
                            dbDetail.ShipFrom.CustomerName = CustomerName.ContactName;
                        }
                        dbDetail.ShipFrom.Address = shipFrom.Address1;
                        dbDetail.ShipFrom.Address2 = shipFrom.Address2;
                        dbDetail.ShipFrom.Area = shipFrom.Area;
                        dbDetail.ShipFrom.City = shipFrom.City;
                        dbDetail.ShipFrom.CompanyName = shipFrom.CompanyName;

                        dbDetail.ShipFrom.Country = new FrayteCountryCode();
                        var country = dbContext.Countries.Where(p => p.CountryId == shipFrom.CountryId).FirstOrDefault();
                        if (country != null)
                        {
                            dbDetail.ShipFrom.Country.CountryId = country.CountryId;
                            dbDetail.ShipFrom.Country.Code = country.CountryCode;
                            dbDetail.ShipFrom.Country.Code2 = country.CountryCode2;
                            dbDetail.ShipFrom.Country.Name = country.CountryName;
                        }

                        dbDetail.ShipFrom.Email = shipFrom.Email;
                        dbDetail.ShipFrom.FirstName = shipFrom.ContactFirstName;
                        dbDetail.ShipFrom.LastName = shipFrom.ContactLastName;
                        dbDetail.ShipFrom.Phone = shipFrom.PhoneNo;
                        dbDetail.ShipFrom.PostCode = shipFrom.Zip;
                        dbDetail.ShipFrom.State = shipFrom.State;
                        dbDetail.ShipFrom.AddressType = shipFrom.TableType;
                    }
                }

                //Ship To
                if (dbDetail.ShipTo != null && dbDetail.ShipTo.AddressBookId > 0)
                {
                    AddressBook shipTo = dbContext.AddressBooks.Find(dbDetail.ShipTo.AddressBookId);
                    if (shipTo != null)
                    {
                        dbDetail.ShipTo.CustomerId = dbDetail.ShipFrom.CustomerId;
                        var CustomerName = dbContext.Users.Where(p => p.UserId == shipTo.CustomerId).FirstOrDefault();
                        if (CustomerName != null)
                        {
                            dbDetail.ShipTo.CustomerName = CustomerName.ContactName;
                        }
                        dbDetail.ShipTo.Address = shipTo.Address1;
                        dbDetail.ShipTo.Address2 = shipTo.Address2;
                        dbDetail.ShipTo.Area = shipTo.Area;
                        dbDetail.ShipTo.City = shipTo.City;
                        dbDetail.ShipTo.CompanyName = shipTo.CompanyName;

                        dbDetail.ShipTo.Country = new FrayteCountryCode();
                        var country = dbContext.Countries.Where(p => p.CountryId == shipTo.CountryId).FirstOrDefault();
                        if (country != null)
                        {
                            dbDetail.ShipTo.Country.CountryId = country.CountryId;
                            dbDetail.ShipTo.Country.Code = country.CountryCode;
                            dbDetail.ShipTo.Country.Code2 = country.CountryCode2;
                            dbDetail.ShipTo.Country.Name = country.CountryName;
                        }

                        dbDetail.ShipTo.Email = shipTo.Email;
                        dbDetail.ShipTo.FirstName = shipTo.ContactFirstName;
                        dbDetail.ShipTo.LastName = shipTo.ContactLastName;
                        dbDetail.ShipTo.Phone = shipTo.PhoneNo;
                        dbDetail.ShipTo.PostCode = shipTo.Zip;
                        dbDetail.ShipTo.State = shipTo.State;
                        dbDetail.ShipTo.AddressType = dbDetail.ShipFrom.AddressType;
                    }
                }
            }
        }

        private void GetQuotationShipFromShipToDetail(DirectBookingShipmentDraftDetail dbDetail)
        {
            QuotationShipment result = dbContext.QuotationShipments.Find(dbDetail.DirectShipmentDraftId);
            if (result != null)
            {
                if (result.FromCountryId > 0)
                {
                    dbDetail.ShipFrom = new DirectBookingDraftCollection();
                    dbDetail.AddressType = result.AddressType;
                    dbDetail.ShipFrom.CustomerId = result.CustomerId;
                    dbDetail.ShipFrom.Address = "";
                    dbDetail.ShipFrom.Address2 = "";
                    dbDetail.ShipFrom.Area = "";
                    dbDetail.ShipFrom.City = "";
                    dbDetail.ShipFrom.CompanyName = "";
                    dbDetail.ShipFrom.FirstName = "";
                    dbDetail.ShipFrom.LastName = "";
                    dbDetail.ShipFrom.Country = new FrayteCountryCode();
                    var CountryCode = dbContext.Countries.Where(a => a.CountryId == result.FromCountryId).FirstOrDefault();
                    if (CountryCode != null)
                    {
                        dbDetail.ShipFrom.Country.CountryId = result.FromCountryId;
                        dbDetail.ShipFrom.Country.Name = CountryCode.CountryName;
                        dbDetail.ShipFrom.Country.Code = CountryCode.CountryCode;
                        dbDetail.ShipFrom.Country.Code2 = CountryCode.CountryCode2;
                    }
                    else
                    {
                        dbDetail.ShipFrom.Country.CountryId = result.FromCountryId;
                    }
                    dbDetail.ShipFrom.Email = "";
                    dbDetail.ShipFrom.Phone = "";
                    dbDetail.ShipFrom.State = "";
                    dbDetail.ShipFrom.PostCode = result.FromPostCode;
                }
                if (result.ToCountryId > 0)
                {
                    dbDetail.ShipTo = new DirectBookingDraftCollection();
                    dbDetail.ShipTo.CustomerId = result.CustomerId;
                    dbDetail.ShipTo.Address = "";
                    dbDetail.ShipTo.Address2 = "";
                    dbDetail.ShipTo.Area = "";
                    dbDetail.ShipTo.City = "";
                    dbDetail.ShipTo.CompanyName = "";
                    dbDetail.ShipTo.FirstName = "";
                    dbDetail.ShipTo.LastName = "";
                    dbDetail.ShipTo.Country = new FrayteCountryCode();
                    var CountryCode = dbContext.Countries.Where(a => a.CountryId == result.ToCountryId).FirstOrDefault();
                    if (CountryCode != null)
                    {
                        dbDetail.ShipTo.Country.CountryId = result.ToCountryId;
                        dbDetail.ShipTo.Country.Name = CountryCode.CountryName;
                        dbDetail.ShipTo.Country.Code = CountryCode.CountryCode;
                        dbDetail.ShipTo.Country.Code2 = CountryCode.CountryCode2;
                    }
                    else
                    {
                        dbDetail.ShipTo.Country.CountryId = result.ToCountryId;
                    }
                    dbDetail.ShipTo.Email = "";
                    dbDetail.ShipTo.Phone = "";
                    dbDetail.ShipTo.State = "";
                    dbDetail.ShipTo.PostCode = result.ToPostCode;
                }
            }
        }

        public void GetDirectShipmentCustomDraftDetail(DirectBookingShipmentDraftDetail dbDetail, string CallingType)
        {
            if (CallingType == FrayteCallingType.ShipmentClone || CallingType == FrayteCallingType.ShipmentReturn)
            {
                //ShipmentCustomDetail customDetail = dbContext.ShipmentCustomDetails.Where(p => p.ShipmentId == dbDetail.DirectShipmentDraftId && p.ShipmentServiceType == FrayteShipmentServiceType.DirectBooking).FirstOrDefault();
                //if (customDetail != null)
                //{
                //    dbDetail.CustomInfo = new CustomInformation();
                //    dbDetail.CustomInfo.ShipmentId = dbDetail.DirectShipmentDraftId;
                //    dbDetail.CustomInfo.ShipmentCustomDetailId = customDetail.ShipmentCustomDetailId;
                //    //EasyPost Details
                //    dbDetail.CustomInfo.ContentsType = customDetail.ContentsType;
                //    dbDetail.CustomInfo.ContentsExplanation = customDetail.ContentsExplanation;
                //    dbDetail.CustomInfo.RestrictionType = customDetail.RestrictionType;
                //    dbDetail.CustomInfo.RestrictionComments = customDetail.RestrictionComments;
                //    dbDetail.CustomInfo.CustomsCertify = customDetail.CustomsCertify;
                //    dbDetail.CustomInfo.CustomsSigner = customDetail.CustomsSigner;
                //    dbDetail.CustomInfo.NonDeliveryOption = customDetail.NonDeliveryOption;
                //    dbDetail.CustomInfo.EelPfc = customDetail.EelPfc;

                //    //Parcle Hub Details
                //    dbDetail.CustomInfo.CatagoryOfItem = customDetail.CatagoryOfItem;
                //    dbDetail.CustomInfo.CatagoryOfItemExplanation = customDetail.CatagoryOfItemExplanation;
                //    //dbDetail.CustomInfo.CommodityCode = customDetail.CommodityCode;
                //    //dbDetail.CustomInfo.TermOfTrade = customDetail.TermOfTrade;
                //    dbDetail.CustomInfo.TermOfTrade = dbDetail.PayTaxAndDuties;
                //}
            }
            else
            {
                ShipmentCustomDetailDraft customDetail = dbContext.ShipmentCustomDetailDrafts.Where(p => p.ShipmentDraftId == dbDetail.DirectShipmentDraftId && p.ShipmentServiceType == FrayteShipmentServiceType.DirectBooking).FirstOrDefault();
                if (customDetail != null)
                {
                    dbDetail.CustomInfo = new CustomInformation();
                    dbDetail.CustomInfo.ShipmentId = dbDetail.DirectShipmentDraftId;
                    dbDetail.CustomInfo.ShipmentCustomDetailId = customDetail.ShipmentCustomDetailDraftId;
                    //EasyPost Details
                    dbDetail.CustomInfo.ContentsType = customDetail.ContentsType;
                    dbDetail.CustomInfo.ContentsExplanation = customDetail.ContentsExplanation;
                    dbDetail.CustomInfo.RestrictionType = customDetail.RestrictionType;
                    dbDetail.CustomInfo.RestrictionComments = customDetail.RestrictionComments;
                    dbDetail.CustomInfo.CustomsCertify = customDetail.CustomsCertify;
                    dbDetail.CustomInfo.CustomsSigner = customDetail.CustomsSigner;
                    dbDetail.CustomInfo.NonDeliveryOption = customDetail.NonDeliveryOption;
                    dbDetail.CustomInfo.EelPfc = customDetail.EelPfc;

                    //Parcle Hub Details
                    dbDetail.CustomInfo.CatagoryOfItem = customDetail.CatagoryOfItem;
                    dbDetail.CustomInfo.CatagoryOfItemExplanation = customDetail.CatagoryOfItemExplanation;
                    //dbDetail.CustomInfo.CommodityCode = customDetail.CommodityCode;
                    //dbDetail.CustomInfo.TermOfTrade = customDetail.TermOfTrade;
                    dbDetail.CustomInfo.TermOfTrade = dbDetail.PayTaxAndDuties;
                }
            }
        }

        #endregion

        #region -- SaveDirectBooking --

        public DirectBookingShipmentDraftDetail SaveDirectBooking(DirectBookingShipmentDraftDetail directBookingShippingDetail)
        {
            try
            {
                //Step 0.1: Set Proper PostCode
                SetPostCode(directBookingShippingDetail);

                var userDetail = (from r in dbContext.Users
                                  join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                                  where r.UserId == directBookingShippingDetail.CreatedBy
                                  select new
                                  {
                                      UserRole = ur.RoleId,
                                      UserId = r.UserId
                                  }).FirstOrDefault();

                //Step 1: Save ShipFrom
                directBookingShippingDetail.ShipFrom.CustomerId = userDetail.UserRole == (int)FrayteUserRole.UserCustomer ? directBookingShippingDetail.CreatedBy : directBookingShippingDetail.CustomerId;
                directBookingShippingDetail.ShipFrom.AddressType = FrayteFromToAddressType.FromAddress;
                SaveDirectShipmentAddress(directBookingShippingDetail.ShipFrom);

                //Step 2: Save ShipTo
                directBookingShippingDetail.ShipTo.CustomerId = userDetail.UserRole == (int)FrayteUserRole.UserCustomer ? directBookingShippingDetail.CreatedBy : directBookingShippingDetail.CustomerId;
                directBookingShippingDetail.ShipTo.AddressType = FrayteFromToAddressType.ToAddress;
                SaveDirectShipmentAddress(directBookingShippingDetail.ShipTo);

                //Step 3: Save Direct Shipmnet + Reference Detail
                SaveDirectShipmnetDetail(directBookingShippingDetail);

                //Step 4: Save Direct Shipment Detail
                SaveDirectShipmentDetailPackages(directBookingShippingDetail);

                //Save 5: Save Custom Information
                SaveCustomInformation(directBookingShippingDetail);

                directBookingShippingDetail.Error = new FratyteError();
                directBookingShippingDetail.Error.Status = true;

                return directBookingShippingDetail;
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
        }

        public DirectShipmentDraft GetDBDateTime(int DirectShipmentDraftId)
        {
            var Result = dbContext.DirectShipmentDrafts.Where(a => a.DirectShipmentDraftId == DirectShipmentDraftId).FirstOrDefault();
            return Result;
        }

        public TimeZoneModal TimeZoneDetail(int CountryId)
        {
            var TimeZone = (from uu in dbContext.Countries
                            join tz in dbContext.Timezones on uu.TimeZoneId equals tz.TimezoneId
                            where uu.CountryId == CountryId
                            select tz).FirstOrDefault();
            TimeZoneModal TZ = new TimeZoneModal();
            if (TimeZone != null)
            {
                TZ.Name = TimeZone.Name;
                TZ.Offset = TimeZone.Offset;
                TZ.OffsetShort = TimeZone.OffsetShort;
                TZ.TimezoneId = TimeZone.TimezoneId;
            }
            return TZ;
        }

        private void SetPostCode(DirectBookingShipmentDraftDetail directBookingShippingDetail)
        {
            //Step 1: Set ShipFrom PinCode
            directBookingShippingDetail.ShipFrom.PostCode = UtilityRepository.PostCodeVerification(directBookingShippingDetail.ShipFrom.PostCode, directBookingShippingDetail.ShipFrom.Country.Code2);

            //Step 2: Set ShipTo PinCode
            directBookingShippingDetail.ShipTo.PostCode = UtilityRepository.PostCodeVerification(directBookingShippingDetail.ShipTo.PostCode, directBookingShippingDetail.ShipTo.Country.Code2);
        }

        private void SaveDirectShipmentDetailPackages(DirectBookingShipmentDraftDetail directBookingShippingDetail)
        {
            try
            {
                if (directBookingShippingDetail.Packages != null && directBookingShippingDetail.Packages.Count > 0)
                {
                    foreach (PackageDraft package in directBookingShippingDetail.Packages)
                    {
                        DirectShipmentDetailDraft packageDetail;
                        if (package.DirectShipmentDetailDraftId > 0)
                        {
                            packageDetail = dbContext.DirectShipmentDetailDrafts.Find(package.DirectShipmentDetailDraftId);
                            if (packageDetail != null)
                            {
                                packageDetail.CartoonValue = package.CartoonValue;
                                packageDetail.DeclaredValue = package.Value;
                                packageDetail.DirectShipmentDraftId = directBookingShippingDetail.DirectShipmentDraftId;
                                packageDetail.Height = package.Height;
                                packageDetail.Length = package.Length;
                                packageDetail.PiecesContent = package.Content;
                                packageDetail.Weight = package.Weight;
                                packageDetail.Width = package.Width;
                            }
                        }
                        else
                        {
                            packageDetail = new DirectShipmentDetailDraft();
                            packageDetail.DeclaredValue = package.Value;
                            packageDetail.DirectShipmentDraftId = directBookingShippingDetail.DirectShipmentDraftId;
                            packageDetail.CartoonValue = package.CartoonValue;
                            packageDetail.Height = package.Height;
                            packageDetail.Length = package.Length;
                            packageDetail.PiecesContent = package.Content;
                            packageDetail.Weight = package.Weight;
                            packageDetail.Width = package.Width;
                            dbContext.DirectShipmentDetailDrafts.Add(packageDetail);
                        }

                        if (packageDetail != null)
                        {
                            dbContext.SaveChanges();
                        }

                        package.DirectShipmentDetailDraftId = packageDetail.DirectShipmentDetailDraftId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw (new FrayteApiException("ShipmentPackageError", ex));
            }
        }

        public void SaveDirectShipmnetDetail(DirectBookingShipmentDraftDetail directBookingShippingDetail)
        {
            DirectShipmentDraft dbDirectShipment;
            try
            {
                FrayteOperationZone operationZone = UtilityRepository.GetOperationZone();
                var TZ = TimeZoneDetail(directBookingShippingDetail.ShipFrom.Country.CountryId);
                if (directBookingShippingDetail.DirectShipmentDraftId == 0)
                {
                    dbDirectShipment = new DirectShipmentDraft();
                    dbDirectShipment.ShipmentStatusId = directBookingShippingDetail.ShipmentStatusId;
                    dbDirectShipment.CurrencyCode = directBookingShippingDetail.Currency == null ? null : directBookingShippingDetail.Currency.CurrencyCode;

                    DirectBookingFindService serviceRequest = new DirectBookingFindService();
                    serviceRequest.FromCountry = directBookingShippingDetail.ShipFrom.Country;
                    serviceRequest.ToCountry = directBookingShippingDetail.ShipTo.Country;
                    serviceRequest.OperationZoneId = operationZone.OperationZoneId;
                    bool isEuropeCountry = CheckEuropeCountries(serviceRequest);

                    dbDirectShipment.LogisticType = UtilityRepository.GetLogisticType(operationZone.OperationZoneName, directBookingShippingDetail.ShipFrom.Country.Code, directBookingShippingDetail.ShipTo.Country.Code, isEuropeCountry);

                    if (dbDirectShipment.LogisticType == FrayteLogisticType.UKShipment)
                    {
                        dbDirectShipment.LogisticServiceType = directBookingShippingDetail.CustomerRateCard != null ? directBookingShippingDetail.CustomerRateCard.CourierName : "";
                    }
                    else
                    {
                        dbDirectShipment.LogisticServiceType = directBookingShippingDetail.CustomerRateCard != null ? directBookingShippingDetail.CustomerRateCard.CourierName : "";
                    }

                    if (dbDirectShipment.ShipmentStatusId == DirectBookingShippingStatus.Draft && directBookingShippingDetail.CustomerRateCard != null)
                    {
                        #region -- Set Base Rate --

                        if (directBookingShippingDetail.CustomerRateCard.ZoneRateCardId > 0)
                        {
                            if (directBookingShippingDetail.CustomerRateCard.LogisticDescription == FrayteShipmentType.HeavyWeight)
                            {
                                dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate * directBookingShippingDetail.CustomerRateCard.Weight;
                            }
                            else
                            {
                                dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate;
                            }
                            dbDirectShipment.ShipmentTypeId = directBookingShippingDetail.CustomerRateCard.LogisticShipmentId;
                            dbDirectShipment.CourierAccountId = directBookingShippingDetail.CustomerRateCard.CourierAccountId;
                            dbDirectShipment.Margin = (dbDirectShipment.BaseRate) * (directBookingShippingDetail.CustomerRateCard.MarginPercent / 100);
                            dbDirectShipment.FuelSurCharge = (dbDirectShipment.BaseRate + dbDirectShipment.Margin + Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge)) * (decimal)(directBookingShippingDetail.CustomerRateCard.FuelSurcharge / 100);

                            //Additional Surcharge
                            dbDirectShipment.AdditionalSurcharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge);
                            dbDirectShipment.FuelMonthYear = directBookingShippingDetail.CustomerRateCard.FuelDate;
                            dbDirectShipment.FuelSurchargePercent = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurcharge);
                        }
                        else
                        {
                            dbDirectShipment.ShipmentTypeId = directBookingShippingDetail.ShippingMethodId;
                            directBookingShippingDetail.CustomerRateCard.LogisticServiceId = 0;
                            dbDirectShipment.CourierAccountId = directBookingShippingDetail.CustomerRateCard.CourierId;
                            dbDirectShipment.BaseRate = (directBookingShippingDetail.CustomerRateCard.Rate) - (directBookingShippingDetail.CustomerRateCard.Margin);
                            dbDirectShipment.Margin = directBookingShippingDetail.CustomerRateCard.Margin;
                            dbDirectShipment.FuelSurCharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurcharge);
                            dbDirectShipment.AdditionalSurcharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge);
                            dbDirectShipment.FuelSurchargePercent = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurchargePercent);
                            dbDirectShipment.FuelMonthYear = directBookingShippingDetail.CustomerRateCard.FuelDate;
                        }

                        #endregion
                    }
                    else if (dbDirectShipment.ShipmentStatusId == DirectBookingShippingStatus.Current && directBookingShippingDetail.CustomerRateCard != null)
                    {
                        #region -- Set Base Rate --

                        if (directBookingShippingDetail.CustomerRateCard.ZoneRateCardId > 0)
                        {
                            if (directBookingShippingDetail.CustomerRateCard.LogisticDescription == FrayteShipmentType.HeavyWeight)
                            {
                                dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate * directBookingShippingDetail.CustomerRateCard.Weight;
                            }
                            else
                            {
                                dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate;
                            }
                            dbDirectShipment.Margin = (dbDirectShipment.BaseRate) * (directBookingShippingDetail.CustomerRateCard.MarginPercent / 100);
                            dbDirectShipment.FuelSurCharge = (dbDirectShipment.BaseRate + dbDirectShipment.Margin + Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge)) * (decimal)(directBookingShippingDetail.CustomerRateCard.FuelSurcharge / 100);
                            dbDirectShipment.ShipmentTypeId = directBookingShippingDetail.CustomerRateCard.LogisticShipmentId;
                            dbDirectShipment.CourierAccountId = directBookingShippingDetail.CustomerRateCard.CourierAccountId;

                            //Additional Surcharge
                            dbDirectShipment.AdditionalSurcharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge);
                            dbDirectShipment.FuelMonthYear = directBookingShippingDetail.CustomerRateCard.FuelDate;
                            dbDirectShipment.FuelSurchargePercent = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurcharge);
                        }
                        else
                        {
                            dbDirectShipment.ShipmentTypeId = directBookingShippingDetail.ShippingMethodId;
                            directBookingShippingDetail.CustomerRateCard.LogisticServiceId = 0;
                            dbDirectShipment.CourierAccountId = directBookingShippingDetail.CustomerRateCard.CourierId;
                            dbDirectShipment.BaseRate = (directBookingShippingDetail.CustomerRateCard.Rate) - (directBookingShippingDetail.CustomerRateCard.Margin);
                            dbDirectShipment.Margin = directBookingShippingDetail.CustomerRateCard.Margin;
                            dbDirectShipment.FuelSurCharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurcharge);
                            dbDirectShipment.AdditionalSurcharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge);
                            dbDirectShipment.FuelSurchargePercent = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurchargePercent);
                            dbDirectShipment.FuelMonthYear = directBookingShippingDetail.CustomerRateCard.FuelDate;
                        }

                        #endregion
                    }
                    else if (dbDirectShipment.ShipmentStatusId == DirectBookingShippingStatus.Cancel && directBookingShippingDetail.CustomerRateCard != null)
                    {
                        #region -- Set Base Raet --

                        if (directBookingShippingDetail.CustomerRateCard.ZoneRateCardId > 0)
                        {
                            if (directBookingShippingDetail.CustomerRateCard.LogisticDescription == FrayteShipmentType.HeavyWeight)
                            {
                                dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate * directBookingShippingDetail.CustomerRateCard.Weight;
                            }
                            else
                            {
                                dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate;
                            }
                            dbDirectShipment.Margin = (dbDirectShipment.BaseRate) * (directBookingShippingDetail.CustomerRateCard.MarginPercent / 100);
                            dbDirectShipment.FuelSurCharge = (dbDirectShipment.BaseRate + dbDirectShipment.Margin + Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge)) * (decimal)(directBookingShippingDetail.CustomerRateCard.FuelSurcharge / 100);
                            dbDirectShipment.ShipmentTypeId = directBookingShippingDetail.CustomerRateCard.LogisticShipmentId;
                            dbDirectShipment.CourierAccountId = directBookingShippingDetail.CustomerRateCard.CourierAccountId;

                            //Additional Surcharge
                            dbDirectShipment.AdditionalSurcharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge);
                            dbDirectShipment.FuelMonthYear = directBookingShippingDetail.CustomerRateCard.FuelDate;
                            dbDirectShipment.FuelSurchargePercent = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurcharge);
                        }
                        else
                        {
                            dbDirectShipment.ShipmentTypeId = directBookingShippingDetail.ShippingMethodId;
                            directBookingShippingDetail.CustomerRateCard.LogisticServiceId = 0;
                            dbDirectShipment.CourierAccountId = directBookingShippingDetail.CustomerRateCard.CourierId;
                            dbDirectShipment.BaseRate = (directBookingShippingDetail.CustomerRateCard.Rate) - (directBookingShippingDetail.CustomerRateCard.Margin);
                            dbDirectShipment.Margin = directBookingShippingDetail.CustomerRateCard.Margin;
                            dbDirectShipment.FuelSurCharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurcharge);
                            dbDirectShipment.AdditionalSurcharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge);
                            dbDirectShipment.FuelSurchargePercent = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurchargePercent);
                            dbDirectShipment.FuelMonthYear = directBookingShippingDetail.CustomerRateCard.FuelDate;
                        }

                        #endregion
                    }

                    //Set Reference Detail
                    dbDirectShipment.Reference1 = directBookingShippingDetail.ReferenceDetail.Reference1;
                    dbDirectShipment.ContentDescription = directBookingShippingDetail.ReferenceDetail.ContentDescription;
                    dbDirectShipment.SpecialInstruction = directBookingShippingDetail.ReferenceDetail.SpecialInstruction;
                    if (directBookingShippingDetail.CustomerRateCard != null && (directBookingShippingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UKMail || directBookingShippingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Yodel || directBookingShippingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Hermes))
                    {
                        //dbDirectShipment.CollectionDate = null;
                        //dbDirectShipment.CollectionTime = null;
                    }
                    else
                    {
                        try
                        {
                            if (directBookingShippingDetail.ReferenceDetail.CollectionDate.HasValue)
                            {
                                dbDirectShipment.CollectionDate = directBookingShippingDetail.ReferenceDetail.CollectionDate.HasValue ? UtilityRepository.ConvertToUniversalTimeWitDate(directBookingShippingDetail.ReferenceDetail.CollectionTime, directBookingShippingDetail.ReferenceDetail.CollectionDate.Value, TZ) : DateTime.UtcNow;
                                dbDirectShipment.CollectionTime = UtilityRepository.TimeSpanConversion(dbDirectShipment.CollectionDate.Value.Hour, dbDirectShipment.CollectionDate.Value.Minute, dbDirectShipment.CollectionDate.Value.Second);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    dbDirectShipment.CustomerId = directBookingShippingDetail.CustomerId;
                    dbDirectShipment.FromAddressId = directBookingShippingDetail.ShipFrom.AddressBookId;
                    dbDirectShipment.ToAddressId = directBookingShippingDetail.ShipTo.AddressBookId;
                    dbDirectShipment.IsPODMailSent = false;
                    dbDirectShipment.OpearionZoneId = operationZone.OperationZoneId;
                    dbDirectShipment.ParcelType = directBookingShippingDetail.ParcelType == null ? "" : directBookingShippingDetail.ParcelType.ParcelType;
                    dbDirectShipment.PaymentPartyTaxAndDuties = directBookingShippingDetail.PayTaxAndDuties;
                    dbDirectShipment.PaymentPartyTaxAndDutiesAccountNo = directBookingShippingDetail.PaymentPartyAccountNumber;
                    dbDirectShipment.CreatedBy = directBookingShippingDetail.CreatedBy;
                    dbDirectShipment.LastUpdated = DateTime.UtcNow;
                    dbDirectShipment.PackageCaculatonType = directBookingShippingDetail.PakageCalculatonType;
                    dbDirectShipment.TaxAndDutiesAcceptedBy = directBookingShippingDetail.TaxAndDutiesAcceptedBy;
                    dbDirectShipment.FrayteNumber = null;
                    directBookingShippingDetail.FrayteNumber = CommonConversion.GetNewFrayteNumber();
                    dbDirectShipment.ModuleType = FrayteShipmentServiceType.DirectBooking;
                    dbDirectShipment.AddressType = directBookingShippingDetail.AddressType;
                    dbDirectShipment.IsPublic = directBookingShippingDetail.IsPublic;
                    dbDirectShipment.SessionId = directBookingShippingDetail.SessionId == 0 ? 0 : directBookingShippingDetail.SessionId;
                    dbDirectShipment.BookingApp = "DirectBooking_SS";
                    dbDirectShipment.ChargeableWeight = directBookingShippingDetail.CustomerRateCard.Weight;
                    dbContext.DirectShipmentDrafts.Add(dbDirectShipment);
                }
                else
                {
                    dbDirectShipment = dbContext.DirectShipmentDrafts.Find(directBookingShippingDetail.DirectShipmentDraftId);
                    if (dbDirectShipment != null)
                    {
                        dbDirectShipment.ShipmentStatusId = directBookingShippingDetail.ShipmentStatusId;
                        dbDirectShipment.CurrencyCode = directBookingShippingDetail.Currency.CurrencyCode;

                        DirectBookingFindService serviceRequest = new DirectBookingFindService();
                        serviceRequest.FromCountry = directBookingShippingDetail.ShipFrom.Country;
                        serviceRequest.ToCountry = directBookingShippingDetail.ShipTo.Country;
                        serviceRequest.OperationZoneId = operationZone.OperationZoneId;
                        bool isEuropeCountry = CheckEuropeCountries(serviceRequest);

                        dbDirectShipment.LogisticType = UtilityRepository.GetLogisticType(operationZone.OperationZoneName, directBookingShippingDetail.ShipFrom.Country.Code, directBookingShippingDetail.ShipTo.Country.Code, isEuropeCountry);
                        if (directBookingShippingDetail.CustomerRateCard != null)
                        {
                            dbDirectShipment.LogisticServiceType = directBookingShippingDetail.CustomerRateCard.LogisticServiceType;
                        }
                        else
                        {
                            dbDirectShipment.LogisticServiceType = null;
                        }

                        if (dbDirectShipment.LogisticType == FrayteLogisticType.UKShipment)
                        {
                            if (directBookingShippingDetail.CustomerRateCard != null)
                            {
                                dbDirectShipment.LogisticServiceType = directBookingShippingDetail.CustomerRateCard.CourierName;
                            }
                            else
                            {
                                dbDirectShipment.LogisticServiceType = null;
                            }
                        }
                        else
                        {
                            if (directBookingShippingDetail.CustomerRateCard != null)
                            {
                                dbDirectShipment.LogisticServiceType = directBookingShippingDetail.CustomerRateCard.CourierName;
                            }
                            else
                            {
                                dbDirectShipment.LogisticServiceType = null;
                            }
                        }

                        if (dbDirectShipment.ShipmentStatusId == DirectBookingShippingStatus.Draft && directBookingShippingDetail.CustomerRateCard != null && directBookingShippingDetail.CustomerRateCard.ZoneRateCardId > 0)
                        {
                            #region -- Update Base Rate --

                            if (directBookingShippingDetail.CustomerRateCard.LogisticDescription == FrayteShipmentType.HeavyWeight)
                            {
                                dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate * directBookingShippingDetail.CustomerRateCard.Weight;
                            }
                            else
                            {
                                dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate;
                            }
                            dbDirectShipment.Margin = (dbDirectShipment.BaseRate) * (directBookingShippingDetail.CustomerRateCard.MarginPercent / 100);
                            dbDirectShipment.FuelSurCharge = (dbDirectShipment.BaseRate + dbDirectShipment.Margin + Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge)) * (decimal)(directBookingShippingDetail.CustomerRateCard.FuelSurcharge / 100);
                            dbDirectShipment.ShipmentTypeId = directBookingShippingDetail.CustomerRateCard.LogisticShipmentId;
                            dbDirectShipment.CourierAccountId = directBookingShippingDetail.CustomerRateCard.CourierAccountId;

                            dbDirectShipment.AdditionalSurcharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge);
                            //dbDirectShipment.FuelMonthYear = directBookingShippingDetail.CustomerRateCard.FuelDate;
                            dbDirectShipment.FuelMonthYear = DateTime.UtcNow.Date;
                            dbDirectShipment.FuelSurchargePercent = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurcharge);

                            #endregion
                        }
                        else if (dbDirectShipment.ShipmentStatusId == DirectBookingShippingStatus.Current && directBookingShippingDetail.CustomerRateCard != null && directBookingShippingDetail.CustomerRateCard.ZoneRateCardId > 0)
                        {
                            #region -- Update Base Rate --

                            if (directBookingShippingDetail.CustomerRateCard.LogisticDescription == FrayteShipmentType.HeavyWeight)
                            {
                                dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate * directBookingShippingDetail.CustomerRateCard.Weight;
                            }
                            else
                            {
                                dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate;
                            }
                            dbDirectShipment.Margin = (dbDirectShipment.BaseRate) * (directBookingShippingDetail.CustomerRateCard.MarginPercent / 100);
                            dbDirectShipment.FuelSurCharge = (dbDirectShipment.BaseRate + dbDirectShipment.Margin + Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge)) * (decimal)(directBookingShippingDetail.CustomerRateCard.FuelSurcharge / 100);
                            dbDirectShipment.ShipmentTypeId = directBookingShippingDetail.CustomerRateCard.LogisticShipmentId;
                            dbDirectShipment.CourierAccountId = directBookingShippingDetail.CustomerRateCard.CourierAccountId;

                            dbDirectShipment.AdditionalSurcharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge);
                            dbDirectShipment.FuelMonthYear = directBookingShippingDetail.CustomerRateCard.FuelDate;
                            dbDirectShipment.FuelSurchargePercent = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurcharge);

                            #endregion
                        }
                        else if (dbDirectShipment.ShipmentStatusId == DirectBookingShippingStatus.Cancel && directBookingShippingDetail.CustomerRateCard != null && directBookingShippingDetail.CustomerRateCard.ZoneRateCardId > 0)
                        {
                            #region -- Update Base Raet --

                            if (directBookingShippingDetail.CustomerRateCard.LogisticDescription == FrayteShipmentType.HeavyWeight)
                            {
                                dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate * directBookingShippingDetail.CustomerRateCard.Weight;
                            }
                            else
                            {
                                dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate;
                            }
                            dbDirectShipment.Margin = (dbDirectShipment.BaseRate) * (directBookingShippingDetail.CustomerRateCard.MarginPercent / 100);
                            dbDirectShipment.FuelSurCharge = (dbDirectShipment.BaseRate + dbDirectShipment.Margin + Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge)) * (decimal)(directBookingShippingDetail.CustomerRateCard.FuelSurcharge / 100);
                            dbDirectShipment.ShipmentTypeId = directBookingShippingDetail.CustomerRateCard.LogisticShipmentId;
                            dbDirectShipment.CourierAccountId = directBookingShippingDetail.CustomerRateCard.CourierAccountId;

                            dbDirectShipment.AdditionalSurcharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge);
                            dbDirectShipment.FuelMonthYear = directBookingShippingDetail.CustomerRateCard.FuelDate;
                            dbDirectShipment.FuelSurchargePercent = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurcharge);

                            #endregion
                        }

                        //Set Reference Detail
                        dbDirectShipment.Reference1 = directBookingShippingDetail.ReferenceDetail.Reference1;
                        dbDirectShipment.ContentDescription = directBookingShippingDetail.ReferenceDetail.ContentDescription;
                        dbDirectShipment.SpecialInstruction = directBookingShippingDetail.ReferenceDetail.SpecialInstruction;
                        if (directBookingShippingDetail.CustomerRateCard != null && (directBookingShippingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UKMail || directBookingShippingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Yodel || directBookingShippingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Hermes))
                        {
                            //dbDirectShipment.CollectionDate = null;
                            //dbDirectShipment.CollectionTime = null;
                        }
                        else
                        {
                            try
                            {
                                if (directBookingShippingDetail.ReferenceDetail.CollectionDate.HasValue)
                                {
                                    dbDirectShipment.CollectionDate = directBookingShippingDetail.ReferenceDetail.CollectionDate.HasValue ? UtilityRepository.ConvertToUniversalTimeWitDate(directBookingShippingDetail.ReferenceDetail.CollectionTime, directBookingShippingDetail.ReferenceDetail.CollectionDate.Value, TZ) : DateTime.UtcNow;
                                    dbDirectShipment.CollectionTime = UtilityRepository.TimeSpanConversion(dbDirectShipment.CollectionDate.Value.Hour, dbDirectShipment.CollectionDate.Value.Minute, dbDirectShipment.CollectionDate.Value.Second);
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                        }
                        dbDirectShipment.CustomerId = directBookingShippingDetail.CustomerId;
                        dbDirectShipment.FromAddressId = directBookingShippingDetail.ShipFrom.AddressBookId;
                        dbDirectShipment.ToAddressId = directBookingShippingDetail.ShipTo.AddressBookId;
                        dbDirectShipment.IsPODMailSent = false;
                        dbDirectShipment.OpearionZoneId = operationZone.OperationZoneId;
                        dbDirectShipment.ParcelType = directBookingShippingDetail.ParcelType.ParcelType;
                        dbDirectShipment.PaymentPartyTaxAndDuties = directBookingShippingDetail.PayTaxAndDuties;
                        dbDirectShipment.PaymentPartyTaxAndDutiesAccountNo = directBookingShippingDetail.PaymentPartyAccountNumber;
                        dbDirectShipment.TaxAndDutiesAcceptedBy = directBookingShippingDetail.TaxAndDutiesAcceptedBy;
                        dbDirectShipment.CreatedBy = directBookingShippingDetail.CreatedBy;
                        dbDirectShipment.PackageCaculatonType = directBookingShippingDetail.PakageCalculatonType;
                        dbDirectShipment.FrayteNumber = null;
                        directBookingShippingDetail.FrayteNumber = CommonConversion.GetNewFrayteNumber();
                        dbDirectShipment.ModuleType = FrayteShipmentServiceType.DirectBooking;
                        dbDirectShipment.AddressType = directBookingShippingDetail.AddressType;
                        dbDirectShipment.IsPublic = directBookingShippingDetail.IsPublic;
                        dbDirectShipment.SessionId = directBookingShippingDetail.SessionId == 0 ? 0 : directBookingShippingDetail.SessionId;
                        dbDirectShipment.BookingApp = "DirectBooking_SS";
                        if (directBookingShippingDetail.CustomerRateCard != null)
                        {
                            dbDirectShipment.ChargeableWeight = directBookingShippingDetail.CustomerRateCard.Weight;
                        }
                        dbContext.Entry(dbDirectShipment).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                if (dbDirectShipment != null)
                {
                    dbContext.SaveChanges();
                }

                directBookingShippingDetail.DirectShipmentDraftId = dbDirectShipment.DirectShipmentDraftId;
            }
            catch (Exception ex)
            {
                throw (new FrayteApiException("ShipmentDetailError", ex));
            }
        }

        public FrayteLogicalPhysicalPath GetShipmentLogisticlabelPath(int directShipmentId)
        {
            var detail = (from r in dbContext.DirectShipments
                          join u in dbContext.Users on r.CustomerId equals u.UserId
                          join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                          where r.DirectShipmentId == directShipmentId
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

        public string GetLogisticLabel(int directShipmentId)
        {
            try
            {
                return dbContext.DirectShipments.Find(directShipmentId).LogisticLabel;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private void SaveDirectShipmentAddress(DirectBookingDraftCollection shipFrom)
        {
            try
            {
                if (shipFrom.Country.CountryId > 0)
                {
                    if (shipFrom.AddressBookId == 0)
                    {
                        // Step 1.4 : Add address to addressBook if not exist aleady
                        if (shipFrom.AddressType == FrayteFromToAddressType.FromAddress)
                        {
                            var addressBookData = dbContext.AddressBooks.Where(p => p.Address1 == shipFrom.Address &&
                                                                               p.Address2 == shipFrom.Address2 &&
                                                                               p.City == shipFrom.City &&
                                                                               p.State == shipFrom.State &&
                                                                               p.PhoneNo == shipFrom.Phone &&
                                                                               p.Area == shipFrom.Area &&
                                                                               p.CompanyName == shipFrom.CompanyName &&
                                                                               p.ContactFirstName == shipFrom.FirstName &&
                                                                               p.ContactLastName == shipFrom.LastName &&
                                                                               p.CountryId == shipFrom.Country.CountryId &&
                                                                               p.CustomerId == shipFrom.CustomerId &&
                                                                               p.Email == shipFrom.Email &&
                                                                               p.Zip == shipFrom.PostCode &&
                                                                               p.IsActive == true &&
                                                                               p.FromAddress == true).ToList();
                            if (addressBookData != null && addressBookData.Count > 0)
                            {
                                shipFrom.AddressBookId = addressBookData[0].AddressBookId;
                            }
                            else
                            {
                                AddressBook dbShipFrom = new AddressBook();
                                dbShipFrom.CustomerId = shipFrom.CustomerId;
                                dbShipFrom.FromAddress = true;
                                dbShipFrom.ToAddress = false;
                                dbShipFrom.Address1 = shipFrom.Address;
                                dbShipFrom.Address2 = shipFrom.Address2;
                                dbShipFrom.Area = shipFrom.Area;
                                dbShipFrom.City = shipFrom.City;
                                dbShipFrom.CompanyName = shipFrom.CompanyName;
                                dbShipFrom.ContactFirstName = shipFrom.FirstName;
                                dbShipFrom.ContactLastName = shipFrom.LastName;
                                dbShipFrom.CountryId = shipFrom.Country.CountryId;
                                dbShipFrom.Email = shipFrom.Email;
                                dbShipFrom.PhoneNo = shipFrom.Phone;
                                dbShipFrom.State = shipFrom.State;
                                dbShipFrom.Zip = shipFrom.PostCode;
                                dbShipFrom.IsActive = true;
                                dbShipFrom.TableType = FrayteTableType.AddressBook;
                                dbShipFrom.IsFavorites = shipFrom.IsFavorites;
                                dbShipFrom.IsDefault = false;
                                dbContext.AddressBooks.Add(dbShipFrom);
                                if (dbShipFrom != null)
                                {
                                    dbContext.SaveChanges();
                                }
                                shipFrom.AddressBookId = dbShipFrom.AddressBookId;
                            }
                        }
                        else if (shipFrom.AddressType == FrayteFromToAddressType.ToAddress)
                        {
                            var addressBookData = dbContext.AddressBooks.Where(p => p.Address1 == shipFrom.Address &&
                                                                               p.Address2 == shipFrom.Address2 &&
                                                                               p.City == shipFrom.City &&
                                                                               p.State == shipFrom.State &&
                                                                               p.PhoneNo == shipFrom.Phone &&
                                                                               p.Area == shipFrom.Area &&
                                                                               p.CompanyName == shipFrom.CompanyName &&
                                                                               p.ContactFirstName == shipFrom.FirstName &&
                                                                               p.ContactLastName == shipFrom.LastName &&
                                                                               p.CountryId == shipFrom.Country.CountryId &&
                                                                               p.CustomerId == shipFrom.CustomerId &&
                                                                               p.Email == shipFrom.Email &&
                                                                               p.Zip == shipFrom.PostCode &&
                                                                               p.IsActive == true &&
                                                                               p.ToAddress == true).ToList();
                            if (addressBookData != null && addressBookData.Count > 0)
                            {
                                shipFrom.AddressBookId = addressBookData[0].AddressBookId;
                            }
                            else
                            {
                                AddressBook dbShipFrom = new AddressBook();
                                dbShipFrom.CustomerId = shipFrom.CustomerId;
                                dbShipFrom.FromAddress = false;
                                dbShipFrom.ToAddress = true;
                                dbShipFrom.Address1 = shipFrom.Address;
                                dbShipFrom.Address2 = shipFrom.Address2;
                                dbShipFrom.Area = shipFrom.Area;
                                dbShipFrom.City = shipFrom.City;
                                dbShipFrom.CompanyName = shipFrom.CompanyName;
                                dbShipFrom.ContactFirstName = shipFrom.FirstName;
                                dbShipFrom.ContactLastName = shipFrom.LastName;
                                dbShipFrom.CountryId = shipFrom.Country.CountryId;
                                dbShipFrom.Email = shipFrom.Email;
                                dbShipFrom.PhoneNo = shipFrom.Phone;
                                dbShipFrom.State = shipFrom.State;
                                dbShipFrom.Zip = shipFrom.PostCode;
                                dbShipFrom.IsActive = true;
                                dbShipFrom.TableType = FrayteTableType.AddressBook;
                                dbShipFrom.IsFavorites = shipFrom.IsFavorites;
                                dbShipFrom.IsDefault = false;
                                dbContext.AddressBooks.Add(dbShipFrom);
                                if (dbShipFrom != null)
                                {
                                    dbContext.SaveChanges();
                                }
                                shipFrom.AddressBookId = dbShipFrom.AddressBookId;
                            }
                        }
                    }
                    else
                    {
                        AddressBook dbShipFrom = dbContext.AddressBooks.Find(shipFrom.AddressBookId);
                        if (dbShipFrom != null)
                        {
                            dbShipFrom.FromAddress = dbShipFrom.ToAddress == true ? false : true;
                            dbShipFrom.ToAddress = dbShipFrom.FromAddress == true ? false : true;
                            dbShipFrom.CustomerId = shipFrom.CustomerId;
                            dbShipFrom.Address1 = shipFrom.Address;
                            dbShipFrom.Address2 = shipFrom.Address2;
                            dbShipFrom.Area = shipFrom.Area;
                            dbShipFrom.City = shipFrom.City;
                            dbShipFrom.CompanyName = shipFrom.CompanyName;
                            dbShipFrom.ContactFirstName = shipFrom.FirstName;
                            dbShipFrom.ContactLastName = shipFrom.LastName;
                            dbShipFrom.CountryId = shipFrom.Country.CountryId;
                            dbShipFrom.Email = shipFrom.Email;
                            dbShipFrom.PhoneNo = shipFrom.Phone;
                            dbShipFrom.State = shipFrom.State;
                            dbShipFrom.Zip = shipFrom.PostCode;
                            dbShipFrom.IsActive = true;
                            dbShipFrom.TableType = FrayteTableType.AddressBook;
                            dbShipFrom.IsFavorites = shipFrom.IsFavorites == true ? true : false;
                        }
                        if (dbShipFrom != null)
                        {
                            dbContext.Entry(dbShipFrom).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                        shipFrom.AddressBookId = dbShipFrom.AddressBookId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw (new FrayteApiException("AddressIssue", ex));
            }
        }

        private void SaveCustomInformation(DirectBookingShipmentDraftDetail directBookingShippingDetail)
        {
            try
            {
                if (directBookingShippingDetail.ShipFrom.Country.Code == directBookingShippingDetail.ShipTo.Country.Code)
                {
                    var removeShipmentCustomDetail = dbContext.ShipmentCustomDetailDrafts.Where(p => p.ShipmentDraftId == directBookingShippingDetail.DirectShipmentDraftId &&
                                                                                                p.ShipmentServiceType == FrayteShipmentServiceType.DirectBooking).FirstOrDefault();
                    if (removeShipmentCustomDetail != null)
                    {
                        dbContext.ShipmentCustomDetailDrafts.Remove(removeShipmentCustomDetail);
                        dbContext.SaveChanges();
                    }

                    return;
                }

                ShipmentCustomDetailDraft shipmentCustomDetail;

                if (directBookingShippingDetail.CustomInfo != null && directBookingShippingDetail.CustomInfo.ShipmentCustomDetailId > 0)
                {
                    shipmentCustomDetail = dbContext.ShipmentCustomDetailDrafts.Find(directBookingShippingDetail.CustomInfo.ShipmentCustomDetailId);
                    if (shipmentCustomDetail != null)
                    {
                        shipmentCustomDetail.ShipmentDraftId = directBookingShippingDetail.DirectShipmentDraftId;
                        shipmentCustomDetail.ShipmentServiceType = FrayteShipmentServiceType.DirectBooking;

                        if (directBookingShippingDetail.CustomerRateCard != null && directBookingShippingDetail.CustomerRateCard.CourierId > 0)
                        {
                            if (directBookingShippingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UK_EU)
                            {
                                //ParcelHub Custom Details
                                shipmentCustomDetail.CatagoryOfItem = directBookingShippingDetail.CustomInfo.CatagoryOfItem;
                                shipmentCustomDetail.CatagoryOfItemExplanation = directBookingShippingDetail.CustomInfo.CatagoryOfItemExplanation;
                                shipmentCustomDetail.CustomsCertify = directBookingShippingDetail.CustomInfo.CustomsCertify;
                                directBookingShippingDetail.CustomInfo.TermOfTrade = directBookingShippingDetail.PayTaxAndDuties;
                                shipmentCustomDetail.TermOfTrade = directBookingShippingDetail.CustomInfo.TermOfTrade;
                            }
                            else
                            {
                                //EasyPost Custome Details
                                shipmentCustomDetail.ContentsType = directBookingShippingDetail.CustomInfo.ContentsType;
                                shipmentCustomDetail.ContentsExplanation = directBookingShippingDetail.CustomInfo.ContentsExplanation;
                                shipmentCustomDetail.RestrictionType = directBookingShippingDetail.CustomInfo.RestrictionType;
                                shipmentCustomDetail.RestrictionComments = directBookingShippingDetail.CustomInfo.RestrictionComments;
                                shipmentCustomDetail.CustomsCertify = directBookingShippingDetail.CustomInfo.CustomsCertify;
                                shipmentCustomDetail.CustomsSigner = directBookingShippingDetail.CustomInfo.CustomsSigner;
                                shipmentCustomDetail.NonDeliveryOption = directBookingShippingDetail.CustomInfo.NonDeliveryOption;
                                shipmentCustomDetail.EelPfc = directBookingShippingDetail.CustomInfo.EelPfc;
                            }
                        }
                        else
                        {
                            if (directBookingShippingDetail.ShippingMethodId > 0)
                            {
                                var CourierName = dbContext.Couriers.Find(directBookingShippingDetail.ShippingMethodId).CourierName;
                                if (CourierName == FrayteCourierCompany.UK_EU)
                                {
                                    //ParcelHub Custom Details
                                    shipmentCustomDetail.CatagoryOfItem = directBookingShippingDetail.CustomInfo.CatagoryOfItem;
                                    shipmentCustomDetail.CatagoryOfItemExplanation = directBookingShippingDetail.CustomInfo.CatagoryOfItemExplanation;
                                    shipmentCustomDetail.CustomsCertify = directBookingShippingDetail.CustomInfo.CustomsCertify;
                                    directBookingShippingDetail.CustomInfo.TermOfTrade = directBookingShippingDetail.PayTaxAndDuties;
                                    shipmentCustomDetail.TermOfTrade = directBookingShippingDetail.CustomInfo.TermOfTrade;
                                }
                                else
                                {
                                    //EasyPost Custome Details
                                    shipmentCustomDetail.ContentsType = directBookingShippingDetail.CustomInfo.ContentsType;
                                    shipmentCustomDetail.ContentsExplanation = directBookingShippingDetail.CustomInfo.ContentsExplanation;
                                    shipmentCustomDetail.RestrictionType = directBookingShippingDetail.CustomInfo.RestrictionType;
                                    shipmentCustomDetail.RestrictionComments = directBookingShippingDetail.CustomInfo.RestrictionComments;
                                    shipmentCustomDetail.CustomsCertify = directBookingShippingDetail.CustomInfo.CustomsCertify;
                                    shipmentCustomDetail.CustomsSigner = directBookingShippingDetail.CustomInfo.CustomsSigner;
                                    shipmentCustomDetail.NonDeliveryOption = directBookingShippingDetail.CustomInfo.NonDeliveryOption;
                                    shipmentCustomDetail.EelPfc = directBookingShippingDetail.CustomInfo.EelPfc;
                                }
                            }
                        }
                    }
                }
                else
                {
                    shipmentCustomDetail = new ShipmentCustomDetailDraft();
                    shipmentCustomDetail.ShipmentDraftId = directBookingShippingDetail.DirectShipmentDraftId;
                    shipmentCustomDetail.ShipmentServiceType = FrayteShipmentServiceType.DirectBooking;
                    var CourierName = "";
                    if (directBookingShippingDetail.ShippingMethodId > 0)
                    {
                        var Courier = dbContext.Couriers.Find(directBookingShippingDetail.ShippingMethodId);
                        if (Courier != null)
                        {
                            CourierName = Courier.CourierName;
                        }
                    }
                    if (CourierName == FrayteCourierCompany.UK_EU)
                    {
                        //ParcelHub Custom Details
                        shipmentCustomDetail.CatagoryOfItem = directBookingShippingDetail.CustomInfo.CatagoryOfItem;
                        shipmentCustomDetail.CatagoryOfItemExplanation = directBookingShippingDetail.CustomInfo.CatagoryOfItemExplanation;
                        shipmentCustomDetail.CustomsCertify = directBookingShippingDetail.CustomInfo.CustomsCertify;
                        shipmentCustomDetail.TermOfTrade = directBookingShippingDetail.CustomInfo.TermOfTrade;
                        directBookingShippingDetail.CustomInfo.TermOfTrade = directBookingShippingDetail.PayTaxAndDuties;
                        shipmentCustomDetail.TermOfTrade = directBookingShippingDetail.CustomInfo.TermOfTrade;
                    }
                    else
                    {
                        //EasyPost Custome Details
                        shipmentCustomDetail.ContentsType = directBookingShippingDetail.CustomInfo.ContentsType;
                        shipmentCustomDetail.ContentsExplanation = directBookingShippingDetail.CustomInfo.ContentsExplanation;
                        shipmentCustomDetail.RestrictionType = directBookingShippingDetail.CustomInfo.RestrictionType;
                        shipmentCustomDetail.RestrictionComments = directBookingShippingDetail.CustomInfo.RestrictionComments;
                        shipmentCustomDetail.CustomsCertify = directBookingShippingDetail.CustomInfo.CustomsCertify;
                        shipmentCustomDetail.CustomsSigner = directBookingShippingDetail.CustomInfo.CustomsSigner;
                        shipmentCustomDetail.NonDeliveryOption = directBookingShippingDetail.CustomInfo.NonDeliveryOption;
                        shipmentCustomDetail.EelPfc = directBookingShippingDetail.CustomInfo.EelPfc;
                    }

                    dbContext.ShipmentCustomDetailDrafts.Add(shipmentCustomDetail);
                }

                if (shipmentCustomDetail != null)
                {
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw (new FrayteApiException("ShipmentCustomInfoError", ex));
            }
        }

        public List<FraytePackageTrackingDetail> SaveEasyPostDetailTrackingDeatil(DirectBookingShipmentDraftDetail directBookingShippingDetail, PackageDraft package, EasyPost.Shipment Shipment, int DirectShipmentId, int increment)
        {
            FraytePackageTrackingDetail fraytePackageTrackingDetail;
            ShipmentEasyPost EasyPostDeatil;
            List<FraytePackageTrackingDetail> list = new List<FraytePackageTrackingDetail>();
            var shipmentdetail = dbContext.DirectShipmentDetails.Where(p => p.DirectShipmentId == DirectShipmentId).ToList();
            if (shipmentdetail != null && shipmentdetail.Count > 0)
            {
                int ShipmentDetailId = shipmentdetail[increment].DirectShipmentDetailId;
                EasyPostDeatil = dbContext.ShipmentEasyPosts.Where(p => p.ShipmentId == ShipmentDetailId && p.ShipmentServiceType == FrayteShipmentServiceType.DirectBooking).FirstOrDefault();
                if (EasyPostDeatil != null)
                {
                    EasyPostDeatil.ShipmentId = shipmentdetail[increment].DirectShipmentDetailId;
                    EasyPostDeatil.EasyPostShipmentId = Shipment.id;
                    EasyPostDeatil.ShipmentServiceType = FrayteShipmentServiceType.DirectBooking;
                    EasyPostDeatil.TrackingCode = Shipment.tracking_code;
                    if (Shipment.tracker != null)
                    {
                        EasyPostDeatil.Carrier = Shipment.tracker.carrier;
                    }
                    if (Shipment.postage_label != null)
                    {
                        EasyPostDeatil.PostageLabel = Shipment.postage_label.label_url;
                    }
                    EasyPostDeatil.BarcodeURL = Shipment.barcode_url;
                    EasyPostDeatil.BatchMessage = Shipment.batch_message;
                    EasyPostDeatil.BatchStatus = Shipment.batch_status;
                    EasyPostDeatil.CreatedAt = Shipment.created_at;
                    EasyPostDeatil.StampURL = Shipment.stamp_url;

                    dbContext.SaveChanges();
                }
                else
                {
                    EasyPostDeatil = new ShipmentEasyPost();
                    EasyPostDeatil.ShipmentId = shipmentdetail[increment].DirectShipmentDetailId;
                    EasyPostDeatil.EasyPostShipmentId = Shipment.id;
                    EasyPostDeatil.ShipmentServiceType = FrayteShipmentServiceType.DirectBooking;
                    EasyPostDeatil.TrackingCode = Shipment.tracking_code;
                    if (Shipment.tracker != null)
                    {
                        EasyPostDeatil.Carrier = Shipment.tracker.carrier;
                    }
                    if (Shipment.postage_label != null)
                    {
                        EasyPostDeatil.PostageLabel = Shipment.postage_label.label_url;
                    }
                    EasyPostDeatil.BarcodeURL = Shipment.barcode_url;
                    EasyPostDeatil.BatchMessage = Shipment.batch_message;
                    EasyPostDeatil.BatchStatus = Shipment.batch_status;
                    EasyPostDeatil.CreatedAt = Shipment.created_at;
                    EasyPostDeatil.StampURL = Shipment.stamp_url;
                    dbContext.ShipmentEasyPosts.Add(EasyPostDeatil);
                    dbContext.SaveChanges();
                }


                for (int i = 0; i < shipmentdetail[increment].CartoonValue; i++)
                {
                    PackageTrackingDetail packageTrackingDetail = new PackageTrackingDetail();
                    packageTrackingDetail.DirectShipmentDetailId = shipmentdetail[increment].DirectShipmentDetailId;
                    packageTrackingDetail.TrackingNo = Shipment.tracking_code;
                    dbContext.PackageTrackingDetails.Add(packageTrackingDetail);
                    dbContext.SaveChanges();

                    fraytePackageTrackingDetail = new FraytePackageTrackingDetail();
                    fraytePackageTrackingDetail.DirectShipmentDetailId = shipmentdetail[increment].DirectShipmentDetailId;
                    fraytePackageTrackingDetail.PackageTrackingDetailId = packageTrackingDetail.PackageTrackingDetailId;
                    fraytePackageTrackingDetail.TrackingNo = Shipment.tracking_code;
                    fraytePackageTrackingDetail.PackageImage = "";
                    fraytePackageTrackingDetail.LabelUrl = Shipment.postage_label.label_url;
                    fraytePackageTrackingDetail.IsDownloaded = false;
                    fraytePackageTrackingDetail.IsPrinted = false;
                    list.Add(fraytePackageTrackingDetail);
                }
                //Finally update the label            

            }
            return list;
        }

        #endregion

        public string DownloadEasyPostImages(List<FraytePackageTrackingDetail> shipmentPackageTrackingDetail, int DirectShipmentId)
        {
            var label = new DirectShipmentRepository().EasyPostPackageImageName(shipmentPackageTrackingDetail[0].TrackingNo, shipmentPackageTrackingDetail[0].LabelUrl, DirectShipmentId);
            return label;
        }

        public string EasyPostPackageImageName(string TrackingNo, string PostageUrl, int DirectShipmentId)
        {
            string ImageName = string.Empty;
            if (!string.IsNullOrEmpty(PostageUrl))
            {
                if (PostageUrl.Contains("https"))
                {
                    if (AppSettings.LabelSave == "")
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            byte[] data = webClient.DownloadData(PostageUrl);
                            System.Drawing.Image labelimage;
                            using (MemoryStream mem = new MemoryStream(data))
                            {
                                labelimage = System.Drawing.Image.FromStream(mem);
                                ImageName = TrackingNo + ".jpg";
                                System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentId + "/");
                                labelimage.Save(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentId + "/" + ImageName);
                            }
                        }
                    }
                    else
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            byte[] data = webClient.DownloadData(PostageUrl);
                            System.Drawing.Image labelimage;
                            using (MemoryStream mem = new MemoryStream(data))
                            {
                                labelimage = System.Drawing.Image.FromStream(mem);
                                ImageName = TrackingNo + ".jpg";
                                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.LabelFolder + DirectShipmentId + "/"));
                                labelimage.Save(HttpContext.Current.Server.MapPath(AppSettings.LabelFolder + DirectShipmentId + "/") + ImageName);
                            }
                        }
                    }
                }
            }
            return ImageName;
        }

        #region -- TNT Integration --

        public List<FraytePackageTrackingDetail> SaveTNTTrackingDeatil(string trackingCode, int directShipmentid, int increment)
        {
            FrayteResult result = new FrayteResult();
            List<FraytePackageTrackingDetail> fraytePackageTrackingDetail = new List<FraytePackageTrackingDetail>();
            try
            {
                var shipmentdetail = dbContext.DirectShipmentDetails.Where(p => p.DirectShipmentId == directShipmentid).ToList();
                FraytePackageTrackingDetail trackingDetail;
                PackageTrackingDetail packageTrackingDetail = new PackageTrackingDetail();
                for (int i = 0; i < shipmentdetail[increment].CartoonValue; i++)
                {
                    packageTrackingDetail.DirectShipmentDetailId = shipmentdetail[increment].DirectShipmentDetailId;
                    packageTrackingDetail.TrackingNo = trackingCode;
                    dbContext.PackageTrackingDetails.Add(packageTrackingDetail);
                    dbContext.SaveChanges();

                    trackingDetail = new FraytePackageTrackingDetail();
                    trackingDetail.DirectShipmentDetailId = shipmentdetail[increment].DirectShipmentDetailId;
                    trackingDetail.PackageTrackingDetailId = packageTrackingDetail.PackageTrackingDetailId;
                    trackingDetail.TrackingNo = trackingCode;
                    trackingDetail.PackageImage = "";
                    trackingDetail.LabelUrl = "";
                    trackingDetail.IsDownloaded = false;
                    trackingDetail.IsPrinted = false;
                    fraytePackageTrackingDetail.Add(trackingDetail);
                }

                result.Status = true;
            }
            catch (Exception ex)
            {
                result.Status = false;
            }
            if (result.Status)
            {
                return fraytePackageTrackingDetail;
            }
            else
            {
                return null;
            }
        }

        public void SaveImage(FraytePackageTrackingDetail data, string image)
        {
            var PackageTracking = dbContext.PackageTrackingDetails.Find(data.PackageTrackingDetailId);
            PackageTracking.PackageImage = image;
            PackageTracking.IsDownloaded = true;
            PackageTracking.IsPrinted = false;
            dbContext.SaveChanges();
        }

        #endregion

        public List<FrayteUserDirectShipment> GetDirectShipment(FrayteTrackDirectBooking track)
        {
            int TotalRows = 0;
            int SkipRows = 0;
            SkipRows = (track.CurrentPage - 1) * track.TakeRows;
            DateTime? fromdate;
            DateTime? todate;

            if (track.FromDate.HasValue)
            {
                fromdate = track.FromDate.Value.Date;
            }
            else
            {
                fromdate = track.FromDate;
            }

            if (track.ToDate.HasValue)
            {
                todate = track.ToDate.Value.Date;
            }
            else
            {
                todate = track.ToDate;
            }

            List<FrayteUserDirectShipment> lstUserShipment = new List<FrayteUserDirectShipment>();
            if (track != null)
            {
                FrayteOperationZone OperationZone = UtilityRepository.GetOperationZone();
                int RoleId = dbContext.UserRoles.Where(p => p.UserId == (track.UserId > 0 ? track.UserId : track.CustomerId)).FirstOrDefault().RoleId;

                #region Admin DirectShipments

                if (RoleId == (int)FrayteUserRole.Admin || RoleId == (int)FrayteUserRole.MasterAdmin)
                {
                    if (track.FromDate == null && track.ToDate == null)
                    {
                        #region
                        //track.UserId = 0;
                        var result = dbContext.spGet_TrackDirectBookingDetail(track.BookingMethod, track.eCommerceShipmentType, fromdate, todate, track.ShipmentStatusId, track.FrayteNumber.Trim(), track.TrackingNo.Trim(), track.LogisticType,
                                                                              SkipRows, track.TakeRows, track.CustomerId, track.LogisticServiceType, track.UserId, OperationZone.OperationZoneId, RoleId, track.CallingFrom).ToList();
                        if (result != null && result.Count > 0)
                        {
                            TotalRows = result[0].TotalRows == null ? 0 : result[0].TotalRows.Value;
                        }
                        else
                        {
                            TotalRows = 0;
                        }

                        if (result != null)
                        {
                            FrayteUserDirectShipment frayte;
                            foreach (var detail in result)
                            {
                                frayte = new FrayteUserDirectShipment();
                                frayte.ShipmentId = detail.DirectShipmentId;
                                frayte.ShipmentCode = "";
                                frayte.Customer = detail.ContactName;
                                frayte.ShippedFromCompany = detail.FromCompany;
                                frayte.ShippedToCompany = detail.ToCompany;
                                frayte.ShippingBy = detail.LogisticCompany;
                                frayte.DisplayName = detail.LogisticCompanyDisplay;
                                frayte.RateType = detail.RateType;
                                frayte.RateTypeDisplay = detail.RateTypeDisplay;
                                frayte.ShippingDate = detail.CreatedOn.HasValue ? detail.CreatedOn.Value : DateTime.UtcNow;
                                frayte.Reference1 = detail.Reference1;
                                frayte.FrayteNumber = detail.FrayteNumber;
                                frayte.Status = detail.StatusName;
                                frayte.DisplayStatus = detail.DisplayStatusName;
                                frayte.TrackingNo = detail.TrackingNo;
                                frayte.TotalRows = TotalRows;
                                frayte.LogisticType = detail.LogisticType;
                                frayte.LogisticTypeDisplay = detail.LogisticTypeDisplay;
                                frayte.BookingApp = detail.BookingApp;
                                if (track.BookingMethod == FrayteShipmentServiceType.eCommerce)
                                {
                                    if (detail.BookingApp == eCommerceShipmentType.eCommerceWS)
                                        frayte.IsTrackingShow = false;
                                    else
                                        frayte.IsTrackingShow = true;
                                }
                                else
                                {
                                    frayte.IsTrackingShow = true;
                                }

                                frayte.ManifestId = detail.ManifestId == null ? 0 : detail.ManifestId.Value;
                                lstUserShipment.Add(frayte);
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        #region
                        //track.UserId = 0;
                        var result = dbContext.spGet_TrackDirectBookingDetail(track.BookingMethod, track.eCommerceShipmentType, fromdate, todate, track.ShipmentStatusId, track.FrayteNumber.Trim(), track.TrackingNo.Trim(), track.LogisticType,
                                                                              SkipRows, track.TakeRows, track.CustomerId, track.LogisticServiceType, track.UserId, OperationZone.OperationZoneId, RoleId, track.CallingFrom).ToList();

                        if (result != null && result.Count > 0)
                        {
                            TotalRows = result[0].TotalRows == null ? 0 : result[0].TotalRows.Value;
                        }
                        else
                        {
                            TotalRows = 0;
                        }

                        if (result != null)
                        {
                            FrayteUserDirectShipment frayte;
                            foreach (var detail in result)
                            {
                                frayte = new FrayteUserDirectShipment();
                                frayte.ShipmentId = detail.DirectShipmentId;
                                frayte.ShipmentCode = "";
                                frayte.Customer = detail.ContactName;
                                frayte.ShippedFromCompany = detail.FromCompany;
                                frayte.ShippedToCompany = detail.ToCompany;
                                frayte.ShippingBy = detail.LogisticCompany;
                                frayte.DisplayName = detail.LogisticCompanyDisplay;
                                frayte.RateType = detail.RateType;
                                frayte.RateTypeDisplay = detail.RateTypeDisplay;
                                frayte.ShippingDate = detail.CreatedOn.HasValue ? detail.CreatedOn.Value : DateTime.UtcNow;
                                frayte.Reference1 = detail.Reference1;
                                frayte.FrayteNumber = detail.FrayteNumber;
                                frayte.Status = detail.StatusName;
                                frayte.DisplayStatus = detail.DisplayStatusName;
                                frayte.TrackingNo = detail.TrackingNo;
                                frayte.TotalRows = TotalRows;
                                frayte.ManifestId = detail.ManifestId == null ? 0 : detail.ManifestId.Value;
                                frayte.LogisticType = detail.LogisticType;
                                frayte.LogisticTypeDisplay = detail.LogisticTypeDisplay;
                                frayte.BookingApp = detail.BookingApp;
                                if (track.BookingMethod == FrayteShipmentServiceType.eCommerce)
                                {
                                    if (detail.BookingApp == eCommerceShipmentType.eCommerceWS)
                                        frayte.IsTrackingShow = false;
                                    else
                                        frayte.IsTrackingShow = true;
                                }
                                else
                                {
                                    frayte.IsTrackingShow = true;
                                }
                                lstUserShipment.Add(frayte);
                            }
                        }

                        #endregion
                    }
                }

                #endregion

                #region Customer DirectShipments

                if (RoleId == (int)FrayteUserRole.Customer)
                {
                    lstUserShipment = new List<FrayteUserDirectShipment>();
                    if (track.FromDate == null && track.ToDate == null)
                    {
                        #region

                        var result = dbContext.spGet_TrackDirectBookingDetail(track.BookingMethod, track.eCommerceShipmentType, fromdate, todate, track.ShipmentStatusId, track.FrayteNumber.Trim(), track.TrackingNo.Trim(), track.LogisticType,
                                                                              SkipRows, track.TakeRows, track.CustomerId, track.LogisticServiceType, track.UserId, OperationZone.OperationZoneId, RoleId, track.CallingFrom).ToList();

                        if (result != null && result.Count > 0)
                        {
                            TotalRows = result[0].TotalRows == null ? 0 : result[0].TotalRows.Value;
                        }
                        else
                        {
                            TotalRows = 0;
                        }

                        if (result != null)
                        {
                            FrayteUserDirectShipment frayte;
                            foreach (var detail in result)
                            {
                                frayte = new FrayteUserDirectShipment();
                                frayte.ShipmentId = detail.DirectShipmentId;
                                frayte.ShipmentCode = "";
                                frayte.Customer = detail.ContactName;
                                frayte.ShippedFromCompany = detail.FromCompany;
                                frayte.ShippedToCompany = detail.ToCompany;
                                frayte.ShippingBy = detail.LogisticCompany;
                                frayte.DisplayName = detail.LogisticCompanyDisplay;
                                frayte.RateType = detail.RateType;
                                frayte.RateTypeDisplay = detail.RateTypeDisplay;
                                frayte.ShippingDate = detail.CreatedOn.HasValue ? detail.CreatedOn.Value : DateTime.UtcNow;
                                frayte.Reference1 = detail.Reference1;
                                frayte.FrayteNumber = detail.FrayteNumber;
                                frayte.Status = detail.StatusName;
                                frayte.DisplayStatus = detail.DisplayStatusName;
                                frayte.TrackingNo = detail.TrackingNo;
                                frayte.TotalRows = TotalRows;
                                frayte.ManifestId = detail.ManifestId == null ? 0 : detail.ManifestId.Value;
                                frayte.LogisticType = detail.LogisticType;
                                frayte.LogisticTypeDisplay = detail.LogisticTypeDisplay;
                                frayte.BookingApp = detail.BookingApp;
                                if (track.BookingMethod == FrayteShipmentServiceType.eCommerce)
                                {
                                    if (detail.BookingApp == eCommerceShipmentType.eCommerceWS)
                                        frayte.IsTrackingShow = false;
                                    else
                                        frayte.IsTrackingShow = true;
                                }
                                else
                                {
                                    frayte.IsTrackingShow = true;
                                }
                                lstUserShipment.Add(frayte);
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        #region

                        var result = dbContext.spGet_TrackDirectBookingDetail(track.BookingMethod, track.eCommerceShipmentType, fromdate, todate, track.ShipmentStatusId, track.FrayteNumber.Trim(), track.TrackingNo.Trim(), track.LogisticType,
                                                                              SkipRows, track.TakeRows, track.CustomerId, track.LogisticServiceType, track.UserId, OperationZone.OperationZoneId, RoleId, track.CallingFrom).ToList();

                        if (result != null && result.Count > 0)
                        {
                            TotalRows = result[0].TotalRows == null ? 0 : result[0].TotalRows.Value;
                        }
                        else
                        {
                            TotalRows = 0;
                        }

                        if (result != null)
                        {
                            FrayteUserDirectShipment frayte;
                            foreach (var detail in result)
                            {
                                frayte = new FrayteUserDirectShipment();
                                frayte.ShipmentId = detail.DirectShipmentId;
                                frayte.ShipmentCode = "";
                                frayte.Customer = detail.ContactName;
                                frayte.ShippedFromCompany = detail.FromCompany;
                                frayte.ShippedToCompany = detail.ToCompany;
                                frayte.ShippingBy = detail.LogisticCompany;
                                frayte.DisplayName = detail.LogisticCompanyDisplay;
                                frayte.RateType = detail.RateType;
                                frayte.RateTypeDisplay = detail.RateTypeDisplay;
                                frayte.ShippingDate = detail.CreatedOn.HasValue ? detail.CreatedOn.Value : DateTime.UtcNow;
                                frayte.Reference1 = detail.Reference1;
                                frayte.FrayteNumber = detail.FrayteNumber;
                                frayte.Status = detail.StatusName;
                                frayte.DisplayStatus = detail.DisplayStatusName;
                                frayte.TrackingNo = detail.TrackingNo;
                                frayte.TotalRows = TotalRows;
                                frayte.ManifestId = detail.ManifestId == null ? 0 : detail.ManifestId.Value;
                                frayte.LogisticType = detail.LogisticType;
                                frayte.LogisticTypeDisplay = detail.LogisticTypeDisplay;
                                frayte.BookingApp = detail.BookingApp;
                                if (track.BookingMethod == FrayteShipmentServiceType.eCommerce)
                                {
                                    if (detail.BookingApp == eCommerceShipmentType.eCommerceWS)
                                        frayte.IsTrackingShow = false;
                                    else
                                        frayte.IsTrackingShow = true;
                                }
                                else
                                {
                                    frayte.IsTrackingShow = true;
                                }
                                lstUserShipment.Add(frayte);
                            }
                        }

                        #endregion
                    }
                }

                #endregion

                #region User DirectShipments

                if (RoleId == (int)FrayteUserRole.Staff)
                {

                    lstUserShipment = new List<FrayteUserDirectShipment>();
                    if (track.FromDate == null && track.ToDate == null)
                    {
                        #region
                        //  track.UserId = 0;
                        var result = dbContext.spGet_TrackDirectBookingDetail(track.BookingMethod, track.eCommerceShipmentType, fromdate, todate, track.ShipmentStatusId, track.FrayteNumber.Trim(), track.TrackingNo.Trim(), track.LogisticType,
                                                                              SkipRows, track.TakeRows, track.CustomerId, track.LogisticServiceType, track.UserId, OperationZone.OperationZoneId, RoleId, track.CallingFrom).ToList();

                        if (result != null && result.Count > 0)
                        {
                            TotalRows = result[0].TotalRows == null ? 0 : result[0].TotalRows.Value;
                        }
                        else
                        {
                            TotalRows = 0;
                        }

                        if (result != null)
                        {
                            FrayteUserDirectShipment frayte;
                            foreach (var detail in result)
                            {
                                frayte = new FrayteUserDirectShipment();
                                frayte.ShipmentId = detail.DirectShipmentId;
                                frayte.ShipmentCode = "";
                                frayte.Customer = detail.ContactName;
                                frayte.ShippedFromCompany = detail.FromCompany;
                                frayte.ShippedToCompany = detail.ToCompany;
                                frayte.ShippingBy = detail.LogisticCompany;
                                frayte.DisplayName = detail.LogisticCompanyDisplay;
                                frayte.RateType = detail.RateType;
                                frayte.RateTypeDisplay = detail.RateTypeDisplay;
                                frayte.ShippingDate = detail.CreatedOn.HasValue ? detail.CreatedOn.Value : DateTime.UtcNow;
                                frayte.Reference1 = detail.Reference1;
                                frayte.FrayteNumber = detail.FrayteNumber;
                                frayte.Status = detail.StatusName;
                                frayte.DisplayStatus = detail.DisplayStatusName;
                                frayte.TrackingNo = detail.TrackingNo;
                                frayte.TotalRows = TotalRows;
                                frayte.ManifestId = detail.ManifestId == null ? 0 : detail.ManifestId.Value;
                                frayte.LogisticType = detail.LogisticType;
                                frayte.LogisticTypeDisplay = detail.LogisticTypeDisplay;
                                frayte.BookingApp = detail.BookingApp;
                                if (track.BookingMethod == FrayteShipmentServiceType.eCommerce)
                                {
                                    if (detail.BookingApp == eCommerceShipmentType.eCommerceWS)
                                        frayte.IsTrackingShow = false;
                                    else
                                        frayte.IsTrackingShow = true;
                                }
                                else
                                {
                                    frayte.IsTrackingShow = true;
                                }
                                lstUserShipment.Add(frayte);
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        #region
                        //   track.UserId = 0;
                        var result = dbContext.spGet_TrackDirectBookingDetail(track.BookingMethod, track.eCommerceShipmentType, fromdate, todate, track.ShipmentStatusId, track.FrayteNumber.Trim(), track.TrackingNo.Trim(), track.LogisticType,
                                                                              SkipRows, track.TakeRows, track.CustomerId, track.LogisticServiceType, track.UserId, OperationZone.OperationZoneId, RoleId, track.CallingFrom).ToList();

                        if (result != null && result.Count > 0)
                        {
                            TotalRows = result[0].TotalRows == null ? 0 : result[0].TotalRows.Value;
                        }
                        else
                        {
                            TotalRows = 0;
                        }

                        if (result != null)
                        {
                            FrayteUserDirectShipment frayte;
                            foreach (var detail in result)
                            {
                                frayte = new FrayteUserDirectShipment();
                                frayte.ShipmentId = detail.DirectShipmentId;
                                frayte.ShipmentCode = "";
                                frayte.Customer = detail.ContactName;
                                frayte.ShippedFromCompany = detail.FromCompany;
                                frayte.ShippedToCompany = detail.ToCompany;
                                frayte.ShippingBy = detail.LogisticCompany;
                                frayte.DisplayName = detail.LogisticCompanyDisplay;
                                frayte.RateType = detail.RateType;
                                frayte.RateTypeDisplay = detail.RateTypeDisplay;
                                frayte.ShippingDate = detail.CreatedOn.HasValue ? detail.CreatedOn.Value : DateTime.UtcNow;
                                frayte.Reference1 = detail.Reference1;
                                frayte.FrayteNumber = detail.FrayteNumber;
                                frayte.Status = detail.StatusName;
                                frayte.DisplayStatus = detail.DisplayStatusName;
                                frayte.TrackingNo = detail.TrackingNo;
                                frayte.TotalRows = TotalRows;
                                frayte.ManifestId = detail.ManifestId == null ? 0 : detail.ManifestId.Value;
                                frayte.LogisticType = detail.LogisticType;
                                frayte.LogisticTypeDisplay = detail.LogisticTypeDisplay;
                                frayte.BookingApp = detail.BookingApp;
                                if (track.BookingMethod == FrayteShipmentServiceType.eCommerce)
                                {
                                    if (detail.BookingApp == eCommerceShipmentType.eCommerceWS)
                                        frayte.IsTrackingShow = false;
                                    else
                                        frayte.IsTrackingShow = true;
                                }
                                else
                                {
                                    frayte.IsTrackingShow = true;
                                }
                                lstUserShipment.Add(frayte);
                            }
                        }

                        #endregion
                    }
                }

                #endregion

                #region UserCustomer DirectShipments

                if (RoleId == (int)FrayteUserRole.UserCustomer)
                {

                    lstUserShipment = new List<FrayteUserDirectShipment>();
                    if (track.FromDate == null && track.ToDate == null)
                    {
                        #region

                        var result = dbContext.spGet_TrackDirectBookingDetail(track.BookingMethod, track.eCommerceShipmentType, fromdate, todate, track.ShipmentStatusId, track.FrayteNumber.Trim(), track.TrackingNo.Trim(), track.LogisticType,
                                                                              SkipRows, track.TakeRows, track.CustomerId, track.LogisticServiceType, track.UserId, OperationZone.OperationZoneId, RoleId, track.CallingFrom).ToList();

                        if (result != null && result.Count > 0)
                        {
                            TotalRows = result[0].TotalRows == null ? 0 : result[0].TotalRows.Value;
                        }
                        else
                        {
                            TotalRows = 0;
                        }

                        if (result != null)
                        {
                            FrayteUserDirectShipment frayte;
                            foreach (var detail in result)
                            {
                                frayte = new FrayteUserDirectShipment();
                                frayte.ShipmentId = detail.DirectShipmentId;
                                frayte.ShipmentCode = "";
                                frayte.Customer = detail.ContactName;
                                frayte.ShippedFromCompany = detail.FromCompany;
                                frayte.ShippedToCompany = detail.ToCompany;
                                frayte.ShippingBy = detail.LogisticCompany;
                                frayte.DisplayName = detail.LogisticCompanyDisplay;
                                frayte.RateType = detail.RateType;
                                frayte.RateTypeDisplay = detail.RateTypeDisplay;
                                frayte.ShippingDate = detail.CreatedOn.HasValue ? detail.CreatedOn.Value : DateTime.UtcNow;
                                frayte.Reference1 = detail.Reference1;
                                frayte.FrayteNumber = detail.FrayteNumber;
                                frayte.Status = detail.StatusName;
                                frayte.DisplayStatus = detail.DisplayStatusName;
                                frayte.TrackingNo = detail.TrackingNo;
                                frayte.TotalRows = TotalRows;
                                frayte.ManifestId = detail.ManifestId == null ? 0 : detail.ManifestId.Value;
                                frayte.LogisticType = detail.LogisticType;
                                frayte.LogisticTypeDisplay = detail.LogisticTypeDisplay;
                                frayte.BookingApp = detail.BookingApp;
                                if (track.BookingMethod == FrayteShipmentServiceType.eCommerce)
                                {
                                    if (detail.BookingApp == eCommerceShipmentType.eCommerceWS)
                                        frayte.IsTrackingShow = false;
                                    else
                                        frayte.IsTrackingShow = true;
                                }
                                else
                                {
                                    frayte.IsTrackingShow = true;
                                }
                                lstUserShipment.Add(frayte);
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        #region

                        var result = dbContext.spGet_TrackDirectBookingDetail(track.BookingMethod, track.eCommerceShipmentType, fromdate, todate, track.ShipmentStatusId, track.FrayteNumber.Trim(), track.TrackingNo.Trim(), track.LogisticType,
                                                                              SkipRows, track.TakeRows, track.CustomerId, track.LogisticServiceType, track.UserId, OperationZone.OperationZoneId, RoleId, track.CallingFrom).ToList();

                        if (result != null && result.Count > 0)
                        {
                            TotalRows = result[0].TotalRows == null ? 0 : result[0].TotalRows.Value;
                        }
                        else
                        {
                            TotalRows = 0;
                        }

                        if (result != null)
                        {
                            FrayteUserDirectShipment frayte;
                            foreach (var detail in result)
                            {
                                frayte = new FrayteUserDirectShipment();
                                frayte.ShipmentId = detail.DirectShipmentId;
                                frayte.ShipmentCode = "";
                                frayte.Customer = detail.ContactName;
                                frayte.ShippedFromCompany = detail.FromCompany;
                                frayte.ShippedToCompany = detail.ToCompany;
                                frayte.ShippingBy = detail.LogisticCompany;
                                frayte.DisplayName = detail.LogisticCompanyDisplay;
                                frayte.RateType = detail.RateType;
                                frayte.RateTypeDisplay = detail.RateTypeDisplay;
                                frayte.ShippingDate = detail.CreatedOn.HasValue ? detail.CreatedOn.Value : DateTime.UtcNow;
                                frayte.Reference1 = detail.Reference1;
                                frayte.FrayteNumber = detail.FrayteNumber;
                                frayte.Status = detail.StatusName;
                                frayte.DisplayStatus = detail.DisplayStatusName;
                                frayte.TrackingNo = detail.TrackingNo;
                                frayte.TotalRows = TotalRows;
                                frayte.ManifestId = detail.ManifestId == null ? 0 : detail.ManifestId.Value;
                                frayte.LogisticType = detail.LogisticType;
                                frayte.LogisticTypeDisplay = detail.LogisticTypeDisplay;
                                frayte.BookingApp = detail.BookingApp;
                                if (track.BookingMethod == FrayteShipmentServiceType.eCommerce)
                                {
                                    if (detail.BookingApp == eCommerceShipmentType.eCommerceWS)
                                        frayte.IsTrackingShow = false;
                                    else
                                        frayte.IsTrackingShow = true;
                                }
                                else
                                {
                                    frayte.IsTrackingShow = true;
                                }
                                lstUserShipment.Add(frayte);
                            }
                        }

                        #endregion
                    }
                }

                #endregion

                #region CustomerStaff DirectShipments

                if (RoleId == (int)FrayteUserRole.CustomerStaff)
                {

                    lstUserShipment = new List<FrayteUserDirectShipment>();
                    if (track.FromDate == null && track.ToDate == null)
                    {
                        #region

                        var result = dbContext.spGet_TrackDirectBookingDetail(track.BookingMethod, track.eCommerceShipmentType, fromdate, todate, track.ShipmentStatusId, track.FrayteNumber.Trim(), track.TrackingNo.Trim(), track.LogisticType,
                                                                              SkipRows, track.TakeRows, track.CustomerId, track.LogisticServiceType, track.UserId, OperationZone.OperationZoneId, RoleId, track.CallingFrom).ToList();

                        if (result != null && result.Count > 0)
                        {
                            TotalRows = result[0].TotalRows == null ? 0 : result[0].TotalRows.Value;
                        }
                        else
                        {
                            TotalRows = 0;
                        }

                        if (result != null)
                        {
                            FrayteUserDirectShipment frayte;
                            foreach (var detail in result)
                            {
                                frayte = new FrayteUserDirectShipment();
                                frayte.ShipmentId = detail.DirectShipmentId;
                                frayte.ShipmentCode = "";
                                frayte.Customer = detail.ContactName;
                                frayte.ShippedFromCompany = detail.FromCompany;
                                frayte.ShippedToCompany = detail.ToCompany;
                                frayte.ShippingBy = detail.LogisticCompany;
                                frayte.DisplayName = detail.LogisticCompanyDisplay;
                                frayte.RateType = detail.RateType;
                                frayte.RateTypeDisplay = detail.RateTypeDisplay;
                                frayte.ShippingDate = detail.CreatedOn.HasValue ? detail.CreatedOn.Value : DateTime.UtcNow;
                                frayte.Reference1 = detail.Reference1;
                                frayte.FrayteNumber = detail.FrayteNumber;
                                frayte.Status = detail.StatusName;
                                frayte.DisplayStatus = detail.DisplayStatusName;
                                frayte.TrackingNo = detail.TrackingNo;
                                frayte.TotalRows = TotalRows;
                                frayte.ManifestId = detail.ManifestId == null ? 0 : detail.ManifestId.Value;
                                frayte.LogisticType = detail.LogisticType;
                                frayte.LogisticTypeDisplay = detail.LogisticTypeDisplay;
                                frayte.BookingApp = detail.BookingApp;
                                if (track.BookingMethod == FrayteShipmentServiceType.eCommerce)
                                {
                                    if (detail.BookingApp == eCommerceShipmentType.eCommerceWS)
                                        frayte.IsTrackingShow = false;
                                    else
                                        frayte.IsTrackingShow = true;
                                }
                                else
                                {
                                    frayte.IsTrackingShow = true;
                                }
                                lstUserShipment.Add(frayte);
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        #region

                        var result = dbContext.spGet_TrackDirectBookingDetail(track.BookingMethod, track.eCommerceShipmentType, fromdate, todate, track.ShipmentStatusId, track.FrayteNumber.Trim(), track.TrackingNo.Trim(), track.LogisticType,
                                                                              SkipRows, track.TakeRows, track.CustomerId, track.LogisticServiceType, track.UserId, OperationZone.OperationZoneId, RoleId, track.CallingFrom).ToList();

                        if (result != null && result.Count > 0)
                        {
                            TotalRows = result[0].TotalRows == null ? 0 : result[0].TotalRows.Value;
                        }
                        else
                        {
                            TotalRows = 0;
                        }

                        if (result != null)
                        {
                            FrayteUserDirectShipment frayte;
                            foreach (var detail in result)
                            {
                                frayte = new FrayteUserDirectShipment();
                                frayte.ShipmentId = detail.DirectShipmentId;
                                frayte.ShipmentCode = "";
                                frayte.Customer = detail.ContactName;
                                frayte.ShippedFromCompany = detail.FromCompany;
                                frayte.ShippedToCompany = detail.ToCompany;
                                frayte.ShippingBy = detail.LogisticCompany;
                                frayte.DisplayName = detail.LogisticCompanyDisplay;
                                frayte.RateType = detail.RateType;
                                frayte.RateTypeDisplay = detail.RateTypeDisplay;
                                frayte.ShippingDate = detail.CreatedOn.HasValue ? detail.CreatedOn.Value : DateTime.UtcNow;
                                frayte.Reference1 = detail.Reference1;
                                frayte.FrayteNumber = detail.FrayteNumber;
                                frayte.Status = detail.StatusName;
                                frayte.DisplayStatus = detail.DisplayStatusName;
                                frayte.TrackingNo = detail.TrackingNo;
                                frayte.TotalRows = TotalRows;
                                frayte.ManifestId = detail.ManifestId == null ? 0 : detail.ManifestId.Value;
                                frayte.LogisticType = detail.LogisticType;
                                frayte.LogisticTypeDisplay = detail.LogisticTypeDisplay;
                                frayte.BookingApp = detail.BookingApp;
                                if (track.BookingMethod == FrayteShipmentServiceType.eCommerce)
                                {
                                    if (detail.BookingApp == eCommerceShipmentType.eCommerceWS)
                                        frayte.IsTrackingShow = false;
                                    else
                                        frayte.IsTrackingShow = true;
                                }
                                else
                                {
                                    frayte.IsTrackingShow = true;
                                }
                                lstUserShipment.Add(frayte);
                            }
                        }

                        #endregion
                    }
                }

                #endregion
            }
            return lstUserShipment.ToList();
        }

        public List<ManifestShipmentModel> GetTrackAndTraceDetail(FrayteTrackDirectBooking trackdetail)
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

            var result = dbContext.spGet_TrackAndTraceDetail(fromdate, todate, trackdetail.ShipmentStatusId, trackdetail.FrayteNumber,
                                                             trackdetail.TrackingNo, trackdetail.LogisticType, trackdetail.CustomerId,
                                                             trackdetail.LogisticServiceType, trackdetail.UserId, OperationZone.OperationZoneId,
                                                             trackdetail.BookingMethod).ToList();

            if (result != null && result.Count > 0)
            {
                foreach (var Obj in result)
                {
                    ManifestShipmentModel track = new ManifestShipmentModel();
                    track.StatusType = Obj.StatusType;
                    track.Courier = Obj.Courier;
                    track.LogisticType = Obj.LogisticType;
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
                    track.Suburb = Obj.Suburb;
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
                    track.ShipmentWeight = Obj.ShipmentWeight.HasValue ? Obj.ShipmentWeight.Value : 0.0m;
                    track.PaymentPartyTaxAndDuties = Obj.PaymentPartyTaxAndDuties;
                    track.PaymentPartyTaxAndDutiesAcceptedBy = Obj.PaymentPartyTaxAndDutiesAcceptedBy;
                    track.ManifestNumber = Obj.ManifestNumber;
                    track.ShipmentContent = Obj.ShipmentContent;
                    track.CreatedOn = Obj.CreatedOn;
                    track.CreatedBy = Obj.CreatedBy;
                    track.DeclaredValueCurrency = Obj.DeclaredValueCurrency;
                    track.EstimatedCost = Obj.EstimatedCost.HasValue ? Obj.EstimatedCost.Value : 0.0m;
                    track.FuelSurcharge = Obj.FuelSurCharge.HasValue ? Obj.FuelSurCharge.Value : 0.0m;
                    track.FuelPercent = (Obj.FuelSurchargePercent.HasValue ? Obj.FuelSurchargePercent.Value : 0.0m).ToString() + " (" + Obj.FuelMonthYear + ")";
                    track.CollectionDateandTime = (Obj.CollectionDate.HasValue ? Obj.CollectionDate.Value.ToString("dd-MMM-yyyy") : "") + " " + (Obj.CollectionTime.HasValue ? Obj.CollectionTime.ToString() : "");
                    track.PickUpRef = Obj.PickUpRef;
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

        public List<ShipmentStatu> GetDirectShipmentStatusList(string BookingType, int UserId)
        {
            var list = dbContext.ShipmentStatus.Where(x => x.BookingType == BookingType &&
            x.ShipmentStatusId != (int)FrayteShipmentStatus.eCHandOver &&
            x.ShipmentStatusId != (int)FrayteShipmentStatus.eCDepartured).ToList();
            return list;
        }

        public FrayteResult DeleteDirectBooking(int DirectBookingDraftId)
        {
            FrayteResult result = new FrayteResult();
            var directShipment = dbContext.DirectShipmentDrafts.Find(DirectBookingDraftId);
            if (directShipment != null)
            {
                var fromAddress = dbContext.AddressBooks.Find(directShipment.FromAddressId);
                if (fromAddress != null)
                {
                    dbContext.AddressBooks.Remove(fromAddress);
                    dbContext.SaveChanges();
                    result.Status = true;
                }

                var toAddress = dbContext.AddressBooks.Find(directShipment.ToAddressId);
                if (toAddress != null)
                {
                    dbContext.AddressBooks.Remove(toAddress);
                    dbContext.SaveChanges();
                    result.Status = true;
                }

                var customInfo = dbContext.ShipmentCustomDetailDrafts.Where(p => p.ShipmentDraftId == DirectBookingDraftId).FirstOrDefault();
                if (customInfo != null)
                {
                    dbContext.ShipmentCustomDetailDrafts.Remove(customInfo);
                    dbContext.SaveChanges();
                    result.Status = true;
                }

                var directShipmentDetail = dbContext.DirectShipmentDetailDrafts.Where(p => p.DirectShipmentDraftId == directShipment.DirectShipmentDraftId).ToList();
                if (directShipmentDetail != null && directShipmentDetail.Count > 0)
                {
                    dbContext.DirectShipmentDetailDrafts.RemoveRange(directShipmentDetail);
                    dbContext.SaveChanges();
                    result.Status = true;
                }

                dbContext.DirectShipmentDrafts.Remove(directShipment);
                dbContext.SaveChanges();
                result.Status = true;
            }
            return result;
        }

        public void SavePackageDetail(CourierPieceDetail pieceDetail, string CourierCompany)
        {
            Frayte.Services.Models.Package package = new Frayte.Services.Models.Package();
            package.LabelName = pieceDetail.PieceTrackingNumber;
            SavePackageDetail(package, pieceDetail.LabelName, pieceDetail.DirectShipmentDetailId, CourierCompany, 0);
        }

        public void SavePackageDetail(Package package, string ImageName, int DirectShipmentDetailId, string CourierCompany, int count)
        {
            if (CourierCompany == FrayteLogisticServiceType.Hermes)
            {
                if (count == 0)
                {
                    var detail = dbContext.PackageTrackingDetails.Where(p => p.TrackingNo == package.LabelName &&
                                                                             p.PackageImage == null).FirstOrDefault();
                    if (detail != null)
                    {
                        if (detail.PackageImage == "" || detail.PackageImage == null)
                        {
                            detail.PackageImage = ImageName;
                            detail.IsDownloaded = true;
                            dbContext.SaveChanges();
                        }
                    }
                }
                else if (count > 0)
                {
                    PackageTrackingDetail sph = new PackageTrackingDetail();
                    sph.DirectShipmentDetailId = DirectShipmentDetailId;
                    sph.TrackingNo = package.LabelName;
                    sph.PackageImage = null;
                    sph.IsDownloaded = false;
                    sph.IsPrinted = false;
                    dbContext.PackageTrackingDetails.Add(sph);
                    dbContext.SaveChanges();
                    if (!string.IsNullOrEmpty(sph.TrackingNo) && sph.DirectShipmentDetailId > 0)
                    {
                        var Result = dbContext.DirectShipmentDetails.Where(a => a.DirectShipmentDetailId == sph.DirectShipmentDetailId).FirstOrDefault();
                        if (Result != null)
                        {
                            SaveDirectBookingPiecesTrackingNo(sph.TrackingNo, Result.DirectShipmentId);
                        }
                    }
                }
            }
            else
            {
                var detail = dbContext.PackageTrackingDetails.Where(p => p.TrackingNo == package.LabelName &&
                                                                         p.DirectShipmentDetailId == DirectShipmentDetailId).FirstOrDefault();
                if (detail != null)
                {
                    detail.PackageImage = ImageName;
                    detail.IsDownloaded = true;
                    dbContext.SaveChanges();
                }
                else
                {
                    PackageTrackingDetail sph = new PackageTrackingDetail();
                    sph.DirectShipmentDetailId = DirectShipmentDetailId;
                    sph.TrackingNo = package.LabelName;
                    sph.PackageImage = ImageName;
                    sph.IsDownloaded = false;
                    sph.IsPrinted = false;
                    dbContext.PackageTrackingDetails.Add(sph);
                    dbContext.SaveChanges();
                    if (!string.IsNullOrEmpty(sph.TrackingNo) && sph.DirectShipmentDetailId > 0)
                    {
                        var Result = dbContext.DirectShipmentDetails.Where(a => a.DirectShipmentDetailId == sph.DirectShipmentDetailId).FirstOrDefault();
                        if (Result != null)
                        {
                            SaveDirectBookingPiecesTrackingNo(sph.TrackingNo, Result.DirectShipmentId);
                        }
                    }
                }
            }
        }

        public List<CustomerLogisticService> GetCustomerLogisticService(int CustomerId)
        {
            var operationzone = UtilityRepository.GetOperationZone();
            var _logisticService = (from u in dbContext.Users
                                    join cl in dbContext.CustomerLogistics on u.UserId equals cl.UserId
                                    join ls in dbContext.LogisticServices on cl.LogisticServiceId equals ls.LogisticServiceId
                                    where u.UserId == CustomerId &&
                                          ls.OperationZoneId == operationzone.OperationZoneId
                                    select new CustomerLogisticService
                                    {
                                        UserId = u.UserId,
                                        LogisticCompany = ls.LogisticCompany,
                                        LogisticServiceType = cl.LogisticServiceType
                                    }).ToList();

            return _logisticService;
        }

        public void CreateZoneRateCard(int OperationZoneId, string CourierCompany, string LogisticType, string RateType, string ModuleType)
        {
            //Step 1: Get all zone for operation zone
            var zoneList = (from ls in dbContext.LogisticServices
                            join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                            where ls.OperationZoneId == OperationZoneId &&
                                ls.LogisticCompany == CourierCompany &&
                                ls.LogisticType == LogisticType &&
                                ls.RateType == RateType &&
                                ls.ModuleType == ModuleType
                            select lsz).ToList();

            //Step 2: Get all Shipment Types            
            var shipmentTypeList = (from ls in dbContext.LogisticServices
                                    join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                    where ls.OperationZoneId == OperationZoneId &&
                                          ls.LogisticCompany == CourierCompany &&
                                          ls.LogisticType == LogisticType &&
                                          ls.RateType == RateType &&
                                          ls.ModuleType == ModuleType
                                    select lsst).ToList();

            //Step 3: Get all logistic weight            
            var logisticWeightList = (from ls in dbContext.LogisticServices
                                      join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                      join lsw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lsw.LogisticServiceShipmentTypeId
                                      where ls.OperationZoneId == OperationZoneId &&
                                            ls.LogisticCompany == CourierCompany &&
                                            ls.LogisticType == LogisticType &&
                                            ls.RateType == RateType &&
                                            ls.ModuleType == ModuleType
                                      select lsw).ToList();


            if (zoneList != null && shipmentTypeList != null)
            {
                foreach (var zone in zoneList)
                {
                    foreach (var shipmentType in shipmentTypeList)
                    {
                        foreach (var logisticWeight in logisticWeightList)
                        {
                            if (shipmentType.LogisticServiceShipmentTypeId == logisticWeight.LogisticServiceShipmentTypeId)
                            {
                                LogisticServiceBaseRateCard rateCard = new LogisticServiceBaseRateCard();
                                rateCard.OperationZoneId = OperationZoneId;
                                rateCard.LogisticServiceZoneId = zone.LogisticServiceZoneId;
                                rateCard.LogisticServiceWeightId = logisticWeight.LogisticServiceWeightId;
                                rateCard.LogisticRate = 0;
                                rateCard.LogisticServiceShipmentTypeId = shipmentType.LogisticServiceShipmentTypeId;
                                rateCard.LogisticServiceCourierAccountId = 0;
                                rateCard.LogisticServiceDimensionId = 0;
                                rateCard.LogisticCurrency = "USD";
                                rateCard.ModuleType = ModuleType;
                                dbContext.LogisticServiceBaseRateCards.Add(rateCard);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
        }

        public void CreateUKZoneRateCard(int OperationZoneId, string CourierCompany, string LogisticType, string ModuleType)
        {
            if (CourierCompany == FrayteLogisticServiceType.Yodel)
            {
                //Step 1: Get all zone for UK operation zone and UK-Shipment                
                var zoneList = (from ls in dbContext.LogisticServices
                                join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                                where ls.OperationZoneId == OperationZoneId &&
                                      ls.LogisticCompany == CourierCompany &&
                                      ls.LogisticType == LogisticType &&
                                      ls.ModuleType == ModuleType
                                select lsz).ToList();

                //Step 2: Get all Shipment Types for Parcel                
                var shipmentTypeList = (from ls in dbContext.LogisticServices
                                        join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                        where ls.OperationZoneId == OperationZoneId &&
                                              ls.LogisticCompany == CourierCompany &&
                                              ls.LogisticType == LogisticType &&
                                              ls.ModuleType == ModuleType &&
                                              lsst.LogisticType == LogisticType
                                        select lsst).ToList();

                //Step 3: Get all logistic weight                
                var logisticWeightList = (from ls in dbContext.LogisticServices
                                          join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                          join lsw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lsw.LogisticServiceShipmentTypeId
                                          where ls.OperationZoneId == OperationZoneId &&
                                                ls.LogisticCompany == CourierCompany &&
                                                ls.LogisticType == LogisticType &&
                                                ls.ModuleType == ModuleType &&
                                                lsst.LogisticType == LogisticType
                                          select lsw).ToList();

                //Step 4: Get all logistic Service Dimension                
                //var logisticServiceDimension = (from ls in dbContext.LogisticServices
                //                                join lsd in dbContext.LogisticServiceDimensionNews on ls.LogisticServiceId equals lsd.LogisticServiceId
                //                                where ls.OperationZoneId == OperationZoneId &&
                //                                      ls.LogisticCompany == CourierCompany &&
                //                                      ls.LogisticType == LogisticType &&
                //                                      ls.ModuleType == ModuleType
                //                                select lsd).ToList();


                //Step 4: Get the logistic weight limit record                
                var lstLogisticWL = (from ls in dbContext.LogisticServices
                                     join lswl in dbContext.LogisticServiceWeightLimits on ls.LogisticServiceId equals lswl.LogisticServiceId
                                     where ls.OperationZoneId == OperationZoneId &&
                                           ls.LogisticCompany == CourierCompany &&
                                           ls.LogisticType == LogisticType &&
                                           ls.ModuleType == ModuleType
                                     select lswl).ToList();


                //Step 5: Run For LogisticWeight
                if (zoneList != null && shipmentTypeList != null)
                {
                    foreach (var zone in zoneList)
                    {
                        foreach (var shipmentType in shipmentTypeList)
                        {
                            foreach (var logisticWeight in logisticWeightList)
                            {
                                if (shipmentType.LogisticServiceShipmentTypeId == logisticWeight.LogisticServiceShipmentTypeId)
                                {
                                    LogisticServiceBaseRateCard rateCard = new LogisticServiceBaseRateCard();
                                    rateCard.OperationZoneId = OperationZoneId;
                                    rateCard.LogisticServiceCourierAccountId = 0;
                                    rateCard.LogisticServiceWeightId = logisticWeight.LogisticServiceWeightId;
                                    rateCard.LogisticRate = 0;
                                    rateCard.LogisticServiceShipmentTypeId = shipmentType.LogisticServiceShipmentTypeId;
                                    rateCard.LogisticServiceZoneId = zone.LogisticServiceZoneId;
                                    rateCard.LogisticCurrency = "USD";
                                    rateCard.LogisticServiceDimensionId = 0;
                                    rateCard.ModuleType = ModuleType;
                                    dbContext.LogisticServiceBaseRateCards.Add(rateCard);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            else if (CourierCompany == FrayteLogisticServiceType.Hermes)
            {
                //Step 1: Get all zone for UK operation zone and UK-Shipment                
                var zoneList = (from ls in dbContext.LogisticServices
                                join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                                where ls.OperationZoneId == OperationZoneId &&
                                      ls.LogisticCompany == CourierCompany &&
                                      ls.LogisticType == LogisticType &&
                                      ls.ModuleType == ModuleType
                                select lsz).ToList();

                //Step 2: Get all logistic weight                
                var logisticWeightList = (from ls in dbContext.LogisticServices
                                          join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                          join lsw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lsw.LogisticServiceShipmentTypeId
                                          where ls.OperationZoneId == OperationZoneId &&
                                                ls.LogisticCompany == CourierCompany &&
                                                ls.LogisticType == LogisticType &&
                                                ls.ModuleType == ModuleType
                                          select lsw).ToList();

                //Step 3: Get all logistic Service Dimension                
                //var logisticServiceDimension = (from ls in dbContext.LogisticServices
                //                                join lsd in dbContext.LogisticServiceDimensionNews on ls.LogisticServiceId equals lsd.LogisticServiceId
                //                                where ls.OperationZoneId == OperationZoneId &&
                //                                      ls.LogisticCompany == CourierCompany &&
                //                                      ls.LogisticType == LogisticType &&
                //                                      ls.ModuleType == ModuleType
                //                                select lsd).ToList();

                //Step 4: Get all logistic Shipment Type
                var logisticShipment = (from ls in dbContext.LogisticServices
                                        join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                        where ls.OperationZoneId == OperationZoneId &&
                                              ls.LogisticCompany == CourierCompany &&
                                              ls.LogisticType == LogisticType &&
                                              ls.ModuleType == ModuleType
                                        select lsst).ToList();


                //Step 5: Run According ZoneList
                if (zoneList != null && zoneList.Count > 0)
                {
                    if (logisticWeightList != null && logisticWeightList.Count > 0)
                    {
                        foreach (var zone in zoneList)
                        {
                            foreach (var shipment in logisticShipment)
                            {
                                foreach (var weight in logisticWeightList)
                                {
                                    if (shipment.LogisticServiceShipmentTypeId == weight.LogisticServiceShipmentTypeId)
                                    {
                                        LogisticServiceBaseRateCard rateCard = new LogisticServiceBaseRateCard();
                                        rateCard.OperationZoneId = OperationZoneId;
                                        rateCard.LogisticServiceCourierAccountId = 0;
                                        rateCard.LogisticServiceWeightId = weight.LogisticServiceWeightId;
                                        rateCard.LogisticRate = 0;
                                        rateCard.LogisticServiceShipmentTypeId = shipment.LogisticServiceShipmentTypeId;
                                        rateCard.ModuleType = ModuleType;
                                        rateCard.LogisticServiceZoneId = zone.LogisticServiceZoneId;
                                        rateCard.LogisticCurrency = "USD";
                                        rateCard.LogisticServiceDimensionId = 0;
                                        dbContext.LogisticServiceBaseRateCards.Add(rateCard);
                                        dbContext.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //Step 1: Get all zone for UK operation zone and UK-Shipment                
                var zoneList = (from ls in dbContext.LogisticServices
                                join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                                where ls.OperationZoneId == OperationZoneId &&
                                      ls.LogisticCompany == CourierCompany &&
                                      ls.LogisticType == LogisticType &&
                                      ls.ModuleType == ModuleType
                                select lsz).ToList();

                //Step 2: Get all Shipment Types for Parcel                
                var shipmentTypeList = (from ls in dbContext.LogisticServices
                                        join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                        where ls.OperationZoneId == OperationZoneId &&
                                              ls.LogisticCompany == CourierCompany &&
                                              ls.LogisticType == LogisticType &&
                                              ls.ModuleType == ModuleType &&
                                              lsst.LogisticType == LogisticType
                                        select lsst).Take(7).ToList();

                //Step 3: Get all logistic weight                
                var logisticWeightList = (from ls in dbContext.LogisticServices
                                          join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                          join lsw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lsw.LogisticServiceShipmentTypeId
                                          where ls.OperationZoneId == OperationZoneId &&
                                                ls.LogisticCompany == CourierCompany &&
                                                ls.LogisticType == LogisticType &&
                                                ls.ModuleType == ModuleType &&
                                                lsst.LogisticType == LogisticType &&
                                                lsw.ParcelType == "BagItService" &&
                                                lsw.PackageType == "Multiple"
                                          select lsw).ToList();

                //Step 4: Get the logistic weight limit record                
                var lstLogisticWL = (from ls in dbContext.LogisticServices
                                     join lswl in dbContext.LogisticServiceWeightLimits on ls.LogisticServiceId equals lswl.LogisticServiceId
                                     where ls.OperationZoneId == OperationZoneId &&
                                           ls.LogisticCompany == CourierCompany &&
                                           ls.LogisticType == LogisticType
                                     select lswl).ToList();


                if (zoneList != null && shipmentTypeList != null)
                {
                    foreach (var zone in zoneList)
                    {
                        foreach (var shipmentType in shipmentTypeList)
                        {
                            foreach (var logisticWeight in logisticWeightList)
                            {
                                if (shipmentType.LogisticServiceShipmentTypeId == logisticWeight.LogisticServiceShipmentTypeId)
                                {
                                    LogisticServiceBaseRateCard rateCard = new LogisticServiceBaseRateCard();
                                    rateCard.OperationZoneId = OperationZoneId;
                                    rateCard.LogisticServiceWeightId = logisticWeight.LogisticServiceWeightId;
                                    rateCard.LogisticServiceZoneId = zone.LogisticServiceZoneId;
                                    rateCard.LogisticServiceShipmentTypeId = shipmentType.LogisticServiceShipmentTypeId;
                                    rateCard.LogisticServiceCourierAccountId = 0;
                                    rateCard.LogisticServiceDimensionId = 0;
                                    rateCard.LogisticRate = 0;
                                    rateCard.ModuleType = ModuleType;
                                    rateCard.LogisticCurrency = "USD";
                                    dbContext.LogisticServiceBaseRateCards.Add(rateCard);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
        }

        public void CreateEUExportImportZoneRateCard(int OperationZoneId, string CourierCompany, string LogisticType, string RateType, string ModuleType)
        {
            //Step 1: Get all zone for operation zone            
            var zoneList = (from ls in dbContext.LogisticServices
                            join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                            where ls.OperationZoneId == OperationZoneId &&
                                  ls.LogisticCompany == CourierCompany &&
                                  ls.LogisticType == LogisticType &&
                                  ls.ModuleType == ModuleType &&
                                  ls.RateType == RateType
                            select lsz).ToList();

            //Step 2: Get all shipment type
            var shipmentType = (from ls in dbContext.LogisticServices
                                join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                where ls.OperationZoneId == OperationZoneId &&
                                      ls.LogisticCompany == CourierCompany &&
                                      ls.LogisticType == LogisticType &&
                                      ls.ModuleType == ModuleType
                                select lsst).ToList();


            //Step 3: Get all logistic weight            
            var logisticWeightList = (from ls in dbContext.LogisticServices
                                      join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                      join lsw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lsw.LogisticServiceShipmentTypeId
                                      where ls.OperationZoneId == OperationZoneId &&
                                            ls.LogisticCompany == CourierCompany &&
                                            ls.LogisticType == LogisticType &&
                                            ls.ModuleType == ModuleType
                                      select lsw).ToList();

            if (zoneList != null && logisticWeightList != null)
            {
                foreach (var zone in zoneList)
                {
                    foreach (var shipment in shipmentType)
                    {
                        foreach (var logisticWeight in logisticWeightList)
                        {
                            LogisticServiceBaseRateCard rateCard = new LogisticServiceBaseRateCard();
                            rateCard.OperationZoneId = OperationZoneId;
                            rateCard.LogisticServiceWeightId = logisticWeight.LogisticServiceWeightId;
                            rateCard.LogisticServiceZoneId = zone.LogisticServiceZoneId;
                            rateCard.LogisticServiceShipmentTypeId = shipment.LogisticServiceShipmentTypeId;
                            rateCard.LogisticServiceCourierAccountId = 0;
                            rateCard.LogisticServiceDimensionId = 0;
                            rateCard.LogisticRate = 0;
                            rateCard.ModuleType = ModuleType;
                            rateCard.LogisticCurrency = "USD";
                            dbContext.LogisticServiceBaseRateCards.Add(rateCard);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
        }

        public List<FraytePackageLabel> GetPackageList(int DirectShipmentId, string CourierCompany, string RateType)
        {
            List<FraytePackageLabel> _package = new List<FraytePackageLabel>();

            if (CourierCompany.Replace(" ", "") == FrayteCourierAccountCode.DHL)
            {
                var item = (from PTD in dbContext.PackageTrackingDetails
                            join DSD in dbContext.DirectShipmentDetails on PTD.DirectShipmentDetailId equals DSD.DirectShipmentDetailId
                            where DSD.DirectShipmentId == DirectShipmentId
                            select new
                            {
                                ImageName = PTD.PackageImage
                            }).FirstOrDefault();

                if (item != null)
                {
                    FraytePackageLabel pl;
                    if (CourierCompany.Replace(" ", "") == FrayteCourierAccountCode.DHL)
                    {
                        pl = new FraytePackageLabel();
                        pl.LabelPath = HostingEnvironment.MapPath("~/PackageLabel/" + DirectShipmentId + "/" + item.ImageName);
                        _package.Add(pl);
                    }
                }
            }
            else
            {
                var result = (from PTD in dbContext.PackageTrackingDetails
                              join DSD in dbContext.DirectShipmentDetails on PTD.DirectShipmentDetailId equals DSD.DirectShipmentDetailId
                              where DSD.DirectShipmentId == DirectShipmentId
                              select new
                              {
                                  ImageName = PTD.PackageImage
                              }).ToList();

                if (result != null && result.Count > 0)
                {
                    FraytePackageLabel pl;
                    if (CourierCompany.Replace(" ", "") == FrayteCourierAccountCode.UKMail)
                    {
                        foreach (var Obj in result)
                        {
                            pl = new FraytePackageLabel();
                            pl.LabelPath = HostingEnvironment.MapPath("~/PackageLabel/" + DirectShipmentId + "/" + Obj.ImageName);
                            _package.Add(pl);
                        }
                    }
                    else
                    {
                        foreach (var Obj in result)
                        {
                            pl = new FraytePackageLabel();
                            pl.LabelPath = HostingEnvironment.MapPath("~/PackageLabel/" + DirectShipmentId + "/" + Obj.ImageName);
                            _package.Add(pl);
                        }
                    }
                }
            }
            return _package;
        }

        public List<FrayteDirectBookingAddressBook> GetCustomerAddressAdvanceSearch(FrayteCustomerAddressSearch filter)
        {
            List<FrayteDirectBookingAddressBook> addressBook = new List<FrayteDirectBookingAddressBook>();

            int TotalRows = 0;
            int SkipRows = 0;

            if (filter.SearchBy == "Select Search Option") { filter.SearchBy = ""; }

            if (filter.SearchText == "'") { filter.SearchText = filter.SearchText.Replace("'", ""); }
            filter.SearchText = filter.SearchText.Replace("'", "''");
            if (filter.SearchText == "%") { filter.SearchText = filter.SearchText.Replace("%", ""); }
            if (filter.SearchText == "_") { filter.SearchText = filter.SearchText.Replace("_", ""); }

            SkipRows = (filter.CurrentPage - 1) * filter.TakeRows;

            if (filter.AddressType == FrayteFromToAddressType.AllAddress)
            {
                var result = dbContext.spGet_ShipmentAddress(filter.AddressType, filter.SearchBy, filter.SearchText, filter.CustomerId, SkipRows, filter.TakeRows).ToList();

                if (result != null && result.Count > 0)
                {
                    TotalRows = result[0].TotalRows == null ? 0 : result[0].TotalRows.Value;
                }
                else
                {
                    TotalRows = 0;
                }

                if (result != null)
                {
                    if (filter.ModuleType == FrayteShipmentServiceType.eCommerce && filter.AddressSearch == FrayteFromToAddressType.ToAddress)
                    {
                        result = (from r in result
                                  join w in dbContext.Warehouses on r.CountryId equals w.CountryId
                                  select r).ToList();
                    }

                    foreach (var Obj in result)
                    {
                        FrayteDirectBookingAddressBook address = new FrayteDirectBookingAddressBook();
                        address.AddressbookId = Obj.AddressBookId.HasValue ? Obj.AddressBookId.Value : 0;
                        address.CustomerId = Obj.CustomerId.HasValue ? Obj.CustomerId.Value : 0;
                        address.CustomerName = Obj.ContactName;
                        address.FromAddress = Obj.FromAddress != null ? Obj.FromAddress.Value : false;
                        address.ToAddress = Obj.ToAddress != null ? Obj.ToAddress.Value : false;
                        address.FirstName = Obj.ContactFirstName;
                        address.LastName = Obj.ContactLastName;
                        address.CompanyName = Obj.CompanyName;
                        address.Email = Obj.Email;
                        address.Phone = Obj.PhoneNo;
                        address.Address = Obj.Address1;
                        address.Address2 = Obj.Address2;
                        address.Area = Obj.Area;
                        address.City = Obj.City;
                        address.State = Obj.State;
                        address.PostCode = Obj.Zip;
                        address.Country = new FrayteCountryCode()
                        {
                            CountryId = Obj.CountryId.HasValue ? Obj.CountryId.Value : 0,
                            Name = Obj.CountryName,
                            Code = Obj.CountryCode,
                            Code2 = Obj.CountryCode2
                        };
                        address.IsActive = Obj.IsActive != null ? Obj.IsActive.Value : false;
                        address.IsFavorites = Obj.IsFavorites != null ? Obj.IsFavorites.Value : false;
                        address.TableType = Obj.TableType;
                        address.TotalRows = TotalRows;
                        address.IsDefault = Obj.IsDefault.HasValue ? Obj.IsDefault.Value : false;
                        addressBook.Add(address);
                    }
                }

                return addressBook;
            }
            else if (filter.AddressType == FrayteFromToAddressType.FromAddress)
            {
                var result = dbContext.spGet_ShipmentAddress(filter.AddressType, filter.SearchBy, filter.SearchText, filter.CustomerId, SkipRows, filter.TakeRows).ToList();

                if (result != null && result.Count > 0)
                {
                    TotalRows = result[0].TotalRows == null ? 0 : result[0].TotalRows.Value;
                }
                else
                {
                    TotalRows = 0;
                }

                if (result != null)
                {
                    if (filter.ModuleType == FrayteShipmentServiceType.eCommerce && filter.AddressSearch == FrayteFromToAddressType.ToAddress)
                    {
                        result = (from r in result
                                  join w in dbContext.Warehouses on r.CountryId equals w.CountryId
                                  select r).ToList();
                    }

                    foreach (var Obj in result)
                    {
                        FrayteDirectBookingAddressBook address = new FrayteDirectBookingAddressBook();
                        address.AddressbookId = Obj.AddressBookId.HasValue ? Obj.AddressBookId.Value : 0;
                        address.CustomerId = Obj.CustomerId.HasValue ? Obj.CustomerId.Value : 0;
                        address.CustomerName = Obj.ContactName;
                        address.FromAddress = Obj.FromAddress != null ? Obj.FromAddress.Value : false;
                        address.ToAddress = Obj.ToAddress != null ? Obj.ToAddress.Value : false;
                        address.FirstName = Obj.ContactFirstName;
                        address.LastName = Obj.ContactLastName;
                        address.CompanyName = Obj.CompanyName;
                        address.Email = Obj.Email;
                        address.Phone = Obj.PhoneNo;
                        address.Address = Obj.Address1;
                        address.Address2 = Obj.Address2;
                        address.Area = Obj.Area;
                        address.City = Obj.City;
                        address.State = Obj.State;
                        address.PostCode = Obj.Zip;
                        address.Country = new FrayteCountryCode()
                        {
                            CountryId = Obj.CountryId.HasValue ? Obj.CountryId.Value : 0,
                            Name = Obj.CountryName,
                            Code = Obj.CountryCode,
                            Code2 = Obj.CountryCode2
                        };
                        address.IsActive = Obj.IsActive != null ? Obj.IsActive.Value : false;
                        address.IsFavorites = Obj.IsFavorites != null ? Obj.IsFavorites.Value : false;
                        address.TableType = Obj.TableType;
                        address.TotalRows = TotalRows;
                        address.IsDefault = Obj.IsDefault.HasValue ? Obj.IsDefault.Value : false;
                        addressBook.Add(address);
                    }
                }

                return addressBook;
            }
            else if (filter.AddressType == FrayteFromToAddressType.ToAddress)
            {
                var result = dbContext.spGet_ShipmentAddress(filter.AddressType, filter.SearchBy, filter.SearchText, filter.CustomerId, SkipRows, filter.TakeRows).ToList();

                if (result != null && result.Count > 0)
                {
                    TotalRows = result[0].TotalRows == null ? 0 : result[0].TotalRows.Value;
                }
                else
                {
                    TotalRows = 0;
                }

                if (result != null)
                {
                    if (filter.ModuleType == FrayteShipmentServiceType.eCommerce && filter.AddressSearch == FrayteFromToAddressType.ToAddress)
                    {
                        result = (from r in result
                                  join w in dbContext.Warehouses on r.CountryId equals w.CountryId
                                  select r).ToList();
                    }

                    foreach (var Obj in result)
                    {
                        FrayteDirectBookingAddressBook address = new FrayteDirectBookingAddressBook();
                        address.AddressbookId = Obj.AddressBookId.HasValue ? Obj.AddressBookId.Value : 0;
                        address.CustomerId = Obj.CustomerId.HasValue ? Obj.CustomerId.Value : 0;
                        address.CustomerName = Obj.ContactName;
                        address.FromAddress = Obj.FromAddress != null ? Obj.FromAddress.Value : false;
                        address.ToAddress = Obj.ToAddress != null ? Obj.ToAddress.Value : false;
                        address.FirstName = Obj.ContactFirstName;
                        address.LastName = Obj.ContactLastName;
                        address.CompanyName = Obj.CompanyName;
                        address.Email = Obj.Email;
                        address.Phone = Obj.PhoneNo;
                        address.Address = Obj.Address1;
                        address.Address2 = Obj.Address2;
                        address.Area = Obj.Area;
                        address.City = Obj.City;
                        address.State = Obj.State;
                        address.PostCode = Obj.Zip;
                        address.Country = new FrayteCountryCode()
                        {
                            CountryId = Obj.CountryId.HasValue ? Obj.CountryId.Value : 0,
                            Name = Obj.CountryName,
                            Code = Obj.CountryCode,
                            Code2 = Obj.CountryCode2
                        };
                        address.IsActive = Obj.IsActive != null ? Obj.IsActive.Value : false;
                        address.IsFavorites = Obj.IsFavorites != null ? Obj.IsFavorites.Value : false;
                        address.TableType = Obj.TableType;
                        address.TotalRows = TotalRows;
                        address.IsDefault = Obj.IsDefault.HasValue ? Obj.IsDefault.Value : false;
                        addressBook.Add(address);
                    }
                }

                return addressBook;
            }
            return null;
        }

        public FrayteResult AddEditCustomerAddressBook(FrayteDirectBookingAddressBook FrayteAddressBook)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                if (FrayteAddressBook != null)
                {
                    if (FrayteAddressBook.AddressbookId > 0)
                    {
                        if (FrayteAddressBook.TableType == FrayteTableType.DirectBooking)
                        {
                            DirectShipmentAddress dsa = dbContext.DirectShipmentAddresses.Where(p => p.DirectShipmentAddressId == FrayteAddressBook.AddressbookId).FirstOrDefault();
                            if (dsa != null && dsa.DirectShipmentAddressId > 0)
                            {
                                dsa.IsActive = false;
                                dbContext.Entry(dsa).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();

                                var addressBookData = dbContext.AddressBooks.Where(p => p.Address1 == FrayteAddressBook.Address &&
                                                                       p.Address2 == FrayteAddressBook.Address2 &&
                                                                       p.City == FrayteAddressBook.City &&
                                                                       p.State == (FrayteAddressBook.State == null ? "" : FrayteAddressBook.State) &&
                                                                       p.PhoneNo == FrayteAddressBook.Phone &&
                                                                       p.Area == FrayteAddressBook.Area &&
                                                                       p.CompanyName == FrayteAddressBook.CompanyName &&
                                                                       p.ContactFirstName == FrayteAddressBook.FirstName &&
                                                                       p.ContactLastName == FrayteAddressBook.LastName &&
                                                                       p.CountryId == FrayteAddressBook.Country.CountryId &&
                                                                       p.CustomerId == FrayteAddressBook.CustomerId &&
                                                                       p.Email == FrayteAddressBook.Email &&
                                                                       p.Zip == FrayteAddressBook.PostCode &&
                                                                       p.IsActive == true).ToList();

                                if (addressBookData != null && addressBookData.Count > 0)
                                {
                                    result.Status = false;
                                }
                                else
                                {
                                    //Save Record Into Address Book Table
                                    AddressBook address = new AddressBook();
                                    address.FromAddress = FrayteAddressBook.FromAddress;
                                    address.ToAddress = FrayteAddressBook.ToAddress;
                                    address.CustomerId = FrayteAddressBook.CustomerId;
                                    address.ContactFirstName = FrayteAddressBook.FirstName;
                                    address.ContactLastName = FrayteAddressBook.LastName;
                                    address.CompanyName = FrayteAddressBook.CompanyName;
                                    address.Email = FrayteAddressBook.Email;
                                    address.PhoneNo = FrayteAddressBook.Phone;
                                    address.Address1 = FrayteAddressBook.Address;
                                    address.Area = FrayteAddressBook.Area;
                                    address.Address2 = FrayteAddressBook.Address2;
                                    address.City = FrayteAddressBook.City;
                                    address.State = (FrayteAddressBook.State == null ? "" : FrayteAddressBook.State);
                                    address.Zip = FrayteAddressBook.PostCode;
                                    address.CountryId = FrayteAddressBook.Country.CountryId;
                                    address.IsActive = FrayteAddressBook.IsActive;
                                    address.TableType = FrayteTableType.AddressBook;
                                    address.IsFavorites = FrayteAddressBook.IsFavorites;
                                    dbContext.AddressBooks.Add(address);
                                    if (address != null)
                                    {
                                        dbContext.SaveChanges();
                                    }
                                    result.Status = true;
                                }
                            }
                        }
                        else if (FrayteAddressBook.TableType == FrayteTableType.AddressBook)
                        {
                            var addressBookData = dbContext.AddressBooks.Where(p => p.Address1 == FrayteAddressBook.Address &&
                                                                           p.Address2 == FrayteAddressBook.Address2 &&
                                                                           p.City == FrayteAddressBook.City &&
                                                                           p.State == (FrayteAddressBook.State == null ? "" : FrayteAddressBook.State) &&
                                                                           p.PhoneNo == FrayteAddressBook.Phone &&
                                                                           p.Area == FrayteAddressBook.Area &&
                                                                           p.CompanyName == FrayteAddressBook.CompanyName &&
                                                                           p.ContactFirstName == FrayteAddressBook.FirstName &&
                                                                           p.ContactLastName == FrayteAddressBook.LastName &&
                                                                           p.CountryId == FrayteAddressBook.Country.CountryId &&
                                                                           p.CustomerId == FrayteAddressBook.CustomerId &&
                                                                           p.Email == FrayteAddressBook.Email &&
                                                                           p.Zip == FrayteAddressBook.PostCode &&
                                                                           p.IsActive == true).ToList();

                            if (addressBookData != null && addressBookData.Count > 0)
                            {
                                result.Status = false;
                            }
                            else
                            {
                                var address = dbContext.AddressBooks.Where(p => p.AddressBookId == FrayteAddressBook.AddressbookId).FirstOrDefault();
                                if (address != null)
                                {
                                    address.CustomerId = FrayteAddressBook.CustomerId;
                                    address.ContactFirstName = FrayteAddressBook.FirstName;
                                    address.ContactLastName = FrayteAddressBook.LastName;
                                    address.CompanyName = FrayteAddressBook.CompanyName;
                                    address.Email = FrayteAddressBook.Email;
                                    address.PhoneNo = FrayteAddressBook.Phone;
                                    address.Address1 = FrayteAddressBook.Address;
                                    address.Area = FrayteAddressBook.Area;
                                    address.Address2 = FrayteAddressBook.Address2;
                                    address.City = FrayteAddressBook.City;
                                    address.State = (FrayteAddressBook.State == null ? "" : FrayteAddressBook.State);
                                    address.Zip = FrayteAddressBook.PostCode;
                                    address.CountryId = FrayteAddressBook.Country.CountryId;
                                    address.IsActive = FrayteAddressBook.IsActive;
                                    address.TableType = FrayteAddressBook.TableType;
                                    address.IsFavorites = FrayteAddressBook.IsFavorites;
                                    dbContext.Entry(address).State = System.Data.Entity.EntityState.Modified;
                                    dbContext.SaveChanges();
                                    result.Status = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        var addressBookData = dbContext.AddressBooks.Where(p => p.Address1 == FrayteAddressBook.Address &&
                                                                           p.Address2 == FrayteAddressBook.Address2 &&
                                                                           p.City == FrayteAddressBook.City &&
                                                                           p.State == (FrayteAddressBook.State == null ? "" : FrayteAddressBook.State) &&
                                                                           p.PhoneNo == FrayteAddressBook.Phone &&
                                                                           p.Area == FrayteAddressBook.Area &&
                                                                           p.CompanyName == FrayteAddressBook.CompanyName &&
                                                                           p.ContactFirstName == FrayteAddressBook.FirstName &&
                                                                           p.ContactLastName == FrayteAddressBook.LastName &&
                                                                           p.CountryId == FrayteAddressBook.Country.CountryId &&
                                                                           p.CustomerId == FrayteAddressBook.CustomerId &&
                                                                           p.Email == FrayteAddressBook.Email &&
                                                                           p.Zip == FrayteAddressBook.PostCode &&
                                                                           p.IsActive == true).ToList();

                        if (addressBookData != null && addressBookData.Count > 0)
                        {
                            result.Status = false;
                        }
                        else
                        {
                            AddressBook adbook = new AddressBook();
                            adbook.FromAddress = FrayteAddressBook.FromAddress;
                            adbook.ToAddress = FrayteAddressBook.ToAddress;
                            adbook.CustomerId = FrayteAddressBook.CustomerId;
                            adbook.ContactFirstName = FrayteAddressBook.FirstName;
                            adbook.ContactLastName = FrayteAddressBook.LastName;
                            adbook.CompanyName = FrayteAddressBook.CompanyName;
                            adbook.Email = FrayteAddressBook.Email;
                            adbook.PhoneNo = FrayteAddressBook.Phone;
                            adbook.Address1 = FrayteAddressBook.Address;
                            adbook.Area = FrayteAddressBook.Area;
                            adbook.Address2 = FrayteAddressBook.Address2;
                            adbook.City = FrayteAddressBook.City;
                            adbook.State = (FrayteAddressBook.State == null ? "" : FrayteAddressBook.State);
                            adbook.Zip = FrayteAddressBook.PostCode;
                            adbook.CountryId = FrayteAddressBook.Country.CountryId;
                            adbook.IsActive = FrayteAddressBook.IsActive;
                            adbook.TableType = FrayteTableType.AddressBook;
                            adbook.IsFavorites = FrayteAddressBook.IsFavorites;
                            dbContext.AddressBooks.Add(adbook);
                            if (adbook != null)
                            {
                                dbContext.SaveChanges();
                            }
                            result.Status = true;
                        }
                    }
                }
                return result;
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
                result.Status = false;
                return result;
            }
        }

        public void MarkAsIsFavorites(int AddressBookId, bool IsFavorites)
        {
            var address = dbContext.AddressBooks.Where(p => p.AddressBookId == AddressBookId).FirstOrDefault();
            if (address != null)
            {
                address.IsFavorites = IsFavorites == true ? false : true;
                dbContext.Entry(address).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public FrayteResult EraseCustomerAddress(int CustomerId, string AddressType)
        {
            FrayteResult result = new FrayteResult();

            if (AddressType == FrayteFromToAddressType.FromAddress)
            {
                var fromdata = (from DB in dbContext.DirectShipments
                                join DSA in dbContext.DirectShipmentAddresses on DB.FromAddressId equals DSA.DirectShipmentAddressId
                                where DB.CustomerId == CustomerId && DSA.IsActive == true
                                select new
                                {
                                    DirectAddressId = DSA.DirectShipmentAddressId
                                }).ToList();

                if (fromdata != null && fromdata.Count > 0)
                {
                    foreach (var Obj in fromdata)
                    {
                        var dsa = dbContext.DirectShipmentAddresses.Where(p => p.DirectShipmentAddressId == Obj.DirectAddressId).FirstOrDefault();
                        if (dsa != null)
                        {
                            dsa.IsActive = false;
                            dbContext.Entry(dsa).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                    result.Status = true;
                }

                var todata = (from AB in dbContext.AddressBooks
                              where AB.CustomerId == CustomerId &&
                                    AB.FromAddress == true && AB.IsActive == true
                              select new
                              {
                                  AddressBookId = AB.AddressBookId
                              }).ToList();

                if (todata != null && todata.Count > 0)
                {
                    foreach (var Obj in todata)
                    {
                        var ad = dbContext.AddressBooks.Where(p => p.AddressBookId == Obj.AddressBookId).FirstOrDefault();
                        if (ad != null)
                        {
                            ad.IsActive = false;
                            dbContext.Entry(ad).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                    result.Status = true;
                }
            }
            else if (AddressType == FrayteFromToAddressType.ToAddress)
            {
                var fromdata = (from DB in dbContext.DirectShipments
                                join DSA in dbContext.DirectShipmentAddresses on DB.ToAddressId equals DSA.DirectShipmentAddressId
                                where DB.CustomerId == CustomerId && DSA.IsActive == true
                                select new
                                {
                                    DirectAddressId = DSA.DirectShipmentAddressId
                                }).ToList();

                if (fromdata != null && fromdata.Count > 0)
                {
                    foreach (var Obj in fromdata)
                    {
                        var dsa = dbContext.DirectShipmentAddresses.Where(p => p.DirectShipmentAddressId == Obj.DirectAddressId).FirstOrDefault();
                        if (dsa != null)
                        {
                            dsa.IsActive = false;
                            dbContext.Entry(dsa).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                    result.Status = true;
                }

                var todata = (from AB in dbContext.AddressBooks
                              where AB.CustomerId == CustomerId &&
                                    AB.ToAddress == true && AB.IsActive == true
                              select new
                              {
                                  AddressBookId = AB.AddressBookId
                              }).ToList();

                if (todata != null && todata.Count > 0)
                {
                    foreach (var Obj in todata)
                    {
                        var ad = dbContext.AddressBooks.Where(p => p.AddressBookId == Obj.AddressBookId).FirstOrDefault();
                        if (ad != null)
                        {
                            ad.IsActive = false;
                            dbContext.Entry(ad).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                    result.Status = true;
                }
            }
            else if (AddressType == FrayteFromToAddressType.AllAddress)
            {
                //Step 1 Erase All From Address
                EraseAllFromAddress(CustomerId, AddressType);

                //Step 2 Erase All To Address
                EraseAllToAddress(CustomerId, AddressType);
            }
            return result;
        }

        public void EraseAllFromAddress(int CustomerId, string AddressType)
        {
            var fromShipdata = (from DB in dbContext.DirectShipments
                                join DSA in dbContext.DirectShipmentAddresses on DB.FromAddressId equals DSA.DirectShipmentAddressId
                                where DB.CustomerId == CustomerId && DSA.IsActive == true
                                select new
                                {
                                    DirectAddressId = DSA.DirectShipmentAddressId
                                }).ToList();

            if (fromShipdata != null && fromShipdata.Count > 0)
            {
                foreach (var Obj in fromShipdata)
                {
                    var dsa = dbContext.DirectShipmentAddresses.Where(p => p.DirectShipmentAddressId == Obj.DirectAddressId).FirstOrDefault();
                    if (dsa != null)
                    {
                        dsa.IsActive = false;
                        dbContext.Entry(dsa).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
            }

            var frombookdata = (from AB in dbContext.AddressBooks
                                where AB.CustomerId == CustomerId &&
                                      AB.FromAddress == true && AB.IsActive == true
                                select new
                                {
                                    AddressBookId = AB.AddressBookId
                                }).ToList();

            if (frombookdata != null && frombookdata.Count > 0)
            {
                foreach (var Obj in frombookdata)
                {
                    var ad = dbContext.AddressBooks.Where(p => p.AddressBookId == Obj.AddressBookId).FirstOrDefault();
                    if (ad != null)
                    {
                        ad.IsActive = false;
                        dbContext.Entry(ad).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        public void EraseAllToAddress(int CustomerId, string AddressType)
        {
            var fromShipdata = (from DB in dbContext.DirectShipments
                                join DSA in dbContext.DirectShipmentAddresses on DB.ToAddressId equals DSA.DirectShipmentAddressId
                                where DB.CustomerId == CustomerId && DSA.IsActive == true
                                select new
                                {
                                    DirectAddressId = DSA.DirectShipmentAddressId
                                }).ToList();

            if (fromShipdata != null && fromShipdata.Count > 0)
            {
                foreach (var Obj in fromShipdata)
                {
                    var dsa = dbContext.DirectShipmentAddresses.Where(p => p.DirectShipmentAddressId == Obj.DirectAddressId).FirstOrDefault();
                    if (dsa != null)
                    {
                        dsa.IsActive = false;
                        dbContext.Entry(dsa).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
            }

            var frombookdata = (from AB in dbContext.AddressBooks
                                where AB.CustomerId == CustomerId &&
                                      AB.ToAddress == true && AB.IsActive == true
                                select new
                                {
                                    AddressBookId = AB.AddressBookId
                                }).ToList();

            if (frombookdata != null && frombookdata.Count > 0)
            {
                foreach (var Obj in frombookdata)
                {
                    var ad = dbContext.AddressBooks.Where(p => p.AddressBookId == Obj.AddressBookId).FirstOrDefault();
                    if (ad != null)
                    {
                        ad.IsActive = false;
                        dbContext.Entry(ad).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        public FrayteResult DeleteCustomerAddress(int AddressId, string TableType)
        {
            FrayteResult result = new FrayteResult();

            if (TableType == FrayteTableType.DirectBooking)
            {
                var dsa = dbContext.DirectShipmentAddresses.Where(p => p.DirectShipmentAddressId == AddressId).FirstOrDefault();
                if (dsa != null)
                {
                    dsa.IsActive = false;
                    dbContext.Entry(dsa).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
                result.Status = true;
            }
            else if (TableType == FrayteTableType.AddressBook)
            {
                var ad = dbContext.AddressBooks.Where(p => p.AddressBookId == AddressId).FirstOrDefault();
                if (ad != null)
                {
                    ad.IsActive = false;
                    dbContext.Entry(ad).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
                result.Status = true;
            }
            return result;
        }

        public FrayteResult SetPrintPackageStatus(Package package)
        {
            FrayteResult result = new FrayteResult();

            if (package != null)
            {
                var packageDeatil = dbContext.PackageTrackingDetails.Find(package.PackageTrackingDetailId);
                if (packageDeatil != null)
                {
                    packageDeatil.IsPrinted = true;
                    dbContext.SaveChanges();
                    result.Status = true;
                }
                else
                {
                    result.Status = false;
                }
            }
            else
            {
                result.Status = false;
            }
            return result;
        }

        public List<DirectBookingCustomer> GetDirectBookingCustomers(int userId, string moduleType)
        {
            // To Do : customer should come according to moduleType
            var operationzone = UtilityRepository.GetOperationZone();

            var userDetail = (from r in dbContext.Users
                              join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                              where r.UserId == userId
                              select new
                              {
                                  UserEmail = r.UserEmail,
                                  RoleId = ur.RoleId
                              }).FirstOrDefault();

            if (userDetail.RoleId == (int)FrayteUserRole.Customer)
            {
                var customers = (from r in dbContext.UserCustomers
                                 join u in dbContext.Users on r.CustomerId equals u.UserId
                                 join ua in dbContext.UserAdditionals on r.CustomerId equals ua.UserId
                                 join ur in dbContext.UserRoles on r.CustomerId equals ur.UserId
                                 where
                                    r.UserId == userId &&
                                    u.IsActive == true
                                 select new DirectBookingCustomer
                                 {
                                     CustomerId = u.UserId,
                                     CustomerName = u.ContactName,
                                     CompanyName = u.CompanyName,
                                     AccountNumber = ua.AccountNo,
                                     EmailId = u.UserEmail,
                                     ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                     CustomerCurrency = ua.CreditLimitCurrencyCode,
                                     OperationZoneId = u.OperationZoneId,
                                     IsShipperTaxAndDuty = ua.IsShipperTaxAndDuty.HasValue ? ua.IsShipperTaxAndDuty.Value : false
                                 }).Distinct().ToList();

                var customer = (from r in dbContext.Users
                                join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                                join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                                where
                                  r.UserId == userId &&
                                  r.IsActive == true
                                select new DirectBookingCustomer
                                {
                                    CustomerId = r.UserId,
                                    CustomerName = r.ContactName,
                                    CompanyName = r.CompanyName,
                                    AccountNumber = ua.AccountNo,
                                    EmailId = r.UserEmail,
                                    ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                    CustomerCurrency = ua.CreditLimitCurrencyCode,
                                    IsShipperTaxAndDuty = ua.IsShipperTaxAndDuty.HasValue ? ua.IsShipperTaxAndDuty.Value : false,
                                    OperationZoneId = r.OperationZoneId
                                }).FirstOrDefault();
                if (customer != null)
                {
                    customers.Add(customer);
                }

                return customers.OrderBy(p => p.CompanyName).ToList();
            }
            else if (userDetail.RoleId == (int)FrayteUserRole.CustomerStaff)
            {
                var customers = (from cs in dbContext.CustomerStaffs
                                 join r in dbContext.Users on cs.UserId equals r.UserId
                                 join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                                 where
                                    cs.CustomerStaffId == userId &&
                                    cs.IsActive == true &&
                                    r.IsActive == true &&
                                    r.OperationZoneId == operationzone.OperationZoneId
                                 select new DirectBookingCustomer
                                 {
                                     CustomerId = r.UserId,
                                     CustomerName = r.ContactName,
                                     CompanyName = r.CompanyName,
                                     AccountNumber = ua.AccountNo,
                                     EmailId = r.UserEmail,
                                     ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                     CustomerCurrency = ua.CreditLimitCurrencyCode,
                                     OperationZoneId = r.OperationZoneId,
                                     IsShipperTaxAndDuty = ua.IsShipperTaxAndDuty.HasValue ? ua.IsShipperTaxAndDuty.Value : false
                                 }).Distinct().ToList();

                return customers.OrderBy(p => p.CompanyName).ToList();
            }
            else
            {
                var customers = (from r in dbContext.Users
                                 join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                                 join ur in dbContext.UserRoles on r.UserId equals ur.UserId
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
                                     EmailId = r.UserEmail,
                                     ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                     CustomerCurrency = ua.CreditLimitCurrencyCode,
                                     OperationZoneId = r.OperationZoneId,
                                     IsShipperTaxAndDuty = ua.IsShipperTaxAndDuty.HasValue ? ua.IsShipperTaxAndDuty.Value : false
                                 }).Distinct().ToList();

                return customers.OrderBy(p => p.CompanyName).ToList();
            }
        }

        public FrayteResult DeleteDirectShipmetPcsDetail(int DirectShipmentDetailId)
        {
            FrayteResult result = new FrayteResult();
            var directShipmentdetail = dbContext.DirectShipmentDetails.Find(DirectShipmentDetailId);
            if (directShipmentdetail != null)
            {
                dbContext.DirectShipmentDetails.Remove(directShipmentdetail);
                dbContext.SaveChanges();
                result.Status = true;
            }
            return result;
        }

        public string GetTrackingNo(int DirectShipmentId)
        {
            string TrackingNo = string.Empty;
            if (DirectShipmentId > 0)
            {
                var Obj = dbContext.DirectShipments.Find(DirectShipmentId);
                if (Obj != null)
                {
                    if (Obj.TrackingDetail.Contains("order"))
                    {
                        var track = (from DS in dbContext.DirectShipments
                                     join DSD in dbContext.DirectShipmentDetails on DS.DirectShipmentId equals DSD.DirectShipmentId
                                     join PTD in dbContext.PackageTrackingDetails on DSD.DirectShipmentDetailId equals PTD.DirectShipmentDetailId
                                     where DS.DirectShipmentId == DirectShipmentId
                                     select new
                                     {
                                         PTD.TrackingNo
                                     }).FirstOrDefault();

                        if (track != null)
                        {
                            TrackingNo = track.TrackingNo;
                        }
                    }
                    else
                    {
                        TrackingNo = Obj.TrackingDetail;
                    }
                }
            }
            return TrackingNo;
        }

        public List<FraytePostCodeAddress> PostCodeAddress(string PostCode, string CountryCode)
        {
            List<FraytePostCodeAddress> _address = new List<FraytePostCodeAddress>();
            try
            {
                FraytePostCodeAddress postcode;
                var postCode = UtilityRepository.PostCodeVerification(PostCode, CountryCode);
                var result = dbContext.spGet_PostCodeAddress(postCode).ToList();
                if (result != null && result.Count > 0)
                {
                    foreach (var Obj in result)
                    {
                        postcode = new FraytePostCodeAddress();
                        postcode.CompanyName = Obj.CompanyName;
                        postcode.Address1 = Obj.Address1;
                        postcode.Address2 = Obj.Address2;
                        postcode.Area = Obj.Area;
                        postcode.City = Obj.City;
                        postcode.PostCode = Obj.PostCode;
                        _address.Add(postcode);
                    }
                }
                return _address;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        public List<FrayteCommercialInvoice> CreateDirectBookingCommercialInvoice(int DirectShipmnetId)
        {
            var invoice = (from ds in dbContext.DirectShipments
                           join dsd in dbContext.DirectShipmentDetails on ds.DirectShipmentId equals dsd.DirectShipmentId
                           join ptd in dbContext.PackageTrackingDetails on dsd.DirectShipmentDetailId equals ptd.DirectShipmentDetailId
                           join da in dbContext.DirectShipmentAddresses on ds.FromAddressId equals da.DirectShipmentAddressId
                           join da1 in dbContext.DirectShipmentAddresses on ds.ToAddressId equals da1.DirectShipmentAddressId
                           join ca in dbContext.Countries on da.CountryId equals ca.CountryId
                           join ca1 in dbContext.Countries on da1.CountryId equals ca1.CountryId
                           where ds.DirectShipmentId == DirectShipmnetId
                           select new FrayteCommercialInvoice
                           {
                               ShipperInfo = " " + da.ContactFirstName + " " + da.ContactLastName + "\n " + da.CompanyName + "\n " + da.Address1 + "\n " + da.Address2 + "\n " + da.City + "\n " + da.State + "\n " + ca.CountryName,
                               ConsigneeInfo = " " + da1.ContactFirstName + " " + da1.ContactLastName + "\n " + da1.CompanyName + "\n " + da1.Address1 + "\n " + da1.Address2 + "\n " + da1.City + "\n " + da1.State + "\n " + ca1.CountryName,
                               Reference = "", //ds.Reference1,
                               DateOfExport = DateTime.UtcNow, //ds.CreatedOn,
                               Currency = "", //ds.CurrencyCode,
                               FromCountry = "", //ca.CountryName,
                               ToCountry = "", //ca1.CountryName,
                               AirWayBillNo = "", //ds.TrackingDetail.Contains("order_") ? ptd.TrackingNo : ds.TrackingDetail,
                               TotalValue = 0,
                               PackageInfo = new List<FrayteCommercialInvoicePackageInfo>()
                               {
                                   new FrayteCommercialInvoicePackageInfo()
                                   {
                                       ShipmentContents = "", //dsd.PiecesContent,
                                       Quantity = 0, //dsd.CartoonValue,
                                       DeclaredValue = 0, //dsd.DeclaredValue.HasValue ? dsd.DeclaredValue.Value : 0.0m
                                   },
                               }
                           }).ToList();

            return invoice;
        }

        public FrayteCommercilaFileName GetCommercilaInvoiceFileName(int DirectShipmnetId)
        {
            var detail = (from DS in dbContext.DirectShipments
                          join UU in dbContext.Users on DS.CustomerId equals UU.UserId
                          join CA in dbContext.LogisticServiceCourierAccounts on DS.CourierAccountId equals CA.LogisticServiceCourierAccountId
                          join LS in dbContext.LogisticServices on CA.LogisticServiceId equals LS.LogisticServiceId
                          where DS.DirectShipmentId == DirectShipmnetId
                          select new FrayteCommercilaFileName
                          {
                              CompanyName = UU.CompanyName,
                              FrayetNo = DS.FrayteNumber,
                              LogisticType = LS.LogisticTypeDisplay,
                              RateType = LS.RateTypeDisplay,
                              CreationDate = DateTime.UtcNow
                          }).FirstOrDefault();

            if (detail != null)
            {
                return detail;
            }
            else
            {
                return null;
            }
        }

        public void SaveDirectShipmentObject(string ShipmetObject, int DirectShipmentDraftId)
        {
            var detail = dbContext.DirectShipmentDrafts.Find(DirectShipmentDraftId);
            if (detail != null)
            {
                detail.EasyPostOrderObject = ShipmetObject;
                detail.LastUpdated = DateTime.UtcNow;
                dbContext.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void SaveEasyPostErrorObject(string ErrorObject, string ShipmetObject, int DirectShipmentDraftId)
        {
            var detail = dbContext.DirectShipmentDrafts.Find(DirectShipmentDraftId);
            if (detail != null)
            {
                detail.EasyPostErrorObject = ErrorObject;
                detail.EasyPostOrderObject = ShipmetObject;
                detail.LastUpdated = DateTime.UtcNow;
                dbContext.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void SaveEasyPosyPickUpObject(string PickUpObject, string ShipmetObject, int DirectShipmentDraftId)
        {
            var detail = dbContext.DirectShipmentDrafts.Find(DirectShipmentDraftId);
            if (detail != null)
            {
                detail.EasyPostPickUpObject = PickUpObject;
                detail.EasyPostOrderObject = ShipmetObject;
                detail.LastUpdated = DateTime.UtcNow;
                dbContext.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public FrayteManifestName DownloadSupplemantoryChargePdf(int DirectShipmentId)
        {
            FrayteManifestName name = new FrayteManifestName();
            var filename = (from ds in dbContext.DirectShipments
                            join lsst in dbContext.LogisticServiceShipmentTypes on ds.ShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                            join ls in dbContext.LogisticServices on lsst.LogisticServiceId equals ls.LogisticServiceId
                            where ds.DirectShipmentId == DirectShipmentId
                            select ls).FirstOrDefault();

            if (filename.SupplemantoryFileName != null && filename.SupplemantoryFileName != "")
            {
                name.FileName = filename.SupplemantoryFileName;
                name.FilePath = AppSettings.WebApiPath + "SupplemantoryChargeFile/" + filename.SupplemantoryFileName;
                name.FileStatus = true;
            }
            else
            {
                name.LogisticCompany = filename.LogisticCompany;
                name.OperationZoneId = filename.OperationZoneId;
            }
            return name;
        }

        public List<FrayteCountryState> CountryStateCode(int CountryId)
        {
            List<FrayteCountryState> _code = new List<FrayteCountryState>();

            var item = (from CSC in dbContext.CountryStates
                        where CSC.CountryId == CountryId
                        select new FrayteCountryState
                        {
                            StateName = CSC.StateName,
                            StateCode = CSC.StateCode
                        }).ToList();

            if (item != null)
            {
                FrayteCountryState state;
                foreach (var Obj in item)
                {
                    state = new FrayteCountryState();
                    state.StateName = Obj.StateName;
                    state.StateCode = Obj.StateCode;
                    _code.Add(state);
                }
                return _code;
            }
            else
            {
                return _code;
            }
        }

        #region -- Upload and Download excel for Pieces Grid --

        public List<Package> GetPiecesDetail(System.Data.DataTable exceldata)
        {
            List<Package> _shipmentdetail = new List<Package>();
            Package frayteshipment;

            foreach (DataRow shipmentdetail in exceldata.Rows)
            {
                frayteshipment = new Package();

                frayteshipment.DirectShipmentDetailId = 0;
                frayteshipment.LabelName = "";
                frayteshipment.PackageTrackingDetailId = 0;
                frayteshipment.TrackingNo = "";
                frayteshipment.IsPrinted = false;
                frayteshipment.Content = shipmentdetail["ShipmentContents"] != null ? CommonConversion.ConvertToString(shipmentdetail, "ShipmentContents") : ""; //shipmentdetail["ShipmentContents"].ToString() : "";
                frayteshipment.Value = shipmentdetail["DeclaredValue"] != null ? CommonConversion.ConvertToDecimal(shipmentdetail, "DeclaredValue") : 0; //Convert.ToDecimal(shipmentdetail["DeclaredValue"].ToString()) : 0;
                frayteshipment.CartoonValue = shipmentdetail["CartonQTY"] != null ? CommonConversion.ConvertToInt(shipmentdetail, "CartonQTY") : 0;// Convert.ToInt32(shipmentdetail["CartonQTY"].ToString()) : 0;
                frayteshipment.Weight = shipmentdetail["Weight"] != null ? CommonConversion.ConvertToDecimal(shipmentdetail, "Weight") : 0;//Convert.ToDecimal(shipmentdetail["Weight"].ToString()) : 0;
                frayteshipment.Length = shipmentdetail["Length"] != null ? CommonConversion.ConvertToDecimal(shipmentdetail, "Length") : 0;// Convert.ToDecimal(shipmentdetail["Length"].ToString()) : 0;
                frayteshipment.Width = shipmentdetail["Width"] != null ? CommonConversion.ConvertToDecimal(shipmentdetail, "Width") : 0;//Convert.ToDecimal(shipmentdetail["Width"].ToString()) : 0;
                frayteshipment.Height = shipmentdetail["Height"] != null ? CommonConversion.ConvertToDecimal(shipmentdetail, "Height") : 0;//Convert.ToDecimal(shipmentdetail["Height"].ToString()) : 0;

                _shipmentdetail.Add(frayteshipment);
            }

            return _shipmentdetail;
        }

        public string getExcelConnectionString(string FileName, string filepath)
        {

            // Microsoft.ACE.OLEDB.12.0
            if (Path.GetExtension(FileName) == ".xlsx")
            {
                return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=Excel 12.0";
            }
            else if (Path.GetExtension(FileName) == ".xls")
            {
                //return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties=Excel 8.0";
                return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=Excel 8.0";
            }
            else if (Path.GetExtension(FileName) == ".csv")
            {
                //   var connString = string.Format(@"Provider=Microsoft.Jet.OleDb.4.0; Data Source={0};Extended Properties=""Text;HDR=YES;FMT=Delimited""", Path.GetDirectoryName(filepath));
                var connString = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0};Extended Properties=""Text;HDR=YES;FMT=Delimited""", Path.GetDirectoryName(filepath));
                //  var path = "E://IRA//frayte//Code//Frayte//Frayte.WebApi//UploadFiles//Shipments//01_23_2017_12_22_59_PiecesDetail.csv";
                //  return "Provider=Microsoft.Jet.OleDb.4.0;Data Source=" + filepath + ";Extended Properties=Text;HDR=YES;FMT=Delimited";

                return connString;
                //"Provider=Microsoft.Jet.OleDb.4.0; Data Source = " + 
                //System.IO.Path.GetDirectoryName(strFileName) +"; Extended Properties = \"Text;HDR=YES;FMT=Delimited\""
            }
            return "";
        }

        public string getFileExtensionString(string FileName)
        {

            if (Path.GetExtension(FileName) == ".xlsx")
            {
                return FrayteFileExtension.EXCEL;
            }
            else if (Path.GetExtension(FileName) == ".xls")
            {
                return FrayteFileExtension.EXCEL;
            }
            else if (Path.GetExtension(FileName) == ".csv")
            {
                return FrayteFileExtension.CSV;
            }

            return "";
        }

        public bool CheckValidExcel(System.Data.DataTable table)
        {
            bool valid = true;

            DataColumnCollection columns = table.Columns;

            if (!columns.Contains("JobNumber"))
            {
                valid = false;
            }
            if (!columns.Contains("JobStyle"))
            {
                valid = false;
            }
            if (!columns.Contains("HSCode"))
            {
                valid = false;
            }
            if (!columns.Contains("CartonQty"))
            {
                valid = false;
            }
            if (!columns.Contains("Pieces"))
            {
                valid = false;
            }
            if (!columns.Contains("WeightKg"))
            {
                valid = false;
            }
            if (!columns.Contains("Lcms"))
            {
                valid = false;
            }
            if (!columns.Contains("Wcms"))
            {
                valid = false;
            }
            if (!columns.Contains("Hcms"))
            {
                valid = false;
            }
            if (!columns.Contains("PiecesContent"))
            {
                valid = false;
            }

            return valid;
        }

        #endregion

        #region -- After Ship --

        #region Get Tracking Detail for single tracking number from Aftership

        public FrayteShipmentTracking GetTracking(string carrierName, string trackingNumber)
        {
            try
            {
                var logisticIntegration = dbContext.LogisticIntegrations.Where(p => p.Name == FrayteIntegration.Aftership).FirstOrDefault();

                //AftershipAPI.Tracking tracking = new AftershipAPI.Tracking(trackingNumber);
                //tracking.slug = "dhl";

                // For live tracking

                List<AftershipAPI.Tracking> trackings = new List<Tracking>();

                AftershipAPI.Tracking tracking;
                if (!string.IsNullOrEmpty(carrierName) && !string.IsNullOrEmpty(trackingNumber))
                {
                    carrierName = carrierName.ToUpper();

                    if (carrierName.Contains("DHL"))
                    {
                        tracking = new AftershipAPI.Tracking(trackingNumber);
                        tracking.slug = "dhl";
                        FrayteShipmentTracking frayteTracking = new FrayteShipmentTracking();
                        AftershipAPI.ConnectionAPI connApi = new AftershipAPI.ConnectionAPI(logisticIntegration.InetgrationKey, logisticIntegration.Url);
                        tracking = connApi.getTrackingByNumber(tracking);
                        trackings.Add(tracking);
                        FrayteShipmentTracking track = mapAftershiptrackingToFrayteTracking(trackings);
                        return track;
                    }
                    else if (carrierName.Contains("TNT"))
                    {
                        tracking = new AftershipAPI.Tracking(trackingNumber);
                        tracking.slug = "tnt";
                        FrayteShipmentTracking frayteTracking = new FrayteShipmentTracking();
                        AftershipAPI.ConnectionAPI connApi = new AftershipAPI.ConnectionAPI(logisticIntegration.InetgrationKey, logisticIntegration.Url);
                        tracking = connApi.getTrackingByNumber(tracking);
                        trackings.Add(tracking);
                        FrayteShipmentTracking track = mapAftershiptrackingToFrayteTracking(trackings);
                        return track;
                    }
                    else if (carrierName.Contains("UPS") || carrierName.Contains("ups"))
                    {
                        tracking = new AftershipAPI.Tracking(trackingNumber);
                        tracking.slug = "ups";
                        FrayteShipmentTracking frayteTracking = new FrayteShipmentTracking();
                        AftershipAPI.ConnectionAPI connApi = new AftershipAPI.ConnectionAPI(logisticIntegration.InetgrationKey, logisticIntegration.Url);
                        tracking = connApi.getTrackingByNumber(tracking);
                        trackings.Add(tracking);
                        FrayteShipmentTracking track = mapAftershiptrackingToFrayteTracking(trackings);
                        return track;
                    }
                    //else if (trackingNumber.Contains("Yodel"))
                    //{
                    //    tracking.slug = "yodel";
                    //}
                    //else if (trackingNumber.Contains("Hermese"))
                    //{
                    //    tracking.slug = "hermese";
                    //}
                    //else if (trackingNumber.Contains("UKiMail"))
                    //{
                    //    tracking.slug = "ukmail";
                    //}

                }
                return null;
            }
            catch (Exception e)
            {

                return null;
            }
        }

        #region Send Status Email on WebHook

        public FrayteResult SendTrackingStatusEmail(AftershipWebhookObject webHookDetail)
        {
            FrayteAftershipmentTrackingEmail obj = new FrayteAftershipmentTrackingEmail();
            FrayteResult result = new FrayteResult();
            var logisticIntegration = dbContext.LogisticIntegrations.Where(p => p.Name == FrayteIntegration.Aftership).FirstOrDefault();
            if (logisticIntegration != null)
            {
                obj.TrackingLink = logisticIntegration.VoidApiUrl;
            }

            var trackingConfiguration = (from r in dbContext.Users
                                         join tc in dbContext.UserTrackingConfigurations on r.UserId equals tc.UserId
                                         where r.UserEmail == webHookDetail.msg.custom_fields.customer_email
                                         select new FrayteTrackingConfiguration
                                         {
                                             CustomerId = r.UserId,
                                             AttemptFailEmails = !string.IsNullOrEmpty(tc.AttemptFailEmails) ? tc.AttemptFailEmails.Split(';').Select(p => new FrayteEmailModel { Email = p }).ToList() : null,
                                             DeliveredEmails = !string.IsNullOrEmpty(tc.DeliveredEmails) ? tc.DeliveredEmails.Split(';').Select(p => new FrayteEmailModel { Email = p }).ToList() : null,
                                             ExceptionEmails = !string.IsNullOrEmpty(tc.ExceptionEmails) ? tc.ExceptionEmails.Split(';').Select(p => new FrayteEmailModel { Email = p }).ToList() : null,
                                             ExpiredEmails = !string.IsNullOrEmpty(tc.ExpiredEmails) ? tc.ExpiredEmails.Split(';').Select(p => new FrayteEmailModel { Email = p }).ToList() : null,
                                             InfoReceivedEmails = !string.IsNullOrEmpty(tc.ExpiredEmails) ? tc.ExpiredEmails.Split(';').Select(p => new FrayteEmailModel { Email = p }).ToList() : null,
                                             InTransitEmails = !string.IsNullOrEmpty(tc.InTransitEmails) ? tc.InTransitEmails.Split(';').Select(p => new FrayteEmailModel { Email = p }).ToList() : null,
                                             OutForDeliveryEmails = !string.IsNullOrEmpty(tc.OutForDeliveryEmails) ? tc.OutForDeliveryEmails.Split(';').Select(p => new FrayteEmailModel { Email = p }).ToList() : null,
                                             UserTrackingConfigurationId = tc.UserTrackingConfigurationId
                                         }
                                         ).FirstOrDefault();

            obj.ts = webHookDetail.ts;
            obj.@event = webHookDetail.@event;
            obj.msg = new Msg();
            obj.msg = webHookDetail.msg;
            obj.msg.checkpoints = webHookDetail.msg.checkpoints.OrderByDescending(p => p.created_at).ThenByDescending(p => p.checkpoint_time).ToList();

            if (webHookDetail.msg.tag == FrayteAftershipStatusTagString.Delivered)
            {
                result = new ShipmentEmailRepository().SendAftershipTrackingEmail(obj, FrayteAftershipStatusTagString.Delivered, trackingConfiguration);
            }
            else if (webHookDetail.msg.tag == FrayteAftershipStatusTagString.OutForDelivery)
            {
                result = new ShipmentEmailRepository().SendAftershipTrackingEmail(obj, FrayteAftershipStatusTagString.OutForDelivery, trackingConfiguration);
            }
            else if (webHookDetail.msg.tag == FrayteAftershipStatusTagString.InTransit)
            {
                result = new ShipmentEmailRepository().SendAftershipTrackingEmail(obj, FrayteAftershipStatusTagString.InTransit, trackingConfiguration);
            }
            else if (webHookDetail.msg.tag == FrayteAftershipStatusTagString.InfoReceived)
            {
                result = new ShipmentEmailRepository().SendAftershipTrackingEmail(obj, FrayteAftershipStatusTagString.InfoReceived, trackingConfiguration);
            }
            else if (webHookDetail.msg.tag == FrayteAftershipStatusTagString.Exception)
            {
                result = new ShipmentEmailRepository().SendAftershipTrackingEmail(obj, FrayteAftershipStatusTagString.Exception, trackingConfiguration);
            }
            else if (webHookDetail.msg.tag == FrayteAftershipStatusTagString.AttemptFail)
            {
                result = new ShipmentEmailRepository().SendAftershipTrackingEmail(obj, FrayteAftershipStatusTagString.AttemptFail, trackingConfiguration);
            }

            return result;
        }

        #endregion

        private FrayteShipmentTracking mapAftershiptrackingToFrayteTracking(List<AftershipAPI.Tracking> trackers)
        {
            try
            {
                FrayteShipmentTracking frayteAftershipTrackingDetails = new FrayteShipmentTracking();

                frayteAftershipTrackingDetails.Tracking = new List<ShipmentTracking>();

                if (trackers != null && trackers.Count > 0)
                {
                    ShipmentTracking trackingDetail;
                    foreach (var tracker in trackers)
                    {
                        trackingDetail = new ShipmentTracking();
                        trackingDetail.TrackingDetails = new List<ShipmentTrackingDetail>();

                        trackingDetail.DestinationCountry = tracker.destinationCountryISO3.ToString();
                        trackingDetail.OriginCountry = tracker.originCountryISO3.ToString();
                        trackingDetail.Title = tracker.title;


                        trackingDetail.CarriertrackingId = tracker.id;
                        trackingDetail.IsHeaderShow = true;
                        trackingDetail.TrackingNumber = tracker.trackingNumber;
                        trackingDetail.ShowHideValue = "Hide";
                        trackingDetail.Carrier = tracker.slug;
                        trackingDetail.StatusId = (int)tracker.tag;
                        if (tracker.tag.ToString() == FrayteAftershipStatusTag.Pending.ToString())
                        {
                            trackingDetail.StatusDisplay = FrayteAftershipStatus.Pending;
                        }
                        else if (tracker.tag.ToString() == FrayteAftershipStatusTag.Delivered.ToString())
                        {
                            trackingDetail.StatusDisplay = FrayteAftershipStatus.Delivered;
                        }
                        else if (tracker.tag.ToString() == FrayteAftershipStatusTag.AttemptFail.ToString())
                        {
                            trackingDetail.StatusDisplay = FrayteAftershipStatus.AttemptFail;
                        }
                        else if (tracker.tag.ToString() == FrayteAftershipStatusTag.InTransit.ToString())
                        {
                            trackingDetail.StatusDisplay = FrayteAftershipStatus.InTransit;
                        }
                        else if (tracker.tag.ToString() == FrayteAftershipStatusTag.InfoReceived.ToString())
                        {
                            trackingDetail.StatusDisplay = FrayteAftershipStatus.InfoReceived;
                        }
                        else if (tracker.tag.ToString() == FrayteAftershipStatusTag.Exception.ToString())
                        {
                            trackingDetail.StatusDisplay = FrayteAftershipStatus.Exception;
                        }
                        else if (tracker.tag.ToString() == FrayteAftershipStatusTag.Expired.ToString())
                        {
                            trackingDetail.StatusDisplay = FrayteAftershipStatus.Expired;
                        }
                        else if (tracker.tag.ToString() == FrayteAftershipStatusTag.OutForDelivery.ToString())
                        {
                            trackingDetail.StatusDisplay = FrayteAftershipStatus.OutForDelivery;
                        }
                        trackingDetail.Status = !string.IsNullOrEmpty(tracker.tag.ToString()) ? tracker.tag.ToString().ToUpper() : "";
                        if (!string.IsNullOrEmpty(tracker.expectedDelivery))
                        //if (!string.IsNullOrEmpty(tracker.checkpoints.Last().checkpointTime))
                        {
                            //trackingDetail.EstimatedDeliveryDate = tracker.checkpoints.Last().checkpointTime;
                            trackingDetail.EstimatedDeliveryDate = tracker.expectedDelivery; //tracker.expectedDelivery.ToString("MM/dd/yyyy");
                                                                                             //var date = tracker.est_delivery_date.Value;
                            trackingDetail.EstimatedDeliveryTime = "";//tracker.est_delivery_date.Value.ToString("HH:MM");
                        }
                        trackingDetail.EstimatedWeight = 0; // tracker.w;
                        trackingDetail.SignedBy = tracker.signedBy;
                        trackingDetail.CreatedAtDate = tracker.createdAt.Date.ToString("MM/dd/yyyy");
                        trackingDetail.CreatedAtTime = tracker.createdAt.ToString("HH:MM");
                        trackingDetail.TrackingDetails = new List<ShipmentTrackingDetail>();
                        if (tracker.checkpoints != null && tracker.checkpoints.Count > 0)
                        {
                            foreach (var trackerdetail in tracker.checkpoints)
                            {
                                var str = trackerdetail.checkpointTime.Split(' ');
                                var time = str[1].Split(':');

                                var track = new ShipmentTrackingDetail();
                                StringBuilder sb = new StringBuilder();

                                if (!string.IsNullOrEmpty(trackerdetail.city))
                                    sb.Append(trackerdetail.city);
                                if (!string.IsNullOrEmpty(trackerdetail.state))
                                    sb.Append(" " + trackerdetail.state);
                                if (!string.IsNullOrEmpty(trackerdetail.zip))
                                    sb.Append(" - " + trackerdetail.zip);
                                if (!string.IsNullOrEmpty("" + trackerdetail.countryName))
                                    sb.Append(" " + trackerdetail.countryName);
                                var date = DateTime.Now;
                                track.IsCollapsed = false;
                                if (DateTime.TryParse(str[0], out date))
                                {
                                    track.Date = date;
                                }
                                else
                                {

                                }
                                track.Time = time[0] + ":" + time[1];
                                track.Tag = trackerdetail.tag;
                                track.Activity = trackerdetail.message;
                                track.Location = sb.ToString();
                                trackingDetail.TrackingDetails.Add(track);
                            }
                        }
                        var temp = trackingDetail.TrackingDetails
                             .OrderByDescending(p => p.Date)
                             .ThenByDescending(p => UtilityRepository.GetTimeFromString(UtilityRepository.GetFlatTimeString(p.Time)).HasValue ? UtilityRepository.GetTimeFromString(UtilityRepository.GetFlatTimeString(p.Time)).Value : DateTime.UtcNow.TimeOfDay).FirstOrDefault();
                        var temp1 = trackingDetail.TrackingDetails
                                                    .OrderBy(p => p.Date)
                                                    .ThenBy(p => UtilityRepository.GetTimeFromString(UtilityRepository.GetFlatTimeString(p.Time)).HasValue ? UtilityRepository.GetTimeFromString(UtilityRepository.GetFlatTimeString(p.Time)).Value : DateTime.UtcNow.TimeOfDay).FirstOrDefault();

                        if (temp != null && temp1 != null)
                        {
                            trackingDetail.InTransitDays = temp.Date.Subtract(temp1.Date).Days + 1;

                            trackingDetail.TrackingDetails = trackingDetail.TrackingDetails
                                                         .OrderByDescending(p => p.Date)
                                                         .ThenByDescending(p => UtilityRepository.GetTimeFromString(UtilityRepository.GetFlatTimeString(p.Time)).HasValue ? UtilityRepository.GetTimeFromString(UtilityRepository.GetFlatTimeString(p.Time)).Value : DateTime.UtcNow.TimeOfDay).ToList();

                        }

                        if (temp != null)
                        {
                            trackingDetail.UpdatedAtDate = temp.Date.ToString("MM/dd/yyyy");
                            trackingDetail.UpdatedAtTime = temp.Time;
                        }
                        else
                        {
                            trackingDetail.UpdatedAtDate = tracker.updatedAt.Date.ToString("MM/dd/yyyy");
                            trackingDetail.UpdatedAtTime = tracker.updatedAt.ToString("HH:MM");
                        }


                        frayteAftershipTrackingDetails.Status = true;
                        frayteAftershipTrackingDetails.Tracking.Add(trackingDetail);
                    }


                    return frayteAftershipTrackingDetails;
                }
                else
                {
                    frayteAftershipTrackingDetails.Tracking = new List<ShipmentTracking>();
                    frayteAftershipTrackingDetails.Status = true;
                    return frayteAftershipTrackingDetails;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region Get Multiple trackings 
        public FrayteShipmentTracking GetMultipleTrackings(TrackAfterShipTracking track)
        {
            try
            {
                var userDetail = dbContext.Users.Find(track.CustomerId);
                if (userDetail != null)
                {
                    var logisticIntegration = dbContext.LogisticIntegrations.Where(p => p.Name == FrayteIntegration.Aftership).FirstOrDefault();

                    FrayteShipmentTracking frayteTracking = new FrayteShipmentTracking();
                    AftershipAPI.ConnectionAPI connApi = new AftershipAPI.ConnectionAPI(logisticIntegration.InetgrationKey, logisticIntegration.Url);
                    ParametersTracking ParametersTrackings = new ParametersTracking();
                    ParametersTrackings.keyword = userDetail.Email;
                    if (track.StatusId == 0)
                    {
                        ParametersTrackings.addTag(AftershipAPI.Enums.StatusTag.Pending);
                    }
                    else if (track.StatusId == 0)
                    {
                        ParametersTrackings.addTag(AftershipAPI.Enums.StatusTag.Pending);
                    }
                    else if (track.StatusId == 1)
                    {
                        ParametersTrackings.addTag(AftershipAPI.Enums.StatusTag.InfoReceived);
                    }
                    else if (track.StatusId == 2)
                    {
                        ParametersTrackings.addTag(AftershipAPI.Enums.StatusTag.InTransit);
                    }
                    else if (track.StatusId == 3)
                    {
                        ParametersTrackings.addTag(AftershipAPI.Enums.StatusTag.OutForDelivery);
                    }
                    else if (track.StatusId == 4)
                    {
                        ParametersTrackings.addTag(AftershipAPI.Enums.StatusTag.AttemptFail);
                    }
                    else if (track.StatusId == 5)
                    {
                        ParametersTrackings.addTag(AftershipAPI.Enums.StatusTag.Delivered);
                    }
                    else if (track.StatusId == 6)
                    {
                        ParametersTrackings.addTag(AftershipAPI.Enums.StatusTag.Exception);
                    }
                    else if (track.StatusId == 7)
                    {
                        ParametersTrackings.addTag(AftershipAPI.Enums.StatusTag.Expired);
                    }

                    ParametersTrackings.page = track.Page;
                    ParametersTrackings.limit = track.Limit;
                    frayteTracking = mapAftershiptrackingToFrayteTracking(connApi.getTrackings(ParametersTrackings));

                    return frayteTracking;
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

        public FrayteShipmentTracking GetMultipleTrackings(int customerId)
        {
            try
            {
                var userDetail = (from r in dbContext.Users
                                  join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                                  where r.UserId == customerId
                                  select new
                                  {
                                      UserId = r.UserId,
                                      Email = r.Email,
                                      UserEmail = r.UserEmail,
                                      RoleId = ur.RoleId
                                  }
                                  ).FirstOrDefault();
                if (userDetail != null)
                {
                    var logisticIntegration = dbContext.LogisticIntegrations.Where(p => p.Name == FrayteIntegration.Aftership).FirstOrDefault();

                    FrayteShipmentTracking frayteTracking = new FrayteShipmentTracking();
                    AftershipAPI.ConnectionAPI connApi = new AftershipAPI.ConnectionAPI(logisticIntegration.InetgrationKey, logisticIntegration.Url);
                    ParametersTracking ParametersTrackings = new ParametersTracking();
                    ParametersTrackings.keyword = (userDetail.RoleId == (int)FrayteUserRole.UserCustomer || userDetail.RoleId == (int)FrayteUserRole.Customer) ? userDetail.Email : userDetail.Email;

                    frayteTracking = mapAftershiptrackingToFrayteTracking(connApi.getTrackings(ParametersTrackings));
                    return frayteTracking;
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


        #endregion

        #region Create Tracking
        public void CreateTracking(int directShipmentid, string directBooking)
        {
            throw new NotImplementedException();
        }

        public void CreateTracking(FrayteAfterShipTracking track)
        {
            try
            {

                var logisticIntegration = dbContext.LogisticIntegrations.Where(p => p.Name == FrayteIntegration.Aftership).FirstOrDefault();
                AftershipAPI.ConnectionAPI connApi = new AftershipAPI.ConnectionAPI(logisticIntegration.InetgrationKey, logisticIntegration.Url);
                AftershipAPI.Tracking tracking;
                tracking = new Tracking(track.TrackingNumber);
                tracking.customFields = new Dictionary<string, string>();
                tracking.customFields.Add("customer_name", track.CustomerName);
                tracking.customFields.Add("customer_email", track.CustomerEmail);
                tracking.customFields.Add("module_type", FrayteShipmentServiceType.DirectBooking);
                tracking.title = track.Title;
                if (AppSettings.ShipmentCreatedFrom == "BATCH")
                {
                    try
                    {
                        connApi.createTracking(tracking);
                    }
                    catch (Exception e)
                    {

                    }
                }
                else
                {
                    connApi.createTracking(tracking);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Could not create the tracking in aftership."));
            }

        }

        public FrayteAfterShipTracking MapDirectShipmentObjToAfterShip(int shipmentId, string moduleType)
        {
            try
            {
                FrayteAfterShipTracking tracking = new FrayteAfterShipTracking();
                if (moduleType == FrayteShipmentServiceType.DirectBooking)
                {
                    var detail = new DirectShipmentRepository().GetDirectBookingDetail(shipmentId, "");
                    var customerDetail = new CustomerRepository().GetCustomerDetail(detail.CustomerId);
                    tracking.TrackingNumber = detail.Packages[0].TrackingNo;
                    if (detail.CustomerRateCard.CourierName == FrayteCourierCompany.DHL)
                    {
                        tracking.Slug = FrayteCourierSlugs.DHL;
                    }
                    else if (detail.CustomerRateCard.CourierName == FrayteCourierCompany.UPS)
                    {
                        tracking.Slug = FrayteCourierSlugs.UPS;
                    }
                    else if (detail.CustomerRateCard.CourierName == FrayteCourierCompany.TNT)
                    {
                        tracking.Slug = FrayteCourierSlugs.TNT;
                    }

                    tracking.Title = detail.FrayteNumber;
                    tracking.CustomerEmail = customerDetail.Email;
                    tracking.CustomerName = customerDetail.CompanyName;
                    tracking.ModuleType = moduleType;

                }
                return tracking;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        #endregion

        #endregion

        public void UpdatePODMailInformation(int UserId)
        {
            try
            {
                var detail = (from ds in dbContext.DirectShipments
                              join ca in dbContext.LogisticServiceCourierAccounts on ds.CourierAccountId equals ca.LogisticServiceCourierAccountId
                              join ls in dbContext.LogisticServices on ca.LogisticServiceId equals ls.LogisticServiceId
                              where
                                ds.CustomerId == UserId &&
                                ds.ShipmentStatusId == 26 &&
                                ds.IsPODMailSent == false
                              select new
                              {
                                  ds.DirectShipmentId,
                                  ls.LogisticCompanyDisplay,
                                  ls.RateTypeDisplay,
                                  ds.TrackingDetail,
                                  ds.DeliveryDate,
                                  ds.DeliveryTime,
                                  ds.SignedBy
                              }).ToList();

                List<FraytePODInfomation> _pod = new List<FraytePODInfomation>();
                FraytePODInfomation info;

                if (detail != null && detail.Count > 0)
                {
                    foreach (var dd in detail)
                    {
                        info = new FraytePODInfomation();
                        info.DirectShipmentId = dd.DirectShipmentId;
                        info.Carrier = dd.LogisticCompanyDisplay + " " + (dd.RateTypeDisplay == null ? "" : dd.RateTypeDisplay);
                        info.TrackingNo = dd.TrackingDetail.Contains("Order_") ? dd.TrackingDetail.Replace("Order_", "") : dd.TrackingDetail;
                        info.DeliveryDate = dd.DeliveryDate.HasValue ? dd.DeliveryDate.Value.ToString("dd-MMM-yy") : string.Empty;
                        info.DeliveryTime = dd.DeliveryTime.HasValue ? dd.DeliveryTime.Value.ToString() : string.Empty;
                        info.SignedBy = dd.SignedBy;
                        _pod.Add(info);
                    }

                    //Send POD mail
                    FrayteResult result = new ShipmentEmailRepository().SendPODMail(_pod, UserId);

                    if (result.Status)
                    {
                        //Update POD information in Direct Shipment
                        foreach (FraytePODInfomation pod in _pod)
                        {
                            var shpment = dbContext.DirectShipments.Where(p => p.DirectShipmentId == pod.DirectShipmentId).FirstOrDefault();
                            if (shpment != null)
                            {
                                shpment.IsPODMailSent = true;
                                shpment.PODMailSentOn = DateTime.UtcNow;
                                dbContext.Entry(shpment).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public FrayteResult DeleteDirectBookingInformation(int DirectShipmentId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var shipment = dbContext.DirectShipments.Find(DirectShipmentId);
                if (shipment != null && shipment.DirectShipmentId > 0)
                {
                    //Sned Shipment Package Label Before Delete
                    new ShipmentEmailRepository().SendDirectShipmentDeleteInformation(shipment.DirectShipmentId, shipment.FrayteNumber);

                    var detail = dbContext.DirectShipmentDetails.Where(p => p.DirectShipmentId == shipment.DirectShipmentId).ToList();
                    if (detail.Count > 0)
                    {
                        foreach (DirectShipmentDetail dd in detail)
                        {
                            var package = dbContext.PackageTrackingDetails.Where(p => p.DirectShipmentDetailId == dd.DirectShipmentDetailId).ToList();
                            if (package.Count > 0)
                            {
                                foreach (PackageTrackingDetail pack in package)
                                {
                                    dbContext.Entry(pack).State = System.Data.Entity.EntityState.Deleted;
                                    dbContext.SaveChanges();
                                }
                            }

                            dbContext.Entry(dd).State = System.Data.Entity.EntityState.Deleted;
                            dbContext.SaveChanges();
                        }
                    }

                    //Remove Package Label with directory
                    if (Directory.Exists(AppSettings.FilePath + shipment.DirectShipmentId))
                    {
                        string[] filePaths = Directory.GetFiles(AppSettings.FilePath + shipment.DirectShipmentId);
                        if (filePaths.Length > 0)
                        {
                            foreach (string filePath in filePaths)
                            {
                                File.Delete(filePath);
                            }
                            Directory.Delete(AppSettings.FilePath + shipment.DirectShipmentId);
                        }
                        else
                        {
                            Directory.Delete(AppSettings.FilePath + shipment.DirectShipmentId);
                        }
                    }

                    dbContext.Entry(shipment).State = System.Data.Entity.EntityState.Deleted;
                    dbContext.SaveChanges();

                    var custom = dbContext.ShipmentCustomDetails.Where(p => p.ShipmentId == shipment.DirectShipmentId).FirstOrDefault();
                    if (custom != null && custom.ShipmentCustomDetailId > 0)
                    {
                        dbContext.Entry(custom).State = System.Data.Entity.EntityState.Deleted;
                        dbContext.SaveChanges();
                    }
                }
                result.Status = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Status = false;
                return result;
            }
        }

        public FrayteCountryCurrentDateTime GetCurrentDateTime(int CountryId)
        {
            FrayteCountryCurrentDateTime datetime = new FrayteCountryCurrentDateTime();

            var detail = (from c in dbContext.Countries
                          join tz in dbContext.Timezones on c.TimeZoneId equals tz.TimezoneId
                          where c.CountryId == CountryId
                          select tz).FirstOrDefault();

            if (detail != null)
            {
                var remoteTimeZone = TimeZoneInfo.FindSystemTimeZoneById(detail.Name);
                var remoteTime = TimeZoneInfo.ConvertTime(DateTime.Now, remoteTimeZone);

                datetime.CurrentDate = remoteTime.Date.ToString("dd-MMM-yyyy");
                datetime.CurrentTime = UtilityRepository.GetFormmatedTime(remoteTime.TimeOfDay.ToString().Replace(":", ""));
                return datetime;
            }
            else
            {
                return datetime;
            }
        }

        public bool SaveDirectBookingRef(string DirectBookingRefNo, int DirectBookingId)
        {
            var Result = dbContext.TrackingNumberRoutes.Where(a => a.Number == DirectBookingRefNo && a.ModuleType == FrayteShipmentServiceType.DirectBooking && a.IsFrayteNumber == true).FirstOrDefault();
            if (Result == null)
            {
                TrackingNumberRoute TNR = new TrackingNumberRoute();
                TNR.Number = DirectBookingRefNo;
                TNR.ShipmentId = DirectBookingId;
                TNR.ModuleType = FrayteShipmentServiceType.DirectBooking;
                TNR.IsFrayteNumber = true;
                dbContext.TrackingNumberRoutes.Add(TNR);
                dbContext.SaveChanges();
                return true;
            }
            return true;
        }

        public bool SaveDirectBookingTrackingNo(string DirectBookingTrackingNo, int DirectBookingId)
        {
            var Result = dbContext.TrackingNumberRoutes.Where(a => a.Number == DirectBookingTrackingNo && a.ModuleType == FrayteShipmentServiceType.DirectBooking && a.IsTrackingNumber == true).FirstOrDefault();
            if (Result == null)
            {
                TrackingNumberRoute TNR = new TrackingNumberRoute();
                TNR.Number = DirectBookingTrackingNo;
                TNR.ShipmentId = DirectBookingId;
                TNR.ModuleType = FrayteShipmentServiceType.DirectBooking;
                TNR.IsTrackingNumber = true;
                dbContext.TrackingNumberRoutes.Add(TNR);
                dbContext.SaveChanges();
                return true;
            }
            return true;
        }

        public bool SaveDirectBookingPiecesTrackingNo(string DirectBookingPiecesTrackingNo, int DirectBookingId)
        {
            var Result = dbContext.TrackingNumberRoutes.Where(a => a.Number == DirectBookingPiecesTrackingNo && a.ModuleType == FrayteShipmentServiceType.DirectBooking && a.IsPiecesTrackingNo == true).FirstOrDefault();
            if (Result == null)
            {
                TrackingNumberRoute TNR = new TrackingNumberRoute();
                TNR.Number = DirectBookingPiecesTrackingNo;
                TNR.ShipmentId = DirectBookingId;
                TNR.ModuleType = FrayteShipmentServiceType.DirectBooking;
                TNR.IsPiecesTrackingNo = true;
                dbContext.TrackingNumberRoutes.Add(TNR);
                dbContext.SaveChanges();
                return true;
            }
            return true;
        }

        /// <summary>
        /// Modification Done By Avinash 
        /// </summary>
        /// <param name="expressBookingDetail"></param>
        /// <param name="result"></param>
        /// <returns></returns>

        #region Avinash 17-APR-2019 & 18-Apr-2019

        public int SaveExpressShipment(ExpressShipmentModel expressBookingDetail, IntegrtaionResult result, string ShipmentType)
        {
            try
            {
                int expressId = SaveExpressShipment(expressBookingDetail, result.TrackingNumber, expressBookingDetail.CustomerId, result.PickupRef, result.ShipmentImage, ShipmentType);
                return expressId;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public int SaveExpressShipment(ExpressShipmentModel expressBookingDetail, string orderId, int CustomerId, string PickupRef, string ShipmentImange, string ShipmentType)
        {
            int ExpressId = 0;
            if (orderId != null || orderId != "")
            {

                //Step 1.0 Save Draft Data Into Direct Shipment Table
                dbContext.spGet_SaveDraftAsDirectShipment(expressBookingDetail.ExpressId, orderId, "DirectBooking", 0, DateTime.UtcNow, DirectBookingShippingStatus.Current, CustomerId, null, null, null, expressBookingDetail.FrayteNumber, PickupRef, ShipmentImange);

                //Step 1.1 Save Tracking No in DirectShipment Table
                Express directShipment = dbContext.Expresses.Where(p => p.TrackingNumber == orderId).FirstOrDefault();
                ExpressId = directShipment.ExpressId;

                //Save tracking no to trackingroute table
                if (!string.IsNullOrEmpty(orderId) && ExpressId > 0)
                {
                    SaveDirectBookingTrackingNo(orderId, ExpressId);
                }
                //Save FrayteRef no to trackingroute table
                if (!string.IsNullOrEmpty(directShipment.FrayteNumber) && ExpressId > 0)
                {
                    SaveDirectBookingRef(directShipment.FrayteNumber, ExpressId);
                }
                //Step 1.2 Save Direct Shipment Address from Addressbook
                ExpressAddress address;
                if (expressBookingDetail.ShipFrom != null)
                {
                    address = new ExpressAddress();
                    address.ContactFirstName = expressBookingDetail.ShipFrom.FirstName;
                    address.ContactLastName = expressBookingDetail.ShipFrom.LastName;
                    address.CompanyName = expressBookingDetail.ShipFrom.CompanyName;
                    address.Email = expressBookingDetail.ShipFrom.Email;
                    address.PhoneNo = expressBookingDetail.ShipFrom.Phone;
                    address.Address1 = expressBookingDetail.ShipFrom.Address;
                    address.Area = expressBookingDetail.ShipFrom.Area;
                    address.Address2 = expressBookingDetail.ShipFrom.Address2;
                    address.City = expressBookingDetail.ShipFrom.City;
                    address.State = expressBookingDetail.ShipFrom.State;
                    address.Zip = expressBookingDetail.ShipFrom.PostCode;
                    address.CountryId = expressBookingDetail.ShipFrom.Country.CountryId;
                    //address.IsActive = true;
                    //address.TableType = FrayteTableType.DirectBooking;
                    dbContext.ExpressAddresses.Add(address);
                    if (address != null)
                    {
                        dbContext.SaveChanges();
                    }
                    //Update Direct Shipment From Address Id
                    if (directShipment != null)
                    {
                        directShipment.FromAddressId = address.ExpressAddressId;
                        dbContext.SaveChanges();
                    }
                }
                if (expressBookingDetail.ShipTo != null)
                {
                    address = new ExpressAddress();
                    address.ContactFirstName = expressBookingDetail.ShipTo.FirstName;
                    address.ContactLastName = expressBookingDetail.ShipTo.LastName;
                    address.CompanyName = expressBookingDetail.ShipTo.CompanyName;
                    address.Email = expressBookingDetail.ShipTo.Email;
                    address.PhoneNo = expressBookingDetail.ShipTo.Phone;
                    address.Address1 = expressBookingDetail.ShipTo.Address;
                    address.Area = expressBookingDetail.ShipTo.Area;
                    address.Address2 = expressBookingDetail.ShipTo.Address2;
                    address.City = expressBookingDetail.ShipTo.City;
                    address.State = expressBookingDetail.ShipTo.State;
                    address.Zip = expressBookingDetail.ShipTo.PostCode;
                    address.CountryId = expressBookingDetail.ShipTo.Country.CountryId;
                    //  address.IsActive = true;
                    //  address.TableType = FrayteTableType.DirectBooking;
                    dbContext.ExpressAddresses.Add(address);

                    if (address != null)
                    {
                        dbContext.SaveChanges();
                    }

                    //updateSession Table

                    if (ShipmentType == "Express")
                    {

                        //UpdateExpressSession(expressBookingDetail);

                    }
                    else
                    {
                        //UpdateSession(dbDetail);
                    }

                    //Update Direct Shipment To Address Id
                    if (directShipment != null)
                    {
                        directShipment.ToAddressId = address.ExpressAddressId;
                        dbContext.SaveChanges();
                    }
                }

                //Step 1.21 Set default address in address book
                if (expressBookingDetail.ShipFrom.IsDefault)
                {
                    UserCountryAddress countryAddress;
                    var shipfromAdd = dbContext.AddressBooks.Find(expressBookingDetail.ShipFrom.ExpressAddressId);
                    if (shipfromAdd != null)
                    {

                        countryAddress = dbContext.UserCountryAddresses.Where(p => p.CustomerId == expressBookingDetail.CustomerId && p.CountryId == expressBookingDetail.ShipFrom.Country.CountryId && p.FromAddressId != null).FirstOrDefault();
                        if (countryAddress != null)
                        {
                            countryAddress.FromAddressId = shipfromAdd.AddressBookId;
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            countryAddress.CountryId = expressBookingDetail.ShipFrom.Country.CountryId;
                            countryAddress.CustomerId = expressBookingDetail.CustomerId;
                            countryAddress.FromAddressId = shipfromAdd.AddressBookId;
                            dbContext.UserCountryAddresses.Add(countryAddress);
                            dbContext.SaveChanges();
                        }
                        if (!shipfromAdd.IsDefault)
                        {
                            shipfromAdd.IsDefault = expressBookingDetail.ShipFrom.IsDefault;
                            dbContext.SaveChanges();
                        }
                    }
                }

                if (expressBookingDetail.ShipTo.IsDefault)
                {
                    var shiptoAdd = dbContext.AddressBooks.Find(expressBookingDetail.ShipTo.ExpressAddressId);
                    if (shiptoAdd != null)
                    {
                        UserCountryAddress countryAddress;
                        countryAddress = dbContext.UserCountryAddresses.Where(p => p.CustomerId == expressBookingDetail.CustomerId && p.CountryId == expressBookingDetail.ShipTo.Country.CountryId && p.ToAddressId != null).FirstOrDefault();
                        if (countryAddress != null)
                        {
                            countryAddress.FromAddressId = shiptoAdd.AddressBookId;
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            countryAddress.CountryId = expressBookingDetail.ShipTo.Country.CountryId;
                            countryAddress.CustomerId = expressBookingDetail.CustomerId;
                            countryAddress.ToAddressId = shiptoAdd.AddressBookId;
                            dbContext.UserCountryAddresses.Add(countryAddress);
                            dbContext.SaveChanges();
                        }
                        if (!shiptoAdd.IsDefault)
                        {
                            shiptoAdd.IsDefault = expressBookingDetail.ShipTo.IsDefault;
                            dbContext.SaveChanges();
                        }
                    }
                }

                //Step 1.3 Save NDS optional service detail
                DirectShipmentOptionalService ser;
                if (expressBookingDetail.Service.OptionalServices != null && expressBookingDetail.Service.OptionalServices.Count > 0)
                {
                    foreach (var detail in expressBookingDetail.Service.OptionalServices)
                    {
                        if (detail.IsEnable)
                        {
                            ser = new DirectShipmentOptionalService();
                            ser.LogisticOptionalServiceId = detail.LogisticOptionalServiceId;
                            ser.OptionalServiceCode = detail.ServiceCode;
                            ser.DirectShipmentId = ExpressId;
                            dbContext.DirectShipmentOptionalServices.Add(ser);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            return ExpressId;
        }

        public void SaveExpressEasyPosyPickUpObject(string PickUpObject, string ShipmetObject, int ExpressId)
        {
            var detail = dbContext.Expresses.Find(ExpressId);
            if (detail != null)
            {
                // Need to add Commented Columns to database Express 

                //detail.EasyPostPickUpObject = PickUpObject;
                //detail.EasyPostOrderObject = ShipmetObject;
                detail.CreatedOnUtc = DateTime.UtcNow;
                dbContext.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void SaveExpressPackageDetail(Package package, string ImageName, int ExpressDetailId, string CourierCompany, int count)
        {
            if (CourierCompany == FrayteLogisticServiceType.Hermes)
            {
                if (count == 0)
                {
                    var detail = dbContext.ExpressDetailPackageLabels.Where(p => p.TrackingNumber == package.LabelName).FirstOrDefault();
                    if (detail != null)
                    {
                        detail.IsDownloaded = true;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        ExpressDetailPackageLabel sph = new ExpressDetailPackageLabel();
                        sph.ExpressShipmentDetailId = ExpressDetailId;
                        sph.TrackingNumber = package.LabelName;
                        sph.PackageLabelName = ImageName;
                        sph.IsDownloaded = false;
                        dbContext.ExpressDetailPackageLabels.Add(sph);
                        dbContext.SaveChanges();
                        if (!string.IsNullOrEmpty(sph.TrackingNumber) && sph.ExpressDetailPackageLabelId > 0)
                        {
                            var Result = dbContext.ExpressDetailPackageLabels.Where(a => a.ExpressDetailPackageLabelId == sph.ExpressDetailPackageLabelId).FirstOrDefault();
                            if (Result != null)
                            {
                                SaveDirectBookingPiecesTrackingNo(sph.TrackingNumber, Result.ExpressDetailPackageLabelId);
                            }
                        }
                    }
                }
                else if (count > 0)
                {
                    ExpressDetailPackageLabel sph = new ExpressDetailPackageLabel();
                    sph.ExpressShipmentDetailId = ExpressDetailId;
                    sph.TrackingNumber = package.LabelName;
                    sph.PackageLabelName = ImageName;
                    sph.IsDownloaded = false;
                    dbContext.ExpressDetailPackageLabels.Add(sph);
                    dbContext.SaveChanges();
                    if (!string.IsNullOrEmpty(sph.TrackingNumber) && sph.ExpressDetailPackageLabelId > 0)
                    {
                        var Result = dbContext.ExpressDetailPackageLabels.Where(a => a.ExpressDetailPackageLabelId == sph.ExpressDetailPackageLabelId).FirstOrDefault();
                        if (Result != null)
                        {
                            SaveDirectBookingPiecesTrackingNo(sph.TrackingNumber, Result.ExpressDetailPackageLabelId);
                        }
                    }
                }
            }
            else
            {
                var detail = dbContext.ExpressDetailPackageLabels.Where(p => p.TrackingNumber == package.LabelName &&
                                                                         p.ExpressShipmentDetailId == ExpressDetailId).FirstOrDefault();
                if (detail != null)
                {

                    detail.IsDownloaded = true;
                    dbContext.SaveChanges();
                }
                else
                {
                    ExpressDetailPackageLabel sph = new ExpressDetailPackageLabel();
                    sph.ExpressShipmentDetailId = ExpressDetailId;
                    sph.TrackingNumber = package.LabelName;
                    sph.PackageLabelName = ImageName;
                    sph.IsDownloaded = false;
                    dbContext.ExpressDetailPackageLabels.Add(sph);
                    dbContext.SaveChanges();

                    if (!string.IsNullOrEmpty(sph.TrackingNumber) && sph.ExpressDetailPackageLabelId > 0)
                    {
                        var Result = dbContext.ExpressDetailPackageLabels.Where(a => a.ExpressDetailPackageLabelId == sph.ExpressDetailPackageLabelId).FirstOrDefault();
                        if (Result != null)
                        {
                            SaveDirectBookingPiecesTrackingNo(sph.TrackingNumber, Result.ExpressDetailPackageLabelId);
                        }
                    }
                }
            }
        }

        public List<int> GetExpressDetailID(int ExpressId)
        {
            List<int> _id = new List<int>();
            _id = dbContext.ExpressDetails.Where(p => p.ExpressId == ExpressId).Select(p => p.ExpressDetailId).ToList();
            return _id;
        }

        #endregion
    }
}