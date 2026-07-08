using DBSQLClient.Conexion;
using DBSQLClient.Demo.Models;
using DBSQLClient.Servicio.Parameter;

namespace DBSQLClient.Demo.Examples;

/// <summary>
/// Patrón para exponer un recurso en una API sin filtrar el <c>Id</c> interno (secuencial y
/// adivinable, ej: <c>/products/1</c>, <c>/products/2</c>...): <see cref="Product.Id"/> sigue
/// siendo la PK/FK real en la base (más compacta y rápida para joins/índices que un GUID), pero
/// solo <see cref="Product.PublicId"/> viaja hacia afuera. La librería no necesita ningún cambio
/// para esto: <c>[PrimaryKey]</c>/<c>[Column]</c> no exigen ningún tipo en particular, y
/// <see cref="SqlParams.Param(string, object?)"/> infiere <c>SqlDbType.UniqueIdentifier</c>
/// automáticamente de un <see cref="Guid"/>.
/// </summary>
public static class Example12_GuidPublicId
{
    public static async Task RunAsync(ISQLClientService db)
    {
        // En una API real el cliente ya tendría este PublicId guardado de una respuesta anterior
        // (ej: el listado de productos). Acá lo obtenemos con una consulta normal solo para
        // simular ese dato de entrada.
        var products = (await db.QueryAsync("SELECT product_id, public_id, product_name FROM Products"))
            .ToList<Product>();
        var publicId = products[0].PublicId;

        Console.WriteLine($"PublicId a consultar (lo único que vería un cliente de la API): {publicId}");

        var result = await db.ExecuteAsync("sp_GetProductByPublicId", SqlParams.AddParams(("PublicId", publicId)));
        var product = result.FirstOrDefault<Product>();

        Console.WriteLine($"Resuelto internamente a Id={product?.Id}: {product?.Name}");
    }
}
