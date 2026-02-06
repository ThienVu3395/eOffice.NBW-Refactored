angular
    .module("aims")
    .controller("IndexCtrl", [
        '$window',
        "$scope",
        "ModalService",
        "UserProfileService",
        "blockUI",
        "ApiClient",
        "PERMISSIONS",
        "ROUTERS",
        "FILTERS",
        "SPECIAL_ACCOUNTS",
        "MODULES",
        function (
            $window,
            $scope,
            ModalService,
            UserProfileService,
            blockUI,
            ApiClient,
            PERMISSIONS,
            ROUTERS,
            FILTERS,
            SPECIAL_ACCOUNTS,
            MODULES) {
            $scope.vm = {};

            var Userdata = UserProfileService.getProfile();

            var module = MODULES.VAN_BAN_KY_SO.ID;

            Init();

            $scope.$on('countThongBaoReport', function (data, mess) {
                LocVanBan();
            });

            function Init() {
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
                GetQuyenUserVB($scope.para.LoaiVB);
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
                var resp = ApiClient
                    .postForm("api/getCore/getAllPermissionsOfUserByModuleResource", $.param({
                        valstring1: "RP",
                        valstring2: 'G',
                        valstring3: resourceid
                    }))
                    .then(
                        function successCallback(response) {
                            $scope.quyenuser.TaoVB = response.data
                                .findIndex(x => x.PermissionAction == PERMISSIONS.VAN_BAN_KY_SO.CREATE_ALL) > -1;

                            LoadfilterTrangThai();
                        },
                        function errorCallback(response) {
                        }
                    );
            }

            function LoadfilterTrangThai() {
                var resp = ApiClient
                    .get("api/getCore/getdanhmuchethong?loai=FilterRP")
                    .then(
                        function successCallback(response) {
                            $scope.FilterTT = response.data;
                            $scope.FilterTT.splice(-1, 1); // bỏ đi văn bản trong tổ
                            $scope.FilterTT.splice(-1, 1); // bỏ đi văn bản đã hủy
                            LoadLoaiMauDon();
                        },
                        function errorCallback(response) {
                        }
                    );
            }

            function LoadLoaiMauDon() {
                var resp = ApiClient
                    .get("api/getCore/getdanhmuchethong?loai=TypeRP_NP")
                    .then(
                        function successCallback(response) {
                            let dsLoaiVBKhongMau = response.data;
                            var resp = ApiClient
                                .get("api/getCore/getdanhmuchethong?loai=TypeRP")
                                .then(
                                    function successCallback(response) {
                                        let dsLoaiVBCoMau = response.data;
                                        $scope.dsLoaiVB = dsLoaiVBKhongMau.concat(dsLoaiVBCoMau);
                                        $scope.dsLoaiVB.unshift({
                                            ID: 0, CODE: "0",
                                            VALUENAME: "-- Tất Cả -- ",
                                            LOAIDM: "Tất cả loại văn bản",
                                            VALUENAMECODE: "TypeRP_All"
                                        });
                                        $scope.vm.Loai = $scope.dsLoaiVB[0];
                                        $scope.para.LoaiVB = parseInt($scope.vm.Loai.CODE);
                                        KiemTraDuongDan();
                                    },
                                    function errorCallback(response) {
                                    }
                                );
                        },
                        function errorCallback(response) {
                        }
                    );
            }

            function KiemTraDuongDan() {
                switch ($window.location.search) {
                    case `?${ROUTERS.VAN_BAN_KY_SO.CHUA_XEM}`:
                        $scope.para.LoaiLoc = FILTERS.VAN_BAN_KY_SO.CHUA_XEM;
                        break;
                    case `?${ROUTERS.VAN_BAN_KY_SO.CHO_DUYET}`:
                        $scope.para.LoaiLoc = FILTERS.VAN_BAN_KY_SO.CHO_DUYET;
                        break;
                    default:
                        $scope.para.LoaiLoc = $scope.FilterTT[0].CODE;
                        break;
                }
                LocVanBan();
            }

            function LocVanBan() {
                blockUI.start();
                $scope.para.BeginDate = $scope.BeginDate == null ? null : $scope.BeginDate.toDateString();
                $scope.para.EndDate = $scope.EndDate == null ? null : $scope.EndDate.toDateString();
                //$scope.para.OrganSendName = $scope.OrganSendName.GroupId == 0 ? null : $scope.OrganSendName.TENPHONG;
                $scope.para.OrganSendName = null;
                var resp = ApiClient
                    .postData("api/QLVBKySo/GetListVB", $.param($scope.para))
                    .then(
                        function successCallback(response) {
                            blockUI.stop();
                            $scope.DsVanBan = response.data;
                            if (response.data.length == 0) {
                                $scope.bigTotalItems = 0;
                            }
                            else {
                                $scope.bigTotalItems = response.data[0].Total;
                            }
                        },
                        function errorCallback(response) {
                            blockUI.stop();
                        }
                    );
            }

            function OpenDetailModal(idvb) {
                ModalService.open({
                    templateUrl: 'formChiTietVB.html',
                    controller: 'chiTietVBKySoCtrl',
                    size: 'lg90',
                    resolve: {
                        idselect: function () {
                            let par = {};
                            par.IDVanBan = idvb;
                            par.Module = module;
                            return par;
                        }
                    }
                }).then(
                    function successCallback() {
                    },
                    function errorCallback() {
                    }
                );
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

            $scope.Viewfilepdf = function (ID) {
                ModalService.open({
                    templateUrl: 'viewPDFonline.html',
                    controller: 'viewFileReportCtrl',
                    size: 'lg100',
                    resolve: {
                        idselect: function () {
                            $scope.xl = {};
                            $scope.xl.id = ID;
                            $scope.xl.type = 11;
                            return $scope.xl;
                        }
                    }
                }).then(
                    function successCallback() {
                    },
                    function errorCallback() {
                    }
                );
            }

            $scope.MoFormThemSuaVB = function (IDVanBan) {
                ModalService.open({
                    templateUrl: 'formThemSuaVB.html',
                    controller: 'themSuaVBKySoCtrl',
                    size: 'lg100',
                    resolve: {
                        idselect: function () {
                            let item = {};
                            item.IDVanBan = IDVanBan;
                            item.Module = module;
                            return item;
                        }
                    }
                }).then(
                    function successCallback(id) {
                        LocVanBan();
                        OpenDetailModal(id); // mở luôn trang chi tiết cho người dùng
                    },
                    function errorCallback() {
                        LocVanBan();
                    }
                );
            }

            $scope.MoFormChiTietVB = function (item) {
                ModalService.open({
                    templateUrl: 'formChiTietVB.html',
                    controller: 'chiTietVBKySoCtrl',
                    size: 'lg90',
                    resolve: {
                        idselect: function () {
                            let par = {};
                            par.IDVanBan = item.ID;
                            par.Module = module;
                            return par;
                        }
                    }
                }).then(
                    function successCallback() {
                        LocVanBan();
                    },
                    function errorCallback() {
                        LocVanBan();
                    }
                );
            }

            // Ng-If HTML View
            $scope.coTheTaoDon = function () {
                return ($scope.quyenuser.TaoVB || SPECIAL_ACCOUNTS.ADMIN.includes(Userdata.username));
            }
        }
    ]);