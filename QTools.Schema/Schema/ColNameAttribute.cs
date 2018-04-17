using System;

namespace QTools.Schema
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ColNameAttribute : Attribute
    {
        public ColNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
