<%@ Page Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeBehind="ScanOrder.aspx.cs" Inherits="C4InventorySerialization.Content.ScanOrder1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="../scripts/services/ScanOrderService.js"></script>
    <script type="text/javascript" src="../scripts/controllers/ScanOrderController.js"></script>

    <div>
        <div ng-controller="ScanController as scan">

            <input id="orderIdInput" type="number" min="0" ng-model="scan.OrderIdLookUp" />
            <button id="loadDelivery" ng-click="scan.LookUp(scan.OrderIdLookUp)">Load Delivery</button>
            <div id="actions" ng-show="scan.Delivery">
                <button id="returnDelivery">Return Entire Delivery</button>
                <button id="clearDelivery">Clear Delivery</button>
                <button id="VerifiedDelivery">Verify Delivery</button>
            </div>
            <p><strong>Page:</strong> {{scan.TableParams.page()}}</p>
            <p><strong>Count per page:</strong> {{scan.TableParams.count()}}</p>

            <table ng-table="scan.TableParams" class="table">
                <tr ng-repeat="user in scan.Data">
                    <td data-title="'Name'">{{user.name}}</td>
                    <td data-title="'Age'">{{user.age}}</td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
