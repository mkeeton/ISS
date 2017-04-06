using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Security.Claims;
using ISS.Authentication.Data.Interfaces.Repositories;

namespace ISS.Managers
{
    public class ApplicationUserManager : UserManager<ISS.Authentication.Domain.Models.User, Guid>
    {
        public ApplicationUserManager(IUserStore<ISS.Authentication.Domain.Models.User, Guid> store)
          : base(store)
        {
            // Configure validation logic for usernames
            this.UserValidator = new UserValidator<ISS.Authentication.Domain.Models.User, Guid>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 2,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var repository = ISS.WebApiApplication.GetContainer().Kernel.Resolve<IUserRepository>();
            var manager = new ApplicationUserManager(repository);

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ISS.Authentication.Domain.Models.User, Guid>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ISS.Authentication.Domain.Models.User user, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await this.CreateIdentityAsync(user, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}