angular
    .module('aims.shared.realtime')
    .factory('ReportHub', ReportHub);

ReportHub.$inject = [
    '$rootScope',
    'Hub',
    'UserProfileService',
    'ApiClient'
];

function ReportHub(
    $rootScope,
    Hub,
    UserProfileService,
    ApiClient
) {
    var service = {};

    var data = UserProfileService.getProfile();
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
        rootPath: '/signalr'
    });

    service.getCountReport = function () {
        ApiClient
            .get('api/QLDieuXe/GetCountThongBao')
            .then(function (res) {
                angular.extend($rootScope, res.data);
            });
    };

    return service;
}
