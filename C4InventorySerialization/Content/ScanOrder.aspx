﻿<%@ Page Language="C#" MasterPageFile="~/Master/Redesign.master" AutoEventWireup="true" CodeBehind="ScanOrder.aspx.cs" Inherits="C4InventorySerialization.Content.ScanOrder1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="../scripts/underscore-min.js"></script>
    <script type="text/javascript" src="../scripts/services/ScanOrderService.js"></script>
    <script type="text/javascript" src="../scripts/controllers/ScanOrderController.js"></script>

    <div style="min-height: 140px;">
        <h2>Deliveries</h2>
        <div ng-controller="ScanController as scan" class="well">
            <form ng-submit="scan.LookUp(scan.OrderIdLookUp)">
                <div class="input-group" style="width: 300px;">
                    <input class="form-control" placeholder="Delivery Number" id="orderIdInput" type="number" min="0" ng-model="scan.OrderIdLookUp" />
                    <span class="input-group-addon " style="cursor: pointer" id="loadDelivery" ng-click="scan.LookUp(scan.OrderIdLookUp)">Load Delivery</span>
                </div>
            </form>
            <div ng-if="scan.DeliveryActionMessage" class="alert alert-success" style="width: 300px; float: left; margin-left: 20px;">{{scan.DeliveryActionMessage}}</div>
            <div style="margin-top:10px;" ng-show="scan.Delivery">
                <div class="panel panel-info">
                    <div class="panel-heading">
                        <h3 style="margin:0px;">Delivery Number: {{scan.Delivery.DeliveryNumber}} </h3>
                    </div>
                    <div class="panel-body">
                        <div style="width: 200px; float: right; margin-bottom: 0px" class="alert alert-success" ng-show="scan.Delivery.NotScannedItems.length==0">Scan Complete <i class="glyphicon glyphicon-check"></i></div>                        
                        <h3 style="margin-top:0px;">Dealer: {{scan.Delivery.DealerName}}/<small>{{scan.Delivery.DealerId}}</small></h3>
                        <label>Address: {{scan.Delivery.Address}}</label><br />
                        <label>Comments: {{scan.Delivery.Comments}}</label><br />
                        <div style="float:right;">
                            <button class="btn btn-warning" id="returnDelivery">Return Entire Delivery</button>
                            <button class="btn btn-danger" id="clearDelivery" ng-click="scan.ClearDelivery(scan.OrderIdLookUp)">Clear Delivery</button>
                            <button class="btn btn-success" id="VerifiedDelivery">Verify Delivery</button>
                        </div>
                    </div>
                </div>
                <div>
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
                            <td data-title="'ID'" sortable="'Id'">{{item.Id}}</td>
                            <td data-title="'Kit Code'" sortable="'ItemCode'">{{item.ItemCode}}</td>
                            <td data-title="'Item Code'" sortable="'RealItemCode'">{{item.RealItemCode}}</td>
                            <td data-title="'Description'" sortable="'AltText'">{{item.AltText}}</td>
                            <td data-title="'#'" sortable="'SerialNum'">{{item.SerialNum}}</td>
                            <td data-title="'Returned'" sortable="'ReturnedByUser'">{{item.ReturnedByUser}}</td>
                        </tr>
                    </table>
                </div>
                <div ng-if="scan.Delivery.ScannedItems.length>0">
                    <h3>Scanned Items</h3>
                    <%--                <table ng-table="scan.Delivery.ScannedItems" class="table">--%>
                    <table ng-table="scan.TableParams2" class="table">
                        <tr ng-repeat="scannedItem in $data">
                            <td data-title="'ID'" sortable="'Id'">{{scannedItem.Id}}</td>
                            <td data-title="'Kit Code'" sortable="'ItemCode'">{{scannedItem.ItemCode}}</td>
                            <td data-title="'Item Code'" sortable="'RealItemCode'">{{scannedItem.RealItemCode}}</td>
                            <td data-title="'Description'" sortable="'AltText'">{{scannedItem.AltText}}</td>
                            <td data-title="'#'" sortable="'SerialNum'">{{scannedItem.SerialNum}}</td>
                            <td data-title="'Serial #'" sortable="'SerialCode'">{{scannedItem.SerialCode}}</td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
