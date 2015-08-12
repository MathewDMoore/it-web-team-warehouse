/// <reference path="../typings/angularjs/angular.d.ts" />
/// <reference path="../typings/underscore/underscore.d.ts" />
interface IScanItem {
    SerialCode: string;
    SerialNum: string;
    KitCounter: number;
    KitId: number;
    ScannedBy: string;
    IsVerified: boolean;
    IsSelected: boolean;
    SmartCodeOnly: boolean;
    NoSerialRequired: boolean;
    ProductId: string;
    Id: number;
    ItemCode: string;
}

interface IDelivery {
    DeliveryNumber: number;
    ScannedItems: ICustomArray<IScanItem>;
    NotScannedItems: ICustomArray<IScanItem>;
    Kits:Array<any>;
    $save: Function;
    $destroy: Function;
    ActiveKits: ICustomArray<any>;
    IsInternal: boolean;
    IsVerified: boolean;
}
class ScanController {
    static $inject = ["$scope", "$modal", "$filter", "$timeout", "ngTableParams", "ScanOrderService", "FirebaseDeliveryService", "ngAudio", "CURRENTUSER"];
    Colors = ['#ffb81e', '#2a767d', '#3ebebe', '#d85927', '#c6b912', '#7e6591', '#ca4346', '#67773f', '#f49630', '#aa8965', '#4fa0bf', '#b9e1e5', '#ffb81e', '#2a767d', '#3ebebe', '#d85927', '#c6b912', '#7e6591', '#ca4346', '#67773f', '#f49630', '#aa8965', '#4fa0bf', '#b9e1e5'];
    ChartData = { Chart: null };
    ChartFilter = { value: null, filter: null };
    ActiveKit: Array<IScanItem>;
    SavingItem = false;
    LookUpIsInternal = false;
    ShouldSelect = false;
    FocusDeliveryInput = true;
    IsSearching = false;
    ShouldFocus = false;

    OrderIdLookUp = null;
    Delivery: IDelivery;
    SerialScanStatus = null;
    DeliveryActionMessage = null;
    SerialCodeLookUp = null;
    NotScannedFilter = null;
    ScannedFilter = null;
    SerialError = null;
    TableParams2 = null;
    TableParams3 = null;
    TableParams = null;

    constructor($scope, private $modal, $filter, private $timeout, ngTableParams, private ScanOrderService, private FirebaseDeliveryService, private ngAudio, private CURRENTUSER) {
        this.TableParams = new ngTableParams({
            page: 1, // show first page
            count: 10, // count per page
            sorting: {
                SerialNum: "asc" // initial sorting
            }
        },
            {
                total: 0, // length of data
                getData: ($defer, params) => {
                    if (this.Delivery.NotScannedItems && this.Delivery.NotScannedItems.length > 0) {
                        var orderedData = params.sorting() ? $filter("orderBy")(this.Delivery.NotScannedItems, params.orderBy()) : this.Delivery.NotScannedItems;
                        orderedData = params.filter() ? $filter("filter")(orderedData, params.filter()) : orderedData;
                        orderedData = orderedData || [];
                        params.total(orderedData.length);
                        $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                    }
                },
            });
        this.TableParams2 = new ngTableParams({
            page: 1, // show first page
            count: 10, // count per page
            sorting: {
                SerialNum: "asc" // initial sorting
            }
        }, {
                total: 0, // length of data
                getData: ($defer, params) => {
                    if (this.Delivery.ScannedItems && this.Delivery.ScannedItems.length > 0) {
                        var orderedData = params.sorting() ? $filter("orderBy")(this.Delivery.ScannedItems, params.orderBy()) : this.Delivery.ScannedItems;
                        orderedData = params.filter() ? $filter("filter")(orderedData, params.filter()) : orderedData;
                        orderedData = orderedData || [];
                        params.total(orderedData.length);
                        $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                    }
                },
            });

        this.TableParams3 = new ngTableParams({
            page: 1, // show first page
            count: 10, // count per page
            sorting: {
                SerialNum: "asc" // initial sorting
            }
        },
            {
                total: 0, // length of data
                getData: ($defer, params) => {
                    if (this.ActiveKit && this.ActiveKit.length > 0) {
                        var orderedData = params.sorting() ? $filter("orderBy")(this.ActiveKit, params.orderBy()) : this.ActiveKit;
                        orderedData = params.filter() ? $filter("filter")(orderedData, params.filter()) : orderedData;
                        orderedData = orderedData || [];
                        params.total(orderedData.length);
                        $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                    }
                },
            });
        //$watches
        $scope.$watch("filter.$", () => {
            if (this.Delivery && (this.Delivery.ScannedItems || this.Delivery.NotScannedItems)) {
                this.TableParams.reload(); // TODO: Fix {scope: null} racecondition
                this.TableParams2.reload(); // TODO: Fix {scope: null} racecondition
                this.TableParams3.reload(); // TODO: Fix {scope: null} racecondition
            }
        });
    }

