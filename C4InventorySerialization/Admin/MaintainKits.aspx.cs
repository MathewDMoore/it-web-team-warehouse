using System;
using System.Data;
using System.Configuration;
using System.Web.UI.WebControls;
using Obout.Grid;

namespace C4InventorySerialization.Admin
{
    public partial class MaintainKits : System.Web.UI.Page
    {
        private readonly string _serverLocation = ConfigurationManager.AppSettings["ServerLocation"];

        void Page_Load(object sender, EventArgs e)
        {
            LoadSql();
            var ddl1 = Grid1.Templates[1].Container.FindControl("ItemCodeEdit") as DropDownList;
            ddl1.DataSource = someDataSource;
            ddl1.DataBind();

        }

        private void LoadSql()
        {

            SqlDataSource2.SelectParameters.Add("SERVERLOCATION", DbType.String, _serverLocation);
        }
    }
}
