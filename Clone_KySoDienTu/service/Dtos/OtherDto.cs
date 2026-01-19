using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clone_KySoDienTu.Service.Dtos
{
    public class dsGopYPhanHoi
    {
        public List<Other_GopYPhanHoiGet> dsGopY { get; set; }
        public int Total { get; set; }
        public int Perpage { get; set; }
        public int Curpage { get; set; }
    }
    public class Other_GopYPhanHoi
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string Loai { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public int TrangThai { get; set; }
        public bool CongKhai { get; set; }
        public string FileAnh { get; set; }
    }

    public class Other_GopYPhanHoiGet : Other_GopYPhanHoi
    {
        public int Total { get; set; }
        public List<Other_GopYPhanHoiGet> phanhoi { get; set; }
        public List<Other_FileGopYPhanHoiDto> listfile { get; set; }
    }
    public class Other_GopYPhanHoiNew : Other_GopYPhanHoi
    {
       public List<Other_FileGopYPhanHoiDto> listfile { get; set; }
    }
    public class Other_FileGopYPhanHoiDto
    {
        public int ID { get; set; }
        public int LOAI { get; set; }
        public int GOPYID { get; set; }
        public string TENFILE { get; set; }
        public string MOTA { get; set; }
        public System.DateTime NGAYTAO { get; set; }
        public bool TRANGTHAI { get; set; }
        public string LOAIFILE { get; set; }
        public int SIZEFILE { get; set; }
        public int VITRIID { get; set; }
    }
}



