(function () {
    "use strict";
    angular.module("oamsapp")
        .controller("ReportCtrl", [
            'thongbao',
            "$scope",
            "blockUI",
            "appSettings",
            "loginservice",
            "userProfile",
            function (
                thongbao,
                $scope,
                blockUI,
                appSettings,
                loginservice,
                userProfile)
            {
                var $ctrl = this;

                $scope._columns = [
                    { name: 'Field1', title: 'Từ ngày', mtype: 'date', mdata: '', mclass: 'col-md-6 col-12', mstyle: '' },
                    { name: 'Ma', title: 'Đến ngày', mtype: 'date', mclass: 'col-md-6 col-12', mstyle: '' },
                    //{ name: 'Ten', title: 'Tên Kệ', mtype: 'select', mdata: 'danhmuc.TypeVB',  mfilter: '1', mclass: 'col-lg-3 col-md-6 col-12', mstyle: '' },
                    //{ name: 'TypeCV', title: 'Tên viết tắt', mtype: 'select', mdata: 'danhmuc.FilterVB', mfilter: '1', mclass: 'col-lg-3 col-md-6 col-12', mstyle: '' },
                    //{ name: 'Ghichu', title: 'Ghi chú', mtype: 'text', mclass: 'col-12' },

                ];

                $scope.btnSource = {
                    option: 0,
                    btns: [
                        [
                            { action: 'add', title: 'Thêm mới', classBtn: 'btn-success', classIcon: 'fas fa-plus' },
                            { action: 'upd', title: 'Sửa', classBtn: 'btn-warning', classIcon: 'fa-edit fas' },
                            { action: 'del', title: 'Xóa', classBtn: 'btn-danger', classIcon: 'fa-backspace fas' },
                        ], [
                            { action: 'addvb', title: 'Thêm văn bản', classBtn: 'btn-success', classIcon: 'fas fa-plus' },
                            { action: 'down', title: 'Tải hồ sơ', classBtn: 'btn-info', classIcon: 'fas fa-download' },
                        ]
                    ],
                    isSearchAdv: true,
                };

                console.log(getParameterByName("typeReport"));

                $scope.tbcolumns = [
                    { name: 'CodeNumber', title: 'Số đến', mtype: 'text', mclass: '', mstyle: 'width: 10%;', },
                    { name: 'FileNotation', title: 'Số hiệu', mtype: 'text', mclass: '', mstyle: 'width: 10%;' },
                    { name: 'OrganName', title: 'BH', mtype: 'text', mclass: '', mstyle: 'width: 20%;' },
                    { name: 'IssuedDate', title: 'Ngày đến', mtype: 'date', mclass: '', mstyle: 'width: 20%;' },
                    { name: 'Subject', title: 'Trích yếu', mtype: 'text', mclass: '', mstyle: 'text-left' },
                ];

                $scope.tbbtn = [
                    { action: 'upd', title: '', mclass: 'btn-outline-primary', micon: 'fas fa-edit', mstyle: '' },
                    { action: 'det', title: '', mclass: 'btn-outline-primary', micon: 'fas fa-search', mstyle: '' },
                    { action: 'del', title: '', mclass: 'btn-outline-danger', micon: 'fas fa-trash', mstyle: '' },
                ];

                $scope.searchobj = {};

                $scope.testfun = function (item) {
                    console.log(item);
                }

                $scope.para = {};

                //LocVanBan();

                function LocVanBan() {
                    blockUI.start();
                    $scope.obj = {
                        valint1: 0,
                        valint2: 11,
                        valint3: 1,
                        valint4: 0,
                        valint5: 0,
                        valstring1: "CV Hành Chính",//valstring1: $scope.para.TypeName,
                        valstring2: $scope.para.NguoiKy,
                        valtime2: $scope.BeginDate == null ? null : $scope.BeginDate.toDateString(),
                        valtime3: $scope.EndDate == null ? null : $scope.EndDate.toDateString()
                    };
                    var resp = loginservice.postdata("api/QLVanBan/GetVB_BaoCao", $.param($scope.obj));
                    resp.then(function (response) {
                        blockUI.stop();
                        $scope.DsVanBan = response.data;
                        $scope.bigTotalItems = response.data.length;
                        $scope.itemsPerPage = response.data.length;
                    }
                        , function errorCallback(response) {
                            blockUI.stop();
                        });
                }
            }])
}());