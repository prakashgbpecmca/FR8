using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frayte.Services.Models
{
    public class PayVisionSuccess
    {
        public string Pv_Result { get; set; }
        public string Pv_Message { get; set; }
        public System.Guid Pv_TrackingMemberCode { get; set; }
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
    }
}