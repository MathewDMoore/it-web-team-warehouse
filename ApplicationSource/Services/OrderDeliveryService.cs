using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using ApplicationSource.Interfaces;
using ApplicationSource.Models;
using Domain;
using Persistence.Repositories.Interfaces;
using StructureMap.Diagnostics;

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
                grouped.OrderBy(g => g.Count()).ToList().ForEach(g => dictionary.Add(g.Key, g.Count()));
                deliveryModel.ChartData = dictionary;
            }
            return deliveryModel;
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

        public SaveScanModel MatchAndSave(MatchModel scanModel)
        {
            var model = new SaveScanModel();
            if (!string.IsNullOrEmpty(scanModel.SerialCode) && scanModel.SerialCode.Length >= 7)
            {
                var productId = scanModel.SerialCode.Substring(scanModel.SerialCode.Length - 7, 5);
                var color = scanModel.SerialCode.Substring(scanModel.SerialCode.Length - 2, 2);
                var matches =
                    _repo.FindUnScannedMatch(new SerialNumberItemQuery
                    {
                        ProductId = productId,
                        Color = color,
                        DocNum = scanModel.DocNumber
                    });
                //Verify there are matches
                if (matches.Any())
                {
                    //check if match requires smart code
                    //All Singles

                    var updated = false;
                    var matchIndex = 0;
                    SerialNumberItem match = null;
                    if (scanModel.KitId > 0)
                    {
                        match =
                            matches.FirstOrDefault(
                                m => m.KitCounter.Equals(scanModel.KitCounter) && m.KitId.Equals(scanModel.KitId));
                        if (match != null)
                        {
                            match.ScannedBy = _identity.Name;
                            match.SerialCode = scanModel.SerialCode;
                            var error = string.Empty;
                            SmartMacCheck(match, out error);
                            if (string.IsNullOrEmpty(error))
                            {
                                _repo.UpdateSerialNumberItem(match, scanModel.IsInternal);
                            }
                            else
                            {
                                model.ErrorMessage = error;
                            }
                        }
                        else
                        {
                            model.ErrorMessage =
                                "No matches found in kit. You may have scanned the bar code incorrectly";
                        }
                    }
                    else
                    {
                        var list = matches.All(m => m.KitId == 0) || matches.All(m => m.KitId > 0)
                            ? matches
                            : matches.Where(m => m.KitId.Equals(0)).ToList();
                        while (!updated && matchIndex <= list.Count - 1)
                        {
                            var item = list[matchIndex];
                            item.ScannedBy = _identity.Name;
                            item.SerialCode = scanModel.SerialCode;
                            var error = string.Empty;
                            SmartMacCheck(item, out error);
                            if (string.IsNullOrEmpty(error))
                            {
                                updated = _repo.UpdateSerialNumberItem(item, scanModel.IsInternal);
                                if (!updated)
                                {
                                    matchIndex++;
                                }
                                else
                                {
                                    match = list[matchIndex];
                                    break;
                                }
                            }
                            else
                            {
                                model.ErrorMessage = error;
                                break;
                            }
                        }
                    }


                    if (match == null && string.IsNullOrEmpty(model.ErrorMessage))
                    {
                        model.ErrorMessage = "All matches already scanned";
                    }
                    else
                    {
                        model.UpdatedItem = match.Map<SerialNumberItem, DeliveryOrderItemModel>();
                    }
                }
                else
                {
                    model.ErrorMessage = "No items found that match that MacId";
                }
            }
            else
            {
                model.ErrorMessage = "Scanned serial code invalid, length is too short";
            }
            //return unique row number, js will need to find this and remove to proper table on view
            return model;
        }

        public bool UpdateScanByUser(UpdateUserNameModel updateModel)
        {
            var successful = _repo.UpdateScanByUser(new UpdateUserNameQuery() { UserName = _identity.Name, DocNum = updateModel.DocNum, SerialNum = updateModel.SerialNum });

            return successful;
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
                if (kitGroups[group.Key].All(i => !string.IsNullOrEmpty(i.SerialCode)))
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
        private void SmartMacCheck(SerialNumberItem item, out string errorMessage)
        {
            item.MacId = item.SerialCode.Length >= 29 ? item.SerialCode.Remove(item.SerialCode.Length - 17, 17) : item.SerialCode;

            //Check if its unique
            var isUnique = !item.SmartCodeOnly && !item.NoSerialization;
            //Check if its repeatable
            var isRepeatable = item.SmartCodeOnly && !item.NoSerialization;
            //If its repeatable, allow it.
            if (!isRepeatable)
            {
                if (item.MacId.Length == 12 || item.MacId.Length == 16 || !isUnique)
                {

                    SerialNumberItem serialItem = null;

                    if (isUnique)
                    {
                        serialItem = _repo.SelectSmartMac(new SerialNumberItemQuery
                        {
                            MacId = item.MacId,
                            ProductGroup = item.ProductGroup
                        });

                        if (serialItem != null)
                        {
                            errorMessage = "This item has been scanned on another delivery order - #" +
                                           serialItem.DocNum;
                            return;
                        }
                    }
                }
                else
                {
                    errorMessage = "Mac is the wrong size. Please enter one the correct length.";
                    return;
                }
            }
            errorMessage = null;
        }
    }
}