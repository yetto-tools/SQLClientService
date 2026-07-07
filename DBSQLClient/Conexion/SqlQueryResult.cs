using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Data.SqlClient;

namespace DBSQLClient.Conexion;

/// <summary>
/// Resultado de una consulta SQL que proporciona múltiples formatos de salida.
/// </summary>
public sealed class SqlQueryResult
{
    private readonly DataSet _dataResult;
    private readonly Dictionary<string, object?> _outputParameters;
    private static readonly JsonSerializerOptions DefaultJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    /// Inicializa una nueva instancia a partir de un <see cref="DataSet"/> devuelto por SQL Server.
    /// </summary>
    /// <param name="dataSet">Conjunto de datos a encapsular.</param>
    /// <param name="parameters">
    /// Parámetros usados en la ejecución. Los que tengan dirección Output, InputOutput o
    /// ReturnValue quedan disponibles a través de <see cref="OutputParameters"/> y
    /// <see cref="GetOutputValue{T}"/>.
    /// </param>
    public SqlQueryResult(DataSet dataSet, IEnumerable<SqlParameter>? parameters = null)
    {
        _dataResult = dataSet?.Copy() ?? new DataSet();
        _outputParameters = ExtractOutputParameters(parameters);
    }

    private static Dictionary<string, object?> ExtractOutputParameters(IEnumerable<SqlParameter>? parameters)
    {
        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        if (parameters is null)
        {
            return result;
        }

        foreach (var parameter in parameters)
        {
            if (parameter.Direction is ParameterDirection.Output
                or ParameterDirection.InputOutput
                or ParameterDirection.ReturnValue)
            {
                result[parameter.ParameterName.TrimStart('@')] =
                    parameter.Value == DBNull.Value ? null : parameter.Value;
            }
        }

        return result;
    }

    /// <summary>
    /// Indica si la ejecución produjo parámetros de salida (Output, InputOutput o ReturnValue).
    /// </summary>
    public bool HasOutputParameters => _outputParameters.Count > 0;

    /// <summary>
    /// Parámetros de salida devueltos por el procedimiento, indexados por nombre (sin '@').
    /// </summary>
    public IReadOnlyDictionary<string, object?> OutputParameters => _outputParameters;

    /// <summary>
    /// Obtiene el valor de un parámetro de salida por nombre (con o sin '@').
    /// </summary>
    /// <typeparam name="T">Tipo de destino.</typeparam>
    /// <param name="name">Nombre del parámetro.</param>
    /// <returns>El valor convertido, o el valor por defecto de <typeparamref name="T"/> si es nulo o no existe.</returns>
    public T? GetOutputValue<T>(string name)
    {
        if (!_outputParameters.TryGetValue(name.TrimStart('@'), out var value) || value is null)
        {
            return default;
        }

        var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        return (T)Convert.ChangeType(value, targetType);
    }

    /// <summary>
    /// Devuelve el resultado como DataSet completo.
    /// </summary>
    public DataSet AsDataSet() => _dataResult;

    /// <summary>
    /// Devuelve la primera tabla del resultado como DataTable.
    /// </summary>
    public DataTable AsDataTable() => _dataResult.Tables.Count > 0 ? _dataResult.Tables[0] : new DataTable();

    /// <summary>
    /// Devuelve la tabla en el índice especificado como DataTable.
    /// </summary>
    public DataTable AsDataTable(int index)
    {
        if (index < 0 || index >= _dataResult.Tables.Count)
        {
            throw new IndexOutOfRangeException("El índice de la tabla está fuera de los límites.");
        }

        return _dataResult.Tables[index];
    }

    /// <summary>
    /// Devuelve las columnas de la primera tabla.
    /// </summary>
    public DataColumnCollection AsDataColumns() => AsDataTable().Columns;

    /// <summary>
    /// Devuelve las filas de la primera tabla.
    /// </summary>
    public DataRowCollection AsDataRows() => AsDataTable().Rows;

    /// <summary>
    /// Devuelve el resultado como una colección enumerable de DataRow.
    /// </summary>
    public IEnumerable<DataRow> AsEnumerable() => AsDataTable().AsEnumerable();

    /// <summary>
    /// Convierte la primera tabla en una lista de instancias del tipo indicado.
    /// </summary>
    /// <typeparam name="T">Tipo de destino con constructor público sin parámetros.</typeparam>
    /// <returns>Lista poblada con las filas de la tabla.</returns>
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
    /// Devuelve la primera instancia tipada o <c>null</c> si no hay resultados.
    /// </summary>
    /// <typeparam name="T">Tipo de destino.</typeparam>
    /// <returns>Primer elemento o <c>null</c>.</returns>
    public T? FirstOrDefault<T>() where T : new() => ToList<T>().FirstOrDefault();

