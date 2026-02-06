using System.Collections.Generic;

namespace Clone_KySoDienTu.Models.Dtos.TongHopChamCong
{
    public class PdfBangTongHopChamCongVm
    {
        public string TieuDe { get; set; }
        public string PhongBan { get; set; }
        public string DanhGiaThang { get; set; }
        public string GhiChu { get; set; }
        public string NgayIn { get; set; }
        public string NguoiKy { get; set; }
        public string NgayLapBieu { get; set; }
        public string NguoiLapBieu { get; set; }
        public List<PdfBangTongHopRow> Rows { get; set; } = new List<PdfBangTongHopRow>();

        // Dành cho FootNotes
        public bool HasFootnotes { get; set; }
        public List<FootnoteBangTongHopItem> Footnotes { get; set; } = new List<FootnoteBangTongHopItem>();
    }

    public class PdfBangTongHopRow
    {
        public int? FnIndex { get; set; } // số thứ tự footnote nếu có ghi chú
        public int STT { get; set; }
        public string MaNhanVien { get; set; }
        public string HoTen { get; set; }
        public string ChucVu { get; set; }
        public string ToChuyenMon { get; set; }
        public string Diem_TuTinh { get; set; }
        public string XepLoai { get; set; }
        public string GhiChu { get; set; }
    }

    public class FootnoteBangTongHopItem
    {
        public int Index { get; set; }
        public string Text { get; set; }   // “Họ và tên (Mã NV): nội dung ghi chú…”
    }
}