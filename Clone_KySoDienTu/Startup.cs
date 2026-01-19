using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;
using System.Timers;
using Clone_KySoDienTu.MyHub;
using Clone_KySoDienTu.Controllers.API;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Office.Interop.Word;
using Microsoft.Owin.Security.Twitter.Messages;
using GatewayServiceTest;
using VNPTdtos;
using VModel;
using SteProject;
using Newtonsoft.Json;

using SmartCAAPI.Dtos.signpdf;

[assembly: OwinStartup(typeof(Clone_KySoDienTu.Startup))]

namespace Clone_KySoDienTu
{
    public partial class Startup
    {
        private static System.Timers.Timer aTimer;
        private static APICal_SuKienController sms = new APICal_SuKienController();

        private readonly string _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
        //private Models.smsVT sms = new Models.smsVT();
        private APIPUGController PUG = new APIPUGController();
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.Map("/signalr", map =>
            {
                var hubConfiguration = new HubConfiguration
                {
                    EnableDetailedErrors = true,
                    EnableJSONP = true,
                    EnableJavaScriptProxies = true
                };
                map.RunSignalR(hubConfiguration);
            });
            aTimer = new System.Timers.Timer(600000); // 60000 : 1p ; 600000 : 10p
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                CallSMSEvent();
            }
            catch
            { 

            }
        }
        private bool getUserDt(string nguoithongbao, string tinnhan, out string log)
        {
            log = "";
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@DSGuiTin", nguoithongbao);
                    var groups = db.Query<Service.Dtos.Core_UserDto>("SMS_GetUserDT", parameters, null, true, null, System.Data.CommandType.StoredProcedure);
                    if (groups.Count() > 0)
                    {
                        foreach (var item in groups)
                        {
                            if (sms.SendSMS(item.Mobile, tinnhan).Result.status.Contains("SUCCESS"))
                            {
                                log = log + "1,";
                            }
                            else
                                log = log + "0,";
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        private void CallSMSEvent()
        {
            if (DateTime.Now.Hour >= 5 && DateTime.Now.Hour <= 18)
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var groups = db.Query<Service.Dtos.TinNhanHenGio>("SMS_GetHengio_New", null, null, true, null, System.Data.CommandType.StoredProcedure);
                    if (groups.Count() > 0)
                    {
                        foreach (var item in groups)
                        {
                            if (item.NgayGui < DateTime.Now)
                            {
                                if (item.Type == 0)
                                {
                                    string logsms;
                                    if (getUserDt(item.ThongBao, item.NoiDung, out logsms))
                                    {
                                        var parameters = new DynamicParameters();
                                        parameters.Add("@SuKienId", item.SuKienId);
                                        parameters.Add("@NgayTao", item.NgayTao);
                                        parameters.Add("@NgayGui", item.NgayGui);
                                        parameters.Add("@ThongBao", item.ThongBao);
                                        parameters.Add("@TrangThai", logsms);
                                        db.Execute("SMS_HengioSaveLog", parameters, null, null, System.Data.CommandType.StoredProcedure);
                                    }
                                }
                                else
                                {
                                    SteProject.CommonModel par = new SteProject.CommonModel();
                                    par.valint1 = item.SuKienId;
                                    par.valint2 = 1;
                                    par.valstring1 = item.ThongBao;
                                    par.valstring2 = item.NoiDung;
                                    if (item.DaGui == 0 && item.SMSHenGio)
                                    {
                                        string logsms;
                                        if (getUserDt(item.ThongBao, item.NoiDung, out logsms))
                                        {

                                            PUG.UpdateSMS(par);
                                        }
                                    }
                                    if (item.DaGui < 2 && item.SMSThongBao)
                                    {
                                        PUG.CallSignal(par);
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }
    }
}