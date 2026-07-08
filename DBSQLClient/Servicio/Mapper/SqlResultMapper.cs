using System.Data;
using System.Reflection;
using DBSQLClient.Conexion;

namespace DBSQLClient.Servicio.Mapper.RelationsMapper
{
    /// <summary>
    /// Helper para mapear resultados con relaciones padre-hijo.
    /// Cuando no se indica el nombre de propiedad de navegación o las claves,
    /// se resuelven automáticamente a partir de los atributos declarados en los modelos
    /// (<see cref="OneToOneAttribute"/>, <see cref="OneToManyAttribute"/>, <see cref="ManyToOneAttribute"/>,
    /// <see cref="ManyToManyAttribute"/>, <see cref="PrimaryKeyAttribute"/>, <see cref="ForeignKeyAttribute"/>).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Los atributos (<c>[OneToOne]</c>, <c>[OneToMany]</c>, <c>[ManyToOne]</c>, <c>[ManyToMany]</c>)
    /// son metadata: se declaran una sola vez sobre una propiedad de navegación del modelo y
    /// describen la <i>forma</i> de la relación (qué tipo hay del otro lado, y si esa propiedad es
    /// una referencia simple o una colección). No ejecutan nada por sí solos.
    /// </para>
    /// <para>
    /// Los métodos <c>Map*</c> de esta clase (y sus equivalentes más cortos en
    /// <see cref="SqlQueryResultExtensions"/>: <c>OneToOne</c>, <c>OneToMany</c>, <c>ManyToOne</c>,
    /// <c>ManyToMany</c>, <c>OneToManyMultiple</c>) son la operación en tiempo de ejecución: leen
    /// las tablas de un <see cref="SqlQueryResult"/> con varios result sets, buscan el atributo
    /// correspondiente por reflexión (vía <see cref="MetadataCache"/>) para saber en qué
    /// propiedad escribir, y arman los objetos. Cada método espera un número y orden de tablas
    /// específico, que tiene que coincidir con lo que devuelve el procedimiento almacenado:
    /// </para>
    /// <list type="table">
    /// <listheader><term>Atributo (en el modelo)</term><description>Método <c>Map*</c> que lo lee / tablas esperadas</description></listheader>
    /// <item>
    /// <term><c>[OneToOne(typeof(Child))]</c> en <c>Parent</c></term>
    /// <description><see cref="MapOneToOne{TParent, TChild}"/>: tabla 0 = <c>Parent</c> (1 fila), tabla 1 = <c>Child</c> (0 o 1 fila).</description>
    /// </item>
    /// <item>
    /// <term><c>[OneToMany(typeof(Child))]</c> en <c>Parent</c></term>
    /// <description>
    /// <see cref="MapOneToMany{TParent, TChild}"/> (un padre): tabla 0 = <c>Parent</c> (1 fila), tabla 1 = hijos.<br/>
    /// <see cref="MapMultipleOneToMany{TParent, TChild}"/> (varios padres a la vez): tabla 0 = padres, tabla 1 = hijos, agrupados por PK/FK.
    /// </description>
    /// </item>
    /// <item>
    /// <term><c>[ManyToOne(typeof(Parent))]</c> en <c>Child</c></term>
    /// <description><see cref="MapManyToOne{TChild, TParent}"/>: tabla 0 = hijos, tabla 1 = <c>Parent</c> único compartido (1 fila).</description>
    /// </item>
    /// <item>
    /// <term><c>[ManyToMany(typeof(Right), typeof(Join))]</c> en <c>Left</c></term>
    /// <description><see cref="MapManyToMany{TLeft, TRight, TJoin}"/>: tabla 0 = <c>Left</c>, tabla 1 = <c>Right</c>, tabla 2 = <c>Join</c>.</description>
    /// </item>
    /// </list>
    /// <para>
    /// La regla práctica para no confundirse: el atributo se declara del lado que tiene la
    /// propiedad de navegación (y su nombre describe la relación <i>desde ese lado</i>), mientras
    /// que el método siempre nombra <b>ambos</b> lados como parámetros de tipo, en el mismo orden
    /// en que aparecen las tablas del resultado — <c>MapOneToMany&lt;Order, OrderItem&gt;</c> y
    /// <c>MapManyToOne&lt;OrderItem, Order&gt;</c> pueden describir la misma relación de datos,
    /// pero leen atributos distintos (uno en <c>Order</c>, el otro en <c>OrderItem</c>) y esperan
    /// las tablas en orden distinto (padre primero vs. hijos primero).
    /// </para>
    /// </remarks>
    public static class SqlResultMapper
    {
        /// <summary>
        /// Mapea una relación 1 a 1: tabla 0 = <typeparamref name="TParent"/> (1 fila), tabla 1 =
        /// <typeparamref name="TChild"/> (0 o 1 fila).
        /// </summary>
        /// <remarks>
        /// Resuelve la propiedad de navegación en <typeparamref name="TParent"/> a partir del
        /// <see cref="OneToOneAttribute"/> declarado hacia <typeparamref name="TChild"/>, salvo
        /// que se indique <paramref name="childPropertyName"/> explícitamente. Si la tabla 1 no
        /// trae ninguna fila (ej: una orden sin factura emitida todavía), la propiedad queda en
        /// su valor por defecto en vez de fallar — no toda relación 1 a 1 tiene siempre su
        /// contraparte.
        /// </remarks>
        /// <typeparam name="TParent">Tipo del lado "padre" (tabla 0).</typeparam>
        /// <typeparam name="TChild">Tipo del lado "hijo" (tabla 1), opcional en los datos.</typeparam>
        /// <param name="result">Resultado con al menos 2 tablas (padre y, opcionalmente, hijo).</param>
        /// <param name="childPropertyName">
        /// Nombre explícito de la propiedad de navegación en <typeparamref name="TParent"/>, para
        /// cuando el modelo no declara <see cref="OneToOneAttribute"/> o tenés más de una
        /// propiedad candidata.
        /// </param>
        /// <returns><typeparamref name="TParent"/> mapeado, con su propiedad de navegación asignada si hubo fila en la tabla 1.</returns>
        /// <exception cref="InvalidOperationException">
        /// El resultado tiene menos de 2 tablas; la tabla 0 no tiene ninguna fila; o no se pasó
        /// <paramref name="childPropertyName"/> y <typeparamref name="TParent"/> no declara
        /// <see cref="OneToOneAttribute"/> hacia <typeparamref name="TChild"/>.
        /// </exception>
        /// <example>
        /// <code>
        /// // sp devuelve 2 result sets: Order (1 fila) e Invoice (0 o 1 fila)
        /// var order = result.MapOneToOne&lt;Order, Invoice&gt;();
        /// Console.WriteLine(order.Invoice?.InvoiceNumber ?? "sin factura todavía");
        /// </code>
        /// </example>
        public static TParent MapOneToOne<TParent, TChild>(
            this SqlQueryResult result,
            string? childPropertyName = null)
            where TParent : new()
            where TChild : new()
        {
            var parentTable = RequireTable(result, 0, typeof(TParent));
            if (parentTable.Rows.Count == 0)
                throw new InvalidOperationException("No se encontró el registro padre.");

            var parent = MapSingleRow<TParent>(parentTable.Rows[0]);

            var childTable = RequireTable(result, 1, typeof(TChild));
            if (childTable.Rows.Count > 0)
            {
                var child = MapSingleRow<TChild>(childTable.Rows[0]);
                var property = ResolveNavigationProperty<TParent, TChild>(
                    childPropertyName,
                    static meta => meta.OneToOne,
                    "OneToOne");

                property.SetValue(parent, child);
            }

            return parent;
        }

