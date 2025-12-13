using System.Data;
using System.Reflection;
using DBSQLClient.Conexion;
using DBSQLClient.Servicio.Mapper.RelationsMapper;


namespace DBSQLClient.Servicio.Mapper.RelationsMapper
{
    /// <summary>
    /// Helper para mapear resultados con relaciones padre-hijo.
    /// </summary>
    public static class SqlResultMapper
    {
        /// <summary>
        /// Mapea un resultado con relación uno-a-muchos.
        /// </summary>
        /// <typeparam name="TParent">Tipo del objeto padre.</typeparam>
        /// <typeparam name="TChild">Tipo del objeto hijo.</typeparam>
        /// <param name="result">Resultado del SP.</param>
        /// <param name="childPropertyName">Nombre de la propiedad de navegación (ej: "Roles").</param>
        /// <returns>Objeto padre con hijos mapeados.</returns>
        public static TParent MapOneToMany<TParent, TChild>(
            this SqlQueryResult result,
            string childPropertyName)
            where TParent : new()
            where TChild : new()
        {
            // Tabla 1: Padre
            var parentTable = result.AsDataTable(0);
            if (parentTable.Rows.Count == 0)
                throw new InvalidOperationException("No se encontró el registro padre.");

            var parent = MapSingleRow<TParent>(parentTable.Rows[0]);

            // Tabla 2: Hijos
            var childTable = result.AsDataTable(1);
            var children = MapRows<TChild>(childTable);

            // Asignar hijos al padre
            var property = typeof(TParent).GetProperty(childPropertyName);
            if (property == null)
                throw new ArgumentException($"Propiedad '{childPropertyName}' no encontrada en {typeof(TParent).Name}");

            property.SetValue(parent, children);

            return parent;
        }

        /// <summary>
        /// Mapea múltiples resultados con relación uno-a-muchos.
        /// </summary>
        public static List<TParent> MapMultipleOneToMany<TParent, TChild>(
            this SqlQueryResult result,
            string childPropertyName,
            string parentKeyProperty = "Id",
            string childForeignKeyProperty = "ParentId")
            where TParent : new()
            where TChild : new()
        {
            // Tabla 1: Padres
            var parentTable = result.AsDataTable(0);
            var parents = MapRows<TParent>(parentTable);

            // Tabla 2: Hijos
            var childTable = result.AsDataTable(1);
            var children = MapRows<TChild>(childTable);

            // Propiedades de navegación
            var childProperty = typeof(TParent).GetProperty(childPropertyName);
            if (childProperty == null)
                throw new ArgumentException($"Propiedad '{childPropertyName}' no encontrada");

            var parentKeyProp = typeof(TParent).GetProperty(parentKeyProperty);
            var childFkProp = typeof(TChild).GetProperty(childForeignKeyProperty);

            // Agrupar hijos por padre
            var childGroups = children.GroupBy(c => childFkProp?.GetValue(c))
                                     .ToDictionary(g => g.Key!, g => g.ToList()) ;

            // Asignar hijos a cada padre
            foreach (var parent in parents)
            {
                var parentKey = parentKeyProp?.GetValue(parent);
                if (childGroups.TryGetValue(parentKey!, out var childList))
                {
                    childProperty.SetValue(parent, childList);
                }
                else
                {
                    // Crear lista vacía si no hay hijos
                    var emptyList = Activator.CreateInstance(childProperty.PropertyType);
                    childProperty.SetValue(parent, emptyList);
                }
            }

            return parents;
        }

