using Clone_KySoDienTu.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;
using Dapper;
using static VModel.LenhDieuXeModel;
using System.Linq;
using Clone_KySoDienTu.Service.Dtos;

namespace Clone_KySoDienTu.Controllers
{
    public class VanBanKiSoController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        #region Lệnh điều xe
        public void GetListLenhDieuXe(SoLoTrinhResponse model) // dùng hiển thị danh sách lệnh điều xe lên sổ lộ trình
        {
            ReportParamsWithMultipleValue<Report.ReportDto.LenhDieuXeDto> objReportParams = new ReportParamsWithMultipleValue<Report.ReportDto.LenhDieuXeDto>();
            List<Report.ReportDto.LenhDieuXeDto> listLDX = new List<Report.ReportDto.LenhDieuXeDto>();
            foreach (LichSu item in model.danhSachChiTiet)
            {
                Report.ReportDto.LenhDieuXeDto obj = new Report.ReportDto.LenhDieuXeDto
                {
                    NgayDi = (DateTime)(item.ngayDi == null ? DateTime.Now : item.ngayDi),
                    DiemBatDau = item.diemBatDau,
                    DiemKetThuc = item.diemKetThuc,
                    SoKilometDi = (int)(item.soKilometDi == null ? 0 : item.soKilometDi),
                    SoKilometVe = (int)(item.soKilometVe == null ? 0 : item.soKilometVe),
                    TongCong = (int)(item.tongCong == null ? 0 : item.tongCong),
                };
                listLDX.Add(obj);
            }
            objReportParams.DataSource = listLDX;
            objReportParams.IDCR = (long)model.thongTin.id;
            objReportParams.Loai = (int)model.thongTin.loai;
            objReportParams.valstring1 = model.thongTin.bienSoXe;
            objReportParams.valstring2 = "Tháng " + model.thongTin.ngayTaoSoLoTrinh.Month + " năm " + model.thongTin.ngayTaoSoLoTrinh.Year;
            objReportParams.valint1 = model.thongTin.tongCongSoKm;
            objReportParams.valint2 = model.thongTin.tieuThuNhienLieu;
            objReportParams.valint3 = model.thongTin.tonNhienLieuDauKy;
            objReportParams.valint4 = model.thongTin.tonNhienLieuCuoiKy;
            objReportParams.valint5 = model.thongTin.ungTruocNhienLieu;
            if(model.thongTin.coChuKySo == true) // đã có chữ ký số => không cần ký bằng Crystal Report
            {
                objReportParams.ReportFileName = "SoLoTrinh_KhongKyNhay.rpt";
            }
            else // chưa có chữ ký số => cần ký bằng Crystal Report
            {
                string _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var thongtin = db.Query<Core_UserDto>("Core_GetUserExtension_NEW", new { @username = model.thongTin.nguoiTao }, null, true, null, CommandType.StoredProcedure).SingleOrDefault();
                    if(thongtin != null)
                    {
                        objReportParams.valstring3 = thongtin.FullName;
                        objReportParams.valstring4 = thongtin.CHUCVU;
                        objReportParams.valstring5 = thongtin.MAPHONG;
                        objReportParams.valstring6 = model.thongTin.ngayTaoSoLoTrinh.Day + "/" + model.thongTin.ngayTaoSoLoTrinh.Month + "/" + model.thongTin.ngayTaoSoLoTrinh.Year;
                    }
                }
                objReportParams.ReportFileName = "SoLoTrinh_CoKyNhay.rpt";
            }
            objReportParams.ReportTitle = "SoLoTrinh";
            this.HttpContext.Session["ReportType"] = "SoLoTrinhReport";
            this.HttpContext.Session["ReportParam"] = objReportParams;
        }

        public void GetDetailPhieuDeNghiSuDungXe(PhieuXinXeViewModel model) // dùng hiển thị chi tiết phiếu đề nghị sử dụng xe
        {
            ReportParams<Report.ReportDto.PhieuDeNghiSuDungXeDto> objReportParams = new ReportParams<Report.ReportDto.PhieuDeNghiSuDungXeDto>();
            List<Report.ReportDto.PhieuDeNghiSuDungXeDto> listLDX = new List<Report.ReportDto.PhieuDeNghiSuDungXeDto>();
            Report.ReportDto.PhieuDeNghiSuDungXeDto obj = new Report.ReportDto.PhieuDeNghiSuDungXeDto
            {
                tenNhanVien = model.tenNhanVien,
                chucVu = model.chucVu,
                soGhe = model.soGhe,
                ngayDiDuKien = (DateTime)(model.ngayDiDuKien == null ? DateTime.Now : model.ngayDiDuKien),
                ngayVeDuKien = (DateTime)(model.ngayVeDuKien == null ? DateTime.Now : model.ngayVeDuKien),
                mucDich = model.mucDich,
                soNguoi = model.soNguoi,
                tenTruongDoan = model.tenTruongDoan,
                soDienThoai = model.soDienThoai,
                diemBatDau = model.diemBatDau,
                diemKetThuc = model.diemKetThuc
            };
            listLDX.Add(obj);
            objReportParams.DataSource = listLDX;
            objReportParams.IDCR = (long)model.id;
            objReportParams.Loai = (int)model.loai;
            objReportParams.ReportFileName = "PhieuDeNghiSuDungXe.rpt";
            objReportParams.ReportTitle = "PhieuDeNghiSuDungXe";
            this.HttpContext.Session["ReportType"] = "PhieuDeNghiSuDungXeReport";
            this.HttpContext.Session["ReportParam"] = objReportParams;
        }

        public void GetDetailLenhDieuXe(LenhDieuXeViewModel model) // dùng hiển thị chi tiết lệnh điều xe
        {
            ReportParams<Report.ReportDto.LenhDieuXeDto> objReportParams = new ReportParams<Report.ReportDto.LenhDieuXeDto>();
            List<Report.ReportDto.LenhDieuXeDto> listLDX = new List<Report.ReportDto.LenhDieuXeDto>();
            Report.ReportDto.LenhDieuXeDto obj = new Report.ReportDto.LenhDieuXeDto
            {
                LoaiXe = model.tenHangXe,
                BienSoXe = model.bienSoXe,
                TenTaiXe = model.tenTaiXe,
                NgayDi = (DateTime)(model.ngayDi == null ? DateTime.Now : model.ngayDi),
                NgayTao = (DateTime)(model.ngayTao == null ? DateTime.Now : model.ngayTao),
                DiemBatDau = model.diemBatDau,
                DiemKetThuc = model.diemKetThuc,
                TenLoaiCongTac = model.tenLoaiCongTac,
                GhiChu = model.ghiNhanCongTac
            };
            listLDX.Add(obj);
            objReportParams.DataSource = listLDX;
            objReportParams.IDCR = (long)model.id;
            objReportParams.Loai = (int)model.loai;
            objReportParams.ReportFileName = "LenhDieuXe1.rpt";
            objReportParams.ReportTitle = "LenhDieuXe";
            this.HttpContext.Session["ReportType"] = "LenhDieuXeReport";
            this.HttpContext.Session["ReportParam"] = objReportParams;
        }

        public void GetDetailQuanLyLenhDieuXe(LenhDieuXeViewModel model) // dùng hiển thị chi tiết quản lý lệnh điều xe
        {
            ReportParams<Report.ReportDto.LenhDieuXeDto> objReportParams = new ReportParams<Report.ReportDto.LenhDieuXeDto>();
            List<Report.ReportDto.LenhDieuXeDto> listLDX = new List<Report.ReportDto.LenhDieuXeDto>();
            Report.ReportDto.LenhDieuXeDto obj = new Report.ReportDto.LenhDieuXeDto
            {
                LoaiXe = model.tenHangXe,
                BienSoXe = model.bienSoXe,
                TenTaiXe = model.tenTaiXe,
                NgayDi = (DateTime)(model.ngayDi == null ? DateTime.Now : model.ngayDi),
                NgayTao = (DateTime)(model.ngayTaoHeThong == null ? DateTime.Now : model.ngayTaoHeThong),
                DiemBatDau = model.diemBatDau,
                DiemKetThuc = model.diemKetThuc,
                SoKilometDi = (int)(model.soKilometDi == null ? 0 : model.soKilometDi),
                SoKilometVe = (int)(model.soKilometVe == null ? 0 : model.soKilometVe),
                SoGioThiCong = (int)(model.soGioThiCong == null ? 0 : model.soGioThiCong),
                TongCong = (int)(model.tongCong == null ? 0 : model.tongCong),
                TenLoaiCongTac = model.tenLoaiCongTac,
                GhiChu = model.ghiNhanCongTac
            };
            listLDX.Add(obj);
            objReportParams.DataSource = listLDX;
            objReportParams.IDCR = (long)model.id;
            objReportParams.Loai = (int)model.loai;
            objReportParams.ReportFileName = "PhieuQuanLyXe.rpt";
            objReportParams.ReportTitle = "PhieuQuanLyXe";
            this.HttpContext.Session["ReportType"] = "PhieuQuanLyXeReport";
            this.HttpContext.Session["ReportParam"] = objReportParams;
        }

        public void GetListQuyetToanNhienLieu(QuyetToanNhienLieuResponse model) // dùng hiển thị danh sách quyết toán nhiên liệu
        {
            ReportParamsWithMultipleValue<Report.ReportDto.QuyetToanNhienLieuDto> objReportParams = new ReportParamsWithMultipleValue<Report.ReportDto.QuyetToanNhienLieuDto>();
            List<Report.ReportDto.QuyetToanNhienLieuDto> listLDX = new List<Report.ReportDto.QuyetToanNhienLieuDto>();
            foreach (ChiTietQuyetToanNhienLieu item in model.danhSachChiTiet)
            {
                Report.ReportDto.QuyetToanNhienLieuDto obj = new Report.ReportDto.QuyetToanNhienLieuDto
                {
                    bienSoXe = item.bienSoXe,
                    soChuyen = item.soChuyen,
                    soKM = (int)(item.soKM == null ? 0 : item.soKM),
                    dinhMuc = (int)(item.dinhMuc == null ? 0 : item.dinhMuc),
                    tonThangTruoc = item.tonThangTruoc,
                    ungThangHienTai = item.ungThangHienTai,
                    tieuThuDau = item.tieuThuDau,
                    tieuThuXang = item.tieuThuXang,
                    noMayXe = item.noMayXe,
                    tonCuoiThang = item.tonCuoiThang,
                    soGioThiCong = (decimal)(item.SoGioThiCong == null ? 0 : item.SoGioThiCong),
                    dinhMucTheoGio = (int)(item.DinhMucTheoGio == null ? 0 : item.DinhMucTheoGio),
                    tongSoKm = (int)(item.TongSoKm == null ? 0 : item.TongSoKm)
                };
                listLDX.Add(obj);
            }
            objReportParams.DataSource = listLDX;
            objReportParams.IDCR = (long)model.thongTin.id;
            objReportParams.Loai = (int)model.thongTin.loai;
            objReportParams.valstring1 = "TPHCM, ngày " + model.thongTin.ngayTao?.Day + " tháng " + model.thongTin.ngayTao?.Month + " năm " + model.thongTin.ngayTao?.Year;
            objReportParams.valstring2 = "THÁNG " + model.thongTin.ngayTao?.Month + "/" + model.thongTin.ngayTao?.Year;
            objReportParams.ReportFileName = "";
            switch (model.thongTin.loai)
            {
                case 13: // P.TCHC
                    objReportParams.ReportFileName = "BangQuyetToanNhienLieuPTCHC.rpt";
                    objReportParams.ReportTitle = "BangQuyetToanNhienLieuPTCHC";
                    this.HttpContext.Session["ReportType"] = "BangQuyetToanNhienLieuPTCHCReport";
                    break;
                case 14: // P.KD
                    objReportParams.ReportFileName = "BangQuyetToanNhienLieuPKD.rpt";
                    objReportParams.ReportTitle = "BangQuyetToanNhienLieuPKD";
                    this.HttpContext.Session["ReportType"] = "BangQuyetToanNhienLieuPKDReport";
                    break;
                case 15: // P.QLMLCN1
                    if(model.thongTin.coChuKySo == true)
                    {
                        objReportParams.ReportFileName = "BangQuyetToanNhienLieuPTCTB_KhongKyNhay.rpt";
                    }
                    else
                    {
                        string _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
                        using (IDbConnection db = new SqlConnection(_cnn))
                        {
                            if (db.State == System.Data.ConnectionState.Closed)
                                db.Open();
                            var thongtin = db.Query<Core_UserDto>("Core_GetUserExtension_NEW", new { @username = model.thongTin.nguoiTao }, null, true, null, CommandType.StoredProcedure).SingleOrDefault();
                            if (thongtin != null)
                            {
                                objReportParams.valstring3 = thongtin.FullName;
                                objReportParams.valstring4 = thongtin.CHUCVU;
                                objReportParams.valstring5 = thongtin.MAPHONG;
                                objReportParams.valstring6 = model.thongTin.ngayTao?.Day + "/" + model.thongTin.ngayTao?.Month + "/" + model.thongTin.ngayTao?.Year;
                            }
                        }
                        objReportParams.ReportFileName = "BangQuyetToanNhienLieuPTCTB_CoKyNhay.rpt";
                    }
                    objReportParams.ReportTitle = "BangQuyetToanNhienLieuPTCTB";
                    this.HttpContext.Session["ReportType"] = "BangQuyetToanNhienLieuPTCTBReport";
                    break;
                default: // ra loại lỗi
                    objReportParams.ReportFileName = "EmployeeReport.rpt";
                    objReportParams.ReportTitle = "EmployeeReport";
                    this.HttpContext.Session["ReportType"] = "EmployeeFormReport";
                    break;
            }
            this.HttpContext.Session["ReportParam"] = objReportParams;
        }
        #endregion

        #region Đơn nghỉ phép
        public void GetDetailDonNghiPhep(DonNghiPhepViewModel model) // dùng hiển thị chi tiết nghỉ phép
        {
            ReportParams<Report.ReportDto.DonNghiPhepDto> objReportParams = new ReportParams<Report.ReportDto.DonNghiPhepDto>();
            List<Report.ReportDto.DonNghiPhepDto> listLDX = new List<Report.ReportDto.DonNghiPhepDto>();
            Report.ReportDto.DonNghiPhepDto obj = new Report.ReportDto.DonNghiPhepDto
            {
                ngayTao = model.ngayTao,
                hoTen = model.tenNhanVien,
                namSinh = model.namSinh,
                soCCCD = model.soCccd,
                ngayCap = model.ngayCapCccd.Day + "/" + model.ngayCapCccd.Month + "/" + model.ngayCapCccd.Year,
                phongBan = model.phongBan,
                chucVu = model.chucVu,
                nghiTuNgay = model.tuNgay,
                denHetNgay = model.denNgay,
                noiNghiPhep = model.noiNghiPhep,
                lyDoNghiPhep = model.lyDo,
                yKien = model.yKienPhongBanDoi,
                quocGia = model.quocGia,
                coChuKySo = model.coChuKySo,
                tenNguoiKy01 = model.DanhSachNguoiKy.Count() == 1 && (model.loai == 7 || model.loai == 8 || model.loai == 9) ? "Giám đốc phụ trách trực tiếp" : model.DanhSachNguoiKy[0].FullName, // dành cho những trưởng P/B do gđ phụ trách trực tiếp làm đơn nghỉ phép
                tenNguoiKy02 = model.DanhSachNguoiKy.Count() == 1 ? model.DanhSachNguoiKy[0].FullName : model.DanhSachNguoiKy[1].FullName,
            };
            listLDX.Add(obj);
            objReportParams.DataSource = listLDX;
            objReportParams.IDCR = (long)model.id;
            objReportParams.Loai = (int)model.loai;
            objReportParams.ReportFileName = "";
            objReportParams.ReportTitle = "";
            this.HttpContext.Session["ReportType"] = "";
            switch (model.loai)
            {
                case 6: // Đơn xin nghỉ phép thường niên trong nước mẫu 1
                    objReportParams.ReportFileName = "DonXinNghiPhepThuongNienTrongNuocMau01";
                    objReportParams.ReportTitle = "DonXinNghiPhepThuongNienTrongNuocMau01";
                    this.HttpContext.Session["ReportType"] = "DonXinNghiPhepThuongNienTrongNuocReport";
                    break;
                case 7: // Đơn xin nghỉ phép thường niên trong nước mẫu 2
                    objReportParams.ReportFileName = "DonXinNghiPhepThuongNienTrongNuocMau02";
                    objReportParams.ReportTitle = "DonXinNghiPhepThuongNienTrongNuocMau02";
                    this.HttpContext.Session["ReportType"] = "DonXinNghiPhepThuongNienTrongNuocReport";
                    break;
                case 8: // Đơn xin nghỉ việc riêng không lương 
                    objReportParams.ReportFileName = "DonXinNghiPhepKhongLuong";
                    objReportParams.ReportTitle = "DonXinNghiPhepKhongLuong";
                    this.HttpContext.Session["ReportType"] = "DonXinNghiPhepKhongLuongReport";
                    break;
                case 9: // Đơn xin nghỉ phép đi nước ngoài
                    objReportParams.ReportFileName = "DonXinNghiPhepDiNuocNgoai";
                    objReportParams.ReportTitle = "DonXinNghiPhepDiNuocNgoai";
                    this.HttpContext.Session["ReportType"] = "DonXinNghiPhepDiNuocNgoaiReport";
                    break;
                case 28: // Đơn giải trình k lấy face id mẫu 5
                    objReportParams.ReportFileName = "DonXinGiaiTrinhKhongLayFaceIDMau01";
                    objReportParams.ReportTitle = "DonXinGiaiTrinhKhongLayFaceIDMau01";
                    this.HttpContext.Session["ReportType"] = "DonXinGiaiTrinhKhongLayFaceIDMau01Report";
                    break;
                case 29: // Đơn giải trình k lấy face id mẫu 6
                    objReportParams.ReportFileName = "DonXinGiaiTrinhKhongLayFaceIDMau02";
                    objReportParams.ReportTitle = "DonXinGiaiTrinhKhongLayFaceIDMau02";
                    this.HttpContext.Session["ReportType"] = "DonXinGiaiTrinhKhongLayFaceIDMau02Report";
                    break;
                default: // ra loại lỗi
                    objReportParams.ReportFileName = "EmployeeReport.rpt";
                    objReportParams.ReportTitle = "EmployeeReport";
                    this.HttpContext.Session["ReportType"] = "EmployeeFormReport";
                    break;
            }
            if(model.coChuKySo == true && objReportParams.ReportTitle != "EmployeeReport") // người kí đã có chữ ký số => k cần phải kí nháy = CR
            {
                objReportParams.ReportFileName = objReportParams.ReportFileName + "_KhongKyNhay.rpt";
            }
            else if (model.coChuKySo == false && objReportParams.ReportTitle != "EmployeeReport") // người kí không có chữ ký số => cần phải kí nháy = CR
            {
                objReportParams.ReportFileName = objReportParams.ReportFileName + "_CoKyNhay.rpt";
            }
            this.HttpContext.Session["ReportParam"] = objReportParams;
        }
        #endregion
    }
}