using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using LogicNP.CryptoLicensing;
using System.IO;
using System.Xml;
using System.Data.Common;
using System.Data.OleDb;
using System.Data;

namespace LicService
{
    [WebService(Namespace = "http://www.ssware.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class LicenseServiceClass : LogicNP.CryptoLicensing.LicenseService
    {
        public LicenseServiceClass()
        {
            /*
            
            *******IMPORTANT***********
            
            Set the 'CryptoLicensing License Service' license code using the SetLicenseCode method below.
            
            Your license code can be found in the license information email we sent you when you purchased CryptoLicensing.
            
            There are two license codes, make sure you use the correct one. See http://www.ssware.com/support/viewtopic.php?t=734 for more information
           
            WARNING: If no or incorrect license code is specified, it reverts to trial mode. Codes generated in trial mode ALWAYS EXPIRE AFTER 30 DAYS.
            
            TIP: To ensure that you do not accidentally generate such codes in production systems, set CryptoLicenseGenerator.FailInTrialMode property to true.
            
            */

            this.Generator.SetLicenseCode(""); // LEAVE BLANK when using trial version of CryptoLicensing.


        }

    }

}