using System;

namespace DotnetQ.QSchema.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class GroupedAttribute : SchemaAttribute
    {
        public override string InQ() => "`g#";
    }
}
