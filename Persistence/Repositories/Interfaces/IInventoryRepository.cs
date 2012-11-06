using Domain;

namespace Persistence.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Delivery GetDelivery(string deliveryNumber);
    }
}