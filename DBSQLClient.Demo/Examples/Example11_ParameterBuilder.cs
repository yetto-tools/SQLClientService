using System.Data;
using DBSQLClient.Conexion;
using DBSQLClient.Servicio.Parameter;

namespace DBSQLClient.Demo.Examples;

/// <summary>
/// <see cref="SqlParameterBuilder"/> fluido en vez de <see cref="SqlParams.AddParams"/>, y único
/// ejemplo de escritura del demo: agrega un ítem al carrito de Ana (vacío hasta este punto) con
/// <c>sp_AddCartItem</c> y recupera el id generado a través de un parámetro de salida.
/// </summary>
public static class Example11_ParameterBuilder
{
    public static async Task RunAsync(ISQLClientService db)
    {
        var parameters = new SqlParameterBuilder()
            .AddInt("CartId", 2)
            .AddInt("VariantId", 2)
            .AddInt("Quantity", 1)
            .AddOutput("NewCartItemId", SqlDbType.Int)
            .Build();

        var result = await db.ExecuteAsync("sp_AddCartItem", parameters);
        var newCartItemId = result.GetOutputValue<int>("NewCartItemId");

        Console.WriteLine($"Ítem agregado al carrito de Ana con Id: {newCartItemId}");
    }
}
