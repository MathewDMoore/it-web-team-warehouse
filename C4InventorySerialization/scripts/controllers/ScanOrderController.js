var app = angular.module("shipApp");

app.controller("ScanController", [
        "$scope", "ngTableParams", "ScanOrderService", function ($scope, ngTableParams, ScanOrderService) {
            var scan = this;
            scan.OrderIdLookUp = null;
            scan.Delivery = null;
            scan.Data = [];
            scan.TableParams = new ngTableParams({
                page: 1, // show first page
                count: 10 // count per page
            }, {
                total: scan.Data.length, // length of data
                getData: function($defer, params) {
                    $defer.resolve(scan.Data.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                }
            });
            scan.LookUp = function(orderId) {
                ScanOrderService.LookUp(orderId).then(function (response) {
                    scan.Delivery = response.data;

                });

            }
        }
    ]
);