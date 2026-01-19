using System;
using System.Collections.Generic;
using System.Text;

namespace VNPTdtos
{
    public class TransactionDto
    {
        public int Module { get; set; }
        public string TranID { get; set; }
        public int RefID { get; set; }
        public int AutoRepeat { get; set; }
        public int IsSuccess { get; set; }
        public string UserName { get; set; }
        public string TenFile { get; set; }
    }

    public class FileModuleDto
    {
        public string MOTA { get; set; }
        public string TENFILE { get; set; }
    }

    public class CallBackFileDto
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string UnsignData { get; set; }
        public string SignedData { get; set; }
        public string HashData { get; set; }
    }

    public class CallBackDto
    {
        public int Module { get; set; }
        public int Status { get; set; }
        public int RefId { get; set; }
        public string ID { get; set; }
        public List<CallBackFileDto> ListFile { get; set; }
    }
}
