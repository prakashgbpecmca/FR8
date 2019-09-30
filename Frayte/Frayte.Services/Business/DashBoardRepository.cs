using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;


namespace Frayte.Services.Business
{
    public class DashBoardRepository
    {
        FrayteEntities dbContext = new FrayteEntities();
        
        public DashBoardModel GetDashBoardInitialDetail()
        {
            DashBoardModel DBM = new DashBoardModel();
            DBM.UnmanifestedShipmentCount = dbContext.Vw_GetUnmanifestedShipmentsCount.Count();
            //DBM.UnmappedHsCodeCount = dbContext.Vw_GetUnMapperHscodeCount.Count();
            DBM.UnmappedHsCodeCount = dbContext.Vw_GetUnMapperHscodeCount.GroupBy(a => a.eCommerceShipmentId).ToList().Count;
            return DBM;
        }
    }
}
