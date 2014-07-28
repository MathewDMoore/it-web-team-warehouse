using System.Collections.Generic;
using Domain;

namespace Persistence.Repositories.Interfaces
{
    public interface IInventoryMasterMapper
    {
        Delivery GetDelivery(DeliveryOrderQuery deliveryNumber);
        SerialNumberItem SelectSmartMac(SerialNumberItemQuery serialNumberItemQuery);
        bool UpdateSerialNumberItem(SerialNumberItem serialNumberItem);
        IEnumerable<SerialNumberItem> SelectDeliveryOrderItems(DeliveryOrderItemsQuery query);
        bool ClearDelivery(DeliveryOrderQuery query);
    }
}