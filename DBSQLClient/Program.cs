
using System.Data;
using DBSQLClient.Conexion;
using DBSQLClient.Helpers;
using DBSQLClient.Models;
using DBSQLClient.Servicio;
using DBSQLClient.Servicio.Mapper.RelationsMapper;

namespace DBSQLClient { 

    public static class Program
    {
        public static void Main()
        {
            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Database=DB_TEST;Integrated Security=True;Persist Security Info=False;Pooling=True;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Application Name=\"SQL Server Management Studio\";Command Timeout=5000";

            var db = new SqlClientService(connectionString);
            
            Task.Run(async () => {
                var result = await db.ExecuteAsync(
                    "sp_User_With_Profile",
                    SqlParams.AddParams(("UserId", 1)));

                // La propiedad de navegación ("Profile") se resuelve automáticamente
                // a partir de [OneToOne(typeof(UserProfile))] en User.
                var user = result.MapOneToOne<User, UserProfile>();

                Console.WriteLine($"ID: {user.Id}");
                Console.WriteLine($"NAME: {user.Name}");
                Console.WriteLine($"EMAIL: {user.Email}");
                Console.WriteLine($"PROFILE BIO: {user.Profile?.Bio}");
                Console.WriteLine($"PROFILE BIRTHDATE: {user.Profile?.BirthDate}");
                Console.WriteLine("-- \n");
                Console.WriteLine($"{user.ToJsonString()}");

                Console.WriteLine("-- \n");
                var result2 = await db.ExecuteAsync("sp_User_With_Orders", SqlParams.AddParams(("UserId", 1)));
                var userOrden = result2.MapOneToMany<User, Order>();
                Console.WriteLine($"{userOrden.ToJsonString()}");

                Console.WriteLine("-- \n");
                // Tabla 0 = Orders (muchos), tabla 1 = User (uno).
                // La propiedad "User" en Order se resuelve vía [ManyToOne(typeof(User))].
                var result3 = await db.ExecuteAsync("sp_Orders_With_User", SqlParams.AddParams(("UserId", 1)));
                var orders = result3.MapManyToOne<Order, User>();
                Console.WriteLine($"{orders.ToJsonString()}");

                Console.WriteLine("-- \n");
                // Tabla 0 = Users, tabla 1 = Roles, tabla 2 = UserRole (unión).
                // Las claves y la propiedad "Roles" se resuelven vía [ManyToMany]/[ForeignKey]/[PrimaryKey].
                var result4 = await db.ExecuteAsync("sp_Users_With_Roles", SqlParams.AddParams(("UserId", 1)));
                var usersWithRoles = result4.MapManyToMany<User, Role, UserRole>();
                Console.WriteLine($"{usersWithRoles.ToJsonString()}");

                Console.WriteLine("-- \n");
                // Procedimiento con parámetro de salida: se agrega con SqlParams.OutParam y se
                // combina con los de entrada. El valor se lee del resultado, no del arreglo original.
                var outputParams = SqlParams.AddParams(("UserId", 1))
                    .Append(SqlParams.OutParam("TotalOrders", SqlDbType.Int))
                    .ToArray();

                var result5 = await db.ExecuteAsync("sp_GetUserOrderTotal", outputParams);
                var totalOrders = result5.GetOutputValue<int>("TotalOrders");
                Console.WriteLine($"TOTAL ORDERS (output param): {totalOrders}");
            })
                .GetAwaiter()
                .GetResult(); 
        }
    }

}