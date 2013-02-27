

function ReturnsController($scope, $http) {
    $scope.IsSearching = false;
    $scope.returnItems = [];
    $scope.addInput = function () {
        $scope.returnItems.push(
            new ReturnItem({ MacId: '', ErrorMessage: '', HasErrors: false, DocNum: '', Success: false })
        );
    };

    $scope.submitItems = function () {
        $scope.IsSearching = true;
        var data = [];

        for (i = 0; i < $scope.returnItems.length; i++) {
            var returnItem = {};
            returnItem.MacId = $scope.returnItems[i].MacId;   
            returnItem.ErrorMessage = $scope.returnItems[i].ErrorMessage;
            returnItem.HasErrors = $scope.returnItems[i].HasErrors;
            returnItem.DocNum = $scope.returnItems[i].DocNum;
            returnItem.Success = $scope.returnItems[i].Success;
            data[i] = returnItem;
        };

        $http({
            url: "/ship/services/PartReturnService.svc/ReturnParts",
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
};

var ReturnItem = function (item) {
    var $scope = this;
    $scope.SmartMac = item.MacId;
    $scope.ErrorMessage = item.ErrorMessage;
    $scope.HasErrors = item.HasErrors;
    $scope.DocNum = item.DocNum;
    $scope.Success =  item.Success;
};
