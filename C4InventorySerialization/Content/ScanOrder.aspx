<%@ Page Language="C#" MasterPageFile="~/Master/Redesign.master" AutoEventWireup="true" CodeBehind="ScanOrder.aspx.cs" Inherits="C4InventorySerialization.Content.ScanOrder1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="../scripts/underscore-min.js"></script>
    <script type="text/javascript" src="../scripts/services/ScanOrderService.js"></script>
    <script type="text/javascript" src="../scripts/controllers/ScanOrderController.js"></script>

    <div style="min-height: 140px;">
        <div ng-controller="ScanController as scan">
            <div class="input-group" style="width: 300px;">
                <input class="form-control" placeholder="Delivery Number" id="orderIdInput" type="number" min="0" ng-model="scan.OrderIdLookUp" />
                <span class="input-group-addon " style="cursor: pointer" id="loadDelivery" ng-click="scan.LookUp(scan.OrderIdLookUp)">Load Delivery</span>
            </div>
            <div ng-if="scan.DeliveryActionMessage" class="alert alert-success" style="width: 300px;float: left;margin-left: 20px;">{{scan.DeliveryActionMessage}}</div>
            <div class="well" ng-show="scan.Delivery">
                <div id="deliveryHeader" class="well">
                    <h3>Delivery Number: {{scan.OrderIdLookUp}} Dealer: {{scan.Delivery.DealerName}}({{scan.Delivery.DealerId}})</h3>
                    <label>Address: {{scan.Delivery.Address}}</label><br />
                    <label>Comments: {{scan.Delivery.Comments}}</label><br />
                    <div class="right">
                        <button class="btn btn-warning" id="returnDelivery">Return Entire Delivery</button>
                        <button class="btn btn-danger" id="clearDelivery" ng-click="scan.ClearDelivery(scan.OrderIdLookUp)">Clear Delivery</button>
                        <button class="btn btn-success" id="VerifiedDelivery">Verify Delivery</button>
                    </div>
                    <div style="width: 200px; float: right; position: relative; top: -90px;" class="alert alert-success" ng-show="scan.Delivery.NotScannedItems.length==0">Scan Complete <i class="glyphicon glyphicon-check"></i></div>

                </div>
                <div class="row">
                    <label>Enter Serial Code: </label>
                    <div class="input-group" style="width: 328px">
                        <input class="form-control" ng-model="scan.SerialCodeLookUp" ng-change="scan.VerifyLineitem(scan.SerialCodeLookUp)" error message="scan.SerialError" />
                        <span class="input-group-addon"><i class="glyphicon glyphicon-search"></i></span>
                    </div>
                </div>
                <div>
                    <span class="text-info">{{scan.Delivery.ScannedItems.length}} of {{scan.Delivery.NotScannedItems.length +scan.Delivery.ScannedItems.length}} Products Scanned</span>
                </div>
                <style>
                    .table .header {
                        text-align: left;
                    }
                </style>
                <div ng-if="scan.Delivery.NotScannedItems.length>0">
                    <h3>Not Scanned Items</h3>
                    <table ng-table="scan.TableParams" class="table">
                        <tr ng-repeat="item in $data">
                            <td data-title="'ID'">{{item.Id}}</td>
                            <td data-title="'Kit Code'">{{item.ItemCode}}</td>
                            <td data-title="'Item Code'">{{item.RealItemCode}}</td>
                            <td data-title="'Description'">{{item.AltText}}</td>
                            <td data-title="'#'">{{item.SerialNum}}</td>
                            <td data-title="'Returned'">{{item.ReturnedByUser}}</td>
                        </tr>
                    </table>
                </div>
                <div ng-if="scan.Delivery.ScannedItems.length>0">
                    <h3>Scanned Items</h3>
                    <%--                <table ng-table="scan.Delivery.ScannedItems" class="table">--%>
                    <table ng-table="scan.TableParams2" class="table">
                        <tr ng-repeat="scannedItem in $data">
                            <td data-title="'ID'">{{scannedItem.Id}}</td>
                            <td data-title="'Kit Code'">{{scannedItem.ItemCode}}</td>
                            <td data-title="'Item Code'">{{scannedItem.RealItemCode}}</td>
                            <td data-title="'Description'">{{scannedItem.AltText}}</td>
                            <td data-title="'#'">{{scannedItem.SerialCode}}</td>
                            <td data-title="'Serial #'">{{scannedItem.SerialNum}}</td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