        /// <summary>
        /// Mapea una relación 1 a muchos para un único padre: tabla 0 =
        /// <typeparamref name="TParent"/> (1 fila), tabla 1 = <typeparamref name="TChild"/>
        /// (0 a N filas, todas hijas de ese padre).
        /// </summary>
        /// <remarks>
        /// Resuelve la propiedad de navegación en <typeparamref name="TParent"/> a partir del
        /// <see cref="OneToManyAttribute"/> declarado hacia <typeparamref name="TChild"/>, salvo
        /// que se indique <paramref name="childPropertyName"/> explícitamente. Si el resultado
        /// trae varios padres a la vez (no solo uno), usá
        /// <see cref="MapMultipleOneToMany{TParent, TChild}"/> en su lugar — este método asume
        /// que <b>toda</b> la tabla 1 pertenece al único padre de la tabla 0, sin filtrar por
        /// clave.
        /// </remarks>
        /// <typeparam name="TParent">Tipo del lado "padre" (tabla 0, un único registro).</typeparam>
        /// <typeparam name="TChild">Tipo de los hijos (tabla 1).</typeparam>
        /// <param name="result">Resultado con al menos 2 tablas (un padre y sus hijos).</param>
        /// <param name="childPropertyName">
        /// Nombre explícito de la propiedad de navegación (colección) en
        /// <typeparamref name="TParent"/>, para cuando el modelo no declara
        /// <see cref="OneToManyAttribute"/> o tenés más de una propiedad candidata.
        /// </param>
        /// <returns><typeparamref name="TParent"/> mapeado, con su colección de hijos asignada (vacía si la tabla 1 no trajo filas).</returns>
        /// <exception cref="InvalidOperationException">
        /// El resultado tiene menos de 2 tablas; la tabla 0 no tiene ninguna fila; o no se pasó
        /// <paramref name="childPropertyName"/> y <typeparamref name="TParent"/> no declara
        /// <see cref="OneToManyAttribute"/> hacia <typeparamref name="TChild"/>.
        /// </exception>
        /// <example>
        /// <code>
        /// // sp devuelve 2 result sets: Order (1 fila) y OrderItems (0 a N filas)
        /// var order = result.MapOneToMany&lt;Order, OrderItem&gt;();
        /// Console.WriteLine($"{order.Items.Count} ítems");
        /// </code>
        /// </example>
        public static TParent MapOneToMany<TParent, TChild>(
            this SqlQueryResult result,
            string? childPropertyName = null)
            where TParent : new()
            where TChild : new()
        {
            var parentTable = RequireTable(result, 0, typeof(TParent));
            if (parentTable.Rows.Count == 0)
                throw new InvalidOperationException("No se encontró el registro padre.");

            var parent = MapSingleRow<TParent>(parentTable.Rows[0]);

            var childTable = RequireTable(result, 1, typeof(TChild));
            var children = MapRows<TChild>(childTable);

            var property = ResolveNavigationProperty<TParent, TChild>(
                childPropertyName,
                static meta => meta.OneToMany,
                "OneToMany");

            property.SetValue(parent, children);

            return parent;
        }

