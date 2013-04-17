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
        readonly string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
        private bool result = false;

        public bool VerifyUniqueMac(string model)
        {

            if (!string.IsNullOrEmpty(model))
            {
                var parsedMacId = model.Length >= 29 ? model.Remove(model.Length - 17, 17) : model;
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

        public bool VerifyUniqueSmartCode(string model)
        {
            if (!string.IsNullOrEmpty(model))
            {
                try
                {
                    using (var sConn = new SqlConnection(connStr))
                    {
                        sConn.Open();

                        var sCmd = new SqlCommand("sp_LocateSmartCode", sConn) { CommandType = CommandType.StoredProcedure };
                        sCmd.Parameters.Add("@SERIALCODE", SqlDbType.NVarChar);
                        sCmd.Parameters["@SERIALCODE"].Value = model;
                        using (IDataReader reader1 = sCmd.ExecuteReader())
                        {
                            result = reader1.RecordsAffected < 1;
                        }
                        sConn.Close();
                    }
                }
                catch (Exception e)
                {

                }
            }
            return result;
        }


    }
}
