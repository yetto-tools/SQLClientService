using System.Data;
using Microsoft.Data.SqlClient;

namespace DBSQLClient.Helpers
{
    /// <summary>
    /// Helper para crear parámetros SQL de forma simplificada.
    /// </summary>
    public static class SqlHelper
    {
        #region Métodos Básicos de Creación

        /// <summary>
        /// Crea un parámetro SQL con nombre y valor.
        /// </summary>
        /// <param name="name">Nombre del parámetro (con o sin @).</param>
        /// <param name="value">Valor del parámetro.</param>
        /// <returns>SqlParameter configurado.</returns>
        public static SqlParameter Param(string name, object? value)
        {
            return new SqlParameter(NormalizeName(name), value ?? DBNull.Value);
        }

        /// <summary>
        /// Crea un parámetro SQL con nombre, valor y tipo.
        /// </summary>
        public static SqlParameter Param(string name, object? value, SqlDbType type)
        {
            return new SqlParameter(NormalizeName(name), type) { Value = value ?? DBNull.Value };
        }

        /// <summary>
        /// Crea un parámetro SQL con nombre, valor, tipo y tamaño.
        /// </summary>
        public static SqlParameter Param(string name, object? value, SqlDbType type, int size)
        {
            return new SqlParameter(NormalizeName(name), type, size) { Value = value ?? DBNull.Value };
        }

        #endregion

        #region Parámetros de Salida

        /// <summary>
        /// Crea un parámetro de salida (Output).
        /// </summary>
        public static SqlParameter OutParam(string name, SqlDbType type)
        {
            return new SqlParameter(NormalizeName(name), type)
            {
                Direction = ParameterDirection.Output
            };
        }

        /// <summary>
        /// Crea un parámetro de salida con tamaño específico.
        /// </summary>
        public static SqlParameter OutParam(string name, SqlDbType type, int size)
        {
            return new SqlParameter(NormalizeName(name), type, size)
            {
                Direction = ParameterDirection.Output
            };
        }

        /// <summary>
        /// Crea un parámetro de entrada/salida (InputOutput).
        /// </summary>
        public static SqlParameter InOutParam(string name, object? value, SqlDbType type)
        {
            return new SqlParameter(NormalizeName(name), type)
            {
                Value = value ?? DBNull.Value,
                Direction = ParameterDirection.InputOutput
            };
        }

        /// <summary>
        /// Crea un parámetro de retorno (ReturnValue).
        /// </summary>
        public static SqlParameter ReturnParam(string name = "@ReturnValue")
        {
            return new SqlParameter(NormalizeName(name), SqlDbType.Int)
            {
                Direction = ParameterDirection.ReturnValue
            };
        }

        #endregion

        #region Parámetros por Tipo Específico

        /// <summary>
        /// Crea un parámetro de tipo entero.
        /// </summary>
        public static SqlParameter Int(string name, int? value)
        {
            return new SqlParameter(NormalizeName(name), SqlDbType.Int) { Value = (object?)value ?? DBNull.Value };
        }

        /// <summary>
        /// Crea un parámetro de tipo string (NVARCHAR).
        /// </summary>
        public static SqlParameter String(string name, string? value, int size = -1)
        {
            return new SqlParameter(NormalizeName(name), SqlDbType.NVarChar, size) { Value = (object?)value ?? DBNull.Value };
        }

        /// <summary>
        /// Crea un parámetro de tipo decimal.
        /// </summary>
        public static SqlParameter Decimal(string name, decimal? value, byte precision = 18, byte scale = 2)
        {
            var param = new SqlParameter(NormalizeName(name), SqlDbType.Decimal)
            {
                Value = (object?)value ?? DBNull.Value,
                Precision = precision,
                Scale = scale
            };
            return param;
        }

        /// <summary>
        /// Crea un parámetro de tipo DateTime.
        /// </summary>
        public static SqlParameter DateTime(string name, DateTime? value)
        {
            return new SqlParameter(NormalizeName(name), SqlDbType.DateTime) { Value = (object?)value ?? DBNull.Value };
        }

        /// <summary>
        /// Crea un parámetro de tipo DateTime2 (mayor precisión).
        /// </summary>
        public static SqlParameter DateTime2(string name, DateTime? value)
        {
            return new SqlParameter(NormalizeName(name), SqlDbType.DateTime2) { Value = (object?)value ?? DBNull.Value };
        }

        /// <summary>
        /// Crea un parámetro de tipo Date (solo fecha).
        /// </summary>
        public static SqlParameter Date(string name, DateTime? value)
        {
            return new SqlParameter(NormalizeName(name), SqlDbType.Date) { Value = (object?)value ?? DBNull.Value };
        }

        /// <summary>
        /// Crea un parámetro de tipo booleano (Bit).
        /// </summary>
        public static SqlParameter Bool(string name, bool? value)
        {
            return new SqlParameter(NormalizeName(name), SqlDbType.Bit) { Value = (object?)value ?? DBNull.Value };
        }

        /// <summary>
        /// Crea un parámetro de tipo GUID.
        /// </summary>
        public static SqlParameter Guid(string name, Guid? value)
        {
            return new SqlParameter(NormalizeName(name), SqlDbType.UniqueIdentifier) { Value = (object?)value ?? DBNull.Value };
        }

        /// <summary>
        /// Crea un parámetro de tipo binario (VarBinary).
        /// </summary>
        public static SqlParameter Binary(string name, byte[]? value)
        {
            return new SqlParameter(NormalizeName(name), SqlDbType.VarBinary, -1) { Value = (object?)value ?? DBNull.Value };
        }

        #endregion

        #region Métodos de Conveniencia

