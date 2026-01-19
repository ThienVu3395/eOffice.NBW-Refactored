using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clone_KySoDienTu.Models
{
    public partial class tbBophanModel
    {

        public string MAPHONG { get; set; }
        public string TENPHONG { get; set; }
        public string DVQUANLY { get; set; }
        public string MADONVI { get; set; }

    }
    public class abntreeModelData
    {
        public string description { get; set; }
    }
    public class abntreeModel
    {
        public string label { get; set; }
        public string id { get; set; }
        public abntreeModelData data { get; set; }
        public List<abntreeModel> children { get; set; }
    }
    public class Nhanvien
    {
    }
    public partial class Nhanvienmodel
    {
        public string ID { get; set; }
        public string USERNAME { get; set; }
        public string HOTEN { get; set; }
        public string EMAIL { get; set; }
        public string FILEHINH { get; set; }
        public int KHOA { get; set; }


    }
    public partial class UserModel
    {
        public string ID { get; set; }
        public string USERNAME { get; set; }
        public string EMAIL { get; set; }
        public string HOLOT { get; set; }
        public string TEN { get; set; }
        public System.DateTime NGAYTAO { get; set; }
        public bool KHOA { get; set; }
        public string CHUCVU { get; set; }
        public string BOPHAN { get; set; }
        public string UYQUYEN { get; set; }
        public Nullable<System.DateTime> NGAYUQ { get; set; }
        public Nullable<System.DateTime> KETTHUCUQ { get; set; }
        public bool HANCHE { get; set; }
        public string FILEANH { get; set; }

        public Nullable<bool> CHECKED { get; set; }
        public Nullable<int> IDGROUP { get; set; }
    }
    public partial class UserModelEdit
    {
        public string ID { get; set; }
        public string USERNAME { get; set; }
        public string EMAIL { get; set; }
        public string HOLOT { get; set; }
        public string MATKHAU { get; set; }
        public string TEN { get; set; }
        public System.DateTime NGAYTAO { get; set; }
        public bool KHOA { get; set; }
        public string CHUCVU { get; set; }
        public string BOPHAN { get; set; }
        public string UYQUYEN { get; set; }
        public Nullable<System.DateTime> NGAYUQ { get; set; }
        public Nullable<System.DateTime> KETTHUCUQ { get; set; }
        public bool HANCHE { get; set; }
        public string FILEANH { get; set; }
    }
    public class tbNhanvienModels
    {
        public string MANHOM { get; set; }
        public int TotalItems { get; set; }

        public int perPage { get; set; }
        public List<UserModel> pdata { get; set; }
    }
    public partial class tbNhomModel
    {
        public string MANHOM { get; set; }
        public string TENNHOM { get; set; }
    }
    public class tbNhomModels
    {
        public int TotalItems { get; set; }

        public int perPage { get; set; }
        public List<tbNhomModel> pdata { get; set; }
    }
    public partial class tbQuyenModel
    {
        public List<tbNhomModel> pchucnang { get; set; }
        public List<tbNhomChucnangModel> pnhom { get; set; }
    }
    public partial class tbChucnangModel
    {
        public int ID { get; set; }
        public int NHOMID { get; set; }
        public string TENCHUCNANG { get; set; }
        public string LINKS { get; set; }
        //public Nullable<bool> CHECKED { get; set; }
        //public Nullable<int> IDGROUP { get; set; }
    }
    public partial class tbNhomChucnangModel
    {
        public int ID { get; set; }
        public int MODULAID { get; set; }
        public int PARENTID { get; set; }
        public string TEN { get; set; }
        public string ICON { get; set; }
        public Nullable<int> THUTU { get; set; }
        public string LINKS { get; set; }
        public int TYPE { get; set; }

    }
    public partial class tbMenuChucNang
    {
        public tbNhomChucnangModel par { get; set; }
        public List<tbMenuChucNang> childitem { get; set; }
    }
    public partial class tbChucnangGroup
    {
        public string NHOMNAME { get; set; }
        public int NHOMID { get; set; }
    }
    public partial class tbChucnangGroupModel
    {
        public string MANHOM { get; set; }
        public List<tbChucnangGroup> pgroup { get; set; }
        public List<tbChucnangModel> pdata { get; set; }
    }

    public class ParamModel
    {
        public string valstring1 { get; set; }
        public string valstring2 { get; set; }
        public string valstring3 { get; set; }
        public string valstring4 { get; set; }
        public int valint1 { get; set; }
        public int valint2 { get; set; }
        public int valint3 { get; set; }
        public int valint4 { get; set; }
    }
}