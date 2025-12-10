

using DBSQLClient.Models;
using DBSQLClient.Servicio.Conexion;
using System.Diagnostics;


var db = new SqlClientService("Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Application Name=\"SQL Server Management Studio\";Command Timeout=5000");

Task.Run(async () =>
{
    Stopwatch stopWatch0 = new();
    stopWatch0.Start();
    try
    {
        //var Consulta = await db.QueryAsync("SELECT * FROM ..spt_monitor; SELECT getdate() as fecha, 'test' as Mensaje ;");
        //var Respuesta = Consulta.AsDataTable();
        var spExecute = await db.ExecuteAsync("USP_TEST");
        Console.WriteLine($"{spExecute.AsDataSet().Tables.Count}");
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




