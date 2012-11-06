using Domain;

namespace Persistence.Repositories.Interfaces
{
    public interface IInventoryMasterMapper
    {
        Delivery GetDelivery(string deliveryNumber);
    }
}