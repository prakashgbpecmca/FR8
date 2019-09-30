//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Frayte.Services.Models;
//using com.esendex.sdk.contacts;
//using com.esendex.sdk.inbox;
//using com.esendex.sdk.messaging;
//using com.esendex.sdk.sent;
//using com.esendex.sdk.session;
//using com.esendex.sdk;
//using System.Net;
//using Frayte.Services.DataAccess;



//namespace Frayte.Services.Business
//{
//    public class CountrySMSRepository
//    {
//        FrayteEntities dbContext = new FrayteEntities();

//        #region SendTextSMS
//        public FrayteResult SendTextSMS(string countryCode, string receipent, string message)
//        {
//            FrayteResult result = new FrayteResult();
//            try
//            {
//                if (string.IsNullOrEmpty(countryCode))
//                    throw new Exception("Country Code must be given.");

//                if (string.IsNullOrEmpty(receipent))
//                    throw new Exception("Receipent Code must be given.");

//                if (string.IsNullOrEmpty(message))
//                    throw new Exception("Message must be given.");

//                var SMSInfo = dbContext.CountrySMS.Where(p => p.CountryCode == countryCode).FirstOrDefault();

//                if (SMSInfo != null)
//                {

//                    switch (SMSInfo.CountryCode)
//                    {
//                        case SMSCountries.UK:
//                            if (string.IsNullOrEmpty(SMSInfo.Username) || string.IsNullOrEmpty(SMSInfo.Password) || string.IsNullOrEmpty(SMSInfo.AccountRef))
//                                throw new Exception("username, password and account reference must be given.");

//                            result = SendMessageUK(SMSInfo.Username, SMSInfo.Password, SMSInfo.AccountRef, receipent, message);
//                            break;
//                        default:
//                            throw new Exception("Could not sent message for " + SMSInfo.CountryCode);
//                    }
//                }
//                else
//                {
//                    throw new Exception("Could not sent message for " + SMSInfo.CountryCode);
//                }
//                return result;

//            }
//            catch (Exception ex)
//            {
//                dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
//                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));
//                result.Status = false;
//                return result;
//            }


//        }
//        #region UK SMS 
//        private FrayteResult SendMessageUK(string username, string password, string accountRef, string sendTo, string message)
//        {
//            FrayteResult result = new FrayteResult();

//            try
//            {
//                EsendexCredentials credentials = new EsendexCredentials(username, password);

//                // set originator in sms 


//                var Message = new SmsMessage(sendTo, message, accountRef);
//                var messagingService = new MessagingService(true, credentials);
//                //     messagingService.SendMessage(new SmsMessage("07811347389", "Your message", "EX00001") { Originator = "FromIsHere" });
//                var messageResult = messagingService.SendMessage(Message);
//                result.Status = true;
//                // can stote batch id and message id for retrieving status of message
//                return result;
//            }
//            catch (Exception ex)
//            {
//                dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
//                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));
//                result.Status = false;
//                return result;
//            }

//        }
//        #endregion
//        #endregion



//        #region Prepare TextSMS

//        public string FormatTextSMS(string actionType, FrayteCommerceShipmentDraft eCommerceBookingDetail)
//        {
//            string message = string.Empty;
//            try
//            {

//                string url = string.Empty;
//                string courierCompany = string.Empty;

//                var courier = dbContext.CountryLogistics.Where(p => p.CountryId == eCommerceBookingDetail.ShipTo.Country.CountryId).FirstOrDefault();
//                if (courier != null)
//                {
//                    courierCompany = courier.LogisticService;
//                }
//                else
//                {
//                    courierCompany = FrayteCourierCompany.DHLExpress;
//                }
//                var opz = UtilityRepository.GetOperationZone();
//                if (opz.OperationZoneName == eCommerceBusinessZone.HKG)
//                {
//                    url = SiteURL.SiteHKG;
//                }
//                else if (opz.OperationZoneName == eCommerceBusinessZone.UK)
//                {
//                    url = SiteURL.SiteUK;
//                }

//                var info = dbContext.NotificationSMS.Where(p => p.Action == actionType).FirstOrDefault();
//                if (info != null && !string.IsNullOrEmpty(info.Description))
//                {
//                    if (info.Description.Contains(SMSShipmentNotification.FirstName))
//                        info.Description = info.Description.Replace(SMSShipmentNotification.FirstName, eCommerceBookingDetail.ShipTo.FirstName);

//                    if (info.Description.Contains(SMSShipmentNotification.LastName))
//                        info.Description = info.Description.Replace(SMSShipmentNotification.LastName, eCommerceBookingDetail.ShipTo.LastName);

//                    if (info.Description.Contains(SMSShipmentNotification.Email))
//                        info.Description = info.Description.Replace(SMSShipmentNotification.Email, eCommerceBookingDetail.ShipTo.Email);

//                    if (info.Description.Contains(SMSShipmentNotification.FrayteNumber))
//                        info.Description = info.Description.Replace(SMSShipmentNotification.FrayteNumber, eCommerceBookingDetail.FrayteNumber);

//                    if (info.Description.Contains(SMSShipmentNotification.TrackingNumber))
//                        info.Description = info.Description.Replace(SMSShipmentNotification.TrackingNumber, eCommerceBookingDetail.Packages[0].TrackingNo);

//                    if (info.Description.Contains(SMSShipmentNotification.URL))
//                        info.Description = info.Description.Replace(SMSShipmentNotification.URL, url);

//                    if (info.Description.Contains(SMSShipmentNotification.CourierCompany))
//                        info.Description = info.Description.Replace(SMSShipmentNotification.CourierCompany, courierCompany);

//                    message = info.Description;

//                }

//            }
//            catch (Exception ex)
//            {
//                message = "";
//            }
//            return message;
//        }
//        #endregion
//    }
//}
