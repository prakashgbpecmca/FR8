using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.UPS
{
    public class VoidShipmentResponseDto
    {
        public Response Response { get; set; }
        public SummaryResultDto SummaryResult { get; set; }
    }

    public class Response
    {
        public ResponseStatusDto ResponseStatus { get; set; }
        public TransactionReferenceDto TransactionReference { get; set; }
    }
    public class SummaryResultDto
    {
        public StatusDto Status { get; set; }
    }
}
