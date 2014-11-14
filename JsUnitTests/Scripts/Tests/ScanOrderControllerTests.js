/// <reference path="../libs/jasmine.js" />
/// <reference path="../libs/jquery.min.js"/>
/// <reference path="../libs/angular.min.js" />
/// <reference path="../libs/angular-animate.min.js" />
/// <reference path="../libs/angular-resource.min.js" />
/// <reference path="../angular-sanitize.min.js" />
/// <reference path="../../../c4inventoryserialization/scripts/angular.min.js" />
/// <reference path="../../../c4inventoryserialization/scripts/libs/angular-bootstrap/ui-bootstrap.min.js" />
/// <reference path="../../../c4inventoryserialization/scripts/libs/angular.audio.js" />
/// <reference path="../../../c4inventoryserialization/scripts/ng-table-export.js" />
/// <reference path="../../../c4inventoryserialization/scripts/directives.js" />
/// <reference path="../libs/firebase.js" />
/// <reference path="../libs/angularfire.min.js" />
/// <reference path="../../../c4inventoryserialization/scripts/shippingfilters.js" />
/// <reference path="../../../c4inventoryserialization/scripts/ng-table.js" />
/// <reference path="../../../c4inventoryserialization/scripts/shippingapp.js" />
/// <reference path="../../../c4inventoryserialization/scripts/services/firebasedeliveryservice.js" />
/// <reference path="../../../c4inventoryserialization/scripts/services/scanorderservice.js" />
/// <reference path="../../../c4inventoryserialization/scripts/controllers/scanordercontroller.js" />
describe("Scan Order Controller Tests", function () {
    var scope, $rootScope, _scanService, _ngTableParams, _ngAudio, _modal, _scan;
    it("Test", function () {
        expect(1).toBe(1);
    });
    beforeEach(module("shipApp"));
    //    $scope, $modal, $filter, $timeout, ngTableParams, ScanOrderService, FirebaseDeliveryService, ngAudio, CURRENTUSER
    //    beforeEach(inject(function(_$rootScope_, $controller, ScanOrderService, _ngTableParams_,FirebaseDeliveryService, $timeout, $modal, _ngAudio_) {
    //        scope = _$rootScope_.$new();
    //        $rootScope = _$rootScope_;
    //        _scanService = ScanOrderService;
    //        _modal = $modal;
    //        _ngAudio = _ngAudio_;
    //        _ngTableParams = _ngTableParams_;
    //        _scan = scope.scan;
    //        _firebase = jasmine.createSpy();
    //        $controller("ScanController as scan", {$scope:scope,FirebaseDeliveryService:_firebase, $filter:$filter,$timeout:$timeout,$modal:_modal,ngAudio:_ngAudio,ScanOrderService:_scanService, CURRENTUSER:'TEST'});
    //    }));
    describe("Default Properties", function () {
        it("Verify default values", function () {
            expect(_scan.ShouldSelect).toBeFalsy();
        });
    });
});