using System;
using System.Web;
using System.Globalization;
using System.IO;

using LogicNP.CryptoLicensing;

namespace LicService
{

    public partial class PlimusHandler : System.Web.UI.Page
    {

        string GetRemoteIPAddress()
        {
            HttpContext context = System.Web.HttpContext.Current;

            string ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
                return context.Request.ServerVariables["REMOTE_ADDR"];
            else
                return ip;

        }

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

        // Plimus uses these IPs when communicating with the license service. You are advised to get an updated list of IPs from Plimus
        // Reference: http://home.plimus.com/ecommerce/learning-center/ipn-code-sample
        string[] plimusIps = { "62.219.121.253", "209.128.93.248", "72.20.107.242", "209.128.93.229", "209.128.93.98", "209.128.93.230", "209.128.93.245", "209.128.93.104", "209.128.93.105", "209.128.93.107",
     "209.128.93.108", "209.128.93.242", "209.128.93.243", "209.128.93.254", "62.216.234.216", "62.216.234.218", "62.216.234.219", "62.216.234.220", "127.0.0.1", "localhost",
     "209.128.104.18", "209.128.104.19", "209.128.104.20", "209.128.104.21", "209.128.104.22", "209.128.104.23", "209.128.104.24", "209.128.104.25", "209.128.104.26", "209.128.104.27",
     "209.128.104.28", "209.128.104.29", "209.128.104.30", "209.128.104.31", "209.128.104.32", "209.128.104.33", "209.128.104.34", "209.128.104.35", "209.128.104.36", "209.128.104.37",
    "99.186.243.9", "99.186.243.10", "99.186.243.11", "99.186.243.12", "99.186.243.13", "99.180.227.233", "99.180.227.234", "99.180.227.235", "99.180.227.236", "99.180.227.237"
    };

        bool IsIPValid(string remoteIP)
        {
            foreach (string ip in plimusIps)
            {
                if (ip == remoteIP)
                    return true;
            }
            return false;
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
                // Uncomment if you want to check for security purposes whether remote IP address is a Plimus IP address
                //if(!IsIPValid(GetRemoteIPAddress()))
                //    throw new Exception("Remote IP Address Invalid");

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
                //gen.NumberOfUsers = (short)GetHttpParam("QUANTITY", 1);

                // Extract info for setting userdata
                string userName = GetHttpParam("CUSTOMER_NAME", string.Empty);
                string email = GetHttpParam("CUSTOMER_EMAIL", string.Empty);
                string company = GetHttpParam("COMPANY_NAME", string.Empty);

                // Set user data
                gen.UserData = userName + "#" + company + "#" + email;

                // To populate the License Management database with user data 
                // fields when generating licenses using the API, 
                // see http://www.ssware.com/support/viewtopic.php?t=750
                // In this case, make sure to remove the "gen.UserData = ..." line above

                // Generate license
                string license = gen.Generate();

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