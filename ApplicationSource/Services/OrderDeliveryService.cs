using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Activation;
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
    }
}