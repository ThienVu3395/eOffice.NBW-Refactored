angular
    .module("aims")
    .controller('themSuaVBNghiPhepCtrl', [
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
        "FileUploader",
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
            FileUploader,
            commonConstants) {
            var $ctrl = this;

            var today = new Date();

            var endDate_min = new Date();

            var maxNguoiDuyet = 2;

            var authHeaders = {};

            var createPdfTool = 0; // 0: Sử dụng Crystal Report; 1: Sử dụng Chrome Headless

            var apiCreateUrl = createPdfTool == 0 ? "api/QLNghiPhep/ThemVanBan" : "api/QLNghiPhep/ThemVanBanPDF";

            var apiUpdateUrl = createPdfTool == 0 ? "api/QLNghiPhep/CapNhatThongTinVanBan" : "api/QLNghiPhep/CapNhatThongTinVanBanPDF";

            $ctrl.NamHienTai = today.getFullYear();

            if (endDate_min.getDay() === 0) { // Chủ nhật
                endDate_min.setDate(endDate_min.getDate() - 3);
            }

            else if (endDate_min.getDay() === 6) { // Thứ bảy
                endDate_min.setDate(endDate_min.getDate() - 2);
            }

            else {
                // Trừ đi 2 ngày
                endDate_min.setDate(endDate_min.getDate() - 2);

                // Kiểm tra nếu là thứ bảy hoặc chủ nhật, thì lùi thêm
                if (endDate_min.getDay() === 0) { // Chủ nhật
                    endDate_min.setDate(endDate_min.getDate() - 2);
                }
                else if (endDate_min.getDay() === 6) { // Thứ bảy
                    endDate_min.setDate(endDate_min.getDate() - 2);
                }
            }

            $ctrl.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $ctrl.Userdata = userProfile.getProfile();

            Init();

            function Init() {
                $ctrl.nhanvien = {};
                $ctrl.loainghi = {};
                $ctrl.NgayTao = today;
                $ctrl.NgayNghiStart = today;
                $ctrl.NgayNghiEnd = today;
                $ctrl.NgayCap = null;
                $ctrl.Filter = {};
                $ctrl.Filter.FilterVanBan = {};
                $ctrl.Filter.FilterLenhDieuXe = {};
                $ctrl.Filter.FilterLenhDieuXe.Filters = [];
                $ctrl.Filter.FilterLenhDieuXe.Page = 1;
                $ctrl.Filter.FilterLenhDieuXe.PageSize = 1000;
                $ctrl.Filter.FilterVanBan.ID = idselect.IDVanBan;
                $ctrl.Filter.FilterVanBan.SoVanBanID = idselect.Module;
                $ctrl.Filter.FilterVanBan.LoaiVB = idselect.Loai;
                $ctrl.Filter.FilterVanBan.LoaiLoc = 0;
                $ctrl.para = {};
                $ctrl.para.FileDinhKem = [];
                $ctrl.para.DanhSachNguoiKy = [];
                $ctrl.TypeFile = [];
                $ctrl.DanhSachNhanVien = [];
                $ctrl.KhoaLoai = false;
                $ctrl.quyenuser = {};
                KhoiTaoSoanThaoTini();
                Loadtypefile();
                GetAccessToken();
                GetNgayNghiLe();
                GetQuyenUser();
                if (idselect.IDVanBan == null) {
                    $ctrl.TieuDeForm = commonConstants.MA_FORM_THEM_MOI;
                    $ctrl.para.Module = idselect.Module;
                    $ctrl.para.coChuKySo = false;
                    $ctrl.taoGiup = false;
                    //console.log($ctrl.Userdata.manhanvien);
                    GetDanhSachNhanVien($ctrl.Userdata.manhanvien);
                }
                else {
                    $ctrl.TieuDeForm = commonConstants.MA_FORM_CAP_NHAT;
                    $ctrl.taoGiup = parseInt(idselect.MaNhanVien) == parseInt($ctrl.Userdata.manhanvien) ? false : true;
                    //console.log(idselect.MaNhanVien);
                    GetDanhSachNhanVien(idselect.MaNhanVien);
                }
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

            function Loadtypefile() {
                var resp = loginservice.getdata("api/getCore/getdanhmuchethong?loai=TypeFile");
                resp.then(
                    function successCallback(response) {
                        $ctrl.TypeFile = response.data;
                        $ctrl.TypeFile.unshift({ ID: 19, CODE: "3", LOAIDM: "Loại file đính kèm", VALUENAME: "|jpg|", VALUENAMECODE: "TypeFile" });
                    },
                    function errorCallback(response) {
                    }
                );
            }

            function GetAccessToken() {
                var resp = loginservice.getdata("api/QLDieuXe/GetAccessToken_DonNghiPhep");
                resp.then(
                    function successCallback(response) {

                        $ctrl.accessToken = response.data;
                        console.log(response.data)
                    },
                    function errorCallback(err) {
                        blockUI.stop();
                    }
                );
            }

            function GetNgayNghiLe() {
                var resp = loginservice.getdata("api/QLNghiPhep/GetNgayNghiLe");
                resp.then(
                    function successCallback(response) {
                        $ctrl.NgayNghiLe = response.data;
                        $ctrl.NgayNghiLe.forEach((item) => {
                            let ngay = (item.ngay.split("T")[0]).split("-");
                            if (endDate_min.getDate() === parseInt(ngay[2]) && endDate_min.getMonth() + 1 === parseInt(ngay[1]) && endDate_min.getFullYear() === parseInt(ngay[0])) {
                                // Trừ đi 2 ngày
                                endDate_min.setDate(endDate_min.getDate() - 1);

                                // Kiểm tra nếu là thứ bảy hoặc chủ nhật, thì lùi thêm
                                if (endDate_min.getDay() === 0) { // Chủ nhật
                                    endDate_min.setDate(endDate_min.getDate() - 2);
                                }
                                else if (endDate_min.getDay() === 6) { // Thứ bảy
                                    endDate_min.setDate(endDate_min.getDate() - 2);
                                }
                            }
                        });

                        KhoiTaoDatePicker();
                    },
                    function errorCallback(err) {
                        blockUI.stop();
                    }
                );
            }

            function GetQuyenUser() {
                var resp = loginservice.postdata("api/getCore/getAllPermissionsOfUserByModuleResource", $.param({ valstring1: "RP", valstring2: 'G', valstring3: 0 }));
                resp.then(
                    function successCallback(response) {
                        $ctrl.quyenuser.TaoVBKhongGioiHanBienDo = response.data.findIndex(x => x.PermissionAction == 'CRP_NOLIMIT_NP') > -1;
                    },
                    function errorCallback(response) {
                    }
                );
            }

            function GetDanhSachNhanVien(manv) {
                var resp = loginservice.getdata("api/QLNghiPhep/GetDanhSachNhanVienTrongPhong?maNhanVien=" + manv);
                resp.then(
                    function successCallback(response) {
                        $ctrl.DanhSachNhanVien = response.data;
                        let index = $ctrl.DanhSachNhanVien.findIndex(x => parseInt(x.maNhanVien) == parseInt(manv));
                        if (index != -1) {
                            $ctrl.nhanvien = $ctrl.DanhSachNhanVien[index];
                        }
                        else {
                            $ctrl.nhanvien = $ctrl.DanhSachNhanVien[0];
                        }
                        GetThongTinNghiPhep($ctrl.nhanvien.maNhanVien, true);
                    },
                    function errorCallback(err) {
                        blockUI.stop();
                    }
                );
            }

            function GetThongTinNghiPhep(manv, isGetDanhSachDon) {
                var resp = loginservice.getdata(`api/QLNghiPhep/GetThongTinNghiPhepNhanVien?maNhanVien=${manv}&denNgay=${$ctrl.NgayNghiEnd.toDateString()}`);
                resp.then(
                    function successCallback(response) {
                        $ctrl.ThongTinNghiPhep = response.data;
                        $ctrl.para.maNhanVien = $ctrl.ThongTinNghiPhep.maNhanVien;
                        $ctrl.para.tenNhanVien = $ctrl.ThongTinNghiPhep.tenNhanVien;
                        $ctrl.para.namSinh = $ctrl.ThongTinNghiPhep.namSinh;
                        $ctrl.para.soCccd = $ctrl.ThongTinNghiPhep.soCccd;
                        $ctrl.para.ngayCapCccd = $ctrl.ThongTinNghiPhep.ngayCapCccd;
                        $ctrl.para.phongBan = $ctrl.ThongTinNghiPhep.tenPhongBan;
                        $ctrl.para.chucVu = $ctrl.ThongTinNghiPhep.tenChucVu;

                        $ctrl.IDChucVu = $ctrl.ThongTinNghiPhep.idChucVu;
                        $ctrl.NgayCap = $ctrl.ThongTinNghiPhep.ngayCapCccd == null ? null : new Date($ctrl.ThongTinNghiPhep.ngayCapCccd);
                        $ctrl.taoGiup = parseInt($ctrl.ThongTinNghiPhep.maNhanVien) == parseInt($ctrl.Userdata.manhanvien) ? false : true;
                        if (isGetDanhSachDon) {
                            GetDanhSachDon();
                        }
                    },
                    function errorCallback(err) {
                        blockUI.stop();
                    }
                );
            }

            function GetDanhSachDon() {
                const vaitroky = ["TB", "TP", "PTP-PT"];
                var resp = loginservice.getdata("api/getCore/getdanhmuchethong?loai=" + appSettings.typeNghiPhep);
                resp.then(
                    function successCallback(response) {
                        $ctrl.dsDon = response.data;

                        // bỏ mẫu đơn lệnh điều xe và phiếu đề nghị sử dụng xe
                        $ctrl.dsDon = $ctrl.dsDon.filter(x => x.ID != 3726 && x.ID != 3727);

                        // bỏ mẫu của phó phòng/nv (ng đang đăng nhập là trưởng)
                        if (vaitroky.includes($ctrl.IDChucVu)) {
                            $ctrl.dsDon = $ctrl.dsDon.filter(x => x.ID != 3716 && x.ID != 3748);
                        }

                        // bỏ mẫu của trưởng phòng (ng đang đăng nhập là phó/nv)
                        if (!vaitroky.includes($ctrl.IDChucVu)) {
                            $ctrl.dsDon = $ctrl.dsDon.filter(x => x.ID != 3723 && x.ID != 3749);
                        }

                        // Check người dùng còn phép không, nếu không chỉ hiện mẫu không lương,nếu còn sẽ mất mẫu k lương 
                        //if ($ctrl.ThongTinNghiPhep.soNgayConLai == 0 || $ctrl.ThongTinNghiPhep.soNgayConLai == null) {
                        //    $ctrl.dsDon = $ctrl.dsDon.filter(x => x.ID == 3724);
                        //}

                        //if ($ctrl.ThongTinNghiPhep.soNgayConLai > 0) {
                        //    $ctrl.dsDon = $ctrl.dsDon.filter(x => x.ID != 3724);
                        //}

                        // Tạo mới
                        if (idselect.IDVanBan == null) {
                            $ctrl.loai = $ctrl.dsDon[0].CODE;
                            GetDanhSachLoaiNghiPhep();
                            GetNguoiKyDuyet();
                        }

                        // Chỉnh sửa
                        if (idselect.IDVanBan != null) {
                            if (idselect.Loai != 0) {
                                let index = $ctrl.dsDon.findIndex(x => x.CODE == idselect.Loai.toString());
                                if (index != -1) {
                                    $ctrl.loai = $ctrl.dsDon[index].CODE;
                                }
                            }
                            else {
                                $ctrl.loai = $ctrl.dsDon[0].CODE;
                            }
                            $ctrl.messLoai = GetTieuDeLoaiDon($ctrl.loai).MessLoai;
                            GetChiTietVB();
                            GetDanhSachLoaiNghiPhep();
                        }

                        $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=12&id=" + parseInt($ctrl.loai);
                        $ctrl.Filter.FilterVanBan.LoaiVB = parseInt($ctrl.loai);
                    },
                    function errorCallback(err) {
                    }
                );
            }

            function GetDanhSachLoaiNghiPhep() {
                var resp = loginservice.getdata("api/QLNghiPhep/GetDanhSachLoaiNghiPhep");
                resp.then(
                    function successCallback(response) {
                        $ctrl.LoaiNghiPhep = response.data;

                        // #TODO : Làm tạm thời
                        $ctrl.LoaiNghiPhep = $ctrl.LoaiNghiPhep.filter(x => x.idLoaiDiDuong != 'TAU_LUA' && x.soNgay != -1)

                        // thêm mới
                        if (idselect.IDVanBan == null) {
                            // TODO: Thêm 1 loại nghỉ phép là nghỉ không lương ở API GetDanhSachLoaiNghiPhep
                            // Check người dùng còn phép không, nếu còn sẽ là loại nghỉ thường niên, còn không thì là loại ốm đau (vd)
                            if ($ctrl.ThongTinNghiPhep.soNgayConLai == 0) {
                                $ctrl.loainghi = $ctrl.LoaiNghiPhep[0];
                                $ctrl.KhoaLoai = false;
                            }

                            if ($ctrl.ThongTinNghiPhep.soNgayConLai > 0) {
                                $ctrl.loainghi = $ctrl.LoaiNghiPhep[0];
                                //$ctrl.KhoaLoai = true;
                            }
                        }

                        // Chỉnh sửa
                        else {
                            let index = $ctrl.LoaiNghiPhep.findIndex(x => x.id == idselect.IDLoaiDiDuong);
                            if (index != -1) {
                                $ctrl.loainghi = $ctrl.LoaiNghiPhep[index];
                            }
                            else {
                                $ctrl.loainghi = $ctrl.LoaiNghiPhep[0];
                            }
                        }

                        $ctrl.para.idLoaiDiDuong = $ctrl.loainghi.id;
                    },
                    function errorCallback(err) {
                        blockUI.stop();
                    }
                );
            }

            function GetNguoiKyDuyet() {
                let result = GetTieuDeLoaiDon($ctrl.loai);
                $ctrl.messLoai = result.MessLoai;
                var resp = loginservice.postdata("api/QLNghiPhep/GetNguoiKyDuyet", $.param({ valint1: result.Loai, valint2: parseInt($ctrl.para.maNhanVien), valstring1: $ctrl.ThongTinNghiPhep.idPhongBan, valstring2: $ctrl.ThongTinNghiPhep.idChucVu }));
                console.log(parseInt($ctrl.para.maNhanVien))

                resp.then(
                    function successCallback(response) {
                        $ctrl.para.DanhSachNguoiKy = response.data;
                        $ctrl.para.DanhSachNguoiKy.forEach(function (item) {
                            item.LoaiKy = 0;
                        });
                    },
                    function errorCallback(err) {
                        blockUI.stop();
                    }
                );
            }

            function GetTieuDeLoaiDon(type) {
                let loai = 10;
                let messLoai = "";
                switch (parseInt(type)) {
                    case commonConstants.MA_DON_NGHI_PHEP_NHAN_VIEN:
                        loai = 10;
                        messLoai = "Đơn xin nghỉ phép mẫu 1 cần Trưởng Phòng/Ban của người xin nghỉ và Trưởng P.TCHC ký duyệt";
                        break;
                    case commonConstants.MA_DON_NGHI_PHEP_TRUONG_PHONG:
                        loai = 11;
                        messLoai = "Đơn xin nghỉ phép mẫu 2 cần PGĐ phụ trách của người xin nghỉ và GĐ ký duyệt";
                        break;
                    case commonConstants.MA_DON_NGHI_PHEP_KHONG_LUONG:
                        loai = 12;
                        messLoai = "Đơn xin nghỉ việc không lương cần ý kiến và chữ ký của Trưởng Phòng/Ban của người xin nghỉ và GĐ ký duyệt";
                        break;
                    case commonConstants.MA_DON_NGHI_PHEP_DI_NUOC_NGOAI:
                        loai = 13;
                        messLoai = "Đơn xin nghỉ phép đi nước ngoài cần Trưởng Phòng/Ban của người xin nghỉ và GĐ ký duyệt";
                        break;
                    case commonConstants.MA_DON_GIAI_TRINH_FACEID_NHAN_VIEN:
                        loai = 14;
                        messLoai = "Đơn giải trình mẫu 5 cần Trưởng Phòng/Ban của người xin làm đơn ký duyệt";
                        break;
                    case commonConstants.MA_DON_GIAI_TRINH_FACEID_TRUONG_PHONG:
                        loai = 15;
                        messLoai = "Đơn giải trình mẫu 6 cần GĐ ký duyệt";
                        break;
                    default:
                        break;
                }
                return { Loai: loai, MessLoai: messLoai };
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
                        $ctrl.para = response.data.DonNghiPhep;
                        $ctrl.para.Module = idselect.Module;
                        //console.log($ctrl.para);
                        $ctrl.NgayDi = $ctrl.para.ngayDi == null ? null : new Date($ctrl.para.ngayDi);
                        $ctrl.NgayTao = $ctrl.para.ngayTao == null ? null : new Date($ctrl.para.ngayTao);
                        $ctrl.NgayCap = $ctrl.para.ngayCapCccd == null ? null : new Date($ctrl.para.ngayCapCccd);
                        $ctrl.NgayNghiStart = $ctrl.para.tuNgay == null ? null : new Date($ctrl.para.tuNgay);
                        $ctrl.NgayNghiEnd = $ctrl.para.denNgay == null ? null : new Date($ctrl.para.denNgay);
                        $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=12&id=" + idselect.Loai;
                        $ctrl.Filter.FilterLenhDieuXe.Filters = [];
                        $ctrl.para.FileDinhKem.forEach(function (item) {
                            delete item.BASE64DATA;
                        });
                    },
                    function errorCallback(err) {
                        blockUI.stop();
                    }
                );
            }

            function SpecialDateDisabled(data) {
                var date = data.date,
                    mode = data.mode;
                return mode === 'day' &&
                    (date.getDay() === 0 ||
                        (date.getDay() === 6 &&
                            (
                                !(date.getDate() == 4 && // ngày 4/5/2024: ngày làm bù của năm 2024
                                    date.getMonth() + 1 == 5 &&
                                    date.getFullYear() == 2024
                                ) &&
                                !(date.getDate() == 26 && // ngày 26/4/2024: ngày làm bù của năm 2025
                                    date.getMonth() + 1 == 4 &&
                                    date.getFullYear() == 2025
                                )
                            )
                        ) ||
                        $ctrl.NgayNghiLe.map(m => m.ngay).some(s => {
                            var ngayLe = new Date(s);
                            return date.getFullYear() == ngayLe.getFullYear() &&
                                date.getDate() == ngayLe.getDate() &&
                                date.getMonth() == ngayLe.getMonth()
                        })
                    );
            }

            function KhoiTaoDatePicker() {
                $ctrl.Ngay01 = {
                    opened: false
                };
                $ctrl.Ngay02 = {
                    opened: false
                };
                $ctrl.Ngay03 = {
                    opened: false
                };
                $ctrl.Ngay04 = {
                    opened: false
                };
                $ctrl.dateOptionsStartDate = {
                    showWeeks: false,
                    //minDate: today,
                    dateDisabled: SpecialDateDisabled
                }
                $ctrl.dateOptionsEndDate = {
                    showWeeks: false,
                    minDate: endDate_min,
                    dateDisabled: SpecialDateDisabled
                }
            }

            function LoadReport(data) {
                let serviceUrl = "../VanBanKiSo/GetDetailDonNghiPhep/";
                let resp = loginservice.postdataNormal(serviceUrl, data);
                resp.then(
                    function successCallback(response) {
                        $window.open("../Report/ReportVanBanKiSoViewer.aspx", "_newtab");
                        setTimeout(function () {
                            blockUI.stop();
                            thongbao.success("Tạo mới/Cập nhật văn bản thành công");
                            $uibModalInstance.close(data);
                        }, 10000);
                    },
                    function errorCallback(err) {
                        thongbao.error(err.message);
                    }
                );
            }

            if ($ctrl.Userdata.access_token) {
                authHeaders.Authorization = 'Bearer ' + $ctrl.Userdata.access_token;
            }

            var uploader = $ctrl.uploader = new FileUploader({
                headers: { "Authorization": authHeaders.Authorization },
                url: appSettings.serverPath + 'api/fileUpload/UploadFilesPRMS',
                withCredentials: true
            });

            uploader.filters.push({
                name: 'docFilter1',
                fn: function (item /*{File|FileLikeObject}*/, options) {
                    var type = '|' + item.name.slice(item.name.lastIndexOf('.') + 1) + '|';
                    if ($ctrl.TypeFile.find(x => x.CODE == 1).VALUENAME.indexOf(type.toLowerCase()) !== -1)
                        return true;
                    if ($ctrl.TypeFile.find(x => x.CODE == 2).VALUENAME.indexOf(type.toLowerCase()) !== -1)
                        return true;
                    if ($ctrl.TypeFile.find(x => x.CODE == 3).VALUENAME.indexOf(type.toLowerCase()) !== -1)
                        return true;
                    else {
                        alert("Không hỗ trợ định dạng file này!!");
                        return false;
                    }
                }
            });

            uploader.filters.push({
                name: 'asyncFilter',
                fn: function (item /*{File|FileLikeObject}*/, options, deferred) {
                    //console.log('asyncFilter');
                    setTimeout(deferred.resolve, 1e3);
                }
            });

            uploader.onAfterAddingFile = function (fileItem) {
                fileItem.upload();
            };

            uploader.onSuccessItem = function (fileItem, response, status, headers) {
                if (response != 0) {
                    let item = {
                        MOTA: fileItem.file.name,
                        LOAIFILE: fileItem.file.name.substr(fileItem.file.name.lastIndexOf('.') + 1),
                        SIZEFILE: fileItem.file.size,
                        IsCRFile: 0
                    };
                    if (idselect.IDVanBan == null) {
                        $ctrl.para.FileDinhKem.push(item);
                    }
                    else {
                        addFile(item);
                    }
                }
                else {
                    thongbao.error("Có lỗi xảy ra khi tải File đính kèm, vui lòng kiểm tra lại định dạng hoặc dung lượng tập tin");
                }
            };

            $ctrl.XemFile = function (item) {
                if (idselect.IDVanBan == null) {
                    let a = document.createElement('a');
                    a.target = "_blank";
                    a.href = appSettings.serverPath + '/Uploadtemp/' + $ctrl.Userdata.username + '/' + item.MOTA;
                    a.click();
                }
                else {
                    let result = item.NGAYTAO.split("T")[0].split("-");
                    let a = document.createElement('a');
                    a.target = "_blank";
                    a.href = appSettings.serverPath + '/Report/ReportFile/' + result[0] + '/' + result[1] + '/' + item.TENFILE;
                    a.click();
                }
            }

            $ctrl.XoaFile = function (item, index) {
                if (idselect.IDVanBan == null) {
                    xoaFileUploadTemp(item.MOTA, index);
                }
                else {
                    xoaFile(item.ID);
                }
            }

            function xoaFileUploadTemp(fileName, index) {
                var resp = loginservice.postdata("api/fileUpload/removefiletemp", $.param({ valstring1: fileName }));
                resp.then(
                    function successCallback(response) {
                        $ctrl.para.FileDinhKem.splice(index, 1);
                    },
                    function errorCallback(err) {
                        blockUI.stop();
                    }
                );
            }

            function xoaFile(id) {
                blockUI.start();
                var resp = loginservice.postdata("api/QLDieuXe/RemoveFile_Update", $.param({ valint1: id, valint2: $ctrl.para.Loai }));
                resp.then(
                    function successCallback(response) {
                        //alert(response.data);
                        blockUI.stop();
                        let index = $ctrl.para.FileDinhKem.findIndex(x => x.ID == id);
                        $ctrl.para.FileDinhKem.splice(index, 1);
                    },
                    function errorCallback(err) {
                        blockUI.stop();
                    }
                );
            }

            function addFile(item) {
                blockUI.start();
                var resp = loginservice.postdata("api/QLDieuXe/UploadFile_Update", $.param({ MoTa: item.MOTA, LoaiFile: item.LOAIFILE, VanBanID: $ctrl.para.id, SizeFile: item.SIZEFILE, Module: idselect.Loai }));
                resp.then(
                    function successCallback(response) {
                        blockUI.stop();
                        let nht = new Date();
                        item.ID = response.data.valint1;
                        item.IsCRFile = 0;
                        item.TENFILE = response.data.valstring1;
                        item.NGAYTAO = nht.getFullYear() + "-" + ((nht.getMonth() + 1) >= 10 ? (nht.getMonth() + 1) : "0" + (nht.getMonth() + 1)) + "-" + nht.getDate() + "T00:00:00";
                        $ctrl.para.FileDinhKem.push(item);
                    },
                    function errorCallback(err) {
                        blockUI.stop();
                    }
                );
            }

            $ctrl.DoiLoaiVB = function () {
                $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=12&id=" + parseInt($ctrl.loai);

                $ctrl.Filter.FilterVanBan.LoaiVB = parseInt($ctrl.loai);

                let index = -1;

                // Loại mẫu đơn chọn là đơn giải trình k lấy faceID thì sẽ chuyển sang loại nghỉ là đơn giải trình và ẩn select 
                if ($ctrl.Filter.FilterVanBan.LoaiVB == 28 || $ctrl.Filter.FilterVanBan.LoaiVB == 29) {
                    index = $ctrl.LoaiNghiPhep.findIndex(x => x.id == "DON_GIAI_TRINH");
                    $ctrl.loainghi = index != -1 ? $ctrl.LoaiNghiPhep[index] : $ctrl.LoaiNghiPhep[0];
                    $ctrl.KhoaLoai = true;
                }

                // Loại mẫu đơn chọn là đơn nghỉ phép thường niên mẫu 1 và mẫu 2 và không lương và xin đi nước ngoài
                else if ($ctrl.Filter.FilterVanBan.LoaiVB == 6 || $ctrl.Filter.FilterVanBan.LoaiVB == 7 || $ctrl.Filter.FilterVanBan.LoaiVB == 8 || $ctrl.Filter.FilterVanBan.LoaiVB == 9) {
                    // nếu hết số ngày phép thì sẽ chuyển sang loại nghỉ không lương
                    if ($ctrl.ThongTinNghiPhep.soNgayConLai == 0) {
                        index = $ctrl.LoaiNghiPhep.findIndex(x => x.id == "KHONG_LUONG");
                    }

                    // nếu còn số ngày phép thì sẽ chuyển sang loại nghỉ thường niên
                    else if ($ctrl.ThongTinNghiPhep.soNgayConLai > 0) {
                        index = $ctrl.LoaiNghiPhep.findIndex(x => x.id == "VIEC_RIENG");
                    }

                    $ctrl.loainghi = index != -1 ? $ctrl.LoaiNghiPhep[index] : $ctrl.LoaiNghiPhep[0];

                    $ctrl.KhoaLoai = false;
                }

                $ctrl.para.idLoaiDiDuong = $ctrl.loainghi.id;

                GetNguoiKyDuyet();
            }

            $ctrl.DoiNhanVien = function () {
                GetThongTinNghiPhep($ctrl.nhanvien.maNhanVien, true);
            }

            $ctrl.DoiLoaiNghiPhep = function () {
                $ctrl.para.idLoaiDiDuong = $ctrl.loainghi.id;
            }

            $ctrl.xoaNguoiKy = function (username) {
                let index = $ctrl.para.DanhSachNguoiKy.findIndex(x => x.UserName == username);
                if (index != -1) {
                    $ctrl.para.DanhSachNguoiKy.splice(index, 1);
                }
            }

            $ctrl.openNgayTao = function () {
                $ctrl.Ngay01.opened = true;
            };

            $ctrl.openNgayNghiStart = function () {
                $ctrl.Ngay02.opened = true;
            };

            $ctrl.openNgayNghiEnd = function () {
                $ctrl.Ngay03.opened = true;
            };

            $ctrl.openNgayCap = function () {
                $ctrl.Ngay04.opened = true;
            };

            $ctrl.changedStartDate = function () {
                let startdate = new Date($ctrl.NgayNghiStart);
                let enddate = new Date($ctrl.NgayNghiEnd);
                if (startdate > enddate) {
                    $ctrl.NgayNghiEnd = $ctrl.NgayNghiStart;
                }
                $ctrl.dateOptionsEndDate.minDate = (startdate < endDate_min) && !$ctrl.quyenuser.TaoVBKhongGioiHanBienDo ? endDate_min : startdate;
            };

            $ctrl.changedEndDate = function () {
                let startdate = new Date($ctrl.NgayNghiStart);
                let enddate = new Date($ctrl.NgayNghiEnd);
                if (startdate > enddate) {
                    $ctrl.NgayNghiStart = $ctrl.NgayNghiEnd;
                }
                GetThongTinNghiPhep($ctrl.para.maNhanVien, false);
            };

            $ctrl.moFormChonNguoiKy = function () {
                ModalService.open({
                    templateUrl: 'modalChonNguoiKy.html',
                    controller: 'chonNguoiKyVBNghiPhepCtrl',
                    size: 'md',
                    resolve: {
                        idselect: function () {
                            let obj = {};
                            obj.dsNguoiKy = $ctrl.para.DanhSachNguoiKy;
                            obj.maNhanVienNguoiTao = $ctrl.para.maNhanVien;
                            return obj;
                        }
                    }
                }).then(function (c) {
                    $ctrl.para.DanhSachNguoiKy.push(c);
                }, function () {
                    blockUI.stop();
                });
            }

            $ctrl.MoFormThongKeBaoCao = function () {
                ModalService.open({
                    templateUrl: 'modalThongKeBaoCao.html',
                    controller: 'baoCaoThongKeNghiPhepCtrl',
                    size: 'md',
                    resolve: {
                        idselect: function () {
                            return $ctrl.para.maNhanVien;
                        }
                    }
                }).then(function () {
                }, function () {
                    blockUI.stop();
                });
            }

            $ctrl.LuuNhap = function () {
                if ($ctrl.taoGiup == true && $ctrl.para.FileDinhKem.length == 0) {
                    thongbao.error("Đơn nghỉ phép/Đơn giải trình tạo giúp cần phải đính kèm file scan đã được ký bởi người xin nghỉ phép");
                    return;
                }
                if ($ctrl.para.DanhSachNguoiKy.length > maxNguoiDuyet || $ctrl.para.DanhSachNguoiKy.length == 0) {
                    thongbao.error("Đơn nghỉ phép/Đơn giải trình cần chọn tối đa " + maxNguoiDuyet + " và tối thiểu 1 người ký duyệt");
                    return;
                }
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Đồng ý tạo đơn nghỉ phép ?';
                        }
                    }
                }).then(
                    function successCallback() {
                        blockUI.start({ message: "Đơn nghỉ phép/Đơn giải trình đang được tạo, xin vui lòng chờ" });
                        $ctrl.para.id = null;
                        $ctrl.para.trangThai = 2;
                        $ctrl.para.ngayTao = $ctrl.NgayTao == null ? today.toDateString() : $ctrl.NgayTao.toDateString();
                        $ctrl.para.ngayCapCccd = $ctrl.NgayCap == null ? today.toDateString() : $ctrl.NgayCap.toDateString();
                        $ctrl.para.tuNgay = $ctrl.NgayNghiStart == null ? today.toDateString() : $ctrl.NgayNghiStart.toDateString();
                        $ctrl.para.denNgay = $ctrl.NgayNghiEnd == null ? today.toDateString() : $ctrl.NgayNghiEnd.toDateString();
                        $ctrl.para.loai = parseInt($ctrl.loai);
                        $ctrl.para.accessToken = $ctrl.accessToken;
                        var resp = loginservice.postdata(apiCreateUrl, $.param($ctrl.para));
                        resp.then(
                            function successCallback(response) {
                                //console.log(response.data);
                                $ctrl.para.id = response.data.Id;

                                if (createPdfTool == 0) {
                                    LoadReport($ctrl.para);
                                }
                                else {
                                    blockUI.stop();
                                    thongbao.success("Tạo mới thành công");
                                    $uibModalInstance.close($ctrl.para);
                                }

                                //GetChiTietVB_LoadReport($ctrl.para);
                            },
                            function errorCallback(err) {
                                //console.log(err.data);
                                thongbao.error(err.data.Message);
                                blockUI.stop();
                            }
                        );
                    },
                    function errorCallback() {
                        blockUI.stop();
                    }
                );
            }

            $ctrl.CapNhatVanBan = function () {
                if ($ctrl.taoGiup == true && $ctrl.para.FileDinhKem.length == 0) {
                    thongbao.error("Đơn nghỉ phép/Đơn giải trình tạo giúp cần phải đính kèm file scan đã được ký bởi người xin nghỉ phép");
                    return;
                }
                if ($ctrl.para.DanhSachNguoiKy.length > maxNguoiDuyet || $ctrl.para.DanhSachNguoiKy.length == 0) {
                    thongbao.error("Đơn nghỉ phép/Đơn giải trình cần chọn tối đa " + maxNguoiDuyet + " và tối thiểu 1 người ký duyệt");
                    return;
                }
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Đồng ý cập nhật thông tin đơn nghỉ phép ?';
                        }
                    }
                }).then(
                    function successCallback() {
                        blockUI.start({ message: "Đơn nghỉ phép/Đơn giải trình đang cập nhật, xin vui lòng chờ" });
                        $ctrl.para.ngayTao = $ctrl.NgayTao == null ? today.toDateString() : $ctrl.NgayTao.toDateString();
                        $ctrl.para.ngayCapCccd = $ctrl.NgayCap == null ? today.toDateString() : $ctrl.NgayCap.toDateString();
                        $ctrl.para.tuNgay = $ctrl.NgayNghiStart == null ? today.toDateString() : $ctrl.NgayNghiStart.toDateString();
                        $ctrl.para.denNgay = $ctrl.NgayNghiEnd == null ? today.toDateString() : $ctrl.NgayNghiEnd.toDateString();
                        $ctrl.para.loai = parseInt($ctrl.loai);
                        $ctrl.para.accessToken = $ctrl.accessToken;
                        //console.log($ctrl.para);
                        var resp = loginservice.postdata(apiUpdateUrl, $.param($ctrl.para));
                        resp.then(
                            function successCallback(response) {
                                //console.log(response.data);
                                //console.log($ctrl.para);

                                if (createPdfTool == 0) {
                                    LoadReport($ctrl.para);
                                }
                                else {
                                    blockUI.stop();
                                    thongbao.success("Cập nhật thành công");
                                    $uibModalInstance.close($ctrl.para);
                                }

                                //GetChiTietVB_LoadReport($ctrl.para);
                            },
                            function errorCallback(err) {
                                thongbao.error(err.data.Message);
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