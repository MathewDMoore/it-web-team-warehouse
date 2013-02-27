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
    public class VerifyUniqueMacService : IVerifyUniqueMacService
    {
        public bool VerifyUniqueMac(string model)
        {
            var result = false;
            var parsedMacID = "";

            string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;

            if (!string.IsNullOrEmpty(model))
            {
                if (model.Length >= 29)
                {
                    parsedMacID = model.Remove(model.Length -17, 17);
                }
                else
                {
                    parsedMacID = model;
                }
                try
                {
                    if (parsedMacID.Length == 12 || parsedMacID.Length == 16)
                    {
                        using (var sConn = new SqlConnection(connStr))
                        {
                            sConn.Open();

                            var sCmd = new SqlCommand("sp_LocateSmartMac", sConn) { CommandType = CommandType.StoredProcedure };
                            sCmd.Parameters.Add("@MACID", SqlDbType.NVarChar);
                            sCmd.Parameters["@MACID"].Value = parsedMacID;
                            using (IDataReader reader1 = sCmd.ExecuteReader())
                            {
                                if (reader1.RecordsAffected < 1)
                                {
                                    result = true;
                                }
                                else
                                    result = false;
                            }
                            sConn.Close();
                        }
                    }
                    else
                        result = true;
                }
                catch
                {

                }
            }

            return result;
        }
    }
}
