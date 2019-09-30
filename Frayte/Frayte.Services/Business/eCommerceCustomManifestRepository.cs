using System;
using System.Collections.Generic;
using System.Linq;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;
using System.IO;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using Frayte.Services.Utility;
using XStreamline.Log;

namespace Frayte.Services.Business
{
    public class eCommerceCustomManifestRepository
    {
        public static Logger Get_Log()
        {
            Logger log = BaseLog.Instance.GetLogger(null);
            return log;
        }

        FrayteEntities dbContext = new FrayteEntities();
        #region
        public void GetMainfestData(DateTime ShipmentDate, TimeSpan FromTime, TimeSpan ToTime)
        {


            var GetCustomer = (from ECS in dbContext.eCommerceShipments
                               where
                                   ECS.EstimatedDateofDelivery <= ShipmentDate &&
                                   ECS.EstimatedTimeofDelivery <= FromTime &&
                                   (ECS.IsManifested != true || ECS.IsManifested == null)
                               select new
                               {
                                   ECS.CustomerId,
                                   ECS.EstimatedDateofDelivery
                               }).ToList();

            var getCustomer = GetCustomer.Distinct();
            var TimeZoneDetail = new TimeZoneModal();
            foreach (var res in getCustomer)
            {


                 TimeZoneDetail = (from ECS in dbContext.Users
                                      join Tmz in dbContext.Timezones on ECS.TimezoneId equals Tmz.TimezoneId
                                      where
                                          ECS.UserId == res.CustomerId
                                      select new TimeZoneModal
                                      {
                                          TimezoneId = Tmz.TimezoneId,
                                          Name = Tmz.Name,
                                          Offset = Tmz.Offset,
                                          OffsetShort = Tmz.OffsetShort
                                      }).FirstOrDefault();
            }
            Logger _log = Get_Log();
            
         
            var FromDateBeforeDayLightTime = new DateTime(2017, 03, 10, 07, 00, 00, DateTimeKind.Utc);
            var FromDateAfterDaylightTime = new DateTime(2017, 04, 14, 07, 00, 00, DateTimeKind.Utc);
            var zzzz = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time").GetUtcOffset(FromDateBeforeDayLightTime).TotalHours;
            var zzzz1 = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time").GetUtcOffset(FromDateBeforeDayLightTime).TotalHours;
            var GetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"); 
           

            _log.Info("Error due to fuel sur charge and exchange rate");
            

            var ServerTimeHour = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time").GetUtcOffset(FromDateAfterDaylightTime).TotalHours;
            var OffsetTimeHour = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time").GetUtcOffset(FromDateAfterDaylightTime).TotalHours;
        
            // Daylight saving time in USA with TimezoneInfo Class
            var TimeZoneInformationHongKong = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
            var TimeZoneInformationNewYork = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            var TimeZoneInformationHK = TimeZoneInfo.ConvertTime(FromDateBeforeDayLightTime, TimeZoneInformationHongKong);
            var DatebyUTCtoSelectedTimezone = TimeZoneInfo.ConvertTime(FromDateBeforeDayLightTime, TimeZoneInformationNewYork);
            var DatebyUTCtoSelectedTimezone1 = TimeZoneInfo.ConvertTime(FromDateAfterDaylightTime, TimeZoneInformationNewYork);
            
            var TimeZoneInformationHKinfo = TimeZoneInfo.ConvertTimeFromUtc(FromDateBeforeDayLightTime, TimeZoneInformationHongKong);
            var TimeZoneInformationHKinfoz = TimeZoneInfo.ConvertTimeFromUtc(FromDateAfterDaylightTime, TimeZoneInformationHongKong);
            var TimeZoneInformationNyinfo = TimeZoneInfo.ConvertTimeFromUtc(FromDateBeforeDayLightTime, TimeZoneInformationNewYork);
            var TimeZoneInformationNyinfoz = TimeZoneInfo.ConvertTimeFromUtc(FromDateAfterDaylightTime, TimeZoneInformationNewYork);
            _log.Info("Error due to fuel sur charge and exchange rate");
            //_log.Error("From Date is" + FromDateAfterDaylightTime + " Us Time after daylight saving time adjustment" + AddHoursToServerTimeZone1);
            _log.Info("Error due to fuel sur charge and exchange rate");
            _log.Error(FromTime.ToString());
            _log.Info("Error due to fuel sur charge and exchange rate");
            _log.Error(ToTime.ToString());
            var Fromtime = new DateTime(2017, 01, 01, FromTime.Hours, FromTime.Minutes, FromTime.Seconds).ToUniversalTime().TimeOfDay;
            var Totime = new DateTime(2017, 01, 01, ToTime.Hours, ToTime.Minutes, ToTime.Seconds).ToUniversalTime().TimeOfDay;
            //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(Fromtime.ToString()));
            //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(Totime.ToString()));
            _log.Info("Error due to fuel sur charge and exchange rate");
            _log.Error(Fromtime.ToString());
            _log.Info("Error due to fuel sur charge and exchange rate");
            _log.Error(Totime.ToString());
            //var TmDetail = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(), TimeZoneInfo.Local);
            var ManifestData = (from ECS in dbContext.eCommerceShipments
                                join EADF in dbContext.eCommerceShipmentAddresses on ECS.FromAddressId equals EADF.eCommerceShipmentAddressId
                                join EADT in dbContext.eCommerceShipmentAddresses on ECS.ToAddressId equals EADT.eCommerceShipmentAddressId
                                join PDD in dbContext.eCommerceShipmentDetails on ECS.eCommerceShipmentId equals PDD.eCommerceShipmentId
                                join TN in dbContext.eCommercePackageTrackingDetails on PDD.eCommerceShipmentDetailId equals TN.eCommerceShipmentDetailId
                                join TD in dbContext.eCommercePackageTrackingDetails on PDD.eCommerceShipmentDetailId equals TD.eCommerceShipmentDetailId
                                join Cur in dbContext.CurrencyTypes on ECS.CurrencyCode equals Cur.CurrencyCode
                                join Cun in dbContext.Countries on EADF.CountryId equals Cun.CountryId
                                where
                                      ECS.EstimatedDateofDelivery.Value <= ShipmentDate &&
                                      ECS.EstimatedTimeofDelivery.Value <= Fromtime &&
                                      (ECS.IsManifested != true || ECS.IsManifested == null)
                                select new FrayteManifestOnExcel
                                {
                                    ECommerceShipmentId = ECS.eCommerceShipmentId,
                                    TrackingNumber = TN.TrackingNo,
                                    Reference = ECS.Reference1,
                                    InternalAccountNumber = PDD.eCommerceShipmentDetailId,
                                    ShipperName = EADF.ContactFirstName + " " + EADF.ContactLastName,
                                    ShipperAddress1 = EADF.Address1,
                                    ShipperAddress2 = EADF.Address2,
                                    ShipperCity = EADF.City,
                                    ShipperZip = EADF.Zip,
                                    ShipperState = EADF.State,
                                    ShipperPhoneNo = EADF.PhoneNo,
                                    ShipperEmail = EADF.Email,
                                    ShipperCountryCode = Cun.CountryName,
                                    ConsigneeName = EADT.ContactFirstName + " " + EADT.ContactLastName,
                                    ConsigneeAddress1 = EADT.Address1,
                                    ConsigneeAddress2 = EADT.Address2,
                                    ConsigneeCity = EADT.City,
                                    ConsigneeZip = EADT.Zip,
                                    ConsigneeState = EADT.State,
                                    ConsigneePhoneNo = EADT.PhoneNo,
                                    ConsigneeEmail = EADT.Email,
                                    ConsigneeCountryCode = Cun.CountryName,
                                    WeightUOM = ECS.PackageCaculatonType,
                                    Currency = Cur.CurrencyDescription,
                                    Pieces = PDD.CartoonValue,
                                    TotalWeight = PDD.Weight * PDD.CartoonValue,
                                    TotalValue = PDD.CartoonValue * PDD.DeclaredValue,
                                    Incoterms = "DDP",
                                    ItemDescription = ECS.ContentDescription,
                                    ItemHScodes = PDD.HSCode,
                                    ItemValue = PDD.DeclaredValue,
                                    EstimatedDateofArrival = ECS.EstimatedDateofArrival,
                                    EstimatedTimeofArrival = ECS.EstimatedTimeofArrival,
                                    EstimatedDateofDelivery = ECS.EstimatedDateofDelivery,
                                    EstimatedTimeofDelivery = ECS.EstimatedTimeofDelivery
                                }).ToList();


            if (ManifestData.Count > 0)
            {
                // Send Mail before Creating Manifest

                var res = ManifestExcelWrite(ManifestData);

                // Send Mail before Creating Manifest

                // Make IsManifested Flag True
                if (res.Message == "True")
                {
                    foreach (var a in ManifestData)
                    {
                        var result = dbContext.eCommerceShipments.Where(p => p.eCommerceShipmentId == a.ECommerceShipmentId).FirstOrDefault();

                        if (result != null)
                        {
                            result.IsManifested = true;
                            dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            else
            {
                Console.Write("There is no record to create custom manifest.");
            }


            //return ManifestData;
        }

        public FrayteManifestExcel ManifestExcelWrite(List<FrayteManifestOnExcel> ManifestData)
        {
            FrayteManifestExcel FMEx = new FrayteManifestExcel();
            //CSV Writesss
            try
            {
                var ManifestFileName = CreateManifest(ManifestData);
                //File.Create(@"D:\ProjectFrayte\Frayte\Frayte.WebApi\Manifestedshipments\" + CreateManifest(ManifestData) + ".csv");
                //StreamWriter sw = new StreamWriter(@"D:\ProjectFrayte\Frayte\Frayte.WebApi\Manifestedshipments\ManifestTest\" + ManifestFileName + ".csv", false);
                StreamWriter sw = new StreamWriter(AppSettings.ManifestFolderPath + ManifestFileName + ".csv", false);
                Type type = typeof(FrayteManifestOnExcel);
                int count = type.GetProperties().Length;
                for (var k = 0; k < count; k++)
                {
                    var pro = typeof(FrayteManifestOnExcel).GetProperties();
                    sw.Write(pro[k].Name);
                    if (k < count)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);

                // Now write all the rows.
                //foreach (var dr in ManifestData)
                //{
                //var p = typeof(FrayteManifestOnExcel).GetProperties();
                for (var l = 0; l < ManifestData.Count; l++)
                {
                    sw.Write(ManifestData[l].ECommerceShipmentId);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].TrackingNumber == null ? "" : ManifestData[l].TrackingNumber);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].Reference == null ? "" : ManifestData[l].Reference));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].InternalAccountNumber);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ShipperName == null ? "" : ManifestData[l].ShipperName));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ShipperAddress1 == null ? "" : ManifestData[l].ShipperAddress1));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    string shipperAddress2 = ChangeStringCommatoSpace(ManifestData[l].ShipperAddress2 == null ? "" : ManifestData[l].ShipperAddress2);
                    sw.Write(shipperAddress2);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    //sw.Write(ChangeStringCommatoSpace(ManifestData[l].ShipperAddress3));
                    //if (l < ManifestData.Count)
                    //    sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ShipperCity == null ? "" : ManifestData[l].ShipperCity));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].ShipperZip == null ? "" : ManifestData[l].ShipperZip);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ShipperState == null ? "" : ManifestData[l].ShipperState));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].ShipperPhoneNo == null ? "" : ManifestData[l].ShipperPhoneNo);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ShipperEmail == null ? "" : ManifestData[l].ShipperEmail));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].ShipperCountryCode == null ? "" : ManifestData[l].ShipperCountryCode);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneeName == null ? "" : ManifestData[l].ConsigneeName));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneeAddress1 == null ? "" : ManifestData[l].ConsigneeAddress1));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneeAddress2 == null ? "" : ManifestData[l].ConsigneeAddress2));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    //sw.Write(ManifestData[l].ConsigneeAddress3);
                    //if (l <= ManifestData.Count)
                    //    sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneeCity == null ? "" : ManifestData[l].ConsigneeCity));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].ConsigneeZip);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    string state = ChangeStringCommatoSpace(ManifestData[l].ConsigneeState == null ? "" : ManifestData[l].ConsigneeState);
                    sw.Write(state);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneePhoneNo == null ? "" : ManifestData[l].ConsigneePhoneNo));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneeEmail == null ? "" : ManifestData[l].ConsigneeEmail));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneeCountryCode == null ? "" : ManifestData[l].ConsigneeCountryCode));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].Pieces);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].TotalWeight);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].WeightUOM == null ? "" : ManifestData[l].WeightUOM);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].TotalValue);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].Currency == null ? "" : ManifestData[l].Currency));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].Incoterms == null ? "" : ManifestData[l].Incoterms));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ItemDescription == null ? "" : ManifestData[l].ItemDescription));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ItemHScodes == null ? "" : ManifestData[l].ItemHScodes));
                    //if (l <= ManifestData.Count)
                    //    sw.Write(",");
                    //sw.Write(ChangeStringCommatoSpace(ManifestData[l].ItemQuantity));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].ItemValue);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].CustomCommodityMap == null ? "" : ManifestData[l].CustomCommodityMap);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].CustomEntryType == null ? "" : ManifestData[l].CustomEntryType);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].CustomTotalValue);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].CustomTotalVAT);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].CustomDuty);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].EstimatedDateofArrival);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].EstimatedTimeofArrival);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].EstimatedDateofDelivery);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].EstimatedTimeofDelivery);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(sw.NewLine);
                }
                //sw.Write(sw.NewLine);
                //}

                //File.Copy(@"D:\ProjectFrayte\Frayte\Frayte.WebApi\Manifestedshipments\ManifestTest\" + ManifestFileName + ".csv", @"D:\ProjectFrayte\Frayte\Frayte.WebApi\Manifestedshipments\ManifestTestCopy\" + ManifestFileName + ".csv");
                sw.Close();


                //Microsoft.Office.Interop.Excel.Application excel;
                //Microsoft.Office.Interop.Excel.Workbook worKbooK;
                //Microsoft.Office.Interop.Excel.Worksheet worKsheeT;
                //Microsoft.Office.Interop.Excel.Range celLrangE;


                //excel = new Microsoft.Office.Interop.Excel.Application();
                //worKbooK = excel.Workbooks.Add(Type.Missing);
                //worKsheeT = (Microsoft.Office.Interop.Excel.Worksheet)worKbooK.ActiveSheet;
                //worKsheeT.Name = "ManifestShipment";
                //worKsheeT.Cells.Font.Size = 15;
                //int rowcount = 2;

                //worKsheeT.Cells[1, 1] = "TrackingNo";
                //worKsheeT.Cells[1, 2] = "Reference";
                //worKsheeT.Cells[1, 3] = "InternalAccount No";
                //worKsheeT.Cells[1, 4] = "ShipperName";
                //worKsheeT.Cells[1, 5] = "ShiperAddress1";
                //worKsheeT.Cells[1, 6] = "ShipAddress2";
                //worKsheeT.Cells[1, 7] = "ShipperCity";
                //worKsheeT.Cells[1, 8] = "ShipperState";
                //worKsheeT.Cells[1, 9] = "ShipperZip";
                //worKsheeT.Cells[1, 10] = "ShipperPhone";
                //worKsheeT.Cells[1, 11] = "ShipperEmail";
                //worKsheeT.Cells[1, 12] = "ShipperCountry";
                //worKsheeT.Cells[1, 13] = "ConsigneeName";
                //worKsheeT.Cells[1, 14] = "ConsigneeAddress1";
                //worKsheeT.Cells[1, 15] = "ConsigneeAddress2";
                //worKsheeT.Cells[1, 16] = "ConsigneeCity";
                //worKsheeT.Cells[1, 17] = "ConsigneeZip";
                //worKsheeT.Cells[1, 18] = "ConsigneeState";
                //worKsheeT.Cells[1, 19] = "ConsigneePhone";
                //worKsheeT.Cells[1, 20] = "ConsigneeEmail";
                //worKsheeT.Cells[1, 21] = "ConsigneeCountry";
                //worKsheeT.Cells[1, 22] = "Pices";
                //worKsheeT.Cells[1, 23] = "TotalWeight";
                //worKsheeT.Cells[1, 24] = "WeightUOM";
                //worKsheeT.Cells[1, 25] = "TotalValue";
                //worKsheeT.Cells[1, 26] = "Currency";
                //worKsheeT.Cells[1, 27] = "Incoterms";
                //worKsheeT.Cells[1, 28] = "ItemDescription";
                //worKsheeT.Cells[1, 29] = "ItemHScodes ";
                //worKsheeT.Cells[1, 30] = "ItemQuantity";
                //worKsheeT.Cells[1, 31] = "ItemValue";
                //worKsheeT.Cells[1, 32] = "CustomCommoditymap";
                //worKsheeT.Cells[1, 33] = "CustomsEntryType";
                //worKsheeT.Cells[1, 34] = "CustomsTotalValue";
                //worKsheeT.Cells[1, 35] = "CustomsTotalVat";
                //worKsheeT.Cells[1, 36] = "CustomsDuty";


                //int j = 0;
                //int i = 0;
                //for (var k = 0; k < ManifestData.Count; k++)
                //{
                //    worKsheeT.Cells[i + 2, j + 1] = ManifestData[i].TrackingNumber;
                //    worKsheeT.Cells[i + 2, j + 2] = ManifestData[i].Reference;
                //    worKsheeT.Cells[i + 2, j + 3] = ManifestData[i].InternalAccountNumber;
                //    worKsheeT.Cells[i + 2, j + 4] = ManifestData[i].ShipperName;
                //    worKsheeT.Cells[i + 2, j + 5] = ManifestData[i].ShipperAddress1;
                //    worKsheeT.Cells[i + 2, j + 6] = ManifestData[i].ShipperAddress2;
                //    worKsheeT.Cells[i + 2, j + 7] = ManifestData[i].ShipperCity;
                //    worKsheeT.Cells[i + 2, j + 8] = ManifestData[i].ShipperState;
                //    worKsheeT.Cells[i + 2, j + 9] = ManifestData[i].ShipperZip;
                //    worKsheeT.Cells[i + 2, j + 10] = ManifestData[i].ShipperPhoneNo;
                //    worKsheeT.Cells[i + 2, j + 11] = ManifestData[i].ShipperEmail;
                //    worKsheeT.Cells[i + 2, j + 12] = ManifestData[i].ShipperCountryCode;
                //    worKsheeT.Cells[i + 2, j + 13] = ManifestData[i].ConsigneeName;
                //    worKsheeT.Cells[i + 2, j + 14] = ManifestData[i].ConsigneeAddress1;
                //    worKsheeT.Cells[i + 2, j + 15] = ManifestData[i].ConsigneeAddress2;
                //    worKsheeT.Cells[i + 2, j + 16] = ManifestData[i].ConsigneeCity;
                //    worKsheeT.Cells[i + 2, j + 17] = ManifestData[i].ConsigneeZip;
                //    worKsheeT.Cells[i + 2, j + 18] = ManifestData[i].ConsigneeState;
                //    worKsheeT.Cells[i + 2, j + 19] = ManifestData[i].ConsigneePhoneNo;
                //    worKsheeT.Cells[i + 2, j + 20] = ManifestData[i].ConsigneeEmail;
                //    worKsheeT.Cells[i + 2, j + 21] = ManifestData[i].ConsigneeCountryCode;
                //    worKsheeT.Cells[i + 2, j + 22] = ManifestData[i].Pieces;
                //    worKsheeT.Cells[i + 2, j + 23] = ManifestData[i].TotalWeight;
                //    worKsheeT.Cells[i + 2, j + 24] = ManifestData[i].WeightUOM;
                //    worKsheeT.Cells[i + 2, j + 25] = ManifestData[i].TotalValue;
                //    worKsheeT.Cells[i + 2, j + 26] = ManifestData[i].Currency;
                //    worKsheeT.Cells[i + 2, j + 27] = ManifestData[i].Incoterms;
                //    worKsheeT.Cells[i + 2, j + 28] = ManifestData[i].ItemDescription;
                //    worKsheeT.Cells[i + 2, j + 29] = ManifestData[i].ItemHScodes;
                //    worKsheeT.Cells[i + 2, j + 30] = ManifestData[i].ItemQuantity;
                //    worKsheeT.Cells[i + 2, j + 31] = ManifestData[i].ItemValue;
                //    i++;
                //}

                //celLrangE = worKsheeT.Range[worKsheeT.Cells[1, 1], worKsheeT.Cells[rowcount, ManifestData.Count]];
                //celLrangE.EntireColumn.AutoFit();
                //Microsoft.Office.Interop.Excel.Borders border = celLrangE.Borders;
                //border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                //border.Weight = 2d;

                //celLrangE = worKsheeT.Range[worKsheeT.Cells[1, 1], worKsheeT.Cells[2, ManifestData.Count]];

                //var ManifestFileName = CreateManifest(ManifestData);

                //worKbooK.SaveAs("D:\\tested1\\" + ManifestFileName + ".csv", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                //                 false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                //                 Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                //string directoryPath = Path.GetDirectoryName("D:\\tested2\\vik1.xlsx");

                // If directory doesn't exist create one
                //if (!Directory.Exists(directoryPath))
                //{
                //    DirectoryInfo di = Directory.CreateDirectory(directoryPath);
                //}
                //File.Copy("D:\\tested1\\" + ManifestFileName + ".csv", "D:\\tested2\\" + ManifestFileName + ".csv");
                //FileInfo fi = new FileInfo("D:\\test\\vik.xlsx");
                //fi.CopyTo("D:\\tested\\vik1.xlsx", true);
                //worKbooK.Close();
                //excel.Quit();
                FMEx.Message = "True";
            }
            catch (Exception ex)
            {

                FMEx.Message = "False";
            }

            return FMEx;
        }

        public string CreateManifest(List<FrayteManifestOnExcel> ManifestData)
        {
            string ManifestName = "";
            //FrayteResult result = new FrayteResult();
            try
            {
                // group the shipments based on CourierCompany 
                //var dasta = manifestShipments.where(p => p.eCommerceShipmentId).Distinct().ToList();


                eCommerceCustomManifest manifest = new eCommerceCustomManifest();

                manifest.ManifestName = "";
                manifest.CreatedOn = DateTime.UtcNow;
                manifest.ModuleType = "eCommerce";

                dbContext.eCommerceCustomManifests.Add(manifest);
                dbContext.SaveChanges();

                //"MNUK-170300001"

                manifest.ManifestName = "MNUKCUS" + GetFormattedManifestId(manifest.CustomManifestId);
                ManifestName = "MNUKCUS" + GetFormattedManifestId(manifest.CustomManifestId);

                dbContext.Entry(manifest).State = System.Data.Entity.EntityState.Modified;
                // dbContext.SaveChanges();
                eCommerceShipment edata = new eCommerceShipment();
                foreach (var mData in ManifestData)
                {
                    var result = dbContext.eCommerceShipments.Where(a => a.eCommerceShipmentId == mData.ECommerceShipmentId).FirstOrDefault();
                    if (result != null)
                    {
                        result.CustomManifestId = manifest.CustomManifestId;
                        //dbContext.eCommerceShipments.Add(edata);
                        dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;

                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                //result.Status = false;
            }
            return ManifestName;
        }

        private string GetFormattedManifestId(int manifestId)
        {

            string manifestName = string.Empty;
            if (manifestId.ToString().Length == 1)
            {
                manifestName = "000" + manifestId.ToString();
            }
            else if (manifestId.ToString().Length == 2)
            {
                manifestName = "00" + manifestId.ToString();
            }
            else if (manifestId.ToString().Length == 3)
            {
                manifestName = "0" + manifestId.ToString();
            }
            else
            {
                manifestName = manifestId.ToString();
            }

            return manifestName;
        }

        public FrayteManifestExcel GetManifestDetailFromExcel()
        {
            FrayteManifestExcel frayteManifestDetailexcel = new Services.Models.FrayteManifestExcel();

            //var httpRequest = HttpContext.Current.Request;

            List<Frayte.Services.Models.FrayteManifestOnExcel> _Manifestdetail = new List<Frayte.Services.Models.FrayteManifestOnExcel>();
            //Frayte.Services.Models.FrayteManifestOnExcel frayteManifestDetailexcel;






            //int ShipmentId = Convert.ToInt32(httpRequest.Form["ShipmentId"].ToString());
            string connString = "";
            //string filename = DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss_") + file.FileName;
            //var filepath = HttpContext.Current.Server.MapPath(Utility.AppSettings.ManifestFolder);
            //var FilePaths = Directory.EnumerateFiles(Utility.AppSettings.ManifestFolder);
            var filePaths = Directory.GetFiles(Utility.AppSettings.ManifestFolder);


            foreach (var file in filePaths)
            {

                //StreamReader reader = new StreamReader(File.OpenRead(file));
                //List<string> listA = new List<String>();
                //List<string> listB = new List<String>();
                //List<string> listC = new List<String>();
                //List<string> listD = new List<String>();
                ////string vara1, vara2, vara3, vara4;
                //while (!reader.EndOfStream)
                //{
                //    string line = reader.ReadLine();
                //    if (!String.IsNullOrWhiteSpace(line))
                //    {
                //        string[] values = line.Split(',');
                //        if (values.Length >= 3)
                //        {
                //            listA.Add(values[0]);
                //            listB.Add(values[1]);
                //            listC.Add(values[2]);
                //            //listD.Add(values[3]);
                //        }
                //    }
                //}
                //string[] firstlistA = listA.ToArray();
                //string[] firstlistB = listB.ToArray();
                //string[] firstlistC = listC.ToArray();
                //string[] firstlistD = listD.ToArray();

                //file.Normalize();
                //string contents = File.ReadAllText(file);
                var filepath = file;
                var filelen = file.Length - 15;
                //var sublenget = filelen5;
                //string filename = file.Substring(filelen, 15);
                var filePathLength = file.Length;
                var FileName = file.Split('\\');
                var FileNameLength = FileName.Length;
                var filename = FileName[FileNameLength - 1];
                //StreamReader sr = new StreamReader(filepath);
                //string Fulltext = sr.ReadToEnd();
                //string filename = "MNUKCUS0006";
                connString = new DirectShipmentRepository().getExcelConnectionString(filename, filepath);
                string fileExtension = "";
                fileExtension = new DirectShipmentRepository().getFileExtensionString(filename);
                try
                {
                    if (!string.IsNullOrEmpty(fileExtension))
                    {

                        var ds = new DataSet();
                        if (fileExtension == FrayteFileExtension.CSV)
                        {
                            try
                            {
                                using (var conn = new OleDbConnection(connString))
                                {
                                    conn.Open();
                                    var query = "SELECT * FROM [" + Path.GetFileName(filename) + "]";
                                    using (var adapter = new OleDbDataAdapter(query, conn))
                                    {
                                        adapter.Fill(ds, "CustomManifest");
                                    }
                                }

                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        else
                        {

                            using (var conn = new OleDbConnection(connString))
                            {
                                conn.Open();
                                DataTable dbSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string firstSheetName = dbSchema.Rows[0]["TABLE_NAME"].ToString();
                                var query = "SELECT * FROM " + "[" + firstSheetName + "]";//[Sheet1$]";
                                using (var adapter = new OleDbDataAdapter(query, conn))
                                {
                                    adapter.Fill(ds, "CustomManifest");
                                }
                            }
                        }



                        if (ds.Tables.Count > 0)
                        {
                            var exceldata = ds.Tables[0];
                            string PiecesColumnList = "CustomCommoditymap,CustomsEntryType,CustomsTotalValue,CustomsTotalVat,CustomsDuty";
                            //bool IsExcelValid = UtilityRepository.CheckUploadExcelFormat(PiecesColumnList, exceldata);
                            bool IsExcelValid = true;
                            if (!IsExcelValid)
                            {
                                frayteManifestDetailexcel.Message = "Columns are not matching with provided template columns. Please check the column names.";
                            }
                            else
                            {
                                if (exceldata.Rows.Count > 0)
                                {
                                    _Manifestdetail = new eCommerceShipmentRepository().SaveCustomManifestDetail(exceldata);
                                    frayteManifestDetailexcel.FrayteManifestDetail = new List<Frayte.Services.Models.FrayteManifestOnExcel>();
                                    frayteManifestDetailexcel.FrayteManifestDetail = _Manifestdetail;
                                    frayteManifestDetailexcel.Message = "OK";
                                }

                                else
                                {
                                    frayteManifestDetailexcel.Message = "No records found.";
                                }
                            }
                        }
                    }
                    else
                    {
                        frayteManifestDetailexcel.Message = "Excel file not valid";
                    }

                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    if (ex != null && !string.IsNullOrEmpty(ex.Message) && ex.Message.Contains("Sheet1$"))
                    {
                        frayteManifestDetailexcel.Message = "Sheet name is invalid.";
                    }
                    else
                    {
                        frayteManifestDetailexcel.Message = "Error while uploading the excel.";
                    }
                    return frayteManifestDetailexcel;

                }
            }

            return frayteManifestDetailexcel;
        }

        public string ChangeStringCommatoSpace(string res)
        {
            string a = "";
            var result = res.Split(',');
            for (int i = 0; i < result.Length; i++)
            {
                a = a + result[i];
            }
            return a;
        }
        #endregion
    }
}
