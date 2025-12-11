using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient;

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
        /// <param name="query">The SQL query string to execute. Must be a valid SQL statement supported by the underlying database.</param>
        /// <param name="parameters">An array of parameters to be applied to the SQL query, or <see langword="null"/> if the query does not
        /// require parameters.</param>
        /// <param name="cancellationToken">An optional cancellation token that can be used to cancel the query operation.</param>
        /// <param name="timeout"></param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="SqlQueryResult"/>
        /// with the data returned by the query.</returns>
        Task<SqlQueryResult> QueryAsync(
            [Required]string query, 
            SqlParameter[]? parameters = null, 
            CancellationToken? cancellationToken = null, 
            int? timeout = null
       );

        /// <summary>
        /// Executes the specified SQL query asynchronously and returns the result.
        /// </summary>
        /// <param name="storeProcedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<SqlQueryResult> ExecuteAsync(
            [Required] string storeProcedureName, 
            SqlParameter[]? parameters = null, 
            CancellationToken? cancellationToken = null, 
            int? timeout = null 
         );







    }


    #endregion

}
