angular
    .module('aims.shared.directives')
    .directive('myFormSubmit', myFormSubmit);

function myFormSubmit() {
    return {
        require: '^form',
        scope: {
            callback: '&myFormSubmit'
        },
        link: function (scope, element, attrs, form) {
            element.on('click', function () {
                if (form.$valid) {
                    scope.$apply(scope.callback);
                }
            });
        }
    };
}