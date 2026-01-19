using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clone_KySoDienTu.Service.Dtos
{
    public class CommonSearch
    {
        public DateTime valtime1 { get; set; }
        public int GroupID { get; set; }
    }
    public class Cal_SuKienNew
    {
        public Cal_SuKienDto par { get; set; }
        public List<GCAL_File> listfile { get; set; }
        public List<Cal_TaiNguyenDto> listtainguyen { get; set; }
        public List<Cal_ThongBaoDto> listthongbao { get; set; }
        public List<Cal_SuKienNoteDto> listnote { get; set; }
    }
    public class Cal_SuKienLogSMS
    {
        public TinNhanHenGio par1 { get; set; }
        public Cal_SuKienSMSLogs par2 { get; set; }

    }
    public class Cal_SuKienLog
    {
        public Cal_SuKienDto par { get; set; }
        public List<Cal_SuKienLogDto> listlog { get; set; }

    }
    public class dsLichDto
    {
        public List<SuKienView> dsLich { get; set; }
        public int Total { get; set; }
        public int Perpage { get; set; }
        public int Curpage { get; set; }
    }
    public class SuKienView
    {
        public int Total { get; set; }
        public int SuKienId { get; set; }
        public string TieuDe { get; set; }
        public int DaXem { get; set; }
        public Nullable<System.DateTime> BatDau { get; set; }
        public Nullable<System.DateTime> KetThuc { get; set; }
        public string TenNhom { get; set; }
    }
    public class Cal_SuKien7
    {
        public int week { get; set; }
        public DateTime NgayThang { get; set; }
        public int All { get; set; }
        public int Sang { get; set; }
        public int Chieu { get; set; }
        //public string SangChieu { get; set; }
        public List<Cal_SuKien7_1> dsSuKienSang { get; set; }
        public List<Cal_SuKien7_1> dsSuKienChieu { get; set; }
    }
    public class Cal_SuKien7_1
    {
        public int SuKienId { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public Nullable<System.DateTime> BatDau { get; set; }
        public Nullable<System.DateTime> KetThuc { get; set; }
        public int filedinhkem { get; set; }
        public int tinnhan { get; set; }
        public int SoLanCapNhat { get; set; }
        public Nullable<bool> QuanTrong { get; set; }
        public Nullable<bool> CaNgay { get; set; }
        public int DuocDuyet { get; set; }
        public string ChuanBi { get; set; }
        public string KhachMoi { get; set; }
        public string ThanhPhan { get; set; }
        public string DiaDiem { get; set; }
        public string LinkOnline { get; set; }
        public string ChuTri { get; set; }
        public string ThuKy { get; set; }

    }
    public class Cal_SuKienDto
    {
        public string TenNhom { get; set; }
        public int SuKienId { get; set; }
        public Nullable<int> NhomId { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public Nullable<System.DateTime> BatDau { get; set; }
        public Nullable<System.DateTime> KetThuc { get; set; }
        public string ChuanBi { get; set; }
        public Nullable<bool> QuanTrong { get; set; }
        public int DuocDuyet { get; set; }
        public Nullable<bool> CaNgay { get; set; }
        public int NguoiTaoId { get; set; }
        public string NguoiTao { get; set; }
        public string NguoiToChuc { get; set; }
        public string DiaDiem { get; set; }
        public string LinkOnline { get; set; }
        public string ChuTri { get; set; }
        public string KhachMoi { get; set; }
        public string ThanhPhan { get; set; }
        public string ThuKy { get; set; }
        public string NguoiThongBao { get; set; }
        public Nullable<bool> SMSThongBao { get; set; }
        public Nullable<bool> PopupThongBao { get; set; }
        public Nullable<bool> EmailThongBao { get; set; }
        public Nullable<bool> SMSHenGio { get; set; }
        public Nullable<bool> PopupHenGio { get; set; }
        public Nullable<bool> EmailHenGio { get; set; }
        public string SuDungTaiNguyen { get; set; }
        public Nullable<int> SMSThoiGian { get; set; }
        public Nullable<int> PopupThoiGian { get; set; }
        public Nullable<int> EmailThoiGian { get; set; }
        public string NguoiNhacHen { get; set; }
        public Nullable<int> SoLanCapNhat { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public string NguoiCapNhat { get; set; }
        public Nullable<System.DateTime> NgayDuyet { get; set; }
        public Nullable<int> NguoiDuyetId { get; set; }
        public string NguoiDuyet { get; set; }
        public Nullable<bool> CoDungTaiNguyen { get; set; }
        public Nullable<bool> CoTaiNguyenDuocDuyet { get; set; }
        public Nullable<int> NguoiDuyetTaiNguyenId { get; set; }
        public string NguoiDuyetTaiNguyen { get; set; }
        public Nullable<System.DateTime> NgayDuyetTaiNguyen { get; set; }

    }
    public class Cal_SuKienLogDto : Cal_SuKienDto
    {
        public Nullable<DateTime> NgayLuuLog { get; set; }
        public string NguoiLuuLog { get; set; }
    }
    public class Cal_ThongBaoDto
    {
        public int SuKienId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int DaXem {get;set;}
        public string GroupName { get; set; }
    }
    public class Cal_TaiNguyenDto
    {
        public int TaiNguyenID { get; set; }
        public int KieuID { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public int SoLuong { get; set; }
        //public bool LaDiaDiem { get; set; }

    }
    public class Cal_SuKienNoteDto
    {
        public int NoteId { get; set; }
        public int SuKienId { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public string fullname { get; set; }
        public string FileAnh { get; set; }
        
    }

    public class Cal_AddSuKienNoteDto : Cal_SuKienNoteDto
    {
        public string TieuDeSMS { get; set; }
        public string NguoiThongBao { get; set; }
        public bool SMSThongBao { get; set; }
    }
    public class TinNhanHenGio
    {
        public int TinNhanId { get; set; }
        public int SuKienId { get; set; }
        public string ThongBao { get; set; }
        public string NoiDung { get; set; }
        public int DaGui { get; set; }
        public DateTime NgayGui { get; set; }
        public DateTime NgayTao { get; set; }
        public bool SMSThongBao { get; set; }
        public bool SMSHenGio { get; set; }
        public int Type { get; set; }
    }
    public class Cal_SuKienSMSLogs
    {
        public int SuKienId { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayGui { get; set; }
        public string ThongBao { get; set; }
        public string TrangThai { get; set; }
    }
    public class CheckTaiNguyen
    {
        public int valint1 { get; set; }
        public int valint2 { get; set; }
        public Nullable<System.DateTime> valstring1 { get; set; }
        public Nullable<System.DateTime> valstring2 { get; set; }
    }

    public class Home_Sukien
    {
        public List<Cal_SuKien7_1> ListNow { get; set; }
        public List<Cal_SuKien7_1> ListNext { get; set; }
    }

    #region File 
    public class Cal_SuKienFileDto
    {
        public int ID { get; set; }
        public int LOAI { get; set; }
        public int SuKienId { get; set; }
        public string TENFILE { get; set; }
        public string MOTA { get; set; }
        public Nullable<System.DateTime> NGAYTAO { get; set; }
        public Nullable<bool> TRANGTHAI { get; set; }
        public string LOAIFILE { get; set; }
        public Nullable<int> SIZEFILE { get; set; }
        public Nullable<int> VITRIID { get; set; }
    }

    public class GCAL_File
    {
        public int ID { get; set; }
        public int ActionId { get; set; }
        public string TenFile { get; set; }
        public string MoTa { get; set; }
        public string LoaiFile { get; set; }
        public int SizeFile { get; set; }
        public DateTime NgayTao { get; set; }
        public bool isDelete { get; set; }
    }
    #endregion
}



