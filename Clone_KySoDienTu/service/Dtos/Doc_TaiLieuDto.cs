using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clone_KySoDienTu.Service.Dtos
{

    public class Doc_NhomTaiLieu
    {
        public int NhomId { get; set; }
        public int ParentId { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public int CountItem { get; set; }
        public int SapXep { get; set; }
        public int YeuCauDuyet { get; set; }
        public int HienThi { get; set; }
    }

    public class AddDoc_NhomTaiLieu
    {
        public int NhomId { get; set; }
        public int ParentId { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public int YeuCauDuyet { get; set; }
        public int HienThi { get; set; }
        public bool CopyPer { get; set; }
    }
    public class UpdateDoc_NhomTaiLieu
    {
        public int TrangThai { get; set; }
        public int NhomId { get; set; }
        public int ParentId { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public int YeuCauDuyet { get; set; }
        public int HienThi { get; set; }
    }
    public class Doc_Menu
    {
        public Doc_NhomTaiLieu value { get; set; }
        public List<Doc_Menu> childmenu { get; set; }
    }

    public class Doc_dsTaiLieu
    {
        public int Total { get; set; }
        public int NhomId { get; set; }
        public int TaiLieuId { get; set; }
        public string Ten { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public Nullable<System.DateTime> NgayBanHanh { get; set; }
        public string NguonNhap { get; set; }
        public string LoaiFile { get; set; }
        public int Khoa { get; set; }
        public bool QuanTrong { get; set; }
        public int TrangThai { get; set; }
    }

    public class Doc_TaiLieuDetail
    {
        public int TaiLieuId { get; set; }
        public int NhomId { get; set; }
        public string TieuDe { get; set; }
        public string KyHieu { get; set; }
        public string GhiChu { get; set; }
        public string TenFile { get; set; }
        public string MoTa { get; set; }
        public Nullable<System.DateTime> NgayBanHanh { get; set; }
        public string TenNhom { get; set; }
        public string LoaiFile { get; set; }
        public string FileSize { get; set; }
        public bool QuanTrong { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<System.DateTime> NgayTaoFileCongVan { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public string NguoiCapNhat { get; set; }
        public bool Khoa { get; set; }
        public string NguoiLock { get; set; }
        public int TrangThai { get; set; }
        public int ThoiGianLuuTru { get; set; }
        public string NguonNhap { get; set; }
        public string NguoiDuyet { get; set; }
        public Nullable<System.DateTime> NgayDuyet { get; set; }
    }
    public class Doc_EditTaiLieu
    {
        public int LoaiEdit { get; set; }
        public int TaiLieuId { get; set; }
        public int NhomId { get; set; }
        public string TieuDe { get; set; }
        public string KyHieu { get; set; }
        public string GhiChu { get; set; }
        public string TenFile { get; set; }
        public string MoTa { get; set; }
        public string LoaiFile { get; set; }
        public string FileSize { get; set; }
        public bool QuanTrong { get; set; }
        public bool Khoa { get; set; }
        public int TrangThai { get; set; }
        public Nullable<System.DateTime> NgayBanHanh { get; set; }
        public int ThoiGianLuuTru { get; set; }
    }
    public class Doc_TaiLieuVersion
    {
        public int VersionId { get; set; }
        public int TaiLieuId { get; set; }
        public int NhomId { get; set; }
        public string TieuDe { get; set; }
        public string GhiChu { get; set; }
        public bool QuanTrong { get; set; }
        public string TenFile { get; set; }
        public string MoTa { get; set; }
        public string LoaiFile { get; set; }
        public string FileSize { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public string NguoiCapNhat { get; set; }
        public string Link { get; set; }

    }
    public class Doc_ListMenu
    {
        public string NhomId { get; set; }
        public string Ten { get; set; }
    }

    public class Doc_ListFileDelete
    {
        public int TaiLieuId { get; set; }
        public int NhomId { get; set; }
        public string TenFile { get; set; }
    }
}



