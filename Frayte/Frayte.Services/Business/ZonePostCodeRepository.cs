using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using System.Data.Entity.Validation;

namespace Frayte.Services.Business
{
    public class ZonePostCodeRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<FrayteCountryUK> UKCountryList()
        {
            List<FrayteCountryUK> _UKCounrty = new List<FrayteCountryUK>();
            try
            {
                FrayteCountryUK fcu;
                var list = dbContext.CountryUKs.ToList();
                if (list != null)
                {
                    foreach (var uk in list)
                    {
                        fcu = new FrayteCountryUK();
                        fcu.CountryUKId = uk.CountryUKId;
                        fcu.CountryName = uk.CountryName;
                        _UKCounrty.Add(fcu);
                    }
                    return _UKCounrty;
                }
            }
            catch
            {

            }
            return _UKCounrty;
        }

        //It will list out all the PostCode which in not assigned to any zone yet.
        public FrayteUKPostCode UKCountryPostCodeList(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType, int ZoneId)
        {
            FrayteUKPostCode _UKCounrty = new FrayteUKPostCode();
            _UKCounrty.TransitTime = 0;
             _UKCounrty.FrayteZonePostCodeList = new List<FrayteCountryUKPostCode>();
            try
            {
                var res = dbContext.LogisticServiceZones.Where(a => a.LogisticServiceZoneId == ZoneId).FirstOrDefault();
                _UKCounrty.TransitTime = res.TransitTime;
                var result1 = (from zpc in dbContext.LogisticServiceZonePostCodes
                               select new
                               {
                                   zpc.PostCode
                               }).Take(200).ToList();

                FrayteCountryUKPostCode fcpc;
                if (result1 != null)
                {
                    foreach (var pc in result1)
                    {
                        fcpc = new FrayteCountryUKPostCode();
                        fcpc.PostCode = pc.PostCode;
                        _UKCounrty.FrayteZonePostCodeList.Add(fcpc);
                    }
                    return _UKCounrty;
                }
            }
            catch
            {

            }
            return _UKCounrty;
        }

