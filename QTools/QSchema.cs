namespace QTools
{
    public abstract class QSchema
    {
        public const string EmptyList = "()";

        public const string Cast = "$";

        public abstract string QType { get; }

        public abstract string QNotation(object o);

        public string AsTableColumn(string name, string qAttributes = "") => string.Concat(qAttributes, name, ":", ColumnType, EmptyList);

        protected virtual bool IsAtom => true;

        private string ColumnType => IsAtom ? string.Concat("`", QType, Cast) : QType;
    }
}
