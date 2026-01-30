angular.module("aims")
    .controller("NotificationController", [
        "$rootScope",
        "NotificationService",
        function ($rootScope, NotificationService) {

            // KHÔNG khởi tạo lại biến
            // chỉ trigger load data
            NotificationService.refreshAll();
        }
    ]);