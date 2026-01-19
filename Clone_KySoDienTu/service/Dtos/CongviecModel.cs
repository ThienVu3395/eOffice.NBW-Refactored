using Clone_KySoDienTu.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clone_KySoDienTu.Controllers.Congviec
{
    public class NhanvienCV
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string BOPHAN { get; set; }
    }
    public class NhanvienCVNV
    {
        public int ID { get; set; }
        public int IDCV { get; set; }
        public string UserName { get; set; }
        public int VAITRO { get; set; }
        public string FullName { get; set; }
        public string BOPHAN { get; set; }
    }
    //Model GiaHan
    public class GiaHan
    {
        public int ID { get; set; }
        public int WorkflowID { get; set; }
        public Nullable<System.DateTime> HOANTHANH { get; set; }
        public Nullable<System.DateTime> HOANTHANH2 { get; set; }
        public string NGUOIGIAHAN { get; set; }
        public Nullable<System.DateTime> NGAYGIAHAN { get; set; }
        public string NGUOIDUYET { get; set; }
        public Nullable<System.DateTime> NGAYDUYET { get; set; }
        public string LYDO { get; set; }
        public string YKIEN { get; set; }
        public int TRANGTHAI { get; set; }
        public string GHICHU { get; set; }
    }

    public class GiaHanNew
    {
        public int ID { get; set; }
        public int WorkflowID { get; set; }
        public string HOANTHANH { get; set; }
        public string HOANTHANH2 { get; set; }
        public string NGUOIGIAHAN { get; set; }
        public string NGAYGIAHAN { get; set; }
        public string NGUOIDUYET { get; set; }
        public string NGAYDUYET { get; set; }
        public string LYDO { get; set; }
        public string YKIEN { get; set; }
        public int TRANGTHAI { get; set; }
        public string GHICHU { get; set; }
    }

    //Model Add Cong Viec
    public class CongviecNew
    {
        public CongviecModel par { get; set; }
        public List<tbFiledinhkemDto> listfile { get; set; }
        public List<NhanvienCVNV> dsnhanvien { get; set; }
    }
    public class CongviecModel
    {
        public int ID { get; set; }
        public string MACV { get; set; }
        public string TENCV { get; set; }
        public string MOTACV { get; set; }
        public string GHICHU { get; set; }
        public int LOAICV { get; set; }
        public int TRANGTHAI { get; set; }
        public string NGUOITAO { get; set; }
        public string THOIHAN { get; set; }
        public string MADONVI { get; set; }
        public string BOPHAN { get; set; }
        public Nullable<int> VANBANID { get; set; }
        public Nullable<bool> KHOAXL { get; set; }
        public string KETQUAXL { get; set; }
        public Nullable<System.DateTime> NGAYHOANTHANH { get; set; }
        public Nullable<int> IDPARENT { get; set; }
        public string NGAYBD { get; set; }
        public int IDVANBANDEN { get; set; }
        public bool isChecked { get; set; }
    }
    
    //Model dsCongViec
    public class Congviec
    {
        public int Total { get; set; }
        public int ID { get; set; }
        public string TENCV { get; set; }
        public int LOAICV { get; set; }
        public int VAITRO { get; set; }
        public string ChildID { get; set; }
        public Nullable<System.DateTime> NGAYBD { get; set; }
        public Nullable<System.DateTime> THOIHAN { get; set; }
        public Nullable<System.DateTime> NGAYGH { get; set; }
        public int TRANGTHAI { get; set; }
        public int DAXEMALL { get; set; }
        public string DAXEM { get; set; }
        public List<tbFiledinhkemDto> listfile { get; set; }
        public List<NhanvienCVNV> dsnhanvien { get; set; }
    }
    public class dsCongviec
    {
        public List<Congviec> dscongviec { get; set; }
        public int Total { get; set; }
        public int Perpage { get; set; }
        public int Curpage { get; set; }
    }
    
    // Model Chi Tiet Cong Viec
    public class CongviecchitietModel
    {
        public Congviecchitiet cvchitiet { get; set; }
        public List<NhanvienCVNV> dsnhanvien { get; set; }
        public List<tbFiledinhkemDto> listfile { get; set; }
        public List<tbFiledinhkemDto> CVlistfile { get; set; }
        public List<CongviecchitietModel> cvTach { get; set; }
    }
    public class Congviecchitiet
    {
        public int ID { get; set; }
        public string TENCV { get; set; }
        
        public string YEUCAURIENG { get; set; }
        public string MOTACV { get; set; }
        public int VAITRO { get; set; }
        public int GIAHAN { get; set; }
        public int LOAICV { get; set; }
        public Nullable<System.DateTime> NGAYBD { get; set; }
        public Nullable<System.DateTime> THOIHAN { get; set; }
        public Nullable<System.DateTime> NGAYGH { get; set; }
        public string NGUOITAO { get; set; }
        public Nullable<System.DateTime> NGAYTAO { get; set; }
        public string TRANGTHAI { get; set; }
        public bool KHOAXL { get; set; }
        public string KETQUAXL { get; set; }
        public int IDVANBANDEN { get; set; }
        public int HoanThanh { get; set; }
    }


    // Model Add Xu Ly
    public class XuLyNew
    {
        public XuLyModel par { get; set; }
        public List<tbFilexulyDto> listfile { get; set; }
        public List<XuLyNew> phanhoi { get; set; }
        public List<NhanvienCV> dsnvphanhoi { get; set; }
    }
   
    public class XuLyModel
    {
        public int ID { get; set; }
        public int PARENTID { get; set; }
        public int WorkflowID { get; set; }
        public string CANBO { get; set; }
        public string TenCanBo { get; set; }
        public Nullable<System.DateTime> NGAYXL { get; set; }
        public string YKIENXL { get; set; }
        public int TRANGTHAI { get; set; }
        public int LOAIXL { get; set; }
        public bool CONGKHAI { get; set; }
       

    }

    public class ViewfilePDF
    {
        public int ID { get; set; }
        public string TENFILE { get; set; }
        public string MOTA { get; set; }
        public DateTime NGAYTAO { get; set; }
    }
    public class tbFiledinhkemDto
    {
        public int ID { get; set; }
        public int LOAI { get; set; }
        public int VANBANID { get; set; }
        public string TENFILE { get; set; }
        public string MOTA { get; set; }
        public System.DateTime NGAYTAO { get; set; }
        public bool TRANGTHAI { get; set; }
        public string LOAIFILE { get; set; }
        public int SIZEFILE { get; set; }
        public int VITRIID { get; set; }
    }
    public class tbFilexulyDto
    {
        public int ID { get; set; }
        public int YKIENID { get; set; }
        public string TENFILE { get; set; }
        public string MOTA { get; set; }
        public System.DateTime NGAYTAO { get; set; }
        public bool TRANGTHAI { get; set; }
        public string LOAIFILE { get; set; }
        public int SIZEFILE { get; set; }
        public int VITRIID { get; set; }
        public int PHATHANH { get; set; }
    }
}