using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Obout.ComboBox;
using Obout.Grid;

namespace C4InventorySerialization.Admin
{
    public partial class MaintainKits : System.Web.UI.Page
    {
        private readonly string _serverLocation = ConfigurationManager.AppSettings["ServerLocation"];
        private readonly string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;

        void Page_Load(object sender, EventArgs e)
        {
            LoadSql();

        }

        private void LoadSql()
        {

           // SqlDataSource2.SelectParameters.Add("SERVERLOCATION", DbType.String, _serverLocation);
        }

        protected void UpdateRecord(object sender, GridRecordEventArgs e)
        {
            var alternateText = e.Record["ALTERNATETEXT"].ToString();
            var kitCode = e.Record["KITCODE"].ToString();
            var itemCode = e.Record["ITEMCODE"].ToString();
            var qty = int.Parse(e.Record["QUANTITY"].ToString());
            int kitItemid = string.IsNullOrEmpty(e.Record["KITITEMID"].ToString()) ? 0 : int.Parse(e.Record["KITITEMID"].ToString());

            using (var sConn = new SqlConnection(connStr))
            {
                sConn.Open();

                var sCmd = new SqlCommand("sp_KitUpdate", sConn);
                sCmd.CommandType = CommandType.StoredProcedure;
                sCmd.Parameters.Add("@KITCODE", SqlDbType.NVarChar);
                sCmd.Parameters.Add("@ALTERNATETEXT", SqlDbType.NVarChar);
                sCmd.Parameters.Add("@ITEMCODE", SqlDbType.NVarChar);
                sCmd.Parameters.Add("@QTY", SqlDbType.Int);
                sCmd.Parameters.Add("@KITITEMID", SqlDbType.Int);

                sCmd.Parameters["@KITCODE"].Value = kitCode.ToUpper();
                sCmd.Parameters["@ALTERNATETEXT"].Value = alternateText;
                sCmd.Parameters["@ITEMCODE"].Value = itemCode;
                sCmd.Parameters["@QTY"].Value = qty;
                sCmd.Parameters["@KITITEMID"].Value = kitItemid;

                var res = sCmd.ExecuteNonQuery();
                sConn.Close();
            }
        }

        protected void DeleteRecord(object sender, GridRecordEventArgs e)
        {
            var kitItemId = e.Record["KITITEMID"].ToString();

            using (var sConn = new SqlConnection(connStr))
            {
                sConn.Open();

                var sCmd = new SqlCommand("sp_KitDeleteItem", sConn);
                sCmd.CommandType = CommandType.StoredProcedure;
                sCmd.Parameters.Add("@KITITEMID", SqlDbType.NVarChar);

                sCmd.Parameters["@KITITEMID"].Value = kitItemId;

                var res = sCmd.ExecuteNonQuery();
                sConn.Close();
            }
        }
    }
}