        /// <summary>
        /// Mapea una relación muchos a 1: tabla 0 = <typeparamref name="TChild"/> (0 a N filas),
        /// tabla 1 = <typeparamref name="TParent"/> (1 fila, compartida por todos los hijos).
        /// </summary>
        /// <remarks>
        /// Resuelve la propiedad de navegación en <typeparamref name="TChild"/> a partir del
        /// <see cref="ManyToOneAttribute"/> declarado hacia <typeparamref name="TParent"/>, salvo
        /// que se indique <paramref name="parentPropertyName"/> explícitamente. Un modelo puede
        /// declarar más de un <see cref="ManyToOneAttribute"/> en propiedades distintas — uno por
        /// cada tipo de padre posible (ej: una orden que pertenece a un usuario registrado <b>o</b>
        /// a un invitado, nunca ambos): pedí el <typeparamref name="TParent"/> que corresponda en
        /// cada llamada y se resuelve la propiedad correcta sin pisar la otra.
        /// </remarks>
        /// <typeparam name="TChild">Tipo de los hijos (tabla 0), cada uno recibe la misma referencia al padre.</typeparam>
        /// <typeparam name="TParent">Tipo del padre único compartido (tabla 1).</typeparam>
        /// <param name="result">Resultado con al menos 2 tablas (hijos y un único padre).</param>
        /// <param name="parentPropertyName">
        /// Nombre explícito de la propiedad de navegación en <typeparamref name="TChild"/>, para
        /// cuando el modelo no declara <see cref="ManyToOneAttribute"/> hacia
        /// <typeparamref name="TParent"/> o tenés más de una propiedad candidata del mismo tipo.
        /// </param>
        /// <returns>Los <typeparamref name="TChild"/> de la tabla 0, cada uno con la referencia al padre ya asignada.</returns>
        /// <exception cref="InvalidOperationException">
        /// El resultado tiene menos de 2 tablas; la tabla 1 (padre) no tiene ninguna fila; o no
        /// se pasó <paramref name="parentPropertyName"/> y <typeparamref name="TChild"/> no
        /// declara <see cref="ManyToOneAttribute"/> hacia <typeparamref name="TParent"/>.
        /// </exception>
        /// <example>
        /// <code>
        /// // sp devuelve 2 result sets: Orders del usuario (0 a N filas) y User (1 fila)
        /// var orders = result.MapManyToOne&lt;Order, User&gt;();
        /// </code>
        /// </example>
        public static List<TChild> MapManyToOne<TChild, TParent>(
            this SqlQueryResult result,
            string? parentPropertyName = null)
            where TChild : new()
            where TParent : new()
        {
            var childTable = RequireTable(result, 0, typeof(TChild));
            var children = MapRows<TChild>(childTable);

            var parentTable = RequireTable(result, 1, typeof(TParent));
            if (parentTable.Rows.Count == 0)
                throw new InvalidOperationException("No se encontró el registro padre.");

            var parent = MapSingleRow<TParent>(parentTable.Rows[0]);

            var property = ResolveNavigationProperty<TChild, TParent>(
                parentPropertyName,
                static meta => meta.ManyToOne,
                "ManyToOne");

            foreach (var child in children)
            {
                property.SetValue(child, parent);
            }

            return children;
        }

