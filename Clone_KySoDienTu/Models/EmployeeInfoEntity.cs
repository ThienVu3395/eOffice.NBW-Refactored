using System;

namespace Clone_KySoDienTu.Models
{
    public class EmployeeInfoEntity
    {
        public int ID { get; set; }
        public string FileNotation { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime DueDate { get; set; }
        public string OrganName { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
    }
}