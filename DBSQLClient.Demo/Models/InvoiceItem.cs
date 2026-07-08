using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models;

/// <summary>
/// Foto fija de lo que se facturó: se llena copiando <see cref="OrderItem"/> al momento de emitir
/// la factura, no es una referencia viva a <c>OrderItems</c>. Si después cambiara el precio de la
/// variante o la cantidad de la orden, esta fila no se entera — una factura ya emitida no debería
/// cambiar (a diferencia de <see cref="Order.Items"/>, que sí refleja la orden tal como está hoy).
/// </summary>
[Table("InvoiceItems")]
public class InvoiceItem
{
    [PrimaryKey]
    [Column("invoice_item_id")]
    public int Id { get; set; }

    [Column("public_id")]
    public Guid PublicId { get; set; }

    [ForeignKey(typeof(Invoice))]
    [Column("invoice_id")]
    public int InvoiceId { get; set; }

    [Column("variant_id")]
    public int VariantId { get; set; }

    public int Quantity { get; set; }

    [Column("unit_price")]
    public decimal UnitPrice { get; set; }
}
