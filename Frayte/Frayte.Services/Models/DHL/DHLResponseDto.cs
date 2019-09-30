using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Frayte.Services.Models.DHL
{

    public class DHLResponseDto
    {
        public string  ShipmentImange { get; set; }
        public string ServiceResponse { get; set; }
        public string PickupRef { get; set; }
        public List<Piece> Pieces { set; get; }
        public string ImageString { get; set; }
        public bool Status { get; set; }
        public string DHLOrderId { get; set; }
        public DHLError Error { get; set; }
    }

    public class Piece
    {
        public string PieceNumber { get; set; }
        public string Depth { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string DimWeight { get; set; }
        public string DataIdentifier { get; set; }
        public string LicensePlate { get; set; }
        public string LicensePlateBarCode { get; set; }
    }

    public class DHLError
    {
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }
}
