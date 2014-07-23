﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using ApplicationSource.Interfaces;
using System.Configuration;
using ApplicationSource.Models;
using Domain;
using Persistence.Repositories.Interfaces;
using StructureMap;

namespace ApplicationSource.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [EnableCustomWebServiceHooks]
    public class VerifyUniqueMacService : IVerifyUniqueMacService
    {
        private readonly SqlConnection _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString);
        private IInventoryRepository _repo;

        public VerifyUniqueMacService(IInventoryRepository repo)
        {
            _repo = repo;
        }

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
                        var serialItem = _repo.SelectSmartMac(new SerialNumberItemQuery { MacId = parsedMacId, ProductGroup = productGroup });
                        if (serialItem != null)
                        {
                            model.IsUnique = false;
                            model.ErrorMessage = "This order exists on another order - #";
                            model.ErrorDeliveryNumber = serialItem.DocNum.ToString();

                        }
                        else
                        {
                            model.IsUnique = true;
                            if (!UpdateRecord(model.SerialCode, model.MacId, model.Id))
                            {
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
            var success = _repo.UpdateSerialNumberItem(new SerialNumberItem { Id = id, MacId = macId, SerialCode = serialCode, Username = HttpContext.Current.User.Identity.Name });
            return success;
        }
    }
}
