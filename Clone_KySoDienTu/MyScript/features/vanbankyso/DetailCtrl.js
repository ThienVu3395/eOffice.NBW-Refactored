angular
    .module("aimss")
    .controller('chiTietVBKySoCtrl', [
        "$scope",
        "$uibModalInstance",
        "blockUI",
        "appSettings",
        "loginservice",
        "userProfile",
        "idselect",
        "thongbao",
        "ModalService",
        function (
            $scope,
            $uibModalInstance,
            blockUI,
            appSettings,
            loginservice,
            userProfile,
            idselect,
            thongbao,
            ModalService) {
            var $ctrl = this;

            $ctrl.NamHienTai = new Date().getFullYear();

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

            $ctrl.Userdata = userProfile.getProfile();

            Init();

            function Init() {
                $ctrl.maxNumberUsers = 5;
                $ctrl.ykienxuly = null;
                $ctrl.quyenuser = {};
                UpdateNgayMo();
                GetYKienXuLy();
                KhoiTaoSoanThaoTini();
                CheckQuyenUserThuong(0);
                CheckQuyenDongMoc();
            }

            function UpdateNgayMo() {
                blockUI.start();
                let resp = loginservice.postdata("api/QLBaoCao/UpdateNgayMo", $.param({ valint1: idselect.IDVanBan, valint2: idselect.Module }));
                resp.then(function (response) {
                    blockUI.stop();
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            function GetChiTietVB() {
                let resp = loginservice.postdata("api/QLBaoCao/GetVanBanChiTiet2", $.param({ valint1: idselect.IDVanBan, valint3: idselect.Module }));
                resp.then(function (response) {
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
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            function KiemTraTrangThaiKySo() {
                if ($ctrl.objVB.TrangThai == 0 && $ctrl.objVB.SmartCAStringID != null && $ctrl.objVB.KySoID != null) {
                    let resp = loginservice.postdata("api/QLVanBan/KiemTraTrangThaiKySo", $.param({ valstring1: $ctrl.objVB.NguoiDuyet, valstring2: $ctrl.objVB.KySoID, valint1: idselect.IDVanBan, valint2: idselect.Module }));
                    resp.then(function (response) {
                        $ctrl.tranStatus = response.data;
                    }
                        , function errorCallback(response) {
                            $ctrl.tranStatus = -1;
                        });
                }
            }

            function GetYKienXuLy() {
                let resp = loginservice.postdata("api/QLBaoCao/GetYKienXuLy", $.param({ valint1: idselect.IDVanBan }));
                resp.then(function (response) {
                    $ctrl.dsYKien = response.data;
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
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

            function CheckQuyenUserThuong(resourceid) {
                var resp = loginservice.postdata("api/getCore/getAllPermissionsOfUserByModuleResource", $.param({ valstring1: "RP", valstring2: 'G', valstring3: resourceid }));
                resp.then(function (response) {
                    //console.log(response.data);
                    $ctrl.quyenuser.DuyetVB = response.data.findIndex(x => x.PermissionAction == 'ARP_ALL') > -1;
                    $ctrl.quyenuser.PhanPhatVB = response.data.findIndex(x => x.PermissionAction == 'PPRP_ALL') > -1;
                    $ctrl.quyenuser.ButPheVB = response.data.findIndex(x => x.PermissionAction == 'BPPP_ALL') > -1;
                    $ctrl.quyenuser.MocVB = response.data.findIndex(x => x.PermissionAction == 'MOCRP_ALL') > -1;
                    $ctrl.quyenuser.DieuChinhVB = response.data.findIndex(x => x.PermissionAction == 'AURP_ALL') > -1;
                    $ctrl.quyenuser.ChinhSuaViTriChuKy = response.data.findIndex(x => x.PermissionAction == 'UPDATE_SIGN') > -1;
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            function CheckQuyenDongMoc() {
                var resp = loginservice.postdata("api/getCore/getAllPermissionsOfUserByModuleResource", $.param({ valstring1: "WF", valstring2: 'G', valstring3: '3' }));
                resp.then(function (response) {
                    //console.log(response.data);
                    $ctrl.quyenuser.DongMocCongTy = response.data.findIndex(x => x.PermissionAction == 'MARK_CNNB') > -1;
                    $ctrl.quyenuser.DongMocDangBo = response.data.findIndex(x => x.PermissionAction == 'MARK_DB') > -1;
                    $ctrl.quyenuser.DongMocDoanTN = response.data.findIndex(x => x.PermissionAction == 'MARK_DTN') > -1;
                    $ctrl.quyenuser.DongMocCongDoan = response.data.findIndex(x => x.PermissionAction == 'MARK_CD') > -1;

                    $ctrl.TenMocCongTy = "cnnb";
                    $ctrl.TenMocDangBo = "mocdang";
                    $ctrl.TenMocDoanTN = "mocdoan";
                    $ctrl.TenMocCongDoan = "moccongdoan";
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            function XemFile(id) {
                //document.getElementById("childframe22").contentWindow.location.reload();
                $ctrl.fileid = id;
                $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?id=" + id + "&type=11";
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

            function DownloadBase64File(contentBase64, fileName) {
                const linkSource = `data:application/pdf;base64,${contentBase64}`;
                const downloadLink = document.createElement('a');
                document.body.appendChild(downloadLink);

                downloadLink.href = linkSource;
                downloadLink.target = '_self';
                downloadLink.download = fileName;
                downloadLink.click();
            }

            $ctrl.DownloadUnsignedFile = function () {
                blockUI.start();
                var resp = loginservice.getdata("api/QLDieuXe/downloadfile_unsign?id=" + $ctrl.objVB.SmartCAStringID);
                resp.then(function (response) {
                    response.data.forEach(function (item) {
                        DownloadBase64File(item.valstring2, item.valstring1);
                    });
                    blockUI.stop();
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            $ctrl.DownloadFile = function () {
                blockUI.start();
                let url = "api/QLBaoCao/getviewpdfvb?id=" + $ctrl.fileid;
                var resp = loginservice.getdatafile(url);
                resp.then(function (response) {
                    blockUI.stop();
                    var headers = response.headers();
                    var filename = headers['x-filename'];
                    var contentType = headers['content-type'];
                    var file = new Blob([response.data], { type: contentType });
                    saveAs(file, filename);
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                        thongbao.error("Không tìm thấy file");
                    });
            }

            $ctrl.PrintFile = function () {
                blockUI.start();
                let url = "api/QLBaoCao/getviewpdfvb?id=" + $ctrl.fileid;
                var resp = loginservice.getdatafile(url);
                resp.then(function (response) {
                    blockUI.stop();
                    var headers = response.headers();
                    var contentType = headers['content-type'];
                    var file = new Blob([response.data], { type: contentType });
                    var reader = new FileReader();
                    reader.readAsDataURL(file);
                    reader.onloadend = function () {
                        window.printJS({ printable: reader.result.split(',')[1], type: 'pdf', base64: true });
                    }
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                        thongbao.error("Không tìm thấy file");
                    });
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
                    controller: 'themSuaVB2Ctrl',
                    size: 'lg100',
                    resolve: {
                        idselect: function () {
                            let item = {};
                            item.IDVanBan = $ctrl.objVB.ID;
                            item.Module = 2;
                            return item;
                        }
                    }
                }).then(function () {
                    GetChiTietVB();

                }, function () {
                    GetChiTietVB();
                });
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
                }).then(function () {
                    GetChiTietVB();
                }, function () {
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
                }).then(function () {
                    GetChiTietVB();
                }, function () {
                });
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
                }).then(function () {
                    if (loaiykien == "BUT_PHE") {
                        GetChiTietVB();
                    }
                    else {
                        GetYKienXuLy();
                    }
                }, function () {
                    if (loaiykien == "BUT_PHE") {
                        GetChiTietVB();
                    }
                    else {
                        GetYKienXuLy();
                    }
                });
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
                }).then(function () {
                    blockUI.start();
                    let resp = loginservice.postdata("api/QLBaoCao/GuiYKienXuLy", $.param({ valint1: idselect.IDVanBan, valstring1: $ctrl.ykienxuly, valint2: parentID, valint3: trangThai, valint4: idselect.Module }));
                    resp.then(
                        function successCallback(response) {
                            blockUI.stop();
                            $ctrl.ykienxuly = null;
                            GetYKienXuLy();
                        },
                        function errorCallback(response) {
                            blockUI.stop();
                        }
                    );
                }, function () {
                });
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
                }).then(function () {
                    let resp = loginservice.postdata("api/QLBaoCao/CapNhatTrangThaiYKienXuLy", $.param({ valint1: ID, valint2: 0 }));
                    resp.then(
                        function successCallback(response) {
                            thongbao.success("Ý kiến xử lý đã được xóa");
                            GetYKienXuLy();
                        },
                        function errorCallback(response) {
                        }
                    );
                }, function () {
                    blockUI.stop();
                });
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
                }).then(function () {
                    let resp = loginservice.postdata("api/QLBaoCao/XoaVanBan", $.param({ valint1: idselect.IDVanBan }));
                    resp.then(
                        function successCallback(response) {
                            thongbao.success("Xóa Thành Công");
                            $ctrl.cancel();
                        },
                        function errorCallback(response) {
                            blockUI.stop();
                        }
                    );
                }, function () {
                });
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
                }).then(function () {
                    $ctrl.signSmartCA = {};
                    $ctrl.Item = {};
                    $ctrl.Item.module = idselect.Module;
                    $ctrl.Item.status = 0;
                    $ctrl.Item.refId = idselect.IDVanBan;
                    $ctrl.Item.linkAPICallback = appSettings.serverSaveFileSign_BaoCao;
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
                    var resp = loginservice.postdataSmartCA(appSettings.apiSmartCA_CreateSign, $.param($ctrl.signSmartCA));
                    resp.then(
                        function successCallback(response) {
                            window.open(appSettings.serverCreateSign + response.data);
                        },
                        function errorCallback(response) {
                            console.log(response.data);
                        }
                    );
                }, function () {
                });
            }

            $ctrl.KyDuyetVB = function () {
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
                }).then(function () {
                    blockUI.start({ message: "Xin vui lòng kiểm tra ứng dụng SmartCA trên điện thoại của bạn" });
                    let noidungbutphe = $ctrl.objVB.ButPhe != null ? removeHTMLTagsAndNewlines(decodeHTMLEntities($ctrl.objVB.ButPhe)) : null;
                    var resp = loginservice.postdata("api/QLBaoCao/GuiThongBaoKyDuyet", $.param({ valint1: idselect.IDVanBan, valint2: idselect.Module, valstring1: $ctrl.objVB.SmartCAStringID, valstring2: $ctrl.objVB.NguoiDuyet, valstring3: noidungbutphe }));
                    resp.then(
                        function successCallback(response) {
                            document.getElementById('childframe').contentWindow.location.reload();
                            if (response.data == -7) {
                                thongbao.error("Sai thông tin đăng nhập SmartCA được cung cấp trên VPĐT");
                            }
                            blockUI.stop();
                        },
                        function errorCallback(err) {
                            blockUI.stop();
                        }
                    );
                }, function () {
                });
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
                }).then(function () {
                    blockUI.start({ message: "Xin vui lòng kiểm tra ứng dụng SmartCA trên điện thoại của bạn" });
                    let noidungbutphe = $ctrl.objVB.ButPhe != null ? removeHTMLTagsAndNewlines(decodeHTMLEntities($ctrl.objVB.ButPhe)) : null;
                    var resp = loginservice.postdata("api/QLBaoCao/GuiThongBaoKyDuyet", $.param({ valint1: idselect.IDVanBan, valint2: idselect.Module, valstring1: $ctrl.objVB.SmartCAStringID, valstring2: $ctrl.objVB.NguoiDuyet, valstring3: noidungbutphe }));
                    resp.then(
                        function successCallback(response) {
                            document.getElementById('childframe').contentWindow.location.reload();
                            if (response.data == -7) {
                                thongbao.error("Sai thông tin đăng nhập SmartCA được cung cấp trên VPĐT");
                            }
                            blockUI.stop();
                        },
                        function errorCallback(err) {
                            blockUI.stop();
                        }
                    );
                }, function () {
                });
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
                }).then(function () {
                    let resp = loginservice.postdata("api/QLBaoCao/ThuHoiVB", $.param({ valint1: idselect.IDVanBan, valint2: 2, valint3: idselect.Module, valstring1: $ctrl.objVB.SmartCAStringID }));
                    resp.then(
                        function successCallback(response) {
                            thongbao.success("Văn bản đã được thu hồi");
                        },
                        function errorCallback(response) {
                        }
                    );
                }, function () {
                    blockUI.stop();
                });
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
                }).then(function () {
                    let resp = loginservice.postdata("api/QLBaoCao/HuyKyVB", $.param({ valint1: idselect.IDVanBan, valint2: 1, valint3: idselect.Module, valint4: $ctrl.objVB.ThuTuKySo, valstring1: $ctrl.objVB.SmartCAStringID }));
                    resp.then(
                        function successCallback(response) {
                            document.getElementById('childframe').contentWindow.location.reload();
                            thongbao.success("Văn bản đã được hủy ký");
                        },
                        function errorCallback(response) {
                        }
                    );
                }, function () {
                    blockUI.stop();
                });
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
                }).then(function () {
                    let resp = loginservice.postdata("api/QLBaoCao/GoDuyetVB", $.param({ valint1: idselect.IDVanBan, valint2: 5, valint3: idselect.Module, valstring1: $ctrl.objVB.SmartCAStringID }));
                    resp.then(
                        function successCallback(response) {
                            document.getElementById('childframe').contentWindow.location.reload();
                            thongbao.success("Văn bản đã được gỡ duyệt");
                        },
                        function errorCallback(response) {
                        }
                    );
                }, function () {
                    blockUI.stop();
                });
            }

            $ctrl.CapNhatTrangThaiKySo = function () {
                let noidungbutphe = $ctrl.objVB.valstring7 != null ? removeHTMLTagsAndNewlines(decodeHTMLEntities($ctrl.objVB.valstring7)) : "";
                let resp = loginservice.postdata("api/QLVanBan/CapNhatTrangThaiKySo", $.param({ valstring1: $ctrl.objVB.NguoiDuyet, valstring2: $ctrl.objVB.KySoID, valstring3: $ctrl.objVB.SmartCAStringID, valint1: $ctrl.objVB.ID, valint2: idselect.Module, valstring4: noidungbutphe }));
                resp.then(function (response) {
                    document.getElementById('childframe').contentWindow.location.reload();
                    GetChiTietVB();
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            $ctrl.ChinhSuaViTriChuKy = function () {
                window.open(appSettings.serverCreateSign + $ctrl.objVB.SmartCAStringID);
            }
        }
    ]);                                                         