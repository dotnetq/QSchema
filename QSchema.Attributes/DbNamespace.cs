using System;

namespace DotnetQ.QSchema.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DbNamespaceAttribute : Attribute
    {
        public DbNamespaceAttribute(string name = "")
        {
            Name = name;
        }

        public string Name { get; }
    }
}
