using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetQ.QSchema.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class SortedAttribute : SchemaAttribute
    {
        public override string InQ() => "`s#";
    }
}

