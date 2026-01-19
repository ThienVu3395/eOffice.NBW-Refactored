using Clone_KySoDienTu.MyHub;
using static VModel.WFModel;
using static VModel.LenhDieuXeModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Web.Http;
using VModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Dapper;
using System.Linq;
using System.Configuration;
using Clone_KySoDienTu.Helpers;
using static Clone_KySoDienTu.SystemConstants.VanBanKiSo;
using static Clone_KySoDienTu.Helpers.HtmlTemplateRenderer;
using System.Web.Hosting;
using Clone_KySoDienTu.Models.Dtos.DonNghiPhep;
using SteProject;

namespace Clone_KySoDienTu.Controllers.API.QuanLyNghiPhep
{
    [Authorize]
    [RoutePrefix("api/QLNghiPhep")]
    public class QuanLyNghiPhepAPIController : EntitySetControllerWithHub<EventHub>
    {
        private readonly string _cnn;

        private readonly string _endpoint;

        private VCode.LenhDieuXeModule vc;

        public QuanLyNghiPhepAPIController()
        {
            _cnn = ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
            _endpoint = ConfigurationManager.AppSettings["EndpointVanBanKySo"];
            vc = new VCode.LenhDieuXeModule(_cnn);
        }

        #region Helpers
        public string moveFile(string TenFile, string LoaiFile)
        {
            try
            {
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/");
                string user = User.Identity.Name;
                sPath += user + @"\";
                DateTime ngaytao = DateTime.Now;
                if (Directory.Exists(sPath))
                {
                    if (File.Exists(Path.Combine(sPath, TenFile)))
                    {
                        FileInfo fs = new FileInfo(Path.Combine(sPath, TenFile));

                        string savePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                        if (Directory.Exists(savePath))
                        {
                            savePath = Path.Combine(savePath, ngaytao.Year.ToString());
                            if (!Directory.Exists(savePath))
                            {
                                Directory.CreateDirectory(savePath);
                            }
                            savePath = Path.Combine(savePath, ngaytao.ToString("MM"));
                            if (!Directory.Exists(savePath))
                            {
                                Directory.CreateDirectory(savePath);
                            }
                            Guid obj = Guid.NewGuid();
                            string newName = obj.ToString() + "." + LoaiFile;
                            string newPath = Path.Combine(savePath, newName);
                            string old1 = Path.Combine(savePath, TenFile);

                            if (File.Exists(old1))
                            {
                                FileInfo del = new FileInfo(old1);
                                del.Delete();
                            }
                            fs.MoveTo(old1);
                            File.Move(old1, newPath);

                            if (Path.GetExtension(TenFile) == ".docx")
                            {
                                string TENFILE2 = Path.ChangeExtension(TenFile, "pdf");
                                FileInfo fs2 = new FileInfo(Path.Combine(sPath, TENFILE2));
                                string newName2 = obj.ToString() + "." + "pdf";
                                string newPath2 = Path.Combine(savePath, newName2);
                                string old2 = Path.Combine(savePath, TENFILE2);

                                if (File.Exists(old2))
                                {
                                    FileInfo del = new FileInfo(old2);
                                    del.Delete();
                                }
                                fs2.MoveTo(old2);
                                File.Move(old2, newPath2);
                            }

                            //if (Path.GetExtension(fileName) == ".docx")
                            //{
                            //    fileName = Path.ChangeExtension(fileName, "pdf");
                            //}

                            return newName;
                        }
                    }
                }

                return "";
            }
            catch
            {
                return "";
            }
        }

        public int deleteFile(List<FileDinhKemViewModel> filelist)
        {
            try
            {
                int iUploadedCnt = 0;
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                for (int i = 0; i < filelist.Count; i++)
                {
                    sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                    sPath = Path.Combine(sPath, filelist[i].NGAYTAO?.ToString("yyyy"));
                    sPath = Path.Combine(sPath, filelist[i].NGAYTAO?.ToString("MM"));
                    sPath = Path.Combine(sPath, filelist[i].TENFILE);
                    if (File.Exists(sPath))
                    {
                        FileInfo del = new FileInfo(sPath);
                        del.Delete();
                        iUploadedCnt = iUploadedCnt + 1;
                    }
                }

                return iUploadedCnt;
            }
            catch
            {
                return 0;
            }
        }
        #endregion

