var SearchMacIdModel = function () {
    var self = this;
    self.searchItems = ko.observableArray([new MacIdItem({ MacId: '', ErrorMessage: '', HasErrors: false, DeliveryNumber: '', IsIRDelivery: false })]);
    self.addInput = function () {
    };

    self.submitSearchMac = function () {
        $('#searchingImage').show();
        $('#macInputError').hide();
        var data = [];

        for (i = 0; i < self.searchItems().length; i++) {
            var macIdItem = {};
            macIdItem.MacId = self.searchItems()[i].MacId();
            macIdItem.HasErrors = self.searchItems()[i].HasErrors();
            macIdItem.ErrorMessage = self.searchItems()[i].ErrorMessage();
            macIdItem.DeliveryNumber = self.searchItems()[i].DeliveryNumber();
            macIdItem.IsIRDelivery = self.searchItems()[i].IsIRDelivery();

            data.push(macIdItem);
        }

        data = $.toJSON(data);

        $.ajax({
            url: "/ship/services/MacIdSearchService.svc/LocateMacIds",
            type: "POST",
            data: data,
            dataType: "html",
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                var jsonResponse = $.parseJSON(response);
                self.searchItems.removeAll();
                for (var x = 0; x < jsonResponse.length; x++) {
                    var newItem = new MacIdItem(jsonResponse[x]);
                    self.searchItems.push(newItem);
                    if (newItem.DeliveryNumber() != 0 & newItem.DeliveryNumber() != '' & !newItem.IsIRDelivery() ) {
                        location = 'ScanSerialNumber.aspx?DeliveryNum=' + newItem.DeliveryNumber();
                    }
                    if(newItem.DeliveryNumber() != 0 & newItem.DeliveryNumber() != '' & newItem.IsIRDelivery() ) {
                        location = 'ScanInventoryRequest.aspx?DeliveryNum=' + newItem.DeliveryNumber();
                    
                    }
                    if (newItem.DeliveryNumber() == 0 || newItem.DeliveryNumber() == '') {
                        location = 'Search.aspx?DeliveryNum=0';
                    }
                }
            },
            complete: function () {
                $('#searchingImage').hide();
            }
        });
    };
};


var MacIdItem = function (item) {
    var self = this;
    self.MacId = ko.observable(item.MacId);
    self.HasErrors = ko.observable(item.HasErrors);
    self.ErrorMessage = ko.observable(item.ErrorMessage);
    self.DeliveryNumber = ko.observable(item.DeliveryNumber);
    self.IsIRDelivery = ko.observable(item.IsIRDelivery);
};