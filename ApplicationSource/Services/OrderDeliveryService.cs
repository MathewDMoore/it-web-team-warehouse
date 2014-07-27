using System;
using System.Linq;
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

        public OrderDeliveryModel OrderLookUp(int orderId)
        {
            var delivery = _repo.GetDelivery(new DeliveryOrderQuery { DocNum = orderId, ServerLocation = _settings.GetServerLocation });
            var items =
                _repo.GetDeliveryItems(new DeliveryOrderItemsQuery
                {
                    DocNum = orderId,
                    ServerLocation = _settings.GetServerLocation,
                    Username = _identity.Name
                });
            var deliveryModel = delivery.Map<Delivery, OrderDeliveryModel>();
            items.ToList().ForEach(i =>
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
            return deliveryModel;
            
        }
        public VerifyUniqueMacModel SaveDeliveryItem(VerifyUniqueMacModel model)
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