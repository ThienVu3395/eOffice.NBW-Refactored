using System;

namespace Clone_KySoDienTu.Models.Dtos.QuanLyChamCong
{
    public sealed class PhieuDanhGiaFilterRequest : CommonParameters
    {
        public long? MauPhieuId { get; set; }
        public string MaNhanVien { get; set; }
        public decimal? MinDiem { get; set; }
        public decimal? MaxDiem { get; set; }
    }

    public sealed class PhieuDanhGiaListItemDto
    {
        public long Id { get; set; }
        public long MauPhieuId { get; set; }
        public string MauPhieuTitle { get; set; }
        public string MauPhieuCode { get; set; }
        public string AppliedFor { get; set; }
        public string DepartmentFor { get; set; }
        public string MaNhanVien { get; set; }
        public string HoVaTen { get; set; }
        public string PhongBan { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }
        public string SmartCaStringId { get; set; }
        public string NguoiPheDuyet { get; set; }
        public bool BiHuyKy { get; set; }
        public bool BiGoDuyet { get; set; }
        public decimal? Diem_TuTinh { get; set; }
        public decimal? Diem_QuanLy { get; set; }
        public decimal? Diem_Chot { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // từ COUNT() OVER() trong SP
        public int TotalCount { get; set; }
    }
}