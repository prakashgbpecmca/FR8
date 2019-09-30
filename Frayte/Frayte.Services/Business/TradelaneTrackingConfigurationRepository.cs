using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Business
{
    public class TradelaneTrackingConfigurationRepository
    {
        FrayteEntities dbContext = new FrayteEntities();
        public List<ShipmentHandlerModel> GetShimentHandlerMethods()
        {
            List<ShipmentHandlerModel> SHMList = new List<ShipmentHandlerModel>();
            var ShipHandMet = dbContext.ShipmentHandlerMethods.ToList();
            if (ShipHandMet != null)
            {
                foreach (var SHMet in ShipHandMet)
                {
                    ShipmentHandlerModel SHM = new ShipmentHandlerModel();
                    SHM.ShipmentHandlerMethodId = SHMet.ShipmentHandlerMethodId;
                    SHM.ShipmentHandlerMethodName = SHMet.ShipmentHandlerMethodName;
                    SHM.ShipmentHandlerMethodDisplay = SHMet.ShipmentHandlerMethodDisplay;
                    SHM.ShipmentHandlerMethodType = SHMet.ShipmentHandlerMethodType;
                    SHMList.Add(SHM);
                }
            }
            return SHMList;
        }
        public List<TrackingMileStoneModel> GetTrackingMileStone(int ShipmentHandlerMethodId)
        {
            List<TrackingMileStoneModel> SMList = new List<TrackingMileStoneModel>();
            var ShipMileStn = dbContext.TrackingMileStones.Where(a => a.ShipmentHandlerMethodId == ShipmentHandlerMethodId).ToList();
            if (ShipMileStn != null)
            {
                foreach (var SHMet in ShipMileStn)
                {
                    TrackingMileStoneModel TMS = new TrackingMileStoneModel();
                    TMS.TrackingMileStoneId = SHMet.TrackingMileStoneId;
                    TMS.MileStoneKey = SHMet.MileStoneKey;
                    TMS.Description = SHMet.Description;
                    TMS.OrderNumber = SHMet.OrderNumber;
                    TMS.CreatedBy = SHMet.CreatedBy;
                    TMS.CreatedOnUtc = SHMet.CreatedOnUtc;
                    TMS.UpdatedBy = SHMet.UpdatedBy.Value;
                    TMS.UpdatedOnUtc = SHMet.UpdatedOnUtc.Value;
                    TMS.ShipmentHandlerMethodId = SHMet.ShipmentHandlerMethodId;
                    SMList.Add(TMS);
                }
            }
            return SMList.OrderBy(a => a.OrderNumber).ToList();
        }

        public List<TradelaneTrackingMileStoneModel> GetTrackingConfiguration(int UserId, string TrackIds)
        {
            List<TradelaneUserTrackingConfiguration> Result = new List<TradelaneUserTrackingConfiguration>();
            TradelaneTrackingConfigurationModel TradTrackList = new TradelaneTrackingConfigurationModel();
            var TrackId = TrackIds.Split(',');
            var Resu = dbContext.TradelaneUserTrackingConfigurations.Where(a => a.UserId == UserId).ToList();
            if (TrackIds != null)
            {
                foreach (var tr in TrackId)
                {
                    foreach (var a in Resu)
                    {

                        if (a.TrackingMileStoneId == Convert.ToInt32(tr))
                        {
                            Result.Add(a);
                        }
                    }
                }
            }
            if (Result.Count > 0)
            {
                TradTrackList.TrackingMileStone = new List<TradelaneTrackingMileStoneModel>();
                foreach (var res in Result.Where(a => a.TrackingMileStoneId != null).ToList())
                {
                    TradelaneTrackingMileStoneModel TM = new TradelaneTrackingMileStoneModel();
                    TM.TradelaneUserTrackingConfigurationId = res.TradelaneUserTrackingConfigurationId;
                    TM.IsEmailSend = res.IsEmailSend;
                    TM.TrackingMileStoneId = res.TrackingMileStoneId.Value;
                    TM.UserId = res.UserId;
                    var ConfigResult = dbContext.TradelaneUserTrackingConfigurationDetails.Where(a => a.TradelaneUserTrackingConfigurationId == res.TradelaneUserTrackingConfigurationId).ToList();
                    TM.ConfigurationDetail = new List<TradelaneTrackingDetail>();
                    foreach (var ConfigDtl in ConfigResult)
                    {
                        TradelaneTrackingDetail TD = new TradelaneTrackingDetail();
                        TD.TradelaneUserTrackingConfigurationDetailId = ConfigDtl.TradelaneUserTrackingConfigurationDetailId;
                        TD.Name = ConfigDtl.Name;
                        TD.Email = ConfigDtl.Email;
                        TD.CreatedBy = ConfigDtl.CreatedBy;
                        TD.CreatedOnUtc = ConfigDtl.CreatedOnUtc;
                        TD.UpdatedOn = ConfigDtl.UpdatedOn.Value;
                        TD.UpdatedOnUtc = ConfigDtl.UpdatedOnUtc.Value;
                        TM.ConfigurationDetail.Add(TD);
                    }
                    TradTrackList.TrackingMileStone.Add(TM);
                }
                //TradTrackList.PreAlert = new TradelaneTrackingPreAlert();
                //var Pre = dbContext.TradelaneUserTrackingConfigurations.Where(a => a.OtherMethod != null).ToList();
                //foreach(var P in Pre)
                //{
                //    if(P.UserId == UserId)
                //    {
                //        TradTrackList.PreAlert.TradelaneUserTrackingConfigurationId = P.TradelaneUserTrackingConfigurationId;
                //        TradTrackList.PreAlert.IsEmailSend = P.IsEmailSend;
                //        TradTrackList.PreAlert.OtherMethod = P.OtherMethod;
                //        TradTrackList.PreAlert.UserId = P.UserId;
                //        var ConfigResult1 = dbContext.TradelaneUserTrackingConfigurationDetails.Where(a => a.TradelaneUserTrackingConfigurationId == TradTrackList.PreAlert.TradelaneUserTrackingConfigurationId).ToList();
                //        TradTrackList.PreAlert.ConfigurationDetail = new List<TradelaneTrackingDetail>();
                //        foreach (var ConfigDtl in ConfigResult1)
                //        {
                //            TradelaneTrackingDetail TD = new TradelaneTrackingDetail();
                //            TD.TradelaneUserTrackingConfigurationDetailId = ConfigDtl.TradelaneUserTrackingConfigurationDetailId;
                //            TD.Name = ConfigDtl.Name;
                //            TD.Email = ConfigDtl.Email;
                //            TD.CreatedBy = ConfigDtl.CreatedBy;
                //            TD.CreatedOnUtc = ConfigDtl.CreatedOnUtc;
                //            TD.UpdatedOn = ConfigDtl.UpdatedOn.Value;
                //            TD.UpdatedOnUtc = ConfigDtl.UpdatedOnUtc.Value;
                //            TradTrackList.PreAlert.ConfigurationDetail.Add(TD);
                //        }
                //        return TradTrackList;
                //    }
                //}
                return TradTrackList.TrackingMileStone;
            }
            else
            {
                return TradTrackList.TrackingMileStone;
            }
        }
        public TradelaneTrackingConfigurationModel SavingTrackingConfiguration(TradelaneTrackingConfigurationModel TradelaneConfig)
        {
            TradelaneTrackingConfigurationModel TC = new TradelaneTrackingConfigurationModel();
            //Saving Tradelane Milestone

            # region MileStone Save
            foreach (var TRKMileStone in TradelaneConfig.TrackingMileStone)
            {
                var result = dbContext.TradelaneUserTrackingConfigurations.Where(a => a.TradelaneUserTrackingConfigurationId == TRKMileStone.TradelaneUserTrackingConfigurationId).FirstOrDefault();
                if (result != null)
                {
                    if (TRKMileStone.IsEmailSend)
                    {
                        result.TrackingMileStoneId = TRKMileStone.TrackingMileStoneId;
                        result.UserId = TRKMileStone.UserId;
                        result.IsEmailSend = TRKMileStone.IsEmailSend;
                        dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        //Saving Tradelane Milestone Detail
                        foreach (var CD in TRKMileStone.ConfigurationDetail)
                        {
                            var ConfigDetail = dbContext.TradelaneUserTrackingConfigurationDetails.Where(a => a.TradelaneUserTrackingConfigurationDetailId == CD.TradelaneUserTrackingConfigurationDetailId).FirstOrDefault();
                            if (ConfigDetail != null)
                            {
                                ConfigDetail.TradelaneUserTrackingConfigurationId = result.TradelaneUserTrackingConfigurationId;
                                ConfigDetail.Name = CD.Name;
                                ConfigDetail.Email = CD.Email;
                                ConfigDetail.CreatedBy = result.UserId;
                                ConfigDetail.CreatedOnUtc = DateTime.UtcNow;
                                ConfigDetail.UpdatedOn = result.UserId;
                                ConfigDetail.UpdatedOnUtc = DateTime.UtcNow;
                                dbContext.Entry(ConfigDetail).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                TradelaneUserTrackingConfigurationDetail TUTCDB = new TradelaneUserTrackingConfigurationDetail();
                                TUTCDB.TradelaneUserTrackingConfigurationId = result.TradelaneUserTrackingConfigurationId;
                                TUTCDB.Name = CD.Name;
                                TUTCDB.Email = CD.Email;
                                TUTCDB.CreatedBy = result.UserId;
                                TUTCDB.CreatedOnUtc = DateTime.UtcNow;
                                TUTCDB.UpdatedOn = result.UserId;
                                TUTCDB.UpdatedOnUtc = DateTime.UtcNow;
                                dbContext.TradelaneUserTrackingConfigurationDetails.Add(TUTCDB);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        result.IsEmailSend = TRKMileStone.IsEmailSend;
                        dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
                else
                {
                    TradelaneUserTrackingConfiguration TrackConfiguration = new TradelaneUserTrackingConfiguration();
                    if (TRKMileStone.IsEmailSend)
                    {
                        TrackConfiguration.TrackingMileStoneId = TRKMileStone.TrackingMileStoneId;
                        TrackConfiguration.UserId = TRKMileStone.UserId;
                        TrackConfiguration.IsEmailSend = TRKMileStone.IsEmailSend;
                        dbContext.TradelaneUserTrackingConfigurations.Add(TrackConfiguration);
                        dbContext.SaveChanges();
                        //Saving Tradelane Milestone Detail
                        foreach (var CD in TRKMileStone.ConfigurationDetail)
                        {
                            var ConfigDetail = dbContext.TradelaneUserTrackingConfigurationDetails.Where(a => a.TradelaneUserTrackingConfigurationDetailId == CD.TradelaneUserTrackingConfigurationDetailId).FirstOrDefault();
                            if (ConfigDetail != null)
                            {
                                ConfigDetail.TradelaneUserTrackingConfigurationId = TrackConfiguration.TradelaneUserTrackingConfigurationId;
                                ConfigDetail.Name = CD.Name;
                                ConfigDetail.Email = CD.Email;
                                ConfigDetail.CreatedBy = TrackConfiguration.UserId;
                                ConfigDetail.CreatedOnUtc = DateTime.UtcNow;
                                ConfigDetail.UpdatedOn = TrackConfiguration.UserId;
                                ConfigDetail.UpdatedOnUtc = DateTime.UtcNow;
                                dbContext.Entry(ConfigDetail).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                TradelaneUserTrackingConfigurationDetail TUTCDB = new TradelaneUserTrackingConfigurationDetail();
                                TUTCDB.TradelaneUserTrackingConfigurationId = TrackConfiguration.TradelaneUserTrackingConfigurationId;
                                TUTCDB.Name = CD.Name;
                                TUTCDB.Email = CD.Email;
                                TUTCDB.CreatedBy = TrackConfiguration.UserId;
                                TUTCDB.CreatedOnUtc = DateTime.UtcNow;
                                TUTCDB.UpdatedOn = TrackConfiguration.UserId;
                                TUTCDB.UpdatedOnUtc = DateTime.UtcNow;
                                dbContext.TradelaneUserTrackingConfigurationDetails.Add(TUTCDB);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        TrackConfiguration.IsEmailSend = TRKMileStone.IsEmailSend;
                        dbContext.TradelaneUserTrackingConfigurations.Add(TrackConfiguration);
                        dbContext.SaveChanges();
                    }
                }
            }
            #endregion

            //Saving Tradelane PreAlert
            #region PreAlert Save
            var result1 = dbContext.TradelaneUserTrackingConfigurations.Where(a => a.TradelaneUserTrackingConfigurationId == TradelaneConfig.PreAlert.TradelaneUserTrackingConfigurationId).FirstOrDefault();
            if (result1 != null)
            {
                result1.TrackingMileStoneId = null;
                result1.UserId = TradelaneConfig.PreAlert.UserId;
                result1.IsEmailSend = TradelaneConfig.PreAlert.IsEmailSend;
                result1.OtherMethod = "PreAlert";
                dbContext.Entry(result1).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                //Saving Tradelane Milestone Detail
                foreach (var CD in TradelaneConfig.PreAlert.ConfigurationDetail)
                {
                    var ConfigDetail = dbContext.TradelaneUserTrackingConfigurationDetails.Where(a => a.TradelaneUserTrackingConfigurationDetailId == CD.TradelaneUserTrackingConfigurationDetailId).FirstOrDefault();
                    if (ConfigDetail != null)
                    {
                        ConfigDetail.TradelaneUserTrackingConfigurationId = result1.TradelaneUserTrackingConfigurationId;
                        ConfigDetail.Name = CD.Name;
                        ConfigDetail.Email = CD.Email;
                        ConfigDetail.CreatedBy = result1.UserId;
                        ConfigDetail.CreatedOnUtc = DateTime.UtcNow;
                        ConfigDetail.UpdatedOn = result1.UserId;
                        ConfigDetail.UpdatedOnUtc = DateTime.UtcNow;
                        dbContext.Entry(ConfigDetail).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        TradelaneUserTrackingConfigurationDetail TUTCDB = new TradelaneUserTrackingConfigurationDetail();
                        TUTCDB.TradelaneUserTrackingConfigurationId = result1.TradelaneUserTrackingConfigurationId;
                        TUTCDB.Name = CD.Name;
                        TUTCDB.Email = CD.Email;
                        TUTCDB.CreatedBy = result1.UserId;
                        TUTCDB.CreatedOnUtc = DateTime.UtcNow;
                        TUTCDB.UpdatedOn = result1.UserId;
                        TUTCDB.UpdatedOnUtc = DateTime.UtcNow;
                        dbContext.TradelaneUserTrackingConfigurationDetails.Add(TUTCDB);
                        dbContext.SaveChanges();
                    }
                }
            }
            else
            {
                TradelaneUserTrackingConfiguration TrackConfiguration = new TradelaneUserTrackingConfiguration();
                TrackConfiguration.TrackingMileStoneId = null;
                TrackConfiguration.UserId = TradelaneConfig.PreAlert.UserId;
                TrackConfiguration.IsEmailSend = TradelaneConfig.PreAlert.IsEmailSend;
                TrackConfiguration.OtherMethod = "PreAlert";
                dbContext.TradelaneUserTrackingConfigurations.Add(TrackConfiguration);
                dbContext.SaveChanges();
                //Saving Tradelane Milestone Detail
                foreach (var CD in TradelaneConfig.PreAlert.ConfigurationDetail)
                {
                    var ConfigDetail = dbContext.TradelaneUserTrackingConfigurationDetails.Where(a => a.TradelaneUserTrackingConfigurationDetailId == CD.TradelaneUserTrackingConfigurationDetailId).FirstOrDefault();
                    if (ConfigDetail != null)
                    {
                        ConfigDetail.TradelaneUserTrackingConfigurationId = TrackConfiguration.TradelaneUserTrackingConfigurationId;
                        ConfigDetail.Name = CD.Name;
                        ConfigDetail.Email = CD.Email;
                        ConfigDetail.CreatedBy = TrackConfiguration.UserId;
                        ConfigDetail.CreatedOnUtc = DateTime.UtcNow;
                        ConfigDetail.UpdatedOn = TrackConfiguration.UserId;
                        ConfigDetail.UpdatedOnUtc = DateTime.UtcNow;
                        dbContext.Entry(ConfigDetail).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        TradelaneUserTrackingConfigurationDetail TUTCDB = new TradelaneUserTrackingConfigurationDetail();
                        TUTCDB.TradelaneUserTrackingConfigurationId = TrackConfiguration.TradelaneUserTrackingConfigurationId;
                        TUTCDB.Name = CD.Name;
                        TUTCDB.Email = CD.Email;
                        TUTCDB.CreatedBy = TrackConfiguration.UserId;
                        TUTCDB.CreatedOnUtc = DateTime.UtcNow;
                        TUTCDB.UpdatedOn = TrackConfiguration.UserId;
                        TUTCDB.UpdatedOnUtc = DateTime.UtcNow;
                        dbContext.TradelaneUserTrackingConfigurationDetails.Add(TUTCDB);
                        dbContext.SaveChanges();
                    }
                }
            }

            #endregion
            return TC;

        }

        public bool DeleteTrackingConfigurationDetail(int TradelaneUserTrackingConfigurationDetailId)
        {
            var result = dbContext.TradelaneUserTrackingConfigurationDetails.Where(a => a.TradelaneUserTrackingConfigurationDetailId == TradelaneUserTrackingConfigurationDetailId).FirstOrDefault();
            if (result != null)
            {
                dbContext.TradelaneUserTrackingConfigurationDetails.Remove(result);
                dbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public TradelaneTrackingPreAlert GetPreAlert(string UserId)
        {
            var Id = Convert.ToInt32(UserId);
            var result = dbContext.TradelaneUserTrackingConfigurations.Where(a => a.UserId == Id).ToList();
            TradelaneTrackingPreAlert TPreAlert = new TradelaneTrackingPreAlert();
            if (result != null && result.Count > 0)
            {
                var Res = result.Where(a => a.OtherMethod == "PreAlert").FirstOrDefault();
                TPreAlert.IsEmailSend = Res.IsEmailSend;
                TPreAlert.OtherMethod = Res.OtherMethod;
                TPreAlert.TradelaneUserTrackingConfigurationId = Res.TradelaneUserTrackingConfigurationId;
                TPreAlert.UserId = Res.UserId;
                TPreAlert.ConfigurationDetail = new List<TradelaneTrackingDetail>();
                var ConfigDetail = dbContext.TradelaneUserTrackingConfigurationDetails.Where(a => a.TradelaneUserTrackingConfigurationId == Res.TradelaneUserTrackingConfigurationId).ToList();
                if (ConfigDetail != null && ConfigDetail.Count > 0)
                {
                    foreach (var Con in ConfigDetail)
                    {
                        TradelaneTrackingDetail TD = new TradelaneTrackingDetail();
                        TD.CreatedBy = Con.CreatedBy;
                        TD.CreatedOnUtc = Con.CreatedOnUtc;
                        TD.Name = Con.Name;
                        TD.Email = Con.Email;
                        TD.TradelaneUserTrackingConfigurationDetailId = Con.TradelaneUserTrackingConfigurationDetailId;
                        TD.UpdatedOn = Con.UpdatedOn.Value;
                        TD.UpdatedOnUtc = Con.UpdatedOnUtc.Value;
                        TPreAlert.ConfigurationDetail.Add(TD);

                    }
                }
                return TPreAlert;
            }
            else
            {
                return TPreAlert;
            }
        }

    }
}
