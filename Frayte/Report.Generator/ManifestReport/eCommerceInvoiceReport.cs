using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Utility;
using Frayte.Services.Business;
using Frayte.Services.Models;
using System.Web;
using System.Drawing.Imaging;
using System.IO;

namespace Report.Generator.ManifestReport
{
    public class eCommerceInvoiceReport
    {
        public FrayteResult GenerateInvoiceReport(int eCommerceShipmentId, eCommerceTaxAndDutyInvoiceReport obj)
        {
            FrayteResult result = new FrayteResult();
            if (!File.Exists(HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + "/" + eCommerceShipmentId + "/" + obj.InvoiceRef.ToString() + ".pdf"))
            {
                List<eCommerceTaxAndDutyInvoiceReport> reportDataSource = new List<eCommerceTaxAndDutyInvoiceReport>();

                if (obj != null)
                {
                    ReportTemplate.Other.eCommerceInvoiceReport re = new ReportTemplate.Other.eCommerceInvoiceReport();
                    re.xrRichText5.Text = obj.eCommerceBookingDetail.ShipTo.FirstName.ToUpper() + " " + obj.eCommerceBookingDetail.ShipTo.LastName.ToUpper() +
                                       System.Environment.NewLine +
                                       obj.eCommerceBookingDetail.ShipTo.CompanyName.ToUpper() + System.Environment.NewLine + obj.eCommerceBookingDetail.ShipTo.Address.ToUpper();

                    if (!string.IsNullOrEmpty(obj.eCommerceBookingDetail.ShipTo.Address2))
                    {
                        re.xrRichText5.Text += System.Environment.NewLine + obj.eCommerceBookingDetail.ShipTo.Address2.ToUpper();
                    }

                    re.xrRichText5.Text += System.Environment.NewLine + obj.eCommerceBookingDetail.ShipTo.City.ToUpper();

                    if (!string.IsNullOrEmpty(obj.eCommerceBookingDetail.ShipTo.PostCode))
                    {
                        re.xrRichText5.Text += " -  " + obj.eCommerceBookingDetail.ShipTo.PostCode.ToUpper();
                    }
                    if (!string.IsNullOrEmpty(obj.eCommerceBookingDetail.ShipTo.State))
                    {
                        re.xrRichText5.Text += System.Environment.NewLine + obj.eCommerceBookingDetail.ShipTo.State.ToUpper();
                    }
                    if (obj.eCommerceBookingDetail.ShipTo != null && obj.eCommerceBookingDetail.ShipTo.Country != null && !string.IsNullOrEmpty(obj.eCommerceBookingDetail.ShipTo.Country.Name))
                    {
                        re.xrRichText5.Text += System.Environment.NewLine + obj.eCommerceBookingDetail.ShipTo.Country.Name.ToUpper();
                    }

                    obj.FreeStorageTime = "YOUR FREE STORAGE PERIOD ENDS "+ obj.FreeStorageTime + " HOURS AFTER SHIPMENT ARRIVAL INTO THE UK - £5 per KG will be charged thereafter";

                    if (!string.IsNullOrEmpty(obj.CompanyDetail.Address.Address2))
                    {
                        //re.xrRichText2.Text += ", " + obj.CompanyDetail.Address.Address2;
                    }
                    if (!string.IsNullOrEmpty(obj.CompanyDetail.Address.City))
                    {
                        //re.xrRichText2.Text += ", " + obj.CompanyDetail.Address.City;
                    }
                    if (!string.IsNullOrEmpty(obj.CompanyDetail.Address.Zip))
                    {
                        //re.xrRichText2.Text += " " + obj.CompanyDetail.Address.Zip;
                    }
                    if (!string.IsNullOrEmpty(obj.CompanyDetail.Address.State))
                    {
                        //re.xrRichText2.Text += ", " + obj.CompanyDetail.Address.State;
                    }
                    if (!string.IsNullOrEmpty(obj.CompanyDetail.Address.Country.Name))
                    {
                        //re.xrRichText2.Text += ", " + obj.CompanyDetail.Address.Country.Name;
                    }

                    //re.xrRichText1.Text = obj.BankDetail.Address.Address1;
                    if (!string.IsNullOrEmpty(obj.BankDetail.Address.Address2))
                    {
                        //re.xrRichText1.Text += ", " + obj.BankDetail.Address.Address2;
                    }
                    if (!string.IsNullOrEmpty(obj.BankDetail.Address.City))
                    {
                        //re.xrRichText1.Text += ", " + obj.BankDetail.Address.City;
                    }
                    if (!string.IsNullOrEmpty(obj.BankDetail.Address.Zip))
                    {
                        //re.xrRichText1.Text += ", " + obj.BankDetail.Address.Zip;
                    }
                    //re.xrTableCell14.Text = "Company No:" + " " + obj.CompanyDetail.CompanyNo + ".";
                    //re.xrTableCell14.Text += "Company Registration Number " + "+44 " + obj.CompanyDetail.CompanyPhoneCode + " " + obj.CompanyDetail.CompanyPhone + ". ";
                    //re.xrTableCell14.Text += "Crest Code " + obj.CompanyDetail.CrestCode + ". ";
                    //re.xrTableCell14.Text += "Registered address: ";
                    //re.xrTableCell14.Text += System.Environment.NewLine;

                    //re.xrTableCell14.Text += obj.CompanyDetail.CompanyName + ", " + obj.CompanyDetail.Address.Address1;

                    if (!string.IsNullOrEmpty(obj.CompanyDetail.Address.Address2))
                    {
                        //re.xrTableCell14.Text += ", " + obj.CompanyDetail.Address.Address2;
                    }
                    if (!string.IsNullOrEmpty(obj.CompanyDetail.Address.City))
                    {
                        //re.xrTableCell14.Text += ", " + obj.CompanyDetail.Address.City;
                    }
                    if (!string.IsNullOrEmpty(obj.CompanyDetail.Address.Zip))
                    {
                        //re.xrTableCell14.Text += " " + obj.CompanyDetail.Address.Zip;
                    }
                    if (!string.IsNullOrEmpty(obj.CompanyDetail.Address.State))
                    {
                        //re.xrTableCell14.Text += ", " + obj.CompanyDetail.Address.State;
                    }
                    if (!string.IsNullOrEmpty(obj.CompanyDetail.Address.Country.Name))
                    {
                        //re.xrTableCell14.Text += ", " + obj.CompanyDetail.Address.Country.Name;
                    }
                     
                    //  re.xrTableCell14.Text 

                    //re.xrRichText4.Text = "P " + "+44" + obj.CompanyDetail.CompanyPhoneCode + " " + obj.CompanyDetail.CompanyPhone;
                    //re.xrRichText4.Text += " | " + " E " + obj.CompanyDetail.SalesEmail;
                     
                    reportDataSource.Add(obj);
                    re.DataSource = reportDataSource;

                    var FileName = "CCS - Customs Duty - Vat Invoice [" + obj.InvoiceRef + "]  [" + obj.CustomerName + "].pdf";
                     
                    string filePathToSave = AppSettings.eCommerceLabelFolder + "/" + eCommerceShipmentId.ToString();
                    filePathToSave = HttpContext.Current.Server.MapPath(filePathToSave);

                    DevExpress.XtraPrinting.PdfExportOptions pdfOptions = re.ExportOptions.Pdf;

                    pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                    pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;

                    if (!System.IO.Directory.Exists(filePathToSave))
                        System.IO.Directory.CreateDirectory(filePathToSave);
                    re.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + "/" + eCommerceShipmentId + "/" + FileName, pdfOptions);
                    result.Status = true;
                }
            }
            else
            {
                result.Status = true;
            }

            return result;
        }
    }
}
