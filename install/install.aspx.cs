using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.Common;
using System.Data.OleDb;
using System.Xml;
using System.IO;

namespace LicService
{

    public partial class install : System.Web.UI.Page
    {
        internal static string basePath = Path.Combine(HttpRuntime.AppDomainAppPath, @"App_Data");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                InitSettingFilesDropDown();
                cmbSettingFiles_SelectedIndexChanged(null, null);
            }

            this.Title = "License Database Configuration";
        }

        void InitSettingFilesDropDown()
        {
            cmbSettingFiles.Items.Clear();
            foreach (string file in Directory.GetFiles(basePath, "*.xml"))
            {
                cmbSettingFiles.Items.Add(Path.GetFileName(file));
            }
            cmbSettingFiles.SelectedIndex = 0;
        }

        protected void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                LicenseServiceClass srvc = new LicenseServiceClass();
                srvc.TestDBConnection(txtConnectionString.Text);
                lblTestConnectionResult.Text = "Connection established successfully with database";
                lblTestConnectionResult.ForeColor = System.Drawing.Color.Black;

            }
            catch (Exception ex)
            {
                lblTestConnectionResult.Text = "Connection cannot be established with database. " + ex.Message;
                lblTestConnectionResult.ForeColor = System.Drawing.Color.Red;
            }
        }


        protected void btnInstall_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = Path.Combine(basePath, cmbSettingFiles.SelectedValue);
                XmlDocument document = new XmlDocument();
                document.Load(filename);
                XmlNode node = document.SelectSingleNode("/settings");
                LogicNP.CryptoLicensing.LicenseService.AddAttribute(node, "connectionstring", txtConnectionString.Text);
                LogicNP.CryptoLicensing.LicenseService.AddAttribute(node, "tablenameprefix", txtTableNamePrefix.Text);
                LogicNP.CryptoLicensing.LicenseService.AddAttribute(node, "recordIPAddress", chkRecordIPAddress.Checked.ToString());
                document.Save(filename);

                lblSaveResult.Text = "Settings successfully saved";
                lblSaveResult.ForeColor = System.Drawing.Color.Black;
            }
            catch (Exception ex)
            {
                lblSaveResult.Text = ex.Message;
                lblSaveResult.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnCreateTables_Click(object sender, EventArgs e)
        {
            try
            {
                LicenseServiceClass srvc = new LicenseServiceClass();
                txtStatus.Text = srvc.CreateDBTables(txtConnectionString.Text, txtTableNamePrefix.Text);
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Failed to create tables in database : " + ex.Message;
            }
            txtStatus.Visible = true;
        }
        protected void cmbSettingFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(basePath, cmbSettingFiles.SelectedValue));

            XmlNode settingsNode = doc.SelectSingleNode("/settings");
            txtConnectionString.Text = settingsNode.Attributes["connectionstring"].Value;
            txtTableNamePrefix.Text = settingsNode.Attributes["tablenameprefix"].Value;
            try
            {
                bool recordIPAddress = false;
                bool.TryParse(settingsNode.Attributes["recordIPAddress"].Value, out recordIPAddress);
                chkRecordIPAddress.Checked = recordIPAddress;
            }
            catch { }

        }

    }

}