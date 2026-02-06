using System.Collections.Generic;
using VModel;

namespace Clone_KySoDienTu.Models.Dtos.DonNghiPhep
{
    public class PdfDonNghiPhepVm
    {
        public string HoVaTen, NamSinh, SoCCCD, DonVi, ChucDanh, NoiNghiPhep, LyDoNghiPhep, YKienPhongBan, QuocGiaNghiPhep;
        public string NgayTao, NgayCapCCCD, NghiTuNgay, DenHetNgay;
        public List<WFModel.Core_UserDto> DanhSachNguoiKy;
        public string NguoiKy01, NguoiKy02;
        public int Module;
    }
}