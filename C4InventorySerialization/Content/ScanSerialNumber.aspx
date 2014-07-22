<%@ Page Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeBehind="ScanSerialNumber.aspx.cs" Inherits="C4InventorySerialization.Content.ScanSerialNumber" %>

<%@ Register Assembly="obout_Interface" Namespace="Obout.Interface" TagPrefix="cc1" %>

<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" src="../scripts/ScanSerialScript.js">
    </script>

    <%  
        String deliveryNumber = Request.QueryString["DeliveryNum"];
        String warning = CheckConfiguration.Text;
        String verifyError = ErrorRecords.Text;
        int verifyBoolean = VerifiedDelivery;
        
    %>

    <asp:Label ID="CheckConfiguration" Visible="false" Text="" runat="server" />
    <asp:Label ID="ErrorRecords" Visible="false" Text="" runat="server" />
    <table width="100%">
        <tr>
            <td>
                <div id="pgTitle">Deliveries</div>
            </td>
            <td>
                <div id="loggedUser">Logged in as: <%=User.Identity.Name %></div>
            </td>
        </tr>
    </table>
    <hr />

    <input type="hidden" id="Warning" value="<%=warning%>" />
    <input type="hidden" id="VerifyError" value="<%=verifyError%>" />
    <input type="hidden" id="VerifiedDelivery" value="<%=verifyBoolean%>" />
    <input type="hidden" id="save_rownum" value="0" />
    <div id="DeliveryEnter">
        Delivery Number:
    <input id="deliverytext" name="t1" type="text" size="10" onchange="SubmitDelivery(this.form.t1.value);" value="<%=deliveryNumber%>" />
        <input type="button" value="Load Delivery" onclick="SubmitDelivery(this.form.t1.value);" />
        <input type="button" value="Return Entire Delivery" onclick="ReturnDelivery(this.form.t1.value);" />
        <input type="button" value="Clear Delivery" onclick="ClearDelivery(this.form.t1.value);" />
        <asp:Button ID="VerifyDelivery" Width="150" Height="25" runat="server" Text="Verify Delivery" OnClientClick="SubmitDelivery(this.form.t1.value)" OnClick="VerifyRecord" />
        <br />
    </div>
    <div id="DeliveryDetails">
        <table>
            <tr>
                <td>
                    <obout:Grid ID="Grid2" AllowAddingRecords="false" AllowSorting="false" ShowFooter="false" AllowDataAccessOnServer="true" ShowHeader="false" OnRebind="RebindGrid" CallbackMode="true" ShowColumnsFooter="false" runat="server" AutoGenerateColumns="False">
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
            </tr>
        </table>
    </div>


    <br />

    <input type="button" value="Return Selected Items" onclick="ReturnDeliveryByLineItem();" />

    <obout:Grid ID="grid1" runat="server" OnRebind="RebindGrid" AllowDataAccessOnServer="true" KeepSelectedRecords="true" CallbackMode="true" AllowColumnResizing="false" AllowAddingRecords="false" Serialize="true"
        AutoGenerateColumns="false" AllowRecordSelection="false" AllowKeyNavigation="false" GenerateRecordIds="true" AllowGrouping="true" ShowGroupsInfo="true"
        ShowFooter="true" AllowSorting="true" PageSize="500" ShowLoadingMessage="false" AllowMultiRecordEditing="false" FolderExports="../Exports">
        <ClientSideEvents OnClientEdit="onEdit" OnBeforeClientUpdate="validate" OnClientCallbackError="onCallbackError" />
        <CssSettings CSSExportHeaderCellStyle="font-family:arial;font-weight: bold;background-color: #CCCCCC;color: black;" CSSExportCellStyle="font-family: arial;background-color: white;color: black;" />
        <ExportingSettings FileName="IR" ExportedFilesLifeTime="0" ExportedFilesTargetWindow="New" AppendTimeStamp="true" ExportHiddenColumns="false" KeepColumnSettings="true" ExportAllPages="true" />
        <TemplateSettings GroupHeaderTemplateId="GroupTemplate" />
        <Columns>
            <obout:Column HeaderText="Return" Index="1" Width="80">
                <TemplateSettings TemplateId="CheckboxTemplate" HeaderTemplateId="CheckboxHeader" />
            </obout:Column>
            <obout:Column DataField="ID" HeaderText="ID" Index="2" Width="80" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="ITEMCODE" HeaderText="Kit Code" Index="3" Width="130" Visible="true" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="REALITEMCODE" HeaderText="Item Code" Index="4" Width="130" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="ALTTEXT" HeaderText="Description" Index="5" Width="230" Wrap="true" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="SERIALNUM" HeaderText="#" Index="6" Width="50" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="RETURNEDBYUSER" Visible="true" HeaderText="Returned" Index="7" ReadOnly="true" Width="90">
            </obout:Column>
            <obout:Column DataField="SERIALCODE" HeaderText="Serial #" Index="8" Width="255" ItemStyle-Wrap="true" Wrap="true">
                <TemplateSettings EditTemplateId="SerialNumEdit" />
            </obout:Column>
            <obout:Column AllowEdit="true" HeaderText="Edit" Index="9" Width="130">
                <TemplateSettings TemplateId="EditBtnTemplate" EditTemplateId="UpdateBtnTemplate" />
            </obout:Column>
            <obout:Column DataField="NOSERIALIZATION" SortOrder="ASC" SortPriority="1" Visible="False" HeaderText="Items to Be Serialized" Index="10" Width="60" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="SMARTCODEONLY" Visible="False" Index="11" Width="60" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="COLOR" Visible="false" HeaderText="COLOR" Index="12" Width="60" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="PRODUCTID" Visible="false" HeaderText="PRODUCTID" Index="13" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="PRODUCTGROUP" Visible="false" HeaderText="PRODUCTGROUP" Index="14" ReadOnly="true">
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
                    <input size="35" id="txtEditText" onchange="onDoubleClick();" tabindex="1" type="text" style="font-family: Verdana; font-size: 7pt;" />
                </Template>
            </obout:GridTemplate>
            <obout:GridTemplate runat="server" ID="EditBtnTemplate">
                <Template>
                    <%# (string.IsNullOrEmpty(Container.DataItem["NOSERIALIZATION"].ToString()) ? "True" : Container.DataItem["NOSERIALIZATION"].ToString()) == "True" ? "<span class=\"btspace\">N/A</span>" : "<a type=\"text/html\" id=\"btnEdit\" onclick=\"grid1.edit_record(this)\" >Edit</a>"%>
                </Template>
            </obout:GridTemplate>
            <obout:GridTemplate runat="server" ID="TplNumbering">
                <Template>
                    <b><%#(Container.RecordIndex)%>.</b>
                </Template>
            </obout:GridTemplate>
            <obout:GridTemplate runat="server" ID="UpdateBtnTemplate">
                <Template>
                    <a type="text/html" id="btnUpdate" tabindex="2" onclick="grid1.update_record(this)">Update</a>
                    |
                        <a type="text/html" id="btnCancel" tabindex="3" onclick="grid1.cancel_edit(this)">Cancel</a>
                </Template>
            </obout:GridTemplate>

            <obout:GridTemplate runat="server" ID="CheckboxHeader">
                <Template>
                    <input type="checkbox" onclick="toggleSelection(this)" id="chkSelector" />
                </Template>
            </obout:GridTemplate>
            <obout:GridTemplate runat="server" ID="CheckboxTemplate">
                <Template>
                    <input type="checkbox" id="chk_grid_<%#Container.DataItem["ID"]%>" />
                </Template>
            </obout:GridTemplate>
        </Templates>
    </obout:Grid>
    <input type="button" value="Export Mac Addresses" onclick="ExportMacAddresses(<%=deliveryNumber%>)" />
    <input type="button" value="Export To Excel" onclick="ExportToExcel()" />
    <input type="button" value="Print Delivery Verification" onclick="PrintGrid()" />
    <asp:Button ID="Button1" Width="150" Height="25" runat="server" Text="Verify Delivery" OnClientClick="SubmitDelivery(this.form.t1.value)" OnClick="VerifyRecord" />
</asp:Content>



