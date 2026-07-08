using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models;

/// <summary>
/// Unifica promoción/descuento/oferta: son la misma idea (una regla que reduce el precio por un<br></br>
/// tiempo). Puede aplicar a una variante puntual o a toda una categoría — ver<br></br>
/// <c>sp_GetVariantEffectivePrice</c> para cómo se resuelve cuál gana cuando aplica más de uno.<br></br>
/// Solo columnas planas: nada en el demo mapea sus relaciones con <c>SqlResultMapper</c>, se<br></br>
/// consulta con <c>QueryAsync</c> + <c>ToList&lt;Discount&gt;()</c>.<br></br>
/// </summary>
[Table("Discounts")]
public class Discount
{
    [PrimaryKey]
    [Column("discount_id")]
    public int Id { get; set; }

    [Column("public_id")]
    public Guid PublicId { get; set; }

    public string Name { get; set; } = null!;

    [Column("discount_type")]
    public string DiscountType { get; set; } = null!;

    public decimal Value { get; set; }

    // Formato personalizado (solo fecha, sin hora) para mostrar que [JsDateTime] no está atado
    // al default ISO-8601 UTC -- Combo.StartDate/EndDate sí usan ese default, para contrastar.
    [Column("start_date")]
    [JsDateTime("yyyy-MM-dd")]
    public DateTime StartDate { get; set; }

    [Column("end_date")]
    [JsDateTime("yyyy-MM-dd")]
    public DateTime? EndDate { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }
}
