using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DBSQLClient.Conexion;

/// <summary>
/// Interfaz que define los métodos para interactuar con SQL Server de forma asincrónica o sincrónica.
/// </summary>
public interface ISQLClientService
{
    /// <summary>
    /// Ejecuta una consulta SQL de texto de forma asíncrona.
    /// </summary>
    /// <param name="query">Consulta SQL a ejecutar.</param>
    /// <param name="parameters">Parámetros opcionales para la consulta.</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    /// <param name="timeout">Tiempo de espera en segundos (valor predeterminado 600).</param>
    /// <returns>Instancia de <see cref="SqlQueryResult"/> con las tablas obtenidas.</returns>
    Task<SqlQueryResult> QueryAsync(
        [Required] string query,
        SqlParameter[]? parameters = null,
        CancellationToken? cancellationToken = null,
        int? timeout = null);

    /// <summary>
    /// Ejecuta un procedimiento almacenado de forma asíncrona.
    /// </summary>
    /// <param name="storeProcedureName">Nombre del procedimiento almacenado.</param>
    /// <param name="parameters">Parámetros opcionales.</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    /// <param name="timeout">Tiempo de espera en segundos (valor predeterminado 600).</param>
    /// <returns>Instancia de <see cref="SqlQueryResult"/> con las tablas obtenidas.</returns>
    Task<SqlQueryResult> ExecuteAsync(
        [Required] string storeProcedureName,
        SqlParameter[]? parameters = null,
        CancellationToken? cancellationToken = null,
        int? timeout = null);

    /// <summary>
    /// Ejecuta una consulta SQL de texto de forma sincrónica.
    /// </summary>
    /// <param name="query">Consulta SQL a ejecutar.</param>
    /// <param name="parameters">Parámetros opcionales para la consulta.</param>
    /// <param name="timeout">Tiempo de espera en segundos (valor predeterminado 600).</param>
    /// <returns>Instancia de <see cref="SqlQueryResult"/> con las tablas obtenidas.</returns>
    SqlQueryResult Query(
        [Required] string query,
        SqlParameter[]? parameters = null,
        int? timeout = null);

    /// <summary>
    /// Ejecuta un procedimiento almacenado de forma sincrónica.
    /// </summary>
    /// <param name="storeProcedureName">Nombre del procedimiento almacenado.</param>
    /// <param name="parameters">Parámetros opcionales.</param>
    /// <param name="timeout">Tiempo de espera en segundos (valor predeterminado 600).</param>
    /// <returns>Instancia de <see cref="SqlQueryResult"/> con las tablas obtenidas.</returns>
    SqlQueryResult Execute(
        [Required] string storeProcedureName,
        SqlParameter[]? parameters = null,
        int? timeout = null);
}
