using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LogicNP.CryptoLicensing;
using System.Text;
using System.Drawing;
using System.IO;

namespace LicService
{
    public partial class ActivateMachine : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                InitSettingFilesDropDown();
            }
        }

        const string exceptionPrefix = "##exception";

        protected void btnActivateMachine_Click(object sender, EventArgs e)
        {
            try
            {

                string settingsFilePath = Path.Combine(install.basePath, cmbSettingFiles.SelectedValue);
                LicenseServiceClass srvc = new LicenseServiceClass();
                srvc.Generator.ServiceParamEncryptionMode = ServiceParamEncryptionMode.None;
                string newLicenseCode = srvc.Activate(txtLicenseCode.Text, txtMachineCode.Text, settingsFilePath);

                if (newLicenseCode.StartsWith(exceptionPrefix))
                    throw new Exception(newLicenseCode.Substring(exceptionPrefix.Length));

                lblMessage.Text = "Machine Activated. Machine-Locked License Code Returned By License Service:<br><br>" +
                    newLicenseCode;
                lblMessage.ForeColor = Color.Empty;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.ForeColor = Color.Red;
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
