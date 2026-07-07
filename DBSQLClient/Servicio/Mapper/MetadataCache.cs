using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;


namespace DBSQLClient.Servicio.Mapper
{

    internal static class MetadataCache
    {
        private static readonly ConcurrentDictionary<Type, EntityMetadata> _cache = new();

        public static EntityMetadata Get(Type type)
        {
            return _cache.GetOrAdd(type, BuildMetadata);
        }

        internal static PropertyInfo RequirePrimaryKey(Type type)
        {
            return Get(type).PrimaryKey
                ?? throw new InvalidOperationException(
                    $"No se definió [PrimaryKey] en el modelo {type.Name}");
        }

        internal static PropertyInfo RequireForeignKey(Type type, Type referenceType)
        {
            return Get(type).ForeignKeys.TryGetValue(referenceType, out var prop)
                ? prop
                : throw new InvalidOperationException(
                    $"No se definió [ForeignKey] hacia {referenceType.Name} en el modelo {type.Name}");
        }

        private static EntityMetadata BuildMetadata(Type type)
        {
            var metadata = new EntityMetadata
            {
                EntityType = type,
                PrimaryKey = type.GetProperties()
                    .FirstOrDefault(p => p.GetCustomAttribute<PrimaryKeyAttribute>() != null)
            };

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var columnName = prop.GetCustomAttribute<ColumnAttribute>()?.Name ?? prop.Name;
                metadata.Columns[columnName] = prop;

                var fk = prop.GetCustomAttribute<ForeignKeyAttribute>();
                if (fk != null)
                {
                    metadata.ForeignKeys[fk.ReferenceType] = prop;
                }

                var otm = prop.GetCustomAttribute<OneToManyAttribute>();
                if (otm != null)
                {
                    metadata.OneToMany[otm.ChildType] = prop;
                }

                var mto = prop.GetCustomAttribute<ManyToOneAttribute>();
                if (mto != null)
                {
                    metadata.ManyToOne[mto.ParentType] = prop;
                }

                var mtm = prop.GetCustomAttribute<ManyToManyAttribute>();
                if (mtm != null)
                {
                    metadata.ManyToMany[mtm.TargetType] = (prop, mtm.JoinType);
                }

                var oto = prop.GetCustomAttribute<OneToOneAttribute>();
                if (oto != null)
                {
                    metadata.OneToOne[oto.TargetType] = prop;
                }
            }

            return metadata;
        }
    }

}
