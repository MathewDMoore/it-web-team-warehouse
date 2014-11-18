<%@ Page Language="C#" MasterPageFile="~/Master/Redesign.master" AutoEventWireup="true" CodeBehind="ScanOrder.aspx.cs" Inherits="C4InventorySerialization.Content.ScanOrder1" %>

<%@ Import Namespace="ApplicationSource" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="../scripts/underscore-min.js"></script>
    <script type="text/javascript" src="../scripts/services/ScanOrderService.js"></script>
    <script type="text/javascript" src="../scripts/services/firebaseDeliveryService.js"></script>
    <script type="text/javascript" src="../scripts/controllers/ScanOrderController2.js"></script>
    <link href="//maxcdn.bootstrapcdn.com/font-awesome/4.1.0/css/font-awesome.min.css" rel="stylesheet" />
    <script src="https://cdn.firebase.com/js/simple-login/1.6.3/firebase-simple-login.js"></script>
    <script type="text/javascript">
        var app = angular.module("shipApp");
        app.constant("AUTH_TOKEN", "<%=Token%>");
        app.constant("CURRENTUSER", "<%=User.Identity.Name%>");
        app.constant("SERVICES_PATH", "/ship/services/");
    </script>
    <!-- USED FOR EASY PIE CHARTS -->
    <script src="/Scripts/libs/jquery.easypiechart.min.js"></script>

    <script src="/Scripts/libs/jquery.flot.min.js"></script>
    <script src="/Scripts/libs/jquery.flot.time.min.js"></script>
    <script src="/Scripts/libs/jquery.flot.min.js"></script>
    <script src="/Scripts/libs/jquery.flot.time.min.js"></script>
    <script src="/Scripts/libs/jquery.flot.tooltip.min.js"></script>
    <script src="/Scripts/libs/jquery.flot.resize.min.js"></script>
    <script src="/Scripts/libs/jquery.flot.orderBars.js"></script>
    <script src="/Scripts/libs/jquery.flot.pie.min.js"></script>
    <div style="min-height: 140px;">
        <h2>Deliveries</h2>
        <div ng-controller="ScanController as scan" class="well" ng-init="scan.Username='<%=User.Identity.Name %>'">
            <% var init = "";
               if (!string.IsNullOrEmpty(Request.QueryString.Get("delivery")))
               {
                   var delivery = Request.QueryString.Get("delivery");
                   var isInternal = !string.IsNullOrEmpty(Request.QueryString.Get("internal")) && Request.QueryString.Get("internal").Equals("true");
                   init = string.Format("ng-init='scan.LookUp({0},{1})'", delivery, isInternal.ToString().ToLower());
               }%>
            <form style="display: inline-block;" ng-submit="scan.LookUp(scan.OrderIdLookUp,scan.LookUpIsInternal)" <%=init %>>
                <div class="input-group" style="width: 300px;">
                    <input class="form-control" placeholder="Delivery Number or MacId" ng-disabled="scan.IsSearching" focus-select="scan.FocusDeliveryInput"  id="orderIdInput" type="text" ng-model="scan.OrderIdLookUp" ng-blur="scan.LookUp(scan.OrderIdLookUp,scan.LookUpIsInternal)" />
                    <span class="input-group-addon " style="cursor: pointer" id="loadDelivery">{{scan.IsSearching?'Loading...':'Load Delivery'}}</span>
                </div>
                <div style="display: inline-block;">
                    <input name="IsInternal" type="radio" ng-model="scan.LookUpIsInternal" ng-value="false" />Dealer Order
                <input name="IsInternal" type="radio" ng-model="scan.LookUpIsInternal" ng-value="true" />Internal Order
                </div>
            </form>
            <div ng-if="scan.DeliveryActionMessage" class="alert alert-success" style="width: 300px; display: inline-block; margin-left: 20px;">{{scan.DeliveryActionMessage}}</div>
            <div style="margin-top: 10px;" ng-if="scan.Delivery">
                <div class="panel panel-info">
                    <div class="panel-heading">
                        <h3 style="margin: 5px 0px; display: inline-block">Delivery Number: {{scan.Delivery.DeliveryNumber}} </h3>
                        <div style="float: right; position: relative; top: -5px;">
                            <div class="btn-group">
                                <button class="btn btn-lg btn-primary">Export <i class="glyphicon glyphicon-export"></i></button>
                                <button type="button" class="btn btn-lg btn-primary dropdown-toggle" style="height: 46px;" data-toggle="dropdown">
                                    <span class="caret" style="font-size: 20px"></span>
                                </button>
                                <ul class="dropdown-menu" role="menu">
                                    <li><a style="cursor: pointer" ng-click="scan.ExportCSV()">Export to CSV</a></li>
                                    <li><a style="cursor: pointer" ng-click="scan.ExportMacId()" ng-disabled="scan.Delivery.ScannedItems.length >0 ">Export MacIDs</a></li>
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div class="panel-body" style="max-height: 160px;">
                        <h3 style="margin-top: 0px;">Dealer: {{scan.Delivery.DealerName}}/<small>{{scan.Delivery.DealerId}}</small></h3>
                        <div ng-if="scan.Delivery.IsInternal" class="verified text-danger" style="float: right;">[ Internal Order ]</div>
                        <label>Address: {{scan.Delivery.Address}}</label><br />
                        <label>Comments: {{scan.Delivery.Comments}}</label><br />
                        <div style="float: right; margin-top: 20px;">
                            <button class="btn btn-warning" id="returnDelivery" ng-click="scan.ReturnDelivery(scan.Delivery)">Return Entire Delivery</button>
                            <button class="btn btn-danger" id="clearDelivery" ng-click="scan.ClearDelivery(scan.Delivery.DeliveryNumber)">Clear Delivery</button>
                            <button class="btn btn-success" id="VerifiedDelivery" ng-disabled="scan.Delivery.IsVerified || !scan.IsScanComplete()" ng-click="scan.VerifyDelivery(scan.Delivery.DeliveryNumber)">Verify Delivery</button>
                        </div>
                        <div style="-webkit-transform: rotate(-5deg); position: relative; left: 408px; font-size: 32px; border: solid; border-radius: 8px; padding: 8px; border-style: solid; top: -80px; float: right;" class="verified" ng-class="{'text-success':scan.Delivery.IsVerified, 'text-danger':!scan.Delivery.IsVerified}"><i class="glyphicon" ng-class="{'glyphicon-check':scan.Delivery.IsVerified, 'glyphicon-remove-circle' : !scan.Delivery.IsVerified }"></i>{{scan.GetDeliveryStatus()}}</div>

                    </div>
                </div>
                <div ng-show="false" class="animate-show">
                    <div id="donut-container" class="onethird" style="margin-right: 20px;">
                        <div class="well" style="width: 50%;">
                            <div>
                                <button class="btn pull-right" ng-if="scan.Filtered"><i class="glyphicon glyphicon-arrow-left"></i></button>
                                <h2>Item Types</h2>
                                <pie-chart data="scan.ChartData" colors="{{scan.Colors}}" filter="scan.ChartFilter"></pie-chart>
                            </div>
                        </div>
                    </div>
                </div>
                <div style="float: left; margin-bottom: 20px;">
                    <form style="display: inline-block;" ng-submit="scan.VerifyLineitem(scan.SerialCodeLookUp)" <%=init %>>
                        <label>Enter Serial Code: </label>
                        <div class="input-group" style="width: 328px">
                            <input class="form-control" ng-model="scan.SerialCodeLookUp" focus-select="scan.ShouldFocus" select="scan.ShouldSelect" ng-disabled="scan.SavingItem" />
                            <span class="input-group-addon"><i class="glyphicon glyphicon-search"></i></span>
                        </div>
                        <span class="text-info">{{scan.GetCurrentScan()}} of {{scan.GetScanTotals()}} Products Scanned</span>
                    </form>
                </div>
                <div class="alert" style="width: 600px; float: left; margin-left: 10px; position: relative; top: 14px;" ng-class="{'alert-danger':!scan.SerialScanStatus.Success, 'alert-success':scan.SerialScanStatus.Success}" ng-show="scan.SerialScanStatus && scan.SerialScanStatus.Message">&nbsp;&nbsp;{{scan.SerialScanStatus.Message}}</div>
                <div style="width: 200px; float: right; margin-bottom: 0px; text-align: center;" class="alert alert-success" ng-show="scan.IsScanComplete()">Scan Complete<i class="glyphicon glyphicon-check"></i></div>
                <style>
                    .table .header {
                        text-align: left;
                    }
                </style>
                <div ng-if="scan.ActiveKit" style="clear: both;">
                    <h3><i class="fa fa-list-alt" style="margin-right: 5px;"></i>Current Kit - {{kitItem[0].ItemCode}}</h3>
                    <div>
                        <table ng-table="scan.TableParams3" class="table">
                            <tr ng-repeat="kitItem in $data track by $index">
                                <td data-title="'ID'" sortable="'Id'">{{kitItem.Id}}</td>
                                <td data-title="'Kit Code'" sortable="'ItemCode'">{{kitItem.ItemCode}}</td>
                                <td data-title="'Item Code'" sortable="'RealItemCode'">{{kitItem.RealItemCode}}</td>
                                <td data-title="'Description'" sortable="'AltText'">{{kitItem.AltText}}</td>
                                <td data-title="'#'" sortable="'SerialNum'">{{kitItem.SerialNum}}</td>
                                <td data-title="'Serial #'" sortable="'SerialCode'">{{kitItem.SerialCode}}</td>
                                <td data-title="'Scanned By'" sortable="'ScannedBy'">{{kitItem.ScannedBy}}</td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div ng-if="scan.Delivery.NotScannedItems&& scan.Delivery.NotScannedItems.length>0" style="clear: both;">
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
                            <td><i ng-class="{'fa fa-list-alt':item.KitId>0}"></i></td>
                            <td data-title="'ID'" sortable="'Id'">{{item.Id}}</td>
                            <td data-title="'Kit Code'" sortable="'ItemCode'">{{item.ItemCode}}</td>
                            <%--  <td data-title="'Kit Id'" sortable="'KitId'">{{item.KitId}}</td>
                            <td data-title="'Kit Counter'" sortable="'KitCounter'">{{item.KitCounter}}</td>--%>
                            <td data-title="'Item Code'" sortable="'RealItemCode'">{{item.RealItemCode}}</td>
                            <td data-title="'Description'" sortable="'AltText'">{{item.AltText}}</td>
                            <td data-title="'#'" sortable="'SerialNum'">{{item.SerialNum}}</td>
                            <td data-title="'Returned'" sortable="'ReturnedByUser'">{{item.ReturnedByUser}}</td>
                        </tr>
                    </table>
                </div>
                <div ng-if="scan.Delivery.ScannedItems && scan.Delivery.ScannedItems.length>0" style="clear: both;">
                    <h3>Scanned Items <span class="btn btn-warning" ng-click="scan.ReturnSelectedItems()" ng-show="scan.HasSelectedReturns()" style="margin-left: 10px;">Return Selected Items</span></h3>
                    <div>
                        Search
                        <div class="input-group" style="width: 328px">
                            <input class="form-control" ng-model="scan.ScannedFilter" ng-change="scan.ScannedSearch(scan.ScannedFilter)" />
                            <span class="input-group-addon"><i class="glyphicon glyphicon-search"></i></span>
                        </div>
                    </div>
                    <table ng-table="scan.TableParams2" class="table" export-csv="csv">
                        <tr ng-repeat="scannedItem in $data track by $index">
                            <td><i ng-class="{'fa fa-list-alt':scannedItem.KitId>0}"></i></td>
                            <td>
                                <input type="checkbox" ng-model="scannedItem.IsSelected" /></td>
                            <td data-title="'ID'" sortable="'Id'">{{scannedItem.Id}}</td>
                            <td data-title="'Kit Code'" sortable="'ItemCode'">{{scannedItem.ItemCode}}</td>
                            <%-- <td data-title="'Kit Id'" sortable="'KitId'">{{scannedItem.KitId}}</td>
                            <td data-title="'Kit Counter'" sortable="'KitCounter'">{{scannedItem.KitCounter}}</td>--%>
                            <td data-title="'Item Code'" sortable="'RealItemCode'">{{scannedItem.RealItemCode}}</td>
                            <td data-title="'Description'" sortable="'AltText'">{{scannedItem.AltText}}</td>
                            <td data-title="'#'" sortable="'SerialNum'">{{scannedItem.SerialNum}}</td>
                            <td data-title="'Serial #'" sortable="'SerialCode'">{{scannedItem.SerialCode}}</td>
                            <td data-title="'Scanned By'" sortable="'ScannedBy'">{{scannedItem.ScannedBy}}</td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
