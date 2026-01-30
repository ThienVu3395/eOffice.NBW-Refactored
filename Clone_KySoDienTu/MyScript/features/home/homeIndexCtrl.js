angular.module("aims")
    .controller("homeIndexCtrl", [
        '$filter',
        '$http',
        '$window',
        "thongbao",
        "ModalService",
        "$scope",
        "appSettings",
        "loginservice",
        "userProfile",
        function (
            $filter,
            $http,
            $window,
            thongbao,
            ModalService,
            $scope,
            appSettings,
            loginservice,
            userProfile) {
            $scope.dataLich = [];

            $scope.dataCongViec = [];

            $scope.dsVB = [];

            $scope.dsTin = [];

            $scope.dsTinTong = [];

            $scope.tieude = ['CV lưu nháp', 'CV chờ duyệt', 'CV đang xử lý', 'CV hoàn thành'];

            $scope.url = "https://vpdt.sawaco.com.vn/";

            $scope.perPage = 3;

            $scope.maxSize = 3;

            $scope.currentPage = 1;

            $scope.loaitinnhan = 1;

            let isOpenModal = false;

            $scope.goPage = function (node) {
                window.location.href = appSettings.serverPath + node;
            }

            $scope.$on('loadAllTrangChu', function (data) {
                //thongbao.noImage("Thông Báo !!!", "Danh Sách Văn Bản Đã Được Cập Nhật");
                //$scope.getCongVan();
            });

            $scope.redirectSte = function (item) {
                $window.open(item, '_blank');
            };

            $scope.getLich = function () {
                var respd = loginservice.getdata("api/home/getLich");
                respd.then(function (response) {
                    $scope.dataLich = response.data;
                }, function errorCallback(response) {
                });
            }

            $scope.getLichCaNhan = function () {
                var respd = loginservice.getdata("api/home/getLichCaNhan");
                respd.then(function (response) {
                    $scope.dataLichCaNhan = response.data;
                }, function errorCallback(response) {
                });
            }

            $scope.getLichCaNhan();

            $scope.lichtong = [];

            var today = moment().format('YYYY/MM/DD');

            function getLichTong() {
                var resp = loginservice.logintong();
                resp.then(function (response) {
                    let accesstoken = response.data.access_token;
                    var authHeaders = {};
                    if (accesstoken) {
                        authHeaders.Authorization = 'Bearer ' + accesstoken;
                    }
                    var resp1 = $http({
                        url: 'https://api.sawaco.com.vn/api/LichCongViec/LichTongCongTy',
                        method: "POST",
                        data: $.param({ username: 'hoang.nm', filter: 1 }),
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded',
                            'Authorization': authHeaders.Authorization
                        },
                    });
                    resp1.then(function (response) {
                        $scope.lichtong = response.data;
                        angular.forEach(response.data.DuLieu.firstlastDayOfWeek2, function (j, index) {
                            if (moment(j).format('YYYY/MM/DD') == today) {
                                $scope.lichtong1 = response.data.DuLieu.listEventAM[index];
                                $scope.lichtong2 = response.data.DuLieu.listEventPM[index];
                                return;
                            }
                        });
                    }
                        , function errorCallback(response) {
                        });

                }
                    , function errorCallback(response) {
                    });

            }
            //getLichTong();

            $scope.chitiettong = function (item) {
                var abc = {};
                abc.Username = 'hoang.nm';
                abc.IDDoiTuong = item.ID;
                abc.week = $scope.lichtong.DuLieu.WeekOfYear;
                abc.year = $scope.lichtong.DuLieu.Year;
                abc.day = $filter('date')(item.StartDate, "dd");
                abc.month = $filter('date')(item.StartDate, "MM");
                abc.idclassify = 1;
                abc.classifyWeekDay = 1;
                abc.ismove = 1;

                ModalService.open({
                    templateUrl: 'lichchitiettong.html',
                    controller: 'lichchitiettongCtrl',
                    resolve: {
                        idselect: function () {
                            return abc;
                        }
                    }
                }).then(function () {
                }, function () {
                });
            }

            $scope.$on('homeGCAL', function (data) {
                $scope.getLich();
                $scope.getLichCaNhan();
            });

            $scope.filtercv = 0;

            $scope.getCongViec = function () {
                var respd = loginservice.postdata("api/congviec/getdscongviec", $.param({ valint1: 1, valint2: 4, valint3: $scope.filtercv, valint4: 5 }));
                respd.then(function (response) {
                    $scope.dscongviec = response.data.dscongviec;
                }, function errorCallback(response) {
                });
            }

            $scope.currentPageXL = 1;

            $scope.getCongViecXuLy = function () {
                var respd = loginservice.postdata("api/congviec/getdscongviec", $.param({ valint1: $scope.currentPageXL, valint2: 5, valint3: 0, valint4: 5 }));
                respd.then(function (response) {
                    $scope.dscongviecXL = response.data.dscongviec;
                    $scope.TotalCVXL = response.data.Total;
                }, function errorCallback(response) {
                });
            }

            $scope.getCongViecXuLy();

            $scope.pageChangedXL = function () {
                $scope.getCongViecXuLy();
            }

            $scope.filtercvchange = function (loc) {
                $scope.filtercv = loc;
                $scope.getCongViec();
            }

            $scope.$on('getAll', function (data) {
                $scope.getCongViec();
            });

            $scope.$on('homeIndex', function (data) {
                $scope.getCongViec();
            });

            $scope.getTinTuc = function () {
                var respd2 = loginservice.getdata("api/home/getTinTuc_TrangChu");
                respd2.then(function (response) {
                    $scope.dsTin = response.data;
                    localStorage.removeItem("VUvaries_ActionType");
                    localStorage.removeItem("VUvaries_IDAction");
                    localStorage.removeItem("VUvaries_CurrentPage");
                }, function errorCallback(response) {
                });
            }

            $scope.getTinTucTong = function () {
                var resp = loginservice.logintong();
                resp.then(function (response) {
                    let accesstoken = response.data.access_token;
                    var authHeaders = {};
                    if (accesstoken) {
                        authHeaders.Authorization = 'Bearer ' + accesstoken;
                    }
                    var resp1 = loginservice.postdatatong('https://api.sawaco.com.vn/api/tintucshare/list', $.param({ "Email": "hoang.nm", "offset": 0, "limit": 1 }), authHeaders.Authorization);
                    resp1.then(function (response) {
                        $scope.dsTinTong = response.data.DuLieu.ModelListNewsEventsCOByFilterAfter;
                    },

                        function errorCallback(response) {
                        }
                    );
                });
            }

            $scope.getCongVan = function (loai) {
                $scope.VLoai = loai;
                $scope.VName = "";
                $scope.VLink = "";
                switch (loai) {
                    case 0:
                        $scope.VLink = "Vanbanden?hanhchinh";
                        $scope.VName = " đến HC";
                        break;
                    case 1:
                        $scope.VLink = "Vanbandi?hanhchinh";
                        $scope.VName = " đi HC";
                        break;
                    case 2:
                        $scope.VLink = "Vanbanden?dangbo";
                        $scope.VName = " đến ĐB";
                        break;
                    case 3:
                        $scope.VLink = "Vanbandi?dangbo";
                        $scope.VName = " đi ĐB";
                        break;
                    case 4:
                        $scope.VLink = "Vanbanden?nddv";
                        $scope.VName = " đến NĐDV";
                        break;
                    case 5:
                        $scope.VLink = "Vanbandi?nddv";
                        $scope.VName = " đi NĐDV";
                        break;
                    default:
                        $scope.VLink = "Vanbanden?hanhchinh";
                        $scope.VName = " đến ĐB";
                        break;
                }
                var respd1 = loginservice.postdata("api/home/getVanBan", $.param({ valint1: loai }));
                respd1.then(function (response) {
                    $scope.dsVB = response.data;
                    //console.log($scope.dsVB);
                    if (response.data.length > 0) {
                        for (let i = 0; i < $scope.dsVB.length; i++) {
                            if ($scope.dsVB[i].LOAIXULY != null) {
                                let loaixuly = $scope.dsVB[i].LOAIXULY.split(",");
                                $scope.dsVB[i].ButPhe = loaixuly.findIndex(x => x == "1") > -1;
                                $scope.dsVB[i].TheoDoi = loaixuly.findIndex(x => x == "2") > -1;
                                $scope.dsVB[i].XuLyChinh = loaixuly.findIndex(x => x == "3") > -1;
                                $scope.dsVB[i].PhoiHop = loaixuly.findIndex(x => x == "4") > -1;
                            }
                        }
                    }
                }, function errorCallback(response) {
                });
            }

            $scope.getCongVanDangXuLy = function (sovb) {
                var respd1 = loginservice.postdata("api/home/getVanBanDangXuLy", $.param({ valint1: sovb }));
                respd1.then(function (response) {
                    $scope.dsVBXuLy = response.data;
                    if (response.data.length > 0) {
                        for (let i = 0; i < $scope.dsVBXuLy.length; i++) {
                            if ($scope.dsVBXuLy[i].LOAIXULY != null) {
                                let loaixuly = $scope.dsVBXuLy[i].LOAIXULY.split(",");
                                $scope.dsVBXuLy[i].ButPhe = loaixuly.findIndex(x => x == "1") > -1;
                                $scope.dsVBXuLy[i].TheoDoi = loaixuly.findIndex(x => x == "2") > -1;
                                $scope.dsVBXuLy[i].XuLyChinh = loaixuly.findIndex(x => x == "3") > -1;
                                $scope.dsVBXuLy[i].PhoiHop = loaixuly.findIndex(x => x == "4") > -1;
                            }
                        }
                    }
                }, function errorCallback(response) {
                });
            }

            $scope.getTinNhan = function (loaitinnhan) {
                var resp = loginservice.postdata("api/home/getTinNhan", $.param({ valint1: ($scope.currentPage - 1) * $scope.perPage, valint2: $scope.perPage, valstring1: null, valint3: 0, valtime: null, valtime2: null, valint4: loaitinnhan }));
                resp.then(function (response) {
                    //console.log(response.data);
                    $scope.events = response.data;
                    if ($scope.events.length > 0) {
                        $scope.TotalCV = $scope.events[0].Total;
                    }
                    else {
                        $scope.TotalCV = 0;
                    }
                    localStorage.removeItem("VUvaries_ActionType");
                    localStorage.removeItem("VUvaries_IDAction");
                    localStorage.removeItem("VUvaries_CurrentPage");
                },
                    function errorCallback(response) {
                        $scope.events = [];
                        $scope.TotalCV = 0;
                        $scope.perPage = 0;
                        $scope.pages = [];
                    });
            }

            $scope.getTaiLieu = function () {
                var respd2 = loginservice.getdata("api/home/getTaiLieu_TrangChu");
                respd2.then(function (response) {
                    $scope.dsTaiLieu = response.data;
                }, function errorCallback(response) {
                });
            }

            $scope.getSinhNhat = function () {
                var respd2 = loginservice.getdata("api/home/getSinhNhat");
                respd2.then(function (response) {
                    $scope.dsSinhNhat = response.data;
                }, function errorCallback(response) {
                });
            }

            getdataInnit();

            function getdataInnit() {
                $scope.toDay = new Date();
                $scope.datetime = $scope.toDay.getDate() + "/" + ($scope.toDay.getMonth() + 1) + "/" + $scope.toDay.getFullYear();
                $scope.getLich();
                $scope.getCongViec(0);
                $scope.getTinTuc();
                //$scope.getTinTucTong();
                $scope.getCongVan(-1);
                $scope.getCongVanDangXuLy(0);
                $scope.getTinNhan($scope.loaitinnhan);
                $scope.getTaiLieu();
                $scope.getSinhNhat();
            }

            $scope.lichchitiet = function (item, type) {
                let template = "";
                let controller = "";
                if (type == 1) {
                    template = "lichchitiet.html";
                    controller = "lichchitietCtrl";
                }
                else if (type == 2) {
                    template = "lichcanhan.html";
                    controller = "lichcanhanCtrl";
                }
                ModalService.open({
                    templateUrl: template,
                    controller: controller,
                    resolve: {
                        idselect: function () {
                            return item;
                        }
                    }
                }).then(function () {
                    $scope.getLichCaNhan();
                }, function () {
                    $scope.getLichCaNhan();
                });
            }

            $scope.ThemUserEvent = function () {
                ModalService.open({
                    templateUrl: '_CreateEditPUGEvent.html',
                    controller: 'createPUGEventCtrl',
                    resolve: {
                        idselect: function () {
                            return 0;
                        }
                    }
                }).then(function () {
                    $scope.getLichCaNhan();
                }, function () {
                });
            }

            $scope.congviecchitiet = function (item) {
                ModalService.open({
                    templateUrl: 'congviecchitiet.html',
                    controller: 'homeCongviecCtrl',
                    resolve: {
                        idselect: function () {
                            return item;
                        }
                    }
                }).then(function () {
                }, function () {
                });
            };

            $scope.MoFormChiTietVB = function (item, loaivb) {
                isOpenModal = true;
                ModalService.open({
                    templateUrl: loaivb == 0 ? 'formChiTietVBDen.html' : 'formChiTietVBDi.html',
                    controller: loaivb == 0 ? 'chiTietVBDenCtrl' : 'chiTietVBDiCtrl',
                    size: 'lg100',
                    resolve: {
                        idselect: function () {
                            let par = {};
                            par.isOpenModal = isOpenModal;
                            par.IDVanBan = item.ID;
                            par.LoaiVB = loaivb;
                            par.TypeName = item.TypeName;
                            return par;
                        }
                    }
                }).then(function (c) {
                    isOpenModal = false;
                    $scope.getCongVan($scope.VLoai);
                    $scope.getCongVanDangXuLy(0);
                }, function () {
                    isOpenModal = false;
                    $scope.getCongVan($scope.VLoai);
                    $scope.getCongVanDangXuLy(0);
                });
            }

            $scope.ViewUsersXuLyVB = function (dsNguoiThamGia) {
                ModalService.open({
                    templateUrl: 'modalViewUserXuLyVB.html',
                    controller: 'XemUserXuLyVBCtrl',
                    resolve: {
                        idselect: function () {
                            let item = {};
                            item.dsNguoiThamGia = dsNguoiThamGia;
                            return item;
                        }
                    }
                }).then(function () {
                }, function () {
                });
            }

            $scope.MoFormThemTinNhan = function (MessageID) {
                ModalService.open({
                    templateUrl: 'formThemSuaTinNhan.html',
                    controller: 'ThemSuaTinNhanCtrl',
                    resolve: {
                        idselect: function () {
                            let item = {};
                            item.MessageId = MessageID;
                            item.ParentID = 0;
                            return item;
                        }
                    }
                }).then(function () {
                }, function () {
                });
            };

            $scope.PhanTrangTinNhan = function () {
                $scope.getTinNhan($scope.loaitinnhan);
            };

            $scope.MarkImportantAll = function (item, IsImportant) { // đánh dấu quan trọng tin lớn
                var resp = loginservice.postdata("api/tinnhan/MarkImportantMessage", $.param({ valint1: item.MessageId, valint2: IsImportant }));
                resp.then(function (response) {
                    $scope.getTinNhan($scope.loaitinnhan);
                },
                    function errorCallback(response) {
                    });
            };

            $scope.ChiTietTinNhan = function (MessageId) {
                localStorage.setItem('VUvaries_IDAction', MessageId);
                localStorage.setItem('VUvaries_ActionType', $scope.loaitinnhan);
                localStorage.setItem('VUvaries_CurrentPage', $scope.currentPage);
                window.location.href = appSettings.serverPath + "/Tinnhan/Detail";
            };

            $scope.ChiTietTinTuc = function (item, loaitin) {
                localStorage.setItem('VUvaries_IDAction', loaitin == 1 ? JSON.stringify(item) : item);
                localStorage.setItem('VUvaries_ActionType', loaitin);
                localStorage.setItem('VUvaries_CurrentPage', 1);
                if (loaitin == 1)
                    window.location.href = appSettings.serverPath + "/BangTin/ViewSawaco";
                else if (loaitin == 6)
                    window.location.href = appSettings.serverPath + "/BangTin/ViewThongBao";
                else if (loaitin == 7)
                    window.location.href = appSettings.serverPath + "/BangTin/ViewSuKien";
                else
                    window.location.href = appSettings.serverPath + "/BangTin/ViewDangDoanThe";
            };

            $scope.GoToTinTuc = function (loaitin) {
                localStorage.setItem('VUvaries_ActionType', loaitin);
                localStorage.setItem('VUvaries_CurrentPage', 1);
                window.location.href = appSettings.serverPath + "/BangTin/Index";
            };

            $scope.GoToLichCoQuan = function () {
                window.location.href = appSettings.serverPath + "/Lichcoquan/Index";
            };

            $scope.GoToLichCaNhan = function () {
                window.location.href = appSettings.serverPath + "/Lichcanhan/Index";
            };

            $scope.GoToCongViec = function () {
                window.location.href = appSettings.serverPath + "/Congviec/Index";
            };

            $scope.GoToTaiLieu = function () {
                window.location.href = appSettings.serverPath + "/TaiLieu/Index";
            };

            $scope.GoToTinTucDangDoanThe = function (loaitin) {
                localStorage.setItem('VUvaries_ActionType', loaitin);
                localStorage.setItem('VUvaries_CurrentPage', 1);
                window.location.href = appSettings.serverPath + "/BangTin/DangDoanThe";
            };

            $scope.openFormTaiLieu = function (item, status) {
                let template = "";
                let controller = "themtailieuCtrl";
                switch (status) {
                    case 1:
                        template = "xemtailieu.html";
                        controller = "xemtailieuCtrl";
                        break;
                    default:
                        template = "themtailieu.html";
                        controller = "themtailieuCtrl";
                        break;
                }
                ModalService.open({
                    templateUrl: template,
                    controller: controller,
                    resolve: {
                        idselect: function () {
                            return item;
                        }
                    }
                }).then(function () {
                    $scope.getTaiLieu();
                }, function () {
                    $scope.getTaiLieu();
                });
            }
        }
    ]);