using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Frayte.Services.Business
{
    public class FuelSurChargeRepository
    {
        FrayteEntities dbcontext = new FrayteEntities();

        public FrayteResult SaveFuelSurCharge(FrayteFuelSurChargeSaveModel FrayteSurCharge)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                FrayteSurCharge.Year = new DateTime(FrayteSurCharge.Year.Year, 1, FrayteSurCharge.Year.Day);
                FuelSurCharge fuelsurcharge;

                var service = dbcontext.LogisticServices.Where(p => p.OperationZoneId == FrayteSurCharge.OperationZoneId && p.IsActive == true).ToList();
                if (service.Count > 0)
                {
                    foreach (var ss in service)
                    {
                        fuelsurcharge = new FuelSurCharge();
                        for (int i = 1; i <= 12; i++)
                        {
                            fuelsurcharge.OperationZoneId = FrayteSurCharge.OperationZoneId;
                            fuelsurcharge.LogisticServiceId = ss.LogisticServiceId;
                            fuelsurcharge.FrayteFuelPercent = 0;
                            fuelsurcharge.ExpryesFuelPercent = 0;
                            fuelsurcharge.DomesticFuelPercent = 0;
                            fuelsurcharge.RoadFuelSurcharge = 0;
                            fuelsurcharge.FuelMonthYear = FrayteSurCharge.Year;
                            fuelsurcharge.CreatedOn = DateTime.UtcNow;
                            fuelsurcharge.UpdatedOn = null;
                            fuelsurcharge.UpdatedBy = 1;
                            fuelsurcharge.IsUpdated = true;
                            dbcontext.FuelSurCharges.Add(fuelsurcharge);
                            dbcontext.SaveChanges();
                            FrayteSurCharge.Year = FrayteSurCharge.Year.AddMonths(1);
                        }
                    }
                }

                result.Status = true;
            }
            catch (Exception ex)
            {
                result.Status = false;
            }

            return result;
        }

        public List<FrayteFuelSurChargeList> GetThreeMonthFuelRate(int OperationZoneId, DateTime Year)
        {
            List<FrayteFuelSurChargeList> _fuel = new List<FrayteFuelSurChargeList>();
            FrayteFuelSurChargeList _fuelssur = new FrayteFuelSurChargeList();
            _fuelssur.LogisticCompany = "";
            var result = GetDistinctLogisticCompany(OperationZoneId);
            try
            {
                if (Year.Month == 1)
                {
                    _fuelssur.FuelSurCharge = new List<FuelSurChargeModel>();
                    for (int ii = 0; ii < result.Count; ii++)
                    {
                        _fuelssur = new FrayteFuelSurChargeList();
                        _fuelssur.FuelSurCharge = new List<FuelSurChargeModel>();
                        var Company = result[ii].LogisticCompany;
                        for (int i = 0; i < 3; i++)
                        {
                            if (i == 0)
                            {
                                var data = dbcontext.FuelSurCharges.Where(r => r.OperationZoneId == OperationZoneId && r.FuelMonthYear.Month == Year.Month && r.FuelMonthYear.Year == Year.Year).FirstOrDefault();
                                if (data != null)
                                {
                                    FuelSurChargeModel fuel = new FuelSurChargeModel();
                                    fuel.FuelSurchargeId = data.FuelSurChargeId;
                                    fuel.FrayteFuelPercent = data.FrayteFuelPercent.ToString();
                                    fuel.ExpryeFuelPercent = data.ExpryesFuelPercent.ToString();
                                    fuel.DomesticFuelPercent = data.DomesticFuelPercent.ToString();
                                    fuel.RoadFuelPercent = data.RoadFuelSurcharge.ToString();
                                    fuel.FuelMonthYear = data.FuelMonthYear;
                                    //_fuelssur.LogisticCompany = data.LogisticCompany;
                                    _fuelssur.FuelSurCharge.Add(fuel);
                                }
                            }
                            else if (i == 1)
                            {
                                DateTime month = Year.AddMonths(-1);
                                var data = dbcontext.FuelSurCharges.Where(r => r.OperationZoneId == OperationZoneId && r.FuelMonthYear.Month == month.Month && r.FuelMonthYear.Year == month.Year).FirstOrDefault();
                                if (data != null)
                                {
                                    FuelSurChargeModel fuel = new FuelSurChargeModel();
                                    fuel.FuelSurchargeId = data.FuelSurChargeId;
                                    fuel.FrayteFuelPercent = data.FrayteFuelPercent.ToString();
                                    fuel.ExpryeFuelPercent = data.ExpryesFuelPercent.ToString();
                                    fuel.DomesticFuelPercent = data.DomesticFuelPercent.ToString();
                                    fuel.RoadFuelPercent = data.RoadFuelSurcharge.ToString();
                                    fuel.FuelMonthYear = data.FuelMonthYear;
                                    _fuelssur.FuelSurCharge.Add(fuel);
                                }
                            }
                            else if (i == 2)
                            {
                                DateTime month = Year.AddMonths(-2);
                                var data = dbcontext.FuelSurCharges.Where(r => r.OperationZoneId == OperationZoneId && r.FuelMonthYear.Month == month.Month && r.FuelMonthYear.Year == month.Year).FirstOrDefault();
                                if (data != null)
                                {
                                    FuelSurChargeModel fuel = new FuelSurChargeModel();
                                    fuel.FuelSurchargeId = data.FuelSurChargeId;
                                    fuel.FrayteFuelPercent = data.FrayteFuelPercent.ToString();
                                    fuel.ExpryeFuelPercent = data.ExpryesFuelPercent.ToString();
                                    fuel.DomesticFuelPercent = data.DomesticFuelPercent.ToString();
                                    fuel.RoadFuelPercent = data.RoadFuelSurcharge.ToString();
                                    fuel.FuelMonthYear = data.FuelMonthYear;
                                    _fuelssur.FuelSurCharge.Add(fuel);
                                }
                            }
                        }
                        if (_fuelssur.FuelSurCharge.Count > 0)
                        {
                            _fuel.Add(_fuelssur);
                        }
                    }

                }
                else if (Year.Month == 2)
                {

                    for (int ii = 0; ii < result.Count; ii++)
                    {
                        _fuelssur = new FrayteFuelSurChargeList();
                        _fuelssur.FuelSurCharge = new List<FuelSurChargeModel>();
                        var Company = result[ii].LogisticCompany;
                        for (int i = 0; i < 3; i++)
                        {

                            if (i == 0)
                            {
                                var data = dbcontext.FuelSurCharges.Where(r => r.OperationZoneId == OperationZoneId && r.FuelMonthYear.Month == Year.Month && r.FuelMonthYear.Year == Year.Year).FirstOrDefault();
                                if (data != null)
                                {
                                    FuelSurChargeModel fuel = new FuelSurChargeModel();
                                    fuel.FuelSurchargeId = data.FuelSurChargeId;
                                    fuel.FrayteFuelPercent = data.FrayteFuelPercent.ToString();
                                    fuel.ExpryeFuelPercent = data.ExpryesFuelPercent.ToString();
                                    fuel.DomesticFuelPercent = data.DomesticFuelPercent.ToString();
                                    fuel.RoadFuelPercent = data.RoadFuelSurcharge.ToString();
                                    fuel.FuelMonthYear = data.FuelMonthYear;
                                    //_fuelssur.LogisticCompany = data.LogisticCompany;
                                    _fuelssur.FuelSurCharge.Add(fuel);
                                }
                            }
                            else if (i == 1)
                            {
                                DateTime month = Year.AddMonths(-1);
                                var data = dbcontext.FuelSurCharges.Where(r => r.OperationZoneId == OperationZoneId && r.FuelMonthYear.Month == month.Month && r.FuelMonthYear.Year == month.Year).FirstOrDefault();
                                if (data != null)
                                {
                                    FuelSurChargeModel fuel = new FuelSurChargeModel();
                                    fuel.FuelSurchargeId = data.FuelSurChargeId;
                                    fuel.FrayteFuelPercent = data.FrayteFuelPercent.ToString();
                                    fuel.ExpryeFuelPercent = data.ExpryesFuelPercent.ToString();
                                    fuel.DomesticFuelPercent = data.DomesticFuelPercent.ToString();
                                    fuel.RoadFuelPercent = data.RoadFuelSurcharge.ToString();
                                    fuel.FuelMonthYear = data.FuelMonthYear;
                                    _fuelssur.FuelSurCharge.Add(fuel);
                                }
                            }
                            else if (i == 2)
                            {
                                DateTime month = Year.AddMonths(-2);
                                var data = dbcontext.FuelSurCharges.Where(r => r.OperationZoneId == OperationZoneId && r.FuelMonthYear.Month == month.Month && r.FuelMonthYear.Year == month.Year).FirstOrDefault();
                                if (data != null)
                                {
                                    FuelSurChargeModel fuel = new FuelSurChargeModel();
                                    fuel.FuelSurchargeId = data.FuelSurChargeId;
                                    fuel.FrayteFuelPercent = data.FrayteFuelPercent.ToString();
                                    fuel.ExpryeFuelPercent = data.ExpryesFuelPercent.ToString();
                                    fuel.DomesticFuelPercent = data.DomesticFuelPercent.ToString();
                                    fuel.RoadFuelPercent = data.RoadFuelSurcharge.ToString();
                                    fuel.FuelMonthYear = data.FuelMonthYear;
                                    _fuelssur.FuelSurCharge.Add(fuel);
                                }
                            }

                        }
                        if (_fuelssur.FuelSurCharge.Count > 0)
                        {
                            _fuel.Add(_fuelssur);
                        }

                    }

                }
                else
                {
                    for (int ii = 0; ii < result.Count; ii++)
                    {
                        _fuelssur = new FrayteFuelSurChargeList();
                        _fuelssur.FuelSurCharge = new List<FuelSurChargeModel>();
                        var Company = result[ii].LogisticCompany;
                        for (int i = 0; i < 3; i++)
                        {
                            if (i == 0)
                            {
                                var data = dbcontext.FuelSurCharges.Where(r => r.OperationZoneId == OperationZoneId && r.FuelMonthYear.Month == Year.Month && r.FuelMonthYear.Year == Year.Year).FirstOrDefault();
                                if (data != null)
                                {
                                    FuelSurChargeModel fuel = new FuelSurChargeModel();
                                    fuel.FuelSurchargeId = data.FuelSurChargeId;
                                    fuel.FrayteFuelPercent = data.FrayteFuelPercent.ToString();
                                    fuel.ExpryeFuelPercent = data.ExpryesFuelPercent.ToString();
                                    fuel.DomesticFuelPercent = data.DomesticFuelPercent.ToString();
                                    fuel.RoadFuelPercent = data.RoadFuelSurcharge.ToString();
                                    fuel.FuelMonthYear = data.FuelMonthYear;
                                    //_fuelssur.LogisticCompany = data.LogisticCompany;
                                    _fuelssur.FuelSurCharge.Add(fuel);
                                }
                            }
                            else if (i == 1)
                            {
                                DateTime month = Year.AddMonths(-1);
                                var data = dbcontext.FuelSurCharges.Where(r => r.OperationZoneId == OperationZoneId && r.FuelMonthYear.Month == month.Month && r.FuelMonthYear.Year == month.Year).FirstOrDefault();
                                if (data != null)
                                {
                                    FuelSurChargeModel fuel = new FuelSurChargeModel();
                                    fuel.FuelSurchargeId = data.FuelSurChargeId;
                                    fuel.FrayteFuelPercent = data.FrayteFuelPercent.ToString();
                                    fuel.ExpryeFuelPercent = data.ExpryesFuelPercent.ToString();
                                    fuel.DomesticFuelPercent = data.DomesticFuelPercent.ToString();
                                    fuel.RoadFuelPercent = data.RoadFuelSurcharge.ToString();
                                    fuel.FuelMonthYear = data.FuelMonthYear;
                                    _fuelssur.FuelSurCharge.Add(fuel);
                                }
                            }
                            else if (i == 2)
                            {
                                DateTime month = Year.AddMonths(-2);
                                var data = dbcontext.FuelSurCharges.Where(r => r.OperationZoneId == OperationZoneId && r.FuelMonthYear.Month == month.Month && r.FuelMonthYear.Year == month.Year).FirstOrDefault();
                                if (data != null)
                                {
                                    FuelSurChargeModel fuel = new FuelSurChargeModel();
                                    fuel.FuelSurchargeId = data.FuelSurChargeId;
                                    fuel.FrayteFuelPercent = data.FrayteFuelPercent.ToString();
                                    fuel.ExpryeFuelPercent = data.ExpryesFuelPercent.ToString();
                                    fuel.DomesticFuelPercent = data.DomesticFuelPercent.ToString();
                                    fuel.RoadFuelPercent = data.RoadFuelSurcharge.ToString();
                                    fuel.FuelMonthYear = data.FuelMonthYear;
                                    _fuelssur.FuelSurCharge.Add(fuel);
                                }
                            }
                        }
                        if (_fuelssur.FuelSurCharge.Count > 0)
                        {
                            _fuel.Add(_fuelssur);
                        }
                    }

                }
                for (int a = 0; a < _fuel.Count; a++)
                {
                    if (_fuel[a].FuelSurCharge == null || _fuel[a].FuelSurCharge.Count < 3)
                    {
                        _fuel[a].FuelSurCharge = SaveFuelCharge(OperationZoneId, Year, _fuel[a].LogisticCompany);
                    }
                    _fuel[a].FuelSurCharge.OrderBy(x => x.FuelMonthYear).ToList();
                }
                return _fuel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<FuelSurChargeModel> SaveFuelCharge(int OperationZoneId, DateTime Year, string LogisticCompany)
        {
            FrayteFuelSurChargeSaveModel FrayteSurCharge = new FrayteFuelSurChargeSaveModel();
            var result = GetDistinctLogisticCompany(OperationZoneId);
            FrayteSurCharge.OperationZoneId = OperationZoneId;
            FrayteSurCharge.Year = Year;
            SaveFuelSurCharge(FrayteSurCharge);

            List<FrayteFuelSurChargeList> _fuel = new List<FrayteFuelSurChargeList>();
            FrayteFuelSurChargeList _fuelssur = new FrayteFuelSurChargeList();
            _fuelssur.FuelSurCharge = new List<FuelSurChargeModel>();
            try
            {
                if (Year.Month == 1)
                {
                    var Company = LogisticCompany;
                    for (int i = 0; i < 3; i++)
                    {
                        if (i == 0)
                        {
                            var data = dbcontext.FuelSurCharges.Where(r => r.OperationZoneId == OperationZoneId && r.FuelMonthYear.Month == Year.Month && r.FuelMonthYear.Year == Year.Year).FirstOrDefault();
                            if (data != null)
                            {
                                FuelSurChargeModel fuel = new FuelSurChargeModel();
                                fuel.FuelSurchargeId = data.FuelSurChargeId;
                                fuel.FrayteFuelPercent = data.FrayteFuelPercent.ToString();
                                fuel.ExpryeFuelPercent = data.ExpryesFuelPercent.ToString();
                                fuel.DomesticFuelPercent = data.DomesticFuelPercent.ToString();
                                fuel.RoadFuelPercent = data.RoadFuelSurcharge.ToString();
                                fuel.FuelMonthYear = data.FuelMonthYear;
                                //_fuelssur.LogisticCompany = data.LogisticCompany;
                                _fuelssur.FuelSurCharge.Add(fuel);
                            }
                        }
                        else if (i == 1)
                        {
                            DateTime month = Year.AddMonths(-1);
                            var data = dbcontext.FuelSurCharges.Where(r => r.OperationZoneId == OperationZoneId && r.FuelMonthYear.Month == month.Month && r.FuelMonthYear.Year == month.Year).FirstOrDefault();
                            if (data != null)
                            {
                                FuelSurChargeModel fuel = new FuelSurChargeModel();
                                fuel.FuelSurchargeId = data.FuelSurChargeId;
                                fuel.FrayteFuelPercent = data.FrayteFuelPercent.ToString();
                                fuel.ExpryeFuelPercent = data.ExpryesFuelPercent.ToString();
                                fuel.DomesticFuelPercent = data.DomesticFuelPercent.ToString();
                                fuel.RoadFuelPercent = data.RoadFuelSurcharge.ToString();
                                fuel.FuelMonthYear = data.FuelMonthYear;
                                _fuelssur.FuelSurCharge.Add(fuel);
                            }
                        }
                        else if (i == 2)
                        {
                            DateTime month = Year.AddMonths(-2);
                            var data = dbcontext.FuelSurCharges.Where(r => r.OperationZoneId == OperationZoneId && r.FuelMonthYear.Month == month.Month && r.FuelMonthYear.Year == month.Year).FirstOrDefault();
                            if (data != null)
                            {
                                FuelSurChargeModel fuel = new FuelSurChargeModel();
                                fuel.FuelSurchargeId = data.FuelSurChargeId;
                                fuel.FrayteFuelPercent = data.FrayteFuelPercent.ToString();
                                fuel.ExpryeFuelPercent = data.ExpryesFuelPercent.ToString();
                                fuel.DomesticFuelPercent = data.DomesticFuelPercent.ToString();
                                fuel.RoadFuelPercent = data.RoadFuelSurcharge.ToString();
                                fuel.FuelMonthYear = data.FuelMonthYear;
                                _fuelssur.FuelSurCharge.Add(fuel);
                            }
                        }
                    }

                }
                else if (Year.Month == 2)
                {
                    var Company = LogisticCompany;
                    for (int i = 0; i < 3; i++)
                    {
                        if (i == 0)
                        {
                            var data = dbcontext.FuelSurCharges.Where(r => r.OperationZoneId == OperationZoneId && r.FuelMonthYear.Month == Year.Month && r.FuelMonthYear.Year == Year.Year).FirstOrDefault();
                            if (data != null)
                            {
                                FuelSurChargeModel fuel = new FuelSurChargeModel();
                                fuel.FuelSurchargeId = data.FuelSurChargeId;
                                fuel.FrayteFuelPercent = data.FrayteFuelPercent.ToString();
                                fuel.ExpryeFuelPercent = data.ExpryesFuelPercent.ToString();
                                fuel.DomesticFuelPercent = data.DomesticFuelPercent.ToString();
                                fuel.RoadFuelPercent = data.RoadFuelSurcharge.ToString();
                                fuel.FuelMonthYear = data.FuelMonthYear;
                                //_fuelssur.LogisticCompany = data.LogisticCompany;
                                _fuelssur.FuelSurCharge.Add(fuel);
                            }
                        }
                        else if (i == 1)
                        {
                            DateTime month = Year.AddMonths(-1);
                            var data = dbcontext.FuelSurCharges.Where(r => r.OperationZoneId == OperationZoneId && r.FuelMonthYear.Month == month.Month && r.FuelMonthYear.Year == month.Year).FirstOrDefault();
                            if (data != null)
                            {
                                FuelSurChargeModel fuel = new FuelSurChargeModel();
                                fuel.FuelSurchargeId = data.FuelSurChargeId;
                                fuel.FrayteFuelPercent = data.FrayteFuelPercent.ToString();
                                fuel.ExpryeFuelPercent = data.ExpryesFuelPercent.ToString();
                                fuel.DomesticFuelPercent = data.DomesticFuelPercent.ToString();
                                fuel.RoadFuelPercent = data.RoadFuelSurcharge.ToString();
                                fuel.FuelMonthYear = data.FuelMonthYear;
                                _fuelssur.FuelSurCharge.Add(fuel);
                            }
                        }
                        else if (i == 2)
                        {
                            DateTime month = Year.AddMonths(-2);
                            var data = dbcontext.FuelSurCharges.Where(r => r.OperationZoneId == OperationZoneId && r.FuelMonthYear.Month == month.Month && r.FuelMonthYear.Year == month.Year).FirstOrDefault();
                            if (data != null)
                            {
                                FuelSurChargeModel fuel = new FuelSurChargeModel();
                                fuel.FuelSurchargeId = data.FuelSurChargeId;
                                fuel.FrayteFuelPercent = data.FrayteFuelPercent.ToString();
                                fuel.ExpryeFuelPercent = data.ExpryesFuelPercent.ToString();
                                fuel.DomesticFuelPercent = data.DomesticFuelPercent.ToString();
                                fuel.RoadFuelPercent = data.RoadFuelSurcharge.ToString();
                                fuel.FuelMonthYear = data.FuelMonthYear;
                                _fuelssur.FuelSurCharge.Add(fuel);
                            }
                        }
                    }

                }
                else
                {
                    var Company = LogisticCompany;
                    for (int i = 0; i < 3; i++)
                    {
                        if (i == 0)
                        {
                            var data = dbcontext.FuelSurCharges.Where(r => r.OperationZoneId == OperationZoneId && r.FuelMonthYear.Month == Year.Month && r.FuelMonthYear.Year == Year.Year).FirstOrDefault();
                            if (data != null)
                            {
                                FuelSurChargeModel fuel = new FuelSurChargeModel();
                                fuel.FuelSurchargeId = data.FuelSurChargeId;
                                fuel.FrayteFuelPercent = data.FrayteFuelPercent.ToString();
                                fuel.ExpryeFuelPercent = data.ExpryesFuelPercent.ToString();
                                fuel.DomesticFuelPercent = data.DomesticFuelPercent.ToString();
                                fuel.RoadFuelPercent = data.RoadFuelSurcharge.ToString();
                                fuel.FuelMonthYear = data.FuelMonthYear;
                                //_fuelssur.LogisticCompany = data.LogisticCompany;
                                _fuelssur.FuelSurCharge.Add(fuel);
                            }
                        }
                        else if (i == 1)
                        {
                            DateTime month = Year.AddMonths(-1);
                            var data = dbcontext.FuelSurCharges.Where(r => r.OperationZoneId == OperationZoneId && r.FuelMonthYear.Month == month.Month && r.FuelMonthYear.Year == month.Year).FirstOrDefault();
                            if (data != null)
                            {
                                FuelSurChargeModel fuel = new FuelSurChargeModel();
                                fuel.FuelSurchargeId = data.FuelSurChargeId;
                                fuel.FrayteFuelPercent = data.FrayteFuelPercent.ToString();
                                fuel.ExpryeFuelPercent = data.ExpryesFuelPercent.ToString();
                                fuel.DomesticFuelPercent = data.DomesticFuelPercent.ToString();
                                fuel.RoadFuelPercent = data.RoadFuelSurcharge.ToString();
                                fuel.FuelMonthYear = data.FuelMonthYear;
                                _fuelssur.FuelSurCharge.Add(fuel);
                            }
                        }
                        else if (i == 2)
                        {
                            DateTime month = Year.AddMonths(-2);
                            var data = dbcontext.FuelSurCharges.Where(r => r.OperationZoneId == OperationZoneId && r.FuelMonthYear.Month == month.Month && r.FuelMonthYear.Year == month.Year).FirstOrDefault();
                            if (data != null)
                            {
                                FuelSurChargeModel fuel = new FuelSurChargeModel();
                                fuel.FuelSurchargeId = data.FuelSurChargeId;
                                fuel.FrayteFuelPercent = data.FrayteFuelPercent.ToString();
                                fuel.ExpryeFuelPercent = data.ExpryesFuelPercent.ToString();
                                fuel.DomesticFuelPercent = data.DomesticFuelPercent.ToString();
                                fuel.RoadFuelPercent = data.RoadFuelSurcharge.ToString();
                                fuel.FuelMonthYear = data.FuelMonthYear;
                                _fuelssur.FuelSurCharge.Add(fuel);
                            }
                        }
                    }


                }
                return _fuelssur.FuelSurCharge.OrderBy(x => x.FuelMonthYear).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<FrayteFuelSurcharge> GetFuelSurCharge(int OperationZoneId, int Year)
        {
            var data = (from r in dbcontext.FuelSurCharges
                        join ls in dbcontext.LogisticServices on r.LogisticServiceId equals ls.LogisticServiceId
                        where
                            r.OperationZoneId == OperationZoneId &&
                            r.FuelMonthYear.Year == Year &&
                            ls.OperationZoneId == OperationZoneId &&
                            ls.IsActive == true
                        select new FrayteFuelSurcharge
                        {
                            LogisticCompany = ls.LogisticCompanyDisplay,
                            UpdatedBy = 0,
                            Type = new List<FrayteLogistictype>()
                            {
                                new FrayteLogistictype()
                                {
                                    LogisticType = ls.LogisticTypeDisplay,
                                    RateType = ls.RateTypeDisplay,
                                    MonthYear = new List<FrayteFuelMonthYear>()
                                    {
                                        new FrayteFuelMonthYear()
                                        {
                                            Month =  r.FuelMonthYear.Month,
                                            Year = r.FuelMonthYear.Year,
                                            RateType = ls.RateTypeDisplay,
                                            LogsiticType = ls.LogisticTypeDisplay,
                                            FuelSurchargeId = r.FuelSurChargeId,
                                            FrayteFuelPercent = r.FrayteFuelPercent,
                                            IsChange = false
                                        }
                                    }
                                }
                            }
                        }).ToList();

            data = data.GroupBy(x => new { x.LogisticCompany })
                       .Select(y => new FrayteFuelSurcharge
                       {
                           LogisticCompany = y.Key.LogisticCompany,
                           UpdatedBy = y.Select(p => p.UpdatedBy).FirstOrDefault(),
                           Type = y.SelectMany(z => z.Type)
                                   .GroupBy(a => new { a.RateType, a.LogisticType })
                                   .Select(b => new FrayteLogistictype
                                   {
                                       RateType = b.Key.RateType,
                                       LogisticType = b.Key.LogisticType,
                                       MonthYear = y.SelectMany(c => c.Type.SelectMany(d => d.MonthYear)
                                                    .Where(p => p.RateType == b.Key.RateType && p.LogsiticType == b.Key.LogisticType))
                                                    .Select(f => new FrayteFuelMonthYear
                                                    {
                                                        Month = f.Month,
                                                        Year = f.Year,
                                                        RateType = f.RateType,
                                                        LogsiticType = f.LogsiticType,
                                                        FuelSurchargeId = f.FuelSurchargeId,
                                                        FrayteFuelPercent = f.FrayteFuelPercent,
                                                        IsChange = false
                                                    }).ToList()
                                   }).OrderBy(p => p.LogisticType).ToList()
                       }).OrderBy(p => p.LogisticCompany).ToList();

            return data;
        }

        public FrayteResult UpdateFuelSurCharge(List<FrayteFuelSurcharge> fuelSurCharge)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                if (fuelSurCharge != null && fuelSurCharge.Count > 0)
                {
                    foreach (var data in fuelSurCharge)
                    {
                        foreach (var type in data.Type)
                        {
                            foreach (var month in type.MonthYear)
                            {
                                if (month.IsChange)
                                {
                                    var fuel = dbcontext.FuelSurCharges.Find(month.FuelSurchargeId);
                                    if (fuel != null)
                                    {
                                        fuel.FrayteFuelPercent = month.FrayteFuelPercent;
                                        fuel.ExpryesFuelPercent = month.FrayteFuelPercent;
                                        fuel.DomesticFuelPercent = month.FrayteFuelPercent;
                                        fuel.RoadFuelSurcharge = month.FrayteFuelPercent;
                                        fuel.IsUpdated = true;
                                        fuel.UpdatedOn = DateTime.UtcNow;
                                        fuel.UpdatedBy = data.UpdatedBy;
                                        dbcontext.Entry(fuel).State = System.Data.Entity.EntityState.Modified;
                                        dbcontext.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    result.Status = true;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
            }
            return result;
        }

        public FrayteStatus UpdateStatus(DateTime datetime, int OperationZoneId)
        {
            FrayteStatus fs = new FrayteStatus();
            //int OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
            if (dbcontext.FuelSurCharges.Where(p => p.OperationZoneId == OperationZoneId && p.FuelMonthYear.Year == datetime.Year && p.FuelMonthYear.Month == datetime.Month).FirstOrDefault().UpdatedOn.HasValue)
            {
                DateTime updatedate = dbcontext.FuelSurCharges.Where(p => p.OperationZoneId == OperationZoneId && p.FuelMonthYear.Year == datetime.Year && p.FuelMonthYear.Month == datetime.Month).FirstOrDefault().UpdatedOn.Value;
                if (updatedate >= datetime)
                {
                    fs.IsFuelSurCharge = true;
                    return fs;
                }
                else
                {
                    int FuelId = dbcontext.FuelSurCharges.Where(p => p.OperationZoneId == OperationZoneId && p.FuelMonthYear.Year == datetime.Year && p.FuelMonthYear.Month == datetime.Month).FirstOrDefault().FuelSurChargeId;
                    var rs = dbcontext.FuelSurCharges.Where(p => p.FuelSurChargeId == FuelId).FirstOrDefault();
                    if (rs != null && rs.FuelSurChargeId > 0)
                    {
                        rs.MailSendOn = DateTime.UtcNow;
                        // Update
                        dbcontext.Entry(rs).State = System.Data.Entity.EntityState.Modified;
                        dbcontext.SaveChanges();
                    }
                    fs.IsFuelSurCharge = false;
                    return fs;
                }
            }
            else
            {
                int FuelId = dbcontext.FuelSurCharges.Where(p => p.OperationZoneId == OperationZoneId && p.FuelMonthYear.Year == datetime.Year && p.FuelMonthYear.Month == datetime.Month).FirstOrDefault().FuelSurChargeId;
                var rs = dbcontext.FuelSurCharges.Where(p => p.FuelSurChargeId == FuelId).FirstOrDefault();
                if (rs != null && rs.FuelSurChargeId > 0)
                {
                    rs.MailSendOn = DateTime.UtcNow;
                    // Update
                    dbcontext.Entry(rs).State = System.Data.Entity.EntityState.Modified;
                    dbcontext.SaveChanges();
                }
                fs.IsFuelSurCharge = false;
                return fs;
            }
        }

        public FrayteStatus GetSendMailStatus(DateTime datetime, int OperationZoneId)
        {
            FrayteStatus fs = new FrayteStatus();
            var item = dbcontext.FuelSurCharges.Where(p => p.OperationZoneId == OperationZoneId && p.FuelMonthYear.Year == datetime.Year && p.FuelMonthYear.Month == datetime.Month).FirstOrDefault();
            if (item.MailSendOn.HasValue)
            {
                if (item.MailSendOn.Value == datetime)
                {

                }
                else
                {
                    fs.FuelMailSentOn = item.MailSendOn.Value;
                }
            }
            return fs;
        }

        public List<int> GetDistinctFuelSurchargeYear()
        {
            try
            {
                List<int> year = new List<int>();
                var yy = dbcontext.FuelSurCharges.Select(p => p.FuelMonthYear.Year).Distinct().ToList();
                if (yy.Count > 0)
                {
                    int j = 0;
                    for (int i = 0; i < yy.Count; i++)
                    {
                        if (yy[i] < DateTime.Now.Year)
                        {
                            year.Add(yy[i]);
                            j++;
                        }
                    }
                    year.Insert(j, DateTime.Now.Year);
                    return year;
                }
                else
                {
                    year.Add(DateTime.Now.Year);
                    return year;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<FrayteMonthYear> GetFuelSurchargeMonthYear(int OperationZoneId)
        {
            try
            {
                List<FrayteMonthYear> year = dbcontext.FuelSurCharges.Where(p => p.OperationZoneId == OperationZoneId)
                                            .Select(p => new FrayteMonthYear
                                            {
                                                Year = p.FuelMonthYear.Year,
                                                Month = p.FuelMonthYear.Month
                                            }).Distinct().ToList();
                if (year.Count > 0)
                {
                    return year;
                }
                else
                {
                    FrayteMonthYear mont;
                    for (int i = 0; i < 12; i++)
                    {
                        mont = new FrayteMonthYear();
                        mont.Month = i + 1;
                        mont.Year = DateTime.Now.Year;
                        year.Add(mont);
                    }
                    return year;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<LogisticCompanyList> GetDistinctLogisticCompany(int OperationZoneId)
        {
            List<LogisticCompanyList> FinalList = new List<LogisticCompanyList>();
            var result = dbcontext.LogisticServices.Where(a => a.OperationZoneId == OperationZoneId).ToList();

            var company = result.Select(p => new
            {
                p.LogisticCompany,
                p.LogisticCompanyDisplay
            }).Distinct().OrderBy(p => p.LogisticCompany).ToList();

            foreach (var a in company)
            {
                LogisticCompanyList res = new LogisticCompanyList();
                res.LogisticCompany = a.LogisticCompany;
                res.LogisticCompanyDisplay = a.LogisticCompanyDisplay;
                FinalList.Add(res);
            }
            return FinalList;
        }
    }
}