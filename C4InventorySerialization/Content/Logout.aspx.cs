using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace C4InventorySerialization.Content
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Abandon();
            HttpCookie adAuthCookie = Response.Cookies["adAuthCookie"];
            adAuthCookie.Expires = DateTime.Now.AddDays(-1D);
            Response.Cookies.Add(adAuthCookie);
            //Response.Cookies.Clear();
            //FormsAuthentication.SignOut();
            FormsAuthentication.SignOut();
        }
    }
}
