using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;
using RazorEngine.Templating;
using System.IO;
using Frayte.Services.Utility;

namespace Frayte.Services.Business
{
    public class PaymentRepository
    {
        FrayteEntities dbContext = new FrayteEntities();
        public List<PaymentCreditNote> GetUserCreditNoteByEmail(string email)
        {
            List<PaymentCreditNote> list = new List<PaymentCreditNote>();
            try
            {
                if (!string.IsNullOrEmpty(email))
                {
                    PaymentCreditNote creditNote;
                    var collection = dbContext.eCommerceUserCreditNotes.Where(p => p.IssuedTo == email).ToList();
                    if (collection.Count > 0)
                    {
                        foreach (var item in collection)
                        {
                            creditNote = new PaymentCreditNote();
                            creditNote.Amount = Math.Round(item.Amount.HasValue ? item.Amount.Value : 0.0M, 2);
                            creditNote.CreditNoteRef = item.CreditNoteReference;
                            creditNote.CurrencyCode = item.CurrencyCode;
                            creditNote.eCommerceUserCreditNoteId = item.eCommerceUserCreditNoteId;
                            list.Add(creditNote);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return list;
        }
        public List<eCommerceCreditNote> GetUserCreditNoteById(int shipmentId)
        {
            List<eCommerceCreditNote> list = new List<eCommerceCreditNote>();
            try
            {

                var email = (from detail in dbContext.eCommerceShipments
                             join da in dbContext.eCommerceShipmentAddresses on detail.ToAddressId equals da.eCommerceShipmentAddressId
                             where detail.eCommerceShipmentId == shipmentId
                             select da.Email
                             ).FirstOrDefault();

                if (!string.IsNullOrEmpty(email))
                {
                    eCommerceCreditNote creditNote;
                    var collection = dbContext.eCommerceUserCreditNotes.Where(p => p.IssuedTo == email).ToList();
                    if (collection.Count > 0)
                    {
                        foreach (var item in collection)
                        {
                            creditNote = new eCommerceCreditNote();
                            creditNote.Amount = Math.Round(item.Amount.HasValue ? item.Amount.Value : 0.0M, 2);
                            creditNote.CreditNoteRef = item.CreditNoteReference;
                            creditNote.CurrencyCode = item.CurrencyCode;
                            creditNote.eCommerceUserCreditNoteId = item.eCommerceUserCreditNoteId;
                            creditNote.Status = item.Status;

                            list.Add(creditNote);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return list;
        }

        public ReceiverPaymentInfo GetReceiverPaymentInfo(string decriptString)
        {
            ReceiverPaymentInfo result = new ReceiverPaymentInfo();
            try
            {
                string frayteNumber = string.Empty;
                string email = string.Empty;
                string[] words = decriptString.Split('|');
                if (words != null && words.Length > 1)
                {
                    frayteNumber = words[0];
                    email = words[1];
                }

                result.ShipmentRef = frayteNumber;
                result.Email = email;
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public FrayteResult SavePaymentTransaction(OnlinePaymentModel item)
        {
            FrayteResult result = new FrayteResult();
            eCommerceInvoiceTransaction transaction = new eCommerceInvoiceTransaction();
            try
            {
                decimal amount = Math.Round(item.PaymentInfo.Pv_Amount, 2);
                if (item != null)
                {
                    eCommerceInvoiceTransaction eCommerceTransaction;
                    var invoice = dbContext.eCommerceInvoices.Where(p => p.InvoiceRef == item.PaymentInfo.Invoice_no).FirstOrDefault();

                    if (invoice != null)
                    {
                        var invoiceDetail = new eCommerceShipmentRepository().GeteCommerceInvoiceObj(invoice.ShipmentId);
                        if (invoice.Status == eCommerceAppTaxAndDutyStatus.TaxAndDutyPaid)
                        {
                            try
                            {
                                // Make entry in transaction table
                                eCommerceTransaction = new eCommerceInvoiceTransaction();
                                eCommerceTransaction.InvoiceId = invoice.InvoiceId;
                                eCommerceTransaction.PaidAmount = amount;
                                eCommerceTransaction.PaidBy = item.PaymentInfo.Email;
                                eCommerceTransaction.PaidOnUtc = DateTime.UtcNow;
                                eCommerceTransaction.PaymentMode = item.PaymentMode;
                                dbContext.eCommerceInvoiceTransactions.Add(eCommerceTransaction);
                                dbContext.SaveChanges();
                                result.Status = true;
                            }
                            catch (Exception ex)
                            {
                                result.Status = false;
                            }
                            try
                            {
                                eCommerceUserCreditNote creditNote = new eCommerceUserCreditNote();
                                creditNote.ShipmentId = invoice.ShipmentId;
                                creditNote.CreditNoteReference = invoice.InvoiceRef;
                                creditNote.IssuedOnUtc = DateTime.UtcNow;
                                creditNote.Amount = amount;
                                creditNote.CurrencyCode = item.PaymentInfo.Currency.CurrencyCode;
                                creditNote.eCommreceInvoiceId = invoice.InvoiceId;
                                creditNote.IssuedTo = item.PaymentInfo.Email;
                                creditNote.IssuedBy = 1;
                                creditNote.Status = "NotUsed";
                                dbContext.eCommerceUserCreditNotes.Add(creditNote);
                                dbContext.SaveChanges();
                                result.Status = true;
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        else if (invoice.Status == eCommerceAppTaxAndDutyStatus.TaxAndDutyPartiallyPaid)
                        {
                            var transactions = dbContext.eCommerceInvoiceTransactions.Where(p => p.InvoiceId == invoice.InvoiceId).ToList();
                            try
                            {
                                // Make entry in transaction table
                                eCommerceTransaction = new eCommerceInvoiceTransaction();
                                eCommerceTransaction.InvoiceId = invoice.InvoiceId;
                                eCommerceTransaction.PaidAmount = amount;
                                eCommerceTransaction.PaidBy = item.PaymentInfo.Email;
                                eCommerceTransaction.PaidOnUtc = DateTime.UtcNow;
                                eCommerceTransaction.PaymentMode = item.PaymentMode;
                                dbContext.eCommerceInvoiceTransactions.Add(eCommerceTransaction);
                                dbContext.SaveChanges();
                                result.Status = true;
                            }
                            catch (Exception ex)
                            {
                                result.Status = false;
                            }

                            decimal totalPaidAmountTemp = 0.00M;

                            if (transactions.Count > 0)
                            {
                                foreach (var trans in transactions)
                                {
                                    totalPaidAmountTemp += trans.PaidAmount;
                                }
                            }
                            amount = amount + Math.Round(totalPaidAmountTemp, 2);
                            if (invoiceDetail.NettCharge == amount)
                            {
                                invoice.Status = eCommerceAppTaxAndDutyStatus.TaxAndDutyPaid;
                            }
                            else if (invoiceDetail.NettCharge > amount)
                            {
                                invoice.Status = eCommerceAppTaxAndDutyStatus.TaxAndDutyPartiallyPaid;
                            }
                            else
                            {
                                invoice.Status = eCommerceAppTaxAndDutyStatus.TaxAndDutyPaid;
                                try
                                {
                                    eCommerceUserCreditNote creditNote = new eCommerceUserCreditNote();
                                    creditNote.ShipmentId = invoice.ShipmentId;
                                    creditNote.CreditNoteReference = invoiceDetail.InvoiceRef;
                                    creditNote.IssuedOnUtc = DateTime.UtcNow;
                                    creditNote.Amount = amount - invoiceDetail.NettCharge;
                                    creditNote.CurrencyCode = item.PaymentInfo.Currency.CurrencyCode;
                                    creditNote.eCommreceInvoiceId = invoice.InvoiceId;
                                    creditNote.IssuedTo = item.PaymentInfo.Email;
                                    creditNote.IssuedBy = 1;
                                    creditNote.Status = "NotUsed";
                                    dbContext.eCommerceUserCreditNotes.Add(creditNote);
                                    dbContext.SaveChanges();
                                    result.Status = true;
                                }
                                catch (Exception ex)
                                {
                                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                                }
                            }
                            dbContext.SaveChanges();
                            result.Status = true;
                        }
                        else if (invoice.Status == eCommerceAppTaxAndDutyStatus.TaxAndDutyUnPaid)
                        {
                            try
                            {
                                // Make entry in transaction table
                                eCommerceTransaction = new eCommerceInvoiceTransaction();
                                eCommerceTransaction.InvoiceId = invoice.InvoiceId;
                                eCommerceTransaction.PaidAmount = amount;
                                eCommerceTransaction.PaidBy = item.PaymentInfo.Email;
                                eCommerceTransaction.PaidOnUtc = DateTime.UtcNow;
                                eCommerceTransaction.PaymentMode = item.PaymentMode;
                                dbContext.eCommerceInvoiceTransactions.Add(eCommerceTransaction);
                                dbContext.SaveChanges();
                                result.Status = true;
                            }
                            catch (Exception ex)
                            {
                                result.Status = false;
                            }

                            if (invoiceDetail.NettCharge == amount)
                            {
                                invoice.Status = eCommerceAppTaxAndDutyStatus.TaxAndDutyPaid;
                            }
                            else if (invoiceDetail.NettCharge > amount)
                            {
                                invoice.Status = eCommerceAppTaxAndDutyStatus.TaxAndDutyPartiallyPaid;
                            }
                            else
                            {
                                invoice.Status = eCommerceAppTaxAndDutyStatus.TaxAndDutyPaid;
                                try
                                {
                                    eCommerceUserCreditNote creditNote = new eCommerceUserCreditNote();
                                    creditNote.ShipmentId = invoice.ShipmentId;
                                    creditNote.CreditNoteReference = invoiceDetail.InvoiceRef;
                                    creditNote.IssuedOnUtc = DateTime.UtcNow;
                                    creditNote.Amount = amount - invoiceDetail.NettCharge;
                                    creditNote.CurrencyCode = item.PaymentInfo.Currency.CurrencyCode;
                                    creditNote.eCommreceInvoiceId = invoice.InvoiceId;
                                    creditNote.IssuedTo = item.PaymentInfo.Email;
                                    creditNote.IssuedBy = 1;
                                    creditNote.Status = "NotUsed";
                                    dbContext.eCommerceUserCreditNotes.Add(creditNote);
                                    dbContext.SaveChanges();
                                    result.Status = true;
                                }
                                catch (Exception ex)
                                {
                                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                                }
                            }
                            dbContext.SaveChanges();
                            result.Status = true;


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return result;
        }

        public eCommerceTaxAndDutyInvoiceReport GetShipmentInvoice(string frayteNumber)
        {
            eCommerceTaxAndDutyInvoiceReport InvoiceReport = new eCommerceTaxAndDutyInvoiceReport();
            var shipment = dbContext.eCommerceShipments.Where(p => p.FrayteNumber == frayteNumber).FirstOrDefault();
            if (shipment != null)
            {
                InvoiceReport = new eCommerceShipmentRepository().GeteCommerceInvoiceObj(shipment.eCommerceShipmentId);
                // Fill extra data in invoice object
                new eCommerceShipmentRepository().GeteCommerceInvoceData(shipment.eCommerceShipmentId, InvoiceReport);
            }
            return InvoiceReport;
        }

        public void SendPaymentEmail(OnlinePaymentModel item, string TransactionNo, string sendFrom, string Status)
        {
            if (item != null && item.PaymentInfo != null)
            {
                // var date = DateTime.SpecifyKind(DateTime.Now,;
                //  OnlinePayment obj = new OnlinePayment();
                DynamicViewBag viewBag = new DynamicViewBag();
                viewBag.AddValue("CustomerFirstName", item.PaymentInfo.FirstName);
                viewBag.AddValue("CustomerLastName", item.PaymentInfo.LastName);
                viewBag.AddValue("CustomerCompany", item.PaymentInfo.CompanyName);
                viewBag.AddValue("PaymentCompany", item.PaymentCompany);
                viewBag.AddValue("Date", item.PaymentInfo.PaymentDate);
                viewBag.AddValue("Time", item.PaymentInfo.PaymentTime);
                viewBag.AddValue("ShippingOrderNo", item.PaymentInfo.Invoice_no);
                viewBag.AddValue("PaymentNo", TransactionNo);
                viewBag.AddValue("Currency", item.PaymentInfo.Currency.CurrencyCode);
                viewBag.AddValue("Amount", Math.Round(item.Amount, 2));

                string toEmail = string.Empty;
                var EmailSubject = string.Empty;
                string toCC = string.Empty;
                if (item.PaymentCompany == OnlinePaymentCompany.Whytecllif)
                {
                    viewBag.AddValue("FromRegards", "WHTYTECLIFF Customer Service Team");
                    viewBag.AddValue("FromCompany", "WHYTECLIFF GROUP Ltd.");
                    viewBag.AddValue("FromSite", "www.WHYTECLIFF.com");
                    viewBag.AddValue("FromPhone", "(+852) 2148 4881");
                    viewBag.AddValue("FromEmail", "accounts@whytecliff.com");
                    //EmailSubject = "Payment Confirmation - WHTYTECLIFF Group Ltd.";
                    EmailSubject = "Payment Completed – Whytecliff Group";
                }
                else if (item.PaymentCompany == OnlinePaymentCompany.VytalSupport)
                {
                    viewBag.AddValue("FromRegards", "VYTAL SUPPORT Customer Service Team");
                    viewBag.AddValue("FromCompany", "VYTAL SUPPORT (Hong Kong) Ltd.");
                    viewBag.AddValue("FromSite", "www.VYTALSUPPORT.com");
                    viewBag.AddValue("FromPhone", "(+852) 2148 4881");
                    viewBag.AddValue("FromEmail", " accounts@vytalsupport.com");
                    //EmailSubject = "Payment Confirmation - VYTAL SUPPORT (Hong Kong) Co. Ltd.";
                    EmailSubject = "Payment Completed – Vytal Support (Hong Kong)";
                }
                else if (item.PaymentCompany == OnlinePaymentCompany.CliffPremus)
                {
                    viewBag.AddValue("FromRegards", "CLIFF PREMIUMS Customer Service Team");
                    viewBag.AddValue("FromCompany", "CLIFF PREMIUMS Ltd.");
                    viewBag.AddValue("FromSite", "www.CLIFFPREMIUMS.com");
                    viewBag.AddValue("FromPhone", "(+852) 2148 4881");
                    viewBag.AddValue("FromEmail", "accounts@cliffpremiums.com ");
                    EmailSubject = "Payment Completed – Cliff Premiums";
                }
                else if (item.PaymentCompany == OnlinePaymentCompany.FrayteCom)
                {
                    viewBag.AddValue("FromRegards", "FRAYTE Logistic Customer Service Team");
                    viewBag.AddValue("FromCompany", "FRAYTE GLOBAL");
                    viewBag.AddValue("FromSite", "www.FRAYTE.com");
                    viewBag.AddValue("FromPhone", "(+852) 2148 4880");
                    viewBag.AddValue("FromEmail", "accounts@frayte.com");
                    EmailSubject = "Payment Completed - FRAYTE GLOBAL";
                }
                else if (item.PaymentCompany == OnlinePaymentCompany.FrayteCoUk)
                {
                    viewBag.AddValue("FromRegards", "FRAYTE Logistic Customer Service Team");
                    viewBag.AddValue("FromCompany", "FRAYTE GLOBAL");
                    viewBag.AddValue("FromSite", "www.FRAYTE.co.uk");
                    viewBag.AddValue("FromPhone", "(+44) 01792 277295");
                    viewBag.AddValue("FromEmail", "accounts@frayte.co.uk");
                    EmailSubject = "Payment Completed - FRAYTE GLOBAL";
                }
                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTemplate/Payment.cshtml");
                var templateService = new TemplateService();

                var EmailBody = templateService.Parse(template, "", viewBag, null);

                var Issend = false;
                if (Status == OnlinePaymentStatus.Success)
                {
                    //Issend = new  ShipmentEmailRepository().SendMail(toEmail, EmailSubject, EmailBody, "", "", item.PaymentCompany);

                }
                if (Issend == true)
                {
                    // obj.IsEmailSent = true;
                    //   dbContext.OnlinePayments.Add(obj);
                    dbContext.SaveChanges();
                }
                else
                {
                    //  obj.IsEmailSent = false;
                    //dbContext.OnlinePayments.Add(obj);
                    dbContext.SaveChanges();
                }
            }
        }

        public PaymentReceiver GetReceiverDetailByEmail(string email)
        {

            PaymentReceiver receiver = new PaymentReceiver();
            var detail = (from r in dbContext.eCommerceShipmentAddresses
                          join c in dbContext.Countries on r.CountryId equals c.CountryId
                          where r.Email == email
                          select new
                          {
                              eCommerceShipmentAddressId = r.eCommerceShipmentAddressId,
                              Email = r.Email,
                              Address1 = r.Address1,
                              Address2 = r.Address2,
                              Area = r.Area,
                              City = r.City,
                              CompanyName = r.CompanyName,
                              ContactFirstName = r.ContactFirstName,
                              ContactLastName = r.ContactLastName,
                              PhoneNo = r.PhoneNo,
                              State = r.State,
                              ZipCode = r.Zip,
                              CountryCode = c.CountryCode,
                              CountryCode2 = c.CountryCode2,
                              CountryId = c.CountryId,
                              CountryName = c.CountryName,
                              CountryPhoneCode = c.CountryPhoneCode

                          }
             ).FirstOrDefault();


            if (detail != null)
            {
                receiver.eCommerceShipmentAddressId = detail.eCommerceShipmentAddressId;
                receiver.Email = detail.Email;
                receiver.Address1 = detail.Address1;
                receiver.Address2 = detail.Address2;
                receiver.Area = detail.Area;
                receiver.City = detail.City;
                receiver.CompanyName = detail.CompanyName;
                receiver.ContactFirstName = detail.ContactFirstName;
                receiver.ContactLastName = detail.ContactLastName;
                receiver.PhoneNo = detail.PhoneNo;
                receiver.State = detail.State;
                receiver.ZipCode = detail.ZipCode;
                receiver.Country = new Country();
                receiver.Country.CountryCode = detail.CountryCode;
                receiver.Country.CountryCode2 = detail.CountryCode2;
                receiver.Country.CountryId = detail.CountryId;
                receiver.Country.CountryName = detail.CountryName;
                receiver.Country.CountryPhoneCode = detail.CountryPhoneCode;
            }
            return receiver;
        }
    }
}
