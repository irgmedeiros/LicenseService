<%@ Page Language="C#" AutoEventWireup="true" Inherits="LicService.UsageReportingConsole" Codebehind="console.aspx.cs" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<%@ Register src="controlfilters.ascx" tagname="ControlFilters1" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Usage Reporting Console</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <uc1:ControlFilters1 ID="WebUserControl11" runat="server" />
        <br />
        <hr />
        <table class="style1">
            <tr>
                <td valign="top">
                    <b>Feature Usage</b><br />
                    <br />
    <asp:LinkButton ID="lnkFeatureUsageByFeature" runat="server" 
        onclick="lnkFeatureUsageByFeature_Click">By Feature</asp:LinkButton>
&nbsp;
                    <br />
    <asp:LinkButton ID="lnkFeatureUsageByDay" runat="server" 
        onclick="lnkFeatureUsageByDay_Click">By Day</asp:LinkButton>
                    <br />
                    <br />
    <p>
        <b>Application Runs</b></p>
                    <p>
        <asp:LinkButton ID="lnkAppRunsWithVersion" runat="server" 
            onclick="lnkAppRunsWithVersion_Click">Total (with version)</asp:LinkButton>
&nbsp;<br />
        <asp:LinkButton ID="lnkAppRunsWithoutVersion" runat="server" 
            onclick="lnkAppRunsWithoutVersion_Click">Total (without version)</asp:LinkButton>
                        <br />
                        <asp:LinkButton ID="lnkAppRunsByDay" runat="server" 
                            onclick="lnkAppRunsByDay_Click">By Day</asp:LinkButton>
    </p>
                    <p>
                        &nbsp;</p>
                    <p>
                        <b>System Profile</b></p>
                    <p>
        <asp:LinkButton ID="lnkSysProfileOS" runat="server" onclick="lnkSysProfileOS_Click">OS</asp:LinkButton>
                        <br />
        <asp:LinkButton ID="lnkSysProfileCLR" runat="server" onclick="lnkSysProfileCLR_Click">.Net Runtime</asp:LinkButton>
    </p>
                </td>
                <td valign="top">
        <asp:Chart ID="Chart1" runat="server" Width="1170px" Height="622px">
            <Series>
                <asp:Series Name="Series1">
                </asp:Series>
            </Series>
            <ChartAreas>
                <asp:ChartArea Name="ChartArea1">
                </asp:ChartArea>
            </ChartAreas>
        </asp:Chart>
        &nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
            </tr>
        </table>
        <br />
        <br />
        &nbsp;<br />
    </div>
    </form>
</body>
</html>
