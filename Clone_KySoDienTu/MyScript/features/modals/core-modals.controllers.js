angular
    .module('aims')
    .controller('ModalGCALCtrl', ModalGCALCtrl)
    .controller('ModalNewCtrl', ModalNewCtrl)
    .controller('ModalCtrl', ModalCtrl)
    .controller('UpdateModulePermissionCtrl', UpdateModulePermissionCtrl)
    .controller('UpdateModuleUserPermissionCtrl', UpdateModuleUserPermissionCtrl)
    .controller('LichCaNhanCtrl', LichCaNhanCtrl);

// -----------------------
// Helpers
// -----------------------
function createStyleManager(styleSheetIndex) {
    var sheet = document.styleSheets[styleSheetIndex || 0];

    function deleteRule(selector) {
        if (!sheet) return;

        var rules = sheet.rules || sheet.cssRules || [];
        for (var i = 0; i < rules.length; i++) {
            if (selector === rules[i].selectorText) {
                sheet.deleteRule(i);
                return;
            }
        }
    }

    function addRule(selector, text) {
        if (!sheet) return;
        deleteRule(selector);
        sheet.insertRule(selector + text);
    }

    return {
        addRule: addRule,
        deleteRule: deleteRule
    };
}

function toggleRowGroupIcon(rowKey, iconElementId, myStyles) {
    var el = document.getElementById(iconElementId);
    if (!el) return;

    var $el = angular.element(el);
    var cls = $el.attr('class');

    if (cls === 'ui-icon ace-icon fa fa-minus center bigger-110 blue') {
        myStyles.addRule('tr[name="' + rowKey + '"]', '{display: none}');
        $el.attr('class', 'ui-icon ace-icon fa fa-plus center bigger-110 blue');
    } else {
        myStyles.deleteRule('tr[name="' + rowKey + '"]');
        $el.attr('class', 'ui-icon ace-icon fa fa-minus center bigger-110 blue');
    }
}

// -----------------------
// modalGCALCtrl
// -----------------------
ModalGCALCtrl.$inject = [
    '$timeout',
    'funGroupUITree',
    'blockUI',
    '$uibModalInstance',
    'loginservice',
    'idselect'
];

