﻿<%@ Page Title="Search Mac ID" Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="C4InventorySerialization.Content.Search" %>

<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <form runat="server">
        <script type="text/jscript" src="../scripts/SearchSmartMac.js"></script>
        <div id="searchMac">
            <!-- ko foreach: model.searchItems-->
            <div id="macIdInput">
                Smart Code:
            <input type="text" size="40" data-bind="value: MacId, valueUpdate: 'afterkeydown' " /><span data-bind="visible: HasErrors, text: ErrorMessage, style: {color: 'red'}"></span>
                <asp:Label runat="server" ID="macInputError" Visible="false" ForeColor="red" Text="Delivery not found for this Mac Id. Please check the Mac Id you entered."></asp:Label>
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
                model = new SearchMacIdModel();
                model.addInput();
                ko.applyBindings(model);
            });

        </script>

    </form>
</asp:Content>
