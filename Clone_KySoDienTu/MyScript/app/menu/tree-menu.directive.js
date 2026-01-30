angular.module("aims")
    .directive("treeMenuSteNew", function ($compile, $window) {
        return {
            restrict: "E",
            scope: {
                model: "=",      // menu tree
                activel: "=",    // active link map
                activeg: "=",    // active group map
                notify: "=",     // badge list [{ID,type,value}]
                onLogout: "&"    // callback logout
            },
            link: function (scope, element, attrs) {

                var maxLevels = attrs.maxlevels || 10;

                // ================= ACTION =================
                scope.goPage = function (node) {
                    if (!node) return;

                    if (node.TYPE === 5) {
                        $window.open(node.LINKS, "_blank");
                    } else if ($window.location.href !== node.LINKS) {
                        $window.location.href = node.LINKS;
                    }
                };

                scope.toggleMenu = function (id) {
                    var el = document.getElementById("P" + id);
                    if (!el) return;

                    var cls = el.className;
                    el.className =
                        cls.indexOf("menu-open") > -1
                            ? "nav-item"
                            : "nav-item menu-is-opening menu-open";
                };

                scope.hasChild = function (items) {
                    return items && items.length > 0;
                };

                scope.nodeClass = function (node) {
                    if (node.TYPE === 0)
                        return "nav-header text-bold";
                    return "nav-item";
                };

                // ================= RENDER =================
                function renderChild(array, level) {
                    var html = "";
                    html += "<ul class='nav nav-treeview' ng-if='hasChild(" + array + ")'>";
                    html += "<li ng-repeat='item in " + array + "' class='nav-item'>";
                    html +=
                        "<a href='#' class='nav-link {{activel[item.par.ID]}}'>" +
                        "<i class='nav-icon fas fa-caret-right'></i>" +
                        "<p>" +
                        "<span ng-click='goPage(item.par)'>{{item.par.TEN}}</span>" +
                        "<span class='badge {{i.type}} right' " +
                        "ng-repeat='i in notify | filter:{ID: \"P\"+item.par.ID+\"P\", value:\"!0\"}'>" +
                        "{{i.value}}</span>" +
                        "<i ng-if='item.childitem.length>0' " +
                        "ng-click='toggleMenu(item.par.ID)' " +
                        "class='fas fa-angle-left right'></i>" +
                        "</p></a>";

                    if (level < maxLevels) {
                        html += renderChild("item.childitem", level + 1);
                    }

                    html += "</li></ul>";
                    return html;
                }

                function renderRoot() {
                    var html = "";
                    html += "<ul class='nav nav-pills nav-sidebar flex-column'>";

                    html +=
                        "<li ng-repeat='item in model' " +
                        "id='P{{item.par.ID}}' " +
                        "ng-class='nodeClass(item.par) + \" \" + activeg[item.par.ID]'>";

                    html +=
                        "<span ng-if='item.par.TYPE==0'>{{item.par.TEN}}</span>" +
                        "<a ng-if='item.par.TYPE!=0' href='#' class='nav-link {{activel[item.par.ID]}}'>" +
                        "<i class='{{item.par.ICON}}'></i>" +
                        "<p>" +
                        "<span ng-click='goPage(item.par)'>{{item.par.TEN}}</span>" +
                        "<span class='badge {{i.type}} right' " +
                        "ng-repeat='i in notify | filter:{ID:\"P\"+item.par.ID+\"P\",value:\"!0\"}'>" +
                        "{{i.value}}</span>" +
                        "<i ng-if='item.childitem.length>0' " +
                        "ng-click='toggleMenu(item.par.ID)' " +
                        "class='fas fa-angle-left right'></i>" +
                        "</p></a>";

                    html += renderChild("item.childitem", 1);
                    html += "</li>";

                    // logout
                    html +=
                        "<li class='nav-item'>" +
                        "<a href='#' class='nav-link' ng-click='onLogout()'>" +
                        "<i class='nav-icon fas fa-sign-out-alt'></i>" +
                        "<p>Đăng xuất tài khoản</p></a></li>";

                    html += "</ul>";
                    return html;
                }

                // ================= COMPILE =================
                try {
                    element.html(renderRoot());
                    $compile(element.contents())(scope);
                } catch (e) {
                    element.html("<b>Menu render error</b>");
                }
            }
        };
    });