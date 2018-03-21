using System;

namespace QTools.Schema
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class PartedAttribute : SchemaAttribute
    {
        public override string InQ() => "`p#";
    }
}
