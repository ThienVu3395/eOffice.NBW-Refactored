using static VModel.WFModel;
using static VModel.LenhDieuXeModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Clone_KySoDienTu.Controllers.API.SmartCAFunction;
using Clone_KySoDienTu.Controllers.API.SmartCAKyDonLuong;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Dapper;
using SmartCAAPI.Dtos.signpdf;
using VnptHashSignatures.Common;
using VnptHashSignatures.Interface;
using VnptHashSignatures.Pdf;
using System.Configuration;
using Clone_KySoDienTu.Models.Dtos.QuanLyChamCong;

namespace Clone_KySoDienTu.Controllers.API.QuanLyDieuXe
{
    [RoutePrefix("api/QLDieuXe")]
    public class QuanLyDieuXeAPIController : ApiController
    {
        private readonly string _cnn;

        private readonly string _endpoint;

        private VCode.LenhDieuXeModule vc;

        private readonly SmartCAFunctionController _smartCAFunction;

        private readonly APIKyDonLuongController _smartCAKyDonLuong;

        public QuanLyDieuXeAPIController()
        {
            _cnn = ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
            _endpoint = ConfigurationManager.AppSettings["EndpointVanBanKySo"];
            vc = new VCode.LenhDieuXeModule(_cnn);
            _smartCAFunction = new SmartCAFunctionController();
            _smartCAKyDonLuong = new APIKyDonLuongController();
        }

        #region Files functions
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

        [HttpPost]
        [Route("RemoveFile_Update")] //Xóa file trong thư mục CongVanFile khi file đó đã có trong csdl
        public string RemoveFile_Update(CommonModel model)
        {
            using (IDbConnection db = new SqlConnection(_cnn))
            {
                if (db.State == System.Data.ConnectionState.Closed)
                {
                    db.Open();
                }
                var dirs = vc.LayMotFileVB(model.valint1, null, model.valint2);
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                sPath = Path.Combine(sPath, dirs.NGAYTAO?.ToString("yyyy"));
                sPath = Path.Combine(sPath, dirs.NGAYTAO?.ToString("MM"));
                sPath = Path.Combine(sPath, dirs.TENFILE);
                if (System.IO.File.Exists(sPath))
                {
                    var listVanBan = vc.LayMotFileVB(model.valint1, dirs.TENFILE, model.valint2);
                    if (listVanBan != null)
                    {
                        FileInfo f = new FileInfo(sPath);

                        f.Delete();
                    }
                }
                if (vc.XoaFileVB(model.valint1) > 0)
                {
                    //Hub.Clients.All.LoadDsUser();
                    return "Remove Success";
                }
                else return "Failed";
            }
        }

        [HttpPost]
        [Route("UploadFile_Update")] // Thêm File trong trường hợp cập nhật dữ liệu
        public IHttpActionResult UploadFile_Update(FileDinhKemRequest model)
        {
            try
            {
                string tenFile = moveFile(model.MoTa, model.LoaiFile);
                model.TenFile = tenFile;
                int result = vc.ThemFileVB(model);
                if (result > 0)
                {
                    //Hub.Clients.All.LoadDsUser();
                    //vc.ThemLogVB(model.valint1, model.valint3, User.Identity.Name, User.Identity.Name + " đã cập nhật file pdf");
                    CommonModel rs = new CommonModel();
                    rs.valint1 = result;
                    rs.valstring1 = tenFile;
                    return Ok(rs);
                }
                else return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("getviewpdfvb")] //Xem file pdf trong thư mục CongVanFile khi file đó đã có trong csdl
        public HttpResponseMessage Getviewpdfvb(int id, int module)
        {
            using (IDbConnection db = new SqlConnection(_cnn))
            {
                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                }
                var dirs = vc.LayMotFileVB(id, null, module);
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                sPath = Path.Combine(sPath, dirs.NGAYTAO?.ToString("yyyy"));
                sPath = Path.Combine(sPath, dirs.NGAYTAO?.ToString("MM"));

                if (Path.GetExtension(dirs.TENFILE) == ".docx")
                {
                    dirs.TENFILE = Path.ChangeExtension(dirs.TENFILE, "pdf");
                }
                sPath = Path.Combine(sPath, dirs.TENFILE);
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = File.Open(sPath, FileMode.Open, FileAccess.Read, FileShare.None); //new System.IO.FileStream(sPath, System.IO.FileMode.Open);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.Add("x-filename", dirs.MOTA);
                return response;
            }
        }

