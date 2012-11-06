using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;

namespace C4InventorySerialization.Content
{
    public partial class MaintainKits : System.Web.UI.Page
    {
        private string _serverLocation = ConfigurationManager.AppSettings["ServerLocation"];

        void Page_Load(object sender, EventArgs e)
        {
            LoadSQL();
            
        }

        private void LoadSQL()
        {

            SqlDataSource2.SelectParameters.Add(new Parameter(_serverLocation, DbType.String));
        }
    }
}
