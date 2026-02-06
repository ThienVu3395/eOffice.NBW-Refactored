angular
    .module("aims")
    .controller('chiTietVBKySoCtrl', [
        "$scope",
        "$uibModalInstance",
        "blockUI",
        "appSettings",
        "ApiClient",
        "UserProfileService",
        "idselect",
        "notificationService",
        "ModalService",
        "FileUtil",
        "StringUtil",
        'TINYMCE_CONFIG',
        "SPECIAL_ACCOUNTS",
        "PERMISSIONS",
        "STATUS",
        function (
            $scope,
            $uibModalInstance,
            blockUI,
            appSettings,
            ApiClient,
            UserProfileService,
            idselect,
            notificationService,
            ModalService,
            FileUtil,
            StringUtil,
            TINYMCE_CONFIG,
            SPECIAL_ACCOUNTS,
            PERMISSIONS,
            STATUS) {
            var $ctrl = this;

            $ctrl.NamHienTai = new Date().getFullYear();

            $ctrl.Userdata = UserProfileService.getProfile();

            $scope.$on('countThongBaoReport', function (data, mess) {
                switch (mess) {
                    case 0:
                        GetChiTietVB();
                        break;
                    case 1:
                        UpdateNgayMo();
                        //GetYKienXuLy();
                        break;
                }
            });

            $ctrl.cancel = function () {
                $uibModalInstance.close('cancel');
            };

            Init();

            function Init() {
                $ctrl.maxNumberUsers = 5;
                $ctrl.ykienxuly = null;
                $ctrl.quyenuser = {};
                $ctrl.objVB = {};


                $ctrl.TenMocCongTy = SPECIAL_ACCOUNTS.TAI_KHOAN_DONG_MOC.CONG_TY;
                $ctrl.TenMocDangBo = SPECIAL_ACCOUNTS.TAI_KHOAN_DONG_MOC.DANG_BO;
                $ctrl.TenMocDoanTN = SPECIAL_ACCOUNTS.TAI_KHOAN_DONG_MOC.DOAN_THANH_NIEN;
                $ctrl.TenMocCongDoan = SPECIAL_ACCOUNTS.TAI_KHOAN_DONG_MOC.CONG_DOAN;

                UpdateNgayMo();
                GetYKienXuLy();
                KhoiTaoSoanThaoTini();
                CheckQuyenUserThuong(0);
                CheckQuyenDongMoc();
            }

            function UpdateNgayMo() {
                blockUI.start();
                let resp = ApiClient
                    .postData("api/QLVBKySo/UpdateNgayMo", $.param({
                        valint1: idselect.IDVanBan,
                        valint2: idselect.Module
                    }))
                    .then(
                        function successCallback(response) {
                            blockUI.stop();
                        }
                        , function errorCallback(response) {
                            blockUI.stop();
                        }
                    );
            }

            function GetChiTietVB() {
                let resp = ApiClient
                    .postData("api/QLVBKySo/GetVanBanChiTiet", $.param({
                        valint1: idselect.IDVanBan,
                        valint3: idselect.Module
                    }))
                    .then(
                        function successCallback(response) {
                            $ctrl.objVB = response.data;
                            //console.log($ctrl.objVB);
                            if ($ctrl.objVB.LOAIXULY != null) {
                                let loaixuly = $ctrl.objVB.LOAIXULY.split(",");
                                $ctrl.quyenuser.CT = loaixuly.findIndex(x => x == "1") > -1;
                                $ctrl.quyenuser.TD = loaixuly.findIndex(x => x == "2") > -1;
                            }
                            if ($ctrl.objVB.FileDinhKem.length > 0) {
                                XemFile($ctrl.objVB.FileDinhKem[0].ID);
                            }
                            if ($ctrl.objVB.FileDinhKem.length == 0) {
                                $ctrl.urldoc = appSettings.serverPath + "/Viewfile/viewfile2?type=11&id=-1";
                            }
                            KiemTraTrangThaiKySo();
                        },
                        function errorCallback(response) {
                            blockUI.stop();
                        }
                    );
            }

            function KiemTraTrangThaiKySo() {
                if ($ctrl.objVB.TrangThai == 0 && $ctrl.objVB.SmartCAStringID != null && $ctrl.objVB.KySoID != null) {
                    let resp = ApiClient
                        .postData("api/QLVanBan/KiemTraTrangThaiKySo", $.param({
                            valstring1: $ctrl.objVB.NguoiDuyet,
                            valstring2: $ctrl.objVB.KySoID,
                            valint1: idselect.IDVanBan,
                            valint2: idselect.Module
                        }))
                        .then(
                            function successCallback(response) {
                                $ctrl.tranStatus = response.data;
                            },
                            function errorCallback(response) {
                                $ctrl.tranStatus = -1;
                            }
                        );
                }
            }

            function GetYKienXuLy() {
                let resp = ApiClient
                    .postData("api/QLVBKySo/GetYKienXuLy", $.param({ valint1: idselect.IDVanBan }))
                    .then(
                        function successCallback(response) {
                            $ctrl.dsYKien = response.data;
                        },
                        function errorCallback(response) {
                            blockUI.stop();
                        }
                    );
            }

            function KhoiTaoSoanThaoTini() {
                // Khai báo đối tượng dùng để soạn thảo văn bản
                $scope.tinymceOptions = angular.copy(TINYMCE_CONFIG);
            }

            function CheckQuyenUserThuong(resourceid) {
                var resp = ApiClient
                    .postData("api/getCore/getAllPermissionsOfUserByModuleResource", $.param({
                        valstring1: "RP",
                        valstring2: 'G',
                        valstring3: resourceid
                    }))
                    .then(
                        function successCallback(response) {
                            //console.log(response.data);
                            $ctrl.quyenuser.DuyetVB = response.data
                                .findIndex(x => x.PermissionAction == PERMISSIONS.VAN_BAN_KY_SO.DUYET) > -1;

                            $ctrl.quyenuser.PhanPhatVB = response.data
                                .findIndex(x => x.PermissionAction == PERMISSIONS.VAN_BAN_KY_SO.PHAN_PHAT) > -1;

                            $ctrl.quyenuser.ButPheVB = response.data
                                .findIndex(x => x.PermissionAction == PERMISSIONS.VAN_BAN_KY_SO.BUT_PHE) > -1;

                            $ctrl.quyenuser.MocVB = response.data
                                .findIndex(x => x.PermissionAction == PERMISSIONS.VAN_BAN_KY_SO.DONG_MOC) > -1;

                            $ctrl.quyenuser.DieuChinhVB = response.data
                                .findIndex(x => x.PermissionAction == PERMISSIONS.VAN_BAN_KY_SO.DIEU_CHINH) > -1;

                            $ctrl.quyenuser.ChinhSuaViTriChuKy = response.data
                                .findIndex(x => x.PermissionAction == PERMISSIONS.VAN_BAN_KY_SO.CHINH_SUA_VI_TRI_CHU_KY) > -1;
                        },
                        function errorCallback(response) {
                            blockUI.stop();
                        }
                    );
            }

            function CheckQuyenDongMoc() {
                var resp = ApiClient
                    .postData("api/getCore/getAllPermissionsOfUserByModuleResource", $.param({
                        valstring1: "WF",
                        valstring2: 'G',
                        valstring3: '3'
                    }))
                    .then(
                        function successCallback(response) {
                            //console.log(response.data);
                            $ctrl.quyenuser.DongMocCongTy = response.data
                                .findIndex(x => x.PermissionAction == PERMISSIONS.VAN_BAN_KY_SO.DONG_MOC_CONG_TY) > -1;

                            $ctrl.quyenuser.DongMocDangBo = response.data
                                .findIndex(x => x.PermissionAction == PERMISSIONS.VAN_BAN_KY_SO.DONG_MOC_DANG_BO) > -1;

                            $ctrl.quyenuser.DongMocDoanTN = response.data
                                .findIndex(x => x.PermissionAction == PERMISSIONS.VAN_BAN_KY_SO.DONG_MOC_DOAN_THANH_NIEN) > -1;

                            $ctrl.quyenuser.DongMocCongDoan = response.data
                                .findIndex(x => x.PermissionAction == PERMISSIONS.VAN_BAN_KY_SO.DONG_MOC_CONG_DOAN) > -1;
                        },
                        function errorCallback(response) {
                            blockUI.stop();
                        }
                    );
            }

            function XemFile(id) {
                //document.getElementById("childframe22").contentWindow.location.reload();
                $ctrl.fileid = id;
                $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?id=" + id + "&type=11";
            }

            $ctrl.DownloadUnsignedFile = function () {
                blockUI.start();
                var resp = ApiClient
                    .get(`api/QLDieuXe/downloadfile_unsign?id=${$ctrl.objVB.SmartCAStringID}`)
                    .then(
                        function successCallback(response) {
                            response.data.forEach(function (item) {
                                FileUtil.downloadBase64File(item.valstring2, item.valstring1);
                            });
                            blockUI.stop();
                        },
                        function errorCallback(response) {
                            blockUI.stop();
                        }
                    );
            }

            $ctrl.DownloadFile = function () {
                blockUI.start();
                let url = `api/QLVBKySo/getviewpdfvb?id=${$ctrl.fileid}`;
                var resp = ApiClient
                    .getdatafile(url)
                    .then(
                        function successCallback(response) {
                            blockUI.stop();
                            var headers = response.headers();
                            var filename = headers['x-filename'];
                            var contentType = headers['content-type'];
                            var file = new Blob([response.data], { type: contentType });
                            saveAs(file, filename);
                        },
                        function errorCallback(response) {
                            blockUI.stop();
                            notificationService.error("Không tìm thấy file");
                        }
                    );
            }

            $ctrl.PrintFile = function () {
                blockUI.start();
                let url = `api/QLVBKySo/getviewpdfvb?id=${$ctrl.fileid}`;
                var resp = ApiClient
                    .getdatafile(url)
                    .then(
                        function successCallback(response) {
                            blockUI.stop();
                            var headers = response.headers();
                            var contentType = headers['content-type'];
                            var file = new Blob([response.data], { type: contentType });
                            var reader = new FileReader();
                            reader.readAsDataURL(file);
                            reader.onloadend = function () {
                                window.printJS({ printable: reader.result.split(',')[1], type: 'pdf', base64: true });
                            }
                        },
                        function errorCallback(response) {
                            blockUI.stop();
                            notificationService.error("Không tìm thấy file");
                        }
                    );
            };

            $ctrl.ViewFile = function (ID) {
                XemFile(ID);
            }

            $ctrl.ViewFileCanCu = function (item) {
                if (item.LOAIFILE == "pdf" || item.LOAIFILE == "PDF") {
                    $scope.fileid = item.IDFile;
                    $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?id=" + item.IDFile + "&type=" + item.NhomId;
                }
                else {
                    let result = item.NGAYTAO.split("T")[0].split("-");
                    let a = document.createElement('a');
                    let physicalFileLink = appSettings.serverPath;
                    a.target = "_blank";
                    switch (item.Module) {
                        case 11: // văn bản ký số
                            physicalFileLink += '/Report/ReportFile/' + result[0] + '/' + result[1] + '/' + item.TENFILE;
                            break;
                        case 3: // công văn đến
                            physicalFileLink += '/CongVanFile/' + result[0] + '/' + result[1] + '/' + item.TENFILE;
                            break;
                        case 4: // công văn đi
                            physicalFileLink += '/CongVanFile/' + result[0] + '/' + result[1] + '/' + item.TENFILE;
                            break;
                        case -1: // tài liệu
                            physicalFileLink += '/DocData/' + item.NhomId + '/' + item.TENFILE;
                            break;
                        default:
                    }
                    a.href = physicalFileLink;
                    a.click();
                }
            }

            $ctrl.MoFormThemSuaVB = function () {
                ModalService.open({
                    templateUrl: 'formThemSuaVB.html',
                    controller: 'themSuaVBKySoCtrl',
                    size: 'lg100',
                    resolve: {
                        idselect: function () {
                            let item = {};
                            item.IDVanBan = $ctrl.objVB.ID;
                            item.Module = idselect.Module;
                            return item;
                        }
                    }
                }).then(
                    function successCallback() {
                        GetChiTietVB();

                    },
                    function errorCallback() {
                        GetChiTietVB();
                    }
                );
            }

            $ctrl.PhanPhatNhom = function () {
                ModalService.open({
                    templateUrl: 'modalPhanPhat.html',
                    controller: 'phanPhatVanBanKySoCtrl',
                    resolve: {
                        idselect: function () {
                            let item = {};
                            item.IDVANBAN = idselect.IDVanBan;
                            item.LoaiVB = idselect.Module;
                            return item;
                        }
                    }
                }).then(
                    function successCallback() {
                        GetChiTietVB();
                    },
                    function errorCallback() {
                        GetChiTietVB();
                    });
            }

            $ctrl.MoFormDieuChinhSoVB = function () {
                ModalService.open({
                    templateUrl: 'modalDieuChinhVBDi.html',
                    controller: 'dieuChinhSoVBCtrl',
                    size: 'md',
                    resolve: {
                        idselect: function () {
                            return $ctrl.objVB;
                        }
                    }
                }).then(
                    function successCallback() {
                        GetChiTietVB();
                    },
                    function errorCallback() {
                    }
                );
            }

            $ctrl.MoFormYKienXuLy = function (loaiykien) {
                ModalService.open({
                    templateUrl: 'modalYKienXuLyVB.html',
                    controller: 'soanYKienXuLyVBReportCtrl',
                    resolve: {
                        idselect: function () {
                            let item = {};
                            item.IDVanBan = $ctrl.objVB.ID;
                            item.Module = $ctrl.objVB.Loai;
                            item.LoaiYKien = loaiykien;
                            item.NhomPhanCong = $ctrl.objVB.NhomPhanCong;
                            item.UserPhanCong = $ctrl.objVB.UserPhanCong;
                            return item;
                        }
                    }
                }).then(
                    function successCallback() {
                        if (loaiykien == "BUT_PHE") {
                            GetChiTietVB();
                        }
                        else {
                            GetYKienXuLy();
                        }
                    },
                    function errorCallback() {
                        if (loaiykien == "BUT_PHE") {
                            GetChiTietVB();
                        }
                        else {
                            GetYKienXuLy();
                        }
                    }
                );
            }

            $ctrl.GuiYKienXuLy = function (parentID, trangThai) {
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
                                valstring1: $ctrl.ykienxuly,
                                valint2: parentID,
                                valint3: trangThai,
                                valint4: idselect.Module
                            }))
                            .then(
                                function successCallback(response) {
                                    blockUI.stop();
                                    $ctrl.ykienxuly = null;
                                    GetYKienXuLy();
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

            $ctrl.XoaYKienXuLy = function (ID) {
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Đồng ý xóa ý kiến xử lý này ?';
                        }
                    }
                }).then(
                    function successCallback() {
                        let resp = ApiClient
                            .postData("api/QLVBKySo/CapNhatTrangThaiYKienXuLy", $.param({ valint1: ID, valint2: 0 }))
                            .then(
                                function successCallback(response) {
                                    notificationService.success("Ý kiến xử lý đã được xóa");
                                    GetYKienXuLy();
                                },
                                function errorCallback(response) {
                                }
                            );
                    },
                    function errorCallback() {
                        blockUI.stop();
                    }
                );
            }

            $ctrl.XoaVanBan = function () {
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Đồng ý xóa văn bản này ?';
                        }
                    }
                }).then(
                    function successCallback() {
                        let resp = ApiClient
                            .postData("api/QLVBKySo/XoaVanBan", $.param({ valint1: idselect.IDVanBan }))
                            .then(
                                function successCallback(response) {
                                    notificationService.success("Xóa Thành Công");
                                    $ctrl.cancel();
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

            $ctrl.GuiDuyetVB = function () {
                if ($ctrl.objVB.DanhSachNguoiKy.length == 0 || $ctrl.objVB.DanhSachNguoiKy == null) {
                    thongbao.error("Xin vui lòng chọn người ký");
                    return;
                }
                if ($ctrl.objVB.FileDinhKem.length == 0 || $ctrl.objVB.FileDinhKem == null) {
                    thongbao.error("Xin vui lòng chọn file đính kèm");
                    return;
                }
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Xác nhận gửi duyệt văn bản này ?';
                        }
                    }
                }).then(
                    function successCallback() {
                        $ctrl.signSmartCA = {};
                        $ctrl.Item = {};
                        $ctrl.Item.module = idselect.Module;
                        $ctrl.Item.status = 0;
                        $ctrl.Item.refId = idselect.IDVanBan;
                        $ctrl.Item.linkAPICallback = appSettings.apiSmartCA.BASE_URL + appSettings.apiSmartCA.SAVE_FILE.VAN_BAN_KY_SO;
                        $ctrl.signSmartCA.Item = $ctrl.Item;
                        $ctrl.signSmartCA.ListSigner = [];
                        $ctrl.signSmartCA.ListFile = [];
                        $ctrl.objVB.DanhSachNguoiKy.forEach(function (item, index) {
                            let obj = {};
                            obj.userName = item.UserName;
                            obj.status = index + 1;
                            $ctrl.signSmartCA.ListSigner.push(obj);
                        });

                        $ctrl.objVB.FileDinhKem.forEach(function (item) {
                            let fileSingle = {};
                            fileSingle.filename = item.MOTA;
                            fileSingle.filepath = item.VITRIFILE;
                            fileSingle.unsignData = item.BASE64DATA;
                            fileSingle.createdDated = new Date(item.NGAYTAO).toDateString();
                            $ctrl.signSmartCA.ListFile.push(fileSingle);
                        });

                        var resp = ApiClient
                            .postDataForSmartCA(appSettings.apiSmartCA.BASE_URL + appSettings.apiSmartCA.CREATE_SIGN, $.param($ctrl.signSmartCA))
                            .then(
                                function successCallback(response) {
                                    window.open(appSettings.dragAndDropSignUIUrl + response.data);
                                },
                                function errorCallback(response) {
                                    console.log(response.data);
                                }
                            );
                    },
                    function errorCallback() {
                    }
                );
            }

            $ctrl.KySoVB = function () {
                if ($ctrl.objVB.valstring7 == null && !$ctrl.quyenuser.MocVB && $ctrl.quyenuser.ButPheVB) {
                    thongbao.error("Văn bản chưa có nội dung bút phê, vui lòng kiểm tra lại trước khi ký");
                }
                if ($ctrl.objVB.NguoiDuyet == "cnnb" && $ctrl.objVB.valstring1 == null && $ctrl.objVB.LoaiKiSo == 2) {
                    thongbao.error("Văn bản chưa được cấp số, vui lòng kiểm tra lại trước khi ký");
                }
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Đồng ý kí duyệt ? Sau khi bấm đồng ý hệ thống sẽ gửi tin nhắn xác nhận kí số đến ứng dụng SmartCA trên thiết bị di động ?';
                        }
                    }
                }).then(
                    function successCallback() {
                        blockUI.start({ message: "Xin vui lòng kiểm tra ứng dụng SmartCA trên điện thoại của bạn" });
                        let noidungbutphe = $ctrl.objVB.ButPhe != null ?
                            StringUtil.removeHTMLTagsAndNewlines($ctrl.objVB.ButPhe) :
                            null;

                        var resp = ApiClient
                            .postData("api/QLVBKySo/GuiThongBaoKyDuyet", $.param({
                                valint1: idselect.IDVanBan,
                                valint2: idselect.Module,
                                valstring1: $ctrl.objVB.SmartCAStringID,
                                valstring2: $ctrl.objVB.NguoiDuyet,
                                valstring3: noidungbutphe
                            }))
                            .then(
                                function successCallback(response) {
                                    document.getElementById('childframe').contentWindow.location.reload();
                                    if (response.data == -7) {
                                        notificationService.error("Sai thông tin đăng nhập SmartCA được cung cấp trên VPĐT");
                                    }
                                    blockUI.stop();
                                },
                                function errorCallback(err) {
                                    blockUI.stop();
                                }
                            );
                    },
                    function errorCallback() {
                    }
                );
            }

            $ctrl.DongDauVB = function () {
                if ($ctrl.objVB.NguoiDuyet == "cnnb" && $ctrl.objVB.valstring1 == null && $ctrl.objVB.LoaiKiSo == 2) {
                    thongbao.error("Văn bản chưa được cấp số, vui lòng kiểm tra lại trước khi ký");
                }
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Đồng ý kí duyệt ? Sau khi bấm đồng ý hệ thống sẽ gửi tin nhắn xác nhận kí số đến ứng dụng SmartCA trên thiết bị di động ?';
                        }
                    }
                }).then(
                    function successCallback() {
                        blockUI.start({ message: "Xin vui lòng kiểm tra ứng dụng SmartCA trên điện thoại của bạn" });

                        let noidungbutphe = $ctrl.objVB.ButPhe != null ?
                            StringUtil.removeHTMLTagsAndNewlines($ctrl.objVB.ButPhe) :
                            null;

                        var resp = ApiClient
                            .postData("api/QLVBKySo/GuiThongBaoKyDuyet", $.param({
                                valint1: idselect.IDVanBan,
                                valint2: idselect.Module,
                                valstring1: $ctrl.objVB.SmartCAStringID,
                                valstring2: $ctrl.objVB.NguoiDuyet,
                                valstring3: noidungbutphe
                            }))
                            .then(
                                function successCallback(response) {
                                    document.getElementById('childframe').contentWindow.location.reload();
                                    if (response.data == -7) {
                                        notificationService.error("Sai thông tin đăng nhập SmartCA được cung cấp trên VPĐT");
                                    }
                                    blockUI.stop();
                                },
                                function errorCallback(err) {
                                    blockUI.stop();
                                }
                            );
                    }, function errorCallback() {
                    }
                );
            }

            $ctrl.ThuHoiVB = function () {
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Xác nhận thu hồi văn bản này ?';
                        }
                    }
                }).then(
                    function successCallback() {
                        let resp = ApiClient
                            .postData("api/QLVBKySo/ThuHoiVB", $.param({
                                valint1: idselect.IDVanBan,
                                valint2: 2,
                                valint3: idselect.Module,
                                valstring1: $ctrl.objVB.SmartCAStringID
                            }))
                            .then(
                                function successCallback(response) {
                                    notificationService.success("Văn bản đã được thu hồi");
                                },
                                function errorCallback(response) {
                                }
                            );
                    },
                    function errorCallback() {
                        blockUI.stop();
                    }
                );
            }

            $ctrl.HuyKyVB = function () {
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Xác nhận hủy ký văn bản này ?';
                        }
                    }
                }).then(
                    function successCallback() {
                        let resp = ApiClient
                            .postData("api/QLVBKySo/HuyKyVB", $.param({
                                valint1: idselect.IDVanBan,
                                valint2: 1,
                                valint3: idselect.Module,
                                valint4: $ctrl.objVB.ThuTuKySo,
                                valstring1: $ctrl.objVB.SmartCAStringID
                            }))
                            .then(
                                function successCallback(response) {
                                    document.getElementById('childframe').contentWindow.location.reload();
                                    notificationService.success("Văn bản đã được hủy ký");
                                },
                                function errorCallback(response) {
                                }
                            );
                    },
                    function errorCallback() {
                        blockUI.stop();
                    }
                );
            }

            $ctrl.GoDuyetVB = function () {
                var currentTime = new Date();
                var nghiDenNgay = new Date($ctrl.objVB.NgayTao);
                nghiDenNgay.setHours(23);
                nghiDenNgay.setMinutes(59);
                nghiDenNgay.setSeconds(59);
                //console.log(nghiDenNgay);
                //if (currentTime > nghiDenNgay) {
                //    thongbao.error("Đã quá thời hạn để hủy văn bản này, văn bản chỉ được hủy trong ngày tạo đơn (" + nghiDenNgay.toLocaleDateString() + ")");
                //    return;
                //}
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Xác nhận hủy văn bản này ?';
                        }
                    }
                }).then(
                    function successCallback() {
                        let resp = ApiClient.postData("api/QLVBKySo/GoDuyetVB", $.param({
                            valint1: idselect.IDVanBan,
                            valint2: 5,
                            valint3: idselect.Module,
                            valstring1: $ctrl.objVB.SmartCAStringID
                        })).then(
                            function successCallback(response) {
                                document.getElementById('childframe').contentWindow.location.reload();
                                notificationService.success("Văn bản đã được gỡ duyệt");
                            },
                            function errorCallback(response) {
                            }
                        );
                    },
                    function errorCallback() {
                        blockUI.stop();
                    }
                );
            }

            $ctrl.CapNhatTrangThaiKySo = function () {
                let noidungbutphe = $ctrl.objVB.valstring7 != null ?
                    StringUtil.removeHTMLTagsAndNewlines($ctrl.objVB.valstring7) :
                    "";

                let resp = ApiClient
                    .postData("api/QLVanBan/CapNhatTrangThaiKySo", $.param({
                        valstring1: $ctrl.objVB.NguoiDuyet,
                        valstring2: $ctrl.objVB.KySoID,
                        valstring3: $ctrl.objVB.SmartCAStringID,
                        valint1: $ctrl.objVB.ID,
                        valint2: idselect.Module,
                        valstring4: noidungbutphe
                    }))
                    .then(
                        function successCallback(response) {
                            document.getElementById('childframe').contentWindow.location.reload();
                            GetChiTietVB();
                        },
                        function errorCallback(response) {
                            blockUI.stop();
                        }
                    );
            }

            $ctrl.ChinhSuaChuKy = function () {
                window.open(appSettings.serverCreateSign + $ctrl.objVB.SmartCAStringID);
            }

            // Ng-If HTML View
            $ctrl.coTheChinhSua = function () {
                return (
                    $ctrl.objVB.NguoiTao == $ctrl.Userdata.username &&
                    $ctrl.objVB.TrangThai == STATUS.LUU_NHAP
                );
            }

            $ctrl.coTheThuHoi = function () {
                return (
                    $ctrl.objVB.NguoiTao == $ctrl.Userdata.username &&
                    $ctrl.objVB.KySoID == null &&
                    $ctrl.objVB.ThuTuKySo == 1 &&
                    $ctrl.objVB.TrangThai == STATUS.CHO_DUYET
                );
            }

            $ctrl.coTheGuiDuyet = function () {
                return (
                    $ctrl.objVB.NguoiTao == $ctrl.Userdata.username &&
                    $ctrl.objVB.TrangThai == STATUS.LUU_NHAP
                );
            }

            $ctrl.coTheKySo = function () {
                return (
                    $ctrl.objVB.NguoiDuyet == $ctrl.Userdata.username &&
                    $ctrl.objVB.KySoID == null &&
                    !$ctrl.quyenuser.DongMocCongTy &&
                    !$ctrl.quyenuser.DongMocDangBo &&
                    !$ctrl.quyenuser.DongMocDoanTN &&
                    !$ctrl.quyenuser.DongMocCongDoan &&
                    $ctrl.objVB.TrangThai == STATUS.CHO_DUYET
                );
            }

            $ctrl.coTheChinhSuaChuKy = function () {
                return (
                    $ctrl.objVB.NguoiDuyet == $ctrl.Userdata.username &&
                    $ctrl.objVB.KySoID == null &&
                    $ctrl.quyenuser.ChinhSuaViTriChuKy &&
                    $ctrl.objVB.TrangThai == STATUS.CHO_DUYET
                );
            }

            $ctrl.coTheDongMoc = function () {
                return (
                    (
                        ($ctrl.objVB.NguoiDuyet == $ctrl.TenMocCongTy && $ctrl.quyenuser.DongMocCongTy) ||
                        ($ctrl.objVB.NguoiDuyet == $ctrl.TenMocDangBo && $ctrl.quyenuser.DongMocDangBo) ||
                        ($ctrl.objVB.NguoiDuyet == $ctrl.TenMocDoanTN && $ctrl.quyenuser.DongMocDoanTN) ||
                        ($ctrl.objVB.NguoiDuyet == $ctrl.TenMocCongDoan && $ctrl.quyenuser.DongMocCongDoan)
                    ) &&
                    $ctrl.objVB.TrangThai == STATUS.CHO_DUYET
                );
            }

            $ctrl.coTheCapNhatButPhe = function () {
                return (
                    (
                        ($ctrl.objVB.NguoiDuyet == $ctrl.Userdata.username) &&
                        ($ctrl.quyenuser.ButPheVB) &&
                        (!$ctrl.quyenuser.MocVB)
                    ) &&
                    $ctrl.objVB.TrangThai == STATUS.CHO_DUYET
                );
            }

            $ctrl.coTheHuyKy = function () {
                return (
                    $ctrl.objVB.NguoiDuyet == $ctrl.Userdata.username &&
                    $ctrl.objVB.KySoID == null &&
                    $ctrl.objVB.TrangThai == STATUS.CHO_DUYET
                );
            }

            $ctrl.coTheGoDuyet = function () {
                return (
                    $ctrl.objVB.NguoiTao == $ctrl.Userdata.username &&
                    $ctrl.objVB.TrangThai == STATUS.DA_DUYET
                );
            }

            $ctrl.coTheCapNhatTrangThaiKySo = function () {
                return (
                    (
                        $ctrl.objVB.NguoiDuyet == $ctrl.Userdata.username ||
                        $ctrl.objVB.NguoiTao == $ctrl.Userdata.username
                    ) &&
                    $ctrl.objVB.TrangThai == STATUS.CHO_DUYET &&
                    $ctrl.objVB.KySoID != null &&
                    $ctrl.tranStatus != 4000
                );
            }

            $ctrl.coTheDieuChinh = function () {
                return (
                    $ctrl.quyenuser.DieuChinhVB &&
                    $ctrl.objVB.TrangThai == STATUS.CHO_DUYET
                );
            }

            $ctrl.coTheChonNoiNhan = function () {
                return (
                    (
                        $ctrl.objVB.NguoiDuyet == $ctrl.Userdata.username ||
                        $ctrl.objVB.NguoiTao == $ctrl.Userdata.username ||
                        $ctrl.quyenuser.CT ||
                        $ctrl.quyenuser.TD
                    ) &&
                    ($ctrl.objVB.TrangThai == STATUS.CHO_DUYET || $ctrl.objVB.TrangThai == STATUS.DA_DUYET)
                );
            }

            $ctrl.coTheXoaVB = function () {
                return (
                    $ctrl.objVB.NguoiTao == $ctrl.Userdata.username &&
                    ($ctrl.objVB.TrangThai == STATUS.LUU_NHAP || $ctrl.objVB.TrangThai == STATUS.DA_XOA)
                );
            }

            $ctrl.coTheGuiYKienXuLy = function () {
                return (
                    $ctrl.quyenuser.CT &&
                    $ctrl.objVB.TrangThai != STATUS.LUU_NHAP
                );
            }

            $ctrl.coTheGuiYKienTiepNhan = function () {
                return (
                    $ctrl.quyenuser.TD &&
                    !$ctrl.quyenuser.MocVB &&
                    $ctrl.objVB.TrangThai != STATUS.LUU_NHAP
                );
            }

            $ctrl.coTheXoaYKien = function (username) {
                return (
                    username == $ctrl.Userdata.username &&
                    (
                        $ctrl.objVB.TrangThai != STATUS.LUU_NHAP
                    )
                );
            }
        }
    ]);                                                         