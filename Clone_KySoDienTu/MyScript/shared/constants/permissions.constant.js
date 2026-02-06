angular.module('aims.shared.constants')
    .constant('PERMISSIONS', {
        PHIEU_NANG_SUAT: {
            CREATE_GROUP: 'CREATE_GROUP',
            CREATE_ALL: 'CREATE_ALL',
            REPORT_GROUP: 'REPORT_GROUP',
            REPORT_ALL: 'REPORT_ALL',
            UPDATE: 'UPDATE',
            MANAGE: 'MANAGE'
        },
        TONG_HOP_PHIEU_NANG_SUAT: {
            CREATE_GROUP: 'CREATE_GROUP',
            CREATE_ALL: 'CREATE_ALL',
            REPORT_GROUP: 'REPORT_GROUP',
            REPORT_ALL: 'REPORT_ALL',
            UPDATE: 'UPDATE',
            MANAGE: 'MANAGE'
        },
        DON_NGHI_PHEP: {
            CREATE: 'CRP_NP',
            REPORT: 'CTTRP_NP',
            VIEW: 'VRP_NP',
            CAP_NHAT: 'URP_NP',
            DUYET: 'ARP_NP',
            XOA: 'DRP_NP',
        },
        VAN_BAN_KY_SO: {
            CREATE_ALL: 'CRP_ALL',
            DUYET: 'ARP_ALL',
            PHAN_PHAT: 'PPRP_ALL',
            BUT_PHE: 'BPPP_ALL',
            DONG_MOC: 'MOCRP_ALL',
            DIEU_CHINH: 'AURP_ALL',
            CHINH_SUA_VI_TRI_CHU_KY: 'UPDATE_SIGN',
            DONG_MOC_CONG_TY: 'MARK_CNNB',
            DONG_MOC_DANG_BO: 'MARK_DB',
            DONG_MOC_DOAN_THANH_NIEN: 'MARK_DTN',
            DONG_MOC_CONG_DOAN: 'MARK_CD',
        }
    });