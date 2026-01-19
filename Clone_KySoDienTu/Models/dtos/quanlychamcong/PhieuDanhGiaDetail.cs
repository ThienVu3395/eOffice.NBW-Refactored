using System;
using System.Collections.Generic;
using VModel;

namespace Clone_KySoDienTu.Models.Dtos.QuanLyChamCong
{
    public sealed class PhieuDanhGiaDetail
    {
        public sealed class PhieuDanhGiaHeaderDto
        {
            public long PhieuId { get; set; }
            public int MauPhieuId { get; set; }
            public string MaNhanVien { get; set; }
            public string HoVaTen { get; set; }
            public string PhongBan { get; set; }
            public string ChucVu { get; set; }
            public string ToChuyenMon { get; set; }
            public byte Month { get; set; }
            public short Year { get; set; }
            public int? TrangThai { get; set; }
            public string GhiChu { get; set; }
            public int? Diem_TuTinh { get; set; }
            public int? Diem_QuanLy { get; set; }
            public int? Diem_Chot { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? CreatedAt { get; set; }
            public string UpdatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public string SmartCaStringId { get; set; }
            public string NguoiDuyet { get; set; }

            public string MauPhieuTitle { get; set; }
            public string MauPhieuCode { get; set; }
            public int? ScoreDefined { get; set; }
            public int ScoreLeafSum { get; set; }
            public int Sum_TuTinh { get; set; }
            public int Sum_QuanLy { get; set; }
            public int Sum_Chot { get; set; }

            // Extend Fields
            public string KySoID { get; set; }
            public string ThuTuKySo { get; set; }
        }

        public sealed class PhieuDanhGiaRowDto
        {
            public int TieuChiId { get; set; }
            public int? ParentId { get; set; }
            public int ViewOrder { get; set; }
            public string Code { get; set; }
            public string Title { get; set; }
            public string SubTitle { get; set; }
            public bool IsParent { get; set; }
            public int Level { get; set; }
            public bool HasChildren { get; set; }
            public int SumChildScore { get; set; }
            public int? Score { get; set; }
            public int? Diem_TuTinh { get; set; }
            public int? Diem_QuanLy { get; set; }
            public int? Diem_Chot { get; set; }
            public string GhiChu { get; set; }
        }

        public sealed class PhieuDanhGiaHistoryDto
        {
            public long Id { get; set; }
            public long PhieuId { get; set; }
            public string FilePath { get; set; }
            public DateTime CreatedOn { get; set; }
            public string CreatedBy { get; set; }
            public string LyDo { get; set; }
            public string LoaiThayDoi { get; set; }
            public byte? TrangThaiCu { get; set; }
            public bool DaXuLy { get; set; }
        }

        public sealed class PhieuDanhGiaDetailResponse
        {
            public PhieuDanhGiaHeaderDto Header { get; set; }
            public List<PhieuDanhGiaRowDto> Rows { get; set; }
            public List<WFModel.FileDinhKemViewModel> Files { get; set; }
            public List<WFModel.Core_UserDto> Signers { get; set; }
            public List<PhieuDanhGiaHistoryDto> Histories { get; set; }
        }
    }
}