using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace C4InventorySerialization.Admin
{
    public partial class ReturnDelivery : System.Web.UI.Page
    {
/*
        int CountDOCNUM = 0;
*/
        int _docnum;
        public string Username;

        private void ProcessReturn()
        {
            Username = User.Identity.Name;

            if (Context.Request.QueryString["DeliveryNum"] != null)
            {
                _docnum = Convert.ToInt32(Context.Request.QueryString["DeliveryNum"]);
            }
            else
            {
                _docnum = 0;
            }
                
            string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
            using (SqlConnection sConn = new SqlConnection(connStr))
            {
                sConn.Open();
                SqlCommand sCmd = new SqlCommand("sp_ReturnDelivery", sConn);
                sCmd.CommandType = CommandType.StoredProcedure;
                sCmd.Parameters.Add("@DOCNUM", SqlDbType.Int);
                sCmd.Parameters["@DOCNUM"].Value = _docnum;
                sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                sCmd.Parameters["@USERNAME"].Value = Username;

                using (sCmd.ExecuteReader())
                {
                        
                }
            }
          
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                ProcessReturn();
            }

        }
    }
}