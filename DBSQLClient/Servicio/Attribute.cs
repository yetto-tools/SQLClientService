using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DBSQLClient.Servicio;

namespace DBSQLClient.Servicio
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PrimaryKeyAttribute : Attribute
    {
    }
    
    
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ForeignKeyAttribute : Attribute
    {
        public Type ReferenceType { get; }

        public ForeignKeyAttribute(Type referenceType)
        {
            ReferenceType = referenceType;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OneToManyAttribute : Attribute
    {
        public Type ChildType { get; }

        public OneToManyAttribute(Type childType)
        {
            ChildType = childType;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ManyToOneAttribute : Attribute
    {
        public Type ParentType { get; }

        public ManyToOneAttribute(Type parentType)
        {
            ParentType = parentType;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ManyToManyAttribute : Attribute
    {
        public Type TargetType { get; }
        public Type JoinType { get; }

        public ManyToManyAttribute(Type targetType, Type joinType)
        {
            TargetType = targetType;
            JoinType = joinType;
        }
    }


    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OneToOneAttribute : Attribute
    {
        public Type TargetType { get; }

        public OneToOneAttribute(Type targetType)
        {
            TargetType = targetType;
        }
    }


    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ColumnAttribute : Attribute
    {
        public string Name { get; }
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
        public string Name { get; }

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
        public string Name { get; }

        public StoredProcedureAttribute(string name)
        {
            Name = name;
        }
    }

}
