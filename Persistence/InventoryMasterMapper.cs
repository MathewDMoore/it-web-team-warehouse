﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Delivery GetDelivery(string deliveryNumber)
        {
            return _sqlMapper.QueryForObject<Delivery>(SELECT_DELIVERY_BY_DELIVERY_NUMBER, deliveryNumber);
        }

          public InventoryMasterMapper(ISqlMapper sqlMapper)
        {
            _sqlMapper = sqlMapper;
        }
    }
}
