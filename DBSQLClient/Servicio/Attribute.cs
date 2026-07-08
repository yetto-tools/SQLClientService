using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DBSQLClient.Servicio;

namespace DBSQLClient.Servicio
{
    /// <summary>
    /// Indica que la propiedad es la clave primaria de la entidad.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PrimaryKeyAttribute : Attribute
    {
    }

    /// <summary>
    /// Indica que la propiedad es una clave foránea que referencia a otra entidad.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ForeignKeyAttribute : Attribute
    {
        /// <summary>
        /// El tipo de la entidad a la que se hace referencia con esta clave foránea.
        /// </summary>
        public Type ReferenceType { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ForeignKeyAttribute"/> con el tipo de referencia especificado.
        /// </summary>
        /// <param name="referenceType"></param>
        public ForeignKeyAttribute(Type referenceType)
        {
            ReferenceType = referenceType;
        }
    }

    /// <summary>
    /// Declara, sobre la propiedad de navegación del lado <b>uno</b> (el padre), una relación
    /// uno a muchos hacia <see cref="ChildType"/> (el tipo de los muchos hijos). La propiedad
    /// debe ser una colección de <see cref="ChildType"/> (ej: <c>List&lt;Order&gt;</c>).
    /// </summary>
    /// <remarks>
    /// La leen dos métodos, según cuántos padres trae el resultado:
    /// <list type="bullet">
    /// <item><c>SqlResultMapper.MapOneToMany&lt;TParent, TChild&gt;</c> (extensión <c>OneToMany</c>):
    /// un solo padre (tabla 0, 1 fila) con sus hijos (tabla 1).</item>
    /// <item><c>SqlResultMapper.MapMultipleOneToMany&lt;TParent, TChild&gt;</c> (extensión
    /// <c>OneToManyMultiple</c>): varios padres a la vez (tabla 0), agrupando los hijos (tabla 1)
    /// por <see cref="PrimaryKeyAttribute"/>/<see cref="ForeignKeyAttribute"/>.</item>
    /// </list>
    /// No confundir con <see cref="ManyToOneAttribute"/>: esa se declara en el modelo hijo, no en
    /// el padre, y apunta en la dirección contraria.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OneToManyAttribute : Attribute
    {
        /// <summary>
        /// El tipo de la entidad hija en la relación uno a muchos.
        /// </summary>
        public Type ChildType { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="OneToManyAttribute"/> con el tipo de entidad hija especificado.
        /// </summary>
        /// <param name="childType"></param>
        public OneToManyAttribute(Type childType)
        {
            ChildType = childType;
        }
    }

    /// <summary>
    /// Declara, sobre la propiedad de navegación del lado <b>muchos</b> (el hijo), una relación
    /// muchos a uno hacia <see cref="ParentType"/> (el tipo del único padre compartido). La
    /// propiedad debe ser una referencia simple a <see cref="ParentType"/> (no una colección).
    /// </summary>
    /// <remarks>
    /// La lee <c>SqlResultMapper.MapManyToOne&lt;TChild, TParent&gt;</c> (extensión
    /// <c>ManyToOne</c>): muchos hijos (tabla 0) que comparten un único padre (tabla 1, 1 fila),
    /// asignado a todos y cada uno de los hijos. Un mismo modelo puede declarar varios
    /// <see cref="ManyToOneAttribute"/> en propiedades distintas — uno por cada tipo de padre
    /// posible (ej: una orden que pertenece a un usuario registrado O a un invitado, nunca ambos:
    /// dos propiedades, cada una con su propio <see cref="ManyToOneAttribute"/>, y
    /// <c>MapManyToOne</c> resuelve la que corresponda según el <c>TParent</c> pedido).
    /// No confundir con <see cref="OneToManyAttribute"/>: esa se declara en el modelo padre, no
    /// en el hijo, y apunta en la dirección contraria.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ManyToOneAttribute : Attribute
    {
        /// <summary>
        /// El tipo de la entidad padre en la relación muchos a uno.
        /// </summary>
        public Type ParentType { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ManyToOneAttribute"/> con el tipo de entidad padre especificado.
        /// </summary>
        /// <param name="parentType"></param>
        public ManyToOneAttribute(Type parentType)
        {
            ParentType = parentType;
        }
    }

    /// <summary>
    /// Declara, sobre la propiedad de navegación del lado <b>izquierdo</b>, una relación muchos a
    /// muchos hacia <see cref="TargetType"/> (el lado derecho) a través de una tabla de unión
    /// <see cref="JoinType"/>. La propiedad debe ser una colección de <see cref="TargetType"/>.
    /// </summary>
    /// <remarks>
    /// La lee <c>SqlResultMapper.MapManyToMany&lt;TLeft, TRight, TJoin&gt;</c> (extensión
    /// <c>ManyToMany</c>): lado izquierdo (tabla 0), lado derecho (tabla 1) y tabla de unión
    /// (tabla 2) — la única de las cuatro relaciones que necesita 3 tablas en vez de 2. El modelo
    /// de la tabla de unión (<see cref="JoinType"/>) solo necesita <see cref="ForeignKeyAttribute"/>
    /// hacia cada lado, no un <see cref="ManyToManyAttribute"/> propio.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ManyToManyAttribute : Attribute
    {
        /// <summary>
        /// El tipo de la entidad objetivo en la relación muchos a muchos.
        /// </summary>
        public Type TargetType { get; }
        
        /// <summary>
        /// El tipo de la tabla de unión que representa la relación muchos a muchos.
        /// </summary>
        public Type JoinType { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ManyToManyAttribute"/> con el tipo de entidad objetivo y el tipo de tabla de unión especificados.
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="joinType"></param>
        public ManyToManyAttribute(Type targetType, Type joinType)
        {
            TargetType = targetType;
            JoinType = joinType;
        }
    }

    /// <summary>
    /// Declara, sobre la propiedad de navegación de cualquiera de los dos lados, una relación uno
    /// a uno hacia <see cref="TargetType"/>. La propiedad debe ser una referencia simple a
    /// <see cref="TargetType"/> (no una colección) — normalmente nullable, porque el lado "uno"
    /// puede no tener todavía su contraparte (ej: una orden sin factura emitida aún).
    /// </summary>
    /// <remarks>
    /// La lee <c>SqlResultMapper.MapOneToOne&lt;TParent, TChild&gt;</c> (extensión
    /// <c>OneToOne</c>): tabla 0 = <c>TParent</c> (1 fila, obligatoria), tabla 1 = <c>TChild</c>
    /// (0 o 1 fila — si no hay fila, la propiedad queda en su valor por defecto en vez de
    /// fallar). Es la única de las cuatro relaciones donde ambos lados son "uno": no hay una
    /// versión "de vuelta" como pasa con <see cref="OneToManyAttribute"/>/
    /// <see cref="ManyToOneAttribute"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OneToOneAttribute : Attribute
    {
        /// <summary>
        /// El tipo de la entidad objetivo en la relación uno a uno.
        /// </summary>
        public Type TargetType { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="OneToOneAttribute"/> con el tipo de entidad objetivo especificado.
        /// </summary>
        /// <param name="targetType"></param>
        public OneToOneAttribute(Type targetType)
        {
            TargetType = targetType;
        }
    }

    /// <summary>
    /// Indica que la propiedad representa una columna en la tabla de la base de datos y permite especificar un nombre de columna diferente al nombre de la propiedad.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Obtiene el nombre de la columna asociada con la propiedad.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ColumnAttribute"/> con el nombre de columna especificado.
        /// </summary>
        /// <param name="name"></param>
        public ColumnAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Excluye la propiedad de todo el mapeo (columnas, PK, FK y relaciones). Útil para
    /// propiedades calculadas o que no vienen de la base de datos.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NotMappedAttribute : Attribute
    {
    }

    /// <summary>
    /// Excluye la propiedad únicamente de la serialización JSON hecha por
    /// <see cref="DBSQLClient.Helpers.ObjectJsonExtensions"/> y <c>SqlQueryResult.ToJson</c>
    /// (ej: <c>ToJsonString</c>, <c>ToJson&lt;T&gt;</c>). No afecta el mapeo desde la base de
    /// datos (columnas, PK, FK, relaciones) ni la deserialización con <c>FromJson</c>/
    /// <c>FromJsonFileAsync</c> — para eso sigue existiendo <see cref="NotMappedAttribute"/>.
    /// Útil para campos sensibles que sí vienen de la base (ej: un hash de password) pero que
    /// nunca deberían salir en un JSON expuesto hacia afuera.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NotSerializedAttribute : Attribute
    {
    }

    /// <summary>
    /// Marca una propiedad <see cref="DateTime"/>/<see cref="Nullable{DateTime}"/> para
    /// serializarse en JSON en un formato explícito, siempre normalizando el valor a UTC antes de
    /// formatear. Sin argumento (o con <c>"ISO-8601"</c>) usa <c>yyyy-MM-ddTHH:mm:ss.fffK</c>
    /// (ej: <c>2026-07-08T10:46:57.970Z</c>) en vez del formato por defecto de
    /// <c>System.Text.Json</c> (ej: <c>2026-07-08T10:46:57.97</c>, sin indicar zona horaria).
    /// </summary>
    /// <remarks>
    /// El formato por defecto de <c>System.Text.Json</c> es ambiguo para un cliente JavaScript:
    /// <c>new Date(...)</c> interpreta una cadena sin zona horaria como **hora local del
    /// navegador**, no UTC — el mismo valor se ve distinto según en qué huso horario esté quien lo
    /// consuma. El sufijo <c>Z</c> del formato por defecto de esta anotación elimina esa
    /// ambigüedad.
    /// <para>
    /// Pasar cualquier otro valor a <see cref="JsDateTimeAttribute(string)"/> (ej:
    /// <c>"yyyy-MM-dd"</c>, <c>"o"</c>, <c>"R"</c>) lo usa tal cual como cadena de formato de
    /// <see cref="DateTime.ToString(string, IFormatProvider)"/> — estándar o personalizada,
    /// cualquiera que soporte <c>System.Globalization</c> — siempre aplicada con
    /// <see cref="System.Globalization.CultureInfo.InvariantCulture"/> (nunca la cultura del
    /// hilo actual: un JSON no debería cambiar de forma según el servidor que lo genera) y
    /// siempre sobre el valor ya normalizado a UTC.
    /// </para>
    /// <para>
    /// En todos los casos se asume que el valor ya representa UTC (igual que un <c>DATETIME</c>
    /// de SQL Server, que llega con <c>DateTimeKind.Unspecified</c> pero sin desplazamiento de
    /// por medio) y se lo marca como tal antes de formatear — nunca convierte una hora local a
    /// UTC. Solo afecta la serialización hecha por
    /// <see cref="DBSQLClient.Helpers.ObjectJsonExtensions"/> y <c>SqlQueryResult.ToJson</c>; la
    /// deserialización con <c>FromJson</c>/<c>FromJsonFileAsync</c> sigue leyendo cualquier
    /// formato ISO-8601 válido con normalidad.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// [JsDateTime]                  // yyyy-MM-ddTHH:mm:ss.fffK (default)
    /// [JsDateTime("ISO-8601")]      // igual que el default, de forma explícita
    /// [JsDateTime("yyyy-MM-dd")]    // solo fecha, sin hora
    /// [JsDateTime("o")]             // round-trip de .NET, con más precisión de fracción de segundo
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class JsDateTimeAttribute : Attribute
    {
        private const string Iso8601Keyword = "ISO-8601";

        /// <summary>
        /// Cadena de formato pedida por el usuario, o <see langword="null"/> si se usó el
        /// constructor sin argumentos. Usar <see cref="ResolveFormat"/> para obtener el formato
        /// efectivo (ya resuelve el default y la palabra clave <c>"ISO-8601"</c>).
        /// </summary>
        public string? Format { get; }

        /// <summary>
        /// Usa el formato por defecto: ISO-8601 UTC con sufijo <c>Z</c>
        /// (<c>yyyy-MM-ddTHH:mm:ss.fffK</c>).
        /// </summary>
        public JsDateTimeAttribute()
        {
        }

        /// <summary>
        /// Usa <paramref name="format"/> como cadena de formato de
        /// <see cref="DateTime.ToString(string, IFormatProvider)"/>, o el default ISO-8601 UTC si
        /// <paramref name="format"/> es <c>"ISO-8601"</c> (sin distinguir mayúsculas/minúsculas).
        /// </summary>
        public JsDateTimeAttribute(string format)
        {
            Format = format;
        }

        /// <summary>
        /// Formato efectivo a usar con <see cref="DateTime.ToString(string, IFormatProvider)"/>:
        /// el formato por defecto si no se pasó ninguno o se pasó <c>"ISO-8601"</c>, o
        /// <see cref="Format"/> tal cual en cualquier otro caso.
        /// </summary>
        internal string ResolveFormat()
        {
            const string defaultFormat = "yyyy-MM-ddTHH:mm:ss.fffK";

            return Format is null || Format.Equals(Iso8601Keyword, StringComparison.OrdinalIgnoreCase)
                ? defaultFormat
                : Format;
        }
    }

    /// <summary>
    /// Declara que el modelo representa filas de una tabla (para lectura directa, ej: SELECT).
    /// No se puede combinar con <see cref="StoredProcedureAttribute"/> en el mismo modelo.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TableAttribute : Attribute
    {
        /// <summary>
        /// Obtiene el nombre de la tabla asociada con la clase.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="TableAttribute"/> con el nombre de la tabla especificado.
        /// </summary>
        /// <param name="name"></param>
        public TableAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Declara que el modelo representa el resultado de un procedimiento almacenado.
    /// No se puede combinar con <see cref="TableAttribute"/> en el mismo modelo.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class StoredProcedureAttribute : Attribute
    {
        /// <summary>
        /// Obtiene el nombre del procedimiento almacenado asociado con la clase.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="StoredProcedureAttribute"/> con el nombre del procedimiento almacenado especificado.
        /// </summary>
        /// <param name="name"></param>
        public StoredProcedureAttribute(string name)
        {
            Name = name;
        }
    }

}
