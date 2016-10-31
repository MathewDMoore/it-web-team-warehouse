<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeBehind="DuplicateMacReport.aspx.cs" Inherits="C4InventorySerialization.Content.DuplicateMacReport" %>

<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <h1>List of duplicate Mac ID's</h1>
    <obout:Grid ID="grid1" runat="server" OnRebind="RebindGrid" AllowDataAccessOnServer="true" KeepSelectedRecords="true" CallbackMode="true" AllowColumnResizing="false" 
        AllowAddingRecords="false" Serialize="true"
        AutoGenerateColumns="false" AllowRecordSelection="false" AllowKeyNavigation="false" GenerateRecordIds="true" AllowGrouping="true" ShowGroupsInfo="true"
        ShowFooter="true" AllowSorting="true" PageSize="500" ShowLoadingMessage="false" AllowMultiRecordEditing="false" FolderExports="../Exports">
        <CssSettings CSSExportHeaderCellStyle="font-family:arial;font-weight: bold;background-color: #CCCCCC;color: black;" CSSExportCellStyle="font-family: arial;background-color: white;color: black;" />
        <ExportingSettings FileName="IR" ExportedFilesLifeTime="0" ExportedFilesTargetWindow="New" AppendTimeStamp="true" ExportHiddenColumns="false" KeepColumnSettings="true" ExportAllPages="true" />
        <TemplateSettings GroupHeaderTemplateId="GroupTemplate" />
        <Columns>
            <obout:Column HeaderText="Return" Index="1" Width="80">
            </obout:Column>
            <obout:Column DataField="ID" HeaderText="ID" Index="2" Width="80" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="ITEMCODE" HeaderText="Kit Code" Index="3" Width="130" Visible="true" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="REALITEMCODE" HeaderText="Item Code" Index="4" Width="150" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="ALTTEXT" HeaderText="Description" Index="5" Width="275" Wrap="true" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="SERIALNUM" HeaderText="#" Index="6" Width="50" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="SERIALCODE" HeaderText="Serial #" Index="7" Width="270" ItemStyle-Wrap="true" Wrap="true">
                <TemplateSettings EditTemplateId="SerialNumEdit" />
            </obout:Column>
            <obout:Column AllowEdit="true" HeaderText="Edit" Index="8" Width="130">
                <TemplateSettings TemplateId="EditBtnTemplate" EditTemplateId="UpdateBtnTemplate" />
            </obout:Column>
            <obout:Column DataField="NOSERIALIZATION" SortOrder="ASC" SortPriority="1" Visible="False" HeaderText="Items to Be Serialized" Index="9" Width="60" ReadOnly="true">
            </obout:Column>
            <obout:Column DataField="SMARTCODEONLY" Visible="False" Index="10" Width="60" ReadOnly="true">
            </obout:Column> 
            <obout:Column DataField="COLOR" Visible="false" HeaderText="COLOR" Index="11" Width="60" ReadOnly="true">
            </obout:Column>
             <obout:Column DataField="PRODUCTID" Visible="false" HeaderText="PRODUCTID" Index="12" ReadOnly="true">
            </obout:Column>
        </Columns>
    </obout:Grid>
</asp:Content>
