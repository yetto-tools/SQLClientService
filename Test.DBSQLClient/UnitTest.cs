using DBSQLClient.Conexion;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

namespace Test.DBSQLClient;

public class UnitTest
{
    /// <summary>
    /// Pruebas unitarias para SqlClientService
    /// </summary>
    public class SqlClientServiceTests : IDisposable
    {
        private readonly string _connectionString;
        private readonly SqlClientService _service;

        public SqlClientServiceTests()
        {
            // Usar una cadena de conexión de prueba
            _connectionString = "Server=localhost;Database=TestDB;Integrated Security=true;";
            _service = new SqlClientService(_connectionString);
        }

        public void Dispose()
        {
            // Limpieza si es necesaria
        }

        #region Pruebas de Constructor

        [Fact]
        public void Constructor_ConCadenaValida_DebeCrearInstancia()
        {
            // Act
            var service = new SqlClientService("Server=localhost;Database=TestDB;");

            // Assert
            service.Should().NotBeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_ConCadenaInvalida_DebeLanzarExcepcion(string connectionString)
        {
            // Act
            Action act = () => new SqlClientService(connectionString);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("*cadena de conexión*");
        }

        #endregion

        #region Pruebas de SqlParameters

        [Fact]
        public void SqlParameters_Constructor_DebeAsignarValores()
        {
            // Act
            var param = new SqlParameter("Id", 123);

            // Assert
            param.ParameterName.Should().Be("Id");
            param.Value.Should().Be(123);
            param.Direction.Should().Be(ParameterDirection.Input);
        }

        [Fact]
        public void SqlParameters_ConDbType_DebeAsignarTipo()
        {
            // Act
            var param = new SqlParameter("Name", SqlDbType.NVarChar) { Value = "Juan" };

            // Assert
            param.ParameterName.Should().Be("Name");
            param.Value.Should().Be("Juan");
            param.SqlDbType.Should().Be(SqlDbType.NVarChar);
        }

        [Fact]
        public void SqlParameters_ConDireccion_DebeAsignarDireccion()
        {
            // Act
            var param = new SqlParameter("OutputParam", SqlDbType.Int) { Direction = ParameterDirection.Output };

            // Assert
            param.Direction.Should().Be(ParameterDirection.Output);
        }

        #endregion

        #region Pruebas de SqlQueryResult

        [Fact]
        public void SqlQueryResult_AsDataSet_DebeRetornarDataSet()
        {
            // Arrange
            var dataSet = CreateSampleDataSet();
            var result = new SqlQueryResult(dataSet);

            // Act
            var returnedDataSet = result.AsDataSet();

            // Assert
            returnedDataSet.Should().NotBeNull();
            returnedDataSet.Tables.Count.Should().Be(1);
        }

        [Fact]
        public void SqlQueryResult_AsDataTable_DebeRetornarPrimeraTabla()
        {
            // Arrange
            var dataSet = CreateSampleDataSet();
            var result = new SqlQueryResult(dataSet);

            // Act
            var table = result.AsDataTable();

            // Assert
            table.Should().NotBeNull();
            table.Rows.Count.Should().Be(2);
            table.Columns.Count.Should().Be(3);
        }

        [Fact]
        public void SqlQueryResult_AsDataColumns_DebeRetornarColumnas()
        {
            // Arrange
            var dataSet = CreateSampleDataSet();
            var result = new SqlQueryResult(dataSet);

            // Act
            var columns = result.AsDataColumns();

            // Assert
            columns.Should().NotBeNull();
            columns.Count.Should().Be(3);
            columns[0].ColumnName.Should().Be("Id");
            columns[1].ColumnName.Should().Be("Name");
            columns[2].ColumnName.Should().Be("Email");
        }

        [Fact]
        public void SqlQueryResult_AsDataRows_DebeRetornarFilas()
        {
            // Arrange
            var dataSet = CreateSampleDataSet();
            var result = new SqlQueryResult(dataSet);

            // Act
            var rows = result.AsDataRows();

            // Assert
            rows.Should().NotBeNull();
            rows.Count.Should().Be(2);
        }

        [Fact]
        public void SqlQueryResult_AsEnumerable_DebePermitirLinq()
        {
            // Arrange
            var dataSet = CreateSampleDataSet();
            var result = new SqlQueryResult(dataSet);

            // Act
            var filteredRows = result.AsEnumerable()
                .Where(row => (int)row["Id"] > 1)
                .ToList();

            // Assert
            filteredRows.Should().HaveCount(1);
            filteredRows[0]["Name"].Should().Be("María");
        }

        [Fact]
        public void SqlQueryResult_ToList_DebeConvertirAObjetos()
        {
            // Arrange
            var dataSet = CreateSampleDataSet();
            var result = new SqlQueryResult(dataSet);

            // Act
            var users = result.ToList<TestUser>();

            // Assert
            users.Should().NotBeNull();
            users.Should().HaveCount(2);
            users[0].Id.Should().Be(1);
            users[0].Name.Should().Be("Juan");
            users[1].Id.Should().Be(2);
            users[1].Name.Should().Be("María");
        }

        [Fact]
        public void SqlQueryResult_FirstOrDefault_DebeRetornarPrimerObjeto()
        {
            // Arrange
            var dataSet = CreateSampleDataSet();
            var result = new SqlQueryResult(dataSet);

            // Act
            var user = result.FirstOrDefault<TestUser>();

            // Assert
            user.Should().NotBeNull();
            user.Id.Should().Be(1);
            user.Name.Should().Be("Juan");
        }

        [Fact]
        public void SqlQueryResult_FirstOrDefault_SinDatos_DebeRetornarNull()
        {
            // Arrange
            var emptyDataSet = new DataSet();
            emptyDataSet.Tables.Add(new DataTable());
            var result = new SqlQueryResult(emptyDataSet);

            // Act
            var user = result.FirstOrDefault<TestUser>();

            // Assert
            user.Should().BeNull();
        }

        [Fact]
        public void SqlQueryResult_RowCount_DebeRetornarNumeroFilas()
        {
            // Arrange
            var dataSet = CreateSampleDataSet();
            var result = new SqlQueryResult(dataSet);

            // Act
            var count = result.RowCount;

            // Assert
            count.Should().Be(2);
        }

        [Fact]
        public void SqlQueryResult_HasRows_DebeSerTrue_CuandoHayDatos()
        {
            // Arrange
            var dataSet = CreateSampleDataSet();
            var result = new SqlQueryResult(dataSet);

            // Act
            var hasRows = result.HasRows;

            // Assert
            hasRows.Should().BeTrue();
        }

        [Fact]
        public void SqlQueryResult_HasRows_DebeSerFalse_CuandoNoHayDatos()
        {
            // Arrange
            var emptyDataSet = new DataSet();
            emptyDataSet.Tables.Add(new DataTable());
            var result = new SqlQueryResult(emptyDataSet);

            // Act
            var hasRows = result.HasRows;

            // Assert
            hasRows.Should().BeFalse();
        }

        #endregion

        #region Pruebas de Serialización JSON

        [Fact]
        public void ToJson_DebeSerializarCorrectamente()
        {
            // Arrange
            var dataSet = CreateSampleDataSet();
            var result = new SqlQueryResult(dataSet);

            // Act
            var json = result.ToJson();

            // Assert
            json.Should().NotBeNullOrEmpty();
            json.Should().Contain("Juan");
            json.Should().Contain("María");
            json.Should().Contain("juan@email.com");
        }

        [Fact]
        public void ToJson_Tipado_DebeSerializarConTipo()
        {
            // Arrange
            var dataSet = CreateSampleDataSet();
            var result = new SqlQueryResult(dataSet);

            // Act
            var json = result.ToJson<TestUser>();

            // Assert
            json.Should().NotBeNullOrEmpty();
            var users = JsonSerializer.Deserialize<List<TestUser>>(json);
            users.Should().HaveCount(2);
            users[0].Name.Should().Be("Juan");
        }

        [Fact]
        public void ToJson_ConOpciones_DebeUsarOpciones()
        {
            // Arrange
            var dataSet = CreateSampleDataSet();
            var result = new SqlQueryResult(dataSet);
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Act
            var json = result.ToJson(options);

            // Assert
            json.Should().NotContain("\n"); // Sin indentación
            // ToJson(options) serializa un Dictionary<string, object?> por fila: PropertyNamingPolicy
            // de System.Text.Json solo aplica a propiedades de POCOs, no a claves de diccionario, así
            // que el nombre de columna original ("Id") se mantiene tal cual (no "id").
            json.Should().Contain("\"Id\"");
        }

        [Fact]
        public void ToJsonDataSet_ConMultiplesTablas_DebeSerializarTodas()
        {
            // Arrange
            var dataSet = new DataSet();
            var table1 = CreateSampleDataTable("Users");
            var table2 = new DataTable("Orders");
            table2.Columns.Add("OrderId", typeof(int));
            table2.Columns.Add("UserId", typeof(int));
            table2.Rows.Add(1, 1);

            dataSet.Tables.Add(table1);
            dataSet.Tables.Add(table2);
            var result = new SqlQueryResult(dataSet);

            // Act
            var json = result.ToJsonDataSet();

            // Assert
            json.Should().Contain("Users");
            json.Should().Contain("Orders");
        }

        [Fact]
        public void FromJson_DebeDeserializarCorrectamente()
        {
            // Arrange
            var json = @"[
                {""Id"": 1, ""Name"": ""Juan"", ""Email"": ""juan@email.com""},
                {""Id"": 2, ""Name"": ""María"", ""Email"": ""maria@email.com""}
            ]";

            // Act
            var users = SqlQueryResult.FromJson<TestUser>(json);

            // Assert
            users.Should().HaveCount(2);
            users[0].Name.Should().Be("Juan");
            users[1].Email.Should().Be("maria@email.com");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void FromJson_ConJsonVacio_DebeRetornarListaVacia(string json)
        {
            // Act
            var users = SqlQueryResult.FromJson<TestUser>(json);

            // Assert
            users.Should().BeEmpty();
        }

        [Fact]
        public void FromJsonToDataTable_DebeCrearDataTable()
        {
            // Arrange
            var json = @"[
                {""Id"": 1, ""Name"": ""Juan""},
                {""Id"": 2, ""Name"": ""María""}
            ]";

            // Act
            var table = SqlQueryResult.FromJsonToDataTable(json);

            // Assert
            table.Should().NotBeNull();
            table.Rows.Count.Should().Be(2);
            table.Columns.Count.Should().Be(2);
            table.Rows[0]["Name"].Should().Be("Juan");
        }

