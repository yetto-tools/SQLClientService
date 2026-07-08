using System.Data;
using DBSQLClient.Conexion;
using DBSQLClient.Servicio.Parameter;

namespace DBSQLClient.Demo.Examples;

/// <summary>
/// Procedimiento con parámetro de salida: se agrega con <see cref="SqlParams.OutParam"/> y se
/// combina con los de entrada. El valor se lee desde el resultado
/// (<see cref="SqlQueryResult.GetOutputValue{T}"/>), no desde la variable original del parámetro.
/// Aquí calcula el subtotal del carrito de Erick (cart_id 1).
/// </summary>
/// <remarks>
/// Para un output <see cref="SqlDbType.Decimal"/> hace falta la sobrecarga con precisión/escala
/// (o <c>.WithPrecision(...)</c>): sin eso, ADO.NET asume <c>Scale = 0</c> y redondea el valor de
/// salida a un entero sin avisar (ej: <c>99.97</c> volvería <c>100</c>).
/// </remarks>
public static class Example03_OutputParameters
{
    public static async Task RunAsync(ISQLClientService db)
    {
        var parameters = SqlParams.AddParams(("CartId", 1))
            .Append(SqlParams.OutParam("Total", SqlDbType.Decimal, precision: 10, scale: 2))
            .ToArray();

        var result = await db.ExecuteAsync("sp_GetCartTotal", parameters);
        var total = result.GetOutputValue<decimal>("Total");

        Console.WriteLine($"SUBTOTAL DEL CARRITO (output param): {total:C}");
    }
}