        /// <summary>
        /// Mapea una relación 1 a muchos con varios padres a la vez: tabla 0 =
        /// <typeparamref name="TParent"/> (0 a N filas), tabla 1 = <typeparamref name="TChild"/>
        /// (0 a N filas), agrupando cada hijo con su padre correspondiente por clave.
        /// </summary>
        /// <remarks>
        /// A diferencia de <see cref="MapOneToMany{TParent, TChild}"/> (un solo padre, toda la
        /// tabla 1 le pertenece), acá cada hijo se agrupa con su padre comparando
        /// <paramref name="parentKeyProperty"/> (por defecto, el <see cref="PrimaryKeyAttribute"/>
        /// de <typeparamref name="TParent"/>) contra <paramref name="childForeignKeyProperty"/>
        /// (por defecto, el <see cref="ForeignKeyAttribute"/> de <typeparamref name="TChild"/>
        /// hacia <typeparamref name="TParent"/>). Un padre sin hijos que le correspondan recibe
        /// una colección <b>vacía</b> (nunca <see langword="null"/>) en la propiedad de
        /// navegación, resuelta igual que en <see cref="MapOneToMany{TParent, TChild}"/> a partir
        /// del <see cref="OneToManyAttribute"/> hacia <typeparamref name="TChild"/> (o
        /// <paramref name="childPropertyName"/> si se indica).
        /// </remarks>
        /// <typeparam name="TParent">Tipo de los padres (tabla 0).</typeparam>
        /// <typeparam name="TChild">Tipo de los hijos (tabla 1).</typeparam>
        /// <param name="result">Resultado con al menos 2 tablas (todos los padres y todos los hijos).</param>
        /// <param name="childPropertyName">Nombre explícito de la propiedad de navegación (colección) en <typeparamref name="TParent"/>, si no seguís la convención de <see cref="OneToManyAttribute"/>.</param>
        /// <param name="parentKeyProperty">Nombre explícito de la propiedad clave en <typeparamref name="TParent"/>, si no seguís la convención de <see cref="PrimaryKeyAttribute"/>.</param>
        /// <param name="childForeignKeyProperty">Nombre explícito de la propiedad de clave foránea en <typeparamref name="TChild"/>, si no seguís la convención de <see cref="ForeignKeyAttribute"/>.</param>
        /// <returns>Los <typeparamref name="TParent"/> de la tabla 0, cada uno con su colección de hijos ya agrupada.</returns>
        /// <exception cref="InvalidOperationException">
        /// El resultado tiene menos de 2 tablas; no se pasó <paramref name="childPropertyName"/> y
        /// <typeparamref name="TParent"/> no declara <see cref="OneToManyAttribute"/> hacia
        /// <typeparamref name="TChild"/>; o falta el <see cref="PrimaryKeyAttribute"/>/
        /// <see cref="ForeignKeyAttribute"/> correspondiente y no se pasó el nombre explícito.
        /// </exception>
        /// <example>
        /// <code>
        /// // sp devuelve 2 result sets: todos los Users y todas las Orders (de todos los usuarios)
        /// var users = result.OneToManyMultiple&lt;User, Order&gt;();
        /// // users[i].Orders trae solo las órdenes de ese usuario; [] si no tiene ninguna
        /// </code>
        /// </example>
        public static List<TParent> MapMultipleOneToMany<TParent, TChild>(
            this SqlQueryResult result,
            string? childPropertyName = null,
            string? parentKeyProperty = null,
            string? childForeignKeyProperty = null)
            where TParent : new()
            where TChild : new()
        {
            var parents = MapRows<TParent>(RequireTable(result, 0, typeof(TParent)));
            var children = MapRows<TChild>(RequireTable(result, 1, typeof(TChild)));

            var childProperty = ResolveNavigationProperty<TParent, TChild>(
                childPropertyName,
                static meta => meta.OneToMany,
                "OneToMany");

            var parentKeyProp = parentKeyProperty is not null
                ? RequireProperty<TParent>(parentKeyProperty)
                : MetadataCache.RequirePrimaryKey(typeof(TParent));

            var childFkProp = childForeignKeyProperty is not null
                ? RequireProperty<TChild>(childForeignKeyProperty)
                : MetadataCache.RequireForeignKey(typeof(TChild), typeof(TParent));

            var childGroups = children.GroupBy(c => childFkProp.GetValue(c))
                                     .ToDictionary(g => g.Key!, g => g.ToList());

            foreach (var parent in parents)
            {
                var parentKey = parentKeyProp.GetValue(parent);
                if (childGroups.TryGetValue(parentKey!, out var childList))
                {
                    childProperty.SetValue(parent, childList);
                }
                else
                {
                    var emptyList = Activator.CreateInstance(childProperty.PropertyType);
                    childProperty.SetValue(parent, emptyList);
                }
            }

            return parents;
        }

