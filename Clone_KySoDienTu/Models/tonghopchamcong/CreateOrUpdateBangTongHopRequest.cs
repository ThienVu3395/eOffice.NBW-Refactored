using System.Collections.Generic;
using VModel;

namespace Clone_KySoDienTu.Models.Dtos.TongHopChamCong
{
    public class CreateOrUpdateBangTongHopRequest
    {
        public long? BangId { get; set; } // Nếu null là tạo mới
        public string TieuDe { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string PhongBan { get; set; }
        public string IdPhongBan { get; set; }
        public string GhiChu { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
        public List<SaveBangTongHopItemDto> Items { get; set; }
        public List<WFModel.Core_UserDto> DanhSachNguoiKy { get; set; }
        public List<WFModel.FileDinhKemViewModel> FileDinhKem { get; set; }
    }
}