    //Properties

    //Private Functions
    _errorSound() {
        this.ngAudio.play("../content/error1.mp3");
        this.SavingItem = false;
        this.ShouldSelect = true;
    }

    _successSound() {
        this.ngAudio.play("../content/success.mp3");
        this.SavingItem = false;
        this.ShouldSelect = true;
        this.SaveAndRefresh(this.TableParams, this.TableParams2, this.TableParams3);
    }

    _cleanUpKit() {
        //Remove Active Kit if all scanned items are complete
        if (this.ActiveKit && _.where(this.ActiveKit, { SerialCode: null || "" }).length === 0) {
            this.Delivery.ScannedItems = <ICustomArray<IScanItem>>(this.Delivery.ScannedItems || []);
            _.each(this.ActiveKit, (kitRow) => {
                this.Delivery.ScannedItems.push(kitRow);
                this.SaveAndRefresh(this.TableParams2);
            });
            this.ActiveKit = null;

            if (this.Delivery.ActiveKits && this.Delivery.ActiveKits.length > 0) {
                this.Delivery.ActiveKits.remove(_.where(this.Delivery.ActiveKits, { Key: this.CURRENTUSER })[0]);
            }

        }
        this.Delivery.$save();
    }

    _processKit(scanItem) {
        //Find Kit Items for SerialCode
        if (this.ActiveKit == null && scanItem.KitId > 0) {
            var matchedKitItems = _.where(this.Delivery.NotScannedItems, { KitId: scanItem.KitId, KitCounter: scanItem.KitCounter });
            if (matchedKitItems.length > 0) {
                //Initialize ActiveKit
                if (this.ActiveKit == null && scanItem.KitId > 0) {
                    this.ActiveKit = [];
                    //Add the scan item to the active kit.
                    var newKit = { Key: this.CURRENTUSER, Value: [] };
                    _.each(matchedKitItems, (item: IScanItem) => {
                        this.ScanOrderService.UpdateScanByUser({ SerialNum: item.SerialNum, DocNum: this.Delivery.DeliveryNumber });
                        this.ActiveKit = this.ActiveKit || [];
                        this.ActiveKit.push(item);
                        this.Delivery.NotScannedItems.remove(item);
                    });
                    if (!this.Delivery.ActiveKits) {
                        newKit.Value = this.ActiveKit;
                        this.Delivery.ActiveKits = <ICustomArray<any>>([]);
                        this.Delivery.ActiveKits.push(newKit);
                    }
                }
            }
        }
        this.SaveAndRefresh(this.TableParams, this.TableParams3);
    }

    //Public Functions
    GetScanTotals() {
        var matches = _.pluck(this.Delivery.ActiveKits, "Value");
        var scannedCount = this.Delivery.ScannedItems ? this.Delivery.ScannedItems.length : 0;
        var notScannedCount = this.Delivery.NotScannedItems ? this.Delivery.NotScannedItems.length : 0;
        var kitItemsCount = 0;
        if (matches && matches.length > 0) {
            _.each(matches, (match) => {
                kitItemsCount += match.length;
            });
        }
        return scannedCount + notScannedCount + kitItemsCount;
    }

    GetCurrentScan() {
        var matches = _.pluck(this.Delivery.ActiveKits, "Value");
        var scannedCount = this.Delivery.ScannedItems ? this.Delivery.ScannedItems.length : 0;
        var kitItemsCount = 0;
        if (matches && matches.length > 0) {
            _.each(matches, (match) => {
                var activeScan = _.where(match, (item) => { return item.ScannedBy; });
                kitItemsCount += activeScan.length;
            });
        }
        return scannedCount + kitItemsCount;
    }

    ScannedSearch(filter) {
        this.TableParams2.filter(filter);
    }

    NotScannedSearch(filter) {
        this.TableParams.filter(filter);
    }

    ActiveKitSearch(filter) {
        this.TableParams3.filter(filter);
    }

