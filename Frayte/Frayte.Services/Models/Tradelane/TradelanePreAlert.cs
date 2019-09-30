using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Tradelane
{
    public class TradelanePreAlertInitial
    {
        public int UserId { get; set; }
        public int TradelaneShipmentId { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string StaffUserName { get; set; }
        public string StaffUserPosition { get; set; }
        public string SiteCompany { get; set; }
        public string StaffUserEmail { get; set; }
        public string StaffUserPhone { get; set; }
        public string SiteAddress { get; set; }
        public TradelaneShipmentEmailUser ShipmentEmail { get; set; }
        public List<PreAlertEmail> PreAlerEmailTo { get; set; }
        public List<PreAlertEmail> PreAlerEmailCC { get; set; }
        public List<PreAlertEmail> PreAlerEmailBCC { get; set; }
        public List<TradelanePreAlertDocument> PreAlertDocumnets { get; set; }
        public string AdditionalInfo { get; set; }
    }

    public class TradelanePreAlertDocument
    {
        public string DocumentType { get; set; }
        public string DocumentTypeDisplay { get; set; }
        public int OrderNumber { get; set; }
        public List<TradelanePreAlertDoc> Documents { get; set; }
    }
    public class TradelanePreAlertDoc
    {
        public int TradelaneShipmentDocumentId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool IsSelected { get; set; }
        public string Document { get; set; }
        public int RevNumber { get; set; }
        public int OrderNumber { get; set; }
    }

    public class TradelaneShipmentEmailUser
    {
        public bool IsCustomerSentEmail { get; set; }
        public string CustomerEmail { get; set; }
        public bool IsShipperSentEmail { get; set; }
        public string ShipperEmail { get; set; }
        public bool IsReceiverSentEmail { get; set; }
        public string ReceiverEmail { get; set; }
        public bool IsNotifyPartySentEmail { get; set; }
        public string NotifyPartyEmail { get; set; }
        public bool IsAgentSentEmail { get; set; }
        public string AgentEmail { get; set; }
    }

    public class PreAlertEmail
    {
        public int TradelaneUserTrackingConfigurationDetailId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

}
