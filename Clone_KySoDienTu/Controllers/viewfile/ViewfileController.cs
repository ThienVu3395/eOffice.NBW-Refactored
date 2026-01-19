using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clone_KySoDienTu.Controllers.Viewfile
{
    public class ViewfileController : Controller
    {
        //
        // GET: /Viewfile/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult viewfileonline()
        {
            return View();
        }
        public ActionResult viewfilepdfonline()
        {
            return View();
        }
        public ActionResult viewfilewordonline()
        {
            return View();
        }
        public ActionResult viewfile2()
        {
            return View();
        }

        public ActionResult FlipBook(
            string urlBackend,
            string folder,
            string fileName,
            string fileNameGuid)
        {
            ViewBag.urlBackend = urlBackend;

            ViewBag.Folder = folder;

            ViewBag.FileName = fileName;

            ViewBag.FileNameGuid = fileNameGuid;

            return View();
        }
    }
}