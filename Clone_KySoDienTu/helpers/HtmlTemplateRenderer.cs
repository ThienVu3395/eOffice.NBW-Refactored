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
            // 1. Đọc HTML template
            var htmlPath = HostingEnvironment.MapPath("~/TemplateHTML/PhieuDanhGia.html");
            var html = File.ReadAllText(htmlPath, Encoding.UTF8);

            // 2. Đọc CSS (inject cho Chrome Headless)
            var commonCssPath = HostingEnvironment.MapPath("~/TemplateHTML/css/print-common.css");
            var phieuCssPath = HostingEnvironment.MapPath("~/TemplateHTML/css/phieudanhgia.css");

            var commonCss = File.ReadAllText(commonCssPath, Encoding.UTF8);
            var phieuCss = File.ReadAllText(phieuCssPath, Encoding.UTF8);

            var cssBlock = $@"
            <style>
                {commonCss}

                {phieuCss}
            </style>
            ";

            // 3. Inject CSS vào HTML
            html = html.Replace("<!-- CSS_INJECT -->", cssBlock);

            // 4. Render dữ liệu vào template (Mustache / Stubble / Render)
            return Render.StringToString(html, vm);
        }

        public static string RenderBangTongHop(PdfBangTongHopChamCongVm vm)
        {
            // 1. Đọc HTML template
            var htmlPath = HostingEnvironment.MapPath("~/TemplateHTML/BangTongHop.html");
            var html = File.ReadAllText(htmlPath, Encoding.UTF8);

            // 2. Đọc CSS (inject cho Chrome Headless)
            var commonCssPath = HostingEnvironment.MapPath("~/TemplateHTML/css/print-common.css");
            var bangCssPath = HostingEnvironment.MapPath("~/TemplateHTML/css/bangtonghop.css");

            var commonCss = File.ReadAllText(commonCssPath, Encoding.UTF8);
            var bangCss = File.ReadAllText(bangCssPath, Encoding.UTF8);

            var cssBlock = $@"
            <style>
                {commonCss}

                {bangCss}
            </style>
            ";

            // 3. Inject CSS vào HTML
            html = html.Replace("<!-- CSS_INJECT -->", cssBlock);

            // 4. Render dữ liệu vào template (Mustache / Stubble / Render)
            return Render.StringToString(html, vm);
        }

        public static string RenderDonNghiPhep(PdfDonNghiPhepVm vm)
        {
            // 0. Render tùy theo mẫu đơn nghỉ phép/giải trình
            var donCssPath = "";
            string templateName = "";
            switch (vm.Module)
            {
                case VanBanKiSo.Module.DON_NGHI_PHEP_MAU_2_TRUONG_PHONG:
                    templateName = "DonNghiPhepTruongPhong.html";
                    break;
                case VanBanKiSo.Module.DON_NGHI_PHEP_MAU_3_KHONG_LUONG:
                    templateName = "DonNghiPhepKhongLuong.html";
                    donCssPath = HostingEnvironment.MapPath("~/TemplateHTML/css/donnghiphep.css");
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

            // 1. Đọc HTML template
            var htmlPath = HostingEnvironment.MapPath($"~/TemplateHTML/{templateName}");
            var html = File.ReadAllText(htmlPath, Encoding.UTF8);

            // 2. Đọc CSS (inject cho Chrome Headless)
            var commonCssPath = HostingEnvironment.MapPath("~/TemplateHTML/css/print-dnp-common.css");

            var commonCss = File.ReadAllText(commonCssPath, Encoding.UTF8);
            var donCss = donCssPath != "" ? File.ReadAllText(donCssPath, Encoding.UTF8) : "";

            var cssBlock = "";
            if (donCss != "")
            {
                cssBlock = $@"
                <style>
                    {commonCss}

                    {donCss}
                </style>
                ";
            }
            else
            {
                cssBlock = $@"
                <style>
                    {commonCss}
                </style>
                ";
            }

            // 3. Inject CSS vào HTML
            html = html.Replace("<!-- CSS_INJECT -->", cssBlock);
            vm.NguoiKy01 = vm.DanhSachNguoiKy[0].FullName;
            vm.NguoiKy02 = vm.DanhSachNguoiKy[1].FullName ?? vm.DanhSachNguoiKy[0].FullName;
            return Render.StringToString(html, vm);
        }
    }
}