using Newtonsoft.Json;
using Clone_KySoDienTu.Models;
using System.Net.Http;
using System.Net;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using VModel;

namespace Clone_KySoDienTu.Service.QuanLyChamCong
{
    public class SmsService : ISmsService
    {
        private readonly smsTriAnh SMSTriAnhRequest = new smsTriAnh();

        public async Task<List<smsTriAnhResponse>> SendSMSTriAnhAsync(
            List<WFModel.Core_UserDto> listUser,
            string message)
        {
            var results = new List<smsTriAnhResponse>();

            if (listUser == null || !listUser.Any())
                return results;

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            };

            using (var client = new HttpClient(handler))
            {
                client.Timeout = TimeSpan.FromSeconds(15);
                client.BaseAddress = new Uri(SMSTriAnhRequest.UrlAPI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + SMSTriAnhRequest.Token);

                // Gửi song song các số điện thoại (Parallel)
                var tasks = listUser.Select(async x =>
                {
                    var result = new smsTriAnhResponse
                    {
                        phoneNumber = x.Mobile,
                        userName = x.UserName
                    };

                    try
                    {
                        var smsRequest = new smsTriAnh
                        {
                            UrlAPI = SMSTriAnhRequest.UrlAPI,
                            Token = SMSTriAnhRequest.Token,
                            Src = SMSTriAnhRequest.Src,
                            useUnicode = SMSTriAnhRequest.useUnicode,
                            Des = x.Mobile,
                            Message = message
                        };

                        var json = JsonConvert.SerializeObject(smsRequest);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync(client.BaseAddress, content);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonData = await response.Content.ReadAsStringAsync();
                            var parsed = JsonConvert.DeserializeObject<smsTriAnhResponse>(jsonData);

                            // Map kết quả nhưng giữ nguyên PhoneNumber
                            result.status = parsed?.status ?? "UNKNOWN";
                            result.timeStamp = parsed?.timeStamp;
                            result.code = result.status.Contains("SUCCESS")
                                ? smsTriAnhStatus.GUI_THANH_CONG
                                : smsTriAnhStatus.GUI_THAT_BAI;
                        }
                        else
                        {
                            result.status = "FAIL";
                            result.code = smsTriAnhStatus.GUI_THAT_BAI;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.status = $"FAIL: {ex.Message}";
                        result.code = smsTriAnhStatus.GUI_THAT_BAI;
                    }

                    return result;
                });

                // Đợi tất cả hoàn thành
                var responses = await Task.WhenAll(tasks);
                results.AddRange(responses);
            }

            return results;
        }
    }
}