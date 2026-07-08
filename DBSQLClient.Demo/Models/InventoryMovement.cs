using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models;

/// <summary>
/// Un movimiento de stock de una <see cref="ProductVariant"/>: <see cref="Quantity"/> tiene
/// signo (positivo = entra, negativo = sale). <see cref="ProductVariant.StockQuantity"/> es el
/// saldo actual (más rápido de leer que sumar todo el historial en cada consulta); esta clase es
/// la auditoría de cómo se llegó a ese saldo. <see cref="OrderId"/> solo tiene valor en
/// movimientos <c>Sale</c>/<c>Return</c>, para trazar a la orden que los originó.
/// </summary>
[Table("InventoryMovements")]
public class InventoryMovement
{
    [PrimaryKey]
    [Column("movement_id")]
    public int Id { get; set; }

    [Column("public_id")]
    public Guid PublicId { get; set; }

    [ForeignKey(typeof(ProductVariant))]
    [Column("variant_id")]
    public int VariantId { get; set; }

    [Column("movement_type")]
    public string MovementType { get; set; } = null!;

    public int Quantity { get; set; }

    [Column("movement_date")]
    [JsDateTime]
    public DateTime MovementDate { get; set; }

    [Column("order_id")]
    public int? OrderId { get; set; }

    public string? Notes { get; set; }
}
