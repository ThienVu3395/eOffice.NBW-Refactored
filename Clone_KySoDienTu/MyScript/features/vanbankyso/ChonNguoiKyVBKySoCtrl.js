angular
    .module("aims")
    .controller('chonNguoiKyVBKySoCtrl', [
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
                let chucvu1 = $ctrl.NguoiKy.CHUCVU ? $ctrl.NguoiKy.CHUCVU : '';
                let chucvu2 = $ctrl.NguoiKy.CHUCVU2 ? $ctrl.NguoiKy.CHUCVU2 : '';
                $ctrl.NguoiKy.CHUCVU = chucvu1 + (chucvu1 && chucvu2 ? '/' : '') + chucvu2;
                $uibModalInstance.close($ctrl.NguoiKy);
            }

            $ctrl.DoiNguoiKy = function () {
                let index = $ctrl.dsLoaiKy.findIndex(x => x.MaLoaiKy == 0);
                if (index != -1) {
                    $ctrl.Khoa = false;
                    $ctrl.LoaiKy = $ctrl.dsLoaiKy[0].MaLoaiKy;
                    $ctrl.dsLoaiKy[index].HienThi = false;
                }
            }

            Init();

            function Init() {
                $ctrl.dsNguoiKy = [];
                $ctrl.NguoiKy = {};
                $ctrl.dsLoaiKy = [{ MaLoaiKy: 0, TenLoaiKy: "Chỉ hiển thị chữ ký", HienThi: true }, { MaLoaiKy: 1, TenLoaiKy: "Hiển thị nội dung và chữ ký", HienThi: true }, { MaLoaiKy: 2, TenLoaiKy: "Chỉ hiển thị nội dung", HienThi: true }];
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
                    $ctrl.NguoiKy = $ctrl.dsNguoiKy[0];
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }
        }
    ]);                                                           