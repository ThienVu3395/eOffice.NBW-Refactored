using System;
using System.Collections.Generic;
using VModel;

namespace Clone_KySoDienTu.Models.Dtos.TongHopChamCong
{
    public class BangTongHopDetail
    {
        public sealed class BangTongHopHeaderDto
        {
            public long BangTongHopId { get; set; }
            public string TieuDe { get; set; }
            public string PhongBan { get; set; }
            public string IdPhongBan { get; set; }
            public int Month { get; set; }
            public int Year { get; set; }
            public int TrangThai { get; set; }
            public string GhiChu { get; set; }
            public string SmartCaStringId { get; set; }
            public string NguoiDuyet { get; set; }
            public string KySoID { get; set; }
            public string ThuTuKySo { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? CreatedAt { get; set; }
            public string UpdatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public sealed class BangTongHopChiTietRowDto
        {
            public long Id { get; set; }
            public long BangTongHopId { get; set; }
            public string MaNhanVien { get; set; }
            public string HoVaTen { get; set; }
            public string ToChuyenMon { get; set; }
            public string ChucVu { get; set; }
            public string IdToChuyenMon { get; set; }
            public string IdChucVu { get; set; }
            public int? DiemText { get; set; }
            public string XepLoai { get; set; }
            public long? PhieuId { get; set; }
            public string GhiChu { get; set; }
            public string FileDanhGia { get; set; }
            public int? TrangThai { get; set; }
            public string TenTrangThai { get; set; }
        }

        public sealed class BangTongHopHistoryDto
        {
            public long Id { get; set; }
            public long BangId { get; set; }
            public string FilePath { get; set; }
            public DateTime CreatedOn { get; set; }
            public string CreatedBy { get; set; }
            public string LyDo { get; set; }
            public string LoaiThayDoi { get; set; }
            public byte? TrangThaiCu { get; set; }
            public bool DaXuLy { get; set; }
        }

        public sealed class BangTongHopDetailResponse
        {
            public BangTongHopHeaderDto Header { get; set; }
            public List<BangTongHopChiTietRowDto> Rows { get; set; }
            public List<WFModel.FileDinhKemViewModel> Files { get; set; }
            public List<WFModel.Core_UserDto> Signers { get; set; }
            public List<BangTongHopHistoryDto> Histories { get; set; }
        }
    }
}