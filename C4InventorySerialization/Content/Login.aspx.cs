﻿using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Security.Principal;
using FormsAuth;

namespace C4InventorySerialization
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string username = User.Identity.Name;
            if (username != "")
            {
                loggedUser.Text = "Currently logged in as: " + username +
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


            LdapAuthentication adAuth = new LdapAuthentication(adPath);
            try
            {
                if (true == adAuth.IsAuthenticated(UserName.Text, Password.Text))
                {
                    String groups = adAuth.GetGroups();

                    //    Create the ticket, and add the groups.
                    bool isCookiePersistent = false;
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, UserName.Text,
                    DateTime.Now, DateTime.Now.AddMinutes(120), isCookiePersistent, groups);

                    //      Encrypt the ticket.
                    String encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                    //      Create a cookie, and then add the encrypted ticket to the cookie as data.
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                    if (true == isCookiePersistent)
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
                Output.Text = "Error authenticating. " + ex.Message;
            }
        }
    }
}
