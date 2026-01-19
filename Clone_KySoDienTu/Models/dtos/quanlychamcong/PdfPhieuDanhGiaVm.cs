using System.Collections.Generic;

namespace Clone_KySoDienTu.Models.Dtos.QuanLyChamCong
{
    public class PdfPhieuDanhGiaVm
    {
        public string HoVaTen, ChucDanh, DanhGiaThang, DonVi, MauPhieuCode, AppliedFor;
        public int TongDiemChuan; public int TongDiemTuCham; // tạm thời bỏ
        public string DiemFull, DiemTuCham, XepLoai, GhiChuFull;
        public string NgayIn, NguoiDanhGia;
        public List<PdfItem> Rows = new List<PdfItem>();
        // Dành cho FootNotes
        public bool HasFootnotes { get; set; }
        public bool HasCommonNote { get; set; }
        public List<FootnoteItem> Footnotes { get; set; } = new List<FootnoteItem>();
    }

    public class PdfItem
    {
        public string Code, Title, SubTitle, KetQua, GhiChu;
        public int? Score;
        public bool IsParent;
        public int? FnIndex { get; set; } // số thứ tự footnote nếu có ghi chú
    }

    public class FootnoteItem
    {
        public int Index { get; set; }
        public string Text { get; set; }   // “Tiêu chí 1.1: nội dung ghi chú…”,
        public bool IsCommonNote { get; set; } // dùng để phân biệt giữa ghi chú cho các tiêu chí và ghi chú chung
    }
}