using Frayte.Security.IdentityModels;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Security.IdentityManager
{
    public class FrayteIdentitySignInManager : SignInManager<FrayteIdentityUser, int>
    {
        public FrayteIdentitySignInManager(FrayteIdentityUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public static FrayteIdentitySignInManager Create(IdentityFactoryOptions<FrayteIdentitySignInManager> options, IOwinContext context)
        {
            return new FrayteIdentitySignInManager(context.GetUserManager<FrayteIdentityUserManager>(), context.Authentication);
        }
    }
}
