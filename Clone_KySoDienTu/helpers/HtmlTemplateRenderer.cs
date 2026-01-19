using Nustache.Core;
using Clone_KySoDienTu.Models.Dtos.DonNghiPhep;
using Clone_KySoDienTu.Models.Dtos.QuanLyChamCong;
using Clone_KySoDienTu.Models.Dtos.TongHopChamCong;
using Clone_KySoDienTu.SystemConstants;
using System.IO;
using System.Text;
using System.Web.Hosting;

namespace Clone_KySoDienTu.Helpers
{
    public static class HtmlTemplateRenderer
    {
        public static string RenderPhieuDanhGia(PdfPhieuDanhGiaVm vm)
        {
            var path = HostingEnvironment.MapPath("~/TemplateHTML/PhieuDanhGia.html");
            var tpl = File.ReadAllText(path, Encoding.UTF8);
            return Render.StringToString(tpl, vm);
        }

        public static string RenderBangTongHop(PdfBangTongHopChamCongVm vm)
        {
            var path = HostingEnvironment.MapPath("~/TemplateHTML/BangTongHop.html");
            var tpl = File.ReadAllText(path, Encoding.UTF8);
            return Render.StringToString(tpl, vm);
        }

        public static string RenderDonNghiPhep(PdfDonNghiPhepVm vm)
        {
            string templateName = "";
            switch (vm.Module)
            {
                case VanBanKiSo.Module.DON_NGHI_PHEP_MAU_2_TRUONG_PHONG:
                    templateName = "DonNghiPhepTruongPhong.html";
                    break;
                case VanBanKiSo.Module.DON_NGHI_PHEP_MAU_3_KHONG_LUONG:
                    templateName = "DonNghiPhepKhongLuong.html";
                    break;
                case VanBanKiSo.Module.DON_NGHI_PHEP_MAU_4_DI_NUOC_NGOAI:
                    templateName = "DonNghiPhepDiNuocNgoai.html";
                    break;
                case VanBanKiSo.Module.DON_GIAI_TRINH_MAU_5_NHAN_VIEN:
                    templateName = "DonGiaiTrinhNhanVien.html";
                    break;
                case VanBanKiSo.Module.DON_GIAI_TRINH_MAU_6_TRUONG_PHONG:
                    templateName = "DonGiaiTrinhTruongPhong.html";
                    break;
                default:
                    templateName = "DonNghiPhepNhanVien.html";
                    break;
            }
            vm.NguoiKy01 = vm.DanhSachNguoiKy[0].FullName;
            vm.NguoiKy02 = vm.DanhSachNguoiKy[1].FullName ?? vm.DanhSachNguoiKy[0].FullName;
            var path = HostingEnvironment.MapPath($"~/TemplateHTML/{templateName}");
            var tpl = File.ReadAllText(path, Encoding.UTF8);
            return Render.StringToString(tpl, vm);
        }
    }
}