        /// <summary>
        /// Mapea una relación muchos a muchos a través de una tabla de unión: tabla 0 =
        /// <typeparamref name="TLeft"/>, tabla 1 = <typeparamref name="TRight"/>, tabla 2 =
        /// <typeparamref name="TJoin"/> (la tabla de unión). Es la única de las cuatro relaciones
        /// que necesita 3 tablas en vez de 2.
        /// </summary>
        /// <remarks>
        /// Resuelve la propiedad de navegación en <typeparamref name="TLeft"/> a partir del
        /// <see cref="ManyToManyAttribute"/> declarado hacia <typeparamref name="TRight"/> (con
        /// <typeparamref name="TJoin"/> como tabla de unión), salvo que se indique
        /// <paramref name="leftCollectionProperty"/> explícitamente. Las claves para cruzar las
        /// tres tablas (<see cref="PrimaryKeyAttribute"/> de cada lado y
        /// <see cref="ForeignKeyAttribute"/> en <typeparamref name="TJoin"/> hacia cada lado) se
        /// resuelven solas salvo que se pasen explícitas con
        /// <paramref name="leftKey"/>/<paramref name="rightKey"/>/
        /// <paramref name="joinLeftKey"/>/<paramref name="joinRightKey"/>. Un
        /// <typeparamref name="TLeft"/> sin ninguna fila de unión que lo relacione recibe una
        /// colección vacía, igual que en <see cref="MapMultipleOneToMany{TParent, TChild}"/>.
        /// </remarks>
        /// <typeparam name="TLeft">Lado izquierdo (tabla 0); declara el <see cref="ManyToManyAttribute"/>.</typeparam>
        /// <typeparam name="TRight">Lado derecho (tabla 1).</typeparam>
        /// <typeparam name="TJoin">Tabla de unión (tabla 2); relaciona un <typeparamref name="TLeft"/> con un <typeparamref name="TRight"/> por fila.</typeparam>
        /// <param name="result">Resultado con al menos 3 tablas (izquierda, derecha y unión).</param>
        /// <param name="leftCollectionProperty">Nombre explícito de la propiedad de navegación (colección) en <typeparamref name="TLeft"/>, si no seguís la convención de <see cref="ManyToManyAttribute"/>.</param>
        /// <param name="leftKey">Nombre explícito de la propiedad clave en <typeparamref name="TLeft"/>, si no seguís la convención de <see cref="PrimaryKeyAttribute"/>.</param>
        /// <param name="rightKey">Nombre explícito de la propiedad clave en <typeparamref name="TRight"/>, si no seguís la convención de <see cref="PrimaryKeyAttribute"/>.</param>
        /// <param name="joinLeftKey">Nombre explícito de la propiedad en <typeparamref name="TJoin"/> que apunta a <typeparamref name="TLeft"/>, si no seguís la convención de <see cref="ForeignKeyAttribute"/>.</param>
        /// <param name="joinRightKey">Nombre explícito de la propiedad en <typeparamref name="TJoin"/> que apunta a <typeparamref name="TRight"/>, si no seguís la convención de <see cref="ForeignKeyAttribute"/>.</param>
        /// <returns>Los <typeparamref name="TLeft"/> de la tabla 0, cada uno con su colección de <typeparamref name="TRight"/> relacionados ya asignada.</returns>
        /// <exception cref="InvalidOperationException">
        /// El resultado tiene menos de 3 tablas; no se pasó <paramref name="leftCollectionProperty"/>
        /// y <typeparamref name="TLeft"/> no declara <see cref="ManyToManyAttribute"/> hacia
        /// <typeparamref name="TRight"/>/<typeparamref name="TJoin"/>; o falta algún
        /// <see cref="PrimaryKeyAttribute"/>/<see cref="ForeignKeyAttribute"/> requerido y no se
        /// pasó el nombre explícito correspondiente.
        /// </exception>
        /// <example>
        /// <code>
        /// // sp devuelve 3 result sets: Users, Roles, y UserRole (la tabla de unión)
        /// var users = result.MapManyToMany&lt;User, Role, UserRole&gt;();
        /// </code>
        /// </example>
        public static List<TLeft> MapManyToMany<TLeft, TRight, TJoin>(
            this SqlQueryResult result,
            string? leftCollectionProperty = null,
            string? leftKey = null,
            string? rightKey = null,
            string? joinLeftKey = null,
            string? joinRightKey = null)
            where TLeft : new()
            where TRight : new()
            where TJoin : new()
        {
            var lefts = MapRows<TLeft>(RequireTable(result, 0, typeof(TLeft)));
            var rights = MapRows<TRight>(RequireTable(result, 1, typeof(TRight)));
            var joins = MapRows<TJoin>(RequireTable(result, 2, typeof(TJoin)));

            var leftNavProp = ResolveNavigationProperty<TLeft, TRight>(
                leftCollectionProperty,
                static meta => meta.ManyToMany.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.NavProp),
                "ManyToMany");

