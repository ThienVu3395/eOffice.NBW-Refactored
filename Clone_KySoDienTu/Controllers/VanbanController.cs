using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clone_KySoDienTu.Controllers.Vanban
{
    public class VanbanController : Controller
    {
        //
        // GET: /Vanban/
       

        public ActionResult ModalDieuChinhVBDi()
        {
            return View();
        }
        public ActionResult ModalPhanPhatTree()
        {
            return View();
        }
    }
}