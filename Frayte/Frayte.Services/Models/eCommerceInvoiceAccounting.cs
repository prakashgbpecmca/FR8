using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class eCommerceInvoiceAccounting
    {
        public int MyProperty { get; set; }
        public eCommerceTaxAndDutyInvoiceReport InvoiceReport { get; set; }
        public eCommerceFile InvoiceFile { get; set; }
        public List<eCommerceCreditNote> UserCreditNote { get; set; }
        public List<eCommerceFile> Attachments { get; set; }
    }

    public class eCommerceFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
    public class eCommerceCreditNote
    {
        public int eCommerceUserCreditNoteId { get; set; }
        public string CreditNoteRef { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime IssuedDate { get; set; }
        public string IssuedTime { get; set; }
        public string IssuedBy { get; set; }
        public string Status { get; set; }
        public string IssuedTo { get; set; }
    }

    public class InVoiceCommunication
    {
        public int ShipmentId { get; set; }
        public int CreatedBy { get; set; }
        public string Description { get; set; }
        public string AccessType { get; set; }
    }

    public class eCommerceInvoiceCommunication
    {
        public int eCommerceCommunicationId { get; set; }
        public int ShipmentId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public TimeSpan CreatedOnTime { get; set; }
        public string CreatedTime { get; set; }
        public string CreatedRole { get; set; }
        public string CreatedRoleDisplay { get; set; }
        public string Description { get; set; }
        public string AccessType { get; set; }

    }

    public class InvoiceEmailCommunication
    {
        public int eCommerceEmailCommunicationId { get; set; }
        public int ShipmentId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime EmailSentOnDate { get; set; }
        public TimeSpan EmailSentOnTime { get; set; }
        public string EmailSentTime { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string SentTo { get; set; }
    }
    public class UserEmailCommunication
    {
        public int eCommerceEmailCommunicationId { get; set; }
        public int ShipmentId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime EmailSentOnDate { get; set; }
        public TimeSpan EmailSentOnTime { get; set; }
        public string EmailSentTime { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string SentTo { get; set; }
    }
    public static class CommunicationAccessType
    {
        public const string Public = "Public";
        public const string Private = "Private";
    }

    public static class eCommerceInvoiceTermsOfPayment
    {
        public const string SevenDays = "7 Days";
        public const string FourteenDays = "14 Days";
        public const string TwentyOneDays = "21 Days";
        public const string ThirtyDays = "30 Days";
        public const string SixtyDays = "60 Days";
        public const string PrePayment = "Pre-Payment";
        public const string PaymentTermFreight = "Terms of Payment for freight";
    }

    public class eCommerceInvoiceCreditNote
    {
        public int eCommerceUserCreditNoteId { get; set; }
        public int ShipmentId { get; set; }
        public string CreditNoteReference { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public int IssuedBy { get; set; }
        public DateTime IssuedOnUtc { get; set; }
        public DateTime UsedOnUtc { get; set; }
        public string IssuedTo { get; set; }
        public string Status { get; set; }
        public int eCommreceInvoiceId { get; set; }
    }
}
