using LicenseCore;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using SteCode.Utilities;
namespace Clone_KySoDienTu.API
{
    public class OptionsConfig
    {
        private static SteLicenseHandler licenseHandler;
        private static SteLicenseEntity _lic;
        private const string _ivStr = "Steven@!@#4567890!";
        
        public static void CheckActive()
        {
            try
            {
                
                string fileActive = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/"), "license.lic");
                if (File.Exists(fileActive))
                {
                    licenseHandler = new SteLicenseHandler(_ivStr);
                    string strlic = File.ReadAllText(fileActive, Encoding.UTF8);
                    _lic = licenseHandler.ParseLicenseFromBASE64String(strlic);

                    string mesg = "";
                    if (!licenseHandler.CheckLicense(_lic, out mesg))
                    {
                        throw new NotImplementedException(mesg);
                    }
                    SteCode.Dapper.SteDapperSQL.Connectionstring = _lic.ConnectionString;
                    Utils._folderTemp = System.Web.Hosting.HostingEnvironment.MapPath("~/_Temp/");
                    Utils._sPathReports = System.Web.Hosting.HostingEnvironment.MapPath("~/Reports/");
                }
                else
                {
                    _lic = null;
                    throw new NotImplementedException("License invalid!");
                }
            }
            catch
            {
                _lic = null;
                throw new NotImplementedException("License invalid!");
            }
        }
    }
}