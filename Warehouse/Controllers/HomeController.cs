using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Warehouse.Controllers
{
    public class HomeController : Controller
    {
        private int _countSerialcode;
        private int _countDocnum;
        private int _docnum;
        private int _verifiedCount;
        private int _verifiedRecords;
        public int VerifiedDelivery;
        public string _userName;
        private string _serverLocation = ConfigurationManager.AppSettings["ServerLocation"];

        public ActionResult ScanSerialNumber(int deliveryNum=0)
        {
//            _docnum = deliveryNum;
//            _userName = User.Identity.Name;
//
//            ValidateDocnum(_docnum);
//            CheckConfiguration.Text = _countDocnum.ToString();
//
//            if (_verifiedCount > 0)
//            {
//                verifiedimg.Visible = true;
//            }
//            else
//            {
//                if (_docnum != 0)
//                {
//                    notverifiedimg.Visible = true;
//                }
//            }
//
//            CreateGrid();
//            CreateGrid2();
            return View("ScanSerialNumber");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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
        private void CreateGrid()
        {
//            if (Context.Request.QueryString["DeliveryNum"] != null)
//                _docnum = Convert.ToInt32(Context.Request.QueryString["DeliveryNum"]);
//            else
//                _docnum = 0;
//
//            if (_docnum != 0)
//            {
//                string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
//
//                using (SqlConnection sConn = new SqlConnection(connStr))
//                {
//                    sConn.Open();
//                    SqlCommand sCmd = new SqlCommand("sp_delivery_synch", sConn);
//                    sCmd.CommandType = CommandType.StoredProcedure;
//                    sCmd.Parameters.Add("@DOCNUM", SqlDbType.Int);
//                    sCmd.Parameters.Add("@SERVERLOCATION", SqlDbType.VarChar);
//                    sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
//
//                    sCmd.Parameters["@DOCNUM"].Value = _docnum;
//                    sCmd.Parameters["@SERVERLOCATION"].Value = _serverLocation;
//                    sCmd.Parameters["@USERNAME"].Value = User.Identity.Name;
//
//                    using (IDataReader reader1 = sCmd.ExecuteReader())
//                    {
//                        grid1.DataSource = reader1;
//                        grid1.DataBind();
//                    }
//                    sConn.Close();
//                }
//            }
        }
    }
}