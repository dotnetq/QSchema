using System;

namespace QTools.Schema
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public abstract class SchemaAttribute : Attribute
    {
        public abstract string InQ();
    }
}
