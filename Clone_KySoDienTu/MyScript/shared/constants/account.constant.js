angular
    .module('aims.shared.constants')
    .constant('SPECIAL_ACCOUNTS', {
        ADMIN: [
            'thienvu.lh',
            'hoang.nm'
        ],

        TAI_KHOAN_DONG_MOC: {
            CONG_TY: "cnnb",
            DANG_BO: "mocdang",
            DOAN_THANH_NIEN: "mocdoan",
            CONG_DOAN: "moccongdoan",
        },

        BLOCKED_VPDT: [
            'dang.nv',
            'tochinh.vt',
            'lam.nh',
            'phuonglinh.pt',
            'ngocoanh.nt',
            'tri.nhu',
            'tuan.nanh',
            'truong.lc',
            'nga.nn'
        ],

        MA_NHAN_VIEN_KHONG_CAN_TAO_DON_NGHI_PHEP: [
            524,
            540,
            493,
            28,
            532,
            502,
            204
        ], // mã nhân viên của ban gđ + kế toán trưởng + trưởng ban ks
    });
