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
using System.IO;
using System.Data.Common;
using System.Xml;
using System.Data.OleDb;
using System.Text;

namespace LicService
{

    public partial class install_import : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lnkConfigure.Visible = false;
            if (!this.IsPostBack)
            {
                cmbTables.Items.Add("License Table - Used When Validating Serials");
                cmbTables.Items.Add("DisabledLicense Table - For Disabling (Revoking) Licenses/Serials");
                InitSettingFilesDropDown();
            }
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (!FileUpload1.HasFile)
                {
                    lblResult.ForeColor = System.Drawing.Color.Red;
                    lblResult.Text = "Please specify a valid file first.";
                    return;
                }

                int succeddedCount = 0;
                int failedCount = 0;
                string settingsFilePath = Path.Combine(install.basePath, cmbSettingFiles.SelectedValue);
                LicenseServiceClass srvc = new LicenseServiceClass();
                StringBuilder sb = null;
                if (cmbTables.SelectedIndex == 0)
                    sb = srvc.ImportLicensesIntoLicenseTable(settingsFilePath, FileUpload1.PostedFile.InputStream, ref succeddedCount, ref failedCount);
                else
                    sb = srvc.ImportLicensesIntoDisabledLicenseTable(settingsFilePath, FileUpload1.PostedFile.InputStream, ref succeddedCount, ref failedCount);

                lblResult.ForeColor = System.Drawing.Color.Black;
                lblResult.Text = succeddedCount.ToString() + " entries successfully imported into database. ";
                if (failedCount > 0)
                {
                    lblResult.ForeColor = System.Drawing.Color.Red;
                    lblResult.Text += failedCount.ToString() + " entries failed.";
                }

                if (sb != null && sb.Length > 0)
                {
                    txtStatus.Text = sb.ToString();
                    txtStatus.Visible = true;
                }
                else
                {
                    txtStatus.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblResult.Text = "Failed to import into license database:" + ex.Message;
                lblResult.ForeColor = System.Drawing.Color.Red;

            }
        }

        void InitSettingFilesDropDown()
        {
            cmbSettingFiles.Items.Clear();
            foreach (string file in Directory.GetFiles(install.basePath, "*.xml"))
            {
                cmbSettingFiles.Items.Add(Path.GetFileName(file));
            }
            cmbSettingFiles.SelectedIndex = 0;
        }
    }

}