var app = angular.module("shipApp");

app.controller("ScanController", function ($scope, $modal, ngTableParams, ScanOrderService) {
    var scan = this;
    scan.OrderIdLookUp = null;
    scan.Delivery = null;
    scan.TableParams = null;
    scan.Data = [];
    scan.SerialError = null;
    scan.DeliveryActionMessage = null;
    scan.LookUp = function (orderId) {
        scan.DeliveryActionMessage = null;
        scan.SerialError = null;
        scan.SerialError = null;

        ScanOrderService.LookUp(orderId).then(function (response) {
            scan.Delivery = response.data;

            scan.TableParams = new ngTableParams({
                page: 1, // show first page
                count: 10 // count per page
            }, {
                total: scan.Delivery.NotScannedItems.length, // length of data
                getData: function ($defer, params) {
                    $defer.resolve(scan.Delivery.NotScannedItems.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                }
            });
            scan.TableParams2 = new ngTableParams({
                page: 1, // show first page
                count: 10 // count per page
            }, {
                total: scan.Delivery.ScannedItems.length, // length of data
                getData: function ($defer, params) {
                    $defer.resolve(scan.Delivery.ScannedItems.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                }
            });
        });

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
            var productId = serialCode.substring(serialCode.length, serialCode.length - 7).substring(0, 5);
            var matched = _.where(scan.Delivery.NotScannedItems, { ProductId: productId, SerialNum: "" });
            var modifiedMac = serialCode.substring(0, serialCode.length - 17);

            if (modifiedMac.length != 12) {
                if (modifiedMac.length != 16) {
                    scan.SerialError = "You have scanned in a code that is not the correct length!";
                }
            }

            if (matched.length > 0) {

                if (!matched.SmartCodeOnly) {

                    var deliveryItem = { SerialCode: serialCode, MacId: serialCode, Id: matched.Id, ProductGroup: matched.ProductGroup };
                    ScanOrderService.SaveDeliveryItem(deliveryItem).then(function (result) {
                        if (!result.data.ErrorMessage) {
                            //                                    var copy = _.clone(matched);
                            scan.Delivery.NotScannedItems = _.without(scan.Delivery.NotScannedItems, matched[0]);
                            matched[0].SerialNum = serialCode;
                            scan.Delivery.ScannedItems.push(matched[0]);
                        } else {
                            scan.SerialError = result.data.ErrorMessage;
                        }
                    });
                }
            }
        }
    };
}
);
var ClearModalCtrl = function ($scope, $modalInstance, docNum) {

    $scope.DocNum = docNum;
    $scope.ok = function () {
        $modalInstance.close();
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};