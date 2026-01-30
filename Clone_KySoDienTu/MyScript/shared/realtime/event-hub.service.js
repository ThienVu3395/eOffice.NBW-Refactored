angular
    .module('aims.shared.realtime')
    .factory('eventHub', eventHub);

eventHub.$inject = [
    '$rootScope',
    '$document',
    '$uibModal',
    'Hub',
    'appSettings',
    'userProfile',
    'loginservice'
];

function eventHub(
    $rootScope,
    $document,
    $uibModal,
    Hub,
    appSettings,
    userProfile,
    loginservice
) {

    var service = {};

    $rootScope.quyenuser = { NhanThongBao: false };
    $rootScope.ListItem = [];

    var data = userProfile.getProfile();
    var accesstoken = data.isLoggedIn ? data.access_token : null;

    var hub = new Hub('eventHub', {
        listeners: {
            onConnected: function () {
                $rootScope.$applyAsync();
            },

            popupEvent: function (item) {
                toastr.options = {
                    preventDuplicates: true,
                    onclick: function () {
                        openCalendarModal(item.valint1);
                    }
                };
                toastr.success(item.valstring2);
                $rootScope.$applyAsync();
            },

            workflow: function (type) {
                $rootScope.$broadcast('homethongbao', type);
                if (type === 1) $rootScope.$broadcast('homeIndex', type);
                if (type === 2 || type === 3) $rootScope.$broadcast('getAll', type);
                $rootScope.$applyAsync();
            },

            gcalevent: function (type) {
                if (type === 1 || type === 2)
                    $rootScope.$broadcast('homeGCAL', type);
                else if (type === 3) {
                    $rootScope.$broadcast('homeGCAL', 2);
                    $rootScope.$broadcast('getCal', type);
                }
                $rootScope.$applyAsync();
            },

            getCount: function () {
                service.getCount();
                $rootScope.$applyAsync();
            },

            getCal: function (mess) {
                service.getCount();
                $rootScope.$broadcast('getCal', mess);
                $rootScope.$applyAsync();
            },

            updateXuLy: function (mess) {
                $rootScope.$broadcast('getAll', mess);
                $rootScope.$applyAsync();
            },

            updateChuaXem: function (mess) {
                getCountDaXem();
                $rootScope.$broadcast('getAll', mess);
                $rootScope.$applyAsync();
            },

            updateCountDuyet: function () {
                getCountDuyetCv();
                $rootScope.$applyAsync();
            },

            updateCountDaXem: function () {
                getCountDaXem();
                $rootScope.$applyAsync();
            },

            countThongBaoVB: function (mess) {
                getChuongThongBaoVB();
                $rootScope.$broadcast('countThongBaoVB', mess);
                $rootScope.$applyAsync();
            }
        },

        queryParams: {
            token: accesstoken
        },

        rootPath: '/' + appSettings.serverApp + 'signalr'
    });

    // ================= PRIVATE =================

    function openCalendarModal(id) {
        var parentElem = angular.element(
            $document[0].querySelector('.content-wrapper')
        );

        $uibModal.open({
            animation: true,
            backdrop: 'static',
            templateUrl: 'lichcanhan.html',
            controller: 'lichcanhanCtrl',
            controllerAs: '$ctrl',
            size: 'lg',
            appendTo: parentElem,
            resolve: {
                idselect: function () {
                    return id;
                }
            }
        });
    }

    function getCountDaXem() {
        loginservice
            .getdata('api/congviec/getdsDaxem')
            .then(
                function successCallback(res) {
                    $rootScope.countCongViec = res.data;
                    recalcTotal();
                },
                function errorCallback(error) {
                }
            );
    }

    function getCountDuyetCv() {
        loginservice
            .getdata('api/congviec/getCountDuyet')
            .then(
                function successCallback(res) {
                    $rootScope.countDuyetcv = res.data;
                    recalcTotal();
                },
                function errorCallback(error) {
                }
            );
    }

    function getChuongThongBaoVB() {
        loginservice
            .getdata('api/QLVanBan/getListVBDenDiAll_ChuaXemVaXoa')
            .then(
                function successCallback(res) {
                    angular.extend($rootScope, res.data);
                    recalcTotal();
                },
                function errorCallback(error) {
                }
            );
    }

    function recalcTotal() {
        $rootScope.counttong =
            ($rootScope.countDuyetcv || 0) +
            ($rootScope.countCongViec || 0) +
            ($rootScope.SoLuongTB || 0);
    }

    // ================= PUBLIC =================

    service.getCount = function () {
        if (data.isLoggedIn) {
            getChuongThongBaoVB();
        } else {
            $rootScope.counttong = 0;
        }
    };

    return service;
}
