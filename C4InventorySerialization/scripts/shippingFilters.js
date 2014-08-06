angular.module('Shipping.Filters', [])
.filter('trustAsHtml', function ($sce) {
    return function (input) {
        return $sce.trustAsHtml(input);
    };
});