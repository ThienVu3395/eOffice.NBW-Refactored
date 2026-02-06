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
using System.IO;
using System.Threading;
using System;
using static VModel.WFModel;
using iTextSharp.text.pdf;

namespace Clone_KySoDienTu.VNPT.Services
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

        public async Task<int> SignSmartCaPDF(
            SignerFrontEndViewDto signerEntryDto, 
            IEnumerable<FilePDFEntryDto> fileList, 
            int Module, int RefId, 
            SignEntryDto SignTable, 
            Core_UserDto signerFlow)
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

        public async Task<int> SignNoSmartCaPDF(
            SignerFrontEndViewDto signerEntryDto,
            IEnumerable<FilePDFEntryDto> fileList,
            int Module,
            int RefId,
            SignEntryDto SignTable,
            Core_UserDto signerFlow,
            string ButPhe)
        {
            UserModel userLogin = await _smartCAFunction.GetUserSmartCA(signerEntryDto.UserName);
            if (userLogin == null)
            {
                return -7;
            }
            if (fileList.Count() > 0)
            {
                string imgPath = System.Web.Hosting.HostingEnvironment.MapPath("~/VNPT/SignImg/");
                foreach (var item in fileList)
                {
                    string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
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
                            imgPath += signerEntryDto.UserName.ToLower() + @"\";
                            _imgBytes = System.IO.File.ReadAllBytes(Path.Combine(imgPath, userLogin.ImgPath));
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

                                using (Stream inputPdfStream = new FileStream(savepath, FileMode.Create))
                                {
                                    var reader = new PdfReader(unsignData);
                                    var stamper = new PdfStamper(reader, inputPdfStream);
                                    var pdfContentByte = stamper.GetOverContent(objPosition.SignPage);

                                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(_imgBytes);
                                    image.SetAbsolutePosition(llx, lly); // x,y
                                    image.ScaleAbsoluteWidth(width); // w
                                    image.ScaleAbsoluteHeight(height); // h
                                    pdfContentByte.AddImage(image);

                                    stamper.Close();
                                }
                            }
                            await _smartCAFunction.UpdateSignedFile(item.Id, Convert.ToBase64String(System.IO.File.ReadAllBytes(savepath)));
                        }
                    }
                    else return -7;
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