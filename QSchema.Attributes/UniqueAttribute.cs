using System;

namespace DotnetQ.QSchema.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class UniqueAttribute : SchemaAttribute
    {
        public override string InQ() => "`u#";
    }
}
