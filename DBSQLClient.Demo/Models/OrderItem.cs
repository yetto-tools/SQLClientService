using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models;
/// <summary>
/// Línea de una orden ya colocada. <see cref="UnitPrice"/> es una copia del precio al momento de
/// la compra, no una referencia viva a <c>ProductVariants.price</c>: si el precio de la variante
/// (o sus descuentos) cambia después, esta orden no se ve afectada.
/// </summary>
[Table("OrderItems")]
public class OrderItem
{
    [PrimaryKey]
    [Column("order_item_id")]
    public int Id { get; set; }

    [Column("public_id")]
    public Guid PublicId { get; set; }

    [ForeignKey(typeof(Order))]
    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("variant_id")]
    public int VariantId { get; set; }

    public int Quantity { get; set; }

    [Column("unit_price")]
    public decimal UnitPrice { get; set; }
}