        [HttpGet]
        [Route("downloadfile_unsign")] //tải file ký số chưa ký
        public async Task<IHttpActionResult> DownloadFileUnsign(string id)
        {
            using (IDbConnection db = new SqlConnection(_cnn))
            {
                if (db.State == System.Data.ConnectionState.Closed)
                {
                    db.Open();
                }
                var signFileList = await _smartCAFunction.FindFiles(id);
                List<CommonModel> responseMessages = new List<CommonModel>();
                if (signFileList.ToList().Count > 0)
                {
                    foreach (var item in signFileList)
                    {
                        //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                        //byte[] pdfBytes = Convert.FromBase64String(item.UnsignData);
                        //response.StatusCode = HttpStatusCode.OK;
                        //response.Content = new ByteArrayContent(pdfBytes);
                        //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                        //response.Content.Headers.Add("x-filename", item.FileName);
                        //responseMessages.Add(response);
                        CommonModel response = new CommonModel();
                        response.valstring1 = item.FileName;
                        response.valstring2 = item.UnsignData;
                        responseMessages.Add(response);
                    }
                }
                return Ok(responseMessages);
            }
        }
        #endregion

        #region General functions
        public async Task<string> GetAccTk_LenhDieuXe()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //set up client
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestUri = new Uri($"Auth", UriKind.Relative);
                    AccessTokenModel model = new AccessTokenModel();
                    model.userName = "qldx";
                    model.password = "123456";
                    model.appID = "QuyTrinhDieuXe";
                    var deserializedProduct = JsonConvert.SerializeObject(model);
                    var content = new StringContent(deserializedProduct, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(requestUri, content);
                    var jsondata = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<AccessTokenResonse>(jsondata);
                    return result.accessToken;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetAccTk_DonNghiPhep()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //set up client
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestUri = new Uri($"Auth", UriKind.Relative);
                    AccessTokenModel model = new AccessTokenModel();
                    model.userName = "qlnp";
                    model.password = "123456";
                    model.appID = "QuyTrinhNghiPhep";
                    var deserializedProduct = JsonConvert.SerializeObject(model);
                    var content = new StringContent(deserializedProduct, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(requestUri, content);
                    var jsondata = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<AccessTokenResonse>(jsondata);
                    return result.accessToken;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<ResponseInt> CapNhatVanBan(CommonModel model)
        {
            UpdateDocumentRequest updateDocumentRequest = new UpdateDocumentRequest();
            updateDocumentRequest.id = model.valint1;
            updateDocumentRequest.nguoiCapNhat = User.Identity.Name;
            updateDocumentRequest.noiDung = model.valint2 == 2 || model.valint2 == 1 ? "2" : "3"; // nếu thu hồi hoặc hủy ký sẽ trả về lưu nháp = 2 , còn lại sẽ hủy đơn = 3
            updateDocumentRequest.accessToken = model.valstring2;
            updateDocumentRequest.className = GetClassName(model.valint3);
            updateDocumentRequest.propertyName = PropertyName.TRANG_THAI;
            ResponseInt result1 = await CapNhatThongTinVanBan(updateDocumentRequest);
            updateDocumentRequest.noiDung = null;
            updateDocumentRequest.propertyName = PropertyName.SMART_CA_ID_STRING;
            ResponseInt result2 = await CapNhatThongTinVanBan(updateDocumentRequest);
            updateDocumentRequest.propertyName = PropertyName.NGUOI_DUYET;
            ResponseInt result3 = await CapNhatThongTinVanBan(updateDocumentRequest);
            if (result1 == null || result2 == null || result3 == null)
            {
                return null;
            }
            return result3;
        }

        public async Task<ResponseInt> CapNhatThongTinVanBan(UpdateDocumentRequest model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //set up client
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + model.accessToken);

                    var requestUri = new Uri($"QuanLyVanBan/cap-nhat", UriKind.Relative);
                    var deserializedProduct = JsonConvert.SerializeObject(model);
                    var content = new StringContent(deserializedProduct, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(requestUri, content);
                    var jsondata = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ResponseInt>(jsondata);
                    return result;
                }
            }
            catch
            {
                return null;
            }
        }

