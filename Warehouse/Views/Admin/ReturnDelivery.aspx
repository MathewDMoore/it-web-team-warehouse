<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/Site.Master" CodeBehind="ReturnDelivery.aspx.cs" Inherits="C4InventorySerialization.Admin.ReturnDelivery" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <%     
    String DeliveryNumber = Request.QueryString["DeliveryNum"];
%>

<div id="RedConfirmation">
The Delivery # <%=DeliveryNumber %> has been returned.  All serial numbers are now available.
<br />
</div>

<div id="LogoutButton">
<input type="button" value="Logout" onclick="location.href = '../Content/Logout.aspx';" /> &nbsp; Logged in as: <%=User.Identity.Name %>
</div>
</asp:Content>
