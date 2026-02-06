using Microsoft.AspNet.Identity;
using System;

namespace OAuthBNLE.DataLogin
{

    public class FSMIdentityRole : IRole
    {
        public FSMIdentityRole()
        {
            Id = Guid.NewGuid().ToString();
        }

        public FSMIdentityRole(string name)
            : this()
        {
            Name = name;
        }

        public FSMIdentityRole(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}