        //It will list out all the PostCodes assigned to the zones.
        public List<FrayteZonePostCode> GetZonePostCodeList(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            List<FrayteZonePostCode> _postcode = new List<FrayteZonePostCode>();
            try
            {
                var zones = (from lsz in dbContext.LogisticServices
                             join lz in dbContext.LogisticServiceZones on lsz.LogisticServiceId equals lz.LogisticServiceId
                             where lsz.OperationZoneId == OperationZoneId &&
                                   lsz.LogisticType == LogisticType &&
                                   lsz.LogisticCompany == CourierCompany &&
                                   lsz.RateType == RateType &&
                                   lsz.ModuleType == ModuleType
                             select new
                             {
                                 lsz.LogisticServiceId,
                                 lz.LogisticServiceZoneId
                             }).ToList();

                if (zones != null && zones.Count > 0)
                {
                    int TotalRows = 0;
                    foreach (var zone in zones)
                    {
                        var result = dbContext.spGet_ZonePostcode(zone.LogisticServiceZoneId, zone.LogisticServiceId, CourierCompany, OperationZoneId, null, 0, 16).ToList();

                        if (result != null && result.Count > 0)
                        {
                            TotalRows = result[0].TotalRows.Value;
                        }
                        else
                        {
                            TotalRows = 0;
                        }

                        foreach (spGet_ZonePostcode_Result zonePostCode in result)
                        {
                            FrayteZonePostCode postCode = new FrayteZonePostCode();
                            postCode.ZonePostCodeId = zonePostCode.LogisticServiceZonePostCodeId;
                            postCode.PostCodeZone = new FrayteZone()
                            {
                                ZoneId = zonePostCode.LogisticServiceZoneId,
                                OperationZoneId = zonePostCode.OperationZoneId,
                                ZoneName = zonePostCode.ZoneName,
                                ZoneDisplayName = zonePostCode.ZoneDisplayName
                            };
                            postCode.TransitTime = zonePostCode.transittime;
                            postCode.PostCode = zonePostCode.PostCode;
                            postCode.IsActive = true;

                            postCode.TotalRows = TotalRows;

                            _postcode.Add(postCode);
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return _postcode;
        }

        public List<FrayteZonePostCode> GetZonePostCodeList(int OperationZoneId, string LogisticType, int ZoneId, string SearchPostcode, int CurrentPage, int TakeRows)
        {
            int TotalRows = 0;
            if (!string.IsNullOrEmpty(SearchPostcode))
            {
                SearchPostcode = "%" + SearchPostcode + "%";
            }

            int SkipRows = 0;
            SkipRows = (CurrentPage - 1) * TakeRows;

            List<FrayteZonePostCode> _postcode = new List<FrayteZonePostCode>();

            var service = (from lsz in dbContext.LogisticServiceZones
                           join ls in dbContext.LogisticServices on lsz.LogisticServiceId equals ls.LogisticServiceId
                           where lsz.LogisticServiceZoneId == ZoneId
                           select new
                           {
                               lsz.LogisticServiceId,
                               ls.LogisticCompany
                           }).FirstOrDefault();

            try
            {
                var result = dbContext.spGet_ZonePostcode(ZoneId, service.LogisticServiceId, service.LogisticCompany, OperationZoneId, SearchPostcode, SkipRows, TakeRows).ToList();

                if (result != null && result.Count > 0)
                {
                    TotalRows = result[0].TotalRows.Value;
                }
                else
                {
                    TotalRows = 0;
                }

                foreach (spGet_ZonePostcode_Result zonePostCode in result)
                {
                    FrayteZonePostCode postCode = new FrayteZonePostCode();
                    postCode.ZonePostCodeId = zonePostCode.LogisticServiceZonePostCodeId;
                    postCode.PostCodeZone = new FrayteZone()
                    {
                        ZoneId = zonePostCode.LogisticServiceZoneId,
                        OperationZoneId = zonePostCode.OperationZoneId,
                        ZoneName = zonePostCode.ZoneName,
                        ZoneColor = zonePostCode.ZoneColor,
                        ZoneDisplayName = zonePostCode.ZoneDisplayName
                    };
                    postCode.PostCode = zonePostCode.PostCode;
                    postCode.IsActive = true;

                    postCode.TotalRows = TotalRows;

                    _postcode.Add(postCode);
                }
            }
            catch
            {

            }
            return _postcode;
        }

        public void SaveZonePostCode(FraytePostCodeUK _postcode)
        {
            try
            {
                LogisticServiceZonePostCode zpc;
                if (_postcode != null)
                {

                    var result = dbContext.LogisticServiceZones.Where(a => a.LogisticServiceZoneId == _postcode.LogisticServiceZoneId).FirstOrDefault();
                    if(result != null)
                    {
                        result.TransitTime = _postcode.TransitTime;
                        dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    if (_postcode.FrayteZonePostCodeList != null && _postcode.FrayteZonePostCodeList.Count > 0)
                    {
                        foreach (var code in _postcode.FrayteZonePostCodeList)
                        {
                            if (code.IsActive)
                            {
                                zpc = dbContext.LogisticServiceZonePostCodes.Where(x => x.LogisticServiceZonePostCodeId == code.ZonePostCodeId).FirstOrDefault();
                                if (zpc != null)
                                {
                                    zpc = new LogisticServiceZonePostCode();
                                    if (code.LogisticCompany == FrayteLogisticServiceType.DHL)
                                    {
                                        zpc.DHLLogisticServiceZoneId = code.PostCodeZone.ZoneId;
                                    }
                                    else if (code.LogisticCompany == FrayteLogisticServiceType.UkMail)
                                    {
                                        zpc.UKMailLogisticServiceZoneId = code.PostCodeZone.ZoneId;
                                    }
                                    else if (code.LogisticCompany == FrayteLogisticServiceType.Hermes)
                                    {
                                        zpc.HermesLogisticServiceZoneId = code.PostCodeZone.ZoneId;
                                    }
                                    else if (code.LogisticCompany == FrayteLogisticServiceType.Yodel)
                                    {
                                        zpc.YodelLogisticServiceZoneId = code.PostCodeZone.ZoneId;
                                    }
                                    zpc.PostCode = code.PostCode;
                                    dbContext.Entry(code).State = System.Data.Entity.EntityState.Modified;
                                    dbContext.SaveChanges();
                                }
                                else
                                {
                                    zpc = new LogisticServiceZonePostCode();
                                    if (code.LogisticCompany == FrayteLogisticServiceType.DHL)
                                    {
                                        zpc.DHLLogisticServiceZoneId = code.PostCodeZone.ZoneId;
                                    }
                                    else if (code.LogisticCompany == FrayteLogisticServiceType.UkMail)
                                    {
                                        zpc.UKMailLogisticServiceZoneId = code.PostCodeZone.ZoneId;
                                    }
                                    else if (code.LogisticCompany == FrayteLogisticServiceType.Hermes)
                                    {
                                        zpc.HermesLogisticServiceZoneId = code.PostCodeZone.ZoneId;
                                    }
                                    else if (code.LogisticCompany == FrayteLogisticServiceType.Yodel)
                                    {
                                        zpc.YodelLogisticServiceZoneId = code.PostCodeZone.ZoneId;
                                    }
                                    zpc.PostCode = code.PostCode;
                                    dbContext.LogisticServiceZonePostCodes.Add(zpc);
                                    dbContext.SaveChanges();
                                }
                            }
                            else
                            {
                                zpc = dbContext.LogisticServiceZonePostCodes.Where(p => p.PostCode == code.PostCode).FirstOrDefault();
                                if (zpc != null)
                                {
                                    dbContext.LogisticServiceZonePostCodes.Remove(zpc);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        string ss = "Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage;
                    }
                }
            }
        }
    }
}
