using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.Tradelane;
using Frayte.Services.Utility;
using System.Web.Hosting;
using System.Web;
using System.IO;
using System.Data.Entity.Validation;

namespace Frayte.Services.Business
{
    public class TradelaneShipmentRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<TradelaneGetShipmentModel> GetTradelaneShipments(TradelaneTrackDirectBooking track)
        {
            List<TradelaneGetShipmentModel> ShipmentList = new List<TradelaneGetShipmentModel>();
            if (track != null)
            {
                FrayteOperationZone OperationZone = UtilityRepository.GetOperationZone();
                int RoleId = dbContext.UserRoles.Where(p => p.UserId == (track.UserId > 0 ? track.UserId : track.CustomerId)).FirstOrDefault().RoleId;
                int TotalRows = 0;
                int SkipRows = 0;
                SkipRows = (track.CurrentPage - 1) * track.TakeRows;
                DateTime? fromdate;
                DateTime? todate;
                var Airline = "";

                if (track.FromDate.HasValue)
                {
                    fromdate = track.FromDate.Value;
                }
                else
                {
                    fromdate = track.FromDate;
                }

                if (track.ToDate.HasValue)
                {
                    todate = track.ToDate.Value;
                }
                else
                {
                    todate = track.ToDate;
                }

                if (track.MAWB != null && track.MAWB != "")
                {
                    var Result1 = GetMawbStr(track.MAWB);
                    track.MAWB = Result1.Item1;
                    Airline = Result1.Item2;
                }

                var Result = dbContext.spGet_TradelaneTrackAndTraceDetail(track.MAWB, fromdate, todate, track.ShipmentStatusId, track.FrayteNumber, SkipRows, track.TakeRows, track.CustomerId, track.UserId, OperationZone.OperationZoneId, RoleId, track.SpecialSearchId, Airline).ToList();
                foreach (var res in Result)
                {
                    var STCode = "";
                    TradelaneGetShipmentModel Shipment = new TradelaneGetShipmentModel();
                    Shipment.DepartureAirport = res.DepartureAirport;
                    Shipment.DestinationAirport = res.DestinationAirport;
                    Shipment.CustomerId = res.CustomerId;
                    Shipment.CreatedOn = res.CreatedOnUtc.Date;
                    Shipment.Customer = res.ContactName;
                    Shipment.IsAgentMAWBAllocated = res.IsAgentMAWBAllocated != null ? res.IsAgentMAWBAllocated.Value : false;
                    Shipment.MAWBAgentId = res.MAWBAgentId != null ? res.MAWBAgentId.Value : 0;
                    Shipment.TradelaneShipmentId = res.TradelaneShipmentId;
                    STCode = !string.IsNullOrEmpty(res.ShipmentHandlerMethodCode) ? " (" + res.ShipmentHandlerMethodCode + ')' : "";
                    Shipment.Status = res.StatusName;
                    Shipment.StatusDisplay = res.DisplayStatusName + STCode;
                    Shipment.MAWB = res.MAWB;
                    Shipment.TotalWeight = res.TotalWeight != null ? res.TotalWeight.Value : 0;
                    Shipment.TotalCarton = res.TotalCartons != null ? res.TotalCartons.Value : 0;
                    Shipment.TotalRows = res.TotalRows.Value;
                    ShipmentList.Add(Shipment);
                }
                return ShipmentList.OrderBy(a => a.Status).ToList();
            }
            return ShipmentList;
        }

