using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clone_KySoDienTu.Models
{
    public class BinhLuanViewModel    
    {
        public int MaBinhLuan { get; set; }
        public int MaTinTuc { get; set; }
        public int MaNguoiDung { get; set; }
        public string TenNguoiDung { get; set; }
        public string NoiDung { get; set; }
        public string TenBaiViet { get; set; }
        public string HinhNguoiDung { get; set; }
        public int CountTin { get; set; }
        public Nullable<bool> HienThi { get; set; }
        public Nullable<System.DateTime> Ngay { get; set; }
        public Nullable<System.TimeSpan> Gio { get; set; }
    }
}