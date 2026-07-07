using System;
using System.Collections.Generic;
using System.Reflection;


namespace DBSQLClient.Servicio.Mapper
{
    /// <summary>
    /// Representa la metadata de una entidad, incluyendo su tipo, clave primaria, nombre de tabla, nombre de procedimiento almacenado, columnas y relaciones con otras entidades.
    /// </summary>
    internal sealed class EntityMetadata
    {
        /// <summary>
        /// Obtiene el tipo de la entidad para la cual se está almacenando la metadata.
        /// </summary>
        public required Type EntityType { get; init; }

        /// <summary>
        /// Obtiene la propiedad que representa la clave primaria de la entidad. Puede ser nula si el tipo no declara un atributo [PrimaryKey] (por ejemplo, en tablas de unión como UserRole).
        /// </summary>
        // Nula cuando el tipo no declara [PrimaryKey] (ej: tablas de unión como UserRole).
        public PropertyInfo? PrimaryKey { get; init; }

        /// <summary>
        /// Obtiene el nombre de la tabla asociado a la entidad, según el atributo [Table]. Puede ser nulo si el modelo no representa una tabla.
        /// </summary>
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
