

using System.Data;
using System.Diagnostics;
using System.Text.Json;
using Test.Data;
using Test.Data.Infraestructura;


var db = new DBSqlClientService("Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Application Name=\"SQL Server Management Studio\";Command Timeout=0");

Task.Run(async () =>
{
    Stopwatch stopWatch0 = new Stopwatch();
    stopWatch0.Start();
    var Result0 = await db.QueryAsyncAsDataTable("select getdate()");

    stopWatch0.Stop();
    TimeSpan ts0 = stopWatch0.Elapsed;
    Console.WriteLine($"Tiempo ms: {ts0.TotalMilliseconds}");
}).Wait();



Task.Run(async () =>
{
    Stopwatch stopWatch = new Stopwatch();
    stopWatch.Start();
    var Result = await db.ExecuteAsyncAsDataSet("dbo.USP_TEST");

    stopWatch.Stop();
    TimeSpan ts = stopWatch.Elapsed;
    Console.WriteLine($"Tiempo ms: {ts.TotalMilliseconds}");

}).Wait();

Task.Run(async () =>
{
    Stopwatch stopWatch1 = new Stopwatch();
    stopWatch1.Start();

    var Result1 = await db.ExecuteStoredProcedureAsync<object>("dbo.USP_TEST");

    stopWatch1.Stop();
    TimeSpan ts1 = stopWatch1.Elapsed;
    Console.WriteLine($"Tiempo ms: {ts1.TotalMilliseconds}");
}).Wait();


Console.WriteLine("fin");