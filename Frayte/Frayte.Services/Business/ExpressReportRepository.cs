using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models.Express;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.Tradelane;
using Frayte.Services.Models.FrayteMAWB;

namespace Frayte.Services.Business
{
    public class ExpressReportRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<ExpressReportDriverManifest> GetDriverManifestReportObj(int tradelaneShipmentId, int userId)
        {

            List<ExpressReportDriverManifest> list = new List<ExpressReportDriverManifest>();
            try
            {
                var user = dbContext.Users.Find(userId);
                string refNO = CommonConversion.GetNewFrayteNumber();
                var customerDetail = (from r in dbContext.Users
                                      join s in dbContext.TradelaneShipments on r.UserId equals s.CustomerId
                                      where s.TradelaneShipmentId == tradelaneShipmentId
                                      select new
                                      {
                                          CustomerName = r.CompanyName

                                      }).FirstOrDefault();

                var Result = dbContext.ExpressManifests.Where(a => a.TradelaneShipmentId == tradelaneShipmentId).FirstOrDefault();
                var ShipmentDetail = new TradelaneBookingRepository().GetTradelaneBookingDetails(tradelaneShipmentId, "");
                var hub = (from r in dbContext.ExpressManifests
                           join h in dbContext.Hubs on r.HubId equals h.HubId
                           where r.TradelaneShipmentId == tradelaneShipmentId
                           select h
                           ).FirstOrDefault();

                ExpressReportDriverManifest reportObj = new ExpressReportDriverManifest();
                reportObj.Ref = refNO;
                reportObj.Barcode = Result.BarCode;
                reportObj.DriverManifestName = "Destination Manifest-" + Result.BarCode + " (" + ShipmentDetail.ShipmentHandlerMethod.DisplayName + ")"; //       EMPM.ExportManifestName = "Origin Manifest-" + Result.BarCode + " (" + ShipmentDetail.ShipmentHandlerMethod.DisplayName + ")";
                reportObj.Hub = hub.Code;
                reportObj.MAWB = ShipmentDetail.AirlinePreference.AilineCode + " " + ShipmentDetail.MAWB.Substring(0, 4) + " " + ShipmentDetail.MAWB.Substring(4, 4);
                reportObj.PrintedBy = user.ContactName;

                var userInfo = (from r in dbContext.Users
                                join tz in dbContext.Timezones on r.TimezoneId equals tz.TimezoneId
                                where r.UserId == userId
                                select tz
                          ).FirstOrDefault();

                var UserTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userInfo.Name);

                //UtilityRepository.UtcDateToOtherTimezone(shipment.CreatedOnUtc, shipment.CreatedOnUtc.TimeOfDay, UserTimeZoneInfo);

                DateTime date = UtilityRepository.UtcDateToOtherTimezone(DateTime.UtcNow, DateTime.UtcNow.TimeOfDay, UserTimeZoneInfo).Item1;
                string time = UtilityRepository.GetFormattedTimeFromString(UtilityRepository.UtcDateToOtherTimezone(DateTime.UtcNow, DateTime.UtcNow.TimeOfDay, UserTimeZoneInfo).Item2);

                // Append the created by time zone in time  
                reportObj.PrintedDateTime = date.ToString("dd-MMM-yy") + " " + time + " " + userInfo.OffsetShort;
                //reportObj.PuickUpAddress = UtilityRepository.ConcatinateAddress(ShipmentDetail.ShipFrom, "");

                if (hub != null)
                {
                    TradelBookingAdress address = new TradelBookingAdress();

                    address.Address = hub.Address;
                    address.Address2 = hub.Address2;
                    address.City = hub.City;
                    address.State = hub.State;
                    address.PostCode = hub.PostCode;
                    address.Country = new FrayteCountryCode();
                    address.Phone = hub.TelephoneNo;
                    address.CompanyName = hub.Name;
                    var country = dbContext.Countries.Where(p => p.CountryId == hub.CountryId).FirstOrDefault();
                    if (country != null)
                    {
                        address.Country.CountryId = country.CountryId;
                        address.Country.Code = country.CountryCode;
                        address.Country.Code2 = country.CountryCode2;
                        address.Country.Name = country.CountryName;
                    }
                    reportObj.PuickUpAddress = UtilityRepository.ConcatinateAddress(address, "");
                }
                //reportObj.PuickUpAddress = UtilityRepository.ConcatinateAddress(ShipmentDetail.ShipTo, "");

