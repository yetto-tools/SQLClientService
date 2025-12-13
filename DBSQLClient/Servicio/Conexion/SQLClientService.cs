using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;


namespace DBSQLClient.Servicio.Conexion
{
    #region Interfaces

    #endregion

    #region Clases de Soporte


    /// <summary>
    /// Resultado de una consulta SQL que proporciona múltiples formatos de salida.
    /// </summary>
    public class SqlQueryResult
    {
        private readonly DataSet _Data_Result;
        private static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        internal SqlQueryResult(DataSet dataSet)
        {
            _Data_Result = dataSet.Copy() ?? new DataSet();
        }



        /// <summary>
        /// Devuelve el resultado como DataSet completo.
        /// </summary>
        public DataSet AsDataSet() => _Data_Result;

        /// <summary>
        /// Devuelve la primera tabla del resultado como DataTable.
        /// </summary>
        public DataTable AsDataTable()
        {
            return _Data_Result.Tables.Count > 0 ? _Data_Result.Tables[0] : new DataTable();
        }

        /// <summary>
        /// Devuelve la tabla en el índice especificado como DataTable.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public DataTable AsDataTable(int index)
        {
            if(index < 0 || index >= _Data_Result.Tables.Count)
                throw new IndexOutOfRangeException("El índice de la tabla está fuera de los límites.");
            
            return _Data_Result.Tables.Count > 0 ? _Data_Result.Tables[index] : new DataTable();
        }

        /// <summary>
        /// Devuelve las columnas de la primera tabla.
        /// </summary>
        public DataColumnCollection AsDataColumns()
        {
            return AsDataTable().Columns;
        }

        /// <summary>
        /// Devuelve las filas de la primera tabla.
        /// </summary>
        public DataRowCollection AsDataRows()
        {
            return AsDataTable().Rows;
        }

        /// <summary>
        /// Devuelve el resultado como una colección enumerable de DataRow.
        /// </summary>
        public IEnumerable<DataRow> AsEnumerable()
        {
            return AsDataTable().AsEnumerable();
        }

        /// <summary>
        /// Convierte el resultado a una lista de objetos del tipo especificado.
        /// </summary>
        public List<T> ToList<T>() where T : new()
        {
            var table = AsDataTable();
            var list = new List<T>();
            var properties = typeof(T).GetProperties();

            foreach (DataRow row in table.Rows)
            {
                var obj = new T();
                foreach (var prop in properties)
                {
                    if (table.Columns.Contains(prop.Name) && row[prop.Name] != DBNull.Value)
                    {
                        prop.SetValue(obj, Convert.ChangeType(row[prop.Name], prop.PropertyType));
                    }
                }
                list.Add(obj);
            }

            return list;
        }

        /// <summary>
        /// Devuelve el primer objeto del tipo especificado o null si no hay resultados.
        /// </summary>
        public T? FirstOrDefault<T>() where T : new()
        {
            return ToList<T>().FirstOrDefault();
        }

        /// <summary>
        /// Devuelve el número de filas afectadas.
        /// </summary>
        public int RowCount => AsDataTable().Rows.Count;

        /// <summary>
        /// Indica si el resultado tiene filas.
        /// </summary>
        public bool HasRows => RowCount > 0;

        #region Métodos de Serialización JSON

        /// <summary>
        /// Serializa el DataTable a formato JSON.
        /// </summary>
        /// <param name="options">Opciones de serialización JSON (opcional).</param>
        /// <returns>Cadena JSON representando los datos.</returns>
        public string ToJson(JsonSerializerOptions? options = null)
        {
            var table = AsDataTable();
            var rows = new List<Dictionary<string, object?>>();

            foreach (DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object?>();
                foreach (DataColumn col in table.Columns)
                {
                    dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                }
                rows.Add(dict);
            }

            return JsonSerializer.Serialize(rows, options ?? DefaultJsonOptions);
        }

