using Frayte.Services.DataAccess;
using Frayte.Services.Models.Tradelane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Express
{
    public class ExpressTradelaneModel
    {

    }


    public class ExpressTradelaneIntegration
    {

        public IntegratedTradelaneShipment Shipment { get; set; }
        public List<MawbAllocationModel> MAWBList { get; set; }

    }

    public class IntegratedTradelaneShipment
    {
        public int TradelaneShipmentId { get; set; }
        public int OperationZoneId { get; set; }
        public HubDetailModel Hub { get; set; }
        public int CustomerId { get; set; }
        public int HAWBNumber { get; set; }
        public string CustomerAccountNumber { get; set; }
        public int ShipmentStatusId { get; set; }
        public List<IntegratedTradelanePackage> Packages { get; set; }
        public TradelBookingAdress ShipFrom { get; set; }
        public TradelBookingAdress ShipTo { get; set; }
        public string ShipperAdditionalNote { get; set; }
        public string ReceiverAdditionalNote { get; set; }
        public TradelBookingAdress NotifyParty { get; set; }
        public string NotifyPartyAdditionalNote { get; set; }
        public bool IsNotifyPartySameAsReceiver { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string CreatedOnTime { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
        public string FrayteNumber { get; set; }
        public string PakageCalculatonType { get; set; }
        public string LogisticType { get; set; }
        public TradelaneAirport DepartureAirport { get; set; }
        public TradelaneAirport DestinationAirport { get; set; }
        public TradelaneShipmentHandlerMethod ShipmentHandlerMethod { get; set; }
        public string ShipmentReference { get; set; }
        public string ShipmentDescription { get; set; }
        public decimal DeclaredValue { get; set; }
        public CurrencyType DeclaredCurrency { get; set; }
        public TradelaneAirline AirlinePreference { get; set; }
        public decimal TotalEstimatedWeight { get; set; }
        public decimal? DeclaredCustomValue { get; set; }
        public decimal? InsuranceAmount { get; set; }
        public string CertificateOfOrigin { get; set; }
        public string ExportLicenceNo { get; set; }
        public string PayTaxAndDuties { get; set; }
        public string TaxAndDutiesAccountNo { get; set; }
        public string TaxAndDutiesAcceptedBy { get; set; }
        public string CustomsSigner { get; set; }
        public bool? DangerousGoods { get; set; }
        public string MAWB { get; set; }
        public int? MAWBAgentId { get; set; }
        public string BatteryDeclarationType { get; set; }
        public string ManifestName { get; set; }
        public FratyteError Error { get; set; }
    }

    public class IntegratedTradelanePackage
    {
        public int TradelaneShipmentDetailId { get; set; }
        public int TradelaneShipmentId { get; set; }
        public string CartonNumber { get; set; }
        public int CartonValue { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string HAWB { get; set; }
        public string BagNumber { get; set; }
        public string Carrier { get; set; }
        public int NoOfPcs { get; set; }
        public int BagId { get; set; }
        public string MyProperty { get; set; }
    }

}
