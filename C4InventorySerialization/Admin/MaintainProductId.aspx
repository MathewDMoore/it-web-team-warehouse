<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/Site.Master" CodeBehind="MaintainProductId.aspx.cs" Inherits="C4InventorySerialization.Admin.MaintainProductId" %>


<%@ Register Assembly="obout_ComboBox" Namespace="Obout.ComboBox" TagPrefix="cc2" %>
<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>
<%@ Register Assembly="obout_Interface" Namespace="Obout.Interface" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function ExportToExcel() {
            MPIGrid.exportToExcel("MaintainProductID", false, false, true, true, true, null);
        }

        function productHistory() {
            window.location = "../Admin/ProductIDHistory.aspx";
        }

        function validateRecord(record) {
            if (record.PRODUCTID.length != 5) {
                alert("Product Code must be five characters");
                return false;
            }
            if (record.COLOR.length != 2) {
                alert("Color code must be 2 characters");
                return false;
            }
            if (record.ITEMCODE == '') {
                alert("Item code must be populated.");
                return false;
            }
        }



        function confirmDelete(record) {
            if (confirm("Are you sure you want to delete this record? \n\n" + "ID: " + record.ID +
            "\nItem Code: " + record.ITEMCODE + "\nAlternate Item Name: " + record.ALTERNATETEXT) == false) {
                return false;
            }
            return true;
        }

        function onCallbackError(errorMessage, commandType, recordIndex, data) {
            alert("There was an error editing the grid.\n\n" +
            "Error: " + errorMessage + "\n\n" +
            "Command: " + commandType + "\n" +
            "Record: " + recordIndex + "\n" +
            "Product ID: " + data.PRODUCTID + "\n" +
            "Item Code: " + data.ITEMCODE + "\n\n" +
            "More than likely you are trying to insert a Product Code or Item Code that already exists.");
        }
    </script>
    <table width="100%">
        <tr>
            <td>
                <div id="pgTitle">Maintain Product ID</div>
            </td>
            <td>
                <div id="loggedUser">Logged in as: <%=User.Identity.Name %></div>
            </td>
        </tr>
    </table>
    <hr />

    <input type="button" value="Export To Excel" onclick="ExportToExcel()" />
    <input type="button" value="Product ID History" onclick="productHistory()" />
    <br />
    <input id="tplApplyFilter" type="button" value="Apply Filter" onclick="MPIGrid.executeFilter();" />
    <input id="tplRemoveFilter" type="button" value="Remove Filter" onclick="MPIGrid.removeFilter();" />

    <obout:Grid ID="MPIGrid" runat="server" OnRebind="RebindGrid" OnUpdateCommand="UpdateRecord" GenerateRecordIds="true" OnInsertCommand="InsertRecord" OnDeleteCommand="DeleteRecord" AutoGenerateColumns="false" PageSize="50" AllowFiltering="true" AllowSorting="true" AllowGrouping="false" Serialize="false" CallbackMode="true" FolderExports="../Exports">
        <PagingSettings Position="TopAndBottom" />
        <AddEditDeleteSettings AddLinksPosition="TopAndBottom" NewRecordPosition="Dynamic" />
        <ExportingSettings ExportAllPages="true" FileName="MaintainProductID" ExportedFilesLifeTime="0" ExportedFilesTargetWindow="New" AppendTimeStamp="true" />
        <ClientSideEvents OnBeforeClientUpdate="validateRecord" OnClientCallbackError="onCallbackError" OnBeforeClientDelete="confirmDelete" OnBeforeClientInsert="validateRecord" />
        <FilteringSettings FilterPosition="Top" InitialState="Visible" />
        <TemplateSettings FilterShowButton_TemplateId="tplShowFilter" FilterApplyButton_TemplateId="tplApplyFilter"
            FilterHideButton_TemplateId="tplHideFilter" FilterRemoveButton_TemplateId="tplRemoveFilter" />
        <Columns>
            <obout:Column HeaderText="ID" DataField="ID" ReadOnly="true" Visible="false" Index="0"></obout:Column>
            <obout:Column HeaderText="Product ID" DataField="PRODUCTID" Index="1" ShowFilterCriterias="false" Width="120">
                <FilterOptions>
                    <obout:FilterOption Type="Contains" IsDefault="true" />
                </FilterOptions>
            </obout:Column>
            <obout:Column HeaderText="Item Code" DataField="ITEMCODE" Index="2" ShowFilterCriterias="false" EditTemplateId="ItemCodeEdit" Width="235">
                <FilterOptions>
                    <obout:FilterOption Type="Contains" IsDefault="true" />
                </FilterOptions>
            </obout:Column>
            <obout:Column HeaderText="Color" DataField="COLOR" Index="3" ShowFilterCriterias="false" Width="100">
                <FilterOptions>
                    <obout:FilterOption Type="Contains" IsDefault="true" />
                </FilterOptions>
            </obout:Column>
            <obout:Column HeaderText="No Serialization" DataField="NOSERIALIZATION" Index="4" AllowFilter="false" ShowFilterCriterias="false" TemplateId="tplNoSerial" EditTemplateId="tplEditNoSerial"></obout:Column>
            <obout:Column HeaderText="Smart Code Only" DataField="SMARTCODEONLY" AllowFilter="false" ShowFilterCriterias="false" Index="5" TemplateId="tplNoSerial" EditTemplateId="tplEditSmartCode"></obout:Column>
            <obout:Column HeaderText="Edit" AllowEdit="true" Index="6" Width="115"></obout:Column>
            <obout:Column HeaderText="Delete" AllowDelete="true" Index="7" Width="90"></obout:Column>
        </Columns>
        <Templates>
            <obout:GridTemplate runat="server" ID="ItemCodeEdit" ControlID="ComboBox1Load" ControlPropertyName="value">
                <Template>
                    <cc2:ComboBox runat="server" ID="ComboBox1Load" EnableViewState="true" Height="200" Width="200" EnableLoadOnDemand="true" OnLoadingItems="ComboBox1_LoadingItems" EmptyText="Select Itemcode" FilterType="Contains" EnableVirtualScrolling="true" Mode="TextBox" LoadingText="Loading..." AutoValidate="false" DataValueField="itemcode" DataTextField="itemcode"></cc2:ComboBox>
                </Template>
            </obout:GridTemplate>
            <obout:GridTemplate runat="server" ID="tplEditNoSerial" ControlID="chkNoSerial" ControlPropertyName="checked" UseQuotes="false">
                <Template>
                    <input type="checkbox" id="chkNoSerial" />
                </Template>
            </obout:GridTemplate>
            <obout:GridTemplate runat="server" ID="tplNoSerial">
                <Template>
                    <%# (Container.Value != "False" ? "yes" : "no") %>
                </Template>
            </obout:GridTemplate>
            <obout:GridTemplate ID="tplEditSmartCode" ControlID="chkSmartCode" ControlPropertyName="checked" UseQuotes="false">
                <Template>
                    <input type="checkbox" id="chkSmartCode" />
                </Template>
            </obout:GridTemplate>
        </Templates>
    </obout:Grid>
</asp:Content>