        public TradelaneGetShipmentModel GetShipmentDetail(string FrayteNumber, string FrayteType)
        {
            TradelaneGetShipmentModel TGS = new TradelaneGetShipmentModel();
            if (FrayteType == "FrayteRefNo")
            {
                var Shipment = dbContext.TradelaneShipments.Where(x => x.FrayteNumber == FrayteNumber).FirstOrDefault();
                if (Shipment != null)
                {
                    TGS.TradelaneShipmentId = Shipment.TradelaneShipmentId;
                }
            }
            else if (FrayteType == "Mawb")
            {
                var MawbRes = GetMawbStr(FrayteNumber);
                var Mawb = MawbRes.Item1;
                var AirlineId = MawbRes.Item2;
                if (!string.IsNullOrEmpty(AirlineId))
                {
                    var Airline = dbContext.Airlines.Where(a => a.AirlineCode == AirlineId).FirstOrDefault();
                    var Shipment = dbContext.TradelaneShipments.Where(x => x.MAWB == Mawb && x.AirlineId == Airline.AirlineId).FirstOrDefault();
                    if (Shipment != null)
                    {
                        TGS.TradelaneShipmentId = Shipment.TradelaneShipmentId;
                    }
                }
                else
                {
                    var Shipment = dbContext.TradelaneShipments.Where(x => x.MAWB == Mawb).FirstOrDefault();
                    if (Shipment != null)
                    {
                        TGS.TradelaneShipmentId = Shipment.TradelaneShipmentId;
                    }
                }
            }
            return TGS;
        }

        public List<ShipmentStatu> GetDirectShipmentStatusList(string BookingType)
        {
            var list = dbContext.ShipmentStatus.Where(x => x.BookingType == BookingType).ToList();
            return list;
        }

        public List<DirectBookingCustomer> GetTradlaneCustomers()
        {
            // To Do : customer should come according to moduleType
            var operationzone = UtilityRepository.GetOperationZone();
            var customers = (from r in dbContext.Users
                             join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                             join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                             where
                                ur.RoleId == (int)FrayteUserRole.Customer &&
                                r.IsActive == true &&
                                ua.IsTradelaneBooking == true &&
                                r.OperationZoneId == operationzone.OperationZoneId
                             select new DirectBookingCustomer
                             {
                                 CustomerId = r.UserId,
                                 CustomerName = r.ContactName,
                                 CompanyName = r.CompanyName,
                                 AccountNumber = ua.AccountNo,
                                 EmailId = r.UserEmail,
                                 ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                 CustomerCurrency = ua.CreditLimitCurrencyCode,
                                 OperationZoneId = r.OperationZoneId
                             }).Distinct().ToList();

            return customers.OrderBy(p => p.CompanyName).ToList();
        }

