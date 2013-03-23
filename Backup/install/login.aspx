<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected void Submit_OnClick(object sender, EventArgs e)
    {
        if (FormsAuthentication.Authenticate(txtUserName.Text, txtPassword.Text))
        {
            FormsAuthentication.RedirectFromLoginPage(txtUserName.Text, true);
        }
        else
        {
            lblMessage.Text = "Invalid login credentials";
        }
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Login</title>
</head>
<body>
    <form id="form1" runat="server">
        Enter credentials specified when the license service was created from CryptoLicensing App:<br />
        <br />
        Username
        <asp:textbox id="txtUserName" runat="server" Width="150px" /><br />
        Password
        <asp:textbox id="txtPassword" runat="server" TextMode="Password" Width="150px" />
  <p><asp:button id="btnSubmit" OnClick="Submit_OnClick"
                      Text="Login" runat="server" />&nbsp;
      <asp:Label ID="lblMessage" runat="server"></asp:Label>

    </form>
</body>
</html>
