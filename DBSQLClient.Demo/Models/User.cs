using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models;

/// <summary>Cliente registrado. No necesita atributos de relación: es el lado "padre"
/// de un <c>[ManyToOne]</c> declarado en <see cref="Order"/>.</summary>
[Table("Users")]
public class User
{
    [PrimaryKey]
    [Column("user_id")]
    [NotSerialized]
    public int Id { get; set; }

    [Column("public_id")]
    public Guid PublicId { get; set; }

    [Column("full_name")]
    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    // Se mapea desde la BD como cualquier otra columna, pero [NotSerialized] evita que
    // aparezca en el JSON expuesto por ToJsonString/ToJson<T> (a diferencia de [NotMapped],
    // que además dejaría de leerlo de la base).
    [Column("password_hash")]
    [NotSerialized]
    public string? PasswordHash { get; set; }
}
