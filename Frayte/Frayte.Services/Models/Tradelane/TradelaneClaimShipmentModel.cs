using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Tradelane
{
    public class TradelaneClaimShipmentModel
    {
        public int TradelaneShipmentId { get; set; }
        public int AgentId { get; set; }
        public string ToEmail { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }


    }

    public class TradelaneAgentEmailModel
    {
       
        public string Agent { get; set; }
        
        public string staff { get; set; }

        public string AgentName { get; set;}


    }
}
