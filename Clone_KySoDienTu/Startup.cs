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
        }
    }
}