using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frayte.Api.Models
{
    public class FrayteApiErrorDto
    {
        public int ErrorCode { get; set; }
        public string Description { get; set; }
    }
}