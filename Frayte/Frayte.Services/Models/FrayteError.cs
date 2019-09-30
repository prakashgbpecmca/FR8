using Frayte.Services.Models.ApiModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteGeneralError
    {
        public string type { get; set; }
        public string carrier { get; set; }
        public string message { get; set; }
    }

    public class Error
    {
        public string code { get; set; }
        public string message { get; set; }
        public IList<object> errors { get; set; }
    }

    public class FrayteEasyPostError
    {
        public Error error { get; set; }
    }

    public class FratyteError
    {
        public List<string> Address { get; set; }
        public List<string> Package { get; set; }
        public List<string> Custom { get; set; }
        public List<string> Service { get; set; }
        public List<string> ServiceError { get; set; }
        public List<FrayteKeyValue> MiscErrors { get; set; }
        public List<string> Miscellaneous { get; set; }
        public bool Status { get; set; }
        public bool IsMailSend { get; set; }
        public string TNTResponse { get; set; }
    }

    public class FrayteKeyValue
    {
        public string Key { get; set; }
        public List<string> Value { get; set; }
    }

    public class FrayteDirectShipment
    {
        public int DirectShipmentId { get; set; }
        public int FromAddressId { get; set; }
        public int ToAddressId { get; set; }
        public int ShipmentStatusId { get; set; }
    }

    public class FrayteDirectBookingDetailError
    {
        public FratyteError Error { get; set; }
        public DirectBookingShipmentDraftDetail BookingDetail { get; set; }
        public decimal GrossWeight { get; set; }
        public string ShipmentType { get; set; }
        public List<FrayteApiError> ErrorCode { get; set; }
    }
}
