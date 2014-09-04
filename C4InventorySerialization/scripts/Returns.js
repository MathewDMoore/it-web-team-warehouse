var app = angular.module("shipApp");

app.controller("ReturnsController", function ($scope, $http) {
    $scope.IsSearching = false;
    $scope.returnItems = [];
    $scope.addInput = function () {
        $scope.returnItems.push(
            new ReturnItem({ SmartCode: '', ErrorMessage: '', HasErrors: false, DocNum: '', Success: false })
        );
    };

    $scope.submitItems = function () {
        $scope.IsSearching = true;
        var data = [];

        for (i = 0; i < $scope.returnItems.length; i++) {
            var returnItem = {};
            returnItem.SmartCode = $scope.returnItems[i].SmartCode;
            returnItem.ErrorMessage = $scope.returnItems[i].ErrorMessage;
            returnItem.HasErrors = $scope.returnItems[i].HasErrors;
            returnItem.DocNum = $scope.returnItems[i].DocNum;
            returnItem.Success = $scope.returnItems[i].Success;
            data[i] = returnItem;
        };

        $http({
            url: "/services/PartReturnService.svc/ReturnParts",
            method: "POST",
            data: data
        }).success(
            function (response) {
                $scope.IsSearching = false;
                $scope.returnItems = [];
                for (var x = 0; x < response.length; x++) {
                    $scope.returnItems.push(new ReturnItem(response[x]));
                }
            });
    };

    $scope.clearItems = function () {
        $scope.returnItems = [];
        $scope.addInput();
    };


    function AddInput() {
        $('input.textbox').on('keydown', function (e) {
            var keyCode = e.keyCode || e.which;

            if (keyCode == 9) {
                e.preventDefault();
                $scope.addInput();
                $scope.$apply();
                var inputs = $('input:text');
                inputs[inputs.length - 1].focus();
                AddInput();
            }
        });
    }

    $(function () {
        AddInput();
    });
});

var ReturnItem = function (item) {
    var $scope = this;
    $scope.SmartCode = item.SmartCode;
    $scope.ErrorMessage = item.ErrorMessage;
    $scope.HasErrors = item.HasErrors;
    $scope.DocNum = item.DocNum;
    $scope.Success = item.Success;
};
