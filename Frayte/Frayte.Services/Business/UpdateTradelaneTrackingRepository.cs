using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.Tradelane;

namespace Frayte.Services.Business
{
    public class UpdateTradelaneTrackingRepository
    {
        FrayteEntities dbContext = new FrayteEntities();
        public FrayteResult SaveTradelaneShipmentTracking(TradelaneOperationslTrackingModel TM)
        {
            FrayteResult FR = new FrayteResult();
            FR.Status = false;
            var Result = dbContext.TradelaneShipmentTrackings.Where(a => a.TradelaneShipmentTrackingId == TM.TradelaneShipmentTrackingId).FirstOrDefault();
            if (Result != null)
            {
                Result.TradlaneShipmentId = TM.TradelaneShipmentId;
                Result.TrackingDescription = TM.TrackingDescription;
                Result.TrackingCode = TM.TrackingCode;
                Result.CreatedOnUtc = TM.CreatedOnUtc;
                Result.FlightNumber = TM.FlightNo;
                Result.CreatedBy = TM.CreatedBy;
                Result.Weight = TM.Weight;
                Result.Pieces = TM.Pieces;
                Result.AirportCode = TM.AirportCode;
                dbContext.Entry(Result).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                //if(Result.TrackingCode == "DLV")
                //{
                //    var res = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == TM.TradelaneShipmentId).FirstOrDefault();
                //    if(res != null)
                //    {
                //        res.ShipmentStatusId = 35;
                //        dbContext.Entry(res).State = System.Data.Entity.EntityState.Modified;
                //        dbContext.SaveChanges();
                //    }
                //}

                FR.Status = true;
            }
            else
            {
                TradelaneShipmentTracking TlST = new TradelaneShipmentTracking();
                TlST.TradlaneShipmentId = TM.TradelaneShipmentId;
                TlST.TrackingDescription = TM.TrackingDescription;
                TlST.TrackingCode = TM.TrackingCode;
                TlST.FlightNumber = TM.FlightNo;
                TlST.CreatedOnUtc = DateTime.UtcNow;
                TlST.CreatedBy = TM.CreatedBy;
                TlST.AirportCode = TM.AirportCode;
                TlST.Weight = TM.Weight;
                TlST.Pieces = TM.Pieces;
                dbContext.TradelaneShipmentTrackings.Add(TlST);
                dbContext.SaveChanges();
                if (TM.TrackingCode == "DEP")
                {
                    var res = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == TM.TradelaneShipmentId).FirstOrDefault();
                    if (res != null)
                    {
                        res.ShipmentStatusId = 30;
                        //res.ShipmentStatusId = 31;
                        res.IsMawbCorrection = false;
                        dbContext.Entry(res).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
                if (TM.TrackingCode == "INT")
                {
                    var res = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == TM.TradelaneShipmentId).FirstOrDefault();
                    if (res != null)
                    {
                        res.ShipmentStatusId = 31;
                        res.IsMawbCorrection = false;
                        dbContext.Entry(res).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
                if (TM.TrackingCode == "ARV")
                {
                    var res = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == TM.TradelaneShipmentId).FirstOrDefault();
                    if (res != null)
                    {
                        res.ShipmentStatusId = 32;
                        //res.ShipmentStatusId = 31;
                        res.IsMawbCorrection = false;
                        dbContext.Entry(res).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
                if (TM.TrackingCode == "DLV")
                {
                    var res = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == TM.TradelaneShipmentId).FirstOrDefault();
                    if (res != null)
                    {
                        res.ShipmentStatusId = 35;
                        res.IsMawbCorrection = false;
                        dbContext.Entry(res).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }

                    // Update Express AWB status if status is delivered

                    new ExpressManifestRepository().UpdateExpressAWbStatus(TM.TradelaneShipmentId);

                }
                FR.Status = true;
                //send mail of add tracking
                if (FR.Status)
                {
                    var Res = new TradelaneEmailRepository().SendUpdateTrackingEmail(TM);
                    if (Res.Status == false)
                    {
                        //FR.Errors = new List<string>();
                        //FR.Errors.Add("Email does not exist for this Operational Status in tracking configuration");
                        //DeleteTradelaneOperationalTracking(TlST.TradelaneShipmentTrackingId);
                    }
                }
            }
            return FR;
        }

        public FrayteResult SaveTradelaneTracking(TradelaneUpdateTrackingModel TM)
        {

            var Shipment = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == TM.TradelaneShipmentId).FirstOrDefault();
            var user = dbContext.Users.Where(a => a.UserId == Shipment.CustomerId).FirstOrDefault();

            FrayteResult FR = new FrayteResult();
            FR.Status = false;
            var Result = dbContext.TradelaneFlightDetails.Where(a => a.TradelaneFlightDetailId == TM.TradelaneFlightId).FirstOrDefault();
            if (Result != null)
            {
                Result.TradelaneShipmentId = TM.TradelaneShipmentId;
                Result.FlightNumber = TM.FlightNo;
                Result.ArrivalAirportCode = TM.DestinationAirportCode;
                Result.DepartureAirportCode = TM.DepartureAirportCode;
                var Timezone = dbContext.Timezones.Where(a => a.TimezoneId == user.TimezoneId).FirstOrDefault();
                TimeZoneModal TZM = new TimeZoneModal();
                if (Timezone != null)
                {
                    TZM.TimezoneId = Timezone.TimezoneId;
                    TZM.Name = Timezone.Name;
                }
                Result.ArrivalDate = UtilityRepository.ConvertToUniversalTimeWitDate(TM.ArrivalTime, TM.ArrivalDate.Value, TZM);
                Result.DepartureDate = UtilityRepository.ConvertToUniversalTimeWitDate(TM.DepartureTime, TM.DepartureDate.Value, TZM);
                Result.BookingStatus = TM.BookingStatus;
                Result.Pieces = TM.TotalPeices;
                Result.TotalVolume = TM.Volume;
                Result.TotalWeight = TM.TotalWeight;
                dbContext.Entry(Result).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                FR.Status = true;
            }
            else
            {
                TradelaneFlightDetail TlST = new TradelaneFlightDetail();
                TlST.TradelaneShipmentId = TM.TradelaneShipmentId;
                TlST.FlightNumber = TM.FlightNo;
                TlST.ArrivalAirportCode = TM.DestinationAirportCode;
                TlST.DepartureAirportCode = TM.DepartureAirportCode;
                var Timezone = dbContext.Timezones.Where(a => a.TimezoneId == user.TimezoneId).FirstOrDefault();
                TimeZoneModal TZM = new TimeZoneModal();
                if (Timezone != null)
                {

                    TZM.TimezoneId = Timezone.TimezoneId;
                    TZM.Name = Timezone.Name;
                }
                TlST.ArrivalDate = UtilityRepository.ConvertToUniversalTimeWitDate(TM.ArrivalTime, TM.ArrivalDate.Value, TZM);
                TlST.DepartureDate = UtilityRepository.ConvertToUniversalTimeWitDate(TM.DepartureTime, TM.DepartureDate.Value, TZM);
                TlST.BookingStatus = TM.BookingStatus;
                TlST.Pieces = TM.TotalPeices;
                TlST.TotalVolume = TM.Volume;
                TlST.TotalWeight = TM.TotalWeight;
                dbContext.TradelaneFlightDetails.Add(TlST);
                dbContext.SaveChanges();
                FR.Status = true;
            }
            return FR;
        }

        public TradelaneTrackingModel GetTradelaneShipmentTracking(int TradelaneShipmentId)
        {
            TradelaneTrackingModel TTM = new TradelaneTrackingModel();
            TTM.TradelaneOperationalDetail = new List<TradelaneOperationslTrackingModel>();
            TTM.ShipmentDetail = new TradelanePublicDetail();
            TTM.TradelaneStatus = new List<TradelaneTrackingShipmentStatus>();
            try
            {
                TTM.TradelaneStatus = GetShipmentTrackingStatus(TradelaneShipmentId);

                var TB = new TradelaneBookingRepository().GetTradelaneBookingDetails(TradelaneShipmentId, "TradelaneShipmentBooking");

                var Res = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).ToList();
                //var ShipmentDetail = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).FirstOrDefault();
                if (TB != null)
                {
                    TTM.ShipmentDetail.CurrentStatus = dbContext.ShipmentStatus.Where(a => a.ShipmentStatusId == TB.ShipmentStatusId).FirstOrDefault().DisplayStatusName;
                    TTM.ShipmentDetail.DepartureAirportCode = TB.DepartureAirport.AirportCode;
                    TTM.ShipmentDetail.DestinationAirportCode = TB.DestinationAirport.AirportCode;
                    TTM.ShipmentDetail.EstimatedWeight = Res.Sum(a => a.Weight);
                    TTM.ShipmentDetail.FrayteNumber = TB.FrayteNumber;
                    TTM.ShipmentDetail.Mawb = TB.AirlinePreference.AilineCode + " " + TB.MAWB.Substring(0, 4) + " " + TB.MAWB.Substring(4, 4) ?? "";
                    TTM.ShipmentDetail.TotalPieces = Res.Count;
                    TTM.ShipmentDetail.TotalVolume = TB.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? Math.Round((Res.Sum(p => p.Length * p.Weight * p.Height)) / (100 * 100 * 100), 2) : Math.Round((Res.Sum(p => p.Length * p.Weight * p.Height)) / (39.37M * 39.37M * 39.37M), 2);
                }

                var Result = dbContext.TradelaneShipmentTrackings.Where(a => a.TradlaneShipmentId == TradelaneShipmentId).ToList();

                if (Result != null && Result.Count > 0)
                {
                    foreach (var TM in Result)
                    {
                        TradelaneOperationslTrackingModel TlST = new TradelaneOperationslTrackingModel();
                        TlST.TradelaneShipmentTrackingId = TM.TradelaneShipmentTrackingId;
                        TlST.TradelaneShipmentId = TM.TradlaneShipmentId;
                        TlST.Weight = TM.Weight != null ? TM.Weight.Value : 0;
                        TlST.Pieces = TM.Pieces != null ? TM.Pieces.Value : 0;
                        TlST.TrackingDescription = TM.TrackingDescription;
                        TlST.FlightNo = TM.FlightNumber;
                        TlST.TrackingCode = TM.TrackingCode;
                        TlST.CreatedOnUtc = TM.CreatedOnUtc;
                        TlST.CreatedBy = TM.CreatedBy;
                        TlST.AirportCode = TM.AirportCode;
                        TTM.TradelaneOperationalDetail.Add(TlST);
                    }
                }
                else
                {

                }

                TTM.TradelaneTrackingDetail = new List<TradelaneUpdateTrackingModel>();

                TTM.TradelaneTrackingDetail = (from TS in dbContext.TradelaneFlightDetails
                                               where TS.TradelaneShipmentId == TradelaneShipmentId
                                               select new TradelaneUpdateTrackingModel
                                               {
                                                   TradelaneFlightId = TS.TradelaneFlightDetailId,
                                                   TradelaneShipmentId = TS.TradelaneShipmentId.Value,
                                                   FlightNo = TS.FlightNumber != null ? TS.FlightNumber : "",
                                                   DepartureDate = TS.DepartureDate,
                                                   DepartureTime = TS.DepartureDate != null ? TS.DepartureDate.Value.ToString() : "",
                                                   ArrivalDate = TS.ArrivalDate.Value,
                                                   ArrivalTime = TS.ArrivalDate != null ? TS.ArrivalDate.Value.ToString() : "",
                                                   BookingStatus = TS.BookingStatus,
                                                   DepartureAirportCode = TS.DepartureAirportCode,
                                                   DestinationAirportCode = TS.ArrivalAirportCode,
                                                   TotalPeices = TS.Pieces.Value,
                                                   TotalWeight = TS.TotalWeight.Value,
                                                   Volume = TS.TotalVolume.Value
                                               }).ToList();

                var Shipment = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).FirstOrDefault();
                if (Shipment != null)
                {
                    var user = dbContext.Users.Where(a => a.UserId == Shipment.CustomerId).FirstOrDefault();
                    var Timezone = dbContext.Timezones.Where(a => a.TimezoneId == user.TimezoneId).FirstOrDefault();
                    TimeZoneInfo TZM = TimeZoneInfo.FindSystemTimeZoneById(Timezone.Name);

                    for (int i = 0; i < TTM.TradelaneTrackingDetail.Count; i++)
                    {
                        if (TTM.TradelaneTrackingDetail[i].DepartureDate != null)
                        {
                            TTM.TradelaneTrackingDetail[i].DepartureTime = UtilityRepository.UtcDateToOtherTimezone(TTM.TradelaneTrackingDetail[i].DepartureDate.Value, TTM.TradelaneTrackingDetail[i].DepartureDate.Value.TimeOfDay, TZM).Item2;
                            TTM.TradelaneTrackingDetail[i].DepartureDate = UtilityRepository.UtcDateToOtherTimezone(TTM.TradelaneTrackingDetail[i].DepartureDate.Value, TTM.TradelaneTrackingDetail[i].DepartureDate.Value.TimeOfDay, TZM).Item1;

                        }
                        if (TTM.TradelaneTrackingDetail[i].ArrivalDate != null)
                        {
                            TTM.TradelaneTrackingDetail[i].ArrivalTime = UtilityRepository.UtcDateToOtherTimezone(TTM.TradelaneTrackingDetail[i].ArrivalDate.Value, TTM.TradelaneTrackingDetail[i].ArrivalDate.Value.TimeOfDay, TZM).Item2;
                            TTM.TradelaneTrackingDetail[i].ArrivalDate = UtilityRepository.UtcDateToOtherTimezone(TTM.TradelaneTrackingDetail[i].ArrivalDate.Value, TTM.TradelaneTrackingDetail[i].ArrivalDate.Value.TimeOfDay, TZM).Item1;

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return TTM;
        }

        public List<TradelaneAirport> GetAirports()
        {
            List<TradelaneAirport> list = (from r in dbContext.Airports
                                           select new TradelaneAirport
                                           {
                                               AirportCodeId = r.AirportCodeId,
                                               CountryId = r.CountryId,
                                               AirportCode = r.AirportCode,
                                               AirportName = r.AirportName
                                           }).ToList();
            return list;
        }

        public List<TrackingMileStoneModel> GetTradelaneMilestone(int ShipmentHandlerMethodId)
        {
            List<TrackingMileStoneModel> List = (from TMi in dbContext.TrackingMileStones
                                                 where TMi.ShipmentHandlerMethodId == ShipmentHandlerMethodId
                                                 select new TrackingMileStoneModel
                                                 {
                                                     TrackingMileStoneId = TMi.TrackingMileStoneId,
                                                     CreatedBy = TMi.CreatedBy,
                                                     CreatedOnUtc = TMi.CreatedOnUtc,
                                                     Description = TMi.Description,
                                                     MileStoneKey = TMi.MileStoneKey,
                                                     OrderNumber = TMi.OrderNumber,
                                                     ShipmentHandlerMethodId = TMi.ShipmentHandlerMethodId,
                                                     UpdatedBy = TMi.UpdatedBy.Value,
                                                     UpdatedOnUtc = TMi.UpdatedOnUtc.Value
                                                 }).ToList();
            return List;
        }

        public int GetTradelaneShipemntHandlerMethodId(int TradelaneShipmentId)
        {
            return dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).FirstOrDefault().ShipmentHandlerMethodId.Value;
        }
        public TradelaneShipmentDetailModel GetShipmentDetail(int TradelaneShipmentId)
        {
            var Result = (from TS in dbContext.TradelaneShipments
                          let pkgtype = TS.PackageCalculatonType
                          join Ar in dbContext.Airlines
                          on TS.AirlineId equals Ar.AirlineId
                          where TS.TradelaneShipmentId == TradelaneShipmentId
                          select new TradelaneShipmentDetailModel
                          {
                              Mawb = TS.MAWB,
                              FrayteRefNo = TS.FrayteNumber,
                              DepartureAirport = TS.DepartureAirportCode,
                              ArrivalAirport = TS.DestinationAirportCode,
                              TotalPieces = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).ToList().Sum(a => a.CartonValue),
                              TotalWeight = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).ToList().Sum(a => a.Weight),
                              Volume = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).Sum(p => pkgtype == FraytePakageCalculationType.kgtoCms ? Math.Round((p.Length / 100) * (p.Width / 100) * (p.Height / 100), 2) : Math.Round((p.Length / 39.37M) * (p.Width / 39.37M) * (p.Height / 39.37M), 2)),
                              Airlines = new TradelaneAirline()
                              {
                                  AirlineId = Ar.AirlineId,
                                  AilineCode = Ar.AirlineCode,
                                  AirLineName = Ar.AirLineName,
                                  CarrierCode2 = Ar.CarrierCode2,
                                  CarrierCode3 = Ar.CarrierCode3
                              }
                          }).FirstOrDefault();


