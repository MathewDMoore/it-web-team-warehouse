using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace C4InventorySerialization.Admin
{
    public partial class ReturnInventoryRequest : System.Web.UI.Page
    {
/*
        int CountDOCNUM = 0;
*/
        int _docnum = 0;
        string _username;

        private void ProcessReturn()
        {
            _username = User.Identity.Name;
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
                SqlCommand sCmd = new SqlCommand("sp_ReturnGoodsIssue_IR", sConn);
                sCmd.CommandType = CommandType.StoredProcedure;
                sCmd.Parameters.Add("@DOCNUM", SqlDbType.Int);
                sCmd.Parameters["@DOCNUM"].Value = _docnum;
                sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                sCmd.Parameters["@USERNAME"].Value = _username;
                using (IDataReader reader1 = sCmd.ExecuteReader())
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