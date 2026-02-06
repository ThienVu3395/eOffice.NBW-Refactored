using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clone_KySoDienTu.Service.Dtos
{
    public class tbNguoiDungDto
    {
        public string USERNAME { get; set; }
        public string EMAIL { get; set; }
        public string HOLOT { get; set; }
        public string TEN { get; set; }
        
    }
    public class tbUserDto
    {
        //public string HOTEN { get; set; }
        //public string FILEHINH { get; set; }
        public int temp { get; set; }
        public string ID { get; set; }
        public string MSNV { get; set; }
        public string HOTEN { get; set; }
        public string FILEHINH { get; set; }
        public string HOLOT { get; set; }
        public string TEN { get; set; }
        public string CHUCVU { get; set; }
        public string BOPHAN { get; set; }
        public string DIACHI { get; set; }
        public string EMAIL { get; set; }
        public string PHONE { get; set; }
        public int NHOMID { get; set; }
        public string USERNAME { get; set; }
        public Nullable<System.DateTime> NGAYTAO { get; set; }
        public List<UserRoles> Roles { get; set; }
    }
    public class UserRoles
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
    }
}



