using System;
using System.Web;
using System.Globalization;
using System.IO;

using LogicNP.CryptoLicensing;

namespace LicService
{
    public partial class RegNowHandler : System.Web.UI.Page
    {

        DateTime GetHttpParam(string paramName, string format, DateTime defaultValue)
        {
            string paramValue = Request.Params[paramName];
            if (paramValue != null)
            {
                try
                {
                    return DateTime.ParseExact(paramValue, format, CultureInfo.InvariantCulture);
                }
                catch { }
            }

            return defaultValue;
        }

        int GetHttpParam(string paramName, int defaultValue)
        {
            string paramValue = Request.Params[paramName];
            if (paramValue != null)
            {
                try
                {
                    return int.Parse(paramValue);
                }
                catch { }
            }

            return defaultValue;
        }

        string GetHttpParam(string paramName, string defaultValue)
        {
            string paramValue = Request.Params[paramName];
            if (paramValue != null)
            {
                return paramValue;
            }

            return defaultValue;
        }

        // START SETTINGS: Change as desired

        // General settings
        string profileName = string.Empty; // Select predefined profile, if any, to be used when generating license

        // If set to true, any exceptions that occur will be logged in time-stamped .log files in the App_Data folder.
        bool log_exceptions = true;


        // END SETTINGS

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Load license project file
                CryptoLicenseGenerator gen = CryptoLicenseGenerator.FromSettingsFile(Path.Combine(HttpRuntime.AppDomainAppPath, LicenseService.MakePortablePath(@"App_Data\settings.xml")));

                // ***IMPORTANT***: Set your 'CryptoLicensing License Service' license code below.
                // It can be found in the email containing the license information that you received from LogicNP when you purchased the product.
                // LEAVE BLANK when using evaluation version of CryptoLicensing.
                gen.SetLicenseCode("");

                // If profile is specified above, select it so that generated license 
                // has same settings as specified by profile
                if (profileName.Length > 0)
                    gen.SetActiveProfile(profileName);

                // OPTIONAL: Set 'Numer of Users' setting of license to the quantity purchased
                //gen.NumberOfUsers = (short)GetHttpParam("quantity", 1);

                // OPTIONAL: Use purchase data if desired
                // RegNow notifies the purchase date in format specified in following line
                //DateTime datePurchased = GetHttpParam("date", "MM/dd/yyyy",DateTime.Now);

                // Extract info for setting userdata
                string userName = GetHttpParam("name", string.Empty);
                string email = GetHttpParam("email", string.Empty);
                string company = GetHttpParam("company", string.Empty);

                // Set user data
                gen.UserData = userName + "#" + company + "#" + email;

                // To populate the License Management database with user data 
                // fields when generating licenses using the API, 
                // see http://www.ssware.com/support/viewtopic.php?t=750
                // In this case, make sure to remove the "gen.UserData = ..." line above

                // Generate license
                string license = gen.Generate();

                // Return generated license code directly inline
                Response.ContentType = "text/plain";
                Response.Write(license);
                Response.End();
            }
            catch (Exception ex)
            {
                if (log_exceptions && !(ex is System.Threading.ThreadAbortException))
                    LicenseService.LogException(ex);
            }

        }
    }

}