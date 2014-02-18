using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Obout.Grid;

namespace C4InventorySerialization.Content
{
    public partial class ScanSerialNumber : System.Web.UI.Page
    {
        private int _countSerialcode;
        private int _countDocnum;
        private int _docnum;
        private int _verifiedCount;
        private int _verifiedRecords;
        public int VerifiedDelivery;
        public string _userName;
        private string _serverLocation = ConfigurationManager.AppSettings["ServerLocation"];
        public string UserName
        {
            get
            {
                var _userName = "";
                if (Session["User"] != null)
                {
                    _userName = ((ApplicationSource.User)Session["User"]).UserName;
                }
                return _userName;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack) return;

            _docnum = Context.Request.QueryString["DeliveryNum"] != null ? Convert.ToInt32(Context.Request.QueryString["DeliveryNum"]) : 0;
            _userName = User.Identity.Name;

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

            CreateGrid();
            CreateGrid2();

            //VerifyDelivery.Click += new EventHandler(this.VerifyRecord);
        }

        protected void RebindGrid(object sender, EventArgs e)
        {
            CreateGrid();
            CreateGrid2();
        }

        private void CreateGrid()
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
                    SqlCommand sCmd = new SqlCommand("sp_delivery_synch", sConn);
                    sCmd.CommandType = CommandType.StoredProcedure;
                    sCmd.Parameters.Add("@DOCNUM", SqlDbType.Int);
                    sCmd.Parameters.Add("@SERVERLOCATION", SqlDbType.VarChar);
                    sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);

                    sCmd.Parameters["@DOCNUM"].Value = _docnum;
                    sCmd.Parameters["@SERVERLOCATION"].Value = _serverLocation;
                    sCmd.Parameters["@USERNAME"].Value = User.Identity.Name;

                    using (IDataReader reader1 = sCmd.ExecuteReader())
                    {
                        grid1.DataSource = reader1;
                        grid1.DataBind();
                    }
                    sConn.Close();
                }
            }
        }

        private void CreateGrid2()
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
                    SqlCommand sCmd = new SqlCommand("sp_delivery_header", sConn);
                    sCmd.CommandType = CommandType.StoredProcedure;
                    sCmd.Parameters.Add("@DOCNUM", SqlDbType.Int);
                    sCmd.Parameters.Add("@SERVERLOCATION", SqlDbType.VarChar);
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

        protected void ValidateDocnum(int docnum)
        {

            string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
            string ValidDOCNUM =
                "SELECT COUNT(*) AS PRODUCTCOUNT FROM [C4_SERIALNUMBERS_OUT] WHERE PRODUCTID IS NULL AND DOCNUM = @DOCNUM";
            string VerifiedDOCNUM =
                "SELECT COUNT(*) AS VERIFIEDCOUNT FROM [C4_SERIALNUMBERS_OUT] WHERE VERIFIED=1 AND DOCNUM = @DOCNUM";
            using (SqlConnection sConn = new SqlConnection(connStr))
            {
                sConn.Open();

                SqlCommand sCmd = new SqlCommand(ValidDOCNUM, sConn);
                SqlCommand sCmd2 = new SqlCommand(VerifiedDOCNUM, sConn);
                sCmd.Parameters.Add("@DOCNUM", SqlDbType.Int);
                sCmd.Parameters["@DOCNUM"].Value = docnum;
                sCmd2.Parameters.Add("@DOCNUM", SqlDbType.Int);
                sCmd2.Parameters["@DOCNUM"].Value = docnum;
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

        protected void ValidateRecord(String macid, String itemCode, int id)
        {

            string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
            string ValidMacID =
                "select count(*) from C4_SERIALNUMBERS_OUT T1, C4_MAINTAINPRODUCTID T2 where T1.PRODUCTID =T2.PRODUCTID AND T1.MACID = @MACID and T1.ITEMCODE = @ITEMCODE and T1.ID not in (@ID) AND T2.SMARTCODEONLY='false' ";
            using (SqlConnection sConn = new SqlConnection(connStr))
            {
                sConn.Open();

                //sConn.BeginTransaction("TransactionFunction");

                SqlCommand sCmd = new SqlCommand(ValidMacID, sConn);
                sCmd.Parameters.Add("@MACID", SqlDbType.VarChar);
                sCmd.Parameters["@MACID"].Value = macid;
                sCmd.Parameters.Add("@ITEMCODE", SqlDbType.VarChar);
                sCmd.Parameters["@ITEMCODE"].Value = itemCode;
                sCmd.Parameters.Add("@ID", SqlDbType.Int);
                sCmd.Parameters["@ID"].Value = id;
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
                    SqlCommand sCmd = new SqlCommand("sp_Delivery_Verify", sConn);
                    sCmd.CommandType = CommandType.StoredProcedure;
                    sCmd.Parameters.Add("@DOCNUM", SqlDbType.Int);
                    sCmd.Parameters["@DOCNUM"].Value = _docnum;
                    sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                    sCmd.Parameters["@USERNAME"].Value = User.Identity.Name;

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
            var connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;

            var serialCode = e.Record["SERIALCODE"].ToString();
            var itemCode = e.Record["REALITEMCODE"].ToString();
            var id = int.Parse(e.Record["ID"].ToString());
            var isSmartCodeOnly = bool.Parse(e.Record["SMARTCODEONLY"].ToString());

            var macId = serialCode.Length > 17 ? serialCode.Remove(serialCode.Length - 17, 17) : serialCode;

            ValidateRecord(macId, itemCode, id);

            const string text1 = "Update C4_SERIALNUMBERS_OUT set SERIALCODE= UPPER(@SERIALCODE), [USERNAME]= @USERNAME, MACID = @MACID, RETURNEDBYUSER = null where ID = @ID";

            if (_countSerialcode > 0 && !isSmartCodeOnly)
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
                    sCmd.Parameters["@USERNAME"].Value = User.Identity.Name;
                    var res = sCmd.ExecuteNonQuery();
                    sConn.Close();
                }
            }
        }

        protected void MacIdErrorMessage(String serialcode, String itemcode)
        {

            var docNumError = 0;
            var idError = 0;
            var connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
            const string errorMessage = "select T1.DOCNUM, T1.ID from C4_SERIALNUMBERS_OUT T1 LEFT OUTER JOIN C4_MAINTAINPRODUCTID T2 ON T1.ITEMCODE = T2.ITEMCODE WHERE T2.SMARTCODEONLY = 0 AND T1.SERIALCODE = @SERIALCODE and T1.ITEMCODE= @ITEMCODE";
            using (var sConn = new SqlConnection(connStr))
            {
                sConn.Open();

                var sCmd = new SqlCommand(errorMessage, sConn);
                sCmd.Parameters.Add("@SERIALCODE", SqlDbType.VarChar);
                sCmd.Parameters["@SERIALCODE"].Value = serialcode;
                sCmd.Parameters.Add("@ITEMCODE", SqlDbType.VarChar);
                sCmd.Parameters["@ITEMCODE"].Value = itemcode;
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

            throw new Exception("This Serial Number is a duplicate on Delivery #: " + docNumError.ToString() + ", ID: " +
                                idError.ToString());
        }

        protected void InvalidItemcode()
        {
            throw new Exception("The Item Code You Entered is Not Valid.");
        }

        protected void InvalidDocnum()
        {
            throw new Exception("The Delivery you entered has items not in the maintain product section.");
        }
    }
}