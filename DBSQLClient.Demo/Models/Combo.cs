using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models;

/// <summary>
/// Bundle de variantes a un precio especial. Solo catálogo por ahora: no se puede agregar un<br></br>
/// combo al carrito todavía (<see cref="CartItem"/>/<see cref="OrderItem"/> siguen<br></br>
/// referenciando variantes individuales).
/// </summary>
[Table("Combos")]
public class Combo
{
    [PrimaryKey]
    [Column("combo_id")]
    [NotSerialized]
    public int Id { get; set; }

    [Column("public_id")]
    public Guid PublicId { get; set; }

    public string Name { get; set; } = null!;

    [Column("combo_price")]
    public decimal ComboPrice { get; set; }

    [Column("start_date")]
    [JsDateTime]
    public DateTime StartDate { get; set; }

    [Column("end_date")]
    [JsDateTime]
    public DateTime? EndDate { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [OneToMany(typeof(ComboItem))]
    public List<ComboItem> Items { get; set; } = new();
}
