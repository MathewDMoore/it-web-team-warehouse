var SearchSmartMacModel = function () {
    var self = this;
    self.searchItems = ko.observableArray([new SmartMacItem({ SmartMac: '', ErrorMessage: '', HasErrors: false , DeliveryNumber: ''})]);
    self.addInput = function () {
    };

    self.submitSearchMac = function () {
        var data =[];

        for (i = 0; i < self.searchItems().length; i++) {
            var smartMacItem = {};
            smartMacItem.SmartMac = self.searchItems()[i].SmartMac();
            smartMacItem.HasErrors = self.searchItems()[i].HasErrors();
            smartMacItem.ErrorMessage = self.searchItems()[i].ErrorMessage();
            smartMacItem.DeliveryNumber = self.searchItems()[i].DeliveryNumber();

            data.push(smartMacItem);
        }

        data = $.toJSON(data);

        $.ajax({
            url: "/services/SmartMacSearchService.svc/LocateSmartMac",
            type: "POST",
            data: data,
            dataType: "html",
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                var jsonResponse = $.parseJSON(response);
                self.searchItems.removeAll();
                for (var x = 0; x < jsonResponse.length; x++) {
                    var newItem = new SmartMacItem(jsonResponse[x]);
                    self.searchItems.push(newItem);

                    location.href = 'SearchMac.aspx?DeliveryNum=' + newItem.DeliveryNumber();
                }
            }
        });
    };
};


var SmartMacItem = function (item) {
    var self = this;
    self.SmartMac = ko.observable(item.SmartMac);
    self.HasErrors = ko.observable(item.HasErrors);
    self.ErrorMessage = ko.observable(item.ErrorMessage);
    self.DeliveryNumber = ko.observable(item.DeliveryNumber);
};