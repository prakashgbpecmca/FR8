using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.Express;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Business
{
    public class ExpresShipmentRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<ExpressGetShipmentModel> GetExpressShipments(ExpressTrackDirectBooking track)
        {
            List<ExpressGetShipmentModel> ShipmentList = new List<ExpressGetShipmentModel>();
            if (track != null)
            {
                FrayteOperationZone OperationZone = UtilityRepository.GetOperationZone();
                int RoleId = dbContext.UserRoles.Where(p => p.UserId == (track.UserId > 0 ? track.UserId : track.CustomerId)).FirstOrDefault().RoleId;

                int TotalRows = 0;
                int SkipRows = 0;

                SkipRows = (track.CurrentPage - 1) * track.TakeRows;

                DateTime? fromdate;
                DateTime? todate;

                var Airline = "";

                if (track.FromDate.HasValue)
                {
                    fromdate = track.FromDate.Value;
                }
                else
                {
                    fromdate = track.FromDate;
                }

                if (track.ToDate.HasValue)
                {
                    todate = track.ToDate.Value;
                }
                else
                {
                    todate = track.ToDate;
                }
                if (track.MAWB != null && track.MAWB != "")
                {
                    var Result1 = GetMawbStr(track.MAWB);
                    track.MAWB = Result1.Item1;
                    Airline = Result1.Item2;

                }
                var Result = dbContext.spGet_ExpressTrackAndTraceDetail(track.MAWB, fromdate, todate, track.AWBNumber, track.CourierTrackingNo, track.ShipmentStatusId, SkipRows, track.TakeRows, track.CustomerId, track.UserId, OperationZone.OperationZoneId, RoleId, track.SpecialSearchId, Airline).ToList();

                foreach (var res in Result)
                {
                    var STCode = "";
                    ExpressGetShipmentModel Shipment = new ExpressGetShipmentModel();
                    Shipment.CustomerId = res.CustomerId;
                    Shipment.CreatedOn = res.CreatedOnUtc.Date;
                    Shipment.Customer = res.ContactName;
                    Shipment.AWBNumber = res.AWBBarcode.Substring(0, 3) + " " + res.AWBBarcode.Substring(3, 3) + " " + res.AWBBarcode.Substring(6, 3) + " " + res.AWBBarcode.Substring(9, 3);
                    Shipment.ExpressShipmentId = res.ExpressId;
                    Shipment.Status = res.StatusName;
                    Shipment.StatusDisplay = res.DisplayStatusName + STCode;
                    Shipment.MAWB = res.MAWB;
                    Shipment.TotalWeight = res.TotalWeight != null ? res.TotalWeight.Value : 0;
                    Shipment.TotalCarton = res.TotalCartons != null ? res.TotalCartons.Value : 0;
                    Shipment.TotalRows = res.TotalRows;

                    //if (res.CreatedOnUtc.AddDays(7).Date < DateTime.Now.Date)
                    //{
                    //    Shipment.IsExpired = true;
                    //}
                    //else
                    //{
                    //    Shipment.IsExpired = false;
                    //}

                    ShipmentList.Add(Shipment);
                }
                return ShipmentList.OrderBy(a => a.Status).ToList();
            }
            return ShipmentList;
        }

        public List<ShipmentStatu> GetExpressStatusList(string BookingType)
        {
            var list = dbContext.ShipmentStatus.Where(x => x.BookingType == BookingType).ToList();
            return list;
        }

        public List<DirectBookingCustomer> GetExpressCustomers(int RoleId, int UserId)
        {
            List<DirectBookingCustomer> customers = new List<DirectBookingCustomer>();
            if (RoleId == 1)
            {
                // To Do : customer should come according to moduleType
                var operationzone = UtilityRepository.GetOperationZone();
                customers = (from r in dbContext.Users
                             join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                             join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                             where
                                ur.RoleId == (int)FrayteUserRole.Customer &&
                                r.IsActive == true &&
                                ua.IsExpressSolutions == true &&
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
                                 OperationZoneId = r.OperationZoneId
                             }).Distinct().ToList();
            }
            else if (RoleId == 20)
            {
                // To Do : customer should come according to moduleType
                var operationzone = UtilityRepository.GetOperationZone();
                customers = (from cs in dbContext.CustomerStaffs
                             join r in dbContext.Users on cs.UserId equals r.UserId into ca
                             from r in ca.DefaultIfEmpty()
                             join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId into uaa
                             from ua in uaa.DefaultIfEmpty()
                             join ur in dbContext.UserRoles on r.UserId equals ur.UserId into urr
                             from ur in urr.DefaultIfEmpty()
                             where
                                ur.RoleId == (int)FrayteUserRole.Customer &&
                                r.IsActive == true &&
                                ua.IsExpressSolutions == true &&
                                cs.CustomerStaffId == UserId &&
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
                                 OperationZoneId = r.OperationZoneId
                             }).Distinct().ToList();
            }
            return customers.OrderBy(p => p.CompanyName).ToList();
        }

        public FrayteResult DeleteExpressShipment(int ExpressShipmentId)
        {
            FrayteResult FE = new FrayteResult();
            FE.Status = false;
            var Result = dbContext.Expresses.Where(a => a.ExpressId == ExpressShipmentId).FirstOrDefault();
            if (Result != null)
            {
                var FromAddress = dbContext.ExpressAddresses.Where(a => a.ExpressAddressId == Result.FromAddressId).FirstOrDefault();
                var ToAddress = dbContext.ExpressAddresses.Where(a => a.ExpressAddressId == Result.ToAddressId).FirstOrDefault();
                var ExpressShipmentDetail = dbContext.ExpressDetails.Where(a => a.ExpressId == Result.ExpressId).ToList();
                if (FromAddress != null)
                {
                    dbContext.ExpressAddresses.Remove(FromAddress);
                    dbContext.SaveChanges();
                }
                if (ToAddress != null)
                {
                    dbContext.ExpressAddresses.Remove(ToAddress);
                    dbContext.SaveChanges();
                }
                if (ExpressShipmentDetail != null && ExpressShipmentDetail.Count > 0)
                {
                    dbContext.ExpressDetails.RemoveRange(ExpressShipmentDetail);
                    dbContext.SaveChanges();
                }
                dbContext.Expresses.Remove(Result);
                dbContext.SaveChanges();
                FE.Status = true;
                return FE;
            }
            else
            {
                return FE;
            }
        }

        public FrayteShipmentTracking GetExpressAWBTracking(int ExpressShipmentId, string TrackingType)
        {
            FrayteShipmentTracking FST = new FrayteShipmentTracking();
            try
            {
                var ShipmentDtl = dbContext.Expresses.Where(a => a.ExpressId == ExpressShipmentId).FirstOrDefault();
                if (TrackingType == "Internal")
                {
                    FST = (from ES in dbContext.Expresses
                           let ESHIP = ES.ExpressId
                           let Sid = ES.ShipmentStatusId
                           let ScannedBy = ES.AWBScannedBy
                           join SS in dbContext.ShipmentStatus on ES.ShipmentStatusId equals SS.ShipmentStatusId
                           join ESA in dbContext.ExpressAddresses on ES.FromAddressId equals ESA.ExpressAddressId into ESSA
                           from ESA in ESSA.DefaultIfEmpty()
                           join EST in dbContext.ExpressAddresses on ES.ToAddressId equals EST.ExpressAddressId into ESST
                           from EST in ESST.DefaultIfEmpty()
                           join HCS in dbContext.HubCarrierServices on ES.HubCarrierServiceId equals HCS.HubCarrierServiceId into HCSA
                           from HCS in HCSA.DefaultIfEmpty()
                           join HC in dbContext.HubCarriers on HCS.HubCarrierId equals HC.HubCarrierId into HCC
                           from HC in HCC.DefaultIfEmpty()
                           where ES.ExpressId == ExpressShipmentId
                           select new FrayteShipmentTracking
                           {
                               Status = true,
                               Tracking = new List<ShipmentTracking>()
                              {
                                  new ShipmentTracking()
                                  {
                                      TrackingNumber = ES.AWBBarcode,
                                      ExpressTrackingNumber = ES.TrackingNumber.Replace("Order_", ""),
                                      CreatedAt = ES.CreatedOnUtc != null ? ES.CreatedOnUtc : DateTime.MinValue,
                                      CreatedAtDate = ES.CreatedOnUtc != null ? ES.CreatedOnUtc.ToString() : "",
                                      UpdatedAtDate = ES.CreatedOnUtc != null ? ES.CreatedOnUtc.ToString() : "",
                                      UpdatedAt = ES.CreatedOnUtc != null ? ES.CreatedOnUtc : DateTime.MinValue,
                                      Status = SS.StatusName,
                                      StatusId = SS.ShipmentStatusId,
                                      StatusDisplay = SS.DisplayStatusName,
                                      Carrier = HC.Carrier != null ? HC.Carrier : "",
                                      EstimatedWeight = Sid != 37 ? (double)dbContext.ExpressDetails.Where(a => a.ExpressId == ESHIP).Sum(a => a.CartonQty * a.Weight) : 0,
                                      NoOfPieces = Sid != 37 ? dbContext.ExpressDetails.Where(a => a.ExpressId == ESHIP).Sum(a => a.CartonQty) : 0,
                                      TrackingDetails = (from fr in dbContext.TrackingDetails
                                                         join MUC in dbContext.MobileUserConfigurations on fr.MobileEventConfigurationId equals MUC.MasterTrackingDetailId
                                                         where fr.ShipmentId == ESHIP
                                                         && fr.CreatedBy == MUC.UserId
                                                              select new ShipmentTrackingDetail
                                                              {
                                                                Activity = MUC.EventMessage,
                                                                Location = fr.Country != null && fr.City != null ? fr.City.ToUpper() + "-" + fr.Country.ToUpper() : "",
                                                                Date = fr.CreatedOn != null ? fr.CreatedOn.Value : DateTime.MinValue,
                                                                Time = ""
                                                              }).ToList(),
                                     IsHeaderShow = true,
                                     ShowHideValue = "Hide",
                                     IsPanelShow = true,
                                     IsShowHideDetail = false
                                  }
                              }
                           }).FirstOrDefault();
                }
                else
                {
                    FST = (from ES in dbContext.Expresses
                           let ESHIP = ES.ExpressId
                           let Sid = ES.ShipmentStatusId
                           let ScannedBy = ES.AWBScannedBy
                           join SS in dbContext.ShipmentStatus on ES.ShipmentStatusId equals SS.ShipmentStatusId
                           join ESA in dbContext.ExpressAddresses on ES.FromAddressId equals ESA.ExpressAddressId into ESSA
                           from ESA in ESSA.DefaultIfEmpty()
                           join EST in dbContext.ExpressAddresses on ES.ToAddressId equals EST.ExpressAddressId into ESST
                           from EST in ESST.DefaultIfEmpty()
                           join HCS in dbContext.HubCarrierServices on ES.HubCarrierServiceId equals HCS.HubCarrierServiceId into HCSA
                           from HCS in HCSA.DefaultIfEmpty()
                           join HC in dbContext.HubCarriers on HCS.HubCarrierId equals HC.HubCarrierId into HCC
                           from HC in HCC.DefaultIfEmpty()
                           where ES.ExpressId == ExpressShipmentId
                           select new FrayteShipmentTracking
                           {
                               Status = true,
                               Tracking = new List<ShipmentTracking>()
                               {
                                  new ShipmentTracking()
                                  {
                                      TrackingNumber = ES.AWBBarcode,
                                      ExpressTrackingNumber = ES.TrackingNumber.Replace("Order_", ""),
                                      CreatedAt = ES.CreatedOnUtc != null ? ES.CreatedOnUtc : DateTime.MinValue,
                                      UpdatedAt = ES.CreatedOnUtc != null ? ES.CreatedOnUtc : DateTime.MinValue,
                                      Status = SS.StatusName,
                                      StatusId = SS.ShipmentStatusId,
                                      StatusDisplay = SS.DisplayStatusName,
                                      Carrier = HC.Carrier != null ? HC.Carrier : "",
                                      EstimatedWeight = Sid != 37 ? (double)dbContext.ExpressDetails.Where(a => a.ExpressId == ESHIP).Sum(a => a.CartonQty * a.Weight) : 0,
                                      NoOfPieces = Sid != 37 ? dbContext.ExpressDetails.Where(a => a.ExpressId == ESHIP).Sum(a => a.CartonQty) : 0,
                                      TrackingDetails = (from fr in dbContext.TrackingDetails
                                                         join MUC in dbContext.MobileUserConfigurations on fr.MobileEventConfigurationId equals MUC.MasterTrackingDetailId
                                                         where fr.ShipmentId == ESHIP
                                                         && fr.CreatedBy == MUC.UserId
                                                         && MUC.IsExternal == true
                                                         select new ShipmentTrackingDetail
                                                         {
                                                           Activity = MUC.EventMessage,
                                                           Location = fr.Country != null && fr.City != null ? fr.City.ToUpper() + "-" + fr.Country.ToUpper() : "",
                                                           Date = fr.CreatedOn != null ? fr.CreatedOn.Value : DateTime.MinValue,
                                                           Time = ""
                                                        }).ToList(),
                                     IsHeaderShow = true,
                                     ShowHideValue = "Hide",
                                     IsPanelShow = true,
                                     IsShowHideDetail = false
                                }
                               }
                           }).FirstOrDefault();
                }

                var res = (from ES in dbContext.Expresses
                           join US in dbContext.Users on ES.CustomerId equals US.UserId
                           join USA in dbContext.UserAdditionals on US.UserId equals USA.UserId
                           where ES.ExpressId == ExpressShipmentId
                           select new
                           {
                               US.TimezoneId
                           }).FirstOrDefault();

                var TZ = dbContext.Timezones.Where(a => a.TimezoneId == res.TimezoneId).FirstOrDefault();
                var TimezoneINformation = TimeZoneInfo.FindSystemTimeZoneById(TZ.Name);
                foreach (var res1 in FST.Tracking)
                {
                    res1.CreatedAtDate = UtilityRepository.UtcDateToOtherTimezone(res1.CreatedAt, res1.CreatedAt.TimeOfDay, TimezoneINformation).Item1.ToString("MM/dd/yyyy");
                    res1.UpdatedAtDate = UtilityRepository.UtcDateToOtherTimezone(res1.CreatedAt, res1.CreatedAt.TimeOfDay, TimezoneINformation).Item1.ToString("MM/dd/yyyy");
                    res1.CreatedAtTime = UtilityRepository.UtcDateToOtherTimezone(res1.CreatedAt, res1.CreatedAt.TimeOfDay, TimezoneINformation).Item2.Substring(0, 2) + ":" + UtilityRepository.UtcDateToOtherTimezone(res1.CreatedAt, res1.CreatedAt.TimeOfDay, TimezoneINformation).Item2.Substring(2, 2);
                    res1.UpdatedAtTime = UtilityRepository.UtcDateToOtherTimezone(res1.CreatedAt, res1.CreatedAt.TimeOfDay, TimezoneINformation).Item2.Substring(0, 2) + ":" + UtilityRepository.UtcDateToOtherTimezone(res1.CreatedAt, res1.CreatedAt.TimeOfDay, TimezoneINformation).Item2.Substring(2, 2);
                    foreach (var res2 in res1.TrackingDetails)
                    {
                        res2.Time = UtilityRepository.UtcDateToOtherTimezone(res2.Date, res2.Date.TimeOfDay, TimezoneINformation).Item2.Substring(0, 2) + ":" + UtilityRepository.UtcDateToOtherTimezone(res2.Date, res2.Date.TimeOfDay, TimezoneINformation).Item2.Substring(2, 2);
                        res2.Date = UtilityRepository.UtcDateToOtherTimezone(res2.Date, res2.Date.TimeOfDay, TimezoneINformation).Item1.Date;
                    }
                }
                List<ShipmentTracing> ShipmentTracking = new List<ShipmentTracing>();

                foreach (var res2 in FST.Tracking)
                {
                    if (res2.StatusId == 41 || res2.StatusId == 44)
                    {
                        var tracking = new AftershipTrackingRepository().GetExpressTracking(res2.Carrier, res2.ExpressTrackingNumber);
                        if (tracking != null && tracking.Tracking.FirstOrDefault().TrackingDetails.Count > 0)
                        {
                            if (tracking.Tracking.FirstOrDefault().TrackingDetails.Count > 2)
                            {
                                tracking.Tracking.FirstOrDefault().TrackingDetails.Reverse();
                                var tkng = tracking.Tracking.FirstOrDefault().TrackingDetails.Skip(2);
                                tkng.Reverse();
                                res2.TrackingDetails.AddRange(tkng);
                            }
                        }
                    }
                }
                return FST;
            }
            catch (Exception e)
            {
                FST.Status = false;
                return FST;
            }
        }

        public FrayteShipmentTracking GetExpressBagTracking(int BagId, string TrackingType)
        {
            FrayteShipmentTracking FST = new FrayteShipmentTracking();
            try
            {
                if (TrackingType == "Internal")
                {
                    FST = (from Bg in dbContext.ExpressBags
                           let ESHIP = Bg.BagId
                           join ES in dbContext.Expresses on Bg.BagId equals ES.BagId into ESS
                           from ES in ESS.DefaultIfEmpty()
                           let Sid = ES.ShipmentStatusId
                           join SS in dbContext.ShipmentStatus on ES.ShipmentStatusId equals SS.ShipmentStatusId into SSS
                           from SS in SSS.DefaultIfEmpty()
                           join ESA in dbContext.ExpressAddresses on ES.FromAddressId equals ESA.ExpressAddressId into ESAA
                           from ESA in ESAA.DefaultIfEmpty()
                           join EST in dbContext.ExpressAddresses on ES.ToAddressId equals EST.ExpressAddressId into ESST
                           from EST in ESST.DefaultIfEmpty()
                           join HC in dbContext.HubCarriers on Bg.HubCarrierId equals HC.HubCarrierId into HCC
                           from HC in HCC.DefaultIfEmpty()
                           where Bg.BagId == BagId
                           select new FrayteShipmentTracking
                           {
                               Status = true,
                               Tracking = new List<ShipmentTracking>()
                              {
                                  new ShipmentTracking()
                                  {
                                      TrackingNumber = Bg.BagBarCode,
                                      CreatedAt = Bg.CreatedOn != null ? Bg.CreatedOn.Value : DateTime.MinValue,
                                      CreatedAtDate = Bg.CreatedOn != null ? Bg.CreatedOn.ToString() : "",
                                      UpdatedAtDate = Bg.CreatedOn != null ? Bg.CreatedOn.ToString() : "",
                                      UpdatedAt = Bg.CreatedOn != null ? Bg.CreatedOn.Value : DateTime.MinValue,
                                      Status = SS.StatusName,
                                      StatusDisplay = SS.DisplayStatusName,
                                      Carrier = HC.Carrier != null ? HC.Carrier : "",
                                      EstimatedWeight = Sid != 37 ? (double)dbContext.Expresses.Where(a => a.BagId == ESHIP).FirstOrDefault().ActualWeight : 0,
                                      NoOfPieces = Sid != 37 ? dbContext.Expresses.Where(a => a.BagId == ESHIP).Count() : 0,
                                      TrackingDetails = (from fr in dbContext.BagManifestTrackings
                                                         join MUC in dbContext.MobileUserConfigurations on fr.MobileEventConfigurationId equals MUC.MasterTrackingDetailId
                                                         where fr.BagId == ESHIP
                                                         && fr.CreatedBy == MUC.UserId
                                                         select new ShipmentTrackingDetail
                                                         {
                                                            Activity = MUC.EventMessage,
                                                            Location = fr.Country != null && fr.City != null ? fr.City.ToUpper() + "-" + fr.Country.ToUpper() : "",
                                                            Date = fr.CreatedOnUtc != null ? fr.CreatedOnUtc : DateTime.MinValue,
                                                            Time = ""
                                                         }).ToList(),
                                     IsHeaderShow = true,
                                     ShowHideValue = "Hide",
                                     IsPanelShow = true,
                                     IsShowHideDetail = false
                                }
                            }
                           }).FirstOrDefault();
                }
                else
                {
                    FST = (from Bg in dbContext.ExpressBags
                           let ESHIP = Bg.BagId
                           join ES in dbContext.Expresses on Bg.BagId equals ES.BagId into ESS
                           from ES in ESS.DefaultIfEmpty()
                           let Sid = ES.ShipmentStatusId
                           join SS in dbContext.ShipmentStatus on ES.ShipmentStatusId equals SS.ShipmentStatusId into SSS
                           from SS in SSS.DefaultIfEmpty()
                           join ESA in dbContext.ExpressAddresses on ES.FromAddressId equals ESA.ExpressAddressId into ESAA
                           from ESA in ESAA.DefaultIfEmpty()
                           join EST in dbContext.ExpressAddresses on ES.ToAddressId equals EST.ExpressAddressId into ESST
                           from EST in ESST.DefaultIfEmpty()
                           join HC in dbContext.HubCarriers on Bg.HubCarrierId equals HC.HubCarrierId into HCC
                           from HC in HCC.DefaultIfEmpty()
                           where Bg.BagId == BagId
                           select new FrayteShipmentTracking
                           {
                               Status = true,
                               Tracking = new List<ShipmentTracking>()
                              {
                                  new ShipmentTracking()
                                  {
                                      TrackingNumber = Bg.BagBarCode,
                                      CreatedAt = Bg.CreatedOn != null ? Bg.CreatedOn.Value : DateTime.MinValue,
                                      CreatedAtDate = Bg.CreatedOn != null ? Bg.CreatedOn.ToString() : "",
                                      UpdatedAtDate = Bg.CreatedOn != null ? Bg.CreatedOn.ToString() : "",
                                      UpdatedAt = Bg.CreatedOn != null ? Bg.CreatedOn.Value : DateTime.MinValue,
                                      Status = SS.StatusName,
                                      StatusDisplay = SS.DisplayStatusName,
                                      Carrier = HC.Carrier != null ? HC.Carrier : "",
                                      EstimatedWeight = Sid != 37 ? (double)dbContext.Expresses.Where(a => a.BagId == ESHIP).FirstOrDefault().ActualWeight : 0,
                                      NoOfPieces = Sid != 37 ? dbContext.Expresses.Where(a => a.BagId == ESHIP).Count() : 0,
                                      TrackingDetails = (from fr in dbContext.BagManifestTrackings
                                                         join MUC in dbContext.MobileUserConfigurations on fr.MobileEventConfigurationId equals MUC.MasterTrackingDetailId
                                                         where fr.BagId == ESHIP
                                                         && fr.CreatedBy == MUC.UserId
                                                         && MUC.IsExternal == true
                                                         select new ShipmentTrackingDetail
                                                         {
                                                            Activity = MUC.EventMessage,
                                                            Location = fr.Country != null && fr.City != null ? fr.City.ToUpper() + "-" + fr.Country.ToUpper() : "",
                                                            Date = fr.CreatedOnUtc != null ? fr.CreatedOnUtc : DateTime.MinValue,
                                                            Time = ""
                                                         }).ToList(),
                                     IsHeaderShow = true,
                                     ShowHideValue = "Hide",
                                     IsPanelShow = true,
                                     IsShowHideDetail = false
                                    }
                                }
                           }).FirstOrDefault();
                }

                var res = (from Bg in dbContext.ExpressBags
                           join ES in dbContext.Expresses on Bg.BagId equals ES.BagId
                           join ESA in dbContext.ExpressAddresses on ES.FromAddressId equals ESA.ExpressAddressId
                           where Bg.BagId == BagId
                           select new
                           {
                               ESA.ExpressAddressId,
                               ESA.CountryId
                           }).FirstOrDefault();

                var TZ = new DirectShipmentRepository().TimeZoneDetail(res.CountryId);
                var TimezoneINformation = TimeZoneInfo.FindSystemTimeZoneById(TZ.Name);
                foreach (var res1 in FST.Tracking)
                {
                    res1.CreatedAtDate = UtilityRepository.UtcDateToOtherTimezone(res1.CreatedAt, res1.CreatedAt.TimeOfDay, TimezoneINformation).Item1.ToString("MM/dd/yyyy");
                    res1.UpdatedAtDate = UtilityRepository.UtcDateToOtherTimezone(res1.CreatedAt, res1.CreatedAt.TimeOfDay, TimezoneINformation).Item1.ToString("MM/dd/yyyy");
                    res1.CreatedAtTime = UtilityRepository.UtcDateToOtherTimezone(res1.CreatedAt, res1.CreatedAt.TimeOfDay, TimezoneINformation).Item2.Substring(0, 2) + ":" + UtilityRepository.UtcDateToOtherTimezone(res1.CreatedAt, res1.CreatedAt.TimeOfDay, TimezoneINformation).Item2.Substring(2, 2);
                    res1.UpdatedAtTime = UtilityRepository.UtcDateToOtherTimezone(res1.CreatedAt, res1.CreatedAt.TimeOfDay, TimezoneINformation).Item2.Substring(0, 2) + ":" + UtilityRepository.UtcDateToOtherTimezone(res1.CreatedAt, res1.CreatedAt.TimeOfDay, TimezoneINformation).Item2.Substring(2, 2);
                    foreach (var res2 in res1.TrackingDetails)
                    {
                        res2.Time = UtilityRepository.UtcDateToOtherTimezone(res2.Date, res2.Date.TimeOfDay, TimezoneINformation).Item2.Substring(0, 2) + ":" + UtilityRepository.UtcDateToOtherTimezone(res2.Date, res2.Date.TimeOfDay, TimezoneINformation).Item2.Substring(2, 2);
                        res2.Date = UtilityRepository.UtcDateToOtherTimezone(res2.Date, res2.Date.TimeOfDay, TimezoneINformation).Item1.Date;
                    }
                }
                return FST;
            }
            catch (Exception e)
            {
                FST.Status = false;
                return FST;
            }
        }

        public int GetExpressTracking(string Number)
        {
            var Id = 0;
            if (!string.IsNullOrEmpty(Number))
            {
                if (Number.Replace(" ", "").Contains("BGL"))
                {
                    Id = dbContext.ExpressBags.Where(a => a.BagBarCode == Number).FirstOrDefault().BagId;
                }
                else if (Number.Replace(" ", "").Count() > 10)
                {
                    var AirlineCode = Number.Replace(" ", "").Substring(0, 3);
                    var Mawb = Number.Replace(" ", "").Substring(3, 8);
                    var AI = dbContext.Airlines.Where(b => b.AirlineCode == AirlineCode).FirstOrDefault().AirlineId;
                    Id = dbContext.TradelaneShipments.Where(a => a.MAWB == Mawb && a.AirlineId == AI).FirstOrDefault().TradelaneShipmentId;
                }
            }
            return Id;
        }

        public Tuple<string, string> GetMawbStr(string Mawb)
        {
            var Airline = "";
            if (Mawb != null && Mawb != "")
            {
                if (Mawb.Length < 10)
                {
                    if (Mawb.Length == 8)
                    {
                        char[] charArray = Mawb.ToCharArray();
                        Array.Reverse(charArray);
                        Mawb = new string(charArray);
                        Mawb = (string)Mawb.Substring(0, 8);
                        char[] charArray1 = Mawb.ToCharArray();
                        Array.Reverse(charArray1);
                        Mawb = new string(charArray1);
                    }
                    if (Mawb.Length == 9 && Mawb.Contains('-'))
                    {
                        var MawbAry = Mawb.Split('-');
                        if (MawbAry[0].Length == 4 && MawbAry[1].Length == 4)
                        {
                            Mawb = Mawb.Trim().Replace("-", "");
                            char[] charArray = Mawb.ToCharArray();
                            Array.Reverse(charArray);
                            Mawb = new string(charArray);
                            Mawb = (string)Mawb.Substring(0, 8);
                            char[] charArray1 = Mawb.ToCharArray();
                            Array.Reverse(charArray1);
                            Mawb = new string(charArray1);
                        }
                    }
                    if (Mawb.Length == 9 && Mawb.Contains(' '))
                    {
                        var MawbAry = Mawb.Split(' ');
                        if (MawbAry[0].Length == 4 && MawbAry[1].Length == 4)
                        {
                            Mawb = Mawb.Trim().Replace(" ", "");
                            char[] charArray = Mawb.ToCharArray();
                            Array.Reverse(charArray);
                            Mawb = new string(charArray);
                            Mawb = (string)Mawb.Substring(0, 8);
                            char[] charArray1 = Mawb.ToCharArray();
                            Array.Reverse(charArray1);
                            Mawb = new string(charArray1);
                        }
                    }
                }
                else if (Mawb.Length > 10)
                {
                    Mawb = Mawb.Trim().Replace(" ", "");
                    if (Mawb.Length == 11)
                    {
                        Airline = Mawb.Substring(0, 3);
                        char[] charArray = Mawb.ToCharArray();
                        Array.Reverse(charArray);
                        Mawb = new string(charArray);
                        Mawb = (string)Mawb.Substring(0, 8);
                        char[] charArray1 = Mawb.ToCharArray();
                        Array.Reverse(charArray1);
                        Mawb = new string(charArray1);
                    }
                    else if (Mawb.Length == 12 && Mawb.Contains('-'))
                    {
                        var MawbAry = Mawb.Split('-');
                        Mawb = Mawb.Trim().Replace("-", "");
                        Airline = Mawb.Substring(0, 3);
                        if (MawbAry[0].Length == 3)
                        {
                            char[] charArray = Mawb.ToCharArray();
                            Array.Reverse(charArray);
                            Mawb = new string(charArray);
                            Mawb = (string)Mawb.Substring(0, 8);
                            char[] charArray1 = Mawb.ToCharArray();
                            Array.Reverse(charArray1);
                            Mawb = new string(charArray1);
                        }
                    }
                    else if (Mawb.Length == 13 && Mawb.Contains('-'))
                    {
                        var MawbAry = Mawb.Split('-');
                        Mawb = Mawb.Trim().Replace("-", "");
                        Airline = Mawb.Substring(0, 3);
                        if (MawbAry[0].Length == 3 && MawbAry[1].Length == 4 && MawbAry[2].Length == 4)
                        {
                            char[] charArray = Mawb.ToCharArray();
                            Array.Reverse(charArray);
                            Mawb = new string(charArray);
                            Mawb = (string)Mawb.Substring(0, 8);
                            char[] charArray1 = Mawb.ToCharArray();
                            Array.Reverse(charArray1);
                            Mawb = new string(charArray1);
                        }
                    }
                }
            }
            return Tuple.Create(Mawb, Airline);
        }

        public string SendExpressConsolidatedMail()
        {
            try
            {
                // To Do : customer should come according to moduleType
                var operationzone = UtilityRepository.GetOperationZone();
                var result = (from r in dbContext.Users
                              join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                              join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                              where
                                 r.IsActive == true &&
                                 ua.IsExpressSolutions == true &&
                                 r.OperationZoneId == operationzone.OperationZoneId

                              select new
                              {
                                  r.UserId,
                                  r.ContactName,
                                  r.CompanyName,
                                  ua.AccountNo,
                                  r.UserEmail,
                                  ua.DaysValidity,
                                  ua.CreditLimitCurrencyCode,
                                  r.OperationZoneId,
                                  r.WorkingStartTime,
                                  r.WorkingEndTime,
                                  r.WorkingWeekDayId

                              }).ToList();

                if (result.Count > 0)
                {
                    foreach (var res in result)
                    {
                        var date = DateTime.UtcNow.Date;

                        var BeforeTwoDay = DateTime.UtcNow.AddDays(-2).Date;
                        var startdate = new DateTime(date.Year, date.Month, date.Day, res.WorkingStartTime.Value.Hours, res.WorkingStartTime.Value.Minutes, res.WorkingStartTime.Value.Seconds, DateTimeKind.Utc);
                        //startdate = startdate.AddHours(-1);
                        var EmailSentStatus = dbContext.ExpressSchedulerEmails.Where(a => a.CustomerId == res.UserId && a.EmailSentOn == date).FirstOrDefault();

                        if (EmailSentStatus == null)
                        {
                            try
                            {
                                //Send Consolidate mail of shipment Booked
                                ExpressConsolidateMailShipmentBooked(res.UserId, date, startdate);
                            }
                            catch (Exception ex)
                            {
                                ExpressSchedulerEmail ExpSchEm1 = new ExpressSchedulerEmail();
                                ExpSchEm1.CustomerId = res.UserId;
                                ExpSchEm1.EmailSentOn = date;
                                ExpSchEm1.ErrorObject = ex.InnerException != null ? ex.InnerException.ToString() : "";
                                dbContext.ExpressSchedulerEmails.Add(ExpSchEm1);
                                dbContext.SaveChanges();
                            }

                            try
                            {
                                //Send Mail of Delivered Shipments
                                ExpressConsolildateMailDelivered(res.UserId, date, startdate);
                            }
                            catch (Exception ex)
                            {
                                ExpressSchedulerEmail ExpSchEm2 = new ExpressSchedulerEmail();
                                ExpSchEm2.CustomerId = res.UserId;
                                ExpSchEm2.EmailSentOn = date;
                                ExpSchEm2.ErrorObject = ex.InnerException != null ? ex.InnerException.ToString() : "";
                                dbContext.ExpressSchedulerEmails.Add(ExpSchEm2);
                                dbContext.SaveChanges();
                            }

                            try
                            {
                                //Send Mail of Dispatched Shipments
                                ExpressConsolidateMailDispatched(res.UserId, date, startdate);
                            }
                            catch (Exception ex)
                            {
                                ExpressSchedulerEmail ExpSchEm3 = new ExpressSchedulerEmail();
                                ExpSchEm3.CustomerId = res.UserId;
                                ExpSchEm3.EmailSentOn = date;
                                ExpSchEm3.ErrorObject = ex.InnerException != null ? ex.InnerException.ToString() : "";
                                dbContext.ExpressSchedulerEmails.Add(ExpSchEm3);
                                dbContext.SaveChanges();
                            }

                            try
                            {
                                //Send Mail of Dispatched Shipments
                                ExpressConsolidateMailMissing(res.UserId, date, startdate);
                            }
                            catch (Exception ex)
                            {
                                ExpressSchedulerEmail ExpSchEm3 = new ExpressSchedulerEmail();
                                ExpSchEm3.CustomerId = res.UserId;
                                ExpSchEm3.EmailSentOn = date;
                                ExpSchEm3.ErrorObject = ex.InnerException != null ? ex.InnerException.ToString() : "";
                                dbContext.ExpressSchedulerEmails.Add(ExpSchEm3);
                                dbContext.SaveChanges();
                            }

                            //ExpressSchedulerEmail ExpSchEm = new ExpressSchedulerEmail();
                            //ExpSchEm.CustomerId = res.UserId;
                            //ExpSchEm.EmailSentOn = date;
                            //dbContext.ExpressSchedulerEmails.Add(ExpSchEm);
                            //dbContext.SaveChanges();
                        }

                        if (EmailSentStatus == null)
                        {
                            //Send Weekly Mawb Mail
                            try
                            {
                                ExpressWeeklyMawbMail(res.UserId, date, startdate);
                            }
                            catch (Exception Ex)
                            {
                                Console.Write(Ex.Message);
                            }
                        }
                    }
                }
                return "";
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public string SendExpressPODConsolidatedMail()
        {
            try
            {
                var operationzone = UtilityRepository.GetOperationZone();
                var result = (from u in dbContext.Users
                              join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                              join e in dbContext.Expresses on u.UserId equals e.CustomerId
                              where
                                 u.IsActive == true &&
                                 ua.IsExpressSolutions == true &&
                                 e.PODMailSentOn == null &&
                                 u.OperationZoneId == operationzone.OperationZoneId
                              select new
                              {
                                  u.UserId,
                                  u.UserEmail,
                                  u.OperationZoneId

                              }).ToList();

                if (result.Count > 0)
                {
                    foreach (var res in result)
                    {
                        var date = DateTime.UtcNow.Date;
                        try
                        {
                            SendExpressPODMailConsolidated(res.UserId);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
                return "";
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        public void SendExpressPODMailConsolidated(int userId)
        {
            List<ExpressDispatchedMawbModel> EDMList = new List<ExpressDispatchedMawbModel>();

            var Mawbs = (from TL in dbContext.TradelaneShipments
                         let TradelaneShipmentId = TL.TradelaneShipmentId
                         join MawbAl in dbContext.TradelaneShipmentAllocations on TL.TradelaneShipmentId equals MawbAl.TradelaneShipmentId
                         join Al in dbContext.Airlines on TL.AirlineId equals Al.AirlineId
                         join Ar in dbContext.Airports on TL.DestinationAirportCode equals Ar.AirportCode
                         join Cn in dbContext.Countries on Ar.CountryId equals Cn.CountryId
                         where
                         TL.CustomerId == userId

                         select new ExpressDispatchedMawbModel()
                         {
                             CreatedOn = TL.CreatedOnUtc,
                             TradelaneShipmentId = TL.TradelaneShipmentId,
                             FlightNo = MawbAl.FlightNumber,
                             ShipTo = Cn.CountryName + " (" + TL.DestinationAirportCode + ")",
                             Mawb = Al.AirlineCode + " " + TL.MAWB.Substring(0, 4) + " " + TL.MAWB.Substring(4, 4),
                             GrossWeight = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).Sum(a => a.CartonValue * a.Weight),
                             TotalCarton = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).Sum(a => a.CartonValue)

                         }).ToList();

            var DistictMawbRecs = Mawbs.GroupBy(a => a.Mawb);

            foreach (var MawbRes in DistictMawbRecs)
            {
                ExpressDispatchedMawbModel EDMM = new ExpressDispatchedMawbModel();
                EDMM.Mawb = MawbRes.FirstOrDefault().Mawb;
                EDMM.TradelaneShipmentId = MawbRes.FirstOrDefault().TradelaneShipmentId;
                EDMM.CreatedOn = MawbRes.FirstOrDefault().CreatedOn;
                EDMM.CreatedOnTime = MawbRes.FirstOrDefault().CreatedOn.ToString("dd-MMM-yyyy");
                EDMM.ShipTo = MawbRes.FirstOrDefault().ShipTo;
                EDMM.FlightNo = MawbRes.FirstOrDefault().FlightNo;
                EDMM.GrossWeight = MawbRes.FirstOrDefault().GrossWeight;
                EDMM.TotalCarton = MawbRes.FirstOrDefault().TotalCarton;
                EDMList.Add(EDMM);
            }

            List<ExpressGetShipmentModel> Shipments = new List<ExpressGetShipmentModel>();
            List<ExpressShipmentBookedConsolidateModel> ESBList = new List<ExpressShipmentBookedConsolidateModel>();

            foreach (var EDM in EDMList)
            {
                Shipments = (from Mn in dbContext.ExpressManifests
                             join Bg in dbContext.ExpressBags on Mn.ExpressManifestId equals Bg.ManifestId
                             join EX in dbContext.Expresses on Bg.BagId equals EX.BagId
                             join Usr in dbContext.Users on EX.CustomerId equals Usr.UserId
                             join HCS in dbContext.HubCarrierServices on EX.HubCarrierServiceId equals HCS.HubCarrierServiceId
                             join HC in dbContext.HubCarriers on HCS.HubCarrierId equals HC.HubCarrierId
                             join Hu in dbContext.Hubs on HC.HubId equals Hu.HubId
                             join EXFA in dbContext.ExpressAddresses on EX.FromAddressId equals EXFA.ExpressAddressId
                             join EXTA in dbContext.ExpressAddresses on EX.ToAddressId equals EXTA.ExpressAddressId
                             where Mn.TradelaneShipmentId == EDM.TradelaneShipmentId
                                   && EX.PODMailSentOn == null

                             select new ExpressGetShipmentModel()
                             {
                                 ExpressShipmentId = EX.ExpressId,
                                 MAWB = EDM.Mawb,
                                 AWBNumber = EX.AWBBarcode,
                                 CreatedOn = EX.CreatedOnUtc,
                                 HubCode = Hu.Code,
                                 Shipper = EXFA.ContactFirstName + " " + EXFA.ContactLastName,
                                 Receiver = EXTA.ContactFirstName + " " + EXTA.ContactLastName,
                                 Customer = Usr.UserName,
                                 Carrier = HC.Carrier,
                                 CustomerId = EX.CustomerId,
                                 TotalCarton = dbContext.ExpressDetails.Where(a => a.ExpressId == EX.ExpressId).Count(),
                                 TotalWeight = dbContext.ExpressDetails.Where(a => a.ExpressId == EX.ExpressId).Sum(ab => ab.Weight)

                             }).ToList();

                ExpressShipmentBookedConsolidateModel ESB = new ExpressShipmentBookedConsolidateModel();
                ESB.Shipments = new List<ExpressGetShipmentModel>();

                var Result = Shipments.GroupBy(a => a.CustomerId);

                foreach (var res1 in Result)
                {
                    ExpressGetShipmentModel EGSM = new ExpressGetShipmentModel();
                    ESB.Mawb = EDM.Mawb;
                    ESB.HubCode = res1.FirstOrDefault().HubCode;
                    EGSM.AWBNumber = res1.FirstOrDefault().AWBNumber;
                    EGSM.Shipper = res1.FirstOrDefault().Shipper;
                    EGSM.Receiver = res1.FirstOrDefault().Receiver;
                    EGSM.CreatedOn = res1.FirstOrDefault().CreatedOn;
                    EGSM.CreatedOnTime = res1.FirstOrDefault().CreatedOn.ToString("dd-MMM-yyyy");
                    EGSM.TotalWeight = res1.FirstOrDefault().TotalWeight;
                    ESB.Shipments.Add(EGSM);
                }
                if (ESB.Shipments.Count > 0)
                {
                    ESBList.Add(ESB);
                }
            }
            //Send Dispatched Mails
            if (EDMList.Count() > 0 && ESBList.Count() > 0)
            {
                new ExpressEmailRepository().SendPODAWBsMail(EDMList, ESBList, userId);
            }
        }

        public List<ExpressDownloadDriverIds> DownloadDriverManifest()
        {
            List<ExpressDownloadDriverIds> EDList = new List<ExpressDownloadDriverIds>();
            var _date = DateTime.UtcNow.Date;
            var _PerviousDate = DateTime.UtcNow.Date.AddDays(-1).Date;

            //var Manifests = dbContext.ExpressManifests.Where(a => a.CreatedOn.Value > _PerviousDate && a.CreatedOn < _date).ToList();
            var Manifests = dbContext.ExpressManifests.Where(a => a.ExpressManifestId == 8).ToList();

            if (Manifests != null && Manifests.Count > 0)
            {

                foreach (var Manifest in Manifests)
                {
                    ExpressDownloadDriverIds EDD = new ExpressDownloadDriverIds();
                    try
                    {
                        var Result = dbContext.TradelaneShipmentDocuments.Where(a => a.TradelaneShipmentId == Manifest.TradelaneShipmentId && a.DocumentType == FrayteTradelaneShipmentDocumentEnum.DriverManifest).FirstOrDefault();
                        if (Result == null)
                        {
                            EDD.TradelaneShipmentId = Manifest.TradelaneShipmentId.Value;
                            EDD.UserId = Manifest.CustomerId.Value;
                            EDList.Add(EDD);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return EDList;
        }

        public void ExpressConsolidateMailShipmentBooked(int UserId, DateTime date, DateTime startdate)
        {
            List<ExpressGetShipmentModel> Shipments = new List<ExpressGetShipmentModel>();
            var BeforeTwoDay = DateTime.UtcNow.AddDays(-1).Date.AddMinutes(-1);
            var CurrentDate = DateTime.UtcNow;
            var beforeoneday = DateTime.UtcNow.Date.AddMinutes(-1);

            if (CurrentDate.TimeOfDay.Hours == startdate.TimeOfDay.Hours)
            {
                //join EXD in dbContext.ExpressDetails on EX.ExpressId equals EXD.ExpressId
                Shipments = (from EX in dbContext.Expresses
                             join Usr in dbContext.Users on EX.CustomerId equals Usr.UserId
                             join HCS in dbContext.HubCarrierServices on EX.HubCarrierServiceId equals HCS.HubCarrierServiceId
                             join HC in dbContext.HubCarriers on HCS.HubCarrierId equals HC.HubCarrierId
                             join Hu in dbContext.Hubs on HC.HubId equals Hu.HubId
                             join EXFA in dbContext.ExpressAddresses on EX.FromAddressId equals EXFA.ExpressAddressId
                             join EXTA in dbContext.ExpressAddresses on EX.ToAddressId equals EXTA.ExpressAddressId
                             where EX.CustomerId == UserId
                             && EX.ShipmentStatusId == 38
                             && EX.CreatedOnUtc <= beforeoneday && EX.CreatedOnUtc > BeforeTwoDay

                             select new ExpressGetShipmentModel()
                             {
                                 ExpressShipmentId = EX.ExpressId,
                                 AWBNumber = EX.AWBBarcode,
                                 CreatedOn = EX.CreatedOnUtc,
                                 HubCode = Hu.Code,
                                 Shipper = EXFA.ContactFirstName + " " + EXFA.ContactLastName,
                                 Receiver = EXTA.ContactFirstName + " " + EXTA.ContactLastName,
                                 Customer = Usr.UserName,
                                 Carrier = HC.Carrier,
                                 TotalCarton = dbContext.ExpressDetails.Where(a => a.ExpressId == EX.ExpressId).Count(),
                                 TotalWeight = dbContext.ExpressDetails.Where(a => a.ExpressId == EX.ExpressId).Sum(ab => ab.Weight)

                             }).ToList();

                List<ExpressShipmentBookedConsolidateModel> ESBList = new List<ExpressShipmentBookedConsolidateModel>();

                var Result = Shipments.GroupBy(a => a.HubCode);

                foreach (var res1 in Result)
                {
                    ExpressShipmentBookedConsolidateModel ESB = new ExpressShipmentBookedConsolidateModel();
                    ESB.Shipments = new List<ExpressGetShipmentModel>();
                    ESB.HubCode = res1.Key;

                    foreach (var r in res1)
                    {
                        ExpressGetShipmentModel EGSM = new ExpressGetShipmentModel();
                        EGSM.AWBNumber = r.AWBNumber;
                        EGSM.Shipper = r.Shipper;
                        EGSM.Receiver = r.Receiver;
                        EGSM.CreatedOn = r.CreatedOn;
                        EGSM.CreatedOnTime = r.CreatedOn.ToString("dd-MMM-yyyy");
                        EGSM.TotalWeight = r.TotalWeight;
                        ESB.Shipments.Add(EGSM);
                    }

                    ESBList.Add(ESB);
                }

                if (Result.Count() > 0)
                {
                    new ExpressEmailRepository().SendAWBsMail(ESBList, UserId);
                }
            }
            else
            {

            }
        }

        public void ExpressConsolildateMailDelivered(int UserId, DateTime date, DateTime startdate)
        {
            List<BagDetail> Bags = new List<BagDetail>();
            var BeforeTwoDay = DateTime.UtcNow.AddDays(-1).Date.AddMinutes(-1);
            var CurrentDate = DateTime.UtcNow;
            var CD = DateTime.UtcNow.Date;
            var beforeoneday = DateTime.UtcNow.Date.AddMinutes(-1);
            if (CurrentDate.TimeOfDay.Hours == startdate.TimeOfDay.Hours)
            {
                //join EXD in dbContext.ExpressDetails on EX.ExpressId equals EXD.ExpressId
                Bags = (from EX in dbContext.Expresses
                        join Usr in dbContext.Users on EX.CustomerId equals Usr.UserId
                        join HCS in dbContext.HubCarrierServices on EX.HubCarrierServiceId equals HCS.HubCarrierServiceId
                        join HC in dbContext.HubCarriers on HCS.HubCarrierId equals HC.HubCarrierId
                        join Hu in dbContext.Hubs on HC.HubId equals Hu.HubId
                        join EXFA in dbContext.ExpressAddresses on EX.FromAddressId equals EXFA.ExpressAddressId
                        join EXTA in dbContext.ExpressAddresses on EX.ToAddressId equals EXTA.ExpressAddressId
                        join EBG in dbContext.ExpressBags on EX.BagId equals EBG.BagId
                        join EXM in dbContext.ExpressManifests on EBG.ManifestId equals EXM.ExpressManifestId
                        join TL in dbContext.TradelaneShipments on EXM.TradelaneShipmentId equals TL.TradelaneShipmentId
                        join AL in dbContext.Airlines on TL.AirlineId equals AL.AirlineId
                        where EX.CustomerId == UserId
                        && EX.ShipmentStatusId == 41
                        && EX.CreatedOnUtc < beforeoneday && EX.CreatedOnUtc > BeforeTwoDay
                        //&& EX.CreatedOnUtc < CD && EX.CreatedOnUtc > BeforeTwoDay
                        select new BagDetail()
                        {
                            HubCode = Hu.Code,
                            Mawb = AL.AirlineCode + " " + TL.MAWB.Substring(0, 4) + " " + TL.MAWB.Substring(4, 4),

                            TotalWeight = (from EX in dbContext.Expresses
                                           join EBD in dbContext.ExpressDetails on EX.ExpressId equals EBD.ExpressId
                                           where EX.BagId == EBG.BagId
                                           select EBD).Sum(a => a.Weight),

                            BagNumber = EBG.BagBarCode,
                            TotalShipments = dbContext.Expresses.Where(a => a.BagId == EX.BagId).ToList().Count,
                            DeliveredShipments = dbContext.Expresses.Where(a => a.BagId == EX.BagId && a.ShipmentStatusId == 41).ToList().Count,
                            Carrier = HC.Carrier

                        }).ToList();

                List<BagDetail> BDList = new List<BagDetail>();
                var Bg = Bags.GroupBy(a => a.BagNumber).ToList();

                foreach (var B in Bg)
                {
                    BagDetail BD = new BagDetail();
                    BD.BagNumber = B.FirstOrDefault().BagNumber;
                    BD.DeliveredShipments = B.FirstOrDefault().DeliveredShipments;
                    BD.TotalShipments = B.FirstOrDefault().TotalShipments;
                    BD.TotalWeight = B.FirstOrDefault().TotalWeight;
                    BD.Carrier = B.FirstOrDefault().Carrier;
                    BD.HubCode = B.FirstOrDefault().HubCode;
                    BD.Mawb = B.FirstOrDefault().Mawb;
                    BDList.Add(BD);
                }

                List<ExpressShipmentDeliveredConsolidateModel> ESBList1 = new List<ExpressShipmentDeliveredConsolidateModel>();

                var Result1 = BDList.GroupBy(a => a.HubCode);

                foreach (var res1 in Result1)
                {
                    var ESB1 = new ExpressShipmentDeliveredConsolidateModel();
                    ESB1.MAWBBags = new List<ExpressMawbShipments>();
                    ESB1.HubCode = res1.Key;
                    var Result2 = res1.GroupBy(a => a.Mawb);

                    foreach (var rs1 in Result2)
                    {
                        var EMS = new ExpressMawbShipments();
                        EMS.BagInfo = new List<BagDetail>();
                        EMS.Mawb = rs1.Key;
                        foreach (var r in rs1)
                        {
                            BagDetail EGSM = new BagDetail();
                            EGSM.BagNumber = r.BagNumber;
                            EGSM.Carrier = r.Carrier;
                            EGSM.Mawb = r.Mawb;
                            EGSM.TotalWeight = r.TotalWeight;
                            EGSM.TotalShipments = r.TotalShipments;
                            EGSM.DeliveredShipments = r.DeliveredShipments;
                            EMS.BagInfo.Add(EGSM);
                        }

                        ESB1.MAWBBags.Add(EMS);
                    }
                    ESBList1.Add(ESB1);
                }

                if (ESBList1.Count() > 0)
                {
                    new ExpressEmailRepository().SendDeliveredAWBsMail(ESBList1, UserId);
                }
            }
            else
            {

            }
        }

        public void ExpressConsolidateMailDispatched(int UserId, DateTime date, DateTime startdate)
        {
            List<ExpressDispatchedMawbModel> EDMList = new List<ExpressDispatchedMawbModel>();
            var CurrentDate = DateTime.UtcNow;
            var BeforeTwoDay = DateTime.UtcNow.AddDays(-1).Date.AddMinutes(-1);
            var beforeoneday = DateTime.UtcNow.Date.AddMinutes(-1);

            if (CurrentDate.TimeOfDay.Hours == startdate.TimeOfDay.Hours)
            {
                var Mawbs = (from TL in dbContext.TradelaneShipments
                             let TradelaneShipmentId = TL.TradelaneShipmentId
                             join EXM in dbContext.ExpressManifests on TL.TradelaneShipmentId equals EXM.TradelaneShipmentId
                             join MawbAl in dbContext.TradelaneShipmentAllocations on TL.TradelaneShipmentId equals MawbAl.TradelaneShipmentId
                             join Al in dbContext.Airlines on TL.AirlineId equals Al.AirlineId
                             join Ar in dbContext.Airports on TL.DestinationAirportCode equals Ar.AirportCode
                             join Cn in dbContext.Countries on Ar.CountryId equals Cn.CountryId
                             where
                             TL.CustomerId == UserId
                             && TL.ShipmentStatusId == 28
                             && TL.CreatedOnUtc <= beforeoneday && TL.CreatedOnUtc > BeforeTwoDay
                             select new ExpressDispatchedMawbModel()
                             {
                                 CreatedOn = TL.CreatedOnUtc,
                                 TradelaneShipmentId = TL.TradelaneShipmentId,
                                 FlightNo = MawbAl.FlightNumber,
                                 ShipTo = Cn.CountryName + " (" + TL.DestinationAirportCode + ")",
                                 Mawb = Al.AirlineCode + " " + TL.MAWB.Substring(0, 4) + " " + TL.MAWB.Substring(4, 4),
                                 GrossWeight = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).Sum(a => a.CartonValue * a.Weight),
                                 TotalCarton = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).Sum(a => a.CartonValue)

                             }).ToList();

                var DistictMawbRecs = Mawbs.GroupBy(a => a.Mawb);

                foreach (var MawbRes in DistictMawbRecs)
                {
                    ExpressDispatchedMawbModel EDMM = new ExpressDispatchedMawbModel();

                    EDMM.Mawb = MawbRes.FirstOrDefault().Mawb;
                    EDMM.TradelaneShipmentId = MawbRes.FirstOrDefault().TradelaneShipmentId;
                    EDMM.CreatedOn = MawbRes.FirstOrDefault().CreatedOn;
                    EDMM.CreatedOnTime = MawbRes.FirstOrDefault().CreatedOn.ToString("dd-MMM-yyyy");
                    EDMM.ShipTo = MawbRes.FirstOrDefault().ShipTo;
                    EDMM.FlightNo = MawbRes.FirstOrDefault().FlightNo;
                    EDMM.GrossWeight = MawbRes.FirstOrDefault().GrossWeight;
                    EDMM.TotalCarton = MawbRes.FirstOrDefault().TotalCarton;
                    EDMList.Add(EDMM);
                }

                List<ExpressGetShipmentModel> Shipments = new List<ExpressGetShipmentModel>();
                List<ExpressShipmentBookedConsolidateModel> ESBList = new List<ExpressShipmentBookedConsolidateModel>();

                foreach (var EDM in EDMList)
                {
                    Shipments = (from Mn in dbContext.ExpressManifests
                                 join Bg in dbContext.ExpressBags on Mn.ExpressManifestId equals Bg.ManifestId
                                 join EX in dbContext.Expresses on Bg.BagId equals EX.BagId
                                 join Usr in dbContext.Users on EX.CustomerId equals Usr.UserId
                                 join HCS in dbContext.HubCarrierServices on EX.HubCarrierServiceId equals HCS.HubCarrierServiceId
                                 join HC in dbContext.HubCarriers on HCS.HubCarrierId equals HC.HubCarrierId
                                 join Hu in dbContext.Hubs on HC.HubId equals Hu.HubId
                                 join EXFA in dbContext.ExpressAddresses on EX.FromAddressId equals EXFA.ExpressAddressId
                                 join EXTA in dbContext.ExpressAddresses on EX.ToAddressId equals EXTA.ExpressAddressId
                                 where Mn.TradelaneShipmentId == EDM.TradelaneShipmentId
                                 && (EX.ShipmentStatusId != 37)
                                 //&& EX.CreatedOnUtc < date && EX.CreatedOnUtc > BeforeTwoDay
                                 select new ExpressGetShipmentModel()
                                 {
                                     ExpressShipmentId = EX.ExpressId,
                                     MAWB = EDM.Mawb,
                                     AWBNumber = EX.AWBBarcode,
                                     CreatedOn = EX.CreatedOnUtc,
                                     HubCode = Hu.Code,
                                     Shipper = EXFA.ContactFirstName + " " + EXFA.ContactLastName,
                                     Receiver = EXTA.ContactFirstName + " " + EXTA.ContactLastName,
                                     Customer = Usr.UserName,
                                     Carrier = HC.Carrier,
                                     TotalCarton = dbContext.ExpressDetails.Where(a => a.ExpressId == EX.ExpressId).Count(),
                                     TotalWeight = dbContext.ExpressDetails.Where(a => a.ExpressId == EX.ExpressId).Sum(ab => ab.Weight)

                                 }).ToList();

                    ExpressShipmentBookedConsolidateModel ESB = new ExpressShipmentBookedConsolidateModel();
                    ESB.Shipments = new List<ExpressGetShipmentModel>();
                    var Result = Shipments.GroupBy(a => a.AWBNumber);

                    foreach (var res1 in Result)
                    {
                        ExpressGetShipmentModel EGSM = new ExpressGetShipmentModel();
                        ESB.Mawb = EDM.Mawb;
                        ESB.HubCode = res1.FirstOrDefault().HubCode;
                        EGSM.AWBNumber = res1.FirstOrDefault().AWBNumber;
                        EGSM.Shipper = res1.FirstOrDefault().Shipper;
                        EGSM.Receiver = res1.FirstOrDefault().Receiver;
                        EGSM.CreatedOn = res1.FirstOrDefault().CreatedOn;
                        EGSM.CreatedOnTime = res1.FirstOrDefault().CreatedOn.ToString("dd-MMM-yyyy");
                        EGSM.TotalWeight = res1.FirstOrDefault().TotalWeight;
                        ESB.Shipments.Add(EGSM);
                    }
                    if (ESB.Shipments.Count > 0)
                    {
                        ESBList.Add(ESB);
                    }
                }
                //Send Dispatched Mails
                if (EDMList.Count() > 0 && ESBList.Count() > 0)
                {
                    new ExpressEmailRepository().SendDispatchedAWBsMail(EDMList, ESBList, UserId);
                }
            }
        }

        public void ExpressConsolidateMailMissing(int UserId, DateTime date, DateTime startdate)
        {
            List<ExpressMissingShipmentModel> EDMList = new List<ExpressMissingShipmentModel>();
            var CurrentDate = DateTime.UtcNow;
            var BeforeTwoDay = DateTime.UtcNow.AddDays(-1).Date.AddMinutes(-1);
            var beforeoneday = DateTime.UtcNow.Date.AddMinutes(-1);
            if (CurrentDate.TimeOfDay.Hours == startdate.TimeOfDay.Hours)
            {
                //var Mawbs = (from TL in dbContext.TradelaneShipments
                //             join EXM in dbContext.ExpressManifests on TL.TradelaneShipmentId equals EXM.TradelaneShipmentId
                //             join Al in dbContext.Airlines on TL.AirlineId equals Al.AirlineId
                //             join Bg in dbContext.ExpressBags on EXM.ExpressManifestId equals Bg.ManifestId
                //             join BTM in dbContext.BagManifestTrackings on Bg.BagId equals BTM.BagId into BTTM
                //             from BTM in BTTM.DefaultIfEmpty()
                //             join Exp in dbContext.Expresses on Bg.BagId equals Exp.BagId
                //             join TD in dbContext.TrackingDetails on Exp.ExpressId equals TD.ShipmentId into TDM
                //             from TD in TDM.DefaultIfEmpty()
                //             let ExpId = Exp.ExpressId
                //             join FA in dbContext.ExpressAddresses on Exp.FromAddressId equals FA.ExpressAddressId
                //             join TA in dbContext.ExpressAddresses on Exp.ToAddressId equals TA.ExpressAddressId
                //             where
                //             TL.CustomerId == UserId
                //             && (TD.IsMissing == true || BTM.IsMissing == true)
                //             && TL.CreatedOnUtc < beforeoneday && TL.CreatedOnUtc > BeforeTwoDay
                //             select new ExpressMissingShipmentModel()
                //             {
                //                 CreatedOn = TL.CreatedOnUtc,
                //                 TradelaneShipmentId = TL.TradelaneShipmentId,
                //                 AWBNumber = Exp.AWBBarcode,
                //                 IsAWBMissing = TD.IsMissing.HasValue && TD.IsMissing.Value == true ? "True" : "Null",
                //                 IsBagMissing = BTM.IsMissing.HasValue && BTM.IsMissing.Value == true ? "True" : "Null",
                //                 BagNumber = Bg.BagBarCode,
                //                 ShipFromCompany = FA.CompanyName != null && FA.CompanyName != "" ? FA.CompanyName : FA.ContactFirstName + " " + FA.ContactLastName,
                //                 ShipToCompany = TA.CompanyName != null && TA.CompanyName != "" ? TA.CompanyName : TA.ContactFirstName + " " + TA.ContactLastName,
                //                 Mawb = Al.AirlineCode + " " + TL.MAWB.Substring(0, 4) + " " + TL.MAWB.Substring(4, 4),
                //                 TotalWeight = dbContext.ExpressDetails.Where(a => a.ExpressId == ExpId).Sum(a => a.CartonQty * a.Weight)
                //             }).ToList();

                var Mawbs = (from Exp in dbContext.Expresses
                             let ExpId = Exp.ExpressId
                             join Bg in dbContext.ExpressBags on Exp.BagId equals Bg.BagId into Bgd
                             from Bg in Bgd.DefaultIfEmpty()
                             join BTM in dbContext.BagManifestTrackings on Bg.BagId equals BTM.BagId into BTTM
                             from BTM in BTTM.DefaultIfEmpty()
                             join TD in dbContext.TrackingDetails on Exp.ExpressId equals TD.ShipmentId into TDM
                             from TD in TDM.DefaultIfEmpty()
                             join FA in dbContext.ExpressAddresses on Exp.FromAddressId equals FA.ExpressAddressId
                             join TA in dbContext.ExpressAddresses on Exp.ToAddressId equals TA.ExpressAddressId
                             where
                             Exp.CustomerId == UserId
                             && (TD.IsMissing == true || BTM.IsMissing == true)
                             && Exp.CreatedOnUtc < beforeoneday && Exp.CreatedOnUtc > BeforeTwoDay
                             select new ExpressMissingShipmentModel()
                             {
                                 CreatedOn = Exp.CreatedOnUtc,
                                 AWBNumber = Exp.AWBBarcode,
                                 IsAWBMissing = TD.IsMissing.HasValue && TD.IsMissing.Value == true ? "True" : "Null",
                                 IsBagMissing = BTM.IsMissing.HasValue && BTM.IsMissing.Value == true ? "True" : "Null",
                                 BagNumber = Bg.BagBarCode,
                                 ShipFromCompany = FA.CompanyName != null && FA.CompanyName != "" ? FA.CompanyName : FA.ContactFirstName + " " + FA.ContactLastName,
                                 ShipToCompany = TA.CompanyName != null && TA.CompanyName != "" ? TA.CompanyName : TA.ContactFirstName + " " + TA.ContactLastName,
                                 TotalWeight = dbContext.ExpressDetails.Where(a => a.ExpressId == ExpId).Sum(a => a.CartonQty * a.Weight)
                             }).ToList();

                var DistictMawbRecs = Mawbs.GroupBy(a => a.AWBNumber);

                foreach (var MawbRes in DistictMawbRecs)
                {
                    ExpressMissingShipmentModel EDMM = new ExpressMissingShipmentModel();

                    EDMM.Mawb = MawbRes.FirstOrDefault().Mawb;
                    EDMM.BagNumber = MawbRes.FirstOrDefault().IsBagMissing == "True" ? "None" : MawbRes.FirstOrDefault().BagNumber;
                    EDMM.TradelaneShipmentId = MawbRes.FirstOrDefault().TradelaneShipmentId;
                    EDMM.CreatedOn = MawbRes.FirstOrDefault().CreatedOn;
                    EDMM.CreatedOnTime = MawbRes.FirstOrDefault().CreatedOn.ToString("dd-MMM-yyyy");
                    EDMM.IsAWBMissing = MawbRes.FirstOrDefault().IsAWBMissing;
                    EDMM.IsBagMissing = MawbRes.FirstOrDefault().IsBagMissing;
                    EDMM.AWBNumber = MawbRes.FirstOrDefault().AWBNumber;
                    EDMM.ShipFromCompany = MawbRes.FirstOrDefault().ShipFromCompany;
                    EDMM.ShipToCompany = MawbRes.FirstOrDefault().ShipToCompany;
                    EDMM.TotalWeight = MawbRes.FirstOrDefault().TotalWeight;
                    //EDMM. = MawbRes.FirstOrDefault().TotalCarton;
                    EDMList.Add(EDMM);
                }

                List<ExpressGetShipmentModel> Shipments = new List<ExpressGetShipmentModel>();
                List<ExpressShipmentBookedConsolidateModel> ESBList = new List<ExpressShipmentBookedConsolidateModel>();

                //Send Dispatched Mails
                if (EDMList.Count() > 0)
                {
                    new ExpressEmailRepository().SendMissingAWBsMail(EDMList, UserId);
                }
            }
        }

        public void ExpressWeeklyMawbMail(int UserId, DateTime date, DateTime startdate)
        {
            List<ExpressMissingShipmentModel> EDMList = new List<ExpressMissingShipmentModel>();
            var CurrentDate = DateTime.UtcNow;
            var BeforeTwoDay = DateTime.UtcNow.AddDays(-1).Date.AddMinutes(-1);
            var beforeoneday = DateTime.UtcNow.Date.AddMinutes(-1);
            if (CurrentDate.TimeOfDay.Hours == startdate.TimeOfDay.Hours)
            {
                //var Mawbs = (from TL in dbContext.TradelaneShipments
                //             join EXM in dbContext.ExpressManifests on TL.TradelaneShipmentId equals EXM.TradelaneShipmentId
                //             join Al in dbContext.Airlines on TL.AirlineId equals Al.AirlineId
                //             join Bg in dbContext.ExpressBags on EXM.ExpressManifestId equals Bg.ManifestId
                //             join BTM in dbContext.BagManifestTrackings on Bg.BagId equals BTM.BagId into BTTM
                //             from BTM in BTTM.DefaultIfEmpty()
                //             join Exp in dbContext.Expresses on Bg.BagId equals Exp.BagId
                //             join TD in dbContext.TrackingDetails on Exp.ExpressId equals TD.ShipmentId into TDM
                //             from TD in TDM.DefaultIfEmpty()
                //             let ExpId = Exp.ExpressId
                //             join FA in dbContext.ExpressAddresses on Exp.FromAddressId equals FA.ExpressAddressId
                //             join TA in dbContext.ExpressAddresses on Exp.ToAddressId equals TA.ExpressAddressId
                //             where
                //             TL.CustomerId == UserId
                //             && (TD.IsMissing == true || BTM.IsMissing == true)
                //             && TL.CreatedOnUtc < beforeoneday && TL.CreatedOnUtc > BeforeTwoDay
                //             select new ExpressMissingShipmentModel()
                //             {
                //                 CreatedOn = TL.CreatedOnUtc,
                //                 TradelaneShipmentId = TL.TradelaneShipmentId,
                //                 AWBNumber = Exp.AWBBarcode,
                //                 IsAWBMissing = TD.IsMissing.HasValue && TD.IsMissing.Value == true ? "True" : "Null",
                //                 IsBagMissing = BTM.IsMissing.HasValue && BTM.IsMissing.Value == true ? "True" : "Null",
                //                 BagNumber = Bg.BagBarCode,
                //                 ShipFromCompany = FA.CompanyName != null && FA.CompanyName != "" ? FA.CompanyName : FA.ContactFirstName + " " + FA.ContactLastName,
                //                 ShipToCompany = TA.CompanyName != null && TA.CompanyName != "" ? TA.CompanyName : TA.ContactFirstName + " " + TA.ContactLastName,
                //                 Mawb = Al.AirlineCode + " " + TL.MAWB.Substring(0, 4) + " " + TL.MAWB.Substring(4, 4),
                //                 TotalWeight = dbContext.ExpressDetails.Where(a => a.ExpressId == ExpId).Sum(a => a.CartonQty * a.Weight)
                //             }).ToList();

                var Mawbs = (from Exp in dbContext.Expresses
                             let ExpId = Exp.ExpressId
                             join Bg in dbContext.ExpressBags on Exp.BagId equals Bg.BagId into Bgd
                             from Bg in Bgd.DefaultIfEmpty()
                             join BTM in dbContext.BagManifestTrackings on Bg.BagId equals BTM.BagId into BTTM
                             from BTM in BTTM.DefaultIfEmpty()
                             join TD in dbContext.TrackingDetails on Exp.ExpressId equals TD.ShipmentId into TDM
                             from TD in TDM.DefaultIfEmpty()
                             join FA in dbContext.ExpressAddresses on Exp.FromAddressId equals FA.ExpressAddressId
                             join TA in dbContext.ExpressAddresses on Exp.ToAddressId equals TA.ExpressAddressId
                             where
                             Exp.CustomerId == UserId
                             && (TD.IsMissing == true || BTM.IsMissing == true)
                             && Exp.CreatedOnUtc < beforeoneday && Exp.CreatedOnUtc > BeforeTwoDay
                             select new ExpressMissingShipmentModel()
                             {
                                 CreatedOn = Exp.CreatedOnUtc,
                                 AWBNumber = Exp.AWBBarcode,
                                 IsAWBMissing = TD.IsMissing.HasValue && TD.IsMissing.Value == true ? "True" : "Null",
                                 IsBagMissing = BTM.IsMissing.HasValue && BTM.IsMissing.Value == true ? "True" : "Null",
                                 BagNumber = Bg.BagBarCode,
                                 ShipFromCompany = FA.CompanyName != null && FA.CompanyName != "" ? FA.CompanyName : FA.ContactFirstName + " " + FA.ContactLastName,
                                 ShipToCompany = TA.CompanyName != null && TA.CompanyName != "" ? TA.CompanyName : TA.ContactFirstName + " " + TA.ContactLastName,
                                 TotalWeight = dbContext.ExpressDetails.Where(a => a.ExpressId == ExpId).Sum(a => a.CartonQty * a.Weight)
                             }).ToList();

                var DistictMawbRecs = Mawbs.GroupBy(a => a.AWBNumber);

                foreach (var MawbRes in DistictMawbRecs)
                {
                    ExpressMissingShipmentModel EDMM = new ExpressMissingShipmentModel();

                    EDMM.Mawb = MawbRes.FirstOrDefault().Mawb;
                    EDMM.BagNumber = MawbRes.FirstOrDefault().IsBagMissing == "True" ? "None" : MawbRes.FirstOrDefault().BagNumber;
                    EDMM.TradelaneShipmentId = MawbRes.FirstOrDefault().TradelaneShipmentId;
                    EDMM.CreatedOn = MawbRes.FirstOrDefault().CreatedOn;
                    EDMM.CreatedOnTime = MawbRes.FirstOrDefault().CreatedOn.ToString("dd-MMM-yyyy");
                    EDMM.IsAWBMissing = MawbRes.FirstOrDefault().IsAWBMissing;
                    EDMM.IsBagMissing = MawbRes.FirstOrDefault().IsBagMissing;
                    EDMM.AWBNumber = MawbRes.FirstOrDefault().AWBNumber;
                    EDMM.ShipFromCompany = MawbRes.FirstOrDefault().ShipFromCompany;
                    EDMM.ShipToCompany = MawbRes.FirstOrDefault().ShipToCompany;
                    EDMM.TotalWeight = MawbRes.FirstOrDefault().TotalWeight;
                    //EDMM. = MawbRes.FirstOrDefault().TotalCarton;
                    EDMList.Add(EDMM);
                }

                List<ExpressGetShipmentModel> Shipments = new List<ExpressGetShipmentModel>();
                List<ExpressShipmentBookedConsolidateModel> ESBList = new List<ExpressShipmentBookedConsolidateModel>();

                //Send Dispatched Mails
                if (EDMList.Count() > 0)
                {
                    new ExpressEmailRepository().SendMissingAWBsMail(EDMList, UserId);
                }
            }
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
                    var detail = dbContext.ExpressDetailPackageLabels.Where(p => p.TrackingNumber == package.LabelName &&
                                                                             p.PackageLabelName == null).FirstOrDefault();
                    if (detail != null)
                    {
                        if (detail.PackageLabelName == "" || detail.PackageLabelName == null)
                        {
                            detail.PackageLabelName = ImageName;
                            detail.IsDownloaded = true;
                            dbContext.SaveChanges();
                        }
                    }
                }
                else if (count > 0)
                {
                    ExpressDetailPackageLabel sph = new ExpressDetailPackageLabel();
                    sph.ExpressShipmentDetailId = DirectShipmentDetailId;
                    sph.TrackingNumber = package.LabelName;
                    sph.PackageLabelName = null;
                    sph.IsDownloaded = false;
                    dbContext.ExpressDetailPackageLabels.Add(sph);
                    dbContext.SaveChanges();
                    if (!string.IsNullOrEmpty(sph.TrackingNumber) && sph.ExpressShipmentDetailId > 0)
                    {
                        var Result = dbContext.ExpressDetails.Where(a => a.ExpressDetailId == sph.ExpressShipmentDetailId).FirstOrDefault();
                        if (Result != null)
                        {
                            SaveDirectBookingPiecesTrackingNo(sph.TrackingNumber, Result.ExpressId);
                        }
                    }
                }
            }
            else
            {
                var detail = dbContext.ExpressDetailPackageLabels.Where(p => p.TrackingNumber == package.LabelName &&
                                                                         p.ExpressShipmentDetailId == DirectShipmentDetailId).FirstOrDefault();
                if (detail != null)
                {
                    detail.PackageLabelName = ImageName;
                    detail.IsDownloaded = false;
                    dbContext.SaveChanges();
                }
                else
                {
                    ExpressDetailPackageLabel sph = new ExpressDetailPackageLabel();
                    sph.ExpressShipmentDetailId = DirectShipmentDetailId;
                    sph.TrackingNumber = package.LabelName;
                    sph.PackageLabelName = ImageName;
                    sph.IsDownloaded = false;
                    dbContext.ExpressDetailPackageLabels.Add(sph);
                    dbContext.SaveChanges();
                    if (!string.IsNullOrEmpty(sph.TrackingNumber) && sph.ExpressShipmentDetailId > 0)
                    {
                        var Result = dbContext.ExpressDetails.Where(a => a.ExpressDetailId == sph.ExpressShipmentDetailId).FirstOrDefault();
                        if (Result != null)
                        {
                            SaveDirectBookingPiecesTrackingNo(sph.TrackingNumber, Result.ExpressId);
                        }
                    }
                }
            }
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
    }
}
