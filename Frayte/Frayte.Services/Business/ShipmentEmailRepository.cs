using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using RazorEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using RazorEngine.Templating;
using System.Globalization;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using Elmah;
using Frayte.Services.Models.Aftership;
using Frayte.Services.Models.Tradelane;
using Frayte.Services.Models.ApiModal;
using Frayte.Services.Models.Express;

namespace Frayte.Services.Business
{
    public class ShipmentEmailRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        #region -- Public Methods Shipment Email --

        public void SendEmail_E1(int shipmentId)
        {
            var e1Detail = new ShipmentRepository().GetE1Detail(shipmentId);
            e1Detail.MailDateTime = UtilityRepository.GetTimeZoneCurrentDateTime(e1Detail.TimeZoneName);

            if (e1Detail.ShipmentType == FrayteShipmentType.Air)
            {
                e1Detail.CBMValue = e1Detail.CBMValue != null ? e1Detail.CBMValue.Value : 0.00m;
            }
            else if (e1Detail.ShipmentType == FrayteShipmentType.Sea)
            {
                e1Detail.CBM = e1Detail.CBMValue != null ? e1Detail.CBMValue.Value.ToString() : Convert.ToString(0.00);
                e1Detail.CBMValue = e1Detail.CBMValue != null ? (decimal?)CommonConversion.RoundDecimalValue(e1Detail.CBMValue.Value) : 0.00m;
            }

            if (e1Detail.GrowssWeight > e1Detail.Chargableweight)
            {
                e1Detail.Chargableweight = e1Detail.GrowssWeight;
            }

            e1Detail.TimeZoneName = e1Detail.TimeZoneShort;
            e1Detail.HostUrl = AppSettings.HostName;
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E1.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e1Detail, null, null);
            var EmailSubject = "Request for Booking Confirmation - " + e1Detail.ShipmentId + " - FRAYTE GLOBAL";
            var To = e1Detail.CustomerEmail;
            var CC = e1Detail.UserEmail + ";" + e1Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

        public void SendEmail_E2(int shipmentId)
        {
            var e2Detail = new ShipmentRepository().GetE2Detail(shipmentId);
            e2Detail.MailDateTime = UtilityRepository.GetTimeZoneCurrentDateTime(e2Detail.TimeZoneName);

            if (e2Detail.ShipmentType == FrayteShipmentType.Air)
            {
                e2Detail.CBMValue = e2Detail.CBMValue != null ? e2Detail.CBMValue.Value : 0.00m;
            }
            else if (e2Detail.ShipmentType == FrayteShipmentType.Sea)
            {
                e2Detail.CBM = e2Detail.CBMValue != null ? e2Detail.CBMValue.Value.ToString() : Convert.ToString(0.00);
                e2Detail.CBMValue = e2Detail.CBMValue != null ? (decimal?)CommonConversion.RoundDecimalValue(e2Detail.CBMValue.Value) : 0.00m;
            }

            if (e2Detail.GrowssWeight > e2Detail.Chargableweight)
            {
                e2Detail.Chargableweight = e2Detail.GrowssWeight;
            }

            e2Detail.TimeZoneName = e2Detail.TimeZoneShort;
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E2.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e2Detail, null, null);
            var EmailSubject = "Booking Acknowledgement - FRAYTE GLOBAL";
            var To = e2Detail.ShipperEmail;
            var CC = e2Detail.UserEmail + ";" + e2Detail.UserManagerEmail;

            string Attachmentpath = AppSettings.EmailServicePath + "\\Company Profile\\Frayte Company Profile.pdf";

            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";

