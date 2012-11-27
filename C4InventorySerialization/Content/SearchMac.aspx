<%@ Page Title="Search Mac ID" Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeBehind="SearchMac.aspx.cs" Inherits="C4InventorySerialization.Content.SearchMac" %>

<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/jscript" src="/scripts/SearchSmartMac.js"></script>
    <div id="searchMac">
        <!-- ko foreach: model.searchItems-->
        <div id="smartMacInput">
            Smart Code:
            <input type="text" size="40" data-bind="value: SmartMac, valueUpdate: 'afterkeydown' " /><span data-bind="visible: HasErrors, text: ErrorMessage, style: {color: 'red'}"></span>
            <asp:label runat="server" id="macInputError" visible="false" forecolor="red" Text="Delivery not found for this Smart Mac. Please check the SmartMac code." ></asp:label>
        </div>
        <!-- /ko -->
    </div>
    <div id="searchingImage" style="display: none">
        <img src="../images/searching.gif" id="searching_image" />
    </div>
    <div id="return">
        <button id="returnButton" data-bind="click: submitSearchMac">Search</button>
    </div>

    <div>
        <obout:Grid ID="Grid2" AllowAddingRecords="false" AllowSorting="false" ShowFooter="false" AllowDataAccessOnServer="true" ShowHeader="false" OnRebind="RebindGrid" CallbackMode="true" ShowColumnsFooter="false" runat="server" AutoGenerateColumns="False">
            <Columns>
                <obout:Column DataField="COLUMN1" Width="150" HeaderText=" " Index="0">
                </obout:Column>
                <obout:Column DataField="COLUMN2" Width="625" Wrap="true" HeaderText=" " Index="1">
                </obout:Column>
            </Columns>
        </obout:Grid>

    </div>
    <script type="text/javascript">
        var model;
        $(document).ready(function () {
            model = new SearchSmartMacModel();
            model.addInput();
            ko.applyBindings(model);
        });

    </script>


</asp:Content>
