﻿using Domain;

namespace Persistence.Repositories.Interfaces
{
    public interface IInventoryMasterMapper
    {
        Delivery GetDelivery(string deliveryNumber);
        SerialNumberItem SelectSmartMac(SerialNumberItemQuery serialNumberItemQuery);
        bool UpdateSerialNumberItem(SerialNumberItem serialNumberItem);
    }
}