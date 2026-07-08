using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models;

/// <summary>Tabla de unión de la relación muchos a muchos entre <see cref="Product"/> y
/// <see cref="Category"/>.</summary>
[Table("ProductCategories")]
public class ProductCategory
{
    [ForeignKey(typeof(Product))]
    [Column("product_id")]
    public int ProductId { get; set; }

    [ForeignKey(typeof(Category))]
    [Column("category_id")]
    public int CategoryId { get; set; }
}
