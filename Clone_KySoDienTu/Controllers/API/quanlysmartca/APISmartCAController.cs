using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using System;
using System.Linq;
using System.Collections.Generic;
using VModel;
using static VModel.WFModel;
using log4net;
using VNPTdtos;
using System.Web.Http.Cors;
using SmartCAAPI.Dtos.signpdf;
using SmartCAAPI.Dtos.usermanager;
using static VModel.LenhDieuXeModel;
using System.Threading.Tasks;
using Clone_KySoDienTu.Controllers.API;
using Clone_KySoDienTu.MyHub;
using Clone_KySoDienTu.Controllers.API.QuanLyNghiPhep;

namespace OAMS.Controllers.API
{
    [AllowAnonymous]
    [RoutePrefix("api/smartca")]
    public class APISmartCAController : EntitySetControllerWithHub<EventHub>
    {
        private readonly string _cnn;

        // Logger for this class
        private static readonly ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private VCode.ReportModule _ReportModule;

        private VCode.LenhDieuXeModule _LenhDieuXeModule;

        private QuanLyNghiPhepAPIController _QuanLyNghiPhepAPI;

        public APISmartCAController()
        {
            _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
            log4net.Config.XmlConfigurator.Configure();
            _ReportModule = new VCode.ReportModule(_cnn);
            _LenhDieuXeModule = new VCode.LenhDieuXeModule(_cnn);
            _QuanLyNghiPhepAPI = new QuanLyNghiPhepAPIController();
        }

        #region general functions
        public async Task<string> CapNhatVanBan(UpdateDocumentRequest updateDocumentRequest,string nguoiKy)
        {
            updateDocumentRequest.propertyName = PropertyName.SMART_CA_ID_STRING;
            ResponseInt rs = await _QuanLyNghiPhepAPI.CapNhatThongTinVanBan(updateDocumentRequest);
            updateDocumentRequest.nguoiCapNhat = rs.NguoiTao;
            updateDocumentRequest.noiDung = nguoiKy;
            updateDocumentRequest.propertyName = PropertyName.NGUOI_DUYET;
            await _QuanLyNghiPhepAPI.CapNhatThongTinVanBan(updateDocumentRequest);
            updateDocumentRequest.noiDung = "0";
            updateDocumentRequest.propertyName = PropertyName.TRANG_THAI;
            await _QuanLyNghiPhepAPI.CapNhatThongTinVanBan(updateDocumentRequest);
            return rs.NguoiTao;
        }

        public async Task<string> CapNhatVanBan_CallBackAPI(UpdateDocumentRequest updateDocumentRequest, int status, string id)
        {
            updateDocumentRequest.propertyName = PropertyName.SMART_CA_ID_STRING;
            if (status != 1) // hủy ký/hết hạn
            {
                updateDocumentRequest.noiDung = null;
            }
            else
            {
                updateDocumentRequest.noiDung = id;
            }
            ResponseInt rs = await _QuanLyNghiPhepAPI.CapNhatThongTinVanBan(updateDocumentRequest);
            updateDocumentRequest.nguoiCapNhat = rs.NguoiTao;
            updateDocumentRequest.noiDung = status == 1 ? "1" : "2";
            updateDocumentRequest.propertyName = PropertyName.TRANG_THAI;
            await _QuanLyNghiPhepAPI.CapNhatThongTinVanBan(updateDocumentRequest);
            return rs.NguoiTao;
        }
        #endregion

