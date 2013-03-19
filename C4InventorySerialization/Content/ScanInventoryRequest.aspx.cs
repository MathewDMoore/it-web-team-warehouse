using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Obout.Grid;
using System.Security.Principal;
using System.Web.UI.WebControls;

namespace C4InventorySerialization.Content
{
    public partial class ScanInventoryRequest : System.Web.UI.Page
    {
        private int _countSerialcode;
        private int _countDocnum;
        private int _docnum;
        private int _verifiedCount;
        private int _verifiedRecords;
        public int VerifiedDelivery;
        public string Username;

        private string _serverLocation = ConfigurationManager.AppSettings["ServerLocation"];


        protected void Page_Load(object sender, EventArgs e)
        {
            Username = User.Identity.Name;
            if (!Page.IsPostBack)
            {
                if (Context.Request.QueryString["DeliveryNum"] != null)
                    _docnum = Convert.ToInt32(Context.Request.QueryString["DeliveryNum"]);
                else
                    _docnum = 0;

                ValidateDocnum(_docnum);
                CheckConfiguration.Text = _countDocnum.ToString();

                if (_verifiedCount > 0)
                {
                    verifiedimg.Visible = true;
                }
                else
                {
                    if (_docnum != 0)
                    {
                        notverifiedimg.Visible = true;
                    }
                }

                LoadSQL();
                CreateGrid();
                
            }
        }

        private void LoadSQL()
        {

            SqlDataSource3.SelectParameters.Add(new Parameter("DeliveryNum", DbType.Int32));
            SqlDataSource3.SelectParameters.Add(new Parameter(_serverLocation, DbType.String));
        }

        protected void RebindGrid(object sender, EventArgs e)
        {
            CreateGrid();
        }

        private void CreateGrid()
        {

            string username = User.Identity.Name;
            if (Context.Request.QueryString["DeliveryNum"] != null)
                _docnum = Convert.ToInt32(Context.Request.QueryString["DeliveryNum"]);
            else
                _docnum = 0;

            if (_docnum != 0)
            {
                string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
                using (SqlConnection sConn = new SqlConnection(connStr))
                {
                    sConn.Open();
                    SqlCommand sCmd = new SqlCommand("sp_GoodsIssue_IR_synch", sConn);
                    sCmd.CommandType = CommandType.StoredProcedure;
                    sCmd.Parameters.Add("@DOCNUM", SqlDbType.Int);                    
                    sCmd.Parameters["@DOCNUM"].Value = _docnum;
                    sCmd.Parameters.Add("@SERVERLOCATION", SqlDbType.NVarChar);
                    sCmd.Parameters["@SERVERLOCATION"].Value = _serverLocation;
                    sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                    sCmd.Parameters["@USERNAME"].Value = username;

                    try
                    {
                        using (IDataReader reader1 = sCmd.ExecuteReader())
                        {
                            grid1.DataSource = reader1;
                            grid1.DataBind();
                        }
                        sConn.Close();
                    }
                    catch (Exception ex)
                    { 
                    
                    }
                }
            }
        }


        protected void ValidateDocnum(int docnum)
        {
            string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
            string ValidDOCNUM =
                "SELECT COUNT(*) AS PRODUCTCOUNT FROM [C4_GOODSISSUE_IR_OUT] WHERE PRODUCTID IS NULL AND DOCNUM = @DOCNUM";
            string VerifiedDOCNUM = "SELECT COUNT(*) AS VERIFIEDCOUNT FROM [C4_GOODSISSUE_IR_OUT] WHERE VERIFIED=1 AND DOCNUM = @DOCNUM";
            using (SqlConnection sConn = new SqlConnection(connStr))
            {
                sConn.Open();

                SqlCommand sCmd = new SqlCommand(ValidDOCNUM, sConn);
                SqlCommand sCmd2 = new SqlCommand(VerifiedDOCNUM, sConn);
                sCmd.Parameters.Add("@DOCNUM", SqlDbType.Int);
                sCmd.Parameters["@DOCNUM"].Value = docnum;
                sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                sCmd.Parameters["@USERNAME"].Value = Username;
                sCmd2.Parameters.Add("@DOCNUM", SqlDbType.Int);
                sCmd2.Parameters["@DOCNUM"].Value = docnum;
                sCmd2.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                sCmd2.Parameters["@USERNAME"].Value = Username;
                using (IDataReader reader1 = sCmd.ExecuteReader())
                {
                    if (reader1.Read())
                    {
                        _countDocnum = reader1.GetInt32(0);
                    }
                }
                using (IDataReader reader1 = sCmd2.ExecuteReader())
                {
                    if (reader1.Read())
                    {
                        _verifiedCount = reader1.GetInt32(0);
                    }
                }
                sConn.Close();
            }
        }

        protected void ValidateRecord(String macId, String itemcode, int ID)
        {
            string username = User.Identity.Name;
            
            string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
            string ValidMacID =
                "select count(*) from C4_GOODSISSUE_IR_OUT T1, C4_MAINTAINPRODUCTID T2 where T1.ITEMCODE =T2.ITEMCODE AND T1.MACID = @MACID and T1.ITEMCODE = @ITEMCODE and T1.ID not in (@ID) AND T2.SMARTCODEONLY='false' ";
            using (SqlConnection sConn = new SqlConnection(connStr))
            {
                sConn.Open();

                SqlCommand sCmd = new SqlCommand(ValidMacID, sConn);
                sCmd.Parameters.Add("@MACID", SqlDbType.VarChar);
                sCmd.Parameters["@MACID"].Value = macId;
                sCmd.Parameters.Add("@ITEMCODE", SqlDbType.VarChar);
                sCmd.Parameters["@ITEMCODE"].Value = itemcode;
                sCmd.Parameters.Add("@ID", SqlDbType.Int);
                sCmd.Parameters["@ID"].Value = ID;
                sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                sCmd.Parameters["@USERNAME"].Value = username;
                using (IDataReader reader1 = sCmd.ExecuteReader())
                {
                    if (reader1.Read())
                    {
                        _countSerialcode = reader1.GetInt32(0);
                    }
                }
                sConn.Close();
            }
        }