        /// <summary>
        /// Mapea una sola fila a un objeto.
        /// </summary>
        private static T MapSingleRow<T>(DataRow row) where T : new()
        {
            var obj = new T();
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                if (row.Table.Columns.Contains(prop.Name) && row[prop.Name] != DBNull.Value)
                {
                    try
                    {
                        var value = row[prop.Name];
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TJoin"></typeparam>
        /// <param name="result"></param>
        /// <param name="leftCollectionProperty"></param>
        /// <param name="leftKey"></param>
        /// <param name="rightKey"></param>
        /// <param name="joinLeftKey"></param>
        /// <param name="joinRightKey"></param>
        /// <returns></returns>
        /// <example><c>var users = result.MapManyToMany<User, Role, UserRole>(leftCollectionProperty: "Roles",joinLeftKey: "UserId",joinRightKey: "RoleId")</c></example>
        public static List<TLeft> MapManyToMany<TLeft, TRight, TJoin>(
            this SqlQueryResult result,
            string leftCollectionProperty,
            string leftKey = "Id",
            string rightKey = "Id",
            string joinLeftKey = "LeftId",
            string joinRightKey = "RightId")
            where TLeft : new()
            where TRight : new()
            where TJoin : new()
        {
            var lefts = MapRows<TLeft>(result.AsDataTable(0));
            var rights = MapRows<TRight>(result.AsDataTable(1));
            var joins = MapRows<TJoin>(result.AsDataTable(2));

            var leftKeyProp = typeof(TLeft).GetProperty(leftKey);
            var rightKeyProp = typeof(TRight).GetProperty(rightKey);
            var joinLeftProp = typeof(TJoin).GetProperty(joinLeftKey);
            var joinRightProp = typeof(TJoin).GetProperty(joinRightKey);

            var rightDict = rights.ToDictionary(
                r => rightKeyProp!.GetValue(r)!
            );

            var leftNavProp = typeof(TLeft).GetProperty(leftCollectionProperty);

            foreach (var left in lefts)
            {
                var leftId = leftKeyProp!.GetValue(left);
                var related = joins
                    .Where(j => Equals(joinLeftProp!.GetValue(j), leftId))
                    .Select(j => rightDict[joinRightProp!.GetValue(j)!])
                    .ToList();

                leftNavProp!.SetValue(left, related);
            }

            return lefts;
        }




        public static TParent MapOneToOne<TParent, TChild>(
           this SqlQueryResult result,
           string childPropertyName)
           where TParent : new()
           where TChild : new()
        {
            var parentTable = result.AsDataTable(0);
            var childTable = result.AsDataTable(1);

            if (parentTable.Rows.Count == 0)
                throw new InvalidOperationException("No se encontró el registro padre.");

            var parent = MapSingleRow<TParent>(parentTable.Rows[0]);

            if (childTable.Rows.Count > 0)
            {
                var child = MapSingleRow<TChild>(childTable.Rows[0]);
                var prop = typeof(TParent).GetProperty(childPropertyName)
                    ?? throw new ArgumentException($"Propiedad '{childPropertyName}' no encontrada");

                prop.SetValue(parent, child);
            }

            return parent;
        }

        private static void SetEmptyCollection(object parent, PropertyInfo prop)
        {
            var list = Activator.CreateInstance(prop.PropertyType);
            prop.SetValue(parent, list);
        }

    }
}

    /// <summary>
    /// Extension methods para SqlQueryResult.
    /// </summary>
    public static class SqlQueryResultExtensions
    {
        /// <summary>
        /// Mapea el resultado a un objeto con relación uno-a-muchos.
        /// </summary>
        /// <example>
        /// var user = result.ToSingleWithChildren&lt;UserModel, UserRol&gt;("Roles");
        /// </example>
        public static TParent OneToMany<TParent, TChild>(
            this SqlQueryResult result,
            string childPropertyName)
            where TParent : new()
            where TChild : new()
        {
            return SqlResultMapper.MapOneToMany<TParent, TChild>(result, childPropertyName);
        }

        /// <summary>
        /// Mapea el resultado a múltiples objetos con relaciones uno-a-muchos.
        /// </summary>
        public static List<TParent> ManyToMany<TParent, TChild>(
            this SqlQueryResult result,
            string childPropertyName,
            string parentKeyProperty = "Id",
            string childForeignKeyProperty = "ParentId")
            where TParent : new()
            where TChild : new()
        {
            return SqlResultMapper.MapMultipleOneToMany<TParent, TChild>(
                result,
                childPropertyName,
                parentKeyProperty,
                childForeignKeyProperty);
        }

    }
