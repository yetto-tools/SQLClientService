using DBSQLClient.Conexion;
using DBSQLClient.Demo.Models;
using DBSQLClient.Helpers;
using DBSQLClient.Servicio.Mapper.RelationsMapper;

namespace DBSQLClient.Demo.Examples;

/// <summary>
/// Serialización JSON: <see cref="SqlQueryResult.ToJson{T}"/> para serializar directamente el
/// resultado plano de una consulta, <see cref="SqlQueryResult.SaveToJsonFileAsync{T}"/>/
/// <see cref="SqlQueryResult.FromJsonFileAsync{T}"/> para guardar y volver a leer desde disco, y
/// <see cref="ObjectJsonExtensions.ToJsonString"/> para serializar un objeto ya mapeado.
/// </summary>
/// <remarks>
/// <see cref="SqlQueryResult.ToJson{T}"/> solo mapea columnas de UNA tabla (como un
/// <c>SELECT</c> normal): no resuelve relaciones. Por eso, en la primera parte de este ejemplo,
/// <c>Categories</c> sale vacío en el JSON — es el comportamiento esperado, no un bug. Si
/// necesitás las relaciones incluidas, primero mapealas con <c>SqlResultMapper</c>
/// (<see cref="Example08_ManyToMany"/>) y recién ahí serializá ese objeto con
/// <see cref="ObjectJsonExtensions.ToJsonString"/>, como se muestra al final.
/// </remarks>
public static class Example10_JsonSerialization
{
    public static async Task RunAsync(ISQLClientService db)
    {
        var result = await db.QueryAsync("SELECT product_id, public_id, product_name, description FROM Products");

        Console.WriteLine("-- ToJson<T>() sobre un SELECT plano: 'Categories' queda vacío a propósito --");
        Console.WriteLine(result.ToJson<Product>());

        var filePath = Path.Combine(Path.GetTempPath(), "dbsqlclient-demo-products.json");
        await result.SaveToJsonFileAsync<Product>(filePath);

        var productsFromFile = await SqlQueryResult.FromJsonFileAsync<Product>(filePath);
        Console.WriteLine($"-- Leídos de nuevo desde '{filePath}': {productsFromFile.Count} productos --");
        File.Delete(filePath);

        Console.WriteLine("-- ToJsonString() sobre un objeto ya mapeado con MapManyToMany: 'Categories' sí viene poblado --");
        var withCategoriesResult = await db.ExecuteAsync("sp_Products_With_Categories");
        var productsWithCategories = withCategoriesResult.MapManyToMany<Product, Category, ProductCategory>();
        Console.WriteLine(productsWithCategories.ToJsonString());
    }
}
