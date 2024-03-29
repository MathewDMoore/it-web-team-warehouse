﻿var Directives = angular.module("myDirectives", []);

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
});