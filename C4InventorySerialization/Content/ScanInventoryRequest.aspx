<%@ Page Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeBehind="ScanInventoryRequest.aspx.cs" Inherits="C4InventorySerialization.Content.ScanInventoryRequest" %>

<%@ Register Assembly="obout_Interface" Namespace="Obout.Interface" TagPrefix="cc1" %>
<%@ Register TagPrefix="combo" Namespace="OboutInc.Combobox" Assembly="obout_Combobox_Net" %>

<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="../scripts/ScanInventoryScript.js"> 
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
                <div id="pgTitle">Inventory Requests</div>
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
        Inventory Request Number:
    <input id="deliverytext" name="t1" type="text" size="10" onchange="SubmitDelivery(this.form.t1.value);" value="<%=deliveryNumber%>" />
        <input type="button" width="175" value="Load Inventory Request" onclick="SubmitDelivery(this.form.t1.value);" />
        <input type="button" width="175" value="Return Entire Inventory Request" onclick="ReturnDelivery(this.form.t1.value);" />
        <input type="button" value="Clear Delivery" onclick="ClearIRDelivery(this.form.t1.value);" />
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
            </tr>
        </table>
    </div>

    <asp:SqlDataSource ID="SqlDataSource3" runat="server"
        ConnectionString="<%$ ConnectionStrings:InventoryConnectionString %>"
        SelectCommand="sp_IR_Header" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
    <br />

    <input type="button" value="Return Selected Items" onclick="ReturnDeliveryByLineItem();" />

    <obout:Grid ID="grid1" runat="server" OnRebind="RebindGrid" AllowDataAccessOnServer="true" KeepSelectedRecords="true" CallbackMode="true" AllowColumnResizing="false" OnUpdateCommand="UpdateRecord" Serialize="true"
        AutoGenerateColumns="false" AllowRecordSelection="true" AllowKeyNavigation="false" GenerateRecordIds="true" AllowGrouping="true" ShowGroupsInfo="true"
        ShowFooter="true" AllowSorting="true" PageSize="500" ShowLoadingMessage="false" AllowMultiRecordEditing="false" FolderExports="../Exports">
        <CssSettings CSSExportHeaderCellStyle="font-family:arial;font-weight: bold;background-color: #CCCCCC;color: black;" CSSExportCellStyle="font-family: arial;background-color: white;color: black;" />
        <ExportingSettings FileName="Delivery" ExportedFilesLifeTime="0" ExportedFilesTargetWindow="New" AppendTimeStamp="true" ExportHiddenColumns="false" KeepColumnSettings="true" ExportAllPages="true" />
        <ClientSideEvents OnClientEdit="onEdit" OnClientUpdate="checkKey" OnBeforeClientUpdate="validate" OnClientCallbackError="onCallbackError" />
        <TemplateSettings GroupHeaderTemplateId="GroupTemplate" />
        <Columns>
            <obout:Column HeaderText="Return" Index="0" Width="80">
                <TemplateSettings TemplateId="CheckboxTemplate" HeaderTemplateId="CheckboxHeader" />
            </obout:Column>
            <obout:Column DataField="ID" HeaderText="ID" Index="1" Width="60" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="ITEMCODE" HeaderText="Item Code" Index="2" Width="130" Visible="true" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="REALITEMCODE" HeaderText="Real Item Code" Index="3" Width="130" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="ALTTEXT" HeaderText="Description" Index="4" Width="230" Wrap="true" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="SERIALNUM" HeaderText="#" Index="5" Width="50" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="RETURNEDBYUSER" Visible="true" HeaderText="Returned" Index="6" Width="90" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="SERIALCODE" HeaderText="Serial #" Index="7" Width="270" ItemStyle-Wrap="true" Wrap="true">
                <TemplateSettings EditTemplateId="SerialNumEdit" />
            </obout:Column>
            <obout:Column DataField="PRODUCTID" Visible="false" HeaderText="ID" Index="8" Width="60" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="NOSERIALIZATION" SortOrder="ASC" SortPriority="1" Visible="false" HeaderText="Items to Be Serialized" Index="9" Width="60" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="COLOR" Visible="false" HeaderText="COLOR" Index="10" Width="60" ReadOnly="true">
            </obout:Column>
            <obout:Column AllowEdit="true" HeaderText="Edit" Index="11" Width="130">
                <TemplateSettings TemplateId="EditBtnTemplate" EditTemplateId="UpdateBtnTemplate" />
            </obout:Column>
            <obout:Column DataField="SMARTCODEONLY" Visible="False" Index="12" Width="60" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="PRODUCTGROUP" Visible="False" Index="13" Width="60" ReadOnly="true">
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
    <input type="button" value="Export To Excel" onclick="ExportToExcel()" />
    <input type="button" value="Print Inventory Request Verification" onclick="PrintGrid()" />
    <asp:Button ID="Button1" Width="175" Height="25" runat="server" Text="Verify Inventory Request" OnClientClick="SubmitDelivery(this.form.t1.value)" OnClick="VerifyRecord" />
</asp:Content>



