using System;

namespace Persistence
{
    public interface ITransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}