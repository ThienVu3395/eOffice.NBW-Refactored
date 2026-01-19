using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clone_KySoDienTu.Models
{
    public class ReportParams<T>
    {
        public string ReportFileName { get; set; }
        public string ReportTitle { get; set; }
        public List<T> DataSource { get; set; }
        public bool IsPassParamToCr { get; set; } = false;
        public int IDVB { get; set; }
        public Int64 IDCR { get; set; }
        public int Loai { get; set; } = 0;
    }

    public class ReportParamsWithMultipleValue<T>
    {
        public string ReportFileName { get; set; }
        public string ReportTitle { get; set; }
        public List<T> DataSource { get; set; }
        public bool IsPassParamToCr { get; set; } = false;
        public int IDVB { get; set; }
        public Int64 IDCR { get; set; }
        public int Loai { get; set; } = 0;
        public string valstring1 { get; set; } = "";
        public string valstring2 { get; set; } = "";
        public string valstring3 { get; set; } = "";
        public string valstring4 { get; set; } = "";
        public string valstring5 { get; set; } = "";
        public string valstring6 { get; set; } = "";
        public int valint1 { get; set; }
        public int valint2 { get; set; }
        public int valint3 { get; set; }
        public int valint4 { get; set; }
        public int valint5 { get; set; }
        public DateTime valdate1 { get; set; }
        public DateTime valdate2 { get; set; }
        public DateTime valdate3 { get; set; }
        public DateTime valdate4 { get; set; }
    }
}