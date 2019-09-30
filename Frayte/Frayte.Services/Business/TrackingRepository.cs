using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using Frayte.Services.DataAccess;

namespace Frayte.Services.Business
{
    public class TrackingRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public TrackingModel GetShipmentDetail(string ModuleType, string TrackingType, string FrayteNumber)
        {
            TrackingModel TGS = new TrackingModel();
            TradelaneShipment Shipment;

            if (!string.IsNullOrEmpty(FrayteNumber) && ModuleType == FrayteShipmentServiceType.TradeLaneBooking)
            {
                if (TrackingType == "FrayteNumber")
                {
                    Shipment = dbContext.TradelaneShipments.Where(x => x.FrayteNumber == FrayteNumber).FirstOrDefault();
                    if (Shipment != null && Shipment.TradelaneShipmentId > 0)
                    {
                        TGS.ModuleType = FrayteShipmentServiceType.TradeLaneBooking;
                        TGS.ShipmentId = Shipment.TradelaneShipmentId;
                        TGS.TrackingType = "frn";
                        goto End;
                    }
                }
                if (TrackingType == "MAWB")
                {
                    var MawbRes = new TradelaneShipmentRepository().GetMawbStr(FrayteNumber);
                    if (!string.IsNullOrEmpty(MawbRes.Item1) && !string.IsNullOrEmpty(MawbRes.Item2))
                    {
                        var Airline = dbContext.Airlines.Where(a => a.AirlineCode == MawbRes.Item2).FirstOrDefault();
                        Shipment = dbContext.TradelaneShipments.Where(x => x.MAWB == MawbRes.Item1 && x.AirlineId == Airline.AirlineId).FirstOrDefault();
                        if (Shipment != null)
                        {
                            TGS.ModuleType = FrayteShipmentServiceType.TradeLaneBooking;
                            TGS.ShipmentId = Shipment.TradelaneShipmentId;
                            TGS.TrackingType = "mawb";
                            goto End;
                        }
                    }
                    else
                    {
                        Shipment = dbContext.TradelaneShipments.Where(x => x.MAWB == MawbRes.Item1).FirstOrDefault();
                        if (Shipment != null)
                        {
                            TGS.ModuleType = FrayteShipmentServiceType.TradeLaneBooking;
                            TGS.ShipmentId = Shipment.TradelaneShipmentId;
                            TGS.TrackingType = "mawb";
                            goto End;
                        }
                    }
                }

                var shipment = (from TSD in dbContext.TradelaneShipmentDetails
                                join TS in dbContext.TradelaneShipments
                                on TSD.TradelaneShipmentId equals TS.TradelaneShipmentId
                                where TSD.HAWB == FrayteNumber
                                select new
                                {
                                    TS.TradelaneShipmentId
                                }).FirstOrDefault();
                if (shipment != null && shipment.TradelaneShipmentId > 0)
                {
                    TGS.ModuleType = FrayteShipmentServiceType.TradeLaneBooking;
                    TGS.ShipmentId = shipment.TradelaneShipmentId;
                    TGS.TrackingType = "hawb";
                }
            }
            if (!string.IsNullOrEmpty(FrayteNumber) && string.IsNullOrEmpty(TGS.ModuleType) && string.IsNullOrEmpty(TGS.TrackingType))
            {
                var shipment = (from DSPD in dbContext.PackageTrackingDetails
                                join DSD in dbContext.DirectShipmentDetails
                                on DSPD.DirectShipmentDetailId equals DSD.DirectShipmentDetailId
                                join DS in dbContext.DirectShipments
                                on DSD.DirectShipmentId equals DS.DirectShipmentId
                                where DSPD.TrackingNo == FrayteNumber || DS.TrackingDetail.Contains(FrayteNumber)
                                select new
                                {
                                    DS.TrackingDetail,
                                    DS.LogisticServiceType
                                }).FirstOrDefault();
                if (shipment != null && !string.IsNullOrEmpty(shipment.TrackingDetail))
                {
                    TGS.ModuleType = FrayteShipmentServiceType.DirectBooking;
                    TGS.TrackingType = "TrackingNo";
                    TGS.CarrierType = shipment.LogisticServiceType;
                    TGS.Number = shipment.TrackingDetail.Replace("Order_", "");
                    goto End;
                }
                var Result1 = dbContext.DirectShipments.Where(a => a.FrayteNumber == FrayteNumber).FirstOrDefault();
                if (Result1 != null)
                {
                    TGS.ModuleType = FrayteShipmentServiceType.DirectBooking;
                    TGS.TrackingType = "FrayteNumber";
                    TGS.CarrierType = Result1.LogisticServiceType;
                    TGS.Number = Result1.TrackingDetail.Replace("Order_", "");
                    goto End;
                }
            }
            End:
            return TGS;
        }

