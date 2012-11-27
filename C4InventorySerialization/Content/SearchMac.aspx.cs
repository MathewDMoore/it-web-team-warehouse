using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace C4InventorySerialization.Content
{
    public partial class SearchMac : System.Web.UI.Page
    {
        private int _docnum;

        protected void Page_Load(object sender, EventArgs e)
        {
            CreateGrid2();
        }

        private void CreateGrid2()
        {
            if (!string.IsNullOrEmpty(Context.Request.QueryString["DeliveryNum"]))
            {
                _docnum = Convert.ToInt32(Context.Request.QueryString["DeliveryNum"]);

                if (_docnum == 0)
                {
                    macInputError.Visible = true;
                    Grid2.ClearPreviousDataSource();
                }

                else
                {
                    string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
                    using (SqlConnection sConn = new SqlConnection(connStr))
                    {
                        sConn.Open();
                        SqlCommand sCmd = new SqlCommand("sp_delivery_header", sConn);
                        sCmd.CommandType = CommandType.StoredProcedure;
                        sCmd.Parameters.Add("@DOCNUM", SqlDbType.Int);
                        sCmd.Parameters.Add("@SERVERLOCATION", SqlDbType.NVarChar);
                        sCmd.Parameters["@DOCNUM"].Value = _docnum;
                        sCmd.Parameters["@SERVERLOCATION"].Value = ConfigurationManager.AppSettings["ServerLocation"];

                        using (IDataReader reader1 = sCmd.ExecuteReader())
                        {
                            Grid2.DataSource = reader1;
                            Grid2.DataBind();
                        }
                        sConn.Close();
                    }
                }
            }
        }

        protected void RebindGrid(object sender, EventArgs e)
        {
            CreateGrid2();
        }

    }
}