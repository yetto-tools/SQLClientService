using System.Data;
using DBSQLClient.Servicio.Parameter;
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
        // También se normaliza el nombre (agregando '@' si falta) como red de seguridad
        // para parámetros creados a mano con `new SqlParameter(...)` sin pasar por SqlParams,
        // ya que SqlParameter de Microsoft NO agrega el '@' por sí solo.
        foreach (var parameter in request.Parameters)
        {
            var clone = (SqlParameter)((ICloneable)parameter).Clone();
            clone.ParameterName = SqlParams.NormalizeName(clone.ParameterName);
            command.Parameters.Add(clone);
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

            var normalizedName = SqlParams.NormalizeName(original.ParameterName);
            if (command.Parameters.Contains(normalizedName))
            {
                original.Value = command.Parameters[normalizedName].Value;
            }
        }
    }

    /// <summary>
    /// Lee todos los result sets del <paramref name="reader"/> hacia un <see cref="DataSet"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="DataTable.Load(IDataReader)"/> ya avanza internamente al siguiente result set
    /// como parte de su propia ejecución (y cierra el reader cuando no quedan más). Por eso este
    /// método NO debe llamar <c>NextResult</c>/<c>NextResultAsync</c> después de un <c>Load</c>
    /// exitoso: hacerlo saltaría el siguiente result set sin leerlo. La única razón para avanzar
    /// manualmente es saltar un result set sin columnas (por ejemplo, de una sentencia que no es
    /// <c>SELECT</c> mezclada en el procedimiento), que <c>Load</c> no puede materializar.
    /// </remarks>
    private static async Task<DataSet> ReadDataSetAsync(SqlDataReader reader, CancellationToken cancellationToken)
    {
        var dataSet = new DataSet();

        while (!reader.IsClosed)
        {
            if (reader.FieldCount == 0)
            {
                if (!await reader.NextResultAsync(cancellationToken).ConfigureAwait(false))
                {
                    break;
                }

                continue;
            }

            var dataTable = new DataTable();
            dataTable.Load(reader);
            dataSet.Tables.Add(dataTable);
        }

        return dataSet;
    }

    /// <inheritdoc cref="ReadDataSetAsync"/>
    private static DataSet ReadDataSet(SqlDataReader reader)
    {
        var dataSet = new DataSet();

        while (!reader.IsClosed)
        {
            if (reader.FieldCount == 0)
            {
                if (!reader.NextResult())
                {
                    break;
                }

                continue;
            }

            var dataTable = new DataTable();
            dataTable.Load(reader);
            dataSet.Tables.Add(dataTable);
        }

        return dataSet;
    }
}
