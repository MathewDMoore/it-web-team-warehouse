<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MaintainKits.aspx.cs" Inherits="C4InventorySerialization.Content.MaintainKits" MasterPageFile="~/Master/Site.Master" %>

<%@ Register Assembly="obout_Grid_NET" Namespace="Obout.Grid" TagPrefix="cc1" %>
<%@ Register TagPrefix="combo" Namespace="OboutInc.Combobox" Assembly="obout_Combobox_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">

        function onBeforeClientDelete(record) {
            if (confirm("Are you sure you want to delete this record? \n\n" + "ID: " + record.ID + "\nItem Code: " + record.ITEMCODE + "\nAlternate Item Name: " + record.ALTERNATETEXT) == false) {
                return false;
            }
            return true;
        }

        function validate(record) {
            if (record.PRODUCTID.length != 5) {
                alert("Product Code must be five characters");
                return false;
            }
            for (var i in record) {
                if (record[i] == '') {
                    // if value is not filled
                    alert("Column " + i + " is mandatory.");
                    return false;
                }
            }
            return true;

        }
    </script>

    <div id="LogoutButton">
        <input type="button" value="Logout" onclick="location.href = '../Content/Logout.aspx';" />
        &nbsp; Logged in as: <%=User.Identity.Name %>
    </div>
    <form runat="server">
        <cc1:Grid ID="Grid1" runat="server" AllowRecordSelection="False" AllowGrouping="true" GroupBy="ITEMCODE" ShowGroupsInfo="true" PageSize="50"
            AutoGenerateColumns="False" DataSourceID="SqlDataSource1">
            <ClientSideEvents OnBeforeClientInsert="validate" OnBeforeClientUpdate="validate" OnBeforeClientDelete="onBeforeClientDelete" />
            <Columns>
                <cc1:Column DataField="ID" ReadOnly="true" HeaderText="ID" Index="0" Width="60">
                </cc1:Column>
                <cc1:Column DataField="ITEMCODE" HeaderText="Item Code" Index="1" Width="200">
                    <TemplateSettings EditTemplateId="ItemCodeEdit" />
                </cc1:Column>
                <cc1:Column DataField="ALTERNATETEXT" HeaderText="Alternate Text" Index="2" Width="260">
                </cc1:Column>
                <cc1:Column DataField="PRODUCTID" HeaderText="Product" Index="3" Width="100">
                </cc1:Column>
                <cc1:Column DataField="QUANTITY" HeaderText="Qty." Index="4" Width="70">
                </cc1:Column>
                <cc1:Column AllowEdit="true" HeaderText="Edit" Index="5" Width="120">
                </cc1:Column>
                <cc1:Column AllowDelete="true" HeaderText="Delete" Index="6" Width="90">
                </cc1:Column>
            </Columns>
            <Templates>
                <cc1:GridTemplate ID="ItemCodeEdit" runat="server" ControlID="ob_cbo1Textbox" ControlPropertyName="value">
                    <Template>
                        <combo:Combobox AjaxMethod="" runat="server" ID="cbo1" DataSourceID="SqlDataSource2"
                            EnableViewState="true" Validate="true" Width="160" Height="75"
                            DataField_Value="sapitemcode" DataField_Text="sapitemcode" Title="Item Codes"
                            AlignContainer="left" InnerWidth="150" CSSOption="cbo1Option" CSSTextbox="cbo1Textbox" CSSTopContainer="cbo1TopContainer">
                        </combo:Combobox>
                    </Template>
                </cc1:GridTemplate>
            </Templates>
        </cc1:Grid>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server"
            ConnectionString="<%$ ConnectionStrings:InventoryConnectionString %>"
            SelectCommand="SELECT [ID], [ITEMCODE], [ALTERNATETEXT], [PRODUCTID], [QUANTITY] FROM [C4_MAINTAINKITS]"
            InsertCommand="Insert Into C4_MAINTAINKITS (ITEMCODE, ALTERNATETEXT, PRODUCTID, QUANTITY) Values (@ITEMCODE, @ALTERNATETEXT,@PRODUCTID, @QUANTITY)"
            UpdateCommand="UPDATE C4_MAINTAINKITS SET ITEMCODE = @ITEMCODE, ALTERNATETEXT = @ALTERNATETEXT,PRODUCTID=@PRODUCTID, QUANTITY = @QUANTITY where ID=@ID"
            DeleteCommand="Delete from C4_MAINTAINKITS where ID=@ID">

            <InsertParameters>
                <asp:Parameter Type="String" Name="ITEMCODE" />
                <asp:Parameter Type="String" Name="ALTERNATETEXT" />
                <asp:Parameter Type="String" Name="PRODUCTID" />
                <asp:Parameter Type="String" Name="QUANTITY" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Type="String" Name="ITEMCODE" />
                <asp:Parameter Type="String" Name="ALTERNATETEXT" />
                <asp:Parameter Type="String" Name="PRODUCTID" />
                <asp:Parameter Type="Int32" Name="QUANTITY" />
            </UpdateParameters>
            <DeleteParameters>
                <asp:Parameter Type="Int32" Name="ID" />
            </DeleteParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSource2" runat="server"
            ConnectionString="<%$ ConnectionStrings:InventoryConnectionString %>"
            SelectCommand="sp_GetSAPItems" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
    </form>
</asp:Content>
