using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Tradelane
{
    public class TradelaneStaffDashBoardModel
    {

        public int PreAlertCount { get; set; }

        public int UnallocatedShipmentCount { get; set; }

        public int MilestoneNotCompletedCount { get; set; }

    }
}
