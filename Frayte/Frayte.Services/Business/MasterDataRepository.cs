using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.Tradelane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Business
{
    public class MasterDataRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<ShipmentTerm> GetShipmentTerms()
        {
            var lstShipmentTerms = dbContext.ShipmentTerms.ToList();

            return lstShipmentTerms;
        }

        public List<PackagingType> GetPackagingType()
        {
            var lstPackagingTypes = dbContext.PackagingTypes.ToList();

            return lstPackagingTypes;
        }

        public List<SpecialDelivery> GetSpecialDelivery()
        {
            var lstSpecialDelivery = dbContext.SpecialDeliveries.ToList();

            return lstSpecialDelivery;
        }

        public List<TradelaneShipmentHandlerMethod> GetShipmentHandlerMethod()
        {
            List<TradelaneShipmentHandlerMethod> list = (from r in dbContext.ShipmentHandlerMethods
                                                         where r.ShipmentHandlerMethodType == ShipmentHandlerMethodEnum.Air
                                                         select new TradelaneShipmentHandlerMethod
                                                         {
                                                             ShipmentHandlerMethodId = r.ShipmentHandlerMethodId,
                                                             ShipmentHandlerMethodName = r.ShipmentHandlerMethodName,
                                                             ShipmentHandlerMethodType = r.ShipmentHandlerMethodType,
                                                             ShipmentHandlerMethodDisplay = r.ShipmentHandlerMethodDisplay
                                                         }).ToList();

            return list;
        }

        public List<TradelaneAirline> GetAirlines()
        {
            List<TradelaneAirline> list = (from r in dbContext.Airlines
                                           select new TradelaneAirline
                                           {
                                               AirlineId = r.AirlineId,
                                               AilineCode = r.AirlineCode,
                                               AirLineName = r.AirLineName,
                                               CarrierCode2 = r.CarrierCode2,
                                               CarrierCode3 = r.CarrierCode3
                                           }).ToList();
            return list;
        }

        public List<TradelaneAirport> GetAirports(int countryId)
        {
            List<TradelaneAirport> list = (from r in dbContext.Airports
                                           where r.CountryId == countryId
                                           select new TradelaneAirport
                                           {
                                               AirportCodeId = r.AirportCodeId,
                                               CountryId = r.CountryId,
                                               AirportCode = r.AirportCode,
                                               AirportName = r.AirportName
                                           }).ToList();
            return list;
        }

        public List<CurrencyType> GetCurrencyType()
        {
            var lstGetCurrencyType = dbContext.CurrencyTypes.ToList();
            var lst = new List<CurrencyType>();
            //lst = lstGetCurrencyType;
            foreach (var djf in lstGetCurrencyType)
            {
                lst.Add(djf);
            }
            var currency = new List<CurrencyType>();

            foreach (var cur in lst)
            {
                if (cur.CurrencyCode == "HKD")
                {
                    currency.Add(cur);
                    lstGetCurrencyType.Remove(cur);
                }
            }
            foreach (var cur in lst)
            {
                if (cur.CurrencyCode == "USD")
                {
                    currency.Add(cur);
                    lstGetCurrencyType.Remove(cur);
                }
            }
            foreach (var cur in lst)
            {
                if (cur.CurrencyCode == "GBP")
                {
                    currency.Add(cur);
                    lstGetCurrencyType.Remove(cur);
                }
            }
            foreach (var cur in lst)
            {
                if (cur.CurrencyCode == "AUD")
                {
                    currency.Add(cur);
                    lstGetCurrencyType.Remove(cur);
                }
            }
            foreach (var cur in lst)
            {
                if (cur.CurrencyCode == "RMB")
                {
                    currency.Add(cur);
                    lstGetCurrencyType.Remove(cur);
                }
            }
            foreach (var cur in lst)
            {
                if (cur.CurrencyCode == "NZD")
                {
                    currency.Add(cur);
                    lstGetCurrencyType.Remove(cur);
                }
            }

            foreach (var cur in lstGetCurrencyType)
            {
                currency.Add(cur);
            }

            return currency;
        }

        public List<CountryShipmentPort> GetShipmentPorts()
        {
            var lstShipmentPorts = dbContext.CountryShipmentPorts.ToList();
            return lstShipmentPorts;
        }

        public List<FrayteParcelType> GetParcelType()
        {
            List<FrayteParcelType> lstParcelTypes = new List<FrayteParcelType>();
            lstParcelTypes.Add(new FrayteParcelType() { ParcelType = "Parcel", ParcelDescription = "Parcel (Non Doc)" });
            //lstParcelTypes.Add(new FrayteParcelType() { ParcelType = "Pallet", ParcelDescription = "Pallet (Non Doc)" });
            lstParcelTypes.Add(new FrayteParcelType() { ParcelType = "Letter", ParcelDescription = "Letter (Document)" });

            return lstParcelTypes;
        }

        public List<Country> ToCountryList()
        {
            List<Country> lstToCountry = new List<Country>();
            var res = dbContext.Countries.Where(a => a.CountryId == 228).FirstOrDefault();
            lstToCountry.Add(res);
            return lstToCountry;
        }
    }
}
