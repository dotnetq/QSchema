using System;

namespace QTools.Schema
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class CircularKeyAttribute : Attribute
    { }
}
