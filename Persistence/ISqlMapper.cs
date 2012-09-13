using System;
using System.Collections;
using System.Collections.Generic;

namespace Persistence
{
    public interface ISqlMapper
    {
        int Update(string statementName, object parameter);
        int Delete(string statementName, object parameter);

        T QueryForObject<T>(string statementName, object parameter);
        object Insert(string statementName, object parameter);

        /// <summary>
        /// Executes a Sql SELECT statement that returns data to populate
        /// a number of result objects.
        /// <p/>
        ///  The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        IList<T> QueryForList<T>(string statementName, object parameterObject);

        /// <summary>
        /// Executes the SQL and retuns all rows selected.
        /// <p/>
        ///  The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="skipResults">The number of rows to skip over.</param>
        /// <param name="maxResults">The maximum number of rows to return.</param>
        /// <returns>A List of result objects.</returns>
        IList<T> QueryForList<T>(string statementName, object parameterObject, int skipResults, int maxResults);

        ITransaction BeginTransaction();
        IEnumerable<T> QueryForList<T>(string statementName, object queryFilter, out int totalResults);
    }
}