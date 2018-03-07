namespace QTools
{
    public abstract class QSchema
    {
        public const string EmptyList = "()";
        public const string Cast = "$";

        public abstract string QType { get; }
        public abstract string QCharType { get; }
        public abstract string QNullValue { get; }
        public abstract string QNotation(object o);

        protected string QNotationImpl<T>(object o)
        {
            if (o != null && o is T value)
            {
                return $"{value}{QCharType}";
            }
            return QNullValue;
        }

        protected virtual bool IsAtom => true;
        protected virtual string ColumnType => IsAtom?string.Concat(QCharType, Cast):QType;

        public string AsTableColumn(string name, string qAttributes = "") => string.Concat(qAttributes,(IsAtom?"`":""), name, ":", ColumnType, EmptyList);
    }
}
