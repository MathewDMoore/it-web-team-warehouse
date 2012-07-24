<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeBehind="ReturnDeliveryByLineItem.aspx.cs" Inherits="C4InventorySerialization.Admin.ReturnDeliveryByLineItem" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <%     
    String lineNum = Request.QueryString["LineNum"];
    //String queryString = lineNum.Replace(' ', ',');
%>

<div id="RedConfirmation">
The ID #(s) <%=lineNum %>  have been returned.
<br />
</div>

<div id="LogoutButton">
<input type="button" value="Logout" onclick="location.href = '../Content/Logout.aspx';" /> &nbsp; Logged in as: <%=User.Identity.Name %>
</div>
</asp:Content>
