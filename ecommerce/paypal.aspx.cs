using System;
using System.Web;
using System.Globalization;
using System.IO;

using LogicNP.CryptoLicensing;
using System.Collections.Specialized;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace LicService
{

    public partial class PayPalHandler : System.Web.UI.Page
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

        Decimal GetHttpParam(string paramName, Decimal defaultValue)
        {
            string paramValue = Request.Params[paramName];
            if (paramValue != null)
            {
                try
                {
                    return Decimal.Parse(paramValue);
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
        string product_name = "My Product"; // The name of your product
        string profileName = string.Empty; // Select predefined profile, if any, to be used when generating license

        // Paypal specific settings
        string receiver_email = string.Empty; // your paypal account email
        bool useSandBox = true; // if true, 'sandbox paypal' (for testing) is used instead of the live Paypal **set to false when going live**
        string item_number = string.Empty; // SKU/item code of your product
        Decimal item_cost = new decimal(100); // cost per unit of your product

        // Send email settings
        bool send_email = true; // If true, an email is sent to the customer

        // Template file used for the body of the email
        // The template contains various placeholders as defined below 
        // %AppDomainAppPath% is the root of your application and is replaced automatically
        string email_template = LicenseService.MakePortablePath(@"%AppDomainAppPath%App_Data\email_templates\license.htm");
        string[] placeholders = new string[] { "%%product_name%%", "%%license%%" }; // placeholders used in email template
        string[] replacements; // initialized in InitPlaceHolders method below

        string email_from_address = string.Empty; // Email From address 
        string email_subject = "License information"; // Email subject
        string email_smtp_server = string.Empty; // Server used for sending email
        int email_smtp_port = 25; // Port used for sending email
        bool email_use_ssl = false; // If true, SSL is used for sending email
        string email_smtp_username = string.Empty; // Username for mail server (leave blank if not required)
        string email_smtp_password = string.Empty; // password for mail server (leave blank if not required)

        // If set to true, any exceptions that occur will be logged in time-stamped .log files in the App_Data folder.
        bool log_exceptions = true;


        // END VARIABLES


        // Used to generate licenses
        CryptoLicenseGenerator gen = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Currently only "web_accept" (used with 'Donate','Buy Now' buttons) is supported, 
                // but can be easily extended to support others such as 'cart'
                string ipn_txn_type = GetHttpParam("txn_type", string.Empty);
                if (ipn_txn_type != "web_accept")
                    throw new Exception("ipn_txn_type is not 'web_accept'");

                // Product bought does not match ours
                string ipn_item_number = GetHttpParam("item_number", string.Empty);
                if (ipn_item_number != item_number)
                    throw new Exception("ipn_item_number not equal to defined item number");

                // Verify that costs match
                int ipn_quantity = GetHttpParam("quantity", 1);
                Decimal ipn_mc_gross = GetHttpParam("mc_gross", Decimal.Zero);
                Decimal ipn_tax = GetHttpParam("tax", Decimal.Zero);
                if (ipn_quantity * item_cost + ipn_tax != ipn_mc_gross)
                    throw new Exception("Cost does not match");

                // Verify IPN notification
                VerifyNotification();

                // Load license project file
                gen = CryptoLicenseGenerator.FromSettingsFile(Path.Combine(HttpRuntime.AppDomainAppPath, LicenseService.MakePortablePath(@"App_Data\settings.xml")));

                // ***IMPORTANT***: Set your 'CryptoLicensing License Service' license code below.
                // It can be found in the email containing the license information that you received from LogicNP when you purchased the product.
                // LEAVE BLANK when using evaluation version of CryptoLicensing.
                gen.SetLicenseCode("");

                // If profile is specified above, select it so that generated license 
                // has same settings as specified by profile
                if (profileName.Length > 0)
                    gen.SetActiveProfile(profileName);

                // OPTIONAL: Set 'Numer of Users' setting of license to the quantity purchased
                // gen.NumberOfUsers = (short)ipn_quantity;

                // Extract info for setting userdata
                string userName = GetHttpParam("first_name", string.Empty) + " " + GetHttpParam("last_name", string.Empty);
                string email = GetHttpParam("payer_email", string.Empty);
                string company = GetHttpParam("payer_business_name", string.Empty);

                // Set user data
                gen.UserData = userName + "#" + company + "#" + email;

                // To populate the License Management database with user data 
                // fields when generating licenses using the API, 
                // see http://www.ssware.com/support/viewtopic.php?t=750
                // In this case, make sure to remove the "gen.UserData = ..." line above

                // Generate license (will be used in email template below)
                gen.Generate();

                // Send email if desired
                if (send_email)
                    SendEmail(email);
            }
            catch (Exception ex)
            {
                if (log_exceptions && !(ex is System.Threading.ThreadAbortException))
                    LicenseService.LogException(ex);
            }

        }

        // helper methods

        // URL encode
        string Encode(string oldValue)
        {
            string newValue = oldValue.Replace("\"", "'");
            newValue = System.Web.HttpUtility.UrlEncode(newValue);
            newValue = newValue.Replace("%2f", "/");
            return newValue;
        }

        // Verifies notification with Paypal
        void VerifyNotification()
        {
            string ipn_receiver_email = GetHttpParam("receiver_email", string.Empty);

            // Does not match our paypal login id
            if (ipn_receiver_email != this.receiver_email)
                throw new Exception();

            string s = "cmd = _notify-validate";
            foreach (string paramName in base.Request.Form)
            {
                string paramValue = this.Encode(base.Request.Form[paramName]);
                s = s + string.Format("&{0}={1}", paramName, paramValue);
            }
            string address = "https://www.paypal.com/cgi-bin/webscr";
            if (this.useSandBox)
            {
                address = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            }

            WebClient client = new WebClient();
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            byte[] bytes = Encoding.UTF8.GetBytes(s);

                // Send HTTP POST to verify IPN
                byte[] buffer2 = client.UploadData(address, "POST", bytes);
                string message = Encoding.UTF8.GetString(buffer2);

            if (message != "VERIFIED") // IPN verification unsuccessful
                throw new Exception("IPN verification failed.");
        }

        string ReadFile(string filePath)
        {
            filePath = LicenseService.GetRealFilePath(filePath);
            StreamReader reader = new StreamReader(filePath);
            string ret = reader.ReadToEnd();
            reader.Close();
            return ret;
        }

        void InitPlaceHolders()
        {
            replacements = new string[placeholders.Length];

            replacements[0] = product_name;
            replacements[1] = gen.LicenseCodes[0];
        }

        string ReplacePlaceholders(string text)
        {
            InitPlaceHolders();
            for (int i = 0; i < placeholders.Length; i++)
            {
                text = text.Replace(placeholders[i], replacements[i]);
            }
            return text;
        }

        string GetEmailBody()
        {
            string body = ReadFile(email_template);
            return ReplacePlaceholders(body);
        }

        private void SendEmail(string email_to_address)
        {
            // Construct message
            MailMessage message = new MailMessage(email_from_address, email_to_address);
            message.Subject = email_subject;
            message.IsBodyHtml = true;
            message.Body = GetEmailBody();

            SmtpClient client = new SmtpClient(email_smtp_server, email_smtp_port);
            client.EnableSsl = email_use_ssl;
            if (string.IsNullOrEmpty(email_smtp_username))
            {
                client.Credentials = new NetworkCredential();
            }
            else
            {
                client.Credentials = new NetworkCredential(email_smtp_username, email_smtp_password);
            }
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Send(message);
        }

    }
}