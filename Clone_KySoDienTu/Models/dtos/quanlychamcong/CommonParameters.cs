using System;

namespace Clone_KySoDienTu.Models.Dtos.QuanLyChamCong
{
    public class CommonParameters
    {
        // Những biến dùng chung để đẩy xuống SP
        public string PhongBan { get; set; }
        public string NguoiPheDuyet { get; set; }
        public string LoaiLoc { get; set; }

        // Thông tin người đang truy cập
        public string UserNameNguoiDangTruyCap { get; set; }
        public string PhongBanNguoiDangTruyCap { get; set; }
        public string ToChuyenMonNguoiDangTruyCap { get; set; }
        public string ChucVuNguoiDangTruyCap { get; set; }
        public string MaNhanVienNguoiDangTruyCap { get; set; }

        // khoảng tháng–năm
        public int? FromMonth { get; set; }   // 1..12
        public int? FromYear { get; set; }   // yyyy
        public int? ToMonth { get; set; }
        public int? ToYear { get; set; }

        // searching
        public DateTime? FromCreatedAt { get; set; }
        public DateTime? ToCreatedAt { get; set; }
        public string Keyword { get; set; }

        // paging + sorting
        public int PageNumber { get; set; } = 1;      // 1-based
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedAt"; // Id | MauPhieuId | ... | Diem_Chot | CreatedAt | UpdatedAt
        public string SortDir { get; set; } = "DESC";       // ASC | DESC
    }
}