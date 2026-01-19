using System;

namespace Clone_KySoDienTu.Service.Dtos
{
    public class Core_UserDto
    {
        public string UserID { get; set; }
        public string MSNV { get; set; }
        public int GroupId { get; set; }
        //public string GroupName { get; set; }
        public string MAPHONG { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Mobile { get; set; }
        public string HOLOT { get; set; }
        public string TEN { get; set; }
        public string DIACHI { get; set; }
        public string BOPHAN { get; set; }
        public string CHUCVU { get; set; }
        public string CHUCVU2 { get; set; }
        public int LOAIXULY { get; set; }
        public int UserIDVB { get; set; }
        public Nullable<System.DateTime> NGAYTAO { get; set; }
        public string FILEANH { get; set; }
        public int Total { get; set; }
    }
    public class Core_DangBoUser
    {
        public string UserID { get; set; }
        public int GroupId { get; set; }
        public string MAPHONG { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string BOPHAN { get; set; }
        public string CHUCVU { get; set; }
    }

    public class Core_Module
    {
        public string ModuleKey { get; set; }
        public string ModuleName { get; set; }
    }

}



