using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Web.Hosting;
using System.Runtime.InteropServices;
using System.Diagnostics;
using RazorEngine.Templating;
using XStreamline.Log;

namespace Frayte.Services.Business
{
    public class eCommerceControlDeptRepository
    {
        FrayteEntities dbContext = new FrayteEntities();
        public List<FrayteUploadshipment> GetUnpaidShipments()
        {

            var UnpaidShipments = (from EI in dbContext.eCommerceInvoices
                                   join ECS in dbContext.eCommerceShipments on EI.ShipmentId equals ECS.eCommerceShipmentId
                                   join ECA in dbContext.eCommerceShipmentAddresses on ECS.ToAddressId equals ECA.eCommerceShipmentAddressId
                                   join CT in dbContext.CurrencyTypes on ECS.CurrencyCode equals CT.CurrencyCode
                                   join CTRY in dbContext.Countries on ECA.CountryId equals CTRY.CountryId
                                   where EI.Status == eCommerceAppTaxAndDutyStatus.TaxAndDutyUnPaid || EI.Status == eCommerceAppTaxAndDutyStatus.TaxAndDutyPartiallyPaid
                                   select new FrayteUploadshipment()
                                   {

                                       ShipTo = new DirectBookingCollection()
                                       {
                                           Country = new FrayteCountryCode()
                                           {
                                               CountryId = CTRY.CountryId,
                                               Code = CTRY.CountryCode,
                                               Code2 = CTRY.CountryCode2,
                                               Name = CTRY.CountryName

                                           },
                                           FirstName = ECA.ContactFirstName,
                                           LastName = ECA.ContactLastName,
                                           PostCode = ECA.Zip,
                                           CompanyName = ECA.CompanyName,
                                           Address = ECA.Address1,
                                           Phone = ECA.PhoneNo,
                                           Email = ECA.Email,
                                           City = ECA.City

                                       },
                                       PayTaxAndDuties = ECS.PaymentPartyTaxAndDuties,
                                       parcelType = ECS.ParcelType,
                                       CurrencyCode = CT.CurrencyCode,
                                       ShipmentDescription = ECS.ContentDescription
                                   }).ToList();

            return UnpaidShipments;

        }
        public static Logger Get_Log()
        {
            Logger log = BaseLog.Instance.GetLogger(null);
            return log;
        }

        public FrayteResult MailSendToControlDept(string FileName, string FilePath, List<FrayteUploadshipment> UnpaidShipments)
        {
            FrayteResult fr = new FrayteResult();

            var operationzone = UtilityRepository.GetOperationZone();
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            DynamicViewBag viewBag = new DynamicViewBag();

            viewBag.AddValue("TrackingDescription", "Please find the attachment.");
            viewBag.AddValue("Name", AppSettings.ControllerDeptName);
            viewBag.AddValue("ImageHeader", "FrayteLogo");

            if (operationzone.OperationZoneId == 1)
            {
                viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
            }
            else
            {
                viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
            }
            var res = UnpaidShipments[0];
            //string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/eCommerceControlDept.cshtml");
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/dhl.cshtml");

            TemplateService templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, res, viewBag, null);
            string EmailSubject = "Manifest Tracking";
            string Attachmentfilepath = FilePath;
            var To = AppSettings.ControllerDeptEmail;
            //_log.Error(MTM.ReceiverMail);
            //var To = "vikshit9292@gmail.com";
            //var CC = customerDetail.UserEmail;
            string Status = "Confirmation";

            //Send mail to Customer
            //SendMail_New(To, CC, "FRAYTE (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment, Status);
            FrayteEmail.SendMail(To, "", EmailSubject, EmailBody, Attachmentfilepath, logoImage);
            return fr;
        }

