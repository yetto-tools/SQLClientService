using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models;

/// <summary>
/// Pertenece a un usuario registrado o a un checkout de invitado, nunca a ambos: por eso declara<br></br>
/// dos <c>[ManyToOne]</c> independientes. <see cref="User"/>, <see cref="Guest"/> e<br></br>
/// <see cref="Invoice"/> son genuinamente nullable (no <c>= null!</c>): según qué SP se use, solo<br></br>
/// una de las dos referencias de dueño se resuelve, y no toda orden tiene factura todavía.
/// </summary>
[Table("Orders")]
public class Order
{
    [PrimaryKey]
    [Column("order_id")]
    public int Id { get; set; }

    [Column("public_id")]
    public Guid PublicId { get; set; }

    [Column("order_date")]
    [JsDateTime]
    public DateTime OrderDate { get; set; }

    public string Status { get; set; } = null!;

    public decimal Total { get; set; }

    [OneToMany(typeof(OrderItem))]
    public List<OrderItem> Items { get; set; } = new();

    [OneToOne(typeof(Invoice))]
    public Invoice? Invoice { get; set; }

    [ManyToOne(typeof(User))]
    public User? User { get; set; }

    [ManyToOne(typeof(GuestCheckout))]
    public GuestCheckout? Guest { get; set; }
}
