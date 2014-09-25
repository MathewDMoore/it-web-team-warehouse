using System;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using ApplicationSource;
using Common;
using StructureMap;
using IoCRegistry = C4InventorySerialization.Helpers.IoCRegistry;

namespace C4InventorySerialization
{
    public class Global : HttpApplication
    {
        private ILogger _log;
        protected void Application_Start(object sender, EventArgs e)
        {
            ObjectFactory.Initialize(x => x.AddRegistry<IoCRegistry>());     
            DomainModelMapper.Initialize();
            _log = ObjectFactory.GetInstance<ILogger>();
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
                _log.LogException(MethodBase.GetCurrentMethod().GetType(), OperationType.LOGIN,ex, ex.Message);
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
            _log.LogException(MethodBase.GetCurrentMethod().GetType(), OperationType.GLOBAL_APP_ERROR, ex, ex.Message);

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