            //Result.Volume = ((dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).ToList().Sum(a => a.Width) * dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).ToList().Sum(a => a.Width) * dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).ToList().Sum(a => a.Width))/6000) * dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).ToList().Sum(a => a.CartonValue);

            return Result;
        }

        public FrayteResult DeleteTradelaneOperationalTracking(int TradelaneShipmentTrackingId)
        {
            FrayteResult FR = new FrayteResult();
            FR.Status = false;
            var Result = dbContext.TradelaneShipmentTrackings.Where(a => a.TradelaneShipmentTrackingId == TradelaneShipmentTrackingId).FirstOrDefault();
            if (Result != null)
            {
                dbContext.TradelaneShipmentTrackings.Remove(Result);
                dbContext.SaveChanges();
                FR.Status = true;
            }
            return FR;

        }
        public FrayteResult DeleteTradelaneTracking(int FlightDetailTrackingId)
        {
            FrayteResult FR = new FrayteResult();
            FR.Status = false;
            var Result = dbContext.TradelaneFlightDetails.Where(a => a.TradelaneFlightDetailId == FlightDetailTrackingId).FirstOrDefault();
            if (Result != null)
            {
                dbContext.TradelaneFlightDetails.Remove(Result);
                dbContext.SaveChanges();
                FR.Status = true;
            }
            return FR;

        }



