namespace Clone_KySoDienTu.Models.Dtos.QuanLyChamCong
{
    public sealed class NotificationCountsQLCCDto
    {
        // Phiếu đánh giá
        public int DraftByMePDG { get; set; } = 0;
        public int WaitingMyApprovalPDG { get; set; } = 0;
        public int WaitingRemovedPDG { get; set; } = 0;
        public int CanceledPDG { get; set; } = 0; // hủy ký
        public int UnapprovedPDG { get; set; } = 0; // gỡ duyệt

        // Bảng tổng hợp
        public int DraftByMeBTH { get; set; } = 0;
        public int WaitingMyApprovalBTH { get; set; } = 0;
        public int WaitingRemovedBTH { get; set; } = 0;
        public int CanceledBTH { get; set; } = 0; // hủy ký
        public int UnapprovedBTH { get; set; } = 0; // gỡ duyệt

        // Total
        public int TotalNotifications { get; set; } = 0;
    }
}