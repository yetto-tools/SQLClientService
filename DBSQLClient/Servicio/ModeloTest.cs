
using DBSQLClient.Servicio;



namespace DBSQLClient.Models
{
    public class User
    {
        [PrimaryKey]
        [Column("user_id")]
        public int Id { get; set; }

        [Column("user_name")]
        public string Name { get; set; }

        public string Email { get; set; }

        [OneToOne(typeof(UserProfile))]
        public UserProfile Profile { get; set; }

        [OneToMany(typeof(Order))]
        public List<Order> Orders { get; set; } = new();

        [ManyToMany(typeof(Role), typeof(UserRole))]
        public List<Role> Roles { get; set; } = new();
    }

    public class UserProfile
    {
        [PrimaryKey]
        [Column("profile_id")]
        public int Id { get; set; }

        [ForeignKey(typeof(User))]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("bio")]
        public string Bio { get; set; }

        [Column("birth_date")]
        public DateTime? BirthDate { get; set; }
    }

    public class Role
    {
        [PrimaryKey]
        [Column("role_id")]
        public int Id { get; set; }

        [Column("role_name")]
        public string Name { get; set; }
    }

    public class UserRole
    {
        [ForeignKey(typeof(User))]
        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey(typeof(Role))]
        [Column("role_id")]
        public int RoleId { get; set; }
    }

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
        public User User { get; set; }
    }
}
