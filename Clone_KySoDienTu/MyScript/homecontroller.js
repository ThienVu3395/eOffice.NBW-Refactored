(function () {
    "use strict";
    angular.module("oamsapp")
        .controller("homecontroller",
            [
                "$rootScope",
                "ModalService",
                "$scope",
                "appSettings",
                "loginservice",
                "userProfile",
                function (
                    $rootScope,
                    ModalService,
                    $scope,
                    appSettings,
                    loginservice,
                    userProfile) {
                    $scope.homeurl = appSettings.serverPath;

                    $scope.dataLoggedIn = {};

                    $scope.isLoggedIn = false;

                    $scope.userName = "";

                    $rootScope.FulluserName = "";

                    $rootScope.ulrimage = "";

                    $scope.access_token = "";

                    $scope.roleName = "";

                    $scope.viewwwin = false;

                    getdataUser();
                    //$scope.Notification =
                    //$scope.Chats.onConnected();

                    function getdataUser() {
                        var data = userProfile.getProfile();
                        if (data.isLoggedIn) {
                            $scope.isLoggedIn = data.isLoggedIn;
                            $scope.userName = data.username;
                            $scope.access_token = data.access_token;
                            $scope.roleName = data.roleName;
                            $scope.viewwwin = $scope.roleName != 'Docgia';
                            var datae = userProfile.getProfileExten();
                            $rootScope.FulluserName = datae.fileusername;
                            if (datae.ulrimage != null && datae.ulrimage != '' && datae.ulrimage != 'null') {
                                $rootScope.ulrimage = appSettings.serverPath + 'Content/image/' + datae.ulrimage;
                            }
                            else {
                                $rootScope.ulrimage = appSettings.serverPath + 'Content/image/user.png';
                            }
                        }
                        else {
                            window.location.href = appSettings.serverPath + appSettings.serverLogin;
                        }
                    }

                    $scope.chuyenthongbao = function (item) {
                        if (item.inttype == 1)
                            window.location.href = appSettings.serverPath + "Qlkhaithac/xetduyetYeucau?id=" + item.intkey + "&idct=" + item.intkey1;
                        else if (item.inttype == 2)
                            window.location.href = appSettings.serverPath + "Qlkhaithac/dsycduyettaikhoan";
                        else window.location.href = appSettings.serverPath + "Baoquan/qlxuattailieu";
                    }

                    $scope.$on('otherloginOntime', function (e, data) {
                        $scope.logout();
                    });

                    $rootScope.logout = function () {
                        var respd;
                        respd = loginservice.postdata("api/Account/Logout");
                        respd.then(function (response) {
                            userProfile.clearall();
                            window.location.href = appSettings.serverPath + appSettings.serverLogin;
                        }, function errorCallback(response) {
                            userProfile.clearall();
                            window.location.href = appSettings.serverPath + appSettings.serverLogin;
                        });

                    };

                    $rootScope.hosocanhan = function () {
                        ModalService.open({
                            templateUrl: '_hosocanhan.html',
                            controller: 'HoSoCaNhanCtrl',
                            resolve: {
                                idselect: function () {
                                    return $scope.userName;
                                }
                            }
                        }).then(function () {
                            getdataUser();
                        }, function () {
                            getdataUser();
                        });
                    };

                    $scope.BaoSuCoTB = function () {
                        ModalService.open({
                            templateUrl: 'formbaosuco.html',
                            controller: 'CapNhatLSSCCtrl',
                            resolve: {
                                idselect: function () {
                                    let xl = {};
                                    xl.from = 'user';
                                    return xl;
                                }
                            }
                        }).then(function () {
                            let resp = loginservice.postdata("api/QLVanBan/CapNhatTrangThaiYKienXuLy", $.param({ valint1: ID, valint2: 0, valint3: idselect.LoaiVB }));
                            resp.then(
                                function successCallback(response) {
                                    thongbao.success("Ý kiến xử lý đã được xóa");
                                    GetYKienXuLy();
                                },
                                function errorCallback(response) {
                                }
                            );
                        }, function () {
                            blockUI.stop();
                        });
                    }

                    $scope.deleteshopping = function () {
                        if (confirm("Bạn có muốn xóa hết tài liệu này ra khỏi yêu cầu?")) {
                            userProfile.clearShopingcart();
                            $scope.countitem = userProfile.getcountShoping();
                            $rootScope.$broadcast('handleBroadcast', 0);
                        }
                    };

                    $scope.countitem = userProfile.getcountShoping();

                    $scope.$on('handleBroadcast', function () {
                        $scope.countitem = userProfile.getcountShoping();
                    });

                    $scope.pData = [];

                    $scope.idfunction;

                    getdatachucnang();

                    function getdatachucnang() {
                        var respd;
                        //respd = loginservice.getdata("api/acountinfo/getuserchucnanggroup");
                        //respd.then(function (response) {
                        //    $scope.pData = response.data;
                        //}, function errorCallback(response) {

                        //});
                    }
                }])
        .controller("homethongbaocontroller",
            [
                "$rootScope",
                "$scope",
                'eventHub',
                "appSettings",
                "loginservice",
                "userProfile",
                function (
                    $rootScope,
                    $scope,
                    eventHub,
                    appSettings,
                    loginservice,
                    userProfile) {
                    // của Vũ                  
                    //var data = userProfile.getProfile();
                    //if (data.isLoggedIn) {
                    //    $scope.isLoggedIn = data.isLoggedIn;
                    //    $scope.userName = data.username;
                    //    $scope.access_token = data.access_token;
                    //    $scope.roleName = data.roleName;
                    //    $scope.viewwwin = $scope.roleName != 'Docgia';
                    //    var datae = userProfile.getProfileExten();
                    //    $scope.FulluserName = datae.fileusername;
                    //    if (datae.ulrimage != null && datae.ulrimage != '' && datae.ulrimage != 'null') {
                    //        $scope.ulrimage = appSettings.serverPath + datae.ulrimage;
                    //    }
                    //    else {
                    //        $scope.ulrimage = appSettings.serverPath + 'Content/image/user.png';
                    //    }
                    //    getListVanBan($scope.userName);
                    //}
                    // của Vũ
                    $scope.homeurl = appSettings.serverPath;
                    $scope.dataLoggedIn = {};
                    $scope.isLoggedIn = false;
                    $scope.userName = "";
                    $scope.FulluserName = "";
                    $scope.ulrimage = "";
                    $scope.access_token = "";
                    $scope.roleName = "";
                    //$rootScope.viewwwin = false;

                    $rootScope.countLich = 0;
                    $rootScope.countDuyet = 0;
                    $rootScope.countCongViec = 0;
                    $rootScope.countDuyetcv = 0;
                    $rootScope.counttong = 0;

                    $rootScope.SoLuongTB = 0;
                    $rootScope.SoLuongTBVBDenDB = 0;
                    $rootScope.SoLuongTBDenNDDVChuaXem = 0;

                    $rootScope.SoLuongTBDi = 0;
                    $rootScope.SoLuongTBVBDiDB = 0;
                    $rootScope.SoLuongTBDiNDDVChuaXem = 0;

                    $rootScope.SoLuongTBDenChoDuyet = 0;
                    $rootScope.SoLuongTBVBDenDBChoDuyet = 0;
                    $rootScope.SoLuongTBDenNDDVChoDuyet = 0;

                    $rootScope.SoLuongTBDiChoDuyet = 0;
                    $rootScope.SoLuongTBVBDiDBChoDuyet = 0;
                    $rootScope.SoLuongTBDiNDDVChoDuyet = 0;

                    $rootScope.SoLuongTBXoa = 0;
                    $rootScope.SoLuongTBXoaDi = 0;

                    $rootScope.TinNhanTBChuaXem = 0;
                    $rootScope.TinNhanTBGui = 0;
                    $rootScope.TinNhanTBDen = 0;
                    $rootScope.TinNhanTBTong = 0;
                    $scope.quyenuser = {};

                    //getdataUser();

                    eventHub.getCount();

                    getQuyen();

                    // Cập nhật thông báo khi bị xóa khỏi văn bản đến/đi
                    $scope.CapNhatThongBao = function (id, loai) {
                        var respd = loginservice.postdata("api/QLVanBan/CapNhatThongBaoXoa", $.param({ valint1: id, valint2: loai }));
                        respd.then(function (response) {
                            eventHub.getCount();
                        }, function errorCallback(response) {
                            blockUI.stop();
                        });
                    };

                    function getQuyen() {
                        var resp = loginservice.postdata("api/getCore/getAllPermissionsOfUserByModuleResource", $.param({ valstring1: "Other", valstring2: 'G', valstring3: 'M' }));
                        resp.then(function (response) {
                            $scope.quyenuser.ThongKeKBYT = response.data.findIndex(x => x.PermissionAction == 'TKKBYT') > -1;
                        }
                            , function errorCallback(response) {
                            });
                    }

                    //function dataInit() {
                    //    $scope.counttong = $scope.countLich + $scope.countDuyet;
                    //}
                    //getCount()
                    //    function getCount() {
                    //        var data = userProfile.getProfile();
                    //        if (data.isLoggedIn) {
                    //            $scope.isLoggedIn = data.isLoggedIn;
                    //            $scope.userName = data.username;
                    //            $scope.access_token = data.access_token;
                    //            $scope.roleName = data.roleName;
                    //            $scope.viewwwin = $scope.roleName != 'Docgia';

                    //            var respd;
                    //            respd = loginservice.postdata("api/cal_sukien//getSuKienUserByName", $.param({ valstring1: $scope.userName, valint1: 0 }));
                    //            respd.then(function (response) {
                    //                //console.log(response.data); 
                    //                $scope.countLich = response.data.length;
                    //                dataInit();
                    //            }, function errorCallback(response) {

                    //            });
                    //            if ($scope.viewwwin) {
                    //                var resp;
                    //                resp = loginservice.getdata("api/cal_sukien/getLichDuyet");
                    //                resp.then(function (response) {
                    //                    //console.log(response.data); 
                    //                    $scope.countDuyet = response.data.length;
                    //                    dataInit();
                    //                }, function errorCallback(response) {
                    //            });
                    //    }
                    //}
                    //else {
                    //    // window.location.href = appSettings.serverPath + appSettings.serverLogin;
                    //    $scope.counttong = 0;
                    //}
                    //}
                }])
        .controller("homethongbaoReportcontroller",
            [
                "$rootScope",
                "$scope",
                'ReportHub',
                function (
                    $rootScope,
                    $scope,
                    ReportHub) {
                    $rootScope.countreport = 0;

                    $rootScope.SoLuongReportChuaXem = 0;
                    $rootScope.SoLuongReportChoDuyet = 0;

                    $rootScope.SoLuongPhieuXinXeChoDuyet = 0;
                    $rootScope.SoLuongLenhDieuXeChoDuyet = 0;
                    $rootScope.SoLuongPhieuQuanLyXeChoDuyet = 0;
                    $rootScope.SoLuongUngTruocChoDuyet = 0;
                    $rootScope.SoLuongQuyetToanChoDuyet = 0;
                    $rootScope.SoLuongSoLoTrinhChoDuyet = 0;

                    $rootScope.SoLuongDonNghiPhepChoDuyet = 0;

                    $rootScope.SoLuongPhieuDanhGiaLuuNhap = 0;
                    $rootScope.SoLuongPhieuDanhGiaChoDuyet = 0;
                    $rootScope.SoLuongPhieuDanhGiaChoHuy = 0;
                    $rootScope.SoLuongPhieuDanhGiaHuyKy = 0;
                    $rootScope.SoLuongPhieuDanhGiaGoDuyet = 0;

                    $rootScope.SoLuongBangDanhGiaTongHopLuuNhap = 0;
                    $rootScope.SoLuongBangDanhGiaTongHopChoDuyet = 0;
                    $rootScope.SoLuongBangDanhGiaTongHopChoHuy = 0;
                    $rootScope.SoLuongBangDanhGiaTongHopHuyKy = 0;
                    $rootScope.SoLuongBangDanhGiaTongHopGoDuyet = 0;

                    ReportHub.getCountReport();
                }])
        .controller("Chatcontroller",
            [
                "$scope",
                "$timeout",
                "$element",
                "appSettings",
                "loginservice",
                "userProfile",
                "ChatService",
                function (
                    $scope,
                    $timeout,
                    $element,
                    appSettings,
                    loginservice,
                    userProfile,
                    ChatService)
                {
                    $scope.homeurl = appSettings.serverPath;

                    $scope.dataLoggedIn = {};

                    $scope.isLoggedIn = false;

                    $scope.userName = "";

                    $scope.FulluserName = "";

                    $scope.ulrimage = "";

                    $scope.access_token = "";

                    $scope.roleName = "";

                    $scope.viewwwin = false;

                    getdataUser();

                    function getdataUser() {
                        var data = userProfile.getProfile();
                        if (data.isLoggedIn) {
                            $scope.isLoggedIn = data.isLoggedIn;
                            $scope.userName = data.username;
                            $scope.access_token = data.access_token;
                            $scope.roleName = data.roleName;
                            $scope.viewwwin = $scope.roleName != 'Docgia';
                            var datae = userProfile.getProfileExten();
                            $scope.FulluserName = datae.fileusername;
                            if (datae.ulrimage != null && datae.ulrimage != '' && datae.ulrimage != 'null') {
                                $scope.ulrimage = appSettings.serverPath + datae.ulrimage;
                            }
                            else {
                                $scope.ulrimage = appSettings.serverPath + 'Content/image/user.png';
                            }
                        }
                        else {
                            // window.location.href = appSettings.serverPath + appSettings.serverLogin;
                        }
                    }

                    //Chat----------
                    $scope.Chats = ChatService;

                    $scope.sendchat = function (item) {
                        if (item.contentchat != undefined && item.contentchat != '') {
                            item.Conversations.push({ Name: '_Me', content: item.contentchat, status: true })
                            $scope.Chats.send(item.Name, item.contentchat);
                            item.contentchat = "";
                            $timeout(function () {
                                var elems = $element.find('#bodychat' + item.Name);
                                if (elems.length > 0)
                                    elems.scrollTop(elems[0].scrollHeight);
                                // Should return array of ng-repeated elements!
                            });
                        }

                    }

                    //userProfile.saveitem("openchatuser");

                    $scope.openchat = userProfile.getsaveitem('openchatuser');

                    $scope.clickopenchat = function () {
                        if ($scope.openchat == '1') {
                            userProfile.saveitem('openchatuser', '0');
                        }
                        else {
                            userProfile.saveitem('openchatuser', '1');
                        }
                    }

                    $scope.useronlineCurr = function () {
                        return function (item) {
                            return item.TimeUp > moment().toDate();
                        };
                    };

                    //$scope.windowsChats = [];
                    $scope.closewinChat = function (toUser) {
                        for (var i = 0; i < $scope.Chats.windowsChats.length; i++) {
                            if ($scope.Chats.windowsChats[i].Name == toUser) {
                                $scope.Chats.windowsChats.splice(i, 1);
                                $scope.listChats = $scope.listChats.replace('_;' + toUser + ';', '');
                                userProfile.saveitem('listChats', $scope.listChats);
                                if ($scope.Chats.windowsChats.length == 0) {
                                    //$('#home3').trigger('click');
                                    $('[href="#home3"]').tab('show');
                                }
                                break;
                            }
                        }
                    }

                    $scope.newwindowChat = function (toUser, reload) {
                        var isnew = true;
                        for (var i = 0; i < $scope.Chats.windowsChats.length; i++) {
                            if ($scope.Chats.windowsChats[i].Name == toUser) {
                                isnew = false;
                                $timeout(function () {
                                    var elems = $element.find('[href="#chat' + toUser + '"]');
                                    elems.tab('show');

                                    // Should return array of ng-repeated elements!
                                });
                            }
                        }
                        if (isnew) {
                            if (reload) {
                                if ($scope.listChats != undefined && $scope.listChats != '') {
                                    $scope.listChats += '_;' + toUser + ';';
                                    userProfile.saveitem('listChats', $scope.listChats);
                                } else {
                                    $scope.listChats = '_;' + toUser + ';';
                                    userProfile.saveitem('listChats', $scope.listChats);
                                }
                            }

                            for (var i = 0; i < $scope.Chats.UserList.length; i++) {
                                if ($scope.Chats.UserList[i].Name == toUser) {
                                    var winnew = {};
                                    winnew.Name = toUser;
                                    winnew.Fileimage = $scope.Chats.UserList[i].Fileimage;
                                    winnew.FullName = $scope.Chats.UserList[i].FullName;
                                    winnew.Conversations = [];
                                    $scope.Chats.windowsChats.push(winnew);
                                    if (reload) {
                                        $timeout(function () {
                                            var elems = $element.find('[href="#chat' + winnew.Name + '"]');
                                            elems.tab('show');

                                            // Should return array of ng-repeated elements!
                                        });
                                    }
                                    var respd = loginservice.postdata("api/userinfo/getchatdata", $.param({ valstring1: toUser }));
                                    respd.then(function (response) {
                                        if (response.data != null) {
                                            if (response.data.data != null)
                                                for (var k = 0; k < response.data.data.length; k++) {
                                                    $scope.Chats.windowsChats[$scope.Chats.windowsChats.length - 1].Conversations.push(response.data.data[k]);
                                                }
                                            $timeout(function () {
                                                var elems1 = $element.find('#bodychat' + toUser);
                                                if (elems1.length > 0)
                                                    elems1.scrollTop(elems1[0].scrollHeight);
                                            });
                                        }
                                    }, function errorCallback(response) {

                                    });
                                    break;
                                }
                            }

                        }
                    }

                    loadopenchat();

                    function loadopenchat() {
                        if ($scope.openchat == '1') {
                            var c = $("#ace-settings-box");
                            if (c.length > 0) {
                                if (c[0].className.indexOf('open') >= 0) {

                                }
                                else {
                                    $("#ace-settings-box").toggleClass("open");
                                }
                            }
                        }
                    }

                    $scope.keydownsearch = function (item) {
                        if (event.keyCode == 13) {
                            $scope.sendchat(item);
                        }
                    }

                    $scope.unread = false;

                    $scope.readedmes = function (item) {
                        $scope.unread = false;
                        item.isnew = false;
                    }

                    $scope.$on('newWinchat', function (e, data) {
                        $timeout(function () {
                            var elems = $element.find('[href="#chat' + data + '"]');
                            elems.tab('show');
                            var elems1 = $element.find('#bodychat' + data);
                            if (elems1.length > 0)
                                elems1.scrollTop(elems1[0].scrollHeight);
                            $scope.unread = true;
                            for (var i = 0; i < $scope.Chats.windowsChats.length; i++)
                                if ($scope.Chats.windowsChats[i].Name == data) {
                                    $scope.Chats.windowsChats[i].isnew = true;
                                }

                            var audio = new Audio(appSettings.serverPath + 'Content/audio/all-eyes-on-me.mp3');
                            audio.play();
                            // Should return array of ng-repeated elements!
                        });

                    });

                    $scope.$on('onConnectedF', function (e, data) {
                        if ($scope.Chats.id == data) {
                            $scope.listChats = userProfile.getsaveitem('listChats').toString();
                            if ($scope.listChats != undefined && $scope.listChats != '') {
                                var li = $scope.listChats.split('_');
                                for (var i = 0; i < li.length; i++) {
                                    var ui = li[i].replace(';', '');
                                    ui = ui.replace(';', '');
                                    if (ui != '' && ui != undefined && ui != '0')
                                        $timeout(function () {
                                            $scope.newwindowChat(ui, false);
                                        });
                                }
                            }
                        }

                    });
                }])
        .controller("functionmenu",
            [
                '$timeout',
                "blockUI",
                "$rootScope",
                "$scope",
                "appSettings",
                "loginservice",
                "userProfile",
                function (
                    $timeout,
                    blockUI,
                    $rootScope,
                    $scope,
                    appSettings,
                    loginservice,
                    userProfile)
                {
                    $scope.pData = [];
                    $scope.pgroupData = [];
                    $scope.ListItem = []
                    $scope.idfunction;
                    $scope.quyenuser = {};
                    $scope.onload = function (id) {
                        $scope.idfunction = id;
                    }

                    $scope.myClick = function (item) {
                        var respd;
                        respd = loginservice.postdata("api/Account/Logout");
                        respd.then(function (response) {
                            userProfile.clearall();
                            window.location.href = appSettings.serverPath + appSettings.serverLogin;
                        }, function errorCallback(response) {
                            userProfile.clearall();
                            window.location.href = appSettings.serverPath + appSettings.serverLogin;
                        });
                    };

                    getQuyen();

                    function getQuyen() {
                        var resp = loginservice.postdata("api/getCore/getAllPermissionsOfUserByModuleResource", $.param({ valstring1: "GCAL", valstring2: 'G', valstring3: 'M' }));
                        resp.then(function (response) {
                            $scope.quyenuser.TaoLich = response.data.findIndex(x => x.PermissionAction == 'U') > -1;
                            $scope.quyenuser.DuyetLich = response.data.findIndex(x => x.PermissionAction == 'A') > -1;
                            getGCALDuyet();
                        }
                            , function errorCallback(response) {
                            });
                    }

                    $scope.pshow = [];

                    $scope.$on('homethongbao', function (event, type) {
                        if (type == 1 || type == 3)
                            getCountDaXem();
                        else
                            getCountDuyetCv();
                    });

                    $scope.$on('homeGCAL', function (event, type) {
                        if (type == 1)
                            getGCALDuyet();
                        else (type == 2)
                        {
                            getGCALCaNhan();
                            getLichTuChoi();
                        }
                    });

                    //#region Private
                    function SteNotify(SID, Stype, Svalue) {
                        let i = $scope.ListItem.findIndex(x => x.ID == SID && x.type == Stype)
                        if (i > -1) {
                            $scope.ListItem.splice(i, 1);
                            $scope.ListItem.push({ ID: SID, type: Stype, value: Svalue })
                        }
                        else {
                            $scope.ListItem.push({ ID: SID, type: Stype, value: Svalue })
                        }
                    }

                    $scope.SteLich = [];

                    function SteIconNotify() {
                        var hamcho = function () {
                            if ($scope.pgroupData.length == 0) {
                                $timeout(hamcho, 300);
                            }
                            else {
                                let i = $scope.pgroupData.findIndex(x => x.par.ID == 9);
                                if (i > -1)
                                    $scope.pgroupData[i].par.ICON = 'nav-icon fas fa-calendar-week ' + ($scope.SteLich[0] > 0 ? 'text-warning' : ($scope.SteLich[1] > 0 ? 'text-info' : ''));
                            }
                        }
                        $timeout(hamcho, 300);
                    }

                    //#endregion
                    //#region workflow
                    getCountDaXem();

                    function getCountDaXem() {
                        var resp1 = loginservice.getdata("api/congviec/getdsDaxem");
                        resp1.then(function (response) {
                            SteNotify('P24P', 'badge-info', response.data);
                            dataInit();
                        },
                            function errorCallback(response) {
                            });
                    }

                    getCountDuyetCv();

                    function getCountDuyetCv() {
                        var resp1 = loginservice.getdata("api/congviec/getCountDuyet");
                        resp1.then(function (response) {
                            SteNotify('P24P', 'badge-warning', response.data);
                            dataInit();
                        },
                            function errorCallback(response) {
                            });
                    }
                    //#endregion

                    //#region GCAL
                    getGCALDuyet()
                    function getGCALDuyet() {
                        if ($scope.quyenuser.DuyetLich) {
                            var resp1 = loginservice.getdata("api/cal_sukien/getLichDuyet");
                            resp1.then(function (response) {
                                $scope.SteLich[0] = response.data;
                                SteNotify('P13P', 'badge-warning', response.data);
                                if (response.data > 0)
                                    SteIconNotify();
                                dataInit();
                            },
                                function errorCallback(response) {
                                });
                        }
                    }
                    //getGCALCaNhan()
                    function getGCALCaNhan() {
                        var respd = loginservice.postdata("api/cal_sukien/getSuKienUserByName", $.param({ valint1: 0 }));
                        respd.then(function (response) {
                            SteNotify('P12P', 'badge-info', response.data);
                            dataInit();
                        }, function errorCallback(response) {
                        });
                    }

                    getLichTuChoi()

                    function getLichTuChoi() {
                        var resp1 = loginservice.getdata("api/cal_sukien/getLichTuChoi");
                        resp1.then(function (response) {
                            $scope.SteLich[1] = response.data;
                            SteNotify('P13P', 'badge-info', response.data);
                            if (response.data > 0)
                                SteIconNotify();
                            dataInit();
                        },
                            function errorCallback(response) {
                            });
                    }
                    //#endregion

                    //#region TinNhan
                    $scope.$on('getCountTinNhan', function (data) {
                        getCountTinNhan();
                    });

                    getCountTinNhan();

                    function getCountTinNhan() {
                        var respd = loginservice.getdata("api/tinnhan/getCountTinNhan");
                        respd.then(function (response) {
                            $rootScope.TinNhanTBChuaXem = response.data.valint1;
                            SteNotify('P25P', 'badge-info', response.data.valint1);
                            dataInit();
                        }, function errorCallback(response) {
                        });
                    }
                    //#endregion TinNhan

                    getdatagroupchucnang();
                    function getdatagroupchucnang() {
                        var respd = loginservice.getdata("api/userinfo/getUsersMenu");
                        respd.then(function (response) {
                            $scope.pgroupData = checkgroupParent(response.data);
                        }, function errorCallback(response) {
                            $scope.pgroupData = [];
                        });
                    }

                    function checkgroupParent(item) {
                        return item.filter(function (st) {
                            $scope.activelink[st.par.ID] = "";
                            $scope.activegroup[st.par.ID] = "";
                            if (st.par.LINKS) {
                                if ($scope.url.indexOf(st.par.LINKS) >= 0) {
                                    $scope.activelink[st.par.ID] = "active";
                                    $scope.activegroup[st.par.ID] = "menu-is-opening menu-open";
                                }
                            }
                            if (st.par.TYPE != 5)
                                st.par.LINKS = appSettings.serverPath + st.par.LINKS;
                            st.childitem = checkactive(st.childitem);
                            return st;
                        });

                    }

                    function checkactive(item) {
                        return item.filter(function (st) {
                            $scope.activelink[st.par.ID] = "";
                            if ($scope.url.indexOf(st.par.LINKS) >= 0) {
                                $scope.activelink[st.par.ID] = "active";
                                $scope.activelink[st.par.PARENTID] = "active";
                                $scope.activegroup[st.par.PARENTID] = "menu-is-opening menu-open";
                            }
                            if (st.par.TYPE != 5)
                                st.par.LINKS = appSettings.serverPath + st.par.LINKS;
                            st.childitem = checkactive(st.childitem);
                            return st;
                        });
                    }

                    $scope.url = window.location.href;

                    $scope.activegroup = {};

                    $scope.activelink = {};

                    $scope.show = function (id) {
                        for (var i = 0; i < $scope.pData.length; i++) {
                            if (id == $scope.pData[i].ID)
                                return true;
                        }
                        return true;
                    }

                    $scope.showgroup = function (id) {
                        for (var i = 0; i < $scope.pData.length; i++) {
                            if (id == $scope.pData[i].NHOMID)
                                return true;
                        }
                        return true;
                    }

                    function dataInit() {
                        $rootScope.countV = $scope.ListItem.filter(x => x.value != 0).length;
                    }
                }])
        .directive('treeMenuSte', function ($compile) {
            return {
                restrict: 'E',

                scope: {
                    localNodes: '=model',
                    localClick: '&click',
                    localG: '=activeg',
                    localL: '=activel'
                },
                link: function (scope, tElement, tAttrs, transclude) {
                    var maxLevels = (angular.isUndefined(tAttrs.maxlevels)) ? 10 : tAttrs.maxlevels;
                    scope.goPage = function (node) {
                        if (window.location.href != node) {
                            window.location.href = node;
                        }
                    }
                    scope.checkdropdown = function (node) {
                        if (node.length > 0) return "dropdown-toggle";
                    }
                    scope.checkHaveChild = function (node) {
                        if (node.length > 0) return true;
                    }

                    /////////////////////////////////////////////////
                    function renderChild(array, level, max) {
                        var text = '';
                        text += '<ul class="submenu" ng-if="checkHaveChild(' + array + ')">';
                        text += '<li href="{{i.par.LINKS}}" class="{{localL[i.par.ID]}}" ng-repeat="i in ' + array + '" >';
                        text += '<a ng-href="{{i.par.LINKS}}" ng-class="checkdropdown(i.childitem)">';
                        text += '<i class="menu-icon fa fa-caret-right"></i><span ng-bind="i.par.TEN"></span>';
                        text += '<b ng-if="i.childitem.length > 0" class="arrow fa fa-angle-down"></b></a>';
                        text += '<b class="arrow"></b>';
                        if (level < max && array.length > 0)
                            text += renderChild('i.childitem', level + 1, max);
                        else
                            text += '</li>';
                        text += '</ul>';
                        return text;
                    }
                    function renderTreeView(collection, level, max) {
                        var text = '';
                        text += '<li class="{{localG[item.par.ID]}}" ng-repeat="item in ' + collection + '" >';
                        text += '<a href="#" ng-class="checkdropdown(item.childitem)"><i class="{{item.par.ICON}}" ng-click="goPage(item.par.LINKS)"></i><span class="menu-text" ng-click="goPage(item.par.LINKS)">{{item.par.TEN}}</span><b ng-if="item.childitem.length > 0" class="arrow fa fa-angle-down"></b></a> ';
                        text += '<b class="arrow"></b>';

                        if (level < max) {
                            text += renderChild('item.childitem', level + 1, max);
                        } else {
                            text += '</li>';
                        }
                        return text;
                    }
                    try {
                        var text = '<ul class="nav nav-list">';
                        text += renderTreeView('localNodes', 1, maxLevels);
                        text += '</ul>';
                        tElement.html(text);
                        $compile(tElement.contents())(scope);
                    }
                    catch (err) {
                        tElement.html('<b>ERROR!!!</b> - ' + err);
                        $compile(tElement.contents())(scope);
                    }
                }
            };
        })
        .directive('treeMenuSteNew', function ($compile) {
            return {
                restrict: 'E',

                scope: {
                    localNodes: '=model',
                    localClick: '&click',
                    localG: '=activeg',
                    localL: '=activel',
                    localNoti: '=stenotify'
                },
                link: function (scope, tElement, tAttrs, transclude) {
                    var maxLevels = (angular.isUndefined(tAttrs.maxlevels)) ? 10 : tAttrs.maxlevels;
                    scope.goPage = function (node) {
                        if (window.location.href != node) {
                            window.location.href = node;
                        }
                    }
                    scope.checkdropdown = function (node) {
                        if (node.length > 0) return "dropdown-toggle";
                    }
                    scope.checkHaveChild = function (node) {
                        if (node.length > 0) return true;
                    }
                    scope.menu = function (node) {
                        var hideThis = document.getElementById('P' + node);
                        let item = angular.element(hideThis).attr('class');

                        if (item === 'nav-item menu-is-opening menu-open')
                            angular.element(hideThis).attr('class', 'nav-item');
                        if (item === 'nav-item')
                            angular.element(hideThis).attr('class', 'nav-item menu-is-opening menu-open');
                    }
                    scope.goPageP = function (node) {
                        scope.menu(node.ID);
                        if (node.TYPE == 5)
                            window.open(node.LINKS, '_blank');
                        else {
                            if (window.location.href != node.LINKS) {
                                window.location.href = node.LINKS;
                            }
                        }
                    }

                    scope.checkTYPE = function (type, ID) {
                        if (type == 1 || type == 5)
                            return 'nav-item ' + ID;
                        else if (type == 0)
                            return 'nav-header text-bold ' + ID;
                    }
                    /////////////////////////////////////////////////
                    function renderChild(array, level, max) {
                        var text = '';
                        text += '<ul class="nav nav-treeview" ng-if="checkHaveChild(' + array + ')">';
                        text += '<li class="nav-item {{localG[item.par.ID]" ng-repeat="item in ' + array + '" >';
                        text += '<a href="#" class="nav-link {{localL[item.par.ID]}}">';
                        text += "<i class='nav-icon fas fa-caret-right'></i> <p><span ng-click='goPage(item.par.LINKS)'>{{item.par.TEN}}</span> <span  class='badge {{i.type}} right' ng-repeat='i in localNoti | filter: { ID: &#39;P&#39;+item.par.ID +&#39;P&#39;, value: &#39;!0&#39; }'>{{i.value}}</span><i ng-if='item.childitem.length > 0' class='fas fa-angle-left right'></i></p></a>";
                        if (level < max && array.length > 0)
                            text += renderChild('item.childitem', level + 1, max);
                        else
                            text += '</li>';
                        text += '</ul>';
                        return text;
                    }
                    function renderTreeView(collection, level, max) {
                        var text = "";

                        text += "<li id='P{{item.par.ID}}' ng-class='checkTYPE(item.par.TYPE,localG[item.par.ID])' ng-repeat='item in " + collection + "' >";
                        text += "<span ng-if='item.par.TYPE == 0' ng-bind='item.par.TEN'></span><a ng-if='item.par.TYPE != 0' href='#' class='nav-link {{localL[item.par.ID]}}'><i id='Icon{{item.par.ID}}' class='{{item.par.ICON}}'></i> <p><span ng-click='goPageP(item.par)'>{{item.par.TEN}}</span> <span  class='badge {{i.type}} right' ng-repeat='i in localNoti | filter: {ID: &#39;P&#39;+item.par.ID+&#39;P&#39;,value: &#39;!0&#39;}'>{{i.value}}</span><i ng-click=menu(item.par.ID) ng-show='item.childitem.length > 0' class='fas fa-angle-left right'></i></p></a>";

                        if (level < max) {
                            text += renderChild('item.childitem', level + 1, max);
                        } else {
                            text += '</li>';
                        }
                        return text;
                    }
                    try {
                        var text = '<ul class="nav nav-pills nav-sidebar flex-column">';
                        text += renderTreeView('localNodes', 1, maxLevels);
                        text += '<li class="nav-item"><a href = "#" class="nav-link" ng-click=localClick()><i class="nav-icon fas fa-sign-out-alt"></i><p>Đăng xuất tài khoản</p></a></li>';
                        text += '</ul>';
                        tElement.html(text);
                        $compile(tElement.contents())(scope);
                    }
                    catch (err) {
                        tElement.html('<b>ERROR!!!</b> - ' + err);
                        $compile(tElement.contents())(scope);
                    }
                }
            };
        })
}());