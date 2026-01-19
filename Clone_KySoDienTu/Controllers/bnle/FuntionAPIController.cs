using Clone_KySoDienTu.Models.Bnle;
using Clone_KySoDienTu.Models.UserInfo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Clone_KySoDienTu.Controllers.BNLE
{
    [Authorize]
    [RoutePrefix("api/funcbase")]
    public class FuntionAPIController : ApiController
    {
        //[HttpPost]
        //[Route("getnewdatatable")]
        //public IEnumerable<DataControl> getnewdatatable(DataParnew par)
        //{
        //    try
        //    {
        //        dbOAMSEntities db = new dbOAMSEntities();
        //        string tablename = par.Tablename;
        //        tablename = CryptData.querydecrypt(tablename);
        //        string ID = par.id.checkIsNull();
        //        var rfun = db.Database.SqlQuery<tbFunction>("select f.* from bnle.tbFunction f where f.MAFUNC = @MAFUNC and KICHTHOAT = 1"
        //           , new SqlParameter("@MAFUNC", tablename)).FirstOrDefault();
        //        if (rfun != null)
        //        {
        //            int numcoll = rfun.NUMCOL.Value;
        //            string widthlabel = "";
        //            widthlabel = string.Format("col-md-{0} col-sm-{0} col-xs-3", rfun.WITHDLABEL.Value);
        //            var vrs = db.Database.SqlQuery<tbFORMFUNC>("select f.* from bnle.tbFORMFUNC f where f.MAFUNC = @MAFUNC and HIENTHI = 1 order by THUTUNHAP"
        //            , new SqlParameter("@MAFUNC", tablename)).ToList();
        //            List<DataControl> hsc = new List<DataControl>();
        //            bool newrow = false;
        //            string keytype = "";
        //            int cur = 0;
        //            bool idkey = false;
        //            for (int i = 0; i < vrs.Count; i++)
        //            {
        //                if (vrs[i].MATRUONG == rfun.IDKEY) idkey = true;
        //                DataControl h = new DataControl();
        //                h.widthlabel = widthlabel;
        //                h.id = "idvb_" + vrs[i].ID;
        //                h.label = vrs[i].TENTRUONG;
        //                h.namecol = vrs[i].MATRUONG;
        //                h.ctype = vrs[i].KIEUDULIEU;
        //                h.width = string.Format("col-md-{0} col-sm-{0} col-xs-12", vrs[i].WITHDFORM.Value);
        //                if (vrs[i].WITHDFORM.Value > 0)
        //                {
        //                    if (cur == 0)
        //                    {
        //                        h.viewed = "0";
        //                        cur = vrs[i].WITHDFORM.Value + rfun.WITHDLABEL.Value;
        //                    }
        //                    else if (vrs[i].WITHDFORM.Value + rfun.WITHDLABEL.Value + cur > 12)
        //                    {
        //                        h.viewed = "0";
        //                        cur = 0;
        //                    }
        //                    else
        //                    {
        //                        h.viewed = "1";
        //                        cur += vrs[i].WITHDFORM.Value + rfun.WITHDLABEL.Value;
        //                    }
        //                }
        //                else
        //                {
        //                    h.width = "0";
        //                }
        //                h.required = vrs[i].BATBUOC == 1 ? true : false;
        //                if (vrs[i].RANGBUOC.checkIsNull() != "")
        //                {
        //                    string wheredir = "";
        //                    if (vrs[i].TUDIEN.checkIsNull() != "")
        //                    {
        //                        wheredir = "where " + vrs[i].TUDIEN;
        //                    }
        //                    string sqldir = string.Format("select {0} as [CODE],{1} as [VALUENAME] from {2} {3}",
        //                    vrs[i].KEYFOR, vrs[i].NAMEFOR, vrs[i].RANGBUOC, wheredir);

        //                    var dirs = db.Database.SqlQuery<DataDirControl>(sqldir).ToList();
        //                    h.dictionary = dirs;
        //                }
        //                else
        //                {
        //                    if (vrs[i].TUDIEN.checkIsNull() != "")
        //                    {
        //                        var dirs = db.Database.SqlQuery<DataDirControl>("select [CODE], [VALUENAME] from [adm].[tbDanhmuc] where LOAIDM = @LOAIDM"
        //                        , new SqlParameter("@LOAIDM", vrs[i].TUDIEN)).ToList();
        //                        h.dictionary = dirs;
        //                    }
        //                    else h.dictionary = new List<DataDirControl>();
        //                }

        //                h.values = vrs[i].MACDINH;
        //                if (rfun.IDKEY == vrs[i].MATRUONG)
        //                {
        //                    keytype = h.ctype;

        //                }
        //                h.disable = false;
        //                hsc.Add(h);
        //            }
        //            if (!idkey)
        //            {
        //                DataControl h = new DataControl();
        //                h.widthlabel = widthlabel;
        //                h.id = "IDKEY";
        //                h.label = rfun.IDKEY;
        //                h.namecol = rfun.IDKEY;
        //                h.ctype = rfun.IDKEY;
        //            }
        //            if (ID != "")
        //            {
        //                StringBuilder sqlcom = new StringBuilder("select *");
        //                sqlcom.Append(" from " + rfun.TABLENAME + " where " + rfun.IDKEY + " = @" + rfun.IDKEY);
        //                var dt = new DataTable();
        //                var conn = db.Database.Connection;
        //                var connectionState = conn.State;
        //                try
        //                {
        //                    if (connectionState != ConnectionState.Open) conn.Open();
        //                    using (var cmd = conn.CreateCommand())
        //                    {
        //                        cmd.CommandText = sqlcom.ToString();
        //                        cmd.CommandType = CommandType.Text;
        //                        switch (keytype)
        //                        {
        //                            case "text": cmd.Parameters.Add(new SqlParameter("@" + rfun.IDKEY, ID)); break;
        //                            case "number": cmd.Parameters.Add(new SqlParameter("@" + rfun.IDKEY, Convert.ToInt32(ID))); break;
        //                            default: cmd.Parameters.Add(new SqlParameter("@" + rfun.IDKEY, ID)); break;
        //                        }
        //                        using (var reader = cmd.ExecuteReader())
        //                        {
        //                            dt.Load(reader);
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    if (connectionState != ConnectionState.Closed) conn.Close();
        //                }
        //                finally
        //                {
        //                    if (connectionState != ConnectionState.Closed) conn.Close();
        //                }
        //                if (dt.Rows.Count > 0)
        //                {
        //                    DataControlCols item = new DataControlCols();
        //                    item.items = new List<DataControlRow>();
        //                    foreach (var j in hsc)
        //                    {
        //                        j.values = dt.Rows[0][j.namecol].ToString();
        //                        j.disable = false;
        //                        if (rfun.IDKEY == j.namecol)
        //                        {
        //                            j.disable = true;
        //                        }

        //                    }
        //                }
        //            }

        //            return hsc.AsEnumerable();
        //        }
        //        return (new List<DataControl>()).AsEnumerable();
        //    }
        //    catch
        //    {
        //        return (new List<DataControl>()).AsEnumerable();
        //    }
        //}

        //[HttpPost]
        //[Route("getnewdata")]
        //public IEnumerable<DataControl> getnewdata(DataParnew par)
        //{
        //    try
        //    {
        //        dbOAMSEntities db = new dbOAMSEntities();
        //        string tablename = par.Tablename;
        //        tablename = CryptData.querydecrypt(tablename);
        //        string ID = par.id.checkIsNull();
        //        var rfun = db.Database.SqlQuery<tbFunction>("select f.* from bnle.tbFunction f where f.MAFUNC = @MAFUNC and KICHTHOAT = 1"
        //           , new SqlParameter("@MAFUNC", tablename)).FirstOrDefault();
        //        if (rfun != null)
        //        {
        //            int numcoll = rfun.NUMCOL.Value;
        //            string widthlabel = "";
        //            widthlabel = string.Format("col-md-{0} col-sm-{0} col-xs-3", rfun.WITHDLABEL.Value);
        //            var vrs = db.Database.SqlQuery<tbFORMFUNC>("select f.* from bnle.tbFORMFUNC f where f.MAFUNC = @MAFUNC and HIENTHI = 1 order by THUTUNHAP"
        //            , new SqlParameter("@MAFUNC", tablename)).ToList();
        //            List<DataControl> hsc = new List<DataControl>();
        //            bool newrow = false;
        //            string keytype = "";
        //            int cur = 0;
        //            for (int i = 0; i < vrs.Count; i++)
        //            {
        //                DataControl h = new DataControl();
        //                h.widthlabel = widthlabel;
        //                h.id = "idvb_" + vrs[i].ID;
        //                h.label = vrs[i].TENTRUONG;
        //                h.namecol = vrs[i].MATRUONG;
        //                h.ctype = vrs[i].KIEUDULIEU;
        //                h.width = string.Format("col-md-{0} col-sm-{0} col-xs-12", vrs[i].WITHDFORM.Value);

        //                if (cur == 0)
        //                {
        //                    h.viewed = "0";
        //                    cur = vrs[i].WITHDFORM.Value + rfun.WITHDLABEL.Value;
        //                }
        //                else if (vrs[i].WITHDFORM.Value + rfun.WITHDLABEL.Value + cur > 12)
        //                {
        //                    h.viewed = "0";
        //                    cur = 0;
        //                }
        //                else
        //                {
        //                    h.viewed = "1";
        //                    cur += vrs[i].WITHDFORM.Value + rfun.WITHDLABEL.Value;
        //                }
        //                h.required = vrs[i].BATBUOC == 1 ? true : false;
        //                if (vrs[i].RANGBUOC.checkIsNull() != "")
        //                {
        //                    string wheredir = "";
        //                    if (vrs[i].TUDIEN.checkIsNull() != "")
        //                    {
        //                        wheredir = "where " + vrs[i].TUDIEN;
        //                    }
        //                    string sqldir = string.Format("select {0} as [CODE],{1} as [VALUENAME] from {2} {3}",
        //                    vrs[i].KEYFOR, vrs[i].NAMEFOR, vrs[i].RANGBUOC, wheredir);

        //                    var dirs = db.Database.SqlQuery<DataDirControl>(sqldir).ToList();
        //                    h.dictionary = dirs;
        //                }
        //                else
        //                {
        //                    if (vrs[i].TUDIEN.checkIsNull() != "")
        //                    {
        //                        var dirs = db.Database.SqlQuery<DataDirControl>("select [CODE], [VALUENAME] from [adm].[tbDanhmuc] where LOAIDM = @LOAIDM"
        //                        , new SqlParameter("@LOAIDM", vrs[i].TUDIEN)).ToList();
        //                        h.dictionary = dirs;
        //                    }
        //                    else h.dictionary = new List<DataDirControl>();
        //                }

        //                h.values = vrs[i].MACDINH;
        //                if (rfun.IDKEY == vrs[i].MATRUONG)
        //                {
        //                    keytype = h.ctype;

        //                }
        //                h.disable = false;
        //                hsc.Add(h);
        //            }
        //            if (ID != "")
        //            {
        //                StringBuilder sqlcom = new StringBuilder("select *");
        //                sqlcom.Append(" from " + rfun.TABLENAME + " where " + rfun.IDKEY + " = @" + rfun.IDKEY);
        //                var dt = new DataTable();
        //                var conn = db.Database.Connection;
        //                var connectionState = conn.State;
        //                try
        //                {
        //                    if (connectionState != ConnectionState.Open) conn.Open();
        //                    using (var cmd = conn.CreateCommand())
        //                    {
        //                        cmd.CommandText = sqlcom.ToString();
        //                        cmd.CommandType = CommandType.Text;
        //                        switch (keytype)
        //                        {
        //                            case "text": cmd.Parameters.Add(new SqlParameter("@" + rfun.IDKEY, ID)); break;
        //                            case "number": cmd.Parameters.Add(new SqlParameter("@" + rfun.IDKEY, Convert.ToInt32(ID))); break;
        //                            default: cmd.Parameters.Add(new SqlParameter("@" + rfun.IDKEY, ID)); break;
        //                        }
        //                        using (var reader = cmd.ExecuteReader())
        //                        {
        //                            dt.Load(reader);
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    if (connectionState != ConnectionState.Closed) conn.Close();
        //                }
        //                finally
        //                {
        //                    if (connectionState != ConnectionState.Closed) conn.Close();
        //                }
        //                if (dt.Rows.Count > 0)
        //                {
        //                    DataControlCols item = new DataControlCols();
        //                    item.items = new List<DataControlRow>();
        //                    foreach (var j in hsc)
        //                    {
        //                        j.values = dt.Rows[0][j.namecol].ToString();
        //                        j.disable = false;
        //                        if (rfun.IDKEY == j.namecol)
        //                        {
        //                            j.disable = true;
        //                        }

        //                    }
        //                }
        //            }

        //            return hsc.AsEnumerable();
        //        }
        //        return (new List<DataControl>()).AsEnumerable();
        //    }
        //    catch
        //    {
        //        return (new List<DataControl>()).AsEnumerable();
        //    }
        //}

        //[HttpPost]
        //[Route("getalldatapage")]
        //public IHttpActionResult getalldatapage(UserAParModel par)
        //{
        //    try
        //    {
        //        dbOAMSEntities db = new dbOAMSEntities();
        //        int p = par.valint1.checkIsNumber();
        //        int nums = CommonSystem._itemsofpage * (p - 1);
        //        string tablename = par.valstring1.checkIsNull();
        //        tablename = CryptData.querydecrypt(tablename);
        //        var rfun = db.Database.SqlQuery<tbFunction>("select f.* from bnle.tbFunction f where f.MAFUNC = @MAFUNC and KICHTHOAT = 1"
        //            , new SqlParameter("@MAFUNC", tablename)).FirstOrDefault();
        //        if (rfun != null)
        //        {
        //            var vrs = db.Database.SqlQuery<tbFORMFUNC>("select f.* from bnle.tbFORMFUNC f where f.MAFUNC = @MAFUNC and HIENTHI = 1 and THUTUGRID > 0 order by THUTUGRID"
        //                , new SqlParameter("@MAFUNC", tablename)).ToList();
        //            List<DataControlCol> headercol = new List<DataControlCol>();
        //            StringBuilder sqlcom = new StringBuilder("select ");

        //            for (int i = 0; i < vrs.Count; i++)
        //            {
        //                DataControlCol h = new DataControlCol();
        //                h.id = "idvb_" + vrs[i].ID;
        //                sqlcom.Append(vrs[i].MATRUONG);
        //                if ((i < vrs.Count - 1))
        //                {
        //                    sqlcom.Append(",");
        //                }
        //                h.label = vrs[i].TENTRUONG;
        //                h.namecol = vrs[i].MATRUONG;
        //                h.width = vrs[i].WITHDGRID;
        //                h.viewgrid = vrs[i].VIEWGRID.checkIsNull() == "" ? "text-align:center" : vrs[i].VIEWGRID;
        //                headercol.Add(h);
        //            }
        //            sqlcom.Append(" from " + rfun.TABLENAME + " ORDER BY " + rfun.ORDERBY + " OFFSET @nums ROWS FETCH NEXT @nume ROWS ONLY");
        //            var controws = db.Database.SqlQuery<int>("select count(*) from  " + rfun.TABLENAME).FirstOrDefault();
        //            var dt = new DataTable();
        //            var conn = db.Database.Connection;
        //            var connectionState = conn.State;
        //            try
        //            {
        //                if (connectionState != ConnectionState.Open) conn.Open();
        //                using (var cmd = conn.CreateCommand())
        //                {
        //                    cmd.CommandText = sqlcom.ToString();
        //                    cmd.CommandType = CommandType.Text;
        //                    cmd.Parameters.Add(new SqlParameter("@nums", nums));
        //                    cmd.Parameters.Add(new SqlParameter("@nume", CommonSystem._itemsofpage));
        //                    using (var reader = cmd.ExecuteReader())
        //                    {
        //                        dt.Load(reader);
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                if (connectionState != ConnectionState.Closed) conn.Close();
        //            }
        //            finally
        //            {
        //                if (connectionState != ConnectionState.Closed) conn.Close();
        //            }
        //            DataGridModel result = new DataGridModel();
        //            result.datacols = headercol;
        //            result.datarows = new List<DataControlCols>();
        //            result.formname = rfun.TENFUNC;
        //            result.viewmode = rfun.VIEWMODE;
        //            result.TotalItems = controws;
        //            result.PerPage = CommonSystem._itemsofpage;
        //            if (dt.Rows.Count > 0)
        //            {
        //                foreach (DataRow r in dt.Rows)
        //                {
        //                    DataControlCols item = new DataControlCols();
        //                    item.items = new List<DataControlRow>();
        //                    foreach (var j in headercol)
        //                    {
        //                        DataControlRow i = new DataControlRow();
        //                        i.namecol = j.namecol;
        //                        i.ctype = "";
        //                        i.values = r[j.namecol].ToString();
        //                        i.viewgrid = j.viewgrid;
        //                        item.items.Add(i);
        //                    }
        //                    item.keyrow = r[rfun.IDKEY].ToString();
        //                    result.datarows.Add(item);
        //                }
        //            }
        //            return Ok(result);
        //        }

        //        return Ok(new DataGridModel());
        //    }
        //    catch
        //    {
        //        return Ok(new DataGridModel());
        //    }
        //}

        //[HttpPost]
        //[Route("savenewdata")]
        //public IHttpActionResult savenewdata(DataSaveControl par)
        //{
        //    try
        //    {
        //        dbOAMSEntities db = new dbOAMSEntities();
        //        string tablename = par.Tablename;
        //        tablename = CryptData.querydecrypt(tablename);
        //        var rfun = db.Database.SqlQuery<tbFunction>("select f.* from bnle.tbFunction f where f.MAFUNC = @MAFUNC and KICHTHOAT = 1"
        //           , new SqlParameter("@MAFUNC", tablename)).FirstOrDefault();
        //        if (rfun != null)
        //        {
        //            //var vrs = db.Database.SqlQuery<tbFORMFUNC>("select f.* from bnle.tbFORMFUNC f where f.MAFUNC = @MAFUNC and HIENTHI = 1 order by THUTUNHAP"
        //            //, new SqlParameter("@MAFUNC", tablename)).ToList();

        //            StringBuilder sqlcom = new StringBuilder("insert into " + rfun.TABLENAME + "( ");
        //            StringBuilder sqlpar = new StringBuilder(" values( ");
        //            SqlParameter[] sqlp = new SqlParameter[par.items.Count];
        //            for (int i = 0; i < par.items.Count; i++)
        //            {
        //                sqlcom.Append(par.items[i].namecol);
        //                sqlpar.Append("@" + par.items[i].namecol);
        //                if ((i < par.items.Count - 1))
        //                {
        //                    sqlcom.Append(",");
        //                    sqlpar.Append(",");
        //                }
        //                switch (par.items[i].ctype)
        //                {
        //                    case "text":
        //                        sqlp[i] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values.checkIsNull()); break;
        //                    case "number":
        //                        if (par.items[i].values != null)
        //                            sqlp[i] = new SqlParameter("@" + par.items[i].namecol, Convert.ToInt32(par.items[i].values));
        //                        else sqlp[i] = new SqlParameter("@" + par.items[i].namecol, DBNull.Value);
        //                        break;
        //                    case "select":
        //                        sqlp[i] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values); break;
        //                    case "textarea":
        //                        sqlp[i] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values); break;
        //                    case "autocomplete":
        //                        sqlp[i] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values); break;
        //                    case "calendar":
        //                        if (par.items[i].values != null)
        //                            sqlp[i] = new SqlParameter("@" + par.items[i].namecol, Convert.ToDateTime(par.items[i].values));
        //                        else sqlp[i] = new SqlParameter("@" + par.items[i].namecol, DBNull.Value);
        //                        break;
        //                    case "check":
        //                        if (par.items[i].values != null)
        //                            sqlp[i] = new SqlParameter("@" + par.items[i].namecol, Convert.ToBoolean(par.items[i].values));
        //                        else sqlp[i] = new SqlParameter("@" + par.items[i].namecol, DBNull.Value);
        //                        break;
        //                    default:
        //                        sqlp[i] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values); break;
        //                }

        //            }
        //            sqlcom.Append(")");
        //            sqlpar.Append(")");
        //            sqlcom.Append(sqlpar);
        //            db.Database.ExecuteSqlCommand(sqlcom.ToString(), sqlp);
        //        }
        //        return Ok(1);
        //    }
        //    catch
        //    {
        //        return BadRequest();
        //    }
        //}
        //[HttpPost]
        //[Route("deletedata")]
        //public IHttpActionResult deletedata(DataParnew par)
        //{
        //    try
        //    {
        //        dbOAMSEntities db = new dbOAMSEntities();
        //        string tablename = par.Tablename;
        //        tablename = CryptData.querydecrypt(tablename);
        //        string ID = par.id.checkIsNull();
        //        var rfun = db.Database.SqlQuery<tbFunction>("select f.* from bnle.tbFunction f where f.MAFUNC = @MAFUNC and KICHTHOAT = 1"
        //           , new SqlParameter("@MAFUNC", tablename)).FirstOrDefault();
        //        if (rfun != null)
        //        {
        //            if (ID != "")
        //            {
        //                StringBuilder sqlcom = new StringBuilder("");
        //                sqlcom.Append("delete " + rfun.TABLENAME + " where " + rfun.IDKEY + " = @" + rfun.IDKEY);
        //                var vrs = db.Database.SqlQuery<tbFORMFUNC>("select f.* from bnle.tbFORMFUNC f where f.MAFUNC = @MAFUNC and MATRUONG = @MATRUONG "
        //                , new SqlParameter("@MAFUNC", tablename), new SqlParameter("@MATRUONG", rfun.IDKEY)).ToList();
        //                string keytype = "";
        //                for (int i = 0; i < vrs.Count; i++)
        //                {
        //                    if (rfun.IDKEY == vrs[i].MATRUONG)
        //                    {
        //                        keytype = vrs[i].KIEUDULIEU; break;
        //                    }
        //                }
        //                SqlParameter parsql;
        //                switch (keytype)
        //                {
        //                    case "text": parsql = new SqlParameter("@" + rfun.IDKEY, ID); break;
        //                    case "number": parsql = new SqlParameter("@" + rfun.IDKEY, Convert.ToInt32(ID)); break;
        //                    default: parsql = new SqlParameter("@" + rfun.IDKEY, ID); break;
        //                }
        //                db.Database.ExecuteSqlCommand(sqlcom.ToString(), parsql);
        //            }
        //        }
        //        return Ok(1);
        //    }
        //    catch
        //    {
        //        return BadRequest();
        //    }
        //}
        //[HttpPost]
        //[Route("saveeditdata")]
        //public IHttpActionResult saveeditdata(DataSaveControl par)
        //{
        //    try
        //    {
        //        dbOAMSEntities db = new dbOAMSEntities();
        //        string tablename = par.Tablename;
        //        tablename = CryptData.querydecrypt(tablename);
        //        string ID = par.id.checkIsNull();
        //        var rfun = db.Database.SqlQuery<tbFunction>("select f.* from bnle.tbFunction f where f.MAFUNC = @MAFUNC and KICHTHOAT = 1"
        //           , new SqlParameter("@MAFUNC", tablename)).FirstOrDefault();
        //        if (rfun != null)
        //        {
        //            //var vrs = db.Database.SqlQuery<tbFORMFUNC>("select f.* from bnle.tbFORMFUNC f where f.MAFUNC = @MAFUNC and HIENTHI = 1 order by THUTUNHAP"
        //            //, new SqlParameter("@MAFUNC", tablename)).ToList();
        //            string keytype = "";
        //            StringBuilder sqlcom = new StringBuilder("update " + rfun.TABLENAME + " set ");
        //            int countpar = 0;
        //            if (rfun.IDKEY == "ID") countpar = par.items.Count + 1;
        //            else countpar = par.items.Count;
        //            SqlParameter[] sqlp = new SqlParameter[countpar];
        //            int k = 0;
        //            for (int i = 0; i < par.items.Count; i++)
        //            {
        //                if (rfun.IDKEY != par.items[i].namecol)
        //                {
        //                    sqlcom.Append(par.items[i].namecol + " = @" + par.items[i].namecol);
        //                    if ((i < par.items.Count - 1))
        //                    {
        //                        sqlcom.Append(",");
        //                    }
        //                    switch (par.items[i].ctype)
        //                    {
        //                        case "text":
        //                            sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values.checkIsNull()); break;
        //                        case "number":
        //                            if (par.items[i].values != null)
        //                                sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, Convert.ToInt32(par.items[i].values));
        //                            else sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, DBNull.Value);
        //                            break;
        //                        case "select":
        //                            sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values); break;
        //                        case "textarea":
        //                            sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values); break;
        //                        case "autocomplete":
        //                            sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values); break;
        //                        case "calendar":
        //                            if (par.items[i].values != null)
        //                                sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, Convert.ToDateTime(par.items[i].values));
        //                            else sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, DBNull.Value);
        //                            break;
        //                        case "check":
        //                            if (par.items[i].values != null)
        //                                sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, Convert.ToBoolean(par.items[i].values));
        //                            else sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, DBNull.Value);
        //                            break;
        //                        default:
        //                            sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values); break;
        //                    }
        //                }
        //                else
        //                {
        //                    keytype = par.items[i].ctype;
        //                }
        //            }
        //            sqlcom.Append(" where " + rfun.IDKEY + " = @" + rfun.IDKEY);
        //            switch (keytype)
        //            {
        //                case "text": sqlp[k] = new SqlParameter("@" + rfun.IDKEY, ID); break;
        //                case "number": sqlp[k] = new SqlParameter("@" + rfun.IDKEY, Convert.ToInt32(ID)); break;
        //                default: sqlp[k] = new SqlParameter("@" + rfun.IDKEY, ID); break;
        //            }
        //            db.Database.ExecuteSqlCommand(sqlcom.ToString(), sqlp);
        //        }
        //        return Ok(1);
        //    }
        //    catch
        //    {
        //        return BadRequest();
        //    }
        //}

        //[HttpPost]
        //[Route("getalldatafilter")]
        //public IHttpActionResult getalldatafilter(UserAParModel par)
        //{
        //    try
        //    {
        //        dbOAMSEntities db = new dbOAMSEntities();
        //        string tablename = par.valstring1.checkIsNull();
        //        tablename = CryptData.querydecrypt(tablename);
        //        var rfun = db.Database.SqlQuery<tbFunction>("select f.* from bnle.tbFunction f where f.MAFUNC = @MAFUNC and KICHTHOAT = 1"
        //            , new SqlParameter("@MAFUNC", tablename)).FirstOrDefault();
        //        if (rfun != null)
        //        {
        //            if (rfun.TABLEPARENT.checkIsNull() != "")
        //            {
        //                DataGridFModel result = new DataGridFModel();
        //                string sqlPARENT = string.Format("select {0} as [CODE],{1} as [VALUENAME] from {2}",
        //                   rfun.PARENTKEY, rfun.PARENTNAME, rfun.TABLEPARENT);
        //                var dirs = db.Database.SqlQuery<DataDirControl>(sqlPARENT).ToList();
        //                result.datafilter = dirs;
        //                result.tablename = rfun.PARENTNAME;
        //                result.formname = rfun.TENFUNC;
        //                result.viewmode = rfun.VIEWMODE;
        //                result.widthfilter = string.Format("col-xs-{0}", rfun.PARENTWITHD);
        //                result.widthgrid = string.Format("col-xs-{0}", 12 - Convert.ToInt32(rfun.PARENTWITHD));
        //                return Ok(result);
        //            }
        //        }

        //        return Ok(new List<DataDirControl>());
        //    }
        //    catch
        //    {
        //        return Ok(new List<DataDirControl>());
        //    }
        //}
        //[HttpPost]
        //[Route("getalldatapagef")]
        //public IHttpActionResult getalldatapagef(UserAParModel par)
        //{
        //    try
        //    {
        //        dbOAMSEntities db = new dbOAMSEntities();
        //        int p = par.valint1.checkIsNumber();
        //        int nums = CommonSystem._itemsofpage * (p - 1);
        //        string tablename = par.valstring1.checkIsNull();
        //        string filterkey = par.valstring2.checkIsNull();
        //        tablename = CryptData.querydecrypt(tablename);
        //        var rfun = db.Database.SqlQuery<tbFunction>("select f.* from bnle.tbFunction f where f.MAFUNC = @MAFUNC and KICHTHOAT = 1"
        //            , new SqlParameter("@MAFUNC", tablename)).FirstOrDefault();
        //        if (rfun != null)
        //        {
        //            var vrs = db.Database.SqlQuery<tbFORMFUNC>("select f.* from bnle.tbFORMFUNC f where f.MAFUNC = @MAFUNC and HIENTHI = 1 and THUTUGRID > 0 order by THUTUGRID"
        //                , new SqlParameter("@MAFUNC", tablename)).ToList();
        //            List<DataControlCol> headercol = new List<DataControlCol>();
        //            StringBuilder sqlcom = new StringBuilder("select ");
        //            StringBuilder sqlcount = new StringBuilder("");
        //            for (int i = 0; i < vrs.Count; i++)
        //            {
        //                DataControlCol h = new DataControlCol();
        //                h.id = "idvb_" + vrs[i].ID;
        //                sqlcom.Append(vrs[i].MATRUONG);
        //                if ((i < vrs.Count - 1))
        //                {
        //                    sqlcom.Append(",");
        //                }
        //                h.label = vrs[i].TENTRUONG;
        //                h.namecol = vrs[i].MATRUONG;
        //                h.width = vrs[i].WITHDGRID;
        //                h.viewgrid = vrs[i].VIEWGRID.checkIsNull() == "" ? "text-align:center" : vrs[i].VIEWGRID;
        //                headercol.Add(h);
        //            }
        //            if(rfun.IDKEY == "ID")
        //                sqlcom.Append(",ID");
        //            DataGridModel result = new DataGridModel();
        //            result.datacols = headercol;

        //            if (filterkey != "" && rfun.KEYFOREIGN.checkIsNull() !="")
        //            {
        //                sqlcount.Append("select count(*) from  " + rfun.TABLENAME + " where " + rfun.KEYFOREIGN  + " = @" + rfun.KEYFOREIGN);
        //                sqlcom.Append(" from " + rfun.TABLENAME + " where " + rfun.KEYFOREIGN + " = @" + rfun.KEYFOREIGN + " ORDER BY " + rfun.ORDERBY + " OFFSET @nums ROWS FETCH NEXT @nume ROWS ONLY");
        //            }
        //            else
        //            {
        //                sqlcom.Append(" from " + rfun.TABLENAME + " ORDER BY " + rfun.ORDERBY + " OFFSET @nums ROWS FETCH NEXT @nume ROWS ONLY");
        //                sqlcount.Append("select count(*) from  " + rfun.TABLENAME);
        //            }
        //            result.datarows = new List<DataControlCols>();
        //            result.formname = rfun.TENFUNC;
        //            result.viewmode = rfun.VIEWMODE;
        //            if (filterkey != "" && rfun.KEYFOREIGN.checkIsNull() != "")
        //            {
        //                var controws = db.Database.SqlQuery<int>(sqlcount.ToString(), new SqlParameter("@" + rfun.KEYFOREIGN, filterkey)).FirstOrDefault();
        //                result.TotalItems = controws;
        //            }
        //            else
        //            {
        //                var controws = db.Database.SqlQuery<int>(sqlcount.ToString()).FirstOrDefault();
        //                result.TotalItems = controws;
        //            }
                    
        //            var dt = new DataTable();
                   
        //            result.PerPage = CommonSystem._itemsofpage;
        //            var conn = db.Database.Connection;
        //            var connectionState = conn.State;
        //            try
        //            {
        //                if (connectionState != ConnectionState.Open) conn.Open();
        //                using (var cmd = conn.CreateCommand())
        //                {
        //                    cmd.CommandText = sqlcom.ToString();
        //                    cmd.CommandType = CommandType.Text;
        //                    if (filterkey != "" && rfun.KEYFOREIGN.checkIsNull() != "")
        //                    {
        //                        cmd.Parameters.Add(new SqlParameter("@" + rfun.KEYFOREIGN, filterkey));
        //                    }
        //                    cmd.Parameters.Add(new SqlParameter("@nums", nums));
        //                    cmd.Parameters.Add(new SqlParameter("@nume", CommonSystem._itemsofpage));
        //                    using (var reader = cmd.ExecuteReader())
        //                    {
        //                        dt.Load(reader);
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                if (connectionState != ConnectionState.Closed) conn.Close();
        //            }
        //            finally
        //            {
        //                if (connectionState != ConnectionState.Closed) conn.Close();
        //            }
        //            if (dt.Rows.Count > 0)
        //            {
        //                foreach (DataRow r in dt.Rows)
        //                {
        //                    DataControlCols item = new DataControlCols();
        //                    item.items = new List<DataControlRow>();
        //                    foreach (var j in headercol)
        //                    {
        //                        DataControlRow i = new DataControlRow();
        //                        i.namecol = j.namecol;
        //                        i.ctype = "";
        //                        i.values = r[j.namecol].ToString();
        //                        i.viewgrid = j.viewgrid;
        //                        item.items.Add(i);
        //                    }
        //                    item.keyrow = r[rfun.IDKEY].ToString();
        //                    result.datarows.Add(item);
        //                }
        //            }
        //            return Ok(result);
        //        }

        //        return Ok(new DataGridModel());
        //    }
        //    catch
        //    {
        //        return Ok(new DataGridModel());
        //    }
        //}
    }
}
