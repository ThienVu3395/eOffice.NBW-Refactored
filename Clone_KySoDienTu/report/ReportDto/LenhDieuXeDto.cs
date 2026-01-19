using System;

namespace Clone_KySoDienTu.Report.ReportDto
{
    public class LenhDieuXeDto
    {
        public string LoaiXe { get; set; }
        public string BienSoXe { get; set; }
        public string TenTaiXe { get; set; }
        public DateTime NgayDi { get; set; }
        public DateTime NgayTao { get; set; }
        public string DiemBatDau { get; set; }
        public string DiemKetThuc { get; set; }
        public Int64 SoKilometDi { get; set; }
        public Int64 SoKilometVe { get; set; }
        public decimal SoGioThiCong { get; set; } = 0;
        public Int64 TongCong { get; set; }
        public string GhiChu { get; set; }
        public string TenLoaiCongTac { get; set; }
        public bool coChuKySo { get; set; } = false;
    }
}