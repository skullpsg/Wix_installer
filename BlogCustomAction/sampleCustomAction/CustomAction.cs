using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.DirectoryServices.AccountManagement;
using System.Windows.Forms;

namespace sampleCustomAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult CheckingUserAvailability(Session session)
        {
            //Here we are getting the value of textbox by using its property value... 
            string userName = session["USERACCOUNT"];
            string password = session["USERMY_PASSWORD"];
            
            if (userName.Contains("\\"))
                session["ACCOUNT"] = userName.Split('\\')[1];
            else
            session["ACCOUNT"] = userName;
            session["MY_PASSWORD"] = password;


            //if you want to check password confirmation you can get value by creating another one text box in user reg 
            //do some logic and assign the  value to ISPWDCOMPARESUCCESS based on that 
            //here I am considering the success scenario and assigning it to 1(string).
            session["ISPWDCOMPARESUCCESS"] = "1";

            //user validation
            int serviceCredentialStatus = ValidateCredential(session, userName, password);
            session["ISCREDENTIALSSUCCESS"] = Convert.ToString(serviceCredentialStatus);
            MessageBox.Show("Account:" + session["ACCOUNT"] + "password" + session["MY_PASSWORD"] + "validCredential:" + session["ISCREDENTIALSSUCCESS"]);
            
            return ActionResult.Success;
        }
        private static int ValidateCredential(Session session, string svcUserName, string svcPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(svcUserName) || string.IsNullOrEmpty(svcPassword))
                    return 0;

                if (svcUserName.ToUpperInvariant() != "LOCALSYSTEM")
                {
                    //UserName/Password check.
                    if (svcUserName.StartsWith(@".\")) //local account
                    {
                        string[] s = svcUserName.Split('\\');
                        svcUserName = s[1];

                        //Local Account
                        using (PrincipalContext pc = new PrincipalContext(ContextType.Machine, System.Environment.MachineName))
                        {
                            bool isValid = pc.ValidateCredentials(svcUserName, svcPassword);

                            if (isValid)
                                return 1;
                            else
                                return 0;
                        }
                    }

                }
                return 3;
            }
            catch (Exception ex)
            {
                return 3;
            }
        }
    }
}

