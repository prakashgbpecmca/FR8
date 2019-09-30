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
using System.IO;

namespace Report.Generator.ManifestReport
{
    public class BaseRateReport
    {
        public FrayteManifestName ExportBaseRateExcel(int LogisticServiceId, int Year)
        {
            FrayteManifestName result = new FrayteManifestName();

            var filename = new ViewBaseRateCardRepository().GetFileName(LogisticServiceId, Year);
            if (filename != null)
            {
                result.FileName = filename;
                result.FilePath = AppSettings.WebApiPath + "ReportFiles/BaseRateCard/" + filename;
            }
            else
            {
                var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

                if (logisticServices != null)
                {
                    if (logisticServices.OperationZoneId == 1)
                    {
                        switch (logisticServices.LogisticCompany)
                        {
                            case FrayteCourierCompany.DHL:
                                switch (logisticServices.LogisticType)
                                {
                                    case FrayteLogisticType.Import:
                                        switch (logisticServices.RateType)
                                        {
                                            case FrayteLogisticRateType.Express:
                                                result = HKDHLImportExpress(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                            case FrayteLogisticRateType.Economy:
                                                result.FileStatus = false;
                                                break;
                                        }
                                        break;
                                    case FrayteLogisticType.Export:
                                        switch (logisticServices.RateType)
                                        {
                                            case FrayteLogisticRateType.Express:
                                                result = HKDHLExportExpress(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                            case FrayteLogisticRateType.Economy:
                                                result.FileStatus = false;
                                                break;
                                        }
                                        break;
                                    case FrayteLogisticType.ThirdParty:
                                        switch (logisticServices.RateType)
                                        {
                                            case FrayteLogisticRateType.Express:
                                                result = HKDHL3rdPartyExpress(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                            case FrayteLogisticRateType.Economy:
                                                result.FileStatus = false;
                                                break;
                                        }
                                        break;
                                }
                                break;
                            case FrayteCourierCompany.TNT:
                                switch (logisticServices.LogisticType)
                                {
                                    case FrayteLogisticType.Import:
                                        switch (logisticServices.RateType)
                                        {
                                            case FrayteLogisticRateType.Express:
                                                result = HKTNTImportExpress(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                            case FrayteLogisticRateType.Economy:
                                                result = HKTNTImportEconomy(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                        }
                                        break;
                                    case FrayteLogisticType.Export:
                                        switch (logisticServices.RateType)
                                        {
                                            case FrayteLogisticRateType.Express:
                                                result = HKTNTExportExpress(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                            case FrayteLogisticRateType.Economy:
                                                result = HKTNTExportEconomy(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                        }
                                        break;
                                    case FrayteLogisticType.ThirdParty:
                                        switch (logisticServices.RateType)
                                        {
                                            case FrayteLogisticRateType.Express:
                                                result.FileStatus = false;
                                                break;
                                            case FrayteLogisticRateType.Economy:
                                                result.FileStatus = false;
                                                break;
                                        }
                                        break;
                                }
                                break;
                            case FrayteCourierCompany.UPS:
                                switch (logisticServices.LogisticType)
                                {
                                    case FrayteLogisticType.Import:
                                        switch (logisticServices.RateType)
                                        {
                                            case FrayteLogisticRateType.SaverLanes:
                                                result = HKUPSImportSaverLanes(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                            case FrayteLogisticRateType.ExpressLanes:
                                                result = HKUPSImportExpressLanes(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                            case FrayteLogisticRateType.Saver:
                                                result = HKUPSImportSaver(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                            case FrayteLogisticRateType.Express:
                                                result = HKUPSImportExpress(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                            case FrayteLogisticRateType.Expedited:
                                                result = HKUPSImportExpedited(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                        }
                                        break;
                                    case FrayteLogisticType.Export:
                                        switch (logisticServices.RateType)
                                        {
                                            case FrayteLogisticRateType.SaverLanes:
                                                result = HKUPSExportSaverLanes(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                            case FrayteLogisticRateType.ExpressLanes:
                                                result = HKUPSExportExpressLanes(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                            case FrayteLogisticRateType.Saver:
                                                result = HKUPSExportSaver(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                            case FrayteLogisticRateType.Express:
                                                result = HKUPSExportExpress(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                            case FrayteLogisticRateType.Expedited:
                                                result = HKUPSExportExpedited(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                        }
                                        break;
                                }
                                break;
                        }
                    }
                    else if (logisticServices.OperationZoneId == 2)
                    {
                        switch (logisticServices.LogisticCompany)
                        {
                            case FrayteCourierCompany.UKMail:
                                switch (logisticServices.LogisticType)
                                {
                                    case FrayteLogisticType.UKShipment:
                                        result = UKUKMail(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                        break;
                                }
                                break;
                            case FrayteCourierCompany.Yodel:
                                switch (logisticServices.LogisticType)
                                {
                                    case FrayteLogisticType.UKShipment:
                                        result = UKYodel(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                        break;
                                }
                                break;
                            case FrayteCourierCompany.Hermes:
                                switch (logisticServices.LogisticType)
                                {
                                    case FrayteLogisticType.UKShipment:
                                        result = UKHermes(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                        break;
                                }
                                break;
                            case FrayteCourierCompany.DHL:
                                switch (logisticServices.LogisticType)
                                {
                                    case FrayteLogisticType.UKShipment:
                                        result = UKDHLDomestic(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                        break;
                                    case FrayteLogisticType.Import:
                                        switch (logisticServices.RateType)
                                        {
                                            case FrayteLogisticRateType.Express:
                                                result = UKDHLImportExpress(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                            case FrayteLogisticRateType.Economy:
                                                result = UKDHLImportEconomy(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                        }
                                        break;
                                    case FrayteLogisticType.Export:
                                        switch (logisticServices.RateType)
                                        {
                                            case FrayteLogisticRateType.Express:
                                                result = UKDHLExportExpress(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                            case FrayteLogisticRateType.Economy:
                                                result = UKDHLExportEconomy(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                        }
                                        break;
                                    case FrayteLogisticType.ThirdParty:
                                        switch (logisticServices.RateType)
                                        {
                                            case FrayteLogisticRateType.Express:
                                                result = UKDHL3rdPartyExpress(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                            case FrayteLogisticRateType.Economy:
                                                result = UKDHL3rdPartyEconomy(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                        }
                                        break;
                                }
                                break;
                            case FrayteCourierCompany.TNT:
                                switch (logisticServices.LogisticType)
                                {
                                    case FrayteLogisticType.Import:
                                        switch (logisticServices.RateType)
                                        {
                                            case FrayteLogisticRateType.Express:
                                                result = UKTNTImportExpress(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                            case FrayteLogisticRateType.Economy:
                                                result = UKTNTImportEconomy(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                        }
                                        break;
                                    case FrayteLogisticType.Export:
                                        switch (logisticServices.RateType)
                                        {
                                            case FrayteLogisticRateType.Express:
                                                result = UKTNTExportExpress(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                            case FrayteLogisticRateType.Economy:
                                                result = UKTNTExportEconomy(logisticServices.LogisticServiceId, logisticServices.OperationZoneId);
                                                break;
                                        }
                                        break;
                                    case FrayteLogisticType.ThirdParty:
                                        switch (logisticServices.RateType)
                                        {
                                            case FrayteLogisticRateType.Express:
                                                result.FileStatus = false;
                                                break;
                                            case FrayteLogisticRateType.Economy:
                                                result.FileStatus = false;
                                                break;
                                        }
                                        break;
                                }
                                break;
                        }
                    }
                }
                else
                {
                    result.FileStatus = false;
                }
            }
            return result;
        }

        internal FrayteManifestName HKDHLImportExpress(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.HKRateCards.DHLImportExpress report = new ReportTemplate.HKRateCards.DHLImportExpress();
                report.Parameters["lblHeader"].Value = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "HKD";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - HKD" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName HKDHLExportExpress(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.HKRateCards.DHLExportExpress report = new ReportTemplate.HKRateCards.DHLExportExpress();
                report.Parameters["lblHeader"].Value = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "HKD";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - HKD" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }
            return result;
        }

        internal FrayteManifestName HKDHL3rdPartyExpress(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);
            var matrix = new ThirdPartyMarixRepository().GetThirdPartyMatrix(LogisticServiceId);
            var zone = new ThirdPartyMarixRepository().GetZoneDetail(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteZoneMatrix> _matrix = new List<FrayteZoneMatrix>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 12)
                            {
                                rate.Rate13 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 12)
                            {
                                rate.Rate13 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 12)
                            {
                                rate.Rate13 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region ThirdParty Matrix

            var tt = matrix
                    .GroupBy(p => new { p.FromZoneDisplay })
                    .Select(g => new
                    {
                        FromZone = g.Select(p => new { p.FromZoneDisplay }).First(),
                        ApplyZone = g.Select(p => new { p.ApplyZoneDisplay }).ToList()
                    }).ToList();

            FrayteZoneMatrix zonematrix;
            for (int i = 0; i < tt.Count; i++)
            {
                zonematrix = new FrayteZoneMatrix();
                zonematrix.FromZone = tt[i].FromZone.FromZoneDisplay;
                zonematrix.ApplyZone = new List<FrayteApplyZone>();
                FrayteApplyZone apply = new FrayteApplyZone();
                for (int j = 0; j < tt[i].ApplyZone.Count; j++)
                {
                    if (j == 0)
                    {
                        apply.Zone1 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 1)
                    {
                        apply.Zone2 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 2)
                    {
                        apply.Zone3 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 3)
                    {
                        apply.Zone4 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 4)
                    {
                        apply.Zone5 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 5)
                    {
                        apply.Zone6 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 6)
                    {
                        apply.Zone7 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 7)
                    {
                        apply.Zone8 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 8)
                    {
                        apply.Zone9 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 9)
                    {
                        apply.Zone10 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 10)
                    {
                        apply.Zone11 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 11)
                    {
                        apply.Zone12 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 12)
                    {
                        apply.Zone13 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    zonematrix.ApplyZone.Add(apply);
                }
                _matrix.Add(zonematrix);
            }

            #endregion

            #region Third Party Zone List

            int n = 1;
            List<FrayteApplyZone> fzone = new List<FrayteApplyZone>();
            FrayteApplyZone _zone = new FrayteApplyZone();

            foreach (FrayteZone ff in zone)
            {
                if (n == 1)
                {
                    _zone.Zone1 = ff.ZoneRateName;
                    _zone.TPZone1 = ff.ZoneDisplayName;
                }
                if (n == 2)
                {
                    _zone.Zone2 = ff.ZoneRateName;
                    _zone.TPZone2 = ff.ZoneDisplayName;
                }
                if (n == 3)
                {
                    _zone.Zone3 = ff.ZoneRateName;
                    _zone.TPZone3 = ff.ZoneDisplayName;
                }
                if (n == 4)
                {
                    _zone.Zone4 = ff.ZoneRateName;
                    _zone.TPZone4 = ff.ZoneDisplayName;
                }
                if (n == 5)
                {
                    _zone.Zone5 = ff.ZoneRateName;
                    _zone.TPZone5 = ff.ZoneDisplayName;
                }
                if (n == 6)
                {
                    _zone.Zone6 = ff.ZoneRateName;
                    _zone.TPZone6 = ff.ZoneDisplayName;
                }
                if (n == 7)
                {
                    _zone.Zone7 = ff.ZoneRateName;
                    _zone.TPZone7 = ff.ZoneDisplayName;
                }
                if (n == 8)
                {
                    _zone.Zone8 = ff.ZoneRateName;
                    _zone.TPZone8 = ff.ZoneDisplayName;
                }
                if (n == 9)
                {
                    _zone.Zone9 = ff.ZoneRateName;
                    _zone.TPZone9 = ff.ZoneDisplayName;
                }
                if (n == 10)
                {
                    _zone.Zone10 = ff.ZoneRateName;
                    _zone.TPZone10 = ff.ZoneDisplayName;
                }
                if (n == 11)
                {
                    _zone.Zone11 = ff.ZoneRateName;
                    _zone.TPZone11 = ff.ZoneDisplayName;
                }
                if (n == 12)
                {
                    _zone.Zone12 = ff.ZoneRateName;
                    _zone.TPZone12 = ff.ZoneDisplayName;
                }
                if (n == 13)
                {
                    _zone.Zone13 = ff.ZoneRateName;
                    _zone.TPZone13 = ff.ZoneDisplayName;
                }
                n++;
            }
            fzone.Add(_zone);

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            zonecountry.ThirdParty = new List<FrayteZoneMatrix>();
            zonecountry.ThirdParty = _matrix;
            zonecountry.Zone = new List<FrayteApplyZone>();
            zonecountry.Zone = fzone;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.HKRateCards.DHL3rdPartyExpress report = new ReportTemplate.HKRateCards.DHL3rdPartyExpress();
                report.Parameters["lblHeader"].Value = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "HKD";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblThirdParty"].Value = "DHL 3rd Party Zone Matrix " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - HKD" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }
            return result;
        }

        internal FrayteManifestName HKTNTImportExpress(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(logisticServices.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.HKRateCards.TNTImportExpress report = new ReportTemplate.HKRateCards.TNTImportExpress();
                report.Parameters["lblHeader"].Value = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "HKD";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - HKD" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName HKTNTImportEconomy(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(logisticServices.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            FrayteBaseRate rate;

            #region NondocRate

            if (Nondocitem != null && Nondocitem.Count > 0)
            {
                var DocNonship = Nondocitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = Nondocitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCard/"));
                ReportTemplate.HKRateCards.TNTImportEconomy report = new ReportTemplate.HKRateCards.TNTImportEconomy();
                report.Parameters["lblHeader"].Value = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "HKD";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - HKD" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName HKTNTExportExpress(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(logisticServices.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion            

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCard/"));
                ReportTemplate.HKRateCards.TNTExportExpress report = new ReportTemplate.HKRateCards.TNTExportExpress();
                report.Parameters["lblHeader"].Value = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "HKD";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - HKD" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName HKTNTExportEconomy(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(logisticServices.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            FrayteBaseRate rate;

            #region NondocRate

            if (Nondocitem != null && Nondocitem.Count > 0)
            {
                var DocNonship = Nondocitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = Nondocitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCard/"));
                ReportTemplate.HKRateCards.TNTExportEconomy report = new ReportTemplate.HKRateCards.TNTExportEconomy();
                report.Parameters["lblHeader"].Value = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "HKD";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - HKD" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName HKUPSImportSaverLanes(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.HKRateCards.UPSImportSaverLanes report = new ReportTemplate.HKRateCards.UPSImportSaverLanes();
                report.Parameters["lblHeader"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "HKD";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateTypeDisplay + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - HKD" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName HKUPSImportExpressLanes(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.HKRateCards.UPSImportExpressLanes report = new ReportTemplate.HKRateCards.UPSImportExpressLanes();
                report.Parameters["lblHeader"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "HKD";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateTypeDisplay + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - HKD" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName HKUPSImportSaver(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.HKRateCards.UPSImportSaver report = new ReportTemplate.HKRateCards.UPSImportSaver();
                report.Parameters["lblHeader"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "HKD";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - HKD" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName HKUPSImportExpress(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.HKRateCards.UPSImportExpress report = new ReportTemplate.HKRateCards.UPSImportExpress();
                report.Parameters["lblHeader"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "HKD";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - HKD" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName HKUPSImportExpedited(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.HKRateCards.UPSImportExpedited report = new ReportTemplate.HKRateCards.UPSImportExpedited();
                report.Parameters["lblHeader"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "HKD";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - HKD" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName HKUPSExportSaverLanes(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.HKRateCards.UPSExportSaverLanes report = new ReportTemplate.HKRateCards.UPSExportSaverLanes();
                report.Parameters["lblHeader"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "HKD";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateTypeDisplay + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - HKD" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName HKUPSExportExpressLanes(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.HKRateCards.UPSExportExpressLanes report = new ReportTemplate.HKRateCards.UPSExportExpressLanes();
                report.Parameters["lblHeader"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "HKD";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateTypeDisplay + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - HKD" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName HKUPSExportSaver(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.HKRateCards.UPSExportSaver report = new ReportTemplate.HKRateCards.UPSExportSaver();
                report.Parameters["lblHeader"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "HKD";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - HKD" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName HKUPSExportExpress(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.HKRateCards.UPSExportExpress report = new ReportTemplate.HKRateCards.UPSExportExpress();
                report.Parameters["lblHeader"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "HKD";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - HKD" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName HKUPSExportExpedited(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.HKRateCards.UPSExportExpedited report = new ReportTemplate.HKRateCards.UPSExportExpedited();
                report.Parameters["lblHeader"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "HKD";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "UPS " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - HKD" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName UKDHLImportExpress(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var addon = new CustomerRepository().GetAddOnRate(LogisticServiceId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteAddOnRate add;

            #region AddOnRate

            if (addon != null && addon.Count > 0)
            {
                var addonweight = addon.GroupBy(x => new { x.WeightFrom, x.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.AddOnRate }).ToList() }).ToList();
                for (int i = 0; i < addonweight.Count; i++)
                {
                    add = new FrayteAddOnRate();
                    add.ShipmentType = "Adder rate per additional 0.5 KG from 10.1 KG";
                    add.Weight = addonweight[i].weights.WeightFrom.ToString() + " - " + addonweight[i].weights.WeightTo.ToString();
                    for (int j = 0; j < addonweight[i].Rates.Count; j++)
                    {
                        if (j == 0)
                        {
                            add.Rate1 = addonweight[i].Rates[j].AddOnRate;
                        }
                        if (j == 1)
                        {
                            add.Rate2 = addonweight[i].Rates[j].AddOnRate;
                        }
                        if (j == 2)
                        {
                            add.Rate3 = addonweight[i].Rates[j].AddOnRate;
                        }
                        if (j == 3)
                        {
                            add.Rate4 = addonweight[i].Rates[j].AddOnRate;
                        }
                        if (j == 4)
                        {
                            add.Rate5 = addonweight[i].Rates[j].AddOnRate;
                        }
                        if (j == 5)
                        {
                            add.Rate6 = addonweight[i].Rates[j].AddOnRate;
                        }
                        if (j == 6)
                        {
                            add.Rate7 = addonweight[i].Rates[j].AddOnRate;
                        }
                        if (j == 7)
                        {
                            add.Rate8 = addonweight[i].Rates[j].AddOnRate;
                        }
                        if (j == 8)
                        {
                            add.Rate9 = addonweight[i].Rates[j].AddOnRate;
                        }
                        if (j == 9)
                        {
                            add.Rate10 = addonweight[i].Rates[j].AddOnRate;
                        }
                    }
                    _add.Add(add);
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.UKRateCards.DHLImportExpress report = new ReportTemplate.UKRateCards.DHLImportExpress();
                report.Parameters["lblHeader"].Value = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "GBP";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - GBP" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }
            return result;
        }

        internal FrayteManifestName UKDHLImportEconomy(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.ToString() + " - " + DocNondocweight[i].weights.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteAddOnRate add;

            #region AddOnRate

            if (addon != null && addon.Count > 0)
            {
                var addonweight = addon.GroupBy(x => new { x.WeightFrom, x.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.AddOnRate }).ToList() }).ToList();
                for (int i = 0; i < addonweight.Count; i++)
                {
                    add = new FrayteAddOnRate();
                    add.ShipmentType = "Adder rate per additional 0.5 KG from 10.1 KG";
                    add.Weight = addonweight[i].weights.WeightFrom.ToString() + " - " + addonweight[i].weights.WeightTo.ToString();
                    for (int j = 0; j < addonweight[i].Rates.Count; j++)
                    {
                        if (j == 0)
                        {
                            add.Rate1 = addonweight[i].Rates[j].AddOnRate;
                        }
                        if (j == 1)
                        {
                            add.Rate2 = addonweight[i].Rates[j].AddOnRate;
                        }
                        if (j == 2)
                        {
                            add.Rate3 = addonweight[i].Rates[j].AddOnRate;
                        }
                        if (j == 3)
                        {
                            add.Rate4 = addonweight[i].Rates[j].AddOnRate;
                        }
                        if (j == 4)
                        {
                            add.Rate5 = addonweight[i].Rates[j].AddOnRate;
                        }
                    }
                    _add.Add(add);
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            zonecountry.AddOnRate = new List<FrayteAddOnRate>();
            zonecountry.AddOnRate = _add;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.UKRateCards.DHLImportEconomy report = new ReportTemplate.UKRateCards.DHLImportEconomy();
                report.Parameters["lblHeader"].Value = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "GBP";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - GBP" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }
            return result;
        }

        internal FrayteManifestName UKDHLExportExpress(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteAddOnRate add;

            #region AddOnRate

            if (addon != null && addon.Count > 0)
            {
                var addonweight = addon.GroupBy(x => new { x.AddOnDescription })
                                       .Select(g => new
                                       {
                                           g.Key.AddOnDescription,
                                           weight = g.Select(p => new { p.WeightFrom, p.WeightTo, p.AddOnRate })
                                                     .GroupBy(y => new { y.WeightFrom, y.WeightTo })
                                                     .Select(h => new
                                                     {
                                                         weights = h.Key,
                                                         Rates = h.Select(p => new { p.AddOnRate }).ToList()
                                                     }).ToList()
                                       }).ToList();

                for (int i = 0; i < addonweight.Count; i++)
                {
                    for (int j = 0; j < addonweight[i].weight.Count; j++)
                    {
                        add = new FrayteAddOnRate();
                        add.ShipmentType = addonweight[i].AddOnDescription;
                        add.Weight = addonweight[i].weight[j].weights.WeightFrom.ToString() + " - " + addonweight[i].weight[j].weights.WeightTo.ToString();
                        for (int k = 0; k < addonweight[i].weight[j].Rates.Count; k++)
                        {
                            if (k == 0)
                            {
                                add.Rate1 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 1)
                            {
                                add.Rate2 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 2)
                            {
                                add.Rate3 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 3)
                            {
                                add.Rate4 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 4)
                            {
                                add.Rate5 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 5)
                            {
                                add.Rate6 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 6)
                            {
                                add.Rate7 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 7)
                            {
                                add.Rate8 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 8)
                            {
                                add.Rate9 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 9)
                            {
                                add.Rate10 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                        }
                        _add.Add(add);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            zonecountry.AddOnRate = new List<FrayteAddOnRate>();
            zonecountry.AddOnRate = _add;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.UKRateCards.DHLExportExpress report = new ReportTemplate.UKRateCards.DHLExportExpress();
                report.Parameters["lblHeader"].Value = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "GBP";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - GBP" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }
            return result;
        }

        internal FrayteManifestName UKDHLExportEconomy(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region HeavyweightRate

            if (HeavyWeightitem != null && HeavyWeightitem.Count > 0)
            {
                var Heavyship = HeavyWeightitem.Select(p => p.shipmentType.LogisticDescriptionDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Heavyweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = Heavyweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = Heavyweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteAddOnRate add;

            #region AddOnRate

            if (addon != null && addon.Count > 0)
            {
                var addonweight = addon.GroupBy(x => new { x.AddOnDescription })
                                       .Select(g => new
                                       {
                                           g.Key.AddOnDescription,
                                           weight = g.Select(p => new { p.WeightFrom, p.WeightTo, p.AddOnRate })
                                                     .GroupBy(y => new { y.WeightFrom, y.WeightTo })
                                                     .Select(h => new
                                                     {
                                                         weights = h.Key,
                                                         Rates = h.Select(p => new { p.AddOnRate }).ToList()
                                                     }).ToList()
                                       }).ToList();

                for (int i = 0; i < addonweight.Count; i++)
                {
                    for (int j = 0; j < addonweight[i].weight.Count; j++)
                    {
                        add = new FrayteAddOnRate();
                        add.ShipmentType = addonweight[i].AddOnDescription;
                        add.Weight = addonweight[i].weight[j].weights.WeightFrom.ToString() + " - " + addonweight[i].weight[j].weights.WeightTo.ToString();
                        for (int k = 0; k < addonweight[i].weight[j].Rates.Count; k++)
                        {
                            if (k == 0)
                            {
                                add.Rate1 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 1)
                            {
                                add.Rate2 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 2)
                            {
                                add.Rate3 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 3)
                            {
                                add.Rate4 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 4)
                            {
                                add.Rate5 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                        }
                        _add.Add(add);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            zonecountry.AddOnRate = new List<FrayteAddOnRate>();
            zonecountry.AddOnRate = _add;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.UKRateCards.DHLExportEconomy report = new ReportTemplate.UKRateCards.DHLExportEconomy();
                report.Parameters["lblHeader"].Value = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "GBP";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - GBP" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }
            return result;
        }

        internal FrayteManifestName UKDHL3rdPartyExpress(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);
            var matrix = new ThirdPartyMarixRepository().GetThirdPartyMatrix(LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(LogisticServiceId);
            var zone = new ThirdPartyMarixRepository().GetZoneDetail(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteZoneMatrix> _matrix = new List<FrayteZoneMatrix>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 9)
                            {
                                rate.Rate10 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 10)
                            {
                                rate.Rate11 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 11)
                            {
                                rate.Rate12 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteAddOnRate add;

            #region AddOnRate

            if (addon != null && addon.Count > 0)
            {
                var weightgroup = addon.GroupBy(p => new { p.WeightFrom, p.WeightTo }).Select(x => new { Weight = x.Key, Rates = x.Select(p => new { p.AddOnRate }).ToList() }).ToList();
                for (int i = 0; i < weightgroup.Count; i++)
                {
                    add = new FrayteAddOnRate();
                    add.ShipmentType = "Adder rate per 1KG from 30.1KG";
                    add.Weight = weightgroup[i].Weight.WeightFrom.ToString() + " +- " + weightgroup[i].Weight.WeightTo.ToString();
                    for (int j = 0; j < weightgroup[i].Rates.Count; j++)
                    {
                        if (j == 0)
                        {
                            add.Rate1 = weightgroup[i].Rates[j].AddOnRate;
                        }
                        if (j == 1)
                        {
                            add.Rate2 = weightgroup[i].Rates[j].AddOnRate;
                        }
                        if (j == 2)
                        {
                            add.Rate3 = weightgroup[i].Rates[j].AddOnRate;
                        }
                        if (j == 3)
                        {
                            add.Rate4 = weightgroup[i].Rates[j].AddOnRate;
                        }
                        if (j == 4)
                        {
                            add.Rate5 = weightgroup[i].Rates[j].AddOnRate;
                        }
                        if (j == 5)
                        {
                            add.Rate6 = weightgroup[i].Rates[j].AddOnRate;
                        }
                        if (j == 6)
                        {
                            add.Rate7 = weightgroup[i].Rates[j].AddOnRate;
                        }
                        if (j == 7)
                        {
                            add.Rate8 = weightgroup[i].Rates[j].AddOnRate;
                        }
                        if (j == 8)
                        {
                            add.Rate9 = weightgroup[i].Rates[j].AddOnRate;
                        }
                        if (j == 9)
                        {
                            add.Rate10 = weightgroup[i].Rates[j].AddOnRate;
                        }
                    }
                    _add.Add(add);
                }
            }

            #endregion

            #region ThirdParty Matrix

            var tt = matrix
                    .GroupBy(p => new { p.FromZoneDisplay })
                    .Select(g => new
                    {
                        FromZone = g.Select(p => new { p.FromZoneDisplay }).First(),
                        ApplyZone = g.Select(p => new { p.ApplyZoneDisplay }).ToList()
                    }).ToList();

            FrayteZoneMatrix zonematrix;
            for (int i = 0; i < tt.Count; i++)
            {
                zonematrix = new FrayteZoneMatrix();
                zonematrix.FromZone = tt[i].FromZone.FromZoneDisplay;
                zonematrix.ApplyZone = new List<FrayteApplyZone>();
                FrayteApplyZone apply = new FrayteApplyZone();
                for (int j = 0; j < tt[i].ApplyZone.Count; j++)
                {
                    if (j == 0)
                    {
                        apply.Zone1 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 1)
                    {
                        apply.Zone2 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 2)
                    {
                        apply.Zone3 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 3)
                    {
                        apply.Zone4 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 4)
                    {
                        apply.Zone5 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 5)
                    {
                        apply.Zone6 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 6)
                    {
                        apply.Zone7 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 7)
                    {
                        apply.Zone8 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 8)
                    {
                        apply.Zone9 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 9)
                    {
                        apply.Zone10 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    zonematrix.ApplyZone.Add(apply);
                }
                _matrix.Add(zonematrix);
            }

            #endregion

            #region Third Party Zone List

            int n = 1;
            List<FrayteApplyZone> fzone = new List<FrayteApplyZone>();
            FrayteApplyZone _zone = new FrayteApplyZone();

            foreach (FrayteZone ff in zone)
            {
                if (n == 1)
                {
                    _zone.Zone1 = ff.ZoneRateName;
                    _zone.TPZone1 = ff.ZoneDisplayName;
                }
                if (n == 2)
                {
                    _zone.Zone2 = ff.ZoneRateName;
                    _zone.TPZone2 = ff.ZoneDisplayName;
                }
                if (n == 3)
                {
                    _zone.Zone3 = ff.ZoneRateName;
                    _zone.TPZone3 = ff.ZoneDisplayName;
                }
                if (n == 4)
                {
                    _zone.Zone4 = ff.ZoneRateName;
                    _zone.TPZone4 = ff.ZoneDisplayName;
                }
                if (n == 5)
                {
                    _zone.Zone5 = ff.ZoneRateName;
                    _zone.TPZone5 = ff.ZoneDisplayName;
                }
                if (n == 6)
                {
                    _zone.Zone6 = ff.ZoneRateName;
                    _zone.TPZone6 = ff.ZoneDisplayName;
                }
                if (n == 7)
                {
                    _zone.Zone7 = ff.ZoneRateName;
                    _zone.TPZone7 = ff.ZoneDisplayName;
                }
                if (n == 8)
                {
                    _zone.Zone8 = ff.ZoneRateName;
                    _zone.TPZone8 = ff.ZoneDisplayName;
                }
                if (n == 9)
                {
                    _zone.Zone9 = ff.ZoneRateName;
                    _zone.TPZone9 = ff.ZoneDisplayName;
                }
                if (n == 10)
                {
                    _zone.Zone10 = ff.ZoneRateName;
                    _zone.TPZone10 = ff.ZoneDisplayName;
                }
                if (n == 11)
                {
                    _zone.Zone11 = ff.ZoneRateName;
                    _zone.TPZone11 = ff.ZoneDisplayName;
                }
                if (n == 12)
                {
                    _zone.Zone12 = ff.ZoneRateName;
                    _zone.TPZone12 = ff.ZoneDisplayName;
                }
                if (n == 13)
                {
                    _zone.Zone13 = ff.ZoneRateName;
                    _zone.TPZone13 = ff.ZoneDisplayName;
                }
                n++;
            }
            fzone.Add(_zone);

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            zonecountry.ThirdParty = new List<FrayteZoneMatrix>();
            zonecountry.ThirdParty = _matrix;
            zonecountry.AddOnRate = new List<FrayteAddOnRate>();
            zonecountry.AddOnRate = _add;
            zonecountry.Zone = new List<FrayteApplyZone>();
            zonecountry.Zone = fzone;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCards/"));
                ReportTemplate.UKRateCards.DHL3rdPartyExpress report = new ReportTemplate.UKRateCards.DHL3rdPartyExpress();
                report.Parameters["lblHeader"].Value = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "GBP";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblThirdParty"].Value = "DHL 3rd Party Zone Matrix " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - GBP" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }
            return result;
        }

        internal FrayteManifestName UKDHL3rdPartyEconomy(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var matrix = new ThirdPartyMarixRepository().GetThirdPartyMatrix(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);
            var zone = new ThirdPartyMarixRepository().GetZoneDetail(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteZoneMatrix> _matrix = new List<FrayteZoneMatrix>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.ReportLogisticDisplay == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.shipmentType.ReportLogisticDisplay == FrayteShipmentType.HeavyWeight).OrderBy(p => p.zone.ZoneId).ToList();

            FrayteBaseRate rate;

            #region Doc&NondocRate

            if (DocAndNondocitem != null && DocAndNondocitem.Count > 0)
            {
                var DocNonship = DocAndNondocitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = DocAndNondocitem.GroupBy(x => x.LogisticWeight.WeightTo).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = DocNondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region ThirdParty Matrix

            var tt = matrix
                    .GroupBy(p => new { p.FromZoneDisplay })
                    .Select(g => new
                    {
                        FromZone = g.Select(p => new { p.FromZoneDisplay }).First(),
                        ApplyZone = g.Select(p => new { p.ApplyZoneDisplay }).ToList()
                    }).ToList();

            FrayteZoneMatrix zonematrix;
            for (int i = 0; i < tt.Count; i++)
            {
                zonematrix = new FrayteZoneMatrix();
                zonematrix.FromZone = tt[i].FromZone.FromZoneDisplay;
                zonematrix.ApplyZone = new List<FrayteApplyZone>();
                FrayteApplyZone apply = new FrayteApplyZone();
                for (int j = 0; j < tt[i].ApplyZone.Count; j++)
                {
                    if (j == 0)
                    {
                        apply.Zone1 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 1)
                    {
                        apply.Zone2 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 2)
                    {
                        apply.Zone3 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 3)
                    {
                        apply.Zone4 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    if (j == 4)
                    {
                        apply.Zone5 = tt[i].ApplyZone[j].ApplyZoneDisplay;
                    }
                    zonematrix.ApplyZone.Add(apply);
                }
                _matrix.Add(zonematrix);
            }

            #endregion

            #region Third Party Zone List

            int n = 1;
            List<FrayteApplyZone> fzone = new List<FrayteApplyZone>();
            FrayteApplyZone _zone = new FrayteApplyZone();

            foreach (FrayteZone ff in zone)
            {
                if (n == 1)
                {
                    _zone.Zone1 = ff.ZoneRateName;
                    _zone.TPZone1 = ff.ZoneDisplayName;
                }
                if (n == 2)
                {
                    _zone.Zone2 = ff.ZoneRateName;
                    _zone.TPZone2 = ff.ZoneDisplayName;
                }
                if (n == 3)
                {
                    _zone.Zone3 = ff.ZoneRateName;
                    _zone.TPZone3 = ff.ZoneDisplayName;
                }
                if (n == 4)
                {
                    _zone.Zone4 = ff.ZoneRateName;
                    _zone.TPZone4 = ff.ZoneDisplayName;
                }
                if (n == 5)
                {
                    _zone.Zone5 = ff.ZoneRateName;
                    _zone.TPZone5 = ff.ZoneDisplayName;
                }
                if (n == 6)
                {
                    _zone.Zone6 = ff.ZoneRateName;
                    _zone.TPZone6 = ff.ZoneDisplayName;
                }
                if (n == 7)
                {
                    _zone.Zone7 = ff.ZoneRateName;
                    _zone.TPZone7 = ff.ZoneDisplayName;
                }
                if (n == 8)
                {
                    _zone.Zone8 = ff.ZoneRateName;
                    _zone.TPZone8 = ff.ZoneDisplayName;
                }
                if (n == 9)
                {
                    _zone.Zone9 = ff.ZoneRateName;
                    _zone.TPZone9 = ff.ZoneDisplayName;
                }
                if (n == 10)
                {
                    _zone.Zone10 = ff.ZoneRateName;
                    _zone.TPZone10 = ff.ZoneDisplayName;
                }
                if (n == 11)
                {
                    _zone.Zone11 = ff.ZoneRateName;
                    _zone.TPZone11 = ff.ZoneDisplayName;
                }
                if (n == 12)
                {
                    _zone.Zone12 = ff.ZoneRateName;
                    _zone.TPZone12 = ff.ZoneDisplayName;
                }
                if (n == 13)
                {
                    _zone.Zone13 = ff.ZoneRateName;
                    _zone.TPZone13 = ff.ZoneDisplayName;
                }
                n++;
            }
            fzone.Add(_zone);

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            zonecountry.ThirdParty = new List<FrayteZoneMatrix>();
            zonecountry.ThirdParty = _matrix;
            zonecountry.Zone = new List<FrayteApplyZone>();
            zonecountry.Zone = fzone;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCard/"));
                ReportTemplate.UKRateCards.DHL3rdPartyEconomy report = new ReportTemplate.UKRateCards.DHL3rdPartyEconomy();
                report.Parameters["lblHeader"].Value = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "GBP";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblThirdParty"].Value = "DHL 3rd Party Zone Matrix " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "DHL " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - GBP" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }
            return result;
        }

        internal FrayteManifestName UKTNTImportExpress(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region NondocRate

            if (Nondocitem != null && Nondocitem.Count > 0)
            {
                var Nonship = Nondocitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Nonship != null && Nonship.Count > 0)
                {
                    var Nondocweight = Nondocitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Nondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Nonship[0].ToString();
                        rate.Weight = Nondocweight[i].weights.WeightFrom.ToString() + " - " + Nondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Nondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Nondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteAddOnRate add;

            #region AddOnRate

            if (addon != null && addon.Count > 0)
            {
                var addonweight = addon.GroupBy(x => new { x.AddOnDescription })
                                       .Select(g => new
                                       {
                                           g.Key.AddOnDescription,
                                           weight = g.Select(p => new { p.WeightFrom, p.WeightTo, p.AddOnRate })
                                                     .GroupBy(y => new { y.WeightFrom, y.WeightTo })
                                                     .Select(h => new
                                                     {
                                                         weights = h.Key,
                                                         Rates = h.Select(p => new { p.AddOnRate }).ToList()
                                                     }).ToList()
                                       }).ToList();

                for (int i = 0; i < addonweight.Count; i++)
                {
                    for (int j = 0; j < addonweight[i].weight.Count; j++)
                    {
                        add = new FrayteAddOnRate();
                        add.ShipmentType = addonweight[i].AddOnDescription;
                        for (int k = 0; k < addonweight[i].weight[j].Rates.Count; k++)
                        {
                            if (k == 0)
                            {
                                add.Rate1 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 1)
                            {
                                add.Rate2 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 2)
                            {
                                add.Rate3 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 3)
                            {
                                add.Rate4 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 4)
                            {
                                add.Rate5 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 5)
                            {
                                add.Rate6 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 6)
                            {
                                add.Rate7 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 7)
                            {
                                add.Rate8 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 8)
                            {
                                add.Rate9 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                        }
                        _add.Add(add);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            zonecountry.AddOnRate = new List<FrayteAddOnRate>();
            zonecountry.AddOnRate = _add;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCard/"));
                ReportTemplate.UKRateCards.TNTImportExpress report = new ReportTemplate.UKRateCards.TNTImportExpress();
                report.Parameters["lblHeader"].Value = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "GBP";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - GBP" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName UKTNTImportEconomy(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();

            FrayteBaseRate rate;

            #region NondocRate

            if (Nondocitem != null && Nondocitem.Count > 0)
            {
                var Nonship = Nondocitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Nonship != null && Nonship.Count > 0)
                {
                    var Nondocweight = Nondocitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weightTo = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Nondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Nonship[0].ToString();
                        if (Nondocweight[i].weightTo.WeightTo == 5.99m)
                            rate.Weight = 0 + " - " + Nondocweight[i].weightTo.WeightTo.ToString();
                        else
                            rate.Weight = Nondocweight[i].weightTo.WeightFrom.ToString() + " - " + Nondocweight[i].weightTo.WeightTo.ToString();

                        for (int j = 0; j < Nondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Nondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteAddOnRate add;

            #region AddOnRate

            if (addon != null && addon.Count > 0)
            {
                var addonweight = addon.GroupBy(x => new { x.AddOnDescription })
                                       .Select(g => new
                                       {
                                           g.Key.AddOnDescription,
                                           weight = g.Select(p => new { p.WeightFrom, p.WeightTo, p.AddOnRate })
                                                     .GroupBy(y => new { y.WeightFrom, y.WeightTo })
                                                     .Select(h => new
                                                     {
                                                         weights = h.Key,
                                                         Rates = h.Select(p => new { p.AddOnRate }).ToList()
                                                     }).ToList()
                                       }).ToList();

                for (int i = 0; i < addonweight.Count; i++)
                {
                    for (int j = 0; j < addonweight[i].weight.Count; j++)
                    {
                        add = new FrayteAddOnRate();
                        add.ShipmentType = addonweight[i].AddOnDescription;
                        for (int k = 0; k < addonweight[i].weight[j].Rates.Count; k++)
                        {
                            if (k == 0)
                            {
                                add.Rate1 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 1)
                            {
                                add.Rate2 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 2)
                            {
                                add.Rate3 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 3)
                            {
                                add.Rate4 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 4)
                            {
                                add.Rate5 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 5)
                            {
                                add.Rate6 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 6)
                            {
                                add.Rate7 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 7)
                            {
                                add.Rate8 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 8)
                            {
                                add.Rate9 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                        }
                        _add.Add(add);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            zonecountry.AddOnRate = new List<FrayteAddOnRate>();
            zonecountry.AddOnRate = _add;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCard/"));
                ReportTemplate.UKRateCards.TNTImportEconomy report = new ReportTemplate.UKRateCards.TNTImportEconomy();
                report.Parameters["lblHeader"].Value = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "GBP";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - GBP" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName UKTNTExportExpress(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Doc).OrderBy(p => p.zone.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < docweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = docweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = docweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            #region NondocRate

            if (Nondocitem != null && Nondocitem.Count > 0)
            {
                var Nonship = Nondocitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Nonship != null && Nonship.Count > 0)
                {
                    var Nondocweight = Nondocitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Nondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Nonship[0].ToString();
                        rate.Weight = Nondocweight[i].weights.WeightFrom.ToString() + " - " + Nondocweight[i].weights.WeightTo.ToString();
                        for (int j = 0; j < Nondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Nondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteAddOnRate add;

            #region AddOnRate

            if (addon != null && addon.Count > 0)
            {
                var addonweight = addon.GroupBy(x => new { x.AddOnDescription })
                                       .Select(g => new
                                       {
                                           g.Key.AddOnDescription,
                                           weight = g.Select(p => new { p.WeightFrom, p.WeightTo, p.AddOnRate })
                                                     .GroupBy(y => new { y.WeightFrom, y.WeightTo })
                                                     .Select(h => new
                                                     {
                                                         weights = h.Key,
                                                         Rates = h.Select(p => new { p.AddOnRate }).ToList()
                                                     }).ToList()
                                       }).ToList();

                for (int i = 0; i < addonweight.Count; i++)
                {
                    for (int j = 0; j < addonweight[i].weight.Count; j++)
                    {
                        add = new FrayteAddOnRate();
                        add.ShipmentType = addonweight[i].AddOnDescription;
                        for (int k = 0; k < addonweight[i].weight[j].Rates.Count; k++)
                        {
                            if (k == 0)
                            {
                                add.Rate1 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 1)
                            {
                                add.Rate2 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 2)
                            {
                                add.Rate3 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 3)
                            {
                                add.Rate4 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 4)
                            {
                                add.Rate5 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 5)
                            {
                                add.Rate6 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 6)
                            {
                                add.Rate7 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 7)
                            {
                                add.Rate8 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 8)
                            {
                                add.Rate9 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                        }
                        _add.Add(add);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            zonecountry.AddOnRate = new List<FrayteAddOnRate>();
            zonecountry.AddOnRate = _add;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCard/"));
                ReportTemplate.UKRateCards.TNTExportExpress report = new ReportTemplate.UKRateCards.TNTExportExpress();
                report.Parameters["lblHeader"].Value = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "GBP";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - GBP" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName UKTNTExportEconomy(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.RateType, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Nondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.Nondoc).OrderBy(p => p.zone.ZoneId).ToList();

            FrayteBaseRate rate;

            #region NondocRate

            if (Nondocitem != null && Nondocitem.Count > 0)
            {
                var Nonship = Nondocitem.Select(p => p.shipmentType.ReportLogisticDisplay).Distinct().ToList();
                if (Nonship != null && Nonship.Count > 0)
                {
                    var Nondocweight = Nondocitem.GroupBy(x => new { x.LogisticWeight.WeightFrom, x.LogisticWeight.WeightTo }).Select(g => new { WeightTo = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Nondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Nonship[0].ToString();
                        if (Nondocweight[i].WeightTo.WeightTo == 5.99m)
                            rate.Weight = 0 + " - " + Nondocweight[i].WeightTo.WeightTo.ToString();
                        else
                            rate.Weight = Nondocweight[i].WeightTo.WeightFrom.ToString() + " - " + Nondocweight[i].WeightTo.WeightTo.ToString();

                        for (int j = 0; j < Nondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 3)
                            {
                                rate.Rate4 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 4)
                            {
                                rate.Rate5 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 5)
                            {
                                rate.Rate6 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 6)
                            {
                                rate.Rate7 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 7)
                            {
                                rate.Rate8 = Nondocweight[i].Rates[j].Rate;
                            }
                            else if (j == 8)
                            {
                                rate.Rate9 = Nondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteAddOnRate add;

            #region AddOnRate

            if (addon != null && addon.Count > 0)
            {
                var addonweight = addon.GroupBy(x => new { x.AddOnDescription })
                                       .Select(g => new
                                       {
                                           g.Key.AddOnDescription,
                                           weight = g.Select(p => new { p.WeightFrom, p.WeightTo, p.AddOnRate })
                                                     .GroupBy(y => new { y.WeightFrom, y.WeightTo })
                                                     .Select(h => new
                                                     {
                                                         weights = h.Key,
                                                         Rates = h.Select(p => new { p.AddOnRate }).ToList()
                                                     }).ToList()
                                       }).ToList();


                for (int i = 0; i < addonweight.Count; i++)
                {
                    for (int j = 0; j < addonweight[i].weight.Count; j++)
                    {
                        add = new FrayteAddOnRate();
                        add.ShipmentType = addonweight[i].AddOnDescription;
                        for (int k = 0; k < addonweight[i].weight[j].Rates.Count; k++)
                        {
                            if (k == 0)
                            {
                                add.Rate1 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 1)
                            {
                                add.Rate2 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 2)
                            {
                                add.Rate3 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 3)
                            {
                                add.Rate4 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 4)
                            {
                                add.Rate5 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 5)
                            {
                                add.Rate6 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 6)
                            {
                                add.Rate7 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 7)
                            {
                                add.Rate8 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                            if (k == 8)
                            {
                                add.Rate9 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                        }
                        _add.Add(add);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = country;
            zonecountry.AddOnRate = new List<FrayteAddOnRate>();
            zonecountry.AddOnRate = _add;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCard/"));
                ReportTemplate.UKRateCards.TNTExportEconomy report = new ReportTemplate.UKRateCards.TNTExportEconomy();
                report.Parameters["lblHeader"].Value = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "GBP";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.Parameters["lblZoneCountry"].Value = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.DataSource = _counrty;
                string FileName = "TNT " + logisticServices.LogisticType + " " + logisticServices.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") - GBP" + ".xls";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }

            return result;
        }

        internal FrayteManifestName UKDHLDomestic(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportUKBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var DocAndNondocitem = filter.FindAll(x => x.shipmentType.LogisticDescription == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.zone.ZoneId).ToList();

            FrayteBaseRate rate;

            #region Doc&NondocRate

            if (DocAndNondocitem != null && DocAndNondocitem.Count > 0)
            {
                var DocNonship = DocAndNondocitem.Select(p => p.shipmentType.LogisticDescriptionDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = DocAndNondocitem.GroupBy(x => x.LogisticWeight.WeightTo).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.ToString();
                        for (int j = 0; j < DocNondocweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = DocNondocweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _rate.Add(rate);
                    }
                }
            }

            #endregion

            FrayteAddOnRate add;

            #region AddOnRate

            if (addon != null && addon.Count > 0)
            {
                var addonweight = addon.GroupBy(x => new { x.AddOnDescription })
                                       .Select(g => new
                                       {
                                           g.Key.AddOnDescription,
                                           weight = g.Select(p => new { p.WeightFrom, p.WeightTo, p.AddOnRate })
                                                     .GroupBy(y => new { y.WeightFrom, y.WeightTo })
                                                     .Select(h => new
                                                     {
                                                         weights = h.Key,
                                                         Rates = h.Select(p => new { p.AddOnRate }).ToList()
                                                     }).ToList()
                                       }).ToList();

                for (int i = 0; i < addonweight.Count; i++)
                {
                    for (int j = 0; j < addonweight[i].weight.Count; j++)
                    {
                        add = new FrayteAddOnRate();
                        add.ShipmentType = addonweight[i].AddOnDescription;
                        add.Weight = addonweight[i].weight[j].weights.WeightFrom.ToString() + " - " + addonweight[i].weight[j].weights.WeightTo.ToString();
                        for (int k = 0; k < addonweight[i].weight[j].Rates.Count; k++)
                        {
                            if (k == 0)
                            {
                                add.Rate1 = addonweight[i].weight[j].Rates[k].AddOnRate;
                            }
                        }
                        _add.Add(add);
                    }
                }
            }

            #endregion

            FrayteBaseRateWithCountry zonecountry = new FrayteBaseRateWithCountry();
            zonecountry.BaseRate = new List<FrayteBaseRate>();
            zonecountry.BaseRate = _rate;
            zonecountry.ZoneCountry = new List<FrayteBaseRateZoneCountry>();
            zonecountry.ZoneCountry = null;
            zonecountry.AddOnRate = new List<FrayteAddOnRate>();
            zonecountry.AddOnRate = _add;
            _counrty.Add(zonecountry);

            if (_counrty != null && _counrty.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCard/"));
                ReportTemplate.UKRateCards.UKDomesticDHL report = new ReportTemplate.UKRateCards.UKDomesticDHL();
                report.Parameters["lblHeader"].Value = "DHL Express UK Domestic " + logisticServices.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "GBP";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.DataSource = _counrty;
                string FileName = "DHL UK Domestic Express Rate Card UK (" + DateTime.Today.Year + ") - GBP" + ".xlsx";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }
            return result;
        }

        internal FrayteManifestName UKUKMail(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportUKBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();

            var Ship1 = filter.FindAll(p => p.shipmentType.LogisticDescription == "NWD").OrderBy(x => x.zone.ZoneId).ToList();
            var Ship2 = filter.FindAll(p => p.shipmentType.LogisticDescription == "NWDN").OrderBy(x => x.zone.ZoneId).ToList();
            var Ship3 = filter.FindAll(p => p.shipmentType.LogisticDescription == "NWD1030").OrderBy(x => x.zone.ZoneId).ToList();
            var Ship4 = filter.FindAll(p => p.shipmentType.LogisticDescription == "NWD0900").OrderBy(x => x.zone.ZoneId).ToList();
            var Ship5 = filter.FindAll(p => p.shipmentType.LogisticDescription == "SAT").OrderBy(x => x.zone.ZoneId).ToList();
            var Ship6 = filter.FindAll(p => p.shipmentType.LogisticDescription == "SAT1030").OrderBy(x => x.zone.ZoneId).ToList();
            var Ship7 = filter.FindAll(p => p.shipmentType.LogisticDescription == "SAT0900").OrderBy(x => x.zone.ZoneId).ToList();
            var Ship8 = filter.FindAll(p => p.shipmentType.LogisticDescription == "SSPP").OrderBy(x => x.zone.ZoneId).ToList();

            FrayteBaseRate rate;

            #region Ship1

            if (Ship1 != null && Ship1.Count > 0)
            {
                var single = Ship1.FindAll(p => p.LogisticWeight.PackageType == "Single").ToList();
                if (single != null && single.Count > 0)
                {
                    #region Parcel

                    var parcel = single.FindAll(p => p.LogisticWeight.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].LogisticWeight.ParcelType;
                            rate.PackageType = parcel[0].LogisticWeight.PackageType;
                            rate.ShipmentType = parcel[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion

                    #region BagItService

                    var bagit = single.FindAll(p => p.LogisticWeight.ParcelType == "BagItService").ToList();
                    if (bagit != null && bagit.Count > 0)
                    {
                        var weight = bagit.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = bagit[0].LogisticWeight.ParcelType;
                            rate.PackageType = bagit[0].LogisticWeight.PackageType;
                            rate.ShipmentType = bagit[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion
                }

                var multiple = Ship1.FindAll(p => p.LogisticWeight.PackageType == "Multiple").ToList();
                if (multiple != null && multiple.Count > 0)
                {
                    #region Parcel

                    var parcel = multiple.FindAll(p => p.LogisticWeight.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].LogisticWeight.ParcelType;
                            rate.PackageType = parcel[0].LogisticWeight.PackageType;
                            rate.ShipmentType = parcel[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion

                    #region BagItService

                    //var bagit = multiple.FindAll(p => p.ParcelType == "BagItService").ToList();
                    //if (bagit != null && bagit.Count > 0)
                    //{
                    //    var weight = bagit.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    //    for (int i = 0; i < weight.Count; i++)
                    //    {
                    //        rate = new FrayteBaseRate();
                    //        rate.ParcelType = bagit[0].ParcelType;
                    //        rate.PackageType = bagit[0].PackageType;
                    //        rate.ShipmentType = bagit[0].ShipmentDisplayType;
                    //        rate.Weight = weight[i].weights;
                    //        for (int j = 0; j < weight[i].Rates.Count; j++)
                    //        {
                    //            if (j == 0)
                    //            {
                    //                rate.Rate1 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 1)
                    //            {
                    //                rate.Rate2 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 2)
                    //            {
                    //                rate.Rate3 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 3)
                    //            {
                    //                rate.Rate4 = weight[i].Rates[j].Rate;
                    //            }
                    //        }
                    //        _rate.Add(rate);
                    //    }
                    //}

                    #endregion
                }
            }

            #endregion

            #region Ship2

            if (Ship2 != null && Ship2.Count > 0)
            {
                var single = Ship2.FindAll(p => p.LogisticWeight.PackageType == "Single").ToList();
                if (single != null && single.Count > 0)
                {
                    #region Parcel

                    var parcel = single.FindAll(p => p.LogisticWeight.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].LogisticWeight.ParcelType;
                            rate.PackageType = parcel[0].LogisticWeight.PackageType;
                            rate.ShipmentType = parcel[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion

                    #region BagItService

                    var bagit = single.FindAll(p => p.LogisticWeight.ParcelType == "BagItService").ToList();
                    if (bagit != null && bagit.Count > 0)
                    {
                        var weight = bagit.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = bagit[0].LogisticWeight.ParcelType;
                            rate.PackageType = bagit[0].LogisticWeight.PackageType;
                            rate.ShipmentType = bagit[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion
                }

                var multiple = Ship2.FindAll(p => p.LogisticWeight.PackageType == "Multiple").ToList();
                if (multiple != null && multiple.Count > 0)
                {
                    #region Parcel

                    var parcel = multiple.FindAll(p => p.LogisticWeight.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].LogisticWeight.ParcelType;
                            rate.PackageType = parcel[0].LogisticWeight.PackageType;
                            rate.ShipmentType = parcel[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion

                    #region BagItService

                    //var bagit = multiple.FindAll(p => p.ParcelType == "BagItService").ToList();
                    //if (bagit != null && bagit.Count > 0)
                    //{
                    //    var weight = bagit.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    //    for (int i = 0; i < weight.Count; i++)
                    //    {
                    //        rate = new FrayteBaseRate();
                    //        rate.ParcelType = bagit[0].ParcelType;
                    //        rate.PackageType = bagit[0].PackageType;
                    //        rate.ShipmentType = bagit[0].ShipmentDisplayType;
                    //        rate.Weight = weight[i].weights;
                    //        for (int j = 0; j < weight[i].Rates.Count; j++)
                    //        {
                    //            if (j == 0)
                    //            {
                    //                rate.Rate1 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 1)
                    //            {
                    //                rate.Rate2 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 2)
                    //            {
                    //                rate.Rate3 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 3)
                    //            {
                    //                rate.Rate4 = weight[i].Rates[j].Rate;
                    //            }
                    //        }
                    //        _rate.Add(rate);
                    //    }
                    //}

                    #endregion
                }
            }

            #endregion

            #region Ship3

            if (Ship3 != null && Ship3.Count > 0)
            {
                var single = Ship3.FindAll(p => p.LogisticWeight.PackageType == "Single").ToList();
                if (single != null && single.Count > 0)
                {
                    #region Parcel

                    var parcel = single.FindAll(p => p.LogisticWeight.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].LogisticWeight.ParcelType;
                            rate.PackageType = parcel[0].LogisticWeight.PackageType;
                            rate.ShipmentType = parcel[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion

                    #region BagItService

                    var bagit = single.FindAll(p => p.LogisticWeight.ParcelType == "BagItService").ToList();
                    if (bagit != null && bagit.Count > 0)
                    {
                        var weight = bagit.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = bagit[0].LogisticWeight.ParcelType;
                            rate.PackageType = bagit[0].LogisticWeight.PackageType;
                            rate.ShipmentType = bagit[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion
                }

                var multiple = Ship3.FindAll(p => p.LogisticWeight.PackageType == "Multiple").ToList();
                if (multiple != null && multiple.Count > 0)
                {
                    #region Parcel

                    var parcel = multiple.FindAll(p => p.LogisticWeight.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].LogisticWeight.ParcelType;
                            rate.PackageType = parcel[0].LogisticWeight.PackageType;
                            rate.ShipmentType = parcel[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion

                    #region BagItService

                    //var bagit = multiple.FindAll(p => p.ParcelType == "BagItService").ToList();
                    //if (bagit != null && bagit.Count > 0)
                    //{
                    //    var weight = bagit.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    //    for (int i = 0; i < weight.Count; i++)
                    //    {
                    //        rate = new FrayteBaseRate();
                    //        rate.ParcelType = bagit[0].ParcelType;
                    //        rate.PackageType = bagit[0].PackageType;
                    //        rate.ShipmentType = bagit[0].ShipmentDisplayType;
                    //        rate.Weight = weight[i].weights;
                    //        for (int j = 0; j < weight[i].Rates.Count; j++)
                    //        {
                    //            if (j == 0)
                    //            {
                    //                rate.Rate1 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 1)
                    //            {
                    //                rate.Rate2 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 2)
                    //            {
                    //                rate.Rate3 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 3)
                    //            {
                    //                rate.Rate4 = weight[i].Rates[j].Rate;
                    //            }
                    //        }
                    //        _rate.Add(rate);
                    //    }
                    //}

                    #endregion
                }
            }

            #endregion

            #region Ship4

            if (Ship4 != null && Ship4.Count > 0)
            {
                var single = Ship4.FindAll(p => p.LogisticWeight.PackageType == "Single").ToList();
                if (single != null && single.Count > 0)
                {
                    #region Parcel

                    var parcel = single.FindAll(p => p.LogisticWeight.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].LogisticWeight.ParcelType;
                            rate.PackageType = parcel[0].LogisticWeight.PackageType;
                            rate.ShipmentType = parcel[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion

                    #region BagItService

                    var bagit = single.FindAll(p => p.LogisticWeight.ParcelType == "BagItService").ToList();
                    if (bagit != null && bagit.Count > 0)
                    {
                        var weight = bagit.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = bagit[0].LogisticWeight.ParcelType;
                            rate.PackageType = bagit[0].LogisticWeight.PackageType;
                            rate.ShipmentType = bagit[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion
                }

                var multiple = Ship4.FindAll(p => p.LogisticWeight.PackageType == "Multiple").ToList();
                if (multiple != null && multiple.Count > 0)
                {
                    #region Parcel

                    var parcel = multiple.FindAll(p => p.LogisticWeight.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].LogisticWeight.ParcelType;
                            rate.PackageType = parcel[0].LogisticWeight.PackageType;
                            rate.ShipmentType = parcel[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion

                    #region BagItService

                    //var bagit = multiple.FindAll(p => p.ParcelType == "BagItService").ToList();
                    //if (bagit != null && bagit.Count > 0)
                    //{
                    //    var weight = bagit.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    //    for (int i = 0; i < weight.Count; i++)
                    //    {
                    //        rate = new FrayteBaseRate();
                    //        rate.ParcelType = bagit[0].ParcelType;
                    //        rate.PackageType = bagit[0].PackageType;
                    //        rate.ShipmentType = bagit[0].ShipmentDisplayType;
                    //        rate.Weight = weight[i].weights;
                    //        for (int j = 0; j < weight[i].Rates.Count; j++)
                    //        {
                    //            if (j == 0)
                    //            {
                    //                rate.Rate1 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 1)
                    //            {
                    //                rate.Rate2 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 2)
                    //            {
                    //                rate.Rate3 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 3)
                    //            {
                    //                rate.Rate4 = weight[i].Rates[j].Rate;
                    //            }
                    //        }
                    //        _rate.Add(rate);
                    //    }
                    //}

                    #endregion
                }
            }

            #endregion

            #region Ship5

            if (Ship5 != null && Ship5.Count > 0)
            {
                var single = Ship5.FindAll(p => p.LogisticWeight.PackageType == "Single").ToList();
                if (single != null && single.Count > 0)
                {
                    #region Parcel

                    var parcel = single.FindAll(p => p.LogisticWeight.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].LogisticWeight.ParcelType;
                            rate.PackageType = parcel[0].LogisticWeight.PackageType;
                            rate.ShipmentType = parcel[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion

                    #region BagItService

                    var bagit = single.FindAll(p => p.LogisticWeight.ParcelType == "BagItService").ToList();
                    if (bagit != null && bagit.Count > 0)
                    {
                        var weight = bagit.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = bagit[0].LogisticWeight.ParcelType;
                            rate.PackageType = bagit[0].LogisticWeight.PackageType;
                            rate.ShipmentType = bagit[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion
                }

                var multiple = Ship5.FindAll(p => p.LogisticWeight.PackageType == "Multiple").ToList();
                if (multiple != null && multiple.Count > 0)
                {
                    #region Parcel

                    var parcel = multiple.FindAll(p => p.LogisticWeight.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].LogisticWeight.ParcelType;
                            rate.PackageType = parcel[0].LogisticWeight.PackageType;
                            rate.ShipmentType = parcel[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion

                    #region BagItService

                    //var bagit = multiple.FindAll(p => p.ParcelType == "BagItService").ToList();
                    //if (bagit != null && bagit.Count > 0)
                    //{
                    //    var weight = bagit.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    //    for (int i = 0; i < weight.Count; i++)
                    //    {
                    //        rate = new FrayteBaseRate();
                    //        rate.ParcelType = bagit[0].ParcelType;
                    //        rate.PackageType = bagit[0].PackageType;
                    //        rate.ShipmentType = bagit[0].ShipmentDisplayType;
                    //        rate.Weight = weight[i].weights;
                    //        for (int j = 0; j < weight[i].Rates.Count; j++)
                    //        {
                    //            if (j == 0)
                    //            {
                    //                rate.Rate1 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 1)
                    //            {
                    //                rate.Rate2 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 2)
                    //            {
                    //                rate.Rate3 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 3)
                    //            {
                    //                rate.Rate4 = weight[i].Rates[j].Rate;
                    //            }
                    //        }
                    //        _rate.Add(rate);
                    //    }
                    //}

                    #endregion
                }
            }

            #endregion

            #region Ship6

            if (Ship6 != null && Ship6.Count > 0)
            {
                var single = Ship6.FindAll(p => p.LogisticWeight.PackageType == "Single").ToList();
                if (single != null && single.Count > 0)
                {
                    #region Parcel

                    var parcel = single.FindAll(p => p.LogisticWeight.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].LogisticWeight.ParcelType;
                            rate.PackageType = parcel[0].LogisticWeight.PackageType;
                            rate.ShipmentType = parcel[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion

                    #region BagItService

                    var bagit = single.FindAll(p => p.LogisticWeight.ParcelType == "BagItService").ToList();
                    if (bagit != null && bagit.Count > 0)
                    {
                        var weight = bagit.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = bagit[0].LogisticWeight.ParcelType;
                            rate.PackageType = bagit[0].LogisticWeight.PackageType;
                            rate.ShipmentType = bagit[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion
                }

                var multiple = Ship6.FindAll(p => p.LogisticWeight.PackageType == "Multiple").ToList();
                if (multiple != null && multiple.Count > 0)
                {
                    #region Parcel

                    var parcel = multiple.FindAll(p => p.LogisticWeight.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].LogisticWeight.ParcelType;
                            rate.PackageType = parcel[0].LogisticWeight.PackageType;
                            rate.ShipmentType = parcel[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion

                    #region BagItService

                    //var bagit = multiple.FindAll(p => p.ParcelType == "BagItService").ToList();
                    //if (bagit != null && bagit.Count > 0)
                    //{
                    //    var weight = bagit.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    //    for (int i = 0; i < weight.Count; i++)
                    //    {
                    //        rate = new FrayteBaseRate();
                    //        rate.ParcelType = bagit[0].ParcelType;
                    //        rate.PackageType = bagit[0].PackageType;
                    //        rate.ShipmentType = bagit[0].ShipmentDisplayType;
                    //        rate.Weight = weight[i].weights;
                    //        for (int j = 0; j < weight[i].Rates.Count; j++)
                    //        {
                    //            if (j == 0)
                    //            {
                    //                rate.Rate1 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 1)
                    //            {
                    //                rate.Rate2 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 2)
                    //            {
                    //                rate.Rate3 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 3)
                    //            {
                    //                rate.Rate4 = weight[i].Rates[j].Rate;
                    //            }
                    //        }
                    //        _rate.Add(rate);
                    //    }
                    //}

                    #endregion
                }
            }

            #endregion

            #region Ship7

            if (Ship7 != null && Ship7.Count > 0)
            {
                var single = Ship7.FindAll(p => p.LogisticWeight.PackageType == "Single").ToList();
                if (single != null && single.Count > 0)
                {
                    #region Parcel

                    var parcel = single.FindAll(p => p.LogisticWeight.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].LogisticWeight.ParcelType;
                            rate.PackageType = parcel[0].LogisticWeight.PackageType;
                            rate.ShipmentType = parcel[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion

                    #region BagItService

                    var bagit = single.FindAll(p => p.LogisticWeight.ParcelType == "BagItService").ToList();
                    if (bagit != null && bagit.Count > 0)
                    {
                        var weight = bagit.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = bagit[0].LogisticWeight.ParcelType;
                            rate.PackageType = bagit[0].LogisticWeight.PackageType;
                            rate.ShipmentType = bagit[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion
                }

                var multiple = Ship7.FindAll(p => p.LogisticWeight.PackageType == "Multiple").ToList();
                if (multiple != null && multiple.Count > 0)
                {
                    #region Parcel

                    var parcel = multiple.FindAll(p => p.LogisticWeight.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].LogisticWeight.ParcelType;
                            rate.PackageType = parcel[0].LogisticWeight.PackageType;
                            rate.ShipmentType = parcel[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion

                    #region BagItService

                    //var bagit = multiple.FindAll(p => p.ParcelType == "BagItService").ToList();
                    //if (bagit != null && bagit.Count > 0)
                    //{
                    //    var weight = bagit.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    //    for (int i = 0; i < weight.Count; i++)
                    //    {
                    //        rate = new FrayteBaseRate();
                    //        rate.ParcelType = bagit[0].ParcelType;
                    //        rate.PackageType = bagit[0].PackageType;
                    //        rate.ShipmentType = bagit[0].ShipmentDisplayType;
                    //        rate.Weight = weight[i].weights;
                    //        for (int j = 0; j < weight[i].Rates.Count; j++)
                    //        {
                    //            if (j == 0)
                    //            {
                    //                rate.Rate1 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 1)
                    //            {
                    //                rate.Rate2 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 2)
                    //            {
                    //                rate.Rate3 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 3)
                    //            {
                    //                rate.Rate4 = weight[i].Rates[j].Rate;
                    //            }
                    //        }
                    //        _rate.Add(rate);
                    //    }
                    //}

                    #endregion
                }
            }

            #endregion

            #region Ship8

            if (Ship8 != null && Ship8.Count > 0)
            {
                var single = Ship8.FindAll(p => p.LogisticWeight.PackageType == "Single").ToList();
                if (single != null && single.Count > 0)
                {
                    #region Parcel

                    var parcel = single.FindAll(p => p.LogisticWeight.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].LogisticWeight.ParcelType;
                            rate.PackageType = parcel[0].LogisticWeight.PackageType;
                            rate.ShipmentType = parcel[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion

                    #region BagItService

                    var bagit = single.FindAll(p => p.LogisticWeight.ParcelType == "BagItService").ToList();
                    if (bagit != null && bagit.Count > 0)
                    {
                        var weight = bagit.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = bagit[0].LogisticWeight.ParcelType;
                            rate.PackageType = bagit[0].LogisticWeight.PackageType;
                            rate.ShipmentType = bagit[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion
                }

                var multiple = Ship8.FindAll(p => p.LogisticWeight.PackageType == "Multiple").ToList();
                if (multiple != null && multiple.Count > 0)
                {
                    #region Parcel

                    var parcel = multiple.FindAll(p => p.LogisticWeight.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.shipmentType.LogisticDescriptionDisplay, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key.WeightTo, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].LogisticWeight.ParcelType;
                            rate.PackageType = parcel[0].LogisticWeight.PackageType;
                            rate.ShipmentType = parcel[0].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            _rate.Add(rate);
                        }
                    }

                    #endregion

                    #region BagItService

                    //var bagit = multiple.FindAll(p => p.ParcelType == "BagItService").ToList();
                    //if (bagit != null && bagit.Count > 0)
                    //{
                    //    var weight = bagit.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    //    for (int i = 0; i < weight.Count; i++)
                    //    {
                    //        rate = new FrayteBaseRate();
                    //        rate.ParcelType = bagit[0].ParcelType;
                    //        rate.PackageType = bagit[0].PackageType;
                    //        rate.ShipmentType = bagit[0].ShipmentDisplayType;
                    //        rate.Weight = weight[i].weights;
                    //        for (int j = 0; j < weight[i].Rates.Count; j++)
                    //        {
                    //            if (j == 0)
                    //            {
                    //                rate.Rate1 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 1)
                    //            {
                    //                rate.Rate2 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 2)
                    //            {
                    //                rate.Rate3 = weight[i].Rates[j].Rate;
                    //            }
                    //            else if (j == 3)
                    //            {
                    //                rate.Rate4 = weight[i].Rates[j].Rate;
                    //            }
                    //        }
                    //        _rate.Add(rate);
                    //    }
                    //}

                    #endregion
                }
            }

            #endregion

            if (_rate != null && _rate.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCard/"));
                ReportTemplate.UKRateCards.UKMail report = new ReportTemplate.UKRateCards.UKMail();
                report.Parameters["lblHeader"].Value = "UK Mail Domestic Shipments Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "GBP";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.DataSource = _rate;
                string FileName = "UK Mail Domestic Shipments Rate Card (" + DateTime.Today.Year + ") - GBP" + ".xlsx";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }
            return result;
        }

        internal FrayteManifestName UKYodel(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportUKBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            List<FrayteBaseRate> _b2b;
            List<FrayteBaseRate> _b2cNeighbour;
            List<FrayteBaseRate> _b2cHome;
            List<FrayteYodel> _yodel = new List<FrayteYodel>();

            var dih = filter.FindAll(x => x.LogisticWeight.ParcelType == "DIH").OrderBy(x => x.zone.ZoneId).ToList();
            var pod = filter.FindAll(x => x.LogisticWeight.ParcelType == "PODDISVC").OrderBy(x => x.zone.ZoneId).ToList();
            var svc = filter.FindAll(x => x.LogisticWeight.ParcelType == "SVCPOD").OrderBy(x => x.zone.ZoneId).ToList();
            var tr = filter.FindAll(x => x.LogisticWeight.ParcelType == "TRPOD").OrderBy(x => x.zone.ZoneId).ToList();

            #region DIH

            if (dih != null && dih.Count > 0)
            {
                FrayteYodel yodel = new FrayteYodel();

                var b2b = dih.FindAll(p => p.LogisticWeight.PackageType == "B2B").OrderBy(x => x.zone.ZoneId).ToList();
                if (b2b != null && b2b.Count > 0)
                {
                    var Ship1 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXP24").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship2 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXP48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship3 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXPNI").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship4 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "HOM72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship5 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "HOM72IN").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship6 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "PRI12MF").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship7 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "SATPRI12").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship8 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXPSAT").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship9 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXP72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship10 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXPISLScHi").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship11 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "DNOSPChIs").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship12 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "ROIR48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship13 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "ROIR72").OrderBy(x => x.zone.ZoneId).ToList();

                    FrayteBaseRate rate;
                    _b2b = new List<FrayteBaseRate>();

                    #region Ship1

                    if (Ship1 != null && Ship1.Count > 0)
                    {
                        var weight = Ship1.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship1[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship1[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship2

                    if (Ship2 != null && Ship2.Count > 0)
                    {
                        var weight = Ship2.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship2[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship2[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship3

                    if (Ship3 != null && Ship3.Count > 0)
                    {
                        var weight = Ship3.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship3[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship3[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship4

                    if (Ship4 != null && Ship4.Count > 0)
                    {
                        var weight = Ship4.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship4[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship4[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship5

                    if (Ship5 != null && Ship5.Count > 0)
                    {
                        var weight = Ship5.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship5[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship5[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship6

                    if (Ship6 != null && Ship6.Count > 0)
                    {
                        var weight = Ship6.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship6[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship6[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship7

                    if (Ship7 != null && Ship7.Count > 0)
                    {
                        var weight = Ship7.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship7[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship7[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship8

                    if (Ship8 != null && Ship8.Count > 0)
                    {
                        var weight = Ship8.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship8[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship8[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship9

                    if (Ship9 != null && Ship9.Count > 0)
                    {
                        var weight = Ship9.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship9[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship9[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship10

                    if (Ship10 != null && Ship10.Count > 0)
                    {
                        var weight = Ship10.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship10[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship10[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship11

                    if (Ship11 != null && Ship11.Count > 0)
                    {
                        var weight = Ship11.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship11[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship11[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship12

                    if (Ship12 != null && Ship12.Count > 0)
                    {
                        var weight = Ship12.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship12[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship12[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship13

                    if (Ship13 != null && Ship13.Count > 0)
                    {
                        var weight = Ship13.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship13[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship13[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    yodel.B2B = new List<FrayteBaseRate>();
                    yodel.B2B = _b2b;
                }

                var b2cNeighbour = dih.FindAll(p => p.LogisticWeight.PackageType == "B2CNeighborhood").OrderBy(x => x.zone.ZoneId).ToList();
                if (b2cNeighbour != null && b2cNeighbour.Count > 0)
                {
                    var Ship1 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXP24").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship2 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXP48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship3 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXPNI").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship4 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "HOM72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship5 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "HOM72IN").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship6 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "PRI12MF").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship7 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "SATPRI12").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship8 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXPSAT").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship9 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXP72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship10 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXPISLScHi").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship11 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "DNOSPChIs").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship12 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "ROIR48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship13 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "ROIR72").OrderBy(x => x.zone.ZoneId).ToList();

                    FrayteBaseRate rate;
                    _b2cNeighbour = new List<FrayteBaseRate>();

                    #region Ship1

                    if (Ship1 != null && Ship1.Count > 0)
                    {
                        var weight = Ship1.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship1[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship1[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship2

                    if (Ship2 != null && Ship2.Count > 0)
                    {
                        var weight = Ship2.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship2[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship2[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship3

                    if (Ship3 != null && Ship3.Count > 0)
                    {
                        var weight = Ship3.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship3[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship3[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship4

                    if (Ship4 != null && Ship4.Count > 0)
                    {
                        var weight = Ship4.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship4[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship4[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship5

                    if (Ship5 != null && Ship5.Count > 0)
                    {
                        var weight = Ship5.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship5[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship5[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship6

                    if (Ship6 != null && Ship6.Count > 0)
                    {
                        var weight = Ship6.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship6[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship6[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship7

                    if (Ship7 != null && Ship7.Count > 0)
                    {
                        var weight = Ship7.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship7[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship7[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship8

                    if (Ship8 != null && Ship8.Count > 0)
                    {
                        var weight = Ship8.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship8[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship8[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship9

                    if (Ship9 != null && Ship9.Count > 0)
                    {
                        var weight = Ship9.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship9[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship9[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship10

                    if (Ship10 != null && Ship10.Count > 0)
                    {
                        var weight = Ship10.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship10[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship10[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship11

                    if (Ship11 != null && Ship11.Count > 0)
                    {
                        var weight = Ship11.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship11[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship11[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship12

                    if (Ship12 != null && Ship12.Count > 0)
                    {
                        var weight = Ship12.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship12[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship12[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship13

                    if (Ship13 != null && Ship13.Count > 0)
                    {
                        var weight = Ship13.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship13[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship13[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    yodel.B2CNeighbourhood = new List<FrayteBaseRate>();
                    yodel.B2CNeighbourhood = _b2cNeighbour;
                }

                var b2cHome = dih.FindAll(p => p.LogisticWeight.PackageType == "B2CHome").OrderBy(x => x.zone.ZoneId).ToList();
                if (b2cHome != null && b2cHome.Count > 0)
                {
                    var Ship1 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXP24").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship2 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXP48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship3 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXPNI").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship4 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "HOM72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship5 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "HOM72IN").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship6 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "PRI12MF").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship7 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "SATPRI12").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship8 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXPSAT").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship9 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXP72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship10 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXPISLScHi").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship11 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "DNOSPChIs").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship12 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "ROIR48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship13 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "ROIR72").OrderBy(x => x.zone.ZoneId).ToList();

                    FrayteBaseRate rate;
                    _b2cHome = new List<FrayteBaseRate>();

                    #region Ship1

                    if (Ship1 != null && Ship1.Count > 0)
                    {
                        var weight = Ship1.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship1[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship1[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship2

                    if (Ship2 != null && Ship2.Count > 0)
                    {
                        var weight = Ship2.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship2[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship2[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship3

                    if (Ship3 != null && Ship3.Count > 0)
                    {
                        var weight = Ship3.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship3[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship3[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship4

                    if (Ship4 != null && Ship4.Count > 0)
                    {
                        var weight = Ship4.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship4[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship4[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship5

                    if (Ship5 != null && Ship5.Count > 0)
                    {
                        var weight = Ship5.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship5[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship5[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship6

                    if (Ship6 != null && Ship6.Count > 0)
                    {
                        var weight = Ship6.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship6[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship6[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship7

                    if (Ship7 != null && Ship7.Count > 0)
                    {
                        var weight = Ship7.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship7[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship7[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship8

                    if (Ship8 != null && Ship8.Count > 0)
                    {
                        var weight = Ship8.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship8[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship8[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship9

                    if (Ship9 != null && Ship9.Count > 0)
                    {
                        var weight = Ship9.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship9[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship9[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship10

                    if (Ship10 != null && Ship10.Count > 0)
                    {
                        var weight = Ship10.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship10[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship10[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship11

                    if (Ship11 != null && Ship11.Count > 0)
                    {
                        var weight = Ship11.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship11[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship11[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship12

                    if (Ship12 != null && Ship12.Count > 0)
                    {
                        var weight = Ship12.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship12[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship12[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship13

                    if (Ship13 != null && Ship13.Count > 0)
                    {
                        var weight = Ship13.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship13[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship13[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    yodel.B2CHome = new List<FrayteBaseRate>();
                    yodel.B2CHome = _b2cHome;
                }

                _yodel.Add(yodel);
            }

            #endregion

            #region PODDISVC

            if (pod != null && pod.Count > 0)
            {
                FrayteYodel yodel = new FrayteYodel();

                var b2b = pod.FindAll(p => p.LogisticWeight.PackageType == "B2B").OrderBy(x => x.zone.ZoneId).ToList();
                if (b2b != null && b2b.Count > 0)
                {
                    var Ship1 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXP24").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship2 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXP48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship3 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXPNI").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship4 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "HOM72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship5 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "HOM72IN").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship6 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "PRI12MF").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship7 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "SATPRI12").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship8 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXPSAT").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship9 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXP72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship10 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXPISLScHi").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship11 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "DNOSPChIs").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship12 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "ROIR48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship13 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "ROIR72").OrderBy(x => x.zone.ZoneId).ToList();

                    FrayteBaseRate rate;
                    _b2b = new List<FrayteBaseRate>();

                    #region Ship1

                    if (Ship1 != null && Ship1.Count > 0)
                    {
                        var weight = Ship1.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship1[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship1[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship2

                    if (Ship2 != null && Ship2.Count > 0)
                    {
                        var weight = Ship2.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship2[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship2[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship3

                    if (Ship3 != null && Ship3.Count > 0)
                    {
                        var weight = Ship3.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship3[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship3[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship4

                    if (Ship4 != null && Ship4.Count > 0)
                    {
                        var weight = Ship4.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship4[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship4[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship5

                    if (Ship5 != null && Ship5.Count > 0)
                    {
                        var weight = Ship5.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship5[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship5[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship6

                    if (Ship6 != null && Ship6.Count > 0)
                    {
                        var weight = Ship6.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship6[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship6[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship7

                    if (Ship7 != null && Ship7.Count > 0)
                    {
                        var weight = Ship7.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship7[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship7[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship8

                    if (Ship8 != null && Ship8.Count > 0)
                    {
                        var weight = Ship8.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship8[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship8[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship9

                    if (Ship9 != null && Ship9.Count > 0)
                    {
                        var weight = Ship9.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship9[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship9[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship10

                    if (Ship10 != null && Ship10.Count > 0)
                    {
                        var weight = Ship10.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship10[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship10[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship11

                    if (Ship11 != null && Ship11.Count > 0)
                    {
                        var weight = Ship11.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship11[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship11[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship12

                    if (Ship12 != null && Ship12.Count > 0)
                    {
                        var weight = Ship12.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship12[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship12[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship13

                    if (Ship13 != null && Ship13.Count > 0)
                    {
                        var weight = Ship13.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship13[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship13[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    yodel.B2B = new List<FrayteBaseRate>();
                    yodel.B2B = _b2b;
                }

                var b2cNeighbour = pod.FindAll(p => p.LogisticWeight.PackageType == "B2CNeighborhood").OrderBy(x => x.zone.ZoneId).ToList();
                if (b2cNeighbour != null && b2cNeighbour.Count > 0)
                {
                    var Ship1 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXP24").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship2 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXP48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship3 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXPNI").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship4 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "HOM72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship5 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "HOM72IN").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship6 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "PRI12MF").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship7 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "SATPRI12").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship8 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXPSAT").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship9 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXP72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship10 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXPISLScHi").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship11 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "DNOSPChIs").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship12 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "ROIR48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship13 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "ROIR72").OrderBy(x => x.zone.ZoneId).ToList();

                    FrayteBaseRate rate;
                    _b2cNeighbour = new List<FrayteBaseRate>();

                    #region Ship1

                    if (Ship1 != null && Ship1.Count > 0)
                    {
                        var weight = Ship1.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship1[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship1[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship2

                    if (Ship2 != null && Ship2.Count > 0)
                    {
                        var weight = Ship2.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship2[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship2[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship3

                    if (Ship3 != null && Ship3.Count > 0)
                    {
                        var weight = Ship3.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship3[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship3[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship4

                    if (Ship4 != null && Ship4.Count > 0)
                    {
                        var weight = Ship4.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship4[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship4[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship5

                    if (Ship5 != null && Ship5.Count > 0)
                    {
                        var weight = Ship5.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship5[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship5[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship6

                    if (Ship6 != null && Ship6.Count > 0)
                    {
                        var weight = Ship6.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship6[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship6[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship7

                    if (Ship7 != null && Ship7.Count > 0)
                    {
                        var weight = Ship7.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship7[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship7[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship8

                    if (Ship8 != null && Ship8.Count > 0)
                    {
                        var weight = Ship8.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship8[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship8[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship9

                    if (Ship9 != null && Ship9.Count > 0)
                    {
                        var weight = Ship9.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship9[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship9[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship10

                    if (Ship10 != null && Ship10.Count > 0)
                    {
                        var weight = Ship10.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship10[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship10[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship11

                    if (Ship11 != null && Ship11.Count > 0)
                    {
                        var weight = Ship11.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship11[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship11[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship12

                    if (Ship12 != null && Ship12.Count > 0)
                    {
                        var weight = Ship12.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship12[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship12[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship13

                    if (Ship13 != null && Ship13.Count > 0)
                    {
                        var weight = Ship13.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship13[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship13[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    yodel.B2CNeighbourhood = new List<FrayteBaseRate>();
                    yodel.B2CNeighbourhood = _b2cNeighbour;
                }

                var b2cHome = pod.FindAll(p => p.LogisticWeight.PackageType == "B2CHome").OrderBy(x => x.zone.ZoneId).ToList();
                if (b2cHome != null && b2cHome.Count > 0)
                {
                    var Ship1 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXP24").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship2 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXP48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship3 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXPNI").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship4 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "HOM72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship5 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "HOM72IN").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship6 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "PRI12MF").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship7 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "SATPRI12").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship8 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXPSAT").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship9 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXP72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship10 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXPISLScHi").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship11 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "DNOSPChIs").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship12 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "ROIR48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship13 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "ROIR72").OrderBy(x => x.zone.ZoneId).ToList();

                    FrayteBaseRate rate;
                    _b2cHome = new List<FrayteBaseRate>();

                    #region Ship1

                    if (Ship1 != null && Ship1.Count > 0)
                    {
                        var weight = Ship1.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship1[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship1[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship2

                    if (Ship2 != null && Ship2.Count > 0)
                    {
                        var weight = Ship2.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship2[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship2[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship3

                    if (Ship3 != null && Ship3.Count > 0)
                    {
                        var weight = Ship3.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship3[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship3[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship4

                    if (Ship4 != null && Ship4.Count > 0)
                    {
                        var weight = Ship4.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship4[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship4[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship5

                    if (Ship5 != null && Ship5.Count > 0)
                    {
                        var weight = Ship5.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship5[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship5[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship6

                    if (Ship6 != null && Ship6.Count > 0)
                    {
                        var weight = Ship6.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship6[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship6[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship7

                    if (Ship7 != null && Ship7.Count > 0)
                    {
                        var weight = Ship7.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship7[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship7[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship8

                    if (Ship8 != null && Ship8.Count > 0)
                    {
                        var weight = Ship8.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship8[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship8[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship9

                    if (Ship9 != null && Ship9.Count > 0)
                    {
                        var weight = Ship9.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship9[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship9[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship10

                    if (Ship10 != null && Ship10.Count > 0)
                    {
                        var weight = Ship10.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship10[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship10[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship11

                    if (Ship11 != null && Ship11.Count > 0)
                    {
                        var weight = Ship11.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship11[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship11[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship12

                    if (Ship12 != null && Ship12.Count > 0)
                    {
                        var weight = Ship12.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship12[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship12[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship13

                    if (Ship13 != null && Ship13.Count > 0)
                    {
                        var weight = Ship13.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship13[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship13[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    yodel.B2CHome = new List<FrayteBaseRate>();
                    yodel.B2CHome = _b2cHome;
                }

                _yodel.Add(yodel);
            }

            #endregion

            #region SVCPOD

            if (svc != null && svc.Count > 0)
            {
                FrayteYodel yodel = new FrayteYodel();

                var b2b = svc.FindAll(p => p.LogisticWeight.PackageType == "B2B").OrderBy(x => x.zone.ZoneId).ToList();
                if (b2b != null && b2b.Count > 0)
                {
                    var Ship1 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXP24").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship2 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXP48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship3 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXPNI").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship4 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "HOM72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship5 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "HOM72IN").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship6 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "PRI12MF").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship7 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "SATPRI12").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship8 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXPSAT").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship9 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXP72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship10 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXPISLScHi").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship11 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "DNOSPChIs").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship12 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "ROIR48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship13 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "ROIR72").OrderBy(x => x.zone.ZoneId).ToList();

                    FrayteBaseRate rate;
                    _b2b = new List<FrayteBaseRate>();

                    #region Ship1

                    if (Ship1 != null && Ship1.Count > 0)
                    {
                        var weight = Ship1.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship1[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship1[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship2

                    if (Ship2 != null && Ship2.Count > 0)
                    {
                        var weight = Ship2.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship2[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship2[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship3

                    if (Ship3 != null && Ship3.Count > 0)
                    {
                        var weight = Ship3.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship3[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship3[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship4

                    if (Ship4 != null && Ship4.Count > 0)
                    {
                        var weight = Ship4.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship4[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship4[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship5

                    if (Ship5 != null && Ship5.Count > 0)
                    {
                        var weight = Ship5.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship5[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship5[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship6

                    if (Ship6 != null && Ship6.Count > 0)
                    {
                        var weight = Ship6.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship6[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship6[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship7

                    if (Ship7 != null && Ship7.Count > 0)
                    {
                        var weight = Ship7.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship7[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship7[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship8

                    if (Ship8 != null && Ship8.Count > 0)
                    {
                        var weight = Ship8.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship8[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship8[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship9

                    if (Ship9 != null && Ship9.Count > 0)
                    {
                        var weight = Ship9.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship9[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship9[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship10

                    if (Ship10 != null && Ship10.Count > 0)
                    {
                        var weight = Ship10.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship10[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship10[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship11

                    if (Ship11 != null && Ship11.Count > 0)
                    {
                        var weight = Ship11.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship11[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship11[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship12

                    if (Ship12 != null && Ship12.Count > 0)
                    {
                        var weight = Ship12.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship12[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship12[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship13

                    if (Ship13 != null && Ship13.Count > 0)
                    {
                        var weight = Ship13.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship13[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship13[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    yodel.B2B = new List<FrayteBaseRate>();
                    yodel.B2B = _b2b;
                }

                var b2cNeighbour = svc.FindAll(p => p.LogisticWeight.PackageType == "B2CNeighborhood").OrderBy(x => x.zone.ZoneId).ToList();
                if (b2cNeighbour != null && b2cNeighbour.Count > 0)
                {
                    var Ship1 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXP24").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship2 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXP48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship3 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXPNI").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship4 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "HOM72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship5 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "HOM72IN").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship6 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "PRI12MF").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship7 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "SATPRI12").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship8 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXPSAT").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship9 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXP72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship10 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXPISLScHi").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship11 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "DNOSPChIs").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship12 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "ROIR48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship13 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "ROIR72").OrderBy(x => x.zone.ZoneId).ToList();

                    FrayteBaseRate rate;
                    _b2cNeighbour = new List<FrayteBaseRate>();

                    #region Ship1

                    if (Ship1 != null && Ship1.Count > 0)
                    {
                        var weight = Ship1.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship1[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship1[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship2

                    if (Ship2 != null && Ship2.Count > 0)
                    {
                        var weight = Ship2.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship2[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship2[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship3

                    if (Ship3 != null && Ship3.Count > 0)
                    {
                        var weight = Ship3.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship3[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship3[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship4

                    if (Ship4 != null && Ship4.Count > 0)
                    {
                        var weight = Ship4.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship4[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship4[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship5

                    if (Ship5 != null && Ship5.Count > 0)
                    {
                        var weight = Ship5.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship5[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship5[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship6

                    if (Ship6 != null && Ship6.Count > 0)
                    {
                        var weight = Ship6.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship6[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship6[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship7

                    if (Ship7 != null && Ship7.Count > 0)
                    {
                        var weight = Ship7.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship7[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship7[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship8

                    if (Ship8 != null && Ship8.Count > 0)
                    {
                        var weight = Ship8.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship8[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship8[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship9

                    if (Ship9 != null && Ship9.Count > 0)
                    {
                        var weight = Ship9.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship9[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship9[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship10

                    if (Ship10 != null && Ship10.Count > 0)
                    {
                        var weight = Ship10.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship10[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship10[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship11

                    if (Ship11 != null && Ship11.Count > 0)
                    {
                        var weight = Ship11.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship11[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship11[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship12

                    if (Ship12 != null && Ship12.Count > 0)
                    {
                        var weight = Ship12.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship12[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship12[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship13

                    if (Ship13 != null && Ship13.Count > 0)
                    {
                        var weight = Ship13.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship13[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship13[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    yodel.B2CNeighbourhood = new List<FrayteBaseRate>();
                    yodel.B2CNeighbourhood = _b2cNeighbour;
                }

                var b2cHome = svc.FindAll(p => p.LogisticWeight.PackageType == "B2CHome").OrderBy(x => x.zone.ZoneId).ToList();
                if (b2cHome != null && b2cHome.Count > 0)
                {
                    var Ship1 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXP24").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship2 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXP48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship3 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXPNI").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship4 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "HOM72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship5 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "HOM72IN").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship6 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "PRI12MF").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship7 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "SATPRI12").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship8 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXPSAT").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship9 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXP72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship10 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXPISLScHi").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship11 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "DNOSPChIs").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship12 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "ROIR48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship13 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "ROIR72").OrderBy(x => x.zone.ZoneId).ToList();

                    FrayteBaseRate rate;
                    _b2cHome = new List<FrayteBaseRate>();

                    #region Ship1

                    if (Ship1 != null && Ship1.Count > 0)
                    {
                        var weight = Ship1.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship1[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship1[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship2

                    if (Ship2 != null && Ship2.Count > 0)
                    {
                        var weight = Ship2.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship2[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship2[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship3

                    if (Ship3 != null && Ship3.Count > 0)
                    {
                        var weight = Ship3.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship3[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship3[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship4

                    if (Ship4 != null && Ship4.Count > 0)
                    {
                        var weight = Ship4.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship4[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship4[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship5

                    if (Ship5 != null && Ship5.Count > 0)
                    {
                        var weight = Ship5.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship5[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship5[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship6

                    if (Ship6 != null && Ship6.Count > 0)
                    {
                        var weight = Ship6.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship6[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship6[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship7

                    if (Ship7 != null && Ship7.Count > 0)
                    {
                        var weight = Ship7.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship7[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship7[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship8

                    if (Ship8 != null && Ship8.Count > 0)
                    {
                        var weight = Ship8.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship8[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship8[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship9

                    if (Ship9 != null && Ship9.Count > 0)
                    {
                        var weight = Ship9.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship9[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship9[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship10

                    if (Ship10 != null && Ship10.Count > 0)
                    {
                        var weight = Ship10.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship10[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship10[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship11

                    if (Ship11 != null && Ship11.Count > 0)
                    {
                        var weight = Ship11.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship11[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship11[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship12

                    if (Ship12 != null && Ship12.Count > 0)
                    {
                        var weight = Ship12.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship12[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship12[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship13

                    if (Ship13 != null && Ship13.Count > 0)
                    {
                        var weight = Ship13.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship13[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship13[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    yodel.B2CHome = new List<FrayteBaseRate>();
                    yodel.B2CHome = _b2cHome;
                }

                _yodel.Add(yodel);
            }

            #endregion

            #region TRPOD

            if (tr != null && tr.Count > 0)
            {
                FrayteYodel yodel = new FrayteYodel();

                var b2b = tr.FindAll(p => p.LogisticWeight.PackageType == "B2B").OrderBy(x => x.zone.ZoneId).ToList();
                if (b2b != null && b2b.Count > 0)
                {
                    var Ship1 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXP24").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship2 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXP48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship3 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXPNI").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship4 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "HOM72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship5 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "HOM72IN").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship6 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "PRI12MF").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship7 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "SATPRI12").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship8 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXPSAT").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship9 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXP72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship10 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "EXPISLScHi").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship11 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "DNOSPChIs").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship12 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "ROIR48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship13 = b2b.FindAll(p => p.shipmentType.LogisticDescription == "ROIR72").OrderBy(x => x.zone.ZoneId).ToList();

                    FrayteBaseRate rate;
                    _b2b = new List<FrayteBaseRate>();

                    #region Ship1

                    if (Ship1 != null && Ship1.Count > 0)
                    {
                        var weight = Ship1.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship1[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship1[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship2

                    if (Ship2 != null && Ship2.Count > 0)
                    {
                        var weight = Ship2.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship2[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship2[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship3

                    if (Ship3 != null && Ship3.Count > 0)
                    {
                        var weight = Ship3.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship3[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship3[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship4

                    if (Ship4 != null && Ship4.Count > 0)
                    {
                        var weight = Ship4.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship4[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship4[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship5

                    if (Ship5 != null && Ship5.Count > 0)
                    {
                        var weight = Ship5.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship5[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship5[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship6

                    if (Ship6 != null && Ship6.Count > 0)
                    {
                        var weight = Ship6.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship6[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship6[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship7

                    if (Ship7 != null && Ship7.Count > 0)
                    {
                        var weight = Ship7.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship7[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship7[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship8

                    if (Ship8 != null && Ship8.Count > 0)
                    {
                        var weight = Ship8.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship8[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship8[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship9

                    if (Ship9 != null && Ship9.Count > 0)
                    {
                        var weight = Ship9.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship9[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship9[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship10

                    if (Ship10 != null && Ship10.Count > 0)
                    {
                        var weight = Ship10.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship10[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship10[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship11

                    if (Ship11 != null && Ship11.Count > 0)
                    {
                        var weight = Ship11.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship11[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship11[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship12

                    if (Ship12 != null && Ship12.Count > 0)
                    {
                        var weight = Ship12.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship12[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship12[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship13

                    if (Ship13 != null && Ship13.Count > 0)
                    {
                        var weight = Ship13.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship13[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship13[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 1;
                            _b2b.Add(rate);
                        }
                    }

                    #endregion

                    yodel.B2B = new List<FrayteBaseRate>();
                    yodel.B2B = _b2b;
                }

                var b2cNeighbour = tr.FindAll(p => p.LogisticWeight.PackageType == "B2CNeighborhood").OrderBy(x => x.zone.ZoneId).ToList();
                if (b2cNeighbour != null && b2cNeighbour.Count > 0)
                {
                    var Ship1 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXP24").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship2 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXP48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship3 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXPNI").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship4 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "HOM72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship5 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "HOM72IN").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship6 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "PRI12MF").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship7 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "SATPRI12").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship8 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXPSAT").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship9 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXP72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship10 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "EXPISLScHi").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship11 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "DNOSPChIs").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship12 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "ROIR48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship13 = b2cNeighbour.FindAll(p => p.shipmentType.LogisticDescription == "ROIR72").OrderBy(x => x.zone.ZoneId).ToList();

                    FrayteBaseRate rate;
                    _b2cNeighbour = new List<FrayteBaseRate>();

                    #region Ship1

                    if (Ship1 != null && Ship1.Count > 0)
                    {
                        var weight = Ship1.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship1[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship1[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship2

                    if (Ship2 != null && Ship2.Count > 0)
                    {
                        var weight = Ship2.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship2[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship2[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship3

                    if (Ship3 != null && Ship3.Count > 0)
                    {
                        var weight = Ship3.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship3[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship3[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship4

                    if (Ship4 != null && Ship4.Count > 0)
                    {
                        var weight = Ship4.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship4[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship4[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship5

                    if (Ship5 != null && Ship5.Count > 0)
                    {
                        var weight = Ship5.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship5[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship5[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship6

                    if (Ship6 != null && Ship6.Count > 0)
                    {
                        var weight = Ship6.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship6[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship6[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship7

                    if (Ship7 != null && Ship7.Count > 0)
                    {
                        var weight = Ship7.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship7[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship7[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship8

                    if (Ship8 != null && Ship8.Count > 0)
                    {
                        var weight = Ship8.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship8[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship8[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship9

                    if (Ship9 != null && Ship9.Count > 0)
                    {
                        var weight = Ship9.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship9[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship9[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship10

                    if (Ship10 != null && Ship10.Count > 0)
                    {
                        var weight = Ship10.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship10[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship10[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship11

                    if (Ship11 != null && Ship11.Count > 0)
                    {
                        var weight = Ship11.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship11[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship11[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship12

                    if (Ship12 != null && Ship12.Count > 0)
                    {
                        var weight = Ship12.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship12[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship12[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship13

                    if (Ship13 != null && Ship13.Count > 0)
                    {
                        var weight = Ship13.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship13[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship13[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 2;
                            _b2cNeighbour.Add(rate);
                        }
                    }

                    #endregion

                    yodel.B2CNeighbourhood = new List<FrayteBaseRate>();
                    yodel.B2CNeighbourhood = _b2cNeighbour;
                }

                var b2cHome = tr.FindAll(p => p.LogisticWeight.PackageType == "B2CHome").OrderBy(x => x.zone.ZoneId).ToList();
                if (b2cHome != null && b2cHome.Count > 0)
                {
                    var Ship1 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXP24").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship2 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXP48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship3 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXPNI").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship4 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "HOM72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship5 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "HOM72IN").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship6 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "PRI12MF").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship7 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "SATPRI12").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship8 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXPSAT").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship9 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXP72").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship10 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "EXPISLScHi").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship11 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "DNOSPChIs").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship12 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "ROIR48").OrderBy(x => x.zone.ZoneId).ToList();
                    var Ship13 = b2cHome.FindAll(p => p.shipmentType.LogisticDescription == "ROIR72").OrderBy(x => x.zone.ZoneId).ToList();

                    FrayteBaseRate rate;
                    _b2cHome = new List<FrayteBaseRate>();

                    #region Ship1

                    if (Ship1 != null && Ship1.Count > 0)
                    {
                        var weight = Ship1.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship1[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship1[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship2

                    if (Ship2 != null && Ship2.Count > 0)
                    {
                        var weight = Ship2.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship2[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship2[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship3

                    if (Ship3 != null && Ship3.Count > 0)
                    {
                        var weight = Ship3.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship3[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship3[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship4

                    if (Ship4 != null && Ship4.Count > 0)
                    {
                        var weight = Ship4.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship4[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship4[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship5

                    if (Ship5 != null && Ship5.Count > 0)
                    {
                        var weight = Ship5.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship5[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship5[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship6

                    if (Ship6 != null && Ship6.Count > 0)
                    {
                        var weight = Ship6.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship6[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship6[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship7

                    if (Ship7 != null && Ship7.Count > 0)
                    {
                        var weight = Ship7.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship7[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship7[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship8

                    if (Ship8 != null && Ship8.Count > 0)
                    {
                        var weight = Ship8.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship8[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship8[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship9

                    if (Ship9 != null && Ship9.Count > 0)
                    {
                        var weight = Ship9.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship9[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship9[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship10

                    if (Ship10 != null && Ship10.Count > 0)
                    {
                        var weight = Ship10.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship10[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship10[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship11

                    if (Ship11 != null && Ship11.Count > 0)
                    {
                        var weight = Ship11.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship11[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship11[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship12

                    if (Ship12 != null && Ship12.Count > 0)
                    {
                        var weight = Ship12.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship12[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship12[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    #region Ship13

                    if (Ship13 != null && Ship13.Count > 0)
                    {
                        var weight = Ship13.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.PackageType = Ship13[i].LogisticWeight.PackageDisplayType;
                            rate.ParcelType = Ship13[i].LogisticWeight.ParcelDisplayType + " per parcel rate";
                            rate.ShipmentType = Ship13[i].shipmentType.LogisticDescriptionDisplay;
                            rate.Weight = weight[i].weights.WeightTo.ToString();
                            for (int j = 0; j < weight[i].Rates.Count; j++)
                            {
                                if (j == 0)
                                {
                                    rate.Rate1 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 1)
                                {
                                    rate.Rate2 = weight[i].Rates[j].Rate;
                                }
                                else if (j == 2)
                                {
                                    rate.Rate3 = weight[i].Rates[j].Rate;
                                }
                            }
                            rate.SortOrder = 3;
                            _b2cHome.Add(rate);
                        }
                    }

                    #endregion

                    yodel.B2CHome = new List<FrayteBaseRate>();
                    yodel.B2CHome = _b2cHome;
                }

                _yodel.Add(yodel);
            }

            #endregion

            if (_yodel != null && _yodel.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCard/"));
                ReportTemplate.UKRateCards.Yodel report = new ReportTemplate.UKRateCards.Yodel();
                report.Parameters["lblHeader"].Value = "Yodel UK Shipments Summery Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "GBP";
                report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(LogisticServiceId, OperationZoneId, DateTime.Now.Month, DateTime.Now.Year);
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.DataSource = _yodel;
                string FileName = "Yodel UK Shipments Rate Card (" + DateTime.Today.Year + ") - GBP" + ".xlsx";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }
            return result;
        }

        internal FrayteManifestName UKHermes(int LogisticServiceId, int OperationZoneId)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(LogisticServiceId);

            var filter = new ViewBaseRateCardRepository().ExportUKBaseRateExcel(logisticServices.OperationZoneId, logisticServices.LogisticType, logisticServices.LogisticCompany, logisticServices.ModuleType);
            var logdate = new CustomerRepository().LogisticServiceDates(LogisticServiceId);
            List<FrayteHermesParcelPOD> _ParcelPOD = new List<FrayteHermesParcelPOD>();
            List<FrayteHermesPacketPOD> _PacketPOD = new List<FrayteHermesPacketPOD>();
            List<FrayteHermesParcelNONPOD> _ParcelNONPOD = new List<FrayteHermesParcelNONPOD>();
            List<FrayteHermesPacketNONPOD> _PacketNONPOD = new List<FrayteHermesPacketNONPOD>();
            List<FrayteHermesBaseRate> _hermes = new List<FrayteHermesBaseRate>();

            var packetitem = filter.FindAll(p => p.shipmentType.LogisticDescription == "Packet").OrderBy(x => x.zone.ZoneId).ToList();
            var parcelitem = filter.FindAll(p => p.shipmentType.LogisticDescription == "Parcel").OrderBy(x => x.zone.ZoneId).ToList();

            #region PacketPODRate

            if (packetitem != null && packetitem.Count > 0)
            {
                FrayteHermesPacketPOD rate;

                #region POD

                var poditem = packetitem.FindAll(p => p.LogisticWeight.PackageType == "POD").ToList();
                var ship = poditem.Select(p => p.shipmentType.LogisticDescription).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var packetweight = poditem.GroupBy(x => x.LogisticWeight.WeightTo).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList(), Zones = g.Select(r => new { r.zone.ZoneDisplayName }).ToList() }).ToList();
                    for (int i = 0; i < packetweight.Count; i++)
                    {
                        rate = new FrayteHermesPacketPOD();
                        rate.ShipmentType = "POD";
                        rate.PackageType = ship[0].ToString();
                        rate.Weight = packetweight[i].weights.ToString();
                        for (int j = 0; j < packetweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = packetweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = packetweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = packetweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 1;
                        _PacketPOD.Add(rate);
                    }
                }

                #endregion                
            }

            #endregion

            #region ParcelPODRate

            if (parcelitem != null && parcelitem.Count > 0)
            {
                FrayteHermesParcelPOD rate;

                #region POD

                var poditem = parcelitem.FindAll(p => p.LogisticWeight.PackageType == "POD").ToList();
                var ship = poditem.Select(p => p.shipmentType.LogisticDescription).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var packetweight = poditem.GroupBy(x => x.LogisticWeight.WeightTo).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList(), Zones = g.Select(r => new { r.zone.ZoneDisplayName }).ToList() }).ToList();
                    for (int i = 0; i < packetweight.Count; i++)
                    {
                        rate = new FrayteHermesParcelPOD();
                        rate.ShipmentType = "POD";
                        rate.PackageType = ship[0].ToString();
                        rate.Weight = packetweight[i].weights.ToString();
                        for (int j = 0; j < packetweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = packetweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = packetweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = packetweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 2;
                        _ParcelPOD.Add(rate);
                    }
                }

                #endregion                
            }

            #endregion

            #region PacketNONPODRate

            if (packetitem != null && packetitem.Count > 0)
            {
                FrayteHermesPacketNONPOD rate;

                #region NONPOD

                var nonpod = packetitem.FindAll(p => p.LogisticWeight.PackageType == "NONPOD").ToList();
                var nonship = nonpod.Select(p => p.shipmentType.LogisticDescription).Distinct().ToList();
                if (nonship != null && nonship.Count > 0)
                {
                    var packetweight = nonpod.GroupBy(x => x.LogisticWeight.WeightTo).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList(), Zones = g.Select(r => new { r.zone.ZoneDisplayName }).ToList() }).ToList();
                    for (int i = 0; i < packetweight.Count; i++)
                    {
                        rate = new FrayteHermesPacketNONPOD();
                        rate.ShipmentType = "NONPOD";
                        rate.PackageType = nonship[0].ToString();
                        rate.Weight = packetweight[i].weights.ToString();
                        for (int j = 0; j < packetweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = packetweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = packetweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = packetweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 3;
                        _PacketNONPOD.Add(rate);
                    }
                }

                #endregion
            }

            #endregion

            #region ParcelNONPODRate

            if (parcelitem != null && parcelitem.Count > 0)
            {
                FrayteHermesParcelNONPOD rate;

                #region NONPOD

                var nonpod = parcelitem.FindAll(p => p.LogisticWeight.PackageType == "NONPOD").ToList();
                var nonship = nonpod.Select(p => p.shipmentType.LogisticDescription).Distinct().ToList();
                if (nonship != null && nonship.Count > 0)
                {
                    var packetweight = nonpod.GroupBy(x => x.LogisticWeight.WeightTo).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList(), Zones = g.Select(r => new { r.zone.ZoneDisplayName }).ToList() }).ToList();
                    for (int i = 0; i < packetweight.Count; i++)
                    {
                        rate = new FrayteHermesParcelNONPOD();
                        rate.ShipmentType = "NONPOD";
                        rate.PackageType = nonship[0].ToString();
                        rate.Weight = packetweight[i].weights.ToString();
                        for (int j = 0; j < packetweight[i].Rates.Count; j++)
                        {
                            if (j == 0)
                            {
                                rate.Rate1 = packetweight[i].Rates[j].Rate;
                            }
                            else if (j == 1)
                            {
                                rate.Rate2 = packetweight[i].Rates[j].Rate;
                            }
                            else if (j == 2)
                            {
                                rate.Rate3 = packetweight[i].Rates[j].Rate;
                            }
                        }
                        rate.SortOrder = 4;
                        _ParcelNONPOD.Add(rate);
                    }
                }

                #endregion
            }

            #endregion

            FrayteHermesBaseRate hermes = new FrayteHermesBaseRate();
            hermes.ParcelPOD = new List<FrayteHermesParcelPOD>();
            hermes.ParcelPOD = _ParcelPOD;
            hermes.PacketPOD = new List<FrayteHermesPacketPOD>();
            hermes.PacketPOD = _PacketPOD;
            hermes.ParcelNONPOD = new List<FrayteHermesParcelNONPOD>();
            hermes.ParcelNONPOD = _ParcelNONPOD;
            hermes.PacketNONPOD = new List<FrayteHermesPacketNONPOD>();
            hermes.PacketNONPOD = _PacketNONPOD;
            _hermes.Add(hermes);

            if (_hermes != null && _hermes.Count > 0)
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "BaseRateCard/"));
                ReportTemplate.UKRateCards.Hermes report = new ReportTemplate.UKRateCards.Hermes();
                report.Parameters["lblHeader"].Value = "Hermes POD and NONPOD Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                report.Parameters["lblCurrency"].Value = "GBP";
                report.Parameters["lblShipment"].Value = "Rate / Parcel Up to 3 Working Day Service";
                report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                report.DataSource = _hermes;
                string FileName = "Hermes UK Shipments Rate Card (" + DateTime.Today.Year + ") - GBP" + ".xlsx";
                report.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "BaseRateCard/" + FileName);
                result = new ViewBaseRateCardRepository().SaveLogisticServiceBaseRateHistory(LogisticServiceId, DateTime.Today.Year, FileName);
            }
            return result;
        }
    }
}