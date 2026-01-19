using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clone_KySoDienTu.Models.Bnle
{
    public class ModelPar
    {
    }
    public class DataParnew
    {
        public string Tablename { get; set; }
        public string id { get; set; }
    }
    public class DataDirControl
    {
        public string CODE { get; set; }
        public string VALUENAME { get; set; }
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
    public class DataGridModel
    {
        public string formname { get; set; }
        public string viewmode { get; set; }
        public int TotalItems { get; set; }
        public int PerPage { get; set; }
        public List<DataControlCol> datacols { get; set; }
        public List<DataControlCols> datarows { get; set; }

    }
    public class DataGridFModel
    {
        public string formname { get; set; }
        public string widthfilter { get; set; }
        public string widthgrid { get; set; }

        public string tablename { get; set; }
        public string viewmode { get; set; }
        public List<DataDirControl> datafilter { get; set; }

    }

}