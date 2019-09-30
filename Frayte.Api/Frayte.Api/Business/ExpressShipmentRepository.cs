using Frayte.Api.Models;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using Frayte.Services.DataAccess;
using System.Net.Mail;
using System.Linq;
using System.Text.RegularExpressions;
using Frayte.Services.Models.Express;
using Frayte.Services.Business;
using Frayte.Services.Utility;
using System.IO;

namespace Frayte.Api.Business
{
    public class ExpressShipmentRepository
    {
        FrayteEntities dbContext = new FrayteEntities();
        public List<FrayteUploadshipment> JsonValidate(ExpressShipmentRequest frayteShipmentRequest, string CallFrom)
        {
            FrayteUploadshipment upload = new FrayteUploadshipment();
            List<FrayteUploadshipment> _upload = new List<FrayteUploadshipment>();
            upload.ShipFrom = new DirectBookingCollection()
            {
                CompanyName = frayteShipmentRequest.ShipFrom.CompanyName,
                FirstName = frayteShipmentRequest.ShipFrom.FirstName,
                LastName = frayteShipmentRequest.ShipFrom.LastName,
                Email = frayteShipmentRequest.ShipFrom.Email,
                Phone = frayteShipmentRequest.ShipFrom.Phone,
                Address = frayteShipmentRequest.ShipFrom.Address.Address1,
                Address2 = frayteShipmentRequest.ShipFrom.Address.Address2,
                City = frayteShipmentRequest.ShipFrom.Address.City,
                State = frayteShipmentRequest.ShipFrom.Address.State,
                Area = frayteShipmentRequest.ShipFrom.Address.Area,
                PostCode = frayteShipmentRequest.ShipFrom.Address.Postcode,
                Country = new FrayteCountryCode()
                {
                    Code = frayteShipmentRequest.ShipFrom.Address.CountryCode
                }
            };
            upload.ShipTo = new DirectBookingCollection()
            {
                CompanyName = frayteShipmentRequest.ShipTo.CompanyName,
                FirstName = frayteShipmentRequest.ShipTo.FirstName,
                LastName = frayteShipmentRequest.ShipTo.LastName,
                Email = frayteShipmentRequest.ShipTo.Email,
                Phone = frayteShipmentRequest.ShipTo.Phone,
                Address = frayteShipmentRequest.ShipTo.Address.Address1,
                Address2 = frayteShipmentRequest.ShipTo.Address.Address2,
                City = frayteShipmentRequest.ShipTo.Address.City,
                State = frayteShipmentRequest.ShipTo.Address.State,
                Area = frayteShipmentRequest.ShipTo.Address.Area,
                PostCode = frayteShipmentRequest.ShipTo.Address.Postcode,
                Country = new FrayteCountryCode()
                {
                    Code = frayteShipmentRequest.ShipTo.Address.CountryCode
                }
            };
            upload.Package = new List<UploadShipmentPackage>();
            foreach (var pp in frayteShipmentRequest.Package)
            {
                UploadShipmentPackage pack = new UploadShipmentPackage()
                {
                    Length = Frayte.Services.CommonConversion.ConvertToDecimal(pp.Length),
                    Width = Frayte.Services.CommonConversion.ConvertToDecimal(pp.Width),
                    Height = Frayte.Services.CommonConversion.ConvertToDecimal(pp.Height),
                    Weight = pp.Weight,
                    CartoonValue = pp.CartoonValue,
                    Value = pp.DeclaredValue,
                    Content = pp.ShipmentContents
                };
                upload.Package.Add(pack);
            };
            upload.CustomInfo = new CustomInformation()
            {
                ContentsType = frayteShipmentRequest.CustomInformation.ContentsType,
                ContentsExplanation = frayteShipmentRequest.CustomInformation.ContentsExplanation,
                RestrictionType = frayteShipmentRequest.CustomInformation.RestrictionType,
                RestrictionComments = frayteShipmentRequest.CustomInformation.RestrictionComments,
                CustomsSigner = frayteShipmentRequest.CustomInformation.CustomsSigner,
                NonDeliveryOption = frayteShipmentRequest.CustomInformation.NonDeliveryOption,
            };
            upload.ServiceCode = frayteShipmentRequest.ServiceCode;
            frayteShipmentRequest.Service = GetExpressService(frayteShipmentRequest.ServiceCode);
            if(frayteShipmentRequest.ShipFrom.Address.CountryCode == "GBR" && frayteShipmentRequest.ShipTo.Address.CountryCode == "GBR")
            {
                frayteShipmentRequest.Service.LogisticServiceType = "UK Domestic";
            }
            else if (frayteShipmentRequest.ShipFrom.Address.CountryCode != "GBR" && frayteShipmentRequest.ShipTo.Address.CountryCode == "GBR")
            {
                frayteShipmentRequest.Service.LogisticServiceType = "Import";
            }
            else if (frayteShipmentRequest.ShipFrom.Address.CountryCode == "GBR" && frayteShipmentRequest.ShipTo.Address.CountryCode != "GBR")
            {
                frayteShipmentRequest.Service.LogisticServiceType = "Export";
            }
            upload.PackageCalculationType = frayteShipmentRequest.PackageCalculationType;
            upload.CurrencyCode = frayteShipmentRequest.DeclaredCurrencyCode;
            upload.PayTaxAndDuties = frayteShipmentRequest.PaymentPartyTaxAndDuties;
            upload.ShipmentReference = frayteShipmentRequest.ShipmentReference;
            upload.ShipmentDescription = frayteShipmentRequest.Package[0].ShipmentContents;
            //upload.CollectionDate = frayteShipmentRequest.RequestedPickupDate.HasValue ? frayteShipmentRequest.RequestedPickupDate.Value.ToString("dd/MM/yyyy") : "";
            //upload.CollectionTime = frayteShipmentRequest.RequestedPickupDate.HasValue ? frayteShipmentRequest.RequestedPickupDate.Value.ToString("HHmm") : "";
            _upload.Add(upload);
            ErrorLog(_upload, CallFrom);

            List<FrayteUploadshipment> Unsucessfuljson = new List<FrayteUploadshipment>();
            foreach (var res in _upload)
            {
                if (res.Errors.Count > 0)
                {
                    Unsucessfuljson.Add(res);
                }
            }
            return Unsucessfuljson;
        }
        public void ErrorLog(List<FrayteUploadshipment> Shipment, string ServiceType)
        {
            foreach (var UploadShipment in Shipment)
            {
                UploadShipment.Errors = new List<string>();
                if (UploadShipment.ShipFrom.Country != null && !string.IsNullOrEmpty(UploadShipment.ShipFrom.Country.Code))
                {
                    var result = dbContext.Countries.Where(a => a.CountryCode == UploadShipment.ShipFrom.Country.Code || a.CountryCode2 == UploadShipment.ShipFrom.Country.Code || a.CountryName == UploadShipment.ShipFrom.Country.Code).FirstOrDefault();
                    if (result == null)
                    {
                        UploadShipment.Errors.Add("From Country Code is wrong please fill correct and upload shipment again");
                    }
                    else
                    {
                        UploadShipment.ShipFrom.Country.Code = result.CountryCode;
                        UploadShipment.ShipFrom.Country.Code2 = result.CountryCode2;
                        UploadShipment.ShipFrom.Country.CountryId = result.CountryId;
                        UploadShipment.ShipFrom.Country.Name = result.CountryName;
                    }
                }
                if (UploadShipment.ShipTo.Country != null && !string.IsNullOrEmpty(UploadShipment.ShipTo.Country.Code))
                {
                    var result = dbContext.Countries.Where(a => a.CountryCode == UploadShipment.ShipTo.Country.Code || a.CountryCode2 == UploadShipment.ShipTo.Country.Code || a.CountryName == UploadShipment.ShipTo.Country.Code).FirstOrDefault();
                    if (result == null)
                    {
                        UploadShipment.Errors.Add("To Country Code is wrong please fill correct and upload shipment again");
                    }
                    else
                    {
                        UploadShipment.ShipTo.Country.Code = result.CountryCode;
                        UploadShipment.ShipTo.Country.Code2 = result.CountryCode2;
                        UploadShipment.ShipTo.Country.CountryId = result.CountryId;
                        UploadShipment.ShipTo.Country.Name = result.CountryName;
                    }
                }

                if (!string.IsNullOrEmpty(UploadShipment.CurrencyCode))
                {
                    var result = dbContext.CurrencyTypes.Where(a => a.CurrencyCode == UploadShipment.CurrencyCode).FirstOrDefault();
                    if (result == null)
                    {
                        UploadShipment.Errors.Add("Currency Code is wrong please fill correct and upload shipment again");
                    }
                }

                if (!string.IsNullOrEmpty(UploadShipment.TrackingNo))
                {
                    var result = dbContext.eCommerceShipments.Where(a => a.TrackingDetail == UploadShipment.TrackingNo).FirstOrDefault();
                    if (result != null)
                    {
                        UploadShipment.Errors.Add("Entered TrackingNo already has been used by other shipment");
                    }
                }
                //if (string.IsNullOrEmpty(UploadShipment.ShipFrom.Email))
                //{ UploadShipment.Errors.Add("From Email is empty please fill and upload shipment again"); }

                if (!string.IsNullOrEmpty(UploadShipment.ShipFrom.Email) && !IsValid(UploadShipment.ShipFrom.Email))
                { UploadShipment.Errors.Add("From Email is not in correct format please fill in correct format and upload shipment again"); }

                //if (string.IsNullOrEmpty(UploadShipment.ShipTo.Email))
                //{ UploadShipment.Errors.Add("To Email is empty please fill and upload shipment again"); }

                if (!string.IsNullOrEmpty(UploadShipment.ShipTo.Email) && !IsValid(UploadShipment.ShipTo.Email))
                { UploadShipment.Errors.Add("To Email is not in correct format please fill in correct format and upload shipment again"); }

                if (UploadShipment.ShipFrom.Country != null && (UploadShipment.ShipFrom.Country.Code == "" || UploadShipment.ShipFrom.Country.Code == null))
                { UploadShipment.Errors.Add("From Country Code is empty please fill and upload shipment again"); }

                if ((UploadShipment.ShipFrom.PostCode == "" || UploadShipment.ShipFrom.PostCode == null || (UploadShipment.ShipFrom.PostCode != null && !new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9_ ]*$").IsMatch(UploadShipment.ShipFrom.PostCode)) && UploadShipment.ShipFrom.Country.Code != "HKG"))
                { UploadShipment.Errors.Add("From Post Code is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipFrom.CompanyName == UploadShipment.ShipFrom.FirstName + " " + UploadShipment.ShipFrom.LastName)
                { UploadShipment.Errors.Add("From CompanyName, From Contact First Name and Last Name never same"); }

                if (UploadShipment.ShipFrom.FirstName == "" || UploadShipment.ShipFrom.FirstName == null)
                { UploadShipment.Errors.Add("From Contact First Name is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipFrom.LastName == "" || UploadShipment.ShipFrom.LastName == null)
                { UploadShipment.Errors.Add("From Contact Last Name is empty please fill and upload shipment again"); }

                //if (UploadShipment.ShipFrom.CompanyName == "" || UploadShipment.ShipFrom.CompanyName == null)
                //{ UploadShipment.Errors.Add("From Company Name is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipFrom.Address == "" || UploadShipment.ShipFrom.Address == null)
                { UploadShipment.Errors.Add("From Address1 is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipFrom.City == "" || UploadShipment.ShipFrom.City == null)
                { UploadShipment.Errors.Add("From City is empty please fill and upload shipment again"); }

                if (string.IsNullOrWhiteSpace(UploadShipment.ShipFrom.State))
                {
                    if (UploadShipment.ShipFrom.Country.Code2 == "HK")
                    {

                    }
                    else
                    {
                        UploadShipment.Errors.Add("From State is empty please fill and upload shipment again");
                    }
                }
                if (UploadShipment.ShipFrom.Phone == "" || UploadShipment.ShipFrom.Phone == null || !new System.Text.RegularExpressions.Regex("^[0-9]+$").IsMatch(UploadShipment.ShipFrom.Phone))
                { UploadShipment.Errors.Add("From TelephoneNo is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipTo.Country != null && (UploadShipment.ShipTo.Country.Code == "" || UploadShipment.ShipTo.Country.Code == null))
                { UploadShipment.Errors.Add("To Country Code is empty please fill and upload shipment again"); }

                if ((UploadShipment.ShipTo.PostCode == "" || UploadShipment.ShipTo.PostCode == null || (UploadShipment.ShipTo.PostCode != null && !new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9_ ]*$").IsMatch(UploadShipment.ShipTo.PostCode)) && UploadShipment.ShipTo.Country.Code != "HKG"))
                { UploadShipment.Errors.Add("To PostCode is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipTo.CompanyName == UploadShipment.ShipTo.FirstName + " " + UploadShipment.ShipTo.LastName)
                { UploadShipment.Errors.Add("To CompanyName, To Contact First Name and Last Name never same"); }

                if (UploadShipment.ShipTo.FirstName == "" || UploadShipment.ShipTo.FirstName == null)
                { UploadShipment.Errors.Add("To Contact First Name is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipTo.LastName == "" || UploadShipment.ShipTo.LastName == null)
                { UploadShipment.Errors.Add("To Contact Last Name is empty please fill and upload shipment again"); }

                //if (UploadShipment.ShipTo.CompanyName == "" || UploadShipment.ShipTo.CompanyName == null)
                //{ UploadShipment.Errors.Add("To CompanyName is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipTo.Address == "" || UploadShipment.ShipTo.Address == null)
                { UploadShipment.Errors.Add("To Address1 is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipTo.City == "" || UploadShipment.ShipTo.City == null)
                { UploadShipment.Errors.Add("To City is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipTo.Phone == "" || UploadShipment.ShipTo.Phone == null || (UploadShipment.ShipTo.Phone != null && !new System.Text.RegularExpressions.Regex("^[0-9]+$").IsMatch(UploadShipment.ShipTo.Phone)))
                { UploadShipment.Errors.Add("To TelephoneNo is empty please fill and upload shipment again"); }

                if (UploadShipment.PackageCalculationType == "" || UploadShipment.PackageCalculationType == null || UploadShipment.PackageCalculationType.ToUpper() != "KGTOCMS" && UploadShipment.PackageCalculationType.ToUpper() != "LBTOINCHS")
                { UploadShipment.Errors.Add("Package Calculation Type is empty or you entered incorrectly please fill and upload shipment again"); }
                else
                {
                    if (UploadShipment.PackageCalculationType.ToUpper() == "KGTOCMS")
                    {
                        UploadShipment.PackageCalculationType = FraytePakageCalculationType.kgtoCms;
                    }
                    else if (UploadShipment.PackageCalculationType.ToUpper() == "LBTOINCHS")
                    {
                        UploadShipment.PackageCalculationType = FraytePakageCalculationType.LbToInchs;
                    }
                }

                if (UploadShipment.PayTaxAndDuties == "" || UploadShipment.PayTaxAndDuties == null || UploadShipment.PayTaxAndDuties.ToUpper() != "RECEIVER" && UploadShipment.PayTaxAndDuties.ToUpper() != "SHIPPER" && UploadShipment.PayTaxAndDuties.ToUpper() != "THIRDPARTY")
                { UploadShipment.Errors.Add("Pay tax and duties is empty or you entered incorrectly please fill and upload shipment again"); }
                else
                {
                    if (UploadShipment.PayTaxAndDuties.ToUpper() == "RECEIVER")
                    {
                        UploadShipment.PayTaxAndDuties = "Receiver";
                    }
                    else if (UploadShipment.PayTaxAndDuties.ToUpper() == "SHIPPER")
                    {
                        UploadShipment.PayTaxAndDuties = "Shipper";
                    }
                    else if (UploadShipment.PayTaxAndDuties.ToUpper() == "THIRDPARTY")
                    {
                        UploadShipment.PayTaxAndDuties = "ThirdParty";
                    }
                }

                //if (UploadShipment.parcelType == "" || UploadShipment.parcelType == null || UploadShipment.parcelType.ToUpper() != "PARCEL" && UploadShipment.PackageCalculationType.ToUpper() != "LETTER")
                //{ UploadShipment.Errors.Add("Parcel Type is empty please fill and upload shipment again"); }

                if (UploadShipment.CurrencyCode == "" || UploadShipment.CurrencyCode == null)
                { UploadShipment.Errors.Add("Currency is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipmentReference == "" || UploadShipment.ShipmentReference == null)
                { UploadShipment.Errors.Add("Shipment Reference is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipmentDescription == "" || UploadShipment.ShipmentDescription == null)
                { UploadShipment.Errors.Add("Shipment Description is empty please fill and upload shipment again"); }

                if (string.IsNullOrEmpty(UploadShipment.ServiceCode) || IsServiceCodeValid(UploadShipment.ServiceCode) == false)
                { UploadShipment.Errors.Add("Service Code is empty or wrong please fill and upload shipment again"); }

                //if (ServiceType == FrayteCallingType.FrayteApi)
                //{
                //    if (string.IsNullOrEmpty(UploadShipment.CollectionDate))
                //    {
                //        UploadShipment.Errors.Add("Collection Date is empty please fill and upload shipment again");
                //    }
                //    else
                //    {
                //        DateTime CollectionDate = DateTime.ParseExact(UploadShipment.CollectionDate, "dd/MM/yyyy", null);
                //        DateTime currentDate = DateTime.ParseExact(DateTime.Now.Date.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);
                //        if (CollectionDate.Date < currentDate.Date)
                //        {
                //            UploadShipment.Errors.Add("Collection Date never small from current date");
                //        }

                //        if (CollectionDate.Date > currentDate.Date.AddDays(5))
                //        { UploadShipment.Errors.Add("Collection Date never greater from current date"); }
                //    }
                //}
                //else
                //{
                //    if ((UploadShipment.CollectionDate != null && UploadShipment.CollectionDate != "" && Convert.ToDateTime(UploadShipment.CollectionDate) == DateTime.MinValue.AddYears(1800)) || (UploadShipment.CollectionDate != null && UploadShipment.CollectionDate != "") || UploadShipment.CollectionDate == null || UploadShipment.CollectionDate == "")
                //    { UploadShipment.Errors.Add("Collection Date is empty please fill and upload shipment again"); }
                //}

                //if (UploadShipment.CollectionTime == "00:00:00.0000000" || UploadShipment.CollectionTime == "" || UploadShipment.CollectionTime == null || (UploadShipment.CollectionTime != null && UploadShipment.CollectionDate != "" && UploadShipment.CollectionDate != null && UploadShipment.CollectionTime != "" && UploadShipment.CollectionTime != null && DateTime.ParseExact(UploadShipment.CollectionDate, "dd/MM/yyyy", null) <= DateTime.Now.Date && TimeSpan.Parse(UploadShipment.CollectionTime) < DateTime.Now.TimeOfDay))
                //{ UploadShipment.Errors.Add("Collection Time is empty please fill and upload shipment again"); }

                //if (UploadShipment.ShipFrom.Country != null && UploadShipment.ShipTo.Country != null && UploadShipment.ShipFrom.Country.Code != UploadShipment.ShipTo.Country.Code)
                //{
                if (UploadShipment.CustomInfo.ContentsType == "" || UploadShipment.CustomInfo.ContentsType == null)
                { UploadShipment.Errors.Add("Contents Type is empty please fill and upload shipment again"); }

                if ((UploadShipment.CustomInfo.ContentsExplanation == "" || UploadShipment.CustomInfo.ContentsExplanation == null) && !string.IsNullOrEmpty(UploadShipment.CustomInfo.ContentsType) && UploadShipment.CustomInfo.ContentsType.ToUpper().Contains("OTHER"))
                { UploadShipment.Errors.Add("Contents Explanation is empty please fill and upload shipment again"); }

                if (UploadShipment.CustomInfo.RestrictionType == "" || UploadShipment.CustomInfo.RestrictionType == null)
                { UploadShipment.Errors.Add("Restriction Type is empty please fill and upload shipment again"); }

                if ((UploadShipment.CustomInfo.RestrictionComments == "" || UploadShipment.CustomInfo.RestrictionComments == null) && !string.IsNullOrEmpty(UploadShipment.CustomInfo.RestrictionType) && UploadShipment.CustomInfo.RestrictionType.ToUpper().Contains("OTHER"))
                { UploadShipment.Errors.Add("Restriction Comments is empty please fill and upload shipment again"); }

                if (UploadShipment.CustomInfo.NonDeliveryOption == "" || UploadShipment.CustomInfo.NonDeliveryOption == null)
                { UploadShipment.Errors.Add("Non Delivery Option is empty please fill and upload shipment again"); }

                if (UploadShipment.CustomInfo.CustomsSigner == "" || UploadShipment.CustomInfo.CustomsSigner == null)
                { UploadShipment.Errors.Add("Customs Signer is empty please fill and upload shipment again"); }
                //}

                int i = 0;
                foreach (var Package in UploadShipment.Package)
                {
                    i++;
                    if (Package.CartoonValue == 0)
                    { UploadShipment.Errors.Add("CartonValue is empty in Line no" + i + "please fill and upload shipment again"); }


                    if (Package.Length == 0)
                    { UploadShipment.Errors.Add("Length is empty in Line no" + i + " please fill and upload shipment again"); }


                    if (Package.Width == 0)
                    { UploadShipment.Errors.Add("Width is empty in Line no" + i + " please fill and upload shipment again"); }


                    if (Package.Height == 0)
                    { UploadShipment.Errors.Add("Height is empty in Line no" + i + " please fill and upload shipment again"); }


                    if (Package.Weight == 0 || Package.Weight * Package.CartoonValue >= 70 * Package.CartoonValue)
                    { UploadShipment.Errors.Add("Weight is empty in Line no" + i + " please fill and upload shipment again"); }


                    if (Package.Value == 0 && UploadShipment.ShipFrom.Country != null && UploadShipment.ShipTo.Country != null && UploadShipment.ShipFrom.Country.Code != UploadShipment.ShipTo.Country.Code)
                    { UploadShipment.Errors.Add("DeclaredValue is empty in Line no" + i + " please fill and upload shipment again"); }


                    if (Package.Content == "" || Package.Content == null)
                    { UploadShipment.Errors.Add("ShipmentContents is empty in Line no" + i + " please fill and upload shipment again"); }
                    i++;
                }
            }
        }

      
        
        public ExpressShipmentModel MappingFrayteRequestToExpressBookingDetail(ExpressShipmentRequest frayteShipmentRequest)
        {

            ExpressShipmentModel expressBookingDetail = new ExpressShipmentModel();
            try
            {
                var customerfrom = GetCustomerDetail(frayteShipmentRequest.Security.AccountNumber);
                var country = GetCountry(frayteShipmentRequest.ShipFrom.Address.CountryCode);

                CheckServiceCode(frayteShipmentRequest);

                

                //Entry in DB for AwbNumber(Shipment PickedUp)
                expressBookingDetail.AWBNumber = GetAWBNumber(expressBookingDetail, frayteShipmentRequest);
                expressBookingDetail.ActualWeight = frayteShipmentRequest.Package.Sum(a => a.Weight);


                expressBookingDetail.ShipFrom = new ExpressAddressModel()
                {
                    CompanyName = frayteShipmentRequest.ShipFrom.CompanyName,
                    Address = frayteShipmentRequest.ShipFrom.Address.Address1,
                    Address2 = frayteShipmentRequest.ShipFrom.Address.Address2,
                    City = frayteShipmentRequest.ShipFrom.Address.City,
                    Email = frayteShipmentRequest.ShipFrom.Email,
                    PostCode = frayteShipmentRequest.ShipFrom.Address.Postcode,
                    State = frayteShipmentRequest.ShipFrom.Address.State,
                    Area = frayteShipmentRequest.ShipFrom.Address.Area,
                    Phone = frayteShipmentRequest.ShipFrom.Phone,
                    Country = new FrayteCountryCode()
                    {
                        Code = country.CountryCode,
                        Code2 = country.CountryCode2,
                        CountryId = country.CountryId,
                        Name = country.CountryName,
                        TimeZoneDetail = null
                    },
                    CustomerId = customerfrom.CustomerId,
                    FirstName = frayteShipmentRequest.ShipFrom.FirstName,
                    LastName = frayteShipmentRequest.ShipFrom.LastName,
                    IsMailSend = false,
                };
                var country1 = GetCountry(frayteShipmentRequest.ShipTo.Address.CountryCode);
                expressBookingDetail.ShipTo = new ExpressAddressModel()
                {
                    CompanyName = frayteShipmentRequest.ShipTo.CompanyName,
                    Address = frayteShipmentRequest.ShipTo.Address.Address1,
                    Address2 = frayteShipmentRequest.ShipTo.Address.Address2,
                    City = frayteShipmentRequest.ShipTo.Address.City,
                    Email = frayteShipmentRequest.ShipTo.Email,
                    PostCode = frayteShipmentRequest.ShipTo.Address.Postcode,
                    State = frayteShipmentRequest.ShipTo.Address.State,
                    Area = frayteShipmentRequest.ShipTo.Address.Area,
                    Phone = frayteShipmentRequest.ShipTo.Phone,
                    Country = new FrayteCountryCode()
                    {
                        Code = country1.CountryCode,
                        Code2 = country1.CountryCode2,
                        CountryId = country1.CountryId,
                        Name = country1.CountryName,
                        TimeZoneDetail = null
                    },
                    CustomerId = customerfrom.CustomerId,
                    FirstName = frayteShipmentRequest.ShipTo.FirstName,
                    LastName = frayteShipmentRequest.ShipTo.LastName,
                    IsMailSend = false,
                };
                expressBookingDetail.DeclaredCurrency = new CurrencyType()
                {
                    CurrencyCode = customerfrom.CurrencyCode,
                    CurrencyDescription = "",
                };
                expressBookingDetail.Packages = new List<ExpressPackageModel>();
                foreach (var package in frayteShipmentRequest.Package)
                {
                    var item = new ExpressPackageModel()
                    {
                        CartonValue = package.CartoonValue,
                        Height = Frayte.Services.CommonConversion.ConvertToDecimal(package.Height),
                        Length = Frayte.Services.CommonConversion.ConvertToDecimal(package.Length),
                        Width = Frayte.Services.CommonConversion.ConvertToDecimal(package.Width),
                        Content = package.ShipmentContents,
                        Weight = package.Weight,
                        Value = package.DeclaredValue
                    };
                    expressBookingDetail.Packages.Add(item);
                };
                //expressBookingDetail.ParcelType = frayteShipmentRequest.par;
                if (frayteShipmentRequest.CustomInformation != null)
                {
                    expressBookingDetail.CustomInformation = new ExpressCustomInformationModel()
                    {
                        ContentsType = frayteShipmentRequest.CustomInformation.ContentsType,
                        RestrictionType = frayteShipmentRequest.CustomInformation.RestrictionType,
                        ContentsExplanation = frayteShipmentRequest.CustomInformation.ContentsExplanation,
                        RestrictionComments = frayteShipmentRequest.CustomInformation.RestrictionComments,
                        CustomsCertify = true,
                        CustomsSigner = frayteShipmentRequest.CustomInformation.CustomsSigner,
                        NonDeliveryOption = frayteShipmentRequest.CustomInformation.NonDeliveryOption,
                    };
                }
                expressBookingDetail.Service = new HubService();
                expressBookingDetail.Service = frayteShipmentRequest.Service;
                expressBookingDetail.PakageCalculatonType = frayteShipmentRequest.PackageCalculationType;
                expressBookingDetail.CustomerId = customerfrom.CustomerId;
                expressBookingDetail.PayTaxAndDuties = frayteShipmentRequest.PaymentPartyTaxAndDuties;
                expressBookingDetail.CreatedBy = customerfrom.CustomerId;
                expressBookingDetail.DeclaredValue = expressBookingDetail.Packages.Sum(a => a.Value);
                //expressBookingDetail = new ReferenceDetail();
                // expressBookingDetail.ReferenceDetail.CollectionDate = Convert.ToDateTime(frayteShipmentRequest.RequestedPickupDate);
                //expressBookingDetail.ReferenceDetail.CollectionTime = frayteShipmentRequest.RequestedPickupDate.HasValue ? frayteShipmentRequest.RequestedPickupDate.Value.ToString("HHmm") : "";
                expressBookingDetail.ShipmentReference = frayteShipmentRequest.ShipmentReference;
                expressBookingDetail.ShipmentDescription = frayteShipmentRequest.ShipmentReference;
                //expressBookingDetail.Service = GetExpressService(frayteShipmentRequest.ServiceCode);
                //directBookingDetail.CustomerId = user != null && user.RoleId == (int)FrayteUserRole.UserCustomer ? user.CustomerId : customerfrom.CustomerId;
                //directBookingDetail.CreatedBy = user != null && user.RoleId == (int)FrayteUserRole.UserCustomer ? user.CreatedBy : customerfrom.CustomerId;
                //directBookingDetail.ShipmentStatusId = 12;
                //directBookingDetail.BookingStatusType = "Darft";
                //directBookingDetail.PakageCalculatonType = frayteShipmentRequest.PackageCalculationType;
                //directBookingDetail.AddressType = frayteShipmentRequest.AddressType;
                return expressBookingDetail;
            }
            catch (Exception ex)
            {
                throw (new FrayteApiException("MappingRequestError", ex));
            }
        }

        public void CheckServiceCode(ExpressShipmentRequest ESR)
        {

            var Country = dbContext.Countries.Where(a => a.CountryCode == ESR.ShipTo.Address.CountryCode).FirstOrDefault();
            if(Country != null)
            {
                var Result = (from HCS in dbContext.HubCarrierServices
                              join HCSC in dbContext.HubCarrierServiceCountries on HCS.HubCarrierServiceId equals HCSC.HubCarrierServiceId
                              where HCS.ServiceCode == ESR.ServiceCode
                              && HCSC.CountryId == Country.CountryId
                              select HCS).FirstOrDefault();
                if (Result == null)
                {
                    throw new Exception("Wrong service code used for wrong address.");
                }
            }
        }

        public string GetAWBNumber(ExpressShipmentModel expressBookingDetail, ExpressShipmentRequest frayteShipmentRequest)
        {
            frayteShipmentRequest.AWBNumber = frayteShipmentRequest.AWBNumber.Replace(" ", "");
            string awbNo = string.Empty;
            if (!string.IsNullOrEmpty(frayteShipmentRequest.AWBNumber))
            {
                if (frayteShipmentRequest.AWBNumber.Substring(0, 3) == frayteShipmentRequest.Security.AccountNumber.Substring(0, 3))
                {
                    var ISCheckAwb = dbContext.Expresses.Where(a => a.AWBBarcode == frayteShipmentRequest.AWBNumber).FirstOrDefault();
                    if (ISCheckAwb != null)
                    {
                        throw new Exception("AWB already exist in system");
                    }
                    else
                    {

                        awbNo= GetAwb(expressBookingDetail, frayteShipmentRequest, frayteShipmentRequest.AWBNumber, "UI");
                    }
                }
                else
                {
                    throw new Exception("AWB Format is wrong");
                }
            }
            else
            {
                awbNo = GetAwb(expressBookingDetail, frayteShipmentRequest, "", "NoNUI");
            }

      
            return awbNo;
        }

        public string GetAwb(ExpressShipmentModel expressBookingDetail,  ExpressShipmentRequest frayteShipmentRequest, string AwbNo, string AwbFrom)
        {
            string Awbno = string.Empty;
            if(AwbFrom == "NoNUI")
            {
            AwbAgain:
                var awbNo = frayteShipmentRequest.Security.AccountNumber.Substring(0, 3).ToString() + new Random().Next(100000000, 999999999);
                var Result = dbContext.Expresses.Where(a => a.AWBBarcode == awbNo).FirstOrDefault();
                if (Result == null)
                {
                    ScanInitalAwbModel SIAM = new ScanInitalAwbModel();
                    SIAM.AwbNumber = awbNo;
                    SIAM.MobileEventId = 1;
                    SIAM.ScannedBy = GetCustomerDetail(frayteShipmentRequest.Security.AccountNumber).CustomerId;
                    var AwbDetial = new ExpressScannedAWBRepository().CollectionScanMobileAwb(SIAM);
                    expressBookingDetail.ExpressId = AwbDetial.ExpressId;
                    Awbno = AwbDetial.AwbNumber;
                    expressBookingDetail.ShipmentStatusId = dbContext.Expresses.Where(a => a.ExpressId == AwbDetial.ExpressId).FirstOrDefault().ShipmentStatusId;
                }
                else
                {
                    goto AwbAgain;
                }
            }
            else
            {
                var Result = dbContext.Expresses.Where(a => a.AWBBarcode == AwbNo).FirstOrDefault();
                if (Result == null)
                {
                    ScanInitalAwbModel SIAM = new ScanInitalAwbModel();
                    SIAM.AwbNumber = AwbNo;
                    SIAM.MobileEventId = 1;
                    SIAM.ScannedBy = GetCustomerDetail(frayteShipmentRequest.Security.AccountNumber).CustomerId;
                    var AwbDetial = new ExpressScannedAWBRepository().CollectionScanMobileAwb(SIAM);
                    expressBookingDetail.ExpressId = AwbDetial.ExpressId;
                    Awbno = AwbDetial.AwbNumber;
                    expressBookingDetail.ShipmentStatusId = dbContext.Expresses.Where(a => a.ExpressId == AwbDetial.ExpressId).FirstOrDefault().ShipmentStatusId;
                }
            }
            return Awbno;
        }

        //public ExpressTrackingModel GetTracking(string Number)
        //{
        //    ExpressTrackingModel ExpTrcMdl = new ExpressTrackingModel();

        //    if (!string.IsNullOrEmpty(Number))
        //    {
        //        Number = Number.Replace(" ", "");
        //        var Result = dbContext.TrackingNumberRoutes.Where(a => a.Number.Contains(Number) && a.ModuleType == FrayteShipmentServiceType.ExpressBooking && (a.IsAWB == true || a.IsTrackingNumber == true)).FirstOrDefault();
        //        if (Result != null)
        //        {
        //            var Res = new UpdateTradelaneTrackingRepository().GetTracking(Number);
        //            if (Res.Count ==null || Res.Count == 0)
        //            {
        //                ExpTrcMdl.Description = "Wrong AWB or Courier Number";
        //            }
        //            else
        //            {
        //                ExpTrcMdl.AWBTracking = new List<FrayteShipmentTracking>();
        //                ExpTrcMdl.AWBTracking.AddRange(Res.Count);
        //            }
        //        }
        //        else
        //        {
        //            ExpTrcMdl.Description = "Wrong AWB or Courier Number";
        //        }
        //    }
        //    else
        //    {
        //        ExpTrcMdl.Description = "Wrong AWB or Courier Number";
        //    }
        //    return ExpTrcMdl;
        //}

        public bool IsServiceCodeValid(string ServiceCode)
        {
            //Where(a => a.ServiceCode == ServiceCode).FirstOrDefault();
            var Result = (from HCS in dbContext.HubCarrierServices
                          join HCC in dbContext.CustomerHubCarrierServices on HCS.HubCarrierServiceId equals HCC.HubCarrierServiceId
                          where HCS.ServiceCode == ServiceCode
                          select HCS).FirstOrDefault();
            if (Result != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public Country GetCountry(string CountryCode)
        {
            var country = dbContext.Countries.FirstOrDefault(s => s.CountryCode == CountryCode || s.CountryCode2 == CountryCode);
            return country;
        }

        public DirectBookingCollection GetCustomerDetail(string AccountNumber)
        {
            var result = (from U in dbContext.Users
                          join UA in dbContext.UserAddresses on U.UserId equals UA.UserId
                          join Add in dbContext.UserAdditionals on UA.UserId equals Add.UserId
                          where Add.AccountNo == AccountNumber
                          select new DirectBookingCollection()
                          {
                              CompanyName = U.CompanyName,
                              CustomerId = UA.UserId,
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
                              IsShipperTaxAndDuty = Add.IsShipperTaxAndDuty.HasValue == true ? true : false,
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

        public HubService GetExpressService(string ServiceCode)
        {

            var service = (from hc in dbContext.HubCarriers
                           join hcs in dbContext.HubCarrierServices on hc.HubCarrierId equals hcs.HubCarrierId
                           join hcsc in dbContext.HubCarrierServiceCountries on hcs.HubCarrierServiceId equals hcsc.HubCarrierServiceId
                           join ccs in dbContext.CustomerHubCarrierServices on hcs.HubCarrierServiceId equals ccs.HubCarrierServiceId
                           where hcs.IsActive == true
                           && hcs.ServiceCode == ServiceCode
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
                               CarrierLogo = hcs.Logo,
                               WeightRoundLogic = hcs.WeightRoundLogic
                           }
                            ).FirstOrDefault();
           

            if (service == null)
            {
                service = new HubService();
            }
            return service;

        }

        public void MappingFrayteResponseToExpressBookingDetail(ExpressShipmentModel ExpressDetail, IntegrtaionResult intResult, ExpressShipmentResponseModel Response)
        {
            Response.ShipmentBookingId = ExpressDetail.ExpressId;
            Response.Status = true;
            Response.Description = "";
            Response.AWBNumber = ExpressDetail.AWBNumber.Substring(0, 3) + " " + ExpressDetail.AWBNumber.Substring(3, 3) + " " + ExpressDetail.AWBNumber.Substring(6, 3) + " " + ExpressDetail.AWBNumber.Substring(9, 3);
            Response.TrackingNumber = intResult.TrackingNumber.Replace("Order_", "");
            Response.CreatedOn = DateTime.UtcNow.ToString("dd-MMM-yyyy");
            Response.Currency = ExpressDetail.DeclaredCurrency.CurrencyCode;
            Response.FromAddress = new FromAddressDto()
            {
                CompanyName = ExpressDetail.ShipFrom.CompanyName,
                FirstName = ExpressDetail.ShipFrom.FirstName,
                LastName = ExpressDetail.ShipFrom.LastName,
                Email = ExpressDetail.ShipFrom.Email,
                Phone = ExpressDetail.ShipFrom.Phone,
                Address = new ShipAddressDto()
                {
                    Address1 = ExpressDetail.ShipFrom.Address,
                    Address2 = ExpressDetail.ShipFrom.Address2,
                    Area = ExpressDetail.ShipFrom.Area,
                    City = ExpressDetail.ShipFrom.City,
                    CountryCode = ExpressDetail.ShipFrom.Country.Code,
                    Postcode = ExpressDetail.ShipFrom.PostCode,
                    State = ExpressDetail.ShipFrom.State
                }
            };
            Response.ToAddress = new ToAddressDto()
            {
                CompanyName = ExpressDetail.ShipTo.CompanyName,
                FirstName = ExpressDetail.ShipTo.FirstName,
                LastName = ExpressDetail.ShipTo.LastName,
                Email = ExpressDetail.ShipTo.Email,
                Phone = ExpressDetail.ShipTo.Phone,
                Address = new ShipAddressDto()
                {
                    Address1 = ExpressDetail.ShipTo.Address,
                    Address2 = ExpressDetail.ShipTo.Address2,
                    Area = ExpressDetail.ShipTo.Area,
                    City = ExpressDetail.ShipTo.City,
                    CountryCode = ExpressDetail.ShipTo.Country.Code,
                    Postcode = ExpressDetail.ShipTo.PostCode,
                    State = ExpressDetail.ShipTo.State
                }
            };

            Response.CustomInfo = new ApiCustomInformation()
            {
                ContentsType = ExpressDetail.CustomInformation.ContentsType,
                ContentsExplanation = ExpressDetail.CustomInformation.ContentsExplanation,
                RestrictionType = ExpressDetail.CustomInformation.RestrictionType,
                RestrictionComments = ExpressDetail.CustomInformation.RestrictionComments,
                CustomsSigner = ExpressDetail.CustomInformation.CustomsSigner,
                NonDeliveryOption = ExpressDetail.CustomInformation.NonDeliveryOption,
            };
            var FilesArray = Directory.GetFiles(AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressDetail.ExpressId);

            if (FilesArray.Length > 0)
            {
                var AllFile = FilesArray.Where(a => a.Contains("All")).FirstOrDefault();
                if (!string.IsNullOrEmpty(AllFile))
                {
                    Response.AllLlabelUrl = AppSettings.LabelVirtualPath + "/PackageLabel/Express/" + FilesArray.Where(a => a.Contains("All")).FirstOrDefault().Split('/').LastOrDefault();
                }
                else
                {
                    Response.AllLlabelUrl = AppSettings.LabelVirtualPath + "/PackageLabel/Express/" + FilesArray.FirstOrDefault().Split('/').LastOrDefault();
                }
            }
            else
            {
                Response.AllLlabelUrl = "";
            }
        }
    }
}