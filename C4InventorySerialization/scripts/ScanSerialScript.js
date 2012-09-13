function keypressed() {
    if (event.keyCode == '13') {
        onDoubleClick();
    }
    if (event.keyCode == '9') {
        onDoubleClick();
    }
}


function validate(record) {
    if (record.SERIALCODE == '') {
        alert("Serial Code is mandatory");
        return false;
    }
    var ValidProduct = Left(Right(record.SERIALCODE, 7), 5);
    if (record.PRODUCTID != ValidProduct) {
        alert("You have scanned the wrong product.\n\n" + "Expecting PRODUCTID: " + record.PRODUCTID + "\n" + "You scanned : " + ValidProduct);
        return false;
    }
    var ValidColor = Left(Right(record.SERIALCODE, 2), 2);
    ValidColor = ValidColor.toUpperCase();
    if (record.COLOR != ValidColor) {
        alert("You have scanned the wrong color of product.\n\n" + "Expecting Color: " + record.COLOR + "\n" + "You scanned : " + ValidColor);
        return false;
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



function checkKey(record) {

    //The return or enter was pressed so submit the form
    var SelRecord = document.getElementById('save_rownum').value;
    //alert(SelRecord);
    //alert("test"+grid1.RecordInEditMode);
    //alert("test" + grid1.Rows[SelRecord].Cells[0].Value);



    if (grid1.RecordInEditMode != SelRecord - 1) {
        try {
            var NoSerial = grid1.Rows[SelRecord].Cells[7].Value
            if (NoSerial != 'True') {
                grid1.editRecord(SelRecord);
            }
        }
        catch (err) { }


    }

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

function onDoubleClick(iRecordIndex) {
    var editingRs = grid1.RecordInEditMode;
    var nextRs = 0;
    if (editingRs != null) {
        dblClickRs = iRecordIndex;
        nextRs = parseInt(editingRs) + 1;
        document.getElementById('save_rownum').value = nextRs;
        grid1.updateRecord(editingRs);


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

    if (verifiedDelivery == 1) {
        PrintGrid();
    }
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
    var sRecordsIds = grid1.getRecordsIds();
    var sDoc = document.getElementById("deliverytext").value;
    var arrRecordsIds = sRecordsIds.split(",");
    var queryString = '';
    var returnWarning = '';
    for (var i = 0; i < arrRecordsIds.length; i++) {
        var oRecord = document.getElementById(arrRecordsIds[i]);
        var oText = oRecord.innerText;
        var oID = oText.split(" ", 1);
        var oCB = document.getElementById("chk_grid_" + oID);
        if (oCB != null) {
            if (oCB.checked == true) {

                if (queryString == '') {
                    queryString = oID;
                    returnWarning = oID;
                }
                else {
                    queryString = queryString + "+" + oID;
                    returnWarning = returnWarning + ", " + oID;
                }
            }
        }
    }

    confirm("Return item ID " + returnWarning + "?");
    location.href = '../Admin/ReturnDeliveryByLineItem.aspx?LineNum=' + queryString;
}