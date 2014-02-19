<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductIDHistory.aspx.cs" MasterPageFile="~/Master/Site.Master" Inherits="C4InventorySerialization.Admin.ProductIDHistory" %>
<%@ Register Assembly="obout_Interface" Namespace="Obout.Interface" TagPrefix="cc1" %>
<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>
<%@ Register TagPrefix="combo" Namespace="OboutInc.Combobox" Assembly="obout_Combobox_Net" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
    function ExportToExcel() {
        PIHGrid.exportToExcel("ProductIDHistory", false, false, true, true, true, null);
    }
    
        function maintainProductID() {
        window.location="../Admin/MaintainProductID.aspx"
    }   
      
</script>
   <table width="100%"><tr>
   <td><div id="pgTitle">Maintain Product ID History</div></td><td><div id="loggedUser">Logged in as:<%=User.Identity.Name %></div></td>
   </tr></table>
   <hr />
   
    <input type="button" value="Back" onclick="maintainProductID()" />
    <input type="button" value="Export to Excel" onclick="ExportToExcel()" />
        <br />
    <input id="tplApplyFilter" type="button" value="Apply Filter" onclick="PIHGrid.executeFilter();" />
    <input id="tplRemoveFilter" type="button" value="Remove Filter" onclick="PIHGrid.removeFilter();" />
    
<obout:Grid ID="PIHGrid" runat="server" AllowAddingRecords="false" OnRebind="RebindGrid" AutoGenerateColumns="false" PageSize="500" AllowFiltering="true" CallbackMode="true" Serialize="true" AllowGrouping="true" FolderExports="../Exports">
<ExportingSettings FileName="MaintainProductID" AppendTimeStamp="true" ExportAllPages="true" KeepColumnSettings="true" ExportedFilesLifeTime="0" ExportedFilesTargetWindow="New" />
<FilteringSettings FilterPosition="Top" InitialState="Visible" />
    <TemplateSettings FilterShowButton_TemplateId="tplShowFilter" FilterApplyButton_TemplateId="tplApplyFilter" 
    FilterHideButton_TemplateId="tplHideFilter" FilterRemoveButton_TemplateId="tplRemoveFilter" />  
<Columns>
<obout:Column DataField="DATE" Width="150" HeaderText="Date" ShowFilterCriterias="false" SortOrder="Desc">
    <FilterOptions>
        <obout:FilterOption  Type="Contains" IsDefault="true" />
    </FilterOptions>
</obout:Column>
<obout:Column DataField="TYPE" Width="115" HeaderText="Action" ShowFilterCriterias="false">
    <FilterOptions>
        <obout:FilterOption  Type="Contains" IsDefault="true" />
    </FilterOptions>
</obout:Column> 
<obout:Column DataField="PRODUCTID" HeaderText="Product ID" ShowFilterCriterias="false" Width="80">
    <FilterOptions>
        <obout:FilterOption  Type="Contains" IsDefault="true" />
    </FilterOptions>
</obout:Column>
<obout:Column DataField="ITEMCODE" HeaderText="Item Code" ShowFilterCriterias="false" Width="200">
    <FilterOptions>
        <obout:FilterOption  Type="Contains" IsDefault="true" />
    </FilterOptions>
</obout:Column>
<obout:Column DataField="COLOR" HeaderText="Color" ShowFilterCriterias="false" Width="90">
    <FilterOptions>
        <obout:FilterOption  Type="Contains" IsDefault="true" />
    </FilterOptions>
</obout:Column>
<obout:Column DataField="SMARTCODEONLY" HeaderText="Smart Code Only" AllowFilter="false" Width="125"></obout:Column>
<obout:Column DataField="NOSERIALIZATION" HeaderText="No Serialization" AllowFilter="false" ShowFilterCriterias="false" Width="125"></obout:Column>
<obout:Column DataField="USERNAME" HeaderText="User" ShowFilterCriterias="false" Width="125">
    <FilterOptions>
        <obout:FilterOption  Type="Contains" IsDefault="true" />
    </FilterOptions>
</obout:Column>

</Columns>
</obout:Grid>
</asp:Content>