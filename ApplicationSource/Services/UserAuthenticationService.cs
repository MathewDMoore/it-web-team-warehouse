using System;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Security;
using ApplicationSource.Interfaces;
using ApplicationSource.Models;

namespace ApplicationSource.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class UserAuthenticationService : IUserAuthenticationService
    {
        public UserAuthenticationModel UserAuthenticationLogin(string userName, string password, string contractorName)
        {
            String adPath = ConfigurationManager.AppSettings["LDAPServer"];
            const string ERROR_MESSAGE = "User was unable to be authenticated. Please double check username and password. If problem persists, contact server administrator";

            LdapAuthentication adAuth = new LdapAuthentication(adPath);
            try
            {
                var isAuthd = adAuth.IsAuthenticated(userName, password);
                if (isAuthd)
                {
                    var scanName = contractorName == null ? userName : contractorName + "-contractor";
                    var user = new User(scanName, scanName, true)
                        {
                            Groups = adAuth.GetGroups().Split('|').ToList(),
                            UserName = userName
                        };
                    HttpContext.Current.Session.Add("User", user);
                    HttpContext.Current.Session.Timeout = 30;

                    var groups = adAuth.GetGroups();

                    //    Create the ticket, and add the groups.
                    var isCookiePersistent = false;
                    var authTicket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.AddMinutes(120), isCookiePersistent, groups);

                    //      Encrypt the ticket.
                    var encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                    return new UserAuthenticationModel { IsAuthenticated = true, EncryptedTicket = encryptedTicket, CookieName = FormsAuthentication.FormsCookieName };
                }

                return new UserAuthenticationModel() { IsAuthenticated = false, ErrorMessage = ERROR_MESSAGE };
            }
            catch (Exception ex)
            {
                return new UserAuthenticationModel() { IsAuthenticated = false, ErrorMessage = ERROR_MESSAGE };
            }
        }
    }
}

