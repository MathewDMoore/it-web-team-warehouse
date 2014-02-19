<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MaintainKits.aspx.cs" Inherits="C4InventorySerialization.Admin.MaintainKits" MasterPageFile="~/Master/Site.Master" %>

<%@ Register Assembly="obout_Grid_NET" Namespace="Obout.Grid" TagPrefix="cc1" %>
<%@ Register TagPrefix="obout" Namespace="Obout.ComboBox" Assembly="obout_ComboBox" %>
<%@ Register TagPrefix="combo" Namespace="OboutInc.Combobox" Assembly="obout_Combobox_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" src="../scripts/MaintainKits.js"></script>

    <div id="LogoutButton">
        <input type="button" value="Logout" onclick="location.href = '../Content/Logout.aspx';" />
        &nbsp; Logged in as: <%= User.Identity.Name %>
    </div>
    <cc1:Grid ID="Grid1" runat="server" AllowRecordSelection="False" AllowGrouping="true" GroupBy="KITCODE" ShowGroupsInfo="true" PageSize="50"
        AutoGenerateColumns="False" DataSourceID="SqlDataSource1" OnUpdateCommand="UpdateRecord" OnDeleteCommand="DeleteRecord" OnInsertCommand="UpdateRecord">
        <ClientSideEvents  OnClientUpdate="validate" OnBeforeClientUpdate="validate" OnBeforeClientDelete="onBeforeClientDelete" OnBeforeClientInsert="validate"
            />
        <Columns>
            <cc1:Column DataField="KITID" ReadOnly="true" HeaderText="Kit Id" Index="0" Width="70">
            </cc1:Column>
            <cc1:Column DataField="KITCODE" HeaderText="Kit Code" Index="1" Width="200">
                <TemplateSettings EditTemplateId="KitCodeEdit" />
            </cc1:Column>
            <cc1:Column DataField="ALTERNATETEXT" HeaderText="Alternate Text" Index="2" Width="260">
            </cc1:Column>
            <cc1:Column DataField="QUANTITY" HeaderText="Qty." Index="3" Width="70">
            </cc1:Column>
            <cc1:Column DataField="ITEMCODE" HeaderText="Item Code" Index="4" Width="200">
                <TemplateSettings EditTemplateId="ItemCodeEdit"></TemplateSettings>
            </cc1:Column>
             <cc1:Column AllowEdit="true" HeaderText="Modify" Index="5" Width="120">
                <TemplateSettings TemplateId="EditBtnTemplate" EditTemplateId="UpdateBtnTemplate" />
            </cc1:Column>
                    <cc1:Column AllowDelete="True" HeaderText="Delete" Index="6" Width="100">
            </cc1:Column>
            <cc1:Column DataField="KITITEMID"  Visible="False"></cc1:Column>
        </Columns>
        <Templates>
            <cc1:GridTemplate ID="KitCodeEdit" runat="server" ControlID="ob_cbo1Textbox" ControlPropertyName="value">
                <Template>
                    <combo:Combobox AjaxMethod="" ID="cbo1" runat="server" DataSourceID="SqlDataSource2"
                        EnableViewState="true" Validate="true" Width="160" Height="75"
                        DataField_Value="sapitemcode" DataField_Text="sapitemcode" Title="Kit Codes"
                        AlignContainer="left" InnerWidth="150" CSSOption="cbo1Option" CSSTextbox="cbo1Textbox" CSSTopContainer="cbo1TopContainer">
                    </combo:Combobox>
                </Template>
            </cc1:GridTemplate>
            <cc1:GridTemplate ID="ItemCodeEdit" runat="server" ControlID="ob_cbo2Textbox" ControlPropertyName="value">
                <Template>
                    <combo:Combobox AjaxMethod="" ID="cbo2" runat="server" DataSourceID="GetProducts"
                        EnableViewState="true" Validate="true" Width="160" Height="75" 
                        DataField_Value="itemcode" DataField_Text="itemcode" Title="Item Codes"
                        AlignContainer="left" InnerWidth="150" CSSOption="cbo1Option" CSSTextbox="cbo2Textbox" CSSTopContainer="cbo2TopContainer">
                    </combo:Combobox>
                </Template>
            </cc1:GridTemplate>
            <cc1:GridTemplate runat="server" ID="UpdateBtnTemplate">
                <Template>
                    <a type="text/html" id="btnUpdate" tabindex="0" onclick="Grid1.update_record(this)">Update</a>
                    |<a type="text/html" id="btnCancel" tabindex="2" onclick="Grid1.cancel_edit(this)">Cancel</a>
                </Template>
            </cc1:GridTemplate>

             <cc1:GridTemplate runat="server" ID="EditBtnTemplate">
                <Template>
                    <a type="text/html" id="btnEdit" onclick="Grid1.edit_record(this)">Edit</a>
                </Template>
            </cc1:GridTemplate>
        </Templates>
    </cc1:Grid>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server"
        ConnectionString="<%$ ConnectionStrings:InventoryConnectionString %>"
        SelectCommand="sp_GetKits"
        InsertCommand="sp_KitUpdate"
        UpdateCommand="sp_KitUpdate"
        DeleteCommand="sp_KitDeleteItem">
        <InsertParameters>
            <asp:Parameter Type="String" Name="ALTERNATETEXT" />
            <asp:Parameter Type="String" Name="KITCODE" />
            <asp:Parameter Type="String" Name="ITEMCODE" />
            <asp:Parameter Type="Int32" Name="QUANTITY" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Type="String" Name="ALTERNATETEXT" />
            <asp:Parameter Type="String" Name="ITEMCODE" />
            <asp:Parameter Type="String" Name="KITCODE" />
            <asp:Parameter Type="Int32" Name="QUANTITY" />
            <asp:Parameter Type="String" Name="KITITEMID"/>
        </UpdateParameters>
        <DeleteParameters>
            <asp:Parameter Type="Int32" Name="ID" />
            <asp:Parameter Type="String" Name="ITEMCODE"/>
            <asp:Parameter Type="Int32" Name="KITITEMID"/>
        </DeleteParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource2" runat="server"
        ConnectionString="<%$ ConnectionStrings:InventoryConnectionString %>"
        SelectCommand="sp_GetSAPItems" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
    <asp:SqlDataSource runat="server" ID="GetProducts" ConnectionString="<%$ ConnectionStrings:InventoryConnectionString %>"
        SelectCommand="sp_GetProducts" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
</asp:Content>
