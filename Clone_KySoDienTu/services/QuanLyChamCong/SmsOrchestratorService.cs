using Clone_KySoDienTu.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VModel;
using static Clone_KySoDienTu.SystemConstants.VanBanKiSo;

namespace Clone_KySoDienTu.Service.QuanLyChamCong
{
    // Service gộp full logic gửi SMS + ghi log => CỰC GỌN – CỰC SẠCH – KHÔNG LẶP LOGIC - DỄ TÁI SỬ DỤNG NHIỀU NƠI
    public class SmsOrchestratorService
    {
        private readonly string _cnn;

        private VCode.QuanLyChamCongModule qlcc;

        private readonly SmsService _smsService;

        public SmsOrchestratorService()
        {
            _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
            qlcc = new VCode.QuanLyChamCongModule(_cnn);
            _smsService = new SmsService();
        }

        public async Task<List<smsTriAnhResponse>> SendAndLogAsync(SmsLogRequest request)
        {
            // 1. Lấy danh sách user
            var listUsers = qlcc.LayDanhSachUserNhanTinSMS(request);

            List<smsTriAnhResponse> results = new List<smsTriAnhResponse>();

            if (!listUsers.Any())
                return results;

            // 2. Gửi SMS
            string message = GetMessageToSendSMS(request.Module);

            if (message == null) return results;

            results = await _smsService.SendSMSTriAnhAsync(listUsers, message);

            // 3. Ghi log
            foreach (var res in results)
            {
                var log = new SmsLog
                {
                    RefId = request.RefId.ToString(),
                    Module = request.Module,
                    Mobile = res.phoneNumber,
                    UserName = res.userName,
                    Message = message,
                    Status = (int)res.code,
                    ErrorMessage = res.status.Contains("FAIL") ? res.status : null,
                    SentDate = res.timeStamp
                };

                qlcc.InsertSMSLog(log);
            }

            return results;
        }

        private string GetMessageToSendSMS(int module)
        {
            string message = null;
            switch (module) 
            { 
                case Module.QUAN_LY_CHAM_CONG:
                    message = DUNG_CHUNG.MESSAGE_PHIEU_DANH_GIA_CAN_KY;
                    break;
                case Module.TONG_HOP_CHAM_CONG:
                    message = DUNG_CHUNG.MESSAGE_BANG_TONG_HOP_CAN_KY;
                    break;
                default:
                    break;
            }
            return message;
        }
    }
}