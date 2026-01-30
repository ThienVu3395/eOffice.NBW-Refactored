angular
    .module('aims.shared.directives')
    .directive('plainTextMaxLength', plainTextMaxLength);

function plainTextMaxLength() {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            var maxLength;

            scope.$watch(attrs.plainTextMaxLength, function (newValue) {
                maxLength = newValue;
                ngModel.$validate();
            });

            ngModel.$validators.plainTextMaxLength = function (modelValue, viewValue) {
                if (!viewValue) return true;

                var val = viewValue.$$unwrapTrustedValue
                    ? viewValue.$$unwrapTrustedValue()
                    : viewValue;

                return val.length <= maxLength;
            };
        }
    };
}
