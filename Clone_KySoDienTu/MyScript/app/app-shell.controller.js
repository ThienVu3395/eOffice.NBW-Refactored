// Chỉ giữ vai trò “khung ứng dụng”, có trách nhiệm
// - Load session
// - Hiển thị avatar, username
// - Điều hướng hồ sơ cá nhân
// - Logout
angular.module("aims")
    .controller("AppShellController", [
        "$rootScope",
        "SessionService",
        "ModalService",
        "appSettings",
        function (
            $rootScope,
            SessionService,
            ModalService,
            appSettings
        ) {
            var vm = this;

            vm.user = {};
            vm.isLoggedIn = false;

            init();

            function init() {
                var profile = SessionService.getProfile();
                if (!profile.isLoggedIn) {
                    redirectLogin();
                    return;
                }

                vm.isLoggedIn = true;
                vm.user = profile;

                // đồng bộ state
                angular.extend($rootScope.session, {
                    isLoggedIn: true,
                    username: profile.username,
                    roleName: profile.roleName,
                    fullName: profile.fullName,
                    avatar: profile.avatar,
                    accessToken: profile.accessToken
                });
            }

            vm.logout = function () {
                SessionService
                    .logout()
                    .finally(redirectLogin);
            };

            vm.openProfile = function () {
                ModalService.open({
                    templateUrl: '_hosocanhan.html',
                    controller: 'HoSoCaNhanCtrl',
                    resolve: {
                        idselect: function () {
                            return vm.user.username;
                        }
                    }
                });
            };

            function redirectLogin() {
                window.location.href = appSettings.serverPath + appSettings.serverLogin;
            }
        }
    ]);
