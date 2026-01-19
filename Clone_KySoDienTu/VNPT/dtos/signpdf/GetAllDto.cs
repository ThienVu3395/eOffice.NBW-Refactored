using SmartCAAPI.Dtos.usermanager;
using System;
using System.Collections.Generic;
using System.Xml;

namespace SmartCAAPI.Dtos.signpdf
{
    #region for FrontEnd
    public class SignerFrontEndViewDto
    {
        public string Id { get; set; }
        public string RefId { get; set; }
        public string TranID { get; set; }
        public string UserName { get; set; }
        public string ObjPosition { get; set; }
        public int Status { get; set; }
        public string FullName { get; set; }
        public string BoPhan { get; set; }
        public string ChucVu { get; set; }
        public int LoaiKy { get; set; }
        public string NoiDungButPhe { get; set; }
        public string SoVanBan { get; set; }
        public string NgayVanBan { get; set; }
        public bool ButPhe { get; set; }
        public string FILEANH { get; set; }
    }
    public class GetAllFrontEndViewDto
    {
        public SignEntryDto Item { get; set; }
        public IEnumerable<GetSignResponse> ListSigner { get; set; }
        public IEnumerable<FilePDFEntryDto> ListFile { get; set; }
    }
    #endregion
    public class SignerEntryDto
    {
        public string Id { get; set; }
        public string RefId { get; set; }
        public string TranID { get; set; }
        public string UserName { get; set; }
        public string ObjPosition { get; set; }
        public int Status { get; set; }
    }
    public class SignEntryDto
    {
        public string Id { get; set; }
        public int Module { get; set; }
        public int Status { get; set; }
        public int RefId { get; set; }
        public string LinkAPICallback { get; set; }
        public int ChildModule { get; set; }
    }
    public class FilePDFEntryDto
    {
        public string Id { get; set; }
        public string RefId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string UnsignData { get; set; }
        public string SignedData { get; set; }
        public string HashData { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class GetAllDto
    {
        public SignEntryDto Item { get; set; }
        public IEnumerable<SignerEntryDto> ListSigner { get; set; }
        public IEnumerable<FilePDFEntryDto> ListFile { get; set; }
    }
    public class CreateAllDto
    {
        public CreateSignDto Item { get; set; }
        public List<CreateSignerDto> ListSigner { get; set; }
        public List<CreateFileDto> ListFile { get; set; }
    }
    public class CreateSignDto
    {
        public int Module { get; set; }
        public int RefId { get; set; }
        public int Status { get; private set; } = 0;
        public string LinkAPICallback { get; set; }
    }
    public class CreateSignerDto
    {
        public string UserName { get; set; }
        public int Status { get; set; }
    }
    public class CreateFileDto
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string UnsignData { get; set; }
        public string HashData { get; set; }
        public DateTime CreatedDated { get; set; }
    }
    public class UpdateSignerDto
    {
        public string Id { get; set; }
        public string ObjPosition { get; set; }
    }
    public class pdfInfo
    {
        public string FileName { get; set; }
        public string Rectangle { get; set; }
        public int SignType { get; set; } = 0;
        public int SignPage { get; set; } = 1;
        public int TotalPage { get; set; }
    }
    public class MyXmlDsigExcC14NTransform : XmlDsigExcC14NTransform
    {
        public MyXmlDsigExcC14NTransform() { }

        public override void LoadInput(Object obj)
        {
            XmlElement root = ((XmlDocument)obj).DocumentElement;
            if (root.Name == "SignedInfo") root.RemoveAttribute("xml:id");
            base.LoadInput(obj);
        }
    }
}