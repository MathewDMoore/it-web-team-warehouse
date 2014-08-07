angular.module('shipApp').service('ScanOrderService', function ($http, $templateCache, $log) {
    var scanOrderObject = {
        LookUp: function(orderId) {

            return $http({ data: orderId, method: "POST", url: "/ship/services/OrderDeliveryService.svc/OrderLookUp" }).success(function(response) {
                return response;
            }).error(function(result) {
                $log.error("ScanOrderService -> OrderLookUp " + result);
            });
        },
        LookUpByMacId: function(macId) {

            return $http({ data: macId, method: "POST", url: "/ship/services/OrderDeliveryService.svc/LocateMacIds" }).success(function(response) {
                return response;
            }).error(function(result) {
                $log.error("ScanOrderService -> OrderLookUp " + result);
            });
        },
        SaveDeliveryItem: function(deliveryItem) {
            return $http({ data: deliveryItem, method: "POST", url: "/ship/services/OrderDeliveryService.svc/SaveDeliveryItem" })
                .success(function(result) {
                    return result;
                });
        },
        ClearDelivery: function(docNum) {
            return $http({ data: docNum, method: "POST", url: "/ship/services/OrderDeliveryService.svc/ClearDelivery" })
                .success(function(result) {
                    return result;
                });
        },
        ReturnSelectedItems: function(ids) {
            return $http({ data: ids, method: "POST", url: "/ship/services/OrderDeliveryService.svc/ReturnDeliveryLineItem" })
                .success(function(result) {
                    return result;
                });
        },
        VerifyDelivery: function(deliveryId) {
            return $http({ data: deliveryId, method: "POST", url: "ship/services/OrderDeliveryService.svc/VerifyDelivery" })
                .success(function(result) {
                    return result;
                });
        }
    };
    return scanOrderObject;

});