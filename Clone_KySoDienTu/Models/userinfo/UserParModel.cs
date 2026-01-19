using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clone_KySoDienTu.Models.UserInfo
{
    public class UserParModel
    {
        public string username { get; set; }

        public int CurrentPage { get; set; }

        public int ID { get; set; }
        public string shopingcart { get; set; }
    }
    public class UserAParModel
    {
        public string valstring1 { get; set; }
        public string valstring2 { get; set; }
        public string valstring3 { get; set; }
        public string valstring4 { get; set; }
        public int valint1 { get; set; }
        public int valint2 { get; set; }
        public int valint3 { get; set; }
        public int valint4 { get; set; }
    }
    public partial class tbNhanvienmodel
    {

        public string USERNAME { get; set; }
        public string HOTEN { get; set; }
        public string EMAIL { get; set; }
        public string FILEHINH { get; set; }
        public int KHOA { get; set; }


    }
}