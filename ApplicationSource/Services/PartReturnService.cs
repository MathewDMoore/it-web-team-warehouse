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
        readonly string _userName = HttpContext.Current.User.Identity.Name;
        readonly string _connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;

        public IList<ReturnLineItem> ReturnParts(IList<ReturnLineItem> model)
        {
            foreach (var item in model)
            {
                if (!string.IsNullOrEmpty(item.MacId))
                {
                    var macId = item.MacId;
                    var parsedMacId = macId.Length >= 29 ? macId.Remove(macId.Length - 17, 17) : macId;

                    try
                    {
                        using (var sConn = new SqlConnection(_connStr))
                        {
                            sConn.Open();

                            var sCmd = new SqlCommand("sp_ReturnItemByMacId", sConn) { CommandType = CommandType.StoredProcedure };
                            sCmd.Parameters.Add("@MACID", SqlDbType.NVarChar);
                            sCmd.Parameters["@MACID"].Value = parsedMacId;
                            sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                            sCmd.Parameters["@USERNAME"].Value = _userName;
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
                        item.ErrorMessage = "Error returning this item. Please Review MacId.";
                    }
                }
                else
                    item.ErrorMessage = "Error returning this item. Please Review MacId.";
            }
            return model;
        }

        public bool ClearDelivery(string docNumber)
        {
            if (!string.IsNullOrEmpty(docNumber))
            {
                bool success;

                try
                {
                    using (var sConn = new SqlConnection(_connStr))
                    {
                        sConn.Open();

                        var sCmd = new SqlCommand("sp_ClearDelivery", sConn) { CommandType = CommandType.StoredProcedure };
                        sCmd.Parameters.Add("@DELIVERYNUMBER", SqlDbType.NVarChar);
                        sCmd.Parameters["@DELIVERYNUMBER"].Value = docNumber;
                        sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                        sCmd.Parameters["@USERNAME"].Value = _userName;
                        using (IDataReader reader1 = sCmd.ExecuteReader())
                        {
                            success = true;
                        }
                    }
                }
                catch (Exception)
                {

                    success = false;
                }

                return success;
            }

            return false;
        }

        public bool ClearIRDelivery(string docNumber)
        {
            if (!string.IsNullOrEmpty(docNumber))
            {
                bool success;

                try
                {
                    using (var sConn = new SqlConnection(_connStr))
                    {
                        sConn.Open();

                        var sCmd = new SqlCommand("sp_ClearIRDelivery", sConn) { CommandType = CommandType.StoredProcedure };
                        sCmd.Parameters.Add("@DELIVERYNUMBER", SqlDbType.NVarChar);
                        sCmd.Parameters["@DELIVERYNUMBER"].Value = docNumber;
                        sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                        sCmd.Parameters["@USERNAME"].Value = _userName;
                        using (IDataReader reader1 = sCmd.ExecuteReader())
                        {
                            success = true;
                        }
                    }
                }
                catch (Exception e)
                {

                    success = false;
                }

                return success;
            }

            return false;
        }

    }
}