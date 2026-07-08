using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using DBSQLClient.Servicio;

namespace DBSQLClient.Helpers
{
    /// <summary>
    /// Proporciona métodos de extensión para serializar objetos a JSON y crear documentos JSON.
    /// </summary>
    public static class ObjectJsonExtensions
    {
        /// <summary>
        /// Resolver que aplica los atributos de serialización de la librería
        /// (<see cref="NotSerializedAttribute"/>, <see cref="JsDateTimeAttribute"/>).
        /// Compartido con <c>SqlQueryResult.ToJson</c> para que funcionen igual sin importar qué
        /// método de la librería se use para serializar.
        /// </summary>
        internal static readonly IJsonTypeInfoResolver LibraryAttributesResolver =
            new DefaultJsonTypeInfoResolver().WithAddedModifier(ApplyLibraryJsonAttributes);

        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            TypeInfoResolver = LibraryAttributesResolver,
            // Por defecto, System.Text.Json escapa todo carácter no-ASCII (ej. "á" -> "á"),
            // pensado para poder incrustar el JSON sin riesgo dentro de un <script> de HTML.
            // Para un valor mapeado desde la base (nombres, descripciones, etc.) eso solo hace el
            // JSON menos legible sin aportar nada; UnsafeRelaxedJsonEscaping deja pasar acentos y
            // demás Unicode tal cual. Sigue escapando lo necesario para que el JSON sea válido
            // (comillas, barras invertidas, caracteres de control) — lo que NO escapa son
            // caracteres sensibles para HTML (`<`, `>`, `&`, `'`), así que si este JSON se va a
            // incrustar directamente en una página HTML (no como respuesta de API consumida por
            // fetch/AJAX), hay que volver a codificarlo en ese punto.
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        /// <summary>
        /// Serializa cualquier objeto a una representación JSON en formato de texto.
        /// </summary>
        /// <remarks>
        /// Este método permite llamar a <c name="ToJsonString()">ToJsonString()</c> de manera encadenada
        /// después de un mapeo u operación similar.
        /// <para>Si no se especifica el parámetro <paramref name="options"/>, se utilizarán
        /// las opciones predeterminadas internas que formatean la salida en PascalCase,</para>
        /// con identación y omitiendo valores nulos. En ambos casos se respetan
        /// <see cref="NotSerializedAttribute"/> y <see cref="JsDateTimeAttribute"/>, salvo que
        /// <paramref name="options"/> ya traiga su propio
        /// <see cref="JsonSerializerOptions.TypeInfoResolver"/> configurado.
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
            return JsonSerializer.Serialize(instance, ResolveOptions(options));
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
            var effectiveOptions = ResolveOptions(options);
            return JsonDocument.Parse(JsonSerializer.Serialize(obj, effectiveOptions));
        }

        /// <summary>
        /// Devuelve <paramref name="options"/> tal cual si ya trae su propio
        /// <see cref="JsonSerializerOptions.TypeInfoResolver"/> (se asume una configuración
        /// deliberada del llamador), o una copia con <see cref="LibraryAttributesResolver"/>
        /// agregado en caso contrario. Nunca muta la instancia recibida:
        /// <see cref="JsonSerializerOptions"/> se "congela" después del primer uso y lanzaría si
        /// se modificara en el lugar.
        /// </summary>
        internal static JsonSerializerOptions ResolveOptions(JsonSerializerOptions? options)
        {
            if (options is null)
            {
                return _options;
            }

            if (options.TypeInfoResolver is not null)
            {
                return options;
            }

            return new JsonSerializerOptions(options) { TypeInfoResolver = LibraryAttributesResolver };
        }

        private static void ApplyLibraryJsonAttributes(JsonTypeInfo typeInfo)
        {
            if (typeInfo.Kind != JsonTypeInfoKind.Object)
            {
                return;
            }

            foreach (var property in typeInfo.Properties)
            {
                var attributeProvider = property.AttributeProvider;
                if (attributeProvider is null)
                {
                    continue;
                }

                if (attributeProvider.IsDefined(typeof(NotSerializedAttribute), inherit: true))
                {
                    property.ShouldSerialize = static (_, _) => false;
                }

                var jsDateTimeAttributes = attributeProvider.GetCustomAttributes(typeof(JsDateTimeAttribute), inherit: true);
                if (jsDateTimeAttributes.Length > 0)
                {
                    var format = ((JsDateTimeAttribute)jsDateTimeAttributes[0]).ResolveFormat();

                    if (property.PropertyType == typeof(DateTime))
                    {
                        property.CustomConverter = new JsCompatibleDateTimeConverter(format);
                    }
                    else if (property.PropertyType == typeof(DateTime?))
                    {
                        property.CustomConverter = new JsCompatibleNullableDateTimeConverter(format);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Normaliza un <see cref="DateTime"/> a UTC (sin convertir horas locales: un valor
    /// <see cref="DateTimeKind.Unspecified"/> se asume que ya representa UTC) y lo formatea con
    /// la cadena de formato pedida, usando <see cref="CultureInfo.InvariantCulture"/>. Compartido
    /// por <see cref="JsCompatibleDateTimeConverter"/> y
    /// <see cref="JsCompatibleNullableDateTimeConverter"/>. Ver <see cref="JsDateTimeAttribute"/>.
    /// </summary>
    internal static class JsDateTimeFormatter
    {
        /// <param name="value">Valor a formatear.</param>
        /// <param name="format">Cadena de formato de <see cref="DateTime.ToString(string, IFormatProvider)"/>.</param>
        public static string Format(DateTime value, string format)
        {
            var utc = value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };

            return utc.ToString(format, CultureInfo.InvariantCulture);
        }
    }

    /// <inheritdoc cref="JsDateTimeFormatter"/>
    internal sealed class JsCompatibleDateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string _format;

        public JsCompatibleDateTimeConverter(string format)
        {
            _format = format;
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.GetDateTime();

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            => writer.WriteStringValue(JsDateTimeFormatter.Format(value, _format));
    }

    /// <inheritdoc cref="JsDateTimeFormatter"/>
    internal sealed class JsCompatibleNullableDateTimeConverter : JsonConverter<DateTime?>
    {
        private readonly string _format;

        public JsCompatibleNullableDateTimeConverter(string format)
        {
            _format = format;
        }

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType == JsonTokenType.Null ? null : reader.GetDateTime();

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStringValue(JsDateTimeFormatter.Format(value.Value, _format));
        }
    }

}
