using System.Collections.Generic;

namespace Persistence
{
    public interface ISqlMapperFactory
    {
        ITransaction BeginTransaction();
    }
}
