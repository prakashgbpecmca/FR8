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
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;

namespace Report.Generator.ManifestReport
{
    public class CustomerBaseRateReport
    {
        public FrayteManifestName CustomerBaseRate(CustomerRate User)
        {
            FrayteManifestName result = new FrayteManifestName();

            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(User.LogisticServiceId);

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
                                            result = HKDHLImportExpress(User, logisticServices.OperationZoneId, "");
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
                                            result = HKDHLExportExpress(User, logisticServices.OperationZoneId, "");
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
                                            result = HKDHL3rdPartyExpress(User, logisticServices.OperationZoneId, "");
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
                                            result = HKTNTImportExpress(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = HKTNTImportEconomy(User, logisticServices.OperationZoneId, "");
                                            break;
                                    }
                                    break;
                                case FrayteLogisticType.Export:
                                    switch (logisticServices.RateType)
                                    {
                                        case FrayteLogisticRateType.Express:
                                            result = HKTNTExportExpress(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = HKTNTExportEconomy(User, logisticServices.OperationZoneId, "");
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
                                            result = HKUPSImportSaverLanes(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.ExpressLanes:
                                            result = HKUPSImportExpressLanes(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Saver:
                                            result = HKUPSImportSaver(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Express:
                                            result = HKUPSImportExpress(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Expedited:
                                            result = HKUPSImportExpedited(User, logisticServices.OperationZoneId, "");
                                            break;
                                    }
                                    break;
                                case FrayteLogisticType.Export:
                                    switch (logisticServices.RateType)
                                    {
                                        case FrayteLogisticRateType.SaverLanes:
                                            result = HKUPSExportSaverLanes(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.ExpressLanes:
                                            result = HKUPSExportExpressLanes(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Saver:
                                            result = HKUPSExportSaver(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Express:
                                            result = HKUPSExportExpress(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Expedited:
                                            result = HKUPSExportExpedited(User, logisticServices.OperationZoneId, "");
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
                                    result = UKUKMail(User, logisticServices.OperationZoneId, "");
                                    break;
                            }
                            break;
                        case FrayteCourierCompany.Yodel:
                            switch (logisticServices.LogisticType)
                            {
                                case FrayteLogisticType.UKShipment:
                                    result = UKYodel(User, logisticServices.OperationZoneId, "");
                                    break;
                            }
                            break;
                        case FrayteCourierCompany.Hermes:
                            switch (logisticServices.LogisticType)
                            {
                                case FrayteLogisticType.UKShipment:
                                    result = UKHermes(User, logisticServices.OperationZoneId, "");
                                    break;
                            }
                            break;
                        case FrayteCourierCompany.DHL:
                            switch (logisticServices.LogisticType)
                            {
                                case FrayteLogisticType.UKShipment:
                                    result = UKDHLDomestic(User, logisticServices.OperationZoneId, "");
                                    break;
                                case FrayteLogisticType.Import:
                                    switch (logisticServices.RateType)
                                    {
                                        case FrayteLogisticRateType.Express:
                                            result = UKDHLImportExpress(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = UKDHLImportEconomy(User, logisticServices.OperationZoneId, "");
                                            break;
                                    }
                                    break;
                                case FrayteLogisticType.Export:
                                    switch (logisticServices.RateType)
                                    {
                                        case FrayteLogisticRateType.Express:
                                            result = UKDHLExportExpress(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = UKDHLExportEconomy(User, logisticServices.OperationZoneId, "");
                                            break;
                                    }
                                    break;
                                case FrayteLogisticType.ThirdParty:
                                    switch (logisticServices.RateType)
                                    {
                                        case FrayteLogisticRateType.Express:
                                            result = UKDHL3rdPartyExpress(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = UKDHL3rdPartyEconomy(User, logisticServices.OperationZoneId, ""); ;
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
                                            result = UKTNTImportExpress(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = UKTNTImportEconomy(User, logisticServices.OperationZoneId, "");
                                            break;
                                    }
                                    break;
                                case FrayteLogisticType.Export:
                                    switch (logisticServices.RateType)
                                    {
                                        case FrayteLogisticRateType.Express:
                                            result = UKTNTExportExpress(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = UKTNTExportEconomy(User, logisticServices.OperationZoneId, "");
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
            return result;
        }

        public FrayteManifestName CustomerBaseRateSummery(CustomerRate User)
        {
            FrayteManifestName result = new FrayteManifestName();

            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(User.LogisticServiceId);

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
                                            result = HKDHLImportExpress(User, logisticServices.OperationZoneId, "Summery");
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
                                            result = HKDHLExportExpress(User, logisticServices.OperationZoneId, "Summery");
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
                                            result = HKDHL3rdPartyExpress(User, logisticServices.OperationZoneId, "Summery");
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
                                            result = HKTNTImportExpress(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = HKTNTImportEconomy(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                    }
                                    break;
                                case FrayteLogisticType.Export:
                                    switch (logisticServices.RateType)
                                    {
                                        case FrayteLogisticRateType.Express:
                                            result = HKTNTExportExpress(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = HKTNTExportEconomy(User, logisticServices.OperationZoneId, "Summery");
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
                                            result = HKUPSImportSaverLanes(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                        case FrayteLogisticRateType.ExpressLanes:
                                            result = HKUPSImportExpressLanes(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                        case FrayteLogisticRateType.Saver:
                                            result = HKUPSImportSaver(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                        case FrayteLogisticRateType.Express:
                                            result = HKUPSImportExpress(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                        case FrayteLogisticRateType.Expedited:
                                            result = HKUPSImportExpedited(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                    }
                                    break;
                                case FrayteLogisticType.Export:
                                    switch (logisticServices.RateType)
                                    {
                                        case FrayteLogisticRateType.SaverLanes:
                                            result = HKUPSExportSaverLanes(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                        case FrayteLogisticRateType.ExpressLanes:
                                            result = HKUPSExportExpressLanes(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                        case FrayteLogisticRateType.Saver:
                                            result = HKUPSExportSaver(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                        case FrayteLogisticRateType.Express:
                                            result = HKUPSExportExpress(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                        case FrayteLogisticRateType.Expedited:
                                            result = HKUPSExportExpedited(User, logisticServices.OperationZoneId, "Summery");
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
                                    result = UKUKMail(User, logisticServices.OperationZoneId, "Summery");
                                    break;
                            }
                            break;
                        case FrayteCourierCompany.Yodel:
                            switch (logisticServices.LogisticType)
                            {
                                case FrayteLogisticType.UKShipment:
                                    result = UKYodel(User, logisticServices.OperationZoneId, "Summery");
                                    break;
                            }
                            break;
                        case FrayteCourierCompany.Hermes:
                            switch (logisticServices.LogisticType)
                            {
                                case FrayteLogisticType.UKShipment:
                                    result = UKHermes(User, logisticServices.OperationZoneId, "Summery");
                                    break;
                            }
                            break;
                        case FrayteCourierCompany.DHL:
                            switch (logisticServices.LogisticType)
                            {
                                case FrayteLogisticType.UKShipment:
                                    result = UKDHLDomestic(User, logisticServices.OperationZoneId, "Summery");
                                    break;
                                case FrayteLogisticType.Import:
                                    switch (logisticServices.RateType)
                                    {
                                        case FrayteLogisticRateType.Express:
                                            result = UKDHLImportExpress(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = UKDHLImportEconomy(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                    }
                                    break;
                                case FrayteLogisticType.Export:
                                    switch (logisticServices.RateType)
                                    {
                                        case FrayteLogisticRateType.Express:
                                            result = UKDHLExportExpress(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = UKDHLExportEconomy(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                    }
                                    break;
                                case FrayteLogisticType.ThirdParty:
                                    switch (logisticServices.RateType)
                                    {
                                        case FrayteLogisticRateType.Express:
                                            result = UKDHL3rdPartyExpress(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = UKDHL3rdPartyEconomy(User, logisticServices.OperationZoneId, "Summery");
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
                                            result = UKTNTImportExpress(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = UKTNTImportEconomy(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                    }
                                    break;
                                case FrayteLogisticType.Export:
                                    switch (logisticServices.RateType)
                                    {
                                        case FrayteLogisticRateType.Express:
                                            result = UKTNTExportExpress(User, logisticServices.OperationZoneId, "Summery");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = UKTNTExportEconomy(User, logisticServices.OperationZoneId, "Summery");
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
            return result;
        }

        public FrayteManifestName CustomerQuoteRateCard(int QuotationShipmentId)
        {
            FrayteManifestName result = new FrayteManifestName();
            var User = new QuotationRepository().GetLogsiticServiceId(QuotationShipmentId);
            var logisticServices = new ViewBaseRateCardRepository().GetLogisticServices(User.LogisticServiceId);

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
                                            result = HKDHLImportExpress(User, logisticServices.OperationZoneId, "");
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
                                            result = HKDHLExportExpress(User, logisticServices.OperationZoneId, "");
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
                                            result = HKDHL3rdPartyExpress(User, logisticServices.OperationZoneId, "");
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
                                            result = HKTNTImportExpress(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = HKTNTImportEconomy(User, logisticServices.OperationZoneId, "");
                                            break;
                                    }
                                    break;
                                case FrayteLogisticType.Export:
                                    switch (logisticServices.RateType)
                                    {
                                        case FrayteLogisticRateType.Express:
                                            result = HKTNTExportExpress(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = HKTNTExportEconomy(User, logisticServices.OperationZoneId, "");
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
                                            result = HKUPSImportSaverLanes(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.ExpressLanes:
                                            result = HKUPSImportExpressLanes(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Saver:
                                            result = HKUPSImportSaver(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Express:
                                            result = HKUPSImportExpress(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Expedited:
                                            result = HKUPSImportExpedited(User, logisticServices.OperationZoneId, "");
                                            break;
                                    }
                                    break;
                                case FrayteLogisticType.Export:
                                    switch (logisticServices.RateType)
                                    {
                                        case FrayteLogisticRateType.SaverLanes:
                                            result = HKUPSExportSaverLanes(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.ExpressLanes:
                                            result = HKUPSExportExpressLanes(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Saver:
                                            result = HKUPSExportSaver(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Express:
                                            result = HKUPSExportExpress(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Expedited:
                                            result = HKUPSExportExpedited(User, logisticServices.OperationZoneId, "");
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
                                    result = UKUKMail(User, logisticServices.OperationZoneId, "");
                                    break;
                            }
                            break;
                        case FrayteCourierCompany.Yodel:
                            switch (logisticServices.LogisticType)
                            {
                                case FrayteLogisticType.UKShipment:
                                    result = UKYodel(User, logisticServices.OperationZoneId, "");
                                    break;
                            }
                            break;
                        case FrayteCourierCompany.Hermes:
                            switch (logisticServices.LogisticType)
                            {
                                case FrayteLogisticType.UKShipment:
                                    result = UKHermes(User, logisticServices.OperationZoneId, "");
                                    break;
                            }
                            break;
                        case FrayteCourierCompany.DHL:
                            switch (logisticServices.LogisticType)
                            {
                                case FrayteLogisticType.UKShipment:
                                    result = UKDHLDomestic(User, logisticServices.OperationZoneId, "");
                                    break;
                                case FrayteLogisticType.Import:
                                    switch (logisticServices.RateType)
                                    {
                                        case FrayteLogisticRateType.Express:
                                            result = UKDHLImportExpress(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = UKDHLImportEconomy(User, logisticServices.OperationZoneId, "");
                                            break;
                                    }
                                    break;
                                case FrayteLogisticType.Export:
                                    switch (logisticServices.RateType)
                                    {
                                        case FrayteLogisticRateType.Express:
                                            result = UKDHLExportExpress(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = UKDHLExportEconomy(User, logisticServices.OperationZoneId, "");
                                            break;
                                    }
                                    break;
                                case FrayteLogisticType.ThirdParty:
                                    switch (logisticServices.RateType)
                                    {
                                        case FrayteLogisticRateType.Express:
                                            result = UKDHL3rdPartyExpress(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = UKDHL3rdPartyEconomy(User, logisticServices.OperationZoneId, ""); ;
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
                                            result = UKTNTImportExpress(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = UKTNTImportEconomy(User, logisticServices.OperationZoneId, "");
                                            break;
                                    }
                                    break;
                                case FrayteLogisticType.Export:
                                    switch (logisticServices.RateType)
                                    {
                                        case FrayteLogisticRateType.Express:
                                            result = UKTNTExportExpress(User, logisticServices.OperationZoneId, "");
                                            break;
                                        case FrayteLogisticRateType.Economy:
                                            result = UKTNTExportEconomy(User, logisticServices.OperationZoneId, "");
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
            return result;
        }

        protected internal FrayteManifestName HKDHLImportExpress(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.HeavyWeight).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                var Heavyship = HeavyWeightitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.Weight.ToString();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.HKRateCards.DHLImportExpressSummery report = new ReportTemplate.HKRateCards.DHLImportExpressSummery();
                    report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.HKRateCards.DHLImportExpress report = new ReportTemplate.HKRateCards.DHLImportExpress();
                        report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.DHLImportExpress report = new ReportTemplate.HKRateCards.DHLImportExpress();
                        report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.DHLImportExpress report = new ReportTemplate.HKRateCards.DHLImportExpress();
                        report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName HKDHLExportExpress(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.HeavyWeight).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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

            #region NondocitemDocAndNondocitem

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                var Heavyship = HeavyWeightitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.Weight.ToString();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.HKRateCards.DHLExportExpressSummery report = new ReportTemplate.HKRateCards.DHLExportExpressSummery();
                    report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.HKRateCards.DHLExportExpress report = new ReportTemplate.HKRateCards.DHLExportExpress();
                        report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.DHLExportExpress report = new ReportTemplate.HKRateCards.DHLExportExpress();
                        report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.DHLExportExpress report = new ReportTemplate.HKRateCards.DHLExportExpress();
                        report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName HKDHL3rdPartyExpress(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var matrix = new ThirdPartyMarixRepository().GetThirdPartyMatrix(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);
            var zone = new ThirdPartyMarixRepository().GetZoneDetail(User.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteZoneMatrix> _matrix = new List<FrayteZoneMatrix>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.HeavyWeight).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                var Heavyship = HeavyWeightitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.Weight.ToString();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.HKRateCards.DHL3rdPartyExpressSummery report = new ReportTemplate.HKRateCards.DHL3rdPartyExpressSummery();
                    report.Parameters["lblHeader"].Value = "DHL 3rd Party " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblThirdParty"].Value = "DHL 3rd Party Zone Matrix " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "DHL 3rd Party " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "DHL 3rd Party " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.HKRateCards.DHL3rdPartyExpress report = new ReportTemplate.HKRateCards.DHL3rdPartyExpress();
                        report.Parameters["lblHeader"].Value = "DHL 3rd Party " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblThirdParty"].Value = "DHL 3rd Party Zone Matrix " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL 3rd Party " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.DHL3rdPartyExpress report = new ReportTemplate.HKRateCards.DHL3rdPartyExpress();
                        report.Parameters["lblHeader"].Value = "DHL 3rd Party " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblThirdParty"].Value = "DHL 3rd Party Zone Matrix " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL 3rd Party " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.DHL3rdPartyExpress report = new ReportTemplate.HKRateCards.DHL3rdPartyExpress();
                        report.Parameters["lblHeader"].Value = "DHL 3rd Party " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblThirdParty"].Value = "DHL 3rd Party Zone Matrix " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL 3rd Party " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName HKTNTImportExpress(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.HeavyWeight).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                var Heavyship = HeavyWeightitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.Weight.ToString();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.HKRateCards.TNTImportExpressSummery report = new ReportTemplate.HKRateCards.TNTImportExpressSummery();
                    report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.HKRateCards.TNTImportExpress report = new ReportTemplate.HKRateCards.TNTImportExpress();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.TNTImportExpress report = new ReportTemplate.HKRateCards.TNTImportExpress();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.TNTImportExpress report = new ReportTemplate.HKRateCards.TNTImportExpress();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName HKTNTImportEconomy(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.HeavyWeight).OrderBy(p => p.ZoneId).ToList();

            FrayteBaseRate rate;

            #region NondocRate

            if (Nondocitem != null && Nondocitem.Count > 0)
            {
                var DocNonship = Nondocitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = Nondocitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                var Heavyship = HeavyWeightitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.Weight.ToString();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.HKRateCards.TNTImportEconomySummery report = new ReportTemplate.HKRateCards.TNTImportEconomySummery();
                    report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.HKRateCards.TNTImportEconomy report = new ReportTemplate.HKRateCards.TNTImportEconomy();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.TNTImportEconomy report = new ReportTemplate.HKRateCards.TNTImportEconomy();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.TNTImportEconomy report = new ReportTemplate.HKRateCards.TNTImportEconomy();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName HKTNTExportExpress(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.HeavyWeight).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                var Heavyship = HeavyWeightitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.Weight.ToString();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.HKRateCards.TNTExportExpressSummery report = new ReportTemplate.HKRateCards.TNTExportExpressSummery();
                    report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.HKRateCards.TNTExportExpress report = new ReportTemplate.HKRateCards.TNTExportExpress();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.TNTExportExpress report = new ReportTemplate.HKRateCards.TNTExportExpress();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.TNTExportExpress report = new ReportTemplate.HKRateCards.TNTExportExpress();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName HKTNTExportEconomy(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.HeavyWeight).OrderBy(p => p.ZoneId).ToList();

            FrayteBaseRate rate;

            #region NondocRate

            if (Nondocitem != null && Nondocitem.Count > 0)
            {
                var DocNonship = Nondocitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = Nondocitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                var Heavyship = HeavyWeightitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.Weight.ToString();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.HKRateCards.TNTExportEconomySummery report = new ReportTemplate.HKRateCards.TNTExportEconomySummery();
                    report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.HKRateCards.TNTExportEconomy report = new ReportTemplate.HKRateCards.TNTExportEconomy();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.TNTExportEconomy report = new ReportTemplate.HKRateCards.TNTExportEconomy();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.TNTExportEconomy report = new ReportTemplate.HKRateCards.TNTExportEconomy();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName HKUPSImportSaverLanes(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.HeavyWeight).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                var Heavyship = HeavyWeightitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.Weight.ToString();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.HKRateCards.UPSImportSaverLanesSummery report = new ReportTemplate.HKRateCards.UPSImportSaverLanesSummery();
                    report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.HKRateCards.UPSImportSaverLanes report = new ReportTemplate.HKRateCards.UPSImportSaverLanes();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.UPSImportSaverLanes report = new ReportTemplate.HKRateCards.UPSImportSaverLanes();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.UPSImportSaverLanes report = new ReportTemplate.HKRateCards.UPSImportSaverLanes();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName HKUPSImportExpressLanes(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.HeavyWeight).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                var Heavyship = HeavyWeightitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.Weight.ToString();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.HKRateCards.UPSImportExpressLanesSummery report = new ReportTemplate.HKRateCards.UPSImportExpressLanesSummery();
                    report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.HKRateCards.UPSImportExpressLanes report = new ReportTemplate.HKRateCards.UPSImportExpressLanes();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.UPSImportExpressLanes report = new ReportTemplate.HKRateCards.UPSImportExpressLanes();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.UPSImportExpressLanes report = new ReportTemplate.HKRateCards.UPSImportExpressLanes();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName HKUPSImportSaver(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.HeavyWeight).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                var Heavyship = HeavyWeightitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.Weight.ToString();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.HKRateCards.UPSImportSaverSummery report = new ReportTemplate.HKRateCards.UPSImportSaverSummery();
                    report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.HKRateCards.UPSImportSaver report = new ReportTemplate.HKRateCards.UPSImportSaver();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.UPSImportSaver report = new ReportTemplate.HKRateCards.UPSImportSaver();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.UPSImportSaver report = new ReportTemplate.HKRateCards.UPSImportSaver();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName HKUPSImportExpress(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.HeavyWeight).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                var Heavyship = HeavyWeightitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.Weight.ToString();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.HKRateCards.UPSImportExpressSummery report = new ReportTemplate.HKRateCards.UPSImportExpressSummery();
                    report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.HKRateCards.UPSImportExpress report = new ReportTemplate.HKRateCards.UPSImportExpress();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.UPSImportExpress report = new ReportTemplate.HKRateCards.UPSImportExpress();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.UPSImportExpress report = new ReportTemplate.HKRateCards.UPSImportExpress();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }
            return result;
        }

        protected internal FrayteManifestName HKUPSImportExpedited(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.HeavyWeight).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                var Heavyship = HeavyWeightitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.Weight.ToString();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.HKRateCards.UPSImportExpeditedSummery report = new ReportTemplate.HKRateCards.UPSImportExpeditedSummery();
                    report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.HKRateCards.UPSImportExpedited report = new ReportTemplate.HKRateCards.UPSImportExpedited();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.UPSImportExpedited report = new ReportTemplate.HKRateCards.UPSImportExpedited();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.UPSImportExpedited report = new ReportTemplate.HKRateCards.UPSImportExpedited();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }
            return result;
        }

        protected internal FrayteManifestName HKUPSExportSaverLanes(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.HeavyWeight).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                var Heavyship = HeavyWeightitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.Weight.ToString();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.HKRateCards.UPSExportSaverLanesSummery report = new ReportTemplate.HKRateCards.UPSExportSaverLanesSummery();
                    report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.HKRateCards.UPSExportSaverLanes report = new ReportTemplate.HKRateCards.UPSExportSaverLanes();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }
                        ReportTemplate.HKRateCards.UPSExportSaverLanes report = new ReportTemplate.HKRateCards.UPSExportSaverLanes();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.UPSExportSaverLanes report = new ReportTemplate.HKRateCards.UPSExportSaverLanes();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName HKUPSExportExpressLanes(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.HeavyWeight).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                var Heavyship = HeavyWeightitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.Weight.ToString();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.HKRateCards.UPSExportExpressLanesSummery report = new ReportTemplate.HKRateCards.UPSExportExpressLanesSummery();
                    report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.HKRateCards.UPSExportExpressLanes report = new ReportTemplate.HKRateCards.UPSExportExpressLanes();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.UPSExportExpressLanes report = new ReportTemplate.HKRateCards.UPSExportExpressLanes();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.UPSExportExpressLanes report = new ReportTemplate.HKRateCards.UPSExportExpressLanes();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + logdate.RateTypeDisplay + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName HKUPSExportSaver(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.HeavyWeight).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                var Heavyship = HeavyWeightitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.Weight.ToString();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.HKRateCards.UPSExportSaverSummery report = new ReportTemplate.HKRateCards.UPSExportSaverSummery();
                    report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.HKRateCards.UPSExportSaver report = new ReportTemplate.HKRateCards.UPSExportSaver();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.UPSExportSaver report = new ReportTemplate.HKRateCards.UPSExportSaver();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.UPSExportSaver report = new ReportTemplate.HKRateCards.UPSExportSaver();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName HKUPSExportExpress(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.HeavyWeight).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                var Heavyship = HeavyWeightitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.Weight.ToString();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.HKRateCards.UPSExportExpressSummery report = new ReportTemplate.HKRateCards.UPSExportExpressSummery();
                    report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.HKRateCards.UPSExportExpress report = new ReportTemplate.HKRateCards.UPSExportExpress();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.UPSExportExpress report = new ReportTemplate.HKRateCards.UPSExportExpress();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.UPSExportExpress report = new ReportTemplate.HKRateCards.UPSExportExpress();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName HKUPSExportExpedited(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.HeavyWeight).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                var Heavyship = HeavyWeightitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Heavyship != null && Heavyship.Count > 0)
                {
                    var Heavyweight = HeavyWeightitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Heavyweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Heavyship[0].ToString();
                        rate.Weight = Heavyweight[i].weights.WeightFrom.ToString() + " - " + Heavyweight[i].weights.Weight.ToString();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.HKRateCards.UPSExportExpeditedSummery report = new ReportTemplate.HKRateCards.UPSExportExpeditedSummery();
                    report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.HKRateCards.UPSExportExpedited report = new ReportTemplate.HKRateCards.UPSExportExpedited();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }
                        ReportTemplate.HKRateCards.UPSExportExpedited report = new ReportTemplate.HKRateCards.UPSExportExpedited();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.HKRateCards.UPSExportExpedited report = new ReportTemplate.HKRateCards.UPSExportExpedited();
                        report.Parameters["lblHeader"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "UPS " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UPS " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName UKDHLImportExpress(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);
            decimal margin = new CustomerRepository().GetCustomerMargin(User.UserId, User.LogisticServiceId, OperationZoneId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                            add.Rate1 = Math.Round(((addonweight[i].Rates[j].AddOnRate) + (addonweight[i].Rates[j].AddOnRate * margin) / 100), 2);
                        }
                        if (j == 1)
                        {
                            add.Rate2 = Math.Round(((addonweight[i].Rates[j].AddOnRate) + (addonweight[i].Rates[j].AddOnRate * margin) / 100), 2);
                        }
                        if (j == 2)
                        {
                            add.Rate3 = Math.Round(((addonweight[i].Rates[j].AddOnRate) + (addonweight[i].Rates[j].AddOnRate * margin) / 100), 2);
                        }
                        if (j == 3)
                        {
                            add.Rate4 = Math.Round(((addonweight[i].Rates[j].AddOnRate) + (addonweight[i].Rates[j].AddOnRate * margin) / 100), 2);
                        }
                        if (j == 4)
                        {
                            add.Rate5 = Math.Round(((addonweight[i].Rates[j].AddOnRate) + (addonweight[i].Rates[j].AddOnRate * margin) / 100), 2);
                        }
                        if (j == 5)
                        {
                            add.Rate6 = Math.Round(((addonweight[i].Rates[j].AddOnRate) + (addonweight[i].Rates[j].AddOnRate * margin) / 100), 2);
                        }
                        if (j == 6)
                        {
                            add.Rate7 = Math.Round(((addonweight[i].Rates[j].AddOnRate) + (addonweight[i].Rates[j].AddOnRate * margin) / 100), 2);
                        }
                        if (j == 7)
                        {
                            add.Rate8 = Math.Round(((addonweight[i].Rates[j].AddOnRate) + (addonweight[i].Rates[j].AddOnRate * margin) / 100), 2);
                        }
                        if (j == 8)
                        {
                            add.Rate9 = Math.Round(((addonweight[i].Rates[j].AddOnRate) + (addonweight[i].Rates[j].AddOnRate * margin) / 100), 2);
                        }
                        if (j == 9)
                        {
                            add.Rate10 = Math.Round(((addonweight[i].Rates[j].AddOnRate) + (addonweight[i].Rates[j].AddOnRate * margin) / 100), 2);
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.UKRateCards.DHLImportExpressSummery report = new ReportTemplate.UKRateCards.DHLImportExpressSummery();
                    report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.UKRateCards.DHLImportExpress report = new ReportTemplate.UKRateCards.DHLImportExpress();
                        report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.DHLImportExpress report = new ReportTemplate.UKRateCards.DHLImportExpress();
                        report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.DHLImportExpress report = new ReportTemplate.UKRateCards.DHLImportExpress();
                        report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }
            return result;
        }

        protected internal FrayteManifestName UKDHLImportEconomy(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);
            decimal margin = new CustomerRepository().GetCustomerMargin(User.UserId, User.LogisticServiceId, OperationZoneId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = DocAndNondocitem.OrderBy(p => p.WeightFrom).ToList();

            FrayteBaseRate rate;

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                            add.Rate1 = Math.Round(((addonweight[i].Rates[j].AddOnRate) + (addonweight[i].Rates[j].AddOnRate * margin) / 100), 2);
                        }
                        if (j == 1)
                        {
                            add.Rate2 = Math.Round(((addonweight[i].Rates[j].AddOnRate) + (addonweight[i].Rates[j].AddOnRate * margin) / 100), 2);
                        }
                        if (j == 2)
                        {
                            add.Rate3 = Math.Round(((addonweight[i].Rates[j].AddOnRate) + (addonweight[i].Rates[j].AddOnRate * margin) / 100), 2);
                        }
                        if (j == 3)
                        {
                            add.Rate4 = Math.Round(((addonweight[i].Rates[j].AddOnRate) + (addonweight[i].Rates[j].AddOnRate * margin) / 100), 2);
                        }
                        if (j == 4)
                        {
                            add.Rate5 = Math.Round(((addonweight[i].Rates[j].AddOnRate) + (addonweight[i].Rates[j].AddOnRate * margin) / 100), 2);
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.UKRateCards.DHLImportEconomySummery report = new ReportTemplate.UKRateCards.DHLImportEconomySummery();
                    report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.UKRateCards.DHLImportEconomy report = new ReportTemplate.UKRateCards.DHLImportEconomy();
                        report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.DHLImportEconomy report = new ReportTemplate.UKRateCards.DHLImportEconomy();
                        report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;


                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.DHLImportEconomy report = new ReportTemplate.UKRateCards.DHLImportEconomy();
                        report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }
            return result;
        }

        protected internal FrayteManifestName UKDHLExportExpress(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);
            decimal margin = new CustomerRepository().GetCustomerMargin(User.UserId, User.LogisticServiceId, OperationZoneId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                                add.Rate1 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 1)
                            {
                                add.Rate2 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 2)
                            {
                                add.Rate3 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 3)
                            {
                                add.Rate4 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 4)
                            {
                                add.Rate5 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 5)
                            {
                                add.Rate6 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 6)
                            {
                                add.Rate7 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 7)
                            {
                                add.Rate8 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 8)
                            {
                                add.Rate9 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 9)
                            {
                                add.Rate10 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.UKRateCards.DHLExportExpressSummery report = new ReportTemplate.UKRateCards.DHLExportExpressSummery();
                    report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.UKRateCards.DHLExportExpress report = new ReportTemplate.UKRateCards.DHLExportExpress();
                        report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.DHLExportExpress report = new ReportTemplate.UKRateCards.DHLExportExpress();
                        report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.DHLExportExpress report = new ReportTemplate.UKRateCards.DHLExportExpress();
                        report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName UKDHLExportEconomy(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);
            decimal margin = new CustomerRepository().GetCustomerMargin(User.UserId, User.LogisticServiceId, OperationZoneId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = DocAndNondocitem.OrderBy(p => p.WeightFrom).ToList();

            FrayteBaseRate rate;

            #region Doc&NondocRate

            if (docandnondoclist != null && docandnondoclist.Count > 0)
            {
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                                add.Rate1 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 1)
                            {
                                add.Rate2 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 2)
                            {
                                add.Rate3 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 3)
                            {
                                add.Rate4 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 4)
                            {
                                add.Rate5 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.UKRateCards.DHLExportEconomySummery report = new ReportTemplate.UKRateCards.DHLExportEconomySummery();
                    report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.UKRateCards.DHLExportEconomy report = new ReportTemplate.UKRateCards.DHLExportEconomy();
                        report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.DHLExportEconomy report = new ReportTemplate.UKRateCards.DHLExportEconomy();
                        report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.DHLExportEconomy report = new ReportTemplate.UKRateCards.DHLExportEconomy();
                        report.Parameters["lblHeader"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }
            return result;
        }

        protected internal FrayteManifestName UKDHL3rdPartyExpress(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var matrix = new ThirdPartyMarixRepository().GetThirdPartyMatrix(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(User.LogisticServiceId);
            var zone = new ThirdPartyMarixRepository().GetZoneDetail(User.LogisticServiceId);
            decimal margin = new CustomerRepository().GetCustomerMargin(User.UserId, User.LogisticServiceId, OperationZoneId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteZoneMatrix> _matrix = new List<FrayteZoneMatrix>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();
            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();

            var docandnondoclist = Nondocitem.Concat(DocAndNondocitem).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var DocNonship = docandnondoclist.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = docandnondoclist.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < DocNondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = DocNonship[0].ToString();
                        rate.Weight = DocNondocweight[i].weights.WeightFrom.ToString() + " - " + DocNondocweight[i].weights.Weight.ToString();
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
                                add.Rate1 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 1)
                            {
                                add.Rate2 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 2)
                            {
                                add.Rate3 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 3)
                            {
                                add.Rate4 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 4)
                            {
                                add.Rate5 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 5)
                            {
                                add.Rate6 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 6)
                            {
                                add.Rate7 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 7)
                            {
                                add.Rate8 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 8)
                            {
                                add.Rate9 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 9)
                            {
                                add.Rate10 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                        }
                        _add.Add(add);
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.UKRateCards.DHL3rdPartyExpressSummery report = new ReportTemplate.UKRateCards.DHL3rdPartyExpressSummery();
                    report.Parameters["lblHeader"].Value = "DHL 3rd Party " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblThirdParty"].Value = "DHL 3rd Party Zone Matrix " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "DHL 3rd Party " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "DHL 3rd Party " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHLs 3rd Party " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.UKRateCards.DHL3rdPartyExpress report = new ReportTemplate.UKRateCards.DHL3rdPartyExpress();
                        report.Parameters["lblHeader"].Value = "DHL 3rd Party " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblThirdParty"].Value = "DHL 3rd Party Zone Matrix " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL 3rd Party " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.DHL3rdPartyExpress report = new ReportTemplate.UKRateCards.DHL3rdPartyExpress();
                        report.Parameters["lblHeader"].Value = "DHL 3rd Party " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblThirdParty"].Value = "DHL 3rd Party Zone Matrix " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL 3rd Party " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.DHL3rdPartyExpress report = new ReportTemplate.UKRateCards.DHL3rdPartyExpress();
                        report.Parameters["lblHeader"].Value = "DHL 3rd Party " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblThirdParty"].Value = "DHL 3rd Party Zone Matrix " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL 3rd Party " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }
            return result;
        }

        protected internal FrayteManifestName UKDHL3rdPartyEconomy(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var matrix = new ThirdPartyMarixRepository().GetThirdPartyMatrix(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);
            var zone = new ThirdPartyMarixRepository().GetZoneDetail(User.LogisticServiceId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteZoneMatrix> _matrix = new List<FrayteZoneMatrix>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();
            var HeavyWeightitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.HeavyWeight).OrderBy(p => p.ZoneId).ToList();

            FrayteBaseRate rate;

            #region Doc&NondocRate

            if (DocAndNondocitem != null && DocAndNondocitem.Count > 0)
            {
                var DocNonship = DocAndNondocitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = DocAndNondocitem.GroupBy(x => x.Weight).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.UKRateCards.DHL3rdPartyEconomySummery report = new ReportTemplate.UKRateCards.DHL3rdPartyEconomySummery();
                    report.Parameters["lblHeader"].Value = "DHL 3rd Party " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblThirdParty"].Value = "DHL 3rd Party Zone Matrix " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "DHL 3rd Party " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "DHL 3rd Party " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHLs 3rd Party " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.UKRateCards.DHL3rdPartyEconomy report = new ReportTemplate.UKRateCards.DHL3rdPartyEconomy();
                        report.Parameters["lblHeader"].Value = "DHL 3rd Party " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblThirdParty"].Value = "DHL 3rd Party Zone Matrix " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL 3rd Party " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.DHL3rdPartyEconomy report = new ReportTemplate.UKRateCards.DHL3rdPartyEconomy();
                        report.Parameters["lblHeader"].Value = "DHL 3rd Party " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblThirdParty"].Value = "DHL 3rd Party Zone Matrix " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL 3rd Party " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.DHL3rdPartyEconomy report = new ReportTemplate.UKRateCards.DHL3rdPartyEconomy();
                        report.Parameters["lblHeader"].Value = "DHL 3rd Party " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblThirdParty"].Value = "DHL 3rd Party Zone Matrix " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "DHL 3rd Party " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL 3rd Party " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }
            return result;
        }

        protected internal FrayteManifestName UKTNTImportExpress(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);
            decimal margin = new CustomerRepository().GetCustomerMargin(User.UserId, User.LogisticServiceId, OperationZoneId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var Nonship = Nondocitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Nonship != null && Nonship.Count > 0)
                {
                    var Nondocweight = Nondocitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Nondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Nonship[0].ToString();
                        rate.Weight = Nondocweight[i].weights.WeightFrom.ToString() + " - " + Nondocweight[i].weights.Weight.ToString();
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
                    for(int j = 0; j < addonweight[i].weight.Count; j++)
                    {
                        add = new FrayteAddOnRate();
                        add.ShipmentType = addonweight[i].AddOnDescription;
                        for (int k = 0; k < addonweight[i].weight[j].Rates.Count; k++)
                        {
                            if (k == 0)
                            {
                                add.Rate1 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 1)
                            {
                                add.Rate2 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 2)
                            {
                                add.Rate3 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 3)
                            {
                                add.Rate4 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 4)
                            {
                                add.Rate5 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 5)
                            {
                                add.Rate6 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 6)
                            {
                                add.Rate7 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 7)
                            {
                                add.Rate8 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 8)
                            {
                                add.Rate9 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.UKRateCards.TNTImportExpressSummery report = new ReportTemplate.UKRateCards.TNTImportExpressSummery();
                    report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.UKRateCards.TNTImportExpress report = new ReportTemplate.UKRateCards.TNTImportExpress();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }
                        ReportTemplate.UKRateCards.TNTImportExpress report = new ReportTemplate.UKRateCards.TNTImportExpress();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.TNTImportExpress report = new ReportTemplate.UKRateCards.TNTImportExpress();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName UKTNTImportEconomy(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);
            decimal margin = new CustomerRepository().GetCustomerMargin(User.UserId, User.LogisticServiceId, OperationZoneId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();

            FrayteBaseRate rate;

            #region NondocRate

            if (Nondocitem != null && Nondocitem.Count > 0)
            {
                var Nonship = Nondocitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Nonship != null && Nonship.Count > 0)
                {
                    var Nondocweight = Nondocitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weightTo = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Nondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Nonship[0].ToString();
                        if (Nondocweight[i].weightTo.Weight == 5.99m)
                            rate.Weight = 0 + " - " + Nondocweight[i].weightTo.Weight.ToString();
                        else
                            rate.Weight = Nondocweight[i].weightTo.WeightFrom.ToString() + " - " + Nondocweight[i].weightTo.Weight.ToString();

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
                    for(int j = 0; j < addonweight[i].weight.Count;j++)
                    {
                        add = new FrayteAddOnRate();
                        add.ShipmentType = addonweight[i].AddOnDescription;
                        for (int k = 0; k < addonweight[i].weight[j].Rates.Count; k++)
                        {
                            if (k == 0)
                            {
                                add.Rate1 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 1)
                            {
                                add.Rate2 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 2)
                            {
                                add.Rate3 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 3)
                            {
                                add.Rate4 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 4)
                            {
                                add.Rate5 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 5)
                            {
                                add.Rate6 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 6)
                            {
                                add.Rate7 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 7)
                            {
                                add.Rate8 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 8)
                            {
                                add.Rate9 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.UKRateCards.TNTImportEconomySummery report = new ReportTemplate.UKRateCards.TNTImportEconomySummery();
                    report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.UKRateCards.TNTImportEconomy report = new ReportTemplate.UKRateCards.TNTImportEconomy();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.TNTImportEconomy report = new ReportTemplate.UKRateCards.TNTImportEconomy();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.TNTImportEconomy report = new ReportTemplate.UKRateCards.TNTImportEconomy();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName UKTNTExportExpress(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);
            decimal margin = new CustomerRepository().GetCustomerMargin(User.UserId, User.LogisticServiceId, OperationZoneId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Docitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Doc).OrderBy(p => p.ZoneId).ToList();
            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();

            FrayteBaseRate rate;

            #region DocRate

            if (Docitem != null && Docitem.Count > 0)
            {
                var ship = Docitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var docweight = Docitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < docweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = ship[0].ToString();
                        rate.Weight = docweight[i].weights.WeightFrom.ToString() + " - " + docweight[i].weights.Weight.ToString();
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
                var Nonship = Nondocitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Nonship != null && Nonship.Count > 0)
                {
                    var Nondocweight = Nondocitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Nondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Nonship[0].ToString();
                        rate.Weight = Nondocweight[i].weights.WeightFrom.ToString() + " - " + Nondocweight[i].weights.Weight.ToString();
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
                    for(int j = 0; j < addonweight[i].weight.Count; j++)
                    {
                        add = new FrayteAddOnRate();
                        add.ShipmentType = addonweight[i].AddOnDescription;
                        for (int k = 0; k < addonweight[i].weight[j].Rates.Count; k++)
                        {
                            if (k == 0)
                            {
                                add.Rate1 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 1)
                            {
                                add.Rate2 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 2)
                            {
                                add.Rate3 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 3)
                            {
                                add.Rate4 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 4)
                            {
                                add.Rate5 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 5)
                            {
                                add.Rate6 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 6)
                            {
                                add.Rate7 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 7)
                            {
                                add.Rate8 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
                            }
                            if (k == 8)
                            {
                                add.Rate9 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.UKRateCards.TNTExportExpressSummery report = new ReportTemplate.UKRateCards.TNTExportExpressSummery();
                    report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.UKRateCards.TNTExportExpress report = new ReportTemplate.UKRateCards.TNTExportExpress();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.TNTExportExpress report = new ReportTemplate.UKRateCards.TNTExportExpress();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.TNTExportExpress report = new ReportTemplate.UKRateCards.TNTExportExpress();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }

            return result;
        }

        protected internal FrayteManifestName UKTNTExportEconomy(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(User.LogisticServiceId);
            var country = new CustomerRepository().GetBaseRateZoneCountry(User.LogisticServiceId);
            decimal margin = new CustomerRepository().GetCustomerMargin(User.UserId, User.LogisticServiceId, OperationZoneId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var Nondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.Nondoc).OrderBy(p => p.ZoneId).ToList();

            FrayteBaseRate rate;

            #region NondocRate

            if (Nondocitem != null && Nondocitem.Count > 0)
            {
                var Nonship = Nondocitem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (Nonship != null && Nonship.Count > 0)
                {
                    var Nondocweight = Nondocitem.GroupBy(x => new { x.WeightFrom, x.Weight }).Select(g => new { weightTo = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < Nondocweight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.ShipmentType = Nonship[0].ToString();
                        if (Nondocweight[i].weightTo.Weight == 5.99m)
                            rate.Weight = 0 + " - " + Nondocweight[i].weightTo.Weight.ToString();
                        else
                            rate.Weight = Nondocweight[i].weightTo.WeightFrom.ToString() + " - " + Nondocweight[i].weightTo.Weight.ToString();

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
                    for(int j = 0; j < addonweight[i].weight.Count; j++)
                    {
                        add = new FrayteAddOnRate();
                        add.ShipmentType = addonweight[i].AddOnDescription;
                        for (int k = 0; k < addonweight[i].weight[j].Rates.Count; k++)
                        {
                            if (k == 0)
                            {
                                add.Rate1 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 1)
                            {
                                add.Rate2 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 2)
                            {
                                add.Rate3 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 3)
                            {
                                add.Rate4 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 4)
                            {
                                add.Rate5 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 5)
                            {
                                add.Rate6 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 6)
                            {
                                add.Rate7 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 7)
                            {
                                add.Rate8 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
                            }
                            if (k == 8)
                            {
                                add.Rate9 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + ((addonweight[i].weight[j].Rates[k].AddOnRate) * margin) / 100), 2);
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.UKRateCards.TNTExportEconomySummery report = new ReportTemplate.UKRateCards.TNTExportEconomySummery();
                    report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.UKRateCards.TNTExportEconomy report = new ReportTemplate.UKRateCards.TNTExportEconomy();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }
                        ReportTemplate.UKRateCards.TNTExportEconomy report = new ReportTemplate.UKRateCards.TNTExportEconomy();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.TNTExportEconomy report = new ReportTemplate.UKRateCards.TNTExportEconomy();
                        report.Parameters["lblHeader"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.Parameters["lblZoneCountry"].Value = "TNT " + User.LogisticType + " " + User.RateType + " Zone Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "TNT " + User.LogisticType + " " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }
            return result;
        }

        protected internal FrayteManifestName UKDHLDomestic(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            var addon = new CustomerRepository().GetAddOnRate(User.LogisticServiceId);
            decimal margin = new CustomerRepository().GetCustomerMargin(User.UserId, User.LogisticServiceId, OperationZoneId);

            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();
            List<FrayteAddOnRate> _add = new List<FrayteAddOnRate>();
            List<FrayteBaseRateWithCountry> _counrty = new List<FrayteBaseRateWithCountry>();

            var DocAndNondocitem = filter.FindAll(x => x.ShipmentType == FrayteShipmentType.DocAndNondoc).OrderBy(p => p.ZoneId).ToList();

            FrayteBaseRate rate;

            #region Doc&NondocRate

            if (DocAndNondocitem != null && DocAndNondocitem.Count > 0)
            {
                var DocNonship = DocAndNondocitem.Select(p => p.ShipmentDisplayType).Distinct().ToList();
                if (DocNonship != null && DocNonship.Count > 0)
                {
                    var DocNondocweight = DocAndNondocitem.GroupBy(x => x.Weight).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
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
                                add.Rate1 = Math.Round(((addonweight[i].weight[j].Rates[k].AddOnRate) + (addonweight[i].weight[j].Rates[k].AddOnRate * margin) / 100), 2);
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.UKRateCards.UKDomesticDHLSummery report = new ReportTemplate.UKRateCards.UKDomesticDHLSummery();
                    report.Parameters["lblHeader"].Value = "DHL Express UK Domestic " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.DataSource = _counrty;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL Express UK Domestic " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "DHL Express UK Domestic " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL Express UK Domestic " + User.RateType + " Summery Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.UKRateCards.UKDomesticDHL report = new ReportTemplate.UKRateCards.UKDomesticDHL();
                        report.Parameters["lblHeader"].Value = "DHL Express UK Domestic " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.DataSource = _counrty;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL Express UK Domestic " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "DHL Express UK Domestic " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL Express UK Domestic " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.UKDomesticDHL report = new ReportTemplate.UKRateCards.UKDomesticDHL();
                        report.Parameters["lblHeader"].Value = "DHL Express UK Domestic " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.DataSource = _counrty;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL Express UK Domestic " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL Express UK Domestic " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL Express UK Domestic " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL Express UK Domestic " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "DHL Express UK Domestic " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL Express UK Domestic " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.UKDomesticDHL report = new ReportTemplate.UKRateCards.UKDomesticDHL();
                        report.Parameters["lblHeader"].Value = "DHL Express UK Domestic " + User.RateType + " Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.DataSource = _counrty;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL Express UK Domestic " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL Express UK Domestic " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "DHL Express UK Domestic " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "DHL Express UK Domestic " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "DHL Express UK Domestic " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "DHL Express UK Domestic " + User.RateType + " Rate Card " + ZoneName + " (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }
            return result;
        }

        protected internal FrayteManifestName UKUKMail(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);
            List<FrayteBaseRate> _rate = new List<FrayteBaseRate>();

            var Ship1 = filter.FindAll(p => p.ShipmentType == "NWD").OrderBy(x => x.ZoneId).ToList();
            var Ship2 = filter.FindAll(p => p.ShipmentType == "NWDN").OrderBy(x => x.ZoneId).ToList();
            var Ship3 = filter.FindAll(p => p.ShipmentType == "NWD1030").OrderBy(x => x.ZoneId).ToList();
            var Ship4 = filter.FindAll(p => p.ShipmentType == "NWD0900").OrderBy(x => x.ZoneId).ToList();
            var Ship5 = filter.FindAll(p => p.ShipmentType == "SAT").OrderBy(x => x.ZoneId).ToList();
            var Ship6 = filter.FindAll(p => p.ShipmentType == "SAT1030").OrderBy(x => x.ZoneId).ToList();
            var Ship7 = filter.FindAll(p => p.ShipmentType == "SAT0900").OrderBy(x => x.ZoneId).ToList();
            var Ship8 = filter.FindAll(p => p.ShipmentType == "SSPP").OrderBy(x => x.ZoneId).ToList();

            FrayteBaseRate rate;

            #region Ship1

            if (Ship1 != null && Ship1.Count > 0)
            {
                var single = Ship1.FindAll(p => p.PackageType == "Single").ToList();
                if (single != null && single.Count > 0)
                {
                    #region Parcel

                    var parcel = single.FindAll(p => p.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].ParcelType;
                            rate.PackageType = parcel[0].PackageType;
                            rate.ShipmentType = parcel[0].ShipmentDisplayType;
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

                    var bagit = single.FindAll(p => p.ParcelType == "BagItService").ToList();
                    if (bagit != null && bagit.Count > 0)
                    {
                        var weight = bagit.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = bagit[0].ParcelType;
                            rate.PackageType = bagit[0].PackageType;
                            rate.ShipmentType = bagit[0].ShipmentDisplayType;
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

                var multiple = Ship1.FindAll(p => p.PackageType == "Multiple").ToList();
                if (multiple != null && multiple.Count > 0)
                {
                    #region Parcel

                    var parcel = multiple.FindAll(p => p.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].ParcelType;
                            rate.PackageType = parcel[0].PackageType;
                            rate.ShipmentType = parcel[0].ShipmentDisplayType;
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
                var single = Ship2.FindAll(p => p.PackageType == "Single").ToList();
                if (single != null && single.Count > 0)
                {
                    #region Parcel

                    var parcel = single.FindAll(p => p.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].ParcelType;
                            rate.PackageType = parcel[0].PackageType;
                            rate.ShipmentType = parcel[0].ShipmentDisplayType;
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

                    var bagit = single.FindAll(p => p.ParcelType == "BagItService").ToList();
                    if (bagit != null && bagit.Count > 0)
                    {
                        var weight = bagit.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = bagit[0].ParcelType;
                            rate.PackageType = bagit[0].PackageType;
                            rate.ShipmentType = bagit[0].ShipmentDisplayType;
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

                var multiple = Ship2.FindAll(p => p.PackageType == "Multiple").ToList();
                if (multiple != null && multiple.Count > 0)
                {
                    #region Parcel

                    var parcel = multiple.FindAll(p => p.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].ParcelType;
                            rate.PackageType = parcel[0].PackageType;
                            rate.ShipmentType = parcel[0].ShipmentDisplayType;
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
                var single = Ship3.FindAll(p => p.PackageType == "Single").ToList();
                if (single != null && single.Count > 0)
                {
                    #region Parcel

                    var parcel = single.FindAll(p => p.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].ParcelType;
                            rate.PackageType = parcel[0].PackageType;
                            rate.ShipmentType = parcel[0].ShipmentDisplayType;
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

                    var bagit = single.FindAll(p => p.ParcelType == "BagItService").ToList();
                    if (bagit != null && bagit.Count > 0)
                    {
                        var weight = bagit.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = bagit[0].ParcelType;
                            rate.PackageType = bagit[0].PackageType;
                            rate.ShipmentType = bagit[0].ShipmentDisplayType;
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

                var multiple = Ship3.FindAll(p => p.PackageType == "Multiple").ToList();
                if (multiple != null && multiple.Count > 0)
                {
                    #region Parcel

                    var parcel = multiple.FindAll(p => p.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].ParcelType;
                            rate.PackageType = parcel[0].PackageType;
                            rate.ShipmentType = parcel[0].ShipmentDisplayType;
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
                var single = Ship4.FindAll(p => p.PackageType == "Single").ToList();
                if (single != null && single.Count > 0)
                {
                    #region Parcel

                    var parcel = single.FindAll(p => p.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].ParcelType;
                            rate.PackageType = parcel[0].PackageType;
                            rate.ShipmentType = parcel[0].ShipmentDisplayType;
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

                    var bagit = single.FindAll(p => p.ParcelType == "BagItService").ToList();
                    if (bagit != null && bagit.Count > 0)
                    {
                        var weight = bagit.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = bagit[0].ParcelType;
                            rate.PackageType = bagit[0].PackageType;
                            rate.ShipmentType = bagit[0].ShipmentDisplayType;
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

                var multiple = Ship4.FindAll(p => p.PackageType == "Multiple").ToList();
                if (multiple != null && multiple.Count > 0)
                {
                    #region Parcel

                    var parcel = multiple.FindAll(p => p.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].ParcelType;
                            rate.PackageType = parcel[0].PackageType;
                            rate.ShipmentType = parcel[0].ShipmentDisplayType;
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
                var single = Ship5.FindAll(p => p.PackageType == "Single").ToList();
                if (single != null && single.Count > 0)
                {
                    #region Parcel

                    var parcel = single.FindAll(p => p.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].ParcelType;
                            rate.PackageType = parcel[0].PackageType;
                            rate.ShipmentType = parcel[0].ShipmentDisplayType;
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

                    var bagit = single.FindAll(p => p.ParcelType == "BagItService").ToList();
                    if (bagit != null && bagit.Count > 0)
                    {
                        var weight = bagit.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = bagit[0].ParcelType;
                            rate.PackageType = bagit[0].PackageType;
                            rate.ShipmentType = bagit[0].ShipmentDisplayType;
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

                var multiple = Ship5.FindAll(p => p.PackageType == "Multiple").ToList();
                if (multiple != null && multiple.Count > 0)
                {
                    #region Parcel

                    var parcel = multiple.FindAll(p => p.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].ParcelType;
                            rate.PackageType = parcel[0].PackageType;
                            rate.ShipmentType = parcel[0].ShipmentDisplayType;
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
                var single = Ship6.FindAll(p => p.PackageType == "Single").ToList();
                if (single != null && single.Count > 0)
                {
                    #region Parcel

                    var parcel = single.FindAll(p => p.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].ParcelType;
                            rate.PackageType = parcel[0].PackageType;
                            rate.ShipmentType = parcel[0].ShipmentDisplayType;
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

                    var bagit = single.FindAll(p => p.ParcelType == "BagItService").ToList();
                    if (bagit != null && bagit.Count > 0)
                    {
                        var weight = bagit.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = bagit[0].ParcelType;
                            rate.PackageType = bagit[0].PackageType;
                            rate.ShipmentType = bagit[0].ShipmentDisplayType;
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

                var multiple = Ship6.FindAll(p => p.PackageType == "Multiple").ToList();
                if (multiple != null && multiple.Count > 0)
                {
                    #region Parcel

                    var parcel = multiple.FindAll(p => p.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].ParcelType;
                            rate.PackageType = parcel[0].PackageType;
                            rate.ShipmentType = parcel[0].ShipmentDisplayType;
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
                var single = Ship7.FindAll(p => p.PackageType == "Single").ToList();
                if (single != null && single.Count > 0)
                {
                    #region Parcel

                    var parcel = single.FindAll(p => p.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].ParcelType;
                            rate.PackageType = parcel[0].PackageType;
                            rate.ShipmentType = parcel[0].ShipmentDisplayType;
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

                    var bagit = single.FindAll(p => p.ParcelType == "BagItService").ToList();
                    if (bagit != null && bagit.Count > 0)
                    {
                        var weight = bagit.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = bagit[0].ParcelType;
                            rate.PackageType = bagit[0].PackageType;
                            rate.ShipmentType = bagit[0].ShipmentDisplayType;
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

                var multiple = Ship7.FindAll(p => p.PackageType == "Multiple").ToList();
                if (multiple != null && multiple.Count > 0)
                {
                    #region Parcel

                    var parcel = multiple.FindAll(p => p.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].ParcelType;
                            rate.PackageType = parcel[0].PackageType;
                            rate.ShipmentType = parcel[0].ShipmentDisplayType;
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
                var single = Ship8.FindAll(p => p.PackageType == "Single").ToList();
                if (single != null && single.Count > 0)
                {
                    #region Parcel

                    var parcel = single.FindAll(p => p.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].ParcelType;
                            rate.PackageType = parcel[0].PackageType;
                            rate.ShipmentType = parcel[0].ShipmentDisplayType;
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

                    var bagit = single.FindAll(p => p.ParcelType == "BagItService").ToList();
                    if (bagit != null && bagit.Count > 0)
                    {
                        var weight = bagit.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = bagit[0].ParcelType;
                            rate.PackageType = bagit[0].PackageType;
                            rate.ShipmentType = bagit[0].ShipmentDisplayType;
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

                var multiple = Ship8.FindAll(p => p.PackageType == "Multiple").ToList();
                if (multiple != null && multiple.Count > 0)
                {
                    #region Parcel

                    var parcel = multiple.FindAll(p => p.ParcelType == "Parcel").ToList();
                    if (parcel != null && parcel.Count > 0)
                    {
                        var weight = parcel.GroupBy(p => new { p.ShipmentType, p.Weight }).Select(g => new { weights = g.Key.Weight, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                        for (int i = 0; i < weight.Count; i++)
                        {
                            rate = new FrayteBaseRate();
                            rate.ParcelType = parcel[0].ParcelType;
                            rate.PackageType = parcel[0].PackageType;
                            rate.ShipmentType = parcel[0].ShipmentDisplayType;
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.UKRateCards.UKMailSummery report = new ReportTemplate.UKRateCards.UKMailSummery();
                    report.Parameters["lblHeader"].Value = "UK Mail Domestic Shipments Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.DataSource = _rate;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UK Mail Domestic Shipments Summery Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "UK Mail Domestic Shipments Summery Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UK Mail Domestic Shipments Summery Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.UKRateCards.UKMail report = new ReportTemplate.UKRateCards.UKMail();
                        report.Parameters["lblHeader"].Value = "UK Mail Domestic Shipments Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.DataSource = _rate;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UK Mail Domestic Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "UK Mail Domestic Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UK Mail Domestic Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.UKMail report = new ReportTemplate.UKRateCards.UKMail();
                        report.Parameters["lblHeader"].Value = "UK Mail Domestic Shipments Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.DataSource = _rate;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UK Mail Domestic Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UK Mail Domestic Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UK Mail Domestic Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UK Mail Domestic Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "UK Mail Domestic Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UK Mail Domestic Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.UKMail report = new ReportTemplate.UKRateCards.UKMail();
                        report.Parameters["lblHeader"].Value = "UK Mail Domestic Shipments Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Today.Month, DateTime.Today.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.DataSource = _rate;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "UK Mail Domestic Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UK Mail Domestic Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "UK Mail Domestic Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "UK Mail Domestic Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "UK Mail Domestic Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "UK Mail Domestic Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }
            return result;
        }

        protected internal FrayteManifestName UKYodel(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);

            List<FrayteBaseRate> _b2b = new List<FrayteBaseRate>();
            List<FrayteBaseRate> _b2cNeighbour = new List<FrayteBaseRate>();
            List<FrayteBaseRate> _b2cHome = new List<FrayteBaseRate>();
            List<FrayteYodel> _yodel = new List<FrayteYodel>();

            var b2b = filter.FindAll(p => p.PackageType == "B2B").OrderBy(x => x.ZoneId).ToList();
            if (b2b != null && b2b.Count > 0)
            {
                var Ship1 = b2b.FindAll(p => p.ShipmentType == "EXP24").OrderBy(x => x.ZoneId).ToList();
                var Ship2 = b2b.FindAll(p => p.ShipmentType == "EXP48").OrderBy(x => x.ZoneId).ToList();
                var Ship3 = b2b.FindAll(p => p.ShipmentType == "EXPNI").OrderBy(x => x.ZoneId).ToList();
                var Ship4 = b2b.FindAll(p => p.ShipmentType == "HOM72").OrderBy(x => x.ZoneId).ToList();
                var Ship5 = b2b.FindAll(p => p.ShipmentType == "HOM72IN").OrderBy(x => x.ZoneId).ToList();
                var Ship6 = b2b.FindAll(p => p.ShipmentType == "PRI12MF").OrderBy(x => x.ZoneId).ToList();
                var Ship7 = b2b.FindAll(p => p.ShipmentType == "SATPRI12").OrderBy(x => x.ZoneId).ToList();
                var Ship8 = b2b.FindAll(p => p.ShipmentType == "EXPSAT").OrderBy(x => x.ZoneId).ToList();
                var Ship9 = b2b.FindAll(p => p.ShipmentType == "EXP72").OrderBy(x => x.ZoneId).ToList();
                var Ship10 = b2b.FindAll(p => p.ShipmentType == "EXPISLScHi").OrderBy(x => x.ZoneId).ToList();
                var Ship11 = b2b.FindAll(p => p.ShipmentType == "DNOSPChIs").OrderBy(x => x.ZoneId).ToList();
                var Ship12 = b2b.FindAll(p => p.ShipmentType == "ROIR48").OrderBy(x => x.ZoneId).ToList();
                var Ship13 = b2b.FindAll(p => p.ShipmentType == "ROIR72").OrderBy(x => x.ZoneId).ToList();

                FrayteBaseRate rate;

                #region Ship1

                if (Ship1 != null && Ship1.Count > 0)
                {
                    var weight = Ship1.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship1[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship1[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship2.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship2[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship2[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship3.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship3[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship3[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship4.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship4[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship4[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship5.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship5[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship5[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship6.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship6[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship6[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship7.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship7[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship7[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship8.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship8[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship8[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship9.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship9[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship9[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship10.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship10[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship10[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship11.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship11[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship11[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship12.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship12[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship12[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship13.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship13[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship13[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
            }

            var b2cNeighbour = filter.FindAll(p => p.PackageType == "B2CNeighborhood").OrderBy(x => x.ZoneId).ToList();
            if (b2cNeighbour != null && b2cNeighbour.Count > 0)
            {
                var Ship1 = b2cNeighbour.FindAll(p => p.ShipmentType == "EXP24").OrderBy(x => x.ZoneId).ToList();
                var Ship2 = b2cNeighbour.FindAll(p => p.ShipmentType == "EXP48").OrderBy(x => x.ZoneId).ToList();
                var Ship3 = b2cNeighbour.FindAll(p => p.ShipmentType == "EXPNI").OrderBy(x => x.ZoneId).ToList();
                var Ship4 = b2cNeighbour.FindAll(p => p.ShipmentType == "HOM72").OrderBy(x => x.ZoneId).ToList();
                var Ship5 = b2cNeighbour.FindAll(p => p.ShipmentType == "HOM72IN").OrderBy(x => x.ZoneId).ToList();
                var Ship6 = b2cNeighbour.FindAll(p => p.ShipmentType == "PRI12MF").OrderBy(x => x.ZoneId).ToList();
                var Ship7 = b2cNeighbour.FindAll(p => p.ShipmentType == "SATPRI12").OrderBy(x => x.ZoneId).ToList();
                var Ship8 = b2cNeighbour.FindAll(p => p.ShipmentType == "EXPSAT").OrderBy(x => x.ZoneId).ToList();
                var Ship9 = b2cNeighbour.FindAll(p => p.ShipmentType == "EXP72").OrderBy(x => x.ZoneId).ToList();
                var Ship10 = b2cNeighbour.FindAll(p => p.ShipmentType == "EXPISLScHi").OrderBy(x => x.ZoneId).ToList();
                var Ship11 = b2cNeighbour.FindAll(p => p.ShipmentType == "DNOSPChIs").OrderBy(x => x.ZoneId).ToList();
                var Ship12 = b2cNeighbour.FindAll(p => p.ShipmentType == "ROIR48").OrderBy(x => x.ZoneId).ToList();
                var Ship13 = b2cNeighbour.FindAll(p => p.ShipmentType == "ROIR72").OrderBy(x => x.ZoneId).ToList();

                FrayteBaseRate rate;

                #region Ship1

                if (Ship1 != null && Ship1.Count > 0)
                {
                    var weight = Ship1.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship1[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship1[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship2.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship2[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship2[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship3.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship3[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship3[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship4.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship4[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship4[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship5.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship5[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship5[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship6.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship6[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship6[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship7.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship7[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship7[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship8.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship8[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship8[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship9.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship9[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship9[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship10.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship10[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship10[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship11.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship11[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship11[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship12.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship12[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship12[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship13.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship13[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship13[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
            }

            var b2cHome = filter.FindAll(p => p.PackageType == "B2CHome").OrderBy(x => x.ZoneId).ToList();
            if (b2cHome != null && b2cHome.Count > 0)
            {
                var Ship1 = b2cHome.FindAll(p => p.ShipmentType == "EXP24").OrderBy(x => x.ZoneId).ToList();
                var Ship2 = b2cHome.FindAll(p => p.ShipmentType == "EXP48").OrderBy(x => x.ZoneId).ToList();
                var Ship3 = b2cHome.FindAll(p => p.ShipmentType == "EXPNI").OrderBy(x => x.ZoneId).ToList();
                var Ship4 = b2cHome.FindAll(p => p.ShipmentType == "HOM72").OrderBy(x => x.ZoneId).ToList();
                var Ship5 = b2cHome.FindAll(p => p.ShipmentType == "HOM72IN").OrderBy(x => x.ZoneId).ToList();
                var Ship6 = b2cHome.FindAll(p => p.ShipmentType == "PRI12MF").OrderBy(x => x.ZoneId).ToList();
                var Ship7 = b2cHome.FindAll(p => p.ShipmentType == "SATPRI12").OrderBy(x => x.ZoneId).ToList();
                var Ship8 = b2cHome.FindAll(p => p.ShipmentType == "EXPSAT").OrderBy(x => x.ZoneId).ToList();
                var Ship9 = b2cHome.FindAll(p => p.ShipmentType == "EXP72").OrderBy(x => x.ZoneId).ToList();
                var Ship10 = b2cHome.FindAll(p => p.ShipmentType == "EXPISLScHi").OrderBy(x => x.ZoneId).ToList();
                var Ship11 = b2cHome.FindAll(p => p.ShipmentType == "DNOSPChIs").OrderBy(x => x.ZoneId).ToList();
                var Ship12 = b2cHome.FindAll(p => p.ShipmentType == "ROIR48").OrderBy(x => x.ZoneId).ToList();
                var Ship13 = b2cHome.FindAll(p => p.ShipmentType == "ROIR72").OrderBy(x => x.ZoneId).ToList();

                FrayteBaseRate rate;

                #region Ship1

                if (Ship1 != null && Ship1.Count > 0)
                {
                    var weight = Ship1.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship1[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship1[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship2.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship2[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship2[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship3.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship3[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship3[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship4.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship4[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship4[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship5.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship5[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship5[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship6.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship6[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship6[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship7.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship7[i].PackageDisplayType;
                        rate.ParcelType = Ship7[i].ParcelDisplayType;
                        rate.ShipmentType = Ship7[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship8.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship8[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship8[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship9.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship9[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship9[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship10.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship10[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship10[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship11.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship11[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship11[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship12.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship12[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship12[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
                    var weight = Ship13.GroupBy(p => new { p.WeightFrom, p.Weight }).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
                    for (int i = 0; i < weight.Count; i++)
                    {
                        rate = new FrayteBaseRate();
                        rate.PackageType = Ship13[i].PackageDisplayType;
                        rate.ParcelType = Ship13[i].ParcelDisplayType + " per parcel rate";
                        rate.ShipmentType = Ship13[i].ShipmentDisplayType;
                        rate.Weight = weight[i].weights.Weight.ToString();
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
            }

            FrayteYodel yodel = new FrayteYodel();
            yodel.B2B = new List<FrayteBaseRate>();
            yodel.B2B = _b2b;
            yodel.B2CNeighbourhood = new List<FrayteBaseRate>();
            yodel.B2CNeighbourhood = _b2cNeighbour;
            yodel.B2CHome = new List<FrayteBaseRate>();
            yodel.B2CHome = _b2cHome;
            _yodel.Add(yodel);

            if (_yodel != null && _yodel.Count > 0)
            {
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.UKRateCards.YodelSummery report = new ReportTemplate.UKRateCards.YodelSummery();
                    report.Parameters["lblHeader"].Value = "Yodel Summery Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Now.Month, DateTime.Now.Year);
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.DataSource = _yodel;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "Yodel Summery UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "Yodel Summery UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "Yodel Summery UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.UKRateCards.Yodel report = new ReportTemplate.UKRateCards.Yodel();
                        report.Parameters["lblHeader"].Value = "Yodel Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Now.Month, DateTime.Now.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.DataSource = _yodel;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "Yodel UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "Yodel UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath += HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "Yodel UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.Yodel report = new ReportTemplate.UKRateCards.Yodel();
                        report.Parameters["lblHeader"].Value = "Yodel Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Now.Month, DateTime.Now.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.DataSource = _yodel;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "Yodel UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "Yodel UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "Yodel UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "Yodel UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "Yodel UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "Yodel UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.Yodel report = new ReportTemplate.UKRateCards.Yodel();
                        report.Parameters["lblHeader"].Value = "Yodel Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblFuel"].Value = UtilityRepository.FuelPercentage(User.LogisticServiceId, OperationZoneId, DateTime.Now.Month, DateTime.Now.Year);
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.DataSource = _yodel;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "Yodel UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "Yodel UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "Yodel UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "Yodel UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "Yodel UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "Yodel UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }
            return result;
        }

        protected internal FrayteManifestName UKHermes(CustomerRate User, int OperationZoneId, string FileType)
        {
            FrayteManifestName result = new FrayteManifestName();

            string ZoneName = UtilityRepository.OperationZoneName(OperationZoneId);
            string ZoneCurrency = UtilityRepository.OperationZoneCurrency(OperationZoneId);
            FrayteUser Customer = UtilityRepository.GetCustomerComapnyDetail(User.UserId);

            List<FrayteCustomerBaseRate> filter;
            if (User.FileType == FrayteCustomerBaseRateFileType.Pdf || User.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                filter = new CustomerRepository().GetBaseRate(User, OperationZoneId);
            }
            else
            {
                filter = new CustomerRepository().GetBaseRate(User);
            }

            var logdate = new CustomerRepository().LogisticServiceDates(User.LogisticServiceId);

            List<FrayteHermesParcelPOD> _ParcelPOD = new List<FrayteHermesParcelPOD>();
            List<FrayteHermesPacketPOD> _PacketPOD = new List<FrayteHermesPacketPOD>();
            List<FrayteHermesParcelNONPOD> _ParcelNONPOD = new List<FrayteHermesParcelNONPOD>();
            List<FrayteHermesPacketNONPOD> _PacketNONPOD = new List<FrayteHermesPacketNONPOD>();
            List<FrayteHermesBaseRate> _hermes = new List<FrayteHermesBaseRate>();

            var packetitem = filter.FindAll(p => p.ShipmentType == "Packet").OrderBy(x => x.ZoneId).ToList();
            var parcelitem = filter.FindAll(p => p.ShipmentType == "Parcel").OrderBy(x => x.ZoneId).ToList();

            #region PacketPODRate

            if (packetitem != null && packetitem.Count > 0)
            {
                FrayteHermesPacketPOD rate;

                #region POD

                var poditem = packetitem.FindAll(p => p.PackageType == "POD").ToList();
                var ship = poditem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var packetweight = poditem.GroupBy(x => x.Weight).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList(), Zones = g.Select(r => new { r.ZoneDisplayName }).ToList() }).ToList();
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

                var poditem = parcelitem.FindAll(p => p.PackageType == "POD").ToList();
                var ship = poditem.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (ship != null && ship.Count > 0)
                {
                    var packetweight = poditem.GroupBy(x => x.Weight).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
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

                var nonpod = packetitem.FindAll(p => p.PackageType == "NONPOD").ToList();
                var nonship = nonpod.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (nonship != null && nonship.Count > 0)
                {
                    var packetweight = nonpod.GroupBy(x => x.Weight).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
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

                var nonpod = parcelitem.FindAll(p => p.PackageType == "NONPOD").ToList();
                var nonship = nonpod.Select(p => p.ReportShipmentDisplay).Distinct().ToList();
                if (nonship != null && nonship.Count > 0)
                {
                    var packetweight = nonpod.GroupBy(x => x.Weight).Select(g => new { weights = g.Key, Rates = g.Select(p => new { p.Rate }).ToList() }).ToList();
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
                if (FileType == "Summery")
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                    ReportTemplate.UKRateCards.HermesSummery report = new ReportTemplate.UKRateCards.HermesSummery();
                    report.Parameters["lblHeader"].Value = "Hermes POD and NONPOD Summery Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                    report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                    report.Parameters["lblShipment"].Value = "Rate / Parcel Up to 3 Working Day Service";
                    report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                    report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                    report.DataSource = _hermes;
                    report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "Hermes UK Shipments Summery Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                    result.FileName = "Hermes UK Shipments Summery Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "Hermes UK Shipments Summery Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                }
                else
                {
                    if (User.SendingOption == FrayteCustomerBaseRateFileType.Email)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        ReportTemplate.UKRateCards.Hermes report = new ReportTemplate.UKRateCards.Hermes();
                        report.Parameters["lblHeader"].Value = "Hermes POD and NONPOD Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblShipment"].Value = "Rate / Parcel Up to 3 Working Day Service";
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.DataSource = _hermes;
                        report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "Hermes UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                        result.FileName = "Hermes UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        result.FilePath = HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "Hermes UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Pdf)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.Hermes report = new ReportTemplate.UKRateCards.Hermes();
                        report.Parameters["lblHeader"].Value = "Hermes POD and NONPOD Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblShipment"].Value = "Rate / Parcel Up to 3 Working Day Service";
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.DataSource = _hermes;

                        PdfExportOptions options = new PdfExportOptions();
                        options.ImageQuality = PdfJpegImageQuality.Highest;
                        options.PdfACompatibility = PdfACompatibility.None;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToPdf(val[1] + "CustomerRateCard/" + User.UserId + "/" + "Hermes UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "Hermes UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "Hermes UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                        else
                        {
                            report.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "Hermes UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf", options);
                            result.FileName = "Hermes UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "Hermes UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".pdf";
                        }
                    }
                    else if (User.FileType == FrayteCustomerBaseRateFileType.Excel)
                    {
                        string value = AppSettings.ReportFolder;
                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            System.IO.Directory.CreateDirectory(val[1] + "CustomerRateCard/" + User.UserId + "/");
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "CustomerRateCard/" + User.UserId + "/"));
                        }

                        ReportTemplate.UKRateCards.Hermes report = new ReportTemplate.UKRateCards.Hermes();
                        report.Parameters["lblHeader"].Value = "Hermes POD and NONPOD Rate Card " + UtilityRepository.FullOperationZoneName(OperationZoneId) + " (" + DateTime.Today.Year.ToString() + ")";
                        report.Parameters["lblCurrency"].Value = UtilityRepository.CustomerCurrency(User.UserId);
                        report.Parameters["lblShipment"].Value = "Rate / Parcel Up to 3 Working Day Service";
                        report.Parameters["lblIssueDate"].Value = logdate.IssuedDate;
                        report.Parameters["lblExpiryDate"].Value = logdate.ExpiryDate;
                        report.DataSource = _hermes;

                        if (value.Contains('#'))
                        {
                            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
                            report.ExportToXls(val[1] + "CustomerRateCard/" + User.UserId + "/" + "Hermes UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "Hermes UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = val[1] + "CustomerRateCard/" + User.UserId + "/" + "Hermes UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                        else
                        {
                            report.ExportToXls(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "CustomerRateCard/" + User.UserId + "/" + "Hermes UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls");
                            result.FileName = "Hermes UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/CustomerRateCard/" + User.UserId + "/" + "Hermes UK Shipments Rate Card (" + DateTime.Today.Year + ") [" + Customer.CompanyName + "]-" + filter[0].CustomerCurrency + ".xls";
                        }
                    }
                }
            }
            return result;
        }
    }
}