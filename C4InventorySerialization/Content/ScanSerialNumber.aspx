<%@ Page Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeBehind="ScanSerialNumber.aspx.cs" Inherits="C4InventorySerialization.Content.ScanSerialNumber" %>

<%@ Register Assembly="obout_Interface" Namespace="Obout.Interface" TagPrefix="cc1" %>

<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">			

<script type="text/javascript">

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
            alert("You have scanned the wrong product.\n\n" + "Expecting PRODUCTID: " + record.PRODUCTID+"\n"+"You scanned : " +ValidProduct);
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
            alert("You must enter a Delivery Number")
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
                    alert("The Delivery Number Must be Number")
                    break;
            }
        }

    }

    

    function SubmitDelivery(sDoc) {
        var o = sDoc;
        if (sDoc == '') {
            alert("You must enter a Delivery Number")
        }
        else {
            switch (isInteger(sDoc)) {
                case true:
                    location.href = 'ScanSerialNumber.aspx?DeliveryNum=' + sDoc;

                    break;

                case false:
                    alert("The Delivery Number Must be Number")
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
        return ((s == null) || (s.length == 0))
    }

    function isDigit(c) {
        return ((c >= "0") && (c <= "9"))
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

  window.onload = function() {
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
        if (sDoc != null){ 
        location.href = '../Content/ExportMacAddresses.aspx?DeliveryNum=' + sDoc;
        }
        else{
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
    
</script>

<%
    String deliveryNumber = Request.QueryString["DeliveryNum"];
    String warning = CheckConfiguration.Text;
    String verifyError = ErrorRecords.Text;
    int verifyBoolean = VerifiedDelivery;
%>
   
   <asp:Label ID="CheckConfiguration" Visible="false" Text="" runat="server" />
   <asp:Label ID="ErrorRecords" Visible="false" Text="" runat="server" /> 
   <table width="100%"><tr>
   <td><div id="pgTitle">Deliveries</div></td><td><div id="loggedUser">Logged in as: <%=User.Identity.Name %></div></td>
   </tr></table>
   <hr />

   <input type="hidden" id="Warning" value="<%=warning%>" />
   <input type="hidden" id="VerifyError" value="<%=verifyError%>" />
   <input type="hidden" id="VerifiedDelivery" value="<%=verifyBoolean%>" />
   <input type="hidden" id="save_rownum" value="0" />
   <div id="DeliveryEnter">
    Delivery Number:
    <input id="deliverytext" name="t1" type="text" size="10" onchange="SubmitDelivery(this.form.t1.value);" value="<%=deliveryNumber%>"/>
    <input type="button" value="Load Delivery" onclick="SubmitDelivery(this.form.t1.value);" />
    <input type="button" value="Return Entire Delivery" onclick="ReturnDelivery(this.form.t1.value);" />
    <asp:Button ID="VerifyDelivery" Width="150" Height="25" runat="server" Text="Verify Delivery" OnClientClick="SubmitDelivery(this.form.t1.value)" OnClick="VerifyRecord" />
    <br />
    </div>
    <div id="DeliveryDetails">
    <table>
    <tr>
    <td>
    <obout:Grid ID="Grid2" AllowAddingRecords="false" AllowSorting="false" ShowFooter="false" AllowDataAccessOnServer="true" ShowHeader="false" OnRebind="RebindGrid" CallbackMode="true" ShowColumnsFooter="false" runat="server" AutoGenerateColumns="False" >
        <Columns>
            <obout:Column DataField="COLUMN1" Width="150" HeaderText=" " Index="0">
            </obout:Column>
            <obout:Column DataField="COLUMN2" Width="625" Wrap="true" HeaderText=" " Index="1">
            </obout:Column>
        </Columns>
    </obout:Grid>
    </td>
    <td>
    <asp:Image ID="verifiedimg" runat="server" ImageUrl="~/images/DeliveryVerified_0.jpg" Visible="false" />
    <asp:Image ID="notverifiedimg" runat="server" ImageUrl="~/images/NotVerified_0.jpg" Visible="false" />
    </td>
    </tr></table>
    </div>
    

    <br />
    
    <input type="button" value="Return Selected Items" onclick="ReturnDeliveryByLineItem();" />
    
    <obout:Grid ID="grid1" runat="server" OnRebind="RebindGrid" AllowDataAccessOnServer="true" KeepSelectedRecords="true" CallbackMode="true" AllowColumnResizing="false" AllowAddingRecords="false" OnUpdateCommand="UpdateRecord" Serialize="true" 
    AutoGenerateColumns="false" AllowRecordSelection="false" AllowKeyNavigation="false" GenerateRecordIds="true" AllowGrouping="true" ShowGroupsInfo="true"
          ShowFooter="true" AllowSorting="true" PageSize="500"  ShowLoadingMessage="false" AllowMultiRecordEditing="false" FolderExports="../Exports">
        <ClientSideEvents OnClientEdit="onEdit" OnClientUpdate="checkKey" OnBeforeClientUpdate="validate" OnClientCallbackError="onCallbackError"/>
        <CssSettings CSSExportHeaderCellStyle="font-family:arial;font-weight: bold;background-color: #CCCCCC;color: black;" CSSExportCellStyle="font-family: arial;background-color: white;color: black;" />
        <ExportingSettings FileName="IR" ExportedFilesLifeTime="0" ExportedFilesTargetWindow="New" AppendTimeStamp="true" ExportHiddenColumns="false" KeepColumnSettings="true" ExportAllPages="true" />
        <TemplateSettings GroupHeaderTemplateId="GroupTemplate" />
            <Columns>
                <obout:Column HeaderText="Return" Index="1" Width="80">
                    <TemplateSettings TemplateID="CheckboxTemplate" HeaderTemplateId="CheckboxHeader"/>
                </obout:Column>
                <obout:Column DataField="ID" HeaderText="ID" Index="0" Width="50" ReadOnly="true">
                </obout:Column>
                <obout:Column DataField="ITEMCODE" HeaderText="Item Code" Index="2" Width="150" Visible="true" ReadOnly="true">
                </obout:Column>
                <obout:Column DataField="ALTTEXT" HeaderText="Description" Index="3" Width="275" Wrap="true" ReadOnly="true">
                </obout:Column>
                <obout:Column DataField="SERIALNUM" HeaderText="#" Index="4" Width="50" ReadOnly="true">
                </obout:Column>
                <obout:Column DataField="SERIALCODE" HeaderText="Serial #" Index="5" Width="250" ItemStyle-Wrap="true" Wrap="true" >
                    <TemplateSettings EditTemplateID="SerialNumEdit" />
                </obout:Column>
                <obout:Column AllowEdit="true" HeaderText="Edit" Index="10" Width="130">
                    <TemplateSettings TemplateId="EditBtnTemplate" EditTemplateId="UpdateBtnTemplate" />
                </obout:Column>
                <obout:Column DataField="PRODUCTID"  Visible="false" HeaderText="ID" Index="7" Width="60" ReadOnly="true">
                </obout:Column>
                <obout:Column DataField="NOSERIALIZATION" SortOrder="ASC" SortPriority="1" Visible="false" HeaderText="Items to Be Serialized" Index="8" Width="60" ReadOnly="true">
                </obout:Column>
                <obout:Column DataField="COLOR" Visible="false" HeaderText="COLOR" Index="9" Width="60" ReadOnly="true">
                </obout:Column>
            </Columns>
            <Templates>
			    <obout:GridTemplate runat="server" ID="GroupTemplate">
			        <Template>
			            <u><%#Container.Column.HeaderText%></u> : <i><%#Container.Value%></i> :  (<%#Container.Group.PageRecordsCount%> <%#Container.Group.PageRecordsCount > 1 ? "records" : "record"%>)
			        </Template>
			    </obout:GridTemplate>
			    <obout:GridTemplate ID="SerialNumEdit" runat="server" ControlID="txtEditText" ControlPropertyName="value">
                    <Template>
                        <input size="35" id="txtEditText" onchange="onDoubleClick();" tabindex="1" type="text" style="font-family:Verdana; font-size:7pt;" />
                    </Template>
                </obout:GridTemplate>
                <obout:GridTemplate runat="server" ID="EditBtnTemplate">
                    <Template>
						<%#Convert.ToBoolean(Container.DataItem["NOSERIALIZATION"])
                                        ? "<span class=\"btspace\">N/A</span>"
                                        : "<a type=\"text/html\" id=\"btnEdit\" onclick=\"grid1.edit_record(this)\" >Edit</a>"%>
						
                    </Template>
               </obout:GridTemplate>
               <obout:GridTemplate runat="server" ID="TplNumbering">
                    <Template>
                        <b><%#(Container.RecordIndex)%>.</b>
                    </Template>
                </obout:GridTemplate>
                <obout:GridTemplate runat="server" ID="UpdateBtnTemplate">
                    <Template>
                        <a type="text/html" id="btnUpdate" tabindex="2" onclick="grid1.update_record(this)" >Update</a>
                        |
                        <a type="text/html" id="btnCancel" tabindex="3" onclick="grid1.cancel_edit(this)" >Cancel</a>
                    </Template>
                </obout:GridTemplate>	
				
		        <obout:GridTemplate runat="server" ID="CheckboxHeader">
					<Template>
						<input type="checkbox" onclick="toggleSelection(this)" id="chkSelector"/>
					</Template>
	            </obout:GridTemplate>
		        <obout:GridTemplate runat="server" ID="CheckboxTemplate">
					<Template>
						<input type="checkbox" id="chk_grid_<%#Container.DataItem["ID"]%>"/>
					</Template>
	            </obout:GridTemplate>
			</Templates>
    </obout:Grid>
    <input type="button" value="Export Mac Addresses" onclick="ExportMacAddresses(<%=deliveryNumber%>)" />
     <input type="button" value="Export To Excel" onclick="ExportToExcel()" />
    <input type="button" value="Print Delivery Verification" onclick="PrintGrid()" />
    <asp:Button ID="Button1" Width="150" Height="25" runat="server" Text="Verify Delivery" OnClientClick="SubmitDelivery(this.form.t1.value)" OnClick="VerifyRecord" />
   </asp:Content>



