(function () {
    "use strict"
    angular.module("oamsapp")
        .controller('viewfileCtrl', [
            "thongbao",
            "$scope",
            "blockUI",
            "loginservice",
            function (
                thongbao,
                $scope,
                blockUI,
                loginservice)
            {
                $scope.pdf = {
                    src: '',  // get pdf source from a URL that points to a pdf
                    data: null // get pdf source from raw data of a pdf
                };

                $scope.Permispdf = {
                    download: 'false',  // get pdf source from a URL that points to a pdf
                    print: 'false' // get pdf source from raw data of a pdf
                };

                var id = 0;

                var url = Settings.serverPath + '/Viewfile/viewfileonline?id=0';

                var api = '';

                if (getParameterByName("tid")) {
                    id = getParameterByName("tid");
                    api = 'api/fileUpload/getviewpdf?file=' + id;
                    getdatafilePDF();
                }

                if (getParameterByName("cvid")) {
                    id = getParameterByName("cvid");
                    let type = getParameterByName("type");
                    api = 'api/congviec/Workflow?id=' + id + '&type=' + type;
                    getdatafilePDF();
                }

                if (getParameterByName("xlid")) {
                    id = getParameterByName("xlid");
                    api = 'api/congviec/getviewpdfxl?id=' + id;
                    getdatafilePDF();
                }

                if (getParameterByName("tbid")) {
                    id = getParameterByName("tbid");
                    api = 'API/QuanLyTaiSan/getviewpdftb?id=' + id;
                    getdatafilePDF();
                }

                if (getParameterByName("vbid")) {
                    id = getParameterByName("vbid");
                    let loai = getParameterByName("loai");
                    api = 'api/QLVanBan/getviewpdfvb?id=' + id + "&loaivb=" + loai;
                    getdatafilePDF();
                }

                if (getParameterByName("calid")) {
                    id = getParameterByName("calid");
                    api = 'api/cal_sukien/getviewpdfCal?id=' + id;
                    getdatafilePDF();
                }

                if (getParameterByName("TaiLieuId")) {
                    id = getParameterByName("TaiLieuId");
                    api = 'api/tailieu/getviewpdf?id=' + id;
                    getdatafilePDF();
                }

                if (getParameterByName("VersionId")) {
                    id = getParameterByName("VersionId");
                    api = 'api/tailieu/getviewpdfver?id=' + id;
                    getdatafilePDF();
                }
                if (getParameterByName("FileId")) {
                    id = getParameterByName("FileId");
                    api = 'api/tinnhan/getviewfile?FileId=' + id;
                    getdatafilePDF();
                }

                if (getParameterByName("otherid")) {
                    id = getParameterByName("otherid");
                    api = 'api/other/viewpdfOther_GopY?id=' + id;
                    getdatafilePDF();
                }
                function getdatafilePDF() {
                    blockUI.start();
                    var resp = loginservice.getdatafile(api);
                    resp.then(function (response) {
                        $scope.pdf.data = new Uint8Array(response.data);
                        blockUI.stop();
                    }
                    , function errorCallback(response) {
                        $scope.pdf.data = null;
                        blockUI.stop();
                        thongbao.errorcenter('Không tìm thấy file');
                    });
                }               
            }]);
}());