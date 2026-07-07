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

        DataSet dataSet;
        using (var reader = await command
            .ExecuteReaderAsync(request.Behavior, request.CancellationToken)
            .ConfigureAwait(false))
        {
            dataSet = await ReadDataSetAsync(reader, request.CancellationToken).ConfigureAwait(false);
        }

        CopyOutputParameterValues(command, request.Parameters);
        return dataSet;
    }

    /// <inheritdoc />
    public DataSet Execute(SqlCommandRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = BuildCommand(connection, request);

        DataSet dataSet;
        using (var reader = command.ExecuteReader(request.Behavior))
        {
            dataSet = ReadDataSet(reader);
        }

        CopyOutputParameterValues(command, request.Parameters);
        return dataSet;
    }

    private static SqlCommand BuildCommand(SqlConnection connection, SqlCommandRequest request)
    {
        var command = connection.CreateCommand();
        command.CommandText = request.CommandText;
        command.CommandType = request.CommandType;
        command.CommandTimeout = request.Timeout;

        // Se clonan los parámetros: un SqlParameter solo puede pertenecer a un
        // SqlParameterCollection a la vez, así que agregar las instancias originales
        // impediría reusar el mismo SqlParameter[] en una segunda ejecución.
        foreach (var parameter in request.Parameters)
        {
            command.Parameters.Add(((ICloneable)parameter).Clone());
        }

        return command;
    }

    /// <summary>
    /// Copia el valor de los parámetros Output/InputOutput/ReturnValue desde los clones
    /// usados en la ejecución hacia los objetos originales que conserva el llamador.
    /// Debe llamarse después de cerrar el <see cref="SqlDataReader"/>: ADO.NET no rellena
    /// estos valores hasta ese momento.
    /// </summary>
    private static void CopyOutputParameterValues(SqlCommand command, SqlParameter[] originalParameters)
    {
        foreach (var original in originalParameters)
        {
            if (original.Direction == ParameterDirection.Input)
            {
                continue;
            }

            if (command.Parameters.Contains(original.ParameterName))
            {
                original.Value = command.Parameters[original.ParameterName].Value;
            }
        }
    }

    private static async Task<DataSet> ReadDataSetAsync(SqlDataReader reader, CancellationToken cancellationToken)
    {
        var dataSet = new DataSet();

        if (!await EnsureResultWithFieldsAsync(reader, cancellationToken).ConfigureAwait(false))
        {
            return dataSet;
        }

        do
        {
            var dataTable = new DataTable();
            dataTable.Load(reader);
            dataSet.Tables.Add(dataTable);
        }
        while (await MoveToNextResultWithFieldsAsync(reader, cancellationToken).ConfigureAwait(false));

        return dataSet;
    }

    private static DataSet ReadDataSet(SqlDataReader reader)
    {
        var dataSet = new DataSet();

        if (!EnsureResultWithFields(reader))
        {
            return dataSet;
        }

        do
        {
            var dataTable = new DataTable();
            dataTable.Load(reader);
            dataSet.Tables.Add(dataTable);
        }
        while (MoveToNextResultWithFields(reader));

        return dataSet;
    }

    private static async Task<bool> EnsureResultWithFieldsAsync(SqlDataReader reader, CancellationToken cancellationToken)
    {
        if (reader.FieldCount > 0)
        {
            return true;
        }

        return await MoveToNextResultWithFieldsAsync(reader, cancellationToken).ConfigureAwait(false);
    }

    private static bool EnsureResultWithFields(SqlDataReader reader)
    {
        if (reader.FieldCount > 0)
        {
            return true;
        }

        return MoveToNextResultWithFields(reader);
    }

    private static async Task<bool> MoveToNextResultWithFieldsAsync(SqlDataReader reader, CancellationToken cancellationToken)
    {
        while (await reader.NextResultAsync(cancellationToken).ConfigureAwait(false))
        {
            if (reader.FieldCount > 0)
            {
                return true;
            }
        }

        return false;
    }

    private static bool MoveToNextResultWithFields(SqlDataReader reader)
    {
        while (reader.NextResult())
        {
            if (reader.FieldCount > 0)
            {
                return true;
            }
        }

        return false;
    }
}