                reportObj.MAWBChargeableWeight = ShipmentDetail.HAWBPackages.Sum(p => p.TotalWeight * p.TotalCartons);
                reportObj.MAWBGrossWeight = ShipmentDetail.HAWBPackages.Sum(p => p.TotalWeight * p.TotalCartons);

                if (ShipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId == 5)
                {
                    var shipmentAllocation = dbContext.TradelaneShipmentAllocations.Where(p => p.TradelaneShipmentId == tradelaneShipmentId && p.LegNum == "Leg2").OrderByDescending(p => p.TradelaneShipmentAllocationId).FirstOrDefault();

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
                        //UtilityRepository.UtcDateToOtherTimezone(shipment.CreatedOnUtc, shipment.CreatedOnUtc.TimeOfDay, UserTimeZoneInfo);
                        DateTime date1 = UtilityRepository.UtcDateToOtherTimezone(DateTime.UtcNow, DateTime.UtcNow.TimeOfDay, TimeZoneInfor).Item1;
                        string time1 = UtilityRepository.GetFormattedTimeFromString(UtilityRepository.UtcDateToOtherTimezone(DateTime.UtcNow, DateTime.UtcNow.TimeOfDay, TimeZoneInfor).Item2);
                        // Append the created by time zone in time  
                        reportObj.ETA = date1.ToString("dd-MMM-yy") + " " + time1;
                        reportObj.ETATimeZone = "(" + timezone.OffsetShort + ")";
                    }
                }
                else
                {
                    var shipmentAllocation = dbContext.TradelaneShipmentAllocations.Where(p => p.TradelaneShipmentId == tradelaneShipmentId).OrderByDescending(p => p.TradelaneShipmentAllocationId).FirstOrDefault();
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
                        //UtilityRepository.UtcDateToOtherTimezone(shipment.CreatedOnUtc, shipment.CreatedOnUtc.TimeOfDay, UserTimeZoneInfo);
                        DateTime date1 = UtilityRepository.UtcDateToOtherTimezone(DateTime.UtcNow, DateTime.UtcNow.TimeOfDay, TimeZoneInfor).Item1;
                        string time1 = UtilityRepository.GetFormattedTimeFromString(UtilityRepository.UtcDateToOtherTimezone(DateTime.UtcNow, DateTime.UtcNow.TimeOfDay, TimeZoneInfor).Item2);
                        // Append the created by time zone in time  
                        reportObj.ETA = date1.ToString("dd-MMM-yy") + " " + time1;
                        reportObj.ETATimeZone = "(" + timezone.OffsetShort + ")";
                    }
                }
                reportObj.CarrierBags = new List<ExpressReportDriverManifestBagDetail>();
                ExpressReportDriverManifestBagDetail mn;

                var detail = (from r in dbContext.ExpressManifests
                              join b in dbContext.ExpressBags on r.ExpressManifestId equals b.ManifestId
                              where r.TradelaneShipmentId == tradelaneShipmentId
                              select b).GroupBy(group => group.Courier).Select(p => new
                              {
                                  key = p.Key,
                                  data = p.Select(q => new
                                  {
                                      Id = q.DriverManifestId,
                                      DriverManifestNumber = dbContext.ExpressDriverManifests.Where(a => a.ExpressDriverManifestId == q.DriverManifestId).FirstOrDefault().DriverManifestBarCode,
                                      Carrier = q.Courier,
                                      BagBarCode = q.BagBarCode,
                                      BagId = q.BagId,
                                      BagNumber = q.BagNumber,
                                      HubCarrierId = q.HubCarrierId

                                  }).ToList()

                              }).ToList();

                if (detail.Count > 0)
                {
                    foreach (var item in detail)
                    {
                        int hubCarrierId = item.data[0].HubCarrierId.HasValue ? item.data[0].HubCarrierId.Value : 0;

                        var hubCarrier = dbContext.HubCarriers.Where(p => p.HubCarrierId == hubCarrierId).FirstOrDefault();
                        mn = new ExpressReportDriverManifestBagDetail();
                        mn.Carrier = item.key;
                        mn.CutOffTime = hubCarrier.CutOffTime;
                        mn.NoOfBags = item.data.Count;

                        decimal totalWeight = 0.00M;
                        int totalPieces = 0;
                        foreach (var item1 in item.data)
                        {
                            var ship = (from r in dbContext.Expresses
                                        join d in dbContext.ExpressDetails on r.ExpressId equals d.ExpressId
                                        where r.BagId == item1.BagId
                                        select d
                                        ).ToList();

                            totalPieces += ship.Count;
                            totalWeight = ship.Sum(p => p.Weight * p.CartonQty);
                        }
                        mn.TotalPieces = totalPieces;
                        mn.TotalWeight = totalWeight;
                        reportObj.CarrierBags.Add(mn);
                    }
                    reportObj.CarrierManifests = new List<ExpressReportDriverCarrierManifest>();
                    ExpressReportDriverCarrierManifest cm;
                    foreach (var item in detail)
                    {
                        cm = new ExpressReportDriverCarrierManifest();
                        int hubCarrierId = item.data[0].HubCarrierId.HasValue ? item.data[0].HubCarrierId.Value : 0;
                        cm.CarrierManifestBarcoede = item.data.FirstOrDefault().DriverManifestNumber;
                        cm.CarrierManifest = item.key + " - (" + item.data.FirstOrDefault().DriverManifestNumber + ")" + " Destination Manifest";
                        cm.CarrierBagDetails = new List<ExpressReportCarrierBagDetail>();
                        ExpressReportCarrierBagDetail d;
                        var hubCarrier = dbContext.HubCarriers.Where(p => p.HubCarrierId == hubCarrierId).FirstOrDefault();

                        if (hubCarrier != null)
                        {
                            TradelBookingAdress address = new TradelBookingAdress();

                            address.Address = hubCarrier.Address;
                            address.Address2 = hubCarrier.Address2;
                            address.City = hubCarrier.City;
                            address.State = hubCarrier.State;
                            address.PostCode = hubCarrier.PostCode;
                            address.Country = new FrayteCountryCode();
                            address.Phone = hubCarrier.TelephoneNo;
                            address.CompanyName = hubCarrier.CompanyName;
                            var country = dbContext.Countries.Where(p => p.CountryId == hubCarrier.CountryId).FirstOrDefault();
                            if (country != null)
                            {
                                address.Country.CountryId = country.CountryId;
                                address.Country.Code = country.CountryCode;
                                address.Country.Code2 = country.CountryCode2;
                                address.Country.Name = country.CountryName;
                            }
                            cm.DropOffAddress = UtilityRepository.ConcatinateAddress(address, "");
                        }

                        foreach (var item1 in item.data)
                        {
                            d = new ExpressReportCarrierBagDetail();

                            d.BagNumber = item1.BagNumber;

                            var sdf = (from r in dbContext.Expresses
                                       join ed in dbContext.ExpressDetails on r.ExpressId equals ed.ExpressId
                                       where r.BagId == item1.BagId
                                       select ed
                                        ).ToList();
                            d.TotalPieces = sdf.Count;
                            d.ExporterName = customerDetail.CustomerName;
                            d.TotalWeight = sdf.Sum(p => p.CartonQty * p.Weight);
                            d.Carrier = item.key;
                            d.BagId = item1.BagId;
                            d.TermAndCondition = hubCarrier.TermAndCondition;
                            cm.CarrierBagDetails.Add(d);
                        }
                        reportObj.CarrierManifests.Add(cm);
                    }
                }
                list.Add(reportObj);
                return list;
            }
            catch (Exception ex)
            {
                return list;
            }
        }

        public List<ExpressReportBagLabel> GetBagLabelReportObj(int bagId, int userId)
        {
            List<ExpressReportBagLabel> list = new List<ExpressReportBagLabel>();
            try
            {
                var user = dbContext.Users.Find(userId);
                string refNO = CommonConversion.GetNewFrayteNumber();

                var bag = dbContext.ExpressBags.Find(bagId);

                if (bag != null)
                {
                    var sdf = (from r in dbContext.Expresses
                               join ed in dbContext.ExpressDetails on r.ExpressId equals ed.ExpressId
                               where r.BagId == bagId
                               select ed).ToList();

                    var hub = (from r in dbContext.Expresses
                               join h in dbContext.HubCarrierServices on r.HubCarrierServiceId equals h.HubCarrierServiceId
                               join hc in dbContext.HubCarriers on h.HubCarrierId equals hc.HubCarrierId
                               join hb in dbContext.Hubs on hc.HubId equals hb.HubId
                               join hu in dbContext.HubUsers on hb.HubId equals hu.HubId
                               join u in dbContext.Users on hu.UserId equals u.UserId
                               where r.BagId == bagId
                               select new
                               {
                                   Code = hb.Code,
                                   NotifyParty = u.CompanyName
                               }
                               ).FirstOrDefault();

                    ExpressReportBagLabel reportObj = new ExpressReportBagLabel();
                    reportObj.Ref = refNO;
                    reportObj.Hub = hub != null ? hub.Code : "";
                    reportObj.Destination = hub != null ? hub.Code : "";
                    reportObj.ServiceType = bag.Courier;
                    reportObj.DestinationAgent = hub != null ? hub.NotifyParty : "";
                    reportObj.TotalQty = sdf.Count;
                    reportObj.BagWeight = sdf != null ? sdf.Sum(p => p.CartonQty * p.Weight) : 0;
                    reportObj.BagBarCode = bag.BagBarCode;

                    var userInfo = (from r in dbContext.Users
                                    join tz in dbContext.Timezones on r.TimezoneId equals tz.TimezoneId
                                    where r.UserId == userId
                                    select tz
                      ).FirstOrDefault();

                    var UserTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userInfo.Name);

                    DateTime date = UtilityRepository.UtcDateToOtherTimezone(DateTime.UtcNow, DateTime.UtcNow.TimeOfDay, UserTimeZoneInfo).Item1;
                    string time = UtilityRepository.GetFormattedTimeFromString(UtilityRepository.UtcDateToOtherTimezone(DateTime.UtcNow, DateTime.UtcNow.TimeOfDay, UserTimeZoneInfo).Item2);

                    // Append the created by time zone in time  
                    reportObj.DateTime = date.ToString("dd-MMM-yy") + " " + time + " " + userInfo.OffsetShort;
                    list.Add(reportObj);
                }

                return list;
            }
            catch (Exception ex)
            {
                return list;
            }
        }

        public ExportManifestPdfModel GetExportManifestPDFDataSource(int TradelaneShipmentId)
        {
            ExportManifestPdfModel EMPM = new ExportManifestPdfModel();
            var Result = dbContext.ExpressManifests.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).FirstOrDefault();
            TradelaneBooking ShipmentDetail = new TradelaneBookingRepository().GetTradelaneBookingDetails(TradelaneShipmentId, "");
            if (Result != null)
            {
                EMPM.ExportManifestName = "Origin Manifest-" + Result.BarCode + " (" + ShipmentDetail.ShipmentHandlerMethod.DisplayName + ")";
                EMPM.BarCode = Result.BarCode;
                EMPM.PrintedBy = dbContext.Users.Where(a => a.UserId == Result.CreadtdBy.Value).FirstOrDefault().ContactName;
                EMPM.PrintedOn = GetDateTimeString(Result.CreadtdBy, Result.CreatedOn);
                EMPM.PickUpAddress = UtilityRepository.ConcatinateExpressAddress(ShipmentDetail.ShipFrom);
                EMPM.MawbInfo = new List<ExpressMawbInformationModel>();
                var MawbDtl = dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).ToList();
                if (MawbDtl != null && MawbDtl.Count > 0)
                {
                    foreach (var MD in MawbDtl)
                    {
                        ExpressMawbInformationModel ExMa = new ExpressMawbInformationModel();
                        ExMa.Airline = GetAirlineName(MD.AirlineId);
                        ExMa.ETA = GetDateTimeZone(MD.AgentId, MD.EstimatedDateofArrival).Item1;
                        ExMa.ETD = GetDateTimeZone(MD.AgentId, MD.EstimatedDateofDelivery).Item1;
                        ExMa.ETATimeZone = GetDateTimeZone(MD.AgentId, MD.EstimatedDateofDelivery).Item2;
                        ExMa.ETDTimeZone = GetDateTimeZone(MD.AgentId, MD.EstimatedDateofDelivery).Item2;
                        ExMa.FlightNo = MD.FlightNumber;
                        var AirlineCode = ExMa.Airline.Split('-');
                        ExMa.Mawb = AirlineCode[1].Substring(1, 3) + " " + MD.MAWB.Substring(0, 4) + " " + MD.MAWB.Substring(4, 4);
                        EMPM.MawbInfo.Add(ExMa);
                    }
                }
                EMPM.BagsInfo = new List<BagDetail>();
                var list = dbContext.spGet_GetExpressManifestedShipments(Result.ExpressManifestId).ToList();
                ExpressViewManifest EVM = new ExpressViewManifest();

                EVM.ManifestedList = new List<ExpressManifestDetail>();
                if (list != null && list.Count > 0)
                {
                    var Bags = list.GroupBy(a => a.BagBarCode).ToList();
                    foreach (var b in Bags)
                    {
                        BagDetail Bg = new BagDetail();
                        EVM.ManifestName = Bags.FirstOrDefault().FirstOrDefault().BarCode;
                        Bg.BagNumber = b.FirstOrDefault().BagBarCode;
                        Bg.Carrier = b.FirstOrDefault().Carrier;
                        Bg.ExporterName = b.FirstOrDefault().ContactName;
                        Bg.TotalPieces = b.FirstOrDefault().TotalNoOfShipments.Value;
                        Bg.TotalWeight = b.FirstOrDefault().TotalWeight;
                        EMPM.BagsInfo.Add(Bg);
                    }
                }
            }
            return EMPM;
        }

        public List<ExpressBagNumberReport> GetBagNumberReportObj(int bagId, int userId)
        {
            List<ExpressBagNumberReport> list = new List<ExpressBagNumberReport>();
            ExpressBagNumberReport reportObj = new ExpressBagNumberReport();

            var bag = dbContext.ExpressBags.Find(bagId);
            if (bag != null)
            {
                if (!string.IsNullOrEmpty(bag.BagNumber))
                {
                    var arr = bag.BagNumber.Split('-');
                    if (arr.Count() > 0 && arr.Count() == 3)
                    {
                        reportObj.Hub = arr[0];
                        reportObj.Carrier = arr[1];
                        reportObj.BagCount = arr[1] + "/" + arr[2];
                    }
                    var manifest = dbContext.ExpressManifests.Find(bag.ManifestId);
                    if (manifest != null)
                    {
                        reportObj.TradelaneShipmentId = manifest.TradelaneShipmentId.HasValue ? manifest.TradelaneShipmentId.Value : 0;
                    }
                    reportObj.Ref = CommonConversion.GetNewFrayteNumber();
                    list.Add(reportObj);
                }
            }
            return list;
        }

        private string GetDateTimeString(int? UserId, DateTime? CreatedOn)
        {
            var customerDetail = dbContext.Users.Where(a => a.UserId == UserId).FirstOrDefault();
            var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == customerDetail.TimezoneId).FirstOrDefault();
            var Timezone = new TimeZoneModal();
            if (timeZone != null)
            {
                Timezone.TimezoneId = timeZone.TimezoneId;
                Timezone.Name = timeZone.Name;
                Timezone.Offset = timeZone.Offset;
                Timezone.OffsetShort = timeZone.OffsetShort;
            }

            var TimeZoneDetail = TimeZoneInfo.FindSystemTimeZoneById(timeZone.Name);
            var CreatedOn1 = UtilityRepository.UtcDateToOtherTimezone(CreatedOn.Value, CreatedOn.Value.TimeOfDay, TimeZoneDetail).Item1;
            return CreatedOn1.ToString("dd-MMM-yyyy hh:mm") + Environment.NewLine + " (" + Timezone.OffsetShort + ")";
        }

        private Tuple<string, string> GetDateTimeZone(int? UserId, DateTime? CreatedOn)
        {
            var customerDetail = dbContext.Users.Where(a => a.UserId == UserId).FirstOrDefault();
            var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == customerDetail.TimezoneId).FirstOrDefault();
            var Timezone = new TimeZoneModal();
            if (timeZone != null)
            {
                Timezone.TimezoneId = timeZone.TimezoneId;
                Timezone.Name = timeZone.Name;
                Timezone.Offset = timeZone.Offset;
                Timezone.OffsetShort = timeZone.OffsetShort;
            }

            var TimeZoneDetail = TimeZoneInfo.FindSystemTimeZoneById(timeZone.Name);
            var CreatedOn1 = UtilityRepository.UtcDateToOtherTimezone(CreatedOn.Value, CreatedOn.Value.TimeOfDay, TimeZoneDetail).Item1;
            return Tuple.Create(CreatedOn1.ToString("dd-MMM-yyyy hh:mm"), "(" + Timezone.OffsetShort + ")");
        }

        private string GetAirlineName(int? AirlineId)
        {
            var getairline = dbContext.Airlines.Where(a => a.AirlineId == AirlineId).FirstOrDefault();
            string airline = "";
            if (getairline != null)
            {
                airline = getairline.AirLineName + " - " + getairline.AirlineCode;
            }
            return airline;
        }
    }
}