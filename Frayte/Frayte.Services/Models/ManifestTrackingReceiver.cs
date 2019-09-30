using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class ManifestTrackingReceiver
    {
        public int CustomerId { get; set; }

        public string ReceiverMail { get; set; }

        public string ReceiverName { get; set; }

        public List<ManifestReceiverTrackingNos> ReceiverTrackingNo { get; set; }
        public string TrackingDescription { get; set; }

    }

    public class ManifestReceiverTrackingNos
    {
        public string ReceiverTrackingNo { get; set; }
        public string TrackingUrl { get; set; }
    }
}