            var leftKeyProp = leftKey is not null
                ? RequireProperty<TLeft>(leftKey)
                : MetadataCache.RequirePrimaryKey(typeof(TLeft));

            var rightKeyProp = rightKey is not null
                ? RequireProperty<TRight>(rightKey)
                : MetadataCache.RequirePrimaryKey(typeof(TRight));

            var joinLeftProp = joinLeftKey is not null
                ? RequireProperty<TJoin>(joinLeftKey)
                : MetadataCache.RequireForeignKey(typeof(TJoin), typeof(TLeft));

            var joinRightProp = joinRightKey is not null
                ? RequireProperty<TJoin>(joinRightKey)
                : MetadataCache.RequireForeignKey(typeof(TJoin), typeof(TRight));

            var rightDict = rights.ToDictionary(r => rightKeyProp.GetValue(r)!);

            foreach (var left in lefts)
            {
                var leftId = leftKeyProp.GetValue(left);
                var related = joins
                    .Where(j => Equals(joinLeftProp.GetValue(j), leftId))
                    .Select(j => rightDict[joinRightProp.GetValue(j)!])
                    .ToList();

                leftNavProp.SetValue(left, related);
            }

            return lefts;
        }

        private static PropertyInfo ResolveNavigationProperty<TOwner, TTarget>(
            string? explicitPropertyName,
            Func<EntityMetadata, Dictionary<Type, PropertyInfo>> selector,
            string relationName)
        {
            if (explicitPropertyName is not null)
            {
                return RequireProperty<TOwner>(explicitPropertyName);
            }

            var map = selector(MetadataCache.Get(typeof(TOwner)));
            if (!map.TryGetValue(typeof(TTarget), out var property))
            {
                throw new InvalidOperationException(
                    $"No se definió [{relationName}] hacia {typeof(TTarget).Name} en el modelo {typeof(TOwner).Name}");
            }

            return property;
        }

        private static PropertyInfo RequireProperty<T>(string propertyName)
        {
            return typeof(T).GetProperty(propertyName)
                ?? throw new ArgumentException($"Propiedad '{propertyName}' no encontrada en {typeof(T).Name}");
        }

