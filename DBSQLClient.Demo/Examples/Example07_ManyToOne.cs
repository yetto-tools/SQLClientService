using DBSQLClient.Conexion;
using DBSQLClient.Demo.Models;
using DBSQLClient.Helpers;
using DBSQLClient.Servicio.Mapper.RelationsMapper;
using DBSQLClient.Servicio.Parameter;

namespace DBSQLClient.Demo.Examples;

/// <summary>
/// Relación muchos a 1, pero con DOS padres posibles: una orden pertenece a un
/// <see cref="User"/> registrado o a un <see cref="GuestCheckout"/> invitado, nunca a ambos.
/// <see cref="Order"/> declara dos <c>[ManyToOne]</c> independientes (uno por tipo de padre) y
/// cada <c>MapManyToOne</c> resuelve el que corresponda según el tipo pedido.
/// </summary>
public static class Example07_ManyToOne
{
    public static async Task RunAsync(ISQLClientService db)
    {
        Console.WriteLine("-- Órdenes de un usuario registrado (Erick) --");
        var userOrdersResult = await db.ExecuteAsync("sp_Orders_With_User", SqlParams.AddParams(("UserId", 1)));
        var userOrders = userOrdersResult.MapManyToOne<Order, User>();
        Console.WriteLine(userOrders.ToJsonString());

        // PasswordHash sí se mapeó desde la base (no es null), pero [NotSerialized] en el
        // modelo hace que nunca aparezca en el ToJsonString() de arriba.
        Console.WriteLine($"PasswordHash mapeado (no serializado en el JSON): {userOrders[0].User?.PasswordHash}");

        Console.WriteLine("-- Órdenes de un checkout de invitado --");
        var guestOrdersResult = await db.ExecuteAsync("sp_Orders_With_Guest", SqlParams.AddParams(("GuestId", 1)));
        var guestOrders = guestOrdersResult.MapManyToOne<Order, GuestCheckout>();
        Console.WriteLine(guestOrders.ToJsonString());
    }
}
