using System.Data;
using DBSQLClient.Conexion;
using DBSQLClient.Demo.Models;
using DBSQLClient.Helpers;
using DBSQLClient.Servicio.Parameter;

namespace DBSQLClient.Demo.Examples;

/// <summary>
/// Precio separado del producto: <c>ProductVariants.price</c> es el precio base, y
/// <c>Discounts</c> (que unifica promoción/descuento/oferta: son la misma idea) lo modifica según
/// reglas por variante puntual o por categoría completa, con vigencia por fecha.
/// <c>sp_GetVariantEffectivePrice</c> devuelve 3 parámetros de salida en la misma llamada —
/// ningún otro ejemplo del demo usa más de uno a la vez.
/// </summary>
/// <remarks>
/// Un descuento puede seguir con <c>is_active = 1</c> pero ya haber vencido por fecha (la "Promo
/// Expirada 50%" del seed): ambas condiciones se exigen a la vez, tanto para listar descuentos
/// vigentes acá como dentro del SP. Si una variante cae bajo más de un descuento vigente, gana el
/// que dé mayor monto en moneda, sin importar si es porcentaje o monto fijo.
/// </remarks>
public static class Example13_VariantPricing
{
    public static async Task RunAsync(ISQLClientService db)
    {
        var discountsResult = await db.QueryAsync(
            "SELECT discount_id, public_id, name, discount_type, value, start_date, end_date, is_active " +
            "FROM Discounts " +
            "WHERE is_active = 1 AND GETDATE() >= start_date AND (end_date IS NULL OR GETDATE() <= end_date)");
        var activeDiscounts = discountsResult.ToList<Discount>();

        Console.WriteLine($"Descuentos vigentes ahora mismo: {activeDiscounts.Count} (la 'Promo Expirada 50%' del seed queda afuera)");
        foreach (var discount in activeDiscounts)
        {
            Console.WriteLine($"- {discount.Name} ({discount.DiscountType} {discount.Value})");
        }

        // Discount.StartDate/EndDate usan [JsDateTime("yyyy-MM-dd")] (formato personalizado, solo
        // fecha) en vez del default ISO-8601 UTC que sí usan Order/Invoice/Combo -- se nota acá.
        Console.WriteLine($"Ejemplo con formato de fecha personalizado: {activeDiscounts[0].ToJsonString()}");

        Console.WriteLine();
        await PrintEffectivePriceAsync(db, variantId: 1, label: "Camiseta M/Rojo (descuento heredado de la categoría Ropa)");
        await PrintEffectivePriceAsync(db, variantId: 3, label: "Zapatilla Runner (descuento directo a la variante)");
        await PrintEffectivePriceAsync(db, variantId: 4, label: "Auriculares Bluetooth (sin ningún descuento aplicable)");
    }

    private static async Task PrintEffectivePriceAsync(ISQLClientService db, int variantId, string label)
    {
        var parameters = SqlParams.AddParams(("VariantId", variantId))
            .Append(SqlParams.OutParam("BasePrice", SqlDbType.Decimal, precision: 10, scale: 2))
            .Append(SqlParams.OutParam("EffectivePrice", SqlDbType.Decimal, precision: 10, scale: 2))
            .Append(SqlParams.OutParam("AppliedDiscountName", SqlDbType.NVarChar, 100))
            .ToArray();

        var result = await db.ExecuteAsync("sp_GetVariantEffectivePrice", parameters);

        var basePrice = result.GetOutputValue<decimal>("BasePrice");
        var effectivePrice = result.GetOutputValue<decimal>("EffectivePrice");
        var appliedDiscountName = result.GetOutputValue<string>("AppliedDiscountName");

        Console.WriteLine($"{label}: base={basePrice:C}, efectivo={effectivePrice:C}, descuento aplicado={appliedDiscountName ?? "(ninguno)"}");
    }
}
