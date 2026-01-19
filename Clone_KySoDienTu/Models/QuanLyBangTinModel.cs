using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clone_KySoDienTu.Models
{
    public class LoaiTinTucModel
    {
        public int MaLoaiTin { get; set; }
        public string Ten { get; set; }
        public bool TrangThai { get; set; }
        public int ThuTuHienThi { get; set; }
        public string Icon { get; set; }
        public int TemplateList { get;set;}
        public string HinhAnhDuPhong { get; set; }
        public int RequiredApproved { get; set; }
    }

    public class TinTucModel
    {
        public int MaTinTuc { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public string MoTa { get; set; }
        public int MaLoaiTin { get; set; }
        public string LoaiTin { get; set; }
        public string HinhNguoiDung { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiCapNhat { get; set; }
        public string DuongDan { get; set; }
        public string NguoiDuyet { get; set; }
        public Nullable<System.DateTime> NgayDuyet { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public int LuotXem { get; set; }
        public bool HienThi { get; set; }
        public bool TinNoiBat { get; set; }
        public string HinhAnh { get; set; }
        public string HinhAnhDuPhong { get; set; }
        public int CountTin { get; set; }
        public List<TapTinModel> TapTinDinhKem { get; set; }
        public List<TapTinModel> FileCu { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public DateTime? NgayHetHanTinMoi { get; set; }
        public DateTime? NgayHetHanTrangChu { get; set; }
        public int? TemplateList { get; set; }
        public int? TemplateDetail { get; set; }
        public List<TinTucModel> TinLienQuan { get; set; }
        public List<TinTucModel> TinCuHon { get; set; }
    }

    public class NguoiDungModel
    {
        public int MaNguoiDung { get; set; }
        public string Ten { get; set; }
        public int MaPhongBan { get; set; }
        public string PhongBan { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ChucVu { get; set; }
        public Nullable<int> NgaySinh { get; set; }
        public Nullable<int> ThangSinh { get; set; }
        public Nullable<int> NamSinh { get; set; }
        public string HinhAnh { get; set; }
    }

    public class UserExtension
    {
        public string username { get; set; }
        public string shopingcart { get; set; }
        public string FILEHINH { get; set; }
        public string HOTEN { get; set; }
    }

    public class TapTinModel
    {
        public int MaTinTuc { get; set; }
        public int MaTapTin { get; set; }
        public string Ten { get; set; }
        public string Url { get; set; }
        public string Size { get; set; }
    }

    public class FilterNewsModel
    {
        public int MaLoaiTin { get; set; }
        public int HienThi { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public string SearchString { get; set; }
    }

    public class BaiVietDaXemModel
    {
        public int MaTinTuc { get; set; }
        public string NguoiDung { get; set; }
        public Nullable<System.DateTime> NgayXem { get; set; }
        public string MoTa { get; set; }
    }

    public class BinhLuanModel
    {
        public int MaBinhLuan { get; set; }
        public int MaTinTuc { get; set; }
        public int MaNguoiDung { get; set; }
        public string TenNguoiDung { get; set; }
        public string HinhAnh { get; set; }
        public string DonVi { get; set; }
        public string NoiDung { get; set; }
        public Nullable<Boolean> HienThi { get; set; }
        public Nullable<System.DateTime> Ngay { get; set; }
        public Nullable<System.TimeSpan> Gio { get; set; }
    }
}