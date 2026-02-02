angular
    .module('aims.shared.constants')
    .constant('FILES', {
        LOAI_DINH_KEM: {
            THUONG: 0,
            KY_SO: 1
        },
        FOLDER: {
            PHIEU_NANG_SUAT: 'ExportsQLCC',
            BANG_TONG_HOP: 'ExportsTHCC'
        },
        DEFAULT_NAME_PHIEU_NANG_SUAT: 'PĐGNS',
        EXPORT: {
            NAME_FORMAT: 'BangTongHopKQTHNVNLD',
            WORD: {
                MIME: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
                EXT: 'docx'
            },
            EXCEL: {
                MIME: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
                EXT: 'xlsx'
            }
        }
    });