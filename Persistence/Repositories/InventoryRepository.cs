using System;
using System.Collections.Generic;
using Domain;
using IBatisNet.DataMapper.Exceptions;
using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ISqlMapperFactory _sqlMapperFactory;
        private ITransaction _transactionInProcess = null;

        public InventoryRepository(ISqlMapperFactory sqlMapperFactory)
        {
            _sqlMapperFactory = sqlMapperFactory;
        }

        public ITransaction BeginTransaction()
        {
            try
            {
                if (_transactionInProcess == null)
                {
                    _transactionInProcess = _sqlMapperFactory.BeginTransaction();
                }
            }
            catch (DataMapperException dex)
            {
                // We already started a transaction
            }
            return _transactionInProcess;
        }

        public void CommitTransaction()
        {
            if (_transactionInProcess != null)
            {
                _transactionInProcess.Commit();
                _transactionInProcess = null;
            }
            else
            {
                throw new InvalidOperationException("There is no transaction in process");
            }
        }

        public void RollbackTransaction()
        {
            if (_transactionInProcess != null)
            {
                _transactionInProcess.Rollback();
                _transactionInProcess = null;
            }
            else
            {
                throw new InvalidOperationException("There is no transaction in process");
            }
        }


        public Delivery GetDelivery(DeliveryOrderQuery deliveryNumber)
        {
            return _sqlMapperFactory.InventoryMasterMapper.GetDelivery(deliveryNumber);
        }

        public SerialNumberItem SelectSmartMac(SerialNumberItemQuery serialNumberItemQuery)
        {
            return _sqlMapperFactory.InventoryMasterMapper.SelectSmartMac(serialNumberItemQuery);
        }

        public bool UpdateSerialNumberItem(SerialNumberItem serialNumberItem)
        {
            return _sqlMapperFactory.InventoryMasterMapper.UpdateSerialNumberItem(serialNumberItem);
        }

        public IEnumerable<SerialNumberItem> GetDeliveryItems(DeliveryOrderItemsQuery query)
        {
            return _sqlMapperFactory.InventoryMasterMapper.SelectDeliveryOrderItems(query);
        }
    }
}