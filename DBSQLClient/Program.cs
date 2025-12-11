

using System.Data;
using System.Diagnostics;
using System.Text.Json;
using DBSQLClient.Models;
using DBSQLClient.Servicio.Conexion;
using DBSQLClient.Servicio.Helpers;
using Microsoft.Data.SqlClient;




var connectionString1= "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Application Name=\"SQL Server Management Studio\";Command Timeout=5000";




var connectionString = "Server=84.46.252.55,1460;Database=dssoluti_db_demo;User Id=dssoluti_usr_demo;Password=039@8w\\CJPw?;TrustServerCertificate=True;";


var db = new SqlClientService(connectionString);

Task.Run(async () =>
{
    Stopwatch stopWatch0 = new();
    stopWatch0.Start();
    try
    {


        var parametros = SqlHelper.Params(
            ("pN1", 1),
            ("pN2", 0),
            ("pN3", 0)
            );


        //var parametros = new SqlParameterBuilder()
        //    .AddInt("pN1", 1)
        //    .AddInt("pN2", 0)
        //    .AddInt("pN3", 0);

        var MenuOpcion1 = new { pN1 = 1, pN2 = 0, pN3 = 0 };

        //var parametros = SqlHelper.FromObject(MenuOpcion1);

        var spExecute = await db.ExecuteAsync("sp_getArticulos", parametros);
        
        Console.WriteLine($"{spExecute.AsDataSet()}");

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }

    stopWatch0.Stop();
    TimeSpan ts0 = stopWatch0.Elapsed;
    Console.WriteLine($"<ExecuteAsync> Tiempo ms: {ts0.TotalMilliseconds}");
}).Wait();

Task.Run(async () =>
{
    Stopwatch stopWatch0 = new();
    stopWatch0.Start();
    try
    {
        var Consulta = await db.QueryAsync("SELECT * FROM ..spt_monitor; SELECT getdate() as fecha, 'test' as Mensaje ;");        
        Console.WriteLine($"{Consulta.AsDataSet().Tables.Count}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }

    stopWatch0.Stop();
    TimeSpan ts0 = stopWatch0.Elapsed;
    Console.WriteLine($"<QueryAsync> Tiempo ms: {ts0.TotalMilliseconds}");

}).Wait();

Task.Run(async () =>
{
    Stopwatch stopWatch = new();
    stopWatch.Start();
    try
    {
        var UserList = await db.ExecuteAsync("sp_Get_User");

        var json =  UserList.ToJson<UserModel>(options: new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase } );

        Console.WriteLine(json);
        
        // Convertir a DataSet
        UserList.AsDataSet();

        // Filtrar con LINQ

        var userEnuModel =  UserList.ToList<UserModel>().Where( x => x.Id == 1 );
                
        // Convertir a objetos tipados
        var user = UserList.FirstOrDefault<UserModel>();

        Console.WriteLine($"ID: {user?.Id}, Nombre: {user?.Nombre}, Correo: {user?.Correo}");   

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }

    stopWatch.Stop();
    TimeSpan ts0 = stopWatch.Elapsed;
    Console.WriteLine($"<ExecuteAsync> Tiempo ms: {ts0.TotalMilliseconds}");
}).Wait();





Console.WriteLine("fin");




