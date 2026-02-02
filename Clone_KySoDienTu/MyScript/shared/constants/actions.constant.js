angular
    .module('aims.shared.constants')
    .constant('ACTIONS', {
        HUY_KY: 1,
        THU_HOI: 2,
        YEU_CAU_HUY: 5,
        TU_CHOI_HUY: 6,
        GO_DUYET: 7,
        GO_YEU_CAU_HUY: 8,

        TITLES: {
            HUY_KY: 'hủy ký',
            GO_DUYET: 'gỡ duyệt',
            YEU_CAU_HUY: 'yêu cầu hủy',
            HUY: 'hủy'
        },

        API: {
            PHIEU_DANH_GIA: {
                HUY_KY: 'api/QLChamCong/HuyKyVB',
                GO_DUYET: 'api/QLChamCong/GoDuyetVB',
                LUONG_YEU_CAU_HUY: 'api/QLChamCong/YeuCauHuy-TuChoiHuy-GoYeuCauHuyVB'
            },
            BANG_TONG_HOP: {
                HUY_KY: 'api/THChamCong/HuyKyVB',
                GO_DUYET: 'api/THChamCong/GoDuyetVB',
                LUONG_YEU_CAU_HUY: 'api/THChamCong/YeuCauHuy-TuChoiHuy-GoYeuCauHuyVB'
            }
        }
    });