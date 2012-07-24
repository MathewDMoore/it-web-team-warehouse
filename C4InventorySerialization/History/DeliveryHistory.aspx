<%@ Page Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeBehind="DeliveryHistory.aspx.cs" Inherits="C4InventorySerialization.Content.DeliveryHistory" %>
<%@ Register Assembly="obout_Interface" Namespace="Obout.Interface" TagPrefix="cc1" %>
<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">			



<script type="text/javascript">
    function ExportToExcel() {
        grid1.exportToExcel("DeliveryHistory", false, true, true, true);
    }   
</script>


   <table width="100%"><tr>
   <td><div id="pgTitle">Delivery History</div></td><td><div id="loggedUser"></div></td>
   </tr></table>
   <hr />
    <asp:SqlDataSource ID="sds1" runat="server" ConnectionString="<%$ ConnectionStrings:InventoryConnectionString %>" SelectCommand="Select * from c4_serialnumbers_out where docnum = @DOCNUM" SelectCommandType="Text">
        <SelectParameters>
            <asp:Parameter Name="DOCNUM" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    
    <input id="tplApplyFilter" type="button" value="Apply Filter" onclick="grid1.executeFilter();" />
    <input id="tplRemoveFilter" type="button" value="Remove Filter" onclick="grid1.removeFilter();" />
    <input type="button" value="Export To Excel" onclick="ExportToExcel()" />
    
    <obout:Grid ID="grid1" Serialize="false" runat="server" OnRebind="RebindGrid" AllowDataAccessOnServer="true" AllowColumnResizing="true" 
    AutoGenerateColumns="false" AllowRecordSelection="false" AllowKeyNavigation="false"  AllowFiltering="true" AllowGrouping="false" ShowGroupsInfo="true"
    ShowFooter="true" AllowSorting="true" PageSize="100"  ShowLoadingMessage="false" AllowMultiRecordEditing="false" AllowAddingRecords="false" Width="100%" FolderExports="../Exports">
            <CssSettings CSSExportHeaderCellStyle="font-family:arial;font-weight: bold;background-color: #CCCCCC;color: black;" CSSExportCellStyle="font-family: arial;font-weight: bold;background-color: #CCCCCC;color: black;" />
            <ExportingSettings ExportDetails="true" FileName="DeliveryHistory" ExportedFilesLifeTime="0" ExportAllPages="false" ExportedFilesTargetWindow="New" AppendTimeStamp="true" />
            <FilteringSettings FilterPosition="Top" InitialState="Visible" />
            <TemplateSettings FilterShowButton_TemplateId="tplShowFilter" FilterApplyButton_TemplateId="tplApplyFilter"
                FilterHideButton_TemplateId="tplHideFilter" FilterRemoveButton_TemplateId="tplRemoveFilter" />  
            <Columns>
                <obout:Column DataField="DATE" HeaderText="Date" Index="0" Width="25%" ShowFilterCriterias="false" ReadOnly="true" SortOrder="Desc">
                    <FilterOptions>
                        <obout:FilterOption Type="Contains" />               
                    </FilterOptions>
                </obout:Column>
                <obout:Column DataField="DOCNUM" HeaderText="Delivery #" Index="1" Width="25%" ShowFilterCriterias="false" Visible="true" ReadOnly="true">
                    <FilterOptions>
                        <obout:FilterOption Type="Contains" IsDefault="true" />           
                    </FilterOptions>
                </obout:Column>
                <obout:Column DataField="VERIFIED" Visible="True" HeaderText="Verified" Index="2" Width="25%" ReadOnly="true" AllowFilter="false">
                </obout:Column>
                <obout:Column DataField="USERNAME" Visible="True" HeaderText="Verified By" ShowFilterCriterias="false" Index="3" Width="25%" ReadOnly="true">
                    <FilterOptions>
                        <obout:FilterOption Type="Contains" IsDefault="true" />
                    </FilterOptions>
                </obout:Column>
            </Columns>
            
            <MasterDetailSettings LoadingMode="OnCallback" />
                <DetailGrids>
                    <obout:DetailGrid ID="Grid3" runat="server" ForeignKeys="DOCNUM" DataSourceID="sds1" Serialize="false" Width="98%" AllowAddingRecords="false" AutoGenerateColumns="false" ShowLoadingMessage="true" AllowSorting="true" AllowGrouping="true">
                        <CssSettings CSSExportHeaderCellStyle="font-weight:font-family:arial; bold;background-color: #CCCCCC;color: black;" />
                        <ExportingSettings ExportAllPages="true" />
                        <Columns>
                            <obout:Column DataField="ID" HeaderText="ID" Index="0" Width="75" ReadOnly="true">
                            </obout:Column>
                            <obout:Column DataField="ITEMCODE" HeaderText="Item Code" Index="1" Width="175" Visible="true" ReadOnly="true">
                            </obout:Column>
                            <obout:Column DataField="ALTTEXT" HeaderText="Description" Index="2" Width="300" Wrap="true" ReadOnly="true">
                            </obout:Column>
                            <obout:Column DataField="SERIALNUM" HeaderText="#" Index="3" Width="50" ReadOnly="true">
                            </obout:Column>
                            <obout:Column DataField="SERIALCODE" HeaderText="Serial #" Index="4" Width="250" ItemStyle-Wrap="true" Wrap="true" ReadOnly="true" >
                            </obout:Column>
                            <obout:Column DataField="USERNAME" HeaderText="User" Index="5" Width="125" ReadOnly="true">
                            </obout:Column>
                        </Columns>
                    </obout:DetailGrid>
            </DetailGrids>
    </obout:Grid>
   
   <input type="button" value="Export To Excel" onclick="ExportToExcel()" />
   
   </asp:Content>



