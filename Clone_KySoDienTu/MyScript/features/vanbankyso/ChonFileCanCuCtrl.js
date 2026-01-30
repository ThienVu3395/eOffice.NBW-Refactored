angular
    .module("aims")
    .controller('chonFileCanCuCtrl', [
        "$uibModalInstance",
        "blockUI",
        "loginservice",
        "idselect",
        "ModalService",
        "appSettings",
        function (
            $uibModalInstance,
            blockUI,
            loginservice,
            idselect,
            ModalService,
            appSettings) {
            var $ctrl = this;

            $ctrl.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            }

            Init();

            function KhoiTaoDatePicker() {
                $ctrl.bd = {
                    opened: false
                };

                $ctrl.ed = {
                    opened: false
                };
            }

            function Init() {
                $ctrl.dsFileCanCu = [];
                $ctrl.dsFileDaChon = idselect.FileCanCu;
                $ctrl.maxSize = 3;
                $ctrl.bigCurrentPage = 1;
                $ctrl.itemsPerPage = 5;
                $ctrl.BeginDate = null;
                $ctrl.EndDate = null;
                $ctrl.para = {};
                $ctrl.para.CodeNumber = null;
                $ctrl.para.FileNotation = null;
                $ctrl.para.SearchString = null;
                $ctrl.para.Start = ($ctrl.bigCurrentPage - 1) * $ctrl.itemsPerPage;
                $ctrl.para.End = $ctrl.itemsPerPage;
                KhoiTaoDatePicker();
                GetFileCanCu();
            }

            function GetFileCanCu() {
                $ctrl.para.BeginDate = $ctrl.BeginDate == null ? null : $ctrl.BeginDate.toDateString();
                $ctrl.para.EndDate = $ctrl.EndDate == null ? null : $ctrl.EndDate.toDateString();
                var resp = loginservice.postdata("api/QLBaoCao/GetFilesCanCu", $.param($ctrl.para));
                resp.then(function (response) {
                    $ctrl.dsFileCanCu = response.data;
                    if ($ctrl.dsFileCanCu.length == 0) {
                        $ctrl.bigTotalItems = 0;
                    }
                    else {
                        $ctrl.bigTotalItems = $ctrl.dsFileCanCu[0].Total;
                    }
                    if ($ctrl.dsFileDaChon.length > 0) {
                        $ctrl.dsFileDaChon.forEach(function (item) {
                            let index = $ctrl.dsFileCanCu.findIndex(x => x.TENFILE == item.TENFILE);
                            if (index != -1) {
                                $ctrl.dsFileCanCu.splice(index, 1);
                            }
                        })
                    }
                },
                    function errorCallback(response) {
                    });
            }

            $ctrl.PhanTrang = function () {
                $ctrl.para.Start = ($ctrl.bigCurrentPage - 1) * $ctrl.itemsPerPage;
                GetFileCanCu();
            }

            $ctrl.TimKiem = function () {
                $ctrl.para.Start = 0;
                GetFileCanCu();
            }

            $ctrl.Reset = function () {
                $ctrl.BeginDate = null;
                $ctrl.EndDate = null;
                $ctrl.para.CodeNumber = null;
                $ctrl.para.FileNotation = null;
                $ctrl.para.SearchString = null;
                $ctrl.bigCurrentPage = 1;
                $ctrl.para.Start = ($ctrl.bigCurrentPage - 1) * $ctrl.itemsPerPage;
                GetFileCanCu();
            }

            $ctrl.openBeginDate = function () {
                $ctrl.bd.opened = true;
            };

            $ctrl.openEndDate = function () {
                $ctrl.ed.opened = true;
            };

            $ctrl.ViewFile = function (item) {
                if (item.LOAIFILE == "PDF" || item.LOAIFILE == "pdf") {
                    ModalService.open({
                        templateUrl: 'viewPDFonline.html',
                        controller: item.VITRIFILE,
                        size: 'lg100',
                        resolve: {
                            idselect: function () {
                                let xl = {};
                                xl.id = item.IDFile;
                                xl.type = item.Module;
                                xl.loaivb = item.Module == 3 ? 0 : 1; // chỉ dành cho xem file công văn đến/đi
                                xl.TaiLieuId = item.ID; // chỉ dành cho xem file tài liệu
                                return xl;
                            }
                        }
                    }).then(function () {
                    }, function () {
                        blockUI.stop();
                    });
                }
                else {
                    let result = item.NGAYTAO.split("T")[0].split("-");
                    let a = document.createElement('a');
                    let physicalFileLink = appSettings.serverPath;
                    a.target = "_blank";
                    switch (item.Module) {
                        case 11: // văn bản ký số
                            physicalFileLink += '/Report/ReportFile/' + result[0] + '/' + result[1] + '/' + item.TENFILE;
                            break;
                        case 3: // công văn đến
                            physicalFileLink += '/CongVanFile/' + result[0] + '/' + result[1] + '/' + item.TENFILE;
                            break;
                        case 4: // công văn đi
                            physicalFileLink += '/CongVanFile/' + result[0] + '/' + result[1] + '/' + item.TENFILE;
                            break;
                        case -1: // tài liệu
                            physicalFileLink += '/DocData/' + item.NhomId + '/' + item.TENFILE;
                            break;
                        default:
                    }
                    a.href = physicalFileLink;
                    a.click();
                }
            }

            $ctrl.ChonFile = function (item, idx) {
                if (idselect.IdVB != null) {
                    blockUI.start();
                    item.VANBANID = idselect.IdVB;
                    var resp = loginservice.postdata("api/QLBaoCao/UploadFileCanCu", $.param(item));
                    resp.then(function (response) {
                        blockUI.stop();
                        let nht = new Date();
                        item.ID = response.data.valint1;
                        item.TENFILE = response.data.valstring1;
                        item.NGAYTAO = nht.getFullYear() + "-" + ((nht.getMonth() + 1) >= 10 ? (nht.getMonth() + 1) : "0" + (nht.getMonth() + 1)) + "-" + nht.getDate() + "T00:00:00";
                        $ctrl.dsFileCanCu.splice(idx, 1);
                        $ctrl.dsFileDaChon.push(item);
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }
                else {
                    $ctrl.dsFileCanCu.splice(idx, 1);
                    $ctrl.dsFileDaChon.push(item);
                }
            };

            $ctrl.XoaFile = function (item, idx) {
                if (idselect.IdVB != null) {
                    var resp = loginservice.postdata("api/QLBaoCao/RemoveFileCanCu", $.param({ valint1: item.ID }));
                    resp.then(function (response) {
                        $ctrl.dsFileCanCu.push(item);
                        $ctrl.dsFileDaChon.splice(idx, 1);
                        blockUI.stop();
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }
                else {
                    $ctrl.dsFileCanCu.push(item);
                    $ctrl.dsFileDaChon.splice(idx, 1);
                }
            };
        }
    ]);                                                          