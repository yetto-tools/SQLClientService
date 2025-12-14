using System.Data;
using Microsoft.Data.SqlClient;

namespace DBSQLClient.Conexion;

/// <summary>
/// Define operaciones para ejecutar comandos SQL y obtener un <see cref="DataSet"/>.
/// </summary>
public interface ISqlCommandExecutor
{
    /// <summary>
    /// Ejecuta un comando SQL de manera asíncrona.
    /// </summary>
    /// <param name="request">Datos del comando a ejecutar.</param>
    /// <returns><see cref="DataSet"/> con las tablas devueltas por el comando.</returns>
    Task<DataSet> ExecuteAsync(SqlCommandRequest request);

    /// <summary>
    /// Ejecuta un comando SQL de manera sincrónica.
    /// </summary>
    /// <param name="request">Datos del comando a ejecutar.</param>
    /// <returns><see cref="DataSet"/> con las tablas devueltas por el comando.</returns>
    DataSet Execute(SqlCommandRequest request);
}

/// <summary>
/// Ejecuta comandos SQL utilizando una <see cref="SqlConnection"/> proporcionada por una factoría.
/// </summary>
public sealed class SqlCommandExecutor : ISqlCommandExecutor
{
    private readonly ISqlConnectionFactory _connectionFactory;

    /// <summary>
    /// Inicializa una nueva instancia del ejecutor.
    /// </summary>
    /// <param name="connectionFactory">Factoría responsable de crear conexiones SQL.</param>
    /// <exception cref="ArgumentNullException">Si la factoría es nula.</exception>
    public SqlCommandExecutor(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    /// <inheritdoc />
    public async Task<DataSet> ExecuteAsync(SqlCommandRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(request.CancellationToken).ConfigureAwait(false);

        using var command = BuildCommand(connection, request);
        using var reader = await command.ExecuteReaderAsync(request.CancellationToken).ConfigureAwait(false);

        return await ReadDataSetAsync(reader, request.CancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public DataSet Execute(SqlCommandRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = BuildCommand(connection, request);
        using var reader = command.ExecuteReader();

        return ReadDataSet(reader);
    }

    private static SqlCommand BuildCommand(SqlConnection connection, SqlCommandRequest request)
    {
        var command = connection.CreateCommand();
        command.CommandText = request.CommandText;
        command.CommandType = request.CommandType;
        command.CommandTimeout = request.Timeout;

        if (request.Parameters.Length > 0)
        {
            command.Parameters.AddRange(request.Parameters);
        }

        return command;
    }

    private static async Task<DataSet> ReadDataSetAsync(SqlDataReader reader, CancellationToken cancellationToken)
    {
        var dataSet = new DataSet();

        while (!reader.IsClosed)
        {
            var dataTable = new DataTable();
            dataTable.Load(reader);
            dataSet.Tables.Add(dataTable);

            //if (!await reader.NextResultAsync(cancellationToken).ConfigureAwait(false))
            //{
            //    break;
            //}
        }

        return dataSet;
    }

    private static DataSet ReadDataSet(SqlDataReader reader)
    {
        var dataSet = new DataSet();

        while (!reader.IsClosed)
        {
            var dataTable = new DataTable();
            dataTable.Load(reader);
            dataSet.Tables.Add(dataTable);
        }

        return dataSet;
    }
}
