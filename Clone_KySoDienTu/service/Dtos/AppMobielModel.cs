using Clone_KySoDienTu.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Clone_KySoDienTu.Models;
namespace Clone_KySoDienTu.Models
{
    public partial class NEWS_TinTucChiTietModel
    {

        public int MaTinTuc { get; set; }

        public string TieuDe { get; set; }

        public string NoiDung { get; set; }

        public string MoTa { get; set; }

        public Nullable<int> MaLoaiTin { get; set; }

        public Nullable<int> NguoiTao { get; set; }

        public string TenNguoiTao { get; set; }

        public Nullable<System.DateTime> NgayTao { get; set; }

        public Nullable<System.DateTime> NgayCapNhat { get; set; }

        public Nullable<int> NguoiCapNhat { get; set; }

        public string TenNguoiCapNhat { get; set; }

        public Nullable<int> NguoiDuyet { get; set; }

        public string TenNguoiDuyet { get; set; }

        public Nullable<System.DateTime> NgayDuyet { get; set; }

        public Nullable<bool> HienThi { get; set; }

        public Nullable<int> LuotXem { get; set; }

        public Nullable<bool> TinNoiBat { get; set; }

        public string HinhAnh { get; set; }

        public Nullable<System.DateTime> NgayHetHan { get; set; }

        public Nullable<System.DateTime> NgayHetHanTinMoi { get; set; }

        public Nullable<System.DateTime> NgayHetHanTrangChu { get; set; }

        public Nullable<bool> Khoa { get; set; }

        public Nullable<int> NguoiKhoa { get; set; }

        public string TenNguoiKhoa { get; set; }

        public Nullable<System.DateTime> NgayKhoa { get; set; }

        public string DuongDan { get; set; }

        public Nullable<int> NgayDang { get; set; }

        public Nullable<int> ThangDang { get; set; }

        public Nullable<int> NamDang { get; set; }

        public string GhiChu { get; set; }

        public Nullable<bool> ChiaSe { get; set; }


    }
    public partial class tbNguoidungModel
    {
      
       
        public string USERNAME { get; set; }
        public string EMAIL { get; set; }
        public string HOLOT { get; set; }
        public string TEN { get; set; }
        public System.DateTime NGAYTAO { get; set; }
        public bool KHOA { get; set; }
        public Nullable<short> LANHDAO { get; set; }
        public string CHUCVU { get; set; }
        public string BOPHAN { get; set; }
        public string UYQUYEN { get; set; }
        public Nullable<System.DateTime> NGAYUQ { get; set; }
        public Nullable<System.DateTime> KETTHUCUQ { get; set; }
        public bool HANCHE { get; set; }
        public string FILEANH { get; set; }
        public string CHUCVU2 { get; set; }
        public Nullable<int> SAPXEP { get; set; }
        public Nullable<int> JobTitleID { get; set; }
        public string ID_Firebade { get; set; }
    }
    public partial class NEWS_LoaiTinTucChiTietModel
    {

        public int MaLoaiTin { get; set; }

        public string Ten { get; set; }

        public Nullable<bool> TrangThai { get; set; }

        public Nullable<int> ThuTuHienThi { get; set; }

        public string Icon { get; set; }

        public Nullable<int> TemplateList { get; set; }

        public Nullable<int> TemplateDetail { get; set; }

        public Nullable<int> NumberOfItems { get; set; }

        public Nullable<bool> IsOnHome { get; set; }

        public Nullable<int> RequiredApproved { get; set; }

        public string NotifiedUsers { get; set; }

        public string HinhAnhDuPhong { get; set; }


    }

