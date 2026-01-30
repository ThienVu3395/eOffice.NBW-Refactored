angular
    .module("aims")
    .controller('phanPhatUserCtrl', [
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
            userProfile) {
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

            function themUser(item, loaiphancong, useridvb) {
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
        }
    ]);