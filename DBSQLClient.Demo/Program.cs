using DBSQLClient.Conexion;
using DBSQLClient.Demo.Database;
using DBSQLClient.Demo.Examples;

namespace DBSQLClient.Demo;

/// <summary>
/// Demo ejecutable de <c>DBSQLClient</c>: cada <c>Example*</c> en <c>Examples/</c> muestra una
/// capacidad de la librería de forma aislada y copiable a otro proyecto. Ver README.md de este
/// proyecto para el detalle de qué hace cada uno.
/// </summary>
public static class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Preparando base de datos de demo...");
        await DemoDatabaseBootstrapper.EnsureCreatedAsync();
        Console.WriteLine($"Base de datos '{DemoDatabase.Name}' lista.\n");

        var db = new SqlClientService(DemoDatabase.ConnectionString);

        await RunAsync("1. Consulta SQL de texto (QueryAsync)", () => Example01_RawQuery.RunAsync(db));
        await RunAsync("2. Procedimiento almacenado simple (ExecuteAsync)", () => Example02_StoredProcedure.RunAsync(db));
        await RunAsync("3. Parámetro de salida (SqlParams.OutParam)", () => Example03_OutputParameters.RunAsync(db));
        await RunAsync("4. Relación uno a uno (MapOneToOne): Order + Invoice", () => Example04_OneToOne.RunAsync(db));
        await RunAsync("5. Atributos de variante EAV (MapOneToMany)", () => Example05_ProductVariantAttributes.RunAsync(db));
        await RunAsync("6. Relación uno a muchos (MapOneToMany): Order + Items", () => Example06_OneToMany.RunAsync(db));
        await RunAsync("7. Relación muchos a uno (MapManyToOne): usuario vs. invitado", () => Example07_ManyToOne.RunAsync(db));
        await RunAsync("8. Relación muchos a muchos (MapManyToMany): Product + Category", () => Example08_ManyToMany.RunAsync(db));
        await RunAsync("9. Uno a muchos con varios padres (OneToManyMultiple): Carts", () => Example09_MultipleOneToMany.RunAsync(db));
        await RunAsync("10. Serialización JSON (ToJson / SaveToJsonFileAsync)", () => Example10_JsonSerialization.RunAsync(db));
        await RunAsync("11. SqlParameterBuilder + escritura (INSERT con output)", () => Example11_ParameterBuilder.RunAsync(db));
        await RunAsync("12. GUID público en vez de Id interno (para exponer en una API)", () => Example12_GuidPublicId.RunAsync(db));
        await RunAsync("13. Precio efectivo con descuentos/promociones (3 output params)", () => Example13_VariantPricing.RunAsync(db));
        await RunAsync("14. Combos (MapOneToMany) con vigencia por fecha", () => Example14_Combo.RunAsync(db));
        await RunAsync("15. Inventario: historial de movimientos + stock disponible", () => Example15_Inventory.RunAsync(db));
    }

    private static async Task RunAsync(string title, Func<Task> example)
    {
        Console.WriteLine(new string('=', 70));
        Console.WriteLine(title);
        Console.WriteLine(new string('=', 70));

        try
        {
            await example();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");
        }

        Console.WriteLine();
    }
}
