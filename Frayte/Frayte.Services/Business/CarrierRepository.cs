using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Business
{
    public class CarrierRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public string GetPreFix(int carrierId)
        {
            var preFix = dbContext.Carriers.Find(carrierId).Prefix;

            return preFix;
        }
        public List<FrayteCarrier> GetCarrierList()
        {
            var lstCarrier = (from c in dbContext.Carriers
                              select new FrayteCarrier()
                              {
                                  CarrierId = c.CarrierId,
                                  CarrierName = c.CarrierName,
                                  Code = c.Code,
                                  Prefix = c.Prefix,
                                  CarrierType = c.CarrierType
                              }).ToList();
            
            return lstCarrier;
        }

        public List<FrayteCarrierModel> GetCarrierList(string carrierType)
        {
            var lstCarrier = (from c in dbContext.Carriers
                              where c.CarrierType == carrierType
                              select new FrayteCarrierModel()
                              {
                                  CarrierId = c.CarrierId,
                                  CarrierName = c.CarrierName                                 
                              }).ToList();

            return lstCarrier;
        }

        public FrayteCarrier SaveCarrier(FrayteCarrier carrier)
        {
            Carrier newCarrier;
            if (carrier.CarrierId > 0 )
            {
                newCarrier = dbContext.Carriers.Where(p => p.CarrierId == carrier.CarrierId).FirstOrDefault();

                newCarrier.CarrierName = carrier.CarrierName;
                newCarrier.Code = carrier.Code;
                newCarrier.Prefix = carrier.Prefix;
                newCarrier.CarrierType = carrier.CarrierType;
            }
            else
            {
                newCarrier = new Carrier();
                newCarrier.CarrierId = carrier.CarrierId;
                newCarrier.CarrierName = carrier.CarrierName;
                newCarrier.Code = carrier.Code;
                newCarrier.Prefix = carrier.Prefix;
                newCarrier.CarrierType = carrier.CarrierType;
            
                dbContext.Carriers.Add(newCarrier);
            }

            dbContext.SaveChanges();

            carrier.CarrierId = newCarrier.CarrierId;

            return carrier;
        }

        public FrayteResult DeleteCarrier(int carrierId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var carrier = new Carrier { CarrierId = carrierId };
                dbContext.Carriers.Attach(carrier);
                dbContext.Carriers.Remove(carrier);
                dbContext.SaveChanges();
                result.Status = true;
            }
            catch(Exception ex)
            {
                result.Status = false;
                result.Errors = new List<string>();
                result.Errors.Add(ex.Message);
            }

            return result;
        }
    }
}