        public List<TradelaneTrackingShipmentStatus> GetShipmentTrackingStatus(int TradelaneShipmentId)
        {
            List<TradelaneTrackingShipmentStatus> TSL = new List<TradelaneTrackingShipmentStatus>();
            var TradelaneTracking = dbContext.TradelaneShipmentTrackings.Where(a => a.TradlaneShipmentId == TradelaneShipmentId).Select(a => a.AirportCode).ToList();
            var AirPortNew = TradelaneTracking.Distinct().ToList();
            var Tracking = dbContext.TradelaneShipmentTrackings.Where(a => a.TradlaneShipmentId == TradelaneShipmentId).ToList();
            if (Tracking != null && AirPortNew != null && AirPortNew.Count > 0)
            {
                var count = 1;
                foreach (var APC in AirPortNew)
                {
                    TradelaneTrackingShipmentStatus TTS = new TradelaneTrackingShipmentStatus();
                    TTS.TrackingStatus = new List<TradelaneTrackingStatus>();
                    TTS.AirportCode = APC;
                    var count1 = 0;
                    foreach (var Tr in Tracking)
                    {
                        var TS = new TradelaneTrackingStatus();
                        if (Tr.AirportCode == APC && Tr.TrackingCode == TradelaneStatus.Booked)
                        {
                            TS.ShipmentCode = TradelaneStatus.Booked;
                            TS.Pieces = Tracking[count1].Pieces != null ? Tracking[count1].Pieces.Value : 0;
                            TS.Date = Tracking[count1].CreatedOnUtc != null ? Tracking[count1].CreatedOnUtc : DateTime.MinValue;
                            TS.TotalWeight = Tracking[count1].Weight != null ? Tracking[count1].Weight.Value : 0;
                            TTS.TrackingStatus.Add(TS);
                        }
                        if (Tr.AirportCode == APC && Tr.TrackingCode == TradelaneStatus.Arrived)
                        {
                            TS.ShipmentCode = TradelaneStatus.Arrived;
                            TS.Pieces = Tracking[count1].Pieces != null ? Tracking[count1].Pieces.Value : 0;
                            TS.Date = Tracking[count1].CreatedOnUtc != null ? Tracking[count1].CreatedOnUtc : DateTime.MinValue;
                            TS.TotalWeight = Tracking[count1].Weight != null ? Tracking[count1].Weight.Value : 0;
                            TTS.TrackingStatus.Add(TS);
                        }
                        if (Tr.AirportCode == APC && Tr.TrackingCode == TradelaneStatus.Departed)
                        {
                            TS.ShipmentCode = TradelaneStatus.Departed;
                            TS.Pieces = Tracking[count1].Pieces != null ? Tracking[count1].Pieces.Value : 0;
                            TS.Date = Tracking[count1].CreatedOnUtc != null ? Tracking[count1].CreatedOnUtc : DateTime.MinValue;
                            TS.TotalWeight = Tracking[count1].Weight != null ? Tracking[count1].Weight.Value : 0;
                            TTS.TrackingStatus.Add(TS);
                        }
                        if (Tr.AirportCode == APC && Tr.TrackingCode == TradelaneStatus.Delivered)
                        {
                            TS.ShipmentCode = TradelaneStatus.Delivered;
                            TS.Pieces = Tracking[count1].Pieces != null ? Tracking[count1].Pieces.Value : 0;
                            TS.Date = Tracking[count1].CreatedOnUtc != null ? Tracking[count1].CreatedOnUtc : DateTime.MinValue;
                            TS.TotalWeight = Tracking[count1].Weight != null ? Tracking[count1].Weight.Value : 0;
                            TTS.TrackingStatus.Add(TS);
                        }
                        if (Tr.AirportCode == APC && Tr.TrackingCode == TradelaneStatus.WarehouseDeparted)
                        {
                            TS.ShipmentCode = TradelaneStatus.WarehouseDeparted;
                            TS.Pieces = Tracking[count1].Pieces != null ? Tracking[count1].Pieces.Value : 0;
                            TS.Date = Tracking[count1].CreatedOnUtc != null ? Tracking[count1].CreatedOnUtc : DateTime.MinValue;
                            TS.TotalWeight = Tracking[count1].Weight != null ? Tracking[count1].Weight.Value : 0;
                            TTS.TrackingStatus.Add(TS);
                        }
                        if (Tr.AirportCode == APC && Tr.TrackingCode == TradelaneStatus.AirportArrived)
                        {
                            TS.ShipmentCode = TradelaneStatus.AirportArrived;
                            TS.Pieces = Tracking[count1].Pieces != null ? Tracking[count1].Pieces.Value : 0;
                            TS.Date = Tracking[count1].CreatedOnUtc != null ? Tracking[count1].CreatedOnUtc : DateTime.MinValue;
                            TS.TotalWeight = Tracking[count1].Weight != null ? Tracking[count1].Weight.Value : 0;
                            TTS.TrackingStatus.Add(TS);
                        }
                        if (Tr.AirportCode == APC && Tr.TrackingCode == TradelaneStatus.AirportDeparture)
                        {
                            TS.ShipmentCode = TradelaneStatus.AirportDeparture;
                            TS.Pieces = Tracking[count1].Pieces != null ? Tracking[count1].Pieces.Value : 0;
                            TS.Date = Tracking[count1].CreatedOnUtc != null ? Tracking[count1].CreatedOnUtc : DateTime.MinValue;
                            TS.TotalWeight = Tracking[count1].Weight != null ? Tracking[count1].Weight.Value : 0;
                            TTS.TrackingStatus.Add(TS);
                        }
                        if (Tr.AirportCode == APC && Tr.TrackingCode == TradelaneStatus.ShipmentReceived)
                        {
                            TS.ShipmentCode = TradelaneStatus.ShipmentReceived;
                            TS.Pieces = Tracking[count1].Pieces != null ? Tracking[count1].Pieces.Value : 0;
                            TS.Date = Tracking[count1].CreatedOnUtc != null ? Tracking[count1].CreatedOnUtc : DateTime.MinValue;
                            TS.TotalWeight = Tracking[count1].Weight != null ? Tracking[count1].Weight.Value : 0;
                            TTS.TrackingStatus.Add(TS);
                        }
                        if (Tr.AirportCode == APC && Tr.TrackingCode == TradelaneStatus.Intransit)
                        {
                            TS.ShipmentCode = TradelaneStatus.Intransit;
                            TS.Pieces = Tracking[count1].Pieces != null ? Tracking[count1].Pieces.Value : 0;
                            TS.Date = Tracking[count1].CreatedOnUtc != null ? Tracking[count1].CreatedOnUtc : DateTime.MinValue;
                            TS.TotalWeight = Tracking[count1].Weight != null ? Tracking[count1].Weight.Value : 0;
                            TTS.TrackingStatus.Add(TS);
                        }
                        count1++;
                    }
                    TSL.Add(TTS);
                }
            }
            return TSL;
        }

