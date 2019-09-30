using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Security.IdentityModels
{
    public class FrayteIdentityUserLogin : IdentityUserLogin<int>
    {
        #region Properties

        public int? UserLoginId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsLocked { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        public int? FailedPasswordAttmemptCount { get; set; }
        public bool? IsRecoveryMailSent { get; set; }
        public string ProfileImage { get; set; }

        #endregion
    }
}
