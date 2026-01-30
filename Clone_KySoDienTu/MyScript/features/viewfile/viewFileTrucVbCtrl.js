angular
    .module("aims")
    .controller('viewFileTrucVbCtrl', [
        'thongbao',
        "$scope",
        "$uibModalInstance",
        "blockUI",
        "appSettings",
        "loginservice",
        "idselect",
        function (
            thongbao,
            $scope,
            $uibModalInstance,
            blockUI,
            appSettings,
            loginservice,
            idselect) {
            var $ctrl = this;

            $ctrl.Print = false;

            $ctrl.sumitformedit = function () {
            }

            $ctrl.ok = function () {
                $ctrl.presult = "0";
            };

            $ctrl.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $ctrl.pdf = {
                src: '',  // get pdf source from a URL that points to a pdf
                data: null // get pdf source from raw data of a pdf
            };

            getdatafilePDF();

            function getdatafilePDF() {
                $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=13&filename=" + idselect.filename;
            }

            $ctrl.save = function () {
                blockUI.start();
                let url = "api/QLTrucVanBan/getviewpdfvb?fileName=" + idselect.filename + "&description=" + idselect.description;
                var resp = loginservice.getdatafile(url);
                resp.then(function (response) {
                    blockUI.stop();
                    var headers = response.headers();
                    var filename = headers['x-filename'];
                    var contentType = headers['content-type'];
                    var file = new Blob([response.data], { type: contentType });
                    saveAs(file, filename);
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                        thongbao.errorcenter("Không tìm thấy file");
                    });
            }

            $ctrl.onPageLoad = function (page) {
                $ctrl.page1 = page;
                $ctrl.pageview = page;
            };
        }
    ]);