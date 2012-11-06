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

namespace ApplicationSource.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class PartReturnService : IPartReturnService
    {


        public IList<ReturnLineItem> ReturnParts(IList<ReturnLineItem> model)
        {
           
            var userName = HttpContext.Current.User.Identity.Name;
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
                            var goodIssueId = 0;
                            var sCmd = new SqlCommand("sp_ReturnIRInfoBySmartMac", sConn) { CommandType = CommandType.StoredProcedure };
                            sCmd.Parameters.Add("@SmartMac", SqlDbType.NVarChar);
                            sCmd.Parameters["@SmartMac"].Value = item.SmartMac;
                            using (IDataReader reader1 = sCmd.ExecuteReader())
                            {
                                while (reader1.Read())
                                {
                                    goodIssueId = Convert.ToInt32(reader1["ID"].ToString());
                                    item.DocNum = reader1["DOCNUM"].ToString();
                                }
                            }

                            if (goodIssueId > 0) 
                            {
                                sCmd = new SqlCommand("sp_ReturnIRByLineItem", sConn) { CommandType = CommandType.StoredProcedure };
                                sCmd.Parameters.Add("@ID", SqlDbType.Int);
                                sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                                sCmd.Parameters["@USERNAME"].Value = HttpContext.Current.User.Identity.Name;
                                sCmd.Parameters["@ID"].Value = goodIssueId;
                                using (IDataReader reader1 = sCmd.ExecuteReader())
                                {
                                    if (reader1.RecordsAffected < 1)
                                    {
                                        item.ErrorMessage = "Error returning this item. Item does not exist on a shipment.";
                                    }
                                }
                            }
                            else
                            {
                                item.ErrorMessage = "Error returning this item. Item does not exist on a shipment.";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        item.ErrorMessage = "Error returning this item. Please Review SmartMac.";
                    }
                }
            }

            return model;
        }
    }
}