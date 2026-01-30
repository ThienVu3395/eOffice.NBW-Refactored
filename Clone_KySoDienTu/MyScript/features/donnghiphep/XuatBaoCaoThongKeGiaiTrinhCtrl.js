angular
    .module("aims")
    .controller('xuatBaoCaoThongKeGiaiTrinhCtrl', [
        "$scope",
        "$uibModalInstance",
        "blockUI",
        "loginservice",
        "thongbao",
        "ModalService",
        "userProfile",
        "idselect",
        function (
            $scope,
            $uibModalInstance,
            blockUI,
            loginservice,
            thongbao,
            ModalService,
            userProfile,
            idselect) {
            var $ctrl = this;

            var Userdata = userProfile.getProfile();

            $scope.$on('closeModal', function (data, mess) {
                thongbao.success(mess);
                $ctrl.cancel();
            });

            $ctrl.khoaChon = false;

            $ctrl.vm = {};

            $ctrl.vm.Loai = {};

            $ctrl.para = {};

            $ctrl.para.idPhongBan = "All";

            $ctrl.parPhongBanNhanVien = {};

            $ctrl.parPhongBanNhanVien.FilterVanBan = {};

            $ctrl.parPhongBanNhanVien.FilterVanBan.CANBO = Userdata.manhanvien;

            $ctrl.parPhongBanNhanVien.FilterVanBan.PhongBan = {};

            $ctrl.parPhongBanNhanVien.FilterLenhDieuXe = {};

            $ctrl.parPhongBanNhanVien.FilterLenhDieuXe.Filters = [];

            $ctrl.parPhongBanNhanVien.FilterLenhDieuXe.Page = 1;

            var today = new Date();

            $ctrl.BeginDate = new Date();

            $ctrl.EndDate = new Date();

            $ctrl.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            GetNgayNghiLe();

            GetDanhSachPhongBan();

            GetDanhSachNhanVien();

            function GetNgayNghiLe() {
                var resp = loginservice.getdata("api/QLNghiPhep/GetNgayNghiLe");
                resp.then(function (response) {
                    $ctrl.NgayNghiLe = response.data;
                    KhoiTaoDatePicker();
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            function SpecialDateDisabled(data) {
                var date = data.date,
                    mode = data.mode;
                return mode === 'day' &&
                    (date.getDay() === 0 ||
                        date.getDay() === 6 ||
                        $ctrl.NgayNghiLe.map(m => m.ngay).some(s => {
                            var ngayLe = new Date(s);
                            return date.getFullYear() == ngayLe.getFullYear() &&
                                date.getDate() == ngayLe.getDate() &&
                                date.getMonth() == ngayLe.getMonth()
                        }));
            }

            function KhoiTaoDatePicker() {
                $ctrl.NgayBatDau = {
                    opened: false
                };

                $ctrl.NgayKetThuc = {
                    opened: false
                };

                $ctrl.dateOptionsStartDate = {
                    showWeeks: false,
                    //minDate: today,
                    dateDisabled: SpecialDateDisabled
                }

                $ctrl.dateOptionsEndDate = {
                    showWeeks: false,
                    minDate: today,
                    dateDisabled: SpecialDateDisabled
                }
            }

            function GetDateTimeFormat(dt) {
                let dtf = new Date(dt);
                //console.log(dtf.getFullYear() + "-" + (dtf.getMonth() + 1) + "-" + dtf.getDate());
                return dtf.getFullYear() + "-" + (dtf.getMonth() + 1) + "-" + dtf.getDate();
            }

            function GetDanhSachPhongBan() {
                blockUI.start();
                let resp = loginservice.getdata("api/QLNghiPhep/GetDanhSachPhongBan");
                resp.then(function (response) {
                    $ctrl.dsPhongBan = response.data;
                    $ctrl.dsPhongBan.unshift({ id: "All", name: "-- Tất cả (" + $ctrl.dsPhongBan.length + ") --" });
                    blockUI.stop();
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            function GetDanhSachNhanVien() {
                $ctrl.parPhongBanNhanVien.FilterLenhDieuXe.PageSize = 1000;
                if ($ctrl.para.idPhongBan != "All") {
                    let obj = {
                        propertyName: "IdPhongBan",
                        type: 0,
                        value: $ctrl.para.idPhongBan
                    }
                    $ctrl.parPhongBanNhanVien.FilterLenhDieuXe.Filters.push(obj);
                }
                var resp = loginservice.postdata("api/QLNghiPhep/GetDanhSachNhanVien", $.param($ctrl.parPhongBanNhanVien));
                resp.then(function (response) {
                    $ctrl.DanhSachNhanVien = response.data.data;
                    $ctrl.DanhSachNhanVien.unshift({ tenPhongBan: "-- Tất cả (" + $ctrl.DanhSachNhanVien.length + ") --", maNhanVien: "0", tenNhanVien: "Tất cả CB/CNV" });
                    if (idselect == null) {
                        $ctrl.khoaChon = false;
                        $ctrl.vm.Loai = $ctrl.DanhSachNhanVien[0];
                    }
                    else {
                        let index = $ctrl.DanhSachNhanVien.findIndex(x => parseInt(x.maNhanVien) == parseInt(idselect));
                        if (index != -1) {
                            $ctrl.khoaChon = true;
                            $ctrl.vm.Loai = $ctrl.DanhSachNhanVien[index];
                        }
                        else {
                            $ctrl.khoaChon = true;
                            $ctrl.vm.Loai = $ctrl.DanhSachNhanVien[0];
                        }
                    }
                    $ctrl.para.maNhanVien = $ctrl.vm.Loai.maNhanVien;
                    $ctrl.parPhongBanNhanVien.FilterLenhDieuXe.Filters = [];
                    blockUI.stop();
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            $ctrl.openNgayBatDau = function () {
                $ctrl.NgayBatDau.opened = true;
            };

            $ctrl.openNgayKetThuc = function () {
                $ctrl.NgayKetThuc.opened = true;
            };

            $ctrl.changedStartDate = function () {
                let startdate = new Date($ctrl.BeginDate);
                let enddate = new Date($ctrl.EndDate);
                if (startdate > enddate) {
                    $ctrl.EndDate = $ctrl.BeginDate;
                }
                $ctrl.dateOptionsEndDate.minDate = startdate;
            };

            $ctrl.changedEndDate = function () {
                let startdate = new Date($ctrl.BeginDate);
                let enddate = new Date($ctrl.EndDate);
                if (startdate > enddate) {
                    $ctrl.BeginDate = $ctrl.EndDate;
                }
            };

            $ctrl.DoiPhongBan = function () {
                GetDanhSachNhanVien();
            }

            $ctrl.DoiNhanVien = function () {
                $ctrl.para.maNhanVien = $ctrl.vm.Loai.maNhanVien;
            }

            $ctrl.XuatReport = function () {
                $ctrl.para.tuNgay = $ctrl.BeginDate == null ? GetDateTimeFormat(today) : GetDateTimeFormat($ctrl.BeginDate);
                $ctrl.para.denNgay = $ctrl.EndDate == null ? GetDateTimeFormat(today) : GetDateTimeFormat($ctrl.EndDate);
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Đồng ý xuất báo cáo với các lựa chọn trên ?';
                        }
                    }
                }).then(function () {
                    blockUI.start();
                    let resp = loginservice.postdata("api/QLNghiPhep/GetBaoCaoGiaiTrinh", $.param($ctrl.para));
                    resp.then(
                        function successCallback(response) {
                            var mediaType = "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,";
                            var a = document.createElement('a');
                            a.href = mediaType + response.data;
                            a.download = "BaoCaoNghiPhep_" + $ctrl.para.tuNgay + "_" + $ctrl.para.denNgay + ".xlsx";
                            a.textContent = 'Download file!';
                            document.body.appendChild(a);
                            a.click();
                            document.body.removeChild(a);
                            thongbao.success("Đã xuất file báo cáo");
                            blockUI.stop();
                        },
                        function errorCallback(response) {
                            thongbao.error("Xin vui lòng thực hiện sau");
                            blockUI.stop();
                        }
                    );
                }, function () {
                    blockUI.stop();
                });
            }
        }
    ]);