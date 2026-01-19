namespace Clone_KySoDienTu.Models.Dtos.QuanLyChamCong
{
    public class TieuChiDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }

        // Có mẫu để trống điểm parent => nên cho phép null
        public int? Score { get; set; }

        // Parent ở root sẽ null
        public int? ParentId { get; set; }

        public int MauPhieuId { get; set; }
        public int ViewOrder { get; set; }
        public int Level { get; set; }

        public bool HasChildren { get; set; }
        public bool IsParent { get; set; }          // NEW

        public string ParentCode { get; set; }
        public string FullTitle { get; set; }

        public int SumChildScore { get; set; }      // đã ISNULL trong SP nên int là đủ
        public int ScoreDisplay { get; set; }       // dùng để hiển thị “Điểm chuẩn”
    }

}