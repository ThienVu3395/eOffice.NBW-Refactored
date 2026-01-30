angular
    .module("aims")
    .controller('chonNguoiKyVBCtrl', [
        "$uibModalInstance",
        "blockUI",
        "loginservice",
        "idselect",
        function (
            $uibModalInstance,
            blockUI,
            loginservice,
            idselect) {
            var $ctrl = this;

            $ctrl.Khoa = false;

            $ctrl.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $ctrl.ChonNguoiKy = function () {
                $ctrl.NguoiKy.LoaiKy = $ctrl.LoaiKy;
                $ctrl.NguoiKy.CHUCVU = $ctrl.NguoiKy.CHUCVU + ($ctrl.NguoiKy.CHUCVU2 == null ? '' : ('/' + $ctrl.NguoiKy.CHUCVU2));
                $uibModalInstance.close($ctrl.NguoiKy);
            }

            $ctrl.DoiNguoiKy = function () {
                let index = $ctrl.dsLoaiKy.findIndex(x => x.MaLoaiKy == 3);
                if (index != -1) {
                    if ($ctrl.NguoiKy.UserName == "mocvden") {
                        $ctrl.Khoa = true;
                        $ctrl.LoaiKy = 3;
                        $ctrl.dsLoaiKy[index].HienThi = true;
                    }
                    else {
                        $ctrl.Khoa = false;
                        $ctrl.LoaiKy = $ctrl.dsLoaiKy[0].MaLoaiKy;
                        $ctrl.dsLoaiKy[index].HienThi = false;
                    }
                }
            }

            Init();

            function Init() {
                $ctrl.dsNguoiKy = [];
                $ctrl.NguoiKy = {};
                $ctrl.dsLoaiKy = [{ MaLoaiKy: 0, TenLoaiKy: "Chỉ hiển thị logo", HienThi: true }, { MaLoaiKy: 1, TenLoaiKy: "Hiển thị nội dung và logo", HienThi: true }, { MaLoaiKy: 2, TenLoaiKy: "Chỉ hiển thị nội dung", HienThi: true }, { MaLoaiKy: 3, TenLoaiKy: "Kiểu hiển thị dành riêng cho mộc công văn", HienThi: false }];
                $ctrl.LoaiKy = $ctrl.dsLoaiKy[0].MaLoaiKy;
                GetNguoiKy();
            }

            function GetNguoiKy() {
                blockUI.start();
                var resp = loginservice.postdata("api/getUser/getDanhSachNhanVien", $.param({ valint1: 9 }));
                resp.then(function (response) {
                    blockUI.stop();
                    $ctrl.dsNguoiKy = response.data;
                    if (idselect.dsNguoiKy.length != 0) {
                        idselect.dsNguoiKy.forEach(function (item) {
                            let index = $ctrl.dsNguoiKy.findIndex(x => x.UserName == item.UserName);
                            if (index != -1) {
                                $ctrl.dsNguoiKy.splice(index, 1);
                            }
                        })
                    }
                    $ctrl.NguoiKy = response.data[0];
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }
        }])
    .controller('chonNguoiKyVBNghiPhepCtrl', [
        "$uibModalInstance",
        "blockUI",
        "loginservice",
        "idselect",
        function (
            $uibModalInstance,
            blockUI,
            loginservice,
            idselect) {
            var $ctrl = this;

            $ctrl.KyNhay = true;

            $ctrl.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $ctrl.ChonNguoiKy = function () {
                $ctrl.NguoiKy.LoaiKy = $ctrl.dsLoaiKy[$ctrl.KyNhay == false ? 1 : 0].MaLoaiKy;
                let chucvu1 = $ctrl.NguoiKy.CHUCVU ? $ctrl.NguoiKy.CHUCVU : '';
                let chucvu2 = $ctrl.NguoiKy.CHUCVU2 ? $ctrl.NguoiKy.CHUCVU2 : '';
                $ctrl.NguoiKy.CHUCVU = chucvu1 + (chucvu1 && chucvu2 ? '/' : '') + chucvu2;
                $uibModalInstance.close($ctrl.NguoiKy);
            }

            Init();

            function Init() {
                $ctrl.dsNguoiKy = [];
                $ctrl.NguoiKy = {};
                $ctrl.dsLoaiKy = [{ MaLoaiKy: 0, TenLoaiKy: "Chỉ hiển thị logo", HienThi: true }, { MaLoaiKy: 1, TenLoaiKy: "Hiển thị nội dung và logo", HienThi: true }];
                GetNguoiKy();
            }

            function GetNguoiKy() {
                blockUI.start();
                var resp = loginservice.postdata("api/getUser/getDanhSachNhanVien", $.param({ valint1: 10 }));
                resp.then(function (response) {
                    blockUI.stop();
                    $ctrl.dsNguoiKy = response.data;
                    if (idselect.dsNguoiKy.length != 0) {
                        idselect.dsNguoiKy.forEach(function (item) {
                            let index = $ctrl.dsNguoiKy.findIndex(x => x.UserName == item.UserName);
                            if (index != -1) {
                                $ctrl.dsNguoiKy.splice(index, 1);
                            }
                        })
                    }
                    if (idselect.maNhanVienNguoiTao != null) {
                        let index = $ctrl.dsNguoiKy.findIndex(x => parseInt(x.MSNV) == parseInt(idselect.maNhanVienNguoiTao));
                        if (index != -1) {
                            $ctrl.dsNguoiKy.splice(index, 1);
                        }
                    }
                    $ctrl.NguoiKy = response.data[0];
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }
        }
    ]);