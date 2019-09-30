using Frayte.Services.Business;
using Frayte.Services.Utility;
using Frayte.Services.Models;
using Report.Generator.ReportTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Net.Http;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using System.IO;
using DevExpress.XtraPrinting;

namespace Report.Generator.ManifestReport
{
    public class QuoteReport
    {
        public FrayteManifestName GetQuotation(int QuotationShipmentId, string CustomerName, string FromPostCode, string ToPostCode, int CreatedBy)
        {
            FrayteManifestName result = new FrayteManifestName();
            var item = new QuotationRepository().GetQuotationDetail(QuotationShipmentId, CustomerName);

            var TimeZone = new QuotationRepository().GetUserTimeZone(CreatedBy);

            var UserType = UtilityRepository.GetUserType(CreatedBy);

            var DateObj = DateTime.UtcNow;
            TimeZoneInfo TimeZoneInformation = TimeZoneInfo.FindSystemTimeZoneById(TimeZone.Name);
            var remoteTime = TimeZoneInfo.ConvertTime(DateObj, TimeZoneInformation).ToString("hh:mm:ss tt");

            if (item != null)
            {
                int CartoonQty = new QuotationRepository().SumofCartoonQty(item.QuotationShipmentId);
                decimal Volume = new QuotationRepository().TotalVolume(item.QuotationShipmentId, item.PackageCalculationType);

                if (item.OperationZoneId == 1)
                {
                    result = HKQuoteReport(item, Volume, CartoonQty, TimeZoneInformation, remoteTime);
                }
                else if (item.OperationZoneId == 2)
                {
                    result = UKQuoteReport(item, Volume, CartoonQty, TimeZoneInformation, remoteTime, UserType, CreatedBy);
                }
            }
            return result;
        }

