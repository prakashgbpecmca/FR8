using Frayte.Security.IdentityDataAccess;
using Frayte.Security.IdentityModels;
using Frayte.Security.IdentityService;
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
    public class FrayteIdentityUserManager : UserManager<FrayteIdentityUser, int>
    {
        #region constructors and destructors
        public FrayteIdentityUserManager(IUserStore<FrayteIdentityUser, int> store) : base(store)
        {
        }
        #endregion

        #region methods
        public static FrayteIdentityUserManager Create(IdentityFactoryOptions<FrayteIdentityUserManager> options, IOwinContext context)
        {
            var manager = new FrayteIdentityUserManager(new UserStore<FrayteIdentityUser, FrayteIdentityRole, int, FrayteIdentityUserLogin, FrayteIdentityUserRole, FrayteIdentityUserClaim>(context.Get<FrayteIdentityDbContext>()));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<FrayteIdentityUser, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true

            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireDigit = false,
                RequireLowercase = false,
                RequireNonLetterOrDigit = false,
                RequireUppercase = false,

            };
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider(
                "PhoneCode",
                new PhoneNumberTokenProvider<FrayteIdentityUser, int>
                {
                    MessageFormat = "Your security code is: {0}"
                });
            manager.RegisterTwoFactorProvider(
                "EmailCode",
                new EmailTokenProvider<FrayteIdentityUser, int>
                {
                    Subject = "Security Code",
                    BodyFormat = "Your security code is: {0}"
                });

            manager.EmailService = new FrayteIdentityEmailService();
            manager.SmsService = new FrayteIdentitySmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<FrayteIdentityUser, int>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            
            return manager;
        }
      
        
        #endregion
    }
}

