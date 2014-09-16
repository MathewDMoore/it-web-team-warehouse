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
.directive("autoSelect", function ($timeout) {
    return {
        scope: { Select: "=select", ShouldDisable: "=isSaving" },
        link: function (scope, element, attrs) {
            $timeout(function () { element.focus(); }, 800);

            scope.$watch("Select", function (newValue) {
                if (newValue) {
                    element.select();
                    element.focus();
                }
                scope.Select = false;
            });
            scope.$watch("ShouldDisable", function (newValue) {
                if (newValue) {
                    element.disable();
                }
            });
        }
    };
});