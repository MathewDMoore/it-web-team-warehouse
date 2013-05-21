<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MaintainKits.aspx.cs" Inherits="C4InventorySerialization.Admin.MaintainKits" MasterPageFile="~/Master/Site.Master" %>

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
        
        var lastEditedRecord = -1;
        function Grid1_Edit(sender, args) {
            sender.selectRecord(sender.RecordInEditMode);
            lastEditedRecord = sender.RecordInEditMode;
        }

        function Grid1_CancelEdit(sender, args) {
            sender.deselectRecord(lastEditedRecord);
            lastEditedRecord = -1;
        }
        
    </script>

    <div id="LogoutButton">
        <input type="button" value="Logout" onclick="location.href = '../Content/Logout.aspx';" />
        &nbsp; Logged in as: <%= User.Identity.Name %>
    </div>
        <cc1:Grid ID="Grid1" runat="server" AllowRecordSelection="False" AllowGrouping="true" GroupBy="KITCODE" ShowGroupsInfo="true" PageSize="50"
            AutoGenerateColumns="False" DataSourceID="SqlDataSource1">
            <ClientSideEvents OnClientEdit="Grid1_Edit" OnClientUpdate="validate" OnBeforeClientUpdate="validate" OnBeforeClientDelete="onBeforeClientDelete" OnBeforeClientInsert="validate"
                OnClientCancelEdit="Grid1_CancelEdit"/>
            <Columns>
                <cc1:Column DataField="ID" ReadOnly="true" HeaderText="ID" Index="0" Width="60">
                </cc1:Column>
                <cc1:Column DataField="KITCODE" HeaderText="Kit Code" Index="1" Width="200">
                    <TemplateSettings EditTemplateId="KitCodeEdit" />
                </cc1:Column>
                <cc1:Column DataField="ALTERNATETEXT" HeaderText="Alternate Text" Index="2" Width="260">
                </cc1:Column>
                <cc1:Column DataField="QUANTITY" HeaderText="Qty." Index="4" Width="70">
                </cc1:Column>
                <cc1:Column DataField="ITEMCODE" HeaderText="Item Code" Index="4" Width="200">
                        <TemplateSettings EditTemplateId="ItemCodeEdit"></TemplateSettings>
                </cc1:Column>
                <cc1:Column AllowEdit="true" HeaderText="Edit" Index="5" Width="120">
                </cc1:Column>
                <cc1:Column AllowDelete="true" HeaderText="Delete" Index="6" Width="90">
                </cc1:Column>
                <cc1:Column DataField="KITITEMID" Visible="False" Index="7">
                    
                </cc1:Column>
            </Columns>
            <Templates>
                <cc1:GridTemplate ID="KitCodeEdit" runat="server" ControlID="ob_cbo1Textbox" ControlPropertyName="value">
                    <Template>
                        <combo:Combobox AjaxMethod="" id="cbo1" runat="server" DataSourceID="SqlDataSource2"
                            EnableViewState="true" Validate="true" Width="160" Height="75"
                            DataField_Value="sapitemcode" DataField_Text="sapitemcode" Title="Kit Codes"
                            AlignContainer="left" InnerWidth="150" CSSOption="cbo1Option" CSSTextbox="cbo1Textbox" CSSTopContainer="cbo1TopContainer">
                        </combo:Combobox>
                    </Template>
                </cc1:GridTemplate>
                 <cc1:GridTemplate ID="ItemCodeEdit" runat="server" ControlID="ob_cbo1Textbox" ControlPropertyName="value">
                    <Template>
                        <combo:Combobox AjaxMethod="" id="cbo2" runat="server" DataSourceID="GetProducts"
                            EnableViewState="true" Validate="true" Width="160" Height="75" 
                            DataField_Value="itemcode" DataField_Text="itemcode" Title="Item Codes"
                            DataField_Value2="ProductID" DataField_Text2="ProductId" Title2="Product Id"
                            AlignContainer="left" InnerWidth="150" CSSOption="cbo1Option" CSSTextbox="cbo1Textbox" CSSTopContainer="cbo1TopContainer">
                        </combo:Combobox>
                    </Template>
                </cc1:GridTemplate>
            </Templates>
        </cc1:Grid>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server"
            ConnectionString="<%$ ConnectionStrings:InventoryConnectionString %>"
            SelectCommand="SELECT [ID], [KITCODE], [ITEMCODE], [ALTERNATETEXT], [QUANTITY] FROM [C4_MAINTAINKITS]"
            InsertCommand="sp_InsertIntoKit"
            UpdateCommand="sp_UpdateKit"
            DeleteCommand="Delete from C4_MAINTAINKITS where ID=@ID">

            <InsertParameters>
                <asp:Parameter Type="String" Name="KITCODE" />
                <asp:Parameter Type="String" Name="ALTERNATETEXT" />
                <asp:Parameter Type="String" Name="QUANTITY" />
                <asp:Parameter Type="String" Name="ITEMCODE" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Type="String" Name="KITCODE" />
                <asp:Parameter Type="String" Name="ALTERNATETEXT" />
                <asp:Parameter Type="Int32" Name="QUANTITY" />
                <asp:Parameter Type="Int32" Name="ITEMCODE" />
            </UpdateParameters>
            <DeleteParameters>
                <asp:Parameter Type="Int32" Name="ID" />
            </DeleteParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSource2" runat="server"
            ConnectionString="<%$ ConnectionStrings:InventoryConnectionString %>"
            SelectCommand="sp_GetSAPItems" SelectCommandType="StoredProcedure">
        </asp:SqlDataSource>
        <asp:SqlDataSource runat="server" ID="GetProducts" ConnectionString="<%$ ConnectionStrings:InventoryConnectionString %>" 
            SelectCommand="sp_GetProducts" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
</asp:Content>
