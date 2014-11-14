﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using ApplicationSource.Interfaces;
using ApplicationSource.Models;
using Domain;
using Persistence.Repositories.Interfaces;

namespace ApplicationSource.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [EnableCustomWebServiceHooks]

    public class OrderDeliveryService : IOrderDeliveryService
    {
        private readonly IInventoryRepository _repo;
        private readonly ISettings _settings;
        private readonly IIdentity _identity;

        public OrderDeliveryService(IInventoryRepository repo, ISettings settings, IIdentity identity)
        {
            _repo = repo;
            _settings = settings;
            _identity = identity;
        }

        public OrderDeliveryModel OrderLookUp(MacDeliveryModel lookup)
        {
            var delivery = _repo.GetDelivery(new DeliveryOrderQuery { DocNum = lookup.DeliveryNumber, ServerLocation = _settings.GetServerLocation, IsInternal = lookup.IsInternal });
            OrderDeliveryModel deliveryModel = null;
            if (delivery != null)
            {
                delivery.IsIrDelivery = lookup.IsInternal;
                var items =
                    _repo.GetDeliveryItems(new DeliveryOrderItemsQuery
                    {
                        DocNum = lookup.DeliveryNumber,
                        ServerLocation = _settings.GetServerLocation,
                        Username = _identity.Name,
                        IsInternal = delivery.IsIrDelivery
                    }).ToList();
                deliveryModel = delivery.Map<Delivery, OrderDeliveryModel>();
                deliveryModel.DeliveryNumber = lookup.DeliveryNumber;
                deliveryModel.IsInternal = lookup.IsInternal;

                FindActiveKits(items, deliveryModel);

                items.ForEach(i =>
                {
                    var model = i.Map<SerialNumberItem, DeliveryOrderItemModel>();
                    if (string.IsNullOrEmpty(i.MacId))
                    {
                        deliveryModel.NotScannedItems.Add(model);
                    }
                    else
                    {
                        deliveryModel.ScannedItems.Add(model);
                    }
                });
                var dictionary = new Dictionary<string, int>();
                var grouped = items.Where(i => i.RealItemCode != null).GroupBy(i => i.RealItemCode).ToList();
                grouped.ForEach(g => dictionary.Add(g.Key, g.Count()));
                deliveryModel.ChartData = dictionary;
            }
            return deliveryModel;
        }

        private void FindActiveKits(List<SerialNumberItem> items, OrderDeliveryModel deliveryModel)
        {
            var kits = items.Where(x => x.KitId > 0 && x.KitCounter > 0 && !string.IsNullOrEmpty(x.ScannedBy)).GroupBy(x => x.ScannedBy);
            var kitGroups = new Dictionary<string, List<DeliveryOrderItemModel>>();
            kits.ToList().ForEach(group =>
            {
                var matched = group.FirstOrDefault();
                var kitItems = items.Where(i => i.KitCounter.Equals(matched.KitCounter) && i.KitId.Equals(matched.KitId));
                if (!kitGroups.ContainsKey(group.Key))
                {
                    kitGroups.Add(group.Key, kitItems.Map<IEnumerable<SerialNumberItem>, List<DeliveryOrderItemModel>>());
                }
                else
                {
                    kitGroups[group.Key].AddRange(kitItems.Map<IEnumerable<SerialNumberItem>, List<DeliveryOrderItemModel>>());
                }
                if (kitGroups[group.Key].All(i => !string.IsNullOrEmpty(i.ScannedBy)))
                {
                    kitGroups.Remove(group.Key);
                }
                else
                {
                    kitItems.ToList().ForEach(ki => items.Remove(ki));
                }
            });
            deliveryModel.ActiveKits = kitGroups;
        }

        public MacDeliveryModel LocateMacIds(string macId)
        {
            var result = new MacDeliveryModel();
            if (!string.IsNullOrEmpty(macId))
            {
                var modifiedMac = macId.Length >= 29 ? macId.Remove(macId.Length - 17, 17) : macId;
                if (modifiedMac.Length == 12 || modifiedMac.Length == 16)
                {
                    var delivery = _repo.GetDeliveryByMacId(modifiedMac);
                    if (delivery != null)
                    {
                        result = new MacDeliveryModel
                        {
                            DeliveryNumber = delivery.DeliveryNumber,
                            IsInternal = delivery.IsIrDelivery
                        };
                    }
                    else
                    {
                        result.Error = "Delivery not found for this Smart Mac.";
                    }
                }
                else
                {
                    result.Error = " Incorrect Mac Id Length, or Mac Id is not a serialized number.";
                }
            }
            return result;
        }



        public VerifyUniqueMacModel SaveDeliveryItem(VerifyUniqueMacModel model)
        {
            var macId = model.MacId.Trim();
            macId = macId.Replace(" ", "");
            var productGroup = model.ProductGroup;

            if (!string.IsNullOrEmpty(model.MacId) || !string.IsNullOrEmpty(productGroup))
            {
                var parsedMacId = macId.Length >= 29 ? macId.Remove(macId.Length - 17, 17) : macId;
                try
                {
                    if (parsedMacId.Length == 12 || parsedMacId.Length == 16 || !model.IsUnique)
                    {

                        SerialNumberItem serialItem = null;

                        if (model.IsUnique)
                        {
                            serialItem = _repo.SelectSmartMac(new SerialNumberItemQuery
                           {
                               MacId = parsedMacId,
                               ProductGroup = productGroup
                           });

                            if (serialItem != null)
                            {
                                model.ErrorMessage = "This item has been scanned on another delivery order - #";
                                model.ErrorDeliveryNumber = serialItem.DocNum;
                                return model;
                            }

                        }
                        model.AlreadyScanned = _repo.IsScanned(new SerialNumberItem { SerialNum = model.SerialNum, DocNum = model.DocNum });

                        if (!model.AlreadyScanned && !UpdateRecord(model.SerialCode, parsedMacId, model.Id, model.IsInternal, model.SerialNum, model.DocNum))
                        {
                            model.ErrorMessage =
                                "There was an error saving this item into the database. Please review the SerialCode or contact IT support.";
                        }
                    }

                }
                catch (Exception e)
                {
                    model.IsUnique = false;
                    model.ErrorMessage = e.Message;
                }
            }
            else
            {
                model.ErrorMessage =
                               "There was an error saving this item into the database. Please review the SerialCode or contact IT support.";
            }
            return model;
        }

        public bool ReturnDelivery(ClearDeliveryModel delivery)
        {
            if (delivery.DeliveryNumber > 0)
            {
                bool success = false;

                try
                {
                    success = _repo.ReturnDelivery(new DeliveryOrderQuery { DocNum = delivery.DeliveryNumber, IsInternal = delivery.IsInternal, Username = _identity.Name });
                }
                catch (Exception)
                {

                    success = false;
                }

                return success;
            }

            return false;
        }

        public bool ReturnDeliveryLineItem(ReturnModel returns)
        {
            var success = false;
            try
            {
                returns.SelectedList.ForEach(i => _repo.ReturnDeliveryLineItem(new SerialNumberItem { Id = i.Id, DocNum = returns.DeliveryNumber, SerialNum = i.SerialNum, Username = _identity.Name }, returns.IsInternal));
                success = true;
            }
            catch (Exception e)
            {
                success = false;
            }
            return success;
        }

        public bool ClearDelivery(ClearDeliveryModel delivery)
        {
            if (delivery.DeliveryNumber > 0)
            {
                bool success = false;

                try
                {
                    success = _repo.ClearDelivery(new DeliveryOrderQuery { DocNum = delivery.DeliveryNumber, IsInternal = delivery.IsInternal, Username = _identity.Name });
                }
                catch (Exception)
                {

                    success = false;
                }

                return success;
            }

            return false;
        }

        public bool VerifyDelivery(int deliveryNumber)
        {
            //Old "sp_Delivery_Verify"
            var query = new DeliveryOrderQuery() { DocNum = deliveryNumber, Username = _identity.Name };
            return _repo.VerifyDelivery(query);
        }

        private bool UpdateRecord(string serialCode, string macId, int id, bool isInternal, int serialNum, int docNum)
        {
            var success = _repo.UpdateSerialNumberItem(new SerialNumberItem { Id = id, MacId = macId, SerialCode = serialCode, ScannedBy = HttpContext.Current.User.Identity.Name, SerialNum = serialNum, DocNum = docNum }, isInternal);
            return success;
        }
    }
}