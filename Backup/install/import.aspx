<%@ Page Language="C#" AutoEventWireup="true" Inherits="LicService.install_import" Codebehind="import.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Import License Codes</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <strong>Import License codes Into Database</strong><br />
        <hr />
        <br />
        <strong>Select Settings File: </strong>
        <asp:DropDownList ID="cmbSettingFiles" runat="server" AutoPostBack="True" Width="231px">
        </asp:DropDownList><br />
        <br />
        <br />
        Choose file containing license codes<br />
        <asp:FileUpload ID="FileUpload1" runat="server" Width="459px" /><br />
        <br />
        Import into following database table<br />
        <asp:DropDownList ID="cmbTables" runat="server" Width="450px">
        </asp:DropDownList><br />
        <br />
        <asp:Button ID="btnImport" runat="server" Text="Import" OnClick="btnImport_Click" />
        &nbsp; &nbsp;&nbsp;
        <asp:HyperLink ID="lnkConfigure" runat="server" NavigateUrl="~/install/install.aspx">Configure Database</asp:HyperLink><br />
        <asp:Label ID="lblResult" runat="server"></asp:Label><br />
        <asp:TextBox ID="txtStatus" runat="server" TextMode="MultiLine" Width="100%" Height="170px" Visible="False"></asp:TextBox><br />
        </div>
    </form>
</body>
</html>
