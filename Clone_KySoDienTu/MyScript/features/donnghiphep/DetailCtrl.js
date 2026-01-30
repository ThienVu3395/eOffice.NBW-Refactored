angular
    .module("aims")
    .controller('chiTietVBNghiPhepCtrl', [
        "$scope",
        "$uibModalInstance",
        "blockUI",
        "appSettings",
        "loginservice",
        "userProfile",
        "idselect",
        "thongbao",
        "ModalService",
        "$window",
        "commonConstants",
        function (
            $scope,
            $uibModalInstance,
            blockUI,
            appSettings,
            loginservice,
            userProfile,
            idselect,
            thongbao,
            ModalService,
            $window,
            commonConstants) {
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

            $ctrl.taoGiup = false;

            Init();

            function Init() {
                //console.log(idselect);
                $ctrl.Filter = {};
                $ctrl.Filter.FilterVanBan = {};
                $ctrl.Filter.FilterLenhDieuXe = {};
                $ctrl.Filter.FilterLenhDieuXe.Filters = [];
                $ctrl.Filter.FilterVanBan.ID = idselect.IDVanBan;
                $ctrl.Filter.FilterVanBan.SoVanBanID = idselect.Module;
                $ctrl.Filter.FilterVanBan.LoaiVB = idselect.Loai;
                //$ctrl.Filter.FilterVanBan.LoaiLoc = 6; // loại lọc chỉ dành cho việc load chi tiết
                $ctrl.maxNumberUsers = 5;
                $ctrl.ykienxuly = null;
                $ctrl.quyenuser = {};
                $ctrl.objVB = {};
                GetAccessToken();
                UpdateNgayMo();
                GetYKienXuLy();
                KhoiTaoSoanThaoTini();
                CheckQuyenUserThuong(0);
            }

            function GetAccessToken() {
                var resp = loginservice.getdata("api/QLDieuXe/GetAccessToken_DonNghiPhep");
                resp.then(
                    function successCallback(response) {
                        $ctrl.accessToken = response.data;
                    },
                    function errorCallback(err) {
                        blockUI.stop();
                    }
                );
            }

            function UpdateNgayMo() {
                blockUI.start();
                let resp = loginservice.postdata("api/QLBaoCao/UpdateNgayMo", $.param({ valint1: idselect.IDVanBan, valint2: idselect.Module }));
                resp.then(
                    function successCallback(response) {
                        blockUI.stop();
                    },
                    function errorCallback(response) {
                        blockUI.stop();
                    }
                );
            }

            function GetYKienXuLy() {
                let resp = loginservice.postdata("api/QLDieuXe/GetYKienXuLy", $.param({ valint1: idselect.IDVanBan, valint2: idselect.Loai }));
                resp.then(
                    function successCallback(response) {
                        $ctrl.dsYKien = response.data;
                    },
                    function errorCallback(err) {
                        blockUI.stop();
                    }
                );
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
                resp.then(
                    function successCallback(response) {
                        $ctrl.quyenuser.CapNhatVB = response.data.findIndex(x => x.PermissionAction == 'URP_NP') > -1;
                        $ctrl.quyenuser.DuyetVB = response.data.findIndex(x => x.PermissionAction == 'ARP_NP') > -1;
                        $ctrl.quyenuser.XoaVB = response.data.findIndex(x => x.PermissionAction == 'DRP_NP') > -1;
                    },
                    function errorCallback(err) {
                    }
                );
            }

            function GetChiTietVB() {
                let obj = {
                    propertyName: "Id",
                    type: 0,
                    value: idselect.IDVanBan
                }
                $ctrl.Filter.FilterLenhDieuXe.Filters.push(obj);
                let resp = loginservice.postdata("api/QLNghiPhep/GetVanBanChiTiet", $.param($ctrl.Filter));
                resp.then(
                    function successCallback(response) {
                        $ctrl.objVB = response.data.DonNghiPhep;
                        $ctrl.objNhanVien = response.data.NhanVien;
                        $ctrl.taoGiup = parseInt($ctrl.objVB.maNhanVien) == parseInt($ctrl.objVB.maNhanVienNguoiTao) ? false : true;
                        $ctrl.nguoiDangDangNhap = parseInt($ctrl.objVB.maNhanVien) == parseInt($ctrl.Userdata.manhanvien) ? true : false;
                        if ($ctrl.objVB.FileDinhKem.length > 0) {
                            XemFile($ctrl.objVB.FileDinhKem[0].ID);
                        }
                        else {
                            $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=12&id=" + $ctrl.objVB.loai;
                        }
                        KiemTraTrangThaiKySo();
                        $ctrl.Filter.FilterLenhDieuXe.Filters = [];
                    },
                    function errorCallback(err) {
                        blockUI.stop();
                    }
                );
            }

            function KiemTraTrangThaiKySo() {
                if ($ctrl.objVB.trangThai == 0 && $ctrl.objVB.smartCAStringID != null && $ctrl.objVB.KySoID != null) {
                    let resp = loginservice.postdata("api/QLVanBan/KiemTraTrangThaiKySo", $.param({ valstring1: $ctrl.objVB.nguoiDuyet, valstring2: $ctrl.objVB.KySoID, valint1: idselect.IDVanBan, valint2: idselect.Module }));
                    resp.then(
                        function successCallback(response) {
                            $ctrl.tranStatus = response.data;
                        },
                        function errorCallback(response) {
                            $ctrl.tranStatus = -1;
                        }
                    );
                }
            }

            function XemFile(id) {
                //document.getElementById("childframe22").contentWindow.location.reload();
                $ctrl.fileid = id;
                $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?id=" + id + "&type=14";
            }

            function LoadReport(data) {
                blockUI.start();
                let serviceUrl = "../VanBanKiSo/GetDetailDonNghiPhep/";
                let resp = loginservice.postdataNormal(serviceUrl, data);
                resp.then(
                    function successCallback(response) {
                        $window.open("../Report/ReportVanBanKiSoViewer.aspx", "_newtab");
                        setTimeout(function () {
                            GetChiTietVB();
                            thongbao.success("Cập nhật thành công");
                            blockUI.stop();
                        }, 10000);
                    },
                    function errorCallback(err) {
                        thongbao.error(err.message);
                        blockUI.stop();
                    }
                );
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
                var resp = loginservice.getdata("api/QLDieuXe/downloadfile_unsign?id=" + $ctrl.objVB.smartCAStringID);
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
                let url = "api/QLDieuXe/getviewpdfvb?id=" + $ctrl.fileid + "&module=" + idselect.Loai;
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

            $ctrl.ViewFile = function (item) {
                if (item.LOAIFILE == "PDF" || item.LOAIFILE == "pdf") {
                    XemFile(item.ID);
                }
                else {
                    let result = item.NGAYTAO.split("T")[0].split("-");
                    let a = document.createElement('a');
                    a.target = "_blank";
                    a.href = appSettings.serverPath + '/Report/ReportFile/' + result[0] + '/' + result[1] + '/' + item.TENFILE;
                    a.click();
                }
            }

            $ctrl.MoFormThemSuaVB = function () {
                ModalService.open({
                    templateUrl: 'formThemSuaVB.html',
                    controller: 'themSuaVBNghiPhepCtrl',
                    size: 'lg100',
                    resolve: {
                        idselect: function () {
                            let item = {};
                            item.IDVanBan = idselect.IDVanBan;
                            item.Module = idselect.Module;
                            item.Loai = $ctrl.objVB.loai;
                            item.MaNhanVien = $ctrl.objVB.maNhanVien;
                            item.IDLoaiDiDuong = $ctrl.objVB.idLoaiDiDuong;
                            return item;
                        }
                    }
                }).then(function (c) {
                    $ctrl.Filter.FilterVanBan.LoaiVB = c.loai;
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
                            item.IDVanBan = idselect.IDVanBan;
                            item.Module = idselect.Module;
                            item.Loai = idselect.Loai;
                            item.LoaiYKien = loaiykien;
                            return item;
                        }
                    }
                }).then(function () {
                    GetYKienXuLy();
                }, function () {
                    GetYKienXuLy();
                });
            }

            $ctrl.MoFormYKienPhongBan = function () {
                ModalService.open({
                    templateUrl: 'modalYKienPhongBan.html',
                    controller: 'soanYKienPhongBanCtrl',
                    resolve: {
                        idselect: function () {
                            let item = {};
                            item.id = $ctrl.objVB.id;
                            item.loai = $ctrl.objVB.loai;
                            item.FileDinhKem = $ctrl.objVB.FileDinhKem;
                            item.accessToken = $ctrl.accessToken;
                            return item;
                        }
                    }
                }).then(function (yKienPhongBan) {
                    $ctrl.objVB.yKienPhongBanDoi = yKienPhongBan;
                    LoadReport($ctrl.objVB);
                }, function () {
                });
            }

            $ctrl.MoFormThongKeBaoCao = function () {
                ModalService.open({
                    templateUrl: 'modalThongKeBaoCao.html',
                    controller: 'baoCaoThongKeNghiPhepCtrl',
                    size: 'md',
                    resolve: {
                        idselect: function () {
                            return $ctrl.objVB.maNhanVien;
                        }
                    }
                }).then(function () {
                }, function () {
                });
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
                    let resp = loginservice.postdata("api/QLDieuXe/GuiYKienXuLy", $.param({ valint1: idselect.IDVanBan, valstring1: $ctrl.ykienxuly, valint2: parentID, valint3: 1, valint4: idselect.Loai, valint5: idselect.Module }));
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
                    blockUI.stop();
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
                }).then(
                    function successCallback() {
                        let resp = loginservice.postdata("api/QLDieuXe/CapNhatTrangThaiYKienXuLy", $.param({ valint1: ID, valint2: 0, valint3: idselect.Loai }));
                        resp.then(
                            function successCallback(response) {
                                thongbao.success("Ý kiến xử lý đã được xóa");
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

            $ctrl.XoaYKienPhongBan = function () {
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Đồng ý xóa ý kiến này ?';
                        }
                    }
                }).then(function () {
                    let item = {};
                    item.id = $ctrl.objVB.id;
                    item.loai = $ctrl.objVB.loai;
                    item.FileDinhKem = $ctrl.objVB.FileDinhKem;
                    item.accessToken = $ctrl.accessToken;
                    item.noiDung = null;
                    var resp = loginservice.postdata("api/QLNghiPhep/CapNhatYKienPhongBan", $.param(item));
                    resp.then(
                        function successCallback(response) {
                            $ctrl.objVB.yKienPhongBanDoi = null;
                            LoadReport($ctrl.objVB);
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

            $ctrl.GuiDuyetVB = function () {
                if ($ctrl.objVB.DanhSachNguoiKy.length == 0 || $ctrl.objVB.DanhSachNguoiKy == null) {
                    thongbao.error("Xin vui lòng chọn người ký");
                    return;
                }
                if ($ctrl.objVB.FileDinhKem.filter((file) => file.IsCRFile == 1).length == 0 || $ctrl.objVB.FileDinhKem == null) {
                    thongbao.error("Xin vui lòng chọn file đính kèm");
                    return;
                }
                if ($ctrl.taoGiup == true && $ctrl.objVB.FileDinhKem.filter((file) => file.IsCRFile == 0).length == 0) {
                    thongbao.error("Đơn nghỉ phép/giải trình tạo giúp cần phải đính kèm file scan đã được ký bởi người xin nghỉ phép/làm đơn");
                    return;
                }
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Xác nhận gửi duyệt đơn nghỉ phép/giải trình này ?';
                        }
                    }
                }).then(function () {
                    $ctrl.signSmartCA = {};
                    $ctrl.Item = {};
                    $ctrl.Item.module = idselect.Module;
                    $ctrl.Item.status = 0;
                    $ctrl.Item.refId = idselect.IDVanBan;
                    $ctrl.Item.linkAPICallback = appSettings.serverSaveFileSign_DonNghiPhep;
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
                        if (item.IsCRFile == 1) {
                            let fileSingle = {};
                            fileSingle.filename = item.MOTA;
                            fileSingle.filepath = item.VITRIFILE;
                            fileSingle.unsignData = item.BASE64DATA;
                            fileSingle.createdDated = new Date(item.NGAYTAO).toDateString();
                            $ctrl.signSmartCA.ListFile.push(fileSingle);
                        }
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
                    blockUI.stop();
                });
            }

            $ctrl.KyDuyetVB = function () {
                if ($ctrl.objVB.yKienPhongBanDoi == null && $ctrl.objVB.DanhSachNguoiKy[0].UserName == $ctrl.Userdata.username && $ctrl.objVB.loai == 8) {
                    thongbao.error("Văn bản chưa có ý kiến phòng/ban, vui lòng kiểm tra lại trước khi ký");
                }
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Đồng ý kí duyệt đơn nghỉ phép/giải trình ? Sau khi bấm đồng ý hệ thống sẽ gửi tin nhắn xác nhận kí số đến ứng dụng SmartCA trên thiết bị di động ?';
                        }
                    }
                }).then(function () {
                    blockUI.start({ message: "Xin vui lòng kiểm tra ứng dụng SmartCA trên điện thoại của bạn" });
                    if ($ctrl.objVB.DanhSachNguoiKy[0].UserName == $ctrl.Userdata.username &&
                        $ctrl.objVB.loai == 8) {
                        var resp = loginservice.postdata("api/QLNghiPhep/CapNhatFileBiBatDongBo", $.param({ valint1: idselect.IDVanBan, valint2: $ctrl.objVB.loai, valstring1: $ctrl.objVB.smartCAStringID }));
                        resp.then(
                            function successCallback(response) {
                                var res = loginservice.postdata("api/QLDieuXe/GuiThongBaoKyDuyet", $.param({ valint1: idselect.IDVanBan, valint2: idselect.Module, valstring1: $ctrl.objVB.smartCAStringID, valstring2: $ctrl.objVB.nguoiDuyet, valstring3: $ctrl.accessToken }));
                                res.then(
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
                            },
                            function errorCallback(err) {
                                blockUI.stop();
                            }
                        );
                    }
                    else {
                        var resp = loginservice.postdata("api/QLDieuXe/GuiThongBaoKyDuyet", $.param({ valint1: idselect.IDVanBan, valint2: idselect.Module, valstring1: $ctrl.objVB.smartCAStringID, valstring2: $ctrl.objVB.nguoiDuyet, valstring3: $ctrl.accessToken }));
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
                    }
                }, function () {
                    blockUI.stop();
                });
            }

            $ctrl.ThuHoiVB = function () {
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Xác nhận thu hồi đơn nghỉ phép/giải trình này ?';
                        }
                    }
                }).then(function () {
                    let resp = loginservice.postdata("api/QLDieuXe/ThuHoiVB", $.param({ valint1: idselect.IDVanBan, valint2: 2, valint3: idselect.Module, valstring1: $ctrl.objVB.smartCAStringID, valstring2: $ctrl.accessToken }));
                    resp.then(
                        function successCallback(response) {
                            thongbao.success("Đơn nghỉ phép/giải trình đã được thu hồi");
                            GetChiTietVB();
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
                            return 'Xác nhận hủy ký đơn nghỉ phép/giải trình này ?';
                        }
                    }
                }).then(function () {
                    let resp = loginservice.postdata("api/QLDieuXe/HuyKyVB", $.param({ valint1: idselect.IDVanBan, valint2: 1, valint3: idselect.Module, valint4: $ctrl.objVB.ThuTuKySo, valstring1: $ctrl.objVB.smartCAStringID, valstring2: $ctrl.accessToken }));
                    resp.then(
                        function successCallback(response) {
                            document.getElementById('childframe').contentWindow.location.reload();
                            GetChiTietVB();
                            thongbao.success("Đơn nghỉ phép/giải trình đã được hủy ký");
                        },
                        function errorCallback(response) {
                        }
                    );
                }, function () {
                    blockUI.stop();
                });
            }

            $ctrl.YeuCauHuyVB = function () {
                var currentTime = new Date();
                var hanChotHuyDon = new Date($ctrl.objVB.denNgay);

                if (hanChotHuyDon.getDay() === 0) { // Chủ nhật
                    hanChotHuyDon.setDate(hanChotHuyDon.getDate() + 1);
                }
                else if (hanChotHuyDon.getDay() === 6) { // Thứ bảy
                    hanChotHuyDon.setDate(hanChotHuyDon.getDate() + 2);
                }
                else {
                    // Cộng lên 2 ngày
                    hanChotHuyDon.setDate(hanChotHuyDon.getDate() + 2);

                    // Kiểm tra nếu là thứ bảy hoặc chủ nhật, thì cộng thêm
                    if (hanChotHuyDon.getDay() === 0) { // Chủ nhật
                        hanChotHuyDon.setDate(hanChotHuyDon.getDate() + 1);
                    }
                    else if (hanChotHuyDon.getDay() === 6) { // Thứ bảy
                        hanChotHuyDon.setDate(hanChotHuyDon.getDate() + 2);
                    }
                }
                hanChotHuyDon.setHours(23);
                hanChotHuyDon.setMinutes(59);
                hanChotHuyDon.setSeconds(59);
                //console.log(hanChotHuyDon);
                if (currentTime > hanChotHuyDon) {
                    ModalService.open({
                        templateUrl: 'modalHuyVB.html',
                        controller: 'soanLyDoHuyVBCtrl',
                        size: 'md',
                        resolve: {
                            idselect: function () {
                                let item = {};
                                item.IDVanBan = idselect.IDVanBan;
                                item.Module = idselect.Module;
                                item.Loai = idselect.Loai;
                                item.SmartCAStringID = $ctrl.objVB.smartCAStringID;
                                item.AccessToken = $ctrl.accessToken;
                                return item;
                            }
                        }
                    }).then(function () {
                        GetChiTietVB();
                    }, function () {
                    });
                }
                else {
                    ModalService.open({
                        templateUrl: 'ConfirmModal.html',
                        controller: 'modalConfirmInstanceCtrl',
                        size: 'md',
                        resolve: {
                            confirmMessage: function () {
                                return 'Xác nhận hủy đơn nghỉ phép/giải trình này ?';
                            }
                        }
                    }).then(function () {
                        let resp = loginservice.postdata("api/QLDieuXe/GoDuyetVB", $.param({ valint1: idselect.IDVanBan, valint2: 5, valint3: idselect.Module, valstring1: $ctrl.objVB.smartCAStringID, valstring2: $ctrl.accessToken }));
                        resp.then(
                            function successCallback(response) {
                                document.getElementById('childframe').contentWindow.location.reload();
                                GetChiTietVB();
                                thongbao.success("Đơn nghỉ phép/giải trình đã được hủy");
                            },
                            function errorCallback(response) {
                            }
                        );
                    }, function () {
                        blockUI.stop();
                    });
                }
            }

            $ctrl.XacNhanHuyVB = function () {
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Xác nhận hủy đơn nghỉ phép/giải trình này ?';
                        }
                    }
                }).then(function () {
                    let resp = loginservice.postdata("api/QLDieuXe/GoDuyetVB", $.param({ valint1: idselect.IDVanBan, valint2: 5, valint3: idselect.Module, valstring1: $ctrl.objVB.smartCAStringID, valstring2: $ctrl.accessToken }));
                    resp.then(
                        function successCallback(response) {
                            document.getElementById('childframe').contentWindow.location.reload();
                            GetChiTietVB();
                            thongbao.success("Đơn nghỉ phép/giải trình đã được hủy");
                        },
                        function errorCallback(response) {
                        }
                    );
                }, function () {
                    blockUI.stop();
                });
            }

            $ctrl.TuChoiHuyVB = function () {
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Xác nhận từ chối yêu cầu hủy đơn nghỉ phép/giải trình này ?';
                        }
                    }
                }).then(function () {
                    let resp = loginservice.postdata("api/QLDieuXe/TuChoiHuyVB", $.param({ valint1: idselect.IDVanBan, valint3: idselect.Module, valstring2: $ctrl.accessToken }));
                    resp.then(
                        function successCallback(response) {
                            GetChiTietVB();
                            thongbao.success("Đã từ chối yêu cầu hủy đơn nghỉ phép/giải trình này");
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
                            return 'Đồng ý xóa đơn nghỉ phép/giải trình ?';
                        }
                    }
                }).then(function () {
                    blockUI.start();
                    var resp = loginservice.postdata("api/QLDieuXe/XoaVanBan", $.param({ valint1: idselect.IDVanBan, valint2: idselect.Module, valint3: idselect.Loai, valstring1: $ctrl.accessToken }));
                    resp.then(
                        function successCallback(response) {
                            blockUI.stop();
                            thongbao.success(response.data.message);
                            $uibModalInstance.close('close');
                        },
                        function errorCallback(err) {
                            blockUI.stop();
                        }
                    );
                }, function () {
                    blockUI.stop();
                });
            }

            $ctrl.CapNhatTrangThaiKySo = function () {
                let resp = loginservice.postdata("api/QLDieuXe/CapNhatTrangThaiKySo", $.param({ valstring1: $ctrl.objVB.nguoiDuyet, valstring2: $ctrl.objVB.KySoID, valstring3: $ctrl.objVB.smartCAStringID, valint1: $ctrl.objVB.id, valint2: idselect.Module, valstring6: $ctrl.accessToken }));
                resp.then(function (response) {
                    document.getElementById('childframe').contentWindow.location.reload();
                    GetChiTietVB();
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            // Ng-If HTML View
            $ctrl.coTheChinhSua = function () {
                return (
                    $ctrl.objVB.nguoiTao == $ctrl.Userdata.username &&
                    $ctrl.objVB.trangThai == commonConstants.TRANG_THAI_LUU_NHAP
                );
            }

            $ctrl.coTheThuHoi = function () {
                return (
                    $ctrl.objVB.nguoiTao == $ctrl.Userdata.username &&
                    $ctrl.objVB.KySoID == null &&
                    $ctrl.objVB.ThuTuKySo == 1 &&
                    $ctrl.objVB.trangThai == commonConstants.TRANG_THAI_CHO_DUYET
                );
            }

            $ctrl.coTheGuiDuyet = function () {
                return (
                    $ctrl.objVB.nguoiTao == $ctrl.Userdata.username &&
                    $ctrl.objVB.trangThai == commonConstants.TRANG_THAI_LUU_NHAP
                );
            }

            $ctrl.coTheKyDuyet = function () {
                return (
                    $ctrl.objVB.nguoiDuyet == $ctrl.Userdata.username &&
                    $ctrl.objVB.KySoID == null &&
                    $ctrl.objVB.trangThai == commonConstants.TRANG_THAI_CHO_DUYET
                );
            }

            $ctrl.coTheHuyKyDuyet = function () {
                return (
                    $ctrl.objVB.nguoiDuyet == $ctrl.Userdata.username &&
                    $ctrl.objVB.KySoID == null &&
                    $ctrl.objVB.trangThai == commonConstants.TRANG_THAI_CHO_DUYET
                );
            }

            $ctrl.coTheYeuCauHuy = function () {
                return (
                    $ctrl.objVB.nguoiTao == $ctrl.Userdata.username &&
                    $ctrl.objVB.trangThai == commonConstants.TRANG_THAI_DA_DUYET
                );
            }

            $ctrl.coTheXacNhanHuy = function () {
                return (
                    $ctrl.quyenuser.XoaVB &&
                    $ctrl.objVB.trangThai == commonConstants.TRANG_THAI_CHO_HUY_DON_NGHI_PHEP
                );
            }

            $ctrl.coTheTuChoiHuy = function () {
                return (
                    $ctrl.quyenuser.XoaVB &&
                    $ctrl.objVB.trangThai == commonConstants.TRANG_THAI_CHO_HUY_DON_NGHI_PHEP
                );
            }

            $ctrl.coTheCapNhatTrangThaiKySo = function () {
                return (
                    (
                        $ctrl.objVB.nguoiDuyet == $ctrl.Userdata.username ||
                        $ctrl.objVB.nguoiTao == $ctrl.Userdata.username
                    ) &&
                    $ctrl.objVB.trangThai == commonConstants.TRANG_THAI_CHO_DUYET &&
                    $ctrl.objVB.KySoID != null &&
                    $ctrl.tranStatus != 4000
                );
            }

            $ctrl.coTheXoa = function () {
                return (
                    (
                        $ctrl.objVB.nguoiTao == $ctrl.Userdata.username ||
                        $ctrl.nguoiDangDangNhap == true
                    ) &&
                    $ctrl.objVB.trangThai == commonConstants.TRANG_THAI_LUU_NHAP
                );
            }

            $ctrl.coTheXoaYKien = function (username) {
                return (
                    username == $ctrl.Userdata.username &&
                    (
                        $ctrl.objVB.TrangThai != commonConstants.TRANG_THAI_LUU_NHAP &&
                        $ctrl.objVB.TrangThai != commonConstants.TRANG_THAI_CHO_HUY
                    )
                );
            }
        }
    ]);