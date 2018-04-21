using System;

namespace DotnetQ.QSchema.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class PartedAttribute : SchemaAttribute
    {
        public override string InQ() => "`p#";
    }
}
