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

                            var sCmd = new SqlCommand("sp_ReturnItemByMacId", sConn) { CommandType = CommandType.StoredProcedure };
                            sCmd.Parameters.Add("@SmartMac", SqlDbType.NVarChar);
                            sCmd.Parameters["@SmartMac"].Value = item.SmartMac;
                            sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                            sCmd.Parameters["@USERNAME"].Value = HttpContext.Current.User.Identity.Name;
                            using (IDataReader reader1 = sCmd.ExecuteReader())
                            {
                                if (reader1.RecordsAffected < 1)
                                {
                                    item.ErrorMessage = "This Mac has already been returned, or does not exist on a delivery.";
                                }
                                else
                                    item.Success = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        item.ErrorMessage = "Error returning this item. Please Review SmartMac.";
                    }
                }
                else
                    item.ErrorMessage = "Error returning this item. Please Review SmartMac.";
            }
            return model;
        }
    }
}