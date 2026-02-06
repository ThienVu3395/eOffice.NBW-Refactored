angular
    .module("aims")
    .controller('soanYKienXuLyVBReportCtrl', [
        "$scope",
        "$uibModalInstance",
        "blockUI",
        "ApiClient",
        "idselect",
        "StringUtil",
        "notificationService",
        "ModalService",
        'TINYMCE_CONFIG',
        function (
            $scope,
            $uibModalInstance,
            blockUI,
            ApiClient,
            idselect,
            StringUtil,
            notificationService,
            ModalService,
            TINYMCE_CONFIG) {
            var $ctrl = this;

            let trangThaiComment = 1;

            $ctrl.ykienxulync = null;

            $ctrl.TieuDeForm = "";

            switch (idselect.LoaiYKien) {
                case "BUT_PHE":
                    $ctrl.TieuDeForm = "bút phê chỉ đạo";
                    $ctrl.ykienxulync = GetNoiDungButPheGoiY();
                    break;
                case "Y_KIEN":
                    $ctrl.TieuDeForm = "ý kiến xử lý";
                    break;
                case "Y_KIEN_TIEP_NHAN":
                    $ctrl.TieuDeForm = "ý kiến tiếp nhận";
                    trangThaiComment = 2;
                    break;
                default:
                    $ctrl.TieuDeForm = "";
            }

            $ctrl.LoaiForm = idselect.LoaiYKien;

            $ctrl.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            KhoiTaoSoanThaoTini();

            function GetNoiDungButPheGoiY() {
                const GetBoPhanFromArray = (loaiXuLy) => idselect.NhomPhanCong
                    .filter(f => f.LOAIXULY === loaiXuLy)
                    .map(m => m.BOPHAN)
                    .join(",");

                const GetUserFromArray = (loaiXuLy) => idselect.UserPhanCong
                    .filter(f => f.LOAIXULY === loaiXuLy && f.isChecked == 1)
                    .map(m => `${m.FullName} (${m.BOPHAN})`)
                    .join(",");

                let arrayNhomTheoDoi = GetBoPhanFromArray(2);
                let arrayUserTheoDoi = GetUserFromArray(2);

                return `<p>Chuyển: ${arrayNhomTheoDoi + ' '} ${arrayUserTheoDoi} tiếp nhận.</p>`;
            }

            function KhoiTaoSoanThaoTini() {
                // Khai báo đối tượng dùng để soạn thảo văn bản
                $scope.tinymceOptions = angular.copy(TINYMCE_CONFIG);
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
                }).then(
                    function successCallback() {
                        blockUI.start();
                        let resp = ApiClient
                            .postData("api/QLVBKySo/GuiYKienXuLy", $.param({
                                valint1: idselect.IDVanBan,
                                valstring1: $ctrl.ykienxulync,
                                valint2: parentID,
                                valint3: trangThaiComment,
                                valint4: idselect.Module
                            }))
                            .then(
                                function successCallback(response) {
                                    blockUI.stop();
                                    notificationService.success("Gửi ý kiến xử lý thành công");
                                    $uibModalInstance.close('cancel');
                                },
                                function errorCallback(response) {
                                    blockUI.stop();
                                }
                            );
                    },
                    function errorCallback() {
                        blockUI.stop();
                    }
                );
            }

            $ctrl.GuiButPheChiDao = function () {
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Đồng ý cập nhật bút phê chỉ đạo ?';
                        }
                    }
                }).then(
                    function successCallback() {
                        blockUI.start();
                        let noidungbutphe = $ctrl.ykienxulync != null ?
                            StringUtil.removeHTMLTagsAndNewlines($ctrl.ykienxulync) :
                            null;

                        let resp = ApiClient
                            .postData("api/QLVBKySo/CapNhatButPhe", $.param({
                                valint1: idselect.IDVanBan,
                                valstring1: $ctrl.ykienxulync,
                                valint2: idselect.Module,
                                valstring2: noidungbutphe
                            }))
                            .then(
                                function successCallback(response) {
                                    blockUI.stop();
                                    notificationService.success("Cập nhật bút phê chỉ đạo thành công");
                                    $uibModalInstance.close('cancel');
                                },
                                function errorCallback(response) {
                                    blockUI.stop();
                                }
                            );
                    },
                    function errorCallback() {
                        blockUI.stop();
                    }
                );
            }
        }
    ]);                                                          