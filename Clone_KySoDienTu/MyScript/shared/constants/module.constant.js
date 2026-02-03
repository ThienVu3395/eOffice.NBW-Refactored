angular
    .module('aims.shared.constants')
    .constant('MODULES', {
        PHIEU_NANG_SUAT: {
            KEY: 'PhieuChamNangSuat',
            RESOURCE_TYPE: 'G',
            RESOURCE_ID: '0',
            ID: 30
        },
        TONG_HOP_PHIEU_NANG_SUAT: {
            KEY: 'TongHopPhieuNangSuat',
            RESOURCE_TYPE: 'G',
            RESOURCE_ID: '0',
            ID: 31,
            MA_LAY_DANH_SACH_KY_DUYET: 7
        },
        DON_NGHI_PHEP: {
            ID: 7,
            DON_NGHI_PHEP_NHAN_VIEN: {
                CODE: 6,
                MESSAGE: "Đơn xin nghỉ phép mẫu 1 cần Trưởng Phòng/Ban của người xin nghỉ và Trưởng P.TCHC ký duyệt",
                TYPE: 10
            },
            DON_NGHI_PHEP_TRUONG_PHONG: {
                CODE: 7,
                MESSAGE: "Đơn xin nghỉ phép mẫu 2 cần PGĐ phụ trách của người xin nghỉ và GĐ ký duyệt",
                TYPE: 11
            },
            DON_NGHI_PHEP_KHONG_LUONG: {
                CODE: 8,
                MESSAGE: "Đơn xin nghỉ việc không lương cần ý kiến và chữ ký của Trưởng Phòng/Ban của người xin nghỉ và GĐ ký duyệt",
                TYPE: 12
            },
            DON_NGHI_PHEP_DI_NUOC_NGOAI: {
                CODE: 9,
                MESSAGE: "Đơn xin nghỉ phép đi nước ngoài cần Trưởng Phòng/Ban của người xin nghỉ và GĐ ký duyệt",
                TYPE: 13
            },
            DON_GIAI_TRINH_FACEID_NHAN_VIEN: {
                CODE: 28,
                MESSAGE: "Đơn giải trình mẫu 5 cần Trưởng Phòng/Ban của người xin làm đơn ký duyệt",
                TYPE: 14
            },
            DON_GIAI_TRINH_FACEID_TRUONG_PHONG: {
                CODE: 29,
                MESSAGE: "Đơn giải trình mẫu 6 cần GĐ ký duyệt",
                TYPE: 15
            }
        }
    });
