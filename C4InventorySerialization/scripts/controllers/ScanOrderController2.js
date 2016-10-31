/// <reference path="../typings/angularjs/angular.d.ts" />
/// <reference path="../typings/underscore/underscore.d.ts" />
var ScanController = (function () {
    function ScanController($scope, $modal, $filter, $timeout, ngTableParams, ScanOrderService, FirebaseDeliveryService, ngAudio, CURRENTUSER) {
        var _this = this;
        this.$modal = $modal;
        this.$timeout = $timeout;
        this.ScanOrderService = ScanOrderService;
        this.FirebaseDeliveryService = FirebaseDeliveryService;
        this.ngAudio = ngAudio;
        this.CURRENTUSER = CURRENTUSER;
        this.Colors = ["#ffb81e", "#2a767d", "#3ebebe", "#d85927", "#c6b912", "#7e6591", "#ca4346", "#67773f", "#f49630", "#aa8965", "#4fa0bf", "#b9e1e5", "#ffb81e", "#2a767d", "#3ebebe", "#d85927", "#c6b912", "#7e6591", "#ca4346", "#67773f", "#f49630", "#aa8965", "#4fa0bf", "#b9e1e5"];
        this.ChartData = { Chart: null };
        this.ChartFilter = { value: null, filter: null };
        this.SavingItem = false;
        this.LookUpIsInternal = false;
        this.ShouldSelect = false;
        this.FocusDeliveryInput = true;
        this.IsSearching = false;
        this.ShouldFocus = false;
        this.OrderIdLookUp = null;
        this.SerialScanStatus = null;
        this.DeliveryActionMessage = null;
        this.SerialCodeLookUp = null;
        this.NotScannedFilter = null;
        this.ScannedFilter = null;
        this.SerialError = null;
        this.TableParams2 = null;
        this.TableParams3 = null;
        this.TableParams = null;
        this.TableParams = new ngTableParams({
            page: 1,
            count: 10,
            sorting: {
                SerialNum: "asc" // initial sorting
            }
        }, {
            total: 0,
            getData: function ($defer, params) {
                if (_this.Delivery.NotScannedItems && _this.Delivery.NotScannedItems.length > 0) {
                    var orderedData = params.sorting() ? $filter("orderBy")(_this.Delivery.NotScannedItems, params.orderBy()) : _this.Delivery.NotScannedItems;
                    orderedData = params.filter() ? $filter("filter")(orderedData, params.filter()) : orderedData;
                    orderedData = orderedData || [];
                    params.total(orderedData.length);
                    $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                }
            },
        });
        this.TableParams2 = new ngTableParams({
            page: 1,
            count: 10,
            sorting: {
                SerialNum: "asc" // initial sorting
            }
        }, {
            total: 0,
            getData: function ($defer, params) {
                if (_this.Delivery.ScannedItems && _this.Delivery.ScannedItems.length > 0) {
                    var orderedData = params.sorting() ? $filter("orderBy")(_this.Delivery.ScannedItems, params.orderBy()) : _this.Delivery.ScannedItems;
                    orderedData = params.filter() ? $filter("filter")(orderedData, params.filter()) : orderedData;
                    orderedData = orderedData || [];
                    params.total(orderedData.length);
                    $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                }
            },
        });
        this.TableParams3 = new ngTableParams({
            page: 1,
            count: 10,
            sorting: {
                SerialNum: "asc" // initial sorting
            }
        }, {
            total: 0,
            getData: function ($defer, params) {
                if (_this.ActiveKit && _this.ActiveKit.length > 0) {
                    var orderedData = params.sorting() ? $filter("orderBy")(_this.ActiveKit, params.orderBy()) : _this.ActiveKit;
                    orderedData = params.filter() ? $filter("filter")(orderedData, params.filter()) : orderedData;
                    orderedData = orderedData || [];
                    params.total(orderedData.length);
                    $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                }
            },
        });
        //$watches
        $scope.$watch("filter.$", function () {
            if (_this.Delivery && (_this.Delivery.ScannedItems || _this.Delivery.NotScannedItems)) {
                _this.TableParams.reload(); // TODO: Fix {scope: null} racecondition
                _this.TableParams2.reload(); // TODO: Fix {scope: null} racecondition
                _this.TableParams3.reload(); // TODO: Fix {scope: null} racecondition
            }
        });
    }
    //Properties
    //Private Functions
    ScanController.prototype._errorSound = function () {
        this.ngAudio.play("../content/error1.mp3");
        this.SavingItem = false;
        this.ShouldSelect = true;
    };
    ScanController.prototype._successSound = function () {
        this.ngAudio.play("../content/success.mp3");
        this.SavingItem = false;
        this.ShouldSelect = true;
        this.SaveAndRefresh(this.TableParams, this.TableParams2, this.TableParams3);
    };
    ScanController.prototype._cleanUpKit = function () {
        var _this = this;
        //Remove Active Kit if all scanned items are complete
        if (this.ActiveKit && _.where(this.ActiveKit, { SerialCode: null || "" }).length === 0) {
            this.Delivery.ScannedItems = (this.Delivery.ScannedItems || []);
            _.each(this.ActiveKit, function (kitRow) {
                _this.Delivery.ScannedItems.push(kitRow);
                _this.SaveAndRefresh(_this.TableParams2);
            });
            this.ActiveKit = null;
            if (this.Delivery.ActiveKits && this.Delivery.ActiveKits.length > 0) {
                this.Delivery.ActiveKits.remove(_.where(this.Delivery.ActiveKits, { Key: this.CURRENTUSER })[0]);
            }
        }
        this.Delivery.$save();
    };
    ScanController.prototype._processKit = function (scanItem) {
        var _this = this;
        //Find Kit Items for SerialCode
        if (this.ActiveKit == null && scanItem.KitId > 0) {
            var matchedKitItems = _.where(this.Delivery.NotScannedItems, { KitId: scanItem.KitId, KitCounter: scanItem.KitCounter });
            if (matchedKitItems.length > 0) {
                //Initialize ActiveKit
                if (this.ActiveKit == null && scanItem.KitId > 0) {
                    this.ActiveKit = [];
                    //Add the scan item to the active kit.
                    var newKit = { Key: this.CURRENTUSER, Value: [] };
                    _.each(matchedKitItems, function (item) {
                        _this.ScanOrderService.UpdateScanByUser({ SerialNum: item.SerialNum, DocNum: _this.Delivery.DeliveryNumber });
                        _this.ActiveKit = _this.ActiveKit || [];
                        _this.ActiveKit.push(item);
                        _this.Delivery.NotScannedItems.remove(item);
                    });
                    if (!this.Delivery.ActiveKits) {
                        newKit.Value = this.ActiveKit;
                        this.Delivery.ActiveKits = ([]);
                        this.Delivery.ActiveKits.push(newKit);
                    }
                }
            }
        }
        this.SaveAndRefresh(this.TableParams, this.TableParams3);
    };
    //Public Functions
    ScanController.prototype.GetScanTotals = function () {
        var matches = _.pluck(this.Delivery.ActiveKits, "Value");
        var scannedCount = this.Delivery.ScannedItems ? this.Delivery.ScannedItems.length : 0;
        var notScannedCount = this.Delivery.NotScannedItems ? this.Delivery.NotScannedItems.length : 0;
        var kitItemsCount = 0;
        if (matches && matches.length > 0) {
            _.each(matches, function (match) {
                kitItemsCount += match.length;
            });
        }
        return scannedCount + notScannedCount + kitItemsCount;
    };
    ScanController.prototype.GetCurrentScan = function () {
        var matches = _.pluck(this.Delivery.ActiveKits, "Value");
        var scannedCount = this.Delivery.ScannedItems ? this.Delivery.ScannedItems.length : 0;
        var kitItemsCount = 0;
        if (matches && matches.length > 0) {
            _.each(matches, function (match) {
                var activeScan = _.where(match, function (item) { return item.ScannedBy; });
                kitItemsCount += activeScan.length;
            });
        }
        return scannedCount + kitItemsCount;
    };
    ScanController.prototype.ScannedSearch = function (filter) {
        this.TableParams2.filter(filter);
    };
    ScanController.prototype.NotScannedSearch = function (filter) {
        this.TableParams.filter(filter);
    };
    ScanController.prototype.ActiveKitSearch = function (filter) {
        this.TableParams3.filter(filter);
    };
    ScanController.prototype.LookUp = function (orderId, isInternal) {
        var _this = this;
        this.Delivery = null;
        this.ActiveKit = null;
        if (!isNaN(orderId) && parseInt(orderId) > 0) {
            this.IsSearching = true;
            this.DeliveryActionMessage = null;
            this.SerialError = null;
            this.ScanOrderService.LookUp(orderId, isInternal).then(function (response) {
                _this.SerialScanStatus = null;
                _this.OrderIdLookUp = null;
                _this.SerialCodeLookUp = null;
                _this.NotScannedFilter = null;
                _this.ScannedFilter = null;
                _this.IsSearching = false;
                if (response.data) {
                    _this.Delivery = _this.FirebaseDeliveryService.GetDelivery(response.data.DeliveryNumber);
                    var chart = [];
                    _.each(response.data.ChartData, function (item) {
                        chart.push({ data: item.Value, label: item.Key + "(" + item.Value + ")" });
                    });
                    _this.ChartData.Chart = chart;
                    _.each(response.data.ScannedItems, function (item) {
                        angular.extend(item, { IsSelected: false });
                    });
                    _.each(response.data.NotScannedItems, function (item) {
                        angular.extend(item, { IsSelected: false });
                    });
                    angular.extend(_this.Delivery, {
                        DeliveryNumber: response.data.DeliveryNumber,
                        DealerId: response.data.DealerId,
                        DealerName: response.data.DealerName,
                        Address: response.data.Address,
                        Comments: response.data.Comments,
                        NotScannedItems: response.data.NotScannedItems || [],
                        ScannedItems: response.data.ScannedItems || [],
                        IsVerified: response.data.IsVerified,
                        IsInternal: response.data.IsInternal,
                        ActiveKits: response.data.ActiveKits,
                    });
                    var activeKit = _.where(response.data.ActiveKits, { Key: _this.CURRENTUSER });
                    if (activeKit && activeKit.length > 0) {
                        _this.ActiveKit = activeKit[0].Value;
                    }
                    _this.ChartFilter.value = _this.GetScanTotals();
                    _this.$timeout(function () { _this.ShouldFocus = true; }, 500);
                }
                else {
                    _this.DeliveryActionMessage = "Delivery not found in SAP. Check delivery number.";
                    _this.FocusDeliveryInput = true;
                    _this.ShouldFocus = false;
                }
                _this.SaveAndRefresh(_this.TableParams, _this.TableParams2, _this.TableParams3);
            });
        }
        else if (orderId) {
            this.IsSearching = true;
            this.ScanOrderService.LookUpByMacId(orderId).then(function (result) {
                _this.IsSearching = false;
                if (result.data.DeliveryNumber > 0) {
                    _this.LookUp(result.data.DeliveryNumber, result.data.IsInternal);
                }
                else {
                    _this.DeliveryActionMessage = "Delivery Not Found";
                    _this.FocusDeliveryInput = true;
                    _this.ShouldFocus = false;
                }
            });
        }
    };
    ScanController.prototype.ClearDelivery = function (docNumber) {
        var _this = this;
        var modalInstance = this.$modal.open({
            templateUrl: "../scripts/templates/ClearDeliveryModal.html",
            controller: ClearModalCtrl,
            resolve: {
                docNum: function () {
                    return docNumber;
                }
            }
        });
        modalInstance.result.then(function () {
            _this.ScanOrderService.ClearDelivery({ DeliveryNumber: docNumber, IsInternal: _this.Delivery.IsInternal }).then(function (result) {
                if (result.data) {
                    _this.FirebaseDeliveryService.Delete(_this.Delivery).then(function () {
                        _this.Delivery.$destroy();
                        _this.Delivery = null;
                        _this.DeliveryActionMessage = "Successfully cleared delivery " + docNumber;
                        _this.FocusDeliveryInput = true;
                    });
                }
                else {
                    _this.DeliveryActionMessage = "There was an error clearing this delivery.";
                }
            });
        }, function () {
            //$log.info('Modal dismissed at: ' + new Date());
        });
    };
    ScanController.prototype.ReturnDelivery = function (delivery) {
        var _this = this;
        var modalInstance = this.$modal.open({
            templateUrl: "../scripts/templates/ReturnDeliveryModal.html",
            controller: ReturnDeliveryModalCtrl,
            resolve: {
                docNum: function () {
                    return delivery.DeliveryNumber;
                }
            }
        });
        modalInstance.result.then(function () {
            _this.ScanOrderService.ReturnDelivery({ DeliveryNumber: delivery.DeliveryNumber, IsInternal: delivery.IsInternal }).then(function (result) {
                if (result.data) {
                    _this.DeliveryActionMessage = "Successfully returned delivery items " + delivery.DeliveryNumber;
                    _this.LookUp(delivery.DeliveryNumber, delivery.IsInternal);
                }
                else {
                    _this.DeliveryActionMessage = "There was an error returning this delivery.";
                }
            });
        }, function () {
        });
    };
    ScanController.prototype.VerifyLineitem = function (serialCode) {
        var _this = this;
        this.SavingItem = true;
        if (!serialCode) {
            this.IsSearching = false;
            this.ShouldFocus = true;
            this.SavingItem = false;
            return false;
        }
        var kitId = 0, kitCounter = 0;
        if (this.ActiveKit && this.ActiveKit.length > 0) {
            kitId = this.ActiveKit[0].KitId;
            kitCounter = this.ActiveKit[0].KitCounter;
        }
        this.ScanOrderService.MatchAndSave({ DocNumber: this.Delivery.DeliveryNumber, SerialCode: serialCode, IsInternal: this.LookUpIsInternal, KitId: kitId, KitCounter: kitCounter }).then(function (result) {
            _this.SavingItem = false;
            if (result.data.ErrorMessage) {
                _this.SerialScanStatus = { Success: false, Select: true, Message: result.data.ErrorMessage };
                _this._errorSound();
                return false;
            }
            var updatedItem = result.data.UpdatedItem;
            if (updatedItem) {
                var list = _this.ActiveKit ? _this.ActiveKit : _this.Delivery.NotScannedItems;
                var found = _.find(list, function (item) { return item.SerialNum === updatedItem.SerialNum; });
                found.ScannedBy = updatedItem.ScannedBy;
                found.SerialCode = serialCode;
                _this.Delivery.ScannedItems = (_this.Delivery.ScannedItems || []);
                if (found.KitId === 0) {
                    _this.Delivery.ScannedItems.push(found);
                    _this.Delivery.NotScannedItems.remove(found);
                    _this.SaveAndRefresh(_this.TableParams, _this.TableParams2);
                }
                else if (found.KitId > 0) {
                    _this._processKit(found);
                    _this._cleanUpKit();
                }
                _this.SerialCodeLookUp = null;
                //Set the status to success
                _this.SerialScanStatus = { Success: true, Message: "Serial Successfully Updated", Select: true };
                _this._successSound();
            }
        });
    };
    ScanController.prototype.HasSelectedReturns = function () {
        var selected = _.where(this.Delivery.ScannedItems, { IsSelected: true });
        return selected.length > 0;
    };
    ScanController.prototype.ReturnSelectedItems = function () {
        var _this = this;
        var selected = _.where(this.Delivery.ScannedItems, { IsSelected: true });
        this.Delivery.NotScannedItems = (this.Delivery.NotScannedItems || []);
        if (selected.length > 0) {
            if (selected[0].KitId > 0 && !selected[0].IsVerified) {
                this.ScanOrderService.ReturnSelectedItems(selected, this.Delivery.IsInternal, this.Delivery.DeliveryNumber).then(function (result) {
                    //TODO: Add success instead of just doing it.
                    _this.Delivery.NotScannedItems = (_this.Delivery.NotScannedItems || []);
                    if (result.data) {
                        _.each(selected, function (item) {
                            item.SerialCode = null;
                            item.ScannedBy = null;
                            item.IsSelected = false;
                            _this.Delivery.ScannedItems.remove(item);
                            _this.Delivery.NotScannedItems.push(item);
                            _this.SaveAndRefresh(_this.TableParams, _this.TableParams2);
                        });
                    }
                });
            }
            else if (!selected.filter(function (i) { return !i.IsVerified; })) {
                this.ScanOrderService.ReturnSelectedItems(selected, this.Delivery.IsInternal, this.Delivery.DeliveryNumber).then(function (result) {
                    //TODO: Add success instead of just doing it.
                    _this.Delivery.NotScannedItems = (_this.Delivery.NotScannedItems || []);
                    if (result.data) {
                        _.each(selected, function (item) {
                            item.SerialCode = null;
                            item.ScannedBy = null;
                            item.IsSelected = false;
                            _this.Delivery.ScannedItems.remove(item);
                            _this.Delivery.NotScannedItems.push(item);
                            _this.SaveAndRefresh(_this.TableParams, _this.TableParams2);
                        });
                    }
                });
            }
        }
    };
    ScanController.prototype.VerifyDelivery = function (docNum) {
        var _this = this;
        var modalInstance = this.$modal.open({
            templateUrl: "../scripts/templates/VerifyDeliveryModal.html",
            controller: VerifyModalCtrl,
            resolve: {
                docNum: function () {
                    return docNum;
                }
            }
        });
        modalInstance.result.then(function () {
            _this.ScanOrderService.VerifyDelivery(docNum).then(function (result) {
                if (result.data) {
                    _this.FirebaseDeliveryService.Delete(_this.Delivery).then(function (ref) {
                        _this.Delivery.$destroy();
                        _this.Delivery = null;
                        _this.DeliveryActionMessage = "Successfully verified delivery" + docNum;
                        _this.FocusDeliveryInput = true;
                    });
                }
                else {
                    alert("There was an error verifying this delivery.");
                }
            });
        }, function () {
        });
    };
    ScanController.prototype.ExportCSV = function () {
        var scannedItems = [];
        _.each(this.Delivery.ScannedItems, function (item) {
            delete item.IsSelected;
            scannedItems.push(item);
        });
        var data = scannedItems.concat(this.Delivery.NotScannedItems);
        var firstItem = data[0];
        var csvContent = "data:text/csv;charset=utf-8,";
        csvContent += _.keys(firstItem).join(",");
        csvContent += "\n";
        _.each(data, function (item) {
            var values = _.values(item);
            _.each(values, function (text, columnIndex) {
                if (_.isString(text)) {
                    values[columnIndex] = text.replace(",", " ");
                }
            });
            var dataString = values.join(",");
            csvContent += dataString + "\n";
        });
        var encodedUri = encodeURI(csvContent);
        var link = document.createElement("a");
        link.setAttribute("href", encodedUri);
        link.setAttribute("download", this.Delivery.DeliveryNumber + "_export.csv");
        link.click();
    };
    ScanController.prototype.GetDeliveryStatus = function () {
        return this.Delivery.IsVerified ? "Delivery Verified!" : "Delivery Not Verified!";
    };
    ScanController.prototype.IsScanComplete = function () {
        var notScannedValid = (this.Delivery.NotScannedItems && this.Delivery.NotScannedItems.length > 0) ? this.Delivery.NotScannedItems.filter(function (i) { return (!i.ProductId || i.SmartCodeOnly && i.NoSerialRequired || (i.SmartCodeOnly && !i.NoSerialRequired)); }).length === 0 : true;
        var noActiveKits = (this.Delivery.ActiveKits == undefined || this.Delivery.ActiveKits && this.Delivery.ActiveKits.length === 0);
        return notScannedValid && noActiveKits;
    };
    ScanController.prototype.GetDeliveryStatusText = function () {
        var successful = this.GetDeliveryStatus();
        if (successful)
            return "Delivery Verified!";
        return "Delivery Not Verified!";
    };
    ScanController.prototype.EnablVerification = function () {
        var items1 = _.filter(this.Delivery.NotScannedItems, function (item) {
            return !item.NoSerialRequired;
        });
        if (this.Delivery.NotScannedItems.length === 0 || items1.length === 0)
            return true;
        return false;
    };
    ScanController.prototype.SaveAndRefresh = function () {
        var tables = [];
        for (var _i = 0; _i < arguments.length; _i++) {
            tables[_i - 0] = arguments[_i];
        }
        for (var index in tables) {
            if (tables.hasOwnProperty(index))
                tables[index].reload();
        }
        this.Delivery.$save();
    };
    ScanController.$inject = ["$scope", "$modal", "$filter", "$timeout", "ngTableParams", "ScanOrderService", "FirebaseDeliveryService", "ngAudio", "CURRENTUSER"];
    return ScanController;
}());
var ClearModalCtrl = (function () {
    function ClearModalCtrl($scope, $modalInstance, docNum) {
        $scope.DocNum = docNum;
        $scope.ok = function () {
            $modalInstance.close();
        };
        $scope.cancel = function () {
            $modalInstance.dismiss("cancel");
        };
    }
    ClearModalCtrl.$inject = ["$scope", "$modalInstance", "docNum"];
    return ClearModalCtrl;
}());
var VerifyModalCtrl = (function () {
    function VerifyModalCtrl($scope, $modalInstance, docNum) {
        $scope.DocNum = docNum;
        $scope.ok = function () {
            $modalInstance.close();
        };
        $scope.cancel = function () {
            $modalInstance.dismiss("cancel");
        };
    }
    VerifyModalCtrl.$inject = ["$scope", "$modalInstance", "docNum"];
    return VerifyModalCtrl;
}());
var ReturnDeliveryModalCtrl = (function () {
    function ReturnDeliveryModalCtrl($scope, $modalInstance, docNum) {
        $scope.DocNum = docNum;
        $scope.ok = function () {
            $modalInstance.close();
        };
        $scope.cancel = function () {
            $modalInstance.dismiss("cancel");
        };
    }
    ReturnDeliveryModalCtrl.$inject = ["$scope", "$modalInstance", "docNum"];
    return ReturnDeliveryModalCtrl;
}());
;
(function (angular) {
    var mod = angular.module("shipApp");
    mod.constant("FIREBASE_URL", "https://c4shiptool.firebaseio.com/dev/");
    mod.controller("VerifyModalCtrl", VerifyModalCtrl);
    mod.controller("ReturnDeliveryModalCtrl", ReturnDeliveryModalCtrl);
    mod.controller("ClearModalCtrl", ClearModalCtrl);
    mod.controller("ScanController", ScanController);
})(angular);
