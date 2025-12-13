using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DBSQLClient.Conexion;

/// <summary>
/// Proporciona métodos para ejecutar consultas SQL y procedimientos almacenados de forma asíncrona o sincrónica.
/// </summary>
public sealed class SqlClientService : ISQLClientService
{
    private const int DefaultTimeout = 600;

    private readonly ISqlCommandExecutor _commandExecutor;

    /// <summary>
    /// Inicializa el servicio usando una cadena de conexión de SQL Server.
    /// </summary>
    /// <param name="connectionString">Cadena de conexión usada para crear nuevas conexiones.</param>
    public SqlClientService(string connectionString)
        : this(new SqlConnectionFactory(connectionString))
    {
    }

    /// <summary>
    /// Inicializa el servicio usando una factoría de conexiones y un ejecutor de comandos opcional.
    /// </summary>
    /// <param name="connectionFactory">Factoría encargada de crear conexiones SQL.</param>
    /// <param name="commandExecutor">Ejecutor de comandos personalizado (opcional).</param>
    public SqlClientService(ISqlConnectionFactory connectionFactory, ISqlCommandExecutor? commandExecutor = null)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _commandExecutor = commandExecutor ?? new SqlCommandExecutor(connectionFactory);
    }

    /// <summary>
    /// Ejecuta de forma asíncrona una consulta SQL de texto.
    /// </summary>
    /// <param name="query">Consulta SQL a ejecutar.</param>
    /// <param name="parameters">Parámetros opcionales para la consulta.</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    /// <param name="timeout">Tiempo de espera en segundos (valor predeterminado 600).</param>
    /// <returns>Resultado encapsulado en <see cref="SqlQueryResult"/>.</returns>
    public Task<SqlQueryResult> QueryAsync(
        [Required] string query,
        SqlParameter[]? parameters = null,
        CancellationToken? cancellationToken = null,
        int? timeout = null)
    {
        var request = BuildRequest(query, CommandType.Text, parameters, cancellationToken, timeout);
        return ExecuteAsync(request);
    }

    /// <summary>
    /// Ejecuta de forma asíncrona un procedimiento almacenado.
    /// </summary>
    /// <param name="storeProcedureName">Nombre del procedimiento almacenado.</param>
    /// <param name="parameters">Parámetros opcionales para el procedimiento.</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    /// <param name="timeout">Tiempo de espera en segundos (valor predeterminado 600).</param>
    /// <returns>Resultado encapsulado en <see cref="SqlQueryResult"/>.</returns>
    public Task<SqlQueryResult> ExecuteAsync(
        [Required] string storeProcedureName,
        SqlParameter[]? parameters = null,
        CancellationToken? cancellationToken = null,
        int? timeout = null)
    {
        var request = BuildRequest(storeProcedureName, CommandType.StoredProcedure, parameters, cancellationToken, timeout);
        return ExecuteAsync(request);
    }

    /// <summary>
    /// Ejecuta de forma sincrónica una consulta SQL de texto.
    /// </summary>
    /// <param name="query">Consulta SQL a ejecutar.</param>
    /// <param name="parameters">Parámetros opcionales para la consulta.</param>
    /// <param name="timeout">Tiempo de espera en segundos (valor predeterminado 600).</param>
    /// <returns>Resultado encapsulado en <see cref="SqlQueryResult"/>.</returns>
    public SqlQueryResult Query(
        [Required] string query,
        SqlParameter[]? parameters = null,
        int? timeout = null)
    {
        var request = BuildRequest(query, CommandType.Text, parameters, cancellationToken: null, timeout);
        return Execute(request);
    }

    /// <summary>
    /// Ejecuta de forma sincrónica un procedimiento almacenado.
    /// </summary>
    /// <param name="storeProcedureName">Nombre del procedimiento almacenado.</param>
    /// <param name="parameters">Parámetros opcionales para el procedimiento.</param>
    /// <param name="timeout">Tiempo de espera en segundos (valor predeterminado 600).</param>
    /// <returns>Resultado encapsulado en <see cref="SqlQueryResult"/>.</returns>
    public SqlQueryResult Execute(
        [Required] string storeProcedureName,
        SqlParameter[]? parameters = null,
        int? timeout = null)
    {
        var request = BuildRequest(storeProcedureName, CommandType.StoredProcedure, parameters, cancellationToken: null, timeout);
        return Execute(request);
    }

    private Task<SqlQueryResult> ExecuteAsync(SqlCommandRequest request)
    {
        return RunSafeAsync(async () => new SqlQueryResult(await _commandExecutor.ExecuteAsync(request).ConfigureAwait(false)), request);
    }

    private SqlQueryResult Execute(SqlCommandRequest request)
    {
        return RunSafe(() => new SqlQueryResult(_commandExecutor.Execute(request)), request);
    }

    private static SqlCommandRequest BuildRequest(
        string commandText,
        CommandType commandType,
        SqlParameter[]? parameters,
        CancellationToken? cancellationToken,
        int? timeout)
    {
        if (string.IsNullOrWhiteSpace(commandText))
        {
            throw new ArgumentException("El texto del comando no puede ser nulo ni estar vacío.", nameof(commandText));
        }

        var effectiveTimeout = timeout ?? DefaultTimeout;
        var token = cancellationToken ?? CancellationToken.None;

        return new SqlCommandRequest(commandText, commandType, parameters, effectiveTimeout, token);
    }

    private static SqlQueryResult RunSafe(Func<SqlQueryResult> operation, SqlCommandRequest request)
    {
        try
        {
            return operation();
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException(BuildSqlErrorMessage(request.CommandText, ex), ex);
        }
        catch (OperationCanceledException)
        {
            throw new OperationCanceledException($"La ejecución del comando '{request.CommandText}' fue cancelada.");
        }
    }

    private static async Task<SqlQueryResult> RunSafeAsync(Func<Task<SqlQueryResult>> operation, SqlCommandRequest request)
    {
        try
        {
            return await operation().ConfigureAwait(false);
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException(BuildSqlErrorMessage(request.CommandText, ex), ex);
        }
        catch (OperationCanceledException)
        {
            throw new OperationCanceledException($"La ejecución del comando '{request.CommandText}' fue cancelada.");
        }
    }

    private static string BuildSqlErrorMessage(string commandText, SqlException ex)
    {
        return $"Error al ejecutar el comando SQL '{commandText}': {ex.Message}";
    }
}
