using Frayte.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    #region Payment
    public class OnlinePaymentModel
    {
        public PaymentInfo PaymentInfo { get; set; }
        public string CardNo { get; set; }
        public string ReasonOfFailure { get; set; }
        public string ExpiryDate { get; set; }
        public string CVC { get; set; }
        public string EmailId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public string PaymentCompany { get; set; }
        public string PaymentMode { get; set; }
        public string BalanceTransactionId { get; set; }
        public string Status { get; set; }
    }

    public class PaymentInfo
    {
        public FrayteCountryCode selectedcountry { get; set; }
        public decimal Pv_Amount { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string CardNumber { get; set; }
        public string cvc { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentTime { get; set; }
        public string ExpDate { get; set; }
        public string Invoice_no { get; set; }
        public FrayteCountryCode Country { get; set; }
        public PaymentCurrency Currency { get; set; }
        public decimal Amout { get; set; }
    }

    public class PaymentCurrency
    {
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
    }
    public static class OnlinePaymentCompany
    {
        public const string VytalSupport = "VTS-HK";
        public const string Whytecllif = "WCG-HK";
        public const string CliffPremus = "CPL-HK";
        public const string FrayteCom = "FRT-HK";
        public const string FrayteCoUk = "FRT-UK";
    }
    public static class OnlinePaymentStatus
    {
        public const string Success = "Succeed";
        public const string Failure = "Failed";

    }

    public class PaymentSentMail
    {
        public string host { get; set; }
        public int port { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string bccEmail { get; set; }
        public string fromMail { get; set; }
        public bool enableSsl { get; set; }

    }

    public class onlinePaymentGridModel
    {
        public string CompanyName { get; set; }
        public string CountryName { get; set; }
        public string CurrencyName { get; set; }
        public decimal CurrencyValue { get; set; }
        public decimal CalCurrrencyValue { get; set; }
        public string Invoice_no { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public DateTime createdDate { get; set; }
        public string payVaya { get; set; }
        public string PaymentCompanyName { get; set; }
        public bool IsEmail { get; set; }
    }
    public static class StripePaymentCompany
    {
        public const string VytalSupport = "VytalSupport";
        public const string Whytecllif = "Whytecliff";
        public const string CliffPremus = "CliffPremiums";
        public const string FrayteCoUk = "FrayteCoUk";
        public const string FrayteCom = "FrayteCom";
    }
    public class PaymentMode
    {
        public const string Live = "Live";
        public const string Test = "Test";
    }
    #endregion

    #region Credit Note

    public class PaymentCreditNote
    {
        public int eCommerceUserCreditNoteId { get; set; }
        public string CreditNoteRef { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } 
        public bool IsSelected { get; set; }
    }
    public class PaymentReceiver
    {
        public int eCommerceShipmentAddressId { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Address1 { get; set; }
        public string Area { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public Country Country { get; set; }
    }

    public class ReceiverPaymentInfo
    {
        public string Email { get; set; }
        public string ShipmentRef { get; set; } 
    }
    #endregion
}
