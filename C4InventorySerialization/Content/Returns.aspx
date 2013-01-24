    <%@ Page Title="" Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true"
    CodeBehind="Returns.aspx.cs" Inherits="C4InventorySerialization.Content.Returns" %>

<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET, Version=7.0.5.0, Culture=neutral, PublicKeyToken=5ddc49d3b53e3f98" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="../scripts/Returns.js"></script>

    <div id="returnCodes">
        <!-- ko foreach: returnItems-->
        <div id="smartCodeInput">
            Smart Code:
            <input type="text" size="40"data-bind="value: SmartMac, valueUpdate: 'afterkeydown', hasfocus: SmartMac.length == 0" />
            <span data-bind="visible: HasErrors, text: ErrorMessage" style="color: red; font-weight: bolder"></span>
            <span data-bind="visible: Success()" style="color: green; font-weight: bolder">Successfully returned item.</span>
        </div>
        <!-- /ko --> 
    </div>
    <div id="searchingImage" style="display: none">
        <img src="../images/searching.gif" id="searching_image" />
    </div>
    <div id="addReturn">
        <button id="addReturnButton" data-bind="click: addInput">Add Another Return</button>
        <button id="returnButton" data-bind="click: submitItems">Return Items</button>
        <button id="clearReturnsButton" data-bind="click: clearItems">Clear Items</button>
    </div>
    <script type="text/javascript">
        var model;
        $(document).ready(function () {
            model = new ReturnsModel();
            model.addInput();   
            ko.applyBindings(model);
        });
    </script>
</asp:Content>
