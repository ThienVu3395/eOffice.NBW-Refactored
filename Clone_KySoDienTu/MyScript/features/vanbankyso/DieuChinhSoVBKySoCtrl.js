angular
    .module("aims")
    .controller('dieuChinhSoVBCtrl', [
        "$scope",
        "$uibModalInstance",
        "blockUI",
        "ApiClient",
        "idselect",
        "notificationService",
        "ModalService",
        function (
            $scope,
            $uibModalInstance,
            blockUI,
            ApiClient,
            idselect,
            notificationService,
            ModalService) {
            var $ctrl = this;

            $scope.$on('closeModal', function (data, mess) {
                notificationService.success(mess);
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
                }).then(
                    function successCallback() {
                        blockUI.start();
                        let resp = ApiClient
                            .postData("api/QLVBKySo/CapNhatDieuChinh", $.param($ctrl.para))
                            .then(
                                function successCallback(response) {
                                    blockUI.stop();
                                    notificationService.success("Điều chỉnh VB thành công");
                                    $uibModalInstance.close('close');
                                },
                                function errorCallback(response) {
                                    blockUI.stop();
                                }
                            );
                    },
                    function errorCallback() {
                    }
                );
            }
        }
    ]);                                                     