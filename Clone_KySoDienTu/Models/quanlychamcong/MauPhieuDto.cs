namespace Clone_KySoDienTu.Models.Dtos.QuanLyChamCong
{
    public class MauPhieuDto
    {
        public int Id { get; set; }
        public string SimpleCode { get; set; }
        public string Title { get; set; }
        public string AppliedFor { get; set; }
        public string DepartmentFor { get; set; }
        public int Score { get; set; }
        public int TieuChiCount { get; set; }
        public string DisplayText { get; set; }
    }
}