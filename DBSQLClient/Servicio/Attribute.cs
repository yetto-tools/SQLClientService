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

}
