using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models;

[Table("Carts")]
public class Cart
{
    [PrimaryKey]
    [Column("cart_id")]
    public int Id { get; set; }

    [Column("public_id")]
    public Guid PublicId { get; set; }

    [ForeignKey(typeof(User))]
    [Column("user_id")]
    public int UserId { get; set; }

    [OneToMany(typeof(CartItem))]
    public List<CartItem> Items { get; set; } = new();
}
