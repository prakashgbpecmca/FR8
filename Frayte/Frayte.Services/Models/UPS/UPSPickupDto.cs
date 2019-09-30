using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.UPS
{
    public class UPSPickupDto
    {
        public UPSSecurityDto UPSSecurity { get; set; }
        public PickupCreationRequestDto PickupCreationRequest { get; set; }

    }

    public class PickupCreationRequestDto
    {
        public Request Request { get; set; }

        public string RatePickupIndicator { get; set; }
        public string TaxInformationIndicator { get; set; }
        public PickupDateInfoDto PickupDateInfo { get; set; }
        public PickupAddressDto PickupAddress { get; set; }
        public string AlternateAddressIndicator { get; set; }
        public PickupPieceDto PickupPiece { get; set; }
        public string OverweightIndicator { get; set; }
        public string PaymentMethod { get; set; }
    }
    public class PickupDateInfoDto
    {
        public string CloseTime { get; set; }
        public string ReadyTime { get; set; }
        public string PickupDate { get; set; }
    }
    public class PickupAddressDto
    {
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string ResidentialIndicator { get; set; }
        public PhoneDto Phone { get; set; }

    }
    public class PickupPieceDto
    {
        public string ServiceCode { get; set; }
        public string Quantity { get; set; }
        public string DestinationCountryCode { get; set; }
        public string ContainerCode { get; set; }
    }
    public class Request
    {
        public TransactionReferenceDto TransactionReference { get; set; }
    }
}
