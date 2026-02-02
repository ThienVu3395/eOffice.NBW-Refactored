using Dapper;
using Clone_KySoDienTu.Controllers.BNLE1;
using Clone_KySoDienTu.Models;
using Clone_KySoDienTu.MyHub;
using Clone_KySoDienTu.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Http;
using SteCode;
using System.Net;
using SteCore;
using System.Net.Http;
using SteProject;
using System.Threading.Tasks;
using SmartCAAPI.Dtos.usermanager;

namespace Clone_KySoDienTu.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/userinfo")]
    //public class APICal_SuKienController : ApiController
    public class APIUserController : EntitySetControllerWithHub<EventHub>
    {
        private readonly string _cnn;
        private VCode.AdminModule vc;

        public APIUserController()
        {
            _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
            vc = new VCode.AdminModule(_cnn);
        }

        [Route("getInfoUser")]
        [HttpPost]
        public IHttpActionResult getInfoUser(CommonModel para)
        {
            try
            {
                var result = vc.GetUserInfo(para.valstring1);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        private IEnumerable<tbMenuChucNang> CreateVM(int parentid, List<tbNhomChucnangModel> source)
        {
            return from men in source
                   where men.PARENTID == parentid
                   orderby men.THUTU
                   select new tbMenuChucNang()
                   {
                       par = men,
                       childitem = CreateVM(men.ID, source).ToList()
                   };
        }

        [Route("getUsersMenu")]
        [HttpGet]
        public IHttpActionResult getUsersMenu()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    if (User.IsInRole("Administrators"))
                        parameters.Add("@UserName", "Admin");
                    else
                        parameters.Add("@UserName", User.Identity.Name);
                    var listall = db.Query<tbNhomChucnangModel>("Core_GetUsersMenu", parameters, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                    var a = CreateVM(-1, listall);
                    return Ok(a);

                }
            }
            catch
            {
                return BadRequest();
            }
        }

        //private byte[] getFile()
        //{
        //    string _pdfInput = @"D:\logo.png";
        //    return File.ReadAllBytes(_pdfInput);
        //}
    }

    [AllowAnonymous]
    [RoutePrefix("api/userSmartCA")]
    public class UserSmartCAAPIController : ApiController
    {
        private readonly string _cnn;

        public UserSmartCAAPIController()
        {
            _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
        }

        public int moveFile(string UserName,string TenFile)
        {
            try
            {
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/thienvu.lh/");
                if (Directory.Exists(sPath))
                {
                    if (File.Exists(Path.Combine(sPath, TenFile)))
                    {
                        FileInfo fs = new FileInfo(Path.Combine(sPath, TenFile));

                        string savePath = System.Web.Hosting.HostingEnvironment.MapPath("~/VNPT/SignImg/");
                        if (Directory.Exists(savePath))
                        {
                            savePath = Path.Combine(savePath, UserName);
                            if (!Directory.Exists(savePath))
                            {
                                Directory.CreateDirectory(savePath);
                            }
                            string newPath = Path.Combine(savePath, TenFile);

                            if (File.Exists(newPath))
                            {
                                FileInfo del = new FileInfo(newPath);
                                del.Delete();
                            }
                            fs.MoveTo(newPath);

                            return 1;
                        }
                    }
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        [Route("get-user")]
        [HttpPost]
        public IHttpActionResult GetAll(CreateUserEntryDto para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    string sql = @"select * from [SmartCA.UserManager] where UserName = @UserName";

                    var result = db.Query<CreateUserEntryDto>(sql, new { @UserName = para.UserName.ToLower() }).SingleOrDefault();
                    return Ok(result);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("create-update-user")]
        [HttpPost]
        public IHttpActionResult CreateUpdateUser(CreateUserEntryDto para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    string sql = @"if exists (select top 1 UserName from [SmartCA.UserManager] where UserName = @UserName)
                                    begin
                                    UPDATE [SmartCA.UserManager]
                                      SET UserSmartCA = @UserSmartCA
                                      ,PassSmartCA = @PassSmartCA
                                      ,ImgBase64 = @ImgBase64
                                      ,ImgPath = @ImgPath
                                      ,Khoa = @Khoa
                                      ,DaCoChuKySo = @DaCoChuKySo
                                      WHERE UserName = @UserName
                                     end
                                    else
                                    begin
                                    Insert into [SmartCA.UserManager] (UserName, UserSmartCA, PassSmartCA, ImgBase64, ImgPath, Khoa,DaCoChuKySo)
                                      values (@UserName, @UserSmartCA, @PassSmartCA, @ImgBase64, @ImgPath, @Khoa, @DaCoChuKySo)
                                    end";
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserName", para.UserName.ToLower());
                    parameters.Add("@UserSmartCA", para.UserSmartCA);
                    parameters.Add("@PassSmartCA", para.PassSmartCA);
                    //var imgBytes = File.ReadAllBytes(para.ImgBase64);
                    //parameters.Add("@ImgBase64", Convert.ToBase64String(x));
                    parameters.Add("@ImgBase64", para.ImgBase64);
                    parameters.Add("@ImgPath", para.imgBaseUrl);
                    parameters.Add("@Khoa", para.Khoa);
                    parameters.Add("@DaCoChuKySo", para.DaCoChuKySo);
                    var result = db.Execute(sql, parameters);
                    return Ok(moveFile(para.UserName.ToLower(),para.imgBaseUrl));
                }
            }
            catch
            {
                return BadRequest();
            }
        }
    }

    [Authorize]
    [RoutePrefix("api/fileUpload")]
    public class UploadFilesController : ApiController
    {
        private readonly string _cnn;
        private SteTSK.SteTSK _SteTSK;
        private VCode.WFModule vc;
        private VCode.DeviceModule vc2;
        private VCode.ReportModule vc3;
        private VCode.LenhDieuXeModule vc4;
        private VCode.PRMSModule vSMS;
        public UploadFilesController()
        {
            _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
            _SteTSK = new SteTSK.SteTSK(_cnn);
            vc = new VCode.WFModule(_cnn);
            vc2 = new VCode.DeviceModule(_cnn);
            vc3 = new VCode.ReportModule(_cnn);
            vc4 = new VCode.LenhDieuXeModule(_cnn);
            vSMS = new VCode.PRMSModule(_cnn);
        }

        [HttpPost]
        [Route("UploadFiles")]
        public string UploadFiles()

        {
            try
            {
                int iUploadedCnt = 0;
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/");
                string user = User.Identity.Name;
                sPath += user + @"\";
                if (!Directory.Exists(sPath))
                {
                    Directory.CreateDirectory(sPath);
                }
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];

                    if (hpf.ContentLength > 0)
                    {
                        string xFileName = Path.Combine(sPath, Path.GetFileName(hpf.FileName));
                        if (!File.Exists(xFileName))
                        {
                            hpf.SaveAs(xFileName);
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                        else
                        {
                            FileInfo f = new FileInfo(xFileName);
                            f.Delete();
                            hpf.SaveAs(xFileName);
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                    }
                }

                if (iUploadedCnt > 0)
                {
                    return iUploadedCnt + " Files Uploaded Successfully";
                }
                else
                {
                    return "Upload Failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        [HttpPost]
        [Route("UploadFilesPRMS")]
        public int UploadFilesPRMS()

        {
            try
            {
                int iUploadedCnt = 0;
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/");
                string user = User.Identity.Name;
                sPath += user + @"\";
                if (!Directory.Exists(sPath))
                {
                    Directory.CreateDirectory(sPath);
                }
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];

                    if (hpf.ContentLength > 0)
                    {
                        string xFileName = Path.Combine(sPath, Path.GetFileName(hpf.FileName));
                        if (!File.Exists(xFileName))
                        {
                            hpf.SaveAs(xFileName);
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                        else
                        {
                            FileInfo f = new FileInfo(xFileName);
                            f.Delete();
                            hpf.SaveAs(xFileName);
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                    }
                }

                if (iUploadedCnt > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        [HttpPost]
        [Route("UploadFilesSmartCA")]
        public string UploadFilesSmartCA()

        {
            try
            {
                int iUploadedCnt = 0;
                string result = null;
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/");
                string user = User.Identity.Name;
                sPath += user + @"\";
                if (!Directory.Exists(sPath))
                {
                    Directory.CreateDirectory(sPath);
                }
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];

                    if (hpf.ContentLength > 0)
                    {
                        string xFileName = Path.Combine(sPath, Path.GetFileName(hpf.FileName));
                        if (!File.Exists(xFileName))
                        {
                            hpf.SaveAs(xFileName);
                            var imgBytes = File.ReadAllBytes(xFileName);
                            result = Convert.ToBase64String(imgBytes);
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                        else
                        {
                            FileInfo f = new FileInfo(xFileName);
                            f.Delete();
                            hpf.SaveAs(xFileName);
                            var imgBytes = File.ReadAllBytes(xFileName);
                            result = Convert.ToBase64String(imgBytes);
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                    }
                }

                if (iUploadedCnt > 0)
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [HttpGet]
        [Route("getviewpdf")]
        public HttpResponseMessage getviewpdf(string file)
        {
            string sPath = "";
            sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/");
            sPath = Path.Combine(sPath, User.Identity.Name);
            sPath = Path.Combine(sPath, file);

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = File.Open(sPath, FileMode.Open, FileAccess.Read, FileShare.None); //new System.IO.FileStream(sPath, System.IO.FileMode.Open);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            return response;
        }

        [HttpPost]
        [Route("removefiletemp")]
        public IHttpActionResult removefiletemp(CommonModel par)
        {
            try
            {
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/");
                string user = User.Identity.Name;
                sPath = Path.Combine(sPath, user);
                sPath = Path.Combine(sPath, par.valstring1.checkIsNull());
                if (File.Exists(sPath))
                {
                    FileInfo del = new FileInfo(sPath);
                    del.Delete();
                    return Ok(1);
                }
                return Ok(0);
            }
            catch
            {
                return Ok(0);
            }
        }
        [HttpGet]
        [Route("removeallfiletemp")]
        public IHttpActionResult removeallfiletemp()
        {
            try
            {
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/");
                string user = User.Identity.Name;
                sPath = Path.Combine(sPath, user);
                System.IO.DirectoryInfo di = new DirectoryInfo(sPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                return Ok(1);
            }
            catch
            {
                return Ok(0);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("getviewpdf2")]
        public HttpResponseMessage getviewpdf2(string file, string token)
        {
            try
            {
                var ticket = Startup.OAuthOptions.AccessTokenFormat.Unprotect(token);
                var identity = ticket.Identity;
                if (!identity.IsAuthenticated)
                {
                    throw new Exception();
                }

                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/");
                sPath = Path.Combine(sPath, identity.Name);
                sPath = Path.Combine(sPath, file);

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = File.Open(sPath, FileMode.Open, FileAccess.Read, FileShare.None); //new System.IO.FileStream(sPath, System.IO.FileMode.Open);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                return response;
            }
            catch
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                return response;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("viewpdf2")]
        public HttpResponseMessage viewpdf2(int type, int id, string token)
        {
            try
            {
                var ticket = Startup.OAuthOptions.AccessTokenFormat.Unprotect(token);
                var identity = ticket.Identity;
                if (!identity.IsAuthenticated)
                {
                    throw new Exception();
                }
                string sPath = "";
                #region lấy dữ liệu
                if (type == 1 || type == 2) // Công việc
                {
                    var dirs = _SteTSK.GetFileWF(new SteProject.CommonModel() { valint1 = id, valint2 = type });
                    if (dirs == null)
                        throw new Exception();
                    sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/FileUpload/");
                    sPath = Path.Combine(sPath, dirs.NgayTao.ToString("yyyy"));
                    sPath = Path.Combine(sPath, dirs.NgayTao.ToString("MM"));
                    sPath = Path.Combine(sPath, dirs.TenFile);
                }
                else if (type == 3 || type == 4) //Công Văn
                {
                    if (id == -1)
                        sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/hello.pdf");
                    else
                    {
                        var dirs = vc.LayMotFileVB(id, type == 3 ? 0 : 1, null);
                        sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CongVanFile/");
                        sPath = Path.Combine(sPath, dirs.NGAYTAO?.ToString("yyyy"));
                        sPath = Path.Combine(sPath, dirs.NGAYTAO?.ToString("MM"));
                        if (Path.GetExtension(dirs.TENFILE) == ".docx")
                        {
                            dirs.TENFILE = Path.ChangeExtension(dirs.TENFILE, "pdf");
                        }
                        sPath = Path.Combine(sPath, dirs.TENFILE);
                    }
                }
                else if (type == 5) //GCAL
                {
                    APICal_SuKienController tempCal = new APICal_SuKienController();
                    var dirs = tempCal.getPDFCal(id);
                    if (dirs == null)
                        throw new Exception();
                    sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CalUpload/");
                    sPath = Path.Combine(sPath, dirs.NGAYTAO.ToString("yyyy"));
                    sPath = Path.Combine(sPath, dirs.NGAYTAO.ToString("MM"));
                    sPath = Path.Combine(sPath, dirs.TENFILE);
                }
                else if (type == 6 || type == 7) //Tài Liệu
                {
                    APITaiLieuController tempTaiLieu = new APITaiLieuController();
                    var dirs = tempTaiLieu.getPDFTaiLieu(id, type);
                    if (dirs == null)
                        throw new Exception();
                    sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/DOCData/");
                    sPath = Path.Combine(sPath, dirs.NhomId.ToString());
                    sPath = Path.Combine(sPath, dirs.TenFile);
                }
                else if (type == 8) //SMS
                {
                    var dirs = vSMS.GetFileByFileId(id);
                    if (dirs == null)
                        throw new Exception();
                    sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/SMSData/");
                    sPath = Path.Combine(sPath, dirs.FileName);
                }
                else if (type == 9) //Order
                {
                    APIOtherController tempOrder = new APIOtherController();
                    var dirs = tempOrder.getPDFOrder(id);
                    if (dirs == null)
                        throw new Exception();
                    sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/FileOther/");
                    sPath = Path.Combine(sPath, dirs.NGAYTAO.ToString("yyyy"));
                    sPath = Path.Combine(sPath, dirs.NGAYTAO.ToString("MM"));
                    sPath = Path.Combine(sPath, dirs.TENFILE);
                }
                else if (type == 10) //QLTB
                {
                    var dirs = vc2.LayMotFileTB(id);
                    if (dirs == null)
                        throw new Exception();
                    sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/ThietBiFile/");
                    sPath = Path.Combine(sPath, dirs.NgayTao?.ToString("yyyy"));
                    sPath = Path.Combine(sPath, dirs.NgayTao?.ToString("MM"));
                    sPath = Path.Combine(sPath, dirs.TenFile);
                }
                else if (type == 11) //Report
                {
                    if (id == -1)
                        sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/hello.pdf");
                    else
                    {
                        var dirs = vc3.LayMotFileVB(id, null);
                        sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                        sPath = Path.Combine(sPath, dirs.NGAYTAO?.ToString("yyyy"));
                        sPath = Path.Combine(sPath, dirs.NGAYTAO?.ToString("MM"));
                        if (Path.GetExtension(dirs.TENFILE) == ".docx")
                        {
                            dirs.TENFILE = Path.ChangeExtension(dirs.TENFILE, "pdf");
                        }
                        sPath = Path.Combine(sPath, dirs.TENFILE);
                    }
                }
                else if (type == 14) //Lệnh điều xe
                {
                    if (id == -1)
                        sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/hello.pdf");
                    else
                    {
                        var dirs = vc4.LayMotFileVB(id, null,10);
                        sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                        sPath = Path.Combine(sPath, dirs.NGAYTAO?.ToString("yyyy"));
                        sPath = Path.Combine(sPath, dirs.NGAYTAO?.ToString("MM"));
                        if (Path.GetExtension(dirs.TENFILE) == ".docx")
                        {
                            dirs.TENFILE = Path.ChangeExtension(dirs.TENFILE, "pdf");
                        }
                        sPath = Path.Combine(sPath, dirs.TENFILE);
                    }
                }
                else if (type == 12) //Report cks
                {
                    sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportForm/");
                    sPath = Path.Combine(sPath, "TypeRP_" + id + ".pdf");
                    if (!File.Exists(sPath))
                    {
                        sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/hello.pdf");
                    }
                }
                else if (type == 15) // Quản lý chấm công
                {
                    if (id == -1)
                        sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/hello.pdf");
                    else
                    {
                        var dirs = vc4.LayMotFileVB(id, null, 10);
                        sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/ExportsQLCC/");
                        //sPath = Path.Combine(sPath, dirs.NGAYTAO?.ToString("yyyy"));
                        //sPath = Path.Combine(sPath, dirs.NGAYTAO?.ToString("MM"));
                        if (Path.GetExtension(dirs.TENFILE) == ".docx")
                        {
                            dirs.TENFILE = Path.ChangeExtension(dirs.TENFILE, "pdf");
                        }
                        sPath = Path.Combine(sPath, dirs.TENFILE);
                    }
                }
                else if (type == 16) // Tổng hợp chấm công
                {
                    if (id == -1)
                        sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/hello.pdf");
                    else
                    {
                        var dirs = vc4.LayMotFileVB(id, null, 10);
                        sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/ExportsTHCC/");
                        //sPath = Path.Combine(sPath, dirs.NGAYTAO?.ToString("yyyy"));
                        //sPath = Path.Combine(sPath, dirs.NGAYTAO?.ToString("MM"));
                        if (Path.GetExtension(dirs.TENFILE) == ".docx")
                        {
                            dirs.TENFILE = Path.ChangeExtension(dirs.TENFILE, "pdf");
                        }
                        sPath = Path.Combine(sPath, dirs.TENFILE);
                    }
                }
                #endregion
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = File.Open(sPath, FileMode.Open, FileAccess.Read, FileShare.None); //new System.IO.FileStream(sPath, System.IO.FileMode.Open);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                return response;
            }
            catch
            {
                string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/filetemp.pdf");
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = File.Open(sPath, FileMode.Open, FileAccess.Read, FileShare.None); //new System.IO.FileStream(sPath, System.IO.FileMode.Open);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                return response;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("viewpdf3")]
        public HttpResponseMessage viewpdf3(string filename, string token)
        {
            try
            {
                var ticket = Startup.OAuthOptions.AccessTokenFormat.Unprotect(token);
                var identity = ticket.Identity;
                if (!identity.IsAuthenticated)
                {
                    throw new Exception();
                }
                string sPath = "";
                //var sPathTemp = @"\\192.168.72.82\ShareData\EdocFolder\IncomingEdoc\";
                string sPathTemp = System.Web.Hosting.HostingEnvironment.MapPath("~/TrucCongVanFile/temp/");
                string sPathOfficial = System.Web.Hosting.HostingEnvironment.MapPath("~/TrucCongVanFile/official/");
                if(File.Exists(Path.Combine(sPathTemp, filename)))
                {
                    sPath = Path.Combine(sPathTemp, filename);
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    var stream = File.Open(sPath, FileMode.Open, FileAccess.Read, FileShare.None);//new System.IO.FileStream(sPath, System.IO.FileMode.Open);
                    response.Content = new StreamContent(stream);
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    return response;
                }
                else if (File.Exists(Path.Combine(sPathOfficial, filename)))
                {
                    sPath = Path.Combine(sPathOfficial, filename);
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    var stream = File.Open(sPath, FileMode.Open, FileAccess.Read, FileShare.None);// new System.IO.FileStream(sPath, System.IO.FileMode.Open);
                    response.Content = new StreamContent(stream);
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    return response;
                }
                else throw new Exception();
            }
            catch
            {
                string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/filetemp.pdf");
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = File.Open(sPath, FileMode.Open, FileAccess.Read, FileShare.None);//new System.IO.FileStream(sPath, System.IO.FileMode.Open);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                return response;
            }
        }
    }

    [Authorize]
    [RoutePrefix("api/getUser")]
    public class GetUserController : ApiController
    {
        private readonly string _cnn;

        public GetUserController()
        {
            _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
        }

        #region Get danh sach User

        [HttpGet]
        [Route("getListUser")] //Get List All User - ui-select
        public IHttpActionResult getListUser()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var user = db.Query<Core_UserDto>("Core_GetListUser", null, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                    return Ok(user);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("getAllPhongBan")]
        public IHttpActionResult getAllPhongBan()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var groups = db.Query<Core_GroupsDto>("Core_GetAllPhongBan", null, null, true, null, System.Data.CommandType.StoredProcedure);

                    return Ok(groups);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("getPhongBan")] //Get List Group tree 1
        public IHttpActionResult getPhongBan()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var groups = db.Query<Core_GroupsDto>("Core_GetPhongBan", null, null, true, null, System.Data.CommandType.StoredProcedure);

                    return Ok(groups);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("getPhongBanUser")] //Get List User Group tree 2
        public IHttpActionResult getPhongBanUser(CommonModel para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    List<Core_GroupsDto> abc = new List<Core_GroupsDto>();
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var groups = db.Query<Core_GroupsDto>("Core_GetPhongBan", null, null, true, null, System.Data.CommandType.StoredProcedure);
                    foreach (var item in groups)
                    {

                        var parameters = new DynamicParameters();
                        //parameters.Add("@MAPHONG", item.MAPHONG);
                        parameters.Add("@GroupId", item.GroupId);
                        parameters.Add("@LISTUSER", para.valstring1.checkIsNull());
                        var user = db.Query<Core_UserDto>("Core_GetUserPhongBan", parameters, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                        if (user.Count() > 0)
                        {
                            Core_GroupsDto xyz = new Core_GroupsDto();
                            xyz.GroupId = item.GroupId;
                            xyz.MAPHONG = item.MAPHONG;
                            xyz.TENPHONG = item.TENPHONG;
                            xyz.Users = user;
                            abc.Add(xyz);
                        }

                    }
                    return Ok(abc);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("getPhongBanLanhDao")] //Get List Group tree 1
        public IHttpActionResult getPhongBanLanhDao(CommonModel para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    List<Core_GroupsDto> abc = new List<Core_GroupsDto>();
                    var groups = db.Query<Core_GroupsDto>("Core_GetPhongBanWithList", new { BOPHAN = para.valstring1.checkIsNull() }, null, true, null, System.Data.CommandType.StoredProcedure);
                    foreach (var item in groups)
                    {
                        var parameters = new DynamicParameters();
                        //parameters.Add("@MAPHONG", item.MAPHONG);
                        parameters.Add("@GroupId", item.GroupId);
                        parameters.Add("@LISTUSER", para.valstring2.checkIsNull());
                        var user = db.Query<Core_UserDto>("Core_GetUserLanhDaoPhong", parameters, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                        if (user.Count() > 0)
                        {
                            Core_GroupsDto xyz = new Core_GroupsDto();
                            xyz.MAPHONG = item.MAPHONG;
                            xyz.TENPHONG = item.TENPHONG;
                            xyz.Users = user;
                            abc.Add(xyz);
                        }
                    }
                    return Ok(abc);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("getDangBo")] //Get List dang bo tree 3
        public IHttpActionResult getDangBo(CommonModel para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    List<Core_GroupChucNangDto> abc = new List<Core_GroupChucNangDto>();
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var groups = db.Query<Core_GroupChucNangDto>("Core_GetGroupsIsView", null, null, true, null, System.Data.CommandType.StoredProcedure);
                    foreach (var item in groups)
                    {

                        var parameters = new DynamicParameters();
                        //parameters.Add("@MAPHONG", item.MAPHONG);
                        parameters.Add("@GroupId", item.GroupId);
                        parameters.Add("@LISTUSER", para.valstring1.checkIsNull());
                        var user = db.Query<Core_DangBoUser>("Core_GetGroupChucNang", parameters, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                        if (user.Count() > 0)
                        {
                            Core_GroupChucNangDto xyz = new Core_GroupChucNangDto();
                            xyz.MAPHONG = item.MAPHONG;
                            xyz.TENPHONG = item.TENPHONG;
                            xyz.GroupId = item.GroupId;
                            xyz.ActionType = item.ActionType;
                            xyz.Users = user;
                            abc.Add(xyz);
                        }

                    }
                    return Ok(abc);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("getDanhSachNhanVien")]
        public IHttpActionResult getDanhSachNhanVien(CommonModel par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@loc", par.valint1.checkIsNumber());
                    parameters.Add("@username", User.Identity.Name);
                    var dirs = db.Query<Core_UserDto>("Core_GetDanhSachNhanVien", parameters, null, true, null, System.Data.CommandType.StoredProcedure);
                    return Ok(dirs);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("getChucVu")]
        public IHttpActionResult getChucVu()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var groups = db.Query<CommonModel>("Core_GetChucVu", null, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                    return Ok(groups);
                }
            }
            catch
            {
                return BadRequest();
            }
        }
        #endregion Get danh sach User
    }

    [Authorize]
    [RoutePrefix("api/getCore")]
    public class GetCoreController : ApiController
    {
        private readonly string _cnn;
        private SteCode.Permission abc;
        private SteCore.SteCore _SteCore;
        private VCode.AdminModule vc;
        public GetCoreController()
        {
            _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
            abc = new SteCode.Permission(_cnn);
            _SteCore = new SteCore.SteCore(_cnn);
            vc = new VCode.AdminModule(_cnn);
        }
        #region Test UI
        [HttpPost]
        [Route("Core_GetAllUsersGroupsUI")]
        public IHttpActionResult Core_GetAllUsersGroupsUI(SteProject.CommonModel par)
        {
            try
            {
                return Ok(_SteCore.Core_GetAllUsersGroupsUI(par));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("Core_GetAllUsersGroupsTree")]
        public IHttpActionResult Core_GetAllUsersGroupsTree(SteProject.CommonModel par)
        {
            try
            {
                return Ok(_SteCore.Core_GetAllUsersGroupsTree(par));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("Core_GetUsersActionID")]
        public IHttpActionResult Core_GetUsersActionID(SteProject.CommonModel par)
        {
            try
            {
                return Ok(_SteCore.Core_GetUsersActionID(par));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("Core_DeleteUITreeGroups")]
        public IHttpActionResult Core_DeleteUITreeGroups(SteProject.CommonModel par)
        {
            try
            {
                if (_SteCore.Core_DeleteUITreeGroups(par))
                    return Ok();
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                string a = ex.ToString();
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("Core_InsertUITreeGroups")]
        public IHttpActionResult Core_InsertUITreeGroups(Core_U_AllUsersGroupsUI par)
        {
            try
            {
                if (_SteCore.Core_InsertAllUserGroupsUI(par))
                    return Ok();
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                string a = ex.ToString();
                return BadRequest();
            }
        }
        #endregion
        public int CheckMenusUser(string link)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    if (User.IsInRole("Administrators"))
                        parameters.Add("@UserName", "Admin");
                    else
                        parameters.Add("@UserName", User.Identity.Name);
                    parameters.Add("@Link", link.checkIsNull());
                    return db.ExecuteScalar<int>("Core_CheckMenusUsers", parameters, null, null, System.Data.CommandType.StoredProcedure);
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }
        [HttpGet]
        [Route("getdanhmuchethong")]
        public IHttpActionResult getdanhmuchethong(string loai)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    string qry = @"select tb.[ID], tb.[CODE], tb.[VALUENAME], tb2.[TENDM] as LOAIDM,tb2.[LOAIDM] as VALUENAMECODE from [adm].[tbDanhmuc] tb join [adm].[tbLoaiDM] tb2 on tb.LOAIDM = tb2.LOAIDM where tb.LOAIDM = @LOAIDM";
                    var dirs = db.Query<DataDirControl>(qry, new { LOAIDM = loai });

                    return Ok(dirs);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("getAllGroups")]
        // Lấy danh sách nhóm. Dùng thêm User vào nhóm và phân quyền cho nhóm
        public IHttpActionResult getAllGroups()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    var listall = db.Query<SteCode.Core_Groups>("Core_GetAllGroups", new { IsView = 1 }, null, true, null, System.Data.CommandType.StoredProcedure);
                    return Ok(listall);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("getAllUsersByAdmin")]
        public IHttpActionResult getAllUsersByAdmin(VModel.WFModel.CommonModel para)
        {
            try
            {
                return Ok(vc.GetAllUserByAdmin(para));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("getAllGroups_Admin")]
        public IHttpActionResult getAllGroups_Admin(SteProject.CommonModel par)
        {
            try
            {
                return Ok(_SteCore.AdminGetAllGroups(par));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        #region Cập nhật User của Nhóm
        [HttpPost]
        [Route("getUserGroups")]
        public IHttpActionResult getUserGroups(CommonModel par)
        {
            try
            {
                return Ok(abc.getUserGroups(par.valstring1.checkIsNull()));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("UpdateUserGroups")] // Thêm User vào Nhóm
        public IHttpActionResult UpdateUserGroups(CommonListModel par)
        {
            try
            {
                return Ok(abc.UpdateUserGroups(par));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        #endregion

        [HttpPost]
        [Route("getAllPermissionsOfUserByModuleResource")]
        //Get quyen theo Module + Resource
        public IHttpActionResult getAllPermissionsOfUserByModuleResource(CommonModel par)
        {
            try
            {
                return Ok(abc.getAllPermissionsOfUserByModuleResource(par.valstring1, par.valstring2, par.valstring3, User.Identity.Name));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        #region nhomquyenCtrl phân quyền Nhóm
        [HttpPost]
        [Route("getGroupRoles")]
        public IHttpActionResult getGroupRoles(CommonModel par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    string qr = "select *, (HOLOT + ' ' + TEN) as HOTEN from [dbo].[Core_V_ViewGroupUserList] where GroupId = @GroupId";
                    var listall = db.Query<tbUserDto>(qr, new { GroupId = par.valint1.checkIsNumber() }).ToList();
                    return Ok(listall);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("GetGroupPers")]
        public IHttpActionResult GetGroupPers(CommonModel par)
        {
            try
            {
                return Ok(abc.GetGroupPers(par.valint1.checkIsNumber()));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("UpdateGroupPers")]
        public IHttpActionResult UpdateGroupPers(CommonListPerModel par)
        {
            try
            {

                return Ok(abc.UpdateGroupPers(par));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        #endregion

        #region dsnhanvienCtrl phân quyền User

        [HttpPost]
        [Route("GetUserPers")]
        public IHttpActionResult GetUserPers(CommonModel par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserName", par.valstring1.checkIsNull());
                    using (var multi = db.QueryMultiple("Core_GetUserPers", parameters, null, null, System.Data.CommandType.StoredProcedure))
                    {
                        var list1 = multi.Read<SteCode.Core_Modules>().ToList();
                        var list2 = multi.Read<Core_Pers>().ToList();
                        var list3 = multi.Read<Core_Pers>().ToList();
                        List<Core_GroupPers> abc = new List<Core_GroupPers>();
                        foreach (var item in list1)
                        {
                            Core_GroupPers xxx = new Core_GroupPers();
                            xxx.module = item;
                            xxx.groupPers = list2.Where(x => x.ModuleKey == item.ModuleKey).OrderByDescending(x => x.ResourceName).ThenByDescending(x => x.PermissionName).ToList();
                            abc.Add(xxx);
                        }

                        Core_V_GroupPers vgroup = new Core_V_GroupPers();
                        vgroup.groupfull = abc;
                        vgroup.groupper = list3;
                        return Ok(vgroup);
                    }

                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("UpdateUserPers")]
        public IHttpActionResult UpdateUserPers(CommonListPerModel par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    SteIntListCreator PerIds = new SteIntListCreator();
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserName", par.valstring1.checkIsNull());
                    if (par.listper != null)
                    {
                        foreach (var item in par.listper)
                        {
                            PerIds.AddValue(item.PermissionId);
                        }
                        parameters.Add("@PerIds", PerIds.GetListAndReset());
                    }
                    else
                    {
                        parameters.Add("@PerIds", PerIds.GetListAndReset());
                    }
                    var listall = db.Execute("Core_UpdateUserPers", parameters, null, null, System.Data.CommandType.StoredProcedure);
                    return Ok(1);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        #endregion

        #region Phan quyen theo Module Administrators

        //Get Quyền Nhóm
        [HttpPost]
        [Route("Core_GetGroupPermissionsOfModuleResource")]
        public IHttpActionResult Core_GetGroupPermissionsOfModuleResource(CommonModel par)
        {
            try
            {
                return Ok(abc.Core_GetGroupPermissionsOfModuleResource(par.valstring1.checkIsNull(), par.valstring2.checkIsNull(), par.valstring3.checkIsNull()));

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("UpdateGroupsPermissions")]
        public IHttpActionResult UpdateGroupsPermissions(CommonListUpdateGroupsPermissionsModel par)
        {
            try
            {
                return Ok(abc.UpdateGroupsPermissions(par));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("getAllUsersSortPhongBan")]
        public IHttpActionResult getAllUsersSortPhongBan()
        {
            try
            {
                return Ok(abc.getAllUsersSortPhongBan());
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        //Get Quyền User
        [HttpPost]
        [Route("Core_GetUserPermissionsOfModuleResource")]
        public IHttpActionResult Core_GetUserPermissionsOfModuleResource(CommonModel par)
        {
            try
            {
                return Ok(abc.Core_GetUserPermissionsOfModuleResource(par.valstring1.checkIsNull(), par.valstring2.checkIsNull(), par.valstring3.checkIsNull()));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("UpdateUsersPermissions")]
        public IHttpActionResult UpdateUsersPermissions(CommonListUpdateUsersPermissionsModel par)
        {
            try
            {
                return Ok(abc.UpdateUsersPermissions(par));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        #endregion

        [HttpPost]
        [Route("DeleteGroupsUser")]
        public IHttpActionResult DeleteGroupsUser(CommonModel para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserName", para.valstring1.checkIsNull());
                    parameters.Add("@GroupId", para.valint1.checkIsNumber());
                    var user = db.Execute("Core_DeleteGroupsUser", parameters, null, null, System.Data.CommandType.StoredProcedure);
                    return Ok(user);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

    }

    [Authorize]
    [RoutePrefix("api/utilities")]
    public class UtilitiesController : ApiController
    {

        private static ULTComboBox _cb = new ULTComboBox();
        public UtilitiesController()
        {
        }
        [HttpPost]
        [Route("ultcombobox")]
        public async Task<IHttpActionResult> ULTComboBox(Result<ComboBox> par)
        {
            try
            {
                return Ok(await _cb.GetAll(par));
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
                return BadRequest();
            }
        }
    }
}