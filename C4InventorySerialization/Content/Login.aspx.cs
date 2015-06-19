using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Security;
using ApplicationSource;
using Common;
using StructureMap;

namespace C4InventorySerialization
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            User user = HttpContext.Current.Session["User"] as User;
            var userName = user == null ? "" : user.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                
                loggedUser.Text = "Currently logged in as: " + userName +
                                  ". You need Administrator permissions to process Returns and Maintain Kits/Products.";
            }
            else
            {
                loggedUser.Text = "Welcome to Serialization Application." +
                                  "You must first login prior to processing deliveries and inventory requests.";
            }
        }
        protected void Login_Click(Object sender, EventArgs e)
        {
            String adPath = ConfigurationManager.AppSettings["LDAPServer"];


            var adAuth = new LdapAuthentication(adPath);
            try
            {
                if (adAuth.IsAuthenticated(UserName.Text, Password.Text))
                {
                    var log = ObjectFactory.GetInstance<ILogger>();

                    log.LogAttempt(MethodBase.GetCurrentMethod().GetType(), OperationType.LOGIN, "LOGIN ATTEMPT", UserName.Text);

                    String groups = adAuth.GetGroups();

                    //    Create the ticket, and add the groups.
                    bool isCookiePersistent = false;
                    var authTicket = new FormsAuthenticationTicket(1, UserName.Text,
                    DateTime.Now, DateTime.Now.AddMinutes(120), isCookiePersistent, groups);

                    //      Encrypt the ticket.
                    String encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                    //      Create a cookie, and then add the encrypted ticket to the cookie as data.
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                    if (isCookiePersistent)
                        authCookie.Expires = authTicket.Expiration;

                    //      Add the cookie to the outgoing cookies collection.
                    Response.Cookies.Add(authCookie);

                    //      You can redirect now.
                    Response.Redirect(FormsAuthentication.GetRedirectUrl(UserName.Text, false));
                }
                else
                {
                    Output.Text = "Authentication did not succeed. Either your user information is incorrect or you don't have permissions.";
                }
            }
            catch (Exception ex)
            {
                var log =ObjectFactory.GetInstance<ILogger>();
                log.LogException(MethodBase.GetCurrentMethod().GetType(), OperationType.LOGIN, ex, ex.Message);

                Output.Text = "Error authenticating. " + ex.Message;
            }
        }
    }
}
