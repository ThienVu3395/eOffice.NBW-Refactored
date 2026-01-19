using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OAuthBNLE.DataLogin
{
    [Table("AspNetUsersExt")]
    public partial class AspNetUsersExt
    {
        [Key]
        public string UserId { get; set; }

        [StringLength(256)]
        public string SecurityQuestion { get; set; }

        [StringLength(128)]
        public string SecurityAnswer { get; set; }

        [StringLength(128)]
        public string SecurityAnswerSalt { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
    }
}