        //public List<TradelaneTrackingShipmentStatus> GetShipmentTrackingStatus(int TradelaneShipmentId)
        //{
        //    List<TradelaneTrackingShipmentStatus> TSL = new List<TradelaneTrackingShipmentStatus>();
        //    var TradelaneTracking = dbContext.TradelaneShipmentTrackings.Where(a => a.TradlaneShipmentId == TradelaneShipmentId).Select(a => a.AirportCode).ToList();
        //    var AirPortNew = TradelaneTracking.Distinct().ToList();

        //    if (AirPortNew != null && AirPortNew.Count > 0)
        //    {

        //        foreach (var APC in AirPortNew)
        //        {
        //            TradelaneTrackingShipmentStatus TTS = new TradelaneTrackingShipmentStatus();
        //            TTS.TrackingStatus = new List<TradelaneTrackingStatus>();
        //            var Tracking2 = dbContext.TradelaneShipmentTrackings.Where(a => a.TradlaneShipmentId == TradelaneShipmentId).GroupBy(a => a.AirportCode).ToList();
        //            var count = 1;
        //            foreach (var Tracking1 in Tracking2)
        //            {
        //                TTS.AirportCode = Tracking1.Key;
        //                var count1 = 0;
        //                foreach (var Tracking in Tracking1)
        //                {
        //                    var TS = new TradelaneTrackingStatus();
        //                    if (Tracking.AirportCode == APC && count == Tracking2.Count && count1 == 0)
        //                    {
        //                        TS.ShipmentCode = Tracking.TrackingCode;
        //                        TS.Pieces = Tracking.Pieces != null ? Tracking.Pieces.Value : 0;
        //                        TS.Date = Tracking.CreatedOnUtc != null ? Tracking.CreatedOnUtc : DateTime.MinValue;
        //                        TS.TotalWeight = Tracking.Weight != null ? Tracking.Weight.Value : 0;
        //                        TTS.TrackingStatus.Add(TS);
        //                    }
        //                    if (Tracking.AirportCode == APC && count == 1 && count1 == Tracking1.Count())
        //                    {
        //                        TS.ShipmentCode = Tracking.TrackingCode;
        //                        TS.Pieces = Tracking.Pieces != null ? Tracking.Pieces.Value : 0;
        //                        TS.Date = Tracking.CreatedOnUtc != null ? Tracking.CreatedOnUtc : DateTime.MinValue;
        //                        TS.TotalWeight = Tracking.Weight != null ? Tracking.Weight.Value : 0;
        //                        TTS.TrackingStatus.Add(TS);
        //                    }
        //                    if (Tracking.AirportCode == APC && count == 2 && count1 == 0)
        //                    {
        //                        TS.ShipmentCode = Tracking.TrackingCode;
        //                        TS.Pieces = Tracking.Pieces != null ? Tracking.Pieces.Value : 0;
        //                        TS.Date = Tracking.CreatedOnUtc != null ? Tracking.CreatedOnUtc : DateTime.MinValue;
        //                        TS.TotalWeight = Tracking.Weight != null ? Tracking.Weight.Value : 0;
        //                        TTS.TrackingStatus.Add(TS);
        //                    }
        //                    if (Tracking.AirportCode == APC && count == 2 && count1 == Tracking1.Count())
        //                    {
        //                        TS.ShipmentCode = Tracking.TrackingCode;
        //                        TS.Pieces = Tracking.Pieces != null ? Tracking.Pieces.Value : 0;
        //                        TS.Date = Tracking.CreatedOnUtc != null ? Tracking.CreatedOnUtc : DateTime.MinValue;
        //                        TS.TotalWeight = Tracking.Weight != null ? Tracking.Weight.Value : 0;
        //                        TTS.TrackingStatus.Add(TS);
        //                    }
        //                    if (Tracking.AirportCode == APC && count == 3 && count1 == 0)
        //                    {
        //                        TS.ShipmentCode = Tracking.TrackingCode;
        //                        TS.Pieces = Tracking.Pieces != null ? Tracking.Pieces.Value : 0;
        //                        TS.Date = Tracking.CreatedOnUtc != null ? Tracking.CreatedOnUtc : DateTime.MinValue;
        //                        TS.TotalWeight = Tracking.Weight != null ? Tracking.Weight.Value : 0;
        //                        TTS.TrackingStatus.Add(TS);
        //                    }
        //                    if (Tracking.AirportCode == APC && count == 3 && count1 == Tracking1.Count())
        //                    {
        //                        TS.ShipmentCode = Tracking.TrackingCode;
        //                        TS.Pieces = Tracking.Pieces != null ? Tracking.Pieces.Value : 0;
        //                        TS.Date = Tracking.CreatedOnUtc != null ? Tracking.CreatedOnUtc : DateTime.MinValue;
        //                        TS.TotalWeight = Tracking.Weight != null ? Tracking.Weight.Value : 0;
        //                        TTS.TrackingStatus.Add(TS);
        //                    }