        /// <summary>
        /// Obtiene la tabla en <paramref name="index"/> del resultado o lanza una excepción explicando<br></br>
        /// cuántas tablas se esperaban, para diferenciar este caso del genérico<br></br>
        /// <see cref="IndexOutOfRangeException"/> de <see cref="SqlQueryResult.AsDataTable(int)"/>.<br></br>
        /// </summary>
        private static DataTable RequireTable(SqlQueryResult result, int index, Type expectedType)
        {
            var tableCount = result.AsDataSet().Tables.Count;
            if (index >= tableCount)
            {
                throw new InvalidOperationException(
                    $"Se esperaba un resultado (tabla) en la posición {index} para mapear '{expectedType.Name}', " +
                    $"pero el procedimiento/consulta solo devolvió {tableCount} resultado(s). " +
                    "Verifica que el procedimiento almacenado tenga una sentencia SELECT independiente " +
                    "por cada tabla que el mapeo requiere.");
            }

            return result.AsDataTable(index);
        }

        /// <summary>
        /// Mapea una sola fila a un objeto, usando el nombre de columna declarado en <see cref="ColumnAttribute"/><br></br>
        /// cuando existe, o el nombre de la propiedad en su defecto.
        /// </summary>
        private static T MapSingleRow<T>(DataRow row) where T : new()
        {
            var obj = new T();
            var metadata = MetadataCache.Get(typeof(T));

            foreach (var (columnName, prop) in metadata.Columns)
            {
                if (row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
                {
                    try
                    {
                        var value = row[columnName];
                        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        prop.SetValue(obj, Convert.ChangeType(value, targetType));
                    }
                    catch
                    {
                        // Ignorar errores de conversión
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// Mapea múltiples filas a una lista de objetos.
        /// </summary>
        private static List<T> MapRows<T>(DataTable table) where T : new()
        {
            var list = new List<T>();

            foreach (DataRow row in table.Rows)
            {
                list.Add(MapSingleRow<T>(row));
            }

            return list;
        }
    }

    /// <summary>
    /// Extension methods para SqlQueryResult.
    /// </summary>
    public static class SqlQueryResultExtensions
    {
        /// <inheritdoc cref="SqlResultMapper.MapOneToOne{TParent, TChild}"/>
        public static TParent OneToOne<TParent, TChild>(
            this SqlQueryResult result,
            string? childPropertyName = null)
            where TParent : new()
            where TChild : new()
        {
            return SqlResultMapper.MapOneToOne<TParent, TChild>(result, childPropertyName);
        }

        /// <inheritdoc cref="SqlResultMapper.MapOneToMany{TParent, TChild}"/>
        public static TParent OneToMany<TParent, TChild>(
            this SqlQueryResult result,
            string? childPropertyName = null)
            where TParent : new()
            where TChild : new()
        {
            return SqlResultMapper.MapOneToMany<TParent, TChild>(result, childPropertyName);
        }

        /// <inheritdoc cref="SqlResultMapper.MapManyToOne{TChild, TParent}"/>
        public static List<TChild> ManyToOne<TChild, TParent>(
            this SqlQueryResult result,
            string? parentPropertyName = null)
            where TChild : new()
            where TParent : new()
        {
            return SqlResultMapper.MapManyToOne<TChild, TParent>(result, parentPropertyName);
        }

        /// <inheritdoc cref="SqlResultMapper.MapMultipleOneToMany{TParent, TChild}"/>
        public static List<TParent> OneToManyMultiple<TParent, TChild>(
            this SqlQueryResult result,
            string? childPropertyName = null,
            string? parentKeyProperty = null,
            string? childForeignKeyProperty = null)
            where TParent : new()
            where TChild : new()
        {
            return SqlResultMapper.MapMultipleOneToMany<TParent, TChild>(
                result,
                childPropertyName,
                parentKeyProperty,
                childForeignKeyProperty);
        }

        /// <inheritdoc cref="SqlResultMapper.MapManyToMany{TLeft, TRight, TJoin}"/>
        public static List<TLeft> ManyToMany<TLeft, TRight, TJoin>(
            this SqlQueryResult result,
            string? leftCollectionProperty = null,
            string? leftKey = null,
            string? rightKey = null,
            string? joinLeftKey = null,
            string? joinRightKey = null)
            where TLeft : new()
            where TRight : new()
            where TJoin : new()
        {
            return SqlResultMapper.MapManyToMany<TLeft, TRight, TJoin>(
                result,
                leftCollectionProperty,
                leftKey,
                rightKey,
                joinLeftKey,
                joinRightKey);
        }
    }
}
