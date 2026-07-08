using System.Data;
using DBSQLClient.Conexion;
using DBSQLClient.Demo.Models;
using DBSQLClient.Helpers;
using DBSQLClient.Servicio.Mapper.RelationsMapper;
using DBSQLClient.Servicio.Parameter;

namespace DBSQLClient.Demo.Examples;

/// <summary>
/// Inventario separado del catálogo: <see cref="ProductVariant.StockQuantity"/> es el saldo
/// actual, y <see cref="InventoryMovement"/> es la auditoría de movimientos (compras, ventas,
/// ajustes, daños) que explican cómo se llegó a ese saldo — mismo patrón "valor base + registro
/// aparte" que <c>Discounts</c> para el precio.
/// </summary>
/// <remarks>
/// <para>
/// El segundo bloque (<c>sp_GetVariantAvailableStock</c>) muestra por qué el stock físico solo no
/// alcanza para saber cuánto se puede vender: hay que descontar lo que ya está reservado en
/// carritos (todavía sin convertirse en orden), o dos clientes podrían "comprar" las mismas
/// últimas unidades sin que ninguno se entere hasta el checkout.
/// </para>
/// <para>
/// En el JSON de <see cref="PrintMovementsAsync"/>, <c>Attributes</c> sale siempre vacío — no
/// porque la variante no tenga atributos (la variante 2 sí tiene Talla/Color, ver
/// <see cref="Example05_ProductVariantAttributes"/>), sino porque <c>sp_Variant_With_Movements</c>
/// no trae esa tabla: <c>MapOneToMany&lt;ProductVariant, InventoryMovement&gt;</c> solo llena
/// <c>Movements</c>, y una relación que ninguna llamada consultó queda en su valor por defecto
/// (mismo caso que <c>Categories</c> vacío en <see cref="Example10_JsonSerialization"/>).
/// </para>
/// </remarks>
public static class Example15_Inventory
{
    public static async Task RunAsync(ISQLClientService db)
    {
        Console.WriteLine("(nota: 'Attributes' sale vacío en los dos JSON de abajo a propósito -- " +
            "sp_Variant_With_Movements no trae esa tabla, no es que la variante no tenga atributos)");

        Console.WriteLine("-- Variante 1 (Camiseta M/Rojo): compra inicial + venta de la orden 1 --");
        await PrintMovementsAsync(db, variantId: 1);

        Console.WriteLine("-- Variante 2 (Camiseta L/Azul): sin ningún movimiento registrado todavía --");
        await PrintMovementsAsync(db, variantId: 2);

        var parameters = SqlParams.AddParams(("VariantId", 1))
            .Append(SqlParams.OutParam("PhysicalStock", SqlDbType.Int))
            .Append(SqlParams.OutParam("ReservedInCarts", SqlDbType.Int))
            .Append(SqlParams.OutParam("AvailableStock", SqlDbType.Int))
            .ToArray();

        var result = await db.ExecuteAsync("sp_GetVariantAvailableStock", parameters);
        var physicalStock = result.GetOutputValue<int>("PhysicalStock");
        var reservedInCarts = result.GetOutputValue<int>("ReservedInCarts");
        var availableStock = result.GetOutputValue<int>("AvailableStock");

        Console.WriteLine(
            $"Camiseta M/Rojo -- físico: {physicalStock}, reservado en carritos: {reservedInCarts}, disponible para vender: {availableStock}");
    }

    private static async Task PrintMovementsAsync(ISQLClientService db, int variantId)
    {
        var result = await db.ExecuteAsync("sp_Variant_With_Movements", SqlParams.AddParams(("VariantId", variantId)));
        var variant = result.MapOneToMany<ProductVariant, InventoryMovement>();

        Console.WriteLine(variant.ToJsonString());
    }
}