        [Fact]
        public async Task SaveToJsonFileAsync_DebeGuardarArchivo()
        {
            // Arrange
            var dataSet = CreateSampleDataSet();
            var result = new SqlQueryResult(dataSet);
            var filePath = "test_output.json";

            try
            {
                // Act
                await result.SaveToJsonFileAsync(filePath);

                // Assert
                System.IO.File.Exists(filePath).Should().BeTrue();
                var content = await System.IO.File.ReadAllTextAsync(filePath);
                content.Should().Contain("Juan");
            }
            finally
            {
                // Cleanup
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
        }

        [Fact]
        public async Task FromJsonFileAsync_DebeLeerArchivo()
        {
            // Arrange
            var filePath = "test_input.json";
            var json = @"[{""Id"": 1, ""Name"": ""Juan"", ""Email"": ""juan@email.com""}]";
            await System.IO.File.WriteAllTextAsync(filePath, json);

            try
            {
                // Act
                var users = await SqlQueryResult.FromJsonFileAsync<TestUser>(filePath);

                // Assert
                users.Should().HaveCount(1);
                users[0].Name.Should().Be("Juan");
            }
            finally
            {
                // Cleanup
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
        }

        [Fact]
        public async Task FromJsonFileAsync_ArchivoNoExiste_DebeLanzarExcepcion()
        {
            // Act
            Func<Task> act = async () => await SqlQueryResult.FromJsonFileAsync<TestUser>("archivo_inexistente.json");

            // Assert
            await act.Should().ThrowAsync<System.IO.FileNotFoundException>();
        }

        #endregion

        #region Pruebas de Manejo de Errores

        [Fact]
        public void QueryAsync_ConConsultaVacia_DebeLanzarExcepcion()
        {
            // Act
            Func<Task> act = async () => await _service.QueryAsync("");

            // Assert
            act.Should().ThrowAsync<ArgumentException>()
               .WithMessage("*consulta*");
        }

        [Fact]
        public void QueryAsync_ConConsultaNull_DebeLanzarExcepcion()
        {
            // Act
            Func<Task> act = async () => await _service.QueryAsync(null);

            // Assert
            act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public void ExecuteAsync_ConNombreVacio_DebeLanzarExcepcion()
        {
            // Act
            Func<Task> act = async () => await _service.ExecuteAsync("");

            // Assert
            act.Should().ThrowAsync<ArgumentException>()
               .WithMessage("*procedimiento almacenado*");
        }

        #endregion

        #region Pruebas de Cancelación

        [Fact]
        public async Task QueryAsync_ConCancelacionToken_DebePoderseCancelar()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel(); // Cancelar inmediatamente

            // Act
            Func<Task> act = async () => await _service.QueryAsync(
                "SELECT * FROM LargeTable",
                null,
                cts.Token
            );

            // Assert
            await act.Should().ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task ExecuteAsync_ConCancelacionToken_DebePoderseCancelar()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel(); // Cancelar inmediatamente

            // Act
            Func<Task> act = async () => await _service.ExecuteAsync(
                "sp_LongRunningProcedure",
                Array.Empty<SqlParameter>(),
                cts.Token
            );

            // Assert
            await act.Should().ThrowAsync<OperationCanceledException>();
        }

        #endregion

        #region Métodos Auxiliares

        private DataSet CreateSampleDataSet()
        {
            var dataSet = new DataSet();
            dataSet.Tables.Add(CreateSampleDataTable());
            return dataSet;
        }

        private DataTable CreateSampleDataTable(string tableName = "Users")
        {
            var table = new DataTable(tableName);
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Email", typeof(string));

            table.Rows.Add(1, "Juan", "juan@email.com");
            table.Rows.Add(2, "María", "maria@email.com");

            return table;
        }

        #endregion

        #region Clases de Prueba

        public class TestUser
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
        }

        #endregion
    }

