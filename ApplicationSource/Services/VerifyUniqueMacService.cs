using System;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using System.ServiceModel.Activation;
using ApplicationSource.Interfaces;
using System.Configuration;
using ApplicationSource.Models;

namespace ApplicationSource.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class VerifyUniqueMacService : IVerifyUniqueMacService
    {
        readonly string _connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;

        public VerifyUniqueMacModel VerifyUniqueMac(VerifyUniqueMacModel model)
        {
            var macId = model.MacId;
            var productGroup = model.ProductGroup;
            if (!string.IsNullOrEmpty(model.MacId) || !string.IsNullOrEmpty(productGroup))
            {
                var parsedMacId = macId.Length >= 29 ? macId.Remove(macId.Length - 17, 17) : macId;
                try
                {
                    if (parsedMacId.Length == 12 || parsedMacId.Length == 16)
                    {
                        using (var sConn = new SqlConnection(_connStr))
                        {
                            sConn.Open();

                            var sCmd = new SqlCommand("sp_LocateSmartMac", sConn) { CommandType = CommandType.StoredProcedure };
                            sCmd.Parameters.Add("@MACID", SqlDbType.NVarChar);
                            sCmd.Parameters["@MACID"].Value = parsedMacId;                            
                            sCmd.Parameters.Add("@PRODUCTGROUP", SqlDbType.NVarChar);
                            sCmd.Parameters["@PRODUCTGROUP"].Value = productGroup;
                            using (IDataReader reader1 = sCmd.ExecuteReader())
                            {
                               
                                while (reader1.Read())
                                {
                                    var item = reader1.GetInt32(0);
                                    if (item  > 0)
                                    {
                                        model.IsUnique = false;
                                        model.ErrorMessage = "This order exists on another order - #";
                                    }
                                    else
                                    {
                                        model.IsUnique = true;
                                    }
                                }
                                reader1.NextResult();
                                while (reader1.Read())
                                {
                                    model.ErrorDeliveryNumber = reader1["DOCNUM"].ToString();

                                }
                            }
                            sConn.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    model.IsUnique = false;
                    model.ErrorMessage = e.Message;
                }
            }
            return model;
        }
    }
}
