using IBatisNet.DataMapper;

namespace Persistence
{
    public class Transaction : ITransaction
    {
        private readonly IBatisNet.DataMapper.ISqlMapper _sqlMapper;
        private readonly ISqlMapSession _sqlMapSession;
        private bool _complete;

        public Transaction(IBatisNet.DataMapper.ISqlMapper sqlMapper)
        {
            _sqlMapper = sqlMapper;
            _sqlMapSession = _sqlMapper.BeginTransaction();
        }

        public void Commit()
        {
            _sqlMapSession.CommitTransaction();
            _complete = true;
        }

        public void Rollback()
        {
            _sqlMapSession.RollBackTransaction();
            _complete = true;
        }

        public void Dispose()
        {
            if (!_complete)
            {
                Rollback();
            }
        }
    }
}