using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clone_KySoDienTu.Service.Dtos
{
    public class Cal_SuKienUser
    {
        public int SuKienId { get; set; }
        public string username { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayXem { get; set; }
        public bool DaXem { get; set; }
        public string FullName { get; set; }

    }
}



