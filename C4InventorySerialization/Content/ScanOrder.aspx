<%@ Page Language="C#" MasterPageFile="~/Master/Redesign.master" AutoEventWireup="true" CodeBehind="ScanOrder.aspx.cs" Inherits="C4InventorySerialization.Content.ScanOrder1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="../scripts/services/ScanOrderService.js"></script>
    <script type="text/javascript" src="../scripts/controllers/ScanOrderController.js"></script>

    <div>
        <div ng-controller="ScanController as scan">

            <input id="orderIdInput" type="number" min="0" ng-model="scan.OrderIdLookUp" />
            <button id="loadDelivery" ng-click="scan.LookUp(scan.OrderIdLookUp)">Load Delivery</button>
            <div id="actions" ng-show="scan.Delivery">
                <br />
                <br />
                <button id="returnDelivery">Return Entire Delivery</button>
                <button id="clearDelivery">Clear Delivery</button>
                <button id="VerifiedDelivery">Verify Delivery</button>
            </div>
            <div class="well" ng-show="scan.Delivery">
                <div id="deliveryHeader" class="well">
                    <br />
                    <label>Delivery Number: {{scan.OrderIdLookUp}}</label><br />
                    <label>Dealer ID: {{scan.Delivery.DealerId}}</label><br />
                    <label>Dealer Name: {{scan.Delivery.DealerName}}</label><br />
                    <label>Address: {{scan.Delivery.Address}}</label><br />
                    <label>Comments: {{scan.Delivery.Comments}}</label><br />
                </div>
                <label>Scanned Item Count: <b>{{scan.Delivery.ScannedItems.length}}</b></label><br/>
                <label>Not Scanned Item Count: <b>{{scan.Delivery.NotScannedItems.length}}</b></label>
                
                <label>Enter Serial Code: </label><input ng-model="scan.SerialCodeLookUp" width="500"/>

                <h3>Not Scanned Items</h3>
                <table ng-table="scan.TableParams" class="table">
                    <tr ng-repeat="item in scan.Delivery.NotScannedItems">
                        <td data-title="'ID'">{{item.Id}}</td>
                        <td data-title="'Kit Code'">{{item.ItemCode}}</td>
                        <td data-title="'Item Code'">{{item.RealItemCode}}</td>
                        <td data-title="'Description'">{{item.AltText}}</td>
                        <td data-title="'#'">{{item.SerialNum}}</td>
                        <td data-title="'Returned'">{{item.ReturnedByUser}}</td>
                    </tr>
                </table>

                <h3>Scanned Items</h3>
                <table ng-table="scan.Delivery.ScannedItems" class="table">
                    <tr ng-repeat="scannedItem in scan.Delivery.ScannedItems">
                        <td data-title="'ID'">{{scannedItem.Id}}</td>
                        <td data-title="'Kit Code'">{{scannedItem.ItemCode}}</td>
                        <td data-title="'Item Code'">{{scannedItem.RealItemCode}}</td>
                        <td data-title="'Description'">{{scannedItem.AltText}}</td>
                        <td data-title="'#'">{{scannedItem.SerialNum}}</td>
                        <td data-title="'Returned'">{{scannedItem.ReturnedByUser}}</td>
                        <td data-title="'Serial #'">{{scannedItem.SerialCode}}</td>
                    </tr>
                </table>

            </div>
        </div>
    </div>
</asp:Content>
