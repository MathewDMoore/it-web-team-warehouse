﻿angular.module('shipApp').service('ScanOrderService', function ($http, $templateCache, $log, SERVICES_PATH) {
    var scanOrderObject = {
        LookUp: function (orderId, isInternal) {

            return $http({ data: { DeliveryNumber: orderId, IsInternal: isInternal }, method: "POST", url: SERVICES_PATH + "OrderDeliveryService.svc/OrderLookUp" }).success(function (response) {
                return response;
            }).error(function (result) {
                $log.error("ScanOrderService -> OrderLookUp " + result);
            });
        },
        MatchAndSave: function (serialCode) {
            return $http({ data: serialCode, method: "POST", url: SERVICES_PATH + "OrderDeliveryService.svc/MatchAndSave" }).success(function (response) {
                return response;
            });
        },
        LookUpByMacId: function (macId) {

            return $http({ data: macId, method: "POST", url: SERVICES_PATH + "OrderDeliveryService.svc/LocateMacIds" }).success(function (response) {
                return response;
            }).error(function (result) {
                $log.error("ScanOrderService -> OrderLookUp " + result);
            });
        },
        ClearDelivery: function (delivery) {
            return $http({
                data: delivery, method: "POST", url: SERVICES_PATH + "OrderDeliveryService.svc/ClearDelivery"
            })
                .success(function (result) {
                    return result;
                });
        },
        ReturnDelivery: function (delivery) {
            return $http({
                data: delivery, method: "POST", url: SERVICES_PATH + "OrderDeliveryService.svc/ReturnDelivery"
            })
                .success(function (result) {
                    return result;
                });
        },
        ReturnSelectedItems: function (selected, isInternal, deliveryNumber) {
            return $http({ data: { SelectedList: selected, IsInternal: isInternal, DeliveryNumber: deliveryNumber }, method: "POST", url: SERVICES_PATH + "OrderDeliveryService.svc/ReturnDeliveryLineItem" })
                .success(function (result) {
                    return result;
                });
        },
        VerifyDelivery: function (deliveryId) {
            return $http({ data: deliveryId, method: "POST", url: SERVICES_PATH + "OrderDeliveryService.svc/VerifyDelivery" })
                .success(function (result) {
                    return result;
                });
        },
        UpdateScanByUser: function (item) {
            return $http({
                data: item,
                method: "POST",
                url: SERVICES_PATH + "OrderDeliveryService.svc/UpdateScanByUser"
            })
                .success(function (result) {
                    return result;
                });
        }
    };
    return scanOrderObject;

});