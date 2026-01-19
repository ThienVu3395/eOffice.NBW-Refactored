angular.module("oamsapp")
    .controller('soanYKienXuLyVBReportCtrl', [
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
        }])
    .controller('soanLyDoHuyVBCtrl', [
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
        }])
    .controller('soanYKienPhongBanCtrl', [
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
        }]);