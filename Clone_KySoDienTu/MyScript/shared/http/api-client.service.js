angular
    .module('aims.shared')
    .service('ApiClient', ApiClient);

function ApiClient($http, appSettings, UserProfileService) {

    function authHeader() {
        const token = UserProfileService.getProfile().accessToken;
        return token ? { Authorization: `Bearer ${token}` } : {};
    }

    this.get = function (url, config = {}) {
        return $http.get(appSettings.serverPath + url, {
            ...config,
            headers: { ...authHeader(), ...config.headers }
        });
    };

    this.postJson = function (url, data, config = {}) {
        return $http.post(appSettings.serverPath + url, data, {
            ...config,
            headers: {
                'Content-Type': 'application/json',
                ...authHeader(),
                ...config.headers
            }
        });
    };

    this.postForm = function (url, data, config = {}) {
        return $http.post(appSettings.serverPath + url, $.param(data), {
            ...config,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                ...authHeader(),
                ...config.headers
            }
        });
    };

    this.download = function (url, data) {
        return $http.post(appSettings.serverPath + url, data, {
            responseType: 'arraybuffer',
            headers: authHeader()
        });
    };
}
