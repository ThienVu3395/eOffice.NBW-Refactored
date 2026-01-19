using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clone_KySoDienTu.Controllers.BNLE
{
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
}