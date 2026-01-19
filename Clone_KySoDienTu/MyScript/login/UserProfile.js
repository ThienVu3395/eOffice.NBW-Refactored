(function () {
    "use strict"
    angular.module("aims.service", [
        'ui.sortable',
        'blockUI',
        'ui.knob',
        'angularBootstrapNavTree',
        'angularFileUpload',
        'ngAnimate',
        'ui.bootstrap',
        'ui.select',
        'ngSanitize',
        'SignalR',
        'pdfjsViewer',
        'chart.js',
        'dndLists'])
        .config(function (blockUIConfig, ChartJsProvider) {
            'blockUI',
                //blockUIConfigProvider.message('Fun with config');
                blockUIConfig.delay = 200;
            blockUIConfig.autoBlock = true;
            blockUIConfig.blockBrowserNavigation = true;
            //blockUIConfigProvider.template('<div class="block-ui-overlay">{{ message }}</div>');
            ChartJsProvider.setOptions({ colors: ['#803690', '#00ADF9', '#DCDCDC', '#46BFBD', '#FDB45C', '#949FB1', '#4D5360'] });
        })
        .filter('propsFilter', function () {
            return function (items, props) {
                var out = [];

                if (angular.isArray(items)) {
                    var keys = Object.keys(props);

                    items.forEach(function (item) {
                        var itemMatches = false;

                        for (var i = 0; i < keys.length; i++) {
                            var prop = keys[i];
                            var text = props[prop].toLowerCase();
                            if (item[prop].toString().toLowerCase().indexOf(text) !== -1) {
                                itemMatches = true;
                                break;
                            }
                        }

                        if (itemMatches) {
                            out.push(item);
                        }
                    });
                } else {
                    // Let the output be the input untouched
                    out = items;
                }

                return out;
            };
        })
        .constant("appSettings", Settings)
        .factory("userProfile", ["appSettings",
            function ($rootScope) {
                var setProfile = function (username, access_token, access_tokenFirebase, refresh_token, roleName) {
                    localStorage.setItem('username', username);
                    localStorage.setItem('accessToken', access_token);
                    localStorage.setItem('access_tokenFirebase', access_tokenFirebase);
                    localStorage.setItem('refreshToken', refresh_token);
                    localStorage.setItem('roleName', roleName);
                    var now = new Date().getTime();
                    localStorage.setItem('setupTime', now + (14 * 24 * 60 * 60 * 1000))
                };
                var setProfileExten = function (fileusername, ulrimage, manv, bophan, chucvu) {
                    localStorage.setItem('fileusername', fileusername);
                    localStorage.setItem('ulrimage', ulrimage);
                    localStorage.setItem('manhanvien', manv);
                    localStorage.setItem('bophan', bophan);
                    localStorage.setItem('chucvu', chucvu);
                };
                var getProfileExten = function () {
                    var profilee = {
                        fileusername: localStorage.getItem('fileusername'),
                        ulrimage: localStorage.getItem('ulrimage'),
                        manhanvien: localStorage.getItem('manhanvien'),
                        bophan: localStorage.getItem('bophan'),
                        chucvu: localStorage.getItem('chucvu'),
                    }
                    return profilee;
                };
                var getProfile = function () {
                    const item = localStorage.getItem('setupTime');
                    if (new Date().getTime() > item) {
                        clearall();
                        var profile = {
                            isLoggedIn: false
                        }
                        return profile;
                    }
                    else {
                        var profile = {
                            isLoggedIn: localStorage.getItem('accessToken') != null,
                            username: localStorage.getItem('username'),
                            access_token: localStorage.getItem('accessToken'),
                            access_tokenFirebase: localStorage.getItem('access_tokenFirebase'),
                            refresh_token: localStorage.getItem('refreshToken'),
                            roleName: localStorage.getItem('roleName'),

                            fullname: localStorage.getItem('fileusername'),
                            manhanvien: localStorage.getItem('manhanvien')
                        }
                        return profile;
                    }
                }
                var addShopingcart = function (typeitem, iditem) {
                    var i = typeitem + "-" + iditem + ";";
                    if (localStorage.getItem('countitemsShopingcart') == null) {
                        clearShopingcart();
                    }
                    var items = getShopingcart();
                    var count = getcountShoping() + 1;
                    items = items + i;
                    localStorage.setItem('countitemsShopingcart', count);
                    localStorage.setItem('itemsShopingcart', items);
                    //NumItem.prepForBroadcast(count);
                };
                var removeShopingcart = function (typeitem, iditem) {
                    var count = getcountShoping()
                    if (count >= 0) {
                        var i = typeitem + "-" + iditem + ";";
                        count--;
                        var items = getShopingcart();
                        items = items.replace(i, "");
                        localStorage.setItem('countitemsShopingcart', count);
                        localStorage.setItem('itemsShopingcart', items);
                    }
                };
                var getShopingcart = function () {
                    var items = localStorage.getItem('itemsShopingcart');
                    return items;
                };
                var getcountShoping = function () {
                    var _count = parseInt(localStorage.getItem('countitemsShopingcart'));
                    return _count;
                };
                var setShoping = function (n, items) {
                    localStorage.setItem('countitemsShopingcart', n);
                    localStorage.setItem('itemsShopingcart', items);
                };
                var clearShopingcart = function () {
                    localStorage.setItem('countitemsShopingcart', 0);
                    localStorage.setItem('itemsShopingcart', "");
                };
                var clearall = function () {
                    localStorage.removeItem("username");
                    localStorage.removeItem("accessToken");
                    localStorage.removeItem("access_tokenFirebase");
                    localStorage.removeItem("roleName");
                    localStorage.removeItem("countitemsShopingcart");
                    localStorage.removeItem("itemsShopingcart");
                    localStorage.removeItem("refreshToken");
                    localStorage.removeItem("item_projectactive");

                    localStorage.removeItem("fileusername");
                    localStorage.removeItem("manhanvien");
                    localStorage.removeItem("bophan");
                    localStorage.removeItem("chucvu");
                    //localStorage.removeItem("pdfjs.history");
                    //localStorage.removeItem("setupTime");
                    //localStorage.removeItem("ulrimage");
                };
                var saveitem = function (itemname, itemvalue) {
                    localStorage.setItem("item_" + itemname, itemvalue);
                };
                var getsaveitem = function (itemname) {
                    return localStorage.getItem("item_" + itemname);
                };
                var clearsaveitem = function (itemname) {
                    localStorage.removeItem("item_" + itemname);
                };
                return {
                    clearall: clearall,
                    setProfile: setProfile,
                    getProfile: getProfile,
                    addShopingcart: addShopingcart,
                    getShopingcart: getShopingcart,
                    clearShopingcart: clearShopingcart,
                    getcountShoping: getcountShoping,
                    removeShopingcart: removeShopingcart,
                    getProfileExten: getProfileExten,
                    setProfileExten: setProfileExten,
                    setShoping: setShoping,
                    saveitem: saveitem,
                    getsaveitem: getsaveitem,
                    clearsaveitem: clearsaveitem
                }
            }
        ])
        .factory("loginservice", [
            "appSettings",
            "userProfile",
            "$http",
            function (
                appSettings,
                userProfile,
                $http)
            {
                this.login = function (userlogin) {
                    var resp = $http({
                        url: appSettings.serverPath + "Token",
                        method: "POST",
                        data: $.param({ grant_type: 'password', username: userlogin.username, password: userlogin.password }),
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                    });
                    return resp;
                };

                this.loginapivpdt = function (userlogin) {
                    var resp1 = $http({
                        url: 'https://apivpdt.capnuocnhabe.vn/' + "Token",
                        //withCredentials: true,
                        method: "POST",
                        data: $.param({ grant_type: 'password', username: userlogin.username, password: userlogin.password }),
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                    });
                    return resp1;
                };

                this.logintong = function () {
                    var resp = $http({
                        url: 'https://api.sawaco.com.vn/token',
                        method: "POST",
                        data: $.param({ grant_type: 'password', username: 'hai.tv', password: '0908616744' }),
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                    });
                    return resp;
                }

                this.postdata = function (urlapi, data) {
                    var accesstoken = userProfile.getProfile().access_token;
                    var authHeaders = {};
                    if (accesstoken) {
                        authHeaders.Authorization = 'Bearer ' + accesstoken;
                    }
                    var resp = $http({
                        url: appSettings.serverPath + urlapi,
                        method: "POST",
                        data: data,
                        //data: $.param(data),
                        headers:
                        {
                            'Content-Type': 'application/x-www-form-urlencoded',
                            'Authorization': authHeaders.Authorization
                        },
                    });
                    return resp;
                };

                this.postdatajson = function (urlapi, data) {
                    var accesstoken = userProfile.getProfile().access_token;
                    var authHeaders = {};
                    if (accesstoken) {
                        authHeaders.Authorization = 'Bearer ' + accesstoken;
                    }
                    var resp = $http({
                        url: appSettings.serverPath + urlapi,
                        method: "POST",
                        data: JSON.stringify(data),
                        headers:
                        {
                            'Content-Type': 'application/json',
                            'Authorization': authHeaders.Authorization
                        },
                    });
                    return resp;
                };

                //Hoàng thêm
                this.postdatatokenfirebase = function (urlapi, data) {
                    var accesstoken = userProfile.getProfile().access_tokenFirebase;
                    var authHeaders = {};
                    if (accesstoken1) {
                        authHeaders.Authorization = 'Bearer ' + accesstoken;
                    }
                    var resp = $http({
                        url: 'https://apivpdt.capnuocnhabe.vn/' + urlapi,
                        method: "POST",
                        data: data,
                        //data: $.param(data),
                        headers:
                        {
                            'Content-Type': 'application/x-www-form-urlencoded',
                            'Authorization': authHeaders.Authorization
                        },
                    });
                    return resp;
                };

                this.getdatatokenfirebase = function (urlapi) {
                    var accesstoken = userProfile.getProfile().access_tokenFirebase;
                    var authHeaders = {};
                    if (accesstoken1) {
                        authHeaders.Authorization = 'Bearer ' + accesstoken;
                    }
                    var resp = $http({
                        url: 'https://apivpdt.capnuocnhabe.vn/' + urlapi,
                        method: "GET",
                        headers:
                        {
                            'Authorization': authHeaders.Authorization
                        },
                    });
                    return resp;
                };

                //Kết thúc hoàng thêm
                this.postdatatong = function (urlapi, data, accesstoken) {
                    var resp = $http({
                        url: urlapi,
                        method: "POST",
                        data: data,
                        headers:
                        {
                            'Content-Type': 'application/x-www-form-urlencoded',
                            'Authorization': accesstoken
                        },
                    });
                    return resp;
                };

                this.getdatatong = function (urlapi, accesstoken) {
                    var resp = $http({
                        url: urlapi,
                        method: "GET",
                        headers:
                        {
                            'Authorization': accesstoken
                        },
                    });
                    return resp;
                };

                this.getdata = function (urlapi) {
                    var accesstoken = userProfile.getProfile().access_token;
                    console.log(accesstoken);
                    var authHeaders = {};
                    if (accesstoken) {
                        authHeaders.Authorization = 'Bearer ' + accesstoken;
                        console.log(authHeaders.Authorization);
                    }
                    var resp = $http({
                        url: appSettings.serverPath + urlapi,
                        method: "GET",
                        headers:
                        {
                            'Authorization': authHeaders.Authorization
                        },
                    });
                    return resp;
                };

                this.postdataSmartCA = function (urlapi, data) {
                    var resp = $http({
                        url: appSettings.apiSmartCA_ServerPath + urlapi,
                        method: "POST",
                        data: data,
                        headers:
                        {
                            'Content-Type': 'application/x-www-form-urlencoded'
                        },
                    });
                    return resp;
                };

                this.postdataNormal = function (urlapi, data) {
                    var resp = $http({
                        url: appSettings.serverPath + urlapi,
                        method: "POST",
                        data: data,
                        headers: { 'Content-Type': 'application/json' }
                    });
                    return resp;
                };

                this.getdataNormal = function (urlapi) {
                    var resp = $http({
                        url: appSettings.serverPath + urlapi,
                        method: 'GET'
                    });
                    return resp;
                };

                // Lệnh điều xe
                this.getdataNormal_Than = function (urlapi) {
                    var resp = $http({
                        url: urlapi,
                        method: 'GET'
                    });
                    return resp;
                };
                // kết thúc lệnh điều xe

                this.getdatafile = function (urlapi) {
                    var accesstoken = userProfile.getProfile().access_token;
                    var authHeaders = {};
                    if (accesstoken) {
                        authHeaders.Authorization = 'Bearer ' + accesstoken;
                    }
                    var resp = $http({
                        url: appSettings.serverPath + urlapi,
                        method: "GET",
                        responseType: 'arraybuffer',
                        headers:
                        {
                            'foo': 'bar',
                            'Authorization': authHeaders.Authorization
                        },
                    });
                    return resp;
                };

                this.postdatafile = function (urlapi, data) {
                    var accesstoken = userProfile.getProfile().access_token;
                    var authHeaders = {};
                    if (accesstoken) {
                        authHeaders.Authorization = 'Bearer ' + accesstoken;
                    }
                    var resp = $http({
                        url: appSettings.serverPath + urlapi,
                        method: "POST",
                        data: $.param(data),
                        responseType: 'arraybuffer',
                        headers:
                        {
                            'foo': 'bar',
                            'Content-Type': 'application/x-www-form-urlencoded',
                            'Authorization': authHeaders.Authorization
                        },
                    });
                    return resp;
                };

                this.postdatafordownloadfile = function (urlapi, data) {
                    var accesstoken = userProfile.getProfile().access_token;
                    var authHeaders = {};
                    if (accesstoken) {
                        authHeaders.Authorization = 'Bearer ' + accesstoken;
                    }
                    var resp = $http({
                        url: appSettings.serverPath + urlapi,
                        method: "POST",
                        data: data,
                        responseType: 'arraybuffer',
                        headers:
                        {
                            'Content-Type': 'application/json',
                            'Authorization': authHeaders.Authorization
                        },
                    });
                    return resp;
                };

                this.deleteallfile = function () {
                    var accesstoken = userProfile.getProfile().access_token;
                    var authHeaders = {};
                    if (accesstoken) {
                        authHeaders.Authorization = 'Bearer ' + accesstoken;
                    }
                    var resp = $http({
                        url: appSettings.serverPath + "api/fileUpload/removeallfiletemp",
                        method: "GET",
                        headers:
                        {
                            'Authorization': authHeaders.Authorization
                        },
                    });
                    resp.then(function (response) {
                    }
                        , function errorCallback(response) {
                        });
                };

                this.postdatapdffile = function (urlapi, data) {
                    var accesstoken = userProfile.getProfile().access_token;
                    var authHeaders = {};
                    if (accesstoken) {
                        authHeaders.Authorization = 'Bearer ' + accesstoken;
                    }
                    var resp = $http({
                        url: appSettings.serverPath + urlapi,
                        method: "POST",
                        data: data,
                        responseType: 'arraybuffer',
                        headers:
                        {
                            'Content-Type': 'application/json',
                            'Accept': 'application/pdf',
                            'Authorization': authHeaders.Authorization
                        },
                    });
                    return resp;
                };

                return {
                    login: this.login,
                    loginapivpdt: this.loginapivpdt,
                    logintong: this.logintong,
                    postdata: this.postdata,
                    postdatajson: this.postdatajson,
                    postdatatokenfirebase: this.postdatatokenfirebase,
                    postdatatong: this.postdatatong,
                    postdataNormal: this.postdataNormal,
                    getdataNormal: this.getdataNormal,
                    getdataNormal_Than: this.getdataNormal_Than,
                    postdataSmartCA: this.postdataSmartCA,
                    getdata: this.getdata,
                    getdatatong: this.getdatatong,
                    getdatatokenfirebase: this.getdatatokenfirebase,
                    getdatafile: this.getdatafile,
                    postdatafile: this.postdatafile,
                    postdatafordownloadfile: this.postdatafordownloadfile,
                    postdatapdffile: this.postdatapdffile,
                    deleteallfile: this.deleteallfile
                }
            }
        ])
        .factory('ChatService',
            [
                "$http",
                "$rootScope",
                "$location",
                "Hub",
                "$timeout",
                "userProfile",
                "appSettings",
                function (
                    $http,
                    $rootScope,
                    $location,
                    Hub,
                    $timeout,
                    userProfile,
                    appSettings)
                {
                    var Chats = this;
                    //Chat ViewModel
                    var Chat = function (chat) {
                        if (!chat) chat = {};

                        var Chat = {
                            UserName: chat.UserName || 'UserX',
                            ChatMessage: chat.ChatMessage || 'MessageY'
                        }

                        return Chat;
                    }
                    Chats.UserList = [];
                    Chats.windowsChats = [];
                    //Hub setup
                    var accesstoken = userProfile.getProfile().access_token;
                    var hub = new Hub("chatHub", {
                        listeners: {
                            'onConnected': function (id, userName, allUsers, messages) {
                                Chats.CountNotification = messages.Notifications.length;
                                Chats.DataNotification = messages.Notifications;
                                Chats.UserList = messages.dataUserOnline;
                                Chats.id = id;
                                for (var i = 0; i < Chats.UserList.length; i++) {
                                    if (Chats.UserList[i].Fileimage == undefined || Chats.UserList[i].Fileimage == '') {
                                        Chats.UserList[i].Fileimage = appSettings.serverPath + 'Content/image/user.png';
                                    }
                                    else {
                                        Chats.UserList[i].Fileimage = appSettings.serverPath + Chats.UserList[i].Fileimage;
                                    }
                                    Chats.UserList[i].TimeUp = moment().add(5, 'hours').toDate();
                                }
                                $rootScope.$apply();
                                $rootScope.$broadcast('onConnectedF', id);
                            },
                            'onreceivedNotifications': function (messages) {
                                Chats.CountNotification = messages.length;
                                Chats.DataNotification = messages;
                                $rootScope.$apply();
                            },
                            'userNewconnected': function (messages) {
                                var k = false;
                                for (var i = 0; i < Chats.UserList.length; i++) {
                                    if (Chats.UserList[i].Name == messages.Name) {
                                        Chats.UserList[i].TimeUp = moment().add(5, 'hours').toDate();
                                        k = true;
                                    }
                                }
                                if (k == false) {
                                    var inew = messages;
                                    inew.Fileimage = "";
                                    if (inew.Fileimage == undefined || inew.Fileimage == '') {
                                        inew.Fileimage = appSettings.serverPath + 'Content/image/user.png';
                                    }
                                    else {
                                        inew.Fileimage = appSettings.serverPath + inew.Fileimage;
                                    }
                                    inew.TimeUp = moment().add(5, 'hours').toDate();
                                    Chats.UserList.push(inew);
                                }

                                $rootScope.$apply();
                            },
                            'userDisconnected': function (messages) {
                                for (var i = 0; i < Chats.UserList.length; i++) {
                                    if (Chats.UserList[i].Name == messages) {
                                        Chats.UserList[i].TimeUp = moment().add(1, 'minutes').toDate();
                                    }
                                }
                                $rootScope.$apply();
                            },
                            'received': function (messages) {
                                var k = false;
                                for (var i = 0; i < Chats.windowsChats.length; i++) {
                                    if (Chats.windowsChats[i].Name == messages.sender) {
                                        Chats.windowsChats[i].Conversations.push({ Name: messages.sender, content: messages.message });
                                        k = true;
                                        break;
                                    }
                                }
                                if (k == false) {
                                    for (var i = 0; i < Chats.UserList.length; i++) {
                                        if (Chats.UserList[i].Name == messages.sender) {
                                            var winnew = {};
                                            winnew.Name = messages.sender;
                                            winnew.Fileimage = Chats.UserList[i].Fileimage;
                                            winnew.FullName = Chats.UserList[i].FullName;
                                            winnew.Conversations = [];
                                            winnew.Conversations.push({ Name: messages.sender, content: messages.message });
                                            Chats.windowsChats.push(winnew);
                                            break;
                                        }
                                    }
                                }
                                $rootScope.$apply();
                                $rootScope.$broadcast('newWinchat', messages.sender);
                            },
                            'otherloginOntime': function (messages) {
                                $rootScope.$broadcast('otherloginOntime', messages.username);
                            },
                            "updatesend": function (messages) {
                                var k = false;
                                for (var i = 0; i < Chats.windowsChats.length; i++) {
                                    if (Chats.windowsChats[i].Name == messages.sendto) {
                                        Chats.windowsChats[i].Conversations.push({ Name: "_Me", content: messages.message });
                                        k = true;
                                        break;
                                    }
                                }
                                if (k == false) {
                                    for (var i = 0; i < Chats.UserList.length; i++) {
                                        if (Chats.UserList[i].Name == messages.sendto) {
                                            var winnew = {};
                                            winnew.Name = messages.sendto;
                                            winnew.Fileimage = Chats.UserList[i].Fileimage;
                                            winnew.FullName = Chats.UserList[i].FullName;
                                            winnew.Conversations = [];
                                            winnew.Conversations.push({ Name: "_Me", content: messages.message });
                                            Chats.windowsChats.push(winnew);
                                            break;
                                        }
                                    }
                                }
                                $rootScope.$apply();
                                $rootScope.$broadcast('newWinchat', messages.sendto);
                            },
                        },
                        methods: ['onConnected'],
                        queryParams: {
                            'token': accesstoken
                        },
                        rootPath: "/" + appSettings.serverApp + "signalr",
                        errorHandler: function (error) {
                        },
                        hubDisconnected: function () {
                            if (hub.connection.lastError) {
                                hub.connection.start();
                            }
                        },
                        transport: 'webSockets',
                        logging: true
                    });
                    Chats.CountNotification = "0";
                    Chats.all = [];
                    Chats.add = function (userName, chatMessage) {
                        Chats.all.push(new Chat({ UserName: userName, ChatMessage: chatMessage }));
                    }

                    Chats.send = function (userName, chatMessage) {
                        hub.invoke("Send", chatMessage, userName)
                            .done(function () {
                                console.log('Invocation of NewContosoChatMessage succeeded');
                            }).fail(function (error) {
                                //console.log('Invocation of NewContosoChatMessage failed. Error: ' + error);
                            });
                        send(userName, chatMessage);
                    }

                    return Chats;
                }]);
})();