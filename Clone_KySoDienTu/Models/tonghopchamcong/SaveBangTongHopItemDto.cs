namespace Clone_KySoDienTu.Models.Dtos.TongHopChamCong
{
    public class SaveBangTongHopItemDto
    {
        public string MaNhanVien { get; set; }
        public string HoTen { get; set; }
        public string ChucVu { get; set; }
        public string ToChuyenMon { get; set; }
        public string IdChucVu { get; set; }
        public string IdToChuyenMon { get; set; }
        public int? Diem_TuTinh { get; set; }
        public string XepLoai { get; set; }
        public string GhiChu { get; set; }
        public long? PhieuId { get; set; }  // link đến bảng PhieuDanhGia nếu có
        public int ViewOrder { get; set; }
    }
}