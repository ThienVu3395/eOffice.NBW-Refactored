angular.module("aims")
    .controller("SessionController", [
        "$rootScope",
        "$window",
        "SessionService",
        "ModalService",
        "appSettings",
        function (
            $rootScope,
            $window,
            SessionService,
            ModalService,
            appSettings
        ) {

            var vm = this;

            vm.isLoggedIn = false;
            vm.user = {};

            init();

            // ================= INIT =================
            function init() {
                var profile = SessionService.getProfile();

                if (!profile.isLoggedIn) {
                    redirectLogin();
                    return;
                }

                vm.isLoggedIn = true;
                vm.user = profile;

                // ghi vào state
                $rootScope.session.isLoggedIn = true;
                $rootScope.session.username = profile.username;
                $rootScope.session.roleName = profile.roleName;
                $rootScope.session.fullName = profile.fullName;
                $rootScope.session.avatar = profile.avatar;
                $rootScope.session.accessToken = profile.accessToken;
            }

            // ================= ACTIONS =================
            vm.logout = function () {
                SessionService.logout()
                    .finally(redirectLogin);
            };

            vm.openProfile = function () {
                ModalService.open({
                    templateUrl: "_hosocanhan.html",
                    controller: "HoSoCaNhanCtrl",
                    controllerAs: "$ctrl",
                    resolve: {
                        idselect: function () {
                            return vm.user.username;
                        }
                    }
                }).then(init, init);
            };

            // ================= HELPERS =================
            function redirectLogin() {
                $window.location.href =
                    appSettings.serverPath + appSettings.serverLogin;
            }
        }
    ]);