            if (!string.IsNullOrEmpty(AppSettings.TOCC))
            {
                FrayteEmail.SendMail(AppSettings.TOCC, CC, EmailSubject, EmailBody, Attachmentpath, logoImage);
            }
            else
            {
                FrayteEmail.SendMail(To, CC, EmailSubject, EmailBody, Attachmentpath, logoImage);
            }
        }

        public void SendEmail_E3(int shipmentId)
        {
            var e3Detail = new ShipmentRepository().GetE3Detail(shipmentId);
            e3Detail.MailDateTime = UtilityRepository.GetTimeZoneCurrentDateTime(e3Detail.TimeZoneName);

            if (e3Detail.ShipmentType == FrayteShipmentType.Air)
            {
                e3Detail.CBMValue = e3Detail.CBMValue != null ? e3Detail.CBMValue.Value : 0.00m;
            }
            else if (e3Detail.ShipmentType == FrayteShipmentType.Sea)
            {
                e3Detail.CBM = e3Detail.CBMValue != null ? e3Detail.CBMValue.Value.ToString() : Convert.ToString(0.00);
                e3Detail.CBMValue = e3Detail.CBMValue != null ? (decimal?)CommonConversion.RoundDecimalValue(e3Detail.CBMValue.Value) : 0.00m;
            }

            if (e3Detail.GrowssWeight > e3Detail.Chargableweight)
            {
                e3Detail.Chargableweight = e3Detail.GrowssWeight;
            }

            if (e3Detail.PickupDate.HasValue)
            {
                e3Detail.PickupDate = UtilityRepository.GetTimeZoneDateTime(e3Detail.PickupDate, e3Detail.ShipmentReadyBy, e3Detail.TimeZoneName);
            }

            e3Detail.TimeZoneName = e3Detail.TimeZoneShort;
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E3.cshtml");
            var templateService = new TemplateService();

            //Attachment: “Warehouse Map” + “Shipping Delivery Notice”
            string shipmentDetailFolder = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/");
            string shipmentDetailPath = shipmentDetailFolder + "ShipmentDetail_" + shipmentId + ".PDF";
            string attachmentPath = "";
            if (File.Exists(shipmentDetailPath))
            {
                attachmentPath = shipmentDetailFolder + "ShipmentDetail_" + shipmentId + ".PDF";
            }
            string deliveryNoteFolder = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/");
            string deliveryNoteFilePath = deliveryNoteFolder + "DeliveryNote_" + shipmentId + ".PDF";

            if (File.Exists(deliveryNoteFilePath))
            {
                attachmentPath += ";" + deliveryNoteFilePath;
            }

            if (e3Detail.ShipmentType == "Courier")
            {
                e3Detail.EasyPostTrackingUrl = AppSettings.HostName + "/index.html#/home/tracking/UPS/" + e3Detail.EasyPostTrackingCode;
            }
            var EmailBody = templateService.Parse(template, e3Detail, null, null);
            string EmailSubject = "";
            if (!String.IsNullOrEmpty(e3Detail.CargoWiseSo))
            {
                EmailSubject = "Booking Confirmation - FRAYTE GLOBAL" + " - " + e3Detail.CargoWiseSo;
            }
            else
            {
                EmailSubject = "Booking Confirmation - FRAYTE GLOBAL" + " - " + e3Detail.ShipmentId.ToString();
            }

            var To = e3Detail.ShipperEmail;
            var CC = e3Detail.UserEmail + ";" + e3Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, attachmentPath);
        }

        public void SendEmail_E3_1(int shipmentId)
        {
            var e31Detail = new ShipmentRepository().GetE31Detail(shipmentId);
            e31Detail.MailDateTime = UtilityRepository.GetTimeZoneCurrentDateTime(e31Detail.TimeZoneName);

            if (e31Detail.ShipmentType == FrayteShipmentType.Air)
            {
                e31Detail.CBMValue = e31Detail.CBMValue != null ? e31Detail.CBMValue.Value : 0.00m;
            }
            else if (e31Detail.ShipmentType == FrayteShipmentType.Sea)
            {
                e31Detail.CBM = e31Detail.CBMValue != null ? e31Detail.CBMValue.Value.ToString() : Convert.ToString(0.00);
                e31Detail.CBMValue = e31Detail.CBMValue != null ? (decimal?)CommonConversion.RoundDecimalValue(e31Detail.CBMValue.Value) : 0.00m;
            }

            if (e31Detail.GrowssWeight > e31Detail.Chargableweight)
            {
                e31Detail.Chargableweight = e31Detail.GrowssWeight;
            }

            if (e31Detail.PickupDate.HasValue)
            {
                e31Detail.PickupDate = e31Detail.PickupDate.HasValue ? e31Detail.PickupDate : DateTime.Now;
                e31Detail.PickupDate = UtilityRepository.GetTimeZoneDateTime(e31Detail.PickupDate, e31Detail.ShipmentReadyBy, e31Detail.TimeZoneName);
            }

            e31Detail.TimeZoneName = e31Detail.TimeZoneShort;
            e31Detail.ShipmentLink = AppSettings.HostName + "/index.html#/home/shipment/" + shipmentId + "/" + (int)FrayteUserRole.Shipper + "/addressdetail";

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E3.1.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e31Detail, null, null);
            var EmailSubject = "Booking Confirmation - FRAYTE GLOBAL";

            //Attachment: “Warehouse Map” + “Shipping Delivery Notice”
            string shipmentDetailFolder = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/");
            string shipmentDetailPath = shipmentDetailFolder + "ShipmentDetail_" + shipmentId + ".PDF";
            string attachmentPath = "";
            if (File.Exists(shipmentDetailPath))
            {
                attachmentPath = shipmentDetailFolder + "ShipmentDetail_" + shipmentId + ".PDF";
            }
            string deliveryNoteFolder = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/");
            string deliveryNoteFilePath = deliveryNoteFolder + "DeliveryNote_" + shipmentId + ".PDF";

            if (File.Exists(deliveryNoteFilePath))
            {
                attachmentPath += ";" + deliveryNoteFilePath;
            }
            var To = e31Detail.ShipperEmail;
            var CC = e31Detail.UserEmail + ";" + e31Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, attachmentPath);
        }

        public void SendEmail_E3_2(int shipmentId)
        {
            var e3Detail = new ShipmentRepository().GetE3Detail(shipmentId);
            e3Detail.MailDateTime = UtilityRepository.GetTimeZoneCurrentDateTime(e3Detail.TimeZoneName);

            if (e3Detail.ShipmentType == FrayteShipmentType.Air)
            {
                e3Detail.CBMValue = e3Detail.CBMValue != null ? e3Detail.CBMValue.Value : 0.00m;
            }
            else if (e3Detail.ShipmentType == FrayteShipmentType.Sea)
            {
                e3Detail.CBM = e3Detail.CBMValue != null ? e3Detail.CBMValue.Value.ToString() : Convert.ToString(0.00);
                e3Detail.CBMValue = e3Detail.CBMValue != null ? (decimal?)CommonConversion.RoundDecimalValue(e3Detail.CBMValue.Value) : 0.00m;
            }

            if (e3Detail.GrowssWeight > e3Detail.Chargableweight)
            {
                e3Detail.Chargableweight = e3Detail.GrowssWeight;
            }

            if (e3Detail.PickupDate.HasValue)
            {
                e3Detail.PickupDate = UtilityRepository.GetTimeZoneDateTime(e3Detail.PickupDate, e3Detail.ShipmentReadyBy, e3Detail.TimeZoneName);
            }

            e3Detail.TimeZoneName = e3Detail.TimeZoneShort;
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E3.2.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e3Detail, null, null);
            var EmailSubject = "Booking Rejected - FRAYTE GLOBAL" + " - " + e3Detail.ShipmentId;
            var To = e3Detail.ShipperEmail;
            var CC = e3Detail.UserEmail + ";" + e3Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

        public void SendEmail_E4(int shipmentId)
        {
            var e4Detail = new ShipmentRepository().GetE4Detail(shipmentId);
            if (e4Detail.PickupDate.HasValue)
            {
                e4Detail.PickupDate = UtilityRepository.GetTimeZoneDateTime(e4Detail.PickupDate, e4Detail.ShipmentReadyBy, e4Detail.TimeZoneName);
            }

            if (e4Detail.ShipmentType == FrayteShipmentType.Air)
            {
                e4Detail.CBMValue = e4Detail.CBMValue != null ? e4Detail.CBMValue.Value : 0.00m;
            }
            else if (e4Detail.ShipmentType == FrayteShipmentType.Sea)
            {
                e4Detail.CBM = e4Detail.CBMValue != null ? e4Detail.CBMValue.Value.ToString() : Convert.ToString(0.00);
                e4Detail.CBMValue = e4Detail.CBMValue != null ? (decimal?)CommonConversion.RoundDecimalValue(e4Detail.CBMValue.Value) : 0.00m;
            }

            if (e4Detail.GrowssWeight > e4Detail.Chargableweight)
            {
                e4Detail.Chargableweight = e4Detail.GrowssWeight;
            }

            e4Detail.TimeZoneName = e4Detail.TimeZoneShort;
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E4.cshtml");
            if (e4Detail.ShipmentType == "Courier")
            {
                e4Detail.EasyPostTrackingUrl = AppSettings.HostName + "/index.html#/home/tracking/UPS/" + e4Detail.EasyPostTrackingCode;
            }

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e4Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e4Detail.CargoWiseSo))
            {
                EmailSubject = "Warehouse Alert ( " + e4Detail.WarehouseCountryCode + " ) - (" + e4Detail.CustomerName + ") -" + e4Detail.CargoWiseSo + "-" + "FRAYTE GLOBAL";
            }
            else
            {
                EmailSubject = "Warehouse Alert ( " + e4Detail.WarehouseCountryCode + " ) - (" + e4Detail.CustomerName + ") -" + e4Detail.ShipmentId.ToString() + "-" + "FRAYTE GLOBAL";
            }

            string attachmentPath = "";
            string deliveryNoteFolder = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/");
            string deliveryNoteFilePath = deliveryNoteFolder + "DeliveryNote_" + shipmentId + ".PDF";

            if (File.Exists(deliveryNoteFilePath))
            {
                attachmentPath = deliveryNoteFilePath;
            }

            var To = e4Detail.WarehouseEmail;
            var CC = e4Detail.UserEmail + ";" + e4Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, attachmentPath);
        }

        public void SendEmail_E4_1(int shipmentId)
        {
            var e4Detail = new ShipmentRepository().GetE4Detail(shipmentId);
            if (e4Detail.PickupDate.HasValue)
            {
                e4Detail.PickupDate = UtilityRepository.GetTimeZoneDateTime(e4Detail.PickupDate, e4Detail.ShipmentReadyBy, e4Detail.TimeZoneName);
            }
            e4Detail.TimeZoneName = e4Detail.TimeZoneShort;

            if (e4Detail.ShipmentType == FrayteShipmentType.Air)
            {
                e4Detail.CBMValue = e4Detail.CBMValue != null ? e4Detail.CBMValue.Value : 0.00m;
            }
            else if (e4Detail.ShipmentType == FrayteShipmentType.Sea)
            {
                e4Detail.CBM = e4Detail.CBMValue != null ? e4Detail.CBMValue.Value.ToString() : Convert.ToString(0.00);
                e4Detail.CBMValue = e4Detail.CBMValue != null ? (decimal?)CommonConversion.RoundDecimalValue(e4Detail.CBMValue.Value) : 0.00m;
            }

            if (e4Detail.GrowssWeight > e4Detail.Chargableweight)
            {
                e4Detail.Chargableweight = e4Detail.GrowssWeight;
            }

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E4.1.cshtml");
            if (e4Detail.ShipmentType == FrayteShipmentType.Courier)
            {
                e4Detail.EasyPostTrackingUrl = AppSettings.HostName + "/index.html#/home/tracking/UPS/" + e4Detail.EasyPostTrackingCode;
            }
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e4Detail, null, null);
            string EmailSubject = string.Empty;
            if (!string.IsNullOrEmpty(e4Detail.WarehouseEmail))
            {
                if (!String.IsNullOrEmpty(e4Detail.CargoWiseSo))
                {
                    EmailSubject = "Shipper uploaded documents - Warehouse Alert ( " + e4Detail.WarehouseCountryCode + " ) - (" + e4Detail.CustomerName + ") -" + e4Detail.CargoWiseSo + "- FRAYTE GLOBAL";
                }
                else
                {
                    EmailSubject = "Shipper uploaded documents - Warehouse Alert ( " + e4Detail.WarehouseCountryCode + " ) - (" + e4Detail.CustomerName + ") -" + e4Detail.ShipmentId.ToString() + "- FRAYTE GLOBAL";
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(e4Detail.CargoWiseSo))
                {
                    EmailSubject = "Shipper uploaded documents - AWB Update Alert  (" + e4Detail.CustomerName + ") -" + e4Detail.CargoWiseSo + "- FRAYTE GLOBAL";
                }
                else
                {
                    EmailSubject = "Shipper uploaded documents - AWB Update Alert  (" + e4Detail.CustomerName + ") -" + e4Detail.ShipmentId.ToString() + "- FRAYTE GLOBAL";
                }
            }

            // Attchments
            string documentFolder = HttpContext.Current.Server.MapPath("~/UploadFiles/Shipments/" + shipmentId);
            string attachmentPath = "";


            if (!string.IsNullOrEmpty(e4Detail.CommercialInvoice) && File.Exists(documentFolder + "\\" + e4Detail.CommercialInvoice))
            {
                attachmentPath += ";" + documentFolder + "\\" + e4Detail.CommercialInvoice;
            }

            if (!string.IsNullOrEmpty(e4Detail.PackingList) && File.Exists(documentFolder + "\\" + e4Detail.PackingList))
            {
                attachmentPath += ";" + documentFolder + "\\" + e4Detail.PackingList;
            }

            if (!string.IsNullOrEmpty(e4Detail.CustomDocument) && File.Exists(documentFolder + "\\" + e4Detail.CustomDocument))
            {
                attachmentPath += ";" + documentFolder + "\\" + e4Detail.CustomDocument;
            }

            if (!string.IsNullOrEmpty(attachmentPath))
            {
                attachmentPath = attachmentPath.Trim().TrimStart(new char[] { ';' });
            }

            string To = !string.IsNullOrEmpty(e4Detail.WarehouseEmail) == true ? e4Detail.WarehouseEmail : e4Detail.UserEmail;
            var CC = e4Detail.UserEmail + ";" + e4Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, attachmentPath);
        }

        public void SendEmail_E5(int shipmentId)
        {
            var e5Detail = new ShipmentRepository().GetE5Detail(shipmentId);
            if (e5Detail.ShippingDate != null && e5Detail.ShippingTime != null && !string.IsNullOrEmpty(e5Detail.TimeZoneName))
            {
                e5Detail.ShippingDate = (DateTime)UtilityRepository.GetTimeZoneDateTime(e5Detail.ShippingDate, e5Detail.ShippingTime, e5Detail.TimeZoneName);
            }

            if (e5Detail.ShipmentType == FrayteShipmentType.Air)
            {
                e5Detail.CBMValue = e5Detail.CBMValue != null ? e5Detail.CBMValue.Value : 0.00m;
            }
            else if (e5Detail.ShipmentType == FrayteShipmentType.Sea)
            {
                e5Detail.CBM = e5Detail.CBMValue != null ? e5Detail.CBMValue.Value.ToString() : Convert.ToString(0.00);
                e5Detail.CBMValue = e5Detail.CBMValue != null ? (decimal?)CommonConversion.RoundDecimalValue(e5Detail.CBMValue.Value) : 0.00m;
            }

            if (e5Detail.GrowssWeight > e5Detail.Chargableweight)
            {
                e5Detail.Chargableweight = e5Detail.GrowssWeight;
            }

            e5Detail.ShippingTime = e5Detail.ShippingTime != null ? e5Detail.ShippingTime : DateTime.Now.TimeOfDay;
            if (!string.IsNullOrEmpty(e5Detail.TimeZoneName))
            {
                e5Detail.ShippingTime = (TimeSpan)UtilityRepository.GetTimeZoneTimeSpan(e5Detail.ShippingTime, e5Detail.TimeZoneName);
            }

            e5Detail.TimeZoneName = e5Detail.TimeZoneShort;
            e5Detail.ConfirmLinkHref = AppSettings.HostName + "/index.html#/public/agent-action/c/" + shipmentId;
            e5Detail.AmendLinkHref = AppSettings.HostName + "/index.html#/public/agent-action/r/" + shipmentId;
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E5.cshtml");

            // Generate PDF for co-loader-booking-form

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e5Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e5Detail.CargoWiseSo))
            {
                EmailSubject = "FRAYTE GLOBAL - NEW Booking - " + e5Detail.CargoWiseSo;
            }
            else
            {
                EmailSubject = "FRAYTE GLOBAL - NEW Booking - " + e5Detail.ShipmentId.ToString();
            }
            string Attachmentpath = "";
            string pdfFolder = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/");
            string pdfFilePath = pdfFolder + "CO_Load_Booking_Form_" + shipmentId + ".PDF";

            if (File.Exists(pdfFilePath))
            {
                Attachmentpath = pdfFilePath;
            }
            var To = e5Detail.AgentEMail;
            var CC = e5Detail.UserEmail + ";" + e5Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, Attachmentpath);
        }

        public void SendEmail_E5_1(int shipmentId)
        {
            var e51Detail = new ShipmentRepository().GetE51Detail(shipmentId);

            if (e51Detail.ShippingDate != null && e51Detail.ShippingTime != null && !string.IsNullOrEmpty(e51Detail.TimeZoneName))
            {
                e51Detail.ShippingDate = (DateTime)UtilityRepository.GetTimeZoneDateTime(e51Detail.ShippingDate, e51Detail.ShippingTime, e51Detail.TimeZoneName);
            }

            if (e51Detail.ShipmentType == FrayteShipmentType.Air)
            {
                e51Detail.CBMValue = e51Detail.CBMValue != null ? e51Detail.CBMValue.Value : 0.00m;
            }
            else if (e51Detail.ShipmentType == FrayteShipmentType.Sea)
            {
                e51Detail.CBM = e51Detail.CBMValue != null ? e51Detail.CBMValue.Value.ToString() : Convert.ToString(0.00);
                e51Detail.CBMValue = e51Detail.CBMValue != null ? (decimal?)CommonConversion.RoundDecimalValue(e51Detail.CBMValue.Value) : 0.00m;
            }

            if (e51Detail.GrowssWeight > e51Detail.Chargableweight)
            {
                e51Detail.Chargableweight = e51Detail.GrowssWeight;
            }

            e51Detail.ShippingTime = e51Detail.ShippingTime != null ? e51Detail.ShippingTime : DateTime.Now.TimeOfDay;
            if (!string.IsNullOrEmpty(e51Detail.TimeZoneName))
            {
                e51Detail.ShippingTime = (TimeSpan)UtilityRepository.GetTimeZoneTimeSpan(e51Detail.ShippingTime, e51Detail.TimeZoneName);
            }

            e51Detail.TimeZoneName = e51Detail.TimeZoneShort;
            e51Detail.ReselectLinkHref = AppSettings.HostName + "/index.html#/public/agent-reselect/" + shipmentId;
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E5.1.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e51Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e51Detail.CargoWiseSo))
            {
                EmailSubject = "FRAYTE GLOBAL - Reselect Agent for shipment - " + e51Detail.CargoWiseSo;
            }
            else
            {
                EmailSubject = "FRAYTE GLOBAL - Reselect Agent for shipment - " + e51Detail.ShipmentId.ToString();
            }

            var To = e51Detail.UserEmail;
            var CC = e51Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

        public void SendEmail_E6(int shipmentId)
        {
            ShipmentDetailE6 sd6 = new ShipmentDetailE6();

            var e6Detail = new ShipmentRepository().GetE6Detail(shipmentId);
            e6Detail.TimeZoneName = e6Detail.TimeZoneShort;
            e6Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/shipmentdocument/" + shipmentId;
            if (e6Detail != null && !string.IsNullOrEmpty(e6Detail.CountryDocuments) && e6Detail.CountryDocuments.Length == 1)
            {
                e6Detail.CountryDocuments = null;
            }

            string countryDocuments = "";
            sd6.CountryDocuments = new List<string>();
            List<string> _item = new List<string>();

            if (e6Detail.CountryDocuments != null)
            {
                foreach (var cdoc in e6Detail.CountryDocuments.Split(','))
                {
                    _item.Add(cdoc);
                }

                sd6.CountryDocuments = _item;
            }

            sd6.UserEmail = e6Detail.UserEmail;
            sd6.UserFax = e6Detail.UserFax;
            sd6.UserManagerEmail = e6Detail.UserManagerEmail;
            sd6.UserName = e6Detail.UserName;
            sd6.UserPhone = e6Detail.UserPhone;
            sd6.UserPosition = e6Detail.UserPosition;
            sd6.UserSkype = e6Detail.UserSkype;

            sd6.TotalWeight = e6Detail.TotalWeight;
            sd6.TotalCTNs = e6Detail.TotalCTNs;
            sd6.ShipperName = e6Detail.ShipperName;
            sd6.ShipperEmail = e6Detail.ShipperEmail;
            sd6.ShipmentMethod = e6Detail.ShipmentMethod;
            sd6.ShipmentId = e6Detail.ShipmentId;
            sd6.ImageHeader = e6Detail.ImageHeader;
            sd6.ClientName = e6Detail.ClientName;
            sd6.TimeZoneName = e6Detail.TimeZoneShort;
            sd6.ShipmentType = e6Detail.ShipmentType;
            if (sd6.ShipmentType == FrayteShipmentType.Courier)
            {
                sd6.EasyPostTrackingCode = e6Detail.EasyPostTrackingCode;
                sd6.EasyPostTrackingUrl = e6Detail.EasyPostTrackingUrl;
                sd6.EsayPostLabel = e6Detail.EsayPostLabel;
            }
            sd6.UploadLinkHref = AppSettings.HostName + "/index.html#/public/shipmentdocument/" + shipmentId;

            if (sd6.ShipmentType == FrayteShipmentType.Courier)
            {
                sd6.EasyPostTrackingUrl = AppSettings.HostName + "/index.html#/home/tracking/UPS/" + e6Detail.EasyPostTrackingCode;
            }
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E6.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, sd6, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e6Detail.CargoWiseSo))
            {
                EmailSubject = e6Detail.ClientName + " Request for Shipping Documents - " + e6Detail.CargoWiseSo;
            }
            else
            {
                EmailSubject = e6Detail.ClientName + " Request for Shipping Documents - " + e6Detail.ShipmentId.ToString();
            }

            string Attachmentpath = AppSettings.EmailServicePath + "\\Company Profile\\Commercial Invoice.xls";
            Attachmentpath += ";" + AppSettings.EmailServicePath + "\\Company Profile\\Packing List.xls";

            var To = e6Detail.ShipperEmail;
            var CC = e6Detail.UserEmail + ";" + e6Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, Attachmentpath);
        }

        public void SendEmail_E61(int shipmentId)
        {
            ShipmentDetailE6 sd6 = new ShipmentDetailE6();

            var e61Detail = new ShipmentRepository().GetE61Detail(shipmentId);
            e61Detail.TimeZoneName = e61Detail.TimeZoneShort;
            e61Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/shipmentdocument/" + shipmentId;

            sd6.CountryDocuments = new List<string>();
            List<string> _item = new List<string>();

            if (e61Detail != null && e61Detail.CountryDocuments != null && e61Detail.CountryDocuments.Length == 1)
            {
                e61Detail.CountryDocuments = null;
            }

            string countryDocuments = "";
            if (e61Detail.CountryDocuments != null)
            {
                foreach (var cdoc in e61Detail.CountryDocuments.Split(','))
                {
                    _item.Add(cdoc);
                }
                sd6.CountryDocuments = _item;
            }

            sd6.UserEmail = e61Detail.UserEmail;
            sd6.UserFax = e61Detail.UserFax;
            sd6.UserManagerEmail = e61Detail.UserManagerEmail;
            sd6.UserName = e61Detail.UserName;
            sd6.UserPhone = e61Detail.UserPhone;
            sd6.UserPosition = e61Detail.UserPosition;
            sd6.UserSkype = e61Detail.UserSkype;

            sd6.TotalWeight = e61Detail.TotalWeight;
            sd6.TotalCTNs = e61Detail.TotalCTNs;
            sd6.ShipperName = e61Detail.ShipperName;
            sd6.ShipperEmail = e61Detail.ShipperEmail;
            sd6.ShipmentMethod = e61Detail.ShipmentMethod;
            sd6.ShipmentId = e61Detail.ShipmentId;
            sd6.ImageHeader = e61Detail.ImageHeader;
            sd6.ClientName = e61Detail.ClientName;
            sd6.TimeZoneName = e61Detail.TimeZoneShort;
            sd6.UploadLinkHref = AppSettings.HostName + "/index.html#/public/shipmentdocument/" + shipmentId;

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E6.1.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, sd6, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e61Detail.CargoWiseSo))
            {
                EmailSubject = e61Detail.ClientName + " Request for Shipping Documents - " + e61Detail.CargoWiseSo + " REMINDER - " + (Convert.ToInt32(e61Detail.RemainderNo) + 1).ToString();
            }
            else
            {
                EmailSubject = e61Detail.ClientName + " Request for Shipping Documents - " + e61Detail.ShipmentId.ToString() + " REMINDER - " + (Convert.ToInt32(e61Detail.RemainderNo) + 1).ToString();
            }


            string Attachmentpath = AppSettings.EmailServicePath + "\\Company Profile\\Commercial invoice.xls";
            Attachmentpath += ";" + AppSettings.EmailServicePath + "\\Company Profile\\Packing List Format.xls";

            var To = e61Detail.ShipperEmail + ";" + e61Detail.ShipperManagerEmail;
            var CC = e61Detail.UserEmail + ";" + e61Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, Attachmentpath);
        }

        public void SendEmail_E62(int shipmentId)
        {

            ShipmentDetailE62 sd6 = new ShipmentDetailE62();
            int wareHouseId = new WarehouseRepository().GetWareHouseId(shipmentId);
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            var e4Detail = new ShipmentRepository().GetE4Detail(shipmentId);

            e4Detail.MailDateTime = UtilityRepository.GetTimeZoneCurrentDateTime(e4Detail.TimeZoneName);


            sd6.MailDate = e4Detail.MailDateTime.ToString("dd-MMM-yyyy");
            sd6.MailTime = e4Detail.MailDateTime.ToString("hh:mm");

            sd6.ReceiverName = e4Detail.ReceiverName;
            sd6.PAddress = e4Detail.PAddress;
            sd6.PAddress2 = e4Detail.PAddress2;
            sd6.PAddress3 = e4Detail.PAddress3;
            sd6.PCity = e4Detail.PCity;
            sd6.PState = e4Detail.PState;
            sd6.PCountry = e4Detail.PCountry;
            sd6.PZip = e4Detail.PZip;

            sd6.RAddress = e4Detail.RAddress;
            sd6.RAddress2 = e4Detail.RAddress2;
            sd6.RAddress3 = e4Detail.RAddress3;
            sd6.RCity = e4Detail.RCity;
            sd6.RState = e4Detail.RState;
            sd6.RCountry = e4Detail.RCountry;
            sd6.RZip = e4Detail.RZip;

            sd6.UserEmail = e4Detail.UserEmail;
            sd6.UserFax = e4Detail.UserFax;
            sd6.UserManagerEmail = e4Detail.UserManagerEmail;
            sd6.UserName = e4Detail.UserName;
            sd6.UserPhone = e4Detail.UserPhone;
            sd6.UserPosition = e4Detail.UserPosition;
            sd6.UserSkype = e4Detail.UserSkype;
            sd6.ShipmentType = e4Detail.ShipmentType;
            if (sd6.ShipmentType == FrayteShipmentType.Courier)
            {
                sd6.TotalCTNs = e4Detail.TotalCTNs;
                sd6.GrowssWeight = e4Detail.GrowssWeight;
                if (e4Detail.GrowssWeight > e4Detail.Chargableweight)
                {
                    e4Detail.Chargableweight = e4Detail.GrowssWeight;
                    sd6.Chargableweight = e4Detail.Chargableweight;
                }
            }
            else if (sd6.ShipmentType == FrayteShipmentType.Air)
            {
                sd6.TotalCTNs = e4Detail.TotalCTNs;
                sd6.GrowssWeight = e4Detail.GrowssWeight;
                sd6.CBMValue = e4Detail.CBMValue != null ? e4Detail.CBMValue.Value : 0.00m;
                if (e4Detail.GrowssWeight > e4Detail.Chargableweight)
                {
                    e4Detail.Chargableweight = e4Detail.GrowssWeight;
                    sd6.Chargableweight = e4Detail.Chargableweight;
                }
            }
            else if (sd6.ShipmentType == FrayteShipmentType.Sea)
            {
                sd6.TotalCTNs = e4Detail.TotalCTNs;
                sd6.GrowssWeight = e4Detail.GrowssWeight;
                sd6.CBM = e4Detail.CBMValue != null ? e4Detail.CBMValue.Value : 0.00m;
                sd6.CBMValue = e4Detail.CBMValue != null ? (decimal?)CommonConversion.RoundDecimalValue(e4Detail.CBMValue.Value) : 0.00m;
                if (e4Detail.GrowssWeight > e4Detail.Chargableweight)
                {
                    e4Detail.Chargableweight = e4Detail.GrowssWeight;
                    sd6.Chargableweight = e4Detail.Chargableweight;
                }
            }

            sd6.ShipperName = e4Detail.ShipperName;
            sd6.ShipmentMethod = e4Detail.ShipmentMethod;
            sd6.ShipmentId = e4Detail.ShipmentId;

            sd6.ImageHeader = e4Detail.ImageHeader;
            sd6.TimeZoneName = e4Detail.TimeZoneShort;
            sd6.UploadLinkHref = AppSettings.HostName + "/index.html#/public/warehouse-drop-off/" + shipmentId;
            string template = "";
            string To = "";
            string EmailSubject = String.Empty;

            if (wareHouseId > 0)
            {
                template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E6.2.cshtml");
                if (!String.IsNullOrEmpty(e4Detail.CargoWiseSo))
                {
                    EmailSubject = "Shipper uploaded documents - Warehouse Alert ( " + e4Detail.WarehouseCountryCode + " ) - (" + e4Detail.CustomerName + ") -" + e4Detail.CargoWiseSo + "- FRAYTE GLOBAL";
                }
                else
                {
                    EmailSubject = "Shipper uploaded documents - Warehouse Alert ( " + e4Detail.WarehouseCountryCode + " ) - (" + e4Detail.CustomerName + ") -" + e4Detail.ShipmentId.ToString() + "- FRAYTE GLOBAL";
                }

                To = e4Detail.WarehouseEmail;
            }
            else
            {
                template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E6.8.cshtml");
                if (!String.IsNullOrEmpty(e4Detail.CargoWiseSo))
                {
                    EmailSubject = "Shipper uploaded documents - DropOff Alert - " + e4Detail.CargoWiseSo + "- FRAYTE GLOBAL";
                }
                else
                {
                    EmailSubject = "Shipper uploaded documents - DropOff Alert - " + e4Detail.ShipmentId.ToString() + "- FRAYTE GLOBAL";
                }


                To = e4Detail.ShipperEmail;
            }

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, sd6, null, null);

            var CC = e4Detail.UserEmail + ";" + e4Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

        public void SendEmail_E63(int shipmentId)
        {

            ShipmentDetailE62 sd6 = new ShipmentDetailE62();

            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            var e4Detail = new ShipmentRepository().GetE4Detail(shipmentId);

            e4Detail.MailDateTime = UtilityRepository.GetTimeZoneCurrentDateTime(e4Detail.TimeZoneName);

            sd6.MailDate = e4Detail.MailDateTime.ToString("dd-MMM-yyyy");
            sd6.MailTime = e4Detail.MailDateTime.ToString("hh:mm");

            sd6.ReceiverName = e4Detail.ReceiverName;
            sd6.PAddress = e4Detail.PAddress;
            sd6.PAddress2 = e4Detail.PAddress2;
            sd6.PAddress3 = e4Detail.PAddress3;
            sd6.PCity = e4Detail.PCity;
            sd6.PState = e4Detail.PState;
            sd6.PCountry = e4Detail.PCountry;
            sd6.PZip = e4Detail.PZip;

            sd6.RAddress = e4Detail.RAddress;
            sd6.RAddress2 = e4Detail.RAddress2;
            sd6.RAddress3 = e4Detail.RAddress3;
            sd6.RCity = e4Detail.RCity;
            sd6.RState = e4Detail.RState;
            sd6.RCountry = e4Detail.RCountry;
            sd6.RZip = e4Detail.RZip;

            sd6.UserEmail = e4Detail.UserEmail;
            sd6.UserFax = e4Detail.UserFax;
            sd6.UserManagerEmail = e4Detail.UserManagerEmail;
            sd6.UserName = e4Detail.UserName;
            sd6.UserPhone = e4Detail.UserPhone;
            sd6.UserPosition = e4Detail.UserPosition;
            sd6.UserSkype = e4Detail.UserSkype;

            sd6.ShipmentType = e4Detail.ShipmentType;
            if (sd6.ShipmentType == FrayteShipmentType.Courier)
            {
                sd6.TotalCTNs = e4Detail.TotalCTNs;
                sd6.GrowssWeight = e4Detail.GrowssWeight;
                if (e4Detail.GrowssWeight > e4Detail.Chargableweight)
                {
                    e4Detail.Chargableweight = e4Detail.GrowssWeight;
                    sd6.Chargableweight = e4Detail.Chargableweight;
                }
            }
            else if (sd6.ShipmentType == FrayteShipmentType.Air)
            {
                sd6.TotalCTNs = e4Detail.TotalCTNs;
                sd6.GrowssWeight = e4Detail.GrowssWeight;
                sd6.CBMValue = e4Detail.CBMValue != null ? e4Detail.CBMValue.Value : 0.00m;
                if (e4Detail.GrowssWeight > e4Detail.Chargableweight)
                {
                    e4Detail.Chargableweight = e4Detail.GrowssWeight;
                    sd6.Chargableweight = e4Detail.Chargableweight;
                }
            }
            else if (sd6.ShipmentType == FrayteShipmentType.Sea)
            {
                sd6.TotalCTNs = e4Detail.TotalCTNs;
                sd6.GrowssWeight = e4Detail.GrowssWeight;
                sd6.CBM = e4Detail.CBMValue != null ? e4Detail.CBMValue.Value : 0.00m;
                sd6.CBMValue = e4Detail.CBMValue != null ? (decimal?)CommonConversion.RoundDecimalValue(e4Detail.CBMValue.Value) : 0.00m;
                if (e4Detail.GrowssWeight > e4Detail.Chargableweight)
                {
                    e4Detail.Chargableweight = e4Detail.GrowssWeight;
                    sd6.Chargableweight = e4Detail.Chargableweight;
                }
            }

            sd6.ShipperName = e4Detail.ShipperName;
            sd6.ShipmentMethod = e4Detail.ShipmentMethod;
            sd6.ShipmentId = e4Detail.ShipmentId;
            sd6.DrpofAddress = e4Detail.DropofAdress;
            sd6.DropofCity = e4Detail.DropofCity;
            sd6.DropofState = e4Detail.DropofState;
            sd6.DropofZip = e4Detail.DropofZip;
            sd6.DropofCountry = e4Detail.DropofCountry;
            sd6.DepartureDate = e4Detail.DepartureDate.Value.Date.ToString("dd-MMM-yyyy");
            sd6.DepartureTime = String.Format("{0:00}:{1:00}", e4Detail.DepartureTime.Value.Hours, e4Detail.DepartureTime.Value.Minutes);
            sd6.ArrivalDate = e4Detail.ArrivalDate.Value.Date.ToString("dd-MMM-yyyy");
            sd6.ArrivalTime = String.Format("{0:00}:{1:00}", e4Detail.ArrivalTime.Value.Hours, e4Detail.ArrivalTime.Value.Minutes);
            sd6.ImageHeader = e4Detail.ImageHeader;
            sd6.TimeZoneName = e4Detail.TimeZoneShort;
            sd6.UploadLinkHref = AppSettings.HostName + "/index.html#/public/operation-staff/" + shipmentId;
            string template2 = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E6.3.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template2, sd6, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e4Detail.CargoWiseSo))
            {
                EmailSubject = "Operation Staff Confirmation - " + e4Detail.CargoWiseSo;
            }
            else
            {
                EmailSubject = "Operation Staff Confirmation - " + e4Detail.ShipmentId.ToString();
            }

            var To = e4Detail.UserEmail;
            var CC = e4Detail.UserEmail + ";" + e4Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

        public void SendEmail_E64(int shipmentId)
        {

            var e64Detail = new ShipmentRepository().GetE64Detail(shipmentId);

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E6.4.cshtml");
            e64Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/shipper-telex/" + shipmentId;

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e64Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e64Detail.CargoWiseSo))
            {
                EmailSubject = "Update Telex Release Information - FRAYTE GLOBAL - " + e64Detail.CargoWiseSo;
            }
            else
            {
                EmailSubject = "Update Telex Release Information - FRAYTE GLOBAL - " + e64Detail.ShipmentId.ToString();
            }

            string Attachmentpath = string.Empty;

            Attachmentpath = AppSettings.EmailServicePath + "\\Company Profile\\Frayte Company Profile.pdf";
            Attachmentpath += ";" + AppSettings.EmailServicePath + "\\Company Profile\\2016_-_Client_Telex_Release.doc";

            if (!string.IsNullOrEmpty(Attachmentpath))
            {
                Attachmentpath = Attachmentpath.Trim().TrimStart(new char[] { ';' });
            }

            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";

            var To = e64Detail.ShipperEmail;
            var CC = e64Detail.UserEmail + ";" + e64Detail.UserManagerEmail;

            if (!string.IsNullOrEmpty(AppSettings.TOCC))
            {
                FrayteEmail.SendMail(AppSettings.TOCC, "", EmailSubject, EmailBody, Attachmentpath, logoImage);
            }
            else
            {
                FrayteEmail.SendMail(To, CC, EmailSubject, EmailBody, Attachmentpath, logoImage);
            }
        }

        public void SendEmail_E65(int shipmentId)
        {
            var e65Detail = new ShipmentRepository().GetE65Detail(shipmentId);
            e65Detail.MailDateTime = UtilityRepository.GetTimeZoneCurrentDateTime(e65Detail.TimeZoneName);

            e65Detail.TimeZoneName = e65Detail.TimeZoneShort;

            if (e65Detail.ShipmentType == FrayteShipmentType.Air)
            {
                e65Detail.CBMValue = e65Detail.CBMValue != null ? e65Detail.CBMValue.Value : 0.00m;
            }
            else if (e65Detail.ShipmentType == FrayteShipmentType.Sea)
            {
                e65Detail.CBM = e65Detail.CBMValue != null ? e65Detail.CBMValue.Value.ToString() : Convert.ToString(0.00);
                e65Detail.CBMValue = e65Detail.CBMValue != null ? (decimal?)CommonConversion.RoundDecimalValue(e65Detail.CBMValue.Value) : 0.00m;
            }

            if (e65Detail.GrowssWeight > e65Detail.Chargableweight)
            {
                e65Detail.Chargableweight = e65Detail.GrowssWeight;
            }
            e65Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/shipper-anticipated/" + shipmentId;
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E6.5.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e65Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e65Detail.CargoWiseSo))
            {
                EmailSubject = "FRAYTE Uploaded AWB#" + e65Detail.AirWayBill + " for Shipment - " + e65Detail.CargoWiseSo;
            }
            else
            {
                EmailSubject = "FRAYTE Uploaded AWB#" + e65Detail.AirWayBill + " for Shipment - " + shipmentId.ToString();
            }

            var To = e65Detail.ShipperEmail;
            var CC = e65Detail.UserEmail + ";" + e65Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

        public void SendEmail_E66(int shipmentId)
        {
            var e65Detail = new ShipmentRepository().GetE65Detail(shipmentId);
            e65Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/shipper-upload-awb/" + shipmentId.ToString();
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E6.6.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e65Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e65Detail.CargoWiseSo))
            {
                EmailSubject = "Upload AWB document for Shipment - " + e65Detail.CargoWiseSo;
            }
            else
            {
                EmailSubject = "Upload AWB document for Shipment - " + shipmentId.ToString();
            }

            var To = e65Detail.ShipperEmail;
            var CC = e65Detail.UserEmail + ";" + e65Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

        public void SendEmail_E67(int shipmentId)
        {
            var e67Detail = new ShipmentRepository().GetE67Detail(shipmentId);
            //   e65Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/shipper-upload-awb/" + shipmentId.ToString();
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E6.7.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e67Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e67Detail.CargoWiseSo))
            {
                EmailSubject = "Agent Updated Information for Shipment - " + e67Detail.CargoWiseSo;
            }
            else
            {
                EmailSubject = "Agent Updated Information for Shipment - " + shipmentId.ToString();
            }

            var To = e67Detail.ShipperEmail;
            var CC = e67Detail.UserEmail + ";" + e67Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

        public void SendEmail_E7(int shipmentId)
        {
            var e7Detail = new ShipmentRepository().GetE7Detail(shipmentId);

            if (!string.IsNullOrEmpty(e7Detail.TimeZoneName))
            {
                e7Detail.ETDTime = UtilityRepository.GetTimeZoneTimeSpan(e7Detail.ETDTime, e7Detail.TimeZoneName);
                e7Detail.ETATime = UtilityRepository.GetTimeZoneTimeSpan(e7Detail.ETATime, e7Detail.TimeZoneName);
            }

            e7Detail.TimeZoneName = e7Detail.TimeZoneShort;

            if (e7Detail.CourierType == FrayteShipmentType.Air)
            {
                e7Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/agent-flight-sea-document/a/" + shipmentId.ToString();
            }
            else if (e7Detail.CourierType == FrayteShipmentType.Sea)
            {
                e7Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/agent-flight-sea-document/s/" + shipmentId.ToString();
            }

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E7.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e7Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e7Detail.CargoWiseSo))
            {
                EmailSubject = "Request for Flight / Vessel Details - " + e7Detail.CargoWiseSo;
            }
            else
            {
                EmailSubject = "Request for Flight / Vessel Details - " + e7Detail.ShipmentId.ToString();
            }

            var To = e7Detail.AgentEMail;
            var CC = e7Detail.UserEmail + ";" + e7Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

        public void SendEmail_E71(int shipmentId)
        {
            var e71Detail = new ShipmentRepository().GetE71Detail(shipmentId);

            e71Detail.ETADate = e71Detail.ETADate.HasValue ? e71Detail.ETADate : DateTime.Now;
            e71Detail.ETATime = e71Detail.ETATime.HasValue ? e71Detail.ETATime : DateTime.Now.TimeOfDay;
            e71Detail.ETDDate = e71Detail.ETDDate.HasValue ? e71Detail.ETDDate : DateTime.Now;
            e71Detail.ETDTime = e71Detail.ETDTime.HasValue ? e71Detail.ETDTime : DateTime.Now.TimeOfDay;
            if (!string.IsNullOrEmpty(e71Detail.TimeZoneName))
            {
                e71Detail.ETATime = UtilityRepository.GetTimeZoneTimeSpan(e71Detail.ETATime, e71Detail.TimeZoneName);
                e71Detail.ETDTime = UtilityRepository.GetTimeZoneTimeSpan(e71Detail.ETDTime, e71Detail.TimeZoneName);
            }

            e71Detail.TimeZoneName = e71Detail.TimeZoneShort;

            if (e71Detail.CourierType == "Air")
            {
                e71Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/agent-flight-sea-document/a/" + shipmentId.ToString();
            }
            else if (e71Detail.CourierType == "Sea")
            {
                e71Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/agent-flight-sea-document/s/" + shipmentId.ToString();
            }

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E7.1.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e71Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e71Detail.CargoWiseSo))
            {
                EmailSubject = "Request for Flight / Vessel Details - " + e71Detail.CargoWiseSo + " REMINDER - " + (Convert.ToInt32(e71Detail.RemainderNo) + 1).ToString(); ;
            }
            else
            {
                EmailSubject = "Request for Flight / Vessel Details - " + e71Detail.ShipmentId.ToString() + " REMINDER - " + (Convert.ToInt32(e71Detail.RemainderNo) + 1).ToString(); ;
            }

            var To = e71Detail.AgentEMail;
            var CC = e71Detail.UserEmail + ";" + e71Detail.UserManagerEmail + ";" + e71Detail.AgentManagerMail;
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

        public void SendEmail_E8(int shipmentId)
        {
            var e8Detail = new ShipmentRepository().GetE8Detail(shipmentId);
            string shipmentType = new ShipmentRepository().GetShipmentCourierType(shipmentId);
            if (shipmentType == FrayteShipmentType.Air)
            {
                e8Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/agent-upload-awb/a/" + shipmentId.ToString();
            }
            else if (shipmentType == FrayteShipmentType.Sea)
            {
                e8Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/agent-upload-awb/s/" + shipmentId.ToString();
            }

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E8.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e8Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e8Detail.CargoWiseSo))
            {
                EmailSubject = "Request for Shipping Documents - " + e8Detail.CargoWiseSo;
            }
            else
            {
                EmailSubject = "Request for Shipping Documents - " + e8Detail.ShipmentId.ToString();
            }

            var To = e8Detail.AgentEMail;
            var CC = e8Detail.UserEmail + ";" + e8Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

        public void SendEmail_E81(int shipmentId)
        {
            var e81Detail = new ShipmentRepository().GetE81Detail(shipmentId);
            //e81Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/agent-upload-awb/" + shipmentId.ToString();
            string shipmentType = new ShipmentRepository().GetShipmentCourierType(shipmentId);
            if (shipmentType == FrayteShipmentType.Air)
            {
                e81Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/agent-upload-awb/a/" + shipmentId.ToString();
            }
            else if (shipmentType == FrayteShipmentType.Sea)
            {
                e81Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/agent-upload-awb/s/" + shipmentId.ToString();
            }
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E8.1.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e81Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e81Detail.CargoWiseSo))
            {
                EmailSubject = "Request for Shipping Documents - " + e81Detail.CargoWiseSo + " REMINDER - " + (Convert.ToInt32(e81Detail.RemainderNo) + 1).ToString();
            }
            else
            {
                EmailSubject = "Request for Shipping Documents - " + e81Detail.ShipmentId.ToString() + " REMINDER - " + (Convert.ToInt32(e81Detail.RemainderNo) + 1).ToString();
            }

            var To = e81Detail.AgentEMail;
            var CC = e81Detail.UserEmail + ";" + e81Detail.UserManagerEmail + ";" + e81Detail.AgentManagerMail;
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

        public void SendEmail_E9(int shipmentId)
        {
            var e9Detail = new ShipmentRepository().GetE9Detail(shipmentId);
            e9Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/agent-upload-awb/" + shipmentId.ToString();
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E9.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e9Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e9Detail.CargoWiseSo))
            {
                EmailSubject = "Request for Shipping Documents - " + e9Detail.CargoWiseSo;
            }
            else
            {
                EmailSubject = "Request for Shipping Documents - " + e9Detail.ShipmentId.ToString();
            }

            var To = e9Detail.AgentEMail;
            var CC = e9Detail.UserEmail + ";" + e9Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

        public void SendEmail_E91(int shipmentId)
        {
            var e91Detail = new ShipmentRepository().GetE91Detail(shipmentId);
            e91Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/agent-upload-awb/" + shipmentId.ToString();
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E9.1.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e91Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e91Detail.CargoWiseSo))
            {
                EmailSubject = "Request for Shipping Documents - " + e91Detail.CargoWiseSo + " REMINDER - " + (Convert.ToInt32(e91Detail.RemainderNo) + 1).ToString();
            }
            else
            {
                EmailSubject = "Request for Shipping Documents - " + e91Detail.ShipmentId.ToString() + " REMINDER - " + (Convert.ToInt32(e91Detail.RemainderNo) + 1).ToString();
            }

            var To = e91Detail.AgentEMail;
            var CC = e91Detail.UserEmail + ";" + e91Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

        public void SendEmail_E10(int shipmentId)
        {
            var e10Detail = new ShipmentRepository().GetE10Detail(shipmentId);

            e10Detail.ETATime = e10Detail.ETATime != null ? e10Detail.ETATime : DateTime.Now.TimeOfDay;
            e10Detail.ETDTime = e10Detail.ETDTime == null ? DateTime.Now.TimeOfDay : e10Detail.ETDTime;
            if (!string.IsNullOrEmpty(e10Detail.TimeZoneName))
            {
                e10Detail.ETATime = UtilityRepository.GetTimeZoneTimeSpan(e10Detail.ETATime, e10Detail.TimeZoneName);
                e10Detail.ETDTime = UtilityRepository.GetTimeZoneTimeSpan(e10Detail.ETDTime, e10Detail.TimeZoneName);
            }

            e10Detail.TimeZoneName = e10Detail.OffsetShort;
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E10.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e10Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e10Detail.CargoWiseSo))
            {
                EmailSubject = "NEW Shipment - FRAYTE GLOBAL - " + e10Detail.CargoWiseSo + " - Shippers Documents";
            }
            else
            {
                EmailSubject = "NEW Shipment - FRAYTE GLOBAL - " + e10Detail.ShipmentId.ToString() + " - Shippers Documents";
            }

            //Get Shipment Documents and MAWB documents detail
            //Attachment: “Warehouse Map” + “Shipping Delivery Notice”
            string documentFolder = HttpContext.Current.Server.MapPath("~/UploadFiles/Shipments/" + shipmentId);
            string attachmentPath = "";

            if (!string.IsNullOrEmpty(e10Detail.AWB) && File.Exists(documentFolder + "\\" + e10Detail.AWB))
            {
                attachmentPath = documentFolder + "\\" + e10Detail.AWB;
            }

            if (!string.IsNullOrEmpty(e10Detail.OtherDocs) && File.Exists(documentFolder + "\\" + e10Detail.OtherDocs))
            {
                attachmentPath += ";" + documentFolder + "\\" + e10Detail.OtherDocs;
            }

            if (!string.IsNullOrEmpty(e10Detail.CommercialInvoice) && File.Exists(documentFolder + "\\" + e10Detail.CommercialInvoice))
            {
                attachmentPath += ";" + documentFolder + "\\" + e10Detail.CommercialInvoice;
            }

            if (!string.IsNullOrEmpty(e10Detail.PackingList) && File.Exists(documentFolder + "\\" + e10Detail.PackingList))
            {
                attachmentPath += ";" + documentFolder + "\\" + e10Detail.PackingList;
            }

            if (!string.IsNullOrEmpty(e10Detail.CustomDocument) && File.Exists(documentFolder + "\\" + e10Detail.CustomDocument))
            {
                attachmentPath += ";" + documentFolder + "\\" + e10Detail.CustomDocument;
            }

            if (!string.IsNullOrEmpty(attachmentPath))
            {
                attachmentPath = attachmentPath.Trim().TrimStart(new char[] { ';' });
            }

            var To = e10Detail.AgentEMail;
            var CC = e10Detail.UserEmail + ";" + e10Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, attachmentPath);
        }

        public void SendEmail_E101(int ShipmentId)
        {
            var e101Detail = new ShipmentRepository().GetE101Detail(ShipmentId);
            e101Detail.TimeZoneName = e101Detail.TimeZoneShort;
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E10.1.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e101Detail, null, null);
            var EmailSubject = string.Empty;
            if (!String.IsNullOrEmpty(e101Detail.CargoWiseSo))
            {
                EmailSubject = "TELEX RELEASE CARGO - " + e101Detail.CargoWiseSo;
            }
            else
            {
                EmailSubject = "TELEX RELEASE CARGO - " + e101Detail.ShipmentId.ToString();
            }

            //Attachments: “Telex Stamp” + “Telex Document”
            string documentFolder = HttpContext.Current.Server.MapPath("~/UploadFiles/Shipments/" + ShipmentId);
            string attachmentPath = "";

            if (!string.IsNullOrEmpty(e101Detail.SeaTelexDocument) && File.Exists(documentFolder + "\\" + e101Detail.SeaTelexDocument))
            {
                attachmentPath = documentFolder + "\\" + e101Detail.SeaTelexDocument;
            }

            //if (!string.IsNullOrEmpty(e101Detail.SeaTelexStamp) && File.Exists(documentFolder + "\\" + e101Detail.SeaTelexStamp))
            //{
            //    attachmentPath += ";" + documentFolder + "\\" + e101Detail.SeaTelexStamp;
            //}

            if (!string.IsNullOrEmpty(attachmentPath))
            {
                attachmentPath = attachmentPath.Trim().TrimStart(new char[] { ';' });
            }

            var To = e101Detail.DestinatingEmail;
            var CC = e101Detail.UserEmail + ";" + e101Detail.UserManagerEmail;
            //SendMail(To, CC, EmailSubject, EmailBody, attachmentPath);
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";

            if (!string.IsNullOrEmpty(AppSettings.TOCC))
            {
                FrayteEmail.SendMail(AppSettings.TOCC, "", EmailSubject, EmailBody, attachmentPath, logoImage);
            }
            else
            {
                FrayteEmail.SendMail(To, CC, EmailSubject, EmailBody, attachmentPath, logoImage);
            }
        }

        public void SendEmail_E11(int shipmentId)
        {
            var e11Detail = new ShipmentRepository().GetE11Detail(shipmentId);
            e11Detail.ETATime = e11Detail.ETATime != null ? e11Detail.ETATime : DateTime.Now.TimeOfDay;
            e11Detail.ETDTime = e11Detail.ETDTime != null ? e11Detail.ETDTime : DateTime.Now.TimeOfDay;
            if (!string.IsNullOrEmpty(e11Detail.TimeZoneName))
            {
                e11Detail.ETATime = UtilityRepository.GetTimeZoneTimeSpan(e11Detail.ETATime, e11Detail.TimeZoneName);
                e11Detail.ETDTime = UtilityRepository.GetTimeZoneTimeSpan(e11Detail.ETDTime, e11Detail.TimeZoneName);
            }
            e11Detail.TimeZoneName = e11Detail.OffsetShort;
            e11Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/agent-anticipated/" + shipmentId;
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E11.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e11Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e11Detail.CargoWiseSo))
            {
                EmailSubject = "NEW Shipment - FRAYTE GLOBAL - " + e11Detail.CargoWiseSo + " - Shippers Documents & MAWB";
            }
            else
            {
                EmailSubject = "NEW Shipment - FRAYTE GLOBAL - " + e11Detail.ShipmentId.ToString() + " - Shippers Documents & MAWB";
            }

            //Get Shipment Documents and MAWB documents detail
            //Attachment: “Warehouse Map” + “Shipping Delivery Notice”
            string documentFolder = HttpContext.Current.Server.MapPath("~/UploadFiles/Shipments/" + shipmentId);
            string attachmentPath = "";

            if (!string.IsNullOrEmpty(e11Detail.AWB) && File.Exists(documentFolder + "\\" + e11Detail.AWB))
            {
                attachmentPath = documentFolder + "\\" + e11Detail.AWB;
            }

            if (!string.IsNullOrEmpty(e11Detail.OtherDocs) && File.Exists(documentFolder + "\\" + e11Detail.OtherDocs))
            {
                attachmentPath += ";" + documentFolder + "\\" + e11Detail.OtherDocs;
            }

            if (!string.IsNullOrEmpty(e11Detail.CommercialInvoice) && File.Exists(documentFolder + "\\" + e11Detail.CommercialInvoice))
            {
                attachmentPath += ";" + documentFolder + "\\" + e11Detail.CommercialInvoice;
            }

            if (!string.IsNullOrEmpty(e11Detail.PackingList) && File.Exists(documentFolder + "\\" + e11Detail.PackingList))
            {
                attachmentPath += ";" + documentFolder + "\\" + e11Detail.PackingList;
            }

            if (!string.IsNullOrEmpty(e11Detail.CustomDocument) && File.Exists(documentFolder + "\\" + e11Detail.CustomDocument))
            {
                attachmentPath += ";" + documentFolder + "\\" + e11Detail.CustomDocument;
            }

            if (!string.IsNullOrEmpty(attachmentPath))
            {
                attachmentPath = attachmentPath.Trim().TrimStart(new char[] { ';' });
            }

            var To = e11Detail.AgentEMail;
            var CC = e11Detail.UserEmail + ";" + e11Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, attachmentPath);
        }

        public void SendEmail_E12(int shipmentId)
        {
            var e12Detail = new ShipmentRepository().GetE12Detail(shipmentId);

            e12Detail.ETATime = e12Detail.ETATime.HasValue ? e12Detail.ETATime : DateTime.Now.TimeOfDay;
            e12Detail.ETDTime = e12Detail.ETDTime.HasValue ? e12Detail.ETDTime : DateTime.Now.TimeOfDay;
            if (!string.IsNullOrEmpty(e12Detail.TimeZoneName))
            {
                e12Detail.ETDTime = UtilityRepository.GetTimeZoneTimeSpan(e12Detail.ETDTime, e12Detail.TimeZoneName);
                e12Detail.ETATime = UtilityRepository.GetTimeZoneTimeSpan(e12Detail.ETATime, e12Detail.TimeZoneName);
            }
            e12Detail.TimeZoneName = e12Detail.OffsetShort;
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E12.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e12Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e12Detail.CargoWiseSo))
            {
                EmailSubject = "NEW Shipment - FRAYTE GLOBAL - " + e12Detail.CargoWiseSo + " - Estimated Delivery Alert ";
            }
            else
            {
                EmailSubject = "NEW Shipment - FRAYTE GLOBAL - " + e12Detail.ShipmentId.ToString() + " - Estimated Delivery Alert ";
            }

            var To = e12Detail.ClientEmail + ";" + e12Detail.ReceiverEmail + ";" + e12Detail.ShipperEmail;
            var CC = e12Detail.UserEmail + ";" + e12Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

        public void SendEmail_E13(int shipmentId)
        {
            var e13Detail = new ShipmentRepository().GetE13Detail(shipmentId);

            e13Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/agent-upload-pod/" + shipmentId.ToString();

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E13.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e13Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e13Detail.CargoWiseSo))
            {
                EmailSubject = "Request for Proof of Delivery - " + e13Detail.CargoWiseSo;
            }
            else
            {
                EmailSubject = "Request for Proof of Delivery - " + e13Detail.ShipmentId.ToString();
            }

            var To = e13Detail.AgentEMail;
            var CC = e13Detail.UserEmail + ";" + e13Detail.UserManagerEmail;
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

        public void SendEmail_E131(int shipmentId)
        {
            var e131Detail = new ShipmentRepository().GetE131Detail(shipmentId);
            e131Detail.UploadLinkHref = AppSettings.HostName + "/index.html#/public/agent-upload-pod/" + shipmentId.ToString();
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E13.1.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e131Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e131Detail.CargoWiseSo))
            {
                EmailSubject = "Request for MAWB - " + e131Detail.ShipmentId.ToString() + " REMINDER - " + (Convert.ToInt32(e131Detail.RemainderNo) + 1).ToString() + e131Detail.CargoWiseSo;
            }
            else
            {
                EmailSubject = "Request for MAWB - " + e131Detail.ShipmentId.ToString() + " REMINDER - " + (Convert.ToInt32(e131Detail.RemainderNo) + 1).ToString() + e131Detail.ShipmentId.ToString();
            }

            var To = e131Detail.AgentEMail;
            var CC = e131Detail.UserEmail + ";" + e131Detail.UserManagerEmail + ";" + e131Detail.AgentManagerMail;
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

        public void SendEmail_E14(int shipmentId)
        {
            var e14Detail = new ShipmentRepository().GetE14Detail(shipmentId);
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E14.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e14Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e14Detail.CargoWiseSo))
            {
                EmailSubject = "Confirmation of Consignment delivery - " + e14Detail.CargoWiseSo;
            }
            else
            {
                EmailSubject = "Confirmation of Consignment delivery - " + e14Detail.ShipmentId.ToString();
            }

            var To = e14Detail.ClientEmail + ";" + e14Detail.ShipperEmail + ";" + e14Detail.ReceiverEmail;
            var CC = e14Detail.UserEmail + ";" + e14Detail.UserManagerEmail;
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";

            string attachmentFolder = HttpContext.Current.Server.MapPath("~/UploadFiles/Shipments/" + e14Detail.ShipmentId + "//");
            string AttachmentPath = "";

            if (!string.IsNullOrEmpty(e14Detail.FinalImage))
            {
                AttachmentPath = attachmentFolder + e14Detail.FinalImage;
            }
            if (!string.IsNullOrEmpty(AppSettings.TOCC))
            {
                FrayteEmail.SendMail(AppSettings.TOCC, "", EmailSubject, EmailBody, AttachmentPath, logoImage);
            }
            else
            {
                FrayteEmail.SendMail(To, CC, EmailSubject, EmailBody, AttachmentPath, logoImage);
            }
        }

        public void SendEmail_E15(int shipmentId)
        {
            var e15Detail = new ShipmentRepository().GetE15Detail(shipmentId);
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E15.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e15Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e15Detail.CargoWiseSo))
            {
                EmailSubject = "Notification of Shipment dispatch by Courier - " + e15Detail.CargoWiseSo;
            }
            else
            {
                EmailSubject = "Notification of Shipment dispatch by Courier - " + e15Detail.ShipmentId.ToString();
            }

            //Attachment: “Outstanding Documents” + “AWB Document”
            string documentFolder = HttpContext.Current.Server.MapPath("~/UploadFiles/Shipments/" + shipmentId);
            string attachmentPath = "";

            if (!string.IsNullOrEmpty(e15Detail.AirWayBillDocument) && File.Exists(documentFolder + "\\" + e15Detail.AirWayBillDocument))
            {
                attachmentPath = documentFolder + "\\" + e15Detail.AirWayBillDocument;
            }
            if (!string.IsNullOrEmpty(e15Detail.CommercialInvoice) && File.Exists(documentFolder + "\\" + e15Detail.CommercialInvoice))
            {
                attachmentPath += ";" + documentFolder + "\\" + e15Detail.CommercialInvoice;
            }

            if (!string.IsNullOrEmpty(e15Detail.PackingList) && File.Exists(documentFolder + "\\" + e15Detail.PackingList))
            {
                attachmentPath += ";" + documentFolder + "\\" + e15Detail.PackingList;
            }

            if (!string.IsNullOrEmpty(e15Detail.CustomDocument) && File.Exists(documentFolder + "\\" + e15Detail.CustomDocument))
            {
                attachmentPath += ";" + documentFolder + "\\" + e15Detail.CustomDocument;
            }

            if (!string.IsNullOrEmpty(attachmentPath))
            {
                attachmentPath = attachmentPath.Trim().TrimStart(new char[] { ';' });
            }

            var To = e15Detail.ShipperEmail;
            var CC = e15Detail.UserEmail + ";" + e15Detail.UserManagerEmail;
            //SendMail(To, CC, EmailSubject, EmailBody, attachmentPath);
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";

            if (!string.IsNullOrEmpty(AppSettings.TOCC))
            {
                FrayteEmail.SendMail(AppSettings.TOCC, "", EmailSubject, EmailBody, attachmentPath, logoImage);
            }
            else
            {
                FrayteEmail.SendMail(To, CC, EmailSubject, EmailBody, attachmentPath, logoImage);
            }
        }

        public void SendEmail_E16(int shipmentId)
        {
            var e16Detail = new ShipmentRepository().GetE16Detail(shipmentId);
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/E16.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, e16Detail, null, null);
            string EmailSubject = String.Empty;
            if (!String.IsNullOrEmpty(e16Detail.CargoWiseSo))
            {
                EmailSubject = "Notification of Shipment dispatch by Courier" + e16Detail.CargoWiseSo;
            }
            else
            {
                EmailSubject = "Notification of Shipment dispatch by Courier" + e16Detail.ShipmentId.ToString();
            }

            string documentFolder = HttpContext.Current.Server.MapPath("~/UploadFiles/Shipments/" + shipmentId);
            string attachmentPath = "";

            if (!string.IsNullOrEmpty(e16Detail.AirWayBillDocument) && File.Exists(documentFolder + "\\" + e16Detail.AirWayBillDocument))
            {
                attachmentPath = documentFolder + "\\" + e16Detail.AirWayBillDocument;
            }
            if (!string.IsNullOrEmpty(e16Detail.CommercialInvoice) && File.Exists(documentFolder + "\\" + e16Detail.CommercialInvoice))
            {
                attachmentPath += ";" + documentFolder + "\\" + e16Detail.CommercialInvoice;
            }

            if (!string.IsNullOrEmpty(e16Detail.PackingList) && File.Exists(documentFolder + "\\" + e16Detail.PackingList))
            {
                attachmentPath += ";" + documentFolder + "\\" + e16Detail.PackingList;
            }

            if (!string.IsNullOrEmpty(e16Detail.CustomDocument) && File.Exists(documentFolder + "\\" + e16Detail.CustomDocument))
            {
                attachmentPath += ";" + documentFolder + "\\" + e16Detail.CustomDocument;
            }

            if (!string.IsNullOrEmpty(attachmentPath))
            {
                attachmentPath = attachmentPath.Trim().TrimStart(new char[] { ';' });
            }

            var To = e16Detail.ReceiverEmail;
            var CC = e16Detail.UserEmail + ";" + e16Detail.UserManagerEmail;
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";

            if (!string.IsNullOrEmpty(AppSettings.TOCC))
            {
                FrayteEmail.SendMail(AppSettings.TOCC, "", EmailSubject, EmailBody, attachmentPath, logoImage);
            }
            else
            {
                FrayteEmail.SendMail(To, CC, EmailSubject, EmailBody, attachmentPath, logoImage);
            }
        }

        public void SendMail(string toEmail, string ccEmail, string bcc, string ReplyTo, string DisplayName, string EmailSubject, string EmailBody)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";

            if (!string.IsNullOrEmpty(AppSettings.TOCC))
            {
                FrayteEmail.SendMail(AppSettings.TOCC, "", "", "", DisplayName, EmailSubject, EmailBody, "", logoImage);
            }
            else
            {
                FrayteEmail.SendMail(toEmail, ccEmail, "", "", DisplayName, EmailSubject, EmailBody, "", logoImage);
            }
        }

        public void SendMail(string toEmail, string ccEmail, string EmailSubject, string EmailBody, string AttachmentFilePath)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";

            if (!string.IsNullOrEmpty(AppSettings.TOCC))
            {
                FrayteEmail.SendMail(AppSettings.TOCC, "", EmailSubject, EmailBody, AttachmentFilePath, logoImage);
            }
            else
            {
                FrayteEmail.SendMail(toEmail, ccEmail, EmailSubject, EmailBody, AttachmentFilePath, logoImage);
            }
        }

        public void Send_PreAlertEmail(string toEmail, string ccEmail, string bccEmail, string replyTo, string EmailSubject, string EmailBody, string AttachmentFilePath, string Status)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);

            if (!string.IsNullOrEmpty(AppSettings.TOCC))
            {
                FrayteEmail.SendPreAlertMail(AppSettings.TOCC, "", "", "", EmailSubject, EmailBody, AttachmentFilePath, ImagePath, Status);
            }
            else
            {
                FrayteEmail.SendPreAlertMail(toEmail, ccEmail, bccEmail, replyTo, EmailSubject, EmailBody, AttachmentFilePath, ImagePath, Status);
            }

        }

        public void SendMail_New(string toEmail, string ccEmail, string DisplayName, string EmailSubject, string EmailBody, string AttachmentFilePath, string Status)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            string trackImage = AppSettings.EmailServicePath + "/Images/TrackShipment.png";
            string amdentImage = AppSettings.EmailServicePath + "/Images/Amend.png";
            string confirmImage = AppSettings.EmailServicePath + "/Images/Confirm.png";
            string rejectImage = AppSettings.EmailServicePath + "/Images/Reject.png";

            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);
            ImagePath.Add(trackImage);
            ImagePath.Add(amdentImage);
            ImagePath.Add(confirmImage);
            ImagePath.Add(rejectImage);

            if (!string.IsNullOrEmpty(AppSettings.TOCC))
            {
                FrayteEmail.SendMail(AppSettings.TOCC, "", DisplayName, EmailSubject, EmailBody, AttachmentFilePath, ImagePath, Status);
            }
            else
            {
                FrayteEmail.SendMail(toEmail, ccEmail, DisplayName, EmailSubject, EmailBody, AttachmentFilePath, ImagePath, Status);
            }
        }

        public void SetShipmentEmailService(Guid shipmentemailserviceid, DateTime date, TimeSpan? t, int emailsentcount)
        {
            dbContext.spSet_ShipmentEmailService(shipmentemailserviceid, date, t, emailsentcount);
        }

        public void DeleteShipmentEmailService(Guid shipmentemailserviceid)
        {
            dbContext.spDelete_ShipmentEmailServices(shipmentemailserviceid);
        }

        public void DeleteShipmentEmailService(string emailTemplateId, int shipmentId)
        {
            var result = dbContext.ShipmentEmailServices.Where(p => p.EmailTemplateId == emailTemplateId && p.ShipmentId == shipmentId).FirstOrDefault();

            if (result != null)
            {
                dbContext.ShipmentEmailServices.Remove(result);
                dbContext.SaveChanges();
            }
        }

        public List<FrayteShipmentEmailDetail> GetShipmentEmailDetail()
        {
            var shipmentemaildetail = dbContext.spGet_ShipmentEmailServices();
            List<FrayteShipmentEmailDetail> FrayteShipmentEmailDetail = new List<FrayteShipmentEmailDetail>();
            FrayteShipmentEmailDetail fsh;
            foreach (var fshdetail in shipmentemaildetail)
            {
                if (fshdetail.WorkingStartTime != null && fshdetail.WorkingEndTime != null)
                {
                    fsh = new FrayteShipmentEmailDetail();

                    fsh.ShipmentEmailServiceId = (Guid)fshdetail.ShipmentEmailServiceId;
                    fsh.ShipmentId = (int)fshdetail.ShipmentId;
                    fsh.ReminderIn = (int)fshdetail.ReminderIn;
                    fsh.IsRepeatReminder = (bool)fshdetail.IsRepeatReminder;
                    fsh.RemindType = fshdetail.RemindType;
                    fsh.EmailSentCount = (int)fshdetail.EmailSentCount;
                    fsh.EmailSentToManagerCount = fshdetail.EmailSentToManagerCount == null ? 0 : (int)fshdetail.EmailSentToManagerCount;
                    fsh.EmailTemplateId = fshdetail.EmailTemplateId;
                    fsh.EmailSentDate = fshdetail.EmailSentDate;
                    fsh.EmailSentTime = (TimeSpan)fshdetail.EmailSentTime;
                    fsh.ManagerEmailTemplateId = fshdetail.ManagerEmailTemplateId;
                    fsh.WorkingStartTime = (TimeSpan)fshdetail.WorkingStartTime;
                    fsh.WorkingEndTime = (TimeSpan)fshdetail.WorkingEndTime;
                    fsh.TimeZoneName = fshdetail.TimeZoneName;
                    fsh.IsPublicHoliday = (bool)fshdetail.IsPublicHoliday;
                    FrayteShipmentEmailDetail.Add(fsh);
                }
            }
            return FrayteShipmentEmailDetail;
        }

        public void LogForEmailSending(string emailTemplateId, int shipmentId, DateTime? date, TimeSpan emailSentTime, int hour = 4)
        {
            ShipmentEmailService emailLogService = new ShipmentEmailService();
            emailLogService.ShipmentEmailServiceId = Guid.NewGuid();
            switch (emailTemplateId)
            {
                case EmailTempletType.E1:
                case EmailTempletType.E4_1:
                case EmailTempletType.E6_1:
                case EmailTempletType.E6_2:
                case EmailTempletType.E6_3:
                case EmailTempletType.E6_5:
                case EmailTempletType.E6_6:
                case EmailTempletType.E7_1:
                case EmailTempletType.E8_1:
                case EmailTempletType.E9_1:
                case EmailTempletType.E13_1:
                    emailLogService.ShipmentId = shipmentId;
                    emailLogService.EmailSentDate = date;
                    emailLogService.EmailSentTime = emailSentTime;
                    emailLogService.ReminderIn = 1;
                    emailLogService.RemindType = EmailReminderType.After;
                    emailLogService.EmailTemplateId = emailTemplateId;
                    emailLogService.IsRepeatReminder = true;
                    emailLogService.EmailSentCount = 0;
                    emailLogService.EmailSentToManagerCount = 3;
                    emailLogService.ManagerEmailTemplateId = emailTemplateId;
                    dbContext.ShipmentEmailServices.Add(emailLogService);
                    dbContext.SaveChanges();

                    break;
                case EmailTempletType.E7:
                    emailLogService.ShipmentId = shipmentId;
                    emailLogService.EmailSentDate = date;
                    emailLogService.EmailSentTime = emailSentTime;
                    emailLogService.ReminderIn = hour;
                    emailLogService.RemindType = EmailReminderType.After;
                    emailLogService.EmailTemplateId = emailTemplateId;
                    emailLogService.IsRepeatReminder = false;
                    emailLogService.EmailSentCount = 0;
                    emailLogService.EmailSentToManagerCount = 3;
                    emailLogService.ManagerEmailTemplateId = "";
                    dbContext.ShipmentEmailServices.Add(emailLogService);
                    dbContext.SaveChanges();
                    break;
                case EmailTempletType.E8:
                    emailLogService.ShipmentId = shipmentId;
                    emailLogService.EmailSentDate = date;
                    emailLogService.EmailSentTime = emailSentTime;
                    emailLogService.ReminderIn = hour;
                    emailLogService.RemindType = EmailReminderType.After;
                    emailLogService.EmailTemplateId = emailTemplateId;
                    emailLogService.IsRepeatReminder = false;
                    emailLogService.EmailSentCount = 0;
                    emailLogService.EmailSentToManagerCount = 3;
                    emailLogService.ManagerEmailTemplateId = "";
                    dbContext.ShipmentEmailServices.Add(emailLogService);
                    dbContext.SaveChanges();
                    break;
                case EmailTempletType.E9:
                    emailLogService.ShipmentId = shipmentId;
                    emailLogService.EmailSentDate = date;
                    emailLogService.EmailSentTime = emailSentTime;
                    emailLogService.ReminderIn = 1;
                    emailLogService.RemindType = EmailReminderType.After;
                    emailLogService.EmailTemplateId = emailTemplateId;
                    emailLogService.IsRepeatReminder = false;
                    emailLogService.EmailSentCount = 0;
                    emailLogService.EmailSentToManagerCount = 3;
                    emailLogService.ManagerEmailTemplateId = "";
                    dbContext.ShipmentEmailServices.Add(emailLogService);
                    dbContext.SaveChanges();
                    break;
                case EmailTempletType.E13:
                    emailLogService.ShipmentId = shipmentId;
                    emailLogService.EmailSentDate = date;
                    emailLogService.EmailSentTime = emailSentTime;
                    emailLogService.ReminderIn = 2;
                    emailLogService.RemindType = EmailReminderType.After;
                    emailLogService.EmailTemplateId = emailTemplateId;
                    emailLogService.IsRepeatReminder = false;
                    emailLogService.EmailSentCount = 0;
                    emailLogService.EmailSentToManagerCount = 3;
                    emailLogService.ManagerEmailTemplateId = "";
                    dbContext.ShipmentEmailServices.Add(emailLogService);
                    dbContext.SaveChanges();
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region -- Direct Booking --

        public void SendDirectBookingConfirmationMail(DirectBookingShipmentDraftDetail shipmentDetail, int DirectShipmentid)
        {
            try
            {
                var userDetail = (from r in dbContext.Users
                                  join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                                  where r.UserId == shipmentDetail.CreatedBy
                                  select new
                                  {
                                      Email = r.Email,
                                      TimeZone = r.TimezoneId,
                                      UserEmail = r.UserEmail,
                                      RoleId = ur.RoleId
                                  }).FirstOrDefault();

                //Get Customer Name and Customer User Detail
                var customerDetail = (from u in dbContext.Users
                                      where u.UserId == (userDetail.RoleId == (int)FrayteUserRole.UserCustomer ? shipmentDetail.CreatedBy : shipmentDetail.CustomerId)
                                      select new
                                      {
                                          CustomerName = u.ContactName,
                                          CustomerEmail = u.UserEmail,
                                          CompanyName = u.CompanyName
                                      }).FirstOrDefault();

                var staffdetail = GetAssociateStaffDetail(shipmentDetail.CustomerId, shipmentDetail.RoleId, shipmentDetail.LogInUseId);

                string ShipmentMethod = string.Empty;
                string Shipment = string.Empty;
                if (shipmentDetail.CustomerRateCard.LogisticServiceId > 0)
                {
                    var logistic = (from lsca in dbContext.LogisticServiceCourierAccounts
                                    join ls in dbContext.LogisticServices on lsca.LogisticServiceId equals ls.LogisticServiceId
                                    where lsca.LogisticServiceId == shipmentDetail.CustomerRateCard.LogisticServiceId
                                    select ls).FirstOrDefault();

                    if (logistic != null)
                    {
                        if (logistic.RateType != null && logistic.RateType != "")
                        {
                            ShipmentMethod = logistic.LogisticCompanyDisplay + " (" + logistic.RateType + ")";
                            Shipment = logistic.LogisticCompany + logistic.RateType;
                        }
                        else
                        {
                            ShipmentMethod = logistic.LogisticCompanyDisplay;
                        }
                    }
                }
                else
                {
                    if (shipmentDetail.CustomerRateCard.RateType != null && shipmentDetail.CustomerRateCard.RateType != "")
                    {
                        ShipmentMethod = shipmentDetail.CustomerRateCard.DisplayName + " (" + shipmentDetail.CustomerRateCard.RateType + ")";
                        Shipment = shipmentDetail.CustomerRateCard.CourierName + shipmentDetail.CustomerRateCard.RateType;
                    }
                    else
                    {
                        ShipmentMethod = shipmentDetail.CustomerRateCard.DisplayName;
                    }
                }

                var FrayteNo = dbContext.DirectShipments.Find(DirectShipmentid);

                var operationzone = UtilityRepository.GetOperationZone();

                #region TrackingNo, TrackingURL

                string TrackNo = string.Empty;
                string TrackUrl = string.Empty;
                var TrackingNo = dbContext.DirectShipments.Where(p => p.DirectShipmentId == DirectShipmentid).FirstOrDefault();
                if (TrackingNo != null)
                {
                    if (TrackingNo.TrackingDetail != null)
                    {
                        if (TrackingNo.TrackingDetail.Contains("Order"))
                        {
                            TrackNo = TrackingNo.TrackingDetail.Replace("Order_", "");
                            TrackUrl = AppSettings.TrackingUrl + "/tracking-detail/" + Shipment + "/" + TrackNo + "/";
                        }
                        else
                        {
                            if (TrackingNo.LogisticServiceType == "DHL" || TrackingNo.LogisticServiceType == "TNT" || TrackingNo.LogisticServiceType == "UPS")
                            {
                                TrackNo = TrackingNo.TrackingDetail;
                                TrackUrl = AppSettings.TrackingUrl + "/tracking-detail/" + Shipment + "/" + TrackNo + "/";
                            }
                            else
                            {
                                TrackNo = TrackingNo.TrackingDetail;
                                TrackUrl = AppSettings.TrackingUrl + "/tracking-detail/UKEUShipment/" + TrackingNo.TrackingDetail + "/";
                            }
                        }
                    }
                }

                #endregion

                var userInfo = (from r in dbContext.Users
                                join tz in dbContext.Timezones on r.TimezoneId equals tz.TimezoneId
                                where r.UserId == shipmentDetail.CreatedBy
                                select tz
                       ).FirstOrDefault();

                var UserTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userInfo.Name);
                var dbShipment = dbContext.DirectShipments.Find(DirectShipmentid);
                var CreatedOnDate = UtilityRepository.UtcDateToOtherTimezone(dbShipment.CreatedOn, dbShipment.CreatedOn.TimeOfDay, UserTimeZoneInfo).Item1;
                var CreatedOnTime = UtilityRepository.GetFormattedTimeFromString(UtilityRepository.UtcDateToOtherTimezone(dbShipment.CreatedOn, dbShipment.CreatedOn.TimeOfDay, UserTimeZoneInfo).Item2);

                DynamicViewBag viewBag = new DynamicViewBag();
                viewBag.AddValue("CreatedOn", CreatedOnDate.ToString("dd-MMM-yyyy") + " " + CreatedOnTime + " - " + userInfo.OffsetShort);
                viewBag.AddValue("TotalWeight", FrayteNo.ChargeableWeight);
                viewBag.AddValue("ShipmentMethod", ShipmentMethod);
                viewBag.AddValue("TrackingNo", TrackNo);
                viewBag.AddValue("TrackingURL", TrackUrl);
                viewBag.AddValue("TotalCartoon", shipmentDetail.Packages.Sum(p => p.CartoonValue));
                viewBag.AddValue("CustomerName", customerDetail.CustomerName);
                viewBag.AddValue("CustomerEmail", customerDetail.CustomerEmail);
                viewBag.AddValue("CompanyName", customerDetail.CompanyName);
                viewBag.AddValue("ImageHeader", "FrayteLogo");
                viewBag.AddValue("TrackButton", "TrackShipment");
                viewBag.AddValue("FrayteNumber", FrayteNo.FrayteNumber);
                viewBag.AddValue("PickUp", FrayteNo.PickUpRef == null ? "" : FrayteNo.PickUpRef);
                viewBag.AddValue("OperationZoneId", shipmentDetail.OpearionZoneId);

                var userCustomerDetail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == shipmentDetail.CustomerId).FirstOrDefault();
                if (userCustomerDetail != null)
                {
                    viewBag.AddValue("SiteAddress", userCustomerDetail.Website);
                    viewBag.AddValue("UserName", userCustomerDetail.OperationStaff);
                    viewBag.AddValue("UserPosition", userCustomerDetail.UserPosition);
                    viewBag.AddValue("UserEmail", userCustomerDetail.OperationStaffEmail);
                    viewBag.AddValue("UserPhone", userCustomerDetail.OperationStaffPhone);
                    viewBag.AddValue("KindRegards", userCustomerDetail.KindRegards);
                }
                else
                {
                    if (shipmentDetail.OpearionZoneId == 1)
                    {
                        viewBag.AddValue("UserPhone", "(+852) 2148 4880");
                    }
                    else
                    {
                        viewBag.AddValue("UserPhone", "(+44) 01792 678017");
                    }

                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                    viewBag.AddValue("UserName", staffdetail.OperationStaffName);
                    viewBag.AddValue("UserPosition", staffdetail.DeptName);
                    viewBag.AddValue("UserEmail", staffdetail.OperationStaffEmail);
                    viewBag.AddValue("KindRegards", "FRAYTE GLOBAL");
                }

                //Get UserType (Normal/Special)
                var UserType = UtilityRepository.GetUserType(shipmentDetail.CustomerId);

                string template = string.Empty;
                string Status = string.Empty;

                if (userDetail.RoleId == (int)FrayteUserRole.UserCustomer)
                {
                    template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + shipmentDetail.CustomerId + "/" + "DirectBookingConfirmation.cshtml");
                }
                else
                {
                    if (UserType != null)
                    {
                        if (UserType == FrayteUserType.SPECIAL)
                        {
                            template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + shipmentDetail.CustomerId + "/" + "DirectBookingConfirmation.cshtml");
                        }
                        else if (UserType == FrayteUserType.NORMAL)
                        {
                            Status = "Confirmation";
                            template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/DirectBookingConfirmation.cshtml");
                        }
                    }
                    else
                    {
                        Status = "Confirmation";
                        template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/DirectBookingConfirmation.cshtml");
                    }
                }

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, shipmentDetail, viewBag, null);
                string EmailSubject = string.Empty;
                EmailSubject = "Booking - " + shipmentDetail.CustomerRateCard.DisplayName + (!string.IsNullOrEmpty(shipmentDetail.CustomerRateCard.RateType) ? " " + shipmentDetail.CustomerRateCard.RateType : "") + " - FR#" + FrayteNo.FrayteNumber;

                var To = customerDetail.CustomerEmail;

                #region Attach Labels 

                string Attachment = UtilityRepository.PackageLabelPath(DirectShipmentid, shipmentDetail.CustomerRateCard.CourierName, shipmentDetail.CustomerRateCard.RateType);

                #endregion

                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Customer Email " + FrayteNo.FrayteNumber));
                //Send mail to Customer
                Send_FrayteEmail(To, AppSettings.TOCC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment, Status, shipmentDetail.CustomerId);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(EmailBody));

                //Send mail to Ship From
                if (!string.IsNullOrEmpty(shipmentDetail.ShipFrom.Email))
                {
                    viewBag = new DynamicViewBag();
                    viewBag.AddValue("CreatedOn", CreatedOnDate.ToString("dd-MMM-yyyy") + " " + CreatedOnTime + " - " + userInfo.OffsetShort);
                    viewBag.AddValue("TotalWeight", FrayteNo.ChargeableWeight);
                    viewBag.AddValue("ShipmentMethod", ShipmentMethod);
                    viewBag.AddValue("TrackingNo", TrackNo);
                    viewBag.AddValue("TrackingURL", TrackUrl);
                    viewBag.AddValue("TotalCartoon", shipmentDetail.Packages.Sum(p => p.CartoonValue));
                    viewBag.AddValue("CustomerName", shipmentDetail.ShipFrom.FirstName + " " + shipmentDetail.ShipFrom.LastName);
                    viewBag.AddValue("CustomerEmail", shipmentDetail.ShipFrom.Email);
                    viewBag.AddValue("CompanyName", shipmentDetail.ShipFrom.CompanyName);
                    viewBag.AddValue("ImageHeader", "FrayteLogo");
                    viewBag.AddValue("TrackButton", "TrackShipment");
                    viewBag.AddValue("FrayteNumber", FrayteNo.FrayteNumber);
                    viewBag.AddValue("PickUp", FrayteNo.PickUpRef == null ? "" : FrayteNo.PickUpRef);
                    viewBag.AddValue("OperationZoneId", shipmentDetail.OpearionZoneId);

                    if (userCustomerDetail != null)
                    {
                        viewBag.AddValue("SiteAddress", userCustomerDetail.Website);
                        viewBag.AddValue("UserName", userCustomerDetail.OperationStaff);
                        viewBag.AddValue("UserPosition", userCustomerDetail.UserPosition);
                        viewBag.AddValue("UserEmail", userCustomerDetail.OperationStaffEmail);
                        viewBag.AddValue("UserPhone", userCustomerDetail.OperationStaffPhone);
                        viewBag.AddValue("KindRegards", userCustomerDetail.KindRegards);
                    }
                    else
                    {
                        if (shipmentDetail.OpearionZoneId == 1)
                        {
                            viewBag.AddValue("UserPhone", "(+852) 2148 4880");
                        }
                        else
                        {
                            viewBag.AddValue("UserPhone", "(+44) 01792 678017");
                        }

                        viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                        viewBag.AddValue("UserName", staffdetail.OperationStaffName);
                        viewBag.AddValue("UserPosition", staffdetail.DeptName);
                        viewBag.AddValue("UserEmail", staffdetail.OperationStaffEmail);
                        viewBag.AddValue("KindRegards", "FRAYTE GLOBAL");
                    }

                    if (userDetail.RoleId == (int)FrayteUserRole.UserCustomer)
                    {
                        template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + shipmentDetail.CustomerId + "/" + "DirectBookingConfirmation.cshtml");
                    }
                    else
                    {
                        if (UserType != null)
                        {
                            if (UserType == FrayteUserType.SPECIAL)
                            {
                                template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + shipmentDetail.CustomerId + "/" + "DirectBookingConfirmation.cshtml");
                            }
                            else if (UserType == FrayteUserType.NORMAL)
                            {
                                Status = "Confirmation";
                                template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/DirectBookingConfirmation.cshtml");
                            }
                        }
                        else
                        {
                            Status = "Confirmation";
                            template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/DirectBookingConfirmation.cshtml");
                        }
                    }

                    templateService = new TemplateService();
                    EmailBody = templateService.Parse(template, shipmentDetail, viewBag, null);

                    To = shipmentDetail.ShipFrom.Email;

                    #region Attach Labels

                    string Attachment1 = UtilityRepository.PackageLabelPath(DirectShipmentid, shipmentDetail.CustomerRateCard.CourierName, shipmentDetail.CustomerRateCard.RateType);

                    #endregion

                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Shipper Email " + FrayteNo.FrayteNumber));
                    Send_FrayteEmail(To, AppSettings.TOCC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment, Status, shipmentDetail.CustomerId);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(EmailBody));
                }

                //Send mail to Ship To
                if (!string.IsNullOrEmpty(shipmentDetail.ShipTo.Email))
                {
                    viewBag = new DynamicViewBag();
                    viewBag.AddValue("CreatedOn", CreatedOnDate.ToString("dd-MMM-yyyy") + " " + CreatedOnTime + " - " + userInfo.OffsetShort);
                    viewBag.AddValue("TotalWeight", FrayteNo.ChargeableWeight);
                    viewBag.AddValue("ShipmentMethod", ShipmentMethod);
                    viewBag.AddValue("TrackingNo", TrackNo);
                    viewBag.AddValue("TrackingURL", TrackUrl);
                    viewBag.AddValue("TotalCartoon", shipmentDetail.Packages.Sum(p => p.CartoonValue));
                    viewBag.AddValue("CustomerName", shipmentDetail.ShipTo.FirstName + " " + shipmentDetail.ShipTo.LastName);
                    viewBag.AddValue("CustomerEmail", shipmentDetail.ShipTo.Email);
                    viewBag.AddValue("CompanyName", shipmentDetail.ShipTo.CompanyName);
                    viewBag.AddValue("ImageHeader", "FrayteLogo");
                    viewBag.AddValue("TrackButton", "TrackShipment");
                    viewBag.AddValue("FrayteNumber", FrayteNo.FrayteNumber);
                    viewBag.AddValue("PickUp", FrayteNo.PickUpRef == null ? "" : FrayteNo.PickUpRef);
                    viewBag.AddValue("OperationZoneId", shipmentDetail.OpearionZoneId);

                    if (userCustomerDetail != null)
                    {
                        viewBag.AddValue("SiteAddress", userCustomerDetail.Website);
                        viewBag.AddValue("UserName", userCustomerDetail.OperationStaff);
                        viewBag.AddValue("UserPosition", userCustomerDetail.UserPosition);
                        viewBag.AddValue("UserEmail", userCustomerDetail.OperationStaffEmail);
                        viewBag.AddValue("UserPhone", userCustomerDetail.OperationStaffPhone);
                        viewBag.AddValue("KindRegards", userCustomerDetail.KindRegards);
                    }
                    else
                    {
                        if (shipmentDetail.OpearionZoneId == 1)
                        {
                            viewBag.AddValue("UserPhone", "(+852) 2148 4880");
                        }
                        else
                        {
                            viewBag.AddValue("UserPhone", "(+44) 01792 678017");
                        }

                        viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                        viewBag.AddValue("UserName", staffdetail.OperationStaffName);
                        viewBag.AddValue("UserPosition", staffdetail.DeptName);
                        viewBag.AddValue("UserEmail", staffdetail.OperationStaffEmail);
                        viewBag.AddValue("KindRegards", "FRAYTE GLOBAL");
                    }

                    if (userDetail.RoleId == (int)FrayteUserRole.UserCustomer)
                    {
                        template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + shipmentDetail.CustomerId + "/" + "DirectBookingConfirmation.cshtml");
                    }
                    else
                    {
                        if (UserType != null)
                        {
                            if (UserType == FrayteUserType.SPECIAL)
                            {
                                template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + shipmentDetail.CustomerId + "/" + "DirectBookingConfirmation.cshtml");
                            }
                            else if (UserType == FrayteUserType.NORMAL)
                            {
                                Status = "Confirmation";
                                template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/DirectBookingConfirmation.cshtml");
                            }
                        }
                        else
                        {
                            Status = "Confirmation";
                            template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/DirectBookingConfirmation.cshtml");
                        }
                    }
                    templateService = new TemplateService();
                    EmailBody = templateService.Parse(template, shipmentDetail, viewBag, null);

                    To = shipmentDetail.ShipTo.Email;

                    #region Attach Labels

                    string Attachment2 = UtilityRepository.PackageLabelPath(DirectShipmentid, shipmentDetail.CustomerRateCard.CourierName, shipmentDetail.CustomerRateCard.RateType);

                    #endregion

                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Receiver Email " + FrayteNo.FrayteNumber));
                    Send_FrayteEmail(To, AppSettings.TOCC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment, Status, shipmentDetail.CustomerId);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(EmailBody));
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void SendDirectBookingLabel(string ToMail, int DirectShipmentId, string CourierName, string CallingType, int RoleId, int LogInUserId)
        {
            DirectBookingShipmentDetail dbDetail = new DirectShipmentRepository().GetDirectBookingDetail(DirectShipmentId, "");

            var shipmentDetail = new DirectShipmentRepository().GetDirectShipmentDraftDetail(DirectShipmentId, "ShipmentClone");

            var logistic = (from lsca in dbContext.LogisticServiceCourierAccounts
                            join ls in dbContext.LogisticServices on lsca.LogisticServiceId equals ls.LogisticServiceId
                            where lsca.LogisticServiceId == dbDetail.CustomerRateCard.LogisticServiceId
                            select ls).FirstOrDefault();

            string ShipmentMethod = string.Empty;
            string Shipment = string.Empty;
            if (logistic != null)
            {
                if (logistic.RateType != null && logistic.RateType != "")
                {
                    ShipmentMethod = logistic.LogisticCompanyDisplay + " (" + logistic.RateType + ")";
                    Shipment = logistic.LogisticCompany + logistic.RateType;
                }
                else
                {
                    ShipmentMethod = logistic.LogisticCompanyDisplay;
                }
            }

            #region TrackingNo, Tracking URL

            string TrackNo = string.Empty;
            string TrackUrl = string.Empty;
            var TrackingNo = dbContext.DirectShipments.Where(p => p.DirectShipmentId == DirectShipmentId).FirstOrDefault();
            if (TrackingNo != null)
            {
                if (TrackingNo.TrackingDetail != null)
                {
                    if (TrackingNo.TrackingDetail.Contains("Order"))
                    {
                        TrackNo = TrackingNo.TrackingDetail.Replace("Order_", "");
                        TrackUrl = AppSettings.TrackingUrl + "/tracking-detail/" + Shipment + "/" + TrackNo + "/";
                    }
                    else
                    {
                        if (TrackingNo.LogisticServiceType == "DHL" || TrackingNo.LogisticServiceType == "TNT" || TrackingNo.LogisticServiceType == "UPS")
                        {
                            TrackNo = TrackingNo.TrackingDetail;
                            TrackUrl = AppSettings.TrackingUrl + "/tracking-detail/" + Shipment + "/" + TrackNo + "/";
                        }
                        else
                        {
                            TrackNo = TrackingNo.TrackingDetail;
                            TrackUrl = AppSettings.TrackingUrl + "/tracking-detail/UKEUShipment/" + TrackingNo.TrackingDetail + "/";
                        }
                    }
                }
            }

            #endregion

            var userDetail = (from r in dbContext.Users
                              join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                              where r.UserId == shipmentDetail.CreatedBy
                              select new
                              {
                                  Email = r.Email,
                                  UserEmail = r.UserEmail,
                                  RoleId = ur.RoleId
                              }).FirstOrDefault();

            //Get Customer Name and Customer User Detail
            var customerDetail = (from u in dbContext.Users
                                  where u.UserId == (userDetail.RoleId == (int)FrayteUserRole.UserCustomer ? shipmentDetail.CreatedBy : shipmentDetail.CustomerId)
                                  select new
                                  {
                                      CustomerName = u.ContactName,
                                      CustomerEmail = u.UserEmail,
                                      CompanyName = u.CompanyName
                                  }).FirstOrDefault();

            //Get Frayte Direct Booking Customer Company
            var detail = (from u in dbContext.Users
                          where u.UserId == dbDetail.CustomerId
                          select new
                          {
                              CompanyName = u.CompanyName
                          }).FirstOrDefault();

            var staffdetail = GetAssociateStaffDetail(dbDetail.CustomerId, RoleId, LogInUserId);

            string Attachment = UtilityRepository.PackageLabelPath(DirectShipmentId, CourierName, logistic.RateType);

            var operationzone = UtilityRepository.GetOperationZone();
            var userInfo = (from r in dbContext.Users
                            join tz in dbContext.Timezones on r.TimezoneId equals tz.TimezoneId
                            where r.UserId == shipmentDetail.CreatedBy
                            select tz
                   ).FirstOrDefault();

            var UserTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userInfo.Name);
            var dbShipment = dbContext.DirectShipments.Find(DirectShipmentId);
            var CreatedOnDate = UtilityRepository.UtcDateToOtherTimezone(dbShipment.CreatedOn, dbShipment.CreatedOn.TimeOfDay, UserTimeZoneInfo).Item1;
            var CreatedOnTime = UtilityRepository.GetFormattedTimeFromString(UtilityRepository.UtcDateToOtherTimezone(dbShipment.CreatedOn, dbShipment.CreatedOn.TimeOfDay, UserTimeZoneInfo).Item2);

            DynamicViewBag viewBag = new DynamicViewBag();
            viewBag.AddValue("CreatedOn", CreatedOnDate.ToString("dd-MMM-yyyy") + " " + CreatedOnTime + " - " + userInfo.OffsetShort);
            viewBag.AddValue("TotalWeight", dbDetail.CustomerRateCard.Weight);
            viewBag.AddValue("FrayteNumber", dbDetail.FrayteNumber);
            viewBag.AddValue("PickUp", dbDetail.PickUpRef == null ? "" : dbDetail.PickUpRef);
            viewBag.AddValue("ShipmentMethod", ShipmentMethod);
            viewBag.AddValue("TrackingNo", TrackNo);
            viewBag.AddValue("TrackingURL", TrackUrl);
            viewBag.AddValue("TotalCartoon", dbDetail.Packages.Sum(p => p.CartoonValue));
            viewBag.AddValue("CustomerName", dbDetail.ShipTo.FirstName + " " + dbDetail.ShipTo.LastName);
            viewBag.AddValue("CustomerEmail", dbDetail.ShipTo.Email);
            viewBag.AddValue("CompanyName", dbDetail.ShipTo.FirstName + " " + dbDetail.ShipTo.LastName);
            viewBag.AddValue("ImageHeader", "FrayteLogo");
            viewBag.AddValue("TrackButton", "TrackShipment");
            viewBag.AddValue("OperationZoneId", dbDetail.OpearionZoneId);

            var userCustomerDetail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == shipmentDetail.CustomerId).FirstOrDefault();
            if (userCustomerDetail != null)
            {
                viewBag.AddValue("SiteAddress", userCustomerDetail.Website);
                viewBag.AddValue("UserName", userCustomerDetail.OperationStaff);
                viewBag.AddValue("UserPosition", userCustomerDetail.UserPosition);
                viewBag.AddValue("UserEmail", userCustomerDetail.OperationStaffEmail);
                viewBag.AddValue("UserPhone", userCustomerDetail.OperationStaffPhone);
                viewBag.AddValue("KindRegards", userCustomerDetail.KindRegards);
            }
            else
            {
                if (shipmentDetail.OpearionZoneId == 1)
                {
                    viewBag.AddValue("UserPhone", "(+852) 2148 4880");
                }
                else
                {
                    viewBag.AddValue("UserPhone", "(+44) 01792 678017");
                }

                viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                viewBag.AddValue("UserName", staffdetail.OperationStaffName);
                viewBag.AddValue("UserPosition", staffdetail.DeptName);
                viewBag.AddValue("UserEmail", staffdetail.OperationStaffEmail);
                viewBag.AddValue("KindRegards", "FRAYTE GLOBAL");
            }

            //Get UserType (Normal/Special)
            var UserType = UtilityRepository.GetUserType(dbDetail.CustomerId);
            string Status = string.Empty;

            string template = string.Empty;
            if (userDetail.RoleId == (int)FrayteUserRole.UserCustomer)
            {
                template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + shipmentDetail.CustomerId + "/" + "DirectBookingConfirmation.cshtml");
            }
            else
            {
                if (UserType != null)
                {
                    if (UserType == FrayteUserType.SPECIAL)
                    {
                        template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + shipmentDetail.CustomerId + "/" + "DirectBookingConfirmation.cshtml");
                    }
                    else if (UserType == FrayteUserType.NORMAL)
                    {
                        Status = "Confirmation";
                        template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/DirectBookingConfirmation.cshtml");
                    }
                }
                else
                {
                    Status = "Confirmation";
                    template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/DirectBookingConfirmation.cshtml");
                }
            }

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, dbDetail, viewBag, null);

            string EmailSubject = string.Empty;

            if (userCustomerDetail != null)
            {
                EmailSubject = "Shipment Booking Detail with Label - FR#" + dbDetail.FrayteNumber;
            }
            else
            {
                EmailSubject = "Shipment Booking Detail with Label - [" + detail.CompanyName + "] - FR#" + dbDetail.FrayteNumber;
            }

            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Label Email " + TrackingNo.FrayteNumber + " " + ToMail));
            //Send mail to Customer
            Send_FrayteEmail(ToMail, AppSettings.TOCC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment, Status, shipmentDetail.CustomerId);
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(EmailBody));
        }

        public void SendDirectBookingCancelMail(int DirectShipmentId)
        {
            var customerDetail = (from ds in dbContext.DirectShipments
                                  join u in dbContext.Users on ds.CustomerId equals u.UserId
                                  join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                  join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                  join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                  join da in dbContext.DirectShipmentAddresses on ds.ToAddressId equals da.DirectShipmentAddressId
                                  where ds.DirectShipmentId == DirectShipmentId
                                  select new
                                  {
                                      CustomerName = da.ContactFirstName + " " + da.ContactLastName,
                                      CustomerEmail = da.Email,
                                      CompanyName = da.CompanyName,
                                      ShipperName = u.ContactName,
                                      ShipperEmail = u.Email,
                                      ShipperCompany = u.CompanyName,
                                      OperationUser = u1.Email,
                                      OperationUserName = u1.UserName,
                                      OperationUserPosition = u1.Position,
                                      OperationUserPhoneNo = u1.TelephoneNo,
                                      FrayteNuber = ds.FrayteNumber
                                  }).FirstOrDefault();

            var shipmentDetail = new DirectShipmentRepository().GetDirectShipmentDraftDetail(DirectShipmentId, "ShipmentClone");

            var userDetail = (from r in dbContext.Users
                              join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                              where r.UserId == shipmentDetail.CreatedBy
                              select new
                              {
                                  Email = r.Email,
                                  UserEmail = r.UserEmail,
                                  RoleId = ur.RoleId
                              }).FirstOrDefault();

            DynamicViewBag viewBag = new DynamicViewBag();
            viewBag.AddValue("ImageHeader", "FrayteLogo");
            viewBag.AddValue("ShipmentOrderNo", customerDetail.FrayteNuber);
            viewBag.AddValue("ShipperName", customerDetail.ShipperName);

            var operationzone = UtilityRepository.GetOperationZone();

            var userCustomerDetail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == shipmentDetail.CustomerId).FirstOrDefault();
            if (userCustomerDetail != null)
            {
                viewBag.AddValue("SiteAddress", userCustomerDetail.Website);
                viewBag.AddValue("UserName", userCustomerDetail.OperationStaff);
                viewBag.AddValue("UserPosition", userCustomerDetail.UserPosition);
                viewBag.AddValue("UserEmail", userCustomerDetail.OperationStaffEmail);
                viewBag.AddValue("UserPhone", userCustomerDetail.OperationStaffPhone);
                viewBag.AddValue("KindRegards", userCustomerDetail.KindRegards);
            }
            else
            {
                if (shipmentDetail.OpearionZoneId == 1)
                {
                    viewBag.AddValue("UserPhone", "(+852) 2148 4880");
                }
                else
                {
                    viewBag.AddValue("UserPhone", "(+44) 01792 678017");
                }

                viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                viewBag.AddValue("UserName", customerDetail.OperationUserName);
                viewBag.AddValue("UserPosition", "Operation Staff");
                viewBag.AddValue("UserEmail", customerDetail.OperationUser);
                viewBag.AddValue("KindRegards", "FRAYTE GLOBAL");
            }
            string template = string.Empty;

            if (userDetail.RoleId == (int)FrayteUserRole.UserCustomer)
            {
                template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + shipmentDetail.CustomerId + "/" + "C1.cshtml");
            }
            else
            {
                template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/C1.cshtml");
            }

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, "", viewBag, null);

            string EmailSubject = string.Empty;
            if (userCustomerDetail != null)
            {
                EmailSubject = "Shipment Cancelled - " + userCustomerDetail.KindRegards + " - " + customerDetail.FrayteNuber;
            }
            else
            {
                string name = (customerDetail.CompanyName == "" || customerDetail.CompanyName == null) ? customerDetail.CustomerName : customerDetail.CompanyName;
                EmailSubject = "Shipment Cancelled - [" + name + "] - FR#" + customerDetail.FrayteNuber;
            }

            var To = customerDetail.ShipperEmail;
            var CC = "";

            if (To != null && To != "")
            {
                //Send mail to Customer
                SendMail_Frayte(To, CC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, shipmentDetail.CustomerId);
            }

            if (!string.IsNullOrEmpty(customerDetail.CustomerEmail))
            {
                viewBag = new DynamicViewBag();
                viewBag.AddValue("ImageHeader", "FrayteLogo");
                viewBag.AddValue("ShipmentOrderNo", customerDetail.FrayteNuber);
                viewBag.AddValue("CustomerName", customerDetail.CustomerName);
                if (userCustomerDetail != null)
                {
                    viewBag.AddValue("SiteAddress", userCustomerDetail.Website);
                    viewBag.AddValue("UserName", userCustomerDetail.OperationStaff);
                    viewBag.AddValue("UserPosition", userCustomerDetail.UserPosition);
                    viewBag.AddValue("UserEmail", userCustomerDetail.OperationStaffEmail);
                    viewBag.AddValue("UserPhone", userCustomerDetail.OperationStaffPhone);
                    viewBag.AddValue("KindRegards", userCustomerDetail.KindRegards);
                }
                else
                {
                    if (shipmentDetail.OpearionZoneId == 1)
                    {
                        viewBag.AddValue("UserPhone", "(+852) 2148 4880");
                    }
                    else
                    {
                        viewBag.AddValue("UserPhone", "(+44) 01792 678017");
                    }

                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                    viewBag.AddValue("UserName", customerDetail.OperationUserName);
                    viewBag.AddValue("UserPosition", "Operation Staff");
                    viewBag.AddValue("UserEmail", customerDetail.OperationUser);
                    viewBag.AddValue("KindRegards", "FRAYTE GLOBAL");
                }

                if (userDetail.RoleId == (int)FrayteUserRole.UserCustomer)
                {
                    template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + shipmentDetail.CustomerId + "/" + "C2.cshtml");
                }
                else
                {
                    template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/C2.cshtml");
                }

                templateService = new TemplateService();
                EmailBody = templateService.Parse(template, "", viewBag, null);

                if (userCustomerDetail != null)
                {
                    EmailSubject = "Shipment Cancelled - " + userCustomerDetail.KindRegards + " - " + customerDetail.FrayteNuber;
                }
                else
                {
                    string name = (customerDetail.CompanyName == "" || customerDetail.CompanyName == null) ? customerDetail.CustomerName : customerDetail.CompanyName;
                    EmailSubject = "Shipment Cancelled - [" + name + "] - FR#" + customerDetail.FrayteNuber;
                }

                To = customerDetail.CustomerEmail;
                CC = "";

                if (To != null && To != "")
                {
                    //Send mail to Customer
                    SendMail_Frayte(To, CC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, shipmentDetail.CustomerId);
                }
            }

            viewBag = new DynamicViewBag();
            viewBag.AddValue("ImageHeader", "FrayteLogo");
            viewBag.AddValue("ShipmentOrderNo", customerDetail.FrayteNuber);
            viewBag.AddValue("ConfirmButton", "Confirm");
            viewBag.AddValue("RejectButton", "Reject");
            viewBag.AddValue("ConfirmAction", AppSettings.TrackingUrl + "/directBooking/c/" + DirectShipmentId.ToString());
            viewBag.AddValue("RejectAction", AppSettings.TrackingUrl + "/directBooking/r/" + DirectShipmentId.ToString());
            viewBag.AddValue("UserName", customerDetail.OperationUserName);
            viewBag.AddValue("UserPosition", "Operation Staff");
            viewBag.AddValue("UserEmail", customerDetail.OperationUser);
            viewBag.AddValue("UserPhone", customerDetail.OperationUserPhoneNo);

            template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/C3.cshtml");

            templateService = new TemplateService();
            EmailBody = templateService.Parse(template, "", viewBag, null);

            EmailSubject = "Shipment Cancelled - " + customerDetail.CustomerName + " FR#" + customerDetail.FrayteNuber;
            To = customerDetail.OperationUser;
            string Status = "Cancel";

            //Send mail to Customer
            SendMail_New(To, "", "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, "", Status);
        }

        public void Send_FrayteEmail(string toEmail, string ccEmail, string DisplayName, string EmailSubject, string EmailBody, string AttachmentFilePath, string Status, int customerId)
        {
            var detail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == customerId).FirstOrDefault();

            string logoImage = detail == null ? AppSettings.EmailServicePath + "/Images/FrayteLogo.png" : AppSettings.EmailServicePath + "/EmailTeamplate/" + customerId + "/Images/" + detail.LogoFileName;
            string trackImage = AppSettings.EmailServicePath + "/Images/TrackShipment.png";
            string amdentImage = AppSettings.EmailServicePath + "/Images/Amend.png";
            string confirmImage = AppSettings.EmailServicePath + "/Images/Confirm.png";
            string rejectImage = AppSettings.EmailServicePath + "/Images/Reject.png";

            List<string> ImagePath = new List<string>();
            if (Status == "Confirmation")
            {
                ImagePath.Add(logoImage);
                ImagePath.Add(trackImage);
            }
            else if (Status == "Cancel")
            {
                ImagePath.Add(logoImage);
                ImagePath.Add(rejectImage);
            }
            else if (Status == "Amend")
            {
                ImagePath.Add(logoImage);
                ImagePath.Add(amdentImage);
            }
            else if (Status == "Tracking")
            {
                ImagePath.Add(logoImage);
                ImagePath.Add(trackImage);
            }
            //For Special Customer
            else
            {
                ImagePath.Add(logoImage);
            }

            FrayteEmail.SendFrayteEmail(toEmail, ccEmail, DisplayName, EmailSubject, EmailBody, AttachmentFilePath, ImagePath, Status, customerId);
        }

        public void SendMail_Frayte(string toEmail, string ccEmail, string DisplayName, string EmailSubject, string EmailBody, int customerId)
        {
            var detail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == customerId).FirstOrDefault();

            string logoImage = detail == null ? AppSettings.EmailServicePath + "/Images/FrayteLogo.png" : AppSettings.EmailServicePath + "/EmailTeamplate/" + customerId + "/Images/" + detail.LogoFileName;

            if (!string.IsNullOrEmpty(AppSettings.TOCC))
            {
                FrayteEmail.SendMail(AppSettings.TOCC, "", "", "", DisplayName, EmailSubject, EmailBody, "", logoImage);
            }
            else
            {
                FrayteEmail.SendMail(toEmail, ccEmail, "", "", DisplayName, EmailSubject, EmailBody, "", logoImage);
            }
        }

        public async Task<bool> SendDirectBookingConfirmationMail(DirectBookingShipmentDetail shipmentDetail, int abc)
        {
            bool flag = false;
            try
            {
                //Get Customer Name and Customer User Detail
                var result = await Task.Run<bool>(() =>
                {
                    var customerDetail = (from u in dbContext.Users
                                          join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                          join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                          join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                          where u.UserId == shipmentDetail.CustomerId
                                          select new
                                          {
                                              CustomerName = u.ContactName,
                                              CustomerEmail = u.Email,
                                              CompanyName = u.CompanyName,
                                              UserName = u1.ContactName,
                                              UserPosition = u1.Position,
                                              UserEmail = u1.Email,
                                              UserPhone = u1.TelephoneNo,
                                              UserSkype = u1.Skype,
                                              UserFax = u1.FaxNumber
                                          }).FirstOrDefault();

                    decimal TotalWeight = shipmentDetail.Packages.Sum(p => p.Weight);
                    string ShipmentMethod = "";

                    #region TrackingNo, TrackingURL

                    string TrackNo = string.Empty;
                    string TrackUrl = string.Empty;
                    var TrackingNo = dbContext.DirectShipments.Where(p => p.DirectShipmentId == shipmentDetail.DirectShipmentId).FirstOrDefault();
                    if (TrackingNo != null)
                    {
                        if (TrackingNo.TrackingDetail != null)
                        {
                            if (TrackingNo.TrackingDetail.Contains("order"))
                            {
                                var dslist = dbContext.DirectShipmentDetails.Where(p => p.DirectShipmentId == TrackingNo.DirectShipmentId).ToList();
                                if (dslist != null)
                                {
                                    foreach (var ds in dslist)
                                    {
                                        var ETrack = dbContext.ShipmentEasyPosts.Where(p => p.ShipmentId == ds.DirectShipmentDetailId).Select(p => p.TrackingCode).FirstOrDefault();
                                        if (ETrack != null)
                                        {
                                            TrackNo = ETrack;
                                            TrackUrl = AppSettings.HostName + "/index.html#/home/tracking-hub/" + ShipmentMethod + "/" + ETrack;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                TrackNo = TrackingNo.TrackingDetail;
                                TrackUrl = AppSettings.HostName + "/index.html#/home/tracking-hub/UKEUShipment/" + TrackingNo.TrackingDetail;
                            }
                        }
                    }

                    #endregion

                    DynamicViewBag viewBag = new DynamicViewBag();
                    viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                    viewBag.AddValue("TotalWeight", TotalWeight);
                    viewBag.AddValue("ShipmentMethod", ShipmentMethod);
                    viewBag.AddValue("TrackingNo", TrackNo);
                    viewBag.AddValue("TrackingURL", TrackUrl);
                    viewBag.AddValue("CustomerName", customerDetail.CustomerName + " (Consignee)");
                    viewBag.AddValue("CustomerEmail", customerDetail.CustomerEmail);
                    viewBag.AddValue("CompanyName", customerDetail.CompanyName);
                    viewBag.AddValue("UserName", customerDetail.UserName);
                    viewBag.AddValue("UserPosition", customerDetail.UserPosition);
                    viewBag.AddValue("UserEmail", customerDetail.UserEmail);
                    viewBag.AddValue("UserPhone", customerDetail.UserPhone);
                    viewBag.AddValue("UserSkype", customerDetail.UserSkype);
                    viewBag.AddValue("UserFax", customerDetail.UserFax);
                    viewBag.AddValue("ImageHeader", "FrayteLogo");
                    viewBag.AddValue("TrackButton", "TrackShipment");

                    string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/DirectBookingConfirmation.cshtml");

                    var templateService = new TemplateService();
                    var EmailBody = templateService.Parse(template, shipmentDetail, viewBag, null);
                    var EmailSubject = "Booking Confirmation - " + shipmentDetail.DirectShipmentId + " - FRAYTE GLOBAL";
                    var To = customerDetail.CustomerEmail;
                    var CC = customerDetail.UserEmail;
                    string Status = "Confirmation";

                    #region Attach Labels

                    string Attachment = UtilityRepository.PackageLabelPath(shipmentDetail.DirectShipmentId, shipmentDetail.CustomerRateCard.CourierAccountCountryCode, shipmentDetail.CustomerRateCard.RateType);

                    #endregion

                    //Send mail to Customer
                    SendMail_New(To, CC, "", EmailSubject, EmailBody, Attachment, Status);

                    flag = true;
                    if (shipmentDetail.ShipFrom.IsMailSend)
                    {
                        //Send mail to Ship From
                        if (!string.IsNullOrEmpty(shipmentDetail.ShipFrom.Email))
                        {
                            viewBag = new DynamicViewBag();
                            viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                            viewBag.AddValue("TotalWeight", TotalWeight);
                            viewBag.AddValue("ShipmentMethod", ShipmentMethod);
                            viewBag.AddValue("TrackingNo", TrackNo);
                            viewBag.AddValue("TrackingURL", TrackUrl);
                            viewBag.AddValue("CustomerName", shipmentDetail.ShipFrom.FirstName + " " + shipmentDetail.ShipFrom.LastName + " (Shipper)");
                            viewBag.AddValue("CustomerEmail", shipmentDetail.ShipFrom.Email);
                            viewBag.AddValue("CompanyName", shipmentDetail.ShipFrom.CompanyName);
                            viewBag.AddValue("UserName", customerDetail.UserName);
                            viewBag.AddValue("UserPosition", customerDetail.UserPosition);
                            viewBag.AddValue("UserEmail", customerDetail.UserEmail);
                            viewBag.AddValue("UserPhone", customerDetail.UserPhone);
                            viewBag.AddValue("UserSkype", customerDetail.UserSkype);
                            viewBag.AddValue("UserFax", customerDetail.UserFax);
                            viewBag.AddValue("ImageHeader", "FrayteLogo");

                            template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/DirectBookingConfirmation.cshtml");

                            templateService = new TemplateService();
                            EmailBody = templateService.Parse(template, shipmentDetail, viewBag, null);
                            EmailSubject = "Booking Confirmation - " + shipmentDetail.DirectShipmentId + " - FRAYTE GLOBAL";
                            To = shipmentDetail.ShipFrom.Email;
                            CC = "";

                            #region Attach Labels

                            string Attachment1 = UtilityRepository.PackageLabelPath(shipmentDetail.DirectShipmentId, shipmentDetail.CustomerRateCard.CourierAccountCountryCode, shipmentDetail.CustomerRateCard.RateType);

                            #endregion

                            SendMail_New(To, CC, "", EmailSubject, EmailBody, Attachment1, Status);
                            flag = true;
                        }
                    }

                    if (shipmentDetail.ShipTo.IsMailSend)
                    {
                        //Send mail to Ship To
                        if (!string.IsNullOrEmpty(shipmentDetail.ShipTo.Email))
                        {
                            viewBag = new DynamicViewBag();
                            viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                            viewBag.AddValue("TotalWeight", TotalWeight);
                            viewBag.AddValue("ShipmentMethod", ShipmentMethod);
                            viewBag.AddValue("TrackingNo", TrackNo);
                            viewBag.AddValue("TrackingURL", TrackUrl);
                            viewBag.AddValue("CustomerName", shipmentDetail.ShipTo.FirstName + " " + shipmentDetail.ShipTo.LastName + " (Customer)");
                            viewBag.AddValue("CustomerEmail", shipmentDetail.ShipTo.Email);
                            viewBag.AddValue("CompanyName", shipmentDetail.ShipTo.CompanyName);
                            viewBag.AddValue("UserName", customerDetail.UserName);
                            viewBag.AddValue("UserPosition", customerDetail.UserPosition);
                            viewBag.AddValue("UserEmail", customerDetail.UserEmail);
                            viewBag.AddValue("UserPhone", customerDetail.UserPhone);
                            viewBag.AddValue("UserSkype", customerDetail.UserSkype);
                            viewBag.AddValue("UserFax", customerDetail.UserFax);
                            viewBag.AddValue("ImageHeader", "FrayteLogo");

                            template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/DirectBookingConfirmation.cshtml");

                            templateService = new TemplateService();
                            EmailBody = templateService.Parse(template, shipmentDetail, viewBag, null);
                            EmailSubject = "Booking Confirmation - " + shipmentDetail.DirectShipmentId + " - FRAYTE GLOBAL";
                            To = shipmentDetail.ShipTo.Email;
                            CC = "";

                            #region Attach Labels

                            string Attachment2 = UtilityRepository.PackageLabelPath(shipmentDetail.DirectShipmentId, shipmentDetail.CustomerRateCard.CourierAccountCountryCode, shipmentDetail.CustomerRateCard.RateType);

                            #endregion

                            SendMail_New(To, CC, "", EmailSubject, EmailBody, Attachment2, Status);
                            flag = true;
                        }
                    }
                    return flag;
                });
                return result;
            }
            catch (Exception e)
            {
                flag = false;
                return flag;
            }
        }

        public void SendDirectBookingDeliveryMail(int DirectShipmentId)
        {
            //Get Customer and Customer Detail
            var customerDetail = (from ds in dbContext.DirectShipments
                                  join sa in dbContext.DirectShipmentAddresses on ds.FromAddressId equals sa.DirectShipmentAddressId
                                  join cc in dbContext.Countries on sa.CountryId equals cc.CountryId
                                  join sa1 in dbContext.DirectShipmentAddresses on ds.ToAddressId equals sa1.DirectShipmentAddressId
                                  join cc1 in dbContext.Countries on sa1.CountryId equals cc1.CountryId
                                  join uu in dbContext.Users on ds.CustomerId equals uu.UserId
                                  join ua in dbContext.UserAddresses on uu.UserId equals ua.UserId
                                  join cs in dbContext.CustomerSettings on uu.UserId equals cs.UserId
                                  where ds.DirectShipmentId == DirectShipmentId
                                  select new
                                  {
                                      CustomerName = uu.ContactName,
                                      CompanyName = uu.CompanyName,
                                      OperationZoneId = ds.OpearionZoneId,
                                      OrderNo = ds.DirectShipmentId,
                                      CreatedOn = ds.CreatedOn,
                                      FrayteNumber = ds.FrayteNumber,
                                      ShipperName = sa.ContactFirstName + " " + sa.ContactLastName,
                                      ShipFrom1 = sa.Address1,
                                      ShipFrom2 = sa.Address2,
                                      ShipCity = sa.City,
                                      ShipState = sa.State,
                                      ShipCountry = cc.CountryName,
                                      ShipPostCode = sa.Zip,
                                      ConsigneeName = sa1.ContactFirstName + " " + sa1.ContactLastName,
                                      ConsigneeFrom1 = sa1.Address1,
                                      ConsigneeFrom2 = sa1.Address2,
                                      ConsigneeCity = sa1.City,
                                      ConsigneeState = sa1.State,
                                      ConsigneeCountry = cc1.CountryName,
                                      ConsigneePostCode = sa1.Zip,
                                      UserName = uu.ContactName,
                                      UserPosition = uu.Position,
                                      UserEmail = uu.Email,
                                      UserPhone = uu.MobileNo,
                                      UserSkype = uu.Skype,
                                      UserFax = uu.FaxNumber,
                                      ScheduleType = cs.ScheduleSetting,
                                      ScheduleSettingType = cs.ScheduleSettingType
                                  }).ToList();

            int PackageCount = dbContext.DirectShipmentDetails.Where(p => p.DirectShipmentId == DirectShipmentId).Sum(p => p.CartoonValue);
            decimal TotalWeight = 0.0m;
            var weight = dbContext.DirectShipmentDetails.Where(p => p.DirectShipmentId == DirectShipmentId).ToList();
            foreach (var obj in weight)
            {
                TotalWeight += obj.Weight * obj.CartoonValue;
            }

            var Detail = customerDetail.Where(p => p.ScheduleType == FrayteReportSettingDays.PerShipment && p.ScheduleSettingType == FrayteCustomerMailSetting.POD).FirstOrDefault();

            DynamicViewBag viewBag = new DynamicViewBag();
            if (Detail != null)
            {
                viewBag.AddValue("CustomerName", Detail.CustomerName);
                viewBag.AddValue("CreatedOn", Detail.CreatedOn.ToString("dd-MMM-yyyy"));
                viewBag.AddValue("OrderNo", Detail.FrayteNumber);
                viewBag.AddValue("ShipperName", Detail.ShipperName);
                viewBag.AddValue("ConsigneeName", Detail.ConsigneeName);
                viewBag.AddValue("ShipFrom1", Detail.ShipFrom1);
                viewBag.AddValue("ShipFrom2", Detail.ShipFrom2);
                viewBag.AddValue("ShipCity", Detail.ShipCity);
                viewBag.AddValue("ShipState", Detail.ShipState);
                viewBag.AddValue("ShipCountry", Detail.ShipCountry);
                viewBag.AddValue("ShipPostCode", Detail.ShipPostCode);
                viewBag.AddValue("ConsigneeFrom1", Detail.ConsigneeFrom1);
                viewBag.AddValue("ConsigneeFrom2", Detail.ConsigneeFrom2);
                viewBag.AddValue("ConsigneeCity", Detail.ConsigneeCity);
                viewBag.AddValue("ConsigneeState", Detail.ConsigneeState);
                viewBag.AddValue("ConsigneeCountry", Detail.ConsigneeCountry);
                viewBag.AddValue("ConsigneePostCode", Detail.ConsigneePostCode);
                viewBag.AddValue("TotalWeight", TotalWeight);
                viewBag.AddValue("PackageCount", PackageCount);
                viewBag.AddValue("UserName", Detail.UserName);
                viewBag.AddValue("UserPosition", Detail.UserPosition);
                viewBag.AddValue("UserEmail", Detail.UserEmail);
                viewBag.AddValue("UserPhone", Detail.UserPhone);
                viewBag.AddValue("UserSkype", Detail.UserSkype);
                viewBag.AddValue("UserFax", Detail.UserFax);
                viewBag.AddValue("ImageHeader", "FrayteLogo");
                if (Detail.OperationZoneId == 1)
                {
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                }
                else
                {
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                }

                var operationzone = UtilityRepository.GetOperationZone();
                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/DirectBookingDelivered.cshtml");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, Detail, viewBag, null);
                var EmailSubject = "Shipment Delivered - [" + Detail.CompanyName + "] - FR#" + Detail.FrayteNumber;
                var To = Detail.UserEmail;
                var CC = "";

                //Send mail to Customer
                SendMail(To, CC, "", "", "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody);
            }
        }

        public void SendApiErrorMail(DirectBookingShipmentDraftDetail directBookingDetail, List<FrayteApiError> ErrorCode)
        {
            try
            {
                FrayteDirectBookingDetailError directBookingError = new FrayteDirectBookingDetailError();
                if (directBookingDetail != null)
                {
                    decimal TotalWeight = directBookingDetail.Packages.Sum(p => p.Weight);

                    directBookingError.ErrorCode = ErrorCode;
                    directBookingError.BookingDetail = directBookingDetail;
                    directBookingError.GrossWeight = TotalWeight;
                    var logistic = (from lsca in dbContext.LogisticServiceCourierAccounts
                                    join ls in dbContext.LogisticServices on lsca.LogisticServiceId equals ls.LogisticServiceId
                                    where lsca.LogisticServiceId == directBookingDetail.CustomerRateCard.LogisticServiceId
                                    select ls).FirstOrDefault();

                    if (logistic != null)
                    {
                        if (logistic.RateType != null && logistic.RateType != "")
                        {
                            directBookingError.ShipmentType = logistic.LogisticCompany + " (" + logistic.RateType + ")";
                        }
                        else
                        {
                            directBookingError.ShipmentType = logistic.LogisticCompany;
                        }
                    }
                }
                else
                {
                    directBookingError.ErrorCode = ErrorCode;
                    directBookingError.BookingDetail = null;
                }

                DynamicViewBag viewBag = new DynamicViewBag();
                viewBag.AddValue("ImageHeader", "FrayteLogo");

                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/ShipmentApiError.cshtml");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, directBookingError, viewBag, null);
                var EmailSubject = "Frayte API Integration Shipment - " + (directBookingDetail != null ? directBookingDetail.DirectShipmentDraftId : 0) + " - Error Detail";
                var To = AppSettings.ParcelHubEmail;
                //Send mail to Customer
                SendMail(To, "", EmailSubject, EmailBody, "");
            }
            catch (Exception ex)
            {

            }
        }

        public void SendShipmentErrorMail(DirectBookingShipmentDraftDetail directBookingDetail, FratyteError frayetError)
        {
            //if(directBookingDetail.CustomerRateCard  )
            decimal TotalWeight = directBookingDetail.Packages.Sum(p => p.Weight);

            FrayteDirectBookingDetailError directBookingError = new FrayteDirectBookingDetailError();
            directBookingError.Error = frayetError;
            directBookingError.BookingDetail = directBookingDetail;
            directBookingError.GrossWeight = TotalWeight;
            var logistic = (from lsca in dbContext.LogisticServiceCourierAccounts
                            join ls in dbContext.LogisticServices on lsca.LogisticServiceId equals ls.LogisticServiceId
                            where lsca.LogisticServiceId == directBookingDetail.CustomerRateCard.LogisticServiceId
                            select ls).FirstOrDefault();

            if (logistic != null)
            {
                if (logistic.RateType != null && logistic.RateType != "")
                {
                    directBookingError.ShipmentType = logistic.LogisticCompany + " (" + logistic.RateType + ")";
                }
                else
                {
                    directBookingError.ShipmentType = logistic.LogisticCompany;
                }
            }

            dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(directBookingError);
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));

            DynamicViewBag viewBag = new DynamicViewBag();
            viewBag.AddValue("ImageHeader", "FrayteLogo");

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/ShipmentError.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, directBookingError, viewBag, null);
            var EmailSubject = "Direct Booking Shipment Draft - " + directBookingDetail.DirectShipmentDraftId + " - Error Detail";
            var To = string.Empty;
            if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UKMail || directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Yodel || directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Hermes)
            {
                To = AppSettings.ParcelHubEmail;
            }
            else
            {
                To = AppSettings.EasyPostEmail;
            }
            var CC = "";

            //Send mail to Customer
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

      
        public void SendShipmentErrorMail(string directBookingDetail)
        {
            DynamicViewBag vs = new DynamicViewBag();
            vs.AddValue("Error", directBookingDetail);

            string template = File.ReadAllText("D:\\ProjectFrayte\\FrayteEmailService\\EmailTeamplate\\EcommShipmentError.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, null, vs, null);
            var EmailSubject = "EcommShipment Error details";
            var To = "vikshit9292@gmail.com";

            var CC = "";

            //Send mail to Customer
            SendMail(To, CC, EmailSubject, EmailBody, "");
        }

        public void SendDirectBookingRejectMail(string CustomerAction, int DirectShipmentId)
        {
            var customerDetail = (from ds in dbContext.DirectShipments
                                  join da in dbContext.DirectShipmentAddresses on ds.ToAddressId equals da.DirectShipmentAddressId
                                  where ds.DirectShipmentId == DirectShipmentId
                                  select new
                                  {
                                      CustomerName = da.ContactFirstName + " " + da.ContactLastName,
                                      CompanyName = da.CompanyName,
                                      CustomerEmail = da.Email,
                                      FrayteNuber = ds.FrayteNumber
                                  }).FirstOrDefault();

            DynamicViewBag viewBag = new DynamicViewBag();
            viewBag.AddValue("ShipmentOrderNo", customerDetail.FrayteNuber);
            viewBag.AddValue("ImageHeader", "FrayteLogo");
            var operationzone = UtilityRepository.GetOperationZone();

            viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);

            if (CustomerAction == ShipmentCustomerAction.Confirm)
            {
                //Update ShipmentStatus
                new ShipmentRepository().UpdateDirectShipmentStatus(DirectShipmentId, (int)FrayteShipmentStatus.Cancel);

                var template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/C4.cshtml");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, "", viewBag, null);
                var EmailSubject = "Shipment Cancelled Confirmation - [" + customerDetail.CompanyName == "" ? customerDetail.CustomerName : customerDetail.CompanyName + "] - FR#" + customerDetail.FrayteNuber;
                var To = customerDetail.CustomerEmail;
                var CC = "";

                if (To != null && To != "")
                {
                    //Send mail to Customer
                    SendMail(To, CC, "", "", "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody);
                }
            }
            else if (CustomerAction == ShipmentCustomerAction.Reject)
            {
                var template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/C5.cshtml");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, "", viewBag, null);
                var EmailSubject = "Shipment Rejection - [" + customerDetail.CompanyName == "" ? customerDetail.CustomerName : customerDetail.CompanyName + "] - FR#" + customerDetail.FrayteNuber;
                var To = customerDetail.CustomerEmail;
                var CC = "";

                if (To != null && To != "")
                {
                    //Send mail to Customer
                    SendMail(To, CC, "", "", "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody);
                }
            }
        }

        public void SendEmailToAdmin(FrayteCustomer frayteUser)
        {
            var admin = dbContext.Users.Find(1);
            var result = new User();
            if (frayteUser.OperationUser.UserId > 0)
            {
                result = dbContext.Users.Where(a => a.UserId == frayteUser.CreatedBy).FirstOrDefault();
            }

            NewCustomerAdminMail model = new NewCustomerAdminMail();
            model.OperationStaffId = frayteUser.OperationUser.UserId;
            model.CustomerId = frayteUser.UserId;
            model.UserName = result.UserName;
            model.UserEmail = result.Email;
            model.UserPhone = result.TelephoneNo;
            model.HostUrl = AppSettings.HostName;

            DynamicViewBag viewBag = new DynamicViewBag();
            viewBag.AddValue("ImageHeader", "FrayteLogo");
            viewBag.AddValue("ConfirmButton", "Confirm");
            viewBag.AddValue("AmendButton", "Amend");
            viewBag.AddValue("RejectButton", "Reject");

            var operationzone = UtilityRepository.GetOperationZone();
            var template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/newCustomer.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, model, viewBag, null);
            var EmailSubject = "New Customer Created - Action Required";
            var To = admin.Email;
            var CC = "";

            if (To != null && To != "")
            {
                //Send mail to Customer
                SendMail_New(To, CC, "FRAYTE - System (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, "", "Amend");
            }
        }

        public void SendAmmendMailToStaff(FrayteCustomer newCustomer, List<string> _amend, int StaffRoleId, int CreatedBy)
        {
            string ToMail = string.Empty;
            var info = (from u in dbContext.Users
                        join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                        join uu in dbContext.Users on ua.OperationUserId equals uu.UserId
                        join lm in dbContext.UserAdditionals on u.UserId equals lm.UserId into leftmmjoin
                        from mm in leftmmjoin.DefaultIfEmpty()
                        join ll in dbContext.Users on mm.ManagerUserId equals ll.UserId into leftlljoin
                        from nn in leftlljoin.DefaultIfEmpty()
                        where u.UserId == newCustomer.UserId
                        select new
                        {
                            OperationStaff = uu.ContactName,
                            OperationStaffEmail = uu.Email,
                            LineManager = nn.ContactName,
                            LineManagerEmail = nn.Email,
                            OperationZoneId = u.OperationZoneId
                        }).FirstOrDefault();

            var staff = (from r in dbContext.UserRoles
                         join u in dbContext.Users on r.UserId equals u.UserId
                         where r.RoleId == StaffRoleId
                         select new
                         {
                             ContactName = u.ContactName,
                             Email = u.Email
                         }).FirstOrDefault();

            if (info != null)
            {
                AdminActionAmendMailModel model = new AdminActionAmendMailModel();
                model.ToName = (info.LineManager == "" || info.LineManager == null) ? info.OperationStaff : info.LineManager;
                if (StaffRoleId == 1)
                {
                    model.UserName = staff.ContactName;
                }
                else if (StaffRoleId == 6)
                {
                    model.UserName = (info.LineManager == "" || info.LineManager == null) ? info.OperationStaff : info.LineManager;
                }
                model.UpdateDetail = _amend;
                model.StaffName = (info.LineManager == "" || info.LineManager == null) ? info.OperationStaff : info.LineManager;

                ToMail = (info.LineManagerEmail == "" || info.LineManagerEmail == null) ? info.OperationStaffEmail : info.LineManagerEmail;

                if (info.OperationZoneId == 1)
                {
                    model.SiteAddress = AppSettings.TrackingUrl;
                    model.UserPhone = "(+852) 2148 4880";
                }
                else if (info.OperationZoneId == 2)
                {
                    model.SiteAddress = AppSettings.TrackingUrl;
                    model.UserPhone = "(+44) 01792 678017";
                }

                model.StaffEmail = (info.LineManagerEmail == "" || info.LineManagerEmail == null) ? info.OperationStaffEmail : info.LineManagerEmail;

                DynamicViewBag viewBag = new DynamicViewBag();
                viewBag.AddValue("ImageHeader", "FrayteLogo");

                var template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/adminCustomerAmend.cshtml");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, model, viewBag, null);

                var EmailSubject = "Alert - Customer Amended";

                if (ToMail != null && ToMail != "")
                {

                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("customer amendmend email to staff")));
                    SendMail(ToMail, "", EmailSubject, EmailBody, "");
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception(EmailBody)));
                }
            }
        }

        public void SendUserCustomerAmmendMail(FrayteCustomer newCustomer, List<string> _amend, int StaffRoleId, int CreatedBy)
        {
            var info = (from ccd in dbContext.CustomerCompanyDetails
                        join u in dbContext.Users on ccd.UserId equals u.UserId
                        where u.UserId == CreatedBy
                        select new
                        {
                            OperationStaff = ccd.OperationStaff,
                            OperationStaffEmail = ccd.OperationStaffEmail,
                            ContactName = u.ContactName,
                            UserPosition = ccd.UserPosition,
                            SiteAddress = ccd.SiteAddress,
                            OperationStaffPhone = ccd.OperationStaffPhone,
                            LogoImage = ccd.LogoFileName
                        }).FirstOrDefault();

            if (info != null)
            {
                AdminActionAmendMailModel model = new AdminActionAmendMailModel();
                model.ToName = info.OperationStaff;
                model.UserName = info.ContactName;
                model.UpdateDetail = _amend;
                model.StaffName = info.UserPosition;
                model.SiteAddress = info.SiteAddress;
                model.UserPhone = info.OperationStaffPhone;
                model.StaffEmail = info.OperationStaffEmail;

                string logoImage = AppSettings.EmailServicePath + "/EmailTeamplate/" + CreatedBy + "/Images/" + info.LogoImage;

                DynamicViewBag viewBag = new DynamicViewBag();
                viewBag.AddValue("ImageHeader", "FrayteLogo");

                var template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + CreatedBy + "/adminCustomerAmend.cshtml");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, model, viewBag, null);

                var EmailSubject = "Alert - Customer Amended";

                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("user customer ammendmend email")));
                FrayteEmail.SendMail(model.StaffEmail, "", EmailSubject, EmailBody, "", logoImage, CreatedBy);
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception(EmailBody)));
            }
        }

        public void SendEmailOperationStaff(FrayteAdminAction actionDetail)
        {
            var operation = dbContext.Users.Find(actionDetail.OperationUserId);
            var To = operation.Email;
            var admin = dbContext.Users.Find(1);
            string CustomerName = dbContext.Users.Find(actionDetail.CustomerId).ContactName;

            AdminActionMailModel model = new AdminActionMailModel();
            model.UserName = admin.ContactName;
            model.UserEmail = admin.Email;
            model.UserPhone = admin.TelephoneNo;
            model.ToName = operation.ContactName;
            model.HostUrl = AppSettings.HostName;

            DynamicViewBag viewBag = new DynamicViewBag();
            viewBag.AddValue("ImageHeader", "FrayteLogo");
            viewBag.AddValue("CustomerName", CustomerName);

            if (actionDetail.ActionType == "c")
            {
                var template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/adminCustomerConfirm.cshtml");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, model, viewBag, null);

                var EmailSubject = "Alert - Customer Confirmed";
                var CC = "";

                if (To != null && To != "")
                {
                    //Send mail to Customer
                    SendMail(To, CC, EmailSubject, EmailBody, "");
                }
            }
            if (actionDetail.ActionType == "r")
            {
                var template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/adminCustomerReject.cshtml");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, model, viewBag, null);
                var EmailSubject = "Alert - Customer Rejected";
                var CC = "";

                if (To != null && To != "")
                {
                    //Send mail to Customer
                    SendMail(To, CC, EmailSubject, EmailBody, "");
                }
            }
        }

        public FrayteSalesRepresentiveEmail GetAssociateStaffDetail(int CustomerId, int RoleId, int LogInUserId)
        {
            FrayteSalesRepresentiveEmail detail = new FrayteSalesRepresentiveEmail();

            if (RoleId == 1 || RoleId == 3 || RoleId == 20)
            {
                detail = (from US1 in dbContext.Users
                          join US2 in dbContext.UserAdditionals on US1.UserId equals US2.UserId
                          join US3 in dbContext.Users on US2.OperationUserId equals US3.UserId
                          where US1.UserId == CustomerId
                          select new FrayteSalesRepresentiveEmail
                          {
                              OperationStaffName = US3.ContactName,
                              OperationStaffEmail = US3.Email,
                              DeptName = "Operation Staff"
                          }).FirstOrDefault();
            }
            else if (RoleId == 6)
            {
                detail = (from US1 in dbContext.Users
                          join US2 in dbContext.UserAdditionals on US1.UserId equals US2.UserId
                          join US3 in dbContext.UserRoles on US2.UserId equals US3.UserId
                          where US1.UserId == LogInUserId &&
                                US3.RoleId == RoleId
                          select new FrayteSalesRepresentiveEmail
                          {
                              OperationStaffName = US1.ContactName,
                              OperationStaffEmail = US1.Email,
                              DeptName = "Operation Staff"
                          }).FirstOrDefault();
            }

            return detail;
        }

        public void SendDirectShipmentDeleteInformation(int DirectShipmentId, string FrayteNumber)
        {
            try
            {
                string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";

                #region Attach Labels 

                string Attachment = UtilityRepository.PackageLabelPath(DirectShipmentId, "", "");

                #endregion

                DynamicViewBag viewBag = new DynamicViewBag();
                viewBag.AddValue("FrayteNumber", FrayteNumber);
                viewBag.AddValue("ImageHeader", "FrayteLogo");

                var template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/DirectBookingDelete.cshtml");
                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, "", viewBag, null);

                if (!string.IsNullOrEmpty(AppSettings.TOCC))
                {
                    FrayteEmail.SendMail(AppSettings.TOCC, "", "Shipment Reference No# " + FrayteNumber + " Deleted", EmailBody, Attachment, logoImage);
                }
                else
                {
                    FrayteEmail.SendMail("anil.kumar@irasys.biz", "", "Shipment Reference No# " + FrayteNumber + " Deleted", EmailBody, Attachment, logoImage);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ex.Message));
            }
        }

        #endregion

        #region -- Send Email After Payment To Customer --

        //public void SendPaymentEmail(Stripeitemmodel item, string TransactionNo)
        //{
        //    if (item != null && item.PaymentInfo != null)
        //    {
        //        DynamicViewBag viewBag = new DynamicViewBag();
        //        var operationzone = UtilityRepository.GetOperationZone();
        //        viewBag.AddValue("CustomerName", item.PaymentInfo.CompanyName);
        //        viewBag.AddValue("Date", System.DateTime.UtcNow.ToString("dd-MMM-yyyy HH:mm"));
        //        viewBag.AddValue("ShippingOrderNo", item.PaymentInfo.Invoice_no);
        //        viewBag.AddValue("PaymentNo", TransactionNo);
        //        viewBag.AddValue("Currency", item.PaymentInfo.Currency.CurrencyCode);
        //        viewBag.AddValue("Amount", Math.Round(item.Amount, 2));
        //        if (operationzone.OperationZoneId == 1)
        //        {
        //            viewBag.AddValue("SiteAddress", "www.frayte.com");
        //            viewBag.AddValue("SiteAdd", "www.frayte.com");
        //            viewBag.AddValue("Link", "Frayte HK");
        //            viewBag.AddValue("PhoneNo", "(+852) 2148 4880");
        //        }
        //        else
        //        {
        //            viewBag.AddValue("SiteAddress", "www.frayte.co.uk");
        //            viewBag.AddValue("SiteAdd", "www.frayte.co.uk");
        //            viewBag.AddValue("Link", "Frayte UK");
        //            viewBag.AddValue("PhoneNo", "(+44) 01792 277295");
        //        }
        //        string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Payment.cshtml");
        //        var templateService = new TemplateService();
        //        var CC = item.EmailCC == null ? "" : item.EmailCC;

        //        var EmailBody = templateService.Parse(template, "", viewBag, null);
        //        var EmailSubject = "Payment Confirmation -                             EmailSubject = "Booking Confirmation - " + shipmentDetail.DirectShipmentId + " - FRAYTE GLOBAL";

        //        FrayteEmail.SendMail(item.PaymentInfo.Email, CC, EmailSubject, EmailBody, "", "");
        //    }
        //}

        #endregion

        #region -- eCommerceShipment Mail --

        public void SendeCommercreBookingConfirmationMail(FrayteCommerceShipmentDraft eCommerceBookingDetail, int eCommerceShipmentId)
        {
            try
            {
                //Get Customer Name and Customer User Detail
                var customerDetail = (from u in dbContext.Users
                                      join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                      join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                      join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                      join tz in dbContext.Timezones on u.TimezoneId equals tz.TimezoneId
                                      where u.UserId == eCommerceBookingDetail.CustomerId
                                      select new
                                      {
                                          CustomerName = u.ContactName,
                                          CustomerEmail = u.Email,
                                          CompanyName = u.CompanyName,
                                          UserName = u1.ContactName,
                                          UserPosition = u1.Position,
                                          UserEmail = u1.Email,
                                          UserPhone = u1.TelephoneNo,
                                          UserSkype = u1.Skype,
                                          UserFax = u1.FaxNumber,
                                          TimeZoneDetail = new TimeZoneModal
                                          {
                                              Name = tz.Name,
                                              Offset = tz.Offset,
                                              OffsetShort = tz.OffsetShort,
                                              TimezoneId = tz.TimezoneId
                                          }
                                      }).FirstOrDefault();

                decimal TotalWeight = eCommerceBookingDetail.Packages.Sum(p => p.Weight);


                var logisticDetail = dbContext.CountryLogistics.Where(p => p.CountryId == eCommerceBookingDetail.ShipTo.Country.CountryId).FirstOrDefault();
                string ShipmentMethod = logisticDetail.LogisticService;

                var FrayteNo = dbContext.eCommerceShipments.Find(eCommerceShipmentId);

                var operationzone = UtilityRepository.GetOperationZone();

                #region TrackingNo, TrackingURL

                string TrackNo = string.Empty;
                string TrackUrl = string.Empty;
                var TrackingNo = dbContext.eCommerceShipments.Where(p => p.eCommerceShipmentId == eCommerceShipmentId).FirstOrDefault();
                if (TrackingNo != null)
                {
                    if (TrackingNo.TrackingDetail != null)
                    {
                        if (TrackingNo.TrackingDetail.Contains("order"))
                        {
                            var dslist = dbContext.eCommerceShipmentDetails.Where(p => p.eCommerceShipmentId == TrackingNo.eCommerceShipmentId).ToList();
                            if (dslist != null)
                            {
                                foreach (var ds in dslist)
                                {
                                    var ETrack = dbContext.eCommerceShipmentEasyPosts.Where(p => p.ShipmentId == ds.eCommerceShipmentDetailId).Select(p => p.TrackingCode).FirstOrDefault();
                                    if (ETrack != null)
                                    {
                                        TrackNo = ETrack;
                                        //TrackUrl = AppSettings.HostName + "/index.html#/home/tracking-hub/" + ShipmentMethod + "/" + ETrack + "/";
                                        TrackUrl = AppSettings.TrackingUrl + "/tracking-detail/" + ShipmentMethod + "/" + ETrack + "/";
                                    }
                                }
                            }
                        }
                        else
                        {
                            TrackNo = TrackingNo.TrackingDetail;
                            //TrackUrl = AppSettings.HostName + "/index.html#/home/tracking-hub/UKEUShipment/" + TrackingNo.TrackingDetail;
                            TrackUrl = AppSettings.TrackingUrl + "/tracking-detail/UKEUShipment/" + TrackingNo.TrackingDetail + "/";
                        }
                    }
                }

                #endregion

                DynamicViewBag viewBag = new DynamicViewBag();
                if (customerDetail.TimeZoneDetail != null)
                {
                    viewBag.AddValue("CreatedOn", UtilityRepository.GetTimeZoneCurrentDateTime(customerDetail.TimeZoneDetail.Name).ToString("dd-MMM-yyyy hh:mm")); ;
                    viewBag.AddValue("TimeZone", customerDetail.TimeZoneDetail.OffsetShort);
                }
                else
                {
                    viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                }

                viewBag.AddValue("TotalWeight", TotalWeight);
                viewBag.AddValue("ShipmentMethod", ShipmentMethod);
                viewBag.AddValue("TrackingNo", TrackNo);
                viewBag.AddValue("TrackingURL", TrackUrl);
                viewBag.AddValue("CustomerName", customerDetail.CustomerName + " (Consignee)");
                viewBag.AddValue("CustomerEmail", customerDetail.CustomerEmail);
                viewBag.AddValue("CompanyName", customerDetail.CompanyName);
                viewBag.AddValue("UserName", customerDetail.UserName);
                viewBag.AddValue("UserPosition", customerDetail.UserPosition);
                viewBag.AddValue("UserEmail", customerDetail.UserEmail);
                viewBag.AddValue("UserPhone", customerDetail.UserPhone);
                viewBag.AddValue("UserSkype", customerDetail.UserSkype);
                viewBag.AddValue("UserFax", customerDetail.UserFax);
                viewBag.AddValue("ImageHeader", "FrayteLogo");
                viewBag.AddValue("TrackButton", "TrackShipment");
                viewBag.AddValue("FrayteNumber", FrayteNo.FrayteNumber);
                if (eCommerceBookingDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
                {
                    viewBag.AddValue("WeightUnit", "kg");
                }
                else if (eCommerceBookingDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
                {
                    viewBag.AddValue("WeightUnit", "lb");
                }
                else
                {
                    viewBag.AddValue("WeightUnit", "kg");
                }
                if (eCommerceBookingDetail.OpearionZoneId == 1)
                {
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                }
                else
                {
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                }

                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/eCommerceBookingConfirmation.cshtml");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, eCommerceBookingDetail, viewBag, null);
                string EmailSubject = string.Empty;
                if (eCommerceBookingDetail.ShipFrom.CompanyName == "" || eCommerceBookingDetail.ShipFrom.CompanyName == null)
                {
                    EmailSubject = "Booking Confirmation - (" + eCommerceBookingDetail.ShipFrom.FirstName + " " + eCommerceBookingDetail.ShipFrom.LastName + ")" + " - FR#" + FrayteNo.FrayteNumber + " - FRAYTE GLOBAL";
                }
                else
                {
                    EmailSubject = "Booking Confirmation - (" + eCommerceBookingDetail.ShipFrom.CompanyName + ")" + " - FR#" + FrayteNo.FrayteNumber + " - FRAYTE GLOBAL";
                }

                var To = customerDetail.CustomerEmail;
                var CC = customerDetail.UserEmail;
                string Status = "Confirmation";

                #region Attach Labels

                string Attachment = new eCommerceShipmentRepository().PackageLabelPDFPath(eCommerceShipmentId, eCommerceShipmentType.eCommerceOnline, "");

                #endregion

                //Send mail to Customer
                SendMail_New(To, CC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment, Status);
                if (eCommerceBookingDetail.ShipFrom.IsMailSend)
                {
                    //Send mail to Ship From
                    if (!string.IsNullOrEmpty(eCommerceBookingDetail.ShipFrom.Email))
                    {
                        viewBag = new DynamicViewBag();
                        if (customerDetail.TimeZoneDetail != null)
                        {
                            viewBag.AddValue("CreatedOn", UtilityRepository.GetTimeZoneCurrentDateTime(customerDetail.TimeZoneDetail.Name).ToString("dd-MMM-yyyy hh:mm")); ;
                            viewBag.AddValue("TimeZone", customerDetail.TimeZoneDetail.OffsetShort);
                        }
                        else
                        {
                            viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                        }
                        viewBag.AddValue("TotalWeight", TotalWeight);
                        viewBag.AddValue("ShipmentMethod", ShipmentMethod);
                        viewBag.AddValue("TrackingNo", TrackNo);
                        viewBag.AddValue("TrackingURL", TrackUrl);
                        viewBag.AddValue("CustomerName", eCommerceBookingDetail.ShipFrom.FirstName + " " + eCommerceBookingDetail.ShipFrom.LastName + " (Shipper)");
                        viewBag.AddValue("CustomerEmail", eCommerceBookingDetail.ShipFrom.Email);
                        viewBag.AddValue("CompanyName", eCommerceBookingDetail.ShipFrom.CompanyName);
                        viewBag.AddValue("UserName", customerDetail.UserName);
                        viewBag.AddValue("UserPosition", customerDetail.UserPosition);
                        viewBag.AddValue("UserEmail", customerDetail.UserEmail);
                        viewBag.AddValue("UserPhone", customerDetail.UserPhone);
                        viewBag.AddValue("UserSkype", customerDetail.UserSkype);
                        viewBag.AddValue("UserFax", customerDetail.UserFax);
                        viewBag.AddValue("ImageHeader", "FrayteLogo");
                        viewBag.AddValue("TrackButton", "TrackShipment");
                        viewBag.AddValue("FrayteNumber", FrayteNo.FrayteNumber);
                        if (eCommerceBookingDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
                        {
                            viewBag.AddValue("WeightUnit", "kg");
                        }
                        else if (eCommerceBookingDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
                        {
                            viewBag.AddValue("WeightUnit", "lb");
                        }
                        else
                        {
                            viewBag.AddValue("WeightUnit", "kg");
                        }
                        if (eCommerceBookingDetail.OpearionZoneId == 1)
                        {
                            viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                        }
                        else
                        {
                            viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                        }

                        template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/eCommerceBookingConfirmation.cshtml");

                        templateService = new TemplateService();
                        EmailBody = templateService.Parse(template, eCommerceBookingDetail, viewBag, null);
                        if (eCommerceBookingDetail.ShipFrom.CompanyName == "" || eCommerceBookingDetail.ShipFrom.CompanyName == null)
                        {
                            EmailSubject = "Booking Confirmation - (" + eCommerceBookingDetail.ShipFrom.FirstName + " " + eCommerceBookingDetail.ShipFrom.LastName + ")" + " - FR#" + FrayteNo.FrayteNumber + " - FRAYTE GLOBAL";
                        }
                        else
                        {
                            EmailSubject = "Booking Confirmation - (" + eCommerceBookingDetail.ShipFrom.CompanyName + ")" + " - FR#" + FrayteNo.FrayteNumber + " - FRAYTE GLOBAL";
                        }
                        To = eCommerceBookingDetail.ShipFrom.Email;
                        CC = "";

                        #region Attach Labels

                        string Attachment1 = new eCommerceShipmentRepository().PackageLabelPath(eCommerceShipmentId);

                        #endregion

                        SendMail_New(To, CC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment1, Status);
                    }
                }

                if (eCommerceBookingDetail.ShipTo.IsMailSend)
                {
                    //Send mail to Ship To
                    if (!string.IsNullOrEmpty(eCommerceBookingDetail.ShipTo.Email))
                    {
                        viewBag = new DynamicViewBag();
                        if (customerDetail.TimeZoneDetail != null)
                        {
                            viewBag.AddValue("CreatedOn", UtilityRepository.GetTimeZoneCurrentDateTime(customerDetail.TimeZoneDetail.Name).ToString("dd-MMM-yyyy hh:mm")); ;
                            viewBag.AddValue("TimeZone", customerDetail.TimeZoneDetail.OffsetShort);
                        }
                        else
                        {
                            viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                        }
                        viewBag.AddValue("TotalWeight", TotalWeight);
                        viewBag.AddValue("ShipmentMethod", ShipmentMethod);
                        viewBag.AddValue("TrackingNo", TrackNo);
                        viewBag.AddValue("TrackingURL", TrackUrl);
                        viewBag.AddValue("CustomerName", eCommerceBookingDetail.ShipTo.FirstName + " " + eCommerceBookingDetail.ShipTo.LastName + " (Customer)");
                        viewBag.AddValue("CustomerEmail", eCommerceBookingDetail.ShipTo.Email);
                        viewBag.AddValue("CompanyName", eCommerceBookingDetail.ShipTo.CompanyName);
                        viewBag.AddValue("UserName", customerDetail.UserName);
                        viewBag.AddValue("UserPosition", customerDetail.UserPosition);
                        viewBag.AddValue("UserEmail", customerDetail.UserEmail);
                        viewBag.AddValue("UserPhone", customerDetail.UserPhone);
                        viewBag.AddValue("UserSkype", customerDetail.UserSkype);
                        viewBag.AddValue("UserFax", customerDetail.UserFax);
                        viewBag.AddValue("ImageHeader", "FrayteLogo");
                        viewBag.AddValue("TrackButton", "TrackShipment");
                        viewBag.AddValue("FrayteNumber", FrayteNo.FrayteNumber);
                        if (eCommerceBookingDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
                        {
                            viewBag.AddValue("WeightUnit", "kg");
                        }
                        else if (eCommerceBookingDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
                        {
                            viewBag.AddValue("WeightUnit", "lb");
                        }
                        else
                        {
                            viewBag.AddValue("WeightUnit", "kg");
                        }
                        if (eCommerceBookingDetail.OpearionZoneId == 1)
                        {
                            viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                        }
                        else
                        {
                            viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                        }

                        template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/eCommerceBookingConfirmation.cshtml");

                        templateService = new TemplateService();
                        EmailBody = templateService.Parse(template, eCommerceBookingDetail, viewBag, null);
                        if (eCommerceBookingDetail.ShipFrom.CompanyName == "" || eCommerceBookingDetail.ShipFrom.CompanyName == null)
                        {
                            EmailSubject = "Booking Confirmation - (" + eCommerceBookingDetail.ShipFrom.FirstName + " " + eCommerceBookingDetail.ShipFrom.LastName + ")" + " - FR#" + FrayteNo.FrayteNumber + " - FRAYTE GLOBAL";
                        }
                        else
                        {
                            EmailSubject = "Booking Confirmation - (" + eCommerceBookingDetail.ShipFrom.CompanyName + ")" + " - FR#" + FrayteNo.FrayteNumber + " - FRAYTE GLOBAL";
                        }
                        To = eCommerceBookingDetail.ShipTo.Email;
                        CC = "";

                        #region Attach Labels

                        string Attachment2 = new eCommerceShipmentRepository().PackageLabelPath(eCommerceShipmentId);

                        #endregion

                        SendMail_New(To, CC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment2, Status);
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        public void SendeCommerceShipmentErrorMail(FrayteCommerceShipmentDraft eCommerceBookingDetail, FratyteError error)
        {
            throw new NotImplementedException();
        }

        public void SendeCommerceBookingLabel(string ToMail, int eCommerceShipmentId, string CourierName)
        {
            FrayteeCommerceShipmentDetail dbDetail = new eCommerceShipmentRepository().GeteCommerceBookingDetail(eCommerceShipmentId, "");

            decimal TotalWeight = 0.0m;
            foreach (var obj in dbDetail.Packages)
            {
                TotalWeight += obj.Weight * obj.CartoonValue;
            }

            string ShipmentMethod = string.Empty;


            string Shipment = string.Empty;

            var logisticDetail = dbContext.CountryLogistics.Where(p => p.CountryId == dbDetail.ShipTo.Country.CountryId).FirstOrDefault();
            ShipmentMethod = logisticDetail.LogisicServiceDisplay;
            Shipment = logisticDetail.LogisticService;
            #region TrackingNo, Tracking URL

            string TrackNo = string.Empty;
            string TrackUrl = string.Empty;
            var TrackingNo = dbContext.eCommerceShipments.Where(p => p.eCommerceShipmentId == eCommerceShipmentId).FirstOrDefault();
            if (TrackingNo != null)
            {
                if (TrackingNo.TrackingDetail != null)
                {
                    if (TrackingNo.TrackingDetail.Contains("order"))
                    {
                        var dslist = dbContext.eCommerceShipmentDetails.Where(p => p.eCommerceShipmentId == TrackingNo.eCommerceShipmentId).ToList();
                        if (dslist != null)
                        {
                            foreach (var ds in dslist)
                            {
                                var ETrack = dbContext.eCommercePackageTrackingDetails.Where(p => p.eCommerceShipmentDetailId == ds.eCommerceShipmentDetailId).Select(p => p.TrackingNo).FirstOrDefault();
                                if (ETrack != null)
                                {
                                    TrackNo = ETrack;
                                    TrackUrl = AppSettings.TrackingUrl + "/index.html#/home/tracking-hub/" + Shipment + "/" + ETrack + "/";
                                }
                            }
                        }
                    }
                    else
                    {
                        TrackNo = TrackingNo.TrackingDetail;
                        TrackUrl = AppSettings.TrackingUrl + "/index.html#/home/tracking-hub/UKEUShipment/" + TrackingNo.TrackingDetail;
                    }
                }
            }

            #endregion

            var customerDetail = (from u in dbContext.Users
                                  join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                  join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                  join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                  where u.UserId == dbDetail.CustomerId
                                  select new
                                  {
                                      CustomerName = u.ContactName,
                                      CustomerEmail = u.Email,
                                      CompanyName = u.CompanyName,
                                      UserName = u1.ContactName,
                                      UserPosition = u1.Position,
                                      UserEmail = u1.Email,
                                      UserPhone = u1.TelephoneNo,
                                      UserSkype = u1.Skype,
                                      UserFax = u1.FaxNumber
                                  }).FirstOrDefault();

            string Attachment = new eCommerceShipmentRepository().PackageLabelPDFPath(eCommerceShipmentId, TrackingNo.BookingApp, CourierName);

            var operationzone = UtilityRepository.GetOperationZone();

            DynamicViewBag viewBag = new DynamicViewBag();
            viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
            viewBag.AddValue("TotalWeight", TotalWeight);
            viewBag.AddValue("FrayteNumber", dbDetail.FrayteNumber);
            viewBag.AddValue("ShipmentMethod", ShipmentMethod);
            viewBag.AddValue("TrackingNo", TrackNo);
            viewBag.AddValue("TrackingURL", TrackUrl);
            viewBag.AddValue("TotalCartoon", dbDetail.Packages.Sum(p => p.CartoonValue));
            viewBag.AddValue("CustomerName", dbDetail.ShipTo.FirstName + " " + dbDetail.ShipTo.LastName);
            viewBag.AddValue("CustomerEmail", dbDetail.ShipTo.Email);
            viewBag.AddValue("CompanyName", dbDetail.ShipTo.FirstName + " " + dbDetail.ShipTo.LastName);
            viewBag.AddValue("UserName", customerDetail.UserName != null ? customerDetail.UserName : "");
            viewBag.AddValue("UserPosition", customerDetail.UserPosition);
            viewBag.AddValue("UserEmail", customerDetail.UserEmail);
            viewBag.AddValue("UserPhone", customerDetail.UserPhone);
            viewBag.AddValue("UserSkype", customerDetail.UserSkype);
            viewBag.AddValue("UserFax", customerDetail.UserFax);
            viewBag.AddValue("ImageHeader", "FrayteLogo");
            viewBag.AddValue("TrackButton", "TrackShipment");
            if (dbDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
            {
                viewBag.AddValue("WeightUnit", "kg");
            }
            else if (dbDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
            {
                viewBag.AddValue("WeightUnit", "lb");
            }
            else
            {
                viewBag.AddValue("WeightUnit", "kg");
            }

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/eCommerceBookingConfirmation.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, dbDetail, viewBag, null);
            var EmailSubject = "Shipment Booking Detail With Label - FR#" + dbDetail.FrayteNumber + " - FRAYTE GLOBAL";
            var CC = "";

            //Send mail to Customer
            SendMail_New(ToMail, CC, "FRAYTE - System (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment, "Confirmation");
        }

        #region InVoice Mail
        public void sendInVoiceMail(int eCommerceShipmentId)
        {
            try
            {
                var dbDetail = new eCommerceShipmentRepository().GeteCommerceBookingDetail(eCommerceShipmentId, "");
                var customerDetail = (from u in dbContext.Users
                                      join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                      join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                      join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                      join tz in dbContext.Timezones on u.TimezoneId equals tz.TimezoneId
                                      where u.UserId == dbDetail.CustomerId
                                      select new
                                      {
                                          CustomerName = u.ContactName,
                                          CustomerEmail = u.Email,
                                          CompanyName = u.CompanyName,
                                          UserName = u1.ContactName,
                                          UserPosition = u1.Position,
                                          UserEmail = u1.Email,
                                          UserPhone = u1.TelephoneNo,
                                          UserSkype = u1.Skype,
                                          UserFax = u1.FaxNumber,
                                          TimeZoneDetail = new TimeZoneModal
                                          {
                                              Name = tz.Name,
                                              Offset = tz.Offset,
                                              OffsetShort = tz.OffsetShort,
                                              TimezoneId = tz.TimezoneId
                                          }
                                      }).FirstOrDefault();

                DynamicViewBag viewBag = new DynamicViewBag();

                //
                var logisticDetail = dbContext.CountryLogistics.Where(p => p.CountryId == dbDetail.ShipTo.Country.CountryId).FirstOrDefault();
                string ShipmentMethod = logisticDetail.LogisticService;

                var FrayteNo = dbContext.eCommerceShipments.Find(eCommerceShipmentId);

                var operationzone = UtilityRepository.GetOperationZone();

                #region TrackingNo, TrackingURL

                string TrackNo = string.Empty;
                string TrackUrl = string.Empty;
                var TrackingNo = dbContext.eCommerceShipments.Where(p => p.eCommerceShipmentId == eCommerceShipmentId).FirstOrDefault();
                if (TrackingNo != null)
                {
                    if (TrackingNo.TrackingDetail != null)
                    {
                        if (TrackingNo.TrackingDetail.Contains("order"))
                        {
                            var dslist = dbContext.eCommerceShipmentDetails.Where(p => p.eCommerceShipmentId == TrackingNo.eCommerceShipmentId).ToList();
                            if (dslist != null)
                            {
                                var id = dslist[0].eCommerceShipmentDetailId;
                                var ETrack = dbContext.eCommercePackageTrackingDetails.Where(p => p.eCommerceShipmentDetailId == id).Select(p => p.TrackingNo).FirstOrDefault();
                                if (ETrack != null)
                                {
                                    TrackNo = ETrack;
                                    TrackUrl = AppSettings.TrackingUrl + "/index.html#/home/tracking-detail/" + ShipmentMethod + "/" + ETrack + "/";
                                }

                            }
                        }
                        else
                        {
                            TrackNo = TrackingNo.TrackingDetail;
                            TrackUrl = AppSettings.TrackingUrl + "/index.html#/home/tracking-detail/UKEUShipment/" + TrackingNo.TrackingDetail;
                        }
                    }
                }

                #endregion

                if (customerDetail.TimeZoneDetail != null)
                {
                    viewBag.AddValue("CreatedOn", UtilityRepository.GetTimeZoneCurrentDateTime(customerDetail.TimeZoneDetail.Name).ToString("dd-MMM-yyyy hh:mm")); ;
                    viewBag.AddValue("TimeZone", customerDetail.TimeZoneDetail.OffsetShort);
                }
                else
                {
                    viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                }
                viewBag.AddValue("ShipmentMethod", ShipmentMethod);
                viewBag.AddValue("TrackingNo", TrackNo);
                viewBag.AddValue("TrackingURL", TrackUrl);
                viewBag.AddValue("CustomerName", customerDetail.CustomerName + " (Consignee)");
                viewBag.AddValue("CustomerEmail", customerDetail.CustomerEmail);
                viewBag.AddValue("CompanyName", customerDetail.CompanyName);
                viewBag.AddValue("UserName", customerDetail.UserName);
                viewBag.AddValue("UserPosition", customerDetail.UserPosition);
                viewBag.AddValue("UserEmail", customerDetail.UserEmail);
                viewBag.AddValue("UserPhone", customerDetail.UserPhone);
                viewBag.AddValue("UserSkype", customerDetail.UserSkype);
                viewBag.AddValue("UserFax", customerDetail.UserFax);
                viewBag.AddValue("ImageHeader", "FrayteLogo");
                viewBag.AddValue("TrackButton", "TrackShipment");
                viewBag.AddValue("FrayteNumber", FrayteNo.FrayteNumber);
                viewBag.AddValue("ReceiverName", dbDetail.ShipTo.FirstName + " " + dbDetail.ShipTo.LastName);


                if (dbDetail.OpearionZoneId == 1)
                {
                    viewBag.AddValue("OperationCountry", "Hong Kong");
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                    viewBag.AddValue("IMPGuideUrl", AppSettings.TrackingUrl + "/impguide");
                    viewBag.AddValue("TrackingEmail", "tracking@frayte.com");
                    viewBag.AddValue("TrackingPhone", "(+852) 2148 4880");
                    viewBag.AddValue("PaymentUrl", "payment.frayte.com");
                    viewBag.AddValue("StaffAddress1", "34 North Street");
                    viewBag.AddValue("StaffAddress2", "");
                    viewBag.AddValue("StaffCity", "Bridgwater");
                    viewBag.AddValue("StaffState", "Somerset");
                    viewBag.AddValue("StaffPostCode", "TA6 3YD");
                    viewBag.AddValue("StaffCountry", "United Kingdom");
                }
                else
                {
                    viewBag.AddValue("OperationCountry", "United Kingdom");
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                    viewBag.AddValue("IMPGuideUrl", AppSettings.TrackingUrl + "/impguide");
                    viewBag.AddValue("TrackingEmail", "tracking@frayte.co.uk");
                    viewBag.AddValue("TrackingPhone", "(+44) 01792 277295");
                    viewBag.AddValue("PaymentUrl", "payment.frayte.co.uk");
                    viewBag.AddValue("StaffAddress1", "34 North Street");
                    viewBag.AddValue("StaffAddress2", "");
                    viewBag.AddValue("StaffCity", "Bridgwater");
                    viewBag.AddValue("StaffState", "Somerset");
                    viewBag.AddValue("StaffPostCode", "TA6 3YD");
                    viewBag.AddValue("StaffCountry", "United Kingdom");
                }
                var invoiceDetail = dbContext.eCommerceInvoices.Where(p => p.ShipmentId == dbDetail.eCommerceShipmentId).FirstOrDefault();
                //
                if (invoiceDetail != null)
                {
                    var totalAmount = (float)(invoiceDetail.TotalAdminVAT + invoiceDetail.TotalVAT + invoiceDetail.AdminDisbursement);
                    viewBag.AddValue("InvoiceAmount", totalAmount.ToString());
                    viewBag.AddValue("InvoiceCurrency", invoiceDetail.CurrencyCode);
                }
                else
                {
                    viewBag.AddValue("InvoiceAmount", "0.00");
                    viewBag.AddValue("InvoiceCurrency", "USD");
                }

                // 
                string encriptedKey = UtilityRepository.ConvertStringToHex(dbDetail.FrayteNumber + "|" + dbDetail.ShipTo.Email, System.Text.Encoding.UTF8, EncriptionKey.PrivateKey);

                string enocoded = WebUtility.UrlEncode(encriptedKey);

                string PaymentUrl = AppSettings.PaymentUrl + "eComm/" + encriptedKey;

                viewBag.AddValue("PaymentTaxAndDutyUrl", PaymentUrl);
                string Attachment = new eCommerceShipmentRepository().InvoicePath(dbDetail.eCommerceShipmentId);
                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/eCommerceInvoice.cshtml");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, dbDetail, viewBag, null);
                var EmailSubject = "SHIPMENT Notification" + "(" + dbDetail.ReferenceDetail.Reference1 + ")";
                //var CC = "print@frayte.co.uk";
                var CC = "";
                //Send mail to Receiver
                if (!string.IsNullOrEmpty(Attachment) && !string.IsNullOrEmpty(dbDetail.ShipTo.Email))
                    SendMail_New(dbDetail.ShipTo.Email, CC, "FRAYTE - System (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment, "");
                else
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Receiver email is empty :" + dbDetail.FrayteNumber));

            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #endregion

        #region -- eCommerceUploadkShipment Mail --

        public void SendeCommercreUploadshipmentBookingConfirmationMail(FrayteUploadshipment eCommerceBookingDetail, int eCommerceShipmentId)
        {
            try
            {
                //Get Customer Name and Customer User Detail
                var customerDetail = (from u in dbContext.Users
                                      join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                      join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                      join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                      join tz in dbContext.Timezones on u.TimezoneId equals tz.TimezoneId
                                      where u.UserId == eCommerceBookingDetail.CustomerId
                                      select new
                                      {
                                          CustomerName = u.ContactName,
                                          CustomerEmail = u.Email,
                                          CompanyName = u.CompanyName,
                                          UserName = u1.ContactName,
                                          UserPosition = u1.Position,
                                          UserEmail = u1.Email,
                                          UserPhone = u1.TelephoneNo,
                                          UserSkype = u1.Skype,
                                          UserFax = u1.FaxNumber,
                                          TimeZoneDetail = new TimeZoneModal
                                          {
                                              Name = tz.Name,
                                              Offset = tz.Offset,
                                              OffsetShort = tz.OffsetShort,
                                              TimezoneId = tz.TimezoneId
                                          }
                                      }).FirstOrDefault();

                decimal TotalWeight = eCommerceBookingDetail.Package.Sum(p => p.Weight);


                var logisticDetail = dbContext.CountryLogistics.Where(p => p.CountryId == eCommerceBookingDetail.ShipTo.Country.CountryId).FirstOrDefault();
                string ShipmentMethod = logisticDetail.LogisticService;

                var FrayteNo = dbContext.eCommerceShipments.Find(eCommerceShipmentId);

                var operationzone = UtilityRepository.GetOperationZone();

                #region TrackingNo, TrackingURL

                string TrackNo = string.Empty;
                string TrackUrl = string.Empty;
                var TrackingNo = dbContext.eCommerceShipments.Where(p => p.eCommerceShipmentId == eCommerceShipmentId).FirstOrDefault();
                if (TrackingNo != null)
                {
                    if (TrackingNo.TrackingDetail != null)
                    {
                        if (TrackingNo.TrackingDetail.Contains("order"))
                        {
                            var dslist = dbContext.eCommerceShipmentDetails.Where(p => p.eCommerceShipmentId == TrackingNo.eCommerceShipmentId).ToList();
                            if (dslist != null)
                            {
                                foreach (var ds in dslist)
                                {
                                    var ETrack = dbContext.eCommerceShipmentEasyPosts.Where(p => p.ShipmentId == ds.eCommerceShipmentDetailId).Select(p => p.TrackingCode).FirstOrDefault();
                                    if (ETrack != null)
                                    {
                                        TrackNo = ETrack;
                                        TrackUrl = AppSettings.HostName + "/index.html#/home/tracking-hub/" + ShipmentMethod + "/" + ETrack + "/";
                                    }
                                }
                            }
                        }
                        else
                        {
                            TrackNo = TrackingNo.TrackingDetail;
                            TrackUrl = AppSettings.HostName + "/index.html#/home/tracking-hub/UKEUShipment/" + TrackingNo.TrackingDetail;
                        }
                    }
                }

                #endregion

                DynamicViewBag viewBag = new DynamicViewBag();
                if (customerDetail.TimeZoneDetail != null)
                {
                    viewBag.AddValue("CreatedOn", UtilityRepository.GetTimeZoneCurrentDateTime(customerDetail.TimeZoneDetail.Name).ToString("dd-MMM-yyyy hh:mm")); ;
                    viewBag.AddValue("TimeZone", customerDetail.TimeZoneDetail.OffsetShort);
                }
                else
                {
                    viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                }

                viewBag.AddValue("TotalWeight", TotalWeight);
                viewBag.AddValue("ShipmentMethod", ShipmentMethod);
                viewBag.AddValue("TrackingNo", TrackNo);
                viewBag.AddValue("TrackingURL", TrackUrl);
                viewBag.AddValue("CustomerName", customerDetail.CustomerName + " (Consignee)");
                viewBag.AddValue("CustomerEmail", customerDetail.CustomerEmail);
                viewBag.AddValue("CompanyName", customerDetail.CompanyName);
                viewBag.AddValue("UserName", customerDetail.UserName);
                viewBag.AddValue("UserPosition", customerDetail.UserPosition);
                viewBag.AddValue("UserEmail", customerDetail.UserEmail);
                viewBag.AddValue("UserPhone", customerDetail.UserPhone);
                viewBag.AddValue("UserSkype", customerDetail.UserSkype);
                viewBag.AddValue("UserFax", customerDetail.UserFax);
                viewBag.AddValue("ImageHeader", "FrayteLogo");
                viewBag.AddValue("TrackButton", "TrackShipment");
                viewBag.AddValue("FrayteNumber", FrayteNo.FrayteNumber);
                if (eCommerceBookingDetail.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
                {
                    viewBag.AddValue("WeightUnit", "kg");
                }
                else if (eCommerceBookingDetail.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
                {
                    viewBag.AddValue("WeightUnit", "lb");
                }
                else
                {
                    viewBag.AddValue("WeightUnit", "kg");
                }
                if (eCommerceBookingDetail.OpearionZoneId == 1)
                {
                    viewBag.AddValue("SiteAddress", "www.frayte.com");
                }
                else
                {
                    viewBag.AddValue("SiteAddress", "www.frayte.co.uk");
                }

                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/eCommerceBookingConfirmation.cshtml");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, eCommerceBookingDetail, viewBag, null);
                string EmailSubject = string.Empty;
                if (eCommerceBookingDetail.ShipFrom.CompanyName == "" || eCommerceBookingDetail.ShipFrom.CompanyName == null)
                {
                    EmailSubject = "Booking Confirmation - (" + eCommerceBookingDetail.ShipFrom.FirstName + " " + eCommerceBookingDetail.ShipFrom.LastName + ")" + " - FR#" + FrayteNo.FrayteNumber + " - FRAYTE GLOBAL";
                }
                else
                {
                    EmailSubject = "Booking Confirmation - (" + eCommerceBookingDetail.ShipFrom.CompanyName + ")" + " - FR#" + FrayteNo.FrayteNumber + " - FRAYTE GLOBAL";
                }

                var To = customerDetail.CustomerEmail;
                var CC = customerDetail.UserEmail;
                string Status = "Confirmation";

                #region Attach Labels

                string Attachment = new eCommerceShipmentRepository().PackageLabelPath(eCommerceShipmentId);

                #endregion

                //Send mail to Customer
                SendMail_New(To, CC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment, Status);
                if (eCommerceBookingDetail.ShipFrom.IsMailSend)
                {
                    //Send mail to Ship From
                    if (!string.IsNullOrEmpty(eCommerceBookingDetail.ShipFrom.Email))
                    {
                        viewBag = new DynamicViewBag();
                        if (customerDetail.TimeZoneDetail != null)
                        {
                            viewBag.AddValue("CreatedOn", UtilityRepository.GetTimeZoneCurrentDateTime(customerDetail.TimeZoneDetail.Name).ToString("dd-MMM-yyyy hh:mm")); ;
                            viewBag.AddValue("TimeZone", customerDetail.TimeZoneDetail.OffsetShort);
                        }
                        else
                        {
                            viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                        }
                        viewBag.AddValue("TotalWeight", TotalWeight);
                        viewBag.AddValue("ShipmentMethod", ShipmentMethod);
                        viewBag.AddValue("TrackingNo", TrackNo);
                        viewBag.AddValue("TrackingURL", TrackUrl);
                        viewBag.AddValue("CustomerName", eCommerceBookingDetail.ShipFrom.FirstName + " " + eCommerceBookingDetail.ShipFrom.LastName + " (Shipper)");
                        viewBag.AddValue("CustomerEmail", eCommerceBookingDetail.ShipFrom.Email);
                        viewBag.AddValue("CompanyName", eCommerceBookingDetail.ShipFrom.CompanyName);
                        viewBag.AddValue("UserName", customerDetail.UserName);
                        viewBag.AddValue("UserPosition", customerDetail.UserPosition);
                        viewBag.AddValue("UserEmail", customerDetail.UserEmail);
                        viewBag.AddValue("UserPhone", customerDetail.UserPhone);
                        viewBag.AddValue("UserSkype", customerDetail.UserSkype);
                        viewBag.AddValue("UserFax", customerDetail.UserFax);
                        viewBag.AddValue("ImageHeader", "FrayteLogo");
                        viewBag.AddValue("TrackButton", "TrackShipment");
                        viewBag.AddValue("FrayteNumber", FrayteNo.FrayteNumber);
                        if (eCommerceBookingDetail.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
                        {
                            viewBag.AddValue("WeightUnit", "kg");
                        }
                        else if (eCommerceBookingDetail.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
                        {
                            viewBag.AddValue("WeightUnit", "lb");
                        }
                        else
                        {
                            viewBag.AddValue("WeightUnit", "kg");
                        }
                        if (eCommerceBookingDetail.OpearionZoneId == 1)
                        {
                            viewBag.AddValue("SiteAddress", "www.frayte.com");
                        }
                        else
                        {
                            viewBag.AddValue("SiteAddress", "www.frayte.co.uk");
                        }

                        template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/eCommerceBookingConfirmation.cshtml");

                        templateService = new TemplateService();
                        EmailBody = templateService.Parse(template, eCommerceBookingDetail, viewBag, null);
                        if (eCommerceBookingDetail.ShipFrom.CompanyName == "" || eCommerceBookingDetail.ShipFrom.CompanyName == null)
                        {
                            EmailSubject = "Booking Confirmation - (" + eCommerceBookingDetail.ShipFrom.FirstName + " " + eCommerceBookingDetail.ShipFrom.LastName + ")" + " - FR#" + FrayteNo.FrayteNumber + " - FRAYTE GLOBAL";
                        }
                        else
                        {
                            EmailSubject = "Booking Confirmation - (" + eCommerceBookingDetail.ShipFrom.CompanyName + ")" + " - FR#" + FrayteNo.FrayteNumber + " - FRAYTE GLOBAL";
                        }
                        To = eCommerceBookingDetail.ShipFrom.Email;
                        CC = "";

                        #region Attach Labels

                        string Attachment1 = new eCommerceShipmentRepository().PackageLabelPath(eCommerceShipmentId);

                        #endregion

                        SendMail_New(To, CC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment1, Status);
                    }
                }

                if (eCommerceBookingDetail.ShipTo.IsMailSend)
                {
                    //Send mail to Ship To
                    if (!string.IsNullOrEmpty(eCommerceBookingDetail.ShipTo.Email))
                    {
                        viewBag = new DynamicViewBag();
                        if (customerDetail.TimeZoneDetail != null)
                        {
                            viewBag.AddValue("CreatedOn", UtilityRepository.GetTimeZoneCurrentDateTime(customerDetail.TimeZoneDetail.Name).ToString("dd-MMM-yyyy hh:mm")); ;
                            viewBag.AddValue("TimeZone", customerDetail.TimeZoneDetail.OffsetShort);
                        }
                        else
                        {
                            viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                        }
                        viewBag.AddValue("TotalWeight", TotalWeight);
                        viewBag.AddValue("ShipmentMethod", ShipmentMethod);
                        viewBag.AddValue("TrackingNo", TrackNo);
                        viewBag.AddValue("TrackingURL", TrackUrl);
                        viewBag.AddValue("CustomerName", eCommerceBookingDetail.ShipTo.FirstName + " " + eCommerceBookingDetail.ShipTo.LastName + " (Customer)");
                        viewBag.AddValue("CustomerEmail", eCommerceBookingDetail.ShipTo.Email);
                        viewBag.AddValue("CompanyName", eCommerceBookingDetail.ShipTo.CompanyName);
                        viewBag.AddValue("UserName", customerDetail.UserName);
                        viewBag.AddValue("UserPosition", customerDetail.UserPosition);
                        viewBag.AddValue("UserEmail", customerDetail.UserEmail);
                        viewBag.AddValue("UserPhone", customerDetail.UserPhone);
                        viewBag.AddValue("UserSkype", customerDetail.UserSkype);
                        viewBag.AddValue("UserFax", customerDetail.UserFax);
                        viewBag.AddValue("ImageHeader", "FrayteLogo");
                        viewBag.AddValue("TrackButton", "TrackShipment");
                        viewBag.AddValue("FrayteNumber", FrayteNo.FrayteNumber);
                        if (eCommerceBookingDetail.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
                        {
                            viewBag.AddValue("WeightUnit", "kg");
                        }
                        else if (eCommerceBookingDetail.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
                        {
                            viewBag.AddValue("WeightUnit", "lb");
                        }
                        else
                        {
                            viewBag.AddValue("WeightUnit", "kg");
                        }
                        if (eCommerceBookingDetail.OpearionZoneId == 1)
                        {
                            viewBag.AddValue("SiteAddress", "www.frayte.com");
                        }
                        else
                        {
                            viewBag.AddValue("SiteAddress", "www.frayte.co.uk");
                        }

                        template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/eCommerceBookingConfirmation.cshtml");

                        templateService = new TemplateService();
                        EmailBody = templateService.Parse(template, eCommerceBookingDetail, viewBag, null);
                        if (eCommerceBookingDetail.ShipFrom.CompanyName == "" || eCommerceBookingDetail.ShipFrom.CompanyName == null)
                        {
                            EmailSubject = "Booking Confirmation - (" + eCommerceBookingDetail.ShipFrom.FirstName + " " + eCommerceBookingDetail.ShipFrom.LastName + ")" + " - FR#" + FrayteNo.FrayteNumber + " - FRAYTE GLOBAL";
                        }
                        else
                        {
                            EmailSubject = "Booking Confirmation - (" + eCommerceBookingDetail.ShipFrom.CompanyName + ")" + " - FR#" + FrayteNo.FrayteNumber + " - FRAYTE GLOBAL";
                        }
                        To = eCommerceBookingDetail.ShipTo.Email;
                        CC = "";

                        #region Attach Labels

                        string Attachment2 = new eCommerceShipmentRepository().PackageLabelPath(eCommerceShipmentId);

                        #endregion

                        SendMail_New(To, CC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment2, Status);
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        public void SendeCommerceUploadShipmentErrorMail(FrayteUploadshipment eCommerceBookingDetail, FratyteError error)
        {
            throw new NotImplementedException();
        }

        public void SendeCommerceUploadshipmentBookingLabel(string ToMail, int eCommerceShipmentId, string CourierName)
        {
            FrayteeCommerceShipmentDetail dbDetail = new eCommerceShipmentRepository().GeteCommerceBookingDetail(eCommerceShipmentId, "");

            decimal TotalWeight = 0.0m;
            foreach (var obj in dbDetail.Packages)
            {
                TotalWeight += obj.Weight * obj.CartoonValue;
            }

            string ShipmentMethod = string.Empty;


            string Shipment = string.Empty;

            var logisticDetail = dbContext.CountryLogistics.Where(p => p.CountryId == dbDetail.ShipTo.Country.CountryId).FirstOrDefault();
            ShipmentMethod = logisticDetail.LogisicServiceDisplay;
            Shipment = logisticDetail.LogisticService;
            #region TrackingNo, Tracking URL

            string TrackNo = string.Empty;
            string TrackUrl = string.Empty;
            var TrackingNo = dbContext.eCommerceShipments.Where(p => p.eCommerceShipmentId == eCommerceShipmentId).FirstOrDefault();
            if (TrackingNo != null)
            {
                if (TrackingNo.TrackingDetail != null)
                {
                    if (TrackingNo.TrackingDetail.Contains("order"))
                    {
                        var dslist = dbContext.eCommerceShipmentDetails.Where(p => p.eCommerceShipmentId == TrackingNo.eCommerceShipmentId).ToList();
                        if (dslist != null)
                        {
                            foreach (var ds in dslist)
                            {
                                var ETrack = dbContext.eCommercePackageTrackingDetails.Where(p => p.eCommerceShipmentDetailId == ds.eCommerceShipmentDetailId).Select(p => p.TrackingNo).FirstOrDefault();
                                if (ETrack != null)
                                {
                                    TrackNo = ETrack;
                                    TrackUrl = AppSettings.HostName + "/index.html#/home/tracking-hub/" + Shipment + "/" + ETrack + "/";
                                }
                            }
                        }
                    }
                    else
                    {
                        TrackNo = TrackingNo.TrackingDetail;
                        TrackUrl = AppSettings.HostName + "/index.html#/home/tracking-hub/UKEUShipment/" + TrackingNo.TrackingDetail;
                    }
                }
            }

            #endregion

            var customerDetail = (from u in dbContext.Users
                                  join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                  join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                  join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                  where u.UserId == dbDetail.CustomerId
                                  select new
                                  {
                                      CustomerName = u.ContactName,
                                      CustomerEmail = u.Email,
                                      CompanyName = u.CompanyName,
                                      UserName = u1.ContactName,
                                      UserPosition = u1.Position,
                                      UserEmail = u1.Email,
                                      UserPhone = u1.TelephoneNo,
                                      UserSkype = u1.Skype,
                                      UserFax = u1.FaxNumber
                                  }).FirstOrDefault();

            string Attachment = new eCommerceShipmentRepository().PackageLabelPath(eCommerceShipmentId);

            var operationzone = UtilityRepository.GetOperationZone();

            DynamicViewBag viewBag = new DynamicViewBag();
            viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
            viewBag.AddValue("TotalWeight", TotalWeight);
            viewBag.AddValue("FrayteNumber", dbDetail.FrayteNumber);
            viewBag.AddValue("ShipmentMethod", ShipmentMethod);
            viewBag.AddValue("TrackingNo", TrackNo);
            viewBag.AddValue("TrackingURL", TrackUrl);
            viewBag.AddValue("TotalCartoon", dbDetail.Packages.Sum(p => p.CartoonValue));
            viewBag.AddValue("CustomerName", dbDetail.ShipTo.FirstName + " " + dbDetail.ShipTo.LastName);
            viewBag.AddValue("CustomerEmail", dbDetail.ShipTo.Email);
            viewBag.AddValue("CompanyName", dbDetail.ShipTo.FirstName + " " + dbDetail.ShipTo.LastName);
            viewBag.AddValue("UserName", customerDetail.UserName != null ? customerDetail.UserName : "");
            viewBag.AddValue("UserPosition", customerDetail.UserPosition);
            viewBag.AddValue("UserEmail", customerDetail.UserEmail);
            viewBag.AddValue("UserPhone", customerDetail.UserPhone);
            viewBag.AddValue("UserSkype", customerDetail.UserSkype);
            viewBag.AddValue("UserFax", customerDetail.UserFax);
            viewBag.AddValue("ImageHeader", "FrayteLogo");
            viewBag.AddValue("TrackButton", "TrackShipment");
            if (dbDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
            {
                viewBag.AddValue("WeightUnit", "kg");
            }
            else if (dbDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
            {
                viewBag.AddValue("WeightUnit", "lb");
            }
            else
            {
                viewBag.AddValue("WeightUnit", "kg");
            }

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/eCommerceBookingConfirmation.cshtml");

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, dbDetail, viewBag, null);
            var EmailSubject = "Shipment Booking Detail With Label - FR#" + dbDetail.FrayteNumber + " - FRAYTE GLOBAL";
            var CC = "";

            //Send mail to Customer
            SendMail_New(ToMail, CC, "FRAYTE - System (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment, "Confirmation");
        }

        #endregion

        #region -- Send Customer Rate Card And Quotation Email --

        public FrayteResult EmailCustomerRateCard(CustomerRate Customer, string FileName, string FilePath)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var operationzone = UtilityRepository.GetOperationZone();
                string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
                List<string> filename = new List<string>();
                filename.Add(FileName);
                var Model = new FrayteCuastomerRateCard();
                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/CustomerRateCard.cshtml");

                Model = (from u in dbContext.Users
                         join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                         join uu in dbContext.Users on ua.SalesUserId equals uu.UserId into leftJoin
                         from lj in leftJoin
                         where u.UserId == Customer.UserId
                         select new FrayteCuastomerRateCard
                         {
                             OperationZoneId = u.OperationZoneId,
                             CompanyName = u.CompanyName,
                             ContactName = u.ContactName,
                             CustomerEmail = u.Email,
                             StaffEmail = (lj == null ? "" : lj.Email),
                             StaffName = (lj == null ? "" : lj.ContactName),
                             SenderDept = "Sales Representative"
                         }).FirstOrDefault();

                if (Model == null || Model.StaffEmail == "" || Model.StaffEmail == null)
                {
                    Model = (from u in dbContext.Users
                             join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                             join uu in dbContext.Users on ua.OperationUserId equals uu.UserId into leftJoin
                             from lj in leftJoin
                             where u.UserId == Customer.UserId
                             select new FrayteCuastomerRateCard
                             {
                                 OperationZoneId = u.OperationZoneId,
                                 CompanyName = u.CompanyName,
                                 ContactName = u.ContactName,
                                 CustomerEmail = u.Email,
                                 StaffEmail = (lj == null ? "" : lj.Email),
                                 StaffName = (lj == null ? "" : lj.ContactName),
                                 SenderDept = "Operation Staff"
                             }).FirstOrDefault();
                }
                if (Model != null)
                {
                    Model.CompanyName = !string.IsNullOrEmpty(Customer.CustomerName) ? Customer.CustomerName : Model.CompanyName;
                    Model.FileName = filename;
                    if (Model.OperationZoneId == 1)
                    {
                        Model.Business = AppSettings.TrackingUrl;
                        Model.SiteAddress = AppSettings.TrackingUrl;
                        Model.SiteLink = "FRAYTE GLOBAL";
                        Model.PhoneNumber = "(+852) 2148 4880";
                    }
                    else
                    {
                        Model.Business = AppSettings.TrackingUrl;
                        Model.SiteAddress = AppSettings.TrackingUrl;
                        Model.SiteLink = "FRAYTE GLOBAL";
                        Model.PhoneNumber = "(+44) 01792 277295";
                    }

                    DynamicViewBag viewBag = new DynamicViewBag();
                    viewBag.AddValue("ImageHeader", "FrayteLogo");

                    var templateService = new TemplateService();
                    var EmailBody = templateService.Parse(template, Model, viewBag, null);
                    var EmailSubject = Model.CompanyName + " Rate Cards - FRAYTE GLOBAL";
                    var To = Customer.CustomerEmail;
                    var CC = Customer.CCEmail == null ? "" : Customer.CCEmail;
                    var BCC = Customer.BCCEmail == null ? "" : Customer.BCCEmail;

                    if (!string.IsNullOrEmpty(AppSettings.TOCC))
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Customer Rate Card Email TOCC " + To));
                        FrayteEmail.SendMail(AppSettings.TOCC, CC, BCC, EmailSubject, EmailBody, FilePath, logoImage);
                        result.Status = true;
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(EmailBody));
                    }
                    else
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Customer Rate Card Email " + To));
                        FrayteEmail.SendMail(To, CC, BCC, EmailSubject, EmailBody, FilePath, logoImage);
                        result.Status = true;
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(EmailBody));
                    }
                }
                else
                {
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return result;
        }

        public FrayteResult EmailCustomerRateCard(int UserId, List<string> filename, List<string> filepath, int OperationZoneId)
        {
            FrayteResult result = new FrayteResult();
            string filePath = @"C:\FMS\FRAYTE HK\Error.txt";
            try
            {
                string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";

                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/CustomerRateCard.cshtml");

                var Model = (from u in dbContext.Users
                             join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                             join uu in dbContext.Users on ua.SalesUserId equals uu.UserId into leftJoin
                             from lj in leftJoin
                             where u.UserId == UserId
                             select new FrayteCuastomerRateCard
                             {
                                 OperationZoneId = u.OperationZoneId,
                                 CompanyName = u.CompanyName,
                                 ContactName = u.ContactName,
                                 CustomerEmail = u.UserEmail,
                                 StaffEmail = (lj == null || lj.Email == "" ? "" : lj.Email),
                                 StaffName = (lj == null || lj.Email == "" ? "" : lj.ContactName),
                                 SenderDept = "Sales Representative"
                             }).FirstOrDefault();

                if (Model == null || Model.StaffEmail == "" || Model.StaffEmail == null)
                {
                    Model = (from u in dbContext.Users
                             join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                             join uu in dbContext.Users on ua.OperationUserId equals uu.UserId into leftJoin
                             from lj in leftJoin
                             where u.UserId == UserId
                             select new FrayteCuastomerRateCard
                             {
                                 OperationZoneId = u.OperationZoneId,
                                 CompanyName = u.CompanyName,
                                 ContactName = u.ContactName,
                                 CustomerEmail = u.UserEmail,
                                 StaffEmail = (lj == null ? "" : lj.Email),
                                 StaffName = (lj == null ? "" : lj.ContactName),
                                 SenderDept = "Operation Staff"
                             }).FirstOrDefault();
                }

                if (Model != null)
                {
                    Model.FileName = filename;

                    if (Model.OperationZoneId == 1)
                    {
                        Model.Business = "www.frayte.com";
                        Model.SiteAddress = "www.frayte.com";
                        Model.SiteLink = "FRAYTE GLOBAL";
                        Model.PhoneNumber = "(+852) 2148 4880";
                    }
                    else
                    {
                        Model.Business = "www.frayte.co.uk";
                        Model.SiteAddress = "www.frayte.co.uk";
                        Model.SiteLink = "FRAYTE GLOBAL";
                        Model.PhoneNumber = "(+44) 01792 277295";
                    }

                    DynamicViewBag viewBag = new DynamicViewBag();
                    viewBag.AddValue("ImageHeader", "FrayteLogo");

                    var templateService = new TemplateService();
                    var EmailBody = templateService.Parse(template, Model, viewBag, null);

                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine(logoImage + Environment.NewLine + template + Environment.NewLine + Model.Business + Environment.NewLine + Model.SiteAddress +
                           "" + Environment.NewLine + Model.SiteLink + Environment.NewLine + Model.PhoneNumber + Environment.NewLine + Model.OperationZoneId + Environment.NewLine + Model.FileName +
                           "" + Environment.NewLine + Model.CompanyName + Environment.NewLine + Model.ContactName + Environment.NewLine + Model.CustomerEmail +
                           "" + Environment.NewLine + Model.StaffEmail + Environment.NewLine + Model.StaffName + Environment.NewLine + Model.SenderDept + Environment.NewLine +
                           "" + Environment.NewLine + EmailBody);
                        writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    }

                    var EmailSubject = Model.CompanyName + " Rate Cards - FRAYTE GLOBAL";

                    FrayteEmail.SendMail(Model.CustomerEmail, "", EmailSubject, EmailBody, filepath, logoImage, OperationZoneId);
                    result.Status = true;
                }
                else
                {
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
            }
            return result;
        }

        public FrayteQuotationResult SendCustomerQuoteMail(FrayteQuotationEmailMail quotationEmailDetail, FrayteManifestName Name, FrayteManifestName ratecard)
        {
            FrayteQuotationResult result = new FrayteQuotationResult();
            try
            {
                var operationzone = UtilityRepository.GetOperationZone();
                FrayteQuoteEmail data = new FrayteQuoteEmail();
                DynamicViewBag viewBag = new DynamicViewBag();

                string logoImage = string.Empty;
                string template = string.Empty;
                string Display = string.Empty;
                var UserType = UtilityRepository.GetUserType(quotationEmailDetail.LoginUserId);
                if (UserType == FrayteUserType.SPECIAL)
                {
                    var company = UtilityRepository.GetCustomerCompnay(quotationEmailDetail.LoginUserId);
                    logoImage = AppSettings.EmailServicePath + "/EmailTeamplate/" + quotationEmailDetail.LoginUserId + "/Images/" + company.LogoFileName;
                    template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + quotationEmailDetail.LoginUserId + "/Quotation.cshtml");

                    data.SenderEmail = company.OperationStaffEmail;
                    data.SenderName = company.OperationStaff;
                    data.DeptName = company.UserPosition;
                    data.SiteAddress = company.SiteAddress;
                    data.PhoneNo = company.OperationStaffPhone;
                    Display = company.CompanyName + " - Quotation (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")";
                    viewBag.AddValue("CustomerCompany", company.KindRegards);
                }
                else
                {
                    logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
                    template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Quotation.cshtml");
                    data.SenderEmail = quotationEmailDetail.SenderEMail;
                    data.SenderName = quotationEmailDetail.SenderName;
                    data.DeptName = quotationEmailDetail.SenderDept == "OperationStaff" ? "Operation Staff" : "Sales Representative";
                    Display = "FRAYTE - Quotation (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")";
                    if (quotationEmailDetail.QuotationDetail.OperationZoneId == 1)
                    {
                        data.SiteAddress = AppSettings.TrackingUrl;
                        data.PhoneNo = "(+852) 2148 4880";
                    }
                    else
                    {
                        data.SiteAddress = AppSettings.TrackingUrl;
                        data.PhoneNo = "(+44) 01792 277295";
                    }
                }

                data.CustomerName = quotationEmailDetail.Name;
                data.CompanyName = quotationEmailDetail.CompanyName;
                data.QuoteNo = Name.FileName.Replace(".pdf", "");
                data.FromCountry = quotationEmailDetail.QuotationDetail.QuotationFromAddress.Country.Name;
                data.ToCountry = quotationEmailDetail.QuotationDetail.QuotationToAddress.Country.Name;
                data.Carrier = quotationEmailDetail.QuotationDetail.LogisticCompanyDisplay + " " + quotationEmailDetail.QuotationDetail.RateTypeDisplay;
                data.Weight = Name.TotalWeight;
                data.TransitTime = Name.TransitTime;
                data.PriceValue = quotationEmailDetail.QuotationDetail.CurrenyCode;
                data.Rate = quotationEmailDetail.QuotationDetail.EstimatedTotalCost.ToString("N", new System.Globalization.CultureInfo("en-US"));
                data.AddOnRate = quotationEmailDetail.QuotationDetail.AdditionalSurcharge.ToString("N", new System.Globalization.CultureInfo("en-US"));
                data.FuelSurcharge = quotationEmailDetail.QuotationDetail.FuelSurCharge.ToString("N", new System.Globalization.CultureInfo("en-US"));
                data.FuelMonth = quotationEmailDetail.FuelMonth;
                data.TotalPrice = ((float)quotationEmailDetail.QuotationDetail.EstimatedCost + quotationEmailDetail.QuotationDetail.AdditionalSurcharge + quotationEmailDetail.QuotationDetail.FuelSurCharge).ToString("N", new System.Globalization.CultureInfo("en-US"));
                data.ValidDate = quotationEmailDetail.QuotationDetail.ValidDate.ToString("dd-MMM-yyyy");
                data.Validity = Name.OfferValidity;
                data.OperationZoneId = quotationEmailDetail.QuotationDetail.OperationZoneId;
                data.FuelPercent = quotationEmailDetail.QuotationDetail.FuelPercent.HasValue ? quotationEmailDetail.QuotationDetail.FuelPercent.Value : 0.00m;
                data.LogisticType = quotationEmailDetail.QuotationDetail.LogisticCompanyDisplay;

                viewBag.AddValue("ImageHeader", "FrayteLogo");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, data, viewBag, null);
                var EmailSubject = "Quotation from " + quotationEmailDetail.QuotationDetail.QuotationFromAddress.Country.Name + " to " + quotationEmailDetail.QuotationDetail.QuotationToAddress.Country.Name + " - " + quotationEmailDetail.CompanyName + " - " + Name.FileName.Replace(".pdf", "");
                var To = quotationEmailDetail.EmailTo;

                string CC = string.Empty;
                string BCC = string.Empty;

                CC += quotationEmailDetail.SenderEMail + ";";
                CC += quotationEmailDetail.SenderEMail == null ? "" : quotationEmailDetail.SenderEMail;
                BCC += quotationEmailDetail.EmailBcc == null ? "" : quotationEmailDetail.EmailBcc;

                string documentFolder = HttpContext.Current.Server.MapPath("~/ReportFiles/");
                string ratecardfolder = HttpContext.Current.Server.MapPath("~/ReportFiles/CustomerRateCard/");
                string Attachmentpath = string.Empty;

                if (File.Exists(documentFolder + Name.FileName))
                {
                    Attachmentpath = documentFolder + Name.FileName;
                    if (quotationEmailDetail.IsRateSend)
                    {
                        Attachmentpath += ";" + ratecardfolder + quotationEmailDetail.QuotationDetail.CustomerId + "\\" + ratecard.FileName;
                    }
                    if (quotationEmailDetail.QuotationDetail.OperationZoneId == 1)
                    {
                        if (quotationEmailDetail.QuotationDetail.LogisticCompany == FrayteLogisticServiceType.DHL)
                        {
                            Attachmentpath += ";" + HttpContext.Current.Server.MapPath("~/ReportFiles/QuoteAttachment/DHL Additional Charges 2018.pdf");
                        }
                        else if (quotationEmailDetail.QuotationDetail.LogisticCompany == FrayteLogisticServiceType.TNT)
                        {
                            Attachmentpath += ";" + HttpContext.Current.Server.MapPath("~/ReportFiles/QuoteAttachment/TNT Additional Charges 2018.pdf");
                        }
                        else if (quotationEmailDetail.QuotationDetail.LogisticCompany == FrayteLogisticServiceType.UPS)
                        {
                            Attachmentpath += ";" + HttpContext.Current.Server.MapPath("~/ReportFiles/QuoteAttachment/UPS Additional Charges 2018.pdf");
                        }
                    }
                }

                if (!string.IsNullOrEmpty(AppSettings.TOCC))
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("View Quotation email TOCC " + To));
                    FrayteEmail.SendMail(AppSettings.TOCC, "", "", "", Display, EmailSubject, EmailBody, Attachmentpath, logoImage, quotationEmailDetail.LoginUserId);
                    result.Status = true;
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(EmailBody));
                }
                else
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("View Quotation email " + To));
                    FrayteEmail.SendMail(To, CC, BCC, quotationEmailDetail.SenderEMail, Display, EmailSubject, EmailBody, Attachmentpath, logoImage, quotationEmailDetail.LoginUserId);
                    result.Status = true;
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(EmailBody));
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Status = false;
                return result;
            }
        }

        public FrayteQuotationResult SendQuotationMail(FrayteQuotationEmailMail quotationEmailDetail, FrayteManifestName Name, FrayteManifestName ratecard)
        {
            FrayteQuotationResult result = new FrayteQuotationResult();
            try
            {
                var operationzone = UtilityRepository.GetOperationZone();
                FrayteQuoteEmail data = new FrayteQuoteEmail();
                DynamicViewBag viewBag = new DynamicViewBag();

                string logoImage = string.Empty;
                string template = string.Empty;
                string Display = string.Empty;
                var UserType = UtilityRepository.GetUserType(quotationEmailDetail.LoginUserId);
                if (UserType == FrayteUserType.SPECIAL)
                {
                    var company = UtilityRepository.GetCustomerCompnay(quotationEmailDetail.LoginUserId);
                    logoImage = AppSettings.EmailServicePath + "/EmailTeamplate/" + quotationEmailDetail.LoginUserId + "/Images/" + company.LogoFileName;
                    template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + quotationEmailDetail.LoginUserId + "/Quotation.cshtml");

                    data.SenderEmail = company.OperationStaffEmail;
                    data.SenderName = company.OperationStaff;
                    data.DeptName = company.UserPosition;
                    data.SiteAddress = company.SiteAddress;
                    data.PhoneNo = company.OperationStaffPhone;
                    Display = company.CompanyName + " - Quotation (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")";
                    viewBag.AddValue("CustomerCompany", company.KindRegards);
                }
                else
                {
                    logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
                    template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Quotation.cshtml");

                    data.SenderEmail = quotationEmailDetail.SenderEMail;
                    data.SenderName = quotationEmailDetail.SenderName;
                    data.DeptName = quotationEmailDetail.SenderDept == "OperationStaff" ? "Operation Staff" : "Sales Representative";
                    Display = "FRAYTE - Quotation (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")";
                    if (quotationEmailDetail.QuotationDetail.OperationZoneId == 1)
                    {
                        data.SiteAddress = AppSettings.TrackingUrl;
                        data.PhoneNo = "(+852) 2148 4880";
                    }
                    else
                    {
                        data.SiteAddress = AppSettings.TrackingUrl;
                        data.PhoneNo = "(+44) 01792 277295";
                    }
                }

                data.CustomerName = quotationEmailDetail.Name;
                data.CompanyName = quotationEmailDetail.CompanyName;
                data.QuoteNo = Name.FileName.Replace(".pdf", "");
                data.FromCountry = quotationEmailDetail.QuotationDetail.QuotationFromAddress.Country.Name;
                data.ToCountry = quotationEmailDetail.QuotationDetail.QuotationToAddress.Country.Name;
                data.Carrier = quotationEmailDetail.QuotationDetail.LogisticCompanyDisplay + " " + quotationEmailDetail.QuotationDetail.RateTypeDisplay;
                data.Weight = Name.TotalWeight;
                data.TransitTime = Name.TransitTime;
                data.PriceValue = quotationEmailDetail.QuotationDetail.CurrenyCode;
                data.Rate = quotationEmailDetail.QuotationDetail.EstimatedTotalCost.ToString("N", new System.Globalization.CultureInfo("en-US"));
                data.AddOnRate = quotationEmailDetail.QuotationDetail.QuotationRateCard.AdditionalSurcharge.ToString("N", new System.Globalization.CultureInfo("en-US"));
                data.FuelSurcharge = quotationEmailDetail.QuotationDetail.FuelSurCharge.ToString("N", new System.Globalization.CultureInfo("en-US"));
                data.FuelMonth = quotationEmailDetail.FuelMonth;
                data.TotalPrice = ((float)quotationEmailDetail.QuotationDetail.EstimatedCost + quotationEmailDetail.QuotationDetail.QuotationRateCard.AdditionalSurcharge + quotationEmailDetail.QuotationDetail.FuelSurCharge).ToString("N", new System.Globalization.CultureInfo("en-US"));
                data.ValidDate = quotationEmailDetail.QuotationDetail.ValidDate.ToString("dd-MMM-yyyy");
                data.Validity = Name.OfferValidity;
                data.OperationZoneId = quotationEmailDetail.QuotationDetail.OperationZoneId;
                data.FuelPercent = quotationEmailDetail.QuotationDetail.FuelPercent.HasValue ? quotationEmailDetail.QuotationDetail.FuelPercent.Value : 0.00m;
                data.LogisticType = quotationEmailDetail.QuotationDetail.LogisticCompanyDisplay;
                viewBag.AddValue("ImageHeader", "FrayteLogo");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, data, viewBag, null);
                var EmailSubject = "Quotation from " + quotationEmailDetail.QuotationDetail.QuotationFromAddress.Country.Name + " to " + quotationEmailDetail.QuotationDetail.QuotationToAddress.Country.Name + " - " + quotationEmailDetail.CompanyName + " - " + Name.FileName.Replace(".pdf", "");
                var To = quotationEmailDetail.EmailTo;

                string CC = string.Empty;
                string BCC = string.Empty;
                CC += quotationEmailDetail.SenderEMail + ";";
                CC += quotationEmailDetail.EmailCc == null ? "" : quotationEmailDetail.EmailCc;
                BCC += quotationEmailDetail.EmailBcc == null ? "" : quotationEmailDetail.EmailBcc;

                string documentFolder = HttpContext.Current.Server.MapPath("~/ReportFiles/");
                string ratecardfolder = HttpContext.Current.Server.MapPath("~/ReportFiles/CustomerRateCard/");
                string Attachmentpath = "";

                if (File.Exists(documentFolder + Name.FileName))
                {
                    Attachmentpath = documentFolder + Name.FileName;
                    if (quotationEmailDetail.IsRateSend)
                    {
                        Attachmentpath += ";" + ratecardfolder + quotationEmailDetail.QuotationDetail.CustomerId + "\\" + ratecard.FileName;
                    }
                    if (quotationEmailDetail.QuotationDetail.OperationZoneId == 1)
                    {
                        if (quotationEmailDetail.QuotationDetail.LogisticCompany == FrayteLogisticServiceType.DHL)
                        {
                            Attachmentpath += ";" + HttpContext.Current.Server.MapPath("~/ReportFiles/QuoteAttachment/DHL Additional Charges 2018.pdf");
                        }
                        else if (quotationEmailDetail.QuotationDetail.LogisticCompany == FrayteLogisticServiceType.TNT)
                        {
                            Attachmentpath += ";" + HttpContext.Current.Server.MapPath("~/ReportFiles/QuoteAttachment/TNT Additional Charges 2018.pdf");
                        }
                        else if (quotationEmailDetail.QuotationDetail.LogisticCompany == FrayteLogisticServiceType.UPS)
                        {
                            Attachmentpath += ";" + HttpContext.Current.Server.MapPath("~/ReportFiles/QuoteAttachment/UPS Additional Charges 2018.pdf");
                        }
                    }
                }


                if (!string.IsNullOrEmpty(AppSettings.TOCC))
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Quotation Email TOCC " + To));
                    FrayteEmail.SendMail(AppSettings.TOCC, "", "", "", Display, EmailSubject, EmailBody, Attachmentpath, logoImage, quotationEmailDetail.LoginUserId);
                    result.Status = true;
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(EmailBody));
                }
                else
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Quotation Email " + To));
                    FrayteEmail.SendMail(To, CC, BCC, quotationEmailDetail.SenderEMail, Display, EmailSubject, EmailBody, Attachmentpath, logoImage, quotationEmailDetail.LoginUserId);
                    result.Status = true;
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(EmailBody));
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Status = false;
                return result;
            }
        }

        #endregion

        #region -- Send Identity Email --

        public FrayteResult sendForgetPasswordEmail(int userId, string token)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var OperationZone = UtilityRepository.GetOperationZone();
                var user = dbContext.Users.Find(userId);
                var userDetail = (from u in dbContext.Users
                                  join ur in dbContext.UserRoles on u.UserId equals ur.UserId
                                  join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                  join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                  join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                  where u.UserId == userId
                                  select new
                                  {
                                      RoleId = ur.RoleId,
                                      CustomerName = u.ContactName,
                                      CustomerEmail = u.Email,
                                      CompanyName = u.CompanyName,
                                      UserName = u1.ContactName,
                                      UserPosition = u1.Position,
                                      UserEmail = u1.Email,
                                      UserPhone = u1.TelephoneNo,
                                      UserSkype = u1.Skype,
                                      UserFax = u1.FaxNumber
                                  }).FirstOrDefault();

                if (user != null && !string.IsNullOrEmpty(token))
                {
                    int id = 0;
                    var detail = (from r in dbContext.Users
                                  join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                                  where r.UserId == userId
                                  select new
                                  {
                                      UserId = r.UserId,
                                      Email = r.Email,
                                      TimeZone = r.TimezoneId,
                                      UserEmail = r.UserEmail,
                                      RoleId = ur.RoleId
                                  }).FirstOrDefault();
                    if (detail.RoleId == (int)FrayteUserRole.UserCustomer)
                    {
                        var UserCustomerDetail = dbContext.UserCustomers.Where(p => p.CustomerId == userId).FirstOrDefault();
                        if (UserCustomerDetail != null)
                        {
                            id = UserCustomerDetail.UserId;
                        }
                    }

                    //Get Customer Name and Customer User Detail
                    var customerDetail = (from u in dbContext.Users
                                          where u.UserId == (detail.RoleId == (int)FrayteUserRole.UserCustomer ? id : userId)
                                          select new
                                          {
                                              UsreId = u.UserId,
                                              CustomerName = u.ContactName,
                                              CustomerEmail = u.UserEmail,
                                              CompanyName = u.CompanyName
                                          }).FirstOrDefault();

                    RecoverEmailModel model = new RecoverEmailModel();
                    string logoImage = string.Empty;
                    model.Name = user.ContactName;
                    var userCustomerDetail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == customerDetail.UsreId).FirstOrDefault();
                    string EmailSubject = string.Empty;
                    if (userCustomerDetail != null)
                    {
                        EmailSubject = "Request for Login Credentials Recovery for MEX logistic Ltd.";
                        logoImage = AppSettings.EmailServicePath + "/EmailTeamplate/" + customerDetail.UsreId + "/Images/" + userCustomerDetail.LogoFileName;
                        model.ImageHeader = userCustomerDetail.LogoFileName;
                        model.SalesEmail = userCustomerDetail.SalesStaffEmail;
                        model.PhoneNumber = userCustomerDetail.OperationStaffPhone;
                        model.SiteAddress = userCustomerDetail.SiteAddress;
                        model.SiteLink = userCustomerDetail.KindRegards;
                        model.RecoveryLink = string.Format(AppSettings.HKUrl + "reset-password/{0}/?{1}", userId, token);
                    }
                    else
                    {
                        EmailSubject = "Request for Login Credentials Recovery for Frayte Management System";
                        logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
                        model.ImageHeader = "FrayteLogo";
                        if (OperationZone.OperationZoneId == 1)
                        {
                            model.SalesEmail = "system@frayte.com";
                            model.PhoneNumber = "(+852) 2148 4880";
                            model.SiteAddress = AppSettings.TrackingUrl;
                            model.SiteLink = "FRAYTE GLOBAL";
                            model.RecoveryLink = string.Format(AppSettings.HKUrl + "reset-password/{0}/?{1}", userId, token);
                        }
                        if (OperationZone.OperationZoneId == 2)
                        {
                            model.SalesEmail = "system@frayte.co.uk";
                            model.PhoneNumber = "(+44) 01792 277295";
                            model.SiteAddress = AppSettings.TrackingUrl;
                            model.SiteLink = "FRAYTE GLOBAL";
                            model.RecoveryLink = string.Format(AppSettings.UKUrl + "reset-password/{0}/?{1}", userId, token);
                        }
                    }

                    string template1 = string.Empty;
                    if (userCustomerDetail != null)
                    {
                        template1 = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + customerDetail.UsreId + "/ForgetPassword.cshtml");
                    }
                    else
                    {
                        template1 = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/ForgetPassword.cshtml");
                    }


                    var EmailBody = RazorEngine.Engine.Razor.RunCompile(template1, "ForgetPassword", null, model);

                    if (string.IsNullOrEmpty(user.UserEmail))
                    {
                        user.UserEmail = user.Email;
                    }

                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("for password email to" + user.UserName)));

                    FrayteEmail.SendMail_ForgetPassword(user.UserEmail, "", EmailSubject, EmailBody, "", logoImage, customerDetail.UsreId);
                    result.Status = true;
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception(EmailBody)));
                }
                else
                {
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                result.Status = false;
            }

            return result;
        }

        #endregion

        #region -- Email Communication --

        internal bool SendEmailCommunication(InvoiceEmailCommunication emailCommunication)
        {
            try
            {
                DynamicViewBag viewBag = new DynamicViewBag();
                viewBag.AddValue("EmailBody", emailCommunication.EmailBody);
                viewBag.AddValue("ImageHeader", "FrayteLogo");

                if (UtilityRepository.GetOperationZone().OperationZoneId == 1)
                {
                    viewBag.AddValue("SalesEmail", "sales@frayte.com");
                    viewBag.AddValue("PhoneNumber", "(+852) 2148 4880");
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                    viewBag.AddValue("SiteLink", "FRAYTE GLOBAL");

                }
                if (UtilityRepository.GetOperationZone().OperationZoneId == 2)
                {
                    viewBag.AddValue("SalesEmail", "sales@frayte.co.uk");
                    viewBag.AddValue("PhoneNumber", "(+44) 01792 277295");
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                    viewBag.AddValue("SiteLink", "FRAYTE GLOBAL");
                }
                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/eCommerceInvoiceCommunication.cshtml");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, "", viewBag, null);
                var EmailSubject = emailCommunication.EmailSubject;

                var CC = emailCommunication.CC == null ? "" : emailCommunication.CC;
                var BCC = emailCommunication.BCC == null ? "" : emailCommunication.BCC;

                FrayteEmail.SendMail(emailCommunication.SentTo, CC, EmailSubject, EmailBody, "", "");

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        #endregion

        #region -- View Shipment --

        public FrayteResult SendManualTrackingEmail(int eCommerceShipmentId, eCommerceManualTracking eCommerceTracking)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var detail = (from r in dbContext.eCommerceShipments
                              join da in dbContext.eCommerceShipmentAddresses on r.ToAddressId equals da.eCommerceShipmentAddressId
                              join u in dbContext.Users on r.CustomerId equals u.UserId
                              join ua1 in dbContext.UserAdditionals on u.UserId equals ua1.UserId
                              join u1 in dbContext.Users on ua1.OperationUserId equals u1.UserId
                              join c in dbContext.Countries on da.CountryId equals c.CountryId
                              join tz in dbContext.Timezones on c.TimeZoneId equals tz.TimezoneId
                              where r.eCommerceShipmentId == eCommerceShipmentId
                              select new
                              {
                                  CustomerName = u.ContactName,
                                  CustomerEmail = u.Email,
                                  CompanyName = u.CompanyName,
                                  UserName = u1.ContactName,
                                  UserPosition = u1.Position,
                                  UserEmail = u1.Email,
                                  UserPhone = u1.TelephoneNo,
                                  UserSkype = u1.Skype,
                                  UserFax = u1.FaxNumber,
                                  ReceiverEmail = da.Email,
                                  TimeZoneDetail = new TimeZoneModal
                                  {
                                      Name = tz.Name,
                                      Offset = tz.Offset,
                                      OffsetShort = tz.OffsetShort,
                                      TimezoneId = tz.TimezoneId
                                  }
                              }
                        ).FirstOrDefault();


                var operationzone = UtilityRepository.GetOperationZone();
                string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
                DynamicViewBag viewBag = new DynamicViewBag();
                if (detail.TimeZoneDetail != null)
                {
                    viewBag.AddValue("CreatedOn", UtilityRepository.GetTimeZoneCurrentDateTime(detail.TimeZoneDetail.Name).ToString("dd-MMM-yyyy hh:mm")); ;
                    viewBag.AddValue("TimeZone", detail.TimeZoneDetail.OffsetShort);
                }
                else
                {
                    viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                }


                List<string> trackingNos = new List<string>();
                trackingNos.Add(eCommerceTracking.TrackingNumber);
                //viewBag.AddValue("FrayteNumber", MTM.FrayteNo);
                viewBag.AddValue("TrackingDescription", eCommerceTracking.TrackingDescription);
                viewBag.AddValue("TrackingNo", trackingNos);
                viewBag.AddValue("CustomerName", detail.CustomerName);
                viewBag.AddValue("UserEmail", detail.UserEmail);
                viewBag.AddValue("UserPhone", detail.UserPhone);
                viewBag.AddValue("ImageHeader", "FrayteLogo");

                if (operationzone.OperationZoneId == 1)
                {
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                }
                else
                {
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                }
                viewBag.AddValue("TrackingURL", AppSettings.TrackingUrl);
                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/eCommerceManifestTracking.cshtml");
                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, "", viewBag, null);
                string EmailSubject = "Shipment Tracking";
                var To = detail.ReceiverEmail;
                var CC = detail.UserEmail;
                string Status = "Confirmation";

                //Send mail to Customer
                SendMail_New(To, CC, "FRAYTE (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, "", Status);
                //FrayteEmail.SendMail(To, CC, EmailSubject, EmailBody, logoImage);
                result.Status = true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                result.Status = false;
            }

            return result;
        }

        #endregion

        #region -- Aftership Tracking Email --

        public FrayteResult SendAftershipTrackingEmail(FrayteAftershipmentTrackingEmail obj, string emailType, FrayteTrackingConfiguration trackingConfiguration)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var ShipmentDetail = new DirectShipmentRepository().GetDirectBookingDetail(dbContext.DirectShipments.Where(p => p.FrayteNumber == obj.msg.title).FirstOrDefault().DirectShipmentId, "");

                var customerAdditional = (from r in dbContext.UserAdditionals
                                          join u in dbContext.Users on r.OperationUserId equals u.UserId
                                          where r.UserId == ShipmentDetail.CustomerId
                                          select new
                                          {
                                              UserName = u.ContactName,
                                              Email = u.Email,
                                              MobileNumber = u.MobileNo,
                                              TelePhoneNumber = u.TelephoneNo,
                                              UserPosition = u.Position
                                          }
                                          ).FirstOrDefault();

                DynamicViewBag viewBag = new DynamicViewBag();

                bool SendEmail = false;

                string Courier = string.Empty;
                if (obj.msg.slug == FrayteCourierSlugs.DHL)
                {
                    Courier = FrayteCourierSlugsDisplay.DHLExpress;
                }
                else if (obj.msg.slug == FrayteCourierSlugs.TNT)
                {
                    Courier = FrayteCourierSlugsDisplay.TNT;
                }
                else if (obj.msg.slug == FrayteCourierSlugs.UPS)
                {
                    Courier = FrayteCourierSlugsDisplay.UPS;
                }
                else if (obj.msg.slug == FrayteCourierSlugs.Yodel)
                {
                    Courier = FrayteCourierSlugsDisplay.Yodel;
                }
                else if (obj.msg.slug == FrayteCourierSlugs.Hermese)
                {
                    Courier = FrayteCourierSlugsDisplay.Hermese;
                }
                else if (obj.msg.slug == FrayteCourierSlugs.UkMail)
                {
                    Courier = FrayteCourierSlugsDisplay.UkMail;
                }

                viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy"));
                viewBag.AddValue("CourierContactNo", customerAdditional.TelePhoneNumber);
                viewBag.AddValue("UserPhoneNumber", customerAdditional.MobileNumber);
                viewBag.AddValue("UserName", customerAdditional.UserName);
                viewBag.AddValue("UserPosition", customerAdditional.UserPosition);
                viewBag.AddValue("UserEmail", customerAdditional.Email);

                StringBuilder sb = new StringBuilder();
                if (obj.msg.checkpoints[0].city != null && !string.IsNullOrEmpty(obj.msg.checkpoints[0].city.ToString()))
                    sb.Append(obj.msg.checkpoints[0].city);
                if (obj.msg.checkpoints[0].state != null && !string.IsNullOrEmpty(obj.msg.checkpoints[0].state.ToString()))
                    sb.Append(" " + obj.msg.checkpoints[0].state);
                if (obj.msg.checkpoints[0].zip != null && !string.IsNullOrEmpty(obj.msg.checkpoints[0].zip.ToString()))
                    sb.Append(" - " + obj.msg.checkpoints[0].zip);
                if (obj.msg.checkpoints[0].country_name != null && !string.IsNullOrEmpty("" + obj.msg.checkpoints[0].country_name.ToString()))
                    sb.Append(" " + obj.msg.checkpoints[0].country_name);


                viewBag.AddValue("LAST_UPDATE_TIME", obj.msg.checkpoints[0].checkpoint_time.ToString("dd-MMM-yyyy hh:mm"));
                viewBag.AddValue("SiteLink", "FRAYTE GLOBAL");
                viewBag.AddValue("Trackingnumber", obj.msg.tracking_number);
                viewBag.AddValue("FrayteNumber", obj.msg.title);
                viewBag.AddValue("Courier", Courier);
                viewBag.AddValue("Title", obj.msg.title);

                if (ShipmentDetail.OpearionZoneId == 1)
                {
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                    viewBag.AddValue("TrackingURL", AppSettings.TrackingUrlHKG + "/tracking-detail/" + obj.msg.slug + "/" + obj.msg.tracking_number + "/");
                }
                else
                {
                    viewBag.AddValue("TrackingURL", AppSettings.TrackingUrl + "/tracking-detail/" + obj.msg.slug + "/" + obj.msg.tracking_number + "/");
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                }

                viewBag.AddValue("DetailStatus", obj.msg.checkpoints[0].message);
                viewBag.AddValue("Location", sb.ToString());
                viewBag.AddValue("ImageHeader", "FrayteLogo");
                viewBag.AddValue("SignBy", obj.msg.signed_by);

                string fileName = string.Empty;
                string EmailSubject = string.Empty;

                List<FrayteEmailModel> list = new List<FrayteEmailModel>();

                if (emailType == FrayteAftershipStatusTagString.InfoReceived && (trackingConfiguration.InfoReceivedEmails != null && trackingConfiguration.InfoReceivedEmails.Count > 0))
                {
                    SendEmail = true;
                    fileName = "InfoReceived.cshtml";
                    EmailSubject = Courier + " - " + obj.msg.title + " - " + FrayteAftershipStatus.InfoReceived;

                    list = trackingConfiguration.InfoReceivedEmails;
                }
                else if (emailType == FrayteAftershipStatusTagString.InTransit && (trackingConfiguration.InTransitEmails != null && trackingConfiguration.InTransitEmails.Count > 0))
                {
                    SendEmail = true;
                    fileName = "Intransit.cshtml";
                    EmailSubject = Courier + " - " + obj.msg.title + " - " + FrayteAftershipStatus.InTransit;
                    list = trackingConfiguration.InTransitEmails;
                }
                else if (emailType == FrayteAftershipStatusTagString.Delivered && trackingConfiguration.DeliveredEmails != null && trackingConfiguration.DeliveredEmails.Count > 0)
                {
                    SendEmail = true;
                    fileName = "Delivered.cshtml";
                    EmailSubject = Courier + " - " + obj.msg.title + " - " + FrayteAftershipStatus.Delivered;
                    list = trackingConfiguration.DeliveredEmails;
                }
                else if (emailType == FrayteAftershipStatusTagString.OutForDelivery && trackingConfiguration.OutForDeliveryEmails != null && trackingConfiguration.OutForDeliveryEmails.Count > 0)
                {
                    SendEmail = true;
                    fileName = "OutForDelivery.cshtml";
                    EmailSubject = Courier + " - " + obj.msg.title + " - " + FrayteAftershipStatus.OutForDelivery;
                    list = trackingConfiguration.OutForDeliveryEmails;
                }

                else if (emailType == FrayteAftershipStatusTagString.Exception && trackingConfiguration.ExceptionEmails != null && trackingConfiguration.ExceptionEmails.Count > 0)
                {
                    SendEmail = true;
                    fileName = "Exception.cshtml";
                    EmailSubject = Courier + " - " + obj.msg.title + " - " + " has a delivery exception";
                    list = trackingConfiguration.ExceptionEmails;
                }
                else if (emailType == FrayteAftershipStatusTagString.AttemptFail && trackingConfiguration.AttemptFailEmails != null && trackingConfiguration.AttemptFailEmails.Count > 0)
                {
                    SendEmail = true;
                    fileName = "FailedAttepmt.cshtml";
                    EmailSubject = Courier + " - " + obj.msg.title + " - " + " experienced failed delivery attempt";
                    list = trackingConfiguration.AttemptFailEmails;
                }

                var TO = string.Empty;
                var CC = string.Empty;

                if (SendEmail)
                {

                    if (list.Count > 0)
                    {
                        viewBag.AddValue("CustomerName", "User");

                        TO = String.Join(";", list.Select(p => p.Email).ToArray());

                        string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + fileName);
                        var templateService = new TemplateService();
                        var EmailBody = templateService.Parse(template, "", viewBag, null);

                        SendMail(TO, CC, EmailSubject, EmailBody, "");
                        // FrayteEmail.SendMail(TO, CC, EmailSubject, EmailBody, "", "");
                        result.Status = true;
                    }


                    if (ShipmentDetail != null)
                    {
                        if (ShipmentDetail.ShipTo.IsMailSend)
                        {
                            viewBag.AddValue("CustomerName", ShipmentDetail.ShipTo.FirstName + " " + ShipmentDetail.ShipTo.LastName);
                            TO = string.Empty;
                            TO = ShipmentDetail.ShipTo.Email;
                            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + fileName);
                            var templateService = new TemplateService();
                            var EmailBody = templateService.Parse(template, "", viewBag, null);

                            FrayteEmail.SendMail(TO, CC, EmailSubject, EmailBody, "", "");
                            result.Status = true;
                        }

                        if (ShipmentDetail.ShipFrom.IsMailSend)
                        {
                            viewBag.AddValue("CustomerName", ShipmentDetail.ShipFrom.FirstName + " " + ShipmentDetail.ShipFrom.LastName);
                            TO = string.Empty;
                            TO = ShipmentDetail.ShipFrom.Email;
                            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + fileName);
                            var templateService = new TemplateService();
                            var EmailBody = templateService.Parse(template, "", viewBag, null);
                            FrayteEmail.SendMail(TO, CC, EmailSubject, EmailBody, "", "");
                            result.Status = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return result;
        }

        public FrayteResult SendAftershipTrackingEmail(FrayteAftershipmentTrackingEmail obj, string emailType)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var ShipmentDetail = new DirectShipmentRepository().GetDirectBookingDetail(dbContext.DirectShipments.Where(p => p.FrayteNumber == obj.msg.title).FirstOrDefault().DirectShipmentId, "");

                var customerAdditional = (from r in dbContext.UserAdditionals
                                          join u in dbContext.Users on r.OperationUserId equals u.UserId
                                          where r.UserId == ShipmentDetail.CustomerId
                                          select new
                                          {
                                              UserName = u.ContactName,
                                              Email = u.Email,
                                              MobileNumber = u.MobileNo,
                                              TelePhoneNumber = u.TelephoneNo,
                                              UserPosition = u.Position
                                          }
                                          ).FirstOrDefault();

                DynamicViewBag viewBag = new DynamicViewBag();

                bool SendEmail = false;

                string Courier = string.Empty;
                if (obj.msg.slug == FrayteCourierSlugs.DHL)
                {
                    Courier = FrayteCourierSlugsDisplay.DHLExpress;
                }
                else if (obj.msg.slug == FrayteCourierSlugs.TNT)
                {
                    Courier = FrayteCourierSlugsDisplay.TNT;
                }
                else if (obj.msg.slug == FrayteCourierSlugs.UPS)
                {
                    Courier = FrayteCourierSlugsDisplay.UPS;
                }
                else if (obj.msg.slug == FrayteCourierSlugs.Yodel)
                {
                    Courier = FrayteCourierSlugsDisplay.Yodel;
                }
                else if (obj.msg.slug == FrayteCourierSlugs.Hermese)
                {
                    Courier = FrayteCourierSlugsDisplay.Hermese;
                }
                else if (obj.msg.slug == FrayteCourierSlugs.UkMail)
                {
                    Courier = FrayteCourierSlugsDisplay.UkMail;
                }

                viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy"));
                viewBag.AddValue("CourierContactNo", customerAdditional.TelePhoneNumber);
                viewBag.AddValue("UserPhoneNumber", customerAdditional.MobileNumber);
                viewBag.AddValue("UserName", customerAdditional.UserName);
                viewBag.AddValue("UserPosition", customerAdditional.UserPosition);
                viewBag.AddValue("UserEmail", customerAdditional.Email);

                StringBuilder sb = new StringBuilder();
                if (obj.msg.checkpoints[0].city != null && !string.IsNullOrEmpty(obj.msg.checkpoints[0].city.ToString()))
                    sb.Append(obj.msg.checkpoints[0].city);
                if (obj.msg.checkpoints[0].state != null && !string.IsNullOrEmpty(obj.msg.checkpoints[0].state.ToString()))
                    sb.Append(" " + obj.msg.checkpoints[0].state);
                if (obj.msg.checkpoints[0].zip != null && !string.IsNullOrEmpty(obj.msg.checkpoints[0].zip.ToString()))
                    sb.Append(" - " + obj.msg.checkpoints[0].zip);
                if (obj.msg.checkpoints[0].country_name != null && !string.IsNullOrEmpty("" + obj.msg.checkpoints[0].country_name.ToString()))
                    sb.Append(" " + obj.msg.checkpoints[0].country_name);


                viewBag.AddValue("LAST_UPDATE_TIME", obj.msg.checkpoints[0].checkpoint_time.ToString("dd-MMM-yyyy hh:mm"));
                viewBag.AddValue("SiteLink", "FRAYTE GLOBAL");
                viewBag.AddValue("Trackingnumber", obj.msg.tracking_number);
                viewBag.AddValue("FrayteNumber", obj.msg.title);
                viewBag.AddValue("Courier", Courier);
                viewBag.AddValue("Title", obj.msg.title);

                if (ShipmentDetail.OpearionZoneId == 1)
                {
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                    viewBag.AddValue("TrackingURL", AppSettings.TrackingUrlHKG + "/tracking-detail/" + obj.msg.slug + "/" + obj.msg.tracking_number + "/");
                }
                else
                {
                    viewBag.AddValue("TrackingURL", AppSettings.TrackingUrl + "/tracking-detail/" + obj.msg.slug + "/" + obj.msg.tracking_number + "/");
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                }

                viewBag.AddValue("DetailStatus", obj.msg.checkpoints[0].message);
                viewBag.AddValue("Location", sb.ToString());
                viewBag.AddValue("ImageHeader", "FrayteLogo");
                viewBag.AddValue("SignBy", obj.msg.signed_by);

                string fileName = string.Empty;
                string EmailSubject = string.Empty;

                List<FrayteEmailModel> list = new List<FrayteEmailModel>();

                if (emailType == FrayteAftershipStatusTagString.InfoReceived)
                {
                    SendEmail = true;
                    fileName = "InfoReceived.cshtml";
                    EmailSubject = Courier + " - " + obj.msg.title + " - " + FrayteAftershipStatus.InfoReceived;

                }
                else if (emailType == FrayteAftershipStatusTagString.InTransit)
                {
                    SendEmail = true;
                    fileName = "Intransit.cshtml";
                    EmailSubject = Courier + " - " + obj.msg.title + " - " + FrayteAftershipStatus.InTransit;
                }
                else if (emailType == FrayteAftershipStatusTagString.Delivered)
                {
                    SendEmail = true;
                    fileName = "Delivered.cshtml";
                    EmailSubject = Courier + " - " + obj.msg.title + " - " + FrayteAftershipStatus.Delivered;

                }
                else if (emailType == FrayteAftershipStatusTagString.OutForDelivery)
                {
                    SendEmail = true;
                    fileName = "OutForDelivery.cshtml";
                    EmailSubject = Courier + " - " + obj.msg.title + " - " + FrayteAftershipStatus.OutForDelivery;

                }

                else if (emailType == FrayteAftershipStatusTagString.Exception)
                {
                    SendEmail = true;
                    fileName = "Exception.cshtml";
                    EmailSubject = Courier + " - " + obj.msg.title + " - " + " has a delivery exception";

                }
                else if (emailType == FrayteAftershipStatusTagString.AttemptFail)
                {
                    SendEmail = true;
                    fileName = "FailedAttepmt.cshtml";
                    EmailSubject = Courier + " - " + obj.msg.title + " - " + " experienced failed delivery attempt";

                }

                var TO = string.Empty;
                var CC = string.Empty;

                if (SendEmail)
                {
                    viewBag.AddValue("CustomerName", "User");
                    TO = "customer@irasyssolutions.com";

                    string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + fileName);
                    var templateService = new TemplateService();
                    var EmailBody = templateService.Parse(template, "", viewBag, null);
                    SendMail(TO, CC, EmailSubject, EmailBody, "");

                    result.Status = true;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return result;
        }

        #endregion

        #region -- Tradelane EMails --

        public void SendTradelaneBookingInformationMail(TradelaneBooking shipmentDetail, int DirectShipmentid)
        {
            try
            {
                //Get Customer Name and Customer User Detail
                var customerDetail = (from u in dbContext.Users
                                      join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                      join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                      join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                      where u.UserId == shipmentDetail.CustomerId
                                      select new
                                      {
                                          StaffName = u1.ContactName,
                                          StaffEmail = u1.Email,
                                          StaffCompany = u1.CompanyName,
                                          CustomerName = u.ContactName,
                                          CustomerEmail = u.UserEmail,
                                          CompanyName = u.CompanyName
                                      }).FirstOrDefault();

                var staffdetail = new TradelaneBookingRepository().GetAssociateStaffDetail(shipmentDetail.CustomerId, shipmentDetail.CreatedBy);

                var packages = dbContext.TradelaneShipmentDetails.Where(p => p.TradelaneShipmentId == shipmentDetail.TradelaneShipmentId && !string.IsNullOrEmpty(p.HAWB)).ToList();
                decimal TotalWeight = 0.0m;
                foreach (var obj in packages)
                {
                    TotalWeight += obj.Weight * obj.CartonValue;
                }

                string ShipmentMethod = shipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodDisplay;
                string Shipment = string.Empty;


                var FrayteNo = shipmentDetail.FrayteNumber;

                var operationzone = UtilityRepository.GetOperationZone();

                #region TrackingNo, TrackingURL


                #endregion

                DynamicViewBag viewBag = new DynamicViewBag();
                viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                viewBag.AddValue("TotalWeight", TotalWeight);
                viewBag.AddValue("ShipmentMethod", ShipmentMethod);
                viewBag.AddValue("TrackingNo", "");
                viewBag.AddValue("TrackingURL", "");
                viewBag.AddValue("TotalCartoon", shipmentDetail.Packages.Sum(p => p.CartonValue));
                viewBag.AddValue("CustomerName", customerDetail.CompanyName);
                viewBag.AddValue("CustomerEmail", customerDetail.CustomerEmail);
                viewBag.AddValue("CompanyName", customerDetail.CompanyName);
                viewBag.AddValue("UserName", staffdetail.OperationStaffName);
                viewBag.AddValue("UserPosition", staffdetail.DeptName);
                viewBag.AddValue("UserEmail", staffdetail.OperationStaffEmail);
                viewBag.AddValue("ImageHeader", "FrayteLogo");
                viewBag.AddValue("TrackButton", "TrackShipment");
                viewBag.AddValue("FrayteNumber", shipmentDetail.FrayteNumber);
                viewBag.AddValue("PickUp", "");
                viewBag.AddValue("OperationZoneId", operationzone.OperationZoneId);
                if (operationzone.OperationZoneId == 1)
                {
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                    viewBag.AddValue("UserPhone", "+852 2148 4880");
                }
                else
                {
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                    viewBag.AddValue("UserPhone", "+44 01792 678017");
                }

                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/TradelaneBookingConfirmation.cshtml");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, shipmentDetail, viewBag, null);
                string EmailSubject = string.Empty;
                if (shipmentDetail.ShipFrom.CompanyName == "" || shipmentDetail.ShipFrom.CompanyName == null)
                {
                    EmailSubject = "Booking Confirmation - (" + shipmentDetail.ShipFrom.FirstName + " " + shipmentDetail.ShipFrom.LastName + ")" + " - FR#" + shipmentDetail.FrayteNumber + " - FRAYTE GLOBAL";
                }
                else
                {
                    EmailSubject = "Booking Confirmation - (" + shipmentDetail.ShipFrom.CompanyName + ")" + " - FR#" + shipmentDetail.FrayteNumber + " - FRAYTE GLOBAL";
                }
                var To = customerDetail.CustomerEmail;
                var CC = "";
                string Status = "Confirmation";

                #region Attach Labels

                //     string Attachment = UtilityRepository.PackageLabelPath(DirectShipmentid, shipmentDetail.CustomerRateCard.CourierName, shipmentDetail.CustomerRateCard.RateType);

                #endregion

                //Send mail to Customer
                SendMail_New(To, CC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, "", Status);
                if (shipmentDetail.ShipFrom.IsMailSend)
                {
                    //Send mail to Ship From
                    if (!string.IsNullOrEmpty(shipmentDetail.ShipFrom.Email))
                    {
                        viewBag = new DynamicViewBag();
                        viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                        viewBag.AddValue("TotalWeight", TotalWeight);
                        viewBag.AddValue("ShipmentMethod", ShipmentMethod);
                        viewBag.AddValue("TrackingNo", "");
                        viewBag.AddValue("TrackingURL", "");
                        viewBag.AddValue("TotalCartoon", shipmentDetail.Packages.Sum(p => p.CartonValue));
                        viewBag.AddValue("CustomerName", shipmentDetail.ShipFrom.FirstName + " " + shipmentDetail.ShipFrom.LastName + " (Shipper)");
                        viewBag.AddValue("CustomerEmail", shipmentDetail.ShipFrom.Email);
                        viewBag.AddValue("CompanyName", shipmentDetail.ShipFrom.CompanyName);
                        viewBag.AddValue("UserName", staffdetail.OperationStaffName);
                        viewBag.AddValue("UserPosition", staffdetail.DeptName);
                        viewBag.AddValue("UserEmail", staffdetail.OperationStaffEmail);
                        viewBag.AddValue("ImageHeader", "FrayteLogo");
                        viewBag.AddValue("TrackButton", "TrackShipment");
                        viewBag.AddValue("FrayteNumber", shipmentDetail.FrayteNumber);
                        viewBag.AddValue("PickUp", "");
                        viewBag.AddValue("OperationZoneId", operationzone.OperationZoneId);
                        if (operationzone.OperationZoneId == 1)
                        {
                            viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                            viewBag.AddValue("UserPhone", "+852 2148 4880");
                        }
                        else
                        {
                            viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                            viewBag.AddValue("UserPhone", "+44 01792 678017");
                        }

                        template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/TradelaneBookingConfirmation.cshtml");

                        templateService = new TemplateService();
                        EmailBody = templateService.Parse(template, shipmentDetail, viewBag, null);
                        if (shipmentDetail.ShipFrom.CompanyName == "" || shipmentDetail.ShipFrom.CompanyName == null)
                        {
                            EmailSubject = "Booking Confirmation - (" + shipmentDetail.ShipFrom.FirstName + " " + shipmentDetail.ShipFrom.LastName + ")" + " - FR#" + shipmentDetail.FrayteNumber + " - FRAYTE GLOBAL";
                        }
                        else
                        {
                            EmailSubject = "Booking Confirmation - (" + shipmentDetail.ShipFrom.CompanyName + ")" + " - FR#" + shipmentDetail.FrayteNumber + " - FRAYTE GLOBAL";
                        }
                        To = shipmentDetail.ShipFrom.Email;
                        CC = "";

                        #region Attach Labels

                        //       string Attachment1 = UtilityRepository.PackageLabelPath(DirectShipmentid, shipmentDetail.CustomerRateCard.CourierName, shipmentDetail.CustomerRateCard.RateType);

                        #endregion

                        SendMail_New(To, CC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, "", Status);
                    }
                }

                if (shipmentDetail.ShipTo.IsMailSend)
                {
                    //Send mail to Ship To
                    if (!string.IsNullOrEmpty(shipmentDetail.ShipTo.Email))
                    {
                        viewBag = new DynamicViewBag();
                        viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                        viewBag.AddValue("TotalWeight", TotalWeight);
                        viewBag.AddValue("ShipmentMethod", ShipmentMethod);
                        viewBag.AddValue("TrackingNo", "");
                        viewBag.AddValue("TrackingURL", "");
                        viewBag.AddValue("TotalCartoon", shipmentDetail.Packages.Sum(p => p.CartonValue));
                        viewBag.AddValue("CustomerName", shipmentDetail.ShipTo.FirstName + " " + shipmentDetail.ShipTo.LastName + " (Customer)");
                        viewBag.AddValue("CustomerEmail", shipmentDetail.ShipTo.Email);
                        viewBag.AddValue("CompanyName", shipmentDetail.ShipTo.CompanyName);
                        viewBag.AddValue("UserName", staffdetail.OperationStaffName);
                        viewBag.AddValue("UserPosition", staffdetail.DeptName);
                        viewBag.AddValue("UserEmail", staffdetail.OperationStaffEmail);
                        viewBag.AddValue("ImageHeader", "FrayteLogo");
                        viewBag.AddValue("TrackButton", "TrackShipment");
                        viewBag.AddValue("FrayteNumber", shipmentDetail.FrayteNumber);
                        viewBag.AddValue("PickUp", "");
                        viewBag.AddValue("OperationZoneId", operationzone.OperationZoneId);
                        if (operationzone.OperationZoneId == 1)
                        {
                            viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                            viewBag.AddValue("UserPhone", "+852 2148 4880");
                        }
                        else
                        {
                            viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                            viewBag.AddValue("UserPhone", "+44 01792 678017");
                        }

                        template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/TradelaneBookingConfirmation.cshtml");

                        templateService = new TemplateService();
                        EmailBody = templateService.Parse(template, shipmentDetail, viewBag, null);
                        if (shipmentDetail.ShipFrom.CompanyName == "" || shipmentDetail.ShipTo.CompanyName == null)
                        {
                            EmailSubject = "Booking Confirmation - (" + shipmentDetail.ShipTo.FirstName + " " + shipmentDetail.ShipFrom.LastName + ")" + " - FR#" + shipmentDetail.FrayteNumber + " - FRAYTE GLOBAL";
                        }
                        else
                        {
                            EmailSubject = "Booking Confirmation - (" + shipmentDetail.ShipTo.CompanyName + ")" + " - FR#" + shipmentDetail.FrayteNumber + " - FRAYTE GLOBAL";
                        }
                        To = shipmentDetail.ShipTo.Email;
                        CC = "";

                        #region Attach Labels

                        //  string Attachment2 = UtilityRepository.PackageLabelPath(DirectShipmentid, shipmentDetail.CustomerRateCard.CourierName, shipmentDetail.CustomerRateCard.RateType);

                        #endregion

                        SendMail_New(To, CC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, "", Status);
                    }
                }
                if (shipmentDetail.NotifyParty.IsMailSend)
                {
                    //Send mail to Ship To
                    if (!string.IsNullOrEmpty(shipmentDetail.ShipTo.Email))
                    {
                        viewBag = new DynamicViewBag();
                        viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                        viewBag.AddValue("TotalWeight", TotalWeight);
                        viewBag.AddValue("ShipmentMethod", ShipmentMethod);
                        viewBag.AddValue("TrackingNo", "");
                        viewBag.AddValue("TrackingURL", "");
                        viewBag.AddValue("TotalCartoon", shipmentDetail.Packages.Sum(p => p.CartonValue));
                        viewBag.AddValue("CustomerName", shipmentDetail.NotifyParty.FirstName + " " + shipmentDetail.NotifyParty.LastName + " (Customer)");
                        viewBag.AddValue("CustomerEmail", shipmentDetail.NotifyParty.Email);
                        viewBag.AddValue("CompanyName", shipmentDetail.NotifyParty.CompanyName);
                        viewBag.AddValue("UserName", staffdetail.OperationStaffName);
                        viewBag.AddValue("UserPosition", staffdetail.DeptName);
                        viewBag.AddValue("UserEmail", staffdetail.OperationStaffEmail);
                        viewBag.AddValue("ImageHeader", "FrayteLogo");
                        viewBag.AddValue("TrackButton", "TrackShipment");
                        viewBag.AddValue("FrayteNumber", shipmentDetail.FrayteNumber);
                        viewBag.AddValue("PickUp", "");
                        viewBag.AddValue("OperationZoneId", operationzone.OperationZoneId);
                        if (operationzone.OperationZoneId == 1)
                        {
                            viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                            viewBag.AddValue("UserPhone", "+852 2148 4880");
                        }
                        else
                        {
                            viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                            viewBag.AddValue("UserPhone", "+44 01792 678017");
                        }

                        template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/DirectBookingConfirmation.cshtml");

                        templateService = new TemplateService();
                        EmailBody = templateService.Parse(template, shipmentDetail, viewBag, null);
                        if (shipmentDetail.ShipFrom.CompanyName == "" || shipmentDetail.ShipFrom.CompanyName == null)
                        {
                            EmailSubject = "Booking Confirmation - (" + shipmentDetail.NotifyParty.FirstName + " " + shipmentDetail.NotifyParty.LastName + ")" + " - FR#" + shipmentDetail.FrayteNumber + " - FRAYTE GLOBAL";
                        }
                        else
                        {
                            EmailSubject = "Booking Confirmation - (" + shipmentDetail.NotifyParty.CompanyName + ")" + " - FR#" + shipmentDetail.FrayteNumber + " - FRAYTE GLOBAL";
                        }
                        To = shipmentDetail.ShipTo.Email;
                        CC = "";

                        #region Attach Labels

                        //  string Attachment2 = UtilityRepository.PackageLabelPath(DirectShipmentid, shipmentDetail.CustomerRateCard.CourierName, shipmentDetail.CustomerRateCard.RateType);

                        #endregion

                        SendMail_New(To, CC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, "", Status);
                    }
                }

            }
            catch (Exception e)
            {
            }
        }

        public void SendTradelaneBookingCustomerConfirmationMail(TradelaneBooking shipmentDetail, int DirectShipmentid)
        {
            try
            {
                //Get Customer Name and Customer User Detail
                var customerDetail = (from u in dbContext.Users
                                      join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                      join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                      join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                      where u.UserId == shipmentDetail.CustomerId
                                      select new
                                      {
                                          CustomerName = u.ContactName,
                                          CustomerEmail = u.Email,
                                          CompanyName = u.CompanyName
                                      }).FirstOrDefault();

                var staffdetail = new TradelaneBookingRepository().GetAssociateStaffDetail(shipmentDetail.CustomerId, shipmentDetail.CreatedBy);

                var packages = dbContext.TradelaneShipmentDetails.Where(p => p.TradelaneShipmentId == shipmentDetail.TradelaneShipmentId && !string.IsNullOrEmpty(p.HAWB)).ToList();
                decimal TotalWeight = 0.0m;
                foreach (var obj in packages)
                {
                    TotalWeight += obj.Weight * obj.CartonValue;
                }

                string ShipmentMethod = shipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodDisplay;
                string Shipment = string.Empty;


                var FrayteNo = shipmentDetail.FrayteNumber;

                var operationzone = UtilityRepository.GetOperationZone();

                #region  Link Action


                var confirmLink = AppSettings.TrackingUrl + "/action/c/" + shipmentDetail.TradelaneShipmentId;
                var rejectLinkLink = AppSettings.TrackingUrl + "/action/r/" + shipmentDetail.TradelaneShipmentId;

                #endregion

                DynamicViewBag viewBag = new DynamicViewBag();
                viewBag.AddValue("ConfirmLink", confirmLink);
                viewBag.AddValue("RejectLink", rejectLinkLink);

                viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                viewBag.AddValue("TotalWeight", TotalWeight);

                viewBag.AddValue("ShipmentMethod", ShipmentMethod);
                viewBag.AddValue("TrackingNo", "");
                viewBag.AddValue("TrackingURL", "");
                viewBag.AddValue("TotalCartoon", shipmentDetail.Packages.Sum(p => p.CartonValue));
                viewBag.AddValue("CustomerName", customerDetail.CustomerName + " (Consignee)");
                viewBag.AddValue("CustomerEmail", customerDetail.CustomerEmail);
                viewBag.AddValue("CompanyName", customerDetail.CompanyName);
                viewBag.AddValue("UserName", staffdetail.OperationStaffName);
                viewBag.AddValue("UserPosition", staffdetail.DeptName);
                viewBag.AddValue("UserEmail", staffdetail.OperationStaffEmail);
                viewBag.AddValue("ImageHeader", "FrayteLogo");
                viewBag.AddValue("TrackButton", "TrackShipment");
                viewBag.AddValue("FrayteNumber", shipmentDetail.FrayteNumber);
                viewBag.AddValue("PickUp", "");
                viewBag.AddValue("OperationZoneId", operationzone.OperationZoneId);
                if (operationzone.OperationZoneId == 1)
                {
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                    viewBag.AddValue("UserPhone", "+852 2148 4880");
                }
                else
                {
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
                    viewBag.AddValue("UserPhone", "+44 01792 678017");
                }

                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/TradelaneBookingCustomerConfirmation.cshtml");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, shipmentDetail, viewBag, null);
                string EmailSubject = string.Empty;
                if (shipmentDetail.ShipFrom.CompanyName == "" || shipmentDetail.ShipFrom.CompanyName == null)
                {
                    EmailSubject = "Booking Confirmation - (" + shipmentDetail.ShipFrom.FirstName + " " + shipmentDetail.ShipFrom.LastName + ")" + " - FR#" + shipmentDetail.FrayteNumber + " - FRAYTE GLOBAL";
                }
                else
                {
                    EmailSubject = "Booking Confirmation - (" + shipmentDetail.ShipFrom.CompanyName + ")" + " - FR#" + shipmentDetail.FrayteNumber + " - FRAYTE GLOBAL";
                }
                var To = customerDetail.CustomerEmail;
                var CC = "";
                string Status = "Confirmation";


                //Send mail to Customer
                SendMail_New(To, CC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, "", Status);

            }
            catch (Exception e)
            {

            }
        }

        public FrayteResult SendPreAlertEmail(TradelanePreAlertInitial preAlerDetail)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var OpeartionZone = UtilityRepository.GetOperationZone();
                var userDetail = (from r in dbContext.Users
                                  join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                                  join uad in dbContext.UserAddresses on r.UserId equals uad.UserId
                                  join c in dbContext.Countries on uad.CountryId equals c.CountryId
                                  join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                                  where r.UserId == preAlerDetail.UserId
                                  select new
                                  {
                                      Email = r.Email,
                                      UserEmail = r.UserEmail,
                                      Name = r.ContactName,
                                      Phone = r.PhoneNumber,
                                      TelePhone = r.TelephoneNo,
                                      RoleId = ur.RoleId,
                                      CountryPhoneCode = c.CountryPhoneCode
                                  }).FirstOrDefault();

                if (userDetail != null)
                {
                    DynamicViewBag viewBag = new DynamicViewBag();
                    viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                    viewBag.AddValue("EmailBody", preAlerDetail.EmailBody);
                    viewBag.AddValue("ImageHeader", "FrayteLogo");

                    // Kind Regards
                    viewBag.AddValue("UserEmail", userDetail.RoleId == (int)FrayteUserRole.Customer ? userDetail.UserEmail : userDetail.Email);
                    viewBag.AddValue("UserName", userDetail.Name);
                    viewBag.AddValue("PhoneNo", "(" + userDetail.CountryPhoneCode + ") " + userDetail.TelePhone);
                    viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);


                    string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Tradelane/PreAlert.cshtml");
                    var templateService = new TemplateService();
                    string EmailSubject = preAlerDetail.EmailSubject;
                    var EmailBody = templateService.Parse(template, preAlerDetail, viewBag, null);


                    //Set To , CC , BCC  
                    string TO = string.Empty;
                    string CC = string.Empty;
                    string BCC = string.Empty;

                    TO = new TradelaneShipmentRepository().PreAlertEmails(preAlerDetail, "TO");
                    CC = new TradelaneShipmentRepository().PreAlertEmails(preAlerDetail, "CC");
                    BCC = new TradelaneShipmentRepository().PreAlertEmails(preAlerDetail, "BCC");


                    CC += userDetail.RoleId == (int)FrayteUserRole.Customer ? userDetail.UserEmail : userDetail.Email;

                    string Status = "Confirmation";

                    #region Attach Labels

                    string Attachments = new TradelaneShipmentRepository().PreAlertShipmentDocuments(preAlerDetail);

                    #endregion

                    //Send mail to Customer
                    Send_PreAlertEmail(TO, CC, BCC, userDetail.RoleId == (int)FrayteUserRole.Customer ? userDetail.UserEmail : userDetail.Email, EmailSubject, EmailBody, Attachments, Status);
                    result.Status = true;
                }

            }
            catch (Exception ex)
            {
                result.Status = false;
            }
            return result;
        }

        #endregion

        #region -- Send POD Mail --

        public FrayteResult SendPODMail(List<FraytePODInfomation> PodDetail, int CustomerId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";

                var customerDetail = (from u in dbContext.Users
                                      join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                      join uu in dbContext.Users on ua.OperationUserId equals uu.UserId
                                      where u.UserId == CustomerId
                                      select new
                                      {
                                          CompanyName = u.CompanyName,
                                          OperationZoneId = u.OperationZoneId,
                                          CustomerEmail = u.UserEmail,
                                          StaffName = uu.ContactName,
                                          StaffEmail = uu.Email
                                      }).FirstOrDefault();

                //var UserTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userInfo.Name);
                //var dbShipment = dbContext.DirectShipments.Find(DirectShipmentid);
                //var CreatedOnDate = UtilityRepository.UtcDateToOtherTimezone(dbShipment.CreatedOn, dbShipment.CreatedOn.TimeOfDay, UserTimeZoneInfo).Item1;
                //var CreatedOnTime = UtilityRepository.GetFormattedTimeFromString(UtilityRepository.UtcDateToOtherTimezone(dbShipment.CreatedOn, dbShipment.CreatedOn.TimeOfDay, UserTimeZoneInfo).Item2);

                DynamicViewBag viewBag = new DynamicViewBag();
                viewBag.AddValue("CompanyName", customerDetail.CompanyName);
                if (customerDetail.OperationZoneId == 1)
                {
                    viewBag.AddValue("UserPhone", "(+852) 2148 4880");
                    viewBag.AddValue("SiteAddress", "www.FRAYTE.com");
                }
                else
                {
                    viewBag.AddValue("UserPhone", "(+44) 01792 678017");
                    viewBag.AddValue("SiteAddress", "www.FRAYTE.co.uk");
                }
                viewBag.AddValue("StaffName", customerDetail.StaffName);
                viewBag.AddValue("StaffEmail", customerDetail.StaffEmail);
                viewBag.AddValue("ImageHeader", "FrayteLogo");

                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/PODInformation.cshtml");

                PODInformation pod = new PODInformation();
                pod.PodDetail = PodDetail;

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, pod, viewBag, null);
                string EmailSubject = "Shipments Delivered - [" + customerDetail.CompanyName + "]";

                var To = customerDetail.CustomerEmail;

                //Send mail to Customer
                if (!string.IsNullOrEmpty(AppSettings.TOCC))
                {
                    FrayteEmail.SendMail("", AppSettings.TOCC, EmailSubject, EmailBody, logoImage);
                    result.Status = true;
                }
                else
                {
                    FrayteEmail.SendMail(To, AppSettings.BCC, EmailSubject, EmailBody, logoImage);
                    result.Status = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion
    }
}