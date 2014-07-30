angular.module('shipApp').service('ScanOrderService', function ($http, $templateCache, $log) {
    var scanOrderObject = {};
    scanOrderObject.LookUp = function (orderID) {

        return $http({ data: orderID, method: "POST", url: "/ship/services/OrderDeliveryService.svc/OrderLookUp" }).success(function (response) {
            return response;
        }).error(function (result) {
            $log.error("AccountService -> GetOrderHistory " + result);
        });
    };
    scanOrderObject.SaveDeliveryItem = function (deliveryItem) {
        var data = { model: deliveryItem };
        return $http({ data: data, method: "POST", url: "/ship/services/OrderDeliveryService.svc/SaveDeliveryItem" })
            .success(function (result) {
                return result;
            });
    };
    scanOrderObject.ClearDelivery = function (docNum) {
        return $http({ data: docNum, method: "POST", url: "/ship/services/OrderDeliveryService.svc/ClearDelivery" })
            .success(function (result) {
                return result;
            });
    };
    scanOrderObject.ReturnSelectedItems = function (ids) {
        return $http({ data: { ids: ids }, method: "POST", url: "/ship/services/OrderDeliveryService.svc/ReturnDeliveryLineItem" })
            .success(function (result) {
                return result;
            });
    };

    return scanOrderObject;

});