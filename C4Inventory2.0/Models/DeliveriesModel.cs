using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Domain;
using Persistence.Repositories;

namespace C4Inventory2._0.Models
{
    public class DeliveriesModel
    {
        public string DeliveryNumber { get; set; }  //= Request.QueryString["DeliveryNum"];
        public string Warning { get; set; }// = CheckConfiguration.Text;
        public string VerifyError { get; set; } //= ErrorRecords.Text;
        public int VerifyBoolean { get; set; } //= VerifiedDelivery;

        //Jordan Stuff
        public string Username { get; set; }
        public int VerifiedDelivery { get; set; }
        public IList<Delivery> Deliveries { get; set; }
        private Delivery Delivery { get; set; }

        private InventoryRepository _repository;
        private int _countSerialcode;
        private int _countDocnum;
        private int _docnum;
        private int _verifiedCount;
        private int _verifiedRecords;

        public DeliveriesModel(string deliveryNum)
        {
            if (!string.IsNullOrEmpty(deliveryNum))
            {
                DeliveryNumber = deliveryNum;
                Delivery = _repository.GetDelivery(deliveryNum);
                // Deliveries = new List<Delivery>().Add(Delivery);
            }

        }

        protected void VerifyRecord(Object sender, EventArgs e)
        {
            var username = Username;

            _docnum = DeliveryNumber != null ? Convert.ToInt32(DeliveryNumber) : 0;

            if (_docnum != 0)
            {
                var connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
                using (var sConn = new SqlConnection(connStr))
                {
                    sConn.Open();
                    var sCmd = new SqlCommand("sp_Delivery_Verify", sConn) { CommandType = CommandType.StoredProcedure };
                    sCmd.Parameters.Add("@DOCNUM", SqlDbType.Int);
                    sCmd.Parameters["@DOCNUM"].Value = _docnum;
                    sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                    sCmd.Parameters["@USERNAME"].Value = username;

                    IDataReader reader1 = sCmd.ExecuteReader();
                    while (reader1.Read())
                    {
                        _verifiedRecords = Convert.ToInt32(reader1["VERIFIEDCOUNT"]);
                    }
                    if (_verifiedRecords == 0)
                    {
                        VerifiedDelivery = 1;
                        // verifiedimg.Visible = true;
                        //notverifiedimg.Visible = false;
                    }
                    else
                    {
                        // ErrorRecords.Text = _verifiedRecords.ToString();
                    }

                    sConn.Close();
                    reader1.Close();
                }
            }
        }
    }
}