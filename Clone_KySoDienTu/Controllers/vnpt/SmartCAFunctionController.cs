using Dapper;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System;
using GatewayServiceTest;
using log4net;
using VnptHashSignatures.Interface;
using VnptHashSignatures.Pdf;
using System.Net;
using VNPTdtos;
using System.Threading.Tasks;
using SmartCAAPI.Dtos.signpdf;
using static VModel.WFModel;
using System.Net.Http.Headers;
using System.Net.Http;
using System.IO;

namespace Clone_KySoDienTu.Controllers.API.SmartCAFunction
{
    public class SmartCAFunctionController : Controller
    {
        private readonly string _cnn;

        // Logger for this class
        private static readonly ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SmartCAFunctionController()
        {
            _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
            log4net.Config.XmlConfigurator.Configure();
        }

        #region VNPT
        public string _getCredentialSmartCA(String accessToken, String serviceUri)
        {
            var response = CoreServiceClient.Query(new ReqCredentialSmartCA(), serviceUri, accessToken);

            if (response != null)
            {
                CredentialSmartCAResponse credentials = JsonConvert.DeserializeObject<CredentialSmartCAResponse>(response);
                return credentials.content[0];
            }
            return "";
        }

        public String _getAccoutSmartCACert(String accessToken, String serviceUri, string credentialId)
        {
            var response = CoreServiceClient.Query(new ReqCertificateSmartCA
            {
                credentialId = credentialId,
                certificates = "chain",
                certInfo = true,
                authInfo = true
            }, serviceUri, accessToken);
            if (response != null)
            {
                CertificateSmartCAResponse req = JsonConvert.DeserializeObject<CertificateSmartCAResponse>(response);
                String certBase64 = req.cert.certificates[0];
                return certBase64.Replace("\r\n", "");
            }
            return "";
        }

        public TranInfoSmartCARespContent _getTranInfo(string accessToken, String serviceUri, String tranId)
        {
            var response = CoreServiceClient.Query(new ContenSignHash
            {
                tranId = tranId
            }, serviceUri, accessToken);

            if (response != null)
            {
                TranInfoSmartCAResp resp = JsonConvert.DeserializeObject<TranInfoSmartCAResp>(response);
                if (resp.code == 0)
                {
                    return resp.content;
                }
                else if (resp.code == 62001)
                {
                    resp.content.tranStatus = 4005;
                    return resp.content;
                }
            }
            return null;
        }

        public string _signHash(string accessToken, String serviceUri, List<FilePDFEntryDto> data, string credentialId)
        {
            var req = new SignHashSmartCAReq
            {
                credentialId = credentialId,
                //refTranId = "1234-5678-90000",
                //notifyUrl = "http://10.169.0.221/api/v1/smart_ca_callback",
                description = "VB Gửi Ký",
                datas = new List<DataSignHash>()
            };
            for (var i = 0; i < data.Count; i++)
            {
                var test = new DataSignHash
                {
                    name = data[i].FileName,
                    hash = data[i].HashData
                };
                req.datas.Add(test);
            }
            var response = CoreServiceClient.Query(req, serviceUri, accessToken);
            if (response != null)
            {
                SignHashSmartCAResponse resp = JsonConvert.DeserializeObject<SignHashSmartCAResponse>(response);
                if (resp.code == 0)
                {
                    return resp.content.tranId;
                }
            }
            return "";
        }

        public string _sign(string accessToken, String serviceUri, IEnumerable<FilePDFEntryDto> data, string credentialId) // return SignHashSmartCAResponse
        {
            var req = new SignSmartCAReq
            {
                credentialId = credentialId,
                description = "VB Gửi Ký",
                datas = new List<DataSign>()
            };
            foreach (FilePDFEntryDto item in data)
            {
                string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                sPath = Path.Combine(sPath, item.CreatedDate.ToString("yyyy"));
                sPath = Path.Combine(sPath, item.CreatedDate.ToString("MM"));
                sPath = Path.Combine(sPath, item.FilePath);
                var test = new DataSign
                {
                    name = item.FileName,
                    dataBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(sPath))
                };
                req.datas.Add(test);
            }
            var body = JsonConvert.SerializeObject(req);

            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    wc.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;
                    string HtmlResult = wc.UploadString(serviceUri, "POST", body);

                    SignHashSmartCAResponse resp = JsonConvert.DeserializeObject<SignHashSmartCAResponse>(HtmlResult);
                    if (resp.code == 0)
                    {
                        return resp.content.tranId; // return resp
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return null;
        }
        #endregion

