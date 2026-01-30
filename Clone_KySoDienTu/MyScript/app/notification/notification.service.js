angular.module("aims")
    .service("NotificationService", function (eventHub, ReportHub) {

        this.refreshAll = function () {
            eventHub.getCount();
            ReportHub.getCountReport();
        };
    });