        public FrayteResult ControlDeptExcel(List<FrayteUploadshipment> UnpaidShipments, out string FileName, out string FilePath)
        {
            FrayteResult FR = new FrayteResult();
            Application ExcelApp = new Application();
            Workbook ExcelWorkBook = null;
            Worksheet ExcelWorkSheet = null;
            //ExcelApp.Visible = true;
            ExcelWorkBook = ExcelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            try
            {
                int r = 1;
                ExcelWorkBook.Worksheets.Add();
                ExcelWorkSheet = ExcelWorkBook.Worksheets[1];
                ExcelWorkSheet.Cells[r, 1] = "ReceiverCountryCode";
                ExcelWorkSheet.Cells[r, 2] = "ReceiverPostCode";
                ExcelWorkSheet.Cells[r, 3] = "ReceiverContactFirstName";
                ExcelWorkSheet.Cells[r, 4] = "ReceiverContactLastName";
                ExcelWorkSheet.Cells[r, 5] = "ReceiverCompanyName";
                ExcelWorkSheet.Cells[r, 6] = "ReceiverAddress1";
                //ExcelWorkSheet.Cells[r, 7] = "ReceiverAddress2";
                ExcelWorkSheet.Cells[r, 7] = "ReceiverCity";
                ExcelWorkSheet.Cells[r, 8] = "ReceiverTelephoneNo";
                ExcelWorkSheet.Cells[r, 9] = "ReceiverEmail";
                ExcelWorkSheet.Cells[r, 10] = "ParcelType";
                ExcelWorkSheet.Cells[r, 11] = "Currency";
                ExcelWorkSheet.Cells[r, 12] = "ShipmentDescription";
                ExcelWorkSheet.Cells[r, 13] = "PaymentPartyTaxandDuty";
                r++;
                for (int j = 0; j < UnpaidShipments.Count; j++)
                {
                    ExcelWorkSheet.Cells[r, 1] = UnpaidShipments[j].ShipTo.Country.Code;
                    ExcelWorkSheet.Cells[r, 2] = UnpaidShipments[j].ShipTo.PostCode;
                    ExcelWorkSheet.Cells[r, 3] = UnpaidShipments[j].ShipTo.FirstName;
                    ExcelWorkSheet.Cells[r, 4] = UnpaidShipments[j].ShipTo.LastName;
                    ExcelWorkSheet.Cells[r, 5] = UnpaidShipments[j].ShipTo.CompanyName;
                    ExcelWorkSheet.Cells[r, 6] = UnpaidShipments[j].ShipTo.Address;
                    //ExcelWorkSheet.Cells[r, 7] = UnpaidShipments[j].ShipTo.Address2;
                    ExcelWorkSheet.Cells[r, 7] = UnpaidShipments[j].ShipTo.City;
                    ExcelWorkSheet.Cells[r, 8] = UnpaidShipments[j].ShipTo.Phone;
                    ExcelWorkSheet.Cells[r, 9] = UnpaidShipments[j].ShipTo.Email;
                    ExcelWorkSheet.Cells[r, 10] = UnpaidShipments[j].parcelType;
                    ExcelWorkSheet.Cells[r, 11] = UnpaidShipments[j].CurrencyCode != null ? UnpaidShipments[j].CurrencyCode : "";
                    ExcelWorkSheet.Cells[r, 12] = UnpaidShipments[j].ShipmentDescription;
                    ExcelWorkSheet.Cells[r, 13] = UnpaidShipments[j].PayTaxAndDuties;
                    r++;
                }

                //ExcelWorkBook.SaveAs(HostingEnvironment.MapPath("~/UploadFiles/ControlDept/" + "ControlDeptExcel" + ".xlsx"));
                ExcelWorkBook.SaveAs(AppSettings.ControlDeptFolderPath, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
        false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);


                ExcelWorkBook.Close();
                ExcelApp.Quit();
                Marshal.ReleaseComObject(ExcelWorkSheet);
                Marshal.ReleaseComObject(ExcelWorkBook);
                Marshal.ReleaseComObject(ExcelApp);
                FR.Status = true;
            }
            catch (Exception ex)
            {
                FR.Status = false;
                Logger _log = Get_Log();
                _log.Info("before send mail section");
                _log.Error(ex.Message);
            }
            finally
            {
                foreach (Process process in Process.GetProcessesByName("Excel"))
                    process.Kill();
            }
            FileName = "ControlDeptExcel.xlsx";
            FilePath = AppSettings.ControlDeptFolderPath;
            return FR;
        }
    }
}