        public FrayteResult DeleteTradelaneShipment(int TradelaneShipmentId)
        {
            FrayteResult FE = new FrayteResult();
            FE.Status = false;
            var Result = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).FirstOrDefault();
            if (Result != null)
            {
                var FromAddress = dbContext.TradelaneShipmentAddresses.Where(a => a.TradelaneShipmentAddressId == Result.FromAddressId).FirstOrDefault();
                var ToAddress = dbContext.TradelaneShipmentAddresses.Where(a => a.TradelaneShipmentAddressId == Result.ToAddressId).FirstOrDefault();
                var TradelaneShipmentDetail = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == Result.TradelaneShipmentId).ToList();
                var TradelaneShipmentDoc = dbContext.TradelaneShipmentDocuments.Where(a => a.TradelaneShipmentId == Result.TradelaneShipmentId).ToList();
                if (FromAddress != null)
                {
                    dbContext.TradelaneShipmentAddresses.Remove(FromAddress);
                    dbContext.SaveChanges();
                }
                if (ToAddress != null)
                {
                    dbContext.TradelaneShipmentAddresses.Remove(ToAddress);
                    dbContext.SaveChanges();
                }
                if (TradelaneShipmentDetail != null && TradelaneShipmentDetail.Count > 0)
                {
                    dbContext.TradelaneShipmentDetails.RemoveRange(TradelaneShipmentDetail);
                    dbContext.SaveChanges();
                }
                if (TradelaneShipmentDoc != null && TradelaneShipmentDoc.Count > 0)
                {
                    dbContext.TradelaneShipmentDocuments.RemoveRange(TradelaneShipmentDoc);
                    dbContext.SaveChanges();
                }
                dbContext.TradelaneShipments.Remove(Result);
                dbContext.SaveChanges();
                FE.Status = true;
                return FE;
            }
            else
            {
                return FE;
            }
        }

        public void SavePreAlertLog(TradelanePreAlertInitial preAlerDetail)
        {
            try
            {
                TradelaneLogging log = new TradelaneLogging();
                log.ShipmentId = preAlerDetail.TradelaneShipmentId;
                log.SentBy = preAlerDetail.UserId;
                log.SentOnUtc = DateTime.UtcNow;
                log.Type = TradelaneLoggingType.PreAlert;

                dbContext.TradelaneLoggings.Add(log);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public TradelaneAgentEmailModel GetAgentsMail(int TradelaneShipmentId)
        {
            TradelaneAgentEmailModel AE = new TradelaneAgentEmailModel();
            var Res = new TradelaneEmailRepository().TradelaneEmailObj(TradelaneShipmentId);

            var AgentName = "";
            var CreatedByMail = "";
            var CreatedBy = (from Usr in dbContext.Users
                             join usrAdd in dbContext.UserAdditionals on Usr.UserId equals usrAdd.UserId
                             join Usr1 in dbContext.Users on usrAdd.OperationUserId equals Usr1.UserId
                             where Usr.UserId == Res.ShipmentDetail.CustomerId
                             select new
                             {
                                 Usr1.Email
                             }).FirstOrDefault();

            if (CreatedBy != null)
            {
                AE.staff = CreatedBy.Email;
            }

            if (Res.ShipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId == 5)
            {
                var Leg1Agent = dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentId == Res.ShipmentDetail.TradelaneShipmentId && a.LegNum == "Leg1").FirstOrDefault();
                if (Leg1Agent != null)
                {
                    var AgentDetail1 = (from Usr in dbContext.Users
                                        where Usr.UserId == Leg1Agent.AgentId
                                        select new
                                        {
                                            Usr.Email,
                                            Usr.ContactName
                                        }).FirstOrDefault();

                    if (AgentDetail1 != null)
                    {
                        AE.Agent = AgentDetail1.Email;
                        AE.AgentName = AgentDetail1.ContactName;
                    }
                }
                var Leg2Agent = dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentId == Res.ShipmentDetail.TradelaneShipmentId && a.LegNum == "Leg2").FirstOrDefault();
                if (Leg2Agent != null)
                {
                    var AgentDetail2 = (from Usr in dbContext.Users
                                        where Usr.UserId == Leg2Agent.AgentId
                                        select new
                                        {
                                            Usr.Email,
                                            Usr.ContactName
                                        }).FirstOrDefault();

                    if (AgentDetail2 != null)
                    {
                        AE.Agent = AE.Agent + ", " + AgentDetail2.Email;
                        AE.AgentName = AgentDetail2.ContactName;
                    }
                }
            }
            else
            {
                var LegAgent = dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentId == Res.ShipmentDetail.TradelaneShipmentId).FirstOrDefault();
                if (LegAgent != null)
                {
                    var AgentDetail = (from Usr in dbContext.Users
                                       where Usr.UserId == LegAgent.AgentId
                                       select new
                                       {
                                           Usr.Email,
                                           Usr.ContactName
                                       }).FirstOrDefault();

                    if (AgentDetail != null)
                    {
                        AE.Agent = AgentDetail.Email;
                        AE.AgentName = AgentDetail.ContactName;
                    }
                }
            }
            return AE;
        }

        public fileName GetLatestMawbDocuments(int TradelaneShipmentId)
        {
            fileName fn = new fileName();
            var LatestDocument = dbContext.TradelaneShipmentDocuments.Where(a => a.TradelaneShipmentId == TradelaneShipmentId && a.DocumentType == "MAWB").OrderByDescending(a => a.RevisionNumber).FirstOrDefault();
            if (LatestDocument != null)
            {
                fn.FileName = LatestDocument.DocumentName;
                fn.FilePath = AppSettings.UploadFolderPath + "/Tradelane/" + TradelaneShipmentId + "/" + LatestDocument.DocumentName;
            }

            return fn;
        }

        public FrayteResult MawbCorrection(TradelaneClaimShipmentModel CorrectionModel)
        {
            FrayteResult FR = new FrayteResult();
            FR.Status = false;
            var Res = new TradelaneEmailRepository().TradelaneEmailObj(CorrectionModel.TradelaneShipmentId);
            var MAList = dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentId == CorrectionModel.TradelaneShipmentId).ToList();
            var File = GetLatestMawbDocuments(CorrectionModel.TradelaneShipmentId);
            if (Res.ShipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId == 5)
            {
                if (MAList.FirstOrDefault().AgentId > 0)
                {
                    new TradelaneEmailRepository().SendMawbCorrectionShipment(MAList.FirstOrDefault(), CorrectionModel, File.FilePath);
                }

                if (MAList.Skip(1).FirstOrDefault().AgentId > 0)
                {
                    new TradelaneEmailRepository().SendMawbCorrectionShipment(MAList.Skip(1).FirstOrDefault(), CorrectionModel, File.FilePath);
                }
                FR.Status = true;

            }
            else
            {
                if (MAList.FirstOrDefault().AgentId > 0)
                {
                    new TradelaneEmailRepository().SendMawbCorrectionShipment(MAList.FirstOrDefault(), CorrectionModel, File.FilePath);
                }
                FR.Status = true;
            }

            if (FR.Status == true)
            {
                var Result = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == CorrectionModel.TradelaneShipmentId).FirstOrDefault();
                if (Result != null)
                {
                    Result.IsMawbCorrection = true;
                    dbContext.Entry(Result).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
            return FR;
        }

        #region Preview HAWB
        public List<string> GetShipmentHAWB(int shipmentId)
        {
            return dbContext.TradelaneShipmentDetails.Where(p => p.TradelaneShipmentId == shipmentId).GroupBy(p => p.HAWB)
                              .Select(group => group.FirstOrDefault().HAWB).Distinct().ToList();
        }

        #endregion

        #region  PreALert

        public TradelanePreAlertInitial PreALertInitials(int shipmentId)
        {
            TradelanePreAlertInitial preAlert = new TradelanePreAlertInitial();

            preAlert.PreAlerEmailTo = (from r in dbContext.TradelaneShipments
                                       join utc in dbContext.TradelaneUserTrackingConfigurations on r.CustomerId equals utc.UserId
                                       join utcd in dbContext.TradelaneUserTrackingConfigurationDetails on utc.TradelaneUserTrackingConfigurationId equals utcd.TradelaneUserTrackingConfigurationId
                                       where r.TradelaneShipmentId == shipmentId &&
                                             utc.OtherMethod == "PreAlert"
                                       select new PreAlertEmail
                                       {
                                           Email = utcd.Email,
                                           Name = utcd.Name,
                                           TradelaneUserTrackingConfigurationDetailId = utcd.TradelaneUserTrackingConfigurationDetailId
                                       }).ToList();

            preAlert.PreAlertDocumnets = (from r in dbContext.TradelaneShipmentDocuments select r)
                                         .Where(p => p.TradelaneShipmentId == shipmentId)
                                         .GroupBy(x => x.DocumentType)
                                         .Select(group => new TradelanePreAlertDocument
                                         {
                                             DocumentType = group.FirstOrDefault().DocumentType,
                                             DocumentTypeDisplay = group.FirstOrDefault().DocumentNameDisplay,
                                             Documents = group
                                                        .Select(subgroup => new TradelanePreAlertDoc
                                                        {
                                                            Document = subgroup.DocumentName,
                                                            TradelaneShipmentDocumentId = subgroup.TradelaneShipmentDocumentId,
                                                            IsSelected = false
                                                        }).ToList()
                                         }).OrderBy(p => p.DocumentType).ToList();

            preAlert.ShipmentEmail = (from r in dbContext.TradelaneShipments
                                      join oa in dbContext.TradelaneShipmentAddresses on r.FromAddressId equals oa.TradelaneShipmentAddressId
                                      join da in dbContext.TradelaneShipmentAddresses on r.ToAddressId equals da.TradelaneShipmentAddressId
                                      join na in dbContext.TradelaneShipmentAddresses on r.NotifyPartyAddressId equals na.TradelaneShipmentAddressId into leftJoinTemp
                                      from caTemp in leftJoinTemp.DefaultIfEmpty()
                                      join u in dbContext.Users on r.CustomerId equals u.UserId
                                      join ua in dbContext.Users on r.MAWBAgentId equals ua.UserId
                                      where r.TradelaneShipmentId == shipmentId
                                      select new TradelaneShipmentEmailUser
                                      {
                                          CustomerEmail = u.UserEmail,
                                          NotifyPartyEmail = (caTemp == null ? da.Email : caTemp.Email),
                                          ReceiverEmail = da.Email,
                                          ShipperEmail = oa.Email,
                                          AgentEmail = ua.Email
                                      }).FirstOrDefault();

            // Get Kind Regards Information
            var staffDetail = (from r in dbContext.TradelaneShipments
                               join u in dbContext.Users on r.CustomerId equals u.UserId
                               join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                               join uau in dbContext.Users on ua.OperationUserId equals uau.UserId
                               join uaud in dbContext.UserAdditionals on uau.UserId equals uaud.UserId
                               join uaudd in dbContext.UserAddresses on uau.UserId equals uaudd.UserId
                               join c in dbContext.Countries on uaudd.CountryId equals c.CountryId
                               where r.TradelaneShipmentId == shipmentId
                               select new
                               {
                                   UserName = uau.ContactName,
                                   UserEmail = uau.Email,
                                   PhoneNumber = uau.TelephoneNo,
                                   CountryCode = c.CountryPhoneCode,
                                   Position = uau.Position,
                                   AdditionalInfo = r.AdditionalInfo
                               }).FirstOrDefault();

            if (staffDetail != null)
            {
                preAlert.StaffUserName = staffDetail.UserName;
                preAlert.StaffUserEmail = staffDetail.UserEmail;
                preAlert.StaffUserPosition = staffDetail.Position;
                preAlert.StaffUserPhone = "(+" + staffDetail.CountryCode + ") " + staffDetail.PhoneNumber;
                preAlert.SiteCompany = "FRAYTE GLOBAL";
                preAlert.AdditionalInfo = staffDetail.AdditionalInfo;
                preAlert.SiteAddress = UtilityRepository.GetOperationZone().OperationZoneId == 1 ? "www.FRAYTE.com" : "www.FRAYTE.co.uk";
            }

            return preAlert;
        }

        public FrayteResult RemoveOtherDocument(int tradelaneShipmentDocumentId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var shipmentDoc = dbContext.TradelaneShipmentDocuments.Find(tradelaneShipmentDocumentId);
                if (shipmentDoc != null)
                {
                    string filePhysicalPath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + shipmentDoc.TradelaneShipmentId + "/" + shipmentDoc.DocumentName);

                    if (File.Exists(filePhysicalPath))
                    {
                        File.Delete(filePhysicalPath);
                    }
                    dbContext.TradelaneShipmentDocuments.Remove(shipmentDoc);
                    dbContext.SaveChanges();
                    result.Status = true;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
            }
            return result;
        }

        public FrayteResult SendPreAlertEmail(TradelanePreAlertInitial preAlerDetail)
        {
            var emailModel = new TradelaneEmailRepository().TradelaneEmailObj(preAlerDetail.TradelaneShipmentId);
            FrayteResult result = new TradelaneEmailRepository().SendEmail_E5(emailModel, preAlerDetail);
            return result;
        }

        public string PreAlertShipmentDocuments(TradelanePreAlertInitial preAlerDetail)
        {
            string attachments = string.Empty;
            string documentFolder = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + preAlerDetail.TradelaneShipmentId);

            if (preAlerDetail != null)
            {
                if (preAlerDetail.PreAlertDocumnets.Count > 0)
                {
                    foreach (var item in preAlerDetail.PreAlertDocumnets)
                    {
                        if (item.Documents != null && item.Documents.Count > 0)
                        {
                            foreach (var doc in item.Documents)
                            {
                                if (doc.IsSelected)
                                {
                                    if (doc.TradelaneShipmentDocumentId > 0)
                                    {
                                        if (!string.IsNullOrEmpty(doc.Document) && File.Exists(documentFolder + "\\" + doc.Document))
                                        {
                                            attachments += documentFolder + "\\" + doc.Document;
                                            attachments += ";";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return attachments;
        }

        public string PreAlertEmails(TradelanePreAlertInitial preAlerDetail, string type)
        {
            string emails = string.Empty;
            if (type == "TO")
            {
                if (preAlerDetail.ShipmentEmail != null)
                {
                    if (!string.IsNullOrEmpty(preAlerDetail.ShipmentEmail.CustomerEmail) && preAlerDetail.ShipmentEmail.IsCustomerSentEmail)
                    {
                        emails += preAlerDetail.ShipmentEmail.CustomerEmail + ";";
                    }
                    if (!string.IsNullOrEmpty(preAlerDetail.ShipmentEmail.ShipperEmail) && preAlerDetail.ShipmentEmail.IsShipperSentEmail)
                    {
                        emails += preAlerDetail.ShipmentEmail.ShipperEmail + ";";
                    }
                    if (!string.IsNullOrEmpty(preAlerDetail.ShipmentEmail.ReceiverEmail) && preAlerDetail.ShipmentEmail.IsReceiverSentEmail)
                    {
                        emails += preAlerDetail.ShipmentEmail.ReceiverEmail + ";";
                    }
                    if (!string.IsNullOrEmpty(preAlerDetail.ShipmentEmail.NotifyPartyEmail) && preAlerDetail.ShipmentEmail.IsNotifyPartySentEmail)
                    {
                        emails += preAlerDetail.ShipmentEmail.NotifyPartyEmail + ";";
                    }
                    if (!string.IsNullOrEmpty(preAlerDetail.ShipmentEmail.AgentEmail) && preAlerDetail.ShipmentEmail.IsAgentSentEmail)
                    {
                        emails += preAlerDetail.ShipmentEmail.AgentEmail + ";";
                    }
                }
                if (preAlerDetail.PreAlerEmailTo != null && preAlerDetail.PreAlerEmailTo.Count > 0)
                {
                    foreach (var item in preAlerDetail.PreAlerEmailTo)
                    {
                        if (!string.IsNullOrEmpty(item.Email))
                        {
                            emails += item.Email;
                            emails += ";";
                        }
                    }
                }
            }
            else if (type == "CC")
            {
                if (preAlerDetail.PreAlerEmailCC != null && preAlerDetail.PreAlerEmailCC.Count > 0)
                {
                    foreach (var item in preAlerDetail.PreAlerEmailCC)
                    {
                        if (!string.IsNullOrEmpty(item.Email))
                        {
                            emails += item.Email;
                            emails += ";";
                        }
                    }
                }
            }
            else if (type == "BCC")
            {
                if (preAlerDetail.PreAlerEmailBCC != null && preAlerDetail.PreAlerEmailBCC.Count > 0)
                {
                    foreach (var item in preAlerDetail.PreAlerEmailBCC)
                    {
                        if (!string.IsNullOrEmpty(item.Email))
                        {
                            emails += item.Email;
                            emails += ";";
                        }
                    }
                }
            }
            else
            {

            }

            return emails;
        }

        #endregion

        public Tuple<string, string> GetMawbStr(string Mawb)
        {
            var Airline = "";
            if (Mawb != null && Mawb != "")
            {
                if (Mawb.Length < 10)
                {
                    //Mawb = Mawb.Trim().Replace(" ", "");
                    //Mawb = Mawb.Trim().Replace("-", "");
                    if (Mawb.Length == 8)
                    {
                        char[] charArray = Mawb.ToCharArray();
                        Array.Reverse(charArray);
                        Mawb = new string(charArray);
                        Mawb = (string)Mawb.Substring(0, 8);
                        char[] charArray1 = Mawb.ToCharArray();
                        Array.Reverse(charArray1);
                        Mawb = new string(charArray1);
                    }
                    if (Mawb.Length == 9 && Mawb.Contains('-'))
                    {
                        var MawbAry = Mawb.Split('-');
                        if (MawbAry[0].Length == 4 && MawbAry[1].Length == 4)
                        {
                            Mawb = Mawb.Trim().Replace("-", "");
                            char[] charArray = Mawb.ToCharArray();
                            Array.Reverse(charArray);
                            Mawb = new string(charArray);
                            Mawb = (string)Mawb.Substring(0, 8);
                            char[] charArray1 = Mawb.ToCharArray();
                            Array.Reverse(charArray1);
                            Mawb = new string(charArray1);
                        }
                    }
                    if (Mawb.Length == 9 && Mawb.Contains(' '))
                    {
                        var MawbAry = Mawb.Split(' ');
                        if (MawbAry[0].Length == 4 && MawbAry[1].Length == 4)
                        {
                            Mawb = Mawb.Trim().Replace(" ", "");
                            char[] charArray = Mawb.ToCharArray();
                            Array.Reverse(charArray);
                            Mawb = new string(charArray);
                            Mawb = (string)Mawb.Substring(0, 8);
                            char[] charArray1 = Mawb.ToCharArray();
                            Array.Reverse(charArray1);
                            Mawb = new string(charArray1);
                        }
                    }
                }
                else if (Mawb.Length > 10)
                {
                    Mawb = Mawb.Trim().Replace(" ", "");
                    if (Mawb.Length == 11)
                    {
                        Airline = Mawb.Substring(0, 3);
                        char[] charArray = Mawb.ToCharArray();
                        Array.Reverse(charArray);
                        Mawb = new string(charArray);
                        Mawb = (string)Mawb.Substring(0, 8);
                        char[] charArray1 = Mawb.ToCharArray();
                        Array.Reverse(charArray1);
                        Mawb = new string(charArray1);
                    }
                    else if (Mawb.Length == 12 && Mawb.Contains('-'))
                    {
                        var MawbAry = Mawb.Split('-');
                        Mawb = Mawb.Trim().Replace("-", "");
                        Airline = Mawb.Substring(0, 3);
                        if (MawbAry[0].Length == 3)
                        {
                            char[] charArray = Mawb.ToCharArray();
                            Array.Reverse(charArray);
                            Mawb = new string(charArray);
                            Mawb = (string)Mawb.Substring(0, 8);
                            char[] charArray1 = Mawb.ToCharArray();
                            Array.Reverse(charArray1);
                            Mawb = new string(charArray1);
                        }
                    }
                    else if (Mawb.Length == 13 && Mawb.Contains('-'))
                    {
                        var MawbAry = Mawb.Split('-');
                        Mawb = Mawb.Trim().Replace("-", "");
                        Airline = Mawb.Substring(0, 3);
                        if (MawbAry[0].Length == 3 && MawbAry[1].Length == 4 && MawbAry[2].Length == 4)
                        {
                            char[] charArray = Mawb.ToCharArray();
                            Array.Reverse(charArray);
                            Mawb = new string(charArray);
                            Mawb = (string)Mawb.Substring(0, 8);
                            char[] charArray1 = Mawb.ToCharArray();
                            Array.Reverse(charArray1);
                            Mawb = new string(charArray1);
                        }
                    }

                }
            }
            return Tuple.Create(Mawb, Airline);
        }

        #region AvinashCode

        public bool SaveMAWBCustomized(MAWBCustomizedFiled mawbCustomizefield)
        {
            try
            {
                var detail = dbContext.MAWBCustomizeds.Find(mawbCustomizefield.MAWBCustomizedeFieldId);
                if (detail != null)
                {
                    detail.TradelaneShipmentId = mawbCustomizefield.TradelaneShipmentId;
                    detail.IssuingCarriersAgentNameandCity = mawbCustomizefield.IssuingCarriersAgentNameandCity;
                    detail.DeclaredValueForCarriage = mawbCustomizefield.DeclaredValueForCarriage;
                    detail.DeclaredValueForCustoms = mawbCustomizefield.DeclaredValueForCustoms;
                    detail.ValuationCharge = mawbCustomizefield.ValuationCharge;
                    detail.Tax = mawbCustomizefield.Tax;
                    detail.TotalOtherChargesDueAgent = mawbCustomizefield.TotalOtherChargesDueAgent;
                    detail.TotalOtherChargesDueCarrier = mawbCustomizefield.TotalOtherChargesDueCarrier;
                    detail.OtherCharges = mawbCustomizefield.OtherCharges;
                    detail.ChargesAtDestination = mawbCustomizefield.ChargesAtDestination;
                    detail.TotalCollectCharges = mawbCustomizefield.TotalCollectCharges;
                    detail.CurrencyConversionRates = mawbCustomizefield.CurrencyConversionRates;
                    detail.TotalPrepaid = mawbCustomizefield.TotalPrepaid;
                    detail.TotalCollect = mawbCustomizefield.TotalCollect;
                    detail.HandlingInformation = mawbCustomizefield.HandlingInformation;
                    detail.AgentsIATACode = mawbCustomizefield.AgentsIATACode;
                    detail.AccountNo = mawbCustomizefield.AccountNo;
                    dbContext.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
                else
                {
                    MAWBCustomized customized = new MAWBCustomized();
                    customized.TradelaneShipmentId = mawbCustomizefield.TradelaneShipmentId;
                    customized.IssuingCarriersAgentNameandCity = mawbCustomizefield.IssuingCarriersAgentNameandCity;
                    customized.DeclaredValueForCarriage = mawbCustomizefield.DeclaredValueForCarriage;
                    customized.DeclaredValueForCustoms = mawbCustomizefield.DeclaredValueForCustoms;
                    customized.ValuationCharge = mawbCustomizefield.ValuationCharge;
                    customized.Tax = mawbCustomizefield.Tax;
                    customized.TotalOtherChargesDueAgent = mawbCustomizefield.TotalOtherChargesDueAgent;
                    customized.TotalOtherChargesDueCarrier = mawbCustomizefield.TotalOtherChargesDueCarrier;
                    customized.OtherCharges = mawbCustomizefield.OtherCharges;
                    customized.ChargesAtDestination = mawbCustomizefield.ChargesAtDestination;
                    customized.TotalCollectCharges = mawbCustomizefield.TotalCollectCharges;
                    customized.CurrencyConversionRates = mawbCustomizefield.CurrencyConversionRates;
                    customized.TotalPrepaid = mawbCustomizefield.TotalPrepaid;
                    customized.TotalCollect = mawbCustomizefield.TotalCollect;
                    customized.HandlingInformation = mawbCustomizefield.HandlingInformation;
                    customized.AgentsIATACode = mawbCustomizefield.AgentsIATACode;
                    customized.AccountNo = mawbCustomizefield.AccountNo;
                    dbContext.MAWBCustomizeds.Add(customized);
                    dbContext.SaveChanges();
                }

                return true;
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessage = ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage;
                var propertyName = ex.EntityValidationErrors.First().ValidationErrors.First().PropertyName;
                return false;
            }
        }

        public MAWBCustomizedFiled GetMawbCustomizePdf(int TradelaneShipmentId)
        {
            MAWBCustomizedFiled field = new MAWBCustomizedFiled();

            var detail = dbContext.MAWBCustomizeds.Where(x=>x.TradelaneShipmentId == TradelaneShipmentId).FirstOrDefault();
            if(detail != null)
            {
                field.MAWBCustomizedeFieldId = detail.MAWBCustomizedeFieldID;
                field.TradelaneShipmentId = detail.TradelaneShipmentId ?? 0;
                field.IssuingCarriersAgentNameandCity = detail.IssuingCarriersAgentNameandCity;
                field.DeclaredValueForCarriage = detail.DeclaredValueForCarriage;
                field.DeclaredValueForCustoms = detail.DeclaredValueForCustoms;
                field.ValuationCharge = detail.ValuationCharge;
                field.Tax = detail.Tax;
                field.TotalOtherChargesDueAgent = detail.TotalOtherChargesDueAgent;
                field.TotalOtherChargesDueCarrier = detail.TotalOtherChargesDueCarrier;
                field.OtherCharges = detail.OtherCharges;
                field.ChargesAtDestination = detail.ChargesAtDestination;
                field.TotalCollectCharges = detail.TotalCollectCharges;
                field.CurrencyConversionRates = detail.CurrencyConversionRates;
                field.TotalPrepaid = detail.TotalPrepaid;
                field.TotalCollect = detail.TotalCollect;
                field.HandlingInformation = detail.HandlingInformation;
                field.AgentsIATACode = detail.AgentsIATACode;
                field.AccountNo = detail.AccountNo;
            }
            return field;
        }

        #endregion
    }
}
