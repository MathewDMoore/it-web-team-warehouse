using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Obout.ComboBox;
using Obout.Grid;

namespace C4InventorySerialization.Admin
{
    public partial class MaintainProductId : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            CreateGrid();
        }

        protected void RebindGrid(object sender, EventArgs e)
        {
            CreateGrid();
        }


        protected void CreateGrid()
        {
            var db = new SerializationDataContext();
            var query = from p in db.MPIs
                        orderby p.ID
                        select new
                                   {
                                       p.ID,
                                       p.PRODUCTID,
                                       p.ITEMCODE,
                                       p.COLOR,
                                       p.SMARTCODEONLY,
                                       p.NOSERIALIZATION
                                   };

            MPIGrid.DataSource = query;
            MPIGrid.DataBind();
        }

        protected void ComboBox1_LoadingItems(object sender, ComboBoxLoadingItemsEventArgs e)
        {
            var data = GetItems(e.Text);

            var comboBox1Load = sender as ComboBox;
            //ComboBox1Load.TemplateControl.NamingContainer.ClientID.Equals("ComboBox1Load");

            for (var i = 0; i < data.Rows.Count; i++)
            {
                comboBox1Load.Items.Add(new ComboBoxItem(data.Rows[i]["itemcode"].ToString()));
            }

            e.ItemsLoadedCount = data.Rows.Count;
            e.ItemsCount = data.Rows.Count;
        }
        protected DataTable GetItems(string item)
        {
                var connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
            var serverLocation = ConfigurationManager.AppSettings["ServerLocation"];
                using (SqlConnection sConn = new SqlConnection(connStr))
                {
                    sConn.Open();
                    SqlCommand sCmd = new SqlCommand("sp_GetSapItemsOnDemand", sConn);
                    sCmd.CommandType = CommandType.StoredProcedure;
                    sCmd.Parameters.Add("@ITEM", SqlDbType.NVarChar);
                    var modifiedItem = "'%" + item + "%'";
                    sCmd.Parameters["@ITEM"].Value = modifiedItem;
                    sCmd.Parameters.Add("@SERVERLOCATION", SqlDbType.NVarChar);
                    sCmd.Parameters["@SERVERLOCATION"].Value = serverLocation;


                    using (IDataReader reader1 = sCmd.ExecuteReader())
                    {
                        var ds = new DataSet();
                        ds.Load(reader1, LoadOption.Upsert, "itemcode");
                        return ds.Tables[0];
                    }

                }

        }

        protected void UpdateRecord(object sender, GridRecordEventArgs e)
        {
            var db = new SerializationDataContext();
            var username = User.Identity.Name;
            int tId = Convert.ToInt32(e.Record["ID"]);
            bool smartCodeBool = false;
            bool noSerialBool = false;
            string noSerial = e.Record["NOSERIALIZATION"].ToString();
            string smartCode = e.Record["SMARTCODEONLY"].ToString();

            if (noSerial == "true" || noSerial == "True")
            {
                noSerialBool = true;
            }

            if (smartCode == "true" || smartCode == "True")
            {
                smartCodeBool = true;
            }

            //Pull up record prior to updating.
            var productBeforeUpdate = (from p in db.MPIs
                                       where p.ID == tId
                                       select p).First();

            //Make a Record in the MaintainProductID_History table prior to updating.
            var productBeforeUpdateHistory = new MPI_History();

            productBeforeUpdateHistory.DATE = DateTime.Now;
            productBeforeUpdateHistory.PRODUCTID = productBeforeUpdate.PRODUCTID;
            productBeforeUpdateHistory.ITEMCODE = productBeforeUpdate.ITEMCODE;
            productBeforeUpdateHistory.COLOR = productBeforeUpdate.COLOR;
            productBeforeUpdateHistory.NOSERIALIZATION = productBeforeUpdate.NOSERIALIZATION;
            productBeforeUpdateHistory.SMARTCODEONLY = productBeforeUpdate.SMARTCODEONLY;
            productBeforeUpdateHistory.TYPE = "UPDATE-BEFORE";
            productBeforeUpdateHistory.USERNAME = username;

            db.MPI_Histories.InsertOnSubmit(productBeforeUpdateHistory);

            //Update MaintainProductID to update the product.
            var productUpdate = (from p in db.MPIs
                                 where p.ID == tId
                                 select p).First();

            productUpdate.ID = tId;
            productUpdate.PRODUCTID = e.Record["PRODUCTID"].ToString();
            productUpdate.ITEMCODE = e.Record["ITEMCODE"].ToString();
            productUpdate.COLOR = e.Record["COLOR"].ToString().ToUpper();
            productUpdate.NOSERIALIZATION = noSerialBool;
            productUpdate.SMARTCODEONLY = smartCodeBool;

            //Make a Record in the MaintainProductID_History table after updating.
            MPI_History productAfterUpdateHistory = new MPI_History();
            productAfterUpdateHistory.DATE = DateTime.Now;
            productAfterUpdateHistory.PRODUCTID = productUpdate.PRODUCTID;
            productAfterUpdateHistory.ITEMCODE = productUpdate.ITEMCODE;
            productAfterUpdateHistory.COLOR = productUpdate.COLOR;
            productAfterUpdateHistory.NOSERIALIZATION = productUpdate.NOSERIALIZATION;
            productAfterUpdateHistory.SMARTCODEONLY = productUpdate.SMARTCODEONLY;
            productAfterUpdateHistory.TYPE = "UPDATE-AFTER";
            productAfterUpdateHistory.USERNAME = username;

            db.MPI_Histories.InsertOnSubmit(productAfterUpdateHistory);
            db.SubmitChanges();
        }

        protected void InsertRecord(object sender, GridRecordEventArgs e)
        {
            var db = new SerializationDataContext();
            var username = User.Identity.Name;
            //            int tId = Convert.ToInt32(e.Record["ID"]);
            bool smartCodeBool = false;
            bool noSerialBool = false;
            string noSerial = e.Record["NOSERIALIZATION"].ToString();
            string smartCode = e.Record["SMARTCODEONLY"].ToString();

            if (noSerial == "true" || noSerial == "True")
            {
                noSerialBool = true;
            }

            if (smartCode == "true" || smartCode == "True")
            {
                smartCodeBool = true;
            }

            //Add new record.
            var product = new MPI()
                              {
                                  PRODUCTID = e.Record["PRODUCTID"].ToString(),
                                  ITEMCODE = e.Record["ITEMCODE"].ToString().ToUpper(),
                                  COLOR = e.Record["COLOR"].ToString().ToUpper(),
                                  NOSERIALIZATION = noSerialBool,
                                  SMARTCODEONLY = smartCodeBool
                              };


            var productHistoryInsert = new MPI_History();
            productHistoryInsert.DATE = DateTime.Now;
            productHistoryInsert.PRODUCTID = product.PRODUCTID;
            productHistoryInsert.ITEMCODE = product.ITEMCODE;
            productHistoryInsert.COLOR = product.COLOR;
            productHistoryInsert.NOSERIALIZATION = product.NOSERIALIZATION;
            productHistoryInsert.SMARTCODEONLY = product.SMARTCODEONLY;
            productHistoryInsert.TYPE = "INSERT";
            productHistoryInsert.USERNAME = username;

            //Insert new record on MPI DB.
            db.MPIs.InsertOnSubmit(product);
            //Insert new Insert record on MPI_Histories DB.
            db.MPI_Histories.InsertOnSubmit(productHistoryInsert);
            db.SubmitChanges();
        }


        protected void DeleteRecord(object sender, GridRecordEventArgs e)
        {
            var db = new SerializationDataContext();
            var username = User.Identity.Name;
            int tId = Convert.ToInt32(e.Record["ID"]);
            bool smartCodeBool = false;
            bool noSerialBool = false;
            string noSerial = e.Record["NOSERIALIZATION"].ToString();
            string smartCode = e.Record["SMARTCODEONLY"].ToString();

            if (noSerial == "true" || noSerial == "True")
            {
                noSerialBool = true;
            }

            if (smartCode == "true" || smartCode == "True")
            {
                smartCodeBool = true;
            }
            var productDelete = (from p in db.MPIs
                                 where p.ID == tId
                                 select p).First();

            var productHistory = new MPI_History();
            productHistory.DATE = DateTime.Now;
            productHistory.PRODUCTID = productDelete.PRODUCTID;
            productHistory.ITEMCODE = productDelete.ITEMCODE;
            productHistory.COLOR = productDelete.COLOR;
            productHistory.NOSERIALIZATION = productDelete.NOSERIALIZATION;
            productHistory.SMARTCODEONLY = productDelete.SMARTCODEONLY;
            productHistory.TYPE = "DELETE";
            productHistory.USERNAME = username;

            //Delete Product from MPI database.
            db.MPIs.DeleteOnSubmit(productDelete);
            //Insert Deletion History into MPI History DB.
            db.MPI_Histories.InsertOnSubmit(productHistory);
            db.SubmitChanges();
        }

    }
}