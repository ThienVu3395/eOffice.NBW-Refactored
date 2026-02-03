using Clone_KySoDienTu.MyHub;
using static VModel.WFModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VModel;
using Dapper;
using System.Threading.Tasks;
using Clone_KySoDienTu.Controllers.API.SmartCAFunction;
using Clone_KySoDienTu.Controllers.API.SmartCAKyDonLuong;
using static Dapper.SqlMapper;
using Newtonsoft.Json;
using System.Configuration;
using System.Text;
using VCode;
using OAMS;

namespace Clone_KySoDienTu.Controllers.API.QuanLyBaoCao
{
    [Authorize]
    [RoutePrefix("api/QLBaoCao")]
    public class QuanLyBaoCaoAPIController : EntitySetControllerWithHub<EventHub>
    {
        private readonly string _cnn;

        private VCode.ReportModule vc;

        private readonly SmartCAFunctionController _smartCAFunction;

        private readonly APIKyDonLuongController _smartCAKyDonLuong;

        public QuanLyBaoCaoAPIController()
        {
            _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
            vc = new VCode.ReportModule(_cnn);
            _smartCAFunction = new SmartCAFunctionController();
            _smartCAKyDonLuong = new APIKyDonLuongController();
        }

        #region files functions

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

        public string[] GetUsersNotification(string nextUserName)
        {
            try
            {
                string[] us = null;
                switch (nextUserName)
                {
                    case "cnnb":
                        us = new string[] { "tuyetnga.nt", nextUserName };
                        break;
                    case "moccongdoan":
                        us = new string[] { "thuong.tt", nextUserName };
                        break;
                    case "moccvdendb":
                        us = new string[] { "hai.tv", nextUserName };
                        break;
                    case "mocdang":
                        us = new string[] { "tranganh.vn", nextUserName };
                        break;
                    case "mocdoan":
                        us = new string[] { "thuydung.nd", nextUserName };
                        break;
                    case "mocvden":
                        us = new string[] { "hai.tv", nextUserName };
                        break;
                    default:
                        us = new string[] { nextUserName };
                        break;
                }
                return us;
            }
            catch
            {
                return new string[] { };
            }
        }

        public string[] GetUsersNotification(string[] userNames)
        {
            if (userNames.Count() == 0)
            {
                return null;
            }
            return userNames;
        }

        public async Task<object> NotificationApp(string[] userNames, int refId, int module)
        {
            if (userNames == null || userNames.Count() == 0)
                return null;
            try
            {
                string tl = null;
                string it = null;
                string message = null;
                int ist = -1;
                switch (module)
                {
                    case 0:
                        tl = NotificationConstant.Title.CONG_VAN_DEN_HANH_CHINH;
                        it = NotificationConstant.IdType.CONG_VAN_CHI_TIET;
                        ist = NotificationConstant.IdSubType.CONG_VAN_DEN;
                        message = NotificationConstant.Message.CONG_VAN;
                        break;
                    case 1:
                        tl = NotificationConstant.Title.CONG_VAN_DI_HANH_CHINH;
                        it = NotificationConstant.IdType.CONG_VAN_CHI_TIET;
                        ist = NotificationConstant.IdSubType.CONG_VAN_DI;
                        message = NotificationConstant.Message.CONG_VAN;
                        break;
                    case 2:
                        tl = NotificationConstant.Title.VAN_BAN_KY_SO;
                        it = NotificationConstant.IdType.VAN_BAN_KY_SO_CHI_TIET;
                        ist = NotificationConstant.IdSubType.VAN_BAN_KY_SO;
                        message = NotificationConstant.Message.VAN_BAN_KY_SO;
                        break;
                    default:
                        break;
                }
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["EndpointVanBanKySo"]);
                    client.DefaultRequestHeaders.Clear();
                    var requestUri = new Uri($"VanPhongDienTuMobile/notification", UriKind.Relative);
                    VanPhongDienTuMobileNotificationRequest vpdtMobileRequest = new VanPhongDienTuMobileNotificationRequest()
                    {
                        title = tl,
                        body = message,
                        refId = refId,
                        userNames = userNames,
                        idType = it,
                        idSubType = ist,
                    };
                    var deserializedProduct = JsonConvert.SerializeObject(vpdtMobileRequest);
                    var content = new StringContent(deserializedProduct, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(requestUri, content);
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<object>(jsonData);
                    return result;
                }
            }
            catch
            {
                return null;
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
                var dirs = vc.LayMotFileVB(model.valint1, null);
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                sPath = Path.Combine(sPath, dirs.NGAYTAO?.ToString("yyyy"));
                sPath = Path.Combine(sPath, dirs.NGAYTAO?.ToString("MM"));
                sPath = Path.Combine(sPath, dirs.TENFILE);
                if (System.IO.File.Exists(sPath))
                {
                    var listVanBan = vc.LayMotFileVB(model.valint1, dirs.TENFILE);
                    if (listVanBan != null)
                    {
                        FileInfo f = new FileInfo(sPath);

                        f.Delete();
                    }
                }
                if (vc.XoaFileDinhKem(model.valint1) > 0)
                {
                    //Hub.Clients.All.LoadDsUser();
                    return "Remove Success";
                }
                else return "Failed";
            }
        }

