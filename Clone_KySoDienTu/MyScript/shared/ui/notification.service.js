angular
    .module('aims.shared.ui')
    .factory('notificationService', notificationService);

notificationService.$inject = ['$window'];

function notificationService($window) {

    var service = {};

    // ================= CONFIG =================

    toastr.options = {
        closeButton: true,
        progressBar: true,
        positionClass: 'toast-top-right',
        timeOut: '3000',
        preventDuplicates: true
    };

    // ================= TOAST =================

    service.success = function (message, title) {
        toastr.success(message, title || 'Thành công');
    };

    service.error = function (message, title) {
        toastr.error(message, title || 'Lỗi');
    };

    service.warning = function (message, title) {
        toastr.warning(message, title || 'Cảnh báo');
    };

    service.info = function (message, title) {
        toastr.info(message, title || 'Thông tin');
    };

    // ================= CONFIRM =================

    service.confirm = function (message, title) {
        return $window.confirm(title ? title + '\n' + message : message);
    };

    // ================= ALERT =================

    service.alert = function (message) {
        $window.alert(message);
    };

    return service;
}