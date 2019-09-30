using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;

namespace Frayte.Services.Business
{
    public class OperationZoneRepository
    {
        FrayteEntities dbContext = new FrayteEntities();
        public List<OperationZoneModel> GetOperationZone()
        {
            List<OperationZoneModel> _operation = new List<OperationZoneModel>();
            var operation = dbContext.OperationZones.ToList();
            OperationZoneModel fz;
            foreach (var rr in operation)
            {
                fz = new OperationZoneModel();
                fz.OperationZoneId = rr.OperationZoneId;
                fz.OperationZoneName = rr.Name;
                _operation.Add(fz);
            }
            return _operation;
        }
    }
}
