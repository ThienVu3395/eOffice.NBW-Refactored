(function () {
    "use strict";
    angular.module("oamsapp")
        .controller("logincontroller",
            [
                "$scope",
                "appSettings",
                "loginservice",
                "userProfile",
                "ModalService",
                "commonConstants",
                function (
                    $scope,
                    appSettings,
                    loginservice,
                    userProfile,
                    ModalService,
                    commonConstants) {
                    $scope.responseData = "";

                    if (getParameterByName("logout")) {
                        logout();
                    }

                    $scope.userName = "";

                    $scope.userRegistrationEmail = "";

                    $scope.userRegistrationPassword = "";

                    $scope.userRegistrationConfirmPassword = "";

                    $scope.userLoginEmail = "";

                    $scope.userLoginPassword = "";

                    $scope.accessToken = "";

                    $scope.refreshToken = "";

                    $scope.error = false;

                    $scope.errormess = "";

                    $scope.dataLogin = userProfile.getProfile();

                    if ($scope.dataLogin.isLoggedIn) {
                        logout();
                    }

                    $scope.login = function () {
                        var userLogin = {
                            grant_type: 'password',
                            username: $scope.userLoginEmail,
                            password: $scope.userLoginPassword
                        };
                        if (!commonConstants.TAI_KHOAN_KHONG_DUOC_DANG_NHAP_VPDT.includes($scope.userLoginEmail)) {
                            var promiselogin = loginservice.login(userLogin);
                            promiselogin.then(function (resp) {
                                $scope.userName = resp.data.userName;
                                $scope.roleName = resp.data.roleName;
                                userProfile.setProfile(resp.data.userName, resp.data.access_token, resp.data.access_token, resp.data.refresh_token, resp.data.roleName);
                                var respd = loginservice.postdata("api/userinfo/getInfoUser", $.param({ valstring1: $scope.userName }));
                                respd.then(
                                    function (response) {
                                        //console.log(response.data);
                                        if (response.data != null) {
                                            userProfile.setProfileExten(
                                                response.data.HOLOT + " " + response.data.TEN,
                                                response.data.FILEANH,
                                                response.data.MSNV,
                                                response.data.BOPHAN,
                                                response.data.CHUCVU);
                                        }
                                        window.location.href = appSettings.serverPath + appSettings.serverHome;
                                    },
                                    function errorCallback(response) {
                                        $scope.error = true;
                                        $scope.errormess = "Tài khoản đã bị khóa!";
                                        userProfile.clearall();
                                    }
                                );
                                //var promiselogin1 = loginservice.loginapivpdt(userLogin);
                                //promiselogin1.then(function (resp1) {
                                //    userProfile.setProfile(resp1.data.userName, resp.data.access_token, resp1.data.access_token, resp.data.refresh_token, resp.data.roleName);
                                //    var respd = loginservice.postdata("api/userinfo/getInfoUser", $.param({ username: $scope.userName, shopingcart: "" }));
                                //    respd.then(function (response) {
                                //        if (response.data != null) {
                                //            userProfile.setProfileExten(response.data.HOLOT + " " + response.data.TEN, response.data.FILEHINH);
                                //        }
                                //        window.location.href = appSettings.serverPath + appSettings.serverHome;
                                //    }
                                //        , function errorCallback(response) {
                                //            $scope.error = true;
                                //            $scope.errormess = "Tài khoản đã bị khóa!";
                                //            userProfile.clearall();
                                //        });
                                //}, function (err) {
                                //});
                            }, function (err) {
                                $scope.errormess = "Tên tài khoản hoặc mật khẩu không tồn tại!";
                                $scope.error = true;
                            });
                        }
                        else {
                            $scope.error = true;
                            $scope.errormess = "Tài khoản này không được cấp phép để đăng nhập vào ứng dụng!";
                        }
                    };

                    function logout() {
                        var respd;
                        respd = loginservice.postdata("api/Account/Logout");
                        respd.then(function (response) {
                            userProfile.clearall();
                        }, function errorCallback(response) {
                            userProfile.clearall();
                        });
                    };

                    $scope.KhaiBaoYTe = function (status) {
                        ModalService.open({
                            templateUrl: 'KhaiBaoYTe.html',
                            controller: 'otherFormKhaiBaoYTeCtrl',
                            resolve: {
                                idselect: function () {
                                    return status;
                                }
                            }
                        }).then(function (c) {
                        }, function () {
                        });
                    }
                }]);
}());