        protected internal FrayteManifestName HKQuoteReport(FrayteQuotationReport item, decimal Volume, int CartoonQty, TimeZoneInfo TimeZoneInformation, string Time)
        {
            FrayteManifestName result = new FrayteManifestName();

            result.OfferValidity = item.OfferValidity;
            result.TransitTime = item.TransitTime;
            ReportTemplate.Other.HKQuote report = new ReportTemplate.Other.HKQuote(item.ShipFrom.ToUpper(), item.ShipTo.ToUpper());

            report.Parameters["WebsiteUrl"].Value = "www.frayte.com".ToUpper();
            report.Parameters["CustomerName"].Value = item.CustomerName;
            report.Parameters["Information"].Value = "Thank you for requesting a spot courier quote from FRAYTE GLOBAL we are pleased to present your personalised quote for [" + item.CompanyName + "] below:";
            report.Parameters["Currency"].Value = item.QuoteCurrency;
            report.Parameters["ReferenceValue"].Value = "This quote is only valid for FRAYTE account number: " + UtilityRepository.FrayteAccountNo(item.FrayteAccountNo) + ". " +
                                                        "Quote issued on " + UtilityRepository.UtcDateToOtherTimezone(Convert.ToDateTime(item.QuoteIssueDate.Date), item.QuoteIssueDate.TimeOfDay, TimeZoneInformation).Item1.ToString("dd-MMM-yyyy") + " " + Time + " with reference number " + UtilityRepository.QuotationFileName(item.QuotationShipmentId.ToString(), item.OperationZoneId, ".pdf").Replace(".pdf", "") + ". " +
                                                        "As per your confirmation, this shipment DOES NOT contain dangerous goods of any type and any quantity. All quotes are excluding any applicable Import " +
                                                        "clearance duty and taxes which are charged against the shipment. The above quote is inclusive of the current fuel surcharge " +
                                                        "unless otherwise stated. Transit Time is in working days and can be subject to change. Multiple piece consignments " +
                                                        "may require palletising, this could increase the chargeable weight of the consignment.";
            report.Parameters["TotalPrice"].Value = Math.Round(item.TotalPrice, 2);
            report.Parameters["Days"].Value = item.OfferValidity;
            report.Parameters["CartoonQty"].Value = CartoonQty;
            report.Parameters["PricePerKg"].Value = item.QuoteCurrency + " " + Math.Round((item.Rate + item.FuelSurcharge + item.SupplementryCharge) / (item.GrossWeight), 2).ToString("N", new System.Globalization.CultureInfo("en-US"));
            if (item.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
            {
                if (item.GrossWeight > Volume)
                    report.Parameters["GrossWeight"].Value = item.GrossWeight.ToString("N", new System.Globalization.CultureInfo("en-US")) + " " + "kg";
                else
                    report.Parameters["GrossWeight"].Value = Math.Round(Volume, 2).ToString("N", new System.Globalization.CultureInfo("en-US")) + " " + "kg";
            }
            else
            {
                if (item.GrossWeight > Volume)
                    report.Parameters["GrossWeight"].Value = item.GrossWeight.ToString("N", new System.Globalization.CultureInfo("en-US")) + " " + "lb.";
                else
                    report.Parameters["GrossWeight"].Value = Math.Round(Volume, 2).ToString("N", new System.Globalization.CultureInfo("en-US")) + " " + "lb.";
            }
            report.Parameters["Volume"].Value = Math.Round(Volume, 2).ToString("N", new System.Globalization.CultureInfo("en-US")) + " cbm";
            report.Parameters["Service"].Value = item.Service;
            report.Parameters["Description"].Value = item.Description;

            report.Parameters["Rate"].Value = item.QuoteCurrency + " " + Math.Round(item.Rate, 2).ToString("N", new System.Globalization.CultureInfo("en-US"));
            report.Parameters["FuelSurcharge"].Value = item.QuoteCurrency + " " + (item.FuelSurcharge).ToString("N", new System.Globalization.CultureInfo("en-US"));
            report.Parameters["Origin"].Value = item.Origin;
            report.Parameters["Destination"].Value = item.Destination;
            if (item.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
            {
                report.Parameters["ChargeWeight"].Value = item.ChargeableWeight.ToString("N", new System.Globalization.CultureInfo("en-US")) + " " + "kg";
                result.TotalWeight = item.GrossWeight.ToString("N", new System.Globalization.CultureInfo("en-US")) + " " + "kg";
            }
            else
            {
                report.Parameters["ChargeWeight"].Value = item.ChargeableWeight.ToString("N", new System.Globalization.CultureInfo("en-US")) + " " + "lb.";
                result.TotalWeight = item.GrossWeight.ToString("N", new System.Globalization.CultureInfo("en-US")) + " " + "lb.";
            }
            report.Parameters["Total"].Value = item.QuoteCurrency + " " + Math.Round(item.TotalPrice, 2).ToString("N", new System.Globalization.CultureInfo("en-US"));

            PdfExportOptions options = new PdfExportOptions();
            options.ImageQuality = PdfJpegImageQuality.Highest;
            options.PdfACompatibility = PdfACompatibility.None;

            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + UtilityRepository.QuotationFileName(item.QuotationShipmentId.ToString(), item.OperationZoneId, ".pdf"), options);
            result.FileName = UtilityRepository.QuotationFileName(item.QuotationShipmentId.ToString(), item.OperationZoneId, ".pdf");
            result.FilePath = AppSettings.WebApiPath + "ReportFiles/" + UtilityRepository.QuotationFileName(item.QuotationShipmentId.ToString(), item.OperationZoneId, ".pdf");
            result.TotalCost = Convert.ToString(Math.Round(item.TotalPrice, 2)) + " " + item.QuoteCurrency;

            return result;
        }

        protected internal FrayteManifestName UKQuoteReport(FrayteQuotationReport item, decimal Volume, int CartoonQty, TimeZoneInfo TimeZoneInformation, string Time, string UserType, int CreatedBy)
        {
            FrayteManifestName result = new FrayteManifestName();

            result.OfferValidity = item.OfferValidity;
            result.TransitTime = item.TransitTime;

            ReportTemplate.Other.UKQuote report = new ReportTemplate.Other.UKQuote(item.ShipFrom.ToUpper(), item.ShipTo.ToUpper(), UserType);

            string name = string.Empty;
            string account = string.Empty;
            if (UserType == FrayteUserType.SPECIAL)
            {
                var company = UtilityRepository.GetCustomerCompnay(CreatedBy);
                report.Parameters["ImagePath"].Value = AppSettings.EmailServicePath + "/EmailTeamplate/" + CreatedBy + "/Images/" + company.LogoFileName;
                report.Parameters["HeaderPath"].Value = AppSettings.EmailServicePath + "/EmailTeamplate/" + CreatedBy + "/Images/MAX-Header.png";
                report.Parameters["WebsiteUrl"].Value = company.MainWebsite.ToUpper();
                name = company.CompanyName + " Logistics.";
                account = company.CompanyName;
            }
            else
            {
                report.Parameters["ImagePath"].Value = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
                report.Parameters["HeaderPath"].Value = AppSettings.EmailServicePath + "/Images/Frayte-Header.png";
                report.Parameters["WebsiteUrl"].Value = "www.frayte.co.uk".ToUpper();
                name = "FRAYTE GLOBAL";
                account = "FRAYTE";
            }

            report.Parameters["CustomerName"].Value = item.CustomerName;
            report.Parameters["Information"].Value = "Thank you for requesting a spot courier quote from " + name + " we are pleased to present your personalised quote for [" + item.CompanyName + "] below:";
            report.Parameters["Currency"].Value = item.QuoteCurrency;
            report.Parameters["ReferenceValue"].Value = "This quote is only valid for " + account + " account number: " + UtilityRepository.FrayteAccountNo(item.FrayteAccountNo) + ". " +
                                                        "Quote issued on " + UtilityRepository.UtcDateToOtherTimezone(Convert.ToDateTime(item.QuoteIssueDate.Date), item.QuoteIssueDate.TimeOfDay, TimeZoneInformation).Item1.ToString("dd-MMM-yyyy") + " " + Time + " with reference number " + UtilityRepository.QuotationFileName(item.QuotationShipmentId.ToString(), item.OperationZoneId, ".pdf").Replace(".pdf", "") + ". " +
                                                        "As per your confirmation, this shipment DOES NOT contain dangerous goods of any type and any quantity. All quotes are excluding any applicable Import " +
                                                        "clearance duty and taxes which are charged against the shipment. The above quote is inclusive of the current fuel surcharge " +
                                                        "unless otherwise stated. Transit Time is in working days and can be subject to change. Multiple piece consignments " +
                                                        "may require palletising, this could increase the chargeable weight of the consignment.";
            report.Parameters["TotalPrice"].Value = Math.Round(item.TotalPrice, 2);
            report.Parameters["Days"].Value = item.OfferValidity;
            report.Parameters["CartoonQty"].Value = CartoonQty;
            report.Parameters["PricePerKg"].Value = item.QuoteCurrency + " " + Math.Round((item.Rate + item.FuelSurcharge + item.SupplementryCharge) / (item.GrossWeight), 2).ToString("N", new System.Globalization.CultureInfo("en-US"));
            if (item.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
            {
                if (item.GrossWeight > Volume)
                    report.Parameters["GrossWeight"].Value = item.GrossWeight.ToString("N", new System.Globalization.CultureInfo("en-US")) + " " + "kg";
                else
                    report.Parameters["GrossWeight"].Value = Math.Round(Volume, 2).ToString("N", new System.Globalization.CultureInfo("en-US")) + " " + "kg";
            }
            else
            {
                if (item.GrossWeight > Volume)
                    report.Parameters["GrossWeight"].Value = item.GrossWeight.ToString("N", new System.Globalization.CultureInfo("en-US")) + " " + "lb.";
                else
                    report.Parameters["GrossWeight"].Value = Math.Round(Volume, 2).ToString("N", new System.Globalization.CultureInfo("en-US")) + " " + "lb.";
            }
            report.Parameters["Volume"].Value = Math.Round(Volume, 2).ToString("N", new System.Globalization.CultureInfo("en-US")) + " cbm";
            report.Parameters["Service"].Value = item.Service;
            report.Parameters["Description"].Value = item.Description;

            report.Parameters["Rate"].Value = item.QuoteCurrency + " " + Math.Round(item.Rate, 2).ToString("N", new System.Globalization.CultureInfo("en-US"));
            report.Parameters["FuelSurcharge"].Value = item.QuoteCurrency + " " + (item.FuelSurcharge).ToString("N", new System.Globalization.CultureInfo("en-US"));
            report.Parameters["Origin"].Value = item.Origin;
            report.Parameters["Destination"].Value = item.Destination;
            if (item.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
            {
                report.Parameters["ChargeWeight"].Value = item.ChargeableWeight.ToString("N", new System.Globalization.CultureInfo("en-US")) + " " + "kg";
                result.TotalWeight = item.GrossWeight.ToString("N", new System.Globalization.CultureInfo("en-US")) + " " + "kg";
            }
            else
            {
                report.Parameters["ChargeWeight"].Value = item.ChargeableWeight.ToString("N", new System.Globalization.CultureInfo("en-US")) + " " + "lb.";
                result.TotalWeight = item.GrossWeight.ToString("N", new System.Globalization.CultureInfo("en-US")) + " " + "lb.";
            }
            report.Parameters["Total"].Value = item.QuoteCurrency + " " + Math.Round(item.TotalPrice, 2).ToString("N", new System.Globalization.CultureInfo("en-US"));

            PdfExportOptions options = new PdfExportOptions();
            options.ImageQuality = PdfJpegImageQuality.Highest;
            options.PdfACompatibility = PdfACompatibility.None;

            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + UtilityRepository.QuotationFileName(item.QuotationShipmentId.ToString(), item.OperationZoneId, ".pdf"), options);
            result.FileName = UtilityRepository.QuotationFileName(item.QuotationShipmentId.ToString(), item.OperationZoneId, ".pdf");
            result.FilePath = AppSettings.WebApiPath + "ReportFiles/" + UtilityRepository.QuotationFileName(item.QuotationShipmentId.ToString(), item.OperationZoneId, ".pdf");
            result.TotalCost = Convert.ToString(Math.Round(item.TotalPrice, 2)) + " " + item.QuoteCurrency;

            return result;
        }
    }
}