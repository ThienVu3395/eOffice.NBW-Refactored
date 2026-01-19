var appname = "";
var baseURL = window.location.protocol + '//' + window.location.host + "/" + appname;

// Setting for beta
//var Settings = {
//    serverPath: baseURL,
//    serverHome: "Home/Index",
//    serverCreateSign: "https://betachukyso.capnuocnhabe.vn/?id=",
//    serverSaveFileSign_CongVan: "https://betavpdt.capnuocnhabe.vn/api/smartca/SaveFileSignCongVan",
//    serverSaveFileSign_BaoCao: "https://betavpdt.capnuocnhabe.vn/api/smartca/SaveFileSignBaoCao",
//    serverSaveFileSign_PhieuXinXe: "https://betavpdt.capnuocnhabe.vn/api/smartca/SaveFileSignPhieuDeNghiSuDungXe",
//    serverSaveFileSign_LenhDieuXe: "https://betavpdt.capnuocnhabe.vn/api/smartca/SaveFileSignLenhDieuXe",
//    serverSaveFileSign_QuanLyLenhDieuXe: "https://betavpdt.capnuocnhabe.vn/api/smartca/SaveFileSignQuanLyLenhDieuXe",
//    serverSaveFileSign_UngTruocNhienLieu: "https://betavpdt.capnuocnhabe.vn/api/smartca/SaveFileSignUngTruocNhienLieu",
//    serverSaveFileSign_QuyetToanNhienLieu: "https://betavpdt.capnuocnhabe.vn/api/smartca/SaveFileSignQuyetToanNhienLieu",
//    serverSaveFileSign_SoLoTrinh: "https://betavpdt.capnuocnhabe.vn/api/smartca/SaveFileSignSoLoTrinh",
//    serverSaveFileSign_DonNghiPhep: "https://betavpdt.capnuocnhabe.vn/api/smartca/SaveFileSignDonNghiPhep",
//    serverSaveFileSign_QuanLyChamCong: "https://betavpdt.capnuocnhabe.vn/api/smartca/SaveFileSignQuanLyChamCong",
//    apiSmartCA_ServerPath: "https://betavpdt.capnuocnhabe.vn/api/",
//    apiSmartCA_ServerTrucCongVanPath: "http://192.168.72.82:8080/hellospringboot-0.0.1-SNAPSHOT/",
//    apiSmartCA_GetUserByUserName: "userSmartCA/get-user",
//    apiSmartCA_CreateUpdateUser: "userSmartCA/create-update-user",
//    apiSmartCA_CreateSign: "smartca/create-sign",
//    serverLogin: "Login/AuthLogin",
//    serverApp: appname,
//    typeNghiPhep: "TypeRP_NPP"
//};

// Setting for release
//var Settings = {
//    serverPath: baseURL,
//    serverHome: "Home/Index",
//    serverCreateSign: "https://chukyso.capnuocnhabe.vn/?id=",
//    serverSaveFileSign_CongVan: "https://vpdt.capnuocnhabe.vn/api/smartca/SaveFileSignCongVan",
//    serverSaveFileSign_BaoCao: "https://vpdt.capnuocnhabe.vn/api/smartca/SaveFileSignBaoCao",
//    serverSaveFileSign_PhieuXinXe: "https://vpdt.capnuocnhabe.vn/api/smartca/SaveFileSignPhieuDeNghiSuDungXe",
//    serverSaveFileSign_LenhDieuXe: "https://vpdt.capnuocnhabe.vn/api/smartca/SaveFileSignLenhDieuXe",
//    serverSaveFileSign_QuanLyLenhDieuXe: "https://vpdt.capnuocnhabe.vn/api/smartca/SaveFileSignQuanLyLenhDieuXe",
//    serverSaveFileSign_UngTruocNhienLieu: "https://vpdt.capnuocnhabe.vn/api/smartca/SaveFileSignUngTruocNhienLieu",
//    serverSaveFileSign_QuyetToanNhienLieu: "https://vpdt.capnuocnhabe.vn/api/smartca/SaveFileSignQuyetToanNhienLieu",
//    serverSaveFileSign_SoLoTrinh: "https://vpdt.capnuocnhabe.vn/api/smartca/SaveFileSignSoLoTrinh",
//    serverSaveFileSign_DonNghiPhep: "https://vpdt.capnuocnhabe.vn/api/smartca/SaveFileSignDonNghiPhep",
//    serverSaveFileSign_QuanLyChamCong: "https://vpdt.capnuocnhabe.vn/api/smartca/SaveFileSignQuanLyChamCong",
//    apiSmartCA_ServerPath: "https://vpdt.capnuocnhabe.vn/api/",
//    apiSmartCA_ServerTrucCongVanPath: "http://192.168.72.97:8080/hellospringboot-0.0.1-SNAPSHOT/",
//    apiSmartCA_GetUserByUserName: "userSmartCA/get-user",
//    apiSmartCA_CreateUpdateUser: "userSmartCA/create-update-user",
//    apiSmartCA_CreateSign: "smartca/create-sign",
//    serverLogin: "Login/AuthLogin",
//    serverApp: appname,
//    typeNghiPhep: "TypeRP_NPP"
//};

// Setting for localhost
var Settings = {
    serverPath: baseURL,
    serverHome: "DonNghiPhep/IndexNghiPhep",
    serverCreateSign: "https://betachukyso.capnuocnhabe.vn/?id=",
    serverSaveFileSign_CongVan : "http://localhost:44381/api/smartca/SaveFileSignCongVan",
    serverSaveFileSign_BaoCao: "http://localhost:44381/api/smartca/SaveFileSignBaoCao",
    serverSaveFileSign_PhieuXinXe: "http://localhost:44381/api/smartca/SaveFileSignPhieuDeNghiSuDungXe",
    serverSaveFileSign_LenhDieuXe: "http://localhost:44381/api/smartca/SaveFileSignLenhDieuXe",
    serverSaveFileSign_QuanLyLenhDieuXe: "http://localhost:44381/api/smartca/SaveFileSignQuanLyLenhDieuXe",
    serverSaveFileSign_UngTruocNhienLieu: "http://localhost:44381/api/smartca/SaveFileSignUngTruocNhienLieu",
    serverSaveFileSign_QuyetToanNhienLieu: "http://localhost:44381/api/smartca/SaveFileSignQuyetToanNhienLieu",
    serverSaveFileSign_SoLoTrinh: "http://localhost:44381/api/smartca/SaveFileSignSoLoTrinh",
    serverSaveFileSign_DonNghiPhep: "http://localhost:44381/api/smartca/SaveFileSignDonNghiPhep",
    serverSaveFileSign_QuanLyChamCong: "http://localhost:44381/api/smartca/SaveFileSignQuanLyChamCong",
    apiSmartCA_ServerPath: "http://localhost:44381/api/",
    apiSmartCA_ServerTrucCongVanPath: "http://localhost:8080/",
    apiSmartCA_GetUserByUserName: "userSmartCA/get-user",
    apiSmartCA_CreateUpdateUser: "userSmartCA/create-update-user",
    apiSmartCA_CreateSign: "smartca/create-sign",
    serverLogin: "Login/AuthLogin",
    serverApp: appname,
    typeNghiPhep: "TypeRP_NPP"
};

var SteTiny = {
    onChange: function (e) {
    },
    menubar: false,
    resize: false,
    toolbar: 'undo redo | bold italic underline | forecolor bullist',
    plugins: 'textcolor advlist',
    skin: 'lightgray',
    theme: 'modern',
    content_css: "/Scripts/tinymce/tinyMod.css"
};