        /// <summary>
        /// Serializa el resultado a JSON como una lista de objetos tipados.
        /// </summary>
        /// <typeparam name="T">Tipo de objeto a serializar.</typeparam>
        /// <param name="options">Opciones de serialización JSON (opcional).</param>
        /// <returns>Cadena JSON representando la lista de objetos.</returns>
        public string ToJson<T>(JsonSerializerOptions? options = null) where T : new()
        {
            var list = ToList<T>();
            return JsonSerializer.Serialize(list, options ?? DefaultJsonOptions);
        }

        /// <summary>
        /// Serializa todas las tablas del DataSet a JSON.
        /// </summary>
        /// <param name="options">Opciones de serialización JSON (opcional).</param>
        /// <returns>Diccionario con el nombre de la tabla como clave y los datos en JSON como valor.</returns>
        public string ToJsonDataSet(JsonSerializerOptions? options = null)
        {
            var dataSetDict = new Dictionary<string, List<Dictionary<string, object?>>>();

            foreach (DataTable table in _Data_Result.Tables)
            {
                var rows = new List<Dictionary<string, object?>>();
                foreach (DataRow row in table.Rows)
                {
                    var dict = new Dictionary<string, object?>();
                    foreach (DataColumn col in table.Columns)
                    {
                        dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                    }
                    rows.Add(dict);
                }
                dataSetDict[string.IsNullOrEmpty(table.TableName) ? $"Table{_Data_Result.Tables.IndexOf(table)}" : table.TableName] = rows;
            }

            return JsonSerializer.Serialize(dataSetDict, options ?? DefaultJsonOptions);
        }

        /// <summary>
        /// Guarda el resultado en un archivo JSON.
        /// </summary>
        /// <param name="filePath">Ruta del archivo donde guardar el JSON.</param>
        /// <param name="options">Opciones de serialización JSON (opcional).</param>
        public async Task SaveToJsonFileAsync(string filePath, JsonSerializerOptions? options = null)
        {
            var json = ToJson(options);
            await System.IO.File.WriteAllTextAsync(filePath, json);
        }

        /// <summary>
        /// Guarda el resultado tipado en un archivo JSON.
        /// </summary>
        /// <typeparam name="T">Tipo de objeto a serializar.</typeparam>
        /// <param name="filePath">Ruta del archivo donde guardar el JSON.</param>
        /// <param name="options">Opciones de serialización JSON (opcional).</param>
        public async Task SaveToJsonFileAsync<T>(string filePath, JsonSerializerOptions? options = null) where T : new()
        {
            var json = ToJson<T>(options);
            await System.IO.File.WriteAllTextAsync(filePath, json);
        }

        #endregion

        #region Métodos de Deserialización JSON

        /// <summary>
        /// Deserializa una cadena JSON a una lista de objetos del tipo especificado.
        /// </summary>
        /// <typeparam name="T">Tipo de objeto a deserializar.</typeparam>
        /// <param name="json">Cadena JSON a deserializar.</param>
        /// <param name="options">Opciones de deserialización JSON (opcional).</param>
        /// <returns>Lista de objetos deserializados.</returns>
        public static List<T> FromJson<T>(string json, JsonSerializerOptions? options = null)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<T>();

            return JsonSerializer.Deserialize<List<T>>(json, options ?? DefaultJsonOptions) ?? new List<T>();
        }

        /// <summary>
        /// Deserializa un archivo JSON a una lista de objetos del tipo especificado.
        /// </summary>
        /// <typeparam name="T">Tipo de objeto a deserializar.</typeparam>
        /// <param name="filePath">Ruta del archivo JSON a deserializar.</param>
        /// <param name="options">Opciones de deserialización JSON (opcional).</param>
        /// <returns>Lista de objetos deserializados.</returns>
        public static async Task<List<T>> FromJsonFileAsync<T>(string filePath, JsonSerializerOptions? options = null)
        {
            if (!System.IO.File.Exists(filePath))
                throw new System.IO.FileNotFoundException($"El archivo '{filePath}' no existe.");

            var json = await System.IO.File.ReadAllTextAsync(filePath);
            return FromJson<T>(json, options);
        }

