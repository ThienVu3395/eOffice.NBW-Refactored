angular
    .module("aims")
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
                const GetBoPhanFromArray = (loaiXuLy) => idselect.NhomPhanCong.filter(f => f.LOAIXULY === loaiXuLy).map(m => m.BOPHAN).join(",");
                const GetUserFromArray = (loaiXuLy) => idselect.UserPhanCong.filter(f => f.LOAIXULY === loaiXuLy && f.isChecked == 1).map(m => `${m.FullName} (${m.BOPHAN})`).join(",");
                let arrayNhomTheoDoi = GetBoPhanFromArray(2);
                let arrayUserTheoDoi = GetUserFromArray(2);
                return `<p>Chuyển: ${arrayNhomTheoDoi + ' '} ${arrayUserTheoDoi} tiếp nhận.</p>`;
            }

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

            function decodeHTMLEntities(input) {
                const doc = new DOMParser().parseFromString(input, 'text/html');
                return doc.documentElement.textContent;
            }

            function removeHTMLTagsAndNewlines(input) {
                //const regex = /<[^>]+>/g;
                //const decodedText = decodeHTMLEntities(input);
                //return decodedText.replace(regex, '').replace(/\n/g, '');
                const decodedText = decodeHTMLEntities(input);
                return decodedText.replace(/<p>(.*?)<\/p>/g, '$1\n').trim();
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
                    let resp = loginservice.postdata("api/QLBaoCao/GuiYKienXuLy", $.param({ valint1: idselect.IDVanBan, valstring1: $ctrl.ykienxulync, valint2: parentID, valint3: trangThaiComment, valint4: idselect.Module }));
                    resp.then(
                        function successCallback(response) {
                            blockUI.stop();
                            thongbao.success("Gửi ý kiến xử lý thành công");
                            $uibModalInstance.close('cancel');
                        },
                        function errorCallback(response) {
                            blockUI.stop();
                        }
                    );
                }, function () {
                    blockUI.stop();
                });
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
                }).then(function () {
                    blockUI.start();
                    let noidungbutphe = $ctrl.ykienxulync != null ? removeHTMLTagsAndNewlines(decodeHTMLEntities($ctrl.ykienxulync)) : null;
                    let resp = loginservice.postdata("api/QLBaoCao/CapNhatButPhe", $.param({ valint1: idselect.IDVanBan, valstring1: $ctrl.ykienxulync, valint2: idselect.Module, valstring2: noidungbutphe }));
                    resp.then(
                        function successCallback(response) {
                            blockUI.stop();
                            thongbao.success("Cập nhật bút phê chỉ đạo thành công");
                            $uibModalInstance.close('cancel');
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