function ModalGCALCtrl($timeout, funGroupUITree, blockUI, $uibModalInstance, loginservice, idselect) {
    var $ctrl = this;

    var myStyles = createStyleManager(0);

    $ctrl.checklist = [];
    $ctrl.modalreturn = [];
    $ctrl.dsnhanvien = [];

    $ctrl.phongban = [];
    $ctrl.nguoidung = [];
    $ctrl.nhomchucnang = [];

    var temp = [];
    if (idselect && idselect.listus) temp = String(idselect.listus).split(',');

    $ctrl.CheckSingle = CheckSingle;
    $ctrl.LuuNhom = LuuNhom;

    $ctrl.myClick = function (rowKey) {
        // rowKey chính là i trong code cũ
        toggleRowGroupIcon(rowKey, rowKey, myStyles);
    };

    $ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $ctrl.close = function (item) {
        $uibModalInstance.close(item);
    };

    init();

    function init() {
        loadNhanVien();
        loadUserSortGroup();
    }

    function loadNhanVien() {
        var resp = loginservice.postdata('api/getUser/getDanhSachNhanVien', $.param({ valint1: 6 }));
        resp.then(function (response) {
            $ctrl.dsnhanvien = response.data || [];
        });
    }

    function loadUserSortGroup() {
        blockUI.start();
        var resp = funGroupUITree.GetAllUsersGroups(0, 'RWF', 'CoreGP');

        resp.then(function (response) {
            $ctrl.phongban = response.data || [];
            $ctrl.phongban.forEach(function (v) {
                myStyles.addRule('tr[name="G' + v.Groups.GroupId + '"]', '{display: none}');
            });

            if (idselect && idselect.listus) {
                waitUntil(function () { return $ctrl.phongban.length > 0; }, 300)
                    .then(function () {
                        angular.forEach([].concat(temp), function (j, index) {
                            var item = $ctrl.phongban.find(function (x) { return x.Groups.FullName == j; });
                            if (item) {
                                $ctrl.checklist.push(item);
                                temp.splice(index, 1);
                            }
                        });
                    });
            } else {
                $ctrl.checklist = [];
            }

            loadUserGroup();
        }).finally(function () {
            blockUI.stop();
        });
    }

    function loadUserGroup() {
        blockUI.start();
        var resp = funGroupUITree.GetAllUsersGroupsTree(0);

        resp.then(function (response) {
            $ctrl.nguoidung = response.data || [];
            $ctrl.nguoidung.forEach(function (v) {
                myStyles.addRule('tr[name="U' + v.Groups.GroupId + '"]', '{display: none}');
            });

            if (idselect && idselect.listus) {
                waitUntil(function () { return ($ctrl.dsnhanvien || []).length > 0; }, 300)
                    .then(function () {
                        angular.forEach([].concat(temp), function (j) {
                            var i = $ctrl.dsnhanvien.findIndex(function (x) {
                                return String(x.FullName).trim() === String(j).trim();
                            });
                            if (i > -1) {
                                $ctrl.checklist.push({
                                    Groups: { FullName: $ctrl.dsnhanvien[i].FullName },
                                    listUsers: [{ UserName: $ctrl.dsnhanvien[i].UserName, FullName: $ctrl.dsnhanvien[i].FullName }]
                                });
                                $ctrl.dsnhanvien.splice(i, 1);
                            }
                        });
                    });
            } else {
                $ctrl.checklist = [];
            }

            loadNhom();
        }).finally(function () {
            blockUI.stop();
        });
    }

    function loadNhom() {
        blockUI.start();
        var resp = funGroupUITree.GetAllUsersGroupsTree(1);

        resp.then(function (response) {
            $ctrl.nhomchucnang = response.data || [];
            $ctrl.nhomchucnang.forEach(function (v) {
                myStyles.addRule('tr[name="C' + v.Groups.GroupId + '"]', '{display: none}');
            });

            if (idselect && idselect.listus) {
                waitUntil(function () { return ($ctrl.dsnhanvien || []).length > 0; }, 300)
                    .then(function () {
                        angular.forEach([].concat(temp), function (j) {
                            var i = $ctrl.dsnhanvien.findIndex(function (x) {
                                return String(x.FullName).trim() === String(j).trim();
                            });
                            if (i > -1) {
                                $ctrl.checklist.push({
                                    Groups: { FullName: $ctrl.dsnhanvien[i].FullName },
                                    listUsers: [{ UserName: $ctrl.dsnhanvien[i].UserName, FullName: $ctrl.dsnhanvien[i].FullName }]
                                });
                                $ctrl.dsnhanvien.splice(i, 1);
                            }
                        });
                    });
            } else {
                $ctrl.checklist = [];
            }
        }).finally(function () {
            blockUI.stop();
        });
    }

    function CheckSingle(item, checked) {
        if (!item) return;

        if (checked) {
            var names = Array.prototype.map.call(item, function (s) { return String(s.FullName).trim(); });
            $ctrl.checklist = $ctrl.checklist.filter(function (x) {
                return names.indexOf(String(x.Groups.FullName).trim()) === -1;
            });
            item.forEach(function (v) {
                $ctrl.checklist.push({
                    Groups: { FullName: v.FullName },
                    listUsers: [{ UserName: v.UserName, FullName: v.FullName }]
                });
            });
        } else {
            var names2 = Array.prototype.map.call(item, function (s) { return String(s.FullName).trim(); });
            $ctrl.checklist = $ctrl.checklist.filter(function (x) {
                return names2.indexOf(String(x.Groups.FullName).trim()) === -1;
            });
        }
    }

    function LuuNhom() {
        $ctrl.modalreturn = [];
        $ctrl.modalreturn.push(idselect.id);
        $ctrl.modalreturn.push($ctrl.checklist);
        $uibModalInstance.close($ctrl.modalreturn);
    }

    function waitUntil(predicateFn, delayMs) {
        return new Promise(function (resolve) {
            (function tick() {
                if (predicateFn()) return resolve();
                $timeout(tick, delayMs || 300);
            })();
        });
    }
}

