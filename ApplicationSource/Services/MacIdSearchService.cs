using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using ApplicationSource.Interfaces;
using ApplicationSource.Models;
using System.Configuration;
using Domain;

namespace ApplicationSource.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class MacIdSearchService : IMacIdSearchService
    {
        public MacIdItem LocateMacIds(MacIdItem macItem)
        {

            string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;

            if (!string.IsNullOrEmpty(macItem.MacId))
            {
                var macId = macItem.MacId;
                try
                {
                    using (var sConn = new SqlConnection(connStr))
                    {

                        var modifiedMac = "";

                        if (macId.Length >= 29)
                            modifiedMac = macId.Remove(macId.Length - 17, 17);
                        else
                            modifiedMac = macId;

                        if (modifiedMac.Length == 12 || modifiedMac.Length == 16)
                        {
                            sConn.Open();
                            var sCmd = new SqlCommand("sp_LocateSmartMac", sConn) { CommandType = CommandType.StoredProcedure };
                            sCmd.Parameters.Add("@MACID", SqlDbType.NVarChar);
                            sCmd.Parameters["@MACID"].Value = modifiedMac;
                            using (IDataReader reader1 = sCmd.ExecuteReader())
                            {
                                while (reader1.Read())
                                {
                                    var docnum = reader1["DOCNUM"].ToString();
                                    var isIrDelivery = reader1["ISIRDELIVERY"].ToString() == "1";
                                    if (!string.IsNullOrEmpty(docnum))
                                    {
                                        macItem.DeliveryNumber = docnum;
                                        macItem.IsIRDelivery = isIrDelivery;
                                    }
                                    else
                                    {
                                        macItem.ErrorMessage = "Delivery not found for this Smart Mac.";
                                        macItem.DeliveryNumber = "0";
                                        macItem.HasErrors = true;
                                    }

                                }
                            }
                        }
                        else
                        {
                            macItem.ErrorMessage += " Incorrect Mac Id Length, or Mac Id is not a serialized number.";
                            macItem.DeliveryNumber = "0";
                            macItem.HasErrors = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    macItem.ErrorMessage += "Delivery not found for this Smart Mac.";
                    macItem.DeliveryNumber = "0";
                    macItem.HasErrors = true;
                }
            }

            return macItem;
        }
    }
}
