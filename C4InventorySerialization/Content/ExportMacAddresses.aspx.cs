using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace C4InventorySerialization.Content
{
    public partial class ExportMacAddresses : System.Web.UI.Page
    {
        private int _docnum = 0;
        public int VerifiedDelivery;
        public string Username;

        protected void Page_Load(object sender, EventArgs e)
        {
            Username = User.Identity.Name;
            if (!Page.IsPostBack)
            {
                var DeliveryNum = Context.Request.QueryString["DeliveryNum"];

                if (DeliveryNum != null)
                    DeliveryNum = Context.Request.QueryString["DeliveryNum"];
                else
                    DeliveryNum = "0";
                
                CreateGrid();
            }
        }

        protected void RebindGrid(object sender, EventArgs e)
        {
            CreateGrid();
        }

        protected void CreateGrid()
        {
            string username = User.Identity.Name;
            var DeliveryNum = Context.Request.QueryString["DeliveryNum"];

            if (DeliveryNum != null)
                DeliveryNum = Context.Request.QueryString["DeliveryNum"];
            else
                DeliveryNum = "0";

            if (DeliveryNum != "0")
            {
                string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
                using (SqlConnection sConn = new SqlConnection(connStr))
                {
                    sConn.Open();
                    SqlCommand sCmd = new SqlCommand("sp_ExportMacAddresses", sConn);
                    sCmd.CommandType = CommandType.StoredProcedure;
                    sCmd.Parameters.Add("@DOCNUM", SqlDbType.Int);
                    sCmd.Parameters["@DOCNUM"].Value = DeliveryNum;

                    using (IDataReader reader1 = sCmd.ExecuteReader())
                    {
                        EMAGrid.DataSource = reader1;
                        EMAGrid.DataBind();
                    }
                    sConn.Close();
                }
            }
        }
    }
}
