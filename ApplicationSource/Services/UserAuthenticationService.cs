using System;
using System.Configuration;
using System.Linq;
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
            var adPath = ConfigurationManager.AppSettings["LDAPServer"];
            const string errorMessage = "User was unable to be authenticated. Please double check username and password. If problem persists, contact server administrator";

            var adAuth = new LdapAuthentication(adPath);

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

                    //Create the ticket, and add the groups.
	                var authTicket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.AddMinutes(120), false, groups);

                    //Encrypt the ticket.
                    var encryptedTicket = FormsAuthentication.Encrypt(authTicket);

	                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
	                {
		                Path = "/ship/Content"
	                };

	                HttpContext.Current.Response.Cookies.Add(authCookie);

					return new UserAuthenticationModel { IsAuthenticated = true, EncryptedTicket = encryptedTicket, CookieName = FormsAuthentication.FormsCookieName };
                }

                return new UserAuthenticationModel { IsAuthenticated = false, ErrorMessage = errorMessage };
            }
            catch (Exception)
            {
                return new UserAuthenticationModel { IsAuthenticated = false, ErrorMessage = errorMessage };
            }
        }
    }
}