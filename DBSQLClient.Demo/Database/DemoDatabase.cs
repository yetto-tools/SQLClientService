namespace DBSQLClient.Demo.Database;

/// <summary>
/// Connection strings del demo. Cambia <see cref="Server"/> si no usas LocalDB.
/// </summary>
public static class DemoDatabase
{
    private const string Server = @"(localdb)\MSSQLLocalDB";

    /// <summary>
    /// Nombre de la base de datos que crea/reconstruye el demo.
    /// </summary>
    public const string Name = "DBSQLClientDemo";

    /// <summary>
    /// Conexión a <c>master</c>, usada solo por <see cref="DemoDatabaseBootstrapper"/> para
    /// poder ejecutar <c>CREATE DATABASE</c> y luego moverse a <see cref="Name"/> con <c>USE</c>.
    /// </summary>
    public static string MasterConnectionString =>
        $"Data Source={Server};Initial Catalog=master;Integrated Security=True;TrustServerCertificate=True;";

    /// <summary>
    /// Conexión que usan todos los <c>Example*</c> para hablar con <see cref="Name"/>.
    /// </summary>
    public static string ConnectionString =>
        $"Data Source={Server};Initial Catalog={Name};Integrated Security=True;TrustServerCertificate=True;Command Timeout=60";
}
