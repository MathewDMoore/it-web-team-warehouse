<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true"
    CodeBehind="Returns.aspx.cs" Inherits="C4InventorySerialization.Content.Returns" %>
<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET, Version=7.0.5.0, Culture=neutral, PublicKeyToken=5ddc49d3b53e3f98" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Label ID="CheckConfiguration" Visible="false" Text="" runat="server" />
    <asp:Label ID="ErrorRecords" Visible="false" Text="" runat="server" />
    <table width="100%">
        <tr>
            <td>
                <div id="pgTitle">
                    Returns</div>
            </td>
            <td>
                <div id="loggedUser">
                    Logged in as:
                    <%=User.Identity.Name %></div>
            </td>
        </tr>
    </table>
    <input type="hidden" id="Warning" value="<%=warning%>" />
    <input type="hidden" id="VerifyError" value="<%=verifyError%>" />
    <div id="DeliveryEnter">
        Delivery Number:
        <input id="deliverytext" name="t1" type="text" size="10"/>
        <input type="button" value="Return Item" onclick="ReturnDelivery(this.form.t1.value);" />
        <input type="button" value="Return Entire Delivery" onclick="ReturnDelivery(this.form.t1.value);" />
        <br />
    </div>
    <div id="DeliveryDetails">
        <table>
            <tr>
                <td>
                    <obout:grid id="Grid2" allowaddingrecords="false" allowsorting="false" showfooter="false"
                        allowdataaccessonserver="true" showheader="false" onrebind="RebindGrid" callbackmode="true"
                        showcolumnsfooter="false" runat="server" autogeneratecolumns="False">
        <Columns>
            <obout:Column DataField="COLUMN1" Width="150" HeaderText=" " Index="0">
            </obout:Column>
            <obout:Column DataField="COLUMN2" Width="625" Wrap="true" HeaderText=" " Index="1">
            </obout:Column>
        </Columns>
    </obout:grid>
                </td>
                <td>
                    <asp:Image ID="verifiedimg" runat="server" ImageUrl="~/images/DeliveryVerified_0.jpg"
                        Visible="false" />
                    <asp:Image ID="notverifiedimg" runat="server" ImageUrl="~/images/NotVerified_0.jpg"
                        Visible="false" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
