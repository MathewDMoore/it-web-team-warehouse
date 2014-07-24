angular.module('shipApp').service('ScanOrderService', function($http, $templateCache, $log) {
    var scanOrderObject = {};
    scanOrderObject.OrderIdLookUp = function(orderID) {
        return $http({ data: orderID, method: "POST", url: "/ship/services/OrderDeliveryService.svc/OrderLookUp" }).success(function(response) {
            return response;
        }).error(function(result) {
            $log.error("AccountService -> GetOrderHistory " + result);
        });
    };

    return scanOrderObject;

});