function keypressed() {
    if (event.keyCode == '13') {
        onDoubleClick();
    }
    if (event.keyCode == '9') {
        onDoubleClick();
    }
}


function validate(record, $http) {
    if (record.SERIALCODE == '') {
        alert("Serial Code is mandatory");
        return false;
    }
    var validProduct = Left(Right(record.SERIALCODE, 7), 5);
    if (record.PRODUCTID != validProduct) {
        alert("You have scanned the wrong product.\n\n" + "Expecting PRODUCTID: " + record.PRODUCTID + "\n" + "You scanned : " + validProduct);
        return false;
    }
    var validColor = Left(Right(record.SERIALCODE, 2), 2);
    validColor = validColor.toUpperCase();
    if (record.COLOR != validColor) {
        alert("You have scanned the wrong color of product.\n\n" + "Expecting Color: " + record.COLOR + "\n" + "You scanned : " + validColor);
        return false;
    }

    var isRequiredSmartCode = record.SMARTCODEONLY == "True";
    var originalMac = record.SERIALCODE.trim();
    var modifiedMac = $.trim(originalMac);

    if (!isRequiredSmartCode) {
        modifiedMac = modifiedMac.substring(0, modifiedMac.length - 17);

        if (modifiedMac.length != 12) {
            if (modifiedMac.length != 16) {
                alert("You have scanned in a code that is not the correct length!");
                return false;
            }
        }

        var smartMacData = {};
        smartMacData.MacId = modifiedMac;
        smartMacData.ProductGroup = record.PRODUCTGROUP;
        smartMacData.ErrorMessage = '';
        smartMacData.IsUnique = false;
        smartMacData.ErrorDeliveryNumber = '';
        smartMacData.Id = record.ID;
        smartMacData.SerialCode = originalMac;
        var preparedData = $.toJSON(smartMacData);
        var isUniqueMac = false;
        $.ajax({
            url: "/ship/services/VerifyUniqueMacService.svc/VerifyUniqueMac",
            type: "POST",
            data: preparedData,
            dataType: "html",
            async: false,
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                var returnedItem = new SmartMacItem($.parseJSON(response));
                isUniqueMac = returnedItem.IsUnique;
                if (returnedItem.IsUnique == false) {
                    alert(returnedItem.ErrorMessage + returnedItem.ErrorDeliveryNumber);
                    return false;
                }

                if (!returnedItem.ErrorMessage) {
                    var nextRs = parseInt(record.SERIALNUM) || 0;
                    checkKey(nextRs);
                } else {
                    alert(returnedItem.ErrorMessage);
                }
            }
        });

        return isUniqueMac;
    }
}


var SmartMacItem = function (item) {
    var self = this;
    self.MacId = item.MacId;
    self.ProductGroup = item.ProductGroup;
    self.ErrorMessage = item.ErrorMessage;
    self.IsUnique = item.IsUnique;
    self.ErrorDeliveryNumber = item.ErrorDeliveryNumber;
    self.SerialCode = item.SerialCode;
    self.Id = item.Id;
};

function ClearDelivery(docNumber) {

    if (docNumber == '') {
        alert("Please enter a delivery number.");
    }

    if (confirm("Are you sure you want to clear(delete) this order? ")) {
        var deliveryData = $.toJSON(docNumber);

        $.ajax({
            url: "/ship/services/PartReturnService.svc/ClearDelivery",
            type: "POST",
            data: deliveryData,
            dataType: "html",
            async: false,
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                var jsonResponse = $.parseJSON(response);
                if (jsonResponse == true) {
                    alert("Successfully Cleared!");
                    location.href = 'ScanSerialNumber.aspx';
                } else {
                    alert("There was an error clearing this delivery.");
                }
            }
        });
    }
}


function Right(str, n) {
    if (n <= 0)
        return "";
    else if (n > String(str).length)
        return str;
    else {
        var iLen = String(str).length;
        return String(str).substring(iLen, iLen - n);
    }
}

function Left(str, n) {
    if (n <= 0)
        return "";
    else if (n > String(str).length)
        return str;
    else
        return String(str).substring(0, n);
}



function checkKey(recordId) {

    //The return or enter was pressed so submit the form
    grid1.refresh();
    setTimeout(function() { grid1.editRecord(recordId); }, 1500);


}


function onEdit(record) {

    document.getElementById('txtEditText').focus();
    document.getElementById('txtEditText').select();
    if (document.getElementById('txtEditText').value.length > 40) {
        document.getElementById('txtEditText').value = " ";
        document.getElementById('txtEditText').select;
    }
}



function onCallbackError(errorMessage, commandType, recordIndex, data) {
    alert(errorMessage);
    if (commandType != "Delete") {
        if (commandType == "Update") {
            grid1.editRecord(recordIndex);
        } else {

        }

        grid1.populateControls(data);
    }
}

function onDoubleClick() {
    var editingRs = grid1.RecordInEditMode;

    if (editingRs != null) {
        var nextRs = parseInt(editingRs) + 1 || 0;
        document.getElementById('save_rownum').value = nextRs;
        //  grid1.updateRecord(editingRs);
        //call validate
    } else {
        dblClickRs = null;
    }
}

