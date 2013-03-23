<%@ Page Language="C#" AutoEventWireup="true" Inherits="LicService.install" Codebehind="install.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div title="License Database Configuration">
        <strong>License Service Configuration</strong><br />
        <hr />
        <br />
        <strong>Select Settings File:
            <asp:DropDownList ID="cmbSettingFiles" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbSettingFiles_SelectedIndexChanged"
                Width="231px">
            </asp:DropDownList><br />
        </strong>
        <hr />
        <b>1. </b>Specify settings...<br />
        <br />
        <b>Connection String:</b>
        <asp:TextBox ID="txtConnectionString" runat="server" Width="530px"></asp:TextBox>
        <br />
        <br />
        Example connection strings:<br />
        For Access Database: <strong>Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\LicenseService.mdb<br />
        </strong>For SQL Server, SQl Server Express, etc:<strong> Data Source=.\SQLEXPRESS;Initial Catalog=LicenseService;Integrated Security=SSPI;  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[
Note:</strong> Make sure that the database specifified via the<strong> Initial Catalog </strong>parameter is present or created.]<strong><br />
        <br />
        </strong>
        More connection string samples:<strong>
        <asp:HyperLink ID="HyperLink1" runat="server" 
            NavigateUrl="http://www.connectionstrings.com" Target="_blank">http://www.connectionstrings.com</asp:HyperLink>
        <br />
        <br />
        </strong>
        Note: See the <b>&quot;Supported Databases&quot; </b>topic in the help file for detailed 
        information on how to to use other database like MySQL, SQL CE, PostGreSQL, 
        Oracle, etc<br />
        <br />
        <asp:Button ID="btnTestConnection" runat="server" Text="Test Database Connection" OnClick="btnTestConnection_Click" />
        <asp:Label ID="lblTestConnectionResult" runat="server"></asp:Label>
        <br />
        <br />
        <br />
        <b>Database
        Table Names Prefix:</b> &nbsp;<asp:TextBox ID="txtTableNamePrefix" runat="server"></asp:TextBox><br />
        <br />
        <asp:CheckBox ID="chkRecordIPAddress" runat="server" 
            Text=" Record IP Address on Activation" Font-Bold="True" />
        <br />
        <br />
        <br />
        <b>2. </b>
        <asp:Button ID="btnInstall" runat="server" OnClick="btnInstall_Click" Text="Save Settings" />
        <asp:Label ID="lblSaveResult" runat="server"></asp:Label>
        <br />
        <br />&nbsp;<br />
        <b>3. </b>
        <asp:Button
            ID="btnCreateTables" runat="server" OnClick="btnCreateTables_Click" 
            Text="Create / Initialize Database Tables" />
        <strong><br />
            </strong><br />
        <asp:TextBox ID="txtStatus" runat="server" TextMode="MultiLine" Width="100%" Height="170px" Visible="False"></asp:TextBox><br />
        <br />
        <br />
        <br />
        <br />
        <br />
    
    </div>
    </form>
</body>
</html>
