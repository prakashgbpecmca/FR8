using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class TradelaneTrackingConfigurationModel
    {
        public List<TradelaneTrackingMileStoneModel> TrackingMileStone { get; set; }
        public TradelaneTrackingPreAlert PreAlert { get; set; }
    }
    public class TradelaneTrackingMileStoneModel
    {
        public int TradelaneUserTrackingConfigurationId { get; set; }
        public int TrackingMileStoneId { get; set; }
        public int UserId { get; set; }
        public bool IsEmailSend { get; set; }
        public List<TradelaneTrackingDetail> ConfigurationDetail { get; set; }

    }
    public class TradelaneTrackingPreAlert
    {
        public int TradelaneUserTrackingConfigurationId { get; set; }
        public string OtherMethod { get; set; }
        public int UserId { get; set; }
        public bool IsEmailSend { get; set; }
        public List<TradelaneTrackingDetail> ConfigurationDetail { get; set; }
    }

    public class TradelaneTrackingDetail
    {
        public int TradelaneUserTrackingConfigurationDetailId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public int UpdatedOn { get; set; }
    }
}
