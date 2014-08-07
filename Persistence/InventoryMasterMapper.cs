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

        public Delivery GetDelivery(DeliveryOrderQuery deliveryNumber)
        {
            return _sqlMapper.QueryForObject<Delivery>(SELECT_DELIVERY_BY_DELIVERY_NUMBER, deliveryNumber);
        }

        public SerialNumberItem SelectSmartMac(SerialNumberItemQuery serialNumberItemQuery)
        {
            return _sqlMapper.QueryForObject<SerialNumberItem>("SelectSmartMac", serialNumberItemQuery);
        }

        public bool UpdateSerialNumberItem(SerialNumberItem serialNumberItem)
        {
            return _sqlMapper.Update("UpdateSerialNumber",serialNumberItem)>0;
        }

        public IEnumerable<SerialNumberItem> SelectDeliveryOrderItems(DeliveryOrderItemsQuery query)
        {
            return _sqlMapper.QueryForList<SerialNumberItem>("SelectDeleiveryOrderItems", query);
        }

        public bool ClearDelivery(DeliveryOrderQuery query)
        {
            return _sqlMapper.Update("sp_ClearDelivery", query) > 0;
        }

        public bool ReturnDeliveryLineItem(SerialNumberItem lineItem)
        {
            return _sqlMapper.Update("ReturnDeliveryByLineItem", lineItem) >0;
        }

        public bool VerifyDelivery(DeliveryOrderQuery query)
        {
          return _sqlMapper.Update("UpdateVerifiedOrder", query) > 0;
        }

        public Delivery GetDeliveryByMacId(string macId)
        {
            return _sqlMapper.QueryForObject<Delivery>("GetDeliveryByMacId", macId);
        }

        public InventoryMasterMapper(ISqlMapper sqlMapper)
        {
            _sqlMapper = sqlMapper;
        }
    }
}