// -----------------------
// modalnewCtrl
// -----------------------
ModalNewCtrl.$inject = [
    'funGroupUITree',
    'blockUI',
    '$uibModalInstance',
    'idselect'
];

function ModalNewCtrl(funGroupUITree, blockUI, $uibModalInstance, idselect) {
    var $ctrl = this;
    var myStyles = createStyleManager(0);

    $ctrl.checklist = [];
    $ctrl.modalreturn = [];

    $ctrl.nguoidung = [];
    $ctrl.nhomchucnang = [];

    var temp = [];
    if (idselect && idselect.listus && idselect.type == 1) {
        temp = idselect.listus.filter(function (x) { return x.VAITRO != idselect.vaitro; });
        angular.forEach(idselect.listus.filter(function (x) { return x.VAITRO == idselect.vaitro; }), function (us) {
            $ctrl.checklist.push({ UserName: us.UserName, FullName: us.FullName });
        });
    }

    $ctrl.CheckSingle = CheckSingle;
    $ctrl.LuuNhom = LuuNhom;

    $ctrl.myClick = function (rowKey) {
        toggleRowGroupIcon(rowKey, rowKey, myStyles);
    };

    $ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $ctrl.close = function (item) {
        $uibModalInstance.close(item);
    };

    init();

    function init() {
        loadUserGroup();
        loadNhom();
    }

    function loadUserGroup() {
        blockUI.start();
        var resp = funGroupUITree.GetAllUsersGroupsTree(0);

        resp.then(function (response) {
            $ctrl.nguoidung = response.data || [];
            $ctrl.nguoidung.forEach(function (v) {
                myStyles.addRule('tr[name="U' + v.Groups.GroupId + '"]', '{display: none}');

                if (temp && temp.length) {
                    angular.forEach(temp, function (j) {
                        var idx = v.listUsers.findIndex(function (x) { return x.UserName == j.UserName; });
                        if (idx > -1) v.listUsers.splice(idx, 1);
                    });
                }
            });

            $ctrl.nguoidung = $ctrl.nguoidung.filter(function (x) { return (x.listUsers || []).length > 0; });
        }).finally(function () {
            blockUI.stop();
        });
    }

    function loadNhom() {
        blockUI.start();
        var resp = funGroupUITree.GetAllUsersGroupsTree(1);

        resp.then(function (response) {
            $ctrl.nhomchucnang = response.data || [];
            $ctrl.nhomchucnang.forEach(function (v) {
                myStyles.addRule('tr[name="C' + v.Groups.GroupId + '"]', '{display: none}');

                if (temp && temp.length) {
                    angular.forEach(temp, function (j) {
                        var idx = v.listUsers.findIndex(function (x) { return x.UserName == j.UserName; });
                        if (idx > -1) v.listUsers.splice(idx, 1);
                    });
                }
            });

            $ctrl.nhomchucnang = $ctrl.nhomchucnang.filter(function (x) { return (x.listUsers || []).length > 0; });
        }).finally(function () {
            blockUI.stop();
        });
    }

    function CheckSingle(item, checked) {
        if (!item) return;

        if (checked) {
            var usernames = Array.prototype.map.call(item, function (s) { return s.UserName; });
            $ctrl.checklist = $ctrl.checklist.filter(function (x) {
                return usernames.indexOf(x.UserName) === -1;
            });

            item.forEach(function (v) {
                $ctrl.checklist.push({ UserName: v.UserName, FullName: v.FullName });
            });
        } else {
            var usernames2 = Array.prototype.map.call(item, function (s) { return s.UserName; });
            $ctrl.checklist = $ctrl.checklist.filter(function (x) {
                return usernames2.indexOf(x.UserName) === -1;
            });
        }
    }

    function LuuNhom() {
        $ctrl.modalreturn = [];
        $ctrl.modalreturn.push(idselect.id);
        $ctrl.modalreturn.push($ctrl.checklist);
        $uibModalInstance.close($ctrl.modalreturn);
    }
}

