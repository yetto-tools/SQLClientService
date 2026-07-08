using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models;

/// <summary>
/// <see cref="Items"/> es una foto fija de <see cref="InvoiceItem"/> (copiada de
/// <see cref="Order.Items"/> al momento de emitir la factura, no una referencia viva a
/// <c>OrderItems</c>): si la orden cambiara después, la factura ya emitida no se ve afectada.
/// Se trae con una segunda llamada a <c>sp_Invoice_With_Items</c> y se asigna a mano porque
/// <c>MapOneToOne&lt;Order, Invoice&gt;</c> solo resuelve 2 tablas (Order + Invoice) por llamada.
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

    [OneToMany(typeof(InvoiceItem))]
    public List<InvoiceItem> Items { get; set; } = new();
}
