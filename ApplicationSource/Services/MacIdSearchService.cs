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
        public IList<MacIdItem> LocateMacIds(IList<MacIdItem> model)
        {

            string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;

            foreach (var item in model)
            {
                if (!string.IsNullOrEmpty(item.MacId))
                {
                    var macId = item.MacId;
                    try
                    {
                        using (var sConn = new SqlConnection(connStr))
                        {

                            var modifiedMac = "";

                            if (macId.Length >= 29)
                                modifiedMac = macId.Remove(macId.Length - 17, 17);
                            else
                                modifiedMac = macId;

                            if (modifiedMac.Length == 14 || modifiedMac.Length == 16)
                            {
                                sConn.Open();
                                var sCmd = new SqlCommand("sp_LocateSmartMac", sConn) { CommandType = CommandType.StoredProcedure };
                                sCmd.Parameters.Add("@MACID", SqlDbType.NVarChar);
                                sCmd.Parameters["@MACID"].Value = modifiedMac;
                                using (IDataReader reader1 = sCmd.ExecuteReader())
                                {
                                    while (reader1.Read())
                                    {
                                        var _docnum = reader1["DOCNUM"].ToString();
                                        var _isIrDelivery = reader1["ISIRDELIVERY"].ToString() == "1";
                                        if (!string.IsNullOrEmpty(_docnum))
                                        {
                                            item.DeliveryNumber = _docnum;
                                            item.IsIRDelivery = _isIrDelivery;
                                        }
                                        else
                                            item.ErrorMessage = "Delivery not found for this Smart Mac.";
                                    }
                                }
                            }
                            else
                            {
                                item.ErrorMessage += " Incorrect Mac Id Length, or Mac Id is not a serialized number.";
                                item.DeliveryNumber = "0";
                                item.HasErrors = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        item.ErrorMessage += "Delivery not found for this Smart Mac.";
                        item.DeliveryNumber = "0";
                        item.HasErrors = true;
                    }
                }
            }

            return model;
        }
    }
}