        #region database
        public async Task<UserModel> GetUserSmartCA(string UserName)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    string sql = @"SELECT [UserSmartCA],[PassSmartCA],[ImgBase64],[ImgPath],[Khoa],[DaCoChuKySo] FROM [SmartCA.UserManager] where UserName = @UserName";
                    var userinfo = db.Query<UserInfo>(sql, new { @UserName = UserName }, null, true, null).SingleOrDefault();
                    string refresh_token = "";
                    var access_token = CoreServiceClient.GetAccessToken(userinfo.UserSmartCA, userinfo.PassSmartCA, out refresh_token);
                    if(access_token != null)
                    {
                        String credential = _getCredentialSmartCA(access_token, "https://gwsca.vnpt.vn/csc/credentials/list");
                        string certBase64 = _getAccoutSmartCACert(access_token, "https://gwsca.vnpt.vn/csc/credentials/info", credential);
                        var result = new UserModel()
                        {
                            ImgBase64 = userinfo.ImgBase64,
                            AccessToken = access_token,
                            CertBase64 = certBase64,
                            Credential = credential,
                            ImgPath = userinfo.ImgPath,
                            Khoa = userinfo.Khoa,
                            DaCoChuKySo = userinfo.DaCoChuKySo
                        };
                        return result;
                    }
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        // lấy thông tin bảng to nhất SmartCA_SignTable
        public async Task<SignEntryDto> FindSignTable(string Id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    string sql = @"select * from [SmartCA_SignTable] where Id = @Id";
                    var result = db.Query<SignEntryDto>(sql, new { @Id = Id }).SingleOrDefault();
                    return result;
                }
            }
            catch
            {
                return new SignEntryDto();
            }
        }

        // lấy thông tin file SmartCA_FilePdf
        public async Task<IEnumerable<FilePDFEntryDto>> FindFiles(string refID)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    string sql = @"select * from [SmartCA_FilePdf] where RefId = @Id";
                    var result = db.Query<FilePDFEntryDto>(sql, new { @Id = refID });
                    return result;
                }
            }
            catch
            {
                return new List<FilePDFEntryDto>();
            }
        }

        // lấy thông tin người tham gia ký SmartCA_Signer theo thứ tự
        public async Task<SignerFrontEndViewDto> FindSigner(string refID, int status)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    string sql = @"select * from [SmartCA_Signer] where RefId = @Id and Status = @Status";
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", refID);
                    parameters.Add("@Status", status);
                    var result = db.Query<SignerFrontEndViewDto>(sql, parameters).SingleOrDefault();
                    return result;
                }
            }
            catch
            {
                return new SignerFrontEndViewDto();
            }
        }

        // lấy danh sách người tham gia ký SmartCA_Signer
        public async Task<List<SignerFrontEndViewDto>> FindSigners(string refID)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    string sql = @"select * from [SmartCA_Signer] where RefId = @Id";
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", refID);
                    var result = db.Query<SignerFrontEndViewDto>(sql, parameters).ToList();
                    return result;
                }
            }
            catch
            {
                return new List<SignerFrontEndViewDto>();
            }
        }

        // lấy thông tin người tham gia ký SmartCA_SignerFlow
        public async Task<Core_UserDto> FindSignerFlow(int refID, string UserName, int Module, int Status)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    string sql = @"select * from [SmartCA_SignerFlow] where RefId = @Id and UserName = @UserName and Module = @Module and LOAIXULY = @Status";
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", refID);
                    parameters.Add("@UserName", UserName);
                    parameters.Add("@Module", Module);
                    parameters.Add("@Status", Status);
                    var result = db.Query<Core_UserDto>(sql, parameters).SingleOrDefault();
                    return result;
                }
            }
            catch
            {
                return new Core_UserDto();
            }
        }

        // Cập nhật trạng thái bảng to nhất SmartCA_SignTable
        public async Task<bool> UpdateStatusSignTable(string signId, int status)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    string sql = @"Update [SmartCA_SignTable] set Status = @Status where Id = @Id";
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", signId);
                    parameters.Add("@Status", status);
                    db.Execute(sql, parameters);
                }
                return true;
            }
            catch
            {
                throw;
            }
        }

        // Cập nhật tranID cho từng người theo ID tự tăng SmartCA_Signer
        public async Task<bool> UpdateTranIdSigner(string signerId, string tranID)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    string sql = @"Update [SmartCA_Signer] set TranId = @TranId where Id = @Id";
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", signerId);
                    parameters.Add("@TranId", tranID);
                    db.Execute(sql, parameters);
                }
                return true;
            }
            catch
            {
                throw;
            }
        }

        // Cập nhật tranID và SignedData = null cho bảng SmartCA_Signer và SmartCA_FilePdf
        // để xử lý trường hợp file có bị sai giữa chừng sẽ trả về và hủy hết tất cả chữ ký
        public async Task<bool> UpdateSignersAndFiles(string refID)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    string sql = @"Update [SmartCA_Signer] set TranID = NULL where RefId = @RefId;
                                   Update [SmartCA_FilePdf] set SignedData = NULL where RefId = @RefId;";
                    db.Execute(sql, new { @RefId = refID });
                }
                return true;
            }
            catch
            {
                throw;
            }
        }

        // Cập nhật SignedData = @SignedData hoặc HashData = @HashData cho bảng SmartCA_FilePdf sau khi đã có chữ ký theo tuần tự
        public async Task<bool> UpdateSignedFile(string FileId, string SignedData)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();

                    string sql = @"Update [SmartCA_FilePdf] set SignedData = @SignedData where Id = @Id";
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", FileId);
                    parameters.Add("@SignedData", SignedData);
                    db.Execute(sql, parameters);
                }
                return true;
            }
            catch
            {
                throw;
            }
        }

        // Thêm hình chữ ký vào file PDF với tọa độ đã đc kéo thả
        public async Task<IHashSigner> AddImgToFilePDF(string imgPath, IHashSigner signer, IEnumerable<pdfInfo> model, string username)
        {
            string _imgBytes = System.Web.Hosting.HostingEnvironment.MapPath("~/VNPT/SignImg/");
            _imgBytes += username + @"\";
            _imgBytes = Path.Combine(_imgBytes, imgPath);
            ((PdfHashSigner)signer).SetReason("Xác nhận tài liệu");
            ((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
            ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.LOGO_ONLY);
            ((PdfHashSigner)signer).SetLayer2Text("Ký bởi: Lê Hoàng Thiên Vũ\nChức vụ: Nhân viên P.CNTT\nNgày ký: 03/03/2023 14:59");
            ((PdfHashSigner)signer).SetFontSize(10);
            ((PdfHashSigner)signer).SetFontColor("000000");
            ((PdfHashSigner)signer).SetFontStyle(PdfHashSigner.FontStyle.Normal);
            ((PdfHashSigner)signer).SetFontName(PdfHashSigner.FontName.Times_New_Roman);
            foreach (var item in model)
            {
                ((PdfHashSigner)signer).AddSignatureView(new PdfSignatureView
                {
                    Rectangle = item.Rectangle,
                    Page = item.SignPage
                });
            }
            ((PdfHashSigner)signer).SetSignatureBorderType(PdfHashSigner.VisibleSigBorder.DASHED);
            return signer;
        }

        // Thực thi Callback APIs để cập nhật trạng thái của các bảng ký số
        public async Task<int> CallBackAPI(int status, SignEntryDto sign, IEnumerable<FilePDFEntryDto> list)
        {
            try
            {
                CallBackDto data = new CallBackDto();
                data.Module = sign.Module;
                data.RefId = sign.RefId;
                data.Status = status;
                data.ID = sign.Id;
                if (status == 1)
                {
                    List<CallBackFileDto> listFile = new List<CallBackFileDto>();
                    foreach (var item in list)
                    {
                        CallBackFileDto file = new CallBackFileDto();
                        file.FileName = item.FileName;
                        file.FilePath = item.FilePath;
                        file.UnsignData = item.UnsignData;
                        file.SignedData = item.SignedData;
                        file.HashData = item.HashData;
                        listFile.Add(file);
                    }
                    data.ListFile = listFile;
                }
                else
                {
                    data.ListFile = new List<CallBackFileDto>();
                }
                using (HttpClient client = new HttpClient())
                {
                    var dataAsString = JsonConvert.SerializeObject(data);
                    var content = new StringContent(dataAsString);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.BaseAddress = new Uri(sign.LinkAPICallback);
                    HttpResponseMessage response = await client.PostAsync(client.BaseAddress, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        if (result == "1")
                        {
                            return 1;
                        }
                        return 0;
                    }
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }
        #endregion
    }
}