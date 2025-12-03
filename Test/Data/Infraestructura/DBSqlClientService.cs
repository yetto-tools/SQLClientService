using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.PortableExecutable;
using Test.Data.Interfaces;

namespace Test.Data.Infraestructura
{
    public class DBSqlClientService : IDBSqlClientService
    {
        private readonly string _connectionString;
        private SqlConnection? _connection;

        public DBSqlClientService(string connectionString)
        {
            _connectionString = connectionString;
        }

        private async Task<SqlConnection> GetConn()
        {
            try
            {
                if (_connection == null)
                    _connection = new SqlConnection(_connectionString);

                if (_connection.State != ConnectionState.Open)
                    await _connection.OpenAsync();

                return _connection;

            }
            catch(SqlException ex)
            {
                throw new InvalidOperationException($"Error SQL: {ex.Message}", ex);
            }

        }

        public async Task<DataTable> QueryAsyncAsDataTable(
            string sqlCommand,
            SqlParameter[]? parameters = default,
            CancellationToken ct = default,
            int Timeout = default)
        {
            var conn = await GetConn();
            await using var cmd = new SqlCommand(sqlCommand, conn)
            {
                CommandType = CommandType.Text
            };

            if (parameters != default)
                cmd.Parameters.AddRange(parameters);

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            using var dtResults = new DataTable();

            Task.Run(() =>
            {
                try
                {
                    dtResults.Load(reader);
                }
                catch (SqlException ex)
                {
                    throw new InvalidOperationException($"Error SQL: {ex.Message}", ex);
                }

            }, ct).Wait(ct);

            return dtResults;
        }



        public async Task<DataSet> ExecuteAsyncAsDataSet(
            string sqlCommand,
            SqlParameter[]? parameters = default,
            CancellationToken ct = default,
            int Timeout = default)
        {
            var conn = await GetConn();
            await using var cmd = new SqlCommand(sqlCommand, conn)
            {
                CommandType = CommandType.Text
            };

            if (parameters != default)
                cmd.Parameters.AddRange(parameters);

            using var results = new DataSet();
            Task.Run(() =>
            {
                try
                {
                    using var adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(results);
                }
                catch (SqlException ex)
                {
                    throw new InvalidOperationException($"Error SQL: {ex.Message}", ex);
                }

            }, ct).Wait(ct);

            return results;
        }



        public async Task<DataSet> ExecuteStoredProcedureAsync(
            string nameStoredProcedure,
            SqlParameter[]? parameters = default,
            CancellationToken ct = default,
            int Timeout = default)
        {
            var conn = await GetConn();
            await using var cmd = new SqlCommand(nameStoredProcedure, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != default)
                cmd.Parameters.AddRange(parameters);

            using var results = new DataSet();

            Task.Run(() =>
            {
                try
                {
                    using var adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(results);
                }
                catch (SqlException ex)
                {
                    throw new InvalidOperationException($"Error SQL: {ex.Message}", ex);
                }

            }, ct).Wait(ct);



            return results;
        }

        public async Task<T?> ExecuteStoredProcedureAsync<T>(
            string nameStoredProcedure,
            SqlParameter[]? parameters = default,
            CancellationToken ct = default,
            int Timeout = default)
        {
            var conn = await GetConn();
            await using var cmd = new SqlCommand(nameStoredProcedure, conn)
            {
                CommandType = CommandType.StoredProcedure
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

            // Mapear a Dataset segun el T generico solicitado
            object? mapped = null;
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

            return (T?)mapped;
        }

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
