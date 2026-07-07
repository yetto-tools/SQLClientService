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
    public static class SqlResultMapper
    {
        /// <summary>
        /// Mapea un resultado con relación uno-a-uno (tabla 0 = padre, tabla 1 = hijo).
        /// </summary>
        public static TParent MapOneToOne<TParent, TChild>(
            this SqlQueryResult result,
            string? childPropertyName = null)
            where TParent : new()
            where TChild : new()
        {
            var parentTable = result.AsDataTable(0);
            if (parentTable.Rows.Count == 0)
                throw new InvalidOperationException("No se encontró el registro padre.");

            var parent = MapSingleRow<TParent>(parentTable.Rows[0]);

            var childTable = result.AsDataTable(1);
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
        /// Mapea un resultado con relación uno-a-muchos (tabla 0 = padre, tabla 1 = hijos).
        /// </summary>
        public static TParent MapOneToMany<TParent, TChild>(
            this SqlQueryResult result,
            string? childPropertyName = null)
            where TParent : new()
            where TChild : new()
        {
            var parentTable = result.AsDataTable(0);
            if (parentTable.Rows.Count == 0)
                throw new InvalidOperationException("No se encontró el registro padre.");

            var parent = MapSingleRow<TParent>(parentTable.Rows[0]);

            var childTable = result.AsDataTable(1);
            var children = MapRows<TChild>(childTable);

            var property = ResolveNavigationProperty<TParent, TChild>(
                childPropertyName,
                static meta => meta.OneToMany,
                "OneToMany");

            property.SetValue(parent, children);

            return parent;
        }

        /// <summary>
        /// Mapea una relación muchos-a-uno (tabla 0 = hijos, tabla 1 = padre único compartido).
        /// </summary>
        /// <example><c>var orders = result.MapManyToOne&lt;Order, User&gt;();</c></example>
        public static List<TChild> MapManyToOne<TChild, TParent>(
            this SqlQueryResult result,
            string? parentPropertyName = null)
            where TChild : new()
            where TParent : new()
        {
            var childTable = result.AsDataTable(0);
            var children = MapRows<TChild>(childTable);

            var parentTable = result.AsDataTable(1);
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
        /// Mapea múltiples resultados con relación uno-a-muchos (tabla 0 = padres, tabla 1 = hijos).
        /// </summary>
        public static List<TParent> MapMultipleOneToMany<TParent, TChild>(
            this SqlQueryResult result,
            string? childPropertyName = null,
            string? parentKeyProperty = null,
            string? childForeignKeyProperty = null)
            where TParent : new()
            where TChild : new()
        {
            var parents = MapRows<TParent>(result.AsDataTable(0));
            var children = MapRows<TChild>(result.AsDataTable(1));

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
        /// Mapea una relación muchos-a-muchos a través de una tabla de unión
        /// (tabla 0 = lado izquierdo, tabla 1 = lado derecho, tabla 2 = tabla de unión).
        /// </summary>
        /// <example><c>var users = result.MapManyToMany&lt;User, Role, UserRole&gt;();</c></example>
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
            var lefts = MapRows<TLeft>(result.AsDataTable(0));
            var rights = MapRows<TRight>(result.AsDataTable(1));
            var joins = MapRows<TJoin>(result.AsDataTable(2));

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
        /// Mapea una sola fila a un objeto, usando el nombre de columna declarado en <see cref="ColumnAttribute"/>
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
        /// <summary>
        /// Mapea el resultado a un objeto con relación uno-a-uno.
        /// </summary>
        public static TParent OneToOne<TParent, TChild>(
            this SqlQueryResult result,
            string? childPropertyName = null)
            where TParent : new()
            where TChild : new()
        {
            return SqlResultMapper.MapOneToOne<TParent, TChild>(result, childPropertyName);
        }

        /// <summary>
        /// Mapea el resultado a un objeto con relación uno-a-muchos.
        /// </summary>
        /// <example>
        /// var user = result.OneToMany&lt;UserModel, UserRol&gt;("Roles");
        /// </example>
        public static TParent OneToMany<TParent, TChild>(
            this SqlQueryResult result,
            string? childPropertyName = null)
            where TParent : new()
            where TChild : new()
        {
            return SqlResultMapper.MapOneToMany<TParent, TChild>(result, childPropertyName);
        }

        /// <summary>
        /// Mapea el resultado a una lista de hijos con relación muchos-a-uno hacia un único padre compartido.
        /// </summary>
        public static List<TChild> ManyToOne<TChild, TParent>(
            this SqlQueryResult result,
            string? parentPropertyName = null)
            where TChild : new()
            where TParent : new()
        {
            return SqlResultMapper.MapManyToOne<TChild, TParent>(result, parentPropertyName);
        }

        /// <summary>
        /// Mapea múltiples resultados con relaciones uno-a-muchos (varios padres, cada uno con sus hijos).
        /// </summary>
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

        /// <summary>
        /// Mapea el resultado a una relación muchos-a-muchos a través de una tabla de unión.
        /// </summary>
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
