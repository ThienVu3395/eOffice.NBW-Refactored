angular
    .module("oamsapp")
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
        }
    ]);