        //                    count1++;
        //                }
        //            }
        //            TSL.Add(TTS);
        //        }
        //    }
        //    return TSL;
        //}

        public int GetTradelaneTracking(string Number, string NumberType)
        {
            var ReturnObj = 0;
            if (NumberType == "MAWB")
            {
                var MawbRes = new TradelaneShipmentRepository().GetMawbStr(Number);
                var Mawb = MawbRes.Item1;
                var AirlineId = MawbRes.Item2;
                if (!string.IsNullOrEmpty(AirlineId))
                {
                    var Airline = dbContext.Airlines.Where(a => a.AirlineCode == AirlineId).FirstOrDefault();
                    var Shipment = dbContext.TradelaneShipments.Where(x => x.MAWB == Mawb && x.AirlineId == Airline.AirlineId).FirstOrDefault();
                    if (Shipment != null)
                    {
                        ReturnObj = Shipment.TradelaneShipmentId;
                    }
                }
                else
                {
                    var Shipment = dbContext.TradelaneShipments.Where(x => x.MAWB == Mawb).FirstOrDefault();
                    if (Shipment != null)
                    {
                        ReturnObj = Shipment.TradelaneShipmentId;
                    }
                }
            }
            else if (NumberType.Trim().Replace(" ", "") == "ShipmentReferenceNo")
            {
                var Result = dbContext.TradelaneShipments.Where(a => a.FrayteNumber == Number).FirstOrDefault();
                if (Result != null)
                    ReturnObj = Result.TradelaneShipmentId;

            }
            else if (NumberType.Trim().Replace(" ", "") == "HAWB")
            {
                var shipment = (from TSD in dbContext.TradelaneShipmentDetails
                                join TS in dbContext.TradelaneShipments
                                on TSD.TradelaneShipmentId equals TS.TradelaneShipmentId
                                where TSD.HAWB == Number
                                select new
                                {
                                    TS.TradelaneShipmentId

                                }).FirstOrDefault();
                if (shipment != null && shipment.TradelaneShipmentId > 0)
                {
                    ReturnObj = shipment.TradelaneShipmentId;
                }
            }
            return ReturnObj;
        }

