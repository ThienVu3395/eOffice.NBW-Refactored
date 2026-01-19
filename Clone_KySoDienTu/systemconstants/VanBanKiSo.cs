namespace Clone_KySoDienTu.SystemConstants
{
    public class VanBanKiSo
    {
        public class Module
        {
            public const int QUAN_LY_CHAM_CONG = 30;
            public const int TONG_HOP_CHAM_CONG = 31;
            public const int DON_NGHI_PHEP_MAU_1_NHAN_VIEN = 6;
            public const int DON_NGHI_PHEP_MAU_2_TRUONG_PHONG = 7;
            public const int DON_NGHI_PHEP_MAU_3_KHONG_LUONG = 8;
            public const int DON_NGHI_PHEP_MAU_4_DI_NUOC_NGOAI = 9;
            public const int DON_GIAI_TRINH_MAU_5_NHAN_VIEN = 28;
            public const int DON_GIAI_TRINH_MAU_6_TRUONG_PHONG = 29;
        }
        public class FILE_DOWNLOADED
        {
            public const string NAME_FORMAT = "MeetingSummaries";
            public const string TEMP_FOLDER_URL = "~/Uploadtemp/";

            public const string WORD_EXTENSION = "docx";
            public const string EXCEL_EXTENSION = "xlsx";
            public const string PDF_EXTENSION = "pdf";

            public const string WORD_FORMAT = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            public const string EXCEL_FORMAT = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            public const string PDF_FORMAT = "application/pdf";

            public const string PHIEU_DANH_GIA_URL = "~/ExportsQLCC/";
            public const string PHIEU_DANH_GIA_FOLDER_NAME = "ExportsQLCC";
            public const string PHIEU_DANH_GIA_BACKUP_FOLDER_NAME = "Backup";

            public const string BANG_TONG_HOP_URL = "~/ExportsTHCC/";
            public const string BANG_TONG_HOP_FOLDER_NAME = "ExportsTHCC";
            public const string BANG_TONG_HOP_BACKUP_FOLDER_NAME = "Backup";

            public const string DON_NGHI_PHEP_URL = "~/Report/ReportFile/";
            public const string DON_NGHI_PHEP_FOLDER_NAME = "Report/ReportFile";
        }
        public class TEN_THAO_TAC
        {
            public const string HUY_KY = "HUY_KY"; // Hủy ký/xác nhận hủy
            public const string GO_DUYET = "GO_DUYET";
            public const string YEU_CAU_HUY = "YEU_CAU_HUY";
            public const string TU_CHOI_HUY = "TU_CHOI_HUY";
            public const string GO_YEU_CAU_HUY = "GO_YEU_CAU_HUY";
        }
        public class MA_THAO_TAC
        {
            public const int HUY_KY = 1;
            public const int GO_DUYET = 7;
            public const int YEU_CAU_HUY = 5;
            public const int TU_CHOI_HUY = 6;
            public const int GO_YEU_CAU_HUY = 8;
        }
        public class DUNG_CHUNG
        {
            public const string MESSAGE_PHIEU_DANH_GIA_CAN_KY = "Có phiếu đánh giá năng suất cần ký";
            public const string MESSAGE_BANG_TONG_HOP_CAN_KY = "Có bảng đánh giá tổng hợp kết quả thực hiện nhiệm vụ của người lao động cần ký";
        }
    }
}