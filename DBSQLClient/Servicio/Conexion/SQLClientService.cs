using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace DBSQLClient.Servicio.Conexion
{
    /// <resumen>
    /// Proporciona métodos para ejecutar consultas SQL y procedimientos almacenados de forma asíncrona en una base de datos SQL Server,
    /// devolviendo los resultados como DataTable, DataSet u objetos mapeados.
    /// </resumen>
    /// <observaciones>Este servicio gestiona las conexiones SQL Server y admite consultas parametrizadas, tiempos de espera de comandos
    /// y tokens de cancelación para operaciones asíncronas. Está pensado para situaciones en las que se requiere un acceso directo y
    /// de bajo nivel a SQL Server, como la ejecución de consultas ad hoc o procedimientos almacenados y la recuperación de
    /// resultados en varios formatos. El servicio no gestiona el agrupamiento de conexiones más allá de lo que proporciona ADO.NET.
    /// No se garantiza la seguridad de los subprocesos; cree una instancia independiente por cada operación simultánea si es necesario.</remarks>
    public class SqlClientService : ISQLClientService
    {

        private readonly string _connectionString;
        

        /// <summary>
        /// Inicializa una nueva instancia de la clase DBSqlClientService utilizando la cadena de conexión especificada.
        /// </summary>
        /// <param name="connectionString">La cadena de conexión utilizada para establecer una conexión con la base de datos. No puede ser nula ni estar vacía.</param>
        public SqlClientService(string connectionString)
        {
            try
            {
                // Verifica que la cadena de conexión no sea nula ni esté vacía.
                _connectionString = connectionString;
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException($"La cadena de conexión no puede ser nula. {nameof(connectionString)}", ex);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("La cadena de conexión no puede estar vacía.", nameof(connectionString), ex);
            }
        }


        /// <summary>
        /// Obtiene de forma asíncrona una conexión abierta a la base de datos SQL para su uso en operaciones con la base de datos.
        /// </summary>
        /// <remarks>Si ya se ha establecido una conexión y está abierta, se devuelve la misma instancia.
        /// De lo contrario, se crea y se abre una nueva conexión. El autor de la llamada es responsable de garantizar la seguridad del subproceso
        /// al utilizar la conexión devuelta.</remarks>
        /// <returns>Una instancia <see cref="SqlConnection"/> que está abierta y lista para comandos de base de datos.</returns>
        /// <exception cref="InvalidOperationException">Se lanza cuando se produce un error al abrir la conexión SQL.</exception>
        private SqlConnection GetConnection()
        {

            try
            {
                return new SqlConnection(_connectionString);           
            }
            catch (SqlException ex)
            {
                // Si ocurre un error al abrir la conexión, lanza una excepción con el mensaje de error.
                throw new InvalidOperationException($"Error al abrir la conexión SQL: {ex.Message}", ex);
            } 

        }

        /// <summary>
        /// Ejecuta la consulta SQL especificada de forma asíncrona y devuelve los resultados como una tabla de datos (DataTable).
        /// </summary>
        /// <param name="sqlCommand">La consulta SQL que se va a ejecutar. No puede ser nula ni estar vacía.</param>
        /// <param name="parameters">Una Array de objetos SqlParameter que se aplicarán a la consulta SQL, o nula si no se requieren parámetros.</param>
        /// <param name="ct">Un CancellationToken que se puede utilizar para cancelar la operación asíncrona.</param>
        /// <param name="Timeout">El tiempo de espera del comando, en segundos. Si se establece en 0, se utiliza el tiempo de espera predeterminado.</param>
        /// <returns>Una tabla de datos que contiene los resultados de la consulta SQL. La tabla de datos estará vacía si la consulta no devuelve ninguna
        /// fila.</returns>
        /// <exception cref="InvalidOperationException">Se lanza si se produce un error al ejecutar la consulta SQL.</exception>
        public async Task<DataTable> QueryAsyncAsDataTable(
            [Required] string sqlCommand,
            SqlParameter[]? parameters = default,
            CancellationToken ct = default,
            int Timeout = default)
        {
            // Almacener la informacion que se recupere en un DataTable.
            using var dtResultSet = new DataTable();

            try
            {
                // Obtiene una conexión abierta a la base de datos SQL.
                using (var conn = GetConnection())
                {
                    await conn.OpenAsync(ct);
                    // Asegura que la conexión esté abierta antes de ejecutar el comando
                    await using var cmd = new SqlCommand(sqlCommand, conn)
                    {
                        CommandType = CommandType.Text,
                        CommandTimeout = Timeout > 0 ? Timeout : 30 // Default timeout is 30 seconds
                    };

                    // Si se proporcionan parámetros, agréguelos al comando
                    if (parameters != default)
                        cmd.Parameters.AddRange(parameters);
                    // Ejecuta el comando de forma asíncrona y llena un DataTable con los resultados
                    using var reader = await cmd.ExecuteReaderAsync();

                    // Lee los resultados del comando SQL y los carga en el DataTable.
                    while (!reader.IsClosed)
                        dtResultSet.Load(reader);
                }
                
            }
            catch (SqlException ex)
            {
                // Si ocurre un error SQL, lanza una excepción con el mensaje de error
                throw new InvalidOperationException($"Error SQL >> {ex.Message} ", ex);
            }

            // Devuelve el DataTable con los resultados de la consulta SQL
            return dtResultSet;
        }




        /// <summary>
        /// Ejecuta el comando SQL especificado de forma asíncrona y devuelve los resultados como un DataSet.
        /// </summary>
        /// <remarks>Cada conjunto de resultados devuelto por el comando SQL se carga en una tabla de datos independiente
        /// dentro del conjunto de datos. El método abre y cierra la conexión a la base de datos automáticamente.</remarks>
        /// <param name="sqlCommand">La consulta SQL que se va a ejecutar. No puede ser nulo ni estar vacía.
        /// <param name="parameters">Una matriz de parámetros SQL que se aplicarán al comando, o nulo para ejecutar sin parámetros.
        /// <param name="ct">Un token de cancelación que se puede utilizar para cancelar la operación asíncrona.
        /// <param name="Timeout">El tiempo de espera del comando, en segundos. Si se establece en 0 o menos, se utiliza el tiempo de espera predeterminado de 30 segundos.</param>
        /// <returns>Un conjunto de datos que contiene los conjuntos de resultados devueltos por el comando SQL. El conjunto de datos estará vacío si la consulta
        /// no devuelve ningún resultado.</returns>
        /// <exception cref="InvalidOperationException">Se lanza si se produce un error al ejecutar el comando SQL.</exception>
        public async Task<DataSet> ExecuteAsyncAsDataSet(
            [Required] string sqlCommand, SqlParameter[]? parameters = default, CancellationToken ct = default, int Timeout = default)
        {
            // Para Almacenar los resultados de la consulta SQL en un DataSet.
            using var dsResultSet = new DataSet();
            try
            {
                // Obtiene una conexión abierta a la base de datos SQL.
                using (var conn = GetConnection())
                {
                    // Asegura que la conexión esté abierta antes de ejecutar el comando
                    await conn.OpenAsync(ct);

                    await using var cmd = new SqlCommand(sqlCommand, conn)
                    {
                        CommandType = CommandType.Text,
                        CommandTimeout = Timeout > 0 ? Timeout : 30 // Default timeout is 30 seconds
                    };
                    // Si se proporcionan parámetros, agréguelos al comando
                    if (parameters != default)
                        cmd.Parameters.AddRange(parameters);

                    // Ejecuta el comando de forma asíncrona y llena un DataSet con los resultados
                    using var reader = await cmd.ExecuteReaderAsync(ct);
                    // Lee los resultados del comando SQL y los carga en el DataSet.
                    while (!reader.IsClosed)
                        dsResultSet.Tables.Add().Load(reader);
                }
                

            }
            catch (SqlException ex)
            {
                // Si ocurre un error SQL, lanza una excepción con el mensaje de error
                throw new InvalidOperationException($"Error SQL: {ex.Message}", ex);
            }
            return dsResultSet;
        }


        /// <summary>
        /// Ejecuta de forma asíncrona un procedimiento almacenado y devuelve los resultados como un conjunto de datos (DataSet).
        /// </summary>
        /// <param name="nameStoredProcedure">El nombre del procedimiento almacenado que se va a ejecutar. No puede ser nulo ni estar vacío.</param>
        /// <param name="parameters">Una Array de objetos SqlParameter que se pasarán al procedimiento almacenado, o nulo para ejecutar sin parámetros.</param>
        /// <param name="ct">Un CancellationToken que se puede utilizar para cancelar la operación asíncrona.</param>
        /// <param name="Timeout">El tiempo de espera del comando, en segundos. Si se establece en 0, se utiliza el tiempo de espera predeterminado.</param>
        /// <returns>Un DataSet que contiene los conjuntos de resultados devueltos por el procedimiento almacenado. El DataSet estará vacío si el
        /// procedimiento almacenado no devuelve ningún resultado.</returns>
        public async Task<DataSet> ExecuteStoredProcedureAsync(
            string nameStoredProcedure,
            SqlParameter[]? parameters = default,
            CancellationToken ct = default,
            int Timeout = default)
        {
            //
            using var results = new DataSet();
            try
            {
                // Verifica que el nombre del procedimiento almacenado no sea nulo ni esté vacío.
                using (var conn = GetConnection())
                {
                    // Asegura que la conexión esté abierta antes de ejecutar el comando
                    await conn.OpenAsync(ct);

                    await using var cmd = new SqlCommand(nameStoredProcedure, conn)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = Timeout > 0 ? Timeout : 30 // Default timeout is 30 seconds
                    };

                    // Si se proporcionan parámetros, agréguelos al comando
                    if (parameters != default)
                        cmd.Parameters.AddRange(parameters);

                    // Crea un DataSet para almacenar los resultados del procedimiento almacenado.
                    using (var reader = await cmd.ExecuteReaderAsync(ct)) 
                    {
                        // Lee los resultados del procedimiento almacenado y los carga en el DataSet.
                        while (!reader.IsClosed)
                            results.Tables.Add().Load(reader);
                    }

                }



            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al ejecutar el procedimiento almacenado: {ex.Message}", ex);
            }

            return results;
        }


        /// <summary>
        /// Executes a stored procedure asynchronously and maps the result to the specified type.
        /// </summary>
        /// <remarks>If T is DataSet, the entire result set is returned. If T is DataTable, only the first
        /// result table is returned. If T is DataRow, only the first row of the first result table is returned. For
        /// scalar types, the value of the first column of the first row is returned. If no data is available, null is
        /// returned. The method does not automatically map complex types; for custom mapping, additional processing may
        /// be required.</remarks>
        /// <typeparam name="T">The type to which the result of the stored procedure will be mapped. Supported types are DataSet, DataTable,
        /// DataRow, or a scalar type corresponding to the first column of the first row in the result set.</typeparam>
        /// <param name="nameStoredProcedure">The name of the stored procedure to execute. Cannot be null or empty.</param>
        /// <param name="parameters">An array of SQL parameters to pass to the stored procedure, or null if no parameters are required.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <param name="Timeout">The command timeout, in seconds. If set to 0 or less, the default timeout of 30 seconds is used.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the mapped result of the stored
        /// procedure as type T, or null if no data is returned.</returns>
        /// <exception cref="InvalidOperationException">Thrown if a SQL error occurs during execution of the stored procedure.</exception>
        public async Task<T?> ExecuteStoredProcedureAsync<T>(
            string nameStoredProcedure,
            SqlParameter[]? parameters = default,
            CancellationToken ct = default,
            int Timeout = default)
        {
            // Mapear a Dataset segun el <T> generico solicitado
            object? mapped = null;
            try
            {

                using (var conn = GetConnection())
                {
                    await conn.OpenAsync(ct);
                    await using var cmd = new SqlCommand(nameStoredProcedure, conn)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = Timeout > 0 ? Timeout : 30 // Default timeout is 30 seconds
                    };

                    if (parameters != default)
                        cmd.Parameters.AddRange(parameters);

                    var results = new DataSet();

                    // Use an async reader and populate one DataTable per result set.
                    await using var reader = await cmd.ExecuteReaderAsync(ct);

                    do
                    {
                        if (reader.FieldCount > 0)
                        {
                            var dataTable = new DataTable();

                            // Crear Columnas desde el esquema del Reader (Asegurar que sea nombre unico por columnas )
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                Type colType;
                                try
                                {
                                    colType = reader.GetFieldType(i);
                                }
                                catch
                                {
                                    colType = typeof(object);
                                }

                                var rawName = reader.GetName(i);
                                var baseName = string.IsNullOrEmpty(rawName) ? $"Column{i}" : rawName;
                                var uniqueName = GetUniqueColumnName(dataTable, baseName);

                                dataTable.Columns.Add(new DataColumn(uniqueName, colType));
                            }

                            // leer filasa de forma asincrona y añador a las tablas
                            var values = new object[reader.FieldCount];
                            while (await reader.ReadAsync(ct))
                            {
                                reader.GetValues(values);
                                var rowValues = new object[values.Length];
                                Array.Copy(values, rowValues, values.Length);
                                dataTable.Rows.Add(rowValues);
                            }

                            results.Tables.Add(dataTable);
                        }
                    } while (await reader.NextResultAsync(ct));


                    if (typeof(T) == typeof(DataSet))
                    {
                        mapped = results;
                    }
                    else if (typeof(T) == typeof(DataTable))
                    {
                        mapped = results.Tables.Count > 0 ? results.Tables[0] : new DataTable();
                    }
                    else if (typeof(T) == typeof(DataRow))
                    {
                        mapped = (results.Tables.Count > 0 && results.Tables[0].Rows.Count > 0) ? results.Tables[0].Rows[0] : null;
                    }
                    else
                    {
                        // si es un tipo scalaer el generico entoces se toma primer valor de la tabla y primera fila 
                        if (results.Tables.Count > 0 && results.Tables[0].Rows.Count > 0 && results.Tables[0].Columns.Count > 0)
                        {
                            var val = results.Tables[0].Rows[0][0];
                            if (val == DBNull.Value)
                                mapped = null;
                            else
                            {
                                var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                                try
                                {
                                    if (targetType.IsInstanceOfType(val))
                                        mapped = val;
                                    else
                                        mapped = Convert.ChangeType(val, targetType);
                                }
                                catch
                                {
                                    //  intentar cambiar mediante la serialización System.Text.Json para tipos complejo
                                    try
                                    {
                                        var json = System.Text.Json.JsonSerializer.Serialize(val);
                                        mapped = System.Text.Json.JsonSerializer.Deserialize(json, targetType);
                                    }
                                    catch
                                    {
                                        mapped = null;
                                    }
                                }
                            }
                        }
                    }

                }

            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"Error SQL: {ex.Message}", ex);
            }

            return (T?)mapped;
        }

        /// <summary>
        /// Creates a fluent query operation so callers can write:
        ///   await db.QueryAsync("select ...").AsDataTable();
        /// </summary>
        /// <param name="sqlCommand">SQL text to execute (required)</param>
        /// <param name="parameters">Optional SQL parameters</param>
        /// <param name="ct">Optional cancellation token</param>
        /// <param name="Timeout">Optional command timeout in seconds</param>
        /// <returns>A query operation with AsDataTable/AsDataSet/As{T} helpers.</returns>
        public QueryOperation QueryAsync(
            [Required] string sqlCommand,
            SqlParameter[]? parameters = default,
            CancellationToken ct = default,
            int Timeout = default)
        {
            if (string.IsNullOrWhiteSpace(sqlCommand))
                throw new ArgumentException("sqlCommand cannot be null or empty.", nameof(sqlCommand));

            return new QueryOperation(this, sqlCommand, parameters, ct, Timeout);
        }

        /// <summary>
        /// Helper returned by <see cref="QueryAsync(string, SqlParameter[], CancellationToken, int)"/>.
        /// Provides async mapping methods such as AsDataTable() and AsDataSet().
        /// </summary>
        public sealed class QueryOperation
        {
            private readonly SqlClientService _service;
            private readonly string _sql;
            private readonly SqlParameter[]? _parameters;
            private readonly CancellationToken _ct;
            private readonly int _timeout;

            internal QueryOperation(SqlClientService service, string sql, SqlParameter[]? parameters, CancellationToken ct, int timeout)
            {
                _service = service;
                _sql = sql;
                _parameters = parameters;
                _ct = ct;
                _timeout = timeout;
            }

            /// <summary>
            /// Ejecuta la consulta y devuelve el primer DataTable como resultado.
            /// Uso: await db.QueryAsync("select ...").AsDataTable();
            /// </summary>
            public Task<DataTable> AsDataTable()
                => _service.QueryAsyncAsDataTable(_sql, _parameters, _ct, _timeout);

            /// <summary>
            /// Ejecuta la consulta y devuelve un DataSet con todos los result sets.
            /// Uso: await db.QueryAsync("select ...").AsDataSet();
            /// </summary>
            public Task<DataSet> AsDataSet()
                => _service.ExecuteAsyncAsDataSet(_sql, _parameters, _ct, _timeout);

            /// <summary>
            /// Ejecuta la consulta y mapea el resultado al tipo T.
            /// Comportamiento igual que ExecuteStoredProcedureAsync{T} respecto a DataSet/DataTable/DataRow/scalar.
            /// Uso: await db.QueryAsync("select ...").As{T}();
            /// </summary>
            public async Task<T?> As<T>()
            {
                object? mapped = null;

                var results = await _service.ExecuteAsyncAsDataSet(_sql, _parameters, _ct, _timeout).ConfigureAwait(false);

                if (typeof(T) == typeof(DataSet))
                {
                    mapped = results;
                }
                else if (typeof(T) == typeof(DataTable))
                {
                    mapped = results.Tables.Count > 0 ? results.Tables[0] : new DataTable();
                }
                else if (typeof(T) == typeof(DataRow))
                {
                    mapped = (results.Tables.Count > 0 && results.Tables[0].Rows.Count > 0) ? results.Tables[0].Rows[0] : null;
                }
                else
                {
                    if (results.Tables.Count > 0 && results.Tables[0].Rows.Count > 0 && results.Tables[0].Columns.Count > 0)
                    {
                        var val = results.Tables[0].Rows[0][0];
                        if (val == DBNull.Value)
                            mapped = null;
                        else
                        {
                            var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                            try
                            {
                                if (targetType.IsInstanceOfType(val))
                                    mapped = val;
                                else
                                    mapped = Convert.ChangeType(val, targetType);
                            }
                            catch
                            {
                                try
                                {
                                    var json = System.Text.Json.JsonSerializer.Serialize(val);
                                    mapped = System.Text.Json.JsonSerializer.Deserialize(json, targetType);
                                }
                                catch
                                {
                                    mapped = null;
                                }
                            }
                        }
                    }
                }

                return (T?)mapped;
            }
        }

        /// <summary>
        /// Generates a unique column name for the specified DataTable based on the provided base name.
        /// </summary>
        /// <param name="table">The DataTable in which to ensure the uniqueness of the column name. Cannot be null.</param>
        /// <param name="baseName">The base name to use when generating the unique column name. Cannot be null or empty.</param>
        /// <returns>A column name that is unique within the Columns collection of the specified DataTable. If the base name is
        /// not already used, it is returned as-is; otherwise, a numeric suffix is appended to create a unique name.</returns>
        private static string GetUniqueColumnName(DataTable table, string baseName)
        {
            if (!table.Columns.Contains(baseName))
                return baseName;

            var suffix = 1;
            string candidate;
            do
            {
                candidate = $"{baseName}_{suffix++}";
            } while (table.Columns.Contains(candidate));

            return candidate;
        }


    }
}