        public List<TradelaneTracking> GetTracking(string Numbers)
        {
            List<TradelaneTracking> TrackingList = new List<TradelaneTracking>();
            List<TradelaneTrackingModel> TTModel = new List<TradelaneTrackingModel>();
            List<FrayteShipmentTracking> FSTList = new List<FrayteShipmentTracking>();
            List<FrayteShipmentTracking> FSTDBList = new List<FrayteShipmentTracking>();
            List<FrayteShipmentTracking> FSTBagList = new List<FrayteShipmentTracking>();

            var Result = !string.IsNullOrEmpty(Numbers) ? Numbers.Split(',') : null;
            if (Result != null && Result.Length > 0)
            {
                foreach (var res in Result)
                {
                    string res1 = string.Empty;
                    if (res.Contains("MNESX"))
                    {
                        res1 = res;
                    }
                    else
                    {
                        res1 = res.Trim().Replace(" ", "");
                    }

                    if (res1 != null)
                    {
                        if (res1.Length > 10)
                        {
                            var MawbRes = new TradelaneShipmentRepository().GetMawbStr(res1);
                            if (!string.IsNullOrEmpty(MawbRes.Item1) && !string.IsNullOrEmpty(MawbRes.Item2))
                            {
                                res1 = MawbRes.Item2 + " " + MawbRes.Item1.Substring(0, 4) + " " + MawbRes.Item1.Substring(4, 4);
                            }
                        }

                        var NewResult1 = new TrackingRepository().GetShipmentDetail(res1);

                        if (NewResult1 != null)
                        {
                            if (NewResult1.TrackingType == "awb")
                            {
                                //Express
                                var TrackingType = "External";
                                var awbtracking = new ExpresShipmentRepository().GetExpressAWBTracking(NewResult1.ShipmentId, TrackingType);
                                FSTList.Add(awbtracking);

                                TradelaneTracking tr = new TradelaneTracking();
                                tr.Status = true;
                                tr.ModuleType = "Express AWB";
                                tr.ExpressTracking = new List<FrayteShipmentTracking>();
                                tr.ExpressTracking = FSTList;
                                TrackingList.Add(tr);
                            }
                            else if (NewResult1.TrackingType == "bag")
                            {
                                //Express
                                var TrackingType = "External";
                                var bagtracking = new ExpresShipmentRepository().GetExpressBagTracking(NewResult1.ShipmentId, TrackingType);
                                FSTBagList.Add(bagtracking);

                                TradelaneTracking tr = new TradelaneTracking();
                                tr.Status = true;
                                tr.ModuleType = "Express BAG";
                                tr.ExpressTracking = new List<FrayteShipmentTracking>();
                                tr.ExpressTracking = FSTBagList;
                                TrackingList.Add(tr);
                            }
                            else if (NewResult1.TrackingType == "exptn")
                            {
                                //Express
                                var TrackingType = "External";
                                var awbtracking = new ExpresShipmentRepository().GetExpressAWBTracking(NewResult1.ShipmentId, TrackingType);
                                FSTList.Add(awbtracking);

                                TradelaneTracking tr = new TradelaneTracking();
                                tr.Status = true;
                                tr.ModuleType = "Express AWB";
                                tr.ExpressTracking = new List<FrayteShipmentTracking>();
                                tr.ExpressTracking = FSTList;
                                TrackingList.Add(tr);
                            }
                            else if (NewResult1.TrackingType == "expmn")
                            {
                                //Express
                                TradelaneTrackingModel TM = GetTradelaneShipmentTracking(NewResult1.ShipmentId);
                                TTModel.Add(TM);

                                TradelaneTracking tr = new TradelaneTracking();
                                tr.Status = true;
                                tr.ModuleType = "Express";
                                tr.Trackingmodel = new List<TradelaneTrackingModel>();
                                tr.Trackingmodel = TTModel;
                                TrackingList.Add(tr);
                            }
                            else if (NewResult1.TrackingType == "mawb")
                            {
                                //Tradelane
                                TradelaneTrackingModel TM = GetTradelaneShipmentTracking(NewResult1.ShipmentId);
                                TTModel.Add(TM);

                                TradelaneTracking tr = new TradelaneTracking();
                                tr.Status = true;
                                tr.ModuleType = "Tradelane";
                                tr.Trackingmodel = new List<TradelaneTrackingModel>();
                                tr.Trackingmodel = TTModel;
                                TrackingList.Add(tr);
                            }
                            else if (NewResult1.TrackingType == "tlfrn")
                            {
                                //Tradelane
                                TradelaneTrackingModel TM = GetTradelaneShipmentTracking(NewResult1.ShipmentId);
                                TTModel.Add(TM);

                                TradelaneTracking tr = new TradelaneTracking();
                                tr.Status = true;
                                tr.ModuleType = "Tradelane";
                                tr.Trackingmodel = new List<TradelaneTrackingModel>();
                                tr.Trackingmodel = TTModel;
                                TrackingList.Add(tr);
                            }
                            else if (NewResult1.TrackingType == "hawb")
                            {
                                //Tradelane
                                TradelaneTrackingModel TM = GetTradelaneShipmentTracking(NewResult1.ShipmentId);
                                TTModel.Add(TM);

                                TradelaneTracking tr = new TradelaneTracking();
                                tr.Status = true;
                                tr.ModuleType = "Tradelane";
                                tr.Trackingmodel = new List<TradelaneTrackingModel>();
                                tr.Trackingmodel = TTModel;
                                TrackingList.Add(tr);
                            }
                            else if (NewResult1.TrackingType == "dbtn")
                            {
                                //DirectBooking
                                var TrackingType = dbContext.DirectShipments.Where(a => a.DirectShipmentId == NewResult1.ShipmentId).FirstOrDefault();
                                var TrackingNumber = TrackingType.TrackingDetail.Replace("Order_", "");
                                var TM = new AftershipTrackingRepository().GetTracking(TrackingType.LogisticServiceType, TrackingNumber);
                                FSTDBList.Add(TM);

                                TradelaneTracking tr = new TradelaneTracking();
                                tr.Status = true;
                                tr.ModuleType = "DirectBooking";
                                tr.BagTracking = new List<FrayteShipmentTracking>();
                                tr.BagTracking = FSTDBList;
                                TrackingList.Add(tr);
                            }
                            else if (NewResult1.TrackingType == "dbfn")
                            {
                                //DirectBooking
                                var TrackingType = dbContext.DirectShipments.Where(a => a.DirectShipmentId == NewResult1.ShipmentId).FirstOrDefault();
                                var TrackingNumber = TrackingType.TrackingDetail.Replace("Order_", "");
                                var TM = new AftershipTrackingRepository().GetTracking(TrackingType.LogisticServiceType, TrackingNumber);
                                FSTDBList.Add(TM);

                                TradelaneTracking tr = new TradelaneTracking();
                                tr.Status = true;
                                tr.ModuleType = "DirectBooking";
                                tr.BagTracking = new List<FrayteShipmentTracking>();
                                tr.BagTracking = FSTDBList;
                                TrackingList.Add(tr);
                            }
                            else if (NewResult1.TrackingType == "dbIspcs")
                            {
                                //DirectBooking
                                var TrackingType = dbContext.DirectShipments.Where(a => a.DirectShipmentId == NewResult1.ShipmentId).FirstOrDefault();
                                var TrackingNumber = TrackingType.TrackingDetail.Replace("Order_", "");
                                var TM = new AftershipTrackingRepository().GetTracking(TrackingType.LogisticServiceType, TrackingNumber);
                                FSTDBList.Add(TM);

                                TradelaneTracking tr = new TradelaneTracking();
                                tr.Status = true;
                                tr.ModuleType = "DirectBooking";
                                tr.BagTracking = new List<FrayteShipmentTracking>();
                                tr.BagTracking = FSTDBList;
                                TrackingList.Add(tr);
                            }
                            else
                            {
                                TradelaneTracking tr = new TradelaneTracking();
                                tr.Status = false;
                                tr.ModuleType = null;
                                tr.Tracking = null;
                                tr.BagTracking = null;
                                tr.ExpressTracking = null;
                                tr.Trackingmodel = null;
                                TrackingList.Add(tr);
                            }
                        }
                        else
                        {
                            TradelaneTracking tr = new TradelaneTracking();
                            tr.Status = false;
                            tr.ModuleType = null;
                            tr.Tracking = null;
                            tr.BagTracking = null;
                            tr.ExpressTracking = null;
                            tr.Trackingmodel = null;
                            TrackingList.Add(tr);
                        }
                    }
                }
            }
            return TrackingList;
        }
    }
}