        protected void VerifyRecord(Object sender, EventArgs e)
        {
            if (Context.Request.QueryString["DeliveryNum"] != null)
                _docnum = Convert.ToInt32(Context.Request.QueryString["DeliveryNum"]);
            else
                _docnum = 0;

            if (_docnum != 0)
            {
                string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
                using (SqlConnection sConn = new SqlConnection(connStr))
                {
                    sConn.Open();
                    SqlCommand sCmd = new SqlCommand("sp_GoodsIssue_IR_Verify", sConn);
                    sCmd.CommandType = CommandType.StoredProcedure;
                    sCmd.Parameters.Add("@DOCNUM", SqlDbType.Int);
                    sCmd.Parameters["@DOCNUM"].Value = _docnum;
                    sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                    sCmd.Parameters["@USERNAME"].Value = Username;

                    IDataReader reader1 = sCmd.ExecuteReader();
                    while (reader1.Read())
                    {
                        _verifiedRecords = Convert.ToInt32(reader1["VERIFIEDCOUNT"]);
                    }
                    if (_verifiedRecords == 0)
                    {
                        VerifiedDelivery = 1;
                        verifiedimg.Visible = true;
                        notverifiedimg.Visible = false;
                    }
                    else
                    {
                        ErrorRecords.Text = _verifiedRecords.ToString();
                    }

                    if (sConn != null)
                    {
                        sConn.Close();
                    }
                    if (reader1 != null)
                    {
                        reader1.Close();
                    }
                }
            }
        }

        protected void UpdateRecord(object sender, GridRecordEventArgs e)
        {
            //TODO: Fix this code;
            string username = User.Identity.Name;
            string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
            
            var serialCode = e.Record["SERIALCODE"].ToString();
            var itemCode =  e.Record["ITEMCODE"].ToString();
            var id =  int.Parse(e.Record["ID"].ToString());
            var macId = serialCode.Remove(serialCode.Length-17, 17);

            ValidateRecord(macId, itemCode, id);            

            string text1 = "Update C4_GOODSISSUE_IR_OUT set SERIALCODE= UPPER(@SERIALCODE), [USERNAME]= @USERNAME, MACID = @MACID where ID = @ID";

            if (_countSerialcode > 0)
            {
                MacIdErrorMessage(serialCode, itemCode);
            }
            else
            {
                using (SqlConnection sConn = new SqlConnection(connStr))
                {
                    sConn.Open();

                    SqlCommand sCmd = new SqlCommand(text1, sConn);
                    sCmd.Parameters.Add("@ID", SqlDbType.Int);
                    sCmd.Parameters["@ID"].Value = id;
                    sCmd.Parameters.Add("@SERIALCODE", SqlDbType.VarChar);
                    sCmd.Parameters["@SERIALCODE"].Value = serialCode;
                    sCmd.Parameters.Add("@MACID", SqlDbType.VarChar);
                    sCmd.Parameters["@MACID"].Value = macId;
                    sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                    sCmd.Parameters["@USERNAME"].Value = username;
                    int res = sCmd.ExecuteNonQuery();
                    sConn.Close();
                }
            }
        }

        protected void MacIdErrorMessage(String serialcode, String itemcode)
        {
            //TODO: Fix this code
            int docNumError = 0;
            int idError = 0;
            string username = User.Identity.Name;
            string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
            string ErrorMessage =
                "select T1.DOCNUM, T1.ID from C4_GOODSISSUE_IR_OUT T1 LEFT OUTER JOIN C4_MAINTAINPRODUCTID T2 ON T1.ITEMCODE = T2.ITEMCODE WHERE T2.SMARTCODEONLY = 0 AND T1.SERIALCODE = @SERIALCODE and T1.ITEMCODE= @ITEMCODE";
            using (SqlConnection sConn = new SqlConnection(connStr))
            {
                sConn.Open();

                SqlCommand sCmd = new SqlCommand(ErrorMessage, sConn);
                sCmd.Parameters.Add("@SERIALCODE", SqlDbType.VarChar);
                sCmd.Parameters["@SERIALCODE"].Value = serialcode;
                sCmd.Parameters.Add("@ITEMCODE", SqlDbType.VarChar);
                sCmd.Parameters["@ITEMCODE"].Value = itemcode;
                sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                sCmd.Parameters["@USERNAME"].Value = Username;
                using (IDataReader reader1 = sCmd.ExecuteReader())
                {
                    if (reader1.Read())
                    {
                        docNumError = reader1.GetInt32(0);
                        idError = reader1.GetInt32(1);
                    }
                }

                sConn.Close();
            }

            throw new Exception("This Serial Number is a duplicate on Inventory Request#: " + docNumError.ToString() + ", ID: " +
                                idError.ToString());
        }

        protected void InvalidItemcode()
        {
            throw new Exception("The Item Code You Entered is Not Valid.");
        }

        protected void InvalidDocnum()
        {
            throw new Exception("The Inventory Request you entered has items not in the maintain product section.");
        }
    }
}