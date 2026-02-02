angular
    .module('aims.shared.constants')
    .constant('ROLES', {
        CHUC_VU: {
            CONG_NHAN: ['CN'],
            NHAN_VIEN: ['NV'],
            THU_KHO: ['TK'],
            TRUONG_PHO_PHONG_BAN: ['PTB', 'PTP', 'PTP-PT', 'TB', 'TP'],
            TO_TRUONG: ['TT']
        },
        TO_NHOM: {
            THAY_DHN: ['Thay-DHN-Q4-NB', 'ThayDHN-Q7'],
            THI_CONG: ['Thi-cong-Q7', 'ThiCong-Q4-NB'],
            LAI_XE: ['LaiXe'],
            VAN_HANH_THIET_BI: ['QuanLy-CoGioi'],
            DO_BE: ['Thay-DHN-Q4-NB', 'ThayDHN-Q7', 'Thi-cong-Q7', 'ThiCong-Q4-NB']
        }
    });
