/// <reference path="../libs/jasmine.js" />
/// <reference path="../libs/jquery.min.js"/>
/// <reference path="../libs/angular-animate.min.js" />
/// <reference path="../libs/angular-resource.min.js" />
/// <reference path="../angular-sanitize.min.js" />

/// <reference path="../../../c4inventoryserialization/scripts/angular.min.js" />
/// <reference path="../libs/angular-mocks.js" />
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
/// <reference path="../../../c4inventoryserialization/scripts/controllers/scanordercontroller2.js" />
describe("Scan Order Controller Tests", function () {
    var $scope, _scanService, _ngTableParams, _ngAudio, $modal;
    beforeEach(module("shipApp"));

//    beforeEach(
//        module(function ($provide) {
//            $provide.constant("CURRENTUSER", "USER");
//            $provide.constant("FIREBASE_URL", "URL");
//        }));


    beforeEach(inject(function (_$rootScope_, $provide, $controller, ScanOrderService, _ngTableParams_, FirebaseDeliveryService, $filter, $timeout, _$modal_, _ngAudio_) {
        $scope = _$rootScope_.$new();
        _scanService = ScanOrderService;
        $modal = _$modal_;
        _ngAudio = _ngAudio_;
        _ngTableParams = _ngTableParams_;
        $controller("ScanController as scan", { $scope: $scope, FirebaseDeliveryService: FirebaseDeliveryService, $filter: $filter, $timeout: $timeout, $modal: $modal, ngAudio: _ngAudio, ScanOrderService: _scanService, CURRENTUSER: "USER" });
    }));
    describe("Default Properties", function () {
        it("Verify default values", function () {
            expect($scope.scan.ShouldSelect).toBeFalsy();
            //            expect(_scan.ShouldSelect).toBeFalsy();
        });
    });
});