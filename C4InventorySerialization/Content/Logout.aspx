<%@ Page Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeBehind="Logout.aspx.cs" Inherits="C4InventorySerialization.Content.Logout" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <table width="100%">
    <tr>
        <td align="center">
            You have been logged out.
            <asp:hyperlink id="hlLogin" runat="server" 
            navigateurl="login.aspx">Log back in.</asp:hyperlink>
        </td>
    </tr>
</table>
<%--    <%Response.Redirect("../Content/ScanSerialNumber.aspx");%>--%>
</asp:Content>
