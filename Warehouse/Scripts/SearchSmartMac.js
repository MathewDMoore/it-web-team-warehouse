var SearchMacIdModel = function () {
    var self = this;
    self.MacItem = ko.observable(new MacIdItem({ MacId: '', ErrorMessage: '', HasErrors: false, DeliveryNumber: '', IsIRDelivery: false }));
    self.addInput = function () {
    };

    self.PrepareData = function() {
        var data = { };
        data.MacId = self.MacItem.MacId;

        return data;
    };

    self.submitSearchMac = function () {
        $('#searchingImage').show();
        $('#macInputError').hide();
    
        var data = $.toJSON(self.PrepareData());

        $.ajax({
            url: "/ship/services/MacIdSearchService.svc/LocateMacIds",
            type: "POST",
            data: data,
            dataType: "html",
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                var jsonResponse = $.parseJSON(response);
                self.MacItem(new MacIdItem(jsonResponse));
                if (self.MacItem().DeliveryNumber() != 0 & self.MacItem().DeliveryNumber() != '' & self.MacItem().DeliveryNumber() != null & !self.MacItem().IsIRDelivery()) {
                    location = 'ScanSerialNumber.aspx?DeliveryNum=' + self.MacItem().DeliveryNumber();
                    }
                if (self.MacItem().DeliveryNumber() != 0 & self.MacItem().DeliveryNumber() != null & self.MacItem().DeliveryNumber() != '' & self.MacItem().IsIRDelivery()) {
                    location = 'ScanInventoryRequest.aspx?DeliveryNum=' + self.MacItem().DeliveryNumber();
                    
                    }
                if (self.MacItem().DeliveryNumber() == 0 || self.MacItem().DeliveryNumber() == '') {
                        
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