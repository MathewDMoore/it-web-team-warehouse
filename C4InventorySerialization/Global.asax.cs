using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml.Linq;

namespace C4InventorySerialization
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }
         
        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            var cookieName = FormsAuthentication.FormsCookieName;
            var authCookie = Context.Request.Cookies[cookieName];

            if (null == authCookie)
            {//There is no authentication cookie.
                return;
            }

            FormsAuthenticationTicket authTicket = null;

            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch (Exception ex)
            {
                //Write the exception to the Event Log.
                return;
            }

            if (null == authTicket)
            {//Cookie failed to decrypt.
                return;
            }

            //When the ticket was created, the UserData property was assigned a
            //pipe-delimited string of group names.
            var groups = authTicket.UserData.Split(new char[] { '|' });

            //Create an Identity.
            var id = new GenericIdentity(authTicket.Name, "LdapAuthentication");

            //This principal flows throughout the request.
            var principal = new GenericPrincipal(id, groups);

            Context.User = principal;

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = HttpContext.Current.Server.GetLastError();

            // Do whatever with the exception here.

        }

        protected void Session_End(object sender, EventArgs e)
        {
            Context.Request.Cookies.Clear();
            Context.Response.Cookies.Clear();
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}