using Persistence.Repositories.Interfaces;

namespace Persistence
{
    public interface ISqlMapperFactory
    {
        ITransaction BeginTransaction();
        IInventoryMasterMapper InventoryMasterMapper { get; }
    }
}
