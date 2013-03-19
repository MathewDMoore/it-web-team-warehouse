using System;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using System.ServiceModel.Activation;
using ApplicationSource.Interfaces;
using System.Configuration;

namespace ApplicationSource.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class VerifyUniqueMacService : IVerifyUniqueMacService
    {
        public bool VerifyUniqueMac(string model)
        {
            var result = false;
            var connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;

            if (!string.IsNullOrEmpty(model))
            {
                var parsedMacId = model.Length >= 29 ? model.Remove(model.Length -17, 17) : model;
                try
                {
                    if (parsedMacId.Length == 12 || parsedMacId.Length == 16)
                    {
                        using (var sConn = new SqlConnection(connStr))
                        {
                            sConn.Open();

                            var sCmd = new SqlCommand("sp_LocateSmartMac", sConn) { CommandType = CommandType.StoredProcedure };
                            sCmd.Parameters.Add("@MACID", SqlDbType.NVarChar);
                            sCmd.Parameters["@MACID"].Value = parsedMacId;
                            using (IDataReader reader1 = sCmd.ExecuteReader())
                            {
                                result = reader1.RecordsAffected < 1;
                            }
                            sConn.Close();
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
            return result;
        }
    }
}
    