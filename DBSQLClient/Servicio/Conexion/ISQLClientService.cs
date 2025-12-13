using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
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
        /// Ejecuta de forma asincrona la consulta SQL especificada y devuelve el resultado como un objeto <see cref="SqlQueryResult"/>.
        /// </summary>
        /// <remarks>Si la consulta es de larga duración o puede cancelarse, proporcione un <paramref
        /// name="cancellationToken"/> para admitir la cancelación cooperativa. El comportamiento del método cuando se produce un tiempo de espera
        /// depende del proveedor de base de datos subyacente.</remarks>
        /// <param name="query">La consulta SQL que se va a ejecutar. No puede ser nulo ni estar vacío.
        /// <param name="parameters">Una matriz de objetos <see cref="SqlParameter"/> que se utilizarán como parámetros para la consulta, o <see langword="null"/>
        /// si la consulta no requiere parámetros.
        /// <param name="cancellationToken">Un <see cref="CancellationToken"/> que se puede utilizar para cancelar la operación de consulta, o <see langword="null"/>
        /// para ejecutar sin soporte de cancelación.</param>
        /// <param name="timeout">El tiempo máximo, en segundos, que se puede esperar a que se complete la consulta antes de que se agote el tiempo de espera, o <see langword="null"/>
        /// para utilizar el tiempo de espera predeterminado.</param>
        /// <returns>Un <see cref="SqlQueryResult"/> que contiene los resultados de la consulta ejecutada.</returns>
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



        /// <summary>
        /// Ejecuta la consulta SQL especificada y devuelve el resultado como un objeto <see cref="SqlQueryResult"/>.
        /// </summary>
        /// <remarks>Si la consulta es de larga duración o puede cancelarse, proporcione un <paramref
        /// name="cancellationToken"/> para admitir la cancelación cooperativa. El comportamiento del método cuando se produce un tiempo de espera
        /// depende del proveedor de base de datos subyacente.</remarks>
        /// <param name="query">La consulta SQL que se va a ejecutar. No puede ser nulo ni estar vacío.
        /// <param name="parameters">Una matriz de objetos <see cref="SqlParameter"/> que se utilizarán como parámetros para la consulta, o <see langword="null"/>
        /// si la consulta no requiere parámetros.
        /// <param name="timeout">El tiempo máximo, en segundos, que se puede esperar a que se complete la consulta antes de que se agote el tiempo de espera, o <see langword="null"/>
        /// para utilizar el tiempo de espera predeterminado.</param>
        /// <returns>Un <see cref="SqlQueryResult"/> que contiene los resultados de la consulta ejecutada.</returns>
        SqlQueryResult Query(
            [Required] string query,
            SqlParameter[]? parameters = null,
            int? timeout = null
        );



        /// <summary>
        ///  
        /// </summary>
        /// <param name="storeProcedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        SqlQueryResult Execute(
            [Required] string storeProcedureName,
            SqlParameter[]? parameters = null,
            int? timeout = null
         );


    }


    #endregion

}