        [HttpPost]
        [Route("RemoveFileCanCu")]
        public string RemoveFileCanCu(CommonModel model)
        {
            using (IDbConnection db = new SqlConnection(_cnn))
            {
                if (db.State == System.Data.ConnectionState.Closed)
                {
                    db.Open();
                }
                if (vc.XoaFileCanCu(model.valint1) > 0)
                {
                    return "Remove Success";
                }
                else return "Failed";
            }
        }

        [HttpPost]
        [Route("UploadFile_Update")] // Thêm File trong trường hợp cập nhật dữ liệu
        public IHttpActionResult UploadFile_Update(CommonModel model)
        {
            try
            {
                string tenFile = moveFile(model.valstring1, model.valstring2);
                int result = vc.ThemFileDinhKem(model.valint1, tenFile, model.valstring1, model.valstring2, model.valint2, 0);
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

        [HttpPost]
        [Route("UploadFileCanCu")]
        public IHttpActionResult UploadFileCanCu(FileDinhKemViewModel model)
        {
            try
            {
                int result = vc.ThemFileCanCu(model.VANBANID, model.TENFILE, model.MOTA, model.LOAIFILE, model.NGAYTAO, model.IDFile, model.Module, model.VITRIFILE, model.SoKyHieu, model.SoVanBan, model.NgayBanHanh, model.NhomId);
                if (result > 0)
                {
                    CommonModel rs = new CommonModel();
                    rs.valint1 = result;
                    rs.valstring1 = model.TENFILE;
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
        public HttpResponseMessage Getviewpdfvb(int id)
        {
            using (IDbConnection db = new SqlConnection(_cnn))
            {
                if (db.State == System.Data.ConnectionState.Closed)
                {
                    db.Open();
                }
                var dirs = vc.LayMotFileVB(id, null);
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
        #endregion

        #region xử lý nghiệp vụ
        [HttpPost]
        [Route("GetListVB2")]
        public IHttpActionResult GetDanhSachVB2(FilterModel2 model)
        {
            try
            {
                model.CANBO = User.Identity.Name;
                var dsVB = vc.GetDanhSachVB2(model);
                return Ok(dsVB);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("GetFiles")]
        public IHttpActionResult GetFiles(CommonModel model)
        {
            try
            {
                List<WFModel.FileDinhKemViewModel> result = vc.LayDanhSachFileVB(model.valint1, model.valint3);
                foreach (WFModel.FileDinhKemViewModel item in result)
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
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("GetFilesCanCu")]
        public IHttpActionResult GetFilesCanCu(FilterModel2 model)
        {
            try
            {
                var dsVB = vc.GetFilesCanCu(model);
                return Ok(dsVB);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("GetNextCodeNumber")]
        public IHttpActionResult GetNextCodeNumber(CommonModel model)
        {
            try
            {
                return Ok(vc.GetNextCodeNumber(model.valint1));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("GetVanBanChiTiet2")]
        public IHttpActionResult GetChiTietVB2(CommonModel model)
        {
            try
            {
                CommonModel_BaoCao result = vc.GetChiTietVB2(model.valint1, User.Identity.Name, model.valint3);
                foreach (WFModel.FileDinhKemViewModel item in result.FileDinhKem)
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
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("GetYKienXuLy")]
        public IHttpActionResult GetYKienXuLy(CommonModel model)
        {
            try
            {
                return Ok(vc.GetListCommentVB(model.valint1));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("GuiYKienXuLy")]
        public IHttpActionResult GuiYKienXuLy(CommonModel model)
        {
            try
            {
                model.valstring2 = User.Identity.Name;
                int IDComment = vc.ThemCommentVB(model);
                if (IDComment == 0)
                {
                    return BadRequest();
                }
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("CapNhatButPhe")]
        public IHttpActionResult CapNhatButPhe(CommonModel model)
        {
            model.valstring2 = model.valstring2 == null ? null : model.valstring2.Replace("\n", "<br/>");
            if (vc.CapNhatButPhe(model.valint1, model.valstring1, User.Identity.Name, model.valint2, model.valstring2) != 0)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("CapNhatTrangThaiYKienXuLy")]
        public IHttpActionResult CapNhatTrangThaiYKienXuLy(CommonModel model)
        {
            try
            {
                return Ok(vc.CapNhatTrangThaiCommentVB(model.valint1, model.valint2));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("UpdateNgayMo")]
        public IHttpActionResult UpdateNgayMo(CommonModel model)
        {
            try
            {
                vc.CapNhatNgayMoVB(model.valint1, User.Identity.Name, model.valint2);
                //List<string> userThongBao = vc.LayUserThongBao(model.valint1, model.valint2,0);
                //if (userThongBao.Count > 0)
                //{
                //    Hub.Clients.Groups(userThongBao).countThongBaoReport(0);
                //}
                Hub.Clients.Group(User.Identity.Name).countThongBaoReport(0);
                return Ok();

            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("ThuHoiVB")]
        public IHttpActionResult ThuHoiVB(CommonModel model)
        {
            try
            {
                List<string> userThongBao = vc.LayUserThongBao(model.valint1, model.valint3, 1);
                string result = vc.ChangeTrangThaiVB(model);
                if (result != null)
                {
                    if (userThongBao.Count > 0)
                    {
                        Hub.Clients.Groups(userThongBao).countThongBaoReport(0);
                    }
                    return Ok(result);
                }
                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("HuyKyVB")]
        public async Task<IHttpActionResult> HuyKyVB(CommonModel model)
        {
            try
            {
                var signFileList = await _smartCAFunction.FindFiles(model.valstring1);
                if (signFileList.ToList().Count > 0)
                {
                    foreach (var item in signFileList)
                    {
                        string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                        sPath = Path.Combine(sPath, item.CreatedDate.ToString("yyyy"));
                        sPath = Path.Combine(sPath, item.CreatedDate.ToString("MM"));
                        sPath = Path.Combine(sPath, item.FilePath);
                        System.IO.File.WriteAllBytes(sPath, Convert.FromBase64String(item.UnsignData));
                    }
                }
                string result = vc.ChangeTrangThaiVB(model);
                List<string> userThongBao = vc.LayUserThongBao(model.valint1, model.valint3, 0);
                if (result != null)
                {
                    if (userThongBao.Count > 0)
                    {
                        vc.ChangeSignerFlowVB(model.valint1, model.valint3, 1, null, "Đã hủy ký", User.Identity.Name, 0, 0, model.valint4);
                        Hub.Clients.Groups(userThongBao).countThongBaoReport(0);
                    }
                    return Ok(result);
                }
                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("GoDuyetVB")]
        public async Task<IHttpActionResult> GoDuyetVB(CommonModel model)
        {
            try
            {
                var signFileList = await _smartCAFunction.FindFiles(model.valstring1);
                if (signFileList.ToList().Count > 0)
                {
                    foreach (var item in signFileList)
                    {
                        string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                        sPath = Path.Combine(sPath, item.CreatedDate.ToString("yyyy"));
                        sPath = Path.Combine(sPath, item.CreatedDate.ToString("MM"));
                        sPath = Path.Combine(sPath, item.FilePath);
                        System.IO.File.WriteAllBytes(sPath, Convert.FromBase64String(item.UnsignData));
                    }
                }
                string result = vc.ChangeTrangThaiVB(model);
                List<string> userThongBao = vc.LayUserThongBao(model.valint1, model.valint3, 0);
                if (result != null)
                {
                    if (userThongBao.Count > 0)
                    {
                        Hub.Clients.Groups(userThongBao).countThongBaoReport(0);
                    }
                    return Ok(result);
                }
                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("XoaVanBan")]
        public IHttpActionResult XoaVanBan(CommonModel model)
        {
            try
            {
                var checkResult1 = vc.LayDanhSachFileVB(model.valint1, -1);
                if (checkResult1.Count > 0)
                {
                    deleteFile(checkResult1);
                }
                vc.XoaVB(model.valint1);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("ThemReport")]
        public IHttpActionResult ThemReport(CommonModel_BaoCao para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    string insertSignTable = @"INSERT INTO [dbo].[Report_DynamicCR]
                                            ([valstring1] ,[valstring2] ,[valstring3] ,[valstring4],[valstring5],[valstring6],[valstring7] ,[valstring8],[valstring9],[valstring10],
                                            [valstring11],[valstring12] ,[valstring13],[valstring14],[valstring15],
                                            [valint1] ,[valint2] ,[valint3] ,[valint4],[valint5],[valint6],[valint7] ,[valint8],[valint9],[valint10],
                                            [valdate1] ,[valdate2] ,[valdate3] ,[valdate4],[valdate5],[valdate6],[valdate7] ,[valdate8],[valdate9],[valdate10],
                                            [valtime1] ,[valtime2] ,[valtime3] ,[valtime4],[valtime5],[valtime6],[valtime7] ,[valtime8],[valtime9],[valtime10],
                                            [valdatetime1] ,[valdatetime2] ,[valdatetime3] ,[valdatetime4],[valdatetime5],[valdatetime6],[valdatetime7] ,[valdatetime8],[valdatetime9],[valdatetime10],
                                            [NguoiTao] ,[NgayTao] ,[SmartCAStringID] ,[TrangThai],[Loai],[XemThuTruocFile],[IsCRForm])

                                            VALUES (@valstring1, @valstring2, @valstring3, @valstring4, @valstring5, @valstring6,@valstring7, @valstring8, @valstring9, @valstring10,
                                                    @valstring11,@valstring12, @valstring13, @valstring14, @valstring15,
                                                    @valint1, @valint2, @valint3, @valint4, @valint5, @valint6,@valint7, @valint8, @valint9, @valint10,
                                                    @valdate1, @valdate2, @valdate3, @valdate4, @valdate5, @valdate6,@valdate7, @valdate8, @valdate9, @valdate10,
                                                    @valtime1, @valtime2, @valtime3, @valtime4, @valtime5, @valtime6,@valtime7, @valtime8, @valtime9, @valtime10,
                                                    @valdatetime1, @valdatetime2, @valdatetime3, @valdatetime4, @valdatetime5, @valdatetime6,@valdatetime7, @valdatetime8, @valdatetime9, @valdatetime10,
                                                    @NguoiTao,@NgayTao,@SmartCAStringID,@TrangThai,@Loai,@XemThuTruocFile,@IsCRForm);

                                            set @ReportID = SCOPE_IDENTITY()
                                            RETURN";
                    var parameters = new DynamicParameters();
                    parameters.Add("@valstring1", para.valstring1);
                    parameters.Add("@valstring2", para.valstring2);
                    parameters.Add("@valstring3", para.valstring3); // tên phòng ban nơi đơn vị nhận
                    parameters.Add("@valstring4", para.valstring4);
                    parameters.Add("@valstring5", para.valstring5);
                    parameters.Add("@valstring6", para.valstring6);
                    parameters.Add("@valstring7", para.valstring7);
                    parameters.Add("@valstring8", para.valstring8);
                    parameters.Add("@valstring9", para.valstring9);
                    parameters.Add("@valstring10", para.valstring10);
                    parameters.Add("@valstring11", para.valstring11);
                    parameters.Add("@valstring12", para.valstring12);
                    parameters.Add("@valstring13", para.valstring13);
                    parameters.Add("@valstring14", para.valstring14);
                    parameters.Add("@valstring15", para.valstring15);
                    parameters.Add("@valint1", para.valint1); // mã phòng ban nơi đơn vị nhận
                    parameters.Add("@valint2", para.valint2);
                    parameters.Add("@valint3", para.valint3);
                    parameters.Add("@valint4", para.valint4);
                    parameters.Add("@valint5", para.valint5);
                    parameters.Add("@valint6", para.valint6);
                    parameters.Add("@valint7", para.valint7);
                    parameters.Add("@valint8", para.valint8);
                    parameters.Add("@valint9", para.valint9);
                    parameters.Add("@valint10", para.valint10);
                    parameters.Add("@valdate1", para.valdate1.Year == 1 ? DateTime.Now : para.valdate1);
                    parameters.Add("@valdate2", para.valdate2.Year == 1 ? DateTime.Now : para.valdate2);
                    parameters.Add("@valdate3", para.valdate3.Year == 1 ? DateTime.Now : para.valdate3);
                    parameters.Add("@valdate4", para.valdate4.Year == 1 ? DateTime.Now : para.valdate4);
                    parameters.Add("@valdate5", para.valdate5.Year == 1 ? DateTime.Now : para.valdate5);
                    parameters.Add("@valdate6", para.valdate6.Year == 1 ? DateTime.Now : para.valdate6);
                    parameters.Add("@valdate7", para.valdate7.Year == 1 ? DateTime.Now : para.valdate7);
                    parameters.Add("@valdate8", para.valdate8.Year == 1 ? DateTime.Now : para.valdate8);
                    parameters.Add("@valdate9", para.valdate9.Year == 1 ? DateTime.Now : para.valdate9);
                    parameters.Add("@valdate10", para.valdate10.Year == 1 ? DateTime.Now : para.valdate10);
                    parameters.Add("@valtime1", para.valtime1);
                    parameters.Add("@valtime2", para.valtime2);
                    parameters.Add("@valtime3", para.valtime3);
                    parameters.Add("@valtime4", para.valtime4);
                    parameters.Add("@valtime5", para.valtime5);
                    parameters.Add("@valtime6", para.valtime6);
                    parameters.Add("@valtime7", para.valtime7);
                    parameters.Add("@valtime8", para.valtime8);
                    parameters.Add("@valtime9", para.valtime9);
                    parameters.Add("@valtime10", para.valtime10);
                    parameters.Add("@valdatetime1", para.valdatetime1.Year == 1 ? DateTime.Now : para.valdatetime1);
                    parameters.Add("@valdatetime2", para.valdatetime2.Year == 1 ? DateTime.Now : para.valdatetime2);
                    parameters.Add("@valdatetime3", para.valdatetime3.Year == 1 ? DateTime.Now : para.valdatetime3);
                    parameters.Add("@valdatetime4", para.valdatetime4.Year == 1 ? DateTime.Now : para.valdatetime4);
                    parameters.Add("@valdatetime5", para.valdatetime5.Year == 1 ? DateTime.Now : para.valdatetime5);
                    parameters.Add("@valdatetime6", para.valdatetime6.Year == 1 ? DateTime.Now : para.valdatetime6);
                    parameters.Add("@valdatetime7", para.valdatetime7.Year == 1 ? DateTime.Now : para.valdatetime7);
                    parameters.Add("@valdatetime8", para.valdatetime8.Year == 1 ? DateTime.Now : para.valdatetime8);
                    parameters.Add("@valdatetime9", para.valdatetime9.Year == 1 ? DateTime.Now : para.valdatetime9);
                    parameters.Add("@valdatetime10", para.valdatetime10.Year == 1 ? DateTime.Now : para.valdatetime10);
                    parameters.Add("@NguoiTao", User.Identity.Name);
                    parameters.Add("@NgayTao", DateTime.Now);
                    parameters.Add("@SmartCAStringID", para.SmartCAStringID);
                    parameters.Add("@TrangThai", para.TrangThai);
                    parameters.Add("@Loai", para.Loai);
                    parameters.Add("@XemThuTruocFile", para.XemThuTruocFile);
                    parameters.Add("@IsCRForm", para.IsCRForm);
                    parameters.Add("@ReportID", null, DbType.Int32, ParameterDirection.Output);
                    var result = db.Execute(insertSignTable, parameters);
                    if (result > 0)
                    {
                        int newId = parameters.Get<int>("@ReportID");
                        if (para.FileDinhKem != null)
                        {
                            foreach (var item in para.FileDinhKem)
                            {
                                string filename = moveFile(item.MOTA, item.LOAIFILE);
                                vc.ThemFileDinhKem(newId, filename, item.MOTA, item.LOAIFILE, item.SIZEFILE, 0);
                            }
                        }
                        if (para.FileCanCu != null)
                        {
                            foreach (var item in para.FileCanCu)
                            {
                                vc.ThemFileCanCu(newId, item.TENFILE, item.MOTA, item.LOAIFILE, item.NGAYTAO, item.IDFile, item.Module , item.VITRIFILE, item.SoKyHieu, item.SoVanBan , item.NgayBanHanh, item.NhomId);
                            }
                        }
                        if (para.NhomPhanCong != null)
                        {
                            foreach (var item in para.NhomPhanCong)
                            {
                                vc.ThemNhomPhanCong(newId, 0, item.GroupId, item.FullName, item.LOAIXULY, User.Identity.Name);
                            }
                        }
                        if (para.UserPhanCong != null)
                        {
                            foreach (var item in para.UserPhanCong)
                            {
                                vc.ThemCaNhan(newId, item.GroupId, item.UserName, item.LOAIXULY, User.Identity.Name, 0);
                            }
                        }
                        vc.ChangeSignerFlowVB(newId, 2, 0, para.DanhSachNguoiKy, null, null, 0, 0, 0);
                        return Ok(newId);
                    }
                    return BadRequest();
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("CapNhatThongTinVanBan2")]
        public IHttpActionResult CapNhatThongTinVanBan2(CommonModel_BaoCao para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    string updateQuery = @"UPDATE [dbo].[Report_DynamicCR]
                                           SET  [valstring1] = @valstring1, [valstring2] = @valstring2, [valstring3] = @valstring3, [valstring4] = @valstring4, [valstring5] = @valstring5,
                                                [valstring6] = @valstring6, [valstring7] = @valstring7, [valstring8] = @valstring8, [valstring9] = @valstring9, [valstring10] = @valstring10,
                                                [valstring11] = @valstring11, [valstring12] = @valstring12, [valstring13] = @valstring13, [valstring14] = @valstring14, [valstring15] = @valstring15,
                                                [valint1] = @valint1, [valint2] = @valint2, [valint3] = @valint3, [valint4] = @valint4, [valint5] = @valint5,
                                                [valint6] = @valint6, [valint7] = @valint7, [valint8] = @valint8, [valint9] = @valint9, [valint10] = @valint10,
                                                [valdate1] = @valdate1, [valdate2] = @valdate2, [valdate3] = @valdate3, [valdate4] = @valdate4, [valdate5] = @valdate5,
                                                [valdate6] = @valdate6, [valdate7] = @valdate7, [valdate8] = @valdate8, [valdate9] = @valdate9, [valdate10] = @valdate10,
                                                [valtime1] =  @valtime1, [valtime2] = @valtime2, [valtime3] = @valtime3, [valtime4] = @valtime4, [valtime5] = @valtime5,
                                                [valtime6] = @valtime6, [valtime7] = @valtime7, [valtime8] = @valtime8, [valtime9] = @valtime9, [valtime10] = @valtime10,
                                                [valdatetime1] = @valdatetime1, [valdatetime2] = @valdatetime2, [valdatetime3] = @valdatetime3, [valdatetime4] = @valdatetime4, [valdatetime5] = @valdatetime5,
                                                [valdatetime6] = @valdatetime6, [valdatetime7] = @valdatetime7, [valdatetime8] = @valdatetime8, [valdatetime9] = @valdatetime9, [valdatetime10] = @valdatetime10,
                                                [NguoiCapNhat] = @NguoiCapNhat, [NgayCapNhat] = @NgayCapNhat, [Loai] = @Loai, [XemThuTruocFile] = @XemThuTruocFile, [IsCRForm] = @IsCRForm

                                           WHERE ID = @ID ;";
                    var parameters = new DynamicParameters();
                    parameters.Add("@valstring1", para.valstring1);
                    parameters.Add("@valstring2", para.valstring2);
                    parameters.Add("@valstring3", para.valstring3);
                    parameters.Add("@valstring4", para.valstring4);
                    parameters.Add("@valstring5", para.valstring5);
                    parameters.Add("@valstring6", para.valstring6);
                    parameters.Add("@valstring7", para.valstring7);
                    parameters.Add("@valstring8", para.valstring8);
                    parameters.Add("@valstring9", para.valstring9);
                    parameters.Add("@valstring10", para.valstring10);
                    parameters.Add("@valstring11", para.valstring11);
                    parameters.Add("@valstring12", para.valstring12);
                    parameters.Add("@valstring13", para.valstring13);
                    parameters.Add("@valstring14", para.valstring14);
                    parameters.Add("@valstring15", para.valstring15);
                    parameters.Add("@valint1", para.valint1);
                    parameters.Add("@valint2", para.valint2);
                    parameters.Add("@valint3", para.valint3);
                    parameters.Add("@valint4", para.valint4);
                    parameters.Add("@valint5", para.valint5);
                    parameters.Add("@valint6", para.valint6);
                    parameters.Add("@valint7", para.valint7);
                    parameters.Add("@valint8", para.valint8);
                    parameters.Add("@valint9", para.valint9);
                    parameters.Add("@valint10", para.valint10);
                    parameters.Add("@valdate1", para.valdate1.Year == 1 ? DateTime.Now : para.valdate1);
                    parameters.Add("@valdate2", para.valdate2.Year == 1 ? DateTime.Now : para.valdate2);
                    parameters.Add("@valdate3", para.valdate3.Year == 1 ? DateTime.Now : para.valdate3);
                    parameters.Add("@valdate4", para.valdate4.Year == 1 ? DateTime.Now : para.valdate4);
                    parameters.Add("@valdate5", para.valdate5.Year == 1 ? DateTime.Now : para.valdate5);
                    parameters.Add("@valdate6", para.valdate6.Year == 1 ? DateTime.Now : para.valdate6);
                    parameters.Add("@valdate7", para.valdate7.Year == 1 ? DateTime.Now : para.valdate7);
                    parameters.Add("@valdate8", para.valdate8.Year == 1 ? DateTime.Now : para.valdate8);
                    parameters.Add("@valdate9", para.valdate9.Year == 1 ? DateTime.Now : para.valdate9);
                    parameters.Add("@valdate10", para.valdate10.Year == 1 ? DateTime.Now : para.valdate10);
                    parameters.Add("@valtime1", para.valtime1);
                    parameters.Add("@valtime2", para.valtime2);
                    parameters.Add("@valtime3", para.valtime3);
                    parameters.Add("@valtime4", para.valtime4);
                    parameters.Add("@valtime5", para.valtime5);
                    parameters.Add("@valtime6", para.valtime6);
                    parameters.Add("@valtime7", para.valtime7);
                    parameters.Add("@valtime8", para.valtime8);
                    parameters.Add("@valtime9", para.valtime9);
                    parameters.Add("@valtime10", para.valtime10);
                    parameters.Add("@valdatetime1", para.valdatetime1.Year == 1 ? DateTime.Now : para.valdatetime1);
                    parameters.Add("@valdatetime2", para.valdatetime2.Year == 1 ? DateTime.Now : para.valdatetime2);
                    parameters.Add("@valdatetime3", para.valdatetime3.Year == 1 ? DateTime.Now : para.valdatetime3);
                    parameters.Add("@valdatetime4", para.valdatetime4.Year == 1 ? DateTime.Now : para.valdatetime4);
                    parameters.Add("@valdatetime5", para.valdatetime5.Year == 1 ? DateTime.Now : para.valdatetime5);
                    parameters.Add("@valdatetime6", para.valdatetime6.Year == 1 ? DateTime.Now : para.valdatetime6);
                    parameters.Add("@valdatetime7", para.valdatetime7.Year == 1 ? DateTime.Now : para.valdatetime7);
                    parameters.Add("@valdatetime8", para.valdatetime8.Year == 1 ? DateTime.Now : para.valdatetime8);
                    parameters.Add("@valdatetime9", para.valdatetime9.Year == 1 ? DateTime.Now : para.valdatetime9);
                    parameters.Add("@valdatetime10", para.valdatetime10.Year == 1 ? DateTime.Now : para.valdatetime10);
                    parameters.Add("@NguoiCapNhat", User.Identity.Name);
                    parameters.Add("@NgayCapNhat", DateTime.Now);
                    parameters.Add("@Loai", para.Loai);
                    parameters.Add("@XemThuTruocFile", para.XemThuTruocFile);
                    parameters.Add("@IsCRForm", para.IsCRForm);
                    parameters.Add("@ID", para.ID);
                    var result = db.Execute(updateQuery, parameters);
                    if (result > 0)
                    {
                        var checkResult1 = vc.LayDanhSachFileVB(para.ID, 1);
                        if (checkResult1.Count > 0)
                        {
                            deleteFile(checkResult1);
                            string deleteQuery = @"DELETE FROM [dbo].[Report_FileDinhKem] WHERE [VANBANID] = @ID AND [IsCRFile] = 1";
                            db.Execute(deleteQuery, new { @ID = para.ID });
                        }
                        vc.ChangeSignerFlowVB(para.ID, 2, 0, para.DanhSachNguoiKy, null, null, 0, 0,0);
                        return Ok(para.ID);
                    }
                    return Ok(0);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("CapNhatDieuChinh")]
        public IHttpActionResult CapNhatDieuChinh(VanBanViewModel model)
        {
            model.NguoiCapNhat = User.Identity.Name;
            if (vc.CapNhatDieuChinh(model) != 0)
            {
                return Ok();
            }
            return BadRequest();
        }
        #endregion

        #region Xử lý nơi nhận văn bản/phân phát văn bản
        [HttpPost]
        [Route("GetUsersNhanVBTheoPhongBan")]
        public IHttpActionResult GetUsersNhanVBTheoPhongBan(CommonModel model)
        {
            try
            {
                return Ok(vc.GetUserNhanVBTheoPhongBan(model.valint1));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("GetUserGroupId")]
        public IHttpActionResult GetUserGroupId(CommonModel model)
        {
            try
            {
                var groupids = vc.GetGroupIdByIDAction(model.valint1, model.valint2, User.Identity.Name);
                List<Core_UserDto> result = new List<Core_UserDto>();
                if (groupids.Count > 0)
                {
                    foreach (var item in groupids)
                    {
                        var us = vc.GetUserTheoGroupID(item.GroupId);
                        result.AddRange(us);
                    }
                }
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("getUserThamGiaVanBan")]
        public IHttpActionResult GetUserThamGiaVB(CommonModel model)
        {
            try
            {
                return Ok(vc.GetUserThamGiaVB(model.valint1, User.Identity.Name, model.valint2));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("getNhomThamGiaVanBan")]
        public IHttpActionResult GetNhomThamGiaVanBan(CommonModel model)
        {
            try
            {
                return Ok(vc.GetNhomThamGiaVB(model.valint1, model.valint2));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("GetUsersTheoVaiTroNguoiPhanPhat")]
        public IHttpActionResult GetUsersTheoVaiTroNguoiPhanPhat(CommonModel model)
        {
            try
            {
                var groupids = vc.GetGroupIdByIDAction(model.valint1, model.valint2, User.Identity.Name);
                return Ok(groupids);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("XoaNhomPhanCong")]
        public IHttpActionResult XoaNhomPhanCong(CommonModel model)
        {
            try
            {
                List<string> userThongBao = vc.LayUserThongBao(model.valint1, model.valint2, 1);
                vc.XoaNhomPhanCong(model.valint1, model.valint2, model.valint3);
                if (userThongBao.Count > 0)
                {
                    Hub.Clients.Groups(userThongBao).countThongBaoReport(0);
                }
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("ThemNhomPhanCong")]
        public IHttpActionResult ThemNhomPhanCong(CommonModel model)
        {
            try
            {
                int IDNhom = vc.ThemNhomPhanCong(model.valint1, model.valint2, model.valint3, model.valstring1, model.valint4, User.Identity.Name);
                if (IDNhom != 0)
                {
                    List<string> userThongBao = vc.LayUserThongBao(model.valint1, model.valint2, 1);
                    if (userThongBao.Count > 0)
                    {
                        Hub.Clients.Groups(userThongBao).countThongBaoReport(0);
                    }
                    return Ok(IDNhom);
                }
                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("ThemUserPhanCong")]
        public IHttpActionResult ThemUserPhanCong(CommonModel model)
        {
            try
            {
                var UserVB = vc.GetIDUserVB(model.valint1, model.valint3, User.Identity.Name, model.valint2);
                //int ThemCanBo = UserVB.UserID.ToInt() != 0 ? vc.ThemCanBoPhanCong(UserVB.UserID.ToInt(), model.valstring1, model.valint4, UserVB.UserIDVB, User.Identity.Name) : vc.ThemCaNhan(model.valint1, model.valint2, model.valint3, model.valstring1, model.valint4, User.Identity.Name, UserVB.UserIDVB);
                int ThemCanBo = vc.ThemCanBoPhanCong(UserVB.UserID.ToInt(), model.valstring1, model.valint4, UserVB.UserIDVB, User.Identity.Name);
                if (ThemCanBo > 0)
                {
                    List<string> userThongBao = vc.LayUserThongBao(model.valint1, model.valint2, 1);
                    if (userThongBao.Count > 0)
                    {
                        Hub.Clients.Groups(userThongBao).countThongBaoReport(0);
                    }
                    return Ok(ThemCanBo);
                }
                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("XoaUserPhanCong")]
        public IHttpActionResult XoaUserPhanCong(CommonModel model)
        {
            try
            {
                var result = vc.GetVBCanBoDeQuy_Nhom(model.valint2, model.valstring1, model.valint3, model.valint4);
                if (result.dsCanBo.Count > 0)
                {
                    foreach (var item in result.dsCanBo)
                    {
                        vc.XoaCanBoPhanCong(item.ID, model.valint2, model.valint3, model.valint4);
                        Hub.Clients.Group(item.CANBO).countThongBaoVB(0);
                    }
                    List<string> userThongBao = vc.LayUserThongBao(model.valint2, model.valint3, 1);
                    if (userThongBao.Count > 0)
                    {
                        Hub.Clients.Groups(userThongBao).countThongBaoReport(0);
                    }
                    return Ok();
                }
                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("ThemCaNhan")]
        public IHttpActionResult ThemCaNhan(CommonModel model)
        {
            try
            {
                int ThemCanBo = vc.ThemCaNhan(model.valint1, model.valint3, model.valstring1, model.valint4, User.Identity.Name, model.valint5);
                if (ThemCanBo > 0)
                {
                    List<string> userThongBao = vc.LayUserThongBao(model.valint1, model.valint2, 1);
                    if (userThongBao.Count > 0)
                    {
                        Hub.Clients.Groups(userThongBao).countThongBaoReport(0);
                    }
                    return Ok(ThemCanBo);
                }
                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("XoaCaNhan")]
        public IHttpActionResult XoaCaNhan(CommonModel model)
        {
            try
            {
                var result = vc.GetVBCanBoDeQuy_CaNhan(model.valint1, model.valstring1, model.valint2);
                if (result.dsCanBo.Count > 0)
                {
                    foreach (var item in result.dsCanBo)
                    {
                        vc.XoaCaNhan(model.valint1, item.CANBO);
                    }
                    List<string> userThongBao = vc.LayUserThongBao(model.valint1, model.valint2, 1);
                    if (userThongBao.Count > 0)
                    {
                        Hub.Clients.Groups(userThongBao).countThongBaoReport(0);
                    }
                    return Ok();
                }
                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }
        #endregion

        #region không sử dụng
        [HttpPost]
        [Route("GetListVB")]
        public IHttpActionResult GetDanhSachVB(FilterModel2 model)
        {
            try
            {
                model.CANBO = User.Identity.Name;
                var dsVB = vc.GetDanhSachVB(model);
                return Ok(dsVB);
            }
            catch
            {
                return BadRequest();
            }
        }
        
        [HttpPost]
        [Route("GetVanBanChiTiet")]
        public IHttpActionResult GetChiTietVB(CommonModel model)
        {
            try
            {
                WFModel.VanBanViewModel result = vc.GetChiTietVB(model.valint1, User.Identity.Name, model.valint3);
                foreach (WFModel.FileDinhKemViewModel item in result.FileDinhKem)
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
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("ThemVanBan")]
        public IHttpActionResult ThemVanBan(VanBanViewModel para)
        {
            try
            {
                para.NguoiTao = User.Identity.Name;
                int CongVanID = vc.ThemVB(para);
                if (CongVanID != 0)
                {
                    if (para.FileDinhKem != null)
                    {
                        foreach (var item in para.FileDinhKem)
                        {
                            string filename = moveFile(item.MOTA, item.LOAIFILE);
                            vc.ThemFileDinhKem(CongVanID, filename, item.MOTA, item.LOAIFILE, item.SIZEFILE, 0);
                        }
                    }
                    vc.ChangeSignerFlowVB(CongVanID, para.Query, 0, para.DanhSachNguoiKy, null, null, 0, 0,0);
                    return Ok(CongVanID);
                }
                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("CapNhatThongTinVanBan")]
        public IHttpActionResult CapNhatThongTin(VanBanViewModel model)
        {
            try
            {
                model.NguoiCapNhat = User.Identity.Name;
                if (vc.CapNhatVB(model) > 0)
                {
                    vc.ChangeSignerFlowVB(model.ID, model.Query, 0, model.DanhSachNguoiKy, null, null, 0, 0,0);
                    return Ok();
                }
                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }
        #endregion
    }
}