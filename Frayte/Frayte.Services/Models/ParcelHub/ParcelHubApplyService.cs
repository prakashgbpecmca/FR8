using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.ParcelHub
{
    public class ParcelHubApplyService
    {
        public int ServiceCustomerUid { get; set; }
        public int DesiredServiceId { get; set; }
        public int DesiredServiceProviderId { get; set; }
    }
}
