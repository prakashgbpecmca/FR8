using Frayte.Security.IdentityManager;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Security.IdentityModels
{
    public class FrayteIdentityUser : IdentityUser<int, FrayteIdentityUserLogin, FrayteIdentityUserRole, FrayteIdentityUserClaim>
    {

        #region properties 
        public string UserEmail { get; set; }
        public int OperationZoneId { get; set; }
        public string CargoWiseId { get; set; }
        public string CargoWiseBardCode { get; set; }
        public string CompanyName { get; set; }
        public int? ClientId { get; set; }
        public bool? IsClient { get; set; }
        public string ContactName { get; set; }
        public string CountryOfOperation { get; set; }
        public string MobileNo { get; set; }
        public string TelephoneNo { get; set; }
        public string FaxNumber { get; set; }
        public TimeSpan? WorkingStartTime { get; set; }
        public TimeSpan? WorkingEndTime { get; set; }
        public int? TimezoneId { get; set; }
        public string VATGST { get; set; }
        public string ShortName { get; set; }
        public string Position { get; set; }
        public string Skype { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public int? WorkingWeekDayId { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        public string ProfileImage { get; set; }
        #endregion
        public async Task<ClaimsIdentity> GenerateUserIdentity(FrayteIdentityUserManager userManager, string authenticationType)
        {
            // Add custom user claims here
            return await userManager.CreateIdentityAsync(this, authenticationType);
        }
    }
}
