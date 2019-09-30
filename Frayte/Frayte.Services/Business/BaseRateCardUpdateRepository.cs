using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Data;
using Microsoft.Office.Interop;
using Microsoft.Office.Interop.Excel;
using System.Web.Hosting;
using System.IO;

namespace Frayte.Services.Business
{
    public class BaseRateCardUpdateRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public FrayteResult UpdateRates(System.Data.DataTable exceldata)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                int k = 0;
                foreach (DataRow shipmentdetail in exceldata.Rows)
                {
                    if (k > 0)
                    {
                        for (int i = 0; i < exceldata.Columns.Count; i++)
                        {
                            var id = exceldata.Columns[i].ColumnName;
                            var weightid = int.Parse(shipmentdetail.ItemArray[0].ToString());
                            if (i > 2)
                            {
                                var zoneid = int.Parse(id.ToString());
                                var dbRate = dbContext.LogisticServiceBaseRateCards.Where(p => p.LogisticServiceZoneId == zoneid && p.LogisticServiceWeightId == weightid).FirstOrDefault();
                                if (dbRate != null)
                                {
                                    dbRate.LogisticRate = decimal.Parse(shipmentdetail.ItemArray[i].ToString());
                                    dbContext.Entry(dbRate).State = System.Data.Entity.EntityState.Modified;
                                    dbContext.SaveChanges();
                                }
                                else
                                {
                                    LogisticServiceBaseRateCard rate = new LogisticServiceBaseRateCard();
                                    rate.OperationZoneId = 2;
                                    rate.LogisticServiceZoneId = zoneid;
                                    rate.LogisticServiceWeightId = weightid;
                                    rate.LogisticServiceShipmentTypeId = 48;
                                    rate.LogisticServiceDimensionId = 0;
                                    rate.LogisticServiceCourierAccountId = 95;
                                    rate.LogisticRate = decimal.Parse(shipmentdetail.ItemArray[i].ToString());
                                    rate.LogisticCurrency = "GBP";
                                    rate.ModuleType = "DirectBooking";

                                    dbContext.LogisticServiceBaseRateCards.Add(rate);
                                    if (rate != null)
                                    {                                        
                                        dbContext.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    k++;
                }
                result.Status = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Status = false;
                return result;
            }
        }

        public FrayteManifestName DownloadRateCardExcelTemplate(string CourierCompany, string LogisticType, string RateType)
        {
            FrayteManifestName file = new FrayteManifestName();

            FrayteOperationZone OperationZone = UtilityRepository.GetOperationZone();

            var zonelist = (from lsz in dbContext.LogisticServiceZones
                            join ls in dbContext.LogisticServices on lsz.LogisticServiceId equals ls.LogisticServiceId
                            where ls.OperationZoneId == OperationZone.OperationZoneId &&
                                  ls.LogisticCompany == CourierCompany &&
                                  ls.LogisticType == LogisticType &&
                                  ls.RateType == RateType
                            select new FrayteBaseRateCardZone
                            {
                                LogisticZoneId = lsz.LogisticServiceZoneId,
                                LogisticZoneName = lsz.ZoneDisplayName
                            }).ToList();

            var weightlist = (from lsw in dbContext.LogisticServiceWeights
                              join lsst in dbContext.LogisticServiceShipmentTypes on lsw.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                              join ls in dbContext.LogisticServices on lsst.LogisticServiceId equals ls.LogisticServiceId
                              where ls.OperationZoneId == OperationZone.OperationZoneId &&
                                    ls.LogisticCompany == CourierCompany &&
                                    ls.LogisticType == LogisticType &&
                                    ls.RateType == RateType
                              select new FrayteBaseRateCardWeight
                              {
                                  LogisticWeightId = lsw.LogisticServiceWeightId,
                                  LogisticWeight = lsw.WeightToDisplay,
                                  LogisticShipmentType = lsst.LogisticDescription
                              }).ToList();

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
                for (int i = 0; i < zonelist.Count; i++)
                {
                    ExcelWorkSheet.Cells[r, i + 4] = zonelist[i].LogisticZoneId;
                    ExcelWorkSheet.Cells[r + 1, i + 4] = zonelist[i].LogisticZoneName;
                }
                r++;

                for (int j = 0; j < weightlist.Count; j++)
                {
                    ExcelWorkSheet.Cells[r + 1, 1] = weightlist[j].LogisticWeightId;
                    ExcelWorkSheet.Cells[r + 1, 2] = weightlist[j].LogisticShipmentType;
                    ExcelWorkSheet.Cells[r + 1, 3] = weightlist[j].LogisticWeight;
                    r++;
                }

                ExcelWorkBook.SaveAs(HostingEnvironment.MapPath("~/UploadFiles/BaseRateCardUpdate/" + CourierCompany + " " + LogisticType + " " + RateType));
                ExcelWorkBook.Close();
                ExcelApp.Quit();
                Marshal.ReleaseComObject(ExcelWorkSheet);
                Marshal.ReleaseComObject(ExcelWorkBook);
                Marshal.ReleaseComObject(ExcelApp);

                file.FileName = CourierCompany + " " + LogisticType + " " + RateType + ".xlsx";
                file.FilePath = AppSettings.WebApiPath + "UploadFiles/BaseRateCardUpdate/" + CourierCompany + " " + LogisticType + " " + RateType + ".xlsx";
            }
            catch (Exception ex)
            {

            }
            finally
            {
                foreach (Process process in Process.GetProcessesByName("Excel"))
                    process.Kill();
            }

            return file;
        }

        public string getExcelConnectionString(string FileName, string filepath)
        {

            // Microsoft.ACE.OLEDB.12.0
            if (Path.GetExtension(FileName) == ".xlsx")
            {
                return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=Excel 12.0";
            }
            else if (Path.GetExtension(FileName) == ".xls")
            {
                return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=Excel 8.0";
            }
            return "";
        }

        public string getFileExtensionString(string FileName)
        {

            if (Path.GetExtension(FileName) == ".xlsx")
            {
                return FrayteFileExtension.EXCEL;
            }
            else if (Path.GetExtension(FileName) == ".xls")
            {
                return FrayteFileExtension.EXCEL;
            }

            return "";
        }
    }
}
