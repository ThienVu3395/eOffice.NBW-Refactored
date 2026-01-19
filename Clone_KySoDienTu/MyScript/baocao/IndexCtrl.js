(function () {
    "use strict";
    angular.module("oamsapp")
        .controller("IndexCtrl", [
            '$window',
            "$scope",
            "ModalService",
            "blockUI",
            "loginservice",
            function (
                $window,
                $scope,
                ModalService,
                blockUI,
                loginservice) {
                Init();

                var isOpenModal = false;

                //$scope.$on('countThongBaoReport', function (data, mess) {
                //    if (!isOpenModal) {
                //        LocVanBan();
                //    }
                //});

                function wait(ms) {
                    return new Promise(resolve => setTimeout(resolve, ms));
                }

                function LoadfilterTrangThai() {
                    var resp = loginservice.getdata("api/getCore/getdanhmuchethong?loai=FilterRP");
                    resp.then(function (response) {
                        $scope.FilterTT = response.data;
                        $scope.para.LoaiLoc = response.data[0].CODE;
                    }
                        , function errorCallback(response) {
                        });
                }

                function LoadLoaiMauDon() {
                    var resp = loginservice.getdata("api/getCore/getdanhmuchethong?loai=TypeRP");
                    resp.then(function (response) {
                        response.data.unshift({ ID: 0, CODE: "0", VALUENAME: "-- Tất Cả -- " });
                        $scope.LoaiCongVan = response.data;
                        $scope.para.TypeName = response.data[0].VALUENAME;
                        $scope.para.SoVanBanID = response.data[0].ID;
                    }
                        , function errorCallback(response) {
                        });
                }

                function KhoiTaoDatePicker() {
                    $scope.bd = {
                        opened: false
                    };

                    $scope.ed = {
                        opened: false
                    };
                }

                function GetQuyenUserVB(resourceid) {
                    var resp = loginservice.postdata("api/getCore/getAllPermissionsOfUserByModuleResource", $.param({ valstring1: "WF", valstring2: 'G', valstring3: resourceid }));
                    resp.then(function (response) {
                        $scope.quyenuser.TaoVBDen = response.data.findIndex(x => x.PermissionAction == 'CINC') > -1;
                    }
                        , function errorCallback(response) {
                        });
                }

                async function Init() {
                    $scope.maxSize = 3;
                    $scope.bigCurrentPage = 1;
                    $scope.itemsPerPage = 10;
                    $scope.BeginDate = null;
                    $scope.EndDate = null;
                    $scope.quyenuser = {};
                    $scope.para = {};
                    $scope.para.Start = ($scope.bigCurrentPage - 1) * $scope.itemsPerPage;
                    $scope.para.End = $scope.itemsPerPage;
                    $scope.para.SearchString = null;
                    $scope.para.FileNotation = null;
                    $scope.para.CodeNumber = null;
                    KhoiTaoDatePicker();
                    var tasks = [
                        async () => {
                            await wait(100);
                            LoadfilterTrangThai();
                        },
                        async () => {
                            await wait(200);
                            LoadLoaiMauDon();
                        },
                        async () => {
                            await wait(300);
                            KiemTraDuongDan();
                        },
                        async () => {
                            await wait(400);
                            GetQuyenUserVB($scope.para.LoaiVB);
                        },
                        async () => {
                            await wait(1000, 'LocVanBan');
                            LocVanBan();
                        }
                    ];
                    await Promise.all(tasks.map(p => p()));
                }

                function LocVanBan() {
                    blockUI.start();
                    $scope.para.BeginDate = $scope.BeginDate == null ? null : $scope.BeginDate.toDateString();
                    $scope.para.EndDate = $scope.EndDate == null ? null : $scope.EndDate.toDateString();
                    var resp = loginservice.postdata("api/QLBaoCao/GetListVB", $.param($scope.para));
                    resp.then(function (response) {
                        blockUI.stop();
                        $scope.DsVanBan = response.data;
                        if (response.data.length == 0) {
                            $scope.bigTotalItems = 0;
                        }
                        else {
                            $scope.bigTotalItems = response.data[0].Total;
                        }
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }

                function KiemTraDuongDan() {
                    switch ($window.location.search) {
                        case '?doncx':
                            $scope.para.LoaiLoc = 1;
                            break;
                        case '?doncd':
                            $scope.para.LoaiLoc = 8;
                            break;
                        default:
                            break;
                    }
                }

                $scope.PhanTrang = function () {
                    $scope.para.Start = ($scope.bigCurrentPage - 1) * $scope.para.End;
                    LocVanBan();
                }

                $scope.LocTrangThai = function (Code) {
                    $scope.para.LoaiLoc = Code;
                    $scope.bigCurrentPage = 1;
                    $scope.para.Start = ($scope.bigCurrentPage - 1) * $scope.para.End;
                    LocVanBan();
                }

                $scope.TimKiem = function () {
                    $scope.para.Start = 0;
                    LocVanBan();
                }

                $scope.Reset = function () {
                    $scope.para.Start = 0;
                    $scope.para.SearchString = null;
                    $scope.para.FileNotation = null;
                    $scope.para.CodeNumber = null;
                    $scope.BeginDate = null;
                    $scope.EndDate = null;
                    LocVanBan();
                }

                $scope.openBeginDate = function () {
                    $scope.bd.opened = true;
                };

                $scope.openEndDate = function () {
                    $scope.ed.opened = true;
                };

                $scope.MoFormThemSuaVB = function (IDVanBan) {
                    ModalService.open({
                        templateUrl: 'formThemSuaVB.html',
                        controller: 'themSuaVBCtrl',
                        size: 'lg100',
                        resolve: {
                            idselect: function () {
                                let item = {};
                                item.IDVanBan = IDVanBan;
                                item.SoVanBanID = $scope.para.SoVanBanID;
                                item.TypeName = $scope.para.TypeName;
                                item.Module = 2;
                                return item;
                            }
                        }
                    }).then(function () {
                        LocVanBan();
                    }, function () {
                    });
                }

                $scope.MoFormChiTietVB = function (item) {
                    isOpenModal = true;
                    ModalService.open({
                        templateUrl: 'formChiTietVB.html',
                        controller: 'chiTietVBCtrl',
                        size: 'lg90',
                        resolve: {
                            idselect: function () {
                                let par = {};
                                par.isOpenModal = isOpenModal;
                                par.IDVanBan = item.ID;
                                par.SoVanBanID = $scope.para.SoVanBanID;
                                par.TypeName = $scope.para.TypeName;
                                par.Module = 2;
                                return par;
                            }
                        }
                    }).then(function () {
                        LocVanBan();
                        isOpenModal = false;
                    }, function () {
                        LocVanBan();
                    });
                }

                $scope.Viewfilepdf = function (ID) {
                    ModalService.open({
                        templateUrl: 'viewPDFonline.html',
                        controller: 'viewfilepdfReportCtrl',
                        size: 'lg100',
                        resolve: {
                            idselect: function () {
                                $scope.xl = {};
                                $scope.xl.id = ID;
                                $scope.xl.type = 11;
                                return $scope.xl;
                            }
                        }
                    }).then(function () {
                    }, function () {
                    });
                }

                $scope.CreateReport = function (IDVanBan) {
                    ModalService.open({
                        templateUrl: 'formThemSuaCR.html',
                        controller: 'AddEditCRCtrl',
                        resolve: {
                            idselect: function () {
                                let item = {};
                                item.IDVanBan = IDVanBan;
                                return item;
                            }
                        }
                    }).then(function () {
                    }, function () {
                    });
                }

                //Test xuất report
                $scope.GetReport = function () {
                    var jsonParam = { valint1: 1 };
                    var serviceUrl = "../Employee/GenerateEmployeeReport/";
                    var resp = loginservice.postdata(serviceUrl, $.param(jsonParam));
                    resp.then(function (response) {
                        $window.open("../Report/ReportViewe.aspx", "_newtab");
                    }
                        , function errorCallback(response) {
                            thongbao.error("Found Error");
                        });
                }
            }])
        .controller("AddEditCRCtrl", [
            '$window',
            'thongbao',
            "$scope",
            "loginservice",
            "idselect",
            "ModalService",
            "$uibModalInstance",
            function (
                $window,
                thongbao,
                $scope,
                loginservice,
                idselect,
                ModalService,
                $uibModalInstance)
            {
                var $ctrl = this;

                Init();

                function Init() {
                    $ctrl.para = {};
                    $ctrl.IssueDate = new Date();
                    $ctrl.DueDate = new Date();
                    KhoiTaoSoanThaoTini();
                    KhoiTaoDatePicker();
                    if (idselect.IDVanBan == null) {
                        $ctrl.TieuDeForm = 0;
                    }
                    else {
                        $ctrl.TieuDeForm = 1;
                    }
                }

                function KhoiTaoSoanThaoTini() {
                    // Khai báo đối tượng dùng để soạn thảo văn bản
                    $scope.tinymceOptions = {
                        onChange: function (e) {
                            // put logic here for keypress and cut/paste changes
                        },
                        menubar: false,
                        toolbar: 'undo redo | bold italic underline | forecolor bullist',
                        plugins: 'textcolor advlist',
                        skin: 'lightgray',
                        theme: 'modern',
                        content_css: "/Scripts/tinymce/tinyMod.css"
                    };
                }

                function KhoiTaoDatePicker() {
                    $ctrl.NgayBanHanh = {
                        opened: false
                    };

                    $ctrl.NgayHieuLuc = {
                        opened: false
                    };
                }

                function LoadReport(ID) {
                    var jsonParam = { valint1: ID };
                    var serviceUrl = "../Employee/GenerateEmployeeReport/";
                    var resp = loginservice.postdata(serviceUrl, $.param(jsonParam));
                    resp.then(function (response) {
                        $window.open("../Report/ReportViewe.aspx", "_newtab");
                    }
                        , function errorCallback(response) {
                            thongbao.error("Found Error");
                        });
                }

                $ctrl.openNgayBanHanh = function () {
                    $ctrl.NgayBanHanh.opened = true;
                };

                $ctrl.openNgayHieuLuc = function () {
                    $ctrl.NgayHieuLuc.opened = true;
                };

                $ctrl.cancel = function () {
                    $uibModalInstance.dismiss('cancel');
                };

                $ctrl.TaoMoi = function () {
                    ModalService.open({
                        templateUrl: 'ConfirmModal.html',
                        controller: 'modalConfirmInstanceCtrl',
                        size: 'md',
                        resolve: {
                            confirmMessage: function () {
                                return 'Đồng ý thêm mới Report ?';
                            }
                        }
                    }).then(function () {
                        $ctrl.para.valdate1 = $ctrl.IssueDate == null ? null : $ctrl.IssueDate.toDateString();
                        $ctrl.para.valdate2 = $ctrl.DueDate == null ? null : $ctrl.DueDate.toDateString();
                        var resp = loginservice.postdata("api/QLBaoCao/ThemReport", $.param($ctrl.para));
                        resp.then(
                            function successCallback(response) {
                                thongbao.success("Thêm Report thành công");
                                LoadReport(response.data);
                            },
                            function errorCallback(response) {
                            }
                        );
                    }, function () {
                        blockUI.stop();
                    });
                }
            }])
        .controller("Index2Ctrl", [
            '$window',
            "$scope",
            "ModalService",
            "blockUI",
            "loginservice",
            function (
                $window,
                $scope,
                ModalService,
                blockUI,
                loginservice)
            {
                $scope.vm = {};

                Init();

                $scope.$on('countThongBaoReport', function (data, mess) {
                    LocVanBan();
                });

                function wait(ms) {
                    return new Promise(resolve => setTimeout(resolve, ms));
                }

                function LoadfilterTrangThai() {
                    var resp = loginservice.getdata("api/getCore/getdanhmuchethong?loai=FilterRP");
                    resp.then(function (response) {
                        $scope.FilterTT = response.data;
                        $scope.para.LoaiLoc = response.data[0].CODE;
                        $scope.FilterTT.splice(-1, 1); // bỏ đi văn bản trong tổ
                        $scope.FilterTT.splice(-1, 1); // bỏ đi văn bản đã hủy
                    }
                        , function errorCallback(response) {
                        });
                }

                function LoadLoaiMauDon() {
                    var resp = loginservice.getdata("api/getCore/getdanhmuchethong?loai=TypeRP_NP");
                    resp.then(function (response) {
                        let dsLoaiVBKhongMau = response.data;
                        var resp = loginservice.getdata("api/getCore/getdanhmuchethong?loai=TypeRP");
                        resp.then(function (response) {
                            let dsLoaiVBCoMau = response.data;
                            $scope.dsLoaiVB = dsLoaiVBKhongMau.concat(dsLoaiVBCoMau);
                            $scope.dsLoaiVB.unshift({ ID: 0, CODE: "0", VALUENAME: "-- Tất Cả -- ", LOAIDM: "Tất cả loại văn bản", VALUENAMECODE: "TypeRP_All" });
                            $scope.vm.Loai = $scope.dsLoaiVB[0];
                            $scope.para.LoaiVB = parseInt($scope.vm.Loai.CODE);
                        }
                            , function errorCallback(response) {
                            });

                    }
                        , function errorCallback(response) {
                        });
                }

                function LoadPhongBan() {
                    var resp = loginservice.getdata("api/getUser/getAllPhongBan");
                    resp.then(function (response) {
                        $scope.DsPhongBan = response.data;
                        $scope.DsPhongBan.unshift({ GroupId: 0, MAPHONG: "ALL", TENPHONG: "-- Tất cả --", Users: null });
                        $scope.OrganSendName = $scope.DsPhongBan[0];
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }

                function KhoiTaoDatePicker() {
                    $scope.bd = {
                        opened: false
                    };

                    $scope.ed = {
                        opened: false
                    };
                }

                function GetQuyenUserVB(resourceid) {
                    var resp = loginservice.postdata("api/getCore/getAllPermissionsOfUserByModuleResource", $.param({ valstring1: "RP", valstring2: 'G', valstring3: resourceid }));
                    resp.then(function (response) {
                        $scope.quyenuser.TaoVB = response.data.findIndex(x => x.PermissionAction == 'CRP_ALL') > -1;
                    }
                        , function errorCallback(response) {
                        });
                }

                async function Init() {
                    $scope.maxSize = 3;
                    $scope.bigCurrentPage = 1;
                    $scope.itemsPerPage = 10;
                    $scope.BeginDate = null;
                    $scope.EndDate = null;
                    $scope.quyenuser = {};
                    $scope.para = {};
                    $scope.para.Start = ($scope.bigCurrentPage - 1) * $scope.itemsPerPage;
                    $scope.para.End = $scope.itemsPerPage;
                    $scope.para.SearchString = null;
                    KhoiTaoDatePicker();
                    //LoadPhongBan();
                    var tasks = [
                        async () => {
                            await wait(100);
                            LoadfilterTrangThai();
                        },
                        async () => {
                            await wait(200);
                            LoadLoaiMauDon();
                        },
                        async () => {
                            await wait(300);
                            KiemTraDuongDan();
                        },
                        async () => {
                            await wait(400);
                            GetQuyenUserVB($scope.para.LoaiVB);
                        },
                        async () => {
                            await wait(1000);
                            LocVanBan();
                        }
                    ];
                    await Promise.all(tasks.map(p => p()));
                }

                function LocVanBan() {
                    blockUI.start();
                    $scope.para.BeginDate = $scope.BeginDate == null ? null : $scope.BeginDate.toDateString();
                    $scope.para.EndDate = $scope.EndDate == null ? null : $scope.EndDate.toDateString();
                    //$scope.para.OrganSendName = $scope.OrganSendName.GroupId == 0 ? null : $scope.OrganSendName.TENPHONG;
                    $scope.para.OrganSendName = null;
                    var resp = loginservice.postdata("api/QLBaoCao/GetListVB2", $.param($scope.para));
                    resp.then(function (response) {
                        blockUI.stop();
                        $scope.DsVanBan = response.data;
                        if (response.data.length == 0) {
                            $scope.bigTotalItems = 0;
                        }
                        else {
                            $scope.bigTotalItems = response.data[0].Total;
                        }
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }

                function KiemTraDuongDan() {
                    switch ($window.location.search) {
                        case '?doncx':
                            $scope.para.LoaiLoc = 4;
                            break;
                        case '?doncd':
                            $scope.para.LoaiLoc = 8;
                            break;
                        default:
                            break;
                    }
                }

                $scope.PhanTrang = function () {
                    $scope.para.Start = ($scope.bigCurrentPage - 1) * $scope.para.End;
                    LocVanBan();
                }

                $scope.LocTrangThai = function (Code) {
                    $scope.para.LoaiLoc = Code;
                    $scope.bigCurrentPage = 1;
                    $scope.para.Start = ($scope.bigCurrentPage - 1) * $scope.para.End;
                    LocVanBan();
                }

                $scope.DoiLoaiVB = function (item) {
                    $scope.para.LoaiVB = parseInt(item.CODE);
                }

                $scope.TimKiem = function () {
                    $scope.para.Start = 0;
                    LocVanBan();
                }

                $scope.Reset = function () {
                    $scope.vm.Loai = $scope.dsLoaiVB[0];
                    $scope.para.LoaiVB = parseInt($scope.vm.Loai.CODE);
                    //$scope.OrganSendName = $scope.DsPhongBan[0];
                    $scope.OrganSendName = null;
                    $scope.para.Start = 0;
                    $scope.para.SearchString = null;
                    $scope.BeginDate = null;
                    $scope.EndDate = null;
                    LocVanBan();
                }

                $scope.openBeginDate = function () {
                    $scope.bd.opened = true;
                };

                $scope.openEndDate = function () {
                    $scope.ed.opened = true;
                };

                function OpenDetailModal(idvb) {
                    ModalService.open({
                        templateUrl: 'formChiTietVB.html',
                        controller: 'chiTietVB2Ctrl',
                        size: 'lg90',
                        resolve: {
                            idselect: function () {
                                let par = {};
                                par.IDVanBan = idvb;
                                par.Module = 2;
                                return par;
                            }
                        }
                    }).then(function () {
                    }, function () {
                    });
                }

                $scope.MoFormThemSuaVB = function (IDVanBan) {
                    ModalService.open({
                        templateUrl: 'formThemSuaVB.html',
                        controller: 'themSuaVB2Ctrl',
                        size: 'lg100',
                        resolve: {
                            idselect: function () {
                                let item = {};
                                item.IDVanBan = IDVanBan;
                                item.Module = 2;
                                return item;
                            }
                        }
                    }).then(function (id) {
                        LocVanBan();
                        OpenDetailModal(id); // mở luôn trang chi tiết cho người dùng
                    }, function () {
                        LocVanBan();
                    });
                }

                $scope.MoFormChiTietVB = function (item) {
                    ModalService.open({
                        templateUrl: 'formChiTietVB.html',
                        controller: 'chiTietVB2Ctrl',
                        size: 'lg90',
                        resolve: {
                            idselect: function () {
                                let par = {};
                                par.IDVanBan = item.ID;
                                par.Module = 2;
                                return par;
                            }
                        }
                    }).then(function () {
                        LocVanBan();
                    }, function () {
                        LocVanBan();
                    });
                }

                $scope.Viewfilepdf = function (ID) {
                    ModalService.open({
                        templateUrl: 'viewPDFonline.html',
                        controller: 'viewfilepdfReportCtrl',
                        size: 'lg100',
                        resolve: {
                            idselect: function () {
                                $scope.xl = {};
                                $scope.xl.id = ID;
                                $scope.xl.type = 11;
                                return $scope.xl;
                            }
                        }
                    }).then(function () {
                    }, function () {
                    });
                }
            }]);
}());