function ReturnDelivery(sDoc) {
    var o = sDoc;
    if (sDoc == '') {
        alert("You must enter a Delivery Number");
    }
    else {
        switch (isInteger(sDoc)) {
            case true:
                if (confirm("Are you sure you want to Return this Delivery? \n All Serial Numbers will be Removed")) {
                    location.href = '../Admin/ReturnDelivery.aspx?DeliveryNum=' + sDoc;
                    break;
                }

                else { return false; }
            case false:
                alert("The Delivery Number Must be Number");
                break;
        }
    }
}

function SubmitDelivery(sDoc) {
    var o = sDoc;
    if (sDoc == '') {
        alert("You must enter a Delivery Number");
    }
    else {
        switch (isInteger(sDoc)) {
            case true:
                location.href = 'ScanSerialNumber.aspx?DeliveryNum=' + sDoc;

                break;

            case false:
                alert("The Delivery Number Must be Number");
                break;

        }
    }
}

function isInteger(s) {
    var i;

    if (isEmpty(s))
        if (isInteger.arguments.length == 1) return 0;
        else return (isInteger.arguments[1] == true);

    for (i = 0; i < s.length; i++) {
        var c = s.charAt(i);

        if (!isDigit(c)) return false;
    }

    return true;
}

function isEmpty(s) {
    return ((s == null) || (s.length == 0));
}

function isDigit(c) {
    return ((c >= "0") && (c <= "9"));
}

function productidcheck() {
    var userMessageLable = document.getElementById('Warning').value;
    var CountErr = parseInt(userMessageLable);
    if (CountErr > 0) {
        alert("Warning!!!!!\n\nThis Delivery Contains Items not in the Maintain Product Section");
    }
}

function verifydeliverycheck() {
    var userMessageLable = document.getElementById('VerifyError').value;
    var CountErr = parseInt(userMessageLable);
    if (CountErr > 0) {
        alert("This Delivery could not be verified! \n\n There are " + CountErr + " records that need to be scanned.");
    }
    var verifiedDelivery = document.getElementById('VerifiedDelivery').value;

    //if (verifiedDelivery == 1) {
    //    if (confirm("This Delivery Request appears to be ready to ship. \n\n Would you like to print a delivery request verification confirmation?")) {
    //        PrintGrid();
    //    }
    //}
}

window.onload = function () {
    verifydeliverycheck();
    productidcheck();
    document.getElementById('deliverytext').focus();
    document.getElementById('deliverytext').select();
}


//Checkbox for Return Delivery By Line Item

function SelectDeselect(oCheckbox) {
    var oElement = oCheckbox.parentNode;
    while (oElement != null && oElement.nodeName != "TR") {
        oElement = oElement.parentNode;
    }

    if (oElement != null) {
        // oElement represents the row where the clicked      
        // checkbox reside
        var oContainer = oElement.parentNode;
        var iRecordIndex = -1;
        for (var i = 0; i < oContainer.childNodes.length; i++) {
            if (oContainer.childNodes[i] == oElement) {
                iRecordIndex = i;
                break;
            }
        }

        if (iRecordIndex != -1) {
            if (oCheckbox.checked == true) {
                // select the record
                grid1.selectRecord(iRecordIndex);
            } else {
                // deselect the record
                grid1.deselectRecord(iRecordIndex);
            }
        }
    }
}

function toggleSelection(checkbox) {
    var arrCheckboxes = document.getElementsByTagName("INPUT");
    for (var i = 0; i < arrCheckboxes.length; i++) {
        if (arrCheckboxes[i].type == "checkbox") {
            if (arrCheckboxes[i].checked != checkbox.checked) {
                arrCheckboxes[i].checked = checkbox.checked;

            }
            else {
                arrCheckboxes[i].unchecked = checkbox.unchecked;

            }
        }
    }
}

function ExportToExcel() {
    grid1.exportToExcel("ScanSerialNumber", false, false, true, true, true, null);
}

function ExportMacAddresses(deliveryNum) {
    var sDoc = document.getElementById("deliverytext").value;
    if (sDoc != null) {
        location.href = '../Content/ExportMacAddresses.aspx?DeliveryNum=' + sDoc;
    }
    else {
        alert("Please load a delivery prior to exporting mac addresses.");
    }
}


function PrintGrid() {
    Grid2.print();
}


// Return Devliery By Line Item.
function ReturnDeliveryByLineItem() {
    var arrRecordsIds = $(".ob_gMCont").find("input:checked");
    var queryString = '';
    var returnWarning = '';
    for (var i = 0; i < arrRecordsIds.length; i++) {
        if (arrRecordsIds[i].id != "chkSelector") {
            var oRecord = arrRecordsIds[i].id.replace("chk_grid_", "");
            queryString += oRecord + "+";
            returnWarning += oRecord + ", ";
        }

    }
    returnWarning = returnWarning.substring(0, returnWarning.length - 2);
    if (confirm("Return item ID " + returnWarning + "?")) {
        location = '../Admin/ReturnDeliveryByLineItem.aspx?LineNum=' + queryString;
    };
}