using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using ApplicationSource.Interfaces;
using System.Configuration;
using ApplicationSource.Models;

namespace ApplicationSource.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class VerifyUniqueMacService : IVerifyUniqueMacService
    {
        private readonly SqlConnection _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString);

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
                        using (_conn)
                        {
                            _conn.Open();

                            var sCmd = new SqlCommand("sp_LocateSmartMac", _conn) { CommandType = CommandType.StoredProcedure };
                            sCmd.Parameters.Add("@MACID", SqlDbType.NVarChar);
                            sCmd.Parameters["@MACID"].Value = parsedMacId;
                            sCmd.Parameters.Add("@PRODUCTGROUP", SqlDbType.NVarChar);
                            sCmd.Parameters["@PRODUCTGROUP"].Value = productGroup;
                            using (IDataReader reader1 = sCmd.ExecuteReader())
                            {

                                while (reader1.Read())
                                {
                                    var item = reader1.GetInt32(0);
                                    if (item > 0)
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
                            _conn.Close();
                            if (model.IsUnique )
                            {
                                if (!UpdateRecord(model.SerialCode, model.MacId, model.Id))
                                            model.ErrorMessage = "There was an error saving this item into the database. Please review the SerialCode or contact IT support.";
                            }
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
        private bool UpdateRecord(string serialCode, string macId, int id)
        {
            const string text1 = "Update C4_SERIALNUMBERS_OUT set SERIALCODE= UPPER(@SERIALCODE), [USERNAME]= @USERNAME, MACID = @MACID, RETURNEDBYUSER = null where ID = @ID";
            var sCmd = new SqlCommand(text1, _conn);
            var success = false;
            using (_conn)
            {
                _conn.Open();
                sCmd.Parameters.Add("@ID", SqlDbType.Int);
                sCmd.Parameters["@ID"].Value = id;
                sCmd.Parameters.Add("@SERIALCODE", SqlDbType.VarChar);
                sCmd.Parameters["@SERIALCODE"].Value = serialCode;
                sCmd.Parameters.Add("@MACID", SqlDbType.VarChar);
                sCmd.Parameters["@MACID"].Value = macId;
                sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                sCmd.Parameters["@USERNAME"].Value = HttpContext.Current.User.Identity.Name;
                success = sCmd.ExecuteNonQuery() > 0;
                _conn.Close();
            }

            return success;
        }
    }
}
