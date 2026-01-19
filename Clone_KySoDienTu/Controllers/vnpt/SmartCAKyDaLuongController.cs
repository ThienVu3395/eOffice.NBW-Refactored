using SmartCAAPI.Dtos.signpdf;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using GatewayServiceTest;
using Newtonsoft.Json;
using System.Threading.Tasks;
using VNPTdtos;
using VnptHashSignatures.Common;
using VnptHashSignatures.Interface;
using VnptHashSignatures.Pdf;
using static VModel.WFModel;
using Clone_KySoDienTu.Controllers.API.SmartCAFunction;

namespace Clone_KySoDienTu.Controllers.API.SmartCAKyDaLuong
{
    public class APIKyDaLuongController : Controller
    {
        private readonly string _cnn;

        private VCode.WFModule vc;

        private VCode.ReportModule vc2;

        private readonly SmartCAFunctionController _smartCAFunction = new SmartCAFunctionController();

        public APIKyDaLuongController()
        {
            _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
            vc = new VCode.WFModule(_cnn);
            vc2 = new VCode.ReportModule(_cnn);
        }

        #region Startup.cs
        public async void CallSmartCAAPI()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    string sql = @"select * from [SmartCA_SignTable] where Status > 0 and RefId is not null";
                    var result = db.Query<SignEntryDto>(sql).ToList();
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            SignPDFAll(item.Id, item.Module, item.RefId);
                        }
                    }
                }
            }
            catch
            {

            }
        }
        #endregion

        #region Xử lý ký đa luồng tuần tự
        public async Task<bool> SignPDFAll(string signPDFId, int Module, int RefId)
        {
            var signList = await _smartCAFunction.FindSignTable(signPDFId);
            var fileList = await _smartCAFunction.FindFiles(signList.Id);

            if (fileList == null || fileList.Count() == 0)
            {
                return false;
            }

            var signerList = await _smartCAFunction.FindSigner(signList.Id, signList.Status);

            if (signerList == null) // Sau khi tất cả User đã ký, xử lý lưu file + cập nhật trạng thái module + cập nhật trạng thái bảng SignTable ( hoàn tất quá trình ký )
            {
                int i = await _smartCAFunction.CallBackAPI(1, signList, fileList);
                if (i == 0) return false;

                await _smartCAFunction.UpdateStatusSignTable(signList.Id, -99);

                //vc2.CapNhatNguoiDuyetVB(RefId, null, signPDFId);
                return true;
            }

            //if (Module == 2)
            //{
            //    vc2.CapNhatNguoiDuyetVB(RefId, signerList.UserName, signPDFId);
            //}

            var signerListFlow = await _smartCAFunction.FindSignerFlow(RefId, signerList.UserName, Module, signList.Status);

            if (string.IsNullOrEmpty(signerList.TranID)) // Check User ký theo thứ tự
            {
                return await SignPDF(signerList, fileList, Module, RefId, signerListFlow.isChecked); // thực hiện gửi thông báo ký
            }

            else
            {
                int i = await Signed(signerList, fileList, Module, RefId, signerListFlow.isChecked); // Check User ký theo thứ tự và kiểm tra trạng thái User đã ký chưa

                if (i == 1) // ký thành công
                {
                    await _smartCAFunction.UpdateStatusSignTable(signList.Id, signList.Status + 1);
                    // Cập nhật lưu file đã ký + check tiếp User tiếp theo và gửi thông báo ký cho User tiếp theo
                    //if (Module == 0 || Module == 1)
                    //{
                    //    List<string> userThongBao = vc.LayUserThongBao(RefId, Module, signerList.UserName);
                    //    if (userThongBao.Count > 0)
                    //    {
                    //        Hub.Clients.Groups(userThongBao).countThongBaoVB(0);
                    //    }
                    //}
                    //else if (Module == 2)
                    //{
                    //    List<string> userThongBao = vc2.LayUserThongBao(RefId, Module, signList.Status + 1);
                    //    if (userThongBao.Count > 0)
                    //    {
                    //        Hub.Clients.Groups(userThongBao).countThongBaoReport(0);
                    //    }
                }
                else if (i == -1) // hủy ký
                {
                    await _smartCAFunction.UpdateStatusSignTable(signList.Id, -1); // Hủy quá trình ký + cập nhật trạng thái bảng SignTable về -1 ( hoàn tất quá trình ký )
                    await _smartCAFunction.UpdateSignersAndFiles(signPDFId); // Cập nhật TranId = null cho các User + Cập nhật SignData = null cho các file
                    await _smartCAFunction.CallBackAPI(4002, signList, fileList); // Cập nhật lưu file về lại file gốc
                    //if (Module == 0 || Module == 1)
                    //{
                    //    List<string> userThongBao = vc.LayUserThongBao(RefId, Module, signerList.UserName);
                    //    if (userThongBao.Count > 0)
                    //    {
                    //        Hub.Clients.Groups(userThongBao).countThongBaoVB(0);
                    //    }
                    //}
                    //else if (Module == 2)
                    //{
                    //    List<string> userThongBao = vc2.LayUserThongBao(RefId, Module, 0);
                    //    if (userThongBao.Count > 0)
                    //    {
                    //        Hub.Clients.Groups(userThongBao).countThongBaoReport(0);
                    //    }
                    //}
                }
                return true;
            }
        }

        public async Task<bool> SignPDF(SignerFrontEndViewDto signer, IEnumerable<FilePDFEntryDto> fileList, int Module, int RefId, int isChecked)
        {
            try
            {
                var logininfo = await _smartCAFunction.GetUserSmartCA(signer.UserName);
                var tranId = _sign(signer.Status, logininfo.AccessToken, "https://gwsca.vnpt.vn/csc/signature/sign", fileList, logininfo.Credential);
                await _smartCAFunction.UpdateTranIdSigner(signer.Id, tranId);
                vc.ChangeSignerFlowVB(RefId, Module, 1, null, "Đang chờ duyệt", signer.UserName, isChecked, signer.Status);
                //if (Module == 0 || Module == 1)
                //{
                //    List<string> userThongBao = vc.LayUserThongBao(RefId, Module, signer.UserName);
                //    if (userThongBao.Count > 0)
                //    {
                //        Hub.Clients.Groups(userThongBao).countThongBaoVB(0);
                //    }
                //}
                //else if (Module == 2)
                //{
                //    List<string> userThongBao = vc2.LayUserThongBao(RefId, Module, 0);
                //    if (userThongBao.Count > 0)
                //    {
                //        Hub.Clients.Groups(userThongBao).countThongBaoReport(0);
                //    }
                //}
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string _sign(int status, string accessToken, String serviceUri, IEnumerable<FilePDFEntryDto> data, string credentialId)
        {
            var req = new SignSmartCAReq
            {
                credentialId = credentialId,
                description = "VB Gửi Ký",
                datas = new List<DataSign>()
            };
            foreach (var item in data)
            {
                var test = new DataSign
                {
                    name = item.FileName,
                    dataBase64 = status == 1 ? item.UnsignData : item.SignedData
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

        public async Task<int> Signed(SignerFrontEndViewDto signer, IEnumerable<FilePDFEntryDto> fileList, int Module, int RefId, int isChecked)
        {
            try
            {
                var logininfo = await _smartCAFunction.GetUserSmartCA(signer.UserName);
                var tranInfo = _smartCAFunction._getTranInfo(logininfo.AccessToken, "https://gwsca.vnpt.vn/csc/credentials/gettraninfo", signer.TranID);
                switch (tranInfo.tranStatus)
                {
                    case 1: //Signed
                        await SignedSuccess(signer, fileList, tranInfo.documents);
                        vc.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã ký duyệt", signer.UserName, isChecked, signer.Status);
                        return 1;
                    case 4001: //Expired
                        if (isChecked < 2)
                        {
                            await _smartCAFunction.UpdateTranIdSigner(signer.Id, string.Empty);
                            vc.ChangeSignerFlowVB(RefId, Module, 1, null, "Đang chờ duyệt", signer.UserName, isChecked + 1, signer.Status);
                            return 0;
                        }
                        else
                        {
                            vc.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã quá hạn", signer.UserName, isChecked, signer.Status);
                            return -1;
                        }
                    case 4002: //Reject
                        vc.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã hủy ký", signer.UserName, isChecked, signer.Status);
                        return -1;
                    default:
                        return 0;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> SignedSuccess(SignerFrontEndViewDto signerDto, IEnumerable<FilePDFEntryDto> fileList, List<DocumentResp> datasigned)
        {
            try
            {
                var logininfo = await _smartCAFunction.GetUserSmartCA(signerDto.UserName);
                var filetolist = fileList.ToList();
                for (int i = 0; i < fileList.Count(); i++)
                {
                    var data = Convert.FromBase64String(signerDto.Status == 1 ? filetolist[i].UnsignData : filetolist[i].SignedData);

                    IHashSigner signer = HashSignerFactory.GenerateSigner(data, logininfo.CertBase64, null, HashSignerFactory.PDF);
                    //signer.SetHashAlgorithm(SignService.Common.HashSignature.Common.MessageDigestAlgorithm.SHA256);
                    ((PdfHashSigner)signer).SetHashAlgorithm(MessageDigestAlgorithm.SHA256);
                    if (signerDto.ObjPosition != null)
                    {
                        List<pdfInfo> pdf = JsonConvert.DeserializeObject<List<pdfInfo>>(signerDto.ObjPosition);
                        var temp = pdf.Where(x => x.FileName == filetolist[i].FileName);
                        if (temp.Count() > 0)
                        {
                            signer = await _smartCAFunction.AddImgToFilePDF(logininfo.ImgPath, signer, temp, "");
                        }
                    }
                    signer.GetSecondHashAsBase64();
                    byte[] signed = signer.Sign(datasigned[i].sig);
                    filetolist[i].SignedData = Convert.ToBase64String(signed);
                    await _smartCAFunction.UpdateSignedFile(filetolist[i].Id, Convert.ToBase64String(signed));
                }
                return 1;
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
