angular.module("aims")
    .controller("homeCongViecCtrl", [
        '$uibModalInstance',
        'thongbao',
        "$scope",
        "$timeout",
        "ModalService",
        "blockUI",
        "appSettings",
        "loginservice",
        "userProfile",
        "idselect",
        function (
            $uibModalInstance,
            thongbao,
            $scope,
            $timeout,
            ModalService,
            blockUI,
            appSettings,
            loginservice,
            userProfile,
            idselect) {
            var $ctrl = this;

            $ctrl.ok = function () {
                $ctrl.presult = "0";
            };

            $ctrl.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $ctrl.xl = {};

            $ctrl.rowID = 0;

            $scope.chitietcongviec = {};

            $scope.quyenuser = {};

            $scope.cv = {};

            $scope.viewwwin = 5;

            $scope.tieude = ['CV lưu nháp', 'CV chờ duyệt', 'CV đang xử lý', 'CV hoàn thành']

            $scope.filtercv = 4;

            $scope.filtervt = 0;

            $scope.abctest = false;

            $scope.abckhoa = false;

            $scope.currentPage = 1;

            $scope.isObjectEmpty = function (card) {
                return Object.keys(card).length === 0;
            }

            //#region Load CV +Signal

            $scope.selectrows = function (id, daxem) {
                blockUI.start();
                $ctrl.rowID = id;
                if ($scope.DaXems[daxem] != 1) {
                    updateDaXem(id, daxem);
                }
                var resp = loginservice.postdata("api/congviec/getchitietcv", $.param({ valint1: id }));
                resp.then(function (response) {
                    $scope.chitietcongviec = {};
                    $scope.listfile = {};
                    $scope.CVlistfile = {};
                    if (response.data.cvchitiet != null) {
                        $scope.chitietcongviec = response.data.cvchitiet;
                        $scope.abctest = $scope.chitietcongviec.TRANGTHAI == 2 ? true : false;
                        $scope.abckhoa = $scope.chitietcongviec.KHOAXL;
                        $scope.viewwwin = $scope.chitietcongviec.VAITRO;
                    }
                    if (response.data.dsnhanvien != null) {
                        $scope.nhanvienct = []; $scope.nhanvientd = []; $scope.nhanvienxl = []; $scope.nhanvienph = [];
                        for (let i = 0; i < response.data.dsnhanvien.length; i++) {
                            switch (response.data.dsnhanvien[i].isRole) {
                                case 1:
                                    $scope.nhanvienct.push(response.data.dsnhanvien[i]);
                                    break;
                                case 2:
                                    $scope.nhanvientd.push(response.data.dsnhanvien[i]);
                                    break;
                                case 3:
                                    $scope.nhanvienxl.push(response.data.dsnhanvien[i]);
                                    break;
                                case 4:
                                    $scope.nhanvienph.push(response.data.dsnhanvien[i]);
                                    break;
                                //default:
                            }
                        }
                        //$scope.nhanviencv = response.data.dsnhanvien;
                    }
                    if (response.data.listfile != null)
                        $scope.listfile = response.data.listfile;
                    if (response.data.CVlistfile != null)
                        $scope.CVlistfile = response.data.CVlistfile;
                    getdsXL(id);
                    blockUI.stop();
                }, function errorCallback(response) {
                    $scope.chitietcongviec = {};
                    $scope.dsXuLy = {};
                    $scope.listfile = {};
                    $scope.CVlistfile = {};
                    blockUI.stop();
                });

            }

            $scope.selectabc = function (item) {
                $ctrl.xl = item;
                $scope.cvTachs = [];
                $scope.DaXems = [];
                $scope.selectID = item.ID;
                $scope.viewwwin = 5;
                $scope.btnGuiXL = item.VAITRO == 3 ? true : false;
                if (item.Child != '') {
                    $scope.cvTachs = item.ChildID.split(',');
                }
                if (item.DAXEM != '') {
                    $scope.DaXems = item.DAXEM.split(',');
                }
                $scope.selectrows($scope.cvTachs[0], 0);
                $scope.selectrow = $scope.cvTachs[0];
            }

            $scope.selectabc(idselect)

            function getdsXL(id) {
                //$ctrl.xl.ID = id;
                var resp = loginservice.postdata("api/congviec/getdsXL", $.param({ valint1: id }));
                resp.then(function (response) {
                    $scope.dsXuLy = {};
                    if (response.data != null) {
                        $scope.dsXuLy = response.data;
                        var hamcho = function () {
                            if ($scope.dsXuLy.length == 0) {
                                $timeout(hamcho, 300);
                            }
                            else {
                                for (var i = 0; i < $scope.dsXuLy.length; i++) {
                                    $scope.dsXuLy[i].isCollapsed = [true, true];
                                }
                            }
                        }
                        $timeout(hamcho, 300);
                    }
                }, function errorCallback(response) {
                    $scope.dsXuLy = {};
                });
            }

            $scope.reloadXuLy = function () {
                getdsXL($ctrl.rowID);
            }
            //#endregion

            $scope.themcongviec = function (id) {
                ModalService.open({
                    templateUrl: 'Addcongviec.html',
                    controller: 'themcongviecCtrl',
                    size: 'lg100',
                    resolve: {
                        idselect: function () {
                            if (id == 1) {
                                $scope.chitietcongviec.tt = 1;
                                return $scope.chitietcongviec;
                            }
                            else if (id == 3) {
                                $scope.chitietcongviec.tt = 3;
                                return $scope.chitietcongviec;
                            }
                            else if (id == 4) {
                                var aa = {}
                                aa.tt = 4;
                                aa.TENCV = $scope.chitietcongviec.TENCV;
                                return aa;
                            }
                            else {
                                return 0;
                            }
                        }
                    }
                }).then(function () {
                    thongbao.success();
                    if (c.TRANGTHAI == 2)
                        updateDaXem(c);
                    else
                        getdataInnit(0);
                }, function () {
                    loginservice.deleteallfile();
                });
            }

            //#region Comment Feedback
            $scope.CreateComment = function () {
                ModalService.open({
                    templateUrl: '_CreateComment.html',
                    controller: 'CreateCommentCtrl',
                    size: 'lg50',
                    resolve: {
                        idselect: function () {
                            return $ctrl.rowID;
                        }
                    }
                }).then(function () {
                    thongbao.success();
                    $scope.reloadXuLy();
                }, function () {
                    loginservice.deleteallfile();
                });
            }

            $scope.CreateFeedback = function (item) {
                ModalService.open({
                    templateUrl: '_CreateFeedback.html',
                    controller: 'CreateFeedbackCtrl',
                    size: 'lg50',
                    resolve: {
                        idselect: function () {
                            return item;
                        }
                    }
                }).then(function () {
                    thongbao.success();
                    $scope.reloadXuLy();
                }, function () {
                    loginservice.deleteallfile();
                });
            }

            $scope.CreateFeedbackSimple = function (item) {
                var items = {};
                items.WorkflowID = item.par.WorkflowID;
                items.PARENTID = item.par.ID;
                items.TRANGTHAI = item.par.TRANGTHAI;
                items.LOAIXL = item.par.LOAIXL;
                items.CONGKHAI = item.par.CONGKHAI;
                items.YKIENXL = item.SteText;
                var resp3 = loginservice.postdata("api/congviec/InsertCommentFeedback", $.param({
                    par: items
                }));
                resp3.then(function (response) {
                    getdsXL($ctrl.rowID);
                    thongbao.success();
                    //    var resp = loginservice.postdata("api/app/UpdateChuaXemMobile", $.param({ valint1: idselect.par.WorkflowID, vallint2: 0 }));
                    //    resp.then(function (response) {
                    //    }, function errorCallback(response) {
                    //    });
                }, function errorCallback(response) {
                    thongbao.error();
                });
            }

            $scope.ChangeStatusComment = function (item) {
                thongbao.sweetAlert()
                    .then(function (result) {
                        if (result.value) {
                            let par = {}
                            par.valint1 = item.ID;
                            par.valint2 = item.TRANGTHAI == 1 ? 0 : 1;
                            var resp = loginservice.postdata("api/congviec/ChangeStatusComment", $.param(par));
                            resp.then(function (response) {
                                thongbao.sweetAlert2(1);
                                getdsXL($ctrl.rowID);
                            }, function errorCallback(response) {
                            });
                        }
                        else if (result.dismiss === Swal.DismissReason.cancel) {
                            thongbao.sweetAlert2(0);
                        }
                    })
            }

            $scope.ChangeStatusFeedback = function (item) {
                thongbao.sweetAlert()
                    .then(function (result) {
                        if (result.value) {
                            let par = {}
                            par.valint1 = item.ID;
                            par.valint2 = item.TRANGTHAI == 1 ? 0 : 1;
                            var resp = loginservice.postdata("api/congviec/ChangeStatusFeedback", $.param(par));
                            resp.then(function (response) {
                                thongbao.sweetAlert2(1);
                                getdsXL($ctrl.rowID);
                            }, function errorCallback(response) {
                            });
                        }
                        else if (result.dismiss === Swal.DismissReason.cancel) {
                            thongbao.sweetAlert2(0);
                        }
                    })
            }

            $scope.DeleteComment = function (id) {
                thongbao.sweetAlert()
                    .then(function (result) {
                        if (result.value) {
                            var resp = loginservice.postdata("api/congviec/DeleteComment", $.param({ valint1: id }));
                            resp.then(function (response) {
                                thongbao.sweetAlert2(1);
                                getdsXL($ctrl.rowID);
                            }, function errorCallback(response) {
                            });
                        }
                        else if (result.dismiss === Swal.DismissReason.cancel) {
                            thongbao.sweetAlert2(0);
                        }
                    })

            }

            $scope.DeleteFeedback = function (id) {
                thongbao.sweetAlert()
                    .then(function (result) {
                        if (result.value) {
                            var resp = loginservice.postdata("api/congviec/DeleteFeedback", $.param({ valint1: id }));
                            resp.then(function (response) {
                                thongbao.sweetAlert2(1);
                                getdsXL($ctrl.rowID);
                            }, function errorCallback(response) {
                            });
                        }
                        else if (result.dismiss === Swal.DismissReason.cancel) {
                            thongbao.sweetAlert2(0);
                        }
                    })

            }
            //#endregion

            //#region View Download File
            $scope.viewfilepdf = function (item, type) {
                item.type = type;
                ModalService.open({
                    templateUrl: 'viewPDFonline.html',
                    controller: 'viewfilepdfWFCtrl',
                    size: 'lg100',
                    resolve: {
                        idselect: function () {
                            return item;
                        }
                    }
                }).then(function () {
                }, function () {
                });
            }

            $scope.downloadFile = function (item, type) {
                blockUI.start();
                var resp = loginservice.getdatafile("api/congviec/Workflow?id=" + item.ID + "&type=" + type);
                resp.then(function (response) {
                    var headers = response.headers();
                    var contentType = headers['content-type'];
                    var file = new Blob([response.data], { type: contentType });
                    saveAs(file, item.MoTa);
                    blockUI.stop();
                }, function (response) {
                    blockUI.stop();
                    thongbao.errorcenter('Không tìm thấy file');
                });
            }

            $scope.viewfilepdfcv = function (i) {
                ModalService.open({
                    templateUrl: 'viewPDFonline.html',
                    controller: 'viewfilepdfvbCtrl',
                    size: 'lg100',
                    resolve: {
                        idselect: function () {
                            var item = {};
                            item.loaivb = 0;
                            item.id = i.ID;
                            item.type = 3;
                            return item;
                        }
                    }
                }).then(function () {
                }, function () {
                });
            }
            //#endregion

            $scope.UpdateWorkflowUser = function (i) {
                ModalService.open({
                    templateUrl: '_UpdateWorkflowUser.html',
                    controller: 'UpdateWorkflowUserCtrl',
                    size: 'lg50',
                    resolve: {
                        idselect: function () {
                            return i;
                        }
                    }
                }).then(function () {
                    getdataInnit(1);
                }, function () {
                });
            }

            $scope.checkDuyet = function (p, loai) {
                thongbao.sweetAlert()
                    .then(function (result) {
                        if (result.value) {
                            var resp = loginservice.postdata("api/congviec/ChangeStatusWorkflow", $.param({ valint1: p.ID, valint2: loai }));
                            resp.then(function (response) {
                                getdataInnit(0);
                            }, function errorCallback(response) {
                            });
                        }
                        else if (result.dismiss === Swal.DismissReason.cancel) {
                            thongbao.sweetAlert2(0);
                        }
                    })
            }

            $scope.LockWorkflow = function (p) {
                thongbao.sweetAlert()
                    .then(function (result) {
                        if (result.value) {
                            var resp = loginservice.postdata("api/congviec/ChangeLockWorkflow", $.param({ valint1: p.ID, valint2: $scope.abckhoa ? 0 : 1 }));
                            resp.then(function (response) {
                                getdataInnit(1);
                            }, function errorCallback(response) {
                                $scope.abckhoa = false;
                            });
                        }
                        else if (result.dismiss === Swal.DismissReason.cancel) {
                            thongbao.sweetAlert2(0);
                        }
                    })
            }

            $scope.DeleteWorkflowByUser = function () {
                thongbao.sweetAlert()
                    .then(function (result) {
                        if (result.value) {
                            var resp = loginservice.postdata("api/congviec/DeleteWorkflowByUser", $.param({ valint1: $ctrl.rowID }));
                            resp.then(function (response) {
                                thongbao.success();
                                getdataInnit(0);
                            }, function errorCallback(response) {
                                thongbao.error();
                            });
                        }
                        else if (result.dismiss === Swal.DismissReason.cancel) {
                            thongbao.sweetAlert2(0);
                        }
                    })
            }

            $scope.DeleteWorkflowInSaveDraft = function () {
                thongbao.sweetAlert()
                    .then(function (result) {
                        if (result.value) {
                            var resp = loginservice.postdata("api/congviec/DeleteWorkflowInSaveDraft", $.param({ valint1: $ctrl.rowID }));
                            resp.then(function (response) {
                                thongbao.success();
                                getdataInnit(0);
                            }, function errorCallback(response) {
                                thongbao.error();
                            });
                        }
                        else if (result.dismiss === Swal.DismissReason.cancel) {
                            thongbao.sweetAlert2(0);
                        }
                    })
            }

            $scope.EndWorkflow = function () {
                thongbao.sweetAlert("Báo cáo hoàn thành công việc")
                    .then(function (result) {
                        if (result.value) {
                            var resp = loginservice.postdata("api/congviec/ChangeStatusWorkflow", $.param({ valint1: $ctrl.rowID, valint2: 3 }));
                            resp.then(function (response) {
                                thongbao.success();
                                getdataInnit(0);
                            }, function errorCallback(response) {
                                thongbao.error();
                            });
                        }
                        else if (result.dismiss === Swal.DismissReason.cancel) {
                            thongbao.sweetAlert2(0);
                        }
                    })
            }

            $scope.CompleteWorkflow = function (GoiY) {
                thongbao.sweetAlert("Gợi ý hoàn thành công việc")
                    .then(function (result) {
                        if (result.value) {
                            var resp = loginservice.postdata("api/congviec/ChangeCompleteWorkflow", $.param({ valint1: $ctrl.rowID, valint2: GoiY == 1 ? 0 : 1 }));
                            resp.then(function (response) {
                                thongbao.success();
                                getdataInnit(0);
                            }, function errorCallback(response) {
                                thongbao.error();
                            });
                        }
                        else if (result.dismiss === Swal.DismissReason.cancel) {
                            thongbao.sweetAlert2(0);
                        }
                    })
            }

            $scope.RenewalsWorkflow = function (i) {
                ModalService.open({
                    templateUrl: '_RenewalsWorkflow.html',
                    controller: 'RenewalsCtrl',
                    size: 'lg50',
                    resolve: {
                        idselect: function () {
                            return i;
                        }
                    }
                }).then(function () {
                    getdataInnit(1);
                }, function () {
                });
            }

            $scope.updateTienDo = function (i) {
                ModalService.open({
                    templateUrl: '_UpdateTienDo.html',
                    controller: 'UpdateTienDoCtrl',
                    size: 'lg50',
                    resolve: {
                        idselect: function () {
                            return i;
                        }
                    }
                }).then(function () {
                    getdataInnit(1);
                }, function () {
                });
            }

            function updateDaXem(id, temp) {
                var resp = loginservice.postdata("api/congviec/updateDaXem", $.param({ valint1: id }));
                resp.then(function (response) {
                    $scope.DaXems[temp] = '1';
                }, function errorCallback(response) {
                });
            };
        }
    ]);