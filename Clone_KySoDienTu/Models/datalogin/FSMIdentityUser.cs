using Clone_KySoDienTu;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OAuthBNLE.DataLogin
{
    public class FSMIdentityUser : IdentityUser<string,
                                IdentityUserLogin,
                                IdentityUserRole,
                                IdentityUserClaim>, IUser
    {
        public FSMIdentityUser()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Task<ClaimsIdentity> GenerateUserIdentity(FSMUserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = manager.CreateIdentity<FSMIdentityUser, string>(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return Task.FromResult(userIdentity);
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(FSMUserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        public Task<ClaimsIdentity> GenerateUserIdentity(FSMUserManager manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = manager.CreateIdentity<FSMIdentityUser, string>(this, authenticationType);
            // Add custom user claims here
            return Task.FromResult(userIdentity);
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(FSMUserManager manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

    }
}