using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DBSQLClient.Helpers
{
    public static class ObjectJsonExtensions
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        /// <summary>
        /// Serializa cualquier objeto a una representación JSON en formato de texto.
        /// </summary>
        /// <remarks>
        /// Este método permite llamar a <c name="ToJsonString()">ToJsonString()</c> de manera encadenada 
        /// después de un mapeo u operación similar.  
        /// <para>Si no se especifica el parámetro <paramref name="options"/>, se utilizarán
        /// las opciones predeterminadas internas que formatean la salida en PascalCase,</para>
        /// con identación y omitiendo valores nulos.
        /// </remarks>
        /// <param name="instance">
        /// Instancia del objeto que se desea serializar a JSON.
        /// </param>
        /// <param name="options">
        /// Opciones personalizadas para el serializador JSON.  
        /// Si es <see langword="null"/> o no se especifica, se utilizarán las opciones por defecto.
        /// </param>
        /// <returns>
        /// <para>Devuelve una cadena (<see cref="string"/>) con el contenido JSON del objeto.</para>
        /// Si <see cref="object"/> <paramref name="instance"/> es <see langword="null"/>, se devolverá <c>{}</c>.
        /// <example>
        /// Ejemplo de uso:
        /// <code>
        /// var user = result
        ///     .ToSingleWithChildren&lt;UserModel, UserRol&gt;("Roles")
        ///     .ToJsonString();
        /// 
        /// Console.WriteLine(user);
        /// </code>
        /// </example>
        /// </returns>

        public static string ToJsonString(this object instance, JsonSerializerOptions? options = default)
        {
            if (instance == null)
                return "{}";
            return JsonSerializer.Serialize(instance, options == default ? _options : options);
        }



        /// <summary>
        /// Crear un JsonDocument desde cualquier objeto.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static JsonDocument ToJsonDocument(this object obj, JsonSerializerOptions? options = default)
        {
            if (obj == null)
                return JsonDocument.Parse("{}");
            JsonSerializer.Serialize(obj, options == default ? _options : options);
            return JsonDocument.Parse(JsonSerializer.Serialize(obj, options == default ? _options : options));
        }
    }

}
