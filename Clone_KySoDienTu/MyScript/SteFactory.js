(function () {
    "use strict";
    angular.module("oamsapp")
        .factory("eventHub", ['$location', "$rootScope", "blockUI", "Hub", "thongbao", "appSettings", "userProfile", "loginservice", '$uibModal','$document',
            function ($location, $rootScope, blockUI, Hub, thongbao, appSettings, userProfile, loginservice, $uibModal, $document) {
                var abc = this;
                $rootScope.quyenuser = {};
                $rootScope.quyenuser.NhanThongBao = false;
                $rootScope.ListItem = [];
                var data = userProfile.getProfile();
                if (data.isLoggedIn) {
                    var accesstoken = data.access_token;
                }

                var hub = new Hub('eventHub', {
                    listeners: {
                        'onConnected': function (id, user) {
                            console.log('test signal');
                            $rootScope.$apply();
                        },
                        'popupEvent': function (item) {
                            toastr.options = {
                                "preventDuplicates": true,
                                "onclick": function () {
                                    testsignalR(item.valint1);
                                }
                            }
                            toastr.success(item.valstring2);
                            $rootScope.$apply();
                        },
                        'workflow': function (type) {
                            $rootScope.$apply();
                            $rootScope.$broadcast('homethongbao', type);
                            if (type == 1)
                                $rootScope.$broadcast('homeIndex', type);
                            if (type == 2 || type == 3)
                                $rootScope.$broadcast('getAll', type);
                        },
                        'gcalevent': function (type) {
                            $rootScope.$apply();
                            if (type == 1 || type == 2)
                                $rootScope.$broadcast('homeGCAL', type);
                            else if (type == 3) {
                                $rootScope.$broadcast('homeGCAL', 2);
                                $rootScope.$broadcast('getCal', type);
                            }
                        },
                        'SteDisConnection': function () {
                            $rootScope.$apply();
                        },
                        'addThongBao': function (user) {
                            $rootScope.$apply();
                        },
                        'addEvent': function (event) {
                            $rootScope.$apply();
                        },
                        'getCount': function (mess) {
                            abc.getCount();
                            $rootScope.$apply();
                        },
                        'getCal': function (mess) {
                            $rootScope.$apply();
                            abc.getCount();
                            $rootScope.$broadcast('getCal', mess);
                        },
                        'updateXuLy': function (mess) {
                            $rootScope.$apply();
                            $rootScope.$broadcast('getAll', mess);
                        },
                        'updateChuaXem': function (mess) {
                            getCountDaXem();
                            $rootScope.$apply();
                            $rootScope.$broadcast('getAll', mess);
                        },
                        'updateCountDuyet': function (mess) {
                            getCountDuyetCv();
                            $rootScope.$apply();
                            $rootScope.$broadcast('getAll', mess);
                        },
                        'updateCountDaXem': function (mess) {
                            getCountDaXem();
                            $rootScope.$apply();
                        },
                        'consoleTimer': function (mess) {
                            console.log("test Timer: " + mess);
                            $rootScope.$apply();
                        },

                        // Module Công Văn
                        'countThongBaoVB': function (mess) {  // Cập nhật SignalR
                            getChuongThongBaoVB();
                            $rootScope.$apply();
                            $rootScope.$broadcast('countThongBaoVB', mess);
                        },

                        'closeModal': function (mess) {  // Đóng modal + hiện message
                            $rootScope.$apply();
                            $rootScope.$broadcast('closeModal',mess);
                        },

                        // Module Tin Tức
                        'countTinTuc': function (mess) {  // Cập nhật Danh sách tin tức
                            $rootScope.$apply();
                            $rootScope.$broadcast('countTinTuc',mess);
                        },

                        // Module tin nhắn
                        'getCountTinNhan': function () {  // Cập nhật lại số lượng thông báo tin nhắn
                            $rootScope.$apply();
                            $rootScope.$broadcast('getCountTinNhan');
                        },

                        // Module khai báo y tế
                        'getCountKhaiBao': function () {  // Cập nhật lại thông báo khai báo y tế
                            getCountKhaiBao();
                            $rootScope.$apply();
                        }
                    },
                    methods: [],

                    queryParams: {
                        'token': accesstoken
                    },

                    rootPath: "/" + appSettings.serverApp + "signalr",

                    errorHandler: function (error) {
                        //console.error(error);
                    },
                    //logging: true
                });

                function testsignalR(id) {
                    var parentElem = angular.element($document[0].querySelector('.content-wrapper'));
                    var modalInstance = $uibModal.open({
                        animation: true,
                        backdrop: 'static',
                        templateUrl: "lichcanhan.html",
                        controller: "lichcanhanCtrl",
                        controllerAs: '$ctrl',
                        size: 'lg',
                        appendTo: parentElem,
                        resolve: {
                            idselect: function () {
                                return id;
                            }
                        }
                    });
                    modalInstance.result.then(function (c) {
                    }, function () {
                    });
                }
                function getCountDaXem() {
                    var resp1 = loginservice.getdata("api/congviec/getdsDaxem");
                    resp1.then(function (response) {
                        $rootScope.countCongViec = response.data;
                        //let i = $rootScope.ListItem.findIndex(x => x.ID == 24)
                        //if (i > -1) {
                        //    $rootScope.ListItem.splice(i, 1);
                        //    $rootScope.ListItem.push({ ID: 24, value: 100 })
                        //}
                        //else {
                        //    $rootScope.ListItem.push({ ID: 24, value: 100 })
                        //}
                        //$rootScope.ListItem.push({ ID: 39, value: 77 })
                        dataInit();
                    },
                        function errorCallback(response) {
                        });
                }
                function getCountDuyetCv() {
                    var resp1 = loginservice.getdata("api/congviec/getCountDuyet");
                    resp1.then(function (response) {
                        $rootScope.countDuyetcv = response.data;
                        dataInit();
                    },
                        function errorCallback(response) {
                        });
                }

                function dataInit() {
                    $rootScope.counttong =
                        $rootScope.countDuyetcv +
                        //$rootScope.countLich + $rootScope.countLichTuChoi + $rootScope.countDuyet +
                        $rootScope.countCongViec +
                        $rootScope.SoLuongTB +
                        $rootScope.SoLuongTBVBDenDB +
                        $rootScope.SoLuongTBDenNDDVChuaXem +

                        $rootScope.SoLuongTBDi +
                        $rootScope.SoLuongTBVBDiDB +
                        $rootScope.SoLuongTBDiNDDVChuaXem +

                        $rootScope.SoLuongTBDenChoDuyet +
                        $rootScope.SoLuongTBVBDenDBChoDuyet +
                        $rootScope.SoLuongTBDenNDDVChoDuyet +

                        $rootScope.SoLuongTBDiChoDuyet +
                        $rootScope.SoLuongTBVBDiDBChoDuyet +
                        $rootScope.SoLuongTBDiNDDVChoDuyet +

                        $rootScope.SoLuongTBXoa +
                        $rootScope.SoLuongTBXoaDi
                }
                //function getQuyen() {
                //    var resp = loginservice.postdata("api/getCore/getAllPermissionsOfUserByModuleResource", $.param({ valstring1: "GCAL", valstring2: 'G', valstring3: 'M' }));
                //    resp.then(function (response) {
                //        $rootScope.viewwwin = response.data.findIndex(x => x.PermissionAction == 'N') > -1;
                //        if ($rootScope.viewwwin) {
                //            var resp;
                //            resp = loginservice.getdata("api/cal_sukien/getLichDuyet");
                //            resp.then(function (response) {
                //                $rootScope.countDuyet = response.data.length;
                //                dataInit();
                //            }, function errorCallback(response) {
                //            });
                //        }
                //    }
                //        , function errorCallback(response) {
                //        });
                //}
                abc.getCount = function () {
                    if (data.isLoggedIn) {
                        //getCountDaXem();
                        //getLichTuChoi();
                        //getCountDuyetCv();
                        getChuongThongBaoVB();
                        //getCountTinNhan();
                        //getCountKhaiBao();
                        //var respd = loginservice.postdata("api/cal_sukien/getSuKienUserByName", $.param({ valint1: 0 }));
                        //respd.then(function (response) {
                        //    $rootScope.countLich = response.data.length;
                        //    dataInit();
                        //}, function errorCallback(response) {
                        //});
                        //getQuyen();
                    }
                    else {
                        $rootScope.counttong = 0;
                    }
                }

                //Load chuông thông báo module công văn
                function getChuongThongBaoVB() {
                    var respd = loginservice.getdata("api/QLVanBan/getListVBDenDiAll_ChuaXemVaXoa");
                    respd.then(function (response) {
                        // Lấy văn bản đến chưa xem
                        $rootScope.SoLuongTB = response.data.SoLuongTB;
                        $rootScope.SoLuongTBVBDenDB = response.data.SoLuongTBVBDenDB;
                        $rootScope.SoLuongTBDenNDDVChuaXem = response.data.SoLuongTBDenNDDVChuaXem;
                        // Lấy văn bản đi chưa xem
                        $rootScope.SoLuongTBDi = response.data.SoLuongTBDi;
                        $rootScope.SoLuongTBVBDiDB = response.data.SoLuongTBVBDiDB;
                        $rootScope.SoLuongTBDiNDDVChuaXem = response.data.SoLuongTBDiNDDVChuaXem;
                        // Lấy văn bản đến chờ duyệt
                        $rootScope.SoLuongTBDenChoDuyet = response.data.SoLuongTBDenChoDuyet;
                        $rootScope.SoLuongTBVBDenDBChoDuyet = response.data.SoLuongTBVBDenDBChoDuyet;
                        $rootScope.SoLuongTBDenNDDVChoDuyet = response.data.SoLuongTBDenNDDVChoDuyet;
                        // Lấy văn bản đi chờ duyệt
                        $rootScope.SoLuongTBDiChoDuyet = response.data.SoLuongTBDiChoDuyet;
                        $rootScope.SoLuongTBVBDiDBChoDuyet = response.data.SoLuongTBVBDiDBChoDuyet;
                        $rootScope.SoLuongTBDiNDDVChoDuyet = response.data.SoLuongTBDiNDDVChoDuyet;
                        // Lấy văn bản đến-đi bị xóa
                        $rootScope.dsTBXoa = response.data.dsTBXoa;
                        $rootScope.dsTBXoaDi = response.data.dsTBXoaDi;
                        $rootScope.SoLuongTBXoa = $rootScope.dsTBXoa.length;
                        $rootScope.SoLuongTBXoaDi = $rootScope.dsTBXoaDi.length;
                        dataInit();
                    }, function errorCallback(response) {
                    });
                }

                function getCountKhaiBao() {
                    var respd = loginservice.postdata("api/other/ThongBaoKhaiBao");
                    respd.then(function (response) {
                        $rootScope.TBKhaiBao = response.data;
                    }, function errorCallback(response) {
                    });
                }

                return abc;
            }])
        .factory("ReportHub", ['$location', "$rootScope", "blockUI", "Hub", "thongbao", "appSettings", "userProfile", "loginservice", '$uibModal', '$document',
            function ($location, $rootScope, blockUI, Hub, thongbao, appSettings, userProfile, loginservice, $uibModal, $document) {
                var abc = this;

                var data = userProfile.getProfile();
                if (data.isLoggedIn) {
                    var accesstoken = data.access_token;
                }

                var hub = new Hub('eventHub', {
                    listeners: {
                        'countThongBaoReport': function (mess) {
                            abc.getCountReport();
                            $rootScope.$apply();
                            $rootScope.$broadcast('countThongBaoReport', mess);
                        },
                    },
                    methods: [],

                    queryParams: {
                        'token': accesstoken
                    },

                    rootPath: "/" + appSettings.serverApp + "signalr",

                    errorHandler: function (error) {
                        //console.error(error);
                    },
                    //logging: true
                });

                abc.getCountReport = function () {
                    getChuongThongBaoReport();
                }

                //Load chuông thông báo Report
                function getChuongThongBaoReport() {
                    var respd = loginservice.getdata("api/QLDieuXe/GetCountThongBao");
                    respd.then(function (response) {
                        //console.log(response.data);
                        $rootScope.SoLuongReportChuaXem = response.data.vanBanKiSoChuaXem ?? 0;
                        $rootScope.SoLuongReportChoDuyet = response.data.vanBanKiSoChoDuyet ?? 0;

                        $rootScope.SoLuongLenhDieuXeChoDuyet = response.data.lenhDieuXeChoDuyet ?? 0;
                        $rootScope.SoLuongUngTruocChoDuyet = response.data.ungTruocNhienLieuChoDuyet ?? 0;
                        $rootScope.SoLuongQuyetToanChoDuyet = response.data.quyetToanNhienLieuChoDuyet ?? 0;
                        $rootScope.SoLuongSoLoTrinhChoDuyet = response.data.soLoTrinhChoDuyet ?? 0;
                        $rootScope.SoLuongPhieuXinXeChoDuyet = response.data.phieuXinXeChoDuyet ?? 0;
                        $rootScope.SoLuongPhieuQuanLyXeChoDuyet = response.data.phieuQuanLyXeChoDuyet ?? 0;

                        $rootScope.SoLuongDonNghiPhepChoDuyet = response.data.donNghiPhepChoDuyet ?? 0;

                        $rootScope.SoLuongPhieuDanhGiaLuuNhap = response.data.phieuDanhGiaLuuNhap ?? 0;
                        $rootScope.SoLuongPhieuDanhGiaChoDuyet = response.data.phieuDanhGiaChoDuyet ?? 0;
                        $rootScope.SoLuongPhieuDanhGiaChoHuy = response.data.phieuDanhGiaChoHuy ?? 0;
                        $rootScope.SoLuongPhieuDanhGiaHuyKy = response.data.phieuDanhGiaHuyKy ?? 0;
                        $rootScope.SoLuongPhieuDanhGiaGoDuyet = response.data.phieuDanhGiaGoDuyet ?? 0;

                        $rootScope.SoLuongBangDanhGiaTongHopLuuNhap = response.data.bangDanhGiaTongHopLuuNhap ?? 0;
                        $rootScope.SoLuongBangDanhGiaTongHopChoDuyet = response.data.bangDanhGiaTongHopChoDuyet ?? 0;
                        $rootScope.SoLuongBangDanhGiaTongHopChoHuy = response.data.bangDanhGiaTongHopChoHuy ?? 0;
                        $rootScope.SoLuongBangDanhGiaTongHopHuyKy = response.data.bangDanhGiaTongHopHuyKy ?? 0;
                        $rootScope.SoLuongBangDanhGiaTongHopGoDuyet = response.data.bangDanhGiaTongHopGoDuyet ?? 0;

                        $rootScope.countreport =
                            $rootScope.SoLuongReportChuaXem
                            + $rootScope.SoLuongReportChoDuyet
                            + $rootScope.SoLuongLenhDieuXeChoDuyet
                            + $rootScope.SoLuongUngTruocChoDuyet
                            + $rootScope.SoLuongQuyetToanChoDuyet
                            + $rootScope.SoLuongSoLoTrinhChoDuyet
                            + $rootScope.SoLuongPhieuXinXeChoDuyet
                            + $rootScope.SoLuongPhieuQuanLyXeChoDuyet

                            + $rootScope.SoLuongDonNghiPhepChoDuyet

                            + $rootScope.SoLuongPhieuDanhGiaLuuNhap
                            + $rootScope.SoLuongPhieuDanhGiaChoDuyet
                            + $rootScope.SoLuongPhieuDanhGiaChoHuy
                            + $rootScope.SoLuongPhieuDanhGiaHuyKy
                            + $rootScope.SoLuongPhieuDanhGiaGoDuyet

                            + $rootScope.SoLuongBangDanhGiaTongHopLuuNhap
                            + $rootScope.SoLuongBangDanhGiaTongHopChoDuyet
                            + $rootScope.SoLuongBangDanhGiaTongHopChoHuy
                            + $rootScope.SoLuongBangDanhGiaTongHopHuyKy
                            + $rootScope.SoLuongBangDanhGiaTongHopGoDuyet
                            ;
                    }, function errorCallback(response) {
                    });
                }

                return abc;
            }])
        .factory("thongbao", ['$document',
            function ($document) {
                this.success = function (noidung = "Thành công") {
                    toastr.success(noidung)
                };
                this.error = function (noidung = "Không thành công") {
                    toastr.error(noidung)
                };
                this.errorcenter = function (noidung) {
                    toastr.options = {
                        "positionClass": "toast-center-center"
                    }
                    toastr.error(noidung)
                };

                var swalWithBootstrapButtons = Swal.mixin({
                    customClass: {
                        confirmButton: 'btn btn-success mx-2',
                        cancelButton: 'btn btn-danger mx-2'
                    },
                    buttonsStyling: false
                })
                this.sweetAlert = function (text) {
                    return swalWithBootstrapButtons.fire({
                        title: 'Thông báo hệ thống !',
                        text: text,
                        //type: 'warning',
                        showCancelButton: true,
                        scrollbarPadding: false,
                        confirmButtonText: 'Đồng ý !',
                        cancelButtonText: 'Không đồng ý !',
                        reverseButtons: true
                    });
                }
                this.sweetAlert2 = function (i) {
                    if (i == 1) {
                        swalWithBootstrapButtons.fire(
                            'Success!',
                            'Operation completed successfully.',
                            'success'
                        )
                    }
                    else {
                        swalWithBootstrapButtons.fire(
                            'Cancelled',
                            'Cancelled successfully.',
                            'error'
                        )
                    }
                }

                return {
                    success: this.success,
                    error: this.error,
                    errorcenter: this.errorcenter,
                    sweetAlert: this.sweetAlert,
                    sweetAlert2: this.sweetAlert2
                }
            }
        ])
        .directive('myFormSubmit', function () {
            return {
                require: '^form',
                scope: {
                    callback: '&myFormSubmit'
                },
                link: function (scope, element, attrs, form) {
                    element.bind('click', function (e) {
                        if (form.$valid) {
                            scope.callback();
                        }
                    });
                }
            };
        })
        .directive('plainTextMaxLength', function ($filter) {
            function stripHtmlTags (v) {
                return String(v).replace(/<[^>]+>/gm, '');
            };
            return {
                restrict: 'A',
                require: 'ngModel',
                link: function (scope, element, attributes, ngModel) {
                    var maxLength, validPlainTextLength;

                    validPlainTextLength = function (v) {
                        if (!v) {
                            return true;
                        }
                        //return stripHtmlTags(v).length <= maxLength
                        return v.length <= maxLength;
                    };
                    maxLength = void 0;
                    scope.$watch(attributes.plainTextMaxLength, function (newValue, oldValue) {
                        if (maxLength !== newValue) {
                            maxLength = newValue;
                            return ngModel.$validate();
                        }
                    });
                    return ngModel.$validators['plainTextMaxLength'] = function (modelValue, viewValue) {

                        if (viewValue.$$unwrapTrustedValue) {
                            return validPlainTextLength(viewValue.$$unwrapTrustedValue());
                        } else {
                            return validPlainTextLength(viewValue);
                        }
                    };

                }
            };
        })
        .filter("hanxuly", function () {
            return function (time, local) {
                if (!time)
                    return "never";

                if (!local) {
                    local = Date.now();
                }

                if (angular.isDate(time)) {
                    time = time.getTime();
                } else if (typeof time === "string") {
                    time = new Date(time).getTime();
                }

                if (angular.isDate(local)) {
                    local = local.getTime();
                } else if (typeof local === "string") {
                    local = new Date(local).getTime();
                }

                if (typeof time !== 'number' || typeof local !== 'number') {
                    return;
                }

                var
                    offset = Math.abs((local - time) / 1000),
                    DAY = 86400
                if (time < local) return 'text-danger';
                else if (offset > (DAY * 2))
                    return 'text-success';
                //else if (offset < (DAY))
                //    return 'label-warning';
                else if (offset < (DAY * 2))
                    return 'text-warning';
            }
        })
        .filter("hanxulylabel", function () {
            return function (time, local) {
                time = new Date(time);
                local = new Date();
                local = new Date(local.getFullYear(), local.getMonth(), local.getDate(), 0, 0);
                return time < local;
            }
        })
        .filter("HanXuLyCongVan", function () {
            return function (time, trangthai) {
                if (!time)
                    return "never";

                var local = Date.now();

                if (angular.isDate(time)) {
                    time = time.getTime();
                } else if (typeof time === "string") {
                    time = new Date(time).getTime();
                }

                if (angular.isDate(local)) {
                    local = local.getTime();
                } else if (typeof local === "string") {
                    local = new Date(local).getTime();
                }

                if (typeof time !== 'number' || typeof local !== 'number') {
                    return;
                }

                if (time <= local) {
                    if (trangthai == 3) {
                        return null;
                    }
                    else return 'text-danger';
                }
                else {
                    return null;
                }
            }
        })
        .filter("timeago", function () {
            //time: the time
            //local: compared to what time? default: now
            //raw: wheter you want in a format of "5 minutes ago", or "5 minutes"
            return function (time, local, raw) {
                if (!time)
                    return "never";

                if (!local) {
                    local = Date.now();
                }

                if (angular.isDate(time)) {
                    time = time.getTime();
                } else if (typeof time === "string") {
                    time = new Date(time).getTime();
                }

                if (angular.isDate(local)) {
                    local = local.getTime();
                } else if (typeof local === "string") {
                    local = new Date(local).getTime();
                }

                if (typeof time !== 'number' || typeof local !== 'number') {
                    return;
                }

                var
                    offset = Math.abs((local - time) / 1000),
                    span = [],
                    MINUTE = 60,
                    HOUR = 3600,
                    DAY = 86400,
                    WEEK = 604800,
                    MONTH = 2592000,
                    YEAR = 31556926,
                    DECADE = 315569260;

                if (offset <= MINUTE) span = ['', raw ? 'now' : 'less than a minute'];
                else if (offset < (MINUTE * 60)) span = [Math.round(Math.abs(offset / MINUTE)), 'min'];
                else if (offset < (HOUR * 24)) span = [Math.round(Math.abs(offset / HOUR)), 'hr'];
                else if (offset < (DAY * 7)) span = [Math.round(Math.abs(offset / DAY)), 'day'];
                else if (offset < (WEEK * 4)) span = [Math.round(Math.abs(offset / WEEK)), 'week'];
                else if (offset < (MONTH * 12)) span = [Math.round(Math.abs(offset / MONTH)), 'month'];
                else if (offset < (YEAR * 10)) span = [Math.round(Math.abs(offset / YEAR)), 'year'];
                else if (offset < (DECADE * 100)) span = [Math.round(Math.abs(offset / DECADE)), 'decade'];
                else span = ['', 'a long time'];

                span[1] += (span[0] === 0 || span[0] > 1) ? 's' : '';
                span = span.join(' ');

                if (raw === true)
                    return span;

                return (time <= local) ? span + ' ago' : 'in ' + span;
            }
        })
        .filter("notifyMenu", function () {
            return function (input, item) {
                let abc = [];
                for (let i = 0; i < input.length; i++) {
                    if (input[i].ID == item && input[i].value > 0) {
                        abc = input[i];
                        break ;
                    }
                }
                return abc;
            }
        })
        .factory("funGroupUITree", ["blockUI", "thongbao", "appSettings", "userProfile", "loginservice",
            function (blockUI, thongbao, appSettings, userProfile, loginservice) {
                var func = this;

                func.GetAllUsersGroups = function (isView, PermissionAction, ModuleKey) {
                    var resp = loginservice.postdata("api/getCore/Core_GetAllUsersGroupsUI", $.param({ valint1: isView, valint2: -1, valstring1: PermissionAction, valstring2: ModuleKey }));
                    return resp;
                }
                func.GetAllUsersGroupsTree = function (isView) {
                    var resp = loginservice.postdata("api/getCore/Core_GetAllUsersGroupsTree", $.param({ valint1: isView, valint2: -1}));
                    return resp;
                }
                func.GetUsersActionID = function (ID) {
                    var resp = loginservice.postdata("api/getCore/Core_GetUsersActionID", $.param({ valint1: ID }));
                    return resp;
                }
                func.DeleteGroup = function (ID) {
                    blockUI.start();
                    let resp = loginservice.postdata("api/getCore/Core_DeleteUITreeGroups", $.param({ valint1: ID }));
                    resp.then(function (response) {
                        blockUI.stop();
                        thongbao.noImage("Xóa thành công", "");
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }
                func.InsertGroup = function (item) {
                    blockUI.start();
                    let resp = loginservice.postdata("api/getCore/Core_InsertUITreeGroups", $.param(item));
                    resp.then(function (response) {
                        blockUI.stop();
                        thongbao.noImage("Thêm thành công", "");
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }
                return func;
            }])
        .controller('modalGCALCtrl', ['$timeout', 'funGroupUITree', "$scope", 'blockUI', '$filter', "$http", "$uibModalInstance", "FileUploader", "appSettings", "loginservice", "userProfile", "idselect", 'uibDateParser',
            function ($timeout, funGroupUITree, $scope, blockUI, $filter, $http, $uibModalInstance, FileUploader, appSettings, loginservice, userProfile, idselect, uibDateParser) {
                var $ctrl = this;

                $ctrl.checklist = [];
                $scope.modalreturn = [];
                $ctrl.dsnhanvien = [];
                if (idselect.listus)
                    var temp = idselect.listus.split(',');
                else
                    var temp = [];
                LoadAll();
                //#region Load
                $ctrl.phongban = [];
                $ctrl.nguoidung = [];
                $ctrl.nhomchucnang = [];
                loadUserSortGroup();
                function loadUserSortGroup() {
                    blockUI.start();
                    var resp = funGroupUITree.GetAllUsersGroups(0, 'RWF', 'CoreGP');
                    resp.then(function (response) {
                        $ctrl.phongban = response.data;
                        $ctrl.phongban.forEach(function (value, key) {
                            myStyles.addRule('tr[name="G' + value.Groups.GroupId + '"]', '{display: none}');
                        });
                        if (idselect.listus) {
                            var hamcho = function () {
                                if ($ctrl.phongban.length == 0) {
                                    $timeout(hamcho, 300);
                                }
                                else {
                                    angular.forEach(temp, function (j, index) {
                                        var item = $ctrl.phongban.find(x => x.Groups.FullName == j);
                                        if (item) {
                                            $ctrl.checklist.push(item);
                                            temp.splice(index, 1);
                                        }
                                    });
                                }
                            }
                            $timeout(hamcho, 300);
                        }
                        else
                            $ctrl.checklist = [];
                        loadUserGroup();
                        blockUI.stop();
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }

                function loadNhom() {
                    blockUI.start();
                    var resp = funGroupUITree.GetAllUsersGroupsTree(1);
                    resp.then(function (response) {
                        $ctrl.nhomchucnang = response.data;
                        $ctrl.nhomchucnang.forEach(function (value, key) {
                            myStyles.addRule('tr[name="C' + value.Groups.GroupId + '"]', '{display: none}');
                        });
                        if (idselect.listus) {
                            var hamcho = function () {
                                if ($ctrl.dsnhanvien.length == 0) {
                                    $timeout(hamcho, 300);
                                }
                                else {
                                    angular.forEach(temp, function (j) {
                                        var i = $ctrl.dsnhanvien.findIndex(x => x.FullName.trim() == j.trim());
                                        if (i > -1) {
                                            $ctrl.checklist.push({ Groups: { FullName: $ctrl.dsnhanvien[i].FullName }, listUsers: [{ UserName: $ctrl.dsnhanvien[i].UserName, FullName: $ctrl.dsnhanvien[i].FullName }] });
                                            $ctrl.dsnhanvien.splice(i, 1);
                                        }
                                    });
                                }
                            }
                            $timeout(hamcho, 300);
                        }
                        else
                            $ctrl.checklist = [];
                        blockUI.stop();
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }
                function loadUserGroup() {
                    blockUI.start();
                    var resp = funGroupUITree.GetAllUsersGroupsTree(0);
                    resp.then(function (response) {
                        $ctrl.nguoidung = response.data;
                        $ctrl.nguoidung.forEach(function (value, key) {
                            myStyles.addRule('tr[name="U' + value.Groups.GroupId + '"]', '{display: none}');
                        });
                        if (idselect.listus) {
                            var hamcho = function () {
                                if ($ctrl.dsnhanvien.length == 0) {
                                    $timeout(hamcho, 300);
                                }
                                else {
                                    angular.forEach(temp, function (j) {
                                        var i = $ctrl.dsnhanvien.findIndex(x => x.FullName.trim() == j.trim());
                                        if (i > -1) {
                                            $ctrl.checklist.push({ Groups: { FullName: $ctrl.dsnhanvien[i].FullName }, listUsers: [{ UserName: $ctrl.dsnhanvien[i].UserName, FullName: $ctrl.dsnhanvien[i].FullName }] });
                                            $ctrl.dsnhanvien.splice(i, 1);
                                        }
                                    });
                                }
                            }
                            $timeout(hamcho, 300);
                        }
                        else
                            $ctrl.checklist = [];
                        loadNhom();
                        blockUI.stop();
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }

                function LoadAll() {
                    var resp = loginservice.postdata("api/getUser/getDanhSachNhanVien", $.param({ valint1: 6 }));
                    resp.then(function (response) {
                        $ctrl.dsnhanvien = response.data;
                    }
                        , function errorCallback(response) {
                        });
                }
                $ctrl.CheckSingle = function (item, checked) {
                    if (checked) {
                        let a = Array.prototype.map.call(item, s => s.FullName.trim());
                        $ctrl.checklist = $ctrl.checklist.filter(x => !a.includes(x.Groups.FullName.trim()));
                        item.forEach(function (value, key) {
                            $ctrl.checklist.push({ Groups: { FullName: value.FullName }, listUsers: [{ UserName: value.UserName, FullName: value.FullName }] });
                        });
                    }
                    else {
                        let a = Array.prototype.map.call(item, s => s.FullName.trim());
                        $ctrl.checklist = $ctrl.checklist.filter(x => !a.includes(x.Groups.FullName.trim()));
                    }

                }
                //#endregion
                $ctrl.LuuNhom = function () {
                    $scope.modalreturn.push(idselect.id);
                    $scope.modalreturn.push($ctrl.checklist);
                    $uibModalInstance.close($scope.modalreturn);
                }
                //#region Giao dien
                $ctrl.myClick = function (i) {
                    var hideThis = document.getElementById(i);
                    var showHide = angular.element(hideThis).attr('class');
                    if (showHide === 'ui-icon ace-icon fa fa-minus center bigger-110 blue') {
                        myStyles.addRule('tr[name="' + i + '"]', '{display: none}');
                        angular.element(hideThis).attr('class', 'ui-icon ace-icon fa fa-plus center bigger-110 blue');
                    }
                    else {
                        myStyles.deleteRule('tr[name="' + i + '"]');
                        angular.element(hideThis).attr('class', 'ui-icon ace-icon fa fa-minus center bigger-110 blue');
                    }
                };
                var myStyles = (function () {
                    var sheet = document.styleSheets[0];
                    function deleteRule(selector) {
                        var rules = sheet.rules || sheet.cssRules;
                        for (var i = 0; i < rules.length; i++) {
                            if (selector == rules[i].selectorText) {
                                sheet.deleteRule(i);
                            }
                        }
                    }

                    function addRule(selector, text) {
                        deleteRule(selector);
                        sheet.insertRule(selector + text);
                    }
                    return {
                        'addRule': addRule,
                        'deleteRule': deleteRule
                    };
                }());
                //#endregion
                //#region Modal
                $ctrl.ok = function () {
                    $ctrl.presult = "0";
                };
                $ctrl.cancel = function () {
                    $uibModalInstance.dismiss('cancel');
                };
                $ctrl.close = function (item) {
                    $uibModalInstance.close(item);
                };
                //#endregion
            }])
        .controller('modalnewCtrl', ['$timeout', 'funGroupUITree', "$scope", 'blockUI', '$filter', "$http", "$uibModalInstance", "FileUploader", "appSettings", "loginservice", "userProfile", "idselect", 'uibDateParser',
            function ($timeout, funGroupUITree, $scope, blockUI, $filter, $http, $uibModalInstance, FileUploader, appSettings, loginservice, userProfile, idselect, uibDateParser) {
                var $ctrl = this;
                $ctrl.checklist = [];
                $scope.modalreturn = [];
                $ctrl.dsnhanvien = [];
                var temp = [];
                if (idselect.listus) {
                    if (idselect.type == 1) {
                        temp = idselect.listus.filter(x => x.VAITRO != idselect.vaitro);
                        angular.forEach(idselect.listus.filter(x => x.VAITRO == idselect.vaitro), function (us) {
                            $ctrl.checklist.push({ UserName: us.UserName, FullName: us.FullName });
                        });

                    }
                }
                //LoadAll();
                //#region Load
                $ctrl.nguoidung = [];
                $ctrl.nhomchucnang = [];
                loadUserGroup();
                function loadUserGroup() {
                    blockUI.start();
                    var resp = funGroupUITree.GetAllUsersGroupsTree(0);
                    resp.then(function (response) {
                        $ctrl.nguoidung = response.data;
                        $ctrl.nguoidung.forEach(function (value, key) {
                            myStyles.addRule('tr[name="U' + value.Groups.GroupId + '"]', '{display: none}');
                            if (temp) {
                                angular.forEach(temp, function (j) {
                                    var i = value.listUsers.findIndex(x => x.UserName== j.UserName);
                                    if (i > -1) {
                                        value.listUsers.splice(i, 1);
                                    }
                                });
                            }
                        });
                        $ctrl.nguoidung = $ctrl.nguoidung.filter(x => x.listUsers.length > 0);
                        blockUI.stop();
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }
                loadNhom();
                function loadNhom() {
                    blockUI.start();
                    var resp = funGroupUITree.GetAllUsersGroupsTree(1);
                    resp.then(function (response) {
                        $ctrl.nhomchucnang = response.data;
                        $ctrl.nhomchucnang.forEach(function (value, key) {
                            myStyles.addRule('tr[name="C' + value.Groups.GroupId + '"]', '{display: none}');
                            if (temp) {
                                angular.forEach(temp, function (j) {
                                    var i = value.listUsers.findIndex(x => x.UserName == j.UserName);
                                    if (i > -1) {
                                        value.listUsers.splice(i, 1);
                                    }
                                });
                            }
                        });
                        $ctrl.nhomchucnang = $ctrl.nhomchucnang.filter(x => x.listUsers.length > 0);
                        blockUI.stop();
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }

                function LoadAll() {
                    var resp = loginservice.postdata("api/getUser/getDanhSachNhanVien", $.param({ valint1: 6 }));
                    resp.then(function (response) {
                        $ctrl.dsnhanvien = response.data;
                    }
                        , function errorCallback(response) {
                        });
                }
                $ctrl.CheckSingle = function (item, checked) {
                    if (checked) {
                        let a = Array.prototype.map.call(item, s => s.UserName);
                        $ctrl.checklist = $ctrl.checklist.filter(x => !a.includes(x.UserName));
                        item.forEach(function (value, key) {
                            $ctrl.checklist.push({ UserName: value.UserName, FullName: value.FullName });
                        });
                    }
                    else {
                        let a = Array.prototype.map.call(item, s => s.UserName);
                        $ctrl.checklist = $ctrl.checklist.filter(x => !a.includes(x.UserName));
                    }

                }
                //#endregion
                $ctrl.LuuNhom = function () {
                    $scope.modalreturn.push(idselect.id);
                    $scope.modalreturn.push($ctrl.checklist);
                    $uibModalInstance.close($scope.modalreturn);
                }
                //#region Giao dien
                $ctrl.myClick = function (i) {
                    var hideThis = document.getElementById(i);
                    var showHide = angular.element(hideThis).attr('class');
                    if (showHide === 'ui-icon ace-icon fa fa-minus center bigger-110 blue') {
                        myStyles.addRule('tr[name="' + i + '"]', '{display: none}');
                        angular.element(hideThis).attr('class', 'ui-icon ace-icon fa fa-plus center bigger-110 blue');
                    }
                    else {
                        myStyles.deleteRule('tr[name="' + i + '"]');
                        angular.element(hideThis).attr('class', 'ui-icon ace-icon fa fa-minus center bigger-110 blue');
                    }
                };
                var myStyles = (function () {
                    var sheet = document.styleSheets[0];
                    function deleteRule(selector) {
                        var rules = sheet.rules || sheet.cssRules;
                        for (var i = 0; i < rules.length; i++) {
                            if (selector == rules[i].selectorText) {
                                sheet.deleteRule(i);
                            }
                        }
                    }

                    function addRule(selector, text) {
                        deleteRule(selector);
                        sheet.insertRule(selector + text);
                    }
                    return {
                        'addRule': addRule,
                        'deleteRule': deleteRule
                    };
                }());
                //#endregion
                //#region Modal
                $ctrl.ok = function () {
                    $ctrl.presult = "0";
                };
                $ctrl.cancel = function () {
                    $uibModalInstance.dismiss('cancel');
                };
                $ctrl.close = function (item) {
                    $uibModalInstance.close(item);
                };
                //#endregion
            }])
        .controller('modalCtrl', ['funGroupUITree', "$scope", 'blockUI', '$filter', "$http", "$uibModalInstance", "FileUploader", "appSettings", "loginservice", "userProfile", "idselect", 'uibDateParser',
            function (funGroupUITree,$scope, blockUI, $filter, $http, $uibModalInstance, FileUploader, appSettings, loginservice, userProfile, idselect, uibDateParser) {
                var $ctrl = this;
                if (idselect.listus)
                    $ctrl.checklist = idselect.listus.split(', ');
                else
                    $ctrl.checklist = [];

                $scope.modalreturn = [];

                $ctrl.CheckSingle = function (item, checked) {
                    if (checked) {
                        let a = Array.prototype.map.call(item, s => s.UserName);
                        $ctrl.checklist = $ctrl.checklist.filter(x => !a.includes(x));
                        item.forEach(function (value, key) {
                            $ctrl.checklist.push(value.UserName);
                        });
                    }
                    else {
                        let a = Array.prototype.map.call(item, s => s.UserName);
                        $ctrl.checklist = $ctrl.checklist.filter(x => !a.includes(x));
                    }

                }

                //#region Load
                $ctrl.phongban = [];
                loadUserSortGroup();
                function loadUserSortGroup() {
                    blockUI.start();
                    var resp = funGroupUITree.GetAllUsersGroupsTree(0);
                    resp.then(function (response) {
                        $ctrl.phongban = response.data;
                        $ctrl.phongban.forEach(function (value, key) {
                            myStyles.addRule('tr[name="' + value.Groups.GroupId + '"]', '{display: none}');
                        });
                        blockUI.stop();
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }
                $ctrl.canhan = [];
                loadDanhba();
                function loadDanhba() {
                    blockUI.start();
                    var resp = loginservice.postdata("api/danhba/getViewdanhba");
                    resp.then(function (response) {
                        $ctrl.canhan = response.data;
                        $ctrl.canhan.forEach(function (value, key) {
                            myStyles.addRule('tr[name="' + value.Groups.GroupId + '"]', '{display: none}');
                        });
                        blockUI.stop();
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }
                //#endregion
                $ctrl.LuuNhom = function () {
                    $scope.modalreturn.push(idselect.id);
                    $scope.modalreturn.push($ctrl.checklist);
                    $uibModalInstance.close($scope.modalreturn);
                }
                //#region Giao dien
                $ctrl.myClick = function (i) {
                    var hideThis = document.getElementById(i);
                    var showHide = angular.element(hideThis).attr('class');
                    if (showHide === 'ui-icon ace-icon fa fa-minus center bigger-110 blue') {
                        myStyles.addRule('tr[name="' + i + '"]', '{display: none}');
                        angular.element(hideThis).attr('class', 'ui-icon ace-icon fa fa-plus center bigger-110 blue');
                    }
                    else {
                        myStyles.deleteRule('tr[name="' + i + '"]');
                        angular.element(hideThis).attr('class', 'ui-icon ace-icon fa fa-minus center bigger-110 blue');
                    }
                };
                var myStyles = (function () {
                    var sheet = document.styleSheets[0];
                    function deleteRule(selector) {
                        var rules = sheet.rules || sheet.cssRules;
                        for (var i = 0; i < rules.length; i++) {
                            if (selector == rules[i].selectorText) {
                                sheet.deleteRule(i);
                            }
                        }
                    }

                    function addRule(selector, text) {
                        deleteRule(selector);
                        sheet.insertRule(selector + text);
                    }
                    return {
                        'addRule': addRule,
                        'deleteRule': deleteRule
                    };
                }());
                //#endregion
                //#region Modal
                $ctrl.ok = function () {
                    $ctrl.presult = "0";
                };
                $ctrl.cancel = function () {
                    $uibModalInstance.dismiss('cancel');
                };
                $ctrl.close = function (item) {
                    $uibModalInstance.close(item);
                };
                //#endregion
            }])
        .directive('groupPer', function ($compile) {
            return {
                restrict: 'E',
                scope: {
                    localtemp: '=val',
                    localNodes: '=model',
                    localClick: '&click'
                },
                link: function (scope, tElement, tAttrs, transclude) {
                    var maxLevels = (angular.isUndefined(tAttrs.maxlevels)) ? 10 : tAttrs.maxlevels;
                    var hasCheckBox = (angular.isUndefined(tAttrs.checkbox)) ? false : true;
                    scope.showItems = [];

                    scope.showHide = function (ulId) {
                        var hideThis = document.getElementById(ulId);
                        var showHide = angular.element(hideThis).attr('class');
                        angular.element(hideThis).attr('class', (showHide === 'show' ? 'hide' : 'show'));
                    }

                    scope.showIcon = function (node) {
                        if (!angular.isUndefined(node.children)) return true;
                    }

                    scope.checkIfChildren = function (node) {
                        if (!angular.isUndefined(node.groupPers)) return true;
                    }

                    scope.CheckAll = function (item) {
                        item.forEach(function (e, i) {
                            if (scope.localtemp.findIndex(x => x.PermissionId == e.PermissionId) > -1)
                                return;
                            scope.localtemp.push(e);
                        });
                    }
                    scope.UnCheckAll = function (item) {
                        item.forEach(function (e, i) {
                            let temp = scope.localtemp.findIndex(x => x.PermissionId == e.PermissionId);
                            scope.localtemp.splice(temp, 1);
                        });
                    }
                    /////////////////////////////////////////////////

                    function renderTreeView(collection, level, max) {
                        var text = '';
                        text += '<li ng-repeat="n in ' + collection + '" >';
                        text += '<span class="show-hide" ng-click=showHide(n.module.ModuleKey)><i class="fa fa-plus-square"></i></span>';
                        text += ' ';
                        text += '<span ng-bind="n.module.ModuleName"></span>';
                        if (hasCheckBox) {
                            text += '<span class="tree-checkbox edit" ng-click=CheckAll(n.groupPers)><i class="fa fa-check-square"></i></span>'
                            text += '<span class="tree-checkbox edit" ng-click=UnCheckAll(n.groupPers)><i class="fa fa-times-circle"></i></span>'
                        }
                        if (level < max) {
                            text += '<ul id="{{n.module.ModuleKey}}" class="hide" ng-if=checkIfChildren(n)>';
                            text += '<li ng-repeat="item in n.groupPers" >';
                            text += '<input class="tree-checkbox" type=checkbox checklist-model="localtemp" checklist-value="item">';
                            text += '<span ng-bind="item.ResourceName + ' + "' - '" + ' +item.PermissionName"></span>';
                            text += '</li></ul></li>';
                        } else {
                            text += '</li>';
                        }

                        return text;
                    }// end renderTreeView();

                    try {
                        var text = '<ul class="tree-view-wrapper">';
                        text += renderTreeView('localNodes', 1, maxLevels);
                        text += '</ul>';
                        tElement.html(text);
                        $compile(tElement.contents())(scope);
                    }
                    catch (err) {
                        tElement.html('<b>ERROR!!!</b> - ' + err);
                        $compile(tElement.contents())(scope);
                    }
                }
            };
        })
        .directive('checklistModel', ['$parse', '$compile', function ($parse, $compile) {
            function contains(arr, item, comparator) {
                if (angular.isArray(arr)) {
                    for (var i = arr.length; i--;) {
                        if (comparator(arr[i], item)) {
                            return true;
                        }
                    }
                }
                return false;
            }
            function add(arr, item, comparator) {
                arr = angular.isArray(arr) ? arr : [];
                if (!contains(arr, item, comparator)) {
                    arr.push(item);
                }
                return arr;
            }
            function remove(arr, item, comparator) {
                if (angular.isArray(arr)) {
                    for (var i = arr.length; i--;) {
                        if (comparator(arr[i], item)) {
                            arr.splice(i, 1);
                            break;
                        }
                    }
                }
                return arr;
            }

            function postLinkFn(scope, elem, attrs) {
                var checklistModel = attrs.checklistModel;
                attrs.$set("checklistModel", null);
                $compile(elem)(scope);
                attrs.$set("checklistModel", checklistModel);
                var checklistModelGetter = $parse(checklistModel);
                var checklistChange = $parse(attrs.checklistChange);
                var checklistBeforeChange = $parse(attrs.checklistBeforeChange);
                var ngModelGetter = $parse(attrs.ngModel);
                var comparator = function (a, b) {
                    if (!isNaN(a) && !isNaN(b)) {
                        return String(a) === String(b);
                    } else {
                        return angular.equals(a, b);
                    }
                };

                if (attrs.hasOwnProperty('checklistComparator')) {
                    if (attrs.checklistComparator[0] == '.') {
                        var comparatorExpression = attrs.checklistComparator.substring(1);
                        comparator = function (a, b) {
                            return a[comparatorExpression] === b[comparatorExpression];
                        };
                    } else {
                        comparator = $parse(attrs.checklistComparator)(scope.$parent);
                    }
                }

                var unbindModel = scope.$watch(attrs.ngModel, function (newValue, oldValue) {
                    if (newValue === oldValue) {
                        return;
                    }

                    if (checklistBeforeChange && (checklistBeforeChange(scope) === false)) {
                        ngModelGetter.assign(scope, contains(checklistModelGetter(scope.$parent), getChecklistValue(), comparator));
                        return;
                    }

                    setValueInChecklistModel(getChecklistValue(), newValue);

                    if (checklistChange) {
                        checklistChange(scope);
                    }
                });

                var unbindCheckListValue = scope.$watch(getChecklistValue, function (newValue, oldValue) {
                    if (newValue != oldValue && angular.isDefined(oldValue) && scope[attrs.ngModel] === true) {
                        var current = checklistModelGetter(scope.$parent);
                        checklistModelGetter.assign(scope.$parent, remove(current, oldValue, comparator));
                        checklistModelGetter.assign(scope.$parent, add(current, newValue, comparator));
                    }
                }, true);

                var unbindDestroy = scope.$on('$destroy', destroy);

                function destroy() {
                    unbindModel();
                    unbindCheckListValue();
                    unbindDestroy();
                }

                function getChecklistValue() {
                    return attrs.checklistValue ? $parse(attrs.checklistValue)(scope.$parent) : attrs.value;
                }

                function setValueInChecklistModel(value, checked) {
                    var current = checklistModelGetter(scope.$parent);
                    if (angular.isFunction(checklistModelGetter.assign)) {
                        if (checked === true) {
                            checklistModelGetter.assign(scope.$parent, add(current, value, comparator));
                        } else {
                            checklistModelGetter.assign(scope.$parent, remove(current, value, comparator));
                        }
                    }
                }

                function setChecked(newArr, oldArr) {
                    if (checklistBeforeChange && (checklistBeforeChange(scope) === false)) {
                        setValueInChecklistModel(getChecklistValue(), ngModelGetter(scope));
                        return;
                    }
                    ngModelGetter.assign(scope, contains(newArr, getChecklistValue(), comparator));
                }
                if (angular.isFunction(scope.$parent.$watchCollection)) {
                    scope.$parent.$watchCollection(checklistModel, setChecked);
                } else {
                    scope.$parent.$watch(checklistModel, setChecked, true);
                }
            }

            return {
                restrict: 'A',
                priority: 1000,
                terminal: true,
                scope: true,
                compile: function (tElement, tAttrs) {
                    if (!tAttrs.checklistValue && !tAttrs.value) {
                        throw 'You should provide `value` or `checklist-value`.';
                    }
                    if (!tAttrs.ngModel) {
                        tAttrs.$set("ngModel", "checked");
                    }
                    return postLinkFn;
                }
            };
        }])
        .controller('modalInstanceCtrl', ['$scope', '$uibModalInstance',
            function ($scope, $uibModalInstance) {
                $scope.ok = function () {
                    $uibModalInstance.close();
                };

                $scope.cancel = function () {
                    $uibModalInstance.dismiss('cancel');
                };
            }])
        .controller('modalConfirmInstanceCtrl', ['$scope', '$uibModalInstance', 'confirmMessage', "ModalService",
            function ($scope, $uibModalInstance, confirmMessage, ModalService) {
                $scope.confirmMessage = confirmMessage || 'đồng ý ?';

                var modalCount = ModalService.getModalCount();

                console.log('Số modal đang mở:', modalCount);

                $scope.ok = function () {
                    $uibModalInstance.close();
                };

                $scope.cancel = function () {
                    $uibModalInstance.dismiss('cancel');
                };
            }])
        .controller('UpdateModulePermissionCtrl', ["$timeout", 'thongbao', "$scope", 'blockUI', '$filter', "$http", "$uibModal", "$uibModalInstance", "FileUploader", "appSettings", "loginservice", "userProfile", "idselect", 'uibDateParser',
            function ($timeout, thongbao, $scope, blockUI, $filter, $http, $uibModal, $uibModalInstance, FileUploader, appSettings, loginservice, userProfile, idselect, uibDateParser) {
                var $ctrl = this;
                $ctrl.Steven = idselect;
                $scope.myClick = function () {
                };
                getdataInnit();
                function getdataInnit() {
                    getAll();
                }
                $scope.CheckAll = function (item) {
                    $ctrl.Steven.listgroupspers = $ctrl.Steven.listgroupspers.filter(function (currentChar) {
                        return currentChar.GroupId !== item;
                    });
                    $ctrl.Steven.listpers.forEach(function (value, key) {
                        $ctrl.Steven.listgroupspers.push({ GroupId: item, PermissionId: value.PermissionId })
                    });
                }
                $scope.UnCheckAll = function (item) {
                    $ctrl.Steven.listgroupspers = $ctrl.Steven.listgroupspers.filter(function (currentChar) {
                        return currentChar.GroupId !== item;
                    });
                }
                function getAll() {
                    var resp = loginservice.postdata("api/getCore/getAllGroups");
                    resp.then(function (response) {
                        $ctrl.Steven.listgroups = response.data;
                    },
                        function errorCallback(response) {
                        });
                }

                $ctrl.sumitformedit = function () {
                    blockUI.start();
                    var resp = loginservice.postdata("api/getCore/UpdateGroupsPermissions", $.param({
                        valstring1: 'Doc',
                        valstring2: 'G',
                        valstring3: $ctrl.Steven.ResourceId,
                        listgroupspers: $ctrl.Steven.listgroupspers,
                    }));
                    resp.then(function (response) {
                        blockUI.stop();
                        $uibModalInstance.close(1);
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                            thongbao.error("Thông báo", "Thêm sự kiện không thành công");
                        });
                }

                $ctrl.ok = function () {
                    $ctrl.presult = "0";
                };
                $ctrl.cancel = function () {
                    $uibModalInstance.dismiss('cancel');
                };
            }])
        .controller('UpdateModuleUserPermissionCtrl', ["$timeout", 'thongbao', "$scope", 'blockUI', '$filter', "$http", "$uibModal", "$uibModalInstance", "FileUploader", "appSettings", "loginservice", "userProfile", "idselect", 'uibDateParser',
            function ($timeout, thongbao, $scope, blockUI, $filter, $http, $uibModal, $uibModalInstance, FileUploader, appSettings, loginservice, userProfile, idselect, uibDateParser) {
                var $ctrl = this;
                $ctrl.Steven = idselect;
                console.log($ctrl.Steven);
                $scope.CheckAll = function (item) {
                    $ctrl.Steven.listuserspers = $ctrl.Steven.listuserspers.filter(function (currentChar) {
                        return currentChar.UserId !== item;
                    });
                    $ctrl.Steven.listpers.forEach(function (value, key) {
                        $ctrl.Steven.listuserspers.push({ UserId: item, PermissionId: value.PermissionId })
                    });
                }
                $scope.UnCheckAll = function (item) {
                    $ctrl.Steven.listuserspers = $ctrl.Steven.listuserspers.filter(function (currentChar) {
                        return currentChar.UserId !== item;
                    });
                }

                $scope.myClick = function (i) {
                    var hideThis = document.getElementById(i);
                    var showHide = angular.element(hideThis).attr('class');
                    if (showHide === 'ui-icon ace-icon fa fa-minus center bigger-110 blue') {
                        myStyles.addRule('tr[name="' + i + '"]', '{display: none}');
                        angular.element(hideThis).attr('class', 'ui-icon ace-icon fa fa-plus center bigger-110 blue');
                    }
                    else {
                        myStyles.deleteRule('tr[name="' + i + '"]');
                        angular.element(hideThis).attr('class', 'ui-icon ace-icon fa fa-minus center bigger-110 blue');
                    }
                };
                getdataInnit();
                function getdataInnit() {
                    getAll();
                }

                function getAll() {
                    var resp = loginservice.postdata("api/getCore/getAllUsersSortPhongBan");
                    resp.then(function (response) {
                        $ctrl.Steven.listusers = response.data;
                        $ctrl.Steven.listusers.forEach(function (value, key) {
                            myStyles.addRule('tr[name="' + value.group.GroupId + '"]', '{display: none}');
                        });
                    },
                        function errorCallback(response) {
                        });
                }
                var myStyles = (function () {
                    var sheet = document.styleSheets[0];
                    function deleteRule(selector) {
                        var rules = sheet.rules || sheet.cssRules;
                        for (var i = 0; i < rules.length; i++) {
                            if (selector == rules[i].selectorText) {
                                sheet.deleteRule(i);
                            }
                        }
                    }

                    function addRule(selector, text) {
                        deleteRule(selector);
                        sheet.insertRule(selector + text);
                    }
                    return {
                        'addRule': addRule,
                        'deleteRule': deleteRule
                    };
                }());
                $ctrl.sumitformedit = function () {
                    blockUI.start();
                    var resp = loginservice.postdata("api/getCore/UpdateUsersPermissions", $.param({
                        valstring1: $ctrl.Steven.ModuleKey,
                        valstring2: $ctrl.Steven.ResourceType,
                        valstring3: $ctrl.Steven.ResourceId,
                        listuserspers: $ctrl.Steven.listuserspers,
                    }));
                    resp.then(function (response) {
                        blockUI.stop();
                        $uibModalInstance.close(1);
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                            thongbao.error("Thông báo", "Thêm không thành công");
                        });
                }

                $ctrl.ok = function () {
                    $ctrl.presult = "0";
                };
                $ctrl.cancel = function () {
                    $uibModalInstance.dismiss('cancel');
                };
            }])
        .directive('treeView', function ($compile) {
            return {
                restrict: 'E',

                scope: {
                    localtemp: '=valtemp',
                    localNodes: '=model',
                    localClick: '&click'
                },
                link: function (scope, tElement, tAttrs, transclude) {
                    var maxLevels = (angular.isUndefined(tAttrs.maxlevels)) ? 10 : tAttrs.maxlevels;
                    var hasCheckBox = (angular.isUndefined(tAttrs.checkbox)) ? false : true;
                    scope.showItems = [];
                    scope.showHide = function (ulId) {
                        var hideThis = document.getElementById(ulId);
                        var showHide = angular.element(hideThis).attr('class');
                        angular.element(hideThis).attr('class', (showHide === 'show' ? 'hide' : 'show'));
                    }

                    scope.showIcon = function (node) {
                        if (!angular.isUndefined(node.Users)) return true;
                    }

                    scope.checkIfChildren = function (node) {
                        if (!angular.isUndefined(node.Users)) return true;
                    }

                    function parentCheckChange(item) {
                        for (var i in item.Users) {
                            item.Users[i].checked = item.checked;
                            var idx = scope.localtemp.indexOf(item.Users[i]);
                            if (idx > -1) {
                                if (!item.checked) {
                                    scope.localtemp.splice(idx, 1);
                                }
                            }
                            else {
                                if (item.checked) {
                                    scope.localtemp.push(item.Users[i]);
                                }
                            }
                        }
                    }
                    //Them Sau
                    function childCheckChange(item) {
                        var allChecks = true;
                        for (var i in item.User) {
                            if (!item.User[i].isChecked) {
                                allChecks = false;
                                break;
                            }
                        }
                        if (allChecks) {
                            item.isChecked = true;
                        }
                        else {
                            item.isChecked = false;
                        }
                        if (item.UserName) {
                            var idx = scope.localtemp.indexOf(item);
                            if (idx > -1) {
                                if (!item.checked) {
                                    scope.localtemp.splice(idx, 1);
                                }
                            }
                            else {
                                if (item.checked) {
                                    scope.localtemp.push(item);
                                }
                            }
                        }
                    }
                    //
                    scope.checkChange = function (node) {
                        if (node.Users) {
                            parentCheckChange(node);
                        }
                        childCheckChange(node);
                    }
                    /////////////////////////////////////////////////

                    //function renderTreeView(collection, level, max) {
                    //    var text = '';
                    //    text += '<li ng-repeat="n in ' + collection + '" >';
                    //    text += '<span ng-show=showIcon(n) class="show-hide" ng-click=showHide(n.MAPHONG)><i class="fa fa-plus-square"></i></span>';
                    //    text += '<span ng-show=!showIcon(n) style="padding-right: 13px"></span>';

                    //    if (hasCheckBox) {
                    //        text += '<input ng-show=showIcon(n) class="tree-checkbox" ng-click=localClick({node:n.UserName})  type=checkbox ng-model=n.checked ng-change=checkChange(n)>';
                    //    }

                    //    text += '<span class="edit"><i class="fa fa-users"></i></span>'

                    //    text += '<label>{{checkIfChildren(n) ? n.TENPHONG : n.FullName}}</label>';
                    //    if (level < max) {
                    //        text += '<ul id="{{n.MAPHONG}}" class="hide" ng-if=checkIfChildren(n)>' + renderTreeView('n.Users', level + 1, max) + '</ul></li>';
                    //    } else {
                    //        text += '</li>';
                    //    }

                    //    return text;
                    //}// end renderTreeView();
                    function renderTreeView(collection, level, max) {
                        var text = '';
                        text += '<li ng-repeat="n in ' + collection + '" >';
                        text += '<span ng-show=showIcon(n) class="show-hide" ng-click=showHide(1+n.MAPHONG)><i class="fa fa-plus-square"></i></span>';
                        //text += '<span ng-show=!showIcon(n) style="padding-right: 13px"></span>';

                        if (hasCheckBox) {
                            text += '<input class="tree-checkbox" ng-click=localClick({node:n.UserName})  type=checkbox ng-model=n.checked ng-change=checkChange(n)>';
                            //text += '<input class="tree-checkbox"  type=checkbox ng-model=n.checked ng-click="toggleSelection(n.UserName)" ng-change=checkChange(n) ng-checked="selectedRefs.indexOf(n.UserName) > -1">';
                        }

                        //text += '<span class="edit" ng-click=localClick({node:n.UserName})><i class="fa fa-users"></i></span>'
                        text += '<span class="edit"><i class="fa fa-users"></i></span>'

                        //text += '<label>{{n.GroupName}}{{n.FullName}}</label>';
                        text += '<label>{{checkIfChildren(n) ? n.TENPHONG : n.FullName}}</label>';
                        if (level < max) {
                            text += '<ul id="{{1+n.MAPHONG}}" class="hide" ng-if=checkIfChildren(n)>' + renderTreeView('n.Users', level + 1, max) + '</ul></li>';
                        } else {
                            text += '</li>';
                        }

                        return text;
                    }// end renderTreeView();
                    try {
                        var text = '<ul class="tree-view-wrapper">';
                        text += renderTreeView('localNodes', 1, maxLevels);
                        text += '</ul>';
                        tElement.html(text);
                        $compile(tElement.contents())(scope);
                    }
                    catch (err) {
                        tElement.html('<b>ERROR!!!</b> - ' + err);
                        $compile(tElement.contents())(scope);
                    }
                }
            };
        })
        .directive('treeView2', function ($compile) {
            return {
                restrict: 'E',

                scope: {
                    localtemp: '=valtemp',
                    localNodes: '=model',
                    localClick: '&click'
                },
                link: function (scope, tElement, tAttrs, transclude) {
                    var maxLevels = (angular.isUndefined(tAttrs.maxlevels)) ? 10 : tAttrs.maxlevels;
                    var hasCheckBox = (angular.isUndefined(tAttrs.checkbox)) ? false : true;
                    scope.showItems = [];
                    scope.showHide = function (ulId) {
                        var hideThis = document.getElementById(ulId);
                        var showHide = angular.element(hideThis).attr('class');
                        angular.element(hideThis).attr('class', (showHide === 'show' ? 'hide' : 'show'));
                    }

                    scope.showIcon = function (node) {
                        if (!angular.isUndefined(node.Users)) return true;
                    }

                    scope.checkIfChildren = function (node) {
                        if (!angular.isUndefined(node.Users)) return true;
                    }

                    function parentCheckChange(item) {
                        for (var i in item.Users) {
                            item.Users[i].checked = item.checked;
                            var idx = scope.localtemp.indexOf(item.Users[i]);
                            if (idx > -1) {
                                if (!item.checked) {
                                    scope.localtemp.splice(idx, 1);
                                }
                            }
                            else {
                                if (item.checked) {
                                    scope.localtemp.push(item.Users[i]);
                                }
                            }
                        }
                    }
                    //Them Sau
                    function childCheckChange(item) {
                        var allChecks = true;
                        for (var i in item.User) {
                            if (!item.User[i].isChecked) {
                                allChecks = false;
                                break;
                            }
                        }
                        if (allChecks) {
                            item.isChecked = true;
                        }
                        else {
                            item.isChecked = false;
                        }
                        if (item.UserName) {
                            var idx = scope.localtemp.indexOf(item);
                            if (idx > -1) {
                                if (!item.checked) {
                                    scope.localtemp.splice(idx, 1);
                                }
                            }
                            else {
                                if (item.checked) {
                                    scope.localtemp.push(item);
                                }
                            }
                        }
                    }
                    //
                    scope.checkChange = function (node) {
                        if (node.Users) {
                            parentCheckChange(node);
                        }
                        childCheckChange(node);
                    }
                    /////////////////////////////////////////////////

                    function renderTreeView(collection, level, max) {
                        var text = '';
                        text += '<li ng-repeat="n in ' + collection + '" >';
                        text += '<span ng-show=showIcon(n) class="show-hide" ng-click=showHide(2+n.MAPHONG)><i class="fa fa-plus-square"></i></span>';
                        //text += '<span ng-show=!showIcon(n) style="padding-right: 13px"></span>';

                        if (hasCheckBox) {
                            text += '<input class="tree-checkbox" ng-click=localClick({node:n.UserName})  type=checkbox ng-model=n.checked ng-change=checkChange(n)>';
                            //text += '<input class="tree-checkbox"  type=checkbox ng-model=n.checked ng-click="toggleSelection(n.UserName)" ng-change=checkChange(n) ng-checked="selectedRefs.indexOf(n.UserName) > -1">';
                        }

                        //text += '<span class="edit" ng-click=localClick({node:n.UserName})><i class="fa fa-users"></i></span>'
                        text += '<span class="edit"><i class="fa fa-users"></i></span>'

                        //text += '<label>{{n.GroupName}}{{n.FullName}}</label>';
                        text += '<label>{{checkIfChildren(n) ? n.TENPHONG : n.FullName}}</label>';
                        if (level < max) {
                            text += '<ul id="{{2+n.MAPHONG}}" class="hide" ng-if=checkIfChildren(n)>' + renderTreeView('n.Users', level + 1, max) + '</ul></li>';
                        } else {
                            text += '</li>';
                        }

                        return text;
                    }// end renderTreeView();

                    try {
                        var text = '<ul class="tree-view-wrapper">';
                        text += renderTreeView('localNodes', 1, maxLevels);
                        text += '</ul>';
                        tElement.html(text);
                        $compile(tElement.contents())(scope);
                    }
                    catch (err) {
                        tElement.html('<b>ERROR!!!</b> - ' + err);
                        $compile(tElement.contents())(scope);
                    }
                }
            };
        })
        .directive('treeView3', function ($compile) {
            return {
                restrict: 'E',

                scope: {
                    localtemp: '=valtemp',
                    localNodes: '=model',
                    localClick: '&click'
                },
                link: function (scope, tElement, tAttrs, transclude) {
                    var maxLevels = (angular.isUndefined(tAttrs.maxlevels)) ? 10 : tAttrs.maxlevels;
                    var hasCheckBox = (angular.isUndefined(tAttrs.checkbox)) ? false : true;
                    scope.showItems = [];
                    scope.showHide = function (ulId) {
                        var hideThis = document.getElementById(ulId);
                        var showHide = angular.element(hideThis).attr('class');
                        angular.element(hideThis).attr('class', (showHide === 'show' ? 'hide' : 'show'));
                    }

                    scope.showIcon = function (node) {
                        if (!angular.isUndefined(node.Users)) return true;
                    }

                    scope.checkIfChildren = function (node) {
                        if (!angular.isUndefined(node.Users)) return true;
                    }

                    function parentCheckChange(item) {
                        for (var i in item.Users) {
                            item.Users[i].checked = item.checked;
                            var idx = scope.localtemp.indexOf(item.Users[i]);
                            if (idx > -1) {
                                if (!item.checked) {
                                    scope.localtemp.splice(idx, 1);
                                }
                            }
                            else {
                                if (item.checked) {
                                    scope.localtemp.push(item.Users[i]);
                                }
                            }
                        }
                    }
                    //Them Sau
                    function childCheckChange(item) {
                        var allChecks = true;
                        for (var i in item.User) {
                            if (!item.User[i].isChecked) {
                                allChecks = false;
                                break;
                            }
                        }
                        if (allChecks) {
                            item.isChecked = true;
                        }
                        else {
                            item.isChecked = false;
                        }
                        if (item.UserName) {
                            var idx = scope.localtemp.indexOf(item);
                            if (idx > -1) {
                                if (!item.checked) {
                                    scope.localtemp.splice(idx, 1);
                                }
                            }
                            else {
                                if (item.checked) {
                                    scope.localtemp.push(item);
                                }
                            }
                        }
                    }
                    //
                    scope.checkChange = function (node) {
                        if (node.Users) {
                            parentCheckChange(node);
                        }
                        childCheckChange(node);
                    }
                    /////////////////////////////////////////////////

                    function renderTreeView(collection, level, max) {
                        var text = '';
                        text += '<li ng-repeat="n in ' + collection + '" >';
                        text += '<span ng-show=showIcon(n) class="show-hide" ng-click=showHide(3+n.MAPHONG)><i class="fa fa-plus-square"></i></span>';
                        //text += '<span ng-show=!showIcon(n) style="padding-right: 13px"></span>';

                        if (hasCheckBox) {
                            text += '<input class="tree-checkbox" ng-click=localClick({node:n.UserName})  type=checkbox ng-model=n.checked ng-change=checkChange(n)>';
                            //text += '<input class="tree-checkbox"  type=checkbox ng-model=n.checked ng-click="toggleSelection(n.UserName)" ng-change=checkChange(n) ng-checked="selectedRefs.indexOf(n.UserName) > -1">';
                        }

                        //text += '<span class="edit" ng-click=localClick({node:n.UserName})><i class="fa fa-users"></i></span>'
                        text += '<span class="edit"><i class="fa fa-users"></i></span>';

                        //text += '<label>{{n.GroupName}}{{n.FullName}}</label>';
                        text += '<label>{{checkIfChildren(n) ? n.TENPHONG : n.FullName}}</label>';
                        if (level < max) {
                            text += '<ul id="{{3+n.MAPHONG}}" class="hide" ng-if=checkIfChildren(n)>' + renderTreeView('n.Users', level + 1, max) + '</ul></li>';
                        } else {
                            text += '</li>';
                        }

                        return text;
                    }// end renderTreeView();

                    try {
                        var text = '<ul class="tree-view-wrapper">';
                        text += renderTreeView('localNodes', 1, maxLevels);
                        text += '</ul>';
                        tElement.html(text);
                        $compile(tElement.contents())(scope);
                    }
                    catch (err) {
                        tElement.html('<b>ERROR!!!</b> - ' + err);
                        $compile(tElement.contents())(scope);
                    }
                }
            };
        })
        .directive('treeView4', function ($compile) {
            return {
                restrict: 'E',

                scope: {
                    localtemp: '=valtemp',
                    localNodes: '=model',
                    localClick: '&click'
                },
                link: function (scope, tElement, tAttrs, transclude) {
                    var maxLevels = (angular.isUndefined(tAttrs.maxlevels)) ? 10 : tAttrs.maxlevels;
                    var hasCheckBox = (angular.isUndefined(tAttrs.checkbox)) ? false : true;
                    scope.showItems = [];
                    scope.showHide = function (ulId) {
                        var hideThis = document.getElementById(ulId);
                        var showHide = angular.element(hideThis).attr('class');
                        angular.element(hideThis).attr('class', (showHide === 'show' ? 'hide' : 'show'));
                    }

                    scope.showIcon = function (node) {
                        if (!angular.isUndefined(node.Users)) return true;
                    }

                    scope.checkIfChildren = function (node) {
                        if (!angular.isUndefined(node.Users)) return true;
                    }

                    function parentCheckChange(item) {
                        for (var i in item.Users) {
                            item.Users[i].checked = item.checked;
                            var idx = scope.localtemp.indexOf(item.Users[i]);
                            if (idx > -1) {
                                if (!item.checked) {
                                    scope.localtemp.splice(idx, 1);
                                }
                            }
                            else {
                                if (item.checked) {
                                    scope.localtemp.push(item.Users[i]);
                                }
                            }
                        }
                    }
                    //Them Sau
                    function childCheckChange(item) {
                        var allChecks = true;
                        for (var i in item.User) {
                            if (!item.User[i].isChecked) {
                                allChecks = false;
                                break;
                            }
                        }
                        if (allChecks) {
                            item.isChecked = true;
                        }
                        else {
                            item.isChecked = false;
                        }
                        if (item.UserName) {
                            var idx = scope.localtemp.indexOf(item);
                            if (idx > -1) {
                                if (!item.checked) {
                                    scope.localtemp.splice(idx, 1);
                                }
                            }
                            else {
                                if (item.checked) {
                                    scope.localtemp.push(item);
                                }
                            }
                        }
                    }
                    //
                    scope.checkChange = function (node) {
                        if (node.Users) {
                            parentCheckChange(node);
                        }
                        childCheckChange(node);
                    }
                    /////////////////////////////////////////////////

                    function renderTreeView(collection, level, max) {
                        var text = '';
                        text += '<li ng-repeat="n in ' + collection + '" >';
                        //text += '<span ng-show=showIcon(n) class="show-hide" ng-click=showHide(3+n.MAPHONG)><i class="fa fa-plus-square"></i></span>';

                        if (hasCheckBox) {
                            text += '<input class="tree-checkbox" ng-click=localClick({node:n.UserName}) type=checkbox ng-model=n.checked ng-change=checkChange(n)>';
                        }

                        text += '<span class="edit"><i class="fa fa-users"></i></span>';
                        text += '<label>{{checkIfChildren(n) ? n.TENPHONG : n.FullName}} ';
                        text += '<span ng-if="n.checked">';
                        text += '<span data-toggle="buttons" class="btn-group btn-overlap btn-corner">';
                        text += '<label class="btn btn-sm btn-white btn-info active"><input type="checkbox" value="1">Lý Thành Tài, Phạm Chí Thiện, Dương Văn Hòa nhận văn bản</label>';
                        text += '<label class="btn btn-sm btn-white btn-info"><input type="checkbox" value="3">Tất cả nhận văn bản</label>';
                        text += '</span></span>';
                        if (level < max) {
                            text += '<ul id="{{3+n.MAPHONG}}" class="hide" ng-if=checkIfChildren(n)>' + renderTreeView('n.Users', level + 1, max) + '</ul></li>';
                        } else {
                            text += '</li>';
                        }

                        return text;
                    }// end renderTreeView();

                    try {
                        var text = '<ul class="tree-view-wrapper">';
                        text += renderTreeView('localNodes', 1, maxLevels);
                        text += '</ul>';
                        tElement.html(text);
                        $compile(tElement.contents())(scope);
                    }
                    catch (err) {
                        tElement.html('<b>ERROR!!!</b> - ' + err);
                        $compile(tElement.contents())(scope);
                    }
                }
            };
        })
        .controller('lichcanhanCtrl', ['$rootScope', '$document', '$window', 'thongbao', "$scope", 'blockUI', "$uibModal", '$filter', "$http", "$uibModalInstance", "FileUploader", "appSettings", "loginservice", "userProfile", "idselect", 'uibDateParser',
            function ($rootScope, $document, $window, thongbao, $scope, blockUI, $uibModal, $filter, $http, $uibModalInstance, FileUploader, appSettings, loginservice, userProfile, idselect, uibDateParser) {
                var $ctrl = this;

                $scope.redirectSte = function (item) {
                    $window.open(item, '_blank');
                };

                getChiTiet();
                function getChiTiet() {
                    var resp = loginservice.postdata("api/danhba/getUserEvent", $.param({ valint1: idselect }));
                    resp.then(function (response) {
                        $scope.selectitem = response.data;
                    },
                        function errorCallback(response) {
                            $scope.selectitem = [];
                        });
                }

                $scope.EditSukien = function () {
                    var parentElem = angular.element($document[0].querySelector('.content-wrapper'));
                    var modalInstance = $uibModal.open({
                        animation: true,
                        backdrop: 'static',
                        templateUrl: '_CreateEditPUGEvent.html',
                        controller: 'editPUGEventCtrl',
                        controllerAs: '$ctrl',
                        size: 'lg',
                        appendTo: parentElem,
                        resolve: {
                            idselect: function () {
                                $scope.selectitem.filter = -1;
                                return $scope.selectitem;
                            }
                        }
                    });
                    modalInstance.result.then(function (c) {
                        thongbao.success();
                        getChiTiet();
                    }, function () {
                        getChiTiet();
                    });
                }
                $scope.DeleteUserEvent = function () {
                    thongbao.sweetAlert('Bạn muốn xóa sự kiện này ?')
                        .then(function (result) {
                            if (result.value) {
                                var resp = loginservice.postdata("api/danhba/deleteUserEvent", $.param({ valint1: idselect }));
                                resp.then(function (response) {
                                    $uibModalInstance.close(1);
                                }, function errorCallback(response) {
                                    thongbao.error();
                                });
                            }
                            else if (result.dismiss === Swal.DismissReason.cancel) {
                                thongbao.sweetAlert2(0);
                            }
                        })
                }
                $scope.EndSukien = function (hoanthanh) {
                    thongbao.sweetAlert('Bạn muốn hoàn thành/tiếp tục sự kiện này ?')
                        .then(function (result) {
                            if (result.value) {
                                var resp = loginservice.postdata("api/danhba/EndUserEvent", $.param({ valint1: idselect, valint2: hoanthanh }));
                                resp.then(function (response) {
                                    $uibModalInstance.close(1);
                                }, function errorCallback(response) {
                                    thongbao.error();
                                });
                            }
                            else if (result.dismiss === Swal.DismissReason.cancel) {
                                thongbao.sweetAlert2(0);
                            }
                        })
                }
                $scope.EndThongbao = function () {
                    thongbao.sweetAlert('Bạn muốn kết thúc nhắc sự kiện này ?')
                        .then(function (result) {
                            if (result.value) {
                                var resp = loginservice.postdata("api/danhba/UpdateQueueSMS", $.param({ valint1: idselect, valint2: 2 }));
                                resp.then(function (response) {
                                    $uibModalInstance.close(1);
                                }, function errorCallback(response) {
                                    thongbao.error();
                                });
                            }
                            else if (result.dismiss === Swal.DismissReason.cancel) {
                                thongbao.sweetAlert2(0);
                            }
                        })
                }

                //#region Modal
                $ctrl.ok = function () {
                    $ctrl.presult = "0";
                };
                $ctrl.cancel = function () {
                    $uibModalInstance.dismiss('cancel');
                };
                $ctrl.close = function (item) {
                    $uibModalInstance.close(item);
                };
                //#endregion
            }]);
}());