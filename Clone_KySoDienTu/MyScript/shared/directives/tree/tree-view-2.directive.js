angular
    .module('aims.shared.directives')
    .directive('treeView2', treeView);

treeView.$inject = ['$compile'];

function treeView($compile) {
    return {
        restrict: 'E',
        scope: {
            localtemp: '=valtemp',
            localNodes: '=model',
            localClick: '&click'
        },
        link: function (scope, element, attrs) {

            var maxLevels = attrs.maxlevels || 10;
            var hasCheckBox = angular.isDefined(attrs.checkbox);

            scope.showHide = function (id) {
                var el = document.getElementById(id);
                if (!el) return;
                el.className = el.className === 'show' ? 'hide' : 'show';
            };

            scope.checkIfChildren = function (node) {
                return angular.isDefined(node.Users);
            };

            function renderTree(collection, level) {
                var html = '';
                html += '<li ng-repeat="n in ' + collection + '">';
                html += '<span ng-if="checkIfChildren(n)" class="show-hide" ng-click="showHide(1+n.MAPHONG)"><i class="fa fa-plus-square"></i></span>';

                if (hasCheckBox) {
                    html += '<input type="checkbox" ng-model="n.checked">';
                }

                html += '<span class="edit"><i class="fa fa-users"></i></span>';
                html += '<label>{{checkIfChildren(n) ? n.TENPHONG : n.FullName}}</label>';

                if (level < maxLevels) {
                    html += '<ul id="{{1+n.MAPHONG}}" class="hide" ng-if="checkIfChildren(n)">';
                    html += renderTree('n.Users', level + 1);
                    html += '</ul>';
                }

                html += '</li>';
                return html;
            }

            var template = '<ul class="tree-view-wrapper">' + renderTree('localNodes', 1) + '</ul>';
            element.html(template);
            $compile(element.contents())(scope);
        }
    };
}