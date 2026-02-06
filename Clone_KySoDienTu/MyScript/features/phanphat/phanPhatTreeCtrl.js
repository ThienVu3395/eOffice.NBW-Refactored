angular
    .module("aims")
    .controller('phanPhatTreeCtrl', [
        "$scope",
        'blockUI',
        "$uibModalInstance",
        "ApiClient",
        "notificationService",
        "idselect",
        function (
            $scope,
            blockUI,
            $uibModalInstance,
            ApiClient,
            notificationService,
            idselect) {
            var $ctrl = this;

            $ctrl.VuLe = idselect;

            //console.log($ctrl.VuLe);

            $scope.$on('closeModal', function (data, mess) {
                notificationService.success(mess);
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
                var resp = ApiClient
                    .postData("api/getUser/getPhongBanUser", $.param({ valstring1: $ctrl.VuLe.listuser }))
                    .then(
                        function successCallback(response) {
                            $ctrl.phongban = response.data;
                            //console.log($ctrl.phongban);
                            $ctrl.phongban.forEach(function (value, key) {
                                myStyles.addRule('tr[name="' + value.GroupId + '"]', '{display: none}');
                                value.checked = false;
                            });
                            blockUI.stop();
                        },
                        function errorCallback(response) {
                            blockUI.stop();
                        }
                    );
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
        }
    ]);