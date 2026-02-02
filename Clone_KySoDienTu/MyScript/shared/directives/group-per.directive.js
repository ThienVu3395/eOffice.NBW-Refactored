angular
    .module('aims.shared.directives')
    .directive('groupPer', groupPer);

groupPer.$inject = ['$compile'];

function groupPer($compile) {
    return {
        restrict: 'E',
        scope: {
            localtemp: '=val',
            localNodes: '=model',
            localClick: '&click'
        },
        link: function (scope, element, attrs) {

            var maxLevels = angular.isUndefined(attrs.maxlevels) ? 10 : attrs.maxlevels;
            var hasCheckBox = angular.isUndefined(attrs.checkbox) ? false : true;

            scope.showHide = function (ulId) {
                var el = document.getElementById(ulId);
                if (!el) return;
                var cls = el.className;
                el.className = cls === 'show' ? 'hide' : 'show';
            };

            scope.showIcon = function (node) {
                return !angular.isUndefined(node.children);
            };

            scope.checkIfChildren = function (node) {
                return !angular.isUndefined(node.groupPers);
            };

            scope.CheckAll = function (items) {
                items.forEach(function (item) {
                    if (scope.localtemp.findIndex(x => x.PermissionId === item.PermissionId) === -1) {
                        scope.localtemp.push(item);
                    }
                });
            };

            scope.UnCheckAll = function (items) {
                items.forEach(function (item) {
                    var idx = scope.localtemp.findIndex(x => x.PermissionId === item.PermissionId);
                    if (idx > -1) {
                        scope.localtemp.splice(idx, 1);
                    }
                });
            };

            function renderTree(collection, level) {
                var html = '';

                html += '<li ng-repeat="n in ' + collection + '">';
                html += '<span class="show-hide" ng-click="showHide(n.module.ModuleKey)">';
                html += '<i class="fa fa-plus-square"></i></span> ';
                html += '<span>{{n.module.ModuleName}}</span>';

                if (hasCheckBox) {
                    html += '<span class="tree-checkbox edit" ng-click="CheckAll(n.groupPers)">';
                    html += '<i class="fa fa-check-square"></i></span>';
                    html += '<span class="tree-checkbox edit" ng-click="UnCheckAll(n.groupPers)">';
                    html += '<i class="fa fa-times-circle"></i></span>';
                }

                if (level < maxLevels) {
                    html += '<ul id="{{n.module.ModuleKey}}" class="hide" ng-if="checkIfChildren(n)">';
                    html += '<li ng-repeat="item in n.groupPers">';
                    html += '<input class="tree-checkbox" type="checkbox" ';
                    html += 'checklist-model="localtemp" checklist-value="item">';
                    html += '<span>{{item.ResourceName}} - {{item.PermissionName}}</span>';
                    html += '</li></ul>';
                }

                html += '</li>';
                return html;
            }

            try {
                var template = '<ul class="tree-view-wrapper">';
                template += renderTree('localNodes', 1);
                template += '</ul>';

                element.html(template);
                $compile(element.contents())(scope);
            }
            catch (err) {
                element.html('<b>ERROR</b>: ' + err);
                $compile(element.contents())(scope);
            }
        }
    };
}
