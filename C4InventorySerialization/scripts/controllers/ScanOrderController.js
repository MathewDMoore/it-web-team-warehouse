var app = angular.module("shipApp");
app.constant("FIREBASE_URL", "https://c4shiptool.firebaseio.com/");
app.controller("ScanController", function ($scope, $modal, $filter, $timeout, ngTableParams, ScanOrderService, FirebaseDeliveryService, CURRENTUSER) {

    $scope.$watch('filter.$', function () {
        if (scan.Delivery && (scan.Delivery.ScannedItems || scan.Delivery.NotScannedItems)) {
            scan.TableParams.reload(); // TODO: Fix {scope: null} racecondition
            scan.TableParams2.reload(); // TODO: Fix {scope: null} racecondition
            scan.TableParams3.reload(); // TODO: Fix {scope: null} racecondition
        }
    });
    var scan = this;
    $scope.$watch("scan.Delivery.ScannedItems", function (newValue, oldValue) {
        if (newValue != oldValue) {
            scan.TableParams2.reload();
        }
    });
    $scope.$watch("scan.Delivery.NotScannedItems", function (newValue, oldValue) {
        if (newValue != oldValue) {
            scan.TableParams.reload();
        }
    });
    $scope.$watch("scan.Delivery.Kits", function(newValue, oldValue) {
        if (newValue != oldValue) {
            scan.TableParams3.reload();
        }
    });
    function _cleanUpKit() {
        //Remove Active Kit if all scanned items are complete
        if (scan.ActiveKit && _.where(scan.ActiveKit, { SerialCode: null, SerialCode: '' }).length == 0) {
            _.each(scan.ActiveKit, function (kitRow) {
                scan.Delivery.ScannedItems.push(kitRow);
            });
            scan.ActiveKit = null;

            scan.Delivery.ActiveKits.pop(_.where(scan.Delivery.ActiveKits, { Key: CURRENTUSER })[0]);
        }
    }

    function _processKit(scanItem) {
        //Find Kit Items for SerialCode
        var matchedKitItems = _.where(scan.Delivery.NotScannedItems, { KitId: scanItem.KitId, KitCounter: scanItem.KitCounter });
        if (matchedKitItems.length > 0) {
            //Initialize ActiveKit
            if (scan.ActiveKit == null && scanItem.KitId > 0) {
                scan.ActiveKit = [];
                var newKit = { Key: CURRENTUSER, Value: [] };
                _.each(matchedKitItems, function (item) {
                    scan.Delivery.NotScannedItems.pop(item);
                    if (item.SerialCode == undefined) {
                        angular.extend(item, { SerialCode: null });
                    }
                    scan.ActiveKit.push(item);
                });
                if (!scan.Delivery.ActiveKits) {
                    newKit.Value = scan.ActiveKit;
                    scan.Delivery.ActiveKits = [];
                    scan.Delivery.ActiveKits.push(newKit);
                }

            }
        }
    }

    scan.SavingItem = false;
    scan.LookUpIsInternal = false;
    scan.Username = null;
    scan.OrderIdLookUp = null;
    scan.Delivery = null;
    scan.TableParams = null;
    scan.Data = [];
    scan.SerialScanStatus = null;
    scan.DeliveryActionMessage = null;
    scan.IsSearching = false;
    scan.TableParams = new ngTableParams({
        page: 1, // show first page
        count: 10, // count per page
        sorting: {
            SerialNum: 'asc'     // initial sorting
        }
    },
                {
                    total: 0, // length of data
                    getData: function ($defer, params) {
                        var orderedData = params.sorting() ? $filter('orderBy')(scan.Delivery.NotScannedItems, params.orderBy()) : scan.Delivery.NotScannedItems;
                        orderedData = params.filter() ? $filter('filter')(orderedData, params.filter()) : orderedData;
                        orderedData = orderedData || [];
                        params.total(orderedData.length);
                        $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                    },
                });
    scan.TableParams2 = new ngTableParams({
        page: 1, // show first page
        count: 10, // count per page
        sorting: {
            SerialNum: 'asc' // initial sorting
        }
    }, {
        total: 0, // length of data
        getData: function ($defer, params) {
            var orderedData = params.sorting() ? $filter('orderBy')(scan.Delivery.ScannedItems, params.orderBy()) : scan.Delivery.ScannedItems;
            orderedData = params.filter() ? $filter('filter')(orderedData, params.filter()) : orderedData;
            orderedData = orderedData || [];
            params.total(orderedData.length);
            $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
        },
    });
    scan.TableParams3 = new ngTableParams({
        page: 1, // show first page
        count: 10, // count per page
        sorting: {
            SerialNum: 'asc'     // initial sorting
        }
    },
                {
                    total: 0, // length of data
                    getData: function ($defer, params) {
                        var orderedData = params.sorting() ? $filter('orderBy')(scan.ActiveKit, params.orderBy()) : scan.ActiveKit;
                        orderedData = params.filter() ? $filter('filter')(orderedData, params.filter()) : orderedData;
                        orderedData = orderedData || [];
                        params.total(orderedData.length);
                        $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                    },
                });
    scan.GetScanTotals = function () {
        var matches = _.pluck(scan.Delivery.ActiveKits, "Value");
        var scannedCount = scan.Delivery.ScannedItems ? scan.Delivery.ScannedItems.length : 0;
        var notScannedCount = scan.Delivery.NotScannedItems ? scan.Delivery.NotScannedItems.length : 0;
        var kitItemsCount = 0;
        if (matches && matches.length > 0) {
            _.each(matches, function (match) {
                kitItemsCount += match.length;
            });
        }
        return scannedCount + notScannedCount + kitItemsCount;
    };

    scan.GetCurrentScan = function () {
        var matches = _.pluck(scan.Delivery.ActiveKits, "Value");
        var scannedCount = scan.Delivery.ScannedItems ? scan.Delivery.ScannedItems.length : 0;
        var kitItemsCount = 0;
        if (matches && matches.length > 0) {
            _.each(matches, function (match) {
                var activeScan = _.filter(match, function (item) { return item.ScannedBy; });
                kitItemsCount += activeScan.length;
            });
        }
        return scannedCount + kitItemsCount;
    };
    scan.ScannedSearch = function (filter) {
        scan.TableParams2.filter(filter);
    };
    scan.NotScannedSearch = function (filter) {
        scan.TableParams.filter(filter);
    };
    scan.ActiveKitSearch = function (filter) {
        scan.TableParams3.filter(filter);
    };
    scan.LookUp = function (orderId, isInternal) {
        scan.Delivery = null;
        scan.ActiveKit = null;
        if (!isNaN(orderId) && parseInt(orderId) > 0) {

            scan.IsSearching = true;
            scan.DeliveryActionMessage = null;
            scan.SerialError = null;

            ScanOrderService.LookUp(orderId, isInternal).then(function (response) {

                scan.SerialScanStatus = null;
                scan.OrderIdLookUp = null;
                scan.SerialCodeLookUp = null;
                scan.NotScannedFilter = null;
                scan.ScannedFilter = null;
                scan.IsSearching = false;

                if (response.data) {
                    scan.Delivery = FirebaseDeliveryService.GetDelivery(response.data.DeliveryNumber);
                    _.each(response.data.ScannedItems, function (item) {
                        angular.extend(item, { IsSelected: false });
                    });
                    _.each(response.data.NotScannedItems, function (item) {
                        angular.extend(item, { IsSelected: false, SerialCode: "", ScannedBy: null });
                    });
                    angular.extend(scan.Delivery, {
                        DeliveryNumber: response.data.DeliveryNumber,
                        DealerId: response.data.DealerId,
                        DealerName: response.data.DealerName,
                        Address: response.data.Address,
                        Comments: response.data.Comments,
                        NotScannedItems: response.data.NotScannedItems || [{}],
                        ScannedItems: response.data.ScannedItems || [{}],
                        IsVerified: response.data.IsVerified,
                        IsInternal: response.data.IsInternal,
                        ActiveKits: response.data.ActiveKits,
                    });
                    var activeKit = _.where(response.data.ActiveKits, { Key: CURRENTUSER });
                    if (activeKit && activeKit.length > 0) {
                        scan.ActiveKit = activeKit[0].Value;
                    }

                    scan.Delivery.$save();
//                    $timeout(function () {
//                        scan.TableParams.reload();
//                        scan.TableParams2.reload();
//                        scan.TableParams3.reload();
//                    }, 500);
                    //scan.Delivery.$watch(function () {

                    //});
                } else {
                    scan.DeliveryActionMessage = "Delivery not found in SAP. Check delivery number.";
                }
            });
        } else if (orderId) {
            scan.IsSearching = true;

            ScanOrderService.LookUpByMacId(orderId).then(function (result) {
                scan.IsSearching = false;

                if (result.data.DeliveryNumber > 0) {
                    scan.LookUp(result.data.DeliveryNumber);
                } else {
                    scan.DeliveryActionMessage = "Delivery Not Found";
                }
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
            ScanOrderService.ClearDelivery({ DeliveryNumber: docNumber, IsInternal: scan.Delivery.IsInternal }).then(function (result) {
                if (result.data) {
                    scan.Delivery = null;
                    scan.DeliveryActionMessage = "Successfully cleared delivery" + docNumber;
                } else {
                    scan.DeliveryActionMessage = "There was an error clearing this delivery.";
                }
            });
        }, function () {
            //$log.info('Modal dismissed at: ' + new Date());
        });

    };
    scan.ReturnDelivery = function (delivery) {

        var modalInstance = $modal.open({
            templateUrl: '/scripts/templates/ReturnDeliveryModal.html',
            controller: ReturnDeliveryModalCtrl,
            resolve: {
                docNum: function () {
                    return delivery.DeliveryNumber;
                }
            }
        });

        modalInstance.result.then(function () {
            ScanOrderService.ReturnDelivery({ DeliveryNumber: delivery.DeliveryNumber, IsInternal: delivery.IsInternal }).then(function (result) {
                if (result.data) {
                    scan.DeliveryActionMessage = "Successfully returned delivery items " + delivery.DeliveryNumber;
                    scan.LookUp(delivery.DeliveryNumber, delivery.IsInternal);
                } else {
                    scan.DeliveryActionMessage = "There was an error returning this delivery.";
                }
            });
        }, function () {
            //$log.info('Modal dismissed at: ' + new Date());
        });

    };
    scan.VerifyLineitem = function (serialCode) {
        if (serialCode && (serialCode.length == 29 || serialCode.length == 33)) {
            scan.SerialScanStatus = null;
            scan.SavingItem = true;

            var modifiedMac = serialCode.substring(0, serialCode.length - 17);

            if (modifiedMac.length != 12) {
                if (modifiedMac.length != 16) {
                    scan.SavingItem = false;
                    scan.SerialScanStatus = { Success: false, Select: true, Message: "You have scanned in a code that is not the correct length!" };
                    return false;
                }
            }
            var productId = serialCode.substring(serialCode.length, serialCode.length - 7).substring(0, 5);
            var color = serialCode.substring(serialCode.length, serialCode.length - 7).substring(5, 7);
            var matched = null;
            if (scan.ActiveKit != null && scan.ActiveKit.length > 0) {
                matched = _.filter(scan.ActiveKit, function (match) { return match.ProductId == productId && match.Color == color && (!match.SerialCode || !match.ScannedBy); });
            } else {
                matched = _.where(scan.Delivery.NotScannedItems, { ProductId: productId, Color: color });
            }
            if (matched.length > 0) {

                if (!matched[0].SmartCodeOnly) {

                    var deliveryItem = { IsInternal: scan.Delivery.IsInternal, SerialCode: serialCode, MacId: modifiedMac, Id: matched[0].Id, ProductGroup: matched[0].ProductGroup };
                    ScanOrderService.SaveDeliveryItem(deliveryItem).then(function (result) {
                        scan.SavingItem = false;

                        if (!result.data.ErrorMessage) {
                            scan.Delivery.ScannedItems = scan.Delivery.ScannedItems || [];
                            matched[0].SerialCode = serialCode;
                            matched[0].ScannedBy = CURRENTUSER;

                            _processKit(matched[0]);

                            //Remove and Add non Kit Item to correct tables
                            if (scan.ActiveKit == null) {
                                scan.Delivery.NotScannedItems.pop(matched[0]);
                                scan.Delivery.ScannedItems.push(matched[0]);
                            }
                            _cleanUpKit();
                            matched[0].IsSelected = false;
                            scan.SerialCodeLookUp = null;
                            scan.Delivery.$save();
//                            $timeout(function () {
//                                scan.TableParams.reload();
//                                scan.TableParams2.reload();
//                                scan.TableParams3.reload();
//                            }, 500);
                            scan.SerialScanStatus = { Success: true, Message: "Serial Successfully Updated", Select: true };


                        } else {
                            scan.SerialScanStatus = { Success: false, Message: result.data.ErrorMessage + result.data.ErrorDeliveryNumber, Select: true };

                        }
                    });
                }
                scan.SavingItem = false;

            } else {
                scan.SavingItem = false;

                if (scan.ActiveKit == null) {
                    scan.SerialScanStatus = { Success: false, Message: "No items found that match that Serial Code. Verify Serial Code and try again", Select: true };
                } else {
                    scan.SerialScanStatus = { Success: false, Message: "No items found in the current kit that match this serial number. Please scan a serial code that matches the products in the current kit.", Select: true };

                }
            }
        }
    };
    scan.HasSelectedReturns = function () {
        var selected = _.where(scan.Delivery.ScannedItems, { IsSelected: true });
        return selected.length > 0;
    };
    scan.ReturnSelectedItems = function () {
        var selected = _.where(scan.Delivery.ScannedItems, { IsSelected: true });
        scan.Delivery.NotScannedItems = scan.Delivery.NotScannedItems || [];
        if (selected.length > 0) {
            var ids = _.pluck(selected, 'Id');
            ScanOrderService.ReturnSelectedItems(ids, scan.Delivery.IsInternal).then(function (result) {
                if (result.data) {
                    _.each(selected, function (item) {
                        // delete item.IsSelected;
                        item.SerialCode = null;
                        item.ScannedBy = null;
                        scan.Delivery.ScannedItems.pop(item);
                        scan.Delivery.NotScannedItems.push(item);
                    });
                    scan.Delivery.$save();
//                    $timeout(function () {
//                        scan.TableParams.reload();
//                        scan.TableParams2.reload();
//                        //                        scan.TableParams3.reload();
//                    }, 500);
                }
            });
        }
    };
    scan.VerifyDelivery = function (docNum) {
        var modalInstance = $modal.open({
            templateUrl: '/scripts/templates/VerifyDeliveryModal.html',
            controller: VerifyModalCtrl,
            resolve: {
                docNum: function () {
                    return docNum;
                }
            }
        });
        modalInstance.result.then(function () {
            ScanOrderService.VerifyDelivery(docNum).then(function (result) {
                if (result.data) {
                    scan.Delivery = null;
                    scan.DeliveryActionMessage = "Successfully verified delivery" + docNum;
                } else {
                    alert("There was an error verifying this delivery.");
                }
            });
        }, function () {
            //$log.info('Modal dismissed at: ' + new Date());
        });
    };
    scan.ExportCSV = function () {
        var scannedItems = [];
        _.each(scan.Delivery.ScannedItems, function (item) {
            delete item.IsSelected;
            scannedItems.push(item);
        });

        var data = scannedItems.concat(scan.Delivery.NotScannedItems);

        var firstItem = data[0];
        var csvContent = "data:text/csv;charset=utf-8,";

        csvContent += _.keys(firstItem).join(",");
        csvContent += "\n";

        _.each(data, function (item, index) {
            var values = _.values(item);
            _.each(values, function (text, index) {
                if (_.isString(text)) {
                    values[index] = text.replace(",", " ");
                }
            });

            var dataString = values.join(",");
            csvContent += index < values.length ? dataString + "\n" : dataString;
        });

        var encodedUri = encodeURI(csvContent);
        var link = document.createElement("a");
        link.setAttribute("href", encodedUri);
        link.setAttribute("download", scan.Delivery.DeliveryNumber + "_export.csv");
        link.click();

    };
    scan.ExportMacId = function () {
        var data = _.pluck(scan.Delivery.ScannedItems, "SerialCode");

        var csvContent = "data:text/csv;charset=utf-8,";

        csvContent += "SerialCode \n";
        csvContent += data.join("\n");

        var encodedUri = encodeURI(csvContent);
        var link = document.createElement("a");
        link.setAttribute("href", encodedUri);
        link.setAttribute("download", scan.Delivery.DeliveryNumber + "_export.csv");
        link.click();

    };
    scan.GetDeliveryStatus = function () {
        // See if the order is ready to be validated.

        return scan.Delivery.IsVerified ? "Delivery Verified!" : "Delivery Not Verified!";
    }
    scan.IsScanComplete = function () {

        var notScannedValid = _.where(scan.Delivery.NotScannedItems, function (item) {
            return item.NoSerialRequired || !item.ReturnedByUser;
        });

        var activeKitValid = (!scan.Delivery.ActiveKits || scan.Delivery.ActiveKits.length == 0);
        return notScannedValid && activeKitValid;
    }
    scan.GetDeliveryStatusText = function () {
        var successful = scan.GetDeliveryStatus();
        if (successful) {
            return "Delivery Verified!";
        }
        return "Delivery Not Verified!";
    }
    scan.EnablVerification = function () {
        var items1 = _.filter(scan.Delivery.NotScannedItems, function (item) {
            return item.NoSerialRequired == false;
        });
        if (scan.Delivery.NotScannedItems.length == 0 || items1.length == 0) {
            return true;
        }
        return false;
    }
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


var VerifyModalCtrl = function ($scope, $modalInstance, docNum) {

    $scope.DocNum = docNum;
    $scope.ok = function () {
        $modalInstance.close();
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};
var ReturnDeliveryModalCtrl = function ($scope, $modalInstance, docNum) {

    $scope.DocNum = docNum;
    $scope.ok = function () {
        $modalInstance.close();
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};