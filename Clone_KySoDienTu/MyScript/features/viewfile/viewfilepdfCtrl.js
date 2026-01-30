(function () {
    "use strict"
    angular.module("oamsapp")
        .controller('viewfilepdfCtrl', [
            "$scope",
            "$uibModalInstance",
            "blockUI",
            "loginservice",
            "idselect",
            function (
                $scope,
                $uibModalInstance,
                blockUI,
                loginservice,
                idselect)
            {
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

                $ctrl.urldoc = "http://localhost:36467/Viewfile/viewfileonline";

                $ctrl.pdf = {
                    src: '',  // get pdf source from a URL that points to a pdf
                    data: null // get pdf source from raw data of a pdf
                };

                $ctrl.Permispdf = {
                    download: 'false',  // get pdf source from a URL that points to a pdf
                    print: 'false' // get pdf source from raw data of a pdf
                };

                getdatafilePDF();

                function getdatafilePDF() {
                    blockUI.start();
                    var resp = loginservice.getdatafile("api/viewfileonline/getviewpdf");
                    resp.then(function (response) {
                        $ctrl.pdf.data = new Uint8Array(response.data);
                        if (!$ctrl.reload)
                            $ctrl.pageview = 1;
                        //document.title = 'FSM - Hệ thống quản trị số hóa tài liệu';
                        blockUI.stop();
                    }
                        , function errorCallback(response) {
                            $ctrl.pdf.data = null;
                            blockUI.stop();

                            //document.title = 'FSM - Hệ thống quản trị số hóa tài liệu';
                        });
                }
                $ctrl.onPageLoad = function (page) {
                    $ctrl.page1 = page;
                    $ctrl.pageview = page;
                };
            }])
        .controller('viewfilepdfTempCtrl', [
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
                idselect)
            {
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
            }])
        .controller('viewfilepdfWFCtrl', [
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
                idselect)
            {
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

                $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?id=" + idselect.ID + "&type=" + idselect.type;

                $ctrl.pdf = {
                    src: '',  
                    data: null
                };
                
                $ctrl.save = function () {
                    blockUI.start();
                    var resp = loginservice.getdatafile("api/congviec/Workflow?id=" + idselect.ID + "&type=" + idselect.type);
                    resp.then(function (response) {
                        blockUI.stop();
                        var headers = response.headers();
                        var contentType = headers['content-type'];
                        var file = new Blob([response.data], { type: contentType });
                        saveAs(file, idselect.MoTa);
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                            thongbao.errorcenter('Không tìm thấy file');
                        });
                }

                $ctrl.printfile = function () {
                    blockUI.start();
                    var resp = loginservice.getdatafile("api/congviec/Workflow?id=" + idselect.ID + "&type=" + idselect.type);
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
                            thongbao.errorcenter('Không tìm thấy file');
                        });
                }

                $ctrl.onPageLoad = function (page) {
                    $ctrl.page1 = page;
                    $ctrl.pageview = page;
                };
            }])
        .controller('viewfilepdfxlCtrl', [
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
                idselect)
            {
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

                $ctrl.urldoc = appSettings.serverPath + "/Viewfile/viewfileonline?xlid=" + idselect;

                $ctrl.pdf = {
                    src: '',  // get pdf source from a URL that points to a pdf
                    data: null // get pdf source from raw data of a pdf
                };
                
                $ctrl.save = function () {
                    blockUI.start();
                    var resp = loginservice.getdatafile("api/congviec/getviewpdfxl?id=" + idselect.ID);
                    resp.then(function (response) {
                        blockUI.stop();
                        var headers = response.headers();
                        var filename = headers['x-filename'];
                        var contentType = headers['content-type'];
                        var file = new Blob([response.data], { type: contentType });
                        saveAs(file, idselect.MOTA);
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                            thongbao.errorcenter("Không tìm thấy file");
                        });
                }

                $ctrl.printfile = function () {
                    blockUI.start();
                    var resp = loginservice.getdatafile("api/congviec/getviewpdfxl?id=" + idselect.ID);
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
                    $ctrl.page1 = page;
                    $ctrl.pageview = page;
                };
            }])
        .controller('viewfilepdfvbCtrl', [
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
                idselect)
            {
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


                $ctrl.pdf = {
                    src: '',  // get pdf source from a URL that points to a pdf
                    data: null // get pdf source from raw data of a pdf
                };

                getdatafilePDF();

                function getdatafilePDF() {
                    $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=" + idselect.type + "&id=" + idselect.id;
                }

                $ctrl.save = function () {
                    blockUI.start();
                    let url = "api/QLVanBan/getviewpdfvb?id=" + idselect.id + "&loaivb=" + idselect.loaivb;
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

                $ctrl.printfile = function () {
                    blockUI.start();
                    let url = "api/QLVanBan/getviewpdfvb?id=" + idselect.id + "&loaivb=" + idselect.loaivb;
                    var resp = loginservice.getdatafile(url);
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
                    $ctrl.page1 = page;
                    $ctrl.pageview = page;
                };
            }])
        .controller('viewfilepdftrucvbCtrl', [
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
                idselect)
            {
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
            }])
        .controller('viewfilepdfReportCtrl', [
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
                idselect)
            {
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

                $ctrl.pdf = {
                    src: '',  // get pdf source from a URL that points to a pdf
                    data: null // get pdf source from raw data of a pdf
                };

                getdatafilePDF();

                function getdatafilePDF() {
                    $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=" + idselect.type + "&id=" + idselect.id;
                }

                $ctrl.save = function () {
                    blockUI.start();
                    let url = "api/QLBaoCao/getviewpdfvb?id=" + idselect.id;
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

                $ctrl.printfile = function () {
                    blockUI.start();
                    let url = "api/QLBaoCao/getviewpdfvb?id=" + idselect.id;
                    var resp = loginservice.getdatafile(url);
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
                    $ctrl.page1 = page;
                    $ctrl.pageview = page;
                };
            }])
        .controller('viewfilepdftbCtrl', [
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
                idselect)
            {
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

                $ctrl.pdf = {
                    src: '',  // get pdf source from a URL that points to a pdf
                    data: null // get pdf source from raw data of a pdf
                };

                getdatafilePDF();

                function getdatafilePDF() {
                    $ctrl.urldoc = appSettings.serverPath + "/Viewfile/viewfileonline?tbid=" + idselect.id;
                }

                $ctrl.save = function () {
                    blockUI.start();
                    let url = "API/QuanLyTaiSan/getviewpdftb?id=" + idselect.id;
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
                            thongbao.centererror("Thông báo", "Không tìm thấy file");
                        });
                }
                $ctrl.printfile = function () {
                    blockUI.start();
                    let url = "API/QuanLyTaiSan/getviewpdftb?id=" + idselect.id;
                    var resp = loginservice.getdatafile(url);
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
                            thongbao.centererror("Thông báo", "Không tìm thấy file");
                        });
                }
                $ctrl.onPageLoad = function (page) {
                    $ctrl.page1 = page;
                    $ctrl.pageview = page;
                };
            }])
        .controller('viewfilepdfCalCtrl', [
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
                idselect)
            {
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

                $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=5&id=" + idselect.ID;

                $ctrl.pdf = {
                    src: '',  
                    data: null 
                };

                $ctrl.save = function () {
                    blockUI.start();
                    var resp = loginservice.getdatafile("api/cal_sukien/getviewpdfCal?id=" + idselect.ID);
                    resp.then(function (response) {
                        blockUI.stop();
                        var headers = response.headers();
                        var contentType = headers['content-type'];
                        var file = new Blob([response.data], { type: contentType });
                        saveAs(file, idselect.MoTa);
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                            thongbao.errorcenter('Không tìm thấy file');
                        });
                }

                $ctrl.printfile = function () {
                    blockUI.start();
                    var resp = loginservice.getdatafile("api/cal_sukien/getviewpdfCal?id=" + idselect.ID);
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
                    $ctrl.page1 = page;
                    $ctrl.pageview = page;
                };
            }])
        .controller('viewfilepdftailieuCtrl', [
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
                idselect)
            {
                var $ctrl = this;

                $ctrl.Print = true;

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

                $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=6&id=" + idselect.TaiLieuId;
   
                $ctrl.onPageLoad = function (page) {
                    $ctrl.page1 = page;
                    $ctrl.pageview = page;
                };

                $ctrl.save = function () {
                    blockUI.start();
                    var resp = loginservice.getdatafile("api/tailieu/getviewpdf?id=" + idselect.TaiLieuId);
                    resp.then(function (response) {
                        blockUI.stop();
                        var headers = response.headers();
                        var filename = headers['x-filename'];
                        var contentType = headers['content-type'];
                        var file = new Blob([response.data], { type: contentType });
                        saveAs(file, idselect.MoTa);
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                            thongbao.errorcenter("Không tìm thấy file");
                        });
                }

                $ctrl.printfile = function () {
                    blockUI.start();
                    var resp = loginservice.getdatafile("api/tailieu/getviewpdf?id=" + idselect.TaiLieuId);
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
            }])
        .controller('viewfilepdftailieuversionCtrl', [
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
                idselect)
            {
                var $ctrl = this;

                $ctrl.Print = true;

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

                $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=7&id=" + idselect.VersionId;

                $ctrl.onPageLoad = function (page) {
                    $ctrl.page1 = page;
                    $ctrl.pageview = page;
                };

                $ctrl.save = function () {
                    blockUI.start();
                    var resp = loginservice.getdatafile("api/tailieu/getviewpdfver?id=" + idselect.VersionId);
                    resp.then(function (response) {
                        blockUI.stop();
                        var headers = response.headers();
                        var filename = headers['x-filename'];
                        var contentType = headers['content-type'];
                        var file = new Blob([response.data], { type: contentType });
                        saveAs(file, idselect.MoTa);
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                            thongbao.errorcenter("Không tìm thấy file");
                        });
                }

                $ctrl.printfile = function () {
                    blockUI.start();
                    var resp = loginservice.getdatafile("api/tailieu/getviewpdfver?id=" + idselect.VersionId);
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
            }])
        .controller('viewfileSMSCtrl', [
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
                idselect)
            {
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

                $ctrl.pdf = {
                    src: '',  // get pdf source from a URL that points to a pdf
                    data: null // get pdf source from raw data of a pdf
                };

                $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=8&id=" + idselect.FileId;

                $ctrl.save = function () {
                    blockUI.start();
                    var resp = loginservice.getdatafile("api/tinnhan/getviewfile?FileId=" + idselect.FileId);
                    resp.then(function (response) {
                        blockUI.stop();
                        var headers = response.headers();
                        var filename = headers['x-filename'];
                        var contentType = headers['content-type'];
                        var file = new Blob([response.data], { type: contentType });
                        saveAs(file, idselect.FilePath);
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                            thongbao.errorcenter("Không tìm thấy file");
                        });
                }

                $ctrl.printfile = function () {
                    blockUI.start();
                    var resp = loginservice.getdatafile("api/tinnhan/getviewfile?FileId=" + idselect.FileId);
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
                    $ctrl.page1 = page;
                    $ctrl.pageview = page;
                };
            }])
        .controller('viewfileOrtherGopYCtrl', [
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
                idselect)
            {
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

                $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=9&id=" + idselect.ID;

                $ctrl.pdf = {
                    src: '',  // get pdf source from a URL that points to a pdf
                    data: null // get pdf source from raw data of a pdf
                };

                $ctrl.save = function () {
                    blockUI.start();
                    var resp = loginservice.getdatafile("api/other/viewpdfOther_GopY?id=" + idselect.ID);
                    resp.then(function (response) {
                        blockUI.stop();
                        var headers = response.headers();
                        var filename = headers['x-filename'];
                        var contentType = headers['content-type'];
                        var file = new Blob([response.data], { type: contentType });
                        saveAs(file, idselect.MOTA);
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                            thongbao.errorcenter("Không tìm thấy file");
                        });
                }

                $ctrl.printfile = function () {
                    blockUI.start();
                    var resp = loginservice.getdatafile("api/other/viewpdfOther_GopY?id=" + idselect.ID);
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
                    $ctrl.page1 = page;
                    $ctrl.pageview = page;
                };
            }])
        .controller('viewfilepdftbCtrl', [
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
                idselect)
            {
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

                $ctrl.pdf = {
                    src: '',  // get pdf source from a URL that points to a pdf
                    data: null // get pdf source from raw data of a pdf
                };

                getdatafilePDF();

                function getdatafilePDF() {
                    $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=10&id=" + idselect.id;
                }

                $ctrl.save = function () {
                    blockUI.start();
                    let url = "api/QuanLyTaiSan/getviewpdftb?id=" + idselect.id;
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

                $ctrl.printfile = function () {
                    blockUI.start();
                    let url = "api/QuanLyTaiSan/getviewpdftb?id=" + idselect.id;
                    var resp = loginservice.getdatafile(url);
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
                    $ctrl.page1 = page;
                    $ctrl.pageview = page;
                };
            }]);
}());