using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models;
/// <summary>
/// Detalle de variantes que componen un combo. <br></br>
/// Solo catálogo por ahora: no se puede agregar un combo <br></br>
/// al carrito todavía (<see cref="CartItem"/>/<see cref="OrderItem"/> siguen<br></br>
/// referenciando variantes individuales).
/// </summary>
/// <remarks>
/// <see cref="Id"/>, <see cref="ComboId"/> y <see cref="VariantId"/> son <c>[NotSerialized]</c>:
/// siguen mapeándose desde la base con normalidad, pero no viajan en el JSON — solo
/// <see cref="PublicId"/> se expondría hacia afuera para identificar este ítem.
/// </remarks>
[Table("ComboItems")]
public class ComboItem
{
    [PrimaryKey]
    [Column("combo_item_id")]
    [NotSerialized]
    public int Id { get; set; }

    [Column("public_id")]
    public Guid PublicId { get; set; }

    [ForeignKey(typeof(Combo))]
    [Column("combo_id")]
    [NotSerialized]
    public int ComboId { get; set; }

    [Column("variant_id")]
    [NotSerialized]
    public int VariantId { get; set; }

    public int Quantity { get; set; }
}
