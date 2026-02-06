namespace Clone_KySoDienTu.Models.Dtos.QuanLyChamCong
{
    // row DB để map nhanh
    public sealed class CriteriaRow
    {
        public int TieuChiId { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public bool IsParent { get; set; }
        public int? Score { get; set; }       // điểm chuẩn
        public int? Diem { get; set; }        // kết quả đã chấm (theo Rater)
        public string GhiChu { get; set; }    // ghi chú
    }
}