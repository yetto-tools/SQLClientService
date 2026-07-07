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
    /// Indica que la propiedad representa una relación uno a muchos con otra entidad.
    /// </summary>
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
    /// Indica que la propiedad representa una relación muchos a uno con otra entidad.
    /// </summary>
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
    /// Indica que la propiedad representa una relación muchos a muchos con otra entidad, utilizando una tabla de unión.
    /// </summary>
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
    /// Indica que la propiedad representa una relación uno a uno con otra entidad.
    /// </summary>
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
