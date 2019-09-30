using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteCourierAccount
    {
        public int LogisticServiceCourierAccountId { get; set; }
        public FrayteOperationZone OperationZone { get; set; }
        public LogisticShipmentService LogisticService { get; set; }        
        public string IntegrationAccountId { get; set; }
        public string AccountNo { get; set; }
        public string AccountCountryCode { get; set; }
        public string Description { get; set; }
        public string ColorCode { get; set; }
        public bool IsActive { get; set; }
    }
}
