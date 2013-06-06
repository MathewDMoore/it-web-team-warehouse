<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true"
    CodeBehind="Returns.aspx.cs" Inherits="C4InventorySerialization.Content.Returns" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="../scripts/Returns.js"></script>
    <div ng-controller="ReturnsController">
        <div id="returnCodes" ng-init="addInput()">
            <div id="smartCodeInput" ng-repeat="item in returnItems" >
                Smart Code:
            <input type="text" size="40" ng-model="item.MacId" on-enter="addInput"/>
                <span ng-show="item.HasErrors" style="color: red; font-weight: bolder">{{item.ErrorMessage}}</span>
                <span ng-show="item.Success" style="color: green; font-weight: bolder">Successfully returned item.</span>
            </div>
        </div>
        <div id="searchingImage" ng-show="IsSearching">
            <img src="../images/searching.gif" id="searching_image" />
        </div>
        <div id="addReturn">
            <input type="button" value="Add Another Return" id="addReturnButton" ng-click="addInput()"/>
            <input type="button" value="Return Items" id="returnButton" ng-click="submitItems()"/>
            <input type="button" id="clearReturnsButton" value="Clear Items" ng-click="clearItems()" />
        </div>
    </div>
</asp:Content>