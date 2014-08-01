var Directives = angular.module("myDirectives", []);

Directives.directive('onEnter', function () {
    return {
        link: function (scope, element, attrs) {
            element.bind('keypress', function (key) {
                if (key.charCode === 9) {
                    key.preventDefault();
                    scope.$apply(attrs.onEnter + '()');
                    var inputs = $('input:text');
                    inputs[inputs.length - 1].focus();

                }
            });
        }
    };
})
.directive("autoSelect", function () {
    return {
        scope: { Select: "=select" },
        link: function (scope, element, attrs) {
            scope.$watch("Select", function (newValue) {
                if (newValue) {
                    element.select();
                    element.focus();
                }
                scope.Select = false;               
            });
        }
    };
});