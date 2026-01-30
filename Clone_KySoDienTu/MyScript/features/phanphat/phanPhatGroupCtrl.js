angular
    .module("aims")
    .controller('phanPhatGroupCtrl', [
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
            userProfile) {
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
        }
    ]);