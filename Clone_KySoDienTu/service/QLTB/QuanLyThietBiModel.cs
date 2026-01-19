using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clone_KySoDienTu.Service.QLTB
{
    public class ThietBiViewModel
    {
        public int MaThietBi { get; set; }
        public string MaTaiSan { get; set; }
        public string Ten { get; set; }
        public string GiaTriThietBi { get; set; }
        public string GiaTriHD{ get; set; }
        public string Model { get; set; }
        public int MaLoaiThietBi { get; set; }
        public string TenLoaiThietBi { get; set; }
        public string NhaCungCap { get; set; }
        public string HangSanXuat { get; set; }
        public int TinhTrang { get; set; }
        public string SoHopDong { get; set; }
        public string LoaiHopDong { get; set; }
        public string Serial { get; set; }
        public string GhiChu { get; set; }
        public string ThoiGianBaoHanh { get; set; }
        public string IP { get; set; }
        public Nullable<System.DateTime> NgayMua { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public Nullable<System.DateTime> NgayBanGiao { get; set; }
        public string NguoiCapNhat { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgayKy { get; set; }
        public string NguoiSuDung { get; set; }
        public string PhongBanSuDung { get; set; }
        public string MaPhongBan { get; set; }
        public int Total { get; set; }
        public int IsDelete { get; set; }
        public List<LinhKienViewModel> DsLinhKien { get; set; }
    }

    public class FilterModel
    {
        public int MaDanhMuc { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public int IsDelete { get; set; }
        public Nullable<System.DateTime> BeginDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string MaPhong { get; set; }
        public int TinhTrang { get; set; }
        public int MaLoaiThietBi { get; set; }
        public string SearchString { get; set; }
    }

    public class LoaiThietBiViewModel
    {
        public int MaLoaiThietBi { get; set; }
        public string Ten { get; set; }
        public int ParentID { get; set; }
    }

    public class HangSanXuatModel
    {
        public int MaHangSanXuat { get; set; }
        public string Ten { get; set; }
    }

    public class PhongBanViewModel
    {
        public string MAPHONG { get; set; }
        public string MAPHONG2 { get; set; }
        public string TENPHONG { get; set; }
    }

    public class UserViewModel
    {
        public string ID { get; set; }
        public string USERNAME { get; set; }
        public string EMAIL { get; set; }
        public string HOTEN { get; set; }
        public string BOPHAN { get; set; }
    }

    public class TinhTrangModel
    {
        public int MaTinhTrang { get; set; }
        public string Ten { get; set; }
    }

    public class LoaiLinhKienModel
    {
        public int MaLoaiLinhKien { get; set; }
        public string Ten { get; set; }
    }

    public class LinhKienViewModel
    {
        public int MaLinhKien { get; set; }
        public int MaThietBi { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public int MaLoaiLinhKien { get; set; }
        public string TenLoaiLinhKien { get; set; }
        public string Serial { get; set; }
        public string Model { get; set; }
        public string NhaCungCap { get; set; }
        public string HangSanXuat { get; set; }
        public string GhiChu { get; set; }
        public string ThoiGianBaoHanh { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public string NguoiCapNhat { get; set; }
        public int TinhTrang { get; set; }
        public string NguoiSuDung { get; set; }
        public string PhongBanSuDung { get; set; }
    }

    public class LichSuThietBiViewModel
    {
        public int ID { get; set; }
        public int MaThietBi { get; set; }
        public int TinhTrang { get; set; }
        public string NguoiSuDung { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string ChiPhi { get; set; }
        public string PhongBanSuDung { get; set; }
        public string NguoiTao { get; set; }
        public string NguoiCapNhat { get; set; }
        public string GhiChu { get; set; }
        public Nullable<System.DateTime> NgayBanGiao { get; set; }


        public string NguoiThucHien { get; set; }
        public int ParentID { get; set; }
        public string HopDong { get; set; }
        public int IsDeleted { get; set; }    
    }

    public class HopDongThietBiViewModel
    {
        public int ID { get; set; }
        public int MaThietBi { get; set; }
        public string Ten { get; set; }
        public string GiaTriHD { get; set; }
        public Nullable<System.DateTime> NgayKy { get; set; }
        public string SoHopDong { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string GhiChu { get; set; }
    }

    public class LoaiHopDongThietBiModel
    {
        public int ID { get; set; }
        public string Ten { get; set; }
    }
}