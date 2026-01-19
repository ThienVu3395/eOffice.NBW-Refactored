using Dapper;
using Clone_KySoDienTu.Service.Dtos;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Http;
using Clone_KySoDienTu.Controllers.Congviec;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using SteProject;

namespace Clone_KySoDienTu.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/other")]
    public class APIOtherController : ApiController
    {
        private readonly string _cnn;
        private VCode.OtherModule vc;
        public APIOtherController()
        {
            _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
            vc = new VCode.OtherModule(_cnn);
        }

        #region Private
        private string moveFile(string TenFile, string LoaiFile)
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

                        string savePath = System.Web.Hosting.HostingEnvironment.MapPath("~/FileOther/");
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

        private string moveFileHDSD(string FileName)
        {
            try
            {
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/" + User.Identity.Name + "/");
                DateTime ngaytao = DateTime.Now;
                if (Directory.Exists(sPath))
                {
                    if (File.Exists(Path.Combine(sPath, FileName)))
                    {
                        FileInfo fs = new FileInfo(Path.Combine(sPath, FileName));
                        string savePath = System.Web.Hosting.HostingEnvironment.MapPath("~/FileOther/");
                        if (!Directory.Exists(savePath))
                        {
                            Directory.CreateDirectory(savePath);
                        }
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
                        savePath = Path.Combine(savePath, FileName);

                        if (File.Exists(savePath))
                        {
                            FileInfo del = new FileInfo(savePath);
                            del.Delete();
                        }
                        fs.MoveTo(savePath);
                        return ngaytao.Year + "/" + ngaytao.ToString("MM");
                    }
                }

                return "";
            }
            catch
            {
                return "";
            }
        }

        public void uploadfile(List<Other_FileGopYPhanHoiDto> listfile, int gopyID)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    var parlist = new List<DynamicParameters>();
                    foreach (var par in listfile)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@LOAI", par.LOAI.checkIsNumber());
                        parameters.Add("@GOPYID", gopyID.checkIsNumber());
                        parameters.Add("@TENFILE", moveFile(par.MOTA.checkIsNull(), par.LOAIFILE.checkIsNull()));
                        parameters.Add("@MOTA", par.MOTA.checkIsNull());
                        parameters.Add("@TRANGTHAI", false);
                        parameters.Add("@LOAIFILE", par.LOAIFILE.checkIsNull());
                        parameters.Add("@SIZEFILE", par.SIZEFILE.checkIsNumber());
                        parlist.Add(parameters);
                    }
                    db.Execute("Other_InsertFileGopYPhanHoi", parlist, null, null, System.Data.CommandType.StoredProcedure);
                }
            }
            catch (Exception)
            {
            }

        }

        private int deleteFile(List<ViewfilePDF> filelist)
        {
            try
            {
                int iUploadedCnt = 0;
                string sPath = "";
                //sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/FileOther/");
                for (int i = 0; i < filelist.Count; i++)
                {
                    sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/FileOther/");
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
        #endregion
        
        private IEnumerable<T> getdsNVFlownode<T>(int workflowID, int vaitro)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", workflowID);
                    parameters.Add("@vaitro", vaitro);
                    var groups = db.Query<T>("TSK_getNhanVienFlow", parameters, null, true, null, System.Data.CommandType.StoredProcedure);
                    return groups;
                }
            }
            catch
            {
                return null;
            }
        }
        [HttpPost]
        [Route("GetAllGopYPhanHoi")]
        public IHttpActionResult GetAllGopYPhanHoi(CommonModel par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@canbo", User.Identity.Name);
                    parameters.Add("@loc", par.valint2.checkIsNumber());
                    parameters.Add("@nums", 5 * (par.valint1.checkIsNumber() - 1));
                    parameters.Add("@nume", 5);
                    parameters.Add("@rTotal", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                    var ds = db.Query<Other_GopYPhanHoiGet>("Other_GetAllGopYPhanHoi", parameters, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                    dsGopYPhanHoi result = new dsGopYPhanHoi();
                    if (ds.Count > 0)
                    {
                        result.Curpage = par.valint1.checkIsNumber();
                        result.Total = parameters.Get<int>("@rTotal"); ;
                        result.Perpage = 5;
                        for(int i = 0; i < ds.Count();i++)
                        {
                            ds[i].listfile = db.Query<Other_FileGopYPhanHoiDto>("Other_GetFileDinhKemGopY", new { GopYID = ds[i].ID }, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                            ds[i].phanhoi = db.Query<Other_GopYPhanHoiGet>("Other_GetChildGopYPhanHoi", new { ParentID = ds[i].ID}, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                            for (int ii = 0; ii < ds[i].phanhoi.Count(); ii++)
                            {
                                ds[i].phanhoi[ii].listfile = db.Query<Other_FileGopYPhanHoiDto>("Other_GetFileDinhKemGopY", new { GopYID = ds[i].phanhoi[ii].ID }, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                            }
                        }
                        result.dsGopY = ds;
                        
                    }
                    else
                    {
                        result.dsGopY = new List<Other_GopYPhanHoiGet>();
                        result.Curpage = par.valint1.checkIsNumber();
                        result.Total = 0;
                        result.Perpage = 5;
                    }

                    return Ok(result);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("InsertGopYPhanHoi")]
        public IHttpActionResult InsertGopYPhanHoi(Other_GopYPhanHoiNew par)
        {
            try { 
            using (IDbConnection db = new SqlConnection(_cnn))
            {
                if (db.State == System.Data.ConnectionState.Closed)
                    db.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@ParentID", par.ParentID.checkIsNumber());
                parameters.Add("@Loai", par.Loai.checkIsNull());
                parameters.Add("@NguoiTao", User.Identity.Name);
                parameters.Add("@TieuDe", par.TieuDe.checkIsNull());
                parameters.Add("@NoiDung", par.NoiDung.checkIsNull());
                parameters.Add("@TrangThai", par.TrangThai.checkIsNumber());
                parameters.Add("@CongKhai", par.CongKhai.checkBoolIsNull());
                parameters.Add("@returnID", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                int a = db.Execute("Other_InsertGopYPhanHoi", parameters, null, null, System.Data.CommandType.StoredProcedure);
                if(a > 0)
                {
                    if (par.ParentID.checkIsNumber() > 0)
                    {
                        if (par.TrangThai.checkIsNumber() == 2)
                        {
                             db.Execute("Other_UpdateTrangThaiGopYPhanHoi", new {ID = par.ParentID.checkIsNumber(), TrangThai = 2 }, null, null, System.Data.CommandType.StoredProcedure);
                        }
                        if (par.CongKhai.checkBoolIsNull())
                        {
                             db.Execute("Other_UpdateCongKhaiGopYPhanHoi", new { ID = par.ParentID.checkIsNumber(), CongKhai = par.CongKhai.checkBoolIsNull() }, null, null, System.Data.CommandType.StoredProcedure); 
                        }
                    }
                    if (par.listfile != null)
                    {
                        par.ID = parameters.Get<int>("@returnID");
                        uploadfile(par.listfile, par.ID);
                    }
                }
                return Ok();
            }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("UpdateTrangThaiGopYPhanHoi")]
        public IHttpActionResult UpdateTrangThaiGopYPhanHoi(CommonModel par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@ID", par.valint1.checkIsNumber());
                    parameters.Add("@TrangThai", par.valint2.checkIsNumber());
                    db.Execute("Other_UpdateTrangThaiGopYPhanHoi", parameters, null, null, System.Data.CommandType.StoredProcedure);
                    return Ok();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("UpdateCongKhaiGopYPhanHoi")]
        public IHttpActionResult UpdateCongKhaiGopYPhanHoi(CommonModel par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@ID", par.valint1.checkIsNumber());
                    parameters.Add("@CongKhai", par.valint2.checkIsNumber());
                    db.Execute("Other_UpdateCongKhaiGopYPhanHoi", parameters, null, null, System.Data.CommandType.StoredProcedure);
                    return Ok();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        #region Hướng dẫn sử dụng
        [HttpGet]
        [Route("GetModules")]
        public IHttpActionResult GetModules()
        {
            try
            {
                return Ok(vc.GetModules());
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("ThemBaiVietHDSD")]
        public IHttpActionResult ThemBaiVietHDSD(VModel.CMSModel.TinTucModel model)
        {
            try
            {
                int TinTucID = vc.ThemBaiVietHDSD(model,User.Identity.Name);
                if (TinTucID > 0)
                {
                    if (model.TapTinDinhKem != null)
                    {
                        foreach (var item in model.TapTinDinhKem)
                        {
                            vc.ThemFileDinhKemHDSD(TinTucID, item.Ten, moveFileHDSD(item.Ten));
                        }
                    }
                    return Ok(TinTucID);
                }
                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("GetDanhSachBaiVietHDSD")]
        public IHttpActionResult GetDanhSachBaiVietHDSD(VModel.WFModel.CommonModel model)
        {
            try
            {
                return Ok(vc.LayDanhSachHDSD(model));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("ChangeHienThiHDSD")]
        public IHttpActionResult ChangeHienThiHDSD(VModel.WFModel.CommonModel model)
        {
            try
            {
                return Ok(vc.ChangeHienThiHDSD(model,User.Identity.Name));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("CapNhatBaiVietHDSD")]
        public IHttpActionResult CapNhatBaiVietHDSD(VModel.CMSModel.TinTucModel model)
        {
            try
            {
                return Ok(vc.CapNhatBaiVietHDSD(model, User.Identity.Name));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("LayChiTietBaiVietHDSD")]
        public IHttpActionResult LayChiTietBaiVietHDSD(VModel.WFModel.CommonModel model)
        {
            try
            {
                return Ok(vc.LayChiTietBaiVietHDSD(model));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("XoaBaiVietHDSD")]
        public IHttpActionResult XoaBaiVietHDSD(VModel.WFModel.CommonModel model)
        {
            try
            {
                var listfile = vc.LayFileDinhKemHDSD(model);
                if (listfile.Count > 0)
                {
                    foreach (var item in listfile)
                    {
                        string imgPath = System.Web.Hosting.HostingEnvironment.MapPath("~/FileOther/" + item.Url + "/" + item.Ten);
                        if (File.Exists(imgPath))
                        {
                            FileInfo f = new FileInfo(imgPath);
                            f.Delete();
                        }
                    }
                }
                return Ok(vc.XoaBaiVietHDSD(model));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("CapNhatFileHDSD")]
        public IHttpActionResult CapNhatFileHDSD(VModel.WFModel.CommonModel model)
        {
            using (IDbConnection db = new SqlConnection(_cnn))
            {
                if (model.valint2 == 1) //xóa
                {
                    string imgPath = "";
                    imgPath = System.Web.Hosting.HostingEnvironment.MapPath("~/FileOther/" + model.valstring1);
                    if (File.Exists(imgPath))
                    {
                        FileInfo f = new FileInfo(imgPath);
                        f.Delete();
                    }
                    return Ok(vc.XoaFileDinhKemHDSD(model));
                }
                else //cap nhat
                {
                    string filename = moveFileHDSD(model.valstring1);
                    int themFile = vc.ThemFileDinhKemHDSD(model.valint3, model.valstring1, filename);
                    if (themFile > 0)
                    {
                        CommonModel tb = new CommonModel();
                        tb.valstring1 = filename;
                        tb.valint1 = themFile;
                        return Ok(tb);
                    }
                    else return BadRequest();
                }
            }
        }

        #endregion


        [HttpPost]
        [Route("LayDanhSachTruyCap")]
        public IHttpActionResult LayDanhSachTruyCap(VModel.WFModel.CommonModel model)
        {
            try
            {
                return Ok(vc.LayDanhSachTruyCap(model));
            }
            catch
            {
                return BadRequest();
            }
        }

        public ViewfilePDF getPDFOrder(int id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    string qry = @"select [ID], [TENFILE],[MOTA],[NGAYTAO] from [dbo].[Other_FileGopYPhanHoi] where ID = @ID";
                    return db.Query<ViewfilePDF>(qry, new { ID = id }).SingleOrDefault();
                }
            }
            catch { return null; }
        }

        #region File
        [HttpGet]
        [Route("viewpdfOther_GopY")]
        public HttpResponseMessage viewpdfOther_GopY(int id)
        {
            using (IDbConnection db = new SqlConnection(_cnn))
            {
                if (db.State == System.Data.ConnectionState.Closed)
                    db.Open();

                string qry = @"select [ID], [TENFILE],[MOTA],[NGAYTAO] from [dbo].[Other_FileGopYPhanHoi] where ID = @ID";
                var dirs = db.Query<ViewfilePDF>(qry, new { ID = id }).SingleOrDefault();
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/FileOther/");
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
        #endregion
    }
}