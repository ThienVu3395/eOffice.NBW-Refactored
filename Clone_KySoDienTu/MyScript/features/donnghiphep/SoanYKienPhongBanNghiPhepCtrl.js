angular
    .module("aims")
    .controller('soanYKienPhongBanVBNghiPheptrl', [
        "$uibModalInstance",
        "blockUI",
        "loginservice",
        "idselect",
        "thongbao",
        "ModalService",
        function (
            $uibModalInstance,
            blockUI,
            loginservice,
            idselect,
            thongbao,
            ModalService) {
            var $ctrl = this;

            $ctrl.ykienxulync = null;

            $ctrl.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $ctrl.GuiYKienXuLy = function () {
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Đồng ý cập nhật ý kiến của phòng ban ?';
                        }
                    }
                }).then(function () {
                    idselect.noiDung = $ctrl.ykienxulync;
                    var resp = loginservice.postdata("api/QLNghiPhep/CapNhatYKienPhongBan", $.param(idselect));
                    resp.then(
                        function successCallback(response) {
                            $uibModalInstance.close($ctrl.ykienxulync);
                        },
                        function errorCallback(err) {
                            thongbao.error(err.data.Message);
                            blockUI.stop();
                        }
                    );
                }, function () {
                    blockUI.stop();
                });
            }
        }
    ]);