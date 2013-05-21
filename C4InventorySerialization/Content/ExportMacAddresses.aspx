<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExportMacAddresses.aspx.cs" MasterPageFile="~/Master/Site.Master" Inherits="C4InventorySerialization.Content.ExportMacAddresses" %>

<%@ Register Assembly="obout_Interface" Namespace="Obout.Interface" TagPrefix="cc1" %>
<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>
<%@ Register TagPrefix="combo" Namespace="OboutInc.Combobox" Assembly="obout_Combobox_Net" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function ExportToExcel() {
            EMAGrid.exportToExcel("ExportMacAddresses", false, false, true, true, true, null);
        }
        function SubmitDelivery(sDoc) {
            var o = sDoc;
            if (sDoc == '') {
                alert("You must enter a Delivery Number")
            }
            else {
                location.href = 'ExportMacAddresses.aspx?DeliveryNum=' + sDoc;

            }

        }
    </script>
    <% String deliveryNumber = Request.QueryString["DeliveryNum"]; %>
    <table width="100%">
        <tr>
            <td>
                <div id="pgTitle">Export Mac Addresses</div>
            </td>
            <td>
                <div id="loggedUser">Logged in as: <%=User.Identity.Name %></div>
            </td>
        </tr>
    </table>
    <hr />
    <div id="DeliveryEnter">
        Delivery Number:
    <input id="deliverytext" name="t1" type="text" size="10" onchange="SubmitDelivery(this.form.t1.value);" value="<%=deliveryNumber%>" />
        <input type="button" value="Load Delivery" onclick="SubmitDelivery(this.form.t1.value);" />
        <input type="button" value="Export to Excel" onclick="ExportToExcel()" />
        <br />
        <input id="tplApplyFilter" type="button" value="Apply Filter" onclick="EMAGrid.executeFilter();" />
        <input id="tplRemoveFilter" type="button" value="Remove Filter" onclick="EMAGrid.removeFilter();" />

        <obout:Grid ID="EMAGrid" runat="server" AllowAddingRecords="false" OnRebind="RebindGrid" AutoGenerateColumns="false" PageSize="500" AllowFiltering="true" CallbackMode="true" Serialize="true" AllowGrouping="true" FolderExports="../Exports">
            <ExportingSettings FileName="ExportMacAddresses" AppendTimeStamp="true" ExportAllPages="true" KeepColumnSettings="true" ExportedFilesLifeTime="0" ExportedFilesTargetWindow="New" />
            <FilteringSettings FilterPosition="Top" InitialState="Visible" />
            <TemplateSettings FilterShowButton_TemplateId="tplShowFilter" FilterApplyButton_TemplateId="tplApplyFilter"
                FilterHideButton_TemplateId="tplHideFilter" FilterRemoveButton_TemplateId="tplRemoveFilter" />
            <Columns>
                <obout:Column DataField="Docnum" Width="150" HeaderText="Delivery #" ShowFilterCriterias="false" SortOrder="Desc">
                    <FilterOptions>
                        <obout:FilterOption Type="Contains" IsDefault="true" />
                    </FilterOptions>
                </obout:Column>
                <obout:Column DataField="ItemCode" Width="200" HeaderText="Item Code" ShowFilterCriterias="false">
                    <FilterOptions>
                        <obout:FilterOption Type="Contains" IsDefault="true" />
                    </FilterOptions>
                </obout:Column>
                <obout:Column DataField="SerialCode" HeaderText="Serial Code" ShowFilterCriterias="false" Width="300">
                    <FilterOptions>
                        <obout:FilterOption Type="Contains" IsDefault="true" />
                    </FilterOptions>
                </obout:Column>
                <obout:Column DataField="Length" HeaderText="Length of Serial Code" ShowFilterCriterias="false" Width="80">
                    <FilterOptions>
                        <obout:FilterOption Type="Contains" IsDefault="true" />
                    </FilterOptions>
                </obout:Column>
                <obout:Column DataField="MACID" HeaderText="Mac Address" ShowFilterCriterias="false" Width="265">
                    <FilterOptions>
                        <obout:FilterOption Type="Contains" IsDefault="true" />
                    </FilterOptions>
                </obout:Column>
            </Columns>
        </obout:Grid>
</asp:Content>
