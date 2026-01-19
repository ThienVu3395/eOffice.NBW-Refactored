using Clone_KySoDienTu.Service.Dtos;
using SteProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Clone_KySoDienTu.Models
{
    [XmlRoot("tbVanBan")]
    public class ltbVanbanOcr
    {
        public string DocCode { get; set; }
        public string FileCode { get; set; }
        public string Identifier { get; set; }
        public string Organld { get; set; }
        public string FileCatalog { get; set; }
        public string FileNotation { get; set; }
        public string DocOrdinal { get; set; }
        public string TypeName { get; set; }
        public string CodeNumber { get; set; }
        public string CodeNotation { get; set; }
        public string IssuedDate { get; set; }
        public string OrganName { get; set; }
        public string Subject { get; set; }
        public string Language { get; set; }
        public string PageAmount { get; set; }
        public string Description { get; set; }
        public string InforSign { get; set; }
        public string Keyword { get; set; }
        public string ModeSte { get; set; }
        public string ConfidenceLevel { get; set; }
        public string Autograph { get; set; }
        public string Format { get; set; }
    }
    public class VanBanViewModel
    {
        public int ID { get; set; }
        public string OrganId { get; set; }
        public int FileCatalog { get; set; }
        public string FileNotation { get; set; }
        public int DocOrdinal { get; set; }
        public string TypeName { get; set; }
        public int CodeNumber { get; set; }
        public string CodeNotation { get; set; }
        public Nullable<System.DateTime> IssuedDate { get; set; }
        public string OrganName { get; set; }
        public string Subject { get; set; }
        public string Language { get; set; }
        public int PageAmount { get; set; }
        public string Description { get; set; }
        public string Position { get; set; }
        public string Fullname { get; set; }
        public Nullable<int> Priority { get; set; }
        public Nullable<int> IssuedAmount { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public int SoVanBanID { get; set; }
        public int TrangThai { get; set; }
        public string NguoiDuyet { get; set; }
        public Nullable<System.DateTime> NgayDuyet { get; set; }
        public Nullable<System.DateTime> NgayKy { get; set; }
        public string NguoiCapNhat { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiButPhe { get; set; }
        public Nullable<System.DateTime> NgayButPhe { get; set; }
        public string NguoiKy { get; set; }
        public string MOREINFO1 { get; set; }
        public string MOREINFO2 { get; set; }
        public string MOREINFO3 { get; set; }
        public string MOREINFO4 { get; set; }
        public string MOREINFO5 { get; set; }
        public string OrganSendName { get; set; }
        public string OrganReceiveName { get; set; }
        public Nullable<System.DateTime> NgayKetThuc { get; set; }
        public string NguoiKetThuc { get; set; }

        public List<WorkFlowModel> WorkFlowModels { get; set; }
        public List<FileDinhKemViewModel> FileDinhKem { get; set; }
        public List<NguoiDungViewModel> NguoiThamGia { get; set; }
        public List<NguoiDungViewModel> NguoiTheoDoi { get; set; }
        public List<NguoiDungViewModel> NguoiThamGiaPhoiHop { get; set; }
        public List<NguoiDungViewModel> NguoiDaXem { get; set; }
        public bool HienThi { get; set; }
        public string TENSO { get; set; }
        public int Total { get; set; }
        public int DaXem { get; set; }
        public int CongKhai { get; set; }
        public string CANBO { get; set; }
        public string TenNguoiDuyet { get; set; }
        public string TenNguoiCapNhat { get; set; }
        public string YKienLanhDao { get; set; }
        public Nullable<System.DateTime> NGAYMO { get; set; }
        public string LANHDAO { get; set; }
        public int PARENTID { get; set; }
        public int IDUSER { get; set; }
        public int LOAIXULY { get; set; }
    }

    public class SoVanBanViewModel
    {
        public int ID { get; set; }
        public string TENSO { get; set; }
        public string KYHIEU { get; set; }
        public string DONVIID { get; set; }
        public string LOAIVB { get; set; }
        public string GHICHU { get; set; }
        public Nullable<int> LOAI { get; set; }
        public Nullable<int> VITRILUU { get; set; }
    }

    public class LogVanBanModel
    {
        public int ID { get; set; }
        public int IDVANBAN { get; set; }
        public int LoaiVanBan { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public string NoiDung { get; set; }
    }

    public class LoaiVanBanViewModel
    {
        public string MALOAI { get; set; }
        public string TENLOAI { get; set; }
        public string GHICHU { get; set; }
    }

    public class FileDinhKemViewModel
    {
        public int ID { get; set; }
        public int LOAI { get; set; }
        public int VANBANID { get; set; }
        public string TENFILE { get; set; }
        public string MOTA { get; set; }
        public Nullable<System.DateTime> NGAYTAO { get; set; }
        public Nullable<bool> TRANGTHAI { get; set; }
        public string LOAIFILE { get; set; }
        public Nullable<int> SIZEFILE { get; set; }
        public Nullable<int> VITRIID { get; set; }
    }

    public class NguoiDungViewModel
    {
        public string ID { get; set; }
        public string USERNAME { get; set; }
        public string EMAIL { get; set; }
        public string HOLOT { get; set; }
        public string TEN { get; set; }
        public string FullName { get; set; }
        public string HOTEN { get; set; }
        public string CHUCVU { get; set; }
        public string BOPHAN { get; set; }
        public int? DaXem { get; set; }
        public int LOAIXULY { get; set; }
        public Nullable<System.DateTime> NgayXem { get; set; }
    }

    public class PhanTrangModel
    {
        public int Start { get; set; }
        public int End { get; set; }
    }

    public class VanBanDenCanBoViewModel
    {
        public int ID { get; set; }
        public int IDUSER { get; set; }
        public int IDVANBAN { get; set; }
        public string CANBO { get; set; }
        public Nullable<System.DateTime> NGAYMO { get; set; }
        public int LOAIXULY { get; set; }
        public int DAXEM { get; set; }
        public int PARENTID { get; set; }
        public string PHONGBAN { get; set; }
        public string NguoiPhanCong { get; set; }
        public Nullable<System.DateTime> NGAYPHANCONG { get; set; }
    }
    
    public class ThongBaoViewModel
    {
        public int SoLuong { get; set; }
    }

    public class FilterModel
    {
        public int SoVanBanID { get; set; }
        public int LoaiVB { get; set; }
        public string CANBO { get; set; }
        public string NguoiTao { get; set; }
        public int DaXem { get; set; }
        public string SearchString { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public string FileNotation { get; set; }
        public string CodeNumber { get; set; }
        public Nullable<System.DateTime> BeginDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public int LoaiTimKiem { get; set; }
        public int DaDuyet { get; set; }
        public int Quyen { get; set; }
        public string TypeName { get; set; }
        public string OrganSendName { get; set; }
        public int HoanThanh { get; set; }
    }

    public class FilterModel2
    {
        public int SoVanBanID { get; set; }
        public int LoaiVB { get; set; }
        public string CANBO { get; set; }
        public string NguoiTao { get; set; }
        public int DaXem { get; set; }
        public string SearchString { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public string FileNotation { get; set; }
        public string CodeNumber { get; set; }
        public Nullable<System.DateTime> BeginDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public int LoaiTimKiem { get; set; }
        public int TrangThai { get; set; }
        public int LoaiLoc { get; set; }
        public string TypeName { get; set; }
        public string OrganSendName { get; set; }
        public int Quyen { get; set; }
    }

    public class YKienXuLyModel
    {
        public int ID { get; set; }
        public int PARENTID { get; set; }
        public int VANBANID { get; set; }
        public string CANBO { get; set; }
        public DateTime NGAYXL { get; set; }
        public string YKIENXL { get; set; }
        public int TRANGTHAI { get; set; }
        public int LOAIXL { get; set; }
        public int CONGKHAI { get; set; }
    }

    public class DanhSachYKienXuLyModel
    {
        public List<YKienXuLyModel> YKienLanhDao { get; set; }
        public List<YKienXuLyModel> YKienTheoDoi { get; set; }
        public List<YKienXuLyModel> YKienXuLy { get; set; }
        public List<YKienXuLyModel> YKienPhoiHop { get; set; }
        public int LOAIXULY { get; set; }
    }

    public class PhanPhatVanBanModel
    {
        public List<Core_UserDto> DsUser { get; set; }
        public int IDVANBAN { get; set; }
        public int IDUSER { get; set; }
        public int PARENTID { get; set; }
        public int LoaiVB { get; set; }
    }

    public class CommonReturnValueModel
    {
        public int ID { get; set; }
        public int ID2 { get; set; }
        public int ID3 { get; set; }
        public int Total { get; set; }
    }

    public class CongVanTrangChuModel
    {
        public List<VanBanViewModel> VanBanDen { get; set; }
        public List<VanBanViewModel> VanBanDi { get; set; }
        public List<VanBanViewModel> VanBangDangBo { get; set; }
    }

    public class ThongBaoModel
    {
        public int SoLuongTB { get; set; }
        public int SoLuongTBVBDenDB { get; set; }
        public int SoLuongTBDi { get; set; }
        public int SoLuongTBVBDiDB { get; set; }
        public int SoLuongTBDenChoDuyet { get; set; }
        public int SoLuongTBVBDenDBChoDuyet { get; set; }
        public int SoLuongTBDiChoDuyet { get; set; }
        public int SoLuongTBVBDiDBChoDuyet { get; set; }
        public List<CommonModel> dsTBXoa { get; set; }
        public List<CommonModel> dsTBXoaDi { get; set; }
    }

    public class PhongBanModel
    {
        public string MAPHONG { get; set; }
        public string TENPHONG { get; set; }
        public string DVQUANLY { get; set; }
        public string MADONVI { get; set; }
    }

    public class VanBanWorkFlowModel
    {
        public int ID { get; set; }
        public int IDVANBANDI { get; set; }
        public int IDVANBANDEN { get; set; }
        public int LOAI { get; set; }
        public int WorkflowID { get; set; }
    }

    public class WorkFlowModel
    {
        public int TRANGTHAI { get; set; }
        public Nullable<System.DateTime> NGAYBD { get; set; }
        public Nullable<System.DateTime> THOIHAN { get; set; }
        public Nullable<System.DateTime> NGAYGH { get; set; }
        public string TENCV { get; set; }
    }

    public class VanBanCongViecModel
    {
        public int TRANGTHAI { get; set; }
        public Nullable<System.DateTime> NGAYBD { get; set; }
        public Nullable<System.DateTime> THOIHAN { get; set; }
        public Nullable<System.DateTime> NGAYGH { get; set; }
        public string TENCV { get; set; }
        public int IDVANBAN { get; set; }
        public int IDCONGVIEC { get; set; }
        public string Subject { get; set; }
        public string FileNotation { get; set; }
        public string TypeName { get; set; }
    }
}