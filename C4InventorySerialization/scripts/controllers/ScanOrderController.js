var app = angular.module("shipApp");

app.controller("ScanController", function ($scope, $modal, $filter, ngTableParams, ScanOrderService) {
    var scan = this;
    scan.OrderIdLookUp = null;
    scan.Delivery = null;
    scan.TableParams = null;
    scan.Data = [];
    scan.SerialScanStatus = null;
    scan.DeliveryActionMessage = null;
    scan.ScannedSearch = function(filter) {
        scan.TableParams2.filter(filter);
//        scan.TableParams2.reload();
    };
    scan.NotScannedSearch = function(filter) {
        scan.TableParams.filter(filter);
//        scan.TableParams.reload();
    };
    scan.LookUp = function (orderId) {
        if (orderId > 0) {
            scan.DeliveryActionMessage = null;
            scan.Delivery = null;
            scan.SerialError = null;

            ScanOrderService.LookUp(orderId).then(function(response) {
                scan.Delivery = response.data;
                _.each(scan.Delivery.ScannedItems, function(item) {
                    angular.extend(item, { IsSelected: false });
                });
                scan.TableParams = new ngTableParams({
                    page: 1, // show first page
                    count: 10 // count per page
                },
                {
                    total: 0, // length of data
                    getData: function($defer, params) {
                        var orderedData = params.sorting() ? $filter('orderBy')(scan.Delivery.NotScannedItems, params.orderBy()) : scan.Delivery.NotScannedItems;
                        orderedData = params.filter() ? $filter('filter')(orderedData, params.filter()) : orderedData;
                        $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                        params.total(orderedData.length);
                        $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                    }
                });
                scan.TableParams2 = new ngTableParams({
                    page: 1, // show first page
                    count: 10 // count per page
                }, {
                    total: 0, // length of data
                    getData: function($defer, params) {
                        var orderedData = params.sorting() ? $filter('orderBy')(scan.Delivery.ScannedItems, params.orderBy()) : scan.Delivery.ScannedItems;
                        orderedData = params.filter() ? $filter('filter')(orderedData, params.filter()) : orderedData;
                        $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                        params.total(orderedData.length);
                        $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                    }
                });
            });
        }
    };
    scan.ClearDelivery = function (docNumber) {

        var modalInstance = $modal.open({
            templateUrl: '/scripts/templates/ClearDeliveryModal.html',
            controller: ClearModalCtrl,
            resolve: {
                docNum: function () {
                    return docNumber;
                }
            }
        });

        modalInstance.result.then(function () {
            ScanOrderService.ClearDelivery(docNumber).then(function (result) {
                if (result.data) {
                    scan.Delivery = null;
                    scan.DeliveryActionMessage = "Successfully cleared delivery" + docNumber;
                } else {
                    alert("There was an error clearing this delivery.");
                }
            });
        }, function () {
            //$log.info('Modal dismissed at: ' + new Date());
        });

    };
    scan.VerifyLineitem = function (serialCode) {
        if (serialCode) {
            scan.SerialScanStatus = null;

            var modifiedMac = serialCode.substring(0, serialCode.length - 17);

            if (modifiedMac.length != 12) {
                if (modifiedMac.length != 16) {
                    scan.SerialScanStatus = { Success: false, Message: "You have scanned in a code that is not the correct length!" };
                    return false;
                }
            }
            var productId = serialCode.substring(serialCode.length, serialCode.length - 7).substring(0, 5);
            var color = serialCode.substring(serialCode.length, serialCode.length - 7).substring(5, 7);
            var matched = _.where(scan.Delivery.NotScannedItems, { ProductId: productId, Color: color });

            if (matched.length > 0) {

                if (!matched[0].SmartCodeOnly) {

                    var deliveryItem = { SerialCode: serialCode, MacId: serialCode, Id: matched[0].Id, ProductGroup: matched[0].ProductGroup };
                    ScanOrderService.SaveDeliveryItem(deliveryItem).then(function (result) {
                        if (!result.data.ErrorMessage) {
                            //                                    var copy = _.clone(matched);
                            scan.Delivery.NotScannedItems.pop(matched[0]);
                            matched[0].SerialCode = serialCode;
                            angular.extend(matched[0], { IsSelected: false });
                            scan.Delivery.ScannedItems.push(matched[0]);
                            scan.SerialCodeLookUp = null;
                            scan.TableParams.reload();
                            scan.TableParams2.reload();
                            scan.SerialScanStatus = { Success: true, Message: "Serial Successfully Updated" };
                        } else {
                            scan.SerialScanStatus = { Success: false, Message: result.data.ErrorMessage };

                        }
                    });
                }
            } else {
                scan.SerialScanStatus = { Success: false, Message: "No items found that match that Serial Code. Verify Serial Code and try again" };
            }
        }
    };
    scan.HasSelectedReturns = function () {
        var selected = _.where(scan.Delivery.ScannedItems, { IsSelected: true });
        return selected.length > 0;
    };
    scan.ReturnSelectedItems = function () {
        var selected = _.where(scan.Delivery.ScannedItems, { IsSelected: true });
        if (selected.length > 0) {
            var ids = _.pluck(selected, 'Id');
            ScanOrderService.ReturnSelectedItems(ids).then(function(result) {
                if (result.data) {
                    _.each(selected, function(item) {
                        delete  item.IsSelected;
                        scan.Delivery.ScannedItems.pop(item);
                        item.SerialCode = null;                       
                        scan.Delivery.NotScannedItems.push(item);
                    });
                    scan.TableParams.reload();
                    scan.TableParams2.reload();
                }
            });
        }
    };
    scan.VerifyDelivery = function () { };
    scan.ExportToExcel = function () { };
    scan.ExportMadId = function () { };
    scan.print = function () { };
});
var ClearModalCtrl = function ($scope, $modalInstance, docNum) {

    $scope.DocNum = docNum;
    $scope.ok = function () {
        $modalInstance.close();
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};