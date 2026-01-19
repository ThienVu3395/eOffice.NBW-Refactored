using static VModel.LenhDieuXeModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Clone_KySoDienTu.Models.Dtos.QuanLyChamCong
{
    [XmlRoot("Users")]
    public class UserListWrapper
    {
        [XmlElement("User")]
        public List<NhanVienViewModel> Items { get; set; }
    }

    public sealed class BangTongHopDownloadRequest
    {
        public string PhongBan { get; set; }
        public int? FromMonth { get; set; }
        public int? FromYear { get; set; }
        public int? ToMonth { get; set; }
        public int? ToYear { get; set; }
        public string NguoiLapBieu { get; set; }
        public string NguoiTruongDonVi { get; set; }
        public List<NhanVienViewModel> DanhSachNhanVien { get; set; }
    }

    public sealed class BangTongHopDownloadResponse
    {
        public int RowNum { get; set; }
        public string MaNhanVien { get; set; }
        public string HoVaTen { get; set; }
        public string PhongBan { get; set; }
        public string ToChuyenMon { get; set; }
        public string ChucVu { get; set; }
        public string IdTo { get; set; }
        public string IdChucVu { get; set; }
        public int? Diem_TuTinh { get; set; }
        public string DiemText { get; set; }
        public string XepLoai { get; set; }
        public string GhiChu { get; set; }
        public int IsBold { get; set; }
        public int? PhieuId { get; set; }
        public string FileDanhGia { get; set; }
        public int? TrangThai { get; set; }
        public string TenTrangThai { get; set; }
    }
}