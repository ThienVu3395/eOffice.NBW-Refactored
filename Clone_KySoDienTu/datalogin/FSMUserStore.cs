using Clone_KySoDienTu.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OAuthBNLE.DataLogin
{
    public class FSMUserStore
    {
    }
    public class FSMUserStore<T> : IUserRoleStore<T>,
                               IUserStore<T>,
                               IUserPasswordStore<T>,
                               IUserEmailStore<T>,
                               IUserLockoutStore<T, string>,
                               IUserTwoFactorStore<T, string>
   where T : FSMIdentityUser
    {
        private readonly FSMUserRepository<T> _userTable;
        private readonly FSMUserRolesRepository _userRolesTable;

        public FSMUserStore(ApplicationDbContext databaseContext)
        {
            _userTable = new FSMUserRepository<T>(databaseContext);
            _userRolesTable = new FSMUserRolesRepository(databaseContext);
        }

        public Task CreateAsync(T user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.Run(() => _userTable.Insert(user));
        }

        public Task<T> FindByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Null or empty argument: userId");
            }

            return Task.Run(() => _userTable.GeTById(userId));
        }

        public Task<bool> GetTwoFactorEnabledAsync(T user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<T> FindByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Null or empty argument: userName");
            }

            return Task.Run(() => _userTable.GeTByName(userName));
        }

        public Task<IList<string>> GetRolesAsync(T user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.Run(() => _userRolesTable.FindByUserId(user.Id));
        }

        public Task<string> GetPasswordHashAsync(T user)
        {
            return Task.Run(() => _userTable.GetPasswordHash(user.Id));
        }

        public Task SetPasswordHashAsync(T user, string passwordHash)
        {
            return Task.Run(() => user.PasswordHash = passwordHash);
        }
        //public Task<IdentityResult> AddPasswordAsync(string userId, string password)
        //{
        //    return Task.Run(() => user.PasswordHash = passwordHash);
        //}
        public Task<T> FindByEmailAsync(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            return Task.Run(() => _userTable.GeTByEmail(email));
        }

        public Task<string> GetEmailAsync(T user)
        {
            return Task.FromResult(user.Email);
        }

        public Task<int> GetAccessFailedCountAsync(T user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(T user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(T user)
        {
            return
                 Task.FromResult(user.LockoutEndDateUtc.HasValue
                     ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                     : new DateTimeOffset());
        }

        public Task SetLockoutEnabledAsync(T user, bool enabled)
        {
            user.LockoutEnabled = enabled;

            return Task.Run(() => _userTable.Update(user));
        }

        //public Task SetPasswordHashAsync(T user, string hashedNewPassword)
        //{
        //    user.PasswordHash = hashedNewPassword;

        //    return Task.Run(() => _userTable.Update(user));
        //}

        public Task SetLockoutEndDateAsync(T user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(T user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(T user, string email)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(T user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(T user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(T user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T user)
        {
            return Task.Run(() => _userTable.Update(user));
            //throw new NotImplementedException();
        }

        public Task AddToRoleAsync(T user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.Run(() => _userRolesTable.AddToRole(user.Id, roleName));
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}