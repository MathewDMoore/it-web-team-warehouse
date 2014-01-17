using System;
using System.Configuration;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using System.Security.Principal;
using System.Web.Services;

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
    }
}
