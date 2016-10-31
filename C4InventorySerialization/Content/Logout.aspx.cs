using System;
using System.Web.Security;

namespace C4InventorySerialization.Content
{
	public partial class Logout: System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			FormsAuthentication.SignOut();
			Session.Abandon();

			var adAuthCookie = Response.Cookies[FormsAuthentication.FormsCookieName];

			if(adAuthCookie != null)
			{
				adAuthCookie.Path = "/ship/Content";
				adAuthCookie.Expires = DateTime.Now.AddDays(-1);
				Response.Cookies.Add(adAuthCookie);
			}
		}
	}
}