        /// <summary>
        /// Crea múltiples parámetros desde tuplas (nombre, valor).
        /// </summary>
        public static SqlParameter[] Params(params (string name, object? value)[] parameters)
        {
            return parameters.Select(p => Param(p.name, p.value)).ToArray();
        }

        /// <summary>
        /// Crea parámetros desde un diccionario.
        /// </summary>
        public static SqlParameter[] FromDictionary(Dictionary<string, object?> parameters)
        {
            return parameters.Select(kvp => Param(kvp.Key, kvp.Value)).ToArray();
        }

        /// <summary>
        /// Crea parámetros desde un objeto anónimo.
        /// </summary>
        public static SqlParameter[] FromObject(object obj)
        {
            var properties = obj.GetType().GetProperties();
            return properties.Select(p => Param(p.Name, p.GetValue(obj))).ToArray();
        }

        #endregion

        #region Métodos Auxiliares

        /// <summary>
        /// Normaliza el nombre del parámetro agregando @ si no lo tiene.
        /// </summary>
        private static string NormalizeName(string name)
        {
            return name.StartsWith("@") ? name : $"@{name}";
        }

        /// <summary>
        /// Obtiene el valor de un parámetro de salida de forma segura.
        /// </summary>
        public static T? GetOutputValue<T>(SqlParameter parameter)
        {
            if (parameter.Value == null || parameter.Value == DBNull.Value)
                return default;

            return (T)parameter.Value;
        }

        #endregion
    }

    #region Extension Methods (Opcional)

    /// <summary>
    /// Métodos de extensión para SqlParameter.
    /// </summary>
    public static class SqlParameterExtensions
    {
        /// <summary>
        /// Configura el parámetro como Output.
        /// </summary>
        public static SqlParameter AsOutput(this SqlParameter param)
        {
            param.Direction = ParameterDirection.Output;
            return param;
        }

        /// <summary>
        /// Configura el parámetro como InputOutput.
        /// </summary>
        public static SqlParameter AsInputOutput(this SqlParameter param)
        {
            param.Direction = ParameterDirection.InputOutput;
            return param;
        }

        /// <summary>
        /// Establece el tamaño del parámetro.
        /// </summary>
        public static SqlParameter WithSize(this SqlParameter param, int size)
        {
            param.Size = size;
            return param;
        }

        /// <summary>
        /// Establece la precisión y escala del parámetro (para decimales).
        /// </summary>
        public static SqlParameter WithPrecision(this SqlParameter param, byte precision, byte scale)
        {
            param.Precision = precision;
            param.Scale = scale;
            return param;
        }

        /// <summary>
        /// Establece el valor del parámetro.
        /// </summary>
        public static SqlParameter WithValue(this SqlParameter param, object? value)
        {
            param.Value = value ?? DBNull.Value;
            return param;
        }
    }

    #endregion

    #region Fluent Builder (Opcional)

    /// <summary>
    /// Builder para crear múltiples parámetros de forma fluida.
    /// </summary>
    public class SqlParameterBuilder
    {
        private readonly List<SqlParameter> _parameters = new();

        /// <summary>
        /// Agrega un parámetro de entrada.
        /// </summary>
        public SqlParameterBuilder Add(string name, object? value)
        {
            _parameters.Add(SqlHelper.Param(name, value));
            return this;
        }

        /// <summary>
        /// Agrega un parámetro con tipo específico.
        /// </summary>
        public SqlParameterBuilder Add(string name, object? value, SqlDbType type)
        {
            _parameters.Add(SqlHelper.Param(name, value, type));
            return this;
        }

        /// <summary>
        /// Agrega un parámetro de tipo string.
        /// </summary>
        public SqlParameterBuilder AddString(string name, string? value, int size = -1)
        {
            _parameters.Add(SqlHelper.String(name, value, size));
            return this;
        }

        /// <summary>
        /// Agrega un parámetro de tipo entero.
        /// </summary>
        public SqlParameterBuilder AddInt(string name, int? value)
        {
            _parameters.Add(SqlHelper.Int(name, value));
            return this;
        }

        /// <summary>
        /// Agrega un parámetro de tipo decimal.
        /// </summary>
        public SqlParameterBuilder AddDecimal(string name, decimal? value, byte precision = 18, byte scale = 2)
        {
            _parameters.Add(SqlHelper.Decimal(name, value, precision, scale));
            return this;
        }

        /// <summary>
        /// Agrega un parámetro de tipo DateTime.
        /// </summary>
        public SqlParameterBuilder AddDateTime(string name, DateTime? value)
        {
            _parameters.Add(SqlHelper.DateTime(name, value));
            return this;
        }

        /// <summary>
        /// Agrega un parámetro de tipo booleano.
        /// </summary>
        public SqlParameterBuilder AddBool(string name, bool? value)
        {
            _parameters.Add(SqlHelper.Bool(name, value));
            return this;
        }

        /// <summary>
        /// Agrega un parámetro de salida.
        /// </summary>
        public SqlParameterBuilder AddOutput(string name, SqlDbType type)
        {
            _parameters.Add(SqlHelper.OutParam(name, type));
            return this;
        }

        /// <summary>
        /// Agrega un parámetro de entrada/salida.
        /// </summary>
        public SqlParameterBuilder AddInputOutput(string name, object? value, SqlDbType type)
        {
            _parameters.Add(SqlHelper.InOutParam(name, value, type));
            return this;
        }

        /// <summary>
        /// Construye el array de parámetros.
        /// </summary>
        public SqlParameter[] Build()
        {
            return _parameters.ToArray();
        }

        /// <summary>
        /// Conversión implícita a array de parámetros.
        /// </summary>
        public static implicit operator SqlParameter[](SqlParameterBuilder builder)
        {
            return builder.Build();
        }
    }

    #endregion

}


