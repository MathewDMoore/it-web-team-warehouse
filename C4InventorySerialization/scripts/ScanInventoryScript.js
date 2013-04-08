function keypressed() {
    if (event.keyCode == '13') {
        onDoubleClick();
    }
    if (event.keyCode == '9') {
        onDoubleClick();
    }
}


function validate(record) {
    //alert(record.ID);
    //alert(document.getElementById('txtEditText').value);
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
    var notDuplicate = false;
    var isRequiredSmartCode = record.SMARTCODEONLY == "True";
    if (!isRequiredSmartCode) {

        var data = $.toJSON(record.SERIALCODE);

        $.ajax({
            url: "/ship/services/VerifyUniqueMacService.svc/VerifyUniqueMac",
            type: "POST",
            data: data,
            dataType: "html",
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                var jsonResponse = $.parseJSON(response);
                notDuplicate = jsonResponse;
                if (!isRequiredSmartCode & notDuplicate == false) {
                    alert("This product currently exists on another delivery, or is not the correct length. Please return the product or check the MacId or SerialCode.");
                    return false;
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
        //grid1.editRecord(iRecordIndex);
    }
}

function ReturnDelivery(sDoc) {
    var o = sDoc;
    if (sDoc == '') {
        alert("You must enter a Inventory Request Number");
    }
    else {
        switch (isInteger(sDoc)) {
            case true:
                if (confirm("Are you sure you want to Return this Inventory Request? \n All Serial Numbers will be Removed")) {
                    location.href = '../Admin/ReturnInventoryRequest.aspx?DeliveryNum=' + sDoc;
                    break;
                }
                else { return false; }
            case false:
                alert("The Delivery Number Must be a number");
                break;
        }
    }

}



function SubmitDelivery(sDoc) {
    var o = sDoc;
    if (sDoc == '') {
        alert("You must enter an Inventory Request Number");
    }
    else {
        switch (isInteger(sDoc)) {
            case true:
                location.href = 'ScanInventoryRequest.aspx?DeliveryNum=' + sDoc;

                break;

            case false:
                alert("The Inventory Request Number Must be a number.");
                break;

        }
    }
}

function FocusOnFirstEdit() {
    var row = document.getElementById('ctl00_MainContent_grid1_ctl02_ob_grid1_R_0');


    if (row != null) {
        var rowEdit = row.lastChild.innerText;
        // if the node exists
        if (rowEdit == 'Edit ') {
            var sRecordsIds = grid1.getRecordsIds();
            var arrRecordsIds = sRecordsIds.split(",");
            var firstRow = arrRecordsIds[0];
            grid1.selectRecord(firstRow);
            grid1.edit_record(firstRow);

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
        alert("Warning!!!!!\n\nThis Inventory Request Contains Items not in the Maintain Product Section");
    }
}

function verifydeliverycheck() {
    var userMessageLable = document.getElementById('VerifyError').value;
    var CountErr = parseInt(userMessageLable);
    if (CountErr > 0) {
        alert("This Inventory Request could not be verified! \n\n There are " + CountErr + " records that need to be scanned.");
    }
    var verifiedDelivery = document.getElementById('VerifiedDelivery').value;

    if (verifiedDelivery == 1) {
        if (confirm("This Inventory Request appears to be ready to ship. \n\n Would you like to print a inventory request verification confirmation?")) {
            PrintGrid();
        }
        else {
            return false;
        }
    }
}


window.onload = function () {
    verifydeliverycheck();
    productidcheck();
    var DocNum = document.getElementById('deliverytext').value;
    if (DocNum == '') {
        document.getElementById('deliverytext').focus();
        document.getElementById('deliverytext').select();
    }

    //else {
    //FocusOnFirstEdit();
    //}


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
    grid1.exportToExcel("ScanInventoryRequest", false, false, true, true, true, null);
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
        location = '../Admin/ReturnIRByLineItem.aspx?LineNum=' + queryString;
    };
}