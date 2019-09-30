using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Tradelane
{
    public class AgentConsolidateMailModel
    {

        public List<TradelaneGetShipmentModel> TradelaneShipmentList { get; set; }
        public NewUserModel User { get; set; }
    }
}
