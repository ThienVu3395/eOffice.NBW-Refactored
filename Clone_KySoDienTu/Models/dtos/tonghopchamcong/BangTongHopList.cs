using Clone_KySoDienTu.Models.Dtos.QuanLyChamCong;
using System;

namespace Clone_KySoDienTu.Models.Dtos.TongHopChamCong
{
    public sealed class BangTongHopFilterRequest : CommonParameters
    {
        public long? BangTongHopId { get; set; }
    }

    public sealed class BangTongHopListItemDto
    {
        public long Id { get; set; }
        public string TieuDe { get; set; }
        public string PhongBan { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }
        public string SmartCaStringId { get; set; }
        public string NguoiPheDuyet { get; set; }
        public bool BiHuyKy { get; set; }
        public bool BiGoDuyet { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalCount { get; set; }
    }
}