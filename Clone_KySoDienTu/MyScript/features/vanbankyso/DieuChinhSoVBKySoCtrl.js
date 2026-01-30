angular
    .module("aims")
    .controller('dieuChinhSoVBCtrl', [
        "$scope",
        "$uibModalInstance",
        "blockUI",
        "loginservice",
        "idselect",
        "thongbao",
        "ModalService",
        function (
            $scope,
            $uibModalInstance,
            blockUI,
            loginservice,
            idselect,
            thongbao,
            ModalService) {
            var $ctrl = this;

            $scope.$on('closeModal', function (data, mess) {
                thongbao.success(mess);
                $ctrl.cancel();
            });

            $ctrl.thongTinVBHienTai = {};

            $ctrl.thongTinVBHienTai = idselect;

            $ctrl.para = {};

            $ctrl.para.ID = $ctrl.thongTinVBHienTai.ID;

            $ctrl.para.CodeNumber = $ctrl.thongTinVBHienTai.valstring1;

            $ctrl.para.FileNotation = $ctrl.thongTinVBHienTai.valstring2;

            $ctrl.DueDate = new Date();

            $ctrl.para.LyDoDieuChinh = "Điều chỉnh thông tin VB";

            $ctrl.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            KhoiTaoDatePicker();

            function KhoiTaoDatePicker() {
                $ctrl.NgayBanHanh = {
                    opened: false
                };
            }

            $ctrl.openNgayBanHanh = function () {
                $ctrl.NgayBanHanh.opened = true;
            };

            $ctrl.CapNhatDieuChinhVB = function () {
                $ctrl.para.IssuedDate = $ctrl.DueDate == null ? null : $ctrl.DueDate.toDateString();
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Đồng ý điều chỉnh ?';
                        }
                    }
                }).then(function () {
                    blockUI.start();
                    let resp = loginservice.postdata("api/QLBaoCao/CapNhatDieuChinh", $.param($ctrl.para));
                    resp.then(
                        function successCallback(response) {
                            blockUI.stop();
                            thongbao.success("Điều chỉnh VB thành công");
                            $uibModalInstance.close('close');
                        },
                        function errorCallback(response) {
                            blockUI.stop();
                        }
                    );
                }, function () {
                });
            }
        }
    ]);                                                     