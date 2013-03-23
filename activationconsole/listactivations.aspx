<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="listactivations.aspx.cs" Inherits="LicService.ListActivations" %>
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
        <b>Enter License Code:</b>
        <asp:TextBox ID="txtLicenseCode" runat="server" Width="757px"></asp:TextBox>
        <br />
        <asp:Button ID="btnGetActivationData" runat="server" 
            OnClick="btnGetActivationData_Click" Text="Get Activations" /><br />
        <br />
        <strong>License Information:<br />
            <asp:Label ID="lblStat" runat="server"></asp:Label><br />
            <br />
            Activation Records (Machine Codes):
            <asp:CheckBox ID="chkUseHashedMachineCodes" runat="server" OnCheckedChanged="chkUseHashedMachineCodes_CheckedChanged"
                Text="Use Hashed Machine Codes" AutoPostBack="True" Checked="True" /></strong><br />
        <asp:ListBox ID="lstRecords" runat="server" Height="173px" Width="770px"></asp:ListBox><br />
        <br />
        <asp:Button ID="btnDeleteMachine" runat="server" 
            OnClick="btnDeleteMachine_Click" Text="Deactivate Selected Machine"
            Width="260px" />
        <br />
        <br />
        <asp:Label ID="lblMessage" runat="server"></asp:Label></div>
    </form>
</body>
</html>
