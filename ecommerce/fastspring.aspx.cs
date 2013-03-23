using System;
using System.Web;
using System.Globalization;
using System.IO;

using LogicNP.CryptoLicensing;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace LicService
{
    public partial class FastSpringHandler : System.Web.UI.Page
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

        string fastspring_private_key = ""; // Specify your FastSpring private key (found in "Remote License Config"-->"Security" tab)

        // END SETTINGS


        // Checks whether the HTTP POST from FastSpring is valid
        bool CheckRequest()
        {
            string paramValues = "";
            string security_request_hash = null;

            // Sort keys
            List<string> sortedKeys = new List<string>();
            foreach (string key in this.Request.Form.Keys)
                sortedKeys.Add(key);

            sortedKeys.Sort();

            // Calculate MD5 hash of values
            foreach (string key in sortedKeys)
            {
                if (key == "security_request_hash")
                    security_request_hash = Request.Form[key];
                else
                    paramValues += Request.Form[key];
            }

            paramValues += fastspring_private_key;

            string hash = CalculateMD5Hash(paramValues);

            return string.Compare(hash,security_request_hash,true)==0;

        }

        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //// OPTIONAL: For security, validate the HTTP POST sent by FastSpring, uncomment following 2 lines if required.
                //if (!CheckRequest())
                //    throw new Exception("HTTP POST notification from FastSpring is invalid!");

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