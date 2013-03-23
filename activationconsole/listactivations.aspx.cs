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
    public partial class ListActivations : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                InitSettingFilesDropDown();
            }

        }

        protected void btnGetActivationData_Click(object sender, EventArgs e)
        {
            try
            {
                lstRecords.Items.Clear();

                string settingsFilePath = Path.Combine(install.basePath, cmbSettingFiles.SelectedValue);
                LicenseServiceClass srvc = new LicenseServiceClass();
                ActivationData data = srvc.GetActivations(txtLicenseCode.Text, settingsFilePath);
                foreach (ActivationPSF apsf in data.activations)
                {
                    string str = apsf.machineCode;
                    if (!chkUseHashedMachineCodes.Checked)
                    {
                        byte[] temp = Convert.FromBase64String(str);
                        str = Encoding.UTF8.GetString(temp);
                    }
                    lstRecords.Items.Add(
                        "Machine Code: " + str
                        + ", IP: " + apsf.ip
                        + ", Activated: " + apsf.activationDateTime.ToString()
                        + ((data.license.ActivationsAreFloating && (data.license.HasFloatingHeartBeatInterval || data.license.HasFloatingLeasePeriod)) ? ", ValidTill: " + apsf.leaseValidTill.ToString() : "")
                        );
                }

                lblStat.Text =
                    "Maximum Activations = " + data.license.MaxActivations.ToString()
                    + ", Used Activations = " + data.numActivations.ToString()
                    + ", Remaining Activations = " + (data.license.MaxActivations - data.numActivations).ToString()
                    + ", Floating = " + data.license.ActivationsAreFloating.ToString()
                    + (data.license.HasFloatingHeartBeatInterval ? ", HeartBeat = " + data.license.FloatingHeartBeatInterval.ToString() + "ms" : "")
                    + (data.license.HasFloatingLeasePeriod ? ", Lease Period = " + data.license.FloatingLeasePeriod.ToString() : "")
                    ;
                lblMessage.Text = string.Empty;

            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
            }
        }

        protected void btnDeleteMachine_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstRecords.SelectedIndex == -1)
                    throw new Exception("Please select a machine from the list above.");

                string str = lstRecords.SelectedItem.Value;

                string[] parts = str.Split(',');
                string mc = parts[0].TrimEnd(' ');
                mc = mc.Replace("Machine Code: ", "");
                if (!chkUseHashedMachineCodes.Checked)
                {
                    byte[] temp = Encoding.UTF8.GetBytes(mc);
                    mc = Convert.ToBase64String(temp);
                }

                string settingsFilePath = Path.Combine(install.basePath, cmbSettingFiles.SelectedValue);
                LicenseServiceClass srvc = new LicenseServiceClass();
                srvc.Generator.ServiceParamEncryptionMode = ServiceParamEncryptionMode.None;
                string dc = srvc.Deactivate(txtLicenseCode.Text, mc, settingsFilePath);
                string message = "Deleted: " + lstRecords.SelectedItem.Value;
                btnGetActivationData_Click(null, null);
                lblMessage.Text = message;
                lblMessage.ForeColor = Color.Empty;

            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.ForeColor = Color.Red;
            }
        }
        protected void chkUseHashedMachineCodes_CheckedChanged(object sender, EventArgs e)
        {
            btnGetActivationData_Click(null, null);
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
