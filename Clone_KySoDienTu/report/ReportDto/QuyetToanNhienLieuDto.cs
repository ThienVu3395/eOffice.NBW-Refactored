using System;

namespace Clone_KySoDienTu.Report.ReportDto
{
    public class QuyetToanNhienLieuDto
    {
        public Int64 id { get; set; }
        public string bienSoXe { get; set; }
        public int soChuyen { get; set; }
        public int soKM { get; set; } = 0;
        public int dinhMuc { get; set; } = 0;
        public int tonThangTruoc { get; set; }
        public int tonCuoiThang { get; set; }
        public int ungThangHienTai { get; set; }
        public int doXangBenNgoai { get; set; }
        public int tieuThuDau { get; set; }
        public int tieuThuXang { get; set; }
        public int noMayXe { get; set; }
        public Int64 IdQuyetToanNhienLieu { get; set; }
        public decimal soGioThiCong { get; set; } = 0;
        public int dinhMucTheoGio { get; set; } = 0;
        public long tongSoKm { get; set; } = 0;
    }
}