var app = angular.module("shipApp");
app.constant("FIREBASE_URL", "https://c4shiptool.firebaseio.com/dev/");
app.controller("ScanController", function ($scope, $modal, $filter, $timeout, ngTableParams, ScanOrderService, FirebaseDeliveryService, ngAudio, CURRENTUSER) {

    $scope.$watch('filter.$', function () {
        if (scan.Delivery && (scan.Delivery.ScannedItems || scan.Delivery.NotScannedItems)) {
            scan.TableParams.reload(); // TODO: Fix {scope: null} racecondition
            scan.TableParams2.reload(); // TODO: Fix {scope: null} racecondition
            scan.TableParams3.reload(); // TODO: Fix {scope: null} racecondition
        }
    });
    var scan = this;
    scan.ShouldSelect = false;
    scan.FocusDeliveryInput = true;
    scan.Colors = ['#ffb81e', '#2a767d', '#3ebebe', '#d85927', '#c6b912', '#7e6591', '#ca4346', '#67773f', '#f49630', '#aa8965', '#4fa0bf', '#b9e1e5', '#ffb81e', '#2a767d', '#3ebebe', '#d85927', '#c6b912', '#7e6591', '#ca4346', '#67773f', '#f49630', '#aa8965', '#4fa0bf', '#b9e1e5'];
    function _errorSound() {
        ngAudio.play("/content/error1.mp3");
        scan.SavingItem = false;
        scan.ShouldSelect = true;
    }
    function _successSound() {
        ngAudio.play("/content/success.mp3");
        scan.SavingItem = false;
        scan.ShouldSelect = true;
    }
    $scope.$watch("scan.Delivery.ScannedItems", function (newValue, oldValue) {
        if (newValue != oldValue) {
            scan.Delivery.$save();
            scan.TableParams2.reload();

        }
    });
    $scope.$watch("scan.Delivery.NotScannedItems", function (newValue, oldValue) {
        if (newValue != oldValue) {
            scan.Delivery.$save();
            scan.TableParams.reload();

        }
    });
    $scope.$watch("scan.Delivery.Kits", function (newValue, oldValue) {
        if (newValue != oldValue) {
            scan.Delivery.$save();
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

            if (scan.Delivery.ActiveKits && scan.Delivery.ActiveKits.length > 0) {
                scan.Delivery.ActiveKits.remove(_.where(scan.Delivery.ActiveKits, { Key: CURRENTUSER })[0]);
            }

        }
        scan.Delivery.$save();
    }

    function _processKit(scanItem) {
        //Find Kit Items for SerialCode
        if (scan.ActiveKit == null && scanItem.KitId > 0) {
            var matchedKitItems = _.where(scan.Delivery.NotScannedItems, { KitId: scanItem.KitId, KitCounter: scanItem.KitCounter });
            if (matchedKitItems.length > 0) {
                //Initialize ActiveKit
                if (scan.ActiveKit == null && scanItem.KitId > 0) {
                    scan.ActiveKit = [];
                    //Add the scan item to the active kit.
                    matchedKitItems.push(scanItem);
                    var newKit = { Key: CURRENTUSER, Value: [] };
                    _.each(matchedKitItems, function (item) {
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
        scan.Delivery.$save();
    }

    scan.ChartData = { Chart: null };
    scan.ChartFilter = { value: null, filter: null };
    scan.SavingItem = false;
    scan.LookUpIsInternal = false;
    scan.Username = null;
    scan.OrderIdLookUp = null;
    scan.Delivery = null;
    scan.TableParams = null;
    scan.SerialScanStatus = null;
    scan.DeliveryActionMessage = null;
    scan.IsSearching = false;
    scan.ShouldFocus = false;
    scan.TableParams = new ngTableParams({
        page: 1, // show first page
        count: 10, // count per page
        sorting: {
            SerialNum: 'asc' // initial sorting
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
            SerialNum: 'asc' // initial sorting
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
    scan.IsSaving = function (saving) {
        console.info("DAMIT: " + saving);
        return saving;
    };
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
                    var chart = [];
                    _.each(response.data.ChartData, function (item) {
                        chart.push({ data: item.Value, label: item.Key + "(" + item.Value + ")" });
                    });
                    scan.ChartData.Chart = chart;

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
                    scan.ChartFilter.value = scan.GetScanTotals();
                    scan.Delivery.$save();

                    $timeout(function () { scan.ShouldFocus = true; }, 500);
                } else {
                    scan.DeliveryActionMessage = "Delivery not found in SAP. Check delivery number.";
                    scan.FocusDeliveryInput = true;
                    scan.ShouldFocus = false;
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
                    scan.FocusDeliveryInput = true;
                    scan.ShouldFocus = false;
                }
            });
        }

    };
    scan.ClearDelivery = function (docNumber) {

        var modalInstance = $modal.open({
            templateUrl: '../scripts/templates/ClearDeliveryModal.html',
            controller: ClearModalCtrl,
            resolve: {
                docNum: function () {
                    return docNumber;
                }
            }
        });

        modalInstance.result.then(function () {
            ScanOrderService.ClearDelivery({ DeliveryNumber: docNumber, IsInternal: scan.Delivery.IsInternal }).then(function (result) {
                if (result.data === "true") {
                    FirebaseDeliveryService.Delete(scan.Delivery).then(function (ref) {
                        scan.Delivery.$destroy();
                        scan.Delivery = null;
                        scan.DeliveryActionMessage = "Successfully cleared delivery " + docNumber;
                        scan.FocusDeliveryInput = true;
                    });
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
            templateUrl: '../scripts/templates/ReturnDeliveryModal.html',
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
        scan.SavingItem = true;

        if (!serialCode) {
            scan.IsSearching = false;
            scan.ShouldFocus = true;
            scan.SavingItem = false;
            return false;
        }
        //get the color and product
        var productId = serialCode.substring(serialCode.length, serialCode.length - 7).substring(0, 5);
        var color = serialCode.substring(serialCode.length, serialCode.length - 7).substring(5, 7);
        var matched = null;

        // See if the item is in a Kit
        if (scan.ActiveKit != null && scan.ActiveKit.length > 0) {
            matched = _.find(scan.ActiveKit, function (match) { return match.ProductId == productId && match.Color == color && (!match.SerialCode || !match.ScannedBy); });
            scan.VerifyAndSaveScan(serialCode, matched, true);
        } else {

            //set matched item
            matched = _.find(scan.Delivery.NotScannedItems, function (item) { return item.ProductId == productId && item.Color == color; });
            if (matched) {
                scan.Delivery.NotScannedItems.remove(matched);
                scan.Delivery.$save();

                //Find all matches in the not scanned table
                var matchedListNotScanned = _.where(scan.Delivery.NotScannedItems, { ProductId: productId, Color: color });
                //find all matches in the scanned table
                var matchedListScanned = _.where(scan.Delivery.ScannedItems, { ProductId: productId, Color: color });
                var itemList = _.union(matchedListNotScanned, matchedListScanned);
                //find if there are single items
                var singleItemsExist = _.find(itemList, function (item) { return item.KitId === 0; });
                //find if there are kit items
                var kitItemsExist = _.find(itemList, function (item) { return item.KitId > 0; });
                //Do items exist in both single and kit items? Then isMixed = true;
                var isMixed = singleItemsExist && kitItemsExist;

                if (matchedListNotScanned.length > 0) {
                    //Delete this and in the if statement call isMixed
                    if (isMixed) {
                        //We know the item is mixed, so find if it has a single item that is scannable
                        var containsSingle = _.find(matchedListNotScanned, function (item) { return item.KitId == 0; });
                        if (containsSingle) {
                            //Found a single item that is scannable, SAVE IT!

                            scan.VerifyAndSaveScan(serialCode, containsSingle, false);
                        } else {
                            _errorSound();
                            scan.Delivery.NotScannedItems.push(matched);
                            //No more single items found, item must be in a kit because it still exists in the NotScannedTable
                            scan.SerialScanStatus = { Success: false, Select: true, Message: "You have scanned in a code that does not have a single item in the order. Did you mean to scan a kit item?" };
                            return false;
                        }
                    } else {

                        //Determine if the item is the primary key.Its not primary, force them to scan primary
                        if (matched.ItemCode.toLowerCase() != matched.RealItemCode.toLowerCase()) {
                            scan.Delivery.NotScannedItems.push(matched);
                            _errorSound();
                            scan.SerialScanStatus = { Success: false, Select: true, Message: "You have scanned in a code that does not have a single item in the order. Did you mean to scan a kit item?" };
                            return false;
                        } else {
                            //matched = matchedListNotScanned[0];
                            //scan.Delivery.NotScannedItems.remove(matched);
                            //Determine if the item is a kit item or not.
                            if (matched.KitId && matched.KitId > 0) {
                                scan.VerifyAndSaveScan(serialCode, matched, true);
                            } else {
                                //Item is a primary key or single item, SAVE IT!

                                scan.VerifyAndSaveScan(serialCode, matched, false);
                            }
                        }
                    };
                }
            } else {
                //Item code not found in not scanned table, or doesnt exist on order
                scan.Delivery.NotScannedItems.push(matched);
                _errorSound();
                scan.SerialScanStatus = { Success: false, Select: true, Message: "Item not found on order." };
                return false;
            }
        }
    };

    scan.VerifyAndSaveScan = function (serialCode, matched, isKitItem) {
        scan.ShouldSelect = false;
        //Make sure that there is a serialcode and a matched product
        if (serialCode && matched) {

            scan.SerialScanStatus = null;

            //If the the item is serializable or requires unique code
            var requiresUniqueScan = !matched.SmartCodeOnly && !matched.NoSerialRequired;

            if (requiresUniqueScan || (matched.SmartCodeOnly && !matched.NoSerialRequired)) {

                var modifiedMac = null;
                //If the item is serializable, check the length and create the modified mac id
                if ((matched.SmartCodeOnly && matched.NoSerialRequired)) {
                    modifiedMac = serialCode.substring(0, serialCode.length - 17);

                    if (modifiedMac.length != 12) {
                        if (modifiedMac.length != 16) {
                            _errorSound();
                            scan.Delivery.NotScannedItems.push(matched);
                            scan.SerialScanStatus = { Success: false, Select: true, Message: "You have scanned in a code that is not the correct length!" };
                            return false;
                        }
                    }
                }
                else {
                    //Otherwise just make the macid = serial code
                    modifiedMac = serialCode;
                }

                //If the scanned items table is null, create one.
                if (!scan.Delivery.ScannedItems) {
                    scan.Delivery.ScannedItems = [];
                }
                //Check all tables to see if the ID exists anywhere in the order already
                if (isKitItem) {
                    var _itemList;
                    if (scan.ActiveKit) {
                        _itemList = scan.ActiveKit.concat(scan.Delivery.ScannedItems);
                    } else {
                        _itemList = scan.Delivery.ScannedItems;
                    }

                    var foundItem = _.find(_itemList, function (item) { return item.SerialCode == serialCode; }) != null;
                    if (foundItem) {

                        scan.SavingItem = false;
                        scan.Delivery.NotScannedItems.push(matched);
                        scan.SerialScanStatus = { Success: false, Select: true, Message: "You have scanned in a code that already exists on this order!" };
                        scan.Delivery.$save();
                        _errorSound();
                        return false;
                    }
                }
                //Otherwise save it
                var deliveryItem = { IsInternal: scan.Delivery.IsInternal, SerialCode: serialCode, MacId: modifiedMac, Id: matched.Id, ProductGroup: matched.ProductGroup, IsUnique: requiresUniqueScan, SerialNum: matched.SerialNum, DocNum: scan.Delivery.$id };

                ScanOrderService.SaveDeliveryItem(deliveryItem).then(function (result) {
                    if (!result.data.ErrorMessage) {
                        if (!scan.Delivery.ScannedItems) {
                            scan.Delivery.ScannedItems = [];
                        }
                        matched.SerialCode = serialCode;
                        matched.ScannedBy = CURRENTUSER;
                        scan.Delivery.$save();
                        //Remove and update kit item to correct tables
                        if (isKitItem) {
                            _processKit(matched);
                            _cleanUpKit();
                        }
                            //Remove and Add non Kit Item to correct tables
                        else {
                            scan.Delivery.ScannedItems.push(matched);
                        }

                        matched.IsSelected = false;
                        scan.SerialCodeLookUp = null;
                        //Set the status to success
                        scan.SerialScanStatus = { Success: true, Message: "Serial Successfully Updated", Select: true };
                        _successSound();
                    }
                        // If there is an error message.
                    else {
                        scan.SerialScanStatus = { Success: false, Message: result.data.ErrorMessage + result.data.ErrorDeliveryNumber, Select: true };
                        scan.IsSearching = false;
                        scan.SerialCodeLookUp = null;
                        //put the item back into the not scanned table.
                        if (scan.Delivery.NotScannedItems && scan.Delivery.NotScannedItems.length >= 0) {
                            scan.Delivery.NotScannedItems.push(matched);
                        } else {
                            scan.Delivery.NotScannedItems = [];
                            scan.Delivery.NotScannedItems.push(matched);
                        }
                        scan.Delivery.$save();
                        _errorSound();
                    }
                    scan.SavingItem = false;
                    scan.SerialCodeLookUp = null;
                    scan.Delivery.$save();
                });
            }
        } else {
            if (matched) {
                scan.Delivery.NotScannedItems.pushed(matched);
                scan.Delivery.$save();
            }
            if (scan.ActiveKit == null) {
                _errorSound();
                scan.Delivery.NotScannedItems.push(matched);
                scan.SerialScanStatus = { Success: false, Message: "No items found that match that Serial Code. Verify Serial Code and try again", Select: true };

            } else {
                _errorSound();
                scan.Delivery.NotScannedItems.push(matched);
                scan.SerialScanStatus = { Success: false, Message: "No items found in the current kit that match this serial number. Please scan a serial code that matches the products in the current kit.", Select: true };

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
            if (selected.KitId > 0 && selected.IsVerified != 1) {
                //var ids = _.where(scan.Delivery.ScannedItems, { KitId: selected.KitId });

                ScanOrderService.ReturnSelectedItems(selected, scan.Delivery.IsInternal, scan.Delivery.DeliveryNumber).then(function (result) {
                    //TODO: Add success instead of just doing it.
                    if (result.data) {
                        _.each(selected, function (item) {
                            // delete item.IsSelected;
                            item.SerialCode = null;
                            item.ScannedBy = null;
                            scan.Delivery.ScannedItems.remove(item);
                            scan.Delivery.NotScannedItems.push(item);
                        });
                        scan.Delivery.$save();
                    }
                });
            } else if (selected.IsVerified != 1) {
                //var ids = _.pluck(selected, 'Id');
                ScanOrderService.ReturnSelectedItems(selected, scan.Delivery.IsInternal, scan.Delivery.DeliveryNumber).then(function (result) {
                    //TODO: Add success instead of just doing it.
                    if (result.data) {
                        _.each(selected, function (item) {
                            // delete item.IsSelected;
                            item.SerialCode = null;
                            item.ScannedBy = null;
                            scan.Delivery.ScannedItems.remove(item);
                            scan.Delivery.NotScannedItems.push(item);
                        });
                        scan.Delivery.$save();
                    }
                });
            }

        }
    };
    scan.VerifyDelivery = function (docNum) {
        var modalInstance = $modal.open({
            templateUrl: '../scripts/templates/VerifyDeliveryModal.html',
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
                    FirebaseDeliveryService.Delete(scan.Delivery).then(function (ref) {
                        scan.Delivery.$destroy();
                        scan.Delivery = null;
                        scan.DeliveryActionMessage = "Successfully verified delivery" + docNum;
                        scan.FocusDeliveryInput = true;
                    });
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
        var data = _.pluck(scan.Delivery.ScannedItems, "Id", "SerialCode", "ItemCode");

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
        var notScannedValid = _.find(scan.Delivery.NotScannedItems, function (item) {

            return (item.ProductId || item.SmartCodeOnly && item.NoSerialRequired || (item.SmartCodeOnly && !item.NoSerialRequired));
        }) == null;

        var noActiveKits = (scan.Delivery.ActiveKits == undefined || scan.Delivery.ActiveKits && scan.Delivery.ActiveKits.length == 0);

        return notScannedValid && noActiveKits;
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
    };
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

    Array.prototype.remove = function (elem) {
        var match = -1;

        while ((match = this.indexOf(elem)) > -1) {
            this.splice(match, 1);
        }
    }
});