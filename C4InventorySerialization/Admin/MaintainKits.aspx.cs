using System;
using System.Data;
using System.Configuration;
using System.Web.UI.WebControls;
namespace C4InventorySerialization.Admin
{
    public partial class MaintainKits : System.Web.UI.Page
    {
        private readonly string _serverLocation = ConfigurationManager.AppSettings["ServerLocation"].ToString();

        void Page_Load(object sender, EventArgs e)
        {
            LoadSql();
            
        }

        private void LoadSql()
        {

            SqlDataSource2.SelectParameters.Add("SERVERLOCATION", DbType.String, _serverLocation);
        }
    }
}
