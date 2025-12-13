using System.Data;
using Microsoft.Data.SqlClient;

namespace DBSQLClient.Conexion;

/// <summary>
/// Representa los datos necesarios para ejecutar un comando SQL.
/// </summary>
public sealed class SqlCommandRequest
{
    /// <summary>
    /// Crea una nueva instancia del comando.
    /// </summary>
    /// <param name="commandText">Texto del comando a ejecutar (consulta o procedimiento).</param>
    /// <param name="commandType">Tipo de comando a ejecutar.</param>
    /// <param name="parameters">Parámetros opcionales que se enviarán con el comando.</param>
    /// <param name="timeout">Tiempo de espera en segundos antes de cancelar la ejecución.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <exception cref="ArgumentException">Cuando <paramref name="commandText"/> es nulo o vacío.</exception>
    public SqlCommandRequest(
        string commandText,
        CommandType commandType,
        SqlParameter[]? parameters,
        int timeout,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(commandText))
        {
            throw new ArgumentException("El texto del comando no puede ser nulo ni estar vacío.", nameof(commandText));
        }

        CommandText = commandText;
        CommandType = commandType;
        Parameters = parameters ?? Array.Empty<SqlParameter>();
        Timeout = timeout;
        CancellationToken = cancellationToken;
    }

    /// <summary>
    /// Texto completo del comando a ejecutar.
    /// </summary>
    public string CommandText { get; }

    /// <summary>
    /// Tipo de comando (Texto o Procedimiento Almacenado).
    /// </summary>
    public CommandType CommandType { get; }

    /// <summary>
    /// Colección de parámetros a enviar con el comando.
    /// </summary>
    public SqlParameter[] Parameters { get; }

    /// <summary>
    /// Tiempo máximo de ejecución permitido en segundos.
    /// </summary>
    public int Timeout { get; }

    /// <summary>
    /// Token de cancelación para operaciones asíncronas.
    /// </summary>
    public CancellationToken CancellationToken { get; }
}
