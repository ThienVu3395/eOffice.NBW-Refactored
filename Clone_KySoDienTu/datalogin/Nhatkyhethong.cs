
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using Dapper;

namespace OAuthBNLE.DataLogin
{
    public static class Nhatkyhethong 
    {
        private static readonly string _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
        
        public static void insertLog(string username, string modulekey)
        {
            string Ip = GetIPAddress();
            string strPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
            string strUrl = HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
            using (IDbConnection db = new SqlConnection(_cnn))
            {
                if (db.State == System.Data.ConnectionState.Closed)
                    db.Open();
                DynamicParameters para = new DynamicParameters();
                para.Add("@UserName", username);
                para.Add("@ModuleKey", modulekey);
                para.Add("@Url", strUrl); 
                para.Add("@UserHostAddress", Ip);
            
                db.Execute("Core_AddSiteLog", para, null, null, System.Data.CommandType.StoredProcedure);
            }
        }

        static string GetIPAddress()
        {
            string IpClient = "";
            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;
            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);
            foreach (IPAddress IP in Host.AddressList)
            {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    IpClient = Convert.ToString(IP);
                }
            }
            return IpClient;
        }
    }
}