// -----------------------
// modalCtrl
// -----------------------
ModalCtrl.$inject = [
    'funGroupUITree',
    'blockUI',
    '$uibModalInstance',
    'loginservice',
    'idselect'
];

function ModalCtrl(funGroupUITree, blockUI, $uibModalInstance, loginservice, idselect) {
    var $ctrl = this;
    var myStyles = createStyleManager(0);

    $ctrl.checklist = (idselect && idselect.listus)
        ? String(idselect.listus).split(', ')
        : [];

    $ctrl.modalreturn = [];

    $ctrl.phongban = [];
    $ctrl.canhan = [];

    $ctrl.CheckSingle = CheckSingle;
    $ctrl.LuuNhom = LuuNhom;

    $ctrl.myClick = function (rowKey) {
        toggleRowGroupIcon(rowKey, rowKey, myStyles);
    };

    $ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    init();

    function init() {
        loadUserSortGroup();
        loadDanhba();
    }

    function CheckSingle(item, checked) {
        if (!item) return;

        if (checked) {
            var usernames = Array.prototype.map.call(item, function (s) { return s.UserName; });
            $ctrl.checklist = $ctrl.checklist.filter(function (x) { return usernames.indexOf(x) === -1; });

            item.forEach(function (v) {
                $ctrl.checklist.push(v.UserName);
            });
        } else {
            var usernames2 = Array.prototype.map.call(item, function (s) { return s.UserName; });
            $ctrl.checklist = $ctrl.checklist.filter(function (x) { return usernames2.indexOf(x) === -1; });
        }
    }

    function loadUserSortGroup() {
        blockUI.start();
        var resp = funGroupUITree.GetAllUsersGroupsTree(0);

        resp.then(function (response) {
            $ctrl.phongban = response.data || [];
            $ctrl.phongban.forEach(function (v) {
                myStyles.addRule('tr[name="' + v.Groups.GroupId + '"]', '{display: none}');
            });
        }).finally(function () {
            blockUI.stop();
        });
    }

    function loadDanhba() {
        blockUI.start();
        var resp = loginservice.postdata('api/danhba/getViewdanhba');

        resp.then(function (response) {
            $ctrl.canhan = response.data || [];
            $ctrl.canhan.forEach(function (v) {
                myStyles.addRule('tr[name="' + v.Groups.GroupId + '"]', '{display: none}');
            });
        }).finally(function () {
            blockUI.stop();
        });
    }

    function LuuNhom() {
        $ctrl.modalreturn = [];
        $ctrl.modalreturn.push(idselect.id);
        $ctrl.modalreturn.push($ctrl.checklist);
        $uibModalInstance.close($ctrl.modalreturn);
    }
}

// -----------------------
// UpdateModulePermissionCtrl
// -----------------------
UpdateModulePermissionCtrl.$inject = [
    'thongbao',
    'blockUI',
    '$uibModalInstance',
    'loginservice',
    'idselect'
];

