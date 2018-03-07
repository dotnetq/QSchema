using System;
using System.Collections.Generic;
using System.Text;

namespace QTools.Schema
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class SortedAttribute : SchemaAttribute
    {
        public override string InQ() => "`s#";
    }
}

