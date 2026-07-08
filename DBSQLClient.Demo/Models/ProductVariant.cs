using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models;

/// <summary>SKU vendible de un <see cref="Product"/>. Sus atributos (talla, color, ancho, etc.)
/// no son columnas fijas: viven en <see cref="ProductVariantAttribute"/> porque varían según la
/// categoría del producto y algunas variantes pueden no tener ninguno.</summary>
[Table("ProductVariants")]
public class ProductVariant
{
    [PrimaryKey]
    [Column("variant_id")]
    public int Id { get; set; }

    [Column("public_id")]
    public Guid PublicId { get; set; }

    [ForeignKey(typeof(Product))]
    [Column("product_id")]
    public int ProductId { get; set; }

    public string Sku { get; set; } = null!;

    public decimal Price { get; set; }

    // Saldo actual (más rápido de leer que sumar el historial completo). El historial en sí
    // vive en Movements/InventoryMovements -- ver sp_GetVariantAvailableStock para el cálculo
    // de cuánto de este stock ya está reservado en carritos y no debería volver a venderse.
    [Column("stock_quantity")]
    public int StockQuantity { get; set; }

    [OneToMany(typeof(ProductVariantAttribute))]
    public List<ProductVariantAttribute> Attributes { get; set; } = new();

    [OneToMany(typeof(InventoryMovement))]
    public List<InventoryMovement> Movements { get; set; } = new();
}
