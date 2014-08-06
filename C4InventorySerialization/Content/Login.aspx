<%@ Page Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="C4InventorySerialization.Login" EnableSessionState="ReadOnly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True"></asp:ScriptManager>
    <script src="../scripts/LogIn.js" type="text/javascript"></script>
    <link href="//netdna.bootstrapcdn.com/bootstrap/3.0.3/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <h3><asp:Label ID="loggedUser" runat="server" ForeColor="Red" /></h3>
    <h1>Please Log In</h1>
    <hr>
    <table cellpadding="8">
        <tr>
            <td>User Name:</td>
            <td>
                <asp:TextBox ID="UserName" Text="" runat="server" /></td>
        </tr>
        <tr>
            <td>Password:</td>
            <td>
                <asp:TextBox ID="Password" TextMode="password" onkeypress="capLock(event)" runat="server" /><div id="divMayus" style="visibility: hidden">Caps Lock is on.</div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="LoginBtn" Text="Log In" OnClick="Login_Click" runat="server" /></td>
            <%--<input type="button" id="LoginBtn" value="Log In" onclick="UserNameCheck()" />--%>
            <td></td>
        </tr>
    </table>

    <!-- /.modal -->
    <hr>
    <h4>
        <h3>
            <asp:Label ID="Output" runat="server" /></h3>
        <label id="errorMessage" />
    </h4>
        
    <!-- Modal code for Contractor Login. -->
    <%--<div class="modal fade" id="myModal">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Contrator Details</h4>
                </div>
                <div class="modal-body">
                    <p>Please enter your first and last name:</p>
                    First Name: 
                    <input type="text" id="modalFirstName" />
                    Last Name: 
                    <input type="text" id="modalLastName" />
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary" onclick="SubmitContractorDetails()">Save changes</button>
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>--%>
</asp:Content>
