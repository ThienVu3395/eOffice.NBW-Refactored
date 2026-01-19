using Dapper;
using Clone_KySoDienTu.Controllers.Congviec;
using Clone_KySoDienTu.Models;
using Clone_KySoDienTu.MyHub;
using Clone_KySoDienTu.Service.Dtos;
using SteProject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Clone_KySoDienTu.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/cal_sukien")]
    //public class APICal_SuKienController : ApiController
    public class APICal_SuKienController : EntitySetControllerWithHub<EventHub>
    {
        private readonly string _cnn;
        private smsVT sms = new smsVT();
        private smsTriAnh smstrianh = new smsTriAnh();
        private readonly int perPage = 30;
        
        public APICal_SuKienController()
        {
            _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
        }

        #region private function
        //public cnnb_ws.result SendSMS(string sodt, string tinnhan)
        //{
        //    sms.UserID = sodt;
        //    sms.Content = tinnhan;
        //    cnnb_ws.CcApiClient abc = new cnnb_ws.CcApiClient();
        //    return abc.wsCpMt(sms.User, sms.Password, sms.CPCode, sms.RequestID, sms.UserID, sms.UserID, sms.ServiceID, sms.CommandCode, sms.Content, sms.ContentType);
        //}

        public async Task<DataObject> SendSMS(string sodt, string tinnhan)
        {
            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (msg, cert, chain, errors) => true
                };

                smstrianh.Des = sodt;
                smstrianh.Message = tinnhan;
                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(15);
                client.BaseAddress = new Uri(smstrianh.UrlAPI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", smstrianh.Token);
                HttpResponseMessage response = await client.PostAsJsonAsync(client.BaseAddress, smstrianh).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsAsync<DataObject>().Result;
                    return dataObjects;
                }
                else
                    throw new Exception();
            }
            catch
            {
                DataObject obj = new DataObject();
                obj.status = "FAIL";
                return obj;
            }
        }
        //"SUCCESS"
        private string SMSInfo(Cal_SuKienDto par)
        {
            return par.TieuDe + ".\n"
                + "Thời gian: " + SYSTEM.DateToString(par.BatDau, "HH:mm") + ' ' + SYSTEM.DateToString(par.BatDau, "dd/MM/yyyy") + ".\n"
                + (par.DiaDiem   != null  ? ("Địa điểm: "   + par.DiaDiem + ".\n") : "")
                + (par.ChuTri    != null  ? ("Chủ trì: "    + par.ChuTri + ".\n") : "")
                + (par.ThanhPhan != null  ? ("Thành phần: " + par.ThanhPhan + ".\n") : "")
                + (par.ChuanBi   != null  ? ("Chuẩn bị: "   + par.ChuanBi + ".\n") : "")
                + (par.KhachMoi  != null  ? ("Khách mời: "  + par.KhachMoi + ".") : "");
        }
        private string SMSInfoDatxe(Cal_SuKienDto par)
        {
            return par.TieuDe + ".\n"
                + "Ngày đi: " + SYSTEM.DateToString(par.BatDau, "HH:mm") + ' ' + SYSTEM.DateToString(par.BatDau, "dd/MM/yyyy") + ".\n"
                + (par.KetThuc != null ? ("Ngày về: " + SYSTEM.DateToString(par.KetThuc, "HH:mm") + ' ' + SYSTEM.DateToString(par.KetThuc, "dd/MM/yyyy") + ".\n") : "")
                + (par.DiaDiem != null ? ("Địa điểm: " + par.DiaDiem + ".\n") : "")
                //+ (par.ChuTri != null ? ("Chủ trì: " + par.ChuTri + ".\n") : "")
                //+ (par.ThanhPhan != null ? ("Thành phần: " + par.ThanhPhan + ".\n") : "")
                + (par.ChuanBi != null ? ("Tài xế: " + par.ChuanBi + ".") : "");
        }
        public bool GetUserDt(int i, string nguoithongbao, string tinnhan, out string log)
        {
            string qry = "";
            log = "";
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@DSGuiTin", nguoithongbao);
                    qry = i == 1 ? "SMS_GetUserDt" : "SMS_GetUserDt2";
                    var groups = db.Query<Core_UserDto>(qry, parameters, null, true, null, System.Data.CommandType.StoredProcedure);
                    if (groups.Count() > 0)
                    {
                        //foreach (var item in groups)
                        //{
                        //    SendSMS(item.Mobile, tinnhan);
                        //}
                        //return true;
                        foreach (var item in groups)
                        {
                            if (SendSMS(item.Mobile, tinnhan).Result.status.Contains("SUCCESS"))
                            {
                                log = log + "1,";
                            }
                            else
                                log = log + "0,";
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static string GetWeekInYear(DateTime datetime, out DateTime FirstDayOfWeek, out int CurrentWeek)
        {
            string txt = "";
            CultureInfo myCI = new CultureInfo(System.Globalization.CultureInfo.CurrentCulture.LCID);
            //System.Globalization.CultureInfo.CurrentCulture.LCID lay theo he thong
            System.Globalization.Calendar myCal = myCI.Calendar;

            // Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            // Displays the number of the current week relative to the beginning of the year.
            //CurrentWeek = myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW);
            CurrentWeek = myCal.GetWeekOfYear(datetime, myCWR, myFirstDOW);
            //FirstDayOfWeek = DateTime.Now;
            FirstDayOfWeek = datetime;
            while (FirstDayOfWeek.DayOfWeek != DayOfWeek.Monday) FirstDayOfWeek = FirstDayOfWeek.AddDays(-1); // tìm đầu tuần

            //LastDayOfWeek = FirstDayOfWeek.AddDays(6);
            // Displays the total number of weeks in the current year.
            DateTime LastDay = new System.DateTime(DateTime.Now.Year, 12, 31);
            return txt;
        }

        private string MoveFile(string TenFile, string LoaiFile)
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

                        string savePath = System.Web.Hosting.HostingEnvironment.MapPath("~/CalUpload/");
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
                            savePath = Path.Combine(savePath, TenFile);

                            if (File.Exists(savePath))
                            {
                                FileInfo del = new FileInfo(savePath);
                                del.Delete();
                            }
                            fs.MoveTo(savePath);
                            File.Move(savePath, newPath);
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

        private void Uploadfile(List<GCAL_File> listfile, int idSuKien)
        {
            using (IDbConnection db = new SqlConnection(_cnn))
            {
                if (db.State == System.Data.ConnectionState.Closed)
                    db.Open();
                List<DynamicParameters> parlist = new List<DynamicParameters>();
                foreach (var par in listfile)
                {
                    var parafile = new DynamicParameters();
                    parafile.Add("@LOAI", 1);
                    parafile.Add("@MOTA", par.MoTa.checkIsNull());
                    parafile.Add("@SuKienId", idSuKien);
                    parafile.Add("@LOAIFILE", par.LoaiFile.checkIsNull());
                    parafile.Add("@SIZEFILE", par.SizeFile.checkIsNumber());
                    parafile.Add("@TENFILE", MoveFile(par.MoTa.checkIsNull(), par.LoaiFile.checkIsNull()));
                    parlist.Add(parafile);

                }
                db.Execute("Cal_ThemFile", parlist, null, null, System.Data.CommandType.StoredProcedure);
            }
        }
        private int DeleteFile(List<ViewfilePDF> filelist)
        {
            try
            {
                int iUploadedCnt = 0;
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CalUpload/");
                for (int i = 0; i < filelist.Count; i++)
                {
                    sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CalUpload/");
                    sPath = Path.Combine(sPath, filelist[i].NGAYTAO.ToString("yyyy"));
                    sPath = Path.Combine(sPath, filelist[i].NGAYTAO.ToString("MM"));
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
        
        private void SaveLogSMSNow (int SukienId,string ThongBao ,string logsms)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SuKienId", SukienId);
                    parameters.Add("@NgayTao", DateTime.Now);
                    parameters.Add("@NgayGui", DateTime.Now);
                    parameters.Add("@ThongBao", ThongBao);
                    parameters.Add("@TrangThai", logsms);
                    db.Execute("SMS_HengioSaveLogNow", parameters, null, null, System.Data.CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }
        }

        // Hàm lưu log và gửi sms dành cho các user trong các module có áp dụng kí số
        public string GetUserMobile_VBKS(string nguoithongbao, string tinnhan)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    string log = null;
                    var parameters = new DynamicParameters();
                    parameters.Add("@DSGuiTin", nguoithongbao);
                    var groups = db.Query<Core_UserDto>("SMS_GetUserDt", parameters, null, true, null, System.Data.CommandType.StoredProcedure);
                    if (groups.Count() > 0)
                    {
                        foreach (var item in groups)
                        {
                            if (SendSMS(item.Mobile, tinnhan).Result.status.Contains("SUCCESS"))
                            {
                                log = log + "1,";
                            }
                            else
                            {
                                log = log + "0,";
                            }
                        }
                        return log;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public bool SaveLogSMS_VBKS(int SukienId, string ThongBao, string logsms, int module)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SuKienId", SukienId);
                    parameters.Add("@NgayTao", DateTime.Now);
                    parameters.Add("@NgayGui", DateTime.Now);
                    parameters.Add("@ThongBao", ThongBao);
                    parameters.Add("@TrangThai", logsms);
                    parameters.Add("@Module", module);
                    if(db.Execute("Report_ThemSMSLog", parameters, null, null, System.Data.CommandType.StoredProcedure) > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
                return false;
            }
        }
        #endregion

        #region Load data
        [Route("getAll")]
        [HttpPost]
        public IHttpActionResult GetAll(CommonModel para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@NhomId", para.valint1.checkIsNumber());
                    return Ok(db.Query<Cal_SuKienDto>("Cal_GetCalAll", parameters, null, true, null, System.Data.CommandType.StoredProcedure).ToList());
                }
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("getday")]
        public IHttpActionResult Getday(CommonSearch para)
        {
            DateTime dateValue = para.valtime1;
            DateTime firstday;
            int a;
            GetWeekInYear(dateValue, out firstday, out a);
            var abc = new List<Cal_SuKien7>();

            int i = 0;
            while (i < 7)
            {
                DateTime temp1 = firstday.AddDays(i);
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    using (var multi = db.QueryMultiple("Cal_GetSuKien7", new { date = temp1, GroupID = para.GroupID.checkIsNumber() }, null, null, System.Data.CommandType.StoredProcedure))
                    {
                        Cal_SuKien7 ds = new Cal_SuKien7();
                        var vrs1 = multi.Read<Cal_SuKien7_1>().ToList();
                        var vrs2 = multi.Read<Cal_SuKien7_1>().ToList();
                        ds.week = a;
                        ds.NgayThang = temp1;
                        ds.All = (vrs1.Count() == 0 ? 1 : vrs1.Count()) + (vrs2.Count() == 0 ? 1 : vrs2.Count());
                        ds.Sang = vrs1.Count() == 0 ? 1: vrs1.Count();
                        ds.Chieu = vrs2.Count() == 0 ? 1 : vrs2.Count();
                        ds.dsSuKienSang = vrs1;
                        ds.dsSuKienChieu = vrs2;
                        abc.Add(ds);
                        
                    }
                
                
                }

                i++;
            }
            return Ok(abc);
        }
        [Route("getSuKienByID")] //Lich chi tiet 
        [HttpPost]
        public IHttpActionResult GetSuKienByID(CommonModel para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    using (var multi = db.QueryMultiple("Cal_GetSuKienByID", new { SuKienId = para.valint1.checkIsNumber() }, null, null, System.Data.CommandType.StoredProcedure))
                    {
                        Cal_SuKienNew item = new Cal_SuKienNew();
                        item.par = multi.Read<Cal_SuKienDto>().SingleOrDefault();
                        item.listfile = multi.Read<GCAL_File>().ToList();
                        item.listtainguyen = multi.Read<Cal_TaiNguyenDto>().ToList();
                        item.listthongbao = multi.Read<Cal_ThongBaoDto>().ToList();
                        item.listnote = multi.Read<Cal_SuKienNoteDto>().ToList();
                        return Ok(item);
                    }

                }
            }
            catch
            {
                return BadRequest();
            }
        }
                
        [Route("getSuKienLog")]
        [HttpPost]
        public IHttpActionResult GetSuKienLog(CommonModel para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    using (var multi = db.QueryMultiple("Cal_GetSuKienLog", new { SuKienId = para.valint1.checkIsNumber() }, null, null, System.Data.CommandType.StoredProcedure))
                    {
                        Cal_SuKienLog item = new Cal_SuKienLog();
                        item.par = multi.Read<Cal_SuKienDto>().SingleOrDefault();
                        item.listlog = multi.Read<Cal_SuKienLogDto>().ToList();
                        return Ok(item);
                    }

                }
            }
            catch
            {
                return BadRequest();
            }
        }
        [Route("getSuKienLogSMS")]
        [HttpPost]
        public IHttpActionResult GetSuKienLogSMS(CommonModel para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    using (var multi = db.QueryMultiple("Cal_GetSuKienLogSMS", new { SuKienId = para.valint1.checkIsNumber() }, null, null, System.Data.CommandType.StoredProcedure))
                    {
                        Cal_SuKienLogSMS item = new Cal_SuKienLogSMS();
                        item.par1 = multi.Read<TinNhanHenGio>().SingleOrDefault();
                        item.par2 = multi.Read<Cal_SuKienSMSLogs>().SingleOrDefault();
                        return Ok(item);
                    }

                }
            }
            catch
            {
                return BadRequest();
            }
        }
        #region Load Data Lịch Cá Nhân
        [Route("getAllSuKien")] //Loc Lich ca nhan
        [HttpPost]
        public IHttpActionResult GetAllSuKien(CommonModel para)
        {
            try
            {
                int loc = para.valint2.checkIsNumber();
                int curPage = para.valint1.checkIsNumber();
                int nums = perPage * (curPage - 1);
                int nume = perPage;
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@username", User.Identity.Name);
                    parameters.Add("@loc", loc);
                    parameters.Add("@nums", nums);
                    parameters.Add("@nume", nume);
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var ds = db.Query<SuKienView>("Cal_GetAllSuKien", parameters, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                    dsLichDto result = new dsLichDto();
                    if (ds.Count > 0)
                    {
                        result.dsLich = ds;
                        result.Curpage = curPage;
                        result.Total = ds[0].Total;
                        result.Perpage = perPage;
                    }
                    else
                    {
                        result.dsLich = new List<SuKienView>();
                        result.Curpage = curPage;
                        result.Total = 0;
                        result.Perpage = perPage;
                    }

                    return Ok(result);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("getSuKienUserByName")] //Factory lay sukien chua xem
        [HttpPost]
        public IHttpActionResult GetSuKienUserByName(CommonModel para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@username", User.Identity.Name);
                    parameters.Add("@DaXem", para.valint1);
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    return Ok(db.ExecuteScalar<int>("Cal_GetSuKienUserByName", parameters, null,  null, System.Data.CommandType.StoredProcedure));
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("SearchAll")]
        public IHttpActionResult SearchAll(CommonSearch para)
        {
            using (IDbConnection db = new SqlConnection(_cnn))
            {
                if (db.State == System.Data.ConnectionState.Closed)
                    db.Open();
                var ds = db.Query<Cal_SuKienDto>("Cal_SearchAll", new { date = para.valtime1.checkDateTimeIsNull() }, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                //dsLichDto result = new dsLichDto();
                //if (ds.Count > 0)
                //{
                //    result.dsLich = ds;
                //    result.Curpage = curPage;
                //    result.Total = ds[0].Total;
                //    result.Perpage = perPage;
                //}
                //else
                //{
                //    result.dsLich = new List<Cal_SuKienDto>();
                //    result.Curpage = curPage;
                //    result.Total = 0;
                //    result.Perpage = perPage;
                //}
                return Ok(ds);
            }
        }

        [Route("getLichDuyet")]
        [HttpGet]
        public IHttpActionResult getLichDuyet()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    return Ok(db.ExecuteScalar<int>("Cal_GetLichDuyet", null, null, null, System.Data.CommandType.StoredProcedure));
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("getLichTuChoi")]
        public IHttpActionResult GetLichTuChoi()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@canbo", User.Identity.Name);
                    return Ok(db.ExecuteScalar<int>("Cal_GetLichTuChoi", parameters, null, null, System.Data.CommandType.StoredProcedure));
                }
            }
            catch
            {
                return BadRequest(); ;
            }
        }
        #endregion
        #endregion

        #region Get + Check tai nguyen

        [Route("getTaiNguyen")]
        [HttpPost]
        public IHttpActionResult GetTaiNguyen(CommonModel para)
        {
            try
            {
                //int KieuID = para.valint1;
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@KieuID", para.valint1.checkIsNumber());
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    return Ok(db.Query<Cal_TaiNguyenDto>("Cal_GetTaiNguyen", parameters, null, true, null, System.Data.CommandType.StoredProcedure));
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("checkTaiNguyen")]
        [HttpPost]
        public IHttpActionResult CheckTaiNguyen(CheckTaiNguyen para)
        {
            try
            {
                //int KieuID = para.valint1;
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@TaiNguyenID", para.valint1.checkIsNumber());
                    parameters.Add("@SuKienId", para.valint2.checkIsNumber());
                    parameters.Add("@BatDau", para.valstring1);
                    parameters.Add("@KetThuc", para.valstring2);

                    return Ok(db.Query<Cal_SuKienDto>("Cal_GetSuKienByTaiNguyen", parameters, null, true, null, System.Data.CommandType.StoredProcedure));
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        #endregion

        #region Them Edit SuKien Update Duyet
        [Route("ThemSuKien")]
        [HttpPost]
        public IHttpActionResult ThemSuaSuKien(Cal_SuKienNew para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("@SuKienId", para.par.SuKienId);
                    parameters.Add("@NhomId", para.par.NhomId);
                    parameters.Add("@TieuDe", para.par.TieuDe);
                    parameters.Add("@NoiDung", para.par.NoiDung);
                    parameters.Add("@BatDau", para.par.BatDau);
                    parameters.Add("@KetThuc", para.par.KetThuc);
                    parameters.Add("@ChuanBi", para.par.ChuanBi);
                    parameters.Add("@QuanTrong", para.par.QuanTrong);
                    parameters.Add("@DuocDuyet", para.par.DuocDuyet);
                    parameters.Add("@CaNgay", para.par.CaNgay);
                    parameters.Add("@NguoiTao", User.Identity.Name);
                    parameters.Add("@NguoiToChuc", User.Identity.Name);
                    parameters.Add("@DiaDiem", para.par.DiaDiem);
                    parameters.Add("@LinkOnline", para.par.LinkOnline);
                    parameters.Add("@ChuTri", para.par.ChuTri);
                    parameters.Add("@KhachMoi", para.par.KhachMoi);
                    parameters.Add("@ThanhPhan", para.par.ThanhPhan);
                    parameters.Add("@NguoiThongBao", para.par.NguoiThongBao);
                    parameters.Add("@SMSThongBao", para.par.SMSThongBao);
                    parameters.Add("@PopupThongBao", para.par.PopupThongBao);
                    parameters.Add("@EmailThongBao", para.par.EmailThongBao);
                    parameters.Add("@SMSHenGio", para.par.SMSHenGio);
                    parameters.Add("@PopupHenGio", para.par.PopupHenGio);
                    parameters.Add("@EmailHenGio", para.par.EmailHenGio);
                    parameters.Add("@SuDungTaiNguyen", para.par.SuDungTaiNguyen);
                    parameters.Add("@SMSThoiGian", para.par.SMSThoiGian);
                    parameters.Add("@PopupThoiGian", para.par.PopupThoiGian);
                    parameters.Add("@EmailThoiGian", para.par.EmailThoiGian);
                    parameters.Add("@NguoiNhacHen", para.par.NguoiNhacHen);

                    if (para.par.DuocDuyet.checkIsNumber() == 2)
                    {
                        parameters.Add("@NguoiDuyetId", 1);
                    }
                    else
                    {
                        parameters.Add("@NguoiDuyetId", 0);
                    }
                    parameters.Add("@NguoiDuyet", User.Identity.Name);
                    parameters.Add("@CoTaiNguyenDuocDuyet", para.par.CoTaiNguyenDuocDuyet);
                    if (para.par.CoTaiNguyenDuocDuyet.checkBoolIsNull())
                        parameters.Add("@NguoiDuyetTaiNguyenId", 1);
                    else
                        parameters.Add("@NguoiDuyetTaiNguyenId", 0);
                    parameters.Add("@CoDungTaiNguyen", para.par.CoDungTaiNguyen);
                    parameters.Add("@NguoiDuyetTaiNguyen", User.Identity.Name);
                    parameters.Add("@EventId", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                    string noidungnhantin = para.par.NhomId == 23 ? SMSInfo(para.par) : SMSInfoDatxe(para.par);
                    string log;
                    if (db.Execute("Cal_AddSuKien", parameters, null, null, System.Data.CommandType.StoredProcedure) > 0)
                    {
                        int SuKienId = parameters.Get<int>("@EventId");
                        
                        if (para.par.DuocDuyet.checkIsNumber() == 1)
                        {
                            var listduyet = db.Query<string>("SMS_GetCalAdmin", null, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                            string list = string.Join(",", listduyet);
                            bool a = GetUserDt(1, list, "Yêu cầu duyệt lịch họp công ty: " + noidungnhantin,out log);
                            Hub.Clients.Groups(listduyet).gcalevent(1);
                        }
                        
                        if (!SYSTEM.IsNullorEmpty(para.par.NguoiThongBao))
                        {
                            var split = para.par.NguoiThongBao.Split(',').ToList();
                            var parlist = new List<DynamicParameters>();
                            for (var i = 0; i < split.Count; i++)
                            {
                                var parafile = new DynamicParameters();
                                parafile.Add("@SuKienId", SuKienId);
                                parafile.Add("@username", split[i]);
                                parlist.Add(parafile);
                            }
                            db.Execute("Cal_UpdateSuKienUser", parlist, null, null, System.Data.CommandType.StoredProcedure);
                            if (para.par.DuocDuyet.checkIsNumber() == 2)
                            {
                                if(para.par.SMSThoiGian.checkIsNumber() > 0)
                                {
                                    DateTime smshengio = (DateTime)para.par.BatDau;
                                    if (smshengio.AddMinutes(-(double)para.par.SMSThoiGian) > DateTime.Now)
                                    {
                                        var partime = new DynamicParameters();
                                        partime.Add("@SuKienId", SuKienId);
                                        partime.Add("@NgayGui", smshengio.AddMinutes(-(double)para.par.SMSThoiGian));
                                        partime.Add("@Nguoitao", User.Identity.Name);
                                        partime.Add("@Noidung", "Thông báo: " + noidungnhantin);
                                        partime.Add("@ThongBao", para.par.NguoiThongBao);
                                        db.Execute("SMS_Hengio", partime, null, null, System.Data.CommandType.StoredProcedure);
                                    }
                                    else if (!para.par.SMSThongBao.checkBoolIsNull())
                                    {
                                        if (GetUserDt(1, para.par.NguoiThongBao, "Thông báo : " + noidungnhantin, out log))
                                            SaveLogSMSNow(SuKienId, para.par.NguoiThongBao, log);
                                    }
                                }
                                if (para.par.SMSThongBao.checkBoolIsNull())
                                {
                                    if (GetUserDt(1, para.par.NguoiThongBao, "Thông báo : " + noidungnhantin, out log))
                                        SaveLogSMSNow(SuKienId, para.par.NguoiThongBao, log);
                                }
                                Hub.Clients.Groups(split).gcalevent(2);
                            }
                        }
                        if (para.listtainguyen != null)
                        {
                            string list = string.Join(",", para.listtainguyen.Select(x => x.TaiNguyenID));
                            var parafile = new DynamicParameters();
                            parafile.Add("@SuKienID", SuKienId);
                            parafile.Add("@TaiNguyenIDs", list);
                            db.Execute("Cal_UpdateSuKienTaiNguyens", parafile, null, null, System.Data.CommandType.StoredProcedure);
                        }

                        if (para.listfile != null)
                        {
                            Uploadfile(para.listfile, SuKienId);
                        }

                        return Ok(SuKienId);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            catch
            {
                return BadRequest();
            }

        }
        [Route("UpdateSuKien")]
        [HttpPost]
        public IHttpActionResult UpdateSuKien(Cal_SuKienNew para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@SuKienId", para.par.SuKienId);
                        parameters.Add("@NhomId", para.par.NhomId);
                        parameters.Add("@TieuDe", para.par.TieuDe);
                        parameters.Add("@NoiDung", para.par.NoiDung);
                        parameters.Add("@BatDau", para.par.BatDau);
                        parameters.Add("@KetThuc", para.par.KetThuc);
                        parameters.Add("@ChuanBi", para.par.ChuanBi);
                        parameters.Add("@QuanTrong", para.par.QuanTrong);
                        parameters.Add("@DuocDuyet", para.par.DuocDuyet);
                        parameters.Add("@CaNgay", para.par.CaNgay);
                        parameters.Add("@NguoiToChuc", User.Identity.Name);
                        parameters.Add("@DiaDiem", para.par.DiaDiem);
                        parameters.Add("@LinkOnline", para.par.LinkOnline);
                        parameters.Add("@ChuTri", para.par.ChuTri);
                        parameters.Add("@KhachMoi", para.par.KhachMoi);
                        parameters.Add("@ThanhPhan", para.par.ThanhPhan);
                        parameters.Add("@NguoiThongBao", para.par.NguoiThongBao);
                        parameters.Add("@SMSThongBao", para.par.SMSThongBao);
                        parameters.Add("@PopupThongBao", para.par.PopupThongBao);
                        parameters.Add("@EmailThongBao", para.par.EmailThongBao);
                        parameters.Add("@SMSHenGio", para.par.SMSHenGio);
                        parameters.Add("@PopupHenGio", para.par.PopupHenGio);
                        parameters.Add("@EmailHenGio", para.par.EmailHenGio);
                        parameters.Add("@SuDungTaiNguyen", para.par.SuDungTaiNguyen);
                        parameters.Add("@SMSThoiGian", para.par.SMSThoiGian);
                        parameters.Add("@PopupThoiGian", para.par.PopupThoiGian);
                        parameters.Add("@EmailThoiGian", para.par.EmailThoiGian);
                        parameters.Add("@NguoiNhacHen", para.par.NguoiNhacHen);

                        if (para.par.DuocDuyet.checkIsNumber() == 2)
                            parameters.Add("@NguoiDuyetId", 1);
                        else
                            parameters.Add("@NguoiDuyetId", 0);
                        parameters.Add("@NguoiDuyet", User.Identity.Name);
                        parameters.Add("@CoTaiNguyenDuocDuyet", para.par.CoTaiNguyenDuocDuyet);
                        if (para.par.CoTaiNguyenDuocDuyet.checkBoolIsNull())
                            parameters.Add("@NguoiDuyetTaiNguyenId", 1);
                        else
                            parameters.Add("@NguoiDuyetTaiNguyenId", 0);
                        parameters.Add("@CoDungTaiNguyen", para.par.CoDungTaiNguyen);
                        parameters.Add("@NguoiDuyetTaiNguyen", User.Identity.Name);


                        string noidungnhantin = para.par.NhomId == 23 ? SMSInfo(para.par) : SMSInfoDatxe(para.par);
                        string log;
                        if (db.Execute("Cal_UpdateSuKien", parameters, null, null, System.Data.CommandType.StoredProcedure) > 0)
                        {
                            if (para.par.DuocDuyet.checkIsNumber() == 1)
                            {
                                var listduyet = db.Query<string>("SMS_GetCalAdmin", null, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                                string list = string.Join(",", listduyet);
                                bool a = GetUserDt(1, list, "Yêu cầu duyệt lịch họp công ty: " + noidungnhantin,out log);
                                Hub.Clients.Groups(listduyet).gcalevent(1);
                                db.Execute("SMS_XoaHengio", new { SuKienId = para.par.SuKienId.checkIsNumber() }, null, null, System.Data.CommandType.StoredProcedure);
                            }
                            else if (para.par.DuocDuyet.checkIsNumber() == 0)
                            {
                                var listduyet = db.Query<string>("SMS_GetCalAdmin", null, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                                Hub.Clients.Groups(listduyet).gcalevent(1);
                                db.Execute("SMS_XoaHengio", new { SuKienId = para.par.SuKienId.checkIsNumber() }, null, null, System.Data.CommandType.StoredProcedure);
                            }

                            if (!SYSTEM.IsNullorEmpty(para.par.NguoiThongBao))
                            {
                                var split = para.par.NguoiThongBao.Split(',').ToList();
                                var parlist = new List<DynamicParameters>();
                                for (var i = 0; i < split.Count; i++)
                                {
                                    var parafile = new DynamicParameters();
                                    parafile.Add("@SuKienId", para.par.SuKienId);
                                    parafile.Add("@username", split[i]);
                                    parlist.Add(parafile);
                                }
                                db.Execute("Cal_UpdateSuKienUser", parlist, null, null, System.Data.CommandType.StoredProcedure);
                                if (para.par.DuocDuyet.checkIsNumber() == 2)
                                {
                                    db.Execute("SMS_XoaHengio", new { SuKienId = para.par.SuKienId.checkIsNumber() }, null, null, System.Data.CommandType.StoredProcedure);
                                    db.Execute("Cal_DeleteSuKienNote", new {SuKienId = para.par.SuKienId.checkIsNumber() }, null, null, System.Data.CommandType.StoredProcedure);
                                    if (para.par.SMSThoiGian.checkIsNumber() > 0)
                                    {
                                        DateTime smshengio = (DateTime)para.par.BatDau;
                                        if (smshengio.AddMinutes(-(double)para.par.SMSThoiGian) > DateTime.Now)
                                        {
                                            var partime = new DynamicParameters();
                                            partime.Add("@SuKienId", para.par.SuKienId);
                                            partime.Add("@NgayGui", smshengio.AddMinutes(-(double)para.par.SMSThoiGian));
                                            partime.Add("@Nguoitao", User.Identity.Name);
                                            partime.Add("@Noidung", "Thông báo: " + noidungnhantin);
                                            partime.Add("@ThongBao", para.par.NguoiThongBao);
                                            db.Execute("SMS_Hengio", partime, null, null, System.Data.CommandType.StoredProcedure);
                                        }
                                        else if (!para.par.SMSThongBao.checkBoolIsNull())
                                        {
                                            if (GetUserDt(1, para.par.NguoiThongBao, "Thông báo : " + noidungnhantin, out log))
                                                SaveLogSMSNow(para.par.SuKienId, para.par.NguoiThongBao, log);
                                        }
                                    }
                                    if (para.par.SMSThongBao.checkBoolIsNull())
                                    {
                                        if (GetUserDt(1, para.par.NguoiThongBao, "Thông báo : " + noidungnhantin, out log))
                                            SaveLogSMSNow(para.par.SuKienId, para.par.NguoiThongBao, log);
                                    }
                                    Hub.Clients.Groups(split).gcalevent(2);
                                }
                                
                            }
                            if (para.listtainguyen != null)
                            {
                                string list = string.Join(",", para.listtainguyen.Select(x => x.TaiNguyenID));
                                var parafile = new DynamicParameters();
                                parafile.Add("@SuKienID", para.par.SuKienId);
                                parafile.Add("@TaiNguyenIDs", list);
                                db.Execute("Cal_UpdateSuKienTaiNguyens", parafile, null, null, System.Data.CommandType.StoredProcedure);
                            }
                            else
                            {
                                db.Execute("Cal_DeleteSuKienTaiNguyens", new { SuKienID = para.par.SuKienId.checkIsNumber() }, null, null, System.Data.CommandType.StoredProcedure);
                            }
                            return Ok();
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
                return BadRequest();
            }
        }
        [Route("EditSuKien")]
        [HttpPost]
        public IHttpActionResult EditSuKien(Cal_SuKienNew para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@SuKienId", para.par.SuKienId);
                        parameters.Add("@NhomId", para.par.NhomId);
                        parameters.Add("@TieuDe", para.par.TieuDe);
                        parameters.Add("@NoiDung", para.par.NoiDung);
                        parameters.Add("@BatDau", para.par.BatDau);
                        parameters.Add("@KetThuc", para.par.KetThuc);
                        parameters.Add("@ChuanBi", para.par.ChuanBi);
                        parameters.Add("@QuanTrong", para.par.QuanTrong);
                        parameters.Add("@DuocDuyet", para.par.DuocDuyet);
                        parameters.Add("@CaNgay", para.par.CaNgay);
                        parameters.Add("@NguoiToChuc", User.Identity.Name);
                        parameters.Add("@DiaDiem", para.par.DiaDiem);
                        parameters.Add("@LinkOnline", para.par.LinkOnline);
                        parameters.Add("@ChuTri", para.par.ChuTri);
                        parameters.Add("@KhachMoi", para.par.KhachMoi);
                        parameters.Add("@ThanhPhan", para.par.ThanhPhan);
                        parameters.Add("@NguoiThongBao", para.par.NguoiThongBao);
                        parameters.Add("@SMSThongBao", para.par.SMSThongBao);
                        parameters.Add("@PopupThongBao", para.par.PopupThongBao);
                        parameters.Add("@EmailThongBao", para.par.EmailThongBao);
                        parameters.Add("@SMSHenGio", para.par.SMSHenGio);
                        parameters.Add("@PopupHenGio", para.par.PopupHenGio);
                        parameters.Add("@EmailHenGio", para.par.EmailHenGio);
                        parameters.Add("@SuDungTaiNguyen", para.par.SuDungTaiNguyen);
                        parameters.Add("@SMSThoiGian", para.par.SMSThoiGian);
                        parameters.Add("@PopupThoiGian", para.par.PopupThoiGian);
                        parameters.Add("@EmailThoiGian", para.par.EmailThoiGian);
                        parameters.Add("@NguoiNhacHen", para.par.NguoiNhacHen);
                        if (para.par.DuocDuyet.checkIsNumber() == 2)
                            parameters.Add("@NguoiDuyetId", 1);
                        else
                            parameters.Add("@NguoiDuyetId", 0);
                        parameters.Add("@NguoiCapNhat", User.Identity.Name);
                        parameters.Add("@CoTaiNguyenDuocDuyet", para.par.CoTaiNguyenDuocDuyet);
                        if (para.par.CoTaiNguyenDuocDuyet.checkBoolIsNull())
                            parameters.Add("@NguoiDuyetTaiNguyenId", 1);
                        else
                            parameters.Add("@NguoiDuyetTaiNguyenId", 0);
                        parameters.Add("@CoDungTaiNguyen", para.par.CoDungTaiNguyen);
                        parameters.Add("@NguoiDuyetTaiNguyen", User.Identity.Name);


                        string noidungnhantin = para.par.NhomId == 23 ? SMSInfo(para.par) : SMSInfoDatxe(para.par);
                        string log;
                        if (db.Execute("Cal_EditSuKien", parameters, null, null, System.Data.CommandType.StoredProcedure) > 0)
                        {
                            if (para.par.DuocDuyet.checkIsNumber() == 1)
                            {
                                var listduyet = db.Query<string>("SMS_GetCalAdmin", null, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                                string list = string.Join(",", listduyet);
                                bool a = GetUserDt(1, list, "Yêu cầu duyệt lịch họp công ty: " + noidungnhantin, out log);
                                Hub.Clients.Groups(listduyet).gcalevent(1);
                                db.Execute("SMS_XoaHengio", new { SuKienId = para.par.SuKienId.checkIsNumber() }, null, null, System.Data.CommandType.StoredProcedure);
                            }
                            else if (para.par.DuocDuyet.checkIsNumber() == 0)
                            {
                                var listduyet = db.Query<string>("SMS_GetCalAdmin", null, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                                Hub.Clients.Groups(listduyet).gcalevent(1);
                                db.Execute("SMS_XoaHengio", new { SuKienId = para.par.SuKienId.checkIsNumber() }, null, null, System.Data.CommandType.StoredProcedure);
                            }

                            if (!SYSTEM.IsNullorEmpty(para.par.NguoiThongBao))
                            {
                                var split = para.par.NguoiThongBao.Split(',').ToList();
                                var parlist = new List<DynamicParameters>();
                                for (var i = 0; i < split.Count; i++)
                                {
                                    var parafile = new DynamicParameters();
                                    parafile.Add("@SuKienId", para.par.SuKienId);
                                    parafile.Add("@username", split[i]);
                                    parlist.Add(parafile);
                                }
                                db.Execute("Cal_UpdateSuKienUser", parlist, null, null, System.Data.CommandType.StoredProcedure);
                                if (para.par.DuocDuyet.checkIsNumber() == 2)
                                {
                                    db.Execute("SMS_XoaHengio", new { SuKienId = para.par.SuKienId.checkIsNumber() }, null, null, System.Data.CommandType.StoredProcedure);
                                    db.Execute("Cal_DeleteSuKienNote", new { SuKienId = para.par.SuKienId.checkIsNumber() }, null, null, System.Data.CommandType.StoredProcedure);
                                    if (para.par.SMSThoiGian.checkIsNumber() > 0)
                                    {
                                        DateTime smshengio = (DateTime)para.par.BatDau;
                                        if (smshengio.AddMinutes(-(double)para.par.SMSThoiGian) > DateTime.Now)
                                        {
                                            var partime = new DynamicParameters();
                                            partime.Add("@SuKienId", para.par.SuKienId);
                                            partime.Add("@NgayGui", smshengio.AddMinutes(-(double)para.par.SMSThoiGian));
                                            partime.Add("@Nguoitao", User.Identity.Name);
                                            partime.Add("@Noidung", "Thông báo: " + noidungnhantin);
                                            partime.Add("@ThongBao", para.par.NguoiThongBao);
                                            db.Execute("SMS_Hengio", partime, null, null, System.Data.CommandType.StoredProcedure);
                                        }
                                        else if (!para.par.SMSThongBao.checkBoolIsNull())
                                        {
                                            if (GetUserDt(1, para.par.NguoiThongBao, "Thông báo : " + noidungnhantin, out log))
                                                SaveLogSMSNow(para.par.SuKienId, para.par.NguoiThongBao, log);
                                        }
                                    }
                                    if (para.par.SMSThongBao.checkBoolIsNull())
                                    {
                                        if (GetUserDt(1, para.par.NguoiThongBao, "Thông báo : " + noidungnhantin, out log))
                                            SaveLogSMSNow(para.par.SuKienId, para.par.NguoiThongBao, log);
                                    }
                                    Hub.Clients.Groups(split).gcalevent(2);
                                }

                            }
                            if (para.listtainguyen != null)
                            {
                                string list = string.Join(",", para.listtainguyen.Select(x => x.TaiNguyenID));
                                var parafile = new DynamicParameters();
                                parafile.Add("@SuKienID", para.par.SuKienId);
                                parafile.Add("@TaiNguyenIDs", list);
                                db.Execute("Cal_UpdateSuKienTaiNguyens", parafile, null, null, System.Data.CommandType.StoredProcedure);
                            }
                            else
                            {
                                db.Execute("Cal_DeleteSuKienTaiNguyens", new { SuKienID = para.par.SuKienId.checkIsNumber() }, null, null, System.Data.CommandType.StoredProcedure);
                            }
                            return Ok();
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
                return BadRequest();
            }
        }
        [Route("TrangThaiSuKien")]
        [HttpPost]
        public IHttpActionResult TrangThaiSuKien(CommonModel para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@SuKienId", para.valint1);
                        parameters.Add("@DuocDuyet", para.valint2);

                        if (db.Execute("Cal_TrangThaiSuKien", parameters, null, null, System.Data.CommandType.StoredProcedure) == 1)
                        {
                            return Ok();
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                }
            }
            catch
            {
                return BadRequest();
            }
        }
        
        [Route("BoDuyetSuKien")]
        [HttpPost]
        public IHttpActionResult BoDuyetSuKien(Cal_SuKienNoteDto para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    {
                        using (var transaction = db.BeginTransaction())
                        {
                            try
                            {
                                var parameters = new DynamicParameters();
                                parameters.Add("@SuKienId", para.SuKienId.checkIsNumber());
                                parameters.Add("@TieuDe", para.TieuDe.checkIsNull());
                                parameters.Add("@NoiDung", para.NoiDung.checkIsNull());
                                parameters.Add("@NguoiTao", para.NguoiTao.checkIsNull());
                                parameters.Add("@NguoiGui", User.Identity.Name);
                                db.Execute("Cal_AddSuKienNote", parameters, transaction, null, System.Data.CommandType.StoredProcedure);
                                var par2 = new DynamicParameters();
                                par2.Add("@SuKienId", para.SuKienId.checkIsNumber());
                                db.Execute("Cal_ThuHoiSuKien", par2, transaction, null, System.Data.CommandType.StoredProcedure);
                                transaction.Commit();
                                Hub.Clients.Group(para.NguoiTao).gcalevent(2);
                                return Ok();
                            }
                            catch
                            {
                                transaction.Rollback();
                                throw;

                            }
                        }

                    }
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("GoDuyetSuKien")]
        [HttpPost]
        public IHttpActionResult GoDuyetSuKien(Cal_AddSuKienNoteDto para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    {
                        using (var transaction = db.BeginTransaction())
                        {
                            try
                            {
                                var parameters = new DynamicParameters();
                                parameters.Add("@SuKienId", para.SuKienId.checkIsNumber());
                                parameters.Add("@TieuDe", para.TieuDe.checkIsNull());
                                parameters.Add("@NoiDung", para.NoiDung.checkIsNull());
                                parameters.Add("@NguoiTao", para.NguoiTao.checkIsNull());
                                parameters.Add("@NguoiGui", User.Identity.Name);
                                db.Execute("Cal_AddSuKienNote", parameters, transaction, null, System.Data.CommandType.StoredProcedure);
                                var par2 = new DynamicParameters();
                                par2.Add("@SuKienId", para.SuKienId.checkIsNumber());
                                db.Execute("Cal_ThuHoiSuKien", par2, transaction, null, System.Data.CommandType.StoredProcedure);
                                db.Execute("SMS_XoaHengio", par2, transaction, null, System.Data.CommandType.StoredProcedure);
                                transaction.Commit();
                                Hub.Clients.Group(para.NguoiTao).gcalevent(2);
                                string log;
                                if (para.SMSThongBao)
                                {
                                    bool a = GetUserDt(1, para.NguoiThongBao, "Hủy cuộc họp: " + para.TieuDeSMS + ". Lý do: " + para.NoiDung.checkIsNull(), out log);
                                }
                                return Ok();
                            }
                            catch
                            {
                                transaction.Rollback();
                                throw;

                            }
                        }

                    }
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("ThuHoiSuKien")]
        [HttpPost]
        public IHttpActionResult ThuHoiSuKien(CommonModel para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@SuKienId", para.valint1.checkIsNumber());
                        if (db.Execute("Cal_ThuHoiSuKien", parameters, null, null, System.Data.CommandType.StoredProcedure) > -1)
                        {
                            var listduyet = db.Query<string>("SMS_GetCalAdmin", null, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                            Hub.Clients.Groups(listduyet).gcalevent(1);
                            db.Execute("SMS_XoaHengio", new { SuKienId = para.valint1.checkIsNumber() }, null, null, System.Data.CommandType.StoredProcedure);
                            return Ok();
                        }
                        else
                        {
                            return Ok("Tạo sự kiện thất bại");
                        }
                    }
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("uploadfileCal")]
        public IHttpActionResult UploadfileCal(Cal_SuKienFileDto par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    
                    var para = new DynamicParameters();
                    para.Add("@LOAI", par.LOAI.checkIsNumber());
                    para.Add("@MOTA", par.MOTA.checkIsNull());
                    para.Add("@SuKienId", par.SuKienId.checkIsNumber());
                    para.Add("@LOAIFILE", par.LOAIFILE.checkIsNull());
                    para.Add("@SIZEFILE", par.SIZEFILE.checkIsNumber());
                    string tenfile = MoveFile(par.MOTA.checkIsNull(), par.LOAIFILE.checkIsNull());
                    para.Add("@TENFILE", tenfile);
                    para.Add("@returnId", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                    if (tenfile != "")
                    {
                        db.Execute("Cal_ThemFile1", para, null, null, System.Data.CommandType.StoredProcedure);
                        par.ID = para.Get<int>("@returnId");
                        par.TENFILE = tenfile;
                        return Ok(par);
                    }
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [Route("updateSuKienUserRead")]
        [HttpPost]
        public IHttpActionResult UpdateSuKienUserRead(CommonModel para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@SuKienId", para.valint1);
                    parameters.Add("@username", User.Identity.Name);
                    parameters.Add("@DaXem", para.valint2);
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    int i = db.Execute("Cal_UpdateSuKienUserRead", parameters, null, null, System.Data.CommandType.StoredProcedure);
                    if (i == 1)
                    {
                        Hub.Clients.Group(User.Identity.Name).gcalevent(3);
                    }
                    return Ok(i);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        
        #endregion

        #region Xoa
        [Route("XoaUserThongBao")]
        [HttpPost]
        public IHttpActionResult XoaUserThongBao(CommonModel para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SuKienId", para.valint1.checkIsNumber());
                    parameters.Add("@user", para.valstring1.checkIsNull());
                    db.Execute("Cal_XoaUserThongBao", parameters, null, null, System.Data.CommandType.StoredProcedure);
                    Hub.Clients.Group(para.valstring1.checkIsNull()).gcalevent(3);
                    //Signal
                    return Ok();
                }
            }
            catch
            {
                return BadRequest();
            }
        }
        [Route("XoaSuKien")]
        [HttpPost]
        public IHttpActionResult XoaSuKien(CommonModel para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SuKienId", para.valint1);
                    string qry1 = @"select * from [dbo].[Cal_SuKienFile] where SuKienId = @SuKienId";
                    string thongbao = @"select NguoiThongBao from [dbo].[Cal_SuKien] where SuKienId = @SuKienId";
                    
                    var abc = db.Query<ViewfilePDF>(qry1, parameters).ToList();
                    var listthongbao = db.Query<string>(thongbao, parameters).ToList();
                    if (db.Execute("Cal_XoaSuKien", parameters, null, null, System.Data.CommandType.StoredProcedure) == -1 )
                    {
                        Hub.Clients.Groups(listthongbao).gcalevent(3);
                        if (abc.Count > 0)
                        {
                            return Ok(DeleteFile(abc));
                        }
                        //Signal
                        return Ok(1);
                    }
                    
                    return Ok(1);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("xoaSuKienFile")]
        public IHttpActionResult XoaSuKienFile(CommonModel par)
        {
            if (par.valint1.checkIsNumber() > 0)
            {
                try
                {
                    using (IDbConnection db = new SqlConnection(_cnn))
                    {
                        if (db.State == System.Data.ConnectionState.Closed)
                            db.Open();

                        string qry1 = @"select * from [dbo].[Cal_SuKienFile] where ID = @ID";

                        var parameters = new DynamicParameters();
                        parameters.Add("@ID", par.valint1);
                        var abc = db.Query<ViewfilePDF>(qry1, parameters).ToList();
                        if (abc.Count > 0)
                        {
                            string qry = @"delete [dbo].[Cal_SuKienFile] where ID = @ID";
                            if (db.Execute(qry, parameters) > 0)
                            {
                                return Ok(DeleteFile(abc));
                            }
                        }
                        return BadRequest(); ;
                    }
                }
                catch
                {
                    return BadRequest();
                }
            }
            return BadRequest();
        }
        #endregion

        #region View file
        [HttpGet]
        [Route("getviewpdf")]
        public HttpResponseMessage Getviewpdf(string file)
        {
            string sPath = "";
            sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/");
            string user = User.Identity.Name;
            sPath = Path.Combine(sPath, user);
            sPath = Path.Combine(sPath, file);
            //sPath1 = System.Web.Hosting.HostingEnvironment.MapPath("~/DataFile/" + file);

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = File.Open(sPath, FileMode.Open, FileAccess.Read, FileShare.None); //new System.IO.FileStream(sPath, System.IO.FileMode.Open);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.Add("x-filename", file);
            return response;
        }
        public ViewfilePDF getPDFCal (int id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    string qry = @"select [ID], [TENFILE],[MOTA],[NGAYTAO] from [dbo].[Cal_SuKienFile] where ID = @ID";
                    return db.Query<ViewfilePDF>(qry, new { ID = id }).SingleOrDefault();
                }
            }
            catch
            {
                return null;
            }
        }
        [HttpGet]
        [Route("getviewpdfCal")]
        public HttpResponseMessage GetviewpdfCal(int id)
        {
            try
            {
                var dirs = getPDFCal(id);
                if (dirs != null)
                {
                    string sPath = "";
                    sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CalUpload/");
                    sPath = Path.Combine(sPath, dirs.NGAYTAO.ToString("yyyy"));
                    sPath = Path.Combine(sPath, dirs.NGAYTAO.ToString("MM"));
                    sPath = Path.Combine(sPath, dirs.TENFILE);

                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    var stream = File.Open(sPath, FileMode.Open, FileAccess.Read, FileShare.None); //new System.IO.FileStream(sPath, System.IO.FileMode.Open);
                    response.Content = new StreamContent(stream);
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    return response;
                }
                else
                    throw new Exception();
            }
            catch
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                return response;
            }
        }

        [HttpGet]
        [Route("getviewpdfCalNote")]
        public HttpResponseMessage GetviewpdfCalNote(int id)
        {
            using (IDbConnection db = new SqlConnection(_cnn))
            {
                if (db.State == System.Data.ConnectionState.Closed)
                    db.Open();

                string qry = @"select [ID], [TENFILE],[MOTA],[NGAYTAO] from [dbo].[Cal_SuKienFileNote] where ID = @ID";
                var dirs = db.Query<ViewfilePDF>(qry, new { ID = id }).SingleOrDefault();
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CalUpload/");
                sPath = Path.Combine(sPath, dirs.NGAYTAO.ToString("yyyy"));
                sPath = Path.Combine(sPath, dirs.NGAYTAO.ToString("MM"));
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
        [Route("DownloadFiles")]
        public HttpResponseMessage Download(int id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    string qry = @"select [ID], [TENFILE],[MOTA],[NGAYTAO] from [dbo].[Cal_SuKienFile] where ID = @ID";
                    var dirs = db.Query<ViewfilePDF>(qry, new { ID = id }).SingleOrDefault();
                    string sPath = "";
                    sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CalUpload/");
                    sPath = Path.Combine(sPath, dirs.NGAYTAO.ToString("yyyy"));
                    sPath = Path.Combine(sPath, dirs.NGAYTAO.ToString("MM"));
                    sPath = Path.Combine(sPath, dirs.TENFILE);

                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    var stream = File.Open(sPath, FileMode.Open, FileAccess.Read, FileShare.None); //new System.IO.FileStream(sPath, System.IO.FileMode.Open);
                    response.Content = new StreamContent(stream);
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.Add("x-filename", dirs.MOTA);
                    //httpResponseMessage.Content.Headers
                    return response;
                }
            }
            catch 
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        #endregion

        
    }
}