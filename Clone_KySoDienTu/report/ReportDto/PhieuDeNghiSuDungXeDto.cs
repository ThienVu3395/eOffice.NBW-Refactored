using System;

namespace Clone_KySoDienTu.Report.ReportDto
{
    public class PhieuDeNghiSuDungXeDto
    {
        public string tenNhanVien { get; set; }
        public string chucVu { get; set; }
        public int soGhe { get; set; }
        public string mucDich { get; set; }
        public int soNguoi { get; set; } = 0;
        public string tenTruongDoan { get; set; } = null;
        public string soDienThoai { get; set; } = null;
        public string diemBatDau { get; set; }
        public string diemKetThuc { get; set; }
        public DateTime ngayDiDuKien { get; set; }
        public DateTime ngayVeDuKien { get; set; }
    }
}