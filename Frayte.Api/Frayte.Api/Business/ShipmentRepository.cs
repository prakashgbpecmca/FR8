using Frayte.Api.Models;
using Frayte.Services;
using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.ApiModal;
using Frayte.Services.Models.Express;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Frayte.Api.Business
{
    public class APIShipmentRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<FrayteUploadshipment> JsonValidate(FrayteShipmentRequest frayteShipmentRequest, string CallFrom)
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
            upload.parcelType = frayteShipmentRequest.ParcelType;
            upload.PackageCalculationType = frayteShipmentRequest.PackageCalculationType;
            upload.CurrencyCode = frayteShipmentRequest.DeclaredCurrencyCode;
            upload.PayTaxAndDuties = frayteShipmentRequest.PaymentPartyTaxAndDuties;
            upload.ShipmentReference = frayteShipmentRequest.ShipmentReference;
            upload.ShipmentDescription = frayteShipmentRequest.Package[0].ShipmentContents;
            upload.CollectionDate = frayteShipmentRequest.RequestedPickupDate.HasValue ? frayteShipmentRequest.RequestedPickupDate.Value.ToString("dd/MM/yyyy") : "";
            upload.CollectionTime = frayteShipmentRequest.RequestedPickupDate.HasValue ? frayteShipmentRequest.RequestedPickupDate.Value.ToString("HHmm") : "";
            _upload.Add(upload);
            new DirectBookingUploadShipmentRepository().ErrorLog(_upload, CallFrom);

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

        public DirectBookingShipmentDraftDetail MappingFrayteRequestToDirectBookingDetail(FrayteShipmentRequest frayteShipmentRequest)
        {

            #region For Sepecial Customer Mapping
            var user = (from u in dbContext.UserAdditionals
                        join uc in dbContext.UserCustomers on u.UserId equals uc.CustomerId into cusotmerUser
                        from abc in cusotmerUser.DefaultIfEmpty()
                        join ur in dbContext.UserRoles on abc.CustomerId equals ur.UserId
                        where u.AccountNo == frayteShipmentRequest.Security.AccountNumber
                        select new
                        {
                            CustomerId = abc.UserId,
                            CreatedBy = abc.CustomerId,
                            RoleId = ur.RoleId
                        }).FirstOrDefault();
            #endregion

            DirectBookingShipmentDraftDetail directBookingDetail = new DirectBookingShipmentDraftDetail();
            try
            {
                var customerfrom = GetCustomerDetail(frayteShipmentRequest.Security.AccountNumber);
                var country = GetCountry(frayteShipmentRequest.ShipFrom.Address.CountryCode);
                directBookingDetail.ShipFrom = new DirectBookingDraftCollection()
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
                    CurrencyCode = customerfrom.CurrencyCode,

                    Country = new FrayteCountryCode()
                    {
                        Code = country.CountryCode,
                        Code2 = country.CountryCode2,
                        CountryId = country.CountryId,
                        Name = country.CountryName,
                        TimeZoneDetail = null
                    },
                    CustomerId = user != null && user.RoleId == (int)FrayteUserRole.UserCustomer ? user.CreatedBy : customerfrom.CustomerId,
                    AddressBookId = 0,
                    FirstName = frayteShipmentRequest.ShipFrom.FirstName,
                    LastName = frayteShipmentRequest.ShipFrom.LastName,
                    IsFavorites = true,
                    IsMailSend = false,
                };
                var country1 = GetCountry(frayteShipmentRequest.ShipTo.Address.CountryCode);
                directBookingDetail.ShipTo = new DirectBookingDraftCollection()
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
                    CurrencyCode = frayteShipmentRequest.DeclaredCurrencyCode,
                    Country = new FrayteCountryCode()
                    {
                        Code = country1.CountryCode,
                        Code2 = country1.CountryCode2,
                        CountryId = country1.CountryId,
                        Name = country1.CountryName,
                        TimeZoneDetail = null
                    },
                    CustomerId = user != null && user.RoleId == (int)FrayteUserRole.UserCustomer ? user.CreatedBy : customerfrom.CustomerId,
                    FirstName = frayteShipmentRequest.ShipTo.FirstName,
                    LastName = frayteShipmentRequest.ShipTo.LastName,
                    AddressBookId = 0,
                    IsMailSend = false,
                };
                directBookingDetail.Currency = new CurrencyType()
                {
                    CurrencyCode = customerfrom.CurrencyCode,
                    CurrencyDescription = "",
                };
                directBookingDetail.PayTaxAndDuties = frayteShipmentRequest.PaymentPartyTaxAndDuties;
                directBookingDetail.Packages = new List<PackageDraft>();
                foreach (var package in frayteShipmentRequest.Package)
                {
                    var item = new PackageDraft()
                    {
                        CartoonValue = package.CartoonValue,
                        Height = Frayte.Services.CommonConversion.ConvertToDecimal(package.Height),
                        Length = Frayte.Services.CommonConversion.ConvertToDecimal(package.Length),
                        Width = Frayte.Services.CommonConversion.ConvertToDecimal(package.Width),
                        Content = package.ShipmentContents,
                        Weight = package.Weight,
                        Value = package.DeclaredValue
                    };
                    directBookingDetail.Packages.Add(item);
                };
                directBookingDetail.ParcelType = new FrayteParcelType()
                {
                    ParcelType = ShipmentType.LetterType(TotalWeight(frayteShipmentRequest.Package), frayteShipmentRequest.ParcelType),
                    ParcelDescription = ShipmentType.DocDescription(TotalWeight(frayteShipmentRequest.Package), frayteShipmentRequest.ParcelType)
                };
                if (frayteShipmentRequest.CustomInformation != null)
                {
                    directBookingDetail.CustomInfo = new Services.Models.CustomInformation()
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

                directBookingDetail.ReferenceDetail = new ReferenceDetail();
                directBookingDetail.ReferenceDetail.CollectionDate = Convert.ToDateTime(frayteShipmentRequest.RequestedPickupDate);
                directBookingDetail.ReferenceDetail.CollectionTime = frayteShipmentRequest.RequestedPickupDate.HasValue ? frayteShipmentRequest.RequestedPickupDate.Value.ToString("HHmm") : "";
                directBookingDetail.ReferenceDetail.ContentDescription = frayteShipmentRequest.ShipmentReference;
                directBookingDetail.ReferenceDetail.Reference1 = frayteShipmentRequest.ShipmentReference;
                directBookingDetail.CustomerRateCard = new DirectBookingService()
                {
                    ZoneRateCardId = 11936,
                    CustomerId = user != null && user.RoleId == (int)FrayteUserRole.UserCustomer ? user.CreatedBy : customerfrom.CustomerId,
                    WeightType = "Heavy Weight",
                    LogisticServiceId = 44,
                    Rate = 143,
                    BaseRate = 130,
                    MarginPercent = 12,
                    CourierName = "UPS",
                    DisplayName = "UPS",
                    CourierAccountId = 116,
                    CourierAccountNo = "xyz",
                    CourierDescription = "Export Saver Hong Kong",
                    CourierAccountCountryCode = "HKG",
                    IntegrationAccountId = "xyz",
                    LogisticType = "Export",
                    LogisticShipmentId = 98,
                    LogisticDescription = "HeavyWeight",
                    LogisticServiceType = "Export",
                    UnitOfMeasurement = "Per Kg",
                    TotalEstimatedCharge = 7150,
                    CurrencyCode = "HKD",
                    CustomerCurrency = frayteShipmentRequest.DeclaredCurrencyCode,
                    RateType = "Saver",
                    RateTypeDisplay = "Saver",
                    PackageCalculationType = "kgToCms",
                    Weight = Convert.ToDecimal(frayteShipmentRequest.Package.FirstOrDefault().Weight),
                    Length = Convert.ToDecimal(frayteShipmentRequest.Package.FirstOrDefault().Length),
                    Width = Convert.ToDecimal(frayteShipmentRequest.Package.FirstOrDefault().Width),
                    Height = Convert.ToDecimal(frayteShipmentRequest.Package.FirstOrDefault().Height),
                    TransitTime = "3"
                };

                directBookingDetail.CustomerId = user != null && user.RoleId == (int)FrayteUserRole.UserCustomer ? user.CustomerId : customerfrom.CustomerId;
                directBookingDetail.CreatedBy = user != null && user.RoleId == (int)FrayteUserRole.UserCustomer ? user.CreatedBy : customerfrom.CustomerId;
                directBookingDetail.ShipmentStatusId = 12;
                directBookingDetail.BookingStatusType = "Darft";
                directBookingDetail.PakageCalculatonType = frayteShipmentRequest.PackageCalculationType;
                directBookingDetail.AddressType = frayteShipmentRequest.AddressType;
                return directBookingDetail;
            }
            catch (Exception ex)
            {
                throw (new FrayteApiException("MappingRequestError", ex));
            }
        }

        public Country GetCountry(string CountryCode)
        {
            var country = dbContext.Countries.FirstOrDefault(s => s.CountryCode == CountryCode || s.CountryCode2 == CountryCode);
            return country;
        }

        public HubService GetExpressService(string ServiceCode)
        {
            HubService HS = new HubService();
            return HS;
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

        public TrackDto GetTrackingInfo(TrackRequest trackRequest)
        {
            TrackDto track = new TrackDto();

            string TrackingNo = string.Empty;
            if (!string.IsNullOrWhiteSpace(trackRequest.TrackingNumber))
            {
                var Obj = dbContext.DirectShipments.FirstOrDefault(s => s.FrayteNumber == trackRequest.TrackingNumber);
                if (Obj != null)
                {
                    if (Obj.TrackingDetail.Contains("Order"))
                    {
                        track = new TrackDto()
                        {
                            LogisticServiceType = Obj.LogisticServiceType,
                            TrackingNumber = Obj.TrackingDetail.Replace("Order_", "")

                        };
                        //{
                        //    track = (from DS in dbContext.DirectShipments
                        //             join DSD in dbContext.DirectShipmentDetails on DS.DirectShipmentId equals DSD.DirectShipmentId
                        //             join PTD in dbContext.PackageTrackingDetails on DSD.DirectShipmentDetailId equals PTD.DirectShipmentDetailId
                        //             where DS.FrayteNumber == trackRequest.FrayteAWBNumber
                        //             select new TrackDto
                        //             {
                        //                 TrackingNumber = PTD.TrackingNo,
                        //                 LogisticServiceType = DS.LogisticServiceType
                        //             }).FirstOrDefault();

                    }
                    else
                    {
                        track.TrackingNumber = Obj.TrackingDetail;
                        track.LogisticServiceType = Obj.LogisticServiceType;
                    }
                }
                else
                {
                    track = (from DS in dbContext.DirectShipments
                             join DSD in dbContext.DirectShipmentDetails on DS.DirectShipmentId equals DSD.DirectShipmentId
                             join PTD in dbContext.PackageTrackingDetails on DSD.DirectShipmentDetailId equals PTD.DirectShipmentDetailId
                             where PTD.TrackingNo == trackRequest.TrackingNumber || DS.TrackingDetail.Contains(trackRequest.TrackingNumber)
                             select new TrackDto
                             {
                                 TrackingNumber = DS.TrackingDetail,
                                 LogisticServiceType = DS.LogisticServiceType
                             }).FirstOrDefault();
                    if (track != null)
                    {
                        track.TrackingNumber = track.TrackingNumber.Replace("Order_", "");
                        track.LogisticServiceType = track.LogisticServiceType;
                    }
                }
            }

            return track;
        }

        public FrayteTrackingDto MappingFrayteTracking(FrayteShipmentTracking trayteShipmentTracking)
        {
            FrayteTrackingDto frayteTracking = new FrayteTrackingDto();

            frayteTracking.Courier = trayteShipmentTracking.Tracking.FirstOrDefault().Courier;
            frayteTracking.CourierService = trayteShipmentTracking.Tracking.FirstOrDefault().Carrier;
            frayteTracking.SignedBy = trayteShipmentTracking.Tracking.FirstOrDefault().SignedBy;
            frayteTracking.CreatedOn = DateandTime(trayteShipmentTracking.Tracking.FirstOrDefault().CreatedAtDate, trayteShipmentTracking.Tracking.FirstOrDefault().CreatedAtTime);
            frayteTracking.EstimatedWeight = trayteShipmentTracking.Tracking.FirstOrDefault().EstimatedWeight;
            frayteTracking.EstimatedDeliveryOn = DateandTime(trayteShipmentTracking.Tracking.FirstOrDefault().EstimatedDeliveryDate, trayteShipmentTracking.Tracking.FirstOrDefault().EstimatedDeliveryTime);
            frayteTracking.TrackingNumber = trayteShipmentTracking.Tracking.FirstOrDefault().TrackingNumber;
            frayteTracking.NoOfPieces = trayteShipmentTracking.Tracking.FirstOrDefault().NoOfPieces;
            frayteTracking.Status = trayteShipmentTracking.Tracking.FirstOrDefault().Status;
            frayteTracking.UpdatedOn = DateandTime(trayteShipmentTracking.Tracking.FirstOrDefault().UpdatedAtDate, trayteShipmentTracking.Tracking.FirstOrDefault().UpdatedAtTime);
            frayteTracking.ActivityDedtail = new List<ActivityDedtail>();
            foreach (var item in trayteShipmentTracking.Tracking.FirstOrDefault().TrackingDetails)
            {
                frayteTracking.ActivityDedtail.Add(new ActivityDedtail()
                {
                    Activity = item.Activity,
                    ActivityOn = DateandTime(item.Date.ToString(), item.Time),
                    EventType = item.EventType,
                    Location = item.Location,
                    Pieces = item.Pieces,
                });
            }
            return frayteTracking;
        }

        public string DateandTime(string Date, string Time)
        {
            string date = null;
            string time = null;

            if (!string.IsNullOrWhiteSpace(Date))
            {
                date = Date;
            }
            if (!string.IsNullOrWhiteSpace(Time))
            {
                if (Time.Contains(":"))
                {
                    Time = Time.Replace(":", "");
                    time = CommonConversion.ConvertStringToTime(Time).ToString();
                }
                else
                {
                    time = CommonConversion.ConvertStringToTime(Time).ToString();
                }
            }
            var output = date + " " + time;
            return output;
        }

        public List<LabelInfo> GetLabelInfo(LabelRequestDto lableRequest)
        {
            List<LabelInfo> labelInfo = new List<LabelInfo>();
            if (!string.IsNullOrWhiteSpace(lableRequest.TrackingNumber))
            {
                labelInfo = (from DS in dbContext.DirectShipments
                             join DSD in dbContext.DirectShipmentDetails on DS.DirectShipmentId equals DSD.DirectShipmentId
                             join PTD in dbContext.PackageTrackingDetails on DSD.DirectShipmentDetailId equals PTD.DirectShipmentDetailId
                             where PTD.TrackingNo == lableRequest.TrackingNumber || DS.TrackingDetail.Contains(lableRequest.TrackingNumber)
                             select new LabelInfo
                             {
                                 Image = PTD.PackageImage,
                                 TrackingNo = PTD.TrackingNo,
                                 DirectShipmentId = DS.DirectShipmentId,
                                 UniqueTrackingNumber = DS.TrackingDetail.Contains("Order") ? PTD.TrackingNo : DS.TrackingDetail
                             }).ToList();

                if (labelInfo == null)
                {
                    var directshipment = dbContext.DirectShipments.FirstOrDefault(s => s.TrackingDetail == lableRequest.TrackingNumber);
                    if (directshipment != null)
                    {
                        labelInfo = (from DS in dbContext.DirectShipments
                                     join DSD in dbContext.DirectShipmentDetails on DS.DirectShipmentId equals DSD.DirectShipmentId
                                     join PTD in dbContext.PackageTrackingDetails on DSD.DirectShipmentDetailId equals PTD.DirectShipmentDetailId
                                     where DS.DirectShipmentId == directshipment.DirectShipmentId
                                     select new LabelInfo
                                     {
                                         Image = PTD.PackageImage,
                                         TrackingNo = PTD.TrackingNo,
                                         DirectShipmentId = DS.DirectShipmentId,
                                         UniqueTrackingNumber = DS.TrackingDetail.Contains("order") ? PTD.TrackingNo : DS.TrackingDetail
                                     }).ToList();

                        if (labelInfo == null)
                        {

                            labelInfo = (from DS in dbContext.DirectShipments
                                         join DSD in dbContext.DirectShipmentDetails on DS.DirectShipmentId equals DSD.DirectShipmentId
                                         join PTD in dbContext.PackageTrackingDetails on DSD.DirectShipmentDetailId equals PTD.DirectShipmentDetailId
                                         where PTD.TrackingNo == lableRequest.TrackingNumber
                                         select new LabelInfo
                                         {
                                             Image = PTD.PackageImage,
                                             TrackingNo = PTD.TrackingNo,
                                             DirectShipmentId = DS.DirectShipmentId,
                                             UniqueTrackingNumber = DS.TrackingDetail.Contains("order") ? PTD.TrackingNo : DS.TrackingDetail
                                         }).ToList();
                        }
                    }
                }
            }
            return labelInfo;
        }

        public UserAdditional UserInfo(string apiKey)
        {
            var decriptKey = CryptoEngine.Decrypt(apiKey, Models.EncriptionKey.PrivateKey);
            IList<string> key = decriptKey.Split(',');
            var Account = key[0].ToString();
            // var accountEmail = key[1].ToString();

            using (FrayteEntities DbContext = new FrayteEntities())
            {
                var userInfo = DbContext.UserAdditionals.FirstOrDefault(s => s.AccountNo == Account /*&& s.AccountMail == accountEmail*/);
                return userInfo;
            }
        }

        public CountryDto CountryInfo(FrayteShipmentRequest frayteShipmentRequest)
        {
            using (FrayteEntities DbContext = new FrayteEntities())
            {
                var FromcountryInfo = DbContext.Countries.FirstOrDefault(c => c.CountryCode == frayteShipmentRequest.ShipFrom.Address.CountryCode);
                var TofromcountryInfo = DbContext.Countries.FirstOrDefault(c => c.CountryCode == frayteShipmentRequest.ShipTo.Address.CountryCode);

                CountryDto countryDto = new CountryDto();

                countryDto.FromAddressContryId = FromcountryInfo.CountryId;
                countryDto.FromAddressContryName = FromcountryInfo.CountryName;
                countryDto.FromAddressContryCode = FromcountryInfo.CountryCode;
                countryDto.ToAddressContryId = TofromcountryInfo.CountryId;
                countryDto.ToAddressContryName = TofromcountryInfo.CountryName;
                countryDto.ToAddressContryCode = TofromcountryInfo.CountryCode;
                return countryDto;
            }
        }

        public DirectBookingFindService MappingDirectBookingFindService(FrayteShipmentRequest FrayteShipmentRequest, CountryDto CountryDto, UserAdditional UserAdditional)
        {
            DirectBookingFindService directBookingFindService = new DirectBookingFindService();
            var totalWeight = TotalWeight(FrayteShipmentRequest.Package);
            decimal PieceTotalWeight = 0;
            foreach (var item in FrayteShipmentRequest.Package)
            {
                var weight = (item.Weight * item.CartoonValue);
                PieceTotalWeight += weight;
            }

            directBookingFindService.Weight = totalWeight >= PieceTotalWeight ? totalWeight : PieceTotalWeight;
            directBookingFindService.CustomerId = UserAdditional.UserId;
            directBookingFindService.ToCountry = new FrayteCountryCode()
            {
                CountryId = CountryDto.ToAddressContryId,
                Name = CountryDto.ToAddressContryName,
                Code = CountryDto.ToAddressContryCode
            };
            directBookingFindService.ToPostCode = FrayteShipmentRequest.ShipTo.Address.Postcode;
            directBookingFindService.PackageType = ShipmentType.PackageType(FrayteShipmentRequest.Package.Count());
            directBookingFindService.AddressType = FrayteShipmentRequest.AddressType != null ? FrayteShipmentRequest.AddressType : ApiAddressType.B2B;
            directBookingFindService.DocType = ShipmentType.DocType(TotalWeight(FrayteShipmentRequest.Package), FrayteShipmentRequest.ParcelType);
            directBookingFindService.FromPostCode = FrayteShipmentRequest.ShipFrom.Address.Postcode;
            directBookingFindService.FromCountry = new FrayteCountryCode()
            {
                CountryId = CountryDto.FromAddressContryId,
                Name = CountryDto.FromAddressContryName,
                Code = CountryDto.FromAddressContryCode
            };
            string datevalue = DateTime.UtcNow.Date.Month.ToString() + "/" + DateTime.UtcNow.Date.Day.ToString() + "/" + DateTime.UtcNow.Date.Year.ToString();
            DateTime date = Frayte.Services.CommonConversion.ConvertToDateTime(datevalue);
            directBookingFindService.Date = date;
            directBookingFindService.CallingFrom = FrayteShipmentServiceType.DirectBooking;
            directBookingFindService.Packages = new List<PackageDraft>();
            foreach (var package in FrayteShipmentRequest.Package)
            {
                var item = new PackageDraft()
                {
                    CartoonValue = package.CartoonValue,
                    Height = Frayte.Services.CommonConversion.ConvertToDecimal(package.Height),
                    Length = Frayte.Services.CommonConversion.ConvertToDecimal(package.Length),
                    Width = Frayte.Services.CommonConversion.ConvertToDecimal(package.Width),
                    Content = package.ShipmentContents,
                    Weight = package.Weight,
                    Value = package.DeclaredValue
                };
                directBookingFindService.Packages.Add(item);
            };
            directBookingFindService.PackageCalculationType = FrayteShipmentRequest.PackageCalculationType;

            return directBookingFindService;
        }

        public decimal TotalWeight(List<Models.Package> package)
        {
            decimal weight = new decimal();
            foreach (var pp in package)
            {
                var product = pp;
                double len = 0;
                double wid = 0;
                double height = 0;
                double qty = pp.CartoonValue;
                if (string.IsNullOrWhiteSpace(pp.Length))
                {
                    len = 0;
                }
                else
                {
                    len = Convert.ToDouble(pp.Length);
                }

                if (string.IsNullOrWhiteSpace(pp.Width))
                {
                    wid = 0;
                }
                else
                {
                    wid = Convert.ToDouble(pp.Width);
                }

                if (string.IsNullOrWhiteSpace(pp.Height))
                {
                    height = 0;
                }
                else
                {
                    height = Convert.ToDouble(pp.Height);
                }

                if (len > 0 && wid > 0 && height > 0)
                {
                    var calcualweight = Convert.ToDecimal(((len * wid * height) / 5000) * qty);
                    var totalCost = Convert.ToDouble(String.Format("{0:0.00}", calcualweight));
                    weight += Convert.ToDecimal(totalCost);
                }
            }

            var sum = weight;
            if (sum == 0.0m)
            {
                return 0.0m;
            }
            else
            {
                string totalsum = sum.ToString();
                string[] key = totalsum.Split('.');
                decimal kgs = TotalKg(package);
                if (key.Length > 1)
                {
                    var AS = Convert.ToDouble(key[1]);
                    if (AS == 0)
                    {
                        if (kgs > Convert.ToDecimal(key[0]))
                        {
                            return TotalKg(package);
                        }
                        else
                        {
                            return Convert.ToDecimal(key[0]);
                        }
                    }
                    else
                    {
                        if (AS > 49)
                        {
                            var r = Convert.ToDecimal(key[0]) + 1;
                            if (kgs > r)
                            {
                                return TotalKg(package);
                            }
                            else
                            {
                                return r;
                            }
                        }
                        else
                        {
                            var s = Convert.ToDecimal(key[0]) + Convert.ToDecimal(0.50);
                            if (kgs > s)
                            {
                                return TotalKg(package);
                            }
                            else
                            {
                                return s;
                            }
                        }
                    }
                }
                else
                {
                    if (kgs > Convert.ToDecimal(key[0]))
                    {
                        return TotalKg(package);
                    }
                    else
                    {
                        return Convert.ToDecimal(key[0]);
                    }
                }
            }
        }

        public decimal TotalKg(List<Models.Package> package)
        {
            if (package.Count >= 1)
            {
                decimal total = 0.0m;
                for (var i = 0; i < package.Count(); i++)
                {
                    var product = package[i];
                    if (product.Weight == 0.0m)
                    {
                        total += 0.0m;
                    }
                    else
                    {
                        if (product.CartoonValue == 0)
                        {
                            var catroon = 0;
                            total = total + product.Weight * catroon;
                        }
                        else
                        {
                            total = total + product.Weight * product.CartoonValue;
                        }
                    }
                }
                return total;
            }
            else
            {
                return 0.0m;
            }
        }

        public List<FrayteRateCard> MappingFrayteRateResponse(List<DirectBookingService> DirectBookingService, string rateTypeCalculation, string rateCurrencyCode)
        {
            List<FrayteRateCard> frayteRateResponse = new List<FrayteRateCard>();
            foreach (var RateCard in DirectBookingService)
            {
                FrayteRateCard frayteRate = new FrayteRateCard();
                frayteRate.RateId = CryptoEngine.Encrypt(RateCard.LogisticServiceId + "#" + RateCard.BaseRate + "#" + RateCard.MarginPercent + "#" + RateCard.AdditionalSurcharge + "#" + RateCard.FuelSurcharge + "#" + RateCard.LogisticShipmentId + "#" + RateCard.CourierAccountId + "#" + RateCard.LogisticServiceType + "#" + RateCard.CourierName + "#" + RateCard.CourierAccountCountryCode + "#" + RateCard.PackageCalculationType + "#" + RateCard.FuelDate + "#" + RateCard.IntegrationAccountId + "#" + RateCard.CourierAccountNo + "#" + RateCard.CourierAccountCountryCode, Models.EncriptionKey.PrivateKey);
                frayteRate.RateType = RateCard.RateType;
                frayteRate.RateServiceType = RateCard.LogisticServiceType;
                frayteRate.RateCalculationId = CryptoEngine.Encrypt(RateCard.BaseRate + "#" + RateCard.MarginPercent + "#" + RateCard.AdditionalSurcharge + "#" + RateCard.FuelSurcharge, Models.EncriptionKey.PrivateKey);
                frayteRate.RateTypeCalculation = rateTypeCalculation;
                frayteRate.FuelCalcuatedOn = RateCard.FuelDate.Value.ToString("MMM-yyyy");
                frayteRate.TotalCost = Math.Round((decimal)RateCard.TotalEstimatedCharge, 2);
                frayteRate.RateCurrencyCode = rateCurrencyCode;
                frayteRate.CourierAccountId = CryptoEngine.Encrypt(RateCard.IntegrationAccountId + "#" + RateCard.CourierAccountNo, Models.EncriptionKey.PrivateKey);
                frayteRate.CourierName = RateCard.CourierName;
                frayteRate.CourierCountryCode = RateCard.CourierAccountCountryCode;
                frayteRateResponse.Add(frayteRate);
            }
            return frayteRateResponse;
        }

        public string[] UpdateDraftRateCard(int DirectShipmentDraftId, string RateCardId)
        {
            string[] decryptrate = new string[] { };
            try
            {
                if (DirectShipmentDraftId > 0)
                {
                    var draftdetail = dbContext.DirectShipmentDrafts.Find(DirectShipmentDraftId);
                    if (draftdetail != null)
                    {
                        decryptrate = DecryptRateCalculation(RateCardId);
                        if (decryptrate.Length > 0)
                        {
                            draftdetail.BaseRate = Convert.ToDecimal(decryptrate[1].ToString());
                            draftdetail.Margin = Math.Round((Convert.ToDecimal(decryptrate[1].ToString())) * (Convert.ToDecimal(decryptrate[2].ToString()) / 100), 2);
                            draftdetail.AdditionalSurcharge = Math.Round(Convert.ToDecimal(decryptrate[3].ToString()), 2);
                            draftdetail.FuelSurchargePercent = Math.Round(Convert.ToDecimal(decryptrate[4].ToString()), 2);
                            draftdetail.ShipmentTypeId = int.Parse(decryptrate[5].ToString());
                            draftdetail.CourierAccountId = int.Parse(decryptrate[6].ToString());
                            draftdetail.FuelSurCharge = Math.Round(((draftdetail.BaseRate + draftdetail.Margin + draftdetail.AdditionalSurcharge) * (Convert.ToDecimal(decryptrate[4].ToString())) / 100).Value, 2);
                            draftdetail.FuelMonthYear = DateTime.Parse(decryptrate[11].ToString() == "" ? DateTime.UtcNow.ToString() : decryptrate[11].ToString());
                            draftdetail.LogisticServiceType = decryptrate[8].ToString();
                            dbContext.Entry(draftdetail).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                            return decryptrate;
                        }
                        else
                        {
                            return decryptrate;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw (new FrayteApiException("UpdateRatesError", ex));
            }
            return decryptrate;
        }

        public string[] DecryptRateCalculation(string RateCardId)
        {
            var rate = CryptoEngine.Decrypt(RateCardId, Models.EncriptionKey.PrivateKey);
            string[] val = rate.Split(new string[] { "#" }, StringSplitOptions.None);
            return val;
        }

        public DirectBookingShipmentDraftDetail GetDirectShipmentDraftDetail(int DirectShipmentDraftId)
        {
            DirectBookingShipmentDraftDetail dbDetail = new DirectBookingShipmentDraftDetail();

            try
            {
                dbDetail.DirectShipmentDraftId = DirectShipmentDraftId;
                dbDetail.FrayteNumber = CommonConversion.GetNewFrayteNumber();

                //Step 1: Get Shipment Detail Draft
                new DirectShipmentRepository().GetDirectShipmnetDraftDetail(dbDetail, "FrayteApiDraft");

                //Step 2: Get Shipment Packages Draft Detail
                new DirectShipmentRepository().GetDirectShipmentPackagesDraftDetail(dbDetail, "FrayteApiDraft");

                //Step 3: Get Ship From and Ship To Detail Draft
                new DirectShipmentRepository().GetDirectShipmentCollectionDraftDetail(dbDetail, "FrayteApiDraft");

                //Step 4: Get Custom Info Detail Draft
                new DirectShipmentRepository().GetDirectShipmentCustomDraftDetail(dbDetail, "FrayteApiDraft");

                return dbDetail;
            }
            catch (Exception ex)
            {
                throw (new FrayteApiException("GettingShipmentDetailError", ex));
            }
        }

        internal void GetDirectShipmnetDraftDetail(DirectBookingShipmentDraftDetail dbDetail)
        {
            try
            {
                DirectShipmentDraft result = dbContext.DirectShipmentDrafts.Find(dbDetail.DirectShipmentDraftId);
                var detail = dbContext.AddressBooks.Where(d => d.AddressBookId == result.FromAddressId).FirstOrDefault();
                var TZ = new DirectShipmentRepository().TimeZoneDetail(detail.CountryId);
                var TimeZoneInformation = TimeZoneInfo.FindSystemTimeZoneById(TZ.Name);

                if (result != null)
                {
                    dbDetail.OpearionZoneId = result.OpearionZoneId.HasValue ? result.OpearionZoneId.Value : 0;
                    dbDetail.CustomerId = result.CustomerId;
                    dbDetail.ShipmentStatusId = result.ShipmentStatusId.HasValue ? result.ShipmentStatusId.Value : 0;
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
                            dbDetail.CustomerRateCard.IntegrationAccountId = courierResult.IntegrationAccountId;
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
                    if (result.CollectionTime.HasValue && result.CollectionDate.HasValue)
                    {
                        dbDetail.ReferenceDetail.CollectionDate = UtilityRepository.UtcDateToOtherTimezone(Convert.ToDateTime(result.CollectionDate), result.CollectionTime.Value, TimeZoneInformation).Item1;
                        dbDetail.ReferenceDetail.CollectionTime = CommonConversion.ConvertStringToTime(UtilityRepository.UtcDateToOtherTimezone(Convert.ToDateTime(result.CollectionDate), result.CollectionTime.Value, TimeZoneInformation).Item2).ToString();
                    }

                    dbDetail.ReferenceDetail.ContentDescription = result.ContentDescription;
                    dbDetail.ReferenceDetail.Reference1 = result.Reference1;
                    dbDetail.ReferenceDetail.SpecialInstruction = result.SpecialInstruction;

                    dbDetail.ShipFrom = new DirectBookingDraftCollection();
                    dbDetail.ShipFrom.AddressBookId = result.FromAddressId.HasValue ? result.FromAddressId.Value : 0;

                    dbDetail.ShipTo = new DirectBookingDraftCollection();
                    dbDetail.ShipTo.AddressBookId = result.ToAddressId.HasValue ? result.ToAddressId.Value : 0;

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
                    dbDetail.CreatedBy = result.CreatedBy.HasValue ? result.CreatedBy.Value : 0;
                    dbDetail.TaxAndDutiesAcceptedBy = result.TaxAndDutiesAcceptedBy;
                    dbDetail.AddressType = result.AddressType;
                }
            }
            catch (Exception ex)
            {
                throw (new FrayteApiException("ShipmentDetailError", ex));
            }
        }

        internal void GetDirectShipmentPackagesDraftDetail(DirectBookingShipmentDraftDetail dbDetail)
        {
            PackageDraft dbPackage;
            dbDetail.Packages = new List<PackageDraft>();
            try
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
                        dbPackage.CartoonValue = Obj.CartoonValue.HasValue ? Obj.CartoonValue.Value : 0;
                        dbPackage.Height = Obj.Height.HasValue ? Obj.Height.Value : 0;
                        dbPackage.Length = Obj.Length.HasValue ? Obj.Length.Value : 0;
                        if (Obj.DeclaredValue.HasValue)
                        {
                            dbPackage.Value = Obj.DeclaredValue.Value;
                        }
                        dbPackage.Weight = Obj.Weight.HasValue ? Obj.Weight.Value : 0;
                        dbPackage.Width = Obj.Width.HasValue ? Obj.Width.Value : 0;
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
            catch (Exception ex)
            {
                throw (new FrayteApiException("PackageDetailError", ex));
            }
        }

        internal void GetDirectShipmentCollectionDraftDetail(DirectBookingShipmentDraftDetail dbDetail)
        {
            try
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
            catch (Exception ex)
            {
                throw (new FrayteApiException("AddressDetailError", ex));
            }
        }

        internal void GetDirectShipmentCustomDraftDetail(DirectBookingShipmentDraftDetail dbDetail)
        {
            try
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
                    dbDetail.CustomInfo.TermOfTrade = customDetail.TermOfTrade;
                }
            }
            catch (Exception ex)
            {
                throw (new FrayteApiException("ShipmentCustomDetailError", ex));
            }
        }

        public Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);

            Graphics g = Graphics.FromImage(bmp);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }

        public FrayteShipmentResponseDto MappingFrayteShipmentResponse(DirectBookingShipmentDraftDetail directBookingDetail, IntegrtaionResult integrtaionResult, string DirectShipmentid, string[] ratecard, string RateId)
        {
            var FrayteShipmentResponse = new FrayteShipmentResponseDto();

            FrayteShipmentResponse.ShipmentBookingId = CryptoEngine.Encrypt(DirectShipmentid, Models.EncriptionKey.PrivateKey);
            FrayteShipmentResponse.Status = true;
            FrayteShipmentResponse.Description = "Your Shipment is created successfully.";
            FrayteShipmentResponse.TrackingNumber = integrtaionResult.TrackingNumber.Contains("Order_") ? integrtaionResult.TrackingNumber.Replace("Order_", "") : integrtaionResult.TrackingNumber;
            FrayteShipmentResponse.Mode = AppSettings.ApplicationMode;
            FrayteShipmentResponse.CreatedOn = directBookingDetail.ReferenceDetail.CollectionDate;
            FrayteShipmentResponse.Currency = directBookingDetail.Currency.CurrencyCode;
            FrayteShipmentResponse.PaymentParty = new PaymentPartyDto()
            {
                AccountNo = directBookingDetail.PaymentPartyAccountNumber,
                PartyType = directBookingDetail.PayTaxAndDuties,
            };
            FrayteShipmentResponse.FromAddress = new FromAddressDto()
            {
                CompanyName = directBookingDetail.ShipFrom.CompanyName,
                Email = directBookingDetail.ShipFrom.Email,
                FirstName = directBookingDetail.ShipFrom.FirstName,
                LastName = directBookingDetail.ShipFrom.LastName,
                Phone = directBookingDetail.ShipFrom.Phone,
                Address = new ShipAddressDto()
                {
                    Address1 = directBookingDetail.ShipFrom.Address,
                    Address2 = directBookingDetail.ShipFrom.Address2,
                    Area = directBookingDetail.ShipFrom.Area,
                    City = directBookingDetail.ShipFrom.City,
                    CountryCode = directBookingDetail.ShipFrom.Country.Code2,
                    Postcode = directBookingDetail.ShipFrom.PostCode,
                    State = directBookingDetail.ShipFrom.State,
                },
            };
            FrayteShipmentResponse.ToAddress = new ToAddressDto()
            {
                CompanyName = directBookingDetail.ShipTo.CompanyName,
                Email = directBookingDetail.ShipTo.Email,
                FirstName = directBookingDetail.ShipTo.FirstName,
                LastName = directBookingDetail.ShipTo.LastName,
                Phone = directBookingDetail.ShipTo.Phone,
                Address = new ShipAddressDto()
                {
                    Address1 = directBookingDetail.ShipTo.Address,
                    Address2 = directBookingDetail.ShipTo.Address2,
                    Area = directBookingDetail.ShipTo.Area,
                    City = directBookingDetail.ShipTo.City,
                    CountryCode = directBookingDetail.ShipTo.Country.Code2,
                    Postcode = directBookingDetail.ShipTo.PostCode,
                    State = directBookingDetail.ShipTo.State,
                },
            };

            string[] Ratecard = ratecard;
            FrayteShipmentResponse.Rates = new RateDto()
            {
                CourierName = ratecard[8],
                RateCurrencyCode = directBookingDetail.Currency.CurrencyCode,
                RateId = RateId,
                RateType = ratecard[7],
                TotalCost = TotalCost(ratecard)
            };
            FrayteShipmentResponse.CustomInfo = new ApiCustomInformation()
            {
                ContentsExplanation = directBookingDetail.CustomInfo.ContentsExplanation,
                ContentsType = directBookingDetail.CustomInfo.ContentsType,
                NonDeliveryOption = directBookingDetail.CustomInfo.NonDeliveryOption,
                CustomsSigner = directBookingDetail.CustomInfo.CustomsSigner,
                RestrictionComments = directBookingDetail.CustomInfo.RestrictionComments,
                RestrictionType = directBookingDetail.CustomInfo.RestrictionType,
            };

            var lableinfo = new APIShipmentRepository().GetLableInfoAll(integrtaionResult.TrackingNumber);
            FrayteShipmentResponse.PackageDetails = new PackageDetailDto();

            if (ratecard[8] == FrayteCourierCompany.DHL)
            {
                FrayteShipmentResponse.PackageDetails.PackageTrackingNumber = integrtaionResult.TrackingNumber.Replace("Order_", "");
                FrayteShipmentResponse.PackageDetails.LabelUrl = new List<string>();
                FrayteShipmentResponse.PackageDetails.LabelUrl.Add(AppSettings.LabelVirtualPath + "/PackageLabel/" + lableinfo.DirectShipmentId + "/" + lableinfo.LogisticLabel);
            }
            else
            {
                if (ratecard[8] == FrayteCourierCompany.TNT || ratecard[8] == FrayteCourierCompany.DPD)
                {

                    FrayteShipmentResponse.PackageDetails.PackageTrackingNumber = integrtaionResult.TrackingNumber;
                    FrayteShipmentResponse.PackageDetails.LabelUrl = new List<string>();
                    string[] LogisticLabel = lableinfo.LogisticLabel.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < LogisticLabel.Length; i++)
                    {
                        if (LogisticLabel[i].Trim() != "")
                        {

                            FrayteShipmentResponse.PackageDetails.LabelUrl.Add(AppSettings.LabelVirtualPath + "/PackageLabel/" + lableinfo.DirectShipmentId + "/" + LogisticLabel[i]);
                        }
                    }

                }
                else
                {
                    FrayteShipmentResponse.PackageDetails.PackageTrackingNumber = integrtaionResult.TrackingNumber;
                    FrayteShipmentResponse.PackageDetails.LabelUrl = new List<string>();
                    FrayteShipmentResponse.PackageDetails.LabelUrl.Add(AppSettings.LabelVirtualPath + "/PackageLabel/" + lableinfo.DirectShipmentId + "/" + lableinfo.LogisticLabel);
                }

            }
            return FrayteShipmentResponse;
        }

        public LabelInfo GetLableInfoAll(string TrackingNo)
        {
            LabelInfo label = new LabelInfo();

            label = (from DS in dbContext.DirectShipments
                     where DS.TrackingDetail.Contains(TrackingNo)
                     select new LabelInfo
                     {
                         DirectShipmentId = DS.DirectShipmentId,
                         LogisticLabel = DS.LogisticLabel
                     }).FirstOrDefault();

            return label;
        }

        public string ImageToBase64(string path)
        {
            using (Image image = Image.FromFile(path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }

        public decimal TotalCost(string[] ratecard)
        {
            var totalCost = new decimal();
            decimal Baserate = Convert.ToDecimal(ratecard[1]);
            decimal MarginPercent = Convert.ToDecimal(ratecard[2]);
            decimal AdditionalCharges = Convert.ToDecimal(ratecard[3]);
            decimal FuelPercent = Convert.ToDecimal(ratecard[4]);
            decimal Rate = (Baserate) + ((Baserate * MarginPercent) / 100);
            decimal TotalEstimatedCost = (Rate + AdditionalCharges) + (((Rate + AdditionalCharges) * FuelPercent) / 100);
            totalCost = Math.Round((TotalEstimatedCost), 2);
            return totalCost;
        }
    }
}