using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AftershipAPI;
using Newtonsoft.Json.Linq;
using Frayte.Services.DataAccess;
using Frayte.Services.Models.Aftership;
using Frayte.Services.Utility;
using Frayte.Services.Models.AU;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Globalization;
using Frayte.Services.Models.SKYPOSTAL;

namespace Frayte.Services.Business
{
    public class AftershipTrackingRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        #region Get Tracking Detail for single tracking number from Aftership

        public FrayteShipmentTracking GetTracking(string carrierName, string trackingNumber)
        {
            try
            {
                var logisticIntegration = dbContext.LogisticIntegrations.Where(p => p.Name == FrayteIntegration.Aftership).FirstOrDefault();
                
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
                        track = track.Status == false ? GetDBTracking(trackingNumber, carrierName) : AddOfflineStatus(track, trackingNumber, carrierName);
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
                        track = track.Status == false ? GetDBTracking(trackingNumber, carrierName) : AddOfflineStatus(track, trackingNumber, carrierName);
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
                        track = track.Status == false ? GetDBTracking(trackingNumber, carrierName) : AddOfflineStatus(track, trackingNumber, carrierName);
                        return track;
                    }
                    else if (carrierName.Contains("DPD") || carrierName.Contains("dpd") || carrierName.Contains("DPDEXPRESS"))
                    {
                        tracking = new AftershipAPI.Tracking(trackingNumber);
                        tracking.slug = "dpd-uk";
                        FrayteShipmentTracking frayteTracking = new FrayteShipmentTracking();
                        AftershipAPI.ConnectionAPI connApi = new AftershipAPI.ConnectionAPI(logisticIntegration.InetgrationKey, logisticIntegration.Url);
                        tracking = connApi.getTrackingByNumber(tracking);
                        trackings.Add(tracking);
                        FrayteShipmentTracking track = mapAftershiptrackingToFrayteTracking(trackings);
                        track = track.Status == false ? GetDBTracking(trackingNumber, carrierName) : AddOfflineStatus(track, trackingNumber, carrierName);
                        return track;
                    }
                    else if (carrierName.Contains("AU") || carrierName.Contains("au"))
                    {
                        var result = new AURepository().AUTrackingWebApiCalling(trackingNumber);

                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            result = result.Replace("\\r\\n ", "");
                            result = result.Replace(@"""", "");
                            var xmltag = "<?xml version=" + "'1.0'" + " encoding=" + "'UTF-8'" + " ?>";
                            result = xmltag + result;
                        }
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(result);
                        var xml = XDocument.Parse(@result);
                        var autrack = new List<AUTrackingModel>();

                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            var data = (from r in xml.Descendants("Table")
                                        select new Trackeventdetails
                                        {
                                            invoice_number = r.Element("Invoice_Number") != null ? r.Element("Invoice_Number").Value : "",
                                            barcode = r.Element("Barcode") != null ? r.Element("Barcode").Value : "",
                                            trackeventdetails = r.Element("trackeventdetails") != null ? r.Element("trackeventdetails").Value : "",
                                            trackeventdateoccured = r.Element("trackeventdateoccured") != null ? r.Element("trackeventdateoccured").Value : "",
                                            city = r.Element("city") != null ? r.Element("city").Value : "",
                                            country = r.Element("country") != null ? r.Element("country").Value : "",

                                        }).ToList();

                            if (data != null)
                            {
                                AUTrackingModel tack = new AUTrackingModel();
                                tack.table = new List<Trackeventdetails>();
                                tack.table = data;
                                tack.Status = true;
                                autrack.Add(tack);
                            }
                        }
                        FrayteShipmentTracking track = mapAutrackingToFrayteTracking(autrack);

                        track = track.Status == false ? GetDBTracking(trackingNumber, carrierName) : AddOfflineStatus(track, trackingNumber, carrierName);
                        return track;

                    }
                    else if (carrierName.Contains("SKYPOSTAL") || carrierName.Contains("SkyPostal"))
                    {
                        var autrack = new List<SkyPostalTrackingResponseModel>();
                        SkyPostalTrackingModel request = new SkyPostalRepository().MappingTracking(trackingNumber);
                        SkyPostalTrackingResponseModel reponse = new SkyPostalRepository().SkyPostalTrackWebApiCalling(request);
                        if (reponse.success == 1)
                        {
                            autrack.Add(reponse);
                            FrayteShipmentTracking track = mapSkyPostaltrackingToFrayteTracking(autrack);

                            track = track.Status == false ? GetDBTracking(trackingNumber, carrierName) : AddOfflineStatus(track, trackingNumber, carrierName);
                            return track;
                        }
                        else
                        {
                            return GetDBTracking(trackingNumber, carrierName);
                        }
                    }
                    else
                    {
                        var logisticServiceDetail = (from r in dbContext.DirectShipments
                                                     join ca in dbContext.LogisticServiceCourierAccounts on r.CourierAccountId equals ca.LogisticServiceCourierAccountId
                                                     join ls in dbContext.LogisticServices on ca.LogisticServiceId equals ls.LogisticServiceId
                                                     where r.TrackingDetail == trackingNumber
                                                     select ls
                                                      ).FirstOrDefault();

                        tracking = new AftershipAPI.Tracking(trackingNumber);

                        if (logisticServiceDetail.LogisticCompany.ToLower().Contains("yodel"))
                        {
                            tracking.slug = FrayteCourierSlugs.Yodel;
                        }
                        else if (logisticServiceDetail.LogisticCompany.ToLower().Contains("hermes"))
                        {
                            tracking.slug = FrayteCourierSlugs.Hermese;
                        }
                        else if (logisticServiceDetail.LogisticCompany.ToLower().Contains("ukmail"))
                        {
                            tracking.slug = FrayteCourierSlugs.UkMail;
                        }
                        else
                        {
                            tracking.slug = string.Empty;
                        }

                        FrayteShipmentTracking frayteTracking = new FrayteShipmentTracking();
                        AftershipAPI.ConnectionAPI connApi = new AftershipAPI.ConnectionAPI(logisticIntegration.InetgrationKey, logisticIntegration.Url);
                        tracking = connApi.getTrackingByNumber(tracking);
                        trackings.Add(tracking);
                        FrayteShipmentTracking track = mapAftershiptrackingToFrayteTracking(trackings);
                        track = track.Status == false ? GetDBTracking(trackingNumber, carrierName) : AddOfflineStatus(track, trackingNumber, carrierName);
                        return track;
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                return GetDBTracking(trackingNumber, carrierName);
            }
        }

        public FrayteShipmentTracking GetExpressTracking(string carrierName, string trackingNumber)
        {
            try
            {
                var logisticIntegration = dbContext.LogisticIntegrations.Where(p => p.Name == FrayteIntegration.Aftership).FirstOrDefault();
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
                    else if (carrierName.Contains("DPD") || carrierName.Contains("dpd") || carrierName.Contains("DPDEXPRESS"))
                    {
                        tracking = new AftershipAPI.Tracking(trackingNumber);
                        tracking.slug = "dpd-uk";
                        FrayteShipmentTracking frayteTracking = new FrayteShipmentTracking();
                        AftershipAPI.ConnectionAPI connApi = new AftershipAPI.ConnectionAPI(logisticIntegration.InetgrationKey, logisticIntegration.Url);
                        tracking = connApi.getTrackingByNumber(tracking);
                        trackings.Add(tracking);
                        FrayteShipmentTracking track = mapAftershiptrackingToFrayteTracking(trackings);
                        return track;
                    }
                    else if (carrierName.Contains("AU") || carrierName.Contains("au"))
                    {
                        var result = new AURepository().AUTrackingWebApiCalling(trackingNumber);

                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            result = result.Replace("\\r\\n ", "");
                            result = result.Replace(@"""", "");
                            var xmltag = "<?xml version=" + "'1.0'" + " encoding=" + "'UTF-8'" + " ?>";
                            result = xmltag + result;
                        }
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(result);
                        var xml = XDocument.Parse(@result);
                        var autrack = new List<AUTrackingModel>();

                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            var data = (from r in xml.Descendants("Table")
                                        select new Trackeventdetails
                                        {
                                            invoice_number = r.Element("Invoice_Number") != null ? r.Element("Invoice_Number").Value : "",
                                            barcode = r.Element("Barcode") != null ? r.Element("Barcode").Value : "",
                                            trackeventdetails = r.Element("trackeventdetails") != null ? r.Element("trackeventdetails").Value : "",
                                            trackeventdateoccured = r.Element("trackeventdateoccured") != null ? r.Element("trackeventdateoccured").Value : "",
                                            city = r.Element("city") != null ? r.Element("city").Value : "",
                                            country = r.Element("country") != null ? r.Element("country").Value : "",

                                        }).ToList();

                            if (data != null)
                            {
                                AUTrackingModel tack = new AUTrackingModel();
                                tack.table = new List<Trackeventdetails>();
                                tack.table = data;
                                tack.Status = true;
                                autrack.Add(tack);
                            }
                        }
                        FrayteShipmentTracking track = mapAutrackingToFrayteTracking(autrack);
                        return track;
                    }
                    else if (carrierName.Contains("SKYPOSTAL") || carrierName.Contains("SkyPostal"))
                    {
                        var autrack = new List<SkyPostalTrackingResponseModel>();
                        SkyPostalTrackingModel request = new SkyPostalRepository().MappingTracking(trackingNumber);
                        SkyPostalTrackingResponseModel reponse = new SkyPostalRepository().SkyPostalTrackWebApiCalling(request);
                        if (reponse.success == 1)
                        {
                            autrack.Add(reponse);
                            FrayteShipmentTracking track = mapSkyPostaltrackingToFrayteTracking(autrack);
                            return track;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        var logisticServiceDetail = (from r in dbContext.DirectShipments
                                                     join ca in dbContext.LogisticServiceCourierAccounts on r.CourierAccountId equals ca.LogisticServiceCourierAccountId
                                                     join ls in dbContext.LogisticServices on ca.LogisticServiceId equals ls.LogisticServiceId
                                                     where r.TrackingDetail == trackingNumber
                                                     select ls
                                                      ).FirstOrDefault();

                        tracking = new AftershipAPI.Tracking(trackingNumber);

                        if (logisticServiceDetail.LogisticCompany.ToLower().Contains("yodel"))
                        {
                            tracking.slug = FrayteCourierSlugs.Yodel;
                        }
                        else if (logisticServiceDetail.LogisticCompany.ToLower().Contains("hermes"))
                        {
                            tracking.slug = FrayteCourierSlugs.Hermese;
                        }
                        else if (logisticServiceDetail.LogisticCompany.ToLower().Contains("ukmail"))
                        {
                            tracking.slug = FrayteCourierSlugs.UkMail;
                        }
                        else
                        {
                            tracking.slug = string.Empty;
                        }

                        FrayteShipmentTracking frayteTracking = new FrayteShipmentTracking();
                        AftershipAPI.ConnectionAPI connApi = new AftershipAPI.ConnectionAPI(logisticIntegration.InetgrationKey, logisticIntegration.Url);
                        tracking = connApi.getTrackingByNumber(tracking);
                        trackings.Add(tracking);
                        FrayteShipmentTracking track = mapAftershiptrackingToFrayteTracking(trackings);
                        return track;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #endregion

        #region Send Status Email on WebHook

        public FrayteResult SendTrackingStatusEmail(AftershipWebhookObject webHookDetail)
        {
            FrayteResult result = new FrayteResult();

            if (webHookDetail != null)
            {
                var logisticIntegration = dbContext.LogisticIntegrations.Where(p => p.Name == FrayteIntegration.Aftership).FirstOrDefault();

                FrayteAftershipmentTrackingEmail obj = new FrayteAftershipmentTrackingEmail();
                if (logisticIntegration != null)
                {
                    obj.TrackingLink = logisticIntegration.VoidApiUrl;
                }
                obj.ts = webHookDetail.ts;
                obj.@event = webHookDetail.@event;
                obj.msg = new Msg();
                obj.msg = webHookDetail.msg;
                obj.msg.checkpoints = webHookDetail.msg.checkpoints.OrderByDescending(p => p.created_at).ThenByDescending(p => p.checkpoint_time).ToList();

                result = new ShipmentEmailRepository().SendAftershipTrackingEmail(obj, webHookDetail.msg.tag);
            }
            else
            {

            }
            return result;
        }

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
                                    track.Date = DateTime.ParseExact(str[0], "MM/dd/yyyy", CultureInfo.InvariantCulture);
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

        private FrayteShipmentTracking mapAutrackingToFrayteTracking(List<AUTrackingModel> trackers)
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
                        int count = tracker.table.Count;
                        trackingDetail = new ShipmentTracking();
                        trackingDetail.TrackingDetails = new List<ShipmentTrackingDetail>();
                        trackingDetail.DestinationCountry = tracker.table[0].country;
                        trackingDetail.OriginCountry = "AU";
                        trackingDetail.Title = "";
                        trackingDetail.CarriertrackingId = tracker.table[0].invoice_number;
                        trackingDetail.IsHeaderShow = true;
                        trackingDetail.TrackingNumber = tracker.table[0].invoice_number;
                        trackingDetail.ShowHideValue = "Hide";
                        trackingDetail.Carrier = "AU";
                        trackingDetail.StatusId = 1;
                        trackingDetail.StatusDisplay = tracker.table[0].trackeventdetails;
                        trackingDetail.Status = tracker.table[0].trackeventdetails;
                        var CreateDateTime = tracker.table[tracker.table.Count - 1].trackeventdateoccured.Split(' ');
                        if (!string.IsNullOrEmpty(tracker.table[0].trackeventdateoccured))
                        {
                            //trackingDetail.EstimatedDeliveryDate = tracker.checkpoints.Last().checkpointTime;
                            trackingDetail.EstimatedDeliveryDate = ""; //tracker.expectedDelivery.ToString("MM/dd/yyyy");
                            //var date = tracker.est_delivery_date.Value;
                            trackingDetail.EstimatedDeliveryTime = "";//tracker.est_delivery_date.Value.ToString("HH:MM");
                        }
                        trackingDetail.EstimatedWeight = 0; // tracker.w;
                        trackingDetail.SignedBy = "";
                        trackingDetail.CreatedAtDate = CreateDateTime[0];
                        trackingDetail.CreatedAtTime = CreateDateTime[1];
                        trackingDetail.TrackingDetails = new List<ShipmentTrackingDetail>();
                        if (tracker.table != null && tracker.table.Count > 0)
                        {
                            foreach (var trackerdetail in tracker.table)
                            {
                                var str = trackerdetail.trackeventdateoccured.Split(' ');
                                var time = str[1];
                                var track = new ShipmentTrackingDetail();
                                StringBuilder sb = new StringBuilder();
                                if (!string.IsNullOrEmpty(trackerdetail.city))
                                    sb.Append(trackerdetail.city);

                                track.IsCollapsed = false;
                                if (!string.IsNullOrWhiteSpace((str[0])))
                                {
                                    string date1 = str[0];
                                    DateTime date = DateTime.ParseExact(date1, "dd/MM/yyyy", null);
                                    track.Date = date;
                                }
                                else
                                {

                                }
                                track.Time = time;
                                track.Tag = "";
                                track.Activity = trackerdetail.trackeventdetails;
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
                            var UpdatDate = tracker.table[0].trackeventdateoccured.Split(' ');

                            trackingDetail.UpdatedAtDate = UpdatDate[0];
                            trackingDetail.UpdatedAtTime = UpdatDate[1];
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

        private FrayteShipmentTracking mapSkyPostaltrackingToFrayteTracking(List<SkyPostalTrackingResponseModel> trackers)
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
                        int count = tracker.response.Count;
                        trackingDetail = new ShipmentTracking();
                        trackingDetail.TrackingDetails = new List<ShipmentTrackingDetail>();
                        trackingDetail.DestinationCountry = tracker.response[count - 1].CTRY_NAME;
                        trackingDetail.OriginCountry = tracker.response[count - 1].CITY_NAME;
                        trackingDetail.Title = "";
                        trackingDetail.CarriertrackingId = "";
                        trackingDetail.IsHeaderShow = true;
                        trackingDetail.TrackingNumber = tracker.response[count - 1].EXT_TRACK;
                        trackingDetail.ShowHideValue = "Hide";
                        trackingDetail.Carrier = tracker.response[count - 1].TPSH_CDG;
                        trackingDetail.StatusId = 1;
                        trackingDetail.StatusDisplay = tracker.response[count - 1].STATUS;
                        trackingDetail.Status = tracker.response[count - 1].STATUS;
                        var CreateDateTime = tracker.response[count - 1].SHIP_DATE_PROCESS;
                        if (!string.IsNullOrEmpty(tracker.response[count - 1].CUSTOM_DATE_PROCESS.ToString()))
                        //if (!string.IsNullOrEmpty(tracker.checkpoints.Last().checkpointTime))
                        {
                            //trackingDetail.EstimatedDeliveryDate = tracker.checkpoints.Last().checkpointTime;
                            trackingDetail.EstimatedDeliveryDate = tracker.response[count - 1].CUSTOM_DATE_PROCESS.ToString("MM/dd/yyyy");
                            //var date = tracker.est_delivery_date.Value;
                            trackingDetail.EstimatedDeliveryTime = tracker.response[count - 1].CUSTOM_DATE_PROCESS.ToString("HH:MM");
                        }
                        trackingDetail.EstimatedWeight = 0; // tracker.w;
                        trackingDetail.SignedBy = "";
                        trackingDetail.CreatedAtDate = CreateDateTime.Date.ToString("MM/dd/yyyy");
                        trackingDetail.CreatedAtTime = CreateDateTime.ToString("HH:MM");
                        trackingDetail.TrackingDetails = new List<ShipmentTrackingDetail>();
                        if (tracker.response != null && tracker.response.Count > 0)
                        {
                            foreach (var trackerdetail in tracker.response)
                            {

                                var track = new ShipmentTrackingDetail();
                                StringBuilder sb = new StringBuilder();
                                if (!string.IsNullOrEmpty(trackerdetail.LOCALITY))
                                    sb.Append(trackerdetail.LOCALITY);
                                var date = DateTime.Now;
                                track.IsCollapsed = false;
                                if (DateTime.TryParse(trackerdetail.CUSTOM_DATE_PROCESS.ToString("MM/dd/yyyy"), out date))
                                {
                                    track.Date = date;
                                }
                                else
                                {

                                }
                                track.Time = trackerdetail.CUSTOM_DATE_PROCESS.ToString("HH:MM");
                                track.Tag = "";
                                track.Activity = trackerdetail.STATUS;
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
                            trackingDetail.UpdatedAtDate = tracker.response[count - 1].CUSTOM_DATE_PROCESS.ToString("MM/dd/yyyy");
                            trackingDetail.UpdatedAtTime = tracker.response[count - 1].CUSTOM_DATE_PROCESS.ToString("HH:MM");
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
                var userDetail = dbContext.Users.Find(customerId);
                if (userDetail != null)
                {
                    var logisticIntegration = dbContext.LogisticIntegrations.Where(p => p.Name == FrayteIntegration.Aftership).FirstOrDefault();

                    FrayteShipmentTracking frayteTracking = new FrayteShipmentTracking();
                    AftershipAPI.ConnectionAPI connApi = new AftershipAPI.ConnectionAPI(logisticIntegration.InetgrationKey, logisticIntegration.Url);
                    ParametersTracking ParametersTrackings = new ParametersTracking();
                    ParametersTrackings.keyword = userDetail.Email;
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
                tracking.slug = track.Slug;
                tracking.customFields = new Dictionary<string, string>();
                tracking.customFields.Add("customer_name", track.CustomerName);
                tracking.customFields.Add("customer_email", track.CustomerEmail);
                tracking.customFields.Add("user_customer_email", track.UserCustomerEmail);
                tracking.customFields.Add("module_type", track.ModuleType);
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

                #region DirectBooking

                if (moduleType == FrayteShipmentServiceType.DirectBooking)
                {
                    var detail = new DirectShipmentRepository().GetDirectBookingDetail(shipmentId, "");

                    var customerDetail = (from r in dbContext.DirectShipments
                                          join u in dbContext.Users on r.CustomerId equals u.UserId
                                          join ur in dbContext.UserRoles on u.UserId equals ur.UserId
                                          where r.DirectShipmentId == shipmentId
                                          select new
                                          {
                                              UserEmail = u.Email,
                                              CompanyName = u.CompanyName,
                                              RoleId = ur.RoleId
                                          }).FirstOrDefault();

                    var userCustomerDetail = (from r in dbContext.DirectShipments
                                              join u in dbContext.Users on r.CreatedBy equals u.UserId
                                              join ur in dbContext.UserRoles on u.UserId equals ur.UserId
                                              where r.DirectShipmentId == shipmentId
                                              select new
                                              {
                                                  UserEmail = u.Email,
                                                  RoleId = ur.RoleId
                                              }).FirstOrDefault();


                    tracking.TrackingNumber = detail.TrackingNo;
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
                    else if (detail.CustomerRateCard.CourierName == FrayteCourierCompany.Hermes)
                    {
                        tracking.Slug = FrayteCourierSlugs.Hermese;
                    }
                    else if (detail.CustomerRateCard.CourierName == FrayteCourierCompany.Yodel)
                    {
                        tracking.Slug = FrayteCourierSlugs.Yodel;
                    }
                    else if (detail.CustomerRateCard.CourierName == FrayteCourierCompany.UKMail)
                    {
                        tracking.Slug = FrayteCourierSlugs.UkMail;
                    }
                    else if (detail.CustomerRateCard.CourierName == FrayteCourierCompany.DPD)
                    {
                        tracking.Slug = FrayteCourierSlugs.DPD;
                    }

                    if (userCustomerDetail != null && userCustomerDetail.RoleId == (int)FrayteUserRole.UserCustomer)
                    {
                        tracking.UserCustomerEmail = userCustomerDetail.UserEmail;
                        tracking.CustomerEmail = string.Empty;
                    }
                    else
                    {
                        tracking.CustomerEmail = customerDetail.UserEmail;
                        tracking.UserCustomerEmail = string.Empty;
                    }

                    tracking.Title = detail.FrayteNumber;
                    tracking.CustomerName = customerDetail.CompanyName;
                    tracking.ModuleType = moduleType;
                }

                #endregion

                #region Express

                if (moduleType == FrayteShipmentServiceType.Express)
                {
                    FrayteAfterShipTracking tracking1 = new FrayteAfterShipTracking();
                    if (moduleType == FrayteShipmentServiceType.Express)
                    {
                        var detail = new ExpressRepository().ScannedShipmentDetail(shipmentId, "");

                        var customerDetail = (from r in dbContext.Expresses
                                              join u in dbContext.Users on r.CustomerId equals u.UserId
                                              join ur in dbContext.UserRoles on u.UserId equals ur.UserId
                                              where r.ExpressId == shipmentId
                                              select new
                                              {
                                                  UserEmail = u.Email,
                                                  CompanyName = u.CompanyName,
                                                  RoleId = ur.RoleId
                                              }).FirstOrDefault();

                        var userCustomerDetail = (from r in dbContext.DirectShipments
                                                  join u in dbContext.Users on r.CreatedBy equals u.UserId
                                                  join ur in dbContext.UserRoles on u.UserId equals ur.UserId
                                                  where r.DirectShipmentId == shipmentId
                                                  select new
                                                  {
                                                      UserEmail = u.Email,
                                                      RoleId = ur.RoleId
                                                  }).FirstOrDefault();

                        detail.Service = new ExpressRepository().GetShipmentService(detail.ExpressId);
                        if (detail.Service != null)
                        {
                            tracking.TrackingNumber = detail.TrackingNo;
                            if (detail.Service.HubCarrier == FrayteCourierCompany.DHL)
                            {
                                tracking.Slug = FrayteCourierSlugs.DHL;
                            }
                            else if (detail.Service.HubCarrier == FrayteCourierCompany.UPS)
                            {
                                tracking.Slug = FrayteCourierSlugs.UPS;
                            }
                            else if (detail.Service.HubCarrier == FrayteCourierCompany.TNT)
                            {
                                tracking.Slug = FrayteCourierSlugs.TNT;
                            }
                            else if (detail.Service.HubCarrier == FrayteCourierCompany.Hermes)
                            {
                                tracking.Slug = FrayteCourierSlugs.Hermese;
                            }
                            else if (detail.Service.HubCarrier == FrayteCourierCompany.Yodel)
                            {
                                tracking.Slug = FrayteCourierSlugs.Yodel;
                            }
                            else if (detail.Service.HubCarrier == FrayteCourierCompany.UKMail)
                            {
                                tracking.Slug = FrayteCourierSlugs.UkMail;
                            }
                            else if (detail.Service.HubCarrier == FrayteCourierCompany.DPD)
                            {
                                tracking.Slug = FrayteCourierSlugs.DPD;
                            }

                            if (userCustomerDetail != null && userCustomerDetail.RoleId == (int)FrayteUserRole.UserCustomer)
                            {
                                tracking.UserCustomerEmail = userCustomerDetail.UserEmail;
                                tracking.CustomerEmail = string.Empty;
                            }
                            else
                            {
                                tracking.CustomerEmail = customerDetail.UserEmail;
                                tracking.UserCustomerEmail = string.Empty;
                            }

                            tracking.Title = detail.FrayteNumber;
                            tracking.CustomerName = customerDetail.CompanyName;
                            tracking.ModuleType = moduleType;
                        }
                    }
                }

                #endregion

                return tracking;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        public FrayteShipmentTracking GetDBTracking(string tracking, string CarrierName)
        {
            try
            {
                var Result = (from DS in dbContext.DirectShipments
                              let DSHIP = DS.DirectShipmentId
                              join DSA in dbContext.DirectShipmentAddresses on DS.FromAddressId equals DSA.DirectShipmentAddressId
                              join DST in dbContext.DirectShipmentTrackings on DS.DirectShipmentId equals DST.DirectShipmentId into DSST
                              from DST in DSST.DefaultIfEmpty()
                              join LSCA in dbContext.LogisticServiceCourierAccounts on DS.CourierAccountId equals LSCA.LogisticServiceCourierAccountId into LSCAA
                              from LSCA in LSCAA.DefaultIfEmpty()
                              join LS in dbContext.LogisticServices on LSCA.LogisticServiceId equals LS.LogisticServiceId into LSS
                              from LS in LSS.DefaultIfEmpty()
                              where DS.TrackingDetail.Contains(tracking)
                              select new FrayteShipmentTracking
                              {
                                  Status = true,
                                  Tracking = new List<ShipmentTracking>()
                                    {
                                        new ShipmentTracking()
                                        {
                                            TrackingNumber = DS.TrackingDetail.Replace("Order_", ""),
                                            CreatedAt = DS.CreatedOn,
                                            CreatedAtDate = DS.CreatedOn.ToString(),
                                            UpdatedAtDate = DS.CreatedOn.ToString(),
                                            UpdatedAt = DS.CreatedOn,
                                            Status = "OffLine Booked",
                                            StatusDisplay = "OffLine Booked",
                                            Carrier = DS.LogisticServiceType + " " + LS.RateType,
                                            EstimatedDeliveryDate = DS.CollectionDate.Value.ToString(),
                                            EstimatedDeliveryTime = DS.CollectionTime.ToString(),
                                            //TrackingDetails = new List<ShipmentTrackingDetail>(),
                                            TrackingDetails = (from fr in dbContext.DirectShipmentTrackings
                                                                where fr.DirectShipmentId == DSHIP
                                                                select new ShipmentTrackingDetail
                                                                {
                                                                Activity = DST.TrackingDescription,
                                                                Location = DST.Location,
                                                                Date = DST.CreatedOnUtc != null ? DST.CreatedOnUtc : DateTime.UtcNow,
                                                                Time = ""
                                                            }).ToList(),
                                            IsHeaderShow = true,
                                            ShowHideValue = "Hide",
                                            IsPanelShow = true,
                                            IsShowHideDetail = true
                                        }
                                    }
                              }).FirstOrDefault();

                var res = (from DS in dbContext.DirectShipments
                           join DSA in dbContext.DirectShipmentAddresses on DS.FromAddressId equals DSA.DirectShipmentAddressId
                           where DS.TrackingDetail.Contains(tracking)
                           select new
                           {
                               DSA.DirectShipmentAddressId,
                               DSA.CountryId
                           }).FirstOrDefault();

                var TZ = new DirectShipmentRepository().TimeZoneDetail(res.CountryId);
                var TimezoneINformation = TimeZoneInfo.FindSystemTimeZoneById(TZ.Name);
                foreach (var res1 in Result.Tracking)
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
                return Result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public FrayteShipmentTracking AddOfflineStatus(FrayteShipmentTracking FST, string Tracking, string CarrierName)
        {
            var Shipment = dbContext.DirectShipments.Where(a => a.TrackingDetail.Contains(Tracking)).FirstOrDefault();
            if (Shipment != null)
            {
                var result = (from DS in dbContext.DirectShipments
                              join DSA in dbContext.DirectShipmentAddresses on DS.FromAddressId equals DSA.DirectShipmentAddressId
                              where DS.TrackingDetail.Contains(Tracking)
                              select new
                              {
                                  DSA.DirectShipmentAddressId,
                                  DSA.CountryId
                              }).FirstOrDefault();
                var Result = dbContext.DirectShipmentTrackings.Where(a => a.DirectShipmentId == Shipment.DirectShipmentId).ToList();
                if (Result.Count > 0)
                {
                    var TZ = new DirectShipmentRepository().TimeZoneDetail(result.CountryId);
                    var TimezoneINformation = TimeZoneInfo.FindSystemTimeZoneById(TZ.Name);
                    var Count = 0;
                    foreach (var res in Result)
                    {
                        ShipmentTrackingDetail STD = new ShipmentTrackingDetail();
                        STD.Activity = res.TrackingDescription;
                        STD.Location = res.Location;
                        STD.Date = UtilityRepository.UtcDateToOtherTimezone(res.CreatedOnUtc, res.CreatedOnUtc.TimeOfDay, TimezoneINformation).Item1.Date;
                        STD.Time = UtilityRepository.UtcDateToOtherTimezone(res.CreatedOnUtc, res.CreatedOnUtc.TimeOfDay, TimezoneINformation).Item2.Substring(0, 2) + ":" + UtilityRepository.UtcDateToOtherTimezone(res.CreatedOnUtc.Date, res.CreatedOnUtc.TimeOfDay, TimezoneINformation).Item2.Substring(2, 2);
                        FST.Tracking.FirstOrDefault().TrackingDetails.Insert(Count, STD);
                        Count++;
                    }
                }
            }
            return FST;
        }

        private static string ReadXMLDocument(string xml_in)
        {
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(xml_in));
            XElement xmlFile = XElement.Load(ms);
            var status = xmlFile.Descendants("newdataset ").Elements("table ");
            string val = GetElementValue(status);
            return val;
        }

        private static string GetElementValue(IEnumerable<XElement> elements)
        {
            foreach (var sat in elements)
            {
                return sat.Value;
            }
            return "";
        }
    }
}