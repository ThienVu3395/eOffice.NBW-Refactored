using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Web;
using Dapper;

namespace Clone_KySoDienTu.Report
{
    public partial class ReportVanBanKiSoViewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadReport();
        }

        public void LoadReport()
        {
            var reportParam = (dynamic)HttpContext.Current.Session["ReportParam"];
            ReportDocument rd = new ReportDocument();
            string path = Server.MapPath("~") + "Report//Rpt//" + reportParam.ReportFileName;
            var datasource = reportParam.DataSource;
            rd.Load(path);
            rd.SetDataSource(datasource);
            CrystalReportViewer2.ReportSource = rd;
            CrystalReportViewer2.RefreshReport();
            string savePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
            savePath = Path.Combine(savePath, reportParam.ReportTitle + ".pdf");
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, savePath);
            MoveReport(reportParam.ReportTitle + ".pdf", reportParam.IDCR, reportParam.Loai);
        }

        public void MoveReport(string fileName,Int64 IDCR, int Loai)
        {
            DateTime ngaytao = DateTime.Now;
            string _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
            string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/ReportFile/");
            string oldPath = Path.Combine(sPath, fileName);
            FileInfo fs = new FileInfo(oldPath);
            int sizefile = (int)fs.Length;
            string savePath = Path.Combine(sPath, ngaytao.Year.ToString());
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            savePath = Path.Combine(savePath, ngaytao.ToString("MM"));
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            Guid obj = Guid.NewGuid();
            string newName = obj.ToString() + "." + "pdf";
            string newPath = Path.Combine(savePath, newName);
            fs.MoveTo(newPath);
            using (IDbConnection db = new SqlConnection(_cnn))
            {
                if (db.State == System.Data.ConnectionState.Closed)
                    db.Open();
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@VANBANID", IDCR);
                dynamicParameters.Add("@TENFILE", newName);
                dynamicParameters.Add("@MOTA", fileName);
                dynamicParameters.Add("@LOAIFILE", "pdf");
                dynamicParameters.Add("@SIZEFILE", sizefile);
                dynamicParameters.Add("@IsCRFile", 1);
                dynamicParameters.Add("@Module", Loai);
                dynamicParameters.Add("@FileID", null, DbType.Int32, ParameterDirection.Output);
                db.Execute("VanBanKiSo_AddFile", dynamicParameters, null, null, CommandType.StoredProcedure);
            }
        }
    }
}