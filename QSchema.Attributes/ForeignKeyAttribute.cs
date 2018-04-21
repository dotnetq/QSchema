using System;

namespace DotnetQ.QSchema.Attributes
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
