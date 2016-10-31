using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using ApplicationSource;

namespace C4InventorySerialization.Content
{
    public partial class Returns : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = (User)HttpContext.Current.Session["User"];
            
        }

        [WebMethod]
        public void ReturnProducts(List<string> returnItems )
        {
                

        }
    }
}