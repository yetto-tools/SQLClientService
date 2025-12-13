using System;
using System.Collections.Concurrent;
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


        internal static PropertyInfo GetPrimaryKey(Type type)
        {
            return type.GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                ?? throw new InvalidOperationException(
                    $"No se definió [PrimaryKey] en el modelo {type.Name}");
        }

        internal static PropertyInfo GetForeignKey(Type type, Type referenceType)
        {
            return type.GetProperties()
                .FirstOrDefault(p =>
                {
                    var attr = p.GetCustomAttribute<ForeignKeyAttribute>();
                    return attr != null && attr.ReferenceType == referenceType;
                })
                ?? throw new InvalidOperationException(
                    $"No se definió [ForeignKey] hacia {referenceType.Name} en el modelo {type.Name}");
        }


        private static EntityMetadata BuildMetadata(Type type)
        {
            var metadata = new EntityMetadata
            {
                EntityType = type,
                PrimaryKey = ResolvePrimaryKey(type)
            };

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // ForeignKey
                var fk = prop.GetCustomAttribute<ForeignKeyAttribute>();
                if (fk != null)
                {
                    metadata.ForeignKeys[fk.ReferenceType] = prop;
                    continue;
                }

                // OneToMany
                var otm = prop.GetCustomAttribute<OneToManyAttribute>();
                if (otm != null)
                {
                    metadata.OneToMany[otm.ChildType] = prop;
                    continue;
                }

                // ManyToOne
                var mto = prop.GetCustomAttribute<ManyToOneAttribute>();
                if (mto != null)
                {
                    metadata.ManyToOne[mto.ParentType] = prop;
                    continue;
                }

                // ManyToMany
                var mtm = prop.GetCustomAttribute<ManyToManyAttribute>();
                if (mtm != null)
                {
                    metadata.ManyToMany[mtm.TargetType] = (prop, mtm.JoinType);
                    continue;
                }

                // OneToOne
                var oto = prop.GetCustomAttribute<OneToOneAttribute>();
                if (oto != null)
                {
                    metadata.OneToOne[oto.TargetType] = prop;
                    continue;
                }
            }

            return metadata;
        }

        private static PropertyInfo ResolvePrimaryKey(Type type)
        {
            return type.GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                ?? throw new InvalidOperationException(
                    $"No se definió [PrimaryKey] en {type.Name}");
        }
    
    }

}
