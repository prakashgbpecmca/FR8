using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Security.IdentityModels
{
    public class FrayteIdentityRole : IdentityRole<int, FrayteIdentityUserRole>
    {
        #region properties 
        public string RoleDisplayName { get; set; }
        #endregion 
    }
}
