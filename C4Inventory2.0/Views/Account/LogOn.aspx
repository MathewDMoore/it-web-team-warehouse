<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<C4Inventory2._0.Models.LogOnModel>" %>
<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="Javascript">
    window.onload = function() {
    document.getElementById('ctl00_MainContent_UserName').focus();
    document.getElementById('ctl00_MainContent_UserName').select();
    }
function capLock(e){
 kc = e.keyCode?e.keyCode:e.which;
 sk = e.shiftKey?e.shiftKey:((kc == 16)?true:false);
 if(((kc >= 65 && kc <= 90) && !sk)||((kc >= 97 && kc <= 122) && sk))
  document.getElementById('divMayus').style.visibility = 'visible';
 else
  document.getElementById('divMayus').style.visibility = 'hidden';
}

</script>

 <h3><asp:Label ID="loggedUser" RunAt="server" ForeColor="Red"/></h3>
<h1>Please Log In</h1>
    <hr>
    <table cellpadding="8">
        <tr>
            <td>User Name:</td>
            <td><asp:TextBox ID="UserName" Text="" RunAt="server"/></td>
        </tr>   
        <tr>
            <td>Password:</td>
            <td><asp:TextBox ID="Password" TextMode="password" onkeypress="capLock(event)" RunAt="server" /><div id="divMayus" style="visibility:hidden">Caps Lock is on.</div> </td>
        </tr>
        <tr>
            <td><asp:Button ID="LoginBtn" Text="Log In" OnClick="Login_Click" RunAt="server" /></td>
            <td></td>
        </tr>
    </table>
    <hr>
    <h3><asp:Label ID="Output" RunAt="server" /></h3>
   

</asp:Content>
