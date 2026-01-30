angular
    .module('aims.shared.realtime')
    .factory('ReportHub', ReportHub);

ReportHub.$inject = [
    '$rootScope',
    'Hub',
    'appSettings',
    'userProfile',
    'loginservice'
];

function ReportHub(
    $rootScope,
    Hub,
    appSettings,
    userProfile,
    loginservice
) {
    var service = {};

    var data = userProfile.getProfile();
    var accesstoken = data.isLoggedIn ? data.access_token : null;

    var hub = new Hub('eventHub', {
        listeners: {
            countThongBaoReport: function (mess) {
                service.getCountReport();
                $rootScope.$broadcast('countThongBaoReport', mess);
                $rootScope.$applyAsync();
            }
        },
        queryParams: {
            token: accesstoken
        },
        rootPath: '/' + appSettings.serverApp + 'signalr'
    });

    service.getCountReport = function () {
        loginservice
            .getdata('api/QLDieuXe/GetCountThongBao')
            .then(function (res) {
                angular.extend($rootScope, res.data);
            });
    };

    return service;
}
