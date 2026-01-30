angular
    .module("aims")
    .controller('soanLyDoHuyVBNghiPhepCtrl', [
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

            $ctrl.ykienxulync = null;

            $ctrl.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            KhoiTaoSoanThaoTini();

            function KhoiTaoSoanThaoTini() {
                // Khai báo đối tượng dùng để soạn thảo văn bản
                $scope.tinymceOptions = {
                    onChange: function (e) {
                        // put logic here for keypress and cut/paste changes
                    },
                    menubar: false,
                    toolbar: 'undo redo | bold italic underline | forecolor bullist',
                    plugins: 'textcolor advlist',
                    skin: 'lightgray',
                    theme: 'modern',
                    content_css: "/Scripts/tinymce/tinyMod.css"
                };
            }

            $ctrl.GuiLyDoHuyDon = function () {
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Đồng ý gửi lý do hủy đơn ?';
                        }
                    }
                }).then(function () {
                    blockUI.start();
                    let resp = loginservice.postdata("api/QLDieuXe/YeuCauHuyVB", $.param({ id: idselect.IDVanBan, module: idselect.Module, accessToken: idselect.AccessToken, lyDoHuyDon: $ctrl.ykienxulync }));
                    resp.then(
                        function successCallback(response) {
                            blockUI.stop();
                            $uibModalInstance.close('close');
                        },
                        function errorCallback(response) {
                            thongbao.success("Xin thử lại sau");
                            blockUI.stop();
                        }
                    );
                }, function () {
                    blockUI.stop();
                });
            }
        }
    ]);