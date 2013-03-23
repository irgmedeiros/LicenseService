<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="controlfilters.ascx.cs" Inherits="LicService.ControlFilters" %>
<style type="text/css">
    .style1
    {
        width: 100%;
    }
    .style2
    {
        width: 286px;
        height: 205px;
    }
    .style3
    {
        width: 292px;
        height: 205px;
    }
    .style4
    {
        width: 228px;
        height: 205px;
    }
    .style5
    {
        height: 205px;
    }
</style>
<table class="style1">
    <tr>
        <td class="style4" valign="top">
            AssociatedLicenseID<br />
            <asp:TextBox ID="txtLicenseID" runat="server"></asp:TextBox>
&nbsp;
            <br />
                        User<br />
    <asp:TextBox ID="txtUserName" runat="server"></asp:TextBox>
&nbsp;<br />
                        OS<br />
    <asp:TextBox ID="txtOSName" runat="server"></asp:TextBox>
&nbsp;<br />
            <br />
    Feature Name
            <br />
    <asp:TextBox ID="txtFeatureName" runat="server" Width="158px"></asp:TextBox>
        </td>
        <td class="style3" valign="top">
            AppName    AppName<br />
    <asp:TextBox ID="txtAppName" runat="server"></asp:TextBox>
&nbsp;<br />
             AppVersion
            <br />
    <asp:DropDownList ID="cmbAppVersionOperator" runat="server">
    </asp:DropDownList>
&nbsp;<asp:TextBox ID="txtAppVersion" runat="server"></asp:TextBox>
            <br />
&nbsp;
            <br />
            Memory      Memory<br />
    <asp:DropDownList ID="cmbMmeoryOperator" runat="server">
    </asp:DropDownList>
&nbsp;<asp:TextBox ID="txtMemoryValue" runat="server"></asp:TextBox>
&nbsp;
            <br />
            .Net Runtime             <br />
            <asp:DropDownList ID="cmbRuntime" runat="server">
            </asp:DropDownList>
&nbsp;<asp:TextBox ID="txtRuntime" runat="server" Width="128px"></asp:TextBox>
        </td>
        <td class="style2">
            <asp:CheckBox ID="chkDateFrom" runat="server" Text="Date From:" />
&nbsp;<asp:Calendar ID="dtFrom" runat="server" Height="113px" Width="62px"></asp:Calendar>
        </td>
        <td class="style5">
            <asp:CheckBox ID="chkDateTo" runat="server" Text="Date To:" />
            <asp:Calendar ID="dtTo" runat="server" Height="117px" Width="43px"></asp:Calendar>
        </td>
        <td class="style5">
        <br />
        <strong>Select Settings File:<br />
&nbsp;<asp:DropDownList ID="cmbSettingFiles" runat="server" AutoPostBack="True"
                Width="231px">
            </asp:DropDownList>
            <br />
            <br />
        </strong>
<asp:Button ID="Button1" runat="server" Text="Apply Filter" onclick="Button1_Click" />
&nbsp;<br />
            <br />
            <asp:Button ID="btnClearFilters" runat="server" 
    Text="Clear All Filters" />

        </td>
    </tr>
</table>


