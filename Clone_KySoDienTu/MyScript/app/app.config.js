/*********************************************************
 * APP CONFIGURATION
 *********************************************************/

var appName = '';
var baseURL = window.location.protocol + '//' + window.location.host + '/' + appName;

/*********************************************************
 * CHỌN MÔI TRƯỜNG Ở ĐÂY
 * LOCAL | BETA | RELEASE
 *********************************************************/
var ENV = 'LOCAL';

/*********************************************************
 * ENV DEFINITIONS
 *********************************************************/
var ENV_CONFIG = {

    LOCAL: {
        serverPath: baseURL,
        serverHome: 'DonNghiPhep/IndexNghiPhep',
        serverLogin: 'Login/AuthLogin',

        dragAndDropSignUIUrl: 'http://localhost:3000/?id=',

        typeNghiPhep: 'TypeRP_NPP',

        apiSmartCA: {
            BASE_URL: 'https://localhost:44381/api/',
            TRUC_CONG_VAN_URL: 'http://localhost:8080/',

            GET_USER_BY_USERNAME: 'userSmartCA/get-user',
            CREATE_UPDATE_USER: 'userSmartCA/create-update-user',
            CREATE_SIGN: 'smartca/create-sign',

            SAVE_FILE: {
                CONG_VAN: 'smartca/SaveFileSignCongVan',
                VAN_BAN_KY_SO: 'smartca/SaveFileSignVanBanKySo',
                PHIEU_XIN_XE: 'smartca/SaveFileSignPhieuDeNghiSuDungXe',
                LENH_DIEU_XE: 'smartca/SaveFileSignLenhDieuXe',
                QL_LENH_DIEU_XE: 'smartca/SaveFileSignQuanLyLenhDieuXe',
                UNG_TRUOC_NHIEN_LIEU: 'smartca/SaveFileSignUngTruocNhienLieu',
                QUYET_TOAN_NHIEN_LIEU: 'smartca/SaveFileSignQuyetToanNhienLieu',
                SO_LO_TRINH: 'smartca/SaveFileSignSoLoTrinh',
                DON_NGHI_PHEP: 'smartca/SaveFileSignDonNghiPhep',
                QL_CHAM_CONG: 'smartca/SaveFileSignQuanLyChamCong'
            }
        }
    },

    BETA: {
        serverPath: baseURL,
        serverHome: 'Home/Index',
        serverLogin: 'Login/AuthLogin',

        dragAndDropSignUIUrl: 'https://betachukyso.capnuocnhabe.vn/?id=',

        typeNghiPhep: 'TypeRP_NPP',

        apiSmartCA: {
            BASE_URL: 'https://betavpdt.capnuocnhabe.vn/api/',
            TRUC_CONG_VAN_URL: 'http://192.168.72.82:8080/hellospringboot-0.0.1-SNAPSHOT/',

            GET_USER_BY_USERNAME: 'userSmartCA/get-user',
            CREATE_UPDATE_USER: 'userSmartCA/create-update-user',
            CREATE_SIGN: 'smartca/create-sign',

            SAVE_FILE: {
                CONG_VAN: 'smartca/SaveFileSignCongVan',
                VAN_BAN_KY_SO: 'smartca/SaveFileSignVanBanKySo',
                PHIEU_XIN_XE: 'smartca/SaveFileSignPhieuDeNghiSuDungXe',
                LENH_DIEU_XE: 'smartca/SaveFileSignLenhDieuXe',
                QL_LENH_DIEU_XE: 'smartca/SaveFileSignQuanLyLenhDieuXe',
                UNG_TRUOC_NHIEN_LIEU: 'smartca/SaveFileSignUngTruocNhienLieu',
                QUYET_TOAN_NHIEN_LIEU: 'smartca/SaveFileSignQuyetToanNhienLieu',
                SO_LO_TRINH: 'smartca/SaveFileSignSoLoTrinh',
                DON_NGHI_PHEP: 'smartca/SaveFileSignDonNghiPhep',
                QL_CHAM_CONG: 'smartca/SaveFileSignQuanLyChamCong'
            }
        }
    },

    RELEASE: {
        serverPath: baseURL,
        serverHome: 'Home/Index',
        serverLogin: 'Login/AuthLogin',

        dragAndDropSignUIUrl: 'https://chukyso.capnuocnhabe.vn/?id=',

        typeNghiPhep: 'TypeRP_NPP',

        apiSmartCA: {
            BASE_URL: 'https://vpdt.capnuocnhabe.vn/api/',
            TRUC_CONG_VAN_URL: 'http://192.168.72.97:8080/hellospringboot-0.0.1-SNAPSHOT/',

            GET_USER_BY_USERNAME: 'userSmartCA/get-user',
            CREATE_UPDATE_USER: 'userSmartCA/create-update-user',
            CREATE_SIGN: 'smartca/create-sign',

            SAVE_FILE: {
                CONG_VAN: 'smartca/SaveFileSignCongVan',
                VAN_BAN_KY_SO: 'smartca/SaveFileSignVanBanKySo',
                PHIEU_XIN_XE: 'smartca/SaveFileSignPhieuDeNghiSuDungXe',
                LENH_DIEU_XE: 'smartca/SaveFileSignLenhDieuXe',
                QL_LENH_DIEU_XE: 'smartca/SaveFileSignQuanLyLenhDieuXe',
                UNG_TRUOC_NHIEN_LIEU: 'smartca/SaveFileSignUngTruocNhienLieu',
                QUYET_TOAN_NHIEN_LIEU: 'smartca/SaveFileSignQuyetToanNhienLieu',
                SO_LO_TRINH: 'smartca/SaveFileSignSoLoTrinh',
                DON_NGHI_PHEP: 'smartca/SaveFileSignDonNghiPhep',
                QL_CHAM_CONG: 'smartca/SaveFileSignQuanLyChamCong'
            }
        }
    }
};

