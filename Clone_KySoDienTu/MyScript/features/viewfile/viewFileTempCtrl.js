angular
    .module("aims")
    .controller('viewFileTempCtrl', [
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

            $ctrl.Print = true;

            $ctrl.sumitformedit = function () {
            }

            $ctrl.ok = function () {
                $ctrl.presult = "0";
            };

            $ctrl.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $ctrl.urldoc = appSettings.serverPath + "/Viewfile/viewfileonline?tid=" + idselect;

            $ctrl.pdf = {
                src: '',
                data: null
            };

            $ctrl.Permispdf = {
                download: 'false',
                print: 'false'
            };

            $ctrl.save = function () {
                blockUI.start();
                var resp = loginservice.getdatafile("api/congviec/getviewpdf?file=" + idselect);
                resp.then(function (response) {
                    blockUI.stop();
                    var headers = response.headers();
                    var filename = headers['x-filename'];
                    var contentType = headers['content-type'];
                    var file = new Blob([response.data], { type: contentType });
                    saveAs(file, idselect);
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                        thongbao.errorcenter("Không tìm thấy file");
                    });
            }

            $ctrl.printfile = function () {
                blockUI.start();
                var resp = loginservice.getdatafile("api/congviec/getviewpdf?file=" + idselect);
                resp.then(function (response) {
                    blockUI.stop();
                    var headers = response.headers();
                    var contentType = headers['content-type'];
                    var file = new Blob([response.data], { type: contentType });
                    var reader = new FileReader();
                    reader.readAsDataURL(file);
                    reader.onloadend = function () {
                        window.printJS({ printable: reader.result.split(',')[1], type: 'pdf', base64: true });
                    }
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                        thongbao.errorcenter("Không tìm thấy file");
                    });
            }

            $ctrl.onPageLoad = function (page) {
                $ctrl.pageview = page;
            };
        }
    ]);