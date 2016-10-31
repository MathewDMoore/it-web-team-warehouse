<%@ Page Title="Search Mac ID" Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="C4InventorySerialization.Content.Search" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/jscript" src="../scripts/SearchSmartMac.js"></script>
    <div id="searchMac">
        <div id="macIdInput">
            Smart Code:
                <input type="text" size="40" data-bind="value: MacItem.MacId, valueUpdate: 'afterkeydown' " />
            <span data-bind="visible: MacItem().HasErrors, text: MacItem().ErrorMessage, style: {color: 'red'}"></span>
        </div>
    </div>
    <div id="searchingImage" style="display: none">
        <img src="../images/searching.gif" id="searching_image" />
    </div>
    <div id="return">
        <button id="returnButton" data-bind="click: submitSearchMac">Search</button>
    </div>
    <script type="text/javascript">
        var model;
        $(document).ready(function () {
            model = new SearchMacIdModel();
            model.addInput();
            ko.applyBindings(model);
        });

    </script>

</asp:Content>
