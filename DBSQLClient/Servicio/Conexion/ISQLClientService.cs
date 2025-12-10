using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DBSQLClient.Servicio.Conexion
{
    /// <summary>
    /// Interfaz que define los métodos para interactuar con SQL Server de forma asíncrona.
    /// </summary>
    /// 
     #region Interfaces
    public interface ISQLClientService
    {
        /// <summary>
        /// Executes the specified SQL query asynchronously and returns the result.
        /// </summary>
        /// <param name="query">The SQL query string to execute. Must be a valid SQL statement.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="SqlQueryResult"/>
        /// representing the outcome of the query.</returns>
        Task<SqlQueryResult> QueryAsync(string query);

        /// <summary>
        /// Executes the specified SQL query asynchronously and returns the result.
        /// </summary>
        /// <param name="query">The SQL query string to execute. Must be a valid SQL statement supported by the underlying database.</param>
        /// <param name="parameters">An array of parameters to be applied to the SQL query, or null if the query does not require parameters.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="SqlQueryResult"/>
        /// with the data returned by the query.</returns>
        Task<SqlQueryResult> QueryAsync(string query, SqlParameters[]? parameters);

        /// <summary>
        /// Executes the specified SQL query asynchronously and returns the result.
        /// </summary>
        /// <param name="query">The SQL query string to execute. Must be a valid SQL statement supported by the underlying database.</param>
        /// <param name="parameters">An array of parameters to be applied to the SQL query, or <see langword="null"/> if the query does not
        /// require parameters.</param>
        /// <param name="cancellationToken">An optional cancellation token that can be used to cancel the query operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="SqlQueryResult"/>
        /// with the data returned by the query.</returns>
        Task<SqlQueryResult> QueryAsync(string query, SqlParameters[]? parameters, CancellationToken? cancellationToken);


        /// <summary>
        /// Executes the specified SQL query asynchronously and returns the result.
        /// </summary>
        /// <param name="query">The SQL query string to execute. Must be a valid SQL statement.</param>
        /// <param name="parameters">An array of parameters to be applied to the query, or <see langword="null"/> if the query does not require
        /// parameters.</param>
        /// <param name="timeout">The maximum duration, in seconds, to wait for the query to complete, or <see langword="null"/> to use the
        /// default timeout.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="SqlQueryResult"/>
        /// with the query results.</returns>
        Task<SqlQueryResult> QueryAsync(string query, SqlParameters[]? parameters, int? timeout);

        /// <summary>
        /// Executes the specified stored procedure asynchronously and returns the result.
        /// </summary>
        /// <param name="storeProcedureName">The name of the stored procedure to execute. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="SqlQueryResult"/>
        /// with the data returned by the stored procedure.</returns>
        Task<SqlQueryResult> ExecuteAsync(string storeProcedureName);

        
        Task<SqlQueryResult> ExecuteAsync(string storeProcedureName, SqlParameters[] parameters);
        
        Task<SqlQueryResult> ExecuteAsync(string storeProcedureName, SqlParameters[] parameters, CancellationToken cancellationToken);
        Task<SqlQueryResult> ExecuteAsync(string storeProcedureName, SqlParameters[] parameters, int timeout);



    }


    #endregion

}
