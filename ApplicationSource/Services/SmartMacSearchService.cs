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
    public class SmartMacSearchService : ISmartMacSearchService
    {
        public IList<SmartMacItem> LocateSmartMac(IList<SmartMacItem> model)
        {

            string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;

            foreach (var item in model)
            {
                if (!string.IsNullOrEmpty(item.SmartMac))   
                {
                    try
                    {
                        using (var sConn = new SqlConnection(connStr))
                        {
                            sConn.Open();

                            var sCmd = new SqlCommand("sp_LocateSmartMac", sConn) { CommandType = CommandType.StoredProcedure };
                            sCmd.Parameters.Add("@SmartMac", SqlDbType.NVarChar);
                            sCmd.Parameters["@SmartMac"].Value = item.SmartMac;
                            using (IDataReader reader1 = sCmd.ExecuteReader())
                            {
                                while (reader1.Read())
                                {
                                    var _docnum = reader1["DOCNUM"].ToString();
                                    if (!string.IsNullOrEmpty(_docnum))
                                    {
                                        item.DeliveryNumber = _docnum;
                                    }
                                    else
                                        item.ErrorMessage = "Delivery not found for this Smart Mac.";
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        item.ErrorMessage = "Delivery not found for this Smart Mac.";
                    }
                }
            }

            return model;
        }
    }
}
