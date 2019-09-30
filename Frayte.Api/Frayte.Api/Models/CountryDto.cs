using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frayte.Api.Models
{
    public class CountryDto
    {
      
        public int FromAddressContryId { get; set; }
        public string FromAddressContryName { get; set; }
        public string FromAddressContryCode { get; set; }
        public int ToAddressContryId { get; set; }
        public string ToAddressContryName { get; set; }
        public string ToAddressContryCode { get; set; }
    }
}