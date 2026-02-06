using Clone_KySoDienTu.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using VModel;

namespace Clone_KySoDienTu.Service.QuanLyChamCong
{
    public interface ISmsService
    {
        Task<List<smsTriAnhResponse>> SendSMSTriAnhAsync(
            List<WFModel.Core_UserDto> listUser,
            string message);
    }
}
