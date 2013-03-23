using System;
using System.Web;
using System.Globalization;
using System.IO;

using LogicNP.CryptoLicensing;

namespace LicService
{
    public partial class ShareItHandler : System.Web.UI.Page
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

        public double IPToNumber(string ip)
        {
            string[] octets;
            double num = 0;
            octets = ip.Split('.');
            for (int i = octets.Length - 1; i >= 0; i--)
            {
                num += ((int.Parse(octets[i]) % 256) * Math.Pow(256, (3 - i)));
            }
            return num;
        }


        bool IsIPValid(string remoteIP)
        {
            // ShareIt uses these IPs when communicating with the license service. You are advised to get an updated list of IPs from ShareIt
            // Reference: ShareIt Key Generator SDK
            double range1_LB = IPToNumber("217.65.128.0");
            double range1_UB = IPToNumber("217.65.143.255");
            double range2_LB = IPToNumber("85.255.16.0");
            double range2_UB = IPToNumber("85.255.31.255");

            double ip = IPToNumber(remoteIP);

            if (ip >= range1_LB && ip <= range1_UB)
                return true;
            if (ip >= range2_LB && ip <= range2_UB)
                return true;

            return false;
        }

        enum ReturnType
        {
            InlineLicenseString,
            AsLicenseFile
        }

        // START SETTINGS: Change as desired

        // General settings
        string profileName = string.Empty; // Select predefined profile, if any, to be used when generating license

        // ShareIt specific settings
        ReturnType retType = ReturnType.InlineLicenseString;
        string licFileName = "license.txt"; // Only used if retType = AsLicenseFile

        // If set to true, any exceptions that occur will be logged in time-stamped .log files in the App_Data folder.
        bool log_exceptions = true;


        // END SETTINGS

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Uncomment if you want to check for security purposes whether remote IP address is a ShareIt IP address
                //if(!IsIPValid(GetRemoteIPAddress()))
                //    throw new Exception("Remote IP address is not a ShareIt IP address");

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

                // OPTIONAL: Use purchase data if desired
                // ShareIt notifies the purchase date in format specified in following line
                //DateTime datePurchased = GetHttpParam("PURCHASE_DATE", "dd/MM/yyyy",DateTime.Now);

                // Extract info for setting userdata
                string userName = GetHttpParam("REG_NAME", string.Empty);
                string email = GetHttpParam("EMAIL", string.Empty);
                string company = GetHttpParam("COMPANY", string.Empty);

                // Set user data
                gen.UserData = userName + "#" + company + "#" + email;

                // To populate the License Management database with user data 
                // fields when generating licenses using the API, 
                // see http://www.ssware.com/support/viewtopic.php?t=750
                // In this case, make sure to remove the "gen.UserData = ..." line above

                // Generate license
                string license = gen.Generate();

                if (retType == ReturnType.InlineLicenseString)
                {
                    // METHOD 1 : Return generated license code directly inline

                    Response.ContentType = "text/plain";
                    Response.Write(license);
                    Response.End();
                }
                else
                {
                    // METHOD 2 : Return license in a file

                    Response.ContentType = "text/file";
                    Response.AppendHeader("Content-Disposition", "filename=" + licFileName);
                    Response.Write(license);
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                if (log_exceptions && !(ex is System.Threading.ThreadAbortException))
                    LicenseService.LogException(ex);
            }

        }
    }

}