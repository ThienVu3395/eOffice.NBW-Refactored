using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Clone_KySoDienTu.Models;
using OAuthBNLE.DataLogin;

namespace Clone_KySoDienTu
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    //public class ApplicationUserManager : UserManager<ApplicationUser>
    //{
    //    public ApplicationUserManager(IUserStore<ApplicationUser> store)
    //        : base(store)
    //    {
    //    }

    //    public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
    //    {
    //        var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
    //        // Configure validation logic for usernames
    //        manager.UserValidator = new UserValidator<ApplicationUser>(manager)
    //        {
    //            AllowOnlyAlphanumericUserNames = false,
    //            RequireUniqueEmail = true
    //        };
    //        // Configure validation logic for passwords
    //        manager.PasswordValidator = new PasswordValidator
    //        {
    //            RequiredLength = 6,
    //            RequireNonLetterOrDigit = true,
    //            RequireDigit = true,
    //            RequireLowercase = true,
    //            RequireUppercase = true,
    //        };
    //        var dataProtectionProvider = options.DataProtectionProvider;
    //        if (dataProtectionProvider != null)
    //        {
    //            manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
    //        }
    //        return manager;
    //    }
    //}
    public class FSMUserManager : UserManager<FSMIdentityUser, string>
    {
        public FSMUserManager(IUserStore<FSMIdentityUser, string> store)
            : base(store)
        {

        }

        // Create method called once during the lifecycle of request
        public static FSMUserManager Create(IdentityFactoryOptions<FSMUserManager> options, IOwinContext context)
        {
            // Configure the userstore to use the DbContext to work with database
            var manager = new FSMUserManager(new FSMUserStore<FSMIdentityUser>(context.Get<ApplicationDbContext>()));

            // The password validator enforces complexity on supplied password
            manager.PasswordValidator = new PasswordValidator()
            {
                RequiredLength = 6//,
                //RequireNonLetterOrDigit = true
            };

            // Use the custom password hasher to validate existing user credentials
            //manager.PasswordHasher = new AppPasswordHasher() { DbContext = context.Get<MyDatabaseContext>() };

            return manager;
        }

    }
}
