using Frayte.Services.Business;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    public class PaymentController : ApiController
    {
        #region Payment 

        [HttpGet]
        public List<eCommerceCreditNote> GetUserCreditNoteById(int shipmentId)
        {

            List<eCommerceCreditNote> list = new List<eCommerceCreditNote>();

            list = new PaymentRepository().GetUserCreditNoteById(shipmentId);


            return list;
        }

        [HttpGet]
        public List<PaymentCreditNote> GetUserCreditNoteByEmail(string email)
        {

            List<PaymentCreditNote> list = new List<PaymentCreditNote>();

            list = new PaymentRepository().GetUserCreditNoteByEmail(email);


            return list;
        }

        public IHttpActionResult CreatePayment()
        {
            FrayteResult result = new FrayteResult();
            //FrayteResult result = new PaymentRepository().CreatePayment();
            return Ok(result);
        }

        public string SendPaypalMail(OnlinePaymentModel item)
        {
            try
            {
                new PaymentRepository().SendPaymentEmail(item, item.BalanceTransactionId, "PAYPAL", item.Status);
                if (item.Status == OnlinePaymentStatus.Success)
                {
                    return "success";
                }
                else
                {
                    dynamic json1 = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json1));
                    return null;
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }

        }

        public IHttpActionResult SavePaymentTransaction(OnlinePaymentModel item)
        {
            FrayteResult result = new PaymentRepository().SavePaymentTransaction(item);
            return Ok(result);
        }

        #endregion

        [HttpGet]
        public ReceiverPaymentInfo GetPaymentInitials(string key)
        {
            try
            {
                string decriptString = UtilityRepository.ConvertHexToString(key, System.Text.Encoding.UTF8);
                ReceiverPaymentInfo result = new PaymentRepository().GetReceiverPaymentInfo(decriptString);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public PaymentReceiver GetReceiverDetailByEmail(string email)
        {
            PaymentReceiver receiver = new PaymentReceiver();
            try
            {
                receiver = new PaymentRepository().GetReceiverDetailByEmail(email);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return receiver;
        }

        [HttpGet]

        public eCommerceTaxAndDutyInvoiceReport GetShipmentInvoice(string frayteNumber)
        {
            eCommerceTaxAndDutyInvoiceReport invoiceDetail = new PaymentRepository().GetShipmentInvoice(frayteNumber);
            return invoiceDetail;
        }

        [HttpGet]
        public List<FrayteCountryCode> Countries()
        {
            return new CountryRepository().ListToCountry();
        }
    }
}
