using System;
using System.Collections.Generic;
using System.Reflection;


namespace DBSQLClient.Servicio.Mapper
{

    internal sealed class EntityMetadata
    {
        public Type EntityType { get; init; }

        public PropertyInfo PrimaryKey { get; init; }

        // FK hacia otros tipos (key = tipo destino)
        public Dictionary<Type, PropertyInfo> ForeignKeys { get; } = new();

        // Navegaciones
        public Dictionary<Type, PropertyInfo> OneToMany { get; } = new();
        public Dictionary<Type, PropertyInfo> ManyToOne { get; } = new();
        public Dictionary<Type, (PropertyInfo NavProp, Type JoinType)> ManyToMany { get; } = new();
        public Dictionary<Type, PropertyInfo> OneToOne { get; } = new();
    }

}
