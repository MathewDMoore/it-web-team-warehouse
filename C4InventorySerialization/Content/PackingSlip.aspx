<%@ Page Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeBehind="PackingSlip.aspx.cs" Inherits="C4InventorySerialization.Content.PackingSlip" Title="Untitled Page" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" 
        Font-Size="8pt" Height="400px" Width="400px">
        <ServerReport DisplayName="Test" ReportPath="/reports/Test"
            ReportServerUrl="http://reporting/ReportServer/" />
        <LocalReport ReportPath="reports\Test.rdlc">
            <DataSources>
                <rsweb:ReportDataSource DataSourceId="ObjectDataSource1" 
                    Name="C4SerialNumbers_C4_SERIALNUMBERS_OUT" />
            </DataSources>
        </LocalReport>
    </rsweb:ReportViewer>
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" 
        SelectMethod="GetData" 
        TypeName="C4InventorySerialization.C4SerialNumbersTableAdapters.C4_SERIALNUMBERS_OUTTableAdapter">
    </asp:ObjectDataSource>
</asp:Content>
