using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace C4InventorySerialization.Admin
{
    public partial class ReturnDeliveryByLineItem : System.Web.UI.Page
    {
        /*
                int CountDOCNUM = 0;
        */
        string _lineNum;

        private void ProcessReturn()
        {
            _lineNum = Request.QueryString["LineNum"];

            if (!string.IsNullOrEmpty(_lineNum))
            {
                string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
                var qString = _lineNum.Split(' ');

                for (var i = 0; i < qString.Length; i++)
                {
                    using (SqlConnection sConn = new SqlConnection(connStr))
                    {
                        sConn.Open();
                        SqlCommand sCmd = new SqlCommand("sp_ReturnDeliveryByLineItem", sConn);
                        sCmd.CommandType = CommandType.StoredProcedure;
                        sCmd.Parameters.Add("@ID", SqlDbType.Int);
                        sCmd.Parameters["@ID"].Value = qString[i];
                        sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                        sCmd.Parameters["@USERNAME"].Value = HttpContext.Current.User.Identity.Name;
                        using (sCmd.ExecuteReader())
                        {

                        }
                    }

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

