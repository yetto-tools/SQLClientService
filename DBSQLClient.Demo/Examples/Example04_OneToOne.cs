using DBSQLClient.Conexion;
using DBSQLClient.Demo.Models;
using DBSQLClient.Helpers;
using DBSQLClient.Servicio.Mapper.RelationsMapper;
using DBSQLClient.Servicio.Parameter;

namespace DBSQLClient.Demo.Examples;

/// <summary>
/// Relación 1 a 1: el SP devuelve 2 result sets (tabla 0 = Order, tabla 1 = Invoice) y
/// <see cref="SqlResultMapper.MapOneToOne"/> los combina en un solo objeto. La propiedad de
/// navegación ("Invoice") se resuelve automáticamente a partir de
/// <c>[OneToOne(typeof(Invoice))]</c> en <see cref="Order"/>.
/// </summary>
/// <remarks>
/// <c>MapOneToOne</c> solo resuelve 2 tablas por llamada, así que el detalle de lo que se
/// facturó (<see cref="Invoice.Items"/>, una foto fija en <c>InvoiceItems</c>, no una referencia
/// viva a <c>OrderItems</c>) se trae con una segunda llamada (<c>sp_Invoice_With_Items</c> +
/// <c>MapOneToMany&lt;Invoice, InvoiceItem&gt;</c>) y se combina a mano en el mismo objeto ya
/// mapeado — el patrón general para poblar varias relaciones de un mismo objeto cuando no hay un
/// solo SP que las traiga todas juntas.
/// </remarks>
public static class Example04_OneToOne
{
    public static async Task RunAsync(ISQLClientService db)
    {
        var result = await db.ExecuteAsync("sp_Order_With_Invoice", SqlParams.AddParams(("OrderId", 1)));
        var order = result.MapOneToOne<Order, Invoice>();

        if (order.Invoice is { } invoice)
        {
            var itemsResult = await db.ExecuteAsync("sp_Invoice_With_Items", SqlParams.AddParams(("InvoiceId", invoice.Id)));
            invoice.Items = itemsResult.MapOneToMany<Invoice, InvoiceItem>().Items;
        }

        Console.WriteLine(order.ToJsonString());
    }
}
