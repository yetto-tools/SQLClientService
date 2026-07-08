using DBSQLClient.Conexion;
using DBSQLClient.Demo.Models;
using DBSQLClient.Helpers;
using DBSQLClient.Servicio.Mapper.RelationsMapper;

namespace DBSQLClient.Demo.Examples;

/// <summary>
/// Relación muchos a muchos a través de una tabla de unión: tabla 0 = Products, tabla 1 =
/// Categories, tabla 2 = ProductCategories. Las claves y la propiedad "Categories" se resuelven
/// vía <c>[ManyToMany]</c>/<c>[ForeignKey]</c>/<c>[PrimaryKey]</c> en los modelos.
/// </summary>
public static class Example08_ManyToMany
{
    public static async Task RunAsync(ISQLClientService db)
    {
        var result = await db.ExecuteAsync("sp_Products_With_Categories");

        var products = result.MapManyToMany<Product, Category, ProductCategory>();

        Console.WriteLine(products.ToJsonString());
    }
}