    /// <summary>
    /// Pruebas de integración (requieren base de datos real)
    /// </summary>
    [Trait("Category", "Integration")]
    public class SqlClientServiceIntegrationTests : IDisposable
    {
        private readonly string _connectionString;
        private readonly SqlClientService _service;

        public SqlClientServiceIntegrationTests()
        {
            // Usar variable de entorno o configuración de prueba
            _connectionString = Environment.GetEnvironmentVariable("TEST_CONNECTION_STRING")
                ?? "Server=localhost;Database=TestDB;Integrated Security=true;";
            _service = new SqlClientService(_connectionString);
        }

        public void Dispose()
        {
            // Limpieza
        }

        [Fact(Skip = "Requiere base de datos real")]
        public async Task QueryAsync_ConsultaReal_DebeRetornarDatos()
        {
            // Arrange
            var query = "SELECT TOP 10 * FROM Users";

            // Act
            var result = await _service.QueryAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.HasRows.Should().BeTrue();
        }

        [Fact(Skip = "Requiere base de datos real")]
        public async Task QueryAsync_ConParametros_DebeEjecutarCorrectamente()
        {
            // Arrange
            var query = "SELECT * FROM Users WHERE Id = @Id";
            var parameters = new[]
            {
                new SqlParameter("Id", 1)
            };

            // Act
            var result = await _service.QueryAsync(query, parameters);

            // Assert
            result.Should().NotBeNull();
            result.HasRows.Should().BeTrue();
            var user = result.FirstOrDefault<SqlClientServiceTests.TestUser>();
            user.Id.Should().Be(1);
        }

        [Fact(Skip = "Requiere base de datos real")]
        public async Task ExecuteAsync_ProcedimientoAlmacenado_DebeEjecutar()
        {
            // Arrange
            var spName = "sp_GetUserById";
            var parameters = new[]
            {
                new SqlParameter("UserId", 1)
            };

            // Act
            var result = await _service.ExecuteAsync(spName, parameters);

            // Assert
            result.Should().NotBeNull();
            result.HasRows.Should().BeTrue();
        }

        [Fact(Skip = "Requiere base de datos real")]
        public async Task QueryAsync_ConTimeout_DebeRespetar()
        {
            // Arrange
            var query = "WAITFOR DELAY '00:00:05'; SELECT 1";
            var timeout = 2; // 2 segundos

            // Act
            Func<Task> act = async () => await _service.QueryAsync(query, null, timeout: timeout);

            // Assert
            await act.Should().ThrowAsync<SqlException>()
                .Where(ex => ex.Message.Contains("timeout") || ex.Message.Contains("tiempo"));
        }
    }
}