    LookUp(orderId, isInternal) {
        this.Delivery = null;
        this.ActiveKit = null;
        if (!isNaN(orderId) && parseInt(orderId) > 0) {

            this.IsSearching = true;
            this.DeliveryActionMessage = null;
            this.SerialError = null;

            this.ScanOrderService.LookUp(orderId, isInternal).then((response) => {

                this.SerialScanStatus = null;
                this.OrderIdLookUp = null;
                this.SerialCodeLookUp = null;
                this.NotScannedFilter = null;
                this.ScannedFilter = null;
                this.IsSearching = false;
                if (response.data) {

                    this.Delivery = this.FirebaseDeliveryService.GetDelivery(response.data.DeliveryNumber);
                    var chart = [];
                    _.each(response.data.ChartData, (item: { Key: any; Value: any }) => {
                        chart.push({ data: item.Value, label: item.Key + "(" + item.Value + ")" });
                    });
                    this.ChartData.Chart = chart;

                    _.each(response.data.ScannedItems, (item) => {
                        angular.extend(item, { IsSelected: false });
                    });
                    _.each(response.data.NotScannedItems, (item) => {
                        angular.extend(item, { IsSelected: false });
                    });
                    angular.extend(this.Delivery, {
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
                    var activeKit = _.where(response.data.ActiveKits, { Key: this.CURRENTUSER });
                    if (activeKit && activeKit.length > 0) {
                        this.ActiveKit = (<{ Value: any }>activeKit[0]).Value;
                    }
                    this.ChartFilter.value = this.GetScanTotals();
                    this.$timeout(() => { this.ShouldFocus = true; }, 500);
                } else {
                    this.DeliveryActionMessage = "Delivery not found in SAP. Check delivery number.";
                    this.FocusDeliveryInput = true;
                    this.ShouldFocus = false;
                }
                this.SaveAndRefresh(this.TableParams, this.TableParams2, this.TableParams3);                
            });
        } else if (orderId) {
            this.IsSearching = true;

            this.ScanOrderService.LookUpByMacId(orderId).then((result) => {
                this.IsSearching = false;

                if (result.data.DeliveryNumber > 0) {
                    this.LookUp(result.data.DeliveryNumber, result.data.IsInternal);
                } else {
                    this.DeliveryActionMessage = "Delivery Not Found";
                    this.FocusDeliveryInput = true;
                    this.ShouldFocus = false;
                }
            });
        }

    }

    ClearDelivery(docNumber) {

        var modalInstance = this.$modal.open({
            templateUrl: "../scripts/templates/ClearDeliveryModal.html",
            controller: ClearModalCtrl,
            resolve: {
                docNum: () => {
                    return docNumber;
                }
            }
        });

        modalInstance.result.then(() => {
            this.ScanOrderService.ClearDelivery({ DeliveryNumber: docNumber, IsInternal: this.Delivery.IsInternal }).then((result) => {
                if (result.data) {
                    this.FirebaseDeliveryService.Delete(this.Delivery).then((ref) => {
                        this.Delivery.$destroy();
                        this.Delivery = null;
                        this.DeliveryActionMessage = "Successfully cleared delivery " + docNumber;
                        this.FocusDeliveryInput = true;
                    });
                } else {
                    this.DeliveryActionMessage = "There was an error clearing this delivery.";
                }
            });
        }, () => {
            //$log.info('Modal dismissed at: ' + new Date());
        });

    }

    ReturnDelivery(delivery) {

        var modalInstance = this.$modal.open({
            templateUrl: "../scripts/templates/ReturnDeliveryModal.html",
            controller: ReturnDeliveryModalCtrl,
            resolve: {
                docNum: () => {
                    return delivery.DeliveryNumber;
                }
            }
        });

        modalInstance.result.then(() => {
            this.ScanOrderService.ReturnDelivery({ DeliveryNumber: delivery.DeliveryNumber, IsInternal: delivery.IsInternal }).then((result) => {
                if (result.data) {
                    this.DeliveryActionMessage = "Successfully returned delivery items " + delivery.DeliveryNumber;
                    this.LookUp(delivery.DeliveryNumber, delivery.IsInternal);
                } else {
                    this.DeliveryActionMessage = "There was an error returning this delivery.";
                }
            });
        }, () => {
            //$log.info('Modal dismissed at: ' + new Date());
        });
    }

    VerifyLineitem(serialCode) {
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
        this.ScanOrderService.MatchAndSave({ DocNumber: this.Delivery.DeliveryNumber, SerialCode: serialCode, IsInternal: this.LookUpIsInternal, KitId: kitId, KitCounter: kitCounter }).then((result) => {
            this.SavingItem = false;

            if (result.data.ErrorMessage) {
                this.SerialScanStatus = { Success: false, Select: true, Message: result.data.ErrorMessage };
                this._errorSound();
                return false;
            }
            var updatedItem = result.data.UpdatedItem;
            if (updatedItem) {
                var list = this.ActiveKit ? this.ActiveKit : this.Delivery.NotScannedItems;
                var found = _.find(list, (item: IScanItem) => { return item.SerialNum === updatedItem.SerialNum; });
                found.ScannedBy = updatedItem.ScannedBy;
                found.SerialCode = serialCode;
                this.Delivery.ScannedItems = <ICustomArray<IScanItem>>(this.Delivery.ScannedItems || []);
                if (found.KitId === 0) {
                    this.Delivery.ScannedItems.push(found);
                    this.Delivery.NotScannedItems.remove(found);
                    this.SaveAndRefresh(this.TableParams,this.TableParams2);
                } else if (found.KitId > 0) {
                    this._processKit(found);
                    this._cleanUpKit();
                }
                this.SerialCodeLookUp = null;
                //Set the status to success
                this.SerialScanStatus = { Success: true, Message: "Serial Successfully Updated", Select: true };
                this._successSound();
            }
        });
    }


    HasSelectedReturns() {
        var selected = _.where(this.Delivery.ScannedItems, { IsSelected: true });
        return selected.length > 0;
    }
    ReturnSelectedItems() {
        var selected = _.where(this.Delivery.ScannedItems, { IsSelected: true });
        this.Delivery.NotScannedItems = <ICustomArray<IScanItem>>(this.Delivery.NotScannedItems || []);
        if (selected.length > 0) {
            if (selected[0].KitId > 0 && !selected[0].IsVerified) {
                //var ids = _.where(scan.Delivery.ScannedItems, { KitId: selected.KitId });

                this.ScanOrderService.ReturnSelectedItems(selected, this.Delivery.IsInternal, this.Delivery.DeliveryNumber).then((result) => {
                    //TODO: Add success instead of just doing it.
                    this.Delivery.NotScannedItems = <ICustomArray<IScanItem>>(this.Delivery.NotScannedItems || []);
                    if (result.data) {
                        _.each(selected, (item) => {
                            // delete item.IsSelected;
                            item.SerialCode = null;
                            item.ScannedBy = null;
                            item.IsSelected = false;
                            this.Delivery.ScannedItems.remove(item);
                            this.Delivery.NotScannedItems.push(item);
                            this.SaveAndRefresh(this.TableParams,this.TableParams2);
                        });
                    }
                });
            } else if (!selected.filter(i => { return !i.IsVerified })) {
                //var ids = _.pluck(selected, 'Id');
                this.ScanOrderService.ReturnSelectedItems(selected, this.Delivery.IsInternal, this.Delivery.DeliveryNumber).then((result) => {
                    //TODO: Add success instead of just doing it.
                    this.Delivery.NotScannedItems = <ICustomArray<IScanItem>>(this.Delivery.NotScannedItems || []);
                    if (result.data) {
                        _.each(selected, (item) => {
                            // delete item.IsSelected;
                            item.SerialCode = null;
                            item.ScannedBy = null;
                            item.IsSelected = false;
                            this.Delivery.ScannedItems.remove(item);
                            this.Delivery.NotScannedItems.push(item);
                            this.SaveAndRefresh(this.TableParams,this.TableParams2);
                        });
                    }
                });
            }

        }
    }
    VerifyDelivery(docNum) {
        var modalInstance = this.$modal.open({
            templateUrl: "../scripts/templates/VerifyDeliveryModal.html",
            controller: VerifyModalCtrl,

            resolve: {
                docNum: () => {
                    return docNum;
                }
            }
        });

        modalInstance.result.then(() => {
            this.ScanOrderService.VerifyDelivery(docNum).then((result) => {
                if (result.data) {
                    this.FirebaseDeliveryService.Delete(this.Delivery).then((ref) => {
                        this.Delivery.$destroy();
                        this.Delivery = null;
                        this.DeliveryActionMessage = "Successfully verified delivery" + docNum;
                        this.FocusDeliveryInput = true;
                    });
                } else {
                    alert("There was an error verifying this delivery.");
                }
            });
        }, () => {
            //$log.info('Modal dismissed at: ' + new Date());
        });
    }

    ExportCSV() {
        var scannedItems = [];
        _.each(this.Delivery.ScannedItems, (item) => {
            delete item.IsSelected;
            scannedItems.push(item);
        });

        var data = scannedItems.concat(this.Delivery.NotScannedItems);

        var firstItem = data[0];
        var csvContent = "data:text/csv;charset=utf-8,";

        csvContent += _.keys(firstItem).join(",");
        csvContent += "\n";

        _.each(data, (item, index) => {
            var values = _.values(item);
            _.each(values, (text, index) => {
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
        link.setAttribute("download", this.Delivery.DeliveryNumber + "_export.csv");
        link.click();

    }

    ExportMacId() {
        var data = _.map(this.Delivery.ScannedItems, (item) => {
            return {
                Id: item.Id, SerialCode: item.SerialCode, ItemCode: item.ItemCode
            }
        });

        var csvContent = "data:text/csv;charset=utf-8,";

        csvContent += "SerialCode \n";
        csvContent += data.join("\n");

        var encodedUri = encodeURI(csvContent);
        var link = document.createElement("a");
        link.setAttribute("href", encodedUri);
        link.setAttribute("download", this.Delivery.DeliveryNumber + "_export.csv");
        link.click();

    }

    GetDeliveryStatus() {
        return this.Delivery.IsVerified ? "Delivery Verified!" : "Delivery Not Verified!";
    }

    IsScanComplete() {
        var notScannedValid = this.Delivery.NotScannedItems.filter((i) => { return (!i.ProductId || i.SmartCodeOnly && i.NoSerialRequired || (i.SmartCodeOnly && !i.NoSerialRequired)) }).length === 0;
        //         _.any(this.Delivery.NotScannedItems, (i:IScanItem) => { return (i.ProductId || i.SmartCodeOnly && i.NoSerialRequired || (i.SmartCodeOnly && !i.NoSerialRequired))});
        //            return (i.ProductId || i.SmartCodeOnly && i.NoSerialRequired || (i.SmartCodeOnly && !i.NoSerialRequired))
        //    }).length ===0;

        var noActiveKits = (this.Delivery.ActiveKits == undefined || this.Delivery.ActiveKits && this.Delivery.ActiveKits.length === 0);

        return notScannedValid && noActiveKits;
    }

    GetDeliveryStatusText() {
        var successful = this.GetDeliveryStatus();
        if (successful) {
            return "Delivery Verified!";
        }
        return "Delivery Not Verified!";
    }

    EnablVerification() {
        var items1 = _.filter(this.Delivery.NotScannedItems, (item) => {
            return !item.NoSerialRequired;
        });
        if (this.Delivery.NotScannedItems.length === 0 || items1.length === 0) {
            return true;
        }
        return false;
    }

    SaveAndRefresh(...tables){
        for (let index in tables) {
            if (tables.hasOwnProperty(index)) {
                tables[index].reload();
            }
        }

        this.Delivery.$save();
    }
}

class ClearModalCtrl {
    static $inject = ["$scope", "$modalInstance", "docNum"];
    constructor($scope, $modalInstance, docNum) {

        $scope.DocNum = docNum;
        $scope.ok = () => {
            $modalInstance.close();
        };

        $scope.cancel = () => {
            $modalInstance.dismiss('cancel');
        };
    }
}
class VerifyModalCtrl {
    static $inject = ["$scope", "$modalInstance", "docNum"];
    constructor($scope, $modalInstance, docNum) {
        $scope.DocNum = docNum;
        $scope.ok = () => {
            $modalInstance.close();
        };

        $scope.cancel = () => {
            $modalInstance.dismiss('cancel');
        };
    }
}
class ReturnDeliveryModalCtrl {
    static $inject = ["$scope", "$modalInstance", "docNum"];
    constructor($scope, $modalInstance, docNum) {

        $scope.DocNum = docNum;
        $scope.ok = () => {
            $modalInstance.close();
        };

        $scope.cancel = () => {
            $modalInstance.dismiss("cancel");
        };
    }
};

interface ICustomArray<T> extends Array<T> {
    remove: Function;
}

((angular) => {
    var mod = angular.module("shipApp");
    mod.constant("FIREBASE_URL", "https://c4shiptool.firebaseio.com/dev/");
    mod.controller("VerifyModalCtrl", VerifyModalCtrl);
    mod.controller("ReturnDeliveryModalCtrl", ReturnDeliveryModalCtrl);
    mod.controller("ClearModalCtrl", ClearModalCtrl);
    mod.controller("ScanController", ScanController);
})(angular);  