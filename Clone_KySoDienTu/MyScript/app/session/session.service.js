angular.module("aims")
    .service("SessionService", function (AuthApiService, ApiClient, UserProfileService, appSettings) {
        this.getProfile = function () {
            var data = UserProfileService.getProfile();
            if (!data.isLoggedIn) return { isLoggedIn: false };

            var ext = UserProfileService.getExtendedProfile();

            return {
                isLoggedIn: true,
                username: data.username,
                roleName: data.roleName,
                accessToken: data.access_token,
                fullName: ext.fileusername,
                avatar: ext.ulrimage
                    ? appSettings.serverPath + 'Content/image/' + ext.ulrimage
                    : appSettings.serverPath + 'Content/image/user.png'
            };
        };

        this.login = function (credentials) {
            return AuthApiService.login(credentials);
        };

        this.logout = function () {
            return ApiClient
                .postData("api/Account/Logout")
                .finally(function () {
                    UserProfileService.clear();
                });
        };
    });