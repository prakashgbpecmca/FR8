using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Business;
using Frayte.Services.DataAccess;

namespace Frayte.Services.Models
{
    public class FryateReportSetting
    {
        public int CustomerPODSettingId { get; set; }
        public int UserId { get; set; }
        public string PODScheduleSetting { get; set; }
        public string ScheduleType { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string ScheduleDay { get; set; }
        public TimeSpan ScheduleTime { get; set; }
        public string AdditionalMails { get; set; }
        public string ScheduleSettingType { get; set; }
        public DateTime CreatedOn { get; set; }
        public Nullable<DateTime> UpdatedOn { get; set; }
        public bool? IsPdf { get; set; }
        public bool? IsExcel { get; set; }
        public List<FryateUserDetail> fryateUserDetail { get; set; }
        public List<FryateUserSettingDetail> fryateUserSettingDetail { get; set; }
    }

    public class FryateUserDetail
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string CompanyName { get; set; }
        public int OperationZoneId { get; set;}
        public bool? IsFuelSurCharge { get; set; }
        public bool? IsCurrecy { get; set; }
    }

    public class FryateUserSettingDetail
    {
        public int CustomerLogisticId { get; set; }
        public FrayteLogisticServiceItem LogisticService { get; set; }
    }

    public class FrayteShipmentReport
    {
        public string Destination { get; set; }
        public string Country { get; set; }
        public string Booked_By { get; set; }
        public string Consignee { get; set; }
        public string DeliveryDate { get; set; }
        public string PODSign { get; set; }
    }

    public class FrayteShipmentReportPDF
    {
        public string PageStyleSheet { get; set; }
        public string BootStrapStyleSheet { get; set; }
        public string pdfLogo { get; set; }
        public List<FrayteShipmentReport> ShipmentReportDetail { get; set; }
    }
}