        #region Common APIs
        [HttpPost]
        [Route("GetDanhSachNhanVien")]
        public async Task<IHttpActionResult> GetDanhSachNhanVien(FilterDto model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestUri = new Uri($"QuanLyNghiPhep/nhan-vien", UriKind.Relative);
                    var deserializedProduct = JsonConvert.SerializeObject(model.FilterLenhDieuXe);
                    var content = new StringContent(deserializedProduct, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(requestUri, content);

                    var jsonData = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<NhanVienDto>(jsonData);
                    if (result.data == null || result.data.Count == 0)
                    {
                        result.data = new List<NhanVienViewModel>();
                    }
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetDanhSachNhanVienQLCC")]
        public async Task<IHttpActionResult> GetDanhSachNhanVienQLCC(FilterDto model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestUri = new Uri($"QuanLyNghiPhep/nhan-vien-qlcc", UriKind.Relative);
                    var deserializedProduct = JsonConvert.SerializeObject(model.FilterLenhDieuXe);
                    var content = new StringContent(deserializedProduct, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(requestUri, content);

                    var jsonData = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<NhanVienDto>(jsonData);
                    if (result.data == null || result.data.Count == 0)
                    {
                        result.data = new List<NhanVienViewModel>();
                    }
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetDanhSachNhanVienTrongPhong")]
        public async Task<IHttpActionResult> GetDanhSachNhanVienTrongPhong(string maNhanVien)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestUri = new Uri($"QuanLyNghiPhep/nhan-vien-trong-phong?maNhanVien=" + maNhanVien, UriKind.Relative);
                    var response = await client.GetAsync(requestUri);
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<object>>(jsonData);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetDanhSachNhanVienTrongPhongHoacToQLCC")]
        public async Task<IHttpActionResult> GetDanhSachNhanVienTrongPhongHoacTo(string maNhanVien, bool isCreatedAllPermission)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestUri = new Uri($"QuanLyNghiPhep/nhan-vien-trong-phong-hoac-to-qlcc?maNhanVien={maNhanVien}&isCreatedAllPermission={isCreatedAllPermission}", UriKind.Relative);
                    var response = await client.GetAsync(requestUri);
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<object>>(jsonData);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetThongTinNghiPhepNhanVien")]
        public async Task<IHttpActionResult> GetThongTinNghiPhepNhanVien(string maNhanVien, string denNgay)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestUri = new Uri($"QuanLyNghiPhep/nhan-vien?maNhanVien={maNhanVien}&denNgay={denNgay}", UriKind.Relative);
                    var response = await client.GetAsync(requestUri);
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<object>(jsonData);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetDanhSachPhongBan")]
        public async Task<IHttpActionResult> GetDanhSachPhongBan()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //set up client
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestUri = new Uri($"QuanLyNghiPhep/phong-ban", UriKind.Relative);
                    var response = await client.GetAsync(requestUri);
                    var jsondata = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<PhongBanDoiViewModel>>(jsondata);
                    return Ok(result);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetDanhSachLoaiNghiPhep")]
        public async Task<IHttpActionResult> GetDanhSachLoaiNghiPhep()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //set up client
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestUri = new Uri($"QuanLyNghiPhep/loai-nghi-phep", UriKind.Relative);
                    var response = await client.GetAsync(requestUri);
                    var jsondata = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<LoaiNghiPhepViewModel>>(jsondata);
                    return Ok(result);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetNgayNghiLe")]
        public async Task<IHttpActionResult> GetNgayNghiLe()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //set up client
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestUri = new Uri($"QuanLyNghiPhep/get-ngay-nghi-le", UriKind.Relative);
                    var response = await client.GetAsync(requestUri);
                    var jsondata = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<object>>(jsondata);
                    return Ok(result);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("GetNguoiKyDuyet")]
        public IHttpActionResult GetNguoiKyDuyet(WFModel.CommonModel par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@loaidon", par.valint1.checkIsNumber());
                    parameters.Add("@manhanvien", par.valint2.checkIsNumber());
                    parameters.Add("@idPhongBan", par.valstring1);
                    parameters.Add("@idChucVu", par.valstring2);
                    var dirs = db.Query<Core_UserDto>("VanBanKiSo_GetSigners", parameters, null, true, null, System.Data.CommandType.StoredProcedure);
                    return Ok(dirs);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        #endregion

        #region Main APIs
        [HttpPost] // Crystal Report
        [Route("ThemVanBan")]
        public async Task<IHttpActionResult> ThemVanBan(DonNghiPhepRequest para)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //set up client
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + para.accessToken);

                    var requestUri = new Uri($"QuanLyNghiPhep/don-nghi-phep", UriKind.Relative);
                    para.nguoiTao = User.Identity.Name;
                    var deserializedProduct = JsonConvert.SerializeObject(para);
                    var content = new StringContent(deserializedProduct, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(requestUri, content);
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ResponseInt>(jsonData);
                    if (result.Id > -1)
                    {
                        if (para.FileDinhKem != null && para.FileDinhKem.Count > 0)
                        {
                            foreach (var item in para.FileDinhKem)
                            {
                                string filename = moveFile(item.MOTA, item.LOAIFILE);
                                FileDinhKemRequest rs = new FileDinhKemRequest();
                                rs.VanBanID = result.Id;
                                rs.MoTa = item.MOTA;
                                rs.LoaiFile = item.LOAIFILE;
                                rs.TenFile = filename;
                                rs.SizeFile = (int)item.SIZEFILE;
                                rs.Module = (int)para.loai;
                                vc.ThemFileVB(rs);
                            }
                        }
                        vc.ChangeSignerFlowVB(result.Id, para.Module, 0, para.DanhSachNguoiKy, null, null, 0, 0, 0);
                        return Ok(result);
                    }
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost] // Chrome Headless
        [Route("ThemVanBanPDF")]
        public async Task<IHttpActionResult> ThemVanBanPDF(DonNghiPhepRequest para)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //set up client
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + para.accessToken);

                    var requestUri = new Uri($"QuanLyNghiPhep/don-nghi-phep", UriKind.Relative);
                    para.nguoiTao = User.Identity.Name;
                    var deserializedProduct = JsonConvert.SerializeObject(para);
                    var content = new StringContent(deserializedProduct, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(requestUri, content);
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ResponseInt>(jsonData);
                    if (result.Id > -1)
                    {
                        // Thêm File đính kèm
                        if (para.FileDinhKem != null && para.FileDinhKem.Count > 0)
                        {
                            foreach (var item in para.FileDinhKem)
                            {
                                string filename = moveFile(item.MOTA, item.LOAIFILE);
                                FileDinhKemRequest rs = new FileDinhKemRequest();
                                rs.VanBanID = result.Id;
                                rs.MoTa = item.MOTA;
                                rs.LoaiFile = item.LOAIFILE;
                                rs.TenFile = filename;
                                rs.SizeFile = (int)item.SIZEFILE;
                                rs.Module = (int)para.loai;
                                vc.ThemFileVB(rs);
                            }
                        }

                        // Thêm người ký
                        vc.ChangeSignerFlowVB(result.Id, para.Module, 0, para.DanhSachNguoiKy, null, null, 0, 0, 0);

                        // Thêm File nghỉ phép ký số HTML
                        var vm = new PdfDonNghiPhepVm
                        {
                            HoVaTen = para.tenNhanVien,
                            NamSinh = para.namSinh.ToString(),
                            SoCCCD = para.soCccd,
                            NgayTao = $"TP.HCM, ngày {DateTime.Now:dd} tháng {DateTime.Now:MM} năm {DateTime.Now:yyyy}",
                            NgayCapCCCD = $"{para.ngayCapCccd:dd}/{para.ngayCapCccd:MM}/{para.ngayCapCccd:yyyy}",
                            DonVi = para.phongBan,
                            ChucDanh = para.chucVu,
                            NghiTuNgay = $"{para.tuNgay:dd}/{para.tuNgay:MM}/{para.tuNgay:yyyy}",
                            DenHetNgay = $"{para.denNgay:dd}/{para.denNgay:MM}/{para.denNgay:yyyy}",
                            NoiNghiPhep = para.noiNghiPhep,
                            LyDoNghiPhep = para.lyDo,
                            DanhSachNguoiKy = para.DanhSachNguoiKy,
                            QuocGiaNghiPhep = para.quocGia ?? "...",
                            YKienPhongBan = para.yKienPhongBanDoi ?? "...",
                            Module = para.loai ?? Module.DON_NGHI_PHEP_MAU_1_NHAN_VIEN
                        };

                        // Render HTML từ template
                        var html = RenderDonNghiPhep(vm);

                        // Convert sang PDF
                        var baseDir = HostingEnvironment.MapPath(FILE_DOWNLOADED.DON_NGHI_PHEP_URL)
                                   ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FILE_DOWNLOADED.DON_NGHI_PHEP_FOLDER_NAME);

                        // Tạo thư mục theo Năm/Tháng
                        DateTime ngaytao = DateTime.Now;
                        var year = ngaytao.Year.ToString();
                        var month = ngaytao.ToString("MM"); // 01, 02, 03...
                        var absDir = Path.Combine(baseDir, year, month);
                        Directory.CreateDirectory(absDir);

                        // Tạo tên file
                        var fileName = Guid.NewGuid().ToString("N") + "." + FILE_DOWNLOADED.PDF_EXTENSION;
                        var savePath = Path.Combine(absDir, fileName);

                        // Convert PDF
                        var pdf = HtmlToPdfConverter.ConvertHtmlString(html, persistOutputTo: savePath);

                        // Thêm File
                        var attachReq = new FileDinhKemRequest
                        {
                            VanBanID = result.Id,
                            MoTa = "DonNghiPhep.pdf",
                            LoaiFile = FILE_DOWNLOADED.PDF_EXTENSION,
                            TenFile = fileName,
                            SizeFile = fileName.Length,
                            IsCRFile = 1,
                            Module = para.loai ?? Module.DON_NGHI_PHEP_MAU_1_NHAN_VIEN
                        };
                        vc.ThemFileVB(attachReq);

                        return Ok(result);
                    }
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetListVB")]
        public async Task<IHttpActionResult> GetDanhSachVB(FilterDto model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //set up client
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestUri = new Uri($"QuanLyNghiPhep/get-don-nghi-phep?userName=" + User.Identity.Name + "&loaiLoc=" + model.FilterVanBan.LoaiLoc + "&maNhanVien=" + model.FilterVanBan.CANBO, UriKind.Relative);
                    //model.FilterVanBan.CANBO = User.Identity.Name;
                    var deserializedProduct = JsonConvert.SerializeObject(model.FilterLenhDieuXe);
                    var content = new StringContent(deserializedProduct, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(requestUri, content);

                    var jsonData = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<LenhDieuXeModel.DonNghiPhepDto>(jsonData);
                    if (result.data == null || result.data.Count == 0)
                    {
                        result.data = new List<DonNghiPhepViewModel>();
                    }
                    else
                    {
                        //var list = result.data.OrderByDescending(t => t.ngayTaoHeThong).ToList();
                        //result.data = list;
                        foreach (DonNghiPhepViewModel item in result.data)
                        {
                            item.FileDinhKem = vc.LayDanhSachFileVB((long)item.id, -1, model.FilterVanBan.LoaiVB);
                        }
                    }
                    return Ok(result);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("GetBaoCaoNghiPhep")]
        public async Task<IHttpActionResult> GetBaoCaoNghiPhep(BaoCaoThongKeNghiPhepRequest model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestUri = new Uri($"QuanLyNghiPhep/thong-ke?tuNgay=" + model.tuNgay + "&denNgay=" + model.denNgay + "&idPhongBan=" + (model.idPhongBan == "All" ? null : model.idPhongBan) + "&maNhanVien=" + (model.maNhanVien == "0" ? null : model.maNhanVien), UriKind.Relative);
                    var response = await client.GetAsync(requestUri);
                    response.EnsureSuccessStatusCode();
                    var jsondata = await response.Content.ReadAsByteArrayAsync();
                    return Ok(Convert.ToBase64String(jsondata));
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("GetBaoCaoGiaiTrinh")]
        public async Task<IHttpActionResult> GetBaoCaoGiaiTrinh(BaoCaoThongKeNghiPhepRequest model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestUri = new Uri($"QuanLyNghiPhep/thong-ke?tuNgay=" + model.tuNgay + "&denNgay=" + model.denNgay + "&idPhongBan=" + (model.idPhongBan == "All" ? null : model.idPhongBan) + "&maNhanVien=" + (model.maNhanVien == "0" ? null : model.maNhanVien), UriKind.Relative);
                    var response = await client.GetAsync(requestUri);
                    response.EnsureSuccessStatusCode();
                    var jsondata = await response.Content.ReadAsByteArrayAsync();
                    return Ok(Convert.ToBase64String(jsondata));
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("GetVanBanChiTiet")]
        public async Task<IHttpActionResult> GetChiTietVB(FilterDto model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //set up client
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestUri = new Uri($"QuanLyNghiPhep/get-don-nghi-phep?id=" + model.FilterVanBan.ID, UriKind.Relative);
                    //var deserializedProduct = JsonConvert.SerializeObject(model.FilterLenhDieuXe);
                    //var content = new StringContent(deserializedProduct, Encoding.UTF8, "application/json");
                    //var response = await client.PostAsync(requestUri, content);
                    var response = await client.GetAsync(requestUri);

                    var jsonData = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<LenhDieuXeModel.DonNghiPhepChiTietDto>(jsonData);
                    if (result == null) { return BadRequest(); }
                    DonNghiPhepViewModel chitietextension = vc.GetChiTietVBDonNghiPhep(model.FilterVanBan.ID, model.FilterVanBan.LoaiVB, model.FilterVanBan.SoVanBanID, result.DonNghiPhep.smartCAStringID);
                    if (chitietextension.FileDinhKem != null)
                    {
                        if (chitietextension.FileDinhKem.Count > 0)
                        {
                            foreach (WFModel.FileDinhKemViewModel item in chitietextension.FileDinhKem)
                            {
                                string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                                sPath = Path.Combine(sPath, item.NGAYTAO?.ToString("yyyy"));
                                sPath = Path.Combine(sPath, item.NGAYTAO?.ToString("MM"));
                                sPath = Path.Combine(sPath, item.TENFILE);
                                if (File.Exists(sPath))
                                {
                                    var imgBytes = File.ReadAllBytes(sPath);
                                    item.BASE64DATA = Convert.ToBase64String(imgBytes);
                                    item.VITRIFILE = item.TENFILE;
                                }
                                else
                                {
                                    item.BASE64DATA = null;
                                    item.VITRIFILE = null;
                                }
                            }
                        }
                    }
                    result.DonNghiPhep.KySoID = chitietextension.KySoID;
                    result.DonNghiPhep.ThuTuKySo = chitietextension.ThuTuKySo;
                    result.DonNghiPhep.FileDinhKem = chitietextension.FileDinhKem;
                    result.DonNghiPhep.DanhSachNguoiKy = chitietextension.DanhSachNguoiKy;
                    return Ok(result);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost] // Crystal Report
        [Route("CapNhatThongTinVanBan")]
        public async Task<IHttpActionResult> CapNhatThongTin(DonNghiPhepViewModel model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //set up client
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + model.accessToken);

                    var requestUri = new Uri($"QuanLyNghiPhep/don-nghi-phep", UriKind.Relative);
                    model.nguoiCapNhat = User.Identity.Name;
                    model.ngayCapNhat = DateTime.Now;

                    var deserializedProduct = JsonConvert.SerializeObject(model);
                    var content = new StringContent(deserializedProduct, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(requestUri, content);
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ResponseInt>(jsonData);
                    if (result.Id > -1)
                    {
                        if (model.FileDinhKem != null)
                        {
                            if (model.FileDinhKem.Count > 0)
                            {
                                deleteFile(model.FileDinhKem.Where(x => x.IsCRFile == 1).ToList());
                                using (IDbConnection db = new SqlConnection(_cnn))
                                {
                                    if (db.State == System.Data.ConnectionState.Closed)
                                        db.Open();
                                    foreach (FileDinhKemViewModel item in model.FileDinhKem)
                                    {
                                        string deleteQuery = @"DELETE FROM [dbo].[VanBanKiSo_FileDinhKem] WHERE ID = @ID";
                                        db.Execute(deleteQuery, new { @ID = item.ID });
                                        if (item.IsCRFile == 0)
                                        {
                                            FileDinhKemRequest rs = new FileDinhKemRequest();
                                            rs.VanBanID = (long)model.id;
                                            rs.MoTa = item.MOTA;
                                            rs.LoaiFile = item.LOAIFILE;
                                            rs.TenFile = item.TENFILE;
                                            rs.SizeFile = (int)item.SIZEFILE;
                                            rs.Module = (int)model.loai;
                                            vc.ThemFileVB(rs);
                                        }
                                    }
                                }
                            }
                        }
                        vc.ChangeSignerFlowVB((long)model.id, model.Module, 0, model.DanhSachNguoiKy, null, null, 0, 0, 0);
                        return Ok(result);
                    }
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost] // Chrome Headless
        [Route("CapNhatThongTinVanBanPDF")]
        public async Task<IHttpActionResult> CapNhatThongTinPDF(DonNghiPhepViewModel model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //set up client
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + model.accessToken);

                    var requestUri = new Uri($"QuanLyNghiPhep/don-nghi-phep", UriKind.Relative);
                    model.nguoiCapNhat = User.Identity.Name;
                    model.ngayCapNhat = DateTime.Now;

                    var deserializedProduct = JsonConvert.SerializeObject(model);
                    var content = new StringContent(deserializedProduct, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(requestUri, content);
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ResponseInt>(jsonData);
                    if (result.Id > -1)
                    {
                        // Thêm File đính kèm
                        if (model.FileDinhKem != null && model.FileDinhKem.Count > 0)
                        {
                            deleteFile(model.FileDinhKem.Where(x => x.IsCRFile == 1).ToList());
                            using (IDbConnection db = new SqlConnection(_cnn))
                            {
                                if (db.State == System.Data.ConnectionState.Closed)
                                    db.Open();
                                foreach (FileDinhKemViewModel item in model.FileDinhKem)
                                {
                                    string deleteQuery = @"DELETE FROM [dbo].[VanBanKiSo_FileDinhKem] WHERE ID = @ID";
                                    db.Execute(deleteQuery, new { @ID = item.ID });
                                    if (item.IsCRFile == 0)
                                    {
                                        FileDinhKemRequest rs = new FileDinhKemRequest();
                                        rs.VanBanID = (long)model.id;
                                        rs.MoTa = item.MOTA;
                                        rs.LoaiFile = item.LOAIFILE;
                                        rs.TenFile = item.TENFILE;
                                        rs.SizeFile = (int)item.SIZEFILE;
                                        rs.Module = (int)model.loai;
                                        vc.ThemFileVB(rs);
                                    }
                                }
                            }
                        }

                        // Thêm người ký
                        vc.ChangeSignerFlowVB((long)model.id, model.Module, 0, model.DanhSachNguoiKy, null, null, 0, 0, 0);

                        // Thêm File nghỉ phép ký số HTML
                        var vm = new PdfDonNghiPhepVm
                        {
                            HoVaTen = model.tenNhanVien,
                            NamSinh = model.namSinh.ToString(),
                            SoCCCD = model.soCccd,
                            NgayTao = $"TP.HCM, ngày {DateTime.Now:dd} tháng {DateTime.Now:MM} năm {DateTime.Now:yyyy}",
                            NgayCapCCCD = $"{model.ngayCapCccd:dd}/{model.ngayCapCccd:MM}/{model.ngayCapCccd:yyyy}",
                            DonVi = model.phongBan,
                            ChucDanh = model.chucVu,
                            NghiTuNgay = $"{model.tuNgay:dd}/{model.tuNgay:MM}/{model.tuNgay:yyyy}",
                            DenHetNgay = $"{model.denNgay:dd}/{model.denNgay:MM}/{model.denNgay:yyyy}",
                            NoiNghiPhep = model.noiNghiPhep,
                            LyDoNghiPhep = model.lyDo,
                            DanhSachNguoiKy = model.DanhSachNguoiKy,
                            QuocGiaNghiPhep = model.quocGia ?? "...",
                            YKienPhongBan = model.yKienPhongBanDoi ?? "...",
                            Module = model.loai ?? Module.DON_NGHI_PHEP_MAU_1_NHAN_VIEN
                        };

                        // Render HTML từ template
                        var html = RenderDonNghiPhep(vm);

                        // Convert sang PDF
                        var baseDir = HostingEnvironment.MapPath(FILE_DOWNLOADED.DON_NGHI_PHEP_URL)
                                   ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FILE_DOWNLOADED.DON_NGHI_PHEP_FOLDER_NAME);

                        // Tạo thư mục theo Năm/Tháng
                        DateTime ngaytao = DateTime.Now;
                        var year = ngaytao.Year.ToString();
                        var month = ngaytao.ToString("MM"); // 01, 02, 03...
                        var absDir = Path.Combine(baseDir, year, month);
                        Directory.CreateDirectory(absDir);

                        // Tạo tên file
                        var fileName = Guid.NewGuid().ToString("N") + "." + FILE_DOWNLOADED.PDF_EXTENSION;
                        var savePath = Path.Combine(absDir, fileName);

                        // Convert PDF
                        var pdf = HtmlToPdfConverter.ConvertHtmlString(html, persistOutputTo: savePath);

                        // Thêm File
                        var attachReq = new FileDinhKemRequest
                        {
                            VanBanID = (long)model.id,
                            MoTa = "DonNghiPhep.pdf",
                            LoaiFile = FILE_DOWNLOADED.PDF_EXTENSION,
                            TenFile = fileName,
                            SizeFile = fileName.Length,
                            IsCRFile = 1,
                            Module = model.loai ?? Module.DON_NGHI_PHEP_MAU_1_NHAN_VIEN
                        };
                        vc.ThemFileVB(attachReq);

                        return Ok(result);
                    }
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("CapNhatYKienPhongBan")]
        public async Task<IHttpActionResult> CapNhatYKienPhongBan(CapNhatYKienPhongBanRequest model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //set up client
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + model.accessToken);

                    var requestUri = new Uri($"QuanLyNghiPhep/cap-nhat-y-kien", UriKind.Relative);
                    model.nguoiCapNhat = User.Identity.Name;
                    var deserializedProduct = JsonConvert.SerializeObject(model);
                    var content = new StringContent(deserializedProduct, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(requestUri, content);
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ResponseInt>(jsonData);
                    if (result.Id > -1)
                    {
                        if (model.FileDinhKem != null)
                        {
                            if (model.FileDinhKem.Count > 0)
                            {
                                deleteFile(model.FileDinhKem.Where(x => x.IsCRFile == 1).ToList());
                                using (IDbConnection db = new SqlConnection(_cnn))
                                {
                                    if (db.State == System.Data.ConnectionState.Closed)
                                        db.Open();
                                    foreach (FileDinhKemViewModel item in model.FileDinhKem)
                                    {
                                        string deleteQuery = @"DELETE FROM [dbo].[VanBanKiSo_FileDinhKem] WHERE ID = @ID";
                                        db.Execute(deleteQuery, new { @ID = item.ID });
                                        if (item.IsCRFile == 0)
                                        {
                                            FileDinhKemRequest rs = new FileDinhKemRequest();
                                            rs.VanBanID = (long)model.id;
                                            rs.MoTa = item.MOTA;
                                            rs.LoaiFile = item.LOAIFILE;
                                            rs.TenFile = item.TENFILE;
                                            rs.SizeFile = (int)item.SIZEFILE;
                                            rs.Module = (int)model.loai;
                                            vc.ThemFileVB(rs);
                                        }
                                    }
                                }
                            }
                        }
                        return Ok(result);
                    }
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("CapNhatFileBiBatDongBo")]
        public IHttpActionResult CapNhatFileBiBatDongBo(WFModel.CommonModel model)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    string getFileDinhKemQr = @"Select * FROM [dbo].[VanBanKiSo_FileDinhKem] WHERE VANBANID = @IDVB and Module = @Module and IsCRFile = 1";
                    var listFile = db.Query<WFModel.FileDinhKemViewModel>(getFileDinhKemQr, new { @IDVB = model.valint1, @Module = model.valint2 }, null, true, null).ToList();
                    if (listFile.Count > 0)
                    {
                        string deleteFileKySoQr = @"DELETE FROM [dbo].[SmartCA_FilePdf] WHERE RefId = @RefId";
                        db.Execute(deleteFileKySoQr, new { @RefId = model.valstring1 });
                        foreach (var fileItem in listFile)
                        {
                            string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                            sPath = Path.Combine(sPath, fileItem.NGAYTAO?.ToString("yyyy"));
                            sPath = Path.Combine(sPath, fileItem.NGAYTAO?.ToString("MM"));
                            sPath = Path.Combine(sPath, fileItem.TENFILE);
                            string insertFile = @"INSERT INTO [SmartCA_FilePdf]
                                                    (ID, RefId, FileName, FilePath, UnsignData, CreatedDate)
                                                    VALUES (newID(), @RefId, @FileName, @FilePath,@UnsignData , @NgayTao);";
                            var imgBytes = File.ReadAllBytes(sPath);
                            var file = new DynamicParameters();
                            file.Add("@RefId", model.valstring1);
                            file.Add("@FileName", fileItem.MOTA);
                            file.Add("@FilePath", fileItem.TENFILE);
                            file.Add("@UnsignData", Convert.ToBase64String(imgBytes));
                            file.Add("@NgayTao", fileItem.NGAYTAO);
                            db.Execute(insertFile, file);
                        }
                    }
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Testing
        [HttpPost] // API dùng để test xuất file đơn nghỉ phép PDF
        [Route("TaoFileDonNghiPhepPDF")]
        public IHttpActionResult GenerateEvalutionFile([FromBody] DonNghiPhepRequest req)
        {
            //Map request -> ViewModel
            var ngayTao = DateTime.Now;
            var ngayCapCCCD = DateTime.Now.AddDays(2);
            var nghiTuNgay = DateTime.Now.AddDays(3);
            var denHetNgay = DateTime.Now.AddDays(4);
            var vm = new PdfDonNghiPhepVm
            {
                HoVaTen = "Lê Hoàng Thiên Vũ",
                NamSinh = "1995",
                SoCCCD = $"1900514799",
                NgayTao = $"TP.HCM, ngày {ngayTao:dd} tháng {ngayTao:MM} năm {ngayTao:yyyy}",
                NgayCapCCCD = $"{ngayCapCCCD:dd}/{ngayCapCCCD:MM}/{ngayCapCCCD:yyyy}",
                DonVi = "P.CNTT",
                ChucDanh = "Nhân viên",
                NghiTuNgay = $"{nghiTuNgay:dd}/{nghiTuNgay:MM}/{nghiTuNgay:yyyy}",
                DenHetNgay = $"{denHetNgay:dd}/{denHetNgay:MM}/{denHetNgay:yyyy}",
                NoiNghiPhep = "Tại gia",
                LyDoNghiPhep = "Nghỉ thường niên",
                DanhSachNguoiKy = req.DanhSachNguoiKy,
                QuocGiaNghiPhep = "Mỹ",
                YKienPhongBan = "Tôi đồng ý với đơn nghỉ này",
                Module = req.Module
            };

            // Render HTML từ template
            var html = RenderDonNghiPhep(vm);

            // Convert sang PDF
            var baseDir = HostingEnvironment.MapPath(FILE_DOWNLOADED.DON_NGHI_PHEP_URL)
                       ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FILE_DOWNLOADED.DON_NGHI_PHEP_FOLDER_NAME);

            // Tạo thư mục theo Năm/Tháng
            DateTime ngaytao = DateTime.Now;
            var year = ngaytao.Year.ToString();
            var month = ngaytao.ToString("MM"); // 01, 02, 03...
            var absDir = Path.Combine(baseDir, year, month);
            Directory.CreateDirectory(absDir);

            // Tạo tên file
            var fileName = Guid.NewGuid().ToString("N") + "." + FILE_DOWNLOADED.PDF_EXTENSION;
            var savePath = Path.Combine(absDir, fileName);

            // Convert PDF
            var pdf = HtmlToPdfConverter.ConvertHtmlString(html, persistOutputTo: savePath);

            // Trả file về client
            var response = FileHelper.CreateFileResponse(
                pdf,
                FILE_DOWNLOADED.PDF_FORMAT,
                "DonNghiPhep.pdf"
            );

            return ResponseMessage(response);
        }
        #endregion
    }
}