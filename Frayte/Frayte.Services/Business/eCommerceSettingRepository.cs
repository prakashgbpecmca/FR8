using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;
using Frayte.Services.Utility;
using AutoMapper;

namespace Frayte.Services.Business
{
    public class eCommerceSettingRepository
    {
        FrayteEntities dbContext = new FrayteEntities();
        public List<eCommerceSettingModel> GeteCommerceHSCodeDetail(int CountryId)
        {
            List<eCommerceSettingModel> res = new List<eCommerceSettingModel>();
            var Result = dbContext.HSCodes.Where(a => a.CountryId == CountryId).ToList();
            Mapper.CreateMap<HSCode, eCommerceSettingModel>();
            AutoMapper.Mapper.Map(Result, res);
            return res;
        }

        public List<eCommerceSettingCountry> CountryList()
        {
            List<eCommerceSettingCountry> res1 = new List<eCommerceSettingCountry>();
            var Result = dbContext.HSCodes.Select(a => a.CountryId).Distinct().ToList();

            for (int i = 0; i < Result.Count; i++)
            {
                var val = Result[i];
                eCommerceSettingCountry res = new eCommerceSettingCountry();
                var CountryDetail = dbContext.Countries.Where(a => a.CountryId == val).FirstOrDefault();
                Mapper.CreateMap<Country, eCommerceSettingCountry>();
                AutoMapper.Mapper.Map(CountryDetail, res);
                res1.Add(res);
            }

            return res1;
        }
        public FrayteResult AddEditHSCodeSetting(eCommerceSettingModel HSCodeDetail)
        {
            FrayteResult FR = new FrayteResult();
            HSCode HSC = new HSCode();
            var Result = dbContext.HSCodes.Where(a => a.HSCodeId == HSCodeDetail.HsCodeId).FirstOrDefault();
            if (Result == null)
            {
                Mapper.CreateMap<eCommerceSettingModel, HSCode>();
                AutoMapper.Mapper.Map(HSCodeDetail, HSC);
                dbContext.HSCodes.Add(HSC);
                dbContext.SaveChanges();
                FR.Status = true;
            }
            else
            {
                Mapper.CreateMap<eCommerceSettingModel, HSCode>();
                AutoMapper.Mapper.Map(HSCodeDetail, Result);
                dbContext.Entry(Result).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                FR.Status = true;
            }

            return FR;
        }
        public FrayteResult DeleteHSCodeSetting(int HsCodeId)
        {
            FrayteResult FR = new FrayteResult();
            var Result = dbContext.HSCodes.Where(a => a.HSCodeId == HsCodeId).FirstOrDefault();
            if (Result != null)
            {
                dbContext.HSCodes.Attach(Result);
                dbContext.HSCodes.Remove(Result);
                dbContext.SaveChanges();
                FR.Status = true;
            }
            else
            {
                FR.Status = false;
            }

            return FR;
        }
    }
}
