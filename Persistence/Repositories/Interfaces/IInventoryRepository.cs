using System.Collections;
using System.Collections.Generic;
using Domain;

namespace Persistence.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Delivery GetDelivery(DeliveryOrderQuery deliveryNumber);
        SerialNumberItem SelectSmartMac(SerialNumberItemQuery serialNumberItemQuery);
        bool UpdateSerialNumberItem(SerialNumberItem serialNumberItem, bool isInternal);
        IEnumerable<SerialNumberItem> GetDeliveryItems(DeliveryOrderItemsQuery query);
        bool ClearDelivery(DeliveryOrderQuery deliveryOrderQuery);
        bool ReturnDeliveryLineItem(SerialNumberItem lineItem, bool isInternal );
        bool VerifyDelivery(DeliveryOrderQuery query);
        Delivery GetDeliveryByMacId(string macId);
        bool ReturnDelivery(DeliveryOrderQuery query);
        bool IsScanned(SerialNumberItem item);
        IList<SerialNumberItem> FindUnScannedMatch(SerialNumberItemQuery matches);
    }
}