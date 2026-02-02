angular
    .module('aims.http')
    .service('ApiClient', ApiClient);

function ApiClient($http, appSettings, UserProfileService) {

    function getAuthHeader() {
        const token = UserProfileService.getProfile().accessToken;
        return token ? { Authorization: `Bearer ${token}` } : {};
    }

    // ================= BASIC =================
    this.get = function (url) {
        return $http({
            url: appSettings.serverPath + url,
            method: 'GET',
            headers: getAuthHeader()
        });
    };

    this.postForm = function (url, data) {
        return $http({
            url: appSettings.serverPath + url,
            method: 'POST',
            data: $.param(data),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                ...getAuthHeader()
            }
        });
    };

    this.postData = function (url, data) {
        return $http({
            url: appSettings.serverPath + url,
            method: 'POST',
            data: data,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                ...getAuthHeader()
            }
        });
    };

    this.postJson = function (url, data) {
        return $http({
            url: appSettings.serverPath + url,
            method: 'POST',
            data: data,
            headers: {
                'Content-Type': 'application/json',
                ...getAuthHeader()
            }
        });
    };

    // ================= FILE =================
    this.downloadFile = function (url) {
        return $http({
            url: appSettings.serverPath + url,
            method: 'GET',
            responseType: 'arraybuffer',
            headers: getAuthHeader()
        });
    };

    this.postFile = function (url, data) {
        return $http({
            url: appSettings.serverPath + url,
            method: 'POST',
            data: $.param(data),
            responseType: 'arraybuffer',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                ...getAuthHeader()
            }
        });
    };

    // ================= EXTERNAL API =================
    this.postExternal = function (baseUrl, url, data, token) {
        return $http({
            url: baseUrl + url,
            method: 'POST',
            data: data,
            headers: {
                Authorization: token
            }
        });
    };
}
