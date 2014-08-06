<%@ Page Language="C#" MasterPageFile="~/Master/Redesign.master" AutoEventWireup="true" CodeBehind="ScanOrder.aspx.cs" Inherits="C4InventorySerialization.Content.ScanOrder1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="../scripts/underscore-min.js"></script>
    <script type="text/javascript" src="../scripts/services/ScanOrderService.js"></script>
    <script type="text/javascript" src="../scripts/controllers/ScanOrderController.js"></script>

    <div style="min-height: 140px;">
        <h2>Deliveries</h2>
        <div ng-controller="ScanController as scan" class="well">
            <form ng-submit="scan.LookUp(scan.OrderIdLookUp)" ng-init="scan.LookUp(<%=Request.QueryString.Get("delivery")%>)">
                <div class="input-group" style="width: 300px;">
                    <input class="form-control" placeholder="Delivery Number" ng-disabled="scan.IsSearching" autofocus id="orderIdInput" type="number" min="0" ng-model="scan.OrderIdLookUp" ng-blur="scan.LookUp(scan.OrderIdLookUp)" />
                    <span class="input-group-addon " style="cursor: pointer" id="loadDelivery" ng-click="scan.LookUp(scan.OrderIdLookUp)">{{scan.IsSearching?'Loading...':'Load Delivery'}}</span>
                </div>
            </form>
            <div ng-if="scan.DeliveryActionMessage" class="alert alert-success" style="width: 300px; float: left; margin-left: 20px;">{{scan.DeliveryActionMessage}}</div>
            <div style="margin-top: 10px;" ng-show="scan.Delivery">
                <div class="panel panel-info">
                    <div class="panel-heading">
                        <h3 style="margin: 5px 0px; display: inline-block">Delivery Number: {{scan.Delivery.DeliveryNumber}} </h3>
                        <div style="float: right; position: relative; top: -5px;">
                            <div class="btn-group">
                                <button class="btn btn-lg btn-primary"><i class="glyphicon glyphicon-export"></i></button>
                                <button type="button" class="btn btn-lg btn-primary dropdown-toggle" data-toggle="dropdown">
                                    <span class="caret" style="font-size: 20px"></span>
                                </button>
                                <ul class="dropdown-menu" role="menu">
                                    <li><a style="cursor: pointer" ng-mousedown="csv.generate()" ng-href="{{ csv.link() }}" download="test.csv">Export to CSV</a></li>
                                    <li><a style="cursor: pointer" ng-click="scan.ExportMacId(scan.Delivery.ScannedItems)">Export MacIDs</a></li>
                                </ul>
                            </div>
                            <span class="btn btn-lg btn-primary"><i class="glyphicon glyphicon-print" id="PrintDelivery" ng-click="scan.Print()"></i></span>
                        </div>
                    </div>
                    <div class="panel-body">
                        <div style="width: 200px; float: right; margin-bottom: 0px" class="alert alert-success" ng-show="scan.Delivery.NotScannedItems.length==0">Scan Complete <i class="glyphicon glyphicon-check"></i></div>
                        <h3 style="margin-top: 0px;">Dealer: {{scan.Delivery.DealerName}}/<small>{{scan.Delivery.DealerId}}</small></h3>
                        <label>Address: {{scan.Delivery.Address}}</label><br />
                        <label>Comments: {{scan.Delivery.Comments}}</label><br />
                        <div style="float: right;">
                            <button class="btn btn-warning" id="returnDelivery">Return Entire Delivery</button>
                            <button class="btn btn-danger" id="clearDelivery" ng-click="scan.ClearDelivery(scan.OrderIdLookUp)">Clear Delivery</button>
                            <button class="btn btn-success" id="VerifiedDelivery" ng-click="scan.VerifyDelivery()">Verify Delivery</button>
                        </div>
                    </div>
                </div>
                <div style="float: left; margin-bottom: 20px;">
                    <label>Enter Serial Code: </label>
                    <div class="input-group" style="width: 328px">
                        <input class="form-control" autofocus auto-select select="scan.SerialScanStatus.Success==false" ng-model="scan.SerialCodeLookUp" ng-change="scan.VerifyLineitem(scan.SerialCodeLookUp)" />
                        <span class="input-group-addon"><i class="glyphicon glyphicon-search"></i></span>
                    </div>
                    <span class="text-info">{{scan.Delivery.ScannedItems.length}} of {{scan.Delivery.NotScannedItems.length +scan.Delivery.ScannedItems.length}} Products Scanned</span>
                </div>
                <div class="alert" style="width: 600px; float: left; margin-left: 10px; position: relative; top: 14px;" ng-class="{'alert-danger':!scan.SerialScanStatus.Success, 'alert-success':scan.SerialScanStatus.Success}" ng-show="scan.SerialScanStatus">{{scan.SerialScanStatus.Message}}</div>

                <style>
                    .table .header {
                        text-align: left;
                    }
                </style>
                <div ng-if="scan.Delivery.NotScannedItems.length>0" style="clear: both;">
                    <h3>Not Scanned Items</h3>
                    <div>
                        Search
                        <div class="input-group" style="width: 328px">
                            <input class="form-control" ng-model="scan.NotScannedFilter" ng-change="scan.NotScannedSearch(scan.NotScannedFilter);" />
                            <span class="input-group-addon"><i class="glyphicon glyphicon-search"></i></span>
                        </div>
                    </div>
                    <table ng-table="scan.TableParams" class="table">
                        <tr ng-repeat="item in $data track by $index">
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
                    <h3>Scanned Items <span class="btn btn-warning" ng-click="scan.ReturnSelectedItems()" ng-show="scan.HasSelectedReturns()" style="margin-left: 10px;">Return Selected Items</span></h3>
                    <div>
                        Search
                        <div class="input-group" style="width: 328px">
                            <input class="form-control" ng-model="scan.ScannedFilter" ng-change="scan.ScannedSearch(scan.ScannedFilter)" />
                            <span class="input-group-addon"><i class="glyphicon glyphicon-search"></i></span>
                        </div>
                    </div>
                    <table ng-table="scan.TableParams2" class="table">
                        <tr ng-repeat="scannedItem in $data track by $index">
                            <td>
                                <input type="checkbox" ng-model="scannedItem.IsSelected" /></td>
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
