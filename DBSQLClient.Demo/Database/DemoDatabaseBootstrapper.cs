using System.Text;
using Microsoft.Data.SqlClient;

namespace DBSQLClient.Demo.Database;

/// <summary>
/// Crea/reconstruye la base de datos del demo ejecutando <c>DemoDatabaseSetup.sql</c> (embebido
/// como recurso) contra LocalDB. Usa <see cref="SqlConnection"/>/<see cref="SqlCommand"/>
/// directamente en vez de pasar por <c>SqlClientService</c>: esto es infraestructura del demo
/// (DDL de varios batches separados por <c>GO</c>), no una capacidad de la librería a mostrar.
/// </summary>
/// <remarks>
/// Este bootstrapper hace DROP + CREATE de todo en cada ejecución para dejar siempre el mismo
/// estado de datos, ideal para practicar. No repliques este patrón contra una base real.
/// </remarks>
public static class DemoDatabaseBootstrapper
{
    private const string ResourceName = "DemoDatabaseSetup.sql";

    /// <summary>
    /// Ejecuta el script de setup completo contra LocalDB, creando <see cref="DemoDatabase.Name"/>
    /// si no existe y reconstruyendo tablas, procedimientos y datos de ejemplo.
    /// </summary>
    public static async Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
    {
        var script = LoadEmbeddedScript();

        using var connection = new SqlConnection(DemoDatabase.MasterConnectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        foreach (var batch in SplitBatches(script))
        {
            using var command = connection.CreateCommand();
            command.CommandText = batch;
            command.CommandTimeout = 60;
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private static string LoadEmbeddedScript()
    {
        var assembly = typeof(DemoDatabaseBootstrapper).Assembly;
        using var stream = assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException(
                $"No se encontró el recurso embebido '{ResourceName}'. Verifica el <EmbeddedResource> en el .csproj.");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Separa el script en batches usando líneas <c>GO</c> como delimitador: T-SQL no permite
    /// mezclar <c>CREATE PROCEDURE</c> con otras sentencias en el mismo batch, y <c>GO</c> solo
    /// lo entienden clientes como sqlcmd/SSMS, no <see cref="SqlCommand"/>.
    /// </summary>
    private static IEnumerable<string> SplitBatches(string script)
    {
        var lines = script.Replace("\r\n", "\n").Split('\n');
        var current = new StringBuilder();

        foreach (var line in lines)
        {
            if (line.Trim().Equals("GO", StringComparison.OrdinalIgnoreCase))
            {
                if (current.Length > 0)
                {
                    yield return current.ToString();
                    current.Clear();
                }

                continue;
            }

            current.AppendLine(line);
        }

        if (current.Length > 0)
        {
            yield return current.ToString();
        }
    }
}
