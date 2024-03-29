﻿using Common;
using Persistence.Repositories.Interfaces;


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

        public IInventoryMasterMapper InventoryMasterMapper
        {
            get { return new InventoryMasterMapper(_sqlMapper); }
        }
    }
}
