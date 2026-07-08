using DBSQLClient.Conexion;
using DBSQLClient.Demo.Models;
using DBSQLClient.Servicio.Parameter;

namespace DBSQLClient.Demo.Examples;

/// <summary>
/// Procedimiento almacenado simple (un solo result set) con
/// <see cref="ISQLClientService.ExecuteAsync"/>.
/// </summary>
public static class Example02_StoredProcedure
{
    public static async Task RunAsync(ISQLClientService db)
    {
        var result = await db.ExecuteAsync("sp_GetProductById", SqlParams.AddParams(("ProductId", 1)));

        var product = result.FirstOrDefault<Product>();

        Console.WriteLine($"ID: {product?.Id}");
        Console.WriteLine($"NOMBRE: {product?.Name}");
        Console.WriteLine($"DESCRIPCIÓN: {product?.Description}");
    }
}
