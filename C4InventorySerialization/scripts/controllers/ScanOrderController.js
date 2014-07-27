var app = angular.module("shipApp");

app.controller("ScanController", [
        "$scope", "ngTableParams", "ScanOrderService", function ($scope, ngTableParams, ScanOrderService) {
            var scan = this;
            scan.OrderIdLookUp = null;
            scan.Delivery = null;
            scan.Data = [];
            scan.TableParams = null;
            scan.LookUp = function (orderId) {
                ScanOrderService.LookUp(orderId).then(function (response) {
                    scan.Delivery = response.data;

                });
                scan.TableParams = new ngTableParams({
                    page: 1, // show first page
                    count: 10 // count per page
                }, {
                    total: scan.Delivery.NotScannedItems.length, // length of data
                    getData: function ($defer, params) {
                        $defer.resolve(scan.Delivery.NotScannedItems.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                    }
                }); scan.TableParams2 = new ngTableParams({
                    page: 1, // show first page
                    count: 10 // count per page
                }, {
                    total: scan.Delivery.ScannedItems.length, // length of data
                    getData: function ($defer, params) {
                        $defer.resolve(scan.Delivery.ScannedItems.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                    }
                });
                
            }
            scan.VerifyLineitem = function (serialCode) {
                if (serialCode) {
                    var productId = serialCode.substring(serialCode.length, serialCode.length - 7).substring(0, 5);
                    var matched = _.where(scan.Delivery.NotScannedItems, { ProductId: productId, SerialNum: "" });
                    if (matched.length > 0) {
                        var modifiedMac = null;

                        if (!matched.SmartCodeOnly) {
                            modifiedMac = serialCode.substring(0, serialCode.length - 17);

                            if (modifiedMac.length != 12) {
                                if (modifiedMac.length != 16) {
                                    scan.SerialError = "You have scanned in a code that is not the correct length!";
                                }
                            }

                            var deliveryItem = { SerialCode: serialCode, MacId: serialCode, Id: matched.Id, ProductGroup: matched.ProductGroup };
                            ScanOrderService.SaveDeliveryItem(deliveryItem).then(function(result) {
                                if (!result.data.ErrorMessage) {
//                                    var copy = _.clone(matched);
                                    scan.Delivery.NotScannedItems = _.without(scan.Delivery.NotScannedItems,matched[0]);
                                    matched[0].SerialNum = serialCode;
                                    scan.Delivery.ScannedItems.push(matched[0]);
                                }
                            });
                        }
                    }
                }
            };
        }
]
);