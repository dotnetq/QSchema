using System;

namespace DotnetQ.QSchema.Attributes
{
    public class IndexAttribute : Attribute
    {
        public IndexAttribute(int indexGroupId)
        {
            IndexGroupId = indexGroupId;
        }

        public int IndexGroupId { get; }
    }
}
