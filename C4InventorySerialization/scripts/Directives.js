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
.directive("error", function () {
    return {
        scope: { Error: "=message" },
        link: function (scope, element, attrs) {
            $(element).popover({ placement: 'right' });
            scope.$watch("Error", function (newValue) {
                if (newValue && newValue.length>0) {
                    $(element).popover({ content:  scope.Error});

                    $(element).popover('show');
                } else {
                    $(element).popover('hide');
                }
            });
        }
    };
});