

using DBSQLClient.Servicio.Conexion;
using System.Data;
using System.Diagnostics;
using System.Text.Json;



var db = new SqlClientService("Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Application Name=\"SQL Server Management Studio\";Command Timeout=5000");

Task.Run(async () =>
{
    Stopwatch stopWatch0 = new();
    stopWatch0.Start();
    try
    {
        var Result0 = await db.QueryAsync("select @@version as Version").AsDataSet();
        Console.WriteLine($"{Result0.Tables.Contains("Table1")}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }

    stopWatch0.Stop();
    TimeSpan ts0 = stopWatch0.Elapsed;
    Console.WriteLine($"<QueryAsyncAsDataTable> Tiempo ms: {ts0.TotalMilliseconds}");
}).Wait();

Task.Run(async () =>
{
    Stopwatch stopWatch4 = new();
    stopWatch4.Start();
    try
    {
        var Result0 = await db.ExecuteAsyncAsDataSet("dbo.USP_TEST");

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }

    stopWatch4.Stop();
    TimeSpan ts0 = stopWatch4.Elapsed;
    Console.WriteLine($"<ExecuteAsyncAsDataSet> Tiempo ms: {ts0.TotalMilliseconds}");
}).Wait();


Task.Run(async () =>
{
    Stopwatch stopWatch1 = new();
    stopWatch1.Start();

    var Result1 = await db.ExecuteStoredProcedureAsync<object>("dbo.USP_TEST");

    stopWatch1.Stop();
    TimeSpan ts1 = stopWatch1.Elapsed;
    Console.WriteLine($"<ExecuteStoredProcedureAsync> Tiempo ms: {ts1.TotalMilliseconds}");
}).Wait();




Console.WriteLine("fin");




