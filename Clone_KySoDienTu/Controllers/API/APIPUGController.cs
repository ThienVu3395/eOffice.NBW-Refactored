using Dapper;
using Clone_KySoDienTu.Controllers.BNLE1;
using Clone_KySoDienTu.Models;
using Clone_KySoDienTu.MyHub;
using Clone_KySoDienTu.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Http;
using SteCode;
using System.Net;
using System.Net.NetworkInformation;
using Clone_KySoDienTu.Controllers.Congviec;
using SteCore;
using System.Net.Http;

namespace Clone_KySoDienTu.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/danhba")]
    public class APIPUGController : EntitySetControllerWithHub<EventHub>
    {
        private readonly string _cnn;
        private StePUG.StePUG _StePUG;
        public APIPUGController()
        {
            _cnn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
            _StePUG = new StePUG.StePUG(_cnn);
        }

        public int UpdateSMS (SteProject.CommonModel par)
        {
            return _StePUG.UpdateQueueSMS(par);
        }
        public void CallSignal(SteProject.CommonModel par)
        {
            Hub.Clients.Group(par.valstring1).popupEvent(par);
        }
        [Route("getViewdanhba")]
        [HttpPost]
        public IHttpActionResult getViewdanhba()
        {
            try
            {
                SteProject.CommonModel par = new SteProject.CommonModel();
                par.valstring1 = User.Identity.Name;
                return Ok(_StePUG.V_GetGroups(par));
            }
            catch
            {
                return BadRequest();
            }
        }

        private IEnumerable<tbMenuChucNang> CreateVM(int parentid, List<tbNhomChucnangModel> source)
        {
            return from men in source
                   where men.PARENTID == parentid
                   orderby men.THUTU
                   select new tbMenuChucNang()
                   {
                       par = men,
                       childitem = CreateVM(men.ID, source).ToList()
                   };
        }

        [Route("getUsersMenu")]
        [HttpGet]
        public IHttpActionResult getUsersMenu()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_cnn))
                {
                    if (db.State == System.Data.ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    if (User.IsInRole("Administrators"))
                        parameters.Add("@UserName", "Admin");
                    else
                        parameters.Add("@UserName", User.Identity.Name);
                    var listall = db.Query<tbNhomChucnangModel>("Core_GetUsersMenu", parameters, null, true, null, System.Data.CommandType.StoredProcedure).ToList();
                    var a = CreateVM(-1, listall);
                    return Ok(a);

                }
            }
            catch
            {
                return BadRequest();
            }
        }

        #region UserEvent
        [Route("getUserEvent")]
        [HttpPost]
        public IHttpActionResult getUserEvent(SteProject.CommonModel par)
        {
            try
            {
                return Ok(_StePUG.GetUserEvent(par));
            }
            catch
            {
                return BadRequest();
            }
        }
        [Route("getAllFullCalendar")]
        [HttpGet]
        public IHttpActionResult getAllFullCalendar()
        {
            try
            {
                return Ok(_StePUG.GetAllFullCalendar(new SteProject.CommonModel() { valstring1 = User.Identity.Name}));
            }
            catch 
            {
                return BadRequest();
            }
        }

        //[Route("insertUserEvent")]
        //[HttpPost]
        //public IHttpActionResult insertUserEvent(StePUG.PUG_UserEvent par)
        //{
        //    try
        //    {
        //        par.NguoiTao = User.Identity.Name;
        //        int i = _StePUG.InsertEventUser(par);
        //        if (i < 0)
        //            throw new Exception();
        //        else
        //        {
        //            if (par.SMSThoiGian.checkIsNumber() > 0)
        //            {
        //                SteProject.CommonModel val = new SteProject.CommonModel();
        //                DateTime a = new DateTime();
        //                a.checkDateTimeIsNull();
        //                val.valint1 = i;
        //                val.valint2 = par.SMSThongBao.checkBoolIsNull() ? 1 : 0;
        //                val.valint3 = par.SMSHenGio.checkBoolIsNull() ? 1 : 0;
        //                val.valstring1 = User.Identity.Name;
        //                val.valstring2 = User.Identity.Name;
        //                val.valstring3 = "Nhắc hẹn nội dung: " + par.TieuDe;
        //                DateTime smshengio = (DateTime)par.BatDau;
        //                if (smshengio.AddMinutes(-(double)par.SMSThoiGian) > DateTime.Now)
        //                {
        //                    _StePUG.InsertQueueSMS(val, smshengio.AddMinutes(-(double)par.SMSThoiGian));
        //                }
        //            }
        //            return Ok(i);
        //        }
        //    }
        //    catch
        //    {
        //        return BadRequest();
        //    }
        //}

        //[Route("editUserEvent")]
        //[HttpPost]
        //public IHttpActionResult editUserEvent(StePUG.PUG_UserEvent par)
        //{
        //    try
        //    {
        //        par.NguoiTao = User.Identity.Name;
        //        int i = _StePUG.EditEventUser(par);
        //        if (i < 0)
        //            throw new Exception();
        //        else
        //        {
        //            if(par.SMSThoiGian.checkIsNumber() > 0)
        //            {
        //                SteProject.CommonModel val = new SteProject.CommonModel();
        //                val.valint1 = par.ID;
        //                val.valint2 = par.SMSThongBao.checkBoolIsNull() ? 1: 0;
        //                val.valint3 = par.SMSHenGio.checkBoolIsNull() ? 1 : 0; 
        //                val.valstring1 = User.Identity.Name;
        //                val.valstring2 = User.Identity.Name;
        //                val.valstring3 = "Nhắc hẹn nội dung: " + par.TieuDe;
        //                DateTime smshengio = (DateTime)par.BatDau;
        //                //if (smshengio.AddMinutes(-(double)par.SMSThoiGian) > DateTime.Now)
        //                //{
        //                    _StePUG.InsertQueueSMS(val, smshengio.AddMinutes(-(double)par.SMSThoiGian));
        //                //}
        //                //else
        //                //    _StePUG.DeleteQueueSMS(val);
        //            }
        //            else
        //            {
        //                SteProject.CommonModel val = new SteProject.CommonModel();
        //                val.valint1 = par.ID;
        //                _StePUG.DeleteQueueSMS(val);
        //            }
        //            return Ok(i);
        //        }
                
        //    }
        //    catch
        //    {
        //        return BadRequest();
        //    }
        //}
        [Route("deleteUserEvent")]
        [HttpPost]
        public IHttpActionResult deleteUserEvent(SteProject.CommonModel par)
        {
            try
            {
                int i = _StePUG.DeleteEventUser(par);
                _StePUG.DeleteQueueSMS(par);
                return Ok(i);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("EndUserEvent")]
        [HttpPost]
        public IHttpActionResult EndUserEvent(SteProject.CommonModel par)
        {
            try
            {
                int i = _StePUG.EndEventUser(par);
                if (i > -1)
                {
                    par.valint2 = par.valint2 == 1 ? 2 : 0;
                    _StePUG.UpdateQueueSMS(par);
                }
                return Ok(i);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("UpdateQueueSMS")]
        [HttpPost]
        public IHttpActionResult UpdateQueueSMS(SteProject.CommonModel par)
        {
            try
            {
                return Ok(_StePUG.UpdateQueueSMS(par));
            }
            catch
            {
                return BadRequest();
            }
        }
        #endregion

    }
}