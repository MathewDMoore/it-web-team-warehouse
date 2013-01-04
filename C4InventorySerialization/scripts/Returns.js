var ReturnsModel = function () {
    var self = this;
    self.returnItems = ko.observableArray();
    self.addInput = function () {
        self.returnItems.push(
            new ReturnItem({ SmartMac: '', ErrorMessage: '', HasErrors: false, DocNum: '', Success: false })
        );
    };

    self.submitItems = function () {
        $('#searchingImage').show();
        var data = [];

        for (i = 0; i < self.returnItems().length; i++) {
            var returnItem = {};
            returnItem.SmartMac = self.returnItems()[i].SmartMac();
            returnItem.ErrorMessage = self.returnItems()[i].ErrorMessage();
            returnItem.HasErrors = self.returnItems()[i].HasErrors();
            returnItem.DocNum = self.returnItems()[i].DocNum();
            returnItem.Success = self.returnItems()[i].Success();
            data[i] = returnItem;
        };

        var parts = $.toJSON(data);

        $.ajax({
            url: "/services/PartReturnService.svc/ReturnParts",
            type: "POST",
            data: parts,
            dataType: "html",
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                var jsonResponse = $.parseJSON(response);
                self.returnItems.removeAll();
                for (var x = 0; x < jsonResponse.length; x++) {
                    self.returnItems.push(new ReturnItem(jsonResponse[x]));
                }
            },
            complete: function () {
                $('#searchingImage').hide();
            }
        });
    };

    self.clearItems = function () {
        self.returnItems.removeAll();
        self.addInput();
    };
};

var ReturnItem = function (item) {
    var self = this;
    self.SmartMac = ko.observable(item.SmartMac);
    self.ErrorMessage = ko.observable(item.ErrorMessage);
    self.HasErrors = ko.observable(item.HasErrors);
    self.DocNum = ko.observable(item.DocNum);
    self.Success = ko.observable(item.Success);
};
