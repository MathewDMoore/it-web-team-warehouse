using System.Collections.Generic;
using Domain;

namespace Persistence.Repositories.Interfaces
{
    public interface IInventoryMasterMapper
    {
        Delivery GetDelivery(DeliveryOrderQuery deliveryNumber);
        SerialNumberItem SelectSmartMac(SerialNumberItemQuery serialNumberItemQuery);
        bool UpdateSerialNumberItem(SerialNumberItem serialNumberItem, bool isInternal);
        IEnumerable<SerialNumberItem> SelectDeliveryOrderItems(DeliveryOrderItemsQuery query);
        bool ClearDelivery(DeliveryOrderQuery query);
        bool ReturnDeliveryLineItem(SerialNumberItem lineItem, bool isInternal);
        bool VerifyDelivery(DeliveryOrderQuery query);
        Delivery GetDeliveryByMacId(string macId);
        bool ReturnDelivery(DeliveryOrderQuery query);
        bool IsScanned(SerialNumberItem item);
        IList<SerialNumberItem> FindUnScannedMatch(SerialNumberItemQuery query);
    }
}