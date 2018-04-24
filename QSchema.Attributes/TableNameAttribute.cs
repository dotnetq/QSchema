using System;

namespace DotnetQ.QSchema.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableNameAttribute : Attribute
    {
        public TableNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
