using System.Collections.Generic;
using Common;
using Domain;
using Persistence.Repositories.Interfaces;

namespace Persistence
{
    class InventoryMasterMapper : IInventoryMasterMapper
    {
        private readonly ISqlMapper _sqlMapper;
        private readonly ILogger _log;

        private const string SELECT_DELIVERY_BY_DELIVERY_NUMBER = "SelectDeliveryByDeliveryNumber";
        private const string SELECT_INTERNAL_DELIVERY_BY_DELIVERY_NUMBER = "SelectDeliveryByDeliveryNumberIR";

        public Delivery GetDelivery(DeliveryOrderQuery deliveryNumber)
        {
            return _sqlMapper.QueryForObject<Delivery>(deliveryNumber.IsInternal ? SELECT_INTERNAL_DELIVERY_BY_DELIVERY_NUMBER : SELECT_DELIVERY_BY_DELIVERY_NUMBER, deliveryNumber);
        }

        public SerialNumberItem SelectSmartMac(SerialNumberItemQuery serialNumberItemQuery)
        {
            return _sqlMapper.QueryForObject<SerialNumberItem>("SelectSmartMac", serialNumberItemQuery);
        }

        public bool UpdateSerialNumberItem(SerialNumberItem serialNumberItem, bool isInternal)
        {
            return _sqlMapper.Update(isInternal ?"UpdateSerialNumberIR": "UpdateSerialNumber",serialNumberItem)>0;
        }

        public IEnumerable<SerialNumberItem> SelectDeliveryOrderItems(DeliveryOrderItemsQuery query)
        {
            return _sqlMapper.QueryForList<SerialNumberItem>(query.IsInternal ? "SelectDeleiveryOrderItemsIR":"SelectDeleiveryOrderItems", query);
        }

        public bool ClearDelivery(DeliveryOrderQuery query)
        {
            return _sqlMapper.Update(query.IsInternal ? "sp_ClearDeliveryIR" : "sp_ClearDelivery", query) > 0;
        }

        public bool ReturnDeliveryLineItem(SerialNumberItem lineItem, bool isInternal)
        {
            return _sqlMapper.Update(isInternal ? "ReturnDeliveryByLineItemIR" : "ReturnDeliveryByLineItem", lineItem) > 0;
        }

        public bool VerifyDelivery(DeliveryOrderQuery query)
        {
          return _sqlMapper.Update(query.IsInternal ?"UpdateVerifiedOrderIR": "UpdateVerifiedOrder", query) > 0;
        }

        public Delivery GetDeliveryByMacId(string macId)
        {
            return _sqlMapper.QueryForObject<Delivery>("GetDeliveryByMacId", macId);
        }

        public bool ReturnDelivery(DeliveryOrderQuery query)
        {
            return _sqlMapper.Update(query.IsInternal ? "ReturnDeliveryIR" : "ReturnDelivery", query) > 0;
        }

        public bool IsScanned(SerialNumberItem item)
        {
            return _sqlMapper.QueryForObject<SerialNumberItem>("IsScanned", item) != null;
        }

        public IList<SerialNumberItem> FindUnScannedMatch(SerialNumberItemQuery query)
        {
            return _sqlMapper.QueryForList<SerialNumberItem>("FindUnScannedMatches", query);
        }

        public InventoryMasterMapper(ISqlMapper sqlMapper)
        {
            _sqlMapper = sqlMapper;
        }
    }
}
