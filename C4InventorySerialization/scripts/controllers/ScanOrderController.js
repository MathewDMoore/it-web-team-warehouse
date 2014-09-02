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
    scan.LookUpIsInternal = false;
    scan.Username = null;
    scan.ScannedBy = null;
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
        if (parseInt(orderId) != NaN && parseInt(orderId) > 0) {

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
                        angular.extend(item, { IsSelected: false, SerialCode: "" });
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
                    });
                    var activeKit = _.where(response.data.ActiveKit, { Key: CURRENTUSER });
                    if (activeKit && activeKit.length > 0) {
                        scan.ActiveKit = activeKit[0];
                    }
                    
                    scan.Delivery.$save();
                    $timeout(function () {
                        scan.TableParams.reload();
                        scan.TableParams2.reload();
                        scan.TableParams3.reload();
                    }, 500);
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

                if (result.data > 0) {
                    scan.LookUp(result.data);
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
        if (serialCode) {
            scan.SerialScanStatus = null;

            var modifiedMac = serialCode.substring(0, serialCode.length - 17);

            if (modifiedMac.length != 12) {
                if (modifiedMac.length != 16) {
                    scan.SerialScanStatus = { Success: false, Select: true, Message: "You have scanned in a code that is not the correct length!" };
                    return false;
                }
            }
            var productId = serialCode.substring(serialCode.length, serialCode.length - 7).substring(0, 5);
            var color = serialCode.substring(serialCode.length, serialCode.length - 7).substring(5, 7);
            var matched = null;
            if (scan.ActiveKit != null && scan.ActiveKit.length > 0) {
                matched = _.where(scan.ActiveKit, { ProductId: productId, Color: color });
            } else {
                matched = _.where(scan.Delivery.NotScannedItems, { ProductId: productId, Color: color });
            }
            if (matched.length > 0) {
                
                //Create active kit table, if item scanned is a kit item
                if (!matched[0].SmartCodeOnly) {

               
                    var deliveryItem = { IsInternal: scan.Delivery.IsInternal, SerialCode: serialCode, MacId: modifiedMac, Id: matched[0].Id, ProductGroup: matched[0].ProductGroup };
                    ScanOrderService.SaveDeliveryItem(deliveryItem).then(function (result) {
                        if (!result.data.ErrorMessage) {
                            scan.Delivery.ScannedItems = scan.Delivery.ScannedItems || [];
                            matched[0].SerialCode = serialCode;
                            matched[0].ScannedBy = CURRENTUSER;
                            var matchedKitItems = null;
                            if (scan.ActiveKit==null && matched[0].KitId > 0) {
                                scan.ActiveKit = [];
                            }

                            if (scan.ActiveKit) {
                                matchedKitItems = _.where(scan.Delivery.NotScannedItems, { KitId: matched[0].KitId, KitCounter: matched[0].KitCounter });
                                _.each(matchedKitItems, function (item) {
                                    scan.Delivery.NotScannedItems.pop(item);
                                    scan.ActiveKit.push(item);
                                });
                            }
                            if (scan.ActiveKit == null) {
                                scan.Delivery.NotScannedItems.pop(matched[0]);
                                scan.Delivery.ScannedItems.push(matched[0]);
                            }
                            if (scan.ActiveKit && _.filter(scan.ActiveKit, function (kitRow) {
                                return kitRow.SerialCode != null;
                            }).length == 0) {
                                _.each(scan.ActiveKit, function (kitRow) {
                                    scan.Delivery.ScannedItems.push(kitRow);
                                });
                                scan.ActiveKit = null;
                            }

                            angular.extend(matched[0], { IsSelected: false });
                            scan.SerialCodeLookUp = null;
                            scan.Delivery.$save();
                            scan.TableParams.reload();
                            scan.TableParams2.reload();
                            scan.TableParams3.reload();
                            scan.SerialScanStatus = { Success: true, Message: "Serial Successfully Updated", Select: false };


                        } else {
                            scan.SerialScanStatus = { Success: false, Message: result.data.ErrorMessage + result.data.ErrorDeliveryNumber, Select: true };

                        }
                    });
                }
            } else {
                if (scan.ActiveKit == null) {
                    scan.SerialScanStatus = { Success: false, Message: "No items found that match that Serial Code. Verify Serial Code and try again", Select: true };
                } else {
                    scan.SerialScanStatus = { Success: false, Message: "No items found that match that Serial Code in the current kit. Complete kit scanned before proceeding", Select: true };

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
                        scan.Delivery.ScannedItems.pop(item);
                        scan.Delivery.NotScannedItems.push(item);
                    });
                    scan.Delivery.$save();
                    scan.TableParams.reload();
                    scan.TableParams2.reload();
                }
            });
        }
    };
    scan.VerifyDelivery = function (orderID) {
        //Verify all items are scanned
        if (scan.Delivery.IsVerified) {
            var modalInstance = $modal.open({
                templateUrl: '/scripts/templates/VerifyDeliveryModal.html',
                controller: VerifyModalCtrl,
                resolve: {
                    docNum: function () {
                        return docNumber;
                    }
                }
            });
            modalInstance.result.then(function () {
                ScanOrderService.VerifyDelivery(orderID).then(function (result) {
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
        }
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
        return scan.Delivery.IsVerified ? "Delivery Verified" : "Delivery Not Verified";
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