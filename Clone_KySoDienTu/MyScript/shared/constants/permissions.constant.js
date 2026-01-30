angular.module('aims.shared')
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
            VIEW: 'VRP_NP'
        }
    });