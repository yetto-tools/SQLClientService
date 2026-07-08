using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models;
/// <summary>
/// Producto del catálogo. Declara <c>[ManyToMany(typeof(Category), typeof(ProductCategory))]</c> <br></br>
/// porque un producto puede estar en varias categorías (y una categoría agrupa varios <br></br>
/// productos) — ver <c>Example08_ManyToMany</c> en el proyecto de ejemplos.<br></br>
/// </summary>
[Table("Products")]
public class Product
{
    [PrimaryKey]
    [Column("product_id")]
    public int Id { get; set; }

    // Id interno (secuencial, usado para joins/índices). PublicId es lo que expondría una API
    // en su lugar, para que un cliente no pueda enumerar productos incrementando un entero.
    [Column("public_id")]
    public Guid PublicId { get; set; }

    [Column("product_name")]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [ManyToMany(typeof(Category), typeof(ProductCategory))]
    public List<Category> Categories { get; set; } = new();
}
