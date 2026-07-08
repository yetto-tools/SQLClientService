using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models;

/// <summary>Un atributo (nombre + valor) de una <see cref="ProductVariant"/>: modelo EAV para
/// que cada variante tenga solo los atributos que le aplican (talla/color en ropa, talla/ancho
/// en calzado, ninguno en electrónica) sin columnas fijas por categoría.</summary>
[Table("ProductVariantAttributes")]
public class ProductVariantAttribute
{
    [PrimaryKey]
    [Column("variant_attribute_id")]
    public int Id { get; set; }

    [Column("public_id")]
    public Guid PublicId { get; set; }

    [ForeignKey(typeof(ProductVariant))]
    [Column("variant_id")]
    public int VariantId { get; set; }

    [Column("attribute_name")]
    public string AttributeName { get; set; } = null!;

    [Column("attribute_value")]
    public string AttributeValue { get; set; } = null!;
}