        public TrackingModel GetShipmentDetail(string FrayteNumber)
        {
            TrackingModel TGS = new TrackingModel();

            var Result = dbContext.TrackingNumberRoutes.Where(a => a.Number == FrayteNumber).FirstOrDefault();
            if (Result != null)
            {
                TGS.Number = Result.Number;
                TGS.ModuleType = Result.ModuleType;
                TGS.ShipmentId = Result.ShipmentId;
                //Tradelane Shipment
                if (Result.IsTradelaneRefNumber == true)
                {
                    TGS.TrackingType = "tlfrn";
                    goto End;
                }
                if (Result.IsMAWB == true)
                {
                    TGS.TrackingType = "mawb";
                    goto End;
                }
                if (Result.IsHAWB == true)
                {
                    TGS.TrackingType = "hawb";
                    goto End;
                }
                //Express Shipment
                if (Result.IsAWB == true)
                {
                    TGS.TrackingType = "awb";
                    goto End;
                }
                if (Result.IsBag == true)
                {
                    TGS.TrackingType = "bag";
                    goto End;
                }
                if (Result.IsExpressManifestNumber == true)
                {
                    TGS.TrackingType = "expmn";
                    goto End;
                }
                if(Result.IsTrackingNumber == true)
                {
                    TGS.TrackingType = "exptn";
                    goto End;
                }

                //DirectBooking Shipment
                if (Result.IsTrackingNumber == true)
                {
                    TGS.TrackingType = "dbtn";
                    goto End;
                }
                if (Result.IsFrayteNumber == true)
                {
                    TGS.TrackingType = "dbfn";
                    goto End;
                }
                if (Result.IsPiecesTrackingNo == true)
                {
                    TGS.TrackingType = "dbIspcs";
                    goto End;
                }
            }
            else
            {
                goto End;                
            }
            End:
            return TGS;
        }

        public TrackingModel ShipmentFilter(string FrayteNumber)
        {
            TrackingModel TGS = new TrackingModel();

            if (!string.IsNullOrEmpty(FrayteNumber))
            {
                if (FrayteNumber.ToUpper().Contains("TL"))
                {
                    TGS.ModuleType = FrayteShipmentServiceType.TradeLaneBooking;
                    TGS.TrackingType = "FrayteNumber";
                }
                else if (!FrayteNumber.ToUpper().Contains("TL") && FrayteNumber.Length == 8)
                {
                    TGS.ModuleType = FrayteShipmentServiceType.TradeLaneBooking;
                    TGS.TrackingType = "MAWB";
                }
                else if (!FrayteNumber.ToUpper().Contains("TL") && FrayteNumber.Length > 10)
                {
                    var MawbRes = new TradelaneShipmentRepository().GetMawbStr(FrayteNumber);
                    if (MawbRes != null && !string.IsNullOrEmpty(MawbRes.Item2))
                    {
                        TGS.ModuleType = FrayteShipmentServiceType.TradeLaneBooking;
                        TGS.TrackingType = "MAWB";
                    }
                    else
                    {
                        TGS.ModuleType = FrayteShipmentServiceType.DirectBooking;
                        TGS.TrackingType = "";
                    }
                }
                else
                {
                    TGS.ModuleType = FrayteShipmentServiceType.DirectBooking;
                    TGS.TrackingType = "";
                }
            }
            return TGS;
        }

        #region Offile Parcel

        public List<FrayteOfflineTracking> MapDirectShipmentObjTacking(int directShipmentid, DirectBookingShipmentDraftDetail directBooking)
        {
            List<FrayteOfflineTracking> list = new List<FrayteOfflineTracking>();
            FrayteOfflineTracking track;
            if (directBooking != null)
            {
                track = new FrayteOfflineTracking();
                track.ShipmentId = directShipmentid;
                track.Message = "Shipment Received - Offline Parcel";
                track.CreatedOn = DateTime.UtcNow;
                track.Location = directBooking.ShipFrom.Country.Name;

                list.Add(track);

                if (directBooking.ReferenceDetail.CollectionDate.HasValue && !string.IsNullOrEmpty(directBooking.ReferenceDetail.CollectionTime))
                {
                    track = new FrayteOfflineTracking();
                    track.ShipmentId = directShipmentid;
                    track.Message = "Scheduled Pickup on " + directBooking.ReferenceDetail.CollectionDate.Value.ToString("dd-MMM-yyyy")
                    + " at " + directBooking.ReferenceDetail.CollectionDate.Value.TimeOfDay.ToString("hh\\:mm");
                    track.CreatedOn = DateTime.UtcNow;
                    track.Location = directBooking.ShipFrom.Country.Name;

                    list.Add(track);
                }
            }

            return list;
        }

        public void UpdateOfflineTracking(List<FrayteOfflineTracking> frayteOfflineTracking)
        {
            if (frayteOfflineTracking != null && frayteOfflineTracking.Count > 0)
            {
                DirectShipmentTracking tracking;
                foreach (var item in frayteOfflineTracking)
                {
                    tracking = new DirectShipmentTracking();
                    tracking.DirectShipmentId = item.ShipmentId;
                    tracking.CreatedOnUtc = DateTime.UtcNow;
                    tracking.Location = item.Location;
                    tracking.TrackingDescription = item.Message;

                    dbContext.DirectShipmentTrackings.Add(tracking);
                    dbContext.SaveChanges();
                }
            }
        }

        #endregion
    }
}
