using System;

namespace DotnetQ.QSchema.Attributes
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