        /// <summary>
        /// Deserializa una cadena JSON a un DataTable.
        /// </summary>
        /// <param name="json">Cadena JSON a deserializar.</param>
        /// <returns>DataTable con los datos deserializados.</returns>
        public static DataTable FromJsonToDataTable(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new DataTable();

            var rows = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);
            if (rows == null || rows.Count == 0)
                return new DataTable();

            var table = new DataTable();

            // Crear columnas basadas en la primera fila
            foreach (var key in rows[0].Keys)
            {
                table.Columns.Add(key);
            }

            // Agregar filas
            foreach (var row in rows)
            {
                var dataRow = table.NewRow();
                foreach (var kvp in row)
                {
                    dataRow[kvp.Key] = kvp.Value ?? DBNull.Value;
                }
                table.Rows.Add(dataRow);
            }

            return table;
        }

        #endregion
    }

    #endregion

    #region Implementación del Servicio

    /// <summary>
    /// Proporciona métodos para ejecutar consultas SQL y procedimientos almacenados de forma asíncrona en una base de datos SQL Server,
    /// devolviendo los resultados como DataTable, DataSet u objetos mapeados.
    /// </summary>
    public class SqlClientService : ISQLClientService
    {
        private readonly string _connectionString;
        private const int DefaultTimeout = 600; // timeOutpor defecto en segundos

        /// <summary>
        /// Inicializa una nueva instancia de la clase SqlClientService utilizando la cadena de conexión especificada.
        /// </summary>
        /// <param name="connectionString">La cadena de conexión utilizada para establecer una conexión con la base de datos.</param>
        public SqlClientService(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("La cadena de conexión no puede ser nula ni estar vacía.", nameof(connectionString));

            _connectionString = connectionString;
        }

        /// <summary>
        /// Obtiene una nueva conexión a la base de datos SQL.
        /// </summary>
        private SqlConnection GetConnection()
        {
            try
            {
                return new SqlConnection(_connectionString);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"Error al crear la conexión SQL: {ex.Message}", ex);
            }
        }

        #region Funciones Asincronas
        
        #region Métodos QueryAsync
        /// <summary>
        /// Método privado que ejecuta una consulta SQL con todos los parámetros opcionales.
        /// </summary>
        public async Task<SqlQueryResult>QueryAsync([Required]string query, SqlParameter[]? parameters = null, CancellationToken? cancellationToken = null, int? timeout = null)
        { 
            
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("La consulta no puede ser nula ni estar vacía.", nameof(query));

            // ✅ Validaciones
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("...", nameof(query));

            // ✅ Lógica correcta
            var token = cancellationToken ?? CancellationToken.None;
            var commandTimeout = timeout ?? DefaultTimeout;



            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync(token);

                using var command = new SqlCommand(query, connection)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout= commandTimeout
                };

                if (parameters != null)
                    command.Parameters.AddRange(parameters);


                using var reader = await command.ExecuteReaderAsync(token);
                using var dataSet = new DataSet();
                while (!reader.IsClosed)
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    dataSet.Tables.Add(dataTable);
                };

                return new SqlQueryResult(dataSet);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"Error al ejecutar la consulta SQL: {ex.Message}", ex);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("La operación de consulta fue cancelada.");
            }
        }

        #endregion

        #region Métodos ExecuteAsync
        /// <summary>
        /// Método privado que ejecuta un procedimiento almacenado con todos los parámetros opcionales.
        /// </summary>
        public async Task<SqlQueryResult> ExecuteAsync([Required] string storeProcedureName, SqlParameter[]? parameters = null, CancellationToken? cancellationToken = null, int? timeout = null)
        {
            if (string.IsNullOrWhiteSpace(storeProcedureName))
                throw new ArgumentException("El nombre del procedimiento almacenado no puede ser nulo ni estar vacío.", nameof(storeProcedureName));

            try
            {

                // ✅ Validaciones
                if (string.IsNullOrWhiteSpace(storeProcedureName))
                    throw new ArgumentException("...", nameof(storeProcedureName));

                // ✅ Lógica correcta
                var token = cancellationToken ?? CancellationToken.None;
                var commandTimeout = timeout ?? DefaultTimeout;

                using var connection = GetConnection();
                await connection.OpenAsync(token);

                using var command = new SqlCommand(storeProcedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = commandTimeout
                };

                if (parameters != null)
                    command.Parameters.AddRange(parameters);


                using var reader = await command.ExecuteReaderAsync(token);
                using var dataSet = new DataSet();
                while (!reader.IsClosed)
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    dataSet.Tables.Add(dataTable);
                };

                return new SqlQueryResult(dataSet);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"Error al ejecutar el procedimiento almacenado '{storeProcedureName}': {ex.Message}", ex);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException($"La ejecución del procedimiento almacenado '{storeProcedureName}' fue cancelada.");
            }
        }

        #endregion
        #endregion


        #region Funciones Sincronas
        /// <summary>
        /// Método privado que ejecuta una consulta SQL con todos los parámetros opcionales.
        /// </summary>
        public SqlQueryResult Query([Required] string query, SqlParameter[]? parameters = null, int? timeout = null)
        {

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("La consulta no puede ser nula ni estar vacía.", nameof(query));

            // ✅ Validaciones
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("...", nameof(query));

            // ✅ Lógica correcta            
            var commandTimeout = timeout ?? DefaultTimeout;



            try
            {
                using var connection = GetConnection();
                connection.Open();

                using var command = new SqlCommand(query, connection)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = commandTimeout
                };

                if (parameters != null)
                    command.Parameters.AddRange(parameters);


                using var reader =  command.ExecuteReader();
                using var dataSet = new DataSet();
                while (!reader.IsClosed)
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    dataSet.Tables.Add(dataTable);
                }
                ;

                return new SqlQueryResult(dataSet);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"Error al ejecutar la consulta SQL: {ex.Message}", ex);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("La operación de consulta fue cancelada.");
            }
        }

        #endregion


        #region Métodos ExecuteAsync
        /// <summary>
        /// Método privado que ejecuta un procedimiento almacenado con todos los parámetros opcionales.
        /// </summary>
        public SqlQueryResult Execute([Required] string storeProcedureName, SqlParameter[]? parameters = null, int? timeout = null)
        {
            if (string.IsNullOrWhiteSpace(storeProcedureName))
                throw new ArgumentException("El nombre del procedimiento almacenado no puede ser nulo ni estar vacío.", nameof(storeProcedureName));

            try
            {

                // ✅ Validaciones
                if (string.IsNullOrWhiteSpace(storeProcedureName))
                    throw new ArgumentException("...", nameof(storeProcedureName));

                // ✅ Lógica correcta
                var commandTimeout = timeout ?? DefaultTimeout;

                using var connection = GetConnection();
                connection.Open();

                using var command = new SqlCommand(storeProcedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = commandTimeout
                };

                if (parameters != null)
                    command.Parameters.AddRange(parameters);


                using var reader = command.ExecuteReader();
                using var dataSet = new DataSet();
                while (!reader.IsClosed)
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    dataSet.Tables.Add(dataTable);
                };

                return new SqlQueryResult(dataSet);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"Error al ejecutar el procedimiento almacenado '{storeProcedureName}': {ex.Message}", ex);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException($"La ejecución del procedimiento almacenado '{storeProcedureName}' fue cancelada.");
            }
        }

        #endregion


    }

    #endregion
}