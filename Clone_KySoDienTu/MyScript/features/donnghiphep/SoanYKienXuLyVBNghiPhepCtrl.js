angular
    .module("aims")
    .controller('soanYKienXuLyVBNghiPhepCtrl', [
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

            $ctrl.TieuDeForm = idselect.LoaiYKien == "BUT_PHE" ? "bút phê chỉ đạo" : "ý kiến xử lý";

            $ctrl.LoaiForm = idselect.LoaiYKien;

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

            $ctrl.GuiYKienXuLy = function (parentID) {
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Đồng ý gửi ý kiến ?';
                        }
                    }
                }).then(function () {
                    blockUI.start();
                    let resp = loginservice.postdata("api/QLDieuXe/GuiYKienXuLy", $.param({ valint1: idselect.IDVanBan, valstring1: $ctrl.ykienxulync, valint2: parentID, valint3: 1, valint4: idselect.Loai }));
                    resp.then(
                        function successCallback(response) {
                            blockUI.stop();
                            thongbao.success("Gửi ý kiến xử lý thành công");
                            $uibModalInstance.close('close');
                        },
                        function errorCallback(response) {
                            blockUI.stop();
                        }
                    );
                }, function () {
                    blockUI.stop();
                });
            }
        }
    ]);