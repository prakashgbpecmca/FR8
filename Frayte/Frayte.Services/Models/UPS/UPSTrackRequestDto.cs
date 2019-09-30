using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class UPSTrackRequestDto
    {
        public SecurityDto Security { get; set; }
        public TrackRequestDto TrackRequest { get; set; }
    }
    public class SecurityDto
    {
        public UsernameTokenDto UsernameToken { get; set; }
        public ServiceAccessTokenDto UPSServiceAccessToken { get; set; }
    }
    public class TrackRequestDto
    {
        public RequestDto Request { get; set; }
        public string InquiryNumber { get; set; }
        public string TrackingOption { get; set; }
    }
}
