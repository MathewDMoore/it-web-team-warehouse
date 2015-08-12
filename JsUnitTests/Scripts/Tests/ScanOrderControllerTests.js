/// <reference path="../libs/jquery.min.js"/>
/// <reference path="../libs/angular.min.js" />
/// <reference path="../libs/angular-mocks.js" />
/// <reference path="../libs/firebase.js" />
/// <reference path="../libs/jasmine-html.js" />
/// <reference path="../libs/angularfire.min.js" />
/// <reference path="../../../c4inventoryserialization/scripts/libs/angular-bootstrap/ui-bootstrap.min.js" />
/// <reference path="../../../c4inventoryserialization/scripts/libs/angular.audio.js" />
/// <reference path="../../../c4inventoryserialization/scripts/ng-table-export.js" />

/// <reference path="../../../c4inventoryserialization/scripts/directives.js" />
/// <reference path="../../../c4inventoryserialization/scripts/common.js" />
/// <reference path="../../../c4inventoryserialization/scripts/shippingfilters.js" />
/// <reference path="../../../c4inventoryserialization/scripts/ng-table.js" />
/// <reference path="../../../c4inventoryserialization/scripts/shippingapp.js" />
/// <reference path="../../../c4inventoryserialization/scripts/services/firebasedeliveryservice.js" />
/// <reference path="../../../c4inventoryserialization/scripts/services/scanorderservice.js" />
/// <reference path="../../../c4inventoryserialization/scripts/controllers/scanordercontroller2.js" />
describe("Scan Order Controller Tests", function () {
    var $scope, _scanService, _ngTableParams, _ngAudio, $modal, _firebase;
    beforeEach(module("shipApp"));
    beforeEach(function () {
        module(function ($provide) {
            $provide.constant('SERVICES_PATH', 'PATH');
            $provide.constant('AUTH_TOKEN', 'TOKEN');
        });
    });
    beforeEach(inject(function (_$rootScope_, $controller, ScanOrderService, _ngTableParams_, $filter, $timeout, _$modal_, _ngAudio_) {
        $scope = _$rootScope_.$new();
        _scanService = ScanOrderService;
        _firebase = {};
        $modal = _$modal_;
        _ngAudio = _ngAudio_;
        _ngTableParams = _ngTableParams_;
        $controller("ScanController as scan", { $scope: $scope, FirebaseDeliveryService: _firebase, $filter: $filter, $timeout: $timeout, $modal: $modal, ngAudio: _ngAudio, ScanOrderService: _scanService, CURRENTUSER: "USER" });
    }));
    describe("Default Properties", function () {
        it("Verify default values", function () {
            expect($scope.scan.ShouldSelect).toBeFalsy();
            expect($scope.scan.SavingItem).toBeFalsy();
            expect($scope.scan.LookUpIsInternal).toBeFalsy();
            expect($scope.scan.IsSearching).toBeFalsy();
            expect($scope.scan.ShouldFocus).toBeFalsy();
            expect($scope.scan.FocusDeliveryInput).toBeTruthy();

            expect($scope.scan.OrderIdLookUp).toBeNull();
            expect($scope.scan.SerialScanStatus).toBeNull();
            expect($scope.scan.DeliveryActionMessage).toBeNull();
            expect($scope.scan.SerialCodeLookUp).toBeNull();
            expect($scope.scan.NotScannedFilter).toBeNull();
            expect($scope.scan.ScannedFilter).toBeNull();
            expect($scope.scan.SerialError).toBeNull();
            expect($scope.scan.Delivery).toBeUndefined();
        });
    });
    //Public Functions
    describe("GetScanTotals Tests", function () {
        it("Returns sum of Scanned/Not/Active Kits items", function () {
            var scanned = [{Foo:1}, {Foo:2}];
            var notScanned = [{Bar:1}, {Bar:2}];
            var kits = [{Value: [1,2] }, {Value:[3,4]}, {Value:[5,6]}];
            $scope.scan.Delivery = {ScannedItems:scanned,NotScannedItems:notScanned, ActiveKits:kits};
            var actual = $scope.scan.GetScanTotals();
            expect(actual).toBe(10);

        });
    });
});

function nullFunction() { return false; }