    public partial class ModelThongTinChung
    {
        public List<New_TinTucModel> listdata { get; set; }
        public int Toltalrow { get; set; }
        public int currentPage { get; set; }
        public int perPage { get; set; }
        //public string rule { get; set; }

    }
    public partial class ModelThongBaoTinTuc
    {
        public List<NEWS_TinTuc> listdata { get; set; }
        public int Toltalrow { get; set; }
        public int currentPage { get; set; }
        public int perPage { get; set; }
        //public string rule { get; set; }

    }
    public partial class NEWS_TinTuc
    {
        public int MaTinTuc { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public string MoTa { get; set; }
        public Nullable<int> MaLoaiTin { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public string NguoiCapNhat { get; set; }
        public string NguoiDuyet { get; set; }
        public Nullable<System.DateTime> NgayDuyet { get; set; }
        public Nullable<bool> HienThi { get; set; }
        public Nullable<int> LuotXem { get; set; }
        public Nullable<bool> TinNoiBat { get; set; }
        public string HinhAnh { get; set; }
        public Nullable<System.DateTime> NgayHetHan { get; set; }
        public Nullable<System.DateTime> NgayHetHanTinMoi { get; set; }
        public Nullable<System.DateTime> NgayHetHanTrangChu { get; set; }
        public Nullable<bool> Khoa { get; set; }
        public string NguoiKhoa { get; set; }
        public Nullable<System.DateTime> NgayKhoa { get; set; }
        public string DuongDan { get; set; }
    }
    public partial class NEWS_LoaiTinTuc
    {

        public int MaLoaiTin { get; set; }
        public string Ten { get; set; }
        public Nullable<bool> TrangThai { get; set; }
        public Nullable<int> ThuTuHienThi { get; set; }
        public string Icon { get; set; }
        public Nullable<int> TemplateList { get; set; }
        public Nullable<int> RequiredApproved { get; set; }
        public string HinhAnhDuPhong { get; set; }

    }
    public partial class ModuleNEWS_TinTuc_TapTin
    {
        public int? MaTinTuc { get; set; }
        public int? MaTinTuc_TapTin { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public string MoTa { get; set; }
        public Nullable<int> MaLoaiTin { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public string NguoiCapNhat { get; set; }
        public string NguoiDuyet { get; set; }
        public Nullable<System.DateTime> NgayDuyet { get; set; }
        public Nullable<bool> HienThi { get; set; }
        public Nullable<int> LuotXem { get; set; }
        public Nullable<bool> TinNoiBat { get; set; }
        public string HinhAnh { get; set; }
        public Nullable<System.DateTime> NgayHetHan { get; set; }
        public Nullable<System.DateTime> NgayHetHanTinMoi { get; set; }
        public Nullable<System.DateTime> NgayHetHanTrangChu { get; set; }
        public Nullable<bool> Khoa { get; set; }
        public string NguoiKhoa { get; set; }
        public Nullable<System.DateTime> NgayKhoa { get; set; }
        public string DuongDan { get; set; }

    
        public int MaTapTin { get; set; }
        public string Ten { get; set; }
        public string Url { get; set; }
        public string Url1 { get; set; }
        public Nullable<System.DateTime> Ngay { get; set; }
    }

    public partial class New_TinTucModel
    {
        public int MaTinTuc { get; set; }

        public string TieuDe { get; set; }

        public string NoiDung { get; set; }

        public string MoTa { get; set; }

        public Nullable<int> MaLoaiTin { get; set; }

        public Nullable<int> NguoiTao { get; set; }

        public string TenNguoiTao { get; set; }

        public Nullable<System.DateTime> NgayTao { get; set; }

        public Nullable<System.DateTime> NgayCapNhat { get; set; }

        public Nullable<int> NguoiCapNhat { get; set; }

        public string TenNguoiCapNhat { get; set; }

        public Nullable<int> NguoiDuyet { get; set; }

        public string TenNguoiDuyet { get; set; }

        public Nullable<System.DateTime> NgayDuyet { get; set; }

        public Nullable<bool> HienThi { get; set; }

        public Nullable<int> LuotXem { get; set; }

        public Nullable<bool> TinNoiBat { get; set; }

        public string HinhAnh { get; set; }

        public Nullable<System.DateTime> NgayHetHan { get; set; }

        public Nullable<System.DateTime> NgayHetHanTinMoi { get; set; }

        public Nullable<System.DateTime> NgayHetHanTrangChu { get; set; }

        public Nullable<bool> Khoa { get; set; }

        public Nullable<int> NguoiKhoa { get; set; }

        public string TenNguoiKhoa { get; set; }

        public Nullable<System.DateTime> NgayKhoa { get; set; }

        public string DuongDan { get; set; }

        public Nullable<int> NgayDang { get; set; }

        public Nullable<int> ThangDang { get; set; }

        public Nullable<int> NamDang { get; set; }

        public string GhiChu { get; set; }

        public Nullable<bool> ChiaSe { get; set; }
    }


    public partial class datasend
    {
        public string app_id { get; set; }
        
        public content contents { get; set; }
        public List<string> include_player_ids { get; set; }
    }

    public partial class content
    {
        public string en { get; set; }
    }
    public class CommonModelMobile
    {
        public string valstring1 { get; set; }
        public string valstring2 { get; set; }

    }

}


