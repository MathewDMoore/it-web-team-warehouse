using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading;
using Common;
using Domain;

namespace Persistence
{
    public class SqlMapperWrapper : ISqlMapper
    {
        private readonly IBatisNet.DataMapper.ISqlMapper _sqlMapper;
        private readonly ILogger _log;

        public SqlMapperWrapper()
        {
            try
            {
                _sqlMapper = SqlConnectionFactory.GetInventoryRepository();
            }
            catch (Exception e)
            {
                _log.LogException(MethodBase.GetCurrentMethod().DeclaringType, OperationType.SQLWRAPPER, e, "");
            }
        }

        public T QueryForObject<T>(string statementName, object parameter)
        {
            int deadLockRetry = 5;
            while (deadLockRetry > 0)
            {
                try
                {
                    return _sqlMapper.QueryForObject<T>(statementName, parameter);
                }
                catch (SqlException ex)
                {
                    if (ex.ErrorCode != 1205 || --deadLockRetry == 0)
                        throw;
                }
                Thread.Sleep(5);
            }
            object obj = null;
            return (T)obj;
        }

        public object Insert(string statementName, object parameter)
        {
            int deadLockRetry = 5;
            while (deadLockRetry > 0)
            {
                try
                {
                    return _sqlMapper.Insert(statementName, parameter);
                }
                catch (SqlException ex)
                {
                    if (ex.ErrorCode != 1205 || --deadLockRetry == 0)
                        throw;
                }
                Thread.Sleep(5);
            }
            return 0;
        }

        public IList QueryForList(string statementName, object parameter)
        {
            int deadLockRetry = 5;
            while (deadLockRetry > 0)
            {
                try
                {
                    return _sqlMapper.QueryForList(statementName, parameter);
                }
                catch (SqlException ex)
                {
                    if (ex.ErrorCode != 1205 || --deadLockRetry == 0)
                        throw;
                }
                Thread.Sleep(5);
            }
            return new ArrayList();
        }

        public IList<object> QueryForGenericList(string statementName, object parameter, IList<object> genericList)
        {
            int deadLockRetry = 5;
            while (deadLockRetry > 0)
            {
                try
                {
                    _sqlMapper.QueryForList(statementName, parameter, genericList);
                    break;
                }
                catch (SqlException ex)
                {
                    if (ex.ErrorCode != 1205 || --deadLockRetry == 0)
                        throw;
                }
                Thread.Sleep(5);
            }
            return genericList;
        }

        public IList<T> QueryForList<T>(string statementName, object parameterObject)
        {
            int deadLockRetry = 5;
            while (deadLockRetry > 0)
            {
                try
                {
                    return _sqlMapper.QueryForList<T>(statementName, parameterObject);
                }
                catch (SqlException ex)
                {
                    if (ex.ErrorCode != 1205 || --deadLockRetry == 0)
                        throw;
                }
                Thread.Sleep(5);
            }
            return new List<T>();
        }

        public IList<T> QueryForList<T>(string statementName, object parameterObject, int skipResults, int maxResults)
        {
            int deadLockRetry = 5;
            while (deadLockRetry > 0)
            {
                try
                {
                    return _sqlMapper.QueryForList<T>(statementName, parameterObject, skipResults, maxResults);
                }
                catch (SqlException ex)
                {
                    if (ex.ErrorCode != 1205 || --deadLockRetry == 0)
                        throw;
                }
                Thread.Sleep(5);
            }
            return new List<T>();
        }

        public IEnumerable<T> QueryForList<T>(string statementName, object queryFilter, out int totalResults)
        {
            int deadLockRetry = 5;
            totalResults = 0;
            while (deadLockRetry > 0)
            {
                try
                {
                    var query = (QueryFilter)queryFilter;
                    var results = _sqlMapper.QueryForList<T>(statementName, queryFilter);
                    totalResults = results.Count;
                    return results.Skip(query.Skip).Take(query.ItemsPerPage);
                }
                catch (SqlException ex)
                {
                    if (ex.ErrorCode != 1205 || --deadLockRetry == 0)
                        throw;
                }
                Thread.Sleep(5);
            }
            return new List<T>();
        }

        public ITransaction BeginTransaction()
        {
            return new Transaction(_sqlMapper);
        }

        public IList QueryForList(string statementName, QueryFilter parameterObject, int skipResults, int maxResults)
        {
            int deadLockRetry = 5;
            while (deadLockRetry > 0)
            {
                try
                {
                    return _sqlMapper.QueryForList(statementName, parameterObject, skipResults, maxResults);
                }
                catch (SqlException ex)
                {
                    if (ex.ErrorCode != 1205 || --deadLockRetry == 0)
                        throw;
                }
                Thread.Sleep(5);
            }
            return new ArrayList();
        }

        public int Update(string statementName, object parameter)
        {
            int deadLockRetry = 5;
            while (deadLockRetry > 0)
            {
                try
                {
                    return _sqlMapper.Update(statementName, parameter);
                }
                catch (SqlException ex)
                {
                    if (ex.ErrorCode != 1205 || --deadLockRetry == 0)
                        throw;
                }
                Thread.Sleep(5);
            }
            return 0;
        }

        public int Delete(string statementName, object parameter)
        {
            int deadLockRetry = 5;
            while (deadLockRetry > 0)
            {
                try
                {
                    return _sqlMapper.Delete(statementName, parameter);
                }
                catch (SqlException ex)
                {
                    if (ex.ErrorCode != 1205 || --deadLockRetry == 0)
                        throw;
                }
                Thread.Sleep(5);
            }
            return 0;
        }
    }
}