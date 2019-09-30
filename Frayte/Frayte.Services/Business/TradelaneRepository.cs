using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Business
{
    public class TradelaneRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public int GetCarrierId(int tradeaneId)
        {
            int carrierId = dbContext.Tradelanes.Find(tradeaneId).CarrierId.Value;
            return carrierId;
        }

        public List<FrayteTradelane> GetTradelaneList()
        {
            List<FrayteTradelane> lstTradelane = new List<FrayteTradelane>();

            var tradelanes = (from t in dbContext.Tradelanes
                              join oa in dbContext.Users on t.OriginatingAgentId equals oa.UserId
                              join da in dbContext.Users on t.DestinationAgentId equals da.UserId
                              join oc in dbContext.Countries on t.OriginCountryId equals oc.CountryId
                              join dc in dbContext.Countries on t.DestinationCountryId equals dc.CountryId
                              join c in dbContext.Carriers on t.CarrierId equals c.CarrierId
                              select new
                              {
                                  t.TradelaneId,
                                  OriginatingAgentId = oa.UserId,
                                  OriginatingAgentName = oa.ContactName,
                                  OriginatingCountryId = oc.CountryId,
                                  OriginatingCountryName = oc.CountryName,
                                  OriginatingCountryCode = oc.CountryCode,
                                  DestinationAgentId = da.UserId,
                                  DestinationAgentName = da.ContactName,
                                  DestinationCountryId = dc.CountryId,
                                  DestinationCountryName = dc.CountryName,
                                  DestinationCountryCode = dc.CountryCode,
                                  t.Direct,
                                  t.Deffered,
                                  c.CarrierId,
                                  c.CarrierName,
                                  c.CarrierType,
                                  t.TransitTime
                              }).ToList();

            if (tradelanes != null)
            {
                foreach (var tradelane in tradelanes)
                {
                    FrayteTradelane frateTradeLane = new FrayteTradelane();
                    frateTradeLane.TradelaneId = tradelane.TradelaneId;
                    frateTradeLane.Direct = tradelane.Direct;
                    frateTradeLane.Deffered = tradelane.Deffered;
                    frateTradeLane.OriginatingAgent = new FrayteUserModel();
                    frateTradeLane.OriginatingAgent.UserId = tradelane.OriginatingAgentId;
                    frateTradeLane.OriginatingAgent.Name = tradelane.OriginatingAgentName;
                    frateTradeLane.DestinationAgent = new FrayteUserModel();
                    frateTradeLane.DestinationAgent.UserId = tradelane.DestinationAgentId;
                    frateTradeLane.DestinationAgent.Name = tradelane.DestinationAgentName;
                    frateTradeLane.OriginatingCountry = new FrayteCountryCode();
                    frateTradeLane.OriginatingCountry.CountryId = tradelane.OriginatingCountryId;
                    frateTradeLane.OriginatingCountry.Code = tradelane.OriginatingCountryCode;
                    frateTradeLane.OriginatingCountry.Name = tradelane.OriginatingCountryName;
                    frateTradeLane.DestinationCountry = new FrayteCountryCode();
                    frateTradeLane.DestinationCountry.CountryId = tradelane.DestinationCountryId;
                    frateTradeLane.DestinationCountry.Code = tradelane.DestinationCountryCode;
                    frateTradeLane.DestinationCountry.Name = tradelane.DestinationCountryName;
                    frateTradeLane.Carrier = new FrayteCarrierModel();
                    frateTradeLane.Carrier.CarrierId = tradelane.CarrierId;
                    frateTradeLane.Carrier.CarrierName = tradelane.CarrierName;
                    frateTradeLane.CarrierType = tradelane.CarrierType;
                    frateTradeLane.TransitTime = tradelane.TransitTime;
                    lstTradelane.Add(frateTradeLane);
                }
            }

            return lstTradelane;
        }

        public List<FrayteTradelane> GetTradelaneList(int userId)
        {
            List<FrayteTradelane> lstTradelane = new List<FrayteTradelane>();

            var tradelanes = (from t in dbContext.Tradelanes
                              join ctl in dbContext.CustomerTradeLanes on t.TradelaneId equals ctl.TradeLaneId
                              join oa in dbContext.Users on t.OriginatingAgentId equals oa.UserId
                              join da in dbContext.Users on t.DestinationAgentId equals da.UserId
                              join oc in dbContext.Countries on t.OriginCountryId equals oc.CountryId
                              join dc in dbContext.Countries on t.DestinationCountryId equals dc.CountryId
                              join c in dbContext.Carriers on t.CarrierId equals c.CarrierId
                              where ctl.UserId == userId
                              select new
                              {
                                  t.TradelaneId,
                                  OriginatingAgentId = oa.UserId,
                                  OriginatingAgentName = oa.ContactName,
                                  OriginatingCountryId = oc.CountryId,
                                  OriginatingCountryName = oc.CountryName,
                                  OriginatingCountryCode = oc.CountryCode,
                                  DestinationAgentId = da.UserId,
                                  DestinationAgentName = da.ContactName,
                                  DestinationCountryId = dc.CountryId,
                                  DestinationCountryName = dc.CountryName,
                                  DestinationCountryCode = dc.CountryCode,
                                  t.Direct,
                                  t.Deffered,
                                  c.CarrierId,
                                  c.CarrierName,
                                  c.CarrierType,
                                  t.TransitTime
                              }).ToList();

            if (tradelanes != null)
            {
                foreach (var tradelane in tradelanes)
                {
                    FrayteTradelane frateTradeLane = new FrayteTradelane();
                    frateTradeLane.TradelaneId = tradelane.TradelaneId;
                    frateTradeLane.Direct = tradelane.Direct;
                    frateTradeLane.Deffered = tradelane.Deffered;
                    frateTradeLane.OriginatingAgent = new FrayteUserModel();
                    frateTradeLane.OriginatingAgent.UserId = tradelane.OriginatingAgentId;
                    frateTradeLane.OriginatingAgent.Name = tradelane.OriginatingAgentName;
                    frateTradeLane.DestinationAgent = new FrayteUserModel();
                    frateTradeLane.DestinationAgent.UserId = tradelane.DestinationAgentId;
                    frateTradeLane.DestinationAgent.Name = tradelane.DestinationAgentName;
                    frateTradeLane.OriginatingCountry = new FrayteCountryCode();
                    frateTradeLane.OriginatingCountry.CountryId = tradelane.OriginatingCountryId;
                    frateTradeLane.OriginatingCountry.Code = tradelane.OriginatingCountryCode;
                    frateTradeLane.OriginatingCountry.Name = tradelane.OriginatingCountryName;
                    frateTradeLane.DestinationCountry = new FrayteCountryCode();
                    frateTradeLane.DestinationCountry.CountryId = tradelane.DestinationCountryId;
                    frateTradeLane.DestinationCountry.Code = tradelane.DestinationCountryCode;
                    frateTradeLane.DestinationCountry.Name = tradelane.DestinationCountryName;
                    frateTradeLane.Carrier = new FrayteCarrierModel();
                    frateTradeLane.Carrier.CarrierId = tradelane.CarrierId;
                    frateTradeLane.Carrier.CarrierName = tradelane.CarrierName;
                    frateTradeLane.CarrierType = tradelane.CarrierType;
                    frateTradeLane.TransitTime = tradelane.TransitTime;
                    lstTradelane.Add(frateTradeLane);
                }
            }

            return lstTradelane;
        }

        public FrayteTradelane SaveTradelane(FrayteTradelane frateTradelane)
        {
            try
            {
                Tradelane tradelane;
                if (frateTradelane.TradelaneId > 0)
                {
                    tradelane = dbContext.Tradelanes.Where(p => p.TradelaneId == frateTradelane.TradelaneId).FirstOrDefault();
                    tradelane.OriginatingAgentId = frateTradelane.OriginatingAgent.UserId;
                    tradelane.OriginCountryId = frateTradelane.OriginatingCountry.CountryId;
                    tradelane.DestinationAgentId = frateTradelane.DestinationAgent.UserId;
                    tradelane.DestinationCountryId = frateTradelane.DestinationCountry.CountryId;
                    tradelane.Direct = frateTradelane.Direct;
                    tradelane.Deffered = frateTradelane.Deffered;
                    tradelane.CarrierId = frateTradelane.Carrier.CarrierId;
                    tradelane.TransitTime = frateTradelane.TransitTime;
                    dbContext.SaveChanges();
                    frateTradelane.TradelaneId = tradelane.TradelaneId;
                }
                else
                {
                    tradelane = new Tradelane();
                    tradelane.OriginatingAgentId = frateTradelane.OriginatingAgent.UserId;
                    tradelane.OriginCountryId = frateTradelane.OriginatingCountry.CountryId;
                    tradelane.DestinationAgentId = frateTradelane.DestinationAgent.UserId;
                    tradelane.DestinationCountryId = frateTradelane.DestinationCountry.CountryId;
                    tradelane.Direct = frateTradelane.Direct;
                    tradelane.Deffered = frateTradelane.Deffered;
                    tradelane.CarrierId = frateTradelane.Carrier.CarrierId;
                    tradelane.TransitTime = frateTradelane.TransitTime;
                    dbContext.Tradelanes.Add(tradelane);
                    dbContext.SaveChanges();
                    frateTradelane.TradelaneId = tradelane.TradelaneId;
                }
                return frateTradelane;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public FrayteResult DeleteTradelane(int tradelaneId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var tradelane = new Tradelane { TradelaneId = tradelaneId };
                dbContext.Tradelanes.Attach(tradelane);
                dbContext.Tradelanes.Remove(tradelane);
                dbContext.SaveChanges();
                result.Status = true;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Errors = new List<string>();
                result.Errors.Add(ex.Message);
            }

            return result;
        }
    }
}
