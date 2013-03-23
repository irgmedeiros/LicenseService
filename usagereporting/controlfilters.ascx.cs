using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LicService
{
    public partial class ControlFilters : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                cmbMmeoryOperator.Items.Add(" = ");
                cmbMmeoryOperator.Items.Add(" <> ");
                cmbMmeoryOperator.Items.Add(" >= ");
                cmbMmeoryOperator.Items.Add(" > ");
                cmbMmeoryOperator.Items.Add(" <= ");
                cmbMmeoryOperator.Items.Add(" < ");

                cmbAppVersionOperator.Items.Add(" = ");
                cmbAppVersionOperator.Items.Add(" <> ");
                cmbAppVersionOperator.Items.Add(" >= ");
                cmbAppVersionOperator.Items.Add(" > ");
                cmbAppVersionOperator.Items.Add(" <= ");
                cmbAppVersionOperator.Items.Add(" < ");

                cmbRuntime.Items.Add(" = ");
                cmbRuntime.Items.Add(" <> ");
                cmbRuntime.Items.Add(" >= ");
                cmbRuntime.Items.Add(" > ");
                cmbRuntime.Items.Add(" <= ");
                cmbRuntime.Items.Add(" < ");
            }

        }

        internal System.Web.UI.WebControls.TextBox TxtUserName
        {
            get
            {
                return txtUserName;
            }
        }

        internal System.Web.UI.WebControls.TextBox TxtFeatureName
        {
            get
            {
                return txtFeatureName;
            }
        }


        internal System.Web.UI.WebControls.Calendar DtTo
        {
            get
            {
                return dtTo;
            }
        }


        internal System.Web.UI.WebControls.Calendar DtFrom
        {
            get
            {
                return dtFrom;
            }
        }

        internal System.Web.UI.WebControls.TextBox TxtOSName
        {
            get
            {
                return txtOSName;
            }
        }

        internal System.Web.UI.WebControls.Button BtnApply
        {
            get
            {
                return Button1;
            }
        }

        internal System.Web.UI.WebControls.Button BtnClearFilters
        {
            get
            {
                return btnClearFilters;
            }
        }

        internal System.Web.UI.WebControls.TextBox TxtAppName
        {
            get
            {
                return txtAppName;
            }
        }
        internal System.Web.UI.WebControls.DropDownList CmbAppVersionOperator
        {
            get
            {
                return cmbAppVersionOperator;
            }
        }
        internal System.Web.UI.WebControls.TextBox TxtAppVersion
        {
            get
            {
                return txtAppVersion;
            }
        }

        internal System.Web.UI.WebControls.DropDownList CmbMmeoryOperator
        {
            get
            {
                return cmbMmeoryOperator;
            }
        }

        internal System.Web.UI.WebControls.TextBox TxtLicenseID
        {
            get
            {
                return txtLicenseID;
            }
        }

        internal System.Web.UI.WebControls.TextBox TxtMemoryValue
        {
            get
            {
                return txtMemoryValue;
            }
        }

        internal System.Web.UI.WebControls.TextBox TxtRuntime
        {
            get
            {
                return txtRuntime;
            }
        }

        internal System.Web.UI.WebControls.DropDownList CmbRuntime
        {
            get
            {
                return cmbRuntime;
            }
        }

        internal System.Web.UI.WebControls.DropDownList CmbSettingFiles
        {
            get
            {
                return cmbSettingFiles;
            }
        }

        internal bool HasDateFromConstraint
        {
            get
            {
                return chkDateFrom.Checked && dtFrom.SelectedDate.Year > 2000 && dtFrom.SelectedDate.Year < 2020;
            }
        }

        internal bool HasDateToConstraint
        {
            get
            {
                return chkDateTo.Checked && dtTo.SelectedDate.Year > 2000 && dtTo.SelectedDate.Year < 2020;
            }
        }

        internal bool HasMemoryConstraint
        {
            get
            {
                return !string.IsNullOrEmpty(txtMemoryValue.Text);
            }
        }

        internal bool HasLicenseIDConstraint
        {
            get
            {
                return !string.IsNullOrEmpty(txtLicenseID.Text);
            }
        }

        internal bool HasAppNameConstraint
        {
            get
            {
                return !string.IsNullOrEmpty(txtAppName.Text);
            }
        }

        internal bool HasAppVersionConstraint
        {
            get
            {
                return !string.IsNullOrEmpty(txtAppVersion.Text);
            }
        }

        internal bool HasUserNameConstraint
        {
            get
            {
                return !string.IsNullOrEmpty(txtUserName.Text);
            }
        }

        internal bool HasOSNameConstraint
        {
            get
            {
                return !string.IsNullOrEmpty(txtOSName.Text);
            }
        }

        internal bool HasFeatureNameConstraint
        {
            get
            {
                return !string.IsNullOrEmpty(txtFeatureName.Text);
            }
        }

        internal bool HasRuntimeConstraint
        {
            get
            {
                return !string.IsNullOrEmpty(txtRuntime.Text);
            }
        }

        internal bool HasUsageReportTableConstraints
        {
            get
            {
                return HasUserNameConstraint || HasOSNameConstraint || HasMemoryConstraint || HasAppNameConstraint
                    || HasAppVersionConstraint || HasDateFromConstraint || HasDateToConstraint || HasLicenseIDConstraint || HasRuntimeConstraint;
            }
        }

        internal bool HasFeatureTableConstraints
        {
            get
            {
                return HasFeatureNameConstraint;
            }
        }

        internal bool HasConstraints
        {
            get
            {
                return HasFeatureTableConstraints || HasUsageReportTableConstraints;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }

        internal void ClearFilters()
        {
            txtAppName.Text = string.Empty;
            txtAppVersion.Text = string.Empty;
            txtUserName.Text = string.Empty;
            txtOSName.Text = string.Empty;
            txtMemoryValue.Text = string.Empty;
            txtFeatureName.Text = string.Empty;
            chkDateFrom.Checked = false;
            chkDateTo.Checked = false;
            txtLicenseID.Text = string.Empty;
            txtRuntime.Text = string.Empty;
        }

    }
}