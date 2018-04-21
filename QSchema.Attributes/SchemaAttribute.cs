using System;

namespace DotnetQ.QSchema.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public abstract class SchemaAttribute : Attribute
    {
        public abstract string InQ();
    }
}
