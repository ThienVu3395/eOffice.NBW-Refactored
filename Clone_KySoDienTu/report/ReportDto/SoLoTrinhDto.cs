using System;
using System.Collections.Generic;

namespace Clone_KySoDienTu.Report.ReportDto
{
    public class SoLoTrinhDto
    {
        public SoLoTrinhViewModel thongTin { get; set; }

        public List<LichSu> danhSachChiTiet { get; set; }
    }

    public class SoLoTrinhViewModel
    {
        public long id { get; set; }

        public string bienSoXe { get; set; }

        public DateTime ngayTaoSoLoTrinh { get; set; }

        public int tongCongSoKm { get; set; }

        public int tieuThuNhienLieu { get; set; }

        public int tonNhienLieuDauKy { get; set; }

        public int tonNhienLieuCuoiKy { get; set; }

        public int ungTruocNhienLieu { get; set; }
    }

    public class LichSu
    {
        public DateTime ngayDi { get; set; }

        public string diemBatDau { get; set; }

        public string diemKetThuc { get; set; }

        public long soKilometDi { get; set; }

        public long soKilometVe { get; set; }

        public long tongCong { get; set; }
    }
}