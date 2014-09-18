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
                if (!string.IsNullOrEmpty(item.SmartCode))
                {
                    try
                    {
                        if (item.MacId.Length == 12 || item.MacId.Length == 16)
                        {
                            using (var sConn = new SqlConnection(_connStr))
                            {
                                sConn.Open();

                                var sCmd = new SqlCommand("sp_ReturnItemByMacId", sConn)
                                    {
                                        CommandType = CommandType.StoredProcedure
                                    };
                                sCmd.Parameters.Add("@MACID", SqlDbType.NVarChar);
                                sCmd.Parameters["@MACID"].Value = item.MacId;
                                sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                                sCmd.Parameters["@USERNAME"].Value = _userName;
                                using (IDataReader reader1 = sCmd.ExecuteReader())
                                {
                                    if (reader1.RecordsAffected < 1)
                                    {
                                        item.Success = false;
                                        item.ErrorMessage =
                                            "This Mac has already been returned, or does not exist on a delivery.";
                                    }
                                    else
                                        item.Success = true;
                                }
                            }
                        }
                        else
                        {
                            item.Success = false;
                            item.ErrorMessage =
                                string.Format("The Smart Code you entered is not the correct length ({0} characters). Must be 12, 16, 29 or 33 in Length.",
                                              item.SmartCode.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        item.Success = false;
                        item.ErrorMessage = "Error returning this item. Please Review MacId.";
                    }
                }
                else
                {
                    item.Success = false;
                    item.ErrorMessage = "Error returning this item. Please Review MacId.";
                }
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