        #region CallBack APIs
        public void CapNhatTrangThaiDonNghiPhep(WFModel.CommonModel model)
        {
            string result = _LenhDieuXeModule.ChangeTrangThaiVB(model);
            if (result != null)
            {
                List<string> userThongBao = _LenhDieuXeModule.LayUserThongBao(model.valint1, model.valint3, model.valint4);
                if (userThongBao.Count > 0)
                {
                    userThongBao.Add(model.valstring2);
                    Hub.Clients.Groups(userThongBao).countThongBaoReport(0);
                }
            }
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("SaveFileSignDonNghiPhep")]
        public async Task<IHttpActionResult> SaveFileSignDonNghiPhep(CallBackDto item)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var para = new WFModel.CommonModel
                    {
                        valint1 = item.RefId,
                        valint3 = item.Module,
                        valint4 = 0,
                        valstring1 = item.ID
                    };
                    switch (item.Status)
                    {
                        case 1: //Signed
                            para.valint2 = 3;
                            break;

                        case 4002: //Reject or Expired
                            para.valint2 = 4;
                            break;
                    }
                    UpdateDocumentRequest updateDocumentRequest01 = new UpdateDocumentRequest();
                    updateDocumentRequest01.id = item.RefId;
                    updateDocumentRequest01.nguoiCapNhat = "null";
                    updateDocumentRequest01.accessToken = await _QuanLyNghiPhepAPI.GetAccessTokenDonNghiPhep();
                    updateDocumentRequest01.className = ClassName.DON_NGHI_PHEP;
                    string nguoitao = await CapNhatVanBan_CallBackAPI(updateDocumentRequest01, item.Status, item.ID);
                    para.valstring2 = nguoitao;
                    CapNhatTrangThaiDonNghiPhep(para);
                    return Ok(1);
                }
            }
            catch (Exception ex)
            {
                return Ok(ex.ToString());
            }
        }
        #endregion

        #region Drag & Drog UI APIs
        [HttpPost]
        [Route("create-sign")]
        public IHttpActionResult CreateSign(CreateAllDto model)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    string insertSignTable = @"set @reId = newID();
                                                INSERT INTO [dbo].[SmartCA_SignTable]
                                                ([ID] ,[Module] ,[Status] ,[RefId] ,[linkAPICallback], [NgayTao])
                                                VALUES (@reId, @Module, 0, @RefId, @linkAPICallback , @NgayTao);";
                    var parameters = new DynamicParameters();
                    parameters.Add("@Module", model.Item.Module);
                    parameters.Add("@RefId", model.Item.RefId);
                    parameters.Add("@linkAPICallback", model.Item.LinkAPICallback);
                    parameters.Add("@NgayTao", DateTime.Now);
                    parameters.Add("@reId", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 256);
                    var result = db.Execute(insertSignTable, parameters);
                    string newId = parameters.Get<string>("@reId");
                    if (result > 0)
                    {
                        foreach (var signerItem in model.ListSigner)
                        {
                            string insertSigner = @"INSERT INTO [SmartCA_Signer]
                                                    (ID, RefId, UserName, Status)
                                                    VALUES (newID(),@RefId, @UserName, @Status);";
                            var signer = new DynamicParameters();
                            signer.Add("@RefId", newId);
                            signer.Add("@UserName", signerItem.UserName.Trim());
                            signer.Add("@Status", signerItem.Status);
                            db.Execute(insertSigner, signer);
                        }
                        foreach (var fileItem in model.ListFile)
                        {
                            string insertFile = @"INSERT INTO [SmartCA_FilePdf]
                                                    (ID, RefId, FileName, FilePath, UnsignData, CreatedDate)
                                                    VALUES (newID(), @RefId, @FileName, @FilePath,@UnsignData , @NgayTao);";
                            var file = new DynamicParameters();
                            file.Add("@RefId", newId);
                            file.Add("@FileName", fileItem.FileName);
                            file.Add("@FilePath", fileItem.FilePath);
                            file.Add("@UnsignData", fileItem.UnsignData);
                            file.Add("@NgayTao", fileItem.CreatedDated);
                            db.Execute(insertFile, file);
                        }
                    }
                    return Ok(newId);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("get-sign")] // lấy ra danh sách tất cả người ký hoặc người ký theo thứ tự
        public IHttpActionResult GetSign(string smartcaID)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    string sql = @"declare @idvanban int,@module int,@status int

                                   select @idvanban = tb1.RefId , @module = tb1.Module, @status = tb1.Status
                                   from [SmartCA_SignTable] tb1
                                   where tb1.Id = @smartcaID; 
                                    
                                   select tb1.*,tb2.Loai as ChildModule
                                   from [SmartCA_SignTable] tb1
                                   left join vDonNghiPhep tb2 on tb1.RefId = tb2.Id and tb1.Module = 7
                                   where tb1.Id = @smartcaID; 

                                    select
                                      tb1.Id
                                    , tb1.[Status]
                                    , tb1.ObjPosition
                                    , tb1.UserName
                                    , tb2.ImgBase64
                                    , tb3.HOLOT + ' ' + tb3.TEN + (case when tb1.UserName = 'cnnb' and tb6.LoaiKy = 2 then N' (Cho số)' when tb1.UserName = 'cnnb' and tb6.LoaiKy = 0 then N' (Đóng mộc)' else '' end) as FullName
                                    , tb4.GroupName as BoPhan
                                    , tb5.Name as ChucVu
                                    , tb6.LoaiKy
                                    , tb6.ButPhe
                                    , tb6.NoiDungButPhe
                                    , tb6.SoVanBan
                                    , tb6.NgayVanBan
                                    from [SmartCA_Signer] tb1
                                    join [SmartCA.UserManager] tb2 on tb1.UserName = tb2.UserName
                                    join [users].[tbNguoidung] tb3 on tb1.UserName = tb3.USERNAME
                                    join [adm].[Core_Nhom] tb4 on tb4.GroupId = tb3.GroupId
                                    join [adm].[Core_JobTitles] tb5 on tb5.JobTitleID = tb3.JobTitleID
                                    join [dbo].[SmartCA_SignerFlow] tb6 on tb6.RefId = @idvanban and tb6.Module = @module and tb6.UserName = tb1.UserName and tb6.LOAIXULY = tb1.[Status]
                                    where tb1.RefID = @smartcaID and @status in (0,tb1.Status)
                                    order by [Status];

                                   select * from [SmartCA_FilePdf] where RefId = @smartcaID";
                    using (var multi = db.QueryMultiple(sql, new { @smartcaID = smartcaID }))
                    {
                        GetAllFrontEndViewDto result = new GetAllFrontEndViewDto();
                        result.Item = multi.Read<SignEntryDto>().SingleOrDefault();
                        result.ListSigner = multi.Read<GetSignResponse>().ToList();
                        result.ListFile = multi.Read<FilePDFEntryDto>().ToList();
                        return Ok(result);
                    }
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("update-signer")]
        public IHttpActionResult UpdateSigner(UpdateSigner para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    if (para.LoaiKy == 3) // dành cho đóng mộc cv đến có số và ngày
                    {
                        string sql2 = @"Update [SmartCA_SignerFlow] set FILEANH = @FileAnh where RefId = @RefId and Module = @Module and LoaiKy = 3";
                        var parameters2 = new DynamicParameters();
                        parameters2.Add("@FileAnh", para.FileAnh);
                        parameters2.Add("@RefId", para.RefId);
                        parameters2.Add("@Module", para.Module);
                        db.Execute(sql2, parameters2);
                    }
                    string sql = @"Update [SmartCA_Signer] set ObjPosition = @ObjPosition where Id = @Id";
                    var parameters = new DynamicParameters();
                    parameters.Add("@ObjPosition", para.Position);
                    parameters.Add("@Id", para.Id);
                    var result = db.Execute(sql, parameters);
                    return Ok(result);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("start-sign")]
        public IHttpActionResult StartSign(CommonModel para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    string sql = @"Update [SmartCA_SignTable] set Status = 1 where Id = @Id;
                                   Update [SmartCA_Signer] set TranID = NULL where RefId = @Id;
                                   Update [SmartCA_FilePdf] set SignedData = NULL where RefId = @Id;";
                    var result = db.Execute(sql, new { @Id = para.valstring1 });
                    return Ok(result);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("update-module-status")]
        public async Task<IHttpActionResult> UpdateModuleStatus(CommonModel para)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    string sql = @"select * from [SmartCA_SignTable] where Id = @Id";
                    var result = db.Query<SignEntryDto>(sql, new { @Id = para.valstring1 }).SingleOrDefault();
                    if (result != null)
                    {
                        string sql2 = @"select UserName from [SmartCA_Signer] where RefId = @Id and Status = @Status";
                        var nguoikydautien = db.Query<SignerEntryDto>(sql2, new { @Id = para.valstring1, @Status = 1 }).SingleOrDefault();
                        if (result.RefId > 0)
                        {
                            var updateModel = new WFModel.CommonModel
                            {
                                valint1 = result.RefId,
                                valint3 = result.Module,
                                valint4 = 1,
                                valstring1 = para.valstring1
                            };
                            var sendSmsRequest = new VModel.SmsLogRequest
                            {
                                RefId = result.RefId,
                                Module = result.Module
                            };
                            if (nguoikydautien != null)
                            {
                                List<string> nguoithongbao = new List<string>{ nguoikydautien.UserName };
                                ResponseInt rs = new ResponseInt();
                                switch (result.Module)
                                {
                                    case 7: // Đơn xin nghỉ phép
                                        updateModel.valint2 = 0;
                                        UpdateDocumentRequest updateDocumentRequest04 = new UpdateDocumentRequest();
                                        updateDocumentRequest04.id = result.RefId;
                                        updateDocumentRequest04.nguoiCapNhat = nguoikydautien.UserName;
                                        updateDocumentRequest04.noiDung = para.valstring1;
                                        updateDocumentRequest04.accessToken = await _QuanLyNghiPhepAPI.GetAccessTokenDonNghiPhep();
                                        updateDocumentRequest04.className = ClassName.DON_NGHI_PHEP;
                                        string ntdnp = await CapNhatVanBan(updateDocumentRequest04, nguoikydautien.UserName);
                                        updateModel.valstring2 = ntdnp;
                                        _ReportModule.ChangeSignerFlowVB(result.RefId, result.Module, 2, null, null, null, 0, 0, result.Status);
                                        CapNhatTrangThaiDonNghiPhep(updateModel);
                                        break;                           
                                    default:
                                        break;
                                }
                                return Ok(1);
                            }
                            else return BadRequest();
                        }

                        else return BadRequest();
                    }
                }
                return Ok(0);
            }
            catch
            {
                return Ok(0);
            }
        }
        #endregion
    }
}