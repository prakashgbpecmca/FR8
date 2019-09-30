using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frayte.Services.Models
{
    public class LoginDetail
    {
        public int EmployeeId { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeMail { get; set; }
        public int EmployeeRoleId { get; set; } 
        public int EmployeeCustomerId { get; set; }
        public int EmployeeCustomerRoleId { get; set; }
        public string EmployeeCompanyLogo { get; set; }
        public string EmployeeCompanyName { get; set; }
        public EmployeeCustomerLogInDetail EmployeeCustomerDetail { get; set; }
        public string SessionId { get; set; }
        public string PhotoUrl { get; set; }
        public int OperationZoneId { get; set; }
        public string OperationZoneName { get; set; }
        public int UserOperationZoneId { get; set; }
        public bool IsLastLogin { get; set; }
        public bool IsRateShow  { get; set; }
        public int ValidDays { get; set; }
        public string CustomerCurrency { get; set; }
        public List<FrayteTab> frayteTabs { get; set; }
    }

    public class EmployeeCustomerLogInDetail
    {
        public int EmployeeCustomerId { get; set; }
        public int EmployeeCustomerRoleId { get; set; }
        public string EmployeeCustomerCompanyLogo { get; set; }
        public string EmployeeCustomerCompany { get; set; }
        public string EmployeeCustomerTrackingEmail { get; set; }
        public string CustomerService { get; set; }
    }

    public class FrayteLogin
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class RecoverEmailModel
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SalesEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string ImageHeader { get; set; }
        public string SiteAddress { get; set; }
        public string SiteLink { get; set; }
        public string RecoveryLink { get; set; }
    }
    public class NewUserModel
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SalesEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string ImageHeader { get; set; }
        public string SiteAddress { get; set; }
        public string SiteLink { get; set; }
        public string RecoveryLink { get; set; }
        public string DeptName { get; set; }
        public int RoleId { get; set; }
    }

    public class RecoveryEmail : FrayteSession
    {
        public string Email { get; set; }
    }
    public class RecoverPassword
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
    public class FrayteChangePassword
    {
        public string UserName { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
    public class FrayteChangeFirstPassword
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class FrayteTab
    {
        public int ModuleLevelId { get; set; }
        public int TabOrder { get; set; }
        public string heading { get; set; }
        public string route { get; set; }
        public string route1 { get; set; }
        public string route2 { get; set; }
        public string tabKey { get; set; }
        public bool active { get; set; }
        public List<ChildTab> childTabs { get; set; }
    }
    public class ChildTab
    {
        public int TabOrder { get; set; }
        public string heading { get; set; }
        public string tabKey { get; set; }
        public string route { get; set; }
        public bool IsDefaultRoute { get; set; }
    }
}