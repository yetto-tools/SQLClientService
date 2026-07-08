using DBSQLClient.Conexion;
using DBSQLClient.Demo.Models;
using DBSQLClient.Helpers;
using DBSQLClient.Servicio.Mapper.RelationsMapper;
using DBSQLClient.Servicio.Parameter;

namespace DBSQLClient.Demo.Examples;

/// <summary>
/// Modelo EAV para atributos de variante de producto: cada <see cref="ProductVariant"/> trae
/// tantas filas de <see cref="ProductVariantAttribute"/> como atributos le apliquen (ninguna
/// columna fija por categoría). Se corre para dos variantes para mostrar ambos casos: la
/// camiseta (Talla + Color) y los auriculares, que no tienen ningún atributo.
/// </summary>
public static class Example05_ProductVariantAttributes
{
    public static async Task RunAsync(ISQLClientService db)
    {
        Console.WriteLine("-- Variante 1 (Camiseta M/Rojo): tiene atributos --");
        await PrintVariantAsync(db, variantId: 1);

        Console.WriteLine("-- Variante 4 (Auriculares): sin ningún atributo --");
        await PrintVariantAsync(db, variantId: 4);
    }

    private static async Task PrintVariantAsync(ISQLClientService db, int variantId)
    {
        var result = await db.ExecuteAsync("sp_Variant_With_Attributes", SqlParams.AddParams(("VariantId", variantId)));

        var variant = result.MapOneToMany<ProductVariant, ProductVariantAttribute>();

        Console.WriteLine(variant.ToJsonString());
    }
}
