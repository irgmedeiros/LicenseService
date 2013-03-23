<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="activatemachine.aspx.cs" Inherits="LicService.ActivateMachine" %>
<%@ Register src="DashBoard.ascx" tagname="DashBoard" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <uc1:DashBoard ID="DashBoard1" runat="server" />
    <div>
        <b>Select Settings File:</b> 
        <asp:DropDownList ID="cmbSettingFiles" runat="server" AutoPostBack="True" Width="231px">
        </asp:DropDownList>
        <br />
        <br />
        <br />
        <b>Enter license code:</b>
        <asp:TextBox ID="txtLicenseCode" runat="server" Width="757px"></asp:TextBox>
        <br />
        <br />
        <br />
        <b>Enter machine code of machine to activate:</b><br />
        <asp:TextBox ID="txtMachineCode" runat="server" Width="757px"></asp:TextBox>
        <br />
        <br />
        <br />
        <asp:Button ID="btnActivate" runat="server" 
            OnClick="btnActivateMachine_Click" Text="Activate"
            Width="260px" />
        <br />
        <br />
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
        <br />
        <br />
        </div>
    </form>
</body>
</html>
