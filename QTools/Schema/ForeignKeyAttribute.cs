using System;

namespace QTools.Schema
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ForeignKeyAttribute : Attribute
    {
        public Type ForeignType { get; }

        public ForeignKeyAttribute(Type foreignType)
        {
            ForeignType = foreignType;
        }
    }
}
