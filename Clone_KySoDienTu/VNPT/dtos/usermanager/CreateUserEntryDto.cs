
namespace SmartCAAPI.Dtos.usermanager
{
    public class CreateUserEntryDto
    {
        public string UserName { get; set; }
        public string UserSmartCA { get; set; }
        public string PassSmartCA { get; set; }
        public string ImgBase64 { get; set; }
        public string imgBaseUrl { get; set; }
        public int LoaiKy { get; set; }
        public bool ButPhe { get; set; }
        public string NoiDungButPhe { get; set; }
        public string SoVanBan { get; set; }
        public string NgayVanBan { get; set; }
        public string FILEANH { get; set; }
        public bool Khoa { get; set; } = false;
        public bool DaCoChuKySo { get; set; } = false;
    }

    public class GetSignResponse
    {
        public string Id { get; set; }
        public int Status { get; set; }
        public string FullName { get; set; }
        public string BoPhan { get; set; }
        public string ChucVu { get; set; }
        public int LoaiKy { get; set; }
        public bool ButPhe { get; set; }
        public string NoiDungButPhe { get; set; }
        public string SoVanBan { get; set; }
        public string NgayVanBan { get; set; }
        public string ImgBase64 { get; set; }
        public string ObjPosition { get; set; }
        public string UserName { get; set; }
    }

    public class GetUserEntryDto
    {
        public string UserName { get; set; }
    }
}



