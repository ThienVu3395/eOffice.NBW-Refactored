angular.module("aims")
    .controller("MenuController", [
        "$scope",
        "$timeout",
        "loginservice",
        "appSettings",
        function ($scope, $timeout, loginservice, appSettings) {

            $scope.menuTree = [];
            $scope.activegroup = {};
            $scope.activelink = {};
            $scope.notifications = [];

            loadMenu();

            function loadMenu() {
                loginservice.getdata("api/userinfo/getUsersMenu")
                    .then(function (res) {
                        $scope.menuTree = normalizeMenu(res.data);
                    });
            }

            function normalizeMenu(data) {
                angular.forEach(data, function (item) {
                    if (item.par.TYPE !== 5) {
                        item.par.LINKS = appSettings.serverPath + item.par.LINKS;
                    }
                });
                return data;
            }
        }
    ]);
