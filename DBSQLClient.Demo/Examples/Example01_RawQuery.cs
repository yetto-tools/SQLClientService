using DBSQLClient.Conexion;
using DBSQLClient.Demo.Models;

namespace DBSQLClient.Demo.Examples;

/// <summary>
/// Consulta SQL de texto con <see cref="ISQLClientService.QueryAsync"/>: sin procedimiento<br></br>
/// almacenado, mapeo directo a una lista tipada.
/// </summary>
public static class Example01_RawQuery
{
    public static async Task RunAsync(ISQLClientService db)
    {
        var result = await db.QueryAsync("SELECT product_id, product_name, description FROM Products");

        var products = result.ToList<Product>();

        Console.WriteLine($"Productos encontrados: {products.Count}");
        foreach (var product in products)
        {
            Console.WriteLine($"- [{product.Id}] {product.Name}: {product.Description}");
        }
    }
}
