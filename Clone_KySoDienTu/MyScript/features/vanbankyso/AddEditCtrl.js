angular
    .module("aims")
    .controller('themSuaVBKySoCtrl', [
        "$scope",
        "$uibModalInstance",
        "blockUI",
        "appSettings",
        "loginservice",
        "userProfile",
        "idselect",
        "thongbao",
        "ModalService",
        "FileUploader",
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
            FileUploader) {
            var $ctrl = this;

            var today = new Date();

            var authHeaders = {};

            var dsnhanvientemp = [];

            var dsnhanvientemp2 = [];

            var timeoutId = null;

            $ctrl.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $ctrl.Userdata = userProfile.getProfile();

            Init();

            function Init() {
                $ctrl.valdate1 = today;
                $ctrl.valdate2 = today;
                $ctrl.dsLoaiVB = [];
                $ctrl.dsNhomPhongBan = [];
                $ctrl.dsUserPhongBan = [];
                $ctrl.NguoiTheoDoi = [];
                $ctrl.loaiVB = {};
                $ctrl.para = {};
                $ctrl.para.FileCanCu = [];
                $ctrl.para.FileDinhKem = [];
                $ctrl.para.DanhSachNguoiKy = [];
                $ctrl.para.XemThuTruocFile = 0;
                $ctrl.TypeFile = [];
                KhoiTaoSoanThaoTini();
                KhoiTaoDatePicker();
                Loadtypefile();
                GetBoPhanNhan();
                if (idselect.IDVanBan == null) {
                    $ctrl.TieuDeForm = 0;
                    LoadLoaiVB();
                }
                else {
                    $ctrl.TieuDeForm = 1;
                }
            }

            function LoadLoaiVB() {
                var resp = loginservice.getdata("api/getCore/getdanhmuchethong?loai=TypeRP_NP");
                resp.then(function (response) {
                    $ctrl.dsLoaiVBKhongMau = response.data;
                    var resp = loginservice.getdata("api/getCore/getdanhmuchethong?loai=TypeRP");
                    resp.then(function (response) {
                        $ctrl.dsLoaiVBCoMau = response.data;
                        $ctrl.dsLoaiVB = $ctrl.dsLoaiVBKhongMau.concat($ctrl.dsLoaiVBCoMau);
                        $ctrl.loaiVB = $ctrl.dsLoaiVB[0];
                        $ctrl.para.IsCRForm = 0;
                        $ctrl.para.Loai = parseInt($ctrl.loaiVB.CODE);
                        $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=12&id=" + $ctrl.loaiVB.CODE;
                    }
                        , function errorCallback(response) {
                        });
                }
                    , function errorCallback(response) {
                    });
            }

            function LoadLoaiVBUpdate(objVB) {
                var resp = loginservice.getdata("api/getCore/getdanhmuchethong?loai=TypeRP_NP");
                resp.then(function (response) {
                    $ctrl.dsLoaiVBKhongMau = response.data;
                    var resp = loginservice.getdata("api/getCore/getdanhmuchethong?loai=TypeRP");
                    resp.then(function (response) {
                        $ctrl.dsLoaiVBCoMau = response.data;
                        $ctrl.dsLoaiVB = $ctrl.dsLoaiVBKhongMau.concat($ctrl.dsLoaiVBCoMau);
                        let valuename = objVB.IsCRForm == 0 ? 'TypeRP' : 'TypeRP_NP';
                        let index = $ctrl.dsLoaiVB.findIndex(x => x.VALUENAMECODE == valuename && parseInt(x.CODE) == objVB.Loai);
                        if (index != -1) {
                            $ctrl.loaiVB = $ctrl.dsLoaiVB[index];
                        }
                        else {
                            $ctrl.loaiVB = $ctrl.dsLoaiVB[0];
                        }
                    }
                        , function errorCallback(response) {
                        });

                }
                    , function errorCallback(response) {
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

            function KhoiTaoDatePicker() {
                $ctrl.Ngay01 = {
                    opened: false
                };
                $ctrl.Ngay02 = {
                    opened: false
                };
            }

            function GetBoPhanNhan() {
                blockUI.start();
                var resp = loginservice.postdata("api/QLVanBan/getUsersNhanVBTheoPhongBan", $.param({ valint1: -1 }));
                resp.then(function (response) {
                    blockUI.stop();
                    response.data.forEach(function (value, key) {
                        $ctrl.dsNhomPhongBan.push(value.group);
                        value.listusers.forEach(function (val, idx) {
                            $ctrl.dsUserPhongBan.push(val);
                        });
                    });
                    if (idselect.IDVanBan != null) {
                        GetChiTietVB();
                    }
                },
                    function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            function GetChiTietVB() {
                let resp = loginservice.postdata("api/QLBaoCao/GetVanBanChiTiet2", $.param({ valint1: idselect.IDVanBan, valint3: idselect.Module }));
                resp.then(function (response) {
                    $ctrl.para = response.data;
                    LayThongTinVBCapNhat();
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            function LayThongTinVBCapNhat() {
                $ctrl.valdate1 = $ctrl.para.valdate1 == null ? null : new Date($ctrl.para.valdate1);
                $ctrl.valdate2 = $ctrl.para.valdate2 == null ? null : new Date($ctrl.para.valdate2);
                if ($ctrl.para.NhomPhanCong.length > 0 || ($ctrl.para.NhomPhanCong.length == 0 && $ctrl.para.UserPhanCong.length > 0)) {
                    for (let i = 0; i < $ctrl.para.NhomPhanCong.length; i++) {
                        let index = $ctrl.dsNhomPhongBan.findIndex(x => x.GroupId == $ctrl.para.NhomPhanCong[i].GroupId);
                        if (index != -1) {
                            $ctrl.dsNhomPhongBan.splice(index, 1);
                            dsnhanvientemp.push($ctrl.para.NhomPhanCong[i]);
                            if ($ctrl.para.NhomPhanCong[i].LOAIXULY == 2) {
                                $ctrl.NguoiTheoDoi.push($ctrl.para.NhomPhanCong[i]);
                            }
                        }
                    }
                    for (let i = 0; i < $ctrl.para.UserPhanCong.length; i++) {
                        $ctrl.para.UserPhanCong[i].checked = $ctrl.para.UserPhanCong[i].isChecked == 0 ? false : true;
                        if ($ctrl.para.UserPhanCong[i].isChecked == 1) {
                            dsnhanvientemp2.push($ctrl.para.UserPhanCong[i]);
                            if ($ctrl.para.UserPhanCong[i].LOAIXULY == 2) {
                                $ctrl.NguoiTheoDoi.push($ctrl.para.UserPhanCong[i]);
                            }
                        }
                    }
                }
                if ($ctrl.para.FileDinhKem.length > 0) {
                    if ($ctrl.para.IsCRForm == 0) {
                        xemFile($ctrl.para.FileDinhKem[0].ID);
                    }
                    else {
                        $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=12&id=" + $ctrl.para.Loai;
                    }
                }
                else if ($ctrl.para.FileDinhKem.length == 0) {
                    if ($ctrl.para.IsCRForm == 0) {
                        $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=3&id=-1";
                    }
                    else {
                        $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=12&id=" + $ctrl.para.Loai;
                    }
                }
                LoadLoaiVBUpdate($ctrl.para);
            }

            function Loadtypefile() {
                var resp = loginservice.getdata("api/getCore/getdanhmuchethong?loai=TypeFile");
                resp.then(function (response) {
                    $ctrl.TypeFile = response.data;
                }
                    , function errorCallback(response) {
                    });
            }

            if ($ctrl.Userdata.access_token) {
                authHeaders.Authorization = 'Bearer ' + $ctrl.Userdata.access_token;
            }

            var uploader = $ctrl.uploader = new FileUploader({
                headers: { "Authorization": authHeaders.Authorization },
                url: appSettings.serverPath + 'api/fileUpload/UploadFilesSmartCA',
                withCredentials: true
            });

            uploader.filters.push({
                name: 'docFilter1',
                fn: function (item /*{File|FileLikeObject}*/, options) {
                    var type = '|' + item.name.slice(item.name.lastIndexOf('.') + 1) + '|';
                    if ($ctrl.TypeFile.find(x => x.CODE == 2).VALUENAME.indexOf(type.toLowerCase()) !== -1)
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
                if (response != null) {
                    let item = {
                        MOTA: fileItem.file.name,
                        LOAIFILE: fileItem.file.name.substr(fileItem.file.name.lastIndexOf('.') + 1),
                        SIZEFILE: fileItem.file.size,
                        BASE64DATA: response,
                        IsCRFile: 0
                    };
                    if (idselect.IDVanBan == null) {
                        $ctrl.para.FileDinhKem.push(item);
                        if ($ctrl.para.IsCRForm == 0) {
                            xemFileUploadTemp(item);
                        }
                    }
                    else {
                        addFile(item);
                    }
                }
                else {
                    thongbao.error("Có lỗi xảy ra, xin vui lòng tải lại trang");
                }
            };

            $ctrl.XemFile = function (item) {
                if (idselect.IDVanBan == null) {
                    if ($ctrl.para.IsCRForm == 0) {
                        xemFileUploadTemp(item);
                    }
                    else {
                        let a = document.createElement('a');
                        a.target = "_blank";
                        a.href = appSettings.serverPath + '/Uploadtemp/' + $ctrl.Userdata.username + '/' + item.MOTA;
                        a.click();
                    }
                }
                else {
                    if ($ctrl.para.IsCRForm == 0) {
                        xemFile(item.ID);
                    }
                    else {
                        let result = item.NGAYTAO.split("T")[0].split("-");
                        let a = document.createElement('a');
                        a.target = "_blank";
                        a.href = appSettings.serverPath + '/Report/ReportFile/' + result[0] + '/' + result[1] + '/' + item.TENFILE;
                        a.click();
                    }
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

            $ctrl.XemFileCanCu = function (item) {
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

            $ctrl.XoaFileCanCu = function (id, idx) {
                if (idselect.IDVanBan == null) {
                    $ctrl.para.FileCanCu.splice(idx, 1);
                }
                else {
                    xoaFileCanCu(id, idx);
                }
            }

            function xemFileUploadTemp(item) {
                $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=0&id=" + item.MOTA;
            }

            function xoaFileUploadTemp(fileName, index) {
                var resp = loginservice.postdata("api/fileUpload/removefiletemp", $.param({ valstring1: fileName }));
                resp.then(function (response) {
                    $ctrl.para.FileDinhKem.splice(index, 1);
                    if ($ctrl.para.FileDinhKem.length == 0) {
                        if ($ctrl.para.IsCRForm == 0) {
                            $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=11&id=-1";
                        }
                    }
                    else {
                        if ($ctrl.para.IsCRForm == 0) {
                            xemFileUploadTemp($ctrl.para.FileDinhKem[0]);
                        }
                    }
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            function xemFile(id) {
                $scope.fileid = id;
                $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?id=" + id + "&type=11";
            }

            function xoaFile(id) {
                blockUI.start();
                var resp = loginservice.postdata("api/QLBaoCao/RemoveFile_Update", $.param({ valint1: id }));
                resp.then(function (response) {
                    //alert(response.data);
                    blockUI.stop();
                    let index = $ctrl.para.FileDinhKem.findIndex(x => x.ID == id);
                    $ctrl.para.FileDinhKem.splice(index, 1);
                    if ($ctrl.para.FileDinhKem.length == 0) {
                        if ($ctrl.para.IsCRForm == 0) {
                            $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=3&id=-1";
                        }
                    }
                    else {
                        if ($ctrl.para.IsCRForm == 0) {
                            xemFile($ctrl.para.FileDinhKem[0].ID);
                        }
                    }
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            function xoaFileCanCu(id, idx) {
                blockUI.start();
                var resp = loginservice.postdata("api/QLBaoCao/RemoveFileCanCu", $.param({ valint1: id }));
                resp.then(function (response) {
                    blockUI.stop();
                    $ctrl.para.FileCanCu.splice(idx, 1);
                    if ($ctrl.para.FileCanCu.length == 0) {
                        if ($ctrl.para.FileDinhKem.length > 0) {
                            xemFile($ctrl.para.FileDinhKem[0].ID);
                        }
                        else {
                            $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=3&id=-1";
                        }
                    }
                    else {
                        $scope.fileid = item.IDFile;
                        $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?id=" + $ctrl.para.FileCanCu[0].IDFile + "&type=" + $ctrl.para.FileCanCu[0].NhomId;
                    }
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            function addFile(item) {
                blockUI.start();
                var resp = loginservice.postdata("api/QLBaoCao/UploadFile_Update", $.param({ valstring1: item.MOTA, valstring2: item.LOAIFILE, valint1: $ctrl.para.ID, valint2: item.SIZEFILE }));
                resp.then(function (response) {
                    blockUI.stop();
                    let nht = new Date();
                    item.ID = response.data.valint1;
                    item.IsCRFile = 0;
                    item.TENFILE = response.data.valstring1;
                    item.NGAYTAO = nht.getFullYear() + "-" + ((nht.getMonth() + 1) >= 10 ? (nht.getMonth() + 1) : "0" + (nht.getMonth() + 1)) + "-" + nht.getDate() + "T00:00:00";
                    $ctrl.para.FileDinhKem.push(item);
                    if ($ctrl.para.IsCRForm == 0) {
                        xemFile(response.data.valint1);
                    }
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            function xoaNhomPhanCong(GroupId) {
                let resp = loginservice.postdata("api/QLBaoCao/XoaNhomPhanCong", $.param({ valint1: idselect.IDVanBan, valint2: idselect.LoaiVB, valint3: GroupId }));
                resp.then(function (response) {
                    thongbao.success("Xóa thành công");
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            function themNhomPhanCong(item) {
                let resp = loginservice.postdata("api/QLBaoCao/ThemNhomPhanCong", $.param({ valint1: idselect.IDVanBan, valint2: idselect.LoaiVB, valint3: item.GroupId, valint4: item.LOAIXULY, valstring1: item.FullName }));
                resp.then(function (response) {
                    thongbao.success("Thêm thành công");
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            function xoaCaNhan(item) {
                let resp = loginservice.postdata("api/QLBaoCao/XoaCaNhan", $.param({ valint1: idselect.IDVanBan, valint2: idselect.LoaiVB, valstring1: item.UserName }));
                resp.then(function (response) {
                    thongbao.success("Xóa thành công");
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            function themCaNhan(item) {
                let resp = loginservice.postdata("api/QLBaoCao/ThemCaNhan", $.param({ valint1: idselect.IDVanBan, valint2: idselect.LoaiVB, valint3: item.GroupId, valint4: item.LOAIXULY, valint5: 0, valstring1: item.UserName }));
                resp.then(function (response) {
                    item.UserIDVB = response.data;
                    item.NGUOITAO = $ctrl.Userdata.username;
                    thongbao.success("Thêm thành công");
                }
                    , function errorCallback(response) {
                        blockUI.stop();
                    });
            }

            $ctrl.DoiLoaiVB = function (item) {
                if (item.VALUENAMECODE == 'TypeRP_NP') { // vb crystal report
                    $ctrl.para.IsCRForm = 1;
                    $ctrl.para.Loai = parseInt(item.CODE);
                    $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=12&id=" + item.CODE;
                }
                else { // vb thường
                    $ctrl.para.IsCRForm = 0;
                    $ctrl.para.Loai = parseInt(item.CODE);
                    if ($ctrl.para.FileDinhKem.length > 0) {
                        if (idselect.IDVanBan == null) {
                            xemFileUploadTemp($ctrl.para.FileDinhKem[0]);
                        }
                        else {
                            xemFile($ctrl.para.FileDinhKem[0].ID);
                        }
                    }
                    else {
                        $ctrl.urldoc = appSettings.serverPath + "Scripts/pdf.js-viewer2/web/viewer.html?type=12&id=" + item.CODE;
                    }
                }
            }

            $ctrl.xoaNguoiKy = function (username) {
                let index = $ctrl.para.DanhSachNguoiKy.findIndex(x => x.UserName == username);
                if (index != -1) {
                    $ctrl.para.DanhSachNguoiKy.splice(index, 1);
                }
            }

            $ctrl.openNgay01 = function () {
                $ctrl.Ngay01.opened = true;
            };

            $ctrl.openNgay02 = function () {
                $ctrl.Ngay02.opened = true;
            };

            $ctrl.moFormChonNguoiKy = function () {
                ModalService.open({
                    templateUrl: 'modalChonNguoiKy.html',
                    controller: 'chonNguoiKyVBCtrl',
                    size: 'md',
                    resolve: {
                        idselect: function () {
                            let obj = {};
                            obj.dsNguoiKy = $ctrl.para.DanhSachNguoiKy;
                            return obj;
                        }
                    }
                }).then(function (c) {
                    $ctrl.para.DanhSachNguoiKy.push(c);
                }, function () {
                    blockUI.stop();
                });
            }

            $ctrl.moFormChonFileCanCu = function () {
                ModalService.open({
                    templateUrl: 'modalChonFileCanCu.html',
                    controller: 'chonFileCanCuCtrl',
                    size: 'lg80',
                    resolve: {
                        idselect: function () {
                            let item = {};
                            item.IdVB = idselect.IDVanBan;
                            item.FileCanCu = $ctrl.para.FileCanCu;
                            return item;
                        }
                    }
                }).then(function () {
                }, function () {
                });
            }

            $ctrl.addUpdateUser = function (item, loaixl, vaitro) {
                if (item.checked || item.isChecked == 1) { // xử lý cá nhân
                    if (loaixl == 0) { // xóa
                        let index = dsnhanvientemp2.findIndex(x => x.UserName == item.UserName);
                        dsnhanvientemp2.splice(index, 1);
                        if (idselect.IDVanBan != null) {
                            xoaCaNhan(item);
                        }
                    }
                    else { // thêm
                        dsnhanvientemp2.push(item);
                        if (idselect.IDVanBan != null) {
                            themCaNhan(item);
                        }
                    }
                }
                else { // xử lý nhóm
                    if (loaixl == 0) { // xóa
                        $ctrl.dsNhomPhongBan.push(item);
                        let index2 = dsnhanvientemp.findIndex(x => x.GroupId == item.GroupId);
                        dsnhanvientemp.splice(index2, 1);
                        if (idselect.IDVanBan != null) {
                            xoaNhomPhanCong(item.GroupId);
                        }
                    }
                    else { // thêm
                        let index = $ctrl.dsNhomPhongBan.findIndex(x => x.GroupId == item.GroupId);
                        $ctrl.dsNhomPhongBan.splice(index, 1);
                        item.LOAIXULY = vaitro;
                        dsnhanvientemp.push(item);
                        if (idselect.IDVanBan != null) {
                            themNhomPhanCong(item);
                        }
                    }
                }
            }

            $ctrl.openUserSelect = function (loaixl) {
                ModalService.open({
                    templateUrl: 'modalPhanPhatTree.html',
                    controller: 'phanphatTreeCtrl',
                    size: 'lg50',
                    resolve: {
                        idselect: function () {
                            let obj = {};
                            obj.vaitro = loaixl;
                            obj.typename = $ctrl.para.TypeName;
                            obj.listuser = Array.prototype.map.call(dsnhanvientemp2, s => s.UserName).toString();
                            obj.listtrangthai = [{ li: true, div: true, seeli: true, Ten: "Nhóm Quản Lý", IsView: 1 }, { li: false, div: false, seeli: true, Ten: "Nhóm Phòng Ban", IsView: 0 }];
                            return obj;
                        }
                    }
                }).then(function (c) {
                    switch (loaixl) {
                        case 2:
                            angular.forEach(c, function (i) {
                                $ctrl.NguoiTheoDoi.push(i);
                                $ctrl.addUpdateUser(i, 1, loaixl);
                            });
                            break;
                    }
                }, function () {
                });
            }

            $ctrl.SearchFileCanCu = function (searchText) { // kỹ thuật debounce , lưu lại để tham khảo, cốt lõi là tạo ra timeOutId r xóa rồi set,dùng cho delay 1 khoảng thời gian mới load
                $ctrl.rst = [];
                if (timeoutId) {
                    clearTimeout(timeoutId);
                }
                timeoutId = setTimeout(async () => {
                    console.log('set timeout');
                    console.log(searchText);
                    var accesstoken = userProfile.getProfile().access_token;
                    var authHeaders = {};
                    if (accesstoken) {
                        authHeaders.Authorization = 'Bearer ' + accesstoken;
                    }
                    const response = await fetch(
                        appSettings.serverPath + `api/QLBaoCao/GetFilesCanCu?searchText=${searchText}`,
                        {
                            method: "GET",
                            headers:
                            {
                                'Authorization': authHeaders.Authorization
                            },
                        }
                    );
                    $ctrl.dsFileCanCu = await response.json();
                    console.log($ctrl.dsFileCanCu);
                }, 1000);
            }

            $ctrl.LuuNhap = function () {
                if ($ctrl.para.FileDinhKem.length == 0 || !$ctrl.para.FileDinhKem) {
                    thongbao.error("Văn bản chưa được đính kèm File");
                    return;
                }
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Đồng ý thêm văn bản ký số ?';
                        }
                    }
                }).then(function () {
                    blockUI.start();
                    $ctrl.para.TrangThai = 2;
                    $ctrl.para.XemThuTruocFile = 0;
                    $ctrl.para.valdate1 = $ctrl.valdate1 == null ? today.toDateString() : $ctrl.valdate1.toDateString();
                    $ctrl.para.valdate2 = $ctrl.valdate2 == null ? today.toDateString() : $ctrl.valdate2.toDateString();
                    $ctrl.para.NhomPhanCong = dsnhanvientemp;
                    $ctrl.para.UserPhanCong = dsnhanvientemp2;
                    //console.log($ctrl.para);
                    var resp = loginservice.postdata("api/QLBaoCao/ThemReport", $.param($ctrl.para));
                    resp.then(
                        function successCallback(response) {
                            blockUI.stop();
                            if ($ctrl.para.IsCRForm == 0) {
                                thongbao.success("Tạo văn bản ký số thành công");
                                $uibModalInstance.close(response.data);
                            }
                        },
                        function errorCallback(response) {
                            thongbao.error("Có lỗi khi tạo văn bản ký số, vui lòng thử lại sau");
                            blockUI.stop();
                        }
                    );
                }, function () {
                    blockUI.stop();
                });
            }

            $ctrl.CapNhatVanBan = function () {
                if ($ctrl.para.DanhSachNguoiKy.length == 0) {
                    thongbao.error("Xin vui lòng chọn người ký");
                    return;
                }
                ModalService.open({
                    templateUrl: 'ConfirmModal.html',
                    controller: 'modalConfirmInstanceCtrl',
                    size: 'md',
                    resolve: {
                        confirmMessage: function () {
                            return 'Đồng ý cập nhật thông tin văn bản ?';
                        }
                    }
                }).then(function () {
                    blockUI.start();
                    $ctrl.para.valdate1 = $ctrl.valdate1 == null ? today.toDateString() : $ctrl.valdate1.toDateString();
                    $ctrl.para.valdate2 = $ctrl.valdate2 == null ? today.toDateString() : $ctrl.valdate2.toDateString();
                    var resp = loginservice.postdata("api/QLBaoCao/CapNhatThongTinVanBan2", $.param($ctrl.para));
                    resp.then(
                        function successCallback(response) {
                            blockUI.stop();
                            if ($ctrl.para.IsCRForm == 0) {
                                thongbao.success("Cập nhật văn bản ký số thành công");
                                $uibModalInstance.close(response.data);
                            }
                        },
                        function errorCallback(response) {
                            blockUI.stop();
                        }
                    );
                }, function () {
                });
            }
        }
    ]);                                                        