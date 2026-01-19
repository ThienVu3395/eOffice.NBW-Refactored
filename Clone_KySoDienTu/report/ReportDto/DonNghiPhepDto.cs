using System;
using System.Collections.Generic;
using static VModel.WFModel;

namespace Clone_KySoDienTu.Report.ReportDto
{
    public class DonNghiPhepDto
    {
        public DateTime ngayTao { get; set; }
        public string hoTen { get; set; }
        public int namSinh { get; set; }
        public string soCCCD { get; set; }
        public string ngayCap { get; set; }
        public string phongBan { get; set; }
        public string chucVu { get; set; }
        public DateTime nghiTuNgay { get; set; }
        public DateTime denHetNgay { get; set; }
        public string noiNghiPhep { get; set; }
        public string lyDoNghiPhep { get; set; }
        public string yKien { get; set; }
        public string quocGia { get; set; }
        public bool coChuKySo { get; set; } = false;
        public string tenNguoiKy01 { get; set; }
        public string tenNguoiKy02 { get; set; }
    }
}