using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class UPSErrorDto
    {
        public FaultDto Fault { get; set; }
    }
    public class FaultDto
    {
        public string faultcode { get; set; }
        public string faultstring { get; set; }
        public DetailDto detail { get; set; }
    }
    public class DetailDto
    {
        public ErrorsDto Errors { get; set; }
    }
    public class ErrorsDto
    {
        public ErrorDetailDto ErrorDetail { get; set; }
    }
    public class ErrorDetailDto
    {
        public string Severity { get; set; }
        public PrimaryErrorCode PrimaryErrorCode { get; set; }
    }
    public class PrimaryErrorCode
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}


