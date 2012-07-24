using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Obout.Grid;

namespace C4InventorySerialization.Content
{
    public partial class IRHistory : System.Web.UI.Page
    {
        public string Username;

        protected void Page_Load(object sender, EventArgs e)
        {
            Username = User.Identity.Name;
            CreateGrid();
        }

        protected void RebindGrid(object sender, EventArgs e)
        {
            CreateGrid();
        }

        private void CreateGrid()
        {
            string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
            using (SqlConnection sConn = new SqlConnection(connStr))
            {
                sConn.Open();
                SqlCommand sCmd = new SqlCommand("sp_IR_History", sConn);
                sCmd.CommandType = CommandType.StoredProcedure;

                using (IDataReader reader1 = sCmd.ExecuteReader())
                {
                    grid1.DataSource = reader1;
                    grid1.DataBind();
                }
                sConn.Close();
            }
        }
    }
}