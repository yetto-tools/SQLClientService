using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models;

/// <summary>Checkout como invitado: sin cuenta ni password. Una <see cref="Order"/> pertenece
/// a un <see cref="User"/> o a un <see cref="GuestCheckout"/>, nunca a ambos.</summary>
[Table("GuestCheckouts")]
public class GuestCheckout
{
    [PrimaryKey]
    [Column("guest_id")]
    public int Id { get; set; }

    [Column("public_id")]
    public Guid PublicId { get; set; }

    [Column("full_name")]
    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;
}
