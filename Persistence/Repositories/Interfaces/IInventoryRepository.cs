using System.Collections;
using System.Collections.Generic;
using Domain;

namespace Persistence.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Delivery GetDelivery(DeliveryOrderQuery deliveryNumber);
        SerialNumberItem SelectSmartMac(SerialNumberItemQuery serialNumberItemQuery);
        bool UpdateSerialNumberItem(SerialNumberItem serialNumberItem);
        IEnumerable<SerialNumberItem> GetDeliveryItems(DeliveryOrderItemsQuery query);
        bool ClearDelivery(DeliveryOrderQuery deliveryOrderQuery);
        bool ReturnDeliveryLineItem(SerialNumberItem lineItem );
    }
}