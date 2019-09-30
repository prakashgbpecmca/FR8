using Frayte.Services.DataAccess;
//using Frayte.WebApi.OAuthContext;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Frayte.Security.IdentityModels;
using Frayte.Security.IdentityManager;

namespace Frayte.WebApi.Providers
{
    public class FrayteAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                var userManager = context.OwinContext.GetUserManager<FrayteIdentityUserManager>();

                var dbContext = context.OwinContext.Get<IdentityDbContext>();

                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                FrayteIdentityUser user;
                user = await userManager.FindByEmailAsync("");
                if (user != null)
                {
                    //  GetOwinContext().GetUserManager<FrayteIdentityUserManager>()
                }
                else
                {
                    user = await userManager.FindAsync(context.UserName, context.Password);
                }

                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                try
                {
                    var roles = await userManager.GetRolesAsync(user.Id);
                    identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                    foreach (var item in roles)
                    {
                        if (!string.IsNullOrEmpty(item))
                            identity.AddClaim(new Claim(ClaimTypes.Role, item));
                    }
                    context.Validated(identity);
                }
                catch (Exception ex)
                {
                    context.SetError("server_error", "Something bad happed. Try again later.");
                    return;
                }
            }
            catch (Exception ex)
            {
                context.SetError("server_error", "Something bad happed. Try again later.");
                return;
            }
        }
    }
}