using DBSQLClient.Servicio;

namespace DBSQLClient.Demo.Models
{
    [Table("Users")]
    public class User
    {
        [PrimaryKey]
        [Column("user_id")]
        public int Id { get; set; }

        [Column("user_name")]
        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        [OneToOne(typeof(UserProfile))]
        public UserProfile Profile { get; set; } = null!;

        [OneToMany(typeof(Order))]
        public List<Order> Orders { get; set; } = new();

        [ManyToMany(typeof(Role), typeof(UserRole))]
        public List<Role> Roles { get; set; } = new();

        // Calculada, no viene de la BD: [NotMapped] evita que el mapper intente
        // asignarle un valor (fallaría igual, al no tener setter).
        [NotMapped]
        public string DisplayName => $"{Name} <{Email}>";
    }

    [Table("UserProfiles")]
    public class UserProfile
    {
        [PrimaryKey]
        [Column("profile_id")]
        public int Id { get; set; }

        [ForeignKey(typeof(User))]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("bio")]
        public string Bio { get; set; } = null!;

        [Column("birth_date")]
        public DateTime? BirthDate { get; set; }
    }

    [Table("Roles")]
    public class Role
    {
        [PrimaryKey]
        [Column("role_id")]
        public int Id { get; set; }

        [Column("role_name")]
        public string Name { get; set; } = null!;
    }

    [Table("UserRoles")]
    public class UserRole
    {
        [ForeignKey(typeof(User))]
        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey(typeof(Role))]
        [Column("role_id")]
        public int RoleId { get; set; }
    }

    [Table("Orders")]
    public class Order
    {
        [PrimaryKey]
        [Column("order_id")]
        public int Id { get; set; }

        [ForeignKey(typeof(User))]
        [Column("user_id")]
        public int UserId { get; set; }

        public decimal Total { get; set; }

        public DateTime OrderDate { get; set; }

        [ManyToOne(typeof(User))]
        public User User { get; set; } = null!;
    }
}
