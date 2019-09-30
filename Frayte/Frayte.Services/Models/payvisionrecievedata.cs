using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frayte.Services.Models
{
    public class PayVisionPaymentDetail
    {
        public int Pv_CurrencyId { get; set; }
        public int Pv_CountryId { get; set; }
        public string Pv_SuccessResponseUrl { get; set; }
         public string Pv_ErrorResponseUrl { get; set; }
        public decimal Pv_Amount { get; set; }
      public List<PayVisionItem> Items { get; set; }
        ///database properties
      public string Pv_Result { get; set; }
      public string Pv_Token_key { get; set; }
      public string Pv_Message { get; set; }
      public string Pv_TransactionId { get; set; }
      public string Pv_TransactionGuid { get; set; }
      public string Pv_TransactionDateTime { get; set; }
      public string Pv_ErrorCode { get; set; }
      public string Pv_ErrorMessage { get; set; }
      public string Pv_BankApprovalCode { get; set; }
      public string Pv_BankCode { get; set; }
      public string Pv_BankMessage { get; set; }
      public string Pv_CardId { get; set; }
      public string Pv_CardGuid { get; set; }
      public string Pv_CardLastFourDigits { get; set; }
      public string Pv_CardExpiryMonth { get; set; }
      public string Pv_CardExpiryYear { get; set; }
      public string CompanyName { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public string Email { get; set; }
      public string Address1 { get; set; }
      public string Address2 { get; set; }
      public string City { get; set; }
      public string State { get; set; }
      public string ZipCode { get; set; }
      public string Invoice_no { get; set; }
      public string Country { get; set; }
      public string Currency { get; set; }
      public string Amout { get; set; }
      public string TotalPaid_Amount { get; set; }
      public string ClientPaymentSuccessUrl { get; set; }
      public string ClientPaymentErrorUrl { get; set; }
    }

    public class PayVisionItem
    {
        public string ItemCode { get; set; }
        public string Itemname { get; set; }
        public string Itemdescription { get; set; }
        public string quantity { get; set; }
        public decimal Itemprice { get; set; }
        public System.Guid Pv_TrackingMemberCode { get; set; }

    }
}