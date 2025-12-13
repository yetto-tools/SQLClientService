using DBSQLClient.Models;
using DBSQLClient.Servicio.Conexion;
using DBSQLClient.Servicio.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;





namespace DBSQLClient
{
    public static class program
    {
        public static void Main()
        {

            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Application Name=\"SQL Server Management Studio\";Command Timeout=5000";

            var db = new SqlClientService(connectionString);

            db.Execute("SP_INSERTAR_PERSONA",
                new SqlParameter[]
                {
                    new SqlParameter("@NOMBRE", "JUAN"),
                    new SqlParameter("@APELLIDO", "PEREZ"),
                    new SqlParameter("@EDAD", 30)
                });


        }
    }
}
