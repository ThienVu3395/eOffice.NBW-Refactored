var appName = '';
var baseURL = window.location.protocol + '//' + window.location.host + '/' + appName;

// =============================
// CHỌN MÔI TRƯỜNG Ở ĐÂY
// =============================

var ENV = 'LOCAL'; // LOCAL | BETA | RELEASE

// =============================
// ENV DEFINITIONS
// =============================

var ENV_CONFIG = {
    LOCAL: {
        serverPath: baseURL,
        serverHome: 'DonNghiPhep/IndexNghiPhep',
        serverCreateSign: 'https://betachukyso.capnuocnhabe.vn/?id=',

        apiSmartCA: {
            BASE_URL: 'http://localhost:44381/api/',
            TRUC_CONG_VAN_URL: 'http://localhost:8080/',
            GET_USER_BY_USERNAME: 'userSmartCA/get-user',
            CREATE_UPDATE_USER: 'userSmartCA/create-update-user',
            CREATE_SIGN: 'smartca/create-sign',
            SAVE_FILE: {
                CONG_VAN: 'smartca/SaveFileSignCongVan',
                BAO_CAO: 'smartca/SaveFileSignBaoCao',
                PHIEU_XIN_XE: 'smartca/SaveFileSignPhieuDeNghiSuDungXe',
                LENH_DIEU_XE: 'smartca/SaveFileSignLenhDieuXe',
                QL_LENH_DIEU_XE: 'smartca/SaveFileSignQuanLyLenhDieuXe',
                UNG_TRUOC_NHIEN_LIEU: 'smartca/SaveFileSignUngTruocNhienLieu',
                QUYET_TOAN_NHIEN_LIEU: 'smartca/SaveFileSignQuyetToanNhienLieu',
                SO_LO_TRINH: 'smartca/SaveFileSignSoLoTrinh',
                DON_NGHI_PHEP: 'smartca/SaveFileSignDonNghiPhep',
                QL_CHAM_CONG: 'smartca/SaveFileSignQuanLyChamCong'
            }
        },

        serverLogin: 'Login/AuthLogin',
        typeNghiPhep: 'TypeRP_NPP'
    },

    BETA: {
        serverPath: baseURL,
        serverHome: 'Home/Index',
        serverCreateSign: 'https://betachukyso.capnuocnhabe.vn/?id=',
        apiSmartCA: {
            BASE_URL: 'https://betavpdt.capnuocnhabe.vn/api/',
            TRUC_CONG_VAN_URL: 'http://192.168.72.82:8080/hellospringboot-0.0.1-SNAPSHOT/'
        },
        serverLogin: 'Login/AuthLogin',
        typeNghiPhep: 'TypeRP_NPP'
    },

    RELEASE: {
        serverPath: baseURL,
        serverHome: 'Home/Index',
        serverCreateSign: 'https://chukyso.capnuocnhabe.vn/?id=',
        apiSmartCA: {
            BASE_URL: 'https://vpdt.capnuocnhabe.vn/api/',
            TRUC_CONG_VAN_URL: 'http://192.168.72.97:8080/hellospringboot-0.0.1-SNAPSHOT/'
        },
        serverLogin: 'Login/AuthLogin',
        typeNghiPhep: 'TypeRP_NPP'
    }
};

// =============================
// EXPORT TO ANGULAR
// =============================

angular
    .module('aims.core')
    .constant('appSettings', ENV_CONFIG[ENV]);