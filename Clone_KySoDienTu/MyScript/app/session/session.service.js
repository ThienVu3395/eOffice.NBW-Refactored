angular.module("aims")
    .service("SessionService", function (loginservice, userProfile, appSettings, $q) {
        this.getProfile = function () {
            var data = userProfile.getProfile();
            if (!data.isLoggedIn) return { isLoggedIn: false };

            var ext = userProfile.getProfileExten();

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

        this.logout = function () {
            return loginservice
                .postdata("api/Account/Logout")
                .finally(function () {
                    userProfile.clearall();
                });
        };
    });