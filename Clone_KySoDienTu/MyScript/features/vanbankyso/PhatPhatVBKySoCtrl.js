angular
    .module("aims")
    .controller('phanPhatVBKySoCtrl', [
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
        }
    ]);