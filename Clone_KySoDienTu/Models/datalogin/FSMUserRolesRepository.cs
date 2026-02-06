using Clone_KySoDienTu.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthBNLE.DataLogin
{

    internal class FSMUserRolesRepository
    {
        private readonly ApplicationDbContext _databaseContext;

        public FSMUserRolesRepository(ApplicationDbContext database)
        {
            _databaseContext = database;
        }

        /// <summary>
        /// Returns a list of user's roles
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public IList<string> FindByUserId(string userId)
        {
            var roles = _databaseContext.AspNetUsers.
                Where(u => u.Id == userId).SelectMany(r => r.AspNetRoles);
            return roles.Select(r => r.Name).ToList();
        }

        /// <summary>
        /// Returns a list of user's roles
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public Task AddToRole(string userId, string roleName)
        {
            var r = _databaseContext.AspNetRoles.Find(roleName);
            return Task.Run(() => _databaseContext.AspNetUsers.
                Where(u => u.Id == userId).First().AspNetRoles.Add(r));
            //return roles.Select(r => r.Name).ToList();
        }
    }
}