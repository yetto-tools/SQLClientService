using DBSQLClient.Conexion;
using DBSQLClient.Demo.Models;
using DBSQLClient.Helpers;
using DBSQLClient.Servicio.Mapper.RelationsMapper;
using DBSQLClient.Servicio.Parameter;

namespace DBSQLClient.Demo.Examples;

/// <summary>
/// Relación 1 a muchos: tabla 0 = Order, tabla 1 = OrderItems. La propiedad de navegación se
/// resuelve vía <c>[OneToMany(typeof(OrderItem))]</c> en <see cref="Order"/>.
/// </summary>
public static class Example06_OneToMany
{
    public static async Task RunAsync(ISQLClientService db)
    {
        var result = await db.ExecuteAsync("sp_Order_With_Items", SqlParams.AddParams(("OrderId", 1)));

        var order = result.MapOneToMany<Order, OrderItem>();

        Console.WriteLine(order.ToJsonString());
    }
}
