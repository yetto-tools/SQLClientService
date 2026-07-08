using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models;

/// <summary>
/// <see cref="Items"/> es el mismo detalle de producto que <see cref="Order.Items"/> (una
/// factura factura exactamente lo que trae la orden en este demo), pero se declara acá también
/// porque <c>MapOneToOne&lt;Order, Invoice&gt;</c> solo resuelve 2 tablas (Order + Invoice): el
/// detalle se trae con una segunda llamada a <c>sp_Invoice_With_Items</c> y se asigna a mano.
/// </summary>
[Table("Invoices")]
public class Invoice
{
    [PrimaryKey]
    [Column("invoice_id")]
    public int Id { get; set; }

    [Column("public_id")]
    public Guid PublicId { get; set; }

    [ForeignKey(typeof(Order))]
    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("invoice_number")]
    public string InvoiceNumber { get; set; } = null!;

    [Column("issued_date")]
    [JsDateTime]
    public DateTime IssuedDate { get; set; }

    [Column("total_amount")]
    public decimal TotalAmount { get; set; }

    [OneToMany(typeof(OrderItem))]
    public List<OrderItem> Items { get; set; } = new();
}
