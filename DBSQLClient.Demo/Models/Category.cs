using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models;

/// <summary>
/// Categoría de producto. Además de agrupar <see cref="Product"/> vía la unión
/// <see cref="ProductCategory"/> (muchos a muchos), un <c>Discount</c> puede apuntar a toda una
/// categoría en vez de a una variante puntual (ver <c>DiscountCategories</c> y
/// <c>sp_GetVariantEffectivePrice</c>): todas las variantes de todos los productos de esa
/// categoría heredan el descuento.
/// </summary>
[Table("Categories")]
public class Category
{
    [PrimaryKey]
    [Column("category_id")]
    public int Id { get; set; }

    [Column("public_id")]
    public Guid PublicId { get; set; }

    [Column("category_name")]
    public string Name { get; set; } = null!;
}
