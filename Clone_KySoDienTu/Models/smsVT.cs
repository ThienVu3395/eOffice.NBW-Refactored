namespace Clone_KySoDienTu.Models
{
    public class smsVT
    {
        public string User {get ;set ;} = "smsbrand_cnnhabe";
        public string Password{ get; set; } = "123456aA@"; 
        public string CPCode{ get; set; } = "CNNHABE";
        public string RequestID{ get; set; } ="1"; 
        public string UserID { get; set; }
        public string ServiceID { get; set; } = "CN_Nha Be"; 
        public string CommandCode { get; set; } = "bulksms"; 
        public string Content { get; set; }
        public string ContentType { get; set; } = "1";

    }

    public class smsTriAnh
    {
        public string UrlAPI { get; set; } = "https://capnuocnhabe5-local.tcrm.vn/apisupport/send/sms";
        //public string Token { get; set; } = "d0oj3vQUAwSrO1K5Sb_doNX5FmVyP78w";
        public string Token { get; set; } = "whuSxBd9xM5tpHLLl-iMaNHyp1YKGHp4";
        public string Des { get; set; }
        public string Src { get; set; } = "CN_NHABE";
        public string Message { get; set; }
        public int useUnicode { get; set; } = 1;
    }

    public enum smsTriAnhStatus
    {
        CHUA_GUI = -1,
        GUI_THAT_BAI = 0,
        GUI_THANH_CONG = 1,
    }

    public class smsTriAnhResponse
    {
        public string phoneNumber { get; set; }
        public string userName { get; set; }
        public string status { get; set; }
        public string timeStamp { get; set; }
        public smsTriAnhStatus code { get; set; }
    }

    public class DataObject
    {
        public string status { get; set; }
        public string timeStamp { get; set; }
    }
}