using Dapper;
using Clone_KySoDienTu.Models;
using Clone_KySoDienTu.MyHub;
using Clone_KySoDienTu.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;
using SteCode;
using SteProject;

namespace Clone_KySoDienTu.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/tailieu")]
    //public class APICal_SuKienController : ApiController
    public class APITaiLieuController : ApiController//EntitySetControllerWithHub<EventHub>
    {
        private readonly string _cnn;
        private Permission abc;
        private SteDoc.SteDoc Doc;
        private int perPage = 10;
        public APITaiLieuController()
        {

            _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
            abc = new SteCode.Permission(_cnn);
            Doc = new SteDoc.SteDoc(_cnn);
        }

        #region private function
        private IEnumerable<Doc_Menu> CreateVM(int parentid, List<Doc_NhomTaiLieu> source)
        {
            return from men in source
                   where men.ParentId == parentid
                   select new Doc_Menu()
                   {
                       value = men,
                       childmenu = CreateVM(men.NhomId, source).ToList()
                   };
        }

        private string moveFile(string TenFile, string LoaiFile, int NhomId)
        {
            try
            {
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploadtemp/");
                string user = User.Identity.Name;
                sPath += user + @"\";
                if (Directory.Exists(sPath))
                {
                    if (File.Exists(Path.Combine(sPath, TenFile)))
                    {
                        FileInfo fs = new FileInfo(Path.Combine(sPath, TenFile));
                        string savePath = System.Web.Hosting.HostingEnvironment.MapPath("~/DOCData/");
                        if (Directory.Exists(savePath))
                        {
                            savePath = Path.Combine(savePath, NhomId.ToString());
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
        private string moveFileCongVan(string TenFile, int NhomId, DateTime? NgayTaoFileCongVan)
        {
            try
            {
                string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CongVanFile/");
                sPath = Path.Combine(sPath, NgayTaoFileCongVan?.Year.ToString());
                if (!Directory.Exists(sPath))
                {
                    Directory.CreateDirectory(sPath);
                }
                sPath = Path.Combine(sPath, NgayTaoFileCongVan?.ToString("MM"));
                if (!Directory.Exists(sPath))
                {
                    Directory.CreateDirectory(sPath);
                }
                if (Directory.Exists(sPath))
                {
                    if (File.Exists(Path.Combine(sPath, TenFile)))
                    {
                        string savePath = System.Web.Hosting.HostingEnvironment.MapPath("~/DOCData/");
                        if (Directory.Exists(savePath))
                        {
                            savePath = Path.Combine(savePath, NhomId.ToString());
                            if (!Directory.Exists(savePath))
                            {
                                Directory.CreateDirectory(savePath);
                            }
                            string newPath = Path.Combine(savePath, TenFile);
                            string oldPath = Path.Combine(sPath, TenFile);
                            if (File.Exists(oldPath))
                            {
                                File.Copy(oldPath, newPath);
                                return TenFile;
                            }
                            return null;
                        }
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private int deleteFile(List<Doc_ListFileDelete> filelist)
        {
            try
            {
                int iUploadedCnt = 0;
                string sPath = "";
                for (int i = 0; i < filelist.Count; i++)
                {
                    sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/DocData/");
                    sPath = Path.Combine(sPath, filelist[i].NhomId.ToString());
                    sPath = Path.Combine(sPath, filelist[i].TenFile);
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

        private void deleteFolder(int NhomId)
        {
            try
            {
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/DocData/");
                sPath = Path.Combine(sPath, NhomId.ToString());
                if (Directory.Exists(sPath))
                {
                    DirectoryInfo directory = new DirectoryInfo(sPath);
                    directory.Delete(true);
                }
            }
            catch
            {
            }
        }
        #endregion

        [Route("getMenu")]
        [HttpGet]
        public IHttpActionResult getMenu()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    var listall = db.Query<Doc_NhomTaiLieu>("Doc_GetNhomTaiLieu", new { UserName= User.Identity.Name }, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                    var a = CreateVM(-1, listall);
                    return Ok(a);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("getAllMenu")]
        [HttpGet]
        public IHttpActionResult getAllNhom()
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
                    var listall = db.Query<Doc_NhomTaiLieu>("Doc_GetNhomTaiLieu", parameters, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                    var a = CreateVM(-1, listall);
                    return Ok(a);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("GetListNhom")]
        [HttpGet]
        public IHttpActionResult GetListNhom()
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
                    var listall = db.Query<Doc_ListMenu>("Doc_GetListNhom", parameters, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                    return Ok(listall);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("getdsTaiLieu")]
        [HttpPost]
        public IHttpActionResult Doc_GetdsTaiLieu(CommonModel par)
        {
            try
            {
                int curPage = par.valint2.checkIsNumber();
                int nums = perPage * (curPage - 1);
                int nume = perPage;
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@NhomId", par.valint1.checkIsNumber());
                    parameters.Add("@loai", par.valint3.checkIsNumber());
                    parameters.Add("@nums", nums);
                    parameters.Add("@nume", nume);
                    parameters.Add("@username", User.Identity.Name);
                    var listall = db.Query<Doc_dsTaiLieu>("Doc_GetdsTaiLieu1", parameters, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                    return Ok(listall);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("SearchTaiLieus")]
        [HttpPost]
        public IHttpActionResult Doc_SearchTaiLieus(CommonModel par)
        {
            try
            {
                int curPage = par.valint2.checkIsNumber();
                int nums = perPage * (curPage - 1);
                int nume = perPage;
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SearchString", par.valstring1.checkIsNull());
                    parameters.Add("@loai", par.valint3.checkIsNumber());
                    parameters.Add("@nums", nums);
                    parameters.Add("@nume", nume);
                    parameters.Add("@username", User.Identity.Name);
                    var listall = db.Query<Doc_dsTaiLieu>("Doc_SearchTaiLieus", parameters, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                    return Ok(listall);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("getdsTaiLieuQuanLy")]
        [HttpPost]
        public IHttpActionResult Doc_GetdsTaiLieuQuanLy(CommonModel par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@Loai", par.valint1);
                    parameters.Add("@User",User.Identity.Name);
                    parameters.Add("@nums", par.valint2);
                    parameters.Add("@nume", par.valint3);
                    var listall = db.Query<Doc_dsTaiLieu>("Doc_GetdsTaiLieuQuanLy", parameters, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                    return Ok(listall);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("getTaiLieu")]
        [HttpPost]
        public IHttpActionResult getTaiLieu(CommonModel par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@TaiLieuId", par.valint1.checkIsNumber());
                    var listall = db.Query<Doc_TaiLieuDetail>("Doc_GetTaiLieu", parameters, null, true, null, System.Data.CommandType.StoredProcedure).SingleOrDefault();
                    return Ok(listall);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("getGroupTaiLieu")]
        [HttpPost]
        public IHttpActionResult getGroupTaiLieu(CommonModel par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@NhomId", par.valint1.checkIsNumber());
                    var listall = db.Query<Doc_NhomTaiLieu>("Doc_GetGroupTaiLieu", parameters, null, true, null, System.Data.CommandType.StoredProcedure).SingleOrDefault();
                    return Ok(listall);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("getTaiLieuVersion")]
        [HttpPost]
        public IHttpActionResult getTaiLieuVersion(CommonModel par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@VersionId", par.valint1.checkIsNumber());
                    var listall = db.Query<Doc_TaiLieuVersion>("Doc_GetTaiLieuVersion", parameters, null, true, null, System.Data.CommandType.StoredProcedure).SingleOrDefault();
                    return Ok(listall);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("getTaiLieuVersions")]
        [HttpPost]
        public IHttpActionResult getTaiLieuVersions(CommonModel par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@TaiLieuId", par.valint1.checkIsNumber());
                    var listall = db.Query<Doc_TaiLieuVersion>("Doc_GetTaiLieuVersions", parameters, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                    return Ok(listall);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("getDuongDanNhom")]
        [HttpPost]
        public IHttpActionResult getDuongDanNhom(CommonModel par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var listall = db.ExecuteScalar<string>("select dbo.Doc_Fun_GetDuongDanNhom (@NhomId) ", new { NhomId = par.valint1.checkIsNumber() });
                    return Ok(listall);
                }
            }
            catch
            {
                return BadRequest();
            }
        }


        [Route("getSubMenu")]
        [HttpPost]
        public IHttpActionResult getSubMenu(SteProject.CommonModel par)
        {
            try
            {
                if (User.IsInRole("Administrators"))
                    par.valstring1 = "Admin";
                else
                    par.valstring1 = User.Identity.Name;
                return Ok(Doc.GetSubMenu(par));
            }
            catch
            {
                return BadRequest();
            }
        }
        #region Them cap nhat duyet tai lieu
        [Route("ThemTaiLieu")]
        [HttpPost]
        public IHttpActionResult ThemTaiLieu(Doc_TaiLieuDetail par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    string tenfile = moveFile(par.TenFile,par.LoaiFile,par.NhomId);
                    var parameters = new DynamicParameters();
                    parameters.Add("@NhomId", par.NhomId);
                    parameters.Add("@TieuDe", par.TieuDe);
                    parameters.Add("@KyHieu", par.KyHieu);
                    parameters.Add("@GhiChu", par.GhiChu);
                    parameters.Add("@TenFile", tenfile);
                    parameters.Add("@MoTa", par.TenFile);
                    parameters.Add("@LoaiFile", par.LoaiFile);
                    parameters.Add("@FileSize", par.FileSize);
                    parameters.Add("@QuanTrong", par.QuanTrong);
                    parameters.Add("@Khoa", par.Khoa);
                    parameters.Add("@NguoiTao", User.Identity.Name);
                    parameters.Add("@TrangThai", par.TrangThai);
                    parameters.Add("@ThoiGianLuuTru", par.ThoiGianLuuTru);
                    parameters.Add("@NgayBanHanh", par.NgayBanHanh);
                    db.Execute("Doc_ThemTaiLieu", parameters, null, null, System.Data.CommandType.StoredProcedure);
                    return Ok("Thêm Tài Liệu Thành Công");
                }
            }
            catch
            {
                return BadRequest();
            }
        }
        [Route("ThemTaiLieuCongVan")]
        [HttpPost]
        public IHttpActionResult ThemTaiLieuCongVan(Doc_TaiLieuDetail par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    string checkMoveFile = moveFileCongVan(par.TenFile, par.NhomId, par.NgayTaoFileCongVan);
                    if (checkMoveFile != null)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@NhomId", par.NhomId);
                        parameters.Add("@TieuDe", par.TieuDe);
                        parameters.Add("@KyHieu", par.KyHieu);
                        parameters.Add("@GhiChu", par.GhiChu);
                        parameters.Add("@TenFile", par.TenFile);
                        parameters.Add("@MoTa", par.MoTa);
                        parameters.Add("@LoaiFile", par.LoaiFile);
                        parameters.Add("@FileSize", par.FileSize);
                        parameters.Add("@QuanTrong", par.QuanTrong);
                        parameters.Add("@Khoa", par.Khoa);
                        parameters.Add("@NguoiTao", User.Identity.Name);
                        parameters.Add("@TrangThai", par.TrangThai);
                        parameters.Add("@ThoiGianLuuTru", par.ThoiGianLuuTru);
                        parameters.Add("@NgayBanHanh", par.NgayBanHanh);
                        db.Execute("Doc_ThemTaiLieu", parameters, null, null, System.Data.CommandType.StoredProcedure);
                        return Ok("Thêm Tài Liệu Thành Công");
                    }
                    return BadRequest();
                }
            }
            catch
            {
                return BadRequest();
            }
        }
        [Route("SuaTaiLieu")]
        [HttpPost]
        public IHttpActionResult SuaTaiLieu(Doc_EditTaiLieu par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("@LoaiEdit", par.LoaiEdit);
                    parameters.Add("@TaiLieuId", par.TaiLieuId);
                    parameters.Add("@NhomId", par.NhomId);
                    parameters.Add("@TieuDe", par.TieuDe);
                    parameters.Add("@KyHieu", par.KyHieu);
                    parameters.Add("@GhiChu", par.GhiChu);
                    if (par.LoaiEdit == 1)
                    {
                        string tenfile = moveFile(par.TenFile, par.LoaiFile, par.NhomId);
                        parameters.Add("@TenFile", tenfile);
                        parameters.Add("@MoTa", par.TenFile);
                    }
                    else
                    {
                        parameters.Add("@TenFile", par.TenFile);
                        parameters.Add("@MoTa", par.MoTa);
                    }
                    parameters.Add("@LoaiFile", par.LoaiFile);
                    parameters.Add("@FileSize", par.FileSize);
                    parameters.Add("@QuanTrong", par.QuanTrong);
                    parameters.Add("@Khoa", par.Khoa);
                    parameters.Add("@NguoiCapNhat", User.Identity.Name);
                    parameters.Add("@TrangThai", par.TrangThai);
                    parameters.Add("@NgayBanHanh", par.NgayBanHanh);
                    parameters.Add("@ThoiGianLuuTru", par.ThoiGianLuuTru);
                    db.Execute("Doc_UpdateTaiLieu", parameters, null, null, System.Data.CommandType.StoredProcedure);
                    return Ok("");
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("ChangeTrangThai")]
        [HttpPost]
        public IHttpActionResult ChangeTrangThai(CommonModel par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@TaiLieuID", par.valint1);
                    parameters.Add("@TrangThai", par.valint2);
                    parameters.Add("@NguoiDuyet", User.Identity.Name);
                    db.Execute("Doc_ChangeTrangThai", parameters, null, null, System.Data.CommandType.StoredProcedure);
                    return Ok("Thành Công");
                }
            }
            catch
            {
                return BadRequest();
            }
        }
        #endregion

        #region Them + Cap nhat nhom + Delete Nhom
        [Route("ThemNhom")]
        [HttpPost]
        public IHttpActionResult ThemNhom(AddDoc_NhomTaiLieu par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("@ParentId", par.ParentId);
                    parameters.Add("@Ten", par.Ten);
                    parameters.Add("@MoTa", par.MoTa.checkIsNull());
                    parameters.Add("@YeuCauDuyet", par.YeuCauDuyet.checkIsNumber());
                    parameters.Add("@HienThi", par.HienThi.checkIsNumber());

                    int NhomId = db.ExecuteScalar<int>("Doc_ThemNhom", parameters, null, null, System.Data.CommandType.StoredProcedure);

                    if (NhomId > 0)
                    {
                        if (par.ParentId > -1)
                        {
                            par.Ten = "---" + par.Ten;
                        }
                        objPermissionDoc objper = new objPermissionDoc();
                        foreach (var i in objper.obj)
                        {
                            string permissionkey = "Doc" + '_' + i.PermissionAction + '_' + "G" + '_' + NhomId.ToString();
                            abc.CreatePermission(i.PermissionAction,i.PermissionName, permissionkey,"Doc","G", NhomId.ToString(), par.Ten);
                        }
                    }
                    if (par.ParentId > -1 && par.CopyPer)
                        abc.CopyPermission("Doc", "G", par.ParentId.ToString(), NhomId.ToString());
                    return Ok(NhomId);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("CapNhatNhom")]
        [HttpPost]
        public IHttpActionResult CapNhatNhom(UpdateDoc_NhomTaiLieu par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("@NhomId", par.NhomId.checkIsNumber());
                    parameters.Add("@ParentId", par.ParentId.checkIsNumber());
                    parameters.Add("@Ten", par.Ten);
                    parameters.Add("@MoTa", par.MoTa.checkIsNull());
                    parameters.Add("@YeuCauDuyet", par.YeuCauDuyet.checkIsNumber());
                    parameters.Add("@HienThi", par.HienThi.checkIsNumber());

                    int NhomId = db.Execute("Doc_UpdateNhom", parameters, null, null, System.Data.CommandType.StoredProcedure);

                    if (NhomId > 0)
                    {
                        if (par.TrangThai.checkIsNumber() > 0)
                        {
                            if (par.ParentId.checkIsNumber() > -1)
                                par.Ten = "---" + par.Ten;
                            else
                                par.Ten = par.Ten.Replace("---","");
                            abc.UpdatePermission("Doc", "G", par.NhomId.checkIsNumber().ToString(), par.Ten.checkIsNull());
                        }
                        //    if (par.ParentId > -1)
                        //    {
                        //        par.Ten = "---" + par.Ten;
                        //    }
                        //    foreach (var i in obj)
                        //    {
                        //        string permissionkey = "Doc" + '_' + i.PermissionAction + '_' + "G" + '_' + NhomId.ToString();
                        //        abc.CreatePermission(i.PermissionAction, i.PermissionName, permissionkey, "Doc", "G", NhomId.ToString(), par.Ten);
                        //    }
                    }

                    return Ok(NhomId);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("UpdateFolderSort")]
        [HttpPost]
        public IHttpActionResult UpdateFolderSort(SteDoc.Doc_U_FolderSort par)
        {
            try
            {
                bool i = Doc.UpdateFolderSort(par);
                if (i)
                    return Ok();
                else
                    throw new Exception();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [Route("CopyPhanQuyen")]
        [HttpPost]
        public IHttpActionResult CopyPhanQuyen(CommonModel par)
        {
            try
            {
                abc.CopyPermission("Doc", "G", par.valint1.ToString(), par.valint2.ToString());
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("DeleteNhom")]
        [HttpPost]
        public IHttpActionResult DeleteNhom(CommonModel par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@NhomId", par.valint1.checkIsNumber());
                    int NhomId = db.ExecuteScalar<int>("Doc_GetSubNhom", parameters, null, null, System.Data.CommandType.StoredProcedure);
                    if (NhomId > 0)
                    {
                        return Ok(new { tt= 0, thongbao = "Có tồn tại nhóm con" });
                    }
                    else
                    {
                        using (var multi = db.QueryMultiple("Doc_DeleteNhom", parameters, null, null, System.Data.CommandType.StoredProcedure))
                        {
                            var list1 = multi.Read<Doc_ListFileDelete>().ToList();
                            var listfile = multi.Read<int>().ToList();
                            if (list1.Count > 0 && listfile.Count > 0)
                            {
                                deleteFile(list1);
                            }
                            deleteFolder(par.valint1.checkIsNumber());
                            int listNhom = multi.ReadFirst<int>();
                            if (listNhom > 0)
                            {
                                abc.DeletePermission("Doc", "G", listNhom.ToString());
                            }
                            return Ok(new { tt = 1, thongbao = "Thành công"});
                        }
                    }

                }

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("DeleteTaiLieu")]
        [HttpPost]
        public IHttpActionResult DeleteTaiLieu(CommonModel par)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@TaiLieuId", par.valint1.checkIsNumber());

                    using (var multi = db.QueryMultiple("Doc_GetListDeleteTaiLieu", parameters, null, null, System.Data.CommandType.StoredProcedure))
                    {
                        var list1 = multi.Read<Doc_ListFileDelete>().ToList();
                        var list2 = multi.Read<Doc_ListFileDelete>().ToList();
                        if (list1.Count > 0)
                        {
                            deleteFile(list1);
                        }
                        if (list2.Count > 0)
                        {
                            deleteFile(list2);
                        }
                        int i = db.Execute("Doc_DeleteTaiLieu", parameters, null, null, System.Data.CommandType.StoredProcedure);
                        return Ok(new { tt = 1, thongbao = "Thành công" });
                    }
                }

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        #endregion

        #region Xem file

        public Doc_TaiLieuDetail getPDFTaiLieu (int id, int type)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    if (type == 6)
                        return db.Query<Doc_TaiLieuDetail>("Doc_GetTaiLieu", new { TaiLieuId = id }, null, true, null, System.Data.CommandType.StoredProcedure).SingleOrDefault();
                    else if (type == 7)
                        return db.Query<Doc_TaiLieuDetail>("Doc_GetTaiLieuVersion", new { VersionId = id }, null, true, null, System.Data.CommandType.StoredProcedure).SingleOrDefault();
                    else
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }
        [HttpGet]
        [Route("getviewpdf")]
        public HttpResponseMessage getviewpdf(int id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var dirs = db.Query<Doc_TaiLieuDetail>("Doc_GetTaiLieu", new { TaiLieuId = id }, null, true, null, System.Data.CommandType.StoredProcedure).SingleOrDefault();
                    string sPath = "";
                    sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/DOCData/");
                    sPath = Path.Combine(sPath, dirs.NhomId.ToString());
                    sPath = Path.Combine(sPath, dirs.TenFile);

                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    var stream = File.Open(sPath, FileMode.Open, FileAccess.Read, FileShare.None); //new System.IO.FileStream(sPath, System.IO.FileMode.Open);
                    response.Content = new StreamContent(stream);
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.Add("x-filename", dirs.MoTa);
                    return response;
                }
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        [Route("getviewpdfver")]
        public HttpResponseMessage getviewpdfver(int id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var dirs = db.Query<Doc_TaiLieuDetail>("Doc_GetTaiLieuVersion", new { VersionId = id }, null, true, null, System.Data.CommandType.StoredProcedure).SingleOrDefault();
                    string sPath = "";
                    sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/DOCData/");
                    sPath = Path.Combine(sPath, dirs.NhomId.ToString());
                    sPath = Path.Combine(sPath, dirs.TenFile);

                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    var stream = File.Open(sPath, FileMode.Open, FileAccess.Read, FileShare.None); //new System.IO.FileStream(sPath, System.IO.FileMode.Open);
                    response.Content = new StreamContent(stream);
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.Add("x-filename", dirs.MoTa);
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