        public string GetClassName(int Module)
        {
            string classname = null;
            switch (Module)
            {
                case 3: // lệnh điều xe
                    classname = ClassName.LICH_SU_DIEU_XE;
                    break;
                case 4: // sổ lộ trình
                    classname = ClassName.SO_LO_TRINH;
                    break;
                case 5: // ứng trước nhiên liệu
                    classname = ClassName.UNG_TRUOC_NHIEN_LIEU;
                    break;
                case 6: // quyết toán nhiên liệu
                    classname = ClassName.QUYET_TOAN_NHIEN_LIEU;
                    break;
                case 7: // đơn xin nghỉ phép
                    classname = ClassName.DON_NGHI_PHEP;
                    break;
                case 8: // phiếu đề nghị sử dụng xe
                    classname = ClassName.PHIEU_XIN_XE;
                    break;
                case 9: // phiếu quản lý lệnh điều xe
                    classname = ClassName.QUAN_LY_LENH_DIEU_XE;
                    break;
                default:
                    break;
            }
            return classname;
        }

        [HttpGet]
        [Route("GetCountThongBao")]
        public async Task<IHttpActionResult> GetSoLuongThongBaoVB()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //set up client
                    client.BaseAddress = new Uri(_endpoint);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestUri = new Uri($"QuanLyVanBan/so-luong?nguoiDuyet=" + User.Identity.Name, UriKind.Relative);
                    var response = await client.GetAsync(requestUri);
                    var jsondata = await response.Content.ReadAsStringAsync();
                    CountThongBaoLenhDieuXeResponse countLenhDieuXe = JsonConvert.DeserializeObject<CountThongBaoLenhDieuXeResponse>(jsondata);
                    ThongBaoModel countDynamicReport = vc.LaySoLuongThongBaoVB(User.Identity.Name, 2);
                    CountThongBaoResponse countThongBao = new CountThongBaoResponse();
                    countThongBao.vanBanKiSoChuaXem = countDynamicReport.SoLuongTB;
                    countThongBao.vanBanKiSoChoDuyet = countDynamicReport.SoLuongTBDenChoDuyet;
                    countThongBao.phieuXinXeChoDuyet = countLenhDieuXe.phieuXinXe;
                    countThongBao.lenhDieuXeChoDuyet = countLenhDieuXe.lenhDieuXe;
                    countThongBao.phieuQuanLyXeChoDuyet = countLenhDieuXe.quanLyLenhDieuXe;
                    countThongBao.ungTruocNhienLieuChoDuyet = countLenhDieuXe.ungTruocNhienLieu;
                    countThongBao.quyetToanNhienLieuChoDuyet = countLenhDieuXe.quyetToanNhienLieu;
                    countThongBao.soLoTrinhChoDuyet = countLenhDieuXe.soLoTrinh;
                    countThongBao.donNghiPhepChoDuyet = countLenhDieuXe.donNghiPhep;
                    using (IDbConnection db = new SqlConnection(_cnn))
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();

                        var parameters = new DynamicParameters();
                        parameters.Add("@UserName", User.Identity.Name);

                        var rs = db.QuerySingleOrDefault<NotificationCountsQLCCDto>(
                            "VanBanKiSo_QuanLyChamCong_GetNotificationCounts",
                            parameters, commandType: CommandType.StoredProcedure
                        ) ?? new NotificationCountsQLCCDto();

                        countThongBao.phieuDanhGiaLuuNhap = rs.DraftByMePDG;
                        countThongBao.phieuDanhGiaChoDuyet = rs.WaitingMyApprovalPDG;
                        countThongBao.phieuDanhGiaChoHuy = rs.WaitingRemovedPDG;
                        countThongBao.phieuDanhGiaHuyKy = rs.CanceledPDG;
                        countThongBao.phieuDanhGiaGoDuyet = rs.UnapprovedPDG;

                        countThongBao.bangDanhGiaTongHopLuuNhap = rs.DraftByMeBTH;
                        countThongBao.bangDanhGiaTongHopChoDuyet = rs.WaitingMyApprovalBTH;
                        countThongBao.bangDanhGiaTongHopChoHuy = rs.WaitingRemovedBTH;
                        countThongBao.bangDanhGiaTongHopHuyKy = rs.CanceledBTH;
                        countThongBao.bangDanhGiaTongHopGoDuyet = rs.UnapprovedBTH;
                    }
                    return Ok(countThongBao);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetAccessToken_LenhDieuXe")]
        public async Task<IHttpActionResult> GetAccessToken_LenhDieuXe()
        {
            try
            {
                var result = await GetAccTk_LenhDieuXe();
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetAccessToken_DonNghiPhep")]
        public async Task<IHttpActionResult> GetAccessToken_DonNghiPhep()
        {
            try
            {
                var result = await GetAccTk_DonNghiPhep();
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        #endregion
    }
}