function UpdateModulePermissionCtrl(thongbao, blockUI, $uibModalInstance, loginservice, idselect) {
    var $ctrl = this;

    $ctrl.Steven = idselect;

    $ctrl.CheckAll = CheckAll;
    $ctrl.UnCheckAll = UnCheckAll;
    $ctrl.sumitformedit = sumitformedit;
    $ctrl.cancel = cancel;

    init();

    function init() {
        getAllGroups();
    }

    function getAllGroups() {
        var resp = loginservice.postdata('api/getCore/getAllGroups');
        resp.then(function (response) {
            $ctrl.Steven.listgroups = response.data || [];
        });
    }

    function CheckAll(groupId) {
        $ctrl.Steven.listgroupspers = ($ctrl.Steven.listgroupspers || []).filter(function (x) {
            return x.GroupId !== groupId;
        });

        ($ctrl.Steven.listpers || []).forEach(function (p) {
            $ctrl.Steven.listgroupspers.push({ GroupId: groupId, PermissionId: p.PermissionId });
        });
    }

    function UnCheckAll(groupId) {
        $ctrl.Steven.listgroupspers = ($ctrl.Steven.listgroupspers || []).filter(function (x) {
            return x.GroupId !== groupId;
        });
    }

    function sumitformedit() {
        blockUI.start();
        var resp = loginservice.postdata('api/getCore/UpdateGroupsPermissions', $.param({
            valstring1: 'Doc',
            valstring2: 'G',
            valstring3: $ctrl.Steven.ResourceId,
            listgroupspers: $ctrl.Steven.listgroupspers
        }));

        resp.then(function () {
            $uibModalInstance.close(1);
        }, function () {
            thongbao.error('Thông báo', 'Thêm sự kiện không thành công');
        }).finally(function () {
            blockUI.stop();
        });
    }

    function cancel() {
        $uibModalInstance.dismiss('cancel');
    }
}

// -----------------------
// UpdateModuleUserPermissionCtrl
// -----------------------
UpdateModuleUserPermissionCtrl.$inject = [
    'thongbao',
    'blockUI',
    '$uibModalInstance',
    'loginservice',
    'idselect'
];

function UpdateModuleUserPermissionCtrl(thongbao, blockUI, $uibModalInstance, loginservice, idselect) {
    var $ctrl = this;
    var myStyles = createStyleManager(0);

    $ctrl.Steven = idselect;

    $ctrl.CheckAll = CheckAll;
    $ctrl.UnCheckAll = UnCheckAll;
    $ctrl.myClick = myClick;
    $ctrl.sumitformedit = sumitformedit;
    $ctrl.cancel = cancel;

    init();

    function init() {
        getAllUsersSortPhongBan();
    }

    function CheckAll(userId) {
        $ctrl.Steven.listuserspers = ($ctrl.Steven.listuserspers || []).filter(function (x) {
            return x.UserId !== userId;
        });

        ($ctrl.Steven.listpers || []).forEach(function (p) {
            $ctrl.Steven.listuserspers.push({ UserId: userId, PermissionId: p.PermissionId });
        });
    }

    function UnCheckAll(userId) {
        $ctrl.Steven.listuserspers = ($ctrl.Steven.listuserspers || []).filter(function (x) {
            return x.UserId !== userId;
        });
    }

    function myClick(rowKey) {
        toggleRowGroupIcon(rowKey, rowKey, myStyles);
    }

    function getAllUsersSortPhongBan() {
        var resp = loginservice.postdata('api/getCore/getAllUsersSortPhongBan');
        resp.then(function (response) {
            $ctrl.Steven.listusers = response.data || [];
            $ctrl.Steven.listusers.forEach(function (v) {
                myStyles.addRule('tr[name="' + v.group.GroupId + '"]', '{display: none}');
            });
        });
    }

    function sumitformedit() {
        blockUI.start();
        var resp = loginservice.postdata('api/getCore/UpdateUsersPermissions', $.param({
            valstring1: $ctrl.Steven.ModuleKey,
            valstring2: $ctrl.Steven.ResourceType,
            valstring3: $ctrl.Steven.ResourceId,
            listuserspers: $ctrl.Steven.listuserspers
        }));

        resp.then(function () {
            $uibModalInstance.close(1);
        }, function () {
            thongbao.error('Thông báo', 'Thêm không thành công');
        }).finally(function () {
            blockUI.stop();
        });
    }

    function cancel() {
        $uibModalInstance.dismiss('cancel');
    }
}

