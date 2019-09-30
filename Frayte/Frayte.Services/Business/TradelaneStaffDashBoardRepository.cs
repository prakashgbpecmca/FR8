using Frayte.Services.Models.Tradelane;
using Frayte.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Business
{
    public class TradelaneStaffDashBoardRepository
    {
        FrayteEntities dbContext = new FrayteEntities();
        public TradelaneStaffDashBoardModel StaffDashBoardInitialDetail()
        {
            TradelaneStaffDashBoardModel TSD = new TradelaneStaffDashBoardModel();
            TSD.PreAlertCount = 0;
            TSD.UnallocatedShipmentCount = 0;
            var PreAlertCount = dbContext.TradelaneShipments.Where(a => a.IsAgentMAWBAllocated == true && a.ShipmentStatusId == 28).ToList();
            if(PreAlertCount != null && PreAlertCount.Count > 0)
            {
                TSD.PreAlertCount = PreAlertCount.Count;
            }
            var UnallocatedShipments = dbContext.TradelaneShipments.Where(a => (a.MAWBAgentId == null || a.MAWBAgentId.Value == 0) && a.ShipmentStatusId == 28).ToList();
            if(UnallocatedShipments.Count > 0)
            {
                TSD.UnallocatedShipmentCount = UnallocatedShipments.Count;
            }

            var MilestoneNotCompleted = dbContext.TradelaneShipments.Where(a => (a.MAWBAgentId == null || a.MAWBAgentId == 0) &&  (a.ShipmentStatusId == 28 || a.ShipmentStatusId == 29 || a.ShipmentStatusId == 30 || a.ShipmentStatusId == 31 || a.ShipmentStatusId == 32 || a.ShipmentStatusId == 34)).ToList();
            if (MilestoneNotCompleted.Count > 0)
            {
                TSD.MilestoneNotCompletedCount = MilestoneNotCompleted.Count;
            }
            return TSD;
        }

    }
}