/*********************************************************
 * EXPORT TO ANGULAR
 *********************************************************/
angular
    .module('aims.core')
    .constant('appSettings', ENV_CONFIG[ENV]);

/*********************************************************
 * BACKWARD COMPATIBILITY (ADAPTER FOR OLD CODE)
 *********************************************************/
angular
    .module('aims.core')
    .factory('Settings', function (appSettings) {

        var api = appSettings.apiSmartCA;

        return {
            serverPath: appSettings.serverPath,
            serverHome: appSettings.serverHome,
            serverLogin: appSettings.serverLogin,
            serverCreateSign: appSettings.serverCreateSign,
            typeNghiPhep: appSettings.typeNghiPhep,

            apiSmartCA_ServerPath: api.BASE_URL,
            apiSmartCA_ServerTrucCongVanPath: api.TRUC_CONG_VAN_URL,
            apiSmartCA_GetUserByUserName: api.GET_USER_BY_USERNAME,
            apiSmartCA_CreateUpdateUser: api.CREATE_UPDATE_USER,
            apiSmartCA_CreateSign: api.CREATE_SIGN,

            serverSaveFileSign_CongVan: api.BASE_URL + api.SAVE_FILE.CONG_VAN,
            serverSaveFileSign_BaoCao: api.BASE_URL + api.SAVE_FILE.BAO_CAO,
            serverSaveFileSign_PhieuXinXe: api.BASE_URL + api.SAVE_FILE.PHIEU_XIN_XE,
            serverSaveFileSign_LenhDieuXe: api.BASE_URL + api.SAVE_FILE.LENH_DIEU_XE,
            serverSaveFileSign_QuanLyLenhDieuXe: api.BASE_URL + api.SAVE_FILE.QL_LENH_DIEU_XE,
            serverSaveFileSign_UngTruocNhienLieu: api.BASE_URL + api.SAVE_FILE.UNG_TRUOC_NHIEN_LIEU,
            serverSaveFileSign_QuyetToanNhienLieu: api.BASE_URL + api.SAVE_FILE.QUYET_TOAN_NHIEN_LIEU,
            serverSaveFileSign_SoLoTrinh: api.BASE_URL + api.SAVE_FILE.SO_LO_TRINH,
            serverSaveFileSign_DonNghiPhep: api.BASE_URL + api.SAVE_FILE.DON_NGHI_PHEP,
            serverSaveFileSign_QuanLyChamCong: api.BASE_URL + api.SAVE_FILE.QL_CHAM_CONG
        };
    });