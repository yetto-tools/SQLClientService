
using DBSQLClient.Conexion;
using DBSQLClient.Helpers;
using DBSQLClient.Models;
using DBSQLClient.Servicio.Mapper.RelationsMapper;







namespace DBSQLClient;

public static class Program
{
    public static void Main()
    {
        var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Database=DB_TEST;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Application Name=\"SQL Server Management Studio\";Command Timeout=5000";

        var db = new SqlClientService(connectionString);

        Task.Run(async () =>
        {
            var result = await db.ExecuteAsync(
                "sp_User_With_Profile",
                SqlHelper.Params(("UserId", 1)));

            var user = result.MapOneToOne<User, UserProfile>("Profile");

            Console.WriteLine($"ID: {user.Id}");
            Console.WriteLine($"NAME: {user.Name}");
            Console.WriteLine($"EMAIL: {user.Email}");
            Console.WriteLine($"PROFILE BIO: {user.Profile?.Bio}");
            Console.WriteLine($"PROFILE BIRTHDATE: {user.Profile?.BirthDate}");
            Console.WriteLine("-- \n");
            Console.WriteLine($"{user.ToJsonString()}");

            Console.WriteLine("-- \n");
            var result2 = await db.ExecuteAsync("sp_User_With_Orders", SqlHelper.Params(("UserId", 1)));
            var userOrden = result2.MapOneToMany<User, Order>("Orders");
            Console.WriteLine($"{userOrden.ToJsonString()}");
        })
        .GetAwaiter()
        .GetResult();
    }
}
