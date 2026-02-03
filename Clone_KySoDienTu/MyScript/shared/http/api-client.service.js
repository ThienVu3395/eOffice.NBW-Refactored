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

    this.postDataForSmartCA = function (url, data) {
        return $http({
            url: url,
            method: 'POST',
            data: data,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
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

    this.postJsonNoAuth = function (url, data) {
        return $http({
            url: appSettings.serverPath + url,
            method: 'POST',
            data: data,
            headers: {
                'Content-Type': 'application/json'
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

    this.getFile = function (url) {
        return $http({
            url: appSettings.serverPath + url,
            method: "GET",
            responseType: 'arraybuffer',
            headers:
            {
                'foo': 'bar',
                ...getAuthHeader()
            },
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
