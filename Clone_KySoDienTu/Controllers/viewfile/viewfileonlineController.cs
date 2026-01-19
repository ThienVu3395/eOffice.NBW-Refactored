using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Clone_KySoDienTu.Controllers.Viewfile
{
    //[Authorize]
    [System.Web.Http.RoutePrefix("api/viewfileonline")]
    public class viewfileonlineController : ApiController
    {
        [HttpGet]
        [Route("getviewpdf")]
        public HttpResponseMessage getviewpdf()
        {

            //if (id > 0)
            //{
            //    dbFSMEntities db = new dbFSMEntities();
            //    dsFilescanModel result = new dsFilescanModel();
            //    var spath = db.Database.SqlQuery<FilescanPath>("proc_getPathfilescan @idfile",
            //        new SqlParameter("@idfile", id)).First();
            //    if (spath != null)
            //    {
            //        string sDes = "";
            //        string sPath = "";
            //        if (spath.VITRILUUTRU != "")
            //        {
            //            sDes = spath.VITRILUUTRU;
            //            if (!Directory.Exists(sDes))
            //            {
            //                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/DataFileScan/");
            //            }
            //            else
            //            {
            //                sPath = spath.VITRILUUTRU;
            //            }
            //        }
            //        else
            //        {
            //            sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/DataFileScan/");
            //        }
            //        if (spath.MADUAN.IndexOf("~PAT") > 0)
            //        {
            //            sPath = Path.Combine(sPath, spath.GHICHU.Substring(1));
            //        }
            //        else
            //        {
            //            sPath = Path.Combine(sPath, spath.MADUAN, spath.MAPHONG, spath.MUCLUC.ToString(), spath.MAHOSO, spath.GHICHU);
            //        }
            //        var response = new HttpResponseMessage(HttpStatusCode.OK);
            //        var stream = new System.IO.FileStream(sPath, System.IO.FileMode.Open);
            //        response.Content = new StreamContent(stream);
            //        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
            //        return response;
            //    }
            //}

            //return response;
            string sPath1 = System.Web.Hosting.HostingEnvironment.MapPath("~/DataFile/Mau_baocao_cuoiky.pdf");
            var response1 = new HttpResponseMessage(HttpStatusCode.OK);
            var stream1 = new System.IO.FileStream(sPath1, System.IO.FileMode.Open);
            response1.Content = new StreamContent(stream1);
            response1.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
            return response1;
        }
    }
}
