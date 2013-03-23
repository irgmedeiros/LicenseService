using System;
using System.Web;
using System.Globalization;
using System.IO;

using LogicNP.CryptoLicensing;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace LicService
{

    public partial class ClickBankHandler : System.Web.UI.Page
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

    public static bool IsIPValid(HttpRequest request)
    {
        string secretKey = "TEST";
        List<string> ipnFields = new List<string>();
        foreach (string param in request.Form.Keys)
        {
            if (param.Equals("cverify"))
            {
                continue;
            }
            ipnFields.Add(param);
        }
        ipnFields.Sort();
        string pop = "";
        foreach (String field in ipnFields)
        {
            pop += request.Form.Get(field) + "|";
        }
        pop += secretKey;
        string cverify = request.Form.Get("cverify");
        byte[] hashedData = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(pop));
        string calced_verification = BitConverter.ToString(hashedData).Replace("-", "").ToUpper().Substring(0, 8);
        return calced_verification.Equals(cverify);
    }

    // START SETTINGS: Change as desired

    // General settings
    string profileName = string.Empty; // Select predefined profile, if any, to be used when generating license
    string product_name = "My Product"; // The name of your product

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

    // END SETTINGS

    // Used to generate licenses
    CryptoLicenseGenerator gen = null; 


    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //Uncomment if you want to check for security purposes whether request is valid
//             if (!IsIPValid(this.Request))
//                 throw new Exception("Invalid IP");

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
            //gen.NumberOfUsers = (short)GetHttpParam("QUANTITY", 1);

            // OPTIONAL: Use purchase data if desired
            // ShareIt notifies the purchase date in format specified in following line
            //DateTime datePurchased = GetHttpParam("PURCHASE_DATE", "dd/MM/yyyy",DateTime.Now);

            // Extract info for setting userdata
            string userName = GetHttpParam("ccustfullname", string.Empty);
            string email = GetHttpParam("ccustemail", string.Empty);

            // Set user data
            gen.UserData = userName + "#" + email;

            // To populate the License Management database with user data 
            // fields when generating licenses using the API, 
            // see http://www.ssware.com/support/viewtopic.php?t=750
            // In this case, make sure to remove the "gen.UserData = ..." line above

            // Generate license
            string license = gen.Generate();

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
        try
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
        catch { }

    }



}

}
