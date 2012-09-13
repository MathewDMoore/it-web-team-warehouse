using System;
using IBatisNet.DataMapper;
using log4net.Core;

namespace Persistence
{
    public class SqlMapperFactory : ISqlMapperFactory
    {
        private readonly ISqlMapper _sqlMapper;
        private readonly ILogger _log;

        public SqlMapperFactory(ISqlMapper sqlMapper, ILogger log)
        {
            _sqlMapper = sqlMapper;
            _log = log;
        }


        public ITransaction BeginTransaction()
        {
            return _sqlMapper.BeginTransaction();
        }
    }
}
