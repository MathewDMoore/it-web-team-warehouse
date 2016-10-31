using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Security;
using ApplicationSource;
namespace C4InventorySerialization.Content
{
	public partial class Login: System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			var user = HttpContext.Current.Session["User"] as User;
			var userName = user == null ? "" : user.UserName;

			if(user != null && !user.Groups.Contains("Admin"))
				loggedUser.Text = $"Currently logged in as: {UserName}. You need Administrator permissions to process Returns and Maintain Kits/Products.";
			else if(string.IsNullOrEmpty(userName))
				loggedUser.Text = "Welcome to Serialization Application. You must first login prior to processing deliveries and inventory requests.";
		}
	}
}