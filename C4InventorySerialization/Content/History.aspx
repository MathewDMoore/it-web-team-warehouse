<%@ Page Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeBehind="History.aspx.cs" Inherits="C4InventorySerialization.Content.History" Title="History" %>

<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="../scripts/History.js">
    </script>

            <p>You can search history by using a Delivery Number, IR Number, SmartMac, MacId, or User name.</p>
            <p>Search Parameter:</p>
            <input id="searchItem" name="searchParameter" type="text" size="40" />
            <select id="searchByParameter">
                <option value="DeliveryNumber">Delivery Number</option>
                <option value="IRNumber">IR Number</option>
                <option value="SmartCode">SmartCode</option>
                <option value="MacId">MacId</option>
                <option value="UserName">UserName</option>
            </select>
            <input id="searchButton" type="button" value="Search" onclick="SubmitSearch(this.form.searchParameter.value);" />
        </div>
        <obout:Grid ID="grid1" runat="server" AllowDataAccessOnServer="true" KeepSelectedRecords="true" CallbackMode="true" AllowColumnResizing="false" Serialize="true"
            AutoGenerateColumns="false" AllowRecordSelection="true" AllowKeyNavigation="false" GenerateRecordIds="true" AllowGrouping="true" ShowGroupsInfo="true"
            ShowFooter="true" AllowSorting="true" PageSize="500" ShowLoadingMessage="false" AllowMultiRecordEditing="false" FolderExports="../Exports">
            <CssSettings CSSExportHeaderCellStyle="font-family:arial;font-weight: bold;background-color: #CCCCCC;color: black;" CSSExportCellStyle="font-family: arial;background-color: white;color: black;" />
            <ExportingSettings FileName="Delivery" ExportedFilesLifeTime="0" ExportedFilesTargetWindow="New" AppendTimeStamp="true" ExportHiddenColumns="false" KeepColumnSettings="true" ExportAllPages="true" />
            <TemplateSettings GroupHeaderTemplateId="GroupTemplate" />
            <Columns>
                <obout:Column DataField="DOCNUM" HeaderText="DocNum" Index="0" Width="80" ReadOnly="true" NullDisplayText="N/A">
                </obout:Column>
                <obout:Column DataField="DATE" HeaderText="Date" Index="1" Width="100" ReadOnly="true" NullDisplayText="N/A">
                </obout:Column>
                <obout:Column DataField="ITEMCODE" HeaderText="Item #" Index="2" Width="120" ReadOnly="true" NullDisplayText="N/A">
                </obout:Column>
                <obout:Column DataField="DSCRIPTION" HeaderText="Description" Index="3" Width="250" ReadOnly="true" NullDisplayText="N/A">
                </obout:Column>
                <obout:Column DataField="SERIALCODE" HeaderText="Serial Code" Index="4" Width="250" Visible="true" ReadOnly="true" NullDisplayText="N/A">
                </obout:Column>
                <obout:Column DataField="USERNAME" HeaderText="User Name" Index="5" Width="90" Wrap="true" ReadOnly="true" Visible="false" NullDisplayText="N/A">
                </obout:Column>
                <obout:Column DataField="MACID" HeaderText="Mac Id" Index="6" Width="150" ItemStyle-Wrap="true" Wrap="true" NullDisplayText="N/A">
                </obout:Column>
                <obout:Column DataField="DISPOSITION" HeaderText="Disposition" Index="7" Width="150" ItemStyle-Wrap="true" Wrap="true" NullDisplayText="N/A">
                </obout:Column>
            </Columns>
        </obout:Grid>
</asp:Content>