    /// <summary>
    /// Devuelve el número de filas afectadas.
    /// </summary>
    public int RowCount => AsDataTable().Rows.Count;

    /// <summary>
    /// Indica si el resultado tiene filas.
    /// </summary>
    public bool HasRows => RowCount > 0;

    /// <summary>
    /// Serializa el DataTable a formato JSON.
    /// </summary>
    /// <param name="options">Opciones de serialización JSON opcionales.</param>
    /// <returns>Cadena JSON con la primera tabla.</returns>
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
    /// <typeparam name="T">Tipo a serializar.</typeparam>
    /// <param name="options">Opciones de serialización JSON opcionales.</param>
    /// <returns>Cadena JSON con la lista tipada.</returns>
    public string ToJson<T>(JsonSerializerOptions? options = null) where T : new()
    {
        var list = ToList<T>();
        return JsonSerializer.Serialize(list, options ?? DefaultJsonOptions);
    }

    /// <summary>
    /// Serializa todas las tablas del DataSet a JSON.
    /// </summary>
    /// <param name="options">Opciones de serialización JSON opcionales.</param>
    /// <returns>Cadena JSON donde cada tabla se representa como lista de diccionarios.</returns>
    public string ToJsonDataSet(JsonSerializerOptions? options = null)
    {
        var dataSetDict = new Dictionary<string, List<Dictionary<string, object?>>>();

        foreach (DataTable table in _dataResult.Tables)
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

            var tableName = string.IsNullOrEmpty(table.TableName)
                ? $"Table{_dataResult.Tables.IndexOf(table)}"
                : table.TableName;

            dataSetDict[tableName] = rows;
        }

        return JsonSerializer.Serialize(dataSetDict, options ?? DefaultJsonOptions);
    }

    /// <summary>
    /// Guarda el resultado en un archivo JSON.
    /// </summary>
    /// <param name="filePath">Ruta del archivo donde se guardará el JSON.</param>
    /// <param name="options">Opciones de serialización JSON opcionales.</param>
    public Task SaveToJsonFileAsync(string filePath, JsonSerializerOptions? options = null)
    {
        var json = ToJson(options);
        return File.WriteAllTextAsync(filePath, json);
    }

    /// <summary>
    /// Guarda el resultado tipado en un archivo JSON.
    /// </summary>
    /// <typeparam name="T">Tipo a serializar.</typeparam>
    /// <param name="filePath">Ruta del archivo donde se guardará el JSON.</param>
    /// <param name="options">Opciones de serialización JSON opcionales.</param>
    public Task SaveToJsonFileAsync<T>(string filePath, JsonSerializerOptions? options = null) where T : new()
    {
        var json = ToJson<T>(options);
        return File.WriteAllTextAsync(filePath, json);
    }

    /// <summary>
    /// Deserializa una cadena JSON a una lista de objetos del tipo especificado.
    /// </summary>
    /// <typeparam name="T">Tipo de destino.</typeparam>
    /// <param name="json">Cadena JSON a deserializar.</param>
    /// <param name="options">Opciones de serialización JSON opcionales.</param>
    /// <returns>Lista deserializada o una lista vacía si el contenido es nulo.</returns>
    public static List<T> FromJson<T>(string json, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<T>();
        }

        return JsonSerializer.Deserialize<List<T>>(json, options ?? DefaultJsonOptions) ?? new List<T>();
    }

    /// <summary>
    /// Deserializa un archivo JSON a una lista de objetos del tipo especificado.
    /// </summary>
    /// <typeparam name="T">Tipo de destino.</typeparam>
    /// <param name="filePath">Ruta al archivo JSON a deserializar.</param>
    /// <param name="options">Opciones de serialización JSON opcionales.</param>
    /// <returns>Lista deserializada con los elementos del archivo.</returns>
    public static async Task<List<T>> FromJsonFileAsync<T>(string filePath, JsonSerializerOptions? options = null)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"El archivo '{filePath}' no existe.");
        }

        var json = await File.ReadAllTextAsync(filePath);
        return FromJson<T>(json, options);
    }

    /// <summary>
    /// Deserializa una cadena JSON a un DataTable.
    /// </summary>
    /// <param name="json">Cadena JSON que representa una colección de filas.</param>
    /// <returns><see cref="DataTable"/> con las columnas y filas deserializadas.</returns>
    public static DataTable FromJsonToDataTable(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new DataTable();
        }

        var rows = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);
        if (rows is not { Count: > 0 })
        {
            return new DataTable();
        }

        var table = new DataTable();

        foreach (var key in rows[0].Keys)
        {
            table.Columns.Add(key);
        }

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
}
