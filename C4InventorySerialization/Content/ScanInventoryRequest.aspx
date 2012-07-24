<%@ Page Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeBehind="ScanInventoryRequest.aspx.cs" Inherits="C4InventorySerialization.Content.ScanInventoryRequest" %>

<%@ Register Assembly="obout_Interface" Namespace="Obout.Interface" TagPrefix="cc1" %>
<%@ Register TagPrefix="combo" Namespace="OboutInc.Combobox" Assembly="obout_Combobox_Net" %>

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
        //alert(record.ID);
        //alert(document.getElementById('txtEditText').value);
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
            //grid1.editRecord(iRecordIndex);
        }
    }

    function ReturnDelivery(sDoc) {
        var o = sDoc;
        if (sDoc == '') {
            alert("You must enter a Inventory Request Number")
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
                    alert("The Delivery Number Must be Number")
                    break;
            }
        }

    }

    

    function SubmitDelivery(sDoc) {
        var o = sDoc;
        if (sDoc == '') {
            alert("You must enter an Inventory Request Number")
        }
        else {
            switch (isInteger(sDoc)) {
                case true:
                    location.href = 'ScanInventoryRequest.aspx?DeliveryNum=' + sDoc;

                    break;

                case false:
                    alert("The Inventory Request Number Must be Number")
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
        return ((s == null) || (s.length == 0))
    }

    function isDigit(c) {
        return ((c >= "0") && (c <= "9"))
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


    window.onload = function() {
        verifydeliverycheck();
        productidcheck();
        var DocNum = document.getElementById('deliverytext').value;
        if (DocNum == ''){
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
            location.href = '../Admin/ReturnIRByLineItem.aspx?LineNum=' + queryString;   
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
   <td><div id="pgTitle">Inventory Requests</div></td><td><div id="loggedUser">Logged in as: <%=User.Identity.Name %></div></td>
   </tr></table>
   <hr />
   <input type="hidden" id="Warning" value="<%=warning%>" />
   <input type="hidden" id="VerifyError" value="<%=verifyError%>" />
   <input type="hidden" id="VerifiedDelivery" value="<%=verifyBoolean%>" />
   <input type="hidden" id="save_rownum" value="0" />
   <div id="DeliveryEnter">
    Inventory Request Number:
    <input id="deliverytext" name="t1" type="text" size="10" onchange="SubmitDelivery(this.form.t1.value);" value="<%=deliveryNumber%>"/>
    <input type="button" width="175" value="Load Inventory Request" onclick="SubmitDelivery(this.form.t1.value);" />
    <input type="button" width="175" value="Return Entire Inventory Request" onclick="ReturnDelivery(this.form.t1.value);" />
    <asp:Button ID="VerifyDelivery" Width="175" Height="25" runat="server" Text="Verify Inventory Request" OnClientClick="SubmitDelivery(this.form.t1.value)" OnClick="VerifyRecord" />
    <br />
    </div>
    <div id="DeliveryDetails">
    <table>
    <tr>
    <td>
    <obout:Grid ID="Grid2" AllowAddingRecords="false" AllowSorting="false" ShowFooter="false" ShowHeader="false" ShowColumnsFooter="false" runat="server" AutoGenerateColumns="False" 
        DataSourceID="SqlDataSource3">
        <Columns>
            <obout:Column DataField="COLUMN1" Width="200" HeaderText=" " Index="0">
            </obout:Column>
            <obout:Column DataField="COLUMN2" Width="450" Wrap="true" HeaderText=" " Index="1">
            </obout:Column>
            <obout:Column DataField="DOCNUM" Width="150" Visible="FALSE" Wrap="true" HeaderText="IR #" Index="1">
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
    
    <asp:SqlDataSource ID="SqlDataSource3" runat="server" 
        ConnectionString="<%$ ConnectionStrings:InventoryConnectionString %>" 
        SelectCommand="sp_IR_Header" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:QueryStringParameter DefaultValue="0" Name="DOCNUM" 
                QueryStringField="DeliveryNum" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <br />
    
    <input type="button" value="Return Selected Items" onclick="ReturnDeliveryByLineItem();" />
    
    <obout:Grid ID="grid1" runat="server" OnRebind="RebindGrid" AllowDataAccessOnServer="true" KeepSelectedRecords="true" CallbackMode="true" AllowColumnResizing="false" OnUpdateCommand="UpdateRecord" Serialize="true" 
    AutoGenerateColumns="false" AllowRecordSelection="true" AllowKeyNavigation="false" GenerateRecordIds="true" AllowGrouping="true" ShowGroupsInfo="true"
          ShowFooter="true" AllowSorting="true" PageSize="500"  ShowLoadingMessage="false" AllowMultiRecordEditing="false" FolderExports="../Exports">
        <CssSettings CSSExportHeaderCellStyle="font-family:arial;font-weight: bold;background-color: #CCCCCC;color: black;" CSSExportCellStyle="font-family: arial;background-color: white;color: black;" />
        <ExportingSettings FileName="Delivery" ExportedFilesLifeTime="0" ExportedFilesTargetWindow="New" AppendTimeStamp="true" ExportHiddenColumns="false" KeepColumnSettings="true" ExportAllPages="true" />
        <ClientSideEvents OnClientEdit="onEdit"  OnClientUpdate="checkKey" OnBeforeClientUpdate="validate" OnClientCallbackError="onCallbackError" />
        <TemplateSettings GroupHeaderTemplateId="GroupTemplate" />
        <Columns>
            <obout:Column DataField="ID" HeaderText="ID" Index="0" Width="50" ReadOnly="true">
            </obout:Column>
            <obout:Column HeaderText="Return" Index="1" Width="80">
            <TemplateSettings TemplateID="CheckboxTemplate" HeaderTemplateId="CheckboxHeader"/>
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
            <obout:Column DataField="PRODUCTID"  Visible="false" HeaderText="ID" Index="6" Width="60" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="NOSERIALIZATION" SortOrder="ASC" SortPriority="1" Visible="false" HeaderText="Items to Be Serialized" Index="7" Width="60" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="COLOR" Visible="false" HeaderText="COLOR" Index="8" Width="60" ReadOnly="true">
            </obout:Column>
            <obout:Column AllowEdit="true" HeaderText="Edit" Index="9" Width="130">
            <TemplateSettings TemplateId="EditBtnTemplate" EditTemplateId="UpdateBtnTemplate" />
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
    <input type="button" value="Export To Excel" onclick="ExportToExcel()" />
    <input type="button" value="Print Inventory Request Verification" onclick="PrintGrid()" />
     <asp:Button ID="Button1" Width="175" Height="25" runat="server" Text="Verify Inventory Request" OnClientClick="SubmitDelivery(this.form.t1.value)" OnClick="VerifyRecord" />
   </asp:Content>



