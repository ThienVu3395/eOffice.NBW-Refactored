namespace SmartCAAPI.Dtos.signpdf
{
    public class UserInfo
    {
        public string UserSmartCA { get; set; }
        public string PassSmartCA { get; set; }
        public string ImgBase64 { get; set; }
        public string ImgPath { get; set; }
        public bool Khoa { get; set; }
        public bool DaCoChuKySo { get; set; }
    }
    public class UserModel
    {
        public string AccessToken { get; set; }
        public string Credential { get; set; }
        public string CertBase64 { get; set; }
        public string ImgBase64 { get; set; }
        public string ImgPath { get; set; }
        public bool Khoa { get; set; }
        public bool DaCoChuKySo { get; set; }
    }
}
