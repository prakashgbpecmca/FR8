using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public static class FrayteABookingApp
    {
        public const string BULK = "Bulk";
        public const string API = "API";
    }
    public class OpeartorJob
    {
        public int OperatorId { get; set; }
        public List<FrayteUnAssignedJob> jobs { get; set; }
    }
    public class FrayteUnAssignedJob
    {
        public int ShipmentId { get; set; }
        public string Customer { get; set; }
        public string ShippedFromCompany { get; set; }
        public string ShippedToCompany { get; set; }
        public string FromCountry { get; set; }
        public string ToCountry { get; set; }
        public string ShipmentDescription { get; set; }
        public string CourierCompany { get; set; }
        public string CourierCompanyDisplay { get; set; }
        public DateTime ShippingDate { get; set; }
        public string Status { get; set; }
        public string DisplayStatus { get; set; }
        public string Reference1 { get; set; }
        public string FrayteNumber { get; set; }
        public DateTime? EstimatedDateOfDeparture { get; set; }
        public string EstimatedTimeOfDeparture { get; set; }
        public DateTime? EstimatedDateOfArrival { get; set; }
        public string EstimatedTimeOfArrival { get; set; }
        public string TrackingNo { get; set; }
        public int TotalRows { get; set; }
        public int ManifestId { get; set; }
        public string ManifestName { get; set; }
    }

    public class TrackHSCodeJob
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int CurrentPage { get; set; }
        public int TakeRows { get; set; }

    }
    public class TrackAssignedJob
    {
        public DateTime? FromDate { get; set; }
        public bool AllShipments { get; set; }
        public DateTime? ToDate { get; set; }
        public int CurrentPage { get; set; }
        public int TakeRows { get; set; }
        public int OperatorId { get; set; }
        public int DestinationCountry { get; set; }
    }
    public class JobDetail
    {
        public int ToltalUnAssignedJobs { get; set; }
        public int ToltalJobs { get; set; }
        public int AvgJobsPerHour { get; set; }
        public int ToltalOperators { get; set; }
        public int JobsInProgress { get; set; }
    }

    public class OperatorWithJobs
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public int JobsAssigned { get; set; }
    }
    public class MangerOperator
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
    }

    public class FrayteMappedJobs
    {
        public int ShipmentId { get; set; }
        public int OrderNumber { get; set; }
        public string Customer { get; set; }
        public string ShippedFromCompany { get; set; }
        public string ShippedToCompany { get; set; }
        public string FromCountry { get; set; }
        public string ToCountry { get; set; }
        public string ShipmentDescription { get; set; }
        public string CourierCompany { get; set; }
        public string CourierCompanyDisplay { get; set; }
        public DateTime ShippingDate { get; set; }
        public string Status { get; set; }
        public string DisplayStatus { get; set; }
        public string Reference1 { get; set; }
        public string FrayteNumber { get; set; }
        public string TrackingNo { get; set; }
        public int TotalRows { get; set; }
        public int ManifestId { get; set; }
        public string ManifestName { get; set; }
        public string StaffAssigned { get; set; }
        public string StaffEmail { get; set; }
        public string StaffCompanyName { get; set; }
        public DateTime? EstimatedDateOfDeparture { get; set; }
        public string EstimatedTimeOfDeparture { get; set; }
        public DateTime? EstimatedDateOfArrival { get; set; }
        public string EstimatedTimeOfArrival { get; set; }
        public List<eCommercePackage> Packages { get; set; }
    }

    public class OpeartorReAssignedJob
    {
        public int OperatorId { get; set; }
        public List<FrayteMappedJobs> jobs { get; set; }
    }
    public class JobsInProgressCount
    {
        public int CompletedJobs { get; set; }
        public int TotalJobs { get; set; }
    }

    public class HSCodeAvgOutput
    {
        public int OperatorId { get; set; }
        public string Name { get; set; }
        public int AvgJobs { get; set; }
    }
}
