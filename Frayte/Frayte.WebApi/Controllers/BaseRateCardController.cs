using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;
using Report.Generator.ManifestReport;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class BaseRateCardController : ApiController
    {
        #region Logistic Service Issued Expiry Date 

        [HttpPost]
        public FrayteResult UpdateLogisticSerice(LogisticServiceDuration obj)
        {
            FrayteResult result = new FrayteResult();
            if (obj != null)
            {
                result = new BaseRateCardRepository().UpdateLogisticSerice(obj);
            }
            else
            {
                result.Status = false;
            }
            return result;
        }

        [HttpGet]
        public LogisticServiceDuration GetLogisrticServiceDuration(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            LogisticServiceDuration logisticSericeDuration = new BaseRateCardRepository().GetLogisticSericeDuration(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType);
            return logisticSericeDuration;
        }

        #endregion

        [HttpGet]
        public List<FrayteZoneBaseRateCard> GetZoneBaseRateCard(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string PackageType, string ParcelType, string DocType, string ModuleType)
        {
            List<FrayteZoneBaseRateCard> list = null;
            if (LogisticType == FrayteLogisticType.UKShipment)
            {
                list = new BaseRateCardRepository().GetUKBaseRate(OperationZoneId, LogisticType, CourierCompany, RateType, PackageType, ParcelType, ModuleType);
            }
            else if (LogisticType == FrayteLogisticType.EUExport || LogisticType == FrayteLogisticType.EUImport)
            {
                list = new BaseRateCardRepository().GetEUBaseRate(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType);
            }
            else if (LogisticType == FrayteLogisticType.Import ||
                     LogisticType == FrayteLogisticType.Export ||
                     LogisticType == FrayteLogisticType.ThirdParty)
            {
                list = new BaseRateCardRepository().GetBaseRate(OperationZoneId, LogisticType, CourierCompany, RateType, DocType, ModuleType);
            }
            return list;
        }

        [HttpGet]
        public List<FryateZoneBaseRateCardLimit> GetZoneBaseRateCardLimit(int OperationZoneId, string LogisticType, string CourierComapny, string RateType, string PackageType, string ParcelType, string ModuleType)
        {
            List<FryateZoneBaseRateCardLimit> lstLimitCard = new BaseRateCardRepository().GetZoneBaseRateCardLimit(OperationZoneId, LogisticType, CourierComapny, RateType, PackageType, ParcelType, ModuleType);
            return lstLimitCard;
        }

        [HttpPost]
        public IHttpActionResult SaveZoneBaseRateCardLimit(List<FryateZoneBaseRateCardLimit> _baseRateCardLimit)
        {
            FrayteResult result = new BaseRateCardRepository().SaveZoneBaseRateCardLimit(_baseRateCardLimit);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult SaveDimensionBaseRate(List<FrayteDimensionBaseRateCard> _baseRateCardLimit)
        {
            FrayteResult result = new BaseRateCardRepository().SaveDimensionBaseRate(_baseRateCardLimit);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult EditZoneBaseRateCard(List<FrayteZoneBaseRateCard> _ratecard)
        {
            FrayteResult result = new BaseRateCardRepository().EditZoneRateCard(_ratecard);
            return Ok(result);
        }

        [HttpGet]
        public List<FrayteOperationZoneCurrency> GetOperationZoneCurrencyCode(int OperationZoneId, string exchangeType)
        {
            var list = new BaseRateCardRepository().OperationZoneCurrencyCode(OperationZoneId, exchangeType);
            return list;
        }

        [HttpGet]
        public List<FrayteLogisticWeight> GetWeight(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string PackageType, string ParcelType, string ModuleType)
        {
            var list = new BaseRateCardRepository().GetLogisticWeight(OperationZoneId, LogisticType, CourierCompany, RateType, PackageType, ParcelType, ModuleType);
            return list;
        }

        [HttpGet]
        public List<FrayteZone> GetZoneDetail(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            var list = new BaseRateCardRepository().GetZoneDetail(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType);
            return list;
        }

        [HttpGet]
        public List<FrayteOperationZone> GetOperationZone()
        {
            var list = new BaseRateCardRepository().GetOperationZone();
            return list;
        }

        [HttpGet]
        public List<FrayteCourierAccount> GetCourierAccount(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            var list = new BaseRateCardRepository().GetCourierAccount(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType);
            return list;
        }

        [HttpGet]
        public List<LogisticShipmentType> GetShipmentType(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            var list = new BaseRateCardRepository().GetShipmentType(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType);
            return list;
        }

        [HttpGet]
        public List<FrayteZoneBaseRateCard> GetZoneUKAddOnRate(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ParcelType, string PackageType, string ModuleType)
        {
            List<FrayteZoneBaseRateCard> _addonrate = new List<FrayteZoneBaseRateCard>();
            _addonrate = new BaseRateCardRepository().GetUKAddOnRate(OperationZoneId, LogisticType, CourierCompany, RateType, ParcelType, PackageType, ModuleType);
            return _addonrate;
        }

        [HttpGet]
        public List<FrayteAddOnRateCard> GetZoneAddOnRate(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ParcelType, string PackageType, string ModuleType)
        {
            List<FrayteAddOnRateCard> _addonrate = new List<FrayteAddOnRateCard>();
            _addonrate = new BaseRateCardRepository().GetAddOnRate(OperationZoneId, LogisticType, CourierCompany, RateType, ParcelType, PackageType, ModuleType);
            return _addonrate;
        }

        [HttpPost]
        public IHttpActionResult UpdateUKAddOnRate(List<FrayteAddRate> _Ukaddon)
        {
            FrayteResult result = new BaseRateCardRepository().UpdateUKAddOnRate(_Ukaddon);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult UpdateAddOnRate(List<FrayteAddRate> _addrate)
        {
            FrayteResult result = new BaseRateCardRepository().UpdateAddOnRate(_addrate);
            return Ok(result);
        }

        [HttpGet]
        public FrayteResult GetAddOnRateIdValid(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string PackageType, string ParcelType, string ModuleType)
        {
            FrayteResult result = new BaseRateCardRepository().ValidAddOnRateId(OperationZoneId, LogisticType, CourierCompany, RateType, ParcelType, PackageType, ModuleType);
            return result;
        }
    }
}