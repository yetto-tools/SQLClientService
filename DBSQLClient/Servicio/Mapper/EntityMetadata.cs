using System;
using System.Collections.Generic;
using System.Reflection;


namespace DBSQLClient.Servicio.Mapper
{

    internal sealed class EntityMetadata
    {
        public Type EntityType { get; init; }

        // Nula cuando el tipo no declara [PrimaryKey] (ej: tablas de unión como UserRole).
        public PropertyInfo? PrimaryKey { get; init; }

        // Nombre de tabla desde [Table]. Nula si el modelo no representa una tabla.
        public string? TableName { get; init; }

        // Nombre de procedimiento almacenado desde [StoredProcedure]. Nula si no aplica.
        public string? StoredProcedureName { get; init; }

        // Nombre de columna (o nombre de propiedad si no hay [Column]) -> propiedad.
        public Dictionary<string, PropertyInfo> Columns { get; } = new(StringComparer.OrdinalIgnoreCase);

        // FK hacia otros tipos (key = tipo destino)
        public Dictionary<Type, PropertyInfo> ForeignKeys { get; } = new();

        // Navegaciones
        public Dictionary<Type, PropertyInfo> OneToMany { get; } = new();
        public Dictionary<Type, PropertyInfo> ManyToOne { get; } = new();
        public Dictionary<Type, (PropertyInfo NavProp, Type JoinType)> ManyToMany { get; } = new();
        public Dictionary<Type, PropertyInfo> OneToOne { get; } = new();
    }

}
