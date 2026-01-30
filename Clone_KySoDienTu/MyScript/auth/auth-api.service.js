angular
    .module('aims.auth')
    .service('AuthApiService', AuthApiService);

function AuthApiService($http, appSettings) {
    this.login = function ({ username, password }) {
        return $http.post(
            appSettings.serverPath + 'Token',
            $.param({
                grant_type: 'password',
                username,
                password
            }),
            {
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
            }
        );
    };

    this.loginExternal = function (baseUrl, username, password) {
        return $http.post(
            baseUrl + 'Token',
            $.param({
                grant_type: 'password',
                username,
                password
            }),
            {
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
            }
        );
    };
}