// -----------------------
// lichcanhanCtrl
// -----------------------
LichCaNhanCtrl.$inject = [
    '$window',
    'thongbao',
    'blockUI',
    '$uibModal',
    '$document',
    '$uibModalInstance',
    'loginservice',
    'idselect'
];

function LichCaNhanCtrl($window, thongbao, blockUI, $uibModal, $document, $uibModalInstance, loginservice, idselect) {
    var $ctrl = this;

    $ctrl.selectitem = null;

    $ctrl.redirectSte = function (url) {
        $window.open(url, '_blank');
    };

    $ctrl.EditSukien = EditSukien;
    $ctrl.DeleteUserEvent = DeleteUserEvent;
    $ctrl.EndSukien = EndSukien;
    $ctrl.EndThongbao = EndThongbao;

    $ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $ctrl.close = function (item) {
        $uibModalInstance.close(item);
    };

    init();

    function init() {
        getChiTiet();
    }

    function getChiTiet() {
        var resp = loginservice.postdata('api/danhba/getUserEvent', $.param({ valint1: idselect }));
        resp.then(function (response) {
            $ctrl.selectitem = response.data;
        }, function () {
            $ctrl.selectitem = [];
        });
    }

    function EditSukien() {
        var parentElem = angular.element($document[0].querySelector('.content-wrapper'));
        var modalInstance = $uibModal.open({
            animation: true,
            backdrop: 'static',
            templateUrl: '_CreateEditPUGEvent.html',
            controller: 'editPUGEventCtrl',
            controllerAs: '$ctrl',
            size: 'lg',
            appendTo: parentElem,
            resolve: {
                idselect: function () {
                    if ($ctrl.selectitem) $ctrl.selectitem.filter = -1;
                    return $ctrl.selectitem;
                }
            }
        });

        modalInstance.result.then(function () {
            thongbao.success();
            getChiTiet();
        }, function () {
            getChiTiet();
        });
    }

    function DeleteUserEvent() {
        thongbao.sweetAlert('Bạn muốn xóa sự kiện này ?')
            .then(function (result) {
                if (result && result.value) {
                    var resp = loginservice.postdata('api/danhba/deleteUserEvent', $.param({ valint1: idselect }));
                    resp.then(function () {
                        $uibModalInstance.close(1);
                    }, function () {
                        thongbao.error();
                    });
                } else if (result && result.dismiss === Swal.DismissReason.cancel) {
                    thongbao.sweetAlert2(0);
                }
            });
    }

    function EndSukien(hoanthanh) {
        thongbao.sweetAlert('Bạn muốn hoàn thành/tiếp tục sự kiện này ?')
            .then(function (result) {
                if (result && result.value) {
                    var resp = loginservice.postdata('api/danhba/EndUserEvent', $.param({ valint1: idselect, valint2: hoanthanh }));
                    resp.then(function () {
                        $uibModalInstance.close(1);
                    }, function () {
                        thongbao.error();
                    });
                } else if (result && result.dismiss === Swal.DismissReason.cancel) {
                    thongbao.sweetAlert2(0);
                }
            });
    }

    function EndThongbao() {
        thongbao.sweetAlert('Bạn muốn kết thúc nhắc sự kiện này ?')
            .then(function (result) {
                if (result && result.value) {
                    var resp = loginservice.postdata('api/danhba/UpdateQueueSMS', $.param({ valint1: idselect, valint2: 2 }));
                    resp.then(function () {
                        $uibModalInstance.close(1);
                    }, function () {
                        thongbao.error();
                    });
                } else if (result && result.dismiss === Swal.DismissReason.cancel) {
                    thongbao.sweetAlert2(0);
                }
            });
    }
}