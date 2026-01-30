angular.module("aims")
    .run(["$rootScope", function ($rootScope) {
        $rootScope.session = {
            isLoggedIn: false,
            username: '',
            roleName: '',
            canView: false,
            fullName: '',
            avatar: '',
            accessToken: ''
        };

        $rootScope.notification = {
            total: 0,
            vanban: {
                den: { chuaXem: 0, choDuyet: 0 },
                di: { chuaXem: 0, choDuyet: 0 },
                xoa: 0
            },
            calendar: { caNhan: 0, choDuyet: 0, tuChoi: 0 },
            message: { chuaXem: 0, tong: 0 }
        };

        $rootScope.report = {
            unread: 0,
            pending: 0,
            donNghiPhep: 0,
            phieuXinXe: 0,
            lenhDieuXe: 0,
            ungTruoc: 0,
            quyetToan: 0,
            soLoTrinh: 0,
            danhGia: {
                luuNhap: 0,
                choDuyet: 0,
                choHuy: 0,
                huyKy: 0,
                goDuyet: 0
            }
        };
    }]);
