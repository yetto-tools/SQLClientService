using DBSQLClient.Conexion;
using DBSQLClient.Demo.Models;
using DBSQLClient.Helpers;
using DBSQLClient.Servicio.Mapper.RelationsMapper;

namespace DBSQLClient.Demo.Examples;

/// <summary>
/// Variante de 1 a muchos con VARIOS padres en el mismo resultado: tabla 0 = todos los Carts,
/// tabla 1 = todos los CartItems. Cada carrito recibe solo sus propios ítems agrupando por
/// clave primaria/foránea. El carrito de Ana no tiene ítems, así que su lista queda vacía en
/// vez de fallar.
/// </summary>
public static class Example09_MultipleOneToMany
{
    public static async Task RunAsync(ISQLClientService db)
    {
        var result = await db.ExecuteAsync("sp_AllCarts_With_Items");

        var carts = result.OneToManyMultiple<Cart, CartItem>();

        Console.WriteLine(carts.ToJsonString());
    }
}
