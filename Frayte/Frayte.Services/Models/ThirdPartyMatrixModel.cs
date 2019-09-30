using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteThirdPartyMatrix
    {
        public int ThirdPartyMatrixId { get; set; }
        public FrayteZone FromZone { get; set; }
        public FrayteZone ToZone { get; set; }
        public FrayteZone ApplyZone { get; set; }
        public FrayteOperationZone OperationZone { get; set; }
    }

    public class FrayteRateThirdPartyMatrix
    {
        public int FromLogisticServiceZoneId { get; set; }
        public string  FromZoneDisplay { get; set; }
        public int ToLogisticServiceZoneId { get; set; }
        public string ToZoneDisplay { get; set; }
        public int ApplyLogisticServiceZoneId { get; set; }
        public string ApplyZoneDisplay { get; set; }
    }

    public class FrayteZoneMatrix
    {
        public string FromZone { get; set; }
        public List<FrayteApplyZone> ApplyZone { get; set; }
    }

    public class FrayteApplyZone
    {
        public string Zone1 { get; set; }
        public string TPZone1 { get; set; }
        public string Zone2 { get; set; }
        public string TPZone2 { get; set; }
        public string Zone3 { get; set; }
        public string TPZone3 { get; set; }
        public string Zone4 { get; set; }
        public string TPZone4 { get; set; }
        public string Zone5 { get; set; }
        public string TPZone5 { get; set; }
        public string Zone6 { get; set; }
        public string TPZone6 { get; set; }
        public string Zone7 { get; set; }
        public string TPZone7 { get; set; }
        public string Zone8 { get; set; }
        public string TPZone8 { get; set; }
        public string Zone9 { get; set; }
        public string TPZone9 { get; set; }
        public string Zone10 { get; set; }
        public string TPZone10 { get; set; }
        public string Zone11 { get; set; }
        public string TPZone11 { get; set; }
        public string Zone12 { get; set; }
        public string TPZone12 { get; set; }
        public string Zone13 { get; set; }
        public string TPZone13 { get; set; }
    }
}
