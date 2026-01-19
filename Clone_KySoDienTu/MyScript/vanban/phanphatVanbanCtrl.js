(function () {
    "use strict";
    angular.module("oamsapp")
        .controller('phanphatTreeCtrl', [
            'thongbao',
            "$scope",
            'blockUI',
            "$uibModalInstance",
            "loginservice",
            "idselect",
            function (
                thongbao,
                $scope,
                blockUI,
                $uibModalInstance,
                loginservice,
                idselect)
            {
                var $ctrl = this;

                $ctrl.VuLe = idselect;

                //console.log($ctrl.VuLe);

                $scope.$on('closeModal', function (data, mess) {
                    thongbao.success(mess);
                    $ctrl.cancel();
                });

                let dsnhanvientemp = [];

                $ctrl.CheckAll = function (item) {
                    item.checked = !item.checked;
                    if (item.checked) {
                        document.getElementsByName('userselect' + item.GroupId).forEach(function (i) {
                            i.checked = true;
                        });
                        item.Users.forEach(function (item) {
                            item.checked = true;
                            item.LOAIXULY = $ctrl.VuLe.vaitro;
                            dsnhanvientemp.push(item);
                        });
                    }
                    else {
                        document.getElementsByName('userselect' + item.GroupId).forEach(function (i) {
                            i.checked = false;
                        });
                        item.Users.forEach(function (item) {
                            item.checked = false;
                            let index = dsnhanvientemp.findIndex(x => x.UserID == item.UserID);
                            dsnhanvientemp.splice(index, 1);
                        });
                    }
                };

                $ctrl.Check = function (child) {
                    child.checked = !child.checked;
                    if (child.checked) {
                        child.LOAIXULY = $ctrl.VuLe.vaitro;
                        dsnhanvientemp.push(child);
                    }
                    else {
                        let index = dsnhanvientemp.findIndex(x => x.UserID == child.GroupId);
                        dsnhanvientemp.splice(index, 1);
                    }
                };

                $ctrl.myClick = function (i) {
                    var hideThis = document.getElementById(i);
                    var showHide = angular.element(hideThis).attr('class');
                    if (showHide === 'ui-icon ace-icon fa fa-minus center bigger-110 blue') {
                        myStyles.addRule('tr[name="' + i + '"]', '{display: none}');
                        angular.element(hideThis).attr('class', 'ui-icon ace-icon fa fa-plus center bigger-110 blue');
                    }
                    else {
                        myStyles.deleteRule('tr[name="' + i + '"]');
                        angular.element(hideThis).attr('class', 'ui-icon ace-icon fa fa-minus center bigger-110 blue');
                    }
                };

                loadUserTheoPhongBan();

                function loadUserTheoPhongBan() {
                    blockUI.start();
                    var resp = loginservice.postdata("api/getUser/getPhongBanUser", $.param({ valstring1: $ctrl.VuLe.listuser }));
                    resp.then(function (response) {
                        $ctrl.phongban = response.data;
                        //console.log($ctrl.phongban);
                        $ctrl.phongban.forEach(function (value, key) {
                            myStyles.addRule('tr[name="' + value.GroupId + '"]', '{display: none}');
                            value.checked = false;
                        });
                        blockUI.stop();
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }

                var myStyles = (function () {
                    var sheet = document.styleSheets[0];
                    function deleteRule(selector) {
                        var rules = sheet.rules || sheet.cssRules;
                        for (var i = 0; i < rules.length; i++) {
                            if (selector == rules[i].selectorText) {
                                sheet.deleteRule(i);
                            }
                        }
                    }

                    function addRule(selector, text) {
                        deleteRule(selector);
                        sheet.insertRule(selector + text);
                    }
                    return {
                        'addRule': addRule,
                        'deleteRule': deleteRule
                    };
                }());

                $ctrl.ChangeGroup = function (IsView) {
                    dsnhanvientemp = [];
                    getAll(IsView);
                }

                $ctrl.LuuNhom = function () {
                    $uibModalInstance.close(dsnhanvientemp);
                }

                $ctrl.cancel = function () {
                    $uibModalInstance.dismiss('cancel');
                };
            }])
        .controller('phanphatNhomCtrl', [
            "$scope",
            "$uibModalInstance",
            "blockUI",
            "loginservice",
            "idselect",
            "thongbao",
            "ModalService",
            "userProfile",
            function (
                $scope,
                $uibModalInstance,
                blockUI,
                loginservice,
                idselect,
                thongbao,
                ModalService,
                userProfile)
            {
                var $ctrl = this;

                let dsnhanvientemp = [];

                $scope.$on('closeModal', function (data, mess) {
                    thongbao.success(mess);
                    $ctrl.cancel();
                });

                $scope.$on('countThongBaoVB', function (data, mess) {
                    dsnhanvientemp = [];
                    loadphongban();
                });

                $ctrl.Userdata = userProfile.getProfile();

                $ctrl.cancel = function () {
                    $uibModalInstance.close('cancel');
                };

                $ctrl.quyenuser = {};

                getQuyenTong();

                loadphongban();

                function getQuyenTong() {
                    var resp = loginservice.postdata("api/getCore/getAllPermissionsOfUserByModuleResource", $.param({ valstring1: "WF", valstring2: 'G', valstring3: 2 }));
                    resp.then(function (response) {
                        $ctrl.quyenuser.PhanPhatToanVB = response.data.findIndex(x => x.PermissionAction == 'SHDRF') > -1;
                    }
                        , function errorCallback(response) {
                        });
                }

                function loadphongban() {
                    $ctrl.vaitrocuaban = {};
                    $ctrl.theodoi = [];
                    $ctrl.xuly = [];
                    $ctrl.phoihop = [];
                    $ctrl.listld4 = [];
                    blockUI.start();
                    var resp = loginservice.postdata("api/QLVanBan/getUsersNhanVBTheoPhongBan", $.param({ valint1: -1 }));
                    resp.then(function (response) {
                        blockUI.stop();
                        response.data.forEach(function (value, key) {
                            $ctrl.listld4.push(value.group);
                        });
                        //console.log($ctrl.listld4);
                        $ctrl.quyenuser.TD = idselect.TD;
                        $ctrl.quyenuser.XLC = idselect.XLC;
                        $ctrl.quyenuser.PH = idselect.PH;
                        $ctrl.quyenuser.BP = idselect.BP;
                        $ctrl.vaitrocuaban.UserName = $ctrl.Userdata.username;
                        $ctrl.vaitrocuaban.LOAIXULY = "";
                        if ($ctrl.quyenuser.BP) {
                            $ctrl.vaitrocuaban.LOAIXULY += "Người bút phê-";
                        }
                        if ($ctrl.quyenuser.PH) {
                            $ctrl.vaitrocuaban.LOAIXULY += "Phối hợp-";
                        }
                        if ($ctrl.quyenuser.TD) {
                            $ctrl.vaitrocuaban.LOAIXULY += "Theo dõi-";
                        }
                        if ($ctrl.quyenuser.XLC) {
                            $ctrl.vaitrocuaban.LOAIXULY += "Xử lý chính-";
                        }
                        getNhomThamGiaVB();
                        getUserThamGiaVB();
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }

                $ctrl.addUpdateUser = function (item, loaixl, vaitro) {
                    if (item.checked || item.isChecked == 1) { // xử lý cá nhân
                        if (loaixl == 0) { // xóa
                            let index = dsnhanvientemp.findIndex(x => x.UserName == item.UserName);
                            dsnhanvientemp.splice(index, 1);
                            xoaCaNhan(item);
                        }
                        else { // thêm
                            dsnhanvientemp.push(item);
                            themCaNhan(item);
                        }
                    }
                    else { // xử lý nhóm
                        if (loaixl == 0) { // xóa
                            $ctrl.listld4.push(item);
                            xoaNhomPhanCong(item.GroupId);
                        }
                        else { // thêm
                            let index = $ctrl.listld4.findIndex(x => x.GroupId == item.GroupId);
                            $ctrl.listld4.splice(index, 1);
                            item.LOAIXULY = vaitro;
                            themNhomPhanCong(item);
                        }
                    }
                }

                $ctrl.openUserSelect = function (loaixl) {
                    ModalService.open({
                        templateUrl: 'modalPhanPhatTree.html',
                        controller: 'phanphatTreeCtrl',
                        resolve: {
                            idselect: function () {
                                let obj = {};
                                obj.vaitro = loaixl;
                                obj.typename = idselect.TypeName;
                                obj.listuser = Array.prototype.map.call(dsnhanvientemp, s => s.UserName).toString();
                                obj.listtrangthai = [{ li: true, div: true, seeli: true, Ten: "Nhóm Quản Lý", IsView: 1 }, { li: false, div: false, seeli: true, Ten: "Nhóm Phòng Ban", IsView: 0 }];
                                return obj;
                            }
                        }
                    }).then(function (c) {
                        switch (loaixl) {
                            case 2:
                                angular.forEach(c, function (i) {
                                    $ctrl.theodoi.push(i);
                                    $ctrl.addUpdateUser(i, 1, loaixl);
                                });
                                break;

                            case 3:
                                angular.forEach(c, function (i) {
                                    $ctrl.xuly.push(i);
                                    $ctrl.addUpdateUser(i, 1, loaixl);
                                });
                                break;

                            case 4:
                                angular.forEach(c, function (i) {
                                    $ctrl.phoihop.push(i);
                                    $ctrl.addUpdateUser(i, 1, loaixl);
                                });
                                break;
                        }
                    });
                }

                function getNhomThamGiaVB() {
                    let respp = loginservice.postdata("api/QLVanBan/getNhomThamGiaVanBan", $.param({ valint1: idselect.IDVANBAN, valint2: idselect.LoaiVB }));
                    respp.then(function (response) {
                        //console.log(response.data);
                        blockUI.stop();
                        for (let i = 0; i < response.data.length; i++) {
                            let index = $ctrl.listld4.findIndex(x => x.GroupId == response.data[i].GroupId);
                            $ctrl.listld4.splice(index, 1);
                            response.data[i].checked = response.data[i].isChecked == 0 ? false : true;
                            if (response.data[i].LOAIXULY == 2) {
                                $ctrl.theodoi.push(response.data[i]);
                            }
                            else if (response.data[i].LOAIXULY == 3) {
                                $ctrl.xuly.push(response.data[i]);
                            }
                            else if (response.data[i].LOAIXULY == 4) {
                                $ctrl.phoihop.push(response.data[i]);
                            }
                        }
                    }
                        , function errorCallback(response) {
                            $ctrl.theodoi = [];
                            $ctrl.xuly = [];
                            $ctrl.phoihop = [];
                        });
                }

                function getUserThamGiaVB() {
                    let respp = loginservice.postdata("api/QLVanBan/getUserThamGiaVanBan", $.param({ valint1: idselect.IDVANBAN, valint2: idselect.LoaiVB }));
                    respp.then(function (response) {
                        blockUI.stop();
                        //console.log(response.data);
                        for (let i = 0; i < response.data.length; i++) {
                            response.data[i].checked = response.data[i].isChecked == 0 ? false : true;
                            dsnhanvientemp.push(response.data[i]);
                            if (response.data[i].LOAIXULY == 2) {
                                $ctrl.theodoi.push(response.data[i]);
                            }
                            else if (response.data[i].LOAIXULY == 3) {
                                $ctrl.xuly.push(response.data[i]);
                            }
                            else if (response.data[i].LOAIXULY == 4) {
                                $ctrl.phoihop.push(response.data[i]);
                            }
                        }
                    }
                        , function errorCallback(response) {
                            $ctrl.theodoi = [];
                            $ctrl.xuly = [];
                            $ctrl.phoihop = [];
                        });
                }

                function xoaNhomPhanCong(GroupId) {
                    let resp = loginservice.postdata("api/QLVanBan/XoaNhomPhanCong", $.param({ valint1: idselect.IDVANBAN, valint2: idselect.LoaiVB, valint3: GroupId }));
                    resp.then(function (response) {
                        thongbao.success("Xóa thành công");
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }

                function themNhomPhanCong(item) {
                    let resp = loginservice.postdata("api/QLVanBan/ThemNhomPhanCong", $.param({ valint1: idselect.IDVANBAN, valint2: idselect.LoaiVB, valint3: item.GroupId, valint4: item.LOAIXULY, valstring1: item.FullName }));
                    resp.then(function (response) {
                        thongbao.success("Thêm thành công");
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }

                function xoaCaNhan(item) {
                    let resp = loginservice.postdata("api/QLVanBan/XoaCaNhan", $.param({ valint1: idselect.IDVANBAN, valint2: idselect.LoaiVB, valstring1: item.UserName }));
                    resp.then(function (response) {
                        thongbao.success("Xóa thành công");
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }

                function themCaNhan(item) {
                    let resp = loginservice.postdata("api/QLVanBan/ThemCaNhan", $.param({ valint1: idselect.IDVANBAN, valint2: idselect.LoaiVB, valint3: item.GroupId, valint4: item.LOAIXULY, valint5: 0, valstring1: item.UserName }));
                    resp.then(function (response) {
                        item.UserIDVB = response.data;
                        item.NGUOITAO = $ctrl.Userdata.username;
                        thongbao.success("Thêm thành công");
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }
            }])
        .controller('phanphatUserCtrl', [
            "$scope",
            "$uibModalInstance",
            "blockUI",
            "loginservice",
            "idselect",
            "thongbao",
            "userProfile",
            function (
                $scope,
                $uibModalInstance,
                blockUI,
                loginservice,
                idselect,
                thongbao,
                userProfile)
            {
                var $ctrl = this;

                $ctrl.quyenuser = {};

                $scope.$on('closeModal', function (data, mess) {
                    thongbao.success(mess);
                    $ctrl.cancel();
                });

                $scope.$on('countThongBaoVB', function (data, mess) {
                    getUsersThamGiaVB();
                });

                $ctrl.vaitrocuaban = {};

                $ctrl.listld4 = [];

                $ctrl.UserData = userProfile.getProfile();

                //console.log(idselect);

                $ctrl.cancel = function () {
                    $uibModalInstance.close('cancel');
                };

                //getQuyenTong();

                getUserPhanPhat();

                getUsersThamGiaVB();

                function getQuyenTong() {
                    var resp = loginservice.postdata("api/getCore/getAllPermissionsOfUserByModuleResource", $.param({ valstring1: "WF", valstring2: 'G', valstring3: "2" }));
                    resp.then(function (response) {
                        $ctrl.quyenuser.PhanPhatVB = response.data.findIndex(x => x.PermissionAction == 'SHDRF') > -1;
                        //console.log($scope.quyenuser);
                    }
                        , function errorCallback(response) {
                        });
                }

                function getUserPhanPhat() { // lấy danh sách toàn bộ user thuộc các nhóm đang tồn tại trong các group đc phân công
                    blockUI.start();
                    var resp = loginservice.postdata("api/QLVanBan/GetUserGroupId", $.param({ valint1: idselect.IDVANBAN, valint2: idselect.LoaiVB }));
                    resp.then(function (response) {
                        //console.log(response.data);
                        blockUI.stop();
                        $ctrl.listld4 = response.data;
                        $ctrl.quyenuser.TD = idselect.TD;
                        $ctrl.quyenuser.XLC = idselect.XLC;
                        $ctrl.quyenuser.PH = idselect.PH;
                        $ctrl.quyenuser.BP = idselect.BP;
                        $ctrl.vaitrocuaban.UserName = $ctrl.UserData.username;
                        $ctrl.vaitrocuaban.LOAIXULY = "";
                        if ($ctrl.quyenuser.BP) {
                            $ctrl.vaitrocuaban.LOAIXULY += "Người bút phê-";
                        }
                        if ($ctrl.quyenuser.TD) {
                            $ctrl.vaitrocuaban.LOAIXULY += "Theo dõi-";
                        }
                        if ($ctrl.quyenuser.XLC) {
                            $ctrl.vaitrocuaban.LOAIXULY += (idselect.ParentIDCaNhan == 0 && idselect.ParentIDNhom == 0) ? "Xử lý chính-" : "Xử lý-";
                        }
                        if ($ctrl.quyenuser.PH) {
                            $ctrl.vaitrocuaban.LOAIXULY += "Phối hợp-";
                        }
                    }
                        , function errorCallback(response) {
                        });
                }

                function getUsersThamGiaVB() { // Lấy full danh sách user đã tham gia xử lý văn bản
                    let respp = loginservice.postdata("api/QLVanBan/getUserThamGiaVanBan", $.param({ valint1: idselect.IDVANBAN, valstring1: idselect.TypeName, valint2: idselect.LoaiVB == 0 ? 2 : 3 }));
                    respp.then(function (response) {
                        blockUI.stop();
                        $ctrl.listthamgia = response.data;
                        getUsersTheoVaiTroPhanPhat();
                    }
                        , function errorCallback(response) {

                        });
                }

                function getUsersTheoVaiTroPhanPhat() { // xử lý cho ds User vào list vai trò tương ứng + show lên giao diện
                    $ctrl.theodoi = [];
                    $ctrl.xuly = [];
                    $ctrl.phoihop = [];
                    $ctrl.listtheodoi = [];
                    $ctrl.listxuly = [];
                    $ctrl.listphoihop = [];
                    $ctrl.listthamgianhomcuatoi = [];
                    var resp = loginservice.postdata("api/QLVanBan/GetUsersTheoVaiTroNguoiPhanPhat", $.param({ valint1: idselect.IDVANBAN, valint2: idselect.LoaiVB }));
                    resp.then(function (response) {
                        //console.log(response.data);
                        $ctrl.UserGroupIds = response.data;
                        $ctrl.UserGroupIds.forEach(function (i) {
                            $ctrl.listld4.forEach(function (j) {
                                if (j.GroupId == i.GroupId) {
                                    if (i.LOAIXULY == 2) {
                                        $ctrl.listtheodoi.push(j);
                                    }
                                    if (i.LOAIXULY == 3) {
                                        $ctrl.listxuly.push(j);
                                    }
                                    if (i.LOAIXULY == 4) {
                                        $ctrl.listphoihop.push(j);
                                    }
                                }
                            });

                            $ctrl.listthamgia.forEach(function (j) {
                                if (j.GroupId == i.GroupId) {
                                    $ctrl.listthamgianhomcuatoi.push(j);
                                }
                            })
                        })

                        $ctrl.listthamgia.forEach(function (i) {
                            if (i.LOAIXULY == 2) {
                                let index = $ctrl.listtheodoi.findIndex(x => x.UserName == i.UserName);
                                if (index != -1) {
                                    $ctrl.listtheodoi.splice(index, 1);
                                }
                                $ctrl.theodoi.push(i);
                            }
                            if (i.LOAIXULY == 3) {
                                let index = $ctrl.listxuly.findIndex(x => x.UserName == i.UserName);
                                if (index != -1) {
                                    $ctrl.listxuly.splice(index, 1);
                                }
                                $ctrl.xuly.push(i);
                            }
                            if (i.LOAIXULY == 4) {
                                let index = $ctrl.listphoihop.findIndex(x => x.UserName == i.UserName);
                                if (index != -1) {
                                    $ctrl.listphoihop.splice(index, 1);
                                }
                                $ctrl.phoihop.push(i);
                            }
                        })
                    }
                        , function errorCallback(response) {
                        });
                }

                function xoaUser(item) {
                    if (item.isChecked == 0) {
                        let resp = loginservice.postdata("api/QLVanBan/XoaUserPhanCong", $.param({ valint1: item.UserIDVB, valint2: idselect.IDVANBAN, valint3: idselect.LoaiVB, valint4: item.GroupId, valstring1: item.UserName }));
                        resp.then(function (response) {
                            thongbao.success("Xóa thành công");
                        }
                            , function errorCallback(response) {
                                blockUI.stop();
                            });
                    }
                    else {
                        let resp = loginservice.postdata("api/QLVanBan/XoaCaNhan", $.param({ valint1: idselect.IDVANBAN, valint2: idselect.LoaiVB, valstring1: item.UserName }));
                        resp.then(function (response) {
                            thongbao.success("Xóa thành công");
                        }
                            , function errorCallback(response) {
                                blockUI.stop();
                            });
                    }
                }

                function themUser(item, loaiphancong,useridvb) {
                    if (loaiphancong == 0) {
                        let resp = loginservice.postdata("api/QLVanBan/ThemUserPhanCong", $.param({ valint1: idselect.IDVANBAN, valint2: idselect.LoaiVB, valint3: item.GroupId, valint4: item.LOAIXULY, valstring1: item.UserName }));
                        resp.then(function (response) {
                            item.UserIDVB = response.data;
                            item.NGUOITAO = $ctrl.vaitrocuaban.UserName;
                            thongbao.success("Thêm thành công");
                        }
                            , function errorCallback(response) {
                                blockUI.stop();
                            });
                    }
                    else {
                        let resp = loginservice.postdata("api/QLVanBan/ThemCaNhan", $.param({ valint1: idselect.IDVANBAN, valint2: idselect.LoaiVB, valint3: item.GroupId, valint4: item.LOAIXULY, valint5: useridvb, valstring1: item.UserName }));
                        resp.then(function (response) {
                            item.UserIDVB = response.data;
                            item.NGUOITAO = $ctrl.vaitrocuaban.UserName;
                            thongbao.success("Thêm thành công");
                        }
                            , function errorCallback(response) {
                                blockUI.stop();
                            });
                    }
                }

                function xoaList(list, item) {
                    let index = list.findIndex(x => x.UserName == item.username);
                    list.splice(index, 1);
                }

                $ctrl.addUser2 = function (item, loaixl, vaitro) {
                    if (loaixl == 0) { // xóa
                        if (vaitro == 2)
                            $ctrl.listtheodoi.push(item);
                        else if (vaitro == 3)
                            $ctrl.listxuly.push(item);
                        else if (vaitro == 4)
                            $ctrl.listphoihop.push(item);
                        xoaUser(item);
                    }
                    else { // thêm
                        let index = $ctrl.listthamgia.findIndex(x => x.UserName == $ctrl.UserData.username && x.LOAIXULY == vaitro && x.GroupId == item.GroupId);
                        if (index != -1) {
                            $ctrl.loaiphancong = $ctrl.listthamgia[index];
                            if (vaitro == 2)
                                xoaList($ctrl.listtheodoi, item);
                            else if (vaitro == 3)
                                xoaList($ctrl.listxuly, item);
                            else if (vaitro == 4)
                                xoaList($ctrl.listphoihop, item);
                            item.LOAIXULY = vaitro;
                            themUser(item, $ctrl.loaiphancong.isChecked, $ctrl.loaiphancong.UserIDVB);
                        }
                    }
                }
            }])
        .controller('phanPhatVanBanKySoCtrl', [
            "$scope",
            "$uibModalInstance",
            "blockUI",
            "loginservice",
            "idselect",
            "thongbao",
            "ModalService",
            "userProfile",
            function (
                $scope,
                $uibModalInstance,
                blockUI,
                loginservice,
                idselect,
                thongbao,
                ModalService,
                userProfile)
            {
                var $ctrl = this;

                $ctrl.Userdata = userProfile.getProfile();

                GetBoPhanNhan();

                $ctrl.cancel = function () {
                    $uibModalInstance.close('cancel');
                };

                $ctrl.addUpdateUser = function (item, loaixl, vaitro) {
                    if (item.checked || item.isChecked == 1) { // xử lý cá nhân
                        if (loaixl == 0) { // xóa
                            let index = $ctrl.dsnhanvientemp.findIndex(x => x.UserName == item.UserName);
                            $ctrl.dsnhanvientemp.splice(index, 1);
                            xoaCaNhan(item);
                        }
                        else { // thêm
                            $ctrl.dsnhanvientemp.push(item);
                            themCaNhan(item);
                        }
                    }
                    else { // xử lý nhóm
                        if (loaixl == 0) { // xóa
                            $ctrl.NguoiTheoDoi.push(item);
                            xoaNhomPhanCong(item.GroupId);
                        }
                        else { // thêm
                            let index = $ctrl.NguoiTheoDoi.findIndex(x => x.GroupId == item.GroupId);
                            $ctrl.NguoiTheoDoi.splice(index, 1);
                            item.LOAIXULY = vaitro;
                            themNhomPhanCong(item);
                        }
                    }
                }

                $ctrl.openUserSelect = function (loaixl) {
                    ModalService.open({
                        templateUrl: 'modalPhanPhatTree.html',
                        controller: 'phanphatTreeCtrl',
                        resolve: {
                            idselect: function () {
                                let obj = {};
                                obj.vaitro = loaixl;
                                obj.listuser = Array.prototype.map.call($ctrl.dsnhanvientemp2, s => s.UserName).toString();
                                obj.listtrangthai = [{ li: true, div: true, seeli: true, Ten: "Nhóm Quản Lý", IsView: 1 }, { li: false, div: false, seeli: true, Ten: "Nhóm Phòng Ban", IsView: 0 }];
                                return obj;
                            }
                        }
                    }).then(function (c) {
                        angular.forEach(c, function (i) {
                            $ctrl.NguoiTheoDoi.push(i);
                            $ctrl.addUpdateUser(i, 1, loaixl);
                        });
                    });
                }

                function GetBoPhanNhan() {
                    blockUI.start();
                    $ctrl.dsNhomPhongBan = [];
                    $ctrl.dsUserPhongBan = [];
                    $ctrl.NguoiTheoDoi = [];
                    $ctrl.dsnhanvientemp = [];
                    $ctrl.dsnhanvientemp2 = [];
                    var resp = loginservice.postdata("api/QLVanBan/getUsersNhanVBTheoPhongBan", $.param({ valint1: -1 }));
                    resp.then(function (response) {
                        response.data.forEach(function (value, key) {
                            $ctrl.dsNhomPhongBan.push(value.group);
                            value.listusers.forEach(function (val, idx) {
                                $ctrl.dsUserPhongBan.push(val);
                            });
                        });
                        GetChiTietVB();
                    },
                        function errorCallback(response) {
                            blockUI.stop();
                        });
                }

                function GetChiTietVB() {
                    let resp = loginservice.postdata("api/QLBaoCao/GetVanBanChiTiet2", $.param({ valint1: idselect.IDVANBAN, valint3: idselect.LoaiVB }));
                    resp.then(function (response) {
                        $ctrl.objVB = response.data;
                        if ($ctrl.objVB.NhomPhanCong.length > 0 || ($ctrl.objVB.NhomPhanCong.length == 0 && $ctrl.objVB.UserPhanCong.length > 0)) {
                            for (let i = 0; i < $ctrl.objVB.NhomPhanCong.length; i++) {
                                let index = $ctrl.dsNhomPhongBan.findIndex(x => x.GroupId == $ctrl.objVB.NhomPhanCong[i].GroupId);
                                if (index != -1) {
                                    $ctrl.dsNhomPhongBan.splice(index, 1);
                                    $ctrl.dsnhanvientemp.push($ctrl.objVB.NhomPhanCong[i]);
                                    if ($ctrl.objVB.NhomPhanCong[i].LOAIXULY == 2) {
                                        $ctrl.NguoiTheoDoi.push($ctrl.objVB.NhomPhanCong[i]);
                                    }
                                }
                            }
                            for (let i = 0; i < $ctrl.objVB.UserPhanCong.length; i++) {
                                $ctrl.objVB.UserPhanCong[i].checked = $ctrl.objVB.UserPhanCong[i].isChecked == 0 ? false : true;
                                $ctrl.dsnhanvientemp2.push($ctrl.objVB.UserPhanCong[i]);
                                if ($ctrl.objVB.UserPhanCong[i].isChecked == 1) {
                                    if ($ctrl.objVB.UserPhanCong[i].LOAIXULY == 2) {
                                        $ctrl.NguoiTheoDoi.push($ctrl.objVB.UserPhanCong[i]);
                                    }
                                }
                            }
                        }
                        blockUI.stop();
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }

                function xoaNhomPhanCong(GroupId) {
                    let resp = loginservice.postdata("api/QLBaoCao/XoaNhomPhanCong", $.param({ valint1: idselect.IDVANBAN, valint2: idselect.LoaiVB, valint3: GroupId }));
                    resp.then(function (response) {
                        thongbao.success("Xóa thành công");
                        GetBoPhanNhan();
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }

                function themNhomPhanCong(item) {
                    let resp = loginservice.postdata("api/QLBaoCao/ThemNhomPhanCong", $.param({ valint1: idselect.IDVANBAN, valint2: idselect.LoaiVB, valint3: item.GroupId, valint4: item.LOAIXULY, valstring1: item.FullName }));
                    resp.then(function (response) {
                        thongbao.success("Thêm thành công");
                        GetBoPhanNhan();
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }

                function xoaCaNhan(item) {
                    let resp = loginservice.postdata("api/QLBaoCao/XoaCaNhan", $.param({ valint1: idselect.IDVANBAN, valint2: idselect.LoaiVB, valstring1: item.UserName }));
                    resp.then(function (response) {
                        thongbao.success("Xóa thành công");
                        GetBoPhanNhan();
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }

                function themCaNhan(item) {
                    let resp = loginservice.postdata("api/QLBaoCao/ThemCaNhan", $.param({ valint1: idselect.IDVANBAN, valint2: idselect.LoaiVB, valint3: item.GroupId, valint4: item.LOAIXULY, valint5: 0, valstring1: item.UserName }));
                    resp.then(function (response) {
                        item.UserIDVB = response.data;
                        item.NGUOITAO = $ctrl.Userdata.username;
                        thongbao.success("Thêm thành công");
                        GetBoPhanNhan();
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }
            }]);
}());