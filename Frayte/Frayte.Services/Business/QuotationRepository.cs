using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Frayte.Services.Business
{
    public class QuotationRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public FrayteQuotationResult SaveQuotation(FrayteQuotationShipment quotationDetail)
        {
            FrayteQuotationResult result = new FrayteQuotationResult();

            //Step 1: Save Quotation detail
            result = SaveQuotationShipmnetDetail(quotationDetail);

            //Step 2: Save Quotation Package Detail
            result = SaveQuotationPackages(quotationDetail);

            //Step 3: Save Quotation Optional Services
            result = SaveQuotationOptionalServices(quotationDetail);

            return result;
        }

        public FrayteQuotationResult EditQuotation(FrayteQuotationShipment quotationDetail)
        {
            FrayteQuotationResult result = new FrayteQuotationResult();

            //Step 1: Edit Quotation detail
            result = SaveQuotationShipmnetDetail(quotationDetail);

            //Step 2: Edit Quotation Optional Services 
            result = EditOptionalServices(quotationDetail);

            return result;
        }

        public FrayteResult DeleteQuotation(int QuotationShipmentId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var quotationdetail = dbContext.QuotationShipmentDetails.Where(p => p.QuotationShipmentId == QuotationShipmentId).ToList();
                if (quotationdetail != null)
                {
                    foreach (var Obj in quotationdetail)
                    {
                        dbContext.QuotationShipmentDetails.Remove(Obj);
                        dbContext.SaveChanges();
                    }
                }

                var quotation = dbContext.QuotationShipments.Find(QuotationShipmentId);
                if (quotation != null)
                {
                    dbContext.QuotationShipments.Remove(quotation);
                    dbContext.SaveChanges();
                    result.Status = true;
                }

                var optionalservicesid = dbContext.QuotationShipmentOptionalServices.Where(x => x.QuotationShipmentId == QuotationShipmentId).Select(p => p.QuotationShipmentOptionalServicesId).FirstOrDefault();
                if (optionalservicesid > 0)
                {
                    var detail = dbContext.QuotationShipmentOptionalServices.Find(optionalservicesid);
                    if (detail != null)
                    {
                        dbContext.QuotationShipmentOptionalServices.Remove(detail);
                        dbContext.SaveChanges();
                        result.Status = true;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Status = false;
                return result;
            }
        }

        public List<DirectBookingService> GetServices(DirectBookingFindService serviceRequest)
        {
            //Step 1: Get/Set Operation Zone Detail
            FrayteOperationZone OperationZone = UtilityRepository.GetOperationZone();
            serviceRequest.OperationZoneId = OperationZone.OperationZoneId;

            //Step 2: Check wheter both the companies are in Europe?
            bool isEuropeCountry = CheckEuropeCountries(serviceRequest);

            //Step 3: Find out the Logistic Type for the shipment
            string logisticType = UtilityRepository.GetLogisticType(OperationZone.OperationZoneName, serviceRequest.FromCountry.Code, serviceRequest.ToCountry.Code, isEuropeCountry);

            List<DirectBookingService> directBookingServices = new List<DirectBookingService>();

            var limit = dbContext.spGet_LogisticWeightLimit(serviceRequest.CustomerId, OperationZone.OperationZoneId, serviceRequest.Weight, logisticType).ToList();
            if (limit != null)
            {
                foreach (var ll in limit)
                {
                    DirectBookingService bookingService = new DirectBookingService();
                    bookingService.CourierName = ll.LogisticCompany;
                    bookingService.LogisticType = ll.LogisticTypeDisplay;
                    bookingService.RateTypeDisplay = ll.RateTypeDisplay;
                    bookingService.Weight = ll.WeightLimit.HasValue ? ll.WeightLimit.Value : 0.00m;
                    bookingService.IsWeightShow = ll.RateStatus.HasValue ? ll.RateStatus.Value : false;
                    directBookingServices.Add(bookingService);
                }
            }
            return directBookingServices;
        }

        public List<FrayteQuotationShipment> GetQuotationShipments(int operationZoneId, int userId, int CustomerId)
        {
            var _list = (from r in dbContext.QuotationShipments
                         join u in dbContext.Users on r.CustomerId equals u.UserId
                         join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                         join fc in dbContext.Countries on r.FromCountryId equals fc.CountryId
                         join tc in dbContext.Countries on r.ToCountryId equals tc.CountryId
                         join qsd in dbContext.QuotationShipmentDetails on r.QuotationShipmentId equals qsd.QuotationShipmentId
                         where r.OpearionZoneId == operationZoneId &&
                               r.CustomerId == CustomerId
                         select new FrayteQuotationShipment
                         {
                             CustomerId = r.CustomerId,
                             CustomerName = u.ContactName,
                             CustomerEmail = u.UserEmail,
                             CompanyName = u.CompanyName,
                             CreatedBy = r.CreatedBy,
                             QuotationShipmentId = r.QuotationShipmentId,
                             CourierDescription = r.CourierDescription,
                             OperationZoneId = r.OpearionZoneId,
                             ParcelType = new FrayteParcelType
                             {
                                 ParcelType = r.ParcelType
                             },
                             PakageCalculatonType = r.PackageCaculatonType,
                             BaseRate = r.BaseRate.Value,
                             MarginCost = r.Margin.Value,
                             FuelPercent = r.FuelSurchargePercent.Value,
                             TotalEstimatedWeight = r.TotalEstimatedWeight,
                             FuelMonthYear = r.FuelMonthYear,
                             EstimatedCost = (float)(Math.Round((r.BaseRate.Value + r.Margin.Value), 2)),
                             EstimatedTotalCost = (float)(Math.Round((r.BaseRate.Value + r.Margin.Value + r.FuelSurCharge.Value + r.AdditionalSurcharge.Value), 2)),
                             FuelSurCharge = (float)(Math.Round((r.FuelSurCharge.Value), 2)),
                             AdditionalSurcharge = (float)(Math.Round((r.AdditionalSurcharge.Value), 2)),
                             RateType = r.RateType,
                             CurrenyCode = r.CurrencyCode,
                             RateTypeDisplay = r.RateTypeDisplay,
                             ShipmentType = r.ShipmentType,
                             LogisticType = r.LogisticType,
                             LogisticTypeDisplay = r.LogisticType,
                             LogisticCompany = r.LogisticCompany,
                             ShipmentTypeDisplay = r.ShipmentType,
                             ParcelServiceType = r.ParcelServiceType,
                             LogisticCompanyDisplay = r.LogisticCompanyDisplay,
                             QuotationFromAddress = new QuotationAddress
                             {
                                 Country = new FrayteCountryCode()
                                 {
                                     CountryId = fc.CountryId,
                                     Name = fc.CountryName,
                                     Code = fc.CountryCode,
                                     Code2 = fc.CountryCode2
                                 },
                                 PostCode = r.FromPostCode
                             },
                             QuotationToAddress = new QuotationAddress
                             {
                                 Country = new FrayteCountryCode()
                                 {
                                     CountryId = tc.CountryId,
                                     Name = tc.CountryName,
                                     Code = tc.CountryCode,
                                     Code2 = tc.CountryCode2
                                 },
                                 PostCode = r.ToPostCode
                             },
                             CreatedOn = r.CreatedOn,
                             ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                             TransitTime = r.TransitTime
                         }).OrderByDescending(p => p.QuotationShipmentId).ToList();

            var finallist = _list.Where(p => p.CreatedOn.AddDays(p.ValidDays) >= DateTime.UtcNow).ToList();

            if (finallist != null && finallist.Count > 0)
            {
                foreach (var data in finallist)
                {
                    var Create = data.CreatedOn;
                    data.ValidDate = data.CreatedOn.AddDays(data.ValidDays);
                    data.ValidDays = Create.AddDays(data.ValidDays).Date >= DateTime.UtcNow.Date ? Create.AddDays(data.ValidDays).Date.Subtract(DateTime.UtcNow.Date).Days : data.ValidDays;
                    data.CompanyName = data.CompanyName;
                    var dbQuodationList = dbContext.QuotationShipmentDetails.Where(p => p.QuotationShipmentId == data.QuotationShipmentId).ToList();
                    data.QuotationPackages = new List<QuotationPackage>();
                    QuotationPackage package;

                    foreach (var dbQuodationDetail in dbQuodationList)
                    {
                        if (dbQuodationDetail != null)
                        {
                            package = new QuotationPackage();
                            package.CartoonValue = dbQuodationDetail.CartonValue;
                            package.QuotationShipmentDetailId = dbQuodationDetail.QuotationShipmentDetailId;
                            package.QuotationShipmentId = dbQuodationDetail.QuotationShipmentId;
                            package.Length = dbQuodationDetail.Length;
                            package.Width = dbQuodationDetail.Width;
                            package.Height = dbQuodationDetail.Height;
                            package.Weight = dbQuodationDetail.Weight;
                            data.QuotationPackages.Add(package);
                        }
                    }
                }
            }
            return finallist;
        }

        public FrayteQuotationReport GetQuotationDetail(int QuotationShipmentId, string CustomerName)
        {
            var item = (from qs in dbContext.QuotationShipments
                        join qsd in dbContext.QuotationShipmentDetails on qs.QuotationShipmentId equals qsd.QuotationShipmentId
                        join c in dbContext.Countries on qs.FromCountryId equals c.CountryId
                        join cc in dbContext.Countries on qs.ToCountryId equals cc.CountryId
                        join u in dbContext.Users on qs.CustomerId equals u.UserId
                        join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                        where qs.QuotationShipmentId == QuotationShipmentId
                        select new FrayteQuotationReport
                        {
                            QuotationShipmentId = qs.QuotationShipmentId,
                            OperationZoneId = qs.OpearionZoneId,
                            CustomerName = CustomerName,
                            CompanyName = u.CompanyName,
                            QuoteCurrency = qs.CurrencyCode,
                            FrayteAccountNo = ua.AccountNo,
                            QuoteIssueDate = qs.CreatedOn,
                            ShipFrom = c.CountryName,
                            ShipTo = cc.CountryName,
                            TotalPrice = Math.Round(((qs.BaseRate.HasValue ? qs.BaseRate.Value : 0.0m) + (qs.AdditionalSurcharge.HasValue ? qs.AdditionalSurcharge.Value : 0.0m) + (qs.Margin.HasValue ? qs.Margin.Value : 0.0m) + (qs.FuelSurCharge.HasValue ? qs.FuelSurCharge.Value : 0.0m)), 2),
                            OfferValidity = ua.DaysValidity > 0 ? ua.DaysValidity.Value.ToString() + " Days" : "7 Days",
                            CartoonQty = 0,
                            PricePerKg = 0.0m,
                            GrossWeight = qs.ChargeableWeight.HasValue ? qs.ChargeableWeight.Value : 0.0m,
                            Volume = 0.0m,
                            Service = qs.LogisticCompanyDisplay + " " + qs.RateTypeDisplay,
                            Description = qs.CourierDescription,
                            TransitTime = qs.TransitTime,
                            Rate = Math.Round(((qs.BaseRate.HasValue ? qs.BaseRate.Value : 0.0m) + (qs.AdditionalSurcharge.HasValue ? qs.AdditionalSurcharge.Value : 0.0m) + (qs.Margin.HasValue ? qs.Margin.Value : 0.0m)), 2),
                            FuelSurcharge = qs.FuelSurCharge.HasValue ? qs.FuelSurCharge.Value : 0.0m,
                            SupplementryCharge = qs.AdditionalSurcharge.HasValue ? qs.AdditionalSurcharge.Value : 0.0m,
                            Origin = c.CountryName,
                            Destination = cc.CountryName,
                            ChargeableWeight = qs.ChargeableWeight.HasValue ? qs.ChargeableWeight.Value : 0.0m,
                            PackageCalculationType = qs.PackageCaculatonType,
                            CreatedOn = qs.CreatedOn,
                        }).FirstOrDefault();

            if (item.OfferValidity != "")
            {
                var OfferValue = item.OfferValidity.Split(' ');
                var val = double.Parse(OfferValue[0]);
                var a = item.CreatedOn.Value.AddDays(double.Parse(OfferValue[0]));

                if (a.Month > item.CreatedOn.Value.Month && (item.CreatedOn.Value.Month == 1 || item.CreatedOn.Value.Month == 3 || item.CreatedOn.Value.Month == 5 || item.CreatedOn.Value.Month == 7 ||
                    item.CreatedOn.Value.Month == 8 || item.CreatedOn.Value.Month == 10 || item.CreatedOn.Value.Month == 12))
                {
                    var aa = item.CreatedOn.Value.AddDays(double.Parse(OfferValue[0])).Day + 31;
                    item.OfferValidity = (aa - DateTime.Now.Day).ToString();
                }
                else if (a.Month > item.CreatedOn.Value.Month && (item.CreatedOn.Value.Month == 4 || item.CreatedOn.Value.Month == 6 || item.CreatedOn.Value.Month == 9 || item.CreatedOn.Value.Month == 11))
                {
                    var aa = item.CreatedOn.Value.AddDays(double.Parse(OfferValue[0])).Day + 30;
                    item.OfferValidity = (aa - DateTime.Now.Day).ToString();
                }
                else if (a.Month > item.CreatedOn.Value.Month && item.CreatedOn.Value.Month == 2)
                {
                    var aa = item.CreatedOn.Value.AddDays(double.Parse(OfferValue[0])).Day + 28;
                    item.OfferValidity = (aa - DateTime.Now.Day).ToString();
                }
                else
                {
                    item.OfferValidity = (item.CreatedOn.Value.AddDays(double.Parse(OfferValue[0])).Day - DateTime.Now.Day).ToString();
                }

                if (double.Parse(item.OfferValidity) == 1)
                {
                    item.OfferValidity = item.OfferValidity + " Day";
                }
                else
                {
                    item.OfferValidity = item.OfferValidity + " Days";
                }
            }

            return item;
        }

        public Frayte.Services.DataAccess.Timezone GetUserTimeZone(int CreatedBy)
        {
            var TimeZone = (from uu in dbContext.Users
                            join tz in dbContext.Timezones on uu.TimezoneId equals tz.TimezoneId
                            where uu.UserId == CreatedBy
                            select tz).FirstOrDefault();
            return TimeZone;
        }

        public FrayteSalesRepresentiveEmail SalesRepresentiveEmail(int UserId, int RoleId)
        {

            var sales = new FrayteSalesRepresentiveEmail();
            var OperationStaff = new FrayteSalesRepresentiveEmail();
            if (RoleId == 1 || RoleId == 3)
            {
                sales = (from US1 in dbContext.Users
                         join US2 in dbContext.UserAdditionals on US1.UserId equals US2.UserId
                         join US3 in dbContext.Users on US2.SalesUserId equals US3.UserId
                         where US1.UserId == UserId
                         select new FrayteSalesRepresentiveEmail
                         {
                             SalesRepresentiveName = US3.ContactName,
                             SalesEmail = US3.Email,
                             DeptName = "SalesRepresentative"
                         }).FirstOrDefault();

                if (sales != null)
                {
                    return sales;
                }
                else
                {
                    OperationStaff = (from US1 in dbContext.Users
                                      join US2 in dbContext.UserAdditionals on US1.UserId equals US2.UserId
                                      join US3 in dbContext.Users on US2.OperationUserId equals US3.UserId
                                      where US1.UserId == UserId
                                      select new FrayteSalesRepresentiveEmail
                                      {
                                          SalesRepresentiveName = US3.ContactName,
                                          SalesEmail = US3.Email,
                                          DeptName = "OperationStaff"
                                      }).FirstOrDefault();
                    return OperationStaff;
                }

            }
            else if (RoleId == 6)
            {
                OperationStaff = (from US1 in dbContext.Users
                                  join US2 in dbContext.UserAdditionals on US1.UserId equals US2.UserId
                                  join US3 in dbContext.Users on US2.UserId equals US3.UserId
                                  where US1.UserId == UserId
                                  select new FrayteSalesRepresentiveEmail
                                  {
                                      SalesRepresentiveName = US3.ContactName,
                                      SalesEmail = US3.Email,
                                      DeptName = "OperationStaff"
                                  }).FirstOrDefault();
                return OperationStaff;
            }
            return null;
        }

        public FrayteResult QuotationValidity(int QuotationShipmentId)
        {
            FrayteResult result = new FrayteResult();
            var item = (from qs in dbContext.QuotationShipments
                        join ua in dbContext.UserAdditionals on qs.CustomerId equals ua.UserId
                        where qs.QuotationShipmentId == QuotationShipmentId
                        select new
                        {
                            CreatedOn = qs.CreatedOn,
                            DaysValidity = ua.DaysValidity
                        }).FirstOrDefault();

            if (item != null)
            {
                DateTime days = item.CreatedOn.AddDays(item.DaysValidity == null ? 0 : item.DaysValidity.Value);
                if (days < DateTime.UtcNow)
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

        public int SumofCartoonQty(int QuotationShipmentId)
        {
            int sum = 0;
            sum = dbContext.QuotationShipmentDetails.Where(p => p.QuotationShipmentId == QuotationShipmentId).Sum(p => p.CartonValue);
            return sum;
        }

        public decimal TotalVolume(int QuotationShipmentId, string PackageCalculationType)
        {
            decimal volume = 0.0m;
            var item = dbContext.QuotationShipmentDetails.Where(p => p.QuotationShipmentId == QuotationShipmentId).ToList();
            foreach (var Obj in item)
            {
                if (PackageCalculationType == FraytePakageCalculationType.kgtoCms)
                    volume += (Obj.Length * 0.01m) + (Obj.Width * 0.01m) + (Obj.Height * 0.01m);
                else
                    volume += (Obj.Length * 0.0254m) + (Obj.Width * 0.0254m) + (Obj.Height * 0.0254m);
            }
            return Math.Round(volume, 2);
        }

        public CustomerRate GetLogsiticServiceId(int QuotationShipmentId)
        {
            var detail = (from qs in dbContext.QuotationShipments
                          join lsst in dbContext.LogisticServiceShipmentTypes on qs.ShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                          join ls in dbContext.LogisticServices on lsst.LogisticServiceId equals ls.LogisticServiceId
                          join u in dbContext.Users on qs.CustomerId equals u.UserId
                          where qs.QuotationShipmentId == QuotationShipmentId
                          select new CustomerRate
                          {
                              LogisticServiceId = ls.LogisticServiceId,
                              CustomerName = u.CompanyName,
                              UserId = u.UserId,
                              RateType = ls.RateTypeDisplay,
                              LogisticCompany = ls.LogisticCompanyDisplay,
                              LogisticType = ls.LogisticTypeDisplay,
                              FileType = "PDF"
                          }).FirstOrDefault();

            return detail;
        }

        public string CustomerAddressType(int CustomerId)
        {
            string type = string.Empty;
            type = (from cl in dbContext.CustomerLogistics
                    where cl.UserId == CustomerId &&
                          cl.LogisticServiceId == 13
                    select cl.LogisticServiceType).FirstOrDefault();

            if (type != null)
            {
                return type;
            }
            else
            {
                return "";
            }
        }

        public FrayteTNTResult GetTNTInforamtion(int LogisticServiceId)
        {
            var info = (from ls in dbContext.LogisticServices
                        where ls.LogisticServiceId == LogisticServiceId
                        select new FrayteTNTResult
                        {
                            LogisticCompany = ls.LogisticCompany,
                            OperationZoneId = ls.OperationZoneId
                        }).FirstOrDefault();

            return info;
        }

        public bool CustomerLogisticServices(int CustomerId)
        {
            var logistic = (from cl in dbContext.CustomerLogistics
                            where cl.UserId == CustomerId
                            select cl).ToList();

            if (logistic != null && logistic.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region Private Methods SaveQuotation

        private FrayteQuotationResult SaveQuotationShipmnetDetail(FrayteQuotationShipment quotationDetail)
        {
            FrayteQuotationResult result = new FrayteQuotationResult();
            try
            {
                if (quotationDetail != null)
                {
                    QuotationShipment dbQuotationShipment;
                    if (quotationDetail.QuotationShipmentId == 0)
                    {
                        dbQuotationShipment = new QuotationShipment();
                        FrayteOperationZone operationZone = UtilityRepository.GetOperationZone();

                        DirectBookingFindService serviceRequest = new DirectBookingFindService();
                        serviceRequest.FromCountry = quotationDetail.QuotationFromAddress.Country;
                        serviceRequest.ToCountry = quotationDetail.QuotationToAddress.Country;
                        serviceRequest.OperationZoneId = operationZone.OperationZoneId;
                        bool isEuropeCountry = CheckEuropeCountries(serviceRequest);

                        dbQuotationShipment.LogisticType = UtilityRepository.GetLogisticType(operationZone.OperationZoneName, quotationDetail.QuotationFromAddress.Country.Code, quotationDetail.QuotationToAddress.Country.Code, isEuropeCountry);

                        if (dbQuotationShipment.LogisticType == FrayteLogisticType.UKShipment)
                        {
                            dbQuotationShipment.LogisticCompany = quotationDetail.QuotationRateCard.CourierName;
                            dbQuotationShipment.LogisticCompanyDisplay = quotationDetail.QuotationRateCard.DisplayName;
                            if (quotationDetail.QuotationRateCard.DisplayName == FrayteLogisticServiceType.DHL)
                            {
                                dbQuotationShipment.RateType = quotationDetail.QuotationRateCard.RateType;
                                dbQuotationShipment.RateTypeDisplay = quotationDetail.QuotationRateCard.RateTypeDisplay;
                            }
                        }
                        else
                        {
                            dbQuotationShipment.LogisticCompany = quotationDetail.QuotationRateCard.CourierName;
                            dbQuotationShipment.LogisticCompanyDisplay = quotationDetail.QuotationRateCard.DisplayName;
                            dbQuotationShipment.RateType = quotationDetail.QuotationRateCard.RateType;
                            dbQuotationShipment.RateTypeDisplay = quotationDetail.QuotationRateCard.RateTypeDisplay;
                        }

                        dbQuotationShipment.ShipmentType = quotationDetail.QuotationRateCard.WeightType;
                        dbQuotationShipment.ParcelServiceType = quotationDetail.QuotationRateCard.ParcelServiceType;
                        dbQuotationShipment.CourierDescription = quotationDetail.QuotationRateCard.CourierDescription;
                        dbQuotationShipment.IntegrationAccountId = quotationDetail.QuotationRateCard.IntegrationAccountId;
                        dbQuotationShipment.CourierAccountNo = quotationDetail.QuotationRateCard.CourierAccountNo;

                        #region -- Set Base Rate --

                        //BaseRate
                        if (quotationDetail.QuotationRateCard.LogisticDescription == FrayteShipmentType.HeavyWeight)
                        {
                            dbQuotationShipment.BaseRate = quotationDetail.QuotationRateCard.BaseRate * quotationDetail.QuotationRateCard.Weight;
                        }
                        else
                        {
                            dbQuotationShipment.BaseRate = quotationDetail.QuotationRateCard.BaseRate;
                        }
                        dbQuotationShipment.ShipmentTypeId = quotationDetail.QuotationRateCard.LogisticShipmentId;
                        decimal TotalCustomerPrice = ((dbQuotationShipment.BaseRate.HasValue ? dbQuotationShipment.BaseRate.Value : 0.00m) + Convert.ToDecimal(quotationDetail.QuotationRateCard.AdditionalSurcharge));
                        dbQuotationShipment.Margin = Math.Round(((TotalCustomerPrice) * (quotationDetail.QuotationRateCard.MarginPercent) / 100), 2);
                        dbQuotationShipment.FuelSurCharge = Math.Round((TotalCustomerPrice + (dbQuotationShipment.Margin.HasValue ? dbQuotationShipment.Margin.Value : 0.0m)) * ((decimal)quotationDetail.QuotationRateCard.FuelSurcharge / 100), 2);
                        dbQuotationShipment.FuelSurchargePercent = (decimal)quotationDetail.QuotationRateCard.FuelSurcharge;

                        //Additional Surcharge
                        dbQuotationShipment.AdditionalSurcharge = Convert.ToDecimal(quotationDetail.QuotationRateCard.AdditionalSurcharge);

                        #endregion

                        dbQuotationShipment.CreatedOn = UtilityRepository.ConvertDateTimetoUniversalTime((DateTime)quotationDetail.CreatedOn);
                        dbQuotationShipment.CreatedBy = quotationDetail.CreatedBy;
                        dbQuotationShipment.CustomerId = quotationDetail.CustomerId;
                        dbQuotationShipment.FromCountryId = quotationDetail.QuotationFromAddress.Country.CountryId;
                        dbQuotationShipment.ToCountryId = quotationDetail.QuotationToAddress.Country.CountryId;
                        dbQuotationShipment.FromPostCode = quotationDetail.QuotationFromAddress.PostCode;
                        dbQuotationShipment.ToPostCode = quotationDetail.QuotationToAddress.PostCode;
                        dbQuotationShipment.OpearionZoneId = quotationDetail.OperationZoneId;
                        dbQuotationShipment.PackageCaculatonType = quotationDetail.PakageCalculatonType;
                        dbQuotationShipment.FuelMonthYear = quotationDetail.QuotationRateCard.FuelDate;
                        dbQuotationShipment.CurrencyCode = quotationDetail.QuotationRateCard.CustomerCurrency;
                        dbQuotationShipment.TotalEstimatedWeight = quotationDetail.TotalEstimatedWeight;
                        dbQuotationShipment.AddressType = quotationDetail.AddressType;
                        dbQuotationShipment.TransitTime = quotationDetail.QuotationRateCard.TransitTime;
                        if (quotationDetail.ParcelType != null)
                        {
                            dbQuotationShipment.ParcelType = quotationDetail.ParcelType.ParcelType;
                        }
                        dbQuotationShipment.ChargeableWeight = quotationDetail.QuotationRateCard.Weight;
                        dbContext.QuotationShipments.Add(dbQuotationShipment);
                        dbContext.SaveChanges();

                        quotationDetail.QuotationShipmentId = dbQuotationShipment.QuotationShipmentId;
                        quotationDetail.LogisticType = dbQuotationShipment.LogisticType;
                        quotationDetail.CurrenyCode = dbQuotationShipment.CurrencyCode;
                        quotationDetail.CourierDescription = dbQuotationShipment.CourierDescription;
                        quotationDetail.LogisticCompany = dbQuotationShipment.LogisticCompany;
                        quotationDetail.LogisticCompanyDisplay = dbQuotationShipment.LogisticCompanyDisplay;
                        quotationDetail.RateType = dbQuotationShipment.RateType;
                        quotationDetail.RateTypeDisplay = dbQuotationShipment.RateTypeDisplay;
                        quotationDetail.ShipmentType = dbQuotationShipment.ShipmentType;
                        quotationDetail.PakageCalculatonType = dbQuotationShipment.PackageCaculatonType;
                        quotationDetail.ParcelServiceType = dbQuotationShipment.ParcelServiceType;
                        if (dbQuotationShipment.BaseRate.HasValue && dbQuotationShipment.Margin.HasValue && dbQuotationShipment.AdditionalSurcharge.HasValue)
                        {
                            quotationDetail.EstimatedCost = (float)Math.Round((dbQuotationShipment.BaseRate.Value + dbQuotationShipment.Margin.Value), 2);
                            quotationDetail.EstimatedTotalCost = (float)Math.Round((dbQuotationShipment.BaseRate.Value + dbQuotationShipment.Margin.Value + dbQuotationShipment.AdditionalSurcharge.Value), 2);
                        }
                        if (dbQuotationShipment.FuelSurCharge.HasValue)
                        {
                            quotationDetail.FuelSurCharge = (float)Math.Round((dbQuotationShipment.FuelSurCharge.Value), 2);
                        }
                        quotationDetail.FuelPercent = dbQuotationShipment.FuelSurchargePercent;
                        quotationDetail.FuelMonthYear = dbQuotationShipment.FuelMonthYear;
                        quotationDetail.AddressType = dbQuotationShipment.AddressType;
                        quotationDetail.ValidDate = quotationDetail.CreatedOn.AddDays(quotationDetail.ValidDays);

                        result.Status = true;
                        result.QuotationDetail = new FrayteQuotationShipment();
                        result.QuotationDetail = quotationDetail;
                    }
                    else if (quotationDetail.QuotationShipmentId > 0)
                    {
                        dbQuotationShipment = dbContext.QuotationShipments.Find(quotationDetail.QuotationShipmentId);
                        if (dbQuotationShipment != null)
                        {
                            FrayteOperationZone operationZone = UtilityRepository.GetOperationZone();

                            DirectBookingFindService serviceRequest = new DirectBookingFindService();
                            serviceRequest.FromCountry = quotationDetail.QuotationFromAddress.Country;
                            serviceRequest.ToCountry = quotationDetail.QuotationToAddress.Country;
                            serviceRequest.OperationZoneId = operationZone.OperationZoneId;
                            bool isEuropeCountry = CheckEuropeCountries(serviceRequest);

                            dbQuotationShipment.LogisticType = UtilityRepository.GetLogisticType(operationZone.OperationZoneName, quotationDetail.QuotationFromAddress.Country.Code, quotationDetail.QuotationToAddress.Country.Code, isEuropeCountry);

                            if (dbQuotationShipment.LogisticType == FrayteLogisticType.UKShipment)
                            {
                                dbQuotationShipment.LogisticCompany = quotationDetail.QuotationRateCard.CourierName;
                                dbQuotationShipment.LogisticCompanyDisplay = quotationDetail.QuotationRateCard.DisplayName;
                            }
                            else
                            {
                                dbQuotationShipment.LogisticCompany = quotationDetail.QuotationRateCard.CourierName;
                                dbQuotationShipment.LogisticCompanyDisplay = quotationDetail.QuotationRateCard.DisplayName;
                                dbQuotationShipment.RateType = quotationDetail.QuotationRateCard.RateType;
                                dbQuotationShipment.RateTypeDisplay = quotationDetail.QuotationRateCard.RateTypeDisplay;
                            }

                            dbQuotationShipment.ShipmentType = quotationDetail.QuotationRateCard.WeightType;
                            dbQuotationShipment.ParcelServiceType = quotationDetail.QuotationRateCard.ParcelServiceType;
                            dbQuotationShipment.CourierDescription = quotationDetail.QuotationRateCard.CourierDescription;
                            dbQuotationShipment.IntegrationAccountId = quotationDetail.QuotationRateCard.IntegrationAccountId;
                            dbQuotationShipment.CourierAccountNo = quotationDetail.QuotationRateCard.CourierAccountNo;

                            #region -- Set Base Rate --

                            //BaseRate
                            if (quotationDetail.QuotationRateCard.LogisticDescription == FrayteShipmentType.HeavyWeight)
                            {
                                dbQuotationShipment.BaseRate = quotationDetail.QuotationRateCard.BaseRate * quotationDetail.QuotationRateCard.Weight;
                            }
                            else
                            {
                                dbQuotationShipment.BaseRate = quotationDetail.QuotationRateCard.BaseRate;
                            }
                            dbQuotationShipment.ShipmentTypeId = quotationDetail.QuotationRateCard.LogisticShipmentId;
                            dbQuotationShipment.Margin = Math.Round((dbQuotationShipment.BaseRate.Value) * (quotationDetail.QuotationRateCard.MarginPercent / 100), 2);
                            dbQuotationShipment.FuelSurCharge = Math.Round(((dbQuotationShipment.BaseRate.HasValue ? dbQuotationShipment.BaseRate.Value : 0) + (dbQuotationShipment.Margin.HasValue ? dbQuotationShipment.Margin.Value : 0) + (Convert.ToDecimal(quotationDetail.QuotationRateCard.AdditionalSurcharge))) * (decimal)(quotationDetail.QuotationRateCard.FuelSurcharge / 100), 2);
                            dbQuotationShipment.FuelSurchargePercent = (decimal)quotationDetail.QuotationRateCard.FuelSurcharge;

                            //Additional Surcharge
                            dbQuotationShipment.AdditionalSurcharge = Convert.ToDecimal(quotationDetail.QuotationRateCard.AdditionalSurcharge);

                            #endregion

                            dbQuotationShipment.CreatedOn = UtilityRepository.ConvertDateTimetoUniversalTime((DateTime)quotationDetail.CreatedOn);
                            dbQuotationShipment.CreatedBy = quotationDetail.CreatedBy;
                            dbQuotationShipment.CustomerId = quotationDetail.CustomerId;
                            dbQuotationShipment.FromCountryId = quotationDetail.QuotationFromAddress.Country.CountryId;
                            dbQuotationShipment.ToCountryId = quotationDetail.QuotationToAddress.Country.CountryId;
                            dbQuotationShipment.FromPostCode = quotationDetail.QuotationFromAddress.PostCode;
                            dbQuotationShipment.ToPostCode = quotationDetail.QuotationToAddress.PostCode;
                            dbQuotationShipment.OpearionZoneId = quotationDetail.OperationZoneId;
                            dbQuotationShipment.PackageCaculatonType = quotationDetail.PakageCalculatonType;
                            dbQuotationShipment.FuelMonthYear = quotationDetail.QuotationRateCard.FuelDate;
                            dbQuotationShipment.CurrencyCode = quotationDetail.QuotationRateCard.CustomerCurrency;
                            dbQuotationShipment.TotalEstimatedWeight = quotationDetail.TotalEstimatedWeight;
                            dbQuotationShipment.AddressType = quotationDetail.AddressType;
                            dbQuotationShipment.TransitTime = quotationDetail.QuotationRateCard.TransitTime;
                            if (quotationDetail.ParcelType != null)
                            {
                                dbQuotationShipment.ParcelType = quotationDetail.ParcelType.ParcelType;
                            }
                            dbContext.Entry(dbQuotationShipment).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            quotationDetail.QuotationShipmentId = dbQuotationShipment.QuotationShipmentId;
                            quotationDetail.LogisticType = dbQuotationShipment.LogisticType;
                            quotationDetail.CurrenyCode = dbQuotationShipment.CurrencyCode;
                            quotationDetail.CourierDescription = dbQuotationShipment.CourierDescription;
                            quotationDetail.LogisticCompany = dbQuotationShipment.LogisticCompany;
                            quotationDetail.LogisticCompanyDisplay = dbQuotationShipment.LogisticCompanyDisplay;
                            quotationDetail.RateType = dbQuotationShipment.RateType;
                            quotationDetail.RateTypeDisplay = dbQuotationShipment.RateTypeDisplay;
                            quotationDetail.ShipmentType = dbQuotationShipment.ShipmentType;
                            quotationDetail.PakageCalculatonType = dbQuotationShipment.PackageCaculatonType;
                            quotationDetail.ParcelServiceType = dbQuotationShipment.ParcelServiceType;
                            if (dbQuotationShipment.BaseRate.HasValue && dbQuotationShipment.Margin.HasValue && dbQuotationShipment.AdditionalSurcharge.HasValue)
                            {
                                quotationDetail.EstimatedCost = (float)Math.Round((dbQuotationShipment.BaseRate.Value + dbQuotationShipment.Margin.Value), 2);
                                quotationDetail.EstimatedTotalCost = (float)Math.Round((dbQuotationShipment.BaseRate.Value + dbQuotationShipment.Margin.Value + dbQuotationShipment.AdditionalSurcharge.Value), 2);
                            }
                            if (dbQuotationShipment.FuelSurCharge.HasValue)
                            {
                                quotationDetail.FuelSurCharge = (float)Math.Round((dbQuotationShipment.FuelSurCharge.Value), 2);
                            }
                            quotationDetail.FuelPercent = dbQuotationShipment.FuelSurchargePercent;
                            quotationDetail.FuelMonthYear = dbQuotationShipment.FuelMonthYear;
                            quotationDetail.AddressType = dbQuotationShipment.AddressType;
                            quotationDetail.ValidDate = quotationDetail.CreatedOn.AddDays(quotationDetail.ValidDays);

                            result.Status = true;
                            result.QuotationDetail = new FrayteQuotationShipment();
                            result.QuotationDetail = quotationDetail;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
            }

            return result;
        }

        private FrayteQuotationResult SaveQuotationPackages(FrayteQuotationShipment quotationDetail)
        {
            FrayteQuotationResult result = new FrayteQuotationResult();
            try
            {
                if (quotationDetail != null && quotationDetail.QuotationPackages != null && quotationDetail.QuotationPackages.Count > 0)
                {
                    QuotationShipmentDetail dbQuotationPackage;
                    foreach (var data in quotationDetail.QuotationPackages)
                    {
                        if (data.QuotationShipmentDetailId == 0)
                        {
                            dbQuotationPackage = new QuotationShipmentDetail();
                            dbQuotationPackage.QuotationShipmentId = quotationDetail.QuotationShipmentId;
                            dbQuotationPackage.Length = data.Length;
                            dbQuotationPackage.Width = data.Width;
                            dbQuotationPackage.Height = data.Height;
                            dbQuotationPackage.Weight = data.Weight;
                            dbQuotationPackage.CartonValue = data.CartoonValue;
                            dbQuotationPackage.DeclaredValue = data.Value;
                            dbQuotationPackage.PiecesContent = data.Content;
                            dbContext.QuotationShipmentDetails.Add(dbQuotationPackage);
                            dbContext.SaveChanges();
                            result.Status = true;
                        }
                    }
                }
                if (result.Status)
                {
                    result.QuotationDetail = quotationDetail;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
            }
            return result;
        }

        private FrayteQuotationResult SaveQuotationOptionalServices(FrayteQuotationShipment quotationDetail)
        {
            FrayteQuotationResult result = new FrayteQuotationResult();
            try
            {
                if (quotationDetail != null && quotationDetail.QuotationRateCard != null && quotationDetail.QuotationRateCard.OptionalServices.Count > 0)
                {
                    QuotationShipmentOptionalService dbservices;
                    foreach (DirectBookingOptionalServices opt in quotationDetail.QuotationRateCard.OptionalServices)
                    {
                        if (opt.IsEnable)
                        {
                            dbservices = new QuotationShipmentOptionalService();
                            dbservices.LogisticOptionalServiceId = opt.LogisticOptionalServiceId;
                            dbservices.QuotationShipmentId = quotationDetail.QuotationShipmentId;
                            dbservices.OptionalServiceCode = opt.ServiceCode;
                            dbContext.QuotationShipmentOptionalServices.Add(dbservices);
                            dbContext.SaveChanges();
                            result.Status = true;
                        }
                        else
                        {
                            result.Status = true;
                        }
                    }
                }
                else
                {
                    result.Status = true;
                }
                if (result.Status)
                {
                    result.QuotationDetail = quotationDetail;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
            }
            return result;
        }

        private FrayteQuotationResult EditOptionalServices(FrayteQuotationShipment quotationDetail)
        {
            FrayteQuotationResult result = new FrayteQuotationResult();
            try
            {
                var dbQuotationShipment = dbContext.QuotationShipmentOptionalServices.Where(x => x.QuotationShipmentId == quotationDetail.QuotationShipmentId).FirstOrDefault();
                if (dbQuotationShipment != null)
                {
                    if (quotationDetail != null && quotationDetail.QuotationRateCard != null && quotationDetail.QuotationRateCard.OptionalServices.Count > 0)
                    {
                        QuotationShipmentOptionalService dbservices;
                        foreach (DirectBookingOptionalServices opt in quotationDetail.QuotationRateCard.OptionalServices)
                        {
                            if (opt.IsEnable)
                            {
                                dbservices = new QuotationShipmentOptionalService();
                                dbservices.LogisticOptionalServiceId = opt.LogisticOptionalServiceId;
                                dbservices.QuotationShipmentId = quotationDetail.QuotationShipmentId;
                                dbservices.OptionalServiceCode = opt.ServiceCode;
                                dbContext.QuotationShipmentOptionalServices.Add(dbservices);
                                dbContext.SaveChanges();
                                result.Status = true;
                            }
                            else
                            {
                                result.Status = true;
                            }
                        }
                    }
                    else
                    {
                        var optionalservicesid = dbContext.QuotationShipmentOptionalServices.Where(x => x.QuotationShipmentId == quotationDetail.QuotationShipmentId).Select(p => p.QuotationShipmentOptionalServicesId).FirstOrDefault();
                        if (optionalservicesid > 0)
                        {
                            var detail = dbContext.QuotationShipmentOptionalServices.Find(optionalservicesid);
                            if (detail != null)
                            {
                                dbContext.QuotationShipmentOptionalServices.Remove(detail);
                                dbContext.SaveChanges();
                                result.Status = true;
                            }
                        }
                    }
                    if (result.Status)
                    {
                        result.QuotationDetail = quotationDetail;
                    }
                }
                else
                {
                    if (quotationDetail != null && quotationDetail.QuotationRateCard != null && quotationDetail.QuotationRateCard.OptionalServices.Count > 0)
                    {
                        QuotationShipmentOptionalService dbservices;
                        foreach (DirectBookingOptionalServices opt in quotationDetail.QuotationRateCard.OptionalServices)
                        {
                            if (opt.IsEnable)
                            {
                                dbservices = new QuotationShipmentOptionalService();
                                dbservices.LogisticOptionalServiceId = opt.LogisticOptionalServiceId;
                                dbservices.QuotationShipmentId = quotationDetail.QuotationShipmentId;
                                dbservices.OptionalServiceCode = opt.ServiceCode;
                                dbContext.QuotationShipmentOptionalServices.Add(dbservices);
                                dbContext.SaveChanges();
                                result.Status = true;
                            }
                            else
                            {
                                result.Status = true;
                            }
                        }
                    }
                    else
                    {
                        result.Status = true;
                    }
                    if (result.Status)
                    {
                        result.QuotationDetail = quotationDetail;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
            }
            return result;
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
                                    join
                                        c in dbContext.Countries on zc.CountryId equals c.CountryId
                                    join
                                        z in dbContext.LogisticServiceZones on zc.LogisticServiceZoneId equals z.LogisticServiceZoneId
                                    join
                                        ls in dbContext.LogisticServices on z.LogisticServiceId equals ls.LogisticServiceId
                                    where ls.OperationZoneId == serviceRequest.OperationZoneId && c.CountryId == serviceRequest.FromCountry.CountryId
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
                                    join
                                        c in dbContext.Countries on zc.CountryId equals c.CountryId
                                    join
                                        z in dbContext.LogisticServiceZones on zc.LogisticServiceZoneId equals z.LogisticServiceZoneId
                                    join
                                        ls in dbContext.LogisticServices on z.LogisticServiceId equals ls.LogisticServiceId
                                    where ls.OperationZoneId == serviceRequest.OperationZoneId && c.CountryId == serviceRequest.ToCountry.CountryId
                                    && (ls.LogisticType == FrayteLogisticType.EUImport || ls.LogisticType == FrayteLogisticType.EUExport)
                                    select zc).ToList();

                if (countryFound != null && countryFound.Count > 0)
                {
                    isToCountryInEurope = true;
                }
            }

            return isFromCountryInEurope && isToCountryInEurope;

        }

        #endregion
    }
}
