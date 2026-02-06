using System.Collections.Generic;
using VModel;

namespace Clone_KySoDienTu.Models.Dtos.QuanLyChamCong
{
    public class CreateOrUpdatePhieuDanhGiaRequest
    {
        public long? PhieuId { get; set; }          // nếu có -> update, nếu null -> tạo mới (UpdateOrCreate)

        public short Year { get; set; }
        public byte Month { get; set; }
        public int MauPhieuId { get; set; }
        public string MauPhieuCode { get; set; }

        public string MaNhanVien { get; set; }
        public string HoVaTen { get; set; }
        public string PhongBan { get; set; }
        public string ChucVu { get; set; }
        public string ToChuyenMon { get; set; }
        public string IdPhongBan { get; set; }
        public string IdChucVu { get; set; }
        public string IdToChuyenMon { get; set; }
        public string CreatedBy { get; set; }

        public byte Rater { get; set; }             // 1 = NV tự chấm; 2 = Quản lý; 3 = Chốt
        public int DiemFull { get; set; }          // điểm tổng của mẫu phiếu (tự điền)
        public int DiemTuCham { get; set; }        // điểm tự chấm
        public string XepLoai { get; set; }        // xếp loại
        public string GhiChuFull { get; set; }     // ghi chú tổng của mẫu phiếu (tự điền)
        public bool Submit { get; set; }           // false = Lưu nháp (TrangThai = 2), true = Nộp (TrangThai = 1)

        public List<SaveItemDto> Items { get; set; } = new List<SaveItemDto>();

        public List<WFModel.FileDinhKemViewModel> FileDinhKem { get; set; }

        public List<WFModel.Core_UserDto> DanhSachNguoiKy { get; set; }
    }
}