using SmartCAAPI.Dtos.signpdf;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using VnptHashSignatures.Common;
using VnptHashSignatures.Interface;
using VnptHashSignatures.Pdf;
using Clone_KySoDienTu.Controllers.API.SmartCAFunction;
using System.IO;
using System.Threading;
using System;
using static VModel.WFModel;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Clone_KySoDienTu.Controllers.API.SmartCAKyDonLuong
{
    public class APIKyDonLuongController : Controller
    {
        private readonly string _cnn;

        private VCode.WFModule vc;

        private VCode.ReportModule vc2;

        private VCode.QuanLyChamCongModule qlcc;

        private readonly SmartCAFunctionController _smartCAFunction = new SmartCAFunctionController();

        public APIKyDonLuongController()
        {
            _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
            vc = new VCode.WFModule(_cnn);
            vc2 = new VCode.ReportModule(_cnn);
            qlcc = new VCode.QuanLyChamCongModule(_cnn);
        }

        public async Task<int> SignPDFBaoCao(SignerFrontEndViewDto signerEntryDto, IEnumerable<FilePDFEntryDto> fileList, int Module, int RefId, SignEntryDto SignTable, Core_UserDto signerFlow, string ButPhe)
        {
            UserModel userLogin = await _smartCAFunction.GetUserSmartCA(signerEntryDto.UserName);
            if (userLogin == null)
            {
                return -7;
            }
            var signerList = new List<IHashSigner>();
            var hashValueList = new List<FilePDFEntryDto>();
            var pdfOutputPaths = new List<String>();
            foreach (var item in fileList)
            {
                byte[] unsignData = null;
                string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                sPath = Path.Combine(sPath, item.CreatedDate.ToString("yyyy"));
                sPath = Path.Combine(sPath, item.CreatedDate.ToString("MM"));
                string savepath = Path.Combine(sPath, item.FilePath);
                pdfOutputPaths.Add(savepath);
                sPath = Path.Combine(sPath, item.FilePath);
                try
                {
                    unsignData = System.IO.File.ReadAllBytes(sPath);
                }
                catch
                {
                    return -1;
                }

                IHashSigner signer = HashSignerFactory.GenerateSigner(unsignData, userLogin.CertBase64, null, HashSignerFactory.PDF);
                ((PdfHashSigner)signer).SetHashAlgorithm(MessageDigestAlgorithm.SHA256);
                string ngayky = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + " (" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ")";
                #region Optional -----------------------------------
                if (signerEntryDto.ObjPosition != null)
                {
                    List<pdfInfo> pdf = JsonConvert.DeserializeObject<List<pdfInfo>>(signerEntryDto.ObjPosition);
                    var temp = pdf.Where(x => x.FileName == item.FileName);
                    if (temp.Count() > 0)
                    {
                        string _imgBytes = System.Web.Hosting.HostingEnvironment.MapPath("~/VNPT/SignImg/");
                        //string thongtinky = "Ký bởi: " + signerFlow.FullName + "\nChức vụ: " + signerFlow.CHUCVU + "\nNgày ký: " + ngayky;
                        string thongtinky = "";
                        _imgBytes += signerEntryDto.UserName + @"\";
                        _imgBytes = Path.Combine(_imgBytes, userLogin.ImgPath);
                        ((PdfHashSigner)signer).SetReason("Xác nhận tài liệu");
                        if (signerFlow.LoaiKy == 0)
                        {
                            //((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
                            //((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.LOGO_ONLY);
                            ((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
                            string noidungbutphe = "";
                            if (signerFlow.NoiDungButPhe != null)
                            {
                                ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_WITH_BACKGROUND);
                                string[] lines = signerFlow.NoiDungButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                                foreach (var text in lines)
                                {
                                    noidungbutphe += "\n";
                                    noidungbutphe += text;
                                }
                            }
                            else
                            {
                                ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.LOGO_ONLY);
                                //if (ButPhe != null)
                                //{
                                //    ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_WITH_BACKGROUND);
                                //    string[] lines = ButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                                //    foreach (var text in lines)
                                //    {
                                //        noidungbutphe += "\n";
                                //        noidungbutphe += text;
                                //    }
                                //}
                                //else
                                //{
                                //    ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.LOGO_ONLY);
                                //}
                            }
                            thongtinky = noidungbutphe == "" ? thongtinky : ("Bút phê chỉ đạo: " + noidungbutphe);
                        }
                        else if (signerFlow.LoaiKy == 1)
                        {
                            ((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
                            ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_WITH_BACKGROUND);
                            string noidungbutphe = "";
                            if (signerFlow.NoiDungButPhe != null)
                            {
                                string[] lines = signerFlow.NoiDungButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                                foreach (var text in lines)
                                {
                                    noidungbutphe += "\n";
                                    noidungbutphe += text;
                                }
                            }
                            else
                            {
                                if (ButPhe != null)
                                {
                                    string[] lines = ButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                                    foreach (var text in lines)
                                    {
                                        noidungbutphe += "\n";
                                        noidungbutphe += text;
                                    }
                                }
                            }
                            thongtinky = noidungbutphe == "" ? thongtinky : (thongtinky + "\nBút phê: " + noidungbutphe);
                        }
                        else if (signerFlow.LoaiKy == 2)
                        {
                            ((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
                            ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_ONLY);
                            thongtinky = signerFlow.SoVanBan;

                            //((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
                            //((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_ONLY);
                            //string noidungbutphe = "";
                            //if (signerFlow.NoiDungButPhe != null)
                            //{
                            //    string[] lines = signerFlow.NoiDungButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                            //    foreach (var text in lines)
                            //    {
                            //        noidungbutphe += "\n";
                            //        noidungbutphe += text;
                            //    }
                            //}
                            //else
                            //{
                            //    if (ButPhe != null)
                            //    {
                            //        string[] lines = ButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                            //        foreach (var text in lines)
                            //        {
                            //            noidungbutphe += "\n";
                            //            noidungbutphe += text;
                            //        }
                            //    }
                            //}
                            //thongtinky = noidungbutphe == "" ? thongtinky : (thongtinky + "\nBút phê: " + noidungbutphe);
                        }
                        else if (signerFlow.LoaiKy == 3)
                        {
                            ((PdfHashSigner)signer).SetCustomImage(Convert.FromBase64String(signerFlow.FILEANH.Split(',')[1]));
                            ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.LOGO_ONLY);
                        }
                        foreach (var objPosition in temp)
                        {
                            ((PdfHashSigner)signer).AddSignatureView(new PdfSignatureView
                            {
                                Rectangle = objPosition.Rectangle,
                                Page = objPosition.SignPage
                            });
                        }
                        ((PdfHashSigner)signer).SetLayer2Text(thongtinky);
                        ((PdfHashSigner)signer).SetFontSize(signerFlow.LoaiKy == 2 ? 13 : 10);
                        ((PdfHashSigner)signer).SetFontColor("0000ff");
                        ((PdfHashSigner)signer).SetFontStyle(PdfHashSigner.FontStyle.Normal);
                        ((PdfHashSigner)signer).SetFontName(PdfHashSigner.FontName.Times_New_Roman);
                        ((PdfHashSigner)signer).SetSignatureBorderType(PdfHashSigner.VisibleSigBorder.DASHED);
                    }
                }
                #endregion -----------------------------------------

                var test = new FilePDFEntryDto();
                test.Id = item.Id;
                test.FileName = item.FileName;
                test.HashData = signer.GetSecondHashAsBase64();
                hashValueList.Add(test);
                signerList.Add(signer);
            }

            var tranID = _smartCAFunction._signHash(userLogin.AccessToken, "https://gwsca.vnpt.vn/csc/signature/signhash", hashValueList, userLogin.Credential);

            if (tranID == "")
            {
                return -1;
            }

            await _smartCAFunction.UpdateTranIdSigner(signerEntryDto.Id, tranID);
            var count = 0;
            var isConfirm = false;
            var datasigneds = new List<String>();

            while (count < 24 && !isConfirm)
            {
                var tranInfo = _smartCAFunction._getTranInfo(userLogin.AccessToken, "https://gwsca.vnpt.vn/csc/credentials/gettraninfo", tranID);
                if (tranInfo != null)
                {
                    if (tranInfo.tranStatus == 4000) // WAITING_FOR_SIGNER_CONFIRM
                    {
                        count = count + 1;
                        Thread.Sleep(10000);
                    }
                    else if (tranInfo.tranStatus == 4001) // EXPIRED
                    {
                        await _smartCAFunction.UpdateTranIdSigner(signerEntryDto.Id, null);
                        if (signerFlow.isChecked < 2)
                        {
                            vc2.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã quá hạn", signerEntryDto.UserName, signerFlow.isChecked + 1, 0, SignTable.Status);
                        }
                        else
                        {
                            vc2.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã quá hạn", signerEntryDto.UserName, signerFlow.isChecked, 0, SignTable.Status);
                            await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, -1);
                            await _smartCAFunction.UpdateSignersAndFiles(SignTable.Id);
                            if (fileList.ToList().Count > 0)
                            {
                                foreach (var item in fileList)
                                {
                                    string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                                    sPath = Path.Combine(sPath, item.CreatedDate.ToString("yyyy"));
                                    sPath = Path.Combine(sPath, item.CreatedDate.ToString("MM"));
                                    sPath = Path.Combine(sPath, item.FilePath);
                                    System.IO.File.WriteAllBytes(sPath, Convert.FromBase64String(item.UnsignData));
                                }
                            }
                            await _smartCAFunction.CallBackAPI(4002, SignTable, fileList);
                        }
                        return 4001;
                    }

                    else if (tranInfo.tranStatus == 4002) // SIGNER_REJECTED
                    {
                        await _smartCAFunction.UpdateTranIdSigner(signerEntryDto.Id, null);
                        vc2.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã hủy ký", signerEntryDto.UserName, signerFlow.isChecked, 0, SignTable.Status);
                        await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, -1);
                        await _smartCAFunction.UpdateSignersAndFiles(SignTable.Id);
                        if (fileList.ToList().Count > 0)
                        {
                            foreach (var item in fileList)
                            {
                                string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                                sPath = Path.Combine(sPath, item.CreatedDate.ToString("yyyy"));
                                sPath = Path.Combine(sPath, item.CreatedDate.ToString("MM"));
                                sPath = Path.Combine(sPath, item.FilePath);
                                System.IO.File.WriteAllBytes(sPath, Convert.FromBase64String(item.UnsignData));
                            }
                        }
                        await _smartCAFunction.CallBackAPI(4002, SignTable, fileList);
                        return 4002;
                    }
                    else
                    {
                        isConfirm = true;
                        foreach (var document in tranInfo.documents)
                        {
                            datasigneds.Add(document.sig);
                        }
                    }
                }
                else
                {
                    return -2;
                }
            }

            if (!isConfirm)
            {
                return -3;
            }

            if (datasigneds.Count == 0)
            {
                return -4;
            }

            if (isConfirm) // SIGNED SUCCESS
            {
                for (int i = 0; i < signerList.Count; i++)
                {
                    var signer = signerList[i];
                    var datasigned = datasigneds[i];
                    var filesigned = hashValueList[i];
                    if (!signer.CheckHashSignature(datasigned))
                    {
                        return -5;
                    }
                    byte[] signed = signer.Sign(datasigned);
                    var outputPath = pdfOutputPaths[i];
                    System.IO.File.WriteAllBytes(outputPath, signed);
                    await _smartCAFunction.UpdateSignedFile(filesigned.Id, Convert.ToBase64String(signed));
                }
                vc2.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã ký duyệt", signerEntryDto.UserName, signerFlow.isChecked, 0, SignTable.Status);
                var nguoikytieptheo = await _smartCAFunction.FindSigner(SignTable.Id, SignTable.Status + 1);
                if (nguoikytieptheo != null)
                {
                    await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, SignTable.Status + 1);
                    vc2.CapNhatNguoiDuyetVB(RefId, nguoikytieptheo.UserName, SignTable.Id);
                    return 11;
                }
                else
                {
                    await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, -99);
                    vc2.CapNhatNguoiDuyetVB(RefId, null, SignTable.Id);
                    await _smartCAFunction.CallBackAPI(1, SignTable, fileList);
                    return 1;
                }
            }
            return -6;
        }

        public async Task<int> SignPDFQuanLyChamCong(SignerFrontEndViewDto signerEntryDto, IEnumerable<FilePDFEntryDto> fileList, int Module, int RefId, SignEntryDto SignTable, Core_UserDto signerFlow, string ButPhe)
        {
            UserModel userLogin = await _smartCAFunction.GetUserSmartCA(signerEntryDto.UserName);
            if (userLogin == null)
            {
                return -7;
            }
            var signerList = new List<IHashSigner>();
            var hashValueList = new List<FilePDFEntryDto>();
            var pdfOutputPaths = new List<String>();
            string pathUrl = GetUrlPathQuanLyChamCong(Module);
            foreach (var item in fileList)
            {
                byte[] unsignData = null;
                string sPath = System.Web.Hosting.HostingEnvironment.MapPath(pathUrl);
                //sPath = Path.Combine(sPath, item.CreatedDate.ToString("yyyy"));
                //sPath = Path.Combine(sPath, item.CreatedDate.ToString("MM"));
                string savepath = Path.Combine(sPath, item.FilePath);
                pdfOutputPaths.Add(savepath);
                sPath = Path.Combine(sPath, item.FilePath);
                try
                {
                    unsignData = System.IO.File.ReadAllBytes(sPath);
                }
                catch
                {
                    return -1;
                }

                IHashSigner signer = HashSignerFactory.GenerateSigner(unsignData, userLogin.CertBase64, null, HashSignerFactory.PDF);
                ((PdfHashSigner)signer).SetHashAlgorithm(MessageDigestAlgorithm.SHA256);
                string ngayky = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + " (" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ")";
                #region Optional -----------------------------------
                if (signerEntryDto.ObjPosition != null)
                {
                    List<pdfInfo> pdf = JsonConvert.DeserializeObject<List<pdfInfo>>(signerEntryDto.ObjPosition);
                    var temp = pdf.Where(x => x.FileName == item.FileName);
                    if (temp.Count() > 0)
                    {
                        string _imgBytes = System.Web.Hosting.HostingEnvironment.MapPath("~/VNPT/SignImg/");
                        //string thongtinky = "Ký bởi: " + signerFlow.FullName + "\nChức vụ: " + signerFlow.CHUCVU + "\nNgày ký: " + ngayky;
                        string thongtinky = "";
                        _imgBytes += signerEntryDto.UserName + @"\";
                        _imgBytes = Path.Combine(_imgBytes, userLogin.ImgPath);
                        ((PdfHashSigner)signer).SetReason("Xác nhận tài liệu");
                        if (signerFlow.LoaiKy == 0)
                        {
                            //((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
                            //((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.LOGO_ONLY);
                            ((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
                            string noidungbutphe = "";
                            if (signerFlow.NoiDungButPhe != null)
                            {
                                ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_WITH_BACKGROUND);
                                string[] lines = signerFlow.NoiDungButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                                foreach (var text in lines)
                                {
                                    noidungbutphe += "\n";
                                    noidungbutphe += text;
                                }
                            }
                            else
                            {
                                ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.LOGO_ONLY);
                                //if (ButPhe != null)
                                //{
                                //    ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_WITH_BACKGROUND);
                                //    string[] lines = ButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                                //    foreach (var text in lines)
                                //    {
                                //        noidungbutphe += "\n";
                                //        noidungbutphe += text;
                                //    }
                                //}
                                //else
                                //{
                                //    ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.LOGO_ONLY);
                                //}
                            }
                            thongtinky = noidungbutphe == "" ? thongtinky : ("Bút phê chỉ đạo: " + noidungbutphe);
                        }
                        else if (signerFlow.LoaiKy == 1)
                        {
                            ((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
                            ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_WITH_BACKGROUND);
                            string noidungbutphe = "";
                            if (signerFlow.NoiDungButPhe != null)
                            {
                                string[] lines = signerFlow.NoiDungButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                                foreach (var text in lines)
                                {
                                    noidungbutphe += "\n";
                                    noidungbutphe += text;
                                }
                            }
                            else
                            {
                                if (ButPhe != null)
                                {
                                    string[] lines = ButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                                    foreach (var text in lines)
                                    {
                                        noidungbutphe += "\n";
                                        noidungbutphe += text;
                                    }
                                }
                            }
                            thongtinky = noidungbutphe == "" ? thongtinky : (thongtinky + "\nBút phê: " + noidungbutphe);
                        }
                        else if (signerFlow.LoaiKy == 2)
                        {
                            ((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
                            ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_ONLY);
                            thongtinky = signerFlow.SoVanBan;

                            //((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
                            //((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_ONLY);
                            //string noidungbutphe = "";
                            //if (signerFlow.NoiDungButPhe != null)
                            //{
                            //    string[] lines = signerFlow.NoiDungButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                            //    foreach (var text in lines)
                            //    {
                            //        noidungbutphe += "\n";
                            //        noidungbutphe += text;
                            //    }
                            //}
                            //else
                            //{
                            //    if (ButPhe != null)
                            //    {
                            //        string[] lines = ButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                            //        foreach (var text in lines)
                            //        {
                            //            noidungbutphe += "\n";
                            //            noidungbutphe += text;
                            //        }
                            //    }
                            //}
                            //thongtinky = noidungbutphe == "" ? thongtinky : (thongtinky + "\nBút phê: " + noidungbutphe);
                        }
                        else if (signerFlow.LoaiKy == 3)
                        {
                            ((PdfHashSigner)signer).SetCustomImage(Convert.FromBase64String(signerFlow.FILEANH.Split(',')[1]));
                            ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.LOGO_ONLY);
                        }
                        foreach (var objPosition in temp)
                        {
                            ((PdfHashSigner)signer).AddSignatureView(new PdfSignatureView
                            {
                                Rectangle = objPosition.Rectangle,
                                Page = objPosition.SignPage
                            });
                        }
                        ((PdfHashSigner)signer).SetLayer2Text(thongtinky);
                        ((PdfHashSigner)signer).SetFontSize(signerFlow.LoaiKy == 2 ? 13 : 10);
                        ((PdfHashSigner)signer).SetFontColor("0000ff");
                        ((PdfHashSigner)signer).SetFontStyle(PdfHashSigner.FontStyle.Normal);
                        ((PdfHashSigner)signer).SetFontName(PdfHashSigner.FontName.Times_New_Roman);
                        ((PdfHashSigner)signer).SetSignatureBorderType(PdfHashSigner.VisibleSigBorder.DASHED);
                    }
                }
                #endregion -----------------------------------------

                var test = new FilePDFEntryDto();
                test.Id = item.Id;
                test.FileName = item.FileName;
                test.HashData = signer.GetSecondHashAsBase64();
                hashValueList.Add(test);
                signerList.Add(signer);
            }

            var tranID = _smartCAFunction._signHash(userLogin.AccessToken, "https://gwsca.vnpt.vn/csc/signature/signhash", hashValueList, userLogin.Credential);

            if (tranID == "")
            {
                return -1;
            }

            await _smartCAFunction.UpdateTranIdSigner(signerEntryDto.Id, tranID);
            var count = 0;
            var isConfirm = false;
            var datasigneds = new List<String>();

            while (count < 24 && !isConfirm)
            {
                var tranInfo = _smartCAFunction._getTranInfo(userLogin.AccessToken, "https://gwsca.vnpt.vn/csc/credentials/gettraninfo", tranID);
                if (tranInfo != null)
                {
                    if (tranInfo.tranStatus == 4000) // WAITING_FOR_SIGNER_CONFIRM
                    {
                        count = count + 1;
                        Thread.Sleep(10000);
                    }
                    else if (tranInfo.tranStatus == 4001) // EXPIRED
                    {
                        await _smartCAFunction.UpdateTranIdSigner(signerEntryDto.Id, null);
                        if (signerFlow.isChecked < 2)
                        {
                            vc2.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã quá hạn", signerEntryDto.UserName, signerFlow.isChecked + 1, 0, SignTable.Status);
                        }
                        else
                        {
                            vc2.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã quá hạn", signerEntryDto.UserName, signerFlow.isChecked, 0, SignTable.Status);
                            await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, -1);
                            await _smartCAFunction.UpdateSignersAndFiles(SignTable.Id);
                            if (fileList.ToList().Count > 0)
                            {
                                foreach (var item in fileList)
                                {
                                    string sPath = System.Web.Hosting.HostingEnvironment.MapPath(pathUrl);
                                    //sPath = Path.Combine(sPath, item.CreatedDate.ToString("yyyy"));
                                    //sPath = Path.Combine(sPath, item.CreatedDate.ToString("MM"));
                                    sPath = Path.Combine(sPath, item.FilePath);
                                    System.IO.File.WriteAllBytes(sPath, Convert.FromBase64String(item.UnsignData));
                                }
                            }
                            await _smartCAFunction.CallBackAPI(4002, SignTable, fileList);
                        }
                        return 4001;
                    }

                    else if (tranInfo.tranStatus == 4002) // SIGNER_REJECTED
                    {
                        await _smartCAFunction.UpdateTranIdSigner(signerEntryDto.Id, null);
                        vc2.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã hủy ký", signerEntryDto.UserName, signerFlow.isChecked, 0, SignTable.Status);
                        await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, -1);
                        await _smartCAFunction.UpdateSignersAndFiles(SignTable.Id);
                        if (fileList.ToList().Count > 0)
                        {
                            foreach (var item in fileList)
                            {
                                string sPath = System.Web.Hosting.HostingEnvironment.MapPath(pathUrl);
                                //sPath = Path.Combine(sPath, item.CreatedDate.ToString("yyyy"));
                                //sPath = Path.Combine(sPath, item.CreatedDate.ToString("MM"));
                                sPath = Path.Combine(sPath, item.FilePath);
                                System.IO.File.WriteAllBytes(sPath, Convert.FromBase64String(item.UnsignData));
                            }
                        }
                        await _smartCAFunction.CallBackAPI(4002, SignTable, fileList);
                        return 4002;
                    }
                    else
                    {
                        isConfirm = true;
                        foreach (var document in tranInfo.documents)
                        {
                            datasigneds.Add(document.sig);
                        }
                    }
                }
                else
                {
                    return -2;
                }
            }

            if (!isConfirm)
            {
                return -3;
            }

            if (datasigneds.Count == 0)
            {
                return -4;
            }

            if (isConfirm) // SIGNED SUCCESS
            {
                for (int i = 0; i < signerList.Count; i++)
                {
                    var signer = signerList[i];
                    var datasigned = datasigneds[i];
                    var filesigned = hashValueList[i];
                    if (!signer.CheckHashSignature(datasigned))
                    {
                        return -5;
                    }
                    byte[] signed = signer.Sign(datasigned);
                    var outputPath = pdfOutputPaths[i];
                    System.IO.File.WriteAllBytes(outputPath, signed);
                    await _smartCAFunction.UpdateSignedFile(filesigned.Id, Convert.ToBase64String(signed));
                }
                vc2.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã ký duyệt", signerEntryDto.UserName, signerFlow.isChecked, 0, SignTable.Status);
                var nguoikytieptheo = await _smartCAFunction.FindSigner(SignTable.Id, SignTable.Status + 1);
                if (nguoikytieptheo != null)
                {
                    await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, SignTable.Status + 1);
                    qlcc.CapNhatNguoiDuyetVB(RefId, nguoikytieptheo.UserName, SignTable.Id, Module);
                    return 11;
                }
                else
                {
                    await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, -99);
                    qlcc.CapNhatNguoiDuyetVB(RefId, null, SignTable.Id, Module);
                    await _smartCAFunction.CallBackAPI(1, SignTable, fileList);
                    return 1;
                }
            }
            return -6;
        }

        public async Task<int> SignPDFLenhDieuXe(SignerFrontEndViewDto signerEntryDto, IEnumerable<FilePDFEntryDto> fileList, int Module, int RefId, SignEntryDto SignTable, Core_UserDto signerFlow)
        {
            UserModel userLogin = await _smartCAFunction.GetUserSmartCA(signerEntryDto.UserName);
            if (userLogin == null)
            {
                return -7;
            }
            var signerList = new List<IHashSigner>();
            var hashValueList = new List<FilePDFEntryDto>();
            var pdfOutputPaths = new List<String>();
            foreach (var item in fileList)
            {
                byte[] unsignData = null;
                string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                sPath = Path.Combine(sPath, item.CreatedDate.ToString("yyyy"));
                sPath = Path.Combine(sPath, item.CreatedDate.ToString("MM"));
                string savepath = Path.Combine(sPath, item.FilePath);
                pdfOutputPaths.Add(savepath);
                sPath = Path.Combine(sPath, item.FilePath);
                try
                {
                    unsignData = System.IO.File.ReadAllBytes(sPath);
                }
                catch
                {
                    return -1;
                }

                IHashSigner signer = HashSignerFactory.GenerateSigner(unsignData, userLogin.CertBase64, null, HashSignerFactory.PDF);
                ((PdfHashSigner)signer).SetHashAlgorithm(MessageDigestAlgorithm.SHA256);

                #region Optional -----------------------------------
                if (signerEntryDto.ObjPosition != null)
                {
                    List<pdfInfo> pdf = JsonConvert.DeserializeObject<List<pdfInfo>>(signerEntryDto.ObjPosition);
                    var temp = pdf.Where(x => x.FileName == item.FileName);
                    if (temp.Count() > 0)
                    {
                        string _imgBytes = System.Web.Hosting.HostingEnvironment.MapPath("~/VNPT/SignImg/");
                        _imgBytes += signerEntryDto.UserName + @"\";
                        _imgBytes = Path.Combine(_imgBytes, userLogin.ImgPath);
                        ((PdfHashSigner)signer).SetReason("Xác nhận tài liệu");
                        string ngayky = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + " (" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ")";
                        string thongtinky = "Ký bởi: " + signerFlow.FullName + "\nChức vụ: " + signerFlow.CHUCVU + "\nNgày ký: " + ngayky;
                        if (signerFlow.LoaiKy == 0)
                        {
                            ((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
                            ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.LOGO_ONLY);
                        }
                        else if (signerFlow.LoaiKy == 1)
                        {
                            ((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
                            ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_WITH_BACKGROUND);
                        }
                        else if (signerFlow.LoaiKy == 2)
                        {
                            ((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
                            ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_ONLY);
                        }
                        else if (signerFlow.LoaiKy == 3)
                        {
                            ((PdfHashSigner)signer).SetCustomImage(Convert.FromBase64String(signerFlow.FILEANH.Split(',')[1]));
                            ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.LOGO_ONLY);
                        }
                        foreach (var objPosition in temp)
                        {
                            ((PdfHashSigner)signer).AddSignatureView(new PdfSignatureView
                            {
                                Rectangle = objPosition.Rectangle,
                                Page = objPosition.SignPage
                            });
                        }
                        ((PdfHashSigner)signer).SetLayer2Text(thongtinky);
                        ((PdfHashSigner)signer).SetFontSize(10);
                        ((PdfHashSigner)signer).SetFontColor("0000ff");
                        ((PdfHashSigner)signer).SetFontStyle(PdfHashSigner.FontStyle.Normal);
                        ((PdfHashSigner)signer).SetFontName(PdfHashSigner.FontName.Times_New_Roman);
                        ((PdfHashSigner)signer).SetSignatureBorderType(PdfHashSigner.VisibleSigBorder.DASHED);
                    }
                }
                #endregion -----------------------------------------

                var test = new FilePDFEntryDto();
                test.Id = item.Id;
                test.FileName = item.FileName;
                test.HashData = signer.GetSecondHashAsBase64();
                hashValueList.Add(test);
                signerList.Add(signer);
            }

            var tranID = _smartCAFunction._signHash(userLogin.AccessToken, "https://gwsca.vnpt.vn/csc/signature/signhash", hashValueList, userLogin.Credential);

            if (tranID == "")
            {
                return -1;
            }

            await _smartCAFunction.UpdateTranIdSigner(signerEntryDto.Id, tranID);
            var count = 0;
            var isConfirm = false;
            var datasigneds = new List<String>();

            while (count < 24 && !isConfirm)
            {
                var tranInfo = _smartCAFunction._getTranInfo(userLogin.AccessToken, "https://gwsca.vnpt.vn/csc/credentials/gettraninfo", tranID);
                if (tranInfo != null)
                {
                    if (tranInfo.tranStatus == 4000) // WAITING_FOR_SIGNER_CONFIRM
                    {
                        count = count + 1;
                        Thread.Sleep(10000);
                    }

                    else if (tranInfo.tranStatus == 4001) // EXPIRED
                    {
                        await _smartCAFunction.UpdateTranIdSigner(signerEntryDto.Id, null);
                        if (signerFlow.isChecked < 2)
                        {
                            vc2.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã quá hạn", signerEntryDto.UserName, signerFlow.isChecked + 1,0, SignTable.Status);
                        }
                        else
                        {
                            vc2.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã quá hạn", signerEntryDto.UserName, signerFlow.isChecked, 0, SignTable.Status);
                            await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, -1);
                            await _smartCAFunction.UpdateSignersAndFiles(SignTable.Id);
                            if (fileList.ToList().Count > 0)
                            {
                                foreach (var item in fileList)
                                {
                                    string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                                    sPath = Path.Combine(sPath, item.CreatedDate.ToString("yyyy"));
                                    sPath = Path.Combine(sPath, item.CreatedDate.ToString("MM"));
                                    sPath = Path.Combine(sPath, item.FilePath);
                                    System.IO.File.WriteAllBytes(sPath, Convert.FromBase64String(item.UnsignData));
                                }
                            }
                            await _smartCAFunction.CallBackAPI(4002, SignTable, fileList);
                        }
                        return 4001;
                    }

                    else if (tranInfo.tranStatus == 4002) // SIGNER_REJECTED
                    {
                        await _smartCAFunction.UpdateTranIdSigner(signerEntryDto.Id, null);
                        vc2.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã hủy ký", signerEntryDto.UserName, signerFlow.isChecked, 0, SignTable.Status);
                        await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, -1);
                        await _smartCAFunction.UpdateSignersAndFiles(SignTable.Id);
                        if (fileList.ToList().Count > 0)
                        {
                            foreach (var item in fileList)
                            {
                                string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
                                sPath = Path.Combine(sPath, item.CreatedDate.ToString("yyyy"));
                                sPath = Path.Combine(sPath, item.CreatedDate.ToString("MM"));
                                sPath = Path.Combine(sPath, item.FilePath);
                                System.IO.File.WriteAllBytes(sPath, Convert.FromBase64String(item.UnsignData));
                            }
                        }
                        await _smartCAFunction.CallBackAPI(4002, SignTable, fileList);
                        return 4002;
                    }

                    else if (tranInfo.tranStatus == 1) // SIGNED SUCCESS
                    {
                        isConfirm = true;
                        foreach (var document in tranInfo.documents)
                        {
                            datasigneds.Add(document.sig);
                        }
                    }
                }
                else
                {
                    return -2;
                }
            }

            if (!isConfirm)
            {
                return -3;
            }

            if (datasigneds.Count == 0)
            {
                return -4;
            }

            if (isConfirm) // SIGNED SUCCESS
            {
                for (int i = 0; i < signerList.Count; i++)
                {
                    var signer = signerList[i];
                    var datasigned = datasigneds[i];
                    var filesigned = hashValueList[i];
                    if (!signer.CheckHashSignature(datasigned))
                    {
                        return -5;
                    }
                    byte[] signed = signer.Sign(datasigned);
                    var outputPath = pdfOutputPaths[i];
                    System.IO.File.WriteAllBytes(outputPath, signed);
                    await _smartCAFunction.UpdateSignedFile(filesigned.Id, Convert.ToBase64String(signed));
                }
                vc2.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã ký duyệt", signerEntryDto.UserName, signerFlow.isChecked, 0, SignTable.Status);
                var nguoikytieptheo = await _smartCAFunction.FindSigner(SignTable.Id, SignTable.Status + 1);
                if (nguoikytieptheo != null)
                {
                    await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, SignTable.Status + 1);
                    return 11;
                }
                else
                {
                    await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, -99);
                    await _smartCAFunction.CallBackAPI(1, SignTable, fileList);
                    return 1;
                }
            }
            return -6;
        }

        public async Task<int> SignPDFCongVan(SignerFrontEndViewDto signerEntryDto, IEnumerable<FilePDFEntryDto> fileList, int Module, int RefId, SignEntryDto SignTable, Core_UserDto signerFlow, string ButPhe)
        {
            UserModel userLogin = await _smartCAFunction.GetUserSmartCA(signerEntryDto.UserName);
            if (userLogin == null)
            {
                return -7;
            }
            var signerList = new List<IHashSigner>();
            var hashValueList = new List<FilePDFEntryDto>();
            var pdfOutputPaths = new List<String>();
            foreach (var item in fileList)
            {
                byte[] unsignData = null;
                string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CongVanFile/");
                sPath = Path.Combine(sPath, item.CreatedDate.ToString("yyyy"));
                sPath = Path.Combine(sPath, item.CreatedDate.ToString("MM"));
                string savepath = Path.Combine(sPath, item.FilePath);
                pdfOutputPaths.Add(savepath);
                sPath = Path.Combine(sPath, item.FilePath);
                try
                {
                    unsignData = System.IO.File.ReadAllBytes(sPath);
                }
                catch
                {
                    return -1;
                }

                IHashSigner signer = HashSignerFactory.GenerateSigner(unsignData, userLogin.CertBase64, null, HashSignerFactory.PDF);
                ((PdfHashSigner)signer).SetHashAlgorithm(MessageDigestAlgorithm.SHA256);

                #region Optional -----------------------------------
                string ngayky = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + " (" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ")";
                if (signerEntryDto.ObjPosition != null)
                {
                    List<pdfInfo> pdf = JsonConvert.DeserializeObject<List<pdfInfo>>(signerEntryDto.ObjPosition);
                    var temp = pdf.Where(x => x.FileName == item.FileName);
                    if (temp.Count() > 0)
                    {
                        string _imgBytes = System.Web.Hosting.HostingEnvironment.MapPath("~/VNPT/SignImg/");
                        string thongtinky = Module == 0 ? ("Ký bởi: " + signerFlow.FullName + "\nChức vụ: " + signerFlow.CHUCVU + "\nNgày ký: " + ngayky) : "";
                        _imgBytes += signerEntryDto.UserName + @"\";
                        _imgBytes = Path.Combine(_imgBytes, userLogin.ImgPath);
                        ((PdfHashSigner)signer).SetReason("Xác nhận tài liệu");
                        if (signerFlow.LoaiKy == 0)
                        {
                            //((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
                            //((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.LOGO_ONLY);

                            ((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
                            string noidungbutphe = "";
                            if (signerFlow.NoiDungButPhe != null)
                            {
                                ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_WITH_BACKGROUND);
                                string[] lines = signerFlow.NoiDungButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                                foreach (var text in lines)
                                {
                                    noidungbutphe += "\n";
                                    noidungbutphe += text;
                                }
                            }
                            else
                            {
                                ((PdfHashSigner)signer).SetRenderingMode(Module == 0 ? PdfHashSigner.RenderMode.TEXT_WITH_BACKGROUND : PdfHashSigner.RenderMode.LOGO_ONLY);
                                //if (ButPhe != null)
                                //{
                                //    ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_WITH_BACKGROUND);
                                //    string[] lines = ButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                                //    foreach (var text in lines)
                                //    {
                                //        noidungbutphe += "\n";
                                //        noidungbutphe += text;
                                //    }
                                //}
                                //else
                                //{
                                //    ((PdfHashSigner)signer).SetRenderingMode(Module == 0 ? PdfHashSigner.RenderMode.TEXT_WITH_BACKGROUND : PdfHashSigner.RenderMode.LOGO_ONLY);
                                //}
                            }
                            thongtinky = noidungbutphe == "" ? thongtinky : (thongtinky + "\nBút phê chỉ đạo: " + noidungbutphe);
                        }
                        else if (signerFlow.LoaiKy == 1)
                        {
                            ((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
                            ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_WITH_BACKGROUND);
                            string noidungbutphe = "";
                            if (signerFlow.NoiDungButPhe != null)
                            {
                                string[] lines = signerFlow.NoiDungButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                                foreach (var text in lines)
                                {
                                    noidungbutphe += "\n";
                                    noidungbutphe += text;
                                }
                            }
                            else
                            {
                                if(ButPhe != null)
                                {
                                    string[] lines = ButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                                    foreach (var text in lines)
                                    {
                                        noidungbutphe += "\n";
                                        noidungbutphe += text;
                                    }
                                }
                            }
                            thongtinky = noidungbutphe == "" ? thongtinky : (thongtinky + "\nBút phê: " + noidungbutphe);
                        }
                        else if (signerFlow.LoaiKy == 2)
                        {
                            ((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
                            ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_ONLY);
                            thongtinky = signerFlow.SoVanBan;

                            //((PdfHashSigner)signer).SetCustomImage(System.IO.File.ReadAllBytes(_imgBytes));
                            //((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_ONLY);
                            //string noidungbutphe = "";
                            //if (signerFlow.NoiDungButPhe != null)
                            //{
                            //    string[] lines = signerFlow.NoiDungButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                            //    foreach (var text in lines)
                            //    {
                            //        noidungbutphe += "\n";
                            //        noidungbutphe += text;
                            //    }
                            //}
                            //else
                            //{
                            //    if (ButPhe != null)
                            //    {
                            //        string[] lines = ButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                            //        foreach (var text in lines)
                            //        {
                            //            noidungbutphe += "\n";
                            //            noidungbutphe += text;
                            //        }
                            //    }
                            //}
                            //thongtinky = noidungbutphe == "" ? thongtinky : (thongtinky + "\nBút phê: " + noidungbutphe);
                        }
                        else if (signerFlow.LoaiKy == 3)
                        {
                            ((PdfHashSigner)signer).SetCustomImage(Convert.FromBase64String(signerFlow.FILEANH.Split(',')[1]));
                            ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.LOGO_ONLY);
                        }
                        foreach (var objPosition in temp)
                        {
                            ((PdfHashSigner)signer).AddSignatureView(new PdfSignatureView
                            {
                                Rectangle = objPosition.Rectangle,
                                Page = objPosition.SignPage
                            });
                        }
                        ((PdfHashSigner)signer).SetLayer2Text(thongtinky);
                        ((PdfHashSigner)signer).SetFontSize(signerFlow.LoaiKy == 2 ? 13 : 10);
                        ((PdfHashSigner)signer).SetFontColor("0000ff");
                        ((PdfHashSigner)signer).SetFontStyle(PdfHashSigner.FontStyle.Normal);
                        ((PdfHashSigner)signer).SetFontName(PdfHashSigner.FontName.Times_New_Roman);
                        ((PdfHashSigner)signer).SetSignatureBorderType(PdfHashSigner.VisibleSigBorder.DASHED);
                    }
                }

                else if (signerEntryDto.ObjPosition == null && signerFlow.LoaiKy == 2)
                {
                    string thongtinky = "Ký bởi: " + signerFlow.FullName + "\nChức vụ: " + signerFlow.CHUCVU + "\nNgày ký: " + ngayky;
                    ((PdfHashSigner)signer).SetReason("Xác nhận tài liệu");
                    ((PdfHashSigner)signer).SetRenderingMode(PdfHashSigner.RenderMode.TEXT_ONLY);
                    ((PdfHashSigner)signer).SetLayer2Text(thongtinky);
                    ((PdfHashSigner)signer).SetFontSize(10);
                    ((PdfHashSigner)signer).SetFontColor("0000ff");
                    ((PdfHashSigner)signer).SetFontStyle(PdfHashSigner.FontStyle.Normal);
                    ((PdfHashSigner)signer).SetFontName(PdfHashSigner.FontName.Times_New_Roman);
                    ((PdfHashSigner)signer).AddSignatureView(new PdfSignatureView
                    {
                        Rectangle = "0,0,200,200",
                        Page = 1
                    });
                    ((PdfHashSigner)signer).SetSignatureBorderType(PdfHashSigner.VisibleSigBorder.DASHED);
                }
                #endregion -----------------------------------------

                var test = new FilePDFEntryDto();
                test.Id = item.Id;
                test.FileName = item.FileName;
                test.HashData = signer.GetSecondHashAsBase64();
                hashValueList.Add(test);
                signerList.Add(signer);
            }

            var tranID = _smartCAFunction._signHash(userLogin.AccessToken, "https://gwsca.vnpt.vn/csc/signature/signhash", hashValueList, userLogin.Credential);

            if (tranID == "")
            {
                return -1;
            }

            await _smartCAFunction.UpdateTranIdSigner(signerEntryDto.Id, tranID);
            var count = 0;
            var isConfirm = false;
            var datasigneds = new List<String>();

            while (count < 24 && !isConfirm)
            {
                var tranInfo = _smartCAFunction._getTranInfo(userLogin.AccessToken, "https://gwsca.vnpt.vn/csc/credentials/gettraninfo", tranID);
                if (tranInfo != null)
                {
                    if (tranInfo.tranStatus == 4000) // WAITING_FOR_SIGNER_CONFIRM
                    {
                        count = count + 1;
                        Thread.Sleep(10000);
                    }
                    else if (tranInfo.tranStatus == 4001) // EXPIRED
                    {
                        await _smartCAFunction.UpdateTranIdSigner(signerEntryDto.Id, null);
                        if (signerFlow.isChecked < 2)
                        {
                            vc.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã quá hạn", signerEntryDto.UserName, signerFlow.isChecked + 1, SignTable.Status);
                        }
                        else
                        {
                            vc.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã quá hạn", signerEntryDto.UserName, signerFlow.isChecked, SignTable.Status);
                            await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, -1);
                            await _smartCAFunction.UpdateSignersAndFiles(SignTable.Id);
                            if (fileList.ToList().Count > 0)
                            {
                                foreach (var item in fileList)
                                {
                                    string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CongVanFile/");
                                    sPath = Path.Combine(sPath, item.CreatedDate.ToString("yyyy"));
                                    sPath = Path.Combine(sPath, item.CreatedDate.ToString("MM"));
                                    sPath = Path.Combine(sPath, item.FilePath);
                                    System.IO.File.WriteAllBytes(sPath, Convert.FromBase64String(item.UnsignData));
                                }
                            }
                            await _smartCAFunction.CallBackAPI(4002, SignTable, fileList);
                        }
                        return 4001;
                    }
                    else if (tranInfo.tranStatus == 4002) // SIGNER_REJECTED
                    {
                        await _smartCAFunction.UpdateTranIdSigner(signerEntryDto.Id, null);
                        vc.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã hủy ký", signerEntryDto.UserName, signerFlow.isChecked, SignTable.Status);
                        await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, -1);
                        await _smartCAFunction.UpdateSignersAndFiles(SignTable.Id);
                        if (fileList.ToList().Count > 0)
                        {
                            foreach (var item in fileList)
                            {
                                string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CongVanFile/");
                                sPath = Path.Combine(sPath, item.CreatedDate.ToString("yyyy"));
                                sPath = Path.Combine(sPath, item.CreatedDate.ToString("MM"));
                                sPath = Path.Combine(sPath, item.FilePath);
                                System.IO.File.WriteAllBytes(sPath, Convert.FromBase64String(item.UnsignData));
                            }
                        }
                        await _smartCAFunction.CallBackAPI(4002, SignTable, fileList);
                        return 4002;
                    }
                    else
                    {
                        isConfirm = true;
                        foreach (var document in tranInfo.documents)
                        {
                            datasigneds.Add(document.sig);
                        }
                    }
                }
                else
                {
                    return -2;
                }
            }

            if (!isConfirm)
            {
                return -3;
            }

            if (datasigneds.Count == 0)
            {
                return -4;
            }

            if (isConfirm) // SIGNED SUCCESS
            {
                var nguoikytieptheo = await _smartCAFunction.FindSigner(SignTable.Id, SignTable.Status + 1);

                for (int i = 0; i < signerList.Count; i++)
                {
                    var signer = signerList[i];
                    var datasigned = datasigneds[i];
                    var filesigned = hashValueList[i];
                    if (!signer.CheckHashSignature(datasigned))
                    {
                        return -5;
                    }
                    byte[] signed = signer.Sign(datasigned);
                    var outputPath = pdfOutputPaths[i];
                    System.IO.File.WriteAllBytes(outputPath, signed);
                    await _smartCAFunction.UpdateSignedFile(filesigned.Id, Convert.ToBase64String(signed));

                    // Cập nhật lưu file ký số đầy đủ chữ ký của BLĐ trước khi đóng mộc
                    SaveFileChuaDongMoc(nguoikytieptheo, outputPath, signed, Module);
                }
                vc.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã ký duyệt", signerEntryDto.UserName, signerFlow.isChecked, SignTable.Status);
                if (nguoikytieptheo != null)
                {
                    await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, SignTable.Status + 1);
                    vc.CapNhatNguoiDuyetVB(RefId, nguoikytieptheo.UserName, SignTable.Id, Module);
                    return 11;
                }
                else
                {
                    await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, -99);
                    vc.CapNhatNguoiDuyetVB(RefId, null, SignTable.Id, Module);
                    await _smartCAFunction.CallBackAPI(1, SignTable, fileList);
                    return 1;
                }
            }
            return -6;
        }

        public async Task<int> SignNoSmartCAPDFCongVan(SignerFrontEndViewDto signerEntryDto, IEnumerable<FilePDFEntryDto> fileList, int Module, int RefId, SignEntryDto SignTable, Core_UserDto signerFlow, string ButPhe)
        {
            UserModel userLogin = await _smartCAFunction.GetUserSmartCA(signerEntryDto.UserName);
            if (userLogin == null)
            {
                return -7;
            }
            if (fileList.Count() > 0)
            {
                string ngayky = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + " (" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ")";
                string imgPath = System.Web.Hosting.HostingEnvironment.MapPath("~/VNPT/SignImg/");
                foreach (var item in fileList)
                {
                    string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CongVanFile/");
                    sPath = Path.Combine(sPath, item.CreatedDate.ToString("yyyy"));
                    sPath = Path.Combine(sPath, item.CreatedDate.ToString("MM"));
                    string savepath = Path.Combine(sPath, item.FilePath);
                    if (signerEntryDto.ObjPosition != null)
                    {
                        List<pdfInfo> pdf = JsonConvert.DeserializeObject<List<pdfInfo>>(signerEntryDto.ObjPosition);
                        var temp = pdf.Where(x => x.FileName == item.FileName);
                        if (temp.Count() > 0)
                        {
                            byte[] _imgBytes = null;
                            string thongtinky = "Ký bởi: " + signerFlow.FullName + "\nChức vụ: " + signerFlow.CHUCVU + "\nNgày ký: " + ngayky;
                            if (signerFlow.LoaiKy == 0 || signerFlow.LoaiKy == 1 || signerFlow.LoaiKy == 2)
                            {
                                imgPath += signerEntryDto.UserName.ToLower() + @"\";
                                _imgBytes = System.IO.File.ReadAllBytes(Path.Combine(imgPath, userLogin.ImgPath));

                                string noidungbutphe = "";
                                if (signerFlow.NoiDungButPhe != null)
                                {
                                    string[] lines = signerFlow.NoiDungButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                                    foreach (var text in lines)
                                    {
                                        noidungbutphe += "\n";
                                        noidungbutphe += text;
                                    }
                                }
                                else
                                {
                                    noidungbutphe = "";
                                    //if (ButPhe != null)
                                    //{
                                    //    string[] lines = ButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                                    //    foreach (var text in lines)
                                    //    {
                                    //        noidungbutphe += "\n";
                                    //        noidungbutphe += text;
                                    //    }
                                    //}
                                }
                                thongtinky = noidungbutphe == "" ? null : (thongtinky + "\nBút phê chỉ đạo: " + noidungbutphe);
                            }
                            else if (signerFlow.LoaiKy == 3) // ký con dấu vuông
                            {
                                _imgBytes = Convert.FromBase64String(signerFlow.FILEANH.Split(',')[1]);
                            }
                            foreach (var objPosition in temp)
                            {
                                byte[] unsignData = null;
                                try
                                {
                                    unsignData = System.IO.File.ReadAllBytes(savepath);
                                }
                                catch
                                {
                                    return -1;
                                }
                                //format recSmartCA (llx,lly,urx,ury)
                                //format recNormal (x,y,w,h) => (llx,lly,w = urx - llx,h = ury - lly)
                                string LeftLlxRec = objPosition.Rectangle.Split(',')[0];
                                string BottomLlyRec = objPosition.Rectangle.Split(',')[1];
                                string RightUrxRec = objPosition.Rectangle.Split(',')[2];
                                string TopUryRec = objPosition.Rectangle.Split(',')[3];
                                int llx = 0;
                                int lly = 0;
                                int urx = 0;
                                int ury = 0;
                                Int32.TryParse(LeftLlxRec, out llx);
                                Int32.TryParse(BottomLlyRec, out lly);
                                Int32.TryParse(RightUrxRec, out urx);
                                Int32.TryParse(TopUryRec, out ury);
                                int height = ury - lly; // height = top - bottom
                                int width = urx - llx; // width = right - left

                                if(signerFlow.NoiDungButPhe == null && signerFlow.LoaiKy != 3) // check nếu không có bút phê và không phải là ký mộc vuông, ô sẽ tự scale chiều dài hoặc dọc lại để chỉ thể hiện chữ ký
                                {
                                    int MAX_WIDTH = 100; // xử lý cho trường hợp kéo chữ ký quá rộng
                                    double SCALE = 1.0; // xử lý cho trường hợp kéo chữ ký quá dài

                                    width = Math.Min(width, MAX_WIDTH);

                                    if (height > width * SCALE)
                                    {
                                        height = (int)(width * SCALE);
                                    }

                                    lly = ury - height;
                                    urx = width + llx;
                                }

                                using (Stream inputPdfStream = new FileStream(savepath, FileMode.Create))
                                {
                                    var reader = new PdfReader(unsignData);
                                    var stamper = new PdfStamper(reader, inputPdfStream);
                                    if (thongtinky == null || signerFlow.LoaiKy == 3) // logo only
                                    {
                                        var pdfContentByte = stamper.GetOverContent(objPosition.SignPage);

                                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(_imgBytes);
                                        image.SetAbsolutePosition(llx, lly); // x,y
                                        image.ScaleAbsoluteWidth(width); // w
                                        image.ScaleAbsoluteHeight(height); // h
                                        pdfContentByte.AddImage(image);
                                    }
                                    else // text only
                                    {
                                        string fontFile = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/Font/times.ttf");
                                        var bf = BaseFont.CreateFont(fontFile, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                                        var tf = new TextField(stamper.Writer, new Rectangle(llx, lly, urx, ury), "newTextField")
                                        {
                                            BorderColor = BaseColor.LIGHT_GRAY,
                                            BorderStyle = PdfBorderDictionary.STYLE_DASHED,
                                            BorderWidth = 0,
                                            DefaultText = "This is a new text field.",
                                            Font = bf,
                                            FontSize = 10,
                                            TextColor = BaseColor.BLUE,
                                            //MaxCharacterLength = thongtinky.Length,
                                            Options = TextField.READ_ONLY | TextField.MULTILINE,
                                            Text = thongtinky
                                        };
                                        var pdfContentByte = stamper.GetOverContent(objPosition.SignPage);
                                        //Get the raw form field
                                        var FF = tf.GetTextField();
                                        //Set the background color
                                        FF.MKBackgroundColor = BaseColor.LIGHT_GRAY;
                                        stamper.AddAnnotation(FF, objPosition.SignPage);
                                    }
                                    stamper.Close();
                                }
                            }
                            await _smartCAFunction.UpdateSignedFile(item.Id, Convert.ToBase64String(System.IO.File.ReadAllBytes(savepath)));
                        }
                        //else return -7;
                    }
                    else return -7;
                    //await _smartCAFunction.UpdateSignedFile(item.Id, Convert.ToBase64String(System.IO.File.ReadAllBytes(savepath)));
                }
                vc.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã ký duyệt", signerEntryDto.UserName, signerFlow.isChecked, SignTable.Status);
                var nguoikytieptheo = await _smartCAFunction.FindSigner(SignTable.Id, SignTable.Status + 1);
                if (nguoikytieptheo != null)
                {
                    await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, SignTable.Status + 1);
                    vc.CapNhatNguoiDuyetVB(RefId, nguoikytieptheo.UserName, SignTable.Id, Module);
                    return 11;
                }
                else
                {
                    await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, -99);
                    vc.CapNhatNguoiDuyetVB(RefId, null, SignTable.Id, Module);
                    await _smartCAFunction.CallBackAPI(1, SignTable, fileList);
                    return 1;
                }
            }
            else return -7;
        }

        public async Task<int> SignNoSmartCAPDFCongVan_ResetFile(SignerFrontEndViewDto signerEntryDto, IEnumerable<FilePDFEntryDto> fileList, Core_UserDto signerFlow)
        {
            UserModel userLogin = await _smartCAFunction.GetUserSmartCA(signerEntryDto.UserName);
            if (userLogin == null)
            {
                return -7;
            }
            if (fileList.Count() > 0)
            {
                string ngayky = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + " (" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ")";
                string imgPath = System.Web.Hosting.HostingEnvironment.MapPath("~/VNPT/SignImg/");
                foreach (var item in fileList)
                {
                    string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CongVanFile/");
                    sPath = Path.Combine(sPath, item.CreatedDate.ToString("yyyy"));
                    sPath = Path.Combine(sPath, item.CreatedDate.ToString("MM"));
                    string savepath = Path.Combine(sPath, item.FilePath);
                    if (signerEntryDto.ObjPosition != null)
                    {
                        List<pdfInfo> pdf = JsonConvert.DeserializeObject<List<pdfInfo>>(signerEntryDto.ObjPosition);
                        var temp = pdf.Where(x => x.FileName == item.FileName);
                        if (temp.Count() > 0)
                        {
                            byte[] _imgBytes = null;
                            string thongtinky = "Ký bởi: " + signerFlow.FullName + "\nChức vụ: " + signerFlow.CHUCVU + "\nNgày ký: " + ngayky;
                            if (signerFlow.LoaiKy == 0 || signerFlow.LoaiKy == 1 || signerFlow.LoaiKy == 2)
                            {
                                imgPath += signerEntryDto.UserName.ToLower() + @"\";
                                _imgBytes = System.IO.File.ReadAllBytes(Path.Combine(imgPath, userLogin.ImgPath));

                                string noidungbutphe = "";
                                if (signerFlow.NoiDungButPhe != null)
                                {
                                    string[] lines = signerFlow.NoiDungButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                                    foreach (var text in lines)
                                    {
                                        noidungbutphe += "\n";
                                        noidungbutphe += text;
                                    }
                                }
                                else
                                {
                                    noidungbutphe = "";
                                }
                                thongtinky = noidungbutphe == "" ? null : (thongtinky + "\nBút phê chỉ đạo: " + noidungbutphe);
                            }
                            else if (signerFlow.LoaiKy == 3) // ký con dấu vuông
                            {
                                _imgBytes = Convert.FromBase64String(signerFlow.FILEANH.Split(',')[1]);
                            }
                            foreach (var objPosition in temp)
                            {
                                byte[] unsignData = null;
                                try
                                {
                                    unsignData = System.IO.File.ReadAllBytes(savepath);
                                }
                                catch
                                {
                                    return -1;
                                }
                                //format recSmartCA (llx,lly,urx,ury)
                                //format recNormal (x,y,w,h) => (llx,lly,w = urx - llx,h = ury - lly)
                                string LeftLlxRec = objPosition.Rectangle.Split(',')[0];
                                string BottomLlyRec = objPosition.Rectangle.Split(',')[1];
                                string RightUrxRec = objPosition.Rectangle.Split(',')[2];
                                string TopUryRec = objPosition.Rectangle.Split(',')[3];
                                int llx = 0;
                                int lly = 0;
                                int urx = 0;
                                int ury = 0;
                                Int32.TryParse(LeftLlxRec, out llx);
                                Int32.TryParse(BottomLlyRec, out lly);
                                Int32.TryParse(RightUrxRec, out urx);
                                Int32.TryParse(TopUryRec, out ury);
                                int height = ury - lly; // height = top - bottom
                                int width = urx - llx; // width = right - left

                                if (signerFlow.NoiDungButPhe == null && signerFlow.LoaiKy != 3) // check nếu không có bút phê và không phải là ký mộc vuông, ô sẽ tự scale chiều dài hoặc dọc lại để chỉ thể hiện chữ ký
                                {
                                    int MAX_WIDTH = 100; // xử lý cho trường hợp kéo chữ ký quá rộng
                                    double SCALE = 1.0; // xử lý cho trường hợp kéo chữ ký quá dài

                                    width = Math.Min(width, MAX_WIDTH);

                                    if (height > width * SCALE)
                                    {
                                        height = (int)(width * SCALE);
                                    }

                                    lly = ury - height;
                                    urx = width + llx;
                                }

                                using (Stream inputPdfStream = new FileStream(savepath, FileMode.Create))
                                {
                                    var reader = new PdfReader(unsignData);
                                    var stamper = new PdfStamper(reader, inputPdfStream);
                                    if (thongtinky == null || signerFlow.LoaiKy == 3) // logo only
                                    {
                                        var pdfContentByte = stamper.GetOverContent(objPosition.SignPage);

                                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(_imgBytes);
                                        image.SetAbsolutePosition(llx, lly); // x,y
                                        image.ScaleAbsoluteWidth(width); // w
                                        image.ScaleAbsoluteHeight(height); // h
                                        pdfContentByte.AddImage(image);
                                    }
                                    else // text only
                                    {
                                        string fontFile = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/Font/times.ttf");
                                        var bf = BaseFont.CreateFont(fontFile, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                                        var tf = new TextField(stamper.Writer, new Rectangle(llx, lly, urx, ury), "newTextField")
                                        {
                                            BorderColor = BaseColor.LIGHT_GRAY,
                                            BorderStyle = PdfBorderDictionary.STYLE_DASHED,
                                            BorderWidth = 0,
                                            DefaultText = "This is a new text field.",
                                            Font = bf,
                                            FontSize = 10,
                                            TextColor = BaseColor.BLUE,
                                            //MaxCharacterLength = thongtinky.Length,
                                            Options = TextField.READ_ONLY | TextField.MULTILINE,
                                            Text = thongtinky
                                        };
                                        var pdfContentByte = stamper.GetOverContent(objPosition.SignPage);
                                        //Get the raw form field
                                        var FF = tf.GetTextField();
                                        //Set the background color
                                        FF.MKBackgroundColor = BaseColor.LIGHT_GRAY;
                                        stamper.AddAnnotation(FF, objPosition.SignPage);
                                    }
                                    stamper.Close();
                                }
                            }
                            await _smartCAFunction.UpdateSignedFile(item.Id, Convert.ToBase64String(System.IO.File.ReadAllBytes(savepath)));
                        }
                    }
                    else return -7;
                }
                return 11;
            }
            else return -7;
        }

        private string GetFullPathFile_ChuaDongMoc(string originalPath)
        {
            // Lấy tên file không có phần mở rộng
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(originalPath);

            // Lấy phần mở rộng (ví dụ .pdf)
            string ext = Path.GetExtension(originalPath);

            // Lấy thư mục chứa file
            string directory = Path.GetDirectoryName(originalPath);

            // Ghép lại tên mới
            string newFileName = $"{fileNameWithoutExt}_chuadongmoc{ext}";

            string newPath = Path.Combine(directory, newFileName);

            return newPath;
        }

        private void SaveFileChuaDongMoc(
            SignerFrontEndViewDto nguoiKyTiepTheo,
            string savedOriginalPath,
            byte[] fileSigned,
            int module)
        {
            var userNameDongMoc = new[] { "moccongdoan", "cnnb", "mocdang", "mocdoan" };

            if (nguoiKyTiepTheo != null &&
                nguoiKyTiepTheo.LoaiKy == 0 && // loại chỉ hiển thị logo
                userNameDongMoc.Contains(nguoiKyTiepTheo.UserName) && // thuộc mảng
                (module == 1 || module == 2)) // áp dụng cho cv đi + cv ký số
            {
                string savepath_chuadongmoc = GetFullPathFile_ChuaDongMoc(savedOriginalPath);

                System.IO.File.WriteAllBytes(savepath_chuadongmoc, fileSigned);
            }
        }

        private string GetUrlPathQuanLyChamCong(int Module)
        {
            string url = "";
            switch (Module) 
            { 
                case SystemConstants.VanBanKiSo.Module.QUAN_LY_CHAM_CONG:
                    url = SystemConstants.VanBanKiSo.FILE_DOWNLOADED.PHIEU_DANH_GIA_URL;
                    break;
                case SystemConstants.VanBanKiSo.Module.TONG_HOP_CHAM_CONG:
                    url = SystemConstants.VanBanKiSo.FILE_DOWNLOADED.BANG_TONG_HOP_URL;
                    break;
                default:
                    break;
            }
            return url;
        }

        // đang bỏ
        public async Task<int> SignNoSmartCAPDFCongVan_Old(SignerFrontEndViewDto signerEntryDto, IEnumerable<FilePDFEntryDto> fileList, int Module, int RefId, SignEntryDto SignTable, Core_UserDto signerFlow, string ButPhe)
        {
            UserModel userLogin = await _smartCAFunction.GetUserSmartCA(signerEntryDto.UserName);
            if (userLogin == null)
            {
                return -7;
            }
            if (fileList.Count() > 0)
            {
                string ngayky = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + " (" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ")";
                string imgPath = System.Web.Hosting.HostingEnvironment.MapPath("~/VNPT/SignImg/");
                foreach (var item in fileList)
                {
                    string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CongVanFile/");
                    sPath = Path.Combine(sPath, item.CreatedDate.ToString("yyyy"));
                    sPath = Path.Combine(sPath, item.CreatedDate.ToString("MM"));
                    string savepath = Path.Combine(sPath, item.FilePath);
                    if (signerEntryDto.ObjPosition != null)
                    {
                        List<pdfInfo> pdf = JsonConvert.DeserializeObject<List<pdfInfo>>(signerEntryDto.ObjPosition);
                        var temp = pdf.Where(x => x.FileName == item.FileName);
                        if (temp.Count() > 0)
                        {
                            byte[] _imgBytes = null;
                            string thongtinky = "Ký bởi: " + signerFlow.FullName + "\nChức vụ: " + signerFlow.CHUCVU + "\nNgày ký: " + ngayky;
                            if (signerFlow.LoaiKy == 0 || signerFlow.LoaiKy == 1) // kí chỉ có logo
                            {
                                imgPath += signerEntryDto.UserName.ToLower() + @"\";
                                _imgBytes = System.IO.File.ReadAllBytes(Path.Combine(imgPath, userLogin.ImgPath));
                            }
                            else if (signerFlow.LoaiKy == 2) // ký chỉ có text
                            {
                                string noidungbutphe = "";
                                if (signerFlow.NoiDungButPhe != null)
                                {
                                    string[] lines = signerFlow.NoiDungButPhe.Split(new string[] { "<br/>" },StringSplitOptions.None);
                                    foreach (var text in lines)
                                    {
                                        noidungbutphe += "\n";
                                        noidungbutphe += text;
                                    }
                                }
                                else
                                {
                                    if (ButPhe != null)
                                    {
                                        string[] lines = ButPhe.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                                        foreach (var text in lines)
                                        {
                                            noidungbutphe += "\n";
                                            noidungbutphe += text;
                                        }
                                    }
                                }
                                thongtinky = noidungbutphe == "" ? thongtinky : (thongtinky + "\nBút phê chỉ đạo: " + noidungbutphe);
                            }
                            else if (signerFlow.LoaiKy == 3) // ký mộc
                            {
                                _imgBytes = Convert.FromBase64String(signerFlow.FILEANH.Split(',')[1]);
                            }
                            foreach (var objPosition in temp)
                            {
                                byte[] unsignData = null;
                                try
                                {
                                    unsignData = System.IO.File.ReadAllBytes(savepath);
                                }
                                catch
                                {
                                    return -1;
                                }
                                //format recSmartCA (llx,lly,urx,ury)
                                //format recNormal (x,y,w,h) => (llx,lly,w = urx - llx,h = ury - lly)
                                string LeftLlxRec = objPosition.Rectangle.Split(',')[0];
                                string BottomLlyRec = objPosition.Rectangle.Split(',')[1];
                                string RightUrxRec = objPosition.Rectangle.Split(',')[2];
                                string TopUryRec = objPosition.Rectangle.Split(',')[3];
                                int llx = 0;
                                int lly = 0;
                                int urx = 0;
                                int ury = 0;
                                Int32.TryParse(LeftLlxRec, out llx);
                                Int32.TryParse(BottomLlyRec, out lly);
                                Int32.TryParse(RightUrxRec, out urx);
                                Int32.TryParse(TopUryRec, out ury);
                                int width = urx - llx;
                                int height = ury - lly;
                                using (Stream inputPdfStream = new FileStream(savepath, FileMode.Create))
                                {
                                    var reader = new PdfReader(unsignData);
                                    var stamper = new PdfStamper(reader, inputPdfStream);
                                    if(signerFlow.LoaiKy != 2) // khác với chỉ có text
                                    {
                                        var pdfContentByte = stamper.GetOverContent(objPosition.SignPage);

                                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(_imgBytes);
                                        image.SetAbsolutePosition(llx, lly); // x,y
                                        image.ScaleAbsoluteWidth(width); // w
                                        image.ScaleAbsoluteHeight(height); // h
                                        pdfContentByte.AddImage(image);
                                    }
                                    else
                                    {
                                        string fontFile = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/Font/times.ttf");
                                        var bf = BaseFont.CreateFont(fontFile, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                                        var tf = new TextField(stamper.Writer, new Rectangle(llx, lly - 70, urx, ury), "newTextField")
                                        {
                                            BorderColor = BaseColor.LIGHT_GRAY,
                                            BorderStyle = PdfBorderDictionary.STYLE_DASHED,
                                            DefaultText = "This is a new text field.",
                                            Font = bf,
                                            FontSize = 10,
                                            TextColor = BaseColor.BLUE,
                                            //MaxCharacterLength = thongtinky.Length,
                                            Options = TextField.READ_ONLY | TextField.MULTILINE,
                                            Text = thongtinky
                                        };
                                        var pdfContentByte = stamper.GetOverContent(objPosition.SignPage);
                                        //Get the raw form field
                                        var FF = tf.GetTextField();
                                        //Set the background color
                                        FF.MKBackgroundColor = BaseColor.LIGHT_GRAY;
                                        stamper.AddAnnotation(FF, objPosition.SignPage);
                                    }
                                    stamper.Close();
                                }
                            }
                        }
                        else return -7;
                    }
                    else return -7;
                    await _smartCAFunction.UpdateSignedFile(item.Id, Convert.ToBase64String(System.IO.File.ReadAllBytes(savepath)));
                }
                vc.ChangeSignerFlowVB(RefId, Module, 1, null, "Đã ký duyệt", signerEntryDto.UserName, signerFlow.isChecked, SignTable.Status);
                var nguoikytieptheo = await _smartCAFunction.FindSigner(SignTable.Id, SignTable.Status + 1);
                if (nguoikytieptheo != null)
                {
                    await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, SignTable.Status + 1);
                    vc.CapNhatNguoiDuyetVB(RefId, nguoikytieptheo.UserName, SignTable.Id, Module);
                    return 11;
                }
                else
                {
                    await _smartCAFunction.UpdateStatusSignTable(SignTable.Id, -99);
                    vc.CapNhatNguoiDuyetVB(RefId, null, SignTable.Id, Module);
                    await _smartCAFunction.CallBackAPI(1, SignTable, fileList);
                    return 1;
                }
            }
            else return -7;
        }
    }
}