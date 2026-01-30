angular
    .module('aims.shared.directives')
    .directive('checklistModel', checklistModel);

checklistModel.$inject = ['$parse', '$compile'];

function checklistModel($parse, $compile) {

    function contains(arr, item, comparator) {
        if (angular.isArray(arr)) {
            for (var i = arr.length; i--;) {
                if (comparator(arr[i], item)) return true;
            }
        }
        return false;
    }

    function add(arr, item, comparator) {
        arr = angular.isArray(arr) ? arr : [];
        if (!contains(arr, item, comparator)) arr.push(item);
        return arr;
    }

    function remove(arr, item, comparator) {
        if (angular.isArray(arr)) {
            for (var i = arr.length; i--;) {
                if (comparator(arr[i], item)) {
                    arr.splice(i, 1);
                    break;
                }
            }
        }
        return arr;
    }

    function postLinkFn(scope, elem, attrs) {
        var checklistModel = attrs.checklistModel;
        attrs.$set('checklistModel', null);
        $compile(elem)(scope);
        attrs.$set('checklistModel', checklistModel);

        var checklistModelGetter = $parse(checklistModel);
        var ngModelGetter = $parse(attrs.ngModel);

        var comparator = angular.equals;

        var unbindModel = scope.$watch(attrs.ngModel, function (newValue, oldValue) {
            if (newValue === oldValue) return;

            var value = getChecklistValue();
            if (newValue) {
                checklistModelGetter.assign(scope.$parent, add(checklistModelGetter(scope.$parent), value, comparator));
            } else {
                checklistModelGetter.assign(scope.$parent, remove(checklistModelGetter(scope.$parent), value, comparator));
            }
        });

        function getChecklistValue() {
            return attrs.checklistValue
                ? $parse(attrs.checklistValue)(scope.$parent)
                : attrs.value;
        }

        scope.$on('$destroy', unbindModel);
    }

    return {
        restrict: 'A',
        priority: 1000,
        terminal: true,
        scope: true,
        compile: function (tElement, tAttrs) {
            if (!tAttrs.ngModel) {
                tAttrs.$set('ngModel', 'checked');
            }
            return postLinkFn;
        }
    };
}
