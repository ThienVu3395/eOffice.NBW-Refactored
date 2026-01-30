angular
    .module("aims")
    .controller("IndexNghiPhepCtrl", [
        '$window',
        "$scope",
        "ModalService",
        "blockUI",
        "loginservice",
        "appSettings",
        "userProfile",
        "commonConstants",
        function (
            $window,
            $scope,
            ModalService,
            blockUI,
            loginservice,
            appSettings,
            userProfile,
            commonConstants) {
            $scope.vm = {};

            var Userdata = userProfile.getProfile();

            var module = commonConstants.MA_MODULE_DON_NGHI_PHEP;

            var namHienTai = new Date().getFullYear();

            $scope.$on('countThongBaoReport', function (data, mess) {
                FilterTrangThai($scope.LoaiLoc);
            });

            Init();

            function Init() {
                $scope.maxSize = 3;
                $scope.BeginDate = new Date(namHienTai, 0, 1, 0, 0, 0, 0); // 01/01, 00:00:00.000
                $scope.EndDate = new Date(namHienTai, 11, 30, 23, 59, 59, 999);
                $scope.vm.Loai = {};
                $scope.vm.Loai2 = {};
                $scope.quyenuser = {};
                $scope.para = {};
                $scope.para.FilterVanBan = {};
                $scope.para.FilterVanBan.CANBO = Userdata.manhanvien;
                $scope.para.FilterVanBan.PhongBan = {};
                $scope.para.FilterLenhDieuXe = {};
                $scope.para.FilterLenhDieuXe.Filters = [];
                $scope.para.FilterLenhDieuXe.Page = 1;
                KhoiTaoDatePicker();
                GetQuyenUserVB(0);
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
                resp.then(
                    function successCallback(response) {
                        //const nhanVienDangNhap = $scope.DanhSachNhanVien[$scope.DanhSachNhanVien.findIndex(f => parseInt(f.maNhanVien) == parseInt(Userdata.manhanvien))];
                        console.log(response.data);
                        let vaitrotao = commonConstants.VAI_TRO_KHONG_CAN_TAO_DON_NGHI_PHEP;
                        //$scope.quyenuser.TaoVB = vaitrotao.includes(nhanVienDangNhap.idChucVu) ? false : true;
                        $scope.quyenuser.TaoVB = response.data
                            .findIndex(x => x.PermissionAction == commonConstants.TEN_QUYEN_TAO_DON_NGHI_PHEP) > -1 &&
                            !vaitrotao.includes(parseInt(Userdata.manhanvien));

                        $scope.quyenuser.XuatBaoCaoVB = response.data
                            .findIndex(x => x.PermissionAction == commonConstants.TEN_QUYEN_XUAT_BAO_CAO_DON_NGHI_PHEP) > -1;

                        $scope.quyenuser.XemVB = response.data
                            .findIndex(x => x.PermissionAction == commonConstants.TEN_QUYEN_XEM_DON_NGHI_PHEP) > -1;

                        GetDanhSachPhongBan();
                    },
                    function errorCallback(err) {
                    }
                );
            }

            function GetDanhSachPhongBan() {
                let resp = loginservice.getdata("api/QLNghiPhep/GetDanhSachPhongBan");
                resp.then(
                    function successCallback(response) {
                        $scope.dsPhongBan = response.data;
                        response.data.unshift({ id: "All", name: "-- Tất cả (" + $scope.dsPhongBan.length + ") --" });
                        $scope.para.FilterVanBan.PhongBan = $scope.dsPhongBan[0];
                        GetDanhSachNhanVien();
                    },
                    function errorCallback(err) {
                        blockUI.stop();
                    }
                );
            }

            function GetDanhSachNhanVien() {
                $scope.para.FilterLenhDieuXe.PageSize = 1000;
                if ($scope.para.FilterVanBan.PhongBan.id != "All") {
                    let obj = {
                        propertyName: "IdPhongBan",
                        type: 0,
                        value: $scope.para.FilterVanBan.PhongBan.id
                    }
                    $scope.para.FilterLenhDieuXe.Filters.push(obj);
                }
                var resp = loginservice.postdata("api/QLNghiPhep/GetDanhSachNhanVien", $.param($scope.para));
                resp.then(
                    function successCallback(response) {
                        $scope.DanhSachNhanVien = response.data.data;
                        $scope.DanhSachNhanVien.unshift({ tenPhongBan: "-- Tất cả (" + $scope.DanhSachNhanVien.length + ") --", maNhanVien: "0", tenNhanVien: "Tất cả CB/CNV" });
                        $scope.vm.Loai = $scope.DanhSachNhanVien[0];
                        $scope.maNhanVien = $scope.vm.Loai.maNhanVien;
                        $scope.tenNhanVien = $scope.vm.Loai.tenNhanVien;
                        $scope.para.FilterLenhDieuXe.Filters = [];
                        GetDanhSachDon();
                    },
                    function errorCallback(response) {
                        blockUI.stop();
                    }
                );
            }

            function GetDanhSachDon() {
                var resp = loginservice.getdata("api/getCore/getdanhmuchethong?loai=" + appSettings.typeNghiPhep);
                resp.then(
                    function successCallback(response) {
                        response.data.unshift({ ID: 0, CODE: "0", VALUENAME: "-- Tất Cả -- " });
                        $scope.dsDon = response.data;
                        let index = $scope.dsDon.findIndex(x => x.ID == 3726);
                        if (index != -1) {
                            $scope.dsDon.splice(index, 1);
                        }
                        let index2 = $scope.dsDon.findIndex(x => x.ID == 3727);
                        if (index2 != -1) {
                            $scope.dsDon.splice(index2, 1);
                        }
                        $scope.para.FilterVanBan.LoaiVB = $scope.dsDon[0].CODE;
                        GetDanhSachLoaiNghiPhep();
                    },
                    function errorCallback(err) {
                    }
                );
            }

            function GetDanhSachLoaiNghiPhep() {
                var resp = loginservice.getdata("api/QLNghiPhep/GetDanhSachLoaiNghiPhep");
                resp.then(
                    function successCallback(response) {
                        $scope.LoaiNghiPhep = response.data;
                        $scope.LoaiNghiPhep.unshift({ moTa: "-- Tất cả (" + $scope.LoaiNghiPhep.length + ") --", id: "0", soNgay: 0, tenLoaiDiDuong: "Tất cả loại nghỉ" });
                        $scope.vm.Loai2 = $scope.LoaiNghiPhep[0];
                        $scope.loainghiphep = $scope.vm.Loai2.id;
                        LoadfilterTrangThai();
                    },
                    function errorCallback(response) {
                        blockUI.stop();
                    }
                );
            }

            function LoadfilterTrangThai() {
                var resp = loginservice.getdata("api/getCore/getdanhmuchethong?loai=FilterRP");
                resp.then(
                    function successCallback(response) {
                        $scope.FilterTT = response.data;
                        switch ($window.location.search) {
                            case `?${commonConstants.ROUTER_DON_NGHI_PHEP_CHUA_XEM}`:
                                $scope.LoaiLoc = commonConstants.MA_FILTER_DON_NGHI_PHEP_CHUA_XEM;
                                break;
                            case `?${commonConstants.ROUTER_DON_NGHI_PHEP_CHO_DUYET}`:
                                $scope.LoaiLoc = commonConstants.MA_FILTER_DON_NGHI_PHEP_CHO_DUYET;
                                break;
                            case `?${commonConstants.ROUTER_DON_NGHI_PHEP_DA_DUYET}`:
                                $scope.LoaiLoc = commonConstants.MA_FILTER_DON_NGHI_PHEP_DA_DUYET;
                                break;
                            default:
                                $scope.LoaiLoc = response.data[0].CODE;
                                break;
                        }
                        FilterTrangThai($scope.LoaiLoc);
                    },
                    function errorCallback(err) {
                    }
                );
            }

            function FilterTrangThai(Code) {
                $scope.para.FilterVanBan.LoaiLoc = Code;
                LocVanBan();
            }

            function LocVanBan() {
                blockUI.start();
                $scope.para.FilterLenhDieuXe.PageSize = 12;
                let value1 = CheckValueOfDateRange($scope.BeginDate);
                let value2 = CheckValueOfDateRange($scope.EndDate);
                // Json đưa lên sẽ là từng object riêng biệt, vd:
                //{
                //"filters": [
                //    { "propertyName": "TuNgay", "type": 11, "value": "2025-1-1" },
                //    { "propertyName": "DenNgay", "type": 12, "value": "2025-12-30" }
                //],
                //"orFilters": [
                //],
                //"page": 1,
                //"pageSize": 10
                //}
                if (value1 != null) {
                    let obj = {
                        propertyName: "TuNgay",
                        type: 11,
                        value: value1
                    }
                    $scope.para.FilterLenhDieuXe.Filters.push(obj);
                }
                if (value2 != null) {
                    let obj = {
                        propertyName: "DenNgay",
                        type: 12,
                        value: value2
                    }
                    $scope.para.FilterLenhDieuXe.Filters.push(obj);
                }
                if ($scope.para.FilterVanBan.PhongBan.id != "All") {
                    let obj = {
                        propertyName: "PhongBan",
                        type: 0,
                        value: $scope.para.FilterVanBan.PhongBan.name
                    }
                    $scope.para.FilterLenhDieuXe.Filters.push(obj);
                }
                if ($scope.maNhanVien != "0") {
                    let obj = {
                        propertyName: "TenNhanVien",
                        type: 0,
                        value: $scope.tenNhanVien
                    }
                    $scope.para.FilterLenhDieuXe.Filters.push(obj);
                }
                if ($scope.loainghiphep != "0") {
                    let obj = {
                        propertyName: "IdLoaiDiDuong",
                        type: 0,
                        value: $scope.loainghiphep
                    }
                    $scope.para.FilterLenhDieuXe.Filters.push(obj);
                }
                if ($scope.para.FilterVanBan.LoaiVB != 0) {
                    let obj = {
                        propertyName: "Loai",
                        type: 0,
                        value: $scope.para.FilterVanBan.LoaiVB
                    }
                    $scope.para.FilterLenhDieuXe.Filters.push(obj);
                }
                //let obj = {
                //    propertyName: "TrangThai",
                //    type: 1,
                //    value: "0,1,2,3,4"
                //}
                //$scope.para.FilterLenhDieuXe.Filters.push(obj);
                //console.log($scope.para);
                var resp = loginservice.postdata("api/QLNghiPhep/GetListVB", $.param($scope.para));
                resp.then(
                    function successCallback(response) {
                        blockUI.stop();
                        //console.log(response.data);
                        $scope.DsVanBan = response.data.data;
                        if ($scope.DsVanBan.length == 0) {
                            $scope.bigTotalItems = 0;
                        }
                        else {
                            $scope.bigTotalItems = response.data.metadata.totalCount;
                        }
                        $scope.para.FilterLenhDieuXe.Filters = [];
                    },
                    function errorCallback(err) {
                        blockUI.stop();
                    }
                );
            }

            function CheckValueOfDateRange(bd) {
                let value = null;
                if (bd != null) {
                    let bdd = new Date(bd);
                    value = bdd.getFullYear() + "-" + (bdd.getMonth() + 1) + "-" + bdd.getDate();
                }
                else if (bd == null) {
                    value = null;
                }
                return value;
            }

            function OpenDetailModal(item) {
                ModalService.open({
                    templateUrl: 'formChiTietVB.html',
                    controller: 'chiTietVBNghiPhepCtrl',
                    size: 'lg90',
                    resolve: {
                        idselect: function () {
                            let par = {};
                            par.IDVanBan = item.id;
                            par.Module = module;
                            par.Loai = item.loai;
                            return par;
                        }
                    }
                }).then(function () {
                }, function () {
                });
            }

            $scope.PhanTrang = function () {
                FilterTrangThai($scope.LoaiLoc);
            }

            $scope.LocTrangThai = function (Code) {
                $scope.LoaiLoc = Code;
                $scope.para.FilterLenhDieuXe.Page = 1;
                FilterTrangThai($scope.LoaiLoc);
            }

            $scope.TimKiem = function () {
                $scope.para.FilterLenhDieuXe.Page = 1;
                FilterTrangThai($scope.LoaiLoc);
            }

            $scope.Reset = function () {
                $scope.para.FilterVanBan.PhongBan = $scope.dsPhongBan[0];
                $scope.para.FilterVanBan.LoaiVB = $scope.dsDon[0].CODE;
                $scope.vm.Loai = $scope.DanhSachNhanVien[0];
                $scope.vm.Loai2 = $scope.LoaiNghiPhep[0];
                $scope.maNhanVien = $scope.vm.Loai.maNhanVien;
                $scope.tenNhanVien = $scope.vm.Loai.tenNhanVien;
                $scope.loainghiphep = $scope.vm.Loai2.id;
                $scope.BeginDate = null;
                $scope.EndDate = null;
                $scope.para.FilterLenhDieuXe.Page = 1;
                FilterTrangThai($scope.LoaiLoc);
            }

            $scope.DoiPhongBan = function () {
                GetDanhSachNhanVien();
            }

            $scope.DoiNhanVien = function () {
                $scope.maNhanVien = $scope.vm.Loai.maNhanVien;
                $scope.tenNhanVien = $scope.vm.Loai.tenNhanVien;
            }

            $scope.DoiLoaiNghiPhep = function () {
                $scope.loainghiphep = $scope.vm.Loai2.id;
            }

            $scope.openEndDate = function () {
                $scope.ed.opened = true;
            };

            $scope.openBeginDate = function () {
                $scope.bd.opened = true;
            };

            $scope.MoFormThemSuaVB = function (IDVanBan) {
                ModalService.open({
                    templateUrl: 'formThemSuaVB.html',
                    controller: 'themSuaVBNghiPhepCtrl',
                    size: 'lg100',
                    resolve: {
                        idselect: function () {
                            let item = {};
                            item.IDVanBan = IDVanBan;
                            item.Module = module;
                            return item;
                        }
                    }
                }).then(function (c) {
                    FilterTrangThai($scope.LoaiLoc);
                    OpenDetailModal(c); // mở luôn trang chi tiết cho người dùng
                }, function () {
                });
            }

            $scope.MoFormChiTietVB = function (item) {
                ModalService.open({
                    templateUrl: 'formChiTietVB.html',
                    controller: 'chiTietVBNghiPhepCtrl',
                    size: 'lg90',
                    resolve: {
                        idselect: function () {
                            let par = {};
                            par.IDVanBan = item.id;
                            par.Module = module;
                            par.Loai = item.loai;
                            return par;
                        }
                    }
                }).then(function (c) {
                    FilterTrangThai($scope.LoaiLoc);
                }, function () {
                    FilterTrangThai($scope.LoaiLoc);
                });
            }

            $scope.MoFormThongKeBaoCaoNghiPhep = function () {
                ModalService.open({
                    templateUrl: 'modalThongKeBaoCao.html',
                    controller: 'baoCaoThongKeNghiPhepCtrl',
                    size: 'md',
                    resolve: {
                        idselect: function () {
                            return null;
                        }
                    }
                }).then(function (c) {
                }, function () {
                });
            }

            $scope.MoFormThongKeBaoCaoGiaiTrinh = function () {
                ModalService.open({
                    templateUrl: 'modalThongKeGiaiTrinh.html',
                    controller: 'baoCaoThongKeGiaiTrinhCtrl',
                    size: 'md',
                    resolve: {
                        idselect: function () {
                            return null;
                        }
                    }
                }).then(function () {
                }, function () {
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
                            $scope.xl.type = 14;
                            return $scope.xl;
                        }
                    }
                }).then(function (c) {
                }, function () {
                });
            }

            // Ng-If HTML View
            $scope.coTheTaoDon = function () {
                return ($scope.quyenuser.TaoVB);
            }

            $scope.coTheXuatBaoCao = function () {
                return ($scope.quyenuser.XuatBaoCaoVB);
            }

            $scope.coTheTest = function () {
                return Userdata.username == "thienvu.lh";
            }

            // Testing
            $scope.DownloadTestFile = function (module) {
                let payload = {
                    "Module": module,
                    "DanhSachNguoiKy": [
                        {
                            "FullName": "Nguyễn Văn A",
                            "ChucVu": "Người làm đơn"
                        },
                        {
                            "FullName": "Trần Thị B",
                            "ChucVu": "Trưởng phòng TCHC"
                        },
                        {
                            "FullName": "Lê Hoàng Thiên Vũ",
                            "ChucVu": "Giám đốc"
                        }
                    ]
                };

                loginservice.postdatapdffile('api/QLNghiPhep/TaoFileDonNghiPhepPDF', payload)
                    .then(function (res) {
                        const blob = new Blob([res.data], { type: 'application/pdf' });
                        const a = document.createElement('a');
                        a.href = URL.createObjectURL(blob);
                        a.download = 'DonNghiPhep.pdf';
                        a.click();
                        URL.revokeObjectURL(a.href);
                    })
                    .catch(function (err) {
                        // Nếu backend trả lỗi JSON nhưng mình set arraybuffer,
                        // có thể convert về string để xem thông báo:
                        try {
                            var decoder = new TextDecoder('utf-8');
                            console.error('ERR:', decoder.decode(err.data));
                        } catch (e) {
                            console.error('ERR:', err);
                        }
                    });
            };
        }
    ]);