using Frayte.Security.IdentityDataAccess;
using Frayte.Security.IdentityModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Security.IdentityManager
{
    public class FrayteIdentityRoleManager : RoleManager<FrayteIdentityRole, int>
    {
        public FrayteIdentityRoleManager(IRoleStore<FrayteIdentityRole, int> store) : base(store)
        {

        }
        public static FrayteIdentityRoleManager Create(IdentityFactoryOptions<FrayteIdentityRoleManager> options, IOwinContext context)
        {
            var manager = new FrayteIdentityRoleManager(new RoleStore<FrayteIdentityRole, int, FrayteIdentityUserRole>(context.Get<FrayteIdentityDbContext>()));
            return manager;
        }
    }
}
