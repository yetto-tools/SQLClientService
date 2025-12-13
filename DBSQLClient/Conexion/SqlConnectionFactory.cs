using Microsoft.Data.SqlClient;

namespace DBSQLClient.Conexion;

/// <summary>
/// Expone un contrato para construir instancias de <see cref="SqlConnection"/>.
/// </summary>
public interface ISqlConnectionFactory
{
    /// <summary>
    /// Crea una nueva conexión lista para ser abierta.
    /// </summary>
    /// <returns>Instancia de <see cref="SqlConnection"/> configurada con la cadena de conexión provista.</returns>
    SqlConnection CreateConnection();
}

/// <summary>
/// Implementación predeterminada de <see cref="ISqlConnectionFactory"/> que encapsula la cadena de conexión.
/// </summary>
public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    /// <summary>
    /// Inicializa una nueva instancia con la cadena de conexión suministrada.
    /// </summary>
    /// <param name="connectionString">Cadena de conexión SQL Server.</param>
    /// <exception cref="ArgumentException">Se produce si la cadena es nula, vacía o solo espacios.</exception>
    public SqlConnectionFactory(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("La cadena de conexión no puede ser nula ni estar vacía.", nameof(connectionString));
        }

        _connectionString = connectionString;
    }

    /// <inheritdoc />
    public SqlConnection CreateConnection() => new(_connectionString);
}
