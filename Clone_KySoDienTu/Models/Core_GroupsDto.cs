using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clone_KySoDienTu.Service.Dtos
{
    public class Core_GroupsDto
    {
        public int GroupId { get; set; }
        //public string GroupName { get; set; }
        //public string Account { get; set; }
        public string MAPHONG { get; set; }
        public string TENPHONG { get; set; }
        public List<Core_UserDto> Users { get; set; }
       
        
    }
    public class Core_GroupChucNangDto
    {
        public int GroupId { get; set; }
        public string MAPHONG { get; set; }
        public string TENPHONG { get; set; }
        public List<Core_DangBoUser> Users { get; set; }
        public int ActionType { get; set; }
    }
}



