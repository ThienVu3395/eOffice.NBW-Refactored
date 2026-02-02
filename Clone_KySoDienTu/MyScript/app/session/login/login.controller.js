angular
    .module("aims.login")
    .controller("LoginController", [
        "$scope",
        "appSettings",
        "AuthApiService",
        "UserProfileService",
        "SPECIAL_ACCOUNTS",
        "UrlUtil",
        "ApiClient",
        function (
            $scope,
            appSettings,
            AuthApiService,
            UserProfileService,
            SPECIAL_ACCOUNTS,
            UrlUtil,
            ApiClient) {

            var vm = this;

            // ================= STATE =================
            vm.userName = "";
            vm.password = "";
            vm.loading = false;
            vm.error = null;

            // ================= INIT =================
            var profile = UserProfileService.getProfile();
            if (profile.isLoggedIn) {
                logout();
            }

            if (UrlUtil.getParameterByName("logout")) {
                logout();
            }

            // ================= ACTIONS =================
            vm.login = function () {
                vm.error = null;
                vm.loading = true;

                if (SPECIAL_ACCOUNTS.BLOCKED_VPDT.includes(vm.userName)) {
                    vm.error = "Tài khoản này không được phép đăng nhập!";
                    vm.loading = false;
                    return;
                }

                AuthApiService.login({
                    username: vm.userName,
                    password: vm.password
                })
                    .then(function (resp) {
                        UserProfileService.setAuthProfile(
                            resp.data.userName,
                            resp.data.access_token,
                            resp.data.refresh_token,
                            resp.data.access_token,
                            resp.data.roleName
                        );

                        ApiClient
                            .postForm("api/userinfo/getInfoUser", { valstring1: resp.data.userName })
                            .then(function (response) {
                                if (response && response.data) {
                                    UserProfileService.setExtendedProfile(
                                        response.data.HOLOT + " " + response.data.TEN,
                                        response.data.FILEANH,
                                        response.data.MSNV,
                                        response.data.BOPHAN,
                                        response.data.CHUCVU
                                    );
                                }

                                window.location.href =
                                    appSettings.serverPath + appSettings.serverHome;
                            })
                            .catch(function () {
                                vm.error = "Tên tài khoản hoặc mật khẩu không đúng!";
                                vm.loading = false;
                            })
                    })
                    .catch(function () {
                        vm.error = "Tên tài khoản hoặc mật khẩu không đúng!";
                        vm.loading = false;
                    })
            };

            function logout() {
                ApiClient
                    .postData("api/Account/Logout")
                    .finally(function () {
                        UserProfileService.clear();
                    });
            }
        }
    ]);