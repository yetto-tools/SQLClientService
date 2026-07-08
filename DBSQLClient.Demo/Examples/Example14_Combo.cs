using DBSQLClient.Conexion;
using DBSQLClient.Demo.Models;
using DBSQLClient.Helpers;
using DBSQLClient.Servicio.Mapper.RelationsMapper;
using DBSQLClient.Servicio.Parameter;

namespace DBSQLClient.Demo.Examples;

/// <summary>
/// Un combo agrupa variantes a un precio especial (<see cref="Combo.ComboPrice"/> vs. la suma de
/// precios individuales), y vence igual que un descuento: el listado de combos vigentes filtra
/// por fecha/<c>is_active</c> antes de mostrar el detalle con <c>MapOneToMany</c>. "Combo
/// Invierno" ya venció y no aparece en el listado aunque siga en la base — <c>sp_Combo_With_Items</c>
/// no filtra vigencia porque sirve para consultar el detalle de cualquier combo que ya conozcas
/// por id, esté vigente o no.
/// </summary>
/// <remarks>
/// Solo catálogo por ahora: los combos no se pueden agregar al carrito todavía (<see
/// cref="CartItem"/>/<see cref="OrderItem"/> siguen referenciando variantes individuales).
/// </remarks>
public static class Example14_Combo
{
    public static async Task RunAsync(ISQLClientService db)
    {
        var activeCombosResult = await db.QueryAsync(
            "SELECT combo_id, public_id, name, combo_price, start_date, end_date, is_active " +
            "FROM Combos " +
            "WHERE is_active = 1 AND GETDATE() >= start_date AND (end_date IS NULL OR GETDATE() <= end_date)");
        var activeCombos = activeCombosResult.ToList<Combo>();

        Console.WriteLine($"Combos vigentes ahora mismo: {activeCombos.Count} ('Combo Invierno' del seed ya venció y queda afuera)");
        foreach (var combo in activeCombos)
        {
            Console.WriteLine($"- {combo.Name}: {combo.ComboPrice:C}");
        }

        var result = await db.ExecuteAsync("sp_Combo_With_Items", SqlParams.AddParams(("ComboId", activeCombos[0].Id)));
        var comboWithItems = result.MapOneToMany<Combo, ComboItem>();

        Console.WriteLine(comboWithItems.ToJsonString());
    }
}
