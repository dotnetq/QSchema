using System;

namespace QTools.Schema
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
