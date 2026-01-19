using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Clone_KySoDienTu.Controllers.BNLE1
{
    public class DataSaveControl
    {
        public List<DataControl> items { get; set; }
        public string Tablename { get; set; }
        public string id { get; set; }
    }
    public class DataControlRow
    {
        public string ctype { get; set; }
        public string viewgrid { get; set; }
        public string values { get; set; }
        public string namecol { get; set; }
    }
    public class DataControlCol
    {
        public string id { get; set; }
        public string namecol { get; set; }
        public string label { get; set; }
        public string width { get; set; }
        public string viewgrid { get; set; }
    }
    public class DataControlCols
    {
        public List<DataControlRow> items { get; set; }
        public string keyrow { get; set; }

        public string Tablename { get; set; }
    }
    public class DataDirControl
    {
        public int ID { get; set; }
        public string LOAIDM { get; set; }
        public string CODE { get; set; }
        public string VALUENAME { get; set; }
        public string VALUENAMECODE { get; set; }
    }
    public class tbFunction
    {
        public string MAFUNC { get; set; }
        public string TENFUNC { get; set; }
        public string TABLENAME { get; set; }
        public string TABLEPARENT { get; set; }
        public string PARENTNAME { get; set; }
        public string PARENTKEY { get; set; }
        public string PARENTWITHD { get; set; }
        public string KEYFOREIGN { get; set; }
        public string IDKEY { get; set; }
        public string VIEWMODE { get; set; }
        public Nullable<int> NUMCOL { get; set; }
        public Nullable<int> WITHDLABEL { get; set; }
        public string ORDERBY { get; set; }
        public bool KICHTHOAT { get; set; }
    }
    public class DataControl
    {
        public string id { get; set; }
        public string ctype { get; set; }
        public string width { get; set; }
        public string widthlabel { get; set; }
        public string label { get; set; }
        public string namecol { get; set; }
        public string viewed { get; set; }
        public bool required { get; set; }
        public List<DataDirControl> dictionary { get; set; }
        public string values { get; set; }
        public bool disable { get; set; }

    }
    public partial class tbFORMFUNC
    {
        public int ID { get; set; }
        public string MAFUNC { get; set; }
        public string MATRUONG { get; set; }
        public bool HIENTHI { get; set; }
        public string TENTRUONG { get; set; }
        public string KIEUDULIEU { get; set; }
        public Nullable<int> DODAI { get; set; }
        public int THUTUNHAP { get; set; }
        public Nullable<int> THUTUGRID { get; set; }
        public Nullable<int> WITHDFORM { get; set; }
        public string WITHDGRID { get; set; }
        public string VIEWGRID { get; set; }
        public int THUTUXEM { get; set; }
        public string RANGBUOC { get; set; }
        public string KEYFOR { get; set; }
        public string NAMEFOR { get; set; }
        public int THUTUTIMKEM { get; set; }
        public int BATBUOC { get; set; }
        public string TUDIEN { get; set; }
        public string MACDINH { get; set; }
    }
    public class DataParnew
    {
        public string Tablename { get; set; }
        public string id { get; set; }
    }
    //[Authorize]
    //[RoutePrefix("api/funcbase")]
    //public class FuntionAPIController : ApiController
    //{
    //    [HttpPost]
    //    [Route("getnewdata")]
    //    public IEnumerable<DataControl> getnewdata(DataParnew par)
    //    {
    //        try
    //        {
    //            Database.dbOAMSEntities db = new Database.dbOAMSEntities();
    //            string tablename = par.Tablename;
    //            tablename = CryptData.querydecrypt(tablename);
    //            string ID = par.id.checkIsNull();
    //            var rfun = db.Database.SqlQuery<tbFunction>("select f.* from bnle.tbFunction f where f.MAFUNC = @MAFUNC and KICHTHOAT = 1"
    //               , new SqlParameter("@MAFUNC", tablename)).FirstOrDefault();
    //            if (rfun != null)
    //            {
    //                int numcoll = rfun.NUMCOL.Value;
    //                string widthlabel = "";
    //                widthlabel = string.Format("col-md-{0} col-sm-{0} col-xs-3", rfun.WITHDLABEL.Value);
    //                var vrs = db.Database.SqlQuery<tbFORMFUNC>("select f.* from bnle.tbFORMFUNC f where f.MAFUNC = @MAFUNC and HIENTHI = 1 order by THUTUNHAP"
    //                , new SqlParameter("@MAFUNC", tablename)).ToList();
    //                List<DataControl> hsc = new List<DataControl>();
    //                bool newrow = false;
    //                string keytype = "";
    //                int cur = 0;
    //                for (int i = 0; i < vrs.Count; i++)
    //                {
    //                    DataControl h = new DataControl();
    //                    h.widthlabel = widthlabel;
    //                    h.id = "idvb_" + vrs[i].ID;
    //                    h.label = vrs[i].TENTRUONG;
    //                    h.namecol = vrs[i].MATRUONG;
    //                    h.ctype = vrs[i].KIEUDULIEU;
    //                    h.width = string.Format("col-md-{0} col-sm-{0} col-xs-12", vrs[i].WITHDFORM.Value);

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
    //                    h.required = vrs[i].BATBUOC == 1 ? true : false;
    //                    if (vrs[i].RANGBUOC.checkIsNull() != "")
    //                    {
    //                        string wheredir = "";
    //                        if (vrs[i].TUDIEN.checkIsNull() != "")
    //                        {
    //                            wheredir = "where " + vrs[i].TUDIEN;
    //                        }
    //                        string sqldir = string.Format("select {0} as [CODE],{1} as [VALUENAME] from {2} {3}",
    //                        vrs[i].KEYFOR, vrs[i].NAMEFOR, vrs[i].RANGBUOC, wheredir);

    //                        var dirs = db.Database.SqlQuery<DataDirControl>(sqldir).ToList();
    //                        h.dictionary = dirs;
    //                    }
    //                    else
    //                    {
    //                        if (vrs[i].TUDIEN.checkIsNull() != "")
    //                        {
    //                            var dirs = db.Database.SqlQuery<DataDirControl>("select [CODE], [VALUENAME] from [adm].[tbDanhmuc] where LOAIDM = @LOAIDM"
    //                            , new SqlParameter("@LOAIDM", vrs[i].TUDIEN)).ToList();
    //                            h.dictionary = dirs;
    //                        }
    //                        else h.dictionary = new List<DataDirControl>();
    //                    }

    //                    h.values = vrs[i].MACDINH;
    //                    if (rfun.IDKEY == vrs[i].MATRUONG)
    //                    {
    //                        keytype = h.ctype;

    //                    }
    //                    h.disable = false;
    //                    hsc.Add(h);
    //                }
    //                if (ID != "")
    //                {
    //                    StringBuilder sqlcom = new StringBuilder("select *");
    //                    sqlcom.Append(" from " + rfun.TABLENAME + " where " + rfun.IDKEY + " = @" + rfun.IDKEY);
    //                    var dt = new DataTable();
    //                    var conn = db.Database.Connection;
    //                    var connectionState = conn.State;
    //                    try
    //                    {
    //                        if (connectionState != ConnectionState.Open) conn.Open();
    //                        using (var cmd = conn.CreateCommand())
    //                        {
    //                            cmd.CommandText = sqlcom.ToString();
    //                            cmd.CommandType = CommandType.Text;
    //                            switch (keytype)
    //                            {
    //                                case "text": cmd.Parameters.Add(new SqlParameter("@" + rfun.IDKEY, ID)); break;
    //                                case "number": cmd.Parameters.Add(new SqlParameter("@" + rfun.IDKEY, Convert.ToInt32(ID))); break;
    //                                default: cmd.Parameters.Add(new SqlParameter("@" + rfun.IDKEY, ID)); break;
    //                            }
    //                            using (var reader = cmd.ExecuteReader())
    //                            {
    //                                dt.Load(reader);
    //                            }
    //                        }
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        if (connectionState != ConnectionState.Closed) conn.Close();
    //                    }
    //                    finally
    //                    {
    //                        if (connectionState != ConnectionState.Closed) conn.Close();
    //                    }
    //                    if (dt.Rows.Count > 0)
    //                    {
    //                        DataControlCols item = new DataControlCols();
    //                        item.items = new List<DataControlRow>();
    //                        foreach (var j in hsc)
    //                        {
    //                            j.values = dt.Rows[0][j.namecol].ToString();
    //                            j.disable = false;
    //                            if (rfun.IDKEY == j.namecol)
    //                            {
    //                                j.disable = true;
    //                            }

    //                        }
    //                    }
    //                }

    //                return hsc.AsEnumerable();
    //            }
    //            return (new List<DataControl>()).AsEnumerable();
    //        }
    //        catch
    //        {
    //            return (new List<DataControl>()).AsEnumerable();
    //        }
    //    }
    //    [HttpPost]
    //    [Route("savenewdata")]
    //    public IHttpActionResult savenewdata(DataSaveControl par)
    //    {
    //        try
    //        {
    //            Database.dbOAMSEntities db = new Database.dbOAMSEntities();
    //            string tablename = par.Tablename;
    //            tablename = CryptData.querydecrypt(tablename);
    //            var rfun = db.Database.SqlQuery<tbFunction>("select f.* from bnle.tbFunction f where f.MAFUNC = @MAFUNC and KICHTHOAT = 1"
    //               , new SqlParameter("@MAFUNC", tablename)).FirstOrDefault();
    //            if (rfun != null)
    //            {
    //                //var vrs = db.Database.SqlQuery<tbFORMFUNC>("select f.* from bnle.tbFORMFUNC f where f.MAFUNC = @MAFUNC and HIENTHI = 1 order by THUTUNHAP"
    //                //, new SqlParameter("@MAFUNC", tablename)).ToList();

    //                StringBuilder sqlcom = new StringBuilder("insert into " + rfun.TABLENAME + "( ");
    //                StringBuilder sqlpar = new StringBuilder(" values( ");
    //                SqlParameter[] sqlp = new SqlParameter[par.items.Count];
    //                for (int i = 0; i < par.items.Count; i++)
    //                {
    //                    sqlcom.Append(par.items[i].namecol);
    //                    sqlpar.Append("@" + par.items[i].namecol);
    //                    if ((i < par.items.Count - 1))
    //                    {
    //                        sqlcom.Append(",");
    //                        sqlpar.Append(",");
    //                    }
    //                    switch (par.items[i].ctype)
    //                    {
    //                        case "text":
    //                            sqlp[i] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values.checkIsNull()); break;
    //                        case "number":
    //                            if (par.items[i].values != null)
    //                                sqlp[i] = new SqlParameter("@" + par.items[i].namecol, Convert.ToInt32(par.items[i].values));
    //                            else sqlp[i] = new SqlParameter("@" + par.items[i].namecol, DBNull.Value);
    //                            break;
    //                        case "select":
    //                            sqlp[i] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values); break;
    //                        case "textarea":
    //                            sqlp[i] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values); break;
    //                        case "autocomplete":
    //                            sqlp[i] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values); break;
    //                        case "calendar":
    //                            if (par.items[i].values != null)
    //                                sqlp[i] = new SqlParameter("@" + par.items[i].namecol, Convert.ToDateTime(par.items[i].values));
    //                            else sqlp[i] = new SqlParameter("@" + par.items[i].namecol, DBNull.Value);
    //                            break;
    //                        case "check":
    //                            if (par.items[i].values != null)
    //                                sqlp[i] = new SqlParameter("@" + par.items[i].namecol, Convert.ToBoolean(par.items[i].values));
    //                            else sqlp[i] = new SqlParameter("@" + par.items[i].namecol, DBNull.Value);
    //                            break;
    //                        default:
    //                            sqlp[i] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values); break;
    //                    }

    //                }
    //                sqlcom.Append(")");
    //                sqlpar.Append(")");
    //                sqlcom.Append(sqlpar);
    //                db.Database.ExecuteSqlCommand(sqlcom.ToString(), sqlp);
    //            }
    //            return Ok(1);
    //        }
    //        catch
    //        {
    //            return BadRequest();
    //        }
    //    }
    //    [HttpPost]
    //    [Route("deletedata")]
    //    public IHttpActionResult deletedata(DataParnew par)
    //    {
    //        try
    //        {
    //            Database.dbOAMSEntities db = new Database.dbOAMSEntities();
    //            string tablename = par.Tablename;
    //            tablename = CryptData.querydecrypt(tablename);
    //            string ID = par.id.checkIsNull();
    //            var rfun = db.Database.SqlQuery<tbFunction>("select f.* from bnle.tbFunction f where f.MAFUNC = @MAFUNC and KICHTHOAT = 1"
    //               , new SqlParameter("@MAFUNC", tablename)).FirstOrDefault();
    //            if (rfun != null)
    //            {
    //                if (ID != "")
    //                {
    //                    StringBuilder sqlcom = new StringBuilder("");
    //                    sqlcom.Append("delete " + rfun.TABLENAME + " where " + rfun.IDKEY + " = @" + rfun.IDKEY);
    //                    var vrs = db.Database.SqlQuery<tbFORMFUNC>("select f.* from bnle.tbFORMFUNC f where f.MAFUNC = @MAFUNC and MATRUONG = @MATRUONG "
    //                    , new SqlParameter("@MAFUNC", tablename), new SqlParameter("@MATRUONG", rfun.IDKEY)).ToList();
    //                    string keytype = "";
    //                    for (int i = 0; i < vrs.Count; i++)
    //                    {
    //                        if (rfun.IDKEY == vrs[i].MATRUONG)
    //                        {
    //                            keytype = vrs[i].KIEUDULIEU; break;
    //                        }
    //                    }
    //                    SqlParameter parsql;
    //                    switch (keytype)
    //                    {
    //                        case "text": parsql = new SqlParameter("@" + rfun.IDKEY, ID); break;
    //                        case "number": parsql = new SqlParameter("@" + rfun.IDKEY, Convert.ToInt32(ID)); break;
    //                        default: parsql = new SqlParameter("@" + rfun.IDKEY, ID); break;
    //                    }
    //                    db.Database.ExecuteSqlCommand(sqlcom.ToString(), parsql);
    //                }
    //            }
    //            return Ok(1);
    //        }
    //        catch
    //        {
    //            return BadRequest();
    //        }
    //    }
    //    [HttpPost]
    //    [Route("saveeditdata")]
    //    public IHttpActionResult saveeditdata(DataSaveControl par)
    //    {
    //        try
    //        {
    //            Database.dbOAMSEntities db = new Database.dbOAMSEntities();
    //            string tablename = par.Tablename;
    //            tablename = CryptData.querydecrypt(tablename);
    //            string ID = par.id.checkIsNull();
    //            var rfun = db.Database.SqlQuery<tbFunction>("select f.* from bnle.tbFunction f where f.MAFUNC = @MAFUNC and KICHTHOAT = 1"
    //               , new SqlParameter("@MAFUNC", tablename)).FirstOrDefault();
    //            if (rfun != null)
    //            {
    //                //var vrs = db.Database.SqlQuery<tbFORMFUNC>("select f.* from bnle.tbFORMFUNC f where f.MAFUNC = @MAFUNC and HIENTHI = 1 order by THUTUNHAP"
    //                //, new SqlParameter("@MAFUNC", tablename)).ToList();
    //                string keytype = "";
    //                StringBuilder sqlcom = new StringBuilder("update " + rfun.TABLENAME + " set ");
    //                int countpar = 0;
    //                if (rfun.IDKEY == "ID") countpar = par.items.Count + 1;
    //                else countpar = par.items.Count;
    //                SqlParameter[] sqlp = new SqlParameter[countpar];
    //                int k = 0;
    //                for (int i = 0; i < par.items.Count; i++)
    //                {
    //                    if (rfun.IDKEY != par.items[i].namecol)
    //                    {
    //                        sqlcom.Append(par.items[i].namecol + " = @" + par.items[i].namecol);
    //                        if ((i < par.items.Count - 1))
    //                        {
    //                            sqlcom.Append(",");
    //                        }
    //                        switch (par.items[i].ctype)
    //                        {
    //                            case "text":
    //                                sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values.checkIsNull()); break;
    //                            case "number":
    //                                if (par.items[i].values != null)
    //                                    sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, Convert.ToInt32(par.items[i].values));
    //                                else sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, DBNull.Value);
    //                                break;
    //                            case "select":
    //                                sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values); break;
    //                            case "textarea":
    //                                sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values); break;
    //                            case "autocomplete":
    //                                sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values); break;
    //                            case "calendar":
    //                                if (par.items[i].values != null)
    //                                    sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, Convert.ToDateTime(par.items[i].values));
    //                                else sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, DBNull.Value);
    //                                break;
    //                            case "check":
    //                                if (par.items[i].values != null)
    //                                    sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, Convert.ToBoolean(par.items[i].values));
    //                                else sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, DBNull.Value);
    //                                break;
    //                            default:
    //                                sqlp[k++] = new SqlParameter("@" + par.items[i].namecol, par.items[i].values); break;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        keytype = par.items[i].ctype;
    //                    }
    //                }
    //                sqlcom.Append(" where " + rfun.IDKEY + " = @" + rfun.IDKEY);
    //                switch (keytype)
    //                {
    //                    case "text": sqlp[k] = new SqlParameter("@" + rfun.IDKEY, ID); break;
    //                    case "number": sqlp[k] = new SqlParameter("@" + rfun.IDKEY, Convert.ToInt32(ID)); break;
    //                    default: sqlp[k] = new SqlParameter("@" + rfun.IDKEY, ID); break;
    //                }
    //                db.Database.ExecuteSqlCommand(sqlcom.ToString(), sqlp);
    //            }
    //            return Ok(1);
    //        }
    //        catch
    //        {
    //            return BadRequest();
    //        }
    //    }
    //}
}