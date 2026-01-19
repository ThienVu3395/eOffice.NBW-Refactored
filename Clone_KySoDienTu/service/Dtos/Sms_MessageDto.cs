using System;
using System.Collections.Generic; 

namespace Clone_KySoDienTu.Service.Dtos
{
    public class Sms_MessageDto
    {
        public int Total { get; set; }
        public int MessageId { get; set; }
        public string SenderName { get; set; }
        public string Subject { get; set; }
        public int IsImportant { get; set; }
        public bool IsDeleted { get; set; }
        public int Box { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public bool IsHtmlView { get; set; }
        public int MessageRead { get; set; }
    }

    public class Sms_AddMessage
    {
        public int MessageId { get; set; }
        public int ParentID { get; set; }
        public int TrangThai { get; set; }
        public string RecipientList { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool RequiredMessageNotify { get; set; }
        public bool IsNotifyMessage { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public string SenderName { get; set; }
        public bool IsHtmlView { get; set; }
        public int MessageRead { get; set; }
        public bool IsImportant { get; set; }
        public List<MFile> FileDinhKem { get; set; }
        public List<MessageUser> Users { get; set; }
        public int VAITRO { get; set; }
    }

    public class MessageUser
    {
        public int MessageId { get; set; }
        public string RecipientName { get; set; }
        public int VAITRO { get; set; }
        public int MessageRead { get; set; }
        public int IsDeleted { get; set; }
        public bool Box { get; set; }
        public bool IsImportant { get; set; }
        public Nullable<System.DateTime> ReadDate { get; set; }
        public string LabelList { get; set; }
        public bool IsStar { get; set; }
    }

    public class MFile
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int FileSize { get; set; }
        public string FileType { get; set; }
    }
}