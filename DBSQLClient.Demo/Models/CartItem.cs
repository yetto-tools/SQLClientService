using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models;

[Table("CartItems")]
public class CartItem
{
    [PrimaryKey]
    [Column("cart_item_id")]
    public int Id { get; set; }

    [Column("public_id")]
    public Guid PublicId { get; set; }

    [ForeignKey(typeof(Cart))]
    [Column("cart_id")]
    public int CartId { get; set; }

    [Column("variant_id")]
    public int VariantId { get; set; }

    public int Quantity { get; set; }
}
